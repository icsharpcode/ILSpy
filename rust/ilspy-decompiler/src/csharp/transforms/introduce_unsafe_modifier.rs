//! `IntroduceUnsafeModifier`: adds the `unsafe` modifier to every entity declaration (other
//! than an accessor) whose subtree uses pointers, and on the way rewrites `*(ptr + i)` to
//! `ptr[i]` and `(*ptr).Member` to `ptr->Member`.
//!
//! A depth-first visit in child order; each node answers whether its subtree is unsafe, and a
//! declaration that gets the modifier answers false to its parent.

use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::{
    self, SyntaxOracle, UNSAFE_METHOD_GROUP, UNSAFE_PARAMETER, UNSAFE_PARAMETERIZED_MEMBER,
    UNSAFE_POINTER_OPERAND, UNSAFE_RETURN_TYPE, UNSAFE_TYPE,
};
use crate::csharp::syntax_schema::{
    binary_operator_expression, composed_type, identifier, indexer_expression, kind,
    member_reference_expression, pointer_reference_expression, unary_operator_expression,
};
use crate::typesystem::types::TypeSystem;

/// `Modifiers.Unsafe`.
const MODIFIERS_UNSAFE: i64 = 0x8000;
/// `UnaryOperatorType.Dereference`.
const UNARY_DEREFERENCE: i64 = 9;
/// `UnaryOperatorType.AddressOf`.
const UNARY_ADDRESS_OF: i64 = 10;
/// `BinaryOperatorType.Add`.
const BINARY_ADD: i64 = 12;
/// The index of the `Modifiers` scalar of every entity declaration.
const MODIFIERS: usize = 0;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    if let Some(root) = tree.root {
        visit(tree, oracle, root);
    }
    Ok(())
}

/// `IntroduceUnsafeModifier.IsUnsafe`: whether `node` needs an unsafe context, with the same
/// rewrites of its subtree as the transform.
pub fn is_unsafe(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, node: SyntaxId) -> bool {
    visit(tree, oracle, node)
}

fn is_entity_declaration(k: u16) -> bool {
    matches!(
        k,
        kind::CONSTRUCTOR_DECLARATION
            | kind::CUSTOM_EVENT_DECLARATION
            | kind::DELEGATE_DECLARATION
            | kind::DESTRUCTOR_DECLARATION
            | kind::ENUM_MEMBER_DECLARATION
            | kind::EVENT_DECLARATION
            | kind::EXTENSION_DECLARATION
            | kind::FIELD_DECLARATION
            | kind::FIXED_FIELD_DECLARATION
            | kind::INDEXER_DECLARATION
            | kind::METHOD_DECLARATION
            | kind::OPERATOR_DECLARATION
            | kind::PROPERTY_DECLARATION
            | kind::TYPE_DECLARATION
    )
}

fn int_scalar(tree: &SyntaxTree, id: SyntaxId, scalar: usize) -> i64 {
    tree.node(id).scalars[scalar].as_int().unwrap_or(0)
}

fn visit_children(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> bool {
    let mut result = false;
    // The visit of a child may replace that child, never its siblings.
    for child in tree.all_children(id) {
        result |= visit(tree, oracle, child);
    }
    let k = tree.kind(id);
    if result && is_entity_declaration(k) {
        if let Scalar::Int(m) = &mut tree.node_mut(id).scalars[MODIFIERS] {
            *m |= MODIFIERS_UNSAFE;
        }
        return false;
    }
    result
}

fn visit(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> bool {
    match tree.kind(id) {
        kind::POINTER_REFERENCE_EXPRESSION
        | kind::SIZE_OF_EXPRESSION
        | kind::FIXED_VARIABLE_INITIALIZER => {
            visit_children(tree, oracle, id);
            true
        }
        kind::COMPOSED_TYPE => {
            int_scalar(tree, id, composed_type::scalar::POINTER_RANK) > 0
                || visit_children(tree, oracle, id)
        }
        kind::FUNCTION_POINTER_AST_TYPE => true,
        kind::UNARY_OPERATOR_EXPRESSION => visit_unary(tree, oracle, id),
        kind::MEMBER_REFERENCE_EXPRESSION => visit_member_reference(tree, oracle, id),
        kind::IDENTIFIER_EXPRESSION
        | kind::STACK_ALLOC_EXPRESSION
        | kind::INVOCATION_EXPRESSION
        | kind::OBJECT_CREATE_EXPRESSION => {
            let result = visit_children(tree, oracle, id);
            has_unsafe_resolve_result(tree, oracle, id) || result
        }
        _ => visit_children(tree, oracle, id),
    }
}

fn visit_unary(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, uoe: SyntaxId) -> bool {
    let result = visit_children(tree, oracle, uoe);
    match int_scalar(tree, uoe, unary_operator_expression::scalar::OPERATOR) {
        UNARY_DEREFERENCE => {
            if let Some(bop) = tree.child(uoe, unary_operator_expression::slot::EXPRESSION)
                && tree.kind(bop) == kind::BINARY_OPERATOR_EXPRESSION
                && int_scalar(tree, bop, binary_operator_expression::scalar::OPERATOR) == BINARY_ADD
                && resolve_result_facts(tree, oracle, bop)
                    .is_some_and(|f| f & UNSAFE_POINTER_OPERAND != 0)
            {
                // *(ptr + int) becomes ptr[int]
                let indexer = tree.new_node(kind::INDEXER_EXPRESSION);
                let left = tree.child(bop, binary_operator_expression::slot::LEFT);
                tree.set_child(indexer, indexer_expression::slot::TARGET, left);
                if let Some(right) = tree.child(bop, binary_operator_expression::slot::RIGHT) {
                    tree.insert_child(indexer, indexer_expression::slot::ARGUMENTS, 0, right);
                }
                copy_annotations_from(tree, indexer, uoe);
                copy_annotations_from(tree, indexer, bop);
                tree.replace(uoe, indexer);
            }
            true
        }
        UNARY_ADDRESS_OF => true,
        _ => result,
    }
}

fn visit_member_reference(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, mre: SyntaxId) -> bool {
    let result = visit_children(tree, oracle, mre);
    if let Some(uoe) = tree.child(mre, member_reference_expression::slot::TARGET)
        && tree.kind(uoe) == kind::UNARY_OPERATOR_EXPRESSION
        && int_scalar(tree, uoe, unary_operator_expression::scalar::OPERATOR) == UNARY_DEREFERENCE
    {
        // (*ptr).Member becomes ptr->Member
        let pre = tree.new_node(kind::POINTER_REFERENCE_EXPRESSION);
        let target = tree.child(uoe, unary_operator_expression::slot::EXPRESSION);
        tree.set_child(pre, pointer_reference_expression::slot::TARGET, target);
        let name = tree
            .child(mre, member_reference_expression::slot::MEMBER_NAME_TOKEN)
            .and_then(|t| tree.node(t).scalars[identifier::scalar::NAME].as_str())
            .unwrap_or_default();
        let token = new_identifier(tree, &name);
        tree.set_child(
            pre,
            pointer_reference_expression::slot::MEMBER_NAME_TOKEN,
            Some(token),
        );
        let type_arguments = tree
            .children(mre, member_reference_expression::slot::TYPE_ARGUMENTS)
            .to_vec();
        for (i, t) in type_arguments.into_iter().enumerate() {
            tree.insert_child(
                pre,
                pointer_reference_expression::slot::TYPE_ARGUMENTS,
                i,
                t,
            );
        }
        copy_annotations_from(tree, pre, uoe);
        // Only the resolve result of the member reference goes to the new node.
        let annotations = std::mem::take(&mut tree.node_mut(pre).annotations);
        tree.node_mut(pre).annotations = annotations
            .into_iter()
            .filter(|&a| syntax_oracle::unsafe_facts(oracle, a).is_none())
            .collect();
        copy_annotations_from(tree, pre, mre);
        tree.replace(mre, pre);
    }
    has_unsafe_resolve_result(tree, oracle, mre) || result
}

/// `Identifier.Create(name)`.
pub(crate) fn new_identifier(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
    let id = tree.new_node(kind::IDENTIFIER);
    let (name, verbatim) = match name.strip_prefix('@') {
        Some(rest) => (rest, true),
        None => (name, false),
    };
    let node = tree.node_mut(id);
    node.scalars[identifier::scalar::NAME] = Scalar::Str(name.encode_utf16().collect());
    node.scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
    id
}

/// `CopyAnnotationsFrom`: appends the annotations of `source` to `target`, and copies of its
/// leading and trailing trivia.
fn copy_annotations_from(tree: &mut SyntaxTree, target: SyntaxId, source: SyntaxId) {
    let annotations = tree.node(source).annotations.clone();
    tree.node_mut(target).annotations.extend(annotations);
    for leading in [true, false] {
        let trivia = if leading {
            tree.node(source).leading_trivia.clone()
        } else {
            tree.node(source).trailing_trivia.clone()
        };
        for t in trivia {
            let copy = deep_clone(tree, t, target);
            let node = tree.node_mut(target);
            if leading {
                node.leading_trivia.push(copy);
            } else {
                node.trailing_trivia.push(copy);
            }
        }
    }
}

/// A copy of the subtree `id` under `parent` (which the caller links); the copies share the
/// annotation handles.
fn deep_clone(tree: &mut SyntaxTree, id: SyntaxId, parent: SyntaxId) -> SyntaxId {
    use crate::csharp::syntax::SlotValue;
    let mut node = tree.node(id).clone();
    node.parent = Some(parent);
    let copy = SyntaxId(tree.nodes.len() as u32);
    tree.nodes.push(node);
    let mut node = tree.node(copy).clone();
    for t in node
        .leading_trivia
        .iter_mut()
        .chain(node.trailing_trivia.iter_mut())
    {
        *t = deep_clone(tree, *t, copy);
    }
    for slot in &mut node.slots {
        match slot {
            SlotValue::Single(Some(c)) => *c = deep_clone(tree, *c, copy),
            SlotValue::Single(None) => {}
            SlotValue::Many(cs) => {
                for c in cs {
                    *c = deep_clone(tree, *c, copy);
                }
            }
        }
    }
    tree.nodes[copy.0 as usize] = node;
    copy
}

/// The `UNSAFE_*` flags of the node's resolve result (`GetResolveResult`, its first annotation
/// that is a `ResolveResult`), `None` for a node without one.
fn resolve_result_facts(tree: &SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> Option<u8> {
    tree.node(id)
        .annotations
        .iter()
        .find_map(|&a| syntax_oracle::unsafe_facts(oracle, a))
}

fn has_unsafe_resolve_result(tree: &SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> bool {
    resolve_result_facts(tree, oracle, id).is_some_and(is_unsafe_resolve_result)
}

/// `HasUnsafeResolveResult` on the facts about a resolve result.
fn is_unsafe_resolve_result(facts: u8) -> bool {
    if facts & UNSAFE_TYPE != 0 {
        true
    } else if facts & UNSAFE_PARAMETERIZED_MEMBER != 0 {
        facts & UNSAFE_PARAMETER != 0
    } else if facts & UNSAFE_METHOD_GROUP != 0 {
        facts & (UNSAFE_RETURN_TYPE | UNSAFE_PARAMETER) != 0
    } else {
        false
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_schema::{expression_statement, method_declaration};

    /// Answers `QUERY_UNSAFE_FACTS` with `facts[handle]`.
    struct Facts(Vec<Option<u8>>);

    impl SyntaxOracle for Facts {
        fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
            assert_eq!(query[0], syntax_oracle::QUERY_UNSAFE_FACTS);
            self.0
                .get(query[1] as usize)
                .copied()
                .flatten()
                .map(|f| vec![f])
        }
    }

    #[test]
    fn unsafe_resolve_results() {
        assert!(is_unsafe_resolve_result(UNSAFE_TYPE));
        assert!(!is_unsafe_resolve_result(
            UNSAFE_PARAMETERIZED_MEMBER | UNSAFE_RETURN_TYPE
        ));
        assert!(is_unsafe_resolve_result(
            UNSAFE_PARAMETERIZED_MEMBER | UNSAFE_PARAMETER
        ));
        assert!(is_unsafe_resolve_result(
            UNSAFE_METHOD_GROUP | UNSAFE_RETURN_TYPE
        ));
        assert!(!is_unsafe_resolve_result(
            UNSAFE_POINTER_OPERAND | UNSAFE_PARAMETER
        ));
    }

    fn unary(tree: &mut SyntaxTree, op: i64, operand: SyntaxId) -> SyntaxId {
        let u = tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
        tree.node_mut(u).scalars[unary_operator_expression::scalar::OPERATOR] = Scalar::Int(op);
        tree.set_child(
            u,
            unary_operator_expression::slot::EXPRESSION,
            Some(operand),
        );
        u
    }

    #[test]
    fn pointer_addition_becomes_an_indexer_in_an_unsafe_method() {
        let mut tree = SyntaxTree::default();
        let method = tree.new_node(kind::METHOD_DECLARATION);
        let p = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let i = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let add = tree.new_node(kind::BINARY_OPERATOR_EXPRESSION);
        tree.node_mut(add).scalars[binary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(BINARY_ADD);
        tree.node_mut(add).annotations = vec![0, 1];
        tree.set_child(add, binary_operator_expression::slot::LEFT, Some(p));
        tree.set_child(add, binary_operator_expression::slot::RIGHT, Some(i));
        let deref = unary(&mut tree, UNARY_DEREFERENCE, add);
        tree.node_mut(deref).annotations = vec![2];
        let comment = tree.new_node(kind::COMMENT);
        tree.node_mut(deref).leading_trivia = vec![comment];
        tree.node_mut(comment).parent = Some(deref);
        tree.set_child(method, method_declaration::slot::BODY, Some(deref));
        tree.root = Some(method);
        let oracle = Facts(vec![None, Some(UNSAFE_POINTER_OPERAND), Some(0)]);
        assert!(!is_unsafe(&mut tree, &oracle, method));
        assert_eq!(
            int_scalar(&tree, method, method_declaration::scalar::MODIFIERS),
            MODIFIERS_UNSAFE
        );
        let indexer = tree.child(method, method_declaration::slot::BODY).unwrap();
        assert_eq!(tree.kind(indexer), kind::INDEXER_EXPRESSION);
        assert_eq!(
            tree.child(indexer, indexer_expression::slot::TARGET),
            Some(p)
        );
        assert_eq!(
            tree.children(indexer, indexer_expression::slot::ARGUMENTS),
            &[i]
        );
        assert_eq!(tree.node(indexer).annotations, vec![2, 0, 1]);
        let copied = tree.node(indexer).leading_trivia[0];
        assert_ne!(copied, comment);
        assert_eq!(tree.node(copied).parent, Some(indexer));
    }

    #[test]
    fn dereferenced_member_access_becomes_a_pointer_reference() {
        let mut tree = SyntaxTree::default();
        let p = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let deref = unary(&mut tree, UNARY_DEREFERENCE, p);
        tree.node_mut(deref).annotations = vec![0, 1];
        let mre = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
        tree.node_mut(mre).annotations = vec![2];
        tree.set_child(mre, member_reference_expression::slot::TARGET, Some(deref));
        let name = new_identifier(&mut tree, "X");
        tree.set_child(
            mre,
            member_reference_expression::slot::MEMBER_NAME_TOKEN,
            Some(name),
        );
        let statement = tree.new_node(kind::EXPRESSION_STATEMENT);
        tree.set_child(statement, expression_statement::slot::EXPRESSION, Some(mre));
        tree.root = Some(statement);
        let oracle = Facts(vec![Some(0), None, Some(0)]);
        assert!(is_unsafe(&mut tree, &oracle, statement));
        let pre = tree
            .child(statement, expression_statement::slot::EXPRESSION)
            .unwrap();
        assert_eq!(tree.kind(pre), kind::POINTER_REFERENCE_EXPRESSION);
        assert_eq!(
            tree.child(pre, pointer_reference_expression::slot::TARGET),
            Some(p)
        );
        assert_eq!(tree.node(pre).annotations, vec![1, 2]);
        let token = tree
            .child(pre, pointer_reference_expression::slot::MEMBER_NAME_TOKEN)
            .unwrap();
        assert_eq!(
            tree.node(token).scalars[identifier::scalar::NAME].as_str(),
            Some("X".to_owned())
        );
    }
}
