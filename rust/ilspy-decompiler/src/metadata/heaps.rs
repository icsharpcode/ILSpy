//! The `#Strings`, `#Blob`, `#GUID` and `#US` heaps (ECMA-335 II.24.2.3-II.24.2.5).

use std::borrow::Cow;

use super::bytes::Bytes;
use super::{Error, Result};

/// An offset into `#Strings`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, Default)]
pub struct StrIdx(pub u32);

/// An offset into `#Blob`.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, Default)]
pub struct BlobIdx(pub u32);

/// A 1-based index into `#GUID`; 0 is the nil GUID.
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, Default)]
pub struct GuidIdx(pub u32);

#[derive(Clone, Copy, Debug, Default)]
pub struct StringHeap<'a>(&'a [u8]);

impl<'a> StringHeap<'a> {
    /// Trailing padding is cut down to one NUL, as System.Reflection.Metadata does, so the
    /// reported size matches it.
    pub fn new(data: &'a [u8]) -> Self {
        match data.iter().rposition(|&b| b != 0) {
            _ if data.is_empty() => Self(data),
            Some(last) if last + 1 == data.len() => Self(data),
            Some(last) => Self(&data[..last + 2]),
            None => Self(&data[..1]),
        }
    }

    pub fn len(&self) -> usize {
        self.0.len()
    }

    pub fn is_empty(&self) -> bool {
        self.0.is_empty()
    }

    /// The NUL-terminated UTF-8 string at `idx`; invalid UTF-8 is replaced, not rejected.
    pub fn get(&self, idx: StrIdx) -> Result<Cow<'a, str>> {
        let start = idx.0 as usize;
        if start == 0 && self.0.is_empty() {
            return Ok(Cow::Borrowed(""));
        }
        let rest = self
            .0
            .get(start..)
            .ok_or(Error::Malformed("string index out of range"))?;
        let end = rest.iter().position(|&b| b == 0).unwrap_or(rest.len());
        Ok(String::from_utf8_lossy(&rest[..end]))
    }
}

#[derive(Clone, Copy, Debug, Default)]
pub struct BlobHeap<'a>(&'a [u8]);

impl<'a> BlobHeap<'a> {
    pub fn new(data: &'a [u8]) -> Self {
        Self(data)
    }

    pub fn len(&self) -> usize {
        self.0.len()
    }

    pub fn is_empty(&self) -> bool {
        self.0.is_empty()
    }

    pub fn get(&self, idx: BlobIdx) -> Result<&'a [u8]> {
        if idx.0 == 0 {
            return Ok(&[]);
        }
        let mut b = Bytes::new(self.0);
        b.skip(idx.0 as usize)
            .map_err(|_| Error::Malformed("blob index out of range"))?;
        let len = b.compressed_u32()? as usize;
        b.take(len)
    }
}

#[derive(Clone, Copy, Debug, Default)]
pub struct GuidHeap<'a>(&'a [u8]);

impl<'a> GuidHeap<'a> {
    pub fn new(data: &'a [u8]) -> Self {
        Self(data)
    }

    pub fn len(&self) -> usize {
        self.0.len()
    }

    pub fn is_empty(&self) -> bool {
        self.0.is_empty()
    }

    pub fn get(&self, idx: GuidIdx) -> Result<[u8; 16]> {
        if idx.0 == 0 {
            return Ok([0; 16]);
        }
        let start = (idx.0 as usize - 1) * 16;
        self.0
            .get(start..start + 16)
            .map(|g| g.try_into().expect("16 bytes"))
            .ok_or(Error::Malformed("GUID index out of range"))
    }
}

/// `#US`: length-prefixed UTF-16 strings. Read by the IL reader for `ldstr`.
#[derive(Clone, Copy, Debug, Default)]
pub struct UserStringHeap<'a>(&'a [u8]);

impl<'a> UserStringHeap<'a> {
    pub fn new(data: &'a [u8]) -> Self {
        Self(data)
    }

    pub fn len(&self) -> usize {
        self.0.len()
    }

    pub fn is_empty(&self) -> bool {
        self.0.is_empty()
    }
}

/// Formats GUID bytes the way `System.Guid.ToString()` does.
pub fn format_guid(g: &[u8; 16]) -> String {
    format!(
        "{:08x}-{:04x}-{:04x}-{:02x}{:02x}-{:02x}{:02x}{:02x}{:02x}{:02x}{:02x}",
        u32::from_le_bytes([g[0], g[1], g[2], g[3]]),
        u16::from_le_bytes([g[4], g[5]]),
        u16::from_le_bytes([g[6], g[7]]),
        g[8],
        g[9],
        g[10],
        g[11],
        g[12],
        g[13],
        g[14],
        g[15]
    )
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn string_heap_trims_padding_to_one_nul() {
        assert_eq!(StringHeap::new(b"\0ab\0\0\0").len(), 4);
        assert_eq!(StringHeap::new(b"\0ab").len(), 3);
        assert_eq!(StringHeap::new(b"\0\0\0\0").len(), 1);
        let h = StringHeap::new(b"\0ab\0cd\0");
        assert_eq!(h.get(StrIdx(1)).unwrap(), "ab");
        assert_eq!(h.get(StrIdx(2)).unwrap(), "b");
        assert_eq!(h.get(StrIdx(0)).unwrap(), "");
    }

    #[test]
    fn guid_formatting_matches_dotnet() {
        let g = [
            0x33, 0x22, 0x11, 0x00, 0x55, 0x44, 0x77, 0x66, 0x88, 0x99, 0xaa, 0xbb, 0xcc, 0xdd,
            0xee, 0xff,
        ];
        assert_eq!(format_guid(&g), "00112233-4455-6677-8899-aabbccddeeff");
    }
}
