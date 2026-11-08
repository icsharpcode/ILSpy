//! `IL/Transforms/LdLocaDupInitObjTransform.cs`: Roslyn's `ldloca; dup; initobj`
//!   stloc s(ldloca v); stobj T(ldloc s, default.value T)   (s loaded more than once)
//! becomes legacy csc's `ldloca; ldloca; initobj`
//!   stloc v(default.value T); stloc s(ldloca v)
//! which lets the later uses of `s` inline.

use crate::il::ast::{Function, Inst, NodeId};
use crate::il::reader::ReadFunction;
use crate::typesystem::types::TypeSystem;

use super::Context;

pub fn run(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>) {
    let f = &mut r.function;
    let body = f.body.expect("body");
    let blocks: Vec<NodeId> = f
        .descendants_post(body)
        .into_iter()
        .filter(|&n| matches!(f.inst(n), Inst::Block { .. }))
        .collect();
    for block in blocks {
        let mut i = 0;
        while i < f.list(block).len() {
            try_transform(f, ctx.ts, block, i);
            i += 1;
        }
    }
}

fn try_transform(f: &mut Function, ts: &mut TypeSystem<'_, '_>, block: NodeId, i: usize) -> bool {
    let inst1 = f.list(block)[i];
    let Some((s, address)) = f.match_stloc(inst1) else {
        return false;
    };
    let Inst::LdLoca { var: v } = *f.inst(address) else {
        return false;
    };
    let Some(&inst2) = f.list(block).get(i + 1) else {
        return false;
    };
    let Inst::StObj {
        target,
        value,
        ty,
        is_volatile,
        unaligned,
    } = *f.inst(inst2)
    else {
        return false;
    };
    if f.match_ldloc(target) != Some(s)
        || !ts.is_compatible_type_for_memory_access(f.var(v).ty, ty.ty)
        || unaligned != 0
        || is_volatile
        || !matches!(f.inst(value), Inst::DefaultValue { .. })
    {
        return false;
    }
    let new_inst = f.add(Inst::StLoc {
        var: v,
        value,
        il_stack_was_empty: false,
        is_stack_adjustment: false,
    });
    f.add_il_range(new_inst, f.il_range(inst2));
    // `Instructions[i] = newInst; Instructions[i + 1] = inst1`: each assignment connects the
    // new item before it releases the old one.
    f.replace(inst1, new_inst);
    f.replace(inst2, inst1);
    true
}
