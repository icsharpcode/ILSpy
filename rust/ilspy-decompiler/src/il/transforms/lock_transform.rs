//! `IL/Transforms/LockTransform.cs`: the `Monitor.Enter`/`Monitor.Exit` patterns the C#
//! compilers emit for `lock` become a `lock` instruction.

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId, VarId};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::known;

/// `IBlockTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    if !ctx.settings.lock_statement {
        return;
    }
    let f = &mut r.function;
    let block = bc.block;
    let mut i = bc.index_of_first_already_transformed_instruction;
    while i > 0 {
        i -= 1;
        let changed = transform_lock_roslyn(f, ctx, block, i)
            || transform_lock_v4(f, ctx, block, i)
            || transform_lock_v4_yield_return(f, ctx, block, i)
            || transform_lock_v2(f, ctx, block, i)
            || transform_lock_mcs(f, ctx, block, i);
        let count = f.list(block).len();
        if changed {
            bc.index_of_first_already_transformed_instruction = count;
        }
        // The removed instructions before `i` moved the try-finally down.
        i = i.min(count);
    }
}

/// The parts of a `try { ... } finally { ... }` statement.
struct TryFinallyParts {
    try_finally: NodeId,
    try_container: NodeId,
    finally_container: NodeId,
}

fn try_finally_at(f: &Function, block: NodeId, i: usize) -> Option<TryFinallyParts> {
    let try_finally = f.list(block)[i];
    let Inst::TryFinally {
        try_block,
        finally_block,
    } = *f.inst(try_finally)
    else {
        return None;
    };
    Some(TryFinallyParts {
        try_finally,
        try_container: try_block,
        finally_container: finally_block,
    })
}

fn stloc_at(f: &Function, block: NodeId, i: usize) -> Option<(NodeId, VarId, NodeId)> {
    let inst = f.list(block)[i];
    f.match_stloc(inst).map(|(v, value)| (inst, v, value))
}

/// The entry point of `container`, if it is a block container.
fn entry_block(f: &Function, container: NodeId) -> Option<NodeId> {
    matches!(f.inst(container), Inst::BlockContainer { .. }).then(|| f.list(container)[0])
}

/// A `stloc flag(ldc.i4 0)` of a boolean variable.
fn is_flag_store(f: &Function, ctx: &mut Context<'_, '_, '_>, flag: VarId, value: NodeId) -> bool {
    known(ctx, f.var(flag).ty) == Some(KnownTypeCode::Boolean) && f.match_ldc_i4(value) == Some(0)
}

/// `call System.Threading.Monitor.<name>` with `arg_count` arguments and no type arguments;
/// returns the arguments.
fn monitor_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    call: NodeId,
    name: &str,
    arg_count: usize,
) -> Option<Vec<NodeId>> {
    let Inst::Call(c) = f.inst(call) else {
        return None;
    };
    let m = &c.method.method;
    let monitor = m
        .declaring
        .is_some_and(|d| ctx.ts.full_name(d) == "System.Threading.Monitor");
    (m.name == name && monitor && m.type_args.is_empty() && c.args.len() == arg_count)
        .then(|| c.args.clone())
}

/// `MatchCall(call, name, obj)` and `MatchCall(call, name, obj, flag)`: `ldloc obj`, then
/// `ldloca flag` when there is a flag.
fn match_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    call: NodeId,
    name: &str,
    obj: VarId,
    flag: Option<VarId>,
) -> bool {
    let Some(args) = monitor_call(f, ctx, call, name, 1 + usize::from(flag.is_some())) else {
        return false;
    };
    f.match_ldloc(args[0]) == Some(obj)
        && flag.is_none_or(|flag| matches!(*f.inst(args[1]), Inst::LdLoca { var } if var == flag))
}

/// `MatchCall(call, name, flag, out obj)`: `call Enter(stloc obj(...), ldloca flag)`.
fn match_call_with_store(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    call: NodeId,
    name: &str,
    flag: VarId,
) -> Option<NodeId> {
    let args = monitor_call(f, ctx, call, name, 2)?;
    (matches!(*f.inst(args[1]), Inst::LdLoca { var } if var == flag)
        && matches!(f.inst(args[0]), Inst::StLoc { .. }))
    .then_some(args[0])
}

/// `MatchExitBlock(entryPoint, flag, obj)`: the finally block calls `Exit(obj)`, under
/// `if (flag)` when there is a flag, and leaves its container.
fn match_exit_block(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
    flag: Option<VarId>,
    obj: VarId,
) -> bool {
    let Some(entry) = entry_block(f, container) else {
        return false;
    };
    let instructions = f.list(entry);
    if instructions.len() != 2 || f.node(entry).incoming_edge_count != 1 {
        return false;
    }
    if let Some(flag) = flag {
        let Some((cond, true_inst)) = f.match_if_instruction(instructions[0]) else {
            return false;
        };
        if !matches!(f.inst(true_inst), Inst::Block { .. }) {
            return false;
        }
        let tests_flag = f.match_ldloc(cond) == Some(flag)
            || f.match_comp(cond, ComparisonKind::Inequality)
                .is_some_and(|(l, r)| {
                    f.match_ldloc(l) == Some(flag) && f.match_ldc_i4(r) == Some(0)
                });
        let true_instructions = f.list(true_inst);
        if !tests_flag
            || true_instructions.len() != 1
            || !match_call(f, ctx, true_instructions[0], "Exit", obj, None)
        {
            return false;
        }
    } else if !match_call(f, ctx, instructions[0], "Exit", obj, None) {
        return false;
    }
    f.match_leave(instructions[1])
        .is_some_and(|(target, value)| target == container && f.match_nop(value))
}

/// The entry block of the try container, when it has instructions and one incoming edge.
fn lock_entry_point(f: &Function, try_container: NodeId) -> Option<NodeId> {
    entry_block(f, try_container)
        .filter(|&e| !f.list(e).is_empty() && f.node(e).incoming_edge_count == 1)
}

/// Removes the given instructions (in descending index order) and replaces the try-finally
/// with `lock (value) try_block`.
fn replace_with_lock(
    f: &mut Function,
    removals: &[(NodeId, usize)],
    parts: &TryFinallyParts,
    object_store: NodeId,
) {
    for &(owner, index) in removals {
        f.remove_at(owner, index);
    }
    let (_, value) = f.match_stloc(object_store).expect("a store");
    let lock = f.add(Inst::Lock {
        on_expression: value,
        body: parts.try_container,
    });
    f.add_il_range(lock, f.il_range(object_store));
    f.replace(parts.try_finally, lock);
}

/// `TransformLockMCS`: `stloc obj(expr); call Enter(ldloc obj); try { ... } finally {
/// call Exit(ldloc obj) }`.
fn transform_lock_mcs(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if i < 2 {
        return false;
    }
    let Some(parts) = try_finally_at(f, block, i) else {
        return false;
    };
    let Some((object_store, obj, _)) = stloc_at(f, block, i - 2) else {
        return false;
    };
    if !match_call(f, ctx, f.list(block)[i - 1], "Enter", obj, None)
        || !f.var(obj).is_single_definition()
    {
        return false;
    }
    if lock_entry_point(f, parts.try_container).is_none()
        || !match_exit_block(f, ctx, parts.finally_container, None, obj)
        || f.var(obj).load_count() > 2
    {
        return false;
    }
    replace_with_lock(f, &[(block, i - 1), (block, i - 2)], &parts, object_store);
    true
}

/// `TransformLockV2`: `stloc obj(ldloc temp); call Enter(ldloc temp); try { ... } finally {
/// call Exit(ldloc obj) }`.
fn transform_lock_v2(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if i < 2 {
        return false;
    }
    let Some(parts) = try_finally_at(f, block, i) else {
        return false;
    };
    let Some((object_store, obj, value)) = stloc_at(f, block, i - 2) else {
        return false;
    };
    let Some(temp) = f.match_ldloc(value) else {
        return false;
    };
    if !match_call(f, ctx, f.list(block)[i - 1], "Enter", temp, None)
        || !f.var(obj).is_single_definition()
    {
        return false;
    }
    if lock_entry_point(f, parts.try_container).is_none()
        || !match_exit_block(f, ctx, parts.finally_container, None, obj)
        || f.var(obj).load_count() > 1
    {
        return false;
    }
    replace_with_lock(f, &[(block, i - 1), (block, i - 2)], &parts, object_store);
    true
}

/// `TransformLockV4`: `stloc flag(ldc.i4 0); try { call Enter(stloc obj(expr), ldloca flag);
/// ... } finally { if (flag) { call Exit(ldloc obj) } }`.
fn transform_lock_v4(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if i < 1 {
        return false;
    }
    let Some(parts) = try_finally_at(f, block, i) else {
        return false;
    };
    let Some((_, flag, flag_value)) = stloc_at(f, block, i - 1) else {
        return false;
    };
    if !is_flag_store(f, ctx, flag, flag_value) {
        return false;
    }
    let Some(entry) = lock_entry_point(f, parts.try_container) else {
        return false;
    };
    let Some(object_store) = match_call_with_store(f, ctx, f.list(entry)[0], "Enter", flag) else {
        return false;
    };
    let (obj, _) = f.match_stloc(object_store).expect("a store");
    if !match_exit_block(f, ctx, parts.finally_container, Some(flag), obj)
        || f.var(obj).load_count() > 1
    {
        return false;
    }
    replace_with_lock(f, &[(block, i - 1), (entry, 0)], &parts, object_store);
    true
}

/// `TransformLockV4YieldReturn`: as `TransformLockV4`, with `stloc exit(stloc obj(expr));
/// call Enter(ldloc obj, ldloca flag)` at the start of the try block (legacy compiler
/// iterators).
fn transform_lock_v4_yield_return(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if i < 1 {
        return false;
    }
    let Some(parts) = try_finally_at(f, block, i) else {
        return false;
    };
    let Some((_, flag, flag_value)) = stloc_at(f, block, i - 1) else {
        return false;
    };
    if !is_flag_store(f, ctx, flag, flag_value) {
        return false;
    }
    let Some(entry) = lock_entry_point(f, parts.try_container) else {
        return false;
    };
    let Some((exit_variable, object_store)) = f.match_stloc(f.list(entry)[0]) else {
        return false;
    };
    let Some((obj, _)) = f.match_stloc(object_store) else {
        return false;
    };
    // The managed matcher reads the second instruction without a bounds check.
    let enter = *f
        .list(entry)
        .get(1)
        .expect("an Enter call after the stores");
    if !match_call(f, ctx, enter, "Enter", obj, Some(flag))
        || !match_exit_block(f, ctx, parts.finally_container, Some(flag), exit_variable)
        || f.var(obj).load_count() > 1
    {
        return false;
    }
    replace_with_lock(
        f,
        &[(block, i - 1), (entry, 1), (entry, 0)],
        &parts,
        object_store,
    );
    true
}

/// `TransformLockRoslyn`: `stloc obj(expr); stloc flag(ldc.i4 0); try { call Enter(ldloc
/// obj, ldloca flag); ... } finally { if (flag) { call Exit(ldloc obj) } }`.
fn transform_lock_roslyn(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if i < 2 {
        return false;
    }
    let Some(parts) = try_finally_at(f, block, i) else {
        return false;
    };
    let Some((_, flag, flag_value)) = stloc_at(f, block, i - 1) else {
        return false;
    };
    let Some((object_store, obj, _)) = stloc_at(f, block, i - 2) else {
        return false;
    };
    if !f.var(obj).is_single_definition() || !is_flag_store(f, ctx, flag, flag_value) {
        return false;
    }
    let Some(entry) = lock_entry_point(f, parts.try_container) else {
        return false;
    };
    if !match_call(f, ctx, f.list(entry)[0], "Enter", obj, Some(flag))
        || !match_exit_block(f, ctx, parts.finally_container, Some(flag), obj)
        || f.var(obj).load_count() > 2
    {
        return false;
    }
    replace_with_lock(
        f,
        &[(block, i - 1), (block, i - 2), (entry, 0)],
        &parts,
        object_store,
    );
    true
}
