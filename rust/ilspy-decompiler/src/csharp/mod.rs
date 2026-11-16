//! The translation of a transformed ILAst function body into C# syntax
//! (`StatementBuilder`, `ExpressionBuilder`, `TranslatedExpression`).
//!
//! The builders cover a growing set of constructs. A body with anything outside that set is
//! not translated: `convert_body` answers why, and the host translates the body itself.

pub mod ast;
mod call_builder;
mod deconstruct;
mod delegate;
mod expression_builder;
mod foreach;
mod lambda;
mod local_function;
mod member_access;
mod statement_builder;
mod switch;
mod translated_expression;
pub mod type_syntax;

use crate::il::ast::{Function, NodeId};
use crate::typesystem::types::{TypeId, TypeSystem};

pub use statement_builder::convert_body;
pub use translated_expression::TranslatedExpression;

/// Why a body is not translated.
pub type Unsupported = String;

/// The state `StatementBuilder` and its `ExpressionBuilder` share while translating one
/// function.
pub(crate) struct Builder<'f, 't, 'c, 'a> {
    /// The function being translated: a copy the translation may change (as
    /// `TransformToForeach` changes the ILAst), which is what the host receives with the body.
    pub(crate) f: &'f mut Function,
    pub(crate) ts: &'t mut TypeSystem<'c, 'a>,
    pub(crate) ast: ast::Ast,
    /// `currentReturnContainer`.
    pub(crate) return_container: NodeId,
    /// `currentResultType`.
    pub(crate) result_type: TypeId,
    pub(crate) settings: crate::il::transforms::Settings,
    /// The host's C# resolver, for the `Disambiguator` checks; without it no member reference
    /// is translated.
    pub(crate) resolver: Option<&'f dyn crate::il::transforms::Resolver>,
    /// `decompilationContext.CurrentTypeDefinition`: the declaring type of the method.
    pub(crate) current_type: Option<crate::typesystem::compilation::TypeDefId>,
    /// The names of `currentFunction.Variables`.
    pub(crate) variable_names: Vec<String>,
    /// `continueTarget`: the block a branch to is written as `continue`.
    pub(crate) continue_target: Option<NodeId>,
    /// `continueCount`: the branches written as `continue` so far.
    pub(crate) continue_count: u32,
    /// `breakTarget`: the container a leave of is written as `break`.
    pub(crate) break_target: Option<NodeId>,
    /// The return statements of the current function, for the return type a lambda infers.
    pub(crate) return_statements: Vec<ast::AstId>,
    /// `currentIsIterator`: a leave of the return container is `yield break`.
    pub(crate) is_iterator: bool,
    /// Whether the current function is async (`AsyncReturnType` set), whose `yield return`
    /// values are of the result type itself.
    pub(crate) is_async: bool,
    /// Whether the current function is a lambda, whose returns can keep their type with a cast.
    pub(crate) in_lambda: bool,
    /// The variables of the top-level function (`ReadFunction::variables`).
    pub(crate) root_variables: Vec<crate::il::ast::VarId>,
    /// The override the last successful call probe resolved to, in place of the method called.
    pub(crate) found_override: Option<crate::semantics::Method>,
    /// `loadedVariablesSet`: the single-definition stack slots loaded so far, whose type a
    /// later store keeps.
    pub(crate) loaded_stack_slots: Vec<crate::il::ast::VarId>,
    /// The parameter of each argument of the call a `CallWithNamedArgs` block translates.
    pub(crate) named_argument_map: Option<Vec<usize>>,
    /// The format string nodes `string.Format` calls written as interpolated strings consumed.
    pub(crate) consumed_format_strings: Vec<usize>,
    /// `ExpectedTargetDetails.NeedsBoxingConversion` of the call being disambiguated.
    pub(crate) needs_boxing_conversion: bool,
    /// `currentFunction.LocalFunctions`: the local functions `DeclareLocalFunctions` declares.
    pub(crate) local_functions: Vec<NodeId>,
    /// How many local functions have been declared.
    pub(crate) declared_local_functions: usize,
    /// Whether the current function is a local function.
    pub(crate) in_local_function: bool,
    /// `currentFunction` where it is a nested function (a lambda or local function).
    pub(crate) current_function: Option<NodeId>,
    /// `labels`: the label `EnsureUniqueLabel` gave each block of the current function.
    pub(crate) labels: Vec<(NodeId, String)>,
    /// `caseLabelMapping`: the block each section of the current switch inlines, with the
    /// first value of its labels (`None` for the default section) and the case type, which a
    /// branch to the block writes as `goto case`.
    pub(crate) case_label_mapping: Vec<(
        NodeId,
        Option<switch::CaseValue>,
        crate::typesystem::types::TypeId,
    )>,
    /// `endContainerLabels`: the label `goto` of a leave out of each container targets.
    pub(crate) end_container_labels: Vec<(NodeId, String)>,
    /// `duplicateLabels`: how many blocks share each `Block.Label`.
    pub(crate) duplicate_labels: std::collections::HashMap<String, u32>,
}

/// The variant name of an instruction, for the reasons of untranslated bodies.
pub(crate) fn inst_name(inst: &crate::il::ast::Inst) -> String {
    let debug = format!("{inst:?}");
    debug
        .split([' ', '{', '('])
        .next()
        .unwrap_or_default()
        .to_owned()
}
