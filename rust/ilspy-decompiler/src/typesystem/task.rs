//! `TypeSystem/TaskType.cs`: task and task-like types.

use super::attrvalue::ArgValue;
use super::compilation::TypeDefId;
use super::known::KnownTypeCode;
use super::names::{FullTypeName, TopLevelTypeName};
use super::reflection::full_type_name_from_reflection;
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
        if !is_task && self.custom_task_builder(def).is_none() {
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
    /// `[AsyncMethodBuilder(typeof(...))]` attribute. The builder type, `Some(None)` for a
    /// null argument.
    pub(crate) fn custom_task_builder(&mut self, def: TypeDefId) -> Option<Option<TypeId>> {
        if self
            .comp
            .type_def(def)
            .map_or(true, |i| i.type_parameter_count > 1)
        {
            return None;
        }
        let attributes = self.type_attributes(def).ok()?;
        let attribute = attributes.iter().find(|a| {
            self.name_and_namespace(a.ty).is_some_and(|(name, ns)| {
                name == "AsyncMethodBuilderAttribute" && ns == "System.Runtime.CompilerServices"
            })
        })?;
        let is_type = attribute.fixed.len() == 1
            && self
                .definition(attribute.fixed[0].ty)
                .and_then(|d| self.comp.type_def(d).ok())
                .is_some_and(|i| i.known == KnownTypeCode::Type);
        if !is_type {
            return None;
        }
        Some(match attribute.fixed[0].value {
            ArgValue::Type(t) => Some(t),
            _ => None,
        })
    }

    /// `IType.IsKnownType(code)`: for a generic known type, any parameterization.
    pub fn is_known(&self, t: TypeId, code: KnownTypeCode) -> bool {
        self.definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .is_some_and(|i| i.known == code)
    }

    /// The builder of a custom task type, when its reflection name has `tpc` type
    /// parameters.
    fn custom_task_builder_name(&mut self, t: TypeId, tpc: u32) -> Option<FullTypeName> {
        let def = self.definition(t)?;
        // shortcut: a null builder argument throws in the managed code; it is no task type here
        let builder = self.custom_task_builder(def)??;
        let name = full_type_name_from_reflection(&self.reflection_name(builder));
        (name.type_parameter_count() == tpc).then_some(name)
    }

    /// `TaskType.IsNonGenericTaskType`: the builder type's name.
    pub fn non_generic_task_builder(&mut self, t: TypeId) -> Option<FullTypeName> {
        if self.is_known(t, KnownTypeCode::Task) {
            return Some(FullTypeName::top_level(TopLevelTypeName::new(
                COMPILER_SERVICES,
                "AsyncTaskMethodBuilder",
                0,
            )));
        }
        self.custom_task_builder_name(t, 0)
    }

    /// `TaskType.IsGenericTaskType`: the builder type's name.
    pub fn generic_task_builder(&mut self, t: TypeId) -> Option<FullTypeName> {
        if self.is_known(t, KnownTypeCode::TaskOfT) {
            return Some(FullTypeName::top_level(TopLevelTypeName::new(
                COMPILER_SERVICES,
                "AsyncTaskMethodBuilder",
                1,
            )));
        }
        self.custom_task_builder_name(t, 1)
    }
}

const COMPILER_SERVICES: &str = "System.Runtime.CompilerServices";
