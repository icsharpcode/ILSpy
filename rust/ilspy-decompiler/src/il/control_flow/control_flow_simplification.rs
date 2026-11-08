//! `IL/ControlFlow/ControlFlowSimplification.cs`: removes `nop`s and dead stack stores,
//! inlines return variables, shortens branch chains and combines basic blocks, so the code a
//! debug build emits looks like the release build's to the later transforms.

use std::collections::HashSet;

use crate::il::ast::{Flags, Function, Inst, NodeId, NopKind, VariableKind};
use crate::il::semantic_helper::is_pure;
use crate::il::transforms::Context;
use crate::typesystem::known::KnownTypeCode;

use super::switch_detection;

#[derive(Clone, Copy, Debug, Default)]
pub struct ControlFlowSimplification {
    /// Replace every branch to a return block with the return (`EarlyILTransforms`).
    pub aggressively_duplicate_return_blocks: bool,
}

fn is_kind(f: &Function, id: NodeId, block: bool) -> bool {
    match f.inst(id) {
        Inst::Block { .. } => block,
        Inst::BlockContainer { .. } => !block,
        _ => false,
    }
}

/// The connected blocks (or containers) of the function in post-order. The managed
/// transforms iterate `Descendants` lazily; skipping what an edit disconnected gives the
/// same visits for the edits made here.
fn connected(f: &Function, blocks: bool) -> Vec<NodeId> {
    let root = f.body.expect("function body");
    f.descendants_post(root)
        .into_iter()
        .filter(|&n| is_kind(f, n, blocks))
        .collect()
}

impl ControlFlowSimplification {
    pub fn run(&self, f: &mut Function, ctx: &mut Context<'_, '_, '_>) {
        for block in connected(f, true) {
            if !f.is_connected(block) {
                continue;
            }
            remove_nop_instructions(f, block);
            remove_dead_stack_stores(f, block, ctx);
            inline_variable_in_return_block(f, block);
            // Before `simplify_branch_chains` starts duplicating return instructions.
            switch_detection::simplify_switch_instruction(f, block, ctx);
        }
        self.simplify_branch_chains(f);
        clean_up_empty_blocks(f, ctx);
    }

    fn simplify_branch_chains(&self, f: &mut Function) {
        let root = f.body.expect("function body");
        let mut blocks_to_move = Vec::new();
        let mut visited = HashSet::new();
        let branches: Vec<NodeId> = f
            .descendants_post(root)
            .into_iter()
            .filter(|&n| matches!(f.inst(n), Inst::Branch { .. }))
            .collect();
        for branch in branches {
            if !f.is_connected(branch) {
                continue;
            }
            // Resolve chained branches to the final target.
            let mut target = f.branch_target(branch);
            visited.clear();
            while f.list(target).len() == 1
                && matches!(f.inst(f.list(target)[0]), Inst::Branch { .. })
            {
                if !visited.insert(target) {
                    break; // a cyclic chain
                }
                let next = f.list(target)[0];
                let next_target = f.branch_target(next);
                f.set_branch_target(branch, next_target);
                f.add_il_range(branch, f.il_range(next));
                if f.node(target).incoming_edge_count == 0 {
                    f.truncate(target, 0); // mark the block for deletion
                }
                target = f.branch_target(branch);
            }
            if is_branch_to_return_block(f, branch) {
                if self.aggressively_duplicate_return_blocks {
                    let ret = f.clone_subtree(f.list(target)[0]);
                    f.replace(branch, ret);
                } else {
                    let local_container = nearest_container(f, branch);
                    if f.node(target).parent != Some(local_container)
                        && f.node(target).incoming_edge_count == 1
                    {
                        // Into the right try-finally scope, so loop detection may still put
                        // the return inside a loop body.
                        blocks_to_move.push((target, local_container));
                    }
                }
            } else if f.list(target).len() == 1
                && let Some((_, value)) = f.match_leave(f.list(target)[0])
                && f.match_nop(value)
            {
                let leave = f.clone_subtree(f.list(target)[0]);
                if !f.il_range(branch).is_empty() {
                    f.add_il_range(leave, f.il_range(branch));
                }
                f.replace(branch, leave);
            }
            if f.node(target).incoming_edge_count == 0 {
                f.truncate(target, 0);
            }
        }
        for (block, container) in blocks_to_move {
            f.remove_block(block);
            f.push(container, block);
        }
    }
}

fn nearest_container(f: &Function, id: NodeId) -> NodeId {
    f.ancestors(id)
        .find(|&a| matches!(f.inst(a), Inst::BlockContainer { .. }))
        .expect("branch inside a container")
}

/// `RemoveNopInstructions`: `nop`s go; the IL range of a `pop` moves to the instruction
/// before it.
fn remove_nop_instructions(f: &mut Function, block: NodeId) {
    let list = f.list(block).to_vec();
    for i in (1..list.len()).rev() {
        if let Inst::Nop { kind: NopKind::Pop } = f.inst(list[i]) {
            f.add_il_range(list[i - 1], f.il_range(list[i]));
        }
    }
    f.retain(block, |f, i| !matches!(f.inst(i), Inst::Nop { .. }));
}

/// `RemoveDeadStackStores`: a store to a stack slot nobody loads goes, or leaves its value
/// behind when the value has effects.
fn remove_dead_stack_stores(f: &mut Function, block: NodeId, ctx: &Context<'_, '_, '_>) {
    let aggressive = ctx.settings.remove_dead_stores;
    let mut i = f.list(block).len();
    while i > 0 {
        i -= 1;
        let inst = f.list(block)[i];
        let Some((var, value)) = f.match_stloc(inst) else {
            continue;
        };
        let v = f.var(var);
        if !(v.is_single_definition() && v.load_count == 0 && v.kind == VariableKind::StackSlot) {
            continue;
        }
        let removable = if aggressive {
            is_pure(f.flags(value))
        } else {
            // `ldstr`: the C# 1.0 compiler emits redundant ones in switch-on-string code.
            matches!(f.inst(value), Inst::LdLoc { .. } | Inst::LdStr { .. })
        };
        if removable {
            f.remove_at(block, i);
            // The managed loop visits the index again (`RemoveAt(i++)`).
            i += 1;
        } else {
            f.add_il_range(value, f.il_range(inst));
            f.replace(inst, value);
        }
    }
}

/// `InlineVariableInReturnBlock`: `v = <inst>; ret(v)` with no other use of `v` becomes
/// `ret(<inst>)`, as in release builds.
fn inline_variable_in_return_block(f: &mut Function, block: NodeId) {
    let list = f.list(block);
    if list.len() != 2 {
        return;
    }
    let (store, ret) = (list[0], list[1]);
    let Some(ret_value) = f.match_return(ret) else {
        return;
    };
    let Some(v) = f.match_ldloc(ret_value) else {
        return;
    };
    let var = f.var(v);
    if !(var.is_single_definition() && var.load_count == 1) {
        return;
    }
    let Some((stored, inst)) = f.match_stloc(store) else {
        return;
    };
    if stored != v {
        return;
    }
    f.add_il_range(inst, f.il_range(ret_value));
    f.add_il_range(inst, f.il_range(store));
    f.replace(ret_value, inst);
    f.remove_at(block, 0);
}

/// `IsBranchToReturnBlock`: the target only returns a variable that no `finally` the branch
/// leaves through touches.
fn is_branch_to_return_block(f: &Function, branch: NodeId) -> bool {
    let target = f.branch_target(branch);
    let list = f.list(target);
    if list.len() != 1 {
        return false;
    }
    let Some(value) = f.match_return(list[0]) else {
        return false;
    };
    let Some(return_var) = f.match_ldloc(value) else {
        return false;
    };
    let container = f.node(target).parent.expect("target in a container");
    let mut inst = branch;
    while inst != container {
        let parent = f
            .node(inst)
            .parent
            .expect("branch inside its target container");
        if let Inst::TryFinally {
            try_block,
            finally_block,
        } = *f.inst(parent)
            && try_block == inst
            && f.is_used_within(return_var, finally_block)
        {
            return false;
        }
        inst = parent;
    }
    true
}

/// `CleanUpEmptyBlocks`: combines blocks with their single-entry successors and drops the
/// emptied, unreachable ones.
fn clean_up_empty_blocks(f: &mut Function, ctx: &Context<'_, '_, '_>) {
    for container in connected(f, false) {
        if !f.is_connected(container) {
            continue;
        }
        for block in f.list(container).to_vec() {
            if f.list(block).is_empty() {
                continue; // already marked for deletion
            }
            // Each successful combination empties a block, so this terminates.
            while combine_block_with_next_block(f, container, block, ctx) {}
        }
        f.retain(container, |f, b| {
            !(f.node(b).incoming_edge_count == 0 && f.list(b).is_empty())
        });
    }
}

/// `CombineBlockWithNextBlock`.
pub fn combine_block_with_next_block(
    f: &mut Function,
    container: NodeId,
    block: NodeId,
    ctx: &Context<'_, '_, '_>,
) -> bool {
    let list = f.list(block);
    let n = list.len();
    // Keep basic blocks: no extended basic blocks before loop detection.
    if n > 1 && f.has_flag(list[n - 2], Flags::MAY_BRANCH) {
        return false;
    }
    let Some(&br) = list.last() else {
        return false;
    };
    let Some(target) = f.match_branch(br) else {
        return false;
    };
    if f.node(target).parent != Some(container) || f.node(target).incoming_edge_count != 1 {
        return false;
    }
    if target == block {
        return false;
    }
    if f.il_range(target).start < f.il_range(block).start && is_dead_true_store(f, block, ctx) {
        // The dead store of the condition of a `while (true)` loop.
        f.remove_at(block, n - 2);
    }
    if f.il_range(block).is_empty() {
        f.add_il_range(block, f.il_range(target));
    }
    f.remove(br);
    for inst in f.truncate(target, 0) {
        f.push(block, inst);
    }
    true
}

/// `IsDeadTrueStore`: the two instructions before the branch store `true` into an unused
/// variable.
fn is_dead_true_store(f: &Function, block: NodeId, ctx: &Context<'_, '_, '_>) -> bool {
    let list = f.list(block);
    if list.len() < 2 {
        return false;
    }
    let Some((var, value)) = f.match_stloc(list[list.len() - 2]) else {
        return false;
    };
    let v = f.var(var);
    if !(v.load_count == 0 && v.address_count == 0) {
        return false;
    }
    f.match_ldc_i4(value) == Some(1) && is_known(ctx, v.ty, KnownTypeCode::Boolean)
}

fn is_known(
    ctx: &Context<'_, '_, '_>,
    ty: crate::typesystem::types::TypeId,
    code: KnownTypeCode,
) -> bool {
    ctx.ts
        .definition(ty)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.known == code)
}
