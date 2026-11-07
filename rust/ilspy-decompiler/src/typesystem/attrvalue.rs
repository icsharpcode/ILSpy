//! Custom attribute values as System.Reflection.Metadata's `CustomAttributeDecoder` decodes
//! them with the decompiler's `TypeProvider` (enum underlying types and `System.Type` from the
//! type system, serialized names through `ParseReflectionName`).

use crate::metadata::bytes::Bytes;
use crate::metadata::tables::{TableId, rows, token};
use crate::metadata::{Error, Result};

use super::compilation::{ModuleId, TypeKind};
use super::known::KnownTypeCode;
use super::types::{Nullability, TypeData, TypeId, TypeSystem};

const TYPE_HANDLE: u32 = 0x40;
const SER_TYPE: u32 = 0x50;
const SER_TAGGED_OBJECT: u32 = 0x51;
const SER_SZARRAY: u32 = 0x1D;
const SER_ENUM: u32 = 0x55;

#[derive(Clone, Debug, PartialEq)]
pub enum ArgValue {
    Null,
    Bool(bool),
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
    Type(TypeId),
    Array(Vec<TypedArg>),
}

#[derive(Clone, Debug, PartialEq)]
pub struct TypedArg {
    pub ty: TypeId,
    pub value: ArgValue,
}

#[derive(Clone, Debug, PartialEq)]
pub struct NamedArg {
    pub is_field: bool,
    pub name: Option<String>,
    pub arg: TypedArg,
}

#[derive(Clone, Debug, PartialEq)]
pub struct AttributeValue {
    pub fixed: Vec<TypedArg>,
    pub named: Vec<NamedArg>,
}

#[derive(Clone, Copy)]
struct ArgInfo {
    ty: TypeId,
    element_type: Option<TypeId>,
    code: u32,
    element_code: u32,
}

/// `ReadCompressedIntegerOrInvalid`: `None` without consuming anything.
fn try_compressed(b: &mut Bytes<'_>) -> Option<u32> {
    let mut peek = *b;
    let value = peek.compressed_u32().ok()?;
    *b = peek;
    Some(value)
}

/// `ReadSignatureTypeCode`.
fn signature_type_code(b: &mut Bytes<'_>) -> u32 {
    match try_compressed(b) {
        Some(17 | 18) => TYPE_HANDLE,
        Some(n) if n <= 255 => n,
        _ => 0,
    }
}

/// `ReadSerializationTypeCode`.
fn serialization_type_code(b: &mut Bytes<'_>) -> u32 {
    match try_compressed(b) {
        Some(n) if n <= 255 => n,
        _ => 0,
    }
}

/// `ReadSerializedString`.
fn serialized_string(b: &mut Bytes<'_>) -> Result<Option<String>> {
    if let Some(len) = try_compressed(b) {
        return Ok(Some(
            String::from_utf8_lossy(b.take(len as usize)?).into_owned(),
        ));
    }
    if b.u8()? != 0xFF {
        return Err(Error::Malformed("invalid serialized string"));
    }
    Ok(None)
}

/// `ReadTypeHandle`: a TypeDef/TypeRef/TypeSpec token, `None` for an invalid encoding.
fn type_handle(b: &mut Bytes<'_>) -> Option<u32> {
    let raw = try_compressed(b)?;
    let table = [0x02u32, 0x01, 0x1B, 0][(raw & 3) as usize];
    (table != 0).then_some(table << 24 | raw >> 2)
}

/// `CustomAttributeDecoder.SkipType`, including its treatment of CLASS / VALUETYPE (it skips
/// a type where the encoding holds a type handle).
fn skip_type(b: &mut Bytes<'_>) -> Result<()> {
    match b.compressed_u32()? {
        0x01..=0x0E | 0x16 | 0x18 | 0x19 | 0x1C => {}
        0x0F | 0x10 | 0x1D | 0x45 => skip_type(b)?,
        0x1B => {
            if b.u8()? & 0x10 != 0 {
                b.compressed_u32()?;
            }
            let count = b.compressed_u32()?;
            skip_type(b)?;
            for _ in 0..count {
                skip_type(b)?;
            }
        }
        0x14 => {
            skip_type(b)?;
            b.compressed_u32()?;
            for _ in 0..b.compressed_u32()? {
                b.compressed_u32()?;
            }
            for _ in 0..b.compressed_u32()? {
                b.compressed_i32()?;
            }
        }
        0x1F | 0x20 => {
            type_handle(b);
            skip_type(b)?;
        }
        0x15 => {
            skip_type(b)?;
            for _ in 0..b.compressed_u32()? {
                skip_type(b)?;
            }
        }
        0x13 => {
            b.compressed_u32()?;
        }
        0x11 | 0x12 => skip_type(b)?,
        _ => return Err(Error::Malformed("unexpected type code")),
    }
    Ok(())
}

/// `KnownTypeCode.ToPrimitiveTypeCode()` as a serialization type code.
fn primitive_code(code: KnownTypeCode) -> u32 {
    use KnownTypeCode::*;
    match code {
        Object => 0x1C,
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
        Void => 0x01,
        TypedReference => 0x16,
        IntPtr => 0x18,
        UIntPtr => 0x19,
        _ => 0,
    }
}

fn known_for_code(code: u32) -> KnownTypeCode {
    super::compilation::primitive_code_to_known(code as u8)
}

impl TypeSystem<'_, '_> {
    /// `TypeProvider.IsSystemType`.
    fn is_system_type(&self, t: TypeId) -> bool {
        self.definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .is_some_and(|i| i.known == KnownTypeCode::Type)
    }

    /// `TypeProvider.GetUnderlyingEnumType`.
    fn underlying_enum_code(&mut self, t: Option<TypeId>) -> Result<u32> {
        let t = t.ok_or(Error::Malformed("null enum type"))?;
        let kind = self
            .definition(t)
            .and_then(|d| self.comp.type_def(d).ok())
            .map(|i| (i.kind, i.enum_underlying));
        let underlying = match (self.data(t), kind) {
            (TypeData::Def(_) | TypeData::Inst { .. }, Some((TypeKind::Enum, code))) => {
                self.known_type(code.unwrap_or(KnownTypeCode::None))
            }
            _ => t,
        };
        let def = self.definition(underlying).ok_or(Error::Unsupported(
            "enum underlying type cannot be resolved",
        ))?;
        Ok(primitive_code(self.comp.type_def(def)?.known))
    }

    /// `TypeProvider.GetTypeFromSerializedName`.
    fn serialized_type(&mut self, name: Option<String>) -> Result<Option<TypeId>> {
        match name {
            None => Ok(None),
            Some(n) => self.parse_reflection_name(&n).map(Some),
        }
    }

    fn sz_array(&mut self, elem: TypeId) -> TypeId {
        self.intern(TypeData::Array {
            elem,
            rank: 1,
            nullability: Nullability::Oblivious,
        })
    }

    fn primitive(&mut self, code: u32) -> TypeId {
        self.known_type(known_for_code(code))
    }

    fn fixed_argument_type(
        &mut self,
        module: ModuleId,
        sig: &mut Bytes<'_>,
        generic_context: Option<Bytes<'_>>,
        is_element: bool,
    ) -> Result<ArgInfo> {
        let code = signature_type_code(sig);
        let mut info = ArgInfo {
            ty: self.primitive(0x1C),
            element_type: None,
            code,
            element_code: 0,
        };
        match code {
            0x02..=0x0E => info.ty = self.primitive(code),
            0x1C => info.code = SER_TAGGED_OBJECT,
            TYPE_HANDLE => {
                let handle =
                    type_handle(sig).ok_or(Error::Malformed("not a TypeDef or TypeRef"))?;
                let row = handle & 0x00FF_FFFF;
                info.ty = match handle >> 24 {
                    0x01 | 0x02 if row != 0 => {
                        self.resolve_token_raw(module, handle, Default::default())?
                    }
                    _ => return Err(Error::Malformed("not a TypeDef or TypeRef")),
                };
                info.code = if self.is_system_type(info.ty) {
                    SER_TYPE
                } else {
                    self.underlying_enum_code(Some(info.ty))?
                };
            }
            SER_SZARRAY => {
                if is_element {
                    return Err(Error::Malformed("nested array in attribute signature"));
                }
                let e = self.fixed_argument_type(module, sig, generic_context, true)?;
                info.element_type = Some(e.ty);
                info.element_code = e.code;
                info.ty = self.sz_array(e.ty);
            }
            0x13 => {
                let Some(mut ctx) = generic_context else {
                    return Err(Error::Malformed("type parameter without generic context"));
                };
                let index = sig.compressed_u32()?;
                let count = ctx.compressed_u32()?;
                if index >= count {
                    return Err(Error::Malformed("type parameter out of range"));
                }
                for _ in 0..index {
                    skip_type(&mut ctx)?;
                }
                return self.fixed_argument_type(module, &mut ctx, None, is_element);
            }
            _ => return Err(Error::Malformed("unexpected attribute parameter type")),
        }
        Ok(info)
    }

    fn named_argument_type(&mut self, value: &mut Bytes<'_>, is_element: bool) -> Result<ArgInfo> {
        let code = serialization_type_code(value);
        let mut info = ArgInfo {
            ty: self.primitive(0x1C),
            element_type: None,
            code,
            element_code: 0,
        };
        match code {
            0x02..=0x0E => info.ty = self.primitive(code),
            SER_TYPE => info.ty = self.known_type(KnownTypeCode::Type),
            SER_TAGGED_OBJECT => {}
            SER_SZARRAY => {
                if is_element {
                    return Err(Error::Malformed("nested array in attribute value"));
                }
                let e = self.named_argument_type(value, true)?;
                info.element_type = Some(e.ty);
                info.element_code = e.code;
                info.ty = self.sz_array(e.ty);
            }
            SER_ENUM => {
                let name = serialized_string(value)?;
                let ty = self.serialized_type(name)?;
                info.code = self.underlying_enum_code(ty)?;
                info.ty = ty.ok_or(Error::Malformed("null enum type"))?;
            }
            _ => return Err(Error::Malformed("unexpected serialization type code")),
        }
        Ok(info)
    }

    fn argument(&mut self, value: &mut Bytes<'_>, mut info: ArgInfo) -> Result<TypedArg> {
        if info.code == SER_TAGGED_OBJECT {
            info = self.named_argument_type(value, false)?;
        }
        let v = match info.code {
            0x02 => ArgValue::Bool(value.u8()? != 0),
            0x05 => ArgValue::Byte(value.u8()?),
            0x03 => ArgValue::Char(value.u16()?),
            0x0D => ArgValue::Double(value.u64()?),
            0x06 => ArgValue::Int16(value.u16()? as i16),
            0x08 => ArgValue::Int32(value.u32()? as i32),
            0x0A => ArgValue::Int64(value.u64()? as i64),
            0x04 => ArgValue::SByte(value.u8()? as i8),
            0x0C => ArgValue::Single(value.u32()?),
            0x07 => ArgValue::UInt16(value.u16()?),
            0x09 => ArgValue::UInt32(value.u32()?),
            0x0B => ArgValue::UInt64(value.u64()?),
            0x0E => match serialized_string(value)? {
                Some(s) => ArgValue::String(s.encode_utf16().collect()),
                None => ArgValue::Null,
            },
            SER_TYPE => {
                let name = serialized_string(value)?;
                match self.serialized_type(name)? {
                    Some(t) => ArgValue::Type(t),
                    None => ArgValue::Null,
                }
            }
            SER_SZARRAY => {
                let count = value.u32()? as i32;
                match count {
                    -1 => ArgValue::Null,
                    n if n < 0 => return Err(Error::Malformed("negative array length")),
                    n => {
                        let element = ArgInfo {
                            ty: info
                                .element_type
                                .expect("array argument has an element type"),
                            element_type: None,
                            code: info.element_code,
                            element_code: 0,
                        };
                        let items = (0..n)
                            .map(|_| self.argument(value, element))
                            .collect::<Result<_>>()?;
                        ArgValue::Array(items)
                    }
                }
            }
            _ => return Err(Error::Malformed("unexpected attribute argument type")),
        };
        Ok(TypedArg {
            ty: info.ty,
            value: v,
        })
    }

    /// `CustomAttribute.DecodeValue(module.TypeProvider)` for a CustomAttribute row.
    pub fn decode_attribute(&mut self, module: ModuleId, row: u32) -> Result<AttributeValue> {
        let comp = self.comp;
        let md = &comp.module(module).md;
        let ca = md.get::<rows::CustomAttribute>(row)?;
        let (signature, spec) = match ca.constructor.decode()? {
            (TableId::MethodDef, r) => (md.get::<rows::MethodDef>(r)?.signature, None),
            (TableId::MemberRef, r) => {
                let mr = md.get::<rows::MemberRef>(r)?;
                let spec = match mr.class.decode()? {
                    (TableId::TypeSpec, s) => Some(md.get::<rows::TypeSpec>(s)?.signature),
                    _ => None,
                };
                (mr.signature, spec)
            }
            _ => return Err(Error::Malformed("attribute constructor kind")),
        };
        let mut sig = Bytes::new(md.blob(signature)?);
        let mut value = Bytes::new(md.blob(ca.value)?);
        if value.u16()? != 1 {
            return Err(Error::Malformed("custom attribute prolog"));
        }
        let header = sig.u8()?;
        if crate::metadata::signature::header_kind(header)
            != crate::metadata::signature::HEADER_KIND_METHOD
            || header & 0x10 != 0
        {
            return Err(Error::Malformed("attribute constructor signature"));
        }
        let count = sig.compressed_u32()?;
        if signature_type_code(&mut sig) != 0x01 {
            return Err(Error::Malformed("attribute constructor must return void"));
        }
        let generic_context = match spec {
            None => None,
            Some(blob) => {
                let mut ctx = Bytes::new(md.blob(blob)?);
                if signature_type_code(&mut ctx) == 0x15 {
                    let kind = ctx.compressed_u32()?;
                    if kind != 0x11 && kind != 0x12 {
                        return Err(Error::Malformed("generic instance kind"));
                    }
                    type_handle(&mut ctx);
                    Some(ctx)
                } else {
                    None
                }
            }
        };
        let mut fixed = Vec::with_capacity(count as usize);
        for _ in 0..count {
            let info = self.fixed_argument_type(module, &mut sig, generic_context, false)?;
            fixed.push(self.argument(&mut value, info)?);
        }
        let named_count = value.u16()?;
        let mut named = Vec::with_capacity(usize::from(named_count));
        for _ in 0..named_count {
            let is_field = match serialization_type_code(&mut value) {
                0x53 => true,
                0x54 => false,
                _ => return Err(Error::Malformed("named argument kind")),
            };
            let info = self.named_argument_type(&mut value, false)?;
            let name = serialized_string(&mut value)?;
            let arg = self.argument(&mut value, info)?;
            named.push(NamedArg {
                is_field,
                name,
                arg,
            });
        }
        Ok(AttributeValue { fixed, named })
    }
}

/// The token of a CustomAttribute row, for listings.
pub fn attribute_token(row: u32) -> u32 {
    token(TableId::CustomAttribute, row)
}
