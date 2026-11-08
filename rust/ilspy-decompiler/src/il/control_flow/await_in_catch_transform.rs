//! `IL/ControlFlow/AwaitInCatchTransform.cs`: the compiler moves a catch block containing an
//! await out of the handler (the handler stores the exception and a block id, and a jump
//! table after the try-catch dispatches to the real catch body). This moves the body back.

use std::collections::HashSet;

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId, NopKind, VarId};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::remove_infeasible_path_transform::sort_blocks;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::TypeSystem;

use super::control_flow_graph::ControlFlowGraph;

struct CatchBlockInfo {
    handler: NodeId,
    real_catch_block_entry_point: NodeId,
    /// A block, or the container to leave.
    next_block_or_exit_container: Option<NodeId>,
    /// The `if` of the jump table, or the switch section.
    jump_table_entry: NodeId,
    object_variable: VarId,
}

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.await_in_catch_finally {
        return;
    }
    let mut changed_containers: Vec<NodeId> = Vec::new();
    let mut removed_blocks: HashSet<NodeId> = HashSet::new();
    let body = r.function.body.expect("body");
    let try_catches: Vec<NodeId> = r
        .function
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(r.function.inst(n), Inst::TryCatch { .. }))
        .collect();
    for try_catch in try_catches {
        let f = &mut r.function;
        let Some(container) = f
            .node(try_catch)
            .parent
            .and_then(|p| f.node(p).parent)
            .filter(|&c| matches!(f.inst(c), Inst::BlockContainer { .. }))
        else {
            continue;
        };
        let Inst::TryCatch { ref handlers, .. } = *f.inst(try_catch) else {
            unreachable!("try-catch");
        };
        let handlers = handlers.clone();
        // The handlers containing an await.
        let transformable = analyze_handlers(f, &handlers);
        let cfg = ControlFlowGraph::new(f, container);
        if !transformable.is_empty() && !changed_containers.contains(&container) {
            changed_containers.push(container);
        }
        let mut switch_instruction = None;
        for result in transformable {
            removed_blocks.clear();
            let node = cfg.node_of(result.real_catch_block_entry_point);

            // Remove the jump table entry and the branches to its block.
            match *f.inst(result.jump_table_entry) {
                Inst::If { .. } => {
                    let jump_table_block = f
                        .node(result.jump_table_entry)
                        .parent
                        .expect("if in a block");
                    f.remove(result.jump_table_entry);
                    for branch in f.descendants_post(try_catch) {
                        if matches!(f.inst(branch), Inst::Branch { .. })
                            && f.branch_target(branch) == jump_table_block
                        {
                            let next = result.next_block_or_exit_container.expect("next block");
                            let replacement = if matches!(f.inst(next), Inst::BlockContainer { .. })
                            {
                                let nop = f.add(Inst::Nop {
                                    kind: NopKind::Normal,
                                });
                                f.add(Inst::Leave {
                                    target: Some(next),
                                    value: nop,
                                })
                            } else {
                                f.add(Inst::Branch {
                                    target: crate::il::ast::BranchTarget::Block(next),
                                })
                            };
                            f.replace(branch, replacement);
                        }
                    }
                }
                Inst::SwitchSection { .. } => {
                    switch_instruction = f.node(result.jump_table_entry).parent;
                }
                _ => {}
            }

            // The real catch block's entry point goes into the handler's container.
            let Inst::TryCatchHandler {
                body: handler_body,
                variable: handler_variable,
                ..
            } = *f.inst(result.handler)
            else {
                unreachable!("handler");
            };
            let catch_block_head = *f.list(handler_body).last().expect("handler block");
            f.remove_block(result.real_catch_block_entry_point);
            f.insert(handler_body, 0, result.real_catch_block_entry_point);
            // The generated catch block goes.
            f.remove_block(catch_block_head);

            transform_async_throw_to_throw(
                f,
                ctx.ts,
                &mut removed_blocks,
                result.real_catch_block_entry_point,
            );

            // Move the blocks the real catch block's entry point dominates.
            for n in 0..cfg.nodes.len() {
                let block = cfg.nodes[n].block;
                if cfg.dominates(node, n) {
                    transform_async_throw_to_throw(f, ctx.ts, &mut removed_blocks, block);
                    if f.node(block).parent == Some(handler_body) {
                        continue;
                    }
                    if !removed_blocks.contains(&block) {
                        f.remove_block(block);
                        f.push(handler_body, block);
                    }
                }
            }

            // Remove the pattern blocks that are now unreachable.
            if let Some(next) = result.next_block_or_exit_container
                && matches!(f.inst(next), Inst::Block { .. })
                && f.node(next).incoming_edge_count == 0
            {
                let mut dependent: Vec<NodeId> = Vec::new();
                let mut current = Some(next);
                while let Some(c) = current {
                    for b in f.descendants_post(c) {
                        if matches!(f.inst(b), Inst::Branch { .. }) {
                            dependent.push(f.branch_target(b));
                        }
                    }
                    f.remove_block(c);
                    if let Some(i) = dependent.iter().position(|&d| d == c) {
                        dependent.remove(i);
                    }
                    current = dependent
                        .iter()
                        .copied()
                        .find(|&d| f.node(d).incoming_edge_count == 0);
                }
            }

            // The common object variable's loads become the handler variable's.
            if result.object_variable != handler_variable {
                for load in f.var(result.object_variable).load_instructions.clone() {
                    if !f.ancestors(load).any(|a| a == result.handler) {
                        continue;
                    }
                    let parent = f.node(load).parent.expect("load has a parent");
                    let handler_type = f.var(handler_variable).ty;
                    let replaced = match *f.inst(parent) {
                        Inst::CastClass { ty, .. } if ty.ty == handler_type => parent,
                        _ => load,
                    };
                    let ldloc = f.add(Inst::LdLoc {
                        var: handler_variable,
                    });
                    f.add_il_range(ldloc, f.il_range(replaced));
                    if replaced != load {
                        f.add_il_range(ldloc, f.il_range(load));
                    }
                    f.replace(replaced, ldloc);
                }
            }
        }

        if let Some(switch) = switch_instruction
            && let Some(block) = f.node(switch).parent
            && matches!(f.inst(block), Inst::Block { .. })
            && f.node(block).incoming_edge_count > 0
        {
            let default_body = default_section_body(f, switch);
            for branch in f.descendants_post(container) {
                if !matches!(f.inst(branch), Inst::Branch { .. })
                    || f.branch_target(branch) != block
                {
                    continue;
                }
                let copy = f.clone_subtree(default_body);
                f.replace(branch, copy);
            }
        }
    }

    for container in changed_containers {
        sort_blocks(&mut r.function, container);
    }
}

/// `SwitchInstruction.GetDefaultSection().Body`: the section with the most labels.
pub(crate) fn default_section_body(f: &Function, switch: NodeId) -> NodeId {
    let Inst::Switch { sections, .. } = f.inst(switch) else {
        unreachable!("switch");
    };
    let mut default = sections[0];
    for &s in sections {
        let count = |s: NodeId| match f.inst(s) {
            Inst::SwitchSection { labels, .. } => labels.count(),
            _ => 0,
        };
        if count(s) > count(default) {
            default = s;
        }
    }
    match *f.inst(default) {
        Inst::SwitchSection { body, .. } => body,
        _ => unreachable!("section"),
    }
}

fn transform_async_throw_to_throw(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    removed_blocks: &mut HashSet<NodeId>,
    block: NodeId,
) {
    let mut v = None;
    if let Some((typed_store, capture_block, throw_block)) =
        match_exception_capture_block(f, ts, block, &mut v)
    {
        // ExceptionDispatchInfo.Capture(v).Throw() => throw;
        let index = f.node(typed_store).child_index as usize;
        f.remove_at(block, index + 1);
        f.remove_at(block, index + 1);
        f.remove_block(capture_block);
        f.remove_block(throw_block);
        removed_blocks.insert(capture_block);
        removed_blocks.insert(throw_block);
        let rethrow = f.add(Inst::Rethrow);
        f.replace(typed_store, rethrow);
    }
}

fn analyze_handlers(f: &Function, handlers: &[NodeId]) -> Vec<CatchBlockInfo> {
    let mut result = Vec::new();
    let mut catch_handler_identifier = None;
    for &handler in handlers {
        let Some((id, identifier, real_entry_point, next, jump_table_entry, object_variable)) =
            match_await_catch_handler(f, handler)
        else {
            continue;
        };
        if id < 1
            || (catch_handler_identifier.is_some() && Some(identifier) != catch_handler_identifier)
        {
            continue;
        }
        catch_handler_identifier = Some(identifier);
        let Inst::TryCatchHandler { variable, .. } = *f.inst(handler) else {
            unreachable!("handler");
        };
        result.push(CatchBlockInfo {
            handler,
            real_catch_block_entry_point: real_entry_point,
            next_block_or_exit_container: next,
            jump_table_entry,
            object_variable: object_variable.unwrap_or(variable),
        });
    }
    result
}

type HandlerMatch = (i32, VarId, NodeId, Option<NodeId>, NodeId, Option<VarId>);

/// `MatchAwaitCatchHandler`:
///   [stloc V_3(ldloc E_100) - copy of the exception variable]
///   stloc V_6(ldloc V_3)    - the exception in the shared object variable
///   stloc V_5(ldc.i4 2)     - the id of the catch block
///   br IL_0075              - to the head of the jump table
fn match_await_catch_handler(f: &Function, handler: NodeId) -> Option<HandlerMatch> {
    let Inst::TryCatchHandler {
        body,
        variable: exception_variable,
        ..
    } = *f.inst(handler)
    else {
        return None;
    };
    let catch_block = f.list(body)[0];
    let list = f.list(catch_block);
    let mut object_variable = None;
    match list.len() {
        3 => {
            let (v, value) = f.match_stloc(list[0])?;
            if f.match_ldloc(value) != Some(exception_variable) {
                return None;
            }
            object_variable = Some(v);
        }
        4 => {
            let (temporary, value) = f.match_stloc(list[0])?;
            if f.match_ldloc(value) != Some(exception_variable) {
                return None;
            }
            let (v, value) = f.match_stloc(list[1])?;
            if f.match_ldloc(value) != Some(temporary) {
                return None;
            }
            object_variable = Some(v);
        }
        // An unused exception variable (catch (Exception)) has no assignment in optimized code.
        _ => {}
    }
    let jump_table_start = f.match_branch(*list.last()?)?;
    if list.len() < 2 {
        return None;
    }
    let (identifier, value) = f.match_stloc(list[list.len() - 2])?;
    let id = f.match_ldc_i4(value)?;
    let (real, next, entry) = match *f.list(jump_table_start) {
        // stloc identifierVariableCopy(identifierVariable); if (...) br realEntryPoint; br next
        [copy, _, _] => {
            let (copy_var, load) = f.match_stloc(copy)?;
            if f.match_ldloc(load) != Some(identifier) {
                return None;
            }
            parse_if_jump_table(f, id, jump_table_start, copy_var)?
        }
        [_, _] => parse_if_jump_table(f, id, jump_table_start, identifier)?,
        [switch] if matches!(f.inst(switch), Inst::Switch { .. }) => {
            parse_switch_jump_table(f, id, switch, identifier)?
        }
        _ => return None,
    };
    Some((id, identifier, real, next, entry, object_variable))
}

fn parse_switch_jump_table(
    f: &Function,
    id: i32,
    switch: NodeId,
    identifier: VarId,
) -> Option<(NodeId, Option<NodeId>, NodeId)> {
    let Inst::Switch {
        value,
        ref sections,
        ..
    } = *f.inst(switch)
    else {
        return None;
    };
    if f.match_ldloc(value) != Some(identifier) {
        return None;
    }
    let default_body = default_section_body(f, switch);
    for &section in sections {
        let Inst::SwitchSection {
            ref labels, body, ..
        } = *f.inst(section)
        else {
            continue;
        };
        if !labels.contains(i64::from(id)) {
            continue;
        }
        let real = f.match_branch(body)?;
        let next = f
            .match_branch(default_body)
            .or_else(|| f.match_leave(default_body).map(|(c, _)| c));
        return Some((real, next, section));
    }
    None
}

fn parse_if_jump_table(
    f: &Function,
    id: i32,
    mut block: NodeId,
    identifier: VarId,
) -> Option<(NodeId, Option<NodeId>, NodeId)> {
    loop {
        let list = f.list(block);
        if list.len() < 2 {
            return None;
        }
        let if_inst = list[list.len() - 2];
        let Inst::If {
            condition,
            true_inst,
            ..
        } = *f.inst(if_inst)
        else {
            return None;
        };
        let last = list[list.len() - 1];
        let exit_container = f.node(block).parent;
        let leaves_container = |i: NodeId| {
            f.match_leave(i)
                .is_some_and(|(c, v)| Some(c) == exit_container && f.match_nop(v))
        };
        let (left, right, real, next_block) = if let Some((left, right)) =
            f.match_comp(condition, ComparisonKind::Equality)
        {
            let real = f.match_branch(true_inst)?;
            let next = f.match_branch(last);
            if next.is_none() && !leaves_container(last) {
                return None;
            }
            (left, right, real, next)
        } else if let Some((left, right)) = f.match_comp(condition, ComparisonKind::Inequality) {
            let real = f.match_branch(last)?;
            let next = f.match_branch(true_inst);
            if next.is_none() && !leaves_container(true_inst) {
                return None;
            }
            (left, right, real, next)
        } else {
            return None;
        };
        if f.match_ldloc(left) != Some(identifier) {
            return None;
        }
        if f.match_ldc_i4(right) == Some(id) {
            return Some((real, next_block.or(exit_container), if_inst));
        }
        match next_block {
            Some(b) if f.list(b).len() == 2 => block = b,
            _ => return None,
        }
    }
}

/// `MatchExceptionCaptureBlock`:
///   stloc typedExceptionVariable(isinst System.Exception(ldloc objectVariable))
///   if (comp.o(ldloc typedExceptionVariable != ldnull)) br captureBlock
///   br throwBlock
/// throwBlock: throw(ldloc objectVariable)
/// captureBlock: callvirt Throw(call Capture(ldloc typedExceptionVariable)); br nextBlock
/// The typed store, the capture block and the throw block.
pub(crate) fn match_exception_capture_block(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    block: NodeId,
    object_variable: &mut Option<VarId>,
) -> Option<(NodeId, NodeId, NodeId)> {
    let list = f.list(block);
    let typed_store = *list.get(list.len().checked_sub(3)?)?;
    let (typed_var, value) = f.match_stloc(typed_store)?;
    let Inst::IsInst { arg, ty } = *f.inst(value) else {
        return None;
    };
    let derives_from_exception = ts.all_base_types(ty.ty).is_ok_and(|bases| {
        bases.iter().any(|&b| {
            ts.definition(b)
                .and_then(|d| ts.comp.type_def(d).ok())
                .is_some_and(|i| i.known == KnownTypeCode::Exception)
        })
    });
    if !derives_from_exception {
        return None;
    }
    let v = f.match_ldloc(arg)?;
    match *object_variable {
        None => *object_variable = Some(v),
        Some(o) if o != v => return None,
        Some(_) => {}
    }
    let (condition, true_inst) = f.match_if_instruction(list[list.len() - 2])?;
    let throw_block = f.match_branch(list[list.len() - 1])?;
    let tested = f.match_comp_null(condition, ComparisonKind::Inequality)?;
    let capture_block = f.match_branch(true_inst)?;
    if f.match_ldloc(tested) != Some(typed_var) {
        return None;
    }
    let throw_ok = f.node(throw_block).incoming_edge_count == 1
        && matches!(*f.list(throw_block), [t] if matches!(*f.inst(t), Inst::Throw { arg } if f.match_ldloc(arg) == *object_variable));
    if !throw_ok {
        return None;
    }
    if f.node(capture_block).incoming_edge_count != 1 || f.list(capture_block).len() != 2 {
        return None;
    }
    let Inst::CallVirt(throw_call) = f.inst(f.list(capture_block)[0]) else {
        return None;
    };
    let [inner] = throw_call.args[..] else {
        return None;
    };
    let Inst::Call(capture) = f.inst(inner) else {
        return None;
    };
    let [captured] = capture.args[..] else {
        return None;
    };
    if f.match_ldloc(captured) != Some(typed_var) {
        return None;
    }
    let is_dispatch_info =
        |ts: &mut TypeSystem<'_, '_>, t: Option<crate::typesystem::types::TypeId>| {
            t.is_some_and(|t| {
                ts.full_name(t) == "System.Runtime.ExceptionServices.ExceptionDispatchInfo"
            })
        };
    let ok = throw_call.method.method.name == "Throw"
        && is_dispatch_info(ts, throw_call.method.method.declaring)
        && capture.method.method.name == "Capture"
        && is_dispatch_info(ts, capture.method.method.declaring);
    ok.then_some((typed_store, capture_block, throw_block))
}
