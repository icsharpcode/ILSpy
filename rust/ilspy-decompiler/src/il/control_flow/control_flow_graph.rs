//! `IL/ControlFlow/ControlFlowGraph.cs` with `FlowAnalysis/ControlFlowNode.cs` and
//! `FlowAnalysis/Dominance.cs`: the graph of the blocks of one container, its dominator tree
//! and which nodes can reach an exit.
//!
//! Returns, exceptions and branches out of the container are not edges.

use std::collections::HashMap;

use crate::il::ast::{Function, Inst, NodeId};
use crate::util::bitset::BitSet;

#[derive(Clone, Debug)]
pub struct CfgNode {
    /// The block's index in the container when the graph was built.
    pub user_index: usize,
    pub block: NodeId,
    pub post_order_number: usize,
    pub immediate_dominator: Option<usize>,
    /// `DominatorTreeChildren`; `None` for an unreachable node.
    pub dominator_tree_children: Option<Vec<usize>>,
    pub predecessors: Vec<usize>,
    pub successors: Vec<usize>,
}

impl CfgNode {
    pub fn is_reachable(&self) -> bool {
        self.dominator_tree_children.is_some()
    }
}

pub struct ControlFlowGraph {
    pub container: NodeId,
    pub nodes: Vec<CfgNode>,
    index: HashMap<NodeId, usize>,
    direct_exit: BitSet,
    reachable_exit: BitSet,
}

/// `IsDescendantOf`, inclusive.
fn is_descendant_of(f: &Function, mut inst: NodeId, ancestor: NodeId) -> bool {
    loop {
        if inst == ancestor {
            return true;
        }
        match f.node(inst).parent {
            Some(p) => inst = p,
            None => return false,
        }
    }
}

impl ControlFlowGraph {
    pub fn new(f: &Function, container: NodeId) -> Self {
        let blocks = f.list(container).to_vec();
        let nodes = blocks
            .iter()
            .enumerate()
            .map(|(i, &block)| CfgNode {
                user_index: i,
                block,
                post_order_number: 0,
                immediate_dominator: None,
                dominator_tree_children: None,
                predecessors: Vec::new(),
                successors: Vec::new(),
            })
            .collect();
        let index = blocks.iter().enumerate().map(|(i, &b)| (b, i)).collect();
        let mut cfg = ControlFlowGraph {
            container,
            nodes,
            index,
            direct_exit: BitSet::new(blocks.len()),
            reachable_exit: BitSet::new(blocks.len()),
        };
        cfg.create_edges(f);
        if !cfg.nodes.is_empty() {
            cfg.compute_dominance(0);
        }
        cfg.reachable_exit = cfg.mark_nodes_with_reachable_exits();
        let leaving = cfg.find_nodes_with_exits_out_of_container();
        cfg.reachable_exit.union_with(&leaving);
        cfg
    }

    /// A graph of given nodes and edges (no container) with the dominator tree from `entry`
    /// (`Dominance.ComputeDominance`); exits are not computed.
    pub fn from_nodes(nodes: Vec<CfgNode>, entry: usize) -> Self {
        let n = nodes.len();
        let mut cfg = ControlFlowGraph {
            container: NodeId(0),
            nodes,
            index: HashMap::new(),
            direct_exit: BitSet::new(n),
            reachable_exit: BitSet::new(n),
        };
        cfg.compute_dominance(entry);
        cfg
    }

    fn add_edge(&mut self, from: usize, to: usize) {
        self.nodes[from].successors.push(to);
        self.nodes[to].predecessors.push(from);
    }

    fn create_edges(&mut self, f: &Function) {
        let container = self.container;
        for i in 0..self.nodes.len() {
            let block = self.nodes[i].block;
            for node in f.descendants_post(block) {
                match *f.inst(node) {
                    Inst::Branch { .. } => {
                        let target = f.branch_target(node);
                        if f.node(target).parent == Some(container) {
                            let to = f
                                .list(container)
                                .iter()
                                .position(|&b| b == target)
                                .expect("target in the container");
                            self.add_edge(i, to);
                        } else if is_descendant_of(f, target, container) {
                            // Control flow within a nested container.
                        } else {
                            // Out of this container: not an edge, but an exit.
                            self.direct_exit.set(i);
                        }
                    }
                    Inst::Leave {
                        target: Some(target),
                        ..
                    // A leave out of the whole function is a return, not a reachable exit.
                    } if !is_descendant_of(f, target, block) && Some(target) != f.body => {
                        self.direct_exit.set(i);
                    }
                    _ => {}
                }
            }
        }
    }

    /// `Dominance.ComputeDominance` ("A Simple, Fast Dominance Algorithm", Cooper, Harvey
    /// and Kennedy).
    fn compute_dominance(&mut self, entry: usize) {
        let order = self.post_order(entry);
        for (i, &n) in order.iter().enumerate() {
            self.nodes[n].post_order_number = i;
        }
        // The entry point is its own dominator during the iteration.
        self.nodes[entry].immediate_dominator = Some(entry);
        loop {
            let mut changed = false;
            for &b in order.iter().rev().skip(1) {
                let mut new_idom = None;
                for &p in &self.nodes[b].predecessors {
                    if self.nodes[p].immediate_dominator.is_some() {
                        new_idom = Some(match new_idom {
                            None => p,
                            Some(other) => self.find_common_dominator(p, other),
                        });
                    }
                }
                if new_idom != self.nodes[b].immediate_dominator {
                    self.nodes[b].immediate_dominator = new_idom;
                    changed = true;
                }
            }
            if !changed {
                break;
            }
        }
        for &n in &order {
            if self.nodes[n].immediate_dominator.is_some() {
                self.nodes[n].dominator_tree_children = Some(Vec::new());
            }
        }
        self.nodes[entry].immediate_dominator = None;
        for &n in &order {
            if let Some(idom) = self.nodes[n].immediate_dominator {
                self.nodes[idom]
                    .dominator_tree_children
                    .as_mut()
                    .expect("dominator is reachable")
                    .push(n);
            }
        }
    }

    /// `TraversePostOrder` along the successors (`GraphTraversal.DepthFirstSearch`).
    fn post_order(&self, entry: usize) -> Vec<usize> {
        let mut visited = vec![false; self.nodes.len()];
        let mut out = Vec::new();
        // (node, next successor to visit)
        let mut stack = vec![(entry, 0usize)];
        visited[entry] = true;
        while let Some(&mut (n, ref mut next)) = stack.last_mut() {
            if let Some(&s) = self.nodes[n].successors.get(*next) {
                *next += 1;
                if !visited[s] {
                    visited[s] = true;
                    stack.push((s, 0));
                }
            } else {
                out.push(n);
                stack.pop();
            }
        }
        out
    }

    pub(crate) fn find_common_dominator(&self, mut a: usize, mut b: usize) -> usize {
        while a != b {
            while self.nodes[a].post_order_number < self.nodes[b].post_order_number {
                a = self.nodes[a].immediate_dominator.expect("dominator");
            }
            while self.nodes[b].post_order_number < self.nodes[a].post_order_number {
                b = self.nodes[b].immediate_dominator.expect("dominator");
            }
        }
        a
    }

    /// `Dominance.MarkNodesWithReachableExits`: reachable nodes from which a node they do not
    /// dominate is reachable.
    fn mark_nodes_with_reachable_exits(&self) -> BitSet {
        let mut non_empty = BitSet::new(self.nodes.len());
        for j in &self.nodes {
            // A join point; the root counts an extra incoming edge.
            let join = j.predecessors.len() >= 2
                || (!j.predecessors.is_empty() && j.immediate_dominator.is_none());
            if !(j.is_reachable() && join) {
                continue;
            }
            for &p in &j.predecessors {
                let mut runner = Some(p);
                while let Some(r) = runner {
                    if Some(r) == j.immediate_dominator || r == j.user_index {
                        break;
                    }
                    non_empty.set(r);
                    runner = self.nodes[r].immediate_dominator;
                }
            }
        }
        non_empty
    }

    /// `FindNodesWithExitsOutOfContainer`: the nodes with a direct exit and their dominators.
    fn find_nodes_with_exits_out_of_container(&self) -> BitSet {
        let mut leaving = BitSet::new(self.nodes.len());
        for node in &self.nodes {
            if leaving.bit(node.user_index) || !self.direct_exit.bit(node.user_index) {
                continue;
            }
            let mut p = Some(node.user_index);
            while let Some(i) = p {
                if leaving.bit(i) {
                    break;
                }
                leaving.set(i);
                p = self.nodes[i].immediate_dominator;
            }
        }
        leaving
    }

    /// `GetNode`: the node of a block that was in the container when the graph was built.
    pub fn node_of(&self, block: NodeId) -> usize {
        self.index[&block]
    }

    /// `ControlFlowNode.Dominates`.
    pub fn dominates(&self, a: usize, b: usize) -> bool {
        let mut tmp = Some(b);
        while let Some(t) = tmp {
            if t == a {
                return true;
            }
            tmp = self.nodes[t].immediate_dominator;
        }
        false
    }

    /// `HasReachableExit`.
    pub fn has_reachable_exit(&self, node: usize) -> bool {
        self.reachable_exit.bit(node)
    }

    /// `HasDirectExitOutOfContainer`.
    pub fn has_direct_exit_out_of_container(&self, node: usize) -> bool {
        self.direct_exit.bit(node)
    }
}
