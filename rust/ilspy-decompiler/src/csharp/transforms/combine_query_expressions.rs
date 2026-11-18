//! `CombineQueryExpressions`: merges a query whose `from` clause reads from another query, either
//! by removing the transparent identifier of a `select new { ... }` (the inner clauses move into
//! the outer query, with `let` clauses for computed members) or with a query continuation
//! (`... into x`), and moves the type argument of a `Cast<T>()` source into the `from` clause.
//!
//! Annotations it creates: `ANNOTATION_LET_IDENTIFIER` (168), a new `LetIdentifierAnnotation`
//! (no payload). The facts about the host's annotations come from `QUERY_QE_ANNOTATION`
//! (`introduce_query_expressions`).

use std::collections::HashMap;

use super::introduce_query_expressions::{
    Annotations, QE_IL_VARIABLE, QE_MEMBER_RESOLVE_RESULT, name_in, new_identifier_expression,
    setting,
};
use super::introduce_unsafe_modifier::copy_annotations_from;
use super::rewrite_visual_basic_with_events::{identifier_expression_name, new_identifier};
use crate::csharp::syntax::{SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::SyntaxOracle;
use crate::csharp::syntax_schema::{
    anonymous_type_create_expression, identifier_expression, invocation_expression, kind,
    member_reference_expression, named_expression, query_continuation_clause, query_expression,
    query_from_clause, query_let_clause, query_select_clause,
};
use crate::typesystem::types::TypeSystem;

/// `new LetIdentifierAnnotation()`.
pub const ANNOTATION_LET_IDENTIFIER: u8 = 168;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    if !setting(oracle, "QueryExpressions")? {
        return Ok(());
    }
    let mut cx = Cx {
        annotations: Annotations::new(oracle),
        // One table for the whole tree, as the managed transform keeps it.
        from_or_let_identifiers: HashMap::new(),
    };
    cx.combine_queries(tree, root)
}

/// `CSharpDecompiler.IsTransparentIdentifier`.
fn is_transparent_identifier(name: &[u16]) -> bool {
    let name = String::from_utf16_lossy(name);
    if name.starts_with("<>") {
        return name.contains("TransparentIdentifier") || name.contains("TranspIdent");
    }
    // The VB compiler names the carriers of its query range variables $VB$It, $VB$It1, ...
    name.starts_with("$VB$It")
}

fn member_name(tree: &SyntaxTree, mre: SyntaxId) -> Vec<u16> {
    name_in(
        tree,
        mre,
        member_reference_expression::slot::MEMBER_NAME_TOKEN,
    )
}

fn has_type_arguments(tree: &SyntaxTree, id: SyntaxId, slot: usize) -> bool {
    !tree.children(id, slot).is_empty()
}

/// `castPattern`: `inExpr.Cast<targetType>()`; the in expression and the target type.
fn match_cast(tree: &SyntaxTree, expr: Option<SyntaxId>) -> Option<(SyntaxId, SyntaxId)> {
    let invocation = expr.filter(|&e| tree.kind(e) == kind::INVOCATION_EXPRESSION)?;
    if !tree
        .children(invocation, invocation_expression::slot::ARGUMENTS)
        .is_empty()
    {
        return None;
    }
    let mre = tree
        .child(invocation, invocation_expression::slot::TARGET)
        .filter(|&t| tree.kind(t) == kind::MEMBER_REFERENCE_EXPRESSION)?;
    if !member_name(tree, mre)
        .iter()
        .copied()
        .eq("Cast".encode_utf16())
    {
        return None;
    }
    let in_expr = tree.child(mre, member_reference_expression::slot::TARGET)?;
    let [target_type] = *tree.children(mre, member_reference_expression::slot::TYPE_ARGUMENTS)
    else {
        return None;
    };
    Some((in_expr, target_type))
}

/// `selectTransparentIdentifierPattern`: `select new { a, x.b, c = expr, ... }` with at least one
/// member; the members.
fn match_select_transparent_identifier(
    tree: &SyntaxTree,
    select: SyntaxId,
) -> Option<Vec<SyntaxId>> {
    let create = tree
        .child(select, query_select_clause::slot::EXPRESSION)
        .filter(|&e| tree.kind(e) == kind::ANONYMOUS_TYPE_CREATE_EXPRESSION)?;
    let initializers = tree.children(create, anonymous_type_create_expression::slot::INITIALIZERS);
    let matches = |&e: &SyntaxId| match tree.kind(e) {
        kind::IDENTIFIER_EXPRESSION => {
            !has_type_arguments(tree, e, identifier_expression::slot::TYPE_ARGUMENTS)
        }
        kind::MEMBER_REFERENCE_EXPRESSION => {
            tree.child(e, member_reference_expression::slot::TARGET)
                .is_some()
                && !has_type_arguments(tree, e, member_reference_expression::slot::TYPE_ARGUMENTS)
        }
        kind::NAMED_EXPRESSION => tree.child(e, named_expression::slot::EXPRESSION).is_some(),
        _ => false,
    };
    (!initializers.is_empty() && initializers.iter().all(matches)).then(|| initializers.to_vec())
}

struct Cx<'a> {
    annotations: Annotations<'a>,
    /// A name of a range variable and the annotation its references get: the
    /// `ILVariableResolveResult` of the identifier, a `LetIdentifierAnnotation`, or none.
    from_or_let_identifiers: HashMap<Vec<u16>, Option<u32>>,
}

impl Cx<'_> {
    fn combine_queries(&mut self, tree: &mut SyntaxTree, node: SyntaxId) -> Result<(), String> {
        // A child's visit may replace that child, never its siblings.
        for child in tree.all_children(node) {
            self.combine_queries(tree, child)?;
        }
        if tree.kind(node) != kind::QUERY_EXPRESSION {
            return Ok(());
        }
        let query = node;
        let from = tree
            .children(query, query_expression::slot::CLAUSES)
            .first()
            .copied()
            .filter(|&c| tree.kind(c) == kind::QUERY_FROM_CLAUSE)
            .ok_or("a query expression does not start with a from clause")?;
        let expression = tree.child(from, query_from_clause::slot::EXPRESSION);
        if let Some(inner) = expression.filter(|&e| tree.kind(e) == kind::QUERY_EXPRESSION) {
            if self.try_remove_transparent_identifier(tree, query, from, inner) {
                self.remove_transparent_identifier_references(tree, query);
            } else {
                let continuation = tree.new_node(kind::QUERY_CONTINUATION_CLAUSE);
                tree.set_child(
                    continuation,
                    query_continuation_clause::slot::PRECEDING_QUERY,
                    Some(inner),
                );
                let name = name_in(tree, from, query_from_clause::slot::IDENTIFIER_TOKEN);
                let identifier = new_identifier(tree, &name);
                tree.set_child(
                    continuation,
                    query_continuation_clause::slot::IDENTIFIER_TOKEN,
                    Some(identifier),
                );
                copy_annotations_from(tree, continuation, from);
                tree.replace(from, continuation);
            }
        } else if let Some((in_expr, target_type)) = match_cast(tree, expression) {
            tree.set_child(from, query_from_clause::slot::TYPE, Some(target_type));
            tree.set_child(from, query_from_clause::slot::EXPRESSION, Some(in_expr));
        }
        Ok(())
    }

    /// `TryRemoveTransparentIdentifier`.
    fn try_remove_transparent_identifier(
        &mut self,
        tree: &mut SyntaxTree,
        query: SyntaxId,
        from: SyntaxId,
        inner: SyntaxId,
    ) -> bool {
        let name = name_in(tree, from, query_from_clause::slot::IDENTIFIER_TOKEN);
        if !is_transparent_identifier(&name) {
            return false;
        }
        let Some(select) = tree
            .children(inner, query_expression::slot::CLAUSES)
            .last()
            .copied()
            .filter(|&c| tree.kind(c) == kind::QUERY_SELECT_CLAUSE)
        else {
            return false;
        };
        let Some(members) = match_select_transparent_identifier(tree, select) else {
            return false;
        };

        // from * in (from x in ... select new { members of anonymous type }) ...
        // =>
        // from x in ... { let x = ... } ...
        tree.detach(from);
        tree.detach(select);
        // Move clauses from innerQuery to query
        let clauses = tree
            .children(inner, query_expression::slot::CLAUSES)
            .to_vec();
        let insertion = clauses.len();
        for (i, clause) in clauses.into_iter().enumerate() {
            tree.insert_child(query, query_expression::slot::CLAUSES, i, clause);
        }

        for expr in members {
            match tree.kind(expr) {
                kind::IDENTIFIER_EXPRESSION => {
                    let name = identifier_expression_name(tree, expr).unwrap_or_default();
                    let annotation = self.annotations.first(tree, expr, QE_IL_VARIABLE);
                    self.from_or_let_identifiers.insert(name, annotation);
                }
                kind::MEMBER_REFERENCE_EXPRESSION => {
                    let name = member_name(tree, expr);
                    self.add_query_let_clause(tree, query, insertion, name, expr);
                }
                _ => {
                    let name = name_in(tree, expr, named_expression::slot::NAME_TOKEN);
                    let Some(value) = tree.child(expr, named_expression::slot::EXPRESSION) else {
                        continue;
                    };
                    if identifier_expression_name(tree, value).is_some_and(|n| n == name) {
                        let annotation = self.annotations.first(tree, value, QE_IL_VARIABLE);
                        self.from_or_let_identifiers.insert(name, annotation);
                        continue;
                    }
                    self.add_query_let_clause(tree, query, insertion, name, value);
                }
            }
        }
        true
    }

    /// `AddQueryLetClause`: every let clause goes right after the moved clauses, so later ones
    /// come first.
    fn add_query_let_clause(
        &mut self,
        tree: &mut SyntaxTree,
        query: SyntaxId,
        insertion: usize,
        name: Vec<u16>,
        expression: SyntaxId,
    ) {
        let let_clause = tree.new_node(kind::QUERY_LET_CLAUSE);
        let identifier = new_identifier(tree, &name);
        tree.set_child(
            let_clause,
            query_let_clause::slot::IDENTIFIER_TOKEN,
            Some(identifier),
        );
        tree.set_child(
            let_clause,
            query_let_clause::slot::EXPRESSION,
            Some(expression),
        );
        let annotation = tree.new_annotation(ANNOTATION_LET_IDENTIFIER, Vec::new());
        tree.node_mut(let_clause).annotations.push(annotation);
        self.from_or_let_identifiers.insert(name, Some(annotation));
        tree.insert_child(
            query,
            query_expression::slot::CLAUSES,
            insertion,
            let_clause,
        );
    }

    /// `RemoveTransparentIdentifierReferences`.
    fn remove_transparent_identifier_references(&mut self, tree: &mut SyntaxTree, node: SyntaxId) {
        for child in tree.all_children(node) {
            self.remove_transparent_identifier_references(tree, child);
        }
        if tree.kind(node) != kind::MEMBER_REFERENCE_EXPRESSION {
            return;
        }
        let mre = node;
        let Some(target) = tree.child(mre, member_reference_expression::slot::TARGET) else {
            return;
        };
        if !identifier_expression_name(tree, target).is_some_and(|n| is_transparent_identifier(&n))
        {
            return;
        }
        let name = member_name(tree, mre);
        let new_ident = new_identifier_expression(tree, &name);
        let type_arguments = tree
            .children(mre, member_reference_expression::slot::TYPE_ARGUMENTS)
            .to_vec();
        for (i, t) in type_arguments.into_iter().enumerate() {
            tree.insert_child(new_ident, identifier_expression::slot::TYPE_ARGUMENTS, i, t);
        }
        copy_annotations_from(tree, new_ident, mre);
        // remove the reference to the property of the anonymous type
        self.annotations
            .remove(tree, new_ident, QE_MEMBER_RESOLVE_RESULT);
        if let Some(&Some(annotation)) = self.from_or_let_identifiers.get(&name) {
            tree.node_mut(new_ident).annotations.push(annotation);
        }
        tree.replace(mre, new_ident);
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::transforms::rewrite_visual_basic_with_events::utf16;

    #[test]
    fn transparent_identifiers_are_recognized() {
        assert!(is_transparent_identifier(&utf16(
            "<>h__TransparentIdentifier0"
        )));
        assert!(is_transparent_identifier(&utf16("<>h__TranspIdent1")));
        assert!(is_transparent_identifier(&utf16("$VB$ItAnonymous")));
        assert!(!is_transparent_identifier(&utf16("$VB$Me")));
        assert!(!is_transparent_identifier(&utf16("TransparentIdentifier")));
        assert!(!is_transparent_identifier(&utf16("<>x")));
    }

    /// `x.Cast<T>()` with `x` and `T` as given.
    fn cast(tree: &mut SyntaxTree, type_arguments: usize, name: &str) -> SyntaxId {
        let mre = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
        let target = new_identifier_expression(tree, &utf16("x"));
        tree.set_child(mre, member_reference_expression::slot::TARGET, Some(target));
        let member = new_identifier(tree, &utf16(name));
        tree.set_child(
            mre,
            member_reference_expression::slot::MEMBER_NAME_TOKEN,
            Some(member),
        );
        for i in 0..type_arguments {
            let t = tree.new_node(kind::PRIMITIVE_TYPE);
            tree.insert_child(mre, member_reference_expression::slot::TYPE_ARGUMENTS, i, t);
        }
        let invocation = tree.new_node(kind::INVOCATION_EXPRESSION);
        tree.set_child(invocation, invocation_expression::slot::TARGET, Some(mre));
        invocation
    }

    #[test]
    fn the_cast_pattern_needs_one_type_argument() {
        let mut tree = SyntaxTree::default();
        let good = cast(&mut tree, 1, "Cast");
        assert!(match_cast(&tree, Some(good)).is_some());
        let two = cast(&mut tree, 2, "Cast");
        assert!(match_cast(&tree, Some(two)).is_none());
        let other = cast(&mut tree, 1, "OfType");
        assert!(match_cast(&tree, Some(other)).is_none());
    }

    #[test]
    fn the_select_pattern_takes_identifiers_members_and_named_expressions() {
        let mut tree = SyntaxTree::default();
        let create = tree.new_node(kind::ANONYMOUS_TYPE_CREATE_EXPRESSION);
        let select = tree.new_node(kind::QUERY_SELECT_CLAUSE);
        tree.set_child(select, query_select_clause::slot::EXPRESSION, Some(create));
        assert!(match_select_transparent_identifier(&tree, select).is_none());
        let a = new_identifier_expression(&mut tree, &utf16("a"));
        tree.insert_child(
            create,
            anonymous_type_create_expression::slot::INITIALIZERS,
            0,
            a,
        );
        assert_eq!(
            match_select_transparent_identifier(&tree, select),
            Some(vec![a])
        );
        let other = tree.new_node(kind::PRIMITIVE_EXPRESSION);
        tree.insert_child(
            create,
            anonymous_type_create_expression::slot::INITIALIZERS,
            1,
            other,
        );
        assert!(match_select_transparent_identifier(&tree, select).is_none());
    }
}
