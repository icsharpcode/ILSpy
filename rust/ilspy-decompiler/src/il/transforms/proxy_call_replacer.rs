//! `IL/Transforms/ProxyCallReplacer.cs`: a call to a compiler-generated proxy method of the
//! current or an outer class, whose body only forwards its parameters to another instance
//! method (the `<>n__0` helpers an async method uses for `base.M()` calls), becomes a call
//! to that method.

use crate::il::ast::{CallData, Inst, MethodOperand, NodeId, VariableKind};
use crate::il::reader::{ReadFunction, read_il};
use crate::metadata::tables::rows;
use crate::typesystem::compilation::TypeDefId;
use crate::typesystem::members::{MethodDefId, has_body};
use crate::typesystem::resolve::MethodOrigin;

use super::{Context, run_early_il_transforms};

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.async_await {
        return;
    }
    let Some(body) = r.function.body else {
        return;
    };
    let calls: Vec<NodeId> = r
        .function
        .descendants_post(body)
        .into_iter()
        .filter(|&n| {
            matches!(
                r.function.inst(n),
                Inst::Call(_) | Inst::CallVirt(_) | Inst::NewObj(_)
            )
        })
        .collect();
    for inst in calls {
        replace(r, ctx, inst);
    }
}

fn replace(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, inst: NodeId) {
    let (Inst::Call(c) | Inst::CallVirt(c) | Inst::NewObj(c)) = r.function.inst(inst) else {
        return;
    };
    let outer = c.method.clone();
    let (outer_is_tail, outer_stack_was_empty) = (c.is_tail, c.il_stack_was_empty);
    if outer.method.is_static {
        return;
    }
    let MethodOrigin::Def(proxy) = outer.method.origin else {
        return;
    };
    if !is_defined_in_current_or_outer_class(ctx, proxy, r.method) {
        return;
    }
    if !ctx
        .ts
        .method_is_compiler_generated_or_in_compiler_generated_class(&outer.method)
    {
        return;
    }
    let comp = ctx.ts.comp;
    let md = &comp.module(proxy.module).md;
    let Ok(def) = md.get::<rows::MethodDef>(proxy.row) else {
        return;
    };
    if !has_body(&def) {
        return;
    }
    let Ok(body) = md.method_body(def.rva) else {
        return;
    };
    // The proxy is read in its own generic context (the managed code's `new GenericContext(inst.Method)`).
    let Ok(mut proxy_function) = read_il(ctx.ts, proxy, body, ctx.reader) else {
        return;
    };
    let mut inner = Context {
        ts: &mut *ctx.ts,
        settings: ctx.settings,
        reader: ctx.reader,
        rerun_requested: false,
        removed_variables: Vec::new(),
        resolver: ctx.resolver,
        root_names: ctx.root_names,
    };
    run_early_il_transforms(&mut proxy_function, &mut inner, false);
    let Some(call) = forwarded_call(&proxy_function) else {
        return;
    };
    let p = &proxy_function.function;
    let Inst::Call(call) = p.inst(call) else {
        return;
    };
    let target = &call.method.method;
    if target.name == ".ctor" || target.name == ".cctor" || target.is_static {
        return;
    }
    let (Ok(target_params), Ok(outer_params)) = (target.params(), outer.method.params()) else {
        return;
    };
    if target_params.len() != outer_params.len() {
        return;
    }
    // The arguments are the proxy's parameters in order (`this` first).
    for (i, &arg) in call.args.iter().enumerate() {
        let Inst::LdLoc { var } = *p.inst(arg) else {
            return;
        };
        let v = p.var(var);
        if v.kind != VariableKind::Parameter || v.index != Some(i as i32 - 1) {
            return;
        }
    }
    // The proxy call's substitution applied to the forwarded method.
    let substitution = (
        outer.method.class_args.clone(),
        outer.method.method_args.clone(),
    );
    let Ok(method) = ctx.ts.specialize_method(&call.method.method, &substitution) else {
        return;
    };
    let new_call = CallData {
        method: MethodOperand {
            method,
            specialized_by: Some(Box::new(outer)),
            ..call.method.clone()
        },
        args: Vec::new(),
        is_tail: call.is_tail && outer_is_tail,
        constrained_to: call.constrained_to,
        il_stack_was_empty: outer_stack_was_empty,
    };
    // The node keeps its arguments and IL range, as `newInst.Arguments.ReplaceList(inst.Arguments)`
    // and `newInst.AddILRange(inst)` do.
    let f = &mut r.function;
    let old = std::mem::replace(f.inst_mut(inst), Inst::Call(new_call));
    let (Inst::Call(old) | Inst::CallVirt(old) | Inst::NewObj(old)) = old else {
        unreachable!("the instruction was a call");
    };
    if let Inst::Call(c) = f.inst_mut(inst) {
        c.args = old.args;
    }
}

/// The `call` the proxy's single block forwards to: `leave (call ...)`, or `call ...` followed
/// by `leave (nop)`.
fn forwarded_call(proxy: &ReadFunction) -> Option<NodeId> {
    let p = &proxy.function;
    let container = proxy.function.body?;
    let Inst::BlockContainer { blocks, .. } = p.inst(container) else {
        return None;
    };
    let &[block] = &blocks[..] else {
        return None;
    };
    let Inst::Block { instructions, .. } = p.inst(block) else {
        return None;
    };
    let leave_value = |i: NodeId| match p.match_leave(i) {
        Some((c, value)) if c == container => Some(value),
        _ => None,
    };
    let call = match instructions[..] {
        [leave] => leave_value(leave)?,
        [call, leave] => {
            if !p.match_nop(leave_value(leave)?) {
                return None;
            }
            call
        }
        _ => return None,
    };
    matches!(p.inst(call), Inst::Call(_)).then_some(call)
}

/// `IsDefinedInCurrentOrOuterClass`: whether `method` is declared by the class of `current`
/// or one of its enclosing classes.
fn is_defined_in_current_or_outer_class(
    ctx: &Context<'_, '_, '_>,
    method: MethodDefId,
    current: MethodDefId,
) -> bool {
    if method.module != current.module {
        return false;
    }
    let md = &ctx.ts.comp.module(method.module).md;
    let Ok(declaring) = md.method_declaring_type(method.row) else {
        return false;
    };
    let mut ty = md.method_declaring_type(current.row).unwrap_or(0);
    while ty != 0 {
        if ty == declaring {
            return true;
        }
        ty = ctx
            .ts
            .comp
            .type_def(TypeDefId {
                module: current.module,
                row: ty,
            })
            .map_or(0, |i| i.declaring);
    }
    false
}
