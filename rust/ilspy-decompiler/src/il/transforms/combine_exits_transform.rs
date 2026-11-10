//! `IL/Transforms/CombineExitsTransform.cs`: in a single-block function,
//! `if (cond) leave (a); leave (b)` becomes `leave (if (cond) a else b)`. DelegateConstruction
//! runs it on the lambdas it builds.

use crate::il::ast::{Function, Inst, NodeId};
use crate::il::reader::ReadFunction;

use super::Context;
use super::expression_transforms::run_on_single_statement;
use super::null_coalescing_transform::unwrap_block;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let Some(container) = f.body else {
        return;
    };
    let Inst::BlockContainer { blocks, .. } = f.inst(container) else {
        return;
    };
    let &[block] = &blocks[..] else {
        return;
    };
    if let Some(combined) = combine_exits(f, block) {
        run_on_single_statement(f, ctx, combined);
    }
}

/// `CombineExits`: the combined `leave`, or `None` when the block does not end in the pattern.
fn combine_exits(f: &mut Function, block: NodeId) -> Option<NodeId> {
    let list = f.list(block);
    let &[.., if_inst, leave_else] = list else {
        return None;
    };
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return None;
    };
    let (_, else_value) = f.match_leave(leave_else)?;
    if !f.match_nop(false_inst) {
        return None;
    }
    // A true branch of one instruction, or a two-instruction block combined first:
    // if (cond) { if (cond2) leave (a); leave (b) } leave (c)
    // => leave (if (cond) (if (cond2) a else b) else c)
    let mut true_instruction = unwrap_block(f, true_inst);
    if let Inst::Block { instructions, .. } = f.inst(true_instruction)
        && instructions.len() == 2
    {
        true_instruction = combine_exits(f, true_instruction)?;
    }
    let (target, value) = f.match_leave(true_instruction)?;
    let (else_target, _) = f.match_leave(leave_else)?;
    if Some(target) != f.body || Some(else_target) != f.body {
        return None;
    }
    if f.match_nop(value) || f.match_nop(else_value) {
        return None;
    }
    let Inst::BlockContainer { expected, .. } = *f.inst(target) else {
        return None;
    };
    let if_range = f.il_range(if_inst);
    let (leave_range, else_range) = (f.il_range(true_instruction), f.il_range(leave_else));
    let combined_value = f.add(Inst::If {
        condition,
        true_inst: value,
        false_inst: else_value,
        csharp_type: expected,
    });
    f.add_il_range(combined_value, if_range);
    let combined = f.add(Inst::Leave {
        target: Some(target),
        value: combined_value,
    });
    f.add_il_range(combined, else_range);
    f.add_il_range(combined, leave_range);
    let index = f.node(if_inst).child_index as usize;
    f.replace(if_inst, combined);
    f.remove_at(block, index + 1);
    Some(combined)
}
