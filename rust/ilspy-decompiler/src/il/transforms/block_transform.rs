//! `IL/Transforms/BlockTransform.cs` (`BlockILTransform`): runs per-block transforms over the
//! dominator tree of every container, in post-order.

use crate::il::ast::{Inst, NodeId};
use crate::il::control_flow::control_flow_graph::ControlFlowGraph;
use crate::il::reader::ReadFunction;

use super::Context;

/// `BlockTransformContext`, with the `Visited` flags of the graph's nodes.
pub struct BlockTransformContext {
    /// The graph of the container, built before the transforms ran on it (it is not kept up
    /// to date).
    pub cfg: ControlFlowGraph,
    /// `ControlFlowNode`: the node of `block`.
    pub node: usize,
    pub block: NodeId,
    /// `IndexOfFirstAlreadyTransformedInstruction`.
    pub index_of_first_already_transformed_instruction: usize,
    /// `ControlFlowNode.Visited`, by node index.
    pub visited: Vec<bool>,
}

/// `IBlockTransform.Run`.
pub type BlockTransform =
    fn(&mut ReadFunction, &mut Context<'_, '_, '_>, &mut BlockTransformContext);

/// A post-order transform of a group: a block transform, or a `StatementTransform` with its
/// children.
pub enum BlockStep {
    Block(BlockTransform),
    Statements(Vec<super::statement_transform::StatementTransform>),
}

/// `BlockILTransform.Run` with `post_order` as the post-order transforms (the pipeline has no
/// pre-order ones).
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, post_order: &[BlockStep]) {
    let body = r.function.body.expect("body");
    let containers: Vec<NodeId> = r
        .function
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(r.function.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for container in containers {
        let cfg = ControlFlowGraph::new(&r.function, container);
        let entry = cfg.node_of(r.function.list(container)[0]);
        let n = cfg.nodes.len();
        let mut bc = BlockTransformContext {
            cfg,
            node: entry,
            block: NodeId(0),
            index_of_first_already_transformed_instruction: 0,
            visited: vec![false; n],
        };
        // `VisitBlock`: the dominator tree from the entry point, in post-order.
        let mut stack = vec![(entry, 0usize)];
        while let Some(&mut (node, ref mut next)) = stack.last_mut() {
            let child = bc.cfg.nodes[node]
                .dominator_tree_children
                .as_ref()
                .and_then(|c| c.get(*next).copied());
            if let Some(child) = child {
                *next += 1;
                stack.push((child, 0));
                continue;
            }
            stack.pop();
            let block = bc.cfg.nodes[node].block;
            bc.node = node;
            bc.block = block;
            bc.index_of_first_already_transformed_instruction = r.function.list(block).len();
            for step in post_order {
                match step {
                    BlockStep::Block(transform) => transform(r, ctx, &mut bc),
                    BlockStep::Statements(children) => {
                        super::statement_transform::run(r, ctx, &mut bc, children)
                    }
                }
                if !ctx.removed_variables.is_empty() {
                    // `ILVariableCollection.Remove` swap-removes.
                    for v in std::mem::take(&mut ctx.removed_variables) {
                        if let Some(i) = r.variables.iter().position(|&x| x == v) {
                            r.variables.swap_remove(i);
                        }
                    }
                }
            }
        }
    }
}
