//! `IntroduceQueryExpressions`: turns chains of `Select`, `Where`, `OrderBy`, `GroupBy`,
//! `SelectMany`, `Join` and `GroupJoin` calls with simple lambdas into query expressions, then
//! terminates degenerate queries with a `select` or merges them into the query they feed.
//!
//! The engine learns what it needs about the annotations through `QUERY_QE_ANNOTATION` (154):
//! for a handle, one byte of `QE_*` flags, then for an `ILVariableResolveResult` the object id of
//! its variable (LEB128, from the table `DeclareVariables` uses) and the variable's name as a count
//! and UTF-16 code units. Annotations it creates: `ANNOTATION_QUERY_GROUP_CLAUSE` (154) and
//! `ANNOTATION_QUERY_JOIN_CLAUSE` (155), each with the handles of the two lambdas' `ILFunction`s
//! (LEB128 handle plus one, 0 for none), and `DeclareVariables`' `ILVariableResolveResult` (96).

use std::collections::HashMap;

use super::add_checked_blocks::is_expression;
use super::introduce_unsafe_modifier::copy_annotations_from;
use super::rewrite_visual_basic_with_events::{
    identifier_expression_name, identifier_name, new_identifier,
};
use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{self as q, SyntaxOracle};
use crate::csharp::syntax_schema::{
    assignment_expression, identifier_expression, indexer_expression, invocation_expression, kind,
    lambda_expression, member_reference_expression, parameter_declaration,
    parenthesized_expression, query_expression, query_from_clause, query_group_clause,
    query_join_clause, query_order_clause, query_ordering, query_select_clause, query_where_clause,
    unary_operator_expression,
};
use crate::typesystem::types::TypeSystem;

/// The facts of an annotation, see the module documentation.
pub const QUERY_QE_ANNOTATION: u8 = 154;
/// `new QueryGroupClauseAnnotation(keyLambda, projectionLambda)`.
pub const ANNOTATION_QUERY_GROUP_CLAUSE: u8 = 154;
/// `new QueryJoinClauseAnnotation(onLambda, equalsLambda)`.
pub const ANNOTATION_QUERY_JOIN_CLAUSE: u8 = 155;
/// `DeclareVariables`' `new ILVariableResolveResult(v, v.Type)`, by the variable's object id.
const ANNOTATION_VARIABLE: u8 = 96;

/// The annotation is a `ResolveResult`.
pub const QE_RESOLVE_RESULT: u8 = 1;
/// A `DecompiledLambdaResolveResult` with `AttemptedConversionWithTypeMismatch`.
pub const QE_LAMBDA_TYPE_MISMATCH: u8 = 2;
/// An `ILVariableResolveResult`.
pub const QE_IL_VARIABLE: u8 = 4;
/// A `MemberResolveResult` (or a class derived from it).
pub const QE_MEMBER_RESOLVE_RESULT: u8 = 8;
/// An `ILFunction`.
pub const QE_IL_FUNCTION: u8 = 16;

/// `UnaryOperatorType.NullConditional`.
const UNARY_NULL_CONDITIONAL: i64 = 12;
/// `QueryOrderingDirection.Descending`.
const DIRECTION_DESCENDING: i64 = 2;

/// What `QUERY_QE_ANNOTATION` tells about an annotation.
#[derive(Clone, Debug, Default, PartialEq, Eq)]
pub(crate) struct Facts {
    pub flags: u8,
    /// The object id and the name of the variable of an `ILVariableResolveResult`.
    pub variable: Option<(u32, Vec<u16>)>,
}

/// The facts of the annotations of a tree, asked once per handle.
pub(crate) struct Annotations<'a> {
    oracle: &'a dyn SyntaxOracle,
    cache: HashMap<u32, Facts>,
}

impl<'a> Annotations<'a> {
    pub fn new(oracle: &'a dyn SyntaxOracle) -> Self {
        Annotations {
            oracle,
            cache: HashMap::new(),
        }
    }

    pub fn facts(&mut self, tree: &SyntaxTree, h: u32) -> &Facts {
        let oracle = self.oracle;
        self.cache.entry(h).or_insert_with(|| {
            if h >= tree.host_handles {
                return created_facts(tree, h);
            }
            let mut q = vec![QUERY_QE_ANNOTATION];
            uvar(&mut q, u64::from(h));
            oracle
                .ask(&q)
                .and_then(|b| parse_facts(&b))
                .unwrap_or_default()
        })
    }

    /// Records the facts of an annotation the transform created.
    pub fn insert(&mut self, h: u32, facts: Facts) {
        self.cache.insert(h, facts);
    }

    /// `node.Annotation<T>()` for the `T` that `flag` stands for: the first such annotation.
    pub fn first(&mut self, tree: &SyntaxTree, node: SyntaxId, flag: u8) -> Option<u32> {
        let handles = tree.node(node).annotations.clone();
        handles
            .into_iter()
            .find(|&h| self.facts(tree, h).flags & flag != 0)
    }

    /// `node.RemoveAnnotations<T>()` for the `T` that `flag` stands for.
    pub fn remove(&mut self, tree: &mut SyntaxTree, node: SyntaxId, flag: u8) {
        let handles = tree.node(node).annotations.clone();
        let kept: Vec<u32> = handles
            .into_iter()
            .filter(|&h| self.facts(tree, h).flags & flag == 0)
            .collect();
        tree.node_mut(node).annotations = kept;
    }

    /// The object id of the variable of the first `ILVariableResolveResult` of `node`.
    pub fn variable(&mut self, tree: &SyntaxTree, node: SyntaxId) -> Option<(u32, Vec<u16>)> {
        let h = self.first(tree, node, QE_IL_VARIABLE)?;
        self.facts(tree, h).variable.clone()
    }
}

/// The facts of an annotation another ported transform created earlier in the same request, as
/// far as its kind tells them.
fn created_facts(tree: &SyntaxTree, h: u32) -> Facts {
    let Some((k, payload)) = tree.new_annotations.get((h - tree.host_handles) as usize) else {
        return Facts::default();
    };
    match *k {
        ANNOTATION_VARIABLE => {
            let mut rest = payload.as_slice();
            Facts {
                flags: QE_RESOLVE_RESULT | QE_IL_VARIABLE,
                variable: read_uvar(&mut rest).map(|id| (id as u32, Vec::new())),
            }
        }
        q::ANNOTATION_MEMBER_RESOLVE_RESULT => Facts {
            flags: QE_RESOLVE_RESULT | QE_MEMBER_RESOLVE_RESULT,
            variable: None,
        },
        // A TypeResolveResult, an OutVarResolveResult, ErrorResolveResult.UnknownError.
        q::ANNOTATION_RETURN_TYPE_RESOLVE_RESULT | 97 | 99 => Facts {
            flags: QE_RESOLVE_RESULT,
            variable: None,
        },
        _ => Facts::default(),
    }
}

fn read_uvar(bytes: &mut &[u8]) -> Option<u64> {
    let mut v = 0u64;
    let mut shift = 0;
    loop {
        let (&x, rest) = bytes.split_first()?;
        *bytes = rest;
        v |= u64::from(x & 0x7F) << shift;
        if x & 0x80 == 0 {
            return Some(v);
        }
        shift += 7;
        if shift > 63 {
            return None;
        }
    }
}

fn parse_facts(b: &[u8]) -> Option<Facts> {
    let (&flags, mut rest) = b.split_first()?;
    let variable = if flags & QE_IL_VARIABLE != 0 {
        let id = read_uvar(&mut rest)? as u32;
        let len = read_uvar(&mut rest)? as usize;
        let units = rest.get(..len * 2)?;
        let name = units
            .as_chunks::<2>()
            .0
            .iter()
            .map(|c| u16::from_le_bytes(*c))
            .collect();
        Some((id, name))
    } else {
        None
    };
    Some(Facts { flags, variable })
}

/// A boolean `DecompilerSettings` property, which the host must know.
pub(crate) fn setting(oracle: &dyn SyntaxOracle, name: &str) -> Result<bool, String> {
    q::setting(oracle, name)
        .ok_or_else(|| format!("the host has no transform context for the setting {name}"))
}

/// The name of the identifier in the single slot `slot` of `id` ("" where there is none).
pub(crate) fn name_in(tree: &SyntaxTree, id: SyntaxId, slot: usize) -> Vec<u16> {
    tree.child(id, slot)
        .map(|i| identifier_name(tree, i))
        .unwrap_or_default()
}

/// `new IdentifierExpression(name)`.
pub(crate) fn new_identifier_expression(tree: &mut SyntaxTree, name: &[u16]) -> SyntaxId {
    let e = tree.new_node(kind::IDENTIFIER_EXPRESSION);
    let i = new_identifier(tree, name);
    tree.set_child(e, identifier_expression::slot::IDENTIFIER_TOKEN, Some(i));
    e
}

/// `AstNode.NextSibling`.
fn next_sibling(tree: &SyntaxTree, id: SyntaxId) -> Option<SyntaxId> {
    let parent = tree.node(id).parent?;
    let siblings = tree.all_children(parent);
    let i = siblings.iter().position(|&s| s == id)?;
    siblings.get(i + 1).copied()
}

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
        oracle,
        annotations: Annotations::new(oracle),
        discards: None,
    };
    cx.decompile_queries(tree, root)?;
    // `rootNode.Descendants` is lazy: a node's next sibling is taken before the node is visited,
    // its first child after, so the clauses moved into a query are visited too.
    let mut next = Vec::new();
    let mut pos = tree.all_children(root).first().copied();
    while let Some(p) = pos {
        if let Some(n) = next_sibling(tree, p) {
            next.push(n);
        }
        if tree.kind(p) == kind::QUERY_EXPRESSION {
            cx.fix_degenerate_query(tree, p)?;
        }
        pos = tree.all_children(p).first().copied().or_else(|| next.pop());
    }
    Ok(())
}

struct Cx<'a> {
    oracle: &'a dyn SyntaxOracle,
    annotations: Annotations<'a>,
    discards: Option<bool>,
}

/// A simple lambda `a => b`: its parameter and its body.
struct SimpleLambda {
    parameter: SyntaxId,
    body: SyntaxId,
}

fn is_degenerate_query(tree: &SyntaxTree, query: Option<SyntaxId>) -> bool {
    let Some(query) = query.filter(|&q| tree.kind(q) == kind::QUERY_EXPRESSION) else {
        return false;
    };
    let last = tree
        .children(query, query_expression::slot::CLAUSES)
        .last()
        .map(|&c| tree.kind(c));
    !matches!(
        last,
        Some(kind::QUERY_SELECT_CLAUSE | kind::QUERY_GROUP_CLAUSE)
    )
}

fn query_expression_in(tree: &SyntaxTree, from: SyntaxId) -> Option<SyntaxId> {
    tree.child(from, query_from_clause::slot::EXPRESSION)
        .filter(|&e| tree.kind(e) == kind::QUERY_EXPRESSION)
}

fn first_from_clause(tree: &SyntaxTree, query: SyntaxId) -> Result<SyntaxId, String> {
    tree.children(query, query_expression::slot::CLAUSES)
        .first()
        .copied()
        .filter(|&c| tree.kind(c) == kind::QUERY_FROM_CLAUSE)
        .ok_or_else(|| "a query expression does not start with a from clause".to_owned())
}

/// `IntroduceQueryExpressions.IsNullConditional`.
pub(crate) fn is_null_conditional(tree: &SyntaxTree, target: Option<SyntaxId>) -> bool {
    let Some(t) = target else {
        return false;
    };
    match tree.kind(t) {
        kind::UNARY_OPERATOR_EXPRESSION => {
            tree.node(t).scalars[unary_operator_expression::scalar::OPERATOR].as_int()
                == Some(UNARY_NULL_CONDITIONAL)
        }
        kind::MEMBER_REFERENCE_EXPRESSION => is_null_conditional(
            tree,
            tree.child(t, member_reference_expression::slot::TARGET),
        ),
        kind::INVOCATION_EXPRESSION => {
            is_null_conditional(tree, tree.child(t, invocation_expression::slot::TARGET))
        }
        kind::INDEXER_EXPRESSION => {
            is_null_conditional(tree, tree.child(t, indexer_expression::slot::TARGET))
        }
        _ => false,
    }
}

fn is_query_clause(k: u16) -> bool {
    matches!(
        k,
        kind::QUERY_CONTINUATION_CLAUSE
            | kind::QUERY_FROM_CLAUSE
            | kind::QUERY_LET_CLAUSE
            | kind::QUERY_WHERE_CLAUSE
            | kind::QUERY_JOIN_CLAUSE
            | kind::QUERY_ORDER_CLAUSE
            | kind::QUERY_SELECT_CLAUSE
            | kind::QUERY_GROUP_CLAUSE
    )
}

fn is_complex_query(tree: &SyntaxTree, mre: SyntaxId) -> bool {
    let parent = tree.node(mre).parent;
    let target = tree.child(mre, member_reference_expression::slot::TARGET);
    (target.is_some_and(|t| tree.kind(t) == kind::INVOCATION_EXPRESSION)
        && parent.is_some_and(|p| tree.kind(p) == kind::INVOCATION_EXPRESSION))
        || parent
            .and_then(|p| tree.node(p).parent)
            .is_some_and(|g| is_query_clause(tree.kind(g)))
}

fn member_name(tree: &SyntaxTree, mre: SyntaxId) -> Vec<u16> {
    name_in(
        tree,
        mre,
        member_reference_expression::slot::MEMBER_NAME_TOKEN,
    )
}

fn parameter_name(tree: &SyntaxTree, p: SyntaxId) -> Vec<u16> {
    name_in(tree, p, parameter_declaration::slot::NAME_TOKEN)
}

fn validate_parameter(tree: &SyntaxTree, p: SyntaxId) -> bool {
    tree.node(p).scalars[parameter_declaration::scalar::PARAMETER_MODIFIER].as_int() == Some(0)
        && tree
            .children(p, parameter_declaration::slot::ATTRIBUTES)
            .is_empty()
}

fn lambda_parameters(tree: &SyntaxTree, lambda: SyntaxId) -> &[SyntaxId] {
    tree.children(lambda, lambda_expression::slot::PARAMETERS)
}

/// The expression body of a lambda, `None` for a block body or another node.
fn lambda_expression_body(tree: &SyntaxTree, expr: SyntaxId) -> Option<SyntaxId> {
    if tree.kind(expr) != kind::LAMBDA_EXPRESSION {
        return None;
    }
    tree.child(expr, lambda_expression::slot::BODY)
        .filter(|&b| is_expression(tree, b))
}

fn arguments(tree: &SyntaxTree, invocation: SyntaxId) -> Vec<SyntaxId> {
    tree.children(invocation, invocation_expression::slot::ARGUMENTS)
        .to_vec()
}

fn is(name: &[u16], text: &str) -> bool {
    name.iter().copied().eq(text.encode_utf16())
}

impl Cx<'_> {
    fn setting_discards(&mut self) -> Result<bool, String> {
        if let Some(v) = self.discards {
            return Ok(v);
        }
        let v = setting(self.oracle, "Discards")?;
        self.discards = Some(v);
        Ok(v)
    }

    fn decompile_queries(&mut self, tree: &mut SyntaxTree, node: SyntaxId) -> Result<(), String> {
        let mut current = node;
        if tree.kind(node) == kind::INVOCATION_EXPRESSION
            && let Some(query) = self.decompile_query(tree, node)?
        {
            let mut replacement = query;
            if tree
                .node(node)
                .parent
                .is_some_and(|p| tree.kind(p) == kind::EXPRESSION_STATEMENT)
                && self.setting_discards()?
            {
                let assignment = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
                tree.node_mut(assignment).scalars[assignment_expression::scalar::OPERATOR] =
                    Scalar::Int(0);
                let discard = new_identifier_expression(tree, &[u16::from(b'_')]);
                tree.set_child(assignment, assignment_expression::slot::LEFT, Some(discard));
                tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(query));
                replacement = assignment;
            }
            tree.replace(node, replacement);
            current = replacement;
        }
        // A child's visit may replace that child, never its siblings.
        for child in tree.all_children(current) {
            self.decompile_queries(tree, child)?;
        }
        Ok(())
    }

    /// `MatchSimpleLambda`.
    fn match_simple_lambda(&mut self, tree: &SyntaxTree, expr: SyntaxId) -> Option<SimpleLambda> {
        let body = lambda_expression_body(tree, expr)?;
        let [parameter] = *lambda_parameters(tree, expr) else {
            return None;
        };
        if let Some(rr) = self.annotations.first(tree, expr, QE_RESOLVE_RESULT)
            && self.annotations.facts(tree, rr).flags & QE_LAMBDA_TYPE_MISMATCH != 0
        {
            return None;
        }
        validate_parameter(tree, parameter).then_some(SimpleLambda { parameter, body })
    }

    /// `MakeFromClause`.
    fn make_from_clause(
        &mut self,
        tree: &mut SyntaxTree,
        parameter: SyntaxId,
        body: Option<SyntaxId>,
    ) -> SyntaxId {
        let from = tree.new_node(kind::QUERY_FROM_CLAUSE);
        let name = parameter_name(tree, parameter);
        let identifier = new_identifier(tree, &name);
        tree.set_child(
            from,
            query_from_clause::slot::IDENTIFIER_TOKEN,
            Some(identifier),
        );
        tree.set_child(from, query_from_clause::slot::EXPRESSION, body);
        copy_annotations_from(tree, from, parameter);
        from
    }

    /// `WrapExpressionInParenthesesIfNecessary`.
    fn wrap_if_necessary(
        tree: &mut SyntaxTree,
        expression: SyntaxId,
        parameter: &[u16],
    ) -> SyntaxId {
        if identifier_expression_name(tree, expression).is_some_and(|n| n == parameter) {
            let p = tree.new_node(kind::PARENTHESIZED_EXPRESSION);
            tree.set_child(
                p,
                parenthesized_expression::slot::EXPRESSION,
                Some(expression),
            );
            return p;
        }
        expression
    }

    fn new_query(tree: &mut SyntaxTree, clauses: &[SyntaxId]) -> SyntaxId {
        let query = tree.new_node(kind::QUERY_EXPRESSION);
        for (i, &c) in clauses.iter().enumerate() {
            tree.insert_child(query, query_expression::slot::CLAUSES, i, c);
        }
        query
    }

    fn select_clause(
        tree: &mut SyntaxTree,
        expression: SyntaxId,
        annotations_from: Option<SyntaxId>,
    ) -> SyntaxId {
        let select = tree.new_node(kind::QUERY_SELECT_CLAUSE);
        tree.set_child(
            select,
            query_select_clause::slot::EXPRESSION,
            Some(expression),
        );
        if let Some(a) = annotations_from {
            copy_annotations_from(tree, select, a);
        }
        select
    }

    /// The handle plus one of the first `ILFunction` annotation of `node`, 0 for none.
    fn function_payload(&mut self, tree: &SyntaxTree, node: SyntaxId, payload: &mut Vec<u8>) {
        let h = self.annotations.first(tree, node, QE_IL_FUNCTION);
        uvar(payload, h.map_or(0, |h| u64::from(h) + 1));
    }

    /// `DecompileQuery`.
    fn decompile_query(
        &mut self,
        tree: &mut SyntaxTree,
        invocation: SyntaxId,
    ) -> Result<Option<SyntaxId>, String> {
        let Some(mre) = tree
            .child(invocation, invocation_expression::slot::TARGET)
            .filter(|&t| tree.kind(t) == kind::MEMBER_REFERENCE_EXPRESSION)
        else {
            return Ok(None);
        };
        let mre_target = tree.child(mre, member_reference_expression::slot::TARGET);
        if is_null_conditional(tree, mre_target) {
            return Ok(None);
        }
        if !tree
            .children(mre, member_reference_expression::slot::TYPE_ARGUMENTS)
            .is_empty()
        {
            return Ok(None);
        }
        let name = String::from_utf16_lossy(&member_name(tree, mre));
        let args = arguments(tree, invocation);
        Ok(match name.as_str() {
            "Select" => {
                if args.len() != 1 || !is_complex_query(tree, mre) {
                    return Ok(None);
                }
                let expr = args[0];
                let Some(l) = self.match_simple_lambda(tree, expr) else {
                    return Ok(None);
                };
                let from = self.make_from_clause(tree, l.parameter, mre_target);
                let pname = parameter_name(tree, l.parameter);
                tree.detach(l.body);
                let body = Self::wrap_if_necessary(tree, l.body, &pname);
                let select = Self::select_clause(tree, body, Some(expr));
                Some(Self::new_query(tree, &[from, select]))
            }
            "GroupBy" => {
                if args.len() == 2 {
                    let (key_lambda, projection_lambda) = (args[0], args[1]);
                    let (Some(l1), Some(l2)) = (
                        self.match_simple_lambda(tree, key_lambda),
                        self.match_simple_lambda(tree, projection_lambda),
                    ) else {
                        return Ok(None);
                    };
                    if parameter_name(tree, l1.parameter) != parameter_name(tree, l2.parameter) {
                        return Ok(None);
                    }
                    let from = self.make_from_clause(tree, l1.parameter, mre_target);
                    let group = tree.new_node(kind::QUERY_GROUP_CLAUSE);
                    tree.set_child(group, query_group_clause::slot::PROJECTION, Some(l2.body));
                    tree.set_child(group, query_group_clause::slot::KEY, Some(l1.body));
                    let mut payload = Vec::new();
                    self.function_payload(tree, key_lambda, &mut payload);
                    self.function_payload(tree, projection_lambda, &mut payload);
                    let h = tree.new_annotation(ANNOTATION_QUERY_GROUP_CLAUSE, payload);
                    tree.node_mut(group).annotations.push(h);
                    Some(Self::new_query(tree, &[from, group]))
                } else if args.len() == 1 {
                    let Some(l) = self.match_simple_lambda(tree, args[0]) else {
                        return Ok(None);
                    };
                    let from = self.make_from_clause(tree, l.parameter, mre_target);
                    let pname = parameter_name(tree, l.parameter);
                    let projection = new_identifier_expression(tree, &pname);
                    copy_annotations_from(tree, projection, l.parameter);
                    let group = tree.new_node(kind::QUERY_GROUP_CLAUSE);
                    tree.set_child(
                        group,
                        query_group_clause::slot::PROJECTION,
                        Some(projection),
                    );
                    tree.set_child(group, query_group_clause::slot::KEY, Some(l.body));
                    Some(Self::new_query(tree, &[from, group]))
                } else {
                    None
                }
            }
            "SelectMany" => {
                if args.len() != 2 {
                    return Ok(None);
                }
                let from_expression_lambda = args[0];
                let Some(l) = self.match_simple_lambda(tree, from_expression_lambda) else {
                    return Ok(None);
                };
                if is_null_conditional(tree, Some(l.body)) {
                    return Ok(None);
                }
                let lambda = args[1];
                let Some(body) = lambda_expression_body(tree, lambda) else {
                    return Ok(None);
                };
                let [p1, p2] = *lambda_parameters(tree, lambda) else {
                    return Ok(None);
                };
                let pname = parameter_name(tree, l.parameter);
                if parameter_name(tree, p1) != pname {
                    return Ok(None);
                }
                let from1 = self.make_from_clause(tree, p1, mre_target);
                let from2 = self.make_from_clause(tree, p2, Some(l.body));
                copy_annotations_from(tree, from2, from_expression_lambda);
                tree.detach(body);
                let body = Self::wrap_if_necessary(tree, body, &pname);
                let select = Self::select_clause(tree, body, None);
                Some(Self::new_query(tree, &[from1, from2, select]))
            }
            "Where" => {
                if args.len() != 1 || !is_complex_query(tree, mre) {
                    return Ok(None);
                }
                let expr = args[0];
                let Some(l) = self.match_simple_lambda(tree, expr) else {
                    return Ok(None);
                };
                let from = self.make_from_clause(tree, l.parameter, mre_target);
                let clause = tree.new_node(kind::QUERY_WHERE_CLAUSE);
                tree.set_child(clause, query_where_clause::slot::CONDITION, Some(l.body));
                copy_annotations_from(tree, clause, expr);
                Some(Self::new_query(tree, &[from, clause]))
            }
            "OrderBy" | "OrderByDescending" | "ThenBy" | "ThenByDescending" => {
                if args.len() != 1 || !is_complex_query(tree, mre) {
                    return Ok(None);
                }
                let mut lambda = args[0];
                let Some(mut l) = self.match_simple_lambda(tree, lambda) else {
                    return Ok(None);
                };
                let pname = parameter_name(tree, l.parameter);
                if !self.validate_then_by_chain(tree, Some(invocation), &pname) {
                    return Ok(None);
                }
                let order = tree.new_node(kind::QUERY_ORDER_CLAUSE);
                let mut mre = mre;
                loop {
                    let n = member_name(tree, mre);
                    let then_by = is(&n, "ThenBy");
                    let last = !then_by && !is(&n, "ThenByDescending");
                    let ascending = if last { is(&n, "OrderBy") } else { then_by };
                    let ordering = tree.new_node(kind::QUERY_ORDERING);
                    tree.set_child(ordering, query_ordering::slot::EXPRESSION, Some(l.body));
                    tree.node_mut(ordering).scalars[query_ordering::scalar::DIRECTION] =
                        Scalar::Int(if ascending { 0 } else { DIRECTION_DESCENDING });
                    copy_annotations_from(tree, ordering, lambda);
                    // insert new ordering at beginning
                    tree.insert_child(order, query_order_clause::slot::ORDERINGS, 0, ordering);
                    if last {
                        break;
                    }
                    // `ValidateThenByChain` checked every link of the chain.
                    let tmp = tree
                        .child(mre, member_reference_expression::slot::TARGET)
                        .ok_or("a validated ThenBy chain ends early")?;
                    mre = tree
                        .child(tmp, invocation_expression::slot::TARGET)
                        .ok_or("a validated ThenBy chain ends early")?;
                    lambda = arguments(tree, tmp)[0];
                    l = self
                        .match_simple_lambda(tree, lambda)
                        .ok_or("a validated ThenBy chain has no simple lambda")?;
                }
                let target = tree.child(mre, member_reference_expression::slot::TARGET);
                let from = self.make_from_clause(tree, l.parameter, target);
                Some(Self::new_query(tree, &[from, order]))
            }
            "Join" | "GroupJoin" => {
                if args.len() != 4 {
                    return Ok(None);
                }
                let group_join = name == "GroupJoin";
                let source2 = args[0];
                if is_null_conditional(tree, Some(source2)) {
                    return Ok(None);
                }
                let outer_lambda = args[1];
                let Some(outer) = self.match_simple_lambda(tree, outer_lambda) else {
                    return Ok(None);
                };
                let inner_lambda = args[2];
                let Some(inner) = self.match_simple_lambda(tree, inner_lambda) else {
                    return Ok(None);
                };
                let lambda = args[3];
                let Some(body) = lambda_expression_body(tree, lambda) else {
                    return Ok(None);
                };
                let [p1, p2] = *lambda_parameters(tree, lambda) else {
                    return Ok(None);
                };
                let element2_name = parameter_name(tree, inner.parameter);
                if !(validate_parameter(tree, p1)
                    && validate_parameter(tree, p2)
                    && parameter_name(tree, p1) == parameter_name(tree, outer.parameter)
                    && (parameter_name(tree, p2) == element2_name || group_join))
                {
                    return Ok(None);
                }
                let from = self.make_from_clause(tree, outer.parameter, mre_target);
                let join = tree.new_node(kind::QUERY_JOIN_CLAUSE);
                let token = new_identifier(tree, &element2_name);
                tree.set_child(
                    join,
                    query_join_clause::slot::JOIN_IDENTIFIER_TOKEN,
                    Some(token),
                );
                copy_annotations_from(tree, token, inner.parameter);
                tree.set_child(join, query_join_clause::slot::IN_EXPRESSION, Some(source2));
                tree.set_child(
                    join,
                    query_join_clause::slot::ON_EXPRESSION,
                    Some(outer.body),
                );
                tree.set_child(
                    join,
                    query_join_clause::slot::EQUALS_EXPRESSION,
                    Some(inner.body),
                );
                if group_join {
                    // `Identifier.CreateIfNotEmpty`: an empty name is no token.
                    let into = parameter_name(tree, p2);
                    if !into.is_empty() {
                        let token = new_identifier(tree, &into);
                        tree.set_child(
                            join,
                            query_join_clause::slot::INTO_IDENTIFIER_TOKEN,
                            Some(token),
                        );
                        copy_annotations_from(tree, token, p2);
                    }
                }
                let mut payload = Vec::new();
                self.function_payload(tree, outer_lambda, &mut payload);
                self.function_payload(tree, inner_lambda, &mut payload);
                let h = tree.new_annotation(ANNOTATION_QUERY_JOIN_CLAUSE, payload);
                tree.node_mut(join).annotations.push(h);
                let select = Self::select_clause(tree, body, Some(lambda));
                Some(Self::new_query(tree, &[from, join, select]))
            }
            _ => None,
        })
    }

    /// `ValidateThenByChain`.
    fn validate_then_by_chain(
        &mut self,
        tree: &SyntaxTree,
        invocation: Option<SyntaxId>,
        expected: &[u16],
    ) -> bool {
        let Some(invocation) = invocation.filter(|&i| tree.kind(i) == kind::INVOCATION_EXPRESSION)
        else {
            return false;
        };
        let [arg] = *tree.children(invocation, invocation_expression::slot::ARGUMENTS) else {
            return false;
        };
        let Some(mre) = tree
            .child(invocation, invocation_expression::slot::TARGET)
            .filter(|&t| tree.kind(t) == kind::MEMBER_REFERENCE_EXPRESSION)
        else {
            return false;
        };
        let Some(l) = self.match_simple_lambda(tree, arg) else {
            return false;
        };
        if parameter_name(tree, l.parameter) != expected {
            return false;
        }
        if !tree
            .children(mre, member_reference_expression::slot::TYPE_ARGUMENTS)
            .is_empty()
        {
            return false;
        }
        let n = member_name(tree, mre);
        let target = tree.child(mre, member_reference_expression::slot::TARGET);
        if is(&n, "OrderBy") || is(&n, "OrderByDescending") {
            !is_null_conditional(tree, target)
        } else if is(&n, "ThenBy") || is(&n, "ThenByDescending") {
            self.validate_then_by_chain(tree, target, expected)
        } else {
            false
        }
    }

    /// The loop body of `Run` after all queries were decompiled: adds a degenerate `select` and
    /// combines the query with degenerate queries it reads from.
    fn fix_degenerate_query(
        &mut self,
        tree: &mut SyntaxTree,
        query: SyntaxId,
    ) -> Result<(), String> {
        let mut from = first_from_clause(tree, query)?;
        if is_degenerate_query(tree, Some(query)) {
            // introduce select for degenerate query
            let name = name_in(tree, from, query_from_clause::slot::IDENTIFIER_TOKEN);
            let expression = new_identifier_expression(tree, &name);
            copy_annotations_from(tree, expression, from);
            let select = Self::select_clause(tree, expression, None);
            let count = tree.children(query, query_expression::slot::CLAUSES).len();
            tree.insert_child(query, query_expression::slot::CLAUSES, count, select);
        }
        let mut inner = query_expression_in(tree, from);
        while let Some(inner_query) = inner.filter(|&q| is_degenerate_query(tree, Some(q))) {
            let inner_from = first_from_clause(tree, inner_query)?;
            let inner_variable = self.annotations.variable(tree, inner_from).map(|v| v.0);
            let range_variable = self.annotations.variable(tree, from);
            // Replace the fromClause with all clauses from the inner query
            tree.detach(from);
            let clauses = tree
                .children(inner_query, query_expression::slot::CLAUSES)
                .to_vec();
            for (i, clause) in clauses.into_iter().enumerate() {
                self.combine_range_variables(tree, clause, inner_variable, range_variable.clone());
                tree.insert_child(query, query_expression::slot::CLAUSES, i, clause);
            }
            from = inner_from;
            inner = query_expression_in(tree, from);
        }
        Ok(())
    }

    /// `CombineRangeVariables`.
    fn combine_range_variables(
        &mut self,
        tree: &mut SyntaxTree,
        clause: SyntaxId,
        old: Option<u32>,
        new: Option<(u32, Vec<u16>)>,
    ) {
        let (Some(old), Some((new_id, new_name))) = (old, new) else {
            return;
        };
        for identifier in tree.descendants_and_self(clause).into_iter().skip(1) {
            if tree.kind(identifier) != kind::IDENTIFIER {
                continue;
            }
            let Some(parent) = tree.node(identifier).parent else {
                continue;
            };
            if self.annotations.variable(tree, parent).map(|v| v.0) != Some(old) {
                continue;
            }
            self.annotations.remove(tree, parent, QE_IL_VARIABLE);
            let mut payload = Vec::new();
            uvar(&mut payload, u64::from(new_id));
            let h = tree.new_annotation(ANNOTATION_VARIABLE, payload);
            self.annotations.insert(
                h,
                Facts {
                    flags: QE_RESOLVE_RESULT | QE_IL_VARIABLE,
                    variable: Some((new_id, new_name.clone())),
                },
            );
            tree.node_mut(parent).annotations.push(h);
            let new_identifier = new_identifier(tree, &new_name);
            tree.replace(identifier, new_identifier);
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_oracle::NoOracle;

    #[test]
    fn facts_are_parsed() {
        let mut b = vec![QE_RESOLVE_RESULT | QE_IL_VARIABLE, 3, 2];
        b.extend_from_slice(&[b'a', 0, b'b', 0]);
        assert_eq!(
            parse_facts(&b),
            Some(Facts {
                flags: QE_RESOLVE_RESULT | QE_IL_VARIABLE,
                variable: Some((3, vec![u16::from(b'a'), u16::from(b'b')])),
            })
        );
        assert_eq!(parse_facts(&[QE_IL_VARIABLE, 3, 2, b'a']), None);
        assert_eq!(
            parse_facts(&[QE_IL_FUNCTION]).unwrap().flags,
            QE_IL_FUNCTION
        );
    }

    #[test]
    fn created_variables_are_known() {
        let mut tree = SyntaxTree {
            host_handles: 2,
            ..SyntaxTree::default()
        };
        let h = tree.new_annotation(ANNOTATION_VARIABLE, vec![5]);
        let mut a = Annotations::new(&NoOracle);
        assert_eq!(a.facts(&tree, h).variable, Some((5, Vec::new())));
        assert_eq!(a.facts(&tree, 0).flags, 0);
    }

    #[test]
    fn null_conditional_targets_are_found_through_member_chains() {
        let mut tree = SyntaxTree::default();
        let uoe = tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
        tree.node_mut(uoe).scalars[unary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(UNARY_NULL_CONDITIONAL);
        let mre = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
        tree.set_child(mre, member_reference_expression::slot::TARGET, Some(uoe));
        let inv = tree.new_node(kind::INVOCATION_EXPRESSION);
        tree.set_child(inv, invocation_expression::slot::TARGET, Some(mre));
        assert!(is_null_conditional(&tree, Some(inv)));
        let plain = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        assert!(!is_null_conditional(&tree, Some(plain)));
        assert!(!is_null_conditional(&tree, None));
    }

    #[test]
    fn a_query_without_select_or_group_is_degenerate() {
        let mut tree = SyntaxTree::default();
        let from = tree.new_node(kind::QUERY_FROM_CLAUSE);
        let query = Cx::new_query(&mut tree, &[from]);
        assert!(is_degenerate_query(&tree, Some(query)));
        let e = new_identifier_expression(&mut tree, &[u16::from(b'x')]);
        let select = Cx::select_clause(&mut tree, e, None);
        tree.insert_child(query, query_expression::slot::CLAUSES, 1, select);
        assert!(!is_degenerate_query(&tree, Some(query)));
        assert!(!is_degenerate_query(&tree, None));
    }
}
