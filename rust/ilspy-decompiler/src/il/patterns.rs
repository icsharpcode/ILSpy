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

    /// `MatchLdcI`: an `ldc.i4` or `ldc.i8` value, also through a sign extension or a zero
    /// extension of an `I4` (which clears the top 32 bits).
    pub fn match_ldc_i(&self, id: NodeId) -> Option<i64> {
        match *self.inst(id) {
            Inst::LdcI4 { value } => Some(i64::from(value)),
            Inst::LdcI8 { value } => Some(value),
            Inst::Conv {
                arg,
                input,
                input_sign,
                target,
                ..
            } => match super::ast::conversion_kind(target, input, input_sign) {
                super::ast::ConversionKind::SignExtend => self.match_ldc_i(arg),
                super::ast::ConversionKind::ZeroExtend if input == super::types::StackType::I4 => {
                    self.match_ldc_i(arg).map(|v| v & i64::from(u32::MAX))
                }
                _ => None,
            },
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
                lifting: super::ast::ComparisonLiftingKind::None,
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

impl Function {
    /// `MatchIfInstruction` without an else branch: the condition and the true instruction.
    pub fn match_if_instruction(&self, id: NodeId) -> Option<(NodeId, NodeId)> {
        match *self.inst(id) {
            Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } if self.match_nop(false_inst) => Some((condition, true_inst)),
            _ => None,
        }
    }

    /// `Block.MatchIfAtEndOfBlock`: `if (condition) trueInst; falseInst` as the last two
    /// instructions, with each logic not of the condition swapping the two.
    pub fn match_if_at_end_of_block(&self, block: NodeId) -> Option<(NodeId, NodeId, NodeId)> {
        let list = self.list(block);
        if list.len() < 2 {
            return None;
        }
        let (mut condition, mut true_inst) = self.match_if_instruction(list[list.len() - 2])?;
        let mut false_inst = list[list.len() - 1];
        while let Some(arg) = self.match_logic_not(condition) {
            condition = arg;
            std::mem::swap(&mut true_inst, &mut false_inst);
        }
        Some((condition, true_inst, false_inst))
    }
}

impl Function {
    /// The variable of an instruction with a variable operand
    /// (`IInstructionWithVariableOperand.Variable`).
    pub fn variable_of(&self, id: NodeId) -> Option<VarId> {
        match *self.inst(id) {
            Inst::LdLoc { var } | Inst::LdLoca { var } | Inst::StLoc { var, .. } => Some(var),
            Inst::TryCatchHandler { variable, .. }
            | Inst::PinnedRegion { variable, .. }
            | Inst::Match { variable, .. } => Some(variable),
            _ => None,
        }
    }

    /// `ILInstruction.Match(other).Success` for instructions without pattern nodes: the same
    /// kind with the same operands, and structurally equal children.
    pub fn structurally_equal(&self, a: NodeId, b: NodeId) -> bool {
        let (ia, ib) = (self.inst(a), self.inst(b));
        let (ca, cb) = (ia.children(), ib.children());
        if ca.len() != cb.len() {
            return false;
        }
        // Compare everything but the child ids.
        let blank = |inst: &Inst, children: &[NodeId]| {
            let mut inst = inst.clone();
            for i in 0..children.len() {
                inst.set_child(i, NodeId(0));
            }
            format!("{inst:?}")
        };
        blank(ia, &ca) == blank(ib, &cb)
            && ca
                .iter()
                .zip(&cb)
                .all(|(&x, &y)| self.structurally_equal(x, y))
    }
}

impl Function {
    /// `ILVariable.IsThis`.
    pub fn is_this(&self, v: VarId) -> bool {
        let var = self.var(v);
        var.kind == super::ast::VariableKind::Parameter && var.index.is_some_and(|i| i < 0)
    }

    /// `MatchLdThis`.
    pub fn match_ld_this(&self, id: NodeId) -> bool {
        self.match_ldloc(id).is_some_and(|v| self.is_this(v))
    }

    /// `MatchStFld`: `stobj(ldflda F(target), value)`, neither volatile nor unaligned.
    pub fn match_stfld(&self, id: NodeId) -> Option<(NodeId, &super::ast::FieldOperand, NodeId)> {
        let Inst::StObj {
            target: address,
            value,
            is_volatile: false,
            unaligned: 0,
            ..
        } = *self.inst(id)
        else {
            return None;
        };
        match self.inst(address) {
            Inst::LdFlda { target, field, .. } => Some((*target, field, value)),
            _ => None,
        }
    }

    /// `MatchLdFld`: `ldobj(ldflda F(target))`, neither volatile nor unaligned. For a field of
    /// a value type, a target `addressof(x)` stands for `x`.
    pub fn match_ldfld(
        &self,
        ts: &mut crate::typesystem::types::TypeSystem<'_, '_>,
        id: NodeId,
    ) -> Option<(NodeId, &super::ast::FieldOperand)> {
        let Inst::LdObj {
            target: address,
            is_volatile: false,
            unaligned: 0,
            ..
        } = *self.inst(id)
        else {
            return None;
        };
        let Inst::LdFlda { target, field, .. } = self.inst(address) else {
            return None;
        };
        let reference = field
            .field
            .declaring
            .is_some_and(|d| ts.is_reference_type(d) == Some(true));
        let target = match self.inst(*target) {
            Inst::AddressOf { value, .. } if !reference => *value,
            _ => *target,
        };
        Some((target, field))
    }

    /// `MatchDefaultOrNullOrZero`.
    pub fn match_default_or_null_or_zero(&self, id: NodeId) -> bool {
        match *self.inst(id) {
            Inst::LdNull | Inst::DefaultValue { .. } => true,
            Inst::LdcI4 { value } => value == 0,
            Inst::LdcI8 { value } => value == 0,
            Inst::LdcF4 { bits } => f32::from_bits(bits) == 0.0,
            Inst::LdcF8 { bits } => f64::from_bits(bits) == 0.0,
            _ => false,
        }
    }
}

impl Function {
    /// `MatchLdLocRef`: `ldloc` of a reference-type variable, or `ldloca` of any other (or of
    /// a type parameter).
    pub fn match_ld_loc_ref(
        &self,
        ts: &mut crate::typesystem::types::TypeSystem<'_, '_>,
        id: NodeId,
    ) -> Option<VarId> {
        match *self.inst(id) {
            Inst::LdLoc { var } => {
                (ts.is_reference_type(self.var(var).ty) == Some(true)).then_some(var)
            }
            Inst::LdLoca { var } => {
                let ty = self.var(var).ty;
                let is_type_param = matches!(
                    ts.data(ty),
                    crate::typesystem::types::TypeData::TypeParam { .. }
                );
                (ts.is_reference_type(ty) != Some(true) || is_type_param).then_some(var)
            }
            _ => None,
        }
    }

    /// `MatchLdFlda`.
    pub fn match_ldflda(&self, id: NodeId) -> Option<(NodeId, &super::ast::FieldOperand)> {
        match self.inst(id) {
            Inst::LdFlda { target, field, .. } => Some((*target, field)),
            _ => None,
        }
    }

    /// `MatchLdsFld`: `ldobj(ldsflda F)`, neither volatile nor unaligned.
    pub fn match_ldsfld(&self, id: NodeId) -> Option<&super::ast::FieldOperand> {
        let Inst::LdObj {
            target,
            is_volatile: false,
            unaligned: 0,
            ..
        } = *self.inst(id)
        else {
            return None;
        };
        match self.inst(target) {
            Inst::LdsFlda { field } => Some(field),
            _ => None,
        }
    }

    /// `SlotInfo.CanInlineInto` of child slot `index`: false for the slots holding statements
    /// or blocks.
    pub fn slot_can_inline_into(&self, parent: NodeId, index: usize) -> bool {
        match self.inst(parent) {
            Inst::If { .. }
            | Inst::Switch { .. }
            | Inst::PinnedRegion { .. }
            | Inst::Match { .. } => index == 0,
            Inst::SwitchSection { .. }
            | Inst::TryCatchHandler { .. }
            | Inst::Block { .. }
            | Inst::BlockContainer { .. }
            | Inst::TryCatch { .. }
            | Inst::TryFinally { .. }
            | Inst::TryFault { .. } => false,
            _ => true,
        }
    }

    /// `ILInlining.FindFirstInlinedCall`: the first call evaluated in `inst`, following only
    /// inlinable slots.
    pub fn find_first_inlined_call(&self, inst: NodeId) -> Option<NodeId> {
        for (i, child) in self.inst(inst).children().into_iter().enumerate() {
            if !self.slot_can_inline_into(inst, i) {
                break;
            }
            if let Some(call) = self.find_first_inlined_call(child) {
                return Some(call);
            }
        }
        matches!(
            self.inst(inst),
            Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_)
        )
        .then_some(inst)
    }
}
