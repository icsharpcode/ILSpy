//! `IL/Transforms/EarlyExpressionTransforms.cs`: the rewrites the async/await decompiler uses
//! on its own; the pipeline step comes later.

use crate::il::ast::{Function, Inst, NodeId};
use crate::il::types::{Sign, StackType};
use crate::typesystem::types::TypeSystem;

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
        let input = f.result_type(ts, replacement);
        let target = ts.primitive_type(ty.ty);
        replacement = f.add(Inst::Conv {
            arg: replacement,
            input,
            input_sign: Sign::None,
            target,
            check_overflow: false,
        });
    }
    f.replace(inst, replacement);
    true
}
