//! `IL/Transforms/NullPropagationTransform.cs`: `v != null ? v.M() : null` becomes `v?.M()`
//! (`nullable.rewrap` around an access chain that starts with `nullable.unwrap`).

use crate::il::ast::{ComparisonKind, ComparisonLiftingKind, Flags, Function, Inst, NodeId};
use crate::il::ast::{NullCoalescingKind, VarId, VariableKind, nullable_type};
use crate::il::control_flow::switch_detection::match_has_value_call;
use crate::il::reader::ReadFunction;
use crate::il::types::StackType;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeId;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::expression_transforms::{match_logic_and, match_logic_or};
use super::il_inlining::{inline_one_if_possible, known, options};
use super::null_coalescing_transform::unwrap_block;
use super::nullable_lifting_transform::{
    is_non_nullable_value_type, match_get_value_or_default, match_nullable_ctor,
};
use super::statement_transform::StatementTransformContext;

/// `NullPropagationStatementTransform.Run`.
pub fn run_statement(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.null_propagation {
        return;
    }
    run_statements(&mut r.function, ctx, block, pos);
}

/// How the tested variable is checked for null.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Mode {
    /// A reference or generic type: `comp(ldloc testedVar == null)`.
    ReferenceType,
    /// A nullable used by value: `call get_HasValue(ldloca testedVar)`.
    NullableByValue,
    /// A nullable used by reference: `call get_HasValue(ldloc testedVar)`.
    NullableByReference,
    /// An unconstrained generic type (`TransformNullPropagationOnUnconstrainedGenericExpression`).
    UnconstrainedType,
}

/// `IfInstruction.IsInConditionSlot`.
fn is_in_condition_slot(f: &Function, inst: NodeId) -> bool {
    let Some(parent) = f.node(inst).parent else {
        return false;
    };
    let index = f.node(inst).child_index;
    match *f.inst(parent) {
        Inst::If { .. } if index == 0 => true,
        Inst::If { .. } => is_in_condition_slot(f, parent),
        Inst::NullCoalescing { .. } if index == 1 => is_in_condition_slot(f, parent),
        Inst::Comp { left, right, .. } => {
            (left == inst && f.match_ldc_i4(right) == Some(0))
                || (right == inst && f.match_ldc_i4(left) == Some(0))
        }
        _ => false,
    }
}

/// `IsProtectedIfInst`: a `logic.and` or `logic.or` in a condition, which stays as it is so
/// that `interfaces != null && interfaces.Count != 0` does not become
/// `(interfaces?.Count ?? 0) != 0`.
pub(crate) fn is_protected_if_inst(f: &Function, if_inst: NodeId) -> bool {
    matches!(f.inst(if_inst), Inst::If { .. })
        && (match_logic_and(f, if_inst).is_some() || match_logic_or(f, if_inst).is_some())
        && is_in_condition_slot(f, if_inst)
}

/// `Run`: the replacement of `condition ? true_inst : false_inst` with a null-conditional
/// operator, if there is one. `condition` is positive (no logic not).
pub(crate) fn run(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
    true_inst: NodeId,
    false_inst: NodeId,
) -> Option<NodeId> {
    if let Inst::Comp {
        kind,
        lifting,
        left,
        right,
        ..
    } = *f.inst(condition)
        && let Some(tested) = f.match_ldloc(left)
        && matches!(f.inst(right), Inst::LdNull)
    {
        if lifting != ComparisonLiftingKind::None {
            return None;
        }
        return match kind {
            // testedVar == null ? trueInst : falseInst
            ComparisonKind::Equality => {
                try_null_propagation(f, ctx, tested, false_inst, true_inst, Mode::ReferenceType)
            }
            ComparisonKind::Inequality => {
                try_null_propagation(f, ctx, tested, true_inst, false_inst, Mode::ReferenceType)
            }
            _ => None,
        };
    }
    if let Some(load) = match_has_value_call(f, ctx, condition) {
        // loadInst.HasValue ? trueInst : falseInst
        match *f.inst(load) {
            Inst::LdLoca { var } => {
                return try_null_propagation(
                    f,
                    ctx,
                    var,
                    true_inst,
                    false_inst,
                    Mode::NullableByValue,
                );
            }
            Inst::LdLoc { var } => {
                return try_null_propagation(
                    f,
                    ctx,
                    var,
                    true_inst,
                    false_inst,
                    Mode::NullableByReference,
                );
            }
            _ => {}
        }
    }
    None
}

/// `TryNullPropagation`: `testedVar != null ? nonNullInst : nullInst`.
fn try_null_propagation(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    tested: VarId,
    mut non_null_inst: NodeId,
    null_inst: NodeId,
    mode: Mode,
) -> Option<NodeId> {
    let mut removed_rewrap_or_nullable_ctor = false;
    if let Some((_, arg)) = match_nullable_ctor(f, ctx, non_null_inst) {
        non_null_inst = arg;
        removed_rewrap_or_nullable_ctor = true;
    } else if let Inst::NullableRewrap { arg, .. } = *f.inst(non_null_inst) {
        non_null_inst = arg;
        removed_rewrap_or_nullable_ctor = true;
    }
    let var_load = is_valid_access_chain(f, ctx, tested, mode, non_null_inst)?;
    // The access chain consists of calls and field accesses, so the inferred type is exact.
    let return_type = f.infer_type(ctx.ts, non_null_inst);
    if matches!(f.inst(null_inst), Inst::LdNull) {
        // testedVar != null ? testedVar.AccessChain : null => testedVar?.AccessChain
        introduce_unwrap(f, ctx, tested, var_load, mode);
        return Some(f.add(Inst::NullableRewrap {
            arg: non_null_inst,
            ty: return_type,
        }));
    }
    if let Inst::DefaultValue { ty, .. } = *f.inst(null_inst)
        && known(ctx, ty.ty) == Some(KnownTypeCode::NullableOfT)
    {
        // testedVar != null ? testedVar.AccessChain : default(T?) => testedVar?.AccessChain
        introduce_unwrap(f, ctx, tested, var_load, mode);
        return Some(f.add(Inst::NullableRewrap {
            arg: non_null_inst,
            ty: ty.ty,
        }));
    }
    // `?? nullInst` only works when the chain returns a non-nullable value, and not for a
    // by-ref-like type such as Span<T>, which cannot be wrapped in Nullable<T>.
    if !removed_rewrap_or_nullable_ctor
        && is_non_nullable_value_type(ctx, return_type)
        && !ctx.ts.is_by_ref_like(return_type)
    {
        // testedVar != null ? testedVar.AccessChain : nullInst
        // => testedVar?.AccessChain ?? nullInst
        introduce_unwrap(f, ctx, tested, var_load, mode);
        let ty = f.infer_type(ctx.ts, null_inst);
        let nullable = nullable_type(ctx.ts, return_type);
        let rewrap = f.add(Inst::NullableRewrap {
            arg: non_null_inst,
            ty: nullable,
        });
        return Some(f.add(Inst::NullCoalescing {
            kind: NullCoalescingKind::NullableWithValueFallback,
            ty,
            value: rewrap,
            fallback: null_inst,
        }));
    }
    None
}

/// `RunStatements`: `if (x != null) x.AccessChain();` becomes `x?.AccessChain();`, and the
/// expansion of `t?.M()` on an unconstrained generic `t` folds back.
pub(crate) fn run_statements(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) {
    let if_inst = f.list(block)[pos];
    if let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(if_inst)
        && f.match_nop(false_inst)
    {
        if let Inst::Comp {
            kind: ComparisonKind::Inequality,
            left,
            right,
            ..
        } = *f.inst(condition)
            && let Some(tested) = f.match_ldloc(left)
            && matches!(f.inst(right), Inst::LdNull)
        {
            try_null_prop_for_void_call(f, ctx, tested, Mode::ReferenceType, true_inst, if_inst);
        } else if let Some(arg) = match_has_value_call(f, ctx, condition) {
            match *f.inst(arg) {
                Inst::LdLoca { var } => try_null_prop_for_void_call(
                    f,
                    ctx,
                    var,
                    Mode::NullableByValue,
                    true_inst,
                    if_inst,
                ),
                Inst::LdLoc { var } => try_null_prop_for_void_call(
                    f,
                    ctx,
                    var,
                    Mode::NullableByReference,
                    true_inst,
                    if_inst,
                ),
                _ => {}
            }
        }
    }
    let Some(m) = match_unconstrained_generic_expression(f, block, pos) else {
        return;
    };
    let parent = f.node(m.non_null_inst).parent.expect("a parent");
    let Some(replacement) = try_null_propagation(
        f,
        ctx,
        m.target,
        m.non_null_inst,
        m.null_inst,
        Mode::UnconstrainedType,
    ) else {
        return;
    };
    match f.inst(parent) {
        Inst::StLoc { .. } | Inst::Leave { .. } => f.set_child_at(parent, 0, replacement),
        // The matched pattern only puts the value into a stloc or a leave.
        other => panic!("unexpected parent of the non-null instruction: {other:?}"),
    }
    // Remove the fallback conditions and blocks.
    f.remove_at(block, pos + 1);
    f.remove_at(block, pos + 1);
    // An end block only reachable through this block joins it.
    if let Some(end_block) = m.end_block
        && f.node(end_block).incoming_edge_count == 1
    {
        for inst in f.list(end_block).to_vec() {
            f.push(block, inst);
        }
        f.remove_at(block, pos + 2);
        f.remove_block(end_block);
    }
    inline_one_if_possible(f, block, pos, options::AGGRESSIVE, ctx);
}

/// `TryNullPropForVoidCall`.
fn try_null_prop_for_void_call(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    tested: VarId,
    mode: Mode,
    body: NodeId,
    if_inst: NodeId,
) {
    if !matches!(f.inst(body), Inst::Block { .. }) {
        return;
    }
    let &[mut body_inst] = f.list(body) else {
        return;
    };
    if let Inst::NullableRewrap { arg, .. } = *f.inst(body_inst) {
        body_inst = arg;
    }
    let Some(var_load) = is_valid_access_chain(f, ctx, tested, mode, body_inst) else {
        return;
    };
    // if (testedVar != null) { testedVar.AccessChain(); } => testedVar?.AccessChain();
    introduce_unwrap(f, ctx, tested, var_load, mode);
    let mut return_type = f.infer_type(ctx.ts, body_inst);
    if known(ctx, return_type) != Some(KnownTypeCode::Void)
        && is_non_nullable_value_type(ctx, return_type)
    {
        return_type = nullable_type(ctx.ts, return_type);
    }
    let replacement = f.add(Inst::NullableRewrap {
        arg: body_inst,
        ty: return_type,
    });
    f.add_il_range(replacement, f.il_range(if_inst));
    f.replace(if_inst, replacement);
}

/// `IsValidEndOfChain`.
fn is_valid_end_of_chain(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    tested: VarId,
    mode: Mode,
    inst: NodeId,
) -> bool {
    match mode {
        // A reference type (`ldloc testedVar`) or an unconstrained generic type
        // (`ldloca testedVar`).
        Mode::ReferenceType => f.match_ld_loc_ref(ctx.ts, inst) == Some(tested),
        Mode::NullableByValue => match_get_value_or_default(f, ctx, inst)
            .is_some_and(|arg| matches!(*f.inst(arg), Inst::LdLoca { var } if var == tested)),
        Mode::NullableByReference => match_get_value_or_default(f, ctx, inst)
            .is_some_and(|arg| f.match_ldloc(arg) == Some(tested)),
        Mode::UnconstrainedType => {
            f.match_ldloc(inst) == Some(tested)
                || matches!(*f.inst(inst), Inst::LdObjIfRef { target, .. }
                    if f.match_ldloc(target) == Some(tested))
        }
    }
}

/// `IType.Kind == TypeKind.Struct`.
fn is_struct(ctx: &mut Context<'_, '_, '_>, t: Option<TypeId>) -> bool {
    t.and_then(|t| ctx.ts.definition(t))
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == TypeKind::Struct)
}

/// Whether any of `args` (the arguments after the first, which is part of the chain)
/// contains a `nullable.unwrap` that is not part of the chain.
fn arguments_after_first_may_unwrap_null(f: &Function, args: &[NodeId]) -> bool {
    args.iter()
        .skip(1)
        .any(|&a| f.has_flag(a, Flags::MAY_UNWRAP_NULL))
}

/// `IsValidAccessChain`: `inst` is a chain of member accesses of at least one link that
/// ends in the load of `tested`; returns that final load.
fn is_valid_access_chain(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    tested: VarId,
    mode: Mode,
    mut inst: NodeId,
) -> Option<NodeId> {
    let mut chain_length = 0;
    loop {
        if is_valid_end_of_chain(f, ctx, tested, mode, inst) {
            return (chain_length >= 1).then_some(inst);
        }
        if let Some((target, _)) = f.match_ldfld(ctx.ts, inst) {
            inst = target;
        } else if let Some((target, field)) = f.match_ldflda(inst) {
            let declaring = field.field.declaring;
            inst = match *f.inst(target) {
                Inst::AddressOf { value, .. } if is_struct(ctx, declaring) => value,
                _ => target,
            };
        } else if let Inst::Call(c) | Inst::CallVirt(c) = f.inst(inst) {
            let &first = c.args.first()?;
            // Only instance or extension methods can be called with `?.`.
            // shortcut: an extension method also needs the C# resolver's
            // CanTransformToExtensionMethodCall, which finds no extension method without a
            // using scope (as in the IL pipeline the tests compare); port it with the
            // semantics step (item 67) and the host's using scope.
            if c.method.method.is_static {
                return None;
            }
            // A setter, adder or remover cannot be called with `?.`.
            if c.method.method.accessor_kind != 0 && c.method.method.accessor_kind != 0x0002 {
                return None;
            }
            let this_type = c.constrained_to.map(|t| t.ty).or(c.method.method.declaring);
            inst = first;
            let is_value_type =
                this_type.is_some_and(|t| ctx.ts.is_reference_type(t) == Some(false));
            match *f.inst(inst) {
                Inst::AddressOf { value, .. } if is_value_type => inst = value,
                Inst::LdObjIfRef { target, .. } => inst = target,
                _ => {}
            }
            // The chain contains no `nullable.unwrap` that is not directly part of it.
            if arguments_after_first_may_unwrap_null(f, &c.args) {
                return None;
            }
        } else {
            match f.inst(inst) {
                &Inst::LdLen { array, .. } => inst = array,
                Inst::LdElema { array, indices, .. } => {
                    let array = *array;
                    if indices
                        .iter()
                        .any(|&i| f.has_flag(i, Flags::MAY_UNWRAP_NULL))
                    {
                        return None;
                    }
                    inst = array;
                }
                &Inst::NullableUnwrap { arg, ref_input, .. } => {
                    inst = arg;
                    if ref_input && let Inst::AddressOf { value, .. } = *f.inst(inst) {
                        inst = value;
                    }
                    // Two null-conditional operators cannot follow each other in C#.
                    if is_valid_end_of_chain(f, ctx, tested, mode, inst) {
                        return None;
                    }
                }
                Inst::Dynamic {
                    kind: crate::il::ast::DynamicKind::GetMember,
                    args,
                    ..
                } => inst = args[0],
                Inst::Dynamic {
                    kind:
                        crate::il::ast::DynamicKind::InvokeMember
                        | crate::il::ast::DynamicKind::GetIndex,
                    args,
                    ..
                } => {
                    if arguments_after_first_may_unwrap_null(f, args) {
                        return None;
                    }
                    inst = args[0];
                }
                // An unknown node: no valid chain.
                _ => return None,
            }
        }
        chain_length += 1;
    }
}

/// `IntroduceUnwrap`: the final load of the chain becomes a `nullable.unwrap`.
fn introduce_unwrap(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    tested: VarId,
    var_load: NodeId,
    mode: Mode,
) {
    let parent = f.node(var_load).parent.expect("a parent");
    let index = f.node(var_load).child_index as usize;
    let ty = f.infer_type(ctx.ts, var_load);
    let result = f.result_type(ctx.ts, var_load);
    let replacement = match mode {
        Mode::ReferenceType | Mode::UnconstrainedType => f.add(Inst::NullableUnwrap {
            arg: var_load,
            ty,
            result,
            ref_input: result == StackType::Ref,
        }),
        Mode::NullableByValue | Mode::NullableByReference => {
            let address = f.inst(var_load).children()[0];
            let load = f.add(Inst::LdLoc { var: tested });
            f.add_il_range(load, f.il_range(address));
            let unwrap = f.add(Inst::NullableUnwrap {
                arg: load,
                ty,
                result,
                ref_input: mode == Mode::NullableByReference,
            });
            f.add_il_range(unwrap, f.il_range(var_load));
            unwrap
        }
    };
    f.set_child_at(parent, index, replacement);
}

/// A match of `TransformNullPropagationOnUnconstrainedGenericExpression`.
struct UnconstrainedMatch {
    target: VarId,
    non_null_inst: NodeId,
    null_inst: NodeId,
    end_block: Option<NodeId>,
}

/// Where the fallback block continues: the end block (with the result temporary it stores
/// to) or the container the function leaves.
#[derive(Clone, Copy)]
enum FallbackEnd {
    Block(NodeId, VarId),
    Leave(NodeId),
}

/// `TransformNullPropagationOnUnconstrainedGenericExpression`:
///
/// ```text
/// stloc target(targetInst)
/// stloc defaultTemporary(default.value type)
/// if (logic.not(comp.o(box `0(ldloc defaultTemporary) != ldnull))) Block fallbackBlock {
///     stloc defaultTemporary(ldobj type(ldloc target))
///     stloc target(ldloca defaultTemporary)
///     if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock2 {
///         stloc resultTemporary(nullInst)
///         br endBlock
///     }
/// }
/// stloc resultTemporary(constrained[type].call_instruction(ldloc target, ...))
/// br endBlock
/// ```
///
/// becomes `stloc resultTemporary(nullable.rewrap(constrained[type].call_instruction(
/// nullable.unwrap(targetInst), ...)))`; likewise with `leave(nullInst)` and
/// `leave(constrained[type].call_instruction(...))` in place of the result temporary.
fn match_unconstrained_generic_expression(
    f: &Function,
    block: NodeId,
    pos: usize,
) -> Option<UnconstrainedMatch> {
    let list = f.list(block);
    if pos + 3 >= list.len() {
        return None;
    }
    // stloc target(...)
    let (target, _) = f.match_stloc(list[pos])?;
    let t = f.var(target);
    if !(t.kind == VariableKind::StackSlot && t.load_count() == 2 && t.store_count() == 2) {
        return None;
    }
    // stloc defaultTemporary(default.value type)
    let (default_temporary, default_expression) = f.match_stloc(list[pos + 1])?;
    let Inst::DefaultValue { ty, .. } = *f.inst(default_expression) else {
        return None;
    };
    let ty = ty.ty;
    // Two stores and loads, and one address (the stloc of target in the fallback block).
    let d = f.var(default_temporary);
    if !(d.kind == VariableKind::Local
        && d.load_count() == 2
        && d.store_count() == 2
        && d.address_count() == 1)
    {
        return None;
    }
    // if (logic.not(comp.o(box `0(ldloc defaultTemporary) != ldnull))) Block fallbackBlock
    let (condition, fallback_block) = f.match_if_instruction(list[pos + 2])?;
    let arg = f.match_comp_null(condition, ComparisonKind::Equality)?;
    if f.match_ldloc(arg) != Some(default_temporary) {
        return None;
    }
    let is_fallback =
        |end| is_fallback_block(f, fallback_block, ty, target, default_temporary, end);
    // stloc resultTemporary(constrained[type].call_instruction(ldloc target, ...)); br endBlock
    if pos + 4 < list.len()
        && let Some((result_temporary, non_null_inst)) = f.match_stloc(list[pos + 3])
        && let Some(end_block) = f.match_branch(list[pos + 4])
        && let Some(null_inst) = is_fallback(FallbackEnd::Block(end_block, result_temporary))
    {
        return Some(UnconstrainedMatch {
            target,
            non_null_inst,
            null_inst,
            end_block: Some(end_block),
        });
    }
    // leave (constrained[type].call_instruction(ldloc target, ...))
    if let Inst::Leave {
        target: Some(container),
        value: non_null_inst,
    } = *f.inst(list[pos + 3])
        && Some(container) == f.body
        && let Some(null_inst) = is_fallback(FallbackEnd::Leave(container))
    {
        return Some(UnconstrainedMatch {
            target,
            non_null_inst,
            null_inst,
            end_block: None,
        });
    }
    None
}

/// `IsFallbackBlock`:
///
/// ```text
/// Block fallbackBlock {
///     stloc defaultTemporary(ldobj type(ldloc target))
///     stloc target(ldloca defaultTemporary)
///     if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock {
///         stloc resultTemporary(nullInst)
///         br endBlock
///     }
/// }
/// ```
///
/// (or `leave(nullInst)` in the innermost block); returns `nullInst`.
fn is_fallback_block(
    f: &Function,
    block: NodeId,
    ty: TypeId,
    target: VarId,
    default_temporary: VarId,
    end: FallbackEnd,
) -> Option<NodeId> {
    if !matches!(f.inst(block), Inst::Block { .. }) {
        return None;
    }
    let &[first, second, third] = f.list(block) else {
        return None;
    };
    // stloc defaultTemporary(ldobj type(ldloc target))
    let (v, value) = f.match_stloc(first)?;
    if v != default_temporary {
        return None;
    }
    let Inst::LdObj {
        target: load,
        ty: t,
        ..
    } = *f.inst(value)
    else {
        return None;
    };
    if t.ty != ty || f.match_ldloc(load) != Some(target) {
        return None;
    }
    // stloc target(ldloca defaultTemporary)
    let (v, default_address) = f.match_stloc(second)?;
    if v != target
        || !matches!(*f.inst(default_address), Inst::LdLoca { var } if var == default_temporary)
    {
        return None;
    }
    // if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock
    let (condition, inner) = f.match_if_instruction(third)?;
    let arg = f.match_comp_null(condition, ComparisonKind::Equality)?;
    if f.match_ldloc(arg) != Some(default_temporary) {
        return None;
    }
    let fallback_inst = unwrap_block(f, inner);
    match end {
        FallbackEnd::Block(end_block, result_temporary)
            if matches!(f.inst(fallback_inst), Inst::Block { .. }) =>
        {
            // { stloc resultTemporary(nullInst); br endBlock }
            let &[store, branch] = f.list(fallback_inst) else {
                return None;
            };
            let (v, null_inst) = f.match_stloc(store)?;
            (v == result_temporary && f.match_branch(branch) == Some(end_block))
                .then_some(null_inst)
        }
        FallbackEnd::Leave(container) => {
            let (leave_target, value) = f.match_leave(fallback_inst)?;
            (leave_target == container && !f.match_nop(value)).then_some(value)
        }
        FallbackEnd::Block(..) => None,
    }
}
