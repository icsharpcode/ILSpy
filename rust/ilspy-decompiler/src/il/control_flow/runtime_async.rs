//! The runtime-async path of `AsyncAwaitDecompiler` (methods the runtime runs as async, with
//! no state machine): `TranslateThisCopyAccess`, `IL/ControlFlow/RuntimeAsyncManualAwaitTransform.cs`
//! and `IL/ControlFlow/RuntimeAsyncExceptionRewriteTransform.cs`.

use std::collections::{HashMap, HashSet, VecDeque};

use crate::il::ast::{
    AwaiterMethod, BranchTarget, ComparisonKind, Flags, Function, Inst, NodeId, NopKind, VarId,
    VariableKind,
};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeSystem};
use crate::util::interval::Interval;

use super::await_in_catch_transform::{default_section_body, match_exception_capture_block};
use super::await_in_finally_transform::{
    match_object_store_catch_handler, move_dominated_blocks_to_container,
};
use super::control_flow_graph::ControlFlowGraph;
use super::yield_return_decompiler::{is_known, new_container};

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    translate_this_copy_access(r, ctx);
    manual_await(r, ctx);
    rewrite_exceptions(r, ctx);
}

fn blocks_under(f: &Function, root: NodeId) -> Vec<NodeId> {
    f.descendants_post(root)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect()
}

fn sort_all_containers(f: &mut Function) {
    let body = f.body.expect("body");
    let containers: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
        .collect();
    for c in containers {
        sort_blocks(f, c);
    }
}

fn leave(f: &mut Function, target: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Leave {
        target: Some(target),
        value: nop,
    })
}

fn branch(f: &mut Function, target: NodeId) -> NodeId {
    f.add(Inst::Branch {
        target: BranchTarget::Block(target),
    })
}

/// `IsDescendantOf`, inclusive.
fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    inst == ancestor || f.ancestors(inst).any(|a| a == ancestor)
}

/// `Block.Instructions.Clear()` then `Add`: the old items disconnect before the new ones
/// connect.
fn clear_and_set(f: &mut Function, block: NodeId, items: &[NodeId]) {
    f.set_list(block, Vec::new());
    for &i in items {
        f.push(block, i);
    }
}

/// `TranslateThisCopyAccess`: in a struct method, Roslyn copies `*this` into a local at the
/// entry; its uses go through the `this` parameter again.
fn translate_this_copy_access(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) -> bool {
    let ts = &mut *ctx.ts;
    let comp = ts.comp;
    let md = &comp.module(r.method.module).md;
    let Ok(declaring) = md.method_declaring_type(r.method.row) else {
        return false;
    };
    let is_struct = declaring != 0
        && comp
            .type_def(crate::typesystem::compilation::TypeDefId {
                module: r.method.module,
                row: declaring,
            })
            .is_ok_and(|i| i.kind == TypeKind::Struct);
    if !is_struct {
        return false;
    }
    let f = &mut r.function;
    let Some(this_param) = r
        .variables
        .iter()
        .copied()
        .find(|&v| f.var(v).kind == VariableKind::Parameter && f.var(v).index == Some(-1))
    else {
        return false;
    };
    let TypeData::ByRef(this_elem) = *ts.data(f.var(this_param).ty) else {
        return false;
    };
    let body = f.body.expect("body");
    let Some(&entry) = f.list(body).first() else {
        return false;
    };
    let Some(&first) = f.list(entry).first() else {
        return false;
    };
    let Some((copy_var, copy_value)) = f.match_stloc(first) else {
        return false;
    };
    if f.var(copy_var).kind != VariableKind::Local || f.var(copy_var).store_count() != 1 {
        return false;
    }
    let Inst::LdObj { target, ty, .. } = *f.inst(copy_value) else {
        return false;
    };
    if f.match_ldloc(target) != Some(this_param) {
        return false;
    }
    if f.var(copy_var).ty != ty.ty || this_elem != ty.ty {
        return false;
    }
    for ldloc in f.descendants_post(body) {
        if !matches!(*f.inst(ldloc), Inst::LdLoc { var } if var == copy_var) {
            continue;
        }
        let this_load = f.add(Inst::LdLoc { var: this_param });
        let ldobj = f.add(Inst::LdObj {
            target: this_load,
            ty,
            is_volatile: false,
            unaligned: 0,
        });
        f.add_il_range(ldobj, f.il_range(ldloc));
        f.replace(ldloc, ldobj);
    }
    for ldloca in f.descendants_post(body) {
        if !matches!(*f.inst(ldloca), Inst::LdLoca { var } if var == copy_var) {
            continue;
        }
        let this_load = f.add(Inst::LdLoc { var: this_param });
        f.add_il_range(this_load, f.il_range(ldloca));
        f.replace(ldloca, this_load);
    }
    f.remove_at(entry, 0);
    true
}

fn is_async_helpers_method(
    ts: &TypeSystem<'_, '_>,
    m: &crate::il::ast::MethodOperand,
    name: &str,
) -> bool {
    m.method.name == name
        && m.method
            .declaring
            .is_some_and(|d| ts.full_name(d) == "System.Runtime.CompilerServices.AsyncHelpers")
}

/// `RuntimeAsyncManualAwaitTransform`: GetAwaiter / get_IsCompleted /
/// AsyncHelpers.UnsafeAwaitAwaiter / GetResult across three blocks become one `await`.
fn manual_await(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let body = r.function.body.expect("body");
    let mut changed = false;
    for block in blocks_under(&r.function, body) {
        if detect_runtime_async_manual_await(&mut r.function, ctx.ts, block) {
            changed = true;
        }
    }
    if changed {
        sort_all_containers(&mut r.function);
    }
}

/// Block X {
///     [stloc awaitable(<value>);]   (when GetAwaiter takes an address)
///     stloc awaiter(call GetAwaiter(<expr>))
///     if (call get_IsCompleted(ldloca awaiter)) br completedBlock
///     br pauseBlock
/// }
/// pauseBlock { call AsyncHelpers.UnsafeAwaitAwaiter[<T>](ldloc awaiter); br completedBlock }
/// completedBlock { call GetResult(ldloca awaiter) (possibly inlined); ... }
fn detect_runtime_async_manual_await(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
) -> bool {
    let list = f.list(block).to_vec();
    let n = list.len();
    if n < 3 {
        return false;
    }
    let Some((awaiter_var, awaiter_value)) = f.match_stloc(list[n - 3]) else {
        return false;
    };
    let (Inst::Call(get_awaiter) | Inst::CallVirt(get_awaiter) | Inst::NewObj(get_awaiter)) =
        f.inst(awaiter_value)
    else {
        return false;
    };
    if get_awaiter.method.method.name != "GetAwaiter" || get_awaiter.args.len() != 1 {
        return false;
    }
    let get_awaiter_method = get_awaiter.method.clone();
    let get_awaiter_arg = get_awaiter.args[0];
    let get_awaiter_callvirt = matches!(f.inst(awaiter_value), Inst::CallVirt(_));
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(list[n - 2])
    else {
        return false;
    };
    let negated = f.match_logic_not(condition);
    let condition = negated.unwrap_or(condition);
    let (Inst::Call(is_completed) | Inst::CallVirt(is_completed) | Inst::NewObj(is_completed)) =
        f.inst(condition)
    else {
        return false;
    };
    if is_completed.method.method.name != "get_IsCompleted" || is_completed.args.len() != 1 {
        return false;
    }
    if !matches!(*f.inst(is_completed.args[0]), Inst::LdLoca { var } if var == awaiter_var) {
        return false;
    }
    let (Some(true_target), Some(false_target)) =
        (f.match_branch(true_inst), f.match_branch(list[n - 1]))
    else {
        return false;
    };
    let (completed_block, pause_block) = if negated.is_some() {
        (false_target, true_target)
    } else {
        (true_target, false_target)
    };
    // pauseBlock { call AsyncHelpers.UnsafeAwaitAwaiter(ldloc awaiter); br completedBlock }
    let [pause_call, pause_br] = *f.list(pause_block) else {
        return false;
    };
    let Inst::Call(pause) = f.inst(pause_call) else {
        return false;
    };
    if !is_async_helpers_method(ts, &pause.method, "UnsafeAwaitAwaiter") || pause.args.len() != 1 {
        return false;
    }
    if f.match_ldloc(pause.args[0]) != Some(awaiter_var) {
        return false;
    }
    if f.match_branch(pause_br) != Some(completed_block) {
        return false;
    }
    // completedBlock starts with GetResult(ldloca awaiter), possibly inlined.
    let Some(&completed_first) = f.list(completed_block).first() else {
        return false;
    };
    let Some(get_result) = f.find_first_inlined_call(completed_first) else {
        return false;
    };
    let (Inst::Call(gr) | Inst::CallVirt(gr) | Inst::NewObj(gr)) = f.inst(get_result) else {
        return false;
    };
    if gr.method.method.name != "GetResult" || gr.args.len() != 1 {
        return false;
    }
    if !matches!(*f.inst(gr.args[0]), Inst::LdLoca { var } if var == awaiter_var) {
        return false;
    }
    let get_result_method = gr.method.clone();

    // The awaitable: GetAwaiter's argument, or the value of a fresh temporary set just above.
    let mut remove_temporary_store = false;
    let mut awaitable = get_awaiter_arg;
    if let Inst::LdLoca { var: tmp } = *f.inst(get_awaiter_arg)
        && n >= 4
        && let Some((v, tmp_value)) = f.match_stloc(list[n - 4])
        && v == tmp
        && f.var(tmp).store_count() == 1
        && f.var(tmp).load_count() == 0
        && f.var(tmp).address_count() == 1
    {
        awaitable = tmp_value;
        remove_temporary_store = true;
    }

    // The await stands in for the whole pattern; the new branch replaces `br pauseBlock`.
    let old_tail_branch = list[n - 1];
    let copy = f.clone_subtree(awaitable);
    let await_inst = f.add(Inst::Await {
        value: copy,
        get_awaiter: Some(AwaiterMethod::Method(get_awaiter_method)),
        get_result: Some(AwaiterMethod::Method(get_result_method)),
        get_awaiter_callvirt,
    });
    f.add_il_range(await_inst, f.il_range(get_result));
    if remove_temporary_store {
        f.add_il_range(await_inst, f.il_range(list[n - 4]));
    }
    f.add_il_range(await_inst, f.il_range(list[n - 3]));
    f.add_il_range(await_inst, f.il_range(list[n - 2]));
    for inst in f.list(pause_block).to_vec() {
        f.add_il_range(await_inst, f.il_range(inst));
    }
    let remove_count = if remove_temporary_store { 4 } else { 3 };
    f.truncate(block, n - remove_count);
    let jump = branch(f, completed_block);
    f.add_il_range(jump, f.il_range(old_tail_branch));
    f.push(block, jump);
    f.replace(get_result, await_inst);
    true
}

/// `RuntimeAsyncExceptionRewriteTransform`: undoes the runtime-async lowering of try-catch
/// and try-finally with awaits.
fn rewrite_exceptions(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.await_in_catch_finally {
        return;
    }
    let f = &mut r.function;
    let ts = &mut *ctx.ts;
    let body = f.body.expect("body");
    let mut changed = false;

    // Catch filters wrapping a type test and an object store look like plain catches
    // afterwards. The captured object variable is shared between handlers, so it is only
    // recorded here and remapped per moved catch body.
    let mut filter_obj_by_handler: HashMap<NodeId, VarId> = HashMap::new();
    let handlers: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::TryCatchHandler { .. }))
        .collect();
    for handler in handlers {
        if normalize_runtime_async_filter(f, ts, handler, &mut filter_obj_by_handler) {
            changed = true;
        }
    }

    let try_catches: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::TryCatch { .. }))
        .collect();
    for try_catch in try_catches {
        let Some(parent_block) = f
            .node(try_catch)
            .parent
            .filter(|&p| matches!(f.inst(p), Inst::Block { .. }))
        else {
            continue;
        };
        let Some(container) = f
            .node(parent_block)
            .parent
            .filter(|&c| matches!(f.inst(c), Inst::BlockContainer { .. }))
        else {
            continue;
        };
        let Inst::TryCatch { ref handlers, .. } = *f.inst(try_catch) else {
            continue;
        };
        let handlers = handlers.clone();
        if let [handler] = handlers[..] {
            let Inst::TryCatchHandler {
                body: handler_body, ..
            } = *f.inst(handler)
            else {
                continue;
            };
            if !matches!(f.inst(handler_body), Inst::BlockContainer { .. }) {
                continue;
            }
            let [handler_block] = *f.list(handler_body) else {
                continue;
            };
            if try_rewrite_try_finally(f, ts, try_catch, handler, parent_block, container) {
                changed = true;
                continue;
            }
            if try_rewrite_try_catch(
                f,
                ts,
                try_catch,
                handler,
                handler_block,
                parent_block,
                container,
                &filter_obj_by_handler,
            ) {
                changed = true;
                continue;
            }
        } else if handlers.len() >= 2
            && try_rewrite_multi_handler_try_catch(
                f,
                ts,
                try_catch,
                &handlers,
                parent_block,
                container,
                &filter_obj_by_handler,
            )
        {
            changed = true;
            continue;
        }
    }

    // The early return from inside a try through a flag local.
    let try_finallys: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::TryFinally { .. }))
        .collect();
    for tf in try_finallys {
        if try_rewrite_flag_based_early_return(f, ts, tf) {
            changed = true;
        }
    }

    if changed {
        sort_all_containers(f);
    }
}

/// `NormalizeRuntimeAsyncFilter`: strips the captured-object prefix of a catch filter's true
/// body: `[stloc tmp2(ldloc tmpVar);] stloc obj(ldloc tmp_or_tmp2) [stloc typedEx(castclass T(ldloc obj));]`.
fn normalize_runtime_async_filter(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    handler: NodeId,
    filter_obj_by_handler: &mut HashMap<NodeId, VarId>,
) -> bool {
    let Inst::TryCatchHandler {
        filter, variable, ..
    } = *f.inst(handler)
    else {
        return false;
    };
    if !is_known(ts, f.var(variable).ty, KnownTypeCode::Object) {
        return false;
    }
    if !matches!(f.inst(filter), Inst::BlockContainer { .. }) {
        return false;
    }
    // stloc tmp(isinst T(ldloc handlerVar)); if (tmp != null) br trueBody; br falseBody
    let entry = f.list(filter)[0];
    let [store, if_inst, _] = *f.list(entry) else {
        return false;
    };
    let Some((tmp_var, tmp_value)) = f.match_stloc(store) else {
        return false;
    };
    let Inst::IsInst { arg, .. } = *f.inst(tmp_value) else {
        return false;
    };
    if f.match_ldloc(arg) != Some(variable) {
        return false;
    }
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return false;
    };
    if !f
        .match_comp_null(condition, ComparisonKind::Inequality)
        .is_some_and(|a| f.match_ldloc(a) == Some(tmp_var))
    {
        return false;
    }
    let Some(true_body) = f.match_branch(true_inst) else {
        return false;
    };
    let list = f.list(true_body).to_vec();
    let n = list.len();
    if n < 1 {
        return false;
    }
    let Some((first, first_value)) = f.match_stloc(list[0]) else {
        return false;
    };
    if f.match_ldloc(first_value) != Some(tmp_var) {
        return false;
    }
    let (tmp2_var, obj_var, mut prefix) = if n > 1
        && let Some((second, second_value)) = f.match_stloc(list[1])
        && f.match_ldloc(second_value) == Some(first)
    {
        (Some(first), second, 2)
    } else {
        (None, first, 1)
    };
    let mut typed_ex_var = None;
    if prefix < n
        && let Some((v, value)) = f.match_stloc(list[prefix])
        && let Inst::CastClass { arg, .. } = *f.inst(value)
        && f.match_ldloc(arg) == Some(obj_var)
    {
        typed_ex_var = Some(v);
        prefix += 1;
    }
    for _ in 0..prefix {
        f.remove_at(true_body, 0);
    }
    // tmp2 and typedEx are unique per handler: a function-wide remap is safe.
    let body = f.body.expect("body");
    if let Some(tmp2) = tmp2_var {
        remap_variable_reads(f, body, tmp2, variable);
    }
    if let Some(typed) = typed_ex_var {
        remap_variable_reads(f, body, typed, variable);
    }
    // Optimized builds inline `castclass T(ldloc obj)` into the filter: remap there only.
    remap_variable_reads(f, filter, obj_var, variable);
    filter_obj_by_handler.insert(handler, obj_var);
    true
}

/// `RemapVariableReads` / `ReplaceVariableReadsWithHandlerVariable`: loads of `from` under
/// `root` read `to` (dropping a cast to `to`'s type), and stores to `from` go.
fn remap_variable_reads(f: &mut Function, root: NodeId, from: VarId, to: VarId) {
    for ldloc in f.descendants_post(root) {
        if !matches!(*f.inst(ldloc), Inst::LdLoc { var } if var == from) {
            continue;
        }
        let parent = f.node(ldloc).parent.expect("load has a parent");
        let replaced = match *f.inst(parent) {
            Inst::CastClass { ty, .. } if ty.ty == f.var(to).ty => parent,
            _ => ldloc,
        };
        let load = f.add(Inst::LdLoc { var: to });
        f.add_il_range(load, f.il_range(replaced));
        if replaced != ldloc {
            f.add_il_range(load, f.il_range(ldloc));
        }
        f.replace(replaced, load);
    }
    for stloc in f.descendants_post(root) {
        if let Inst::StLoc { var, .. } = *f.inst(stloc)
            && var == from
            && let Some(parent) = f.node(stloc).parent
            && matches!(f.inst(parent), Inst::Block { .. })
        {
            let index = f.node(stloc).child_index as usize;
            f.remove_at(parent, index);
        }
    }
}

/// `TargetContainer` of a branch or leave.
fn target_container(f: &Function, inst: NodeId) -> Option<NodeId> {
    match *f.inst(inst) {
        Inst::Branch { .. } => f.node(f.branch_target(inst)).parent,
        Inst::Leave { target, .. } => target,
        _ => None,
    }
}

/// `FindFlagInitStore`: the flag-init store before the try region, matched by the variable's
/// slot, kind and type (SplitVariables may have split it from the handler's variable).
fn find_flag_init_store(
    f: &Function,
    parent_block: NodeId,
    try_catch: NodeId,
    reference: VarId,
    matches_init: impl Fn(&Function, NodeId) -> bool,
) -> Option<NodeId> {
    let try_index = f.node(try_catch).child_index as usize;
    let r = f.var(reference);
    f.list(parent_block)[..try_index]
        .iter()
        .copied()
        .find(|&s| {
            f.match_stloc(s).is_some_and(|(v, value)| {
                let v = f.var(v);
                v.index == r.index && v.kind == r.kind && v.ty == r.ty && matches_init(f, value)
            })
        })
}

/// `IsLeaveToContainerOrAncestor` / `LeaveExitsContainer`.
fn leave_exits_container(f: &Function, leave: NodeId, container: NodeId) -> bool {
    match *f.inst(leave) {
        Inst::Leave {
            target: Some(t), ..
        } => t == container || is_descendant_of(f, container, t),
        _ => false,
    }
}

fn is_leaving_function(f: &Function, inst: NodeId) -> bool {
    matches!(*f.inst(inst), Inst::Leave { target: Some(t), .. } if Some(t) == f.body)
}

/// `TryRewriteTryFinally`:
///   stloc obj(ldnull); .try { ...; br continuation }
///   catch ex : object when (ldc.i4 1) { [stloc tmp(ldloc ex);] stloc obj(ldloc tmp_or_ex); br continuation }
///   continuation { <finally body>; if (obj == null) leave outer; <dispatch> }
/// becomes a try-finally around the finally body.
fn try_rewrite_try_finally(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    try_catch: NodeId,
    handler: NodeId,
    parent_block: NodeId,
    container: NodeId,
) -> bool {
    let Inst::TryCatchHandler {
        body: handler_body,
        variable,
        ..
    } = *f.inst(handler)
    else {
        return false;
    };
    if !is_known(ts, f.var(variable).ty, KnownTypeCode::Object) {
        return false;
    }
    let Some((object_variable, continuation)) =
        match_object_store_catch_handler(f, handler_body, variable)
    else {
        return false;
    };
    if !is_known(ts, f.var(object_variable).ty, KnownTypeCode::Object) {
        return false;
    }
    if f.node(continuation).parent != Some(container) {
        return false;
    }
    let Some(flag_init_store) =
        find_flag_init_store(f, parent_block, try_catch, object_variable, |f, v| {
            matches!(f.inst(v), Inst::LdNull)
        })
    else {
        return false;
    };
    let Inst::TryCatch { try_block, .. } = *f.inst(try_catch) else {
        return false;
    };
    // Every exit of the try body branches to `continuation` (a try body that only throws has
    // none).
    for inst in f.descendants_post(try_block) {
        if !matches!(f.inst(inst), Inst::Branch { .. } | Inst::Leave { .. }) {
            continue;
        }
        let Some(target) = target_container(f, inst) else {
            continue;
        };
        if is_descendant_of(f, target, try_block) {
            continue;
        }
        if matches!(f.inst(inst), Inst::Branch { .. }) && f.branch_target(inst) == continuation {
            continue;
        }
        return false;
    }
    let Some((finally_exit_block, after_finally_exit)) =
        find_finally_dispatch_exit(f, continuation, object_variable, container)
    else {
        return false;
    };
    // A branch to a one-instruction leave block keeps that block after the try-finally.
    let leave_block = if matches!(f.inst(after_finally_exit), Inst::Branch { .. }) {
        Some(f.branch_target(after_finally_exit))
    } else {
        None
    };

    // The dominator analysis uses the graph from before the structural changes.
    let cfg = ControlFlowGraph::new(f, container);
    let finally_container = new_container(f);
    f.add_il_range(finally_container, f.il_range(handler_body));
    let try_finally = f.add(Inst::TryFinally {
        try_block,
        finally_block: finally_container,
    });
    f.add_il_range(try_finally, f.il_range(try_catch));
    f.replace(try_catch, try_finally);

    // The finally body: the blocks `continuation` dominates, up to the finally exit block.
    move_dominated_blocks_to_container(
        f,
        continuation,
        Some(finally_exit_block),
        &cfg,
        finally_container,
    );
    rewrite_finally_exit(f, finally_exit_block, finally_container);

    // Normal completion of the try block runs the finally and resumes after it.
    for br in f.descendants_post(try_block) {
        if matches!(f.inst(br), Inst::Branch { .. }) && f.branch_target(br) == continuation {
            let l = leave(f, try_block);
            f.add_il_range(l, f.il_range(br));
            f.replace(br, l);
        }
    }
    // A successor keeps the parent block's end point unreachable, unless the try body always
    // throws.
    if !f.has_flag(try_finally, Flags::END_POINT_UNREACHABLE) {
        match leave_block {
            Some(b) => {
                let jump = branch(f, b);
                f.add_il_range(jump, f.il_range(after_finally_exit));
                f.push(parent_block, jump);
            }
            None => f.push(parent_block, after_finally_exit),
        }
    }
    // The pre-init `stloc obj(ldnull)` and dead `stloc <int>(ldc.i4 0)` right before the
    // try-finally (which would block UsingTransform) go.
    f.remove(flag_init_store);
    remove_dead_flag_stores(f, parent_block, try_finally);
    true
}

fn remove_dead_flag_stores(f: &mut Function, parent_block: NodeId, try_finally: NodeId) {
    while f.node(try_finally).child_index > 0 {
        let prev = f.list(parent_block)[f.node(try_finally).child_index as usize - 1];
        match f.match_stloc(prev) {
            Some((v, value)) if f.match_ldc_i4(value) == Some(0) && f.var(v).load_count() == 0 => {
                f.remove(prev)
            }
            _ => break,
        }
    }
}

/// `FindFinallyDispatchExit`: the block ending in "if (obj == null) leave outer; br
/// dispatchHead", with a valid dispatch idiom. The block and the if's exit.
fn find_finally_dispatch_exit(
    f: &Function,
    start: NodeId,
    object_variable: VarId,
    container: NodeId,
) -> Option<(NodeId, NodeId)> {
    let mut visited = HashSet::new();
    let mut queue = VecDeque::from([start]);
    while let Some(b) = queue.pop_front() {
        if !visited.insert(b) {
            continue;
        }
        if let Some((dispatch_head, after)) = match_finally_exit_block(f, b, object_variable) {
            return collect_dispatch_blocks(f, dispatch_head, object_variable)
                .then_some((b, after));
        }
        for br in f.descendants_post(b) {
            if matches!(f.inst(br), Inst::Branch { .. }) {
                let t = f.branch_target(br);
                if f.node(t).parent == Some(container) {
                    queue.push_back(t);
                }
            }
        }
    }
    None
}

/// `MatchFinallyExitBlock`: `if (comp.o(ldloc obj == ldnull)) <leave outer | br block>; br
/// dispatchHead`. The dispatch head and the exit (a branch to a one-instruction function
/// leave block counts as that leave).
fn match_finally_exit_block(
    f: &Function,
    block: NodeId,
    object_variable: VarId,
) -> Option<(NodeId, NodeId)> {
    let list = f.list(block);
    if list.len() < 2 {
        return None;
    }
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(list[list.len() - 2])
    else {
        return None;
    };
    if f.match_comp_null(condition, ComparisonKind::Equality)
        .and_then(|a| f.match_ldloc(a))
        != Some(object_variable)
    {
        return None;
    }
    let mut after = true_inst;
    match *f.inst(after) {
        Inst::Leave { .. } if is_leaving_function(f, after) => {}
        Inst::Branch { .. } => {
            let target = f.branch_target(after);
            if let [only] = *f.list(target)
                && is_leaving_function(f, only)
            {
                after = only;
            }
        }
        _ => return None,
    }
    let dispatch_head = f.match_branch(list[list.len() - 1])?;
    Some((dispatch_head, after))
}

/// `CollectDispatchBlocks`: stloc tmp(isinst Exception(ldloc obj)); if (tmp != null) br capture; br throw
fn collect_dispatch_blocks(f: &Function, head: NodeId, object_variable: VarId) -> bool {
    let [store, if_inst, to_throw] = *f.list(head) else {
        return false;
    };
    let Some((typed, value)) = f.match_stloc(store) else {
        return false;
    };
    let Inst::IsInst { arg, .. } = *f.inst(value) else {
        return false;
    };
    if f.match_ldloc(arg) != Some(object_variable) {
        return false;
    }
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return false;
    };
    f.match_comp_null(condition, ComparisonKind::Inequality)
        .and_then(|a| f.match_ldloc(a))
        == Some(typed)
        && matches!(f.inst(true_inst), Inst::Branch { .. })
        && matches!(f.inst(to_throw), Inst::Branch { .. })
}

/// `RewriteFinallyExit`: the trailing `if` and branch become `leave finallyContainer`.
fn rewrite_finally_exit(f: &mut Function, block: NodeId, finally_container: NodeId) {
    let n = f.list(block).len();
    let if_inst = f.list(block)[n - 2];
    let br = f.list(block)[n - 1];
    let (if_range, br_range) = (f.il_range(if_inst), f.il_range(br));
    f.truncate(block, n - 2);
    let l = leave(f, finally_container);
    f.add_il_range(l, if_range);
    f.add_il_range(l, br_range);
    f.push(block, l);
}

/// `CollectReachable`: the blocks of `entry`'s container reachable from it.
fn collect_reachable(f: &Function, entry: NodeId) -> Vec<NodeId> {
    let container = f.node(entry).parent;
    let mut visited = HashSet::new();
    let mut result = Vec::new();
    let mut queue = VecDeque::from([entry]);
    while let Some(b) = queue.pop_front() {
        if !visited.insert(b) {
            continue;
        }
        if f.node(b).parent != container {
            continue;
        }
        result.push(b);
        for br in f.descendants_post(b) {
            if matches!(f.inst(br), Inst::Branch { .. }) {
                let t = f.branch_target(br);
                if f.node(t).parent == container {
                    queue.push_back(t);
                }
            }
        }
    }
    result
}

/// The `[stloc tmp(ldloc ex);] [stloc obj(ldloc tmp_or_ex);]` prefix before the flag store
/// of a handler block: the object variable and the temporary.
fn match_handler_prefix(
    f: &Function,
    block: NodeId,
    exception_variable: VarId,
) -> Option<(Option<VarId>, Option<VarId>)> {
    let list = f.list(block);
    let mut prefix = list.len() - 2;
    let mut object_variable = None;
    let mut temporary = None;
    if prefix >= 1
        && let Some((v, value)) = f.match_stloc(list[prefix - 1])
    {
        if f.match_ldloc(value) == Some(exception_variable) {
            object_variable = Some(v);
            prefix -= 1;
        } else if let Some(tmp) = f.match_ldloc(value)
            && prefix >= 2
            && let Some((tmp2, tmp_value)) = f.match_stloc(list[prefix - 2])
            && tmp == tmp2
            && f.match_ldloc(tmp_value) == Some(exception_variable)
        {
            temporary = Some(tmp);
            object_variable = Some(v);
            prefix -= 2;
        } else {
            return None;
        }
    }
    (prefix == 0).then_some((object_variable, temporary))
}

/// `TryRewriteTryCatch`:
///   stloc num(0); .try { ...; br continuation }
///   catch ex : T { [prefix] stloc num(1); br continuation }
///   continuation { if (num == 1) br catchBody; leave outer }
/// moves the catch body back into the handler.
#[allow(clippy::too_many_arguments)]
fn try_rewrite_try_catch(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    try_catch: NodeId,
    handler: NodeId,
    handler_block: NodeId,
    parent_block: NodeId,
    container: NodeId,
    filter_obj_by_handler: &HashMap<NodeId, VarId>,
) -> bool {
    let list = f.list(handler_block).to_vec();
    if list.len() < 2 {
        return false;
    }
    let Some(continuation) = f.match_branch(list[list.len() - 1]) else {
        return false;
    };
    if f.node(continuation).parent != Some(container) {
        return false;
    }
    let Some((num_variable, num_value)) = f.match_stloc(list[list.len() - 2]) else {
        return false;
    };
    if f.match_ldc_i4(num_value) != Some(1)
        || !is_known(ts, f.var(num_variable).ty, KnownTypeCode::Int32)
    {
        return false;
    }
    let Inst::TryCatchHandler {
        body: handler_body,
        variable,
        ..
    } = *f.inst(handler)
    else {
        return false;
    };
    let Some((object_variable, temporary)) = match_handler_prefix(f, handler_block, variable)
    else {
        return false;
    };
    let Some(flag_init_store) =
        find_flag_init_store(f, parent_block, try_catch, num_variable, |f, v| {
            f.match_ldc_i4(v) == Some(0)
        })
    else {
        return false;
    };
    let Some((catch_body_entry, after_catch_exit)) =
        match_catch_entry_check(f, continuation, num_variable, container)
    else {
        return false;
    };

    // The catch body's blocks move into the handler.
    let body_blocks = collect_reachable(f, catch_body_entry);
    for &b in &body_blocks {
        f.remove_block(b);
    }
    let jump = branch(f, catch_body_entry);
    clear_and_set(f, handler_block, &[jump]);
    for &b in &body_blocks {
        f.push(handler_body, b);
    }
    for v in [
        object_variable,
        temporary,
        filter_obj_by_handler.get(&handler).copied(),
    ]
    .into_iter()
    .flatten()
    {
        remap_variable_reads(f, handler_body, v, variable);
    }
    for b in blocks_under(f, handler_body) {
        replace_dispatch_idiom_with_rethrow(f, ts, b, variable);
    }
    // The catch entry check becomes the plain exit.
    clear_and_set(f, continuation, &[after_catch_exit]);
    f.remove(flag_init_store);
    true
}

/// `TryRewriteMultiHandlerTryCatch`: several handlers storing distinct flag values, and a
/// switch (or an if chain) dispatching to their bodies after the try-catch.
#[allow(clippy::too_many_arguments)]
fn try_rewrite_multi_handler_try_catch(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    try_catch: NodeId,
    handlers: &[NodeId],
    parent_block: NodeId,
    container: NodeId,
    filter_obj_by_handler: &HashMap<NodeId, VarId>,
) -> bool {
    let mut continuation = None;
    let mut num_variable = None;
    let mut seen = HashSet::new();
    let mut infos = Vec::new();
    for &handler in handlers {
        let Inst::TryCatchHandler {
            body: hb, variable, ..
        } = *f.inst(handler)
        else {
            return false;
        };
        if !matches!(f.inst(hb), Inst::BlockContainer { .. }) {
            return false;
        }
        let [hblock] = *f.list(hb) else {
            return false;
        };
        let list = f.list(hblock);
        if list.len() < 2 {
            return false;
        }
        let Some(br) = f.match_branch(list[list.len() - 1]) else {
            return false;
        };
        match continuation {
            None => continuation = Some(br),
            Some(c) if c != br => return false,
            Some(_) => {}
        }
        if f.node(br).parent != Some(container) {
            return false;
        }
        let Some((nv, nval)) = f.match_stloc(list[list.len() - 2]) else {
            return false;
        };
        let Some(k) = f.match_ldc_i4(nval) else {
            return false;
        };
        if !is_known(ts, f.var(nv).ty, KnownTypeCode::Int32) {
            return false;
        }
        match num_variable {
            None => num_variable = Some(nv),
            Some(n) if n != nv => return false,
            Some(_) => {}
        }
        if !seen.insert(k) {
            return false;
        }
        let Some((body_obj, body_tmp)) = match_handler_prefix(f, hblock, variable) else {
            return false;
        };
        infos.push((handler, k, body_obj, body_tmp));
    }
    let (Some(continuation), Some(num_variable)) = (continuation, num_variable) else {
        return false;
    };
    let Some(flag_init_store) =
        find_flag_init_store(f, parent_block, try_catch, num_variable, |f, v| {
            f.match_ldc_i4(v) == Some(0)
        })
    else {
        return false;
    };
    let Some((case_blocks, default_exit)) = match_switch_dispatch(f, continuation, num_variable)
        .or_else(|| match_if_chain_dispatch(f, continuation, num_variable, container))
    else {
        return false;
    };
    if !infos.iter().all(|(_, k, _, _)| case_blocks.contains_key(k)) {
        return false;
    }
    let cfg = ControlFlowGraph::new(f, container);
    for (handler, k, body_obj, body_tmp) in infos {
        let case_body = case_blocks[&k];
        let Inst::TryCatchHandler {
            body: handler_body,
            variable,
            ..
        } = *f.inst(handler)
        else {
            unreachable!("handler");
        };
        let handler_entry = f.list(handler_body)[0];
        move_dominated_blocks_to_container(f, case_body, None, &cfg, handler_body);
        // The entry keeps only a branch to the case body (now at index 1 or later).
        let jump = branch(f, case_body);
        clear_and_set(f, handler_entry, &[jump]);
        for v in [
            body_obj,
            body_tmp,
            filter_obj_by_handler.get(&handler).copied(),
        ]
        .into_iter()
        .flatten()
        {
            remap_variable_reads(f, handler_body, v, variable);
        }
        for b in blocks_under(f, handler_body) {
            replace_dispatch_idiom_with_rethrow(f, ts, b, variable);
        }
    }
    // The continuation is the default exit; the copy has no IL range of its own.
    let copy = f.clone_subtree(default_exit);
    f.set_il_range(copy, Interval::default());
    clear_and_set(f, continuation, &[copy]);
    f.remove(flag_init_store);
    true
}

/// `MatchIfChainDispatch`: `if (num == K_i) br case_K_i; br next` blocks ending in a leave.
fn match_if_chain_dispatch(
    f: &Function,
    continuation: NodeId,
    num: VarId,
    container: NodeId,
) -> Option<(HashMap<i32, NodeId>, NodeId)> {
    let mut case_blocks = HashMap::new();
    let mut visited = HashSet::new();
    let mut current = continuation;
    loop {
        if !visited.insert(current) {
            return None;
        }
        let [if_inst, fallthrough] = *f.list(current) else {
            return None;
        };
        let Inst::If {
            condition,
            true_inst,
            ..
        } = *f.inst(if_inst)
        else {
            return None;
        };
        let (lhs, rhs) = f.match_comp(condition, ComparisonKind::Equality)?;
        if f.match_ldloc(lhs) != Some(num) {
            return None;
        }
        let k = f.match_ldc_i4(rhs)?;
        let case_block = f.match_branch(true_inst)?;
        if case_blocks.insert(k, case_block).is_some() {
            return None;
        }
        if matches!(f.inst(fallthrough), Inst::Leave { .. })
            && leave_exits_container(f, fallthrough, container)
        {
            return Some((case_blocks, fallthrough));
        }
        let next = f.match_branch(fallthrough)?;
        if f.node(next).parent != Some(container) {
            return None;
        }
        // A one-instruction leave block ends the chain.
        if let [final_leave] = *f.list(next)
            && matches!(f.inst(final_leave), Inst::Leave { .. })
            && leave_exits_container(f, final_leave, container)
        {
            return Some((case_blocks, final_leave));
        }
        current = next;
    }
}

/// `MatchSwitchDispatch`: `switch (ldloc num) { case K: br case_K ...; default: <leave outer | br end> }`.
fn match_switch_dispatch(
    f: &Function,
    continuation: NodeId,
    num: VarId,
) -> Option<(HashMap<i32, NodeId>, NodeId)> {
    let [switch] = *f.list(continuation) else {
        return None;
    };
    let Inst::Switch {
        value,
        ref sections,
        ..
    } = *f.inst(switch)
    else {
        return None;
    };
    if f.match_ldloc(value) != Some(num) {
        return None;
    }
    let default_body = default_section_body(f, switch);
    let default_exit = match f.inst(default_body) {
        Inst::Leave { .. } if is_leaving_function(f, default_body) => default_body,
        Inst::Branch { .. } => default_body,
        _ => return None,
    };
    let mut case_blocks = HashMap::new();
    for &section in sections {
        let Inst::SwitchSection { ref labels, body } = *f.inst(section) else {
            return None;
        };
        if body == default_body {
            continue;
        }
        let case_block = f.match_branch(body)?;
        // Each other section covers exactly one value.
        if labels.count() != 1 {
            return None;
        }
        case_blocks.insert(labels.intervals()[0].start as i32, case_block);
    }
    Some((case_blocks, default_exit))
}

/// `MatchCatchEntryCheck`: `if (num == 1) br catchBody; <exit>` or `if (num != 1) <exit>; br
/// catchBody`. The catch body and the exit.
fn match_catch_entry_check(
    f: &Function,
    continuation: NodeId,
    num: VarId,
    container: NodeId,
) -> Option<(NodeId, NodeId)> {
    let [if_inst, second] = *f.list(continuation) else {
        return None;
    };
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(if_inst)
    else {
        return None;
    };
    let tests_flag = |kind| {
        f.match_comp(condition, kind)
            .is_some_and(|(l, r)| f.match_ldloc(l) == Some(num) && f.match_ldc_i4(r) == Some(1))
    };
    if tests_flag(ComparisonKind::Equality)
        && let Some(entry) = f.match_branch(true_inst)
        && is_container_exit(f, second, container)
    {
        return (f.node(entry).parent == Some(container)).then_some((entry, second));
    }
    if tests_flag(ComparisonKind::Inequality)
        && is_container_exit(f, true_inst, container)
        && let Some(entry) = f.match_branch(second)
    {
        return (f.node(entry).parent == Some(container)).then_some((entry, true_inst));
    }
    None
}

/// `IsContainerExit`: a leave out of `container`, a branch into a strict ancestor container,
/// or a branch to a one-instruction leave block of `container` that leaves it.
fn is_container_exit(f: &Function, inst: NodeId, container: NodeId) -> bool {
    match *f.inst(inst) {
        Inst::Leave { .. } => leave_exits_container(f, inst, container),
        Inst::Branch { .. } => {
            let target = f.branch_target(inst);
            let Some(target_container) = f.node(target).parent else {
                return false;
            };
            if target_container != container && is_descendant_of(f, container, target_container) {
                return true;
            }
            target_container == container
                && matches!(*f.list(target), [l] if matches!(f.inst(l), Inst::Leave { .. }) && leave_exits_container(f, l, container))
        }
        _ => false,
    }
}

/// `FlagVariableMatches`: the same variable, or the same slot and kind with an `int` type.
fn flag_variable_matches(
    f: &Function,
    ts: &TypeSystem<'_, '_>,
    candidate: VarId,
    flag: VarId,
) -> bool {
    candidate == flag
        || (f.var(candidate).index == f.var(flag).index
            && f.var(candidate).kind == f.var(flag).kind
            && is_known(ts, f.var(candidate).ty, KnownTypeCode::Int32))
}

/// `TryRewriteFlagBasedEarlyReturn`: a `return` crossing a try-finally with awaits is lowered
/// to `stloc capture(value); stloc flag(K); leave try` and, after the try-finally,
/// `if (flag == K) leave outer (capture)`. Each flag setter gets the early action directly.
fn try_rewrite_flag_based_early_return(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    try_finally: NodeId,
) -> bool {
    let Some(parent_block) = f
        .node(try_finally)
        .parent
        .filter(|&p| matches!(f.inst(p), Inst::Block { .. }))
    else {
        return false;
    };
    let Some(container) = f
        .node(parent_block)
        .parent
        .filter(|&c| matches!(f.inst(c), Inst::BlockContainer { .. }))
    else {
        return false;
    };
    let index = f.node(try_finally).child_index as usize;
    if index + 1 == f.list(parent_block).len() {
        return false;
    }
    let next = f.list(parent_block)[index + 1];
    let (check_block, check_start, check_inline) = if matches!(f.inst(next), Inst::Branch { .. })
        && f.node(f.branch_target(next)).parent == Some(container)
    {
        (f.branch_target(next), 0, false)
    } else if matches!(f.inst(next), Inst::If { .. }) {
        (parent_block, index + 1, true)
    } else {
        return false;
    };
    let check_list = f.list(check_block).to_vec();
    if check_start + 1 >= check_list.len() {
        return false;
    }
    let Inst::If {
        condition,
        true_inst,
        ..
    } = *f.inst(check_list[check_start])
    else {
        return false;
    };
    let flag_test = |kind| {
        let (l, r) = f.match_comp(condition, kind)?;
        Some((f.match_ldloc(l)?, f.match_ldc_i4(r)?))
    };
    let (flag_var, target_k, early_action, normal_action) =
        if let Some((v, k)) = flag_test(ComparisonKind::Equality) {
            (v, k, true_inst, check_list[check_start + 1])
        } else if let Some((v, k)) = flag_test(ComparisonKind::Inequality) {
            (v, k, check_list[check_start + 1], true_inst)
        } else {
            return false;
        };
    if !is_known(ts, f.var(flag_var).ty, KnownTypeCode::Int32) {
        return false;
    }
    let early_template = if matches!(f.inst(early_action), Inst::Leave { .. })
        && leave_exits_container(f, early_action, container)
    {
        vec![early_action]
    } else if matches!(f.inst(early_action), Inst::Branch { .. })
        && f.node(f.branch_target(early_action)).parent == Some(container)
        && let Some(t) = early_action_template(f, f.branch_target(early_action), container)
    {
        t
    } else {
        return false;
    };
    let Inst::TryFinally { try_block, .. } = *f.inst(try_finally) else {
        return false;
    };
    if !matches!(f.inst(try_block), Inst::BlockContainer { .. }) {
        return false;
    }
    // The flag setters anywhere in the try block: `stloc flag(K); leave tryBlock` at the end.
    let mut flag_setters = Vec::new();
    for b in blocks_under(f, try_block) {
        let list = f.list(b);
        let n = list.len();
        if n < 2 {
            continue;
        }
        let Some((v, value)) = f.match_stloc(list[n - 2]) else {
            continue;
        };
        if !flag_variable_matches(f, ts, v, flag_var) || f.match_ldc_i4(value) != Some(target_k) {
            continue;
        }
        if !f
            .match_leave(list[n - 1])
            .is_some_and(|(c, _)| c == try_block)
        {
            continue;
        }
        flag_setters.push(b);
    }
    if flag_setters.is_empty() {
        return false;
    }
    // The flag is only set there and by the pre-try `stloc flag(0)`.
    for &store in &f.var(flag_var).store_instructions {
        let Some((_, value)) = f.match_stloc(store) else {
            continue;
        };
        if flag_setters.iter().any(|&fs| f.list(fs).contains(&store)) {
            continue;
        }
        if f.node(store).parent == Some(parent_block) && f.match_ldc_i4(value) == Some(0) {
            continue;
        }
        return false;
    }

    for &fs in &flag_setters {
        let n = f.list(fs).len();
        f.remove_at(fs, n - 1);
        f.remove_at(fs, n - 2);
        for &inst in &early_template {
            let spliced = f.clone_subtree(inst);
            f.set_il_range(spliced, Interval::default());
            f.push(fs, spliced);
        }
    }
    let normal_copy = f.clone_subtree(normal_action);
    f.set_il_range(normal_copy, Interval::default());
    if check_inline {
        f.remove_at(parent_block, check_start + 1);
        f.remove_at(parent_block, check_start);
        f.push(parent_block, normal_copy);
    } else {
        clear_and_set(f, check_block, &[normal_copy]);
    }
    sort_blocks(f, try_block);
    if f.has_flag(try_finally, Flags::END_POINT_UNREACHABLE) {
        let keep = f.node(try_finally).child_index as usize + 1;
        while f.list(parent_block).len() > keep {
            let last = f.list(parent_block).len() - 1;
            f.remove_at(parent_block, last);
        }
    }
    // The pre-try `stloc flag(0)`.
    let mut i = 0;
    while let Some(&inst) = f.list(parent_block).get(i) {
        if let Some((v, value)) = f.match_stloc(inst)
            && flag_variable_matches(f, ts, v, flag_var)
            && f.match_ldc_i4(value) == Some(0)
        {
            f.remove_at(parent_block, i);
        } else {
            i += 1;
        }
    }
    true
}

/// `TryGetEarlyActionTemplate`: the stores of a chain of `stores; br next` blocks and the
/// final leave out of `container`.
fn early_action_template(
    f: &Function,
    early_block: NodeId,
    container: NodeId,
) -> Option<Vec<NodeId>> {
    let mut template = Vec::new();
    let mut visited = HashSet::new();
    let mut current = early_block;
    loop {
        if !visited.insert(current) {
            return None;
        }
        let list = f.list(current);
        let (&last, rest) = list.split_last()?;
        template.extend_from_slice(rest);
        if matches!(f.inst(last), Inst::Leave { .. }) && leave_exits_container(f, last, container) {
            template.push(last);
            return Some(template);
        }
        current = f.match_branch(last)?;
    }
}

/// `ReplaceDispatchIdiomWithRethrow`: the `ExceptionDispatchInfo` rethrow of a `throw;`.
fn replace_dispatch_idiom_with_rethrow(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    handler_variable: VarId,
) {
    let mut v = Some(handler_variable);
    let Some((typed_store, capture_block, throw_block)) =
        match_exception_capture_block(f, ts, block, &mut v)
    else {
        return;
    };
    if v != Some(handler_variable) {
        return;
    }
    // The rethrow stands in for the whole idiom.
    let index = f.node(typed_store).child_index as usize;
    let rethrow = f.add(Inst::Rethrow);
    f.add_il_range(rethrow, f.il_range(typed_store));
    for inst in [f.list(block)[index + 1], f.list(block)[index + 2]] {
        f.add_il_range(rethrow, f.il_range(inst));
    }
    for inst in f
        .list(capture_block)
        .to_vec()
        .into_iter()
        .chain(f.list(throw_block).to_vec())
    {
        f.add_il_range(rethrow, f.il_range(inst));
    }
    f.remove_at(block, index + 1);
    f.remove_at(block, index + 1);
    f.remove_block(capture_block);
    f.remove_block(throw_block);
    f.replace(typed_store, rethrow);
}
