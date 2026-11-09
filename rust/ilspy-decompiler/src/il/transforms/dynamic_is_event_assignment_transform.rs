//! `IL/Transforms/DynamicIsEventAssignmentTransform.cs`: a dynamic `d.Event += b` lowers to
//! a runtime is-event check that picks between the event accessor and a compound assignment;
//! the diamond (an if/else, or two leaves when the result is returned) collapses to the
//! compound assignment.

use crate::il::ast::{DynamicKind, Function, Inst, NodeId, VarId};
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::expression_transforms::{
    transform_dynamic_add_assign_or_remove_assign, transform_dynamic_set_member,
};
use super::statement_transform::StatementTransformContext;

/// `ExpressionType.AddAssign` and `SubtractAssign`.
const ADD_ASSIGN: i32 = 63;
const SUBTRACT_ASSIGN: i32 = 73;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    _ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    let f = &mut r.function;
    if !try_collapse_if_else_diamond(f, sc, block, pos) {
        try_collapse_return_form(f, sc, block, pos);
    }
}

fn is_dynamic(f: &Function, inst: NodeId, kind: DynamicKind) -> bool {
    matches!(f.inst(inst), Inst::Dynamic { kind: k, .. } if *k == kind)
}

fn dynamic_args(f: &Function, inst: NodeId) -> Vec<NodeId> {
    match f.inst(inst) {
        Inst::Dynamic { args, .. } => args.clone(),
        _ => Vec::new(),
    }
}

fn dynamic_name(f: &Function, inst: NodeId) -> Option<String> {
    match f.inst(inst) {
        Inst::Dynamic { name, .. } => name.clone(),
        _ => None,
    }
}

/// `Block.Unwrap`.
fn unwrap_block(f: &Function, inst: NodeId) -> NodeId {
    if let Inst::Block { final_inst, .. } = *f.inst(inst)
        && f.list(inst).len() == 1
        && f.match_nop(final_inst)
    {
        return f.list(inst)[0];
    }
    inst
}

/// The cache prefix of the diamond.
struct CachePrefix {
    is_event: NodeId,
    flag: VarId,
    get_member_var: VarId,
    get_member: NodeId,
    diamond_pos: usize,
    value_variable: Option<(VarId, NodeId)>,
}

/// `MatchCachePrefix`:
/// ```text
/// stloc flag(dynamic.isevent(target))
/// if (comp(ldloc flag == 0)) stloc getMember(dynamic.getmember E(target))
/// [stloc copyOfValue(value)]
/// ```
fn match_cache_prefix(f: &Function, block: NodeId, pos: usize) -> Option<CachePrefix> {
    let list = f.list(block);
    if pos + 3 >= list.len() {
        return None;
    }
    let (flag, is_event) = f.match_stloc(list[pos])?;
    if !is_dynamic(f, is_event, DynamicKind::IsEvent) {
        return None;
    }
    let var = f.var(flag);
    if !(var.is_single_definition() && var.load_count() == 2) {
        return None;
    }
    let cache_store = match_lhs_cache_if_instruction(f, list[pos + 1], flag)?;
    let (get_member_var, get_member) = f.match_stloc(cache_store)?;
    if !is_dynamic(f, get_member, DynamicKind::GetMember) {
        return None;
    }
    let mut diamond_pos = pos + 2;
    // An optional temporary of the value, loaded once per branch of the diamond.
    let mut value_variable = None;
    if let Some((v, value)) = f.match_stloc(list[diamond_pos]) {
        let var = f.var(v);
        let loads_in_dynamic = var.load_instructions.iter().all(|&ld| {
            f.node(ld)
                .parent
                .is_some_and(|p| matches!(f.inst(p), Inst::Dynamic { .. }))
        });
        if var.is_single_definition() && var.load_count() == 2 && loads_in_dynamic {
            diamond_pos += 1;
            value_variable = Some((v, value));
        }
    }
    Some(CachePrefix {
        is_event,
        flag,
        get_member_var,
        get_member,
        diamond_pos,
        value_variable,
    })
}

/// `MatchFlagEqualsZero`: `comp(ldloc flag == ldc.i4 0)`.
fn match_flag_equals_zero(f: &Function, condition: NodeId, flag: VarId) -> bool {
    f.match_comp(condition, crate::il::ast::ComparisonKind::Equality)
        .is_some_and(|(l, r)| f.match_ldloc(l) == Some(flag) && f.match_ldc_i4(r) == Some(0))
}

/// `MatchLhsCacheIfInstruction`: `if (flag == 0) stloc ...`.
fn match_lhs_cache_if_instruction(f: &Function, inst: NodeId, flag: VarId) -> Option<NodeId> {
    let (condition, true_inst) = f.match_if_instruction(inst)?;
    if !match_flag_equals_zero(f, condition, flag) {
        return None;
    }
    let store = unwrap_block(f, true_inst);
    matches!(f.inst(store), Inst::StLoc { .. }).then_some(store)
}

/// `MatchEventAccessor`: `add_E(target, value)` for `+=`, `remove_E(target, value)` for `-=`.
fn match_event_accessor(
    f: &Function,
    invoke: NodeId,
    target: NodeId,
    value: NodeId,
    operation: i32,
    member_name: Option<&str>,
) -> bool {
    let accessor = match operation {
        ADD_ASSIGN => format!("add_{}", member_name.unwrap_or_default()),
        SUBTRACT_ASSIGN => format!("remove_{}", member_name.unwrap_or_default()),
        _ => return false,
    };
    let args = dynamic_args(f, invoke);
    dynamic_name(f, invoke).as_deref() == Some(accessor.as_str())
        && f.structurally_equal(args[0], target)
        && f.structurally_equal(args[1], value)
}

/// `MatchCachedEventCompound`: `dynamic.setmember E(target, dynamic.binary.operator
/// op(ldloc getMember, value))` matching the accessor call; returns the set-member and the
/// getter variable's load.
fn match_cached_event_compound(
    f: &Function,
    compound: NodeId,
    invoke: NodeId,
    is_event: NodeId,
    get_member_var: VarId,
) -> Option<(NodeId, NodeId)> {
    if !is_dynamic(f, compound, DynamicKind::SetMember) {
        return None;
    }
    let set_args = dynamic_args(f, compound);
    let &[set_target, set_value] = &set_args[..] else {
        return None;
    };
    let event_target = *dynamic_args(f, is_event).first()?;
    if !f.structurally_equal(event_target, set_target) {
        return None;
    }
    let Inst::Dynamic {
        kind: DynamicKind::BinaryOperator,
        operation,
        ref args,
        ..
    } = *f.inst(set_value)
    else {
        return None;
    };
    let &[left, right] = &args[..] else {
        return None;
    };
    if f.match_ldloc(left) != Some(get_member_var) {
        return None;
    }
    let name = dynamic_name(f, compound);
    if !match_event_accessor(f, invoke, set_target, right, operation, name.as_deref()) {
        return None;
    }
    Some((compound, left))
}

/// `MatchIsEventAssignmentIfInstruction`: the if/else diamond; returns the set-member, the
/// getter variable's load and the flag load in the condition.
fn match_is_event_assignment_if_instruction(
    f: &Function,
    inst: NodeId,
    prefix: &CachePrefix,
) -> Option<(NodeId, NodeId, NodeId)> {
    let Inst::If {
        condition,
        mut true_inst,
        mut false_inst,
        ..
    } = *f.inst(inst)
    else {
        return None;
    };
    let condition_use = if match_flag_equals_zero(f, condition, prefix.flag) {
        f.match_comp(condition, crate::il::ast::ComparisonKind::Equality)?
            .0
    } else if f.match_ldloc(condition) == Some(prefix.flag) {
        std::mem::swap(&mut true_inst, &mut false_inst);
        condition
    } else {
        return None;
    };
    // true_inst is now the compound branch, false_inst the accessor call.
    let invoke = unwrap_block(f, false_inst);
    if !is_dynamic(f, invoke, DynamicKind::InvokeMember) || dynamic_args(f, invoke).len() != 2 {
        return None;
    }
    let (set_member, get_member_use) = match_cached_event_compound(
        f,
        unwrap_block(f, true_inst),
        invoke,
        prefix.is_event,
        prefix.get_member_var,
    )?;
    Some((set_member, get_member_use, condition_use))
}

/// `InlineCopyOfValue`: the value temporary, now used once, is inlined.
fn inline_copy_of_value(f: &mut Function, block: NodeId, pos: usize, prefix: &CachePrefix) {
    let Some((v, value)) = prefix.value_variable else {
        return;
    };
    let var = f.var(v);
    if var.store_count() == 1 && var.address_count() == 0 && var.load_count() == 1 {
        let load = var.load_instructions[0];
        f.replace(load, value);
        f.remove_at(block, pos);
    }
}

/// `TryCollapseIfElseDiamond`.
fn try_collapse_if_else_diamond(
    f: &mut Function,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) -> bool {
    let Some(prefix) = match_cache_prefix(f, block, pos) else {
        return false;
    };
    // The diamond may be the statement or nested in an argument or a leave value.
    let diamond = f.list(block)[prefix.diamond_pos];
    for descendant in f.descendants_post(diamond) {
        let Some((set_member, get_member_use, condition_use)) =
            match_is_event_assignment_if_instruction(f, descendant, &prefix)
        else {
            continue;
        };
        // The duplicate condition collapses.
        f.replace(get_member_use, prefix.get_member);
        f.replace(condition_use, prefix.is_event);
        f.remove_at(block, pos);
        f.remove_at(block, pos);
        transform_dynamic_set_member(f, set_member);
        let collapsed = transform_dynamic_add_assign_or_remove_assign(f, descendant);
        debug_assert!(
            collapsed,
            "dynamic is-event assignment matched but could not be collapsed to a compound assignment"
        );
        inline_copy_of_value(f, block, pos, &prefix);
        // The following transforms visit the new compound assignment.
        sc.request_rerun_at(pos);
        return true;
    }
    false
}

/// `MatchIsEventReturnDiamond`: `if (condition) leave t(accessor call); leave t(compound)`.
fn match_is_event_return_diamond(
    f: &Function,
    if_inst: NodeId,
    leave: NodeId,
) -> Option<(NodeId, NodeId, NodeId)> {
    let (condition, true_inst) = f.match_if_instruction(if_inst)?;
    let (target, add_value) = f.match_leave(unwrap_block(f, true_inst))?;
    let (leave_target, compound_value) = f.match_leave(leave)?;
    if leave_target != target {
        return None;
    }
    (is_dynamic(f, add_value, DynamicKind::InvokeMember) && dynamic_args(f, add_value).len() == 2)
        .then_some((condition, add_value, compound_value))
}

/// `MatchUncachedIsEventReturnAssignment`.
fn match_uncached_is_event_return_assignment(f: &Function, if_inst: NodeId, leave: NodeId) -> bool {
    let Some((condition, invoke, compound_value)) =
        match_is_event_return_diamond(f, if_inst, leave)
    else {
        return false;
    };
    if !is_dynamic(f, condition, DynamicKind::IsEvent) {
        return false;
    }
    let Inst::DynamicCompoundAssign {
        operation,
        target: get_member,
        value,
        ..
    } = *f.inst(compound_value)
    else {
        return false;
    };
    if !is_dynamic(f, get_member, DynamicKind::GetMember) {
        return false;
    }
    let (Some(&event_target), Some(&member_target)) = (
        dynamic_args(f, condition).first(),
        dynamic_args(f, get_member).first(),
    ) else {
        return false;
    };
    let name = dynamic_name(f, get_member);
    f.structurally_equal(event_target, member_target)
        && match_event_accessor(f, invoke, member_target, value, operation, name.as_deref())
}

/// `MatchIsEventReturnAssignment`: the cached return diamond; returns the set-member and the
/// getter variable's load.
fn match_is_event_return_assignment(
    f: &Function,
    if_inst: NodeId,
    leave: NodeId,
    prefix: &CachePrefix,
) -> Option<(NodeId, NodeId)> {
    let (condition, invoke, compound_value) = match_is_event_return_diamond(f, if_inst, leave)?;
    if f.match_ldloc(condition) != Some(prefix.flag) {
        return None;
    }
    match_cached_event_compound(
        f,
        compound_value,
        invoke,
        prefix.is_event,
        prefix.get_member_var,
    )
}

/// `TryCollapseReturnForm`.
fn try_collapse_return_form(
    f: &mut Function,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    let prefix = match_cache_prefix(f, block, pos);
    let diamond_pos = prefix.as_ref().map_or(pos, |p| p.diamond_pos);
    let list = f.list(block);
    if diamond_pos + 1 >= list.len() {
        return;
    }
    let (if_inst, leave) = (list[diamond_pos], list[diamond_pos + 1]);
    let cached_match = match &prefix {
        Some(p) => match match_is_event_return_assignment(f, if_inst, leave, p) {
            Some(m) => Some(m),
            None => return,
        },
        None => {
            if !match_uncached_is_event_return_assignment(f, if_inst, leave) {
                return;
            }
            None
        }
    };
    // The `if (isevent) leave add` branch goes; the compound leave stays as the result.
    f.remove_at(block, diamond_pos);
    if let (Some(p), Some((set_member, get_member_use))) = (&prefix, cached_match) {
        f.replace(get_member_use, p.get_member);
        f.remove_at(block, pos);
        f.remove_at(block, pos);
        transform_dynamic_set_member(f, set_member);
        inline_copy_of_value(f, block, pos, p);
    }
    sc.request_rerun_at(pos);
}
