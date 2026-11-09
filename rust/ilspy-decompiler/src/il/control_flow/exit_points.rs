//! `IL/ControlFlow/ExitPoints.cs` (`DetectExitPoints`): jumps out of a container to the
//! instruction that follows it become `leave` of the container, so loops and blocks get their
//! natural exits.

use crate::il::ast::{ContainerKind, Flags, Function, Inst, NodeId, NopKind};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;

/// What follows a container (`GetExit`).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Exit {
    /// The container ends its block: the exit is found from the jumps out of it.
    NotYetDetermined,
    /// The container is in no slot with a following instruction.
    None,
    Inst(NodeId),
}

/// `GetExit`: the instruction after `inst` in its block, looking through try blocks, catch
/// handlers and pinned regions.
pub fn get_exit(f: &Function, inst: NodeId) -> Exit {
    let Some(parent) = f.node(inst).parent else {
        return Exit::None;
    };
    let index = f.node(inst).child_index as usize;
    match f.inst(parent) {
        Inst::Block { instructions, .. } if index < instructions.len() => {
            match instructions.get(index + 1) {
                Some(&next) => Exit::Inst(next),
                None => Exit::NotYetDetermined,
            }
        }
        // TryBlockSlot, TryCatch.HandlerSlot, TryCatchHandler.BodySlot, PinnedRegion.BodySlot,
        // LockInstruction.BodySlot.
        Inst::TryCatch { .. } => get_exit(f, parent),
        Inst::TryFinally { try_block, .. } | Inst::TryFault { try_block, .. }
            if *try_block == inst =>
        {
            get_exit(f, parent)
        }
        Inst::TryCatchHandler { body, .. } if *body == inst => get_exit(f, parent),
        Inst::PinnedRegion { body, .. } if *body == inst => get_exit(f, parent),
        Inst::Lock { body, .. } if *body == inst => get_exit(f, parent),
        _ => Exit::None,
    }
}

/// `CompatibleExitInstruction`: branches to the same block, or value-less leaves of the same
/// container.
pub fn compatible_exit_instruction(f: &Function, exit1: NodeId, exit2: Exit) -> bool {
    let Exit::Inst(exit2) = exit2 else {
        return false;
    };
    match (f.inst(exit1), f.inst(exit2)) {
        (Inst::Branch { .. }, Inst::Branch { .. }) => {
            f.branch_target(exit1) == f.branch_target(exit2)
        }
        (
            Inst::Leave {
                target: t1,
                value: v1,
            },
            Inst::Leave {
                target: t2,
                value: v2,
            },
        ) => t1 == t2 && f.match_nop(*v1) && f.match_nop(*v2),
        _ => false,
    }
}

struct ContainerContext {
    container: NodeId,
    current_exit: Exit,
    /// When the exit is not yet determined: the jumps out of the container.
    potential_exits: Option<Vec<NodeId>>,
}

pub fn run(r: &mut ReadFunction, _ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let mut d = DetectExitPoints {
        blocks_potentially_made_unreachable: Vec::new(),
        container_stack: Vec::new(),
    };
    let body = f.body.expect("body");
    d.visit(f, body);
    for block in d.blocks_potentially_made_unreachable {
        let unreachable = f.node(block).incoming_edge_count == 0
            || (f.node(block).incoming_edge_count == 1 && is_infinite_loop(f, block));
        // A block already taken out keeps a stale parent: only a block still in its container
        // goes.
        let in_container = f
            .node(block)
            .parent
            .is_some_and(|c| f.list(c).get(f.node(block).child_index as usize) == Some(&block));
        if unreachable && in_container {
            f.remove_block(block);
        }
    }
}

fn is_infinite_loop(f: &Function, block: NodeId) -> bool {
    matches!(*f.list(block), [b] if f.match_branch(b) == Some(block))
}

fn leave(f: &mut Function, container: NodeId, range_of: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let l = f.add(Inst::Leave {
        target: Some(container),
        value: nop,
    });
    f.add_il_range(l, f.il_range(range_of));
    l
}

struct DetectExitPoints {
    blocks_potentially_made_unreachable: Vec<NodeId>,
    /// Innermost last.
    container_stack: Vec<ContainerContext>,
}

impl DetectExitPoints {
    /// `ContainerContext.HandleExit` of the stack entry `entry`.
    fn handle_exit(&mut self, f: &mut Function, entry: usize, inst: NodeId) {
        let e = &mut self.container_stack[entry];
        if e.current_exit == Exit::NotYetDetermined && f.leave_count(e.container) == 0 {
            e.potential_exits
                .as_mut()
                .expect("potential exits")
                .push(inst);
        } else if compatible_exit_instruction(f, inst, e.current_exit) {
            // shortcut: an exit an inner container already replaced is stale here; the managed code replaces its old slot, this skips it
            if f.is_connected(inst) {
                let container = e.container;
                let l = leave(f, container, inst);
                f.replace(inst, l);
            }
        }
    }

    fn visit(&mut self, f: &mut Function, inst: NodeId) {
        match *f.inst(inst) {
            Inst::BlockContainer { .. } => self.visit_block_container(f, inst),
            Inst::Block { .. } => {
                // The instructions only (not the final instruction), as the list grows.
                let mut i = 0;
                while let Some(&child) = f.list(inst).get(i) {
                    self.visit(f, child);
                    i += 1;
                }
            }
            Inst::Branch { .. } => {
                let target = f.branch_target(inst);
                for entry in (0..self.container_stack.len()).rev() {
                    let container = self.container_stack[entry].container;
                    if target == container || f.ancestors(target).any(|a| a == container) {
                        break;
                    }
                    self.handle_exit(f, entry, inst);
                }
            }
            Inst::Leave {
                target: Some(target),
                value,
            } => {
                self.visit(f, value);
                if !f.match_nop(value) {
                    return;
                }
                let leaving_function = Some(target) == f.body;
                let target_normal = matches!(
                    f.inst(target),
                    Inst::BlockContainer {
                        kind: ContainerKind::Normal,
                        ..
                    }
                );
                for entry in (0..self.container_stack.len()).rev() {
                    let container = self.container_stack[entry].container;
                    if target == container {
                        break;
                    }
                    if leaving_function || !target_normal {
                        let entry_normal = matches!(
                            f.inst(container),
                            Inst::BlockContainer {
                                kind: ContainerKind::Normal,
                                ..
                            }
                        );
                        if !entry_normal {
                            // Only for the innermost loop or switch.
                            self.handle_exit(f, entry, inst);
                            break;
                        }
                    } else {
                        self.handle_exit(f, entry, inst);
                    }
                }
            }
            _ => {
                let mut i = 0;
                while let Some(&child) = f.inst(inst).children().get(i) {
                    self.visit(f, child);
                    i += 1;
                }
            }
        }
    }

    fn visit_block_container(&mut self, f: &mut Function, container: NodeId) {
        let this_exit = get_exit(f, container);
        self.container_stack.push(ContainerContext {
            container,
            current_exit: this_exit,
            potential_exits: (this_exit == Exit::NotYetDetermined).then(Vec::new),
        });
        let depth = self.container_stack.len();
        let mut i = 0;
        while let Some(&block) = f.list(container).get(i) {
            self.visit(f, block);
            i += 1;
        }
        let entry = self.container_stack.pop().expect("container stack");
        debug_assert_eq!(depth, self.container_stack.len() + 1);
        let Some(potential) = entry.potential_exits else {
            return;
        };
        let connected: Vec<NodeId> = potential
            .iter()
            .copied()
            .filter(|&e| f.is_connected(e))
            .collect();
        let Some(new_exit) = choose_exit(f, &connected) else {
            return;
        };
        for &exit in &potential {
            if f.is_connected(exit) && compatible_exit_instruction(f, new_exit, Exit::Inst(exit)) {
                let l = leave(f, container, exit);
                f.replace(exit, l);
            }
        }
        let mut inst = container;
        let block = loop {
            let parent = f.node(inst).parent.expect("container in a block");
            if matches!(f.inst(parent), Inst::Block { .. }) {
                break parent;
            }
            inst = parent;
        };
        if f.has_flag(block, Flags::END_POINT_UNREACHABLE) {
            if matches!(f.inst(new_exit), Inst::Branch { .. }) {
                self.blocks_potentially_made_unreachable
                    .push(f.branch_target(new_exit));
            }
        } else {
            f.push(block, new_exit);
        }
    }
}

/// `ChooseExit`: the first exit, preferring one that does not leave the function.
fn choose_exit(f: &Function, exits: &[NodeId]) -> Option<NodeId> {
    let &first = exits.first()?;
    let leaves_function =
        |e: NodeId| matches!(*f.inst(e), Inst::Leave { target, .. } if target == f.body);
    if leaves_function(first)
        && let Some(&other) = exits.iter().skip(1).find(|&&e| !leaves_function(e))
    {
        return Some(other);
    }
    Some(first)
}
