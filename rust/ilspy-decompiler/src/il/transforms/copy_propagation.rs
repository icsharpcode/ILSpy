//! `IL/Transforms/CopyPropagation.cs`: a very simple copy propagation. A stack slot or ref local
//! stored once from an address load, a single-definition variable or parameter, or a
//! flag-free leaf is replaced by copies of that value; a dead store to a stack slot is removed.

use std::collections::HashSet;

use crate::il::ast::{BlockKind, Flags, Function, Inst, NodeId, VarId, Variable, VariableKind};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::types::StackType;
use crate::typesystem::known::KnownTypeCode;
use crate::util::interval::Interval;

use super::Context;
use super::il_inlining::{inline_into, known, options};
use super::transform_assignment::{is_small_integer_type, same_il_variable};

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    // Variables of which the function lists several parts (`ILVariableEqualityComparer`).
    let split: HashSet<VarId> = r
        .variables
        .iter()
        .filter(|&&v| {
            r.variables
                .iter()
                .any(|&w| w != v && same_il_variable(&r.function, v, w))
        })
        .copied()
        .collect();
    let Some(body) = r.function.body else {
        return;
    };
    for block in r.function.descendants_post(body) {
        if !matches!(
            r.function.inst(block),
            Inst::Block {
                kind: BlockKind::ControlFlow,
                ..
            }
        ) {
            continue;
        }
        // New variables go to the function the block belongs to (`v.Function.Variables`).
        match enclosing_lambda(&r.function, block) {
            Some(lambda) => {
                let mut variables = match r.function.inst_mut(lambda) {
                    Inst::Function { variables, .. } => std::mem::take(variables),
                    _ => unreachable!("an enclosing lambda is a function"),
                };
                run_on_block(&mut r.function, &mut variables, ctx, block, &split);
                if let Inst::Function { variables: v, .. } = r.function.inst_mut(lambda) {
                    *v = variables;
                }
            }
            None => run_on_block(&mut r.function, &mut r.variables, ctx, block, &split),
        }
    }
}

/// The nearest `Function` instruction above `node`, if any.
fn enclosing_lambda(f: &Function, node: NodeId) -> Option<NodeId> {
    let mut current = f.node(node).parent;
    while let Some(p) = current {
        if matches!(f.inst(p), Inst::Function { .. }) {
            return Some(p);
        }
        current = f.node(p).parent;
    }
    None
}

/// `RunOnBlock`.
fn run_on_block(
    f: &mut Function,
    variables: &mut Vec<VarId>,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    split: &HashSet<VarId>,
) {
    let mut i = 0usize;
    while i < f.list(block).len() {
        let store = f.list(block)[i];
        if let Some((v, copied)) = f.match_stloc(store) {
            let var = f.var(v);
            if var.is_single_definition()
                && var.load_count() == 0
                && var.kind == VariableKind::StackSlot
            {
                // A dead store to a stack slot.
                if is_pure(f.flags(copied)) {
                    f.remove_at(block, i);
                    // This can open up new inlining opportunities.
                    let c = inline_into(f, block, i, options::NONE, ctx);
                    i = i.wrapping_sub(c + 1);
                } else {
                    // The value is evaluated for its side effects.
                    let range = f.il_range(store);
                    f.add_il_range(copied, range);
                    f.set_child_at(block, i, copied);
                }
            } else if var.is_single_definition()
                && can_perform_copy_propagation(f, ctx, v, copied, split)
            {
                do_propagate(f, variables, ctx, v, copied, block, &mut i);
            }
        }
        i = i.wrapping_add(1);
    }
}

/// `CanPerformCopyPropagation`.
fn can_perform_copy_propagation(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    target: VarId,
    value: NodeId,
    split: &HashSet<VarId>,
) -> bool {
    let t = f.var(target);
    if is_small_integer_type(ctx, t.ty) {
        return false;
    }
    // A non-local code move might change semantics when there are split variables.
    if split.contains(&target) {
        return false;
    }
    match *f.inst(value) {
        // Address loads always return the same value for the same operands, so they can be
        // copied, except that ldelema and ldflda might throw.
        Inst::LdLoca { .. } | Inst::LdsFlda { .. } => true,
        Inst::LdElema { .. } | Inst::LdFlda { .. } => {
            !ctx.reader.use_ref_locals_for_accurate_order_of_evaluation
        }
        Inst::LdLoc { var } => {
            if split.contains(&var) {
                return false;
            }
            let source = f.var(var);
            let mut is_bool = |ty| known(ctx, ty) == Some(KnownTypeCode::Boolean);
            if is_bool(source.ty) != is_bool(t.ty) {
                return false;
            }
            match source.kind {
                // The caller's initialization is the first store, so a parameter must not be
                // assigned to (directly or through ldarga).
                VariableKind::Parameter => source.is_single_definition(),
                // To avoid removing too many variables, only into a stack slot or ref local.
                _ => {
                    source.is_single_definition()
                        && (t.kind == VariableKind::StackSlot || t.stack_type == StackType::Ref)
                }
            }
        }
        // Any instruction without special behavior can be copied into a stack slot.
        _ => {
            f.flags(value) == Flags::NONE
                && f.inst(value).children().is_empty()
                && t.kind == VariableKind::StackSlot
        }
    }
}

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
            capture_scope: None,
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
