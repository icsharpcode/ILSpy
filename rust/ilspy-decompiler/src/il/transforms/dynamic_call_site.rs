//! `IL/Transforms/DynamicCallSiteTransform.cs`: the call site initialization pattern of the
//! C# runtime binder becomes a `DynamicInstruction`. The async/await decompiler runs it per
//! block (`RunOnBasicBlock`); the pipeline step comes later.

use std::collections::HashMap;

use crate::il::ast::{
    ComparisonKind, DynamicArgInfo, DynamicKind, FieldOperand, Function, Inst, NodeId, VarId,
    VariableKind,
};
use crate::il::types::{PrimitiveType, Sign, StackType};
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::FieldOrigin;
use crate::typesystem::types::{TypeId, TypeSystem};

use super::Context;

const CALL_SITE_TYPE_NAME: &str = "System.Runtime.CompilerServices.CallSite";
const CSHARP_BINDER_TYPE_NAME: &str = "Microsoft.CSharp.RuntimeBinder.Binder";
const ARGUMENT_INFO_TYPE_NAME: &str = "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo";
/// `CSharpArgumentInfoFlags.IsStaticType`.
const IS_STATIC_TYPE: i32 = 0x20;
/// `CSharpBinderFlags.CheckedContext`.
const CHECKED_CONTEXT: i32 = 1;

/// `IField` equality of the cache fields, by definition.
type FieldKey = (crate::typesystem::compilation::ModuleId, u32);

fn field_key(field: &FieldOperand) -> Option<FieldKey> {
    match field.field.origin {
        FieldOrigin::Def { module, row } => Some((module, row)),
        FieldOrigin::Fake => None,
    }
}

#[derive(Clone, Debug)]
struct CallSiteInfo {
    inverted: bool,
    branch_after_init: NodeId,
    conditional_jump_to_init: NodeId,
    init_block: NodeId,
    delegate_type: TypeId,
    kind: DynamicKind,
    flags: i32,
    operation: i32,
    context: Option<TypeId>,
    convert_target_type: Option<TypeId>,
    type_arguments: Option<Vec<TypeId>>,
    argument_infos: Vec<DynamicArgInfo>,
    member_name: Option<String>,
}

/// `RunOnBasicBlock`: the call sites initialized in `block`, transformed in its container.
/// The now-unreachable init blocks stay for the caller's `SortBlocks`.
pub fn run_on_basic_block(f: &mut Function, ctx: &mut Context<'_, '_, '_>, block: NodeId) {
    if !ctx.settings.dynamic {
        return;
    }
    let mut callsites = HashMap::new();
    find_dynamic_call_sites_in_block(f, ctx.ts, block, &mut callsites);
    let container = f.node(block).parent.expect("block in a container");
    transform_call_sites(f, ctx.ts, container, &callsites);
}

fn transform_call_sites(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    parent: NodeId,
    callsites: &HashMap<FieldKey, CallSiteInfo>,
) -> Vec<NodeId> {
    let mut stores_to_remove = Vec::new();
    let mut modified_containers = Vec::new();
    for invoke in f.descendants_post(parent) {
        let Inst::CallVirt(call) = f.inst(invoke) else {
            continue;
        };
        let is_delegate = call
            .method
            .method
            .declaring
            .and_then(|d| ts.definition(d))
            .and_then(|d| ts.comp.type_def(d).ok())
            .is_some_and(|i| i.kind == TypeKind::Delegate);
        if !is_delegate || call.method.method.name != "Invoke" || call.args.is_empty() {
            continue;
        }
        let mut first_argument = call.args[0];
        if let Some(slot) = f.match_ldloc(first_argument)
            && is_single_definition_temporary(f, slot)
            && let Some((_, value)) = f.match_stloc(f.var(slot).store_instructions[0])
        {
            first_argument = value;
        }
        let Some((cache_field_load, target_field)) = f.match_ldfld(ts, first_argument) else {
            continue;
        };
        let target_field = field_key(target_field);
        let Some(cache_field) = f.match_ldsfld(cache_field_load).and_then(field_key) else {
            continue;
        };
        let Some(callsite) = callsites.get(&cache_field) else {
            continue;
        };
        let mut dead_arguments = Vec::new();
        let Some(replacement) =
            make_dynamic_instruction(f, ts, callsite, invoke, &mut dead_arguments)
        else {
            continue;
        };
        f.replace(invoke, replacement);
        let jump = callsite.conditional_jump_to_init;
        let block = f.node(jump).parent.expect("jump in a block");
        f.remove(jump);
        if callsite.inverted {
            let Inst::If { true_inst, .. } = *f.inst(jump) else {
                unreachable!("if");
            };
            f.replace(callsite.branch_after_init, true_inst);
        }
        for arg in dead_arguments {
            let Some(temporary) = f.match_ldloc(arg) else {
                continue;
            };
            if !is_single_definition_temporary(f, temporary) || f.var(temporary).load_count() != 0 {
                continue;
            }
            let store = f.var(temporary).store_instructions[0];
            let Some((_, value)) = f.match_stloc(store) else {
                continue;
            };
            if !f
                .node(store)
                .parent
                .is_some_and(|p| matches!(f.inst(p), Inst::Block { .. }))
            {
                continue;
            }
            if f.match_ldsfld(value).and_then(field_key) == Some(cache_field) {
                stores_to_remove.push(store);
            }
            if let Some((load, field)) = f.match_ldfld(ts, value)
                && field_key(field) == target_field
                && f.match_ldsfld(load).and_then(field_key) == Some(cache_field)
            {
                stores_to_remove.push(store);
            }
            if match_get_type_from_handle(f, ts, value).is_some() {
                stores_to_remove.push(store);
            }
        }
        let container = f.node(block).parent.expect("block in a container");
        if !modified_containers.contains(&container) {
            modified_containers.push(container);
        }
    }
    for store in stores_to_remove {
        let Some(block) = f.node(store).parent else {
            continue;
        };
        let index = f.node(store).child_index as usize;
        if f.list(block).get(index) == Some(&store) {
            f.remove_at(block, index);
        }
    }
    modified_containers
}

/// A compiler-generated temporary: a single-definition stack slot, or a single-definition
/// local spilled from a state machine field (an await in the invoke's arguments).
fn is_single_definition_temporary(f: &Function, v: VarId) -> bool {
    let var = f.var(v);
    var.is_single_definition()
        && (var.kind == VariableKind::StackSlot
            || (var.kind == VariableKind::Local && var.state_machine_field.is_some()))
}

/// `TransformExpressionTrees.MatchGetTypeFromHandle`: `call GetTypeFromHandle(ldtoken T)`.
pub fn match_get_type_from_handle(
    f: &Function,
    ts: &TypeSystem<'_, '_>,
    inst: NodeId,
) -> Option<TypeId> {
    let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = f.inst(inst) else {
        return None;
    };
    let is_get_type = c.method.method.name == "GetTypeFromHandle"
        && c.method
            .method
            .declaring
            .is_some_and(|d| ts.full_name(d) == "System.Type");
    if !is_get_type || c.args.len() != 1 {
        return None;
    }
    match *f.inst(c.args[0]) {
        Inst::LdTypeToken { ty } => Some(ty.ty),
        _ => None,
    }
}

/// `MatchTypeOf`: `GetTypeFromHandle`, or a variable assigned one exactly once.
fn match_type_of(f: &Function, ts: &TypeSystem<'_, '_>, inst: NodeId) -> Option<TypeId> {
    if let Some(t) = match_get_type_from_handle(f, ts, inst) {
        return Some(t);
    }
    let v = f.match_ldloc(inst)?;
    if !f.var(v).is_single_definition() {
        return None;
    }
    let (_, value) = f.match_stloc(*f.var(v).store_instructions.first()?)?;
    match_get_type_from_handle(f, ts, value)
}

fn find_dynamic_call_sites_in_block(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    callsites: &mut HashMap<FieldKey, CallSiteInfo>,
) {
    let list = f.list(block);
    if list.len() < 2 {
        return;
    }
    // if (comp(ldsfld <>p__3 == ldnull)) br IL_000c; br IL_002b
    let if_inst = list[list.len() - 2];
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return;
    };
    let branch_after_init = list[list.len() - 1];
    let Some(after_target) = f.match_branch(branch_after_init) else {
        return;
    };
    let Some((cache_field, delegate, invert)) = match_call_site_cache_null_check(f, ts, condition)
    else {
        return;
    };
    let Some(true_block) = f.match_branch(true_inst) else {
        return;
    };
    let (init_block, target_after_init) = if invert {
        (after_target, true_block)
    } else {
        (true_block, after_target)
    };
    let Some((mut info, block_after_init)) =
        scan_call_site_init_block(f, ts, init_block, cache_field, delegate)
    else {
        return;
    };
    if target_after_init != block_after_init {
        return;
    }
    info.conditional_jump_to_init = if_inst;
    info.inverted = invert;
    info.branch_after_init = branch_after_init;
    // shortcut: a second init of the same cache field throws in the managed Dictionary.Add; the first wins here
    callsites.entry(cache_field).or_insert(info);
}

fn match_call_site_cache_null_check(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    condition: NodeId,
) -> Option<(FieldKey, TypeId, bool)> {
    let (argument, invert) = match f.match_comp_null(condition, ComparisonKind::Equality) {
        Some(a) => (a, false),
        None => (
            f.match_comp_null(condition, ComparisonKind::Inequality)?,
            true,
        ),
    };
    let field = f.match_ldsfld(argument)?;
    let ty = field.field.ty;
    let args = ts.type_arguments(ty);
    if args.len() != 1 || ts.full_name(ty) != CALL_SITE_TYPE_NAME {
        return None;
    }
    let delegate = args[0];
    let is_delegate = ts
        .definition(delegate)
        .and_then(|d| ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == TypeKind::Delegate);
    if !is_delegate {
        return None;
    }
    Some((field_key(field)?, delegate, invert))
}

fn scan_call_site_init_block(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    init_block: NodeId,
    cache_field: FieldKey,
    delegate: TypeId,
) -> Option<(CallSiteInfo, NodeId)> {
    let list = f.list(init_block).to_vec();
    let n = list.len();
    if f.node(init_block).incoming_edge_count != 1 || n < 2 {
        return None;
    }
    let block_after_init = f.match_branch(list[n - 1])?;
    // stsfld cacheField(call CallSite<T>.Create(call Binder.X(...)))
    let Inst::StObj {
        target,
        value,
        is_volatile: false,
        unaligned: 0,
        ..
    } = *f.inst(list[n - 2])
    else {
        return None;
    };
    let Inst::LdsFlda { field } = f.inst(target) else {
        return None;
    };
    if field_key(field) != Some(cache_field) {
        return None;
    }
    let Inst::Call(create) = f.inst(value) else {
        return None;
    };
    let create_declaring = create.method.method.declaring?;
    if !create.method.method.type_args.is_empty()
        || create.args.len() != 1
        || create.method.method.name != "Create"
        || ts.full_name(create_declaring) != CALL_SITE_TYPE_NAME
        || ts.type_arguments(create_declaring).len() != 1
    {
        return None;
    }
    let Inst::Call(binder) = f.inst(create.args[0]) else {
        return None;
    };
    let binder_declaring = binder.method.method.declaring?;
    if ts.full_name(binder_declaring) != CSHARP_BINDER_TYPE_NAME
        || ts.type_parameter_count(binder_declaring) != 0
    {
        return None;
    }
    let mut info = CallSiteInfo {
        inverted: false,
        branch_after_init: init_block,
        conditional_jump_to_init: init_block,
        init_block,
        delegate_type: delegate,
        kind: DynamicKind::Invoke,
        flags: 0,
        operation: 0,
        context: None,
        convert_target_type: None,
        type_arguments: None,
        argument_infos: Vec::new(),
        member_name: None,
    };
    let args = binder.args.clone();
    // The argument `index` of the binder call is a local stored by init block instruction
    // `pos`: its value.
    let stored = |index: usize, pos: usize| -> Option<(VarId, NodeId)> {
        let v = f.match_ldloc(*args.get(index)?)?;
        let (sv, value) = f.match_stloc(*list.get(pos)?)?;
        (sv == v).then_some((v, value))
    };
    let ldstr = |inst: NodeId| match f.inst(inst) {
        Inst::LdStr { value } => Some(String::from_utf16_lossy(value)),
        _ => None,
    };
    match binder.method.method.name.as_str() {
        "IsEvent" => {
            info.kind = DynamicKind::IsEvent;
            // No array initializer: the arguments are inlined.
            if args.len() != 3 {
                return None;
            }
            info.flags = f.match_ldc_i4(args[0])?;
            info.member_name = Some(ldstr(args[1])?);
            info.context = Some(match_get_type_from_handle(f, ts, args[2])?);
            Some((info, block_after_init))
        }
        "Convert" => {
            info.kind = DynamicKind::Convert;
            if args.len() != 3 {
                return None;
            }
            info.flags = f.match_ldc_i4(args[0])?;
            info.convert_target_type = Some(match_get_type_from_handle(f, ts, args[1])?);
            info.context = Some(match_get_type_from_handle(f, ts, args[2])?);
            Some((info, block_after_init))
        }
        "InvokeMember" => {
            info.kind = DynamicKind::InvokeMember;
            if args.len() != 5 {
                return None;
            }
            info.flags = f.match_ldc_i4(stored(0, 0)?.1)?;
            info.member_name = Some(ldstr(stored(1, 1)?.1)?);
            // The type arguments: ldnull or an array initializer.
            let variable = f.match_ldloc(args[2])?;
            let (variable_or_temporary, value) = f.match_stloc(list[2])?;
            let mut number_of_type_arguments = 0;
            if !matches!(f.inst(value), Inst::LdNull) {
                let Inst::NewArr { ty, ref indices } = *f.inst(value) else {
                    return None;
                };
                let is_type_array = ts
                    .definition(ty.ty)
                    .and_then(|d| ts.comp.type_def(d).ok())
                    .is_some_and(|i| i.known == KnownTypeCode::Type);
                if !is_type_array || indices.len() != 1 {
                    return None;
                }
                number_of_type_arguments = f.match_ldc_i4(indices[0])?;
                let values = handle_simple_array_initializer(
                    f,
                    init_block,
                    3,
                    variable_or_temporary,
                    number_of_type_arguments,
                )?;
                let mut type_arguments = Vec::with_capacity(values.len());
                for v in values {
                    type_arguments.push(match_get_type_from_handle(f, ts, v?)?);
                }
                info.type_arguments = Some(type_arguments);
            }
            let mut offset = number_of_type_arguments as usize;
            // csc stores the array initializer's temporary in the variable.
            if variable_or_temporary != variable {
                let (v, value) = f.match_stloc(*list.get(3 + offset)?)?;
                if v != variable || f.match_ldloc(value) != Some(variable_or_temporary) {
                    return None;
                }
                offset += 1;
            }
            info.context = Some(match_get_type_from_handle(f, ts, stored(3, 3 + offset)?.1)?);
            let (variable, value) = stored(4, 4 + offset)?;
            extract_argument_info(f, ts, value, &mut info, 5 + offset, variable)?;
            Some((info, block_after_init))
        }
        "GetMember" | "SetMember" => {
            info.kind = if binder.method.method.name == "GetMember" {
                DynamicKind::GetMember
            } else {
                DynamicKind::SetMember
            };
            if args.len() != 4 {
                return None;
            }
            info.flags = f.match_ldc_i4(stored(0, 0)?.1)?;
            info.member_name = Some(ldstr(stored(1, 1)?.1)?);
            info.context = Some(match_get_type_from_handle(f, ts, stored(2, 2)?.1)?);
            let (variable, value) = stored(3, 3)?;
            extract_argument_info(f, ts, value, &mut info, 4, variable)?;
            Some((info, block_after_init))
        }
        name @ ("GetIndex" | "SetIndex" | "InvokeConstructor" | "Invoke") => {
            info.kind = match name {
                "GetIndex" => DynamicKind::GetIndex,
                "SetIndex" => DynamicKind::SetIndex,
                "InvokeConstructor" => DynamicKind::InvokeConstructor,
                _ => DynamicKind::Invoke,
            };
            if args.len() != 3 {
                return None;
            }
            info.flags = f.match_ldc_i4(stored(0, 0)?.1)?;
            info.context = Some(match_get_type_from_handle(f, ts, stored(1, 1)?.1)?);
            let (variable, value) = stored(2, 2)?;
            extract_argument_info(f, ts, value, &mut info, 3, variable)?;
            Some((info, block_after_init))
        }
        name @ ("UnaryOperation" | "BinaryOperation") => {
            info.kind = if name == "BinaryOperation" {
                DynamicKind::BinaryOperator
            } else {
                DynamicKind::UnaryOperator
            };
            if args.len() != 4 {
                return None;
            }
            info.flags = f.match_ldc_i4(stored(0, 0)?.1)?;
            info.operation = f.match_ldc_i4(stored(1, 1)?.1)?;
            info.context = Some(match_get_type_from_handle(f, ts, stored(2, 2)?.1)?);
            let (variable, value) = stored(3, 3)?;
            extract_argument_info(f, ts, value, &mut info, 4, variable)?;
            Some((info, block_after_init))
        }
        _ => None,
    }
}

/// `ExtractArgumentInfo`: `new CSharpArgumentInfo[n] { CSharpArgumentInfo.Create(flags, name), ... }`.
fn extract_argument_info(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    value: NodeId,
    info: &mut CallSiteInfo,
    offset: usize,
    variable: VarId,
) -> Option<()> {
    let Inst::NewArr { ty, ref indices } = *f.inst(value) else {
        return None;
    };
    if ts.full_name(ty.ty) != ARGUMENT_INFO_TYPE_NAME || indices.len() != 1 {
        return None;
    }
    let count = f.match_ldc_i4(indices[0])?;
    let values = handle_simple_array_initializer(f, info.init_block, offset, variable, count)?;
    let invoke = ts.get_delegate_invoke_method(info.delegate_type).ok()??;
    let compile_time_types = invoke.params().ok()?;
    let mut infos = Vec::with_capacity(values.len());
    for (i, arg) in values.into_iter().enumerate() {
        let Inst::Call(create) = f.inst(arg?) else {
            return None;
        };
        let is_create = create.method.method.name == "Create"
            && create
                .method
                .method
                .declaring
                .is_some_and(|d| ts.full_name(d) == ARGUMENT_INFO_TYPE_NAME)
            && create.args.len() == 2;
        if !is_create {
            return None;
        }
        let flags = f.match_ldc_i4(create.args[0])?;
        let name = match f.inst(create.args[1]) {
            Inst::LdStr { value } => Some(String::from_utf16_lossy(value)),
            Inst::LdNull => None,
            _ => return None,
        };
        infos.push(DynamicArgInfo {
            flags,
            name,
            // shortcut: an argument without a delegate parameter throws in the managed code; it is no call site here
            compile_time_type: *compile_time_types.get(i + 1)?,
        });
    }
    info.argument_infos = infos;
    Some(())
}

/// `TransformArrayInitializers.HandleSimpleArrayInitializer` for a one-dimensional array of
/// `length` elements stored in `store`, from instruction `pos` of `block`: the element values,
/// `None` for the elements left default.
fn handle_simple_array_initializer(
    f: &Function,
    block: NodeId,
    pos: usize,
    store: VarId,
    length: i32,
) -> Option<Vec<Option<NodeId>>> {
    let list = f.list(block);
    let mut instructions_to_remove = 0;
    let mut element_count = 0;
    let mut values: Vec<Option<NodeId>> = Vec::new();
    let mut next_minimum_index = 0i32;
    let mut i = pos;
    while i < list.len() {
        // stobj T(ldelema T(ldloc store, index), value), or the address in a temporary first
        let (indices, value, step) = if let Inst::StObj { target, value, .. } = *f.inst(list[i]) {
            match f.inst(target) {
                Inst::LdElema { array, indices, .. } if f.match_ldloc(*array) == Some(store) => {
                    (indices.clone(), value, 1)
                }
                _ => break,
            }
        } else if let Some((temporary, address)) = f.match_stloc(list[i]) {
            if !(f.var(temporary).is_single_definition() && f.var(temporary).load_count() == 1) {
                break;
            }
            let Inst::LdElema {
                array, ref indices, ..
            } = *f.inst(address)
            else {
                break;
            };
            if f.match_ldloc(array) != Some(store) || i + 1 >= list.len() {
                break;
            }
            let Inst::StObj { target, value, .. } = *f.inst(list[i + 1]) else {
                break;
            };
            if f.match_ldloc(target) != Some(temporary) {
                break;
            }
            (indices.clone(), value, 2)
        } else {
            break;
        };
        if f.descendants_post(value)
            .into_iter()
            .any(|n| f.variable_of(n) == Some(store))
        {
            break;
        }
        if indices.len() != 1 || length <= 0 {
            break;
        }
        loop {
            // `CalculateNextIndices`: an index in range that does not go back.
            let index = f.match_ldc_i4(indices[0])?;
            if index < 0 || index >= length || index < next_minimum_index {
                return None;
            }
            let exact = index == next_minimum_index;
            next_minimum_index += 1;
            if exact {
                values.push(Some(value));
                element_count += 1;
                instructions_to_remove += step;
            } else {
                values.push(None);
            }
            if exact || values.len() as i32 >= length {
                break;
            }
        }
        i += step;
    }
    if let Some(&next) = list.get(i)
        && let Inst::StObj { target, .. } = *f.inst(next)
        && let Inst::LdElema { array, .. } = *f.inst(target)
        && f.match_ldloc(array) == Some(store)
    {
        // An element is modified right after the initializer: no partial initializers.
        return None;
    }
    if pos + instructions_to_remove >= list.len() || element_count == 0 {
        return None;
    }
    // shortcut: the constructor-initializer case of `mustTransform` is left out; the async decompiler never runs this in a constructor
    let must_transform = is_catch_when_block(f, block);
    if element_count < length / 3 - 5 && (!must_transform || length == i32::MAX) {
        return None;
    }
    while (values.len() as i32) < length {
        values.push(None);
    }
    Some(values)
}

/// `ILInlining.IsCatchWhenBlock`: the block is in the filter of a catch handler.
fn is_catch_when_block(f: &Function, block: NodeId) -> bool {
    let Some(container) = std::iter::once(block)
        .chain(f.ancestors(block))
        .find(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
    else {
        return false;
    };
    f.node(container).parent.is_some_and(
        |p| matches!(*f.inst(p), Inst::TryCatchHandler { filter, .. } if filter == container),
    )
}

/// `MakeDynamicInstruction`: the instruction replacing the invoke, and the invoke's
/// arguments it drops.
fn make_dynamic_instruction(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    callsite: &CallSiteInfo,
    invoke: NodeId,
    dead_arguments: &mut Vec<NodeId>,
) -> Option<NodeId> {
    let Inst::CallVirt(call) = f.inst(invoke) else {
        unreachable!("callvirt");
    };
    let args = call.args.clone();
    let mut arguments: Vec<NodeId> = args.get(2..).unwrap_or_default().to_vec();
    let mut ty = callsite.convert_target_type;
    let mut name = callsite.member_name.clone();
    let mut type_arguments = None;
    let mut arg_infos = callsite.argument_infos.clone();
    match callsite.kind {
        DynamicKind::BinaryOperator | DynamicKind::SetMember => arguments.truncate(2),
        DynamicKind::UnaryOperator
        | DynamicKind::GetMember
        | DynamicKind::Convert
        | DynamicKind::IsEvent => {
            arguments.truncate(1);
        }
        _ => {}
    }
    match callsite.kind {
        DynamicKind::BinaryOperator | DynamicKind::SetMember => arg_infos.truncate(2),
        DynamicKind::UnaryOperator | DynamicKind::GetMember => arg_infos.truncate(1),
        DynamicKind::Convert | DynamicKind::IsEvent => arg_infos.clear(),
        _ => {}
    }
    match callsite.kind {
        DynamicKind::InvokeConstructor => {
            // ArgumentInfos[0] is the constructed type.
            ty = match_type_of(f, ts, *arguments.first()?);
            ty?;
            dead_arguments.extend(args.iter().take(3));
            arguments.remove(0);
        }
        DynamicKind::InvokeMember => {
            ty = None;
            if callsite
                .argument_infos
                .first()
                .is_some_and(|i| i.flags & IS_STATIC_TYPE != 0)
                && let Some(&first) = arguments.first()
                && let Some(t) = match_type_of(f, ts, first)
            {
                ty = Some(t);
                dead_arguments.push(first);
                arguments.remove(0);
            }
            dead_arguments.extend(args.iter().take(2));
            type_arguments = callsite.type_arguments.clone();
        }
        _ => dead_arguments.extend(args.iter().take(2)),
    }
    if !matches!(
        callsite.kind,
        DynamicKind::GetMember
            | DynamicKind::SetMember
            | DynamicKind::InvokeMember
            | DynamicKind::IsEvent
    ) {
        name = None;
    }
    if !matches!(
        callsite.kind,
        DynamicKind::Convert | DynamicKind::InvokeConstructor | DynamicKind::InvokeMember
    ) {
        ty = None;
    }
    let operation = if matches!(
        callsite.kind,
        DynamicKind::BinaryOperator | DynamicKind::UnaryOperator
    ) {
        callsite.operation
    } else {
        0
    };
    let dynamic = f.add(Inst::Dynamic {
        kind: callsite.kind,
        binder_flags: callsite.flags,
        context: callsite.context,
        name,
        operation,
        ty,
        type_arguments,
        arg_infos,
        args: arguments,
    });
    if callsite.kind == DynamicKind::Convert && f.result_type(ts, dynamic) == StackType::Unknown {
        // With missing references the primitive type is coerced to None.
        return Some(f.add(Inst::Conv {
            arg: dynamic,
            input: StackType::Unknown,
            input_sign: Sign::None,
            target: PrimitiveType::None,
            check_overflow: callsite.flags & CHECKED_CONTEXT != 0,
        }));
    }
    Some(dynamic)
}
