//! `IL/Transforms/NullableLiftingTransform.cs`: the `HasValue` / `GetValueOrDefault`
//! patterns of operators on nullable values become lifted operators, `??` and `bool?` logic.

use crate::il::ast::{
    BinaryNumericOperator, CallData, ComparisonKind, ComparisonLiftingKind, Function, Inst,
    MethodOperand, NodeId, NullCoalescingKind, TypeOperand, VarId, nullable_type,
};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::expression_transforms::{logic_not, match_logic_and, match_logic_or};
use super::il_inlining::known;
use super::statement_transform::StatementTransformContext;
use super::transform_assignment::nullable_underlying;

/// `NullableLiftingStatementTransform.Run`.
pub fn run_statement(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    NullableLifting::default().run_statements(&mut r.function, ctx, block, pos);
}

/// `Run(Comp)`: `comp(a.GetValueOrDefault() == const)` with a non-zero constant becomes
/// `comp.lifted(a == const)`.
pub fn run_comp(f: &mut Function, ctx: &mut Context<'_, '_, '_>, comp: NodeId) {
    let Inst::Comp {
        kind,
        lifting: ComparisonLiftingKind::None,
        left,
        right,
        ..
    } = *f.inst(comp)
    else {
        return;
    };
    if !kind.is_equality_or_inequality() {
        return;
    }
    for (operand, other, index) in [(left, right, 0), (right, left, 1)] {
        if let Some(arg) = match_get_value_or_default(f, ctx, operand)
            && f.match_ldc_i(other).is_some_and(|v| v != 0)
        {
            let declaring = call_method(f, operand).and_then(|m| m.declaring);
            if let Inst::Comp { lifting, .. } = f.inst_mut(comp) {
                *lifting = ComparisonLiftingKind::CSharp;
            }
            let ty = declaring.unwrap_or_else(|| ctx.ts.intern(TypeData::UnknownSpecial));
            let ld_obj = new_ld_obj(f, arg, ty);
            f.set_child_at(comp, index, ld_obj);
            return;
        }
    }
}

/// `Run(IfInstruction)`.
pub fn run_if(f: &mut Function, ctx: &mut Context<'_, '_, '_>, if_inst: NodeId) -> bool {
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return false;
    };
    let lifted = NullableLifting::default().lift(f, ctx, if_inst, condition, true_inst, false_inst);
    if let Some(lifted) = lifted {
        f.replace(if_inst, lifted);
        return true;
    }
    false
}

/// `Run(BinaryNumericInstruction)`: a `bit.and` of two booleans.
pub fn run_bit_and(f: &mut Function, ctx: &mut Context<'_, '_, '_>, bni: NodeId) -> bool {
    let Inst::BinaryNumeric { left, right, .. } = *f.inst(bni) else {
        return false;
    };
    let zero = f.add(Inst::LdcI4 { value: 0 });
    let lifted = NullableLifting::default().lift(f, ctx, bni, left, right, zero);
    if let Some(lifted) = lifted {
        f.replace(bni, lifted);
        return true;
    }
    false
}

fn call_method(f: &Function, inst: NodeId) -> Option<ResolvedMethod> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => Some(c.method.method.clone()),
        _ => None,
    }
}

fn new_ld_obj(f: &mut Function, target: NodeId, ty: TypeId) -> NodeId {
    f.add(Inst::LdObj {
        target,
        ty: TypeOperand { token: 0, ty },
        is_volatile: false,
        unaligned: 0,
    })
}

fn new_comp(
    f: &mut Function,
    kind: ComparisonKind,
    lifting: ComparisonLiftingKind,
    input: StackType,
    sign: Sign,
    left: NodeId,
    right: NodeId,
) -> NodeId {
    f.add(Inst::Comp {
        kind,
        lifting,
        sign,
        input,
        left,
        right,
    })
}

/// `MatchHasValueCall(inst, out arg)`: `call Nullable<T>.get_HasValue(arg)`.
fn match_has_value_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    crate::il::control_flow::switch_detection::match_has_value_call(f, ctx, inst)
}

/// `MatchHasValueCall(inst, out v)`: `call get_HasValue(ldloca v)`.
fn match_has_value_call_var(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<VarId> {
    let arg = match_has_value_call(f, ctx, inst)?;
    match *f.inst(arg) {
        Inst::LdLoca { var } => Some(var),
        _ => None,
    }
}

/// `MatchNegatedHasValueCall(inst, v)`.
fn match_negated_has_value_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    v: VarId,
) -> bool {
    f.match_logic_not(inst)
        .is_some_and(|arg| match_has_value_call_var(f, ctx, arg) == Some(v))
}

/// `MatchGetValueOrDefault(inst, out arg)`: `call Nullable<T>.GetValueOrDefault(arg)`.
pub(crate) fn match_get_value_or_default(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<NodeId> {
    let Inst::Call(c) = f.inst(inst) else {
        return None;
    };
    let &[arg] = &c.args[..] else {
        return None;
    };
    if c.method.method.name != "GetValueOrDefault" {
        return None;
    }
    let declaring = c.method.method.declaring?;
    (known(ctx, declaring) == Some(KnownTypeCode::NullableOfT)).then_some(arg)
}

/// `MatchGetValueOrDefault(inst, out v)`: `call GetValueOrDefault(ldloca v)`.
fn match_get_value_or_default_var(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<VarId> {
    let arg = match_get_value_or_default(f, ctx, inst)?;
    match *f.inst(arg) {
        Inst::LdLoca { var } => Some(var),
        _ => None,
    }
}

/// `MatchNullableCtor`: `newobj Nullable<T>..ctor(arg)`; returns `T` and `arg`.
pub(crate) fn match_nullable_ctor(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId)> {
    let Inst::NewObj(c) = f.inst(inst) else {
        return None;
    };
    let &[arg] = &c.args[..] else {
        return None;
    };
    if c.method.method.name != ".ctor" {
        return None;
    }
    let declaring = c.method.method.declaring?;
    if known(ctx, declaring) != Some(KnownTypeCode::NullableOfT) {
        return None;
    }
    Some((nullable_underlying(ctx, declaring), arg))
}

/// `NullableType.IsNullable`.
fn is_nullable(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    known(ctx, t) == Some(KnownTypeCode::NullableOfT)
}

/// `NullableType.IsNonNullableValueType`.
pub(crate) fn is_non_nullable_value_type(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    ctx.ts.is_reference_type(t) == Some(false) && !is_nullable(ctx, t)
}

/// `MatchNull(inst, out underlyingType)`: `default.value T?`.
fn match_null(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> Option<TypeId> {
    let Inst::DefaultValue { ty, .. } = *f.inst(inst) else {
        return None;
    };
    is_nullable(ctx, ty.ty).then(|| nullable_underlying(ctx, ty.ty))
}

/// `CSharpOperators.IsComparisonOperator`.
fn is_comparison_operator(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    ctx.ts.method_is_operator(m)
        && m.params().is_ok_and(|p| p.len() == 2)
        && matches!(
            m.name.as_str(),
            "op_Equality"
                | "op_Inequality"
                | "op_LessThan"
                | "op_GreaterThan"
                | "op_LessThanOrEqual"
                | "op_GreaterThanOrEqual"
        )
}

/// `CSharpOperators.LiftUserDefinedOperator`: the lifted form of a user-defined operator,
/// with nullable parameters and (except for comparisons) a nullable return type.
pub(crate) fn lift_user_defined_operator(
    ctx: &mut Context<'_, '_, '_>,
    operand: &MethodOperand,
) -> Option<MethodOperand> {
    let m = &operand.method;
    let comparison = is_comparison_operator(ctx, m);
    if comparison {
        if known(ctx, m.ret) != Some(KnownTypeCode::Boolean) {
            return None;
        }
    } else if !is_non_nullable_value_type(ctx, m.ret) {
        return None;
    }
    let params = m.params().ok()?;
    for &p in &params {
        if !is_non_nullable_value_type(ctx, p) {
            return None;
        }
    }
    let mut lifted = m.clone();
    lifted.base_params = m
        .base_params
        .iter()
        .map(|&p| nullable_type(ctx.ts, p))
        .collect();
    if !comparison {
        lifted.ret = nullable_type(ctx.ts, m.ret);
    }
    Some(MethodOperand {
        method: lifted,
        lifted: true,
        ..operand.clone()
    })
}

/// A new call with the shape of `call` and other arguments.
fn new_call_like(
    f: &mut Function,
    call: &CallData,
    method: MethodOperand,
    args: Vec<NodeId>,
) -> NodeId {
    f.add(Inst::Call(CallData {
        method,
        args,
        is_tail: call.is_tail,
        constrained_to: call.constrained_to,
        il_stack_was_empty: call.il_stack_was_empty,
    }))
}

/// `CompOrDecimal`: a comparison, or a comparison operator of `decimal`.
struct CompOrDecimal {
    instruction: NodeId,
    kind: ComparisonKind,
    left: NodeId,
    right: NodeId,
    is_lifted: bool,
}

/// `MatchCompOrDecimal`.
fn match_comp_or_decimal(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<CompOrDecimal> {
    match f.inst(inst) {
        &Inst::Comp {
            kind,
            lifting,
            left,
            right,
            ..
        } => Some(CompOrDecimal {
            instruction: inst,
            kind,
            left,
            right,
            is_lifted: lifting != ComparisonLiftingKind::None,
        }),
        Inst::Call(c) if c.args.len() == 2 && !c.method.lifted => {
            let m = c.method.method.clone();
            let (left, right) = (c.args[0], c.args[1]);
            if !ctx.ts.method_is_operator(&m) {
                return None;
            }
            let kind = match m.name.as_str() {
                "op_Equality" => ComparisonKind::Equality,
                "op_Inequality" => ComparisonKind::Inequality,
                "op_LessThan" => ComparisonKind::LessThan,
                "op_LessThanOrEqual" => ComparisonKind::LessThanOrEqual,
                "op_GreaterThan" => ComparisonKind::GreaterThan,
                "op_GreaterThanOrEqual" => ComparisonKind::GreaterThanOrEqual,
                _ => return None,
            };
            let is_decimal = m
                .declaring
                .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::Decimal));
            is_decimal.then_some(CompOrDecimal {
                instruction: inst,
                kind,
                left,
                right,
                is_lifted: false,
            })
        }
        _ => None,
    }
}

impl CompOrDecimal {
    /// `LeftExpectedType` / `RightExpectedType`: the parameter type of the operator, unknown
    /// for a `comp`.
    fn expected_type(&self, f: &Function, index: usize) -> Option<TypeId> {
        match f.inst(self.instruction) {
            Inst::Call(c) => c.method.method.params().ok()?.get(index).copied(),
            _ => None,
        }
    }

    /// `MakeLifted`.
    fn make_lifted(
        &self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        new_kind: ComparisonKind,
        left: NodeId,
        right: NodeId,
    ) -> Option<NodeId> {
        match f.inst(self.instruction).clone() {
            Inst::Comp { input, sign, .. } => {
                let comp = new_comp(
                    f,
                    new_kind,
                    ComparisonLiftingKind::CSharp,
                    input,
                    sign,
                    left,
                    right,
                );
                f.add_il_range(comp, f.il_range(self.instruction));
                Some(comp)
            }
            Inst::Call(call) => {
                let method = if new_kind == self.kind {
                    call.method.clone()
                } else if new_kind == ComparisonKind::Inequality
                    && call.method.method.name == "op_Equality"
                {
                    let m = &call.method.method;
                    let params = m.params().ok()?;
                    let declaring = m.declaring?;
                    let candidates = ctx
                        .ts
                        .get_methods(declaring, None, MemberOptions::default())
                        .unwrap_or_default();
                    let inequality = candidates.into_iter().find(|c| {
                        c.name == "op_Inequality" && c.params().is_ok_and(|p| p == params)
                    })?;
                    MethodOperand {
                        range_accessor: None, // Found by lookup: the wire sends the method's signature.
                        specialized_by: None,
                        token: 0,
                        method: inequality,
                        context: call.method.context,
                        lifted: false,
                    }
                } else {
                    return None;
                };
                let lifted = lift_user_defined_operator(ctx, &method)
                    .expect("decimal comparison operators lift");
                let new_call = new_call_like(f, &call, lifted, vec![left, right]);
                f.add_il_range(new_call, f.il_range(self.instruction));
                Some(new_call)
            }
            _ => None,
        }
    }
}

/// `IsPure` of lifting: a nullable constructor counts as its argument.
fn is_pure_lifted(f: &Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    let inst = match match_nullable_ctor(f, ctx, inst) {
        Some((_, arg)) => arg,
        None => inst,
    };
    is_pure(f.flags(inst))
}

/// `BitSet` over the nullable variables.
type Bits = Vec<bool>;

fn all_bits(bits: &Bits, count: usize) -> bool {
    (0..count).all(|i| bits.get(i).copied().unwrap_or(false))
}

#[derive(Default)]
struct NullableLifting {
    /// `nullableVars`: `None` until a condition is analyzed.
    nullable_vars: Option<Vec<VarId>>,
}

impl NullableLifting {
    fn vars(&self) -> &[VarId] {
        self.nullable_vars.as_deref().unwrap_or_default()
    }

    /// `RunStatements`: `if (!c) leave(default); leave(newobj Nullable(expr))` returns the
    /// lifted expression.
    fn run_statements(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
        pos: usize,
    ) -> bool {
        let list = f.list(block);
        if pos + 2 != list.len() {
            return false;
        }
        let (if_inst, else_leave) = (list[pos], list[pos + 1]);
        let Some((condition, true_inst)) = f.match_if_instruction(if_inst) else {
            return false;
        };
        let then_leave = unwrap_block(f, true_inst);
        let Inst::Leave {
            target: Some(then_target),
            value: then_value,
        } = *f.inst(then_leave)
        else {
            return false;
        };
        let Inst::Leave {
            target: Some(else_target),
            value: else_value,
        } = *f.inst(else_leave)
        else {
            return false;
        };
        if then_target != else_target {
            return false;
        }
        let Some(lifted) = self.lift(f, ctx, if_inst, condition, then_value, else_value) else {
            return false;
        };
        f.set_child_at(then_leave, 0, lifted);
        f.replace(if_inst, then_leave);
        f.remove_at(block, pos + 1);
        true
    }

    /// `AnalyzeCondition`: `v1.HasValue & ... & vn.HasValue`, collecting the variables.
    fn analyze_condition(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        condition: NodeId,
    ) -> bool {
        if let Some(v) = match_has_value_call_var(f, ctx, condition) {
            self.nullable_vars.get_or_insert_with(Vec::new).push(v);
            return true;
        }
        if let Inst::BinaryNumeric {
            op: BinaryNumericOperator::BitAnd,
            left,
            right,
            ..
        } = *f.inst(condition)
        {
            if f.result_type(ctx.ts, condition) != StackType::I4 {
                return false;
            }
            return self.analyze_condition(f, ctx, left) && self.analyze_condition(f, ctx, right);
        }
        false
    }

    /// `AnalyzeNegatedCondition`.
    fn analyze_negated_condition(
        &mut self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        condition: NodeId,
    ) -> bool {
        f.match_logic_not(condition)
            .is_some_and(|arg| self.analyze_condition(f, ctx, arg))
    }

    /// `Lift`: the main lifting logic of `ifInst` (an if, or a `bit.and`).
    fn lift(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        if_inst: NodeId,
        mut condition: NodeId,
        mut true_inst: NodeId,
        mut false_inst: NodeId,
    ) -> Option<NodeId> {
        while let Some(arg) = f.match_logic_not(condition) {
            condition = arg;
            std::mem::swap(&mut true_inst, &mut false_inst);
        }
        if ctx.settings.null_propagation
            && !super::null_propagation_transform::is_protected_if_inst(f, if_inst)
            && let Some(r) =
                super::null_propagation_transform::run(f, ctx, condition, true_inst, false_inst)
        {
            f.add_il_range(r, f.il_range(if_inst));
            return Some(r);
        }
        if !ctx.settings.lift_nullables {
            return None;
        }
        let with_range = |f: &mut Function, result: Option<NodeId>| {
            if let Some(r) = result {
                f.add_il_range(r, f.il_range(if_inst));
            }
            result
        };
        if self.analyze_condition(f, ctx, condition) {
            // (v1 != null && ... && vn != null) ? trueInst : falseInst
            let result = self.lift_normal(f, ctx, true_inst, false_inst);
            return with_range(f, result);
        }
        if let Some(comp) = match_comp_or_decimal(f, ctx, condition) {
            // A C#-style lifted comparison checks the values before the HasValue bits.
            if comp.kind.is_equality_or_inequality() {
                if comp.kind == ComparisonKind::Inequality {
                    std::mem::swap(&mut true_inst, &mut false_inst);
                }
                if f.match_ldc_i4(false_inst) == Some(0) {
                    // (a.GetValueOrDefault() == b.GetValueOrDefault()) ? (a.HasValue == b.HasValue) : false
                    return self
                        .lift_csharp_equality_comparison(
                            f,
                            ctx,
                            &comp,
                            ComparisonKind::Equality,
                            true_inst,
                        )
                        .or_else(|| {
                            self.lift_csharp_user_equality_comparison(
                                f,
                                ctx,
                                &comp,
                                ComparisonKind::Equality,
                                true_inst,
                            )
                        });
                } else if f.match_ldc_i4(false_inst) == Some(1) {
                    // (a.GetValueOrDefault() == b.GetValueOrDefault()) ? (a.HasValue != b.HasValue) : true
                    return self
                        .lift_csharp_equality_comparison(
                            f,
                            ctx,
                            &comp,
                            ComparisonKind::Inequality,
                            true_inst,
                        )
                        .or_else(|| {
                            self.lift_csharp_user_equality_comparison(
                                f,
                                ctx,
                                &comp,
                                ComparisonKind::Inequality,
                                true_inst,
                            )
                        });
                } else if !comp.is_lifted
                    && is_generic_new_pattern(f, ctx, comp.left, comp.right, true_inst, false_inst)
                {
                    // (default(T) == null) ? Activator.CreateInstance<T>() : default(T)
                    return Some(true_inst);
                }
            } else if !comp.is_lifted {
                // One of < <= > >=: false unless all HasValue bits are true.
                if f.match_ldc_i4(false_inst) == Some(0)
                    && self.analyze_condition(f, ctx, true_inst)
                {
                    return self.lift_csharp_comparison(f, ctx, &comp, comp.kind);
                }
                if f.match_ldc_i4(true_inst) == Some(0)
                    && self.analyze_condition(f, ctx, false_inst)
                {
                    return self.lift_csharp_comparison(f, ctx, &comp, comp.kind.negate());
                }
                if f.match_ldc_i4(false_inst) == Some(1)
                    && self.analyze_negated_condition(f, ctx, true_inst)
                {
                    let result = self.lift_csharp_comparison(f, ctx, &comp, comp.kind)?;
                    return Some(logic_not(f, ctx, result));
                }
                if f.match_ldc_i4(true_inst) == Some(1)
                    && self.analyze_negated_condition(f, ctx, false_inst)
                {
                    let result = self.lift_csharp_comparison(f, ctx, &comp, comp.kind.negate())?;
                    return Some(logic_not(f, ctx, result));
                }
            }
        }
        // Equality comparisons with bool?:
        if let Some(v) = match_get_value_or_default_var(f, ctx, condition) {
            let v_type = f.var(v).ty;
            let underlying = nullable_underlying(ctx, v_type);
            let is_boolean = known(ctx, underlying) == Some(KnownTypeCode::Boolean);
            let lifted_comp =
                |f: &mut Function, kind: ComparisonKind, load_range: NodeId, constant: NodeId| {
                    let load = f.add(Inst::LdLoc { var: v });
                    f.add_il_range(load, f.il_range(load_range));
                    let comp = new_comp(
                        f,
                        kind,
                        ComparisonLiftingKind::CSharp,
                        StackType::I4,
                        Sign::None,
                        load,
                        constant,
                    );
                    f.add_il_range(comp, f.il_range(if_inst));
                    comp
                };
            if is_boolean
                && match_has_value_call_var(f, ctx, true_inst) == Some(v)
                && f.match_ldc_i4(false_inst) == Some(0)
            {
                // v.GetValueOrDefault() ? v.HasValue : false => v == true
                let one = f.add(Inst::LdcI4 { value: 1 });
                f.add_il_range(one, f.il_range(false_inst));
                return Some(lifted_comp(f, ComparisonKind::Equality, true_inst, one));
            } else if f.match_ldc_i4(true_inst) == Some(0)
                && match_has_value_call_var(f, ctx, false_inst) == Some(v)
            {
                // v.GetValueOrDefault() ? false : v.HasValue => v == false
                return Some(lifted_comp(
                    f,
                    ComparisonKind::Equality,
                    false_inst,
                    true_inst,
                ));
            } else if is_boolean
                && match_negated_has_value_call(f, ctx, true_inst, v)
                && f.match_ldc_i4(false_inst) == Some(1)
            {
                // v.GetValueOrDefault() ? !v.HasValue : true => v != true
                return Some(lifted_comp(
                    f,
                    ComparisonKind::Inequality,
                    true_inst,
                    false_inst,
                ));
            } else if f.match_ldc_i4(true_inst) == Some(1)
                && match_negated_has_value_call(f, ctx, false_inst, v)
            {
                // v.GetValueOrDefault() ? true : !v.HasValue => v != false
                let zero = f.add(Inst::LdcI4 { value: 0 });
                f.add_il_range(zero, f.il_range(true_inst));
                return Some(lifted_comp(f, ComparisonKind::Inequality, false_inst, zero));
            }
        }
        // & and | on bool?:
        if let Some(v) = f.match_ldloc(true_inst) {
            if let Some((utype, arg)) = match_nullable_ctor(f, ctx, false_inst)
                && known(ctx, utype) == Some(KnownTypeCode::Boolean)
                && f.match_ldc_i4(arg) == Some(0)
            {
                // condition ? v : (bool?)false => condition & v
                let result = f.add(Inst::ThreeValuedBoolAnd {
                    left: condition,
                    right: true_inst,
                });
                return with_range(f, Some(result));
            }
            if let Some(v2) = f.match_ldloc(false_inst)
                && let Some((nullable1, nullable2)) =
                    match_three_valued_logic_condition_pattern(f, ctx, condition)
            {
                if v == nullable1 && v2 == nullable2 {
                    let result = f.add(Inst::ThreeValuedBoolOr {
                        left: true_inst,
                        right: false_inst,
                    });
                    return with_range(f, Some(result));
                } else if v == nullable2 && v2 == nullable1 {
                    let result = f.add(Inst::ThreeValuedBoolAnd {
                        left: false_inst,
                        right: true_inst,
                    });
                    return with_range(f, Some(result));
                }
            }
        } else if f.match_ldloc(false_inst).is_some()
            && let Some((utype, arg)) = match_nullable_ctor(f, ctx, true_inst)
            && known(ctx, utype) == Some(KnownTypeCode::Boolean)
            && f.match_ldc_i4(arg) == Some(1)
        {
            // condition ? (bool?)true : v => condition | v
            let result = f.add(Inst::ThreeValuedBoolOr {
                left: condition,
                right: false_inst,
            });
            return with_range(f, Some(result));
        }
        None
    }

    /// `LiftCSharpEqualityComparison`.
    fn lift_csharp_equality_comparison(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        value_comp: &CompOrDecimal,
        new_kind: ComparisonKind,
        mut has_value_test: NodeId,
    ) -> Option<NodeId> {
        let mut negated = false;
        while let Some(arg) = f.match_logic_not(has_value_test) {
            has_value_test = arg;
            negated = !negated;
        }
        if let Inst::Comp {
            kind,
            lifting,
            left,
            right,
            ..
        } = *f.inst(has_value_test)
        {
            if value_comp.is_lifted || lifting != ComparisonLiftingKind::None {
                return None;
            }
            // The HasValue comparison must be the same operator as the value comparison.
            if (if negated { kind.negate() } else { kind }) != new_kind {
                return None;
            }
            let left_var = match_has_value_call_var(f, ctx, left)?;
            let right_var = match_has_value_call_var(f, ctx, right)?;
            self.nullable_vars = Some(vec![left_var]);
            let left_lifted = self.do_lift(f, ctx, value_comp.left);
            self.nullable_vars = Some(vec![right_var]);
            let right_lifted = self.do_lift(f, ctx, value_comp.right);
            if let (Some((l, lb)), Some((r, rb))) = (left_lifted, right_lifted)
                && lb[0]
                && rb[0]
                && is_pure(f.flags(l))
                && is_pure(f.flags(r))
            {
                return value_comp.make_lifted(f, ctx, new_kind, l, r);
            }
        } else if new_kind == ComparisonKind::Equality
            && !negated
            && let Some(v) = match_has_value_call_var(f, ctx, has_value_test)
        {
            // A nullable compared with a non-nullable: the normal comparison code.
            self.nullable_vars = Some(vec![v]);
            return self.lift_csharp_comparison(f, ctx, value_comp, new_kind);
        } else if new_kind == ComparisonKind::Inequality
            && negated
            && let Some(v) = match_has_value_call_var(f, ctx, has_value_test)
        {
            self.nullable_vars = Some(vec![v]);
            return self.lift_csharp_comparison(f, ctx, value_comp, new_kind);
        }
        None
    }

    /// `LiftCSharpComparison`.
    fn lift_csharp_comparison(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        comp: &CompOrDecimal,
        new_kind: ComparisonKind,
    ) -> Option<NodeId> {
        if comp.is_lifted {
            // Legacy csc's `num.GetValueOrDefault() == const && num.HasValue`: the HasValue
            // check is redundant, and the lifting on comp already lifted the left side.
            if self.vars().len() != 1 {
                return None;
            }
            let v = self.vars()[0];
            if f.match_ldloc(comp.left) == Some(v) || f.match_ldloc(comp.right) == Some(v) {
                let left = f.clone_subtree(comp.left);
                let right = f.clone_subtree(comp.right);
                return comp.make_lifted(f, ctx, new_kind, left, right);
            }
            return None;
        }
        let left_type = comp.expected_type(f, 0);
        let right_type = comp.expected_type(f, 1);
        let (left, right, bits) =
            self.do_lift_binary(f, ctx, comp.left, comp.right, left_type, right_type)?;
        // Only instructions that are pure after lifting (calls to GetValueOrDefault are not
        // pure before).
        if is_pure_lifted(f, ctx, left) && is_pure_lifted(f, ctx, right) {
            if !all_bits(&bits, self.vars().len()) {
                // A nullable variable that does not contribute to the result.
                return None;
            }
            return comp.make_lifted(f, ctx, new_kind, left, right);
        }
        None
    }

    /// `LiftCSharpUserEqualityComparison`: user-defined `==` and `!=` on two nullables.
    fn lift_csharp_user_equality_comparison(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        has_value_comp: &CompOrDecimal,
        new_kind: ComparisonKind,
        nested_if: NodeId,
    ) -> Option<NodeId> {
        if has_value_comp.is_lifted {
            return None;
        }
        let nullable1 = match_has_value_call_var(f, ctx, has_value_comp.left)?;
        let nullable2 = match_has_value_call_var(f, ctx, has_value_comp.right)?;
        let Inst::If {
            condition,
            true_inst,
            false_inst,
            ..
        } = *f.inst(nested_if)
        else {
            return None;
        };
        let (mut condition, mut true_inst, mut false_inst) = (condition, true_inst, false_inst);
        while let Some(arg) = f.match_logic_not(condition) {
            condition = arg;
            std::mem::swap(&mut true_inst, &mut false_inst);
        }
        let nullable = match_has_value_call_var(f, ctx, condition)?;
        if nullable != nullable1 && nullable != nullable2 {
            return None;
        }
        let expected_false = if new_kind == ComparisonKind::Equality {
            1
        } else {
            0
        };
        if f.match_ldc_i4(false_inst) != Some(expected_false) {
            return None;
        }
        let mut negated = false;
        while let Some(arg) = f.match_logic_not(true_inst) {
            negated = !negated;
            true_inst = arg;
        }
        let Inst::Call(call) = f.inst(true_inst).clone() else {
            return None;
        };
        if !(ctx.ts.method_is_operator(&call.method.method) && call.args.len() == 2) {
            return None;
        }
        let expect_equality = (new_kind == ComparisonKind::Equality) ^ negated;
        let expected_name = if expect_equality {
            "op_Equality"
        } else {
            "op_Inequality"
        };
        if call.method.method.name != expected_name {
            return None;
        }
        let lifted_operator = lift_user_defined_operator(ctx, &call.method)?;
        self.nullable_vars = Some(vec![nullable1]);
        let left = self.do_lift(f, ctx, call.args[0]);
        self.nullable_vars = Some(vec![nullable2]);
        let right = self.do_lift(f, ctx, call.args[1]);
        if let (Some((l, lb)), Some((r, rb))) = (left, right)
            && lb[0]
            && rb[0]
            && is_pure(f.flags(l))
            && is_pure(f.flags(r))
        {
            let mut replacement = new_call_like(f, &call, lifted_operator, vec![l, r]);
            f.add_il_range(replacement, f.il_range(true_inst));
            if negated {
                replacement = logic_not(f, ctx, replacement);
            }
            return Some(replacement);
        }
        None
    }

    /// `LiftCSharpUserComparison`: `(v1 != null && ...) ? call op_X(lhs, rhs) : false`.
    fn lift_csharp_user_comparison(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        mut true_inst: NodeId,
        false_inst: NodeId,
    ) -> Option<NodeId> {
        let mut negated = false;
        while let Some(arg) = f.match_logic_not(true_inst) {
            negated = !negated;
            true_inst = arg;
        }
        let Inst::Call(call) = f.inst(true_inst).clone() else {
            return None;
        };
        if call.method.lifted || !is_comparison_operator(ctx, &call.method.method) {
            return None;
        }
        let name = call.method.method.name.clone();
        let expected = if (name == "op_Inequality") ^ negated {
            1
        } else {
            0
        };
        if f.match_ldc_i4(false_inst) != Some(expected) {
            return None;
        }
        let lifted_operator = lift_user_defined_operator(ctx, &call.method);
        // Equality is special (true when both are null): only with one nullable variable.
        if (name == "op_Equality" || name == "op_Inequality") && self.vars().len() != 1 {
            return None;
        }
        let lifted_operator = lifted_operator?;
        let params = call.method.method.params().unwrap_or_default();
        let (left, right, bits) = self.do_lift_binary(
            f,
            ctx,
            call.args[0],
            call.args[1],
            params.first().copied(),
            params.get(1).copied(),
        )?;
        if !all_bits(&bits, self.vars().len()) {
            return None;
        }
        let mut result = new_call_like(f, &call, lifted_operator, vec![left, right]);
        f.add_il_range(result, f.il_range(true_inst));
        if negated {
            result = logic_not(f, ctx, result);
        }
        Some(result)
    }

    /// `LiftNormal`.
    fn lift_normal(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        mut true_inst: NodeId,
        false_inst: NodeId,
    ) -> Option<NodeId> {
        if let Inst::If {
            condition,
            true_inst: nested_true,
            false_inst: nested_false,
            ..
        } = *f.inst(true_inst)
        {
            let (mut nested_condition, mut nested_true, mut nested_false) =
                (condition, nested_true, nested_false);
            while let Some(arg) = f.match_logic_not(nested_condition) {
                nested_condition = arg;
                std::mem::swap(&mut nested_true, &mut nested_false);
            }
            // Roslyn's pointless `nullable.HasValue && (!nullable.HasValue || ...)`.
            if let Some(v) = match_has_value_call_var(f, ctx, nested_condition)
                && self.vars().contains(&v)
            {
                true_inst = nested_true;
            }
        }
        let mut is_null_coalescing_with_non_nullable_fallback = false;
        let (utype, expr_to_lift) = match match_nullable_ctor(f, ctx, true_inst) {
            Some(m) => m,
            None => {
                is_null_coalescing_with_non_nullable_fallback = true;
                let utype = f.infer_type(ctx.ts, true_inst);
                if self.vars().len() == 1 && f.match_ldloc(true_inst) == Some(self.vars()[0]) {
                    // v.HasValue ? ldloc v : fallback => v ?? fallback
                    let ty = f.var(self.vars()[0]).ty;
                    return Some(f.add(Inst::NullCoalescing {
                        kind: NullCoalescingKind::Nullable,
                        ty,
                        value: true_inst,
                        fallback: false_inst,
                    }));
                }
                if let Some(result) =
                    self.lift_csharp_user_comparison(f, ctx, true_inst, false_inst)
                {
                    return Some(result);
                }
                (utype, true_inst)
            }
        };
        let mut lifted;
        if self.vars().len() == 1
            && match_get_value_or_default_var(f, ctx, expr_to_lift) == Some(self.vars()[0])
        {
            // v.HasValue ? v.GetValueOrDefault() : fallback => conv.nop.lifted(ldloc v) ?? fallback;
            // DoLift introduces no nop conversions, hence this case.
            let v = self.vars()[0];
            let input_utype = nullable_underlying(ctx, f.var(v).ty);
            lifted = f.add(Inst::LdLoc { var: v });
            let target = ctx.ts.primitive_type(utype);
            if input_utype != utype && target != PrimitiveType::None {
                // short? and int? are different structs, unlike short and int on the stack.
                let input = ctx.ts.stack_type(input_utype);
                let input_sign = ctx.ts.type_sign(input_utype);
                lifted = f.add(Inst::Conv {
                    arg: lifted,
                    input,
                    input_sign,
                    target,
                    check_overflow: false,
                    is_lifted: true,
                });
            }
        } else {
            let (l, bits) = self.do_lift(f, ctx, expr_to_lift)?;
            if !all_bits(&bits, self.vars().len()) {
                // A nullable variable that does not contribute to the result.
                return None;
            }
            lifted = l;
        }
        if is_null_coalescing_with_non_nullable_fallback {
            lifted = f.add(Inst::NullCoalescing {
                kind: NullCoalescingKind::NullableWithValueFallback,
                ty: utype,
                value: lifted,
                fallback: false_inst,
            });
        } else if match_null(f, ctx, false_inst) != Some(utype) {
            // The fallback is no `default(utype?)`: `??` provides it.
            let ty = nullable_type(ctx.ts, utype);
            lifted = f.add(Inst::NullCoalescing {
                kind: NullCoalescingKind::Nullable,
                ty,
                value: lifted,
                fallback: false_inst,
            });
        }
        Some(lifted)
    }

    /// `DoLift`: the lifted form of `inst` and which nullable variables it uses.
    fn do_lift(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        inst: NodeId,
    ) -> Option<(NodeId, Bits)> {
        let count = self.vars().len();
        if let Some(input_var) = match_get_value_or_default_var(f, ctx, inst) {
            // n.GetValueOrDefault() lifted is n.
            let found: Bits = self.vars().iter().map(|&v| v == input_var).collect();
            if !found.iter().any(|&b| b) {
                return None;
            }
            let load = f.add(Inst::LdLoc { var: input_var });
            f.add_il_range(load, f.il_range(inst));
            return Some((load, found));
        }
        match f.inst(inst).clone() {
            Inst::Conv {
                arg,
                input,
                input_sign,
                target,
                check_overflow,
                ..
            } => {
                let (arg, bits) = self.do_lift(f, ctx, arg)?;
                // A throwing instruction needs all nullable variables as arguments, so it
                // does not run when one is null.
                let throws =
                    f.inst(inst).direct_flags().0 & crate::il::ast::Flags::MAY_THROW.0 != 0;
                if throws && !all_bits(&bits, count) {
                    return None;
                }
                let new = f.add(Inst::Conv {
                    arg,
                    input,
                    input_sign,
                    target,
                    check_overflow,
                    is_lifted: true,
                });
                f.add_il_range(new, f.il_range(inst));
                Some((new, bits))
            }
            Inst::BitNot { arg, .. } => {
                let underlying = f.result_type(ctx.ts, inst);
                let (arg, bits) = self.do_lift(f, ctx, arg)?;
                let new = f.add(Inst::BitNot {
                    arg,
                    lifted_underlying: Some(underlying),
                });
                f.add_il_range(new, f.il_range(inst));
                Some((new, bits))
            }
            Inst::BinaryNumeric {
                op,
                check_overflow,
                sign,
                left_input,
                right_input,
                left,
                right,
                ..
            } => {
                let (l, r, bits) = self.do_lift_binary(f, ctx, left, right, None, None)?;
                let throws =
                    f.inst(inst).direct_flags().0 & crate::il::ast::Flags::MAY_THROW.0 != 0;
                if throws && !all_bits(&bits, count) {
                    return None;
                }
                let new = f.add(Inst::BinaryNumeric {
                    op,
                    check_overflow,
                    sign,
                    left_input,
                    right_input,
                    left: l,
                    right: r,
                    csharp_result_type: None,
                    is_lifted: true,
                });
                f.add_il_range(new, f.il_range(inst));
                Some((new, bits))
            }
            Inst::Comp {
                kind: ComparisonKind::Equality,
                lifting: ComparisonLiftingKind::None,
                input,
                sign,
                left,
                right,
            } if self.is_lifted_bool_not(f, ctx, left, right) => {
                // C# has three-valued logic only for operator ! on bool?.
                let (arg, bits) = self.do_lift(f, ctx, left).expect("a lifted operand");
                let right_copy = f.clone_subtree(right);
                let new = new_comp(
                    f,
                    ComparisonKind::Equality,
                    ComparisonLiftingKind::ThreeValuedLogic,
                    input,
                    sign,
                    arg,
                    right_copy,
                );
                f.add_il_range(new, f.il_range(inst));
                Some((new, bits))
            }
            Inst::Call(call) if ctx.ts.method_is_operator(&call.method.method) => {
                // Lifted user-defined operators, but not comparisons (those return bool).
                let lifted_operator = lift_user_defined_operator(ctx, &call.method)?;
                if !is_nullable(ctx, lifted_operator.method.ret) {
                    return None;
                }
                let (args, bits) = match call.args[..] {
                    [arg] => {
                        let (a, bits) = self.do_lift(f, ctx, arg)?;
                        (vec![a], bits)
                    }
                    [l, r] => {
                        let params = call.method.method.params().unwrap_or_default();
                        let (a, b, bits) = self.do_lift_binary(
                            f,
                            ctx,
                            l,
                            r,
                            params.first().copied(),
                            params.get(1).copied(),
                        )?;
                        (vec![a, b], bits)
                    }
                    _ => return None,
                };
                // A method call (a side effect) must involve all the nullable variables.
                if !all_bits(&bits, count) {
                    return None;
                }
                let new = new_call_like(f, &call, lifted_operator, args);
                f.add_il_range(new, f.il_range(inst));
                Some((new, bits))
            }
            _ => None,
        }
    }

    /// The `DoLift` case `comp(v.GetValueOrDefault() == ldc.i4 0)` of a `bool?` variable.
    fn is_lifted_bool_not(
        &self,
        f: &Function,
        ctx: &mut Context<'_, '_, '_>,
        left: NodeId,
        right: NodeId,
    ) -> bool {
        let Some(v) = match_get_value_or_default_var(f, ctx, left) else {
            return false;
        };
        if !self.vars().contains(&v) || f.match_ldc_i4(right) != Some(0) {
            return false;
        }
        let underlying = nullable_underlying(ctx, f.var(v).ty);
        known(ctx, underlying) == Some(KnownTypeCode::Boolean)
    }

    /// `DoLiftBinary`: both operands lifted, or a pure non-nullable operand wrapped in
    /// `new Nullable<T>(...)`.
    fn do_lift_binary(
        &mut self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        lhs: NodeId,
        rhs: NodeId,
        left_expected: Option<TypeId>,
        right_expected: Option<TypeId>,
    ) -> Option<(NodeId, NodeId, Bits)> {
        let left = self.do_lift(f, ctx, lhs);
        let right = self.do_lift(f, ctx, rhs);
        let (left, left_bits) = match left {
            Some((l, b)) => (Some(l), Some(b)),
            None => (None, None),
        };
        let (mut right, right_bits) = match right {
            Some((r, b)) => (Some(r), Some(b)),
            None => (None, None),
        };
        let mut left = left;
        if left.is_some() && right.is_none() && is_pure(f.flags(rhs)) {
            // A pure non-nullable expression in the lifted expression.
            let copy = f.clone_subtree(rhs);
            right = Some(new_nullable(f, ctx, copy, right_expected));
        }
        if left.is_none() && right.is_some() && is_pure(f.flags(lhs)) {
            let copy = f.clone_subtree(lhs);
            left = Some(new_nullable(f, ctx, copy, left_expected));
        }
        let (left, right) = (left?, right?);
        let mut bits = left_bits
            .or(right_bits.clone())
            .expect("bits of a lifted operand");
        if let Some(rb) = right_bits {
            for (i, b) in rb.into_iter().enumerate() {
                if b {
                    bits[i] = true;
                }
            }
        }
        Some((left, right, bits))
    }
}

/// `NewNullable`: `new Nullable<T>(inst)`, or `inst` when the expected type is unknown.
fn new_nullable(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    underlying: Option<TypeId>,
) -> NodeId {
    let Some(underlying) = underlying else {
        return inst;
    };
    if ctx.ts.is_unknown_kind(underlying)
        && matches!(ctx.ts.data(underlying), TypeData::UnknownSpecial)
    {
        return inst;
    }
    let ty = nullable_type(ctx.ts, underlying);
    let ctors = ctx
        .ts
        .get_constructors(ty, MemberOptions::DECLARED)
        .unwrap_or_default();
    let Some(ctor) = ctors
        .into_iter()
        .find(|c| c.params().is_ok_and(|p| p.len() == 1))
    else {
        return inst;
    };
    f.add(Inst::NewObj(CallData {
        method: MethodOperand {
            range_accessor: None, // Found by type lookup: the wire sends the constructor's signature.
            specialized_by: None,
            token: 0,
            method: ctor,
            context: 0,
            lifted: false,
        },
        args: vec![inst],
        is_tail: false,
        constrained_to: None,
        il_stack_was_empty: false,
    }))
}

/// `IsGenericNewPattern`: `(default(T) == null) ? Activator.CreateInstance<T>() : default(T)`.
fn is_generic_new_pattern(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    comp_left: NodeId,
    comp_right: NodeId,
    true_inst: NodeId,
    false_inst: NodeId,
) -> bool {
    let Inst::DefaultValue { ty, .. } = *f.inst(false_inst) else {
        return false;
    };
    let is_create_instance = match f.inst(true_inst) {
        Inst::Call(c) => {
            let m = &c.method.method;
            let type_args = m.method_args.clone().unwrap_or_else(|| m.type_args.clone());
            m.declaring
                .is_some_and(|d| ctx.ts.full_name(d) == "System.Activator")
                && m.name == "CreateInstance"
                && type_args.len() == 1
        }
        _ => false,
    };
    let Inst::DefaultValue { ty: ty2, .. } = *f.inst(comp_left) else {
        return false;
    };
    is_create_instance
        && matches!(ctx.ts.data(ty.ty), TypeData::TypeParam { .. })
        && ty.ty == ty2.ty
        && matches!(f.inst(comp_right), Inst::LdNull)
}

/// `MatchThreeValuedLogicConditionPattern`:
/// `nullable1.GetValueOrDefault() || (!nullable2.GetValueOrDefault() && !nullable1.HasValue)`.
fn match_three_valued_logic_condition_pattern(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
) -> Option<(VarId, VarId)> {
    let (lhs, rhs) = match_logic_or(f, condition)?;
    let nullable1 = match_get_value_or_default_var(f, ctx, lhs)?;
    let u1 = nullable_underlying(ctx, f.var(nullable1).ty);
    if known(ctx, u1) != Some(KnownTypeCode::Boolean) {
        return None;
    }
    let (lhs, rhs) = match_logic_and(f, rhs)?;
    let arg = f.match_logic_not(lhs)?;
    let nullable2 = match_get_value_or_default_var(f, ctx, arg)?;
    let u2 = nullable_underlying(ctx, f.var(nullable2).ty);
    if known(ctx, u2) != Some(KnownTypeCode::Boolean) {
        return None;
    }
    let arg = f.match_logic_not(rhs)?;
    (match_has_value_call_var(f, ctx, arg) == Some(nullable1)).then_some((nullable1, nullable2))
}

/// `Block.Unwrap`.
fn unwrap_block(f: &Function, inst: NodeId) -> NodeId {
    if let Inst::Block { final_inst, .. } = *f.inst(inst)
        && f.list(inst).len() == 1
        && f.match_nop(final_inst)
    {
        return f.list(inst)[0];
    }
    inst
}
