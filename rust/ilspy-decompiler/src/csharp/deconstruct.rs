//! `ExpressionBuilder.VisitDeconstructInstruction`: `(a, (b, _)) = value`.

use crate::il::ast::{Inst, NodeId, VarId};
use crate::typesystem::known::KnownTypeCode;

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// The state of `ConstructTuple`: the next assignment to take.
struct Positions {
    assignments: Vec<NodeId>,
    assignment: usize,
    /// `conversionMapping`: the variable a conversion stores each input variable's value to.
    conversions: Vec<(VarId, VarId)>,
}

impl Builder<'_, '_, '_, '_> {
    /// `VisitDeconstructInstruction`: the tuple of the assignment targets assigned the
    /// deconstructed value, of type `void`. Init stores of the targets' temporaries
    /// (`ReplaceAssignmentTarget`) are not translated.
    pub(crate) fn deconstruct(
        &mut self,
        inst: NodeId,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Inst::Deconstruct {
            init,
            pattern,
            conversions,
            assignments,
        } = self.f.inst(inst).clone()
        else {
            unreachable!("a deconstruction");
        };
        let Inst::Match {
            variable,
            tested_operand,
            ..
        } = *self.f.inst(pattern)
        else {
            return Err("a deconstruction of another pattern".to_owned());
        };
        if !init.is_empty() {
            return Err("a deconstruction with init stores".to_owned());
        }
        // shortcut: in a lambda the enclosing call's overload resolution then needs a cast of
        // the lambda the managed builder does without; find the difference before lifting it.
        if self.in_lambda {
            return Err("a deconstruction in a lambda".to_owned());
        }
        let rhs_type = self.f.var(variable).ty;
        let mut rhs = self.translate(tested_operand, Some(rhs_type))?;
        // Deconstructing a value type takes the address of the deconstructed value.
        if let AstNode::RefDirection { expression } = *self.ast.node(rhs.expression) {
            rhs = self.unwrap_child(rhs.expression, expression);
        }
        let rhs = self.convert_to(rhs, rhs_type, false)?;
        let block_instructions = |b: &Self, block: NodeId| match b.f.inst(block) {
            Inst::Block { instructions, .. } => instructions.clone(),
            _ => Vec::new(),
        };
        let mut positions = Positions {
            assignments: block_instructions(self, assignments),
            assignment: 0,
            conversions: Vec::new(),
        };
        for conv in block_instructions(self, conversions) {
            if let Some((output, input)) = self.conversion_st_loc(conv) {
                positions.conversions.push((input, output));
            }
        }
        let lhs = self.construct_tuple(pattern, &mut positions)?;
        let e = self.ast.add(AstNode::Assignment {
            left: lhs,
            right: rhs.expression,
        });
        self.ast.with_il(e, inst);
        let void = self.ts.known_type(KnownTypeCode::Void);
        self.ast.with_rr(e, Rr::Plain(void));
        Ok(TranslatedExpression {
            expression: e,
            ty: void,
        })
    }

    /// `DeconstructInstruction.IsConversionStLoc`: `stloc output(conv(ldloc input))`.
    fn conversion_st_loc(&self, inst: NodeId) -> Option<(VarId, VarId)> {
        let Inst::StLoc { var, value, .. } = *self.f.inst(inst) else {
            return None;
        };
        let Inst::Conv { arg, .. } = *self.f.inst(value) else {
            return None;
        };
        match *self.f.inst(arg) {
            Inst::LdLoc { var: input } | Inst::LdLoca { var: input } => Some((var, input)),
            _ => None,
        }
    }

    /// `ConstructTuple(matchInstruction)`: a `var` sub-pattern with a designator is its
    /// assignment target, one without `_`; any other sub-pattern a nested tuple.
    fn construct_tuple(
        &mut self,
        pattern: NodeId,
        positions: &mut Positions,
    ) -> Result<AstId, Unsupported> {
        let Inst::Match { sub_patterns, .. } = self.f.inst(pattern).clone() else {
            return Err("a deconstruction of another pattern".to_owned());
        };
        let mut elements = Vec::with_capacity(sub_patterns.len());
        for sub in sub_patterns {
            let Inst::Match {
                variable,
                sub_patterns: nested,
                is_deconstruct_call,
                is_deconstruct_tuple,
                check_type,
                check_not_null,
                ..
            } = self.f.inst(sub).clone()
            else {
                return Err("a deconstruction of another sub-pattern".to_owned());
            };
            let is_var = !check_type
                && !check_not_null
                && !is_deconstruct_call
                && !is_deconstruct_tuple
                && nested.is_empty();
            if !is_var {
                elements.push(self.construct_tuple(sub, positions)?);
                continue;
            }
            let v = self.f.var(variable);
            if v.load_count() + v.address_count() == 0 {
                elements.push(self.ast.add(AstNode::Identifier {
                    name: "_".to_owned(),
                }));
                continue;
            }
            let value = positions
                .conversions
                .iter()
                .find(|&&(input, _)| input == variable)
                .map_or(variable, |&(_, output)| output);
            let assignment = *positions
                .assignments
                .get(positions.assignment)
                .ok_or("a deconstruction with fewer assignments than variables")?;
            positions.assignment += 1;
            elements.push(self.assignment_target(assignment, value)?);
        }
        let tuple = self.ast.add(AstNode::DeconstructionTuple { elements });
        Ok(self.ast.with_il(tuple, pattern))
    }

    /// `ConstructAssignmentTarget(assignment, value)`: the left side of the assignment.
    fn assignment_target(
        &mut self,
        assignment: NodeId,
        value: VarId,
    ) -> Result<AstId, Unsupported> {
        let loads =
            |b: &Self, n: NodeId| matches!(*b.f.inst(n), Inst::LdLoc { var } if var == value);
        let assigns = match self.f.inst(assignment) {
            Inst::StLoc { value: v, .. } | Inst::StObj { value: v, .. } => loads(self, *v),
            Inst::Call(c) | Inst::CallVirt(c) => c.args.last().is_some_and(|&a| loads(self, a)),
            _ => false,
        };
        if !assigns {
            return Err("a deconstruction assignment of another kind".to_owned());
        }
        let e = self.translate(assignment, None)?;
        let AstNode::Assignment { left, .. } = *self.ast.node(e.expression) else {
            return Err("a deconstruction assignment that is no assignment".to_owned());
        };
        Ok(self.unwrap_child(e.expression, left).expression)
    }
}
