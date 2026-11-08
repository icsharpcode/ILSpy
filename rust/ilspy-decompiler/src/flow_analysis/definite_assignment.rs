//! `FlowAnalysis/DefiniteAssignmentVisitor.cs`: which variables may be read before they are
//! written.
//!
//! Local functions do not exist in the ILAst yet, so their fixed-point iteration is not
//! needed.

use std::collections::HashMap;

use crate::il::ast::{Flags, Function, Inst, NodeId, VarId};
use crate::typesystem::members::RefKind;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::TypeSystem;
use crate::util::bitset::BitSet;

use super::data_flow::{self, Analysis, Flow};

/// Bit `i`: a path from the entry reaches here without writing variable `i`. The initial
/// state has every bit set, the bottom state none.
#[derive(Clone, Debug)]
pub struct State {
    bits: BitSet,
}

impl data_flow::State for State {
    fn less_than_or_equal(&self, other: &Self) -> bool {
        self.bits.is_subset_of(&other.bits)
    }

    fn replace_with(&mut self, other: &Self) {
        self.bits.replace_with(&other.bits);
    }

    fn join_with(&mut self, incoming: &Self) {
        self.bits.union_with(&incoming.bits);
    }

    fn trigger_finally(&mut self, finally_state: &Self) {
        // Definitely assigned after try-finally if assigned in the try or the finally block.
        self.bits.intersect_with(&finally_state.bits);
    }

    fn is_bottom(&self) -> bool {
        !self.bits.any()
    }

    fn replace_with_bottom(&mut self) {
        self.bits.clear_all();
    }
}

struct DefiniteAssignment<'t, 'c, 'a> {
    ts: &'t mut TypeSystem<'c, 'a>,
    var_index: HashMap<VarId, usize>,
    used_uninitialized: BitSet,
}

impl DefiniteAssignment<'_, '_, '_> {
    fn store(&self, flow: &mut Flow<State>, var: VarId) {
        if let Some(&i) = self.var_index.get(&var) {
            flow.state.bits.clear_range(i, i + 1);
        }
    }

    /// The parameter an argument is passed to is an `out` parameter.
    fn is_out_argument(&mut self, f: &Function, call: NodeId, arg_index: usize) -> bool {
        let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = f.inst(call) else {
            return false;
        };
        let first_param =
            usize::from(!c.method.method.is_static && !matches!(f.inst(call), Inst::NewObj(_)));
        let Some(index) = arg_index.checked_sub(first_param) else {
            return false;
        };
        match c.method.method.origin {
            MethodOrigin::Def(d) => self
                .ts
                .method_signature(d)
                .1
                .get(index)
                .is_some_and(|p| p.ref_kind == RefKind::Out),
            // Parameters decoded from a signature are never `out`.
            _ => false,
        }
    }
}

impl Analysis for DefiniteAssignment<'_, '_, '_> {
    type S = State;

    fn handle_store(&mut self, flow: &mut Flow<State>, _f: &Function, _inst: NodeId, var: VarId) {
        self.store(flow, var);
    }

    fn handle_load(&mut self, flow: &mut Flow<State>, f: &Function, inst: NodeId) {
        let var = f.variable_of(inst).expect("load");
        if let Some(&i) = self.var_index.get(&var)
            && flow.state.bits.bit(i)
        {
            self.used_uninitialized.set(i);
        }
    }

    /// `HandleCall`: arguments passed to `out` parameters need no initialization and are
    /// initialized after the whole call.
    fn visit_override(&mut self, flow: &mut Flow<State>, f: &Function, inst: NodeId) -> bool {
        let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = f.inst(inst) else {
            return false;
        };
        let mut out_vars = Vec::new();
        for (i, &arg) in c.args.iter().enumerate() {
            if let Inst::LdLoca { var } = *f.inst(arg)
                && self.is_out_argument(f, inst, i)
            {
                out_vars.push(var);
            } else {
                flow.visit(self, f, arg);
            }
        }
        for var in out_vars {
            self.store(flow, var);
        }
        true
    }
}

/// `IsPotentiallyUsedUninitialized` for each variable of `variables` (the function's variable
/// list), by position.
pub fn potentially_used_uninitialized(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    variables: &[VarId],
) -> BitSet {
    let var_index = variables.iter().enumerate().map(|(i, &v)| (v, i)).collect();
    let mut initial = BitSet::new(variables.len());
    for i in 0..variables.len() {
        initial.set(i);
    }
    let mut a = DefiniteAssignment {
        ts,
        var_index,
        used_uninitialized: BitSet::new(variables.len()),
    };
    let mut flow = Flow::new(
        State { bits: initial },
        Flags::MAY_READ_LOCALS | Flags::MAY_WRITE_LOCALS,
    );
    flow.visit(&mut a, f, f.body.expect("function body"));
    a.used_uninitialized
}
