//! CIL method bodies (ECMA-335 II.25.4).

use super::bytes::Bytes;
use super::{Error, Result};

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub struct ExceptionClause {
    /// `0` catch, `1` filter, `2` finally, `4` fault.
    pub kind: u32,
    pub try_offset: u32,
    pub try_length: u32,
    pub handler_offset: u32,
    pub handler_length: u32,
    /// Class token for catch clauses, filter offset for filter clauses.
    pub class_token_or_filter: u32,
}

#[derive(Clone, Debug)]
pub struct MethodBody<'a> {
    pub max_stack: u16,
    /// StandAloneSig row of the locals signature, 0 for none.
    pub local_sig: u32,
    pub init_locals: bool,
    pub code: &'a [u8],
    pub clauses: Vec<ExceptionClause>,
    /// Bytes taken by header, code and exception sections.
    pub size: usize,
}

impl<'a> MethodBody<'a> {
    /// Parses the body starting at `data[0]`. Like System.Reflection.Metadata, only the first
    /// extra data section is read.
    pub fn parse(data: &'a [u8]) -> Result<Self> {
        let mut b = Bytes::new(data);
        let first = b.u8()?;
        if first & 3 == 2 {
            let code = b.take(usize::from(first >> 2))?;
            return Ok(MethodBody {
                max_stack: 8,
                local_sig: 0,
                init_locals: false,
                code,
                clauses: Vec::new(),
                size: b.pos(),
            });
        }
        if first & 3 != 3 {
            return Err(Error::Malformed("invalid method header"));
        }
        let second = b.u8()?;
        if second >> 4 != 3 {
            return Err(Error::Malformed("invalid fat method header size"));
        }
        let init_locals = first & 0x10 != 0;
        let more_sections = first & 0x08 != 0;
        let max_stack = b.u16()?;
        let code_size = b.u32()?;
        let local_token = b.u32()?;
        let local_sig = match local_token {
            0 => 0,
            t if t & 0x7F00_0000 == 0x1100_0000 => t & 0x00FF_FFFF,
            _ => return Err(Error::Malformed("invalid local signature token")),
        };
        if code_size > i32::MAX as u32 {
            return Err(Error::Malformed("negative code size"));
        }
        let code = b.take(code_size as usize)?;
        let mut clauses = Vec::new();
        if more_sections {
            b.align(4)?;
            let kind = b.u8()?;
            if kind & 1 != 1 {
                return Err(Error::Malformed("invalid exception section header"));
            }
            let mut data_size = usize::from(b.u8()?);
            if kind & 0x40 != 0 {
                data_size += usize::from(b.u16()?) << 8;
                for _ in 0..data_size / 24 {
                    clauses.push(ExceptionClause {
                        kind: b.u32()?,
                        try_offset: b.u32()?,
                        try_length: b.u32()?,
                        handler_offset: b.u32()?,
                        handler_length: b.u32()?,
                        class_token_or_filter: b.u32()?,
                    });
                }
            } else {
                b.skip(2)?;
                for _ in 0..data_size / 12 {
                    clauses.push(ExceptionClause {
                        kind: u32::from(b.u16()?),
                        try_offset: u32::from(b.u16()?),
                        try_length: u32::from(b.u8()?),
                        handler_offset: u32::from(b.u16()?),
                        handler_length: u32::from(b.u8()?),
                        class_token_or_filter: b.u32()?,
                    });
                }
            }
        }
        Ok(MethodBody {
            max_stack,
            local_sig,
            init_locals,
            code,
            clauses,
            size: b.pos(),
        })
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn tiny_body() {
        let body = MethodBody::parse(&[0x0A, 0x02, 0x2A, 0xFF]).unwrap();
        assert_eq!(body.code, &[0x02, 0x2A]);
        assert_eq!(body.max_stack, 8);
        assert_eq!(body.size, 3);
    }

    #[test]
    fn fat_body_with_small_clauses() {
        let mut data = vec![0x1B, 0x30, 0x02, 0x00, 0x04, 0, 0, 0, 0x01, 0, 0, 0x11];
        data.extend_from_slice(&[0x00, 0x00, 0x00, 0x2A]);
        // small EH section: 1 clause (12 bytes) + 4-byte header
        data.extend_from_slice(&[0x01, 16, 0, 0]);
        data.extend_from_slice(&[0x02, 0x00, 0x00, 0x00, 0x02, 0x02, 0x00, 0x01, 0, 0, 0, 0]);
        let body = MethodBody::parse(&data).unwrap();
        assert!(body.init_locals);
        assert_eq!(body.local_sig, 1);
        assert_eq!(body.clauses.len(), 1);
        assert_eq!(body.clauses[0].kind, 2);
        assert_eq!(body.size, data.len());
    }
}
