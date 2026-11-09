//! `IL/Transforms/InlineArrayTransform.cs`: the compiler's inline array helpers of
//! `<PrivateImplementationDetails>` become `ldelema.inlinearray` element addresses.

use crate::il::ast::{Function, Inst, NodeId};
use crate::typesystem::attrvalue::ArgValue;
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::resolve::ResolvedMethod;
use crate::typesystem::types::{TypeData, TypeId};

use super::Context;
use super::il_inlining::known;

/// `RunOnExpression`.
pub fn run_on_expression(f: &mut Function, ctx: &mut Context<'_, '_, '_>, inst: NodeId) -> bool {
    let matched = match_span_indexer_with_inline_array_as_span(f, ctx, inst)
        .or_else(|| match_inline_array_element_ref(f, ctx, inst))
        .map(|(ty, addr, index, ro)| (ty, addr, Some(index), ro))
        .or_else(|| {
            match_inline_array_first_element_ref(f, ctx, inst)
                .map(|(ty, addr, ro)| (ty, addr, None, ro))
        });
    let Some((ty, array, index, is_read_only)) = matched else {
        return false;
    };
    let index = index.unwrap_or_else(|| f.add(Inst::LdcI4 { value: 0 }));
    let new_inst = f.add(Inst::LdElemaInlineArray {
        ty,
        array,
        indices: vec![index],
        is_read_only,
    });
    f.add_il_range(new_inst, f.il_range(inst));
    f.replace(inst, new_inst);
    true
}

fn call_parts(f: &Function, inst: NodeId) -> Option<(ResolvedMethod, Vec<NodeId>)> {
    match f.inst(inst) {
        Inst::Call(c) => Some((c.method.method.clone(), c.args.clone())),
        _ => None,
    }
}

/// `IType.GetInlineArrayLength()`: the length of `[InlineArray(n)]` on a struct.
fn inline_array_length(ctx: &mut Context<'_, '_, '_>, t: TypeId) -> Option<i32> {
    let def = ctx.ts.definition(t)?;
    if ctx.ts.comp.type_def(def).ok()?.kind != TypeKind::Struct {
        return None;
    }
    let attributes = ctx.ts.type_attributes(def).ok()?;
    let attribute = attributes.into_iter().find(|a| {
        ctx.ts.full_name(a.ty) == "System.Runtime.CompilerServices.InlineArrayAttribute"
    })?;
    match attribute.fixed.first()?.value {
        ArgValue::Int32(n) => Some(n),
        _ => None,
    }
}

/// `MatchSpanGetItem`: the instance indexer getter of `System.Span<T>` or
/// `System.ReadOnlySpan<T>`.
fn match_span_get_item(
    ctx: &mut Context<'_, '_, '_>,
    m: &ResolvedMethod,
    code: KnownTypeCode,
) -> bool {
    !m.is_static && m.name == "get_Item" && m.declaring.is_some_and(|d| known(ctx, d) == Some(code))
}

/// `MatchInlineArrayHelper`: a static helper `name<TBuffer, TElement>` of
/// `<PrivateImplementationDetails>` taking `ref TBuffer` (and an `int` unless it returns the
/// first element); returns `TBuffer`.
fn match_inline_array_helper(
    ctx: &mut Context<'_, '_, '_>,
    m: &ResolvedMethod,
    name: &str,
) -> Option<TypeId> {
    let declaring = m.declaring?;
    if !m.is_static
        || m.name != name
        || !matches!(ctx.ts.data(declaring), TypeData::Def(_))
        || ctx.ts.full_name(declaring) != "<PrivateImplementationDetails>"
    {
        return None;
    }
    let type_arguments = m.method_args.clone().unwrap_or_else(|| m.type_args.clone());
    let &[buffer, _] = &type_arguments[..] else {
        return None;
    };
    let params = m.params().ok()?;
    let by_ref_buffer = |ctx: &mut Context<'_, '_, '_>, p: TypeId| matches!(*ctx.ts.data(p), TypeData::ByRef(e) if e == buffer);
    let ok = if name.contains("FirstElement") {
        matches!(params[..], [p] if by_ref_buffer(ctx, p))
    } else {
        match params[..] {
            [p, length] => {
                by_ref_buffer(ctx, p) && known(ctx, length) == Some(KnownTypeCode::Int32)
            }
            _ => false,
        }
    };
    ok.then_some(buffer)
}

/// `MatchSpanIndexerWithInlineArrayAsSpan`:
/// `call get_Item(addressof Span<T>(call InlineArrayAsSpan(addr, length)), index)`.
fn match_span_indexer_with_inline_array_as_span(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId, NodeId, bool)> {
    let (method, args) = call_parts(f, inst)?;
    let (is_read_only, helper) =
        if match_span_get_item(ctx, &method, KnownTypeCode::ReadOnlySpanOfT) {
            (true, "InlineArrayAsReadOnlySpan")
        } else if match_span_get_item(ctx, &method, KnownTypeCode::SpanOfT) {
            (false, "InlineArrayAsSpan")
        } else {
            return None;
        };
    let &[span, index] = &args[..] else {
        return None;
    };
    let Inst::AddressOf { value: target, .. } = *f.inst(span) else {
        return None;
    };
    let (target_method, target_args) = call_parts(f, target)?;
    let buffer = match_inline_array_helper(ctx, &target_method, helper)?;
    let &[addr, length] = &target_args[..] else {
        return None;
    };
    let length = f.match_ldc_i4(length)?;
    // An unknown length compares as false, so the length is accepted.
    if length < 0 || inline_array_length(ctx, buffer).is_some_and(|n| length > n) {
        return None;
    }
    Some((buffer, addr, index, is_read_only))
}

/// `MatchInlineArrayElementRef`: `call InlineArrayElementRef(ReadOnly)(addr, ldc.i4 index)`.
fn match_inline_array_element_ref(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId, NodeId, bool)> {
    let (method, args) = call_parts(f, inst)?;
    let &[addr, index] = &args[..] else {
        return None;
    };
    let index_value = f.match_ldc_i4(index)?;
    let (buffer, is_read_only) =
        if let Some(b) = match_inline_array_helper(ctx, &method, "InlineArrayElementRef") {
            (b, false)
        } else {
            (
                match_inline_array_helper(ctx, &method, "InlineArrayElementRefReadOnly")?,
                true,
            )
        };
    // An unknown length compares as false, so the index is accepted.
    if index_value < 0 || inline_array_length(ctx, buffer).is_some_and(|n| index_value >= n) {
        return None;
    }
    Some((buffer, addr, index, is_read_only))
}

/// `MatchInlineArrayFirstElementRef`: `call InlineArrayFirstElementRef(ReadOnly)(addr)`.
fn match_inline_array_first_element_ref(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> Option<(TypeId, NodeId, bool)> {
    let (method, args) = call_parts(f, inst)?;
    let &[addr] = &args[..] else {
        return None;
    };
    if let Some(buffer) = match_inline_array_helper(ctx, &method, "InlineArrayFirstElementRef") {
        return Some((buffer, addr, false));
    }
    let buffer = match_inline_array_helper(ctx, &method, "InlineArrayFirstElementRefReadOnly")?;
    Some((buffer, addr, true))
}
