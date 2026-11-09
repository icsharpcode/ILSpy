//! `IL/Transforms/UserDefinedLogicTransform.cs`: the lowering of a user-defined `&&` or `||`
//! (`op_False`/`op_True` and `op_BitwiseAnd`/`op_BitwiseOr`), and of `&&` and `||` on dynamic
//! operands, becomes `user.logic` or `dynamic.logic.operator`.

use crate::il::ast::{DynamicKind, Function, Inst, NodeId, VarId};
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::csharp_semantics::type_erasure_equivalent;
use super::il_inlining::known;
use super::statement_transform::StatementTransformContext;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeData;

/// `ExpressionType.And`, `AndAlso`, `Or`, `OrElse`, `IsTrue` and `IsFalse`.
const AND: i32 = 2;
const AND_ALSO: i32 = 3;
const OR: i32 = 36;
const OR_ELSE: i32 = 37;
const IS_TRUE: i32 = 83;
const IS_FALSE: i32 = 84;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    let f = &mut r.function;
    if legacy_pattern(f, ctx, sc, block, pos) {
        return;
    }
    roslyn_optimized(f, ctx, block, pos);
}

/// `MatchIfInstructionPositiveCondition`: the condition without logic nots, and the branches
/// they swap.
pub(crate) fn match_if_positive_condition(
    f: &Function,
    inst: NodeId,
) -> Option<(NodeId, NodeId, NodeId)> {
    let Inst::If {
        mut condition,
        mut true_inst,
        mut false_inst,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    while let Some(arg) = f.match_logic_not(condition) {
        condition = arg;
        std::mem::swap(&mut true_inst, &mut false_inst);
    }
    Some((condition, true_inst, false_inst))
}

/// `RoslynOptimized`:
/// `if (logic.not(call op_False(ldloc lhsVar))) leave (call op_BitwiseAnd(ldloc lhsVar, rhs));
/// leave (ldloc lhsVar)` becomes `leave (user.logic op_BitwiseAnd(ldloc lhsVar, rhs))`.
fn roslyn_optimized(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let Some((condition, mut true_inst, mut false_inst)) =
        match_if_positive_condition(f, f.list(block)[pos])
    else {
        return false;
    };
    let Some(&next) = f.list(block).get(pos + 1) else {
        return false;
    };
    if f.match_nop(true_inst) {
        true_inst = next;
    } else if f.match_nop(false_inst) {
        false_inst = next;
    } else {
        return false;
    }
    let (Some(true_value), Some(false_value)) =
        (f.match_return(true_inst), f.match_return(false_inst))
    else {
        return false;
    };
    let Some(transformed) = transform(f, ctx, condition, true_value, false_value)
        .or_else(|| transform_dynamic(f, ctx, condition, true_value, false_value))
    else {
        return false;
    };
    f.set_child_at(next, 0, transformed);
    f.remove_at(block, pos);
    true
}

/// `LegacyPattern`:
/// `stloc s(lhs); if (logic.not(call op_False(ldloc s))) { stloc s(call op_BitwiseAnd(ldloc s,
/// rhs)) }` becomes `stloc s(user.logic op_BitwiseAnd(lhs, rhs))`.
fn legacy_pattern(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) -> bool {
    let stloc = f.list(block)[pos];
    let Some((s, lhs)) = f.match_stloc(stloc) else {
        return false;
    };
    if f.var(s).kind != crate::il::ast::VariableKind::StackSlot {
        return false;
    }
    let Some(&if_inst) = f.list(block).get(pos + 1) else {
        return false;
    };
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return false;
    };
    let Some(condition) = f.match_logic_not(condition) else {
        return false;
    };
    let Some((s2, condition_method_name)) = match_condition(f, ctx, condition) else {
        return false;
    };
    if s2 != s || !f.match_nop(false_inst) {
        return false;
    }
    let true_inst = super::null_coalescing_transform::unwrap_block(f, true_inst);
    let Some((v, store_value)) = f.match_stloc(true_inst) else {
        return false;
    };
    if v != s {
        return false;
    }
    let Inst::Call(c) = f.inst(store_value) else {
        return false;
    };
    let (method, args) = (c.method.clone(), c.args.clone());
    if !match_bitwise_call(f, ctx, store_value, s, &condition_method_name) {
        return false;
    }
    if is_used_within(f, s, args[1]) {
        return false;
    }
    let user_logic = f.add(Inst::UserDefinedLogic {
        method,
        left: lhs,
        right: args[1],
    });
    f.set_il_range(user_logic, f.il_range(store_value));
    f.set_child_at(stloc, 0, user_logic);
    f.remove_at(block, pos + 1);
    // The stloc may now be inlined.
    sc.request_rerun();
    true
}

/// `ILVariable.IsUsedWithin`.
fn is_used_within(f: &Function, v: VarId, inst: NodeId) -> bool {
    f.descendants_post(inst)
        .into_iter()
        .any(|n| f.variable_of(n) == Some(v))
}

/// `MatchCondition`: `call op_True(ldloc v)` or `call op_False(ldloc v)`; the variable and the
/// operator name.
fn match_condition(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
) -> Option<(VarId, String)> {
    let Inst::Call(c) = f.inst(condition) else {
        return None;
    };
    if !ctx.ts.method_is_operator(&c.method.method) || c.args.len() != 1 || c.method.lifted {
        return None;
    }
    let name = c.method.method.name.clone();
    if name != "op_True" && name != "op_False" {
        return None;
    }
    Some((f.match_ldloc(c.args[0])?, name))
}

/// `MatchBitwiseCall`: `call op_BitwiseAnd(ldloc v, ...)` for `op_False`, `op_BitwiseOr` for
/// `op_True`.
fn match_bitwise_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    call: NodeId,
    v: VarId,
    condition_method_name: &str,
) -> bool {
    let Inst::Call(c) = f.inst(call) else {
        return false;
    };
    if !ctx.ts.method_is_operator(&c.method.method) || c.args.len() != 2 || c.method.lifted {
        return false;
    }
    if f.match_ldloc(c.args[0]) != Some(v) {
        return false;
    }
    let name = c.method.method.name.as_str();
    (condition_method_name == "op_False" && name == "op_BitwiseAnd")
        || (condition_method_name == "op_True" && name == "op_BitwiseOr")
}

/// `Transform`: `if (call op_False(ldloc lhsVar)) ldloc lhsVar else call op_BitwiseAnd(ldloc
/// lhsVar, rhs)` becomes `user.logic op_BitwiseAnd(ldloc lhsVar, rhs)` (and `op_True` with
/// `op_BitwiseOr`).
pub(crate) fn transform(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
    true_inst: NodeId,
    false_inst: NodeId,
) -> Option<NodeId> {
    let (lhs_var, condition_method_name) = match_condition(f, ctx, condition)?;
    if f.match_ldloc(true_inst) != Some(lhs_var) {
        return None;
    }
    if !match_bitwise_call(f, ctx, false_inst, lhs_var, &condition_method_name) {
        return None;
    }
    let Inst::Call(c) = f.inst(false_inst) else {
        unreachable!("a call");
    };
    let (method, left, right) = (c.method.clone(), c.args[0], c.args[1]);
    let result = f.add(Inst::UserDefinedLogic {
        method,
        left,
        right,
    });
    for range_of in [condition, true_inst, false_inst] {
        f.add_il_range(result, f.il_range(range_of));
    }
    Some(result)
}

/// The operation and operand of a `dynamic.unary.operator`.
fn dynamic_unary(f: &Function, inst: NodeId) -> Option<(i32, NodeId)> {
    match f.inst(inst) {
        Inst::Dynamic {
            kind: DynamicKind::UnaryOperator,
            operation,
            args,
            ..
        } => Some((*operation, args[0])),
        _ => None,
    }
}

/// `TransformDynamic`: `if (dynamic.unary.operator IsFalse(ldloc lhsVar)) ldloc lhsVar else
/// dynamic.binary.operator And(ldloc lhsVar, rhs)` becomes `dynamic.logic.operator AndAlso(ldloc
/// lhsVar, rhs)` (and the `IsTrue` / `Or` / `OrElse` forms, also with boxes and `op_True` /
/// `op_False` conditions).
pub(crate) fn transform_dynamic(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
    mut true_inst: NodeId,
    mut false_inst: NodeId,
) -> Option<NodeId> {
    let is_bool_box =
        |f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId| match *f.inst(inst) {
            Inst::Box { arg, ty } if known(ctx, ty.ty) == Some(KnownTypeCode::Boolean) => Some(arg),
            _ => None,
        };
    // The condition.
    let (unary_op, lhs_var) = if let Some(lhs_var) = f.match_ldloc(condition) {
        // if (ldloc lhsVar) box bool(ldloc lhsVar) else dynamic.binary.operator.logic Or(...)
        if let Some(arg) = is_bool_box(f, ctx, true_inst) {
            true_inst = arg;
            (IS_TRUE, lhs_var)
        } else {
            // The condition negates, the branches swap.
            let arg = is_bool_box(f, ctx, false_inst)?;
            false_inst = true_inst;
            true_inst = arg;
            (IS_FALSE, lhs_var)
        }
    } else if let Some((operation, operand)) = dynamic_unary(f, condition) {
        (operation, f.match_ldloc(operand)?)
    } else if let Some((lhs_var, operator_method_name)) = match_condition(f, ctx, condition) {
        // if (call op_False(ldloc s)) box S(ldloc s) else dynamic.binary.operator.logic And(...)
        let unary_op = if operator_method_name == "op_True" {
            IS_TRUE
        } else {
            IS_FALSE
        };
        let Inst::Call(c) = f.inst(condition) else {
            unreachable!("a call");
        };
        let param_type = *c.method.method.params().ok()?.first()?;
        let param_type = skip_modifiers(ctx, param_type);
        if ctx.ts.is_reference_type(param_type) == Some(false) {
            // A value-type operand drops the box.
            if let Inst::Box { arg, ty } = *f.inst(true_inst)
                && type_erasure_equivalent(ctx, ty.ty, param_type)
            {
                true_inst = arg;
            } else if !matches!(f.inst(true_inst), Inst::LdcI4 { .. }) {
                return None;
            }
        }
        (unary_op, lhs_var)
    } else {
        return None;
    };
    // The true branch.
    let rhs_unary = if f.match_ldloc(true_inst) == Some(lhs_var) {
        // The typical pattern, where the expression is dynamic.
        None
    } else if f.match_ldc_i4(true_inst) == Some(1) && unary_op == IS_TRUE {
        // logic.or(IsTrue(lhsVar), IsTrue(lhsVar | rhs)) => IsTrue(lhsVar || rhs)
        let (operation, operand) = dynamic_unary(f, false_inst)?;
        if operation != IS_TRUE {
            return None;
        }
        let rhs_unary = false_inst;
        false_inst = operand;
        Some(rhs_unary)
    } else {
        return None;
    };
    let (expected_bitop, logic_op) = match unary_op {
        IS_FALSE => (AND, AND_ALSO),
        IS_TRUE => (OR, OR_ELSE),
        _ => return None,
    };
    // The false branch.
    let Inst::Dynamic {
        kind: DynamicKind::BinaryOperator,
        binder_flags,
        context,
        operation,
        ref arg_infos,
        ref args,
        ..
    } = *f.inst(false_inst)
    else {
        return None;
    };
    if operation != expected_bitop || f.match_ldloc(args[0]) != Some(lhs_var) {
        return None;
    }
    let (arg_infos, args) = (arg_infos.clone(), args.clone());
    let logic_inst = f.add(Inst::Dynamic {
        kind: DynamicKind::LogicOperator,
        binder_flags,
        context,
        name: None,
        operation: logic_op,
        ty: None,
        type_arguments: None,
        arg_infos,
        args,
    });
    f.set_il_range(logic_inst, f.il_range(false_inst));
    match rhs_unary {
        Some(rhs_unary) => {
            f.set_child_at(rhs_unary, 0, logic_inst);
            Some(rhs_unary)
        }
        None => Some(logic_inst),
    }
}

/// `IType.SkipModifiers()`.
fn skip_modifiers(
    ctx: &mut Context<'_, '_, '_>,
    t: crate::typesystem::types::TypeId,
) -> crate::typesystem::types::TypeId {
    match *ctx.ts.data(t) {
        TypeData::Modified { inner, .. } => skip_modifiers(ctx, inner),
        _ => t,
    }
}
