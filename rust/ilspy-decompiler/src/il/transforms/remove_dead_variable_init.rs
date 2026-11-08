//! `IL/Transforms/RemoveDeadVariableInit.cs`: clears `UsesInitialValue` of variables that are
//! never read uninitialized, and removes stores to variables that are never read (keeping
//! the side effects of the stored value).

use std::collections::VecDeque;

use crate::flow_analysis::definite_assignment::potentially_used_uninitialized;
use crate::il::ast::{Inst, VariableKind};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;

use super::Context;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    reset_uses_initial_value_flag(r, ctx);
    // In iterators and async methods the C# compiler stores null or default to variables that
    // go out of scope.
    let remove_dead_stores =
        r.async_return_type.is_some() || r.is_iterator || ctx.settings.remove_dead_stores;
    let mut queue: VecDeque<_> = r.variables.iter().copied().collect();
    while let Some(v) = queue.pop_front() {
        let f = &mut r.function;
        let var = f.var(v);
        if !matches!(var.kind, VariableKind::Local | VariableKind::StackSlot) {
            continue;
        }
        if !(var.remove_if_redundant || remove_dead_stores) {
            continue;
        }
        // Mono iterators capture these; their loads appear only with DelegateConstruction.
        if r.state_machine_compiled_with_mono && var.state_machine_field.is_some() {
            continue;
        }
        if var.load_count() != 0 || var.address_count() != 0 {
            continue;
        }
        for store in var.store_instructions.clone() {
            let Inst::StLoc { value, .. } = *f.inst(store) else {
                continue;
            };
            let Some(parent) = f.node(store).parent else {
                continue;
            };
            if !matches!(f.inst(parent), Inst::Block { .. }) {
                continue;
            }
            if is_pure(f.flags(value)) {
                f.remove(store);
            } else {
                f.replace(store, value);
            }
            if let Inst::LdLoc { var } = *f.inst(value) {
                queue.push_back(var);
            }
        }
    }
}

/// `ResetUsesInitialValueFlag`.
pub fn reset_uses_initial_value_flag(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let used = potentially_used_uninitialized(&r.function, ctx.ts, &r.variables);
    for (i, &v) in r.variables.iter().enumerate() {
        let var = r.function.var_mut(v);
        if var.kind != VariableKind::Parameter && !used.bit(i) {
            var.uses_initial_value = false;
        }
    }
}
