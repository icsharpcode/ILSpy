//! `IL/Transforms/IILTransform.cs`: the transform context and the IL pipeline the engine
//! serves, in the order of `CSharpDecompiler.GetILTransforms()` flattened to its leaves.

use crate::typesystem::types::TypeSystem;

use super::control_flow::control_flow_simplification::ControlFlowSimplification;
use super::reader::ReadFunction;

pub mod block_transform;
pub mod cached_delegate_initialization;
pub mod cached_read_only_span_initialization;
pub mod copy_propagation;
pub mod delegate_construction;
pub mod detect_catch_when_condition_blocks;
pub mod dynamic_call_site;
pub mod early_expression_transforms;
pub mod expression_transforms;
pub mod il_inlining;
pub mod index_range_transform;
pub mod inline_return_transform;
pub mod introduce_ref_read_only_modifier_on_locals;
pub mod ldloca_dup_init_obj_transform;
pub mod lock_transform;
pub mod pattern_matching_transform;
pub mod remove_dead_variable_init;
pub mod remove_infeasible_path_transform;
pub mod split_variables;
pub mod switch_on_nullable_transform;
pub mod switch_on_string_transform;
pub mod using_transform;

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
    "PatternMatching",
    "RecursivePatternMatching",
    "PatternCombinators",
    "RelationalPatterns",
    "AssumeArrayLengthFitsIntoInt32",
    "SwitchExpressions",
    "Ranges",
    "LockStatement",
    "UsingStatement",
    "AsyncUsingAndForEachStatement",
    "IntroduceRefModifiersOnStructs",
    "AnonymousMethods",
    "ArrayInitializers",
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
    pub pattern_matching: bool,
    pub recursive_pattern_matching: bool,
    pub pattern_combinators: bool,
    pub relational_patterns: bool,
    pub assume_array_length_fits_into_int32: bool,
    pub switch_expressions: bool,
    pub ranges: bool,
    pub lock_statement: bool,
    pub using_statement: bool,
    pub async_using_and_for_each_statement: bool,
    pub introduce_ref_modifiers_on_structs: bool,
    pub anonymous_methods: bool,
    pub array_initializers: bool,
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
            pattern_matching: bit(13),
            recursive_pattern_matching: bit(14),
            pattern_combinators: bit(15),
            relational_patterns: bit(16),
            assume_array_length_fits_into_int32: bit(17),
            switch_expressions: bit(18),
            ranges: bit(19),
            lock_statement: bit(20),
            using_statement: bit(21),
            async_using_and_for_each_statement: bit(22),
            introduce_ref_modifiers_on_structs: bit(23),
            anonymous_methods: bit(24),
            array_initializers: bit(25),
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

/// A pipeline leaf: a whole-function transform, or a post-order block transform of the
/// `BlockILTransform` group with the given index. Consecutive leaves of one group run in one
/// pass of the block driver, as the managed group does.
enum Leaf {
    Function(fn(&mut ReadFunction, &mut Context<'_, '_, '_>)),
    Block(u8, block_transform::BlockTransform),
}

use Leaf::{Block, Function};

/// The leaves of the IL pipeline the engine has ported, in run order. `IL_PREFIX` is its
/// length.
const PIPELINE: &[Leaf] = &[
    Function(|r, ctx| ControlFlowSimplification::default().run(&mut r.function, ctx)),
    Function(split_variables::run),
    Function(|r, ctx| il_inlining::ILInlining::default().run(r, ctx)),
    Function(|r, _| inline_return_transform::run(&mut r.function)),
    Function(|r, _| remove_infeasible_path_transform::run(&mut r.function)),
    Function(super::control_flow::detect_pinned_regions::run),
    Function(super::control_flow::yield_return_decompiler::run),
    Function(super::control_flow::async_await_decompiler::run),
    Function(detect_catch_when_condition_blocks::run),
    Function(super::control_flow::exit_points::run),
    Function(ldloca_dup_init_obj_transform::run),
    Function(early_expression_transforms::run),
    Function(split_variables::run),
    Function(remove_dead_variable_init::run),
    Function(|r, ctx| ControlFlowSimplification::default().run(&mut r.function, ctx)),
    Function(dynamic_call_site::run),
    Function(super::control_flow::switch_detection::run),
    Function(switch_on_string_transform::run),
    Function(switch_on_nullable_transform::run),
    Function(split_variables::run),
    Function(introduce_ref_read_only_modifier_on_locals::run),
    Block(0, super::control_flow::loop_detection::run),
    Function(super::control_flow::exit_points::run),
    Function(pattern_matching_transform::run),
    Block(1, super::control_flow::condition_detection::run),
    Block(1, lock_transform::run),
    Block(1, using_transform::run),
    Block(1, cached_delegate_initialization::run),
    Block(1, cached_read_only_span_initialization::run),
];

/// Runs the first `prefix` leaves of the pipeline.
///
/// # Panics
/// When `prefix` exceeds `IL_PREFIX`; the FFI layer rejects such requests first.
pub fn run_prefix(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, prefix: u32) {
    let mut leaves = PIPELINE[..prefix as usize].iter().peekable();
    while let Some(leaf) = leaves.next() {
        match *leaf {
            Function(transform) => transform(r, ctx),
            Block(group, first) => {
                let mut group_transforms = vec![first];
                while let Some(&&Block(g, next)) = leaves.peek()
                    && g == group
                {
                    group_transforms.push(next);
                    leaves.next();
                }
                block_transform::run(r, ctx, &group_transforms);
            }
        }
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
