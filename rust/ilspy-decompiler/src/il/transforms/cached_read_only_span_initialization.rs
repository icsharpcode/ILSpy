//! `IL/Transforms/CachedReadOnlySpanInitialization.cs`: the lazy array cache Roslyn emits
//! for a `ReadOnlySpan<T>` from an array literal on frameworks without
//! `RuntimeHelpers.CreateSpan` is collapsed to the array initializer.
//!
//! ```text
//! stloc V(ldobj T[](ldsflda <PrivateImplementationDetails>.cache))
//! if (V == null) {
//!     stloc V(arrayInitializer)
//!     stobj T[](ldsflda <PrivateImplementationDetails>.cache, ldloc V)
//! }
//! ```
//! becomes `stloc V(arrayInitializer)`, so the output does not name the cache field, which
//! C# cannot express.

use crate::il::ast::{ComparisonKind, Function, Inst, NodeId};
use crate::il::reader::ReadFunction;

use super::Context;
use super::block_transform::BlockTransformContext;

/// `IBlockTransform.Run`.
pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, bc: &mut BlockTransformContext) {
    if !ctx.settings.array_initializers {
        return;
    }
    let f = &mut r.function;
    let block = bc.block;
    // The store of the cache precedes the if, so index 0 has nothing to match.
    let mut i = bc.index_of_first_already_transformed_instruction;
    while i > 1 {
        i -= 1;
        let inst = f.list(block)[i];
        if matches!(f.inst(inst), Inst::If { .. }) && do_transform(f, ctx, block, i, inst) {
            bc.index_of_first_already_transformed_instruction = f.list(block).len();
        }
    }
}

/// `DoTransform`.
fn do_transform(
    f: &mut Function,
    ctx: &mut Context<'_, '_, '_>,
    block: NodeId,
    i: usize,
    inst: NodeId,
) -> bool {
    // stloc V(ldobj(ldsflda cacheField)) of a compiler-generated field.
    let store_before_if = f.list(block)[i - 1];
    let Some((v, before_value)) = f.match_stloc(store_before_if) else {
        return false;
    };
    let Inst::LdObj { target, .. } = *f.inst(before_value) else {
        return false;
    };
    let Inst::LdsFlda { field } = f.inst(target) else {
        return false;
    };
    let cache_field = field.field.clone();
    if !ctx
        .ts
        .field_is_compiler_generated_or_in_compiler_generated_class(&cache_field)
    {
        return false;
    }
    // Two stores (the load of the cache, the initializer) and three loads (the null check,
    // the write-back, the one real use).
    let var = f.var(v);
    if var.store_count() != 2 || var.load_count() != 3 || var.address_count() != 0 {
        return false;
    }
    let Inst::If {
        condition,
        true_inst,
        false_inst,
        ..
    } = *f.inst(inst)
    else {
        return false;
    };
    if !f.match_nop(false_inst) || !matches!(f.inst(true_inst), Inst::Block { .. }) {
        return false;
    }
    let &[store_value, write_back] = f.list(true_inst) else {
        return false;
    };
    if f.match_comp_null(condition, ComparisonKind::Equality)
        .and_then(|arg| f.match_ldloc(arg))
        != Some(v)
    {
        return false;
    }
    let Some(value) = f
        .match_stloc(store_value)
        .filter(|&(sv, _)| sv == v)
        .map(|(_, value)| value)
    else {
        return false;
    };
    let Inst::StObj {
        target: write_target,
        value: written,
        ..
    } = *f.inst(write_back)
    else {
        return false;
    };
    let Inst::LdsFlda { field: field2 } = f.inst(write_target) else {
        return false;
    };
    let same_field = cache_field.origin == field2.field.origin
        && cache_field.declaring == field2.field.declaring;
    if !same_field || f.match_ldloc(written) != Some(v) {
        return false;
    }
    f.set_child_at(store_before_if, 0, value);
    f.remove_at(block, i);
    true
}
