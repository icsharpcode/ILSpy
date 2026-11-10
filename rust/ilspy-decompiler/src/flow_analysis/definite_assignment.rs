//! `FlowAnalysis/DefiniteAssignmentVisitor.cs`: which variables may be read before they are
//! written.

use std::collections::{HashMap, HashSet};

use crate::il::ast::{Flags, Function, Inst, MethodOperand, NodeId, VarId};
use crate::typesystem::members::{MethodDefId, RefKind};
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::TypeSystem;
use crate::util::bitset::BitSet;

use super::data_flow::{self, Analysis, Flow, State as _};

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
    /// `stateOfLocalFunctionUse`, by the local function's method.
    local_function_use: HashMap<MethodDefId, State>,
    /// `localFunctionsNeedingAnalysis`.
    needing_analysis: HashSet<MethodDefId>,
}

impl DefiniteAssignment<'_, '_, '_> {
    /// `HandleLocalFunctionUse`: the state at a use of a local function is (joined into) its
    /// state on entry.
    fn local_function_use(&mut self, flow: &Flow<State>, method: &MethodOperand) {
        let Some(l) = &method.local_function else {
            return;
        };
        match self.local_function_use.get_mut(&l.method) {
            Some(entry) => {
                if !flow.state.less_than_or_equal(entry) {
                    entry.join_with(&flow.state);
                    self.needing_analysis.insert(l.method);
                }
            }
            None => {
                self.local_function_use.insert(l.method, flow.state.clone());
                self.needing_analysis.insert(l.method);
            }
        }
    }

    /// `VisitILFunction`: a lambda's body is visited where it is declared (its captured
    /// variables must be assigned there), then the state before it is restored; local
    /// functions are visited in the states of their uses until nothing changes.
    fn visit_function(
        &mut self,
        flow: &mut Flow<State>,
        f: &Function,
        body: NodeId,
        local_functions: &[NodeId],
    ) {
        let before = flow.state.clone();
        let before_on_exception = flow.current_state_on_exception.clone();
        flow.visit(self, f, body);
        loop {
            let mut changed = false;
            for &lf in local_functions {
                let Inst::Function {
                    read: Some(read), ..
                } = f.inst(lf)
                else {
                    continue;
                };
                if !self.needing_analysis.remove(&read.method) {
                    continue;
                }
                let entry = self.local_function_use[&read.method].clone();
                flow.state.replace_with(&entry);
                flow.current_state_on_exception.replace_with(&entry);
                flow.visit(self, f, lf);
                changed = true;
            }
            if !changed {
                break;
            }
        }
        flow.current_state_on_exception = before_on_exception;
        flow.state = before;
    }

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
        match f.inst(inst) {
            Inst::Function {
                body,
                local_functions,
                ..
            } => {
                let (body, local_functions) = (*body, local_functions.clone());
                self.visit_function(flow, f, body, &local_functions);
                return true;
            }
            Inst::LdFtn { method } => {
                self.local_function_use(flow, method);
                return true;
            }
            _ => {}
        }
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
        self.local_function_use(flow, &c.method);
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
    potentially_used_uninitialized_in(f, ts, variables, None)
}

/// `potentially_used_uninitialized` of a nested function (`function`), or of the top-level
/// function with its local functions (`None`).
pub fn potentially_used_uninitialized_in(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    variables: &[VarId],
    function: Option<NodeId>,
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
        local_function_use: HashMap::new(),
        needing_analysis: HashSet::new(),
    };
    let mut flow = Flow::new(
        State { bits: initial },
        Flags::MAY_READ_LOCALS | Flags::MAY_WRITE_LOCALS,
    );
    match function {
        Some(n) => flow.visit(&mut a, f, n),
        None => {
            let body = f.body.expect("function body");
            let local_functions = f.local_functions.clone();
            a.visit_function(&mut flow, f, body, &local_functions);
        }
    }
    a.used_uninitialized
}
