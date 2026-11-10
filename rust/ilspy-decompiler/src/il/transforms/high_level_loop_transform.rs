//! `IL/Transforms/HighLevelLoopTransform.cs`: plain loops become `while (condition)`,
//! `do ... while (condition)` and `for` loops (container kinds While, DoWhile and For).

use crate::il::ast::{ContainerKind, Flags, Function, Inst, NodeId, NopKind, VarId, VariableKind};
use crate::il::control_flow::condition_detection::{extract_block, get_increment_block, invert_if};
use crate::il::control_flow::yield_return_decompiler::new_block;
use crate::il::reader::ReadFunction;

use super::Context;
use super::expression_transforms::{logic_not, match_logic_and, run_on_single_statement};

const CONDITION: usize = 0;
const TRUE_INST: usize = 1;
const FALSE_INST: usize = 2;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    // `function.Descendants`: the body, then the local functions, in post-order.
    let roots: Vec<NodeId> = f
        .body
        .into_iter()
        .chain(f.local_functions.iter().copied())
        .collect();
    let loops: Vec<NodeId> = roots
        .into_iter()
        .flat_map(|root| f.descendants_post(root))
        .filter(|&n| {
            matches!(
                f.inst(n),
                Inst::BlockContainer {
                    kind: ContainerKind::Loop,
                    ..
                }
            )
        })
        .collect();
    for lp in loops {
        // Still a plain loop: an earlier match may not have changed it.
        if !matches!(
            f.inst(lp),
            Inst::BlockContainer {
                kind: ContainerKind::Loop,
                ..
            }
        ) {
            continue;
        }
        if let Some((condition, body)) = match_while_loop(f, ctx, lp) {
            if ctx.settings.for_statement {
                match_for_loop(f, ctx, lp, condition, body);
            }
            continue;
        }
        if ctx.settings.do_while_statement {
            match_do_while_loop(f, ctx, lp);
        }
    }
}

fn set_kind(f: &mut Function, container: NodeId, kind: ContainerKind) {
    if let Inst::BlockContainer { kind: k, .. } = f.inst_mut(container) {
        *k = kind;
    }
}

fn slot(f: &Function, if_inst: NodeId, index: usize) -> NodeId {
    f.inst(if_inst).children()[index]
}

/// `MatchLeave(container)`: a `leave` of `container` without value.
fn is_leave_of(f: &Function, inst: NodeId, container: NodeId) -> bool {
    f.match_leave(inst)
        .is_some_and(|(c, value)| c == container && f.match_nop(value))
}

fn new_leave(f: &mut Function, container: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Leave {
        target: Some(container),
        value: nop,
    })
}

fn new_branch(f: &mut Function, block: NodeId) -> NodeId {
    f.add(Inst::Branch {
        target: crate::il::ast::BranchTarget::Block(block),
    })
}

/// `MatchWhileLoop`: `if (!condition) leave loop` (or `if (condition) { body } leave loop`) at
/// the start of the loop becomes the condition block of a while loop. Returns the condition
/// and the new body block.
fn match_while_loop(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    lp: NodeId,
) -> Option<(NodeId, NodeId)> {
    let entry = f.list(lp)[0];
    let &first = f.list(entry).first()?;
    let Inst::If { false_inst, .. } = *f.inst(first) else {
        return None;
    };
    if !f.match_nop(false_inst) {
        return None;
    }
    if uses_variable_captured_in_loop(f, lp, slot(f, first, CONDITION)) {
        return None;
    }
    if !is_leave_of(f, slot(f, first, TRUE_INST), lp) {
        // The loop body nested in the if: `if (condition) { body } leave loop`.
        let list = f.list(entry);
        if list.len() != 2 || !is_leave_of(f, *list.last()?, lp) {
            return None;
        }
        let true_inst = slot(f, first, TRUE_INST);
        if !f.has_flag(true_inst, Flags::END_POINT_UNREACHABLE) {
            let leave = new_leave(f, lp);
            f.push(true_inst, leave);
        }
        invert_if(f, ctx, entry, first);
    }
    set_kind(f, lp, ContainerKind::While);
    // Negate the condition; the exit moves to the else branch.
    let condition = slot(f, first, CONDITION);
    let negated = logic_not(f, ctx, condition);
    f.set_child_at(first, CONDITION, negated);
    let exit = slot(f, first, TRUE_INST);
    f.set_child_at(first, FALSE_INST, exit);
    // The rest of the entry block becomes the loop body.
    let body = new_block(f);
    f.add_ref(body);
    let end = f.list(entry).len();
    extract_block(f, entry, 1, end, body);
    f.insert(lp, 1, body);
    f.release_ref(body);
    if !f.has_flag(body, Flags::END_POINT_UNREACHABLE) {
        let leave = new_leave(f, lp);
        f.push(body, leave);
    }
    let branch = new_branch(f, body);
    f.set_child_at(first, TRUE_INST, branch);
    run_on_single_statement(f, ctx, first);
    Some((first, body))
}

/// `SplitConditions`: the operands of nested `&&`.
fn split_conditions(f: &Function, expression: NodeId, conditions: &mut Vec<NodeId>) {
    match match_logic_and(f, expression) {
        Some((l, r)) => {
            split_conditions(f, l, conditions);
            split_conditions(f, r, conditions);
        }
        None => conditions.push(expression),
    }
}

/// `IfInstruction.LogicAnd(lhs, rhs)`.
fn logic_and(f: &mut Function, ctx: &mut Context<'_, '_, '_>, lhs: NodeId, rhs: NodeId) -> NodeId {
    let ty = f.infer_type(ctx.ts, rhs);
    let zero = f.add(Inst::LdcI4 { value: 0 });
    f.add(Inst::If {
        condition: lhs,
        true_inst: rhs,
        false_inst: zero,
        csharp_type: Some(ty),
    })
}

/// `MatchDoWhileLoop`: the conditions at the end of a block become the condition block of a
/// do-while loop.
fn match_do_while_loop(f: &mut Function, ctx: &mut Context<'_, '_, '_>, lp: NodeId) -> bool {
    let Some((conditions, exit, swap, mut split, unwrap)) = analyze_do_while_conditions(f, lp)
    else {
        return false;
    };
    if conditions.is_empty() {
        return false;
    }
    let mut original = f.node(exit).parent.expect("the exit is in a block");
    if unwrap {
        // A condition block nested in a condition followed by a return: flip the condition and
        // swap the blocks.
        let return_condition = f.node(original).parent.expect("an if");
        let top = f.node(return_condition).parent.expect("a block");
        let index = f.node(return_condition).child_index as usize;
        let leave_function = f.list(top)[index + 1];
        let condition = slot(f, return_condition, CONDITION);
        let negated = logic_not(f, ctx, condition);
        f.set_child_at(return_condition, CONDITION, negated);
        f.set_child_at(return_condition, TRUE_INST, leave_function);
        run_on_single_statement(f, ctx, return_condition);
        let index = f.node(return_condition).child_index as usize;
        f.remove_at(top, index + 1);
        for inst in f.list(original).to_vec() {
            f.push(top, inst);
        }
        original = top;
        split = true;
    }
    let len = f.list(original).len();
    let start = len - conditions.len() - 1;
    for _ in start..len {
        f.remove_at(original, start);
    }
    let condition_block = if split {
        let block = new_block(f);
        f.push(lp, block);
        let branch = new_branch(f, block);
        f.push(original, branch);
        block
    } else {
        // Move the condition block to the end.
        let mut blocks = f.list(lp).to_vec();
        blocks.retain(|&b| b != original);
        blocks.push(original);
        f.set_list(lp, blocks);
        original
    };
    // All conditions and the exit combine into one if instruction.
    let range = f.il_range(exit);
    f.add_il_range(condition_block, range);
    let mut condition: Option<NodeId> = None;
    for &inst in &conditions {
        let range = f.il_range(inst);
        f.add_il_range(condition_block, range);
        match condition {
            None => {
                condition = Some(inst);
                if swap {
                    // The branches swap and the condition is negated.
                    let c = slot(f, inst, CONDITION);
                    let negated = logic_not(f, ctx, c);
                    f.set_child_at(inst, CONDITION, negated);
                    let true_inst = slot(f, inst, TRUE_INST);
                    f.set_child_at(inst, FALSE_INST, true_inst);
                    f.set_child_at(inst, TRUE_INST, exit);
                } else {
                    f.set_child_at(inst, FALSE_INST, exit);
                }
            }
            Some(combined) => {
                let current = slot(f, combined, CONDITION);
                let other = slot(f, inst, CONDITION);
                let other = if swap {
                    logic_not(f, ctx, other)
                } else {
                    other
                };
                let and = logic_and(f, ctx, other, current);
                f.set_child_at(combined, CONDITION, and);
            }
        }
    }
    let condition = condition.expect("at least one condition");
    f.push(condition_block, condition);
    run_on_single_statement(f, ctx, condition);
    set_kind(f, lp, ContainerKind::DoWhile);
    true
}

/// `AnalyzeDoWhileConditions`: the conditions, the exit, and whether to swap, split and unwrap.
#[allow(clippy::type_complexity)]
fn analyze_do_while_conditions(
    f: &Function,
    lp: NodeId,
) -> Option<(Vec<NodeId>, NodeId, bool, bool, bool)> {
    // From the bottom: the entry point is a condition block only when no other block is.
    for &block in f.list(lp).iter().rev() {
        if let Some((swap, unwrap, condition_block)) = match_do_while_condition_block(f, lp, block)
        {
            let conditions = collect_conditions(f, lp, condition_block, swap);
            let list = f.list(condition_block);
            // Split when the block is the entry point or holds other instructions too.
            let split = condition_block == f.list(lp)[0] || list.len() > conditions.len() + 1;
            return Some((conditions, *list.last()?, swap, split, unwrap));
        }
    }
    None
}

/// `CollectConditions`: the if instructions before the block's exit usable as loop conditions.
fn collect_conditions(f: &Function, lp: NodeId, block: NodeId, swap: bool) -> Vec<NodeId> {
    let entry = f.list(lp)[0];
    let list = f.list(block);
    let mut out = Vec::new();
    let mut i = list.len() as isize - 2;
    while i >= 0 {
        let inst = list[i as usize];
        let Inst::If { false_inst, .. } = *f.inst(inst) else {
            break;
        };
        if !f.match_nop(false_inst)
            || uses_variable_captured_in_loop(f, lp, slot(f, inst, CONDITION))
        {
            break;
        }
        let true_inst = slot(f, inst, TRUE_INST);
        let ok = if swap {
            is_leave_of(f, true_inst, lp)
        } else {
            f.match_branch(true_inst) == Some(entry)
        };
        if !ok {
            break;
        }
        out.push(inst);
        i -= 1;
    }
    out
}

/// `UsesVariableCapturedInLoop`.
fn uses_variable_captured_in_loop(f: &Function, lp: NodeId, condition: NodeId) -> bool {
    f.descendants_post(condition)
        .into_iter()
        .any(|n| variable_operand(f, n).is_some_and(|v| f.var(v).capture_scope == Some(lp)))
}

/// The variable of an `IInstructionWithVariableOperand`.
fn variable_operand(f: &Function, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::LdLoc { var }
        | Inst::LdLoca { var }
        | Inst::StLoc { var, .. }
        | Inst::TryCatchHandler { variable: var, .. }
        | Inst::Match { variable: var, .. }
        | Inst::PinnedRegion { variable: var, .. }
        | Inst::Using { variable: var, .. } => Some(var),
        _ => None,
    }
}

/// `MatchDoWhileConditionBlock`: `if (c) br entry; leave loop` or `if (c) leave loop; br
/// entry` at the end of `block` (or of a block nested in a condition before a return).
/// Returns whether to swap, whether the block is nested, and the condition block.
fn match_do_while_condition_block(
    f: &Function,
    lp: NodeId,
    block: NodeId,
) -> Option<(bool, bool, NodeId)> {
    let entry = f.list(lp)[0];
    let list = f.list(block);
    if list.len() < 2 {
        return None;
    }
    let mut last = list[list.len() - 1];
    let mut if_inst = list[list.len() - 2];
    let mut condition_block = block;
    let mut unwrap = false;
    let Inst::If { false_inst, .. } = *f.inst(if_inst) else {
        return None;
    };
    if !f.match_nop(false_inst) {
        return None;
    }
    let true_inst = slot(f, if_inst, TRUE_INST);
    if f.match_return(last).is_some() && matches!(f.inst(true_inst), Inst::Block { .. }) {
        let nested = f.list(true_inst);
        if nested.len() < 2 {
            return None;
        }
        last = nested[nested.len() - 1];
        if_inst = nested[nested.len() - 2];
        let Inst::If { false_inst, .. } = *f.inst(if_inst) else {
            return None;
        };
        if !f.match_nop(false_inst) {
            return None;
        }
        unwrap = true;
        condition_block = true_inst;
    }
    let swap = if f.match_branch(last) == Some(entry) {
        true
    } else if is_leave_of(f, last, lp) {
        false
    } else {
        return None;
    };
    let true_inst = slot(f, if_inst, TRUE_INST);
    let ok = if swap {
        is_leave_of(f, true_inst, lp)
    } else {
        f.match_branch(true_inst) == Some(entry)
    };
    ok.then_some((swap, unwrap, condition_block))
}

/// `MatchForLoop`: a while loop with an increment block (or a final increment statement)
/// becomes a for loop.
fn match_for_loop(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    lp: NodeId,
    while_condition: NodeId,
    body: NodeId,
) -> bool {
    let entry = f.list(lp)[0];
    // For loops have exactly two incoming edges at the entry point.
    if f.node(entry).incoming_edge_count != 2 {
        return false;
    }
    if let Some(increment) = get_increment_block(f, lp, body) {
        // The condition block, the loop body and the increment block at least.
        if f.list(increment).len() <= 1 || f.list(lp).len() < 3 {
            return false;
        }
        if stores_ref_local_used_after_loop(f, ctx, lp, increment) {
            return false;
        }
        let mut blocks = f.list(lp).to_vec();
        blocks.retain(|&b| b != increment);
        blocks.push(increment);
        f.set_list(lp, blocks);
        set_kind(f, lp, ContainerKind::For);
        return true;
    }
    // The increment statements move into a block of their own.
    let list = f.list(body);
    let (Some(&last), Some(&second_to_last)) =
        (list.last(), list.len().checked_sub(2).map(|i| &list[i]))
    else {
        return false;
    };
    if f.match_branch(last) != Some(entry) {
        return false;
    }
    // Only numeric increments.
    let Some(increment_variable) = match_increment(f, second_to_last) else {
        return false;
    };
    if f.var(increment_variable).kind == VariableKind::Parameter {
        return false;
    }
    let mut conditions = Vec::new();
    split_conditions(f, slot(f, while_condition, CONDITION), &mut conditions);
    let mut for_condition: Option<NodeId> = None;
    let mut number = 0;
    for &condition in &conditions {
        let nodes = f.descendants_post(condition);
        // The increment variable is used in the condition, which assigns nothing.
        if !nodes
            .iter()
            .any(|&n| matches!(*f.inst(n), Inst::LdLoc { var } if var == increment_variable))
        {
            break;
        }
        if nodes.iter().any(|&n| is_assignment(f, n)) {
            break;
        }
        match for_condition {
            None => {
                let ty = f.infer_type(ctx.ts, while_condition);
                let (true_inst, false_inst) = (
                    slot(f, while_condition, TRUE_INST),
                    slot(f, while_condition, FALSE_INST),
                );
                for_condition = Some(f.add(Inst::If {
                    condition,
                    true_inst,
                    false_inst,
                    // `whileCondition.InferType`: `void` for a statement.
                    csharp_type: Some(ty),
                }));
            }
            Some(fc) => {
                let current = slot(f, fc, CONDITION);
                let and = logic_and(f, ctx, current, condition);
                f.set_child_at(fc, CONDITION, and);
            }
        }
        number += 1;
    }
    let Some(for_condition) = for_condition else {
        return false;
    };
    f.replace(while_condition, for_condition);
    run_on_single_statement(f, ctx, for_condition);
    for &condition in conditions[number..].iter().rev() {
        let negated = logic_not(f, ctx, condition);
        let leave = new_leave(f, lp);
        let nop = f.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let inst = f.add(Inst::If {
            condition: negated,
            true_inst: leave,
            false_inst: nop,
            csharp_type: None,
        });
        f.insert(body, 0, inst);
        run_on_single_statement(f, ctx, inst);
    }
    // A new increment block at the end.
    let increment = new_block(f);
    f.push(lp, increment);
    let index = f.node(second_to_last).child_index as usize;
    f.remove_at(body, index);
    f.remove_at(body, index);
    f.push(increment, second_to_last);
    f.push(increment, last);
    let range = f.il_range(second_to_last);
    f.add_il_range(increment, range);
    let branch = new_branch(f, increment);
    f.push(body, branch);
    set_kind(f, lp, ContainerKind::For);
    true
}

/// `StoresRefLocalUsedAfterLoop`.
fn stores_ref_local_used_after_loop(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    lp: NodeId,
    increment: NodeId,
) -> bool {
    let list = f.list(increment);
    list[..list.len() - 1]
        .iter()
        .flat_map(|&inst| f.descendants_post(inst))
        .filter_map(|n| match *f.inst(n) {
            Inst::StLoc { var, .. } => Some(var),
            _ => None,
        })
        .any(|v| ctx.ts.is_by_ref_like(f.var(v).ty) && is_variable_used_after_loop(f, lp, v))
}

/// `IsVariableUsedAfterLoop`.
fn is_variable_used_after_loop(f: &Function, lp: NodeId, v: VarId) -> bool {
    let var = f.var(v);
    var.load_instructions
        .iter()
        .chain(&var.address_instructions)
        .chain(&var.store_instructions)
        .any(|&u| {
            common_parent(f, lp, u).is_some_and(|p| {
                matches!(
                    f.inst(p),
                    Inst::Block {
                        kind: crate::il::ast::BlockKind::ControlFlow,
                        ..
                    }
                )
            }) && is_before(f, lp, u)
        })
}

/// `GetCommonParent`.
fn common_parent(f: &Function, a: NodeId, b: NodeId) -> Option<NodeId> {
    let level = |n: NodeId| f.ancestors(n).count();
    let (mut a, mut b) = (Some(a), Some(b));
    let (mut la, mut lb) = (level(a?), level(b?));
    while la > lb {
        a = f.node(a?).parent;
        la -= 1;
    }
    while lb > la {
        b = f.node(b?).parent;
        lb -= 1;
    }
    while a != b {
        a = f.node(a?).parent;
        b = f.node(b?).parent;
    }
    a
}

/// `ILInstruction.IsBefore`: whether `a` comes before `b` in a post-order walk.
pub(crate) fn is_before(f: &Function, a: NodeId, b: NodeId) -> bool {
    let level = |n: NodeId| f.ancestors(n).count() + 1;
    let (mut a, mut b) = (a, b);
    let (mut level_a, mut level_b) = (level(a), level(b));
    let (original_a, original_b) = (level_a, level_b);
    while level_a > level_b {
        a = f.node(a).parent.expect("an ancestor");
        level_a -= 1;
    }
    while level_b > level_a {
        b = f.node(b).parent.expect("an ancestor");
        level_b -= 1;
    }
    if a == b {
        // One is a descendant of the other: the deeper one comes first in post-order.
        return original_a > original_b;
    }
    while f.node(a).parent != f.node(b).parent {
        a = f.node(a).parent.expect("an ancestor");
        b = f.node(b).parent.expect("an ancestor");
    }
    f.node(a).child_index < f.node(b).child_index
}

/// `IsAssignment`.
fn is_assignment(f: &Function, inst: NodeId) -> bool {
    matches!(
        f.inst(inst),
        Inst::StLoc { .. }
            | Inst::NumericCompoundAssign { .. }
            | Inst::UserDefinedCompoundAssign { .. }
            | Inst::DynamicCompoundAssign { .. }
    )
}

/// `MatchIncrement`: `stloc v(add(ldloc v, arg))` or `compound.assign(ldloca v, arg)`.
fn match_increment(f: &Function, inst: NodeId) -> Option<VarId> {
    match *f.inst(inst) {
        Inst::StLoc { var, value, .. } => match *f.inst(value) {
            Inst::BinaryNumeric {
                op: crate::il::ast::BinaryNumericOperator::Add,
                left,
                ..
            } if matches!(*f.inst(left), Inst::LdLoc { var: l } if l == var) => Some(var),
            _ => None,
        },
        Inst::NumericCompoundAssign {
            target_kind: crate::il::ast::CompoundTargetKind::Address,
            target,
            ..
        }
        | Inst::UserDefinedCompoundAssign {
            target_kind: crate::il::ast::CompoundTargetKind::Address,
            target,
            ..
        }
        | Inst::DynamicCompoundAssign {
            target_kind: crate::il::ast::CompoundTargetKind::Address,
            target,
            ..
        } => match *f.inst(target) {
            Inst::LdLoca { var } => Some(var),
            _ => None,
        },
        _ => None,
    }
}
