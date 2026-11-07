//! Signature blobs (ECMA-335 II.23.2), decoded into plain trees.
//!
//! The decoder accepts and rejects exactly what System.Reflection.Metadata's `SignatureDecoder`
//! does, so the two can be compared signature by signature.

use super::bytes::Bytes;
use super::tables::{TableId, token};
use super::{Error, Result};

#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum Primitive {
    Void,
    Boolean,
    Char,
    SByte,
    Byte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Single,
    Double,
    String,
    TypedReference,
    IntPtr,
    UIntPtr,
    Object,
}

impl Primitive {
    fn from_code(code: u32) -> Option<Self> {
        use Primitive::*;
        Some(match code {
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
            _ => return None,
        })
    }

    /// The `System` type name.
    pub fn name(self) -> &'static str {
        use Primitive::*;
        match self {
            Void => "Void",
            Boolean => "Boolean",
            Char => "Char",
            SByte => "SByte",
            Byte => "Byte",
            Int16 => "Int16",
            UInt16 => "UInt16",
            Int32 => "Int32",
            UInt32 => "UInt32",
            Int64 => "Int64",
            UInt64 => "UInt64",
            Single => "Single",
            Double => "Double",
            String => "String",
            TypedReference => "TypedReference",
            IntPtr => "IntPtr",
            UIntPtr => "UIntPtr",
            Object => "Object",
        }
    }
}

/// How a type handle was introduced: by `CLASS`, by `VALUETYPE`, or neither (modifiers).
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum HandleKind {
    Unspecified,
    Class,
    ValueType,
}

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub struct ArrayShape {
    pub rank: u32,
    pub sizes: Vec<u32>,
    pub lower_bounds: Vec<i32>,
}

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub enum SigType {
    Primitive(Primitive),
    /// A TypeDef, TypeRef or (in modifiers only) TypeSpec token.
    Handle {
        token: u32,
        kind: HandleKind,
    },
    GenericInst {
        generic: Box<SigType>,
        args: Vec<SigType>,
    },
    TypeParam(u32),
    MethodParam(u32),
    Pointer(Box<SigType>),
    ByRef(Box<SigType>),
    Pinned(Box<SigType>),
    SzArray(Box<SigType>),
    Array {
        elem: Box<SigType>,
        shape: ArrayShape,
    },
    FnPtr(Box<MethodSig>),
    Modified {
        modifier: Box<SigType>,
        inner: Box<SigType>,
        required: bool,
    },
}

#[derive(Clone, Debug, PartialEq, Eq, Hash)]
pub struct MethodSig {
    pub header: u8,
    pub generic_params: u32,
    pub ret: SigType,
    pub params: Vec<SigType>,
    /// Parameters before the vararg sentinel; equals `params.len()` without one.
    pub required_params: usize,
}

pub const HEADER_KIND_FIELD: u8 = 0x06;
pub const HEADER_KIND_LOCALS: u8 = 0x07;
pub const HEADER_KIND_PROPERTY: u8 = 0x08;
pub const HEADER_KIND_METHOD_SPEC: u8 = 0x0A;
const HEADER_GENERIC: u8 = 0x10;
const SENTINEL: u32 = 0x41;

pub const HEADER_KIND_METHOD: u8 = 0x00;

/// The signature kind of a header byte. Calling conventions 0-5 and 9 share the nibble and all
/// mean "method".
pub fn header_kind(header: u8) -> u8 {
    match header & 0x0F {
        0..=5 | 9 => HEADER_KIND_METHOD,
        kind => kind,
    }
}

fn type_handle(b: &mut Bytes<'_>, kind: HandleKind, allow_spec: bool) -> Result<SigType> {
    let raw = b.compressed_u32()?;
    let table = match raw & 3 {
        0 => TableId::TypeDef,
        1 => TableId::TypeRef,
        2 if allow_spec => TableId::TypeSpec,
        2 => {
            return Err(Error::Malformed(
                "TypeSpec where TypeDef or TypeRef expected",
            ));
        }
        _ => return Err(Error::Malformed("invalid TypeDefOrRefOrSpec tag")),
    };
    let row = raw >> 2;
    if row == 0 {
        return Err(Error::Malformed("nil type handle"));
    }
    Ok(SigType::Handle {
        token: token(table, row),
        kind,
    })
}

fn type_with_code(b: &mut Bytes<'_>, code: u32) -> Result<SigType> {
    if let Some(p) = Primitive::from_code(code) {
        return Ok(SigType::Primitive(p));
    }
    let boxed = |b: &mut Bytes<'_>| decode_type(b).map(Box::new);
    Ok(match code {
        0x0F => SigType::Pointer(boxed(b)?),
        0x10 => SigType::ByRef(boxed(b)?),
        0x45 => SigType::Pinned(boxed(b)?),
        0x1D => SigType::SzArray(boxed(b)?),
        0x1B => SigType::FnPtr(Box::new(decode_method(b)?)),
        0x14 => {
            let elem = boxed(b)?;
            let rank = b.compressed_u32()?;
            let sizes = (0..b.compressed_u32()?)
                .map(|_| b.compressed_u32())
                .collect::<Result<_>>()?;
            let lower_bounds = (0..b.compressed_u32()?)
                .map(|_| b.compressed_i32())
                .collect::<Result<_>>()?;
            SigType::Array {
                elem,
                shape: ArrayShape {
                    rank,
                    sizes,
                    lower_bounds,
                },
            }
        }
        0x1F | 0x20 => SigType::Modified {
            modifier: Box::new(type_handle(b, HandleKind::Unspecified, true)?),
            inner: boxed(b)?,
            required: code == 0x1F,
        },
        0x15 => SigType::GenericInst {
            generic: boxed(b)?,
            args: type_sequence(b)?,
        },
        0x13 => SigType::TypeParam(b.compressed_u32()?),
        0x1E => SigType::MethodParam(b.compressed_u32()?),
        0x11 => type_handle(b, HandleKind::ValueType, false)?,
        0x12 => type_handle(b, HandleKind::Class, false)?,
        _ => return Err(Error::Malformed("unexpected signature type code")),
    })
}

/// One type (II.23.2.12 and its relatives).
pub fn decode_type(b: &mut Bytes<'_>) -> Result<SigType> {
    let code = b.compressed_u32()?;
    type_with_code(b, code)
}

fn type_sequence(b: &mut Bytes<'_>) -> Result<Vec<SigType>> {
    let count = b.compressed_u32()?;
    if count == 0 {
        return Err(Error::Malformed("empty type sequence"));
    }
    (0..count).map(|_| decode_type(b)).collect()
}

fn expect_kind(b: &mut Bytes<'_>, kind: u8) -> Result<u8> {
    let header = b.u8()?;
    if header_kind(header) != kind {
        return Err(Error::Malformed("unexpected signature header"));
    }
    Ok(header)
}

/// A method definition, reference, stand-alone method or property signature.
pub fn decode_method(b: &mut Bytes<'_>) -> Result<MethodSig> {
    let header = b.u8()?;
    if !matches!(
        header_kind(header),
        HEADER_KIND_METHOD | HEADER_KIND_PROPERTY
    ) {
        return Err(Error::Malformed("not a method or property signature"));
    }
    let generic_params = if header & HEADER_GENERIC != 0 {
        b.compressed_u32()?
    } else {
        0
    };
    let count = b.compressed_u32()?;
    let ret = decode_type(b)?;
    let mut params = Vec::new();
    let mut required_params = None;
    for _ in 0..count {
        let code = b.compressed_u32()?;
        if code == SENTINEL && required_params.is_none() {
            required_params = Some(params.len());
            params.push(decode_type(b)?);
        } else {
            params.push(type_with_code(b, code)?);
        }
    }
    let required_params = required_params.unwrap_or(params.len());
    Ok(MethodSig {
        header,
        generic_params,
        ret,
        params,
        required_params,
    })
}

pub fn decode_field(b: &mut Bytes<'_>) -> Result<SigType> {
    expect_kind(b, HEADER_KIND_FIELD)?;
    decode_type(b)
}

pub fn decode_locals(b: &mut Bytes<'_>) -> Result<Vec<SigType>> {
    expect_kind(b, HEADER_KIND_LOCALS)?;
    type_sequence(b)
}

pub fn decode_method_spec(b: &mut Bytes<'_>) -> Result<Vec<SigType>> {
    expect_kind(b, HEADER_KIND_METHOD_SPEC)?;
    type_sequence(b)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn decodes_generic_method_with_modifiers_and_arrays() {
        // generic<1> instance int32 (!!0[], int32 modreq(TypeRef 1)&, int32[0...,0...])
        let blob = [
            0x30, 0x01, 0x03, 0x08, 0x1D, 0x1E, 0x00, 0x10, 0x1F, 0x05, 0x08, 0x14, 0x08, 0x02,
            0x00, 0x02, 0x00, 0x00,
        ];
        let sig = decode_method(&mut Bytes::new(&blob)).unwrap();
        assert_eq!(sig.generic_params, 1);
        assert_eq!(sig.ret, SigType::Primitive(Primitive::Int32));
        assert_eq!(sig.params.len(), 3);
        assert_eq!(sig.required_params, 3);
        assert!(matches!(&sig.params[1], SigType::ByRef(inner)
            if matches!(**inner, SigType::Modified { required: true, .. })));
    }

    #[test]
    fn vararg_sentinel_splits_required_parameters() {
        // vararg void (int32, ..., string)
        let blob = [0x05, 0x02, 0x01, 0x08, 0x41, 0x0E];
        let sig = decode_method(&mut Bytes::new(&blob)).unwrap();
        assert_eq!(sig.required_params, 1);
        assert_eq!(sig.params.len(), 2);
    }

    #[test]
    fn rejects_what_srm_rejects() {
        // TypeSpec in CLASS, empty locals, TypeDef row 0
        assert!(decode_field(&mut Bytes::new(&[0x06, 0x12, 0x06])).is_err());
        assert!(decode_locals(&mut Bytes::new(&[0x07, 0x00])).is_err());
        assert!(decode_field(&mut Bytes::new(&[0x06, 0x12, 0x00])).is_err());
    }
}
