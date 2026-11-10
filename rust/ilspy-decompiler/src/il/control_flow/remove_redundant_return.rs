//! `IL/ControlFlow/RemoveRedundantReturn.cs`: like the exit point detection, but only for the
//! `leave` instructions leaving a whole `void` function or iterator (`return` or
//! `yield break`), which become an implicit fall-through where no `goto` is needed.

use crate::il::ast::{BlockKind, ContainerKind, Function, Inst, NodeId, NopKind};
use crate::il::reader::ReadFunction;
use crate::il::transforms::Context;
use crate::il::transforms::il_inlining::known;
use crate::il::transforms::local_function_decompiler::all_functions;
use crate::typesystem::known::KnownTypeCode;

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    // `function.Descendants.OfType<ILFunction>()`: the nested functions, the top-level one last.
    let mut functions: Vec<Option<NodeId>> =
        all_functions(&r.function).into_iter().map(Some).collect();
    functions.push(None);
    for function in functions {
        let (body, return_type, is_iterator) = match function {
            None => {
                let ret = ctx.ts.method_definition(r.method).ok().map(|m| m.ret);
                (r.function.body, r.async_return_type.or(ret), r.is_iterator)
            }
            Some(n) => match r.function.inst(n) {
                Inst::Function {
                    body,
                    return_type,
                    read,
                    ..
                } => {
                    let (async_return, iterator) = read.as_ref().map_or((None, false), |read| {
                        (read.async_return_type, read.is_iterator)
                    });
                    (
                        Some(*body),
                        Some(async_return.unwrap_or(*return_type)),
                        iterator,
                    )
                }
                _ => continue,
            },
        };
        let Some(c) = body else {
            continue;
        };
        if !matches!(r.function.inst(c), Inst::BlockContainer { .. }) {
            continue;
        }
        let is_void = return_type.is_some_and(|t| known(ctx, t) == Some(KnownTypeCode::Void));
        if !(is_void || is_iterator) {
            continue;
        }
        let f = &mut r.function;
        let &last_block = f.list(c).last().expect("a container has blocks");
        let &last = f.list(last_block).last().expect("a block exit");
        if f.match_leave(last)
            .is_some_and(|(t, _)| is_leaving_function(f, t))
        {
            let list = f.list(last_block);
            if list.len() >= 2 {
                let second_to_last = list[list.len() - 2];
                convert_return_to_fallthrough(f, Some(second_to_last));
            }
        } else if convert_return_to_fallthrough(f, Some(last)) {
            let leave = new_leave(f, c);
            f.push(last_block, leave);
        }
    }
}

/// `Leave.IsLeavingFunction`: the target is a function's body.
fn is_leaving_function(f: &Function, container: NodeId) -> bool {
    Some(container) == f.body
        || f.node(container)
            .parent
            .is_some_and(|p| matches!(f.inst(p), Inst::Function { .. }))
}

fn new_leave(f: &mut Function, container: NodeId) -> NodeId {
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    f.add(Inst::Leave {
        target: Some(container),
        value: nop,
    })
}

/// A `return` without a value.
fn is_plain_return(f: &Function, inst: NodeId) -> bool {
    f.match_leave(inst)
        .is_some_and(|(t, value)| is_leaving_function(f, t) && f.match_nop(value))
}

/// `ConvertReturnToFallthrough`: whether a return at the end of `inst` was made implicit.
fn convert_return_to_fallthrough(f: &mut Function, inst: Option<NodeId>) -> bool {
    let Some(inst) = inst else {
        return false;
    };
    let mut result = false;
    match f.inst(inst).clone() {
        Inst::BlockContainer {
            kind: ContainerKind::Normal,
            blocks,
            ..
        } => {
            // The body of a try block or similar; loops and switches were handled by the exit
            // point detection.
            let &last_block = blocks.last().expect("a container has blocks");
            let &last = f.list(last_block).last().expect("a block exit");
            if is_plain_return(f, last) {
                f.set_leave_target(last, inst);
                result = true;
            } else if convert_return_to_fallthrough(f, Some(last)) {
                let leave = new_leave(f, inst);
                f.push(last_block, leave);
                result = true;
            }
        }
        Inst::TryCatch {
            try_block,
            handlers,
        } => {
            result |= convert_return_to_fallthrough(f, Some(try_block));
            for h in handlers {
                if let Inst::TryCatchHandler { body, .. } = *f.inst(h) {
                    result |= convert_return_to_fallthrough(f, Some(body));
                }
            }
        }
        Inst::TryFinally { try_block, .. } => {
            result |= convert_return_to_fallthrough(f, Some(try_block));
        }
        Inst::Lock { body, .. } | Inst::Using { body, .. } | Inst::PinnedRegion { body, .. } => {
            result |= convert_return_to_fallthrough(f, Some(body));
        }
        Inst::If {
            true_inst,
            false_inst,
            ..
        } => {
            result |= convert_return_to_fallthrough(f, Some(true_inst));
            result |= convert_return_to_fallthrough(f, Some(false_inst));
        }
        Inst::Block {
            kind: BlockKind::ControlFlow,
            ..
        } => {
            let mut last = f.list(inst).last().copied();
            if let Some(l) = last
                && is_plain_return(f, l)
            {
                let len = f.list(inst).len();
                f.remove_at(inst, len - 1);
                result = true;
                last = f.list(inst).last().copied();
            }
            result |= convert_return_to_fallthrough(f, last);
        }
        _ => {}
    }
    result
}
