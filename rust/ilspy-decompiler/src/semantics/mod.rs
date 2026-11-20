//! C# semantics: conversions (`CSharpConversions`), type inference (`TypeInference`) and
//! overload resolution (`OverloadResolution`, `MethodGroupResolveResult`), with the resolve
//! results they consume (`ICSharpCode.Decompiler.Semantics`).

pub mod conversion;
pub mod conversions;
pub mod decompiled_lambda;
pub mod overload_resolution;
pub mod type_inference;
pub mod types;

use std::fmt;
use std::rc::Rc;
use std::sync::atomic::{AtomicU32, Ordering};

pub use conversion::Conversion;
pub use conversions::Conversions;
pub use overload_resolution::{OverloadResolution, OverloadResolutionErrors};
pub use type_inference::{TypeInference, TypeInferenceAlgorithm};

use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::members::{ConstValue, MethodDefId, RefKind, SymbolKind};
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// `IParameter`, as the C# semantics see it.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct Parameter {
    pub name: String,
    pub ty: TypeId,
    pub ref_kind: RefKind,
    pub is_params: bool,
    pub is_optional: bool,
}

impl Parameter {
    /// A by-value parameter without a name.
    pub fn of_type(ty: TypeId) -> Parameter {
        Parameter {
            name: String::new(),
            ty,
            ref_kind: RefKind::None,
            is_params: false,
            is_optional: false,
        }
    }
}

/// Which method a `Method` is an instance of.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum MethodKey {
    Def(MethodDefId),
    /// The implicit parameterless constructor of a struct or enum without one.
    DummyConstructor(TypeDefId),
    /// A method created without metadata (`FakeMethod`); every one is distinct.
    Fake(u32),
}

/// `IMethod`: a method definition (or fake) and the substitution applied to it. Two methods
/// are equal when they are the same definition under the same substitution.
#[derive(Clone, Debug)]
pub struct Method {
    pub key: MethodKey,
    pub name: String,
    /// The (specialized) declaring type.
    pub declaring: Option<TypeId>,
    pub is_static: bool,
    pub is_operator: bool,
    pub is_overridable: bool,
    /// `IMethod.ReturnTypeIsRefReadOnly`.
    pub return_type_is_ref_readonly: bool,
    /// The definition's type parameters (`MemberDefinition.TypeParameters`).
    pub type_parameters: Vec<TypeId>,
    /// The definition's parameters, before any substitution.
    pub definition_parameters: Vec<Parameter>,
    pub definition_return_type: TypeId,
    /// The substitution; `None` lists leave that kind of type parameter alone.
    pub class_args: Option<Vec<TypeId>>,
    pub method_args: Option<Vec<TypeId>>,
    /// The parameters and return type after the substitution.
    pub parameters: Vec<Parameter>,
    pub return_type: TypeId,
    /// `LiftedUserDefinedOperator`: the lifted form of a user-defined operator, whose
    /// parameters (and return type, except for comparisons) are nullable.
    pub lifted: bool,
}

impl PartialEq for Method {
    fn eq(&self, other: &Self) -> bool {
        self.key == other.key
            && self.lifted == other.lifted
            && self.class_args == other.class_args
            && self.method_args == other.method_args
    }
}

static NEXT_FAKE: AtomicU32 = AtomicU32::new(0);

impl Method {
    /// `new FakeMethod(compilation, SymbolKind.Method)` with these parameters, returning
    /// `void` unless `return_type` says otherwise; every fake method is distinct.
    pub fn fake(name: &str, parameters: Vec<Parameter>, return_type: TypeId) -> Method {
        Method {
            key: MethodKey::Fake(NEXT_FAKE.fetch_add(1, Ordering::Relaxed)),
            name: name.to_owned(),
            declaring: None,
            is_static: false,
            is_operator: false,
            is_overridable: false,
            return_type_is_ref_readonly: false,
            type_parameters: Vec::new(),
            definition_parameters: parameters.clone(),
            definition_return_type: return_type,
            class_args: None,
            method_args: None,
            parameters,
            return_type,
            lifted: false,
        }
    }

    /// The method a resolved member reference is, `None` for a fake reference.
    pub fn from_resolved(ts: &mut TypeSystem<'_, '_>, m: &ResolvedMethod) -> Option<Method> {
        let key = match m.origin {
            MethodOrigin::Def(id) => MethodKey::Def(id),
            MethodOrigin::DummyConstructor(def) => MethodKey::DummyConstructor(def),
            MethodOrigin::Fake => return None,
        };
        let (definition_return_type, definition_parameters, type_parameters, flags) = match key {
            MethodKey::Def(id) => {
                let (ret, params) = ts.method_signature(id);
                let definition = ts.method_definition(id).ok()?;
                let flags = ts.method_modifiers_of(id).ok()?;
                let params = params
                    .into_iter()
                    .map(|p| Parameter {
                        name: p.name,
                        ty: p.ty,
                        ref_kind: p.ref_kind,
                        is_params: p.is_params,
                        is_optional: p.optional,
                    })
                    .collect();
                (ret, params, definition.type_args, Some(flags))
            }
            _ => (m.ret, Vec::new(), Vec::new(), None),
        };
        let is_operator = match key {
            MethodKey::Def(id) => {
                let accessor = ts.accessor_owner(id);
                ts.method_symbol_kind(id, accessor).ok()? == SymbolKind::Operator
            }
            _ => false,
        };
        let parameters = m
            .params()
            .ok()?
            .into_iter()
            .enumerate()
            .map(|(i, ty)| match definition_parameters.get(i) {
                Some(p) => Parameter { ty, ..p.clone() },
                None => Parameter::of_type(ty),
            })
            .collect();
        Some(Method {
            key,
            name: m.name.clone(),
            declaring: m.declaring,
            is_static: m.is_static,
            is_operator,
            is_overridable: flags.is_some_and(|f| f.is_overridable),
            // shortcut: `[IsReadOnly]` on the return parameter is not read; upgrade when a
            // delegate with a `ref readonly` return reaches a method group conversion.
            return_type_is_ref_readonly: false,
            type_parameters,
            definition_parameters,
            definition_return_type,
            class_args: m.class_args.clone(),
            method_args: m.method_args.clone(),
            parameters,
            return_type: m.ret,
            lifted: false,
        })
    }

    /// `((IMethod)MemberDefinition).Specialize(new TypeParameterSubstitution(class, method))`.
    pub fn specialize_definition(
        &self,
        ts: &mut TypeSystem<'_, '_>,
        class: Option<Vec<TypeId>>,
        method: Option<Vec<TypeId>>,
    ) -> Method {
        if let MethodKey::Def(id) = self.key
            && let Ok(definition) = ts.method_definition(id)
        {
            let specialized = ts.specialize(definition, class.clone(), method.clone());
            if let Some(m) = Method::from_resolved(ts, &specialized) {
                return m;
            }
        }
        let parameters = self
            .definition_parameters
            .iter()
            .map(|p| Parameter {
                ty: ts.substitute(p.ty, class.as_deref(), method.as_deref()),
                ..p.clone()
            })
            .collect();
        Method {
            return_type: ts.substitute(
                self.definition_return_type,
                class.as_deref(),
                method.as_deref(),
            ),
            parameters,
            class_args: class,
            method_args: method,
            ..self.clone()
        }
    }
}

/// `LambdaResolveResult`: an anonymous function, as conversions and type inference query it.
pub trait Lambda {
    /// Whether the lambda has a parameter list (`delegate { }` has none).
    fn has_parameter_list(&self) -> bool;
    /// Whether this is an anonymous method (`delegate (...) { }`) rather than a lambda.
    fn is_anonymous_method(&self) -> bool;
    /// Whether the parameter types are implicit.
    fn is_implicitly_typed(&self) -> bool;
    fn is_async(&self) -> bool;
    fn parameters(&self) -> &[Parameter];
    /// The lambda the C# body translation made, which the host can rebuild.
    fn decompiled(&self) -> Option<&decompiled_lambda::DecompiledLambda> {
        None
    }
    /// The return type the body infers to under `parameter_types` (`None` for an explicitly
    /// typed lambda).
    fn inferred_return_type(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        parameter_types: Option<&[TypeId]>,
    ) -> TypeId;
    /// Whether the lambda converts to a delegate with these parameter and return types.
    fn is_valid(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        parameter_types: &[TypeId],
        return_type: TypeId,
    ) -> Conversion;
}

/// `MethodListWithDeclaringType`.
#[derive(Clone, Debug)]
pub struct MethodList {
    pub declaring: TypeId,
    pub methods: Vec<Method>,
}

/// `MethodGroupResolveResult`.
#[derive(Clone, Debug)]
pub struct MethodGroup {
    /// The expression the methods are looked up on.
    pub target: ResolveResult,
    pub name: String,
    /// Grouped by declaring type, base types first.
    pub method_lists: Vec<MethodList>,
    pub type_arguments: Vec<TypeId>,
    /// The extension method groups, the innermost scope first.
    pub extension_methods: Vec<Vec<Method>>,
}

/// A resolved expression (`ResolveResult` and the subclasses the C# semantics look at).
#[derive(Clone)]
pub enum ResolveResult {
    /// `new ResolveResult(type)`: an expression of that type.
    Expression(TypeId),
    /// `TypeResolveResult`.
    Type(TypeId),
    /// `ConstantResolveResult`.
    Constant {
        ty: TypeId,
        value: ConstValue,
    },
    /// `DefaultLiteralResolveResult`: the type of the `default(T)` the literal was shortened
    /// from (`?` when it stands for no shortened expression); its own type is `NoType`.
    DefaultLiteral(TypeId),
    /// `InterpolatedStringResolveResult`.
    InterpolatedString(TypeId),
    /// `ThrowResolveResult`.
    Throw,
    /// `TupleResolveResult`.
    Tuple {
        ty: TypeId,
        elements: Vec<ResolveResult>,
    },
    /// `ByReferenceResolveResult`: `ty` is the by-reference type.
    ByReference {
        ty: TypeId,
        element: TypeId,
        ref_kind: RefKind,
    },
    /// `OutVarResolveResult`.
    OutVar(TypeId),
    /// `ThisResolveResult`.
    This {
        ty: TypeId,
        causes_non_virtual_invocation: bool,
    },
    Lambda(Rc<dyn Lambda>),
    MethodGroup(Rc<MethodGroup>),
}

impl ResolveResult {
    /// `new ByReferenceResolveResult(elementType, kind)`.
    pub fn by_reference(
        ts: &mut TypeSystem<'_, '_>,
        element: TypeId,
        ref_kind: RefKind,
    ) -> ResolveResult {
        ResolveResult::ByReference {
            ty: ts.intern(TypeData::ByRef(element)),
            element,
            ref_kind,
        }
    }

    /// `ResolveResult.Type`.
    pub fn ty(&self, ts: &mut TypeSystem<'_, '_>) -> TypeId {
        match self {
            ResolveResult::Expression(t)
            | ResolveResult::Type(t)
            | ResolveResult::Constant { ty: t, .. }
            | ResolveResult::InterpolatedString(t)
            | ResolveResult::Tuple { ty: t, .. }
            | ResolveResult::ByReference { ty: t, .. }
            | ResolveResult::OutVar(t)
            | ResolveResult::This { ty: t, .. } => *t,
            ResolveResult::DefaultLiteral(_)
            | ResolveResult::Throw
            | ResolveResult::Lambda(_)
            | ResolveResult::MethodGroup(_) => ts.intern(TypeData::NoType),
        }
    }

    /// `ResolveResult.IsCompileTimeConstant`.
    /// A default literal is a constant expression (C# standard 12.8.21) whose value, like the
    /// null literal's, is null until the conversion gives it a type.
    pub fn is_compile_time_constant(&self) -> bool {
        matches!(
            self,
            ResolveResult::Constant { .. } | ResolveResult::DefaultLiteral(_)
        )
    }

    /// `ResolveResult.ConstantValue`.
    pub fn constant_value(&self) -> Option<&ConstValue> {
        static NULL: ConstValue = ConstValue::Null;
        match self {
            ResolveResult::Constant { value, .. } => Some(value),
            ResolveResult::DefaultLiteral(_) => Some(&NULL),
            _ => None,
        }
    }

    pub fn as_lambda(&self) -> Option<&dyn Lambda> {
        match self {
            ResolveResult::Lambda(l) => Some(&**l),
            _ => None,
        }
    }
}

impl fmt::Debug for ResolveResult {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            ResolveResult::Expression(t) => write!(f, "Expression({t:?})"),
            ResolveResult::Type(t) => write!(f, "Type({t:?})"),
            ResolveResult::Constant { ty, value } => write!(f, "Constant({ty:?}, {value:?})"),
            ResolveResult::DefaultLiteral(t) => write!(f, "DefaultLiteral({t:?})"),
            ResolveResult::InterpolatedString(t) => write!(f, "InterpolatedString({t:?})"),
            ResolveResult::Throw => f.write_str("Throw"),
            ResolveResult::Tuple { ty, elements } => write!(f, "Tuple({ty:?}, {elements:?})"),
            ResolveResult::ByReference { ty, ref_kind, .. } => {
                write!(f, "ByReference({ty:?}, {ref_kind:?})")
            }
            ResolveResult::OutVar(t) => write!(f, "OutVar({t:?})"),
            ResolveResult::This { ty, .. } => write!(f, "This({ty:?})"),
            ResolveResult::Lambda(_) => f.write_str("Lambda"),
            ResolveResult::MethodGroup(g) => write!(f, "MethodGroup({})", g.name),
        }
    }
}
