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
            _ => ResolveResult::Expression(ty),
        }
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
        let ty = e.ty;
        let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
        if self.ts.normalize_with(ty, n) == self.ts.normalize_with(target, n) {
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
                self.ast.with_rr(c, Rr::Constant { ty: target, value });
                for inst in il {
                    self.ast.with_il(c, inst);
                }
                return Ok(TranslatedExpression {
                    expression: c,
                    ty: target,
                });
            }
            CastResult::Error => {
                if self.ts.is_reference_type(target) == Some(true)
                    && self.ts.is_reference_type(ty) == Some(true)
                {
                    return Err("a conversion through object".to_owned());
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
        Err("a conversion that needs a cast".to_owned())
    }

    /// `CSharpResolver.ResolveCast(targetType, expression)` as far as `ConvertTo` reads it.
    fn resolve_cast(
        &mut self,
        target: TypeId,
        rr: &ResolveResult,
    ) -> Result<CastResult, Unsupported> {
        let ResolveResult::Constant { value, .. } = rr else {
            return Ok(CastResult::Conversion);
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

    /// `IType.GetEnumUnderlyingType()`.
    fn enum_underlying(&mut self, t: TypeId) -> TypeId {
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

/// `CSharpPrimitiveCast.Cast(targetType, input, checkForOverflow: false)` for integral, char
/// and bool constants; `None` where the managed cast throws (an `ErrorResolveResult`).
/// Floating point and decimal values are not translated yet.
fn primitive_cast(target: u8, value: &ConstValue) -> Result<Option<ConstValue>, Unsupported> {
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
