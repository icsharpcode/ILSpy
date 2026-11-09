//! `IL/Transforms/ExpressionTransforms.cs`: expression-level clean-ups of one statement
//! (negations pushed into comparisons, canonical logic and/or, redundant conversions and
//! boxes, conditional operators).
//!
//! The statement visitor skips control-flow blocks and the bodies of containers: as a block
//! transform it runs after the blocks inside were handled.

use crate::il::ast::{
    BlockKind, ComparisonKind, ComparisonLiftingKind, ContainerKind, ConversionKind, Function,
    Inst, NodeId, TypeOperand, conversion_kind,
};
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::early_expression_transforms::{
    address_of_ldloc_to_ldloca, fix_comparison_kind_ld_null, ldobj_to_ldloc, stobj_to_stloc,
};
use super::il_inlining::known;

/// `ExpressionTransforms.RunOnSingleStatement`.
pub fn run_on_single_statement(f: &mut Function, ctx: &mut Context<'_, '_, '_>, statement: NodeId) {
    debug_assert!(
        f.node(statement)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::Block { .. })),
        "a statement is a direct child of a block"
    );
    // The managed method runs on a fresh statement context, which drops rerun requests.
    let rerun_requested = ctx.rerun_requested;
    visit(f, ctx, statement);
    ctx.rerun_requested = rerun_requested;
}

/// `IStatementTransform.Run` of `ExpressionTransforms`: the statement at `pos`.
pub fn run_statement(f: &mut Function, ctx: &mut Context<'_, '_, '_>, block: NodeId, pos: usize) {
    let statement = f.list(block)[pos];
    visit(f, ctx, statement);
}

/// `Default`: the children in order, each looked up when its turn comes.
fn visit_children(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    let count = f.inst(inst).children().len();
    for i in 0..count {
        let Some(&child) = f.inst(inst).children().get(i) else {
            break;
        };
        visit(f, ctx, child);
    }
}

fn visit(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    match *f.inst(inst) {
        Inst::BlockContainer { kind, .. } => {
            if kind == ContainerKind::Switch {
                // Only the switch value of the condition block.
                let entry = f.list(inst)[0];
                let switch = f.list(entry)[0];
                let Inst::Switch { value, .. } = *f.inst(switch) else {
                    panic!("a switch container starts with the switch");
                };
                visit(f, ctx, value);
                handle_switch_expression(f, ctx, inst, switch);
            }
        }
        Inst::Block { kind, .. } => {
            if kind != BlockKind::ControlFlow {
                visit_children(f, ctx, inst);
            }
        }
        Inst::Comp { .. } => visit_comp(f, ctx, inst),
        Inst::Conv { .. } => visit_conv(f, ctx, inst),
        Inst::Box { arg, ty } => {
            visit(f, ctx, arg);
            let arg = f.inst(inst).children()[0];
            // For reference types, box is a no-op.
            if ctx.ts.is_reference_type(ty.ty) == Some(true)
                && f.result_type(ctx.ts, arg) == f.result_type(ctx.ts, inst)
            {
                f.add_il_range(arg, f.il_range(inst));
                f.replace(inst, arg);
            }
        }
        Inst::LdElema { .. } | Inst::NewArr { .. } => {
            visit_children(f, ctx, inst);
            clean_up_array_indices(f, inst);
            if matches!(f.inst(inst), Inst::LdElema { .. }) {
                super::index_range_transform::handle_ld_elema(f, ctx, inst);
            }
        }
        Inst::LdObj { .. } => {
            visit_children(f, ctx, inst);
            address_of_ldloc_to_ldloca(f, ctx.ts, inst);
            if ldobj_to_ldloc(f, ctx.ts, inst) {
                return;
            }
            transform_decimal_field_to_constant(f, ctx, inst);
        }
        Inst::LdObjIfRef { .. } => visit_ld_obj_if_ref(f, ctx, inst),
        Inst::StObj { .. } => {
            visit_children(f, ctx, inst);
            if stobj_to_stloc(f, ctx.ts, inst) {
                ctx.rerun_requested = true;
            } else {
                super::transform_assignment::handle_compound_assign(f, ctx, inst);
            }
        }
        Inst::If { .. } => visit_if(f, ctx, inst),
        Inst::StLoc { .. } => {
            visit_children(f, ctx, inst);
            super::transform_assignment::handle_compound_assign(f, ctx, inst);
        }
        Inst::Call(_) => visit_call(f, ctx, inst),
        Inst::CallVirt(_) => {
            visit_children(f, ctx, inst);
            // shortcut: TransformCollectionAndObjectInitializers.HandleEmptyWith is not ported
            // yet; it comes with that transform (item 45).
            super::transform_assignment::handle_compound_assign(f, ctx, inst);
        }
        Inst::NewObj(_) => visit_new_obj(f, ctx, inst),
        Inst::Dynamic {
            kind: crate::il::ast::DynamicKind::SetMember,
            ..
        } => {
            visit_children(f, ctx, inst);
            transform_dynamic_set_member(f, inst);
        }
        Inst::Dynamic {
            kind: crate::il::ast::DynamicKind::SetIndex,
            ..
        } => {
            visit_children(f, ctx, inst);
            transform_dynamic_set_index(f, inst);
        }
        Inst::TryCatchHandler { .. } => {
            visit_children(f, ctx, inst);
            let Inst::TryCatchHandler { filter, body, .. } = *f.inst(inst) else {
                unreachable!("a catch handler");
            };
            if matches!(f.inst(filter), Inst::BlockContainer { .. }) && f.list(filter).len() == 1 {
                let entry = f.list(filter)[0];
                transform_catch_when(f, inst, entry);
            }
            let Inst::TryCatchHandler { body: new_body, .. } = *f.inst(inst) else {
                unreachable!("a catch handler");
            };
            debug_assert_eq!(body, new_body);
            if matches!(f.inst(body), Inst::BlockContainer { .. }) {
                let entry = f.list(body)[0];
                transform_catch_variable(f, inst, entry, true);
            }
        }
        Inst::BinaryNumeric { .. } => {
            visit_children(f, ctx, inst);
            visit_binary_numeric(f, ctx, inst);
        }
        Inst::Match {
            tested_operand,
            ref sub_patterns,
            ..
        } => {
            let sub_patterns = sub_patterns.clone();
            visit(f, ctx, tested_operand);
            // Sub-patterns allow only certain instructions; only their negations simplify.
            for sub_pattern in sub_patterns {
                for node in f.descendants_post(sub_pattern) {
                    if f.is_connected(node)
                        && let Some(arg) = f.match_logic_not(node)
                    {
                        handle_logic_not(f, ctx, node, arg);
                    }
                }
            }
        }
        // shortcut: the NullableLifting, TransformAssignment, UserDefinedLogic, array and span initializer, inline array, delegate and catch rewrites of ExpressionTransforms are not ported yet; the equality gate shows where they matter
        _ => visit_children(f, ctx, inst),
    }
}

fn comp_parts(
    f: &Function,
    comp: NodeId,
) -> (ComparisonKind, ComparisonLiftingKind, Sign, NodeId, NodeId) {
    match *f.inst(comp) {
        Inst::Comp {
            kind,
            lifting,
            sign,
            left,
            right,
            ..
        } => (kind, lifting, sign, left, right),
        ref other => panic!("{other:?} is not a comparison"),
    }
}

fn set_comp_kind(f: &mut Function, comp: NodeId, new: ComparisonKind) {
    if let Inst::Comp { kind, .. } = f.inst_mut(comp) {
        *kind = new;
    }
}

/// `UnwrapConv(kind)`.
pub(crate) fn unwrap_conv(f: &Function, inst: NodeId, kind: ConversionKind) -> NodeId {
    match *f.inst(inst) {
        Inst::Conv {
            arg,
            input,
            input_sign,
            target,
            ..
        } if conversion_kind(target, input, input_sign) == kind => arg,
        _ => inst,
    }
}

fn visit_comp(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    // `logic.not(arg)` is sugar for `comp(arg != ldc.i4 0)`.
    if let Some(arg) = f.match_logic_not(inst) {
        let to_visit = handle_logic_not(f, ctx, inst, arg);
        visit(f, ctx, to_visit.unwrap_or(arg));
        return;
    }
    let (kind, lifting, _, left, right) = comp_parts(f, inst);
    if kind == ComparisonKind::Inequality
        && lifting == ComparisonLiftingKind::None
        && f.match_ldc_i4(right) == Some(0)
        && ({
            let t = f.infer_type(ctx.ts, left);
            known(ctx, t) == Some(KnownTypeCode::Boolean)
        } || matches!(f.match_ldc_i4(left), Some(0 | 1)))
    {
        // `x` is known to be 0 or 1: `comp(x != 0) => x`.
        f.add_il_range(left, f.il_range(inst));
        f.replace(inst, left);
        visit(f, ctx, left);
        return;
    }
    if ctx.settings.lift_nullables {
        super::nullable_lifting_transform::run_comp(f, ctx, inst);
    }
    visit_children(f, ctx, inst);
    let (_, lifting, _, _, _) = comp_parts(f, inst);
    if lifting != ComparisonLiftingKind::None {
        return;
    }
    fix_comparison_kind_ld_null(f, ctx.ts, inst);
    // The parts after the children and FixComparisonKindLdNull changed them.
    let (kind, _, sign, left, right) = comp_parts(f, inst);
    let right_without_conv = unwrap_conv(
        f,
        unwrap_conv(f, right, ConversionKind::SignExtend),
        ConversionKind::ZeroExtend,
    );
    if f.match_ldc_i4(right_without_conv) == Some(0)
        && sign == Sign::Unsigned
        && matches!(
            kind,
            ComparisonKind::GreaterThan | ComparisonKind::LessThanOrEqual
        )
    {
        // comp.unsigned(left > 0) => comp(left != 0); comp.unsigned(left <= 0) => comp(left == 0)
        let new = if kind == ComparisonKind::GreaterThan {
            ComparisonKind::Inequality
        } else {
            ComparisonKind::Equality
        };
        set_comp_kind(f, inst, new);
        visit_comp(f, ctx, inst);
    } else if f.match_ldc_i4(right_without_conv) == Some(0) && kind.is_equality_or_inequality() {
        if let Inst::LdLen {
            array,
            result: StackType::I,
        } = *f.inst(left)
        {
            // comp(ldlen array == conv i4->i(ldc.i4 0)) => comp(ldlen.i4 array == ldc.i4 0)
            if let Inst::Comp { input, .. } = f.inst_mut(inst) {
                *input = StackType::I4;
            }
            let ld_len = f.add(Inst::LdLen {
                array,
                result: StackType::I4,
            });
            f.add_il_range(ld_len, f.il_range(left));
            f.replace(left, ld_len);
            f.set_child_at(inst, 1, right_without_conv);
        } else if let Inst::Conv {
            arg,
            target: PrimitiveType::I,
            ..
        } = *f.inst(left)
            && f.result_type(ctx.ts, arg) == StackType::Obj
        {
            // C++/CLI: comp(conv o->i (ldloc obj) == conv i4->i (ldc.i4 0)) => comp(ldloc obj == ldnull)
            if let Inst::Comp { input, .. } = f.inst_mut(inst) {
                *input = StackType::Obj;
            }
            f.set_child_at(inst, 0, arg);
            let null = f.add(Inst::LdNull);
            f.add_il_range(null, f.il_range(right));
            f.add_il_range(null, f.il_range(right_without_conv));
            f.set_child_at(inst, 1, null);
        }
    }
}

/// `HandleLogicNot`: a negation pushed into a comparison or a logic and/or; the instruction
/// that replaced `inst`, if any.
fn handle_logic_not(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    arg: NodeId,
) -> Option<NodeId> {
    if let Inst::Comp {
        kind,
        lifting,
        input,
        ..
    } = *f.inst(arg)
    {
        if (!input.is_float() && lifting == ComparisonLiftingKind::None)
            || kind.is_equality_or_inequality()
        {
            set_comp_kind(f, arg, kind.negate());
            f.add_il_range(arg, f.il_range(inst));
            f.replace(inst, arg);
            return Some(arg);
        }
    } else if let Some((lhs, rhs)) = match_logic_and(f, arg) {
        // logic.not(if (lhs) rhs else ldc.i4 0) => if (logic.not(lhs)) ldc.i4 1 else logic.not(rhs)
        let ldc0 = f.inst(arg).children()[2];
        let not_lhs = logic_not(f, ctx, lhs);
        f.add_il_range(not_lhs, f.il_range(inst));
        f.set_child_at(arg, 0, not_lhs);
        let one = f.add(Inst::LdcI4 { value: 1 });
        f.add_il_range(one, f.il_range(ldc0));
        f.set_child_at(arg, 1, one);
        let not_rhs = logic_not(f, ctx, rhs);
        f.add_il_range(not_rhs, f.il_range(inst));
        f.set_child_at(arg, 2, not_rhs);
        f.replace(inst, arg);
        return Some(arg);
    } else if let Some((lhs, rhs)) = match_logic_or(f, arg) {
        // logic.not(if (lhs) ldc.i4 1 else rhs) => if (logic.not(lhs)) logic.not(rhs) else ldc.i4 0
        let ldc1 = f.inst(arg).children()[1];
        let not_lhs = logic_not(f, ctx, lhs);
        f.add_il_range(not_lhs, f.il_range(inst));
        f.set_child_at(arg, 0, not_lhs);
        let not_rhs = logic_not(f, ctx, rhs);
        f.add_il_range(not_rhs, f.il_range(inst));
        f.set_child_at(arg, 1, not_rhs);
        let zero = f.add(Inst::LdcI4 { value: 0 });
        f.add_il_range(zero, f.il_range(ldc1));
        f.set_child_at(arg, 2, zero);
        f.replace(inst, arg);
        return Some(arg);
    }
    None
}

/// `MatchLogicAnd`: `if (lhs) rhs else ldc.i4 0`.
pub(crate) fn match_logic_and(f: &Function, inst: NodeId) -> Option<(NodeId, NodeId)> {
    match *f.inst(inst) {
        Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } if f.match_ldc_i4(false_inst) == Some(0) => Some((condition, true_inst)),
        _ => None,
    }
}

/// `MatchLogicOr`: `if (lhs) ldc.i4 1 else rhs`.
pub(crate) fn match_logic_or(f: &Function, inst: NodeId) -> Option<(NodeId, NodeId)> {
    match *f.inst(inst) {
        Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } if f.match_ldc_i4(true_inst) == Some(1) => Some((condition, false_inst)),
        _ => None,
    }
}

/// `Comp.LogicNot`.
pub(crate) fn logic_not(f: &mut Function, ctx: &mut Context<'_, '_, '_>, arg: NodeId) -> NodeId {
    let input = f.result_type(ctx.ts, arg);
    let zero = f.add(Inst::LdcI4 { value: 0 });
    f.add(Inst::Comp {
        kind: ComparisonKind::Equality,
        lifting: ComparisonLiftingKind::None,
        sign: Sign::None,
        input,
        left: arg,
        right: zero,
    })
}

fn visit_conv(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    let arg = f.inst(inst).children()[0];
    visit(f, ctx, arg);
    let Inst::Conv {
        arg,
        target,
        check_overflow,
        ..
    } = *f.inst(inst)
    else {
        return;
    };
    if let Inst::LdLen {
        array,
        result: StackType::I,
    } = *f.inst(arg)
        && target.is_integer()
        && (!check_overflow || ctx.settings.assume_array_length_fits_into_int32)
    {
        // conv.i4(ldlen array) => ldlen.i4(array)
        f.add_il_range(inst, f.il_range(arg));
        let ld_len = f.add(Inst::LdLen {
            array,
            result: target.stack_type(),
        });
        f.add_il_range(ld_len, f.il_range(inst));
        f.replace(inst, ld_len);
        return;
    }
    if target.is_float()
        && let Inst::Conv {
            arg: inner_arg,
            input,
            input_sign,
            target: PrimitiveType::R,
            is_lifted: inner_lifted,
            ..
        } = *f.inst(arg)
        && conversion_kind(PrimitiveType::R, input, input_sign) == ConversionKind::IntToFloat
    {
        // conv.rN(conv.r.un(...)) => conv.rN.un(...)
        let new = f.add(Inst::Conv {
            is_lifted: inner_lifted
                || matches!(
                    f.inst(inst),
                    Inst::Conv {
                        is_lifted: true,
                        ..
                    }
                ),
            arg: inner_arg,
            input,
            input_sign,
            target,
            check_overflow,
        });
        f.replace(inst, new);
    }
}

/// `CleanUpArrayIndices`: `conv.i` around an array index goes.
fn clean_up_array_indices(f: &mut Function, inst: NodeId) {
    let indices = f.list(inst).to_vec();
    for index in indices {
        if let Inst::Conv {
            arg,
            input,
            input_sign,
            target,
            check_overflow,
            is_lifted: false,
        } = *f.inst(index)
            && target.stack_type() == StackType::I
        {
            let kind = conversion_kind(target, input, input_sign);
            if (kind == ConversionKind::Truncate && check_overflow)
                || kind == ConversionKind::ZeroExtend
                || kind == ConversionKind::SignExtend
            {
                f.replace(index, arg);
            }
        }
    }
}

fn visit_ld_obj_if_ref(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    visit_children(f, ctx, inst);
    let target = f.inst(inst).children()[0];
    // There already is a temporary, so the ldobj.if.ref is a no-op.
    if matches!(f.inst(target), Inst::AddressOf { .. }) {
        f.replace(inst, target);
        return;
    }
    if let Some(s) = f.match_ldloc(target) {
        let var = f.var(s);
        if var.is_single_definition()
            && var.load_count() == 1
            && let [store] = var.store_instructions[..]
            && let Inst::StLoc { value, .. } = *f.inst(store)
            && let Inst::LdLoca { var: v } = *f.inst(value)
            && f.var(v).address_count() == 1
            && f.var(v).store_count() == 1
        {
            f.replace(inst, target);
        }
    }
}

fn if_parts(f: &Function, inst: NodeId) -> (NodeId, NodeId, NodeId) {
    match *f.inst(inst) {
        Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } => (condition, true_inst, false_inst),
        ref other => panic!("{other:?} is not an if"),
    }
}

fn visit_if(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    let (_, true_inst, _) = if_parts(f, inst);
    visit(f, ctx, true_inst);
    let (_, _, false_inst) = if_parts(f, inst);
    visit(f, ctx, false_inst);
    let inst = handle_conditional_operator(f, ctx, inst);
    // Canonical logic and/or:
    // if (cond) ldc.i4 0 else RHS => if (!cond) RHS else ldc.i4 0
    // if (cond) RHS else ldc.i4 1 => if (!cond) ldc.i4 1 else RHS
    // (not when both are 1, which would swap forever).
    let (condition, true_inst, false_inst) = if_parts(f, inst);
    if (f.match_ldc_i4(true_inst) == Some(0) && f.match_ldc_i4(false_inst) != Some(0))
        || (f.match_ldc_i4(false_inst) == Some(1) && f.match_ldc_i4(true_inst) != Some(1))
    {
        f.set_child_at(inst, 1, false_inst);
        f.set_child_at(inst, 2, true_inst);
        let not = logic_not(f, ctx, condition);
        f.set_child_at(inst, 0, not);
    }
    // The condition after those changes.
    let (condition, _, _) = if_parts(f, inst);
    visit(f, ctx, condition);
    if super::nullable_lifting_transform::run_if(f, ctx, inst) {
        return;
    }
    // shortcut: UserDefinedLogicTransform is not ported yet; it comes with item 51.
    if transform_dynamic_add_assign_or_remove_assign(f, inst) {
        return;
    }
    let (condition, true_inst, false_inst) = if_parts(f, inst);
    if is_pattern_match(f, ctx, condition)
        && f.match_ldc_i4(true_inst) == Some(1)
        && f.match_ldc_i4(false_inst) == Some(0)
    {
        // match(x) ? true : false => match(x)
        f.add_il_range(condition, f.il_range(inst));
        f.replace(inst, condition);
    }
}

/// `MatchInstruction.IsPatternMatch`, as far as this transform asks.
fn is_pattern_match(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    match *f.inst(inst) {
        Inst::Match { .. } => true,
        Inst::Comp { kind, right, .. } => {
            if let Some(operand) = f.match_logic_not(inst)
                && is_pattern_match(f, ctx, operand)
            {
                return ctx.settings.pattern_combinators;
            }
            if !ctx.settings.relational_patterns && !kind.is_equality_or_inequality() {
                return false;
            }
            if !ctx.settings.pattern_combinators && kind == ComparisonKind::Inequality {
                return false;
            }
            is_constant(f, ctx, right)
        }
        Inst::Call(ref c) => {
            let [_, value] = c.args[..] else {
                return false;
            };
            let m = &c.method.method;
            if m.name != "op_Equality" || !ctx.ts.method_is_operator(m) {
                return false;
            }
            matches!(
                (m.declaring.and_then(|d| known(ctx, d)), f.inst(value)),
                (Some(KnownTypeCode::String), Inst::LdStr { .. })
                    | (Some(KnownTypeCode::Decimal), Inst::LdcDecimal { .. })
            )
        }
        _ => false,
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

/// `HandleConditionalOperator`: `if (c) stloc A(V1) else stloc A(V2)` becomes
/// `stloc A(if (c) V1 else V2)`; the if that is left.
fn handle_conditional_operator(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> NodeId {
    let (condition, true_inst, false_inst) = if_parts(f, inst);
    let single = |f: &Function, b: NodeId| match f.inst(b) {
        Inst::Block { instructions, .. } if instructions.len() == 1 => Some(instructions[0]),
        _ => None,
    };
    let (Some(t), Some(fl)) = (single(f, true_inst), single(f, false_inst)) else {
        return inst;
    };
    let Some((v, value1)) = f.match_stloc(t) else {
        return inst;
    };
    let Some((v2, value2)) = f.match_stloc(fl) else {
        return inst;
    };
    if v != v2 {
        return inst;
    }
    let mut ty = f.var(v).ty;
    // A tighter `bool` type matters for logic and/or.
    let type1 = f.infer_type(ctx.ts, value1);
    let type2 = f.infer_type(ctx.ts, value2);
    let is_bool = |ctx: &mut Context<'_, '_, '_>, t| known(ctx, t) == Some(KnownTypeCode::Boolean);
    if is_bool(ctx, type1) && (is_bool(ctx, type2) || matches!(f.match_ldc_i4(value2), Some(0 | 1)))
    {
        ty = type1;
    } else if is_bool(ctx, type2) && matches!(f.match_ldc_i4(value1), Some(0 | 1)) {
        ty = type2;
    }
    let not = logic_not(f, ctx, condition);
    let new_if = f.add(Inst::If {
        condition: not,
        true_inst: value2,
        false_inst: value1,
        csharp_type: Some(ty),
    });
    f.add_il_range(new_if, f.il_range(inst));
    let store = f.add(Inst::StLoc {
        var: v,
        value: new_if,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.replace(inst, store);
    // The new store may be inlined.
    ctx.rerun_requested = true;
    new_if
}

/// `MatchBinaryNumericInstruction(op)`: the operands.
fn match_binary(
    f: &Function,
    inst: NodeId,
    want: crate::il::ast::BinaryNumericOperator,
) -> Option<(NodeId, NodeId)> {
    match *f.inst(inst) {
        Inst::BinaryNumeric {
            op, left, right, ..
        } if op == want => Some((left, right)),
        _ => None,
    }
}

/// `VisitBinaryNumericInstruction`: `a << (b & 31)` becomes `a << b`.
fn visit_binary_numeric(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    use crate::il::ast::BinaryNumericOperator as Op;
    let Inst::BinaryNumeric { op, right, .. } = *f.inst(inst) else {
        return;
    };
    if matches!(op, Op::ShiftLeft | Op::ShiftRight)
        && let Some((lhs, rhs)) = match_binary(f, right, Op::BitAnd)
        && match_expected_shift_size(f, ctx, inst, rhs)
    {
        f.set_child_at(inst, 1, lhs);
    }
    if matches!(op, Op::BitAnd | Op::BitOr | Op::BitXor) {
        let Inst::BinaryNumeric { left, right, .. } = *f.inst(inst) else {
            unreachable!("a binary instruction");
        };
        let left_type = f.infer_type(ctx.ts, left);
        let right_type = f.infer_type(ctx.ts, right);
        detect_improved_type_for_bit_op(f, ctx, inst, left_type, right_type);
        let boolean =
            |ctx: &mut Context<'_, '_, '_>, t| known(ctx, t) == Some(KnownTypeCode::Boolean);
        if op == Op::BitAnd && boolean(ctx, left_type) && boolean(ctx, right_type) {
            // e.g. `(a.GetValueOrDefault() == b.GetValueOrDefault()) & (a.HasValue & b.HasValue)`
            super::nullable_lifting_transform::run_bit_and(f, ctx, inst);
        }
    }
}

/// `DetectImprovedTypeForForBitOp`: `bit.or(a, b)` of two bools (or the same primitive integer
/// type) has that type.
fn detect_improved_type_for_bit_op(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    left_type: crate::typesystem::types::TypeId,
    right_type: crate::typesystem::types::TypeId,
) {
    use crate::typesystem::types::TypeData;
    if left_type != right_type {
        return;
    }
    let code = known(ctx, left_type);
    // `IsCSharpPrimitiveIntegerType`, `IsCSharpNativeIntegerType` or `bool`.
    let applies = matches!(
        code,
        Some(
            KnownTypeCode::Byte
                | KnownTypeCode::SByte
                | KnownTypeCode::Int16
                | KnownTypeCode::UInt16
                | KnownTypeCode::Int32
                | KnownTypeCode::UInt32
                | KnownTypeCode::Int64
                | KnownTypeCode::UInt64
                | KnownTypeCode::Boolean
        )
    ) || matches!(ctx.ts.data(left_type), TypeData::NInt | TypeData::NUInt);
    if !applies {
        return;
    }
    if let Inst::BinaryNumeric {
        csharp_result_type, ..
    } = f.inst_mut(inst)
        && *csharp_result_type != Some(left_type)
    {
        *csharp_result_type = Some(left_type);
    }
}

/// `MatchExpectedShiftSize`: the mask of the shift count for the result type.
fn match_expected_shift_size(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    rhs: NodeId,
) -> bool {
    use crate::il::ast::BinaryNumericOperator as Op;
    match f.result_type(ctx.ts, inst) {
        StackType::I4 => f.match_ldc_i4(rhs) == Some(31),
        StackType::I8 => f.match_ldc_i4(rhs) == Some(63),
        // sizeof(IntPtr) * 8 - 1
        StackType::I => {
            let Some((mult, one)) = match_binary(f, rhs, Op::Sub) else {
                return false;
            };
            let Some((size, eight)) = match_binary(f, mult, Op::Mul) else {
                return false;
            };
            matches!(*f.inst(size), Inst::SizeOf { ty } if ctx.ts.stack_type(ty.ty) == StackType::I)
                && f.match_ldc_i4(eight) == Some(8)
                && f.match_ldc_i4(one) == Some(1)
        }
        _ => false,
    }
}

/// `TransformDecimalFieldToConstant`: `decimal.One`, `MinusOne` and `Zero` become constants.
fn transform_decimal_field_to_constant(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) {
    let Some(field) = f.match_ldsfld(inst) else {
        return;
    };
    let field = field.field.clone();
    if field
        .declaring
        .is_none_or(|d| known(ctx, d) != Some(KnownTypeCode::Decimal))
    {
        return;
    }
    // `decimal.GetBits`: low, middle, high, flags (sign in bit 31).
    let bits = match field.name.as_str() {
        "One" => [1, 0, 0, 0],
        "MinusOne" => [1, 0, 0, 0x8000_0000],
        "Zero" => [0, 0, 0, 0],
        _ => return,
    };
    let target = f.inst(inst).children()[0];
    let constant = f.add(Inst::LdcDecimal { bits });
    f.add_il_range(constant, f.il_range(inst));
    f.add_il_range(constant, f.il_range(target));
    f.replace(inst, constant);
}

fn handler_variable(f: &Function, handler: NodeId) -> crate::il::ast::VarId {
    match *f.inst(handler) {
        Inst::TryCatchHandler { variable, .. } => variable,
        ref other => panic!("{other:?} is not a catch handler"),
    }
}

fn add_exception_specifier_il_range(
    f: &mut Function,
    handler: NodeId,
    range: crate::util::interval::Interval,
) {
    if let Inst::TryCatchHandler {
        exception_specifier_il_range,
        ..
    } = f.inst_mut(handler)
    {
        *exception_specifier_il_range =
            crate::il::ast::combine_il_range(*exception_specifier_il_range, range);
    }
}

/// `TransformCatchVariable`: `catch ex { stloc v(ldloc ex); ... }` becomes `catch v { ... }`
/// when `v` is used only inside the handler.
fn transform_catch_variable(
    f: &mut Function,
    handler: NodeId,
    entry: NodeId,
    is_catch_block: bool,
) {
    use crate::il::ast::VariableKind;
    let handler_var = handler_variable(f, handler);
    if !f.var(handler_var).is_single_definition() || f.var(handler_var).load_count() != 1 {
        // The handler variable has other uses.
        return;
    }
    let Some((exception_var, mut slot_load)) =
        f.list(entry).first().and_then(|&i| f.match_stloc(i))
    else {
        // Not the pattern with a second variable; an inlined unbox.any can still go.
        let load = f.var(handler_var).load_instructions[0];
        if let Some(parent) = f.node(load).parent
            && let Inst::UnboxAny { arg, ty } = *f.inst(parent)
            && ty.ty == f.var(handler_var).ty
        {
            let range = f.il_range(parent);
            f.replace(parent, arg);
            add_exception_specifier_il_range(f, handler, range);
        }
        return;
    };
    if !matches!(
        f.var(exception_var).kind,
        VariableKind::Local | VariableKind::StackSlot
    ) {
        return;
    }
    if let Inst::UnboxAny { arg, ty } = *f.inst(slot_load) {
        // Catching a type parameter: csc emits an unbox.any.
        if ty.ty != f.var(handler_var).ty {
            return;
        }
        slot_load = arg;
    }
    if f.match_ldloc(slot_load) != Some(handler_var) {
        return;
    }
    // The variable is used only inside the handler.
    let var = f.var(exception_var);
    let uses: Vec<NodeId> = var
        .load_instructions
        .iter()
        .chain(&var.store_instructions)
        .chain(&var.address_instructions)
        .copied()
        .collect();
    if !uses
        .iter()
        .all(|&u| u == handler || f.ancestors(u).any(|a| a == handler))
    {
        return;
    }
    let (name, ty, has_generated_name) = {
        let h = f.var(handler_var);
        (h.name.clone(), h.ty, h.has_generated_name)
    };
    let v = f.var_mut(exception_var);
    v.kind = VariableKind::ExceptionLocal;
    v.name = name;
    v.ty = ty;
    v.has_generated_name = has_generated_name;
    f.set_variable(handler, exception_var);
    let first = f.list(entry)[0];
    if is_catch_block {
        for node in f.descendants_post(first) {
            let range = f.il_range(node);
            add_exception_specifier_il_range(f, handler, range);
        }
    }
    f.remove_at(entry, 0);
}

/// `TransformCatchWhen`: a filter container of one `leave (condition)` becomes the condition.
fn transform_catch_when(f: &mut Function, handler: NodeId, entry: NodeId) {
    transform_catch_variable(f, handler, entry, false);
    if let [only] = *f.list(entry)
        && let Some((_, condition)) = f.match_leave(only)
    {
        f.set_child_at(handler, 0, condition);
    }
}

/// `HandleSwitchExpression`: a switch container whose sections each store the same variable
/// (or each return) and leave becomes a switch expression in that store (or return).
fn handle_switch_expression(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
    switch: NodeId,
) {
    if !ctx.settings.switch_expressions {
        return;
    }
    let default = crate::il::control_flow::switch_detection::default_section(f, switch);
    let mut leave_target: Option<NodeId> = None;
    let mut result_variable: Option<crate::il::ast::VarId> = None;
    for &section in f.list(switch) {
        let Inst::SwitchSection {
            ref labels,
            body,
            has_null_label,
            ..
        } = *f.inst(section)
        else {
            return;
        };
        // Every section except the default has exactly one label.
        if section != default && labels.count() != if has_null_label { 0 } else { 1 } {
            return;
        }
        let Some(section_block) = f.match_branch(body) else {
            return;
        };
        if f.node(section_block).incoming_edge_count != 1
            || f.node(section_block).parent != Some(container)
        {
            return;
        }
        match *f.list(section_block) {
            [only] => match *f.inst(only) {
                Inst::Throw { .. } => {}
                Inst::Leave {
                    target: Some(target),
                    ..
                } => {
                    if Some(target) != f.body {
                        return;
                    }
                    leave_target.get_or_insert(target);
                }
                _ => return,
            },
            [store, exit] => {
                let Some((v, _)) = f.match_stloc(store) else {
                    return;
                };
                if f.match_leave(exit)
                    .is_none_or(|(c, value)| c != container || !f.match_nop(value))
                {
                    return;
                }
                if *result_variable.get_or_insert(v) != v {
                    return;
                }
            }
            _ => return,
        }
    }
    // Exactly one of them.
    if result_variable.is_none() == leave_target.is_none() {
        return;
    }
    let result_type = match (result_variable, leave_target) {
        (Some(v), _) => f.var(v).ty,
        (None, Some(target)) => match *f.inst(target) {
            Inst::BlockContainer {
                expected: Some(t), ..
            } => t,
            _ => return,
        },
        (None, None) => unreachable!("one of them"),
    };
    // C# has no ref-returning switch expression.
    if matches!(
        ctx.ts.data(result_type),
        crate::typesystem::types::TypeData::ByRef(_)
    ) {
        return;
    }
    if let Inst::Switch { value, .. } = *f.inst(switch)
        && let Inst::StringToInt { ref map, .. } = *f.inst(value)
    {
        // Each integer is used for one value.
        let mut used = std::collections::HashSet::new();
        if !map.iter().all(|(_, v)| used.insert(*v)) {
            return;
        }
    }
    if let Inst::Switch {
        result_type: rt, ..
    } = f.inst_mut(switch)
    {
        *rt = Some(result_type);
    }
    for section in f.list(switch).to_vec() {
        let body = f.inst(section).children()[0];
        let block = f.branch_target(body);
        let first = f.list(block)[0];
        let new_body = match *f.inst(first) {
            Inst::Throw { .. } if f.list(block).len() == 1 => first,
            Inst::Leave { value, .. } if f.list(block).len() == 1 => value,
            Inst::StLoc { value, .. } => value,
            ref other => panic!("{other:?} in a switch expression section"),
        };
        f.set_child_at(section, 0, new_body);
    }
    let replacement = match result_variable {
        Some(v) => f.add(Inst::StLoc {
            var: v,
            value: switch,
            il_stack_was_empty: false,
            is_stack_adjustment: false,
        }),
        None => f.add(Inst::Leave {
            target: leave_target,
            value: switch,
        }),
    };
    f.replace(container, replacement);
    // The new store may be inlined.
    ctx.rerun_requested = true;
}

/// `VisitCall`.
fn visit_call(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    if let Some(replacement) = get_value_or_default_to_null_coalescing(f, ctx, inst) {
        visit(f, ctx, replacement);
        return;
    }
    if let Some(replacement) =
        super::transform_array_initializers::transform_runtime_helpers_create_span_initialization(
            f, ctx, inst,
        )
    {
        f.replace(inst, replacement);
        visit(f, ctx, replacement);
        return;
    }
    visit_children(f, ctx, inst);
    if ctx.settings.inline_arrays && super::inline_array_transform::run_on_expression(f, ctx, inst)
    {
        return;
    }
    // shortcut: TransformCollectionAndObjectInitializers.HandleEmptyWith, which runs when no
    // compound assignment is made, is not ported yet; it comes with that transform (item 45).
    super::transform_assignment::handle_compound_assign(f, ctx, inst);
}

/// `call Nullable<T>.GetValueOrDefault(a, b)` with a pure `b` becomes `a ?? b`.
fn get_value_or_default_to_null_coalescing(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let m = &c.method.method;
    let declaring = m.declaring?;
    if m.name != "GetValueOrDefault" || c.args.len() != 2 {
        return None;
    }
    let &[nullable_value, fallback] = &c.args[..] else {
        return None;
    };
    if known(ctx, declaring) != Some(KnownTypeCode::NullableOfT)
        || !crate::il::semantic_helper::is_pure(f.flags(fallback))
    {
        return None;
    }
    let ld_obj = f.add(Inst::LdObj {
        target: nullable_value,
        ty: TypeOperand {
            token: 0,
            ty: declaring,
        },
        is_volatile: false,
        unaligned: 0,
    });
    let ty = super::transform_assignment::nullable_underlying(ctx, declaring);
    let replacement = f.add(Inst::NullCoalescing {
        kind: crate::il::ast::NullCoalescingKind::NullableWithValueFallback,
        ty,
        value: ld_obj,
        fallback,
    });
    f.add_il_range(replacement, f.il_range(inst));
    f.replace(inst, replacement);
    Some(replacement)
}

/// `VisitNewObj`.
fn visit_new_obj(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    if let Some(span) = transform_span_t_ctor_containing_stack_alloc(f, ctx, inst) {
        f.replace(inst, span);
        // An extra store of the span is inlined.
        if let Some(statement) = containing_statement(f, span) {
            let block = f.node(statement).parent.expect("a statement in a block");
            let index = f.node(statement).child_index as usize;
            let next = f.inst(block).children().get(index + 1).copied();
            if let Some(next) = next
                && let Some((_, value)) = f.match_stloc(next)
                && matches!(f.inst(value), Inst::LdLoc { .. })
            {
                super::il_inlining::inline_one_if_possible(
                    f,
                    block,
                    index,
                    super::il_inlining::options::AGGRESSIVE,
                    ctx,
                );
            }
        }
        return;
    }
    if let Some(replacement) =
        super::transform_array_initializers::transform_span_t_array_initialization(f, ctx, inst)
    {
        f.replace(inst, replacement);
        return;
    }
    if let Some(delegate) = transform_delegate_ctor_ld_virt_ftn_to_ld_virt_delegate(f, ctx, inst) {
        f.replace(inst, delegate);
        return;
    }
    visit_children(f, ctx, inst);
}

/// `Block.GetContainingStatement`: the ancestor (or `inst`) in a control-flow block.
fn containing_statement(f: &Function, inst: NodeId) -> Option<NodeId> {
    std::iter::once(inst).chain(f.ancestors(inst)).find(|&n| {
        f.node(n).parent.is_some_and(|p| {
            matches!(
                f.inst(p),
                Inst::Block {
                    kind: BlockKind::ControlFlow,
                    ..
                }
            )
        })
    })
}

/// `MatchesElementCount`: `size_in_bytes` allocates `count` elements of `element`.
fn matches_element_count(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    size_in_bytes: NodeId,
    element: TypeId,
    count: NodeId,
) -> bool {
    use super::transform_assignment::{PointerOffset, pointer_offset_detect};
    match pointer_offset_detect(f, ctx, size_in_bytes, element, true, true) {
        Some(PointerOffset::Inst(n)) => {
            f.structurally_equal(unwrap_conv(f, n, ConversionKind::ZeroExtend), count)
        }
        Some(PointerOffset::Constant(c)) => f.match_ldc_i4(count) == Some(c),
        None => false,
    }
}

/// `TransformSpanTCtorContainingStackAlloc`: `newobj Span<T>(localloc(size), count)` becomes
/// `localloc.span T(count)`; with a stackalloc initializer block, the block gets the span
/// constructor as its final instruction.
fn transform_span_t_ctor_containing_stack_alloc(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    new_obj: NodeId,
) -> Option<NodeId> {
    let Inst::NewObj(c) = f.inst(new_obj) else {
        return None;
    };
    let call = c.clone();
    let ty = call.method.method.declaring?;
    if !matches!(
        known(ctx, ty),
        Some(KnownTypeCode::SpanOfT | KnownTypeCode::ReadOnlySpanOfT)
    ) {
        return None;
    }
    let type_arguments = ctx.ts.type_arguments(ty);
    let (&[first, count], &[element]) = (&call.args[..], &type_arguments[..]) else {
        return None;
    };
    if let Inst::LocAlloc { arg: size } = *f.inst(first)
        && matches_element_count(f, ctx, size, element, count)
    {
        return Some(f.add(Inst::LocAllocSpan { arg: count, ty }));
    }
    if !matches!(
        f.inst(first),
        Inst::Block {
            kind: BlockKind::StackAllocInitializer,
            ..
        }
    ) {
        return None;
    }
    let initializer = first;
    let (v, value) = f.match_stloc(*f.list(initializer).first()?)?;
    let pointer = f.var(v).ty;
    let memory = match *ctx.ts.data(pointer) {
        TypeData::Pointer(e) | TypeData::ByRef(e) => e,
        _ => return None,
    };
    if !ctx.ts.is_compatible_type_for_memory_access(memory, element) {
        return None;
    }
    let Inst::LocAlloc { arg: size } = *f.inst(value) else {
        return None;
    };
    if !matches_element_count(f, ctx, size, element, count) {
        return None;
    }
    // The block addresses the allocation through the pointer, and only its result is the
    // span: the constructor becomes its final instruction.
    let load = f.add(Inst::LdLoc { var: v });
    let ctor = f.add(Inst::NewObj(crate::il::ast::CallData {
        method: call.method.clone(),
        args: vec![load, count],
        is_tail: false,
        constrained_to: None,
        il_stack_was_empty: false,
    }));
    f.add_il_range(ctor, f.il_range(new_obj));
    let final_index = f.list(initializer).len();
    f.set_child_at(initializer, final_index, ctor);
    Some(initializer)
}

/// `TransformDelegateCtorLdVirtFtnToLdVirtDelegate`: `newobj D(target, ldvirtftn M(target))`
/// with a pure target becomes `ldvirtdelegate D M(target)`.
fn transform_delegate_ctor_ld_virt_ftn_to_ld_virt_delegate(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::NewObj(c) = f.inst(inst) else {
        return None;
    };
    let ty = c.method.method.declaring?;
    let &[target, ftn] = &c.args[..] else {
        return None;
    };
    let is_delegate = ctx
        .ts
        .definition(ty)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == crate::typesystem::compilation::TypeKind::Delegate);
    if !is_delegate {
        return None;
    }
    let Inst::LdVirtFtn {
        arg: ftn_target,
        ref method,
    } = *f.inst(ftn)
    else {
        return None;
    };
    let method = method.clone();
    if !crate::il::semantic_helper::is_pure(f.flags(target))
        || !f.structurally_equal(target, ftn_target)
    {
        return None;
    }
    let delegate = f.add(Inst::LdVirtDelegate {
        arg: target,
        ty,
        method,
    });
    for range_of in [inst, ftn, ftn_target] {
        f.add_il_range(delegate, f.il_range(range_of));
    }
    Some(delegate)
}

/// `CSharpBinderFlags` the dynamic rewrites test.
const INVOKE_SPECIAL_NAME: i32 = 0x4;
const VALUE_FROM_COMPOUND_ASSIGNMENT: i32 = 0x80;
const RESULT_DISCARDED: i32 = 0x100;

/// `ExpressionType` values of the compound operations.
const ADD_ASSIGN: i32 = 63;
const SUBTRACT_ASSIGN: i32 = 73;

/// `DynamicCompoundAssign.IsExpressionTypeSupported`: the `...Assign` operations, without
/// `PowerAssign`.
pub(crate) fn is_dynamic_compound_operation(operation: i32) -> bool {
    matches!(operation, 63..=70 | 72..=80)
}

/// `CompoundEvalModeFromOperation`: post-increments and post-decrements evaluate to the old
/// value.
pub(crate) fn dynamic_compound_eval_mode(operation: i32) -> crate::il::ast::CompoundEvalMode {
    use crate::il::ast::CompoundEvalMode;
    // PostIncrementAssign, PostDecrementAssign.
    if matches!(operation, 79 | 80) {
        CompoundEvalMode::EvaluatesToOldValue
    } else {
        CompoundEvalMode::EvaluatesToNewValue
    }
}

/// The binder flags, name, operation, argument infos and arguments of a dynamic instruction.
type DynamicParts = (
    i32,
    Option<String>,
    i32,
    Vec<crate::il::ast::DynamicArgInfo>,
    Vec<NodeId>,
);

/// The parts of a dynamic instruction of `kind`.
fn dynamic_parts(
    f: &Function,
    inst: NodeId,
    kind: crate::il::ast::DynamicKind,
) -> Option<DynamicParts> {
    match f.inst(inst) {
        Inst::Dynamic {
            kind: k,
            binder_flags,
            name,
            operation,
            arg_infos,
            args,
            ..
        } if *k == kind => Some((
            *binder_flags,
            name.clone(),
            *operation,
            arg_infos.clone(),
            args.clone(),
        )),
        _ => None,
    }
}

/// The `dynamic.compound.op(left, right)` a compound binary operator stands for.
fn new_dynamic_compound_assign(
    f: &mut Function,
    operation: i32,
    binder_flags: i32,
    infos: &[crate::il::ast::DynamicArgInfo],
    left: NodeId,
    right: NodeId,
) -> NodeId {
    f.add(Inst::DynamicCompoundAssign {
        operation,
        binder_flags,
        target_info: infos[0].clone(),
        value_info: infos[1].clone(),
        eval_mode: dynamic_compound_eval_mode(operation),
        target_kind: crate::il::ast::CompoundTargetKind::Dynamic,
        target: left,
        value: right,
    })
}

/// `TransformDynamicSetMemberInstruction`: `dynamic.setmember.compound N(target,
/// dynamic.binary.operator op(dynamic.getmember N(target), value))` becomes
/// `dynamic.compound.op(dynamic.getmember N(target), value)`.
pub(crate) fn transform_dynamic_set_member(f: &mut Function, inst: NodeId) {
    use crate::il::ast::DynamicKind;
    let Some((flags, name, _, _, args)) = dynamic_parts(f, inst, DynamicKind::SetMember) else {
        return;
    };
    if flags & VALUE_FROM_COMPOUND_ASSIGNMENT == 0 {
        return;
    }
    let &[target, value] = &args[..] else {
        return;
    };
    let Some((op_flags, _, operation, infos, op_args)) =
        dynamic_parts(f, value, DynamicKind::BinaryOperator)
    else {
        return;
    };
    let &[left, right] = &op_args[..] else {
        return;
    };
    let Some((_, member_name, _, _, member_args)) = dynamic_parts(f, left, DynamicKind::GetMember)
    else {
        return;
    };
    let &[member_target] = &member_args[..] else {
        return;
    };
    if !f.structurally_equal(member_target, target)
        || !crate::il::semantic_helper::is_pure(f.flags(member_target))
    {
        return;
    }
    if name != member_name || !is_dynamic_compound_operation(operation) {
        return;
    }
    let compound = new_dynamic_compound_assign(f, operation, op_flags, &infos, left, right);
    f.replace(inst, compound);
}

/// `VisitDynamicSetIndexInstruction`: `dynamic.setindex.compound(target, index,
/// dynamic.binary.operator op(dynamic.getindex(target, index), value))` becomes
/// `dynamic.compound.op(dynamic.getindex(target, index), value)`.
fn transform_dynamic_set_index(f: &mut Function, inst: NodeId) {
    use crate::il::ast::DynamicKind;
    let Some((flags, _, _, _, args)) = dynamic_parts(f, inst, DynamicKind::SetIndex) else {
        return;
    };
    if flags & VALUE_FROM_COMPOUND_ASSIGNMENT == 0 {
        return;
    }
    let Some(&value) = args.last() else {
        return;
    };
    let Some((op_flags, _, operation, infos, op_args)) =
        dynamic_parts(f, value, DynamicKind::BinaryOperator)
    else {
        return;
    };
    let &[left, right] = &op_args[..] else {
        return;
    };
    let Some((_, _, _, _, index_args)) = dynamic_parts(f, left, DynamicKind::GetIndex) else {
        return;
    };
    if args.len() != index_args.len() + 1 {
        return;
    }
    // The same arguments go to the getter and the setter.
    for (j, &index_arg) in index_args.iter().enumerate() {
        if !crate::il::semantic_helper::is_pure(f.flags(index_arg))
            || !f.structurally_equal(index_arg, args[j])
        {
            return;
        }
    }
    if !is_dynamic_compound_operation(operation) {
        return;
    }
    let compound = new_dynamic_compound_assign(f, operation, op_flags, &infos, left, right);
    f.replace(inst, compound);
}

/// `Block.Unwrap`: the single instruction of a block without a final value.
fn unwrap_block(f: &Function, inst: NodeId) -> NodeId {
    if let Inst::Block { final_inst, .. } = *f.inst(inst)
        && f.list(inst).len() == 1
        && f.match_nop(final_inst)
    {
        return f.list(inst)[0];
    }
    inst
}

/// `TransformDynamicAddAssignOrRemoveAssign`: `if (dynamic.isevent(target)) add_E(target,
/// value) else dynamic.compound.addassign(dynamic.getmember E(target), value)` becomes the
/// compound assignment.
pub(crate) fn transform_dynamic_add_assign_or_remove_assign(
    f: &mut Function,
    inst: NodeId,
) -> bool {
    use crate::il::ast::DynamicKind;
    let Inst::If {
        mut condition,
        mut true_inst,
        mut false_inst,
        ..
    } = *f.inst(inst)
    else {
        return false;
    };
    // `MatchIfInstructionPositiveCondition`.
    while let Some(arg) = f.match_logic_not(condition) {
        condition = arg;
        std::mem::swap(&mut true_inst, &mut false_inst);
    }
    let Some((_, _, _, _, event_args)) = dynamic_parts(f, condition, DynamicKind::IsEvent) else {
        return false;
    };
    let &[event_target] = &event_args[..] else {
        return false;
    };
    let true_inst = unwrap_block(f, true_inst);
    let false_inst = unwrap_block(f, false_inst);
    let Inst::DynamicCompoundAssign {
        operation,
        target: get_member,
        value: compound_value,
        ..
    } = *f.inst(false_inst)
    else {
        return false;
    };
    let Some((_, member_name, _, _, member_args)) =
        dynamic_parts(f, get_member, DynamicKind::GetMember)
    else {
        return false;
    };
    let &[member_target] = &member_args[..] else {
        return false;
    };
    if !crate::il::semantic_helper::is_pure(f.flags(event_target))
        || !f.structurally_equal(event_target, member_target)
    {
        return false;
    }
    let Some((invoke_flags, invoke_name, _, _, invoke_args)) =
        dynamic_parts(f, true_inst, DynamicKind::InvokeMember)
    else {
        return false;
    };
    if invoke_flags & INVOKE_SPECIAL_NAME == 0 || invoke_flags & RESULT_DISCARDED == 0 {
        return false;
    }
    let member_name = member_name.unwrap_or_default();
    let expected = match operation {
        ADD_ASSIGN => format!("add_{member_name}"),
        SUBTRACT_ASSIGN => format!("remove_{member_name}"),
        _ => return false,
    };
    if invoke_name.as_deref() != Some(expected.as_str()) {
        return false;
    }
    let (Some(&first), Some(&second)) = (invoke_args.first(), invoke_args.get(1)) else {
        return false;
    };
    if !f.structurally_equal(compound_value, second) || !f.structurally_equal(first, member_target)
    {
        return false;
    }
    f.replace(inst, false_inst);
    true
}
