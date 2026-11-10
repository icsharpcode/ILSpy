//! `IL/ControlFlow/AsyncAwaitDecompiler.cs`: turns a method that only starts a compiler-
//! generated state machine back into an `async` method (or async iterator), by inlining the
//! state machine's `MoveNext()` and translating its awaits.
//!
//! `MoveNext()` is read into an arena of its own and analyzed there; its body is then
//! imported into the method's arena, where the managed code continues on the moved
//! instructions.

use std::collections::{HashMap, HashSet};

use crate::il::ast::{
    AwaiterMethod, BranchTarget, ComparisonKind, DynamicKind, Function, Inst, MethodOperand,
    NodeId, NopKind, VarId, VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::semantic_helper::is_pure;
use crate::il::transforms::il_inlining::{inline_all_in_block, inline_one, options};
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::il::transforms::split_variables::remove_dead_variables;
use crate::il::transforms::{
    Context, copy_propagation, dynamic_call_site, early_expression_transforms,
    remove_dead_variable_init, run_early_il_transforms,
};
use crate::il::types::PrimitiveType;
use crate::metadata::Metadata;
use crate::metadata::bytes::Bytes;
use crate::metadata::tables::rows;
use crate::typesystem::compilation::{TypeDefId, TypeKind, type_def_full_name, type_ref_full_name};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::members::MethodDefId;
use crate::typesystem::names::FullTypeName;
use crate::typesystem::resolve::MethodOrigin;
use crate::typesystem::types::{GenericContext, TypeData, TypeId, TypeSystem};
use crate::util::long_set::LongSet;

use super::control_flow_simplification::combine_block_with_next_block;
use super::state_range_analysis::{Mode, OrderedMap, StateRangeAnalysis};
use super::symbolic_execution::{FieldDef, SymbolicEvaluationContext, SymbolicValue};
use super::yield_return_decompiler::{
    Result, create_il_ast, def_of, def_row, fail, get_body_entry_point, invalid_branch,
    is_compiler_generated, is_field, is_known, new_block,
    resolve_ienumerable_ienumerator_field_mapping, set_body, single_block,
    translate_fields_to_local_access, try_finallys,
};

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum MethodType {
    Void,
    Task,
    TaskOfT,
    AsyncEnumerator,
    AsyncEnumerable,
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.async_await {
        return;
    }
    let void = ctx.ts.known_type(KnownTypeCode::Void);
    let mut d = AsyncAwaitDecompiler {
        outer: r.method,
        outer_context: r.generic_context,
        task_type: void,
        underlying_return_type: void,
        method_type: MethodType::Void,
        state_machine_type: None,
        builder_type: None,
        builder_field: None,
        state_field: None,
        initial_state: 0,
        field_to_parameter: OrderedMap::default(),
        cached_field_to_parameter: Vec::new(),
        dispose_mode_field: None,
        move_next: None,
        cached_state_var: None,
        main_try_catch: None,
        set_result_return_block: None,
        final_state: 0,
        final_state_known: false,
        result_var: None,
        set_result_yield_block: None,
        do_finally_bodies: None,
        smallest_awaiter_var_index: i32::MAX,
        move_next_leaves: HashSet::new(),
        await_blocks: HashMap::new(),
        is_visual_basic_state_machine: false,
        catch_handler_offset: -1,
        await_debug_infos: Vec::new(),
    };
    d.run(r, ctx);
}

struct AsyncAwaitDecompiler {
    outer: MethodDefId,
    /// The context the async method was read in (`context.Function.GenericContext`).
    outer_context: crate::typesystem::types::GenericContext,
    /// The return type of the async method (or `IAsyncEnumerable<T>` / `IAsyncEnumerator<T>`).
    task_type: TypeId,
    /// The `T` of `Task<T>`; for async enumerators the yielded type.
    underlying_return_type: TypeId,
    method_type: MethodType,
    state_machine_type: Option<TypeDefId>,
    builder_type: Option<TypeId>,
    builder_field: Option<FieldDef>,
    state_field: Option<FieldDef>,
    initial_state: i32,
    field_to_parameter: OrderedMap<FieldDef, VarId>,
    /// `MoveNext()` variables caching a parameter field, and the parameter.
    cached_field_to_parameter: Vec<(VarId, VarId)>,
    /// `disposeMode` (async enumerators only).
    dispose_mode_field: Option<FieldDef>,
    // Set by `analyze_move_next`, in the `MoveNext()` arena until `inline_body_of_move_next`.
    move_next: Option<ReadFunction>,
    cached_state_var: Option<VarId>,
    main_try_catch: Option<NodeId>,
    /// The block `return` statements jump to (for async enumerators `yield break`).
    set_result_return_block: Option<NodeId>,
    final_state: i32,
    final_state_known: bool,
    result_var: Option<VarId>,
    /// The block `yield return` statements jump to.
    set_result_yield_block: Option<NodeId>,
    do_finally_bodies: Option<VarId>,
    // Set by `analyze_state_machine`.
    smallest_awaiter_var_index: i32,
    move_next_leaves: HashSet<NodeId>,
    /// Per block holding an await: the awaiter variable and the field that keeps it across
    /// the yield point.
    await_blocks: HashMap<NodeId, (VarId, Option<FieldDef>)>,
    is_visual_basic_state_machine: bool,
    catch_handler_offset: i32,
    await_debug_infos: Vec<(i32, i32)>,
}

/// `MatchCall`: an instance `call` or `callvirt` of `name` with arguments.
fn match_call<'f>(f: &'f Function, inst: NodeId, name: &str) -> Option<&'f [NodeId]> {
    match f.inst(inst) {
        Inst::Call(c) | Inst::CallVirt(c)
            if c.method.method.name == name && !c.method.method.is_static =>
        {
            (!c.args.is_empty()).then_some(&c.args[..])
        }
        _ => None,
    }
}

/// `StackSlotValue`: the value of a single-definition stack slot loaded by `inst`.
fn stack_slot_value(f: &Function, inst: NodeId) -> NodeId {
    if let Some(v) = f.match_ldloc(inst)
        && f.var(v).kind == VariableKind::StackSlot
        && f.var(v).is_single_definition()
        && let Some((_, value)) = f.match_stloc(f.var(v).store_instructions[0])
    {
        return value;
    }
    inst
}

/// `UnwrapConvUnknown`.
fn unwrap_conv_unknown(f: &Function, inst: NodeId) -> NodeId {
    match *f.inst(inst) {
        Inst::Conv {
            arg,
            target: PrimitiveType::Unknown,
            ..
        } => arg,
        _ => inst,
    }
}

fn at(f: &Function, block: NodeId, pos: usize) -> Option<NodeId> {
    f.list(block).get(pos).copied()
}

/// The full name an `InterfaceImpl` names, a generic instance by its generic type
/// (`GetFullTypeName` of a TypeDef, TypeRef or TypeSpec).
fn interface_full_name(md: &Metadata<'_>, token: u32) -> Option<FullTypeName> {
    let row = token & 0x00FF_FFFF;
    match token >> 24 {
        0x01 => type_ref_full_name(md, row).ok(),
        0x02 => type_def_full_name(md, row).ok(),
        0x1B => {
            let spec = md.get::<rows::TypeSpec>(row).ok()?;
            let mut b = Bytes::new(md.blob(spec.signature).ok()?);
            // GENERICINST (CLASS | VALUETYPE) TypeDefOrRefOrSpec ...
            if b.u8().ok()? != 0x15 || !matches!(b.u8().ok()?, 0x11 | 0x12) {
                return None;
            }
            let coded = b.compressed_u32().ok()?;
            let generic = match coded & 3 {
                0 => 0x0200_0000,
                1 => 0x0100_0000,
                _ => return None,
            } | (coded >> 2);
            interface_full_name(md, generic)
        }
        _ => None,
    }
}

/// `IsCompilerGeneratorAsyncEnumerator`.
fn is_compiler_generated_async_enumerator(md: &Metadata<'_>, type_def: u32) -> bool {
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
        md.get::<rows::InterfaceImpl>(ii)
            .and_then(|r| r.interface.token())
            .ok()
            .and_then(|t| interface_full_name(md, t))
            .is_some_and(|n| {
                n.nested.is_empty()
                    && n.top.ns == "System.Collections.Generic"
                    && n.top.name == "IAsyncEnumerator"
                    && n.top.tpc == 1
            })
    })
}

/// `IsCalledByEntryPoint`: the compiler-generated `<Main>` entry point calls `method`.
fn is_called_by_entry_point(md: &Metadata<'_>, method: u32) -> bool {
    let Some(pe) = &md.pe else {
        return false;
    };
    let entry = pe.cli.entry_point & 0x00FF_FFFF;
    if entry == 0 {
        return false;
    }
    let Ok(shim) = md.get::<rows::MethodDef>(entry) else {
        return false;
    };
    if !md.string(shim.name).is_ok_and(|n| n == "<Main>") || shim.rva == 0 {
        return false;
    }
    let Ok(body) = md.method_body(shim.rva) else {
        return false;
    };
    let mut code = crate::il::opcodes::Code::new(body.code);
    while code.remaining() > 0 {
        let Ok(op) = code.opcode() else {
            return false;
        };
        if op != 0x28 {
            code.skip_operand(op);
            continue;
        }
        match code.i32() {
            Ok(token) if token as u32 == 0x0600_0000 | method => return true,
            Ok(_) => {}
            Err(_) => return false,
        }
    }
    false
}

impl AsyncAwaitDecompiler {
    fn is_async_enumerator(&self) -> bool {
        matches!(
            self.method_type,
            MethodType::AsyncEnumerable | MethodType::AsyncEnumerator
        )
    }

    fn md<'c, 'a>(&self, ts: &TypeSystem<'c, 'a>) -> &'c Metadata<'a> {
        &ts.comp.module(self.outer.module).md
    }

    fn run(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        if !self.match_task_creation_pattern(r, ctx)
            && !self.match_async_enumerator_creation_pattern(r, ctx)
        {
            if r.async_return_type.is_some() {
                super::runtime_async::run(r, ctx);
            }
            return;
        }
        let analyzed = self
            .analyze_move_next(ctx)
            .and_then(|()| self.validate_catch_block(ctx))
            .and_then(|()| self.analyze_dispose_async(ctx));
        if analyzed.is_err() {
            return;
        }

        self.inline_body_of_move_next(r, ctx);
        self.clean_up_body_of_move_next(r, ctx);

        self.analyze_state_machine(r, ctx);
        self.detect_await_pattern(r, ctx);
        self.clean_do_finally_bodies(r);

        let body = r.function.body.expect("body");
        let mut map = std::mem::take(&mut self.field_to_parameter);
        translate_fields_to_local_access(r, ctx, body, &mut map, false);
        self.field_to_parameter = map;
        self.translate_cached_fields_to_locals(r);

        self.finalize_inline_move_next(r);
        let expected = if self.is_async_enumerator() {
            ctx.ts.known_type(KnownTypeCode::Void)
        } else {
            self.underlying_return_type
        };
        if let Inst::BlockContainer { expected: e, .. } = r.function.inst_mut(body) {
            *e = Some(expected);
        }

        // TranslateFieldsToLocalAccess may have opened up new inlining opportunities.
        run_early_il_transforms(r, ctx, false);

        if !self.is_visual_basic_state_machine {
            super::await_in_catch_transform::run(r, ctx);
            super::await_in_finally_transform::run(r, ctx);
        }

        self.await_debug_infos
            .sort_by_key(|&(yield_offset, _)| yield_offset);
        // The compiler records the catch handler for async void methods and for the kickoff
        // method of an async Main entry point.
        let record_catch_handler = self.method_type == MethodType::Void
            || is_called_by_entry_point(self.md(ctx.ts), self.outer.row);
        r.async_debug_info = Some((
            if record_catch_handler {
                self.catch_handler_offset
            } else {
                -1
            },
            std::mem::take(&mut self.await_debug_infos),
        ));
    }

    /// `MatchStFld` against the state machine variable: the field's definition and the value.
    fn match_stfld_sm(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
        sm: VarId,
    ) -> Option<(Option<FieldDef>, NodeId)> {
        let (target, field, value) = f.match_stfld(inst)?;
        let field = def_of(field);
        (f.match_ld_loc_ref(ts, target) == Some(sm)).then_some((field, value))
    }

    fn match_task_creation_pattern(
        &mut self,
        r: &ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) -> bool {
        let f = &r.function;
        let container = f.body.expect("body");
        let [entry] = *f.list(container) else {
            return false;
        };
        let body = f.list(entry).to_vec();
        if body.len() < 4 {
            return false;
        }
        // The start call (second to last) tells the most.
        let mut pos = body.len() - 2;
        let (Inst::Call(start) | Inst::CallVirt(start) | Inst::NewObj(start)) = f.inst(body[pos])
        else {
            return false;
        };
        if start.method.method.name != "Start" {
            return false;
        }
        let ts = &mut *ctx.ts;
        self.task_type = ts.method_signature(self.outer).0;
        let Some(builder_type) = start.method.method.declaring else {
            return false;
        };
        self.builder_type = Some(builder_type);
        let builder_type_name = match ts.definition(builder_type) {
            Some(def) => match type_def_full_name(&ts.comp.module(def.module).md, def.row) {
                Ok(n) => n,
                Err(_) => return false,
            },
            None => match ts.data(builder_type) {
                TypeData::Unknown { name, .. } => (**name).clone(),
                _ => return false,
            },
        };
        if is_known(ts, self.task_type, KnownTypeCode::Void) {
            self.method_type = MethodType::Void;
            self.underlying_return_type = self.task_type;
            if builder_type_name
                != FullTypeName::top_level(crate::typesystem::names::TopLevelTypeName::new(
                    "System.Runtime.CompilerServices",
                    "AsyncVoidMethodBuilder",
                    0,
                ))
            {
                return false;
            }
        } else if let Some(from_task) = ts.non_generic_task_builder(self.task_type) {
            self.method_type = MethodType::Task;
            self.underlying_return_type = ts.known_type(KnownTypeCode::Void);
            if from_task != builder_type_name {
                return false;
            }
        } else if let Some(from_task) = ts.generic_task_builder(self.task_type) {
            self.method_type = MethodType::TaskOfT;
            self.underlying_return_type = if is_known(ts, self.task_type, KnownTypeCode::TaskOfT) {
                ts.unpack_any_task(self.task_type)
            } else {
                match ts.type_arguments(builder_type).first() {
                    Some(&t) => t,
                    None => return false,
                }
            };
            if from_task != builder_type_name {
                return false;
            }
        } else {
            return false;
        }
        if start.args.len() != 2 {
            return false;
        }
        let mut load_builder = start.args[0];
        let Inst::LdLoca {
            var: state_machine_var,
        } = *f.inst(start.args[1])
        else {
            return false;
        };
        self.state_machine_type = ts.definition(f.var(state_machine_var).ty);
        let Some(state_machine_type) = self.state_machine_type else {
            return false;
        };
        pos -= 1;

        if let Some(builder_var) = f.match_ld_loc_ref(ts, load_builder) {
            // stloc builder(ldfld StateMachine::<>t__builder(ldloc stateMachine))
            match f.match_stloc(body[pos]) {
                Some((v, value)) if v == builder_var => load_builder = value,
                _ => return false,
            }
            pos -= 1;
        }
        let (load_state_machine, builder_field) =
            if let Some((target, field)) = f.match_ldfld(ts, load_builder) {
                (target, def_of(field))
            } else if let Some((target, field)) = f.match_ldflda(load_builder) {
                // Roslyn 3.6 calls Start without copying the builder.
                (target, def_of(field))
            } else {
                return false;
            };
        self.builder_field = builder_field;
        if !(f.match_ld_loc_ref(ts, load_state_machine) == Some(state_machine_var)
            || f.match_ldloc(load_state_machine) == Some(state_machine_var))
        {
            return false;
        }

        let last = body[body.len() - 1];
        if self.method_type == MethodType::Void {
            if !f
                .match_leave(last)
                .is_some_and(|(c, v)| c == container && f.match_nop(v))
            {
                return false;
            }
        } else {
            // ret(call(AsyncTaskMethodBuilder::get_Task, ldflda(StateMachine::<>t__builder, ldloca(stateMachine))))
            let Some(return_value) = f.match_return(last) else {
                return false;
            };
            let Some(args) = match_call(f, return_value, "get_Task") else {
                return false;
            };
            if args.len() != 1 {
                return false;
            }
            let (target, field) = if ts.is_reference_type(builder_type) == Some(true) {
                match f.match_ldfld(ts, args[0]) {
                    Some((t, field)) => (t, def_of(field)),
                    None => return false,
                }
            } else {
                match f.match_ldflda(args[0]) {
                    Some((t, field)) => (t, def_of(field)),
                    None => return false,
                }
            };
            if field != self.builder_field {
                return false;
            }
            if !(f.match_ldloc(target) == Some(state_machine_var)
                || matches!(*f.inst(target), Inst::LdLoca { var } if var == state_machine_var))
            {
                return false;
            }
        }

        if self.is_potential_visual_basic_state_machine_initialization(
            f,
            ts,
            body[0],
            state_machine_var,
        ) {
            // The Visual Basic compiler assigns the fields in another order.
            if self.match_visual_basic_state_machine_field_assignments(
                f,
                ts,
                &body,
                state_machine_var,
            ) {
                self.is_visual_basic_state_machine = true;
                return true;
            }
            // A partial match may have filled the map.
            self.field_to_parameter = OrderedMap::default();
        }

        // The last field assignment is the state: stfld <>1__state(ldloca stateMachine, ldc.i4 -1)
        let Some((state_field, initial)) = self.match_stfld_sm(f, ts, body[pos], state_machine_var)
        else {
            return false;
        };
        self.state_field = state_field;
        let Some(initial) = f.match_ldc_i4(initial) else {
            return false;
        };
        self.initial_state = initial;
        if initial != -1 {
            return false;
        }

        let stop = pos;
        pos = 0;
        if ts
            .comp
            .type_def(state_machine_type)
            .is_ok_and(|i| i.kind == TypeKind::Class)
        {
            // stloc stateMachine(newobj StateMachine.ctor())
            let Some((v, init)) = f.match_stloc(body[pos]) else {
                return false;
            };
            if v != state_machine_var {
                return false;
            }
            let Inst::NewObj(newobj) = f.inst(init) else {
                return false;
            };
            if !newobj.args.is_empty()
                || newobj
                    .method
                    .method
                    .declaring
                    .and_then(|t| ts.definition(t))
                    != Some(state_machine_type)
            {
                return false;
            }
            pos += 1;
        }
        let mut builder_field_is_initialized = false;
        while pos < stop {
            // stfld StateMachine.field(ldloca stateMachine, ldvar(param))
            let Some((field, init)) = self.match_stfld_sm(f, ts, body[pos], state_machine_var)
            else {
                return false;
            };
            let Some(field) = field else {
                return false;
            };
            if Some(field) == self.builder_field {
                // stfld StateMachine.builder(ldloca stateMachine, call Create())
                if !matches!(f.inst(init), Inst::Call(c) if c.method.method.name == "Create" && c.args.is_empty())
                {
                    return false;
                }
                builder_field_is_initialized = true;
            } else if let Some(v) = f.match_ldloc(init)
                && f.var(v).kind == VariableKind::Parameter
            {
                self.field_to_parameter.insert(field, v);
            } else if let Inst::LdObj { target, .. } = *f.inst(init)
                && f.match_ld_this(target)
            {
                // stfld <>4__this(ldloc stateMachine, ldobj AsyncInStruct(ldloc this))
                self.field_to_parameter
                    .insert(field, f.match_ldloc(target).expect("ldloc this"));
            } else {
                return false;
            }
            pos += 1;
        }
        builder_field_is_initialized
    }

    fn is_potential_visual_basic_state_machine_initialization(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
        sm: VarId,
    ) -> bool {
        // stobj VB$StateMachine(ldloca stateMachine, default.value VB$StateMachine)
        if let Inst::StObj {
            target, value, ty, ..
        } = *f.inst(inst)
            && matches!(*f.inst(target), Inst::LdLoca { var } if var == sm)
            && matches!(*f.inst(value), Inst::DefaultValue { ty: t, .. } if t.ty == ty.ty)
        {
            return true;
        }
        // stloc stateMachine(newobj VB$StateMachine..ctor())
        if let Some((v, init)) = f.match_stloc(inst)
            && v == sm
            && let Inst::NewObj(newobj) = f.inst(init)
            && newobj.args.is_empty()
            && newobj
                .method
                .method
                .declaring
                .and_then(|t| ts.definition(t))
                == self.state_machine_type
        {
            return true;
        }
        false
    }

    fn match_visual_basic_state_machine_field_assignments(
        &mut self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        body: &[NodeId],
        sm: VarId,
    ) -> bool {
        let n = body.len();
        // stfld $State(ldloca stateField, ldc.i4 -1)
        let Some((state_field, initial)) = self.match_stfld_sm(f, ts, body[n - 4], sm) else {
            return false;
        };
        self.state_field = state_field;
        let Some(initial) = f.match_ldc_i4(initial) else {
            return false;
        };
        self.initial_state = initial;
        if initial != -1 {
            return false;
        }
        // stfld $Builder(ldloca stateMachine, call Create())
        let Some((builder_field2, init)) = self.match_stfld_sm(f, ts, body[n - 3], sm) else {
            return false;
        };
        if !is_field(self.builder_field, builder_field2) {
            return false;
        }
        if !matches!(f.inst(init), Inst::Call(c) if c.method.method.name == "Create" && c.args.is_empty())
        {
            return false;
        }
        for &inst in &body[1..n - 4] {
            let Some((field, mut init)) = self.match_stfld_sm(f, ts, inst, sm) else {
                return false;
            };
            // The legacy Visual Basic compiler may call RuntimeHelpers.GetObjectValue here.
            if let Inst::Call(c) = f.inst(init)
                && c.args.len() == 1
                && c.method.method.is_static
                && c.method.method.name == "GetObjectValue"
            {
                init = c.args[0];
            }
            let Some(field) = field else {
                return false;
            };
            if let Some(v) = f.match_ldloc(init)
                && f.var(v).kind == VariableKind::Parameter
            {
                self.field_to_parameter.insert(field, v);
            } else if let Inst::LdObj { target, .. } = *f.inst(init)
                && f.match_ld_this(target)
            {
                self.field_to_parameter
                    .insert(field, f.match_ldloc(target).expect("ldloc this"));
            } else {
                return false;
            }
        }
        true
    }

    fn match_async_enumerator_creation_pattern(
        &mut self,
        r: &ReadFunction,
        ctx: &mut Context<'_, '_, '_>,
    ) -> bool {
        if !ctx.settings.async_enumerator {
            return false;
        }
        let ts = &mut *ctx.ts;
        self.task_type = ts.method_signature(self.outer).0;
        if is_known(ts, self.task_type, KnownTypeCode::IAsyncEnumeratorOfT) {
            self.method_type = MethodType::AsyncEnumerator;
        } else if is_known(ts, self.task_type, KnownTypeCode::IAsyncEnumerableOfT) {
            self.method_type = MethodType::AsyncEnumerable;
        } else {
            return false;
        }
        let args = ts.type_arguments(self.task_type);
        let [underlying] = args[..] else {
            return false;
        };
        self.underlying_return_type = underlying;
        let f = &r.function;
        let container = f.body.expect("body");
        let [entry] = *f.list(container) else {
            return false;
        };
        let body = f.list(entry).to_vec();
        let new_obj = if body.len() == 1 {
            // No parameters passed to the enumerator (not even 'this'): ret(newobj(...))
            let Some(new_obj) = f.match_return(body[0]) else {
                return false;
            };
            if !self.match_enumerator_creation_new_obj(f, ts, new_obj) {
                return false;
            }
            new_obj
        } else {
            // stloc v(newobj<CountUpSlowly> d__0..ctor(ldc.i4 - 2)); stfld ...(ldloc v, ...);
            // leave(ldloc v)
            let Some((v, new_obj)) = f.match_stloc(body[0]) else {
                return false;
            };
            if !self.match_enumerator_creation_new_obj(f, ts, new_obj) {
                return false;
            }
            let mut pos = 1;
            while let Some(&inst) = body.get(pos)
                && let Some((field, value)) = self.match_stfld_sm(f, ts, inst, v)
            {
                let Some(field) = field else {
                    return false;
                };
                if let Some(p) = f.match_ldloc(value)
                    && f.var(p).kind == VariableKind::Parameter
                {
                    self.field_to_parameter.insert(field, p);
                } else if let Inst::LdObj { target, .. } = *f.inst(value)
                    && f.match_ld_this(target)
                {
                    self.field_to_parameter
                        .insert(field, f.match_ldloc(target).expect("ldloc this"));
                } else {
                    return false;
                }
                pos += 1;
            }
            let Some(returned) = body.get(pos).and_then(|&i| f.match_return(i)) else {
                return false;
            };
            if f.match_ldloc(returned) != Some(v) {
                return false;
            }
            new_obj
        };
        // The async logic expects the 'in progress' state as the initial one.
        self.initial_state = -1;
        let Inst::NewObj(call) = f.inst(new_obj) else {
            unreachable!("matched a newobj");
        };
        let ctor = call.method.clone();
        let analyzed = self.analyze_enumerator_ctor(ctx, &ctor).and_then(|()| {
            if self.method_type == MethodType::AsyncEnumerable {
                self.resolve_ienumerable_ienumerator_field_mapping(ctx)
            } else {
                Ok(())
            }
        });
        analyzed.is_ok()
    }

    /// `MatchEnumeratorCreationNewObj`: `newobj(CurrentType/...::.ctor, ldc.i4(state))`.
    fn match_enumerator_creation_new_obj(
        &mut self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
    ) -> bool {
        let Inst::NewObj(new_obj) = f.inst(inst) else {
            return false;
        };
        if new_obj.args.len() != 1 || f.match_ldc_i4(new_obj.args[0]).is_none() {
            return false;
        }
        self.state_machine_type = new_obj
            .method
            .method
            .declaring
            .and_then(|t| ts.definition(t));
        let Some(sm) = self.state_machine_type else {
            return false;
        };
        let comp = ts.comp;
        let sm_declaring = comp.type_def(sm).ok().map(|i| i.declaring);
        let outer_declaring = comp
            .module(self.outer.module)
            .md
            .method_declaring_type(self.outer.row)
            .ok();
        if sm.module != self.outer.module || sm_declaring != outer_declaring {
            return false;
        }
        is_compiler_generated_async_enumerator(&comp.module(self.outer.module).md, sm.row)
    }

    /// `AnalyzeEnumeratorCtor`: the state and builder fields from the constructor.
    fn analyze_enumerator_ctor(
        &mut self,
        ctx: &mut Context<'_, '_, '_>,
        ctor: &MethodOperand,
    ) -> Result<()> {
        self.builder_field = None;
        self.state_field = None;
        let read = create_il_ast(ctx, self.outer, self.outer_context, def_row(ctor))?;
        let f = &read.function;
        let Some(body) = single_block(f, f.body.expect("body")) else {
            return fail("Missing enumeratorCtor.Body");
        };
        for &inst in f.list(body) {
            if let Some((target, field, value)) = f.match_stfld(inst)
                && f.match_ld_this(target)
            {
                if let Some(arg) = f.match_ldloc(value)
                    && f.var(arg).kind == VariableKind::Parameter
                    && f.var(arg).index == Some(0)
                {
                    self.state_field = def_of(field);
                }
                if matches!(f.inst(value), Inst::Call(c) if c.method.method.name == "Create") {
                    self.builder_field = def_of(field);
                }
            }
        }
        let (Some(_), Some((module, row))) = (self.state_field, self.builder_field) else {
            return fail("");
        };
        self.builder_type = Some(ctx.ts.field_type(module, row).0);
        Ok(())
    }

    fn resolve_ienumerable_ienumerator_field_mapping(
        &mut self,
        ctx: &mut Context<'_, '_, '_>,
    ) -> Result<()> {
        let sm = self.state_machine_type.expect("state machine type");
        let md = self.md(ctx.ts);
        let row = md
            .type_methods(sm.row)
            .unwrap_or_default()
            .into_iter()
            .find(|&m| {
                md.get::<rows::MethodDef>(m)
                    .and_then(|d| md.string(d.name))
                    .is_ok_and(|n| n.ends_with(".GetAsyncEnumerator"))
            });
        let Some(row) = row else {
            return fail("");
        };
        resolve_ienumerable_ienumerator_field_mapping(
            ctx,
            self.outer,
            self.outer_context,
            row,
            &mut self.field_to_parameter,
        )
    }

    /// `AnalyzeMoveNext`: everything of `MoveNext()` outside the big try-catch.
    fn analyze_move_next(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        let Some(sm) = self
            .state_machine_type
            .filter(|sm| sm.module == self.outer.module)
        else {
            return fail("");
        };
        let md = self.md(ctx.ts);
        let row = md
            .type_methods(sm.row)
            .unwrap_or_default()
            .into_iter()
            .find(|&m| {
                md.get::<rows::MethodDef>(m)
                    .and_then(|d| md.string(d.name))
                    .is_ok_and(|n| n == "MoveNext")
            });
        let Some(row) = row else {
            return fail("");
        };
        let mn = create_il_ast(ctx, self.outer, self.outer_context, row)?;
        let f = &mn.function;
        let container = f.body.expect("body");
        let entry = f.list(container)[0];
        if f.node(entry).incoming_edge_count != 1 {
            return fail("");
        }
        let mut analyzed = vec![false; f.list(container).len()];
        self.cached_state_var = None;
        let mut pos = 0;
        // Visual Basic initializes doFinallyBodies at the start: stloc doFinallyBodies(ldc.i4 1)
        if self.is_visual_basic_state_machine
            && let Some((v, one)) = at(f, entry, pos).and_then(|i| f.match_stloc(i))
            && f.match_ldc_i4(one) == Some(1)
        {
            self.do_finally_bodies = Some(v);
            pos += 1;
        }
        while let Some(store) = at(f, entry, pos)
            && let Some((v, value)) = f.match_stloc(store)
        {
            // stloc V_1(ldfld <>4__this(ldloc this))
            let Some((target, field)) = f.match_ldfld(ctx.ts, value) else {
                return fail("");
            };
            if !f.match_ld_this(target) {
                return fail("");
            }
            let field = def_of(field);
            if is_field(field, self.state_field) && self.cached_state_var.is_none() {
                // stloc(cachedState, ldfld(valuetype StateMachineStruct::<>1__state, ldloc(this)))
                self.cached_state_var = Some(v);
            } else if let Some(&param) = field.and_then(|d| self.field_to_parameter.get(&d)) {
                if !f.var(v).is_single_definition() {
                    return fail("");
                }
                match self
                    .cached_field_to_parameter
                    .iter_mut()
                    .find(|(c, _)| *c == v)
                {
                    Some(entry) => entry.1 = param,
                    None => self.cached_field_to_parameter.push((v, param)),
                }
            } else {
                return fail("");
            }
            pos += 1;
        }
        let Some(main_try_catch) =
            at(f, entry, pos).filter(|&i| matches!(f.inst(i), Inst::TryCatch { .. }))
        else {
            return fail("");
        };
        self.main_try_catch = Some(main_try_catch);
        // stloc doFinallyBodies(ldc.i4 1)
        let Inst::TryCatch { try_block, .. } = *f.inst(main_try_catch) else {
            unreachable!("try-catch");
        };
        if let Some(&first) = f.list(f.list(try_block)[0]).first()
            && let Some((v, one)) = f.match_stloc(first)
            && f.var(v).kind == VariableKind::Local
            && is_known(ctx.ts, f.var(v).ty, KnownTypeCode::Boolean)
            && f.match_ldc_i4(one) == Some(1)
        {
            self.do_finally_bodies = Some(v);
        }

        analyzed[0] = true;
        pos = 1;
        if self.match_yield_block(f, ctx.ts, container, pos) {
            self.set_result_yield_block = Some(f.list(container)[pos]);
            analyzed[pos] = true;
            pos += 1;
        } else {
            self.set_result_yield_block = None;
        }
        self.set_result_return_block =
            self.check_set_result_return_block(f, ctx.ts, container, pos, &mut analyzed)?;
        if !analyzed.iter().all(|&b| b) {
            return fail("too many blocks");
        }
        self.move_next = Some(mn);
        Ok(())
    }

    fn match_yield_block(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        container: NodeId,
        pos: usize,
    ) -> bool {
        if !self.is_async_enumerator() {
            return false;
        }
        let Some(block) = at(f, container, pos) else {
            return false;
        };
        // call SetResult(ldflda <>v__promiseOfValueOrEnd(ldloc this), ldc.i4 1); leave IL_0000(nop)
        let [call, leave] = *f.list(block) else {
            return false;
        };
        let Some(args) = match_call(f, call, "SetResult") else {
            return false;
        };
        args.len() == 2
            && self.is_builder_or_promise_field_on_this(f, ts, args[0])
            && f.match_ldc_i4(args[1]) == Some(1)
            && f.match_leave(leave)
                .is_some_and(|(c, v)| c == container && f.match_nop(v))
    }

    fn check_set_result_return_block(
        &mut self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        container: NodeId,
        index: usize,
        analyzed: &mut [bool],
    ) -> Result<Option<NodeId>> {
        if index >= f.list(container).len() {
            // Absent when the function never returns normally; the final state comes from
            // the catch block.
            self.result_var = None;
            self.final_state_known = false;
            return Ok(None);
        }
        let mut block = f.list(container)[index];
        let mut pos = 0;
        // [vb-only] stloc S_10(ldloc this)
        if let Some((v, value)) = at(f, block, pos).and_then(|i| f.match_stloc(i))
            && f.match_ld_this(value)
            && f.var(v).kind == VariableKind::StackSlot
        {
            pos += 1;
        }
        // [vb-only] stloc S_11(ldc.i4 -2)
        let mut final_state_slot = None;
        let mut final_state_slot_value = None;
        if let Some((v, value)) = at(f, block, pos).and_then(|i| f.match_stloc(i))
            && let Some(c) = f.match_ldc_i4(value)
            && f.var(v).kind == VariableKind::StackSlot
        {
            final_state_slot = Some(v);
            final_state_slot_value = Some(c);
            pos += 1;
        }
        // [vb-only] stloc cachedStateVar(ldloc S_11)
        if let Some((v, value)) = at(f, block, pos).and_then(|i| f.match_stloc(i))
            && f.var(v).kind == VariableKind::Local
            && f.var(v).index == self.cached_state_var.and_then(|c| f.var(c).index)
            && final_state_slot.is_some()
            && f.match_ldloc(value) == final_state_slot
        {
            pos += 1;
        }
        // stfld <>1__state(ldloc this, ldc.i4 -2)
        let Some(final_state) = at(f, block, pos).and_then(|i| self.match_state_assignment(f, i))
        else {
            return fail("");
        };
        self.final_state = final_state;
        if final_state_slot_value.is_some_and(|v| v != final_state) {
            return fail("");
        }
        self.final_state_known = true;
        pos += 1;
        // [optional] stfld <>u__N(ldloc this, ldnull), before or after the disposal of the
        // combined tokens (async iterators with an [EnumeratorCancellation] token).
        match_hoisted_local_cleanup(f, block, &mut pos)?;
        if pos + 2 == f.list(block).len()
            && let Some((condition, true_inst, false_inst)) = f.match_if_at_end_of_block(block)
        {
            match self.match_dispose_combined_tokens(
                f, ts, container, condition, true_inst, false_inst, analyzed,
            ) {
                Some(set_result_and_exit) => {
                    analyzed[f.node(block).child_index as usize] = true;
                    block = set_result_and_exit;
                    pos = 0;
                }
                None => return fail(""),
            }
        }
        match_hoisted_local_cleanup(f, block, &mut pos)?;
        self.check_set_result_and_exit(f, ts, container, block, &mut pos)?;
        analyzed[f.node(block).child_index as usize] = true;
        Ok(Some(f.list(container)[index]))
    }

    #[allow(clippy::too_many_arguments)]
    fn match_dispose_combined_tokens(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        container: NodeId,
        condition: NodeId,
        true_inst: NodeId,
        false_inst: NodeId,
        analyzed: &mut [bool],
    ) -> Option<NodeId> {
        //   if (comp.o(ldfld <>x__combinedTokens(ldloc this) == ldnull)) br setResultAndExit
        //   br disposeCombinedTokens
        // disposeCombinedTokens:
        //   callvirt Dispose(ldfld <>x__combinedTokens(ldloc this))
        //   stfld <>x__combinedTokens(ldloc this, ldnull)
        //   br setResultAndExit
        let tested = f.match_comp_null(condition, ComparisonKind::Equality)?;
        let (target, cts_field) = f.match_ldfld(ts, tested)?;
        if !f.match_ld_this(target) {
            return None;
        }
        let cts_type = cts_field.field.ty;
        let cts = def_of(cts_field);
        let TypeData::Def(cts_def) = *ts.data(cts_type) else {
            return None;
        };
        let name = type_def_full_name(&ts.comp.module(cts_def.module).md, cts_def.row).ok()?;
        if !(name.nested.is_empty()
            && name.top.name == "CancellationTokenSource"
            && name.top.ns == "System.Threading")
        {
            return None;
        }
        let set_result_and_exit = f.match_branch(true_inst)?;
        let disposed = f.match_branch(false_inst)?;
        if f.node(set_result_and_exit).parent != Some(container)
            || f.node(disposed).parent != Some(container)
        {
            return None;
        }
        let list = f.list(disposed);
        let Inst::CallVirt(dispose) = f.inst(*list.first()?) else {
            return None;
        };
        if dispose.method.method.name != "Dispose" || dispose.args.len() != 1 {
            return None;
        }
        let (target2, field2) = f.match_ldfld(ts, dispose.args[0])?;
        if !(f.match_ld_this(target2) && def_of(field2) == cts) {
            return None;
        }
        let (target3, field3, stored) = f.match_stfld(*list.get(1)?)?;
        if !(f.match_ld_this(target3) && def_of(field3) == cts)
            || !matches!(f.inst(stored), Inst::LdNull)
        {
            return None;
        }
        if f.match_branch(*list.get(2)?) != Some(set_result_and_exit) {
            return None;
        }
        analyzed[f.node(disposed).child_index as usize] = true;
        Some(set_result_and_exit)
    }

    fn check_set_result_and_exit(
        &mut self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        container: NodeId,
        block: NodeId,
        pos: &mut usize,
    ) -> Result<()> {
        // [optional] call Complete(ldflda <>t__builder(ldloc this)) (Roslyn >=3.9)
        // call SetResult(ldflda <>t__builder(ldloc this), ldloc result)
        // [optional] call Complete(ldflda <>t__builder(ldloc this))
        // leave IL_0000
        self.match_complete_call(f, ts, block, pos)?;
        let Some(args) = at(f, block, *pos).and_then(|i| match_call(f, i, "SetResult")) else {
            return fail("");
        };
        if !self.is_builder_or_promise_field_on_this(f, ts, args[0]) {
            return fail("");
        }
        match self.method_type {
            MethodType::TaskOfT => {
                if args.len() != 2 {
                    return fail("");
                }
                let Some(v) = f.match_ldloc(args[1]) else {
                    return fail("");
                };
                self.result_var = Some(v);
            }
            MethodType::Task | MethodType::Void => {
                self.result_var = None;
                if args.len() != 1 {
                    return fail("");
                }
            }
            MethodType::AsyncEnumerable | MethodType::AsyncEnumerator => {
                self.result_var = None;
                if args.len() != 2 || f.match_ldc_i4(args[1]) != Some(0) {
                    return fail("");
                }
            }
        }
        *pos += 1;
        self.match_complete_call(f, ts, block, pos)?;
        if !at(f, block, *pos).is_some_and(|i| {
            f.match_leave(i)
                .is_some_and(|(c, v)| c == container && f.match_nop(v))
        }) {
            return fail("");
        }
        Ok(())
    }

    fn validate_catch_block(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        // catch E_143 : System.Exception if (ldc.i4 1) BlockContainer {
        //   stloc exception(ldloc E_143)
        //   stfld <>1__state(ldloc this, ldc.i4 -2)
        //   [optional] stfld <>u__N(ldloc this, ldnull)
        //   call SetException(ldfld <>t__builder(ldloc this), ldloc exception)
        //   [optional] call Complete(ldfld <>t__builder(ldloc this))
        //   leave IL_0000
        // }
        let mn = self.move_next.take().expect("MoveNext");
        let result = self.validate_catch_block_in(&mn.function, ctx.ts);
        self.move_next = Some(mn);
        result
    }

    fn validate_catch_block_in(&mut self, f: &Function, ts: &mut TypeSystem<'_, '_>) -> Result<()> {
        let Inst::TryCatch { handlers, .. } = f.inst(self.main_try_catch.expect("try-catch"))
        else {
            unreachable!("try-catch");
        };
        let [handler] = handlers[..] else {
            return fail("");
        };
        let Inst::TryCatchHandler {
            filter,
            body,
            variable,
            ..
        } = *f.inst(handler)
        else {
            unreachable!("handler");
        };
        if !is_known(ts, f.var(variable).ty, KnownTypeCode::Exception) {
            return fail("");
        }
        if f.match_ldc_i4(filter) != Some(1) {
            return fail("");
        }
        if !matches!(f.inst(body), Inst::BlockContainer { .. }) {
            return fail("");
        }
        let mut analyzed = vec![false; f.list(body).len()];
        let mut catch_block = f.list(body)[0];
        self.catch_handler_offset = f.il_range(catch_block).start;
        let mut pos = 0;
        // [vb-only] call SetProjectError(ldloc E_143)
        if self.is_visual_basic_state_machine
            && let Some(Inst::Call(c)) = at(f, catch_block, pos).map(|i| f.inst(i))
            && c.method.method.name == "SetProjectError"
            && c.args.len() == 1
            && f.match_ldloc(c.args[0]) == Some(variable)
        {
            pos += 1;
        }
        // stloc exception(ldloc E_143)
        let Some((exception, value)) = at(f, catch_block, pos).and_then(|i| f.match_stloc(i))
        else {
            return fail("");
        };
        pos += 1;
        if f.match_ldloc(value) != Some(variable) {
            return fail("");
        }
        // stfld <>1__state(ldloc this, ldc.i4 -2)
        let Some(new_state) =
            at(f, catch_block, pos).and_then(|i| self.match_state_assignment(f, i))
        else {
            return fail("");
        };
        pos += 1;
        if self.final_state_known {
            if new_state != self.final_state {
                return fail("");
            }
        } else {
            self.final_state = new_state;
            self.final_state_known = true;
        }
        match_hoisted_local_cleanup(f, catch_block, &mut pos)?;
        if pos + 2 == f.list(catch_block).len()
            && let Some((condition, true_inst, false_inst)) =
                f.match_if_at_end_of_block(catch_block)
        {
            match self.match_dispose_combined_tokens(
                f,
                ts,
                body,
                condition,
                true_inst,
                false_inst,
                &mut analyzed,
            ) {
                Some(set_result_and_exit) => {
                    analyzed[f.node(catch_block).child_index as usize] = true;
                    catch_block = set_result_and_exit;
                    pos = 0;
                }
                None => return fail(""),
            }
        }
        match_hoisted_local_cleanup(f, catch_block, &mut pos)?;
        self.match_complete_call(f, ts, catch_block, &mut pos)?;
        // call SetException(ldfld <>t__builder(ldloc this), ldloc exception)
        let Some(args) = at(f, catch_block, pos).and_then(|i| match_call(f, i, "SetException"))
        else {
            return fail("");
        };
        if args.len() != 2
            || !self.is_builder_or_promise_field_on_this(f, ts, args[0])
            || f.match_ldloc(args[1]) != Some(exception)
        {
            return fail("");
        }
        pos += 1;
        self.match_complete_call(f, ts, catch_block, &mut pos)?;
        // [vb-only] call ClearProjectError()
        if self.is_visual_basic_state_machine
            && let Some(Inst::Call(c)) = at(f, catch_block, pos).map(|i| f.inst(i))
            && c.method.method.name == "ClearProjectError"
            && c.args.is_empty()
        {
            pos += 1;
        }
        // leave IL_0000
        let root = f.body.expect("body");
        if !at(f, catch_block, pos).is_some_and(|i| {
            f.match_leave(i)
                .is_some_and(|(c, v)| c == root && f.match_nop(v))
        }) {
            return fail("");
        }
        analyzed[f.node(catch_block).child_index as usize] = true;
        if !analyzed.iter().all(|&b| b) {
            return fail("");
        }
        Ok(())
    }

    fn match_complete_call(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
        pos: &mut usize,
    ) -> Result<()> {
        if let Some(args) = at(f, block, *pos).and_then(|i| match_call(f, i, "Complete")) {
            if !(args.len() == 1 && self.is_builder_field_on_this(f, ts, args[0])) {
                return fail("");
            }
            *pos += 1;
        }
        Ok(())
    }

    fn is_builder_field_on_this(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
    ) -> bool {
        let builder_is_reference = self
            .builder_type
            .is_some_and(|t| ts.is_reference_type(t) == Some(true));
        let found = if builder_is_reference {
            // ldfld(StateMachine::<>t__builder, ldloc(this))
            f.match_ldfld(ts, inst).map(|(t, field)| (t, def_of(field)))
        } else {
            // ldflda(StateMachine::<>t__builder, ldloc(this))
            f.match_ldflda(inst).map(|(t, field)| (t, def_of(field)))
        };
        found.is_some_and(|(target, field)| {
            f.match_ld_this(target) && field == self.builder_field && field.is_some()
        })
    }

    fn is_builder_or_promise_field_on_this(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
    ) -> bool {
        if self.is_async_enumerator() {
            // The promise fields of other methods are not checked.
            return true;
        }
        self.is_builder_field_on_this(f, ts, inst)
    }

    /// `MatchStateAssignment`: `stfld(StateMachine::<>1__state, ldloc(this), ldc.i4(stateId))`.
    fn match_state_assignment(&self, f: &Function, inst: NodeId) -> Option<i32> {
        let (target, field, value) = f.match_stfld(inst)?;
        if f.match_ld_this(stack_slot_value(f, target)) && is_field(def_of(field), self.state_field)
        {
            f.match_ldc_i4(stack_slot_value(f, value))
        } else {
            None
        }
    }

    /// `AnalyzeDisposeAsync`: the `disposeMode` field `DisposeAsync()` sets.
    fn analyze_dispose_async(&mut self, ctx: &mut Context<'_, '_, '_>) -> Result<()> {
        self.dispose_mode_field = None;
        if !self.is_async_enumerator() {
            return Ok(());
        }
        let sm = self.state_machine_type.expect("state machine type");
        let md = self.md(ctx.ts);
        let row = md
            .type_methods(sm.row)
            .unwrap_or_default()
            .into_iter()
            .find(|&m| {
                md.get::<rows::MethodDef>(m)
                    .and_then(|d| md.string(d.name))
                    .is_ok_and(|n| n.ends_with(".DisposeAsync"))
            });
        let Some(row) = row else {
            return fail("Could not find DisposeAsync()");
        };
        let read = create_il_ast(ctx, self.outer, self.outer_context, row)?;
        let f = &read.function;
        for store in f.descendants_post(f.body.expect("body")) {
            let Some((target, field, value)) = f.match_stfld(store) else {
                continue;
            };
            if !f.match_ld_this(target) {
                continue;
            }
            if f.match_ldc_i4(value) != Some(1) {
                return fail("");
            }
            if self.dispose_mode_field.is_some() {
                return fail("Multiple stores to disposeMode in DisposeAsync()");
            }
            self.dispose_mode_field = def_of(field);
        }
        Ok(())
    }

    /// `InlineBodyOfMoveNext`: the main try block becomes the method body.
    fn inline_body_of_move_next(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        let mut mn = self.move_next.take().expect("MoveNext");
        let mn_root = mn.function.body.expect("body");
        let Inst::TryCatch { try_block, .. } =
            *mn.function.inst(self.main_try_catch.expect("try-catch"))
        else {
            unreachable!("try-catch");
        };
        let imported = r.function.import(std::mem::take(&mut mn.function));
        let map_node = |n: Option<NodeId>| n.map(|n| imported.node(n));
        let map_var = |v: Option<VarId>| v.map(|v| imported.var(v));
        self.main_try_catch = map_node(self.main_try_catch);
        self.set_result_return_block = map_node(self.set_result_return_block);
        self.set_result_yield_block = map_node(self.set_result_yield_block);
        self.cached_state_var = map_var(self.cached_state_var);
        self.result_var = map_var(self.result_var);
        self.do_finally_bodies = map_var(self.do_finally_bodies);
        for entry in &mut self.cached_field_to_parameter {
            entry.0 = imported.var(entry.0);
        }
        let mn_root = imported.node(mn_root);
        let body = imported.node(try_block);

        let f = &mut r.function;
        set_body(f, body);
        r.async_return_type = Some(self.underlying_return_type);
        let token = 0x0600_0000 | mn.method.row;
        r.move_next_method = ctx
            .ts
            .resolve_method(self.outer.module, token, GenericContext::default())
            .ok()
            .map(|method| MethodOperand {
                range_accessor: None,
                specialized_by: None,
                local_function: None,
                lifted: false,
                token,
                method,
                context: 0,
            });
        r.sequence_point_candidates = std::mem::take(&mut mn.sequence_point_candidates);
        r.code_size = mn.code_size;
        r.local_signature_length = mn.local_signature_length;
        r.is_iterator = self.is_async_enumerator();
        f.release_root(mn_root);

        for branch in f.descendants_post(body) {
            if let Inst::Branch {
                target: BranchTarget::Block(target),
            } = *f.inst(branch)
                && Some(target) == self.set_result_return_block
            {
                let value = match self.result_var {
                    Some(v) => f.add(Inst::LdLoc { var: v }),
                    None => f.add(Inst::Nop {
                        kind: NopKind::Normal,
                    }),
                };
                let leave = f.add(Inst::Leave {
                    target: Some(body),
                    value,
                });
                f.add_il_range(leave, f.il_range(branch));
                f.replace(branch, leave);
            }
        }
        if let Some(yield_block) = self.set_result_yield_block {
            // Branches to it remain for now.
            f.push(body, yield_block);
        }
        for leave in f.descendants_post(body) {
            if let Inst::Leave {
                target: Some(c), ..
            } = *f.inst(leave)
                && c == mn_root
            {
                f.set_leave_target(leave, body);
                self.move_next_leaves.insert(leave);
            }
        }
        let mut registered: HashSet<VarId> = r.variables.iter().copied().collect();
        for n in r.function.descendants_post(body) {
            if let Some(v) = r.function.variable_of(n)
                && registered.insert(v)
            {
                r.variables.push(v);
            }
        }
        remove_dead_variables(r);
        let mut registered: HashSet<VarId> = r.variables.iter().copied().collect();
        for (_, p) in self.field_to_parameter.iter() {
            if registered.insert(*p) {
                r.variables.push(*p);
            }
        }
    }

    fn finalize_inline_move_next(&mut self, r: &mut ReadFunction) {
        let f = &mut r.function;
        let body = f.body.expect("body");
        for leave in f.descendants_post(body) {
            if self.move_next_leaves.contains(&leave) && f.is_connected(leave) {
                let invalid =
                    invalid_branch(f, "leave MoveNext - await not detected correctly".into());
                f.add_il_range(invalid, f.il_range(leave));
                f.replace(leave, invalid);
            }
        }
        // Delete dead loads of the state cache variable.
        for block in f.descendants_post(body) {
            if !matches!(f.inst(block), Inst::Block { .. }) {
                continue;
            }
            let mut i = f.list(block).len();
            while i > 0 {
                i -= 1;
                if let Some((v, value)) = f.match_stloc(f.list(block)[i])
                    && f.var(v).is_single_definition()
                    && f.var(v).load_count() == 0
                    && self.cached_state_var.is_some()
                    && f.match_ldloc(value) == self.cached_state_var
                {
                    f.remove_at(block, i);
                }
            }
        }
    }

    fn clean_up_body_of_move_next(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        let body = r.function.body.expect("body");
        // Copy-propagate stack slots holding an 'ldloca'.
        let stores = single_definition_stores(&r.function, body, |f, var, value| {
            f.var(var).kind == VariableKind::StackSlot
                && matches!(f.inst(value), Inst::LdLoca { .. })
        });
        for store in stores {
            copy_propagation::propagate(&mut r.function, &mut r.variables, ctx, store);
        }
        // Simplify stobj(ldloca) -> stloc.
        let stobjs: Vec<NodeId> = r
            .function
            .descendants_post(body)
            .into_iter()
            .filter(|&n| matches!(r.function.inst(n), Inst::StObj { .. }))
            .collect();
        for stobj in stobjs {
            early_expression_transforms::stobj_to_stloc(&mut r.function, ctx.ts, stobj);
        }
        if self.is_visual_basic_state_machine {
            // Visual Basic keeps the builder field's ldflda in stack slots, which the await
            // matchers would not see through.
            let builder_field = self.builder_field;
            let stores = single_definition_stores(&r.function, body, |f, var, value| {
                f.var(var).kind == VariableKind::StackSlot
                    && f.match_ldflda(value).is_some_and(|(target, field)| {
                        // shortcut: `field.Equals(builderField)` is false for a specialized field; the definition is compared
                        is_field(def_of(field), builder_field) && f.match_ld_this(target)
                    })
            });
            for store in stores {
                copy_propagation::propagate(&mut r.function, &mut r.variables, ctx, store);
            }
            // Lone 'ldc.i4' of older Roslyn VB compilers.
            remove_lone_ldc_i4(&mut r.function, body);
        }
        // Copy-propagate temporaries holding a copy of 'this'.
        let stores =
            single_definition_stores(&r.function, body, |f, _, value| f.match_ld_this(value));
        for store in stores {
            copy_propagation::propagate(&mut r.function, &mut r.variables, ctx, store);
        }
        remove_dead_variable_init::run(r, ctx);
        let body = r.function.body.expect("body");
        let blocks: Vec<NodeId> = r
            .function
            .descendants_post(body)
            .into_iter()
            .filter(|&n| matches!(r.function.inst(n), Inst::Block { .. }))
            .collect();
        for block in blocks {
            // Dead variables stay: TranslateFieldsToLocalAccess may revive them.
            inline_all_in_block(&mut r.function, block, options::NONE, ctx);
            if self.is_async_enumerator() {
                // The C# compiler sometimes leaves a lone 'ldc.i4' of an optimized-out store to
                // the state variable.
                r.function
                    .retain(block, |f, i| !matches!(f.inst(i), Inst::LdcI4 { .. }));
            }
        }
    }

    /// `AnalyzeStateMachine`: replaces 'leave IL_0000' with an await and a jump to the block
    /// the next `MoveNext()` call enters.
    fn analyze_state_machine(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        self.smallest_awaiter_var_index = i32::MAX;
        let body = r.function.body.expect("body");
        let containers: Vec<NodeId> = r
            .function
            .descendants_post(body)
            .into_iter()
            .filter(|&n| matches!(r.function.inst(n), Inst::BlockContainer { .. }))
            .collect();
        for container in containers {
            for block in r.function.list(container).to_vec() {
                self.inline_awaiter_copies(&mut r.function, ctx, block);
            }
            // Fold the runtime ICriticalNotifyCompletion check into the single-call form.
            for block in r.function.list(container).to_vec() {
                self.normalize_await_on_completed_dual_branch(&mut r.function, ctx.ts, block);
            }
            // A state range analysis per container.
            let mut analysis = StateRangeAnalysis::new(
                Mode::AsyncMoveNext,
                self.state_field,
                self.cached_state_var,
                false,
            );
            analysis.do_finally_bodies = self.do_finally_bodies;
            analysis
                .assign_state_ranges(&r.function, ctx.ts, container, LongSet::universe())
                .unwrap_or_else(|e| panic!("SymbolicAnalysisFailedException: {}", e.0));
            let state_to_block = analysis.block_state_set_mapping(&r.function, container);

            for block in r.function.list(container).to_vec() {
                dynamic_call_site::run_on_basic_block(&mut r.function, ctx, block);
                let f = &mut r.function;
                let last = *f.list(block).last().expect("block ends with a jump");
                if matches!(f.inst(last), Inst::Leave { .. })
                    && self.move_next_leaves.contains(&last)
                {
                    // Likely an 'await' block.
                    if let Some((awaiter, awaiter_field, state, yield_offset)) =
                        self.analyze_await_block(f, ctx.ts, block)
                    {
                        let ldloca = f.add(Inst::LdLoca { var: awaiter });
                        let await_inst = f.add(Inst::Await {
                            value: ldloca,
                            get_awaiter: None,
                            get_result: None,
                            get_awaiter_callvirt: false,
                        });
                        f.push(block, await_inst);
                        let jump = match state_to_block.get(i64::from(state)) {
                            Some(&target) => {
                                self.await_debug_infos
                                    .push((yield_offset, f.il_range(target).start));
                                f.add(Inst::Branch {
                                    target: BranchTarget::Block(target),
                                })
                            }
                            None => {
                                invalid_branch(f, format!("Could not find block for state {state}"))
                            }
                        };
                        f.push(block, jump);
                        self.await_blocks.insert(block, (awaiter, awaiter_field));
                        if let Some(index) = f.var(awaiter).index
                            && index < self.smallest_awaiter_var_index
                        {
                            self.smallest_awaiter_var_index = index;
                        }
                    }
                } else if self.set_result_yield_block.is_some()
                    && f.match_branch(last) == self.set_result_yield_block
                {
                    // A 'yield return' in an async enumerator.
                    let jump = match self.analyze_yield_return(f, ctx.ts, block) {
                        Some((value, state)) => {
                            let yield_return = f.add(Inst::YieldReturn { value });
                            f.push(block, yield_return);
                            match state_to_block.get(i64::from(state)) {
                                Some(&target) => f.add(Inst::Branch {
                                    target: BranchTarget::Block(target),
                                }),
                                None => invalid_branch(
                                    f,
                                    format!("Could not find block for state {state}"),
                                ),
                            }
                        }
                        None => invalid_branch(f, "Could not detect 'yield return'".into()),
                    };
                    f.push(block, jump);
                }
                self.transform_yield_break(f, ctx.ts, block);
            }
            for block in r.function.list(container).to_vec() {
                self.simplify_if_dispose_mode(&mut r.function, ctx.ts, block);
            }
            let f = &mut r.function;
            // Skip the state dispatcher: jump directly to the initial state.
            if let Some(&entry) = state_to_block.get(i64::from(self.initial_state)) {
                let block = new_block(f);
                let jump = f.add(Inst::Branch {
                    target: BranchTarget::Block(entry),
                });
                f.push(block, jump);
                f.insert(container, 0, block);
            }
            sort_blocks(f, container);
            // Re-join the chains of collapsed dynamic awaiter call sites for
            // DetectAwaitPattern, after the unreachable init blocks are gone.
            self.coalesce_dynamic_awaiter_blocks(&mut r.function, ctx, container);
            sort_blocks(&mut r.function, container);
        }
    }

    fn transform_yield_break(
        &self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) -> bool {
        // stfld disposeMode(ldloc this, ldc.i4 1)
        // br nextBlock
        let list = f.list(block);
        if list.len() < 2 {
            return false;
        }
        let br = list[list.len() - 1];
        let Some(target_block) = f.match_branch(br) else {
            return false;
        };
        let Some((target, field, value)) = f.match_stfld(list[list.len() - 2]) else {
            return false;
        };
        if !f.match_ld_this(target)
            || !is_field(def_of(field), self.dispose_mode_field)
            || f.match_ldc_i4(value) != Some(1)
        {
            return false;
        }
        // A 'yield break;'.
        match self.find_yield_break_target(f, ts, target_block) {
            YieldBreakTarget::Block(b) => f.set_branch_target(br, b),
            YieldBreakTarget::Container(c) => {
                let nop = f.add(Inst::Nop {
                    kind: NopKind::Normal,
                });
                let leave = f.add(Inst::Leave {
                    target: Some(c),
                    value: nop,
                });
                f.add_il_range(leave, f.il_range(br));
                f.replace(br, leave);
            }
        }
        true
    }

    /// `FindYieldBreakTarget`: follows the jumps from `block` assuming disposeMode == 1.
    fn find_yield_break_target(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        mut block: NodeId,
    ) -> YieldBreakTarget {
        let mut visited = HashSet::new();
        let eval = SymbolicEvaluationContext::new(self.dispose_mode_field, false);
        'blocks: loop {
            for &first in f.list(block) {
                let mut inst = first;
                while let Inst::If {
                    condition,
                    true_inst,
                    false_inst,
                    ..
                } = *f.inst(inst)
                {
                    inst = match eval.eval(f, ts, condition).as_bool() {
                        SymbolicValue::IntegerConstant(c) => {
                            if c != 0 {
                                true_inst
                            } else {
                                false_inst
                            }
                        }
                        SymbolicValue::StateInSet(set) => {
                            if set.contains(1) {
                                true_inst
                            } else {
                                false_inst
                            }
                        }
                        _ => return YieldBreakTarget::Block(block),
                    };
                }
                if let Some(target) = f.match_branch(inst) {
                    if visited.insert(block) {
                        block = target;
                        continue 'blocks;
                    }
                    // An infinite loop.
                    return YieldBreakTarget::Block(block);
                }
                match *f.inst(inst) {
                    Inst::Leave {
                        target: Some(c),
                        value,
                    } if matches!(f.inst(value), Inst::Nop { .. }) => {
                        return YieldBreakTarget::Container(c);
                    }
                    Inst::Nop { .. } => continue,
                    _ => return YieldBreakTarget::Block(block),
                }
            }
        }
    }

    fn is_ld_dispose_mode(&self, f: &Function, ts: &mut TypeSystem<'_, '_>, inst: NodeId) -> bool {
        f.match_ldfld(ts, inst).is_some_and(|(target, field)| {
            f.match_ld_this(target) && is_field(def_of(field), self.dispose_mode_field)
        })
    }

    /// `SimplifyIfDisposeMode`.
    fn simplify_if_dispose_mode(
        &self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) -> bool {
        // Roslyn may optimize out an "if (disposeMode)" but keep the field access.
        let dead: Vec<NodeId> = f
            .list(block)
            .iter()
            .copied()
            .filter(|&i| self.is_ld_dispose_mode(f, ts, i))
            .collect();
        if !dead.is_empty() {
            f.retain(block, |_, i| !dead.contains(&i));
        }
        // if (logic.not(ldfld disposeMode(ldloc this))) br falseInst
        // br trueInst
        let Some((condition, _, false_inst)) = f.match_if_at_end_of_block(block) else {
            return false;
        };
        if !self.is_ld_dispose_mode(f, ts, condition) {
            return false;
        }
        let n = f.list(block).len();
        let if_inst = f.list(block)[n - 2];
        f.replace(if_inst, false_inst);
        f.remove_at(block, n - 1);
        true
    }

    /// `NormalizeAwaitOnCompletedDualBranch`: when the awaiter's static type may not
    /// implement ICriticalNotifyCompletion, the compiler chooses AwaitOnCompleted or
    /// AwaitUnsafeOnCompleted at runtime. Both await the same awaiter, so the diamond folds
    /// into the single-call form.
    fn normalize_await_on_completed_dual_branch(
        &mut self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) -> bool {
        let count = f.list(block).len();
        if count < 3 {
            return false;
        }
        let list = f.list(block).to_vec();
        let Some(on_completed) = f.match_branch(list[count - 1]) else {
            return false;
        };
        let Some((condition, true_branch)) = f.match_if_instruction(list[count - 2]) else {
            return false;
        };
        let Some(unsafe_on_completed) = f.match_branch(true_branch) else {
            return false;
        };
        let Some(critical_var) = f
            .match_comp_null(condition, ComparisonKind::Inequality)
            .and_then(|t| f.match_ldloc(t))
        else {
            return false;
        };
        let mut isinst_pos = count as isize - 3;
        let mut sm_var = None;
        if isinst_pos >= 0
            && let Some((v, value)) = f.match_stloc(list[isinst_pos as usize])
            && f.match_ld_this(value)
        {
            sm_var = Some(v);
            isinst_pos -= 1;
        }
        if isinst_pos < 0 {
            return false;
        }
        let isinst_pos = isinst_pos as usize;
        let Some((v, critical_value)) = f.match_stloc(list[isinst_pos]) else {
            return false;
        };
        if v != critical_var {
            return false;
        }
        let Inst::IsInst { arg, ty } = *f.inst(critical_value) else {
            return false;
        };
        if ts.full_name(ty.ty) != "System.Runtime.CompilerServices.ICriticalNotifyCompletion" {
            return false;
        }
        let Some(awaiter_var) = f.match_ldloc(arg) else {
            return false;
        };
        let matches_state_machine_arg = |f: &Function, a: NodeId| match sm_var {
            Some(sm) => matches!(*f.inst(a), Inst::LdLoca { var } if var == sm),
            None => f.match_ld_this(a),
        };

        // unsafeOnCompleted: call AwaitUnsafeOnCompleted(builder, ldloca criticalVar, smArg); br merge
        let [unsafe_call, unsafe_br] = *f.list(unsafe_on_completed) else {
            return false;
        };
        let Some(unsafe_args) = match_call(f, unsafe_call, "AwaitUnsafeOnCompleted") else {
            return false;
        };
        if unsafe_args.len() != 3
            || !self.is_builder_field_on_this(f, ts, unsafe_args[0])
            || !matches!(*f.inst(unsafe_args[1]), Inst::LdLoca { var } if var == critical_var)
            || !matches_state_machine_arg(f, unsafe_args[2])
        {
            return false;
        }
        let Some(merge) = f.match_branch(unsafe_br) else {
            return false;
        };

        // onCompleted: stloc notifyVar(castclass INotifyCompletion(ldloc awaiterVar));
        // call AwaitOnCompleted(builder, ldloca notifyVar, smArg); br merge
        let [notify_store, safe_call, safe_br] = *f.list(on_completed) else {
            return false;
        };
        let Some((notify_var, notify_value)) = f.match_stloc(notify_store) else {
            return false;
        };
        let Inst::CastClass {
            arg: cast_arg,
            ty: cast_ty,
        } = *f.inst(notify_value)
        else {
            return false;
        };
        if ts.full_name(cast_ty.ty) != "System.Runtime.CompilerServices.INotifyCompletion"
            || f.match_ldloc(cast_arg) != Some(awaiter_var)
        {
            return false;
        }
        let Some(safe_args) = match_call(f, safe_call, "AwaitOnCompleted") else {
            return false;
        };
        if safe_args.len() != 3
            || !self.is_builder_field_on_this(f, ts, safe_args[0])
            || !matches!(*f.inst(safe_args[1]), Inst::LdLoca { var } if var == notify_var)
            || !matches_state_machine_arg(f, safe_args[2])
        {
            return false;
        }
        if f.match_branch(safe_br) != Some(merge) {
            return false;
        }
        // The pre-Roslyn compiler clears doFinallyBodies right before leaving MoveNext.
        let leave_pos = usize::from(
            f.list(merge)
                .first()
                .is_some_and(|&i| self.match_store_do_finally_bodies(f, ts, i)),
        );
        if f.list(merge).len() != leave_pos + 1 {
            return false;
        }
        let merge_leave = f.list(merge)[leave_pos];
        if !matches!(f.inst(merge_leave), Inst::Leave { .. }) {
            return false;
        }

        // Rewrite `block` into the single-call form.
        let await_call = f.remove_at(unsafe_on_completed, 0);
        let awaiter_arg = match f.inst(await_call) {
            Inst::Call(c) | Inst::CallVirt(c) => c.args[1],
            _ => unreachable!("matched a call"),
        };
        let ldloca = f.add(Inst::LdLoca { var: awaiter_var });
        f.replace(awaiter_arg, ldloca);
        // The isinst test, the if and the branch go; the state, awaiter field and smVar
        // stores stay.
        f.truncate(block, count - 2);
        f.remove_at(block, isinst_pos);
        f.push(block, await_call);
        if leave_pos > 0 {
            let store = f.list(merge)[0];
            let copy = f.clone_subtree(store);
            f.push(block, copy);
        }
        let new_leave = f.clone_subtree(merge_leave);
        self.move_next_leaves.insert(new_leave);
        f.push(block, new_leave);
        true
    }

    /// `MatchStoreDoFinallyBodies`: the 'stloc doFinallyBodies(ldc.i4 0)' before leaving
    /// MoveNext.
    fn match_store_do_finally_bodies(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        inst: NodeId,
    ) -> bool {
        let Some(do_finally) = self.do_finally_bodies else {
            return false;
        };
        f.match_stloc(inst).is_some_and(|(v, value)| {
            f.var(v).kind == VariableKind::Local
                && is_known(ts, f.var(v).ty, KnownTypeCode::Boolean)
                && f.var(v).index == f.var(do_finally).index
                && f.match_ldc_i4(value) == Some(0)
        })
    }

    /// `InlineAwaiterCopies`: the pre-Roslyn compiler copies the awaiter into a fresh local
    /// before each completion-interface type test; the await matchers identify an await by
    /// its awaiter variable.
    fn inline_awaiter_copies(
        &self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        block: NodeId,
    ) {
        let mut i = f.list(block).len();
        while i > 0 {
            i -= 1;
            let Some(&store) = f.list(block).get(i) else {
                continue;
            };
            let Some((copy, value)) = f.match_stloc(store) else {
                continue;
            };
            let var = f.var(copy);
            if var.kind != VariableKind::Local
                || !var.is_single_definition()
                || var.load_count() != 1
                || !matches!(f.inst(value), Inst::LdLoc { .. })
            {
                continue;
            }
            let Some(use_parent) = f.node(var.load_instructions[0]).parent else {
                continue;
            };
            let is_awaiter_access = match *f.inst(use_parent) {
                Inst::Dynamic {
                    kind: DynamicKind::GetMember,
                    ref name,
                    ..
                } => name.as_deref() == Some("IsCompleted"),
                Inst::Dynamic {
                    kind: DynamicKind::InvokeMember,
                    ref name,
                    ..
                } => name.as_deref() == Some("GetResult"),
                Inst::IsInst { ty, .. } | Inst::CastClass { ty, .. } => {
                    let name = ctx.ts.full_name(ty.ty);
                    name == "System.Runtime.CompilerServices.ICriticalNotifyCompletion"
                        || name == "System.Runtime.CompilerServices.INotifyCompletion"
                }
                _ => false,
            };
            if is_awaiter_access {
                inline_one(f, store, options::AGGRESSIVE, ctx);
            }
        }
    }

    fn analyze_await_block(
        &self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) -> Option<(VarId, Option<FieldDef>, i32, i32)> {
        let list = f.list(block).to_vec();
        let mut pos = list.len() as isize - 2;
        let get = |p: isize| (p >= 0).then(|| list.get(p as usize).copied()).flatten();
        if pos >= 0
            && self.do_finally_bodies.is_some()
            && matches!(f.inst(list[pos as usize]), Inst::StLoc { .. })
        {
            if !self.match_store_do_finally_bodies(f, ts, list[pos as usize]) {
                return None;
            }
            pos -= 1;
        }
        // call AwaitUnsafeOnCompleted(ldflda <>t__builder(ldloc this), ldloca awaiter, ldloc this),
        // or AwaitOnCompleted when the awaiter does not implement ICriticalNotifyCompletion.
        let call = get(pos)?;
        let args = match_call(f, call, "AwaitUnsafeOnCompleted")
            .or_else(|| match_call(f, call, "AwaitOnCompleted"))?
            .to_vec();
        if args.len() != 3 || !self.is_builder_field_on_this(f, ts, args[0]) {
            return None;
        }
        let Inst::LdLoca { var: awaiter } = *f.inst(args[1]) else {
            return None;
        };
        if f.match_ld_this(args[2]) {
            // A struct state machine.
            pos -= 1;
        } else if let Inst::LdLoca { var: temp } = *f.inst(args[2]) {
            // Non-optimized Roslyn uses a class: stloc tempVar(ldloc this)
            if !(pos > 0
                && f.match_stloc(list[pos as usize - 1])
                    .is_some_and(|(v, value)| v == temp && f.match_ld_this(value)))
            {
                return None;
            }
            pos -= 2;
        } else {
            return None;
        }
        // stfld StateMachine.<>awaiter(ldloc this, ldloc awaiter)
        let (target, awaiter_field, value) = f.match_stfld(get(pos)?)?;
        if !f.match_ld_this(target) || f.match_ldloc(value) != Some(awaiter) {
            return None;
        }
        let awaiter_field = def_of(awaiter_field);
        pos -= 1;
        // The IL offset for the debug info.
        let state_store = get(pos)?;
        let yield_offset = f.il_range(state_store).end;
        // stfld <>1__state(ldloc S_10, ldloc S_11), the slots holding this and the state
        let (target, field, value) = f.match_stfld(state_store)?;
        if !f.match_ld_this(stack_slot_value(f, target))
            || !is_field(def_of(field), self.state_field)
        {
            return None;
        }
        let state = f.match_ldc_i4(stack_slot_value(f, value))?;
        if pos > 0
            && let Some((v, value)) = f.match_stloc(list[pos as usize - 1])
            && f.var(v).kind == VariableKind::Local
            && self
                .cached_state_var
                .is_some_and(|c| f.var(v).index == f.var(c).index)
            && f.match_ldc_i4(stack_slot_value(f, value)) == Some(state)
        {
            // The assignment to cachedStateVar goes too.
            pos -= 1;
        }
        let mut pos = pos as usize;
        f.truncate(block, pos);
        // Preceding dead stores.
        while pos > 0
            && let Some((v, value)) = f.match_stloc(f.list(block)[pos - 1])
            && f.var(v).is_single_definition()
            && f.var(v).load_count() == 0
            && f.var(v).kind == VariableKind::StackSlot
            && is_pure(f.flags(value))
        {
            pos -= 1;
        }
        f.truncate(block, pos);
        Some((awaiter, awaiter_field, state, yield_offset))
    }

    fn analyze_yield_return(
        &self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) -> Option<(NodeId, i32)> {
        let _ = ts;
        // stfld current(ldloc this, ldstr "yieldValue")
        // stloc S_45(ldloc this); stloc S_46(ldc.i4 -5); stloc V_0(ldloc S_46)
        // stfld stateField(ldloc S_45, ldloc S_46)
        // br setResultYieldBlock
        let list = f.list(block).to_vec();
        let mut pos = list.len() as isize - 2;
        // A state assignment is right before the 'yield return'.
        if pos < 0 {
            return None;
        }
        let new_state = self.match_state_assignment(f, list[pos as usize])?;
        pos -= 1;
        if pos >= 0
            && let Some((v, value)) = f.match_stloc(list[pos as usize])
            && Some(v) == self.cached_state_var
        {
            if f.match_ldc_i4(stack_slot_value(f, value)) == Some(new_state) {
                pos -= 1;
            } else {
                return None;
            }
        }
        while pos >= 0
            && let Some((v, value)) = f.match_stloc(list[pos as usize])
        {
            if f.var(v).kind != VariableKind::StackSlot || !is_pure(f.flags(value)) {
                return None;
            }
            pos -= 1;
        }
        if pos < 0 {
            return None;
        }
        // stfld current(...); the field is not compared with get_Current.
        let (target, _, value) = f.match_stfld(list[pos as usize])?;
        if !f.match_ld_this(stack_slot_value(f, target)) {
            return None;
        }
        f.truncate(block, pos as usize);
        Some((value, new_state))
    }

    /// `CoalesceDynamicAwaiterBlocks`: joins chains of blocks linked by unconditional
    /// branches, except the await blocks (whose branch to the resume block must stay).
    fn coalesce_dynamic_awaiter_blocks(
        &self,
        f: &mut Function,
        ctx: &mut Context<'_, '_, '_>,
        container: NodeId,
    ) {
        let mut i = 0;
        while let Some(&block) = f.list(container).get(i) {
            i += 1;
            if f.list(block).is_empty() || self.await_blocks.contains_key(&block) {
                continue;
            }
            while combine_block_with_next_block(f, container, block, ctx) {}
            // Only now is each awaiter copy in the block of the call site using it.
            self.inline_awaiter_copies(f, ctx, block);
        }
    }

    fn detect_await_pattern(&mut self, r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
        let body = r.function.body.expect("body");
        let containers: Vec<NodeId> = r
            .function
            .descendants_post(body)
            .into_iter()
            .filter(|&n| matches!(r.function.inst(n), Inst::BlockContainer { .. }))
            .collect();
        for container in containers {
            for block in r.function.list(container).to_vec() {
                self.detect_await_pattern_in(&mut r.function, ctx.ts, block);
            }
            sort_blocks(&mut r.function, container);
        }
    }

    fn detect_await_pattern_in(
        &mut self,
        f: &mut Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
    ) {
        // block:
        //   stloc awaiterVar(callvirt GetAwaiter(...))
        //   if (call get_IsCompleted(ldloca awaiterVar)) br completedBlock
        //   br awaitBlock
        // awaitBlock: .. br resumeBlock
        // resumeBlock: .. br completedBlock
        let list = f.list(block).to_vec();
        let n = list.len();
        if n < 3 {
            return;
        }
        let Some((awaiter_var, get_awaiter_value)) = f.match_stloc(list[n - 3]) else {
            return;
        };
        let (awaited_value, get_awaiter_method, get_awaiter_callvirt, is_dynamic_await) = match f
            .inst(get_awaiter_value)
        {
            Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)
                if c.method.method.name == "GetAwaiter"
                    && (!c.method.method.is_static || self.is_extension_method(ts, &c.method))
                    && c.args.len() == 1 =>
            {
                (
                    c.args[0],
                    AwaiterMethod::Method(c.method.clone()),
                    matches!(f.inst(get_awaiter_value), Inst::CallVirt(_)),
                    false,
                )
            }
            // Awaiting a dynamic value: GetAwaiter, IsCompleted and GetResult are call sites.
            Inst::Dynamic {
                kind: DynamicKind::InvokeMember,
                name: Some(name),
                args,
                ..
            } if name == "GetAwaiter" && args.len() == 1 => (
                args[0],
                AwaiterMethod::Dynamic("GetAwaiter".into()),
                true,
                true,
            ),
            _ => return,
        };
        let Some((mut condition, true_inst)) = f.match_if_instruction(list[n - 2]) else {
            return;
        };
        let Some(mut completed_block) = f.match_branch(true_inst) else {
            return;
        };
        let Some(mut await_block) = f.match_branch(list[n - 1]) else {
            return;
        };
        // An inverted condition swaps the branches.
        if let Some(negated) = f.match_logic_not(condition) {
            condition = negated;
            std::mem::swap(&mut completed_block, &mut await_block);
        }
        if is_dynamic_await {
            // if (dynamic.convert bool(dynamic.getmember "IsCompleted"(ldloc awaiterVar)))
            let mut test = condition;
            if let Inst::Dynamic {
                kind: DynamicKind::Convert,
                ty: Some(t),
                ref args,
                ..
            } = *f.inst(test)
                && is_known(ts, t, KnownTypeCode::Boolean)
            {
                test = args[0];
            }
            let is_completed = match f.inst(test) {
                Inst::Dynamic {
                    kind: DynamicKind::GetMember,
                    name: Some(name),
                    args,
                    ..
                } if name == "IsCompleted" => f.match_ldloc(args[0]) == Some(awaiter_var),
                _ => false,
            };
            if !is_completed {
                return;
            }
        } else {
            let Some(args) = match_call(f, condition, "get_IsCompleted") else {
                return;
            };
            if args.len() != 1
                || f.match_ld_loc_ref(ts, unwrap_conv_unknown(f, args[0])) != Some(awaiter_var)
            {
                return;
            }
        }
        let Some(&(await_block_var, awaiter_field)) = self.await_blocks.get(&await_block) else {
            return;
        };
        if await_block_var != awaiter_var {
            return;
        }
        let Some((resume_block, stack_field)) = check_await_block(f, await_block) else {
            return;
        };
        if !self.check_resume_block(
            f,
            ts,
            resume_block,
            awaiter_var,
            awaiter_field,
            completed_block,
            stack_field,
        ) {
            return;
        }
        // The first instruction of completedBlock holds the GetResult call, maybe inlined.
        let Some(&first) = f.list(completed_block).first() else {
            return;
        };
        let (get_result_call, get_result_method) = if is_dynamic_await {
            // dynamic.invokemember "GetResult"(ldloc awaiterVar), possibly inlined
            let found = std::iter::once(first)
                .chain(f.descendants_post(first))
                .find(|&n| match f.inst(n) {
                    Inst::Dynamic {
                        kind: DynamicKind::InvokeMember,
                        name: Some(name),
                        args,
                        ..
                    } => {
                        name == "GetResult"
                            && args.len() == 1
                            && f.match_ldloc(args[0]) == Some(awaiter_var)
                    }
                    _ => false,
                });
            let Some(found) = found else {
                return;
            };
            (found, AwaiterMethod::Dynamic("GetResult".into()))
        } else {
            let Some(call) = f.find_first_inlined_call(first) else {
                return;
            };
            let Some(get_result_args) = match_call(f, call, "GetResult") else {
                return;
            };
            if get_result_args.len() != 1
                || f.match_ld_loc_ref(ts, unwrap_conv_unknown(f, get_result_args[0]))
                    != Some(awaiter_var)
            {
                return;
            }
            let method = match f.inst(call) {
                Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c) => c.method.clone(),
                _ => unreachable!("a call"),
            };
            (call, AwaiterMethod::Method(method))
        };

        // Transform.
        f.remove_at(block, n - 3);
        f.remove_at(block, n - 3);
        let last = *f.list(block).last().expect("branch");
        f.set_branch_target(last, completed_block);
        let value = unwrap_conv_unknown(f, awaited_value);
        let await_inst = f.add(Inst::Await {
            value,
            get_awaiter: Some(get_awaiter_method),
            get_result: Some(get_result_method),
            get_awaiter_callvirt,
        });
        f.replace(get_result_call, await_inst);

        // A useless reset of awaiterVar.
        if let Some(&second) = f.list(completed_block).get(1)
            && let Inst::StObj { target, value, .. } = *f.inst(second)
            && matches!(*f.inst(target), Inst::LdLoca { var } if var == awaiter_var)
            && matches!(f.inst(value), Inst::DefaultValue { .. })
        {
            f.remove_at(completed_block, 1);
        }
    }

    fn is_extension_method(&self, ts: &mut TypeSystem<'_, '_>, m: &MethodOperand) -> bool {
        match m.method.origin {
            MethodOrigin::Def(d) => {
                let owner = ts.accessor_owner(d).map(|(o, _)| o);
                ts.method_flags(d, owner).extension
            }
            _ => false,
        }
    }

    #[allow(clippy::too_many_arguments)]
    fn check_resume_block(
        &self,
        f: &Function,
        ts: &mut TypeSystem<'_, '_>,
        block: NodeId,
        awaiter_var: VarId,
        awaiter_field: Option<FieldDef>,
        completed_block: NodeId,
        stack_field: Option<Option<FieldDef>>,
    ) -> bool {
        let mut pos = 0;
        if !restore_stack(f, ts, block, &mut pos, stack_field) {
            return false;
        }
        let store_to_awaiter =
            |f: &Function, ts: &mut TypeSystem<'_, '_>, inst: Option<NodeId>| -> bool {
                // stloc awaiterVar(ldfld awaiterField(ldloc this))
                let Some((v, mut value)) = inst.and_then(|i| f.match_stloc(i)) else {
                    return false;
                };
                if v != awaiter_var {
                    return false;
                }
                // A reference-type awaiter may be kept in an `object` field and cast back.
                if let Inst::CastClass { arg, ty } | Inst::UnboxAny { arg, ty } = *f.inst(value)
                    && ty.ty == f.var(awaiter_var).ty
                {
                    value = arg;
                }
                f.match_ldfld(ts, value).is_some_and(|(target, field)| {
                    f.match_ld_this(target) && def_of(field) == awaiter_field
                })
            };
        let reset_awaiter_field = |f: &Function, ts: &mut TypeSystem<'_, '_>, pos: &mut usize| {
            // stfld awaiterField(ldloc this, default.value)
            if let Some((target, field, value)) = at(f, block, *pos).and_then(|i| f.match_stfld(i))
                && f.match_ld_this(target)
                && def_of(field) == awaiter_field
                && matches!(f.inst(value), Inst::DefaultValue { .. } | Inst::LdNull)
            {
                *pos += 1;
            } else if let Some((v, value)) = at(f, block, *pos).and_then(|i| f.match_stloc(i))
                && matches!(f.inst(value), Inst::DefaultValue { .. })
                && let Some((_, field, stored)) =
                    at(f, block, *pos + 1).and_then(|i| f.match_stfld(i))
                && def_of(field) == awaiter_field
                && f.match_ldloc(stored) == Some(v)
            {
                // stloc V_6(default.value TaskAwaiter); stobj TaskAwaiter(ldflda <>u__$awaiter4(ldloc this), ldloc V_6)
                *pos += 2;
            }
            let _ = ts;
        };
        if self.is_visual_basic_state_machine {
            // Visual Basic assigns the fields in another order.
            if !self.match_state_field_assignment(f, block, &mut pos) {
                return false;
            }
            pos += 1;
            if !store_to_awaiter(f, ts, at(f, block, pos)) {
                return false;
            }
            pos += 1;
            reset_awaiter_field(f, ts, &mut pos);
        } else {
            if !store_to_awaiter(f, ts, at(f, block, pos)) {
                return false;
            }
            pos += 1;
            reset_awaiter_field(f, ts, &mut pos);
            if !self.match_state_field_assignment(f, block, &mut pos) {
                return false;
            }
            pos += 1;
        }
        at(f, block, pos).and_then(|i| f.match_branch(i)) == Some(completed_block)
    }

    /// `MatchStateFieldAssignement`:
    ///   stloc S_28(ldc.i4 -1); stloc cachedStateVar(ldloc S_28); stfld <>1__state(ldloc this, ldloc S_28)
    fn match_state_field_assignment(&self, f: &Function, block: NodeId, pos: &mut usize) -> bool {
        let mut m1_var = None;
        if let Some((v, value)) = at(f, block, *pos).and_then(|i| f.match_stloc(i))
            && f.match_ldc_i4(value) == Some(self.initial_state)
            && f.var(v).kind == VariableKind::StackSlot
        {
            m1_var = Some(v);
            *pos += 1;
        }
        if let Some((v, value)) = at(f, block, *pos).and_then(|i| f.match_stloc(i))
            && f.var(v).kind == VariableKind::Local
            && f.var(v).index == self.cached_state_var.and_then(|c| f.var(c).index)
            && ((m1_var.is_some() && f.match_ldloc(value) == m1_var)
                || f.match_ldc_i4(value) == Some(self.initial_state))
        {
            *pos += 1;
        }
        let Some((target, field, value)) = at(f, block, *pos).and_then(|i| f.match_stfld(i)) else {
            return false;
        };
        f.match_ld_this(target)
            && is_field(def_of(field), self.state_field)
            && (f.match_ldc_i4(value) == Some(self.initial_state)
                || (m1_var.is_some() && f.match_ldloc(value) == m1_var))
    }

    /// `CleanDoFinallyBodies`: removes the use of doFinallyBodies.
    fn clean_do_finally_bodies(&mut self, r: &mut ReadFunction) {
        let Some(do_finally) = self.do_finally_bodies else {
            // Roslyn-compiled code has none.
            return;
        };
        let f = &mut r.function;
        let body = f.body.expect("body");
        let entry = get_body_entry_point(f, body);
        if let Some((v, value)) = f.list(entry).first().and_then(|&i| f.match_stloc(i))
            && v == do_finally
            && f.match_ldc_i4(value) == Some(1)
        {
            // The initial assignment, unless the control flow rearrangement removed it.
            f.remove_at(entry, 0);
        }
        if f.var(do_finally).store_count() != 0 || f.var(do_finally).address_count() != 0 {
            // Another variable misdetected as doFinallyBodies: the initial store comes back.
            let one = f.add(Inst::LdcI4 { value: 1 });
            let store = f.add(Inst::StLoc {
                var: do_finally,
                value: one,
                il_stack_was_empty: false,
                is_stack_adjustment: false,
            });
            let entry = f.list(body)[0];
            f.insert(entry, 0, store);
            return;
        }
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
                && f.match_logic_not(condition)
                    .is_some_and(|a| f.match_ldloc(a) == Some(do_finally))
            {
                // Always false with 'await' instructions.
                f.remove_at(entry, 0);
            }
        }
        // Any remaining load becomes the constant 1.
        for load in f.var(do_finally).load_instructions.clone() {
            let one = f.add(Inst::LdcI4 { value: 1 });
            f.add_il_range(one, f.il_range(load));
            f.replace(load, one);
        }
    }

    /// `TranslateCachedFieldsToLocals`.
    fn translate_cached_fields_to_locals(&self, r: &mut ReadFunction) {
        let f = &mut r.function;
        for &(cached, param) in &self.cached_field_to_parameter {
            for inst in f.var(cached).load_instructions.clone() {
                f.set_variable(inst, param);
            }
            for inst in f.var(cached).address_instructions.clone() {
                f.set_variable(inst, param);
            }
        }
    }
}

enum YieldBreakTarget {
    Block(NodeId),
    Container(NodeId),
}

/// The stores of single-definition variables under `root` that `keep` accepts, as a snapshot.
fn single_definition_stores(
    f: &Function,
    root: NodeId,
    keep: impl Fn(&Function, VarId, NodeId) -> bool,
) -> Vec<NodeId> {
    f.descendants_post(root)
        .into_iter()
        .filter(|&n| match *f.inst(n) {
            Inst::StLoc { var, value, .. } => {
                f.var(var).is_single_definition() && keep(f, var, value)
            }
            _ => false,
        })
        .collect()
}

fn remove_lone_ldc_i4(f: &mut Function, root: NodeId) {
    let blocks: Vec<NodeId> = f
        .descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect();
    for block in blocks {
        f.retain(block, |f, i| !matches!(f.inst(i), Inst::LdcI4 { .. }));
    }
}

/// `MatchHoistedLocalCleanup`: `stfld <>u__N(ldloc this, default)` stores
/// (https://github.com/dotnet/roslyn/pull/39735).
fn match_hoisted_local_cleanup(f: &Function, block: NodeId, pos: &mut usize) -> Result<()> {
    while let Some((target, _, value)) = at(f, block, *pos).and_then(|i| f.match_stfld(i)) {
        if !f.match_ld_this(target) || !f.match_default_or_null_or_zero(value) {
            return fail("");
        }
        *pos += 1;
    }
    Ok(())
}

/// `CheckAwaitBlock`: `(pre-Roslyn: save stack) await(ldloca V_2); br resumeBlock`. The
/// resume block, and the stack field (`Some(None)` for an unresolved field).
fn check_await_block(f: &Function, block: NodeId) -> Option<(NodeId, Option<Option<FieldDef>>)> {
    let list = f.list(block);
    if list.len() < 2 {
        return None;
    }
    let mut pos = 0;
    let mut stack_field = None;
    if let Some((v, _)) = f.match_stloc(list[0])
        && f.var(v).is_single_definition()
    {
        let (target, field, _) = f.match_stfld(*list.get(1)?)?;
        if !f.match_ld_this(target) {
            return None;
        }
        stack_field = Some(def_of(field));
        pos = 2;
    }
    if !matches!(f.inst(*list.get(pos)?), Inst::Await { .. }) {
        return None;
    }
    let resume = f.match_branch(*list.get(pos + 1)?)?;
    Some((resume, stack_field))
}

/// `RestoreStack`: `stloc temp(unbox.any T(ldfld <>t__stack(ldloc this)))`, the stack slot
/// stores, and `stfld <>t__stack(ldloc this, ldnull)`.
fn restore_stack(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    pos: &mut usize,
    stack_field: Option<Option<FieldDef>>,
) -> bool {
    let Some(stack_field) = stack_field else {
        return true;
    };
    let Some((v, value)) = at(f, block, *pos).and_then(|i| f.match_stloc(i)) else {
        return false;
    };
    if !f.var(v).is_single_definition() {
        return false;
    }
    let Inst::UnboxAny { arg, .. } = *f.inst(value) else {
        return false;
    };
    if !f
        .match_ldfld(ts, arg)
        .is_some_and(|(target, field)| f.match_ld_this(target) && def_of(field) == stack_field)
    {
        return false;
    }
    *pos += 1;
    while let Some((v, _)) = at(f, block, *pos).and_then(|i| f.match_stloc(i))
        && f.var(v).kind == VariableKind::StackSlot
    {
        *pos += 1;
    }
    if let Some((target, field, value)) = at(f, block, *pos).and_then(|i| f.match_stfld(i))
        && f.match_ld_this(target)
        && def_of(field) == stack_field
        && matches!(f.inst(value), Inst::LdNull)
    {
        *pos += 1;
    }
    true
}
