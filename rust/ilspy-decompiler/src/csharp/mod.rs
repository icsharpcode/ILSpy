//! The translation of a transformed ILAst function body into C# syntax
//! (`StatementBuilder`, `ExpressionBuilder`, `TranslatedExpression`).
//!
//! The builders cover a growing set of constructs. A body with anything outside that set is
//! not translated: `convert_body` answers why, and the host translates the body itself.

pub mod ast;
mod expression_builder;
mod statement_builder;
mod translated_expression;

use crate::il::ast::{Function, NodeId};
use crate::typesystem::types::{TypeId, TypeSystem};

pub use statement_builder::convert_body;
pub use translated_expression::TranslatedExpression;

/// Why a body is not translated.
pub type Unsupported = String;

/// The state `StatementBuilder` and its `ExpressionBuilder` share while translating one
/// function.
pub(crate) struct Builder<'f, 't, 'c, 'a> {
    pub(crate) f: &'f Function,
    pub(crate) ts: &'t mut TypeSystem<'c, 'a>,
    pub(crate) ast: ast::Ast,
    /// `currentReturnContainer`.
    pub(crate) return_container: NodeId,
    /// `currentResultType`.
    pub(crate) result_type: TypeId,
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
