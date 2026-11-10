//! `IL/Transforms/FixRemainingIncrements.cs`: a user-defined `op_Increment` or `op_Decrement`
//! call that TransformAssignment left alone (Roslyn optimized the incremented variable out,
//! as in `Console.WriteLine(UserType.op_Increment(a + b))`) becomes a compound assignment to
//! a variable.

use crate::il::ast::{CompoundEvalMode, CompoundTargetKind, Function, Inst, NodeId};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;

use super::Context;
use super::il_extraction::extract;
use super::il_inlining::known;
use super::transform_assignment::is_increment_or_decrement;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let Some(body) = f.body else {
        return;
    };
    let mut calls_to_fix = Vec::new();
    for call in descendants_pre(f, body) {
        let Inst::Call(c) = f.inst(call) else {
            continue;
        };
        let method = c.method.method.clone();
        if !is_increment_or_decrement(ctx, &method) || c.args.len() != 1 {
            continue;
        }
        // For decimal, legacy csc can optimize `d + 1m` to `op_Increment(d)`;
        // ReplaceMethodCallsWithOperators handles these calls.
        if method
            .declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Decimal))
        {
            continue;
        }
        calls_to_fix.push(call);
    }
    for call in calls_to_fix {
        fix(f, ctx, call);
    }
}

fn fix(f: &mut Function, ctx: &mut Context<'_, '_, '_>, call: NodeId) {
    let Inst::Call(c) = f.inst(call) else {
        return;
    };
    let (operand, arg) = (c.method.clone(), c.args[0]);
    let range = f.il_range(call);
    let store_in_block = f.node(call).parent.and_then(|store| {
        let Inst::StLoc { var, value, .. } = *f.inst(store) else {
            return None;
        };
        let block = f.node(store).parent?;
        let Inst::Block { instructions, .. } = f.inst(block) else {
            return None;
        };
        let index = f.node(store).child_index as usize;
        (value == call && index < instructions.len()).then_some((var, block, index))
    });
    let (variable, replaced) = if let Some((var, _, _)) = store_in_block {
        // stloc V(call op_Increment(...)) -> stloc V(...); compound.assign op_Increment(V)
        f.replace(call, arg);
        (var, None)
    } else {
        let Some(var) = extract(f, ctx, arg) else {
            // The managed code fails a Debug assertion here.
            return;
        };
        let Ok(params) = operand.method.params() else {
            return;
        };
        f.var_mut(var).ty = params[0];
        (var, Some(call))
    };
    let target = f.add(Inst::LdLoca { var: variable });
    let one = f.add(Inst::LdcI4 { value: 1 });
    let compound_assign = f.add(Inst::UserDefinedCompoundAssign {
        method: operand,
        eval_mode: CompoundEvalMode::EvaluatesToNewValue,
        target_kind: CompoundTargetKind::Address,
        target,
        value: one,
    });
    f.set_il_range(compound_assign, range);
    match (replaced, store_in_block) {
        (Some(call), _) => f.replace(call, compound_assign),
        (None, Some((_, block, index))) => f.insert(block, index + 1, compound_assign),
        (None, None) => unreachable!("either the store or the call is replaced"),
    }
}

/// `Descendants`: the nodes under `root` (inclusive) in pre-order.
fn descendants_pre(f: &Function, root: NodeId) -> Vec<NodeId> {
    let mut out = Vec::new();
    let mut stack = vec![root];
    while let Some(id) = stack.pop() {
        out.push(id);
        stack.extend(f.inst(id).children().into_iter().rev());
    }
    out
}
