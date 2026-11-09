//! The C# conversions the IL transforms ask the managed `CSharpConversions` about.
//!
//! shortcut: these approximate `CSharpConversions.ImplicitConversion` for the operand types
//! the transforms see (identity, numeric, nullable, reference and boxing conversions); port
//! the C# conversions with the semantics step (item 67).

use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::il_inlining::known;
use super::transform_assignment::nullable_underlying;

/// The kind of a valid implicit conversion (`Conversion.IsIdentityConversion` and so on).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub(crate) enum ImplicitConversion {
    Identity,
    Numeric,
    Nullable,
    Reference,
    Boxing,
    Dynamic,
}

/// `NormalizeTypeVisitor.TypeErasure.EquivalentTypes`.
pub(crate) fn type_erasure_equivalent(ctx: &mut Context<'_, '_, '_>, a: TypeId, b: TypeId) -> bool {
    ctx.ts.normalize(a) == ctx.ts.normalize(b)
}

fn is_nullable(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    known(ctx, t) == Some(KnownTypeCode::NullableOfT)
}

fn underlying(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    if is_nullable(ctx, t) {
        nullable_underlying(ctx, t)
    } else {
        t
    }
}

/// `CSharpConversions.ImplicitConversion(from, to)`: the kind, None when there is none.
pub(crate) fn implicit_conversion(
    ctx: &mut Context<'_, '_, '_>,
    from: TypeId,
    to: TypeId,
) -> Option<ImplicitConversion> {
    if type_erasure_equivalent(ctx, from, to) {
        return Some(ImplicitConversion::Identity);
    }
    if implicit_numeric(ctx, from, to) {
        return Some(ImplicitConversion::Numeric);
    }
    if is_nullable(ctx, to) {
        let to_underlying = underlying(ctx, to);
        let from_underlying = underlying(ctx, from);
        if type_erasure_equivalent(ctx, from_underlying, to_underlying)
            || implicit_numeric(ctx, from_underlying, to_underlying)
        {
            return Some(ImplicitConversion::Nullable);
        }
    }
    if is_implicit_reference_conversion(ctx, from, to) {
        return Some(ImplicitConversion::Reference);
    }
    if is_boxing_conversion(ctx, from, to) {
        return Some(ImplicitConversion::Boxing);
    }
    if matches!(ctx.ts.data(to), TypeData::Dynamic) {
        return Some(ImplicitConversion::Dynamic);
    }
    None
}

/// The implicit numeric conversions (C# 10.2.3).
fn implicit_numeric(ctx: &mut Context<'_, '_, '_>, from: TypeId, to: TypeId) -> bool {
    use KnownTypeCode as K;
    let (Some(f), Some(t)) = (known(ctx, from), known(ctx, to)) else {
        return false;
    };
    let targets: &[K] = match f {
        K::SByte => &[
            K::Int16,
            K::Int32,
            K::Int64,
            K::Single,
            K::Double,
            K::Decimal,
        ],
        K::Byte => &[
            K::Int16,
            K::UInt16,
            K::Int32,
            K::UInt32,
            K::Int64,
            K::UInt64,
            K::Single,
            K::Double,
            K::Decimal,
        ],
        K::Int16 => &[K::Int32, K::Int64, K::Single, K::Double, K::Decimal],
        K::UInt16 => &[
            K::Int32,
            K::UInt32,
            K::Int64,
            K::UInt64,
            K::Single,
            K::Double,
            K::Decimal,
        ],
        K::Char => &[
            K::UInt16,
            K::Int32,
            K::UInt32,
            K::Int64,
            K::UInt64,
            K::Single,
            K::Double,
            K::Decimal,
        ],
        K::Int32 => &[K::Int64, K::Single, K::Double, K::Decimal],
        K::UInt32 => &[K::Int64, K::UInt64, K::Single, K::Double, K::Decimal],
        K::Int64 | K::UInt64 => &[K::Single, K::Double, K::Decimal],
        K::Single => &[K::Double],
        _ => &[],
    };
    targets.contains(&t)
}

/// `CSharpConversions.IsImplicitReferenceConversion`: a reference type to a base type or an
/// interface, and array covariance.
pub(crate) fn is_implicit_reference_conversion(
    ctx: &mut Context<'_, '_, '_>,
    from: TypeId,
    to: TypeId,
) -> bool {
    if ctx.ts.is_reference_type(to) != Some(true) || ctx.ts.is_reference_type(from) != Some(true) {
        return false;
    }
    if matches!(ctx.ts.data(from), TypeData::UnknownSpecial) {
        return false;
    }
    if let (
        TypeData::Array {
            elem: fe, rank: fr, ..
        },
        TypeData::Array {
            elem: te, rank: tr, ..
        },
    ) = (ctx.ts.data(from).clone(), ctx.ts.data(to).clone())
        && fr == tr
    {
        return is_implicit_reference_conversion(ctx, fe, te);
    }
    is_base_type(ctx, from, to)
}

/// A boxing conversion: a value type to `object`, `ValueType`, `Enum` or an interface it
/// implements.
fn is_boxing_conversion(ctx: &mut Context<'_, '_, '_>, from: TypeId, to: TypeId) -> bool {
    if ctx.ts.is_reference_type(from) != Some(false) || ctx.ts.is_reference_type(to) != Some(true) {
        return false;
    }
    let from = underlying(ctx, from);
    is_base_type(ctx, from, to)
}

/// Whether `to` is among the base types of `from` (`GetAllBaseTypes`).
fn is_base_type(ctx: &mut Context<'_, '_, '_>, from: TypeId, to: TypeId) -> bool {
    let Ok(bases) = ctx.ts.all_base_types(from) else {
        return false;
    };
    let to = ctx.ts.normalize(to);
    bases.into_iter().any(|b| ctx.ts.normalize(b) == to)
}
