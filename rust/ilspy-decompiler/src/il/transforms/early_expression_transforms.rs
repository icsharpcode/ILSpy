//! `IL/Transforms/EarlyExpressionTransforms.cs`: expression rewrites the early pipeline needs
//! (comparisons with null, `stobj`/`ldobj` of local addresses, `AsyncHelpers.Await`, decimal
//! constants), in a post-order visit (pre-order for `newobj`).

use crate::il::ast::{
    AwaiterMethod, ComparisonKind, ComparisonLiftingKind, Function, Inst, NodeId,
};
use crate::il::reader::ReadFunction;
use crate::il::types::{Sign, StackType};
use crate::typesystem::known::KnownTypeCode;
use crate::typesystem::types::{TypeData, TypeSystem};

use super::Context;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let is_async = r.async_return_type.is_some();
    let f = &mut r.function;
    let body = f.body.expect("body");
    visit(f, ctx, is_async, body);
}

fn visit_children(f: &mut Function, ctx: &mut Context<'_, '_, '_>, is_async: bool, inst: NodeId) {
    let mut i = 0;
    while let Some(&child) = f.inst(inst).children().get(i) {
        visit(f, ctx, is_async, child);
        i += 1;
    }
}

fn visit(f: &mut Function, ctx: &mut Context<'_, '_, '_>, is_async: bool, inst: NodeId) {
    match f.inst(inst) {
        Inst::Comp { .. } => {
            visit_children(f, ctx, is_async, inst);
            fix_comparison_kind_ld_null(f, ctx.ts, inst);
        }
        Inst::StObj { .. } => {
            visit_children(f, ctx, is_async, inst);
            stobj_to_stloc(f, ctx.ts, inst);
        }
        Inst::LdObj { .. } => {
            visit_children(f, ctx, is_async, inst);
            address_of_ldloc_to_ldloca(f, ctx.ts, inst);
            ldobj_to_ldloc(f, ctx.ts, inst);
        }
        Inst::Call(_) => {
            visit_children(f, ctx, is_async, inst);
            transform_async_helpers_await_to_await(f, ctx, is_async, inst);
        }
        Inst::NewObj(_) => {
            if let Some(bits) = transform_decimal_ctor_to_constant(f, ctx.ts, inst) {
                let constant = f.add(Inst::LdcDecimal { bits });
                f.replace(inst, constant);
                return;
            }
            visit_children(f, ctx, is_async, inst);
        }
        _ => visit_children(f, ctx, is_async, inst),
    }
}

/// `FixComparisonKindLdNull`: an ordered comparison with null is an (in)equality, and
/// `box T(x) ==/!= null` of a type parameter compares `x`.
pub fn fix_comparison_kind_ld_null(f: &mut Function, ts: &mut TypeSystem<'_, '_>, comp: NodeId) {
    let Inst::Comp {
        kind, left, right, ..
    } = *f.inst(comp)
    else {
        return;
    };
    if !matches!(
        f.inst(comp),
        Inst::Comp {
            lifting: ComparisonLiftingKind::None,
            ..
        }
    ) {
        return;
    }
    let new_kind = if matches!(f.inst(right), Inst::LdNull) {
        match kind {
            ComparisonKind::GreaterThan => ComparisonKind::Inequality,
            ComparisonKind::LessThanOrEqual => ComparisonKind::Equality,
            k => k,
        }
    } else if matches!(f.inst(left), Inst::LdNull) {
        match kind {
            ComparisonKind::LessThan => ComparisonKind::Inequality,
            ComparisonKind::GreaterThanOrEqual => ComparisonKind::Equality,
            k => k,
        }
    } else {
        kind
    };
    if let Inst::Comp { kind, .. } = f.inst_mut(comp) {
        *kind = new_kind;
    }
    if matches!(f.inst(right), Inst::LdNull)
        && let Inst::Box { arg, ty } = *f.inst(left)
        && is_type_parameter(ts, ty.ty)
        && new_kind.is_equality_or_inequality()
    {
        f.replace(left, arg);
        let input = f.result_type(ts, arg);
        if let Inst::Comp { input: i, .. } = f.inst_mut(comp) {
            *i = input;
        }
    }
}

/// `IType.Kind == TypeKind.TypeParameter`, also with a nullability annotation.
fn is_type_parameter(ts: &TypeSystem<'_, '_>, t: crate::typesystem::types::TypeId) -> bool {
    match ts.data(t) {
        TypeData::TypeParam { .. } => true,
        TypeData::Annotated { inner, .. } => is_type_parameter(ts, *inner),
        _ => false,
    }
}

/// `StObjToStLoc`: `stobj(ldloca v, value)` becomes `stloc v(value)`.
pub fn stobj_to_stloc(f: &mut Function, ts: &mut TypeSystem<'_, '_>, inst: NodeId) -> bool {
    let Inst::StObj {
        target,
        value,
        ty,
        is_volatile,
        unaligned,
    } = *f.inst(inst)
    else {
        return false;
    };
    let Inst::LdLoca { var } = *f.inst(target) else {
        return false;
    };
    if !ts.is_compatible_type_for_memory_access(f.var(var).ty, ty.ty)
        || unaligned != 0
        || is_volatile
    {
        return false;
    }
    let mut replacement = f.add(Inst::StLoc {
        var,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.add_il_range(replacement, f.il_range(inst));
    let in_instruction_slot = f.node(inst).parent.is_some_and(|p| {
        matches!(f.inst(p), Inst::Block { instructions, .. }
            if instructions.get(f.node(inst).child_index as usize) == Some(&inst))
    });
    if f.var(var).stack_type == StackType::Unknown
        && !ts.is_unknown_kind(ty.ty)
        && !in_instruction_slot
    {
        replacement = conv_to(f, ts, replacement, ty.ty);
    }
    f.replace(inst, replacement);
    true
}

/// `new Conv(inst, type.ToPrimitiveType(), checkForOverflow: false, Sign.None)`.
fn conv_to(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
    ty: crate::typesystem::types::TypeId,
) -> NodeId {
    let input = f.result_type(ts, inst);
    let target = ts.primitive_type(ty);
    f.add(Inst::Conv {
        is_lifted: false,
        arg: inst,
        input,
        input_sign: Sign::None,
        target,
        check_overflow: false,
    })
}

/// `LdObjToLdLoc`: `ldobj(ldloca v)` becomes `ldloc v`.
pub(crate) fn ldobj_to_ldloc(f: &mut Function, ts: &mut TypeSystem<'_, '_>, inst: NodeId) -> bool {
    let Inst::LdObj {
        target,
        ty,
        is_volatile,
        unaligned,
    } = *f.inst(inst)
    else {
        return false;
    };
    let Inst::LdLoca { var } = *f.inst(target) else {
        return false;
    };
    if !ts.is_compatible_type_for_memory_access(f.var(var).ty, ty.ty)
        || unaligned != 0
        || is_volatile
    {
        return false;
    }
    let mut replacement = f.add(Inst::LdLoc { var });
    f.add_il_range(replacement, f.il_range(inst));
    if f.var(var).stack_type == StackType::Unknown && !ts.is_unknown_kind(ty.ty) {
        replacement = conv_to(f, ts, replacement, ty.ty);
    }
    f.replace(inst, replacement);
    true
}

/// `AddressOfLdLocToLdLoca`: `ldobj(ldflda*(addressof(ldloc v)))` becomes
/// `ldobj(ldflda*(ldloca v))`; a read through `addressof(ldobj(addr))` of a compatible type
/// reads `addr` directly.
pub(crate) fn address_of_ldloc_to_ldloca(
    f: &mut Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
) {
    let Inst::LdObj { target, .. } = *f.inst(inst) else {
        return;
    };
    let mut temp = target;
    let mut ranges = vec![f.il_range(temp)];
    while let Inst::LdFlda { target: inner, .. } = *f.inst(temp) {
        temp = inner;
        ranges.push(f.il_range(temp));
    }
    let Inst::AddressOf {
        value: address_of_target,
        ty: address_of_type,
    } = *f.inst(temp)
    else {
        return;
    };
    let replacement = match *f.inst(address_of_target) {
        Inst::LdLoc { var } => {
            let r = f.add(Inst::LdLoca { var });
            f.add_il_range(r, f.il_range(address_of_target));
            r
        }
        // The enclosing ldobj only reads through the temporary the addressof creates, so the
        // read goes through the inner address (e.g. element 8+ of a long tuple via Rest). The
        // type check keeps reads of an incompatible type off that address.
        Inst::LdObj {
            target: inner_target,
            ty: inner_ty,
            is_volatile: false,
            unaligned: 0,
        } if ts.is_compatible_type_for_memory_access(inner_ty.ty, address_of_type.ty) => {
            ranges.push(f.il_range(address_of_target));
            inner_target
        }
        _ => return,
    };
    for r in ranges {
        f.add_il_range(replacement, r);
    }
    f.replace(temp, replacement);
}

/// `TransformAsyncHelpersAwaitToAwait`: runtime-async `call AsyncHelpers.Await(value)` is an
/// `await` (only in a runtime-async method).
fn transform_async_helpers_await_to_await(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    is_async: bool,
    inst: NodeId,
) -> bool {
    if !ctx.settings.async_await || !is_async {
        return false;
    }
    let Inst::Call(c) = f.inst(inst) else {
        return false;
    };
    let is_await = c.method.method.is_static
        && c.args.len() == 1
        && c.method.method.name == "Await"
        && c.method
            .method
            .declaring
            .is_some_and(|d| ctx.ts.full_name(d) == "System.Runtime.CompilerServices.AsyncHelpers");
    if !is_await {
        return false;
    }
    let (value, method) = (c.args[0], c.method.clone());
    let await_inst = f.add(Inst::Await {
        value,
        get_awaiter: None,
        get_result: Some(AwaiterMethod::Method(method)),
        get_awaiter_callvirt: false,
    });
    f.add_il_range(await_inst, f.il_range(inst));
    f.replace(inst, await_inst);
    true
}

/// `TransformDecimalCtorToConstant`: `new decimal(constant)` and `new decimal(lo, mid, hi,
/// isNegative, scale)`, as the bits of `decimal.GetBits`.
fn transform_decimal_ctor_to_constant(
    f: &Function,
    ts: &mut TypeSystem<'_, '_>,
    inst: NodeId,
) -> Option<[u32; 4]> {
    let Inst::NewObj(c) = f.inst(inst) else {
        return None;
    };
    let declaring = c.method.method.declaring?;
    if !ts
        .definition(declaring)
        .and_then(|d| ts.comp.type_def(d).ok())
        .is_some_and(|i| i.known == KnownTypeCode::Decimal)
    {
        return None;
    }
    match c.args[..] {
        [arg] => {
            let val = f.match_ldc_i(arg)?;
            let params = c.method.method.params().ok()?;
            let param = ts
                .definition(*params.first()?)
                .and_then(|d| ts.comp.type_def(d).ok())
                .map(|i| i.known);
            let value = match param? {
                KnownTypeCode::Int32 => i128::from(val as i32),
                KnownTypeCode::UInt32 => i128::from(val as u32),
                KnownTypeCode::Int64 => i128::from(val),
                KnownTypeCode::UInt64 => i128::from(val as u64),
                _ => return None,
            };
            let magnitude = value.unsigned_abs();
            Some([
                magnitude as u32,
                (magnitude >> 32) as u32,
                (magnitude >> 64) as u32,
                if value < 0 { 0x8000_0000 } else { 0 },
            ])
        }
        [lo, mid, hi, negative, scale] => {
            let (lo, mid, hi, negative, scale) = (
                f.match_ldc_i4(lo)?,
                f.match_ldc_i4(mid)?,
                f.match_ldc_i4(hi)?,
                f.match_ldc_i4(negative)?,
                f.match_ldc_i4(scale)? as u8,
            );
            if scale > 28 {
                return None;
            }
            let sign = if negative != 0 { 0x8000_0000 } else { 0 };
            Some([
                lo as u32,
                mid as u32,
                hi as u32,
                sign | (u32::from(scale) << 16),
            ])
        }
        _ => None,
    }
}
