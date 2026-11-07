//! Inheritance: `IType.DirectBaseTypes` for every type, `GetAllBaseTypes` /
//! `GetNonInterfaceBaseTypes`, the modifiers and accessibility of methods, properties and
//! events, explicit interface implementations (MethodImpl rows), indexers
//! (`DefaultMemberName`) and `InheritanceHelper.GetBaseMembers`.

use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Error, Result};

use super::attrs::{
    ACC_NONE, ACC_PRIVATE, ACC_PROTECTED, ACC_PROTECTED_OR_INTERNAL, accessibility_value,
};
use super::attrvalue::ArgValue;
use super::compilation::{ModuleId, TypeDefId, TypeKind, is_known_type_name};
use super::members::{MethodDefId, SymbolKind, method_accessibility, method_modifiers};
use super::resolve::{MethodOrigin, ResolvedMethod};
use super::types::{TypeData, TypeId, TypeParamOwner, TypeSystem};

const METHOD_FINAL: u16 = 0x0020;
const METHOD_VIRTUAL: u16 = 0x0040;
const METHOD_ABSTRACT: u16 = 0x0400;

/// The modifiers `MetadataMethod` derives from its MethodAttributes (properties and events
/// take those of an accessor).
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct Modifiers {
    pub is_static: bool,
    pub is_abstract: bool,
    pub is_sealed: bool,
    pub is_virtual: bool,
    pub is_override: bool,
    pub is_overridable: bool,
}

impl Modifiers {
    pub fn of_method_flags(flags: u16) -> Self {
        let [is_static, is_abstract, is_sealed, is_virtual, is_override] = method_modifiers(flags);
        Modifiers {
            is_static,
            is_abstract,
            is_sealed,
            is_virtual,
            is_override,
            is_overridable: flags & (METHOD_ABSTRACT | METHOD_VIRTUAL) != 0
                && flags & METHOD_FINAL == 0,
        }
    }
}

/// A member definition: a Field, MethodDef, Property or Event row.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum MemberDef {
    Field(ModuleId, u32),
    Method(MethodDefId),
    Property(ModuleId, u32),
    Event(ModuleId, u32),
}

/// A member definition as seen through a (possibly parameterized) declaring type.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct MemberRef {
    pub def: MemberDef,
    pub declaring: TypeId,
}

/// `AccessibilityExtensions.Union`.
pub fn union_accessibility(a: u8, b: u8) -> u8 {
    let (a, b) = if a > b { (b, a) } else { (a, b) };
    if a == ACC_PROTECTED && b == super::attrs::ACC_INTERNAL {
        ACC_PROTECTED_OR_INTERNAL
    } else {
        b
    }
}

impl TypeSystem<'_, '_> {
    /// `IType.DirectBaseTypes`.
    pub fn type_direct_base_types(&mut self, t: TypeId) -> Result<Vec<TypeId>> {
        match self.data(t).clone() {
            TypeData::Def(def) => self.direct_base_types(def),
            TypeData::Inst { generic, args } => {
                let Some(def) = self.definition(generic) else {
                    return Ok(Vec::new());
                };
                let bases = self.direct_base_types(def)?;
                Ok(bases
                    .into_iter()
                    .map(|b| self.substitute(b, Some(&args), None))
                    .collect())
            }
            TypeData::Annotated { inner, .. } => self.type_direct_base_types(inner),
            TypeData::Tuple { underlying, .. } => self.type_direct_base_types(underlying),
            TypeData::TypeParam { owner, index } => {
                let (module, table, row) = match owner {
                    TypeParamOwner::Type(def) => (def.module, TableId::TypeDef, def.row),
                    TypeParamOwner::Method(m) => (m.module, TableId::MethodDef, m.row),
                    // shortcut: dummy type parameters report no base types; upgrade when a
                    // caller needs the `object` base `DummyTypeParameter` reports.
                    TypeParamOwner::DummyType | TypeParamOwner::DummyMethod => {
                        return Ok(Vec::new());
                    }
                };
                let tps = self.type_parameters(module, table, row)?;
                Ok(tps
                    .get(index as usize)
                    .map(|tp| tp.constraints.clone())
                    .unwrap_or_default())
            }
            // shortcut: arrays, pointers and the special types report no base types; upgrade
            // when member lookup on them is ported.
            _ => Ok(Vec::new()),
        }
    }

    /// `IType.GetAllBaseTypes()`: base types before derived ones, the type itself last.
    pub fn all_base_types(&mut self, t: TypeId) -> Result<Vec<TypeId>> {
        let mut out = Vec::new();
        self.collect_base_types(t, false, &mut Vec::new(), &mut out)?;
        Ok(out)
    }

    /// `IType.GetNonInterfaceBaseTypes()`.
    pub fn non_interface_base_types(&mut self, t: TypeId) -> Result<Vec<TypeId>> {
        let mut out = Vec::new();
        self.collect_base_types(t, true, &mut Vec::new(), &mut out)?;
        Ok(out)
    }

    /// `BaseTypeCollector.CollectBaseTypes`.
    fn collect_base_types(
        &mut self,
        t: TypeId,
        skip_interfaces: bool,
        active: &mut Vec<TypeId>,
        out: &mut Vec<TypeId>,
    ) -> Result<()> {
        let def = self.definition(t);
        let key = def.map_or(t, |d| self.intern_def(d));
        if active.contains(&key) {
            return Ok(());
        }
        active.push(key);
        if !out.contains(&t) {
            let skip = skip_interfaces
                && def.is_some_and(|d| {
                    self.comp
                        .type_def(d)
                        .is_ok_and(|i| i.kind != TypeKind::Interface)
                });
            for base in self.type_direct_base_types(t)? {
                if skip && self.is_interface(base) {
                    continue;
                }
                self.collect_base_types(base, skip_interfaces, active, out)?;
            }
            out.push(t);
        }
        active.pop();
        Ok(())
    }

    fn method_row(&self, m: MethodDefId) -> Result<rows::MethodDef> {
        self.comp.module(m.module).md.get::<rows::MethodDef>(m.row)
    }

    /// `IMethod` modifiers of a MethodDef.
    pub fn method_modifiers_of(&self, m: MethodDefId) -> Result<Modifiers> {
        Ok(Modifiers::of_method_flags(self.method_row(m)?.flags))
    }

    /// `IMethod.Accessibility` of a MethodDef, as the managed enum value.
    pub fn method_accessibility_of(&self, m: MethodDefId) -> Result<u8> {
        Ok(accessibility_value(method_accessibility(
            self.method_row(m)?.flags,
        )))
    }

    fn method_name(&self, m: MethodDefId) -> Result<String> {
        let md = &self.comp.module(m.module).md;
        Ok(md.string(self.method_row(m)?.name)?.into_owned())
    }

    /// `MetadataTypeDefinition.GetOverrides`: the MethodImpl declarations a method body
    /// implements, resolved in the declaring type's generic context.
    pub fn explicit_implementations(&mut self, m: MethodDefId) -> Result<Vec<ResolvedMethod>> {
        let Some(declaring) = self.method_declaring(m)? else {
            return Ok(Vec::new());
        };
        let comp = self.comp;
        let md = &comp.module(m.module).md;
        let ctx = self.type_context(Some(declaring));
        let mut result = Vec::new();
        for row in md.type_method_impls(declaring.row) {
            let imp = md.get::<rows::MethodImpl>(row)?;
            if imp.body.decode()? != (TableId::MethodDef, m.row) {
                continue;
            }
            result.push(self.resolve_method(m.module, imp.declaration.token()?, ctx)?);
        }
        Ok(result)
    }

    /// `IMethod.IsExplicitInterfaceImplementation`.
    pub fn is_explicit_interface_implementation(&mut self, m: MethodDefId) -> Result<bool> {
        if !self.method_name(m)?.contains('.') {
            return Ok(false);
        }
        let Some(declaring) = self.method_declaring(m)? else {
            return Ok(false);
        };
        let md = &self.comp.module(m.module).md;
        for row in md.type_method_impls(declaring.row) {
            if md.get::<rows::MethodImpl>(row)?.body.decode()? == (TableId::MethodDef, m.row) {
                return Ok(true);
            }
        }
        Ok(false)
    }

    /// `MetadataTypeDefinition.DefaultMemberName`: the string of the first
    /// `[DefaultMember]` with exactly one such fixed argument.
    pub fn default_member_name(&mut self, def: TypeDefId) -> Result<Option<String>> {
        let comp = self.comp;
        let md = &comp.module(def.module).md;
        for ca in md.custom_attributes(TableId::TypeDef, def.row) {
            let known = md.attribute_type(ca).is_ok_and(|t| {
                is_known_type_name(md, t, "System.Reflection", "DefaultMemberAttribute", 0)
            });
            if !known {
                continue;
            }
            let value = self.decode_attribute(def.module, ca)?;
            if let [arg] = value.fixed.as_slice()
                && let ArgValue::String(units) = &arg.value
            {
                return Ok(Some(String::from_utf16_lossy(units)));
            }
        }
        Ok(None)
    }

    /// The accessor `MetadataProperty.AnyAccessor` names: the getter, else the setter.
    pub fn property_any_accessor(&self, module: ModuleId, row: u32) -> Option<MethodDefId> {
        let (get, set, _) = self.comp.module(module).md.property_accessors(row);
        let any = if get != 0 { get } else { set };
        (any != 0).then_some(MethodDefId { module, row: any })
    }

    /// The accessor `MetadataEvent.AnyAccessor` names: adder, remover, else raiser.
    pub fn event_any_accessor(&self, module: ModuleId, row: u32) -> Option<MethodDefId> {
        let (add, remove, raise, _) = self.comp.module(module).md.event_accessors(row);
        [add, remove, raise]
            .into_iter()
            .find(|&r| r != 0)
            .map(|row| MethodDefId { module, row })
    }

    pub fn member_name(&self, member: MemberDef) -> Result<String> {
        let md = |module: ModuleId| &self.comp.module(module).md;
        Ok(match member {
            MemberDef::Field(module, row) => md(module)
                .string(md(module).get::<rows::Field>(row)?.name)?
                .into_owned(),
            MemberDef::Method(m) => self.method_name(m)?,
            MemberDef::Property(module, row) => md(module)
                .string(md(module).get::<rows::Property>(row)?.name)?
                .into_owned(),
            MemberDef::Event(module, row) => md(module)
                .string(md(module).get::<rows::Event>(row)?.name)?
                .into_owned(),
        })
    }

    /// The members `GetInterfaceMembersFromAccessor` yields: the accessor owners of the
    /// accessor's explicit implementations.
    fn interface_members_from_accessor(
        &mut self,
        accessor: Option<MethodDefId>,
    ) -> Result<Vec<MemberRef>> {
        let Some(accessor) = accessor else {
            return Ok(Vec::new());
        };
        let mut result = Vec::new();
        for imp in self.explicit_implementations(accessor)? {
            let MethodOrigin::Def(id) = imp.origin else {
                continue;
            };
            let Some((owner, _)) = self.accessor_owner(id) else {
                continue;
            };
            let row = owner & 0x00FF_FFFF;
            let def = match owner >> 24 {
                0x17 => MemberDef::Property(id.module, row),
                0x14 => MemberDef::Event(id.module, row),
                _ => continue,
            };
            let declaring = match imp.declaring {
                Some(d) => d,
                None => continue,
            };
            result.push(MemberRef { def, declaring });
        }
        Ok(result)
    }

    /// `IProperty.ExplicitlyImplementedInterfaceMembers`.
    pub fn property_explicit_implementations(
        &mut self,
        module: ModuleId,
        row: u32,
    ) -> Result<Vec<MemberRef>> {
        let any = self.property_any_accessor(module, row);
        self.interface_members_from_accessor(any)
    }

    /// `IEvent.ExplicitlyImplementedInterfaceMembers`.
    pub fn event_explicit_implementations(
        &mut self,
        module: ModuleId,
        row: u32,
    ) -> Result<Vec<MemberRef>> {
        let any = self.event_any_accessor(module, row);
        self.interface_members_from_accessor(any)
    }

    /// `IProperty.SymbolKind`: `Indexer` for the type's default member with parameters, the
    /// kind of the implemented property for an explicit implementation, else `Property`.
    pub fn property_symbol_kind(&mut self, module: ModuleId, row: u32) -> Result<SymbolKind> {
        self.property_symbol_kind_guarded(module, row, &mut Vec::new())
    }

    fn property_symbol_kind_guarded(
        &mut self,
        module: ModuleId,
        row: u32,
        busy: &mut Vec<(ModuleId, u32)>,
    ) -> Result<SymbolKind> {
        let name = self.member_name(MemberDef::Property(module, row))?;
        let declaring = self
            .property_any_accessor(module, row)
            .map(|a| self.method_declaring(a))
            .transpose()?
            .flatten();
        if let Some(declaring) = declaring
            && self.default_member_name(declaring)?.as_deref() == Some(name.as_str())
            && !self.property_signature(module, row)?.1.is_empty()
        {
            return Ok(SymbolKind::Indexer);
        }
        if name.contains('.') && !busy.contains(&(module, row)) {
            busy.push((module, row));
            let implemented = self.property_explicit_implementations(module, row)?;
            let kind = match implemented.first().map(|m| m.def) {
                Some(MemberDef::Property(m, r)) => self.property_symbol_kind_guarded(m, r, busy)?,
                _ => SymbolKind::Property,
            };
            busy.pop();
            return Ok(kind);
        }
        Ok(SymbolKind::Property)
    }

    /// Modifiers of a property or event (`AnyAccessor`'s; none without accessors).
    pub fn member_modifiers(&self, member: MemberDef) -> Result<Modifiers> {
        let accessor = match member {
            MemberDef::Method(m) => Some(m),
            MemberDef::Property(module, row) => self.property_any_accessor(module, row),
            MemberDef::Event(module, row) => self.event_any_accessor(module, row),
            MemberDef::Field(..) => return Err(Error::Unsupported("field modifiers")),
        };
        accessor.map_or(Ok(Modifiers::default()), |a| self.method_modifiers_of(a))
    }

    /// `IEntity.Accessibility` of a member.
    pub fn member_accessibility(&mut self, member: MemberDef) -> Result<u8> {
        match member {
            MemberDef::Field(module, row) => {
                let md = &self.comp.module(module).md;
                Ok(accessibility_value(super::members::field_accessibility(
                    md.get::<rows::Field>(row)?.flags,
                )))
            }
            MemberDef::Method(m) => self.method_accessibility_of(m),
            MemberDef::Event(module, row) => match self.event_any_accessor(module, row) {
                Some(a) => self.method_accessibility_of(a),
                None => Ok(ACC_NONE),
            },
            MemberDef::Property(module, row) => self.property_accessibility(module, row),
        }
    }

    /// `MetadataProperty.ComputeAccessibility`: an override of one accessor copies the
    /// accessibility of the nearest non-override base property (`protected internal` across
    /// modules becomes `protected`); otherwise the union of the accessors'.
    fn property_accessibility(&mut self, module: ModuleId, row: u32) -> Result<u8> {
        let (get, set, _) = self.comp.module(module).md.property_accessors(row);
        let member = MemberDef::Property(module, row);
        if self.member_modifiers(member)?.is_override && (get == 0 || set == 0) {
            for base in self.base_members(member, false)? {
                if !self.member_modifiers(base.def)?.is_override {
                    let acc = self.member_accessibility(base.def)?;
                    let base_module = match base.def {
                        MemberDef::Field(m, _)
                        | MemberDef::Property(m, _)
                        | MemberDef::Event(m, _) => m,
                        MemberDef::Method(m) => m.module,
                    };
                    return Ok(
                        if acc == ACC_PROTECTED_OR_INTERNAL && base_module != module {
                            ACC_PROTECTED
                        } else {
                            acc
                        },
                    );
                }
            }
        }
        let of = |ts: &Self, r: u32| {
            if r == 0 {
                Ok(ACC_NONE)
            } else {
                ts.method_accessibility_of(MethodDefId { module, row: r })
            }
        };
        Ok(union_accessibility(of(self, get)?, of(self, set)?))
    }

    /// `ISymbol.SymbolKind` of a member definition.
    pub fn member_symbol_kind(&mut self, member: MemberDef) -> Result<SymbolKind> {
        Ok(match member {
            MemberDef::Field(..) => SymbolKind::Field,
            MemberDef::Method(m) => {
                let owner = self.accessor_owner(m);
                self.method_symbol_kind(m, owner)?
            }
            MemberDef::Property(module, row) => self.property_symbol_kind(module, row)?,
            MemberDef::Event(..) => SymbolKind::Event,
        })
    }

    /// Parameter types of a method or property, seen through `declaring` (its type arguments
    /// substituted for the class type parameters).
    fn member_parameter_types(
        &mut self,
        member: MemberDef,
        declaring: TypeId,
    ) -> Result<Option<Vec<TypeId>>> {
        let params = match member {
            MemberDef::Method(m) => self.method_signature(m).1,
            MemberDef::Property(module, row) => self.property_signature(module, row)?.1,
            MemberDef::Field(..) | MemberDef::Event(..) => return Ok(None),
        };
        let args = match self.data(declaring) {
            TypeData::Inst { args, .. } => Some(args.to_vec()),
            _ => None,
        };
        Ok(Some(
            params
                .iter()
                .map(|p| self.substitute(p.ty, args.as_deref(), None))
                .collect(),
        ))
    }

    fn method_type_parameter_count(&self, member: MemberDef) -> Option<usize> {
        match member {
            MemberDef::Method(m) => Some(
                self.comp
                    .module(m.module)
                    .md
                    .generic_params(TableId::MethodDef, m.row)
                    .len(),
            ),
            _ => None,
        }
    }

    /// `SignatureComparer.Ordinal.Equals`.
    fn same_signature(&mut self, a: &MemberRef, b: &MemberRef) -> Result<bool> {
        if self.member_symbol_kind(a.def)? != self.member_symbol_kind(b.def)?
            || self.member_name(a.def)? != self.member_name(b.def)?
        {
            return Ok(false);
        }
        let (Some(pa), Some(pb)) = (
            self.member_parameter_types(a.def, a.declaring)?,
            self.member_parameter_types(b.def, b.declaring)?,
        ) else {
            return Ok(true);
        };
        if let (Some(ta), Some(tb)) = (
            self.method_type_parameter_count(a.def),
            self.method_type_parameter_count(b.def),
        ) && ta != tb
        {
            return Ok(false);
        }
        if pa.len() != pb.len() {
            return Ok(false);
        }
        for (x, y) in pa.into_iter().zip(pb) {
            if self.normalize_parameter(x) != self.normalize_parameter(y) {
                return Ok(false);
            }
        }
        Ok(true)
    }

    fn member_declaring_def(&mut self, member: MemberDef) -> Result<Option<TypeDefId>> {
        let accessor = match member {
            MemberDef::Method(m) => Some(m),
            MemberDef::Property(module, row) => self.property_any_accessor(module, row),
            MemberDef::Event(module, row) => self.event_any_accessor(module, row),
            MemberDef::Field(module, row) => {
                let declaring = self.comp.module(module).md.field_declaring_type(row)?;
                return Ok((declaring != 0).then_some(TypeDefId {
                    module,
                    row: declaring,
                }));
            }
        };
        match accessor {
            Some(a) => self.method_declaring(a),
            None => Ok(None),
        }
    }

    /// The members of `t` (not inherited) named `name` with accessibility above private:
    /// `GetAccessors` when `accessors`, else `GetMembers` (fields, methods, properties,
    /// events of the definition, seen through `t`).
    fn declared_members(
        &mut self,
        t: TypeId,
        name: &str,
        accessors: bool,
    ) -> Result<Vec<MemberRef>> {
        let Some(def) = self.definition(t) else {
            return Ok(Vec::new());
        };
        let comp = self.comp;
        let md = &comp.module(def.module).md;
        let mut candidates = Vec::new();
        if accessors {
            for p in comp.visible_properties(def)? {
                let (get, set, _) = md.property_accessors(p);
                candidates.extend([get, set].into_iter().filter(|&r| r != 0));
            }
            for e in comp.visible_events(def)? {
                let (add, remove, raise, _) = md.event_accessors(e);
                candidates.extend([add, remove, raise].into_iter().filter(|&r| r != 0));
            }
            let mut result = Vec::new();
            for row in candidates {
                let m = MemberDef::Method(MethodDefId {
                    module: def.module,
                    row,
                });
                if self.member_name(m)? == name && self.member_accessibility(m)? > ACC_PRIVATE {
                    result.push(MemberRef {
                        def: m,
                        declaring: t,
                    });
                }
            }
            return Ok(result);
        }
        let mut members: Vec<MemberDef> = comp
            .visible_fields(def)?
            .into_iter()
            .map(|r| MemberDef::Field(def.module, r))
            .collect();
        for row in md.type_methods(def.row)? {
            let m = MethodDefId {
                module: def.module,
                row,
            };
            if self.accessor_owner(m).is_none() && comp.is_visible_method(def.module, row) {
                members.push(MemberDef::Method(m));
            }
        }
        members.extend(
            comp.visible_properties(def)?
                .into_iter()
                .map(|r| MemberDef::Property(def.module, r)),
        );
        members.extend(
            comp.visible_events(def)?
                .into_iter()
                .map(|r| MemberDef::Event(def.module, r)),
        );
        let mut result = Vec::new();
        for m in members {
            if self.member_name(m)? == name && self.member_accessibility(m)? > ACC_PRIVATE {
                result.push(MemberRef {
                    def: m,
                    declaring: t,
                });
            }
        }
        Ok(result)
    }

    /// `InheritanceHelper.GetBaseMembers(member, includeImplementedInterfaces)` for a member
    /// definition: same-signature members of the base types, derived-most first.
    pub fn base_members(
        &mut self,
        member: MemberDef,
        include_interfaces: bool,
    ) -> Result<Vec<MemberRef>> {
        let mut result = Vec::new();
        let mut member = member;
        if include_interfaces {
            let implemented = match member {
                MemberDef::Method(m) if self.is_explicit_interface_implementation(m)? => self
                    .explicit_implementations(m)?
                    .into_iter()
                    .filter_map(|r| match (r.origin, r.declaring) {
                        (MethodOrigin::Def(id), Some(declaring)) => Some(MemberRef {
                            def: MemberDef::Method(id),
                            declaring,
                        }),
                        _ => None,
                    })
                    .collect(),
                MemberDef::Property(module, row)
                    if self
                        .property_any_accessor(module, row)
                        .map(|a| self.is_explicit_interface_implementation(a))
                        .transpose()?
                        .unwrap_or(false) =>
                {
                    self.property_explicit_implementations(module, row)?
                }
                MemberDef::Event(module, row)
                    if self
                        .event_any_accessor(module, row)
                        .map(|a| self.is_explicit_interface_implementation(a))
                        .transpose()?
                        .unwrap_or(false) =>
                {
                    self.event_explicit_implementations(module, row)?
                }
                _ => Vec::new(),
            };
            if let [single] = implemented.as_slice() {
                member = single.def;
                result.push(single.clone());
            }
        }
        let Some(declaring) = self.member_declaring_def(member)? else {
            return Ok(result);
        };
        let declaring_type = self.intern_def(declaring);
        let bases = if include_interfaces {
            self.all_base_types(declaring_type)?
        } else {
            self.non_interface_base_types(declaring_type)?
        };
        let this = MemberRef {
            def: member,
            declaring: declaring_type,
        };
        let name = self.member_name(member)?;
        let accessors = self.member_symbol_kind(member)? == SymbolKind::Accessor;
        for base in bases.into_iter().rev() {
            if base == declaring_type {
                continue;
            }
            for candidate in self.declared_members(base, &name, accessors)? {
                if self.same_signature(&this, &candidate)? {
                    result.push(candidate);
                }
            }
        }
        Ok(result)
    }
}
