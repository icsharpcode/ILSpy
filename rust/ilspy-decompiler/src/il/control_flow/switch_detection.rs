//! `IL/ControlFlow/SwitchDetection.cs`: the switch clean-up `ControlFlowSimplification` runs
//! (`SimplifySwitchInstruction`). The detection transform itself is a later pipeline step.

use std::collections::HashMap;

use crate::il::ast::{BinaryNumericOperator, Function, Inst, NodeId};
use crate::il::transforms::Context;
use crate::util::long_set::LongSet;

fn labels(f: &Function, section: NodeId) -> &LongSet {
    match f.inst(section) {
        Inst::SwitchSection { labels, .. } => labels,
        other => panic!("{other:?} is not a switch section"),
    }
}

fn section_body(f: &Function, section: NodeId) -> NodeId {
    match f.inst(section) {
        Inst::SwitchSection { body, .. } => *body,
        other => panic!("{other:?} is not a switch section"),
    }
}

fn set_labels(f: &mut Function, section: NodeId, new: LongSet) {
    if let Inst::SwitchSection { labels, .. } = f.inst_mut(section) {
        *labels = new;
    }
}

/// `SimplifySwitchInstruction`: merges the sections that branch to the same block, moves a
/// constant offset of the switch value into the labels and sorts the sections.
pub fn simplify_switch_instruction(f: &mut Function, block: NodeId, ctx: &Context<'_, '_, '_>) {
    // Blocks are basic blocks here, so a switch can only be the last instruction.
    let Some(&sw) = f.list(block).last() else {
        return;
    };
    if !matches!(f.inst(sw), Inst::Switch { .. }) {
        return;
    }
    let mut primary: HashMap<NodeId, NodeId> = HashMap::new();
    let mut kept = Vec::new();
    for &section in &f.list(sw).to_vec() {
        if let Some(target) = f.match_branch(section_body(f, section)) {
            if let Some(&first) = primary.get(&target) {
                let merged = labels(f, first).union_with(labels(f, section));
                set_labels(f, first, merged);
                continue;
            }
            primary.insert(target, section);
        }
        kept.push(section);
    }
    // `RemoveAll`.
    let kept: std::collections::HashSet<NodeId> = kept.into_iter().collect();
    f.retain(sw, |_, s| kept.contains(&s));
    adjust_labels(f, sw);
    sort_switch_sections(f, sw, ctx);
}

fn switch_value(f: &Function, sw: NodeId) -> NodeId {
    match f.inst(sw) {
        Inst::Switch { value, .. } => *value,
        other => panic!("{other:?} is not a switch"),
    }
}

/// `AdjustLabels`.
fn adjust_labels(f: &mut Function, sw: NodeId) {
    let value = switch_value(f, sw);
    let Inst::BinaryNumeric {
        op,
        check_overflow: false,
        left,
        right,
        ..
    } = *f.inst(value)
    else {
        return;
    };
    let Some(val) = f.match_ldc_i(right) else {
        return;
    };
    let offset = match op {
        BinaryNumericOperator::Add => val.wrapping_neg(),
        BinaryNumericOperator::Sub => val,
        _ => return,
    };
    f.replace(value, left);
    for section in f.list(sw).to_vec() {
        let moved = labels(f, section).add_offset(offset);
        set_labels(f, section, moved);
    }
}

/// `SortSwitchSections`: a stable sort, as `OrderBy`.
fn sort_switch_sections(f: &mut Function, sw: NodeId, ctx: &Context<'_, '_, '_>) {
    let first_label = |f: &Function, s: NodeId| labels(f, s).values().next().unwrap_or(0);
    let mut sections = f.list(sw).to_vec();
    if ctx.settings.sort_switch_sections {
        sections.sort_by_key(|&s| first_label(f, s));
    } else {
        // `int?` keys: sections that neither branch nor leave sort first.
        let start = |f: &Function, s: NodeId| {
            let body = section_body(f, s);
            match f.inst(body) {
                Inst::Branch { .. } => f.match_branch(body).map(|b| f.il_range(b).start),
                Inst::Leave { .. } => Some(f.il_range(body).start),
                _ => None,
            }
        };
        sections.sort_by_key(|&s| (start(f, s), first_label(f, s)));
    }
    f.set_list(sw, sections);
}
