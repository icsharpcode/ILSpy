//! `IL/Transforms/InterpolatedStringTransform.cs`: the `DefaultInterpolatedStringHandler` calls
//! an interpolated string compiles to become a `Block InterpolatedString` in the place of the
//! final `ToStringAndClear` call.
//!
//! ```text
//! stloc v(newobj DefaultInterpolatedStringHandler..ctor(ldc.i4 literalLength, ldc.i4 formattedCount))
//! call AppendLiteral(ldloca v, ldstr "...")
//! call AppendFormatted(ldloca v, ...)
//! ... call ToStringAndClear(ldloca v) ...
//! ```

use crate::il::ast::{BlockKind, Function, Inst, NodeId, NopKind, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::known::KnownTypeCode;

use super::Context;
use super::block_transform::BlockTransformContext;
use super::il_inlining::{Find, find_load_in_next, known, options};
use super::statement_transform::StatementTransformContext;

/// `IStatementTransform.Run`.
pub fn run(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    _bc: &mut BlockTransformContext,
    _sc: &mut StatementTransformContext,
    block: NodeId,
    pos: usize,
) {
    if !ctx.settings.string_interpolation {
        return;
    }
    let f = &mut r.function;
    let interpolation_start = pos;
    // stloc v(newobj DefaultInterpolatedStringHandler..ctor(ldc.i4 literalLength, ldc.i4 formattedCount))
    let Some((v, new_obj)) = f.match_stloc(f.list(block)[pos]) else {
        return;
    };
    if f.var(v).kind != VariableKind::Local {
        return;
    }
    let Inst::NewObj(c) = f.inst(new_obj) else {
        return;
    };
    let &[length, count] = &c.args[..] else {
        return;
    };
    let declaring = c.method.method.declaring;
    let handler = Some(KnownTypeCode::DefaultInterpolatedStringHandler);
    let v_type = f.var(v).ty;
    if known(ctx, v_type) != handler
        || declaring.and_then(|d| known(ctx, d)) != handler
        || f.match_ldc_i4(length).is_none()
        || f.match_ldc_i4(count).is_none()
    {
        return;
    }
    // { call MethodName(ldloca v, ...) }
    let mut pos = pos + 1;
    while is_known_call(f, ctx, block, pos, v) {
        pos += 1;
    }
    let interpolation_end = pos;
    // ... call ToStringAndClear(ldloca v) ...
    let Some(insertion_point) = find_to_string_and_clear(
        f,
        ctx,
        block,
        pos,
        interpolation_start,
        interpolation_end,
        v,
    ) else {
        return;
    };
    let var = f.var(v);
    if !(var.store_count() == 1
        && var.address_count() as usize == interpolation_end - interpolation_start
        && var.load_count() == 0)
    {
        return;
    }
    f.var_mut(v).kind = VariableKind::InitializerTarget;
    let instructions = f.list(block)[interpolation_start..interpolation_end].to_vec();
    let nop = f.add(Inst::Nop {
        kind: NopKind::Normal,
    });
    let replacement = f.add(Inst::Block {
        kind: BlockKind::InterpolatedString,
        instructions,
        final_inst: nop,
    });
    let final_index = interpolation_end - interpolation_start;
    f.replace(insertion_point, replacement);
    f.set_child_at(replacement, final_index, insertion_point);
    for _ in interpolation_start..interpolation_end {
        f.remove_at(block, interpolation_start);
    }
}

/// `IsKnownCall`: an `AppendLiteral` or `AppendFormatted` call on the handler.
fn is_known_call(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
    v: VarId,
) -> bool {
    let list = f.list(block);
    if pos + 1 >= list.len() {
        return false;
    }
    let Inst::Call(c) = f.inst(list[pos]) else {
        return false;
    };
    if c.args.len() <= 1 || !matches!(*f.inst(c.args[0]), Inst::LdLoca { var } if var == v) {
        return false;
    }
    if c.method.method.is_static
        || c.method.method.declaring.and_then(|d| known(ctx, d))
            != Some(KnownTypeCode::DefaultInterpolatedStringHandler)
    {
        return false;
    }
    let is_str = |n: NodeId| matches!(f.inst(n), Inst::LdStr { .. });
    let is_i4 = |n: NodeId| matches!(f.inst(n), Inst::LdcI4 { .. });
    let args = &c.args;
    match (c.method.method.name.as_str(), args.len()) {
        ("AppendLiteral", 2) => is_str(args[1]),
        ("AppendFormatted", 2) => true,
        ("AppendFormatted", 3) => is_str(args[2]) || is_i4(args[2]),
        ("AppendFormatted", 4) => is_i4(args[2]) && is_str(args[3]),
        _ => false,
    }
}

/// `FindToStringAndClear`: the `call ToStringAndClear(ldloca v)` in the statement at `pos`
/// into which all the handler calls can move.
fn find_to_string_and_clear(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    pos: usize,
    interpolation_start: usize,
    interpolation_end: usize,
    v: VarId,
) -> Option<NodeId> {
    let &stmt = f.list(block).get(pos)?;
    let mut insertion_point = None;
    for i in interpolation_start..interpolation_end {
        let moved = f.list(block)[i];
        let Find::Found(load) = find_load_in_next(f, ctx, Some(stmt), v, moved, options::NONE)
        else {
            return None;
        };
        insertion_point = insertion_point.or(f.node(load).parent);
    }
    let insertion_point = insertion_point?;
    match f.inst(insertion_point) {
        Inst::Call(c)
            if c.args.len() == 1
                && c.method.method.name == "ToStringAndClear"
                && !c.method.method.is_static =>
        {
            Some(insertion_point)
        }
        _ => None,
    }
}
