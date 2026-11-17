//! `PrettifyAssignments`: simplifies `x = x op y` (also `x = (T)(x op y)`) into `x op= y`, and
//! `x += 1` / `x -= 1` into `x++` / `x--` (or `++x` / `--x` where the value is used).
//!
//! A depth-first visit in child order that handles each assignment after its children. The host
//! answers the semantic questions, each query being its kind and one or two lists of annotation
//! handles (a count and the handles, LEB128), the annotations of the nodes whose
//! `GetResolveResult` the managed code reads:
//! - `QUERY_IMPLICITLY_CONVERTIBLE` (126): the right operand's and the cast type's annotations;
//!   one byte, whether the operand converts implicitly to the cast type.
//! - `QUERY_CONSTANT_ONE` (127): the right-hand side's annotations; one byte, whether it is a
//!   compile-time constant 1 of a primitive integer type, `float` or `double`.
//! - `QUERY_CUSTOM_OPERATOR` (128): the assignment's annotations; one byte, whether one is a
//!   `CallInstruction`, `UserDefinedCompoundAssign` or `DynamicCompoundAssign`.

use crate::csharp::syntax::{Scalar, SlotValue, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{self, SyntaxOracle};
use crate::csharp::syntax_schema::{
    assignment_expression, binary_operator_expression, cast_expression, identifier,
    indexer_expression, kind, member_reference_expression, unary_operator_expression,
};
use crate::typesystem::types::TypeSystem;

const QUERY_IMPLICITLY_CONVERTIBLE: u8 = 126;
const QUERY_CONSTANT_ONE: u8 = 127;
const QUERY_CUSTOM_OPERATOR: u8 = 128;

/// `AssignmentOperatorType.Assign`, `.Add` and `.Subtract`.
const ASSIGN: i64 = 0;
const ASSIGN_ADD: i64 = 1;
const ASSIGN_SUBTRACT: i64 = 2;
/// `UnaryOperatorType.Increment`, `.Decrement`, `.PostIncrement`, `.PostDecrement`,
/// `.Dereference`.
const INCREMENT: i64 = 5;
const DECREMENT: i64 = 6;
const POST_INCREMENT: i64 = 7;
const POST_DECREMENT: i64 = 8;
const DEREFERENCE: i64 = 9;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let increments = syntax_oracle::setting(oracle, "IntroduceIncrementAndDecrement")
        .ok_or("the host did not answer the IntroduceIncrementAndDecrement setting")?;
    if let Some(root) = tree.root {
        visit(tree, oracle, increments, root)?;
    }
    Ok(())
}

fn visit(
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
    increments: bool,
    id: SyntaxId,
) -> Result<(), String> {
    // The visit of a child may replace that child, never its siblings.
    for child in tree.all_children(id) {
        visit(tree, oracle, increments, child)?;
    }
    if tree.kind(id) == kind::ASSIGNMENT_EXPRESSION {
        visit_assignment(tree, oracle, increments, id)?;
    }
    Ok(())
}

fn int_scalar(tree: &SyntaxTree, id: SyntaxId, scalar: usize) -> i64 {
    tree.node(id).scalars[scalar].as_int().unwrap_or(0)
}

fn visit_assignment(
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
    increments: bool,
    assignment: SyntaxId,
) -> Result<(), String> {
    let mut rhs = tree.child(assignment, assignment_expression::slot::RIGHT);
    // The cast type, whose resolve result type is the expected type of the operand.
    let mut cast_type = None;
    if let Some(cast) = rhs
        && tree.kind(cast) == kind::CAST_EXPRESSION
    {
        rhs = tree.child(cast, cast_expression::slot::EXPRESSION);
        cast_type = Some(tree.child(cast, cast_expression::slot::TYPE));
    }
    if let Some(binary) = rhs
        && tree.kind(binary) == kind::BINARY_OPERATOR_EXPRESSION
        && int_scalar(tree, assignment, assignment_expression::scalar::OPERATOR) == ASSIGN
    {
        let left = tree.child(assignment, assignment_expression::slot::LEFT);
        let binary_left = tree.child(binary, binary_operator_expression::slot::LEFT);
        let binary_right = tree.child(binary, binary_operator_expression::slot::RIGHT);
        if can_convert_to_compound_assignment(tree, left)
            && is_match(tree, left, binary_left)
            && let Some(right) = binary_right
            && is_implicitly_convertible(tree, oracle, right, cast_type)?
        {
            let op = assignment_operator_for_binary_operator(int_scalar(
                tree,
                binary,
                binary_operator_expression::scalar::OPERATOR,
            ));
            if op != ASSIGN {
                tree.node_mut(assignment).scalars[assignment_expression::scalar::OPERATOR] =
                    Scalar::Int(op);
                copy_annotations_from(tree, assignment, binary);
                tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(right));
            }
        }
    }
    let op = int_scalar(tree, assignment, assignment_expression::scalar::OPERATOR);
    if increments && (op == ASSIGN_ADD || op == ASSIGN_SUBTRACT) {
        let right = tree.child(assignment, assignment_expression::slot::RIGHT);
        let right_annotations = right.map_or(&[][..], |r| &tree.node(r).annotations[..]);
        if ask(oracle, QUERY_CONSTANT_ONE, &[right_annotations])?
            && !ask(
                oracle,
                QUERY_CUSTOM_OPERATOR,
                &[&tree.node(assignment).annotations],
            )?
        {
            // When the parent is an expression statement, pre- or post-increment doesn't
            // matter; the post forms are the more common ones.
            let statement = tree
                .node(assignment)
                .parent
                .is_some_and(|p| tree.kind(p) == kind::EXPRESSION_STATEMENT);
            let unary_op = match (statement, op == ASSIGN_ADD) {
                (true, true) => POST_INCREMENT,
                (true, false) => POST_DECREMENT,
                (false, true) => INCREMENT,
                (false, false) => DECREMENT,
            };
            let unary = tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
            tree.node_mut(unary).scalars[unary_operator_expression::scalar::OPERATOR] =
                Scalar::Int(unary_op);
            let left = tree.child(assignment, assignment_expression::slot::LEFT);
            tree.set_child(unary, unary_operator_expression::slot::EXPRESSION, left);
            copy_annotations_from(tree, unary, assignment);
            tree.replace(assignment, unary);
        }
    }
    Ok(())
}

/// Asks the host the query `kind` about the annotation lists `lists`; its one-byte answer.
fn ask(oracle: &dyn SyntaxOracle, kind: u8, lists: &[&[u32]]) -> Result<bool, String> {
    let mut q = vec![kind];
    for list in lists {
        uvar(&mut q, list.len() as u64);
        for &h in *list {
            uvar(&mut q, u64::from(h));
        }
    }
    let answer = oracle
        .ask(&q)
        .ok_or_else(|| format!("the host did not answer the query {kind}"))?;
    Ok(answer.first() == Some(&1))
}

/// `IsImplicitlyConvertible`: true without a cast, else whether `rhs` converts implicitly to
/// the type of the cast type node `cast_type`'s resolve result.
fn is_implicitly_convertible(
    tree: &SyntaxTree,
    oracle: &dyn SyntaxOracle,
    rhs: SyntaxId,
    cast_type: Option<Option<SyntaxId>>,
) -> Result<bool, String> {
    let Some(cast_type) = cast_type else {
        return Ok(true);
    };
    let type_annotations = cast_type.map_or(&[][..], |t| &tree.node(t).annotations[..]);
    ask(
        oracle,
        QUERY_IMPLICITLY_CONVERTIBLE,
        &[&tree.node(rhs).annotations, type_annotations],
    )
}

/// `GetAssignmentOperatorForBinaryOperator`, on the numeric values of `BinaryOperatorType` and
/// `AssignmentOperatorType`.
fn assignment_operator_for_binary_operator(bop: i64) -> i64 {
    match bop {
        12 => 1, // Add
        13 => 2, // Subtract
        14 => 3, // Multiply
        15 => 4, // Divide
        16 => 5, // Modulus
        17 => 6, // ShiftLeft
        18 => 7, // ShiftRight
        19 => 8, // UnsignedShiftRight
        1 => 9,  // BitwiseAnd
        2 => 10, // BitwiseOr
        5 => 11, // ExclusiveOr
        _ => ASSIGN,
    }
}

fn can_convert_to_compound_assignment(tree: &SyntaxTree, left: Option<SyntaxId>) -> bool {
    let Some(left) = left else {
        return false;
    };
    match tree.kind(left) {
        kind::MEMBER_REFERENCE_EXPRESSION => is_without_side_effects(
            tree,
            tree.child(left, member_reference_expression::slot::TARGET),
        ),
        kind::INDEXER_EXPRESSION => {
            is_without_side_effects(tree, tree.child(left, indexer_expression::slot::TARGET))
                && tree
                    .children(left, indexer_expression::slot::ARGUMENTS)
                    .iter()
                    .all(|&a| is_without_side_effects(tree, Some(a)))
        }
        kind::UNARY_OPERATOR_EXPRESSION
            if int_scalar(tree, left, unary_operator_expression::scalar::OPERATOR)
                == DEREFERENCE =>
        {
            is_without_side_effects(
                tree,
                tree.child(left, unary_operator_expression::slot::EXPRESSION),
            )
        }
        _ => is_without_side_effects(tree, Some(left)),
    }
}

fn is_without_side_effects(tree: &SyntaxTree, node: Option<SyntaxId>) -> bool {
    node.is_some_and(|n| {
        matches!(
            tree.kind(n),
            kind::THIS_REFERENCE_EXPRESSION
                | kind::IDENTIFIER_EXPRESSION
                | kind::TYPE_REFERENCE_EXPRESSION
                | kind::BASE_REFERENCE_EXPRESSION
        )
    })
}

/// `AstNode.IsMatch` of two pattern-free trees, as the generated `DoMatch` compares them: the
/// same kind, equal scalars (text locations and `Identifier.IsVerbatim` ignored) and matching
/// children slot by slot; an absent child matches only an absent one.
fn is_match(tree: &SyntaxTree, a: Option<SyntaxId>, b: Option<SyntaxId>) -> bool {
    let (a, b) = match (a, b) {
        (None, None) => return true,
        (Some(a), Some(b)) => (a, b),
        _ => return false,
    };
    let (na, nb) = (tree.node(a), tree.node(b));
    if na.kind != nb.kind {
        return false;
    }
    for (i, (sa, sb)) in na.scalars.iter().zip(&nb.scalars).enumerate() {
        if na.kind == kind::IDENTIFIER && i == identifier::scalar::IS_VERBATIM {
            continue;
        }
        match (sa, sb) {
            (Scalar::Location(..), Scalar::Location(..)) => {}
            // shortcut: literal values compare by handle, so two equal literals never match;
            // a side-effect-free left-hand side holds no literal, so this is never reached there.
            _ if sa != sb => return false,
            _ => {}
        }
    }
    na.slots
        .iter()
        .zip(&nb.slots)
        .all(|(sa, sb)| match (sa, sb) {
            (SlotValue::Single(ca), SlotValue::Single(cb)) => is_match(tree, *ca, *cb),
            (SlotValue::Many(ca), SlotValue::Many(cb)) => {
                ca.len() == cb.len()
                    && ca
                        .iter()
                        .zip(cb)
                        .all(|(x, y)| is_match(tree, Some(*x), Some(*y)))
            }
            _ => false,
        })
}

/// `CopyAnnotationsFrom`: appends the annotations of `source` to `target`, and copies of its
/// leading and trailing trivia.
pub(super) fn copy_annotations_from(tree: &mut SyntaxTree, target: SyntaxId, source: SyntaxId) {
    let annotations = tree.node(source).annotations.clone();
    tree.node_mut(target).annotations.extend(annotations);
    for leading in [true, false] {
        let list = if leading {
            tree.node(source).leading_trivia.clone()
        } else {
            tree.node(source).trailing_trivia.clone()
        };
        for t in list {
            let c = tree.clone_subtree(t);
            tree.node_mut(c).parent = Some(target);
            if leading {
                tree.node_mut(target).leading_trivia.push(c);
            } else {
                tree.node_mut(target).trailing_trivia.push(c);
            }
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_schema::{expression_statement, identifier_expression};

    fn ident(tree: &mut SyntaxTree, name: &str, verbatim: bool) -> SyntaxId {
        let e = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let t = tree.new_node(kind::IDENTIFIER);
        tree.node_mut(t).scalars[identifier::scalar::NAME] =
            Scalar::Str(name.encode_utf16().collect());
        tree.node_mut(t).scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
        tree.set_child(e, identifier_expression::slot::IDENTIFIER_TOKEN, Some(t));
        e
    }

    #[test]
    fn matching_ignores_verbatim_and_compares_names() {
        let mut tree = SyntaxTree::default();
        let a = ident(&mut tree, "x", false);
        let b = ident(&mut tree, "x", true);
        let c = ident(&mut tree, "y", false);
        assert!(is_match(&tree, Some(a), Some(b)));
        assert!(!is_match(&tree, Some(a), Some(c)));
        assert!(!is_match(&tree, Some(a), None));
        assert!(is_match(&tree, None, None));
    }

    /// Answers every query with `answer`.
    struct Always(bool);

    impl SyntaxOracle for Always {
        fn ask(&self, _query: &[u8]) -> Option<Vec<u8>> {
            Some(vec![u8::from(self.0)])
        }
    }

    #[test]
    fn x_equals_x_plus_one_becomes_a_post_increment() {
        let mut tree = SyntaxTree::default();
        let left = ident(&mut tree, "x", false);
        let x = ident(&mut tree, "x", false);
        let one = tree.new_node(kind::PRIMITIVE_EXPRESSION);
        let add = tree.new_node(kind::BINARY_OPERATOR_EXPRESSION);
        tree.node_mut(add).scalars[binary_operator_expression::scalar::OPERATOR] = Scalar::Int(12);
        tree.node_mut(add).annotations = vec![3];
        tree.set_child(add, binary_operator_expression::slot::LEFT, Some(x));
        tree.set_child(add, binary_operator_expression::slot::RIGHT, Some(one));
        let assignment = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        tree.node_mut(assignment).annotations = vec![2];
        tree.set_child(assignment, assignment_expression::slot::LEFT, Some(left));
        tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(add));
        let statement = tree.new_node(kind::EXPRESSION_STATEMENT);
        tree.set_child(
            statement,
            expression_statement::slot::EXPRESSION,
            Some(assignment),
        );
        tree.root = Some(statement);
        // Every query answers true, so "custom operator" holds: only the compound assignment.
        visit(&mut tree, &Always(true), true, statement).unwrap();
        assert_eq!(
            int_scalar(&tree, assignment, assignment_expression::scalar::OPERATOR),
            ASSIGN_ADD
        );
        assert_eq!(tree.node(assignment).annotations, vec![2, 3]);
        assert_eq!(
            tree.child(assignment, assignment_expression::slot::RIGHT),
            Some(one)
        );

        // With "constant one" true and "custom operator" false, the increment.
        struct OneNoCustom;
        impl SyntaxOracle for OneNoCustom {
            fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
                Some(vec![u8::from(query[0] == QUERY_CONSTANT_ONE)])
            }
        }
        visit_assignment(&mut tree, &OneNoCustom, true, assignment).unwrap();
        let unary = tree
            .child(statement, expression_statement::slot::EXPRESSION)
            .unwrap();
        assert_eq!(tree.kind(unary), kind::UNARY_OPERATOR_EXPRESSION);
        assert_eq!(
            int_scalar(&tree, unary, unary_operator_expression::scalar::OPERATOR),
            POST_INCREMENT
        );
        assert_eq!(
            tree.child(unary, unary_operator_expression::slot::EXPRESSION),
            Some(left)
        );
        assert_eq!(tree.node(unary).annotations, vec![2, 3]);
    }
}
