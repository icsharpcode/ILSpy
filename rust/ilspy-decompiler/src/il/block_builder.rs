//! `IL/BlockBuilder.cs`: puts the basic blocks of the IL reader into the container structure
//! of the exception regions and connects the branches.

use std::collections::HashMap;

use crate::metadata::il_body::ExceptionClause;
use crate::metadata::{Error, Result};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeId, TypeSystem};
use crate::util::interval::{Interval, LongInterval};
use crate::util::long_set::LongSet;

use super::ast::*;
use super::reader::cast;
use super::types::StackType;

/// `DisassemblerHelpers.OffsetToString`.
fn offset_to_string(offset: i32) -> String {
    format!("IL_{offset:04x}")
}

struct OnErrorDispatch {
    variable: VarId,
    target_offsets: Vec<i32>,
    branches: Vec<NodeId>,
}

pub(super) struct BlockBuilder<'f> {
    f: &'f mut Function,
    try_instructions: Vec<NodeId>,
    handler_containers: HashMap<i32, NodeId>,
    container_stack: Vec<NodeId>,
    int32: TypeId,
    /// In creation order, keyed by the try-catch.
    on_error_dispatchers: Vec<(NodeId, OnErrorDispatch)>,
}

fn container(f: &mut Function, range: Interval) -> NodeId {
    let c = f.add(Inst::BlockContainer {
        kind: ContainerKind::Normal,
        expected: None,
        blocks: Vec::new(),
    });
    f.add_il_range(c, range);
    c
}

fn new_block(f: &mut Function) -> NodeId {
    let final_inst = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Block {
        kind: BlockKind::ControlFlow,
        instructions: Vec::new(),
        final_inst,
    })
}

fn try_block(f: &Function, try_inst: NodeId) -> NodeId {
    match f.inst(try_inst) {
        Inst::TryCatch { try_block, .. }
        | Inst::TryFinally { try_block, .. }
        | Inst::TryFault { try_block, .. } => *try_block,
        other => panic!("{other:?} is not a try instruction"),
    }
}

impl<'f> BlockBuilder<'f> {
    pub(super) fn new(f: &'f mut Function, int32: TypeId) -> Self {
        BlockBuilder {
            f,
            try_instructions: Vec::new(),
            handler_containers: HashMap::new(),
            container_stack: Vec::new(),
            int32,
            on_error_dispatchers: Vec::new(),
        }
    }

    fn create_container_structure(
        &mut self,
        ts: &mut TypeSystem<'_, '_>,
        clauses: &[ExceptionClause],
        variable_by_handler: &[Option<VarId>],
    ) -> Result<()> {
        let mut try_catches: Vec<NodeId> = Vec::new();
        for (i, eh) in clauses.iter().enumerate() {
            let try_range =
                Interval::new(eh.try_offset as i32, (eh.try_offset + eh.try_length) as i32);
            let handler_range = Interval::new(
                eh.handler_offset as i32,
                (eh.handler_offset + eh.handler_length) as i32,
            );
            let handler_block = container(self.f, handler_range);
            self.add_handler_container(handler_block)?;
            if eh.kind & 6 != 0 {
                let try_container = container(self.f, try_range);
                let inst = self.f.add(if eh.kind & 2 != 0 {
                    Inst::TryFinally {
                        try_block: try_container,
                        finally_block: handler_block,
                    }
                } else {
                    Inst::TryFault {
                        try_block: try_container,
                        fault_block: handler_block,
                    }
                });
                self.f.set_il_range(inst, try_range);
                self.try_instructions.push(inst);
                continue;
            }
            let existing = try_catches
                .iter()
                .copied()
                .find(|&tc| self.f.il_range(try_block(self.f, tc)) == try_range);
            let try_catch = match existing {
                Some(tc) => tc,
                None => {
                    let try_container = container(self.f, try_range);
                    let tc = self.f.add(Inst::TryCatch {
                        try_block: try_container,
                        handlers: Vec::new(),
                    });
                    self.f.add_il_range(tc, try_range);
                    try_catches.push(tc);
                    self.try_instructions.push(tc);
                    tc
                }
            };
            let filter = if eh.kind & 1 != 0 {
                let int32 = ts.known_type(KnownTypeCode::Int32);
                let filter_block = self.f.add(Inst::BlockContainer {
                    kind: ContainerKind::Normal,
                    expected: Some(int32),
                    blocks: Vec::new(),
                });
                self.f.add_il_range(
                    filter_block,
                    Interval::new(eh.class_token_or_filter as i32, eh.handler_offset as i32),
                );
                self.add_handler_container(filter_block)?;
                filter_block
            } else {
                self.f.add(Inst::LdcI4 { value: 1 })
            };
            let variable = variable_by_handler[i].ok_or(Error::Internal(
                "The given key was not present in the dictionary.",
            ))?;
            let handler = self.f.add(Inst::TryCatchHandler {
                filter,
                body: handler_block,
                variable,
            });
            let (filter_range, handler_range) =
                (self.f.il_range(filter), self.f.il_range(handler_block));
            self.f.add_il_range(handler, filter_range);
            self.f.add_il_range(handler, handler_range);
            self.f.push(try_catch, handler);
            let range = self.f.il_range(handler);
            self.f.add_il_range(try_catch, range);
        }
        // OrderBy try start, then by descending try end; both sorts are stable.
        let f = &*self.f;
        self.try_instructions.sort_by(|&a, &b| {
            let (ra, rb) = (f.il_range(try_block(f, a)), f.il_range(try_block(f, b)));
            ra.start.cmp(&rb.start).then(rb.end.cmp(&ra.end))
        });
        Ok(())
    }

    fn add_handler_container(&mut self, c: NodeId) -> Result<()> {
        let start = self.f.il_range(c).start;
        if self.handler_containers.insert(start, c).is_some() {
            return Err(Error::Internal(
                "An item with the same key has already been added.",
            ));
        }
        Ok(())
    }

    /// `CreateBlocks`: `blocks` are the reader's basic blocks in any order.
    pub(super) fn create_blocks(
        &mut self,
        ts: &mut TypeSystem<'_, '_>,
        clauses: &[ExceptionClause],
        variable_by_handler: &[Option<VarId>],
        main_container: NodeId,
        code_size: i32,
        blocks: &[NodeId],
    ) -> Result<Vec<VarId>> {
        self.create_container_structure(ts, clauses, variable_by_handler)?;
        self.f
            .set_il_range(main_container, Interval::new(0, code_size));
        let mut blocks = blocks.to_vec();
        blocks.sort_by_key(|&b| self.f.il_range(b).start);
        let mut current = main_container;
        let mut next_try_index = 0;
        for block in blocks {
            let start = self.f.il_range(block).start;
            while start >= self.f.il_range(current).end && !self.container_stack.is_empty() {
                current = self.container_stack.pop().expect("non-empty");
            }
            if let Some(&handler) = self.handler_containers.get(&start) {
                self.container_stack.push(current);
                current = handler;
            }
            while let Some(&next_try) = self.try_instructions.get(next_try_index)
                && start == self.f.il_range(try_block(self.f, next_try)).start
            {
                let block_for_try = new_block(self.f);
                let range = self.f.il_range(next_try);
                self.f.set_il_range(block_for_try, range);
                self.f.push(block_for_try, next_try);
                self.f.push(current, block_for_try);
                self.container_stack.push(current);
                current = try_block(self.f, next_try);
                next_try_index += 1;
            }
            self.f.push(current, block);
        }
        self.connect_branches(main_container, ts)?;
        Ok(self.create_on_error_dispatchers())
    }

    fn connect_branches(&mut self, inst: NodeId, ts: &mut TypeSystem<'_, '_>) -> Result<()> {
        match self.f.inst(inst).clone() {
            Inst::Branch {
                target: BranchTarget::Offset(offset),
            } => match self.find_branch_target(offset) {
                Some(block) => {
                    self.f.inst_mut(inst).clone_from(&Inst::Branch {
                        target: BranchTarget::Block(block),
                    });
                }
                None => {
                    let invalid = self.f.add(Inst::InvalidBranch {
                        message: format!(
                            "Could not find block for branch target {}",
                            offset_to_string(offset)
                        ),
                        expected: StackType::Void,
                    });
                    let range = self.f.il_range(inst);
                    self.f.set_il_range(invalid, range);
                    self.f.replace(inst, invalid);
                }
            },
            Inst::Branch { .. } => {}
            Inst::Leave {
                target: None,
                value,
            } => {
                let target = *self
                    .container_stack
                    .last()
                    .expect("leave inside a container");
                let expected = self.f.result_type(ts, target);
                let start = self.f.il_range(inst).start;
                let new_value = cast(self.f, ts, value, expected, None, start);
                if let Inst::Leave { target: t, .. } = self.f.inst_mut(inst) {
                    *t = Some(target);
                }
                if new_value != value {
                    self.f.replace(value, new_value);
                }
            }
            Inst::BlockContainer { blocks, .. } => {
                self.container_stack.push(inst);
                // Blocks appended while connecting (on-error jump targets) are not processed.
                for block in blocks {
                    self.connect_branches(block, ts)?;
                    let last = self.f.list(block).last().copied();
                    if last.is_none_or(|l| !self.f.has_flag(l, Flags::END_POINT_UNREACHABLE)) {
                        let invalid = self.f.add(Inst::InvalidBranch {
                            message: "Unexpected end of block".to_owned(),
                            expected: StackType::Void,
                        });
                        self.f.push(block, invalid);
                    }
                }
                self.container_stack.pop();
            }
            other => {
                for child in other.children() {
                    self.connect_branches(child, ts)?;
                }
            }
        }
        Ok(())
    }

    fn find_branch_target(&mut self, target: i32) -> Option<NodeId> {
        for i in (0..self.container_stack.len()).rev() {
            let c = self.container_stack[i];
            for &block in self.f.list(c) {
                let range = self.f.il_range(block);
                if range.start == target && !range.is_empty() {
                    return Some(block);
                }
            }
            let parent = self.f.node(c).parent;
            if let Some(handler) = parent
                && matches!(self.f.inst(handler), Inst::TryCatchHandler { body, .. } if *body == c)
            {
                let try_catch = self
                    .f
                    .node(handler)
                    .parent
                    .expect("handler inside a try-catch");
                let try_range = self.f.il_range(try_block(self.f, try_catch));
                if try_range.start < target && target < try_range.end {
                    return Some(self.create_branch_target_for_on_error_jump(try_catch, target));
                }
            }
        }
        None
    }

    fn create_branch_target_for_on_error_jump(&mut self, try_catch: NodeId, target: i32) -> NodeId {
        let index = match self
            .on_error_dispatchers
            .iter()
            .position(|(tc, _)| *tc == try_catch)
        {
            Some(i) => i,
            None => {
                let start = self.f.il_range(try_catch).start;
                let variable = self.f.add_var(Variable {
                    kind: VariableKind::Local,
                    ty: self.int32,
                    stack_type: StackType::I4,
                    index: None,
                    name: format!("try{start:04x}_dispatch"),
                    has_generated_name: false,
                    initial_value_is_initialized: false,
                    uses_initial_value: false,
                    is_ref_read_only: false,
                    load_count: 0,
                    store_instruction_count: 0,
                    address_count: 0,
                });
                self.on_error_dispatchers.push((
                    try_catch,
                    OnErrorDispatch {
                        variable,
                        target_offsets: Vec::new(),
                        branches: Vec::new(),
                    },
                ));
                self.on_error_dispatchers.len() - 1
            }
        };
        let variable = self.on_error_dispatchers[index].1.variable;
        if !self.on_error_dispatchers[index]
            .1
            .target_offsets
            .contains(&target)
        {
            self.on_error_dispatchers[index]
                .1
                .target_offsets
                .push(target);
        }
        let block = new_block(self.f);
        let value = self.f.add(Inst::LdcI4 { value: target });
        let store = self.f.add(Inst::StLoc {
            var: variable,
            value,
            il_stack_was_empty: false,
            is_stack_adjustment: false,
        });
        self.f.push(block, store);
        let try_start = self.f.il_range(try_block(self.f, try_catch)).start;
        let branch = self.f.add(Inst::Branch {
            target: BranchTarget::Offset(try_start),
        });
        self.f.push(block, branch);
        self.on_error_dispatchers[index].1.branches.push(branch);
        let top = *self.container_stack.last().expect("inside a container");
        self.f.push(top, block);
        block
    }

    /// Returns the dispatcher variables (`OnErrorDispatcherVariables`).
    fn create_on_error_dispatchers(&mut self) -> Vec<VarId> {
        let dispatchers = std::mem::take(&mut self.on_error_dispatchers);
        let variables = dispatchers.iter().map(|(_, d)| d.variable).collect();
        for (try_catch, dispatch) in dispatchers {
            let block = self
                .f
                .node(try_catch)
                .parent
                .expect("try-catch inside a block");
            let split_at = self.f.node(try_catch).child_index as usize;
            let minus_one = self.f.add(Inst::LdcI4 { value: -1 });
            let reset = self.f.add(Inst::StLoc {
                var: dispatch.variable,
                value: minus_one,
                il_stack_was_empty: false,
                is_stack_adjustment: false,
            });
            self.f.insert(block, split_at, reset);
            let split_at = self.f.node(try_catch).child_index as usize;
            let new = new_block(self.f);
            let range = self.f.il_range(try_catch);
            self.f.add_il_range(new, range);
            let moved = self.f.truncate(block, split_at);
            self.f.set_list(new, moved);
            let branch = self.f.add(Inst::Branch {
                target: BranchTarget::Block(new),
            });
            self.f.push(block, branch);
            let parent = self.f.node(block).parent.expect("block inside a container");
            self.f.push(parent, new);
            for b in dispatch.branches {
                if let Inst::Branch { target } = self.f.inst_mut(b) {
                    *target = BranchTarget::Block(new);
                }
            }

            let try_body = try_block(self.f, try_catch);
            let start = self.f.il_range(try_catch).start;
            let dispatch_block = new_block(self.f);
            self.f
                .add_il_range(dispatch_block, Interval::new(start, start + 1));
            let mut sections = Vec::new();
            for &offset in &dispatch.target_offsets {
                let target = self.f.list(try_body).iter().copied().find(|&b| {
                    let r = self.f.il_range(b);
                    r.start == offset && !r.is_empty()
                });
                let body = match target {
                    Some(t) => self.f.add(Inst::Branch {
                        target: BranchTarget::Block(t),
                    }),
                    None => self.f.add(Inst::InvalidBranch {
                        message: format!(
                            "Could not find block for branch target {}",
                            offset_to_string(offset)
                        ),
                        expected: StackType::Void,
                    }),
                };
                sections.push(self.f.add(Inst::SwitchSection {
                    labels: LongSet::single(i64::from(offset)),
                    body,
                }));
            }
            let used = LongSet::from_intervals(
                dispatch
                    .target_offsets
                    .iter()
                    .map(|&o| LongInterval::inclusive(i64::from(o), i64::from(o))),
            );
            let entry = self.f.list(try_body)[0];
            let body = self.f.add(Inst::Branch {
                target: BranchTarget::Block(entry),
            });
            sections.push(self.f.add(Inst::SwitchSection {
                labels: used.invert(),
                body,
            }));
            let value = self.f.add(Inst::LdLoc {
                var: dispatch.variable,
            });
            let switch = self.f.add(Inst::Switch {
                value,
                sections,
                ty: None,
            });
            self.f.add_il_range(switch, Interval::new(start, start + 1));
            let note = self.f.add(Inst::InvalidExpression {
                message: "ILSpy has introduced the following switch to emulate a goto from catch-block to try-block".to_owned(),
                expected: StackType::Unknown,
                severity: "Note".to_owned(),
            });
            self.f.push(dispatch_block, note);
            self.f.push(dispatch_block, switch);
            self.f.insert(try_body, 0, dispatch_block);
        }
        variables
    }
}

/// `BlockContainer.TopologicalSort(deleteUnreachableBlocks: true)`.
pub(super) fn topological_sort(f: &Function, container: NodeId) -> Vec<NodeId> {
    let blocks = f.list(container);
    let Some(&entry) = blocks.first() else {
        return Vec::new();
    };
    let mut visited = vec![false; blocks.len()];
    let mut post_order = Vec::new();
    let mut worklist = vec![(entry, false)];
    while let Some(&(node, is_post)) = worklist.last() {
        if is_post {
            post_order.push(node);
            worklist.pop();
            continue;
        }
        let index = f.node(node).child_index as usize;
        if visited[index] {
            worklist.pop();
            continue;
        }
        visited[index] = true;
        *worklist.last_mut().expect("non-empty") = (node, true);
        for d in f.descendants_post(node) {
            if let Inst::Branch {
                target: BranchTarget::Block(t),
            } = f.inst(d)
                && f.node(*t).parent == Some(container)
            {
                worklist.push((*t, false));
            }
        }
    }
    post_order.reverse();
    post_order
}
