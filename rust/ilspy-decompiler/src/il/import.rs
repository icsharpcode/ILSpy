//! Moving the nodes and variables of one function's arena into another. The managed state
//! machine decompilers read compiler-generated methods into separate `ILFunction`s and then
//! move their instructions and variables over; in Rust each function owns an arena, so the
//! other arena is appended and its ids are shifted.

use super::ast::{BranchTarget, Function, Inst, NodeId, VarId};

/// Where an imported arena's nodes and variables start in the importing one.
#[derive(Clone, Copy, Debug)]
pub struct Imported {
    node_offset: u32,
    var_offset: u32,
}

impl Imported {
    pub fn node(&self, id: NodeId) -> NodeId {
        NodeId(id.0 + self.node_offset)
    }

    pub fn var(&self, id: VarId) -> VarId {
        VarId(id.0 + self.var_offset)
    }
}

impl Inst {
    /// Shifts every node and variable id the instruction refers to.
    fn shift_ids(&mut self, by: Imported) {
        for (i, child) in self.children().into_iter().enumerate() {
            self.set_child(i, by.node(child));
        }
        match self {
            Inst::Branch {
                target: BranchTarget::Block(b),
            } => *b = by.node(*b),
            Inst::Leave {
                target: Some(c), ..
            } => *c = by.node(*c),
            Inst::LdLoc { var }
            | Inst::LdLoca { var }
            | Inst::StLoc { var, .. }
            | Inst::TryCatchHandler { variable: var, .. }
            | Inst::PinnedRegion { variable: var, .. }
            | Inst::Using { variable: var, .. } => *var = by.var(*var),
            _ => {}
        }
    }
}

impl Function {
    /// Appends `other`'s nodes and variables. Connection state, use lists and counts come
    /// along unchanged: `other`'s tree stays connected to its own (now imported) root until
    /// the caller releases it.
    pub fn import(&mut self, other: Function) -> Imported {
        let by = Imported {
            node_offset: self.nodes.len() as u32,
            var_offset: self.variables.len() as u32,
        };
        for mut node in other.nodes {
            node.inst.shift_ids(by);
            node.parent = node.parent.map(|p| by.node(p));
            self.nodes.push(node);
        }
        for mut var in other.variables {
            for list in [
                &mut var.load_instructions,
                &mut var.store_instructions,
                &mut var.address_instructions,
            ] {
                for n in list.iter_mut() {
                    *n = by.node(*n);
                }
            }
            self.variables.push(var);
        }
        by
    }

    /// `ILFunction.ReleaseRef` of a function root: disconnects the tree under `root`.
    pub fn release_root(&mut self, root: NodeId) {
        self.release_ref(root);
    }
}
