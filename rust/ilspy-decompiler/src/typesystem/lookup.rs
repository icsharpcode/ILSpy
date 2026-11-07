//! Member lookup on types (`IType.GetMethods` / `GetConstructors` / `GetAccessors` /
//! `GetProperties` / `GetFields` / `GetEvents` / `GetNestedTypes`, `GetMembersHelper`):
//! members of the type and, unless inherited members are ignored, of its non-interface base
//! types (base types first), seen through parameterized types' type arguments.

use crate::metadata::Result;

use super::compilation::{TypeDefId, TypeKind};
use super::inheritance::{MemberDef, MemberRef};
use super::members::SymbolKind;
use super::resolve::ResolvedMethod;
use super::types::{TypeData, TypeId, TypeSystem};

/// `GetMemberOptions`.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct MemberOptions {
    /// `IgnoreInheritedMembers`.
    pub ignore_inherited: bool,
    /// `ReturnMemberDefinitions`.
    pub definitions: bool,
}

impl MemberOptions {
    pub const DECLARED: MemberOptions = MemberOptions {
        ignore_inherited: true,
        definitions: false,
    };
}

impl TypeSystem<'_, '_> {
    /// The types whose declared members a lookup visits.
    fn lookup_types(&mut self, t: TypeId, options: MemberOptions) -> Result<Vec<TypeId>> {
        if options.ignore_inherited {
            Ok(vec![t])
        } else {
            self.non_interface_base_types(t)
        }
    }

    /// `lookup_types` for member lookups, which find nothing on `void` itself (nested type
    /// lookups still see its base types).
    fn member_lookup_types(&mut self, t: TypeId, options: MemberOptions) -> Result<Vec<TypeId>> {
        let void = matches!(self.data(t), TypeData::Def(def)
            if self.comp.type_def(*def).is_ok_and(|i| i.kind == TypeKind::Void));
        if void {
            return Ok(Vec::new());
        }
        self.lookup_types(t, options)
    }

    /// The type definition behind a definition or parameterized type, `None` for `void` (which
    /// has no members) and for every other kind of type.
    fn member_source(&self, t: TypeId) -> Option<TypeDefId> {
        // shortcut: type parameters, arrays and the special types report no members; upgrade
        // when a caller looks up members on them.
        let def = match self.data(t) {
            TypeData::Def(def) => *def,
            TypeData::Inst { generic, .. } => self.definition(*generic)?,
            _ => return None,
        };
        let void = self
            .comp
            .type_def(def)
            .is_ok_and(|i| i.kind == TypeKind::Void);
        (!void).then_some(def)
    }

    fn class_arguments(&self, t: TypeId) -> Option<Vec<TypeId>> {
        match self.data(t) {
            TypeData::Inst { args, .. } => Some(args.to_vec()),
            _ => None,
        }
    }

    /// `IType.GetMethods(typeArguments)`: non-constructor methods; with type arguments only
    /// methods with that many type parameters, specialized by them.
    pub fn get_methods(
        &mut self,
        t: TypeId,
        type_arguments: Option<&[TypeId]>,
        options: MemberOptions,
    ) -> Result<Vec<ResolvedMethod>> {
        let method_args = type_arguments.filter(|a| !a.is_empty());
        let mut result = Vec::new();
        for base in self.member_lookup_types(t, options)? {
            let Some(def) = self.member_source(base) else {
                continue;
            };
            let class = self.class_arguments(base);
            for (m, kind) in self.definition_methods(def)? {
                if kind == SymbolKind::Constructor {
                    continue;
                }
                if let Some(args) = method_args
                    && m.type_args.len() != args.len()
                {
                    continue;
                }
                if !options.definitions && (class.is_some() || method_args.is_some()) {
                    result.push(self.specialize(m, class.clone(), method_args.map(<[_]>::to_vec)));
                } else {
                    result.push(m);
                }
            }
        }
        Ok(result)
    }

    /// `IType.GetConstructors()` (by default only the type's own).
    pub fn get_constructors(
        &mut self,
        t: TypeId,
        options: MemberOptions,
    ) -> Result<Vec<ResolvedMethod>> {
        let mut result = Vec::new();
        for base in self.member_lookup_types(t, options)? {
            let Some(def) = self.member_source(base) else {
                continue;
            };
            let class = self.class_arguments(base);
            for m in self.definition_constructors(def)? {
                result.push(self.specialize_as_declared(m, base, class.clone(), options));
            }
        }
        Ok(result)
    }

    /// `IType.GetAccessors()`; `None` entries are the managed code's null removers.
    pub fn get_accessors(
        &mut self,
        t: TypeId,
        options: MemberOptions,
    ) -> Result<Vec<Option<ResolvedMethod>>> {
        let mut result = Vec::new();
        for base in self.member_lookup_types(t, options)? {
            let Some(def) = self.member_source(base) else {
                continue;
            };
            let class = self.class_arguments(base);
            for m in self.definition_accessors(def, None)? {
                result
                    .push(m.map(|m| self.specialize_as_declared(m, base, class.clone(), options)));
            }
        }
        Ok(result)
    }

    /// `GetConstructorsOrAccessorsImpl`: through a parameterized type, specialized by its
    /// arguments and declared by it.
    fn specialize_as_declared(
        &mut self,
        m: ResolvedMethod,
        base: TypeId,
        class: Option<Vec<TypeId>>,
        options: MemberOptions,
    ) -> ResolvedMethod {
        match class {
            Some(args) if !options.definitions => {
                let mut m = self.specialize(m, Some(args), None);
                m.declaring = Some(base);
                m
            }
            _ => m,
        }
    }

    fn member_refs(
        &mut self,
        t: TypeId,
        options: MemberOptions,
        members: impl Fn(&Self, TypeDefId) -> Result<Vec<MemberDef>>,
    ) -> Result<Vec<MemberRef>> {
        let mut result = Vec::new();
        for base in self.member_lookup_types(t, options)? {
            let Some(def) = self.member_source(base) else {
                continue;
            };
            let declaring = if options.definitions {
                self.intern_def(def)
            } else {
                base
            };
            result.extend(
                members(self, def)?
                    .into_iter()
                    .map(|def| MemberRef { def, declaring }),
            );
        }
        Ok(result)
    }

    /// `IType.GetProperties()`.
    pub fn get_properties(&mut self, t: TypeId, options: MemberOptions) -> Result<Vec<MemberRef>> {
        self.member_refs(t, options, |ts, def| {
            Ok(ts
                .comp
                .visible_properties(def)?
                .into_iter()
                .map(|r| MemberDef::Property(def.module, r))
                .collect())
        })
    }

    /// `IType.GetFields()`.
    pub fn get_fields(&mut self, t: TypeId, options: MemberOptions) -> Result<Vec<MemberRef>> {
        self.member_refs(t, options, |ts, def| {
            Ok(ts
                .comp
                .visible_fields(def)?
                .into_iter()
                .map(|r| MemberDef::Field(def.module, r))
                .collect())
        })
    }

    /// `IType.GetEvents()`.
    pub fn get_events(&mut self, t: TypeId, options: MemberOptions) -> Result<Vec<MemberRef>> {
        self.member_refs(t, options, |ts, def| {
            Ok(ts
                .comp
                .visible_events(def)?
                .into_iter()
                .map(|r| MemberDef::Event(def.module, r))
                .collect())
        })
    }

    /// `IType.GetNestedTypes(typeArguments)`: a generic nested type found through an outer
    /// type takes the outer type's arguments (or its own type parameters) followed by
    /// `type_arguments`, or unbound arguments without them.
    pub fn get_nested_types(
        &mut self,
        t: TypeId,
        type_arguments: Option<&[TypeId]>,
        options: MemberOptions,
    ) -> Result<Vec<TypeId>> {
        // A type definition's own nested types are returned as definitions.
        if let TypeData::Def(def) = *self.data(t)
            && options.ignore_inherited
            && type_arguments.is_none()
        {
            let nested = self.comp.nested_types(def);
            return Ok(nested.into_iter().map(|n| self.intern_def(n)).collect());
        }
        let mut result = Vec::new();
        for base in self.lookup_types(t, options)? {
            let Some(outer) = self.definition(base) else {
                continue;
            };
            let outer_count = self.comp.type_def(outer)?.type_parameter_count as usize;
            let class = self.class_arguments(base);
            for nested in self.comp.nested_types(outer) {
                let total = self.comp.type_def(nested)?.type_parameter_count as usize;
                if let Some(args) = type_arguments
                    && total.checked_sub(outer_count) != Some(args.len())
                {
                    continue;
                }
                let generic = self.intern_def(nested);
                if total == 0 || options.definitions {
                    result.push(generic);
                    continue;
                }
                let mut args = Vec::with_capacity(total);
                for i in 0..outer_count.min(total) {
                    args.push(match &class {
                        Some(c) => c
                            .get(i)
                            .copied()
                            .unwrap_or_else(|| self.intern(TypeData::UnknownSpecial)),
                        None => {
                            let owner = self.class_param_owner(outer, i as u32);
                            self.intern(TypeData::TypeParam {
                                owner,
                                index: i as u32,
                            })
                        }
                    });
                }
                for i in outer_count..total {
                    args.push(match type_arguments {
                        Some(a) => a[i - outer_count],
                        None => self.intern(TypeData::Unbound),
                    });
                }
                result.push(self.intern(TypeData::Inst {
                    generic,
                    args: args.into(),
                }));
            }
        }
        Ok(result)
    }

    /// `IType.GetDelegateInvokeMethod()`.
    pub fn get_delegate_invoke_method(&mut self, t: TypeId) -> Result<Option<ResolvedMethod>> {
        let is_delegate = self
            .definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .is_some_and(|i| i.kind == TypeKind::Delegate);
        if !is_delegate {
            return Ok(None);
        }
        Ok(self
            .get_methods(t, None, MemberOptions::DECLARED)?
            .into_iter()
            .find(|m| m.name == "Invoke"))
    }
}
