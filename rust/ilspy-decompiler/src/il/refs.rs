//! Use counts of the ILAst (`ILInstruction.AddRef` / `ReleaseRef` and the `Connected` /
//! `Disconnected` overrides): a node is connected while it is reachable from the function
//! root, and only connected instructions are loads, stores and address uses of their
//! variable, branches into a block or leaves of a container. Every editing operation of
//! `Function` keeps this up to date in the managed order, because transforms iterate the
//! variables' instruction lists: connecting visits the children before the node,
//! disconnecting the node before its children, and a list removal moves the last entry into
//! the gap.

use std::collections::HashMap;

use super::ast::{BranchTarget, Function, Inst, NodeId, VarId};

/// Which instruction list of a variable an instruction belongs to.
#[derive(Clone, Copy)]
enum VarList {
    Load,
    Store,
    Address,
}

impl Function {
    pub fn is_connected(&self, id: NodeId) -> bool {
        self.node(id).ref_count > 0
    }

    /// `ILFunction.AddRef` of the body: connects `body` as the function root.
    pub fn connect_root(&mut self, body: NodeId) {
        self.add_ref(body);
    }

    /// `AddRef`: on the first reference, the children connect first, then the node
    /// (`Connected` calls the base, which visits the children, before its own work).
    pub(super) fn add_ref(&mut self, id: NodeId) {
        let mut stack = vec![(id, false)];
        while let Some((n, expanded)) = stack.pop() {
            if expanded {
                self.count(n, true);
                continue;
            }
            let node = &mut self.nodes[n.0 as usize];
            node.ref_count += 1;
            if node.ref_count == 1 {
                stack.push((n, true));
                for child in self.inst(n).children().into_iter().rev() {
                    stack.push((child, false));
                }
            }
        }
    }

    /// `ReleaseRef`: on the last release, the node disconnects first, then its children.
    pub(super) fn release_ref(&mut self, id: NodeId) {
        let mut stack = vec![id];
        while let Some(n) = stack.pop() {
            let node = &mut self.nodes[n.0 as usize];
            assert!(node.ref_count > 0, "released a disconnected node");
            node.ref_count -= 1;
            if node.ref_count == 0 {
                self.count(n, false);
                for child in self.inst(n).children().into_iter().rev() {
                    stack.push(child);
                }
            }
        }
    }

    fn var_list(&mut self, var: VarId, list: VarList) -> &mut Vec<NodeId> {
        let v = self.var_mut(var);
        match list {
            VarList::Load => &mut v.load_instructions,
            VarList::Store => &mut v.store_instructions,
            VarList::Address => &mut v.address_instructions,
        }
    }

    /// `AddXInstruction` / `RemoveXInstruction` (a swap-remove).
    fn update_var_list(&mut self, id: NodeId, var: VarId, list: VarList, connect: bool) {
        if connect {
            let entries = self.var_list(var, list);
            entries.push(id);
            let index = entries.len() as u32 - 1;
            self.nodes[id.0 as usize].var_list_index = index;
        } else {
            let index = self.node(id).var_list_index as usize;
            let entries = self.var_list(var, list);
            assert_eq!(entries[index], id, "variable instruction list out of sync");
            entries.swap_remove(index);
            if let Some(&moved) = entries.get(index) {
                self.nodes[moved.0 as usize].var_list_index = index as u32;
            }
        }
    }

    /// What connecting or disconnecting `id` itself changes.
    fn count(&mut self, id: NodeId, connect: bool) {
        fn bump(c: &mut u32, connect: bool) {
            if connect {
                *c += 1;
            } else {
                *c = c.checked_sub(1).expect("use count underflow");
            }
        }
        match *self.inst(id) {
            Inst::LdLoc { var } => self.update_var_list(id, var, VarList::Load, connect),
            Inst::LdLoca { var } => self.update_var_list(id, var, VarList::Address, connect),
            Inst::StLoc { var, .. }
            | Inst::TryCatchHandler { variable: var, .. }
            | Inst::Match { variable: var, .. }
            | Inst::PinnedRegion { variable: var, .. }
            | Inst::Using { variable: var, .. } => {
                self.update_var_list(id, var, VarList::Store, connect)
            }
            Inst::Branch {
                target: BranchTarget::Block(b),
            } => bump(&mut self.nodes[b.0 as usize].incoming_edge_count, connect),
            Inst::Leave {
                target: Some(c), ..
            } => bump(&mut self.nodes[c.0 as usize].leave_count, connect),
            Inst::BlockContainer { .. } => {
                if let Some(entry) = self.entry_point(id) {
                    bump(
                        &mut self.nodes[entry.0 as usize].incoming_edge_count,
                        connect,
                    );
                }
            }
            _ => {}
        }
    }

    /// The entry point of a container, or None for any other node.
    pub fn entry_point(&self, id: NodeId) -> Option<NodeId> {
        match self.inst(id) {
            Inst::BlockContainer { blocks, .. } => blocks.first().copied(),
            _ => None,
        }
    }

    /// Moves the entry point's implicit incoming edge after the block list of a connected
    /// container changed (`BlockContainer.InstructionCollectionUpdateComplete`).
    pub(super) fn entry_point_changed(&mut self, owner: NodeId, before: Option<NodeId>) {
        let after = self.entry_point(owner);
        if before == after || !self.is_connected(owner) {
            return;
        }
        if let Some(b) = before {
            self.nodes[b.0 as usize].incoming_edge_count -= 1;
        }
        if let Some(a) = after {
            self.nodes[a.0 as usize].incoming_edge_count += 1;
        }
    }

    /// `Branch.TargetBlock = block`.
    pub fn set_branch_target(&mut self, branch: NodeId, block: NodeId) {
        let connected = self.is_connected(branch);
        if connected {
            self.count(branch, false);
        }
        match self.inst_mut(branch) {
            Inst::Branch { target } => *target = BranchTarget::Block(block),
            other => panic!("{other:?} is not a branch"),
        }
        if connected {
            self.count(branch, true);
        }
    }

    /// `Leave.TargetContainer = container`.
    pub fn set_leave_target(&mut self, leave: NodeId, container: NodeId) {
        let connected = self.is_connected(leave);
        if connected {
            self.count(leave, false);
        }
        match self.inst_mut(leave) {
            Inst::Leave { target, .. } => *target = Some(container),
            other => panic!("{other:?} is not a leave"),
        }
        if connected {
            self.count(leave, true);
        }
    }

    /// The target block of a connected branch.
    pub fn branch_target(&self, branch: NodeId) -> NodeId {
        match self.inst(branch) {
            Inst::Branch {
                target: BranchTarget::Block(b),
            } => *b,
            other => panic!("{other:?} is not a connected branch"),
        }
    }

    /// `ILVariable` setter of a load, address or store instruction.
    pub fn set_variable(&mut self, id: NodeId, var: VarId) {
        let connected = self.is_connected(id);
        if connected {
            self.count(id, false);
        }
        match self.inst_mut(id) {
            Inst::LdLoc { var: v }
            | Inst::LdLoca { var: v }
            | Inst::StLoc { var: v, .. }
            | Inst::TryCatchHandler { variable: v, .. }
            | Inst::Match { variable: v, .. }
            | Inst::PinnedRegion { variable: v, .. }
            | Inst::Using { variable: v, .. } => *v = var,
            other => panic!("{other:?} has no variable"),
        }
        if connected {
            self.count(id, true);
        }
    }

    /// `ILInstruction.Clone`: a deep, disconnected copy of `id`. Branches and leaves that
    /// target a block or container inside the copy are redirected into the copy.
    pub fn clone_subtree(&mut self, id: NodeId) -> NodeId {
        let mut map = HashMap::new();
        for old in self.descendants_post(id) {
            let mut inst = self.inst(old).clone();
            for (i, child) in inst.children().into_iter().enumerate() {
                inst.set_child(i, map[&child]);
            }
            let new = self.add(inst);
            self.set_il_range(new, self.il_range(old));
            map.insert(old, new);
        }
        for &new in map.values() {
            match self.inst_mut(new) {
                Inst::Branch {
                    target: BranchTarget::Block(b),
                } => {
                    if let Some(&mapped) = map.get(b) {
                        *b = mapped;
                    }
                }
                Inst::Leave {
                    target: Some(c), ..
                } => {
                    if let Some(&mapped) = map.get(c) {
                        *c = mapped;
                    }
                }
                _ => {}
            }
        }
        map[&id]
    }

    /// The parent chain of `id`, nearest first (`Ancestors` without `id` itself).
    pub fn ancestors(&self, id: NodeId) -> impl Iterator<Item = NodeId> + '_ {
        std::iter::successors(self.node(id).parent, |&p| self.node(p).parent)
    }

    /// Whether any connected instruction under `root` uses `var` (`IsUsedWithin`).
    pub fn is_used_within(&self, var: VarId, root: NodeId) -> bool {
        self.descendants_post(root)
            .into_iter()
            .any(|n| match *self.inst(n) {
                Inst::LdLoc { var: v }
                | Inst::LdLoca { var: v }
                | Inst::StLoc { var: v, .. }
                | Inst::TryCatchHandler { variable: v, .. }
                | Inst::PinnedRegion { variable: v, .. }
                | Inst::Using { variable: v, .. } => v == var,
                _ => false,
            })
    }
}

impl Function {
    /// `function.LocalFunctions.Add(localFunction)` for a nested function: `local_function`
    /// becomes its last child, counted once more when the function is connected.
    pub fn add_local_function(&mut self, function: NodeId, local_function: NodeId) {
        match self.inst_mut(function) {
            Inst::Function {
                local_functions, ..
            } => local_functions.push(local_function),
            other => panic!("{other:?} is not a function"),
        }
        self.relink(function);
        if self.is_connected(function) {
            self.add_ref(local_function);
        }
    }
}

#[cfg(test)]
mod tests {
    use super::super::ast::*;
    use crate::typesystem::types::TypeId;
    use crate::util::interval::Interval;

    fn var(f: &mut Function) -> VarId {
        f.add_var(Variable {
            kind: VariableKind::Local,
            ty: TypeId::PLACEHOLDER,
            stack_type: crate::il::types::StackType::I4,
            index: Some(0),
            name: "V_0".into(),
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
        })
    }

    fn block(f: &mut Function, instructions: Vec<NodeId>) -> NodeId {
        let final_inst = f.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        f.add(Inst::Block {
            kind: BlockKind::ControlFlow,
            instructions,
            final_inst,
        })
    }

    /// A container with an entry block storing `ldloc v` into `v` and branching to a second
    /// block, which leaves the container.
    fn sample(f: &mut Function) -> (NodeId, NodeId, NodeId, NodeId, VarId) {
        let v = var(f);
        let load = f.add(Inst::LdLoc { var: v });
        let store = f.add(Inst::StLoc {
            var: v,
            value: load,
            il_stack_was_empty: true,
            is_stack_adjustment: false,
        });
        let exit_value = f.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let leave = f.add(Inst::Leave {
            target: None,
            value: exit_value,
        });
        let exit = block(f, vec![leave]);
        let branch = f.add(Inst::Branch {
            target: BranchTarget::Block(exit),
        });
        let entry = block(f, vec![store, branch]);
        let container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Normal,
            expected: None,
            blocks: vec![entry, exit],
        });
        if let Inst::Leave { target, .. } = f.inst_mut(leave) {
            *target = Some(container);
        }
        (container, entry, exit, store, v)
    }

    #[test]
    fn counts_start_when_the_root_is_connected() {
        let mut f = Function::default();
        let (container, entry, exit, _, v) = sample(&mut f);
        assert_eq!(f.var(v).load_count(), 0);
        f.connect_root(container);
        assert_eq!(
            (f.var(v).load_count(), f.var(v).store_count()),
            (1, 1),
            "one load, one store"
        );
        assert_eq!(
            f.node(entry).incoming_edge_count,
            1,
            "the implicit entry edge"
        );
        assert_eq!(f.node(exit).incoming_edge_count, 1, "the branch");
        assert_eq!(f.leave_count(container), 1);
    }

    #[test]
    fn replacing_a_store_by_its_value_keeps_the_value_counted() {
        let mut f = Function::default();
        let (container, _, _, store, v) = sample(&mut f);
        f.connect_root(container);
        let value = f.inst(store).children()[0];
        f.replace(store, value);
        assert_eq!(f.var(v).store_count(), 0);
        assert_eq!(f.var(v).load_count(), 1);
        assert!(f.is_connected(value) && !f.is_connected(store));
    }

    #[test]
    fn list_edits_move_the_entry_edge_and_release_removed_items() {
        let mut f = Function::default();
        let (container, entry, exit, _, v) = sample(&mut f);
        f.connect_root(container);
        f.remove_at(container, 0);
        assert_eq!(f.node(exit).incoming_edge_count, 1, "now the entry point");
        assert_eq!(f.node(entry).incoming_edge_count, 0);
        assert_eq!(
            f.var(v).load_count(),
            0,
            "the removed entry block took its load"
        );
        f.truncate(exit, 0);
        assert_eq!(f.leave_count(container), 0);
    }

    #[test]
    fn retargeting_and_cloning() {
        let mut f = Function::default();
        let (container, entry, exit, _, _) = sample(&mut f);
        f.connect_root(container);
        let branch = f.list(entry)[1];
        f.set_branch_target(branch, entry);
        assert_eq!(f.node(exit).incoming_edge_count, 0);
        assert_eq!(f.node(entry).incoming_edge_count, 2);
        f.set_il_range(container, Interval::new(0, 4));
        let copy = f.clone_subtree(container);
        assert!(!f.is_connected(copy));
        assert_eq!(f.il_range(copy), Interval::new(0, 4));
        let copied_entry = f.list(copy)[0];
        let copied_branch = f.list(copied_entry)[1];
        assert_eq!(f.branch_target(copied_branch), copied_entry);
        assert_eq!(
            f.node(entry).incoming_edge_count,
            2,
            "a disconnected copy counts nothing"
        );
    }

    #[test]
    fn variable_lists_follow_the_managed_order() {
        let mut f = Function::default();
        let v = var(&mut f);
        let loads: Vec<NodeId> = (0..3).map(|_| f.add(Inst::LdLoc { var: v })).collect();
        // `throw(ldloc)` statements in one block, so each load is its own statement's child.
        let throws: Vec<NodeId> = loads
            .iter()
            .map(|&l| f.add(Inst::Throw { arg: l }))
            .collect();
        let b = block(&mut f, throws.clone());
        let container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Normal,
            expected: None,
            blocks: vec![b],
        });
        f.connect_root(container);
        assert_eq!(f.var(v).load_instructions, loads, "connected in tree order");
        f.remove_at(b, 0);
        assert_eq!(
            f.var(v).load_instructions,
            [loads[2], loads[1]],
            "the last entry moves into the gap"
        );
        f.push(b, throws[0]);
        assert_eq!(f.var(v).load_instructions, [loads[2], loads[1], loads[0]]);
    }
}
