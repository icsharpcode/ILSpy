//! `IL/Transforms/RemoveInfeasiblePathTransform.cs`: a block that sets a stack slot to a
//! constant and branches to a block that only tests that slot gets the branch the test would
//! take.
//!
//! ```text
//! Block IL_0018 (incoming: *) {          Block IL_0019 (incoming: > 1) {
//!     stloc s(ldc.i4 1)                       if (logic.not(ldloc s)) br IL_0027
//!     br IL_0019                              br IL_001d
//! }                                       }
//! ```
//! `br IL_0019` becomes `br IL_001d`.

use crate::il::ast::{Function, Inst, NodeId, VarId, VariableKind};
use crate::il::block_builder::topological_sort;

pub fn run(f: &mut Function) {
    let root = f.body.expect("function body");
    let containers: Vec<NodeId> = f
        .descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for container in containers {
        if !f.is_connected(container) {
            continue;
        }
        let mut changed = false;
        for block in f.list(container).to_vec() {
            changed |= do_transform(f, block);
        }
        if changed {
            sort_blocks(f, container);
        }
    }
}

/// `BlockContainer.SortBlocks(deleteUnreachableBlocks: true)`.
pub fn sort_blocks(f: &mut Function, container: NodeId) {
    if f.list(container).len() < 2 {
        return;
    }
    let order = topological_sort(f, container);
    f.set_list(container, order);
}

fn do_transform(f: &mut Function, block: NodeId) -> bool {
    let Some((s, value, br)) = match_block1(f, block) else {
        return false;
    };
    let Some(exit) = match_block2(f, f.branch_target(br), s, value) else {
        return false;
    };
    let copy = f.clone_subtree(exit);
    f.replace(br, copy);
    f.var_mut(s).remove_if_redundant = true;
    true
}

/// `stloc s(ldc.i4 0 or 1); br target` with a stack slot `s`.
fn match_block1(f: &Function, block: NodeId) -> Option<(VarId, i32, NodeId)> {
    let [store, br] = *f.list(block) else {
        return None;
    };
    let (s, value) = f.match_stloc(store)?;
    if f.var(s).kind != VariableKind::StackSlot {
        return None;
    }
    let value = f.match_ldc_i4(value).filter(|v| matches!(v, 0 | 1))?;
    f.match_branch(br)?;
    Some((s, value, br))
}

/// `if (ldloc s) trueInst; falseInst` (logic nots swap the arms) in a block with more than
/// one incoming edge; the arm the constant takes, when it is a branch or a value-less leave.
fn match_block2(f: &Function, block: NodeId, s: VarId, value: i32) -> Option<NodeId> {
    let list = f.list(block);
    if list.len() != 2 || f.node(block).incoming_edge_count <= 1 {
        return None;
    }
    let (condition, true_inst, false_inst) = f.match_if_at_end_of_block(block)?;
    if f.match_ldloc(condition) != Some(s) {
        return None;
    }
    let exit = if value != 0 { true_inst } else { false_inst };
    let is_exit = match f.inst(exit) {
        Inst::Branch { .. } => true,
        &Inst::Leave { value, .. } => matches!(f.inst(value), Inst::Nop { .. }),
        _ => false,
    };
    is_exit.then_some(exit)
}
