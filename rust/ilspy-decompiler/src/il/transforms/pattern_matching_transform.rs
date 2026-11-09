//! `IL/Transforms/PatternMatchingTransform.cs`: an `isinst` test stored in a variable used
//! only where the test succeeded becomes a type pattern (`x is T v`), with property
//! sub-patterns and null checks folded in.

use crate::il::ast::{
    ComparisonKind, ComparisonLiftingKind, Function, Inst, NodeId, TypeOperand, VarId, VariableKind,
};
use crate::il::control_flow::control_flow_graph::ControlFlowGraph;
use crate::il::control_flow::exit_points::{Exit, compatible_exit_instruction};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::il_inlining::known;
use crate::il::types::{Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.pattern_matching {
        return;
    }
    let f = &mut r.function;
    let body = f.body.expect("body");
    let containers: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for container in containers {
        let mut cfg: Option<ControlFlowGraph> = None;
        for block in f.list(container).to_vec().into_iter().rev() {
            if pattern_match_value_types(f, ctx, block, container, &mut cfg) {
                continue;
            }
            pattern_match_ref_types(f, ctx, block, container, &mut cfg);
        }
        f.retain(container, |f, b| !f.list(b).is_empty());
    }
}

fn compatible(f: &Function, a: NodeId, b: NodeId) -> bool {
    compatible_exit_instruction(f, a, Exit::Inst(b))
}

fn if_parts(f: &Function, if_inst: NodeId) -> (NodeId, NodeId) {
    match *f.inst(if_inst) {
        Inst::If {
            condition,
            true_inst,
            ..
        } => (condition, true_inst),
        ref other => panic!("{other:?} is not an if"),
    }
}

/// `block.Instructions[^1] = inst`.
fn set_last(f: &mut Function, block: NodeId, inst: NodeId) {
    let mut list = f.list(block).to_vec();
    let last = list.last_mut().expect("instructions");
    if *last != inst {
        *last = inst;
        f.set_list(block, list);
    }
}

/// `ifInst.TrueInst = inst`.
fn set_true_inst(f: &mut Function, if_inst: NodeId, inst: NodeId) {
    let (_, current) = if_parts(f, if_inst);
    if current != inst {
        f.replace(current, inst);
    }
}

/// `block.Instructions.Clear(); block.Instructions.Add(inst)`.
fn reduce_to(f: &mut Function, block: NodeId, inst: NodeId) {
    f.truncate(block, 0);
    f.push(block, inst);
}

fn new_match(f: &mut Function, variable: VarId, tested_operand: NodeId) -> NodeId {
    f.add(Inst::Match {
        variable,
        method: None,
        tested_operand,
        sub_patterns: Vec::new(),
        is_deconstruct_call: false,
        is_deconstruct_tuple: false,
        check_type: false,
        check_not_null: false,
    })
}

fn set_match_flags(f: &mut Function, m: NodeId, check_type: bool, check_not_null: bool) {
    if let Inst::Match {
        check_type: t,
        check_not_null: n,
        ..
    } = f.inst_mut(m)
    {
        *t |= check_type;
        *n |= check_not_null;
    }
}

fn match_variable(f: &Function, m: NodeId) -> VarId {
    match *f.inst(m) {
        Inst::Match { variable, .. } => variable,
        ref other => panic!("{other:?} is not a match"),
    }
}

fn tested_operand(f: &Function, m: NodeId) -> NodeId {
    match *f.inst(m) {
        Inst::Match { tested_operand, .. } => tested_operand,
        ref other => panic!("{other:?} is not a match"),
    }
}

/// `new Comp(kind, sign, left, right)`: the input type is the left operand's.
fn comp(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    kind: ComparisonKind,
    left: NodeId,
    right: NodeId,
) -> NodeId {
    let input = f.result_type(ctx.ts, left);
    f.add(Inst::Comp {
        kind,
        lifting: ComparisonLiftingKind::None,
        sign: Sign::None,
        input,
        left,
        right,
    })
}

/// `Comp.LogicNot`.
fn logic_not(f: &mut Function, ctx: &mut Context<'_, '_, '_>, arg: NodeId) -> NodeId {
    let zero = f.add(Inst::LdcI4 { value: 0 });
    comp(f, ctx, ComparisonKind::Equality, arg, zero)
}

fn is_type_parameter(ctx: &Context<'_, '_, '_>, mut ty: TypeId) -> bool {
    loop {
        match ctx.ts.data(ty) {
            TypeData::TypeParam { .. } => return true,
            TypeData::Annotated { inner, .. } | TypeData::Modified { inner, .. } => ty = *inner,
            _ => return false,
        }
    }
}

/// `PatternMatchRefTypes`.
///
/// ```text
/// stloc V(isinst T(testedOperand))
/// if (comp.o(ldloc V == ldnull)) br falseBlock
/// br trueBlock
/// =>
/// if (match.type[T].notnull(V = testedOperand)) br trueBlock
/// br falseBlock
/// ```
///
/// (or with `stloc s(isinst ...); stloc v(ldloc s)`), when all other uses of V are in blocks
/// dominated by trueBlock.
fn pattern_match_ref_types(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    container: NodeId,
    cfg: &mut Option<ControlFlowGraph>,
) -> bool {
    let Some((mut condition, mut true_inst, mut false_inst)) = f.match_if_at_end_of_block(block)
    else {
        return false;
    };
    let list = f.list(block).to_vec();
    let mut pos = list.len() as isize - 3;
    if let Some(condition_var) = f.match_ldloc(condition) {
        // stloc conditionVar(comp.o(ldloc s == ldnull))
        // if (logic.not(ldloc conditionVar)) br trueBlock
        if pos < 0 {
            return false;
        }
        let v = f.var(condition_var);
        if !(v.is_single_definition() && v.load_count() == 1 && v.kind == VariableKind::StackSlot) {
            return false;
        }
        let Some((var, value)) = f.match_stloc(list[pos as usize]) else {
            return false;
        };
        if var != condition_var {
            return false;
        }
        condition = value;
        pos -= 1;
    }
    let load_in_null_check = if let Some(l) = f.match_comp_null(condition, ComparisonKind::Equality)
    {
        std::mem::swap(&mut true_inst, &mut false_inst);
        l
    } else if let Some(l) = f.match_comp_null(condition, ComparisonKind::Inequality) {
        l
    } else {
        return false;
    };
    let Some(s) = f.match_ldloc(load_in_null_check) else {
        return false;
    };
    if !f.var(s).is_single_definition()
        || !matches!(f.var(s).kind, VariableKind::Local | VariableKind::StackSlot)
        || pos < 0
    {
        return false;
    }
    // stloc V(isinst T(testedOperand))
    let store_to_v = list[pos as usize];
    let Some((v, mut value)) = f.match_stloc(store_to_v) else {
        return false;
    };
    if f.match_ldloc(value) == Some(s) {
        // stloc v(ldloc s)
        pos -= 1;
        if pos < 0 {
            return false;
        }
        let Some((s2, value2)) = f.match_stloc(list[pos as usize]) else {
            return false;
        };
        if s2 != s {
            return false;
        }
        value = value2;
        if !matches!(f.var(v).kind, VariableKind::Local | VariableKind::StackSlot)
            || f.var(s).load_count() != 2
        {
            return false;
        }
    } else if v != s {
        return false;
    }
    // stloc S(unbox.any T(isinst T(testedOperand)))
    let unbox_type = if let Inst::UnboxAny { arg, ty } = *f.inst(value) {
        value = arg;
        Some(ty.ty)
    } else {
        None
    };
    let Inst::IsInst { arg: tested, ty } = *f.inst(value) else {
        return false;
    };
    let ty = ty.ty;
    if ctx.ts.is_reference_type(ty) != Some(true)
        || unbox_type.is_some_and(|u| u != ty)
        || f.var(v).ty != ty
    {
        return false;
    }
    if !check_all_uses_dominated_by(
        f,
        v,
        container,
        true_inst,
        store_to_v,
        Some(load_in_null_check),
        cfg,
    ) {
        return false;
    }
    // if (match.type[T].notnull(V = testedOperand)) br trueBlock
    let if_inst = list[list.len() - 2];
    let (old_condition, _) = if_parts(f, if_inst);
    let m = new_match(f, v, tested);
    set_match_flags(f, m, true, true);
    f.add_il_range(m, f.il_range(old_condition));
    f.replace(old_condition, m);
    set_true_inst(f, if_inst, true_inst);
    set_last(f, block, false_inst);
    let if_index = f.node(if_inst).child_index as usize;
    for _ in pos as usize..if_index {
        f.remove_at(block, pos as usize);
    }
    f.var_mut(v).kind = VariableKind::PatternLocal;
    detect_property_sub_patterns(f, ctx, m, true_inst, false_inst, container, cfg);
    true
}

/// `DetectPropertySubPatterns`: property and field patterns of the blocks the pattern's true
/// branch runs.
fn detect_property_sub_patterns(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    parent_pattern: NodeId,
    mut true_inst: NodeId,
    parent_false_inst: NodeId,
    container: NodeId,
    cfg: &mut Option<ControlFlowGraph>,
) -> NodeId {
    if !ctx.settings.recursive_pattern_matching {
        return true_inst;
    }
    loop {
        let true_block = if matches!(f.inst(true_inst), Inst::Block { .. }) {
            true_inst
        } else if let Some(b) = f.match_branch(true_inst) {
            b
        } else {
            break;
        };
        if !(f.node(true_block).incoming_edge_count == 1
            && f.node(true_block).parent == Some(container))
        {
            break;
        }
        match detect_property_sub_pattern(
            f,
            ctx,
            parent_pattern,
            true_block,
            parent_false_inst,
            cfg,
        ) {
            Some(next) => true_inst = next,
            None => break,
        }
    }
    true_inst
}

/// `DetectPropertySubPattern`.
fn detect_property_sub_pattern(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    parent_pattern: NodeId,
    block: NodeId,
    parent_false_inst: NodeId,
    cfg: &mut Option<ControlFlowGraph>,
) -> Option<NodeId> {
    let parent_var = match_variable(f, parent_pattern);
    // if (match.notnull.type[System.String] (V_0 = callvirt get_C(ldloc V_2))) br IL_0022
    // br IL_0037
    if let Some((condition, mut true_inst, mut false_inst)) =
        match_block_containing_one_condition(f, block)
    {
        let mut negate = false;
        if !compatible(f, parent_false_inst, false_inst) {
            if !compatible(f, parent_false_inst, true_inst) {
                return None;
            }
            std::mem::swap(&mut true_inst, &mut false_inst);
            negate = true;
        }
        let sub_pattern = if let Some(operand) = is_pattern_match(f, ctx, condition) {
            let (target, _) = property_or_field_access(f, ctx, operand)?;
            if f.match_ld_loc_ref(ctx.ts, target) != Some(parent_var) {
                return None;
            }
            if negate && !ctx.settings.pattern_combinators {
                return None;
            }
            if negate {
                logic_not(f, ctx, condition)
            } else {
                condition
            }
        } else if let Some((target, _)) = property_or_field_access(f, ctx, condition) {
            if f.match_ld_loc_ref(ctx.ts, target) != Some(parent_var) {
                return None;
            }
            if !negate && !ctx.settings.pattern_combinators {
                return None;
            }
            // Sub pattern: implicit != 0.
            let zero = f.add(Inst::LdcI4 { value: 0 });
            let kind = if negate {
                ComparisonKind::Equality
            } else {
                ComparisonKind::Inequality
            };
            comp(f, ctx, kind, condition, zero)
        } else {
            return None;
        };
        f.push(parent_pattern, sub_pattern);
        reduce_to(f, block, true_inst);
        return Some(true_inst);
    }
    let &first = f.list(block).first()?;
    let (target_variable, operand) = f.match_stloc(first)?;
    let (target, member_type) = property_or_field_access(f, ctx, operand)?;
    if f.match_ld_loc_ref(ctx.ts, target) != Some(parent_var) {
        return None;
    }
    if f.var(target_variable).ty != member_type {
        return None;
    }
    let block_container = f.node(block).parent.expect("block in a container");
    if !check_all_uses_dominated_by(f, target_variable, block_container, block, first, None, cfg) {
        return None;
    }
    // Property var pattern.
    let var_pattern = new_match(f, target_variable, operand);
    f.add_il_range(var_pattern, f.il_range(first));
    f.push(parent_pattern, var_pattern);
    f.remove_at(block, 0);
    f.var_mut(target_variable).kind = VariableKind::PatternLocal;
    let ty = f.var(target_variable).ty;
    if known(ctx, ty) == Some(KnownTypeCode::NullableOfT) {
        return Some(
            match_nullable_has_value_check_pattern(
                f,
                ctx,
                block,
                var_pattern,
                parent_false_inst,
                cfg,
            )
            .unwrap_or(block),
        );
    }
    if let Some(after_null_check) =
        match_null_check_pattern(f, block, var_pattern, parent_false_inst)
    {
        Some(detect_property_sub_patterns(
            f,
            ctx,
            var_pattern,
            after_null_check,
            parent_false_inst,
            block_container,
            cfg,
        ))
    } else if ctx.ts.is_reference_type(ty) == Some(false) {
        Some(detect_property_sub_patterns(
            f,
            ctx,
            var_pattern,
            block,
            parent_false_inst,
            block_container,
            cfg,
        ))
    } else {
        Some(block)
    }
}

/// `MatchNullCheckPattern`: `if (ldloc v != null)` after a var pattern makes it `notnull`.
fn match_null_check_pattern(
    f: &mut Function,
    block: NodeId,
    var_pattern: NodeId,
    parent_false_inst: NodeId,
) -> Option<NodeId> {
    let (condition, mut true_inst, mut false_inst) =
        match_block_containing_one_condition(f, block)?;
    let var = match_variable(f, var_pattern);
    if let Some(arg) = f.match_comp_null(condition, ComparisonKind::Equality)
        && f.match_ldloc(arg) == Some(var)
    {
        std::mem::swap(&mut true_inst, &mut false_inst);
    } else if let Some(arg) = f.match_comp_null(condition, ComparisonKind::Inequality)
        && f.match_ldloc(arg) == Some(var)
    {
    } else {
        return None;
    }
    if !compatible(f, false_inst, parent_false_inst) {
        return None;
    }
    set_match_flags(f, var_pattern, false, true);
    reduce_to(f, block, true_inst);
    Some(true_inst)
}

/// `NullableLiftingTransform.MatchHasValueCall(inst, v)`: `call get_HasValue(ldloca v)`.
fn match_has_value_call_of(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    v: VarId,
) -> bool {
    crate::il::control_flow::switch_detection::match_has_value_call(f, ctx, inst)
        .is_some_and(|arg| matches!(*f.inst(arg), Inst::LdLoca { var } if var == v))
}

/// `NullableLiftingTransform.MatchGetValueOrDefault(inst, v)`:
/// `call GetValueOrDefault(ldloca v)`.
fn match_get_value_or_default_of(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    v: VarId,
) -> bool {
    let Inst::Call(c) = f.inst(inst) else {
        return false;
    };
    let [arg] = c.args[..] else {
        return false;
    };
    c.method.method.name == "GetValueOrDefault"
        && c.method
            .method
            .declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::NullableOfT))
        && matches!(*f.inst(arg), Inst::LdLoca { var } if var == v)
}

/// `MatchNullableHasValueCheckPattern`: a `HasValue` check of a nullable var pattern becomes
/// a null or not-null pattern, or a lifted comparison with a constant.
fn match_nullable_has_value_check_pattern(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    var_pattern: NodeId,
    parent_false_inst: NodeId,
    cfg: &mut Option<ControlFlowGraph>,
) -> Option<NodeId> {
    let var = match_variable(f, var_pattern);
    if !(f.var(var).store_count() == 1 && f.var(var).load_count() == 0) {
        return None;
    }
    let (condition, true_inst, false_inst) = match_block_containing_one_condition(f, block)?;
    if !match_has_value_call_of(f, ctx, condition, var) {
        return None;
    }
    let nullable_type = f.var(var).ty;
    let underlying = ctx
        .ts
        .type_arguments(nullable_type)
        .first()
        .copied()
        .unwrap_or(nullable_type);
    let underlying_stack_type = ctx.ts.stack_type(underlying);
    let lifted_null_comp = |f: &mut Function, kind: ComparisonKind| {
        let tested = tested_operand(f, var_pattern);
        let default = f.add(Inst::DefaultValue {
            ty: TypeOperand {
                token: 0,
                ty: nullable_type,
            },
            il_stack_was_empty: false,
        });
        f.add(Inst::Comp {
            kind,
            lifting: ComparisonLiftingKind::CSharp,
            sign: Sign::None,
            input: underlying_stack_type,
            left: tested,
            right: default,
        })
    };
    if !compatible(f, false_inst, parent_false_inst) {
        if compatible(f, true_inst, parent_false_inst) {
            if f.var(var).address_count() != 1 {
                return None;
            }
            // Nullable.HasValue check -> null pattern.
            let null_comp = lifted_null_comp(f, ComparisonKind::Equality);
            f.replace(var_pattern, null_comp);
            reduce_to(f, block, false_inst);
            return Some(false_inst);
        }
        return None;
    }
    if f.var(var).address_count() == 1 && ctx.settings.pattern_combinators {
        // Nullable.HasValue check -> not null pattern.
        let not_null_comp = lifted_null_comp(f, ComparisonKind::Inequality);
        f.replace(var_pattern, not_null_comp);
        reduce_to(f, block, true_inst);
        return Some(true_inst);
    } else if f.var(var).address_count() != 2 {
        return None;
    }
    let true_block = f.match_branch(true_inst)?;
    if !(f.node(true_block).parent == f.node(block).parent
        && f.node(true_block).incoming_edge_count == 1)
    {
        return None;
    }
    if let Some(&first) = f.list(true_block).first()
        && let Some((new_target_variable, get_value_or_default)) = f.match_stloc(first)
        && match_get_value_or_default_of(f, ctx, get_value_or_default, var)
    {
        // Nullable.HasValue check + Nullable.GetValueOrDefault pattern.
        set_match_flags(f, var_pattern, false, true);
        f.set_variable(var_pattern, new_target_variable);
        f.var_mut(new_target_variable).kind = VariableKind::PatternLocal;
        reduce_to(f, block, true_inst);
        f.remove_at(true_block, 0);
        let block_container = f.node(block).parent.expect("block in a container");
        return Some(detect_property_sub_patterns(
            f,
            ctx,
            var_pattern,
            true_block,
            parent_false_inst,
            block_container,
            cfg,
        ));
    }
    let (condition, mut true_inst, mut false_inst) =
        match_block_containing_one_condition(f, true_block)?;
    let Inst::Comp {
        kind, left, right, ..
    } = *f.inst(condition)
    else {
        return None;
    };
    if !(is_constant(f, ctx, right) && match_get_value_or_default_of(f, ctx, left, var)) {
        return None;
    }
    if !(ctx.settings.relational_patterns
        || matches!(kind, ComparisonKind::Equality | ComparisonKind::Inequality))
    {
        return None;
    }
    let mut negated = false;
    if !compatible(f, false_inst, parent_false_inst) {
        if !compatible(f, true_inst, parent_false_inst) {
            return None;
        }
        std::mem::swap(&mut true_inst, &mut false_inst);
        negated = true;
    }
    let rejected = if negated {
        ComparisonKind::Equality
    } else {
        ComparisonKind::Inequality
    };
    if kind == rejected || (negated && !ctx.settings.pattern_combinators) {
        return None;
    }
    // Nullable.HasValue check + Nullable.GetValueOrDefault pattern:
    // comp.i4(call GetValueOrDefault(ldloca v) != ldc.i4 42) => comp.i4.lifted(testedOperand != ldc.i4 42)
    reduce_to(f, block, true_inst);
    f.truncate(true_block, 0);
    let tested = tested_operand(f, var_pattern);
    f.replace(left, tested);
    if let Inst::Comp { lifting, .. } = f.inst_mut(condition) {
        *lifting = ComparisonLiftingKind::CSharp;
    }
    let result = if negated {
        logic_not(f, ctx, condition)
    } else {
        condition
    };
    f.replace(var_pattern, result);
    Some(true_inst)
}

/// `PropertyOrFieldAccess`: a getter call or a field load; its target and the member's type.
fn property_or_field_access(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    operand: NodeId,
) -> Option<(NodeId, TypeId)> {
    if let Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) = f.inst(operand) {
        let [target] = c.args[..] else {
            return None;
        };
        let ty = ctx.ts.getter_property_type(&c.method.method)?;
        return Some((target, ty));
    }
    let (target, field) = f.match_ldfld(ctx.ts, operand)?;
    Some((target, field.field.ty))
}

/// `MatchBlockContainingOneCondition`: a block of an if and its else branch, the condition
/// possibly stored in a temporary first.
fn match_block_containing_one_condition(
    f: &Function,
    block: NodeId,
) -> Option<(NodeId, NodeId, NodeId)> {
    match f.list(block).len() {
        2 => f.match_if_at_end_of_block(block),
        3 => {
            let (load_temp, mut true_inst, mut false_inst) = f.match_if_at_end_of_block(block)?;
            let temp = f.match_ldloc(load_temp)?;
            if !(f.var(temp).is_single_definition() && f.var(temp).load_count() == 1) {
                return None;
            }
            let (v, mut condition) = f.match_stloc(f.list(block)[0])?;
            if v != temp {
                return None;
            }
            while let Some(arg) = f.match_logic_not(condition) {
                condition = arg;
                std::mem::swap(&mut true_inst, &mut false_inst);
            }
            Some((condition, true_inst, false_inst))
        }
        _ => None,
    }
}

/// `MatchInstruction.IsPatternMatch`: a pattern (a match, a comparison with a constant, a
/// string or decimal equality); its tested operand.
fn is_pattern_match(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> Option<NodeId> {
    match *f.inst(inst) {
        Inst::Match { tested_operand, .. } => Some(tested_operand),
        Inst::Comp {
            kind, left, right, ..
        } => {
            if let Some(operand) = f.match_logic_not(inst)
                && let Some(tested) = is_pattern_match(f, ctx, operand)
            {
                return ctx.settings.pattern_combinators.then_some(tested);
            }
            if !ctx.settings.relational_patterns
                && !matches!(kind, ComparisonKind::Equality | ComparisonKind::Inequality)
            {
                return None;
            }
            if !ctx.settings.pattern_combinators && kind == ComparisonKind::Inequality {
                return None;
            }
            is_constant(f, ctx, right).then_some(left)
        }
        Inst::Call(ref c) => {
            let [tested, value] = c.args[..] else {
                return None;
            };
            let m = &c.method.method;
            if m.name != "op_Equality" || !ctx.ts.method_is_operator(m) {
                return None;
            }
            let code = m.declaring.and_then(|d| known(ctx, d));
            match (code, f.inst(value)) {
                (Some(KnownTypeCode::String), Inst::LdStr { .. }) => Some(tested),
                (Some(KnownTypeCode::Decimal), Inst::LdcDecimal { .. }) => Some(tested),
                _ => None,
            }
        }
        _ => None,
    }
}

/// `MatchInstruction.IsConstant`.
fn is_constant(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    match *f.inst(inst) {
        Inst::LdcDecimal { .. }
        | Inst::LdcF4 { .. }
        | Inst::LdcF8 { .. }
        | Inst::LdcI4 { .. }
        | Inst::LdcI8 { .. }
        | Inst::LdNull => true,
        Inst::DefaultValue { ty, .. } => {
            f.result_type(ctx.ts, inst) == StackType::Obj
                || known(ctx, ty.ty) == Some(KnownTypeCode::NullableOfT)
        }
        _ => false,
    }
}

/// `CheckAllUsesDominatedBy`: every use of `v` (except the given store and load) is in a
/// block dominated by the target of `true_inst`.
fn check_all_uses_dominated_by(
    f: &Function,
    v: VarId,
    container: NodeId,
    true_inst: NodeId,
    store_to_v: NodeId,
    load_in_null_check: Option<NodeId>,
    cfg: &mut Option<ControlFlowGraph>,
) -> bool {
    let target_block = if matches!(f.inst(true_inst), Inst::Block { .. }) {
        true_inst
    } else if let Some(b) = f.match_branch(true_inst) {
        b
    } else {
        return false;
    };
    if f.node(target_block).parent != Some(container)
        || f.node(target_block).incoming_edge_count != 1
    {
        return false;
    }
    let cfg = cfg.get_or_insert_with(|| ControlFlowGraph::new(f, container));
    let target_node = cfg.node_of(target_block);
    let var = f.var(v);
    let uses = var
        .load_instructions
        .iter()
        .chain(&var.address_instructions)
        .chain(&var.store_instructions);
    for &u in uses {
        if u == store_to_v || Some(u) == load_in_null_check {
            continue;
        }
        let mut found = None;
        let mut current = Some(u);
        while let Some(c) = current {
            let parent = f.node(c).parent;
            if parent == Some(container) {
                found = Some(c);
                break;
            }
            current = parent;
        }
        let Some(found) = found else {
            return false;
        };
        if !cfg.dominates(target_node, cfg.node_of(found)) {
            return false;
        }
    }
    true
}

/// `PatternMatchValueTypes`.
///
/// ```text
/// [stloc temp(ldloc testedOperand)]
/// if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
/// br unboxBlock
/// --
/// Block unboxBlock (incoming: 1) {
///     stloc V(unbox.any T(ldloc temp))
///     ...
/// }
/// =>
/// if (match.type[T].notnull(V = testedOperand)) br unboxBlock
/// br falseBlock
/// ```
fn pattern_match_value_types(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    container: NodeId,
    cfg: &mut Option<ControlFlowGraph>,
) -> bool {
    let Some((ty, tested_operand, tested_variable, box_type1, unbox_block, false_inst)) =
        match_is_inst_block(f, ctx, block)
    else {
        return false;
    };
    let list = f.list(block).to_vec();
    let temp_store = list.len().checked_sub(3).map(|i| list[i]).filter(|&s| {
        f.match_stloc(s)
            .is_some_and(|(_, value)| f.match_ldloc(value) == Some(tested_variable))
    });
    let temp_var = temp_store.and_then(|s| f.match_stloc(s)).map(|(v, _)| v);
    let Some((unbox_operand, box_type2, store_to_v)) = match_unbox_block(f, ctx, unbox_block, ty)
    else {
        return false;
    };
    if box_type1 != box_type2 {
        return false;
    }
    if unbox_operand == tested_variable {
    } else if Some(unbox_operand) == temp_var {
        let t = f.var(unbox_operand);
        if !(t.is_single_definition() && t.load_count() == 1) {
            return false;
        }
    } else {
        return false;
    }
    let (v, _) = f.match_stloc(store_to_v).expect("a store");
    if !check_all_uses_dominated_by(f, v, container, unbox_block, store_to_v, None, cfg) {
        return false;
    }
    let if_inst = list[list.len() - 2];
    let (old_condition, _) = if_parts(f, if_inst);
    let m = new_match(f, v, tested_operand);
    set_match_flags(f, m, true, true);
    f.replace(old_condition, m);
    let br = f.add(Inst::Branch {
        target: crate::il::ast::BranchTarget::Block(unbox_block),
    });
    set_true_inst(f, if_inst, br);
    set_last(f, block, false_inst);
    f.remove_at(unbox_block, 0);
    if Some(unbox_operand) == temp_var {
        f.remove(temp_store.expect("a temp store"));
    }
    // Condition detection picks the then-branch by the blocks' start offsets: the unbox block
    // starts at an instruction of the then-block, not inside the pattern matching machinery.
    if let Some(&first) = f.list(unbox_block).first() {
        f.set_il_range(unbox_block, f.il_range(first));
    }
    f.var_mut(v).kind = VariableKind::PatternLocal;
    detect_property_sub_patterns(f, ctx, m, unbox_block, false_inst, container, cfg);
    true
}

/// `MatchIsInstBlock`: `if (comp.o(isinst T(ldloc v) == ldnull)) br falseBlock; br unboxBlock`,
/// the tested value possibly boxed from a type parameter; the type, the tested operand and
/// variable, the box type, the unbox block and the false branch.
#[allow(clippy::type_complexity)]
fn match_is_inst_block(
    f: &Function,
    ctx: &Context<'_, '_, '_>,
    block: NodeId,
) -> Option<(TypeId, NodeId, VarId, Option<TypeId>, NodeId, NodeId)> {
    let (condition, mut true_inst, mut false_inst) = f.match_if_at_end_of_block(block)?;
    let arg = if let Some(a) = f.match_comp_null(condition, ComparisonKind::Equality) {
        std::mem::swap(&mut true_inst, &mut false_inst);
        a
    } else {
        f.match_comp_null(condition, ComparisonKind::Inequality)?
    };
    let Inst::IsInst {
        arg: tested_operand,
        ty,
    } = *f.inst(arg)
    else {
        return None;
    };
    // The box type is that of any box, though only one of a type parameter is looked through.
    let (box_arg, box_type) = match *f.inst(tested_operand) {
        Inst::Box { arg, ty: bt } if is_type_parameter(ctx, bt.ty) => (arg, Some(bt.ty)),
        Inst::Box { ty: bt, .. } => (tested_operand, Some(bt.ty)),
        _ => (tested_operand, None),
    };
    let tested_variable = f.match_ldloc(box_arg)?;
    let unbox_block = f.match_branch(true_inst)?;
    if f.node(unbox_block).parent != f.node(block).parent {
        return None;
    }
    Some((
        ty.ty,
        tested_operand,
        tested_variable,
        box_type,
        unbox_block,
        false_inst,
    ))
}

/// `MatchUnboxBlock`: `stloc V(unbox.any T(ldloc v))` (or through `isinst T(box ``0(...))`)
/// first in the unbox block; the tested variable, the box type and the store.
fn match_unbox_block(
    f: &Function,
    ctx: &Context<'_, '_, '_>,
    unbox_block: NodeId,
    ty: TypeId,
) -> Option<(VarId, Option<TypeId>, NodeId)> {
    if f.node(unbox_block).incoming_edge_count != 1 {
        return None;
    }
    let &store_to_v = f.list(unbox_block).first()?;
    let (_, value) = f.match_stloc(store_to_v)?;
    let Inst::UnboxAny { mut arg, ty: t } = *f.inst(value) else {
        return None;
    };
    if t.ty != ty {
        return None;
    }
    if let Inst::IsInst {
        arg: isinst_arg,
        ty: isinst_type,
    } = *f.inst(arg)
        && isinst_type.ty == ty
    {
        arg = isinst_arg;
    }
    let mut box_type = None;
    if let Inst::Box {
        arg: box_arg,
        ty: bt,
    } = *f.inst(arg)
    {
        box_type = Some(bt.ty);
        if is_type_parameter(ctx, bt.ty) {
            arg = box_arg;
        }
    }
    let tested_variable = f.match_ldloc(arg)?;
    if box_type.is_some_and(|b| b != f.var(tested_variable).ty) {
        return None;
    }
    Some((tested_variable, box_type, store_to_v))
}
