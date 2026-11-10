//! `IL/Transforms/ReduceNestingTransform.cs`: when an `if` or a switch is followed by a keyword
//! exit (`return`, `break`, `continue`), the exit is duplicated into the branches so the else
//! block or default case can follow the statement instead of nesting in it; if statements are
//! also inverted back to IL order, and empty `try-finally` around a pinned region is removed.
//! The IL order of the code does not change.

use crate::il::ast::{BlockKind, ContainerKind, Flags, Function, Inst, NodeId, NopKind};
use crate::il::control_flow::condition_detection::invert_if;
use crate::il::control_flow::control_flow_graph::ControlFlowGraph;
use crate::il::reader::ReadFunction;

use crate::il::types::StackType;
use crate::typesystem::types::TypeSystem;

use super::Context;
use super::null_coalescing_transform::unwrap_block;

const TRUE_INST: usize = 1;
const FALSE_INST: usize = 2;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let Some(body) = f.body else {
        return;
    };
    visit_container(f, ctx, body, None);
    // `function.Descendants`: the body, then the local functions, in post-order.
    let roots: Vec<NodeId> = std::iter::once(body)
        .chain(f.local_functions.iter().copied())
        .collect();
    let try_finallies: Vec<NodeId> = roots
        .into_iter()
        .flat_map(|root| f.descendants_post(root))
        .filter(|&n| matches!(f.inst(n), Inst::TryFinally { .. }))
        .collect();
    for tf in try_finallies {
        eliminate_redundant_try_finally(f, tf);
    }
}

fn container_kind(f: &Function, container: NodeId) -> ContainerKind {
    match *f.inst(container) {
        Inst::BlockContainer { kind, .. } => kind,
        _ => ContainerKind::Normal,
    }
}

fn slot(f: &Function, inst: NodeId, index: usize) -> NodeId {
    f.inst(inst).children()[index]
}

fn is_block(f: &Function, inst: NodeId) -> bool {
    matches!(f.inst(inst), Inst::Block { .. })
}

/// `Leave.IsLeavingFunction`: the target is a function's body.
fn is_leaving_function(f: &Function, container: NodeId) -> bool {
    Some(container) == f.body
        || f.node(container)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::Function { .. }))
}

/// `MatchLeave(container)`: a `leave` of `container` without value.
fn is_leave_of(f: &Function, inst: NodeId, container: NodeId) -> bool {
    f.match_leave(inst)
        .is_some_and(|(c, value)| c == container && f.match_nop(value))
}

/// `Visit(BlockContainer, continueTarget)`.
fn visit_container(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
    continue_target: Option<NodeId>,
) {
    let continue_target = match container_kind(f, container) {
        ContainerKind::Loop | ContainerKind::While => Some(f.list(container)[0]),
        ContainerKind::DoWhile | ContainerKind::For => f.list(container).last().copied(),
        _ => continue_target,
    };
    // Visiting may append blocks to the container, but changes nothing else in the list.
    let mut i = 0;
    while i < f.list(container).len() {
        let block = f.list(container)[i];
        visit_block(f, ctx, block, continue_target, None);
        i += 1;
    }
}

/// `Visit(Block, continueTarget, nextInstruction)`: `next` is what runs after the block's end
/// point (only absent when the end point is unreachable).
fn visit_block(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    continue_target: Option<NodeId>,
    next: Option<NodeId>,
) {
    let mut i = 0;
    while i < f.list(block).len() {
        let inst = f.list(block)[i];
        // Transformations change the following instructions, so this is looked up each time.
        let next_insn = |f: &Function| f.list(block).get(i + 1).copied().or(next);
        match f.inst(inst) {
            Inst::BlockContainer { .. } => {
                visit_container(f, ctx, inst, continue_target);
                if container_kind(f, inst) == ContainerKind::Switch
                    && let Some(exit) = can_duplicate_exit(f, next_insn(f), continue_target)
                    && reduce_switch_nesting(f, ctx.ts, block, inst, exit)
                {
                    remove_redundant_exit(f, block, next);
                }
            }
            Inst::If { .. } => {
                improve_il_ordering(f, ctx, block, inst, continue_target);
                if let Some(exit) = can_duplicate_exit(f, next_insn(f), continue_target)
                    && reduce_nesting(f, ctx, block, inst, exit)
                {
                    remove_redundant_exit(f, block, next);
                }
                let true_inst = slot(f, inst, TRUE_INST);
                if is_block(f, true_inst) {
                    let n = next_insn(f);
                    visit_block(f, ctx, true_inst, continue_target, n);
                }
                let false_inst = slot(f, inst, FALSE_INST);
                if is_block(f, false_inst) {
                    if f.has_flag(slot(f, inst, TRUE_INST), Flags::END_POINT_UNREACHABLE) {
                        extract_else_block(f, inst);
                    } else {
                        let n = next_insn(f);
                        visit_block(f, ctx, false_inst, continue_target, n);
                    }
                }
            }
            _ => {
                // Blocks leave containers only by leave instructions, so only `continue_target`
                // matters here.
                visit_containers(f, ctx, inst, continue_target);
                // Reducing nesting inside try, using, lock and so on may make the end point
                // unreachable, which leaves the block's exit after it dead.
                let len = f.list(block).len();
                if i + 2 == len && f.has_flag(inst, Flags::END_POINT_UNREACHABLE) {
                    f.remove_at(block, len - 1);
                }
            }
        }
        i += 1;
    }
}

/// `VisitContainers`: the containers inside an instruction; nested functions are transformed
/// already.
fn visit_containers(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    continue_target: Option<NodeId>,
) {
    match f.inst(inst) {
        Inst::Function { .. } => {}
        Inst::BlockContainer { .. } => visit_container(f, ctx, inst, continue_target),
        _ => {
            for child in f.inst(inst).children() {
                visit_containers(f, ctx, child, continue_target);
            }
        }
    }
}

/// `GetStartILOffset(inst, out isEmpty)`: of the value of a leave with one.
fn start_il_offset(f: &Function, inst: NodeId) -> (i32, bool) {
    let inst = match *f.inst(inst) {
        Inst::Leave { value, .. } if !f.match_nop(value) => value,
        _ => inst,
    };
    let range = f.il_range(inst);
    (range.start, range.is_empty())
}

/// `ImproveILOrdering`: an if statement with an unreachable end point and no else is inverted
/// to put the first statements of both branches in IL order.
fn improve_il_ordering(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    if_inst: NodeId,
    continue_target: Option<NodeId>,
) {
    let true_inst = slot(f, if_inst, TRUE_INST);
    if !f.has_flag(block, Flags::END_POINT_UNREACHABLE)
        || !f.has_flag(true_inst, Flags::END_POINT_UNREACHABLE)
        || !f.match_nop(slot(f, if_inst, FALSE_INST))
    {
        return;
    }
    let index = f.node(if_inst).child_index as usize;
    let (true_start, true_empty) = start_il_offset(f, true_inst);
    let (false_start, false_empty) = start_il_offset(f, f.list(block)[index + 1]);
    if true_empty || false_empty || false_start >= true_start {
        return;
    }
    let &last = f.list(block).last().expect("a block exit");
    if let Some((target, _)) = f.match_leave(last)
        && !is_leaving_function(f, target)
        && container_kind(f, target) == ContainerKind::Normal
    {
        // A non-keyword leave must stay last (fall-through) unless a keyword replaces it.
        let Some(keyword) = can_duplicate_exit(f, Some(last), continue_target) else {
            return;
        };
        let copy = f.clone_subtree(keyword);
        f.replace(last, copy);
    }
    invert_if(f, ctx, block, if_inst);
}

/// `ReduceNesting`: the exit after an if is duplicated into its branches.
fn reduce_nesting(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    if_inst: NodeId,
    exit: NodeId,
) -> bool {
    let (mut max_statements, mut max_depth) = (0, 0);
    update_stats(
        f,
        ctx.ts,
        slot(f, if_inst, TRUE_INST),
        &mut max_statements,
        &mut max_depth,
    );
    if f.match_nop(slot(f, if_inst, FALSE_INST)) {
        // if (cond) { ... } exit;  ->  if (!cond) exit; ...; exit;
        if max_depth < 2 {
            return false;
        }
        ensure_end_point_unreachable(f, block, exit);
        // The block's own exit (such as a leave from a try) stays as it is.
        let &last = f.list(block).last().expect("a block exit");
        ensure_end_point_unreachable(f, slot(f, if_inst, TRUE_INST), last);
        invert_if(f, ctx, block, if_inst);
        let true_inst = slot(f, if_inst, TRUE_INST);
        if !f.structurally_equal(true_inst, exit) {
            let copy = f.clone_subtree(exit);
            f.replace(true_inst, copy);
        }
        return true;
    }
    // An else-if chain is one group from its root if.
    if else_if_parent(f, if_inst).is_some() {
        return false;
    }
    let mut if_inst = if_inst;
    loop {
        let unwrapped = unwrap_block(f, slot(f, if_inst, FALSE_INST));
        if !matches!(f.inst(unwrapped), Inst::If { .. }) {
            break;
        }
        update_stats(
            f,
            ctx.ts,
            slot(f, unwrapped, TRUE_INST),
            &mut max_statements,
            &mut max_depth,
        );
        if_inst = unwrapped;
    }
    let false_block = slot(f, if_inst, FALSE_INST);
    if !is_block(f, false_block)
        || !should_reduce_nesting(f, ctx.ts, false_block, max_statements, max_depth)
    {
        return false;
    }
    // The else block moves out, with exits all the way up the else-if chain.
    let mut current = Some(if_inst);
    while let Some(if_inst) = current {
        let parent = else_if_parent(f, if_inst);
        ensure_end_point_unreachable(f, slot(f, if_inst, TRUE_INST), exit);
        if f.has_flag(slot(f, if_inst, FALSE_INST), Flags::END_POINT_UNREACHABLE) {
            // The instructions that ended up dead (just the exit).
            let remove_after = f.node(if_inst).child_index as usize + 1;
            let len = f.list(block).len();
            for _ in remove_after..len {
                f.remove_at(block, remove_after);
            }
        }
        extract_else_block(f, if_inst);
        current = parent;
    }
    true
}

/// `ReduceSwitchNesting`: the breaks of a switch become the exit after it, and the default
/// case moves out.
fn reduce_switch_nesting(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    parent_block: NodeId,
    switch_container: NodeId,
    exit: NodeId,
) -> bool {
    // A break from an outer container would change its meaning inside the switch.
    if let Some((target, _)) = f.match_leave(exit)
        && !is_leaving_function(f, target)
    {
        return false;
    }
    let entry = f.list(switch_container)[0];
    let &[switch_inst] = f.list(entry) else {
        panic!("Single: the switch block holds one instruction");
    };
    let Inst::Switch { ref sections, .. } = *f.inst(switch_inst) else {
        return false;
    };
    let sections = sections.clone();
    // `MaxBy(s => s.Labels.Count())`: the first section with the most labels.
    let mut default_section = sections[0];
    for &s in &sections[1..] {
        if section_label_count(f, s) > section_label_count(f, default_section) {
            default_section = s;
        }
    }
    let Some(default_block) = f.match_branch(section_body(f, default_section)) else {
        return false;
    };
    if f.node(default_block).incoming_edge_count != 1
        || f.node(default_block).parent != Some(switch_container)
    {
        return false;
    }
    let (mut max_statements, mut max_depth) = (0, 0);
    for &s in &sections {
        if s != default_section
            && let Some(case_block) = f.match_branch(section_body(f, s))
            && f.node(case_block).parent == Some(switch_container)
        {
            update_stats(f, ts, case_block, &mut max_statements, &mut max_depth);
        }
    }
    if !should_reduce_nesting(f, ts, default_block, max_statements, max_depth) {
        return false;
    }
    // The default case's dominator tree has no exits to other cases.
    let cfg = ControlFlowGraph::new(f, switch_container);
    let default_node = cfg.node_of(default_block);
    let mut tree = Vec::new();
    let mut stack = vec![default_node];
    while let Some(n) = stack.pop() {
        tree.push(n);
        let children = cfg.nodes[n]
            .dominator_tree_children
            .clone()
            .unwrap_or_default();
        stack.extend(children.into_iter().rev());
    }
    if tree
        .iter()
        .flat_map(|&n| cfg.nodes[n].successors.iter())
        .any(|&s| !cfg.dominates(default_node, s))
    {
        return false;
    }
    let parent_container = f.node(parent_block).parent;
    let parent_is_container =
        parent_container.is_some_and(|p| matches!(f.inst(p), Inst::BlockContainer { .. }));
    if tree.len() > 1 && !parent_is_container {
        return false;
    }
    // A leave after the switch (from a try, pinned region and so on) lets the extracted default
    // block fall through, rather than duplicating what follows the container.
    let list = f.list(parent_block);
    if list.len() >= 2 && list[list.len() - 2] == switch_container {
        let &default_last = f.list(default_block).last().expect("a block exit");
        let &parent_last = f.list(parent_block).last().expect("a block exit");
        if is_leave_of(f, default_last, switch_container) {
            f.replace(default_last, parent_last);
        }
        let len = f.list(parent_block).len();
        f.remove_at(parent_block, len - 1);
    }
    // Every break becomes the exit.
    let breaks: Vec<NodeId> = f
        .descendants_post(switch_container)
        .into_iter()
        .filter(|&n| is_leave_of(f, n, switch_container))
        .collect();
    for b in breaks {
        let copy = f.clone_subtree(exit);
        f.replace(b, copy);
    }
    // The default section branch becomes a break.
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let leave = f.add(Inst::Leave {
        target: Some(switch_container),
        value: nop,
    });
    let body = section_body(f, default_section);
    f.replace(body, leave);
    // The default blocks leave the switch container.
    let default_blocks: Vec<NodeId> = tree.iter().map(|&n| cfg.nodes[n].block).collect();
    let remaining: Vec<NodeId> = f
        .list(switch_container)
        .iter()
        .copied()
        .filter(|b| !default_blocks.contains(b))
        .collect();
    f.set_list(switch_container, remaining);
    for inst in f.list(default_block).to_vec() {
        f.push(parent_block, inst);
    }
    // Further blocks of the default case go to the parent container.
    if default_blocks.len() > 1 {
        let parent_container = parent_container.expect("checked above");
        let start = f
            .list(parent_container)
            .iter()
            .position(|&b| b == parent_block)
            .expect("the parent block is in its container")
            + 1;
        for (i, &b) in default_blocks[1..].iter().enumerate() {
            f.insert(parent_container, start + i, b);
        }
    }
    true
}

fn section_body(f: &Function, section: NodeId) -> NodeId {
    match *f.inst(section) {
        Inst::SwitchSection { body, .. } => body,
        _ => unreachable!("a switch section"),
    }
}

fn section_label_count(f: &Function, section: NodeId) -> u64 {
    match f.inst(section) {
        Inst::SwitchSection { labels, .. } => labels.count(),
        _ => 0,
    }
}

/// `CanDuplicateExit`: the keyword exit (`return`, `break`, `continue`) `exit` is or leads to.
fn can_duplicate_exit(
    f: &Function,
    exit: Option<NodeId>,
    continue_target: Option<NodeId>,
) -> Option<NodeId> {
    let exit = exit?;
    if continue_target.is_some() && f.match_branch(exit) == continue_target {
        // continue
        return Some(exit);
    }
    // No valued returns.
    let (target, value) = f.match_leave(exit)?;
    if !f.match_nop(value) {
        return None;
    }
    if is_leaving_function(f, target) || container_kind(f, target) != ContainerKind::Normal {
        // return or break
        return Some(exit);
    }
    // A leave from a try, pinned region, lock and so on: the instruction after the target
    // container decides.
    let mut leaving = target;
    loop {
        let parent = f.node(leaving).parent?;
        if let Inst::Block { instructions, .. } = f.inst(parent)
            && instructions.last() != Some(&leaving)
        {
            break;
        }
        match *f.inst(parent) {
            Inst::TryFinally { finally_block, .. } => {
                // No leaves from finally containers, nor past a finally that does not return.
                if finally_block == leaving || f.has_flag(parent, Flags::END_POINT_UNREACHABLE) {
                    return None;
                }
            }
            Inst::TryFault { fault_block, .. } if fault_block == leaving => return None,
            _ => {}
        }
        leaving = parent;
    }
    let block = f.node(leaving).parent?;
    let index = f.node(leaving).child_index as usize;
    let target_inst = f.list(block)[index + 1];
    can_duplicate_exit(f, Some(target_inst), continue_target)
}

/// `EnsureEndPointUnreachable`: a copy of the following exit closes a block that can fall
/// through.
fn ensure_end_point_unreachable(f: &mut Function, inst: NodeId, fallthrough_exit: NodeId) {
    if !is_block(f, inst) {
        return;
    }
    if !f.has_flag(inst, Flags::END_POINT_UNREACHABLE) {
        let copy = f.clone_subtree(fallthrough_exit);
        f.push(inst, copy);
    }
}

/// `RemoveRedundantExit`.
fn remove_redundant_exit(f: &mut Function, block: NodeId, implicit_exit: Option<NodeId>) {
    let Some(implicit_exit) = implicit_exit else {
        return;
    };
    let &last = f.list(block).last().expect("a block exit");
    if f.structurally_equal(last, implicit_exit) {
        let len = f.list(block).len();
        f.remove_at(block, len - 1);
    }
}

/// `GetElseIfParent`: the if whose else block holds only `if_inst`.
fn else_if_parent(f: &Function, if_inst: NodeId) -> Option<NodeId> {
    let parent = f.node(if_inst).parent?;
    if unwrap_block(f, parent) != if_inst {
        return None;
    }
    let grand = f.node(parent).parent?;
    match *f.inst(grand) {
        Inst::If { false_inst, .. } if false_inst == parent => Some(grand),
        _ => None,
    }
}

/// `UpdateStats`.
fn update_stats(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
    max_statements: &mut usize,
    max_depth: &mut usize,
) {
    let mut statements = 0;
    compute_stats(f, ts, inst, &mut statements, max_depth, 0, true);
    *max_statements = (*max_statements).max(statements);
}

/// `ComputeStats`: the number of statements and the deepest nesting below an instruction.
fn compute_stats(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
    statements: &mut usize,
    max_depth: &mut usize,
    depth: usize,
    is_statement: bool,
) {
    if is_statement {
        *statements += 1;
    }
    if depth > *max_depth {
        *max_depth = depth;
    }
    match f.inst(inst) {
        Inst::Block {
            kind,
            instructions,
            final_inst,
        } => {
            if is_statement {
                // Blocks are not statements themselves.
                *statements -= 1;
            }
            let child_statements = !matches!(
                kind,
                BlockKind::CallWithNamedArgs | BlockKind::CallInlineAssign
            );
            for &child in instructions {
                compute_stats(f, ts, child, statements, max_depth, depth, child_statements);
            }
            compute_stats(f, ts, *final_inst, statements, max_depth, depth, false);
        }
        Inst::BlockContainer { kind, blocks, .. } => {
            if !is_statement {
                // A container in an expression is always a statement.
                *statements += 1;
            }
            let kind = *kind;
            let mut body = blocks[0];
            if matches!(kind, ContainerKind::For | ContainerKind::While) {
                body = match_condition_block(f, inst, blocks[0])
                    .expect("Invalid condition block in loop.");
            }
            // The implicit leave does not count.
            let &last = f.list(body).last().expect("a block exit");
            let implicit = match kind {
                ContainerKind::For | ContainerKind::DoWhile => {
                    f.match_branch(last) == blocks.last().copied()
                }
                ContainerKind::Loop | ContainerKind::While => {
                    f.match_branch(last) == Some(blocks[0])
                }
                ContainerKind::Normal => is_leave_of(f, last, inst),
                // The switch instruction counts as a statement anyway.
                ContainerKind::Switch => true,
            };
            if implicit {
                *statements -= 1;
            }
            compute_stats(f, ts, body, statements, max_depth, depth + 1, true);
        }
        Inst::If {
            true_inst,
            false_inst,
            csharp_type,
            ..
        } if csharp_type.is_none_or(|t| ts.stack_type(t) == StackType::Void) => {
            compute_stats(f, ts, *true_inst, statements, max_depth, depth + 1, true);
            // Else-ifs count at the same depth.
            let mut else_inst = *false_inst;
            loop {
                let unwrapped = unwrap_block(f, else_inst);
                let Inst::If {
                    true_inst,
                    false_inst,
                    ..
                } = *f.inst(unwrapped)
                else {
                    break;
                };
                *statements += 1;
                compute_stats(f, ts, true_inst, statements, max_depth, depth + 1, true);
                else_inst = false_inst;
            }
            compute_stats(f, ts, else_inst, statements, max_depth, depth + 1, true);
        }
        Inst::SwitchSection { body, .. } => {
            // A statement for each case label; the case block counts at the current depth.
            *statements += 1;
            let switch = f.node(inst).parent;
            let container = switch
                .and_then(|s| f.node(s).parent)
                .and_then(|b| f.node(b).parent);
            if let Some(case_block) = f.match_branch(*body)
                && f.node(case_block).parent == container
            {
                compute_stats(f, ts, case_block, statements, max_depth, depth, true);
            }
        }
        Inst::Function { body, .. } => {
            let mut body_statements = 0;
            let mut body_max_depth = *max_depth;
            compute_stats(
                f,
                ts,
                *body,
                &mut body_statements,
                &mut body_max_depth,
                depth,
                true,
            );
            // Inline functions do not count.
            if body_statements >= 2 {
                *statements += body_statements;
                *max_depth = body_max_depth;
            }
        }
        _ => {
            // Containers in the children hold statements.
            let mut sub = 0;
            for child in f.inst(inst).children() {
                compute_stats(f, ts, child, &mut sub, max_depth, depth, false);
            }
            *statements += sub;
            if is_statement && sub > 0 {
                // Only the contents of the first container count; this statement already did.
                *statements -= 1;
            }
        }
    }
}

/// `BlockContainer.MatchConditionBlock`: `if (condition) br body else leave container`.
fn match_condition_block(f: &Function, container: NodeId, block: NodeId) -> Option<NodeId> {
    let &[inst] = f.list(block) else {
        return None;
    };
    let Inst::If {
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    if !is_leave_of(f, false_inst, container) {
        return None;
    }
    f.match_branch(true_inst)
}

/// `ShouldReduceNesting`: whether duplicating exits into the sibling branches is worth one
/// less level of nesting for `block`. (The managed method takes the siblings' depth too but
/// does not read it.)
fn should_reduce_nesting(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    max_statements: usize,
    _max_depth: usize,
) -> bool {
    let (mut statements2, mut depth2) = (0, 0);
    update_stats(f, ts, block, &mut statements2, &mut depth2);
    depth2 >= 2 || depth2 >= 1 && statements2 > max_statements || statements2 >= 2 * max_statements
}

/// `ExtractElseBlock`: `if (c) { ...; exit; } else { b }` becomes `if (c) { ...; exit; } b`.
fn extract_else_block(f: &mut Function, if_inst: NodeId) {
    let block = f.node(if_inst).parent.expect("an if in a block");
    let false_block = slot(f, if_inst, FALSE_INST);
    let start = f.node(if_inst).child_index as usize + 1;
    for (i, inst) in f.list(false_block).to_vec().into_iter().enumerate() {
        f.insert(block, start + i, inst);
    }
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.set_child_at(if_inst, FALSE_INST, nop);
}

/// `EliminateRedundantTryFinally`: an empty finally around a pinned region goes (the C#
/// compiler emits one for fixed statements).
fn eliminate_redundant_try_finally(f: &mut Function, try_finally: NodeId) {
    let Inst::TryFinally {
        try_block,
        finally_block,
    } = *f.inst(try_finally)
    else {
        return;
    };
    if !matches!(f.inst(finally_block), Inst::BlockContainer { .. }) {
        return;
    }
    // `SingleInstruction`.
    let single = match f.list(finally_block) {
        &[b] => match f.list(b) {
            &[i] => Some(i),
            _ => None,
        },
        _ => None,
    };
    if !single.is_some_and(|i| is_leave_of(f, i, finally_block)) {
        return;
    }
    if !matches!(f.inst(try_block), Inst::BlockContainer { .. }) {
        return;
    }
    let &[block] = f.list(try_block) else {
        return;
    };
    let pinned = match *f.list(block) {
        [p] => p,
        [p, leave] if is_leave_of(f, leave, try_block) => p,
        _ => return,
    };
    if matches!(f.inst(pinned), Inst::PinnedRegion { .. }) {
        f.replace(try_finally, pinned);
    }
}
