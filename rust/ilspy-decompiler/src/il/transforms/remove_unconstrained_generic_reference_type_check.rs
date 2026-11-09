//! `IL/Transforms/RemoveUnconstrainedGenericReferenceTypeCheck.cs`: the reference type check
//! the compiler emits before a constrained call on an unconstrained generic receiver goes,
//! and the arguments stored before it inline into the call:
//!
//! ```text
//! [stloc address(...)]
//! stloc temp(default.value ``0)
//! if (comp.o(ldloc temp == ldnull)) Block IL_002a {
//!     stloc temp(ldobj ``0(ldloc address))
//!     stloc address(ldloca temp)
//! }
//! stloc V_i(expr_i)
//! ...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), ldloc V_i ...))...
//! =>
//! [stloc address(...)]
//! ...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), expr_i ...))...
//! ```

use crate::il::ast::{ComparisonKind, Inst, NodeId};
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{can_move_into_call_virt, variable_can_be_used_for_inlining};
use super::statement_transform::StatementTransformContext;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    let f = &mut r.function;
    let start_pos = pos;
    // stloc temp(default.value ``0)
    let Some((temp, default_value)) = f.match_stloc(f.list(block)[pos]) else {
        return;
    };
    let Inst::DefaultValue { ty, .. } = *f.inst(default_value) else {
        return;
    };
    let ty = ty.ty;
    let t = f.var(temp);
    if t.store_count() != 2 || t.load_count() != 1 || t.address_count() != 1 {
        return;
    }
    // if (comp.o(ldloc temp == ldnull)) Block { stloc temp(ldobj ``0(ldloc address));
    // stloc address(ldloca temp) }
    let Some(&if_inst) = f.list(block).get(pos + 1) else {
        return;
    };
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return;
    };
    if f.match_comp_null(condition, ComparisonKind::Equality)
        .and_then(|l| f.match_ldloc(l))
        != Some(temp)
    {
        return;
    }
    if !matches!(f.inst(true_inst), Inst::Block { .. }) || !f.match_nop(false_inst) {
        return;
    }
    let &[temp_store, address_reassign] = f.list(true_inst) else {
        return;
    };
    let (Some((temp_store_var, temp_store_value)), Some((address, address_value))) =
        (f.match_stloc(temp_store), f.match_stloc(address_reassign))
    else {
        return;
    };
    if temp_store_var != temp {
        return;
    }
    let Inst::LdObj {
        target: address_load_for_ldobj,
        ty: ldobj_type,
        ..
    } = *f.inst(temp_store_value)
    else {
        return;
    };
    if ldobj_type.ty != ty || f.match_ldloc(address_load_for_ldobj) != Some(address) {
        return;
    }
    if !matches!(*f.inst(address_value), Inst::LdLoca { var } if var == temp) {
        return;
    }
    let a = f.var(address);
    if a.store_count() != 2 || a.load_count() != 2 || a.address_count() != 0 {
        return;
    }
    let pos = pos + 2;
    // pos is the first store to V_i.
    // ...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), ldloc V_i ...))...
    let Some(&call_target) = f
        .var(address)
        .load_instructions
        .iter()
        .find(|&&l| l != address_load_for_ldobj)
    else {
        return;
    };
    let Some(ldobj_if_ref) = f
        .node(call_target)
        .parent
        .filter(|&p| matches!(f.inst(p), Inst::LdObjIfRef { .. }))
    else {
        return;
    };
    let Some(call) = f
        .node(ldobj_if_ref)
        .parent
        .filter(|&p| matches!(f.inst(p), Inst::CallVirt(_)))
    else {
        return;
    };
    let Inst::CallVirt(c) = f.inst(call) else {
        unreachable!("a callvirt");
    };
    if c.args.first() != Some(&ldobj_if_ref) || c.constrained_to.map(|t| t.ty) != Some(ty) {
        return;
    }
    let args = c.args.clone();
    let mut containing_stmt = call;
    loop {
        match f.node(containing_stmt).parent {
            Some(p) if p == block => break,
            Some(p) => containing_stmt = p,
            None => return,
        }
    }
    let stmt_index = f.node(containing_stmt).child_index as usize;
    if stmt_index < pos {
        return;
    }
    // All temporaries the call uses must inline.
    let mut temporary_init_index = stmt_index as isize - 1;
    let mut replacements = Vec::new();
    let mut arg_index = args.len() as isize - 1;
    while arg_index > 0 && temporary_init_index >= pos as isize {
        let argument = args[arg_index as usize];
        if let Some(var) = f.match_ldloc(argument)
            && let Some((v, expr)) = f.match_stloc(f.list(block)[temporary_init_index as usize])
            && v == var
            && variable_can_be_used_for_inlining(f, var)
        {
            if !can_move_into_call_virt(f, ctx, expr, containing_stmt, call, argument) {
                return;
            }
            replacements.push((argument, expr));
            temporary_init_index -= 1;
        }
        arg_index -= 1;
    }
    // Every store to a V_i is processed.
    if temporary_init_index != pos as isize - 1 {
        return;
    }
    for (argument, expr) in replacements {
        f.replace(argument, expr);
    }
    let stmt_index = f.node(containing_stmt).child_index as usize;
    for _ in start_pos..stmt_index {
        f.remove_at(block, start_pos);
    }
}
