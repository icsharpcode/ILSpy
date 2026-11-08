//! `IL/ControlFlow/SwitchAnalysis.cs`: reconstructs a switch from the tree of conditional
//! branches (and IL switches) that test one integer variable, as the C# compiler emits for
//! non-contiguous case labels. It expects basic blocks.

use std::collections::HashMap;

use crate::il::ast::{BinaryNumericOperator, ComparisonKind, Function, Inst, NodeId, VarId};
use crate::il::types::Sign;
use crate::util::interval::LongInterval;
use crate::util::long_set::LongSet;

#[derive(Default)]
pub struct SwitchAnalysis {
    /// The variable the switch tests; the first match of an analysis fixes it.
    switch_var: Option<VarId>,
    /// Whether an analyzed block ended in an IL switch.
    pub contains_il_switch: bool,
    /// The detected sections: the values and the instruction they run.
    pub sections: Vec<(LongSet, NodeId)>,
    /// De-duplicates sections that branch to the same block.
    target_block_to_section_index: HashMap<NodeId, usize>,
    /// De-duplicates sections that are value-less leaves of the same container.
    target_container_to_section_index: HashMap<NodeId, usize>,
    /// Blocks that can go once the tail of the root block is a switch.
    pub inner_blocks: Vec<NodeId>,
    root_block: Option<NodeId>,
    pub allow_unreachable_cases: bool,
}

impl SwitchAnalysis {
    /// The variable of the switch, after a successful `analyze_block`.
    pub fn switch_variable(&self) -> VarId {
        self.switch_var.expect("a successful analysis")
    }

    pub fn root(&self) -> NodeId {
        self.root_block.expect("an analyzed block")
    }

    /// `AnalyzeBlock`: whether the last two instructions of `block` can become a switch.
    pub fn analyze_block(&mut self, f: &Function, block: NodeId) -> bool {
        self.switch_var = None;
        self.root_block = Some(block);
        self.target_block_to_section_index.clear();
        self.target_container_to_section_index.clear();
        self.sections.clear();
        self.inner_blocks.clear();
        self.contains_il_switch = false;
        self.analyze(f, block, &LongSet::universe(), true)
    }

    /// `AnalyzeBlock(block, inputValues, tailOnly)`: `input` are the values the variable can
    /// have on entry; `tail_only` analyzes only the last two instructions.
    fn analyze(&mut self, f: &Function, block: NodeId, input: &LongSet, tail_only: bool) -> bool {
        let list = f.list(block);
        let Some(&last) = list.last() else {
            // A block SwitchDetection already emptied.
            return false;
        };
        if !tail_only {
            let root = self.root();
            // Only if-structures that form a tree, in the root's container.
            if f.node(block).incoming_edge_count != 1 || block == root {
                return false;
            }
            if f.node(block).parent != f.node(root).parent {
                return false;
            }
        }
        let n = list.len();
        let true_values;
        if n >= 2
            && let Some((condition, true_inst)) = f.match_if_instruction(list[n - 2])
            && let Some(values) = self.analyze_condition(f, condition)
        {
            if !(tail_only || n == 2) {
                return false;
            }
            let values = values.intersect_with(input);
            if values.set_equals(input) || values.is_empty() {
                return false;
            }
            if let Some(target) = f.match_branch(true_inst)
                && self.analyze(f, target, &values, false)
            {
                self.inner_blocks.push(target);
            } else {
                self.add_section(f, values.clone(), true_inst);
            }
            true_values = values;
        } else if matches!(f.inst(last), Inst::Switch { .. }) {
            if !(tail_only || n == 1) {
                return false;
            }
            if self.analyze_switch(f, last, input) {
                self.contains_il_switch = true;
                return true;
            }
            return false;
        } else {
            return false;
        }
        let remaining = input.except_with(&true_values);
        if let Some(target) = f.match_branch(last)
            && self.analyze(f, target, &remaining, false)
        {
            self.inner_blocks.push(target);
        } else {
            self.add_section(f, remaining, last);
        }
        true
    }

    fn analyze_switch(&mut self, f: &Function, sw: NodeId, input: &LongSet) -> bool {
        let Inst::Switch {
            value,
            ref sections,
            ..
        } = *f.inst(sw)
        else {
            unreachable!("a switch");
        };
        let offset = if self.match_switch_var(f, value) {
            0
        } else if let Inst::BinaryNumeric {
            op,
            check_overflow,
            left,
            right,
            ..
        } = *f.inst(value)
        {
            if check_overflow {
                return false;
            }
            if self.match_switch_var(f, left)
                && let Some(val) = f.match_ldc_i(right)
            {
                match op {
                    BinaryNumericOperator::Add => val.wrapping_neg(),
                    BinaryNumericOperator::Sub => val,
                    _ => return false,
                }
            } else {
                return false;
            }
        } else {
            return false;
        };
        for &section in sections {
            let Inst::SwitchSection {
                ref labels, body, ..
            } = *f.inst(section)
            else {
                unreachable!("switch sections");
            };
            let values = labels.add_offset(offset).intersect_with(input);
            if !self.allow_unreachable_cases && values.is_empty() {
                return false;
            }
            if values.count() > 1
                && let Some(target) = f.match_branch(body)
                && self.analyze(f, target, &values, false)
            {
                self.inner_blocks.push(target);
            } else {
                self.add_section(f, values, body);
            }
        }
        true
    }

    /// `AddSection`: a branch (or value-less leave) joins the section with the same target.
    fn add_section(&mut self, f: &Function, values: LongSet, inst: NodeId) {
        if values.is_empty() {
            return;
        }
        let (map, target) = if let Some(block) = f.match_branch(inst) {
            (&mut self.target_block_to_section_index, block)
        } else if let Some((container, value)) = f.match_leave(inst)
            && f.match_nop(value)
        {
            (&mut self.target_container_to_section_index, container)
        } else {
            self.sections.push((values, inst));
            return;
        };
        if let Some(&i) = map.get(&target) {
            self.sections[i] = (self.sections[i].0.union_with(&values), inst);
        } else {
            map.insert(target, self.sections.len());
            self.sections.push((values, inst));
        }
    }

    /// `MatchSwitchVar`: an `ldloc` of the switch variable, which the first match fixes.
    fn match_switch_var(&mut self, f: &Function, inst: NodeId) -> bool {
        let Some(v) = f.match_ldloc(inst) else {
            return false;
        };
        match self.switch_var {
            Some(s) => s == v,
            None => {
                self.switch_var = Some(v);
                true
            }
        }
    }

    /// `MatchSwitchVar(inst, out sub)`: the variable, or the variable minus a constant.
    fn match_switch_var_sub(&mut self, f: &Function, inst: NodeId) -> Option<i64> {
        if let Inst::BinaryNumeric {
            op: BinaryNumericOperator::Sub,
            check_overflow: false,
            left,
            right,
            ..
        } = *f.inst(inst)
            && let Some(sub) = f.match_ldc_i(right)
        {
            return self.match_switch_var(f, left).then_some(sub);
        }
        self.match_switch_var(f, inst).then_some(0)
    }

    /// `AnalyzeCondition`: the values of the variable for which `condition` is true.
    fn analyze_condition(&mut self, f: &Function, condition: NodeId) -> Option<LongSet> {
        if let Inst::Comp {
            kind,
            sign,
            left,
            right,
            ..
        } = *f.inst(condition)
            && let Some(sub) = self.match_switch_var_sub(f, left)
            && let Some(val) = f.match_ldc_i(right)
        {
            Some(make_set_where_comparison_is_true(kind, val, sign).add_offset(sub))
        } else if self.match_switch_var(f, condition) {
            Some(LongSet::single(0).invert())
        } else if let Some(arg) = f.match_logic_not(condition) {
            self.analyze_condition(f, arg).map(|values| values.invert())
        } else {
            None
        }
    }
}

/// `MakeSetWhereComparisonIsTrue`: the values `x` for which `x kind val` holds.
pub fn make_set_where_comparison_is_true(kind: ComparisonKind, val: i64, sign: Sign) -> LongSet {
    match kind {
        ComparisonKind::Equality => LongSet::single(val),
        ComparisonKind::Inequality => LongSet::single(val).invert(),
        ComparisonKind::LessThan => greater_than_or_equal(val, sign).invert(),
        ComparisonKind::LessThanOrEqual => less_than_or_equal(val, sign),
        ComparisonKind::GreaterThan => less_than_or_equal(val, sign).invert(),
        ComparisonKind::GreaterThanOrEqual => greater_than_or_equal(val, sign),
    }
}

fn greater_than_or_equal(val: i64, sign: Sign) -> LongSet {
    if sign == Sign::Signed {
        return LongSet::from_interval(LongInterval::inclusive(val, i64::MAX));
    }
    if val >= 0 {
        // val..=ulong.MaxValue as signed longs is two ranges.
        LongSet::from_interval(LongInterval::inclusive(val, i64::MAX))
            .union_with(&LongSet::from_interval(LongInterval::new(i64::MIN, 0)))
    } else {
        LongSet::from_interval(LongInterval::new(val, 0))
    }
}

fn less_than_or_equal(val: i64, sign: Sign) -> LongSet {
    if sign == Sign::Signed {
        return LongSet::from_interval(LongInterval::inclusive(i64::MIN, val));
    }
    if val >= 0 {
        LongSet::from_interval(LongInterval::inclusive(0, val))
    } else {
        // 0..=(ulong)val as signed longs is two ranges.
        LongSet::from_interval(LongInterval::inclusive(0, i64::MAX)).union_with(
            &LongSet::from_interval(LongInterval::inclusive(i64::MIN, val)),
        )
    }
}
