//! `IL/ControlFlow/LoopDetection.cs`: a block with a back edge from a block it dominates
//! heads a loop, whose blocks move into a new loop container with a single exit where
//! possible. The body of a switch also moves into its own container.

use crate::il::ast::{ContainerKind, Function, Inst, NodeId, NopKind};
use crate::il::control_flow::control_flow_graph::{CfgNode, ControlFlowGraph};
use crate::il::control_flow::switch_detection::LoopContext;
use crate::il::control_flow::yield_return_decompiler::{branch, new_block};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::block_transform::BlockTransformContext;

/// The exit point `FindExitPoint` picks.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum ExitPoint {
    /// Unknown: the loop extends heuristically (`null`).
    Unknown,
    /// The loop has no exit point and takes all dominated blocks (`NoExitPoint`).
    NoExit,
    Node(usize),
}

impl ExitPoint {
    fn node(self) -> Option<usize> {
        match self {
            ExitPoint::Node(n) => Some(n),
            _ => None,
        }
    }
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    let f = &mut r.function;
    let block = bc.block;
    let container = bc.cfg.container;
    let mut ld = LoopDetection {
        bc,
        container,
        is_switch: false,
        loop_context: None,
    };
    if let Some(&last) = f.list(block).last()
        && matches!(f.inst(last), Inst::Switch { .. })
    {
        ld.detect_switch_body(f, ctx, block, last);
    }
    // The node of the potential loop head.
    let h = ld.bc.node;
    let mut lp: Option<Vec<usize>> = None;
    for t in ld.bc.cfg.nodes[h].predecessors.clone() {
        if ld.bc.cfg.dominates(h, t) {
            let lp = lp.get_or_insert_with(|| {
                ld.bc.visited[h] = true;
                vec![h]
            });
            ld.traverse_predecessors(t, lp);
        }
    }
    let Some(mut lp) = lp else {
        return;
    };
    ld.include_nested_containers(f, &mut lp);
    let exit = ld.extend_loop(f, ctx, h, &mut lp);
    let cfg = &ld.bc.cfg;
    lp.sort_by(|&a, &b| {
        cfg.nodes[b]
            .post_order_number
            .cmp(&cfg.nodes[a].post_order_number)
    });
    // Reset so that outer loops can be found.
    for &n in &lp {
        ld.bc.visited[n] = false;
    }
    ld.construct_loop(f, &lp, exit);
}

struct LoopDetection<'b> {
    bc: &'b mut BlockTransformContext,
    /// `currentBlockContainer`: the container of the graph.
    container: NodeId,
    /// While the body of a switch is detected.
    is_switch: bool,
    loop_context: Option<LoopContext>,
}

impl LoopDetection<'_> {
    fn loops(&self) -> &LoopContext {
        self.loop_context.as_ref().expect("a switch loop context")
    }

    /// `TraversePreOrder(n => n.Predecessors, list.Add)`: marks and appends the unvisited
    /// nodes reachable backwards from `start`.
    fn traverse_predecessors(&mut self, start: usize, out: &mut Vec<usize>) {
        let mut stack = vec![start];
        while let Some(n) = stack.pop() {
            if self.bc.visited[n] {
                continue;
            }
            self.bc.visited[n] = true;
            out.push(n);
            stack.extend(self.bc.cfg.nodes[n].predecessors.iter().rev());
        }
    }

    fn children(&self, n: usize) -> &[usize] {
        self.bc.cfg.nodes[n]
            .dominator_tree_children
            .as_deref()
            .unwrap_or(&[])
    }

    /// `IncludeNestedContainers`: the blocks of loops detected earlier inside the loop's blocks.
    fn include_nested_containers(&mut self, f: &Function, lp: &mut Vec<usize>) {
        let mut i = 0;
        while i < lp.len() {
            let block = self.bc.cfg.nodes[lp[i]].block;
            self.include_block(f, block, lp);
            i += 1;
        }
    }

    fn include_block(&mut self, f: &Function, block: NodeId, lp: &mut Vec<usize>) {
        for inst in f.list(block).to_vec() {
            if !matches!(f.inst(inst), Inst::BlockContainer { .. }) {
                continue;
            }
            let blocks = f.list(inst).to_vec();
            self.include_block(f, blocks[0], lp);
            for &b in &blocks[1..] {
                let node = self.bc.cfg.node_of(b);
                if !self.bc.visited[node] {
                    self.bc.visited[node] = true;
                    lp.push(node);
                }
            }
        }
    }

    /// `ExtendLoop`: adds the blocks up to the exit point, or extends heuristically.
    fn extend_loop(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        head: usize,
        lp: &mut Vec<usize>,
    ) -> ExitPoint {
        let exit = self.find_exit_point(f, ctx, head, lp);
        if exit != ExitPoint::Unknown {
            let mut stack = vec![head];
            while let Some(n) = stack.pop() {
                if !self.bc.visited[n] {
                    self.bc.visited[n] = true;
                    lp.push(n);
                }
                let children: Vec<usize> = self
                    .children(n)
                    .iter()
                    .copied()
                    .filter(|&c| {
                        Some(c) != exit.node()
                            && (!self.is_switch || !self.loops().match_continue(c))
                    })
                    .collect();
                stack.extend(children.into_iter().rev());
            }
            if self.is_switch {
                let mut i = 1;
                while i < lp.len() {
                    for p in self.bc.cfg.nodes[lp[i]].predecessors.clone() {
                        if !self.bc.visited[p] {
                            self.bc.visited[p] = true;
                            lp.push(p);
                        }
                    }
                    i += 1;
                }
            }
        } else {
            self.extend_loop_heuristic(lp, head);
        }
        exit
    }

    /// `HasReachableExit`; for a switch, continue blocks are no exits.
    fn has_reachable_exit(&self, node: usize) -> bool {
        if self.is_switch {
            !self.loops().break_targets(&self.bc.cfg, node).is_empty()
        } else {
            self.bc.cfg.has_reachable_exit(node)
        }
    }

    /// `FindExitPoint`: a single exit for the loop, without writing the visited flags.
    fn find_exit_point(
        &self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        head: usize,
        natural_loop: &[usize],
    ) -> ExitPoint {
        let cfg = &self.bc.cfg;
        if !self.has_reachable_exit(head) {
            // No node outside the loop is reachable from it: a loop without exit is possible.
            let head_block = cfg.nodes[head].block;
            if let Some(exit_branch) = is_possible_foreach_loop(f, ctx, head_block) {
                if let Some(exit_branch) = exit_branch {
                    let target = f.branch_target(exit_branch);
                    if let Some(&node) = cfg.nodes[head]
                        .successors
                        .iter()
                        .find(|&&n| cfg.nodes[n].block == target)
                        && cfg.dominates(head, node)
                        && !cfg.has_reachable_exit(node)
                    {
                        return ExitPoint::Node(node);
                    }
                }
                return ExitPoint::NoExit;
            }
            let mut exit = ExitPoint::Unknown;
            let mut offset = -1;
            consider_return_as_exit_point(f, head_block, &mut exit, &mut offset);
            for &node in self.children(head) {
                self.pick_exit_point(f, node, &mut exit, &mut offset);
            }
            return exit;
        }
        // All paths from the loop head to the reachable exits must run through the exit point.
        let (rev, exit_node_arity) = self.prepare_reverse_cfg(head);
        let exit_index = cfg.nodes.len();
        let mut common = head;
        for &node in natural_loop {
            if rev.nodes[node].is_reachable() {
                common = rev.find_common_dominator(common, node);
            }
        }
        // `common` post-dominates the loop; walk up the post-dominator tree to a valid exit.
        while common != exit_index {
            if !self.bc.visited[common] && self.validate_exit_point(head, common) {
                return ExitPoint::Node(common);
            }
            common = rev.nodes[common]
                .immediate_dominator
                .expect("post-dominator chain");
        }
        // The artificial exit node: several exits, or one the post-dominance did not consider.
        if exit_node_arity > 1 {
            return ExitPoint::Unknown;
        }
        // For a switch, the single break target gets the leave instructions.
        if exit_node_arity == 1 && self.is_switch {
            let mut targets = self.loops().break_targets(cfg, head);
            targets.sort_unstable();
            targets.dedup();
            let [target] = targets[..] else {
                panic!("a switch with one exit node has one break target");
            };
            return ExitPoint::Node(target);
        }
        ExitPoint::NoExit
    }

    /// `ValidateExitPoint`: no node reachable from the exit point is dominated by the loop
    /// head but not by the exit point.
    fn validate_exit_point(&self, head: usize, exit: usize) -> bool {
        let cfg = &self.bc.cfg;
        let mut stack = vec![exit];
        while let Some(node) = stack.pop() {
            // An empty dominance frontier needs no check.
            if !cfg.has_reachable_exit(node) {
                continue;
            }
            for &succ in &cfg.nodes[node].successors {
                if head != succ && cfg.dominates(head, succ) && !cfg.dominates(exit, succ) {
                    return false;
                }
            }
            stack.extend(self.children(node).iter().rev());
        }
        true
    }

    /// `PickExitPoint`: the node with the highest IL offset that has no reachable exit (the
    /// real exit, for compilers that emit IL in source order).
    fn pick_exit_point(&self, f: &Function, node: usize, exit: &mut ExitPoint, offset: &mut i32) {
        let mut stack = vec![node];
        while let Some(node) = stack.pop() {
            if self.is_switch && self.loops().match_continue(node) {
                continue;
            }
            let block = self.bc.cfg.nodes[node].block;
            let start = f.il_range(block).start;
            if start > *offset
                && !self.has_reachable_exit(node)
                && in_container(f, block, self.container)
            {
                // A block already moved into a nested loop cannot be exited to. The children
                // are not visited: their later offsets would pull most code into the loop.
                *exit = ExitPoint::Node(node);
                *offset = start;
                continue;
            }
            consider_return_as_exit_point(f, block, exit, offset);
            stack.extend(self.children(node).iter().rev());
        }
    }

    /// `PrepareReverseCFG`: the reversed edges among the nodes the loop head dominates, with an
    /// artificial exit node (the last) for edges that leave them or the container; and the
    /// number of nodes treated as exits (0, 1, or 2 for more).
    fn prepare_reverse_cfg(&self, head: usize) -> (ControlFlowGraph, u8) {
        let cfg = &self.bc.cfg;
        let n = cfg.nodes.len();
        let mut nodes: Vec<CfgNode> = (0..=n)
            .map(|i| CfgNode {
                user_index: i,
                block: cfg.nodes.get(i).map_or(NodeId(0), |c| c.block),
                post_order_number: 0,
                immediate_dominator: None,
                dominator_tree_children: None,
                predecessors: Vec::new(),
                successors: Vec::new(),
            })
            .collect();
        let add_edge = |nodes: &mut Vec<CfgNode>, from: usize, to: usize| {
            nodes[from].successors.push(to);
            nodes[to].predecessors.push(from);
        };
        let mut treated_as_exit: Option<usize> = None;
        let mut multiple = false;
        for i in 0..n {
            if !cfg.dominates(head, i)
                || (self.is_switch && i != head && self.loops().match_continue(i))
            {
                continue;
            }
            for &succ in &cfg.nodes[i].successors {
                // Edges to outer loops still count as exits (no labelled continue).
                if self.is_switch && self.loops().match_continue_depth(succ, 1) {
                    continue;
                }
                if cfg.dominates(head, succ) {
                    add_edge(&mut nodes, succ, i);
                } else {
                    let first = *treated_as_exit.get_or_insert(succ);
                    if first != succ {
                        multiple = true;
                    }
                    add_edge(&mut nodes, n, i);
                }
            }
            if cfg.has_direct_exit_out_of_container(i) {
                add_edge(&mut nodes, n, i);
            }
        }
        let arity = if multiple {
            2
        } else {
            u8::from(treated_as_exit.is_some())
        };
        (ControlFlowGraph::from_nodes(nodes, n), arity)
    }

    /// `ExtendLoopHeuristic`: in pre-order of the dominator tree, adds each candidate whose
    /// addition reduces the number of exit points.
    fn extend_loop_heuristic(&mut self, lp: &mut Vec<usize>, start: usize) {
        let mut stack = vec![start];
        while let Some(candidate) = stack.pop() {
            if !self.bc.visited[candidate] {
                // The nodes the candidate adds, marked as visited: the extended loop.
                let mut additional = Vec::new();
                self.traverse_predecessors(candidate, &mut additional);
                let mut new_exit_points: Vec<usize> = Vec::new();
                for &n in &additional {
                    for &s in &self.bc.cfg.nodes[n].successors {
                        if !self.bc.visited[s] && !new_exit_points.contains(&s) {
                            new_exit_points.push(s);
                        }
                    }
                }
                // Back to the unextended loop, to count the exit points it has.
                for &n in &additional {
                    self.bc.visited[n] = false;
                }
                let removed = additional
                    .iter()
                    .filter(|&&n| self.is_exit_point(n))
                    .count();
                let added = new_exit_points
                    .iter()
                    .filter(|&&n| !self.is_exit_point(n))
                    .count();
                if removed > added {
                    self.traverse_predecessors(candidate, lp);
                }
            }
            stack.extend(self.children(candidate).iter().rev());
        }
    }

    /// `IsExitPoint`: outside the loop, with a predecessor in it.
    fn is_exit_point(&self, node: usize) -> bool {
        !self.bc.visited[node]
            && self.bc.cfg.nodes[node]
                .predecessors
                .iter()
                .any(|&p| self.bc.visited[p])
    }

    /// `ConstructLoop`: the loop's blocks move into a new loop container in the head block.
    fn construct_loop(&self, f: &mut Function, lp: &[usize], exit: ExitPoint) {
        let old_entry = self.bc.cfg.nodes[lp[0]].block;
        let exit_target = exit.node().map(|n| self.bc.cfg.nodes[n].block);
        let loop_container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Loop,
            expected: None,
            blocks: Vec::new(),
        });
        let new_entry = new_block(f);
        f.push(loop_container, new_entry);
        // The head's instructions move (not the block, which branches outside the loop target).
        let instructions = f.list(old_entry).to_vec();
        f.set_list(new_entry, instructions);
        f.add_il_range(new_entry, f.il_range(old_entry));
        f.set_list(old_entry, vec![loop_container]);
        if let Some(target) = exit_target {
            let br = branch(f, target);
            f.push(old_entry, br);
        }
        f.add_il_range(loop_container, f.il_range(new_entry));
        self.move_blocks_into_container(f, lp, loop_container);
        // Branches to the old head go to the new entry; branches to the exit leave the loop.
        for node in f.descendants_post(loop_container) {
            if !matches!(f.inst(node), Inst::Branch { .. }) {
                continue;
            }
            let target = f.branch_target(node);
            if target == old_entry {
                f.set_branch_target(node, new_entry);
            } else if Some(target) == exit_target {
                let l = leave(f, loop_container);
                f.add_il_range(l, f.il_range(node));
                f.replace(node, l);
            }
        }
    }

    /// `MoveBlocksIntoContainer`: blocks a nested loop already holds move with it.
    fn move_blocks_into_container(&self, f: &mut Function, lp: &[usize], container: NodeId) {
        for &node in &lp[1..] {
            let block = self.bc.cfg.nodes[node].block;
            if in_container(f, block, self.container) {
                let old_child_index = f.node(block).child_index as usize;
                f.push(container, block);
                f.swap_remove_block_at(self.container, old_child_index);
            }
        }
    }

    /// `DetectSwitchBody`: the blocks of a switch move into a switch container.
    fn detect_switch_body(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        switch_inst: NodeId,
    ) {
        let h = self.bc.node;
        self.is_switch = true;
        self.loop_context = Some(LoopContext::new(f, &self.bc.cfg, h));
        let mut nodes = vec![h];
        self.bc.visited[h] = true;
        let mut exit = self.extend_loop(f, ctx, h, &mut nodes);
        if let ExitPoint::Node(ep) = exit
            && self.bc.cfg.dominates(h, ep)
            && self.bc.cfg.nodes[ep].predecessors.len() == 1
            && !self.has_reachable_exit(ep)
        {
            // An exit point reached by a single break moves into the switch: nesting shows in
            // which cases the code runs.
            let mut stack = vec![ep];
            while let Some(n) = stack.pop() {
                nodes.push(n);
                stack.extend(self.children(n).iter().rev());
            }
            for &n in &nodes {
                self.bc.visited[n] = true;
            }
            exit = ExitPoint::Unknown;
        }
        // Reverse post-order makes the output read better.
        let cfg = &self.bc.cfg;
        nodes.sort_by(|&a, &b| {
            cfg.nodes[b]
                .post_order_number
                .cmp(&cfg.nodes[a].post_order_number)
        });
        for &n in &nodes {
            self.bc.visited[n] = false;
        }
        let switch_container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Switch,
            expected: None,
            blocks: Vec::new(),
        });
        let new_entry = new_block(f);
        f.add_il_range(new_entry, f.il_range(switch_inst));
        f.push(switch_container, new_entry);
        f.push(new_entry, switch_inst);
        let mut instructions = f.list(block).to_vec();
        *instructions.last_mut().expect("the switch") = switch_container;
        f.set_list(block, instructions);
        let exit_target = exit.node().map(|n| self.bc.cfg.nodes[n].block);
        if let Some(target) = exit_target {
            let br = branch(f, target);
            f.push(block, br);
        }
        f.add_il_range(switch_container, f.il_range(new_entry));
        self.move_blocks_into_container(f, &nodes, switch_container);
        if let Some(target) = exit_target {
            for node in f.descendants_post(switch_container) {
                if matches!(f.inst(node), Inst::Branch { .. }) && f.branch_target(node) == target {
                    let l = leave(f, switch_container);
                    f.add_il_range(l, f.il_range(node));
                    f.replace(node, l);
                }
            }
        }
        self.is_switch = false;
    }
}

/// Whether `block` is (still) one of the blocks of `container`.
fn in_container(f: &Function, block: NodeId, container: NodeId) -> bool {
    f.node(block).parent == Some(container)
        && f.list(container).get(f.node(block).child_index as usize) == Some(&block)
}

fn leave(f: &mut Function, container: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Leave {
        target: Some(container),
        value: nop,
    })
}

/// `ConsiderReturnAsExitPoint`: a `return;` that ControlFlowSimplification combined with the
/// condition block; with `NoExit`, it joins the body and DetectExitPoints moves it out again.
fn consider_return_as_exit_point(
    f: &Function,
    block: NodeId,
    exit: &mut ExitPoint,
    offset: &mut i32,
) {
    let Some((_, true_inst, false_inst)) = f.match_if_at_end_of_block(block) else {
        return;
    };
    for inst in [true_inst, false_inst] {
        let start = f.il_range(inst).start;
        if start > *offset
            && let Inst::Leave {
                target: Some(c),
                value,
            } = *f.inst(inst)
            && Some(c) == f.body
            && matches!(f.inst(value), Inst::Nop { .. })
        {
            *exit = ExitPoint::NoExit;
            *offset = start;
        }
    }
}

/// `IsPossibleForeachLoop`: the head is the `MoveNext` check at the start of a try-finally's
/// try block; the exit branch (the false branch, when a branch).
fn is_possible_foreach_loop(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    head: NodeId,
) -> Option<Option<NodeId>> {
    let container = f.node(head).parent?;
    let try_finally = f.node(container).parent?;
    if !matches!(*f.inst(try_finally), Inst::TryFinally { try_block, .. } if try_block == container)
    {
        return None;
    }
    let [first, mut false_inst] = *f.list(head) else {
        return None;
    };
    let (mut condition, mut true_inst) = f.match_if_instruction(first)?;
    while let Some(arg) = f.match_logic_not(condition) {
        condition = arg;
        std::mem::swap(&mut true_inst, &mut false_inst);
    }
    let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = f.inst(condition) else {
        return None;
    };
    if c.method.method.name != "MoveNext" {
        return None;
    }
    let [arg] = c.args[..] else {
        return None;
    };
    f.match_ld_loc_ref(ctx.ts, arg)?;
    let exit_branch = matches!(f.inst(false_inst), Inst::Branch { .. }).then_some(false_inst);
    // The head is the try block's entry point, past blocks that only branch on.
    let mut entry = f.list(container)[0];
    while f.node(entry).incoming_edge_count == 1
        && let [only] = *f.list(entry)
        && let Some(target) = f.match_branch(only)
    {
        entry = target;
    }
    (entry == head).then_some(exit_branch)
}
