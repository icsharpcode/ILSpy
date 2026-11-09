//! `FlowAnalysis/DataFlowVisitor.cs`: a forward data flow analysis over the ILAst, generic
//! in its state and in what loads and stores do to it.
//!
//! The managed states are classes (or structs around a `BitSet`), so the visitor shares
//! state objects by reference in a few places; the port clones where the managed code
//! clones and moves where it only renames, which keeps every observable join the same.

use std::collections::{BTreeSet, HashMap};

use crate::il::ast::{BranchTarget, Flags, Function, Inst, NodeId, VarId};

/// `IDataFlowState<Self>`: a join semi-lattice with a bottom element.
pub trait State: Clone {
    fn less_than_or_equal(&self, other: &Self) -> bool;
    fn replace_with(&mut self, other: &Self);
    fn join_with(&mut self, incoming: &Self);
    /// The state after a `finally` block ran with `finally_state` at its end, for a branch
    /// that was in this state when it entered the `finally`.
    fn trigger_finally(&mut self, finally_state: &Self);
    fn is_bottom(&self) -> bool;
    fn replace_with_bottom(&mut self);
}

/// What an analysis does at the instructions that touch variables.
pub trait Analysis {
    type S: State;
    /// A store to `var` (`stloc` after its value, or a catch handler's variable).
    fn handle_store(&mut self, flow: &mut Flow<Self::S>, f: &Function, inst: NodeId, var: VarId);
    /// An `ldloc` or `ldloca`.
    fn handle_load(&mut self, flow: &mut Flow<Self::S>, f: &Function, inst: NodeId);
    /// Visits `inst` in place of the default case (an override like `VisitCall`); false lets
    /// the visitor handle it.
    fn visit_override(&mut self, _flow: &mut Flow<Self::S>, _f: &Function, _inst: NodeId) -> bool {
        false
    }
}

/// The visitor's state (`DataFlowVisitor<State>` fields).
pub struct Flow<S: State> {
    pub state: S,
    /// `currentStateOnException`: everything that can reach the innermost enclosing handler.
    pub current_state_on_exception: S,
    bottom: S,
    /// Instructions with these direct flags need a case of their own (`flagsRequiringManualImpl`).
    manual_flags: Flags,
    state_on_branch: HashMap<NodeId, S>,
    state_on_leave: HashMap<NodeId, S>,
    work_lists: HashMap<NodeId, BTreeSet<usize>>,
    branches_triggering_finally: Vec<(NodeId, S)>,
    state_on_exception: HashMap<NodeId, S>,
}

impl<S: State> Flow<S> {
    /// `Initialize`. `extra_manual_flags` are what the analysis handles itself
    /// (`MayWriteLocals` for the reaching definitions).
    pub fn new(initial: S, extra_manual_flags: Flags) -> Self {
        let mut bottom = initial.clone();
        bottom.replace_with_bottom();
        Flow {
            current_state_on_exception: initial.clone(),
            state: initial,
            bottom,
            manual_flags: Flags::CONTROL_FLOW
                | Flags::MAY_BRANCH
                | Flags::MAY_UNWRAP_NULL
                | Flags::END_POINT_UNREACHABLE
                | extra_manual_flags,
            state_on_branch: HashMap::new(),
            state_on_leave: HashMap::new(),
            work_lists: HashMap::new(),
            branches_triggering_finally: Vec::new(),
            state_on_exception: HashMap::new(),
        }
    }

    fn mark_unreachable(&mut self) {
        self.state.replace_with_bottom();
    }

    fn block_input_state(&mut self, block: NodeId) -> &mut S {
        let bottom = &self.bottom;
        self.state_on_branch
            .entry(block)
            .or_insert_with(|| bottom.clone())
    }

    /// Visits `id`. Recursion follows the tree, as the managed visitor does.
    pub fn visit<A: Analysis<S = S>>(&mut self, a: &mut A, f: &Function, id: NodeId) {
        if a.visit_override(self, f, id) {
            return;
        }
        match f.inst(id) {
            Inst::BlockContainer { blocks, .. } => self.visit_block_container(a, f, id, blocks),
            Inst::Branch { target } => {
                let BranchTarget::Block(target) = *target else {
                    panic!("unconnected branch");
                };
                let container = f.node(target).parent.expect("target in a container");
                if executes_finally_block(f, id, container) {
                    self.branches_triggering_finally
                        .push((id, self.state.clone()));
                } else {
                    let s = self.state.clone();
                    self.merge_into_target_block(f, target, &s);
                }
                self.mark_unreachable();
            }
            &Inst::Leave { target, value } => {
                self.visit(a, f, value);
                let container = target.expect("connected leave");
                if executes_finally_block(f, id, container) {
                    self.branches_triggering_finally
                        .push((id, self.state.clone()));
                } else {
                    let s = self.state.clone();
                    self.merge_into_state_on_leave(container, &s);
                }
                self.mark_unreachable();
            }
            &Inst::Throw { arg } => {
                self.visit(a, f, arg);
                self.mark_unreachable();
            }
            Inst::Rethrow | Inst::InvalidBranch { .. } => self.mark_unreachable(),
            Inst::TryCatch {
                try_block,
                handlers,
            } => {
                let mut on_exception = self.handle_try_block(a, f, id, *try_block);
                let mut endpoint = self.state.clone();
                for &handler in handlers {
                    let &Inst::TryCatchHandler {
                        filter,
                        body,
                        variable,
                        ..
                    } = f.inst(handler)
                    else {
                        panic!("catch handler expected");
                    };
                    self.state.replace_with(&on_exception);
                    a.handle_store(self, f, handler, variable);
                    self.visit(a, f, filter);
                    on_exception.join_with(&self.state);
                    self.visit(a, f, body);
                    endpoint.join_with(&self.state);
                }
                self.state = endpoint;
            }
            Inst::TryCatchHandler { .. } => panic!("catch handlers are visited by their try"),
            &Inst::TryFinally {
                try_block,
                finally_block,
            } => {
                let old_count = self.branches_triggering_finally.len();
                let on_exception = self.handle_try_block(a, f, id, try_block);
                let mut on_success = self.state.clone();
                self.state.join_with(&on_exception);
                self.visit(a, f, finally_block);
                self.process_branches_leaving_try_finally(f, id, old_count);
                on_success.trigger_finally(&self.state);
                self.state = on_success;
            }
            &Inst::TryFault {
                try_block,
                fault_block,
            } => {
                let on_exception = self.handle_try_block(a, f, id, try_block);
                let on_success = std::mem::replace(&mut self.state, on_exception);
                self.visit(a, f, fault_block);
                self.state = on_success;
            }
            &Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } => {
                let (before_then, before_else) = self.evaluate_condition(a, f, condition);
                self.state = before_then;
                self.visit(a, f, true_inst);
                let after_true = std::mem::replace(&mut self.state, before_else);
                self.visit(a, f, false_inst);
                self.state.join_with(&after_true);
            }
            Inst::Switch {
                value, sections, ..
            } => {
                self.visit(a, f, *value);
                let before_sections = self.state.clone();
                self.visit(a, f, sections[0]);
                let mut after_sections = self.state.clone();
                for &section in &sections[1..] {
                    self.state.replace_with(&before_sections);
                    self.visit(a, f, section);
                    after_sections.join_with(&self.state);
                }
                self.state = after_sections;
            }
            &Inst::StLoc { var, value, .. } => {
                self.visit(a, f, value);
                a.handle_store(self, f, id, var);
            }
            &Inst::YieldReturn { value } => self.visit(a, f, value),
            // `ReachingDefinitionsVisitor.VisitPinnedRegion`.
            &Inst::PinnedRegion {
                variable,
                init,
                body,
            } => {
                self.visit(a, f, init);
                a.handle_store(self, f, id, variable);
                self.visit(a, f, body);
            }
            // `VisitUsingInstruction`: no store of the variable is recorded.
            &Inst::Using {
                resource_expression,
                body,
                ..
            } => {
                self.visit(a, f, resource_expression);
                self.visit(a, f, body);
            }
            // `VisitLockInstruction`.
            &Inst::Lock {
                on_expression,
                body,
            } => {
                self.visit(a, f, on_expression);
                self.visit(a, f, body);
            }
            Inst::LdLoc { .. } | Inst::LdLoca { .. } => a.handle_load(self, f, id),
            inst => {
                if inst.direct_flags().0 & self.manual_flags.0 != 0 {
                    panic!("the data flow analysis has no case for {inst:?}");
                }
                for child in inst.children() {
                    self.visit(a, f, child);
                }
            }
        }
    }

    fn visit_block_container<A: Analysis<S = S>>(
        &mut self,
        a: &mut A,
        f: &Function,
        container: NodeId,
        blocks: &[NodeId],
    ) {
        let mut work_list = BTreeSet::new();
        let state = self.state.clone();
        let entry = self.block_input_state(blocks[0]);
        if !state.less_than_or_equal(entry) {
            entry.join_with(&state);
            work_list.insert(0);
        }
        self.work_lists.insert(container, work_list);
        while let Some(index) = self.work_lists[&container].first().copied() {
            self.work_lists.get_mut(&container).unwrap().remove(&index);
            let block = blocks[index];
            let input = self.state_on_branch[&block].clone();
            self.state.replace_with(&input);
            self.visit(a, f, block);
        }
        match self.state_on_leave.get(&container) {
            Some(exit) => {
                let exit = exit.clone();
                self.state.replace_with(&exit);
            }
            None => self.mark_unreachable(),
        }
        self.work_lists.remove(&container);
    }

    fn merge_into_target_block(&mut self, f: &Function, target: NodeId, branch_state: &S) {
        let target_state = self.block_input_state(target);
        if !branch_state.less_than_or_equal(target_state) {
            target_state.join_with(branch_state);
            let container = f.node(target).parent.expect("target in a container");
            let index = f.node(target).child_index as usize;
            // A branch to a block that left its container (a stale parent) finds no work list;
            // the managed code only asserts.
            if let Some(work_list) = self.work_lists.get_mut(&container) {
                work_list.insert(index);
            }
        }
    }

    fn merge_into_state_on_leave(&mut self, container: NodeId, branch_state: &S) {
        match self.state_on_leave.get_mut(&container) {
            Some(s) => s.join_with(branch_state),
            None => {
                self.state_on_leave.insert(container, branch_state.clone());
            }
        }
    }

    /// `HandleTryBlock`: visits the try block with its own exception state and returns the
    /// state the handlers start in.
    fn handle_try_block<A: Analysis<S = S>>(
        &mut self,
        a: &mut A,
        f: &Function,
        inst: NodeId,
        try_block: NodeId,
    ) -> S {
        let new = match self.state_on_exception.remove(&inst) {
            Some(mut s) => {
                s.join_with(&self.state);
                s
            }
            None => self.state.clone(),
        };
        let old = std::mem::replace(&mut self.current_state_on_exception, new);
        self.visit(a, f, try_block);
        let new = std::mem::replace(&mut self.current_state_on_exception, old);
        self.current_state_on_exception.join_with(&new);
        let result = new.clone();
        self.state_on_exception.insert(inst, new);
        result
    }

    fn process_branches_leaving_try_finally(
        &mut self,
        f: &Function,
        try_finally: NodeId,
        old_count: usize,
    ) {
        let pending = self.branches_triggering_finally.split_off(old_count);
        for (branch, mut branch_state) in pending {
            branch_state.trigger_finally(&self.state);
            let container = match f.inst(branch) {
                Inst::Leave { target, .. } => target.expect("connected leave"),
                _ => {
                    let target = f.branch_target(branch);
                    f.node(target).parent.expect("target in a container")
                }
            };
            if executes_finally_block(f, try_finally, container) {
                self.branches_triggering_finally
                    .push((branch, branch_state));
            } else if let Inst::Leave { .. } = f.inst(branch) {
                self.merge_into_state_on_leave(container, &branch_state);
            } else {
                let target = f.branch_target(branch);
                self.merge_into_target_block(f, target, &branch_state);
            }
        }
    }

    /// `EvaluateCondition`: the states after `inst` when it is true and when it is false.
    fn evaluate_condition<A: Analysis<S = S>>(
        &mut self,
        a: &mut A,
        f: &Function,
        inst: NodeId,
    ) -> (S, S) {
        match *f.inst(inst) {
            Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } => {
                let (before_then, before_else) = self.evaluate_condition(a, f, condition);
                self.state = before_then;
                let (mut on_true, mut on_false) = self.evaluate_condition(a, f, true_inst);
                self.state = before_else;
                let (else_true, else_false) = self.evaluate_condition(a, f, false_inst);
                on_true.join_with(&else_true);
                on_false.join_with(&else_false);
                (on_true, on_false)
            }
            Inst::LdcI4 { value } => {
                if value == 0 {
                    (self.bottom.clone(), self.state.clone())
                } else {
                    (self.state.clone(), self.bottom.clone())
                }
            }
            _ => {
                self.visit(a, f, inst);
                (self.state.clone(), self.state.clone())
            }
        }
    }
}

/// `Branch.GetExecutesFinallyBlock`: whether leaving from `inst` to `container` runs a
/// `finally` block on the way.
pub fn executes_finally_block(f: &Function, inst: NodeId, container: NodeId) -> bool {
    let mut current = Some(inst);
    while let Some(i) = current
        && i != container
    {
        let parent = f.node(i).parent;
        if let Some(p) = parent
            && let Inst::TryFinally { try_block, .. } = *f.inst(p)
            && try_block == i
        {
            return true;
        }
        current = parent;
    }
    false
}
