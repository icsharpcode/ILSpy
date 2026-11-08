//! `IL/Transforms/CopyPropagation.cs`. The state machine decompilers use `Propagate` on its
//! own; the pipeline step comes later.

use crate::il::ast::{Flags, Function, Inst, NodeId, VarId, Variable, VariableKind};
use crate::util::interval::Interval;

use super::Context;
use super::il_inlining::{inline_into, options};

/// `Propagate`: replaces every load of the single-definition variable stored by `store`
/// with a copy of the stored value.
pub fn propagate(
    f: &mut Function,
    variables: &mut Vec<VarId>,
    ctx: &mut Context<'_, '_, '_>,
    store: NodeId,
) {
    let (v, value) = f.match_stloc(store).expect("stloc");
    debug_assert!(f.var(v).is_single_definition());
    let block = f.node(store).parent.expect("store in a block");
    let mut i = f.node(store).child_index as usize;
    do_propagate(f, variables, ctx, v, value, block, &mut i);
}

/// `DoPropagate`: the arguments of the copied expression are un-inlined into `C_n` stack
/// slots first, so only the expression itself is duplicated.
fn do_propagate(
    f: &mut Function,
    variables: &mut Vec<VarId>,
    ctx: &mut Context<'_, '_, '_>,
    v: VarId,
    copied: NodeId,
    block: NodeId,
    i: &mut usize,
) {
    let first_uninlined = *i;
    let args = f.inst(copied).children();
    let mut uninlined = Vec::with_capacity(args.len());
    for &arg in &args {
        let ty = f.infer_type(ctx.ts, arg);
        let stack_type = f.result_type(ctx.ts, arg);
        let name = format!("C_{}", f.il_range(arg).start);
        let temp = f.add_var(Variable {
            kind: VariableKind::StackSlot,
            ty,
            stack_type,
            index: None,
            name,
            has_generated_name: true,
            initial_value_is_initialized: false,
            uses_initial_value: false,
            is_ref_read_only: false,
            remove_if_redundant: false,
            state_machine_field: None,
            load_instructions: Vec::new(),
            store_instructions: Vec::new(),
            address_instructions: Vec::new(),
        });
        let st = f.add(Inst::StLoc {
            var: temp,
            value: arg,
            il_stack_was_empty: false,
            is_stack_adjustment: false,
        });
        f.insert(block, *i, st);
        *i += 1;
        uninlined.push(temp);
    }
    variables.extend(&uninlined);
    for expr in f.var(v).load_instructions.clone() {
        let clone = f.clone_subtree(copied);
        for (j, &temp) in uninlined.iter().enumerate() {
            let child = f.inst(clone).children()[j];
            let load = f.add(Inst::LdLoc { var: temp });
            f.replace(child, load);
        }
        // A copy from far away: its old IL range would give wrong sequence points.
        f.set_il_range(clone, Interval::default());
        let parent = f.node(expr).parent.expect("load has a parent");
        let is_stobj_target =
            matches!(f.inst(parent), Inst::StObj { target, .. } if *target == expr);
        if is_stobj_target && f.has_direct_flag(clone, Flags::MAY_THROW) {
            let value_pure = match *f.inst(parent) {
                Inst::StObj { value, .. } => crate::il::semantic_helper::is_pure(f.flags(value)),
                _ => true,
            };
            let early_thrower = matches!(f.inst(clone), Inst::LdElema { .. } | Inst::LdFlda { .. });
            if early_thrower && !value_pure {
                // C# computes the stored value before dereferencing the target, so the copied
                // ldflda / ldelema delays its exception, as inlining does.
                match f.inst_mut(clone) {
                    Inst::LdFlda {
                        delay_exceptions, ..
                    }
                    | Inst::LdElema {
                        delay_exceptions, ..
                    } => *delay_exceptions = true,
                    _ => {}
                }
            }
        }
        f.replace(expr, clone);
    }
    f.remove_at(block, *i);
    let c = inline_into(f, block, *i, options::NONE, ctx);
    *i = i.wrapping_sub(c + 1);
    if !uninlined.is_empty() {
        // The stores of the un-inlined arguments are candidates too: rewind to them.
        *i = (*i as isize).min(first_uninlined as isize - 1) as usize;
    }
}
