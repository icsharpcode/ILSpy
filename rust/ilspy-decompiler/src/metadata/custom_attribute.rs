//! Custom attribute value blobs (ECMA-335 II.23.3).
//!
//! Decoding needs the underlying type of enum arguments, which may live in another assembly,
//! so the caller supplies a [`TypeResolver`]. [`ModuleResolver`] answers from the module alone.

use std::borrow::Cow;

use super::bytes::Bytes;
use super::signature::{self, MethodSig, Primitive, SigType};
use super::tables::{Coded, TableId, rows};
use super::{Error, Metadata, Result};

/// Questions the decoder cannot answer from the blob alone.
pub trait TypeResolver {
    /// Whether a TypeDef/TypeRef token denotes `System.Type`.
    fn is_system_type(&self, token: u32) -> bool;
    /// The underlying type of the enum a TypeDef/TypeRef token denotes.
    fn enum_underlying_type(&self, token: u32) -> Option<Primitive>;
    /// The underlying type of the enum named by a serialized (reflection) type name.
    fn enum_underlying_type_by_name(&self, name: &str) -> Option<Primitive>;
}

#[derive(Clone, Debug, PartialEq)]
pub enum ElemType {
    Primitive(Primitive),
    Type,
    /// `object`: the value carries its own type tag.
    Tagged,
    Enum(Primitive),
    SzArray(Box<ElemType>),
}

#[derive(Clone, Debug, PartialEq)]
pub enum Value {
    Bool(bool),
    Char(u16),
    I8(i8),
    U8(u8),
    I16(i16),
    U16(u16),
    I32(i32),
    U32(u32),
    I64(i64),
    U64(u64),
    F32(f32),
    F64(f64),
    /// Strings and `System.Type` names; `None` is the null reference.
    String(Option<String>),
    Type(Option<String>),
    Boxed(ElemType, Box<Value>),
    Array(Option<Vec<Value>>),
}

#[derive(Clone, Debug, PartialEq)]
pub struct NamedArgument {
    pub is_field: bool,
    pub ty: ElemType,
    pub name: Option<String>,
    pub value: Value,
}

#[derive(Clone, Debug, PartialEq)]
pub struct CustomAttributeValue {
    pub fixed: Vec<Value>,
    pub named: Vec<NamedArgument>,
}

const UNRESOLVED_ENUM: Error = Error::Unsupported("enum type not resolvable from this module");

/// Decodes the value of a CustomAttribute row.
pub fn decode(
    md: &Metadata<'_>,
    attribute: &rows::CustomAttribute,
    resolver: &dyn TypeResolver,
) -> Result<CustomAttributeValue> {
    let (ctor_sig, generic_args) = constructor(md, attribute.constructor)?;
    if ctor_sig.header & 0x10 != 0 || ctor_sig.ret != SigType::Primitive(Primitive::Void) {
        return Err(Error::Malformed("attribute constructor signature"));
    }
    let mut b = Bytes::new(md.blob(attribute.value)?);
    if b.u16()? != 1 {
        return Err(Error::Malformed("custom attribute prolog"));
    }
    let fixed = ctor_sig
        .params
        .iter()
        .map(|p| {
            let ty = elem_type(p, &generic_args, resolver, false)?;
            read_value(&mut b, &ty, resolver)
        })
        .collect::<Result<_>>()?;
    let count = b.u16()?;
    let named = (0..count)
        .map(|_| {
            let is_field = match b.u8()? {
                0x53 => true,
                0x54 => false,
                _ => return Err(Error::Malformed("named argument kind")),
            };
            let ty = serialized_type(&mut b, resolver, false)?;
            let name = ser_string(&mut b)?;
            let value = read_value(&mut b, &ty, resolver)?;
            Ok(NamedArgument {
                is_field,
                ty,
                name,
                value,
            })
        })
        .collect::<Result<_>>()?;
    Ok(CustomAttributeValue { fixed, named })
}

/// The constructor signature, plus the type arguments of its declaring generic instance.
fn constructor(md: &Metadata<'_>, ctor: Coded) -> Result<(MethodSig, Vec<SigType>)> {
    let (table, row) = ctor.decode()?;
    let (blob, parent) = match table {
        TableId::MethodDef => (md.get::<rows::MethodDef>(row)?.signature, None),
        TableId::MemberRef => {
            let m = md.get::<rows::MemberRef>(row)?;
            (m.signature, Some(m.class))
        }
        _ => return Err(Error::Malformed("attribute constructor kind")),
    };
    let sig = signature::decode_method(&mut Bytes::new(md.blob(blob)?))?;
    let mut generic_args = Vec::new();
    if let Some(parent) = parent
        && let (TableId::TypeSpec, spec) = parent.decode()?
    {
        let spec = md.get::<rows::TypeSpec>(spec)?;
        if let SigType::GenericInst { args, .. } =
            signature::decode_type(&mut Bytes::new(md.blob(spec.signature)?))?
        {
            generic_args = args;
        }
    }
    Ok((sig, generic_args))
}

fn elem_type(
    ty: &SigType,
    generic_args: &[SigType],
    resolver: &dyn TypeResolver,
    is_element: bool,
) -> Result<ElemType> {
    Ok(match ty {
        SigType::Primitive(Primitive::Object) => ElemType::Tagged,
        SigType::Primitive(p) if serializable_primitive(*p) => ElemType::Primitive(*p),
        SigType::Modified { inner, .. } => elem_type(inner, generic_args, resolver, is_element)?,
        SigType::Handle { token, .. } if resolver.is_system_type(*token) => ElemType::Type,
        SigType::Handle { token, .. } => ElemType::Enum(
            resolver
                .enum_underlying_type(*token)
                .ok_or(UNRESOLVED_ENUM)?,
        ),
        SigType::SzArray(elem) if !is_element => {
            ElemType::SzArray(Box::new(elem_type(elem, generic_args, resolver, true)?))
        }
        SigType::TypeParam(i) => {
            let arg = generic_args
                .get(*i as usize)
                .ok_or(Error::Malformed("attribute type parameter out of range"))?;
            elem_type(arg, &[], resolver, is_element)?
        }
        _ => return Err(Error::Malformed("type not allowed in a custom attribute")),
    })
}

fn serializable_primitive(p: Primitive) -> bool {
    !matches!(
        p,
        Primitive::Void | Primitive::TypedReference | Primitive::IntPtr | Primitive::UIntPtr
    )
}

/// A `FieldOrPropType` (II.23.3).
fn serialized_type(
    b: &mut Bytes<'_>,
    resolver: &dyn TypeResolver,
    is_element: bool,
) -> Result<ElemType> {
    let code = b.u8()?;
    Ok(match code {
        0x02..=0x0E => {
            let blob = [code];
            match signature::decode_type(&mut Bytes::new(&blob))? {
                SigType::Primitive(p) => ElemType::Primitive(p),
                _ => unreachable!("codes 0x02-0x0E are primitives"),
            }
        }
        0x50 => ElemType::Type,
        0x51 => ElemType::Tagged,
        0x55 => {
            let name = ser_string(b)?.ok_or(Error::Malformed("null enum type name"))?;
            ElemType::Enum(
                resolver
                    .enum_underlying_type_by_name(&name)
                    .ok_or(UNRESOLVED_ENUM)?,
            )
        }
        0x1D if !is_element => ElemType::SzArray(Box::new(serialized_type(b, resolver, true)?)),
        _ => return Err(Error::Malformed("serialization type code")),
    })
}

fn ser_string(b: &mut Bytes<'_>) -> Result<Option<String>> {
    let mut peek = *b;
    if peek.u8()? == 0xFF {
        *b = peek;
        return Ok(None);
    }
    let len = b.compressed_u32()? as usize;
    Ok(Some(String::from_utf8_lossy(b.take(len)?).into_owned()))
}

fn read_value(b: &mut Bytes<'_>, ty: &ElemType, resolver: &dyn TypeResolver) -> Result<Value> {
    Ok(match ty {
        ElemType::Primitive(p) | ElemType::Enum(p) => read_primitive(b, *p)?,
        ElemType::Type => Value::Type(ser_string(b)?),
        ElemType::Tagged => {
            let inner = serialized_type(b, resolver, false)?;
            let value = read_value(b, &inner, resolver)?;
            Value::Boxed(inner, Box::new(value))
        }
        ElemType::SzArray(elem) => match b.u32()? {
            u32::MAX => Value::Array(None),
            n => Value::Array(Some(
                (0..n)
                    .map(|_| read_value(b, elem, resolver))
                    .collect::<Result<_>>()?,
            )),
        },
    })
}

fn read_primitive(b: &mut Bytes<'_>, p: Primitive) -> Result<Value> {
    use Primitive::*;
    Ok(match p {
        Boolean => Value::Bool(b.u8()? != 0),
        Char => Value::Char(b.u16()?),
        SByte => Value::I8(b.u8()? as i8),
        Byte => Value::U8(b.u8()?),
        Int16 => Value::I16(b.u16()? as i16),
        UInt16 => Value::U16(b.u16()?),
        Int32 => Value::I32(b.u32()? as i32),
        UInt32 => Value::U32(b.u32()?),
        Int64 => Value::I64(b.u64()? as i64),
        UInt64 => Value::U64(b.u64()?),
        Single => Value::F32(f32::from_bits(b.u32()?)),
        Double => Value::F64(f64::from_bits(b.u64()?)),
        String => Value::String(ser_string(b)?),
        _ => {
            return Err(Error::Malformed(
                "primitive not allowed in a custom attribute",
            ));
        }
    })
}

/// Resolves enums and `System.Type` from the module's own TypeDefs and TypeRef names only.
pub struct ModuleResolver<'m, 'a> {
    md: &'m Metadata<'a>,
}

impl<'m, 'a> ModuleResolver<'m, 'a> {
    pub fn new(md: &'m Metadata<'a>) -> Self {
        Self { md }
    }

    fn type_name(&self, token: u32) -> Option<(Cow<'a, str>, Cow<'a, str>)> {
        let row = token & 0x00FF_FFFF;
        match TableId::from_index((token >> 24) as usize)? {
            TableId::TypeDef => {
                let t = self.md.get::<rows::TypeDef>(row).ok()?;
                Some((
                    self.md.string(t.namespace).ok()?,
                    self.md.string(t.name).ok()?,
                ))
            }
            TableId::TypeRef => {
                let t = self.md.get::<rows::TypeRef>(row).ok()?;
                Some((
                    self.md.string(t.namespace).ok()?,
                    self.md.string(t.name).ok()?,
                ))
            }
            _ => None,
        }
    }

    /// The underlying type of a TypeDef enum: the type of its first instance field.
    fn local_enum(&self, type_def: u32) -> Option<Primitive> {
        let t = self.md.get::<rows::TypeDef>(type_def).ok()?;
        let base = self.type_name(t.extends.token().ok()?)?;
        if base.0 != "System" || base.1 != "Enum" {
            return None;
        }
        self.md
            .type_fields(type_def)
            .ok()?
            .into_iter()
            .find_map(|f| {
                let field = self.md.get::<rows::Field>(f).ok()?;
                if field.flags & 0x0010 != 0 {
                    return None; // static
                }
                match signature::decode_field(&mut Bytes::new(self.md.blob(field.signature).ok()?))
                {
                    Ok(SigType::Primitive(p)) => Some(p),
                    _ => None,
                }
            })
    }

    fn full_name(&self, type_def: u32) -> Option<String> {
        let t = self.md.get::<rows::TypeDef>(type_def).ok()?;
        let (ns, name) = (
            self.md.string(t.namespace).ok()?,
            self.md.string(t.name).ok()?,
        );
        let simple = if ns.is_empty() {
            name.into_owned()
        } else {
            format!("{ns}.{name}")
        };
        match self.md.declaring_type(type_def).ok()? {
            0 => Some(simple),
            outer => Some(format!("{}+{simple}", self.full_name(outer)?)),
        }
    }
}

impl TypeResolver for ModuleResolver<'_, '_> {
    fn is_system_type(&self, token: u32) -> bool {
        self.type_name(token)
            .is_some_and(|(ns, name)| ns == "System" && name == "Type")
    }

    fn enum_underlying_type(&self, token: u32) -> Option<Primitive> {
        match TableId::from_index((token >> 24) as usize)? {
            TableId::TypeDef => self.local_enum(token & 0x00FF_FFFF),
            _ => None,
        }
    }

    fn enum_underlying_type_by_name(&self, name: &str) -> Option<Primitive> {
        // Assembly-qualified names refer elsewhere; strip nothing and match local names only.
        (1..=self.md.tables.rows(TableId::TypeDef))
            .find(|&row| self.full_name(row).as_deref() == Some(name))
            .and_then(|row| self.local_enum(row))
    }
}
