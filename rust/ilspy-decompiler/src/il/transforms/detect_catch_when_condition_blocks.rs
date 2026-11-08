//! `IL/Transforms/DetectCatchWhenConditionBlocks.cs`: a catch filter testing the exception
//! type becomes `catch (T ex) when (...)`: the handler variable takes the type, and copies of
//! it in the handler are replaced by the variable.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, BranchTarget, ComparisonKind, Function, Inst, NodeId, VarId,
    VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::types::Sign;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeId, TypeSystem};

use super::Context;
use super::remove_infeasible_path_transform::sort_blocks;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let ts = &mut *ctx.ts;
    let body = f.body.expect("body");
    let handlers: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::TryCatchHandler { .. }))
        .collect();
    for handler in handlers {
        let Inst::TryCatchHandler {
            filter, variable, ..
        } = *f.inst(handler)
        else {
            unreachable!("handler");
        };
        if !matches!(f.inst(filter), Inst::BlockContainer { .. }) {
            continue;
        }
        let var_stack_type = f.var(variable).stack_type;
        if let Some((exception_type, exception_slot, when_condition_block)) =
            match_catch_when_entry_point(f, variable, filter)
            && ts.stack_type(exception_type) == var_stack_type
        {
            f.var_mut(variable).ty = exception_type;
            let entry = f.list(filter)[0];
            let list = f.list(entry).to_vec();
            if list.len() == 3 {
                // stloc temp(isinst T(ldloc ex)); if (comp(ldloc temp != ldnull)) br when; br false
                // => stloc temp(ldloc ex); br when
                let Inst::StLoc { value, .. } = *f.inst(list[0]) else {
                    unreachable!("stloc");
                };
                f.replace(value, exception_slot);
                let jump = f.add(Inst::Branch {
                    target: BranchTarget::Block(when_condition_block),
                });
                f.replace(list[1], jump);
                f.remove_at(entry, 2);
                sort_blocks(f, filter);
            } else if list.len() == 2 {
                // if (comp(isinst T(ldloc ex) != ldnull)) br when; br false => br when
                let jump = f.add(Inst::Branch {
                    target: BranchTarget::Block(when_condition_block),
                });
                f.replace(list[0], jump);
                f.remove_at(entry, 1);
                sort_blocks(f, filter);
            }
            propagate_exception_variable(f, handler);
        } else if let Some((exception_type, type_test)) =
            match_vb_on_error_catch_filter(f, ts, variable, filter)
            && ts.stack_type(exception_type) == var_stack_type
        {
            f.var_mut(variable).ty = exception_type;
            let Inst::BinaryNumeric { right, .. } = *f.inst(type_test) else {
                unreachable!("bit.and");
            };
            f.replace(type_test, right);
            propagate_exception_variable(f, handler);
        }
    }
}

fn is_known(ts: &TypeSystem<'_, '_>, t: TypeId, code: KnownTypeCode) -> bool {
    ts.definition(t)
        .and_then(|d| ts.comp.type_def(d).ok())
        .is_some_and(|i| i.known == code)
}

/// `MatchCompUnsignedZero`: `x == 0` (`x <= 0` unsigned) or `x != 0` (`x > 0` unsigned).
fn match_comp_unsigned_zero(f: &Function, inst: NodeId, kind: ComparisonKind) -> Option<NodeId> {
    let unsigned_kind = if kind == ComparisonKind::Equality {
        ComparisonKind::LessThanOrEqual
    } else {
        ComparisonKind::GreaterThan
    };
    match *f.inst(inst) {
        Inst::Comp {
            kind: k,
            sign,
            left,
            right,
            ..
        } if f.match_ldc_i4(right) == Some(0)
            && (k == kind || (k == unsigned_kind && sign == Sign::Unsigned)) =>
        {
            Some(left)
        }
        _ => None,
    }
}

/// `EarlyExpressionTransforms.FixComparisonKindLdNull`: an ordered comparison with null is an
/// (in)equality.
fn fix_comparison_kind_ld_null(f: &mut Function, comp: NodeId) {
    let Inst::Comp {
        kind, left, right, ..
    } = *f.inst(comp)
    else {
        return;
    };
    let new_kind = if matches!(f.inst(right), Inst::LdNull) {
        match kind {
            ComparisonKind::GreaterThan => ComparisonKind::Inequality,
            ComparisonKind::LessThanOrEqual => ComparisonKind::Equality,
            k => k,
        }
    } else if matches!(f.inst(left), Inst::LdNull) {
        match kind {
            ComparisonKind::LessThan => ComparisonKind::Inequality,
            ComparisonKind::GreaterThanOrEqual => ComparisonKind::Equality,
            k => k,
        }
    } else {
        kind
    };
    if let Inst::Comp { kind, .. } = f.inst_mut(comp) {
        *kind = new_kind;
    }
}

/// `MatchVBOnErrorCatchFilter` (On Error Resume Next / GoTo):
///   leave container(bit.and(bit.and(comp(isinst System.Exception(ldloc ex) > ldnull),
///     comp.unsigned(ldloc activeHandler > ldc.i4 0)), logic.not(ldloc resumeTarget)))
/// The exception type and the inner bit.and.
fn match_vb_on_error_catch_filter(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    variable: VarId,
    container: NodeId,
) -> Option<(TypeId, NodeId)> {
    let entry = *f.list(container).first()?;
    let [leave] = *f.list(entry) else {
        return None;
    };
    if f.node(entry).incoming_edge_count != 1 {
        return None;
    }
    let (target, condition) = f.match_leave(leave)?;
    if target != container {
        return None;
    }
    let Inst::BinaryNumeric {
        op: BinaryNumericOperator::BitAnd,
        left: bit_and,
        right: outer_right,
        ..
    } = *f.inst(condition)
    else {
        return None;
    };
    let Inst::BinaryNumeric {
        op: BinaryNumericOperator::BitAnd,
        left: comp,
        right: inner_right,
        ..
    } = *f.inst(bit_and)
    else {
        return None;
    };
    f.match_ldloc(match_comp_unsigned_zero(
        f,
        outer_right,
        ComparisonKind::Equality,
    )?)?;
    f.match_ldloc(match_comp_unsigned_zero(
        f,
        inner_right,
        ComparisonKind::Inequality,
    )?)?;
    if !matches!(f.inst(comp), Inst::Comp { .. }) {
        return None;
    }
    fix_comparison_kind_ld_null(f, comp);
    let (_, exception_type) = match_is_inst_not_null(f, comp, variable)?;
    if !is_known(ts, exception_type, KnownTypeCode::Exception) {
        return None;
    }
    Some((exception_type, bit_and))
}

/// `MatchIsInstNotNull`: `comp(isinst T(ldloc ex) != ldnull)`; the `ldloc ex` and `T`.
fn match_is_inst_not_null(
    f: &Function,
    condition: NodeId,
    variable: VarId,
) -> Option<(NodeId, TypeId)> {
    let arg = f.match_comp_null(condition, ComparisonKind::Inequality)?;
    let Inst::IsInst { arg: slot, ty } = *f.inst(arg) else {
        return None;
    };
    (f.match_ldloc(slot) == Some(variable)).then_some((slot, ty.ty))
}

/// `MatchCatchWhenEntryPoint`: the exception type, the `ldloc ex` and the block of the user
/// condition.
fn match_catch_when_entry_point(
    f: &Function,
    variable: VarId,
    container: NodeId,
) -> Option<(TypeId, NodeId, NodeId)> {
    let entry = *f.list(container).first()?;
    if f.node(entry).incoming_edge_count != 1 {
        return None;
    }
    match *f.list(entry) {
        // stloc temp(isinst T(ldloc ex)); if (comp(ldloc temp != ldnull)) br when; br false
        [store, if_inst, false_branch] => {
            let (temp, isinst) = f.match_stloc(store)?;
            if f.var(temp).kind != VariableKind::StackSlot {
                return None;
            }
            let Inst::IsInst { arg: slot, ty } = *f.inst(isinst) else {
                return None;
            };
            if f.match_ldloc(slot) != Some(variable) {
                return None;
            }
            let (condition, branch) = f.match_if_instruction(if_inst)?;
            let (left, right) = f.match_comp(condition, ComparisonKind::Inequality)?;
            let false_block = f.match_branch(false_branch)?;
            if !match_false_block(f, container, false_block) {
                return None;
            }
            let null_and_temp = |a: NodeId, b: NodeId| {
                matches!(f.inst(a), Inst::LdNull) && f.match_ldloc(b) == Some(temp)
            };
            if null_and_temp(left, right) || null_and_temp(right, left) {
                return Some((ty.ty, slot, f.match_branch(branch)?));
            }
            None
        }
        // if (comp(isinst T(ldloc ex) != ldnull)) br when; br false
        [if_inst, false_branch] => {
            let (condition, branch) = f.match_if_instruction(if_inst)?;
            let (slot, ty) = match_is_inst_not_null(f, condition, variable)?;
            let false_block = f.match_branch(false_branch)?;
            if !match_false_block(f, container, false_block) {
                return None;
            }
            Some((ty, slot, f.match_branch(branch)?))
        }
        _ => None,
    }
}

/// `MatchFalseBlock`: `stloc returnVar(ldc.i4 0); br exitBlock`, with the exit block
/// `leave container(ldloc returnVar)` (two incoming edges).
fn match_false_block(f: &Function, container: NodeId, false_block: NodeId) -> bool {
    let [store, br] = *f.list(false_block) else {
        return false;
    };
    if f.node(false_block).incoming_edge_count != 1 {
        return false;
    }
    let Some((return_var, zero)) = f.match_stloc(store) else {
        return false;
    };
    let Some(exit) = f.match_branch(br) else {
        return false;
    };
    if f.match_ldc_i4(zero) != Some(0) || f.node(exit).incoming_edge_count != 2 {
        return false;
    }
    let [leave] = *f.list(exit) else {
        return false;
    };
    f.match_leave(leave)
        .is_some_and(|(c, v)| c == container && f.match_ldloc(v) == Some(return_var))
}

/// `PropagateExceptionVariable`: in the handler, `stloc b(ldloc ex)` and `stloc
/// b(castclass T(ldloc ex))` copies of a single-definition exception variable are replaced by
/// the variable.
fn propagate_exception_variable(f: &mut Function, handler: NodeId) {
    let Inst::TryCatchHandler { variable, .. } = *f.inst(handler) else {
        unreachable!("handler");
    };
    if !f.var(variable).is_single_definition() {
        return;
    }
    let mut i = 0;
    while let Some(&load) = f.var(variable).load_instructions.get(i) {
        if !f.ancestors(load).any(|a| a == handler) {
            i += 1;
            continue;
        }
        // Only store statements copying the variable unchanged.
        let Some(statement) = containing_statement(f, load) else {
            i += 1;
            continue;
        };
        let Inst::StLoc { value, .. } = *f.inst(statement) else {
            i += 1;
            continue;
        };
        if value == load {
            propagate_exception_instance(f, handler, variable, statement, &mut i);
        } else if let Inst::CastClass { arg, ty } = *f.inst(value)
            && ty.ty == f.var(variable).ty
            && arg == load
        {
            f.replace(value, load);
            propagate_exception_instance(f, handler, variable, statement, &mut i);
        } else {
            i += 1;
        }
    }
}

fn propagate_exception_instance(
    f: &mut Function,
    handler: NodeId,
    variable: VarId,
    store: NodeId,
    i: &mut usize,
) {
    let Inst::StLoc { var: copy, .. } = *f.inst(store) else {
        unreachable!("stloc");
    };
    for load in f.var(copy).load_instructions.clone() {
        if !f.ancestors(load).any(|a| a == handler) {
            continue;
        }
        let replacement = f.add(Inst::LdLoc { var: variable });
        f.add_il_range(replacement, f.il_range(load));
        f.replace(load, replacement);
    }
    if f.var(copy).load_count() == 0
        && let Some(block) = f.node(store).parent
        && matches!(f.inst(block), Inst::Block { .. })
    {
        let index = f.node(store).child_index as usize;
        f.remove_at(block, index);
    } else {
        *i += 1;
    }
}

/// `Block.GetContainingStatement`: the ancestor (or `inst`) directly in a control flow block.
fn containing_statement(f: &Function, inst: NodeId) -> Option<NodeId> {
    let mut current = inst;
    loop {
        let parent = f.node(current).parent?;
        if matches!(
            f.inst(parent),
            Inst::Block {
                kind: BlockKind::ControlFlow,
                ..
            }
        ) {
            return Some(current);
        }
        current = parent;
    }
}
