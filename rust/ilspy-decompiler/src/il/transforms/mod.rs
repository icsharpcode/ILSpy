//! `IL/Transforms/IILTransform.cs`: the transform context and the IL pipeline the engine
//! serves, in the order of `CSharpDecompiler.GetILTransforms()` flattened to its leaves.

use crate::typesystem::types::{TypeId, TypeSystem};

use super::control_flow::control_flow_simplification::ControlFlowSimplification;
use super::reader::ReadFunction;

pub mod assign_variable_names;
pub mod block_transform;
pub mod cached_delegate_initialization;
pub mod cached_read_only_span_initialization;
pub mod combine_exits_transform;
pub mod copy_propagation;
pub mod csharp_semantics;
pub mod deconstruction_transform;
pub mod delegate_construction;
pub mod detect_catch_when_condition_blocks;
pub mod dynamic_call_site;
pub mod dynamic_is_event_assignment_transform;
pub mod early_expression_transforms;
pub mod expand_nested_conditionals;
pub mod expression_transforms;
pub mod fix_remaining_increments;
pub mod high_level_loop_transform;
pub mod il_extraction;
pub mod il_inlining;
pub mod index_range_transform;
pub mod inline_array_transform;
pub mod inline_return_transform;
pub mod interpolated_string_transform;
pub mod introduce_dynamic_type_on_locals;
pub mod introduce_native_int_type_on_locals;
pub mod introduce_ref_read_only_modifier_on_locals;
pub mod introduce_scoped_modifier_on_locals;
pub mod ldloca_dup_init_obj_transform;
pub mod local_function_decompiler;
pub mod lock_transform;
pub mod named_argument_transform;
pub mod null_coalescing_transform;
pub mod null_propagation_transform;
pub mod nullable_lifting_transform;
pub mod pattern_matching_transform;
pub mod proxy_call_replacer;
pub mod reduce_nesting_transform;
pub mod remove_dead_variable_init;
pub mod remove_infeasible_path_transform;
pub mod remove_unconstrained_generic_reference_type_check;
pub mod split_variables;
pub mod statement_transform;
pub mod switch_on_nullable_transform;
pub mod switch_on_string_transform;
pub mod transform_array_initializers;
pub mod transform_assignment;
pub mod transform_collection_and_object_initializers;
pub mod transform_display_class_usage;
pub mod transform_expression_trees;
pub mod user_defined_logic_transform;
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
    "AggressiveInlining",
    "MakeAssignmentExpressions",
    "IntroduceIncrementAndDecrement",
    "CheckedOperators",
    "NativeIntegers",
    "UnsignedRightShift",
    "InlineArrays",
    "ThrowExpressions",
    "NullPropagation",
    "StackAllocInitializers",
    "Utf8StringLiterals",
    "WithExpressions",
    "UseObjectCreationOfGenericTypeParameter",
    "ObjectOrCollectionInitializers",
    "DictionaryInitializers",
    "ExpressionTrees",
    "Deconstruction",
    "NamedArguments",
    "StringInterpolation",
    "ExtensionMethodsInCollectionInitializers",
    "LocalFunctions",
    "StaticLocalFunctions",
    "AggressiveScalarReplacementOfAggregates",
    "ForStatement",
    "DoWhileStatement",
    "ScopedRef",
    "FieldKeyword",
    "AlwaysQualifyMemberReferences",
    "NonTrailingNamedArguments",
    "UsingDeclarations",
    "OptionalArguments",
    "UseLambdaSyntax",
    "DefaultLiterals",
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
    pub aggressive_inlining: bool,
    pub make_assignment_expressions: bool,
    pub introduce_increment_and_decrement: bool,
    pub checked_operators: bool,
    pub native_integers: bool,
    pub unsigned_right_shift: bool,
    pub inline_arrays: bool,
    pub throw_expressions: bool,
    pub null_propagation: bool,
    pub stack_alloc_initializers: bool,
    pub utf8_string_literals: bool,
    pub with_expressions: bool,
    pub use_object_creation_of_generic_type_parameter: bool,
    pub object_or_collection_initializers: bool,
    pub dictionary_initializers: bool,
    pub expression_trees: bool,
    pub deconstruction: bool,
    pub named_arguments: bool,
    pub string_interpolation: bool,
    pub extension_methods_in_collection_initializers: bool,
    pub local_functions: bool,
    pub static_local_functions: bool,
    pub aggressive_scalar_replacement_of_aggregates: bool,
    pub for_statement: bool,
    pub do_while_statement: bool,
    pub scoped_ref: bool,
    pub field_keyword: bool,
    pub always_qualify_member_references: bool,
    pub non_trailing_named_arguments: bool,
    pub using_declarations: bool,
    pub optional_arguments: bool,
    pub use_lambda_syntax: bool,
    pub default_literals: bool,
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
            aggressive_inlining: bit(26),
            make_assignment_expressions: bit(27),
            introduce_increment_and_decrement: bit(28),
            checked_operators: bit(29),
            native_integers: bit(30),
            unsigned_right_shift: bit(31),
            inline_arrays: bit(32),
            throw_expressions: bit(33),
            null_propagation: bit(34),
            stack_alloc_initializers: bit(35),
            utf8_string_literals: bit(36),
            with_expressions: bit(37),
            use_object_creation_of_generic_type_parameter: bit(38),
            object_or_collection_initializers: bit(39),
            dictionary_initializers: bit(40),
            expression_trees: bit(41),
            deconstruction: bit(42),
            named_arguments: bit(43),
            string_interpolation: bit(44),
            extension_methods_in_collection_initializers: bit(45),
            local_functions: bit(46),
            static_local_functions: bit(47),
            aggressive_scalar_replacement_of_aggregates: bit(48),
            for_statement: bit(49),
            do_while_statement: bit(50),
            scoped_ref: bit(51),
            field_keyword: bit(52),
            always_qualify_member_references: bit(53),
            non_trailing_named_arguments: bit(54),
            using_declarations: bit(55),
            optional_arguments: bit(56),
            use_lambda_syntax: bit(57),
            default_literals: bit(58),
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
    /// `StatementTransformContext.RequestRerun()` from a transform that does not see the
    /// statement context; the statement driver takes it after each child.
    pub rerun_requested: bool,
    /// Variables a transform merged into others (`RecombineVariables`); the block driver
    /// removes them from the function's variables after each transform.
    pub removed_variables: Vec<super::ast::VarId>,
    /// The host's C# resolver (`ILTransformContext.CSharpResolver`); without one, the
    /// transforms answer as a resolver without a using scope does (no extension methods).
    pub resolver: Option<&'t dyn Resolver>,
    /// The names AssignVariableNames' root scope collects from the host's type system and using
    /// scope; without them it collects none.
    pub root_names: Option<&'t RootNames>,
}

/// What AssignVariableNames' root scope collects around the method: the lower-case member names
/// of the declaring type, which it only avoids, and the names it also reserves.
#[derive(Clone, Debug, Default)]
pub struct RootNames {
    pub member_names: Vec<String>,
    pub reserved_names: Vec<String>,
}

/// The questions the transforms ask the host's C# resolver.
pub trait Resolver {
    /// `CSharpResolver.CanTransformToExtensionMethodCall` for `query`; asked with the
    /// decompiled method's generic context.
    fn can_transform_to_extension_method_call(
        &self,
        ts: &TypeSystem<'_, '_>,
        query: &ExtensionMethodQuery<'_>,
    ) -> bool;

    /// `Disambiguator.ProbeField`: whether `field`, written as a simple name (`target` is
    /// `None`) or as a member access on `target`, binds back to the field, as the decompiled
    /// method's `ExpressionBuilder` resolves names (its current type and member included).
    fn field_reference_binds(
        &self,
        ts: &TypeSystem<'_, '_>,
        field: &super::ast::FieldOperand,
        target: Option<LookupTarget>,
    ) -> bool;

    /// The branch `ExpressionBuilder.ConvertField` takes for `field`, whose target is the
    /// `this` instance (or absent) when `target_is_this`.
    fn field_kind(
        &self,
        ts: &TypeSystem<'_, '_>,
        field: &super::ast::FieldOperand,
        target_is_this: bool,
    ) -> FieldKind;

    /// `Disambiguator.LookUpName` for an invocation with the written `type_arguments`: the
    /// method group `name` resolves to from `target` (`MemberLookup.Lookup`) or unqualified
    /// (`ResolveSimpleName`), its method definitions grouped by declaring type, base types
    /// first; `None` when the name is no method group.
    fn method_group(
        &self,
        ts: &TypeSystem<'_, '_>,
        name: &str,
        target: Option<LookupTarget>,
        type_arguments: &[TypeId],
    ) -> Option<Vec<MethodGroupList>>;

    /// `Disambiguator.IsUnambiguousAccess` for a property without parameters: what the name of
    /// the property `accessor` belongs to binds to, written as a simple name (`target` is
    /// `None`) or as a member access on `target`, with `IsAppropriateCallTarget` for `callvirt`.
    fn accessor_reference(
        &self,
        ts: &TypeSystem<'_, '_>,
        accessor: &super::ast::MethodOperand,
        target: Option<LookupTarget>,
        callvirt: bool,
    ) -> AccessorBinding;

    /// `CSharpResolver.CanTransformToExtensionMethodCall(method, type_arguments, target,
    /// arguments, names)` as the decompiled method's `ExpressionBuilder` resolves it, for
    /// `CallBuilder.TryUseExtensionMethodSyntax`; the target and the arguments are expressions
    /// or constants.
    fn can_use_extension_method_syntax(
        &self,
        ts: &TypeSystem<'_, '_>,
        method: &super::ast::MethodOperand,
        type_arguments: &[TypeId],
        target: &crate::semantics::ResolveResult,
        arguments: &[crate::semantics::ResolveResult],
        names: Option<&[Option<String>]>,
    ) -> Option<bool>;

    /// The constructors `IsUnambiguousCall` adds as candidates for `newobj` of `constructor`:
    /// those of its declaring type that the decompiled method can access, as one list;
    /// `None` when the host cannot name them all by metadata row.
    fn constructors(
        &self,
        ts: &TypeSystem<'_, '_>,
        constructor: &super::ast::MethodOperand,
    ) -> Option<Vec<MethodGroupList>>;
}

/// What a property name binds to (`Resolver::accessor_reference`).
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum AccessorBinding {
    /// No member, an error, or a member the accessor is not an appropriate call of.
    None,
    /// The accessor's own property.
    Owner,
    /// Another member an accessor call of it may write, such as the declaration it overrides.
    Other,
}

/// The methods of a method group one type declares (`MethodListWithDeclaringType`), as
/// definitions.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct MethodGroupList {
    pub declaring: crate::typesystem::compilation::TypeDefId,
    pub methods: Vec<crate::typesystem::members::MethodDefId>,
}

/// How `ConvertField` treats a field, which depends on the declarations around the body.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum FieldKind {
    /// An ordinary field reference.
    Ordinary,
    /// The C# 14 `field` keyword of the property whose accessor is decompiled.
    FieldKeyword,
    /// The backing field of an automatic property (the Property row): whether it needs a
    /// qualifier is decided for the property.
    AutoPropertyBackingField { property_row: u32 },
    /// A field named `field` inside a property accessor, which always needs a qualifier.
    NamedFieldInAccessor,
    /// The backing field of an automatic event (the Event row), written as the event.
    AutoEventBackingField { event_row: u32 },
}

/// The target of a member lookup, as its resolve result presents it to the resolver.
#[derive(Clone, Copy, Debug)]
pub enum LookupTarget {
    /// `this` or `base` (`ThisResolveResult`) of the given type.
    This {
        ty: TypeId,
        causes_non_virtual_invocation: bool,
    },
    /// A type name (`TypeResolveResult`).
    Type(TypeId),
    /// Any other expression of the given type.
    Expression(TypeId),
}

/// A `CanTransformToExtensionMethodCall` question.
pub enum ExtensionMethodQuery<'q> {
    /// `NullPropagationTransform.CanTransformToExtensionMethodCall(call)`: the call's target
    /// type and argument types, and when the target loads a single-definition variable stored
    /// once, that variable's type and the stored value's type.
    Call {
        method: &'q super::ast::MethodOperand,
        target: TypeId,
        target_store: Option<(TypeId, TypeId)>,
        arguments: Vec<TypeId>,
    },
    /// `CanTransformToExtensionMethodCall(method, ignoreTypeArguments: true)`.
    Method {
        method: &'q super::ast::MethodOperand,
    },
}

/// A pipeline leaf: a whole-function transform, or a post-order block transform of the
/// `BlockILTransform` group with the given index. Consecutive leaves of one group run in one
/// pass of the block driver, as the managed group does. A statement leaf is a child of the
/// group's `StatementTransform`; consecutive ones share it.
#[derive(Clone, Copy)]
enum Leaf {
    Function(fn(&mut ReadFunction, &mut Context<'_, '_, '_>)),
    Block(u8, block_transform::BlockTransform),
    Statement(u8, statement_transform::StatementTransform),
}

use Leaf::{Block, Function, Statement};

impl Leaf {
    fn group(&self) -> Option<u8> {
        match *self {
            Function(_) => None,
            Block(g, _) | Statement(g, _) => Some(g),
        }
    }
}

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
    Statement(1, |r, ctx, _, _, block, pos| {
        il_inlining::run_statement(
            r,
            ctx,
            block,
            pos,
            il_inlining::options::ALLOW_INLINING_OF_LDLOCA,
        )
    }),
    Statement(1, |r, ctx, _, _, block, pos| {
        expression_transforms::run_statement(&mut r.function, ctx, block, pos)
    }),
    Statement(1, dynamic_is_event_assignment_transform::run),
    Statement(1, transform_assignment::run),
    Statement(1, null_coalescing_transform::run),
    Statement(1, nullable_lifting_transform::run_statement),
    Statement(1, null_propagation_transform::run_statement),
    Statement(1, transform_array_initializers::run),
    Statement(1, transform_collection_and_object_initializers::run),
    Statement(1, transform_expression_trees::run),
    Statement(1, index_range_transform::run),
    Statement(1, deconstruction_transform::run),
    Statement(1, named_argument_transform::run),
    Statement(1, remove_unconstrained_generic_reference_type_check::run),
    Statement(1, user_defined_logic_transform::run),
    Statement(1, interpolated_string_transform::run),
    Function(proxy_call_replacer::run),
    Function(fix_remaining_increments::run),
    Function(copy_propagation::run),
    Function(delegate_construction::run),
    Function(local_function_decompiler::run),
    Function(transform_display_class_usage::run),
    Function(high_level_loop_transform::run),
    Function(reduce_nesting_transform::run),
    Function(super::control_flow::remove_redundant_return::run),
    Function(introduce_dynamic_type_on_locals::run),
    Function(introduce_native_int_type_on_locals::run),
    Function(introduce_scoped_modifier_on_locals::run),
    Function(expand_nested_conditionals::run),
    Function(assign_variable_names::run),
];

/// The index of DelegateConstruction in `PIPELINE`: a lambda runs the leaves before it.
pub const DELEGATE_CONSTRUCTION_LEAF: u32 = 48;

/// The index of LocalFunctionDecompiler in `PIPELINE`: a local function runs the leaves
/// before it.
pub const LOCAL_FUNCTION_DECOMPILER_LEAF: u32 = 49;

/// Runs the first `prefix` leaves of the pipeline.
///
/// # Panics
/// When `prefix` exceeds `IL_PREFIX`; the FFI layer rejects such requests first.
pub fn run_prefix(r: &mut ReadFunction, ctx: &mut Context<'_, '_, '_>, prefix: u32) {
    use block_transform::BlockStep;
    let mut leaves = PIPELINE[..prefix as usize].iter().peekable();
    while let Some(leaf) = leaves.next() {
        let Some(group) = leaf.group() else {
            if let Function(transform) = *leaf {
                transform(r, ctx);
                r.flush_pending_variables();
            }
            continue;
        };
        let mut steps: Vec<BlockStep> = Vec::new();
        let mut next = Some(leaf);
        while let Some(leaf) = next {
            match (*leaf, steps.last_mut()) {
                (Statement(_, t), Some(BlockStep::Statements(children))) => children.push(t),
                (Statement(_, t), _) => steps.push(BlockStep::Statements(vec![t])),
                (Block(_, t), _) => steps.push(BlockStep::Block(t)),
                (Function(_), _) => unreachable!("a group leaf"),
            }
            next = leaves.next_if(|l| l.group() == Some(group));
        }
        block_transform::run(r, ctx, &steps);
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
                let n = format!("I_{}", self.function.helper_variable_count);
                self.function.helper_variable_count += 1;
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
            capture_scope: None,
            is_scoped: false,
            is_scoped_without_initializer: false,
        });
        self.flush_pending_variables();
        self.variables.push(v);
        v
    }

    /// Applies what transforms queued for the variable list, in the order the managed
    /// collection sees it: first the removals (`RecombineVariables`; `Remove` swap-removes),
    /// then the variables `Function::register_variable` created.
    pub fn apply_variable_changes(&mut self, removed: &mut Vec<super::ast::VarId>) {
        for v in std::mem::take(removed) {
            if let Some(i) = self.variables.iter().position(|&x| x == v) {
                self.variables.swap_remove(i);
            }
        }
        self.flush_pending_variables();
    }

    /// Appends the variables `Function::register_variable` created to the variable list.
    pub fn flush_pending_variables(&mut self) {
        self.variables.append(&mut self.function.pending_variables);
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

    #[test]
    fn delegate_construction_follows_copy_propagation() {
        let index = super::DELEGATE_CONSTRUCTION_LEAF as usize;
        let super::Leaf::Function(previous) = super::PIPELINE[index - 1] else {
            panic!("CopyPropagation is a function leaf");
        };
        assert_eq!(
            previous as usize,
            super::copy_propagation::run as fn(&mut _, &mut _) as usize
        );
    }
}
