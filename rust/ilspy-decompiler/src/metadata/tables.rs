//! The `#~` / `#-` table stream (ECMA-335 II.22, II.24.2.6).
//!
//! Every table's columns are declared once in `tables!`; the macro produces the column
//! schema used for row sizes and a typed row struct per table.

use super::bytes::Bytes;
use super::heaps::{BlobIdx, GuidIdx, StrIdx};
use super::{Error, Result};

/// The kinds of coded index (II.24.2.6), with their tag-to-table maps.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub enum CodedKind {
    TypeDefOrRef,
    HasConstant,
    HasCustomAttribute,
    HasFieldMarshal,
    HasDeclSecurity,
    MemberRefParent,
    HasSemantics,
    MethodDefOrRef,
    MemberForwarded,
    Implementation,
    CustomAttributeType,
    ResolutionScope,
    TypeOrMethodDef,
}

impl CodedKind {
    pub fn tables(self) -> &'static [Option<TableId>] {
        use TableId::*;
        match self {
            CodedKind::TypeDefOrRef => &[Some(TypeDef), Some(TypeRef), Some(TypeSpec)],
            CodedKind::HasConstant => &[Some(Field), Some(Param), Some(Property)],
            CodedKind::HasCustomAttribute => &[
                Some(MethodDef),
                Some(Field),
                Some(TypeRef),
                Some(TypeDef),
                Some(Param),
                Some(InterfaceImpl),
                Some(MemberRef),
                Some(Module),
                Some(DeclSecurity),
                Some(Property),
                Some(Event),
                Some(StandAloneSig),
                Some(ModuleRef),
                Some(TypeSpec),
                Some(Assembly),
                Some(AssemblyRef),
                Some(File),
                Some(ExportedType),
                Some(ManifestResource),
                Some(GenericParam),
                Some(GenericParamConstraint),
                Some(MethodSpec),
            ],
            CodedKind::HasFieldMarshal => &[Some(Field), Some(Param)],
            CodedKind::HasDeclSecurity => &[Some(TypeDef), Some(MethodDef), Some(Assembly)],
            CodedKind::MemberRefParent => &[
                Some(TypeDef),
                Some(TypeRef),
                Some(ModuleRef),
                Some(MethodDef),
                Some(TypeSpec),
            ],
            CodedKind::HasSemantics => &[Some(Event), Some(Property)],
            CodedKind::MethodDefOrRef => &[Some(MethodDef), Some(MemberRef)],
            CodedKind::MemberForwarded => &[Some(Field), Some(MethodDef)],
            CodedKind::Implementation => &[Some(File), Some(AssemblyRef), Some(ExportedType)],
            CodedKind::CustomAttributeType => &[None, None, Some(MethodDef), Some(MemberRef), None],
            CodedKind::ResolutionScope => &[
                Some(Module),
                Some(ModuleRef),
                Some(AssemblyRef),
                Some(TypeRef),
            ],
            CodedKind::TypeOrMethodDef => &[Some(TypeDef), Some(MethodDef)],
        }
    }

    pub fn tag_bits(self) -> u32 {
        (self.tables().len() as u32)
            .next_power_of_two()
            .trailing_zeros()
    }

    /// The raw column value that refers to `table`/`row`, as stored (and searched) on disk.
    pub fn encode(self, table: TableId, row: u32) -> Option<u32> {
        let tag = self.tables().iter().position(|&t| t == Some(table))? as u32;
        Some(row << self.tag_bits() | tag)
    }
}

/// A coded index as stored; decoding is lazy so a bad tag only fails the column it is in.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
pub struct Coded {
    pub kind: CodedKind,
    pub raw: u32,
}

impl Coded {
    pub fn decode(self) -> Result<(TableId, u32)> {
        let bits = self.kind.tag_bits();
        let tag = (self.raw & ((1 << bits) - 1)) as usize;
        let table = self
            .kind
            .tables()
            .get(tag)
            .copied()
            .flatten()
            .ok_or(Error::Malformed("invalid coded index tag"))?;
        let row = self.raw >> bits;
        if row > 0x00FF_FFFF {
            return Err(Error::Malformed("coded index row out of range"));
        }
        Ok((table, row))
    }

    /// The metadata token, keeping the table even when the row is 0 (a nil reference).
    pub fn token(self) -> Result<u32> {
        self.decode().map(|(table, row)| token(table, row))
    }
}

pub fn token(table: TableId, row: u32) -> u32 {
    (table as u32) << 24 | row
}

#[derive(Clone, Copy, Debug)]
pub enum Col {
    U16,
    U32,
    Str,
    Guid,
    Blob,
    Table(TableId),
    Coded(CodedKind),
}

/// Accessors for one row's cells; `Row::decode` reads typed rows from it.
pub trait Row: Sized {
    const TABLE: TableId;
    fn decode(cells: &[u32]) -> Self;
}

macro_rules! col_kind {
    (u16) => {
        Col::U16
    };
    (u32) => {
        Col::U32
    };
    (str) => {
        Col::Str
    };
    (guid) => {
        Col::Guid
    };
    (blob) => {
        Col::Blob
    };
    ([$t:ident]) => {
        Col::Table(TableId::$t)
    };
    ({$c:ident}) => {
        Col::Coded(CodedKind::$c)
    };
}

macro_rules! col_type {
    (u16) => {
        u16
    };
    (u32) => {
        u32
    };
    (str) => {
        StrIdx
    };
    (guid) => {
        GuidIdx
    };
    (blob) => {
        BlobIdx
    };
    ([$t:ident]) => {
        u32
    };
    ({$c:ident}) => {
        Coded
    };
}

macro_rules! col_value {
    (u16, $v:expr) => {
        $v as u16
    };
    (u32, $v:expr) => {
        $v
    };
    (str, $v:expr) => {
        StrIdx($v)
    };
    (guid, $v:expr) => {
        GuidIdx($v)
    };
    (blob, $v:expr) => {
        BlobIdx($v)
    };
    ([$t:ident], $v:expr) => {
        $v
    };
    ({$c:ident}, $v:expr) => {
        Coded {
            kind: CodedKind::$c,
            raw: $v,
        }
    };
}

macro_rules! tables {
    ($( $id:ident = $num:literal { $( $field:ident : $kind:tt ),* $(,)? } )*) => {
        /// Table numbers (II.22).
        #[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord)]
        #[repr(u8)]
        pub enum TableId { $( $id = $num ),* }

        impl TableId {
            pub const ALL: &'static [TableId] = &[ $( TableId::$id ),* ];

            pub fn from_index(index: usize) -> Option<TableId> {
                Self::ALL.iter().copied().find(|&t| t as usize == index)
            }

            pub fn name(self) -> &'static str {
                match self { $( TableId::$id => stringify!($id) ),* }
            }

            pub fn columns(self) -> &'static [Col] {
                match self { $( TableId::$id => &[ $( col_kind!($kind) ),* ] ),* }
            }
        }

        /// One typed row struct per table.
        pub mod rows {
            use super::*;
            $(
                #[derive(Clone, Copy, Debug)]
                pub struct $id { $( pub $field: col_type!($kind) ),* }

                impl Row for $id {
                    const TABLE: TableId = TableId::$id;
                    #[allow(unused_assignments)]
                    fn decode(cells: &[u32]) -> Self {
                        let mut i = 0;
                        $( let $field = col_value!($kind, cells[i]); i += 1; )*
                        $id { $( $field ),* }
                    }
                }
            )*
        }
    };
}

tables! {
    Module = 0x00 { generation: u16, name: str, mvid: guid, enc_id: guid, enc_base_id: guid }
    TypeRef = 0x01 { resolution_scope: {ResolutionScope}, name: str, namespace: str }
    TypeDef = 0x02 {
        flags: u32, name: str, namespace: str, extends: {TypeDefOrRef},
        field_list: [Field], method_list: [MethodDef],
    }
    FieldPtr = 0x03 { field: [Field] }
    Field = 0x04 { flags: u16, name: str, signature: blob }
    MethodPtr = 0x05 { method: [MethodDef] }
    MethodDef = 0x06 {
        rva: u32, impl_flags: u16, flags: u16, name: str, signature: blob, param_list: [Param],
    }
    ParamPtr = 0x07 { param: [Param] }
    Param = 0x08 { flags: u16, sequence: u16, name: str }
    InterfaceImpl = 0x09 { class: [TypeDef], interface: {TypeDefOrRef} }
    MemberRef = 0x0A { class: {MemberRefParent}, name: str, signature: blob }
    // `kind` is the element type byte plus one byte of padding.
    Constant = 0x0B { kind: u16, parent: {HasConstant}, value: blob }
    CustomAttribute = 0x0C { parent: {HasCustomAttribute}, constructor: {CustomAttributeType}, value: blob }
    FieldMarshal = 0x0D { parent: {HasFieldMarshal}, native_type: blob }
    DeclSecurity = 0x0E { action: u16, parent: {HasDeclSecurity}, permission_set: blob }
    ClassLayout = 0x0F { packing_size: u16, class_size: u32, parent: [TypeDef] }
    FieldLayout = 0x10 { offset: u32, field: [Field] }
    StandAloneSig = 0x11 { signature: blob }
    EventMap = 0x12 { parent: [TypeDef], event_list: [Event] }
    EventPtr = 0x13 { event: [Event] }
    Event = 0x14 { flags: u16, name: str, event_type: {TypeDefOrRef} }
    PropertyMap = 0x15 { parent: [TypeDef], property_list: [Property] }
    PropertyPtr = 0x16 { property: [Property] }
    Property = 0x17 { flags: u16, name: str, signature: blob }
    MethodSemantics = 0x18 { semantics: u16, method: [MethodDef], association: {HasSemantics} }
    MethodImpl = 0x19 { class: [TypeDef], body: {MethodDefOrRef}, declaration: {MethodDefOrRef} }
    ModuleRef = 0x1A { name: str }
    TypeSpec = 0x1B { signature: blob }
    ImplMap = 0x1C { flags: u16, member_forwarded: {MemberForwarded}, import_name: str, import_scope: [ModuleRef] }
    FieldRva = 0x1D { rva: u32, field: [Field] }
    EncLog = 0x1E { token: u32, func_code: u32 }
    EncMap = 0x1F { token: u32 }
    Assembly = 0x20 {
        hash_alg_id: u32, major: u16, minor: u16, build: u16, revision: u16, flags: u32,
        public_key: blob, name: str, culture: str,
    }
    AssemblyProcessor = 0x21 { processor: u32 }
    AssemblyOs = 0x22 { platform_id: u32, major: u32, minor: u32 }
    AssemblyRef = 0x23 {
        major: u16, minor: u16, build: u16, revision: u16, flags: u32,
        public_key_or_token: blob, name: str, culture: str, hash_value: blob,
    }
    AssemblyRefProcessor = 0x24 { processor: u32, assembly_ref: [AssemblyRef] }
    AssemblyRefOs = 0x25 { platform_id: u32, major: u32, minor: u32, assembly_ref: [AssemblyRef] }
    File = 0x26 { flags: u32, name: str, hash_value: blob }
    ExportedType = 0x27 { flags: u32, type_def_id: u32, name: str, namespace: str, implementation: {Implementation} }
    ManifestResource = 0x28 { offset: u32, flags: u32, name: str, implementation: {Implementation} }
    NestedClass = 0x29 { nested: [TypeDef], enclosing: [TypeDef] }
    GenericParam = 0x2A { number: u16, flags: u16, owner: {TypeOrMethodDef}, name: str }
    MethodSpec = 0x2B { method: {MethodDefOrRef}, instantiation: blob }
    GenericParamConstraint = 0x2C { owner: [GenericParam], constraint: {TypeDefOrRef} }
}

pub const TABLE_COUNT: usize = 0x2D;
const MAX_COLUMNS: usize = 9;

#[derive(Clone, Copy, Debug, Default)]
struct TableInfo {
    rows: u32,
    row_size: usize,
    offset: usize,
    columns: [(u8, u8); MAX_COLUMNS],
}

#[derive(Clone, Debug)]
pub struct Tables<'a> {
    data: &'a [u8],
    pub major_version: u8,
    pub minor_version: u8,
    pub heap_sizes: u8,
    pub valid: u64,
    pub sorted: u64,
    info: [TableInfo; TABLE_COUNT],
}

impl<'a> Tables<'a> {
    /// `all_large` forces 4-byte indices (the `#JTD` minimal-delta format).
    pub fn parse(stream: &'a [u8], all_large: bool) -> Result<Self> {
        let mut b = Bytes::new(stream);
        b.skip(4)?;
        let major_version = b.u8()?;
        let minor_version = b.u8()?;
        let heap_sizes = b.u8()?;
        b.skip(1)?;
        let valid = b.u64()?;
        let sorted = b.u64()?;
        if valid >> TABLE_COUNT != 0 {
            return Err(Error::Unsupported("tables beyond GenericParamConstraint"));
        }
        let mut rows = [0u32; TABLE_COUNT];
        for (i, r) in rows.iter_mut().enumerate() {
            if valid & (1 << i) != 0 {
                *r = b.u32()?;
            }
        }
        if heap_sizes & 0x40 != 0 {
            b.skip(4)?;
        }
        let large_heap = |bit: u8| all_large || heap_sizes & bit != 0;
        let size = |col: Col| -> u8 {
            match col {
                Col::U16 => 2,
                Col::U32 => 4,
                Col::Str => 2 + 2 * u8::from(large_heap(0x01)),
                Col::Guid => 2 + 2 * u8::from(large_heap(0x02)),
                Col::Blob => 2 + 2 * u8::from(large_heap(0x04)),
                Col::Table(t) => 2 + 2 * u8::from(all_large || rows[t as usize] >= 0x1_0000),
                Col::Coded(k) => {
                    let limit = 1u32 << (16 - k.tag_bits());
                    let large = all_large
                        || k.tables()
                            .iter()
                            .flatten()
                            .any(|&t| rows[t as usize] >= limit);
                    2 + 2 * u8::from(large)
                }
            }
        };
        let mut info = [TableInfo::default(); TABLE_COUNT];
        let mut offset = b.pos();
        for &table in TableId::ALL {
            let t = &mut info[table as usize];
            let mut col_offset = 0u8;
            for (i, &col) in table.columns().iter().enumerate() {
                let s = size(col);
                t.columns[i] = (col_offset, s);
                col_offset += s;
            }
            t.rows = rows[table as usize];
            t.row_size = usize::from(col_offset);
            t.offset = offset;
            offset += t.row_size * t.rows as usize;
        }
        if offset > stream.len() {
            return Err(Error::Malformed("tables extend beyond the table stream"));
        }
        Ok(Tables {
            data: stream,
            major_version,
            minor_version,
            heap_sizes,
            valid,
            sorted,
            info,
        })
    }

    pub fn rows(&self, table: TableId) -> u32 {
        self.info[table as usize].rows
    }

    pub fn row_size(&self, table: TableId) -> usize {
        self.info[table as usize].row_size
    }

    fn check_row(&self, table: TableId, row: u32) -> Result<&TableInfo> {
        let t = &self.info[table as usize];
        if row == 0 || row > t.rows {
            return Err(Error::Malformed("row number out of range"));
        }
        Ok(t)
    }

    /// The raw value of one cell. `row` is 1-based.
    pub fn cell(&self, table: TableId, row: u32, column: usize) -> Result<u32> {
        let t = self.check_row(table, row)?;
        Ok(self.cell_unchecked(t, row - 1, column))
    }

    fn cell_unchecked(&self, t: &TableInfo, row_index: u32, column: usize) -> u32 {
        let (col_offset, size) = t.columns[column];
        let at = t.offset + row_index as usize * t.row_size + usize::from(col_offset);
        let bytes = &self.data[at..at + usize::from(size)];
        match size {
            2 => u32::from(u16::from_le_bytes([bytes[0], bytes[1]])),
            _ => u32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3]]),
        }
    }

    pub fn get<R: Row>(&self, row: u32) -> Result<R> {
        let t = self.check_row(R::TABLE, row)?;
        let mut cells = [0u32; MAX_COLUMNS];
        for (i, cell) in cells.iter_mut().enumerate().take(R::TABLE.columns().len()) {
            *cell = self.cell_unchecked(t, row - 1, i);
        }
        Ok(R::decode(&cells))
    }

    /// Binary search for a row whose `column` equals `value`, assuming the table is sorted on
    /// it; on unsorted data it returns what System.Reflection.Metadata's search returns.
    /// Returns a 1-based row number.
    pub fn binary_search(&self, table: TableId, column: usize, value: u32) -> Option<u32> {
        let t = &self.info[table as usize];
        let (mut lo, mut hi) = (0i64, i64::from(t.rows) - 1);
        while lo <= hi {
            let mid = (lo + hi) / 2;
            let v = self.cell_unchecked(t, mid as u32, column);
            if value > v {
                lo = mid + 1;
            } else if value < v {
                hi = mid - 1;
            } else {
                return Some(mid as u32 + 1);
            }
        }
        None
    }

    /// All rows around a binary-search hit whose `column` equals `value`, as 1-based rows.
    pub fn binary_search_range(
        &self,
        table: TableId,
        column: usize,
        value: u32,
    ) -> std::ops::Range<u32> {
        let Some(hit) = self.binary_search(table, column, value) else {
            return 1..1;
        };
        let t = &self.info[table as usize];
        let matches = |row: u32| self.cell_unchecked(t, row - 1, column) == value;
        let mut start = hit;
        while start > 1 && matches(start - 1) {
            start -= 1;
        }
        let mut end = hit;
        while end < t.rows && matches(end + 1) {
            end += 1;
        }
        start..end + 1
    }

    /// The first row whose `column` equals `value`.
    pub fn linear_search(&self, table: TableId, column: usize, value: u32) -> Option<u32> {
        let t = &self.info[table as usize];
        (0..t.rows)
            .find(|&i| self.cell_unchecked(t, i, column) == value)
            .map(|i| i + 1)
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn coded_index_tag_widths_match_ecma() {
        assert_eq!(CodedKind::HasCustomAttribute.tag_bits(), 5);
        assert_eq!(CodedKind::CustomAttributeType.tag_bits(), 3);
        assert_eq!(CodedKind::MemberRefParent.tag_bits(), 3);
        assert_eq!(CodedKind::ResolutionScope.tag_bits(), 2);
        assert_eq!(CodedKind::TypeOrMethodDef.tag_bits(), 1);
    }

    #[test]
    fn coded_index_decoding() {
        let c = Coded {
            kind: CodedKind::TypeDefOrRef,
            raw: 0x49,
        };
        assert_eq!(c.decode(), Ok((TableId::TypeRef, 0x12)));
        assert_eq!(c.token(), Ok(0x0100_0012));
        let bad = Coded {
            kind: CodedKind::TypeDefOrRef,
            raw: 0x03,
        };
        assert!(bad.decode().is_err());
        assert_eq!(
            CodedKind::TypeDefOrRef.encode(TableId::TypeRef, 0x12),
            Some(0x49)
        );
    }

    #[test]
    fn every_table_fits_the_column_limit() {
        assert!(
            TableId::ALL
                .iter()
                .all(|t| t.columns().len() <= MAX_COLUMNS)
        );
        assert_eq!(TableId::ALL.len(), TABLE_COUNT);
    }
}
