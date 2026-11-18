//! `RewriteVisualBasicWithEventsInInitializeComponent`: in a Windows Forms `InitializeComponent`,
//! references to a VB `WithEvents` property become references to its backing field, and an
//! assignment of the property is followed by the event subscriptions its setter performs, the
//! shape the Windows Forms designer recognizes. Assignments outside `InitializeComponent` stay on
//! the property, which keeps VB's detach/attach semantics on reassignment.
//!
//! What the transform knows about symbols it asks the host (`syntax_oracle`, queries 32..47).

use std::collections::HashMap;

use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{
    self as q, ANNOTATION_HOST_OBJECT, ANNOTATION_MEMBER_RESOLVE_RESULT,
    ANNOTATION_RETURN_TYPE_RESOLVE_RESULT, MEMBER_HAS_SETTER, MEMBER_STATIC,
    MEMBER_WINDOWS_FORMS_INITIALIZE_COMPONENT, MemberFacts, MemberRole, SyntaxOracle,
};
use crate::csharp::syntax_schema::{
    accessor, assignment_expression, block_statement, expression_statement, identifier,
    identifier_expression, kind, member_reference_expression, object_create_expression,
    property_declaration, variable_declaration_statement, variable_initializer,
};
use crate::typesystem::types::TypeSystem;

/// `AssignmentOperatorType` values.
const ASSIGN: i64 = 0;
const ADD: i64 = 1;
const SUBTRACT: i64 = 2;

/// `VisualBasicWithEventsSubscription`, by symbol ids.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
struct Subscription {
    event: u32,
    handler: u32,
}

/// `VisualBasicWithEventsInfo` without the property, which keys it.
struct Info {
    backing_field: u32,
    subscriptions: Vec<Subscription>,
}

pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let mut rewriter = Rewriter {
        oracle,
        properties: HashMap::new(),
        in_initialize_component: false,
    };
    rewriter.collect_with_events_properties(tree, root)?;
    if !rewriter.properties.is_empty() {
        rewriter.visit(tree, root)?;
    }
    Ok(())
}

struct Rewriter<'o> {
    oracle: &'o dyn SyntaxOracle,
    /// The `WithEvents` properties by the symbol id of their definition.
    properties: HashMap<u32, Info>,
    in_initialize_component: bool,
}

impl Rewriter<'_> {
    /// `AstNode.GetSymbol`: the symbol of the node's first `ResolveResult` annotation.
    fn symbol(&self, tree: &SyntaxTree, id: SyntaxId) -> Option<u32> {
        tree.node(id)
            .annotations
            .iter()
            .find_map(|&h| q::node_symbol(self.oracle, h))
            .flatten()
    }

    fn facts(&self, symbol: u32) -> Result<MemberFacts, String> {
        q::member_facts(self.oracle, symbol)
            .ok_or_else(|| format!("the host knows no symbol {symbol}"))
    }

    /// The symbol of `id` with its facts where it has the role `role`.
    fn symbol_as(
        &self,
        tree: &SyntaxTree,
        id: SyntaxId,
        role: MemberRole,
    ) -> Result<Option<(u32, MemberFacts)>, String> {
        let Some(symbol) = self.symbol(tree, id) else {
            return Ok(None);
        };
        let facts = self.facts(symbol)?;
        Ok((facts.role == role).then_some((symbol, facts)))
    }

    fn collect_with_events_properties(
        &mut self,
        tree: &SyntaxTree,
        root: SyntaxId,
    ) -> Result<(), String> {
        for property in descendants(tree, root) {
            if tree.kind(property) != kind::PROPERTY_DECLARATION {
                continue;
            }
            if let Some((symbol, info)) = self.try_analyze_with_events_property(tree, property)? {
                self.properties.insert(symbol, info);
            } else if let Some((symbol, facts)) =
                self.symbol_as(tree, property, MemberRole::Property)?
                && let Some(field) = q::accessed_through_property_field(self.oracle, symbol)
            {
                self.properties.insert(
                    facts.member_definition,
                    Info {
                        backing_field: field,
                        subscriptions: Vec::new(),
                    },
                );
            }
        }
        Ok(())
    }

    /// `AutoEventDecompiler.TryAnalyzeVisualBasicWithEventsProperty`: the property's symbol id
    /// and what its setter does.
    fn try_analyze_with_events_property(
        &self,
        tree: &SyntaxTree,
        declaration: SyntaxId,
    ) -> Result<Option<(u32, Info)>, String> {
        let Some((property, facts)) = self.symbol_as(tree, declaration, MemberRole::Property)?
        else {
            return Ok(None);
        };
        let Some(body) = tree
            .child(declaration, property_declaration::slot::SETTER)
            .and_then(|setter| tree.child(setter, accessor::slot::BODY))
        else {
            return Ok(None);
        };
        if facts.parameter_count != 0 || facts.flags & MEMBER_HAS_SETTER == 0 {
            return Ok(None);
        }
        let nodes = descendants(tree, body);
        let assignments = || {
            nodes
                .iter()
                .copied()
                .filter(|&n| tree.kind(n) == kind::ASSIGNMENT_EXPRESSION)
        };

        let mut backing_field = None;
        for assignment in assignments() {
            if operator(tree, assignment) != ASSIGN {
                continue;
            }
            let right = tree.child(assignment, assignment_expression::slot::RIGHT);
            if right.and_then(|r| identifier_expression_name(tree, r)) != Some(utf16("value")) {
                continue;
            }
            let Some(left) = tree.child(assignment, assignment_expression::slot::LEFT) else {
                continue;
            };
            let Some((_, field)) = self.symbol_as(tree, left, MemberRole::Field)? else {
                continue;
            };
            let field = field.member_definition;
            if backing_field.is_some_and(|b| b != field) {
                return Ok(None);
            }
            backing_field = Some(field);
        }
        let Some(backing_field) = backing_field else {
            return Ok(None);
        };
        if !self.looks_like_with_events_backing_field(property, &facts, backing_field)? {
            return Ok(None);
        }

        let mut local_handlers = HashMap::new();
        for &declaration in &nodes {
            if tree.kind(declaration) != kind::VARIABLE_DECLARATION_STATEMENT {
                continue;
            }
            for &variable in
                tree.children(declaration, variable_declaration_statement::slot::VARIABLES)
            {
                let initializer = tree.child(variable, variable_initializer::slot::INITIALIZER);
                if let Some(handler) = self.try_get_event_handler(tree, initializer)? {
                    let name = tree
                        .child(variable, variable_initializer::slot::NAME_TOKEN)
                        .map(|n| identifier_name(tree, n))
                        .unwrap_or_default();
                    local_handlers.insert(name, handler);
                }
            }
        }

        let mut added = Vec::new();
        let mut removed = Vec::new();
        for assignment in assignments() {
            let op = operator(tree, assignment);
            if op != ADD && op != SUBTRACT {
                continue;
            }
            let Some(left) = tree.child(assignment, assignment_expression::slot::LEFT) else {
                continue;
            };
            let Some((event, _)) = self.symbol_as(tree, left, MemberRole::Event)? else {
                continue;
            };
            let right = tree.child(assignment, assignment_expression::slot::RIGHT);
            let handler = match self.try_get_event_handler(tree, right)? {
                Some(h) => h,
                None => match right
                    .and_then(|r| identifier_expression_name(tree, r))
                    .and_then(|name| local_handlers.get(&name))
                {
                    Some(&h) => h,
                    None => continue,
                },
            };
            let subscription = Subscription { event, handler };
            if op == ADD {
                added.push(subscription);
            } else {
                removed.push(subscription);
            }
        }

        if added.is_empty() || removed.len() != added.len() {
            return Ok(None);
        }
        if !added.iter().all(|a| removed.contains(a)) {
            return Ok(None);
        }
        Ok(Some((
            property,
            Info {
                backing_field,
                subscriptions: added,
            },
        )))
    }

    /// `AutoEventDecompiler.LooksLikeVisualBasicWithEventsBackingField`.
    fn looks_like_with_events_backing_field(
        &self,
        property: u32,
        property_facts: &MemberFacts,
        field: u32,
    ) -> Result<bool, String> {
        if !q::field_matches_property(self.oracle, property, field) {
            return Ok(false);
        }
        let field_facts = self.facts(field)?;
        if (field_facts.flags & MEMBER_STATIC) != (property_facts.flags & MEMBER_STATIC) {
            return Ok(false);
        }
        let mut underscore_name = utf16("_");
        underscore_name.extend_from_slice(&property_facts.name);
        if field_facts.name == underscore_name {
            return Ok(true);
        }
        Ok(q::accessed_through_property_names(self.oracle, field).contains(&property_facts.name))
    }

    /// `AutoEventDecompiler.TryGetEventHandler`: the method a handler expression names.
    fn try_get_event_handler(
        &self,
        tree: &SyntaxTree,
        expression: Option<SyntaxId>,
    ) -> Result<Option<u32>, String> {
        let mut target = expression;
        if let Some(e) = expression
            && tree.kind(e) == kind::OBJECT_CREATE_EXPRESSION
            && let [argument] = tree.children(e, object_create_expression::slot::ARGUMENTS)
        {
            target = Some(*argument);
        }
        let Some(target) = target else {
            return Ok(None);
        };
        Ok(self
            .symbol_as(tree, target, MemberRole::Method)?
            .map(|(m, _)| m))
    }

    /// The visitor, in the order of `DepthFirstAstVisitor`: each node's children as they are
    /// before the node is visited.
    fn visit(&mut self, tree: &mut SyntaxTree, id: SyntaxId) -> Result<(), String> {
        match tree.kind(id) {
            kind::METHOD_DECLARATION => {
                let is_initialize_component = self
                    .symbol_as(tree, id, MemberRole::Method)?
                    .is_some_and(|(_, f)| f.flags & MEMBER_WINDOWS_FORMS_INITIALIZE_COMPONENT != 0);
                if !is_initialize_component {
                    return Ok(());
                }
                let old = self.in_initialize_component;
                self.in_initialize_component = true;
                let result = self.visit_children(tree, id);
                self.in_initialize_component = old;
                result
            }
            kind::EXPRESSION_STATEMENT => self.visit_expression_statement(tree, id),
            kind::IDENTIFIER_EXPRESSION | kind::MEMBER_REFERENCE_EXPRESSION => {
                if self.try_rewrite_property_reference(tree, id)? {
                    return Ok(());
                }
                self.visit_children(tree, id)
            }
            _ => self.visit_children(tree, id),
        }
    }

    fn visit_children(&mut self, tree: &mut SyntaxTree, id: SyntaxId) -> Result<(), String> {
        for child in tree.all_children(id) {
            self.visit(tree, child)?;
        }
        Ok(())
    }

    fn visit_expression_statement(
        &mut self,
        tree: &mut SyntaxTree,
        statement: SyntaxId,
    ) -> Result<(), String> {
        let Some(info) = self.with_events_assignment(tree, statement)? else {
            return self.visit_children(tree, statement);
        };
        let (assignment, backing_field, subscriptions) = info;
        let left = tree
            .child(assignment, assignment_expression::slot::LEFT)
            .ok_or("an assignment without left side")?;
        let field = self.create_field_reference(tree, backing_field)?;
        tree.replace(left, field);
        self.visit_children(tree, statement)?;
        let Some(block) = tree.node(statement).parent else {
            return Ok(());
        };
        if tree.kind(block) != kind::BLOCK_STATEMENT {
            return Ok(());
        }
        let mut index = tree
            .children(block, block_statement::slot::STATEMENTS)
            .iter()
            .position(|&s| s == statement)
            .ok_or("a statement missing from its block")?;
        for subscription in subscriptions {
            let s = self.create_subscription_statement(tree, backing_field, subscription)?;
            index += 1;
            tree.insert_child(block, block_statement::slot::STATEMENTS, index, s);
        }
        Ok(())
    }

    /// For `statement` in `InitializeComponent` assigning a `WithEvents` property with `=`: the
    /// assignment, the backing field and the subscriptions.
    fn with_events_assignment(
        &self,
        tree: &SyntaxTree,
        statement: SyntaxId,
    ) -> Result<Option<(SyntaxId, u32, Vec<Subscription>)>, String> {
        if !self.in_initialize_component {
            return Ok(None);
        }
        let Some(assignment) = tree.child(statement, expression_statement::slot::EXPRESSION) else {
            return Ok(None);
        };
        if tree.kind(assignment) != kind::ASSIGNMENT_EXPRESSION
            || operator(tree, assignment) != ASSIGN
        {
            return Ok(None);
        }
        let Some(left) = tree.child(assignment, assignment_expression::slot::LEFT) else {
            return Ok(None);
        };
        let Some((_, facts)) = self.symbol_as(tree, left, MemberRole::Property)? else {
            return Ok(None);
        };
        Ok(self
            .properties
            .get(&facts.member_definition)
            .map(|info| (assignment, info.backing_field, info.subscriptions.clone())))
    }

    fn try_rewrite_property_reference(
        &mut self,
        tree: &mut SyntaxTree,
        expression: SyntaxId,
    ) -> Result<bool, String> {
        if !self.in_initialize_component {
            return Ok(false);
        }
        let Some((_, facts)) = self.symbol_as(tree, expression, MemberRole::Property)? else {
            return Ok(false);
        };
        let Some(backing_field) = self
            .properties
            .get(&facts.member_definition)
            .map(|i| i.backing_field)
        else {
            return Ok(false);
        };
        let field = self.create_field_reference(tree, backing_field)?;
        // `CopyAnnotationsFrom`: the annotations after the new ones, the trivia copied.
        let annotations = tree.node(expression).annotations.clone();
        tree.node_mut(field).annotations.extend(annotations);
        let leading = tree.node(expression).leading_trivia.clone();
        let trailing = tree.node(expression).trailing_trivia.clone();
        for (list, is_leading) in [(leading, true), (trailing, false)] {
            for t in list {
                let copy = tree.clone_subtree(t);
                tree.node_mut(copy).parent = Some(field);
                let node = tree.node_mut(field);
                if is_leading {
                    node.leading_trivia.push(copy);
                } else {
                    node.trailing_trivia.push(copy);
                }
            }
        }
        tree.replace(expression, field);
        Ok(true)
    }

    /// `this.name` or, for a static member, `name`, annotated with the member.
    fn create_member_reference(
        &self,
        tree: &mut SyntaxTree,
        member: u32,
    ) -> Result<SyntaxId, String> {
        let facts = self.facts(member)?;
        let node = if facts.flags & MEMBER_STATIC != 0 {
            let e = tree.new_node(kind::IDENTIFIER_EXPRESSION);
            let name = new_identifier(tree, &facts.name);
            tree.set_child(e, identifier_expression::slot::IDENTIFIER_TOKEN, Some(name));
            e
        } else {
            let this = tree.new_node(kind::THIS_REFERENCE_EXPRESSION);
            member_reference(tree, this, &facts.name)
        };
        let rr = member_resolve_result(tree, member);
        tree.node_mut(node).annotations.push(rr);
        Ok(node)
    }

    /// `CreateFieldReference`.
    fn create_field_reference(
        &self,
        tree: &mut SyntaxTree,
        field: u32,
    ) -> Result<SyntaxId, String> {
        self.create_member_reference(tree, field)
    }

    /// `CreateSubscriptionStatement`: `field.Event += new EventType(handler);`.
    fn create_subscription_statement(
        &self,
        tree: &mut SyntaxTree,
        field: u32,
        subscription: Subscription,
    ) -> Result<SyntaxId, String> {
        let event_target = self.create_field_reference(tree, field)?;
        let event = self.facts(subscription.event)?;
        let left = member_reference(tree, event_target, &event.name);
        let rr = member_resolve_result(tree, subscription.event);
        tree.node_mut(left).annotations.push(rr);
        let handler = self.create_member_reference(tree, subscription.handler)?;
        let right = tree.new_node(kind::OBJECT_CREATE_EXPRESSION);
        let ty = self.return_type_syntax(tree, subscription.event)?;
        tree.set_child(right, object_create_expression::slot::TYPE, Some(ty));
        tree.insert_child(right, object_create_expression::slot::ARGUMENTS, 0, handler);
        let assignment = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        tree.set_child(assignment, assignment_expression::slot::LEFT, Some(left));
        tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(right));
        tree.node_mut(assignment).scalars[assignment_expression::scalar::OPERATOR] =
            Scalar::Int(ADD);
        let mut payload = Vec::new();
        uvar(&mut payload, u64::from(subscription.event));
        let rr = tree.new_annotation(ANNOTATION_RETURN_TYPE_RESOLVE_RESULT, payload);
        tree.node_mut(assignment).annotations.push(rr);
        let statement = tree.new_node(kind::EXPRESSION_STATEMENT);
        tree.set_child(
            statement,
            expression_statement::slot::EXPRESSION,
            Some(assignment),
        );
        Ok(statement)
    }

    /// `TypeSystemAstBuilder.ConvertType(member.ReturnType)`, which the host builds; its
    /// annotations become annotations the engine creates, referring to the host's objects.
    fn return_type_syntax(&self, tree: &mut SyntaxTree, member: u32) -> Result<SyntaxId, String> {
        let (base, bytes) = q::return_type_syntax(self.oracle, member)
            .ok_or("the host did not convert the return type")?;
        let first = tree.nodes.len();
        let ty = tree.decode_subtree(&bytes)?;
        let host_object = |tree: &mut SyntaxTree, h: u32| {
            let mut payload = Vec::new();
            uvar(&mut payload, u64::from(base + h));
            tree.new_annotation(ANNOTATION_HOST_OBJECT, payload)
        };
        for n in first..tree.nodes.len() {
            let id = SyntaxId(n as u32);
            let annotations = tree.node(id).annotations.clone();
            let remapped = annotations
                .into_iter()
                .map(|h| host_object(tree, h))
                .collect();
            tree.node_mut(id).annotations = remapped;
            for i in 0..tree.node(id).scalars.len() {
                if let Scalar::Object(h) = tree.node(id).scalars[i] {
                    let handle = host_object(tree, h);
                    tree.node_mut(id).scalars[i] = Scalar::Object(handle);
                }
            }
        }
        Ok(ty)
    }
}

fn member_resolve_result(tree: &mut SyntaxTree, member: u32) -> u32 {
    let mut payload = Vec::new();
    uvar(&mut payload, u64::from(member));
    tree.new_annotation(ANNOTATION_MEMBER_RESOLVE_RESULT, payload)
}

/// `new MemberReferenceExpression(target, name)`.
fn member_reference(tree: &mut SyntaxTree, target: SyntaxId, name: &[u16]) -> SyntaxId {
    let e = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
    tree.set_child(e, member_reference_expression::slot::TARGET, Some(target));
    let name = new_identifier(tree, name);
    tree.set_child(
        e,
        member_reference_expression::slot::MEMBER_NAME_TOKEN,
        Some(name),
    );
    e
}

/// `Identifier.Create(name)`: a leading `@` makes a verbatim identifier.
pub(crate) fn new_identifier(tree: &mut SyntaxTree, name: &[u16]) -> SyntaxId {
    let id = tree.new_node(kind::IDENTIFIER);
    let (name, verbatim) = match name.split_first() {
        Some((&c, rest)) if c == u16::from(b'@') => (rest, true),
        _ => (name, false),
    };
    let node = tree.node_mut(id);
    node.scalars[identifier::scalar::NAME] = Scalar::Str(name.to_vec());
    node.scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
    id
}

fn operator(tree: &SyntaxTree, assignment: SyntaxId) -> i64 {
    tree.node(assignment).scalars[assignment_expression::scalar::OPERATOR]
        .as_int()
        .unwrap_or(-1)
}

pub(crate) fn identifier_name(tree: &SyntaxTree, identifier: SyntaxId) -> Vec<u16> {
    match &tree.node(identifier).scalars[identifier::scalar::NAME] {
        Scalar::Str(s) => s.clone(),
        _ => Vec::new(),
    }
}

/// `IdentifierExpression.Identifier` where `id` is an identifier expression.
pub(crate) fn identifier_expression_name(tree: &SyntaxTree, id: SyntaxId) -> Option<Vec<u16>> {
    if tree.kind(id) != kind::IDENTIFIER_EXPRESSION {
        return None;
    }
    let token = tree.child(id, identifier_expression::slot::IDENTIFIER_TOKEN)?;
    Some(identifier_name(tree, token))
}

pub(crate) fn utf16(s: &str) -> Vec<u16> {
    s.encode_utf16().collect()
}

/// `AstNode.Descendants`: the subtree below `id` in pre-order, without trivia.
fn descendants(tree: &SyntaxTree, id: SyntaxId) -> Vec<SyntaxId> {
    let mut result = Vec::new();
    let mut stack: Vec<SyntaxId> = tree.all_children(id).into_iter().rev().collect();
    while let Some(n) = stack.pop() {
        result.push(n);
        stack.extend(tree.all_children(n).into_iter().rev());
    }
    result
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn identifiers_follow_identifier_create() {
        let mut tree = SyntaxTree::default();
        let plain = new_identifier(&mut tree, &utf16("_Button1"));
        assert_eq!(identifier_name(&tree, plain), utf16("_Button1"));
        assert_eq!(
            tree.node(plain).scalars[identifier::scalar::IS_VERBATIM],
            Scalar::Bool(false)
        );
        let verbatim = new_identifier(&mut tree, &utf16("@class"));
        assert_eq!(identifier_name(&tree, verbatim), utf16("class"));
        assert_eq!(
            tree.node(verbatim).scalars[identifier::scalar::IS_VERBATIM],
            Scalar::Bool(true)
        );
    }

    #[test]
    fn descendants_are_in_pre_order() {
        let mut tree = SyntaxTree::default();
        let statement = tree.new_node(kind::EXPRESSION_STATEMENT);
        let assignment = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        let left = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let name = new_identifier(&mut tree, &utf16("a"));
        let right = tree.new_node(kind::THIS_REFERENCE_EXPRESSION);
        tree.set_child(
            left,
            identifier_expression::slot::IDENTIFIER_TOKEN,
            Some(name),
        );
        tree.set_child(assignment, assignment_expression::slot::LEFT, Some(left));
        tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(right));
        tree.set_child(
            statement,
            expression_statement::slot::EXPRESSION,
            Some(assignment),
        );
        assert_eq!(
            descendants(&tree, statement),
            vec![assignment, left, name, right]
        );
        assert_eq!(identifier_expression_name(&tree, left), Some(utf16("a")));
    }
}
