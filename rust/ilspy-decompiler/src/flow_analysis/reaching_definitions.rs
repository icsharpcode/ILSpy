//! `FlowAnalysis/ReachingDefinitionsVisitor.cs`: which stores of the analyzed variables can
//! reach each point. The state is a bit set over all stores (plus one "uninitialized" pseudo
//! store per variable) and a reachability bit.

use std::collections::HashMap;

use crate::il::ast::{Flags, Function, Inst, NodeId, VarId};
use crate::util::bitset::BitSet;

use super::data_flow::{self, Flow};

const REACHABLE_BIT: usize = 0;
const FIRST_STORE_INDEX: usize = 1;

/// `ReachingDefinitionsVisitor.State`.
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
        // The finally block runs after the branch, so the stores at its end reach the target.
        if self.is_reachable() {
            self.replace_with(finally_state);
        }
    }

    fn is_bottom(&self) -> bool {
        !self.bits.bit(REACHABLE_BIT)
    }

    fn replace_with_bottom(&mut self) {
        self.bits.clear_all();
    }
}

impl State {
    pub fn is_reachable(&self) -> bool {
        self.bits.bit(REACHABLE_BIT)
    }
}

/// The store tables of `ReachingDefinitionsVisitor`, built from the function before the
/// analysis runs.
pub struct ReachingDefinitions {
    /// `allStores`: index 0 is the reachability bit, then per analyzed variable its
    /// uninitialized pseudo store (`None`) followed by its stores in tree order.
    all_stores: Vec<Option<NodeId>>,
    store_index: HashMap<NodeId, usize>,
    /// Per variable index (`IndexInFunction`), its first store index; one extra entry ends
    /// the last range.
    first_store_index: Vec<usize>,
    analyzed: Vec<bool>,
    var_index: HashMap<VarId, usize>,
}

impl ReachingDefinitions {
    /// Prepares the analysis of the variables in `variables` (the function's variable list,
    /// whose positions are `IndexInFunction`) for which `analyzed` holds.
    pub fn new(f: &Function, variables: &[VarId], analyzed: Vec<bool>) -> (Self, Flow<State>) {
        let var_index: HashMap<VarId, usize> =
            variables.iter().enumerate().map(|(i, &v)| (v, i)).collect();
        let mut stores_by_var: Vec<Option<Vec<Option<NodeId>>>> =
            analyzed.iter().map(|&a| a.then(|| vec![None])).collect();
        let root = f.body.expect("function body");
        for inst in f.descendants_post(root) {
            if !f.has_direct_flag(inst, Flags::MAY_WRITE_LOCALS) {
                continue;
            }
            let var = match *f.inst(inst) {
                Inst::StLoc { var, .. } | Inst::TryCatchHandler { variable: var, .. } => var,
                ref other => panic!("{other:?} writes no known variable"),
            };
            if let Some(&vi) = var_index.get(&var)
                && let Some(stores) = &mut stores_by_var[vi]
            {
                stores.push(Some(inst));
            }
        }
        let mut all_stores = vec![None; FIRST_STORE_INDEX];
        let mut store_index = HashMap::new();
        let mut first_store_index = Vec::with_capacity(variables.len() + 1);
        for stores in &stores_by_var {
            first_store_index.push(all_stores.len());
            if let Some(stores) = stores {
                for (i, &s) in stores.iter().enumerate().skip(1) {
                    store_index.insert(s.expect("a real store"), all_stores.len() + i);
                }
                all_stores.extend(stores);
            }
        }
        first_store_index.push(all_stores.len());
        let mut initial = BitSet::new(all_stores.len());
        initial.set(REACHABLE_BIT);
        for (vi, &a) in analyzed.iter().enumerate() {
            if a {
                initial.set(first_store_index[vi]);
            }
        }
        let rd = ReachingDefinitions {
            all_stores,
            store_index,
            first_store_index,
            analyzed,
            var_index,
        };
        let flow = Flow::new(State { bits: initial }, Flags::MAY_WRITE_LOCALS);
        (rd, flow)
    }

    pub fn is_analyzed_variable(&self, var: VarId) -> bool {
        self.var_index.get(&var).is_some_and(|&i| self.analyzed[i])
    }

    /// `HandleStore`: the store replaces every other reaching store of its variable.
    pub fn handle_store(&self, flow: &mut Flow<State>, inst: NodeId, var: VarId) {
        if !(self.is_analyzed_variable(var) && flow.state.is_reachable()) {
            return;
        }
        let vi = self.var_index[&var];
        let (start, end) = (self.first_store_index[vi], self.first_store_index[vi + 1]);
        flow.state.bits.clear_range(start, end);
        let si = self.store_index[&inst];
        flow.state.bits.set(si);
        flow.current_state_on_exception.bits.set(si);
    }

    /// `GetStores`: the stores of `var` that reach `state`, in store order.
    pub fn stores(&self, state: &State, var: VarId) -> Vec<NodeId> {
        let vi = self.var_index[&var];
        let (mut start, end) = (
            self.first_store_index[vi] + 1,
            self.first_store_index[vi + 1],
        );
        let mut out = Vec::new();
        while start < end {
            let Some(next) = state.bits.next_set_bit(start, end) else {
                break;
            };
            out.push(self.all_stores[next].expect("a real store"));
            start = next + 1;
        }
        out
    }

    /// `IsPotentiallyUninitialized`.
    pub fn is_potentially_uninitialized(&self, state: &State, var: VarId) -> bool {
        state.bits.bit(self.first_store_index[self.var_index[&var]])
    }
}
