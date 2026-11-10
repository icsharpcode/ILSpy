//! `ICSharpCode.Decompiler.Semantics.Conversion`: the kind of a C# conversion.

use std::fmt;

use super::Method;

/// The built-in conversions without payload (`BuiltinConversion`).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Builtin {
    Identity,
    NullLiteral,
    ConstantExpression,
    Reference,
    Dynamic,
    Nullable,
    Pointer,
    Boxing,
    Unboxing,
    TryCast,
    InterpolatedString,
    ThrowExpression,
    InlineArray,
    ImplicitSpan,
    ExplicitSpan,
    DefaultLiteral,
}

/// A conversion. Equality follows the managed `Conversion.Equals`: built-in conversions
/// compare by kind, a user-defined conversion by its operator and flags (not the conversions
/// around it), a method group conversion by its method.
#[derive(Clone, Debug)]
pub enum Conversion {
    /// `Conversion.None`.
    None,
    Builtin {
        implicit: bool,
        kind: Builtin,
    },
    /// `NumericOrEnumerationConversion`.
    Numeric {
        implicit: bool,
        lifted: bool,
        enumeration: bool,
    },
    UserDefined {
        method: Box<Method>,
        implicit: bool,
        lifted: bool,
        valid: bool,
        before: Box<Conversion>,
        after: Box<Conversion>,
    },
    MethodGroup {
        method: Box<Method>,
        virtual_lookup: bool,
        captures_first_argument: bool,
        valid: bool,
    },
    Tuple {
        implicit: bool,
        elements: Vec<Conversion>,
    },
}

const fn builtin(implicit: bool, kind: Builtin) -> Conversion {
    Conversion::Builtin { implicit, kind }
}

impl Conversion {
    pub const IDENTITY: Conversion = builtin(true, Builtin::Identity);
    pub const IMPLICIT_NUMERIC: Conversion = Conversion::Numeric {
        implicit: true,
        lifted: false,
        enumeration: false,
    };
    pub const EXPLICIT_NUMERIC: Conversion = Conversion::Numeric {
        implicit: false,
        lifted: false,
        enumeration: false,
    };
    pub const IMPLICIT_LIFTED_NUMERIC: Conversion = Conversion::Numeric {
        implicit: true,
        lifted: true,
        enumeration: false,
    };
    pub const EXPLICIT_LIFTED_NUMERIC: Conversion = Conversion::Numeric {
        implicit: false,
        lifted: true,
        enumeration: false,
    };
    pub const NULL_LITERAL: Conversion = builtin(true, Builtin::NullLiteral);
    pub const IMPLICIT_CONSTANT_EXPRESSION: Conversion = builtin(true, Builtin::ConstantExpression);
    pub const IMPLICIT_REFERENCE: Conversion = builtin(true, Builtin::Reference);
    pub const EXPLICIT_REFERENCE: Conversion = builtin(false, Builtin::Reference);
    pub const IMPLICIT_DYNAMIC: Conversion = builtin(true, Builtin::Dynamic);
    pub const EXPLICIT_DYNAMIC: Conversion = builtin(false, Builtin::Dynamic);
    pub const IMPLICIT_NULLABLE: Conversion = builtin(true, Builtin::Nullable);
    pub const EXPLICIT_NULLABLE: Conversion = builtin(false, Builtin::Nullable);
    pub const IMPLICIT_POINTER: Conversion = builtin(true, Builtin::Pointer);
    pub const EXPLICIT_POINTER: Conversion = builtin(false, Builtin::Pointer);
    pub const BOXING: Conversion = builtin(true, Builtin::Boxing);
    pub const UNBOXING: Conversion = builtin(false, Builtin::Unboxing);
    pub const TRY_CAST: Conversion = builtin(false, Builtin::TryCast);
    pub const IMPLICIT_INTERPOLATED_STRING: Conversion = builtin(true, Builtin::InterpolatedString);
    pub const THROW_EXPRESSION: Conversion = builtin(true, Builtin::ThrowExpression);
    pub const INLINE_ARRAY: Conversion = builtin(true, Builtin::InlineArray);
    pub const IMPLICIT_SPAN: Conversion = builtin(true, Builtin::ImplicitSpan);
    pub const EXPLICIT_SPAN: Conversion = builtin(false, Builtin::ExplicitSpan);
    pub const DEFAULT_LITERAL: Conversion = builtin(true, Builtin::DefaultLiteral);

    /// `Conversion.EnumerationConversion`.
    pub const fn enumeration(implicit: bool, lifted: bool) -> Conversion {
        Conversion::Numeric {
            implicit,
            lifted,
            enumeration: true,
        }
    }

    /// `Conversion.UserDefinedConversion`.
    pub fn user_defined(
        method: Method,
        implicit: bool,
        before: Conversion,
        after: Conversion,
        lifted: bool,
        ambiguous: bool,
    ) -> Conversion {
        Conversion::UserDefined {
            method: Box::new(method),
            implicit,
            lifted,
            valid: !ambiguous,
            before: Box::new(before),
            after: Box::new(after),
        }
    }

    /// `Conversion.MethodGroupConversion` (`valid`) or `InvalidMethodGroupConversion`.
    pub fn method_group(
        method: Method,
        virtual_lookup: bool,
        captures_first_argument: bool,
        valid: bool,
    ) -> Conversion {
        Conversion::MethodGroup {
            method: Box::new(method),
            virtual_lookup,
            captures_first_argument,
            valid,
        }
    }

    /// `Conversion.TupleConversion`: implicit when every element conversion is.
    pub fn tuple(elements: Vec<Conversion>) -> Conversion {
        Conversion::Tuple {
            implicit: elements.iter().all(Conversion::is_implicit),
            elements,
        }
    }

    pub fn is_valid(&self) -> bool {
        match self {
            Conversion::None => false,
            Conversion::UserDefined { valid, .. } | Conversion::MethodGroup { valid, .. } => *valid,
            _ => true,
        }
    }

    pub fn is_implicit(&self) -> bool {
        match self {
            Conversion::None => false,
            Conversion::Builtin { implicit, .. }
            | Conversion::Numeric { implicit, .. }
            | Conversion::UserDefined { implicit, .. }
            | Conversion::Tuple { implicit, .. } => *implicit,
            Conversion::MethodGroup { .. } => true,
        }
    }

    pub fn is_explicit(&self) -> bool {
        match self {
            Conversion::None | Conversion::MethodGroup { .. } => false,
            Conversion::Builtin { implicit, .. }
            | Conversion::Numeric { implicit, .. }
            | Conversion::UserDefined { implicit, .. }
            | Conversion::Tuple { implicit, .. } => !*implicit,
        }
    }

    fn is_builtin(&self, k: Builtin) -> bool {
        matches!(self, Conversion::Builtin { kind, .. } if *kind == k)
    }

    pub fn is_identity(&self) -> bool {
        self.is_builtin(Builtin::Identity)
    }

    pub fn is_null_literal(&self) -> bool {
        self.is_builtin(Builtin::NullLiteral)
    }

    pub fn is_constant_expression(&self) -> bool {
        self.is_builtin(Builtin::ConstantExpression)
    }

    pub fn is_reference(&self) -> bool {
        self.is_builtin(Builtin::Reference)
    }

    pub fn is_dynamic(&self) -> bool {
        self.is_builtin(Builtin::Dynamic)
    }

    pub fn is_nullable(&self) -> bool {
        self.is_builtin(Builtin::Nullable)
    }

    pub fn is_pointer(&self) -> bool {
        self.is_builtin(Builtin::Pointer)
    }

    pub fn is_boxing(&self) -> bool {
        self.is_builtin(Builtin::Boxing)
    }

    pub fn is_unboxing(&self) -> bool {
        self.is_builtin(Builtin::Unboxing)
    }

    pub fn is_try_cast(&self) -> bool {
        self.is_builtin(Builtin::TryCast)
    }

    pub fn is_interpolated_string(&self) -> bool {
        self.is_builtin(Builtin::InterpolatedString)
    }

    pub fn is_throw_expression(&self) -> bool {
        self.is_builtin(Builtin::ThrowExpression)
    }

    pub fn is_inline_array(&self) -> bool {
        self.is_builtin(Builtin::InlineArray)
    }

    pub fn is_implicit_span(&self) -> bool {
        self.is_builtin(Builtin::ImplicitSpan)
    }

    pub fn is_explicit_span(&self) -> bool {
        self.is_builtin(Builtin::ExplicitSpan)
    }

    pub fn is_default_literal(&self) -> bool {
        self.is_builtin(Builtin::DefaultLiteral)
    }

    pub fn is_numeric(&self) -> bool {
        matches!(
            self,
            Conversion::Numeric {
                enumeration: false,
                ..
            }
        )
    }

    pub fn is_enumeration(&self) -> bool {
        matches!(
            self,
            Conversion::Numeric {
                enumeration: true,
                ..
            }
        )
    }

    pub fn is_lifted(&self) -> bool {
        matches!(
            self,
            Conversion::Numeric { lifted: true, .. } | Conversion::UserDefined { lifted: true, .. }
        )
    }

    pub fn is_user_defined(&self) -> bool {
        matches!(self, Conversion::UserDefined { .. })
    }

    pub fn is_method_group(&self) -> bool {
        matches!(self, Conversion::MethodGroup { .. })
    }

    pub fn is_tuple(&self) -> bool {
        matches!(self, Conversion::Tuple { .. })
    }

    pub fn is_virtual_method_lookup(&self) -> bool {
        matches!(
            self,
            Conversion::MethodGroup {
                virtual_lookup: true,
                ..
            }
        )
    }

    pub fn delegate_captures_first_argument(&self) -> bool {
        matches!(
            self,
            Conversion::MethodGroup {
                captures_first_argument: true,
                ..
            }
        )
    }

    /// The operator of a user-defined conversion, the chosen method of a method group
    /// conversion.
    pub fn method(&self) -> Option<&Method> {
        match self {
            Conversion::UserDefined { method, .. } | Conversion::MethodGroup { method, .. } => {
                Some(method)
            }
            _ => None,
        }
    }

    pub fn conversion_before_user_defined_operator(&self) -> Option<&Conversion> {
        match self {
            Conversion::UserDefined { before, .. } => Some(before),
            _ => None,
        }
    }

    pub fn conversion_after_user_defined_operator(&self) -> Option<&Conversion> {
        match self {
            Conversion::UserDefined { after, .. } => Some(after),
            _ => None,
        }
    }

    /// The element conversions of a tuple conversion.
    pub fn element_conversions(&self) -> Option<&[Conversion]> {
        match self {
            Conversion::Tuple { elements, .. } => Some(elements),
            _ => None,
        }
    }
}

impl PartialEq for Conversion {
    fn eq(&self, other: &Self) -> bool {
        match (self, other) {
            (Conversion::None, Conversion::None) => true,
            (
                Conversion::Builtin {
                    implicit: a,
                    kind: ka,
                },
                Conversion::Builtin {
                    implicit: b,
                    kind: kb,
                },
            ) => a == b && ka == kb,
            (
                Conversion::Numeric {
                    implicit: a,
                    lifted: la,
                    enumeration: ea,
                },
                Conversion::Numeric {
                    implicit: b,
                    lifted: lb,
                    enumeration: eb,
                },
            ) => a == b && la == lb && ea == eb,
            (
                Conversion::UserDefined {
                    method: ma,
                    implicit: ia,
                    lifted: la,
                    valid: va,
                    ..
                },
                Conversion::UserDefined {
                    method: mb,
                    implicit: ib,
                    lifted: lb,
                    valid: vb,
                    ..
                },
            ) => la == lb && ia == ib && va == vb && ma == mb,
            (
                Conversion::MethodGroup { method: ma, .. },
                Conversion::MethodGroup { method: mb, .. },
            ) => ma == mb,
            (Conversion::Tuple { elements: a, .. }, Conversion::Tuple { elements: b, .. }) => {
                a == b
            }
            _ => false,
        }
    }
}

impl fmt::Display for Conversion {
    /// `Conversion.ToString()`.
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        let direction = |implicit: bool| if implicit { "implicit" } else { "explicit" };
        match self {
            Conversion::None => f.write_str("None"),
            Conversion::Builtin { implicit, kind } => {
                let name = match kind {
                    Builtin::Identity => return f.write_str("identity conversion"),
                    Builtin::NullLiteral => return f.write_str("null-literal conversion"),
                    Builtin::Boxing => return f.write_str("boxing conversion"),
                    Builtin::Unboxing => return f.write_str("unboxing conversion"),
                    Builtin::TryCast => return f.write_str("try cast"),
                    Builtin::InterpolatedString => return f.write_str("interpolated string"),
                    Builtin::ThrowExpression => {
                        return f.write_str("throw-expression conversion");
                    }
                    Builtin::InlineArray => return f.write_str("inline array conversion"),
                    Builtin::ImplicitSpan => return f.write_str("implicit span conversion"),
                    Builtin::ExplicitSpan => return f.write_str("explicit span conversion"),
                    Builtin::DefaultLiteral => {
                        return f.write_str("default-literal conversion");
                    }
                    Builtin::ConstantExpression => "constant-expression",
                    Builtin::Reference => "reference",
                    Builtin::Dynamic => "dynamic",
                    Builtin::Nullable => "nullable",
                    Builtin::Pointer => "pointer",
                };
                write!(f, "{} {name} conversion", direction(*implicit))
            }
            Conversion::Numeric {
                implicit,
                lifted,
                enumeration,
            } => write!(
                f,
                "{}{}{} conversion",
                direction(*implicit),
                if *lifted { " lifted" } else { "" },
                if *enumeration {
                    " enumeration"
                } else {
                    " numeric"
                }
            ),
            Conversion::UserDefined {
                method,
                implicit,
                lifted,
                valid,
                ..
            } => write!(
                f,
                "{}{}{}user-defined conversion ({})",
                direction(*implicit),
                if *lifted { " lifted" } else { "" },
                if *valid { "" } else { " ambiguous" },
                method.name
            ),
            Conversion::MethodGroup { method, .. } => {
                write!(f, "method group conversion ({})", method.name)
            }
            Conversion::Tuple { implicit, .. } => {
                write!(f, "{}  tuple conversion", direction(*implicit))
            }
        }
    }
}
