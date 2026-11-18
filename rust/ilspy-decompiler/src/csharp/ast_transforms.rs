//! The AST transforms the engine runs on the C# syntax tree of a request (`AST_PREFIX` of
//! `CSharpDecompiler.GetAstTransforms`, in order), between the host's tree building and the
//! host's remaining transforms.
//!
//! Each ported transform is registered in `TRANSFORMS` under its index in the built-in list.
//! `run_one` runs a single registered transform, out of order, which `AstTransformEqualityTests`
//! compares with the managed transform on the same tree; `run` runs the served prefix.

use super::syntax::SyntaxTree;
use super::syntax_oracle::SyntaxOracle;
use crate::typesystem::types::TypeSystem;

/// A ported AST transform: `IAstTransform.Run` on the whole tree.
pub type Transform =
    fn(&mut TypeSystem<'_, '_>, &mut SyntaxTree, &dyn SyntaxOracle) -> Result<(), String>;

/// The names of the built-in AST transforms, in `GetAstTransforms` order; the host checks them
/// against its list.
pub const BUILT_IN: &[&str] = &[
    "PatternStatementTransform",
    "RewriteVisualBasicWithEventsInInitializeComponent",
    "ReplaceMethodCallsWithOperators",
    "IntroduceUnsafeModifier",
    "AddCheckedBlocks",
    "DeclareVariables",
    "TransformFieldAndConstructorInitializers",
    "PrettifyAssignments",
    "IntroduceUsingDeclarations",
    "IntroduceQueryExpressions",
    "CombineQueryExpressions",
    "NormalizeBlockStatements",
    "FlattenSwitchBlocks",
    "RenameVisualBasicAnonymousTypes",
    "FixNameCollisions",
    "AddXmlDocumentationTransform",
];

/// The ported transforms, by their index in `BUILT_IN`.
const TRANSFORMS: &[(usize, Transform)] = &[
    (0, super::transforms::pattern_statement_transform::run),
    (1, super::transforms::rewrite_visual_basic_with_events::run),
    (
        2,
        super::transforms::replace_method_calls_with_operators::run,
    ),
    (3, super::transforms::introduce_unsafe_modifier::run),
    (4, super::transforms::add_checked_blocks::run),
    (5, super::transforms::declare_variables::run),
    (
        6,
        super::transforms::field_and_constructor_initializers::run,
    ),
    (7, super::transforms::prettify_assignments::run),
    (8, super::transforms::introduce_using_declarations::run),
    (9, super::transforms::introduce_query_expressions::run),
    (10, super::transforms::combine_query_expressions::run),
    (11, super::transforms::normalize_block_statements::run),
    (
        13,
        super::transforms::rename_visual_basic_anonymous_types::run,
    ),
    (14, super::transforms::fix_name_collisions::run),
    (15, super::transforms::add_xml_documentation::run),
];

fn transform(index: usize) -> Option<Transform> {
    TRANSFORMS
        .iter()
        .find(|(i, _)| *i == index)
        .map(|(_, t)| *t)
}

/// Runs the first `prefix` built-in AST transforms on `tree`. `flags` are the transforms the
/// caller put before the built-in ones (`AstRequestFlags`), which the engine performs first;
/// `oracle` answers questions about the host objects the tree refers to.
pub fn run(
    ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
    prefix: u32,
    flags: u32,
) -> Result<(), String> {
    if prefix > crate::AST_PREFIX {
        return Err(format!(
            "AST prefix {prefix} is beyond the engine's {}",
            crate::AST_PREFIX
        ));
    }
    if prefix > 0 && flags != 0 {
        return Err("early AST transforms the engine does not perform yet".to_owned());
    }
    for (index, name) in BUILT_IN.iter().enumerate().take(prefix as usize) {
        let t =
            transform(index).ok_or_else(|| format!("the AST transform {name} is not ported"))?;
        t(ts, tree, oracle)?;
        tree.materialize_new_annotations(oracle)?;
    }
    Ok(())
}

/// Runs only the built-in transform `index` on `tree`, whatever the served prefix: the check of
/// a single ported transform against the managed one.
pub fn run_one(
    ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
    index: usize,
) -> Result<(), String> {
    let t = transform(index).ok_or_else(|| {
        format!(
            "the AST transform {} is not ported",
            BUILT_IN.get(index).copied().unwrap_or("?")
        )
    })?;
    t(ts, tree, oracle)
}

/// Whether the built-in transform `index` is ported.
pub fn is_ported(index: usize) -> bool {
    transform(index).is_some()
}
