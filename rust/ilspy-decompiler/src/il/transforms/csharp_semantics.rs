//! The C# conversions the IL transforms ask the managed `CSharpConversions` about, answered
//! by `crate::semantics::Conversions`.

use crate::semantics::{Conversion, Conversions};
use crate::typesystem::types::TypeId;

use super::Context;

/// `NormalizeTypeVisitor.TypeErasure.EquivalentTypes`.
pub(crate) fn type_erasure_equivalent(ctx: &mut Context<'_, '_, '_>, a: TypeId, b: TypeId) -> bool {
    Conversions::new(ctx.ts).identity(a, b)
}

/// `CSharpConversions.ImplicitConversion(from, to)`.
pub(crate) fn implicit_conversion(
    ctx: &mut Context<'_, '_, '_>,
    from: TypeId,
    to: TypeId,
) -> Conversion {
    Conversions::new(ctx.ts).implicit_type(from, to)
}

/// `CSharpConversions.IsImplicitReferenceConversion`.
pub(crate) fn is_implicit_reference_conversion(
    ctx: &mut Context<'_, '_, '_>,
    from: TypeId,
    to: TypeId,
) -> bool {
    Conversions::new(ctx.ts).is_implicit_reference(from, to)
}
