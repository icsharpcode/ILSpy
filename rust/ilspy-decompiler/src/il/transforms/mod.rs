//! `IL/Transforms/IILTransform.cs`: the transform context and the IL pipeline the engine
//! serves, in the order of `CSharpDecompiler.GetILTransforms()` flattened to its leaves.

use crate::typesystem::types::TypeSystem;

use super::control_flow::control_flow_simplification::ControlFlowSimplification;
use super::reader::ReadFunction;

pub mod copy_propagation;
pub mod detect_catch_when_condition_blocks;
pub mod dynamic_call_site;
pub mod early_expression_transforms;
pub mod il_inlining;
pub mod inline_return_transform;
pub mod introduce_ref_read_only_modifier_on_locals;
pub mod ldloca_dup_init_obj_transform;
pub mod remove_dead_variable_init;
pub mod remove_infeasible_path_transform;
pub mod split_variables;
pub mod switch_on_nullable_transform;
pub mod switch_on_string_transform;

/// The `DecompilerSettings` the ported transforms read, by property name. The host sends
/// them as a bit set in this order (`Interop/RustSettings.cs` looks the names up by
/// reflection, so a renamed setting fails loudly instead of being ignored).
pub const SETTING_NAMES: &[&str] = &[
    "RemoveDeadStores",
    "SortSwitchSections",
    "PatternBasedFixedStatement",
    "YieldReturn",
    "AsyncAwait",
    "AsyncEnumerator",
    "AwaitInCatchFinally",
    "Dynamic",
    "SparseIntegerSwitch",
    "RemoveDeadCode",
    "SwitchStatementOnString",
    "SwitchOnReadOnlySpanChar",
    "LiftNullables",
];

/// The values of `SETTING_NAMES`.
#[derive(Clone, Copy, Debug, Default)]
pub struct Settings {
    pub remove_dead_stores: bool,
    pub sort_switch_sections: bool,
    pub pattern_based_fixed_statement: bool,
    pub yield_return: bool,
    pub async_await: bool,
    pub async_enumerator: bool,
    pub await_in_catch_finally: bool,
    pub dynamic: bool,
    pub sparse_integer_switch: bool,
    pub remove_dead_code: bool,
    pub switch_statement_on_string: bool,
    pub switch_on_read_only_span_char: bool,
    pub lift_nullables: bool,
}

impl Settings {
    /// Bit `i` is `SETTING_NAMES[i]`.
    pub fn from_bits(bits: u64) -> Settings {
        let bit = |i: usize| bits & (1 << i) != 0;
        Settings {
            remove_dead_stores: bit(0),
            sort_switch_sections: bit(1),
            pattern_based_fixed_statement: bit(2),
            yield_return: bit(3),
            async_await: bit(4),
            async_enumerator: bit(5),
            await_in_catch_finally: bit(6),
            dynamic: bit(7),
            sparse_integer_switch: bit(8),
            remove_dead_code: bit(9),
            switch_statement_on_string: bit(10),
            switch_on_read_only_span_char: bit(11),
            lift_nullables: bit(12),
        }
    }
}

/// `ILTransformContext`, as far as the ported transforms use it.
pub struct Context<'t, 'c, 'a> {
    pub ts: &'t mut TypeSystem<'c, 'a>,
    pub settings: Settings,
    /// How methods are read (`CreateILReader`): the state machine decompilers read the
    /// methods of compiler-generated classes with the same options.
    pub reader: super::reader::ReaderOptions<'t>,
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
    super::control_flow::yield_return_decompiler::run,
    super::control_flow::async_await_decompiler::run,
    detect_catch_when_condition_blocks::run,
    super::control_flow::exit_points::run,
    ldloca_dup_init_obj_transform::run,
    early_expression_transforms::run,
    split_variables::run,
    remove_dead_variable_init::run,
    |r, ctx| ControlFlowSimplification::default().run(&mut r.function, ctx),
    dynamic_call_site::run,
    super::control_flow::switch_detection::run,
    switch_on_string_transform::run,
    switch_on_nullable_transform::run,
    split_variables::run,
    introduce_ref_read_only_modifier_on_locals::run,
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
        self.register_variable_named(ctx, kind, ty, None)
    }

    /// `RegisterVariable` with a name; a missing or blank name becomes `I_n`.
    pub fn register_variable_named(
        &mut self,
        ctx: &mut Context<'_, '_, '_>,
        kind: super::ast::VariableKind,
        ty: crate::typesystem::types::TypeId,
        name: Option<String>,
    ) -> super::ast::VarId {
        let (name, has_generated_name) = match name {
            Some(n) if !n.trim().is_empty() => (n, false),
            _ => {
                let n = format!("I_{}", self.helper_variable_count);
                self.helper_variable_count += 1;
                (n, true)
            }
        };
        let stack_type = ctx.ts.stack_type(ty);
        let v = self.function.add_var(super::ast::Variable {
            kind,
            ty,
            stack_type,
            index: None,
            name,
            has_generated_name,
            initial_value_is_initialized: false,
            uses_initial_value: false,
            is_ref_read_only: false,
            remove_if_redundant: false,
            state_machine_field: None,
            load_instructions: Vec::new(),
            store_instructions: Vec::new(),
            address_instructions: Vec::new(),
        });
        self.variables.push(v);
        v
    }
}

/// `CSharpDecompiler.EarlyILTransforms(aggressivelyDuplicateReturnBlocks)`: the clean-up the
/// state machine decompilers run on methods they read and on their result.
pub fn run_early_il_transforms(
    r: &mut ReadFunction,
    ctx: &mut Context<'_, '_, '_>,
    aggressively_duplicate_return_blocks: bool,
) {
    ControlFlowSimplification {
        aggressively_duplicate_return_blocks,
    }
    .run(&mut r.function, ctx);
    split_variables::run(r, ctx);
    il_inlining::ILInlining::default().run(r, ctx);
}

#[cfg(test)]
mod tests {
    #[test]
    fn the_pipeline_is_the_served_prefix() {
        assert_eq!(super::PIPELINE.len() as u32, crate::IL_PREFIX);
    }
}
