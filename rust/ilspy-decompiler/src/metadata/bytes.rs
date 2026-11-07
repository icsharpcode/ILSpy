use super::{Error, Result};

/// A little-endian cursor over a byte slice.
#[derive(Clone, Copy, Debug)]
pub struct Bytes<'a> {
    data: &'a [u8],
    pos: usize,
}

impl<'a> Bytes<'a> {
    pub fn new(data: &'a [u8]) -> Self {
        Self { data, pos: 0 }
    }

    pub fn pos(&self) -> usize {
        self.pos
    }

    pub fn remaining(&self) -> usize {
        self.data.len() - self.pos
    }

    pub fn is_empty(&self) -> bool {
        self.remaining() == 0
    }

    pub fn take(&mut self, n: usize) -> Result<&'a [u8]> {
        let end = self
            .pos
            .checked_add(n)
            .filter(|&end| end <= self.data.len())
            .ok_or(Error::Truncated)?;
        let slice = &self.data[self.pos..end];
        self.pos = end;
        Ok(slice)
    }

    pub fn skip(&mut self, n: usize) -> Result<()> {
        self.take(n).map(|_| ())
    }

    /// Advances to the next multiple of `n` (a power of two) from the start of the slice.
    pub fn align(&mut self, n: usize) -> Result<()> {
        let pad = self.pos.wrapping_neg() & (n - 1);
        self.skip(pad)
    }

    fn array<const N: usize>(&mut self) -> Result<[u8; N]> {
        Ok(self.take(N)?.try_into().expect("take returned N bytes"))
    }

    pub fn u8(&mut self) -> Result<u8> {
        Ok(self.array::<1>()?[0])
    }

    pub fn u16(&mut self) -> Result<u16> {
        self.array().map(u16::from_le_bytes)
    }

    pub fn u32(&mut self) -> Result<u32> {
        self.array().map(u32::from_le_bytes)
    }

    pub fn u64(&mut self) -> Result<u64> {
        self.array().map(u64::from_le_bytes)
    }

    /// ECMA-335 II.23.2 unsigned compressed integer; returns the value and its encoded length.
    fn compressed(&mut self) -> Result<(u32, usize)> {
        let b0 = u32::from(self.u8()?);
        let (value, len) = if b0 & 0x80 == 0 {
            (b0, 1)
        } else if b0 & 0xC0 == 0x80 {
            ((b0 & 0x3F) << 8 | u32::from(self.u8()?), 2)
        } else if b0 & 0xE0 == 0xC0 {
            let rest = self.take(3)?;
            let value = (b0 & 0x1F) << 24
                | u32::from(rest[0]) << 16
                | u32::from(rest[1]) << 8
                | u32::from(rest[2]);
            (value, 4)
        } else {
            return Err(Error::Malformed("invalid compressed integer"));
        };
        Ok((value, len))
    }

    pub fn compressed_u32(&mut self) -> Result<u32> {
        self.compressed().map(|(value, _)| value)
    }

    /// ECMA-335 II.23.2 signed compressed integer (rotated sign bit).
    pub fn compressed_i32(&mut self) -> Result<i32> {
        let (raw, len) = self.compressed()?;
        let negative = raw & 1 != 0;
        let value = (raw >> 1) as i32;
        Ok(if !negative {
            value
        } else {
            value
                | match len {
                    1 => -0x40,
                    2 => -0x2000,
                    _ => -0x1000_0000,
                }
        })
    }
}

/// Reads a little-endian `u16` at `offset`.
pub fn u16_at(data: &[u8], offset: usize) -> Result<u16> {
    let mut b = Bytes::new(data);
    b.skip(offset)?;
    b.u16()
}

/// Reads a little-endian `u32` at `offset`.
pub fn u32_at(data: &[u8], offset: usize) -> Result<u32> {
    let mut b = Bytes::new(data);
    b.skip(offset)?;
    b.u32()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn compressed_integers_follow_ecma_examples() {
        // II.23.2 examples.
        for (bytes, value) in [
            (&[0x03][..], 3),
            (&[0x7F], 0x7F),
            (&[0x80, 0x80], 0x80),
            (&[0xAE, 0x57], 0x2E57),
            (&[0xBF, 0xFF], 0x3FFF),
            (&[0xC0, 0x00, 0x40, 0x00], 0x4000),
            (&[0xDF, 0xFF, 0xFF, 0xFF], 0x1FFF_FFFF),
        ] {
            assert_eq!(Bytes::new(bytes).compressed_u32(), Ok(value));
        }
        for (bytes, value) in [
            (&[0x06][..], 3),
            (&[0x7B], -3),
            (&[0x80, 0x80], 64),
            (&[0x01], -64),
            (&[0xC0, 0x00, 0x40, 0x00], 8192),
            (&[0x80, 0x01], -8192),
            (&[0xDF, 0xFF, 0xFF, 0xFE], 268_435_455),
            (&[0xC0, 0x00, 0x00, 0x01], -268_435_456),
        ] {
            assert_eq!(Bytes::new(bytes).compressed_i32(), Ok(value));
        }
        assert!(Bytes::new(&[0xE0]).compressed_u32().is_err());
        assert_eq!(Bytes::new(&[0x80]).compressed_u32(), Err(Error::Truncated));
    }
}
