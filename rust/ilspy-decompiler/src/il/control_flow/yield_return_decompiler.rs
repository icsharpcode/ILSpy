//! `IL/ControlFlow/YieldReturnDecompiler.cs`: turns a method that only creates a
//! compiler-generated enumerator back into an iterator, by decompiling the enumerator's
//! `MoveNext()` and translating its state machine into `yield return` / `yield break`.
//!
//! The helper methods of the enumerator class are read into arenas of their own (the managed
//! code reads them into separate `ILFunction`s); the converted `MoveNext()` body and the
//! decompiled finally methods are imported into the iterator's arena.

use std::collections::{HashMap, HashSet};

use crate::il::ast::{
    BranchTarget, ComparisonKind, ContainerKind, FieldOperand, Function, Inst, MethodOperand,
    NodeId, NopKind, VarId, VariableKind,
};
use crate::il::reader::{ReadFunction, read_il_in_context};
use crate::il::transforms::copy_propagation;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::il::transforms::split_variables::remove_dead_variables;
use crate::il::transforms::{Context, run_early_il_transforms};
use crate::il::types::{Sign, StackType};
use crate::metadata::Metadata;
use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{
    ModuleId, is_known_type_name, type_def_full_name, type_ref_full_name,
};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::{MethodDefId, has_body};
use crate::typesystem::resolve::{FieldOrigin, MethodOrigin, ResolvedField};
use crate::typesystem::types::{GenericContext, TypeId, TypeSystem};
use crate::util::interval::Interval;
use crate::util::long_dict::LongDict;
use crate::util::long_set::LongSet;

use super::state_range_analysis::{MethodKey, Mode, OrderedMap, StateRangeAnalysis};
use super::symbolic_execution::{FieldDef, SymbolicAnalysisFailed, field_definition};

pub(crate) type Result<T> = std::result::Result<T, SymbolicAnalysisFailed>;

pub(crate) fn fail<T>(message: impl Into<String>) -> Result<T> {
    Err(SymbolicAnalysisFailed(message.into()))
}

pub(crate) fn def_of(field: &FieldOperand) -> Option<FieldDef> {
    field_definition(&field.field.origin)
}

/// `field.MemberDefinition.Equals(other)` against an optional definition.
pub(crate) fn is_field(field: Option<FieldDef>, other: Option<FieldDef>) -> bool {
    field.is_some() && field == other
}

/// The MethodDef row a method's `MetadataToken` names, 0 for a method without one.
pub(crate) fn def_row(m: &MethodOperand) -> u32 {
    match m.method.origin {
        MethodOrigin::Def(d) => d.row,
        _ => 0,
    }
}

pub(crate) fn is_known(ts: &TypeSystem<'_, '_>, ty: TypeId, code: KnownTypeCode) -> bool {
    ts.definition(ty)
        .and_then(|d| ts.comp.type_def(d).ok())
        .is_some_and(|i| i.known == code)
}

pub(crate) fn new_block(f: &mut Function) -> NodeId {
    let final_inst = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Block {
        kind: crate::il::ast::BlockKind::ControlFlow,
        instructions: Vec::new(),
        final_inst,
    })
}

pub(crate) fn new_container(f: &mut Function) -> NodeId {
    f.add(Inst::BlockContainer {
        kind: ContainerKind::Normal,
        expected: None,
        blocks: Vec::new(),
    })
}

pub(crate) fn branch(f: &mut Function, target: NodeId) -> NodeId {
    f.add(Inst::Branch {
        target: BranchTarget::Block(target),
    })
}

pub(crate) fn invalid_branch(f: &mut Function, message: String) -> NodeId {
    f.add(Inst::InvalidBranch {
        message,
        expected: StackType::Void,
    })
}

/// `SingleBlock`: the body as one basic block.
pub(crate) fn single_block(f: &Function, body: NodeId) -> Option<NodeId> {
    match f.inst(body) {
        Inst::Block { .. } => Some(body),
        Inst::BlockContainer { blocks, .. } if blocks.len() == 1 => Some(blocks[0]),
        _ => None,
    }
}

/// `AsyncAwaitDecompiler.GetBodyEntryPoint`: the entry point, following branches into blocks
/// that are only reached from there.
pub fn get_body_entry_point(f: &Function, body: NodeId) -> NodeId {
    let mut entry = f.list(body)[0];
    while let Some(&first) = f.list(entry).first()
        && let Some(target) = f.match_branch(first)
        && f.node(target).incoming_edge_count == 1
        && f.node(target).parent == Some(body)
    {
        entry = target;
    }
    entry
}

/// `Block.GetPredecessor`: the instruction before `inst` in its block.
pub(crate) fn predecessor(f: &Function, inst: NodeId) -> Option<NodeId> {
    let parent = f.node(inst).parent?;
    let index = f.node(inst).child_index as usize;
    match f.inst(parent) {
        Inst::Block { instructions, .. } if index > 0 => instructions.get(index - 1).copied(),
        _ => None,
    }
}

pub(crate) fn try_finallys(f: &Function, root: NodeId) -> Vec<NodeId> {
    f.descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::TryFinally { .. }))
        .collect()
}

/// `IsMethod`: the method has `name`, or explicitly implements a method of that name.
fn is_method(md: &Metadata<'_>, row: u32, name: &str) -> bool {
    let Ok(def) = md.get::<rows::MethodDef>(row) else {
        return false;
    };
    if md.string(def.name).is_ok_and(|n| n == name) {
        return true;
    }
    let Ok(declaring) = md.method_declaring_type(row) else {
        return false;
    };
    for imp in md.type_method_impls(declaring) {
        let Ok(imp) = md.get::<rows::MethodImpl>(imp) else {
            continue;
        };
        if !matches!(imp.body.decode(), Ok((TableId::MethodDef, r)) if r == row) {
            continue;
        }
        let declared = match imp.declaration.decode() {
            Ok((TableId::MethodDef, r)) => md.get::<rows::MethodDef>(r).map(|d| d.name),
            Ok((TableId::MemberRef, r)) => match md.get::<rows::MemberRef>(r) {
                // A field reference (signature header 6) is not a method declaration.
                Ok(mr) if md.blob(mr.signature).is_ok_and(|s| s.first() != Some(&6)) => Ok(mr.name),
                _ => continue,
            },
            _ => continue,
        };
        if declared.is_ok_and(|n| md.string(n).is_ok_and(|n| n == name)) {
            return true;
        }
    }
    false
}

/// The first method of `type_def` that `is_method` accepts, 0 if none.
pub(crate) fn find_method(md: &Metadata<'_>, type_def: u32, name: &str) -> u32 {
    md.type_methods(type_def)
        .unwrap_or_default()
        .into_iter()
        .find(|&m| is_method(md, m, name))
        .unwrap_or(0)
}

pub(crate) fn is_compiler_generated(md: &Metadata<'_>, type_def: u32) -> bool {
    md.custom_attributes(TableId::TypeDef, type_def)
        .into_iter()
        .any(|ca| {
            md.attribute_type(ca).is_ok_and(|t| {
                is_known_type_name(
                    md,
                    t,
                    "System.Runtime.CompilerServices",
                    "CompilerGeneratedAttribute",
                    0,
                )
            })
        })
}

/// `IsCompilerGeneratorEnumerator`: a nested, compiler-generated class implementing
/// `System.Collections.IEnumerator`.
fn is_compiler_generated_enumerator(md: &Metadata<'_>, type_def: u32) -> bool {
    if type_def == 0 {
        return false;
    }
    let declaring = md.declaring_type(type_def).unwrap_or(0);
    let generated = is_compiler_generated(md, type_def)
        || (declaring != 0 && is_compiler_generated(md, declaring));
    if !generated || declaring == 0 {
        return false;
    }
    md.type_interface_impls(type_def).any(|ii| {
        let Ok(token) = md
            .get::<rows::InterfaceImpl>(ii)
            .and_then(|r| r.interface.token())
        else {
            return false;
        };
        let name = match token >> 24 {
            0x01 => type_ref_full_name(md, token & 0x00FF_FFFF),
            0x02 => type_def_full_name(md, token & 0x00FF_FFFF),
            // A generic instance is never the non-generic interface.
            _ => return false,
        };
        name.is_ok_and(|n| {
            n.nested.is_empty() && n.top.ns == "System.Collections" && n.top.name == "IEnumerator"
        })
    })
}

/// Raw IL decoding for `ValidateConstructor`; running past the end is an invalid body.
struct IlCursor<'a> {
    code: &'a [u8],
    pos: usize,
}

impl IlCursor<'_> {
    fn u8(&mut self) -> Option<u8> {
        let b = *self.code.get(self.pos)?;
        self.pos += 1;
        Some(b)
    }

    fn i32(&mut self) -> Option<i32> {
        let bytes = self.code.get(self.pos..self.pos + 4)?;
        self.pos += 4;
        Some(i32::from_le_bytes(bytes.try_into().ok()?))
    }

    /// `DecodeOpCodeSkipNop`: two-byte opcodes are `0xFE00 | second byte`.
    fn op_skip_nop(&mut self) -> Option<u16> {
        loop {
            let b = self.u8()?;
            let op = if b == 0xFE {
                0xFE00 | u16::from(self.u8()?)
            } else {
                u16::from(b)
            };
            if op != 0x00 {
                return Some(op);
            }
        }
    }

    /// `DecodeLdArg`.
    fn ldarg(&mut self, expected: u16) -> Option<bool> {
        Some(match self.op_skip_nop()? {
            0xFE09 => {
                let lo = self.u8()?;
                let hi = self.u8()?;
                u16::from_le_bytes([lo, hi]) == expected
            }
            0x0E => u16::from(self.u8()?) == expected,
            0x02 => expected == 0,
            0x03 => expected == 1,
            _ => false,
        })
    }
}

const OP_CALL: u16 = 0x28;
const OP_RET: u16 = 0x2A;

/// `CreateILAst`: reads a method of a compiler-generated class of the iterator's or async
/// method's type, in its generic context, and runs the early transforms over it.
pub fn create_il_ast(
    ctx: &mut Context<'_, '_, '_>,
    outer: MethodDefId,
    row: u32,
) -> Result<ReadFunction> {
    if row == 0 {
        return fail("Method not found");
    }
    let comp = ctx.ts.comp;
    let md = &comp.module(outer.module).md;
    let def = md
        .get::<rows::MethodDef>(row)
        .map_err(|e| SymbolicAnalysisFailed(e.to_string()))?;
    if !has_body(&def) {
        let name = md.string(def.name).unwrap_or_default();
        return fail(format!("Method {name} has no body"));
    }
    let body = md
        .method_body(def.rva)
        .map_err(|e| SymbolicAnalysisFailed(e.to_string()))?;
    let outer_context = ctx
        .ts
        .method_context(outer)
        .map_err(|e| SymbolicAnalysisFailed(e.to_string()))?;
    let generic_context = GenericContext {
        class: outer_context.class,
        method: None,
        class_tail: outer_context.method,
    };
    let method = MethodDefId {
        module: outer.module,
        row,
    };
    let mut read = read_il_in_context(ctx.ts, method, body, ctx.reader, Some(generic_context))
        .map_err(|e| SymbolicAnalysisFailed(e.to_string()))?;
    let mut inner = Context {
        ts: &mut *ctx.ts,
        settings: ctx.settings,
        reader: ctx.reader,
        rerun_requested: false,
        removed_variables: Vec::new(),
    };
    run_early_il_transforms(&mut read, &mut inner, true);
    Ok(read)
}

/// `ResolveIEnumerableIEnumeratorFieldMapping`: `GetEnumerator()` copies the parameter
/// fields into the fields the enumerator reads.
pub fn resolve_ienumerable_ienumerator_field_mapping(
    ctx: &mut Context<'_, '_, '_>,
    outer: MethodDefId,
    row: u32,
    field_to_parameter: &mut OrderedMap<FieldDef, VarId>,
) -> Result<()> {
    if row == 0 {
        // Maybe only an IEnumerator implementation.
        return Ok(());
    }
    let get_enumerator = create_il_ast(ctx, outer, row)?;
    let f = &get_enumerator.function;
    for block in f.descendants_post(f.body.expect("body")) {
        if !matches!(f.inst(block), Inst::Block { .. }) {
            continue;
        }
        for &inst in f.list(block) {
            // storeTarget.storeField = this.loadField;
            if let Some((_, store_field, value)) = f.match_stfld(inst)
                && let Some((load_target, load_field)) = f.match_ldfld(ctx.ts, value)
                && f.match_ld_this(load_target)
                && let Some(load) = def_of(load_field)
                && let Some(&p) = field_to_parameter.get(&load)
                && let Some(store) = def_of(store_field)
            {
                field_to_parameter.insert(store, p);
            }
        }
    }
    Ok(())
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.yield_return {
        return;
    }
    let comp = ctx.ts.comp;
    let md = &comp.module(r.method.module).md;
    let Ok(current_type) = md.method_declaring_type(r.method.row) else {
        return;
    };
    let mut d = YieldReturnDecompiler {
        outer: r.method,
        module: r.method.module,
        current_type,
        enumerator_type: 0,
        enumerator_ctor: 0,
        mono: false,
        visual_basic: false,
        legacy_visual_basic: false,
        dispose_method: 0,
        state_field: None,
        current_field: None,
        disposing_field: None,
        field_to_parameter: OrderedMap::default(),
        finally_method_to_state_range: None,
        decompiled_finally_methods: OrderedMap::default(),
        return_stores: Vec::new(),
        skip_finally_bodies: None,
        do_finally_bodies: None,
        cached_state_vars: Vec::new(),
    };
    d.run(r, ctx);
}

struct YieldReturnDecompiler {
    /// The iterator method being decompiled.
    outer: MethodDefId,
    module: ModuleId,
    /// TypeDef rows (0 for none) of the iterator's class and the enumerator class.
    current_type: u32,
    enumerator_type: u32,
    /// MethodDef rows, 0 for none.
    enumerator_ctor: u32,
    mono: bool,
    visual_basic: bool,
    /// Implies `visual_basic`.
    legacy_visual_basic: bool,
    dispose_method: u32,
    state_field: Option<FieldDef>,
    current_field: Option<FieldDef>,
    disposing_field: Option<FieldDef>,
    /// The enumerator fields holding the iterator's parameters (its arena's variables).
    field_to_parameter: OrderedMap<FieldDef, VarId>,
    /// From `Dispose()`: the states each finally method is called for.
    finally_method_to_state_range: Option<OrderedMap<MethodKey, (MethodOperand, LongSet)>>,
    /// The state a finally method enters, and its body until a try-finally takes it.
    decompiled_finally_methods: OrderedMap<MethodKey, (Option<i32>, Option<ReadFunction>)>,
    /// Stores of `yield break` temporaries.
    return_stores: Vec<NodeId>,
    skip_finally_bodies: Option<VarId>,
    do_finally_bodies: Option<VarId>,
    cached_state_vars: Vec<VarId>,
}

impl YieldReturnDecompiler {
    fn md<'c, 'a>(&self, ts: &TypeSystem<'c, 'a>) -> &'c Metadata<'a> {
        &ts.comp.module(self.module).md
    }

    fn run(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        if !self.match_enumerator_creation_pattern(r, ctx) {
            return;
        }
        let new_body = match self.analyze(r, ctx) {
            Ok(b) => b,
            Err(e) => {
                r.warnings
                    .push(format!("yield-return decompiler failed: {}", e.0));
                return;
            }
        };
        r.is_iterator = true;
        r.state_machine_compiled_with_mono = self.mono;
        r.state_machine_compiled_with_legacy_visual_basic = self.legacy_visual_basic;
        let old_body = r.function.body.expect("function body");
        set_body(&mut r.function, new_body);
        let mut registered: HashSet<VarId> = r.variables.iter().copied().collect();
        for n in r.function.descendants_post(new_body) {
            if let Some(v) = r.function.variable_of(n)
                && registered.insert(v)
            {
                r.variables.push(v);
            }
        }
        let parameters: Vec<(FieldDef, VarId)> = self.field_to_parameter.iter().copied().collect();
        for (field, p) in parameters {
            r.function.var_mut(p).state_machine_field = Some(field_operand(ctx.ts, field));
        }

        if self.mono || self.visual_basic {
            // Nested containers first, so that edges from their unreachable code into the main
            // container are gone before it is cleaned up.
            for block in r.function.list(new_body).to_vec() {
                for n in r.function.descendants_post(block) {
                    if matches!(r.function.inst(n), Inst::BlockContainer { .. }) {
                        sort_blocks(&mut r.function, n);
                    }
                }
            }
        }
        sort_blocks(&mut r.function, new_body);

        let cleaned = if self.mono {
            self.clean_skip_finally_bodies(r, ctx)
        } else if self.legacy_visual_basic {
            self.clean_do_finally_bodies(r);
            Ok(())
        } else if self.visual_basic {
            self.clean_finally_state_checks(r);
            Ok(())
        } else {
            self.decompile_finally_blocks(ctx)
                .map(|()| self.reconstruct_try_finally_blocks(r))
        };
        if let Err(e) = cleaned {
            r.is_iterator = false;
            r.state_machine_compiled_with_mono = false;
            r.state_machine_compiled_with_legacy_visual_basic = false;
            set_body(&mut r.function, old_body);
            remove_dead_variables(r);
            r.warnings
                .push(format!("yield-return decompiler failed: {}", e.0));
            return;
        }

        let body = r.function.body.expect("function body");
        let mut map = std::mem::take(&mut self.field_to_parameter);
        translate_fields_to_local_access(r, ctx, body, &mut map, self.mono);
        self.field_to_parameter = map;

        if self.mono || self.visual_basic {
            let f = &r.function;
            let stores_of = |v: VarId| -> Vec<NodeId> {
                f.var(v)
                    .store_instructions
                    .iter()
                    .copied()
                    .filter(|&s| matches!(f.inst(s), Inst::StLoc { .. }))
                    .collect()
            };
            let mut stores = Vec::new();
            if let Some(state) = self.state_field
                && let Some(&v) = self.field_to_parameter.get(&state)
            {
                stores.extend(stores_of(v));
            }
            for &v in &self.cached_state_vars {
                stores.extend(stores_of(v));
            }
            self.return_stores.extend(stores);
        }

        for store in std::mem::take(&mut self.return_stores) {
            let f = &mut r.function;
            let Some((v, _)) = f.match_stloc(store) else {
                continue;
            };
            let Some(block) = f.node(store).parent else {
                continue;
            };
            let index = f.node(store).child_index as usize;
            let in_block = matches!(f.inst(block), Inst::Block { instructions, .. }
                if instructions.get(index) == Some(&store));
            if f.var(v).load_count() == 0 && f.var(v).address_count() == 0 && in_block {
                f.remove_at(block, index);
            }
        }

        // TranslateFieldsToLocalAccess may have opened up new inlining opportunities.
        run_early_il_transforms(r, ctx, false);
    }

    fn analyze(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) -> Result<NodeId> {
        self.analyze_ctor(ctx)?;
        self.analyze_current_property(ctx)?;
        self.resolve_ienumerable_ienumerator_field_mapping(ctx)?;
        self.construct_exception_table(ctx)?;
        self.analyze_move_next(r, ctx)
    }

    fn create_il_ast(&self, ctx: &mut Context<'_, '_, '_>, row: u32) -> Result<ReadFunction> {
        create_il_ast(ctx, self.outer, row)
    }

    fn match_enumerator_creation_pattern(
        &mut self,
        r: &ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) -> bool {
        let f = &r.function;
        let Some(body) = f.body.and_then(|b| single_block(f, b)) else {
            return false;
        };
        let list = f.list(body).to_vec();
        if list.is_empty() {
            return false;
        }
        let md = self.md(ctx.ts);
        if list.len() == 1 {
            // No parameters passed to the enumerator (not even 'this'): ret(newobj(...)).
            let Some(new_obj) = f.match_return(list[0]) else {
                return false;
            };
            if self.match_enumerator_creation_new_obj(f, md, new_obj, 1) {
                return true;
            }
            if self.match_enumerator_creation_new_obj(f, md, new_obj, 0) {
                self.mono = true;
                return true;
            }
            return false;
        }

        // With parameters, the instance is stored in a variable, the parameters are copied
        // into it, and the instance is returned.
        let Some((var1, new_obj)) = f.match_stloc(list[0]) else {
            return false;
        };
        if self.match_enumerator_creation_new_obj(f, md, new_obj, 1) {
        } else if self.match_enumerator_creation_new_obj(f, md, new_obj, 0) {
            let Inst::NewObj(call) = f.inst(new_obj) else {
                unreachable!("matched a newobj");
            };
            if self.validate_constructor(ctx.ts, &call.method) {
                self.mono = true;
            } else {
                self.visual_basic = true;
                self.legacy_visual_basic = true;
            }
        } else {
            return false;
        }
        let mut pos = 1;

        let mut state_field_initialized = false;
        while pos < list.len() {
            // stfld(..., ldloc(var_1), ldloc(parameter)) or, in structs,
            // stfld(..., ldloc(var_1), ldobj(ldloc(this))).
            let Some((target, stored, value)) = f.match_stfld(list[pos]) else {
                break;
            };
            if f.match_ldloc(target) != Some(var1) {
                return false;
            }
            let field = def_of(stored);
            if let Some(p) = f.match_ldloc(value)
                && f.var(p).kind == VariableKind::Parameter
            {
                if let Some(field) = field {
                    self.field_to_parameter.insert(field, p);
                }
            } else if let Inst::LdObj { target: this, .. } = *f.inst(value)
                && f.match_ld_this(this)
            {
                if let Some(field) = field {
                    self.field_to_parameter
                        .insert(field, f.match_ldloc(this).expect("ldloc this"));
                }
            } else if (self.mono || self.legacy_visual_basic)
                && matches!(f.match_ldc_i4(value), Some(-2..=0))
            {
                self.state_field = field;
                state_field_initialized = true;
            } else {
                return false;
            }
            pos += 1;
        }

        // Debug builds may copy var_1 into var_2 before returning it.
        let mut var2 = None;
        if let Some((v2, value)) = list.get(pos).and_then(|&i| f.match_stloc(i)) {
            var2 = Some(v2);
            if f.match_ldloc(value) == Some(var1) {
                pos += 1;
            }
        }
        let returned = var2.unwrap_or(var1);
        if self.mono && !state_field_initialized {
            // Mono initializes the state field separately, unless it stays 0.
            if let Some((target, field, value)) = list.get(pos).and_then(|&i| f.match_stfld(i))
                && f.match_ldloc(target) == Some(returned)
                && matches!(f.match_ldc_i4(value), Some(-2 | 0))
            {
                self.state_field = def_of(field);
                pos += 1;
            }
        }
        list.get(pos)
            .and_then(|&i| f.match_return(i))
            .is_some_and(|v| f.match_ldloc(v) == Some(returned))
    }

    /// `MatchEnumeratorCreationNewObj` (one argument, the initial state -2 or 0) and
    /// `MatchMonoEnumeratorCreationNewObj` (no arguments).
    fn match_enumerator_creation_new_obj(
        &mut self,
        f: &Function,
        md: &Metadata<'_>,
        inst: NodeId,
        args: usize,
    ) -> bool {
        let Inst::NewObj(call) = f.inst(inst) else {
            return false;
        };
        if call.args.len() != args {
            return false;
        }
        if args == 1 && !matches!(f.match_ldc_i4(call.args[0]), Some(-2 | 0)) {
            return false;
        }
        self.enumerator_ctor = def_row(&call.method);
        self.enumerator_type = if self.enumerator_ctor == 0 {
            0
        } else {
            md.method_declaring_type(self.enumerator_ctor).unwrap_or(0)
        };
        let declaring = if self.enumerator_type == 0 {
            0
        } else {
            md.declaring_type(self.enumerator_type).unwrap_or(0)
        };
        declaring == self.current_type && is_compiler_generated_enumerator(md, self.enumerator_type)
    }

    /// `TransformDisplayClassUsage.ValidateConstructor` for a parameterless constructor:
    /// `ldarg.0; call object::.ctor(); ret`.
    fn validate_constructor(&self, ts: &mut TypeSystem<'_, '_>, method: &MethodOperand) -> bool {
        let Ok(params) = method.method.params() else {
            return false;
        };
        let MethodOrigin::Def(d) = method.method.origin else {
            return false;
        };
        // shortcut: the one-parameter form is unreachable from a newobj without arguments; port it with the display class transform
        if !params.is_empty() || d.module != self.module {
            return false;
        }
        let md = self.md(ts);
        let Ok(def) = md.get::<rows::MethodDef>(d.row) else {
            return false;
        };
        if def.rva == 0 {
            return false;
        }
        let Ok(body) = md.method_body(def.rva) else {
            return false;
        };
        if !body.clauses.is_empty() || body.code.len() < 7 {
            return false;
        }
        let mut c = IlCursor {
            code: body.code,
            pos: 0,
        };
        let mut check = || -> Option<bool> {
            if !c.ldarg(0)? || c.op_skip_nop()? != OP_CALL {
                return Some(false);
            }
            let token = c.i32()? as u32;
            if token & 0x00FF_FFFF == 0 {
                return Some(false);
            }
            let Ok(base) = ts.resolve_method(self.module, token, GenericContext::default()) else {
                return Some(false);
            };
            if !base
                .declaring
                .is_some_and(|t| is_known(ts, t, KnownTypeCode::Object))
            {
                return Some(false);
            }
            if !matches!(base.name.as_str(), ".ctor" | ".cctor")
                || !base.params().is_ok_and(|p| p.is_empty())
            {
                return Some(false);
            }
            Some(c.op_skip_nop()? == OP_RET)
        };
        check().unwrap_or(false)
    }

    /// `AnalyzeCtor`: the field the constructor stores its first parameter in is the state.
    fn analyze_ctor(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        let ctor = self.create_il_ast(ctx, self.enumerator_ctor)?;
        let f = &ctor.function;
        let Some(body) = single_block(f, f.body.expect("body")) else {
            return fail("Missing enumeratorCtor.Body");
        };
        for &inst in f.list(body) {
            if let Some((target, field, value)) = f.match_stfld(inst)
                && f.match_ld_this(target)
                && let Some(arg) = f.match_ldloc(value)
                && f.var(arg).kind == VariableKind::Parameter
                && f.var(arg).index == Some(0)
            {
                self.state_field = def_of(field);
            }
        }
        if self.state_field.is_none() && !self.mono {
            return fail("Could not find stateField");
        }
        Ok(())
    }

    /// `AnalyzeCurrentProperty`: the field `get_Current` returns.
    fn analyze_current_property(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        let row = find_method(self.md(ctx.ts), self.enumerator_type, "get_Current");
        let getter = self.create_il_ast(ctx, row)?;
        let f = &getter.function;
        let Some(body) = single_block(f, f.body.expect("body")) else {
            return fail("get_Current has no body");
        };
        match *f.list(body) {
            // Release builds: ret(ldfld F(ldloc(this))).
            [ret] => {
                if let Some(value) = f.match_return(ret)
                    && let Some((target, field)) = f.match_ldfld(ctx.ts, value)
                    && f.match_ld_this(target)
                {
                    self.current_field = def_of(field);
                }
            }
            // Debug builds: stloc V(ldfld F(ldloc(this))); ret(ldloc V).
            [store, ret] => {
                if let Some((v, value)) = f.match_stloc(store)
                    && let Some((target, field)) = f.match_ldfld(ctx.ts, value)
                    && f.match_ld_this(target)
                    && let Some(returned) = f.match_return(ret)
                    && f.match_ldloc(returned) == Some(v)
                {
                    self.current_field = def_of(field);
                }
            }
            _ => {}
        }
        if self.current_field.is_none() {
            return fail("Could not find currentField");
        }
        Ok(())
    }

    fn resolve_ienumerable_ienumerator_field_mapping(
        &mut self,
        ctx: &mut Context<'_, '_, '_>,
    ) -> Result<()> {
        let row = find_method(self.md(ctx.ts), self.enumerator_type, "GetEnumerator");
        resolve_ienumerable_ienumerator_field_mapping(
            ctx,
            self.outer,
            row,
            &mut self.field_to_parameter,
        )
    }

    /// `ConstructExceptionTable`: from `Dispose()`, the states each finally method runs for
    /// (or, for Mono and Visual Basic, only the disposing field).
    fn construct_exception_table(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        let md = self.md(ctx.ts);
        self.dispose_method = find_method(md, self.enumerator_type, "Dispose");
        let dispose = self.create_il_ast(ctx, self.dispose_method)?;
        let f = &dispose.function;
        let body = f.body.expect("body");
        if !self.visual_basic && !self.mono {
            'blocks: for &block in f.list(body) {
                for &inst in f.list(block) {
                    if let Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) = f.inst(inst)
                        && c.args.len() == 1
                        && f.match_ld_this(c.args[0])
                        && is_method(md, def_row(&c.method), "MoveNext")
                    {
                        self.visual_basic = true;
                        break 'blocks;
                    }
                }
            }
        }
        if self.mono || self.visual_basic {
            let entry = f.list(body)[0];
            for &inst in f.list(entry) {
                if matches!(f.inst(inst), Inst::Branch { .. }) {
                    break;
                }
                if let Some((target, field, value)) = f.match_stfld(inst)
                    && f.match_ld_this(target)
                    && is_known(ctx.ts, field.field.ty, KnownTypeCode::Boolean)
                    && f.match_ldc_i4(value) == Some(1)
                {
                    self.disposing_field = def_of(field);
                    break;
                }
            }
            // Mono and Visual Basic keep their try-finally blocks inline.
            self.finally_method_to_state_range = None;
        } else {
            let mut analysis =
                StateRangeAnalysis::new(Mode::IteratorDispose, self.state_field, None, false);
            analysis.assign_state_ranges(f, ctx.ts, body, LongSet::universe())?;
            self.finally_method_to_state_range =
                Some(std::mem::take(&mut analysis.finally_method_to_state_range));
        }
        Ok(())
    }

    /// `AnalyzeMoveNext`: converts the `MoveNext()` body and imports it into the iterator's
    /// arena.
    fn analyze_move_next(
        &mut self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) -> Result<NodeId> {
        let row = find_method(self.md(ctx.ts), self.enumerator_type, "MoveNext");
        let mut mn = self.create_il_ast(ctx, row)?;
        let token = 0x0600_0000 | row;
        let method = ctx
            .ts
            .resolve_method(self.module, token, GenericContext::default())
            .map_err(|e| SymbolicAnalysisFailed(e.to_string()))?;
        r.move_next_method = Some(MethodOperand {
            lifted: false,
            token,
            method,
            state_machine_context: false,
        });
        r.sequence_point_candidates = mn.sequence_point_candidates.clone();
        r.code_size = mn.code_size;
        r.local_signature_length = mn.local_signature_length;

        let root = mn.function.body.expect("body");
        // Copy-propagate temporaries holding a copy of 'this' (pre-Roslyn compilers make
        // them), then stack slots holding a 32-bit constant.
        for pass in 0..2 {
            let f = &mn.function;
            let stores: Vec<NodeId> = f
                .descendants_post(root)
                .into_iter()
                .filter(|&n| match *f.inst(n) {
                    Inst::StLoc { var, value, .. } => {
                        f.var(var).is_single_definition()
                            && if pass == 0 {
                                f.match_ld_this(value)
                            } else {
                                f.var(var).kind == VariableKind::StackSlot
                                    && matches!(f.inst(value), Inst::LdcI4 { .. })
                            }
                    }
                    _ => false,
                })
                .collect();
            for store in stores {
                copy_propagation::propagate(&mut mn.function, &mut mn.variables, ctx, store);
            }
        }
        let blocks: Vec<NodeId> = mn
            .function
            .descendants_post(root)
            .into_iter()
            .filter(|&n| matches!(mn.function.inst(n), Inst::Block { .. }))
            .collect();
        for block in blocks {
            mn.function
                .retain(block, |f, i| !matches!(f.inst(i), Inst::LdcI4 { .. }));
        }

        let f = &mut mn.function;
        let mut body = root;
        if let [block] = *f.list(body)
            && let [inst] = *f.list(block)
            && let Inst::TryFault {
                try_block,
                fault_block,
            } = *f.inst(inst)
        {
            body = try_block;
            let fault = match f.inst(fault_block) {
                Inst::BlockContainer { blocks, .. } if blocks.len() == 1 => blocks[0],
                _ => return fail("Unexpected number of blocks in MoveNext() fault block"),
            };
            let expected = match *f.list(fault) {
                [call, leave] => {
                    matches!(f.inst(call), Inst::Call(c)
                        if def_row(&c.method) == self.dispose_method
                            && c.args.len() == 1
                            && f.match_ld_this(c.args[0]))
                        && f.match_leave(leave)
                            .is_some_and(|(c, v)| c == fault_block && f.match_nop(v))
                }
                _ => false,
            };
            if !expected {
                return fail("Unexpected fault block contents in MoveNext()");
            }
        }

        if self.legacy_visual_basic
            && matches!(f.list(body).len(), 1 | 2)
            && let [first, second] = *f.list(f.list(body)[0])
            && let Some((first_var, one)) = f.match_stloc(first)
            && f.match_ldc_i4(one) == Some(1)
        {
            self.do_finally_bodies = Some(first_var);
            if let Inst::TryCatch {
                try_block,
                ref handlers,
            } = *f.inst(second)
                && handlers.len() == 1
            {
                let Inst::TryCatchHandler {
                    body: handler_body,
                    variable,
                    ..
                } = *f.inst(handlers[0])
                else {
                    unreachable!("try-catch handler");
                };
                let catch_block = match f.inst(handler_body) {
                    Inst::BlockContainer { blocks, .. } if blocks.len() == 1 => blocks[0],
                    _ => return fail("Unexpected number of blocks in MoveNext() catch block"),
                };
                let expected = match *f.list(catch_block) {
                    [call, store, state, rethrow] => {
                        matches!(f.inst(call), Inst::Call(c)
                            if c.method.method.name == "SetProjectError"
                                && c.args.len() == 1
                                && f.match_ldloc(c.args[0]) == Some(variable))
                            && f.match_stloc(store)
                                .is_some_and(|(_, v)| f.match_ldloc(v) == Some(variable))
                            && f.match_stfld(state).is_some_and(|(this, field, value)| {
                                f.match_ld_this(this)
                                    && is_field(def_of(field), self.state_field)
                                    && matches!(f.inst(value), Inst::LdcI4 { .. })
                            })
                            && matches!(f.inst(rethrow), Inst::Rethrow)
                    }
                    _ => false,
                };
                if !expected {
                    return fail("Unexpected catch block contents in MoveNext()");
                }
                // Move the return block into the try block.
                if f.list(body).len() == 2 {
                    let return_block = f.list(body)[1];
                    f.push(try_block, return_block);
                }
                body = try_block;
            }
        }

        if self.state_field.is_none() {
            // Mono may leave the state field implicitly 0: the first instruction of
            // MoveNext() loads it.
            let first = f.list(f.list(body)[0])[0];
            if let Some((_, value)) = f.match_stloc(first)
                && let Some((target, field)) = f.match_ldfld(ctx.ts, value)
                && f.match_ld_this(target)
                && is_known(ctx.ts, field.field.ty, KnownTypeCode::Int32)
            {
                self.state_field = def_of(field);
            } else {
                return fail("Could not find state field.");
            }
        }

        let mut skip_finally_bodies = None;
        if self.mono {
            // Mono guards its finally bodies with a skipFinallyBodies variable.
            let entry = f.list(body)[0];
            'search: for tf in try_finallys(f, body) {
                let Inst::TryFinally { finally_block, .. } = *f.inst(tf) else {
                    unreachable!("try-finally");
                };
                let Some(&first) = f.entry_point(finally_block).and_then(|e| f.list(e).first())
                else {
                    continue;
                };
                if let Inst::If { condition, .. } = *f.inst(first)
                    && let Some(arg) = f.match_logic_not(condition)
                    && let Some(v) = f.match_ldloc(arg)
                    && is_known(ctx.ts, f.var(v).ty, KnownTypeCode::Boolean)
                {
                    let initialized = f.list(entry).iter().take(3).any(|&i| {
                        f.match_stloc(i)
                            .is_some_and(|(s, value)| s == v && f.match_ldc_i4(value) == Some(0))
                    });
                    if initialized {
                        skip_finally_bodies = Some(v);
                        break 'search;
                    }
                }
            }
        }

        self.propagate_copies_of_fields(f, body);

        // Try blocks nested in `body` cannot contain yields, so only its own blocks matter
        // for the control flow.
        let mut analysis = StateRangeAnalysis::new(
            Mode::IteratorMoveNext,
            self.state_field,
            None,
            self.legacy_visual_basic,
        );
        analysis.skip_finally_bodies = skip_finally_bodies;
        analysis.do_finally_bodies = self.do_finally_bodies;
        analysis.assign_state_ranges(f, ctx.ts, body, LongSet::universe())?;
        let cached_state_vars = analysis.cached_state_vars().to_vec();

        let mut conv = Conversion {
            d: self,
            ts: ctx.ts,
            skip_finally_bodies,
            return_stores: Vec::new(),
        };
        let new_body = conv.convert_body(f, body, &mut analysis)?;
        let return_stores = conv.return_stores;
        mn.variables.clear();
        // The instructions moved into the new body are released along with the old tree.
        mn.function.release_root(root);

        let imported = r.function.import(mn.function);
        self.return_stores
            .extend(return_stores.into_iter().map(|n| imported.node(n)));
        self.skip_finally_bodies = skip_finally_bodies.map(|v| imported.var(v));
        self.do_finally_bodies = self.do_finally_bodies.map(|v| imported.var(v));
        self.cached_state_vars = cached_state_vars
            .into_iter()
            .map(|v| imported.var(v))
            .collect();
        Ok(imported.node(new_body))
    }

    /// `PropagateCopiesOfFields`: undoes Roslyn copying fields of the enumerator into locals
    /// at the start of `MoveNext()`.
    fn propagate_copies_of_fields(&self, f: &mut Function, body: NodeId) {
        let mutable_fields: HashSet<Option<FieldDef>> = f
            .descendants_post(body)
            .into_iter()
            .filter_map(|n| match f.inst(n) {
                Inst::LdFlda { field, .. }
                    if !f
                        .node(n)
                        .parent
                        .is_some_and(|p| matches!(f.inst(p), Inst::LdObj { .. })) =>
                {
                    Some(def_of(field))
                }
                _ => None,
            })
            .collect();
        let entry = f.list(body)[0];
        let mut i = 0;
        while let Some(&store) = f.list(entry).get(i) {
            let Some((v, value)) = f.match_stloc(store) else {
                break;
            };
            let field = match *f.inst(value) {
                Inst::LdObj { target, .. } => match f.inst(target) {
                    Inst::LdFlda {
                        target: this,
                        field,
                        ..
                    } if f.match_ld_this(*this) => def_of(field),
                    _ => break,
                },
                _ => break,
            };
            if !f.var(v).is_single_definition() {
                break;
            }
            if !mutable_fields.contains(&field) {
                // Unlike CopyPropagation, the ldobj's arguments are copied too.
                for load in f.var(v).load_instructions.clone() {
                    let copy = f.clone_subtree(value);
                    f.replace(load, copy);
                }
                f.remove_at(entry, i);
            } else if field == self.state_field {
                i += 1;
            } else {
                // A load of a mutable field other than the state field.
                break;
            }
        }
    }

    /// `GetNewState`: the state a block transitions to at its start.
    fn get_new_state(&self, f: &Function, block: NodeId) -> Option<i32> {
        let &first = f.list(block).first()?;
        if let Some((target, field, value)) = f.match_stfld(first)
            && f.match_ld_this(target)
            && is_field(def_of(field), self.state_field)
            && let Some(state) = f.match_ldc_i4(value)
        {
            return Some(state);
        }
        if let Inst::Call(c) = f.inst(first)
            && c.args.len() == 1
            && f.match_ld_this(c.args[0])
            && let Some((outer_state, _)) = self
                .decompiled_finally_methods
                .get(&MethodKey::of(&c.method))
        {
            return *outer_state;
        }
        None
    }

    fn is_state_assignment(&self, f: &Function, inst: NodeId) -> bool {
        f.match_stfld(inst).is_some_and(|(target, field, _)| {
            f.match_ld_this(target) && is_field(def_of(field), self.state_field)
        })
    }

    /// `DecompileFinallyBlocks`.
    fn decompile_finally_blocks(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        let methods: Vec<(MethodKey, MethodOperand)> = self
            .finally_method_to_state_range
            .as_ref()
            .expect("finally methods of Dispose()")
            .iter()
            .map(|(k, (m, _))| (*k, m.clone()))
            .collect();
        for (key, method) in methods {
            let mut finally = self.create_il_ast(ctx, def_row(&method))?;
            let f = &mut finally.function;
            let body = f.body.expect("body");
            let entry = f.list(body)[0];
            let new_state = self.get_new_state(f, entry);
            if new_state.is_some() {
                f.remove_at(entry, 0);
            }
            // Unrecognized state assignments in a finally method rule out the conversion.
            for inst in f.descendants_post(body) {
                if self.is_state_assignment(f, inst) {
                    return fail(format!(
                        "Unknown state transition in {} at IL_{:04x}",
                        method.method.name,
                        f.il_range(inst).start
                    ));
                }
            }
            f.release_root(body);
            self.decompiled_finally_methods
                .insert(key, (new_state, Some(finally)));
        }
        Ok(())
    }

    /// `ReconstructTryFinallyBlocks`: a state assignment opens a try block, a call to the
    /// finally method leaves it. The blocks are topologically sorted, so a try block's entry
    /// comes first.
    fn reconstruct_try_finally_blocks(&mut self, r: &mut ReadFunction) {
        let new_body = r.function.body.expect("body");
        let blocks = r.function.list(new_body).to_vec();
        let mut block_state = vec![0i32; blocks.len()];
        block_state[0] = -1;
        let mut state_to_container: HashMap<i32, NodeId> = HashMap::from([(-1, new_body)]);
        for (i, &block) in blocks.iter().enumerate() {
            let old_state = block_state[i];
            let (new_state, container) = match self.get_new_state(&r.function, block) {
                Some(new_state) => {
                    r.function.remove_at(block, 0);
                    let container = match state_to_container.get(&new_state) {
                        Some(&c) => c,
                        None => {
                            // The entry point of a try block: the block now holds the whole
                            // try-finally, which goes into the container of the old state.
                            self.create_try_block(r, block, new_state, &mut state_to_container);
                            state_to_container[&old_state]
                        }
                    };
                    (new_state, container)
                }
                None => (old_state, state_to_container[&old_state]),
            };
            let f = &mut r.function;
            if container != new_body {
                // The stale entry in the body keeps the other blocks' indices for now.
                f.push(container, block);
            }
            for br in f.descendants_post(block) {
                if !matches!(f.inst(br), Inst::Branch { .. }) {
                    continue;
                }
                let target = f.branch_target(br);
                if f.node(target).parent != Some(new_body) {
                    continue;
                }
                let mut state_after_branch = new_state;
                if let Some(p) = predecessor(f, br)
                    && let Inst::Call(c) = f.inst(p)
                    && c.args.len() == 1
                    && f.match_ld_this(c.args[0])
                    && def_row(&c.method) == self.dispose_method
                {
                    // Pre-Roslyn compilers turn "yield break;" into "Dispose(); goto
                    // return_false;": a transition to the final state.
                    state_after_branch = -1;
                    let nop = f.add(Inst::Nop {
                        kind: NopKind::Normal,
                    });
                    f.replace(p, nop);
                }
                block_state[f.node(target).child_index as usize] = state_after_branch;
            }
        }
        r.function
            .retain(new_body, |f, b| f.node(b).parent == Some(new_body));
    }

    fn create_try_block(
        &mut self,
        r: &mut ReadFunction,
        block: NodeId,
        state: i32,
        state_to_container: &mut HashMap<i32, NodeId>,
    ) {
        let finally_methods = self
            .finally_method_to_state_range
            .as_mut()
            .expect("finally methods of Dispose()");
        let finally_method = finally_methods
            .iter()
            .find(|(_, (_, range))| range.contains(i64::from(state)))
            .map(|(k, _)| *k);
        if let Some(k) = finally_method {
            // Nested try-finally blocks must not find it again.
            finally_methods.remove(&k);
        }

        let f = &mut r.function;
        let try_block = new_block(f);
        f.add_il_range(try_block, f.il_range(block));
        for inst in f.list(block).to_vec() {
            f.push(try_block, inst);
        }
        let try_container = new_container(f);
        f.push(try_container, try_block);
        f.add_il_range(try_container, f.il_range(try_block));
        state_to_container.insert(state, try_container);

        let finally_block = match finally_method {
            None => invalid_branch(
                f,
                format!(
                    "Could not find finallyMethod for state={state}.\n\
                     Possibly this method is affected by a C# compiler bug that causes the finally body\n\
                     not to run in case of an exception or early 'break;' out of a loop consuming this iterable."
                ),
            ),
            Some(k) => match self
                .decompiled_finally_methods
                .get_mut(&k)
                .and_then(|(_, finally)| finally.take())
            {
                Some(finally) => {
                    let body = finally.function.body.expect("body");
                    let imported = r.function.import(finally.function);
                    r.variables
                        .extend(finally.variables.iter().map(|&v| imported.var(v)));
                    imported.node(body)
                }
                None => invalid_branch(&mut r.function, "Missing decompiledFinallyMethod".into()),
            },
        };
        let f = &mut r.function;
        f.set_list(block, Vec::new());
        let try_finally = f.add(Inst::TryFinally {
            try_block: try_container,
            finally_block,
        });
        f.add_il_range(try_finally, f.il_range(try_container));
        f.push(block, try_finally);
    }

    /// `CleanSkipFinallyBodies`: with `yield` instructions, Mono's skipFinallyBodies is
    /// always false.
    fn clean_skip_finally_bodies(
        &self,
        r: &mut ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) -> Result<()> {
        let Some(skip) = self.skip_finally_bodies else {
            return Ok(());
        };
        if r.function.var(skip).store_count() != 0 || r.function.var(skip).address_count() != 0 {
            // Another variable misdetected; removing its initial store of 0 is harmless.
            return Ok(());
        }
        let body = r.function.body.expect("body");
        for tf in try_finallys(&r.function, body) {
            let f = &mut r.function;
            let Inst::TryFinally { finally_block, .. } = *f.inst(tf) else {
                unreachable!("try-finally");
            };
            if !matches!(f.inst(finally_block), Inst::BlockContainer { .. }) {
                continue;
            }
            let mut entry = get_body_entry_point(f, finally_block);
            if let Some(&first) = f.list(entry).first()
                && let Inst::If {
                    condition,
                    true_inst,
                    ..
                } = *f.inst(first)
                && f.match_logic_not(condition)
                    .is_some_and(|a| f.match_ldloc(a) == Some(skip))
            {
                f.replace(first, true_inst);
                f.truncate(entry, 1);
                entry = get_body_entry_point(f, finally_block);
            }
            if let [call, leave] = *f.list(entry)
                && let Some(row) = self.mono_finally_method(f, ctx.ts, call)
                && f.match_leave(leave)
                    .is_some_and(|(c, v)| c == finally_block && f.match_nop(v))
            {
                let finally = self.create_il_ast(ctx, row)?;
                let finally_body = finally.function.body.expect("body");
                // The function keeps its own reference to the body, as the managed
                // ILFunction does.
                let imported = r.function.import(finally.function);
                r.function
                    .replace(finally_block, imported.node(finally_body));
                r.variables
                    .extend(finally.variables.iter().map(|&v| imported.var(v)));
            }
        }
        Ok(())
    }

    /// `IsCallToMonoFinallyMethod`: `this.<>__FinallyN()` of the enumerator class.
    fn mono_finally_method(
        &self,
        f: &Function,
        ts: &TypeSystem<'_, '_>,
        call: NodeId,
    ) -> Option<u32> {
        let Inst::Call(c) = f.inst(call) else {
            return None;
        };
        if c.args.len() != 1 || !f.match_ld_this(c.args[0]) {
            return None;
        }
        if !c.method.method.name.starts_with("<>__Finally") {
            return None;
        }
        let declaring = c.method.method.declaring.and_then(|t| ts.definition(t))?;
        if declaring.module != self.module || declaring.row != self.enumerator_type {
            return None;
        }
        match c.method.method.origin {
            MethodOrigin::Def(d) => Some(d.row),
            _ => None,
        }
    }

    /// `CleanDoFinallyBodies`: with `yield` instructions, legacy Visual Basic's
    /// doFinallyBodies is always true.
    fn clean_do_finally_bodies(&self, r: &mut ReadFunction) {
        let Some(do_finally) = self.do_finally_bodies else {
            return;
        };
        let f = &mut r.function;
        if f.var(do_finally).store_count() != 0 || f.var(do_finally).address_count() != 0 {
            return;
        }
        let body = f.body.expect("body");
        for tf in try_finallys(f, body) {
            let Inst::TryFinally { finally_block, .. } = *f.inst(tf) else {
                unreachable!("try-finally");
            };
            if !matches!(f.inst(finally_block), Inst::BlockContainer { .. }) {
                continue;
            }
            let entry = get_body_entry_point(f, finally_block);
            if let Some(&first) = f.list(entry).first()
                && let Inst::If { condition, .. } = *f.inst(first)
                && let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality)
                && f.match_ldloc(left) == Some(do_finally)
                && f.match_ldc_i4(right) == Some(0)
            {
                f.remove_at(entry, 0);
            }
        }
        for load in f.var(do_finally).load_instructions.clone() {
            let one = f.add(Inst::LdcI4 { value: 1 });
            f.add_il_range(one, f.il_range(load));
            f.replace(load, one);
        }
    }

    /// `CleanFinallyStateChecks`: Visual Basic's `if (state >= 0)` in finally blocks.
    fn clean_finally_state_checks(&self, r: &mut ReadFunction) {
        let f = &mut r.function;
        let body = f.body.expect("body");
        for tf in try_finallys(f, body) {
            let Inst::TryFinally { finally_block, .. } = *f.inst(tf) else {
                unreachable!("try-finally");
            };
            if !matches!(f.inst(finally_block), Inst::BlockContainer { .. }) {
                continue;
            }
            let entry = get_body_entry_point(f, finally_block);
            if let Some(&first) = f.list(entry).first()
                && let Inst::If { condition, .. } = *f.inst(first)
                && let Inst::Comp {
                    kind: ComparisonKind::GreaterThanOrEqual,
                    input: StackType::I4,
                    sign: Sign::Signed,
                    left,
                    right,
                    ..
                } = *f.inst(condition)
                && f.match_ldloc(left)
                    .is_some_and(|v| self.cached_state_vars.contains(&v))
                && f.match_ldc_i4(right) == Some(0)
            {
                f.remove_at(entry, 0);
            }
        }
    }
}

/// `ILFunction.Body = body`: the new body connects before the old one is released, and its
/// parent is the function (no node).
pub(crate) fn set_body(f: &mut Function, body: NodeId) {
    let old = f.body.replace(body);
    f.nodes[body.0 as usize].parent = None;
    f.add_ref(body);
    if let Some(old) = old {
        f.release_ref(old);
    }
}

/// A field definition as an operand (`IField.MemberDefinition`).
pub(crate) fn field_operand(ts: &mut TypeSystem<'_, '_>, (module, row): FieldDef) -> FieldOperand {
    let md = &ts.comp.module(module).md;
    let name = md
        .get::<rows::Field>(row)
        .and_then(|f| md.string(f.name).map(|n| n.into_owned()))
        .unwrap_or_default();
    let declaring = md.field_declaring_type(row).unwrap_or(0);
    let declaring = (declaring != 0).then(|| {
        ts.intern_def(crate::typesystem::compilation::TypeDefId {
            module,
            row: declaring,
        })
    });
    let (ty, _) = ts.field_type(module, row);
    FieldOperand {
        token: 0x0400_0000 | row,
        field: ResolvedField {
            origin: FieldOrigin::Def { module, row },
            declaring,
            name,
            ty,
        },
        state_machine_context: false,
    }
}

/// `ConvertBody` and its helpers.
struct Conversion<'d, 't, 'c, 'a> {
    d: &'d YieldReturnDecompiler,
    ts: &'t mut TypeSystem<'c, 'a>,
    /// In the `MoveNext()` arena.
    skip_finally_bodies: Option<VarId>,
    return_stores: Vec<NodeId>,
}

/// The containers one `convert_body` call translates between.
struct Bodies {
    old: NodeId,
    new: NodeId,
    block_states: LongDict<NodeId>,
}

impl Conversion<'_, '_, '_, '_> {
    /// `ConvertBody`: rebuilds the blocks of `old_body` with `yield return`, `yield break`
    /// and gotos to the block of each next state, splitting blocks at state changes and
    /// finally method calls for the try-finally reconstruction.
    fn convert_body(
        &mut self,
        f: &mut Function,
        old_body: NodeId,
        analysis: &mut StateRangeAnalysis,
    ) -> Result<NodeId> {
        let d = self.d;
        let new_body = new_container(f);
        f.add_il_range(new_body, f.il_range(old_body));
        let bodies = Bodies {
            old: old_body,
            new: new_body,
            block_states: analysis.block_state_set_mapping(f, old_body),
        };
        let old_blocks = f.list(old_body).to_vec();
        // All new blocks exist first, so gotos can reference them.
        for &old in &old_blocks {
            let block = new_block(f);
            f.add_il_range(block, f.il_range(old));
            f.push(new_body, block);
        }
        for (i, &old_block) in old_blocks.iter().enumerate() {
            let mut new_block = f.list(new_body)[i];
            for old_inst in f.list(old_block).to_vec() {
                if let Some((target, field, value)) = f.match_stfld(old_inst)
                    && f.match_ld_this(target)
                {
                    let field = def_of(field);
                    if is_field(field, d.state_field) {
                        if f.match_ldc_i4(value).is_none() {
                            return fail("Assigned non-constant to iterator.state field");
                        }
                        // The state change stays as the first instruction of its own block.
                        new_block = split_block(f, &bodies, new_block, old_inst);
                    } else if is_field(field, d.current_field) {
                        let yield_return = f.add(Inst::YieldReturn { value });
                        f.add_il_range(yield_return, f.il_range(old_inst));
                        f.push(new_block, yield_return);
                        let pos = f.node(old_inst).child_index as usize + 1;
                        self.convert_branch_after_yield_return(
                            f, &bodies, new_block, old_block, pos,
                        )?;
                        break;
                    }
                } else if let Inst::Call(c) = f.inst(old_inst)
                    && c.args.len() == 1
                    && f.match_ld_this(c.args[0])
                    // shortcut: without a Dispose() analysis (Mono, Visual Basic) the managed lookup throws; here it never matches
                    && d.finally_method_to_state_range
                        .as_ref()
                        .is_some_and(|m| m.contains_key(&MethodKey::of(&c.method)))
                {
                    new_block = split_block(f, &bodies, new_block, old_inst);
                } else if let Inst::TryFinally { try_block, .. } = *f.inst(old_inst)
                    && (d.mono || d.visual_basic)
                {
                    // Mono and Visual Basic keep try-finally inline: convert its try block too.
                    let mut nested = analysis.create_nested();
                    nested.assign_state_ranges(f, self.ts, try_block, LongSet::universe())?;
                    let new_try = self.convert_body(f, try_block, &mut nested)?;
                    f.replace(try_block, new_try);
                } else if d.legacy_visual_basic
                    && let Some((condition, true_inst)) = f.match_if_instruction(old_inst)
                    && let Some((left, right)) = f.match_comp(condition, ComparisonKind::Equality)
                    && let Some((this, field)) = f.match_ldfld(self.ts, left)
                    && f.match_ld_this(this)
                    && is_field(def_of(field), d.disposing_field)
                    && f.match_ldc_i4(right) == Some(0)
                {
                    f.push(new_block, true_inst);
                    f.add_il_range(new_block, f.il_range(true_inst));
                    self.update_branch_targets(f, &bodies, true_inst);
                    break;
                }
                f.push(new_block, old_inst);
                f.add_il_range(new_block, f.il_range(old_inst));
                self.update_branch_targets(f, &bodies, old_inst);
            }
        }
        // A new entry point jumps to the initial state, so the compiler-generated state
        // dispatch becomes unreachable and is deleted.
        let initial_state = if d.legacy_visual_basic { -1 } else { 0 };
        let goto = make_goto(f, &bodies, initial_state)?;
        let entry = new_block(f);
        f.push(entry, goto);
        f.insert(new_body, 0, entry);
        Ok(new_body)
    }

    fn convert_branch_after_yield_return(
        &mut self,
        f: &mut Function,
        bodies: &Bodies,
        new_block: NodeId,
        mut old_block: NodeId,
        mut pos: usize,
    ) -> Result<()> {
        let d = self.d;
        let at = |f: &Function, block: NodeId, pos: usize| f.list(block).get(pos).copied();
        if d.mono && d.disposing_field.is_some() {
            // Mono skips the state assignment when 'this.disposing' is set:
            //   if (ldfld $disposing(ldloc this)) br exit
            //   br targetBlock   // stfld $PC(ldloc this, ldc.i4 1); br setSkipFinallyBodies
            if let Some(i) = at(f, old_block, pos)
                && let Some((condition, _)) = f.match_if_instruction(i)
                && let Some((target, field)) = f.match_ldfld(self.ts, condition)
                && f.match_ld_this(target)
                && is_field(def_of(field), d.disposing_field)
                && let Some(next) = at(f, old_block, pos + 1)
                && let Some(target_block) = f.match_branch(next)
                && f.node(target_block).parent == f.node(old_block).parent
            {
                old_block = target_block;
                pos = 0;
            }
        }

        // The Visual Basic compiler also stores the new state in a local.
        let mut local_new_state = None;
        if let Some(i) = at(f, old_block, pos)
            && let Some((_, value)) = f.match_stloc(i)
            && let Some(state) = f.match_ldc_i4(value)
        {
            local_new_state = Some(state);
            pos += 1;
        }

        let new_state = match at(f, old_block, pos).and_then(|i| f.match_stfld(i)) {
            Some((target, field, value))
                if f.match_ld_this(target) && is_field(def_of(field), d.state_field) =>
            {
                f.match_ldc_i4(value)
                    .filter(|&s| local_new_state.is_none_or(|l| l == s))
            }
            _ => None,
        };
        let Some(new_state) = new_state else {
            return fail("Unable to find new state assignment for yield return");
        };
        pos += 1;
        // Mono may branch to its setSkipFinallyBodies block here.
        if let Some(i) = at(f, old_block, pos)
            && let Some(target_block) = f.match_branch(i)
            && f.node(target_block).parent == f.node(old_block).parent
        {
            old_block = target_block;
            pos = 0;
        }
        if let Some(i) = at(f, old_block, pos)
            && let Some((v, value)) = f.match_stloc(i)
            && Some(v) == self.skip_finally_bodies
        {
            if f.match_ldc_i4(value) != Some(1) {
                return fail("Unexpected assignment to skipFinallyBodies");
            }
            pos += 1;
        }
        // SplitVariables splits legacy Visual Basic's doFinallyBodies, so the local index
        // identifies it.
        if let Some(i) = at(f, old_block, pos)
            && let Some((v, value)) = f.match_stloc(i)
            && f.var(v).kind == VariableKind::Local
            && f.var(v).index == d.do_finally_bodies.and_then(|x| f.var(x).index)
        {
            if f.match_ldc_i4(value) != Some(0) {
                return fail("Unexpected assignment to doFinallyBodies");
            }
            pos += 1;
        }

        let returns_true = |f: &Function, i: Option<NodeId>| {
            i.and_then(|i| f.match_return(i))
                .is_some_and(|v| f.match_ldc_i4(v) == Some(1))
        };
        let i = at(f, old_block, pos);
        let found = returns_true(f, i)
            // A jump to a common return block, e.g. on Mono.
            || i.and_then(|i| f.match_branch(i))
                .is_some_and(|t| returns_true(f, f.list(t).first().copied()));
        if !found {
            return fail("Unable to find 'return true' for yield return");
        }
        let goto = make_goto(f, bodies, new_state)?;
        f.push(new_block, goto);
        Ok(())
    }

    /// `UpdateBranchTargets`: redirects branches and leaves of the old body to the new one;
    /// a return becomes `yield break` (`leave` of the new body).
    fn update_branch_targets(&mut self, f: &mut Function, bodies: &Bodies, inst: NodeId) {
        match *f.inst(inst) {
            Inst::Branch {
                target: BranchTarget::Block(target),
            } => {
                if f.node(target).parent == Some(bodies.old) {
                    let new_target = f.list(bodies.new)[f.node(target).child_index as usize];
                    f.set_branch_target(inst, new_target);
                }
            }
            Inst::Leave {
                target: Some(container),
                value,
            } => {
                if Some(container) == f.body {
                    let mut valid_yield_break = f.match_ldc_i4(value) == Some(0);
                    if let Some(v) = f.match_ldloc(value)
                        && matches!(f.var(v).kind, VariableKind::Local | VariableKind::StackSlot)
                        && f.var(v).store_instructions.iter().all(|&s| {
                            f.match_stloc(s)
                                .is_some_and(|(_, value)| f.match_ldc_i4(value) == Some(0))
                        })
                    {
                        valid_yield_break = true;
                        self.return_stores
                            .extend(f.var(v).store_instructions.iter().copied());
                    }
                    let replacement = if valid_yield_break {
                        let nop = f.add(Inst::Nop {
                            kind: NopKind::Normal,
                        });
                        f.add(Inst::Leave {
                            target: Some(bodies.new),
                            value: nop,
                        })
                    } else {
                        // Not an error: possibly unreachable code that is removed soon
                        // (mcs iterators have it).
                        invalid_branch(f, "Unexpected return in MoveNext()".into())
                    };
                    f.add_il_range(replacement, f.il_range(inst));
                    f.replace(inst, replacement);
                } else if container == bodies.old {
                    f.set_leave_target(inst, bodies.new);
                }
            }
            _ => {}
        }
        for child in f.inst(inst).children() {
            self.update_branch_targets(f, bodies, child);
        }
    }
}

/// `SplitBlock`: continues in a new block unless the current one is still empty.
fn split_block(f: &mut Function, bodies: &Bodies, new_block: NodeId, old_inst: NodeId) -> NodeId {
    if f.list(new_block).is_empty() {
        return new_block;
    }
    let block = self::new_block(f);
    let start = f.il_range(old_inst).start;
    f.add_il_range(block, Interval::new(start, start));
    f.push(bodies.new, block);
    let goto = branch(f, block);
    f.push(new_block, goto);
    block
}

/// `MakeGoTo`: a branch to the block of state `v`.
fn make_goto(f: &mut Function, bodies: &Bodies, v: i32) -> Result<NodeId> {
    match bodies.block_states.get(i64::from(v)) {
        Some(&target) => {
            let target = if f.node(target).parent == Some(bodies.old) {
                f.list(bodies.new)[f.node(target).child_index as usize]
            } else {
                target
            };
            Ok(branch(f, target))
        }
        None => fail(format!("Could not find block for state {v}")),
    }
}

/// `TranslateFieldsToLocalAccess`: accesses of enumerator fields become variable accesses.
pub fn translate_fields_to_local_access(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
    map: &mut OrderedMap<FieldDef, VarId>,
    is_compiled_with_mono: bool,
) {
    if let Inst::LdFlda { target, field, .. } = r.function.inst(inst)
        && r.function.match_ld_this(*target)
    {
        let field = field.clone();
        let def = def_of(&field);
        let v = match def.and_then(|d| map.get(&d).copied()) {
            Some(v) => v,
            None => {
                let name = &field.field.name;
                let name = if name.starts_with('<')
                    && let Some(pos) = name.find('>')
                    && pos > 1
                {
                    Some(name[1..pos].to_owned())
                } else {
                    None
                };
                let v = r.register_variable_named(ctx, VariableKind::Local, field.field.ty, name);
                let var = r.function.var_mut(v);
                // The field was default-initialized; the variable keeps those semantics.
                var.initial_value_is_initialized = true;
                var.uses_initial_value = true;
                var.state_machine_field = Some(field);
                if let Some(d) = def {
                    map.insert(d, v);
                }
                v
            }
        };
        let f = &mut r.function;
        let replacement = if f.var(v).stack_type == StackType::Ref {
            // The 'this' pointer of a struct.
            f.add(Inst::LdLoc { var: v })
        } else {
            f.add(Inst::LdLoca { var: v })
        };
        f.add_il_range(replacement, f.il_range(inst));
        f.replace(inst, replacement);
    } else if !is_compiled_with_mono && r.function.match_ld_this(inst) {
        let f = &mut r.function;
        let expected = f.result_type(ctx.ts, inst);
        let replacement = f.add(Inst::InvalidExpression {
            message: "stateMachine".into(),
            expected,
            severity: "Error".into(),
        });
        f.add_il_range(replacement, f.il_range(inst));
        f.replace(inst, replacement);
    } else {
        for child in r.function.inst(inst).children() {
            translate_fields_to_local_access(r, ctx, child, map, is_compiled_with_mono);
        }
        let f = &mut r.function;
        let state_machine_local = |f: &Function, target: NodeId| match *f.inst(target) {
            Inst::LdLoca { var } if f.var(var).state_machine_field.is_some() => Some(var),
            _ => None,
        };
        match *f.inst(inst) {
            Inst::LdObj { target, .. } => {
                if let Some(var) = state_machine_local(f, target) {
                    let load = f.add(Inst::LdLoc { var });
                    f.add_il_range(load, f.il_range(inst));
                    f.add_il_range(load, f.il_range(target));
                    f.replace(inst, load);
                }
            }
            Inst::StObj { target, value, .. } => {
                if let Some(var) = state_machine_local(f, target) {
                    let store = f.add(Inst::StLoc {
                        var,
                        value,
                        il_stack_was_empty: false,
                        is_stack_adjustment: false,
                    });
                    f.add_il_range(store, f.il_range(inst));
                    f.add_il_range(store, f.il_range(target));
                    f.replace(inst, store);
                }
            }
            _ => {}
        }
    }
}
