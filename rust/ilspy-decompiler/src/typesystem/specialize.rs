//! The specialization API on resolved methods (`IMember.Specialize` on specialized members,
//! `SpecializedMethod.TypeParameters`, member equality) and `IsReferenceType` of type
//! parameters, which needs their effective base class.

use crate::metadata::Result;
use crate::metadata::tables::TableId;

use super::compilation::TypeKind;
use super::known::KnownTypeCode;
use super::members::TypeParamInfo;
use super::resolve::{MethodOrigin, ResolvedMethod};
use super::types::{TypeData, TypeId, TypeParamOwner, TypeSystem};

/// A substitution as `TypeParameterSubstitution` holds it: `None` keeps that kind of type
/// parameter unchanged.
pub type Substitution = (Option<Vec<TypeId>>, Option<Vec<TypeId>>);

impl TypeSystem<'_, '_> {
    /// `TypeParameterSubstitution.Compose(outer, inner)`: `inner` first, then `outer`.
    pub fn compose(&mut self, outer: &Substitution, inner: &Substitution) -> Substitution {
        if inner.0.is_none() && inner.1.is_none() {
            return outer.clone();
        }
        let apply = |ts: &mut Self, list: &Option<Vec<TypeId>>, fallback: &Option<Vec<TypeId>>| {
            match list {
                Some(args) => Some(
                    args.iter()
                        .map(|&a| ts.substitute(a, outer.0.as_deref(), outer.1.as_deref()))
                        .collect(),
                ),
                None => fallback.clone(),
            }
        };
        let class = apply(self, &inner.0, &outer.0);
        let method = apply(self, &inner.1, &outer.1);
        (class, method)
    }

    /// `IMember.Specialize(substitution)` on a possibly specialized method: the method's
    /// substitution composed with `substitution`, applied to its definition.
    pub fn specialize_method(
        &mut self,
        m: &ResolvedMethod,
        substitution: &Substitution,
    ) -> Result<ResolvedMethod> {
        let MethodOrigin::Def(id) = m.origin else {
            return Ok(self.specialize(m.clone(), substitution.0.clone(), substitution.1.clone()));
        };
        let existing = (m.class_args.clone(), m.method_args.clone());
        let (class, method) = self.compose(substitution, &existing);
        let definition = self.method_definition(id)?;
        Ok(if class.is_none() && method.is_none() {
            definition
        } else {
            self.specialize(definition, class, method)
        })
    }

    /// `IMember.Equals` for methods: the same definition under the same substitution.
    pub fn same_method(&self, a: &ResolvedMethod, b: &ResolvedMethod) -> bool {
        a.origin == b.origin && a.class_args == b.class_args && a.method_args == b.method_args
    }

    /// `IMethod.TypeParameters` of a (specialized) method: its definition's type parameters
    /// with constraints seen through the class substitution. The parameters are the
    /// definition's own (the Rust type system does not give specialized members an identity
    /// to own them).
    pub fn method_type_parameters(&mut self, m: &ResolvedMethod) -> Result<Vec<TypeParamInfo>> {
        let MethodOrigin::Def(id) = m.origin else {
            return Ok(Vec::new());
        };
        let mut tps = self.type_parameters(id.module, TableId::MethodDef, id.row)?;
        let class = m.class_args.clone();
        for tp in &mut tps {
            tp.constraints = tp
                .constraints
                .iter()
                .map(|&c| self.substitute(c, class.as_deref(), None))
                .collect();
        }
        Ok(tps)
    }

    pub fn type_parameter_info(
        &mut self,
        owner: TypeParamOwner,
        index: u32,
    ) -> Option<TypeParamInfo> {
        let (module, table, row) = match owner {
            TypeParamOwner::Type(def) => (def.module, TableId::TypeDef, def.row),
            TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => {
                (m.module, TableId::MethodDef, m.row)
            }
            TypeParamOwner::DummyType | TypeParamOwner::DummyMethod => return None,
            TypeParamOwner::Synthetic { id, .. } => {
                return Some(self.synthetic_type_parameter(id).clone());
            }
        };
        let mut info = self
            .type_parameters(module, table, row)
            .ok()?
            .into_iter()
            .nth(index as usize)?;
        if let TypeParamOwner::ContextMethod { context, .. } = owner {
            info.constraints = self.context_method_constraints(context, info.constraints);
        }
        Some(info)
    }

    /// A nullability-annotated type parameter is still that type parameter.
    fn without_annotation(&self, t: TypeId) -> TypeId {
        match self.data(t) {
            TypeData::Annotated { inner, .. } => *inner,
            _ => t,
        }
    }

    fn type_kind(&self, t: TypeId) -> Option<TypeKind> {
        match self.data(t) {
            TypeData::Def(_) | TypeData::Inst { .. } | TypeData::Annotated { .. } => self
                .definition(t)
                .and_then(|d| self.comp.type_def(d).ok())
                .map(|i| i.kind),
            _ => None,
        }
    }

    fn is_derived_from(&mut self, t: TypeId, base: TypeId) -> bool {
        let Some(base) = self.definition(base) else {
            return false;
        };
        let Some(def) = self.definition(t) else {
            return false;
        };
        let t = self.intern_def(def);
        self.all_base_types(t)
            .is_ok_and(|all| all.iter().any(|&b| self.definition(b) == Some(base)))
    }

    /// `ITypeParameter.EffectiveBaseClass` of a type parameter type; `None` for other types.
    pub fn type_parameter_effective_base_class(&mut self, t: TypeId) -> Option<TypeId> {
        let TypeData::TypeParam { owner, index } = *self.data(self.without_annotation(t)) else {
            return None;
        };
        Some(self.effective_base_class(owner, index, &mut Vec::new()))
    }

    /// `ITypeParameter.EffectiveBaseClass`; `busy` guards cyclic constraints.
    fn effective_base_class(
        &mut self,
        owner: TypeParamOwner,
        index: u32,
        busy: &mut Vec<(TypeParamOwner, u32)>,
    ) -> TypeId {
        if busy.contains(&(owner, index)) {
            return self.intern(TypeData::UnknownSpecial);
        }
        let Some(info) = self.type_parameter_info(owner, index) else {
            return self.known_type(KnownTypeCode::Object);
        };
        if info.value_type {
            return self.known_type(KnownTypeCode::ValueType);
        }
        busy.push((owner, index));
        let mut classes = Vec::new();
        for c in info.constraints {
            if self.type_kind(c) == Some(TypeKind::Class) {
                classes.push(c);
            } else if let TypeData::TypeParam { owner, index } =
                *self.data(self.without_annotation(c))
            {
                let base = self.effective_base_class(owner, index, busy);
                if self.type_kind(base) == Some(TypeKind::Class) {
                    classes.push(base);
                }
            }
        }
        busy.pop();
        let Some(&first) = classes.first() else {
            return self.known_type(KnownTypeCode::Object);
        };
        let mut result = first;
        for &c in &classes[1..] {
            if self.is_derived_from(c, result) {
                result = c;
            }
        }
        result
    }

    /// `ITypeParameter.IsReferenceType`.
    pub fn type_parameter_is_reference_type(
        &mut self,
        owner: TypeParamOwner,
        index: u32,
    ) -> Option<bool> {
        let info = self.type_parameter_info(owner, index)?;
        if info.value_type {
            return Some(false);
        }
        if info.reference_type {
            return Some(true);
        }
        let base = self.effective_base_class(owner, index, &mut Vec::new());
        match self.type_kind(base) {
            Some(TypeKind::Class | TypeKind::Delegate) => {
                let known = self
                    .definition(base)
                    .and_then(|d| self.comp.type_def(d).ok())
                    .map(|i| i.known);
                match known {
                    Some(
                        KnownTypeCode::Object | KnownTypeCode::ValueType | KnownTypeCode::Enum,
                    ) => None,
                    _ => Some(true),
                }
            }
            Some(TypeKind::Struct | TypeKind::Enum) => Some(false),
            _ => None,
        }
    }
}
