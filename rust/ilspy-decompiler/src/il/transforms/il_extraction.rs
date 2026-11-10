//! `IL/Transforms/ILExtraction.cs` (`ILInstruction.Extract`): the opposite of inlining. An
//! instruction moves out into a store to a new stack slot in front of its statement, leaving
//! a load of the slot behind; predecessors whose evaluation order would change move too.

use crate::il::ast::VariableKind;
use crate::il::ast::{BlockKind, ContainerKind, Flags, Function, Inst, NodeId, NopKind, VarId};
use crate::il::types::StackType;

use super::Context;

/// `ExtractionContext`.
struct ExtractionContext {
    /// `FlagsBeingMoved`: the combined flags of all instructions being moved.
    flags_being_moved: Flags,
    /// `MoveActions`: the predecessors that move as well, in the order they were registered.
    moves: Vec<NodeId>,
}

impl ExtractionContext {
    /// `RegisterMoveIfNecessary`: a predecessor with any flags cannot be reordered with the
    /// instructions being moved, so it moves as well.
    fn register_move_if_necessary(&mut self, f: &Function, predecessor: NodeId) {
        let flags = f.flags(predecessor);
        if flags != Flags::NONE {
            self.flags_being_moved |= flags;
            self.moves.push(predecessor);
        }
    }
}

/// `PrepareExtract`: whether `inst_to_extract` may move out of child slot `index` of
/// `parent`, registering the predecessors that have to move with it.
fn prepare_extract(
    f: &Function,
    ctx: &mut ExtractionContext,
    parent: NodeId,
    index: usize,
) -> bool {
    match f.inst(parent) {
        // Out of the entry point, unless that moves code out of a loop.
        Inst::BlockContainer { .. } => {
            index == 0
                && f.entry_point(parent)
                    .is_some_and(|e| f.node(e).incoming_edge_count == 1)
        }
        Inst::NullableRewrap { .. } => {
            prepare_extract_default(f, ctx, parent, index)
                && ctx.flags_being_moved.0 & Flags::MAY_UNWRAP_NULL.0 == 0
        }
        _ => prepare_extract_default(f, ctx, parent, index),
    }
}

/// `ILInstruction.PrepareExtract`: the slot allows inlining, and so do the slots of all
/// predecessors, which are evaluated before.
fn prepare_extract_default(
    f: &Function,
    ctx: &mut ExtractionContext,
    parent: NodeId,
    index: usize,
) -> bool {
    if !f.slot_can_inline_into(parent, index) {
        return false;
    }
    let children = f.inst(parent).children();
    for i in (0..index).rev() {
        if !f.slot_can_inline_into(parent, i) {
            return false;
        }
        ctx.register_move_if_necessary(f, children[i]);
    }
    true
}

/// Puts `inst` into a new store to a new stack slot at `index` of `block`, leaving a load of
/// the slot in its place.
fn move_to_store(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    block: NodeId,
    index: usize,
    owner: Option<NodeId>,
) -> VarId {
    let ty = f.infer_type(ctx.ts, inst);
    let v = f.register_variable_in(ctx.ts, owner, VariableKind::StackSlot, ty, None);
    let load = f.add(Inst::LdLoc { var: v });
    f.replace(inst, load);
    let store = f.add(Inst::StLoc {
        var: v,
        value: inst,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.insert(block, index, store);
    v
}

/// `ExtractionContext.Extract`: moves `inst_to_extract` out to a store in front of its
/// statement and returns the new variable, or `None` when extraction is not possible.
pub(crate) fn extract(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst_to_extract: NodeId,
) -> Option<VarId> {
    // The variables belong to the function holding the instruction.
    let owner = f.enclosing_function(inst_to_extract);
    let mut ec = ExtractionContext {
        flags_being_moved: f.flags(inst_to_extract),
        moves: Vec::new(),
    };
    let mut inst = inst_to_extract;
    loop {
        let parent = f.node(inst).parent?;
        if let Inst::If { .. } = f.inst(parent)
            && f.node(inst).child_index != 0
            && f.result_type(ctx.ts, parent) == StackType::Void
        {
            // An if branch cannot take a store, but a block in its place can.
            let final_inst = f.add(Inst::Nop {
                kind: NopKind::Normal,
            });
            let new_block = f.add(Inst::Block {
                kind: BlockKind::ControlFlow,
                instructions: Vec::new(),
                final_inst,
            });
            f.replace(inst, new_block);
            f.push(new_block, inst);
        }
        let parent = f.node(inst).parent?;
        if let Inst::Block {
            kind: BlockKind::ControlFlow,
            ..
        } = *f.inst(parent)
        {
            let block = parent;
            // A target block: extraction is possible all the way, if the container allows it.
            if let Some(container) = f.node(block).parent
                && let Inst::BlockContainer {
                    kind, ref blocks, ..
                } = *f.inst(container)
            {
                let entry = f.entry_point(container);
                let last = blocks.last().copied();
                match kind {
                    ContainerKind::Normal | ContainerKind::Loop => {}
                    // Not out of the switch head; its container's block may take it.
                    ContainerKind::Switch => {
                        if Some(block) == entry && f.node(inst).child_index == 0 {
                            inst = container;
                            continue;
                        }
                    }
                    // Not out of the condition block.
                    ContainerKind::While => {
                        if Some(block) == entry {
                            return None;
                        }
                    }
                    ContainerKind::DoWhile => {
                        if Some(block) == last {
                            return None;
                        }
                    }
                    // Not out of the condition or the increment block.
                    ContainerKind::For => {
                        if Some(block) == entry || Some(block) == last {
                            return None;
                        }
                    }
                }
            }
            let insert_index = f.node(inst).child_index as usize;
            let v = move_to_store(f, ctx, inst_to_extract, block, insert_index, owner);
            for predecessor in ec.moves {
                move_to_store(f, ctx, predecessor, block, insert_index, owner);
            }
            return Some(v);
        }
        let index = f.node(inst).child_index as usize;
        if !prepare_extract(f, &mut ec, parent, index) {
            return None;
        }
        inst = parent;
    }
}
