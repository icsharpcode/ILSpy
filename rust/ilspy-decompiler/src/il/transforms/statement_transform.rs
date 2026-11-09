//! `IL/Transforms/StatementTransform.cs`: per-statement transforms that depend on each
//! other run interleaved, statement by statement from the end of the block, and a transform
//! can ask for a position to be run again.

use crate::il::ast::NodeId;
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;

/// `StatementTransformContext`: the rerun requests of the transforms.
#[derive(Default)]
pub struct StatementTransformContext {
    rerun_current_position: bool,
    rerun_position: Option<usize>,
}

impl StatementTransformContext {
    /// `RequestRerun(pos)`: run the transforms again from `pos` (the highest request wins).
    pub fn request_rerun_at(&mut self, pos: usize) {
        if self.rerun_position.is_none_or(|p| pos > p) {
            self.rerun_position = Some(pos);
        }
    }

    /// `RequestRerun()`: run the transforms again at the current position.
    pub fn request_rerun(&mut self) {
        self.rerun_current_position = true;
    }
}

/// `IStatementTransform.Run(block, pos, context)`.
pub type StatementTransform = fn(
    &mut ReadFunction,
    &mut Context<'_, '_, '_>,
    &mut BlockTransformContext,
    &mut StatementTransformContext,
    NodeId,
    usize,
);

/// `StatementTransform.Run`: `children` on each position from the last one not yet
/// transformed down to 0, repeating positions on request.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    bc: &mut BlockTransformContext,
    children: &[StatementTransform],
) {
    let block = bc.block;
    if bc.index_of_first_already_transformed_instruction == 0 {
        return;
    }
    let mut sc = StatementTransformContext {
        rerun_current_position: false,
        rerun_position: Some(bc.index_of_first_already_transformed_instruction - 1),
    };
    let mut pos: isize = 0;
    while pos >= 0 {
        if let Some(rerun) = sc.rerun_position.take() {
            // A transform that removes instructions leaves the block context's index stale;
            // the managed driver then reads past the end of the block and throws.
            assert!(
                rerun < r.function.list(block).len(),
                "ArgumentOutOfRangeException: statement position {rerun} past the end of the block"
            );
            pos = rerun as isize;
        }
        for transform in children {
            transform(r, ctx, bc, &mut sc, block, pos as usize);
            r.apply_variable_changes(&mut ctx.removed_variables);
            // A request from a transform that runs without the statement context.
            if std::mem::take(&mut ctx.rerun_requested) {
                sc.request_rerun();
            }
            if sc.rerun_current_position {
                sc.rerun_current_position = false;
                sc.request_rerun_at(pos as usize);
            }
            if sc.rerun_position.is_some() {
                break;
            }
        }
        if sc.rerun_position.is_none() {
            pos -= 1;
        }
    }
}
