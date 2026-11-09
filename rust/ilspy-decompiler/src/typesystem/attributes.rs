//! Attribute lists as the managed entities report them (`AttributeListBuilder` and the
//! `GetAttributes` of `MetadataTypeDefinition`, `MetadataMethod`, `MetadataField`,
//! `MetadataParameter`, `MetadataProperty`, `MetadataEvent`, `MetadataTypeParameter` and the
//! module): pseudo-attributes synthesized from metadata flags and tables, then the custom
//! attributes the type system options do not consume, then security declarations.

use crate::metadata::bytes::Bytes;
use crate::metadata::tables::{CodedKind, TableId, rows};
use crate::metadata::{Error, Result};

use super::attrvalue::{ArgValue, NamedArg, TypedArg, serialized_string, try_compressed};
use super::compilation::{ModuleId, TypeDefId, TypeKind, is_known_type_name};
use super::known::KnownTypeCode;
use super::members::{ConstValue, MethodDefId, ParamInfo, RefKind, SymbolKind};
use super::names::TopLevelTypeName;
use super::options;
use super::resolve::ResolvedMethod;
use super::types::{GenericContext, TypeData, TypeId, TypeSystem};

/// An attribute: its type, constructor (`None` for a built-in attribute, whose constructor is
/// looked up by `builtin_constructor`), arguments and whether decoding them failed.
#[derive(Clone, Debug)]
pub struct Attribute {
    pub ty: TypeId,
    pub ctor: Option<ResolvedMethod>,
    pub fixed: Vec<TypedArg>,
    pub named: Vec<NamedArg>,
    pub has_decode_errors: bool,
}

/// The `SymbolKind` an attribute list belongs to, as `IgnoreAttribute` distinguishes them.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum Target {
    Module,
    TypeDefinition,
    Field,
    Member(SymbolKind),
    Parameter,
    ReturnType,
    TypeParameter,
}

const INTEROP: &str = "System.Runtime.InteropServices";
const COMPILER_SERVICES: &str = "System.Runtime.CompilerServices";

const SERIALIZABLE: (&str, &str) = ("System", "SerializableAttribute");
const COM_IMPORT: (&str, &str) = (INTEROP, "ComImportAttribute");
const SPECIAL_NAME: (&str, &str) = (COMPILER_SERVICES, "SpecialNameAttribute");
const STRUCT_LAYOUT: (&str, &str) = (INTEROP, "StructLayoutAttribute");
const DLL_IMPORT: (&str, &str) = (INTEROP, "DllImportAttribute");
const PRESERVE_SIG: (&str, &str) = (INTEROP, "PreserveSigAttribute");
const METHOD_IMPL: (&str, &str) = (COMPILER_SERVICES, "MethodImplAttribute");
const FIELD_OFFSET: (&str, &str) = (INTEROP, "FieldOffsetAttribute");
const NON_SERIALIZED: (&str, &str) = ("System", "NonSerializedAttribute");
const MARSHAL_AS: (&str, &str) = (INTEROP, "MarshalAsAttribute");
const PERMISSION_SET: (&str, &str) = ("System.Security.Permissions", "PermissionSetAttribute");
const OPTIONAL: (&str, &str) = (INTEROP, "OptionalAttribute");
const DEFAULT_PARAMETER_VALUE: (&str, &str) = (INTEROP, "DefaultParameterValueAttribute");
const IN: (&str, &str) = (INTEROP, "InAttribute");
const OUT: (&str, &str) = (INTEROP, "OutAttribute");
const INDEXER_NAME: (&str, &str) = (COMPILER_SERVICES, "IndexerNameAttribute");
const ASSEMBLY_VERSION: (&str, &str) = ("System.Reflection", "AssemblyVersionAttribute");
const TYPE_FORWARDED_TO: (&str, &str) = (COMPILER_SERVICES, "TypeForwardedToAttribute");
const COMPILER_GENERATED: (&str, &str) = (COMPILER_SERVICES, "CompilerGeneratedAttribute");

/// `AttributeListBuilder.IgnoreAttribute(TopLevelTypeName, SymbolKind)`.
fn ignored(options: u32, ns: &str, name: &str, target: Target) -> bool {
    let method_like = matches!(
        target,
        Target::Member(
            SymbolKind::Method
                | SymbolKind::Operator
                | SymbolKind::Constructor
                | SymbolKind::Destructor
                | SymbolKind::Accessor
        )
    );
    let on = |bit: u32| options & bit != 0;
    match ns {
        COMPILER_SERVICES => match name {
            "DynamicAttribute" => on(options::DYNAMIC),
            "NativeIntegerAttribute" => on(options::NATIVE_INTEGERS),
            "TupleElementNamesAttribute" => on(options::TUPLE),
            "ExtensionAttribute" => on(options::EXTENSION_METHODS),
            "DecimalConstantAttribute" => {
                on(options::DECIMAL_CONSTANTS)
                    && matches!(target, Target::Field | Target::Parameter)
            }
            "IsReadOnlyAttribute" => match target {
                Target::TypeDefinition | Target::Parameter => {
                    on(options::READ_ONLY_STRUCTS_AND_PARAMETERS)
                }
                Target::Member(SymbolKind::Method | SymbolKind::Accessor) => {
                    on(options::READ_ONLY_METHODS)
                }
                Target::ReturnType
                | Target::Member(SymbolKind::Property | SymbolKind::Indexer)
                | Target::Field => true,
                _ => false,
            },
            "IsByRefLikeAttribute" => on(options::REF_STRUCTS) && target == Target::TypeDefinition,
            "IsUnmanagedAttribute" => {
                on(options::UNMANAGED_CONSTRAINTS) && target == Target::TypeParameter
            }
            "NullableAttribute" => on(options::NULLABILITY),
            "NullableContextAttribute" => {
                on(options::NULLABILITY) && (target == Target::TypeDefinition || method_like)
            }
            "ScopedRefAttribute" => on(options::SCOPED_REF) && target == Target::Parameter,
            "RequiresLocationAttribute" => {
                on(options::REF_READ_ONLY_PARAMETERS) && target == Target::Parameter
            }
            "ParamCollectionAttribute" => {
                on(options::PARAMS_COLLECTIONS) && target == Target::Parameter
            }
            _ => false,
        },
        "System" => name == "ParamArrayAttribute" && target == Target::Parameter,
        _ => false,
    }
}

fn int(ty: TypeId, value: i32) -> TypedArg {
    TypedArg {
        ty,
        value: ArgValue::Int32(value),
    }
}

/// `object` constant of a `DefaultParameterValue` argument.
fn constant_value(v: &ConstValue) -> ArgValue {
    match v {
        ConstValue::Null => ArgValue::Null,
        ConstValue::Boolean(b) => ArgValue::Bool(*b),
        ConstValue::Char(c) => ArgValue::Char(*c),
        ConstValue::SByte(x) => ArgValue::SByte(*x),
        ConstValue::Byte(x) => ArgValue::Byte(*x),
        ConstValue::Int16(x) => ArgValue::Int16(*x),
        ConstValue::UInt16(x) => ArgValue::UInt16(*x),
        ConstValue::Int32(x) => ArgValue::Int32(*x),
        ConstValue::UInt32(x) => ArgValue::UInt32(*x),
        ConstValue::Int64(x) => ArgValue::Int64(*x),
        ConstValue::UInt64(x) => ArgValue::UInt64(*x),
        ConstValue::Single(bits) => ArgValue::Single(*bits),
        ConstValue::Double(bits) => ArgValue::Double(*bits),
        ConstValue::String(units) => ArgValue::String(units.clone()),
        ConstValue::Decimal(words) => ArgValue::Decimal(*words),
    }
}

impl TypeSystem<'_, '_> {
    fn known_attribute_type(&mut self, (ns, name): (&str, &str)) -> TypeId {
        self.find_top_level(TopLevelTypeName::new(ns, name, 0))
    }

    fn interop_type(&mut self, name: &str) -> TypeId {
        self.find_top_level(TopLevelTypeName::new(INTEROP, name, 0))
    }

    /// A built-in attribute (`DefaultAttribute`).
    fn builtin(
        &mut self,
        known: (&str, &str),
        fixed: Vec<TypedArg>,
        named: Vec<NamedArg>,
    ) -> Attribute {
        Attribute {
            ty: self.known_attribute_type(known),
            ctor: None,
            fixed,
            named,
            has_decode_errors: false,
        }
    }

    /// `AttributeBuilder.AddNamedArg`: a field when the attribute type (or a base class)
    /// declares a field of that name, else a property.
    fn named_arg(
        &mut self,
        attribute: TypeId,
        name: &str,
        ty: TypeId,
        value: ArgValue,
    ) -> Result<NamedArg> {
        let mut is_field = false;
        for base in self.non_interface_base_types(attribute)? {
            let Some(def) = self.definition(base) else {
                continue;
            };
            let md = &self.comp.module(def.module).md;
            for row in self.comp.visible_fields(def)? {
                if md.string(md.get::<rows::Field>(row)?.name)? == name {
                    is_field = true;
                }
            }
        }
        Ok(NamedArg {
            is_field,
            name: Some(name.to_owned()),
            arg: TypedArg { ty, value },
        })
    }

    /// `DefaultAttribute.Constructor`: the attribute type's constructor whose parameter types
    /// are the fixed arguments' types.
    pub fn builtin_constructor(&mut self, attribute: &Attribute) -> Result<Option<ResolvedMethod>> {
        let Some(def) = self.definition(attribute.ty) else {
            return Ok(None);
        };
        let types: Vec<TypeId> = attribute.fixed.iter().map(|a| a.ty).collect();
        for ctor in self.definition_constructors(def)? {
            if ctor.params()? == types {
                return Ok(Some(ctor));
            }
        }
        Ok(None)
    }

    /// `AttributeListBuilder.Add(CustomAttributeHandleCollection, SymbolKind)`.
    fn add_custom(
        &mut self,
        out: &mut Vec<Attribute>,
        module: ModuleId,
        table: TableId,
        row: u32,
        target: Target,
    ) -> Result<()> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let options = self.options(module);
        for ca in md.custom_attributes(table, row) {
            let token = md.get::<rows::CustomAttribute>(ca)?.constructor.token()?;
            let ctor = self.resolve_method(module, token, GenericContext::default())?;
            let ty = ctor.declaring.ok_or(Error::Unsupported(
                "attribute constructor without declaring type",
            ))?;
            if self.attribute_ignored(ty, options, target) {
                continue;
            }
            let (fixed, named, has_decode_errors) = match self.decode_attribute(module, ca) {
                Ok(v) => (v.fixed, v.named, false),
                Err(_) => (Vec::new(), Vec::new(), true),
            };
            out.push(Attribute {
                ty,
                ctor: Some(ctor),
                fixed,
                named,
                has_decode_errors,
            });
        }
        Ok(())
    }

    /// `IgnoreAttribute(IType, SymbolKind)`: only top-level, non-generic types are ignored.
    fn attribute_ignored(&self, ty: TypeId, options: u32, target: Target) -> bool {
        let top = match self.data(ty) {
            TypeData::Def(def) => match self.comp.type_def(*def) {
                Ok(info) if info.declaring == 0 && info.type_parameter_count == 0 => {
                    info.full_name.top.clone()
                }
                _ => return false,
            },
            TypeData::Unknown { name, .. } if name.nested.is_empty() && name.top.tpc == 0 => {
                name.top.clone()
            }
            _ => return false,
        };
        ignored(options, &top.ns, &top.name, target)
    }

    /// `AttributeListBuilder.AddSecurityAttributes`: a declaration that fails to decode adds
    /// nothing.
    fn add_security(
        &mut self,
        out: &mut Vec<Attribute>,
        module: ModuleId,
        table: TableId,
        row: u32,
    ) -> Result<()> {
        let md = &self.comp.module(module).md;
        let Some(key) = CodedKind::HasDeclSecurity.encode(table, row) else {
            return Ok(());
        };
        let rows: Vec<u32> = md
            .tables
            .binary_search_range(TableId::DeclSecurity, 1, key)
            .collect();
        for r in rows {
            let mut decoded = Vec::new();
            if self.security_attributes(&mut decoded, module, r).is_ok() {
                out.extend(decoded);
            }
        }
        Ok(())
    }

    fn security_attributes(
        &mut self,
        out: &mut Vec<Attribute>,
        module: ModuleId,
        row: u32,
    ) -> Result<()> {
        let md = &self.comp.module(module).md;
        let decl = md.get::<rows::DeclSecurity>(row)?;
        let action_type = self.find_top_level(TopLevelTypeName::new(
            "System.Security.Permissions",
            "SecurityAction",
            0,
        ));
        let action = int(action_type, i32::from(decl.action));
        let blob = md.blob(decl.permission_set)?;
        let mut b = Bytes::new(blob);
        if b.u8()? == b'.' {
            let count = b.compressed_u32()?;
            for _ in 0..count {
                let name = serialized_string(&mut b)?;
                let ty = match name {
                    Some(n) => self.parse_reflection_name(&n)?,
                    None => return Err(Error::Malformed("null security attribute type")),
                };
                b.compressed_u32()?;
                let named_count = b.compressed_u32()?;
                let named = self.decode_named_arguments(&mut b, named_count)?;
                out.push(Attribute {
                    ty,
                    ctor: None,
                    fixed: vec![action.clone()],
                    named,
                    has_decode_errors: false,
                });
            }
        } else {
            let units: Vec<u16> = blob
                .as_chunks::<2>()
                .0
                .iter()
                .map(|c| u16::from_le_bytes(*c))
                .collect();
            let ty = self.known_attribute_type(PERMISSION_SET);
            let string = self.known_type(KnownTypeCode::String);
            let xml = self.named_arg(ty, "XML", string, ArgValue::String(units))?;
            out.push(self.builtin(PERMISSION_SET, vec![action], vec![xml]));
        }
        Ok(())
    }

    /// `AttributeListBuilder.ConvertMarshalInfo` for a Field or Param with a FieldMarshal row.
    fn add_marshal_info(
        &mut self,
        out: &mut Vec<Attribute>,
        module: ModuleId,
        table: TableId,
        row: u32,
    ) -> Result<()> {
        let md = &self.comp.module(module).md;
        let Some(key) = CodedKind::HasFieldMarshal.encode(table, row) else {
            return Ok(());
        };
        let Some(r) = md.tables.binary_search(TableId::FieldMarshal, 0, key) else {
            return Ok(());
        };
        let mut b = Bytes::new(md.blob(md.get::<rows::FieldMarshal>(r)?.native_type)?);
        let attribute_type = self.known_attribute_type(MARSHAL_AS);
        let unmanaged_type = self.interop_type("UnmanagedType");
        let int32 = self.known_type(KnownTypeCode::Int32);
        let kind = i32::from(b.u8()?);
        let mut named = Vec::new();
        match kind {
            0x1e => {
                let size = try_compressed(&mut b).map_or(0, |s| s as i32);
                named.push(self.named_arg(
                    attribute_type,
                    "SizeConst",
                    int32,
                    ArgValue::Int32(size),
                )?);
                if b.remaining() > 0 {
                    let sub = i32::from(b.u8()?);
                    if sub != 0x66 {
                        named.push(self.named_arg(
                            attribute_type,
                            "ArraySubType",
                            unmanaged_type,
                            ArgValue::Int32(sub),
                        )?);
                    }
                }
            }
            0x1d => {
                if b.remaining() > 0 {
                    let var = i32::from(b.u8()?);
                    if var != 0 {
                        let var_enum = self.interop_type("VarEnum");
                        named.push(self.named_arg(
                            attribute_type,
                            "SafeArraySubType",
                            var_enum,
                            ArgValue::Int32(var),
                        )?);
                    }
                }
            }
            0x2a => {
                let sub = if b.remaining() > 0 {
                    i32::from(b.u8()?)
                } else {
                    0x66
                };
                if sub != 0x50 {
                    named.push(self.named_arg(
                        attribute_type,
                        "ArraySubType",
                        unmanaged_type,
                        ArgValue::Int32(sub),
                    )?);
                }
                let mut next = || try_compressed(&mut b).map_or(-1, |v| v as i32);
                let size_parameter_index = next();
                let size = next();
                let size_parameter_multiplier = next();
                if size >= 0 {
                    named.push(self.named_arg(
                        attribute_type,
                        "SizeConst",
                        int32,
                        ArgValue::Int32(size),
                    )?);
                }
                if size_parameter_multiplier != 0 && size_parameter_index >= 0 {
                    let int16 = self.known_type(KnownTypeCode::Int16);
                    named.push(self.named_arg(
                        attribute_type,
                        "SizeParamIndex",
                        int16,
                        ArgValue::Int16(size_parameter_index as i16),
                    )?);
                }
            }
            0x2c => {
                let _guid = serialized_string(&mut b)?;
                let _unmanaged = serialized_string(&mut b)?;
                let managed = serialized_string(&mut b)?;
                let cookie = serialized_string(&mut b)?;
                let string = self.known_type(KnownTypeCode::String);
                if let Some(m) = managed {
                    named.push(self.named_arg(
                        attribute_type,
                        "MarshalType",
                        string,
                        ArgValue::String(m.encode_utf16().collect()),
                    )?);
                }
                if let Some(c) = cookie.filter(|c| !c.is_empty()) {
                    named.push(self.named_arg(
                        attribute_type,
                        "MarshalCookie",
                        string,
                        ArgValue::String(c.encode_utf16().collect()),
                    )?);
                }
            }
            0x17 => {
                let size = b.compressed_u32()? as i32;
                named.push(self.named_arg(
                    attribute_type,
                    "SizeConst",
                    int32,
                    ArgValue::Int32(size),
                )?);
            }
            _ => {}
        }
        out.push(Attribute {
            ty: attribute_type,
            ctor: None,
            fixed: vec![int(unmanaged_type, kind)],
            named,
            has_decode_errors: false,
        });
        Ok(())
    }

    /// `ITypeDefinition.GetAttributes()`.
    pub fn type_attributes(&mut self, def: TypeDefId) -> Result<Vec<Attribute>> {
        let comp = self.comp;
        let md = &comp.module(def.module).md;
        let info = comp.type_def(def)?;
        let flags = info.flags;
        let mut out = Vec::new();
        if flags & 0x2000 != 0 {
            out.push(self.builtin(SERIALIZABLE, Vec::new(), Vec::new()));
        }
        if flags & 0x1000 != 0 {
            out.push(self.builtin(COM_IMPORT, Vec::new(), Vec::new()));
        }
        if flags & (0x400 | 0x800) == 0x400 {
            out.push(self.builtin(SPECIAL_NAME, Vec::new(), Vec::new()));
        }
        let layout_kind = match flags & 0x18 {
            0x08 => 0,
            0x10 => 2,
            _ => 3,
        };
        let char_set = match flags & 0x30000 {
            0 => 2,
            0x10000 => 3,
            0x20000 => 4,
            _ => 1,
        };
        let (packing, size) = md.type_layout(def.row)?;
        let default_layout = if info.kind == TypeKind::Struct { 0 } else { 3 };
        if layout_kind != default_layout || char_set != 2 || packing > 0 || size > 0 {
            let attribute_type = self.known_attribute_type(STRUCT_LAYOUT);
            let layout_type = self.interop_type("LayoutKind");
            let mut named = Vec::new();
            if char_set != 2 {
                let char_set_type = self.interop_type("CharSet");
                named.push(self.named_arg(
                    attribute_type,
                    "CharSet",
                    char_set_type,
                    ArgValue::Int32(char_set),
                )?);
            }
            let int32 = self.known_type(KnownTypeCode::Int32);
            if packing > 0 {
                named.push(self.named_arg(
                    attribute_type,
                    "Pack",
                    int32,
                    ArgValue::Int32(i32::from(packing)),
                )?);
            }
            if size > 0 {
                named.push(self.named_arg(
                    attribute_type,
                    "Size",
                    int32,
                    ArgValue::Int32(size as i32),
                )?);
            }
            out.push(Attribute {
                ty: attribute_type,
                ctor: None,
                fixed: vec![int(layout_type, layout_kind)],
                named,
                has_decode_errors: false,
            });
        }
        self.add_custom(
            &mut out,
            def.module,
            TableId::TypeDef,
            def.row,
            Target::TypeDefinition,
        )?;
        self.add_security(&mut out, def.module, TableId::TypeDef, def.row)?;
        Ok(out)
    }

    /// `IMethod.GetAttributes()`.
    pub fn method_attributes(&mut self, m: MethodDefId) -> Result<Vec<Attribute>> {
        let comp = self.comp;
        let md = &comp.module(m.module).md;
        let def = md.get::<rows::MethodDef>(m.row)?;
        let options = self.options(m.module);
        let mut impl_flags = def.impl_flags & !0x0003;
        if options & options::RUNTIME_ASYNC != 0 {
            impl_flags &= !0x2000;
        }
        let code_type = i32::from(def.impl_flags & 0x0003);
        let mut out = Vec::new();

        let import = md.method_import(m.row);
        let imp = if import == 0 {
            None
        } else {
            Some(md.get::<rows::ImplMap>(import)?)
        };
        if def.flags & 0x2000 != 0
            && let Some(imp) = imp.filter(|i| i.import_scope != 0)
        {
            let attribute_type = self.known_attribute_type(DLL_IMPORT);
            let string = self.known_type(KnownTypeCode::String);
            let boolean = self.known_type(KnownTypeCode::Boolean);
            let dll = md.string(md.get::<rows::ModuleRef>(imp.import_scope)?.name)?;
            let mut named = Vec::new();
            let a = imp.flags;
            let flag = |ts: &mut Self, name: &str, value: bool| {
                ts.named_arg(attribute_type, name, boolean, ArgValue::Bool(value))
            };
            if a & 0x0020 == 0x0020 {
                named.push(flag(self, "BestFitMapping", false)?);
            }
            if a & 0x0010 == 0x0010 {
                named.push(flag(self, "BestFitMapping", true)?);
            }
            let calling_convention = match a & 0x0700 {
                0 => 0,
                0x0200 => 2,
                0x0500 => 5,
                0x0300 => 3,
                0x0400 => 4,
                0x0100 => 1,
                _ => return Err(Error::Unsupported("unknown calling convention")),
            };
            if calling_convention != 1 {
                let ty = self.interop_type("CallingConvention");
                named.push(self.named_arg(
                    attribute_type,
                    "CallingConvention",
                    ty,
                    ArgValue::Int32(calling_convention),
                )?);
            }
            let char_set = match a & 0x0006 {
                0x0002 => 2,
                0x0006 => 4,
                0x0004 => 3,
                _ => 1,
            };
            if char_set != 1 {
                let ty = self.interop_type("CharSet");
                named.push(self.named_arg(
                    attribute_type,
                    "CharSet",
                    ty,
                    ArgValue::Int32(char_set),
                )?);
            }
            if imp.import_name.0 != 0 && md.string(imp.import_name)? != md.string(def.name)? {
                let entry = md.string(imp.import_name)?.encode_utf16().collect();
                named.push(self.named_arg(
                    attribute_type,
                    "EntryPoint",
                    string,
                    ArgValue::String(entry),
                )?);
            }
            if a & 0x0001 == 0x0001 {
                named.push(flag(self, "ExactSpelling", true)?);
            }
            if impl_flags & 0x0080 == 0x0080 {
                impl_flags &= !0x0080;
            } else {
                named.push(flag(self, "PreserveSig", false)?);
            }
            if a & 0x0040 == 0x0040 {
                named.push(flag(self, "SetLastError", true)?);
            }
            if a & 0x2000 == 0x2000 {
                named.push(flag(self, "ThrowOnUnmappableChar", false)?);
            }
            if a & 0x1000 == 0x1000 {
                named.push(flag(self, "ThrowOnUnmappableChar", true)?);
            }
            let dll = TypedArg {
                ty: string,
                value: ArgValue::String(dll.encode_utf16().collect()),
            };
            out.push(Attribute {
                ty: attribute_type,
                ctor: None,
                fixed: vec![dll],
                named,
                has_decode_errors: false,
            });
        }

        if impl_flags == 0x0080 && code_type == 0 {
            out.push(self.builtin(PRESERVE_SIG, Vec::new(), Vec::new()));
            impl_flags = 0;
        }
        if impl_flags != 0 {
            let attribute_type = self.known_attribute_type(METHOD_IMPL);
            let options_type = self.find_top_level(TopLevelTypeName::new(
                COMPILER_SERVICES,
                "MethodImplOptions",
                0,
            ));
            let mut named = Vec::new();
            if code_type != 0 {
                let code_type_type = self.find_top_level(TopLevelTypeName::new(
                    COMPILER_SERVICES,
                    "MethodCodeType",
                    0,
                ));
                named.push(self.named_arg(
                    attribute_type,
                    "MethodCodeType",
                    code_type_type,
                    ArgValue::Int32(code_type),
                )?);
            }
            out.push(Attribute {
                ty: attribute_type,
                ctor: None,
                fixed: vec![int(options_type, i32::from(impl_flags))],
                named,
                has_decode_errors: false,
            });
        }

        let owner = self.accessor_owner(m);
        let kind = self.method_symbol_kind(m, owner)?;
        if def.flags & (0x0800 | 0x1000) == 0x0800 && kind == SymbolKind::Method {
            out.push(self.builtin(SPECIAL_NAME, Vec::new(), Vec::new()));
        }
        self.add_custom(
            &mut out,
            m.module,
            TableId::MethodDef,
            m.row,
            Target::Member(kind),
        )?;
        self.add_security(&mut out, m.module, TableId::MethodDef, m.row)?;
        Ok(out)
    }

    /// `IMethod.GetReturnTypeAttributes()`.
    pub fn return_type_attributes(&mut self, m: MethodDefId) -> Result<Vec<Attribute>> {
        let md = &self.comp.module(m.module).md;
        let mut out = Vec::new();
        if let Some(&first) = md.method_params(m.row)?.first()
            && md.get::<rows::Param>(first)?.sequence == 0
        {
            self.add_marshal_info(&mut out, m.module, TableId::Param, first)?;
            self.add_custom(
                &mut out,
                m.module,
                TableId::Param,
                first,
                Target::ReturnType,
            )?;
        }
        Ok(out)
    }

    /// `IField.GetAttributes()`.
    pub fn field_attributes(&mut self, module: ModuleId, row: u32) -> Result<Vec<Attribute>> {
        let md = &self.comp.module(module).md;
        let f = md.get::<rows::Field>(row)?;
        let mut out = Vec::new();
        let offset = md.field_offset(row)?;
        if offset != -1 {
            let int32 = self.known_type(KnownTypeCode::Int32);
            out.push(self.builtin(FIELD_OFFSET, vec![int(int32, offset as i32)], Vec::new()));
        }
        if f.flags & 0x0080 != 0 {
            out.push(self.builtin(NON_SERIALIZED, Vec::new(), Vec::new()));
        }
        if f.flags & (0x0200 | 0x0400) == 0x0200 {
            out.push(self.builtin(SPECIAL_NAME, Vec::new(), Vec::new()));
        }
        self.add_marshal_info(&mut out, module, TableId::Field, row)?;
        self.add_custom(&mut out, module, TableId::Field, row, Target::Field)?;
        Ok(out)
    }

    /// `IParameter.IsDefaultValueAssignmentAllowed()` for parameter `index` of `params`,
    /// declared by `declaring`.
    fn default_value_assignment_allowed(
        &mut self,
        params: &[ParamInfo],
        index: usize,
        declaring: Option<TypeDefId>,
    ) -> Result<bool> {
        let individual = |p: &ParamInfo| {
            p.optional
                && p.has_constant_in_signature
                && matches!(
                    p.ref_kind,
                    RefKind::None | RefKind::In | RefKind::RefReadOnly
                )
        };
        if !individual(&params[index]) {
            return Ok(false);
        }
        for other in params[index + 1..].iter().rev() {
            if self.is_closure_parameter(other, declaring)? {
                continue;
            }
            if individual(other) || other.is_params {
                continue;
            }
            return Ok(false);
        }
        Ok(true)
    }

    /// `LocalFunctionDecompiler.IsClosureParameter`: a `ref` to a compiler-generated struct in
    /// the same type tree as `current`.
    fn is_closure_parameter(&mut self, p: &ParamInfo, current: Option<TypeDefId>) -> Result<bool> {
        let TypeData::ByRef(elem) = self.data(p.ty).clone() else {
            return Ok(false);
        };
        let Some(def) = self.definition(elem) else {
            return Ok(false);
        };
        if self.comp.type_def(def)?.kind != TypeKind::Struct {
            return Ok(false);
        }
        self.is_potential_closure(current, def)
    }

    fn declaring_type_def(&self, def: TypeDefId) -> Option<TypeDefId> {
        self.comp
            .type_def(def)
            .ok()
            .filter(|i| i.declaring != 0)
            .map(|i| TypeDefId {
                module: def.module,
                row: i.declaring,
            })
    }

    /// Whether the row carries `[CompilerGenerated]`.
    fn has_compiler_generated(&self, module: ModuleId, table: TableId, row: u32) -> bool {
        let md = &self.comp.module(module).md;
        md.custom_attributes(table, row).into_iter().any(|ca| {
            md.attribute_type(ca).is_ok_and(|t| {
                is_known_type_name(md, t, COMPILER_GENERATED.0, COMPILER_GENERATED.1, 0)
            })
        })
    }

    /// `IsCompilerGeneratedOrIsInCompilerGeneratedClass` of a method.
    pub fn method_is_compiler_generated_or_in_compiler_generated_class(
        &self,
        m: &ResolvedMethod,
    ) -> bool {
        if let super::resolve::MethodOrigin::Def(id) = m.origin
            && self.has_compiler_generated(id.module, TableId::MethodDef, id.row)
        {
            return true;
        }
        m.declaring
            .and_then(|t| self.definition(t))
            .is_some_and(|d| self.is_compiler_generated_or_in_compiler_generated_class(d))
    }

    /// `IsCompilerGeneratedOrIsInCompilerGeneratedClass` of a field.
    pub fn field_is_compiler_generated_or_in_compiler_generated_class(
        &self,
        field: &super::resolve::ResolvedField,
    ) -> bool {
        if let super::resolve::FieldOrigin::Def { module, row } = field.origin
            && self.has_compiler_generated(module, TableId::Field, row)
        {
            return true;
        }
        field
            .declaring
            .and_then(|t| self.definition(t))
            .is_some_and(|d| self.is_compiler_generated_or_in_compiler_generated_class(d))
    }

    /// `IMethod.IsOperator`.
    pub fn method_is_operator(&mut self, m: &ResolvedMethod) -> bool {
        let super::resolve::MethodOrigin::Def(id) = m.origin else {
            return false;
        };
        let accessor = self.accessor_owner(id);
        self.method_symbol_kind(id, accessor).ok() == Some(SymbolKind::Operator)
    }

    /// The name of the property of a getter (`IsAccessor`, `AccessorKind == Getter` and
    /// `AccessorOwner.Name`).
    pub fn getter_property_name(&mut self, m: &ResolvedMethod) -> Option<String> {
        let super::resolve::MethodOrigin::Def(id) = m.origin else {
            return None;
        };
        let (owner, semantics) = self.accessor_owner(id)?;
        // Table 0x17 is Property; semantics 0x02 is Getter.
        if owner >> 24 != 0x17 || semantics != 0x02 {
            return None;
        }
        self.member_name(super::inheritance::MemberDef::Property(
            id.module,
            owner & 0x00FF_FFFF,
        ))
        .ok()
    }

    /// The `ReturnType` of the property of a getter (`AccessorOwner.ReturnType`), with its
    /// nullability, as seen through the getter's class substitution.
    pub fn getter_property_type(&mut self, m: &ResolvedMethod) -> Option<TypeId> {
        let super::resolve::MethodOrigin::Def(id) = m.origin else {
            return None;
        };
        let (owner, semantics) = self.accessor_owner(id)?;
        if owner >> 24 != 0x17 || semantics != 0x02 {
            return None;
        }
        let (ty, _) = self
            .property_signature(id.module, owner & 0x00FF_FFFF)
            .ok()?;
        Some(self.substitute(ty, m.class_args.as_deref(), None))
    }

    fn is_compiler_generated_or_in_compiler_generated_class(&self, def: TypeDefId) -> bool {
        let mut current = Some(def);
        while let Some(d) = current {
            if self.has_compiler_generated(d.module, TableId::TypeDef, d.row) {
                return true;
            }
            current = self.declaring_type_def(d);
        }
        false
    }

    /// `TransformDisplayClassUsage.IsPotentialClosure` for a struct display class.
    fn is_potential_closure(
        &mut self,
        decompiled: Option<TypeDefId>,
        display: TypeDefId,
    ) -> Result<bool> {
        if !self.is_compiler_generated_or_in_compiler_generated_class(display) {
            return Ok(false);
        }
        let mut ancestors = Vec::new();
        let mut parent = self.declaring_type_def(display);
        while let Some(p) = parent {
            ancestors.push(p);
            parent = self.declaring_type_def(p);
        }
        let mut current = decompiled;
        while let Some(c) = current {
            if ancestors.contains(&c) {
                return Ok(true);
            }
            current = self.declaring_type_def(c);
        }
        Ok(false)
    }

    /// `IParameter.GetAttributes()` of parameter `index` of a method or property with these
    /// `params`, declared by `declaring`.
    pub fn parameter_attributes(
        &mut self,
        module: ModuleId,
        params: &[ParamInfo],
        index: usize,
        declaring: Option<TypeDefId>,
    ) -> Result<Vec<Attribute>> {
        let p = &params[index];
        let mut out = Vec::new();
        if p.row == 0 {
            return Ok(out);
        }
        let allowed = self.default_value_assignment_allowed(params, index, declaring)?;
        if p.optional && !allowed {
            out.push(self.builtin(OPTIONAL, Vec::new(), Vec::new()));
        }
        if !p.is_decimal_constant && p.has_constant_in_signature && !allowed {
            let object = self.known_type(KnownTypeCode::Object);
            let arg = TypedArg {
                ty: object,
                value: constant_value(&p.value),
            };
            out.push(self.builtin(DEFAULT_PARAMETER_VALUE, vec![arg], Vec::new()));
        }
        if p.flags & 0x0001 != 0 && !matches!(p.ref_kind, RefKind::In | RefKind::RefReadOnly) {
            out.push(self.builtin(IN, Vec::new(), Vec::new()));
        }
        if p.flags & 0x0002 != 0 && p.ref_kind != RefKind::Out {
            out.push(self.builtin(OUT, Vec::new(), Vec::new()));
        }
        let row = p.row;
        self.add_custom(&mut out, module, TableId::Param, row, Target::Parameter)?;
        self.add_marshal_info(&mut out, module, TableId::Param, row)?;
        Ok(out)
    }

    /// `IProperty.GetAttributes()`.
    pub fn property_attributes(&mut self, module: ModuleId, row: u32) -> Result<Vec<Attribute>> {
        let md = &self.comp.module(module).md;
        let p = md.get::<rows::Property>(row)?;
        let name = md.string(p.name)?.into_owned();
        let kind = self.property_symbol_kind(module, row)?;
        let explicit = match self.property_any_accessor(module, row) {
            Some(a) => self.is_explicit_interface_implementation(a)?,
            None => false,
        };
        let mut out = Vec::new();
        if kind == SymbolKind::Indexer && name != "Item" && !explicit {
            let string = self.known_type(KnownTypeCode::String);
            let arg = TypedArg {
                ty: string,
                value: ArgValue::String(name.encode_utf16().collect()),
            };
            out.push(self.builtin(INDEXER_NAME, vec![arg], Vec::new()));
        }
        if p.flags & (0x0200 | 0x0400) == 0x0200 {
            out.push(self.builtin(SPECIAL_NAME, Vec::new(), Vec::new()));
        }
        self.add_custom(
            &mut out,
            module,
            TableId::Property,
            row,
            Target::Member(kind),
        )?;
        Ok(out)
    }

    /// `IEvent.GetAttributes()`.
    pub fn event_attributes(&mut self, module: ModuleId, row: u32) -> Result<Vec<Attribute>> {
        let md = &self.comp.module(module).md;
        let e = md.get::<rows::Event>(row)?;
        let mut out = Vec::new();
        if e.flags & (0x0200 | 0x0400) == 0x0200 {
            out.push(self.builtin(SPECIAL_NAME, Vec::new(), Vec::new()));
        }
        self.add_custom(
            &mut out,
            module,
            TableId::Event,
            row,
            Target::Member(SymbolKind::Event),
        )?;
        Ok(out)
    }

    /// `ITypeParameter.GetAttributes()` of a GenericParam row.
    pub fn type_parameter_attributes(
        &mut self,
        module: ModuleId,
        row: u32,
    ) -> Result<Vec<Attribute>> {
        let mut out = Vec::new();
        self.add_custom(
            &mut out,
            module,
            TableId::GenericParam,
            row,
            Target::TypeParameter,
        )?;
        Ok(out)
    }

    /// `MetadataModule.GetAssemblyAttributes()`.
    pub fn assembly_attributes(&mut self, module: ModuleId) -> Result<Vec<Attribute>> {
        let md = &self.comp.module(module).md;
        let mut out = Vec::new();
        if md.tables.rows(TableId::Assembly) == 0 {
            return Ok(out);
        }
        self.add_custom(&mut out, module, TableId::Assembly, 1, Target::Module)?;
        self.add_security(&mut out, module, TableId::Assembly, 1)?;
        let a = md.get::<rows::Assembly>(1)?;
        let version = format!("{}.{}.{}.{}", a.major, a.minor, a.build, a.revision);
        let string = self.known_type(KnownTypeCode::String);
        let arg = TypedArg {
            ty: string,
            value: ArgValue::String(version.encode_utf16().collect()),
        };
        out.push(self.builtin(ASSEMBLY_VERSION, vec![arg], Vec::new()));
        self.add_type_forwarders(&mut out, module)?;
        Ok(out)
    }

    /// `MetadataModule.GetModuleAttributes()`.
    pub fn module_attributes(&mut self, module: ModuleId) -> Result<Vec<Attribute>> {
        let mut out = Vec::new();
        self.add_custom(&mut out, module, TableId::Module, 1, Target::Module)?;
        if self.comp.module(module).md.tables.rows(TableId::Assembly) == 0 {
            self.add_type_forwarders(&mut out, module)?;
        }
        Ok(out)
    }

    fn add_type_forwarders(&mut self, out: &mut Vec<Attribute>, module: ModuleId) -> Result<()> {
        let md = &self.comp.module(module).md;
        for row in 1..=md.tables.rows(TableId::ExportedType) {
            let e = md.get::<rows::ExportedType>(row)?;
            let forwarder = e.flags & 0x0020_0000 != 0
                && matches!(e.implementation.decode(), Ok((TableId::AssemblyRef, _)));
            if !forwarder {
                continue;
            }
            let target =
                self.resolve_token(module, 0x2700_0000 | row, GenericContext::default())?;
            let type_type = self.known_type(KnownTypeCode::Type);
            let arg = TypedArg {
                ty: type_type,
                value: ArgValue::Type(target),
            };
            out.push(self.builtin(TYPE_FORWARDED_TO, vec![arg], Vec::new()));
        }
        Ok(())
    }
}
