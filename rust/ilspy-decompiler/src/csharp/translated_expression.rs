//! `TranslatedExpression`: a C# expression with the type of its resolve result.

use crate::typesystem::fnptr::Normalization;
use crate::typesystem::types::TypeId;

use super::ast::AstId;
use super::{Builder, Unsupported};

/// An expression the builders produced, with `ResolveResult.Type`.
#[derive(Clone, Copy, Debug)]
pub struct TranslatedExpression {
    pub expression: AstId,
    pub ty: TypeId,
}

impl Builder<'_, '_, '_, '_> {
    /// `TranslatedExpression.ConvertTo(targetType, expressionBuilder, allowImplicitConversion)`
    /// for an expression without a default literal, conversion or invocation resolve result.
    /// Only the identity case is translated: an expression whose type equals the target type
    /// up to nullability and tuple element names stays as it is.
    pub(crate) fn convert_to(
        &mut self,
        e: TranslatedExpression,
        target: TypeId,
        _allow_implicit_conversion: bool,
    ) -> Result<TranslatedExpression, Unsupported> {
        let n = Normalization::IGNORE_NULLABILITY_AND_TUPLES;
        if self.ts.normalize_with(e.ty, n) == self.ts.normalize_with(target, n) {
            return Ok(e);
        }
        Err("a conversion of the expression".to_owned())
    }
}
