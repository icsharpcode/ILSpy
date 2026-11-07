//! Members of type definitions as the managed `MetadataMethod`, `MetadataField`,
//! `MetadataProperty`, `MetadataEvent`, `MetadataParameter` and `MetadataTypeParameter` see
//! them under `TypeSystemOptions.None`.

use crate::metadata::bytes::Bytes;
use crate::metadata::custom_attribute::{self, ModuleResolver, Value};
use crate::metadata::signature::{self, SigType};
use crate::metadata::tables::{TableId, rows};
use crate::metadata::{Error, Metadata, Result};

use super::compilation::{ModuleId, TypeDefId, TypeKind, is_known_type_name};
use super::known::KnownTypeCode;
use super::types::{GenericContext, TypeData, TypeId, TypeParamOwner, TypeSystem};

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
pub struct MethodDefId {
    pub module: ModuleId,
    pub row: u32,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum SymbolKind {
    Method,
    Constructor,
    Destructor,
    Operator,
    Accessor,
}

impl SymbolKind {
    pub fn name(self) -> &'static str {
        match self {
            SymbolKind::Method => "Method",
            SymbolKind::Constructor => "Constructor",
            SymbolKind::Destructor => "Destructor",
            SymbolKind::Operator => "Operator",
            SymbolKind::Accessor => "Accessor",
        }
    }
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum RefKind {
    None,
    Out,
    Ref,
}

impl RefKind {
    pub fn name(self) -> &'static str {
        match self {
            RefKind::None => "None",
            RefKind::Out => "Out",
            RefKind::Ref => "Ref",
        }
    }
}

/// A constant as `BlobReader.ReadConstant` (or the decimal constant attribute) yields it.
#[derive(Clone, Debug, PartialEq)]
pub enum ConstValue {
    Null,
    Boolean(bool),
    Char(u16),
    SByte(i8),
    Byte(u8),
    Int16(i16),
    UInt16(u16),
    Int32(i32),
    UInt32(u32),
    Int64(i64),
    UInt64(u64),
    Single(u32),
    Double(u64),
    String(Vec<u16>),
    /// `decimal.GetBits`: lo, mid, hi, flags.
    Decimal([u32; 4]),
}

#[derive(Clone, Debug)]
pub struct ParamInfo {
    pub ty: TypeId,
    pub name: String,
    pub ref_kind: RefKind,
    pub optional: bool,
    pub has_constant_in_signature: bool,
    pub value: ConstValue,
}

#[derive(Clone, Debug)]
pub struct TypeParamInfo {
    pub name: String,
    pub variance: &'static str,
    pub default_ctor: bool,
    pub reference_type: bool,
    pub value_type: bool,
    pub allows_ref_like: bool,
    pub constraints: Vec<TypeId>,
}

const METHOD_STATIC: u16 = 0x0010;
const METHOD_FINAL: u16 = 0x0020;
const METHOD_VIRTUAL: u16 = 0x0040;
const METHOD_HIDE_BY_SIG: u16 = 0x0080;
const METHOD_NEW_SLOT: u16 = 0x0100;
const METHOD_ABSTRACT: u16 = 0x0400;
const METHOD_SPECIAL_NAME: u16 = 0x0800;
const METHOD_PINVOKE: u16 = 0x2000;
const METHOD_RT_SPECIAL_NAME: u16 = 0x1000;
const METHOD_FAMILY: u16 = 0x0004;
const PARAM_IN: u16 = 0x0001;
const PARAM_OUT: u16 = 0x0002;
const PARAM_OPTIONAL: u16 = 0x0010;

const OPERATOR_NAMES: &[&str] = &[
    "op_LogicalNot",
    "op_OnesComplement",
    "op_Increment",
    "op_CheckedIncrement",
    "op_Decrement",
    "op_CheckedDecrement",
    "op_True",
    "op_False",
    "op_UnaryPlus",
    "op_UnaryNegation",
    "op_CheckedUnaryNegation",
    "op_Addition",
    "op_CheckedAddition",
    "op_Subtraction",
    "op_CheckedSubtraction",
    "op_Multiply",
    "op_CheckedMultiply",
    "op_Division",
    "op_CheckedDivision",
    "op_Modulus",
    "op_BitwiseAnd",
    "op_BitwiseOr",
    "op_ExclusiveOr",
    "op_LeftShift",
    "op_RightShift",
    "op_UnsignedRightShift",
    "op_Equality",
    "op_Inequality",
    "op_GreaterThan",
    "op_LessThan",
    "op_GreaterThanOrEqual",
    "op_LessThanOrEqual",
    "op_Implicit",
    "op_Explicit",
    "op_CheckedExplicit",
];

/// `Accessibility` for MethodAttributes' member access mask.
pub fn method_accessibility(flags: u16) -> &'static str {
    match flags & 7 {
        6 => "Public",
        3 => "Internal",
        1 => "Private",
        4 => "Protected",
        2 => "ProtectedAndInternal",
        5 => "ProtectedOrInternal",
        _ => "None",
    }
}

/// `Accessibility` for FieldAttributes' access mask.
pub fn field_accessibility(flags: u16) -> &'static str {
    match flags & 7 {
        6 => "Public",
        2 => "ProtectedAndInternal",
        3 => "Internal",
        4 => "Protected",
        5 => "ProtectedOrInternal",
        _ => "Private",
    }
}

/// `MethodDefinition.HasBody()`.
pub fn has_body(m: &rows::MethodDef) -> bool {
    m.flags & (METHOD_ABSTRACT | METHOD_PINVOKE) == 0 && m.impl_flags & 0x1007 == 0 && m.rva > 0
}

/// `IsStatic`, `IsAbstract`, `IsSealed`, `IsVirtual`, `IsOverride` of a method.
pub fn method_modifiers(flags: u16) -> [bool; 5] {
    let is_static = flags & METHOD_STATIC != 0;
    let is_virtual = if is_static {
        flags & (METHOD_ABSTRACT | METHOD_VIRTUAL) == METHOD_VIRTUAL
    } else {
        flags & (METHOD_ABSTRACT | METHOD_VIRTUAL | METHOD_NEW_SLOT | METHOD_FINAL)
            == METHOD_VIRTUAL | METHOD_NEW_SLOT
    };
    [
        is_static,
        flags & METHOD_ABSTRACT != 0,
        flags & (METHOD_ABSTRACT | METHOD_FINAL | METHOD_NEW_SLOT | METHOD_STATIC) == METHOD_FINAL,
        is_virtual,
        flags & (METHOD_NEW_SLOT | METHOD_VIRTUAL | METHOD_STATIC) == METHOD_VIRTUAL,
    ]
}

/// `BlobReader.ReadConstant` of a Constant row.
pub fn read_constant(md: &Metadata<'_>, row: u32) -> Result<ConstValue> {
    let c = md.get::<rows::Constant>(row)?;
    let mut b = Bytes::new(md.blob(c.value)?);
    Ok(match c.kind & 0xFF {
        0x02 => ConstValue::Boolean(b.u8()? != 0),
        0x03 => ConstValue::Char(b.u16()?),
        0x04 => ConstValue::SByte(b.u8()? as i8),
        0x05 => ConstValue::Byte(b.u8()?),
        0x06 => ConstValue::Int16(b.u16()? as i16),
        0x07 => ConstValue::UInt16(b.u16()?),
        0x08 => ConstValue::Int32(b.u32()? as i32),
        0x09 => ConstValue::UInt32(b.u32()?),
        0x0A => ConstValue::Int64(b.u64()? as i64),
        0x0B => ConstValue::UInt64(b.u64()?),
        0x0C => ConstValue::Single(b.u32()?),
        0x0D => ConstValue::Double(b.u64()?),
        0x0E => {
            let bytes = b.take(b.remaining())?;
            ConstValue::String(
                bytes
                    .as_chunks::<2>()
                    .0
                    .iter()
                    .map(|p| u16::from_le_bytes(*p))
                    .collect(),
            )
        }
        0x12 => {
            if b.u32()? != 0 {
                return Err(Error::Malformed("invalid null constant"));
            }
            ConstValue::Null
        }
        _ => return Err(Error::Malformed("constant with invalid type code")),
    })
}

fn has_known_attribute(md: &Metadata<'_>, table: TableId, row: u32, ns: &str, name: &str) -> bool {
    md.custom_attributes(table, row).into_iter().any(|ca| {
        md.attribute_type(ca)
            .is_ok_and(|t| is_known_type_name(md, t, ns, name, 0))
    })
}

const DECIMAL_CONSTANT: (&str, &str) = (
    "System.Runtime.CompilerServices",
    "DecimalConstantAttribute",
);

/// `DecimalConstantHelper.GetDecimalConstantValue`: `Null` when the attribute does not
/// decode to five suitable arguments.
fn decimal_constant(md: &Metadata<'_>, table: TableId, row: u32) -> Result<ConstValue> {
    for ca in md.custom_attributes(table, row) {
        if !md
            .attribute_type(ca)
            .is_ok_and(|t| is_known_type_name(md, t, DECIMAL_CONSTANT.0, DECIMAL_CONSTANT.1, 0))
        {
            continue;
        }
        let attribute = md.get::<rows::CustomAttribute>(ca)?;
        let value = custom_attribute::decode(md, &attribute, &ModuleResolver::new(md))?;
        let [Value::U8(scale), Value::U8(sign), hi, mid, lo] = value.fixed.as_slice() else {
            return Ok(ConstValue::Null);
        };
        let words = match (hi, mid, lo) {
            (Value::U32(h), Value::U32(m), Value::U32(l)) => [*l, *m, *h],
            (Value::I32(h), Value::I32(m), Value::I32(l)) => [*l as u32, *m as u32, *h as u32],
            _ => return Ok(ConstValue::Null),
        };
        if *scale > 28 {
            return Err(Error::Malformed("decimal scale out of range"));
        }
        let flags = u32::from(*scale) << 16 | if *sign != 0 { 0x8000_0000 } else { 0 };
        return Ok(ConstValue::Decimal([words[0], words[1], words[2], flags]));
    }
    Ok(ConstValue::Null)
}

impl<'c, 'a> TypeSystem<'c, 'a> {
    /// The owner of class type parameter `index` of `def`: nested types share the outer
    /// type's parameters for the leading indices.
    pub fn class_param_owner(&self, def: TypeDefId, index: u32) -> TypeParamOwner {
        if let Ok(info) = self.comp.type_def(def)
            && info.declaring != 0
        {
            let outer = TypeDefId {
                module: def.module,
                row: info.declaring,
            };
            if self
                .comp
                .type_def(outer)
                .is_ok_and(|o| index < o.type_parameter_count)
            {
                return self.class_param_owner(outer, index);
            }
        }
        TypeParamOwner::Type(def)
    }

    pub fn type_context(&self, def: Option<TypeDefId>) -> GenericContext {
        GenericContext {
            class: def.map(|d| {
                (
                    d,
                    self.comp.type_def(d).map_or(0, |i| i.type_parameter_count),
                )
            }),
            method: None,
        }
    }

    fn method_declaring(&self, m: MethodDefId) -> Result<Option<TypeDefId>> {
        let row = self.comp.module(m.module).md.method_declaring_type(m.row)?;
        Ok((row != 0).then_some(TypeDefId {
            module: m.module,
            row,
        }))
    }

    pub fn method_context(&self, m: MethodDefId) -> Result<GenericContext> {
        let md = &self.comp.module(m.module).md;
        let mut ctx = self.type_context(self.method_declaring(m)?);
        ctx.method = Some((m, md.generic_params(TableId::MethodDef, m.row).len() as u32));
        Ok(ctx)
    }

    /// The type kind as `IType.Kind` reports it, for the interface test on constraints.
    pub fn is_interface(&self, t: TypeId) -> bool {
        self.definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .is_some_and(|i| i.kind == TypeKind::Interface)
            && matches!(self.data(t), TypeData::Def(_) | TypeData::Inst { .. })
    }

    /// Type parameters of a TypeDef (`owner_table` TypeDef) or MethodDef.
    pub fn type_parameters(
        &mut self,
        module: ModuleId,
        owner_table: TableId,
        owner_row: u32,
    ) -> Result<Vec<TypeParamInfo>> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let rows: Vec<u32> = md.generic_params(owner_table, owner_row).collect();
        let mut result = Vec::with_capacity(rows.len());
        for (index, gp_row) in rows.iter().enumerate() {
            // Leading parameters of a nested type are the enclosing type's.
            let (owner, source_row) = if owner_table == TableId::TypeDef {
                let def = TypeDefId {
                    module,
                    row: owner_row,
                };
                match self.class_param_owner(def, index as u32) {
                    TypeParamOwner::Type(o) if o != def => {
                        let outer_rows: Vec<u32> =
                            md.generic_params(TableId::TypeDef, o.row).collect();
                        (TypeParamOwner::Type(o), outer_rows[index])
                    }
                    owner => (owner, *gp_row),
                }
            } else {
                (
                    TypeParamOwner::Method(MethodDefId {
                        module,
                        row: owner_row,
                    }),
                    *gp_row,
                )
            };
            let gp = md.get::<rows::GenericParam>(source_row)?;
            let ctx = match owner {
                TypeParamOwner::Type(def) => self.type_context(Some(def)),
                TypeParamOwner::Method(m) => self.method_context(m)?,
                _ => GenericContext::default(),
            };
            let mut constraints = Vec::new();
            let mut has_non_interface = false;
            for c in md.generic_param_constraints(source_row) {
                let token = md
                    .get::<rows::GenericParamConstraint>(c)?
                    .constraint
                    .token()?;
                let t = self.resolve_token(module, token, ctx)?;
                has_non_interface |= !self.is_interface(t);
                constraints.push(t);
            }
            let value_type = gp.flags & 0x0008 != 0;
            if value_type {
                constraints.push(self.known_type(KnownTypeCode::ValueType));
            } else if !has_non_interface {
                constraints.push(self.known_type(KnownTypeCode::Object));
            }
            result.push(TypeParamInfo {
                name: md.string(gp.name)?.into_owned(),
                variance: match gp.flags & 3 {
                    2 => "Contravariant",
                    1 => "Covariant",
                    _ => "Invariant",
                },
                default_ctor: gp.flags & 0x0010 != 0,
                reference_type: gp.flags & 0x0004 != 0,
                value_type,
                allows_ref_like: gp.flags & 0x0020 != 0,
                constraints,
            });
        }
        Ok(result)
    }

    /// `MetadataMethod.DecodeSignature(...)` shared by methods and properties: parameters
    /// paired with their Param rows (gaps get unnamed parameters), plus `__arglist` for
    /// varargs.
    fn decode_parameters(
        &mut self,
        module: ModuleId,
        sig: &signature::MethodSig,
        ctx: GenericContext,
        param_rows: Option<Vec<u32>>,
    ) -> Result<(TypeId, Vec<ParamInfo>)> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let ret = self.from_sig(module, &sig.ret, ctx)?;
        let types = sig
            .params
            .iter()
            .map(|p| self.from_sig(module, p, ctx))
            .collect::<Result<Vec<_>>>()?;
        let required = sig.required_params;
        let unnamed = |ts: &Self, ty: TypeId| ParamInfo {
            ty,
            name: String::new(),
            ref_kind: if matches!(ts.data(ty), TypeData::ByRef(_)) {
                RefKind::Ref
            } else {
                RefKind::None
            },
            optional: false,
            has_constant_in_signature: false,
            value: ConstValue::Null,
        };
        let mut params = Vec::with_capacity(required + 1);
        for row in param_rows.unwrap_or_default() {
            let p = md.get::<rows::Param>(row)?;
            let seq = usize::from(p.sequence);
            if seq == 0 || !(params.len() < seq && seq <= required) {
                continue;
            }
            while params.len() < seq - 1 {
                let ty = types[params.len()];
                params.push(unnamed(self, ty));
            }
            let ty = types[params.len()];
            params.push(self.parameter(md, row, &p, ty)?);
        }
        while params.len() < required {
            let ty = types[params.len()];
            params.push(unnamed(self, ty));
        }
        if sig.header & 0x0F == 0x05 {
            let arglist = self.intern(TypeData::ArgList);
            params.push(ParamInfo {
                ty: arglist,
                ..unnamed(self, arglist)
            });
        }
        Ok((ret, params))
    }

    fn parameter(
        &self,
        md: &Metadata<'_>,
        row: u32,
        p: &rows::Param,
        ty: TypeId,
    ) -> Result<ParamInfo> {
        let ref_kind = if !matches!(self.data(ty), TypeData::ByRef(_)) {
            RefKind::None
        } else if p.flags & (PARAM_IN | PARAM_OUT) == PARAM_OUT {
            RefKind::Out
        } else {
            RefKind::Ref
        };
        let optional = p.flags & PARAM_OPTIONAL != 0;
        let is_decimal = optional
            && has_known_attribute(
                md,
                TableId::Param,
                row,
                DECIMAL_CONSTANT.0,
                DECIMAL_CONSTANT.1,
            );
        let constant = md.default_value(TableId::Param, row);
        let value = if is_decimal {
            decimal_constant(md, TableId::Param, row).unwrap_or(ConstValue::Null)
        } else if constant == 0 {
            ConstValue::Null
        } else {
            read_constant(md, constant).unwrap_or(ConstValue::Null)
        };
        Ok(ParamInfo {
            ty,
            name: md.string(p.name)?.into_owned(),
            ref_kind,
            optional,
            has_constant_in_signature: !is_decimal && constant != 0,
            value,
        })
    }

    /// Return type and parameters of a method; `?` and none when the signature is bad.
    pub fn method_signature(&mut self, m: MethodDefId) -> (TypeId, Vec<ParamInfo>) {
        let result = (|| {
            let md = &self.comp.module(m.module).md;
            let def = md.get::<rows::MethodDef>(m.row)?;
            let sig = signature::decode_method(&mut Bytes::new(md.blob(def.signature)?))?;
            let ctx = self.method_context(m)?;
            let rows = md.method_params(m.row)?;
            self.decode_parameters(m.module, &sig, ctx, Some(rows))
        })();
        result.unwrap_or_else(|_| (self.intern(TypeData::UnknownSpecial), Vec::new()))
    }

    /// `MethodSemanticsLookup.GetSemantics`: the property or event a method is a C# accessor
    /// of (getter, setter, adder, remover; not raisers).
    pub fn accessor_owner(&mut self, m: MethodDefId) -> Option<(u32, u16)> {
        let comp = self.comp;
        let map = self.accessor_owners.entry(m.module).or_insert_with(|| {
            let md = &comp.module(m.module).md;
            let mut map = std::collections::HashMap::new();
            let token = crate::metadata::tables::token;
            for p in 1..=md.tables.rows(TableId::Property) {
                let (get, set, _) = md.property_accessors(p);
                for (method, semantics) in [(get, 0x02), (set, 0x01)] {
                    if method != 0 {
                        map.entry(method)
                            .or_insert((token(TableId::Property, p), semantics));
                    }
                }
            }
            for e in 1..=md.tables.rows(TableId::Event) {
                let (add, remove, _, _) = md.event_accessors(e);
                for (method, semantics) in [(add, 0x08), (remove, 0x10)] {
                    if method != 0 {
                        map.entry(method)
                            .or_insert((token(TableId::Event, e), semantics));
                    }
                }
            }
            map
        });
        map.get(&m.row).copied()
    }

    pub fn method_symbol_kind(
        &mut self,
        m: MethodDefId,
        accessor: Option<(u32, u16)>,
    ) -> Result<SymbolKind> {
        let md = &self.comp.module(m.module).md;
        let def = md.get::<rows::MethodDef>(m.row)?;
        let generic = !md.generic_params(TableId::MethodDef, m.row).is_empty();
        let name = md.string(def.name)?;
        const FINALIZER: u16 = METHOD_VIRTUAL | METHOD_FAMILY | METHOD_HIDE_BY_SIG;
        Ok(if accessor.is_some() {
            SymbolKind::Accessor
        } else if def.flags & (METHOD_SPECIAL_NAME | METHOD_RT_SPECIAL_NAME) != 0 && !generic {
            if name == ".cctor" || name == ".ctor" {
                SymbolKind::Constructor
            } else if OPERATOR_NAMES.contains(&name.as_ref()) {
                SymbolKind::Operator
            } else {
                SymbolKind::Method
            }
        } else if def.flags & FINALIZER == FINALIZER && !generic {
            let (ret, params) = self.method_signature(m);
            let declaring_is_class = self
                .method_declaring(m)?
                .and_then(|d| self.comp.type_def(d).ok())
                .is_some_and(|i| i.kind == TypeKind::Class);
            if name == "Finalize"
                && params.is_empty()
                && self.definition(ret).is_some_and(|d| {
                    self.comp
                        .type_def(d)
                        .is_ok_and(|i| i.known == KnownTypeCode::Void)
                })
                && declaring_is_class
            {
                SymbolKind::Destructor
            } else {
                SymbolKind::Method
            }
        } else if def.flags & METHOD_STATIC != 0 && !generic {
            match name.rfind('.') {
                Some(i) if i > 0 && OPERATOR_NAMES.contains(&&name[i + 1..]) => {
                    SymbolKind::Operator
                }
                _ => SymbolKind::Method,
            }
        } else {
            SymbolKind::Method
        })
    }

    /// A field's type (`?` when the signature is bad) and whether its outermost modifier is
    /// `modreq(IsVolatile)`.
    pub fn field_type(&mut self, module: ModuleId, row: u32) -> (TypeId, bool) {
        let result = (|| {
            let md = &self.comp.module(module).md;
            let f = md.get::<rows::Field>(row)?;
            let sig = signature::decode_field(&mut Bytes::new(md.blob(f.signature)?))?;
            let declaring = md.field_declaring_type(row)?;
            let ctx = self.type_context((declaring != 0).then_some(TypeDefId {
                module,
                row: declaring,
            }));
            let ty = self.from_sig(module, &sig, ctx)?;
            let volatile = match &sig {
                SigType::Modified { modifier, .. } => {
                    let m = self.from_sig(module, modifier, ctx)?;
                    self.name_and_namespace(m).is_some_and(|(n, ns)| {
                        n == "IsVolatile" && ns == "System.Runtime.CompilerServices"
                    })
                }
                _ => false,
            };
            Ok::<_, Error>((ty, volatile))
        })();
        result.unwrap_or_else(|_| (self.intern(TypeData::UnknownSpecial), false))
    }

    /// The field's constant (`GetConstantValue(throwOnInvalidMetadata: false)`).
    pub fn field_constant(&self, module: ModuleId, row: u32) -> ConstValue {
        let md = &self.comp.module(module).md;
        match md.default_value(TableId::Field, row) {
            0 => ConstValue::Null,
            c => read_constant(md, c).unwrap_or(ConstValue::Null),
        }
    }

    /// A property's type and parameters, decoded with the parameter rows of its getter (or
    /// setter). `Err` when it has no accessor, where the managed code fails outright.
    pub fn property_signature(
        &mut self,
        module: ModuleId,
        row: u32,
    ) -> Result<(TypeId, Vec<ParamInfo>)> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let (get, set, _) = md.property_accessors(row);
        let any = if get != 0 { get } else { set };
        if any == 0 {
            return Err(Error::Unsupported("property without accessors"));
        }
        let declaring = md.method_declaring_type(any)?;
        if declaring == 0 {
            return Err(Error::Unsupported(
                "property accessor without declaring type",
            ));
        }
        let ctx = self.type_context(Some(TypeDefId {
            module,
            row: declaring,
        }));
        let result = (|| {
            let p = md.get::<rows::Property>(row)?;
            let sig = signature::decode_method(&mut Bytes::new(md.blob(p.signature)?))?;
            let rows = md.method_params(any)?;
            self.decode_parameters(module, &sig, ctx, Some(rows))
        })();
        Ok(result.unwrap_or_else(|_| (self.intern(TypeData::UnknownSpecial), Vec::new())))
    }

    /// An event's type (`MetadataModule.ResolveType` of its EventType).
    pub fn event_type(&mut self, module: ModuleId, row: u32) -> Result<TypeId> {
        let md = &self.comp.module(module).md;
        let (add, remove, raise, _) = md.event_accessors(row);
        let any = [add, remove, raise]
            .into_iter()
            .find(|&r| r != 0)
            .unwrap_or(0);
        let declaring = if any == 0 {
            0
        } else {
            md.method_declaring_type(any)?
        };
        let ctx = self.type_context((declaring != 0).then_some(TypeDefId {
            module,
            row: declaring,
        }));
        let token = md.get::<rows::Event>(row)?.event_type.token()?;
        self.resolve_token(module, token, ctx)
    }
}
