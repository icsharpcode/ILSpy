//! `IL/Transforms/TransformAssignment.cs`, the part ExpressionTransforms calls:
//! `HandleCompoundAssign` turns `x = x op y` into compound assignments.

use crate::il::ast::{
    BinaryNumericOperator, BlockKind, CompoundEvalMode, CompoundTargetKind, ConversionKind,
    DynamicKind, Function, Inst, MethodOperand, NodeId, VarId, VariableKind, conversion_kind,
};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::types::{Sign, StackType};
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::known;
use super::statement_transform::StatementTransformContext;

/// The `MethodSemanticsAttributes` of a property setter and getter.
const SETTER: u16 = 0x0001;
const GETTER: u16 = 0x0002;
/// The Property table, as the high byte of an accessor owner token.
const PROPERTY_TABLE: u32 = 0x17;

/// The call data of a `call`, `callvirt` or `newobj` (`CallInstruction`), with whether it is a
/// `newobj`.
fn call_instruction(f: &Function, id: NodeId) -> Option<(&crate::il::ast::CallData, u8)> {
    match f.inst(id) {
        Inst::Call(c) => Some((c, 0)),
        Inst::CallVirt(c) => Some((c, 1)),
        Inst::NewObj(c) => Some((c, 2)),
        _ => None,
    }
}

/// The property and semantics of an accessor (`IMethod.AccessorOwner`, `AccessorKind`).
fn property_accessor(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> Option<(u32, u16)> {
    let MethodOrigin::Def(id) = m.origin else {
        return None;
    };
    let (owner, semantics) = ctx.ts.accessor_owner(id)?;
    (owner >> 24 == PROPERTY_TABLE).then_some((owner, semantics))
}

/// `IsSameMember(call.Method, (call.Method.AccessorOwner as IProperty)?.Setter)`.
fn is_property_setter(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    // An unresolved `set_X` is guessed to be the setter of a fake property.
    if m.origin == MethodOrigin::Fake {
        return m.accessor_kind == SETTER;
    }
    property_accessor(ctx, m).is_some_and(|(_, s)| s == SETTER)
}

/// `IType.IsSmallIntegerType()`: a size of 1 to 3 bytes.
fn is_small_integer_type(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    let size = ctx.ts.type_size(t);
    size > 0 && size < 4
}

/// `ILVariableEqualityComparer.Equals`: the same variable, or two parts of one IL local.
fn same_il_variable(f: &Function, a: VarId, b: VarId) -> bool {
    if a == b {
        return true;
    }
    let (x, y) = (f.var(a), f.var(b));
    if matches!(x.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
        || matches!(y.kind, VariableKind::StackSlot | VariableKind::PatternLocal)
        || x.kind != y.kind
    {
        return false;
    }
    if x.index.is_some() {
        return x.index == y.index;
    }
    match (&x.state_machine_field, &y.state_machine_field) {
        (Some(fx), Some(fy)) => {
            fx.field.origin == fy.field.origin && fx.field.declaring == fy.field.declaring
        }
        _ => false,
    }
}

/// `ILFunction.RecombineVariables(variable1, variable2)`: the uses of `variable2` move to
/// `variable1`; the block driver drops `variable2` from the function's variables.
fn recombine_variables(f: &mut Function, ctx: &mut Context<'_, '_, '_>, v1: VarId, v2: VarId) {
    if v1 == v2 {
        return;
    }
    let v = f.var(v2);
    let uses: Vec<NodeId> = v
        .load_instructions
        .iter()
        .chain(&v.store_instructions)
        .chain(&v.address_instructions)
        .copied()
        .collect();
    for inst in uses {
        f.set_variable(inst, v1);
    }
    ctx.removed_variables.push(v2);
}

/// `UnwrapSmallIntegerConv`: the argument of a truncating `conv` to a small integer type.
fn unwrap_small_integer_conv(f: &Function, inst: NodeId) -> (NodeId, Option<NodeId>) {
    if let Inst::Conv {
        arg,
        input,
        input_sign,
        target,
        ..
    } = *f.inst(inst)
        && conversion_kind(target, input, input_sign) == ConversionKind::Truncate
        && target.is_small_integer()
    {
        return (arg, Some(inst));
    }
    (inst, None)
}

/// `IsCompoundStore`: the type stored to and the value stored, when `inst` can be a compound
/// store.
fn is_compound_store(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId)> {
    match f.inst(inst) {
        &Inst::StObj {
            target, value, ty, ..
        } => {
            // `stobj.Type` may be `int` (from stind.i4) for a `ref MyEnum`: prefer the type of
            // the target when memory accesses through both agree.
            let target_type = f.infer_type(ctx.ts, target);
            let element = match *ctx.ts.data(target_type) {
                TypeData::ByRef(elem) | TypeData::Pointer(elem) => Some(elem),
                _ => None,
            };
            let store_type = match element {
                Some(elem) if ctx.ts.is_compatible_type_for_memory_access(elem, ty.ty) => elem,
                _ => ty.ty,
            };
            is_pure(f.flags(target)).then_some((store_type, value))
        }
        Inst::Call(c) | Inst::CallVirt(c) => {
            let params = c.method.method.params().ok()?;
            let &store_type = params.last()?;
            for &arg in &c.args[..c.args.len().saturating_sub(1)] {
                if let Some((v, _)) = f.match_stloc(arg)
                    && f.var(v).is_single_definition()
                    && f.var(v).load_count() == 1
                {
                    // IsMatchingCompoundLoad adjusts this case.
                    continue;
                }
                if !is_pure(f.flags(arg)) {
                    return None;
                }
            }
            let value = *c.args.last()?;
            let method = c.method.method.clone();
            is_property_setter(ctx, &method).then_some((store_type, value))
        }
        &Inst::StLoc { var, value, .. }
            if matches!(
                f.var(var).kind,
                VariableKind::Local | VariableKind::Parameter
            ) =>
        {
            Some((f.var(var).ty, value))
        }
        _ => None,
    }
}

/// How `IsMatchingCompoundLoad` adjusts the tree once the transform commits.
enum FinalizeMatch {
    None,
    /// `AdjustArguments`: the setter's argument temporaries move into the getter call.
    AdjustArguments {
        getter: NodeId,
        setter: NodeId,
    },
    /// `RecombineVariables(load, store)`.
    Recombine(VarId, VarId),
}

impl FinalizeMatch {
    fn apply(self, f: &mut Function, ctx: &mut Context<'_, '_, '_>) {
        match self {
            FinalizeMatch::None => {}
            FinalizeMatch::AdjustArguments { getter, setter } => {
                let getter_args = call_instruction(f, getter).expect("a call").0.args.len();
                for j in 0..getter_args {
                    let setter_arg = call_instruction(f, setter).expect("a call").0.args[j];
                    if let Some((_, value)) = f.match_stloc(setter_arg) {
                        f.set_child_at(getter, j, value);
                    }
                }
            }
            FinalizeMatch::Recombine(v1, v2) => recombine_variables(f, ctx, v1, v2),
        }
    }
}

/// `MatchingGetterAndSetterCalls`: a getter and setter of one property with the same
/// arguments.
fn matching_getter_and_setter_calls(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    getter: NodeId,
    setter: NodeId,
) -> Option<FinalizeMatch> {
    let (getter_call, getter_op) = call_instruction(f, getter)?;
    let (setter_call, setter_op) = call_instruction(f, setter)?;
    let getter_method = getter_call.method.method.clone();
    let setter_method = setter_call.method.method.clone();
    let (getter_owner, getter_semantics) = property_accessor(ctx, &getter_method)?;
    let (setter_owner, setter_semantics) = property_accessor(ctx, &setter_method)?;
    let same_owner = getter_owner == setter_owner
        && matches!((getter_method.origin, setter_method.origin),
            (MethodOrigin::Def(a), MethodOrigin::Def(b)) if a.module == b.module);
    if !same_owner || getter_op != setter_op {
        return None;
    }
    if getter_semantics != GETTER || setter_semantics != SETTER {
        return None;
    }
    let (getter_args, setter_args) = (&getter_call.args, &setter_call.args);
    if setter_args.len() != getter_args.len() + 1 {
        return None;
    }
    let mut finalize = FinalizeMatch::None;
    for j in 0..getter_args.len() {
        if let Some((v, _)) = f.match_stloc(setter_args[j])
            && f.var(v).is_single_definition()
            && f.var(v).load_count() == 1
            && f.match_ldloc(getter_args[j]) == Some(v)
        {
            // The setter's argument is saved in a temporary the getter call reuses.
            finalize = FinalizeMatch::AdjustArguments { getter, setter };
            continue;
        }
        if !is_pure(f.flags(getter_args[j]))
            || !f.structurally_equal(getter_args[j], setter_args[j])
        {
            return None;
        }
    }
    Some(finalize)
}

/// `IsMatchingCompoundLoad`: `load` reads what `store` writes. Returns the target, its kind
/// and the adjustment to make. `previous` is the instruction before the load, if the caller
/// knows it.
fn is_matching_compound_load(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    load: NodeId,
    store: NodeId,
    forbidden: Option<VarId>,
    previous: Option<NodeId>,
) -> Option<(NodeId, CompoundTargetKind, FinalizeMatch)> {
    if let (
        &Inst::LdObj { target, .. },
        &Inst::StObj {
            target: store_target,
            ..
        },
    ) = (f.inst(load), f.inst(store))
    {
        if !is_pure(f.flags(target)) {
            return None;
        }
        if forbidden.is_some_and(|v| f.is_used_within(v, target)) {
            return None;
        }
        if f.structurally_equal(target, store_target) {
            return Some((target, CompoundTargetKind::Address, FinalizeMatch::None));
        }
        // S_0 as the target lets inlining remove it later (the previous instruction lies
        // before the transform's starting instruction).
        return is_duplicated_address_computation(f, store_target, target, forbidden, previous)
            .then_some((
                store_target,
                CompoundTargetKind::Address,
                FinalizeMatch::None,
            ));
    }
    if let Some(finalize) = matching_getter_and_setter_calls(f, ctx, load, store) {
        if forbidden.is_some_and(|v| f.is_used_within(v, load)) {
            return None;
        }
        return Some((load, CompoundTargetKind::Property, finalize));
    }
    if let (&Inst::LdLoc { var: lv }, &Inst::StLoc { var: sv, .. }) = (f.inst(load), f.inst(store))
        && same_il_variable(f, lv, sv)
    {
        if forbidden.is_some_and(|v| same_il_variable(f, lv, v)) {
            return None;
        }
        let target = f.add(Inst::LdLoca { var: lv });
        f.add_il_range(target, f.il_range(load));
        return Some((
            target,
            CompoundTargetKind::Address,
            FinalizeMatch::Recombine(lv, sv),
        ));
    }
    None
}

/// `ImplicitTruncationResult`.
#[allow(clippy::enum_variant_names)]
#[derive(Clone, Copy, PartialEq, Eq)]
enum Truncation {
    ValuePreserved,
    ValueChanged,
    ValueChangedDueToSignMismatch,
}

/// `IsImplicitTruncation`: storing `value` into a variable of `ty` may change it.
pub(crate) fn is_implicit_truncation(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    value: NodeId,
    ty: TypeId,
    allow_nullable_value: bool,
) -> bool {
    check_implicit_truncation(f, ctx, value, ty, allow_nullable_value) != Truncation::ValuePreserved
}

fn common_truncation(left: Truncation, right: Truncation) -> Truncation {
    if left == right {
        left
    } else {
        Truncation::ValueChanged
    }
}

/// `CheckImplicitTruncation`.
fn check_implicit_truncation(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    value: NodeId,
    ty: TypeId,
    allow_nullable_value: bool,
) -> Truncation {
    use Truncation::*;
    // Implicit truncation only happens for small integer types; other truncations in IL get
    // conv instructions from the reader.
    if !is_small_integer_type(ctx, ty) {
        return ValuePreserved;
    }
    match *f.inst(value) {
        Inst::LdcI4 { value: v } => {
            let fits = match ctx.ts.enum_underlying_known(ty) {
                Some(KnownTypeCode::Boolean) => v == 0 || v == 1,
                Some(KnownTypeCode::Byte) => (0..=255).contains(&v),
                Some(KnownTypeCode::SByte) => (-128..=127).contains(&v),
                Some(KnownTypeCode::Int16) => (-32768..=32767).contains(&v),
                Some(KnownTypeCode::UInt16 | KnownTypeCode::Char) => (0..=65535).contains(&v),
                _ => false,
            };
            if fits { ValuePreserved } else { ValueChanged }
        }
        Inst::Conv { target, .. } => {
            let primitive = ctx.ts.primitive_type(ty);
            if target == primitive {
                ValuePreserved
            } else if primitive.size() == target.size()
                && primitive.sign() != target.sign()
                && primitive.has_opposite_sign()
            {
                ValueChangedDueToSignMismatch
            } else {
                ValueChanged
            }
        }
        // comp yields 0 or 1, which always fits.
        Inst::Comp { .. } => ValuePreserved,
        Inst::BinaryNumeric {
            op:
                BinaryNumericOperator::BitAnd
                | BinaryNumericOperator::BitOr
                | BinaryNumericOperator::BitXor,
            left,
            right,
            ..
        } => {
            // When both inputs fit, so does the result.
            let l = check_implicit_truncation(f, ctx, left, ty, allow_nullable_value);
            if l == ValueChanged {
                return ValueChanged;
            }
            let r = check_implicit_truncation(f, ctx, right, ty, allow_nullable_value);
            common_truncation(l, r)
        }
        Inst::BinaryNumeric { .. } => ValueChanged,
        Inst::If {
            true_inst,
            false_inst,
            ..
        } => {
            let t = check_implicit_truncation(f, ctx, true_inst, ty, allow_nullable_value);
            if t == ValueChanged {
                return ValueChanged;
            }
            let e = check_implicit_truncation(f, ctx, false_inst, ty, allow_nullable_value);
            common_truncation(t, e)
        }
        _ => {
            let mut inferred = f.infer_type(ctx.ts, value);
            if allow_nullable_value {
                inferred = nullable_underlying(ctx, inferred);
            }
            if ctx.ts.is_unknown_kind(inferred) {
                return ValueChanged;
            }
            let inferred_primitive = ctx.ts.primitive_type(inferred);
            let primitive = ctx.ts.primitive_type(ty);
            let same_sign = inferred_primitive.sign() == primitive.sign();
            if inferred_primitive.size() <= primitive.size() && same_sign {
                ValuePreserved
            } else if inferred_primitive.size() == primitive.size()
                && !same_sign
                && primitive.has_opposite_sign()
            {
                ValueChangedDueToSignMismatch
            } else {
                ValueChanged
            }
        }
    }
}

/// `NullableType.GetUnderlyingType`.
pub(crate) fn nullable_underlying(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    if let TypeData::Inst { generic, args } = ctx.ts.data(t)
        && args.len() == 1
    {
        let (generic, arg) = (*generic, args[0]);
        if known(ctx, generic) == Some(KnownTypeCode::NullableOfT) {
            return arg;
        }
    }
    t
}

/// `PointerArithmeticOffset.ComputeSizeOf`.
fn compute_size_of(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<i64> {
    match ctx.ts.enum_underlying_known(t)? {
        KnownTypeCode::Boolean | KnownTypeCode::SByte | KnownTypeCode::Byte => Some(1),
        KnownTypeCode::Char | KnownTypeCode::Int16 | KnownTypeCode::UInt16 => Some(2),
        KnownTypeCode::Int32 | KnownTypeCode::UInt32 | KnownTypeCode::Single => Some(4),
        KnownTypeCode::Int64 | KnownTypeCode::UInt64 | KnownTypeCode::Double => Some(8),
        KnownTypeCode::Decimal => Some(16),
        _ => None,
    }
}

/// What `PointerArithmeticOffset.Detect` returns: an existing instruction, or a new
/// `ldc.i4` with that value.
#[derive(Clone, Copy)]
pub(crate) enum PointerOffset {
    Inst(NodeId),
    Constant(i32),
}

/// `PointerArithmeticOffset.Detect`: the element count `byte_offset` stands for when it is a
/// multiple of the size of `element`.
pub(crate) fn pointer_offset_detect(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    mut byte_offset: NodeId,
    element: TypeId,
    check_for_overflow: bool,
    unwrap_zero_extension: bool,
) -> Option<PointerOffset> {
    if let Inst::Conv {
        arg,
        input: StackType::I8,
        target,
        ..
    } = *f.inst(byte_offset)
        && target.stack_type() == StackType::I
    {
        byte_offset = arg;
    }
    let size = compute_size_of(ctx, element);
    if size == Some(1) {
        return Some(PointerOffset::Inst(byte_offset));
    }
    let unwrap = |f: &Function, inst: NodeId, kind: ConversionKind| {
        super::expression_transforms::unwrap_conv(f, inst, kind)
    };
    match *f.inst(byte_offset) {
        Inst::BinaryNumeric {
            op: BinaryNumericOperator::Mul,
            check_overflow,
            left,
            right,
            ..
        } => {
            if check_overflow != check_for_overflow {
                return None;
            }
            let matches_size = size.is_some_and(|s| s > 0 && f.match_ldc_i(right) == Some(s))
                || match *f.inst(unwrap(f, right, ConversionKind::SignExtend)) {
                    Inst::SizeOf { ty } => {
                        ctx.ts.normalize_parameter(ty.ty) == ctx.ts.normalize_parameter(element)
                    }
                    _ => false,
                };
            if !matches_size {
                return None;
            }
            let count = if unwrap_zero_extension {
                unwrap(f, left, ConversionKind::ZeroExtend)
            } else {
                left
            };
            Some(PointerOffset::Inst(count))
        }
        _ => {
            if let Inst::SizeOf { ty } = *f.inst(unwrap(f, byte_offset, ConversionKind::SignExtend))
            {
                return (ty.ty == element).then_some(PointerOffset::Constant(1));
            }
            // A constant offset may be a constant-folded multiplication.
            match (f.match_ldc_i(byte_offset), size) {
                (Some(v), Some(s)) if s > 0 && v % s == 0 && v > 0 => {
                    i32::try_from(v / s).ok().map(PointerOffset::Constant)
                }
                _ => None,
            }
        }
    }
}

/// `NumericCompoundAssign.IsBinaryCompatibleWithType`.
fn is_binary_compatible_with_type(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    binary: NodeId,
    ty: TypeId,
) -> bool {
    let Inst::BinaryNumeric {
        op,
        sign,
        check_overflow,
        right,
        is_lifted,
        ..
    } = *f.inst(binary)
    else {
        return false;
    };
    let mut ty = ty;
    if is_lifted {
        if known(ctx, ty) != Some(KnownTypeCode::NullableOfT) {
            return false;
        }
        ty = nullable_underlying(ctx, ty);
    }
    // An unknown type risks a wrong compound assignment.
    if ctx.ts.is_unknown_kind(ty) {
        return false;
    }
    let kind = ctx
        .ts
        .definition(ty)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .map(|i| i.kind);
    use BinaryNumericOperator as Op;
    if let TypeData::Pointer(element) = *ctx.ts.data(ty) {
        return match op {
            // The byte offset is a multiple of the element size.
            Op::Add | Op::Sub => {
                pointer_offset_detect(f, ctx, right, element, check_overflow, false).is_some()
            }
            _ => false,
        };
    }
    if kind == Some(TypeKind::Enum) {
        if !matches!(op, Op::Add | Op::Sub | Op::BitAnd | Op::BitOr | Op::BitXor) {
            return false;
        }
    } else if matches!(
        known(ctx, ty),
        Some(KnownTypeCode::IntPtr | KnownTypeCode::UIntPtr)
    ) && !matches!(ctx.ts.data(ty), TypeData::NInt | TypeData::NUInt)
    {
        // `target.intptr *= (nint)2` needs native integers; shifts do not work even then.
        if !ctx.settings.native_integers || matches!(op, Op::ShiftLeft | Op::ShiftRight) {
            return false;
        }
    }
    if sign != Sign::None {
        let sign_mismatch_allowed =
            sign == Sign::Unsigned && op == Op::ShiftRight && ctx.settings.unsigned_right_shift;
        let csharp_small_integer = matches!(
            known(ctx, ty),
            Some(
                KnownTypeCode::Byte
                    | KnownTypeCode::SByte
                    | KnownTypeCode::Int16
                    | KnownTypeCode::UInt16
            )
        );
        if csharp_small_integer {
            // C# promotes to int, so the operation must be signed.
            if sign != Sign::Signed && !sign_mismatch_allowed {
                return false;
            }
        } else if ctx.ts.type_sign(ty) != sign && !sign_mismatch_allowed {
            return false;
        }
    }
    // The right-hand side must not need truncation for the target type.
    !is_implicit_truncation(f, ctx, right, ty, is_lifted)
}

/// `ValidateCompoundAssign`.
fn validate_compound_assign(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    binary: NodeId,
    conv: Option<NodeId>,
    target_type: TypeId,
) -> bool {
    if !is_binary_compatible_with_type(f, ctx, binary, target_type) {
        return false;
    }
    let Some(conv) = conv else {
        return true;
    };
    let (
        Inst::Conv {
            target,
            check_overflow: conv_overflow,
            ..
        },
        Inst::BinaryNumeric { check_overflow, .. },
    ) = (f.inst(conv), f.inst(binary))
    else {
        return false;
    };
    *target == ctx.ts.primitive_type(target_type) && conv_overflow == check_overflow
}

/// `ExpressionBuilder.GetAssignmentOperatorTypeFromMetadataName(name) != null`.
fn is_assignment_operator_name(ctx: &Context<'_, '_, '_>, name: &str) -> bool {
    match name {
        "op_Addition" | "op_Subtraction" | "op_Multiply" | "op_Division" | "op_Modulus"
        | "op_BitwiseAnd" | "op_BitwiseOr" | "op_ExclusiveOr" | "op_LeftShift"
        | "op_RightShift" => true,
        "op_UnsignedRightShift" => ctx.settings.unsigned_right_shift,
        _ => false,
    }
}

/// `UserDefinedCompoundAssign.IsIncrementOrDecrement`.
pub(crate) fn is_increment_or_decrement(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    if !(m.is_static && ctx.ts.method_is_operator(m)) {
        return false;
    }
    match m.name.as_str() {
        "op_Increment" | "op_Decrement" => true,
        "op_CheckedIncrement" | "op_CheckedDecrement" => ctx.settings.checked_operators,
        _ => false,
    }
}

/// `UserDefinedCompoundAssign.IsStringConcat`.
fn is_string_concat(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    m.name == "Concat"
        && m.is_static
        && m.declaring
            .is_some_and(|d| known(ctx, d) == Some(KnownTypeCode::String))
}

/// The single type argument of `t` when it is `known_code<T>`.
fn single_type_argument(
    ctx: &mut Context<'_, '_, '_>,
    t: TypeId,
    code: KnownTypeCode,
) -> Option<TypeId> {
    if known(ctx, t) != Some(code) {
        return None;
    }
    match ctx.ts.data(t) {
        TypeData::Inst { args, .. } if args.len() == 1 => Some(args[0]),
        _ => None,
    }
}

/// `CallBuilder.IsStringToReadOnlySpanCharImplicitConversion`.
fn is_string_to_read_only_span_char_implicit_conversion(
    ctx: &mut Context<'_, '_, '_>,
    m: &ResolvedMethod,
) -> bool {
    let Ok(params) = m.params() else {
        return false;
    };
    m.name == "op_Implicit"
        && ctx.ts.method_is_operator(m)
        && params.len() == 1
        && single_type_argument(ctx, m.ret, KnownTypeCode::ReadOnlySpanOfT)
            .is_some_and(|a| known(ctx, a) == Some(KnownTypeCode::Char))
        && known(ctx, params[0]) == Some(KnownTypeCode::String)
}

/// `ILInlining.IsReadOnlySpanCharCtor`.
fn is_read_only_span_char_ctor(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> bool {
    let Ok(params) = m.params() else {
        return false;
    };
    m.name == ".ctor"
        && params.len() == 1
        && m.declaring
            .and_then(|d| single_type_argument(ctx, d, KnownTypeCode::ReadOnlySpanOfT))
            .is_some_and(|a| known(ctx, a) == Some(KnownTypeCode::Char))
        && match *ctx.ts.data(params[0]) {
            TypeData::ByRef(e) => known(ctx, e) == Some(KnownTypeCode::Char),
            _ => false,
        }
}

/// Whether `inst` sits in the instruction list of a block (`Block.InstructionSlot`).
fn is_block_instruction(f: &Function, inst: NodeId) -> bool {
    f.node(inst).parent.is_some_and(|p| {
        matches!(f.inst(p), Inst::Block { .. })
            && (f.node(inst).child_index as usize) < f.list(p).len()
            && f.list(p)[f.node(inst).child_index as usize] == inst
    })
}

/// `HandleCompoundAssign`: compound assignments whose value is unused, or whose setter has an
/// inlined assignment.
///
/// ```text
/// callvirt set_P(ldloc S_1, binary.op(callvirt get_P(ldloc S_1), value))
///   => compound.op.new(callvirt get_P(ldloc S_1), value)
/// callvirt set_P(ldloc S_1, stloc v(binary.op(callvirt get_P(ldloc S_1), value)))
///   => stloc v(compound.op.new(callvirt get_P(ldloc S_1), value))
/// stobj(target, binary.op(ldobj(target), ...)), target pure
///   => compound.op(target, ...)
/// ```
pub fn handle_compound_assign(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    store: NodeId,
) -> bool {
    if !ctx.settings.make_assignment_expressions || !ctx.settings.introduce_increment_and_decrement
    {
        return false;
    }
    // A setter call replaced by a compound assignment changes the value of the expression,
    // so only a statement-level call qualifies.
    if call_instruction(f, store).is_some() && !is_block_instruction(f, store) {
        return false;
    }
    let Some((target_type, mut setter_value)) = is_compound_store(f, ctx, store) else {
        return false;
    };
    let store_in_setter = f.match_stloc(setter_value).map(|_| setter_value);
    let mut forbidden = None;
    if let Some(inner_store) = store_in_setter {
        // The stloc moves to the top level.
        let (v, value) = f.match_stloc(inner_store).expect("a store");
        forbidden = Some(v);
        setter_value = value;
        let var_type = f.var(v).ty;
        if is_small_integer_type(ctx, var_type) {
            // `stloc v` truncates: the type of v must match the property's.
            if ctx.ts.type_size(var_type) != ctx.ts.type_size(target_type)
                || ctx.ts.type_sign(var_type) != ctx.ts.type_sign(target_type)
            {
                return false;
            }
        }
    }
    let new_inst;
    let (unwrapped, small_int_conv) = unwrap_small_integer_conv(f, setter_value);
    if let Inst::BinaryNumeric {
        op,
        check_overflow,
        sign,
        left_input,
        right_input,
        left,
        right,
        is_lifted,
        ..
    } = *f.inst(unwrapped)
    {
        // Local variables become compound assignments only for user-defined operators.
        if matches!(f.inst(store), Inst::StLoc { .. }) {
            return false;
        }
        let Some((target, target_kind, finalize)) =
            is_matching_compound_load(f, ctx, left, store, forbidden, None)
        else {
            return false;
        };
        if !validate_compound_assign(f, ctx, unwrapped, small_int_conv, target_type) {
            return false;
        }
        finalize.apply(f, ctx);
        let underlying_result = crate::il::ast::binary_result_type(op, left_input, right_input);
        new_inst = f.add(Inst::NumericCompoundAssign {
            op,
            check_overflow,
            sign,
            left_input,
            right_input,
            underlying_result,
            is_lifted,
            ty: target_type,
            eval_mode: CompoundEvalMode::EvaluatesToNewValue,
            target_kind,
            target,
            value: right,
        });
        f.add_il_range(new_inst, f.il_range(unwrapped));
    } else if let Some(call) = match f.inst(setter_value) {
        Inst::Call(c) => Some(c.clone()),
        _ => None,
    } {
        let method = call.method.method.clone();
        let operand: MethodOperand = call.method.clone();
        let (load, rhs) = if ctx.ts.method_is_operator(&method) {
            let Some(&first) = call.args.first() else {
                return false;
            };
            let rhs = match call.args.len() {
                2 => {
                    if !is_assignment_operator_name(ctx, &method.name) {
                        return false;
                    }
                    Some(call.args[1])
                }
                1 => {
                    if !is_increment_or_decrement(ctx, &method) {
                        return false;
                    }
                    // A dummy value: there is no instruction for a user-defined unary
                    // operator call.
                    None
                }
                _ => return false,
            };
            // Nullables would need more thought (`operatorCall.IsLifted`).
            if call.method.lifted {
                return false;
            }
            (first, rhs)
        } else if is_string_concat(ctx, &method) {
            if matches!(f.inst(store), Inst::StLoc { .. }) {
                return false;
            }
            if call.args.len() != 2 {
                return false;
            }
            if known(ctx, target_type) != Some(KnownTypeCode::String) {
                return false;
            }
            let mut arg = call.args[0];
            if let Inst::Call(c) = f.inst(arg)
                && is_string_to_read_only_span_char_implicit_conversion(
                    ctx,
                    &c.method.method.clone(),
                )
            {
                arg = c.args[0];
                let char_span = match f.inst(call.args[1]) {
                    Inst::NewObj(n) => {
                        let ctor = n.method.method.clone();
                        matches!(n.args[..], [a] if matches!(f.inst(a), Inst::AddressOf { .. }))
                            && is_read_only_span_char_ctor(ctx, &ctor)
                    }
                    _ => false,
                };
                if !char_span {
                    return false;
                }
            }
            (arg, Some(call.args[1]))
        } else {
            return false;
        };
        let Some((target, target_kind, finalize)) =
            is_matching_compound_load(f, ctx, load, store, forbidden, None)
        else {
            return false;
        };
        finalize.apply(f, ctx);
        let value = match rhs {
            Some(value) => value,
            None => f.add(Inst::LdcI4 { value: 1 }),
        };
        new_inst = f.add(Inst::UserDefinedCompoundAssign {
            method: operand,
            eval_mode: CompoundEvalMode::EvaluatesToNewValue,
            target_kind,
            target,
            value,
        });
    } else if let Inst::Dynamic {
        kind: DynamicKind::BinaryOperator,
        binder_flags,
        operation,
        ref arg_infos,
        ref args,
        ..
    } = *f.inst(setter_value)
    {
        let (arg_infos, args) = (arg_infos.clone(), args.clone());
        let Some((target, target_kind, finalize)) =
            is_matching_compound_load(f, ctx, args[0], store, forbidden, None)
        else {
            return false;
        };
        finalize.apply(f, ctx);
        let operation = to_compound_operation(operation);
        // The managed constructor throws for operations without a compound form.
        assert!(
            super::expression_transforms::is_dynamic_compound_operation(operation),
            "ArgumentOutOfRangeException: no dynamic compound assignment for ExpressionType {operation}"
        );
        new_inst = f.add(Inst::DynamicCompoundAssign {
            operation,
            binder_flags,
            target_info: arg_infos[0].clone(),
            value_info: arg_infos[1].clone(),
            eval_mode: super::expression_transforms::dynamic_compound_eval_mode(operation),
            target_kind,
            target,
            value: args[1],
        });
    } else {
        return false;
    }
    f.add_il_range(new_inst, f.il_range(setter_value));
    let mut replacement = new_inst;
    if let Some(inner_store) = store_in_setter {
        f.set_child_at(inner_store, 0, new_inst);
        replacement = inner_store;
        // A top-level stloc may be inlined.
        ctx.rerun_requested = true;
    }
    f.replace(store, replacement);
    if let Some(parent) = f.node(replacement).parent
        && matches!(
            f.inst(parent),
            Inst::Block {
                kind: BlockKind::CallInlineAssign,
                ..
            }
        )
    {
        // `Block CallInlineAssign { stloc I_0(compound.op(...)); final: ldloc I_0 }`
        // becomes the compound assignment.
        f.replace(parent, new_inst);
    }
    true
}

/// `ToCompound`: the `...Assign` form of a binary `ExpressionType`; other operations stay.
fn to_compound_operation(operation: i32) -> i32 {
    match operation {
        0 => 63,  // Add => AddAssign
        1 => 74,  // AddChecked => AddAssignChecked
        2 => 64,  // And => AndAssign
        12 => 65, // Divide => DivideAssign
        14 => 66, // ExclusiveOr => ExclusiveOrAssign
        19 => 67, // LeftShift => LeftShiftAssign
        25 => 68, // Modulo => ModuloAssign
        26 => 69, // Multiply => MultiplyAssign
        27 => 75, // MultiplyChecked => MultiplyAssignChecked
        36 => 70, // Or => OrAssign
        39 => 71, // Power => PowerAssign
        41 => 72, // RightShift => RightShiftAssign
        42 => 73, // Subtract => SubtractAssign
        43 => 76, // SubtractChecked => SubtractAssignChecked
        other => other,
    }
}

/// `IsDuplicatedAddressComputation`: Roslyn sometimes computes the address twice:
/// ```text
/// stloc S_0(ldloc refParam)
/// stloc V_0(ldobj System.Int32(ldloc refParam))
/// stobj System.Int32(ldloc S_0, binary.add.i4(ldloc V_0, ldc.i4 1))
/// ```
fn is_duplicated_address_computation(
    f: &Function,
    mut store_target: NodeId,
    mut load_target: NodeId,
    forbidden: Option<VarId>,
    previous: Option<NodeId>,
) -> bool {
    while let (
        Inst::LdFlda {
            target: st,
            field: sf,
            ..
        },
        Inst::LdFlda {
            target: lt,
            field: lf,
            ..
        },
    ) = (f.inst(store_target), f.inst(load_target))
    {
        if !(sf.field.origin == lf.field.origin && sf.field.declaring == lf.field.declaring) {
            return false;
        }
        (store_target, load_target) = (*st, *lt);
    }
    let Some(s) = f.match_ldloc(store_target) else {
        return false;
    };
    let var = f.var(s);
    if !(var.kind == VariableKind::StackSlot && var.is_single_definition() && Some(s) != forbidden)
    {
        return false;
    }
    // `SingleOrDefault()` of the stores, compared with the previous instruction.
    let single_store = match var.store_instructions[..] {
        [only] => Some(only),
        _ => None,
    };
    if single_store != previous {
        return false;
    }
    previous
        .and_then(|p| f.match_stloc(p))
        .is_some_and(|(_, value)| f.structurally_equal(value, load_target))
}

/// `IStatementTransform.Run` of `TransformAssignment`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if ctx.settings.make_assignment_expressions
        && (transform_inline_assignment_stobj_or_call(r, ctx, block, pos)
            || transform_inline_assignment_local(&mut r.function, ctx, block, pos))
    {
        // Both create a top-level stloc, which may be inlined.
        sc.request_rerun();
        return;
    }
    let f = &mut r.function;
    if ctx.settings.introduce_increment_and_decrement
        && (transform_post_inc_dec_operator_with_inline_store(f, ctx, block, pos)
            || transform_post_inc_dec_operator(f, ctx, block, pos)
            || transform_pre_inc_dec_operator_with_inline_store(f, ctx, block, pos))
    {
        // The new top-level stloc may be inlined.
        sc.request_rerun();
    }
}

/// `SwapSign`: the integer type of the same size and the opposite sign.
fn swap_sign(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    use crate::il::types::PrimitiveType as P;
    let code = match ctx.ts.primitive_type(t) {
        P::I1 => KnownTypeCode::Byte,
        P::I2 => KnownTypeCode::UInt16,
        P::I4 => KnownTypeCode::UInt32,
        P::I8 => KnownTypeCode::UInt64,
        P::U1 => KnownTypeCode::SByte,
        P::U2 => KnownTypeCode::Int16,
        P::U4 => KnownTypeCode::Int32,
        P::U8 => KnownTypeCode::Int64,
        P::I => KnownTypeCode::UIntPtr,
        P::U => KnownTypeCode::IntPtr,
        _ => panic!("ArgumentException: type must have an opposing sign"),
    };
    ctx.ts.known_type(code)
}

/// `MatchLdcI(1) || MatchLdcF4(1) || MatchLdcF8(1)`.
fn is_one(f: &Function, inst: NodeId) -> bool {
    f.match_ldc_i(inst) == Some(1)
        || matches!(*f.inst(inst), Inst::LdcF4 { bits } if f32::from_bits(bits) == 1.0)
        || matches!(*f.inst(inst), Inst::LdcF8 { bits } if f64::from_bits(bits) == 1.0)
}

/// A new numeric compound assignment from `binary` (`new NumericCompoundAssign(binary, ...)`).
fn new_numeric_compound_assign(
    f: &mut Function,
    binary: NodeId,
    target: NodeId,
    target_kind: CompoundTargetKind,
    ty: TypeId,
    eval_mode: CompoundEvalMode,
) -> NodeId {
    let Inst::BinaryNumeric {
        op,
        check_overflow,
        sign,
        left_input,
        right_input,
        right,
        is_lifted,
        ..
    } = *f.inst(binary)
    else {
        unreachable!("a binary instruction");
    };
    let compound = f.add(Inst::NumericCompoundAssign {
        op,
        check_overflow,
        sign,
        left_input,
        right_input,
        underlying_result: crate::il::ast::binary_result_type(op, left_input, right_input),
        is_lifted,
        ty,
        eval_mode,
        target_kind,
        target,
        value: right,
    });
    f.add_il_range(compound, f.il_range(binary));
    compound
}

/// `new UserDefinedCompoundAssign(method, evalMode, target, targetKind, new LdcI4(1))`.
fn new_user_defined_increment(
    f: &mut Function,
    method: MethodOperand,
    target: NodeId,
    target_kind: CompoundTargetKind,
    eval_mode: CompoundEvalMode,
) -> NodeId {
    let one = f.add(Inst::LdcI4 { value: 1 });
    f.add(Inst::UserDefinedCompoundAssign {
        method,
        eval_mode,
        target_kind,
        target,
        value: one,
    })
}

fn new_stloc(f: &mut Function, var: VarId, value: NodeId) -> NodeId {
    f.add(Inst::StLoc {
        var,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    })
}

/// The increment or decrement in the value of an inc/dec pattern: the binary instruction
/// and its operand, or the user-defined operator call and its operand.
enum IncDec {
    Binary {
        binary: NodeId,
        operand: NodeId,
    },
    Operator {
        method: MethodOperand,
        operand: NodeId,
    },
}

/// The shared head of `TransformPreIncDecOperatorWithInlineStore` and
/// `TransformPostIncDecOperatorWithInlineStore`: a `+ 1` / `- 1` (after a small-integer
/// conversion) or `op_Increment` / `op_Decrement` call; `target_type` may get the opposite
/// sign for the conversion.
fn match_inc_dec(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    value: NodeId,
    target_type: &mut TypeId,
) -> Option<IncDec> {
    let (unwrapped, conv) = unwrap_small_integer_conv(f, value);
    if let Inst::BinaryNumeric {
        op, left, right, ..
    } = *f.inst(unwrapped)
        && is_one(f, right)
    {
        if !matches!(op, BinaryNumericOperator::Add | BinaryNumericOperator::Sub) {
            return None;
        }
        if let Some(conv) = conv
            && let Inst::Conv { target, .. } = *f.inst(conv)
        {
            let primitive = ctx.ts.primitive_type(*target_type);
            if primitive.size() == target.size() && primitive.sign() != target.sign() {
                *target_type = swap_sign(ctx, *target_type);
            }
        }
        if !validate_compound_assign(f, ctx, unwrapped, conv, *target_type) {
            return None;
        }
        return Some(IncDec::Binary {
            binary: unwrapped,
            operand: left,
        });
    }
    if let Inst::Call(c) = f.inst(value) {
        let method = c.method.method.clone();
        if ctx.ts.method_is_operator(&method) && c.args.len() == 1 {
            if !matches!(method.name.as_str(), "op_Increment" | "op_Decrement") {
                return None;
            }
            if c.method.lifted {
                return None;
            }
            return Some(IncDec::Operator {
                method: c.method.clone(),
                operand: c.args[0],
            });
        }
    }
    None
}

/// `TransformPreIncDecOperatorWithInlineStore`:
/// `stloc l(stloc target(binary.add(ldloc target, ldc.i4 1)))` becomes
/// `stloc l(compound.op.new(ldloca target, ldc.i4 1))`.
fn transform_pre_inc_dec_operator_with_inline_store(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let store = f.list(block)[pos];
    let Some((target_type1, value1)) = is_compound_store(f, ctx, store) else {
        return false;
    };
    let Some((target_type2, value2)) = is_compound_store(f, ctx, value1) else {
        return false;
    };
    if target_type1 != target_type2 {
        return false;
    }
    let mut target_type = target_type1;
    let Some(inc_dec) = match_inc_dec(f, ctx, value2, &mut target_type) else {
        return false;
    };
    let operand = match &inc_dec {
        IncDec::Binary { operand, .. } | IncDec::Operator { operand, .. } => *operand,
    };
    let (Some((outer_var, _)), Some(_)) = (f.match_stloc(store), f.match_stloc(value1)) else {
        return false;
    };
    if !matches!(f.inst(operand), Inst::LdLoc { .. }) {
        return false;
    }
    if !matches!(
        f.var(outer_var).kind,
        VariableKind::Local | VariableKind::StackSlot
    ) {
        return false;
    }
    let Some((target, target_kind, finalize)) =
        is_matching_compound_load(f, ctx, operand, value1, None, None)
    else {
        return false;
    };
    let outer_type = f.var(outer_var).ty;
    if is_implicit_truncation(f, ctx, value1, outer_type, false) {
        return false;
    }
    finalize.apply(f, ctx);
    let compound = match inc_dec {
        IncDec::Binary { binary, .. } => new_numeric_compound_assign(
            f,
            binary,
            target,
            target_kind,
            target_type,
            CompoundEvalMode::EvaluatesToNewValue,
        ),
        IncDec::Operator { method, .. } => new_user_defined_increment(
            f,
            method,
            target,
            target_kind,
            CompoundEvalMode::EvaluatesToNewValue,
        ),
    };
    let new_store = new_stloc(f, outer_var, compound);
    f.set_child_at(block, pos, new_store);
    true
}

/// `TransformPostIncDecOperatorWithInlineStore`:
/// `stobj(target, binary.add(stloc l(ldobj(target)), ldc.i4 1))` becomes
/// `stloc l(compound.op.old(target, ldc.i4 1))`.
fn transform_post_inc_dec_operator_with_inline_store(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let store = f.list(block)[pos];
    let Some((mut target_type, value)) = is_compound_store(f, ctx, store) else {
        return false;
    };
    let Some(inc_dec) = match_inc_dec(f, ctx, value, &mut target_type) else {
        return false;
    };
    let operand = match &inc_dec {
        IncDec::Binary { operand, .. } | IncDec::Operator { operand, .. } => *operand,
    };
    let Some((v, stored)) = f.match_stloc(operand) else {
        return false;
    };
    if !matches!(f.var(v).kind, VariableKind::Local | VariableKind::StackSlot) {
        return false;
    }
    let Some((target, target_kind, finalize)) =
        is_matching_compound_load(f, ctx, stored, store, Some(v), None)
    else {
        return false;
    };
    let var_type = f.var(v).ty;
    if is_implicit_truncation(f, ctx, stored, var_type, false) {
        return false;
    }
    finalize.apply(f, ctx);
    let compound = match inc_dec {
        IncDec::Binary { binary, .. } => new_numeric_compound_assign(
            f,
            binary,
            target,
            target_kind,
            target_type,
            CompoundEvalMode::EvaluatesToOldValue,
        ),
        IncDec::Operator { method, .. } => new_user_defined_increment(
            f,
            method,
            target,
            target_kind,
            CompoundEvalMode::EvaluatesToOldValue,
        ),
    };
    let new_store = new_stloc(f, v, compound);
    f.set_child_at(block, pos, new_store);
    true
}

/// `TransformPostIncDecOperator`: `stloc tmp(ldobj(target)); stobj(target,
/// binary.add(ldloc tmp, ldc.i4 1))` becomes `stloc tmp(compound.op.old(target, ldc.i4 1))`
/// (or just the compound assignment when `tmp` is unused).
fn transform_post_inc_dec_operator(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    let list = f.list(block);
    let inst = list[i];
    let (Some((tmp, inst_value)), Some(&store)) = (f.match_stloc(inst), list.get(i + 1)) else {
        return false;
    };
    let previous = i.checked_sub(1).map(|p| list[p]);
    let Some((mut target_type, value)) = is_compound_store(f, ctx, store) else {
        return false;
    };
    let truncation = check_implicit_truncation(f, ctx, inst_value, target_type, false);
    if truncation == Truncation::ValueChanged {
        // `stloc tmp` truncates the value.
        return false;
    }
    if truncation == Truncation::ValueChangedDueToSignMismatch {
        // Only a stobj of exactly the target type can change its sign.
        let stobj_of_target =
            matches!(*f.inst(store), Inst::StObj { ty, .. } if ty.ty == target_type);
        if !stobj_of_target {
            return false;
        }
    }
    let Some((target, target_kind, finalize)) =
        is_matching_compound_load(f, ctx, inst_value, store, Some(tmp), previous)
    else {
        return false;
    };
    let (unwrapped, conv) = unwrap_small_integer_conv(f, value);
    let compound = if let Inst::BinaryNumeric {
        op,
        check_overflow,
        left,
        right,
        ..
    } = *f.inst(unwrapped)
    {
        if !matches!(op, BinaryNumericOperator::Add | BinaryNumericOperator::Sub) {
            return false;
        }
        if f.match_ldloc(left) != Some(tmp) {
            return false;
        }
        if let TypeData::Pointer(element) = *ctx.ts.data(target_type) {
            let one = match pointer_offset_detect(f, ctx, right, element, check_overflow, false) {
                Some(PointerOffset::Inst(n)) => f.match_ldc_i(n) == Some(1),
                Some(PointerOffset::Constant(c)) => c == 1,
                None => false,
            };
            if !one {
                return false;
            }
        } else if !is_one(f, right) {
            return false;
        }
        if truncation == Truncation::ValueChangedDueToSignMismatch {
            // The sign change skips the implicit truncation.
            target_type = swap_sign(ctx, target_type);
            if let Inst::StObj { ty, .. } = f.inst_mut(store) {
                ty.ty = target_type;
            }
        }
        if !validate_compound_assign(f, ctx, unwrapped, conv, target_type) {
            return false;
        }
        finalize.apply(f, ctx);
        new_numeric_compound_assign(
            f,
            unwrapped,
            target,
            target_kind,
            target_type,
            CompoundEvalMode::EvaluatesToOldValue,
        )
    } else if let Inst::Call(c) = f.inst(value)
        && c.args.len() == 1
        && ctx.ts.method_is_operator(&c.method.method.clone())
    {
        let (method, operand) = (c.method.clone(), c.args[0]);
        if f.match_ldloc(operand) != Some(tmp) {
            return false;
        }
        if !is_increment_or_decrement(ctx, &method.method) {
            return false;
        }
        if method.lifted {
            return false;
        }
        finalize.apply(f, ctx);
        new_user_defined_increment(
            f,
            method,
            target,
            target_kind,
            CompoundEvalMode::EvaluatesToOldValue,
        )
    } else {
        return false;
    };
    f.set_child_at(inst, 0, compound);
    f.remove_at(block, i + 1);
    let var = f.var(tmp);
    if var.is_single_definition() && var.load_count() == 0 {
        // A dead store: a statement-level post-increment.
        f.replace(inst, compound);
    }
    true
}

/// `TransformInlineAssignmentLocal`: `stloc s(value); stloc l(ldloc s)` without truncation
/// becomes `stloc s(stloc l(value))`.
fn transform_inline_assignment_local(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let list = f.list(block);
    let (Some(&inst), Some(&next)) = (list.get(pos), list.get(pos + 1)) else {
        return false;
    };
    let (Some((stack_var, value)), Some((var, next_value))) =
        (f.match_stloc(inst), f.match_stloc(next))
    else {
        return false;
    };
    if f.var(stack_var).kind != VariableKind::StackSlot {
        return false;
    }
    if !matches!(
        f.var(var).kind,
        VariableKind::Local | VariableKind::Parameter
    ) {
        return false;
    }
    if f.match_ldloc(next_value) != Some(stack_var) {
        return false;
    }
    let (stack_type, var_type) = (f.var(stack_var).ty, f.var(var).ty);
    if is_implicit_truncation(f, ctx, value, stack_type, false)
        || is_implicit_truncation(f, ctx, value, var_type, false)
    {
        return false;
    }
    // Ref locals must be initialized where they are declared, which this transform cannot
    // ensure.
    if f.var(var).stack_type == StackType::Ref {
        return false;
    }
    f.remove_at(block, pos);
    let inner = new_stloc(f, var, value);
    let inline_assignment = new_stloc(f, stack_var, inner);
    f.replace(next, inline_assignment);
    true
}

/// The index of `inst` in the list of `block` (`Instructions.Remove`).
fn remove_from_block(f: &mut Function, block: NodeId, inst: NodeId) {
    if let Some(index) = f.list(block).iter().position(|&i| i == inst) {
        f.remove_at(block, index);
    }
}

/// `TransformInlineAssignmentStObjOrCall`: `stloc s(value); [stloc l(ldloc s);]
/// stobj(target, ldloc s)` becomes `stloc l(stobj(target, value))`, and a setter call
/// `set_P(..., ldloc s)` becomes `stloc l(Block CallInlineAssign { set_P(..., stloc i(value));
/// final: ldloc i })`.
fn transform_inline_assignment_stobj_or_call(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
) -> bool {
    let f = &mut r.function;
    let list = f.list(block).to_vec();
    let Some((s, value)) = list.get(pos).and_then(|&i| f.match_stloc(i)) else {
        return false;
    };
    let inst = list[pos];
    // It can be a compiler-generated local.
    if !matches!(f.var(s).kind, VariableKind::StackSlot | VariableKind::Local) {
        return false;
    }
    let s_type = f.var(s).ty;
    if is_implicit_truncation(f, ctx, value, s_type, false) {
        // `stloc s` truncates.
        return false;
    }
    let Some(&next1) = list.get(pos + 1) else {
        return false;
    };
    let (local, local_store, next_pos) = if let Some((l, l_value)) = f.match_stloc(next1) {
        // With an extra local, `s` goes away: it has no other uses.
        if f.var(l).kind != VariableKind::Local || f.match_ldloc(l_value) != Some(s) {
            return false;
        }
        if !(f.var(s).is_single_definition() && f.var(s).load_count() == 2) {
            return false;
        }
        (l, Some(next1), pos + 2)
    } else {
        // An inline assignment would leave a dead store.
        if f.var(s).load_count() == 1 && f.var(s).address_count() == 0 {
            return false;
        }
        (s, None, pos + 1)
    };
    let Some(&next) = list.get(next_pos) else {
        return false;
    };
    match *f.inst(next) {
        Inst::StObj {
            target,
            value: stored,
            ty,
            unaligned,
            ..
        } => {
            // unaligned.stobj has no C# form.
            if unaligned > 0 || f.match_ldloc(stored) != Some(s) {
                return false;
            }
            if !is_pure(f.flags(target)) || f.is_used_within(s, target) {
                return false;
            }
            let pointer_type = f.infer_type(ctx.ts, target);
            let mut new_type = ty.ty;
            if let TypeData::ByRef(element) | TypeData::Pointer(element) =
                *ctx.ts.data(pointer_type)
                && ctx.ts.is_compatible_type_for_memory_access(element, ty.ty)
            {
                new_type = element;
            }
            match check_implicit_truncation(f, ctx, value, new_type, false) {
                // stobj truncates.
                Truncation::ValueChanged => return false,
                // The opposite sign skips the truncation.
                Truncation::ValueChangedDueToSignMismatch => new_type = swap_sign(ctx, new_type),
                Truncation::ValuePreserved => {}
            }
            if let Inst::StObj { ty, .. } = f.inst_mut(next) {
                ty.ty = new_type;
            }
            if let Some(ls) = local_store {
                remove_from_block(f, block, ls);
            }
            remove_from_block(f, block, next);
            f.set_child_at(next, 1, value);
            let inline_assignment = new_stloc(f, local, next);
            f.replace(inst, inline_assignment);
            // The caller's rerun runs HandleCompoundAssign on the stobj where it applies.
            true
        }
        Inst::Call(_) | Inst::CallVirt(_) => {
            let (call_method, args) = match f.inst(next) {
                Inst::Call(c) | Inst::CallVirt(c) => (c.method.method.clone(), c.args.clone()),
                _ => unreachable!("a call"),
            };
            if f.result_type(ctx.ts, next) != StackType::Void || args.is_empty() {
                return false;
            }
            if !is_property_setter(ctx, &call_method) {
                return false;
            }
            let params = call_method.params().unwrap_or_default();
            let is_indexer = match call_method.origin {
                MethodOrigin::Def(id) => {
                    let (owner, _) = property_accessor(ctx, &call_method).expect("a setter");
                    ctx.ts
                        .property_symbol_kind(id.module, owner & 0x00FF_FFFF)
                        .ok()
                        == Some(crate::typesystem::members::SymbolKind::Indexer)
                }
                // A guessed fake property is an indexer when the setter has more parameters.
                _ => params.len() > 1,
            };
            // A parameterized property has no C# property syntax, so no expression form.
            if !(is_indexer || params.len() == 1) {
                return false;
            }
            let (&last, rest) = args.split_last().expect("arguments");
            if f.match_ldloc(last) != Some(s) {
                return false;
            }
            for &arg in rest {
                if !is_pure(f.flags(arg)) || f.is_used_within(s, arg) {
                    return false;
                }
            }
            let value_type = *params.last().expect("a setter parameter");
            if is_implicit_truncation(f, ctx, value, value_type, false) {
                // The setter call truncates.
                return false;
            }
            if let Some(ls) = local_store {
                remove_from_block(f, block, ls);
            }
            remove_from_block(f, block, next);
            let new_var = r.register_variable(ctx, VariableKind::StackSlot, value_type);
            let f = &mut r.function;
            let inner = new_stloc(f, new_var, value);
            f.set_child_at(next, args.len() - 1, inner);
            let load = f.add(Inst::LdLoc { var: new_var });
            let inline_block = f.add(Inst::Block {
                kind: BlockKind::CallInlineAssign,
                instructions: vec![next],
                final_inst: load,
            });
            let inline_assignment = new_stloc(f, local, inline_block);
            f.replace(inst, inline_assignment);
            // ExpressionTransforms does not look into inline blocks.
            handle_compound_assign(f, ctx, next);
            true
        }
        _ => false,
    }
}
