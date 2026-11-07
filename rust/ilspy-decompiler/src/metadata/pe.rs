//! The minimal PE/COFF reading needed to find the CLI header and resolve RVAs in file layout.

use super::bytes::{Bytes, u32_at};
use super::{Error, Result};

#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct DataDirectory {
    pub rva: u32,
    pub size: u32,
}

#[derive(Clone, Debug)]
pub struct Section {
    pub name: [u8; 8],
    pub virtual_size: u32,
    pub virtual_address: u32,
    pub raw_size: u32,
    pub raw_pointer: u32,
}

/// ECMA-335 II.25.3.3.
#[derive(Clone, Debug)]
pub struct CliHeader {
    pub major_runtime_version: u16,
    pub minor_runtime_version: u16,
    pub metadata: DataDirectory,
    pub flags: u32,
    pub entry_point: u32,
    pub resources: DataDirectory,
    pub strong_name_signature: DataDirectory,
}

#[derive(Clone, Debug)]
pub struct PeImage<'a> {
    pub data: &'a [u8],
    pub machine: u16,
    pub characteristics: u16,
    pub is_pe32_plus: bool,
    pub sections: Vec<Section>,
    pub cli: CliHeader,
}

const CLI_HEADER_DIRECTORY: usize = 14;

fn directory(b: &mut Bytes<'_>) -> Result<DataDirectory> {
    Ok(DataDirectory {
        rva: b.u32()?,
        size: b.u32()?,
    })
}

impl<'a> PeImage<'a> {
    pub fn parse(data: &'a [u8]) -> Result<Self> {
        if data.len() < 0x40 || &data[..2] != b"MZ" {
            return Err(Error::NotPe("missing MZ signature"));
        }
        let pe_offset = u32_at(data, 0x3C)? as usize;
        let mut b = Bytes::new(data);
        b.skip(pe_offset)
            .map_err(|_| Error::NotPe("PE header offset out of range"))?;
        if b.take(4).map_err(|_| Error::NotPe("truncated"))? != b"PE\0\0" {
            return Err(Error::NotPe("missing PE signature"));
        }
        let machine = b.u16()?;
        let section_count = b.u16()?;
        b.skip(12)?; // time stamp, symbol table pointer, symbol count
        let optional_size = usize::from(b.u16()?);
        let characteristics = b.u16()?;
        let optional_start = b.pos();
        let is_pe32_plus = match b.u16()? {
            0x10B => false,
            0x20B => true,
            _ => return Err(Error::NotPe("unknown optional header magic")),
        };
        let (count_offset, dirs_offset) = if is_pe32_plus { (108, 112) } else { (92, 96) };
        let dir_count = if optional_size >= count_offset + 4 {
            u32_at(data, optional_start + count_offset)? as usize
        } else {
            0
        };
        let cli_dir = if dir_count > CLI_HEADER_DIRECTORY
            && optional_size >= dirs_offset + (CLI_HEADER_DIRECTORY + 1) * 8
        {
            let mut d = Bytes::new(data);
            d.skip(optional_start + dirs_offset + CLI_HEADER_DIRECTORY * 8)?;
            directory(&mut d)?
        } else {
            DataDirectory::default()
        };

        let mut s = Bytes::new(data);
        s.skip(optional_start + optional_size)?;
        let sections = (0..section_count)
            .map(|_| {
                let name = s.take(8)?.try_into().expect("8 bytes");
                let section = Section {
                    name,
                    virtual_size: s.u32()?,
                    virtual_address: s.u32()?,
                    raw_size: s.u32()?,
                    raw_pointer: s.u32()?,
                };
                s.skip(16)?;
                Ok(section)
            })
            .collect::<Result<Vec<_>>>()?;

        let mut image = PeImage {
            data,
            machine,
            characteristics,
            is_pe32_plus,
            sections,
            cli: CliHeader {
                major_runtime_version: 0,
                minor_runtime_version: 0,
                metadata: DataDirectory::default(),
                flags: 0,
                entry_point: 0,
                resources: DataDirectory::default(),
                strong_name_signature: DataDirectory::default(),
            },
        };
        let cli_offset = image.directory_offset(cli_dir).ok_or(Error::NotManaged)?;
        if cli_dir.size < 72 {
            return Err(Error::Malformed("CLI header too small"));
        }
        let mut c = Bytes::new(data);
        c.skip(cli_offset)?;
        c.skip(4)?; // cb
        image.cli = CliHeader {
            major_runtime_version: c.u16()?,
            minor_runtime_version: c.u16()?,
            metadata: directory(&mut c)?,
            flags: c.u32()?,
            entry_point: c.u32()?,
            resources: directory(&mut c)?,
            strong_name_signature: directory(&mut c)?,
        };
        if image.cli.metadata.size == 0 {
            return Err(Error::NotManaged);
        }
        Ok(image)
    }

    fn containing_section(&self, rva: u32) -> Option<&Section> {
        self.sections.iter().find(|s| {
            s.virtual_address <= rva
                && u64::from(rva) < u64::from(s.virtual_address) + u64::from(s.virtual_size)
        })
    }

    /// The file offset of a directory, or `None` if no section contains it.
    fn directory_offset(&self, dir: DataDirectory) -> Option<usize> {
        let section = self.containing_section(dir.rva)?;
        Some((section.raw_pointer + (dir.rva - section.virtual_address)) as usize)
    }

    /// The bytes of a directory; it must lie inside the file.
    pub fn directory_data(&self, dir: DataDirectory) -> Result<&'a [u8]> {
        let offset = self
            .directory_offset(dir)
            .ok_or(Error::Malformed("directory outside all sections"))?;
        let mut b = Bytes::new(self.data);
        b.skip(offset)?;
        b.take(dir.size as usize)
    }

    /// The section data from `rva` to the end of its section's initialized data: the region a
    /// method body may occupy. Empty when no section contains `rva`.
    pub fn section_data(&self, rva: u32) -> Result<&'a [u8]> {
        let Some(section) = self.containing_section(rva) else {
            return Ok(&[]);
        };
        let size = section.virtual_size.min(section.raw_size) as usize;
        let start = section.raw_pointer as usize;
        let block = self
            .data
            .get(start..start.checked_add(size).ok_or(Error::Truncated)?)
            .ok_or(Error::Truncated)?;
        let relative = (rva - section.virtual_address) as usize;
        Ok(block.get(relative..).unwrap_or(&[]))
    }
}
