//! `IL/ILReader.cs`: decodes a method body into an ILAst function.
//!
//! The structure follows the managed reader: blocks are imported from a queue with an input
//! stack each (re-imported when a merge widens a stack slot type), instructions that produce a
//! value wait on an expression stack until something forces them into stack slot variables,
//! and afterwards the stack slots of every edge are unified and named.

use std::collections::{HashMap, HashSet, VecDeque};

use crate::metadata::bytes::Bytes;
use crate::metadata::il_body::{ExceptionClause, MethodBody};
use crate::metadata::signature;
use crate::metadata::tables::rows;
use crate::metadata::{Error, Result};
use crate::typesystem::attrs::AttributeSource;
use crate::typesystem::compilation::{ModuleId, TypeDefId, TypeKind};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{MethodDefId, RefKind};
use crate::typesystem::options;
use crate::typesystem::resolve::{FieldOrigin, ResolvedField, ResolvedMethod};
use crate::typesystem::types::{GenericContext, Nullability, TypeData, TypeId, TypeSystem};
use crate::util::bitset::BitSet;
use crate::util::interval::Interval;
use crate::util::interval::LongInterval;
use crate::util::long_set::LongSet;

use super::ast::*;
use super::block_builder::{BlockBuilder, topological_sort};
use super::debug_info::DebugInfo;
use super::opcodes::{Code, set_branch_targets};
use super::types::{PrimitiveType, Sign, StackType};

/// `SRMExtensions.MethodImplAsync`: the method body uses runtime async.
const METHOD_IMPL_ASYNC: u16 = 0x2000;

/// Reader settings that change the output (`ILReader` properties).
#[derive(Clone, Copy, Default)]
pub struct ReaderOptions<'d> {
    pub use_ref_locals_for_accurate_order_of_evaluation: bool,
    /// `DebugInfo` when `UseDebugSymbols` is set: local names and types from the PDB.
    pub debug_info: Option<&'d dyn DebugInfo>,
}

struct ImportedBlock {
    block: NodeId,
    start: i32,
    /// Bottom to top.
    input_stack: Vec<VarId>,
    import_started: bool,
    outgoing: Vec<(usize, Vec<VarId>)>,
}

struct Decoded {
    inst: NodeId,
    pushed: bool,
}

/// The result of reading a method: the function and what `ILReader.ReadIL` reports besides.
pub struct ReadFunction {
    /// `ILFunction.AsyncReturnType`: the unpacked task type of a runtime async method.
    pub async_return_type: Option<TypeId>,
    pub function: Function,
    /// `ILFunction.Variables`, in order.
    pub variables: Vec<VarId>,
    pub code_size: i32,
    pub local_signature_length: usize,
    pub warnings: Vec<String>,
    pub sequence_point_candidates: Vec<i32>,
}

/// The kind of a type as the reader distinguishes it (`IType.Kind`).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Kind {
    Array,
    TypeParameter,
    Unknown,
    Def(TypeKind),
    Other,
}

pub(super) struct Reader<'r, 'c, 'a> {
    ts: &'r mut TypeSystem<'c, 'a>,
    module: ModuleId,
    method: ResolvedMethod,
    method_id: MethodDefId,
    ctx: GenericContext,
    options: ReaderOptions<'r>,
    body: MethodBody<'a>,
    code: Code<'a>,
    pub(super) f: Function,
    pub(super) warnings: Vec<String>,
    sequence_point_candidates: Vec<i32>,
    method_return_stack_type: StackType,
    current_stack: Vec<VarId>,
    current_block: Option<usize>,
    expression_stack: Vec<NodeId>,
    parameter_variables: Vec<VarId>,
    local_variables: Vec<VarId>,
    is_branch_target: BitSet,
    pub(super) main_container: NodeId,
    current_instruction_start: i32,
    blocks: Vec<ImportedBlock>,
    block_by_offset: HashMap<i32, usize>,
    import_queue: VecDeque<usize>,
    /// Per exception clause index.
    pub(super) variable_by_handler: Vec<Option<VarId>>,
    stack_variables: Vec<VarId>,
    constrained_prefix: Option<TypeOperand>,
    /// `ILFunction.AsyncReturnType`, set for runtime async methods.
    async_return_type: Option<TypeId>,
}

fn ldc_i4(f: &mut Function, value: i32) -> NodeId {
    f.add(Inst::LdcI4 { value })
}

fn conv(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    arg: NodeId,
    target: PrimitiveType,
    check: bool,
    sign: Sign,
) -> NodeId {
    let input = f.result_type(ts, arg);
    let needs_sign = check || (!input.is_float() && target.is_float());
    f.add(Inst::Conv {
        arg,
        input,
        input_sign: if needs_sign { sign } else { Sign::None },
        target,
        check_overflow: check,
    })
}

/// `Comp.LogicNot`: `comp.i4(arg == ldc.i4 0)`.
fn logic_not(f: &mut Function, arg: NodeId) -> NodeId {
    let zero = ldc_i4(f, 0);
    f.add(Inst::Comp {
        kind: ComparisonKind::Equality,
        sign: Sign::None,
        input: StackType::I4,
        left: arg,
        right: zero,
    })
}

impl<'r, 'c, 'a> Reader<'r, 'c, 'a> {
    fn add(&mut self, inst: Inst) -> NodeId {
        self.f.add(inst)
    }

    fn result_type(&mut self, id: NodeId) -> StackType {
        self.f.result_type(self.ts, id)
    }

    fn conv(&mut self, arg: NodeId, target: PrimitiveType, check: bool, sign: Sign) -> NodeId {
        conv(&mut self.f, self.ts, arg, target, check, sign)
    }

    fn invalid_branch(&mut self, message: &str) -> NodeId {
        self.add(Inst::InvalidBranch {
            message: message.to_owned(),
            expected: StackType::Void,
        })
    }

    fn invalid_expression(&mut self, message: &str) -> NodeId {
        self.add(Inst::InvalidExpression {
            message: message.to_owned(),
            expected: StackType::Unknown,
            severity: "Error".to_owned(),
        })
    }

    fn warn(&mut self, message: &str) {
        self.warnings.push(format!(
            "IL_{:04x}: {message}",
            self.current_instruction_start
        ));
    }

    fn kind(&mut self, t: TypeId) -> Kind {
        match self.ts.data(t).clone() {
            TypeData::Array { .. } => Kind::Array,
            TypeData::TypeParam { .. } => Kind::TypeParameter,
            TypeData::Annotated { inner, .. } => self.kind(inner),
            TypeData::Unknown { .. } | TypeData::UnknownSpecial => Kind::Unknown,
            TypeData::Def(_) | TypeData::Inst { .. } | TypeData::Tuple { .. } => {
                match self.ts.definition(t) {
                    Some(d) => self
                        .ts
                        .comp
                        .type_def(d)
                        .map_or(Kind::Other, |i| Kind::Def(i.kind)),
                    None => Kind::Unknown,
                }
            }
            _ => Kind::Other,
        }
    }

    fn type_operand(&mut self, token: u32) -> Result<TypeOperand> {
        let ty = self.ts.resolve_token(self.module, token, self.ctx)?;
        Ok(TypeOperand { token, ty })
    }

    fn known(&mut self, code: KnownTypeCode) -> TypeOperand {
        TypeOperand {
            token: 0,
            ty: self.ts.known_type(code),
        }
    }

    /// `ReadAndDecodeMetadataToken`.
    fn read_token(&mut self) -> Result<u32> {
        let token = self.code.i32()?;
        if token <= 0 || token & 0x00FF_FFFF == 0 {
            return Err(Error::Malformed("Invalid metadata token"));
        }
        Ok(token as u32)
    }

    fn read_type(&mut self) -> Result<TypeOperand> {
        let token = self.read_token()?;
        self.type_operand(token)
    }

    fn read_method(&mut self) -> Result<MethodOperand> {
        let token = self.read_token()?;
        let method = self.ts.resolve_method(self.module, token, self.ctx)?;
        Ok(MethodOperand { token, method })
    }

    /// `ResolveEntity(token) as IField`.
    fn resolve_field(&mut self, token: u32) -> Result<Option<ResolvedField>> {
        let row = token & 0x00FF_FFFF;
        match token >> 24 {
            0x04 => {
                let md = &self.ts.comp.module(self.module).md;
                let f = md.get::<rows::Field>(row)?;
                let name = md.string(f.name)?.into_owned();
                let declaring = md.field_declaring_type(row)?;
                let declaring = (declaring != 0).then(|| {
                    self.ts.intern_def(TypeDefId {
                        module: self.module,
                        row: declaring,
                    })
                });
                let (ty, _) = self.ts.field_type(self.module, row);
                Ok(Some(ResolvedField {
                    origin: FieldOrigin::Def {
                        module: self.module,
                        row,
                    },
                    declaring,
                    name,
                    ty,
                }))
            }
            0x0A => {
                let md = &self.ts.comp.module(self.module).md;
                let mr = md.get::<rows::MemberRef>(row)?;
                let blob = md.blob(mr.signature)?;
                if blob.first().map(|h| h & 0x0F) != Some(0x06) {
                    return Ok(None);
                }
                Ok(Some(self.ts.resolve_field_ref(
                    self.module,
                    row,
                    self.ctx,
                )?))
            }
            _ => Ok(None),
        }
    }

    fn read_field(&mut self) -> Result<FieldOperand> {
        let token = self.read_token()?;
        let field = self
            .resolve_field(token)?
            .ok_or(Error::Malformed("Invalid field token"))?;
        Ok(FieldOperand { token, field })
    }

    fn new_variable(
        &mut self,
        kind: VariableKind,
        ty: TypeId,
        stack_type: StackType,
        index: Option<i32>,
    ) -> VarId {
        let parameter = kind == VariableKind::Parameter;
        self.f.add_var(Variable {
            kind,
            ty,
            stack_type,
            index,
            name: String::new(),
            has_generated_name: false,
            initial_value_is_initialized: parameter,
            uses_initial_value: parameter,
            is_ref_read_only: false,
        })
    }

    /// `ILReader.Init`.
    pub(super) fn new(
        ts: &'r mut TypeSystem<'c, 'a>,
        method_id: MethodDefId,
        body: MethodBody<'a>,
        options: ReaderOptions<'r>,
    ) -> Result<Self> {
        let module = method_id.module;
        let method = ts.method_definition(method_id)?;
        let ctx = ts.method_context(method_id)?;
        let impl_flags = ts
            .comp
            .module(module)
            .md
            .get::<rows::MethodDef>(method_id.row)?
            .impl_flags;
        let is_runtime_async =
            ts.options(module) & options::RUNTIME_ASYNC != 0 && impl_flags & METHOD_IMPL_ASYNC != 0;
        let ret = if is_runtime_async {
            ts.unpack_any_task(method.ret)
        } else {
            method.ret
        };
        let method_return_stack_type = ts.stack_type(ret);
        let mut f = Function::default();
        let main_container = f.add(Inst::BlockContainer {
            kind: ContainerKind::Normal,
            expected: Some(ret),
            blocks: Vec::new(),
        });
        let code = Code::new(body.code);
        let len = body.code.len();
        let clause_count = body.clauses.len();
        let mut reader = Reader {
            ts,
            module,
            method,
            method_id,
            ctx,
            options,
            body,
            code,
            f,
            warnings: Vec::new(),
            sequence_point_candidates: Vec::new(),
            method_return_stack_type,
            current_stack: Vec::new(),
            current_block: None,
            expression_stack: Vec::new(),
            parameter_variables: Vec::new(),
            local_variables: Vec::new(),
            is_branch_target: BitSet::new(len),
            main_container,
            current_instruction_start: 0,
            blocks: Vec::new(),
            block_by_offset: HashMap::new(),
            import_queue: VecDeque::new(),
            variable_by_handler: vec![None; clause_count],
            stack_variables: Vec::new(),
            constrained_prefix: None,
            async_return_type: is_runtime_async.then_some(ret),
        };
        reader.init_parameter_variables()?;
        reader.init_local_variables()?;
        let locals_initialized = reader.body.init_locals;
        for &v in &reader.local_variables.clone() {
            let var = reader.f.var_mut(v);
            var.initial_value_is_initialized = locals_initialized;
            var.uses_initial_value = true;
        }
        Ok(reader)
    }

    fn init_parameter_variables(&mut self) -> Result<()> {
        let (_, params) = self.ts.method_signature(self.method_id);
        let is_static = self.method.is_static;
        let mut count = params.len() + usize::from(!is_static);
        if params
            .last()
            .is_some_and(|p| matches!(self.ts.data(p.ty), TypeData::ArgList))
        {
            count -= 1;
        }
        if !is_static {
            let declaring = self
                .method
                .declaring
                .ok_or(Error::Malformed("method without declaring type"))?;
            let declaring = match *self.ts.data(declaring) {
                TypeData::Def(def) if self.ts.comp.type_def(def)?.type_parameter_count > 0 => {
                    let args: Vec<TypeId> = (0..self.ts.comp.type_def(def)?.type_parameter_count)
                        .map(|i| {
                            let owner = self.ts.class_param_owner(def, i);
                            self.ts.intern(TypeData::TypeParam { owner, index: i })
                        })
                        .collect();
                    self.ts.intern(TypeData::Inst {
                        generic: declaring,
                        args: args.into(),
                    })
                }
                _ => declaring,
            };
            let owner = self.ts.accessor_owner(self.method_id).map(|(o, _)| o);
            let this_read_only = self.ts.method_flags(self.method_id, owner).this_read_only;
            let v = self.parameter_variable(-1, declaring, "this");
            self.f.var_mut(v).is_ref_read_only = this_read_only;
            self.parameter_variables.push(v);
        }
        let offset = usize::from(!is_static);
        for p in params.iter().take(count - offset) {
            let index = self.parameter_variables.len() - offset;
            let v = self.parameter_variable(index as i32, p.ty, &p.name);
            self.f.var_mut(v).is_ref_read_only =
                matches!(p.ref_kind, RefKind::In | RefKind::RefReadOnly);
            self.parameter_variables.push(v);
        }
        Ok(())
    }

    /// `CreateILVariable(index, parameterType, name)`.
    fn parameter_variable(&mut self, index: i32, ty: TypeId, name: &str) -> VarId {
        let mut ty = ty;
        if index < 0
            && let Some(def) = self.ts.definition(ty)
            && !matches!(self.ts.data(ty), TypeData::FnPtr(_))
        {
            let def_type = self.ts.intern_def(def);
            if self.ts.is_reference_type(def_type) == Some(false) {
                ty = self.ts.intern(TypeData::ByRef(ty));
            }
        }
        let stack_type = self.ts.stack_type(ty);
        let v = self.new_variable(VariableKind::Parameter, ty, stack_type, Some(index));
        let var = self.f.var_mut(v);
        if index < 0 {
            var.name = "this".to_owned();
        } else if name.trim().is_empty() {
            var.name = format!("P_{index}");
            var.has_generated_name = true;
        } else {
            var.name = name.to_owned();
        }
        v
    }

    fn init_local_variables(&mut self) -> Result<()> {
        if self.body.local_sig == 0 {
            return Ok(());
        }
        let types = match self.decode_local_signature() {
            Ok(types) => types,
            Err(e) => {
                self.warnings
                    .push(format!("Error decoding local variables: {e}"));
                Vec::new()
            }
        };
        let token = 0x0600_0000 | self.method_id.row;
        let debug_info = self.options.debug_info;
        for (index, ty) in types.into_iter().enumerate() {
            let index = index as i32;
            let (kind, mut ty) = match *self.ts.data(self.skip_modifiers(ty)) {
                TypeData::Pinned(elem) => (VariableKind::PinnedLocal, elem),
                _ => (VariableKind::Local, ty),
            };
            if let Some(extra) = debug_info.and_then(|d| d.extra_type_info(token, index)) {
                let options = self.ts.options(self.module);
                ty = self.ts.apply_pdb_extra_type_info(
                    ty,
                    options,
                    extra.dynamic_flags,
                    extra.tuple_element_names,
                );
            }
            let stack_type = self.ts.stack_type(ty);
            let v = self.new_variable(kind, ty, stack_type, Some(index));
            // `string.IsNullOrWhiteSpace` names count as missing.
            let name = debug_info
                .and_then(|d| d.local_name(token, index))
                .filter(|n| !n.chars().all(char::is_whitespace));
            let var = self.f.var_mut(v);
            var.has_generated_name = name.is_none();
            var.name = name.unwrap_or_else(|| format!("V_{index}"));
            self.local_variables.push(v);
        }
        Ok(())
    }

    fn skip_modifiers(&self, mut t: TypeId) -> TypeId {
        while let TypeData::Modified { inner, .. } = self.ts.data(t) {
            t = *inner;
        }
        t
    }

    /// `MetadataModule.DecodeLocalSignature`.
    fn decode_local_signature(&mut self) -> Result<Vec<TypeId>> {
        let md = &self.ts.comp.module(self.module).md;
        let sig = md.get::<rows::StandAloneSig>(self.body.local_sig)?;
        let blob = md.blob(sig.signature)?;
        if blob.first().map(|h| h & 0x0F) != Some(signature::HEADER_KIND_LOCALS) {
            return Err(Error::Malformed("Expected LocalVariables signature"));
        }
        let locals = signature::decode_locals(&mut Bytes::new(blob))?;
        let options = self.ts.options(self.module);
        let mut types = Vec::with_capacity(locals.len());
        for local in &locals {
            let raw = self.ts.from_sig_raw(self.module, local, self.ctx)?;
            types.push(self.ts.apply_attributes(
                raw,
                self.module,
                AttributeSource::None,
                options,
                Nullability::Oblivious,
                false,
            )?);
        }
        Ok(types)
    }

    /// `ImportedBlock.MergeStackTypes`.
    fn merge_stack_types(&mut self, index: usize, new_edge: &[VarId]) -> bool {
        let existing = self.blocks[index].input_stack.clone();
        let mut changed = false;
        for (a, b) in existing.iter().rev().zip(new_edge.iter().rev()) {
            if self.f.var(*a).stack_type < self.f.var(*b).stack_type {
                changed = true;
            }
        }
        if !changed || existing.len() != new_edge.len() {
            return false;
        }
        let merged: Vec<VarId> = existing
            .iter()
            .zip(new_edge)
            .map(|(&a, &b)| {
                if self.f.var(a).stack_type < self.f.var(b).stack_type {
                    b
                } else {
                    a
                }
            })
            .collect();
        let block = &mut self.blocks[index];
        block.input_stack = merged;
        block.import_started = false;
        true
    }

    /// `StoreStackForOffset`.
    fn store_stack_for_offset(&mut self, offset: i32, stack: Vec<VarId>) -> usize {
        if let Some(&existing) = self.block_by_offset.get(&offset) {
            let was_imported = self.blocks[existing].import_started;
            if self.merge_stack_types(existing, &stack) && was_imported {
                self.import_queue.push_back(existing);
            }
            return existing;
        }
        let block = self.new_block(offset);
        self.blocks.push(ImportedBlock {
            block,
            start: offset,
            input_stack: stack,
            import_started: false,
            outgoing: Vec::new(),
        });
        let index = self.blocks.len() - 1;
        self.block_by_offset.insert(offset, index);
        self.import_queue.push_back(index);
        index
    }

    pub(super) fn new_block(&mut self, offset: i32) -> NodeId {
        let final_inst = self.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let block = self.add(Inst::Block {
            kind: BlockKind::ControlFlow,
            instructions: Vec::new(),
            final_inst,
        });
        self.f.add_il_range(block, Interval::new(offset, offset));
        block
    }

    /// `ReadInstructions`.
    pub(super) fn read_instructions(&mut self) -> Result<()> {
        self.store_stack_for_offset(0, Vec::new());
        if self.code.is_empty() {
            let block = self.blocks[0].block;
            let inst = self.invalid_branch(
                "Empty body found. Decompiled assembly might be a reference assembly.",
            );
            self.f.push(block, inst);
            return Ok(());
        }
        set_branch_targets(self.body.code, &mut self.is_branch_target);
        self.prepare_branch_targets_and_stacks_for_exception_handlers()?;
        while let Some(index) = self.import_queue.pop_front() {
            self.read_block(index)?;
        }
        self.ensure_exception_handlers_have_blocks();
        let union_find = self.check_outgoing_edges()?;
        self.collect_stack_variables(union_find);
        Ok(())
    }

    fn read_block(&mut self, index: usize) -> Result<()> {
        let block = self.blocks[index].block;
        self.f.set_list(block, Vec::new());
        self.blocks[index].outgoing.clear();
        self.blocks[index].import_started = true;
        self.code.offset = self.blocks[index].start as usize;
        self.current_block = Some(index);
        self.current_stack = self.blocks[index].input_stack.clone();
        while self.code.remaining() > 0 {
            let start = self.code.offset as i32;
            self.current_instruction_start = start;
            let started_with_empty_stack = self.current_stack_is_empty();
            let decoded = match self.decode_instruction() {
                Ok(d) => d,
                Err(Error::Malformed(message)) | Err(Error::Unsupported(message)) => Decoded {
                    inst: self.invalid_branch(message),
                    pushed: false,
                },
                Err(e) => return Err(e),
            };
            let inst = decoded.inst;
            let ty = self.result_type(inst);
            let is_invalid = matches!(
                self.f.inst(inst),
                Inst::InvalidBranch { .. } | Inst::InvalidExpression { .. }
            );
            if ty == StackType::Unknown && !is_invalid {
                self.warn("Unknown result type (might be due to invalid IL or missing references)");
            }
            let end = self.code.offset as i32;
            self.f.add_il_range(inst, Interval::new(start, end));
            if !decoded.pushed {
                self.flush_expression_stack();
                self.f.push(block, inst);
            }
            if (!decoded.pushed && self.is_sequence_point_instruction(inst))
                || started_with_empty_stack
            {
                self.sequence_point_candidates
                    .push(self.f.il_range(inst).start);
            }
            if self.f.has_direct_flag(inst, Flags::END_POINT_UNREACHABLE) {
                break;
            }
            let branch_target = self
                .is_branch_target
                .get(end as usize)
                .ok_or(Error::Internal(
                    "Index was outside the bounds of the array.",
                ))?;
            if branch_target || self.f.has_flag(inst, Flags::MAY_BRANCH) {
                break;
            }
        }
        self.flush_expression_stack();
        let start = self.blocks[index].start;
        self.f
            .add_il_range(block, Interval::new(start, self.code.offset as i32));
        if !self.f.has_flag(block, Flags::END_POINT_UNREACHABLE) {
            let branch = if self.code.remaining() > 0 {
                let target = self.code.offset as i32;
                self.mark_branch_target(target);
                self.add(Inst::Branch {
                    target: BranchTarget::Offset(target),
                })
            } else {
                self.invalid_branch("End of method reached without returning.")
            };
            let last = self.f.list(block).last().copied();
            let switch_with_nop_default = last.and_then(|l| match self.f.inst(l) {
                Inst::Switch { sections, .. } => sections.last().copied(),
                _ => None,
            });
            match switch_with_nop_default {
                Some(section)
                    if matches!(self.f.inst(section), Inst::SwitchSection { body, .. }
                        if matches!(self.f.inst(*body), Inst::Nop { .. })) =>
                {
                    let Inst::SwitchSection { body, .. } = *self.f.inst(section) else {
                        unreachable!()
                    };
                    self.f.replace(body, branch);
                }
                _ => self.f.push(block, branch),
            }
        }
        Ok(())
    }

    fn current_stack_is_empty(&self) -> bool {
        self.current_stack.is_empty() && self.expression_stack.is_empty()
    }

    fn prepare_branch_targets_and_stacks_for_exception_handlers(&mut self) -> Result<()> {
        let clauses: Vec<ExceptionClause> = self.body.clauses.clone();
        for (i, eh) in clauses.iter().enumerate() {
            self.is_branch_target.set(eh.try_offset as usize);
            let stack = match eh.kind & 7 {
                0 => {
                    let catch_type =
                        self.ts
                            .resolve_token(self.module, eh.class_token_or_filter, self.ctx)?;
                    vec![self.exception_variable(i, catch_type, eh.handler_offset as i32)]
                }
                1 => {
                    let object = self.ts.known_type(KnownTypeCode::Object);
                    vec![self.exception_variable(i, object, eh.handler_offset as i32)]
                }
                _ => Vec::new(),
            };
            if eh.kind & 7 == 1 {
                let filter = eh.class_token_or_filter as i32;
                self.is_branch_target.set(filter as usize);
                self.store_stack_for_offset(filter, stack.clone());
            }
            self.is_branch_target.set(eh.handler_offset as usize);
            self.store_stack_for_offset(eh.handler_offset as i32, stack);
        }
        Ok(())
    }

    fn exception_variable(&mut self, clause: usize, ty: TypeId, handler: i32) -> VarId {
        let stack_type = self.ts.stack_type(ty);
        let v = self.new_variable(
            VariableKind::ExceptionStackSlot,
            ty,
            stack_type,
            Some(handler),
        );
        let var = self.f.var_mut(v);
        var.name = format!("E_{handler}");
        var.has_generated_name = true;
        self.variable_by_handler[clause] = Some(v);
        v
    }

    fn ensure_exception_handlers_have_blocks(&mut self) {
        let offsets: Vec<i32> = self
            .body
            .clauses
            .iter()
            .map(|c| c.try_offset as i32)
            .collect();
        for offset in offsets {
            if self.block_by_offset.contains_key(&offset) {
                continue;
            }
            let block = self.new_block(offset);
            let inst = self.invalid_branch("Unreachable try block");
            self.f.push(block, inst);
            self.blocks.push(ImportedBlock {
                block,
                start: offset,
                input_stack: Vec::new(),
                import_started: false,
                outgoing: Vec::new(),
            });
            self.block_by_offset.insert(offset, self.blocks.len() - 1);
        }
    }

    fn is_sequence_point_instruction(&self, inst: NodeId) -> bool {
        matches!(
            self.f.inst(inst),
            Inst::Nop { .. } | Inst::Call(_) | Inst::CallIndirect { .. } | Inst::CallVirt(_)
        )
    }

    /// `CheckOutgoingEdges`: the union-find of stack slots that meet on an edge, with stack
    /// adjustments inserted where the types differ.
    fn check_outgoing_edges(&mut self) -> Result<UnionFind> {
        let mut union_find = UnionFind::default();
        for index in 0..self.blocks.len() {
            let edges = self.blocks[index].outgoing.clone();
            for (target, stack) in edges {
                let target_stack = self.blocks[target].input_stack.clone();
                if stack.len() != target_stack.len() {
                    let end = self.f.il_range(self.blocks[index].block).end;
                    self.warnings.push(format!(
                        "IL_{end:04x}->IL{:04x}: Incompatible stack heights: {} vs {}",
                        self.blocks[target].start,
                        stack.len(),
                        target_stack.len()
                    ));
                    continue;
                }
                for (&a, &b) in stack.iter().rev().zip(target_stack.iter().rev()) {
                    let (sa, sb) = (self.f.var(a).stack_type, self.f.var(b).stack_type);
                    if sa == sb {
                        union_find.merge(a, b);
                    } else {
                        if !is_valid_stack_type_merge(sa, sb) {
                            let end = self.f.il_range(self.blocks[index].block).end;
                            self.warnings.push(format!(
                                "IL_{end:04x}->IL{:04x}: Incompatible stack types: {} vs {}",
                                self.blocks[target].start,
                                sa.name(),
                                sb.name()
                            ));
                        }
                        let block = self.blocks[index].block;
                        self.insert_stack_adjustment(block, a, b);
                    }
                }
            }
        }
        Ok(union_find)
    }

    fn insert_stack_adjustment(&mut self, block: NodeId, a: VarId, b: VarId) {
        let mut position = self.f.list(block).len();
        while position > 0
            && self
                .f
                .has_flag(self.f.list(block)[position - 1], Flags::MAY_BRANCH)
        {
            position -= 1;
        }
        let load = self.add(Inst::LdLoc { var: a });
        let target = self.f.var(b).stack_type.to_primitive(Sign::None);
        let value = self.conv(load, target, false, Sign::Signed);
        let store = self.add(Inst::StLoc {
            var: b,
            value,
            il_stack_was_empty: false,
            is_stack_adjustment: true,
        });
        self.f.insert(block, position, store);
    }

    /// `CollectStackVariablesVisitor` over the blocks in import order.
    fn collect_stack_variables(&mut self, mut union_find: UnionFind) {
        let mut seen: Vec<VarId> = Vec::new();
        let mut seen_set: HashSet<VarId> = HashSet::new();
        for index in 0..self.blocks.len() {
            let block = self.blocks[index].block;
            for node in self.f.descendants_post(block) {
                let var = match self.f.inst(node) {
                    Inst::LdLoc { var } | Inst::StLoc { var, .. } => *var,
                    _ => continue,
                };
                if self.f.var(var).kind != VariableKind::StackSlot {
                    continue;
                }
                let mapped = union_find.find(var);
                if seen_set.insert(mapped) {
                    seen.push(mapped);
                    self.f.var_mut(mapped).name = format!("S_{}", seen.len() - 1);
                }
                let (t1, t2) = (self.f.var(var).ty, self.f.var(mapped).ty);
                let st = self.f.var(mapped).stack_type;
                if var != mapped && t1 != t2 && st != StackType::VT {
                    self.f.var_mut(mapped).ty = find_stack_type(self.ts, st);
                }
                match self.f.inst_mut(node) {
                    Inst::LdLoc { var } | Inst::StLoc { var, .. } => *var = mapped,
                    _ => unreachable!(),
                }
            }
        }
        self.stack_variables = seen;
    }

    // ---- the expression stack ----

    fn push(&mut self, inst: NodeId) -> Decoded {
        self.expression_stack.push(inst);
        Decoded { inst, pushed: true }
    }

    fn stmt(inst: NodeId) -> Decoded {
        Decoded {
            inst,
            pushed: false,
        }
    }

    fn peek_stack_type(&mut self) -> StackType {
        self.peek_stack_type_at_depth(0)
    }

    fn peek_stack_type_at_depth(&mut self, depth: usize) -> StackType {
        if depth < self.expression_stack.len() {
            let inst = self.expression_stack[self.expression_stack.len() - 1 - depth];
            return self.result_type(inst);
        }
        let skip = depth - self.expression_stack.len();
        match self.current_stack.len().checked_sub(skip + 1) {
            Some(i) => self.f.var(self.current_stack[i]).stack_type,
            None => StackType::Unknown,
        }
    }

    fn stack_underflow(&mut self) -> NodeId {
        let inst = self.invalid_expression("Stack underflow");
        let offset = self.code.offset as i32;
        self.f.set_il_range(inst, Interval::new(offset, offset));
        inst
    }

    fn peek(&mut self) -> NodeId {
        self.flush_expression_stack();
        match self.current_stack.last() {
            Some(&v) => self.add(Inst::LdLoc { var: v }),
            None => self.stack_underflow(),
        }
    }

    fn pop(&mut self) -> NodeId {
        if let Some(inst) = self.expression_stack.pop() {
            return inst;
        }
        match self.current_stack.pop() {
            Some(v) => self.add(Inst::LdLoc { var: v }),
            None => self.stack_underflow(),
        }
    }

    fn pop_as(&mut self, expected: StackType) -> NodeId {
        let inst = self.pop();
        let offset = self.code.offset as i32;
        let mut warnings = std::mem::take(&mut self.warnings);
        let result = cast(
            &mut self.f,
            self.ts,
            inst,
            expected,
            Some(&mut warnings),
            offset,
        );
        self.warnings = warnings;
        result
    }

    fn pop_pointer(&mut self) -> NodeId {
        let inst = self.pop();
        match self.result_type(inst) {
            StackType::I4 | StackType::I8 | StackType::Unknown => {
                self.conv(inst, PrimitiveType::I, false, Sign::None)
            }
            StackType::I | StackType::Ref => inst,
            other => {
                self.warn(&format!(
                    "Expected native int or pointer, but got {}",
                    other.name()
                ));
                self.conv(inst, PrimitiveType::I, false, Sign::None)
            }
        }
    }

    fn pop_stobj_target(&mut self) -> NodeId {
        if let Some(&last) = self.expression_stack.last()
            && matches!(
                self.f.inst(last),
                Inst::LdElema { .. } | Inst::LdFlda { .. }
            )
        {
            self.flush_expression_stack();
        }
        self.pop_pointer()
    }

    fn declaring_is_reference_type(&mut self, field: &FieldOperand) -> Option<bool> {
        let declaring = field.field.declaring?;
        self.ts.is_reference_type(declaring)
    }

    fn pop_field_target(&mut self, field: &FieldOperand) -> NodeId {
        match self.declaring_is_reference_type(field) {
            Some(true) => self.pop_as(StackType::Obj),
            Some(false) => self.pop_pointer(),
            None => match self.peek_stack_type() {
                StackType::Obj | StackType::VT | StackType::Unknown => self.pop(),
                _ => self.pop_pointer(),
            },
        }
    }

    fn pop_ldfld_target(&mut self, field: &FieldOperand) -> NodeId {
        match self.declaring_is_reference_type(field) {
            Some(true) => self.pop_as(StackType::Obj),
            Some(false) => match self.peek_stack_type() {
                StackType::VT | StackType::Unknown => {
                    let value = self.pop();
                    let ty = field.field.declaring.expect("declaring type");
                    self.add(Inst::AddressOf {
                        value,
                        ty: TypeOperand { token: 0, ty },
                    })
                }
                _ => self.pop_pointer(),
            },
            None => match self.peek_stack_type() {
                StackType::Obj | StackType::VT | StackType::Unknown => self.pop(),
                _ => self.pop_pointer(),
            },
        }
    }

    /// `FlushExpressionStack`: moves the pending values into stack slot variables.
    fn flush_expression_stack(&mut self) {
        let block = self.blocks[self.current_block.expect("current block")].block;
        for inst in std::mem::take(&mut self.expression_stack) {
            let ty = self.f.infer_type(self.ts, inst);
            let stack_type = self.result_type(inst);
            let v = self.new_variable(VariableKind::StackSlot, ty, stack_type, None);
            self.f.var_mut(v).has_generated_name = true;
            self.current_stack.push(v);
            let store = self.add(Inst::StLoc {
                var: v,
                value: inst,
                il_stack_was_empty: false,
                is_stack_adjustment: false,
            });
            let range = self.f.il_range(inst);
            self.f.set_il_range(store, range);
            self.f.push(block, store);
        }
    }

    fn mark_branch_target(&mut self, target: i32) {
        self.flush_expression_stack();
        let stack = self.current_stack.clone();
        let target_block = self.store_stack_for_offset(target, stack.clone());
        let current = self.current_block.expect("current block");
        self.blocks[current].outgoing.push((target_block, stack));
    }

    fn is_invalid_branch(&self, target: i32) -> bool {
        target < 0 || target as usize >= self.code.len()
    }

    // ---- instruction decoding ----

    fn decode_instruction(&mut self) -> Result<Decoded> {
        if self.code.remaining() == 0 {
            return Ok(Self::stmt(self.invalid_branch("Unexpected end of body")));
        }
        let op = self.code.opcode()?;
        use KnownTypeCode as K;
        use PrimitiveType as P;
        Ok(match op {
            0xFE16 => return self.decode_constrained(),
            0xFE1E => return self.decode_readonly(),
            0xFE14 => return self.decode_tail(),
            0xFE12 => return self.decode_unaligned(),
            0xFE13 => return self.decode_volatile(),
            0x58 => self.binary(BinaryNumericOperator::Add, false, Sign::None),
            0xD6 => self.binary(BinaryNumericOperator::Add, true, Sign::Signed),
            0xD7 => self.binary(BinaryNumericOperator::Add, true, Sign::Unsigned),
            0x5F => self.binary(BinaryNumericOperator::BitAnd, false, Sign::None),
            0xFE00 => {
                let i = self.add(Inst::Arglist);
                self.push(i)
            }
            0x3B | 0x2E => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::Equality, false)?)
            }
            0x3C | 0x2F => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::GreaterThanOrEqual, false)?)
            }
            0x41 | 0x34 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::GreaterThanOrEqual, true)?)
            }
            0x3D | 0x30 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::GreaterThan, false)?)
            }
            0x42 | 0x35 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::GreaterThan, true)?)
            }
            0x3E | 0x31 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::LessThanOrEqual, false)?)
            }
            0x43 | 0x36 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::LessThanOrEqual, true)?)
            }
            0x3F | 0x32 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::LessThan, false)?)
            }
            0x44 | 0x37 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::LessThan, true)?)
            }
            0x40 | 0x33 => {
                Self::stmt(self.comparison_branch(op, ComparisonKind::Inequality, true)?)
            }
            0x38 | 0x2B => Self::stmt(self.unconditional_branch(op, false)?),
            0x01 => Self::stmt(self.add(Inst::DebugBreak)),
            0x39 | 0x2C => Self::stmt(self.conditional_branch(op, true)?),
            0x3A | 0x2D => Self::stmt(self.conditional_branch(op, false)?),
            0x28 => return self.decode_call(CallKind::Call),
            0x6F => return self.decode_call(CallKind::CallVirt),
            0x29 => return self.decode_call_indirect(),
            0xFE01 => {
                let c = self.comparison(ComparisonKind::Equality, false);
                self.push(c)
            }
            0xFE02 => {
                let c = self.comparison(ComparisonKind::GreaterThan, false);
                self.push(c)
            }
            0xFE03 => {
                let c = self.comparison(ComparisonKind::GreaterThan, true);
                self.push(c)
            }
            0xFE04 => {
                let c = self.comparison(ComparisonKind::LessThan, false);
                self.push(c)
            }
            0xFE05 => {
                let c = self.comparison(ComparisonKind::LessThan, true);
                self.push(c)
            }
            0xC3 => {
                let arg = self.peek();
                Self::stmt(self.add(Inst::Ckfinite { arg }))
            }
            0x67 => self.conv_push(P::I1, false, Sign::None),
            0x68 => self.conv_push(P::I2, false, Sign::None),
            0x69 => self.conv_push(P::I4, false, Sign::None),
            0x6A => self.conv_push(P::I8, false, Sign::None),
            0x6B => self.conv_push(P::R4, false, Sign::Signed),
            0x6C => self.conv_push(P::R8, false, Sign::Signed),
            0xD2 => self.conv_push(P::U1, false, Sign::None),
            0xD1 => self.conv_push(P::U2, false, Sign::None),
            0x6D => self.conv_push(P::U4, false, Sign::None),
            0x6E => self.conv_push(P::U8, false, Sign::None),
            0xD3 => self.conv_push(P::I, false, Sign::None),
            0xE0 => self.conv_push(P::U, false, Sign::None),
            0x76 => self.conv_push(P::R, false, Sign::Unsigned),
            0xB3 => self.conv_push(P::I1, true, Sign::Signed),
            0xB5 => self.conv_push(P::I2, true, Sign::Signed),
            0xB7 => self.conv_push(P::I4, true, Sign::Signed),
            0xB9 => self.conv_push(P::I8, true, Sign::Signed),
            0xB4 => self.conv_push(P::U1, true, Sign::Signed),
            0xB6 => self.conv_push(P::U2, true, Sign::Signed),
            0xB8 => self.conv_push(P::U4, true, Sign::Signed),
            0xBA => self.conv_push(P::U8, true, Sign::Signed),
            0xD4 => self.conv_push(P::I, true, Sign::Signed),
            0xD5 => self.conv_push(P::U, true, Sign::Signed),
            0x82 => self.conv_push(P::I1, true, Sign::Unsigned),
            0x83 => self.conv_push(P::I2, true, Sign::Unsigned),
            0x84 => self.conv_push(P::I4, true, Sign::Unsigned),
            0x85 => self.conv_push(P::I8, true, Sign::Unsigned),
            0x86 => self.conv_push(P::U1, true, Sign::Unsigned),
            0x87 => self.conv_push(P::U2, true, Sign::Unsigned),
            0x88 => self.conv_push(P::U4, true, Sign::Unsigned),
            0x89 => self.conv_push(P::U8, true, Sign::Unsigned),
            0x8A => self.conv_push(P::I, true, Sign::Unsigned),
            0x8B => self.conv_push(P::U, true, Sign::Unsigned),
            0xFE17 => {
                let size = self.pop_as(StackType::I4);
                let source = self.pop_pointer();
                let dest = self.pop_pointer();
                Self::stmt(self.add(Inst::Cpblk {
                    dest,
                    source,
                    size,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0x5B => self.binary(BinaryNumericOperator::Div, false, Sign::Signed),
            0x5C => self.binary(BinaryNumericOperator::Div, false, Sign::Unsigned),
            0x25 => {
                let i = self.peek();
                self.push(i)
            }
            0xFE11 => {
                let value = self.pop();
                Self::stmt(self.add(Inst::Leave {
                    target: None,
                    value,
                }))
            }
            0xDC => {
                let value = self.add(Inst::Nop {
                    kind: NopKind::Normal,
                });
                Self::stmt(self.add(Inst::Leave {
                    target: None,
                    value,
                }))
            }
            0xFE18 => {
                let size = self.pop_as(StackType::I4);
                let value = self.pop_as(StackType::I4);
                let address = self.pop_pointer();
                Self::stmt(self.add(Inst::Initblk {
                    address,
                    value,
                    size,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0x27 => Self::stmt(self.decode_jmp()?),
            0x0E | 0xFE09 => {
                let index = self.code.index(op)?;
                let i = self.ldarg(index);
                self.push(i)
            }
            0x02..=0x05 => {
                let i = self.ldarg(i32::from(op - 0x02));
                self.push(i)
            }
            0x0F | 0xFE0A => {
                let index = self.code.index(op)?;
                let i = self.ldarga(index);
                self.push(i)
            }
            0x20 => {
                let value = self.code.i32()?;
                let i = self.add(Inst::LdcI4 { value });
                self.push(i)
            }
            0x21 => {
                let value = self.code.i64()?;
                let i = self.add(Inst::LdcI8 { value });
                self.push(i)
            }
            0x22 => {
                let bits = self.code.u32()?;
                let i = self.add(Inst::LdcF4 { bits });
                self.push(i)
            }
            0x23 => {
                let bits = self.code.u64()?;
                let i = self.add(Inst::LdcF8 { bits });
                self.push(i)
            }
            0x15..=0x1E => {
                let i = self.add(Inst::LdcI4 {
                    value: i32::from(op) - 0x16,
                });
                self.push(i)
            }
            0x1F => {
                let value = i32::from(self.code.i8()?);
                let i = self.add(Inst::LdcI4 { value });
                self.push(i)
            }
            0x14 => {
                let i = self.add(Inst::LdNull);
                self.push(i)
            }
            0x72 => {
                let token = self.code.i32()?;
                let md = &self.ts.comp.module(self.module).md;
                let value = md.user_strings.get((token & 0x00FF_FFFF) as usize)?;
                let i = self.add(Inst::LdStr { value });
                self.push(i)
            }
            0xFE06 => {
                let method = self.read_method()?;
                let i = self.add(Inst::LdFtn { method });
                self.push(i)
            }
            0x46 => self.ldind(K::SByte),
            0x48 => self.ldind(K::Int16),
            0x4A => self.ldind(K::Int32),
            0x4C => self.ldind(K::Int64),
            0x47 => self.ldind(K::Byte),
            0x49 => self.ldind(K::UInt16),
            0x4B => self.ldind(K::UInt32),
            0x4E => self.ldind(K::Single),
            0x4F => self.ldind(K::Double),
            0x4D => self.ldind(K::IntPtr),
            0x50 => self.ldind(K::Object),
            0x11 | 0xFE0C => {
                let index = self.code.index(op)?;
                let i = self.ldloc(index);
                self.push(i)
            }
            0x06..=0x09 => {
                let i = self.ldloc(i32::from(op - 0x06));
                self.push(i)
            }
            0x12 | 0xFE0D => {
                let index = self.code.index(op)?;
                let i = self.ldloca(index);
                self.push(i)
            }
            0xDD | 0xDE => Self::stmt(self.unconditional_branch(op, true)?),
            0xFE0F => {
                let arg = self.pop();
                let i = self.add(Inst::LocAlloc { arg });
                self.push(i)
            }
            0x5A => self.binary(BinaryNumericOperator::Mul, false, Sign::None),
            0xD8 => self.binary(BinaryNumericOperator::Mul, true, Sign::Signed),
            0xD9 => self.binary(BinaryNumericOperator::Mul, true, Sign::Unsigned),
            0x65 => self.neg(),
            0x73 => return self.decode_call(CallKind::NewObj),
            0x00 => Self::stmt(self.add(Inst::Nop {
                kind: NopKind::Normal,
            })),
            0x66 => {
                let arg = self.pop();
                let i = self.add(Inst::BitNot { arg });
                self.push(i)
            }
            0x60 => self.binary(BinaryNumericOperator::BitOr, false, Sign::None),
            0x26 => {
                self.flush_expression_stack();
                self.pop();
                Self::stmt(self.add(Inst::Nop { kind: NopKind::Pop }))
            }
            0x5D => self.binary(BinaryNumericOperator::Rem, false, Sign::Signed),
            0x5E => self.binary(BinaryNumericOperator::Rem, false, Sign::Unsigned),
            0x2A => Self::stmt(self.ret()),
            0x62 => self.binary(BinaryNumericOperator::ShiftLeft, false, Sign::None),
            0x63 => self.binary(BinaryNumericOperator::ShiftRight, false, Sign::Signed),
            0x64 => self.binary(BinaryNumericOperator::ShiftRight, false, Sign::Unsigned),
            0x10 | 0xFE0B => {
                let index = self.code.index(op)?;
                Self::stmt(self.starg(index))
            }
            0x52 => self.stind(StackType::I4, K::SByte),
            0x53 => self.stind(StackType::I4, K::Int16),
            0x54 => self.stind(StackType::I4, K::Int32),
            0x55 => self.stind(StackType::I8, K::Int64),
            0x56 => self.stind(StackType::F4, K::Single),
            0x57 => self.stind(StackType::F8, K::Double),
            0xDF => self.stind(StackType::I, K::IntPtr),
            0x51 => self.stind(StackType::Obj, K::Object),
            0x13 | 0xFE0E => {
                let index = self.code.index(op)?;
                Self::stmt(self.stloc(index))
            }
            0x0A..=0x0D => Self::stmt(self.stloc(i32::from(op - 0x0A))),
            0x59 => self.binary(BinaryNumericOperator::Sub, false, Sign::None),
            0xDA => self.binary(BinaryNumericOperator::Sub, true, Sign::Signed),
            0xDB => self.binary(BinaryNumericOperator::Sub, true, Sign::Unsigned),
            0x45 => Self::stmt(self.decode_switch()?),
            0x61 => self.binary(BinaryNumericOperator::BitXor, false, Sign::None),
            0x8C => {
                let ty = self.read_type()?;
                let st = self.ts.stack_type(ty.ty);
                let arg = self.pop_as(st);
                let i = self.add(Inst::Box { arg, ty });
                self.push(i)
            }
            0x74 => {
                let arg = self.pop_as(StackType::Obj);
                let ty = self.read_type()?;
                let i = self.add(Inst::CastClass { arg, ty });
                self.push(i)
            }
            0x70 => {
                let ty = self.read_type()?;
                let source = self.pop_pointer();
                let ld = self.add(Inst::LdObj {
                    target: source,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                });
                let target = self.pop_stobj_target();
                Self::stmt(self.add(Inst::StObj {
                    target,
                    value: ld,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0xFE15 => {
                let target = self.pop_stobj_target();
                let ty = self.read_type()?;
                let empty = self.current_stack_is_empty();
                let value = self.add(Inst::DefaultValue {
                    ty,
                    il_stack_was_empty: empty,
                });
                Self::stmt(self.add(Inst::StObj {
                    target,
                    value,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0x75 => {
                let ty = self.read_type()?;
                if self.ts.is_reference_type(ty.ty) != Some(true) {
                    self.flush_expression_stack();
                }
                let arg = self.pop_as(StackType::Obj);
                let i = self.add(Inst::IsInst { arg, ty });
                self.push(i)
            }
            0xA3 => {
                let ty = self.read_type()?;
                self.ldelem(ty)
            }
            0x90 => self.ldelem_known(K::SByte),
            0x92 => self.ldelem_known(K::Int16),
            0x94 => self.ldelem_known(K::Int32),
            0x96 => self.ldelem_known(K::Int64),
            0x91 => self.ldelem_known(K::Byte),
            0x93 => self.ldelem_known(K::UInt16),
            0x95 => self.ldelem_known(K::UInt32),
            0x98 => self.ldelem_known(K::Single),
            0x99 => self.ldelem_known(K::Double),
            0x97 => self.ldelem_known(K::IntPtr),
            0x9A => self.ldelem_known(K::Object),
            0x8F => {
                let index = self.pop();
                let array = self.pop();
                let ty = self.read_type()?;
                let i = self.add(Inst::LdElema {
                    ty,
                    array,
                    indices: vec![index],
                    delay_exceptions: false,
                    is_read_only: false,
                });
                self.push(i)
            }
            0x7B => {
                let field = self.read_field()?;
                let target = self.pop_ldfld_target(&field);
                let ty = TypeOperand {
                    token: 0,
                    ty: field.field.ty,
                };
                let address = self.add(Inst::LdFlda {
                    target,
                    field,
                    delay_exceptions: true,
                });
                let i = self.add(Inst::LdObj {
                    target: address,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                });
                self.push(i)
            }
            0x7C => {
                let field = self.read_field()?;
                let target = self.pop_field_target(&field);
                let i = self.add(Inst::LdFlda {
                    target,
                    field,
                    delay_exceptions: false,
                });
                self.push(i)
            }
            0x7D => {
                let field = self.read_field()?;
                let ty = TypeOperand {
                    token: 0,
                    ty: field.field.ty,
                };
                let st = self.ts.stack_type(ty.ty);
                let value = self.pop_as(st);
                let target = self.pop_field_target(&field);
                let address = self.add(Inst::LdFlda {
                    target,
                    field,
                    delay_exceptions: true,
                });
                Self::stmt(self.add(Inst::StObj {
                    target: address,
                    value,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0x8E => {
                let array = self.pop_as(StackType::Obj);
                let i = self.add(Inst::LdLen {
                    array,
                    result: StackType::I,
                });
                self.push(i)
            }
            0x71 => {
                let target = self.pop_pointer();
                let ty = self.read_type()?;
                let i = self.add(Inst::LdObj {
                    target,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                });
                self.push(i)
            }
            0x7E => {
                let field = self.read_field()?;
                let ty = TypeOperand {
                    token: 0,
                    ty: field.field.ty,
                };
                let address = self.add(Inst::LdsFlda { field });
                let i = self.add(Inst::LdObj {
                    target: address,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                });
                self.push(i)
            }
            0x7F => {
                let field = self.read_field()?;
                let i = self.add(Inst::LdsFlda { field });
                self.push(i)
            }
            0x80 => {
                let field = self.read_field()?;
                let ty = TypeOperand {
                    token: 0,
                    ty: field.field.ty,
                };
                let st = self.ts.stack_type(ty.ty);
                let value = self.pop_as(st);
                let target = self.add(Inst::LdsFlda { field });
                Self::stmt(self.add(Inst::StObj {
                    target,
                    value,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0xD0 => {
                let token = self.read_token()?;
                let i = self.ldtoken(token)?;
                self.push(i)
            }
            0xFE07 => {
                let arg = self.pop();
                let method = self.read_method()?;
                let i = self.add(Inst::LdVirtFtn { arg, method });
                self.push(i)
            }
            0xC6 => {
                let arg = self.pop_pointer();
                let ty = self.read_type()?;
                let i = self.add(Inst::MakeRefAny { arg, ty });
                self.push(i)
            }
            0x8D => {
                let ty = self.read_type()?;
                let index = self.pop();
                let i = self.add(Inst::NewArr {
                    ty,
                    indices: vec![index],
                });
                self.push(i)
            }
            0xFE1D => {
                let arg = self.pop();
                let i = self.add(Inst::RefAnyType { arg });
                self.push(i)
            }
            0xC2 => {
                let arg = self.pop();
                let ty = self.read_type()?;
                let i = self.add(Inst::RefAnyValue { arg, ty });
                self.push(i)
            }
            0xFE1A => Self::stmt(self.add(Inst::Rethrow)),
            0xFE1C => {
                let ty = self.read_type()?;
                let i = self.add(Inst::SizeOf { ty });
                self.push(i)
            }
            0xA4 => {
                let ty = self.read_type()?;
                Self::stmt(self.stelem(ty))
            }
            0x9C => self.stelem_known(K::SByte),
            0x9D => self.stelem_known(K::Int16),
            0x9E => self.stelem_known(K::Int32),
            0x9F => self.stelem_known(K::Int64),
            0xA0 => self.stelem_known(K::Single),
            0xA1 => self.stelem_known(K::Double),
            0x9B => self.stelem_known(K::IntPtr),
            0xA2 => self.stelem_known(K::Object),
            0x81 => {
                let ty = self.read_type()?;
                let st = self.ts.stack_type(ty.ty);
                let value = self.pop_as(st);
                let target = self.pop_stobj_target();
                Self::stmt(self.add(Inst::StObj {
                    target,
                    value,
                    ty,
                    is_volatile: false,
                    unaligned: 0,
                }))
            }
            0x7A => {
                let arg = self.pop();
                Self::stmt(self.add(Inst::Throw { arg }))
            }
            0x79 => {
                let arg = self.pop();
                let ty = self.read_type()?;
                let i = self.add(Inst::Unbox { arg, ty });
                self.push(i)
            }
            0xA5 => {
                let arg = self.pop();
                let ty = self.read_type()?;
                let i = self.add(Inst::UnboxAny { arg, ty });
                self.push(i)
            }
            _ => Self::stmt(self.invalid_branch(&format!("Unknown opcode: 0x{:02X}", op))),
        })
    }

    fn conv_push(&mut self, target: PrimitiveType, check: bool, sign: Sign) -> Decoded {
        let arg = self.pop();
        let c = self.conv(arg, target, check, sign);
        self.push(c)
    }

    fn ldind(&mut self, code: KnownTypeCode) -> Decoded {
        let target = self.pop_pointer();
        let ty = self.known(code);
        let i = self.add(Inst::LdObj {
            target,
            ty,
            is_volatile: false,
            unaligned: 0,
        });
        self.push(i)
    }

    fn stind(&mut self, value_type: StackType, code: KnownTypeCode) -> Decoded {
        let value = self.pop_as(value_type);
        let target = self.pop_stobj_target();
        let ty = self.known(code);
        Self::stmt(self.add(Inst::StObj {
            target,
            value,
            ty,
            is_volatile: false,
            unaligned: 0,
        }))
    }

    fn ldelem_known(&mut self, code: KnownTypeCode) -> Decoded {
        let ty = self.known(code);
        self.ldelem(ty)
    }

    fn ldelem(&mut self, ty: TypeOperand) -> Decoded {
        let index = self.pop();
        let array = self.pop();
        let address = self.add(Inst::LdElema {
            ty,
            array,
            indices: vec![index],
            delay_exceptions: true,
            is_read_only: false,
        });
        let i = self.add(Inst::LdObj {
            target: address,
            ty,
            is_volatile: false,
            unaligned: 0,
        });
        self.push(i)
    }

    fn stelem_known(&mut self, code: KnownTypeCode) -> Decoded {
        let ty = self.known(code);
        Self::stmt(self.stelem(ty))
    }

    fn stelem(&mut self, ty: TypeOperand) -> NodeId {
        let st = self.ts.stack_type(ty.ty);
        let value = self.pop_as(st);
        let index = self.pop();
        let array = self.pop();
        let address = self.add(Inst::LdElema {
            ty,
            array,
            indices: vec![index],
            delay_exceptions: true,
            is_read_only: false,
        });
        self.add(Inst::StObj {
            target: address,
            value,
            ty,
            is_volatile: false,
            unaligned: 0,
        })
    }

    fn neg(&mut self) -> Decoded {
        let ty = self.peek_stack_type();
        let zero = match ty {
            StackType::I => {
                let c = ldc_i4(&mut self.f, 0);
                self.conv(c, PrimitiveType::I, false, Sign::None)
            }
            StackType::I8 => self.add(Inst::LdcI8 { value: 0 }),
            StackType::F4 => self.add(Inst::LdcF4 { bits: 0 }),
            StackType::F8 => self.add(Inst::LdcF8 { bits: 0 }),
            StackType::I4 => ldc_i4(&mut self.f, 0),
            _ => {
                self.warn("Unsupported input type for neg.");
                ldc_i4(&mut self.f, 0)
            }
        };
        let value = self.pop();
        let (left_input, right_input) = (self.result_type(zero), self.result_type(value));
        let i = self.add(Inst::BinaryNumeric {
            op: BinaryNumericOperator::Sub,
            check_overflow: false,
            sign: Sign::None,
            left_input,
            right_input,
            left: zero,
            right: value,
        });
        self.push(i)
    }

    fn ret(&mut self) -> NodeId {
        let container = self.main_container;
        if self.method_return_stack_type == StackType::Void {
            let value = self.add(Inst::Nop {
                kind: NopKind::Normal,
            });
            self.add(Inst::Leave {
                target: Some(container),
                value,
            })
        } else if self.current_instruction_start == 0 {
            self.invalid_branch(
                "Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.",
            )
        } else {
            let value = self.pop_as(self.method_return_stack_type);
            self.add(Inst::Leave {
                target: Some(container),
                value,
            })
        }
    }

    fn ldarg(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.parameter_variables.get(i))
        {
            Some(&var) => self.add(Inst::LdLoc { var }),
            None => self.invalid_expression(&format!("ldarg {v} (out-of-bounds)")),
        }
    }

    fn ldarga(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.parameter_variables.get(i))
        {
            Some(&var) => self.add(Inst::LdLoca { var }),
            None => self.invalid_expression(&format!("ldarga {v} (out-of-bounds)")),
        }
    }

    fn starg(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.parameter_variables.get(i))
        {
            Some(&var) => {
                let value = self.pop_as(self.f.var(var).stack_type);
                self.add(Inst::StLoc {
                    var,
                    value,
                    il_stack_was_empty: false,
                    is_stack_adjustment: false,
                })
            }
            None => {
                self.flush_expression_stack();
                self.pop();
                self.invalid_expression(&format!("starg {v} (out-of-bounds)"))
            }
        }
    }

    fn ldloc(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.local_variables.get(i))
        {
            Some(&var) => self.add(Inst::LdLoc { var }),
            None => self.invalid_expression(&format!("ldloc {v} (out-of-bounds)")),
        }
    }

    fn ldloca(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.local_variables.get(i))
        {
            Some(&var) => self.add(Inst::LdLoca { var }),
            None => self.invalid_expression(&format!("ldloca {v} (out-of-bounds)")),
        }
    }

    fn stloc(&mut self, v: i32) -> NodeId {
        match usize::try_from(v)
            .ok()
            .and_then(|i| self.local_variables.get(i))
        {
            Some(&var) => {
                let value = self.pop_as(self.f.var(var).stack_type);
                let empty = self.current_stack_is_empty();
                self.add(Inst::StLoc {
                    var,
                    value,
                    il_stack_was_empty: empty,
                    is_stack_adjustment: false,
                })
            }
            None => {
                self.flush_expression_stack();
                self.pop();
                self.invalid_expression(&format!("stloc {v} (out-of-bounds)"))
            }
        }
    }

    // ---- prefixes ----

    fn decode_constrained(&mut self) -> Result<Decoded> {
        let ty = self.read_type()?;
        self.constrained_prefix = Some(ty);
        let decoded = self.decode_instruction();
        if let Ok(d) = &decoded
            && !matches!(
                self.f.inst(d.inst),
                Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_)
            )
        {
            self.warn("Ignored invalid 'constrained' prefix");
        }
        self.constrained_prefix = None;
        decoded
    }

    fn decode_tail(&mut self) -> Result<Decoded> {
        let d = self.decode_instruction()?;
        match self.f.inst_mut(d.inst) {
            Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => c.is_tail = true,
            _ => self.warn("Ignored invalid 'tail' prefix"),
        }
        Ok(d)
    }

    fn decode_unaligned(&mut self) -> Result<Decoded> {
        let alignment = self.code.u8()?;
        let d = self.decode_instruction()?;
        match self.f.inst_mut(d.inst) {
            Inst::LdObj { unaligned, .. }
            | Inst::StObj { unaligned, .. }
            | Inst::Cpblk { unaligned, .. }
            | Inst::Initblk { unaligned, .. } => *unaligned = alignment,
            _ => self.warn("Ignored invalid 'unaligned' prefix"),
        }
        Ok(d)
    }

    fn decode_volatile(&mut self) -> Result<Decoded> {
        let d = self.decode_instruction()?;
        match self.f.inst_mut(d.inst) {
            Inst::LdObj { is_volatile, .. }
            | Inst::StObj { is_volatile, .. }
            | Inst::Cpblk { is_volatile, .. }
            | Inst::Initblk { is_volatile, .. } => *is_volatile = true,
            _ => self.warn("Ignored invalid 'volatile' prefix"),
        }
        Ok(d)
    }

    fn decode_readonly(&mut self) -> Result<Decoded> {
        let d = self.decode_instruction()?;
        match self.f.inst_mut(d.inst) {
            Inst::LdElema { is_read_only, .. } => *is_read_only = true,
            _ => self.warn("Ignored invalid 'readonly' prefix"),
        }
        Ok(d)
    }

    // ---- calls ----

    fn decode_call(&mut self, kind: CallKind) -> Result<Decoded> {
        let method = self.read_method()?;
        let m = method.method.clone();
        let declaring = m
            .declaring
            .unwrap_or_else(|| self.ts.intern(TypeData::UnknownSpecial));
        let declaring_kind = self.kind(declaring);
        let params = m.params()?;
        if declaring_kind == Kind::Array {
            let arguments = self.prepare_arguments(kind, &m, &params, false);
            let TypeData::Array { elem, .. } = *self.ts.data(declaring) else {
                unreachable!()
            };
            let elem = TypeOperand { token: 0, ty: elem };
            if kind == CallKind::NewObj {
                let i = self.add(Inst::NewArr {
                    ty: elem,
                    indices: arguments,
                });
                return Ok(self.push(i));
            }
            match m.name.as_str() {
                "Set" => {
                    let mut indices = arguments;
                    let value = indices.pop().expect("Set has a value argument");
                    let target = indices.remove(0);
                    let address = self.add(Inst::LdElema {
                        ty: elem,
                        array: target,
                        indices,
                        delay_exceptions: true,
                        is_read_only: false,
                    });
                    return Ok(Self::stmt(self.add(Inst::StObj {
                        target: address,
                        value,
                        ty: elem,
                        is_volatile: false,
                        unaligned: 0,
                    })));
                }
                "Get" => {
                    let mut indices = arguments;
                    let target = indices.remove(0);
                    let address = self.add(Inst::LdElema {
                        ty: elem,
                        array: target,
                        indices,
                        delay_exceptions: true,
                        is_read_only: false,
                    });
                    let i = self.add(Inst::LdObj {
                        target: address,
                        ty: elem,
                        is_volatile: false,
                        unaligned: 0,
                    });
                    return Ok(self.push(i));
                }
                "Address" => {
                    let mut indices = arguments;
                    let target = indices.remove(0);
                    let i = self.add(Inst::LdElema {
                        ty: elem,
                        array: target,
                        indices,
                        delay_exceptions: false,
                        is_read_only: false,
                    });
                    return Ok(self.push(i));
                }
                name => {
                    self.warn(&format!("Unknown method called on array type: {name}"));
                    return self.finish_call(kind, method, arguments);
                }
            }
        }
        let value_type_ctor = m.name == ".ctor"
            && !m.is_static
            && kind == CallKind::Call
            && self.kind(m.ret) == Kind::Def(TypeKind::Void);
        let struct_ctor = value_type_ctor
            && match declaring_kind {
                Kind::Def(TypeKind::Struct) => true,
                Kind::Unknown => {
                    matches!(
                        self.peek_stack_type_at_depth(params.len()),
                        StackType::Ref | StackType::I
                    ) && self.ts.is_reference_type(declaring) != Some(true)
                }
                _ => false,
            };
        if struct_ctor {
            let arguments = self.prepare_arguments(kind, &m, &params, true);
            let empty = self.current_stack_is_empty();
            let mut args = arguments;
            let target = args.remove(0);
            let constrained_to = self.constrained_prefix;
            let newobj = self.add(Inst::NewObj(CallData {
                method,
                args,
                is_tail: false,
                constrained_to,
                il_stack_was_empty: empty,
            }));
            let ty = TypeOperand {
                token: 0,
                ty: declaring,
            };
            return Ok(Self::stmt(self.add(Inst::StObj {
                target,
                value: newobj,
                ty,
                is_volatile: false,
                unaligned: 0,
            })));
        }
        let arguments = self.prepare_arguments(kind, &m, &params, false);
        self.finish_call(kind, method, arguments)
    }

    fn finish_call(
        &mut self,
        kind: CallKind,
        method: MethodOperand,
        args: Vec<NodeId>,
    ) -> Result<Decoded> {
        let empty = self.current_stack_is_empty();
        let data = CallData {
            method,
            args,
            is_tail: false,
            constrained_to: self.constrained_prefix,
            il_stack_was_empty: empty,
        };
        let call = self.add(match kind {
            CallKind::Call => Inst::Call(data),
            CallKind::CallVirt => Inst::CallVirt(data),
            CallKind::NewObj => Inst::NewObj(data),
        });
        if self.result_type(call) != StackType::Void {
            Ok(self.push(call))
        } else {
            Ok(Self::stmt(call))
        }
    }

    /// `CallInstruction.ExpectedTypeForThisPointer`.
    fn expected_type_for_this(&mut self, declaring: TypeId, constrained: bool) -> StackType {
        if constrained || self.kind(declaring) == Kind::TypeParameter {
            return StackType::Ref;
        }
        match self.ts.is_reference_type(declaring) {
            Some(true) => StackType::Obj,
            Some(false) => StackType::Ref,
            None => StackType::Unknown,
        }
    }

    fn prepare_arguments(
        &mut self,
        kind: CallKind,
        m: &ResolvedMethod,
        params: &[TypeId],
        first_is_stobj_target: bool,
    ) -> Vec<NodeId> {
        let first = usize::from(kind != CallKind::NewObj && !m.is_static);
        let mut arguments = vec![NodeId(u32::MAX); first + params.len()];
        let declaring = m
            .declaring
            .unwrap_or_else(|| self.ts.intern(TypeData::UnknownSpecial));
        let constrained = self.constrained_prefix;
        let type_of_this = constrained.map_or(declaring, |c| c.ty);
        let expected = self.expected_type_for_this(declaring, constrained.is_some());
        let requires_ldobj_if_ref = first == 1
            && !first_is_stobj_target
            && self.options.use_ref_locals_for_accurate_order_of_evaluation
            && expected == StackType::Ref
            && self.ts.is_reference_type(type_of_this) != Some(false);
        for i in (0..params.len()).rev() {
            if requires_ldobj_if_ref {
                self.flush_expression_stack();
            }
            let st = self.ts.stack_type(params[i]);
            arguments[first + i] = self.pop_as(st);
        }
        if first == 1 {
            arguments[0] = if first_is_stobj_target {
                self.pop_stobj_target()
            } else {
                let target = self.pop_as(expected);
                if requires_ldobj_if_ref {
                    self.add(Inst::LdObjIfRef {
                        target,
                        ty: TypeOperand {
                            token: 0,
                            ty: type_of_this,
                        },
                    })
                } else {
                    target
                }
            };
        }
        arguments
    }

    fn decode_call_indirect(&mut self) -> Result<Decoded> {
        let token = self.read_token()?;
        if token >> 24 != 0x11 {
            return Err(Error::Malformed("Invalid calli metadata token"));
        }
        let (is_instance, has_explicit_this, signature, params, ret) =
            self.decode_method_signature(token & 0x00FF_FFFF)?;
        let _ = ret;
        let function_pointer = self.pop_as(StackType::I);
        let first = usize::from(is_instance);
        let mut args = vec![NodeId(u32::MAX); first + params.len()];
        for i in (0..params.len()).rev() {
            let st = self.ts.stack_type(params[i]);
            args[first + i] = self.pop_as(st);
        }
        if first == 1 {
            args[0] = self.pop();
        }
        let call = self.add(Inst::CallIndirect {
            is_instance,
            has_explicit_this,
            signature,
            token,
            function_pointer,
            args,
        });
        if self.result_type(call) != StackType::Void {
            Ok(self.push(call))
        } else {
            Ok(Self::stmt(call))
        }
    }

    /// `MetadataModule.DecodeMethodSignature`: header bits, the function pointer type, its
    /// parameter types and return type.
    fn decode_method_signature(
        &mut self,
        row: u32,
    ) -> Result<(bool, bool, TypeId, Vec<TypeId>, TypeId)> {
        let md = &self.ts.comp.module(self.module).md;
        let sig = md.get::<rows::StandAloneSig>(row)?;
        let blob = md.blob(sig.signature)?;
        let header = *blob
            .first()
            .ok_or(Error::Malformed("Expected Method signature"))?;
        if matches!(header & 0x0F, 0x06..=0x08) || header & 0x0F == 0x0A {
            return Err(Error::Malformed("Expected Method signature"));
        }
        let method_sig = signature::decode_method(&mut Bytes::new(blob))?;
        let fnptr = self
            .ts
            .function_pointer_from_sig(self.module, &method_sig, self.ctx)?;
        let options = self.ts.options(self.module);
        let fnptr = self.ts.apply_attributes(
            fnptr,
            self.module,
            AttributeSource::None,
            options,
            Nullability::Oblivious,
            false,
        )?;
        let (params, ret) = match self.ts.data(fnptr) {
            TypeData::FnPtr(f) => (f.params.to_vec(), f.ret),
            _ => (Vec::new(), fnptr),
        };
        Ok((header & 0x20 != 0, header & 0x40 != 0, fnptr, params, ret))
    }

    fn comparison(&mut self, kind: ComparisonKind, un: bool) -> NodeId {
        if !kind.is_equality_or_inequality() && self.peek_stack_type() == StackType::Obj {
            self.flush_expression_stack();
        }
        let mut right = self.pop();
        let mut left = self.pop();
        let (lt, rt) = (self.result_type(left), self.result_type(right));
        if matches!(lt, StackType::Obj | StackType::Ref) && rt.is_integer() {
            if rt == StackType::I4 {
                right = self.conv(right, PrimitiveType::I, false, Sign::None);
            }
            let target = self.result_type(right).to_primitive(Sign::None);
            left = self.conv(left, target, false, Sign::None);
        } else if matches!(rt, StackType::Obj | StackType::Ref) && lt.is_integer() {
            if lt == StackType::I4 {
                left = self.conv(left, PrimitiveType::I, false, Sign::None);
            }
            let target = self.result_type(left).to_primitive(Sign::None);
            right = self.conv(right, target, false, Sign::None);
        }
        self.make_explicit(
            &mut left,
            &mut right,
            StackType::I4,
            StackType::I,
            PrimitiveType::I,
        );
        self.make_explicit(
            &mut left,
            &mut right,
            StackType::I4,
            StackType::I8,
            PrimitiveType::I8,
        );
        self.make_explicit(
            &mut left,
            &mut right,
            StackType::I,
            StackType::I8,
            PrimitiveType::I8,
        );
        let (lt, rt) = (self.result_type(left), self.result_type(right));
        if lt.is_float() && rt.is_float() {
            if lt != rt {
                self.make_explicit(
                    &mut left,
                    &mut right,
                    StackType::F4,
                    StackType::F8,
                    PrimitiveType::R8,
                );
            }
            let input = self.result_type(left);
            if un {
                let c = self.add(Inst::Comp {
                    kind: kind.negate(),
                    sign: Sign::None,
                    input,
                    left,
                    right,
                });
                logic_not(&mut self.f, c)
            } else {
                self.add(Inst::Comp {
                    kind,
                    sign: Sign::None,
                    input,
                    left,
                    right,
                })
            }
        } else if lt.is_integer() && rt.is_integer() && !kind.is_equality_or_inequality() {
            self.add(Inst::Comp {
                kind,
                sign: if un { Sign::Unsigned } else { Sign::Signed },
                input: lt,
                left,
                right,
            })
        } else if lt == rt {
            self.add(Inst::Comp {
                kind,
                sign: Sign::None,
                input: lt,
                left,
                right,
            })
        } else {
            self.warn(&format!(
                "Invalid comparison between {} and {}",
                lt.name(),
                rt.name()
            ));
            if lt < rt {
                left = self.conv(left, rt.to_primitive(Sign::None), false, Sign::Signed);
            } else {
                right = self.conv(right, lt.to_primitive(Sign::None), false, Sign::Signed);
            }
            let input = self.result_type(left);
            self.add(Inst::Comp {
                kind,
                sign: Sign::None,
                input,
                left,
                right,
            })
        }
    }

    fn make_explicit(
        &mut self,
        left: &mut NodeId,
        right: &mut NodeId,
        source: StackType,
        target: StackType,
        conversion: PrimitiveType,
    ) {
        let (lt, rt) = (self.result_type(*left), self.result_type(*right));
        if lt == source && rt == target {
            *left = self.conv(*left, conversion, false, Sign::None);
        } else if lt == target && rt == source {
            *right = self.conv(*right, conversion, false, Sign::None);
        }
    }

    fn branch_or_invalid(&mut self, target: i32) -> NodeId {
        if !self.is_invalid_branch(target) {
            self.mark_branch_target(target);
            self.add(Inst::Branch {
                target: BranchTarget::Offset(target),
            })
        } else {
            self.invalid_branch("Invalid branch target")
        }
    }

    fn comparison_branch(&mut self, op: u16, kind: ComparisonKind, un: bool) -> Result<NodeId> {
        let start = self.code.offset as i32 - 1;
        let target = self.code.branch_target(op)?;
        let condition = self.comparison(kind, un);
        let end = self.code.offset as i32;
        self.f.add_il_range(condition, Interval::new(start, end));
        let true_inst = self.branch_or_invalid(target);
        let false_inst = self.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        Ok(self.add(Inst::If {
            condition,
            true_inst,
            false_inst,
        }))
    }

    fn conditional_branch(&mut self, op: u16, negate: bool) -> Result<NodeId> {
        let target = self.code.branch_target(op)?;
        let mut condition = self.pop();
        let cmp = if negate {
            ComparisonKind::Equality
        } else {
            ComparisonKind::Inequality
        };
        condition = match self.result_type(condition) {
            StackType::Obj => {
                let null = self.add(Inst::LdNull);
                self.add(Inst::Comp {
                    kind: cmp,
                    sign: Sign::None,
                    input: StackType::Obj,
                    left: condition,
                    right: null,
                })
            }
            StackType::I => {
                let zero = ldc_i4(&mut self.f, 0);
                let zero = self.conv(zero, PrimitiveType::I, false, Sign::None);
                self.add(Inst::Comp {
                    kind: cmp,
                    sign: Sign::None,
                    input: StackType::I,
                    left: condition,
                    right: zero,
                })
            }
            StackType::I8 => {
                let zero = self.add(Inst::LdcI8 { value: 0 });
                self.add(Inst::Comp {
                    kind: cmp,
                    sign: Sign::None,
                    input: StackType::I8,
                    left: condition,
                    right: zero,
                })
            }
            StackType::Ref => {
                let left = self.conv(condition, PrimitiveType::I, false, Sign::None);
                let zero = ldc_i4(&mut self.f, 0);
                let zero = self.conv(zero, PrimitiveType::I, false, Sign::None);
                self.add(Inst::Comp {
                    kind: cmp,
                    sign: Sign::None,
                    input: StackType::I,
                    left,
                    right: zero,
                })
            }
            StackType::I4 => {
                if negate {
                    logic_not(&mut self.f, condition)
                } else {
                    condition
                }
            }
            _ => {
                let c = self.conv(condition, PrimitiveType::I4, false, Sign::None);
                if negate { logic_not(&mut self.f, c) } else { c }
            }
        };
        let true_inst = self.branch_or_invalid(target);
        let false_inst = self.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        Ok(self.add(Inst::If {
            condition,
            true_inst,
            false_inst,
        }))
    }

    fn unconditional_branch(&mut self, op: u16, is_leave: bool) -> Result<NodeId> {
        let target = self.code.branch_target(op)?;
        if is_leave {
            self.flush_expression_stack();
            self.current_stack.clear();
        }
        Ok(self.branch_or_invalid(target))
    }

    fn decode_switch(&mut self) -> Result<NodeId> {
        let targets = self.code.switch_targets()?;
        let value = self.pop_as(StackType::I4);
        let mut sections = Vec::with_capacity(targets.len() + 1);
        for (i, &target) in targets.iter().enumerate() {
            let body = self.branch_or_invalid(target);
            sections.push(self.add(Inst::SwitchSection {
                labels: LongSet::single(i as i64),
                body,
            }));
        }
        let body = self.add(Inst::Nop {
            kind: NopKind::Normal,
        });
        let labels = LongSet::from_interval(LongInterval::new(0, targets.len() as i64)).invert();
        sections.push(self.add(Inst::SwitchSection { labels, body }));
        Ok(self.add(Inst::Switch { value, sections }))
    }

    fn binary(&mut self, op: BinaryNumericOperator, check: bool, sign: Sign) -> Decoded {
        let mut right = self.pop();
        let mut left = self.pop();
        if !matches!(op, BinaryNumericOperator::Add | BinaryNumericOperator::Sub) {
            if self.result_type(left) == StackType::Ref {
                left = self.conv(left, PrimitiveType::I, false, Sign::None);
            }
            if self.result_type(right) == StackType::Ref {
                right = self.conv(right, PrimitiveType::I, false, Sign::None);
            }
        }
        if !matches!(
            op,
            BinaryNumericOperator::ShiftLeft | BinaryNumericOperator::ShiftRight
        ) {
            self.make_explicit(
                &mut left,
                &mut right,
                StackType::I4,
                StackType::I,
                PrimitiveType::I,
            );
            self.make_explicit(
                &mut left,
                &mut right,
                StackType::I4,
                StackType::I8,
                PrimitiveType::I8,
            );
            self.make_explicit(
                &mut left,
                &mut right,
                StackType::I,
                StackType::I8,
                PrimitiveType::I8,
            );
            self.make_explicit(
                &mut left,
                &mut right,
                StackType::F4,
                StackType::F8,
                PrimitiveType::R8,
            );
        }
        let (left_input, right_input) = (self.result_type(left), self.result_type(right));
        let i = self.add(Inst::BinaryNumeric {
            op,
            check_overflow: check,
            sign,
            left_input,
            right_input,
            left,
            right,
        });
        self.push(i)
    }

    fn decode_jmp(&mut self) -> Result<NodeId> {
        let method = self.read_method()?;
        let m = method.method.clone();
        let mut args = Vec::new();
        if !m.is_static {
            args.push(self.ldarg(0));
        }
        for _ in m.params()? {
            let index = args.len() as i32;
            args.push(self.ldarg(index));
        }
        let call = self.add(Inst::Call(CallData {
            method,
            args,
            is_tail: true,
            constrained_to: None,
            il_stack_was_empty: true,
        }));
        let container = self.main_container;
        Ok(self.add(Inst::Leave {
            target: Some(container),
            value: call,
        }))
    }

    fn ldtoken(&mut self, token: u32) -> Result<NodeId> {
        match token >> 24 {
            0x01 | 0x02 | 0x1B => {
                let ty = self.type_operand(token)?;
                Ok(self.add(Inst::LdTypeToken { ty }))
            }
            0x06 | 0x2B => {
                let method = self.ts.resolve_method(self.module, token, self.ctx)?;
                Ok(self.add(Inst::LdMemberToken {
                    member: MemberOperand::Method(MethodOperand { token, method }),
                }))
            }
            0x04 | 0x0A => {
                if let Some(field) = self.resolve_field(token)? {
                    return Ok(self.add(Inst::LdMemberToken {
                        member: MemberOperand::Field(FieldOperand { token, field }),
                    }));
                }
                let method = self.ts.resolve_method(self.module, token, self.ctx)?;
                Ok(self.add(Inst::LdMemberToken {
                    member: MemberOperand::Method(MethodOperand { token, method }),
                }))
            }
            _ => Err(Error::Malformed(
                "Invalid metadata token for ldtoken instruction.",
            )),
        }
    }

    /// The rest of `ReadIL` after `ReadInstructions`: the block structure, the variable list
    /// and the removal of unreachable blocks.
    fn finish(mut self) -> Result<ReadFunction> {
        let code_size = self.code.len() as i32;
        let blocks: Vec<NodeId> = self.blocks.iter().map(|b| b.block).collect();
        let int32 = self.ts.known_type(KnownTypeCode::Int32);
        let clauses = self.body.clauses.clone();
        let main = self.main_container;
        let on_error_variables = BlockBuilder::new(&mut self.f, int32).create_blocks(
            self.ts,
            &clauses,
            &self.variable_by_handler,
            main,
            code_size,
            &blocks,
        )?;
        self.f.body = Some(main);
        let mut variables = self.parameter_variables.clone();
        variables.extend(&self.local_variables);
        variables.extend(&self.stack_variables);
        variables.extend(self.variable_by_handler.iter().flatten());
        variables.extend(on_error_variables);
        let mut removed = Vec::new();
        for c in self.f.descendants_post(main) {
            if !matches!(self.f.inst(c), Inst::BlockContainer { .. }) {
                continue;
            }
            let order = topological_sort(&self.f, c);
            if order.len() < self.f.list(c).len() {
                removed.extend(
                    self.f
                        .list(c)
                        .iter()
                        .copied()
                        .filter(|b| !order.contains(b)),
                );
            }
            self.f.set_list(c, order);
        }
        let mut warnings = Vec::new();
        if !removed.is_empty() {
            removed.sort_by_key(|&b| self.f.il_range(b).start);
            let offsets: Vec<String> = removed
                .iter()
                .map(|&b| format!("IL_{:04x}", self.f.il_range(b).start))
                .collect();
            warnings.push(format!(
                "Discarded unreachable code: {}",
                offsets.join(", ")
            ));
        }
        warnings.append(&mut self.warnings);
        self.sequence_point_candidates.sort_unstable();
        Ok(ReadFunction {
            function: self.f,
            variables,
            code_size,
            local_signature_length: self.local_variables.len(),
            async_return_type: self.async_return_type,
            warnings,
            sequence_point_candidates: self.sequence_point_candidates,
        })
    }
}

/// `ILReader.ReadIL` for a method definition read in its own generic context.
pub fn read_il<'c, 'a>(
    ts: &mut TypeSystem<'c, 'a>,
    method: MethodDefId,
    body: MethodBody<'a>,
    options: ReaderOptions<'_>,
) -> Result<ReadFunction> {
    let mut reader = Reader::new(ts, method, body, options)?;
    reader.read_instructions()?;
    reader.finish()
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum CallKind {
    Call,
    CallVirt,
    NewObj,
}

fn is_valid_stack_type_merge(a: StackType, b: StackType) -> bool {
    use StackType::*;
    matches!((a, b), (I, I4) | (I4, I) | (F4, F8) | (F8, F4)) || a == Unknown || b == Unknown
}

/// `ILReader.Cast`: makes the conversions IL performs implicitly explicit.
pub fn cast(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
    expected: StackType,
    warnings: Option<&mut Vec<String>>,
    il_offset: i32,
) -> NodeId {
    let actual = f.result_type(ts, inst);
    if expected == actual {
        return inst;
    }
    let warn = |message: String| {
        if let Some(w) = warnings {
            w.push(format!("IL_{il_offset:04x}: {message}"));
        }
    };
    if let Inst::InvalidExpression { expected: e, .. } = f.inst_mut(inst) {
        *e = expected;
        return inst;
    }
    let c = |f: &mut Function, ts: &mut TypeSystem<'_, '_>, arg, target, sign| {
        conv(f, ts, arg, target, false, sign)
    };
    match (expected, actual) {
        (StackType::I, StackType::I4) => c(f, ts, inst, PrimitiveType::I, Sign::None),
        (StackType::I4, StackType::I) => c(f, ts, inst, PrimitiveType::I4, Sign::None),
        (StackType::Unknown, _) => c(f, ts, inst, PrimitiveType::Unknown, Sign::None),
        (_, StackType::Ref) => {
            let pointer = c(f, ts, inst, PrimitiveType::I, Sign::None);
            match expected {
                StackType::I4 => c(f, ts, pointer, PrimitiveType::I4, Sign::None),
                StackType::I => pointer,
                StackType::I8 => c(f, ts, pointer, PrimitiveType::I8, Sign::None),
                _ => {
                    warn(format!(
                        "Expected {}, but got {}",
                        expected.name(),
                        StackType::Ref.name()
                    ));
                    c(
                        f,
                        ts,
                        pointer,
                        expected.to_primitive(Sign::None),
                        Sign::None,
                    )
                }
            }
        }
        (StackType::Ref, _) => {
            if !actual.is_integer() && actual != StackType::Obj {
                warn(format!(
                    "Expected {}, but got {}",
                    expected.name(),
                    actual.name()
                ));
            }
            c(f, ts, inst, PrimitiveType::Ref, Sign::None)
        }
        (StackType::F8, StackType::F4) => c(f, ts, inst, PrimitiveType::R8, Sign::Signed),
        (StackType::F4, StackType::F8) => c(f, ts, inst, PrimitiveType::R4, Sign::Signed),
        _ => {
            warn(format!(
                "Expected {}, but got {}",
                expected.name(),
                actual.name()
            ));
            c(f, ts, inst, expected.to_primitive(Sign::None), Sign::Signed)
        }
    }
}

/// `UnionFind<ILVariable>`.
#[derive(Default)]
pub(super) struct UnionFind {
    parent: HashMap<VarId, VarId>,
    rank: HashMap<VarId, u32>,
}

impl UnionFind {
    pub(super) fn find(&mut self, v: VarId) -> VarId {
        let p = *self.parent.get(&v).unwrap_or(&v);
        if p == v {
            return v;
        }
        let root = self.find(p);
        self.parent.insert(v, root);
        root
    }

    pub(super) fn merge(&mut self, a: VarId, b: VarId) {
        let (ra, rb) = (self.find(a), self.find(b));
        if ra == rb {
            return;
        }
        let (ka, kb) = (
            *self.rank.get(&ra).unwrap_or(&0),
            *self.rank.get(&rb).unwrap_or(&0),
        );
        if ka < kb {
            self.parent.insert(ra, rb);
        } else if ka > kb {
            self.parent.insert(rb, ra);
        } else {
            self.parent.insert(rb, ra);
            self.rank.insert(ra, ka + 1);
        }
    }
}
