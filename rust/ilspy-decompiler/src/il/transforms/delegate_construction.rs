//! `IL/Transforms/DelegateConstruction.cs`: a delegate construction of an anonymous method,
//! `newobj D(target, ldftn M)`, becomes a nested function (a lambda) holding `M`'s body.
//!
//! The lambda is read into its own arena, the IL transforms before this one run over it there,
//! and it is then imported into the function's arena. The managed code runs those transforms
//! with the lambda already in the tree; the lambda only refers to its own variables until the
//! delegate target replaces its `this` afterwards, so the order makes no difference.

use crate::il::ast::{
    Function, FunctionKind, Inst, MethodOperand, NestedRead, NodeId, VarId, VariableKind,
};
use crate::il::reader::{ReadFunction, read_il_in_context};
use crate::metadata::tables::{TableId, rows};
use crate::typesystem::compilation::{TypeDefId, TypeKind};
use crate::typesystem::members::{MethodDefId, has_body};
use crate::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use crate::typesystem::types::{GenericContext, TypeData, TypeId};

use super::local_function_decompiler::is_local_function_method;
use super::transform_collection_and_object_initializers::contains_anonymous_type;
use super::{Context, DELEGATE_CONSTRUCTION_LEAF, combine_exits_transform, run_prefix};

/// The parts of a delegate construction (`MatchDelegateConstruction`'s out parameters).
pub(crate) struct DelegateConstructionParts {
    pub method: MethodOperand,
    pub target: NodeId,
    /// `None` for a constructor without a declaring type (the unknown type).
    pub delegate_type: Option<TypeId>,
}

/// `MatchDelegateConstruction(inst, ...)`: `newobj D(target, ldftn M)` or
/// `newobj D(target, ldvirtftn M)` of a delegate type (or an unknown one).
// `allowTransformed` has no effect here: DelegateConstruction replaces the whole `newobj`, so
// no `newobj D(target, function)` reaches the transforms that pass it.
pub(crate) fn match_delegate_construction(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
    delegate_construction_parts(f, ctx, inst).is_some()
}

/// `MatchDelegateConstruction` with its out parameters.
pub(crate) fn delegate_construction_parts(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<DelegateConstructionParts> {
    let parts = match f.inst(inst) {
        Inst::LdVirtDelegate { arg, ty, method } => DelegateConstructionParts {
            method: method.clone(),
            target: *arg,
            delegate_type: Some(*ty),
        },
        Inst::NewObj(call) => {
            let [target, function] = call.args[..] else {
                return None;
            };
            let (Inst::LdFtn { method } | Inst::LdVirtFtn { method, .. }) = f.inst(function) else {
                return None;
            };
            DelegateConstructionParts {
                method: method.clone(),
                target,
                delegate_type: call.method.method.declaring,
            }
        }
        _ => return None,
    };
    // A method without a declaring type has the unknown type there.
    match parts.delegate_type {
        Some(t) if !is_delegate_or_unknown(ctx, t) => None,
        _ => Some(parts),
    }
}

/// `delegateType.Kind == TypeKind.Delegate || delegateType.Kind == TypeKind.Unknown`.
fn is_delegate_or_unknown(ctx: &mut Context<'_, '_, '_>, mut delegate_type: TypeId) -> bool {
    // A parameterized type has the kind of its generic type.
    while let TypeData::Inst { generic, .. } | TypeData::Annotated { inner: generic, .. } =
        *ctx.ts.data(delegate_type)
    {
        delegate_type = generic;
    }
    if matches!(
        ctx.ts.data(delegate_type),
        TypeData::Unknown { .. } | TypeData::UnknownSpecial
    ) {
        return true;
    }
    is_delegate_type(ctx, delegate_type)
}

fn is_delegate_type(ctx: &Context<'_, '_, '_>, t: TypeId) -> bool {
    ctx.ts
        .definition(t)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == TypeKind::Delegate)
}

/// `IILTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    if !ctx.settings.anonymous_methods {
        return;
    }
    run_on(r, ctx, None, &mut Vec::new());
}

/// `Run` on the top-level function (`None`) or on a lambda of its arena; `active` holds the
/// methods whose lambdas are being built (`activeMethods`).
fn run_on(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    scope: Option<NodeId>,
    active: &mut Vec<MethodDefId>,
) {
    let method = function_method(r, scope);
    active.push(method);
    // `decompilationContext.CurrentTypeDefinition`.
    let decompiled_type = declaring_type(ctx, method);
    let root = scope.or(r.function.body);
    for inst in root.map_or_else(Vec::new, |root| r.function.descendants_post(root)) {
        let Some(parts) = delegate_construction_parts(&r.function, ctx, inst) else {
            continue;
        };
        if transform_delegate_construction(r, ctx, scope, active, decompiled_type, inst, &parts)
            .is_none()
        {
            continue;
        }
        let f = &mut r.function;
        let (Inst::LdLoc { var } | Inst::LdLoca { var }) = *f.inst(parts.target) else {
            continue;
        };
        if f.var(var).kind == VariableKind::Local {
            f.var_mut(var).kind = VariableKind::DisplayClassLocal;
        }
        let v = f.var(var);
        if v.is_single_definition()
            && let [store] = v.store_instructions[..]
            && let Some((_, value)) = f.match_stloc(store)
            && matches!(f.inst(value), Inst::NewObj(_))
        {
            let scope = closest_container(f, store);
            f.var_mut(var).capture_scope = scope;
        }
    }
    active.pop();
}

/// `TransformDelegateConstruction`: the lambda that replaced `value`, if any.
fn transform_delegate_construction(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    scope: Option<NodeId>,
    active: &mut Vec<MethodDefId>,
    decompiled_type: Option<TypeDefId>,
    value: NodeId,
    parts: &DelegateConstructionParts,
) -> Option<NodeId> {
    let mut target_method = parts.method.method.clone();
    if !is_anonymous_method(ctx, decompiled_type, &target_method) {
        return None;
    }
    let MethodOrigin::Def(mut id) = target_method.origin else {
        return None;
    };
    if is_local_function_method(ctx.ts, id) {
        return None;
    }
    if !validate_delegate_target(&r.function, parts.target) {
        return None;
    }
    if active.contains(&id) {
        warnings(r, scope).push(
            " Found self-referencing delegate construction. Abort transformation to avoid stack overflow."
                .to_owned(),
        );
        return None;
    }
    let mut lambda = read_delegate(ctx, id, &target_method)?;
    if let Some(unwrapped) = try_unwrap_visual_basic_delegate_wrapper(ctx, &lambda, &target_method)
    {
        let MethodOrigin::Def(unwrapped_id) = unwrapped.origin else {
            return None;
        };
        target_method = unwrapped;
        id = unwrapped_id;
        lambda = read_delegate(ctx, id, &target_method)?;
    }
    for &v in &lambda.variables {
        let var = lambda.function.var_mut(v);
        if var.kind != VariableKind::Parameter {
            var.name = format!("{}{}", target_method.name, var.name);
        }
    }
    let this_variable = lambda.variables.iter().copied().find(|&v| {
        let var = lambda.function.var(v);
        var.kind == VariableKind::Parameter && var.index == Some(-1)
    });
    {
        let mut nested = Context {
            ts: &mut *ctx.ts,
            settings: ctx.settings,
            reader: ctx.reader,
            rerun_requested: false,
            removed_variables: Vec::new(),
            resolver: ctx.resolver,
        };
        run_prefix(&mut lambda, &mut nested, DELEGATE_CONSTRUCTION_LEAF);
        combine_exits_transform::run(&mut lambda, &mut nested);
        lambda.flush_pending_variables();
    }
    let (function, this_variable) = import_lambda(
        r,
        lambda,
        parts.delegate_type,
        &target_method,
        this_variable,
    );
    r.function.replace(value, function);
    if let Inst::Function { body, .. } = *r.function.inst(function) {
        r.function.release_root(body);
    }
    replace_delegate_target(&mut r.function, function, parts.target, this_variable);
    // Nested lambdas.
    run_on(r, ctx, Some(function), active);
    let function = match parts.delegate_type {
        Some(t) => {
            try_unwrap_transformed_visual_basic_delegate_wrapper(ctx, &mut r.function, function, t)
                .unwrap_or(function)
        }
        None => function,
    };
    let f = &mut r.function;
    let (target_range, value_range) = (f.il_range(parts.target), f.il_range(value));
    f.add_il_range(function, target_range);
    f.add_il_range(function, value_range);
    Some(function)
}

/// `ILReader.ReadIL` of a delegate's method in the context its type arguments give
/// (`GenericContextFromTypeArguments`), or `None` when they are not all type parameters.
fn read_delegate(
    ctx: &mut Context<'_, '_, '_>,
    id: MethodDefId,
    method: &ResolvedMethod,
) -> Option<ReadFunction> {
    let md = &ctx.ts.comp.module(id.module).md;
    let def = md.get::<rows::MethodDef>(id.row).ok()?;
    if !has_body(&def) {
        return None;
    }
    let is_type_parameter =
        |ts: &crate::typesystem::types::TypeSystem<'_, '_>, t: TypeId| match *ts.data(t) {
            TypeData::TypeParam { .. } => true,
            TypeData::Annotated { inner, .. } => {
                matches!(ts.data(inner), TypeData::TypeParam { .. })
            }
            _ => false,
        };
    let class = method.class_args.clone().unwrap_or_default();
    let method_args = method.method_args.clone().unwrap_or_default();
    if !class
        .iter()
        .chain(&method_args)
        .all(|&t| is_type_parameter(ctx.ts, t))
    {
        return None;
    }
    let context = ctx.ts.explicit_context(class, method_args);
    let body = md.method_body(def.rva).ok()?;
    let mut read = read_il_in_context(ctx.ts, id, body, ctx.reader, Some(context)).ok()?;
    read.kind = FunctionKind::Delegate;
    Some(read)
}

/// Moves `lambda` into `r`'s arena as a delegate function (not yet in the tree, its body
/// still holding the reference of its old root). Returns it and its `this` variable.
fn import_lambda(
    r: &mut ReadFunction,
    mut lambda: ReadFunction,
    delegate_type: Option<TypeId>,
    method: &ResolvedMethod,
    this_variable: Option<VarId>,
) -> (NodeId, Option<VarId>) {
    let body = lambda.function.body.expect("a read function has a body");
    let captured = std::mem::take(&mut lambda.function.captured_variables);
    let helper_variable_count = lambda.function.helper_variable_count;
    let by = r.function.import(std::mem::take(&mut lambda.function));
    let read = NestedRead {
        method: lambda.method,
        context: context_index(&lambda.generic_context),
        code_size: lambda.code_size,
        local_signature_length: lambda.local_signature_length,
        async_return_type: lambda.async_return_type,
        is_iterator: lambda.is_iterator,
        state_machine_compiled_with_mono: lambda.state_machine_compiled_with_mono,
        state_machine_compiled_with_legacy_visual_basic: lambda
            .state_machine_compiled_with_legacy_visual_basic,
        move_next_method: lambda.move_next_method,
        async_debug_info: lambda.async_debug_info,
        warnings: lambda.warnings,
        sequence_point_candidates: lambda.sequence_point_candidates,
        method_operand: None,
    };
    let parameters = method
        .params()
        .unwrap_or_default()
        .into_iter()
        .map(|t| (t, String::new()))
        .collect();
    let function = r.function.add(Inst::Function {
        kind: FunctionKind::Delegate,
        delegate_type,
        return_type: method.ret,
        parameters,
        variables: lambda.variables.iter().map(|&v| by.var(v)).collect(),
        helper_variable_count,
        body: by.node(body),
        read: Some(Box::new(read)),
        local_functions: Vec::new(),
        declaration_scope: None,
        reduced: None,
    });
    for (owner, v) in captured {
        let owner = owner.map_or(function, |n| by.node(n));
        r.function.add_captured_variable(Some(owner), by.var(v));
    }
    (function, this_variable.map(|v| by.var(v)))
}

/// `MethodOperand::context` of the context a function was read in.
fn context_index(context: &GenericContext) -> u32 {
    match (context.explicit, context.class_tail) {
        (Some(e), _) => e + 2,
        (None, Some(_)) => 1,
        (None, None) => 0,
    }
}

/// `ReplaceDelegateTargetVisitor`: loads of `this_variable` (and `ldobj(ldloca this)`, as async
/// delegates use) in `function` become copies of `target`; the variable holding the target
/// becomes one of `function`'s captured variables.
pub(crate) fn replace_delegate_target(
    f: &mut Function,
    function: NodeId,
    target: NodeId,
    this_variable: Option<VarId>,
) {
    let Some(this_variable) = this_variable else {
        return;
    };
    if let Some(v) = target_variable(f, target) {
        f.add_captured_variable(Some(function), v);
    }
    let mut loads = Vec::new();
    let mut stack = vec![function];
    while let Some(n) = stack.pop() {
        let is_load = match *f.inst(n) {
            Inst::LdLoc { var } => var == this_variable,
            Inst::LdObj { target, .. } => {
                matches!(*f.inst(target), Inst::LdLoca { var } if var == this_variable)
            }
            _ => false,
        };
        if is_load {
            loads.push(n);
        } else {
            stack.extend(f.inst(n).children().into_iter().rev());
        }
    }
    for load in loads {
        let copy = f.clone_subtree(target);
        f.replace(load, copy);
    }
}

/// The display class variable a delegate target loads: `ldloc v`, or the `ldloc v` at the end
/// of `ldobj(ldflda field(... ldloc v))`.
fn target_variable(f: &Function, target: NodeId) -> Option<VarId> {
    match *f.inst(target) {
        Inst::LdLoc { var } => Some(var),
        Inst::LdObj {
            target: mut inner, ..
        } => {
            while let Inst::LdFlda { target, .. } = *f.inst(inner) {
                inner = target;
            }
            match *f.inst(inner) {
                Inst::LdLoc { var } => Some(var),
                _ => None,
            }
        }
        _ => None,
    }
}

/// `IsAnonymousMethod`.
fn is_anonymous_method(
    ctx: &mut Context<'_, '_, '_>,
    decompiled_type: Option<TypeDefId>,
    method: &ResolvedMethod,
) -> bool {
    if method.name.starts_with('<') || method.name.contains('$') {
        return true;
    }
    if let MethodOrigin::Def(id) = method.origin
        && ctx
            .ts
            .has_compiler_generated(id.module, TableId::MethodDef, id.row)
    {
        return true;
    }
    if let Some(display) = method.declaring.and_then(|d| ctx.ts.definition(d))
        && ctx.ts.is_potential_closure_type(decompiled_type, display)
    {
        return true;
    }
    // `ContainsAnonymousType`.
    let types: Vec<TypeId> = std::iter::once(method.ret)
        .chain(method.params().unwrap_or_default())
        .collect();
    types.into_iter().any(|t| contains_anonymous_type(ctx, t))
}

/// `ValidateDelegateTarget`.
fn validate_delegate_target(f: &Function, inst: NodeId) -> bool {
    match *f.inst(inst) {
        Inst::LdNull => true,
        Inst::LdLoc { var } => f.var(var).is_single_definition(),
        Inst::LdObj { target, .. } => {
            if matches!(f.inst(target), Inst::LdsFlda { .. }) {
                return true;
            }
            // Delegate targets stored in nested display classes:
            // ldobj(ldflda field(ldobj(ldflda field(ldloc displayClass)))).
            let mut current = inst;
            while let Inst::LdObj { target, .. } | Inst::LdFlda { target, .. } = *f.inst(current) {
                current = target;
            }
            matches!(f.inst(current), Inst::LdLoc { .. })
        }
        _ => false,
    }
}

/// `TryUnwrapVisualBasicDelegateWrapper`: the method a VB `$__R` wrapper only forwards to.
fn try_unwrap_visual_basic_delegate_wrapper(
    ctx: &mut Context<'_, '_, '_>,
    function: &ReadFunction,
    wrapper: &ResolvedMethod,
) -> Option<ResolvedMethod> {
    if !wrapper.name.contains("$__R") {
        return None;
    }
    let f = &function.function;
    let this_variable = function.variables.iter().copied().find(|&v| {
        let var = f.var(v);
        var.kind == VariableKind::Parameter && var.index == Some(-1)
    })?;
    let container = f.body?;
    let block = *f.list(container).first()?;
    let &[first, leave] = f.list(block) else {
        return None;
    };
    if f.match_leave(leave).map(|(c, _)| c) != Some(container) {
        return None;
    }
    let is_this = |n: NodeId| matches!(*f.inst(n), Inst::LdLoc { var } if var == this_variable);
    let unwrapped = match f.inst(first) {
        Inst::Call(call) => {
            if call.args.len() != 1 || !is_this(call.args[0]) {
                return None;
            }
            call.method.method.clone()
        }
        Inst::CallVirt(call) => {
            let declaring = call.method.method.declaring?;
            if call.method.method.name != "Invoke"
                || !is_delegate_type(ctx, declaring)
                || call.args.len() != 1
            {
                return None;
            }
            let parts = delegate_construction_parts(f, ctx, call.args[0])?;
            if !is_this(parts.target) {
                return None;
            }
            parts.method.method
        }
        _ => return None,
    };
    let definition = |m: &ResolvedMethod| m.declaring.and_then(|d| ctx.ts.definition(d));
    let wrapper_type = definition(wrapper);
    if definition(&unwrapped) != wrapper_type {
        return None;
    }
    is_anonymous_method(ctx, wrapper_type, &unwrapped).then_some(unwrapped)
}

/// `TryUnwrapTransformedVisualBasicDelegateWrapper`: a lambda that only invokes a VB
/// anonymous delegate's lambda is replaced by that lambda.
fn try_unwrap_transformed_visual_basic_delegate_wrapper(
    ctx: &mut Context<'_, '_, '_>,
    f: &mut Function,
    function: NodeId,
    delegate_type: TypeId,
) -> Option<NodeId> {
    let Inst::Function { body, .. } = *f.inst(function) else {
        return None;
    };
    let block = *f.list(body).first()?;
    let &[first, leave] = f.list(block) else {
        return None;
    };
    if f.match_leave(leave).map(|(c, _)| c) != Some(body) {
        return None;
    }
    let Inst::CallVirt(call) = f.inst(first) else {
        return None;
    };
    let declaring = call.method.method.declaring?;
    if call.method.method.name != "Invoke" || !is_delegate_type(ctx, declaring) {
        return None;
    }
    let &[inner] = &call.args[..] else {
        return None;
    };
    let Inst::Function {
        delegate_type: Some(inner_type),
        ..
    } = *f.inst(inner)
    else {
        return None;
    };
    if !type_name(ctx, inner_type).is_some_and(|n| n.starts_with("VB$AnonymousDelegate_")) {
        return None;
    }
    if let Inst::Function {
        delegate_type: d, ..
    } = f.inst_mut(inner)
    {
        *d = Some(delegate_type);
    }
    f.replace(function, inner);
    // The wrapper's captured variables go with it.
    f.captured_variables
        .retain(|&(owner, _)| owner != Some(function));
    Some(inner)
}

/// `IType.Name` of a type definition or instance (the metadata name).
fn type_name(ctx: &Context<'_, '_, '_>, t: TypeId) -> Option<String> {
    let def = ctx.ts.definition(t)?;
    let md = &ctx.ts.comp.module(def.module).md;
    md.string(md.get::<rows::TypeDef>(def.row).ok()?.name)
        .ok()
        .map(|n| n.into_owned())
}

/// The method of the top-level function or of a lambda.
fn function_method(r: &ReadFunction, scope: Option<NodeId>) -> MethodDefId {
    match scope.map(|n| r.function.inst(n)) {
        Some(Inst::Function {
            read: Some(read), ..
        }) => read.method,
        _ => r.method,
    }
}

/// `function.Warnings` of the top-level function or of a lambda.
fn warnings(r: &mut ReadFunction, scope: Option<NodeId>) -> &mut Vec<String> {
    if let Some(n) = scope
        && let Inst::Function {
            read: Some(read), ..
        } = r.function.inst_mut(n)
    {
        return &mut read.warnings;
    }
    &mut r.warnings
}

fn declaring_type(ctx: &Context<'_, '_, '_>, method: MethodDefId) -> Option<TypeDefId> {
    let md = &ctx.ts.comp.module(method.module).md;
    let row = md.method_declaring_type(method.row).ok()?;
    (row != 0).then_some(TypeDefId {
        module: method.module,
        row,
    })
}

/// `BlockContainer.FindClosestContainer`.
fn closest_container(f: &Function, inst: NodeId) -> Option<NodeId> {
    std::iter::once(inst)
        .chain(f.ancestors(inst))
        .find(|&n| matches!(f.inst(n), Inst::BlockContainer { .. }))
}
