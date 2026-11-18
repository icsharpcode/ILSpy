//! `NormalizeBlockStatements`: puts the embedded statements of loops, `fixed`, `lock`, `if` and
//! `using` into blocks or takes them out of redundant ones, by the `AlwaysUseBraces` setting;
//! turns calculated getter-only properties and indexers into expression-bodied ones; and makes
//! the only namespace of a syntax tree file-scoped.
//!
//! A depth-first visit in child order: statements are handled after their children,
//! properties and indexers before theirs. The settings come from `QUERY_SETTING`.

use super::prettify_assignments::copy_annotations_from;
use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::{self, SyntaxOracle};
use crate::csharp::syntax_schema::{
    accessor, block_statement, do_while_statement, fixed_statement, for_statement,
    foreach_statement, if_else_statement, indexer_declaration, kind, lock_statement,
    namespace_declaration, property_declaration, return_statement, using_statement,
    while_statement,
};
use crate::typesystem::types::TypeSystem;

/// `Modifiers.Readonly`, the modifiers that move from an accessor to its property.
const MOVABLE_MODIFIERS: i64 = 0x0200;

struct Normalize {
    always_use_braces: bool,
    file_scoped_namespaces: bool,
    expression_bodies: bool,
    has_namespace: bool,
    single_namespace: Option<SyntaxId>,
}

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let setting = |name: &str| {
        syntax_oracle::setting(oracle, name)
            .ok_or_else(|| format!("the host did not answer the {name} setting"))
    };
    let mut n = Normalize {
        always_use_braces: setting("AlwaysUseBraces")?,
        file_scoped_namespaces: setting("FileScopedNamespaces")?,
        expression_bodies: setting("UseExpressionBodyForCalculatedGetterOnlyProperties")?,
        has_namespace: false,
        single_namespace: None,
    };
    if let Some(root) = tree.root {
        n.visit(tree, root);
    }
    Ok(())
}

impl Normalize {
    fn visit_children(&mut self, tree: &mut SyntaxTree, id: SyntaxId) {
        // The visit of a child may replace that child's children, never its siblings.
        for child in tree.all_children(id) {
            self.visit(tree, child);
        }
    }

    fn visit(&mut self, tree: &mut SyntaxTree, id: SyntaxId) {
        match tree.kind(id) {
            kind::SYNTAX_TREE => {
                self.single_namespace = None;
                self.has_namespace = false;
                self.visit_children(tree, id);
                if self.file_scoped_namespaces
                    && let Some(ns) = self.single_namespace
                {
                    tree.node_mut(ns).scalars[namespace_declaration::scalar::IS_FILE_SCOPED] =
                        Scalar::Bool(true);
                }
            }
            kind::NAMESPACE_DECLARATION => {
                self.single_namespace = None;
                if !self.has_namespace {
                    self.has_namespace = true;
                    self.single_namespace = Some(id);
                }
                self.visit_children(tree, id);
            }
            kind::IF_ELSE_STATEMENT => {
                self.visit_children(tree, id);
                let t = tree.child(id, if_else_statement::slot::TRUE_STATEMENT);
                self.do_transform(tree, t, id);
                let f = tree.child(id, if_else_statement::slot::FALSE_STATEMENT);
                self.do_transform(tree, f, id);
            }
            kind::USING_STATEMENT => {
                self.visit_children(tree, id);
                let s = tree.child(id, using_statement::slot::EMBEDDED_STATEMENT);
                self.do_transform(tree, s, id);
            }
            k @ (kind::WHILE_STATEMENT
            | kind::DO_WHILE_STATEMENT
            | kind::FOREACH_STATEMENT
            | kind::FOR_STATEMENT
            | kind::FIXED_STATEMENT
            | kind::LOCK_STATEMENT) => {
                self.visit_children(tree, id);
                let slot = match k {
                    kind::WHILE_STATEMENT => while_statement::slot::EMBEDDED_STATEMENT,
                    kind::DO_WHILE_STATEMENT => do_while_statement::slot::EMBEDDED_STATEMENT,
                    kind::FOREACH_STATEMENT => foreach_statement::slot::EMBEDDED_STATEMENT,
                    kind::FOR_STATEMENT => for_statement::slot::EMBEDDED_STATEMENT,
                    kind::FIXED_STATEMENT => fixed_statement::slot::EMBEDDED_STATEMENT,
                    _ => lock_statement::slot::EMBEDDED_STATEMENT,
                };
                if let Some(s) = tree.child(id, slot) {
                    insert_block(tree, s);
                }
            }
            kind::PROPERTY_DECLARATION => {
                if self.expression_bodies {
                    simplify(
                        tree,
                        id,
                        property_declaration::slot::GETTER,
                        property_declaration::slot::EXPRESSION_BODY,
                        property_declaration::scalar::MODIFIERS,
                    );
                }
                self.visit_children(tree, id);
            }
            kind::INDEXER_DECLARATION => {
                if self.expression_bodies {
                    simplify(
                        tree,
                        id,
                        indexer_declaration::slot::GETTER,
                        indexer_declaration::slot::EXPRESSION_BODY,
                        indexer_declaration::scalar::MODIFIERS,
                    );
                }
                self.visit_children(tree, id);
            }
            _ => self.visit_children(tree, id),
        }
    }

    fn do_transform(&self, tree: &mut SyntaxTree, statement: Option<SyntaxId>, parent: SyntaxId) {
        let Some(statement) = statement else {
            return;
        };
        if self.always_use_braces {
            if !is_else_if(tree, statement, parent) {
                insert_block(tree, statement);
            }
        } else if tree.kind(statement) == kind::BLOCK_STATEMENT
            && let [inner] = *tree.children(statement, block_statement::slot::STATEMENTS)
            && is_allowed_as_embedded_statement(tree, inner, Some(parent))
        {
            tree.detach(inner);
            tree.replace(statement, inner);
        } else if !is_allowed_as_embedded_statement(tree, statement, Some(parent)) {
            insert_block(tree, statement);
        }
    }
}

/// Whether `statement` is in a slot of kind `FalseStatement` of its parent.
fn in_false_statement_slot(tree: &SyntaxTree, statement: SyntaxId) -> bool {
    let Some(parent) = tree.node(statement).parent else {
        return false;
    };
    tree.parent_slot(statement)
        .is_some_and(|s| tree.node(parent).schema().slots[s].kind == "FalseStatement")
}

fn is_else_if(tree: &SyntaxTree, statement: SyntaxId, parent: SyntaxId) -> bool {
    tree.kind(parent) == kind::IF_ELSE_STATEMENT && in_false_statement_slot(tree, statement)
}

fn insert_block(tree: &mut SyntaxTree, statement: SyntaxId) {
    if tree.kind(statement) == kind::BLOCK_STATEMENT {
        return;
    }
    let b = tree.new_node(kind::BLOCK_STATEMENT);
    tree.replace(statement, b);
    if tree.kind(statement) == kind::EMPTY_STATEMENT && tree.all_children(statement).is_empty() {
        copy_annotations_from(tree, b, statement);
    } else {
        tree.insert_child(b, block_statement::slot::STATEMENTS, 0, statement);
    }
}

fn is_allowed_as_embedded_statement(
    tree: &SyntaxTree,
    statement: SyntaxId,
    parent: Option<SyntaxId>,
) -> bool {
    let parent_kind = parent.map(|p| tree.kind(p));
    match tree.kind(statement) {
        kind::IF_ELSE_STATEMENT => {
            parent_kind == Some(kind::IF_ELSE_STATEMENT) && in_false_statement_slot(tree, statement)
        }
        kind::VARIABLE_DECLARATION_STATEMENT
        | kind::WHILE_STATEMENT
        | kind::DO_WHILE_STATEMENT
        | kind::SWITCH_STATEMENT
        | kind::FOREACH_STATEMENT
        | kind::FOR_STATEMENT
        | kind::LOCK_STATEMENT
        | kind::FIXED_STATEMENT => false,
        kind::USING_STATEMENT => {
            parent_kind == Some(kind::USING_STATEMENT)
                && tree.node(statement).scalars[using_statement::scalar::IS_ENHANCED].as_bool()
                    != Some(true)
        }
        _ => !parent
            .and_then(|p| tree.node(p).parent)
            .is_some_and(|gp| tree.kind(gp) == kind::IF_ELSE_STATEMENT),
    }
}

/// `SimplifyPropertyDeclaration` / `SimplifyIndexerDeclaration`: the declaration `decl` without
/// setter, initializer or expression body whose getter (in slot `getter_slot`, without attributes
/// and with no modifier but `readonly`) is `{ return expression; }` gets `expression` as its
/// expression body (slot `body_slot`), the getter's modifiers and annotations, and loses the
/// getter.
fn simplify(
    tree: &mut SyntaxTree,
    decl: SyntaxId,
    getter_slot: usize,
    body_slot: usize,
    modifiers: usize,
) {
    let Some(expression) = calculated_getter_expression(tree, decl, getter_slot, body_slot) else {
        return;
    };
    let Some(getter) = tree.child(decl, getter_slot) else {
        return;
    };
    let getter_modifiers = tree.node(getter).scalars[accessor::scalar::MODIFIERS]
        .as_int()
        .unwrap_or(0);
    if getter_modifiers & !MOVABLE_MODIFIERS != 0 {
        return;
    }
    if let Scalar::Int(m) = &mut tree.node_mut(decl).scalars[modifiers] {
        *m |= getter_modifiers;
    }
    tree.set_child(decl, body_slot, Some(expression));
    copy_annotations_from(tree, decl, getter);
    tree.detach(getter);
}

/// The pattern `CalculatedGetterOnlyPropertyPattern` / `CalculatedGetterOnlyIndexerPattern`:
/// the returned expression where `decl` matches. Name, attributes, modifiers, private
/// implementation type, return type and parameters match anything.
fn calculated_getter_expression(
    tree: &SyntaxTree,
    decl: SyntaxId,
    getter_slot: usize,
    body_slot: usize,
) -> Option<SyntaxId> {
    let (setter_slot, initializer_slot) = if tree.kind(decl) == kind::PROPERTY_DECLARATION {
        (
            property_declaration::slot::SETTER,
            Some(property_declaration::slot::INITIALIZER),
        )
    } else {
        (indexer_declaration::slot::SETTER, None)
    };
    if tree.child(decl, setter_slot).is_some()
        || tree.child(decl, body_slot).is_some()
        || initializer_slot.is_some_and(|s| tree.child(decl, s).is_some())
    {
        return None;
    }
    let getter = tree.child(decl, getter_slot)?;
    if !tree.children(getter, accessor::slot::ATTRIBUTES).is_empty() {
        return None;
    }
    let body = tree.child(getter, accessor::slot::BODY)?;
    let [ret] = *tree.children(body, block_statement::slot::STATEMENTS) else {
        return None;
    };
    if tree.kind(ret) != kind::RETURN_STATEMENT {
        return None;
    }
    tree.child(ret, return_statement::slot::EXPRESSION)
}

#[cfg(test)]
mod tests {
    use super::*;

    fn block_with(tree: &mut SyntaxTree, statements: &[SyntaxId]) -> SyntaxId {
        let b = tree.new_node(kind::BLOCK_STATEMENT);
        for (i, &s) in statements.iter().enumerate() {
            tree.insert_child(b, block_statement::slot::STATEMENTS, i, s);
        }
        b
    }

    fn normalize(always_use_braces: bool) -> Normalize {
        Normalize {
            always_use_braces,
            file_scoped_namespaces: true,
            expression_bodies: true,
            has_namespace: false,
            single_namespace: None,
        }
    }

    #[test]
    fn a_single_statement_block_of_an_if_is_unwrapped_and_a_loop_body_is_wrapped() {
        let mut tree = SyntaxTree::default();
        let ret = tree.new_node(kind::RETURN_STATEMENT);
        let block = block_with(&mut tree, &[ret]);
        let if_else = tree.new_node(kind::IF_ELSE_STATEMENT);
        tree.set_child(
            if_else,
            if_else_statement::slot::TRUE_STATEMENT,
            Some(block),
        );
        let empty = tree.new_node(kind::EMPTY_STATEMENT);
        tree.node_mut(empty).annotations = vec![4];
        let while_loop = tree.new_node(kind::WHILE_STATEMENT);
        tree.set_child(
            while_loop,
            while_statement::slot::EMBEDDED_STATEMENT,
            Some(empty),
        );
        let root = block_with(&mut tree, &[if_else, while_loop]);
        tree.root = Some(root);
        normalize(false).visit(&mut tree, root);
        assert_eq!(
            tree.child(if_else, if_else_statement::slot::TRUE_STATEMENT),
            Some(ret)
        );
        let body = tree
            .child(while_loop, while_statement::slot::EMBEDDED_STATEMENT)
            .unwrap();
        assert_eq!(tree.kind(body), kind::BLOCK_STATEMENT);
        assert!(
            tree.children(body, block_statement::slot::STATEMENTS)
                .is_empty()
        );
        assert_eq!(tree.node(body).annotations, vec![4]);
    }

    #[test]
    fn an_else_if_keeps_its_shape_with_always_use_braces() {
        let mut tree = SyntaxTree::default();
        let inner = tree.new_node(kind::IF_ELSE_STATEMENT);
        let ret = tree.new_node(kind::RETURN_STATEMENT);
        tree.set_child(inner, if_else_statement::slot::TRUE_STATEMENT, Some(ret));
        let outer = tree.new_node(kind::IF_ELSE_STATEMENT);
        let ret2 = tree.new_node(kind::RETURN_STATEMENT);
        tree.set_child(outer, if_else_statement::slot::TRUE_STATEMENT, Some(ret2));
        tree.set_child(outer, if_else_statement::slot::FALSE_STATEMENT, Some(inner));
        tree.root = Some(outer);
        normalize(true).visit(&mut tree, outer);
        assert_eq!(
            tree.child(outer, if_else_statement::slot::FALSE_STATEMENT),
            Some(inner)
        );
        let t = tree
            .child(outer, if_else_statement::slot::TRUE_STATEMENT)
            .unwrap();
        assert_eq!(tree.kind(t), kind::BLOCK_STATEMENT);
        let t = tree
            .child(inner, if_else_statement::slot::TRUE_STATEMENT)
            .unwrap();
        assert_eq!(tree.kind(t), kind::BLOCK_STATEMENT);
    }

    #[test]
    fn a_calculated_getter_becomes_an_expression_body() {
        let mut tree = SyntaxTree::default();
        let value = tree.new_node(kind::PRIMITIVE_EXPRESSION);
        let ret = tree.new_node(kind::RETURN_STATEMENT);
        tree.set_child(ret, return_statement::slot::EXPRESSION, Some(value));
        let body = block_with(&mut tree, &[ret]);
        let getter = tree.new_node(kind::ACCESSOR);
        tree.node_mut(getter).scalars[accessor::scalar::MODIFIERS] = Scalar::Int(MOVABLE_MODIFIERS);
        tree.node_mut(getter).annotations = vec![7];
        tree.set_child(getter, accessor::slot::BODY, Some(body));
        let property = tree.new_node(kind::PROPERTY_DECLARATION);
        tree.set_child(property, property_declaration::slot::GETTER, Some(getter));
        tree.root = Some(property);
        normalize(false).visit(&mut tree, property);
        assert_eq!(
            tree.child(property, property_declaration::slot::GETTER),
            None
        );
        assert_eq!(
            tree.child(property, property_declaration::slot::EXPRESSION_BODY),
            Some(value)
        );
        assert_eq!(
            tree.node(property).scalars[property_declaration::scalar::MODIFIERS],
            Scalar::Int(MOVABLE_MODIFIERS)
        );
        assert_eq!(tree.node(property).annotations, vec![7]);
    }
}
