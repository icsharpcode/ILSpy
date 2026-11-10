//! `IL/ControlFlow/ConditionDetection.cs`: blocks reached only from one branch move into the
//! if (or after the block they are the unconditional exit of), and the ifs get the exits,
//! branch order and short-circuit conditions that read best.

use std::cmp::Ordering;

use crate::il::ast::{ComparisonKind, ComparisonLiftingKind, ContainerKind, Flags, Function, Inst};
use crate::il::ast::{NodeId, NopKind};
use crate::il::control_flow::exit_points::{Exit, compatible_exit_instruction};
use crate::il::control_flow::switch_detection::match_increment_block;
use crate::il::control_flow::yield_return_decompiler::new_block;
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::block_transform::BlockTransformContext;
use crate::il::types::Sign;

#[derive(Clone, Copy, PartialEq, Eq)]
enum Keyword {
    Break,
    Return,
    Continue,
    Other,
}

const CONDITION: usize = 0;
const TRUE_INST: usize = 1;
const FALSE_INST: usize = 2;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    let f = &mut r.function;
    let block = bc.block;
    let mut cd = ConditionDetection {
        current_container: f.node(block).parent.expect("block in a container"),
        bc,
    };
    match second_to_last(f, block) {
        Some(if_inst) if matches!(f.inst(if_inst), Inst::If { .. }) => {
            cd.handle_if_instruction(f, ctx, block, if_inst)
        }
        _ => {
            cd.inline_exit_branch(f, block);
        }
    }
}

struct ConditionDetection<'b> {
    bc: &'b mut BlockTransformContext,
    current_container: NodeId,
}

fn second_to_last(f: &Function, block: NodeId) -> Option<NodeId> {
    let list = f.list(block);
    (list.len() >= 2).then(|| list[list.len() - 2])
}

fn slot(f: &Function, if_inst: NodeId, index: usize) -> NodeId {
    f.inst(if_inst).children()[index]
}

fn nop_with_range_of(f: &mut Function, of: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add_il_range(nop, f.il_range(of));
    nop
}

/// `Comp.LogicNot` of the if's condition, put in its place.
fn negate_condition(f: &mut Function, ctx: &mut Context<'_, '_, '_>, if_inst: NodeId) {
    let condition = slot(f, if_inst, CONDITION);
    let input = f.result_type(ctx.ts, condition);
    let zero = f.add(Inst::LdcI4 { value: 0 });
    let not = f.add(Inst::Comp {
        kind: ComparisonKind::Equality,
        lifting: ComparisonLiftingKind::None,
        sign: Sign::None,
        input,
        left: condition,
        right: zero,
    });
    f.set_child_at(if_inst, CONDITION, not);
}

fn index_of(f: &Function, block: NodeId, inst: NodeId) -> usize {
    f.list(block)
        .iter()
        .position(|&i| i == inst)
        .expect("instruction in the block")
}

/// `TryGetExit`: the last instruction of a block (or the instruction itself), when its end
/// point is unreachable.
fn try_get_exit(f: &Function, inst: NodeId) -> Option<NodeId> {
    let inst = match f.inst(inst) {
        Inst::Block { instructions, .. } if !instructions.is_empty() => {
            *instructions.last().expect("instructions")
        }
        _ => inst,
    };
    f.has_flag(inst, Flags::END_POINT_UNREACHABLE)
        .then_some(inst)
}

/// `GetExit`.
fn get_exit(f: &Function, inst: NodeId) -> NodeId {
    match f.inst(inst) {
        Inst::Block { instructions, .. } => *instructions.last().expect("a block exit"),
        _ => inst,
    }
}

/// `IsEmpty`: a nop or an empty block.
fn is_empty(f: &Function, inst: NodeId) -> bool {
    match f.inst(inst) {
        Inst::Nop { .. } => true,
        Inst::Block {
            instructions,
            final_inst,
            ..
        } => instructions.is_empty() && f.match_nop(*final_inst),
        _ => false,
    }
}

fn compatible(f: &Function, a: NodeId, b: NodeId) -> bool {
    compatible_exit_instruction(f, a, Exit::Inst(b))
}

/// `GetStartILOffset`: of the value of a leave with one.
fn get_start_il_offset(f: &Function, inst: NodeId) -> i32 {
    if let Inst::Leave { value, .. } = *f.inst(inst)
        && !f.match_nop(value)
    {
        return f.il_range(value).start;
    }
    f.il_range(inst).start
}

/// `ExtractBlock`: moves the instructions `start..end` of `block` into `extracted`.
pub(crate) fn extract_block(
    f: &mut Function,
    block: NodeId,
    start: usize,
    end: usize,
    extracted: NodeId,
) {
    for i in start..end {
        let inst = f.list(block)[i];
        f.push(extracted, inst);
        f.add_il_range(extracted, f.il_range(inst));
    }
    for _ in start..end {
        f.remove_at(block, start);
    }
}

/// `HighLevelLoopTransform.GetIncrementBlock`: the one block besides the loop body that is
/// simple statements and a branch to the loop head.
pub(crate) fn get_increment_block(f: &Function, container: NodeId, body: NodeId) -> Option<NodeId> {
    let entry = f.list(container)[0];
    let mut found = f
        .list(container)
        .iter()
        .copied()
        .filter(|&b| b != body && match_increment_block(f, b) == Some(entry));
    let first = found.next()?;
    assert!(
        found.next().is_none(),
        "SingleOrDefault: several increment blocks"
    );
    Some(first)
}

/// `IsContinueBlock`.
fn is_continue_block(f: &Function, container: NodeId, block: NodeId) -> bool {
    if !matches!(
        f.inst(container),
        Inst::BlockContainer {
            kind: ContainerKind::Loop,
            ..
        }
    ) {
        return false;
    }
    let entry = f.list(container)[0];
    if f.node(entry).incoming_edge_count == 2
        && let Some(increment) = get_increment_block(f, container, entry)
    {
        return block == increment;
    }
    block == entry
}

fn container_kind(f: &Function, container: NodeId) -> ContainerKind {
    match *f.inst(container) {
        Inst::BlockContainer { kind, .. } => kind,
        ref other => panic!("{other:?} is not a container"),
    }
}

fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    inst == ancestor || f.ancestors(inst).any(|a| a == ancestor)
}

impl ConditionDetection<'_> {
    fn handle_if_instruction(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        if_inst: NodeId,
    ) {
        while self.inline_true_branch(f, block, if_inst) || self.inline_exit_branch(f, block) {
            self.pick_better_block_exit(f, ctx, block, if_inst);
            self.merge_common_branches(f, ctx, block, if_inst);
            swap_empty_then(f, ctx, if_inst);
            introduce_short_circuit(f, ctx, if_inst);
        }
        self.pick_better_block_exit(f, ctx, block, if_inst);
        order_if_blocks(f, ctx, if_inst);
        self.bc.index_of_first_already_transformed_instruction = f.list(block).len();
    }

    /// `InlineTrueBranch`: the target of the then-branch moves into the if.
    fn inline_true_branch(&mut self, f: &mut Function, block: NodeId, if_inst: NodeId) -> bool {
        let true_inst = slot(f, if_inst, TRUE_INST);
        if !self.can_inline(f, true_inst) {
            if second_to_last(f, block) == Some(if_inst)
                && f.match_nop(slot(f, if_inst, FALSE_INST))
            {
                let exit = *f.list(block).last().expect("instructions");
                if compatible(f, true_inst, exit) {
                    // An empty block as then-branch.
                    let nop = nop_with_range_of(f, true_inst);
                    f.set_child_at(if_inst, TRUE_INST, nop);
                }
            }
            return false;
        }
        let target = f.branch_target(true_inst);
        // Held so that the block is not disconnected in between.
        f.add_ref(target);
        f.remove_block(target);
        f.set_child_at(if_inst, TRUE_INST, target);
        f.release_ref(target);
        true
    }

    /// `InlineExitBranch`: the target of the block's unconditional exit is appended.
    fn inline_exit_branch(&mut self, f: &mut Function, block: NodeId) -> bool {
        let Some(&exit) = f.list(block).last() else {
            return false;
        };
        if !self.can_inline(f, exit) {
            return false;
        }
        let target = f.branch_target(exit);
        let len = f.list(block).len();
        f.remove_at(block, len - 1);
        self.bc.index_of_first_already_transformed_instruction = f.list(block).len();
        for inst in f.list(target).to_vec() {
            f.push(block, inst);
        }
        f.remove_block(target);
        true
    }

    /// `CanInline`: a branch to a block of this container with no other entry.
    fn can_inline(&self, f: &Function, inst: NodeId) -> bool {
        if !matches!(f.inst(inst), Inst::Branch { .. }) {
            return false;
        }
        let target = f.branch_target(inst);
        f.node(target).parent == Some(self.current_container)
            && f.node(target).incoming_edge_count == 1
    }

    /// `MergeCommonBranches`: an exit both branches share is produced once after the if.
    fn merge_common_branches(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        if_inst: NodeId,
    ) {
        let mut then_exits = Vec::new();
        add_exits(f, slot(f, if_inst, TRUE_INST), 0, &mut then_exits);
        if then_exits.is_empty() {
            return;
        }
        let false_inst_index = index_of(f, block, if_inst) + 1;
        let mut else_exits = Vec::new();
        add_exits(f, block, false_inst_index, &mut else_exits);
        let mut common_exit: Option<NodeId> = None;
        for &exit in &else_exits {
            if !then_exits.iter().any(|&e2| compatible(f, exit, e2)) {
                continue;
            }
            if common_exit.is_none_or(|c| {
                self.compare_block_exit_priority(f, exit, c, false) == Ordering::Greater
            }) {
                common_exit = Some(exit);
            }
        }
        let Some(common_exit) = common_exit else {
            return;
        };
        let block_exit = *f.list(block).last().expect("instructions");
        if self.compare_block_exit_priority(f, block_exit, common_exit, true) == Ordering::Greater
            && !will_short_circuit(f, block, if_inst, common_exit)
        {
            return;
        }
        self.produce_exit(f, ctx, slot(f, if_inst, TRUE_INST), 0, common_exit);
        self.produce_exit(f, ctx, block, false_inst_index, common_exit);
        if Some(if_inst) != second_to_last(f, block) {
            // An else-block removes the goto.
            let new = new_block(f);
            f.set_child_at(if_inst, FALSE_INST, new);
            let start = index_of(f, block, if_inst) + 1;
            let end = f.list(block).len() - 1;
            extract_block(f, block, start, end, new);
        }
        // The redundant `goto blockExit;` of the then-branch goes.
        let true_inst = slot(f, if_inst, TRUE_INST);
        match f.inst(true_inst) {
            Inst::Block { instructions, .. } if instructions.len() != 1 => {
                let len = instructions.len();
                f.remove_at(true_inst, len - 1);
            }
            _ => {
                let nop = nop_with_range_of(f, true_inst);
                f.set_child_at(if_inst, TRUE_INST, nop);
            }
        }
    }

    /// `ProduceExit`: inverts ifs until `search` ends in an exit compatible with `target`.
    fn produce_exit(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        search: NodeId,
        start: usize,
        target: NodeId,
    ) -> bool {
        let Some(exit) = try_get_exit(f, search) else {
            return false;
        };
        if compatible(f, exit, target) {
            return true;
        }
        if matches!(f.inst(search), Inst::Block { .. }) {
            let mut i = start;
            while let Some(&inst) = f.list(search).get(i) {
                if matches!(f.inst(inst), Inst::If { .. })
                    && self.produce_exit(f, ctx, slot(f, inst, TRUE_INST), 0, target)
                {
                    invert_if(f, ctx, search, inst);
                    return true;
                }
                i += 1;
            }
        }
        false
    }

    /// `PickBetterBlockExit`: inverts the if when its then-branch has the better exit.
    fn pick_better_block_exit(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        if_inst: NodeId,
    ) {
        let exit = get_exit(f, block);
        if is_empty(f, slot(f, if_inst, FALSE_INST))
            && let Some(true_exit) = try_get_exit(f, slot(f, if_inst, TRUE_INST))
            && self.compare_block_exit_priority(f, true_exit, exit, false) == Ordering::Greater
        {
            invert_if(f, ctx, block, if_inst);
        }
    }

    /// `CompareBlockExitPriority`: which exit reads better at the end of the block (the
    /// greater); `strongly` ignores IL order.
    fn compare_block_exit_priority(
        &self,
        f: &Function,
        exit1: NodeId,
        exit2: NodeId,
        strongly: bool,
    ) -> Ordering {
        let keyword1 = self.keyword_exit(f, exit1);
        let keyword2 = self.keyword_exit(f, exit2);
        if keyword1.is_some() != keyword2.is_some() {
            return if keyword1.is_some() {
                Ordering::Less
            } else {
                Ordering::Greater
            };
        }
        if let (Some(k1), Some(k2)) = (keyword1, keyword2) {
            if container_kind(f, self.current_container) == ContainerKind::Switch {
                if (k1 == Keyword::Break) != (k2 == Keyword::Break) {
                    return if k1 == Keyword::Break {
                        Ordering::Greater
                    } else {
                        Ordering::Less
                    };
                }
            } else {
                if (k1 == Keyword::Break) != (k2 == Keyword::Break) {
                    return if k1 == Keyword::Break {
                        Ordering::Less
                    } else {
                        Ordering::Greater
                    };
                }
                if (k1 == Keyword::Continue) != (k2 == Keyword::Continue) {
                    return if k1 == Keyword::Continue {
                        Ordering::Greater
                    } else {
                        Ordering::Less
                    };
                }
            }
        } else {
            // Branches and leaves.
            let is_branch1 = matches!(f.inst(exit1), Inst::Branch { .. });
            let is_branch2 = matches!(f.inst(exit2), Inst::Branch { .. });
            if is_branch1 != is_branch2 {
                return if is_branch1 {
                    Ordering::Less
                } else {
                    Ordering::Greater
                };
            }
            let leave_container = |e: NodeId| {
                f.match_leave(e)
                    .filter(|&(_, v)| f.match_nop(v))
                    .map(|(c, _)| c)
            };
            if let (Some(c1), Some(c2)) = (leave_container(exit1), leave_container(exit2))
                && c1 != c2
            {
                return if is_descendant_of(f, c2, c1) {
                    Ordering::Greater
                } else {
                    Ordering::Less
                };
            }
        }
        if strongly {
            return Ordering::Equal;
        }
        if let (Some(b1), Some(b2)) = (f.match_branch(exit1), f.match_branch(exit2)) {
            return f.il_range(b1).start.cmp(&f.il_range(b2).start);
        }
        if let (Some((_, a1)), Some((_, a2))) = (f.match_leave(exit1), f.match_leave(exit2)) {
            return f.il_range(a1).start.cmp(&f.il_range(a2).start);
        }
        f.il_range(exit1).start.cmp(&f.il_range(exit2).start)
    }

    /// `IsKeywordExit`: the keyword an exit becomes (`Other` for throws and the like), or
    /// `None` for a plain branch or leave.
    fn keyword_exit(&self, f: &Function, exit: NodeId) -> Option<Keyword> {
        match *f.inst(exit) {
            Inst::Branch { .. } => {
                let target = f.branch_target(exit);
                let container = f.node(target).parent.expect("target in a container");
                is_continue_block(f, container, target).then_some(Keyword::Continue)
            }
            Inst::Leave {
                target: Some(container),
                ..
            } => {
                if Some(container) == f.body {
                    Some(Keyword::Return)
                } else if container_kind(f, container) != ContainerKind::Normal {
                    Some(Keyword::Break)
                } else {
                    None
                }
            }
            _ => Some(Keyword::Other),
        }
    }
}

/// `AddExits`: the exit of `search` and of the then-branches of its ifs from `start` on.
fn add_exits(f: &Function, search: NodeId, start: usize, exits: &mut Vec<NodeId>) {
    let Some(exit) = try_get_exit(f, search) else {
        return;
    };
    exits.push(exit);
    if matches!(f.inst(search), Inst::Block { .. }) {
        for &inst in f.list(search).iter().skip(start) {
            if matches!(f.inst(inst), Inst::If { .. }) {
                add_exits(f, slot(f, inst, TRUE_INST), 0, exits);
            }
        }
    }
}

/// `WillShortCircuit`: the if and the one holding `else_exit` follow each other and both
/// only exit, so they combine into one condition.
fn will_short_circuit(f: &Function, block: NodeId, if_inst: NodeId, else_exit: NodeId) -> bool {
    let then_inst_is_single_exit = |inst: NodeId| {
        let Some((_, true_inst)) = f.match_if_instruction(inst) else {
            return false;
        };
        let single = match f.inst(true_inst) {
            Inst::Block { instructions, .. } => instructions.len() == 1,
            _ => true,
        };
        single && try_get_exit(f, true_inst).is_some()
    };
    if !then_inst_is_single_exit(if_inst) {
        return false;
    }
    let mut else_if = else_exit;
    while f.node(else_if).parent != Some(block) {
        else_if = f.node(else_if).parent.expect("an ancestor in the block");
    }
    f.list(block).iter().position(|&i| i == else_if) == Some(index_of(f, block, if_inst) + 1)
        && then_inst_is_single_exit(else_if)
}

/// `InvertIf`: the then-branch becomes the rest of the block, and the rest of the block (or
/// its exit) the then-branch, under the negated condition.
pub(crate) fn invert_if(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    if_inst: NodeId,
) {
    let exit = get_exit(f, block);
    let then_inst = slot(f, if_inst, TRUE_INST);
    f.add_ref(then_inst);
    if Some(if_inst) != second_to_last(f, block) {
        let new = new_block(f);
        f.set_child_at(if_inst, TRUE_INST, new);
        let start = index_of(f, block, if_inst) + 1;
        let end = f.list(block).len();
        extract_block(f, block, start, end, new);
    } else {
        f.set_child_at(if_inst, TRUE_INST, exit);
        let len = f.list(block).len();
        f.remove_at(block, len - 1);
    }
    if matches!(f.inst(then_inst), Inst::Block { .. }) {
        for inst in f.list(then_inst).to_vec() {
            f.push(block, inst);
        }
    } else {
        f.push(block, then_inst);
    }
    f.release_ref(then_inst);
    negate_condition(f, ctx, if_inst);
    crate::il::transforms::expression_transforms::run_on_single_statement(f, ctx, if_inst);
}

/// `SwapEmptyThen`: an empty then-branch swaps with the else-branch.
fn swap_empty_then(f: &mut Function, ctx: &mut Context<'_, '_, '_>, if_inst: NodeId) {
    let old_true = slot(f, if_inst, TRUE_INST);
    if !is_empty(f, old_true) {
        return;
    }
    let false_inst = slot(f, if_inst, FALSE_INST);
    f.set_child_at(if_inst, TRUE_INST, false_inst);
    let nop = nop_with_range_of(f, old_true);
    f.set_child_at(if_inst, FALSE_INST, nop);
    negate_condition(f, ctx, if_inst);
}

/// `IntroduceShortCircuit`: `if (a) { if (b) x }` becomes `if (a && b) x`.
fn introduce_short_circuit(f: &mut Function, ctx: &mut Context<'_, '_, '_>, if_inst: NodeId) {
    if !is_empty(f, slot(f, if_inst, FALSE_INST)) {
        return;
    }
    let true_block = slot(f, if_inst, TRUE_INST);
    let Inst::Block {
        ref instructions,
        final_inst,
        ..
    } = *f.inst(true_block)
    else {
        return;
    };
    let [nested] = instructions[..] else {
        return;
    };
    if !f.match_nop(final_inst) {
        return;
    }
    let Some((nested_condition, nested_true)) = f.match_if_instruction(nested) else {
        return;
    };
    // `IfInstruction.LogicAnd`.
    let condition = slot(f, if_inst, CONDITION);
    let csharp_type = f.infer_type(ctx.ts, nested_condition);
    let zero = f.add(Inst::LdcI4 { value: 0 });
    let and = f.add(Inst::If {
        condition,
        true_inst: nested_condition,
        false_inst: zero,
        csharp_type: Some(csharp_type),
    });
    f.set_child_at(if_inst, CONDITION, and);
    f.set_child_at(if_inst, TRUE_INST, nested_true);
}

/// `OrderIfBlocks`: the branches follow IL order.
fn order_if_blocks(f: &mut Function, ctx: &mut Context<'_, '_, '_>, if_inst: NodeId) {
    let true_inst = slot(f, if_inst, TRUE_INST);
    let false_inst = slot(f, if_inst, FALSE_INST);
    if is_empty(f, false_inst)
        || get_start_il_offset(f, true_inst) <= get_start_il_offset(f, false_inst)
    {
        return;
    }
    f.add_ref(true_inst);
    f.set_child_at(if_inst, TRUE_INST, false_inst);
    f.set_child_at(if_inst, FALSE_INST, true_inst);
    f.release_ref(true_inst);
    negate_condition(f, ctx, if_inst);
}
