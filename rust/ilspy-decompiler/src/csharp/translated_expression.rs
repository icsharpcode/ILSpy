//! `TranslatedExpression`: a C# expression with the type of its resolve result.

use crate::semantics::types::TypeKind;
use crate::semantics::{Conversions, ResolveResult};
use crate::typesystem::fnptr::Normalization;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::ConstValue;
use crate::typesystem::types::{TypeData, TypeId};

use super::ast::{AstId, AstNode, Rr};
use super::{Builder, Unsupported};

/// An expression the builders produced, with `ResolveResult.Type`.
#[derive(Clone, Copy, Debug)]
pub struct TranslatedExpression {
    pub expression: AstId,
    pub ty: TypeId,
}

/// `TypeCode` values (`ReflectionHelper.GetTypeCode`).
mod code {
    pub const BOOLEAN: u8 = 3;
    pub const CHAR: u8 = 4;
    pub const UINT64: u8 = 12;
    pub const SINGLE: u8 = 13;
    pub const DOUBLE: u8 = 14;
    pub const DECIMAL: u8 = 15;
    pub const STRING: u8 = 18;
}

/// What `CSharpResolver.ResolveCast` gives for a constant.
enum CastResult {
    /// A `ConstantResolveResult` of the target type.
    Constant(ConstValue),
    /// An `ErrorResolveResult`.
    Error,
    /// A `ConversionResolveResult`: the cast is no compile-time constant.
    Conversion,
}

impl Builder<'_, '_, '_, '_> {
    /// The resolve result of an expression the builders made, as the C# semantics see it.
    pub(crate) fn semantic_rr(&self, e: AstId, ty: TypeId) -> ResolveResult {
        match &self.ast.annotations(e).rr {
            Some(Rr::Constant { ty, value }) => ResolveResult::Constant {
                ty: *ty,
                value: value.clone(),
            },
            // `ByReferenceResolveResult` of a `ref` or `out` expression.
            Some(rr @ (Rr::ByReference(_) | Rr::ByReferenceOut(_))) => match *self.ts.data(ty) {
                TypeData::ByRef(element) => ResolveResult::ByReference {
                    ty,
                    element,
                    ref_kind: if matches!(rr, Rr::ByReferenceOut(_)) {
                        crate::typesystem::members::RefKind::Out
                    } else {
                        crate::typesystem::members::RefKind::Ref
                    },
                },
                _ => ResolveResult::Expression(ty),
            },
            Some(Rr::Lambda(index)) => self.lambda_rr(*index),
            Some(Rr::MethodGroup { index, .. }) => {
                ResolveResult::MethodGroup(self.ast.method_groups[*index].clone())
            }
            Some(Rr::FormatString { ty, .. }) => ResolveResult::InterpolatedString(*ty),
            Some(Rr::DefaultLiteral { shortened_from }) => {
                ResolveResult::DefaultLiteral(*shortened_from)
            }
            _ => ResolveResult::Expression(ty),
        }
    }

    /// `Unwrapped`: an unwrapped default literal takes its value from the target type, which
    /// may have to be spelled out again.
    fn unwrapped(
        &mut self,
        e: TranslatedExpression,
        target: TypeId,
        allow_implicit_conversion: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        if matches!(self.rr_of(e.expression), Rr::DefaultLiteral { .. }) {
            return self.convert_to(e, target, allow_implicit_conversion);
        }
        Ok(e)
    }

    /// `RestoreDefaultLiteralType`: `default(T)` again for a `default` literal, with its
    /// instructions.
    pub(crate) fn restore_default_literal_type(
        &mut self,
        e: TranslatedExpression,
    ) -> Result<TranslatedExpression, Unsupported> {
        let Rr::DefaultLiteral { shortened_from } = self.rr_of(e.expression) else {
            return Ok(e);
        };
        let restored = self.default_value_expression(shortened_from)?;
        for inst in self.ast.annotations(e.expression).il.clone() {
            self.ast.with_il(restored.expression, inst);
        }
        Ok(restored)
    }

    /// `TranslatedExpression.ConvertTo(targetType, expressionBuilder, checkForOverflow: false,
    /// allowImplicitConversion)` for the expressions the builders make: unchanged when the
    /// types are equal up to nullability and tuple names, when the target is `void`, `?` or
    /// unknown with the same name, or (with `allow_implicit_conversion`) when an implicit
    /// conversion exists; a constant is folded into the target type. A body that needs a cast
    /// is not translated.
    pub(crate) fn convert_to(
        &mut self,
        e: TranslatedExpression,
        target: TypeId,
        allow_implicit_conversion: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
        if let Rr::DefaultLiteral { shortened_from } = self.rr_of(e.expression) {
            // The context still supplies the type the literal was shortened from.
            if allow_implicit_conversion
                && self.ts.normalize_with(shortened_from, n) == self.ts.normalize_with(target, n)
            {
                return Ok(e);
            }
            let restored = self.restore_default_literal_type(e)?;
            return self.convert_to(restored, target, allow_implicit_conversion);
        }
        let ty = e.ty;
        let same = self.ts.normalize_with(ty, n) == self.ts.normalize_with(target, n);
        // The target type supplied by the context shortens `default(T)` to `default`.
        if same
            && allow_implicit_conversion
            && self.settings.default_literals
            && matches!(
                self.ast.node(e.expression),
                AstNode::DefaultValue { ty: Some(_) }
            )
        {
            *self.ast.node_mut(e.expression) = AstNode::DefaultValue { ty: None };
            let a = self.ast.annotations_mut(e.expression);
            a.il_before_rr = !a.il.is_empty();
            a.rr = Some(Rr::DefaultLiteral { shortened_from: ty });
            let no_type = self.ts.intern(TypeData::NoType);
            return Ok(TranslatedExpression {
                expression: e.expression,
                ty: no_type,
            });
        }
        // `UseImplicitMethodGroupConversion`: `new D(target.M)` is written as `target.M`.
        if same
            && allow_implicit_conversion
            && self.settings.use_implicit_method_group_conversion
            && let AstNode::DelegateCreate { reference, .. } = *self.ast.node(e.expression)
        {
            return Ok(self.unwrap_child(e.expression, reference));
        }
        // A cast the engine made: `CastCanBeMadeImplicit` may unwrap it again.
        if let AstNode::Cast {
            expression: inner, ..
        } = *self.ast.node(e.expression)
        {
            let input = self.rr_of(inner);
            let input_ty = self.rr_type(&input);
            let input_rr = self.semantic_rr(inner, input_ty);
            // A lambda's cast carries `LambdaConversion` in its resolve result.
            let conversion = if matches!(self.rr_of(e.expression), Rr::LambdaConversion { .. }) {
                crate::semantics::Conversion::ANONYMOUS_FUNCTION
            } else {
                Conversions::new(self.ts).explicit(&input_rr, ty)
            };
            // The cast of an implicit conversion operator carries that user-defined conversion,
            // which `CastCanBeMadeImplicit` drops where the target type is identical.
            let operator = matches!(self.rr_of(e.expression), Rr::ImplicitConversion { .. });
            // Its conversion is the implicit one from the operand's type (`HandleImplicitConversion`).
            let conversion = if operator {
                Conversions::new(self.ts).implicit_type(input_ty, ty)
            } else {
                conversion
            };
            let user_defined = conversion.is_user_defined();
            let implicit = self.cast_can_be_made_implicit(&conversion, input_ty, ty, target)?;
            if same {
                if allow_implicit_conversion && implicit {
                    let cast_rr = self.rr_of(e.expression);
                    let unwrapped = self.unwrap_child(e.expression, inner);
                    // The dropped user-defined conversion is remembered, for a later cast.
                    if user_defined {
                        self.ast
                            .annotations_mut(unwrapped.expression)
                            .implicit_conversion = Some((ty, cast_rr));
                    }
                    return self.unwrapped(unwrapped, target, allow_implicit_conversion);
                }
                return Ok(e);
            }
            // Between different types a user-defined conversion keeps its cast.
            if implicit && !user_defined {
                let unwrapped = self.unwrap_child(e.expression, inner);
                if allow_implicit_conversion {
                    return self.unwrapped(unwrapped, target, allow_implicit_conversion);
                }
                return self.convert_to(unwrapped, target, allow_implicit_conversion);
            }
        }
        if same {
            return Ok(e);
        }
        match self.ts.kind(target) {
            TypeKind::Void | TypeKind::None => return Ok(e),
            TypeKind::Unknown
                if matches!(self.ts.data(target), TypeData::UnknownSpecial)
                    || self.ts.reflection_name(target) == self.ts.reflection_name(ty) =>
            {
                return Ok(e);
            }
            _ => {}
        }
        // A user-defined conversion dropped from the expression is cast again before any
        // other conversion.
        if let Some((cast_type, cast_rr)) = self
            .ast
            .annotations_mut(e.expression)
            .implicit_conversion
            .take()
        {
            let c = self.ast.add(AstNode::Cast {
                ty: cast_type,
                expression: e.expression,
                unchecked: false,
            });
            self.ast.with_rr(c, cast_rr);
            let cast = TranslatedExpression {
                expression: c,
                ty: cast_type,
            };
            return self.convert_to(cast, target, allow_implicit_conversion);
        }
        let utype = self.ts.nullable_underlying(ty);
        let target_utype = self.ts.nullable_underlying(target);
        let target_ustack = self.ts.stack_type(target_utype);
        // The special cases of the managed conversion: bool to and from integers, native
        // integers, enums or chars and pointers, references.
        if (self.ts.is_known(ty, KnownTypeCode::Boolean)
            && !self.ts.is_known(target_utype, KnownTypeCode::Boolean)
            && target_ustack.is_integer())
            || self.ts.is_known(target, KnownTypeCode::Boolean)
            || self.ts.is_known(ty, KnownTypeCode::IntPtr)
            || self.ts.is_known(ty, KnownTypeCode::UIntPtr)
            || ((self.ts.is_known(target_utype, KnownTypeCode::IntPtr)
                || self.ts.is_known(target_utype, KnownTypeCode::UIntPtr))
                && self.ts.stack_type(utype).is_integer())
            || self.ts.kind(target).is_any_pointer()
            || self.ts.kind(ty).is_any_pointer()
            || self.ts.kind(target) == TypeKind::ByReference
        {
            return Err("a conversion that needs a cast".to_owned());
        }
        let rr = self.semantic_rr(e.expression, ty);
        if let ResolveResult::Constant { value, .. } = &rr
            && !matches!(value, ConstValue::Null)
            && self.ts.is_nullable(target)
            && utype != target_utype
            && target_ustack.is_integer()
        {
            return Err("a constant converted to a nullable integer".to_owned());
        }
        // `resolver.ResolveCast(targetType, ResolveResult)`
        let cast = self.resolve_cast(target, &rr)?;
        match cast {
            CastResult::Constant(value) => {
                // `ConvertConstantValue(rr, allowImplicitConversion).WithILInstruction(...)`
                let il = self.ast.annotations(e.expression).il.clone();
                let c = self.ast.add(AstNode::ConstantValue {
                    ty: target,
                    value: value.clone(),
                    allow_implicit_conversion,
                });
                let (ty, value) =
                    self.constant_value_rr(target, value, allow_implicit_conversion)?;
                self.ast.with_rr(c, Rr::Constant { ty, value });
                for inst in il {
                    self.ast.with_il(c, inst);
                }
                return Ok(TranslatedExpression { expression: c, ty });
            }
            CastResult::Error => {
                // Between unrelated reference types the conversion goes through object.
                if self.ts.is_reference_type(target) == Some(true)
                    && self.ts.is_reference_type(ty) == Some(true)
                    && !self.ts.is_known(target, KnownTypeCode::Object)
                    && !self.ts.is_known(ty, KnownTypeCode::Object)
                {
                    let object = self.ts.known_type(KnownTypeCode::Object);
                    let via = self.convert_to(e, object, false)?;
                    return self.convert_to(via, target, allow_implicit_conversion);
                }
            }
            CastResult::Conversion => {
                if self.ts.kind(ty) == TypeKind::Dynamic
                    && self.ts.is_reference_type(target) == Some(true)
                    && !self.ts.is_known(target, KnownTypeCode::Object)
                {
                    return Err("a conversion from dynamic".to_owned());
                }
            }
        }
        if allow_implicit_conversion && Conversions::new(self.ts).implicit(&rr, target).is_valid() {
            return Ok(e);
        }
        // The cast, with the resolve result of `ResolveCast`, and an unchecked annotation for
        // an integer target.
        if matches!(self.ast.node(e.expression), AstNode::Base) {
            return Err("a cast of base".to_owned());
        }
        if matches!(rr, ResolveResult::Constant { .. })
            && matches!(self.ts.kind(target_utype), TypeKind::NInt | TypeKind::NUInt)
        {
            return Err("a constant cast to a native integer".to_owned());
        }
        let c = self.ast.add(AstNode::Cast {
            ty: target,
            expression: e.expression,
            unchecked: target_ustack.is_integer(),
        });
        self.ast.with_rr(c, Rr::Conversion { ty: target });
        Ok(TranslatedExpression {
            expression: c,
            ty: target,
        })
    }

    /// `CSharpResolver.ResolveCast(targetType, expression)` as far as `ConvertTo` reads it.
    fn resolve_cast(
        &mut self,
        target: TypeId,
        rr: &ResolveResult,
    ) -> Result<CastResult, Unsupported> {
        let ResolveResult::Constant { value, .. } = rr else {
            // A `ConversionResolveResult`, which is an error without an explicit conversion.
            return Ok(
                if Conversions::new(self.ts).explicit(rr, target).is_valid() {
                    CastResult::Conversion
                } else {
                    CastResult::Error
                },
            );
        };
        let c = Conversions::new(self.ts).explicit(rr, target);
        if c.is_user_defined() {
            return Ok(CastResult::Conversion);
        }
        let underlying = self.enum_underlying(target);
        let code = self.ts.type_code(underlying);
        if (code::BOOLEAN..=code::DECIMAL).contains(&code) && !matches!(value, ConstValue::Null) {
            if matches!(value, ConstValue::String(_)) {
                return Ok(CastResult::Error);
            }
            return primitive_cast(code, value).map(|v| match v {
                Some(v) => CastResult::Constant(v),
                None => CastResult::Error,
            });
        }
        if code == code::STRING {
            return Ok(match value {
                ConstValue::Null | ConstValue::String(_) => CastResult::Constant(value.clone()),
                _ => CastResult::Error,
            });
        }
        if matches!(self.ts.kind(underlying), TypeKind::NInt | TypeKind::NUInt)
            && !matches!(value, ConstValue::Null)
        {
            return Err("a constant converted to a native integer".to_owned());
        }
        Ok(CastResult::Conversion)
    }

    /// `TranslatedExpression.CastCanBeMadeImplicit`: whether the cast of `input` to `old` with
    /// `conversion` can be left out when the result is converted to `new` anyway.
    fn cast_can_be_made_implicit(
        &mut self,
        conversion: &crate::semantics::Conversion,
        input: TypeId,
        old: TypeId,
        new: TypeId,
    ) -> Result<bool, Unsupported> {
        // If the cast was required for the old conversion, it stays.
        if !conversion.is_implicit() {
            return Ok(false);
        }
        // nint has an identity conversion with IntPtr, but other implicit conversions.
        let native = |b: &Self, t: TypeId| matches!(b.ts.kind(t), TypeKind::NInt | TypeKind::NUInt);
        if native(self, old) || native(self, new) {
            return Ok(false);
        }
        if conversion.is_interpolated_string() || conversion.is_implicit_span() {
            return Err("a cast of an interpolated string or span".to_owned());
        }
        let mut conversions = Conversions::new(self.ts);
        if conversion.is_boxing() {
            return Ok(conversions.is_boxing_or_involving_type_parameter(input, new));
        }
        Ok(conversions.identity(old, new))
    }

    /// The resolve result `ExpressionBuilder.ConvertConstantValue(new ConstantResolveResult(ty,
    /// value), allow_implicit_conversion)` gives its expression, which the host builds for a
    /// `ConstantValue` node: the annotation `TypeSystemAstBuilder.ConvertConstantValue` adds,
    /// unless a cast goes around the literal. A null of a reference, nullable or pointer type
    /// is the null literal (`NullType`), and a small integer an `int` literal; enum members and
    /// other literals keep the constant's type.
    pub(crate) fn constant_value_rr(
        &mut self,
        ty: TypeId,
        value: ConstValue,
        allow_implicit_conversion: bool,
    ) -> Result<(TypeId, ConstValue), Unsupported> {
        let underlying = self.ts.nullable_underlying(ty);
        if matches!(value, ConstValue::Null) {
            let literal = (allow_implicit_conversion || self.ts.kind(ty) == TypeKind::Null)
                && (self.ts.is_reference_type(ty) == Some(true)
                    || self.ts.is_known(ty, KnownTypeCode::NullableOfT)
                    || self.ts.kind(ty).is_any_pointer());
            return Ok(if literal {
                (self.ts.intern(TypeData::Null), value)
            } else {
                (ty, value)
            });
        }
        // SByte, Byte, Int16, UInt16 outside of enums.
        if allow_implicit_conversion
            && self.ts.kind(underlying) != TypeKind::Enum
            && (5..=8).contains(&self.ts.type_code(underlying))
        {
            // `IsSpecialConstant` writes the type's MinValue or MaxValue as a member reference.
            if matches!(
                value,
                ConstValue::SByte(i8::MIN | i8::MAX)
                    | ConstValue::Byte(u8::MAX)
                    | ConstValue::Int16(i16::MIN | i16::MAX)
                    | ConstValue::UInt16(u16::MAX)
            ) {
                return Err("a small integer constant written as MinValue or MaxValue".to_owned());
            }
            let v = integral_value(&value).ok_or("a small integer constant of another kind")?;
            let int = self.ts.known_type(KnownTypeCode::Int32);
            return Ok((int, ConstValue::Int32(v as i32)));
        }
        Ok((ty, value))
    }

    /// `resolver.WithCheckForOverflow(checked).ResolveCast(target, constant)` where the result
    /// is a constant: its value, or `None` for no constant (a conversion, or an error such as
    /// the overflow a checked narrowing reports).
    pub(crate) fn resolve_constant_cast(
        &mut self,
        target: TypeId,
        ty: TypeId,
        value: &ConstValue,
        checked: bool,
    ) -> Result<Option<ConstValue>, Unsupported> {
        let rr = ResolveResult::Constant {
            ty,
            value: value.clone(),
        };
        let CastResult::Constant(result) = self.resolve_cast(target, &rr)? else {
            return Ok(None);
        };
        if checked {
            // `CSharpPrimitiveCast` in a checked context throws where the value changes.
            match (integral_value(value), integral_value(&result)) {
                (Some(a), Some(b)) if a == b => {}
                (Some(_), Some(_)) => return Ok(None),
                _ if result == *value => {}
                _ => return Err("a checked constant cast of a non-integral value".to_owned()),
            }
        }
        Ok(Some(result))
    }

    /// `IType.GetEnumUnderlyingType()`.
    pub(crate) fn enum_underlying(&mut self, t: TypeId) -> TypeId {
        let underlying = self
            .ts
            .definition(t)
            .and_then(|d| self.ts.comp.type_def(d).ok())
            .filter(|i| i.kind == crate::typesystem::compilation::TypeKind::Enum)
            .and_then(|i| i.enum_underlying);
        match underlying {
            Some(code) => self.ts.known_type(code),
            None => t,
        }
    }
}

/// The numeric value of an integral or char constant.
pub(crate) fn integral_value(value: &ConstValue) -> Option<i128> {
    Some(match *value {
        ConstValue::Char(c) => i128::from(c),
        ConstValue::SByte(v) => i128::from(v),
        ConstValue::Byte(v) => i128::from(v),
        ConstValue::Int16(v) => i128::from(v),
        ConstValue::UInt16(v) => i128::from(v),
        ConstValue::Int32(v) => i128::from(v),
        ConstValue::UInt32(v) => i128::from(v),
        ConstValue::Int64(v) => i128::from(v),
        ConstValue::UInt64(v) => i128::from(v),
        _ => return None,
    })
}

/// `CSharpPrimitiveCast.Cast(targetType, input, checkForOverflow: false)` for integral, char
/// and bool constants; `None` where the managed cast throws (an `ErrorResolveResult`).
/// Floating point and decimal values are not translated yet.
pub(crate) fn primitive_cast(
    target: u8,
    value: &ConstValue,
) -> Result<Option<ConstValue>, Unsupported> {
    let input: i128 = match *value {
        ConstValue::Boolean(b) => {
            return Ok((target == code::BOOLEAN).then_some(ConstValue::Boolean(b)));
        }
        ConstValue::Char(c) => i128::from(c),
        ConstValue::SByte(v) => i128::from(v),
        ConstValue::Byte(v) => i128::from(v),
        ConstValue::Int16(v) => i128::from(v),
        ConstValue::UInt16(v) => i128::from(v),
        ConstValue::Int32(v) => i128::from(v),
        ConstValue::UInt32(v) => i128::from(v),
        ConstValue::Int64(v) => i128::from(v),
        ConstValue::UInt64(v) => i128::from(v),
        _ => return Err("a floating point or decimal constant cast".to_owned()),
    };
    // Unchecked: truncated to the target's width.
    Ok(Some(match target {
        code::BOOLEAN => return Ok(None),
        code::CHAR => ConstValue::Char(input as u16),
        5 => ConstValue::SByte(input as i8),
        6 => ConstValue::Byte(input as u8),
        7 => ConstValue::Int16(input as i16),
        8 => ConstValue::UInt16(input as u16),
        9 => ConstValue::Int32(input as i32),
        10 => ConstValue::UInt32(input as u32),
        11 => ConstValue::Int64(input as i64),
        code::UINT64 => ConstValue::UInt64(input as u64),
        code::SINGLE => ConstValue::Single(if input < 0 {
            (input as i64 as f32).to_bits()
        } else {
            (input as u64 as f32).to_bits()
        }),
        code::DOUBLE => ConstValue::Double(if input < 0 {
            (input as i64 as f64).to_bits()
        } else {
            (input as u64 as f64).to_bits()
        }),
        _ => return Err("a constant cast to decimal".to_owned()),
    }))
}
