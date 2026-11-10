//! Entity flags that depend on `TypeSystemOptions` and well-known attributes: ref structs,
//! readonly structs and members, extension methods, init-only setters, `params`, and the
//! `scoped` lifetime rules.

use crate::metadata::Metadata;
use crate::metadata::custom_attribute::Value;
use crate::metadata::tables::{TableId, rows};

use super::compilation::{TypeDefId, TypeKind, is_known_type_name};
use super::members::MethodDefId;
use super::options;
use super::types::{Nullability, TypeData, TypeId, TypeParamOwner, TypeSystem};

const COMPILER_SERVICES: &str = "System.Runtime.CompilerServices";
const CODE_ANALYSIS: &str = "System.Diagnostics.CodeAnalysis";

/// `ScopedKind`.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum ScopedKind {
    None,
    ScopedRef,
    ScopedValue,
}

impl ScopedKind {
    pub fn name(self) -> &'static str {
        match self {
            ScopedKind::None => "None",
            ScopedKind::ScopedRef => "ScopedRef",
            ScopedKind::ScopedValue => "ScopedValue",
        }
    }
}

/// Whether `table`/`row` carries the attribute `ns.name`.
pub fn has_attribute(md: &Metadata<'_>, table: TableId, row: u32, ns: &str, name: &str) -> bool {
    md.custom_attributes(table, row).into_iter().any(|ca| {
        md.attribute_type(ca)
            .is_ok_and(|t| is_known_type_name(md, t, ns, name, 0))
    })
}

pub fn has_compiler_attribute(md: &Metadata<'_>, table: TableId, row: u32, name: &str) -> bool {
    has_attribute(md, table, row, COMPILER_SERVICES, name)
}

pub fn has_unscoped_ref(md: &Metadata<'_>, table: TableId, row: u32) -> bool {
    has_attribute(md, table, row, CODE_ANALYSIS, "UnscopedRefAttribute")
}

/// `MetadataModule.UseUpdatedEscapeRules`: `[module: RefSafetyRules(11)]`.
pub fn uses_updated_escape_rules(md: &Metadata<'_>) -> bool {
    for ca in md.custom_attributes(TableId::Module, 1) {
        if !md.attribute_type(ca).is_ok_and(|t| {
            is_known_type_name(md, t, COMPILER_SERVICES, "RefSafetyRulesAttribute", 0)
        }) {
            continue;
        }
        let Ok(attribute) = md.get::<rows::CustomAttribute>(ca) else {
            continue;
        };
        let resolver = crate::metadata::custom_attribute::ModuleResolver::new(md);
        let Ok(value) = crate::metadata::custom_attribute::decode(md, &attribute, &resolver) else {
            continue;
        };
        if let [Value::I32(version)] = value.fixed.as_slice() {
            return *version == 11;
        }
    }
    false
}

#[derive(Clone, Copy, Debug, Default)]
pub struct TypeFlags {
    pub by_ref_like: bool,
    pub read_only: bool,
    pub has_extensions: bool,
    pub is_static: bool,
}

#[derive(Clone, Copy, Debug)]
pub struct MethodFlags {
    pub extension: bool,
    pub this_read_only: bool,
    pub return_read_only: bool,
    pub init_only: bool,
    pub this_scope: ScopedKind,
}

impl TypeSystem<'_, '_> {
    /// `IsByRefLike`, `IsReadOnly`, `HasExtensions` and `IsStatic` of a type definition.
    pub fn type_flags(&self, def: TypeDefId) -> TypeFlags {
        let md = &self.comp.module(def.module).md;
        let Ok(info) = self.comp.type_def(def) else {
            return TypeFlags::default();
        };
        let opts = self.options(def.module);
        let is_static = info.flags & 0x0180 == 0x0180;
        let is_struct = info.kind == TypeKind::Struct;
        TypeFlags {
            by_ref_like: is_struct
                && opts & options::REF_STRUCTS != 0
                && has_compiler_attribute(md, TableId::TypeDef, def.row, "IsByRefLikeAttribute"),
            read_only: is_struct
                && opts & options::READ_ONLY_STRUCTS_AND_PARAMETERS != 0
                && has_compiler_attribute(md, TableId::TypeDef, def.row, "IsReadOnlyAttribute"),
            has_extensions: info.kind == TypeKind::Class
                && is_static
                && opts & options::EXTENSION_METHODS != 0
                && has_compiler_attribute(md, TableId::TypeDef, def.row, "ExtensionAttribute"),
            is_static,
        }
    }

    /// `IType.IsByRefLike`.
    pub fn is_by_ref_like(&self, t: TypeId) -> bool {
        match self.data(t) {
            TypeData::Def(def) => self.type_flags(*def).by_ref_like,
            TypeData::Inst { generic, .. } => self.is_by_ref_like(*generic),
            TypeData::ByRef(_) => true,
            TypeData::Annotated { inner, .. }
            | TypeData::Modified { inner, .. }
            | TypeData::Pinned(inner) => self.is_by_ref_like(*inner),
            _ => false,
        }
    }

    /// `ITypeParameter.AllowsRefLikeType` of a type parameter type.
    pub fn allows_ref_like(&self, t: TypeId) -> bool {
        match self.data(t) {
            TypeData::Annotated { inner, .. } => self.allows_ref_like(*inner),
            TypeData::TypeParam { owner, index } => {
                let (module, table, row) = match owner {
                    TypeParamOwner::Type(def) => (def.module, TableId::TypeDef, def.row),
                    TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => {
                        (m.module, TableId::MethodDef, m.row)
                    }
                    _ => return false,
                };
                let md = &self.comp.module(module).md;
                md.generic_params(table, row)
                    .nth(*index as usize)
                    .and_then(|gp| md.get::<rows::GenericParam>(gp).ok())
                    .is_some_and(|gp| gp.flags & 0x0020 != 0)
            }
            _ => false,
        }
    }

    /// `IsRefLikeOrAllowsRefLikeType`.
    pub fn is_ref_like_or_allows(&self, t: TypeId) -> bool {
        self.is_by_ref_like(t) || self.allows_ref_like(t)
    }

    /// The flags of a method definition; `property` is the property it is an accessor of.
    pub fn method_flags(&mut self, m: MethodDefId, accessor_owner: Option<u32>) -> MethodFlags {
        let comp = self.comp;
        let md = &comp.module(m.module).md;
        let opts = self.options(m.module);
        let Ok(def) = md.get::<rows::MethodDef>(m.row) else {
            return MethodFlags {
                extension: false,
                this_read_only: false,
                return_read_only: false,
                init_only: false,
                this_scope: ScopedKind::None,
            };
        };
        let is_static = def.flags & 0x0010 != 0;
        let declaring_row = md.method_declaring_type(m.row).unwrap_or(0);
        let declaring = (declaring_row != 0).then_some(TypeDefId {
            module: m.module,
            row: declaring_row,
        });
        let return_param = md
            .method_params(m.row)
            .ok()
            .and_then(|p| p.first().copied())
            .filter(|&p| md.get::<rows::Param>(p).is_ok_and(|p| p.sequence == 0));
        let this_scope = if is_static
            || declaring
                .and_then(|d| comp.type_def(d).ok())
                .map(|i| i.kind)
                != Some(TypeKind::Struct)
            || opts & options::SCOPED_REF == 0
        {
            ScopedKind::None
        } else {
            let unscoped = has_unscoped_ref(md, TableId::MethodDef, m.row)
                || accessor_owner
                    .filter(|t| t >> 24 == TableId::Property as u32)
                    .is_some_and(|t| has_unscoped_ref(md, TableId::Property, t & 0x00FF_FFFF));
            if unscoped && uses_updated_escape_rules(md) {
                ScopedKind::None
            } else {
                ScopedKind::ScopedRef
            }
        };
        MethodFlags {
            extension: is_static
                && opts & options::EXTENSION_METHODS != 0
                && has_compiler_attribute(md, TableId::MethodDef, m.row, "ExtensionAttribute"),
            this_read_only: declaring.is_some_and(|d| self.type_flags(d).read_only)
                || opts & options::READ_ONLY_METHODS != 0
                    && has_compiler_attribute(md, TableId::MethodDef, m.row, "IsReadOnlyAttribute"),
            return_read_only: return_param.is_some_and(|p| {
                has_compiler_attribute(md, TableId::Param, p, "IsReadOnlyAttribute")
            }),
            init_only: self.method_init_only(m),
            this_scope,
        }
    }

    /// `MetadataMethod.IsInitOnly`: the raw return type's outermost modifier is
    /// `IsExternalInit`.
    fn method_init_only(&mut self, m: MethodDefId) -> bool {
        let result = (|| {
            let md = &self.comp.module(m.module).md;
            let def = md.get::<rows::MethodDef>(m.row)?;
            let sig = crate::metadata::signature::decode_method(
                &mut crate::metadata::bytes::Bytes::new(md.blob(def.signature)?),
            )?;
            let ctx = self.method_context(m)?;
            // The whole signature decodes first; a bad one leaves the flag unset.
            for p in &sig.params {
                self.from_sig_raw(m.module, p, ctx)?;
            }
            let ret = self.from_sig_raw(m.module, &sig.ret, ctx)?;
            Ok::<_, crate::metadata::Error>(match self.data(ret).clone() {
                TypeData::Modified { modifier, .. } => self
                    .name_and_namespace(modifier)
                    .is_some_and(|(n, ns)| n == "IsExternalInit" && ns == COMPILER_SERVICES),
                _ => false,
            })
        })();
        result.unwrap_or(false)
    }

    /// `MetadataTypeParameter.NullabilityConstraint`.
    pub fn nullability_constraint(
        &self,
        owner: TypeParamOwner,
        gp_row: u32,
        module: super::compilation::ModuleId,
    ) -> Nullability {
        let md = &self.comp.module(module).md;
        let owner_options = match owner {
            TypeParamOwner::Type(def) => self.type_options_of(def),
            TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => {
                self.method_options_of(m)
            }
            _ => self.options(module),
        };
        if owner_options & options::NULLABILITY == 0 {
            return Nullability::Oblivious;
        }
        for ca in md.custom_attributes(TableId::GenericParam, gp_row) {
            if !md
                .attribute_type(ca)
                .is_ok_and(|t| is_known_type_name(md, t, COMPILER_SERVICES, "NullableAttribute", 0))
            {
                continue;
            }
            let Ok(attribute) = md.get::<rows::CustomAttribute>(ca) else {
                continue;
            };
            let resolver = crate::metadata::custom_attribute::ModuleResolver::new(md);
            if let Ok(value) = crate::metadata::custom_attribute::decode(md, &attribute, &resolver)
                && let [Value::U8(b)] = value.fixed.as_slice()
                && let Some(n) = Nullability::from_byte(*b)
            {
                return n;
            }
        }
        match owner {
            TypeParamOwner::Method(m) | TypeParamOwner::ContextMethod { method: m, .. } => self
                .method_nullable_context(m)
                .unwrap_or(Nullability::Oblivious),
            TypeParamOwner::Type(def) => self.type_nullable_context(def),
            _ => Nullability::Oblivious,
        }
    }
}

impl TypeSystem<'_, '_> {
    /// `IField.IsReadOnly`: an `initonly` field.
    pub fn field_is_read_only(&self, field: &super::resolve::ResolvedField) -> bool {
        let super::resolve::FieldOrigin::Def { module, row } = field.origin else {
            return false;
        };
        let md = &self.comp.module(module).md;
        md.get::<rows::Field>(row)
            .is_ok_and(|f| f.flags & 0x0020 != 0)
    }

    /// `IField.ReturnTypeIsRefReadOnly`: a `ref readonly` field (`[IsReadOnly]`).
    pub fn field_return_type_is_ref_read_only(
        &self,
        field: &super::resolve::ResolvedField,
    ) -> bool {
        let super::resolve::FieldOrigin::Def { module, row } = field.origin else {
            return false;
        };
        let md = &self.comp.module(module).md;
        has_compiler_attribute(md, TableId::Field, row, "IsReadOnlyAttribute")
    }

    /// `IMethod.ReturnTypeIsRefReadOnly`.
    pub fn method_return_type_is_ref_read_only(
        &mut self,
        m: &super::resolve::ResolvedMethod,
    ) -> bool {
        let super::resolve::MethodOrigin::Def(id) = m.origin else {
            return false;
        };
        let owner = self.accessor_owner(id).map(|(o, _)| o);
        self.method_flags(id, owner).return_read_only
    }
}
