//! `IL/ControlFlow/SymbolicExecution.cs`: evaluating instructions over a symbolic state
//! variable, for the state machine decompilers.

use crate::il::ast::{
    BinaryNumericOperator, ComparisonKind, Function, Inst, NodeId, VarId, VariableKind,
};
use crate::typesystem::compilation::ModuleId;
use crate::typesystem::resolve::FieldOrigin;
use crate::util::long_set::LongSet;

use super::switch_analysis::make_set_where_comparison_is_true;

/// `SymbolicAnalysisFailedException`: the state machine does not have the expected shape.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct SymbolicAnalysisFailed(pub String);

/// A field definition (`IField.MemberDefinition`) as its row.
pub type FieldDef = (ModuleId, u32);

/// The field definition of an `ldflda` / field operand, if it has one.
pub fn field_definition(origin: &FieldOrigin) -> Option<FieldDef> {
    match *origin {
        FieldOrigin::Def { module, row } => Some((module, row)),
        FieldOrigin::Fake => None,
    }
}

#[derive(Clone, Debug)]
pub enum SymbolicValue {
    Unknown,
    IntegerConstant(i32),
    /// `state + constant`.
    State(i32),
    This,
    /// A boolean that is true iff the state is in the set.
    StateInSet(LongSet),
}

impl SymbolicValue {
    /// `AsBool`: `if (state + c)` is `if (state != -c)`.
    pub fn as_bool(self) -> SymbolicValue {
        match self {
            SymbolicValue::State(c) => {
                SymbolicValue::StateInSet(LongSet::single(i64::from(c.wrapping_neg())).invert())
            }
            other => other,
        }
    }
}

/// `SymbolicEvaluationContext`.
pub struct SymbolicEvaluationContext {
    state_field: Option<FieldDef>,
    legacy_visual_basic: bool,
    state_variables: Vec<VarId>,
}

impl SymbolicEvaluationContext {
    pub fn new(state_field: Option<FieldDef>, legacy_visual_basic: bool) -> Self {
        SymbolicEvaluationContext {
            state_field,
            legacy_visual_basic,
            state_variables: Vec::new(),
        }
    }

    pub fn add_state_variable(&mut self, v: VarId) {
        if !self.state_variables.contains(&v) {
            self.state_variables.push(v);
        }
    }

    pub fn state_variables(&self) -> &[VarId] {
        &self.state_variables
    }

    pub fn eval(
        &self,
        f: &Function,
        ts: &mut crate::typesystem::types::TypeSystem<'_, '_>,
        inst: NodeId,
    ) -> SymbolicValue {
        use SymbolicValue::*;
        match *f.inst(inst) {
            Inst::BinaryNumeric {
                op: BinaryNumericOperator::Sub,
                check_overflow,
                left,
                right,
                ..
            } if self.legacy_visual_basic || !check_overflow => {
                let (l, r) = (self.eval(f, ts, left), self.eval(f, ts, right));
                match (l, r) {
                    (State(a), IntegerConstant(b)) => State(a.wrapping_sub(b)),
                    (IntegerConstant(a), IntegerConstant(b)) => IntegerConstant(a.wrapping_sub(b)),
                    _ => Unknown,
                }
            }
            Inst::Comp {
                kind,
                sign,
                left,
                right,
                ..
            } => {
                let (l, r) = (self.eval(f, ts, left), self.eval(f, ts, right));
                match (l, r) {
                    (State(c), IntegerConstant(v)) => {
                        // `state + c (kind) v`.
                        let true_sums = make_set_where_comparison_is_true(kind, i64::from(v), sign);
                        StateInSet(true_sums.add_offset(i64::from(c.wrapping_neg())))
                    }
                    (StateInSet(set), IntegerConstant(v)) => match kind {
                        ComparisonKind::Equality if v == 0 => StateInSet(set.invert()),
                        ComparisonKind::Inequality if v != 0 => StateInSet(set),
                        _ => Unknown,
                    },
                    _ => Unknown,
                }
            }
            Inst::LdLoc { var } => {
                let v = f.var(var);
                if self.state_variables.contains(&var) {
                    State(0)
                } else if v.kind == VariableKind::Parameter && v.index.is_some_and(|i| i < 0) {
                    This
                } else {
                    Unknown
                }
            }
            Inst::LdcI4 { value } => IntegerConstant(value),
            _ => {
                if let Some((target, field)) = f.match_ldfld(ts, inst) {
                    let def = field_definition(&field.field.origin);
                    if !matches!(self.eval(f, ts, target), This) {
                        return Unknown;
                    }
                    if def != self.state_field || self.state_field.is_none() {
                        return Unknown;
                    }
                    return State(0);
                }
                Unknown
            }
        }
    }
}
