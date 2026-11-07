//! `TypeSystemOptions` bits, as the host passes them.

pub const DYNAMIC: u32 = 0x1;
pub const TUPLE: u32 = 0x2;
pub const EXTENSION_METHODS: u32 = 0x4;
pub const DECIMAL_CONSTANTS: u32 = 0x20;
pub const KEEP_MODIFIERS: u32 = 0x40;
pub const READ_ONLY_STRUCTS_AND_PARAMETERS: u32 = 0x80;
pub const REF_STRUCTS: u32 = 0x100;
pub const UNMANAGED_CONSTRAINTS: u32 = 0x200;
pub const NULLABILITY: u32 = 0x400;
pub const READ_ONLY_METHODS: u32 = 0x800;
pub const NATIVE_INTEGERS: u32 = 0x1000;
pub const FUNCTION_POINTERS: u32 = 0x2000;
pub const SCOPED_REF: u32 = 0x4000;
pub const NATIVE_INTEGERS_WITHOUT_ATTRIBUTE: u32 = 0x8000;
pub const REF_READ_ONLY_PARAMETERS: u32 = 0x1_0000;
pub const PARAMS_COLLECTIONS: u32 = 0x2_0000;
