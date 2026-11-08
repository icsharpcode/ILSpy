//! `IL/Transforms/IILTransform.cs`: the transform context and the IL pipeline the engine
//! serves, in the order of `CSharpDecompiler.GetILTransforms()` flattened to its leaves.

use crate::typesystem::types::TypeSystem;

use super::control_flow::control_flow_simplification::ControlFlowSimplification;
use super::reader::ReadFunction;

pub mod il_inlining;
pub mod inline_return_transform;
pub mod remove_infeasible_path_transform;
pub mod split_variables;

/// The `DecompilerSettings` the ported transforms read, by property name. The host sends
/// them as a bit set in this order (`Interop/RustSettings.cs` looks the names up by
/// reflection, so a renamed setting fails loudly instead of being ignored).
pub const SETTING_NAMES: &[&str] = &[
    "RemoveDeadStores",
    "SortSwitchSections",
    "PatternBasedFixedStatement",
];

/// The values of `SETTING_NAMES`.
#[derive(Clone, Copy, Debug, Default)]
pub struct Settings {
    pub remove_dead_stores: bool,
    pub sort_switch_sections: bool,
    pub pattern_based_fixed_statement: bool,
}

impl Settings {
    /// Bit `i` is `SETTING_NAMES[i]`.
    pub fn from_bits(bits: u64) -> Settings {
        let bit = |i: usize| bits & (1 << i) != 0;
        Settings {
            remove_dead_stores: bit(0),
            sort_switch_sections: bit(1),
            pattern_based_fixed_statement: bit(2),
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
    |r, _| remove_infeasible_path_transform::run(&mut r.function),
    super::control_flow::detect_pinned_regions::run,
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

impl ReadFunction {
    /// `ILFunction.RegisterVariable`: a new variable named `I_n` from the function's helper
    /// counter, appended to the variable list.
    pub fn register_variable(
        &mut self,
        ctx: &mut Context<'_, '_, '_>,
        kind: super::ast::VariableKind,
        ty: crate::typesystem::types::TypeId,
    ) -> super::ast::VarId {
        let name = format!("I_{}", self.helper_variable_count);
        self.helper_variable_count += 1;
        let stack_type = ctx.ts.stack_type(ty);
        let v = self.function.add_var(super::ast::Variable {
            kind,
            ty,
            stack_type,
            index: None,
            name,
            has_generated_name: true,
            initial_value_is_initialized: false,
            uses_initial_value: false,
            is_ref_read_only: false,
            remove_if_redundant: false,
            load_instructions: Vec::new(),
            store_instructions: Vec::new(),
            address_instructions: Vec::new(),
        });
        self.variables.push(v);
        v
    }
}

#[cfg(test)]
mod tests {
    #[test]
    fn the_pipeline_is_the_served_prefix() {
        assert_eq!(super::PIPELINE.len() as u32, crate::IL_PREFIX);
    }
}
