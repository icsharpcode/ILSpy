//! `DecompiledLambdaResolveResult`: the resolve result of a lambda the C# body translation
//! made, with the type its body infers.

use std::cell::Cell;

use crate::typesystem::fnptr::Normalization;
use crate::typesystem::types::TypeId;

use super::{Conversion, Conversions, Lambda, Parameter};

/// `DecompiledLambdaResolveResult` of a lambda with a parameter list.
#[derive(Debug)]
pub struct DecompiledLambda {
    pub delegate_type: TypeId,
    /// `InferredReturnType`: the type the body infers, which can differ from the function's
    /// return type where a return statement converts implicitly.
    pub inferred_return_type: TypeId,
    /// `ReturnType`: the function's return type.
    pub return_type: TypeId,
    pub parameters: Vec<Parameter>,
    /// `AttemptedConversionWithTypeMismatch`: a conversion to a delegate whose parameter
    /// types differ from the lambda's (beyond nullability) was tried.
    pub attempted_conversion_with_type_mismatch: Cell<bool>,
    /// `IsImplicitlyTyped`: the parameter list is written without types (a parameter of an
    /// anonymous type).
    pub implicitly_typed: bool,
    /// `IsAnonymousMethod`: `delegate (...) { ... }`.
    pub anonymous_method: bool,
    /// `HasParameterList`: without one, `delegate { ... }` converts to any parameter list.
    pub has_parameter_list: bool,
    /// `IsAsync`.
    pub is_async: bool,
}

impl Lambda for DecompiledLambda {
    fn has_parameter_list(&self) -> bool {
        self.has_parameter_list
    }

    fn is_anonymous_method(&self) -> bool {
        self.anonymous_method
    }

    fn is_implicitly_typed(&self) -> bool {
        self.implicitly_typed
    }

    fn is_async(&self) -> bool {
        self.is_async
    }

    fn parameters(&self) -> &[Parameter] {
        &self.parameters
    }

    fn decompiled(&self) -> Option<&DecompiledLambda> {
        Some(self)
    }

    /// The body's type, whatever the parameter types: the lambda keeps its explicit types.
    fn inferred_return_type(
        &self,
        _conversions: &mut Conversions<'_, '_, '_>,
        _parameter_types: Option<&[TypeId]>,
    ) -> TypeId {
        self.inferred_return_type
    }

    fn is_valid(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        parameter_types: &[TypeId],
        return_type: TypeId,
    ) -> Conversion {
        // An anonymous method without parameter list is applicable to any parameter list.
        let checked: &[Parameter] = if !self.has_parameter_list {
            &[]
        } else {
            if self.parameters.len() != parameter_types.len() {
                return Conversion::None;
            }
            &self.parameters
        };
        let n = Normalization::IGNORE_NULLABILITY;
        for (&t, p) in parameter_types.iter().zip(checked) {
            if conversions.ts.normalize_with(t, n) == conversions.ts.normalize_with(p.ty, n) {
                continue;
            }
            self.attempted_conversion_with_type_mismatch.set(true);
            if !conversions.identity(t, p.ty) {
                // Other parameter types may make an implicitly typed lambda valid too.
                return if self.implicitly_typed {
                    Conversion::ANONYMOUS_FUNCTION
                } else {
                    Conversion::None
                };
            }
        }
        if conversions.identity(self.return_type, return_type)
            || conversions
                .implicit_type(self.inferred_return_type, return_type)
                .is_valid()
        {
            Conversion::ANONYMOUS_FUNCTION
        } else {
            Conversion::None
        }
    }
}
