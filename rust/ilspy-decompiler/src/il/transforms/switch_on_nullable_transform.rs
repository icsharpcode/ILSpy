//! `IL/Transforms/SwitchOnNullableTransform.cs`: a switch on `GetValueOrDefault()` behind a
//! `HasValue` check becomes a lifted switch with a `case null` section.

use crate::il::ast::{Function, Inst, NodeId, TypeOperand};
use crate::il::control_flow::switch_detection::{
    match_has_value_call, simplify_switch_instruction,
};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::transforms::Context;
use crate::il::transforms::il_inlining::known;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeId;
use crate::util::long_set::LongSet;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.lift_nullables {
        return;
    }
    let f = &mut r.function;
    let body = f.body.expect("body");
    let blocks: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect();
    let mut changed_containers: Vec<NodeId> = Vec::new();
    for block in blocks {
        let mut changed = false;
        let mut i = f.list(block).len() as isize - 1;
        while i >= 0 {
            let u = i as usize;
            let list = f.list(block).to_vec();
            if let Some(new_switch) = match_switch_on_nullable(f, ctx, &list, u) {
                f.add_il_range(new_switch, f.il_range(list[u - 2]));
                f.replace(list[u + 1], new_switch);
                for _ in 0..3 {
                    f.remove_at(block, u - 2);
                }
                i -= 2;
                changed = true;
            } else if let Some(new_switch) = match_roslyn_switch_on_nullable(f, ctx, &list, u) {
                f.add_il_range(new_switch, f.il_range(list[u]));
                f.add_il_range(new_switch, f.il_range(list[u + 1]));
                f.replace(list[u], new_switch);
                f.remove_at(block, u + 1);
                changed = true;
            }
            i -= 1;
        }
        if !changed {
            continue;
        }
        simplify_switch_instruction(f, block, ctx);
        if let Some(container) = f.node(block).parent
            && matches!(f.inst(container), Inst::BlockContainer { .. })
            && !changed_containers.contains(&container)
        {
            changed_containers.push(container);
        }
    }
    for container in changed_containers {
        sort_blocks(f, container);
    }
}

/// `NullableLiftingTransform.MatchGetValueOrDefault`: the argument of
/// `call Nullable<T>.GetValueOrDefault()`.
fn match_get_value_or_default(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let [arg] = c.args[..] else {
        return None;
    };
    if c.method.method.name != "GetValueOrDefault" {
        return None;
    }
    let declaring = c.method.method.declaring?;
    (known(ctx, declaring) == Some(KnownTypeCode::NullableOfT)).then_some(arg)
}

/// The declaring type of the `get_HasValue` call.
fn nullable_type(f: &Function, get_has_value: NodeId) -> Option<TypeId> {
    match f.inst(get_has_value) {
        Inst::Call(c) => c.method.method.declaring,
        _ => None,
    }
}

/// `MatchSwitchOnNullable`: the legacy C# pattern.
///
/// ```text
/// stloc tmp(ldloca switchValueVar)
/// stloc switchVariable(call GetValueOrDefault(ldloc tmp))
/// if (logic.not(call get_HasValue(ldloc tmp))) br nullCaseBlock
/// br switchBlock
/// ```
fn match_switch_on_nullable(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    list: &[NodeId],
    i: usize,
) -> Option<NodeId> {
    if i < 2 {
        return None;
    }
    let (tmp, ldloca) = f.match_stloc(list[i - 2])?;
    let (switch_variable, get_value_or_default) = f.match_stloc(list[i - 1])?;
    let (condition, true_inst) = f.match_if_instruction(list[i])?;
    if !f.var(tmp).is_single_definition() || f.var(tmp).load_count() != 2 {
        return None;
    }
    if !f.var(switch_variable).is_single_definition() || f.var(switch_variable).load_count() != 1 {
        return None;
    }
    let switch_block = f.match_branch(*list.get(i + 1)?)?;
    let null_case_block = f.match_branch(true_inst)?;
    let Inst::LdLoca {
        var: switch_value_var,
    } = *f.inst(ldloca)
    else {
        return None;
    };
    let get_has_value = f.match_logic_not(condition)?;
    let get_value_or_default_arg = match_get_value_or_default(f, ctx, get_value_or_default)?;
    let get_has_value_arg = match_has_value_call(f, ctx, get_has_value)?;
    if f.match_ldloc(get_has_value_arg) != Some(tmp)
        || f.match_ldloc(get_value_or_default_arg) != Some(tmp)
    {
        return None;
    }
    // switch (ldloc switchVariable) { ... }
    let [switch_inst] = *f.list(switch_block) else {
        return None;
    };
    if f.node(switch_block).incoming_edge_count != 1
        || !matches!(f.inst(switch_inst), Inst::Switch { .. })
    {
        return None;
    }
    let ty = nullable_type(f, get_has_value);
    let value = f.add(Inst::LdLoc {
        var: switch_value_var,
    });
    Some(build_lifted_switch(
        f,
        null_case_block,
        switch_inst,
        value,
        ty,
    ))
}

/// `BuildLiftedSwitch`: the sections of `switch_inst` and a `case null` section.
fn build_lifted_switch(
    f: &mut Function,
    null_case_block: NodeId,
    switch_inst: NodeId,
    value: NodeId,
    ty: Option<TypeId>,
) -> NodeId {
    let mut sections = f.list(switch_inst).to_vec();
    let body = f.add(Inst::Branch {
        target: crate::il::ast::BranchTarget::Block(null_case_block),
    });
    sections.push(f.add(Inst::SwitchSection {
        labels: LongSet::empty(),
        body,
        has_null_label: true,
        is_compiler_generated_default_section: false,
    }));
    f.add(Inst::Switch {
        value,
        sections,
        ty,
        is_lifted: true,
        result_type: None,
    })
}

/// `MatchRoslynSwitchOnNullable`.
///
/// ```text
/// if (logic.not(call get_HasValue(target))) br nullCaseBlock
/// br switchBlock
/// --
/// stloc switchVar(call GetValueOrDefault(ldloca tmp))   (or inlined into the switch)
/// switch (ldloc switchVar) { ... }
/// ```
fn match_roslyn_switch_on_nullable(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    list: &[NodeId],
    i: usize,
) -> Option<NodeId> {
    let (condition, true_inst) = f.match_if_instruction(list[i])?;
    let switch_block = f.match_branch(*list.get(i + 1)?)?;
    let null_case_block = f.match_branch(true_inst)?;
    let get_has_value = f.match_logic_not(condition)?;
    let target = match_has_value_call(f, ctx, get_has_value)?;
    if !is_pure(f.flags(target)) {
        return None;
    }
    if f.node(switch_block).incoming_edge_count != 1 {
        return None;
    }
    let switch_inst = match *f.list(switch_block) {
        [store, sw] => {
            let (switch_var, get_value_or_default) = f.match_stloc(store)?;
            if !f.var(switch_var).is_single_definition() || f.var(switch_var).load_count() != 1 {
                return None;
            }
            let target2 = match_get_value_or_default(f, ctx, get_value_or_default)?;
            if !f.structurally_equal(target2, target) || !matches!(f.inst(sw), Inst::Switch { .. })
            {
                return None;
            }
            sw
        }
        // `call GetValueOrDefault(ldloca tmp)` inlined into the switch.
        [sw] => {
            let Inst::Switch { value, .. } = *f.inst(sw) else {
                return None;
            };
            let target2 = match_get_value_or_default(f, ctx, value)?;
            if !f.structurally_equal(target2, target) {
                return None;
            }
            sw
        }
        _ => return None,
    };
    let ty = nullable_type(f, get_has_value);
    let switch_value = if let Inst::LdLoca { var } = *f.inst(target) {
        let ld = f.add(Inst::LdLoc { var });
        f.add_il_range(ld, f.il_range(target));
        ld
    } else {
        let declaring = ty.expect("get_HasValue has a declaring type");
        f.add(Inst::LdObj {
            target,
            ty: TypeOperand {
                token: 0,
                ty: declaring,
            },
            is_volatile: false,
            unaligned: 0,
        })
    };
    Some(build_lifted_switch(
        f,
        null_case_block,
        switch_inst,
        switch_value,
        ty,
    ))
}
