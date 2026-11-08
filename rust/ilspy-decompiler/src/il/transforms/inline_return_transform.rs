//! `IL/Transforms/InlineReturnTransform.cs`: a return block that returns a local stored right
//! before each branch to it moves into the branching container, or is duplicated there.

use crate::il::ast::{Function, Inst, NodeId, VariableKind};

pub fn run(f: &mut Function) {
    let root = f.body.expect("function body");
    let mut to_modify: Vec<(NodeId, NodeId, NodeId)> = Vec::new();
    let leaves: Vec<NodeId> = f
        .descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Leave { .. }))
        .collect();
    for leave in leaves {
        // A leave block: a block of only the leave.
        let Some(leave_block) = f.node(leave).parent else {
            continue;
        };
        if !matches!(f.inst(leave_block), Inst::Block { .. }) || f.list(leave_block).len() != 1 {
            continue;
        }
        let Some((_, value)) = f.match_leave(leave) else {
            continue;
        };
        let Some(return_var) = f.match_ldloc(value) else {
            continue;
        };
        if f.var(return_var).kind != VariableKind::Local {
            continue;
        }
        if let Some(list) = can_modify_instructions(f, return_var, leave_block) {
            to_modify.extend(list);
        }
    }
    for (container, block, branch) in to_modify {
        // One branch moves the block; more branches get copies.
        let block = if f.node(block).incoming_edge_count == 1 {
            f.remove_block(block);
            block
        } else {
            f.clone_subtree(block)
        };
        f.push(container, block);
        f.set_branch_target(branch, block);
    }
}

/// `CanModifyInstructions`: every store of `return_var` is the second-to-last instruction of
/// a block that then branches to `leave_block`. Returns (container, leave block, branch) per
/// store, in `StoreInstructions` order.
fn can_modify_instructions(
    f: &Function,
    return_var: crate::il::ast::VarId,
    leave_block: NodeId,
) -> Option<Vec<(NodeId, NodeId, NodeId)>> {
    let mut out = Vec::new();
    for &store in &f.var(return_var).store_instructions {
        if !matches!(f.inst(store), Inst::StLoc { .. }) {
            return None;
        }
        let store_block = f.node(store).parent?;
        if !matches!(f.inst(store_block), Inst::Block { .. }) {
            return None;
        }
        let list = f.list(store_block);
        let index = f.node(store).child_index as usize;
        if index + 2 != list.len() {
            return None;
        }
        let branch = list[index + 1];
        if f.match_branch(branch) != Some(leave_block) {
            return None;
        }
        let container = f
            .ancestors(store)
            .find(|&a| matches!(f.inst(a), Inst::BlockContainer { .. }))?;
        out.push((container, leave_block, branch));
    }
    Some(out)
}
