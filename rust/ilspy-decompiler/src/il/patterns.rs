//! `IL/Instructions/PatternMatching.cs`: the `Match*` helpers of `ILInstruction`.

use super::ast::{BranchTarget, Function, Inst, NodeId, VarId};

impl Function {
    /// `MatchLdLoc`.
    pub fn match_ldloc(&self, id: NodeId) -> Option<VarId> {
        match *self.inst(id) {
            Inst::LdLoc { var } => Some(var),
            _ => None,
        }
    }

    /// `MatchStLoc`: the variable and the value.
    pub fn match_stloc(&self, id: NodeId) -> Option<(VarId, NodeId)> {
        match *self.inst(id) {
            Inst::StLoc { var, value, .. } => Some((var, value)),
            _ => None,
        }
    }

    /// `MatchBranch`: the target block.
    pub fn match_branch(&self, id: NodeId) -> Option<NodeId> {
        match *self.inst(id) {
            Inst::Branch {
                target: BranchTarget::Block(b),
            } => Some(b),
            _ => None,
        }
    }

    /// `MatchLdcI4`.
    pub fn match_ldc_i4(&self, id: NodeId) -> Option<i32> {
        match *self.inst(id) {
            Inst::LdcI4 { value } => Some(value),
            _ => None,
        }
    }

    /// `MatchLdcI`: an `ldc.i4` or `ldc.i8` value.
    pub fn match_ldc_i(&self, id: NodeId) -> Option<i64> {
        match *self.inst(id) {
            Inst::LdcI4 { value } => Some(i64::from(value)),
            Inst::LdcI8 { value } => Some(value),
            _ => None,
        }
    }

    /// `MatchNop`.
    pub fn match_nop(&self, id: NodeId) -> bool {
        matches!(self.inst(id), Inst::Nop { .. })
    }

    /// `MatchLeave`: the target container and the value.
    pub fn match_leave(&self, id: NodeId) -> Option<(NodeId, NodeId)> {
        match *self.inst(id) {
            Inst::Leave {
                target: Some(c),
                value,
            } => Some((c, value)),
            _ => None,
        }
    }

    /// `MatchReturn`: a `leave` out of the function body, and its value.
    pub fn match_return(&self, id: NodeId) -> Option<NodeId> {
        self.match_leave(id)
            .filter(|&(c, _)| Some(c) == self.body)
            .map(|(_, value)| value)
    }
}
