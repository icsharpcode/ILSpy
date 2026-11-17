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

pub const VERSION: u32 = 5;

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
    pub const LOCK: u8 = 70;
    pub const USING: u8 = 71;
    pub const NUMERIC_COMPOUND_ASSIGN: u8 = 72;
    pub const USER_DEFINED_COMPOUND_ASSIGN: u8 = 73;
    pub const LOC_ALLOC_SPAN: u8 = 74;
    pub const LD_VIRT_DELEGATE: u8 = 75;
    pub const LD_ELEMA_INLINE_ARRAY: u8 = 76;
    pub const NULL_COALESCING: u8 = 77;
    pub const DYNAMIC_COMPOUND_ASSIGN: u8 = 78;
    pub const THREE_VALUED_BOOL_AND: u8 = 79;
    pub const THREE_VALUED_BOOL_OR: u8 = 80;
    pub const NULLABLE_UNWRAP: u8 = 81;
    pub const NULLABLE_REWRAP: u8 = 82;
    pub const LD_STR_UTF8: u8 = 83;
    pub const FUNCTION: u8 = 84;
    pub const EXPRESSION_TREE_CAST: u8 = 85;
    pub const DECONSTRUCT: u8 = 86;
    pub const DECONSTRUCT_RESULT: u8 = 87;
    pub const USER_DEFINED_LOGIC: u8 = 88;
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

    /// One function; `body` is the C# translation of its body when it was asked for, which
    /// follows the function (see `wire::ast`).
    pub fn function(
        &mut self,
        row: u32,
        r: &ReadFunction,
        body: Option<&super::ast::Body>,
    ) -> Result<(), String> {
        let nested = nested_variables(&r.function);
        let mut w = FunctionWriter {
            ts: self.ts,
            tables: &mut self.tables,
            f: &r.function,
            out: Bytes::default(),
            vars: r
                .variables
                .iter()
                .chain(&nested)
                .enumerate()
                .map(|(i, &v)| (v, i))
                .collect(),
            blocks: HashMap::new(),
            block_order: Vec::new(),
            containers: HashMap::new(),
            written: HashMap::new(),
            special_constants: true,
        };
        w.write(r, &nested)?;
        super::ast::write_body(&mut w, body)?;
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

pub(super) struct FunctionWriter<'w, 't, 'c, 'a> {
    pub(super) ts: &'t TypeSystem<'c, 'a>,
    /// `UseSpecialConstants` for the constants written now: off in the initializers of
    /// integer and decimal arrays.
    pub(super) special_constants: bool,
    tables: &'w mut Tables,
    f: &'w Function,
    pub(super) out: Bytes,
    pub(super) vars: HashMap<VarId, usize>,
    blocks: HashMap<NodeId, usize>,
    block_order: Vec<NodeId>,
    containers: HashMap<NodeId, usize>,
    /// The nodes written so far, numbered in completion order; a node with two parents is
    /// written once and then referenced (`op::NODE_REFERENCE`).
    pub(super) written: HashMap<NodeId, usize>,
}

impl FunctionWriter<'_, '_, '_, '_> {
    pub(super) fn ty(&mut self, t: TypeId) {
        let i = self.tables.ty(self.ts, t);
        self.out.uvar(u64::from(i));
    }

    /// A member token, shifted left by one; bit 0 says the generic context it resolves in
    /// (`MethodOperand::context`) follows, which is otherwise the function's own.
    pub(super) fn member(&mut self, token: u32, context: u32) {
        self.out
            .uvar(u64::from(token) << 1 | u64::from(context != 0));
        if context != 0 {
            self.out.uvar(u64::from(context));
        }
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

    pub(super) fn string(&mut self, s: &str) {
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

    /// One variable of a variable list.
    fn variable(&mut self, v: VarId) {
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
            | u8::from(var.remove_if_redundant) << 4
            | u8::from(var.is_scoped) << 5
            | u8::from(var.is_scoped_without_initializer) << 6);
        self.out.bool(var.state_machine_field.is_some());
        if let Some(field) = &var.state_machine_field {
            self.member(field.token, field.context);
        }
    }

    /// The method of a call (or another instruction calling a method): the member, the
    /// lookup data of a method without token, and the lifted and range accessor flags.
    pub(super) fn call_method(
        &mut self,
        method: &crate::il::ast::MethodOperand,
    ) -> Result<(), String> {
        // A method `ProxyCallReplacer` specialized: first the proxy call, in whose generic
        // context the host resolves the token.
        self.out.bool(method.specialized_by.is_some());
        if let Some(proxy) = &method.specialized_by {
            self.call_method(proxy)?;
        }
        self.member(method.token, method.context);
        if method.token == 0 {
            // A method found by type lookup: its type, name and parameter types.
            let m = &method.method;
            self.ty(m.declaring.ok_or("method without a declaring type")?);
            self.string(&m.name);
            // A synthetic range accessor is found by the underlying parameters.
            let params = match &method.range_accessor {
                Some(r) => r.underlying_params.clone(),
                None => m.params().map_err(|e| e.to_string())?,
            };
            self.out.len(params.len());
            for p in params {
                // A lifted operator is found by its non-lifted parameters.
                let p = if method.lifted {
                    match self.ts.data(p) {
                        crate::typesystem::types::TypeData::Inst { args, .. }
                            if args.len() == 1 =>
                        {
                            args[0]
                        }
                        _ => p,
                    }
                } else {
                    p
                };
                self.ty(p);
            }
        }
        // The host wraps the method with `CSharpOperators.LiftUserDefinedOperator`.
        self.out.bool(method.lifted);
        // The host wraps the method in a `SyntheticRangeIndexAccessor`.
        self.out.bool(method.range_accessor.is_some());
        if let Some(r) = &method.range_accessor {
            self.ty(r.index_or_range_type);
            self.out.bool(r.slicing);
        }
        // The host makes the method a `LocalFunctionMethod` of the local function read from
        // `method` in `context`.
        self.out.bool(method.local_function.is_some());
        if let Some(l) = &method.local_function {
            self.out.uvar(u64::from(l.method.row));
            self.out.uvar(u64::from(l.context));
            self.out.bool(l.reduced.is_static_local_function);
            self.out
                .uvar(u64::from(l.reduced.number_of_compiler_generated_parameters));
            self.out.uvar(u64::from(
                l.reduced.number_of_compiler_generated_type_parameters,
            ));
            self.out.bool(l.name.is_some());
            if let Some(name) = &l.name {
                self.string(name);
            }
        }
        Ok(())
    }

    /// The type system's explicit generic contexts, which `member` refers to by index.
    fn contexts(&mut self) {
        let contexts = self.ts.explicit_contexts().to_vec();
        self.out.len(contexts.len());
        for c in contexts {
            self.out.len(c.class.len());
            for t in c.class {
                self.ty(t);
            }
            self.out.len(c.method.len());
            for t in c.method {
                self.ty(t);
            }
            self.out.bool(c.method_type_parameters_of.is_some());
            if let Some((token, context, first)) = c.method_type_parameters_of {
                self.member(token, context);
                self.out.uvar(u64::from(first));
            }
        }
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

    fn write(&mut self, r: &ReadFunction, nested: &[VarId]) -> Result<(), String> {
        let body = r.function.body.ok_or("function without body")?;
        self.contexts();
        self.out.len(r.code_size as usize);
        self.out.len(r.local_signature_length);
        match r.async_return_type {
            Some(t) => {
                let i = self.tables.ty(self.ts, t);
                self.out.uvar(u64::from(i) + 1);
            }
            None => self.out.u8(0),
        }
        self.out.uvar(u64::from(r.function.helper_variable_count));
        self.out.u8(u8::from(r.is_iterator)
            | u8::from(r.state_machine_compiled_with_mono) << 1
            | u8::from(r.state_machine_compiled_with_legacy_visual_basic) << 2);
        self.out.bool(r.move_next_method.is_some());
        if let Some(m) = &r.move_next_method {
            self.member(m.token, m.context);
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
            self.variable(v);
        }
        // The variables of nested functions follow; each function lists its own.
        self.out.len(nested.len());
        for &v in nested {
            self.variable(v);
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
        for lf in self.f.local_functions.clone() {
            self.number(lf);
        }
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
        self.node(body)?;
        // `LocalFunctions` of the top-level function.
        let local_functions = self.f.local_functions.clone();
        self.out.len(local_functions.len());
        for lf in local_functions {
            self.node(lf)?;
        }
        // Capture scopes refer to containers, which are numbered by now.
        let captures: Vec<(usize, usize)> = r
            .variables
            .iter()
            .chain(nested)
            .filter_map(|&v| {
                let scope = self.f.var(v).capture_scope?;
                Some((self.vars[&v], *self.containers.get(&scope)?))
            })
            .collect();
        self.out.len(captures.len());
        for (v, container) in captures {
            self.out.len(v);
            self.out.len(container);
        }
        // `DeclarationScope` of each local function: its node number and the container's.
        let mut scopes: Vec<(usize, usize)> = self
            .written
            .iter()
            .filter_map(|(&n, &i)| match self.f.inst(n) {
                Inst::Function {
                    declaration_scope: Some(scope),
                    ..
                } => Some((i, *self.containers.get(scope)?)),
                _ => None,
            })
            .collect();
        scopes.sort_unstable();
        self.out.len(scopes.len());
        for (function, container) in scopes {
            self.out.len(function);
            self.out.len(container);
        }
        // `CapturedVariables`: the function as its node number + 1 (0 for the top-level one).
        let captured = self.f.captured_variables.clone();
        self.out.len(captured.len());
        for (function, v) in captured {
            let function = match function {
                Some(n) => {
                    1 + *self
                        .written
                        .get(&n)
                        .ok_or("captured in an unwritten function")?
                }
                None => 0,
            };
            self.out.len(function);
            self.var(v)?;
        }
        Ok(())
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
                csharp_result_type,
                is_lifted,
            } => {
                tag(op::BINARY_NUMERIC, &mut self.out);
                self.out.u8(o as u8);
                self.out.bool(check_overflow);
                self.out.u8(sign as u8);
                self.stack_type(left_input);
                self.stack_type(right_input);
                self.opt_ty(csharp_result_type);
                self.out.bool(is_lifted);
                self.node(left)?;
                self.node(right)?;
            }
            Inst::BitNot {
                arg,
                lifted_underlying,
            } => {
                tag(op::BIT_NOT, &mut self.out);
                self.out.bool(lifted_underlying.is_some());
                if let Some(st) = lifted_underlying {
                    self.stack_type(st);
                }
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
                result_type,
            } => {
                tag(op::SWITCH, &mut self.out);
                self.out.bool(is_lifted);
                self.opt_ty(result_type);
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
                exception_specifier_il_range,
            } => {
                tag(op::TRY_CATCH_HANDLER, &mut self.out);
                self.var(variable)?;
                self.out.ivar(i64::from(exception_specifier_il_range.start));
                self.out.ivar(i64::from(exception_specifier_il_range.end));
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
                self.call_method(&c.method)?;
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
                is_lifted,
            } => {
                tag(op::CONV, &mut self.out);
                self.stack_type(input);
                self.out.u8(input_sign as u8);
                self.out.u8(target as u8);
                self.out.bool(check_overflow);
                self.out.bool(is_lifted);
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
            Inst::LdStrUtf8 { value } => {
                tag(op::LD_STR_UTF8, &mut self.out);
                let units: Vec<u16> = value.encode_utf16().collect();
                self.out.len(units.len());
                for u in units {
                    self.out.raw(&u.to_le_bytes());
                }
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
                self.call_method(&method)?;
            }
            Inst::LdVirtFtn { arg, method } => {
                tag(op::LD_VIRT_FTN, &mut self.out);
                self.call_method(&method)?;
                self.node(arg)?;
            }
            Inst::LdTypeToken { ty } => {
                tag(op::LD_TYPE_TOKEN, &mut self.out);
                self.ty(ty.ty);
            }
            Inst::LdMemberToken { member } => {
                tag(op::LD_MEMBER_TOKEN, &mut self.out);
                let (token, state_machine) = match member {
                    MemberOperand::Method(m) => (m.token, m.context),
                    MemberOperand::Field(f) => (f.token, f.context),
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
                    self.member(m.token, m.context);
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
                self.member(field.token, field.context);
                self.out.bool(delay_exceptions);
                self.node(target)?;
            }
            Inst::LdsFlda { field } => {
                tag(op::LDS_FLDA, &mut self.out);
                self.member(field.token, field.context);
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
            Inst::Lock {
                on_expression,
                body,
            } => {
                tag(op::LOCK, &mut self.out);
                self.node(on_expression)?;
                self.node(body)?;
            }
            Inst::Using {
                variable,
                resource_expression,
                body,
                is_async,
                is_ref_struct,
            } => {
                tag(op::USING, &mut self.out);
                self.var(variable)?;
                self.node(resource_expression)?;
                self.node(body)?;
                self.out
                    .u8(u8::from(is_async) | u8::from(is_ref_struct) << 1);
            }
            Inst::NumericCompoundAssign {
                op: o,
                check_overflow,
                sign,
                left_input,
                right_input,
                is_lifted,
                ty,
                eval_mode,
                target_kind,
                target,
                value,
                ..
            } => {
                tag(op::NUMERIC_COMPOUND_ASSIGN, &mut self.out);
                self.out.bool(is_lifted);
                self.out.u8(o as u8);
                self.out.bool(check_overflow);
                self.out.u8(sign as u8);
                self.stack_type(left_input);
                self.stack_type(right_input);
                self.ty(ty);
                self.out.u8(eval_mode as u8);
                self.out.u8(target_kind as u8);
                self.node(target)?;
                self.node(value)?;
            }
            Inst::UserDefinedCompoundAssign {
                ref method,
                eval_mode,
                target_kind,
                target,
                value,
            } => {
                tag(op::USER_DEFINED_COMPOUND_ASSIGN, &mut self.out);
                self.member(method.token, method.context);
                self.out.u8(eval_mode as u8);
                self.out.u8(target_kind as u8);
                self.node(target)?;
                self.node(value)?;
            }
            Inst::DynamicCompoundAssign {
                operation,
                binder_flags,
                ref target_info,
                ref value_info,
                target_kind,
                target,
                value,
                ..
            } => {
                // The host derives the evaluation mode from the operation, as the constructor
                // does.
                tag(op::DYNAMIC_COMPOUND_ASSIGN, &mut self.out);
                self.out.ivar(i64::from(operation));
                self.out.ivar(i64::from(binder_flags));
                self.out.len(2);
                for info in [target_info, value_info] {
                    self.out.ivar(i64::from(info.flags));
                    self.out.bool(info.name.is_some());
                    if let Some(n) = &info.name {
                        self.string(n);
                    }
                    self.ty(info.compile_time_type);
                }
                self.out.u8(target_kind as u8);
                self.node(target)?;
                self.node(value)?;
            }
            Inst::ThreeValuedBoolAnd { left, right } | Inst::ThreeValuedBoolOr { left, right } => {
                let t = if matches!(self.f.inst(id), Inst::ThreeValuedBoolAnd { .. }) {
                    op::THREE_VALUED_BOOL_AND
                } else {
                    op::THREE_VALUED_BOOL_OR
                };
                tag(t, &mut self.out);
                self.node(left)?;
                self.node(right)?;
            }
            Inst::LocAllocSpan { arg, ty } => {
                tag(op::LOC_ALLOC_SPAN, &mut self.out);
                self.ty(ty);
                self.node(arg)?;
            }
            Inst::LdVirtDelegate {
                arg,
                ty,
                ref method,
            } => {
                tag(op::LD_VIRT_DELEGATE, &mut self.out);
                self.ty(ty);
                self.member(method.token, method.context);
                self.node(arg)?;
            }
            Inst::LdElemaInlineArray {
                ty,
                array,
                ref indices,
                is_read_only,
            } => {
                tag(op::LD_ELEMA_INLINE_ARRAY, &mut self.out);
                self.ty(ty);
                self.out.bool(is_read_only);
                self.node(array)?;
                self.nodes(indices)?;
            }
            Inst::NullCoalescing {
                kind,
                ty,
                value,
                fallback,
            } => {
                tag(op::NULL_COALESCING, &mut self.out);
                self.out.u8(kind as u8);
                self.ty(ty);
                self.node(value)?;
                self.node(fallback)?;
            }
            Inst::UserDefinedLogic {
                ref method,
                left,
                right,
            } => {
                tag(op::USER_DEFINED_LOGIC, &mut self.out);
                self.call_method(method)?;
                self.node(left)?;
                self.node(right)?;
            }
            Inst::NullableUnwrap {
                arg,
                ty,
                result,
                ref_input,
            } => {
                tag(op::NULLABLE_UNWRAP, &mut self.out);
                self.ty(ty);
                self.stack_type(result);
                self.out.bool(ref_input);
                self.node(arg)?;
            }
            Inst::Function {
                kind,
                delegate_type,
                return_type,
                ref parameters,
                ref variables,
                helper_variable_count,
                body,
                ref read,
                ref local_functions,
                reduced,
                ref name,
                ..
            } => {
                tag(op::FUNCTION, &mut self.out);
                self.out.u8(kind as u8);
                self.opt_ty(delegate_type);
                self.ty(return_type);
                self.out.len(parameters.len());
                for (ty, name) in parameters {
                    self.ty(*ty);
                    self.string(name);
                }
                self.out.len(variables.len());
                for &v in variables {
                    self.var(v)?;
                }
                self.out.uvar(u64::from(helper_variable_count));
                // A function read from a method body: its method, context and reader results.
                self.out.bool(read.is_some());
                if let Some(read) = read {
                    // A local function without a body has the use site's method itself.
                    self.out.bool(read.method_operand.is_some());
                    if let Some(m) = &read.method_operand {
                        self.call_method(m)?;
                    }
                    self.out.uvar(u64::from(read.method.row));
                    self.out.uvar(u64::from(read.context));
                    self.out.len(read.code_size as usize);
                    self.out.len(read.local_signature_length);
                    self.opt_ty(read.async_return_type);
                    self.out.u8(u8::from(read.is_iterator)
                        | u8::from(read.state_machine_compiled_with_mono) << 1
                        | u8::from(read.state_machine_compiled_with_legacy_visual_basic) << 2);
                    self.out.bool(read.move_next_method.is_some());
                    if let Some(m) = &read.move_next_method {
                        self.member(m.token, m.context);
                    }
                    self.out.bool(read.async_debug_info.is_some());
                    if let Some((catch_handler, awaits)) = &read.async_debug_info {
                        self.out.ivar(i64::from(*catch_handler));
                        self.out.len(awaits.len());
                        for &(yield_offset, resume_offset) in awaits {
                            self.out.ivar(i64::from(yield_offset));
                            self.out.ivar(i64::from(resume_offset));
                        }
                    }
                    self.out.len(read.warnings.len());
                    for w in &read.warnings {
                        self.string(w);
                    }
                    self.out.len(read.sequence_point_candidates.len());
                    for &s in &read.sequence_point_candidates {
                        self.out.ivar(i64::from(s));
                    }
                }
                self.node(body)?;
                self.nodes(local_functions)?;
                // `ReducedMethod` of a local function.
                self.out.bool(reduced.is_some());
                if let Some(r) = reduced {
                    self.out.bool(r.is_static_local_function);
                    self.out
                        .uvar(u64::from(r.number_of_compiler_generated_parameters));
                    self.out
                        .uvar(u64::from(r.number_of_compiler_generated_type_parameters));
                }
                // `Name`, when it is not the method's.
                self.out.bool(name.is_some());
                if let Some(name) = name {
                    self.string(name);
                }
            }
            Inst::Deconstruct {
                ref init,
                pattern,
                conversions,
                assignments,
            } => {
                tag(op::DECONSTRUCT, &mut self.out);
                self.nodes(init)?;
                self.node(pattern)?;
                self.node(conversions)?;
                self.node(assignments)?;
            }
            Inst::DeconstructResult {
                index,
                ty,
                result,
                arg,
            } => {
                tag(op::DECONSTRUCT_RESULT, &mut self.out);
                self.out.ivar(i64::from(index));
                self.ty(ty);
                self.stack_type(result);
                self.node(arg)?;
            }
            Inst::ExpressionTreeCast {
                ty,
                arg,
                is_checked,
            } => {
                tag(op::EXPRESSION_TREE_CAST, &mut self.out);
                self.ty(ty);
                self.out.bool(is_checked);
                self.node(arg)?;
            }
            Inst::NullableRewrap { arg, ty } => {
                tag(op::NULLABLE_REWRAP, &mut self.out);
                self.ty(ty);
                self.node(arg)?;
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
                            self.member(m.token, m.context);
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
                    self.member(m.token, m.context);
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
                with_system_index,
            } => {
                tag(op::LD_ELEMA, &mut self.out);
                self.ty(ty.ty);
                self.out.bool(delay_exceptions);
                self.out.bool(is_read_only);
                self.out.bool(with_system_index);
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
            Ok(f) => w.function(*row, f, None),
            Err(e) => Err(e.clone()),
        };
        if let Err(e) = written {
            w.failed(*row, &e);
        }
    }
    w.finish()
}

/// Reads one method body of `module`, runs the first `il_prefix` IL transforms and encodes the function as a one-entry message in the `read_module` format; a failed
/// read is an error for the caller, not an entry. `analysis` reads and finishes the body as
/// `CSharpDecompiler.DecompileBodyForAnalysis` does: in a context of the declaring type's
/// parameters only, which specializes the method's signature through them, and with
/// CombineExitsTransform after the prefix.
#[allow(clippy::too_many_arguments)]
pub fn read_function(
    ts: &mut TypeSystem<'_, '_>,
    method: crate::typesystem::members::MethodDefId,
    options: crate::il::reader::ReaderOptions<'_>,
    il_prefix: u32,
    settings: crate::il::transforms::Settings,
    resolver: Option<&dyn crate::il::transforms::Resolver>,
    root_names: Option<&crate::il::transforms::RootNames>,
    analysis: bool,
    translate_body: bool,
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
    let context = if analysis {
        analysis_context(ts, method).map_err(|e| e.to_string())?
    } else {
        None
    };
    let mut f = crate::il::reader::read_il_in_context(ts, method, body, options, context)
        .map_err(|e| e.to_string())?;
    let mut ctx = crate::il::transforms::Context {
        ts,
        settings,
        reader: options,
        rerun_requested: false,
        removed_variables: Vec::new(),
        resolver,
        root_names,
    };
    crate::il::transforms::run_prefix(&mut f, &mut ctx, il_prefix);
    if analysis {
        crate::il::transforms::combine_exits_transform::run(&mut f, &mut ctx);
    }
    // A translated body comes with the function as the translation changed it.
    let translated =
        match translate_body.then(|| crate::csharp::convert_body(ts, &f, settings, resolver)) {
            Some(Ok((ast, stmt, function))) => {
                f.function = function;
                // Variables the translation registered, as `RegisterVariable` adds them.
                f.flush_pending_variables();
                Some(Ok((ast, stmt)))
            }
            Some(Err(reason)) => Some(Err(reason)),
            None => None,
        };
    if matches!(translated, Some(Ok(_))) {
        ts.intern_oblivious_instances();
        ts.intern_constant_types();
    }
    let mut w = ModuleWriter::new(ts);
    w.function(method.row, &f, translated.as_ref())?;
    if matches!(translated, Some(Ok(_))) {
        w.tables.type_syntax(ts, settings.lift_nullables);
    }
    Ok(w.finish())
}

/// `new GenericContext(classTypeParameters: method.DeclaringTypeDefinition?.TypeParameters,
/// methodTypeParameters: null)`.
fn analysis_context(
    ts: &mut TypeSystem<'_, '_>,
    method: crate::typesystem::members::MethodDefId,
) -> crate::metadata::Result<Option<crate::typesystem::types::GenericContext>> {
    // Without class type parameters the substitution changes nothing: the own context reads alike.
    let Some((def, count)) = ts.method_context(method)?.class.filter(|&(_, n)| n > 0) else {
        return Ok(None);
    };
    let class = (0..count)
        .map(|index| {
            let owner = ts.class_param_owner(def, index);
            ts.intern(crate::typesystem::types::TypeData::TypeParam { owner, index })
        })
        .collect();
    Ok(Some(ts.explicit_context(class, Vec::new())))
}

/// Encodes `query` for the host's resolver: the tables, then the explicit generic contexts, the kind (0 `Call`, 1 `Method`),
/// the method as `call_method` writes it, and for `Call` the target type, whether the target
/// store types follow (then the variable's and the value's type) and the argument types.
pub fn resolver_query(
    ts: &TypeSystem<'_, '_>,
    query: &crate::il::transforms::ExtensionMethodQuery<'_>,
) -> Result<Vec<u8>, String> {
    use crate::il::transforms::ExtensionMethodQuery;
    query_message(ts, |w| {
        match query {
            ExtensionMethodQuery::Call {
                method,
                target,
                target_store,
                arguments,
            } => {
                w.out.u8(0);
                w.call_method(method)?;
                w.ty(*target);
                w.out.bool(target_store.is_some());
                if let Some((variable, value)) = *target_store {
                    w.ty(variable);
                    w.ty(value);
                }
                w.out.len(arguments.len());
                for &a in arguments {
                    w.ty(a);
                }
            }
            ExtensionMethodQuery::Method { method } => {
                w.out.u8(1);
                w.call_method(method)?;
            }
        }
        Ok(())
    })
}

/// A `field_reference_binds` question in the `resolver_query` format: kind 2 with the field
/// for a simple name; kind 3 with the field, the target's kind (0 `this`/`base` followed by
/// whether it causes a non-virtual invocation, 1 a type, 2 another expression) and the
/// target's type for a member access.
pub fn field_reference_query(
    ts: &TypeSystem<'_, '_>,
    field: &crate::il::ast::FieldOperand,
    target: Option<crate::il::transforms::LookupTarget>,
) -> Result<Vec<u8>, String> {
    use crate::il::transforms::LookupTarget;
    query_message(ts, |w| {
        w.out.u8(if target.is_some() { 3 } else { 2 });
        w.member(field.token, field.context);
        match target {
            None => {}
            Some(LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            }) => {
                w.out.u8(0);
                w.out.bool(causes_non_virtual_invocation);
                w.ty(ty);
            }
            Some(LookupTarget::Type(ty)) => {
                w.out.u8(1);
                w.ty(ty);
            }
            Some(LookupTarget::Expression(ty)) => {
                w.out.u8(2);
                w.ty(ty);
            }
        }
        Ok(())
    })
}

/// A `field_kind` question in the `resolver_query` format: kind 4, the field and whether its
/// target is the `this` instance.
pub fn field_kind_query(
    ts: &TypeSystem<'_, '_>,
    field: &crate::il::ast::FieldOperand,
    target_is_this: bool,
) -> Result<Vec<u8>, String> {
    query_message(ts, |w| {
        w.out.u8(4);
        w.member(field.token, field.context);
        w.out.bool(target_is_this);
        Ok(())
    })
}

/// A `method_group` question in the `resolver_query` format: kind 5, the name, and the
/// target as `field_reference_query` writes it after a byte saying whether there is one, then
/// the number of type arguments and the types.
/// The answer is 0 for no method group, or 1, the number of lists and per list the declaring
/// type's module index and TypeDef row and its methods' module indices and MethodDef rows, as
/// unsigned LEB128.
pub fn method_group_query(
    ts: &TypeSystem<'_, '_>,
    name: &str,
    target: Option<crate::il::transforms::LookupTarget>,
    type_arguments: &[crate::typesystem::types::TypeId],
) -> Result<Vec<u8>, String> {
    lookup_query(ts, 5, name, target, type_arguments)
}

/// Encodes the host's lookup of a method reference's name (kind 11), as `method_group_query`
/// but not as an invocation target.
pub fn method_group_reference_query(
    ts: &TypeSystem<'_, '_>,
    name: &str,
    target: Option<crate::il::transforms::LookupTarget>,
    type_arguments: &[crate::typesystem::types::TypeId],
) -> Result<Vec<u8>, String> {
    lookup_query(ts, 11, name, target, type_arguments)
}

/// Encodes the host's indexer lookup on `target` (kind 10), as `method_group_query` with an
/// empty name; the answer is decoded with `method_group_answer`.
pub fn indexers_query(
    ts: &TypeSystem<'_, '_>,
    target: crate::il::transforms::LookupTarget,
) -> Result<Vec<u8>, String> {
    lookup_query(ts, 10, "", Some(target), &[])
}

fn lookup_query(
    ts: &TypeSystem<'_, '_>,
    kind: u8,
    name: &str,
    target: Option<crate::il::transforms::LookupTarget>,
    type_arguments: &[crate::typesystem::types::TypeId],
) -> Result<Vec<u8>, String> {
    use crate::il::transforms::LookupTarget;
    query_message(ts, |w| {
        w.out.u8(kind);
        w.string(name);
        w.out.bool(target.is_some());
        match target {
            None => {}
            Some(LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            }) => {
                w.out.u8(0);
                w.out.bool(causes_non_virtual_invocation);
                w.ty(ty);
            }
            Some(LookupTarget::Type(ty)) => {
                w.out.u8(1);
                w.ty(ty);
            }
            Some(LookupTarget::Expression(ty)) => {
                w.out.u8(2);
                w.ty(ty);
            }
        }
        w.out.len(type_arguments.len());
        for &t in type_arguments {
            w.ty(t);
        }
        Ok(())
    })
}

/// An `accessor_reference` question in the `resolver_query` format: kind 6, the accessor as
/// `call_method` writes it, whether the call is a `callvirt`, and the target as
/// `method_group_query` writes it. The answer is 0 for `AccessorBinding::None`, 1 for
/// `Owner` and 2 for `Other`.
pub fn accessor_reference_query(
    ts: &TypeSystem<'_, '_>,
    accessor: &crate::il::ast::MethodOperand,
    target: Option<crate::il::transforms::LookupTarget>,
    callvirt: bool,
) -> Result<Vec<u8>, String> {
    use crate::il::transforms::LookupTarget;
    query_message(ts, |w| {
        w.out.u8(6);
        w.call_method(accessor)?;
        w.out.bool(callvirt);
        w.out.bool(target.is_some());
        match target {
            None => {}
            Some(LookupTarget::This {
                ty,
                causes_non_virtual_invocation,
            }) => {
                w.out.u8(0);
                w.out.bool(causes_non_virtual_invocation);
                w.ty(ty);
            }
            Some(LookupTarget::Type(ty)) => {
                w.out.u8(1);
                w.ty(ty);
            }
            Some(LookupTarget::Expression(ty)) => {
                w.out.u8(2);
                w.ty(ty);
            }
        }
        Ok(())
    })
}

/// A `can_use_extension_method_syntax` question in the `resolver_query` format: kind 8, the
/// method as `call_method` writes it, the type arguments (count and types), the target and
/// the arguments (count first), each 0 and its type for an expression, 1, its type and the
/// constant as `wire::ast` writes it, 2, the element type and the `RefKind` (in the order of
/// `ReferenceKind`) for a by-reference argument, 3 and the type for an `out var`, and the
/// argument names (0 for none, else 1 and per argument whether it has a name and the name).
/// The answer is 1 for yes.
#[allow(clippy::too_many_arguments)]
pub fn extension_method_syntax_query(
    ts: &TypeSystem<'_, '_>,
    method: &crate::il::ast::MethodOperand,
    method_args: Option<&[crate::typesystem::types::TypeId]>,
    type_arguments: &[crate::typesystem::types::TypeId],
    target: &crate::semantics::ResolveResult,
    arguments: &[crate::semantics::ResolveResult],
    method_groups: &[Option<crate::il::transforms::MethodGroupOperand<'_>>],
    names: Option<&[Option<String>]>,
) -> Result<Vec<u8>, String> {
    use crate::il::transforms::LookupTarget;
    use crate::semantics::ResolveResult;
    query_message(ts, |w| {
        w.out.u8(8);
        w.call_method(method)?;
        w.out.len(type_arguments.len());
        for &t in type_arguments {
            w.ty(t);
        }
        // The type arguments that replace the method's.
        w.out.bool(method_args.is_some());
        if let Some(args) = method_args {
            w.out.len(args.len());
            for &t in args {
                w.ty(t);
            }
        }
        let operand = |w: &mut FunctionWriter<'_, '_, '_, '_>, rr: &ResolveResult| {
            match rr {
                ResolveResult::Expression(ty) => {
                    w.out.u8(0);
                    w.ty(*ty);
                }
                ResolveResult::Constant { ty, value } => {
                    w.out.u8(1);
                    w.ty(*ty);
                    super::ast::constant(w, value);
                }
                ResolveResult::ByReference {
                    element, ref_kind, ..
                } => {
                    w.out.u8(2);
                    w.ty(*element);
                    w.out.u8(*ref_kind as u8);
                }
                ResolveResult::OutVar(ty) => {
                    w.out.u8(3);
                    w.ty(*ty);
                }
                ResolveResult::Lambda(l) => {
                    let d = l.decompiled().ok_or("a lambda of another kind")?;
                    w.out.u8(4);
                    w.ty(d.delegate_type);
                    w.ty(d.return_type);
                    w.ty(d.inferred_return_type);
                    w.out.len(d.parameters.len());
                    for p in &d.parameters {
                        w.ty(p.ty);
                        w.string(&p.name);
                    }
                    w.out.bool(d.attempted_conversion_with_type_mismatch.get());
                    w.out.bool(d.implicitly_typed);
                    w.out.bool(d.anonymous_method);
                }
                _ => return Err("an operand of another kind".to_owned()),
            }
            Ok(())
        };
        operand(w, target)?;
        w.out.len(arguments.len());
        for (i, a) in arguments.iter().enumerate() {
            // 5: a method group, which the host looks up again (`Disambiguator.LookUpName`
            // without invocation, then `WithChosenMethod`).
            if let Some(Some(group)) = method_groups.get(i) {
                w.out.u8(5);
                w.call_method(group.method)?;
                match group.target {
                    // 4: a local function's group, by its name.
                    None if group.local_function.is_some() => {
                        w.out.u8(4);
                        w.string(group.local_function.unwrap_or_default());
                    }
                    None => w.out.u8(0),
                    Some(LookupTarget::This {
                        ty,
                        causes_non_virtual_invocation,
                    }) => {
                        w.out.u8(1);
                        w.out.bool(causes_non_virtual_invocation);
                        w.ty(ty);
                    }
                    Some(LookupTarget::Type(ty)) => {
                        w.out.u8(2);
                        w.ty(ty);
                    }
                    Some(LookupTarget::Expression(ty)) => {
                        w.out.u8(3);
                        w.ty(ty);
                    }
                }
                w.out.len(group.type_arguments.len());
                for &t in group.type_arguments {
                    w.ty(t);
                }
                continue;
            }
            operand(w, a)?;
        }
        match names {
            None => w.out.u8(0),
            Some(names) => {
                w.out.u8(1);
                for n in names {
                    w.out.bool(n.is_some());
                    if let Some(n) = n {
                        w.string(n);
                    }
                }
            }
        }
        Ok(())
    })
}

/// A `constructors` question in the `resolver_query` format: kind 7 and the constructor as
/// `call_method` writes it. The answer is encoded as for `method_group_query`.
pub fn constructors_query(
    ts: &TypeSystem<'_, '_>,
    constructor: &crate::il::ast::MethodOperand,
) -> Result<Vec<u8>, String> {
    query_message(ts, |w| {
        w.out.u8(7);
        w.call_method(constructor)?;
        Ok(())
    })
}

/// Encodes whether `types` are accessible from the current type (kind 12); the answer is 1
/// for yes.
pub fn types_accessible_query(
    ts: &TypeSystem<'_, '_>,
    types: &[TypeId],
) -> Result<Vec<u8>, String> {
    query_message(ts, |w| {
        w.out.u8(12);
        w.out.len(types.len());
        for &t in types {
            w.ty(t);
        }
        Ok(())
    })
}

/// Encodes the host's singularization of `word` (kind 9); the answer is the result in UTF-8.
pub fn singularize_query(ts: &TypeSystem<'_, '_>, word: &str) -> Result<Vec<u8>, String> {
    query_message(ts, |w| {
        w.out.u8(9);
        w.string(word);
        Ok(())
    })
}

/// Decodes a `method_group_query` answer.
pub fn method_group_answer(bytes: &[u8]) -> Option<Vec<crate::il::transforms::MethodGroupList>> {
    use crate::il::transforms::MethodGroupList;
    use crate::typesystem::compilation::{ModuleId, TypeDefId};
    use crate::typesystem::members::MethodDefId;
    let mut pos = 0;
    let mut uvar = || -> Option<u32> {
        let mut value = 0u32;
        let mut shift = 0;
        loop {
            let b = *bytes.get(pos)?;
            pos += 1;
            value |= u32::from(b & 0x7F).checked_shl(shift)?;
            if b & 0x80 == 0 {
                return Some(value);
            }
            shift += 7;
        }
    };
    if uvar()? == 0 {
        return None;
    }
    let lists = uvar()?;
    let mut result = Vec::with_capacity(lists as usize);
    for _ in 0..lists {
        let declaring = TypeDefId {
            module: ModuleId(uvar()?),
            row: uvar()?,
        };
        let count = uvar()?;
        let mut methods = Vec::with_capacity(count as usize);
        for _ in 0..count {
            methods.push(MethodDefId {
                module: ModuleId(uvar()?),
                row: uvar()?,
            });
        }
        result.push(MethodGroupList { declaring, methods });
    }
    Some(result)
}

/// A resolver question: the tables, the explicit generic contexts and what `write` adds.
fn query_message(
    ts: &TypeSystem<'_, '_>,
    write: impl FnOnce(&mut FunctionWriter<'_, '_, '_, '_>) -> Result<(), String>,
) -> Result<Vec<u8>, String> {
    let mut tables = Tables::default();
    let empty = Function::default();
    let mut w = FunctionWriter {
        ts,
        tables: &mut tables,
        f: &empty,
        out: Bytes::default(),
        vars: HashMap::new(),
        blocks: HashMap::new(),
        block_order: Vec::new(),
        containers: HashMap::new(),
        written: HashMap::new(),
        special_constants: true,
    };
    w.contexts();
    write(&mut w)?;
    let payload = std::mem::take(&mut w.out);
    Ok(tables.finish(VERSION, payload))
}

/// An entity request's message: the tables with the type syntax, then 1 and what `write`
/// writes of the declaration `convert` builds, or 0 and why the engine does not write it (the
/// host converts it, counted).
fn entity_message<T>(
    ts: &mut TypeSystem<'_, '_>,
    options: crate::csharp::entity_syntax::EntityOptions,
    convert: impl FnOnce(&mut TypeSystem<'_, '_>) -> Result<T, String>,
    write: impl FnOnce(&mut FunctionWriter<'_, '_, '_, '_>, &T),
) -> Result<Vec<u8>, String> {
    // The types the type and constant syntax name, which the writers only find.
    ts.intern(crate::typesystem::types::TypeData::Unbound);
    ts.intern_constant_types();
    let syntax = convert(ts);
    ts.intern_oblivious_instances();
    let ts = &*ts;
    let mut tables = Tables::default();
    let empty = Function::default();
    let mut w = FunctionWriter {
        ts,
        tables: &mut tables,
        f: &empty,
        out: Bytes::default(),
        vars: HashMap::new(),
        blocks: HashMap::new(),
        block_order: Vec::new(),
        containers: HashMap::new(),
        written: HashMap::new(),
        special_constants: true,
    };
    match syntax {
        Err(reason) => {
            w.out.u8(0);
            w.string(&reason);
        }
        Ok(s) => {
            w.out.u8(1);
            write(&mut w, &s);
        }
    }
    let payload = std::mem::take(&mut w.out);
    tables.type_syntax(ts, options.nullable_specifier);
    Ok(tables.finish(VERSION, payload))
}

/// `TypeSystemAstBuilder.ConvertEntity` of a field (`csharp::entity_syntax`) as a message.
pub fn field_syntax(
    ts: &mut TypeSystem<'_, '_>,
    module: crate::typesystem::compilation::ModuleId,
    row: u32,
    options: crate::csharp::entity_syntax::EntityOptions,
) -> Result<Vec<u8>, String> {
    entity_message(
        ts,
        options,
        |ts| crate::csharp::entity_syntax::convert_field(ts, module, row, options),
        |w, field| {
            w.out.uvar(u64::from(field.modifiers));
            write_attributes(w, &field.attributes);
            w.ty(field.return_type);
            w.out.bool(field.ref_readonly);
            write_optional_constant(w, field.initializer.as_ref());
        },
    )
}

/// `TypeSystemAstBuilder.ConvertEntity` of a method, operator, constructor or destructor
/// (`csharp::entity_syntax`) as a message.
pub fn method_syntax(
    ts: &mut TypeSystem<'_, '_>,
    module: crate::typesystem::compilation::ModuleId,
    row: u32,
    options: crate::csharp::entity_syntax::EntityOptions,
) -> Result<Vec<u8>, String> {
    use crate::csharp::entity_syntax::{ConstraintTypeSyntax, MethodKindSyntax};
    entity_message(
        ts,
        options,
        |ts| crate::csharp::entity_syntax::convert_method(ts, module, row, options),
        |w, m| {
            match &m.kind {
                MethodKindSyntax::Method => w.out.u8(0),
                MethodKindSyntax::Operator(name) => {
                    w.out.u8(1);
                    w.string(name);
                }
                MethodKindSyntax::Constructor(name) => {
                    w.out.u8(2);
                    w.string(name);
                }
                MethodKindSyntax::Destructor(name) => {
                    w.out.u8(3);
                    w.string(name);
                }
            }
            w.out.uvar(u64::from(m.modifiers));
            write_attributes(w, &m.attributes);
            write_attributes(w, &m.return_attributes);
            match m.return_type {
                Some(t) => {
                    w.out.u8(1);
                    w.ty(t);
                }
                None => w.out.u8(0),
            }
            w.out.bool(m.ref_readonly);
            w.string(&m.name);
            w.out.len(m.type_parameters.len());
            for tp in &m.type_parameters {
                w.out.u8(tp.variance);
                w.string(&tp.name);
                write_attributes(w, &tp.attributes);
            }
            write_parameters(w, &m.parameters);
            w.out.bool(m.this_modifier);
            w.out.len(m.constraints.len());
            for c in &m.constraints {
                w.string(&c.type_parameter);
                w.out.len(c.base_types.len());
                for b in &c.base_types {
                    match b {
                        ConstraintTypeSyntax::Keyword(k, nullable) => {
                            w.out.u8(0);
                            w.string(k);
                            w.out.bool(*nullable);
                        }
                        ConstraintTypeSyntax::Type(t) => {
                            w.out.u8(1);
                            w.ty(*t);
                        }
                    }
                }
            }
            match m.private_implementation_type {
                Some(t) => {
                    w.out.u8(1);
                    w.ty(t);
                }
                None => w.out.u8(0),
            }
        },
    )
}

/// `TypeSystemAstBuilder.ConvertEntity` of a property, indexer or event
/// (`csharp::entity_syntax`) as a message.
pub fn property_syntax(
    ts: &mut TypeSystem<'_, '_>,
    token: u32,
    options: crate::csharp::entity_syntax::EntityOptions,
    custom_events: bool,
    init_accessors: bool,
) -> Result<Vec<u8>, String> {
    let module = crate::typesystem::compilation::ModuleId(0);
    let row = token & 0x00FF_FFFF;
    entity_message(
        ts,
        options,
        |ts| {
            if token >> 24 == 0x14 {
                crate::csharp::entity_syntax::convert_event(ts, module, row, options, custom_events)
            } else {
                crate::csharp::entity_syntax::convert_property(
                    ts,
                    module,
                    row,
                    options,
                    init_accessors,
                )
            }
        },
        |w, p| {
            w.out.u8(p.kind);
            w.out.uvar(u64::from(p.modifiers));
            write_attributes(w, &p.attributes);
            w.ty(p.return_type);
            w.out.bool(p.ref_readonly);
            w.string(&p.name);
            write_parameters(w, &p.parameters);
            for a in &p.accessors {
                match a {
                    Some(a) => {
                        w.out.u8(1);
                        w.out.uvar(u64::from(a.row));
                        w.out.u8(a.kind);
                        w.out.bool(a.is_init_only);
                        w.out.uvar(u64::from(a.modifiers));
                        write_attributes(w, &a.attributes);
                        write_attributes(w, &a.return_attributes);
                        write_attributes(w, &a.parameter_attributes);
                    }
                    None => w.out.u8(0),
                }
            }
            match p.private_implementation_type {
                Some(t) => {
                    w.out.u8(1);
                    w.ty(t);
                }
                None => w.out.u8(0),
            }
        },
    )
}

/// `TypeSystemAstBuilder.ConvertEntity` of a type definition (`csharp::entity_syntax`) as a
/// message.
pub fn type_syntax(
    ts: &mut TypeSystem<'_, '_>,
    row: u32,
    options: crate::csharp::entity_syntax::EntityOptions,
    record_classes: bool,
    record_structs: bool,
) -> Result<Vec<u8>, String> {
    let module = crate::typesystem::compilation::ModuleId(0);
    entity_message(
        ts,
        options,
        |ts| {
            crate::csharp::entity_syntax::convert_type_definition(
                ts,
                module,
                row,
                options,
                record_classes,
                record_structs,
            )
        },
        |w, t| {
            match t.class_type {
                Some(c) => {
                    w.out.u8(1);
                    w.out.u8(c);
                }
                None => w.out.u8(0),
            }
            w.out.uvar(u64::from(t.modifiers));
            write_attributes(w, &t.attributes);
            w.string(&t.name);
            w.out.len(t.type_parameters.len());
            for tp in &t.type_parameters {
                w.out.u8(tp.variance);
                w.string(&tp.name);
                write_attributes(w, &tp.attributes);
            }
            w.out.len(t.base_types.len());
            for &b in &t.base_types {
                w.ty(b);
            }
            write_constraints(w, &t.constraints);
            write_attributes(w, &t.return_attributes);
            match t.return_type {
                Some(r) => {
                    w.out.u8(1);
                    w.ty(r);
                }
                None => w.out.u8(0),
            }
            w.out.bool(t.ref_readonly);
            write_parameters(w, &t.parameters);
        },
    )
}

/// `TypeSystemAstBuilder.ConvertExtension` of the extension block of a marker type
/// (`csharp::entity_syntax`) as a message.
pub fn extension_syntax(
    ts: &mut TypeSystem<'_, '_>,
    row: u32,
    options: crate::csharp::entity_syntax::EntityOptions,
) -> Result<Vec<u8>, String> {
    let module = crate::typesystem::compilation::ModuleId(0);
    entity_message(
        ts,
        options,
        |ts| crate::csharp::entity_syntax::convert_extension(ts, module, row, options),
        |w, e| {
            w.out.len(e.type_parameters.len());
            for tp in &e.type_parameters {
                w.out.u8(tp.variance);
                w.string(&tp.name);
                write_attributes(w, &tp.attributes);
            }
            write_parameters(w, std::slice::from_ref(&e.receiver));
            write_constraints(w, &e.constraints);
        },
    )
}

fn write_constraints(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    constraints: &[crate::csharp::entity_syntax::ConstraintSyntax],
) {
    use crate::csharp::entity_syntax::ConstraintTypeSyntax;
    w.out.len(constraints.len());
    for c in constraints {
        w.string(&c.type_parameter);
        w.out.len(c.base_types.len());
        for b in &c.base_types {
            match b {
                ConstraintTypeSyntax::Keyword(k, nullable) => {
                    w.out.u8(0);
                    w.string(k);
                    w.out.bool(*nullable);
                }
                ConstraintTypeSyntax::Type(t) => {
                    w.out.u8(1);
                    w.ty(*t);
                }
            }
        }
    }
}

fn write_parameters(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    parameters: &[crate::csharp::entity_syntax::ParameterSyntax],
) {
    w.out.len(parameters.len());
    for p in parameters {
        w.out.u8(p.ref_kind);
        w.out.bool(p.is_params);
        w.out.bool(p.is_scoped);
        write_attributes(w, &p.attributes);
        w.ty(p.ty);
        w.string(&p.name);
        write_optional_constant(w, p.default_value.as_ref());
    }
}

fn write_attributes(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    attributes: &[crate::csharp::entity_syntax::AttributeSyntax],
) {
    w.out.len(attributes.len());
    for a in attributes {
        write_attribute(w, a);
    }
}

fn write_optional_constant(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    c: Option<&crate::csharp::constant_syntax::ConstSyntax>,
) {
    match c {
        Some(c) => {
            w.out.u8(1);
            super::ast::constant_syntax_node(w, c);
        }
        None => w.out.u8(0),
    }
}

/// One attribute of `csharp::entity_syntax`.
fn write_attribute(
    w: &mut FunctionWriter<'_, '_, '_, '_>,
    a: &crate::csharp::entity_syntax::AttributeSyntax,
) {
    w.ty(a.ty);
    match &a.identifier {
        Some(id) => {
            w.out.u8(1);
            w.string(id);
        }
        None => w.out.u8(0),
    }
    match a.constructor {
        Some((declaring, module, row)) => {
            w.out.u8(1);
            w.ty(declaring);
            w.out.len(module.0 as usize);
            w.out.uvar(u64::from(row));
        }
        None => w.out.u8(0),
    }
    w.out.len(a.arguments.len());
    for c in &a.arguments {
        super::ast::constant_syntax_node(w, c);
    }
    w.out.len(a.named.len());
    for n in &a.named {
        w.string(&n.name);
        w.out.bool(n.is_field);
        w.ty(n.ty);
        super::ast::constant_syntax_node(w, &n.value);
    }
    w.out.bool(a.has_decode_errors);
}

/// The variables of the nested functions (lambdas) in `f`, in tree order.
fn nested_variables(f: &crate::il::ast::Function) -> Vec<VarId> {
    // The body, then the top-level function's local functions.
    f.body
        .into_iter()
        .chain(f.local_functions.iter().copied())
        .flat_map(|root| f.descendants_post(root).into_iter().rev())
        .filter_map(|n| match f.inst(n) {
            Inst::Function { variables, .. } => Some(variables.clone()),
            _ => None,
        })
        .flatten()
        .collect()
}
