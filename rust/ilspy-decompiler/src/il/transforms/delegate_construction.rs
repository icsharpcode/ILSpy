//! `IL/Transforms/DelegateConstruction.cs`, the delegate construction matcher other
//! transforms use.

use crate::il::ast::{Function, Inst, NodeId};
use crate::typesystem::compilation::TypeKind;
use crate::typesystem::types::TypeData;

use super::Context;

/// `MatchDelegateConstruction(inst, ...)`: `newobj D(target, ldftn M)` or
/// `newobj D(target, ldvirtftn M)` of a delegate type (or an unknown one).
// shortcut: the engine has no ldvirtdelegate or nested ILFunction yet, so the
// `LdVirtDelegate` case and `allowTransformed` cannot apply; add them with
// DelegateConstruction itself.
pub(crate) fn match_delegate_construction(
    f: &Function,
    ctx: &mut Context<'_, '_, '_>,
    inst: NodeId,
) -> bool {
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
    let Some(mut delegate_type) = call.method.method.declaring else {
        return true;
    };
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
