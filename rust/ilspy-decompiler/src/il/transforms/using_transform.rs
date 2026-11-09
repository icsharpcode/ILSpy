//! `IL/Transforms/UsingTransform.cs`: a resource stored before a try-finally that disposes
//! it becomes a `using` instruction.

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::inheritance::MemberDef;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::lookup::MemberOptions;
use crate::typesystem::members::SymbolKind;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{is_passed_to_in_parameter, is_used_as_this_pointer_in_call, known};
use super::pattern_matching_transform::{match_get_value_or_default_of, match_has_value_call_of};

/// `IBlockTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    if !ctx.settings.using_statement {
        return;
    }
    let f = &mut r.function;
    let block = bc.block;
    let mut i = bc.index_of_first_already_transformed_instruction;
    while i > 0 {
        i -= 1;
        if transform_using(f, ctx, block, i)
            || transform_using_vb(f, ctx, block, i)
            || transform_async_using(f, ctx, block, i)
        {
            bc.index_of_first_already_transformed_instruction = f.list(block).len();
        }
    }
}

/// The instructions of `id` when it is a block.
fn block_instructions(f: &Function, id: NodeId) -> Option<&[NodeId]> {
    matches!(f.inst(id), Inst::Block { .. }).then(|| f.list(id))
}

fn is_descendant_of(f: &Function, inst: NodeId, ancestor: NodeId) -> bool {
    inst == ancestor || f.ancestors(inst).any(|a| a == ancestor)
}

/// `IMethod.FullName`.
fn method_full_name(ctx: &mut Context<'_, '_, '_>, m: &ResolvedMethod) -> String {
    match m.declaring {
        Some(d) => format!("{}.{}", ctx.ts.full_name(d), m.name),
        None => m.name.clone(),
    }
}

/// `NullableType.GetUnderlyingType`.
fn nullable_underlying(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> TypeId {
    if let TypeData::Inst { generic, args } = ctx.ts.data(t)
        && args.len() == 1
    {
        let (generic, arg) = (*generic, args[0]);
        if known(ctx, generic) == Some(KnownTypeCode::NullableOfT) {
            return arg;
        }
    }
    t
}

/// `IType.Kind == TypeKind.Struct`.
fn is_struct(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    ctx.ts
        .definition(t)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == TypeKind::Struct)
}

/// `IType.GetAllBaseTypes().Any(b => b.IsKnownType(code))`.
fn has_base_type(ctx: &mut Context<'_, '_, '_>, t: TypeId, code: KnownTypeCode) -> bool {
    let bases = ctx.ts.all_base_types(t).unwrap_or_default();
    bases.into_iter().any(|b| known(ctx, b) == Some(code))
}

/// The methods named `name` without type parameters and parameters.
fn parameterless_methods(
    ctx: &mut Context<'_, '_, '_>,
    t: TypeId,
    name: &str,
) -> Vec<ResolvedMethod> {
    let methods = ctx
        .ts
        .get_methods(t, None, MemberOptions::default())
        .unwrap_or_default();
    methods
        .into_iter()
        .filter(|m| {
            m.name == name && m.type_args.is_empty() && m.params().is_ok_and(|p| p.is_empty())
        })
        .collect()
}

/// `CheckResourceType`.
fn check_resource_type(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    // The non-generic IEnumerator is no IDisposable; it stands for the non-generic foreach.
    if known(ctx, t) == Some(KnownTypeCode::IEnumerator)
        || has_base_type(ctx, t, KnownTypeCode::IEnumerator)
    {
        return true;
    }
    let underlying = nullable_underlying(ctx, t);
    if has_base_type(ctx, underlying, KnownTypeCode::IDisposable) {
        return true;
    }
    if ctx.settings.introduce_ref_modifiers_on_structs
        && is_struct(ctx, t)
        && ctx.ts.is_by_ref_like(t)
    {
        return true;
    }
    // The GetEnumerator pattern.
    parameterless_methods(ctx, t, "GetEnumerator")
        .into_iter()
        .any(|m| implements_foreach_pattern(ctx, m.ret))
}

/// `ImplementsForeachPattern`.
fn implements_foreach_pattern(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> bool {
    let move_next = parameterless_methods(ctx, t, "MoveNext");
    if !move_next
        .into_iter()
        .any(|m| known(ctx, m.ret) == Some(KnownTypeCode::Boolean))
    {
        return false;
    }
    let properties = ctx
        .ts
        .get_properties(t, MemberOptions::default())
        .unwrap_or_default();
    properties.into_iter().any(|p| {
        let MemberDef::Property(module, row) = p.def else {
            return false;
        };
        let can_get = ctx.ts.comp.module(module).md.property_accessors(row).0 != 0;
        can_get
            && ctx.ts.member_name(p.def).is_ok_and(|n| n == "Current")
            && ctx.ts.property_symbol_kind(module, row).ok() != Some(SymbolKind::Indexer)
    })
}

/// `CheckAsyncResourceType`: the full name of the dispose method.
fn check_async_resource_type(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<String> {
    let t = nullable_underlying(ctx, t);
    if has_base_type(ctx, t, KnownTypeCode::IAsyncDisposable) {
        return Some("System.IAsyncDisposable.DisposeAsync".into());
    }
    let methods = parameterless_methods(ctx, t, "DisposeAsync");
    match &methods[..] {
        [] => None,
        [m] => Some(method_full_name(ctx, m)),
        // `SingleOrDefault` throws on more than one match.
        _ => panic!("more than one DisposeAsync method"),
    }
}

/// The checks the three patterns share for the resource variable `v` and the try-finally:
/// a local, loaded only inside the try-finally, stored at most once.
fn resource_variable_ok(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    v: VarId,
    try_finally: NodeId,
    in_parameters: bool,
) -> bool {
    let var = f.var(v);
    if var.kind != VariableKind::Local {
        return false;
    }
    if var
        .load_instructions
        .iter()
        .any(|&ld| !is_descendant_of(f, ld, try_finally))
    {
        return false;
    }
    let Inst::TryFinally { try_block, .. } = *f.inst(try_finally) else {
        unreachable!("a try-finally")
    };
    for &la in &var.address_instructions {
        if !is_descendant_of(f, la, try_finally) {
            return false;
        }
        if is_descendant_of(f, la, try_block)
            && !(is_used_as_this_pointer_in_call(f, ctx, la)
                || (in_parameters && is_passed_to_in_parameter(f, ctx, la)))
        {
            return false;
        }
    }
    var.store_instructions.len() <= 1
}

/// `using (v = value) try_block` with the IL range of `store`, if given.
fn new_using(
    f: &mut Function,
    v: VarId,
    value: NodeId,
    try_block: NodeId,
    is_async: bool,
    is_ref_struct: bool,
    store: Option<NodeId>,
) -> NodeId {
    f.var_mut(v).kind = VariableKind::UsingLocal;
    let using = f.add(Inst::Using {
        variable: v,
        resource_expression: value,
        body: try_block,
        is_async,
        is_ref_struct,
    });
    if let Some(store) = store {
        f.add_il_range(using, f.il_range(store));
    }
    using
}

/// `TransformUsing`: `stloc v(value); try { ... } finally { v.Dispose() }`.
fn transform_using(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    let instructions = f.list(block);
    if i + 1 >= instructions.len() {
        return false;
    }
    let (store, try_finally) = (instructions[i], instructions[i + 1]);
    let Inst::TryFinally {
        try_block,
        finally_block,
    } = *f.inst(try_finally)
    else {
        return false;
    };
    let Some((v, value)) = f.match_stloc(store) else {
        return false;
    };
    let using_null = matches!(f.inst(value), Inst::LdNull);
    let ty = f.var(v).ty;
    if !(using_null || check_resource_type(ctx, ty)) {
        return false;
    }
    if !resource_variable_ok(f, ctx, v, try_finally, true) {
        return false;
    }
    if !matches!(f.inst(finally_block), Inst::BlockContainer { .. })
        || !match_dispose_block(
            f,
            ctx,
            finally_block,
            v,
            using_null,
            "System.IDisposable.Dispose",
            KnownTypeCode::IDisposable,
        )
    {
        return false;
    }
    let is_ref_struct = ctx.settings.introduce_ref_modifiers_on_structs
        && is_struct(ctx, ty)
        && ctx.ts.is_by_ref_like(ty);
    f.remove_at(block, i + 1);
    let using = new_using(f, v, value, try_block, false, is_ref_struct, Some(store));
    f.set_child_at(block, i, using);
    true
}

/// `TransformUsingVB`: `try { stloc v(value); ... } finally { v.Dispose() }`.
fn transform_using_vb(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    let Some(&try_finally) = f.list(block).get(i) else {
        return false;
    };
    let Inst::TryFinally {
        try_block,
        finally_block,
    } = *f.inst(try_finally)
    else {
        return false;
    };
    if !matches!(f.inst(try_block), Inst::BlockContainer { .. }) {
        return false;
    }
    let entry = f.list(try_block)[0];
    let Some((v, value)) = f.list(entry).first().and_then(|&s| f.match_stloc(s)) else {
        return false;
    };
    let using_null = matches!(f.inst(value), Inst::LdNull);
    if !(using_null || check_resource_type(ctx, f.var(v).ty)) {
        return false;
    }
    if !resource_variable_ok(f, ctx, v, try_finally, false) {
        return false;
    }
    if !matches!(f.inst(finally_block), Inst::BlockContainer { .. })
        || !match_dispose_block(
            f,
            ctx,
            finally_block,
            v,
            using_null,
            "System.IDisposable.Dispose",
            KnownTypeCode::IDisposable,
        )
    {
        return false;
    }
    f.remove_at(entry, 0);
    let using = new_using(f, v, value, try_block, false, false, None);
    f.set_child_at(block, i, using);
    true
}

/// `TransformAsyncUsing`: `stloc v(value); try { ... } finally { await v.DisposeAsync() }`.
fn transform_async_using(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
) -> bool {
    if !ctx.settings.async_using_and_for_each_statement {
        return false;
    }
    let instructions = f.list(block);
    if i < 1 || i >= instructions.len() {
        return false;
    }
    let (store, try_finally) = (instructions[i - 1], instructions[i]);
    let Inst::TryFinally {
        try_block,
        finally_block,
    } = *f.inst(try_finally)
    else {
        return false;
    };
    let Some((v, value)) = f.match_stloc(store) else {
        return false;
    };
    let Some(dispose_method) = check_async_resource_type(ctx, f.var(v).ty) else {
        return false;
    };
    if !resource_variable_ok(f, ctx, v, try_finally, false) {
        return false;
    }
    if !matches!(f.inst(finally_block), Inst::BlockContainer { .. })
        || !match_dispose_block(
            f,
            ctx,
            finally_block,
            v,
            false,
            &dispose_method,
            KnownTypeCode::IAsyncDisposable,
        )
    {
        return false;
    }
    f.remove_at(block, i);
    let using = new_using(f, v, value, try_block, true, false, Some(store));
    f.set_child_at(block, i - 1, using);
    true
}

/// `MatchDisposeBlock`: the finally block disposes `obj` and leaves its container.
fn match_dispose_block(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    container: NodeId,
    mut obj: VarId,
    using_null: bool,
    dispose_method: &str,
    dispose_type: KnownTypeCode,
) -> bool {
    let entry = f.list(container)[0];
    if f.node(entry).incoming_edge_count != 1 {
        return false;
    }
    let instructions = f.list(entry);
    let mut pos = 0;
    let obj_type = f.var(obj).ty;
    let mut is_reference = ctx.ts.is_reference_type(obj_type) != Some(false);
    // Optional: stloc temp(isinst TDisposable(ldloc obj)).
    if let Some((temp, isinst)) = instructions.first().and_then(|&s| f.match_stloc(s)) {
        let Inst::IsInst { arg, ty } = *f.inst(isinst) else {
            return false;
        };
        if f.match_ldloc(arg) != Some(obj) || known(ctx, ty.ty) != Some(dispose_type) {
            return false;
        }
        if !f.var(temp).is_single_definition() {
            return false;
        }
        is_reference = true;
        pos += 1;
        obj = temp;
    }
    let Some(&check) = instructions.get(pos) else {
        return false;
    };
    let Some(loads_in_check) = match_dispose_check(
        f,
        ctx,
        obj,
        check,
        is_reference,
        using_null,
        dispose_method,
        dispose_type,
    ) else {
        return false;
    };
    // The optional temporary is used only in the dispose check.
    if pos > 0 && f.var(obj).load_count() != loads_in_check {
        return false;
    }
    pos += 1;
    let leaves = instructions
        .get(pos)
        .and_then(|&l| f.match_leave(l))
        .is_some_and(|(target, value)| target == container && f.match_nop(value));
    leaves && pos + 1 == instructions.len()
}

/// `UnwrapAwait`: the argument of `await`, without an `addressof`.
fn unwrap_await(f: &Function, inst: NodeId) -> Option<NodeId> {
    let Inst::Await { value, .. } = *f.inst(inst) else {
        return None;
    };
    Some(match *f.inst(value) {
        Inst::AddressOf { value: inner, .. } => inner,
        _ => value,
    })
}

/// `MatchDisposeCheck`: the number of loads of `obj` in the check when `check` disposes it.
#[allow(clippy::too_many_arguments)]
fn match_dispose_check(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    mut obj: VarId,
    check: NodeId,
    is_reference: bool,
    using_null: bool,
    dispose_method: &str,
    dispose_type: KnownTypeCode,
) -> Option<u32> {
    let is_async = dispose_type == KnownTypeCode::IAsyncDisposable;
    let obj_type = f.var(obj).ty;
    if known(ctx, obj_type) == Some(KnownTypeCode::NullableOfT) {
        let mut invocation = if let Some((condition, dispose_inst)) = f.match_if_instruction(check)
        {
            if !match_has_value_call_of(f, ctx, condition, obj) {
                return None;
            }
            let &[invocation] = block_instructions(f, dispose_inst)? else {
                return None;
            };
            invocation
        } else if let Inst::NullableRewrap { arg, .. } = *f.inst(check) {
            arg
        } else {
            return None;
        };
        if is_async {
            invocation = unwrap_await(f, invocation)?;
        }
        let Inst::CallVirt(call) = f.inst(invocation) else {
            return None;
        };
        if method_full_name(ctx, &call.method.method) != dispose_method
            || !call.method.method.params().is_ok_and(|p| p.is_empty())
        {
            return None;
        }
        let &[first] = &call.args[..] else {
            return None;
        };
        let matched = match *f.inst(first) {
            Inst::UnboxAny { arg, ty } if known(ctx, ty.ty) == Some(dispose_type) => {
                let Inst::Box { arg: boxed, ty } = *f.inst(arg) else {
                    return None;
                };
                let same_underlying =
                    nullable_underlying(ctx, ty.ty) == nullable_underlying(ctx, obj_type);
                known(ctx, ty.ty) == Some(KnownTypeCode::NullableOfT)
                    && same_underlying
                    && f.match_ldloc(boxed) == Some(obj)
            }
            Inst::AddressOf { value, .. } => {
                match_get_value_or_default_of(f, ctx, value, obj)
                    || matches!(*f.inst(value), Inst::NullableUnwrap { arg, .. }
                        if f.match_ldloc(arg) == Some(obj))
            }
            _ => false,
        };
        return matched.then_some(2);
    }
    let mut target;
    let mut boxed_value = false;
    let dispose_call;
    let mut dispose_method = dispose_method.to_owned();
    let mut obj_loads = 2;
    let is_rewrap = matches!(f.inst(check), Inst::NullableRewrap { .. });
    if is_reference && let Inst::NullableRewrap { arg, .. } = *f.inst(check) {
        // The null check of a reference type may have become `obj?.Dispose();`.
        let Inst::CallVirt(cv) = f.inst(arg) else {
            return None;
        };
        target = *cv.args.first()?;
        if let Inst::LdObjIfRef { target: t, .. } = *f.inst(target) {
            target = t;
        }
        let Inst::NullableUnwrap { arg: unwrapped, .. } = *f.inst(target) else {
            return None;
        };
        obj_loads = 1;
        dispose_call = cv;
        target = unwrapped;
    } else if is_reference {
        // Reference types have a null check.
        let (condition, dispose_inst) = f.match_if_instruction(check)?;
        let is_inlined_isinst =
            match_null_check_or_type_check(f, ctx, condition, &mut obj, dispose_type)?;
        let &[mut invocation] = block_instructions(f, dispose_inst)? else {
            return None;
        };
        if is_async {
            invocation = unwrap_await(f, invocation)?;
        }
        let Inst::CallVirt(cv) = f.inst(invocation) else {
            return None;
        };
        target = *cv.args.first()?;
        if let Inst::LdObjIfRef { target: t, .. } = *f.inst(target) {
            target = t;
        }
        match *f.inst(target) {
            Inst::Box { arg, ty } if ty.ty == f.var(obj).ty => target = arg,
            Inst::IsInst { arg, ty }
                if is_inlined_isinst && known(ctx, ty.ty) == Some(dispose_type) =>
            {
                target = arg
            }
            _ => {}
        }
        dispose_call = cv;
    } else if is_struct(ctx, obj_type) && ctx.ts.is_by_ref_like(obj_type) {
        let Inst::Call(call) = f.inst(check) else {
            return None;
        };
        // The managed check compares type objects by reference: only a type definition (not a
        // parameterized type, which each signature creates anew) can be the same object.
        if call.method.method.declaring != Some(obj_type)
            || !matches!(ctx.ts.data(obj_type), TypeData::Def(_))
        {
            return None;
        }
        target = *call.args.first()?;
        if call.method.method.name != "Dispose" {
            return None;
        }
        dispose_method = method_full_name(ctx, &call.method.method);
        dispose_call = call;
    } else {
        let mut check = check;
        if is_async {
            check = unwrap_await(f, check)?;
        }
        let (Inst::Call(cv) | Inst::CallVirt(cv) | Inst::NewObj(cv)) = f.inst(check) else {
            return None;
        };
        target = *cv.args.first()?;
        if let Inst::Box { arg, ty } = *f.inst(target)
            && ty.ty == f.var(obj).ty
        {
            boxed_value = ctx.ts.is_reference_type(ty.ty) != Some(true);
            target = arg;
        }
        dispose_call = cv;
    }
    let method = &dispose_call.method.method;
    if method.is_static {
        return None;
    }
    if is_async {
        if method.name != "DisposeAsync" {
            return None;
        }
    } else if method_full_name(ctx, method) != dispose_method {
        return None;
    }
    if !method.params().is_ok_and(|p| p.is_empty()) || dispose_call.args.len() != 1 {
        return None;
    }
    let matched = f.match_ld_loc_ref(ctx.ts, target) == Some(obj)
        || (boxed_value && f.match_ldloc(target) == Some(obj))
        || (using_null && matches!(f.inst(dispose_call.args[0]), Inst::LdNull))
        || (is_reference
            && is_rewrap
            && matches!(*f.inst(target), Inst::IsInst { arg, ty }
                if f.match_ldloc(arg) == Some(obj) && known(ctx, ty.ty) == Some(dispose_type)));
    matched.then_some(obj_loads)
}

/// `MatchNullCheckOrTypeCheck`: whether the check tests an inlined `isinst`; `obj` becomes
/// the variable the dispose call uses.
fn match_null_check_or_type_check(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    condition: NodeId,
    obj: &mut VarId,
    dispose_type: KnownTypeCode,
) -> Option<bool> {
    if let Some((mut left, right)) = f.match_comp(condition, ComparisonKind::Inequality) {
        let mut is_inlined_isinst = false;
        if let Some((inline_var, inline_value)) = f.match_stloc(left) {
            let Inst::IsInst { arg, ty } = *f.inst(inline_value) else {
                return None;
            };
            if known(ctx, ty.ty) != Some(dispose_type) {
                return None;
            }
            let var = f.var(inline_var);
            if !var.is_single_definition() || var.load_count() != 1 {
                return None;
            }
            if known(ctx, var.ty) != Some(dispose_type) {
                return None;
            }
            if f.match_ldloc(arg) != Some(*obj) || !matches!(f.inst(right), Inst::LdNull) {
                return None;
            }
            *obj = inline_var;
            return Some(true);
        } else if let Inst::IsInst { arg, ty } = *f.inst(left)
            && known(ctx, ty.ty) == Some(dispose_type)
        {
            is_inlined_isinst = true;
            left = arg;
        }
        return (f.match_ldloc(left) == Some(*obj) && matches!(f.inst(right), Inst::LdNull))
            .then_some(is_inlined_isinst);
    }
    if let Inst::Match {
        check_not_null: true,
        check_type: true,
        tested_operand,
        variable,
        ..
    } = *f.inst(condition)
        && let Some(v) = f.match_ldloc(tested_operand)
    {
        if v != *obj || known(ctx, f.var(variable).ty) != Some(dispose_type) {
            return None;
        }
        *obj = variable;
        return Some(false);
    }
    None
}
