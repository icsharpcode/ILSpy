//! IL opcode decoding (`Disassembler/ILParser.cs`, `Metadata/ILOpCodes.cs`) over a byte
//! cursor that, like SRM's `BlobReader`, fails on reads past the end.

use crate::metadata::{Error, Result};
use crate::util::bitset::BitSet;

/// `OperandType` values (`Metadata/OperandType.cs`); 255 is "no such opcode".
pub mod operand {
    pub const BR_TARGET: u8 = 0;
    pub const FIELD: u8 = 1;
    pub const I: u8 = 2;
    pub const I8: u8 = 3;
    pub const METHOD: u8 = 4;
    pub const NONE: u8 = 5;
    pub const R: u8 = 7;
    pub const SIG: u8 = 9;
    pub const STRING: u8 = 10;
    pub const SWITCH: u8 = 11;
    pub const TOK: u8 = 12;
    pub const TYPE: u8 = 13;
    pub const VARIABLE: u8 = 14;
    pub const SHORT_BR_TARGET: u8 = 15;
    pub const SHORT_I: u8 = 16;
    pub const SHORT_R: u8 = 17;
    pub const SHORT_VARIABLE: u8 = 18;
}

/// `ILOpCodes.operandTypes`, indexed by `((op & 0x200) >> 1) | (op & 0xff)`.
#[rustfmt::skip]
static OPERAND_TYPES: [u8; 287] = [
    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 18, 18, 18, 18, 18, 18, 5, 5, 5, 5,
    5, 5, 5, 5, 5, 5, 5, 16, 2, 3, 17, 7, 255, 5, 5, 4, 4, 9, 5, 15, 15, 15, 15, 15,
    15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 5, 5,
    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 13, 13, 10, 4, 13, 13, 5, 255,
    255, 13, 5, 1, 1, 1, 1, 1, 1, 13, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 13, 13, 5, 13,
    5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 13, 13, 13, 255, 255,
    255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 5, 5, 5, 5, 5, 5, 5, 5, 255, 255, 255, 255, 255,
    255, 255, 13, 5, 255, 255, 13, 255, 255, 255, 255, 255, 255, 255, 255, 255, 12, 5, 5, 5, 5, 5, 5, 5,
    5, 5, 5, 5, 5, 0, 15, 5, 5, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
    255, 255, 255, 255, 255, 255, 255, 255, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 4,
    255, 14, 14, 14, 14, 14, 14, 5, 255, 5, 16, 5, 5, 13, 13, 5, 5, 255, 5, 255, 13, 5, 5,
];

/// `ILOpCodeExtensions.GetOperandType`.
pub fn operand_type(op: u16) -> u8 {
    let index = (((op & 0x200) >> 1) | (op & 0xff)) as usize;
    OPERAND_TYPES.get(index).copied().unwrap_or(255)
}

/// `OperandSize`.
fn operand_size(ty: u8) -> usize {
    match ty {
        operand::I8 | operand::R => 8,
        operand::BR_TARGET
        | operand::FIELD
        | operand::METHOD
        | operand::I
        | operand::SIG
        | operand::STRING
        | operand::TOK
        | operand::TYPE
        | operand::SHORT_R
        | operand::SWITCH => 4,
        operand::VARIABLE => 2,
        operand::SHORT_VARIABLE | operand::SHORT_BR_TARGET | operand::SHORT_I => 1,
        _ => 0,
    }
}

/// SRM `ILOpCodeExtensions.IsBranch`: the conditional and unconditional branches and `leave`.
pub fn is_branch(op: u16) -> bool {
    (0x2B..=0x44).contains(&op) || op == 0xDD || op == 0xDE
}

/// SRM `GetBranchOperandSize`.
fn branch_operand_size(op: u16) -> usize {
    if (0x2B..=0x37).contains(&op) || op == 0xDE {
        1
    } else {
        4
    }
}

/// A cursor over IL code.
#[derive(Clone, Debug)]
pub struct Code<'a> {
    pub bytes: &'a [u8],
    pub offset: usize,
}

impl<'a> Code<'a> {
    pub fn new(bytes: &'a [u8]) -> Self {
        Code { bytes, offset: 0 }
    }

    pub fn len(&self) -> usize {
        self.bytes.len()
    }

    pub fn is_empty(&self) -> bool {
        self.bytes.is_empty()
    }

    pub fn remaining(&self) -> usize {
        self.bytes.len().saturating_sub(self.offset)
    }

    fn take<const N: usize>(&mut self) -> Result<[u8; N]> {
        let slice = self
            .bytes
            .get(self.offset..self.offset + N)
            .ok_or(Error::Malformed("Out of bounds"))?;
        self.offset += N;
        Ok(slice.try_into().expect("slice of N bytes"))
    }

    pub fn u8(&mut self) -> Result<u8> {
        Ok(self.take::<1>()?[0])
    }

    pub fn i8(&mut self) -> Result<i8> {
        Ok(self.u8()? as i8)
    }

    pub fn u16(&mut self) -> Result<u16> {
        Ok(u16::from_le_bytes(self.take()?))
    }

    pub fn i32(&mut self) -> Result<i32> {
        Ok(i32::from_le_bytes(self.take()?))
    }

    pub fn u32(&mut self) -> Result<u32> {
        Ok(u32::from_le_bytes(self.take()?))
    }

    pub fn i64(&mut self) -> Result<i64> {
        Ok(i64::from_le_bytes(self.take()?))
    }

    pub fn u64(&mut self) -> Result<u64> {
        Ok(u64::from_le_bytes(self.take()?))
    }

    /// `DecodeOpCode`.
    pub fn opcode(&mut self) -> Result<u16> {
        let first = self.u8()?;
        if first == 0xFE && self.remaining() >= 1 {
            Ok(0xFE00 + u16::from(self.u8()?))
        } else {
            Ok(u16::from(first))
        }
    }

    /// `SkipOperand`.
    pub fn skip_operand(&mut self, op: u16) {
        let ty = operand_type(op);
        let size = if ty == operand::SWITCH {
            let n = if self.remaining() >= 4 {
                self.u32().unwrap_or(u32::MAX)
            } else {
                u32::MAX
            };
            if n < (i32::MAX / 4) as u32 {
                n as usize * 4
            } else {
                i32::MAX as usize
            }
        } else {
            operand_size(ty)
        };
        if size <= self.remaining() {
            self.offset += size;
        } else {
            self.offset = self.bytes.len();
        }
    }

    /// `DecodeBranchTarget`: the target offset, `i32::MIN` when the operand is cut off.
    pub fn branch_target(&mut self, op: u16) -> Result<i32> {
        let size = branch_operand_size(op);
        if size > self.remaining() {
            return Ok(i32::MIN);
        }
        let relative = if size == 4 {
            self.i32()?
        } else {
            i32::from(self.i8()?)
        };
        Ok(relative.wrapping_add(self.offset as i32))
    }

    /// `DecodeSwitchTargets`.
    pub fn switch_targets(&mut self) -> Result<Vec<i32>> {
        if self.remaining() < 4 {
            self.offset = self.bytes.len();
            return Ok(Vec::new());
        }
        let mut count = self.u32()?;
        let overflow = count as usize > self.remaining() / 4;
        if overflow {
            count = (self.remaining() / 4) as u32;
        }
        let base = (self.offset + 4 * count as usize) as i32;
        let mut targets = Vec::with_capacity(count as usize);
        for _ in 0..count {
            targets.push(self.i32()?.wrapping_add(base));
        }
        if overflow {
            self.offset = self.bytes.len();
        }
        Ok(targets)
    }

    /// `DecodeIndex`.
    pub fn index(&mut self, op: u16) -> Result<i32> {
        match operand_type(op) {
            operand::SHORT_VARIABLE => Ok(i32::from(self.u8()?)),
            operand::VARIABLE => Ok(i32::from(self.u16()?)),
            _ => Err(Error::Malformed("opcode has no index operand")),
        }
    }
}

/// `ILParser.SetBranchTargets` over the whole body.
pub fn set_branch_targets(code: &[u8], targets: &mut BitSet) {
    let mut c = Code::new(code);
    let len = code.len() as i32;
    while c.remaining() > 0 {
        let Ok(op) = c.opcode() else {
            break;
        };
        if op == 0x45 {
            if let Ok(ts) = c.switch_targets() {
                for t in ts {
                    if (0..len).contains(&t) {
                        targets.set(t as usize);
                    }
                }
            }
        } else if is_branch(op) {
            if let Ok(t) = c.branch_target(op)
                && (0..len).contains(&t)
            {
                targets.set(t as usize);
            }
        } else {
            c.skip_operand(op);
        }
    }
}
