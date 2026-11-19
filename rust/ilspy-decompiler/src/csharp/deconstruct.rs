//! `ExpressionBuilder.VisitDeconstructInstruction`: `(a, (b, _)) = value`.

use crate::il::ast::{Inst, NodeId, VarId, VariableKind};
use crate::typesystem::known::KnownTypeCode;

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, TranslatedExpression, Unsupported};

/// The state of `ConstructTuple`: the next assignment to take.
struct Positions {
    assignments: Vec<NodeId>,
    assignment: usize,
    /// `conversionMapping`: the variable a conversion stores each input variable's value to.
    conversions: Vec<(VarId, VarId)>,
    /// `inits` and `initPos`: the init stores whose values replace the loads of their
    /// temporaries in the assignment targets, and the next one to take.
    inits: Vec<NodeId>,
    init: usize,
}

impl Builder<'_, '_, '_, '_> {
    /// `VisitDeconstructInstruction`: the tuple of the assignment targets assigned the
    /// deconstructed value, of type `void`.
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
        // shortcut: in a void lambda the enclosing call's overload resolution then needs a cast
        // of the lambda the managed builder does without (`Task.Run(() => { (a, b) = ...; })`:
        // the engine's best candidate is `Run<?>(Func<?>)`, the void lambda's unknown inferred
        // return type fixing `TResult`, where the managed one picks `Run(Action)`, although
        // inference, `Fix` and betterness are ported alike); find the difference before
        // lifting it. Lambdas that return a value translate as the managed builder does.
        if self.in_lambda
            && self.ts.kind(self.result_type) == crate::semantics::types::TypeKind::Void
        {
            return Err("a deconstruction in a void lambda".to_owned());
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
            inits: init,
            init: 0,
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
            elements.push(self.assignment_target(assignment, value, positions)?);
        }
        let tuple = self.ast.add(AstNode::DeconstructionTuple { elements });
        Ok(self.ast.with_il(tuple, pattern))
    }

    /// `ConstructAssignmentTarget(assignment, value)`: the left side of the assignment.
    fn assignment_target(
        &mut self,
        assignment: NodeId,
        value: VarId,
        positions: &mut Positions,
    ) -> Result<AstId, Unsupported> {
        let loads =
            |b: &Self, n: NodeId| matches!(*b.f.inst(n), Inst::LdLoc { var } if var == value);
        let (assigns, targets) = match self.f.inst(assignment).clone() {
            Inst::StLoc { value: v, .. } => (loads(self, v), Vec::new()),
            Inst::StObj {
                target, value: v, ..
            } => {
                let mut target = target;
                while let Inst::LdFlda { target: nested, .. } = *self.f.inst(target) {
                    target = nested;
                }
                (loads(self, v), vec![target])
            }
            Inst::Call(c) | Inst::CallVirt(c) => match c.args.split_last() {
                Some((&last, rest)) => (loads(self, last), rest.to_vec()),
                None => (false, Vec::new()),
            },
            _ => (false, Vec::new()),
        };
        if !assigns {
            return Err("a deconstruction assignment of another kind".to_owned());
        }
        // `ReplaceAssignmentTarget`: the load of an init temporary takes the init's value.
        for target in targets {
            let Inst::LdLoc { var } = *self.f.inst(target) else {
                continue;
            };
            if self.f.var(var).kind != VariableKind::DeconstructionInitTemporary {
                continue;
            }
            let init = *positions
                .inits
                .get(positions.init)
                .ok_or("a deconstruction with fewer init stores than temporaries")?;
            let Inst::StLoc {
                value: init_value, ..
            } = *self.f.inst(init)
            else {
                return Err("a deconstruction init of another kind".to_owned());
            };
            // shortcut: `translate` takes the init's value for the load, but builder code that
            // inspects an argument's instruction directly still sees the load; replace it in
            // the engine's ILAst (and send that tree) if a fixture shows the difference.
            self.replaced_loads.push((target, init_value));
            positions.init += 1;
        }
        let e = self.translate(assignment, None)?;
        let AstNode::Assignment { left, .. } = *self.ast.node(e.expression) else {
            return Err("a deconstruction assignment that is no assignment".to_owned());
        };
        Ok(self.unwrap_child(e.expression, left).expression)
    }
}
