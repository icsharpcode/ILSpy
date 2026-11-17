//! `AddCheckedBlocks`: places checked/unchecked blocks and expressions so that every expression
//! the expression builder annotated as checked or unchecked ends up in its context.
//!
//! Placing them is an optimization over statements and expressions, with these goals (the first
//! has the highest priority):
//! 1. use the minimum number of checked blocks plus expressions,
//! 2. prefer checked expressions over checked blocks,
//! 3. keep the scope of checked expressions as small as possible,
//! 4. open checked blocks as late as possible (so that no variable declaration moves into a
//!    block while the variable is still used after it), and close them as late as possible.
//!
//! The annotations are host objects; the oracle tells which of the three
//! (`QUERY_CHECKED_STATE`) an annotation handle is, and the request's
//! `CheckForOverflowUnderflow` setting decides the context of the whole tree.

use std::cmp::Ordering;
use std::rc::Rc;

use crate::csharp::syntax::{SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::{SyntaxOracle, check_for_overflow_underflow, checked_state};
use crate::csharp::syntax_schema::{
    block_statement, checked_expression, checked_statement, kind, unchecked_expression,
    unchecked_statement,
};
use crate::typesystem::types::TypeSystem;

/// The cost of a placement: blocks plus expressions first, then fewer blocks.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
struct Cost {
    blocks: i32,
    expressions: i32,
}

impl Cost {
    const ZERO: Cost = Cost::new(0, 0);
    /// The highest cost for which the addition of blocks and expressions does not overflow.
    const INFINITE: Cost = Cost::new(0x3fff_ffff, 0x3fff_ffff);

    const fn new(blocks: i32, expressions: i32) -> Cost {
        Cost {
            blocks,
            expressions,
        }
    }

    fn key(self) -> (i32, i32) {
        (self.blocks + self.expressions, self.blocks)
    }

    /// The cost when an expression of this cost is wrapped in a checked/unchecked expression;
    /// nested layers of such expressions are penalized, which reads better for less nesting.
    fn wrap_in_checked_expr(self) -> Cost {
        if self.expressions == 0 {
            Cost::new(self.blocks, 1)
        } else {
            Cost::new(self.blocks, self.expressions + 2)
        }
    }
}

impl std::ops::Add for Cost {
    type Output = Cost;

    fn add(self, b: Cost) -> Cost {
        Cost::new(self.blocks + b.blocks, self.expressions + b.expressions)
    }
}

impl std::ops::AddAssign for Cost {
    fn add_assign(&mut self, b: Cost) {
        *self = *self + b;
    }
}

impl PartialOrd for Cost {
    fn partial_cmp(&self, other: &Cost) -> Option<Ordering> {
        Some(self.cmp(other))
    }
}

impl Ord for Cost {
    fn cmp(&self, other: &Cost) -> Ordering {
        self.key().cmp(&other.key())
    }
}

/// The blocks and expressions to insert, in insertion order.
#[derive(Debug)]
enum Inserted {
    List(Rc<Inserted>, Rc<Inserted>),
    Expression {
        expression: SyntaxId,
        checked: bool,
    },
    /// The statements from `first` (inclusive) to `last` (exclusive, `None` for the end of the
    /// block).
    Block {
        first: Option<SyntaxId>,
        last: Option<SyntaxId>,
        checked: bool,
    },
}

type Nodes = Option<Rc<Inserted>>;

fn concat(a: &Nodes, b: &Nodes) -> Nodes {
    match (a, b) {
        (None, _) => b.clone(),
        (_, None) => a.clone(),
        (Some(a), Some(b)) => Some(Rc::new(Inserted::List(a.clone(), b.clone()))),
    }
}

fn one(node: Inserted) -> Nodes {
    Some(Rc::new(node))
}

/// The best placements of a node in a checked and in an unchecked context.
#[derive(Clone)]
struct Outcome {
    cost_checked: Cost,
    nodes_checked: Nodes,
    cost_unchecked: Cost,
    nodes_unchecked: Nodes,
}

impl Default for Outcome {
    fn default() -> Outcome {
        Outcome {
            cost_checked: Cost::ZERO,
            nodes_checked: None,
            cost_unchecked: Cost::ZERO,
            nodes_unchecked: None,
        }
    }
}

/// The checked state an annotation asks for.
#[derive(Clone, Copy, PartialEq, Eq, Debug)]
enum Annotation {
    Checked,
    Unchecked,
    ExplicitUnchecked,
}

/// `AddCheckedBlocks.Run` on the whole tree.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let checked = check_for_overflow_underflow(oracle)
        .ok_or("AddCheckedBlocks needs the request's CheckForOverflowUnderflow setting")?;
    run_node(tree, oracle, root, checked)
}

fn run_node(
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
    node: SyntaxId,
    checked: bool,
) -> Result<(), String> {
    if tree.kind(node) != kind::BLOCK_STATEMENT {
        for child in tree.all_children(node) {
            run_node(tree, oracle, child, checked)?;
        }
        return Ok(());
    }
    let r = Search { tree, oracle }.block(node)?;
    let nodes = if checked {
        r.nodes_checked
    } else {
        r.nodes_unchecked
    };
    insert(tree, nodes)
}

struct Search<'a> {
    tree: &'a SyntaxTree,
    oracle: &'a dyn SyntaxOracle,
}

impl Search<'_> {
    /// `GetResultFromBlock`: tracks a checked and an unchecked context, each with and without an
    /// open block of the other kind.
    fn block(&self, block: SyntaxId) -> Result<Outcome, String> {
        // a) context is checked, no unchecked block open
        let mut cost_c = Cost::ZERO;
        let mut nodes_c: Nodes = None;
        // b) context is checked, an unchecked block is open
        let mut cost_c_open = Cost::INFINITE;
        let mut nodes_c_open: Nodes = None;
        let mut unchecked_start: Option<SyntaxId> = None;
        // c) context is unchecked, no checked block open
        let mut cost_u = Cost::ZERO;
        let mut nodes_u: Nodes = None;
        // d) context is unchecked, a checked block is open
        let mut cost_u_open = Cost::INFINITE;
        let mut nodes_u_open: Nodes = None;
        let mut checked_start: Option<SyntaxId> = None;

        let statements = self
            .tree
            .children(block, block_statement::slot::STATEMENTS)
            .to_vec();
        let mut index = 0;
        loop {
            let statement = statements.get(index).copied();
            // Closing a block is free; '<=' closes it as late as possible.
            if cost_c_open <= cost_c {
                cost_c = cost_c_open;
                nodes_c = concat(
                    &nodes_c_open,
                    &one(Inserted::Block {
                        first: unchecked_start,
                        last: statement,
                        checked: false,
                    }),
                );
            }
            if cost_u_open <= cost_u {
                cost_u = cost_u_open;
                nodes_u = concat(
                    &nodes_u_open,
                    &one(Inserted::Block {
                        first: checked_start,
                        last: statement,
                        checked: true,
                    }),
                );
            }
            let Some(statement) = statement else {
                break;
            };
            // Opening a block; '<=' opens it as late as possible.
            if cost_c + Cost::new(1, 0) <= cost_c_open {
                cost_c_open = cost_c + Cost::new(1, 0);
                nodes_c_open = nodes_c.clone();
                unchecked_start = Some(statement);
            }
            if cost_u + Cost::new(1, 0) <= cost_u_open {
                cost_u_open = cost_u + Cost::new(1, 0);
                nodes_u_open = nodes_u.clone();
                checked_start = Some(statement);
            }
            let s = self.node(statement)?;
            cost_c += s.cost_checked;
            nodes_c = concat(&nodes_c, &s.nodes_checked);
            cost_c_open += s.cost_unchecked;
            nodes_c_open = concat(&nodes_c_open, &s.nodes_unchecked);
            cost_u += s.cost_unchecked;
            nodes_u = concat(&nodes_u, &s.nodes_unchecked);
            cost_u_open += s.cost_checked;
            nodes_u_open = concat(&nodes_u_open, &s.nodes_checked);

            let k = self.tree.kind(statement);
            if k == kind::LABEL_STATEMENT || k == kind::LOCAL_FUNCTION_DECLARATION_STATEMENT {
                // A label moved into a block may become unreachable for its gotos, and a local
                // function moved into a block may go out of scope of its call sites.
                cost_c_open = Cost::INFINITE;
                cost_u_open = Cost::INFINITE;
            }
            index += 1;
        }
        Ok(Outcome {
            cost_checked: cost_c,
            nodes_checked: nodes_c,
            cost_unchecked: cost_u,
            nodes_unchecked: nodes_u,
        })
    }

    /// `GetResult` of any node.
    fn node(&self, node: SyntaxId) -> Result<Outcome, String> {
        let tree = self.tree;
        if tree.kind(node) == kind::BLOCK_STATEMENT {
            return self.block(node);
        }
        let mut r = Outcome::default();
        for child in tree.all_children(node) {
            let c = self.node(child)?;
            r.cost_checked += c.cost_checked;
            r.nodes_checked = concat(&r.nodes_checked, &c.nodes_checked);
            r.cost_unchecked += c.cost_unchecked;
            r.nodes_unchecked = concat(&r.nodes_unchecked, &c.nodes_unchecked);
        }
        if !is_expression(tree, node) {
            return Ok(r);
        }
        match self.annotation(node)? {
            Some(Annotation::ExplicitUnchecked) => {
                // An explicit unchecked() expression is always forced, in either context.
                r.cost_checked = r.cost_unchecked.wrap_in_checked_expr();
                r.cost_unchecked = r.cost_unchecked.wrap_in_checked_expr();
                r.nodes_unchecked = concat(
                    &r.nodes_unchecked,
                    &one(Inserted::Expression {
                        expression: node,
                        checked: false,
                    }),
                );
                r.nodes_checked = r.nodes_unchecked.clone();
            }
            // A huge cost in the other context still leaves a solution where none satisfies all
            // annotations (e.g. "for (checked(M().x += 1); true; unchecked(M().x += 2)) {}").
            Some(Annotation::Checked) => r.cost_unchecked += Cost::new(10000, 0),
            Some(Annotation::Unchecked) => r.cost_checked += Cost::new(10000, 0),
            None => {}
        }
        let parent = tree.node(node).parent;
        if parent.is_some_and(|p| tree.kind(p) == kind::EXPRESSION_STATEMENT) {
            // A top-level expression cannot be wrapped in checked/unchecked.
        } else if in_expression_slot(tree, node) {
            // '<' introduces expressions on the deepest level possible.
            let wrap_checked = r.cost_checked.wrap_in_checked_expr();
            let wrap_unchecked = r.cost_unchecked.wrap_in_checked_expr();
            if wrap_checked < r.cost_unchecked {
                r.cost_unchecked = wrap_checked;
                r.nodes_unchecked = concat(
                    &r.nodes_checked,
                    &one(Inserted::Expression {
                        expression: node,
                        checked: true,
                    }),
                );
            } else if wrap_unchecked < r.cost_checked {
                r.cost_checked = wrap_unchecked;
                r.nodes_checked = concat(
                    &r.nodes_unchecked,
                    &one(Inserted::Expression {
                        expression: node,
                        checked: false,
                    }),
                );
            }
        }
        Ok(r)
    }

    /// The first checked/unchecked annotation of `node`.
    fn annotation(&self, node: SyntaxId) -> Result<Option<Annotation>, String> {
        for &handle in &self.tree.node(node).annotations {
            match checked_state(self.oracle, handle) {
                None => {}
                Some(0) => return Ok(Some(Annotation::Checked)),
                Some(1) => return Ok(Some(Annotation::Unchecked)),
                Some(2) => return Ok(Some(Annotation::ExplicitUnchecked)),
                Some(other) => return Err(format!("unknown checked state {other}")),
            }
        }
        Ok(None)
    }
}

/// Whether `node` is an `Expression`: the concrete expression classes are exactly the kinds
/// whose name ends in "Expression" (`ExpressionStatement` and `SwitchExpressionSection` do not).
pub(crate) fn is_expression(tree: &SyntaxTree, node: SyntaxId) -> bool {
    tree.node(node).schema().name.ends_with("Expression")
}

/// Whether the slot holding `node` takes any expression (`Slot.ChildType` is assignable from
/// `Expression`), so that a checked/unchecked expression may replace it.
fn in_expression_slot(tree: &SyntaxTree, node: SyntaxId) -> bool {
    let (Some(parent), Some(slot)) = (tree.node(node).parent, tree.slot_of(node)) else {
        return false;
    };
    matches!(
        tree.node(parent).schema().slots[slot].child,
        "Expression" | "AstNode"
    )
}

/// `InsertedNode.Insert`, in order.
fn insert(tree: &mut SyntaxTree, nodes: Nodes) -> Result<(), String> {
    let mut stack: Vec<Rc<Inserted>> = nodes.into_iter().collect();
    while let Some(n) = stack.pop() {
        match &*n {
            Inserted::List(a, b) => {
                stack.push(b.clone());
                stack.push(a.clone());
            }
            &Inserted::Expression {
                expression,
                checked,
            } => {
                let (k, slot) = if checked {
                    (
                        kind::CHECKED_EXPRESSION,
                        checked_expression::slot::EXPRESSION,
                    )
                } else {
                    (
                        kind::UNCHECKED_EXPRESSION,
                        unchecked_expression::slot::EXPRESSION,
                    )
                };
                let wrapper = tree.new_node(k);
                tree.replace(expression, wrapper);
                tree.set_child(wrapper, slot, Some(expression));
            }
            &Inserted::Block {
                first,
                last,
                checked,
            } => {
                // A block without start has infinite cost and is never selected.
                let first = first.ok_or("a checked block without start")?;
                let parent = tree
                    .node(first)
                    .parent
                    .ok_or("a checked block start without parent")?;
                let siblings = tree.children(parent, block_statement::slot::STATEMENTS);
                let start = siblings
                    .iter()
                    .position(|&s| s == first)
                    .ok_or("a checked block start outside its block")?;
                let moved: Vec<SyntaxId> = siblings[start + 1..]
                    .iter()
                    .copied()
                    .take_while(|&s| Some(s) != last)
                    .collect();
                let body = tree.new_node(kind::BLOCK_STATEMENT);
                for (i, s) in moved.into_iter().enumerate() {
                    tree.insert_child(body, block_statement::slot::STATEMENTS, i, s);
                }
                let (k, slot) = if checked {
                    (kind::CHECKED_STATEMENT, checked_statement::slot::BODY)
                } else {
                    (kind::UNCHECKED_STATEMENT, unchecked_statement::slot::BODY)
                };
                let block = tree.new_node(k);
                tree.set_child(block, slot, Some(body));
                tree.replace(first, block);
                tree.insert_child(body, block_statement::slot::STATEMENTS, 0, first);
            }
        }
    }
    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_oracle::{QUERY_CHECK_FOR_OVERFLOW_UNDERFLOW, QUERY_CHECKED_STATE};
    use crate::csharp::syntax_schema::{
        assignment_expression, binary_operator_expression, expression_statement,
    };

    /// Handle 1 is the checked annotation; the context is unchecked.
    struct Oracle;

    impl SyntaxOracle for Oracle {
        fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
            match (query[0], query[1]) {
                (QUERY_CHECKED_STATE, 1) => Some(vec![0]),
                (QUERY_CHECK_FOR_OVERFLOW_UNDERFLOW, _) => Some(vec![0]),
                _ => None,
            }
        }
    }

    /// `x = a + b;` with `a + b` annotated checked.
    fn statement(tree: &mut SyntaxTree) -> (SyntaxId, SyntaxId) {
        let binary = tree.new_node(kind::BINARY_OPERATOR_EXPRESSION);
        for slot in [
            binary_operator_expression::slot::LEFT,
            binary_operator_expression::slot::RIGHT,
        ] {
            let id = tree.new_node(kind::IDENTIFIER_EXPRESSION);
            tree.set_child(binary, slot, Some(id));
        }
        tree.node_mut(binary).annotations = vec![1];
        let assign = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        let x = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        tree.set_child(assign, assignment_expression::slot::LEFT, Some(x));
        tree.set_child(assign, assignment_expression::slot::RIGHT, Some(binary));
        let stmt = tree.new_node(kind::EXPRESSION_STATEMENT);
        tree.set_child(stmt, expression_statement::slot::EXPRESSION, Some(assign));
        (stmt, binary)
    }

    fn block_with(n: usize) -> (SyntaxTree, SyntaxId, Vec<(SyntaxId, SyntaxId)>) {
        let mut tree = SyntaxTree::default();
        let block = tree.new_node(kind::BLOCK_STATEMENT);
        let mut stmts = Vec::new();
        for i in 0..n {
            let s = statement(&mut tree);
            tree.insert_child(block, block_statement::slot::STATEMENTS, i, s.0);
            stmts.push(s);
        }
        tree.root = Some(block);
        (tree, block, stmts)
    }

    fn transform(tree: &mut SyntaxTree) {
        let root = tree.root.unwrap();
        run_node(tree, &Oracle, root, false).unwrap();
    }

    #[test]
    fn cost_order_prefers_fewer_blocks_on_ties() {
        assert!(Cost::new(0, 1) < Cost::new(1, 0));
        assert!(Cost::new(1, 0) < Cost::new(0, 2));
        assert!(Cost::new(1, 0) <= Cost::new(1, 0));
        assert_eq!(Cost::new(0, 0).wrap_in_checked_expr(), Cost::new(0, 1));
        assert_eq!(Cost::new(2, 1).wrap_in_checked_expr(), Cost::new(2, 3));
    }

    #[test]
    fn a_single_checked_operation_gets_a_checked_expression() {
        let (mut tree, block, stmts) = block_with(1);
        transform(&mut tree);
        let (stmt, binary) = stmts[0];
        assert_eq!(
            tree.children(block, block_statement::slot::STATEMENTS),
            &[stmt]
        );
        let wrapper = tree.node(binary).parent.unwrap();
        assert_eq!(tree.kind(wrapper), kind::CHECKED_EXPRESSION);
        assert_eq!(
            tree.slot_of(wrapper),
            Some(assignment_expression::slot::RIGHT)
        );
    }

    #[test]
    fn two_checked_statements_get_a_checked_block() {
        let (mut tree, block, stmts) = block_with(2);
        transform(&mut tree);
        let outer = tree.children(block, block_statement::slot::STATEMENTS);
        assert_eq!(outer.len(), 1);
        assert_eq!(tree.kind(outer[0]), kind::CHECKED_STATEMENT);
        let body = tree.child(outer[0], checked_statement::slot::BODY).unwrap();
        assert_eq!(
            tree.children(body, block_statement::slot::STATEMENTS),
            &[stmts[0].0, stmts[1].0]
        );
        assert_eq!(
            tree.kind(tree.node(stmts[0].1).parent.unwrap()),
            kind::ASSIGNMENT_EXPRESSION
        );
    }
}
