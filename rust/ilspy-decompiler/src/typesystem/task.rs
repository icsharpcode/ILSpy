//! `TypeSystem/TaskType.cs`: task and task-like types.

use super::compilation::TypeDefId;
use super::known::KnownTypeCode;
use super::types::{TypeData, TypeId, TypeSystem};

impl TypeSystem<'_, '_> {
    /// `TaskType.UnpackAnyTask`: the result type of `Task`, `Task<T>` and task-like types
    /// (`void` for the non-generic ones); any other type unchanged.
    pub fn unpack_any_task(&mut self, t: TypeId) -> TypeId {
        let Some(def) = self.definition(t) else {
            return t;
        };
        let Ok(info) = self.comp.type_def(def) else {
            return t;
        };
        // `IsTask` accepts `Task<T>` only as a `ParameterizedType`, not wrapped in a nullability
        // annotation.
        let is_task = match info.known {
            KnownTypeCode::Task => true,
            KnownTypeCode::TaskOfT => matches!(self.data(t), TypeData::Inst { .. }),
            _ => false,
        };
        if !is_task && !self.is_custom_task(def) {
            return t;
        }
        // A nullability annotation forwards `TypeArguments` to the annotated type.
        let unannotated = match *self.data(t) {
            TypeData::Annotated { inner, .. } => inner,
            _ => t,
        };
        match self.type_arguments(unannotated).first() {
            Some(&arg) => arg,
            None => self.known_type(KnownTypeCode::Void),
        }
    }

    /// `TaskType.IsCustomTask`: a type with at most one type parameter and an
    /// `[AsyncMethodBuilder(typeof(...))]` attribute.
    fn is_custom_task(&mut self, def: TypeDefId) -> bool {
        if self
            .comp
            .type_def(def)
            .map_or(true, |i| i.type_parameter_count > 1)
        {
            return false;
        }
        let Ok(attributes) = self.type_attributes(def) else {
            return false;
        };
        let Some(attribute) = attributes.iter().find(|a| {
            self.name_and_namespace(a.ty).is_some_and(|(name, ns)| {
                name == "AsyncMethodBuilderAttribute" && ns == "System.Runtime.CompilerServices"
            })
        }) else {
            return false;
        };
        attribute.fixed.len() == 1
            && self
                .definition(attribute.fixed[0].ty)
                .and_then(|d| self.comp.type_def(d).ok())
                .is_some_and(|i| i.known == KnownTypeCode::Type)
    }
}
