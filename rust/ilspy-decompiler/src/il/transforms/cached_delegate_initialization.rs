//! `IL/Transforms/CachedDelegateInitialization.cs`: the compilers cache delegates of
//! lambdas and method groups in static fields or locals; the cache checks are removed so
//! the delegate construction remains.

use crate::il::ast::{ComparisonKind, ConversionKind, Function, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::resolve::ResolvedField;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::delegate_construction::match_delegate_construction;
use super::il_inlining::{inline_one_if_possible, options};

/// `IBlockTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    if !ctx.settings.anonymous_methods {
        return;
    }
    let block = bc.block;
    let mut i = bc.index_of_first_already_transformed_instruction;
    while i > 0 {
        i -= 1;
        let f = &mut r.function;
        let inst = f.list(block)[i];
        if !matches!(f.inst(inst), Inst::If { .. }) {
            continue;
        }
        let changed = if with_field(f, ctx, block, i, inst) {
            true
        } else if with_local(f, ctx, block, i, inst) {
            inline_one_if_possible(f, block, i, options::AGGRESSIVE, ctx);
            true
        } else if roslyn_with_local(f, ctx, block, i, inst, true)
            || roslyn_with_local(f, ctx, block, i, inst, false)
        {
            f.remove_at(block, i);
            true
        } else if vb(f, ctx, inst) {
            true
        } else if vb_with_return(f, ctx, block, i, inst) {
            f.remove_at(block, i);
            true
        } else {
            vb_with_closure(f, ctx, inst)
        };
        if changed {
            bc.index_of_first_already_transformed_instruction = r.function.list(block).len();
        }
    }
}

fn same_field(a: &ResolvedField, b: &ResolvedField) -> bool {
    a.origin == b.origin && a.declaring == b.declaring
}

/// The parts of `if (condition) true_inst` without an else branch, when `true_inst` is a
/// block of one instruction: the condition and that instruction.
fn if_with_single_statement(f: &Function, inst: NodeId) -> Option<(NodeId, NodeId)> {
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    if !matches!(f.inst(true_inst), Inst::Block { .. }) || !f.match_nop(false_inst) {
        return None;
    }
    let &[statement] = f.list(true_inst) else {
        return None;
    };
    Some((condition, statement))
}

/// `MatchStsFld`: `stobj(ldsflda F, value)`, neither volatile nor unaligned.
fn match_stsfld(f: &Function, id: NodeId) -> Option<(&ResolvedField, NodeId)> {
    let Inst::StObj {
        target,
        value,
        is_volatile: false,
        unaligned: 0,
        ..
    } = *f.inst(id)
    else {
        return None;
    };
    match f.inst(target) {
        Inst::LdsFlda { field } => Some((&field.field, value)),
        _ => None,
    }
}

/// `UnwrapConv(ConversionKind.Invalid)`.
fn unwrap_invalid_conv(f: &Function, mut inst: NodeId) -> NodeId {
    while let Inst::Conv {
        arg,
        input,
        input_sign,
        target,
        ..
    } = *f.inst(inst)
        && crate::il::ast::conversion_kind(target, input, input_sign) == ConversionKind::Invalid
    {
        inst = arg;
    }
    inst
}

/// `inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + offset)` in a block.
fn sibling(f: &Function, block: NodeId, i: usize, offset: isize) -> Option<NodeId> {
    let index = i.checked_add_signed(offset)?;
    f.list(block).get(index).copied()
}

/// `CachedDelegateInitializationWithField`: `if (ldsfld F == null) stsfld F(delegate)`
/// before the one use of `ldsfld F`.
fn with_field(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
    inst: NodeId,
) -> bool {
    let Some((condition, store)) = if_with_single_statement(f, inst) else {
        return false;
    };
    let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
        return false;
    };
    let Some(field) = f.match_ldsfld(left).map(|o| o.field.clone()) else {
        return false;
    };
    if !matches!(f.inst(right), Inst::LdNull) {
        return false;
    }
    let Some((field2, value)) = match_stsfld(f, store) else {
        return false;
    };
    if !same_field(&field, field2)
        || !ctx
            .ts
            .field_is_compiler_generated_or_in_compiler_generated_class(&field)
    {
        return false;
    }
    // The managed code casts the value to `NewObj` first.
    let construction = unwrap_invalid_conv(f, value);
    if !matches!(f.inst(construction), Inst::NewObj(_))
        || !match_delegate_construction(f, ctx, construction)
    {
        return false;
    }
    let usages: Vec<NodeId> = sibling(f, block, i, 1)
        .map(|next| {
            f.descendants_post(next)
                .into_iter()
                .filter(|&d| {
                    f.match_ldsfld(d)
                        .is_some_and(|o| same_field(&o.field, &field))
                })
                .collect()
        })
        .unwrap_or_default();
    match usages[..] {
        // A discarded method-group conversion (`_ = (Action)M;`) caches the delegate without
        // reading the cache back: the conversion stays, the caching goes.
        [] => f.replace(inst, value),
        [usage] => {
            f.replace(usage, value);
            let index = f.node(inst).child_index as usize;
            f.remove_at(block, index);
        }
        _ => return false,
    }
    true
}

/// `CachedDelegateInitializationWithLocal`: `stloc v(ldnull); ...; if (ldloc v == null)
/// stloc v(delegate)` before the one use of `v`.
fn with_local(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
    inst: NodeId,
) -> bool {
    let Some((condition, store)) = if_with_single_statement(f, inst) else {
        return false;
    };
    let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
        return false;
    };
    let Some(v) = f.match_ldloc(left) else {
        return false;
    };
    if !matches!(f.inst(right), Inst::LdNull) {
        return false;
    }
    let Some(value) = f
        .match_stloc(store)
        .filter(|&(sv, _)| sv == v)
        .map(|(_, value)| value)
    else {
        return false;
    };
    if !matches!(f.inst(value), Inst::NewObj(_)) || !match_delegate_construction(f, ctx, value) {
        return false;
    }
    // No other stores or loads of the variable.
    let var = f.var(v);
    if var.store_count() != 2
        || var.store_instructions.len() != 2
        || var.load_count() != 2
        || var.address_count() != 0
    {
        return false;
    }
    // The other store assigns null in a block.
    let Some(&other_store) = var.store_instructions.iter().find(|&&s| s != store) else {
        return false;
    };
    let Some((_, other_value)) = f.match_stloc(other_store) else {
        return false;
    };
    let Some(other_parent) = f.node(other_store).parent else {
        return false;
    };
    if !matches!(f.inst(other_value), Inst::LdNull)
        || !matches!(f.inst(other_parent), Inst::Block { .. })
    {
        return false;
    }
    let Some(next) = sibling(f, block, i, 1) else {
        return false;
    };
    let usages = f
        .descendants_post(next)
        .into_iter()
        .filter(|&d| f.match_ldloc(d) == Some(v))
        .count();
    if usages != 1 {
        return false;
    }
    let other_index = f.node(other_store).child_index as usize;
    f.remove_at(other_parent, other_index);
    f.replace(inst, store);
    true
}

/// `CachedDelegateInitializationRoslynInStaticWithLocal` (`static_field`) and
/// `CachedDelegateInitializationRoslynWithLocal`: `stloc s(ldobj F); if (ldloc s == null)
/// stloc s(stobj F(delegate))`; the first store gets the delegate.
fn roslyn_with_local(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
    inst: NodeId,
    static_field: bool,
) -> bool {
    let Some((condition, store)) = if_with_single_statement(f, inst) else {
        return false;
    };
    let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
        return false;
    };
    let Some(s) = f.match_ldloc(left) else {
        return false;
    };
    if !matches!(f.inst(right), Inst::LdNull) {
        return false;
    }
    let Some(store_before_if) = sibling(f, block, i, -1) else {
        return false;
    };
    let (Some((v1, store_value)), Some((v2, before_value))) =
        (f.match_stloc(store), f.match_stloc(store_before_if))
    else {
        return false;
    };
    if v1 != s || v2 != s {
        return false;
    }
    let Inst::StObj {
        target: stobj_target,
        value: delegate,
        ..
    } = *f.inst(store_value)
    else {
        return false;
    };
    let Inst::LdObj {
        target: ldobj_target,
        ..
    } = *f.inst(before_value)
    else {
        return false;
    };
    if !matches!(f.inst(delegate), Inst::NewObj(_)) {
        return false;
    }
    let field = |id: NodeId| match f.inst(id) {
        Inst::LdsFlda { field } if static_field => Some(&field.field),
        Inst::LdFlda { field, .. } if !static_field => Some(&field.field),
        _ => None,
    };
    let (Some(field1), Some(field2)) = (field(stobj_target), field(ldobj_target)) else {
        return false;
    };
    if !same_field(field1, field2) || !match_delegate_construction(f, ctx, delegate) {
        return false;
    }
    f.set_child_at(store_before_if, 0, delegate);
    true
}

/// The stack slot both branches of `if (...) { stloc s(a) } else { stloc s(b) }` store,
/// with `a` and `b`.
fn vb_branches(f: &Function, inst: NodeId) -> Option<(NodeId, VarId, NodeId, NodeId)> {
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    if !matches!(f.inst(true_inst), Inst::Block { .. })
        || !matches!(f.inst(false_inst), Inst::Block { .. })
    {
        return None;
    }
    let (&[t], &[e]) = (f.list(true_inst), f.list(false_inst)) else {
        return None;
    };
    let (s, true_value) = f.match_stloc(t)?;
    let (s2, false_value) = f.match_stloc(e)?;
    if s != s2 {
        return None;
    }
    let var = f.var(s);
    if var.kind != VariableKind::StackSlot || var.store_count() != 2 {
        return None;
    }
    Some((condition, s, true_value, false_value))
}

/// The new `stloc s(delegate)` that replaces `inst`.
fn replace_with_store(f: &mut Function, inst: NodeId, s: VarId, delegate: NodeId) {
    let stloc = f.add(Inst::StLoc {
        var: s,
        value: delegate,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.replace(inst, stloc);
}

/// `CachedDelegateInitializationVB`: `if (ldobj F == null) stloc s(stobj F(delegate)) else
/// stloc s(ldobj F)` becomes `stloc s(delegate)`.
fn vb(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    let Some((condition, s, true_value, false_value)) = vb_branches(f, inst) else {
        return false;
    };
    let Inst::StObj {
        target: stobj_target,
        value: delegate,
        ..
    } = *f.inst(true_value)
    else {
        return false;
    };
    let Inst::LdObj {
        target: ldobj_target,
        ..
    } = *f.inst(false_value)
    else {
        return false;
    };
    if !matches!(f.inst(delegate), Inst::NewObj(_)) {
        return false;
    }
    let (Inst::LdsFlda { field: field1 }, Inst::LdsFlda { field: field2 }) =
        (f.inst(stobj_target), f.inst(ldobj_target))
    else {
        return false;
    };
    if !same_field(&field1.field, &field2.field) {
        return false;
    }
    let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
        return false;
    };
    if !matches!(f.inst(right), Inst::LdNull) || !f.structurally_equal(false_value, left) {
        return false;
    }
    if !match_delegate_construction(f, ctx, delegate) {
        return false;
    }
    replace_with_store(f, inst, s, delegate);
    true
}

/// `CachedDelegateInitializationVBWithReturn`: `if (ldsfld F != null) return ldsfld F;
/// leave(stsfld F(delegate))`; the leave returns the delegate.
fn vb_with_return(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
    inst: NodeId,
) -> bool {
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return false;
    };
    let Some(arg) = f.match_comp_null(condition, ComparisonKind::Inequality) else {
        return false;
    };
    let Some(field) = f.match_ldsfld(arg).map(|o| o.field.clone()) else {
        return false;
    };
    if !f.match_nop(false_inst) {
        return false;
    }
    if !f
        .match_return(true_inst)
        .and_then(|v| f.match_ldsfld(v))
        .is_some_and(|o| same_field(&o.field, &field))
    {
        return false;
    }
    let Some(leave_after_if) = sibling(f, block, i, 1) else {
        return false;
    };
    // `IsLeavingFunction`: the leave targets the function body.
    let Some(value) = f.match_return(leave_after_if) else {
        return false;
    };
    let Some((field2, delegate)) = match_stsfld(f, value) else {
        return false;
    };
    if !same_field(&field, field2) || !match_delegate_construction(f, ctx, delegate) {
        return false;
    }
    f.set_child_at(leave_after_if, 0, delegate);
    true
}

/// `CachedDelegateInitializationVBWithClosure`: `if (ldobj c.F != null) stloc s(ldobj c.F)
/// else stloc s(stobj c.F(delegate))` becomes `stloc s(delegate)`.
fn vb_with_closure(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    let Some((condition, s, true_value, false_value)) = vb_branches(f, inst) else {
        return false;
    };
    let Inst::StObj {
        target: stobj_target,
        value: delegate,
        ..
    } = *f.inst(false_value)
    else {
        return false;
    };
    let Inst::LdObj {
        target: ldobj_target,
        ..
    } = *f.inst(true_value)
    else {
        return false;
    };
    if !matches!(f.inst(delegate), Inst::NewObj(_)) {
        return false;
    }
    let (
        Inst::LdFlda {
            target: target1,
            field: field1,
            ..
        },
        Inst::LdFlda {
            target: target2,
            field: field2,
            ..
        },
    ) = (f.inst(stobj_target), f.inst(ldobj_target))
    else {
        return false;
    };
    if !same_field(&field1.field, &field2.field) || !f.structurally_equal(*target1, *target2) {
        return false;
    }
    let Some(left) = f.match_comp_null(condition, ComparisonKind::Inequality) else {
        return false;
    };
    if !f.structurally_equal(true_value, left) || !match_delegate_construction(f, ctx, delegate) {
        return false;
    }
    replace_with_store(f, inst, s, delegate);
    true
}
