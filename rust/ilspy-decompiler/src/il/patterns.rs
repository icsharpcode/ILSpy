//! `IL/Instructions/PatternMatching.cs`: the `Match*` helpers of `ILInstruction`.

use super::ast::{BranchTarget, ComparisonKind, Function, Inst, NodeId, VarId};

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

impl Function {
    /// `MatchLogicNot`: `comp.i4(x == ldc.i4 0)`, unlifted.
    pub fn match_logic_not(&self, id: NodeId) -> Option<NodeId> {
        match *self.inst(id) {
            Inst::Comp {
                kind: ComparisonKind::Equality,
                left,
                right,
                ..
            } if self.match_ldc_i4(right) == Some(0) => Some(left),
            _ => None,
        }
    }

    /// `MatchCompEquals` (`want` Equality) and `MatchCompNotEquals` (Inequality): a
    /// comparison under any number of logic nots, each flipping the kind.
    pub fn match_comp(&self, id: NodeId, want: ComparisonKind) -> Option<(NodeId, NodeId)> {
        let mut this = id;
        let mut kind = want;
        while let Some(arg) = self.match_logic_not(this)
            && matches!(self.inst(arg), Inst::Comp { .. })
        {
            this = arg;
            kind = if kind == ComparisonKind::Equality {
                ComparisonKind::Inequality
            } else {
                ComparisonKind::Equality
            };
        }
        match *self.inst(this) {
            Inst::Comp {
                kind: k,
                left,
                right,
                ..
            } if k == kind => Some((left, right)),
            _ => None,
        }
    }

    /// `MatchCompEqualsNull` / `MatchCompNotEqualsNull`: the operand compared with `ldnull`.
    pub fn match_comp_null(&self, id: NodeId, want: ComparisonKind) -> Option<NodeId> {
        let (left, right) = self.match_comp(id, want)?;
        if matches!(self.inst(right), Inst::LdNull) {
            Some(left)
        } else if matches!(self.inst(left), Inst::LdNull) {
            Some(right)
        } else {
            None
        }
    }
}
