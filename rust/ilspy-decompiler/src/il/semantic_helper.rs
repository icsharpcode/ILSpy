//! `IL/SemanticHelper.cs`: purity and reordering of instructions.

use std::collections::HashSet;

use super::ast::{Flags, Function, Inst, NodeId, VarId};

/// `IsPure`: reading locals and internal control flow are pure (an infinite loop needs
/// `MayBranch`).
pub fn is_pure(flags: Flags) -> bool {
    flags.0 & !(Flags::MAY_READ_LOCALS.0 | Flags::CONTROL_FLOW.0) == 0
}

/// `MayReorder`: whether `inst1; inst2` may become `inst2; inst1`.
pub fn may_reorder(f: &Function, inst1: NodeId, inst2: NodeId) -> bool {
    if !is_pure(f.flags(inst1)) && !is_pure(f.flags(inst2)) {
        return false;
    }
    !inst2_might_write_to_variable_read_by_inst1(f, inst1, inst2)
        && !inst2_might_write_to_variable_read_by_inst1(f, inst2, inst1)
}

fn inst2_might_write_to_variable_read_by_inst1(f: &Function, inst1: NodeId, inst2: NodeId) -> bool {
    if !f.has_flag(inst1, Flags::MAY_READ_LOCALS) {
        return false;
    }
    let variables: HashSet<VarId> = f
        .descendants_post(inst1)
        .into_iter()
        .filter_map(|n| f.match_ldloc(n))
        .collect();
    if f.has_flag(inst2, Flags::SIDE_EFFECT)
        && variables.iter().any(|&v| f.var(v).address_count > 0)
    {
        // Indirect writes may hit any variable whose address is taken.
        return true;
    }
    f.descendants_post(inst2).into_iter().any(|n| {
        f.has_direct_flag(n, Flags::MAY_WRITE_LOCALS)
            && match *f.inst(n) {
                Inst::StLoc { var, .. } | Inst::TryCatchHandler { variable: var, .. } => {
                    variables.contains(&var)
                }
                _ => false,
            }
    })
}
