//! `IL/Transforms/NullCoalescingTransform.cs`: null checks that store a fallback become `??`
//! (`if.notnull`), including throw expressions.

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId, NullCoalescingKind, VariableKind};
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{Find, find_load_in_next, inline_one_if_possible, options};
use super::statement_transform::StatementTransformContext;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if transform_ref_types(&mut r.function, ctx, block, pos) {
        return;
    }
    if transform_hoisted_constructor_argument_null_guard(r, ctx, block, pos) {
        return;
    }
    transform_throw_expression_value_types(&mut r.function, ctx, block, pos);
}

/// `Block.Unwrap`.
pub(crate) fn unwrap_block(f: &Function, inst: NodeId) -> NodeId {
    if let Inst::Block { final_inst, .. } = *f.inst(inst)
        && f.list(inst).len() == 1
        && f.match_nop(final_inst)
    {
        return f.list(inst)[0];
    }
    inst
}

/// The new `if.notnull(value, fallback)`.
fn new_null_coalescing(
    f: &mut Function,
    ty: crate::typesystem::types::TypeId,
    kind: NullCoalescingKind,
    value: NodeId,
    fallback: NodeId,
) -> NodeId {
    f.add(Inst::NullCoalescing {
        kind,
        ty,
        value,
        fallback,
    })
}

/// `TransformRefTypes`: `stloc s(value); if (s == null) stloc s(fallback)` (also with a
/// temporary, or a throw in the if) becomes `stloc s(if.notnull(value, fallback))`.
fn transform_ref_types(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let list = f.list(block);
    let Some((s, value)) = list.get(pos).and_then(|&i| f.match_stloc(i)) else {
        return false;
    };
    let store = list[pos];
    if f.var(s).kind != VariableKind::StackSlot {
        return false;
    }
    let Some(&next) = list.get(pos + 1) else {
        return false;
    };
    let Some((condition, true_inst)) = f.match_if_instruction(next) else {
        return false;
    };
    let is_null_check = f
        .match_comp(condition, ComparisonKind::Equality)
        .is_some_and(|(l, r)| f.match_ldloc(l) == Some(s) && matches!(f.inst(r), Inst::LdNull));
    if !is_null_check {
        return false;
    }
    let true_inst = unwrap_block(f, true_inst);
    let fallback = if let Some((v, fallback)) = f.match_stloc(true_inst)
        && v == s
    {
        // if (s == null) stloc s(fallback)
        fallback
    } else if let Inst::Block { .. } = f.inst(true_inst)
        && let &[first, second] = f.list(true_inst)
        && let Some((temporary, fallback)) = f.match_stloc(first)
        && f.var(temporary).is_single_definition()
        && f.var(temporary).load_count() == 1
        && let Some((v, use_of_temporary)) = f.match_stloc(second)
        && v == s
        && f.match_ldloc(use_of_temporary) == Some(temporary)
    {
        // if (s == null) { stloc v(fallback); stloc s(ldloc v) }
        fallback
    } else if ctx.settings.throw_expressions && matches!(f.inst(true_inst), Inst::Throw { .. }) {
        // if (s == null) throw(...)
        true_inst
    } else {
        return false;
    };
    let ty = f.var(s).ty;
    let coalescing = new_null_coalescing(f, ty, NullCoalescingKind::Ref, value, fallback);
    f.set_child_at(store, 0, coalescing);
    f.remove_at(block, pos + 1);
    inline_one_if_possible(f, block, pos, options::NONE, ctx);
    true
}

/// `IsArgumentNullGuard`: `if (comp(ldloc param == ldnull)) throw(...)`.
fn is_argument_null_guard(f: &Function, inst: NodeId) -> Option<(NodeId, NodeId)> {
    let (condition, true_inst) = f.match_if_instruction(inst)?;
    let throw = unwrap_block(f, true_inst);
    if !matches!(f.inst(throw), Inst::Throw { .. }) {
        return None;
    }
    let (left, right) = f.match_comp(condition, ComparisonKind::Equality)?;
    if !matches!(f.inst(right), Inst::LdNull) {
        return None;
    }
    let param = f.match_ldloc(left)?;
    (f.var(param).kind == VariableKind::Parameter).then_some((left, throw))
}

/// Puts `child` back into its slot of `parent` (`siblings[index] = child`), undoing the
/// parent a tentative instruction gave it.
fn restore_parent(f: &mut Function, parent: NodeId, index: usize, child: NodeId) {
    f.set_child_at(parent, index, child);
}

/// `TransformHoistedConstructorArgumentNullGuard`: a null guard the compiler hoisted in front
/// of a constructor initializer's argument folds into the argument as a throw expression.
fn transform_hoisted_constructor_argument_null_guard(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    // A throw expression is the only way to express the folded form.
    if !ctx.settings.throw_expressions || !r.function.is_instance_constructor {
        return false;
    }
    let f = &mut r.function;
    let guard = f.list(block)[pos];
    let Some((param_load, throw)) = is_argument_null_guard(f, guard) else {
        return false;
    };
    // Only a guard before the chained constructor call is necessarily compiler-hoisted.
    if !super::il_inlining::is_in_constructor_initializer(f, ctx, guard) {
        return false;
    }
    let Some(&next) = f.list(block).get(pos + 1) else {
        return false;
    };
    let param = f.match_ldloc(param_load).expect("a parameter load");
    let param_type = f.var(param).ty;
    let (load_parent, load_index) = (
        f.node(param_load).parent.expect("a parent"),
        f.node(param_load).child_index as usize,
    );
    let (throw_parent, throw_index) = (
        f.node(throw).parent.expect("a parent"),
        f.node(throw).child_index as usize,
    );
    let expression_with_throw =
        new_null_coalescing(f, param_type, NullCoalescingKind::Ref, param_load, throw);
    let first_use = match find_load_in_next(
        f,
        ctx,
        Some(next),
        param,
        expression_with_throw,
        options::NONE,
    ) {
        Find::Found(load) if matches!(f.inst(load), Inst::LdLoc { .. }) => load,
        _ => {
            restore_parent(f, load_parent, load_index, param_load);
            restore_parent(f, throw_parent, throw_index, throw);
            return false;
        }
    };
    let temp = r.register_variable(ctx, VariableKind::StackSlot, param_type);
    let f = &mut r.function;
    f.set_variable(first_use, temp);
    let stloc = f.add(Inst::StLoc {
        var: temp,
        value: expression_with_throw,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.add_il_range(stloc, f.il_range(guard));
    f.set_child_at(block, pos, stloc);
    inline_one_if_possible(f, block, pos, options::NONE, ctx);
    true
}

/// `TransformThrowExpressionValueTypes`: `stloc v(value); if (!v.HasValue) throw(...);
/// ... v.GetValueOrDefault() ...` becomes `... if.notnull(value, throw(...)) ...`.
fn transform_throw_expression_value_types(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let list = f.list(block);
    if pos + 2 >= list.len() {
        return false;
    }
    let (store, guard, next) = (list[pos], list[pos + 1], list[pos + 2]);
    let Some((v, value)) = f.match_stloc(store) else {
        return false;
    };
    let var = f.var(v);
    if !(var.store_count() == 1 && var.load_count() == 0 && var.address_count() == 2) {
        return false;
    }
    let Some((condition, true_inst)) = f.match_if_instruction(guard) else {
        return false;
    };
    let throw = unwrap_block(f, true_inst);
    if !matches!(f.inst(throw), Inst::Throw { .. }) {
        return false;
    }
    let Some(call) = f.match_logic_not(condition) else {
        return false;
    };
    let declaring = match f.inst(call) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => c.method.method.declaring,
        _ => return false,
    };
    if !super::pattern_matching_transform::match_has_value_call_of(f, ctx, call, v) {
        return false;
    }
    let Some(declaring) = declaring else {
        return false;
    };
    let (throw_parent, throw_index) = (
        f.node(throw).parent.expect("a parent"),
        f.node(throw).child_index as usize,
    );
    let underlying = super::transform_assignment::nullable_underlying(ctx, declaring);
    let coalescing = new_null_coalescing(
        f,
        underlying,
        NullCoalescingKind::NullableWithValueFallback,
        value,
        throw,
    );
    if let Find::Found(load) = find_load_in_next(f, ctx, Some(next), v, coalescing, options::NONE)
        && let Some(parent) = f.node(load).parent
        && super::pattern_matching_transform::match_get_value_or_default_of(f, ctx, parent, v)
    {
        f.replace(parent, coalescing);
        f.remove_at(block, pos);
        f.remove_at(block, pos);
        return true;
    }
    restore_parent(f, store, 0, value);
    restore_parent(f, throw_parent, throw_index, throw);
    false
}
