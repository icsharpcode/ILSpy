//! A compilation: the main module plus its reference closure, in the order the host computed.
//!
//! Which assemblies make up the closure, and which module an AssemblyRef or ModuleRef row
//! resolves to, is host policy (the managed `DecompilerTypeSystem` and its assembly resolver
//! decide it). The host passes the outcome; the engine only resolves names within it.

use std::collections::HashMap;
use std::sync::OnceLock;

use crate::metadata::bytes::Bytes;
use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Error, Metadata, Result};

use super::known::KnownTypeCode;
use super::names::{FullTypeName, TopLevelTypeName, split_type_parameter_count};

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct ModuleId(pub u32);

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct TypeDefId {
    pub module: ModuleId,
    pub row: u32,
}

/// One module as the host describes it.
pub struct ModuleInput<'a> {
    pub image: &'a [u8],
    /// The simple assembly name (empty for a netmodule).
    pub assembly_name: String,
    /// For each AssemblyRef row (1-based, index `row - 1`), the module it resolves to.
    pub assembly_refs: Vec<Option<ModuleId>>,
    /// For each ModuleRef row, the module it resolves to.
    pub module_refs: Vec<Option<ModuleId>>,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum TypeKind {
    Class,
    Interface,
    Struct,
    Delegate,
    Enum,
    Void,
}

impl TypeKind {
    pub fn name(self) -> &'static str {
        match self {
            TypeKind::Class => "Class",
            TypeKind::Interface => "Interface",
            TypeKind::Struct => "Struct",
            TypeKind::Delegate => "Delegate",
            TypeKind::Enum => "Enum",
            TypeKind::Void => "Void",
        }
    }
}

/// What the managed `MetadataTypeDefinition` computes eagerly.
#[derive(Clone, Debug)]
pub struct TypeDefInfo {
    pub full_name: FullTypeName,
    pub flags: u32,
    /// Enclosing TypeDef row, 0 for a top-level type.
    pub declaring: u32,
    pub type_parameter_count: u32,
    pub known: KnownTypeCode,
    pub kind: TypeKind,
    /// For enums, the known type of the underlying type (`None` when it is not a primitive).
    pub enum_underlying: Option<KnownTypeCode>,
}

pub struct Module<'a> {
    pub md: Metadata<'a>,
    pub assembly_name: String,
    assembly_refs: Vec<Option<ModuleId>>,
    module_refs: Vec<Option<ModuleId>>,
    type_defs: OnceLock<Vec<Result<TypeDefInfo>>>,
    top_level: OnceLock<HashMap<TopLevelTypeName, u32>>,
    forwarders: OnceLock<HashMap<FullTypeName, u32>>,
    nested: OnceLock<HashMap<u32, Vec<u32>>>,
}

pub struct Compilation<'a> {
    modules: Vec<Module<'a>>,
    /// `TypeSystemOptions` of every module (the host applies them uniformly).
    pub options: u32,
}

const TYPE_ATTR_INTERFACE: u32 = 0x20;
const TYPE_ATTR_VISIBILITY: u32 = 0x07;
const FIELD_ATTR_STATIC: u16 = 0x10;

impl<'a> Compilation<'a> {
    /// Module 0 is the main module.
    pub fn new(inputs: Vec<ModuleInput<'a>>, options: u32) -> Result<Self> {
        let modules = inputs
            .into_iter()
            .map(|input| {
                Ok(Module {
                    md: Metadata::parse(input.image)?,
                    assembly_name: input.assembly_name,
                    assembly_refs: input.assembly_refs,
                    module_refs: input.module_refs,
                    type_defs: OnceLock::new(),
                    top_level: OnceLock::new(),
                    forwarders: OnceLock::new(),
                    nested: OnceLock::new(),
                })
            })
            .collect::<Result<_>>()?;
        Ok(Self { modules, options })
    }

    pub fn module(&self, id: ModuleId) -> &Module<'a> {
        &self.modules[id.0 as usize]
    }

    pub fn module_ids(&self) -> impl Iterator<Item = ModuleId> + use<> {
        (0..self.modules.len() as u32).map(ModuleId)
    }

    pub fn type_def(&self, id: TypeDefId) -> Result<&TypeDefInfo> {
        let defs = self.module(id.module).type_defs();
        match defs.get(id.row as usize - 1) {
            Some(Ok(info)) => Ok(info),
            Some(Err(e)) => Err(e.clone()),
            None => Err(Error::Malformed("TypeDef row out of range")),
        }
    }

    /// The type definition a full name denotes in `module`, following type forwarders
    /// (`IModule.GetTypeDefinition(FullTypeName)`).
    pub fn find_type_definition(
        &self,
        module: ModuleId,
        name: &FullTypeName,
        busy: &mut Vec<ModuleId>,
    ) -> Result<Option<TypeDefId>> {
        let m = self.module(module);
        let mut def = match m.top_level().get(&name.top) {
            Some(&row) => TypeDefId { module, row },
            None => {
                let key = FullTypeName::top_level(name.top.clone());
                let Some(&exported) = m.forwarders().get(&key) else {
                    return Ok(None);
                };
                match self.resolve_forwarded(module, exported, busy)? {
                    Some(def) => def,
                    None => return Ok(None),
                }
            }
        };
        let mut tpc = name.top.tpc;
        for (nested_name, additional) in &name.nested {
            tpc += additional;
            let Some(found) = self.find_nested(def, nested_name, tpc)? else {
                return Ok(None);
            };
            def = found;
        }
        Ok(Some(def))
    }

    /// `ITypeDefinition.NestedTypes`, in NestedClass table order.
    pub fn nested_types(&self, outer: TypeDefId) -> Vec<TypeDefId> {
        let m = self.module(outer.module);
        m.nested().get(&outer.row).map_or_else(Vec::new, |rows| {
            rows.iter()
                .map(|&row| TypeDefId {
                    module: outer.module,
                    row,
                })
                .collect()
        })
    }

    /// The first module whose simple assembly name equals `name`, ignoring case. The host
    /// keeps one module per simple name, so this is also what full-name matching finds.
    pub fn module_by_short_name(&self, name: &str) -> Option<ModuleId> {
        self.module_ids()
            .find(|&id| eq_ignore_case(&self.module(id).assembly_name, name))
    }

    fn find_nested(&self, outer: TypeDefId, name: &str, tpc: u32) -> Result<Option<TypeDefId>> {
        let m = self.module(outer.module);
        for &row in m.nested().get(&outer.row).map_or(&[][..], Vec::as_slice) {
            let def = TypeDefId {
                module: outer.module,
                row,
            };
            let info = self.type_def(def)?;
            if info.full_name.name() == name && info.type_parameter_count == tpc {
                return Ok(Some(def));
            }
        }
        Ok(None)
    }

    /// The definition an ExportedType row of `module` forwards to; `busy` holds the modules
    /// whose forwarders are being followed, so a chain that comes back ends as unresolved.
    pub fn resolve_forwarded(
        &self,
        module: ModuleId,
        exported: u32,
        busy: &mut Vec<ModuleId>,
    ) -> Result<Option<TypeDefId>> {
        let md = &self.module(module).md;
        let target = self.forward_target(module, exported)?;
        let name = exported_full_name(md, exported)?;
        let Some(target) = target else {
            return Ok(None);
        };
        if busy.contains(&module) {
            return Ok(None);
        }
        busy.push(module);
        let result = self.find_type_definition(target, &name, busy);
        busy.pop();
        result
    }

    fn forward_target(&self, module: ModuleId, exported: u32) -> Result<Option<ModuleId>> {
        let md = &self.module(module).md;
        let e = md.get::<rows::ExportedType>(exported)?;
        match e.implementation.decode()? {
            (TableId::File, _) => Ok(Some(module)),
            (TableId::ExportedType, outer) => self.forward_target(module, outer),
            (TableId::AssemblyRef, row) => {
                let r = md.get::<rows::AssemblyRef>(row)?;
                let short = md.string(r.name)?;
                Ok(self
                    .module_ids()
                    .find(|&id| eq_ignore_case(&self.module(id).assembly_name, &short)))
            }
            _ => Err(Error::Malformed("exported type implementation")),
        }
    }

    /// The module a TypeRef's resolution scope names (`MetadataModule.GetDeclaringModule`);
    /// `None` means "search every module".
    pub fn declaring_module(&self, module: ModuleId, type_ref: u32) -> Result<Option<ModuleId>> {
        let m = self.module(module);
        let tr = m.md.get::<rows::TypeRef>(type_ref)?;
        Ok(match tr.resolution_scope.decode()? {
            (TableId::TypeRef, 0) | (TableId::AssemblyRef, 0) | (TableId::ModuleRef, 0) => None,
            (TableId::TypeRef, outer) => return self.declaring_module(module, outer),
            (TableId::AssemblyRef, row) => m.assembly_refs.get(row as usize - 1).copied().flatten(),
            (TableId::ModuleRef, row) => m.module_refs.get(row as usize - 1).copied().flatten(),
            _ => Some(module),
        })
    }
}

/// `string.Equals(a, b, StringComparison.OrdinalIgnoreCase)`: per-character simple uppercase.
fn eq_ignore_case(a: &str, b: &str) -> bool {
    fn upper(c: char) -> char {
        let mut u = c.to_uppercase();
        match (u.next(), u.next()) {
            (Some(single), None) => single,
            _ => c,
        }
    }
    a.chars().map(upper).eq(b.chars().map(upper))
}

impl<'a> Module<'a> {
    pub fn type_defs(&self) -> &[Result<TypeDefInfo>] {
        self.type_defs.get_or_init(|| {
            (1..=self.md.tables.rows(TableId::TypeDef))
                .map(|row| self.compute_type_def(row))
                .collect()
        })
    }

    fn top_level(&self) -> &HashMap<TopLevelTypeName, u32> {
        self.top_level.get_or_init(|| {
            let mut map = HashMap::new();
            for row in 1..=self.md.tables.rows(TableId::TypeDef) {
                if self.md.declaring_type(row).unwrap_or(0) != 0 {
                    continue;
                }
                if let Ok(t) = self.md.get::<rows::TypeDef>(row)
                    && let (Ok(ns), Ok(name)) =
                        (self.md.string(t.namespace), self.md.string(t.name))
                {
                    let (name, tpc) = split_type_parameter_count(&name);
                    map.insert(TopLevelTypeName::new(&ns, name, tpc), row);
                }
            }
            map
        })
    }

    fn forwarders(&self) -> &HashMap<FullTypeName, u32> {
        self.forwarders.get_or_init(|| {
            let mut map = HashMap::new();
            for row in 1..=self.md.tables.rows(TableId::ExportedType) {
                if let Ok(name) = exported_full_name(&self.md, row) {
                    map.insert(name, row);
                }
            }
            map
        })
    }

    fn nested(&self) -> &HashMap<u32, Vec<u32>> {
        self.nested.get_or_init(|| {
            let mut map: HashMap<u32, Vec<u32>> = HashMap::new();
            for row in 1..=self.md.tables.rows(TableId::NestedClass) {
                if let Ok(n) = self.md.get::<rows::NestedClass>(row) {
                    map.entry(n.enclosing).or_default().push(n.nested);
                }
            }
            map
        })
    }

    fn compute_type_def(&self, row: u32) -> Result<TypeDefInfo> {
        let md = &self.md;
        let t = md.get::<rows::TypeDef>(row)?;
        let full_name = type_def_full_name(md, row)?;
        let declaring = md.declaring_type(row)?;
        let type_parameter_count = md.generic_params(TableId::TypeDef, row).len() as u32;
        let known = if declaring == 0 {
            KnownTypeCode::from_top_level(&full_name.top)
        } else {
            KnownTypeCode::None
        };
        let base = t.extends.token().unwrap_or(0) & 0x00FF_FFFF != 0;
        let base_is =
            |code: KnownTypeCode| base && is_known_type(md, t.extends.token().unwrap_or(0), code);
        let mut enum_underlying = None;
        let kind = if t.flags & TYPE_ATTR_INTERFACE != 0 {
            TypeKind::Interface
        } else if let Some(underlying) = self.enum_underlying(row, base_is(KnownTypeCode::Enum))? {
            enum_underlying = Some(underlying);
            TypeKind::Enum
        } else if base_is(KnownTypeCode::Enum)
            || base_is(KnownTypeCode::ValueType)
                && !(full_name.nested.is_empty()
                    && KnownTypeCode::from_top_level(&full_name.top) == KnownTypeCode::Enum)
        {
            if known == KnownTypeCode::Void {
                TypeKind::Void
            } else {
                TypeKind::Struct
            }
        } else if base_is(KnownTypeCode::MulticastDelegate) {
            TypeKind::Delegate
        } else {
            TypeKind::Class
        };
        Ok(TypeDefInfo {
            full_name,
            flags: t.flags,
            declaring,
            type_parameter_count,
            known,
            kind,
            enum_underlying,
        })
    }

    /// `TypeDefinition.IsEnum(out underlyingType)`: the raw element type byte of the first
    /// instance field, mapped to a known type.
    fn enum_underlying(&self, row: u32, extends_enum: bool) -> Result<Option<KnownTypeCode>> {
        if !extends_enum {
            return Ok(None);
        }
        for field in self.md.type_fields(row)? {
            let f = self.md.get::<rows::Field>(field)?;
            if f.flags & FIELD_ATTR_STATIC != 0 {
                continue;
            }
            let mut b = Bytes::new(self.md.blob(f.signature)?);
            if crate::metadata::signature::header_kind(b.u8()?)
                != crate::metadata::signature::HEADER_KIND_FIELD
            {
                return Ok(None);
            }
            return Ok(Some(primitive_code_to_known(b.u8()?)));
        }
        Ok(None)
    }
}

/// `PrimitiveTypeCode.ToKnownTypeCode()` for a raw element type byte.
pub fn primitive_code_to_known(code: u8) -> KnownTypeCode {
    use KnownTypeCode::*;
    match code {
        0x01 => Void,
        0x02 => Boolean,
        0x03 => Char,
        0x04 => SByte,
        0x05 => Byte,
        0x06 => Int16,
        0x07 => UInt16,
        0x08 => Int32,
        0x09 => UInt32,
        0x0A => Int64,
        0x0B => UInt64,
        0x0C => Single,
        0x0D => Double,
        0x0E => String,
        0x16 => TypedReference,
        0x18 => IntPtr,
        0x19 => UIntPtr,
        0x1C => Object,
        _ => KnownTypeCode::None,
    }
}

/// `TypeDefinition.GetFullTypeName`.
pub fn type_def_full_name(md: &Metadata<'_>, row: u32) -> Result<FullTypeName> {
    let t = md.get::<rows::TypeDef>(row)?;
    let raw = md.string(t.name)?;
    let (name, tpc) = split_type_parameter_count(&raw);
    match md.declaring_type(row)? {
        0 => {
            let ns = md.string(t.namespace)?;
            Ok(FullTypeName::top_level(TopLevelTypeName::new(
                &ns, name, tpc,
            )))
        }
        outer => Ok(type_def_full_name(md, outer)?.nested_type(name.to_owned(), tpc)),
    }
}

/// `TypeReferenceHandle.GetFullTypeName`, including its fallbacks for unreadable names.
pub fn type_ref_full_name(md: &Metadata<'_>, row: u32) -> Result<FullTypeName> {
    let tr = md.get::<rows::TypeRef>(row)?;
    let raw = md
        .string(tr.name)
        .map(|n| n.into_owned())
        .unwrap_or_else(|_| format!("TR{:08x}", 0x0100_0000 | row));
    let (name, tpc) = split_type_parameter_count(&raw);
    let declaring = match tr.resolution_scope.decode() {
        Ok((TableId::TypeRef, outer)) if outer != 0 => outer,
        _ => 0,
    };
    if declaring == 0 {
        let ns = md
            .string(tr.namespace)
            .map(|n| n.into_owned())
            .unwrap_or_default();
        Ok(FullTypeName::top_level(TopLevelTypeName::new(
            &ns, name, tpc,
        )))
    } else {
        Ok(type_ref_full_name(md, declaring)?.nested_type(name.to_owned(), tpc))
    }
}

/// `ExportedType.GetFullTypeName`.
pub fn exported_full_name(md: &Metadata<'_>, row: u32) -> Result<FullTypeName> {
    let e = md.get::<rows::ExportedType>(row)?;
    let raw = md.string(e.name)?;
    let (name, tpc) = split_type_parameter_count(&raw);
    match e.implementation.decode()? {
        (TableId::ExportedType, outer) => {
            Ok(exported_full_name(md, outer)?.nested_type(name.to_owned(), tpc))
        }
        _ => {
            let ns = md.string(e.namespace)?;
            Ok(FullTypeName::top_level(TopLevelTypeName::new(&ns, name, tpc)).clone())
        }
    }
}

/// `EntityHandle.IsKnownType`: a metadata-only name check of a TypeDef, TypeRef or TypeSpec
/// token. Bad metadata answers `false`.
pub fn is_known_type(md: &Metadata<'_>, token: u32, code: KnownTypeCode) -> bool {
    code.type_name()
        .is_some_and(|(ns, name, tpc)| is_known_type_name(md, token, ns, name, tpc))
}

/// `IsKnownType` against a top-level name (namespace, name, type parameter count).
pub fn is_known_type_name(md: &Metadata<'_>, token: u32, ns: &str, name: &str, tpc: u32) -> bool {
    let row = token & 0x00FF_FFFF;
    if row == 0 {
        return false;
    }
    let names = match token >> 24 {
        0x01 => md.get::<rows::TypeRef>(row).ok().and_then(|tr| {
            match tr.resolution_scope.decode().ok()? {
                (_, 0) | (TableId::TypeRef, _) => None,
                _ => Some((tr.namespace, tr.name)),
            }
        }),
        0x02 => md.get::<rows::TypeDef>(row).ok().and_then(|td| {
            (td.flags & TYPE_ATTR_VISIBILITY < 2).then_some((td.namespace, td.name))
        }),
        0x1B => {
            return md
                .get::<rows::TypeSpec>(row)
                .and_then(|s| md.blob(s.signature))
                .is_ok_and(|blob| {
                    signature_is_known_type(md, &mut Bytes::new(blob), (ns, name, tpc))
                });
        }
        _ => None,
    };
    let Some((ns_idx, name_idx)) = names else {
        return false;
    };
    let (Ok(actual_ns), Ok(actual_name)) = (md.string(ns_idx), md.string(name_idx)) else {
        return false;
    };
    let name_matches = if tpc == 0 {
        actual_name == name
    } else {
        split_type_parameter_count(&actual_name) == (name, tpc)
    };
    name_matches && actual_ns == ns
}

fn signature_is_known_type(
    md: &Metadata<'_>,
    b: &mut Bytes<'_>,
    target: (&str, &str, u32),
) -> bool {
    let Ok(type_code) = b.compressed_u32() else {
        return false;
    };
    match type_code {
        0x01..=0x0E | 0x16 | 0x18 | 0x19 | 0x1C => {
            primitive_code_to_known(type_code as u8).type_name() == Some(target)
        }
        0x1F | 0x20 => b.compressed_u32().is_ok() && signature_is_known_type(md, b, target),
        0x15 => signature_is_known_type(md, b, target),
        0x11 | 0x12 => match b.compressed_u32() {
            Ok(raw) => {
                let table = [0x02u32, 0x01, 0x1B, 0][(raw & 3) as usize];
                table != 0
                    && is_known_type_name(md, table << 24 | raw >> 2, target.0, target.1, target.2)
            }
            Err(_) => false,
        },
        _ => false,
    }
}
