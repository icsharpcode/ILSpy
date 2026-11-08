//! `IL/Transforms/SwitchOnStringTransform.cs`: the switch-on-string patterns of the C#
//! compilers (cascading ifs, the C# 1 `IsInterned` chain, the legacy `Dictionary` and
//! `Hashtable` lookups, Roslyn's string hash and length-and-char switches) become a switch
//! over `string.to.int`.

use std::collections::HashMap;

use crate::il::ast::{
    ComparisonKind, FieldOperand, Function, Inst, NodeId, NopKind, VarId, VariableKind,
};
use crate::il::control_flow::switch_analysis::SwitchAnalysis;
use crate::il::control_flow::switch_detection::{
    default_section, inline_switch_expression_default_case_throw_helper,
    is_switch_expression_throw_helper_block, match_compute_string_or_read_only_span_hash_call,
    simplify_switch_instruction,
};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::il_inlining::known;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::ResolvedField;
use crate::typesystem::types::TypeId;
use crate::util::interval::LongInterval;
use crate::util::long_set::LongSet;

/// A string value; `None` is null.
type Str = Option<Vec<u16>>;

/// What a case runs: a block (to branch to) or an instruction (a leave).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Target {
    Block(NodeId),
    Inst(NodeId),
}

/// An entry of `HashtableInitializer`, by its field.
struct HashtableInfo {
    field: ResolvedField,
    labels: Vec<(Str, i32)>,
    jump_to_next: Option<NodeId>,
    containing_block: NodeId,
    previous: NodeId,
    next: NodeId,
    transformed: bool,
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.switch_statement_on_string {
        return;
    }
    let f = &mut r.function;
    let body = f.body.expect("body");
    let entry = f.list(body)[0];
    let mut t = Transform {
        hashtables: Vec::new(),
        analysis: SwitchAnalysis::default(),
    };
    t.scan_hashtable_initializer_blocks(f, ctx, entry);
    let mut changed_containers: Vec<NodeId> = Vec::new();
    let blocks: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect();
    for block in blocks {
        if f.node(block).incoming_edge_count == 0 {
            continue;
        }
        let mut changed = false;
        let mut i = f.list(block).len() as isize - 1;
        while i >= 0 {
            let mut u = i as usize;
            if t.simplify_cascading_if_statements(f, ctx, block, &mut u)
                || t.simplify_csharp1_cascading_if_statements(f, ctx, block, &mut u)
                || t.match_legacy_switch_on_string_with_hashtable(f, ctx, block, &mut u)
                || t.match_legacy_switch_on_string_with_dict(f, ctx, block, &mut u)
                || t.match_roslyn_switch_on_string(f, ctx, block, &mut u)
                || t.match_roslyn_switch_on_string_using_length_and_char(f, ctx, block, u)
            {
                changed = true;
            }
            i = u as isize - 1;
        }
        if !changed {
            continue;
        }
        simplify_switch_instruction(f, block, ctx);
        inline_switch_expression_default_case_throw_helper(f, ctx, block);
        if let Some(container) = f.node(block).parent
            && matches!(f.inst(container), Inst::BlockContainer { .. })
            && !changed_containers.contains(&container)
        {
            changed_containers.push(container);
        }
    }
    // The transformed hashtable initializers go from the entry point.
    let mut omitted: HashMap<NodeId, NodeId> = HashMap::new();
    for info in &t.hashtables {
        if !info.transformed {
            continue;
        }
        let actual = omitted
            .get(&info.previous)
            .copied()
            .unwrap_or(info.previous);
        if let Some(jump) = info.jump_to_next {
            let list = f.list(actual);
            let second_to_last = list[list.len() - 2];
            f.replace(second_to_last, jump);
        }
        let last = *f.list(actual).last().expect("instructions");
        let br = branch(f, info.next);
        f.replace(last, br);
        omitted
            .entry(info.containing_block)
            .or_insert(info.previous);
        if !changed_containers.contains(&body) {
            changed_containers.push(body);
        }
    }
    // With all initializers gone, the initial null check goes too.
    let entry = f.list(body)[0];
    if !t.hashtables.is_empty()
        && omitted.len() == t.hashtables.len()
        && f.list(entry).len() == 2
        && let Inst::If { true_inst, .. } = *f.inst(f.list(entry)[0])
        && let Some(begin) = f.match_branch(true_inst)
        && f.match_branch(f.list(entry)[1]) == Some(begin)
    {
        f.remove_at(entry, 0);
    }
    for container in changed_containers {
        sort_blocks(f, container);
    }
}

fn branch(f: &mut Function, block: NodeId) -> NodeId {
    f.add(Inst::Branch {
        target: crate::il::ast::BranchTarget::Block(block),
    })
}

fn leave(f: &mut Function, container: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Leave {
        target: Some(container),
        value: nop,
    })
}

fn section(f: &mut Function, labels: LongSet, body: NodeId) -> NodeId {
    f.add(Inst::SwitchSection {
        labels,
        body,
        has_null_label: false,
        is_compiler_generated_default_section: false,
    })
}

/// The labels outside `0..count`.
fn default_labels(count: usize) -> LongSet {
    LongSet::from_interval(LongInterval::new(0, count as i64)).invert()
}

/// A branch to a block target, or the instruction itself.
fn body_of(f: &mut Function, target: Target) -> NodeId {
    match target {
        Target::Block(b) => branch(f, b),
        Target::Inst(i) => i,
    }
}

fn switch_of(f: &mut Function, value: NodeId, sections: Vec<NodeId>) -> NodeId {
    f.add(Inst::Switch {
        value,
        sections,
        ty: None,
        is_lifted: false,
    })
}

fn string_to_int(f: &mut Function, arg: NodeId, map: Vec<(Str, i32)>, ty: TypeId) -> NodeId {
    f.add(Inst::StringToInt { arg, map, ty })
}

/// `ArrayToDictionary`: the values with their positions.
fn indexed(values: impl IntoIterator<Item = Str>) -> Vec<(Str, i32)> {
    values
        .into_iter()
        .enumerate()
        .map(|(i, v)| (v, i as i32))
        .collect()
}

/// `InstructionCollection.RemoveRange`.
fn remove_range(f: &mut Function, block: NodeId, index: usize, count: usize) {
    for _ in 0..count {
        f.remove_at(block, index);
    }
}

fn is_null_or_empty(s: &Str) -> bool {
    s.as_ref().is_none_or(|v| v.is_empty())
}

fn is_known(ctx: &mut Context<'_, '_, '_>, ty: TypeId, code: KnownTypeCode) -> bool {
    known(ctx, ty) == Some(code)
}

fn declaring_is(ctx: &mut Context<'_, '_, '_>, ty: Option<TypeId>, code: KnownTypeCode) -> bool {
    ty.is_some_and(|t| is_known(ctx, t, code))
}

fn declaring_full_name(ctx: &Context<'_, '_, '_>, ty: Option<TypeId>) -> Option<String> {
    ty.map(|t| ctx.ts.full_name(t))
}

fn same_field(a: &ResolvedField, b: &ResolvedField) -> bool {
    a.origin == b.origin && a.declaring == b.declaring
}

fn match_ldstr(f: &Function, id: NodeId) -> Option<Vec<u16>> {
    match f.inst(id) {
        Inst::LdStr { value } => Some(value.clone()),
        _ => None,
    }
}

fn match_ldloca(f: &Function, id: NodeId) -> Option<VarId> {
    match *f.inst(id) {
        Inst::LdLoca { var } => Some(var),
        _ => None,
    }
}

fn is_ldnull(f: &Function, id: NodeId) -> bool {
    matches!(f.inst(id), Inst::LdNull)
}

/// `MatchLeave(out BlockContainer)`: a value-less leave.
fn match_leave_nop(f: &Function, id: NodeId) -> Option<NodeId> {
    f.match_leave(id)
        .filter(|&(_, v)| f.match_nop(v))
        .map(|(c, _)| c)
}

fn second_to_last(f: &Function, block: NodeId) -> Option<NodeId> {
    let list = f.list(block);
    (list.len() >= 2).then(|| list[list.len() - 2])
}

/// `ILVariableEqualityComparer`.
fn variables_equal(f: &Function, x: VarId, y: VarId) -> bool {
    if x == y {
        return true;
    }
    let (a, b) = (f.var(x), f.var(y));
    if matches!(a.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
        || matches!(b.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
    {
        return false;
    }
    if a.kind != b.kind {
        return false;
    }
    if a.index.is_some() {
        a.index == b.index
    } else if let Some(fa) = &a.state_machine_field {
        b.state_machine_field
            .as_ref()
            .is_some_and(|fb| same_field(&fa.field, &fb.field))
    } else {
        false
    }
}

/// `MatchLdObj`: the target and the type.
fn match_ldobj(f: &Function, id: NodeId) -> Option<(NodeId, TypeId)> {
    match *f.inst(id) {
        Inst::LdObj { target, ty, .. } => Some((target, ty.ty)),
        _ => None,
    }
}

fn match_ldsflda(f: &Function, id: NodeId) -> Option<&FieldOperand> {
    match f.inst(id) {
        Inst::LdsFlda { field } => Some(field),
        _ => None,
    }
}

struct Transform {
    hashtables: Vec<HashtableInfo>,
    analysis: SwitchAnalysis,
}

impl Transform {
    fn hashtable_index(&self, field: &ResolvedField) -> Option<usize> {
        self.hashtables
            .iter()
            .position(|h| same_field(&h.field, field))
    }

    /// `ScanHashtableInitializerBlocks`: the chain of compiler-generated `Hashtable`
    /// initializers at the start of the method.
    fn scan_hashtable_initializer_blocks(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        entry: NodeId,
    ) {
        let list = f.list(entry);
        if list.len() != 2 {
            return;
        }
        // if (comp(volatile.ldobj Hashtable(ldsflda field) != ldnull)) br switchHead
        // br tableInitBlock
        let Some((condition, branch_to_switch_head)) = f.match_if_instruction(list[0]) else {
            return;
        };
        let Some(mut table_init) = f.match_branch(list[1]) else {
            return;
        };
        let Some((mut dict_field, dict_type)) =
            self.match_null_checked_hashtable(f, ctx, condition)
        else {
            return;
        };
        let Some(switch_head) = f.match_branch(branch_to_switch_head) else {
            return;
        };
        if f.node(table_init).incoming_edge_count != 1 || f.list(table_init).len() < 3 {
            return;
        }
        let mut previous = entry;
        while let Some((values, after)) =
            extract_string_values_from_init_block(f, ctx, table_init, dict_type, &dict_field, true)
        {
            let next_head =
                second_to_last(f, table_init).filter(|&i| matches!(f.inst(i), Inst::If { .. }));
            if self.hashtable_index(&dict_field).is_none() {
                self.hashtables.push(HashtableInfo {
                    field: dict_field.clone(),
                    labels: values,
                    jump_to_next: next_head,
                    containing_block: table_init,
                    previous,
                    next: after,
                    transformed: false,
                });
            }
            previous = table_init;
            // An if before the end of the block may jump to the next initializer.
            let Some(next_head) = next_head else {
                break;
            };
            let Inst::If {
                condition,
                true_inst,
                ..
            } = *f.inst(next_head)
            else {
                unreachable!("an if");
            };
            let Some((next_field, _)) = self.match_null_checked_hashtable(f, ctx, condition) else {
                break;
            };
            if f.match_branch(true_inst) != Some(switch_head) {
                break;
            }
            table_init = after;
            dict_field = next_field;
        }
    }

    /// `comp(volatile.ldobj Hashtable(ldsflda field) != ldnull)`.
    fn match_null_checked_hashtable(
        &self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        condition: NodeId,
    ) -> Option<(ResolvedField, TypeId)> {
        let (left, right) = f.match_comp(condition, ComparisonKind::Inequality)?;
        if !is_ldnull(f, right) {
            return None;
        }
        match_dictionary_field_load(f, ctx, left, is_non_generic_hashtable)
    }

    /// `SimplifyCascadingIfStatements`: `if (op_Equality(v, "a")) br caseA; br next` chains.
    fn simplify_cascading_if_statements(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: &mut usize,
    ) -> bool {
        let list = f.list(block);
        let Some((mut condition, mut first_block_or_default_jump)) =
            f.match_if_instruction(list[*i])
        else {
            return false;
        };
        let mut next_case_jump = list[*i + 1];
        while let Some(arg) = f.match_logic_not(condition) {
            condition = arg;
            std::mem::swap(&mut first_block_or_default_jump, &mut next_case_jump);
        }
        let Some((mut switch_value_var, first_block_value, is_vb)) =
            match_string_equality_comparison(f, ctx, condition)
        else {
            return false;
        };
        if is_vb {
            std::mem::swap(&mut first_block_or_default_jump, &mut next_case_jump);
        }
        let first_block = if let Some(b) = f.match_branch(first_block_or_default_jump) {
            Some(b)
        } else if match_leave_nop(f, first_block_or_default_jump).is_some() {
            None
        } else {
            return false;
        };
        let mut values: Vec<(Str, Target)> = Vec::new();
        let mut unique: Vec<Str> = Vec::new();
        let mut matches_with_current = 0usize;
        let mut case_blocks = vec![block];
        let mut add = |value: Str,
                       target: Target,
                       values: &mut Vec<(Str, Target)>,
                       matches: &mut usize|
         -> bool {
            if unique.contains(&value) {
                return false;
            }
            unique.push(value.clone());
            *matches += 1;
            values.push((value, target));
            true
        };
        let first_target =
            first_block.map_or(Target::Inst(first_block_or_default_jump), Target::Block);
        if is_vb && is_null_or_empty(&first_block_value) {
            if !add(None, first_target, &mut values, &mut matches_with_current)
                || !add(
                    Some(Vec::new()),
                    first_target,
                    &mut values,
                    &mut matches_with_current,
                )
            {
                return false;
            }
        } else if !add(
            first_block_value,
            first_target,
            &mut values,
            &mut matches_with_current,
        ) {
            return false;
        }
        let mut remove_extra_load = false;
        let mut keep_assignment_before = false;
        let mut switch_value: Option<NodeId> = None;
        let list = f.list(block).to_vec();
        if *i >= 1
            && let Some((v, value)) = f.match_stloc(list[*i - 1])
            && v == switch_value_var
        {
            switch_value = Some(value);
            // Newer Roslyn versions use an extra variable.
            if *i >= 2
                && let Some(other) = f.match_ldloc(value)
                && f.var(other).is_single_definition()
                && f.var(other).load_count() == 1
                && let Some((v2, new_value)) = f.match_stloc(list[*i - 2])
                && v2 == other
            {
                switch_value = Some(new_value);
                remove_extra_load = true;
            }
        } else if *i >= 1
            && let Some((stloc_var, stloc_value)) = f.match_stloc(list[*i - 1])
        {
            if f.match_ldloc(stloc_value) == Some(switch_value_var) {
                // Optimized legacy code copies the value twice.
                let other = switch_value_var;
                switch_value_var = stloc_var;
                matches_with_current = 0;
                if *i >= 2
                    && let Some((v2, value)) = f.match_stloc(list[*i - 2])
                    && v2 == other
                    && f.var(other).is_single_definition()
                    && f.var(other).load_count() == 2
                {
                    switch_value = Some(value);
                    remove_extra_load = true;
                } else {
                    switch_value = Some(f.add(Inst::LdLoc { var: other }));
                }
            } else {
                // The variable before the switch is unrelated to it.
                keep_assignment_before = true;
            }
        } else {
            // The instruction before the switch is unrelated to it.
            keep_assignment_before = true;
        }
        let var_ty = f.var(switch_value_var).ty;
        if !is_known(ctx, var_ty, KnownTypeCode::String) {
            if !ctx.settings.switch_on_read_only_span_char {
                return false;
            }
            if !is_known(ctx, var_ty, KnownTypeCode::ReadOnlySpanOfT)
                && !is_known(ctx, var_ty, KnownTypeCode::SpanOfT)
            {
                return false;
            }
        }
        // The if is followed by a branch to the next case.
        let Some(mut current_case_block) = f.match_branch(next_case_jump) else {
            return false;
        };
        let mut current: Option<NodeId> = Some(current_case_block);
        let mut next_case: Option<NodeId>;
        loop {
            next_case = None;
            let Some((next, value, empty_string_equals_null, target)) =
                match_case_block(f, ctx, current_case_block, switch_value_var)
            else {
                break;
            };
            next_case = Some(next);
            if empty_string_equals_null && is_null_or_empty(&value) {
                if !add(None, target, &mut values, &mut matches_with_current)
                    || !add(
                        Some(Vec::new()),
                        target,
                        &mut values,
                        &mut matches_with_current,
                    )
                {
                    return false;
                }
            } else if !add(value, target, &mut values, &mut matches_with_current) {
                return false;
            }
            case_blocks.push(current_case_block);
            if matches!(f.inst(next), Inst::Block { .. }) {
                current_case_block = next;
                current = Some(next);
            } else {
                current = None;
                break;
            }
        }
        // Short if-chains are more plausibly hand-written, unless the chain ends in the
        // compiler-generated throw helper of a switch expression.
        if values.len() < 3
            && !current.is_some_and(|c| is_switch_expression_throw_helper_block(f, ctx, c))
        {
            return false;
        }
        // A switch value used elsewhere keeps its store.
        let loads = f.var(switch_value_var).load_count() as usize;
        if loads > matches_with_current
            || !validate_uses_of_switch_value_variable(f, switch_value_var, &case_blocks)
        {
            keep_assignment_before = true;
            remove_extra_load = false;
            switch_value = None;
        }
        // Without a moved value (the store stays): a fresh load of the variable.
        let switch_value = switch_value.unwrap_or_else(|| {
            f.add(Inst::LdLoc {
                var: switch_value_var,
            })
        });
        let offset = usize::from(first_block.is_none());
        let mut sections = Vec::new();
        for (index, &(_, target)) in values.iter().skip(offset).enumerate() {
            let body = match target {
                Target::Block(b) => branch(f, b),
                Target::Inst(inst) => f.clone_subtree(inst),
            };
            sections.push(section(f, LongSet::single(index as i64), body));
        }
        let default_body = match current {
            Some(c) => branch(f, c),
            None => leave(f, next_case.expect("the container the chain leaves")),
        };
        let count = sections.len();
        sections.push(section(f, default_labels(count), default_body));
        let map = indexed(values.iter().skip(offset).map(|(v, _)| v.clone()));
        let sti = string_to_int(f, switch_value, map, var_ty);
        let inst = switch_of(f, sti, sections);
        let list = f.list(block).to_vec();
        if remove_extra_load {
            f.add_il_range(inst, f.il_range(list[*i - 2]));
            f.replace(list[*i - 2], inst);
            remove_range(f, block, *i - 1, 3);
            *i = i.wrapping_sub(2);
        } else if keep_assignment_before {
            f.add_il_range(inst, f.il_range(list[*i]));
            f.replace(list[*i], inst);
            f.remove_at(block, *i + 1);
        } else {
            f.add_il_range(inst, f.il_range(list[*i - 1]));
            f.replace(list[*i - 1], inst);
            remove_range(f, block, *i, 2);
            *i = i.wrapping_sub(1);
        }
        true
    }

    /// `SimplifyCSharp1CascadingIfStatements`: the C# 1 chain over `string.IsInterned`.
    fn simplify_csharp1_cascading_if_statements(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: &mut usize,
    ) -> bool {
        if *i < 1 {
            return false;
        }
        let list = f.list(block).to_vec();
        // stloc switchValueVar(ldloc temp)
        // if (comp(ldloc temp == ldnull)) br defaultBlock
        // br isInternedBlock
        let Some((condition, default_block_jump)) = f.match_if_instruction(list[*i]) else {
            return false;
        };
        let Some(is_interned_block) = f.match_branch(list[*i + 1]) else {
            return false;
        };
        let Some(default_or_null_block) = f.match_branch(default_block_jump) else {
            return false;
        };
        let Some(temp) = f
            .match_comp_null(condition, ComparisonKind::Equality)
            .and_then(|l| f.match_ldloc(l))
        else {
            return false;
        };
        if !(f.var(temp).kind == VariableKind::StackSlot && f.var(temp).load_count() == 2) {
            return false;
        }
        let Some((switch_value_var, switch_value)) = f.match_stloc(list[*i - 1]) else {
            return false;
        };
        if f.match_ldloc(switch_value) != Some(temp) {
            return false;
        }
        // stloc copy(call IsInterned(ldloc switchValueVar))
        // if (comp(ldloc copy == ldstr "case1")) br caseBlock1
        // br caseHeader2
        let interned = f.list(is_interned_block);
        if f.node(is_interned_block).incoming_edge_count != 1 || interned.len() != 3 {
            return false;
        }
        let Some((copy, arg)) = f.match_stloc(interned[0]) else {
            return false;
        };
        if is_is_interned_call(f, ctx, arg).and_then(|a| f.match_ldloc(a)) != Some(switch_value_var)
        {
            return false;
        }
        let switch_value_var = copy;
        let mut condition_offset = 1;
        let mut current_case_block = is_interned_block;
        let mut values: Vec<(Str, NodeId)> = Vec::new();
        if !f.var(copy).is_single_definition() {
            return false;
        }
        // Each case: if (comp(ldloc v == ldstr "label")) br caseBlock; br next
        loop {
            let current = f.list(current_case_block);
            let Some((condition, case_block_jump)) = current
                .get(condition_offset)
                .and_then(|&c| f.match_if_instruction(c))
            else {
                break;
            };
            if current.len() != condition_offset + 2 {
                break;
            }
            let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
                break;
            };
            if f.match_ldloc(left) != Some(switch_value_var) {
                break;
            }
            let Some(value) = match_ldstr(f, right) else {
                break;
            };
            let parent = f.node(current_case_block).parent;
            if !(f.match_branch(case_block_jump).is_some()
                || (match_leave_nop(f, case_block_jump).is_some()
                    && match_leave_nop(f, case_block_jump) == parent))
            {
                break;
            }
            let Some(next) = f.match_branch(current[condition_offset + 1]) else {
                // The managed match leaves no current block; nothing can follow.
                return false;
            };
            current_case_block = next;
            condition_offset = 0;
            let clone = f.clone_subtree(case_block_jump);
            values.push((Some(value), clone));
        }
        if values.len() != f.var(copy).load_count() as usize {
            return false;
        }
        // A switch with case null.
        if current_case_block != default_or_null_block {
            let b = branch(f, default_or_null_block);
            values.push((None, b));
        }
        let mut sections = Vec::new();
        for (index, &(_, body)) in values.iter().enumerate() {
            sections.push(section(f, LongSet::single(index as i64), body));
        }
        let default_body = branch(f, current_case_block);
        let count = sections.len();
        sections.push(section(f, default_labels(count), default_body));
        let string = ctx.ts.known_type(KnownTypeCode::String);
        let map = indexed(values.into_iter().map(|(v, _)| v));
        let sti = string_to_int(f, switch_value, map, string);
        let inst = switch_of(f, sti, sections);
        f.add_il_range(inst, f.il_range(list[*i - 1]));
        f.replace(list[*i], inst);
        f.remove_at(block, *i + 1);
        f.remove_at(block, *i - 1);
        true
    }

    /// `MatchLegacySwitchOnStringWithDict`: the C# 2 `Dictionary<string, int>` pattern.
    fn match_legacy_switch_on_string_with_dict(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: &mut usize,
    ) -> bool {
        let list = f.list(block).to_vec();
        // stloc switchValueVar(switchValue) (optional)
        // if (comp(ldloc switchValueVar == ldnull)) br nullCase
        // br nextBlock
        let Some((condition, exit_block_jump)) = f.match_if_instruction(list[*i]) else {
            return false;
        };
        let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
            return false;
        };
        if !is_ldnull(f, right) {
            return false;
        }
        let Some(mut switch_value_var) = f.match_ldloc(left) else {
            return false;
        };
        if !f.var(switch_value_var).is_single_definition() {
            return false;
        }
        // A stack slot stored just before the null check: the stored variable is the one.
        let mut switch_value = None;
        if f.var(switch_value_var).kind == VariableKind::StackSlot
            && *i >= 1
            && let Some((extra_var, extra_value)) = f.match_stloc(list[*i - 1])
            && f.match_ldloc(extra_value) == Some(switch_value_var)
        {
            switch_value_var = extra_var;
            switch_value = Some(extra_value);
        }
        if !is_known(ctx, f.var(switch_value_var).ty, KnownTypeCode::String) {
            return false;
        }
        // Either br nullCase or leave container.
        let null_value_case_block = f.match_branch(exit_block_jump);
        let leave_container = if null_value_case_block.is_none() {
            let Some(c) = match_leave_nop(f, exit_block_jump) else {
                return false;
            };
            Some(c)
        } else {
            None
        };
        let Some(next_block) = list
            .get(*i + 1)
            .and_then(|&j| f.match_branch(j))
            .filter(|&b| f.node(b).incoming_edge_count == 1)
        else {
            return false;
        };
        // if (comp(volatile.ldobj Dictionary<string, int>(ldsflda field) != ldnull)) br tryGetValueBlock
        // br dictInitBlock
        let next = f.list(next_block);
        if next.len() != 2 {
            return false;
        }
        let Some((condition, try_get_value_block_jump)) = f.match_if_instruction(next[0]) else {
            return false;
        };
        let Some(try_get_value_block) = f.match_branch(try_get_value_block_jump) else {
            return false;
        };
        let Some(dict_init_block) = f
            .match_branch(next[1])
            .filter(|&b| f.node(b).incoming_edge_count == 1)
        else {
            return false;
        };
        let Some((left, right)) = f.match_comp(condition, ComparisonKind::Inequality) else {
            return false;
        };
        if !is_ldnull(f, right) {
            return false;
        }
        let Some((dict_field, dictionary_type)) =
            match_dictionary_field_load(f, ctx, left, is_string_to_int_dictionary)
        else {
            return false;
        };
        // stloc dict(newobj Dictionary..ctor(ldc.i4 valuesLength))
        // call Add(ldloc dict, ldstr value, ldc.i4 index) ...
        // volatile.stobj Dictionary<string, int>(ldsflda field, ldloc dict)
        // br switchHeadBlock
        if f.node(dict_init_block).incoming_edge_count != 1 || f.list(dict_init_block).len() < 3 {
            return false;
        }
        let Some((mut string_values, block_after_init)) = extract_string_values_from_init_block(
            f,
            ctx,
            dict_init_block,
            dictionary_type,
            &dict_field,
            false,
        ) else {
            return false;
        };
        if try_get_value_block != block_after_init {
            return false;
        }
        // if (logic.not(call TryGetValue(volatile.ldobj ...(ldsflda field), ldloc v, ldloca index))) br defaultBlock
        // br switchBlock
        let tgv = f.list(try_get_value_block);
        if f.node(try_get_value_block).incoming_edge_count != 2 || tgv.len() != 2 {
            return false;
        }
        let Some((condition, default_block_jump)) = f.match_if_instruction(tgv[0]) else {
            return false;
        };
        let default_block = f.match_branch(default_block_jump);
        if default_block.is_none() && match_leave_nop(f, default_block_jump).is_none() {
            return false;
        }
        let Some(arg) = f.match_logic_not(condition) else {
            return false;
        };
        let (Inst::Call(c) | Inst::CallVirt(c)) = f.inst(arg) else {
            return false;
        };
        if c.method.method.name != "TryGetValue" || c.args.len() < 3 {
            return false;
        }
        let args = c.args.clone();
        let Some((dict_field2, _)) =
            match_dictionary_field_load(f, ctx, args[0], is_string_to_int_dictionary)
        else {
            return false;
        };
        if !same_field(&dict_field2, &dict_field) {
            return false;
        }
        if f.match_ldloc(args[1]) != Some(switch_value_var) {
            return false;
        }
        let Some(switch_index_var) = match_ldloca(f, args[2]) else {
            return false;
        };
        let Some(switch_block) = f.match_branch(tgv[1]) else {
            return false;
        };
        // switch (ldloc switchIndexVar) { ... }, or for mcs with one case a single if.
        let sb = f.list(switch_block).to_vec();
        if f.node(switch_block).incoming_edge_count != 1 || sb.is_empty() {
            return false;
        }
        let mut sections: Vec<NodeId> = Vec::new();
        match *f.inst(sb[0]) {
            Inst::Switch {
                value,
                sections: ref existing,
                ..
            } => {
                if sb.len() != 1 || f.match_ldloc(value) != Some(switch_index_var) {
                    return false;
                }
                sections.extend(existing.iter().copied());
            }
            Inst::If {
                condition,
                true_inst,
                ..
            } => {
                if sb.len() != 2 {
                    return false;
                }
                let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
                    return false;
                };
                if f.match_ldloc(left) != Some(switch_index_var) || f.match_ldc_i4(right) != Some(0)
                {
                    return false;
                }
                let s0 = section(f, LongSet::single(0), true_inst);
                f.add_il_range(s0, f.il_range(sb[0]));
                let s1 = section(f, LongSet::single(0).invert(), sb[1]);
                f.add_il_range(s1, f.il_range(sb[1]));
                sections.push(s0);
                sections.push(s1);
            }
            _ => {}
        }
        // mcs: sections without a value map to the default section.
        fix_cases_without_value(f, &mut sections, &string_values);
        // A switch with case null.
        if let Some(null_block) = null_value_case_block
            && Some(null_block) != default_block
        {
            let body = branch(f, null_block);
            if !add_null_section(f, &mut sections, &mut string_values, body) {
                return false;
            }
        } else if let Some(lc) = leave_container
            && match_leave_nop(f, default_block_jump) != Some(lc)
            && !add_null_section(f, &mut sections, &mut string_values, exit_block_jump)
        {
            return false;
        }
        let mut keep_assignment_before = false;
        let switch_value = match switch_value {
            Some(v) if f.var(switch_value_var).load_count() <= 2 => v,
            _ => {
                keep_assignment_before = true;
                f.add(Inst::LdLoc {
                    var: switch_value_var,
                })
            }
        };
        let ty = f.var(switch_value_var).ty;
        let sti = string_to_int(f, switch_value, string_values, ty);
        let inst = switch_of(f, sti, sections);
        f.replace(list[*i + 1], inst);
        if keep_assignment_before {
            // The null check goes.
            f.add_il_range(inst, f.il_range(list[*i]));
            f.remove_at(block, *i);
            *i = i.wrapping_sub(1);
        } else {
            // The null check and the store before it go.
            f.add_il_range(inst, f.il_range(list[*i - 1]));
            remove_range(f, block, *i - 1, 2);
            *i = i.wrapping_sub(2);
        }
        true
    }

    /// `MatchLegacySwitchOnStringWithHashtable`: the C# 1 `Hashtable` lookup, with its
    /// initializer found by `scan_hashtable_initializer_blocks`.
    fn match_legacy_switch_on_string_with_hashtable(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: &mut usize,
    ) -> bool {
        let list = f.list(block).to_vec();
        // stloc tmp(ldloc switch-value)
        // stloc switchVariable(ldloc tmp)
        // if (comp(ldloc tmp == ldnull)) br nullCaseBlock
        // br getItemBlock
        if list.len() != *i + 4 {
            return false;
        }
        let Some((tmp, switch_value)) = f.match_stloc(list[*i]) else {
            return false;
        };
        let Some((switch_variable, tmp_load)) = f.match_stloc(list[*i + 1]) else {
            return false;
        };
        if f.match_ldloc(tmp_load) != Some(tmp) {
            return false;
        }
        let Some((condition, null_case_block_branch)) = f.match_if_instruction(list[*i + 2]) else {
            return false;
        };
        let Some(get_item_block) = f.match_branch(list[*i + 3]) else {
            return false;
        };
        let null_case_block = f.match_branch(null_case_block_branch);
        let null_is_leave = matches!(f.inst(null_case_block_branch), Inst::Leave { .. });
        if null_case_block.is_none() && !null_is_leave {
            return false;
        }
        let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
            return false;
        };
        if !(is_ldnull(f, right) && f.match_ldloc(left) == Some(tmp)) {
            return false;
        }
        // stloc tmp2(call get_Item(volatile.ldobj Hashtable(ldsflda field), ldloc switchVariable))
        // stloc switchVariable(ldloc tmp2)
        // if (comp(ldloc tmp2 == ldnull)) br defaultCaseBlock
        // br switchBlock
        let gi = f.list(get_item_block);
        if f.node(get_item_block).incoming_edge_count != 1 || gi.len() != 4 {
            return false;
        }
        let Some((tmp2, get_item)) = f.match_stloc(gi[0]) else {
            return false;
        };
        let Inst::Call(get_item_call) = f.inst(get_item) else {
            return false;
        };
        if get_item_call.method.method.name != "get_Item" {
            return false;
        }
        let Some((switch_variable2, tmp2_load)) = f.match_stloc(gi[1]) else {
            return false;
        };
        if f.match_ldloc(tmp2_load) != Some(tmp2) {
            return false;
        }
        if !variables_equal(f, switch_variable, switch_variable2) {
            return false;
        }
        let Some((condition, default_block_branch)) = f.match_if_instruction(gi[2]) else {
            return false;
        };
        let Some(switch_block) = f.match_branch(gi[3]) else {
            return false;
        };
        let default_block = f.match_branch(default_block_branch);
        if default_block.is_none() && !matches!(f.inst(default_block_branch), Inst::Leave { .. }) {
            return false;
        }
        let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality) else {
            return false;
        };
        if !(is_ldnull(f, right) && f.match_ldloc(left) == Some(tmp2)) {
            return false;
        }
        let gi_args = get_item_call.args.clone();
        if gi_args.len() != 2 {
            return false;
        }
        let Some((dict_field, _)) =
            match_dictionary_field_load(f, ctx, gi_args[0], is_non_generic_hashtable)
        else {
            return false;
        };
        if f.match_ldloc(gi_args[1]) != Some(switch_variable) {
            return false;
        }
        // The hashtable initializer at the start of the method.
        let Some(index) = self.hashtable_index(&dict_field) else {
            return false;
        };
        // switch (ldobj System.Int32(unbox System.Int32(ldloc switchVariable))) { ... }
        let sb = f.list(switch_block);
        if f.node(switch_block).incoming_edge_count != 1 || sb.len() != 1 {
            return false;
        }
        let Inst::Switch {
            value,
            sections: ref existing,
            ..
        } = *f.inst(sb[0])
        else {
            return false;
        };
        let Some((target, ldobj_type)) = match_ldobj(f, value) else {
            return false;
        };
        let Inst::Unbox {
            arg,
            ty: unbox_type,
        } = *f.inst(target)
        else {
            return false;
        };
        if f.match_ldloc(arg) != Some(switch_variable2)
            || !is_known(ctx, ldobj_type, KnownTypeCode::Int32)
            || unbox_type.ty != ldobj_type
        {
            return false;
        }
        let mut sections = existing.clone();
        // A switch with case null.
        if !null_is_leave && null_case_block != default_block {
            let body = branch(f, null_case_block.expect("a branch"));
            let mut labels = std::mem::take(&mut self.hashtables[index].labels);
            let added = add_null_section(f, &mut sections, &mut labels, body);
            self.hashtables[index].labels = labels;
            if !added {
                return false;
            }
        }
        let string = ctx.ts.known_type(KnownTypeCode::String);
        let labels = self.hashtables[index].labels.clone();
        let sti = string_to_int(f, switch_value, labels, string);
        let inst = switch_of(f, sti, sections);
        f.add_il_range(inst, f.il_range(list[*i]));
        f.replace(list[*i], inst);
        remove_range(f, block, *i + 1, 3);
        self.hashtables[index].transformed = true;
        true
    }

    /// `MatchRoslynSwitchOnString`: the switch over `ComputeStringHash`, each case checking
    /// the string again.
    fn match_roslyn_switch_on_string(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: &mut usize,
    ) -> bool {
        let list = f.list(block).to_vec();
        if *i >= list.len() - 1 {
            return false;
        }
        // stloc switchValueVar(switchValue)
        // if (comp(ldloc switchValueVar == ldnull)) br nullCase
        // br nextBlock
        let mut switch_block = block;
        let mut switch_block_offset = *i;
        let mut null_value_case_block = None;
        let mut inst_for_null_check = None;
        if let Some((condition, exit_block_jump)) = f.match_if_instruction(list[*i])
            && let Some(checked) = f.match_comp_null(condition, ComparisonKind::Equality)
        {
            inst_for_null_check = Some(checked);
            let Some(next_block) = f.match_branch(list[*i + 1]) else {
                return false;
            };
            if f.node(next_block).incoming_edge_count != 1 {
                return false;
            }
            let Some(null_block) = f.match_branch(exit_block_jump) else {
                return false;
            };
            null_value_case_block = Some(null_block);
            switch_block = next_block;
            switch_block_offset = 0;
        }
        // stloc switchValueVar(call ComputeStringHash(switchValueLoad))
        // switch (ldloc switchValueVar) { ... }
        let sbl = f.list(switch_block).to_vec();
        let Some(&switch_inst) = sbl.get(switch_block_offset + 1) else {
            return false;
        };
        let Inst::Switch { value, .. } = *f.inst(switch_inst) else {
            return false;
        };
        let Some(switch_value_var) = f.match_ldloc(value) else {
            return false;
        };
        let Some(switch_value_load) = match_compute_string_or_read_only_span_hash_call(
            f,
            ctx,
            sbl[switch_block_offset],
            switch_value_var,
        ) else {
            return false;
        };
        let load_var = f.match_ldloc(switch_value_load).expect("an ldloc");
        if let Some(checked) = inst_for_null_check
            && f.match_ldloc(checked) != Some(load_var)
        {
            return false;
        }
        let mut string_values: Vec<(Str, Target)> = Vec::new();
        let default = default_section(f, switch_inst);
        let default_body = section_body(f, default);
        let mut exit_or_default_block = f.match_branch(default_body);
        if exit_or_default_block.is_none() && match_leave_nop(f, default_body).is_none() {
            return false;
        }
        for s in f.list(switch_inst).to_vec() {
            if s == default {
                continue;
            }
            let Some(target) = f.match_branch(section_body(f, s)) else {
                return false;
            };
            let (string_value, empty_string_equals_null, target_or_leave, current_exit) =
                if let Some((t, exit)) =
                    match_roslyn_empty_string_case_block_head(f, ctx, target, load_var)
                {
                    (Some(Vec::new()), false, t, exit)
                } else if let Some((t, exit, value, empty)) =
                    match_roslyn_case_block_head(f, ctx, target, load_var)
                {
                    (value, empty, t, exit)
                } else {
                    return false;
                };
            if current_exit != exit_or_default_block {
                return false;
            }
            if empty_string_equals_null && is_null_or_empty(&string_value) {
                string_values.push((None, target_or_leave));
                string_values.push((Some(Vec::new()), target_or_leave));
            } else {
                string_values.push((string_value, target_or_leave));
            }
        }
        if let Some(null_block) = null_value_case_block
            && exit_or_default_block != Some(null_block)
        {
            string_values.push((None, Target::Block(null_block)));
        }
        // Newer Roslyn versions (3.7 on) check for null in the default case.
        let mut exit_or_default = exit_or_default_block.map(Target::Block);
        if !string_values.iter().any(|(v, _)| v.is_none())
            && let Some(null_block) =
                is_null_check_in_default_block(f, &mut exit_or_default, Some(load_var))
        {
            string_values.push((None, Target::Block(null_block)));
        }
        exit_or_default_block = match exit_or_default {
            Some(Target::Block(b)) => Some(b),
            _ => None,
        };
        if let Some(b) = exit_or_default_block {
            // The target may have moved past the null check in the default block.
            f.set_branch_target(default_body, b);
        }
        let generated = matches!(
            f.inst(default),
            Inst::SwitchSection {
                is_compiler_generated_default_section: true,
                ..
            }
        );
        let ty = f.var(load_var).ty;
        let replace_with_switch = |f: &mut Function, offset: usize, switch_value: NodeId| {
            let map = indexed(string_values.iter().map(|(v, _)| v.clone()));
            let mut sections = Vec::new();
            for (idx, &(_, target)) in string_values.iter().enumerate() {
                let body = body_of(f, target);
                sections.push(section(f, LongSet::single(idx as i64), body));
            }
            let d = section(f, default_labels(string_values.len()), default_body);
            if let Inst::SwitchSection {
                is_compiler_generated_default_section,
                ..
            } = f.inst_mut(d)
            {
                *is_compiler_generated_default_section = generated;
            }
            sections.push(d);
            let sti = string_to_int(f, switch_value, map, ty);
            let new_switch = switch_of(f, sti, sections);
            let old = f.list(block)[offset];
            f.replace(old, new_switch);
            new_switch
        };
        if switch_block == block {
            // stloc switchValueLoadVariable(switchValue)
            // stloc switchValueVar(call ComputeStringHash(ldloc switchValueLoadVariable))
            // switch (ldloc switchValueVar) { ... }
            // The load variable goes when only the generated equality checks use it.
            let mut switch_value_inst = switch_value_load;
            let mut keep_assignment_before = true;
            if *i >= 1
                && let Some((v, tmp)) = f.match_stloc(list[*i - 1])
                && v == load_var
                && f.var(load_var).is_single_definition()
                && f.var(load_var).load_count() as usize == f.list(switch_inst).len()
            {
                switch_value_inst = tmp;
                keep_assignment_before = false;
            }
            // The hash computation becomes the switch; the old switch goes.
            let new_switch = replace_with_switch(f, *i, switch_value_inst);
            f.add_il_range(new_switch, f.il_range(list[*i + 1]));
            f.remove_at(block, *i + 1);
            if !keep_assignment_before {
                f.add_il_range(new_switch, f.il_range(list[*i - 1]));
                f.remove_at(block, *i - 1);
                *i = i.wrapping_sub(1);
            }
        } else {
            let mut switch_value_inst = switch_value_load;
            let mut keep_assignment_before = true;
            if *i >= 2
                && let Some((temporary, temporary_value)) = f.match_stloc(list[*i - 2])
                && let Some((v, temp_load)) = f.match_stloc(list[*i - 1])
                && v == load_var
                && f.match_ldloc(temp_load) == Some(temporary)
            {
                switch_value_inst = temporary_value;
                keep_assignment_before = false;
            }
            // The null check becomes the switch; the jump to the switch block goes.
            let new_switch = replace_with_switch(f, *i, switch_value_inst);
            f.add_il_range(new_switch, f.il_range(switch_inst));
            f.add_il_range(new_switch, f.il_range(list[*i + 1]));
            f.remove_at(block, *i + 1);
            if !keep_assignment_before {
                f.add_il_range(new_switch, f.il_range(list[*i - 2]));
                remove_range(f, block, *i - 2, 2);
                *i = i.wrapping_sub(2);
            }
        }
        true
    }

    /// `MatchRoslynSwitchOnStringUsingLengthAndChar`: Roslyn's switch on the string length,
    /// then on a distinguishing char (dotnet/roslyn#66081).
    fn match_roslyn_switch_on_string_using_length_and_char(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        i: usize,
    ) -> bool {
        let list = f.list(block).to_vec();
        // if (comp(ldloc switchValueVar == ldnull)) br nullCase
        // br nextBlock
        let mut st = LengthAndChar {
            default_case: None,
            default_is_compiler_generated: false,
            null_case: None,
        };
        let switch_on_length_block;
        let start;
        let mut switch_value_var;
        if let Some((condition, exit_block_jump)) = f.match_if_instruction(list[i])
            && let Some(checked) = f.match_comp_null(condition, ComparisonKind::Equality)
            && let Some(v) = f.match_ldloc(checked)
        {
            switch_value_var = Some(v);
            let Some(next_block) = list.get(i + 1).and_then(|&j| f.match_branch(j)) else {
                return false;
            };
            st.null_case = f.match_branch(exit_block_jump);
            if st.null_case.is_none() && match_leave_nop(f, exit_block_jump).is_none() {
                return false;
            }
            // if (comp(ldloc switchValueVar == ldnull)) br ...
            // br switchOnLengthBlock
            let next = f.list(next_block);
            if f.node(next_block).incoming_edge_count == 1
                && let Some((c, _)) = next.first().and_then(|&n| f.match_if_instruction(n))
                && f.match_comp_null(c, ComparisonKind::Equality)
                    .and_then(|l| f.match_ldloc(l))
                    == Some(v)
            {
                let Some(b) = next.get(1).and_then(|&n| f.match_branch(n)) else {
                    return false;
                };
                switch_on_length_block = b;
            } else {
                switch_on_length_block = next_block;
            }
            if f.node(switch_on_length_block).incoming_edge_count != 1 {
                return false;
            }
            start = 0;
        } else {
            switch_on_length_block = block;
            // Found by `match_switch_on_length_block`.
            switch_value_var = None;
            start = i;
        }
        let Some(blocks_by_length) = st.match_switch_on_length_block(
            f,
            ctx,
            &mut switch_value_var,
            switch_on_length_block,
            start,
        ) else {
            return false;
        };
        let Some(switch_value_var) = switch_value_var else {
            return false;
        };
        let mut string_values: Vec<(Str, Target)> = Vec::new();
        for (length_set, target) in blocks_by_length {
            if length_set.count() != 1 {
                if Some(target) != st.null_case.map(Target::Block) {
                    return false;
                }
                continue;
            }
            let length = length_set.intervals()[0].start as i32;
            match target {
                Target::Inst(_) => {}
                Target::Block(target_block) => {
                    if let Some(mapping) = st.match_switch_on_char_block(
                        f,
                        ctx,
                        &mut self.analysis,
                        target_block,
                        length,
                        switch_value_var,
                    ) {
                        for item in mapping {
                            if string_values.iter().any(|x| x.0 == item.0) {
                                return false;
                            }
                            string_values.push(item);
                        }
                    } else if let Some((body_or_leave, exit, string_value, _)) =
                        match_roslyn_case_block_head(f, ctx, target_block, switch_value_var)
                    {
                        if exit.map(Target::Block) != st.default_case {
                            return false;
                        }
                        if string_values.iter().any(|x| x.0 == string_value) {
                            return false;
                        }
                        string_values.push((string_value, body_or_leave));
                    } else if length == 0 {
                        string_values.push((Some(Vec::new()), target));
                    } else {
                        return false;
                    }
                }
            }
        }
        if !string_values.iter().any(|(v, _)| v.is_none()) {
            if let Some(null_block) =
                is_null_check_in_default_block(f, &mut st.default_case, Some(switch_value_var))
            {
                string_values.push((None, Target::Block(null_block)));
            } else if let Some(null_case) = st.null_case
                && Some(Target::Block(null_case)) != st.default_case
            {
                string_values.push((None, Target::Block(null_case)));
            }
        }
        let Some(default_case) = st.default_case else {
            return false;
        };
        let map = indexed(string_values.iter().map(|(v, _)| v.clone()));
        let mut sections = Vec::new();
        for (idx, &(_, target)) in string_values.iter().enumerate() {
            let body = body_of(f, target);
            sections.push(section(f, LongSet::single(idx as i64), body));
        }
        let ty = f.var(switch_value_var).ty;
        let ld = f.add(Inst::LdLoc {
            var: switch_value_var,
        });
        let sti = string_to_int(f, ld, map, ty);
        let default_body = body_of(f, default_case);
        let d = section(f, default_labels(string_values.len()), default_body);
        if let Inst::SwitchSection {
            is_compiler_generated_default_section,
            ..
        } = f.inst_mut(d)
        {
            *is_compiler_generated_default_section = st.default_is_compiler_generated;
        }
        sections.push(d);
        let new_switch = switch_of(f, sti, sections);
        f.add_il_range(new_switch, f.il_range(list[i]));
        if st.null_case.is_some() {
            f.add_il_range(new_switch, f.il_range(list[i + 1]));
        }
        f.replace(list[i], new_switch);
        f.truncate(block, i + 1);
        true
    }
}

/// The state `MatchRoslynSwitchOnStringUsingLengthAndChar` shares with its local functions.
struct LengthAndChar {
    default_case: Option<Target>,
    default_is_compiler_generated: bool,
    null_case: Option<NodeId>,
}

impl LengthAndChar {
    /// `InheritCompilerGeneratedDefaultMarker`: the char switches and the switch on the length
    /// share the throw-helper block of the implicit default case; whichever SwitchDetection
    /// processed first carries the marker.
    fn inherit_compiler_generated_default_marker(&mut self, f: &Function, inner: NodeId) {
        self.default_is_compiler_generated |= f.list(inner).iter().any(|&s| {
            let Inst::SwitchSection {
                body,
                is_compiler_generated_default_section: true,
                ..
            } = *f.inst(s)
            else {
                return false;
            };
            f.match_branch(body)
                .is_some_and(|t| Some(Target::Block(t)) == self.default_case)
        });
    }

    /// `MatchGetChars`: `s[index]` of the switch value, as `get_Chars` or a span indexer.
    fn match_get_chars(
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        inst: NodeId,
        var: VarId,
    ) -> Option<i32> {
        let index_of = |f: &Function, call: NodeId, ldloca: bool| -> Option<i32> {
            let Inst::Call(c) = f.inst(call) else {
                return None;
            };
            let [arg0, arg1] = c.args[..] else {
                return None;
            };
            let ok = if ldloca {
                match_ldloca(f, arg0) == Some(var)
            } else {
                f.match_ldloc(arg0) == Some(var)
            };
            f.match_ldc_i4(arg1).filter(|&i| ok && i >= 0)
        };
        if ctx.settings.switch_on_read_only_span_char
            && let Some((target, ty)) = match_ldobj(f, inst)
            && is_known(ctx, ty, KnownTypeCode::UInt16)
        {
            let Inst::Call(c) = f.inst(target) else {
                return None;
            };
            let name = declaring_full_name(ctx, c.method.method.declaring);
            if c.method.method.name != "get_Item"
                || !matches!(name.as_deref(), Some("System.ReadOnlySpan" | "System.Span"))
            {
                return None;
            }
            index_of(f, target, true)
        } else {
            let Inst::Call(c) = f.inst(inst) else {
                return None;
            };
            if c.method.method.name != "get_Chars"
                || declaring_full_name(ctx, c.method.method.declaring).as_deref()
                    != Some("System.String")
            {
                return None;
            }
            index_of(f, inst, false)
        }
    }

    /// `MatchSwitchOnCharBlock`: a switch on one char of the string; the strings of its cases.
    fn match_switch_on_char_block(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        analysis: &mut SwitchAnalysis,
        block: NodeId,
        length: i32,
        var: VarId,
    ) -> Option<Vec<(Str, Target)>> {
        if f.node(block).incoming_edge_count != 1 {
            return None;
        }
        let list = f.list(block);
        let (index, sections): (i32, Vec<(LongSet, NodeId)>) = match *list {
            [sw] => {
                let Inst::Switch { value, .. } = *f.inst(sw) else {
                    return None;
                };
                let index = Self::match_get_chars(f, ctx, value, var)?;
                self.inherit_compiler_generated_default_marker(f, sw);
                (index, switch_sections(f, sw))
            }
            [store, sw] => {
                let (char_temp, get_chars) = f.match_stloc(store)?;
                let index = Self::match_get_chars(f, ctx, get_chars, var)?;
                let Inst::Switch { value, .. } = *f.inst(sw) else {
                    return None;
                };
                if f.match_ldloc(value) != Some(char_temp) {
                    return None;
                }
                self.inherit_compiler_generated_default_marker(f, sw);
                (index, switch_sections(f, sw))
            }
            _ => {
                if !analysis.analyze_block(f, block) {
                    return None;
                }
                let (char_temp, get_chars) = f.match_stloc(list[0])?;
                let index = Self::match_get_chars(f, ctx, get_chars, var)?;
                if analysis.switch_variable() != char_temp {
                    return None;
                }
                (index, analysis.sections.clone())
            }
        };
        if index >= length {
            return None;
        }
        let mut results = Vec::new();
        let mut has_default_section = false;
        for (labels, body) in sections {
            if labels.count() == 1 {
                let ch = labels.values().next().expect("one value") as u16;
                let mut target_block = f.match_branch(body)?;
                if length == 1 {
                    results.push((Some(vec![ch]), Target::Block(target_block)));
                } else {
                    while let Some((body_or_leave, exit, string_value, _)) =
                        match_roslyn_case_block_head(f, ctx, target_block, var)
                    {
                        let s = string_value.as_ref()?;
                        if s.len() != length as usize || s[index as usize] != ch {
                            return None;
                        }
                        results.push((string_value, body_or_leave));
                        if exit == self.null_case {
                            break;
                        }
                        target_block = exit?;
                    }
                }
            } else if !has_default_section {
                has_default_section = true;
            } else {
                return None;
            }
        }
        (!results.is_empty()).then_some(results)
    }

    /// `MatchSwitchOnLengthBlock`: the switch (or the if) on the string length; the targets by
    /// length, finding the switch variable when not yet known.
    fn match_switch_on_length_block(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        switch_value_var: &mut Option<VarId>,
        block: NodeId,
        start: usize,
    ) -> Option<Vec<(LongSet, Target)>> {
        let list = f.list(block);
        let mut blocks = Vec::new();
        let (sw, get_length_call) = match list.len() - start {
            1 => {
                let sw = list[start];
                let Inst::Switch { value, .. } = *f.inst(sw) else {
                    return None;
                };
                (Some(sw), value)
            }
            2 => {
                let (length_var, get_length) = f.match_stloc(list[start])?;
                let sw = list[start + 1];
                let Inst::Switch { value, .. } = *f.inst(sw) else {
                    return None;
                };
                if f.match_ldloc(value) != Some(length_var) {
                    return None;
                }
                (Some(sw), get_length)
            }
            3 => {
                let (length_var, get_length) = f.match_stloc(list[start])?;
                let (condition, goto_length) = f.match_if_instruction(list[start + 1])?;
                let mut target = f.match_branch(goto_length)?;
                let mut goto_else = f.match_branch(list[start + 2])?;
                let (lhs, rhs) = match f.match_comp(condition, ComparisonKind::Equality) {
                    Some(p) => p,
                    None => {
                        let p = f.match_comp(condition, ComparisonKind::Inequality)?;
                        std::mem::swap(&mut target, &mut goto_else);
                        p
                    }
                };
                self.default_case = Some(Target::Block(goto_else));
                if f.match_ldloc(lhs) != Some(length_var) {
                    return None;
                }
                let length = i64::from(f.match_ldc_i4(rhs)?);
                blocks.push((LongSet::single(length), Target::Block(target)));
                blocks.push((LongSet::single(length).invert(), Target::Block(goto_else)));
                (None, get_length)
            }
            _ => return None,
        };
        let Inst::Call(call) = f.inst(get_length_call) else {
            return None;
        };
        let [arg] = call.args[..] else {
            return None;
        };
        if call.method.method.name != "get_Length" {
            return None;
        }
        match call.method.method.declaring.and_then(|d| known(ctx, d)) {
            Some(KnownTypeCode::String) => {
                if switch_value_var.is_none() || f.match_ldloc(arg) != *switch_value_var {
                    return None;
                }
            }
            Some(KnownTypeCode::ReadOnlySpanOfT | KnownTypeCode::SpanOfT) => {
                if !ctx.settings.switch_on_read_only_span_char {
                    return None;
                }
                *switch_value_var = Some(match_ldloca(f, arg)?);
            }
            _ => return None,
        }
        let Some(sw) = sw else {
            return Some(blocks);
        };
        let mut blocks = Vec::new();
        for s in f.list(sw).to_vec() {
            let Inst::SwitchSection {
                ref labels,
                body,
                has_null_label,
                is_compiler_generated_default_section,
            } = *f.inst(s)
            else {
                unreachable!("switch sections");
            };
            if has_null_label {
                return None;
            }
            let target_inst = match f.match_branch(body) {
                Some(b) => Target::Block(b),
                None if match_leave_nop(f, body).is_some() => Target::Inst(body),
                None => return None,
            };
            if labels.count() != 1 {
                if self.default_case.is_none() {
                    self.default_case = Some(target_inst);
                }
                if self.default_case != Some(target_inst) {
                    return None;
                }
                // An implicit-default throw helper already inlined here keeps its marker.
                self.default_is_compiler_generated |= is_compiler_generated_default_section;
            } else {
                blocks.push((labels.clone(), target_inst));
            }
        }
        Some(blocks)
    }
}

fn switch_sections(f: &Function, sw: NodeId) -> Vec<(LongSet, NodeId)> {
    f.list(sw)
        .iter()
        .map(|&s| match f.inst(s) {
            Inst::SwitchSection { labels, body, .. } => (labels.clone(), *body),
            other => panic!("{other:?} is not a switch section"),
        })
        .collect()
}

fn section_body(f: &Function, s: NodeId) -> NodeId {
    match f.inst(s) {
        Inst::SwitchSection { body, .. } => *body,
        other => panic!("{other:?} is not a switch section"),
    }
}

/// `ValidateUsesOfSwitchValueVariable`: every load is in one of the case blocks.
fn validate_uses_of_switch_value_variable(
    f: &Function,
    var: VarId,
    case_blocks: &[NodeId],
) -> bool {
    f.var(var).load_instructions.iter().all(|&load| {
        f.ancestors(load).any(|a| case_blocks.contains(&a)) || case_blocks.contains(&load)
    })
}

/// `IsIsInternedCall`: the argument of `string.IsInterned`.
fn is_is_interned_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let [arg] = c.args[..] else {
        return None;
    };
    (declaring_is(ctx, c.method.method.declaring, KnownTypeCode::String)
        && c.method.method.is_static
        && c.method.method.name == "IsInterned")
        .then_some(arg)
}

/// `MatchCaseBlock`: `if (op_Equality(v, "a")) br caseBlock; br nextBlock` (or its inverted
/// form); the next block (or the container a leave exits), the value, whether the empty
/// string equals null (VB) and the case target.
fn match_case_block(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    var: VarId,
) -> Option<(NodeId, Str, bool, Target)> {
    if f.node(block).incoming_edge_count != 1 || f.list(block).len() != 2 {
        return None;
    }
    let (condition, mut case_branch, mut next_branch) = f.match_if_at_end_of_block(block)?;
    let (v, value, is_vb) = match_string_equality_comparison(f, ctx, condition)?;
    if v != var {
        return None;
    }
    if is_vb {
        std::mem::swap(&mut case_branch, &mut next_branch);
    }
    let target = if let Some(b) = f.match_branch(case_branch) {
        Target::Block(b)
    } else if match_leave_nop(f, case_branch).is_some() {
        Target::Inst(case_branch)
    } else {
        return None;
    };
    let next = f
        .match_branch(next_branch)
        .or_else(|| match_leave_nop(f, next_branch))?;
    Some((next, value, is_vb, target))
}

/// `FixCasesWithoutValue`: mcs sections whose labels map to no string join the default
/// section (the one with the most labels).
fn fix_cases_without_value(f: &mut Function, sections: &mut Vec<NodeId>, values: &[(Str, i32)]) {
    let Some(&first) = sections.first() else {
        return;
    };
    let labels = |f: &Function, s: NodeId| match f.inst(s) {
        Inst::SwitchSection { labels, .. } => labels.clone(),
        other => panic!("{other:?} is not a switch section"),
    };
    let has_label = |f: &Function, s: NodeId| {
        values
            .iter()
            .any(|&(_, i)| labels(f, s).contains(i64::from(i)))
    };
    let mut default = first;
    let mut without_labels = Vec::new();
    for &s in sections.iter() {
        if s == default {
            continue;
        }
        if labels(f, s).count() > labels(f, default).count() {
            if !has_label(f, default) {
                without_labels.push(default);
            }
            default = s;
            continue;
        }
        if !has_label(f, s) {
            without_labels.push(s);
        }
    }
    for s in without_labels {
        let (l, null) = match f.inst(s) {
            Inst::SwitchSection {
                labels,
                has_null_label,
                ..
            } => (labels.clone(), *has_null_label),
            _ => unreachable!("a switch section"),
        };
        if let Inst::SwitchSection {
            labels,
            has_null_label,
            ..
        } = f.inst_mut(default)
        {
            *labels = labels.union_with(&l);
            *has_null_label |= null;
        }
        if let Some(p) = sections.iter().position(|&x| x == s) {
            sections.remove(p);
        }
    }
}

/// `AddNullSection`: a section for null with the label after the last string's.
fn add_null_section(
    f: &mut Function,
    sections: &mut Vec<NodeId>,
    values: &mut Vec<(Str, i32)>,
    body: NodeId,
) -> bool {
    let Some(max) = values.iter().map(|&(_, i)| i).max() else {
        return false;
    };
    let label = LongSet::single(i64::from(max) + 1);
    let conflicts: Vec<NodeId> = sections
        .iter()
        .copied()
        .filter(
            |&s| matches!(f.inst(s), Inst::SwitchSection { labels, .. } if labels.overlaps(&label)),
        )
        .collect();
    match conflicts[..] {
        [] => {}
        [c] => {
            let Inst::SwitchSection { labels, .. } = f.inst_mut(c) else {
                unreachable!("a switch section");
            };
            if labels.count() == 1 {
                // The only label cannot go.
                return false;
            }
            *labels = labels.except_with(&label);
        }
        _ => return false,
    }
    values.push((None, max + 1));
    let s = section(f, label, body);
    sections.push(s);
    true
}

/// `MatchDictionaryFieldLoad`: `volatile.ldobj type(ldsflda field)` of a compiler-generated
/// field.
fn match_dictionary_field_load(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    type_matcher: fn(&mut Context<'_, '_, '_>, TypeId) -> bool,
) -> Option<(ResolvedField, TypeId)> {
    let (target, ty) = match_ldobj(f, inst)?;
    if !type_matcher(ctx, ty) {
        return None;
    }
    let field = &match_ldsflda(f, target)?.field;
    (ctx.ts
        .field_is_compiler_generated_or_in_compiler_generated_class(field)
        || field.name.starts_with("$$method"))
    .then(|| (field.clone(), ty))
}

/// `ExtractStringValuesFromInitBlock`: the values the `Add` calls of a dictionary or
/// hashtable initializer put in, and the block after it.
fn extract_string_values_from_init_block(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    dictionary_type: TypeId,
    dictionary_field: &ResolvedField,
    is_hashtable_pattern: bool,
) -> Option<(Vec<(Str, i32)>, NodeId)> {
    let list = f.list(block);
    // stloc dictVar(newobj Dictionary..ctor(ldc.i4 valuesLength))
    // -or-
    // stloc dictVar(newobj Hashtable..ctor(ldc.i4 capacity, ldc.f4 loadFactor))
    let (dict_var, new_obj) = f.match_stloc(*list.first()?)?;
    let Inst::NewObj(c) = f.inst(new_obj) else {
        return None;
    };
    if c.method.method.declaring != Some(dictionary_type) {
        return None;
    }
    let values_length = match c.args[..] {
        [capacity, load_factor] => {
            let n = f.match_ldc_i4(capacity)?;
            if !matches!(*f.inst(load_factor), Inst::LdcF4 { bits } if bits == 0.5f32.to_bits()) {
                return None;
            }
            n
        }
        [capacity] => f.match_ldc_i4(capacity)?,
        _ => 0,
    };
    if values_length < 0 {
        return None;
    }
    let mut values = Vec::new();
    let mut i = 0;
    while let Some(&inst) = list.get(i + 1)
        && let Some((index, value)) = match_add_call(f, ctx, dictionary_type, inst, dict_var)
    {
        values.push((Some(value), index));
        i += 1;
    }
    // volatile.stobj dictionaryType(ldsflda dictionaryField, ldloc dictVar)
    let Inst::StObj {
        target, value, ty, ..
    } = *f.inst(*list.get(i + 1)?)
    else {
        return None;
    };
    if ty.ty != dictionary_type
        || !match_ldsflda(f, target).is_some_and(|fo| same_field(&fo.field, dictionary_field))
        || f.match_ldloc(value) != Some(dict_var)
    {
        return None;
    }
    if is_hashtable_pattern && matches!(f.inst(*list.get(i + 2)?), Inst::If { .. }) {
        return Some((values, f.match_branch(*list.get(i + 3)?)?));
    }
    Some((values, f.match_branch(*list.get(i + 2)?)?))
}

/// `MatchAddCall`: `call Add(ldloc dictVar, ldstr value, ldc.i4 index)`, the index possibly
/// boxed.
fn match_add_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    dictionary_type: TypeId,
    inst: NodeId,
    dict_var: VarId,
) -> Option<(i32, Vec<u16>)> {
    let (Inst::Call(c) | Inst::CallVirt(c)) = f.inst(inst) else {
        return None;
    };
    let [dict, key, index] = c.args[..] else {
        return None;
    };
    if c.method.method.name != "Add" || f.match_ldloc(dict) != Some(dict_var) {
        return None;
    }
    let value = match match_ldstr(f, key) {
        Some(v) => v,
        None => {
            let field = f.match_ldsfld(key)?;
            if !(declaring_is(ctx, field.field.declaring, KnownTypeCode::String)
                && field.field.name == "Empty")
            {
                return None;
            }
            Vec::new()
        }
    };
    if c.method.method.declaring != Some(dictionary_type) || c.method.method.is_static {
        return None;
    }
    let index = match *f.inst(index) {
        Inst::Box { arg, .. } => f.match_ldc_i4(arg),
        _ => f.match_ldc_i4(index),
    }?;
    Some((index, value))
}

fn is_string_to_int_dictionary(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> bool {
    if ctx.ts.full_name(ty) != "System.Collections.Generic.Dictionary" {
        return false;
    }
    let args = ctx.ts.type_arguments(ty);
    let [key, value] = args[..] else {
        return false;
    };
    is_known(ctx, key, KnownTypeCode::String) && is_known(ctx, value, KnownTypeCode::Int32)
}

fn is_non_generic_hashtable(ctx: &mut Context<'_, '_, '_>, ty: TypeId) -> bool {
    ctx.ts.full_name(ty) == "System.Collections.Hashtable" && ctx.ts.type_arguments(ty).is_empty()
}

/// `IsNullCheckInDefaultBlock`: `if (comp(ldloc v == ldnull)) br nullCase; br newDefault` as
/// the old default block; moves `exit_or_default` to the new default and returns the null
/// case.
fn is_null_check_in_default_block(
    f: &Function,
    exit_or_default: &mut Option<Target>,
    var: Option<VarId>,
) -> Option<NodeId> {
    let Some(Target::Block(block)) = *exit_or_default else {
        return None;
    };
    let list = f.list(block);
    let (condition, then_branch) = f.match_if_instruction(*list.first()?)?;
    let arg = f.match_comp_null(condition, ComparisonKind::Equality)?;
    if var.is_none() || f.match_ldloc(arg) != var {
        return None;
    }
    let null_case = f.match_branch(then_branch)?;
    if f.node(null_case).parent != f.node(block).parent {
        return None;
    }
    let else_block = f.match_branch(*list.get(1)?)?;
    if f.node(else_block).parent != f.node(block).parent {
        return None;
    }
    *exit_or_default = Some(Target::Block(else_block));
    Some(null_case)
}

/// `MatchRoslynCaseBlockHead`: `if (op_Equality(v, "a")) br body; br exit` (or negated);
/// the body (or leave), the exit block (`None` for a leave), the value and whether the empty
/// string equals null.
fn match_roslyn_case_block_head(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    target: NodeId,
    var: VarId,
) -> Option<(Target, Option<NodeId>, Str, bool)> {
    let list = f.list(target);
    let [first, second] = *list else {
        return None;
    };
    let (mut condition, mut body_branch) = match f.match_if_instruction(first) {
        Some(p) => p,
        // Without an if when the body is the exit: `call op_Equality(...); leave`.
        None => (first, second),
    };
    let mut exit_branch = second;
    while let Some(expr) = f.match_logic_not(condition) {
        std::mem::swap(&mut exit_branch, &mut body_branch);
        condition = expr;
    }
    let (v, string_value, is_vb) = match_string_equality_comparison(f, ctx, condition)?;
    if v != var {
        return None;
    }
    if is_vb {
        std::mem::swap(&mut exit_branch, &mut body_branch);
    }
    let exit = f.match_branch(exit_branch);
    if exit.is_none() && match_leave_nop(f, exit_branch).is_none() {
        return None;
    }
    if match_leave_nop(f, body_branch).is_some() {
        return Some((Target::Inst(body_branch), exit, string_value, is_vb));
    }
    let body = f.match_branch(body_branch)?;
    Some((Target::Block(body), exit, string_value, is_vb))
}

/// `MatchRoslynEmptyStringCaseBlockHead`: the null check and `get_Length` check of the
/// empty-string case.
fn match_roslyn_empty_string_case_block_head(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    target: NodeId,
    var: VarId,
) -> Option<(Target, Option<NodeId>)> {
    let list = f.list(target);
    if list.len() != 2 || f.node(target).incoming_edge_count != 1 {
        return None;
    }
    let (null_comparison, exit_branch) = f.match_if_instruction(list[0])?;
    let arg = f.match_comp_null(null_comparison, ComparisonKind::Equality)?;
    if f.match_ldloc(arg) != Some(var) {
        return None;
    }
    let length_check_block = f.match_branch(list[1])?;
    let lc = f.list(length_check_block);
    if lc.len() != 2 || f.node(length_check_block).incoming_edge_count != 1 {
        return None;
    }
    let (mut length_check_condition, mut exit_branch2) = f.match_if_instruction(lc[0])?;
    let body_branch;
    if let Some(a) = f.match_logic_not(length_check_condition) {
        body_branch = exit_branch2;
        exit_branch2 = lc[1];
        length_check_condition = a;
    } else {
        body_branch = lc[1];
    }
    let exit = f.match_branch(exit_branch2);
    if exit.is_none() && match_leave_nop(f, exit_branch2).is_none() {
        return None;
    }
    if !match_string_length_call(f, ctx, length_check_condition, var) {
        return None;
    }
    if !f.structurally_equal(exit_branch, exit_branch2) {
        return None;
    }
    if match_leave_nop(f, body_branch).is_some() {
        return Some((Target::Inst(body_branch), exit));
    }
    let body = f.match_branch(body_branch)?;
    Some((Target::Block(body), exit))
}

/// `MatchStringLengthCall`: `call get_Length(ldloc v)` of `string`.
fn match_string_length_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    var: VarId,
) -> bool {
    let Inst::Call(c) = f.inst(inst) else {
        return false;
    };
    let [arg] = c.args[..] else {
        return false;
    };
    declaring_is(ctx, c.method.method.declaring, KnownTypeCode::String)
        && ctx.ts.getter_property_name(&c.method.method).as_deref() == Some("Length")
        && f.match_ldloc(arg) == Some(var)
}

/// `MatchStringEqualityComparison`: `call string.op_Equality(ldloc v, ldstr s)`, VB's
/// `CompareString(ldloc v, ldstr s, false)`, `SequenceEqual(ldloc v, AsSpan(ldstr s))` or
/// `comp(ldloc v == ldnull)`; the variable, the value and whether it is VB's comparison
/// (which returns 0 on equality and treats the empty string as null).
fn match_string_equality_comparison(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
) -> Option<(VarId, Str, bool)> {
    let mut condition = condition;
    // if (x != 0) is if (x).
    while let Inst::Comp {
        kind: ComparisonKind::Inequality,
        left,
        right,
        ..
    } = *f.inst(condition)
        && f.match_ldc_i4(right) == Some(0)
    {
        condition = left;
    }
    if let Inst::Call(c) = f.inst(condition) {
        let m = &c.method.method;
        let declaring = declaring_full_name(ctx, m.declaring);
        let mut is_vb = false;
        let (left, right) = if m.name == "op_Equality"
            && c.args.len() == 2
            && ctx.ts.method_is_operator(m)
            && declaring_is(ctx, m.declaring, KnownTypeCode::String)
        {
            (c.args[0], c.args[1])
        } else if m.is_static
            && m.name == "CompareString"
            && declaring.as_deref() == Some("Microsoft.VisualBasic.CompilerServices.Operators")
            && c.args.len() == 3
        {
            is_vb = true;
            // Option Compare Text (case insensitive) has no C# equivalent.
            if f.match_ldc_i4(c.args[2]) != Some(0) {
                return None;
            }
            (c.args[0], c.args[1])
        } else if m.is_static
            && m.name == "SequenceEqual"
            && declaring.as_deref() == Some("System.MemoryExtensions")
            && c.args.len() == 2
        {
            let Inst::Call(as_span) = f.inst(c.args[1]) else {
                return None;
            };
            let a = &as_span.method.method;
            let [ld_str] = as_span.args[..] else {
                return None;
            };
            if !(a.is_static
                && a.name == "AsSpan"
                && declaring_full_name(ctx, a.declaring).as_deref()
                    == Some("System.MemoryExtensions"))
            {
                return None;
            }
            (c.args[0], ld_str)
        } else {
            return None;
        };
        let var = f.match_ldloc(left)?;
        let value = match_ldstr(f, right)?;
        Some((var, Some(value), is_vb))
    } else {
        let arg = f.match_comp_null(condition, ComparisonKind::Equality)?;
        Some((f.match_ldloc(arg)?, None, false))
    }
}
