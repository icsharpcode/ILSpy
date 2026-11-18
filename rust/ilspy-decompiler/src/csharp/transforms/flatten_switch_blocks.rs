//! `FlattenSwitchBlocks`: a switch section whose only non-empty statement is a block that
//! declares no local (directly, outside nested blocks) gets the block's statements instead.
//!
//! The managed transform walks `Descendants` while it changes the tree; that walk reads a
//! node's children only after the node is handled, so the moved statements are visited too.

use crate::csharp::syntax::{SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::SyntaxOracle;
use crate::csharp::syntax_schema::{block_statement, kind, switch_section};
use crate::typesystem::types::TypeSystem;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    _oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    if let Some(root) = tree.root {
        for child in tree.all_children(root) {
            visit(tree, child);
        }
    }
    Ok(())
}

fn visit(tree: &mut SyntaxTree, id: SyntaxId) {
    if tree.kind(id) == kind::SWITCH_SECTION {
        flatten(tree, id);
    }
    for child in tree.all_children(id) {
        visit(tree, child);
    }
}

fn flatten(tree: &mut SyntaxTree, section: SyntaxId) {
    let mut non_empty = tree
        .children(section, switch_section::slot::STATEMENTS)
        .iter()
        .copied()
        .filter(|&s| tree.kind(s) != kind::EMPTY_STATEMENT);
    let (Some(block), None) = (non_empty.next(), non_empty.next()) else {
        return;
    };
    if tree.kind(block) != kind::BLOCK_STATEMENT {
        return;
    }
    let statements = tree
        .children(block, block_statement::slot::STATEMENTS)
        .to_vec();
    if statements
        .iter()
        .any(|&s| contains_local_declaration(tree, s))
    {
        return;
    }
    tree.detach(block);
    for s in statements {
        let end = tree
            .children(section, switch_section::slot::STATEMENTS)
            .len();
        tree.insert_child(section, switch_section::slot::STATEMENTS, end, s);
    }
}

fn contains_local_declaration(tree: &SyntaxTree, node: SyntaxId) -> bool {
    match tree.kind(node) {
        kind::VARIABLE_DECLARATION_STATEMENT
        | kind::LOCAL_FUNCTION_DECLARATION_STATEMENT
        | kind::OUT_VAR_DECLARATION_EXPRESSION => true,
        kind::BLOCK_STATEMENT => false,
        _ => tree
            .all_children(node)
            .into_iter()
            .any(|c| contains_local_declaration(tree, c)),
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    fn section_with(tree: &mut SyntaxTree, statements: &[SyntaxId]) -> SyntaxId {
        let s = tree.new_node(kind::SWITCH_SECTION);
        for (i, &st) in statements.iter().enumerate() {
            tree.insert_child(s, switch_section::slot::STATEMENTS, i, st);
        }
        s
    }

    fn block_with(tree: &mut SyntaxTree, statements: &[SyntaxId]) -> SyntaxId {
        let b = tree.new_node(kind::BLOCK_STATEMENT);
        for (i, &st) in statements.iter().enumerate() {
            tree.insert_child(b, block_statement::slot::STATEMENTS, i, st);
        }
        b
    }

    #[test]
    fn a_lone_block_is_flattened_after_the_empty_statements() {
        let mut tree = SyntaxTree::default();
        let brk = tree.new_node(kind::BREAK_STATEMENT);
        let block = block_with(&mut tree, &[brk]);
        let empty = tree.new_node(kind::EMPTY_STATEMENT);
        let section = section_with(&mut tree, &[block, empty]);
        flatten(&mut tree, section);
        assert_eq!(
            tree.children(section, switch_section::slot::STATEMENTS),
            &[empty, brk]
        );
        assert_eq!(tree.node(block).parent, None);
    }

    #[test]
    fn a_block_with_a_declaration_stays() {
        let mut tree = SyntaxTree::default();
        let decl = tree.new_node(kind::VARIABLE_DECLARATION_STATEMENT);
        let block = block_with(&mut tree, &[decl]);
        let section = section_with(&mut tree, &[block]);
        flatten(&mut tree, section);
        assert_eq!(
            tree.children(section, switch_section::slot::STATEMENTS),
            &[block]
        );
        // A declaration inside a nested block does not count.
        let inner = block_with(&mut tree, &[]);
        let decl2 = tree.new_node(kind::VARIABLE_DECLARATION_STATEMENT);
        tree.insert_child(inner, block_statement::slot::STATEMENTS, 0, decl2);
        assert!(!contains_local_declaration(&tree, inner));
    }
}
