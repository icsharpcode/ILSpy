//! The IL value model: `StackType`, `PrimitiveType`, `Sign` and their conversions
//! (`ILTypeExtensions`, `TypeUtils`), and `IType.GetStackType()` / `ToPrimitiveType()` on the
//! type system.

use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeId, TypeSystem};

/// `StackType`. The order matters: where two branches meet with different types in a stack
/// slot, the larger one wins.
#[derive(Clone, Copy, Debug, PartialEq, Eq, PartialOrd, Ord, Hash)]
pub enum StackType {
    Unknown,
    I4,
    I,
    I8,
    F4,
    F8,
    Obj,
    VT,
    Ref,
    Void,
}

impl StackType {
    pub fn name(self) -> &'static str {
        match self {
            StackType::Unknown => "Unknown",
            StackType::I4 => "I4",
            StackType::I => "I",
            StackType::I8 => "I8",
            StackType::F4 => "F4",
            StackType::F8 => "F8",
            StackType::Obj => "Obj",
            StackType::VT => "VT",
            StackType::Ref => "Ref",
            StackType::Void => "Void",
        }
    }

    pub fn is_integer(self) -> bool {
        matches!(self, StackType::I4 | StackType::I | StackType::I8)
    }

    pub fn is_float(self) -> bool {
        matches!(self, StackType::F4 | StackType::F8)
    }

    /// `ToPrimitiveType(sign)`.
    pub fn to_primitive(self, sign: Sign) -> PrimitiveType {
        let unsigned = sign == Sign::Unsigned;
        match self {
            StackType::I4 if unsigned => PrimitiveType::U4,
            StackType::I4 => PrimitiveType::I4,
            StackType::I8 if unsigned => PrimitiveType::U8,
            StackType::I8 => PrimitiveType::I8,
            StackType::I if unsigned => PrimitiveType::U,
            StackType::I => PrimitiveType::I,
            StackType::F4 => PrimitiveType::R4,
            StackType::F8 => PrimitiveType::R8,
            StackType::Ref => PrimitiveType::Ref,
            StackType::Unknown => PrimitiveType::Unknown,
            _ => PrimitiveType::None,
        }
    }

    /// `ToKnownTypeCode(sign)`.
    pub fn to_known(self, sign: Sign) -> KnownTypeCode {
        let unsigned = sign == Sign::Unsigned;
        match self {
            StackType::I4 if unsigned => KnownTypeCode::UInt32,
            StackType::I4 => KnownTypeCode::Int32,
            StackType::I8 if unsigned => KnownTypeCode::UInt64,
            StackType::I8 => KnownTypeCode::Int64,
            StackType::I if unsigned => KnownTypeCode::UIntPtr,
            StackType::I => KnownTypeCode::IntPtr,
            StackType::F4 => KnownTypeCode::Single,
            StackType::F8 => KnownTypeCode::Double,
            StackType::Obj => KnownTypeCode::Object,
            StackType::Void => KnownTypeCode::Void,
            _ => KnownTypeCode::None,
        }
    }
}

/// `Sign`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum Sign {
    None,
    Signed,
    Unsigned,
}

/// `PrimitiveType` (the values are SRM's `PrimitiveTypeCode`s).
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
#[repr(u8)]
pub enum PrimitiveType {
    None = 0,
    I1 = 4,
    I2 = 6,
    I4 = 8,
    I8 = 10,
    R4 = 12,
    R8 = 13,
    U1 = 5,
    U2 = 7,
    U4 = 9,
    U8 = 11,
    I = 24,
    U = 25,
    Ref = 16,
    R = 254,
    Unknown = 255,
}

impl PrimitiveType {
    pub fn stack_type(self) -> StackType {
        use PrimitiveType::*;
        match self {
            I1 | U1 | I2 | U2 | I4 | U4 => StackType::I4,
            I8 | U8 => StackType::I8,
            I | U => StackType::I,
            R4 => StackType::F4,
            R8 | R => StackType::F8,
            Ref => StackType::Ref,
            Unknown => StackType::Unknown,
            None => StackType::Obj,
        }
    }

    pub fn sign(self) -> Sign {
        use PrimitiveType::*;
        match self {
            I1 | I2 | I4 | I8 | R4 | R8 | R | I => Sign::Signed,
            U1 | U2 | U4 | U8 | U => Sign::Unsigned,
            _ => Sign::None,
        }
    }

    pub fn has_opposite_sign(self) -> bool {
        use PrimitiveType::*;
        matches!(self, I1 | I2 | I4 | I8 | U1 | U2 | U4 | U8 | I | U)
    }

    /// `GetSize()` with an 8-byte native int (`TypeUtils.NativeIntSize`).
    pub fn size(self) -> u32 {
        use PrimitiveType::*;
        match self {
            I1 | U1 => 1,
            I2 | U2 => 2,
            I4 | U4 | R4 => 4,
            I8 | R8 | U8 | R | I | U | Ref => 8,
            _ => 0,
        }
    }

    pub fn is_small_integer(self) -> bool {
        self.size() < 4
    }

    pub fn is_integer(self) -> bool {
        self.stack_type().is_integer()
    }

    pub fn is_float(self) -> bool {
        matches!(
            self,
            PrimitiveType::R4 | PrimitiveType::R8 | PrimitiveType::R
        )
    }

    /// `ToKnownTypeCode()`.
    pub fn to_known(self) -> KnownTypeCode {
        use PrimitiveType::*;
        match self {
            I1 => KnownTypeCode::SByte,
            I2 => KnownTypeCode::Int16,
            I4 => KnownTypeCode::Int32,
            I8 => KnownTypeCode::Int64,
            R4 => KnownTypeCode::Single,
            R8 | R => KnownTypeCode::Double,
            U1 => KnownTypeCode::Byte,
            U2 => KnownTypeCode::UInt16,
            U4 => KnownTypeCode::UInt32,
            U8 => KnownTypeCode::UInt64,
            I => KnownTypeCode::IntPtr,
            U => KnownTypeCode::UIntPtr,
            _ => KnownTypeCode::None,
        }
    }

    /// `KnownTypeCode.ToPrimitiveType()`.
    pub fn from_known(code: KnownTypeCode) -> PrimitiveType {
        use PrimitiveType::*;
        match code {
            KnownTypeCode::SByte => I1,
            KnownTypeCode::Int16 => I2,
            KnownTypeCode::Int32 => I4,
            KnownTypeCode::Int64 => I8,
            KnownTypeCode::Single => R4,
            KnownTypeCode::Double => R8,
            KnownTypeCode::Byte => U1,
            KnownTypeCode::UInt16 | KnownTypeCode::Char => U2,
            KnownTypeCode::UInt32 => U4,
            KnownTypeCode::UInt64 => U8,
            KnownTypeCode::IntPtr => I,
            KnownTypeCode::UIntPtr => U,
            _ => None,
        }
    }
}

impl TypeSystem<'_, '_> {
    /// `IType.Kind` for the kinds stack-type logic distinguishes; `None` for definitions and
    /// parameterized types, whose kind comes from their definition.
    fn skip_modifiers(&self, t: TypeId) -> TypeId {
        match self.data(t) {
            TypeData::Modified { inner, .. } => self.skip_modifiers(*inner),
            _ => t,
        }
    }

    /// The known type code of the definition behind `t` after `GetEnumUnderlyingType()`.
    pub(crate) fn enum_underlying_known(&self, t: TypeId) -> Option<KnownTypeCode> {
        let def = self.definition(t)?;
        let info = self.comp.type_def(def).ok()?;
        let is_definition_or_instance = matches!(
            self.data(t),
            TypeData::Def(_) | TypeData::Inst { .. } | TypeData::Annotated { .. }
        );
        if is_definition_or_instance && info.kind == TypeKind::Enum {
            // An enum's underlying type is a primitive known type.
            return Some(info.enum_underlying.unwrap_or(KnownTypeCode::None));
        }
        Some(info.known)
    }

    /// `IType.Kind == TypeKind.Unknown`: unknown types, and parameterized or annotated types
    /// of them (which take their definition's kind).
    pub fn is_unknown_kind(&self, t: TypeId) -> bool {
        match self.data(t) {
            TypeData::Unknown { .. } | TypeData::UnknownSpecial => true,
            TypeData::Inst { generic, .. } => self.is_unknown_kind(*generic),
            TypeData::Annotated { inner, .. } => self.is_unknown_kind(*inner),
            _ => false,
        }
    }

    /// `IType.GetStackType()`.
    pub fn stack_type(&mut self, t: TypeId) -> StackType {
        let t = self.skip_modifiers(t);
        if self.is_unknown_kind(t) {
            return if self.is_reference_type(t) == Some(true) {
                StackType::Obj
            } else {
                StackType::Unknown
            };
        }
        match self.data(t).clone() {
            TypeData::ByRef(_) => return StackType::Ref,
            TypeData::Pointer(_) | TypeData::NInt | TypeData::NUInt | TypeData::FnPtr(_) => {
                return StackType::I;
            }
            TypeData::TypeParam { .. } => {
                return if self.is_reference_type(t) == Some(true) {
                    StackType::Obj
                } else {
                    StackType::VT
                };
            }
            TypeData::Annotated { inner, .. }
                if matches!(self.data(inner), TypeData::TypeParam { .. }) =>
            {
                return self.stack_type(inner);
            }
            _ => {}
        }
        match self.enum_underlying_known(t) {
            Some(
                KnownTypeCode::Boolean
                | KnownTypeCode::Char
                | KnownTypeCode::SByte
                | KnownTypeCode::Byte
                | KnownTypeCode::Int16
                | KnownTypeCode::UInt16
                | KnownTypeCode::Int32
                | KnownTypeCode::UInt32,
            ) => StackType::I4,
            Some(KnownTypeCode::Int64 | KnownTypeCode::UInt64) => StackType::I8,
            Some(KnownTypeCode::Single) => StackType::F4,
            Some(KnownTypeCode::Double) => StackType::F8,
            Some(KnownTypeCode::Void) => StackType::Void,
            Some(KnownTypeCode::IntPtr | KnownTypeCode::UIntPtr) => StackType::I,
            _ if self.is_reference_type(t) == Some(true) => StackType::Obj,
            _ => StackType::VT,
        }
    }

    /// `IType.ToPrimitiveType()`.
    pub fn primitive_type(&mut self, t: TypeId) -> PrimitiveType {
        let t = self.skip_modifiers(t);
        if self.is_unknown_kind(t) {
            return PrimitiveType::Unknown;
        }
        match self.data(t) {
            TypeData::ByRef(_) => return PrimitiveType::Ref,
            // Without TypeSystemOptions.FunctionPointers a function pointer is a UIntPtr struct.
            TypeData::FnPtr(_) if self.definition(t).is_some() => {}
            TypeData::NInt | TypeData::FnPtr(_) => return PrimitiveType::I,
            TypeData::NUInt => return PrimitiveType::U,
            _ => {}
        }
        match self.enum_underlying_known(t) {
            Some(code) => PrimitiveType::from_known(code),
            None => PrimitiveType::None,
        }
    }
}

/// `TypeUtils.NativeIntSize`: between 4 (`Int32`) and 8 (`Int64`).
pub const NATIVE_INT_SIZE: i32 = 6;

impl TypeSystem<'_, '_> {
    /// `TypeUtils.GetSize`: the size in bytes, `NATIVE_INT_SIZE` for pointer-sized types and
    /// 0 for structs and other types of unknown size.
    pub fn type_size(&mut self, t: TypeId) -> i32 {
        let t = self.skip_modifiers(t);
        if matches!(
            self.data(t),
            TypeData::ByRef(_) | TypeData::Pointer(_) | TypeData::NInt | TypeData::NUInt
        ) {
            return NATIVE_INT_SIZE;
        }
        if let Some(def) = self.definition(t)
            && self
                .comp
                .type_def(def)
                .is_ok_and(|i| i.kind == TypeKind::Class)
        {
            return NATIVE_INT_SIZE;
        }
        match self.enum_underlying_known(t) {
            Some(KnownTypeCode::Boolean | KnownTypeCode::SByte | KnownTypeCode::Byte) => 1,
            Some(KnownTypeCode::Char | KnownTypeCode::Int16 | KnownTypeCode::UInt16) => 2,
            Some(KnownTypeCode::Int32 | KnownTypeCode::UInt32 | KnownTypeCode::Single) => 4,
            Some(KnownTypeCode::IntPtr | KnownTypeCode::UIntPtr) => NATIVE_INT_SIZE,
            Some(KnownTypeCode::Int64 | KnownTypeCode::UInt64 | KnownTypeCode::Double) => 8,
            _ => 0,
        }
    }

    /// `IType.GetSign()`.
    pub fn type_sign(&mut self, t: TypeId) -> Sign {
        let t = self.skip_modifiers(t);
        match self.data(t) {
            TypeData::Pointer(_) | TypeData::NUInt | TypeData::FnPtr(_) => return Sign::Unsigned,
            TypeData::NInt => return Sign::Signed,
            _ => {}
        }
        match self.enum_underlying_known(t) {
            Some(
                KnownTypeCode::SByte
                | KnownTypeCode::Int16
                | KnownTypeCode::Int32
                | KnownTypeCode::Int64
                | KnownTypeCode::IntPtr
                | KnownTypeCode::Single
                | KnownTypeCode::Double
                | KnownTypeCode::Decimal,
            ) => Sign::Signed,
            Some(
                KnownTypeCode::UIntPtr
                | KnownTypeCode::Char
                | KnownTypeCode::Boolean
                | KnownTypeCode::Byte
                | KnownTypeCode::UInt16
                | KnownTypeCode::UInt32
                | KnownTypeCode::UInt64,
            ) => Sign::Unsigned,
            _ => Sign::None,
        }
    }

    /// `TypeUtils.IsCompatibleTypeForMemoryAccess`: accessing memory of `memory_type` as
    /// `access_type` gives the same result.
    pub fn is_compatible_type_for_memory_access(
        &mut self,
        memory_type: TypeId,
        access_type: TypeId,
    ) -> bool {
        // `NormalizeTypeVisitor.TypeErasure`: both sides normalize alike within one function.
        let m = self.normalize_parameter(memory_type);
        let a = self.normalize_parameter(access_type);
        if m == a {
            return true;
        }
        if self.is_reference_type(m) == Some(true) && self.is_reference_type(a) == Some(true) {
            return true;
        }
        let (ms, as_) = (self.stack_type(m), self.stack_type(a));
        if ms == as_ && ms.is_integer() && self.type_size(m) == self.type_size(a) {
            return true;
        }
        self.is_unknown_kind(m) || self.is_unknown_kind(a)
    }
}
