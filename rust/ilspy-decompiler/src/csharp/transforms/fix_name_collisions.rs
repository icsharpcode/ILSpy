//! `FixNameCollisions`: a private field whose name collides with another member of its type
//! (typically a compiler-generated event field not detected as a pattern) is renamed, and so is
//! every reference to it.
//!
//! What the transform knows about symbols it asks the host. Its query kind (of 224..=237):
//! - 224 `QUERY_PRIVATE_FIELD` (symbol id, see `syntax_oracle::QUERY_NODE_SYMBOL`): one byte,
//!   whether the symbol is an `IField` of `Accessibility.Private`.

use std::collections::{HashMap, HashSet};

use super::rename_visual_basic_anonymous_types::symbol;
use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::SyntaxOracle;
use crate::csharp::syntax_schema::{
    field_declaration, identifier, identifier_expression, kind, member_reference_expression,
    member_type, operator_declaration, primitive_type, simple_type, type_declaration,
    variable_initializer,
};
use crate::typesystem::types::TypeSystem;

pub(crate) const QUERY_PRIVATE_FIELD: u8 = 224;

/// `OperatorDeclaration.GetName`, by `OperatorType` value.
const OPERATOR_NAMES: &[&str] = &[
    "op_LogicalNot",
    "op_OnesComplement",
    "op_Increment",
    "op_CheckedIncrement",
    "op_Decrement",
    "op_CheckedDecrement",
    "op_True",
    "op_False",
    "op_UnaryPlus",
    "op_UnaryNegation",
    "op_CheckedUnaryNegation",
    "op_Addition",
    "op_CheckedAddition",
    "op_Subtraction",
    "op_CheckedSubtraction",
    "op_Multiply",
    "op_CheckedMultiply",
    "op_Division",
    "op_CheckedDivision",
    "op_Modulus",
    "op_BitwiseAnd",
    "op_BitwiseOr",
    "op_ExclusiveOr",
    "op_LeftShift",
    "op_RightShift",
    "op_UnsignedRightShift",
    "op_Equality",
    "op_Inequality",
    "op_GreaterThan",
    "op_LessThan",
    "op_GreaterThanOrEqual",
    "op_LessThanOrEqual",
    "op_Implicit",
    "op_Explicit",
    "op_CheckedExplicit",
];

type Name = Vec<u16>;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    if let Some(root) = tree.root {
        fix(tree, oracle, root);
    }
    Ok(())
}

fn fix(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, root: SyntaxId) {
    let mut renamed: HashMap<u32, Name> = HashMap::new();
    for type_decl in tree.descendants_and_self(root) {
        if tree.kind(type_decl) != kind::TYPE_DECLARATION {
            continue;
        }
        let members = tree
            .children(type_decl, type_declaration::slot::MEMBERS)
            .to_vec();
        let member_names: HashSet<Name> = members
            .iter()
            .map(|&m| {
                let name = entity_name(tree, m);
                match private_implementation_type(tree, m) {
                    None => name,
                    Some(t) => {
                        let mut full = type_text(tree, t);
                        full.push(u16::from(b'.'));
                        full.extend(name);
                        full
                    }
                }
            })
            .collect();
        // member_names does not include fields or non-custom events because those don't have a
        // single name, but a list of VariableInitializers.
        for field in members {
            if tree.kind(field) != kind::FIELD_DECLARATION {
                continue;
            }
            let &[variable] = tree.children(field, field_declaration::slot::VARIABLES) else {
                continue;
            };
            let old_name = tree
                .child(variable, variable_initializer::slot::NAME_TOKEN)
                .map(|i| identifier_name(tree, i))
                .unwrap_or_default();
            if !member_names.contains(&old_name) {
                continue;
            }
            let Some(symbol) = symbol(tree, oracle, field) else {
                continue;
            };
            if !is_private_field(oracle, symbol) {
                continue;
            }
            let new_name = pick_new_name(&member_names, &old_name);
            let token = create_identifier(tree, &new_name);
            tree.set_child(
                variable,
                variable_initializer::slot::NAME_TOKEN,
                Some(token),
            );
            renamed.insert(symbol, new_name);
        }
    }
    if renamed.is_empty() {
        return;
    }

    for node in tree.descendants_and_self(root) {
        let slot = match tree.kind(node) {
            kind::IDENTIFIER_EXPRESSION => identifier_expression::slot::IDENTIFIER_TOKEN,
            kind::MEMBER_REFERENCE_EXPRESSION => {
                member_reference_expression::slot::MEMBER_NAME_TOKEN
            }
            _ => continue,
        };
        let Some(new_name) = symbol(tree, oracle, node).and_then(|s| renamed.get(&s)) else {
            continue;
        };
        // An IdentifierExpression / MemberReferenceExpression always carries its name identifier.
        if let Some(token) = tree.child(node, slot) {
            tree.node_mut(token).scalars[identifier::scalar::NAME] = Scalar::Str(new_name.clone());
        }
    }
}

fn is_private_field(oracle: &dyn SyntaxOracle, symbol: u32) -> bool {
    let mut query = vec![QUERY_PRIVATE_FIELD];
    uvar(&mut query, u64::from(symbol));
    oracle.ask(&query).is_some_and(|b| b.first() == Some(&1))
}

fn pick_new_name(member_names: &HashSet<Name>, name: &[u16]) -> Name {
    let prefixed: Name = "m_".encode_utf16().chain(name.iter().copied()).collect();
    if !member_names.contains(&prefixed) {
        return prefixed;
    }
    (2u64..)
        .map(|num| {
            let mut n = name.to_vec();
            n.extend(num.to_string().encode_utf16());
            n
        })
        .find(|n| !member_names.contains(n))
        .expect("an unbounded range")
}

fn identifier_name(tree: &SyntaxTree, id: SyntaxId) -> Name {
    match &tree.node(id).scalars[identifier::scalar::NAME] {
        Scalar::Str(s) => s.clone(),
        _ => Vec::new(),
    }
}

/// `Identifier.Create(name)`: a leading '@' makes a verbatim identifier.
fn create_identifier(tree: &mut SyntaxTree, name: &[u16]) -> SyntaxId {
    let id = tree.new_node(kind::IDENTIFIER);
    let (name, verbatim) = match name.split_first() {
        Some((&c, rest)) if c == u16::from(b'@') => (rest, true),
        _ => (name, false),
    };
    let scalars = &mut tree.node_mut(id).scalars;
    scalars[identifier::scalar::NAME] = Scalar::Str(name.to_vec());
    scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
    id
}

/// The first child of `id` in the slot of kind `slot_kind`.
fn child_of_slot_kind(tree: &SyntaxTree, id: SyntaxId, slot_kind: &str) -> Option<SyntaxId> {
    let index = tree
        .node(id)
        .schema()
        .slots
        .iter()
        .position(|s| s.kind == slot_kind && !s.collection)?;
    tree.child(id, index)
}

/// `EntityDeclaration.Name` of a type member.
fn entity_name(tree: &SyntaxTree, id: SyntaxId) -> Name {
    match tree.kind(id) {
        kind::FIELD_DECLARATION | kind::EVENT_DECLARATION => Vec::new(),
        kind::INDEXER_DECLARATION => "Item".encode_utf16().collect(),
        kind::OPERATOR_DECLARATION => {
            let op = tree.node(id).scalars[operator_declaration::scalar::OPERATOR_TYPE]
                .as_int()
                .unwrap_or(-1);
            usize::try_from(op)
                .ok()
                .and_then(|i| OPERATOR_NAMES.get(i))
                .map(|n| n.encode_utf16().collect())
                .unwrap_or_default()
        }
        _ => child_of_slot_kind(tree, id, "Identifier")
            .map(|i| identifier_name(tree, i))
            .unwrap_or_default(),
    }
}

fn private_implementation_type(tree: &SyntaxTree, id: SyntaxId) -> Option<SyntaxId> {
    child_of_slot_kind(tree, id, "PrivateImplementationType")
}

/// `AstType.ToString()` for the type shapes an explicit interface implementation names.
/// shortcut: other shapes, verbatim and keyword escapes are not written as the output visitor
/// would; the text only meets field names after a '.', so write them out if a field name ever
/// can contain one.
fn type_text(tree: &SyntaxTree, id: SyntaxId) -> Name {
    let mut out = Vec::new();
    let push = |s: &str, out: &mut Name| out.extend(s.encode_utf16());
    let arguments = |slot: usize, out: &mut Name| {
        let args = tree.children(id, slot);
        if !args.is_empty() {
            out.push(u16::from(b'<'));
            for (i, &a) in args.iter().enumerate() {
                if i > 0 {
                    out.extend(", ".encode_utf16());
                }
                out.extend(type_text(tree, a));
            }
            out.push(u16::from(b'>'));
        }
    };
    match tree.kind(id) {
        kind::SIMPLE_TYPE => {
            if let Some(i) = tree.child(id, simple_type::slot::IDENTIFIER_TOKEN) {
                out.extend(identifier_name(tree, i));
            }
            arguments(simple_type::slot::TYPE_ARGUMENTS, &mut out);
        }
        kind::MEMBER_TYPE => {
            if let Some(t) = tree.child(id, member_type::slot::TARGET) {
                out.extend(type_text(tree, t));
            }
            let double_colon = tree.node(id).scalars[member_type::scalar::IS_DOUBLE_COLON]
                .as_bool()
                .unwrap_or(false);
            push(if double_colon { "::" } else { "." }, &mut out);
            if let Some(i) = tree.child(id, member_type::slot::MEMBER_NAME_TOKEN) {
                out.extend(identifier_name(tree, i));
            }
            arguments(member_type::slot::TYPE_ARGUMENTS, &mut out);
        }
        kind::PRIMITIVE_TYPE => {
            if let Scalar::Str(k) = &tree.node(id).scalars[primitive_type::scalar::KEYWORD] {
                out.extend_from_slice(k);
            }
        }
        _ => push(tree.node(id).schema().name, &mut out),
    }
    out
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_oracle::QUERY_NODE_SYMBOL;
    use crate::csharp::syntax_schema::property_declaration;

    fn u(s: &str) -> Name {
        s.encode_utf16().collect()
    }

    /// Handle `h` resolves to symbol `h`; symbols below 10 are private fields.
    struct Oracle;

    impl SyntaxOracle for Oracle {
        fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
            let mut out = Vec::new();
            match query[0] {
                QUERY_NODE_SYMBOL => uvar(&mut out, u64::from(query[1]) + 1),
                QUERY_PRIVATE_FIELD => out.push(u8::from(query[1] < 10)),
                _ => return None,
            }
            Some(out)
        }
    }

    fn ident(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
        let i = tree.new_node(kind::IDENTIFIER);
        tree.node_mut(i).scalars[identifier::scalar::NAME] = Scalar::Str(u(name));
        i
    }

    fn member(tree: &mut SyntaxTree, t: SyntaxId, member: SyntaxId) {
        let n = tree.children(t, type_declaration::slot::MEMBERS).len();
        tree.insert_child(t, type_declaration::slot::MEMBERS, n, member);
    }

    fn field(tree: &mut SyntaxTree, t: SyntaxId, symbol: u32, name: &str) -> SyntaxId {
        let f = tree.new_node(kind::FIELD_DECLARATION);
        tree.node_mut(f).annotations = vec![symbol];
        let v = tree.new_node(kind::VARIABLE_INITIALIZER);
        let i = ident(tree, name);
        tree.set_child(v, variable_initializer::slot::NAME_TOKEN, Some(i));
        tree.insert_child(f, field_declaration::slot::VARIABLES, 0, v);
        member(tree, t, f);
        v
    }

    fn property(tree: &mut SyntaxTree, t: SyntaxId, name: &str) {
        let p = tree.new_node(kind::PROPERTY_DECLARATION);
        let i = ident(tree, name);
        tree.set_child(p, property_declaration::slot::NAME_TOKEN, Some(i));
        member(tree, t, p);
    }

    fn variable_name(tree: &SyntaxTree, v: SyntaxId) -> String {
        let i = tree
            .child(v, variable_initializer::slot::NAME_TOKEN)
            .unwrap();
        String::from_utf16_lossy(&identifier_name(tree, i))
    }

    #[test]
    fn colliding_private_fields_and_their_references_are_renamed() {
        let mut tree = SyntaxTree::default();
        let t = tree.new_node(kind::TYPE_DECLARATION);
        property(&mut tree, t, "A");
        property(&mut tree, t, "B");
        property(&mut tree, t, "m_B");
        property(&mut tree, t, "B2");
        property(&mut tree, t, "C");
        let a = field(&mut tree, t, 1, "A");
        let b = field(&mut tree, t, 2, "B");
        let c = field(&mut tree, t, 11, "C");
        let d = field(&mut tree, t, 3, "D");
        let e = tree.new_node(kind::EVENT_DECLARATION);
        member(&mut tree, t, e);
        let reference = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        tree.node_mut(reference).annotations = vec![1];
        let r = ident(&mut tree, "A");
        tree.set_child(
            reference,
            identifier_expression::slot::IDENTIFIER_TOKEN,
            Some(r),
        );
        let m = tree.new_node(kind::METHOD_DECLARATION);
        tree.set_child(
            m,
            crate::csharp::syntax_schema::method_declaration::slot::BODY,
            Some(reference),
        );
        member(&mut tree, t, m);
        tree.root = Some(t);

        fix(&mut tree, &Oracle, t);
        assert_eq!(variable_name(&tree, a), "m_A");
        assert_eq!(variable_name(&tree, b), "B3");
        assert_eq!(variable_name(&tree, c), "C");
        assert_eq!(variable_name(&tree, d), "D");
        assert_eq!(String::from_utf16_lossy(&identifier_name(&tree, r)), "m_A");
    }

    #[test]
    fn explicit_implementations_are_qualified() {
        let mut tree = SyntaxTree::default();
        let simple = tree.new_node(kind::SIMPLE_TYPE);
        let i = ident(&mut tree, "IList");
        tree.set_child(simple, simple_type::slot::IDENTIFIER_TOKEN, Some(i));
        let arg = tree.new_node(kind::PRIMITIVE_TYPE);
        tree.node_mut(arg).scalars[primitive_type::scalar::KEYWORD] = Scalar::Str(u("int"));
        tree.insert_child(simple, simple_type::slot::TYPE_ARGUMENTS, 0, arg);
        let member = tree.new_node(kind::MEMBER_TYPE);
        tree.set_child(member, member_type::slot::TARGET, Some(simple));
        let n = ident(&mut tree, "Inner");
        tree.set_child(member, member_type::slot::MEMBER_NAME_TOKEN, Some(n));
        assert_eq!(type_text(&tree, member), u("IList<int>.Inner"));

        let op = tree.new_node(kind::OPERATOR_DECLARATION);
        tree.node_mut(op).scalars[operator_declaration::scalar::OPERATOR_TYPE] = Scalar::Int(11);
        assert_eq!(entity_name(&tree, op), u("op_Addition"));
        assert_eq!(
            OPERATOR_NAMES.len(),
            35,
            "one name per OperatorType value up to CheckedExplicit"
        );
    }
}
