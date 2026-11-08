//! `IL/ControlFlow/StateRangeAnalysis.cs`: symbolically executes state machine code to find
//! for which values of the state field each block is reachable.
//!
//! Assumption: no loops or backward jumps. The state is a symbolic variable that may appear
//! as `state + x`; every block gets the set of states it is reachable for, starting from
//! the universe at the first instruction and narrowed by the conditional jumps.

use std::collections::HashMap;

use crate::il::ast::{Function, Inst, MethodOperand, NodeId, VarId};
use crate::typesystem::members::MethodDefId;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::TypeSystem;
use crate::util::long_dict::LongDict;
use crate::util::long_set::LongSet;

use super::symbolic_execution::{
    FieldDef, SymbolicAnalysisFailed, SymbolicEvaluationContext, SymbolicValue, field_definition,
};

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Mode {
    IteratorMoveNext,
    IteratorDispose,
    AsyncMoveNext,
    AwaitInFinally,
}

/// `IMethod.MemberDefinition` as a dictionary key.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum MethodKey {
    Def(MethodDefId),
    /// A method without a definition, identified by its token.
    Token(u32),
}

impl MethodKey {
    pub fn of(m: &MethodOperand) -> MethodKey {
        match m.method.origin {
            MethodOrigin::Def(d) => MethodKey::Def(d),
            _ => MethodKey::Token(m.token),
        }
    }
}

/// An insertion-ordered map, as the managed `Dictionary` enumerates without removals.
#[derive(Clone, Debug)]
pub struct OrderedMap<K, V> {
    entries: Vec<(K, V)>,
    index: HashMap<K, usize>,
}

impl<K: Copy + Eq + std::hash::Hash, V> Default for OrderedMap<K, V> {
    fn default() -> Self {
        OrderedMap {
            entries: Vec::new(),
            index: HashMap::new(),
        }
    }
}

impl<K: Copy + Eq + std::hash::Hash, V> OrderedMap<K, V> {
    pub fn get(&self, k: &K) -> Option<&V> {
        self.index.get(k).map(|&i| &self.entries[i].1)
    }

    pub fn insert(&mut self, k: K, v: V) {
        match self.index.get(&k) {
            Some(&i) => self.entries[i].1 = v,
            None => {
                self.index.insert(k, self.entries.len());
                self.entries.push((k, v));
            }
        }
    }

    pub fn get_mut(&mut self, k: &K) -> Option<&mut V> {
        self.index.get(k).map(|&i| &mut self.entries[i].1)
    }

    pub fn contains_key(&self, k: &K) -> bool {
        self.index.contains_key(k)
    }

    /// `Remove`; the remaining entries keep their order.
    pub fn remove(&mut self, k: &K) {
        if let Some(i) = self.index.remove(k) {
            self.entries.remove(i);
            for j in self.index.values_mut() {
                if *j > i {
                    *j -= 1;
                }
            }
        }
    }

    pub fn iter(&self) -> impl Iterator<Item = &(K, V)> {
        self.entries.iter()
    }

    pub fn keys(&self) -> impl Iterator<Item = &K> {
        self.entries.iter().map(|(k, _)| k)
    }
}

pub struct StateRangeAnalysis {
    mode: Mode,
    state_field: Option<FieldDef>,
    legacy_visual_basic: bool,
    pub eval: SymbolicEvaluationContext,
    ranges: OrderedMap<NodeId, LongSet>,
    ranges_for_leave: OrderedMap<NodeId, LongSet>,
    /// Iterator Dispose(): per finally method, the states it is called for.
    pub finally_method_to_state_range: OrderedMap<MethodKey, (MethodOperand, LongSet)>,
    pub do_finally_bodies: Option<VarId>,
    pub skip_finally_bodies: Option<VarId>,
}

impl StateRangeAnalysis {
    pub fn new(
        mode: Mode,
        state_field: Option<FieldDef>,
        cached_state_var: Option<VarId>,
        legacy_visual_basic: bool,
    ) -> Self {
        let mut eval = SymbolicEvaluationContext::new(state_field, legacy_visual_basic);
        if let Some(v) = cached_state_var {
            eval.add_state_variable(v);
        }
        StateRangeAnalysis {
            mode,
            state_field,
            legacy_visual_basic,
            eval,
            ranges: OrderedMap::default(),
            ranges_for_leave: OrderedMap::default(),
            finally_method_to_state_range: OrderedMap::default(),
            do_finally_bodies: None,
            skip_finally_bodies: None,
        }
    }

    /// `CreateNestedAnalysis`: the same settings and cached state variables, fresh results.
    pub fn create_nested(&self) -> StateRangeAnalysis {
        let mut sra =
            StateRangeAnalysis::new(self.mode, self.state_field, None, self.legacy_visual_basic);
        sra.do_finally_bodies = self.do_finally_bodies;
        sra.skip_finally_bodies = self.skip_finally_bodies;
        for &v in self.eval.state_variables() {
            sra.eval.add_state_variable(v);
        }
        sra
    }

    pub fn cached_state_vars(&self) -> &[VarId] {
        self.eval.state_variables()
    }

    /// `AssignStateRanges`: the states for which the end of `inst` is reached (a subset of
    /// `range`; empty for unsupported instructions).
    pub fn assign_state_ranges(
        &mut self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
        range: LongSet,
    ) -> Result<LongSet, SymbolicAnalysisFailed> {
        match f.inst(inst) {
            Inst::BlockContainer { blocks, .. } => {
                self.add_state_range(blocks[0], range);
                for &block in blocks {
                    // No jumps to blocks already processed are assumed.
                    if let Some(r) = self.ranges.get(&block).cloned() {
                        self.assign_state_ranges(f, ts, block, r)?;
                    }
                }
                // `leave` edges are not tracked: conservatively empty.
                return Ok(LongSet::empty());
            }
            Inst::Block { instructions, .. } => {
                let mut range = range;
                for &i in instructions {
                    if range.is_empty() {
                        break;
                    }
                    range = self.assign_state_ranges(f, ts, i, range)?;
                }
                return Ok(range);
            }
            &Inst::TryFinally {
                try_block,
                finally_block,
            } if self.mode == Mode::IteratorDispose => {
                let after_try = self.assign_state_ranges(f, ts, try_block, range.clone())?;
                let after_finally = self.assign_state_ranges(f, ts, finally_block, range)?;
                return Ok(after_try.intersect_with(&after_finally));
            }
            Inst::Switch {
                value, sections, ..
            } => {
                if let SymbolicValue::State(c) = self.eval.eval(f, ts, *value) {
                    let mut exits = Vec::new();
                    for &section in sections {
                        let Inst::SwitchSection { labels, body, .. } = f.inst(section) else {
                            unreachable!("switch sections");
                        };
                        // `switch (state + c)` takes `case v` iff `state == v - c`.
                        let effective = labels.add_offset(i64::from(c).wrapping_neg());
                        let result = self.assign_state_ranges(
                            f,
                            ts,
                            *body,
                            range.intersect_with(&effective),
                        )?;
                        exits.extend(result.intervals().iter().copied());
                    }
                    return Ok(LongSet::from_intervals(exits));
                }
            }
            &Inst::If {
                condition,
                true_inst,
                false_inst,
                ..
            } => {
                if let SymbolicValue::StateInSet(true_ranges) =
                    self.eval.eval(f, ts, condition).as_bool()
                {
                    let after_true = self.assign_state_ranges(
                        f,
                        ts,
                        true_inst,
                        range.intersect_with(&true_ranges),
                    )?;
                    let after_false = self.assign_state_ranges(
                        f,
                        ts,
                        false_inst,
                        range.except_with(&true_ranges),
                    )?;
                    return Ok(after_true.union_with(&after_false));
                }
            }
            Inst::Branch { .. } => {
                self.add_state_range(f.branch_target(inst), range);
                return Ok(LongSet::empty());
            }
            Inst::Leave {
                target: Some(c), ..
            } if self.mode == Mode::AwaitInFinally => {
                let c = *c;
                let merged = match self.ranges_for_leave.get(&c) {
                    Some(existing) => range.union_with(existing),
                    None => range,
                };
                self.ranges_for_leave.insert(c, merged);
                return Ok(LongSet::empty());
            }
            Inst::Nop { .. } => return Ok(range),
            &Inst::StLoc { var, value, .. } => {
                // Pre-Roslyn async/await and Mono iterators use generated flag variables.
                if Some(var) == self.do_finally_bodies || Some(var) == self.skip_finally_bodies {
                    return Ok(range);
                }
                if let SymbolicValue::State(0) = self.eval.eval(f, ts, value) {
                    self.eval.add_state_variable(var);
                    return Ok(range);
                }
            }
            Inst::Call(call) if self.mode == Mode::IteratorDispose => {
                // A finally method; usually in a finally block, but e.g. foreach over an array
                // puts the call outside.
                let key = MethodKey::of(&call.method);
                assert!(
                    !self.finally_method_to_state_range.contains_key(&key),
                    "a finally method is called twice in Dispose()"
                );
                self.finally_method_to_state_range
                    .insert(key, (call.method.clone(), range));
                return Ok(LongSet::empty());
            }
            Inst::StObj { .. } if self.mode == Mode::IteratorMoveNext => {
                if let Some((target, field, value)) = f.match_stfld(inst)
                    && f.match_ld_this(target)
                    && field_definition(&field.field.origin) == self.state_field
                    && self.state_field.is_some()
                    && f.match_ldc_i4(value) == Some(-1)
                {
                    // Mono resets the state field in MoveNext(): not user code.
                    return Ok(range);
                }
            }
            Inst::StObj { .. } if self.mode == Mode::IteratorDispose => {
                if let Some((target, field, value)) = f.match_stfld(inst)
                    && f.match_ld_this(target)
                {
                    // Roslyn 4.13 marks the iterator as disposed and clears hoisted locals.
                    if field_definition(&field.field.origin) == self.state_field
                        && self.state_field.is_some()
                        && f.match_ldc_i4(value) == Some(-2)
                    {
                        return Ok(range);
                    }
                    if f.match_default_or_null_or_zero(value) {
                        return Ok(range);
                    }
                }
            }
            _ => {}
        }
        // User code: stop.
        if self.mode == Mode::IteratorDispose && f.match_return(inst).is_none() {
            return Err(SymbolicAnalysisFailed(
                "Unexpected instruction in Iterator.Dispose()".into(),
            ));
        }
        Ok(LongSet::empty())
    }

    fn add_state_range(&mut self, block: NodeId, range: LongSet) {
        let merged = match self.ranges.get(&block) {
            Some(existing) => range.union_with(existing),
            None => range,
        };
        self.ranges.insert(block, merged);
    }

    /// `GetBlockStateSetMapping`: states to blocks. Blocks outside `container` come first,
    /// then the container's blocks from the last; earlier entries win.
    pub fn block_state_set_mapping(&self, f: &Function, container: NodeId) -> LongDict<NodeId> {
        let mut entries = Vec::new();
        for (block, states) in self.ranges.iter() {
            if f.node(*block).parent != Some(container) {
                entries.push((states.clone(), *block));
            }
        }
        for &block in f.list(container).iter().rev() {
            if let Some(states) = self.ranges.get(&block) {
                entries.push((states.clone(), block));
            }
        }
        LongDict::new(entries)
    }

    /// `GetBlockStateSetMappingForLeave`.
    pub fn block_state_set_mapping_for_leave(&self) -> LongDict<NodeId> {
        LongDict::new(self.ranges_for_leave.iter().map(|(c, s)| (s.clone(), *c)))
    }
}
