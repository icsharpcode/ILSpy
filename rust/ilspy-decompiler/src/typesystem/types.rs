//! Interned types and their construction from metadata.
//!
//! `TypeId`s are indices into one `TypeSystem`'s interner, so structural equality is `==`.
//! A `TypeSystem` belongs to one request and borrows the shared `Compilation`.
//!
//! Signatures decode in two stages, like the managed type system: `from_sig_raw` is
//! `TypeProvider` (modifiers kept, no attribute semantics) and `attrs::apply_attributes` is
//! `ApplyAttributeTypeVisitor`, run where the managed code runs it.

use std::collections::HashMap;

use crate::metadata::bytes::Bytes;
use crate::metadata::signature::{self, SigType};
use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Error, Result};

use super::attrs::AttributeSource;
use super::compilation::{Compilation, ModuleId, TypeDefId, TypeKind, type_ref_full_name};
pub use super::fnptr::FnPtrType;
use super::known::KnownTypeCode;
use super::names::{FullTypeName, TopLevelTypeName};
use super::options;

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct TypeId(u32);

impl TypeId {
    /// A type id for unit tests that never look the type up.
    #[cfg(test)]
    pub(crate) const PLACEHOLDER: TypeId = TypeId(0);
}

/// Who declares a type parameter. Dummy parameters stand in for indices outside the generic
/// context, like the managed `DummyTypeParameter`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum TypeParamOwner {
    Type(TypeDefId),
    Method(super::members::MethodDefId),
    DummyType,
    DummyMethod,
}

impl TypeParamOwner {
    pub fn is_method(self) -> bool {
        matches!(
            self,
            TypeParamOwner::Method(_) | TypeParamOwner::DummyMethod
        )
    }
}

/// `Nullability`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum Nullability {
    Oblivious = 0,
    NotNullable = 1,
    Nullable = 2,
}

impl Nullability {
    pub fn from_byte(b: u8) -> Option<Self> {
        match b {
            0 => Some(Nullability::Oblivious),
            1 => Some(Nullability::NotNullable),
            2 => Some(Nullability::Nullable),
            _ => None,
        }
    }
}

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub enum TypeData {
    Def(TypeDefId),
    /// A reference that resolved to nothing; `is_reference_type` comes from the signature.
    Unknown {
        name: Box<FullTypeName>,
        is_reference_type: Option<bool>,
    },
    /// `SpecialType.UnknownType` (`?`).
    UnknownSpecial,
    /// `SpecialType.UnboundTypeArgument`: the missing arguments of a nested type found through an
    /// outer type.
    Unbound,
    TypeParam {
        owner: TypeParamOwner,
        index: u32,
    },
    Inst {
        generic: TypeId,
        args: Box<[TypeId]>,
    },
    Array {
        elem: TypeId,
        rank: u32,
        nullability: Nullability,
    },
    Pointer(TypeId),
    ByRef(TypeId),
    Pinned(TypeId),
    FnPtr(Box<FnPtrType>),
    /// `SpecialType.ArgList`, the trailing parameter of a varargs method.
    ArgList,
    /// A custom modifier, before `ApplyAttributeTypeVisitor` removes it.
    Modified {
        modifier: TypeId,
        inner: TypeId,
        required: bool,
    },
    Dynamic,
    NInt,
    NUInt,
    /// `TupleType`: element types with optional names over a `System.ValueTuple` instance.
    Tuple {
        underlying: TypeId,
        elements: Box<[TypeId]>,
        names: Box<[Option<Box<str>>]>,
    },
    /// `NullabilityAnnotatedType` (and its type parameter variant).
    Annotated {
        inner: TypeId,
        nullability: Nullability,
    },
}

/// The type parameters in scope, as `(owner, count)`.
#[derive(Clone, Copy, Debug, Default)]
pub struct GenericContext {
    pub class: Option<(TypeDefId, u32)>,
    pub method: Option<(super::members::MethodDefId, u32)>,
}

pub struct TypeSystem<'c, 'a> {
    pub comp: &'c Compilation<'a>,
    data: Vec<TypeData>,
    map: HashMap<TypeData, TypeId>,
    known: HashMap<KnownTypeCode, TypeId>,
    /// Per module: method row to `(owner token, semantics)` of C# accessors.
    pub(super) accessor_owners: HashMap<ModuleId, HashMap<u32, (u32, u16)>>,
}

impl<'c, 'a> TypeSystem<'c, 'a> {
    pub fn new(comp: &'c Compilation<'a>) -> Self {
        Self {
            comp,
            data: Vec::new(),
            map: HashMap::new(),
            known: HashMap::new(),
            accessor_owners: HashMap::new(),
        }
    }

    /// The `TypeSystemOptions` of `module`.
    pub fn options(&self, module: ModuleId) -> u32 {
        self.comp.module(module).options
    }

    pub fn intern(&mut self, data: TypeData) -> TypeId {
        if let Some(&id) = self.map.get(&data) {
            return id;
        }
        let id = TypeId(self.data.len() as u32);
        self.data.push(data.clone());
        self.map.insert(data, id);
        id
    }

    pub fn data(&self, id: TypeId) -> &TypeData {
        &self.data[id.0 as usize]
    }

    /// `ICompilation.FindType(TopLevelTypeName)`: the first module defining the name wins.
    pub fn find_top_level(&mut self, top: TopLevelTypeName) -> TypeId {
        let name = FullTypeName::top_level(top);
        let comp = self.comp;
        let data = comp
            .module_ids()
            .find_map(|m| {
                comp.find_type_definition(m, &name, &mut Vec::new())
                    .ok()
                    .flatten()
            })
            .map_or_else(
                || TypeData::Unknown {
                    name: Box::new(name.clone()),
                    is_reference_type: None,
                },
                TypeData::Def,
            );
        self.intern(data)
    }

    /// `ICompilation.FindType(KnownTypeCode)`.
    pub fn known_type(&mut self, code: KnownTypeCode) -> TypeId {
        if let Some(&id) = self.known.get(&code) {
            return id;
        }
        let id = match code.top_level_name() {
            None => self.intern(TypeData::UnknownSpecial),
            Some(top) => self.find_top_level(top),
        };
        self.known.insert(code, id);
        id
    }

    pub fn type_parameter_count(&self, id: TypeId) -> u32 {
        match self.data(id) {
            TypeData::Def(def) => self
                .comp
                .type_def(*def)
                .map_or(0, |i| i.type_parameter_count),
            TypeData::Unknown { name, .. } => name.type_parameter_count(),
            TypeData::Annotated { inner, .. } => self.type_parameter_count(*inner),
            _ => 0,
        }
    }

    /// `TypeProvider.GetTypeFromReference`.
    fn type_from_ref(&mut self, module: ModuleId, row: u32, raw_kind: u8) -> Result<TypeId> {
        let comp = self.comp;
        let declaring = comp.declaring_module(module, row)?;
        let name = type_ref_full_name(&comp.module(module).md, row)?;
        let found = match declaring {
            Some(m) => comp.find_type_definition(m, &name, &mut Vec::new())?,
            None => {
                let mut found = None;
                for m in comp.module_ids() {
                    if let Some(def) = comp.find_type_definition(m, &name, &mut Vec::new())? {
                        found = Some(def);
                        break;
                    }
                }
                found
            }
        };
        Ok(self.intern(match found {
            Some(def) => TypeData::Def(def),
            None => TypeData::Unknown {
                name: Box::new(name),
                is_reference_type: is_reference_type(raw_kind),
            },
        }))
    }

    fn type_from_def(&mut self, module: ModuleId, row: u32) -> Result<TypeId> {
        if row == 0 || row > self.comp.module(module).md.tables.rows(TableId::TypeDef) {
            return Err(Error::Malformed("TypeDef handle out of range"));
        }
        Ok(self.intern(TypeData::Def(TypeDefId { module, row })))
    }

    fn type_from_spec(
        &mut self,
        module: ModuleId,
        row: u32,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        let md = &self.comp.module(module).md;
        let spec = md.get::<rows::TypeSpec>(row)?;
        let sig = signature::decode_type(&mut Bytes::new(md.blob(spec.signature)?))?;
        self.from_sig_raw(module, &sig, ctx)
    }

    /// The `TypeProvider` result for a TypeDef, TypeRef, TypeSpec or ExportedType token,
    /// before attribute semantics.
    pub fn resolve_token_raw(
        &mut self,
        module: ModuleId,
        token: u32,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        let row = token & 0x00FF_FFFF;
        match token >> 24 {
            0x02 => self.type_from_def(module, row),
            0x01 => self.type_from_ref(module, row, 0),
            0x1B => self.type_from_spec(module, row, ctx),
            _ => Err(Error::Malformed("not a type handle")),
        }
    }

    /// `MetadataModule.ResolveType(handle, context, options, typeAttributes, nullableContext)`.
    pub fn resolve_token_with(
        &mut self,
        module: ModuleId,
        token: u32,
        ctx: GenericContext,
        attributes: AttributeSource,
        options: u32,
        nullable_context: Nullability,
    ) -> Result<TypeId> {
        let row = token & 0x00FF_FFFF;
        if row == 0 {
            return Ok(self.intern(TypeData::UnknownSpecial));
        }
        if token >> 24 == 0x27 {
            let def = self.comp.resolve_forwarded(module, row, &mut Vec::new())?;
            return Ok(match def {
                Some(def) => self.intern(TypeData::Def(def)),
                None => {
                    let name =
                        super::compilation::exported_full_name(&self.comp.module(module).md, row)?;
                    self.intern(TypeData::Unknown {
                        name: Box::new(name),
                        is_reference_type: None,
                    })
                }
            });
        }
        let raw = self.resolve_token_raw(module, token, ctx)?;
        self.apply_attributes(raw, module, attributes, options, nullable_context, false)
    }

    /// `ResolveType` with the module's options and no attributes.
    pub fn resolve_token(
        &mut self,
        module: ModuleId,
        token: u32,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        let options = self.options(module);
        self.resolve_token_with(
            module,
            token,
            ctx,
            AttributeSource::None,
            options,
            Nullability::Oblivious,
        )
    }

    /// `MetadataModule.ResolveDeclaringType`: resolved without dynamic, tuple, nullability and
    /// native integer semantics, then with tuples (and the rest) applied to type arguments only.
    pub fn resolve_declaring_type(
        &mut self,
        module: ModuleId,
        token: u32,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        let options = self.options(module);
        let removed = options::DYNAMIC
            | options::TUPLE
            | options::NULLABILITY
            | options::NATIVE_INTEGERS
            | options::NATIVE_INTEGERS_WITHOUT_ATTRIBUTE;
        let t = self.resolve_token_with(
            module,
            token,
            ctx,
            AttributeSource::None,
            options & !removed,
            Nullability::Oblivious,
        )?;
        self.apply_attributes(
            t,
            module,
            AttributeSource::None,
            options,
            Nullability::Oblivious,
            true,
        )
    }

    /// The type a decoded signature denotes, as `TypeProvider` builds it: modifiers kept,
    /// no attribute semantics.
    pub fn from_sig_raw(
        &mut self,
        module: ModuleId,
        sig: &SigType,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        Ok(match sig {
            SigType::Primitive(p) => {
                let code = super::compilation::primitive_code_to_known(primitive_byte(*p));
                self.known_type(code)
            }
            SigType::Handle { token, kind } => {
                let raw_kind = match kind {
                    signature::HandleKind::Class => 0x12,
                    signature::HandleKind::ValueType => 0x11,
                    signature::HandleKind::Unspecified => 0,
                };
                let row = token & 0x00FF_FFFF;
                match token >> 24 {
                    0x02 => self.type_from_def(module, row)?,
                    0x01 => self.type_from_ref(module, row, raw_kind)?,
                    _ => self.type_from_spec(module, row, ctx)?,
                }
            }
            SigType::GenericInst { generic, args } => {
                let generic = self.from_sig_raw(module, generic, ctx)?;
                let args = args
                    .iter()
                    .map(|a| self.from_sig_raw(module, a, ctx))
                    .collect::<Result<Box<[_]>>>()?;
                let tpc = self.type_parameter_count(generic);
                if tpc == 0 || tpc as usize != args.len() {
                    generic
                } else {
                    self.intern(TypeData::Inst { generic, args })
                }
            }
            SigType::TypeParam(index) => {
                let owner = match ctx.class {
                    Some((def, count)) if *index < count => self.class_param_owner(def, *index),
                    _ => TypeParamOwner::DummyType,
                };
                self.intern(TypeData::TypeParam {
                    owner,
                    index: *index,
                })
            }
            SigType::MethodParam(index) => {
                let owner = match ctx.method {
                    Some((m, count)) if *index < count => TypeParamOwner::Method(m),
                    _ => TypeParamOwner::DummyMethod,
                };
                self.intern(TypeData::TypeParam {
                    owner,
                    index: *index,
                })
            }
            SigType::Pointer(t) => {
                let t = self.from_sig_raw(module, t, ctx)?;
                self.intern(TypeData::Pointer(t))
            }
            SigType::ByRef(t) => {
                let t = self.from_sig_raw(module, t, ctx)?;
                self.intern(TypeData::ByRef(t))
            }
            SigType::Pinned(t) => {
                let t = self.from_sig_raw(module, t, ctx)?;
                self.intern(TypeData::Pinned(t))
            }
            SigType::SzArray(t) => {
                let elem = self.from_sig_raw(module, t, ctx)?;
                self.intern(TypeData::Array {
                    elem,
                    rank: 1,
                    nullability: Nullability::Oblivious,
                })
            }
            SigType::Array { elem, shape } => {
                let elem = self.from_sig_raw(module, elem, ctx)?;
                self.intern(TypeData::Array {
                    elem,
                    rank: shape.rank,
                    nullability: Nullability::Oblivious,
                })
            }
            SigType::FnPtr(sig) => self.function_pointer(module, sig, ctx)?,
            SigType::Modified {
                modifier,
                inner,
                required,
            } => {
                let modifier = self.from_sig_raw(module, modifier, ctx)?;
                let inner = self.from_sig_raw(module, inner, ctx)?;
                self.intern(TypeData::Modified {
                    modifier,
                    inner,
                    required: *required,
                })
            }
        })
    }

    /// A signature type with attribute semantics but no attributes
    /// (`ApplyAttributesToType(type, null, options, Oblivious)`), e.g. `IntroduceTupleTypes`.
    pub fn from_sig(
        &mut self,
        module: ModuleId,
        sig: &SigType,
        ctx: GenericContext,
    ) -> Result<TypeId> {
        let raw = self.from_sig_raw(module, sig, ctx)?;
        let options = self.options(module);
        self.apply_attributes(
            raw,
            module,
            AttributeSource::None,
            options,
            Nullability::Oblivious,
            false,
        )
    }

    /// `ITypeDefinition.DirectBaseTypes`.
    pub fn direct_base_types(&mut self, def: TypeDefId) -> Result<Vec<TypeId>> {
        let comp = self.comp;
        let md = &comp.module(def.module).md;
        let info = comp.type_def(def)?;
        let ctx = self.type_context(Some(def));
        let options = self.options(def.module);
        let t = md.get::<rows::TypeDef>(def.row)?;
        let mut bases = Vec::new();
        let base = match t.extends.token() {
            Ok(token) if token & 0x00FF_FFFF == 0 => None,
            Ok(token) => Some(
                self.resolve_token_with(
                    def.module,
                    token,
                    ctx,
                    AttributeSource::Entity(TableId::TypeDef, def.row),
                    options,
                    Nullability::Oblivious,
                )
                .unwrap_or_else(|_| self.intern(TypeData::UnknownSpecial)),
            ),
            Err(_) => Some(self.intern(TypeData::UnknownSpecial)),
        };
        match base {
            Some(b) => bases.push(b),
            None if info.kind == TypeKind::Interface => {
                bases.push(self.known_type(KnownTypeCode::Object))
            }
            None => {}
        }
        for row in md.type_interface_impls(def.row) {
            let iface = md.get::<rows::InterfaceImpl>(row)?.interface.token()?;
            bases.push(self.resolve_token_with(
                def.module,
                iface,
                ctx,
                AttributeSource::Entity(TableId::InterfaceImpl, row),
                options,
                Nullability::Oblivious,
            )?);
        }
        Ok(bases)
    }

    /// `IType.GetDefinition()`: the definition of a type or of a parameterized type's generic.
    pub fn definition(&self, id: TypeId) -> Option<TypeDefId> {
        match self.data(id) {
            TypeData::Def(def) => Some(*def),
            TypeData::Inst { generic, .. } => self.definition(*generic),
            TypeData::Annotated { inner, .. } => self.definition(*inner),
            TypeData::Tuple { underlying, .. } => self.definition(*underlying),
            // Without TypeSystemOptions.FunctionPointers a function pointer acts as an alias
            // for UIntPtr; function_pointer resolves UIntPtr whenever it creates one.
            // shortcut: reads the main module's options where the managed type reads those of
            // the module that created it; upgrade if modules ever get different
            // FunctionPointers settings outside tests.
            TypeData::FnPtr(_) if self.options(ModuleId(0)) & options::FUNCTION_POINTERS == 0 => {
                self.known
                    .get(&KnownTypeCode::UIntPtr)
                    .and_then(|&t| self.definition(t))
            }
            _ => None,
        }
    }

    /// `IType.IsReferenceType`.
    pub fn is_reference_type(&mut self, id: TypeId) -> Option<bool> {
        match self.data(id).clone() {
            TypeData::Def(def) => Some(!matches!(
                self.comp.type_def(def).ok()?.kind,
                TypeKind::Struct | TypeKind::Enum | TypeKind::Void
            )),
            TypeData::Unknown {
                is_reference_type, ..
            } => is_reference_type,
            TypeData::Inst { generic, .. } => self.is_reference_type(generic),
            TypeData::Annotated { inner, .. } | TypeData::Modified { inner, .. } => {
                self.is_reference_type(inner)
            }
            TypeData::Array { .. } | TypeData::Dynamic => Some(true),
            TypeData::Pinned(e) => self.is_reference_type(e),
            TypeData::Tuple { .. } | TypeData::NInt | TypeData::NUInt | TypeData::FnPtr(_) => {
                Some(false)
            }
            // `PointerType` and `ByReferenceType` answer null.
            TypeData::Pointer(_) | TypeData::ByRef(_) => None,
            TypeData::TypeParam { owner, index } => {
                self.type_parameter_is_reference_type(owner, index)
            }
            TypeData::UnknownSpecial | TypeData::Unbound | TypeData::ArgList => None,
        }
    }

    /// `IType.FullName`.
    pub fn full_name(&self, id: TypeId) -> String {
        match self.data(id) {
            TypeData::Def(def) => self.def_full_name(*def),
            TypeData::Unknown { name, .. } => {
                let mut s = format!("{}.{}", name.top.ns, name.top.name);
                for (n, _) in &name.nested {
                    s.push('.');
                    s.push_str(n);
                }
                s
            }
            TypeData::Inst { generic, .. } | TypeData::Annotated { inner: generic, .. } => {
                self.full_name(*generic)
            }
            _ => self.reflection_name(id),
        }
    }

    /// `MetadataTypeDefinition.FullName`: enclosing types joined with `.`.
    fn def_full_name(&self, def: TypeDefId) -> String {
        let Ok(info) = self.comp.type_def(def) else {
            return "!bad".to_owned();
        };
        let name = info.full_name.name();
        if info.declaring != 0 {
            let outer = TypeDefId {
                module: def.module,
                row: info.declaring,
            };
            format!("{}.{name}", self.def_full_name(outer))
        } else if !info.full_name.top.ns.is_empty() {
            format!("{}.{name}", info.full_name.top.ns)
        } else {
            name.to_owned()
        }
    }

    /// `IType.ReflectionName`.
    pub fn reflection_name(&self, id: TypeId) -> String {
        match self.data(id) {
            TypeData::Def(def) => self
                .comp
                .type_def(*def)
                .map_or_else(|_| "!bad".to_owned(), |i| i.full_name.reflection_name()),
            TypeData::Unknown { name, .. } => name.reflection_name(),
            TypeData::UnknownSpecial => "?".to_owned(),
            TypeData::Unbound => String::new(),
            TypeData::TypeParam { owner, index } => {
                format!("{}{index}", if owner.is_method() { "``" } else { "`" })
            }
            TypeData::Inst { generic, args } => {
                let args: Vec<String> = args
                    .iter()
                    .map(|a| format!("[{}]", self.reflection_name(*a)))
                    .collect();
                format!("{}[{}]", self.reflection_name(*generic), args.join(","))
            }
            TypeData::Array { elem, rank, .. } => {
                format!(
                    "{}[{}]",
                    self.reflection_name(*elem),
                    ",".repeat(rank.saturating_sub(1) as usize)
                )
            }
            TypeData::Pointer(t) => format!("{}*", self.reflection_name(*t)),
            TypeData::ByRef(t) => format!("{}&", self.reflection_name(*t)),
            TypeData::Pinned(t) => format!("{} pinned", self.reflection_name(*t)),
            TypeData::FnPtr(_) => "delegate*".to_owned(),
            TypeData::ArgList => "__arglist".to_owned(),
            TypeData::Modified {
                modifier,
                inner,
                required,
            } => format!(
                "{} {}({})",
                self.reflection_name(*inner),
                if *required { "modreq" } else { "modopt" },
                self.full_name(*modifier)
            ),
            TypeData::Dynamic => "dynamic".to_owned(),
            TypeData::NInt => "nint".to_owned(),
            TypeData::NUInt => "nuint".to_owned(),
            TypeData::Tuple { underlying, .. } => self.reflection_name(*underlying),
            TypeData::Annotated { inner, .. } => self.reflection_name(*inner),
        }
    }
}

fn is_reference_type(raw_kind: u8) -> Option<bool> {
    match raw_kind {
        0x11 => Some(false),
        0x12 => Some(true),
        _ => None,
    }
}

fn primitive_byte(p: signature::Primitive) -> u8 {
    use signature::Primitive::*;
    match p {
        Void => 0x01,
        Boolean => 0x02,
        Char => 0x03,
        SByte => 0x04,
        Byte => 0x05,
        Int16 => 0x06,
        UInt16 => 0x07,
        Int32 => 0x08,
        UInt32 => 0x09,
        Int64 => 0x0A,
        UInt64 => 0x0B,
        Single => 0x0C,
        Double => 0x0D,
        String => 0x0E,
        TypedReference => 0x16,
        IntPtr => 0x18,
        UIntPtr => 0x19,
        Object => 0x1C,
    }
}
