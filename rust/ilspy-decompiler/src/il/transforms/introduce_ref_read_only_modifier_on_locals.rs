//! `IL/Transforms/IntroduceRefReadOnlyModifierOnLocals.cs`: by-ref locals whose stores need a
//! readonly reference become `ref readonly`.

use crate::il::ast::{Function, Inst, VarId, VariableKind};
use crate::il::reader::ReadFunction;
use crate::typesystem::types::TypeData;

use super::Context;
use super::il_inlining::is_readonly_reference;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    for v in r.variables.clone() {
        if f.var(v).kind == VariableKind::Parameter || !is_by_reference(ctx, f.var(v).ty) {
            continue;
        }
        if is_used_as_ref_readonly(f, ctx, v) {
            f.var_mut(v).is_ref_read_only = true;
        }
    }
}

/// `Type.Kind == TypeKind.ByReference`, looking through modifiers and nullability.
fn is_by_reference(ctx: &Context<'_, '_, '_>, mut ty: crate::typesystem::types::TypeId) -> bool {
    loop {
        match ctx.ts.data(ty) {
            TypeData::ByRef(_) => return true,
            TypeData::Modified { inner, .. } | TypeData::Annotated { inner, .. } => ty = *inner,
            _ => return false,
        }
    }
}

/// `IsUsedAsRefReadonly`: a store C# allows only into a readonly reference, or one that keeps
/// the semantics of a `readonly.ldelema`.
fn is_used_as_ref_readonly(f: &Function, ctx: &mut Context<'_, '_, '_>, v: VarId) -> bool {
    for &store in &f.var(v).store_instructions {
        let Inst::StLoc { value, .. } = *f.inst(store) else {
            continue;
        };
        if is_readonly_reference(f, ctx, value) {
            return true;
        }
        let mut val = value;
        while let Inst::LdFlda { target, .. } = *f.inst(val) {
            val = target;
        }
        if matches!(
            f.inst(val),
            Inst::LdElema {
                is_read_only: true,
                ..
            }
        ) {
            return true;
        }
    }
    false
}
