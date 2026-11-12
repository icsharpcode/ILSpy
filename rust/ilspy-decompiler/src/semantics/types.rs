//! The type queries of the C# semantics: `IType.Kind`, `ReflectionHelper.GetTypeCode`,
//! `IsKnownType`, `NullableType`, `TupleType.GetTupleElementTypes`, `TaskType` and the
//! base type definitions.

use crate::metadata::tables::TableId;
use crate::typesystem::compilation::{TypeDefId, TypeKind as DefinitionKind};
use crate::typesystem::flags::has_attribute;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// `TypeKind`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum TypeKind {
    Other,
    Class,
    Interface,
    Struct,
    Delegate,
    Enum,
    Void,
    Unknown,
    Null,
    None,
    Dynamic,
    UnboundTypeArgument,
    TypeParameter,
    Array,
    Pointer,
    ByReference,
    ArgList,
    Tuple,
    ModOpt,
    ModReq,
    Intersection,
    NInt,
    NUInt,
    FunctionPointer,
}

impl TypeKind {
    /// `TypeKindExtensions.IsAnyPointer`.
    pub fn is_any_pointer(self) -> bool {
        matches!(self, TypeKind::Pointer | TypeKind::FunctionPointer)
    }
}

/// `TupleType.RestPosition`.
pub const TUPLE_REST: usize = 8;

/// `VarianceModifier`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Variance {
    Invariant,
    Covariant,
    Contravariant,
}

impl Variance {
    /// `VarianceModifierExtensions.Combine`: the variance of a position nested in another.
    pub fn combine(self, inner: Variance) -> Variance {
        match (self, inner) {
            (Variance::Invariant, _) | (_, Variance::Invariant) => Variance::Invariant,
            (Variance::Covariant, v) => v,
            (Variance::Contravariant, Variance::Covariant) => Variance::Contravariant,
            (Variance::Contravariant, Variance::Contravariant) => Variance::Covariant,
        }
    }
}

impl TypeSystem<'_, '_> {
    /// `IType.Kind`.
    pub fn kind(&self, t: TypeId) -> TypeKind {
        match self.data(t) {
            TypeData::Def(def) => match self.comp.type_def(*def).map(|i| i.kind) {
                Ok(DefinitionKind::Class) => TypeKind::Class,
                Ok(DefinitionKind::Interface) => TypeKind::Interface,
                Ok(DefinitionKind::Struct) => TypeKind::Struct,
                Ok(DefinitionKind::Delegate) => TypeKind::Delegate,
                Ok(DefinitionKind::Enum) => TypeKind::Enum,
                Ok(DefinitionKind::Void) => TypeKind::Void,
                Err(_) => TypeKind::Unknown,
            },
            TypeData::Unknown { .. } | TypeData::UnknownSpecial => TypeKind::Unknown,
            TypeData::Unbound => TypeKind::UnboundTypeArgument,
            TypeData::TypeParam { .. } => TypeKind::TypeParameter,
            TypeData::Inst { generic, .. } => self.kind(*generic),
            TypeData::Array { .. } => TypeKind::Array,
            TypeData::Pointer(_) => TypeKind::Pointer,
            TypeData::ByRef(_) => TypeKind::ByReference,
            TypeData::Pinned(_) => TypeKind::Other,
            // Without TypeSystemOptions.FunctionPointers a function pointer is UIntPtr.
            TypeData::FnPtr(_) if self.definition(t).is_some() => TypeKind::Struct,
            TypeData::FnPtr(_) => TypeKind::FunctionPointer,
            TypeData::ArgList => TypeKind::ArgList,
            TypeData::Modified { required: true, .. } => TypeKind::ModReq,
            TypeData::Modified { .. } => TypeKind::ModOpt,
            TypeData::Dynamic => TypeKind::Dynamic,
            TypeData::NInt => TypeKind::NInt,
            TypeData::NUInt => TypeKind::NUInt,
            TypeData::Tuple { .. } => TypeKind::Tuple,
            TypeData::Annotated { inner, .. } => self.kind(*inner),
            TypeData::Null => TypeKind::Null,
            TypeData::NoType => TypeKind::None,
            TypeData::Intersection(_) => TypeKind::Intersection,
        }
    }

    /// `type.GetDefinition()?.KnownTypeCode`, `None` without a definition.
    pub fn known_type_code(&self, t: TypeId) -> KnownTypeCode {
        self.definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .map_or(KnownTypeCode::None, |i| i.known)
    }

    /// `IntersectionType.Create`: the distinct types in order; `?` for none, the type itself
    /// for one.
    pub fn intersection(&mut self, types: &[TypeId]) -> TypeId {
        let mut distinct: Vec<TypeId> = Vec::new();
        for &t in types {
            if !distinct.contains(&t) {
                distinct.push(t);
            }
        }
        match distinct[..] {
            [] => self.intern(TypeData::UnknownSpecial),
            [single] => single,
            _ => self.intern(TypeData::Intersection(distinct.into())),
        }
    }

    /// `ReflectionHelper.GetTypeCode`: the `TypeCode` of a primitive type definition
    /// (`Empty`, 0, for every other type, including annotated and parameterized ones).
    pub fn type_code(&self, t: TypeId) -> u8 {
        let TypeData::Def(def) = self.data(t) else {
            return 0;
        };
        match self.comp.type_def(*def).map(|i| i.known) {
            Ok(code) if code as u8 <= KnownTypeCode::String as u8 => code as u8,
            _ => 0,
        }
    }

    /// `IType.TypeParameterCount`: a parameterized type counts its type arguments.
    pub fn type_parameter_count_of(&self, t: TypeId) -> usize {
        match self.data(t) {
            TypeData::Inst { args, .. } => args.len(),
            TypeData::Annotated { inner, .. } => self.type_parameter_count_of(*inner),
            _ => self.type_parameter_count(t) as usize,
        }
    }

    /// The type arguments of a parameterized type, seen through a nullability annotation.
    fn instance_arguments(&self, t: TypeId) -> Option<(TypeId, &[TypeId])> {
        match self.data(t) {
            TypeData::Inst { generic, args } => Some((*generic, args)),
            TypeData::Annotated { inner, .. } => self.instance_arguments(*inner),
            _ => None,
        }
    }

    /// The first type argument of a parameterized type (`TypeArguments[0]`).
    pub fn first_type_argument(&self, t: TypeId) -> Option<TypeId> {
        self.instance_arguments(t)
            .and_then(|(_, args)| args.first().copied())
    }

    /// `NullableType.IsNullable`.
    pub fn is_nullable(&self, t: TypeId) -> bool {
        let t = self.skip_modifiers(t);
        matches!(self.data(t), TypeData::Inst { generic, args }
            if args.len() == 1 && self.is_known(*generic, KnownTypeCode::NullableOfT))
    }

    /// `NullableType.GetUnderlyingType`: the element type of a nullable type, otherwise the
    /// type itself.
    pub fn nullable_underlying(&self, t: TypeId) -> TypeId {
        let s = self.skip_modifiers(t);
        match self.data(s) {
            TypeData::Inst { generic, args }
                if args.len() == 1 && self.is_known(*generic, KnownTypeCode::NullableOfT) =>
            {
                args[0]
            }
            _ => t,
        }
    }

    /// `NullableType.Create`.
    pub fn make_nullable(&mut self, element: TypeId) -> TypeId {
        let nullable = self.known_type(KnownTypeCode::NullableOfT);
        if self.definition(nullable).is_some() {
            self.intern(TypeData::Inst {
                generic: nullable,
                args: [element].into(),
            })
        } else {
            self.intern(TypeData::UnknownSpecial)
        }
    }

    /// `NullableType.IsNonNullableValueType`.
    pub fn is_non_nullable_value_type(&mut self, t: TypeId) -> bool {
        self.is_reference_type(t) == Some(false) && !self.is_nullable(t)
    }

    /// `TupleType.GetTupleElementTypes`: the element types of a tuple or `System.ValueTuple`
    /// type, flattening `Rest`; `None` for other types.
    pub fn tuple_element_types(&mut self, t: TypeId) -> Option<Vec<TypeId>> {
        let mut output = Vec::new();
        self.collect_tuple_element_types(t, &mut output)
            .then_some(output)
    }

    fn collect_tuple_element_types(&mut self, t: TypeId, output: &mut Vec<TypeId>) -> bool {
        if let TypeData::Tuple { elements, .. } = self.data(t) {
            output.extend(elements.iter());
            return true;
        }
        let Some(def) = self.definition(t) else {
            return false;
        };
        let Ok(info) = self.comp.type_def(def) else {
            return false;
        };
        if !matches!(info.kind, DefinitionKind::Class | DefinitionKind::Struct)
            || info.full_name.top.ns != "System"
            || info.full_name.name() != "ValueTuple"
        {
            return false;
        }
        let tpc = info.type_parameter_count as usize;
        let args = self.type_arguments(t);
        if tpc > 0 && tpc < TUPLE_REST {
            output.extend(args);
            true
        } else if tpc == TUPLE_REST && args.len() == TUPLE_REST {
            output.extend(&args[..TUPLE_REST - 1]);
            self.collect_tuple_element_types(args[TUPLE_REST - 1], output)
        } else {
            false
        }
    }

    /// `IType.TupleUnderlyingTypeOrSelf()`: the underlying type of a tuple, without
    /// nullability (also that of a generic type definition, as `WithoutNullability` drops it).
    pub fn tuple_underlying_or_self(&mut self, t: TypeId) -> TypeId {
        let t = match self.data(t) {
            TypeData::Tuple { underlying, .. } => *underlying,
            _ => t,
        };
        self.change_nullability(t, crate::typesystem::types::Nullability::Oblivious)
    }

    /// `TypeSystemExtensions.IsArrayInterfaceType`: `IEnumerable<T>`, `ICollection<T>`,
    /// `IList<T>`, `IReadOnlyCollection<T>` or `IReadOnlyList<T>`.
    pub fn is_array_interface(&self, t: TypeId) -> bool {
        self.type_parameter_count_of(t) == 1
            && matches!(
                self.known_type_code(t),
                KnownTypeCode::IEnumerableOfT
                    | KnownTypeCode::ICollectionOfT
                    | KnownTypeCode::IListOfT
                    | KnownTypeCode::IReadOnlyCollectionOfT
                    | KnownTypeCode::IReadOnlyListOfT
            )
    }

    /// `TypeSystemExtensions.IsInlineArrayType`.
    pub fn is_inline_array(&self, t: TypeId) -> bool {
        if self.kind(t) != TypeKind::Struct {
            return false;
        }
        self.definition(t).is_some_and(|d| {
            has_attribute(
                &self.comp.module(d.module).md,
                TableId::TypeDef,
                d.row,
                "System.Runtime.CompilerServices",
                "InlineArrayAttribute",
            )
        })
    }

    /// `TypeSystemExtensions.GetInlineArrayElementType`: the type of the single instance
    /// field, `?` otherwise.
    pub fn inline_array_element_type(&mut self, t: TypeId) -> TypeId {
        let unknown = self.intern(TypeData::UnknownSpecial);
        let Some(def) = self.definition(t) else {
            return unknown;
        };
        let md = &self.comp.module(def.module).md;
        let fields: Vec<u32> = self
            .comp
            .visible_fields(def)
            .unwrap_or_default()
            .into_iter()
            .filter(|&row| {
                // FieldAttributes.Static
                md.get::<crate::metadata::tables::rows::Field>(row)
                    .is_ok_and(|f| f.flags & 0x0010 == 0)
            })
            .collect();
        let [row] = fields[..] else {
            return unknown;
        };
        let (ty, _) = self.field_type(def.module, row);
        let args = self.instance_arguments(t).map(|(_, a)| a.to_vec());
        self.substitute(ty, args.as_deref(), None)
    }

    /// `TaskType.IsTask`: `Task`, or a parameterized `Task<T>`.
    pub fn is_task(&self, t: TypeId) -> bool {
        match self.known_type_code(t) {
            KnownTypeCode::Task => true,
            KnownTypeCode::TaskOfT => self.instance_arguments(t).is_some(),
            _ => false,
        }
    }

    /// `TaskType.IsCustomTask`: a type definition with at most one type parameter and an
    /// `[AsyncMethodBuilder(typeof(...))]`.
    pub fn is_custom_task(&mut self, t: TypeId) -> bool {
        self.definition(t)
            .is_some_and(|def| self.custom_task_builder(def).is_some())
    }

    /// `IType.GetDelegateInvokeMethod()` as a `Method`.
    pub fn delegate_invoke(&mut self, t: TypeId) -> Option<super::Method> {
        let invoke = self.get_delegate_invoke_method(t).ok()??;
        super::Method::from_resolved(self, &invoke)
    }

    /// The variance of type parameter `index` of a type definition.
    pub fn type_parameter_variance(&mut self, def: TypeDefId, index: usize) -> Variance {
        let variance = self
            .type_parameters(def.module, TableId::TypeDef, def.row)
            .ok()
            .and_then(|tps| tps.get(index).map(|tp| tp.variance));
        match variance {
            Some("Covariant") => Variance::Covariant,
            Some("Contravariant") => Variance::Contravariant,
            _ => Variance::Invariant,
        }
    }

    /// `IType.GetAllBaseTypeDefinitions()`: the definitions of all base types, the type's own
    /// last.
    pub fn all_base_type_definitions(&mut self, t: TypeId) -> Vec<TypeDefId> {
        let mut result = Vec::new();
        for b in self.all_base_types(t).unwrap_or_default() {
            if let Some(d) = self.definition(b)
                && !result.contains(&d)
            {
                result.push(d);
            }
        }
        result
    }

    /// `ITypeDefinition.IsDerivedFrom(baseType)`.
    pub fn is_derived_from_definition(&mut self, def: TypeDefId, base: TypeDefId) -> bool {
        let t = self.intern_def(def);
        self.all_base_type_definitions(t).contains(&base)
    }
}
