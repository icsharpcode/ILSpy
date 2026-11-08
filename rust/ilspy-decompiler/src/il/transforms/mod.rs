//! `IL/Transforms/IILTransform.cs`: the transform context and the IL pipeline the engine
//! serves, in the order of `CSharpDecompiler.GetILTransforms()` flattened to its leaves.

use crate::typesystem::types::TypeSystem;

use super::control_flow::control_flow_simplification::ControlFlowSimplification;
use super::reader::ReadFunction;

pub mod il_inlining;
pub mod inline_return_transform;
pub mod split_variables;

/// The `DecompilerSettings` the ported transforms read, by property name. The host sends
/// them as a bit set in this order (`Interop/RustSettings.cs` looks the names up by
/// reflection, so a renamed setting fails loudly instead of being ignored).
pub const SETTING_NAMES: &[&str] = &["RemoveDeadStores", "SortSwitchSections"];

/// The values of `SETTING_NAMES`.
#[derive(Clone, Copy, Debug, Default)]
pub struct Settings {
    pub remove_dead_stores: bool,
    pub sort_switch_sections: bool,
}

impl Settings {
    /// Bit `i` is `SETTING_NAMES[i]`.
    pub fn from_bits(bits: u64) -> Settings {
        let bit = |i: usize| bits & (1 << i) != 0;
        Settings {
            remove_dead_stores: bit(0),
            sort_switch_sections: bit(1),
        }
    }
}

/// `ILTransformContext`, as far as the ported transforms use it.
pub struct Context<'t, 'c, 'a> {
    pub ts: &'t mut TypeSystem<'c, 'a>,
    pub settings: Settings,
}

/// The leaves of the IL pipeline the engine has ported, in run order. `IL_PREFIX` is its
/// length.
const PIPELINE: &[fn(&mut ReadFunction, &mut Context<'_, '_, '_>)] = &[
    |r, ctx| ControlFlowSimplification::default().run(&mut r.function, ctx),
    split_variables::run,
    |r, ctx| il_inlining::ILInlining::default().run(r, ctx),
    |r, _| inline_return_transform::run(&mut r.function),
];

/// Runs the first `prefix` leaves of the pipeline.
///
/// # Panics
/// When `prefix` exceeds `IL_PREFIX`; the FFI layer rejects such requests first.
pub fn run_prefix(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, prefix: u32) {
    for transform in &PIPELINE[..prefix as usize] {
        transform(r, ctx);
    }
}

#[cfg(test)]
mod tests {
    #[test]
    fn the_pipeline_is_the_served_prefix() {
        assert_eq!(super::PIPELINE.len() as u32, crate::IL_PREFIX);
    }
}
