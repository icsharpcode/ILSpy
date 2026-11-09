//! `IL/Transforms/DelegateConstruction.cs`, the delegate construction matcher other
//! transforms use.

use crate::il::ast::{Function, Inst, NodeId};
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::types::TypeData;

use super::Context;

/// `MatchDelegateConstruction(inst, ...)`: `newobj D(target, ldftn M)` or
/// `newobj D(target, ldvirtftn M)` of a delegate type (or an unknown one).
// shortcut: the engine has no nested ILFunction yet, so `allowTransformed` cannot apply; add
// it with DelegateConstruction itself.
pub(crate) fn match_delegate_construction(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
    if let Inst::LdVirtDelegate { ty, .. } = *f.inst(inst) {
        return is_delegate_or_unknown(ctx, ty);
    }
    let Inst::NewObj(call) = f.inst(inst) else {
        return false;
    };
    let [_, method] = call.args[..] else {
        return false;
    };
    if !matches!(f.inst(method), Inst::LdFtn { .. } | Inst::LdVirtFtn { .. }) {
        return false;
    }
    // A method without a declaring type has the unknown type there.
    let Some(delegate_type) = call.method.method.declaring else {
        return true;
    };
    is_delegate_or_unknown(ctx, delegate_type)
}

/// `delegateType.Kind == TypeKind.Delegate || delegateType.Kind == TypeKind.Unknown`.
fn is_delegate_or_unknown(
    ctx: &mut Context<'_, '_, '_>,
    mut delegate_type: crate::typesystem::types::TypeId,
) -> bool {
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
    ctx.ts
        .definition(delegate_type)
        .and_then(|d| ctx.ts.comp.type_def(d).ok())
        .is_some_and(|i| i.kind == TypeKind::Delegate)
}
