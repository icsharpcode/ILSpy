//! ECMA-335 metadata reader: PE file layout, metadata root, heaps, tables, signatures,
//! custom attribute values and method bodies. No PDB reading; the host provides debug info.

pub mod bytes;
pub mod custom_attribute;
mod error;
pub mod heaps;
pub mod il_body;
pub mod listing;
pub mod pe;
pub mod signature;
pub mod tables;

use std::borrow::Cow;

pub use error::{Error, Result};
use heaps::{BlobHeap, BlobIdx, GuidHeap, GuidIdx, StrIdx, StringHeap, UserStringHeap};
use il_body::MethodBody;
use pe::PeImage;
use tables::{CodedKind, TableId, Tables, rows};

use bytes::Bytes;

/// A parsed module. Borrows the image; nothing is copied.
#[derive(Clone, Debug)]
pub struct Metadata<'a> {
    pub pe: PeImage<'a>,
    pub version: String,
    pub strings: StringHeap<'a>,
    pub blobs: BlobHeap<'a>,
    pub guids: GuidHeap<'a>,
    pub user_strings: UserStringHeap<'a>,
    pub tables: Tables<'a>,
}

impl<'a> Metadata<'a> {
    pub fn parse(image: &'a [u8]) -> Result<Self> {
        let pe = PeImage::parse(image)?;
        let root = pe.directory_data(pe.cli.metadata)?;
        let mut b = Bytes::new(root);
        if b.u32()? != 0x424A_5342 {
            return Err(Error::Malformed("missing metadata signature"));
        }
        b.skip(8)?; // major, minor, reserved
        let version_len = b.u32()? as usize;
        let version_bytes = b.take(version_len)?;
        let version_end = version_bytes
            .iter()
            .position(|&c| c == 0)
            .unwrap_or(version_len);
        let version = String::from_utf8_lossy(&version_bytes[..version_end]).into_owned();
        b.skip(2)?; // flags
        let stream_count = b.u16()?;

        let (mut strings, mut blobs, mut guids, mut user_strings) = Default::default();
        let mut table_stream = None;
        let mut minimal_delta = false;
        for _ in 0..stream_count {
            let offset = b.u32()? as usize;
            let size = b.u32()? as usize;
            let name_start = b.pos();
            let name_len = root[name_start..]
                .iter()
                .take(32)
                .position(|&c| c == 0)
                .ok_or(Error::Malformed("unterminated stream name"))?;
            let name = &root[name_start..name_start + name_len];
            b.skip(name_len + 1)?;
            b.align(4)?;
            let data = root
                .get(offset..offset.checked_add(size).ok_or(Error::Truncated)?)
                .ok_or(Error::Malformed("stream outside metadata"))?;
            match name {
                b"#~" | b"#-" => table_stream = Some(data),
                b"#Strings" => strings = StringHeap::new(data),
                b"#Blob" => blobs = BlobHeap::new(data),
                b"#GUID" => guids = GuidHeap::new(data),
                b"#US" => user_strings = UserStringHeap::new(data),
                b"#JTD" => minimal_delta = true,
                _ => {}
            }
        }
        let tables = Tables::parse(
            table_stream.ok_or(Error::Malformed("no table stream"))?,
            minimal_delta,
        )?;
        Ok(Metadata {
            pe,
            version,
            strings,
            blobs,
            guids,
            user_strings,
            tables,
        })
    }

    pub fn string(&self, idx: StrIdx) -> Result<Cow<'a, str>> {
        self.strings.get(idx)
    }

    pub fn blob(&self, idx: BlobIdx) -> Result<&'a [u8]> {
        self.blobs.get(idx)
    }

    pub fn guid(&self, idx: GuidIdx) -> Result<[u8; 16]> {
        self.guids.get(idx)
    }

    pub fn get<R: tables::Row>(&self, row: u32) -> Result<R> {
        self.tables.get(row)
    }

    /// The method body at `rva`.
    pub fn method_body(&self, rva: u32) -> Result<MethodBody<'a>> {
        MethodBody::parse(self.pe.section_data(rva)?)
    }

    /// The list owned by `row` of `owner`, where `column` holds each owner's first list row,
    /// mapped through the pointer table if present.
    fn owned_list(
        &self,
        owner: TableId,
        row: u32,
        column: usize,
        list: TableId,
        ptr: TableId,
    ) -> Result<Vec<u32>> {
        let first = self.tables.cell(owner, row, column)?;
        if first == 0 {
            return Ok(Vec::new());
        }
        let use_ptr = self.tables.rows(ptr) > 0;
        let last = if row == self.tables.rows(owner) {
            i64::from(self.tables.rows(if use_ptr { ptr } else { list }))
        } else {
            i64::from(self.tables.cell(owner, row + 1, column)?) - 1
        };
        (i64::from(first)..=last)
            .map(|r| {
                let r = r as u32;
                if use_ptr {
                    self.tables.cell(ptr, r, 0)
                } else {
                    Ok(r)
                }
            })
            .collect()
    }

    pub fn type_fields(&self, type_def: u32) -> Result<Vec<u32>> {
        self.owned_list(
            TableId::TypeDef,
            type_def,
            4,
            TableId::Field,
            TableId::FieldPtr,
        )
    }

    pub fn type_methods(&self, type_def: u32) -> Result<Vec<u32>> {
        self.owned_list(
            TableId::TypeDef,
            type_def,
            5,
            TableId::MethodDef,
            TableId::MethodPtr,
        )
    }

    pub fn method_params(&self, method: u32) -> Result<Vec<u32>> {
        self.owned_list(
            TableId::MethodDef,
            method,
            5,
            TableId::Param,
            TableId::ParamPtr,
        )
    }

    pub fn type_events(&self, type_def: u32) -> Result<Vec<u32>> {
        match self.tables.linear_search(TableId::EventMap, 0, type_def) {
            Some(map) => {
                self.owned_list(TableId::EventMap, map, 1, TableId::Event, TableId::EventPtr)
            }
            None => Ok(Vec::new()),
        }
    }

    pub fn type_properties(&self, type_def: u32) -> Result<Vec<u32>> {
        match self.tables.linear_search(TableId::PropertyMap, 0, type_def) {
            Some(map) => self.owned_list(
                TableId::PropertyMap,
                map,
                1,
                TableId::Property,
                TableId::PropertyPtr,
            ),
            None => Ok(Vec::new()),
        }
    }

    /// InterfaceImpl rows of a type.
    pub fn type_interface_impls(&self, type_def: u32) -> std::ops::Range<u32> {
        self.tables
            .binary_search_range(TableId::InterfaceImpl, 0, type_def)
    }

    /// The enclosing TypeDef row of a nested type, 0 if none.
    pub fn declaring_type(&self, type_def: u32) -> Result<u32> {
        match self.tables.binary_search(TableId::NestedClass, 0, type_def) {
            Some(row) => Ok(self.get::<rows::NestedClass>(row)?.enclosing),
            None => Ok(0),
        }
    }

    /// `(packing size, class size)`, `(0, 0)` without a ClassLayout row.
    pub fn type_layout(&self, type_def: u32) -> Result<(u16, u32)> {
        match self.tables.binary_search(TableId::ClassLayout, 2, type_def) {
            Some(row) => {
                let layout = self.get::<rows::ClassLayout>(row)?;
                if layout.class_size > i32::MAX as u32 {
                    return Err(Error::Malformed("type size out of range"));
                }
                Ok((layout.packing_size, layout.class_size))
            }
            None => Ok((0, 0)),
        }
    }

    /// The explicit field offset, -1 without a FieldLayout row.
    pub fn field_offset(&self, field: u32) -> Result<i64> {
        match self.tables.binary_search(TableId::FieldLayout, 1, field) {
            Some(row) => {
                let offset = self.get::<rows::FieldLayout>(row)?.offset;
                Ok(if offset > i32::MAX as u32 {
                    -1
                } else {
                    i64::from(offset)
                })
            }
            None => Ok(-1),
        }
    }

    pub fn field_rva(&self, field: u32) -> Result<u32> {
        match self.tables.binary_search(TableId::FieldRva, 1, field) {
            Some(row) => Ok(self.get::<rows::FieldRva>(row)?.rva),
            None => Ok(0),
        }
    }

    fn coded_parent(kind: CodedKind, table: TableId, row: u32) -> u32 {
        kind.encode(table, row)
            .expect("table is part of the coded index")
    }

    /// The marshalling descriptor of a Field or Param.
    pub fn marshalling(&self, table: TableId, row: u32) -> Result<&'a [u8]> {
        let key = Self::coded_parent(CodedKind::HasFieldMarshal, table, row);
        match self.tables.binary_search(TableId::FieldMarshal, 0, key) {
            Some(r) => self.blob(self.get::<rows::FieldMarshal>(r)?.native_type),
            None => Ok(&[]),
        }
    }

    /// The Constant row of a Field, Param or Property, 0 if none.
    pub fn default_value(&self, table: TableId, row: u32) -> u32 {
        let key = Self::coded_parent(CodedKind::HasConstant, table, row);
        self.tables
            .binary_search(TableId::Constant, 1, key)
            .unwrap_or(0)
    }

    /// The ImplMap row of a method, 0 if none.
    pub fn method_import(&self, method: u32) -> u32 {
        let key = Self::coded_parent(CodedKind::MemberForwarded, TableId::MethodDef, method);
        self.tables
            .binary_search(TableId::ImplMap, 1, key)
            .unwrap_or(0)
    }

    /// MethodSemantics rows of an Event or Property.
    pub fn semantics(&self, table: TableId, row: u32) -> std::ops::Range<u32> {
        let key = Self::coded_parent(CodedKind::HasSemantics, table, row);
        self.tables
            .binary_search_range(TableId::MethodSemantics, 2, key)
    }

    /// GenericParam rows of a TypeDef or MethodDef.
    pub fn generic_params(&self, table: TableId, row: u32) -> std::ops::Range<u32> {
        let key = Self::coded_parent(CodedKind::TypeOrMethodDef, table, row);
        self.tables
            .binary_search_range(TableId::GenericParam, 2, key)
    }

    /// GenericParamConstraint rows of a generic parameter.
    pub fn generic_param_constraints(&self, generic_param: u32) -> std::ops::Range<u32> {
        self.tables
            .binary_search_range(TableId::GenericParamConstraint, 0, generic_param)
    }
}
