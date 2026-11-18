//! `ExtensionInfo`: the C# 14 extension blocks of an extension container, in both metadata
//! encodings (V1: one `<>E__` group type per block; V2: one `<G>$` grouping type per receiver
//! type, with one `<M>$` marker type per block), and which static method of the container
//! implements each extension member.

use crate::metadata::tables::{TableId, rows};

use super::compilation::TypeDefId;
use super::fnptr::Normalization;
use super::members::{MethodDefId, SymbolKind};
use super::types::{TypeData, TypeId, TypeParamOwner, TypeSystem};

/// One extension block: its marker method (`<Extension>$`, whose parameter is the receiver) and
/// the type parameters its members are specialized with.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct ExtensionGroup {
    pub marker: MethodDefId,
    pub type_parameters: Vec<TypeId>,
}

/// `ExtensionMemberInfo`: an extension member, its block and its implementation.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub struct ExtensionMemberInfo {
    /// The index of the block in `ExtensionInfo::groups`.
    pub group: usize,
    pub extension: MethodDefId,
    pub implementation: MethodDefId,
}

#[derive(Clone, Debug, Default)]
pub struct ExtensionInfo {
    pub groups: Vec<ExtensionGroup>,
    pub members: Vec<ExtensionMemberInfo>,
}

impl ExtensionInfo {
    /// `InfoOfExtensionMember`.
    pub fn of_extension(&self, method: MethodDefId) -> Option<ExtensionMemberInfo> {
        self.members.iter().copied().find(|m| m.extension == method)
    }

    /// `InfoOfImplementationMember`.
    pub fn of_implementation(&self, method: MethodDefId) -> Option<ExtensionMemberInfo> {
        self.members
            .iter()
            .copied()
            .find(|m| m.implementation == method)
    }
}

impl TypeSystem<'_, '_> {
    fn method_name_of(&self, m: MethodDefId) -> Option<String> {
        let md = &self.comp.module(m.module).md;
        let def = md.get::<rows::MethodDef>(m.row).ok()?;
        Some(md.string(def.name).ok()?.into_owned())
    }

    fn type_name_of(&self, def: TypeDefId) -> Option<String> {
        Some(self.comp.type_def(def).ok()?.full_name.name().to_owned())
    }

    /// A type of extension blocks in a type with extensions: the `<>E__` group type (encoding V1)
    /// or the `<G>$` grouping type (encoding V2), whose members belong to their blocks.
    pub fn is_extension_group_type(&self, def: TypeDefId) -> bool {
        let in_container = self.comp.type_def(def).is_ok_and(|i| {
            i.declaring != 0
                && self
                    .type_flags(TypeDefId {
                        module: def.module,
                        row: i.declaring,
                    })
                    .has_extensions
        });
        in_container
            && (self.is_sealed_class_named(def, "<>E__") || self.is_sealed_class_named(def, "<G>$"))
    }

    /// A sealed class whose name starts with `prefix`.
    fn is_sealed_class_named(&self, def: TypeDefId, prefix: &str) -> bool {
        self.comp.type_def(def).is_ok_and(|i| {
            i.kind == super::compilation::TypeKind::Class
                && i.flags & 0x100 != 0
                && i.full_name.name().starts_with(prefix)
        })
    }

    /// The methods of `def` that are no constructors, with the `<Extension>$` markers apart.
    fn group_methods(&mut self, def: TypeDefId) -> (Vec<MethodDefId>, Vec<MethodDefId>) {
        let rows = self
            .comp
            .module(def.module)
            .md
            .type_methods(def.row)
            .unwrap_or_default();
        let (mut markers, mut methods) = (Vec::new(), Vec::new());
        for row in rows {
            let m = MethodDefId {
                module: def.module,
                row,
            };
            if self
                .method_symbol_kind(m, None)
                .is_ok_and(|k| k == SymbolKind::Constructor)
            {
                continue;
            }
            let is_static = self.method_modifiers_of(m).is_ok_and(|x| x.is_static);
            let parameters = self.method_signature(m).1.len();
            if self.method_name_of(m).as_deref() == Some("<Extension>$")
                && is_static
                && parameters == 1
            {
                markers.push(m);
            } else {
                methods.push(m);
            }
        }
        (markers, methods)
    }

    /// The type parameters of `def` as its members' signatures name them.
    fn own_type_parameters(&mut self, def: TypeDefId) -> Vec<TypeId> {
        let count = self
            .comp
            .type_def(def)
            .map_or(0, |i| i.type_parameter_count);
        (0..count)
            .map(|index| {
                let owner = self.class_param_owner(def, index);
                self.intern(TypeData::TypeParam { owner, index })
            })
            .collect()
    }

    /// The first fixed argument of a method's `[ExtensionMarker]`, the name of its marker type.
    /// Read from the metadata: the attribute lists of the type system leave the marker out.
    fn extension_marker_name(&mut self, m: MethodDefId) -> Option<String> {
        let comp = self.comp;
        let md = &comp.module(m.module).md;
        let row = md
            .custom_attributes(TableId::MethodDef, m.row)
            .into_iter()
            .find(|&ca| {
                md.attribute_type(ca).is_ok_and(|t| {
                    super::compilation::is_known_type_name(
                        md,
                        t,
                        "System.Runtime.CompilerServices",
                        "ExtensionMarkerAttribute",
                        0,
                    )
                })
            })?;
        let value = self.decode_attribute(m.module, row).ok()?;
        match &value.fixed.first()?.value {
            super::attrvalue::ArgValue::String(s) => Some(String::from_utf16_lossy(s)),
            _ => None,
        }
    }

    /// `new ExtensionInfo(module, extensionContainer)`; `None` for a type without extensions.
    pub fn extension_info(&mut self, container: TypeDefId) -> Option<ExtensionInfo> {
        if !self.type_flags(container).has_extensions {
            return None;
        }
        let mut info = ExtensionInfo::default();
        let container_methods: Vec<MethodDefId> = self
            .comp
            .module(container.module)
            .md
            .type_methods(container.row)
            .unwrap_or_default()
            .into_iter()
            .map(|row| MethodDefId {
                module: container.module,
                row,
            })
            .collect();
        for nested in self.comp.nested_types(container) {
            // Encoding V1: a `<>E__` group type with one marker and the extension members.
            if self.is_sealed_class_named(nested, "<>E__") {
                let (markers, methods) = self.group_methods(nested);
                if let [marker] = markers[..] {
                    let type_parameters = self.own_type_parameters(nested);
                    let group = info.groups.len();
                    info.groups.push(ExtensionGroup {
                        marker,
                        type_parameters,
                    });
                    self.collect_implementations(
                        &mut info,
                        group,
                        nested,
                        &methods,
                        &container_methods,
                    );
                    continue;
                }
            }
            // Encoding V2: a `<G>$` grouping type with one `<M>$` marker type per block.
            if !self.is_sealed_class_named(nested, "<G>$") {
                continue;
            }
            for marker_type in self.comp.nested_types(nested) {
                let marker_name = self.type_name_of(marker_type).unwrap_or_default();
                if !marker_name.starts_with("<M>$") || !self.type_flags(marker_type).is_static {
                    continue;
                }
                let (markers, _) = self.group_methods(marker_type);
                let [marker] = markers[..] else {
                    continue;
                };
                let type_parameters = self.own_type_parameters(marker_type);
                let group = info.groups.len();
                info.groups.push(ExtensionGroup {
                    marker,
                    type_parameters,
                });
                let (_, grouping_methods) = self.group_methods(nested);
                let members: Vec<MethodDefId> = grouping_methods
                    .into_iter()
                    .filter(|&m| {
                        self.extension_marker_name(m).as_deref() == Some(marker_name.as_str())
                    })
                    .collect();
                self.collect_implementations(
                    &mut info,
                    group,
                    marker_type,
                    &members,
                    &container_methods,
                );
            }
        }
        Some(info)
    }

    /// `CollectImplementationMethods`: the static method of the container whose signature is the
    /// extension member's with the receiver first, and the block's and the member's type
    /// parameters as its own.
    fn collect_implementations(
        &mut self,
        info: &mut ExtensionInfo,
        group: usize,
        marker_type: TypeDefId,
        extensions: &[MethodDefId],
        container_methods: &[MethodDefId],
    ) {
        let n = Normalization::TYPE_ERASURE;
        let marker = info.groups[group].marker;
        let receiver = self.method_signature(marker).1.first().map(|p| p.ty);
        let marker_args = self.own_type_parameters(marker_type);
        for &extension in extensions {
            let extension_type_parameters = self
                .comp
                .module(extension.module)
                .md
                .generic_params(TableId::MethodDef, extension.row)
                .count() as u32;
            let extension_args: Vec<TypeId> = (0..extension_type_parameters)
                .map(|index| {
                    self.intern(TypeData::TypeParam {
                        owner: TypeParamOwner::Method(extension),
                        index,
                    })
                })
                .collect();
            let substitution: Vec<TypeId> =
                marker_args.iter().chain(&extension_args).copied().collect();
            let has_instance = !self
                .method_modifiers_of(extension)
                .is_ok_and(|m| m.is_static);
            let offset = usize::from(has_instance);
            let (extension_return, extension_params) = self.method_signature(extension);
            let extension_name = self.method_name_of(extension);
            let mut found = None;
            for &implementation in container_methods {
                if !self
                    .method_modifiers_of(implementation)
                    .is_ok_and(|m| m.is_static)
                    || self.method_name_of(implementation) != extension_name
                {
                    continue;
                }
                let tpc = self
                    .comp
                    .module(implementation.module)
                    .md
                    .generic_params(TableId::MethodDef, implementation.row)
                    .count();
                let (ret, params) = self.method_signature(implementation);
                if tpc != substitution.len() || params.len() != extension_params.len() + offset {
                    continue;
                }
                let same = |ts: &mut Self, a: TypeId, b: TypeId| {
                    let a = ts.substitute(a, None, Some(&substitution));
                    ts.normalize_with(a, n) == ts.normalize_with(b, n)
                };
                if has_instance && !receiver.is_some_and(|r| same(self, params[0].ty, r)) {
                    continue;
                }
                if !extension_params
                    .iter()
                    .zip(&params[offset..])
                    .all(|(e, i)| same(self, i.ty, e.ty))
                {
                    continue;
                }
                if !same(self, ret, extension_return) {
                    continue;
                }
                found = Some(implementation);
            }
            if let Some(implementation) = found {
                info.members.push(ExtensionMemberInfo {
                    group,
                    extension,
                    implementation,
                });
            }
        }
    }
}
