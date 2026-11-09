//! Wire v1: the ILAst functions of the IL reader (`Interop/IlAstMaterializer.cs` reads it).
//!
//! Payload: a function count, then per method `row`, a status byte (0 read, 1 failed with a
//! message) and for a read function its header, variables, warnings, sequence point
//! candidates, the block table and the body. Blocks and containers are numbered in pre-order
//! of the body; branches name blocks by number, so the table also lists blocks that are no
//! longer in the tree but still targeted.

use std::collections::HashMap;

use crate::il::ast::*;
use crate::il::reader::ReadFunction;
use crate::il::types::StackType;
use crate::typesystem::types::{TypeId, TypeSystem};

use super::{Bytes, Tables};

pub const VERSION: u32 = 1;

/// Node tags; `IlAstMaterializer.cs` mirrors them.
mod op {
    pub const INVALID_BRANCH: u8 = 0;
    pub const INVALID_EXPRESSION: u8 = 1;
    pub const NOP: u8 = 2;
    pub const BLOCK_CONTAINER: u8 = 3;
    pub const BLOCK: u8 = 4;
    pub const BINARY_NUMERIC: u8 = 5;
    pub const BIT_NOT: u8 = 6;
    pub const ARGLIST: u8 = 7;
    pub const BRANCH: u8 = 8;
    pub const LEAVE: u8 = 9;
    pub const IF: u8 = 10;
    pub const SWITCH: u8 = 11;
    pub const SWITCH_SECTION: u8 = 12;
    pub const TRY_CATCH: u8 = 13;
    pub const TRY_CATCH_HANDLER: u8 = 14;
    pub const TRY_FINALLY: u8 = 15;
    pub const TRY_FAULT: u8 = 16;
    pub const DEBUG_BREAK: u8 = 17;
    pub const COMP: u8 = 18;
    pub const CALL: u8 = 19;
    pub const CALL_VIRT: u8 = 20;
    pub const NEW_OBJ: u8 = 21;
    pub const CALL_INDIRECT: u8 = 22;
    pub const CKFINITE: u8 = 23;
    pub const CONV: u8 = 24;
    pub const LD_LOC: u8 = 25;
    pub const LD_LOCA: u8 = 26;
    pub const ST_LOC: u8 = 27;
    pub const ADDRESS_OF: u8 = 28;
    pub const LD_STR: u8 = 29;
    pub const LDC_I4: u8 = 30;
    pub const LDC_I8: u8 = 31;
    pub const LDC_F4: u8 = 32;
    pub const LDC_F8: u8 = 33;
    pub const LD_NULL: u8 = 34;
    pub const LD_FTN: u8 = 35;
    pub const LD_VIRT_FTN: u8 = 36;
    pub const LD_TYPE_TOKEN: u8 = 37;
    pub const LD_MEMBER_TOKEN: u8 = 38;
    pub const LOC_ALLOC: u8 = 39;
    pub const CPBLK: u8 = 40;
    pub const INITBLK: u8 = 41;
    pub const LD_FLDA: u8 = 42;
    pub const LDS_FLDA: u8 = 43;
    pub const CAST_CLASS: u8 = 44;
    pub const IS_INST: u8 = 45;
    pub const LD_OBJ: u8 = 46;
    pub const LD_OBJ_IF_REF: u8 = 47;
    pub const ST_OBJ: u8 = 48;
    pub const BOX: u8 = 49;
    pub const UNBOX: u8 = 50;
    pub const UNBOX_ANY: u8 = 51;
    pub const NEW_ARR: u8 = 52;
    pub const DEFAULT_VALUE: u8 = 53;
    pub const THROW: u8 = 54;
    pub const RETHROW: u8 = 55;
    pub const SIZE_OF: u8 = 56;
    pub const LD_LEN: u8 = 57;
    pub const LD_ELEMA: u8 = 58;
    pub const MAKE_REF_ANY: u8 = 59;
    pub const REF_ANY_TYPE: u8 = 60;
    pub const REF_ANY_VALUE: u8 = 61;
    pub const PINNED_REGION: u8 = 62;
    pub const GET_PINNABLE_REFERENCE: u8 = 63;
    pub const YIELD_RETURN: u8 = 64;
    pub const AWAIT: u8 = 65;
    pub const DYNAMIC: u8 = 66;
    pub const LDC_DECIMAL: u8 = 67;
    pub const STRING_TO_INT: u8 = 68;
    pub const MATCH: u8 = 69;
    /// A node already written (it has two parents).
    pub const NODE_REFERENCE: u8 = 255;
}

/// Writes every function of one module.
pub struct ModuleWriter<'t, 'c, 'a> {
    ts: &'t TypeSystem<'c, 'a>,
    tables: Tables,
    payload: Bytes,
    count: usize,
    functions: Bytes,
}

impl<'t, 'c, 'a> ModuleWriter<'t, 'c, 'a> {
    pub fn new(ts: &'t TypeSystem<'c, 'a>) -> Self {
        ModuleWriter {
            ts,
            tables: Tables::default(),
            payload: Bytes::default(),
            count: 0,
            functions: Bytes::default(),
        }
    }

    pub fn failed(&mut self, row: u32, message: &str) {
        self.count += 1;
        self.functions.uvar(u64::from(row));
        self.functions.u8(1);
        let s = self.tables.string(message);
        self.functions.uvar(u64::from(s));
    }

    pub fn function(&mut self, row: u32, r: &ReadFunction) -> Result<(), String> {
        let mut w = FunctionWriter {
            ts: self.ts,
            tables: &mut self.tables,
            f: &r.function,
            out: Bytes::default(),
            vars: r
                .variables
                .iter()
                .enumerate()
                .map(|(i, &v)| (v, i))
                .collect(),
            blocks: HashMap::new(),
            block_order: Vec::new(),
            containers: HashMap::new(),
            written: HashMap::new(),
        };
        w.write(r)?;
        self.count += 1;
        self.functions.uvar(u64::from(row));
        self.functions.u8(0);
        self.functions.raw(&w.out.0);
        Ok(())
    }

    pub fn finish(mut self) -> Vec<u8> {
        self.payload.len(self.count);
        self.payload.raw(&self.functions.0);
        self.tables.finish(VERSION, self.payload)
    }
}

struct FunctionWriter<'w, 't, 'c, 'a> {
    ts: &'t TypeSystem<'c, 'a>,
    tables: &'w mut Tables,
    f: &'w Function,
    out: Bytes,
    vars: HashMap<VarId, usize>,
    blocks: HashMap<NodeId, usize>,
    block_order: Vec<NodeId>,
    containers: HashMap<NodeId, usize>,
    /// The nodes written so far, numbered in completion order; a node with two parents is
    /// written once and then referenced (`op::NODE_REFERENCE`).
    written: HashMap<NodeId, usize>,
}

impl FunctionWriter<'_, '_, '_, '_> {
    fn ty(&mut self, t: TypeId) {
        let i = self.tables.ty(self.ts, t);
        self.out.uvar(u64::from(i));
    }

    /// A member token, shifted left by one; bit 0 says the host resolves it in the state
    /// machine context (the function's class and method type parameters as class parameters).
    fn member(&mut self, token: u32, state_machine_context: bool) {
        self.out
            .uvar(u64::from(token) << 1 | u64::from(state_machine_context));
    }

    /// An optional type as its index + 1, 0 for none.
    fn opt_ty(&mut self, t: Option<TypeId>) {
        match t {
            Some(t) => {
                let i = self.tables.ty(self.ts, t);
                self.out.uvar(u64::from(i) + 1);
            }
            None => self.out.uvar(0),
        }
    }

    fn string(&mut self, s: &str) {
        let i = self.tables.string(s);
        self.out.uvar(u64::from(i));
    }

    fn var(&mut self, v: VarId) -> Result<(), String> {
        let i = *self
            .vars
            .get(&v)
            .ok_or("instruction uses a variable outside the function")?;
        self.out.len(i);
        Ok(())
    }

    fn stack_type(&mut self, st: StackType) {
        self.out.u8(st as u8);
    }

    fn block_number(&mut self, b: NodeId) -> usize {
        let next = self.blocks.len();
        *self.blocks.entry(b).or_insert_with(|| {
            self.block_order.push(b);
            next
        })
    }

    /// Numbers the blocks and containers of the tree in pre-order, then the blocks that are
    /// only reachable as branch targets.
    fn number(&mut self, root: NodeId) {
        let mut stack = vec![root];
        let mut targets = Vec::new();
        // A node with two parents is written once.
        let mut seen = std::collections::HashSet::new();
        while let Some(id) = stack.pop() {
            if !seen.insert(id) {
                continue;
            }
            match self.f.inst(id) {
                Inst::Block { .. } => {
                    self.block_number(id);
                }
                Inst::BlockContainer { .. } => {
                    let next = self.containers.len();
                    self.containers.insert(id, next);
                }
                Inst::Branch {
                    target: BranchTarget::Block(t),
                } => targets.push(*t),
                _ => {}
            }
            stack.extend(self.f.inst(id).children().into_iter().rev());
        }
        for t in targets {
            self.block_number(t);
        }
    }

    fn write(&mut self, r: &ReadFunction) -> Result<(), String> {
        let body = r.function.body.ok_or("function without body")?;
        self.out.len(r.code_size as usize);
        self.out.len(r.local_signature_length);
        match r.async_return_type {
            Some(t) => {
                let i = self.tables.ty(self.ts, t);
                self.out.uvar(u64::from(i) + 1);
            }
            None => self.out.u8(0),
        }
        self.out.uvar(u64::from(r.helper_variable_count));
        self.out.u8(u8::from(r.is_iterator)
            | u8::from(r.state_machine_compiled_with_mono) << 1
            | u8::from(r.state_machine_compiled_with_legacy_visual_basic) << 2);
        self.out.bool(r.move_next_method.is_some());
        if let Some(m) = &r.move_next_method {
            self.member(m.token, m.state_machine_context);
        }
        self.out.bool(r.async_debug_info.is_some());
        if let Some((catch_handler, awaits)) = &r.async_debug_info {
            self.out.ivar(i64::from(*catch_handler));
            self.out.len(awaits.len());
            for &(yield_offset, resume_offset) in awaits {
                self.out.ivar(i64::from(yield_offset));
                self.out.ivar(i64::from(resume_offset));
            }
        }
        self.out.len(r.variables.len());
        for &v in &r.variables {
            let var = self.f.var(v);
            self.out.u8(var.kind as u8);
            self.ty(var.ty);
            self.stack_type(var.stack_type);
            match var.index {
                Some(i) => {
                    self.out.u8(1);
                    self.out.ivar(i64::from(i));
                }
                None => self.out.u8(0),
            }
            self.string(&var.name);
            self.out.u8(u8::from(var.has_generated_name)
                | u8::from(var.initial_value_is_initialized) << 1
                | u8::from(var.uses_initial_value) << 2
                | u8::from(var.is_ref_read_only) << 3
                | u8::from(var.remove_if_redundant) << 4);
            self.out.bool(var.state_machine_field.is_some());
            if let Some(field) = &var.state_machine_field {
                let (token, state_machine) = (field.token, field.state_machine_context);
                self.member(token, state_machine);
            }
        }
        self.out.len(r.warnings.len());
        for w in &r.warnings {
            self.string(w);
        }
        self.out.len(r.sequence_point_candidates.len());
        for &s in &r.sequence_point_candidates {
            self.out.ivar(i64::from(s));
        }
        self.number(body);
        self.out.len(self.block_order.len());
        for &b in &self.block_order.clone() {
            let Inst::Block { kind, .. } = self.f.inst(b) else {
                return Err("branch target is not a block".to_owned());
            };
            self.out.u8(*kind as u8);
            let range = self.f.il_range(b);
            self.out.ivar(i64::from(range.start));
            self.out.ivar(i64::from(range.end));
        }
        self.out.len(self.containers.len());
        self.node(body)
    }

    fn nodes(&mut self, ids: &[NodeId]) -> Result<(), String> {
        self.out.len(ids.len());
        for &id in ids {
            self.node(id)?;
        }
        Ok(())
    }

    fn node(&mut self, id: NodeId) -> Result<(), String> {
        if let Some(&index) = self.written.get(&id) {
            self.out.u8(op::NODE_REFERENCE);
            self.out.len(index);
            return Ok(());
        }
        self.node_once(id)?;
        let index = self.written.len();
        self.written.insert(id, index);
        Ok(())
    }

    fn node_once(&mut self, id: NodeId) -> Result<(), String> {
        let range = self.f.il_range(id);
        let inst = self.f.inst(id).clone();
        let tag = |t: u8, out: &mut Bytes| {
            out.u8(t);
            out.ivar(i64::from(range.start));
            out.ivar(i64::from(range.end));
        };
        match inst {
            Inst::InvalidBranch { message, expected } => {
                tag(op::INVALID_BRANCH, &mut self.out);
                self.string(&message);
                self.stack_type(expected);
            }
            Inst::InvalidExpression {
                message,
                expected,
                severity,
            } => {
                tag(op::INVALID_EXPRESSION, &mut self.out);
                self.string(&message);
                self.stack_type(expected);
                self.string(&severity);
            }
            Inst::Nop { kind } => {
                tag(op::NOP, &mut self.out);
                self.out.u8(kind as u8);
            }
            Inst::BlockContainer {
                kind,
                expected,
                blocks,
            } => {
                tag(op::BLOCK_CONTAINER, &mut self.out);
                self.out.len(self.containers[&id]);
                self.out.u8(kind as u8);
                match expected {
                    Some(t) => {
                        let i = self.tables.ty(self.ts, t);
                        self.out.uvar(u64::from(i) + 1);
                    }
                    None => self.out.u8(0),
                }
                self.nodes(&blocks)?;
            }
            Inst::Block {
                instructions,
                final_inst,
                ..
            } => {
                tag(op::BLOCK, &mut self.out);
                self.out.len(self.blocks[&id]);
                self.nodes(&instructions)?;
                self.node(final_inst)?;
            }
            Inst::BinaryNumeric {
                op: o,
                check_overflow,
                sign,
                left_input,
                right_input,
                left,
                right,
            } => {
                tag(op::BINARY_NUMERIC, &mut self.out);
                self.out.u8(o as u8);
                self.out.bool(check_overflow);
                self.out.u8(sign as u8);
                self.stack_type(left_input);
                self.stack_type(right_input);
                self.node(left)?;
                self.node(right)?;
            }
            Inst::BitNot { arg } => {
                tag(op::BIT_NOT, &mut self.out);
                self.node(arg)?;
            }
            Inst::Arglist => tag(op::ARGLIST, &mut self.out),
            Inst::Branch { target } => {
                tag(op::BRANCH, &mut self.out);
                let BranchTarget::Block(b) = target else {
                    return Err("unconnected branch".to_owned());
                };
                let n = self.blocks[&b];
                self.out.len(n);
            }
            Inst::Leave { target, value } => {
                tag(op::LEAVE, &mut self.out);
                let c = target
                    .and_then(|t| self.containers.get(&t).copied())
                    .ok_or("leave without a container in the tree")?;
                self.out.len(c);
                self.node(value)?;
            }
            Inst::If {
                condition,
                true_inst,
                false_inst,
                csharp_type,
            } => {
                tag(op::IF, &mut self.out);
                self.opt_ty(csharp_type);
                self.node(condition)?;
                self.node(true_inst)?;
                self.node(false_inst)?;
            }
            Inst::Switch {
                value,
                sections,
                ty,
                is_lifted,
            } => {
                tag(op::SWITCH, &mut self.out);
                self.out.bool(is_lifted);
                // The type as its index + 1, 0 when unknown.
                match ty {
                    Some(t) => {
                        let i = self.tables.ty(self.ts, t);
                        self.out.uvar(u64::from(i) + 1);
                    }
                    None => self.out.uvar(0),
                }
                self.node(value)?;
                self.nodes(&sections)?;
            }
            Inst::SwitchSection {
                labels,
                body,
                has_null_label,
                is_compiler_generated_default_section,
            } => {
                tag(op::SWITCH_SECTION, &mut self.out);
                self.out
                    .u8(u8::from(has_null_label)
                        | u8::from(is_compiler_generated_default_section) << 1);
                let intervals = labels.intervals();
                self.out.len(intervals.len());
                for i in intervals {
                    self.out.ivar(i.start);
                    self.out.ivar(i.end);
                }
                self.node(body)?;
            }
            Inst::TryCatch {
                try_block,
                handlers,
            } => {
                tag(op::TRY_CATCH, &mut self.out);
                self.node(try_block)?;
                self.nodes(&handlers)?;
            }
            Inst::TryCatchHandler {
                filter,
                body,
                variable,
            } => {
                tag(op::TRY_CATCH_HANDLER, &mut self.out);
                self.var(variable)?;
                self.node(filter)?;
                self.node(body)?;
            }
            Inst::TryFinally {
                try_block,
                finally_block,
            } => {
                tag(op::TRY_FINALLY, &mut self.out);
                self.node(try_block)?;
                self.node(finally_block)?;
            }
            Inst::TryFault {
                try_block,
                fault_block,
            } => {
                tag(op::TRY_FAULT, &mut self.out);
                self.node(try_block)?;
                self.node(fault_block)?;
            }
            Inst::DebugBreak => tag(op::DEBUG_BREAK, &mut self.out),
            Inst::Comp {
                kind,
                lifting,
                sign,
                input,
                left,
                right,
            } => {
                tag(op::COMP, &mut self.out);
                self.out.u8(kind as u8);
                self.out.u8(lifting as u8);
                self.out.u8(sign as u8);
                self.stack_type(input);
                self.node(left)?;
                self.node(right)?;
            }
            Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => {
                let t = match self.f.inst(id) {
                    Inst::Call(_) => op::CALL,
                    Inst::CallVirt(_) => op::CALL_VIRT,
                    _ => op::NEW_OBJ,
                };
                tag(t, &mut self.out);
                self.member(c.method.token, c.method.state_machine_context);
                if c.method.token == 0 {
                    // A constructor found by type lookup: its type and parameter types.
                    let m = &c.method.method;
                    if m.name != ".ctor" {
                        return Err(format!("method {} has no token", m.name));
                    }
                    self.ty(m.declaring.ok_or("constructor without a declaring type")?);
                    let params = m.params().map_err(|e| e.to_string())?;
                    self.out.len(params.len());
                    for p in params {
                        self.ty(p);
                    }
                }
                self.out.u8(u8::from(c.is_tail)
                    | u8::from(c.il_stack_was_empty) << 1
                    | u8::from(c.constrained_to.is_some()) << 2);
                if let Some(t) = c.constrained_to {
                    self.ty(t.ty);
                }
                self.nodes(&c.args)?;
            }
            Inst::CallIndirect {
                is_instance,
                has_explicit_this,
                signature,
                function_pointer,
                args,
                ..
            } => {
                tag(op::CALL_INDIRECT, &mut self.out);
                self.out.bool(is_instance);
                self.out.bool(has_explicit_this);
                self.ty(signature);
                self.nodes(&args)?;
                self.node(function_pointer)?;
            }
            Inst::Ckfinite { arg } => {
                tag(op::CKFINITE, &mut self.out);
                self.node(arg)?;
            }
            Inst::Conv {
                arg,
                input,
                input_sign,
                target,
                check_overflow,
            } => {
                tag(op::CONV, &mut self.out);
                self.stack_type(input);
                self.out.u8(input_sign as u8);
                self.out.u8(target as u8);
                self.out.bool(check_overflow);
                self.node(arg)?;
            }
            Inst::LdLoc { var } => {
                tag(op::LD_LOC, &mut self.out);
                self.var(var)?;
            }
            Inst::LdLoca { var } => {
                tag(op::LD_LOCA, &mut self.out);
                self.var(var)?;
            }
            Inst::StLoc {
                var,
                value,
                il_stack_was_empty,
                is_stack_adjustment,
            } => {
                tag(op::ST_LOC, &mut self.out);
                self.var(var)?;
                self.out
                    .u8(u8::from(il_stack_was_empty) | u8::from(is_stack_adjustment) << 1);
                self.node(value)?;
            }
            Inst::AddressOf { value, ty } => {
                tag(op::ADDRESS_OF, &mut self.out);
                self.ty(ty.ty);
                self.node(value)?;
            }
            Inst::LdStr { value } => {
                tag(op::LD_STR, &mut self.out);
                self.out.len(value.len());
                for u in value {
                    self.out.raw(&u.to_le_bytes());
                }
            }
            Inst::LdcI4 { value } => {
                tag(op::LDC_I4, &mut self.out);
                self.out.ivar(i64::from(value));
            }
            Inst::LdcI8 { value } => {
                tag(op::LDC_I8, &mut self.out);
                self.out.ivar(value);
            }
            Inst::LdcF4 { bits } => {
                tag(op::LDC_F4, &mut self.out);
                self.out.raw(&bits.to_le_bytes());
            }
            Inst::LdcF8 { bits } => {
                tag(op::LDC_F8, &mut self.out);
                self.out.raw(&bits.to_le_bytes());
            }
            Inst::LdcDecimal { bits } => {
                tag(op::LDC_DECIMAL, &mut self.out);
                for b in bits {
                    self.out.raw(&b.to_le_bytes());
                }
            }
            Inst::LdNull => tag(op::LD_NULL, &mut self.out),
            Inst::LdFtn { method } => {
                tag(op::LD_FTN, &mut self.out);
                self.member(method.token, method.state_machine_context);
            }
            Inst::LdVirtFtn { arg, method } => {
                tag(op::LD_VIRT_FTN, &mut self.out);
                self.member(method.token, method.state_machine_context);
                self.node(arg)?;
            }
            Inst::LdTypeToken { ty } => {
                tag(op::LD_TYPE_TOKEN, &mut self.out);
                self.ty(ty.ty);
            }
            Inst::LdMemberToken { member } => {
                tag(op::LD_MEMBER_TOKEN, &mut self.out);
                let (token, state_machine) = match member {
                    MemberOperand::Method(m) => (m.token, m.state_machine_context),
                    MemberOperand::Field(f) => (f.token, f.state_machine_context),
                };
                self.member(token, state_machine);
            }
            Inst::LocAlloc { arg } => {
                tag(op::LOC_ALLOC, &mut self.out);
                self.node(arg)?;
            }
            Inst::Match {
                variable,
                method,
                tested_operand,
                sub_patterns,
                is_deconstruct_call,
                is_deconstruct_tuple,
                check_type,
                check_not_null,
            } => {
                tag(op::MATCH, &mut self.out);
                self.var(variable)?;
                self.out.u8(u8::from(is_deconstruct_call)
                    | u8::from(is_deconstruct_tuple) << 1
                    | u8::from(check_type) << 2
                    | u8::from(check_not_null) << 3);
                self.out.bool(method.is_some());
                if let Some(m) = &method {
                    self.member(m.token, m.state_machine_context);
                }
                self.node(tested_operand)?;
                self.nodes(&sub_patterns)?;
            }
            Inst::StringToInt { arg, map, ty } => {
                tag(op::STRING_TO_INT, &mut self.out);
                self.ty(ty);
                self.out.len(map.len());
                for (key, value) in &map {
                    self.out.bool(key.is_some());
                    if let Some(k) = key {
                        self.out.len(k.len());
                        for u in k {
                            self.out.raw(&u.to_le_bytes());
                        }
                    }
                    self.out.ivar(i64::from(*value));
                }
                self.node(arg)?;
            }
            Inst::Cpblk {
                dest,
                source,
                size,
                is_volatile,
                unaligned,
            } => {
                tag(op::CPBLK, &mut self.out);
                self.out.bool(is_volatile);
                self.out.u8(unaligned);
                self.node(dest)?;
                self.node(source)?;
                self.node(size)?;
            }
            Inst::Initblk {
                address,
                value,
                size,
                is_volatile,
                unaligned,
            } => {
                tag(op::INITBLK, &mut self.out);
                self.out.bool(is_volatile);
                self.out.u8(unaligned);
                self.node(address)?;
                self.node(value)?;
                self.node(size)?;
            }
            Inst::LdFlda {
                target,
                field,
                delay_exceptions,
            } => {
                tag(op::LD_FLDA, &mut self.out);
                self.member(field.token, field.state_machine_context);
                self.out.bool(delay_exceptions);
                self.node(target)?;
            }
            Inst::LdsFlda { field } => {
                tag(op::LDS_FLDA, &mut self.out);
                self.member(field.token, field.state_machine_context);
            }
            Inst::CastClass { arg, ty }
            | Inst::IsInst { arg, ty }
            | Inst::Box { arg, ty }
            | Inst::Unbox { arg, ty }
            | Inst::UnboxAny { arg, ty }
            | Inst::MakeRefAny { arg, ty }
            | Inst::RefAnyValue { arg, ty } => {
                let t = match self.f.inst(id) {
                    Inst::CastClass { .. } => op::CAST_CLASS,
                    Inst::IsInst { .. } => op::IS_INST,
                    Inst::Box { .. } => op::BOX,
                    Inst::Unbox { .. } => op::UNBOX,
                    Inst::UnboxAny { .. } => op::UNBOX_ANY,
                    Inst::MakeRefAny { .. } => op::MAKE_REF_ANY,
                    _ => op::REF_ANY_VALUE,
                };
                tag(t, &mut self.out);
                self.ty(ty.ty);
                self.node(arg)?;
            }
            Inst::PinnedRegion {
                variable,
                init,
                body,
            } => {
                tag(op::PINNED_REGION, &mut self.out);
                self.var(variable)?;
                self.node(init)?;
                self.node(body)?;
            }
            Inst::YieldReturn { value } => {
                tag(op::YIELD_RETURN, &mut self.out);
                self.node(value)?;
            }
            Inst::Await {
                value,
                ref get_awaiter,
                ref get_result,
                get_awaiter_callvirt,
            } => {
                tag(op::AWAIT, &mut self.out);
                for m in [get_awaiter, get_result] {
                    match m {
                        None => self.out.u8(0),
                        Some(AwaiterMethod::Method(m)) => {
                            self.out.u8(1);
                            self.member(m.token, m.state_machine_context);
                        }
                        Some(AwaiterMethod::Dynamic(name)) => {
                            self.out.u8(2);
                            self.string(name);
                        }
                    }
                }
                self.out.bool(get_awaiter_callvirt);
                self.node(value)?;
            }
            Inst::Dynamic {
                kind,
                binder_flags,
                context,
                ref name,
                operation,
                ty,
                ref type_arguments,
                ref arg_infos,
                ref args,
            } => {
                tag(op::DYNAMIC, &mut self.out);
                self.out.u8(kind as u8);
                self.out.ivar(i64::from(binder_flags));
                self.opt_ty(context);
                self.out.bool(name.is_some());
                if let Some(n) = name {
                    self.string(n);
                }
                self.out.ivar(i64::from(operation));
                self.opt_ty(ty);
                self.out.bool(type_arguments.is_some());
                if let Some(ts) = type_arguments {
                    self.out.len(ts.len());
                    for &t in ts {
                        self.ty(t);
                    }
                }
                self.out.len(arg_infos.len());
                for info in arg_infos {
                    self.out.ivar(i64::from(info.flags));
                    self.out.bool(info.name.is_some());
                    if let Some(n) = &info.name {
                        self.string(n);
                    }
                    self.ty(info.compile_time_type);
                }
                self.nodes(args)?;
            }
            Inst::GetPinnableReference { arg, ty, method } => {
                tag(op::GET_PINNABLE_REFERENCE, &mut self.out);
                self.ty(ty);
                self.out.bool(method.is_some());
                if let Some(m) = method {
                    self.member(m.token, m.state_machine_context);
                }
                self.node(arg)?;
            }
            Inst::LdObj {
                target,
                ty,
                is_volatile,
                unaligned,
            } => {
                tag(op::LD_OBJ, &mut self.out);
                self.ty(ty.ty);
                self.out.bool(is_volatile);
                self.out.u8(unaligned);
                self.node(target)?;
            }
            Inst::LdObjIfRef { target, ty } => {
                tag(op::LD_OBJ_IF_REF, &mut self.out);
                self.ty(ty.ty);
                self.node(target)?;
            }
            Inst::StObj {
                target,
                value,
                ty,
                is_volatile,
                unaligned,
            } => {
                tag(op::ST_OBJ, &mut self.out);
                self.ty(ty.ty);
                self.out.bool(is_volatile);
                self.out.u8(unaligned);
                self.node(target)?;
                self.node(value)?;
            }
            Inst::NewArr { ty, indices } => {
                tag(op::NEW_ARR, &mut self.out);
                self.ty(ty.ty);
                self.nodes(&indices)?;
            }
            Inst::DefaultValue {
                ty,
                il_stack_was_empty,
            } => {
                tag(op::DEFAULT_VALUE, &mut self.out);
                self.ty(ty.ty);
                self.out.bool(il_stack_was_empty);
            }
            Inst::Throw { arg } => {
                tag(op::THROW, &mut self.out);
                self.node(arg)?;
            }
            Inst::Rethrow => tag(op::RETHROW, &mut self.out),
            Inst::SizeOf { ty } => {
                tag(op::SIZE_OF, &mut self.out);
                self.ty(ty.ty);
            }
            Inst::LdLen { array, result } => {
                tag(op::LD_LEN, &mut self.out);
                self.stack_type(result);
                self.node(array)?;
            }
            Inst::LdElema {
                ty,
                array,
                indices,
                delay_exceptions,
                is_read_only,
            } => {
                tag(op::LD_ELEMA, &mut self.out);
                self.ty(ty.ty);
                self.out.bool(delay_exceptions);
                self.out.bool(is_read_only);
                self.node(array)?;
                self.nodes(&indices)?;
            }
            Inst::RefAnyType { arg } => {
                tag(op::REF_ANY_TYPE, &mut self.out);
                self.node(arg)?;
            }
        }
        Ok(())
    }
}

/// Reads every method body of `module` and encodes the results: a failed read (or a panic,
/// caught per method) is recorded with its message instead of the function.
pub fn read_module(
    ts: &mut TypeSystem<'_, '_>,
    module: crate::typesystem::compilation::ModuleId,
    options: crate::il::reader::ReaderOptions<'_>,
) -> Vec<u8> {
    use crate::metadata::tables::{TableId, rows};
    use crate::typesystem::members::MethodDefId;
    let md = &ts.comp.module(module).md;
    let count = md.tables.rows(TableId::MethodDef);
    let mut results = Vec::new();
    for row in 1..=count {
        let outcome = std::panic::catch_unwind(std::panic::AssertUnwindSafe(|| {
            let md = &ts.comp.module(module).md;
            let rva = md.get::<rows::MethodDef>(row)?.rva;
            if rva == 0 {
                return Ok(None);
            }
            let body = md.method_body(rva)?;
            crate::il::reader::read_il(ts, MethodDefId { module, row }, body, options).map(Some)
        }));
        match outcome {
            Ok(Ok(None)) => {}
            Ok(Ok(Some(f))) => results.push((row, Ok(f))),
            Ok(Err(e)) => results.push((row, Err(e.to_string()))),
            Err(panic) => {
                let message = panic
                    .downcast_ref::<&str>()
                    .map(|s| (*s).to_owned())
                    .or_else(|| panic.downcast_ref::<String>().cloned())
                    .unwrap_or_default();
                results.push((row, Err(format!("panic: {message}"))));
            }
        }
    }
    let mut w = ModuleWriter::new(ts);
    for (row, r) in &results {
        let written = match r {
            Ok(f) => w.function(*row, f),
            Err(e) => Err(e.clone()),
        };
        if let Err(e) = written {
            w.failed(*row, &e);
        }
    }
    w.finish()
}

/// Reads one method body of `module`, runs the first `il_prefix` IL transforms and encodes
/// the function as a one-entry message in the
/// `read_module` format; a failed read is an error for the caller, not an entry.
pub fn read_function(
    ts: &mut TypeSystem<'_, '_>,
    method: crate::typesystem::members::MethodDefId,
    options: crate::il::reader::ReaderOptions<'_>,
    il_prefix: u32,
    settings: crate::il::transforms::Settings,
) -> Result<Vec<u8>, String> {
    use crate::metadata::tables::rows;
    let md = &ts.comp.module(method.module).md;
    let rva = md
        .get::<rows::MethodDef>(method.row)
        .map_err(|e| e.to_string())?
        .rva;
    if rva == 0 {
        return Err(format!("MethodDef row {} has no body", method.row));
    }
    let body = md.method_body(rva).map_err(|e| e.to_string())?;
    let mut f = crate::il::reader::read_il(ts, method, body, options).map_err(|e| e.to_string())?;
    let mut ctx = crate::il::transforms::Context {
        ts,
        settings,
        reader: options,
    };
    crate::il::transforms::run_prefix(&mut f, &mut ctx, il_prefix);
    let mut w = ModuleWriter::new(ts);
    w.function(method.row, &f)?;
    Ok(w.finish())
}
