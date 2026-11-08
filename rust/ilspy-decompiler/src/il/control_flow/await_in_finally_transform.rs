//! `IL/ControlFlow/AwaitInFinallyTransform.cs`: the compiler lowers a finally block with an
//! await into a catch-all handler that stores the exception, the finally body after the
//! try-catch, and a rethrow of the stored exception. This rebuilds the try-finally.
//!
//!   catch exceptionVariable : System.Object when (ldc.i4 1) {
//!     stloc objectVariable(ldloc exceptionVariable); br finallyBlock
//!   }
//!   finallyBlock: ... (the finally body, with awaits)
//!   afterFinallyBlock: stloc V_1(ldloc objectVariable); if (comp.o(ldloc V_1 == ldnull)) br noThrow; br capture

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId, NopKind, VarId};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeSystem;
use crate::util::long_set::LongSet;

use super::await_in_catch_transform::match_exception_capture_block;
use super::control_flow_graph::ControlFlowGraph;
use super::state_range_analysis::{Mode, StateRangeAnalysis};
use super::yield_return_decompiler::{is_known, new_container};

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.await_in_catch_finally {
        return;
    }
    let mut needs_unreachable_code_cleanup = false;
    let body = r.function.body.expect("body");
    let try_catches: Vec<NodeId> = r
        .function
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(r.function.inst(n), Inst::TryCatch { .. }))
        .collect();
    for try_catch in try_catches {
        let f = &mut r.function;
        let Some(parent_block) = f.node(try_catch).parent else {
            continue;
        };
        let Some(container) = f
            .node(parent_block)
            .parent
            .filter(|&c| matches!(f.inst(c), Inst::BlockContainer { .. }))
        else {
            continue;
        };
        // await in finally uses a single catch block with catch type object.
        let Inst::TryCatch {
            try_block,
            ref handlers,
        } = *f.inst(try_catch)
        else {
            unreachable!("try-catch");
        };
        let [handler] = handlers[..] else {
            continue;
        };
        let Inst::TryCatchHandler {
            body: catch_container,
            variable: exception_variable,
            ..
        } = *f.inst(handler)
        else {
            unreachable!("handler");
        };
        if !matches!(f.inst(catch_container), Inst::BlockContainer { .. }) {
            continue;
        }
        if !is_known(ctx.ts, f.var(exception_variable).ty, KnownTypeCode::Object) {
            continue;
        }
        let Some((object_variable, entry_point_of_finally)) =
            match_object_store_catch_handler(f, catch_container, exception_variable)
        else {
            continue;
        };
        // The object variable is used once, at the end of the finally block.
        if f.var(object_variable).load_count() != 1 || f.var(object_variable).store_count() > 2 {
            continue;
        }
        let Some(before_exception_capture_block) =
            find_closest_block(f, f.var(object_variable).load_instructions[0])
        else {
            continue;
        };
        let Some((no_throw_block, _exception_capture_block, object_variable_copy)) =
            find_block_after_finally(f, ctx.ts, before_exception_capture_block, object_variable)
        else {
            continue;
        };
        let index = f.node(try_catch).child_index as usize;
        let Some(init) = index
            .checked_sub(1)
            .and_then(|i| f.inst(parent_block).children().get(i).copied())
        else {
            continue;
        };
        let Some((state_variable, value)) = f.match_stloc(init) else {
            continue;
        };
        if f.match_ldc_i4(value) != Some(0) {
            continue;
        }
        if !validate_state_variable(f, state_variable, init, try_catch, entry_point_of_finally) {
            continue;
        }

        let mut analysis =
            StateRangeAnalysis::new(Mode::AwaitInFinally, None, Some(state_variable), false);
        analysis
            .assign_state_ranges(f, ctx.ts, no_throw_block, LongSet::universe())
            .unwrap_or_else(|e| panic!("SymbolicAnalysisFailedException: {}", e.0));
        let no_throw_container = f.node(no_throw_block).parent.expect("block in a container");
        let mapping = analysis.block_state_set_mapping(f, no_throw_container);
        let mapping_for_leave = analysis.block_state_set_mapping_for_leave();

        let cfg = ControlFlowGraph::new(f, container);
        needs_unreachable_code_cleanup = true;

        let finally_container = new_container(f);
        f.add_il_range(finally_container, f.il_range(catch_container));
        let try_finally = f.add(Inst::TryFinally {
            try_block,
            finally_block: finally_container,
        });
        f.add_il_range(try_finally, f.il_range(try_block));
        f.replace(try_catch, try_finally);

        move_dominated_blocks_to_container(
            f,
            entry_point_of_finally,
            Some(before_exception_capture_block),
            &cfg,
            finally_container,
        );
        simplify_end_of_finally(
            f,
            object_variable,
            before_exception_capture_block,
            object_variable_copy,
            finally_container,
        );

        if f.list(no_throw_block)
            .first()
            .is_some_and(|&i| f.match_ldloc(i) == Some(state_variable))
        {
            f.remove_at(no_throw_block, 0);
        }

        for branch in f.descendants_post(try_block) {
            if !matches!(f.inst(branch), Inst::Branch { .. })
                || f.branch_target(branch) != entry_point_of_finally
            {
                continue;
            }
            let state = f
                .node(branch)
                .parent
                .filter(|&p| matches!(f.inst(p), Inst::Block { .. }))
                .and_then(|p| {
                    let i = f.node(branch).child_index as usize;
                    let prev = *f.list(p).get(i.checked_sub(1)?)?;
                    let (v, value) = f.match_stloc(prev)?;
                    (v == state_variable)
                        .then(|| f.match_ldc_i4(value))
                        .flatten()
                })
                .unwrap_or(0);
            if let Some(&target) = mapping.get(i64::from(state)) {
                f.set_branch_target(branch, target);
            } else if let Some(&target) = mapping_for_leave.get(i64::from(state)) {
                let nop = f.add(Inst::Nop {
                    kind: NopKind::Normal,
                });
                let leave = f.add(Inst::Leave {
                    target: Some(target),
                    value: nop,
                });
                f.add_il_range(leave, f.il_range(branch));
                f.replace(branch, leave);
            } else {
                f.set_branch_target(branch, no_throw_block);
            }
        }
    }

    if needs_unreachable_code_cleanup {
        // Deleting blocks anywhere can make blocks in parent containers unreachable.
        let f = &mut r.function;
        let body = f.body.expect("body");
        let containers: Vec<NodeId> = f
            .descendants_post(body)
            .into_iter()
            .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
            .collect();
        for container in containers {
            sort_blocks(f, container);
        }
    }
}

/// `Block.FindClosestBlock`.
fn find_closest_block(f: &Function, inst: NodeId) -> Option<NodeId> {
    std::iter::once(inst)
        .chain(f.ancestors(inst))
        .find(|&n| matches!(f.inst(n), Inst::Block { .. }))
}

fn simplify_end_of_finally(
    f: &mut Function,
    object_variable: VarId,
    block: NodeId,
    object_variable_copy: VarId,
    finally_container: NodeId,
) {
    let list = f.list(block).to_vec();
    let n = list.len();
    if n >= 3
        && let Some((condition, _)) = f.match_if_instruction(list[n - 2])
        && matches!(f.inst(list[n - 1]), Inst::Branch { .. })
        && let Some((v, value)) = f.match_stloc(list[n - 3])
        && v == object_variable_copy
        && f.match_ldloc(value) == Some(object_variable)
    {
        let tests_copy = |kind| {
            f.match_comp_null(condition, kind)
                .is_some_and(|a| f.match_ldloc(a) == Some(object_variable_copy))
        };
        if tests_copy(ComparisonKind::Equality) || tests_copy(ComparisonKind::Inequality) {
            f.remove_at(block, n - 3);
            f.remove_at(block, n - 3);
            let branch = list[n - 1];
            let nop = f.add(Inst::Nop {
                kind: NopKind::Normal,
            });
            let leave = f.add(Inst::Leave {
                target: Some(finally_container),
                value: nop,
            });
            f.add_il_range(leave, f.il_range(branch));
            f.replace(branch, leave);
        }
        // Otherwise a broken block, which the managed code asserts on.
    }
}

fn validate_state_variable(
    f: &Function,
    state: VarId,
    initializer: NodeId,
    try_catch: NodeId,
    entry_point_of_finally: NodeId,
) -> bool {
    if f.var(state).address_count() > 0 {
        return false;
    }
    for &store in &f.var(state).store_instructions {
        if store == initializer {
            continue;
        }
        let Some((_, value)) = f.match_stloc(store) else {
            return false;
        };
        if f.match_ldc_i4(value).is_none() {
            return false;
        }
        if !f.ancestors(store).any(|a| a == try_catch) {
            return false;
        }
        let Some(block) = f
            .node(store)
            .parent
            .filter(|&p| matches!(f.inst(p), Inst::Block { .. }))
        else {
            return false;
        };
        let next = f
            .list(block)
            .get(f.node(store).child_index as usize + 1)
            .copied();
        if next.and_then(|i| f.match_branch(i)) != Some(entry_point_of_finally) {
            return false;
        }
    }
    true
}

/// `MatchObjectStoreCatchHandler`: `[stloc tmp(ldloc exceptionVariable);] stloc
/// objectVariable(ldloc tmp_or_exceptionVariable); br branchTarget`.
pub(crate) fn match_object_store_catch_handler(
    f: &Function,
    catch_container: NodeId,
    exception_variable: VarId,
) -> Option<(VarId, NodeId)> {
    let entry = f.list(catch_container)[0];
    let list = f.list(entry);
    let object_variable = match list.len() {
        2 => {
            let (v, value) = f.match_stloc(list[0])?;
            if f.match_ldloc(value) != Some(exception_variable) {
                return None;
            }
            v
        }
        3 => {
            let (temporary, value) = f.match_stloc(list[0])?;
            if f.match_ldloc(value) != Some(exception_variable) {
                return None;
            }
            let (v, value) = f.match_stloc(list[1])?;
            if f.match_ldloc(value) != Some(temporary) {
                return None;
            }
            v
        }
        _ => return None,
    };
    let target = f.match_branch(list[list.len() - 1])?;
    Some((object_variable, target))
}

/// `MoveDominatedBlocksToContainer`: `new_entry_point` and the blocks it dominates, except
/// those `end_block` dominates (but `end_block` itself), move into `target`.
pub(crate) fn move_dominated_blocks_to_container(
    f: &mut Function,
    new_entry_point: NodeId,
    end_block: Option<NodeId>,
    cfg: &ControlFlowGraph,
    target: NodeId,
) {
    let node = cfg.node_of(new_entry_point);
    let end_node = end_block.map(|b| cfg.node_of(b));
    f.remove_block(new_entry_point);
    f.push(target, new_entry_point);
    for n in 0..cfg.nodes.len() {
        let block = cfg.nodes[n].block;
        if !cfg.dominates(node, n) {
            continue;
        }
        if let Some(end) = end_node
            && end != n
            && cfg.dominates(end, n)
        {
            continue;
        }
        if f.node(block).parent == Some(target) {
            continue;
        }
        f.remove_block(block);
        f.push(target, block);
    }
}

/// `FindBlockAfterFinally`:
///   stloc V_7(ldloc I_0); if (comp.o(ldloc V_7 == ldnull)) br noThrow; br exceptionCapture
/// The no-throw block, the exception capture block and the object variable's copy.
fn find_block_after_finally(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    object_variable: VarId,
) -> Option<(NodeId, NodeId, VarId)> {
    let list = f.list(block);
    let n = list.len();
    if n < 3 {
        return None;
    }
    let (copy, value) = f.match_stloc(list[n - 3])?;
    if f.match_ldloc(value) != Some(object_variable) {
        return None;
    }
    let (condition, no_throw_branch) = f.match_if_instruction(list[n - 2])?;
    let mut no_throw = f.match_branch(no_throw_branch)?;
    let mut capture = f.match_branch(list[n - 1])?;
    let (arg, inverted) = match f.match_comp_null(condition, ComparisonKind::Equality) {
        Some(arg) => (arg, false),
        None => (
            f.match_comp_null(condition, ComparisonKind::Inequality)?,
            true,
        ),
    };
    if f.match_ldloc(arg) != Some(copy) {
        return None;
    }
    if inverted {
        std::mem::swap(&mut no_throw, &mut capture);
    }
    let mut copy_var = Some(copy);
    match_exception_capture_block(f, ts, capture, &mut copy_var)?;
    Some((no_throw, capture, copy))
}
