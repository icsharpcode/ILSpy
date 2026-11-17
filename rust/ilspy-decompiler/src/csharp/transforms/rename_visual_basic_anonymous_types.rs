//! `RenameVisualBasicAnonymousTypes`: the VB anonymous types that are declared rather than
//! written as C# anonymous types (a settable property makes them so) get legal C# names: every
//! identifier of such a type or of one of its members has its '$' replaced by '_', and each
//! declaration gets a comment explaining why it is there.
//!
//! What the transform knows about symbols it asks the host. Its query kind (of 210..=223):
//! - 210 `QUERY_VB_ANONYMOUS_ENTITY` (symbol id, see `syntax_oracle::QUERY_NODE_SYMBOL`): no answer
//!   where the symbol is no `IEntity`; else a flags byte (`ENTITY_TYPE_DEFINITION`,
//!   `ENTITY_DECLARED_ANONYMOUS`) and the entity name as UTF-16 code units.

use crate::csharp::syntax::{Scalar, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{self as q, SyntaxOracle};
use crate::csharp::syntax_schema::{comment, identifier, kind};
use crate::typesystem::types::TypeSystem;

/// `CommentType.SingleLine`.
pub(crate) const COMMENT_SINGLE_LINE: i64 = 0;

const DOLLAR: u16 = b'$' as u16;

pub(crate) const QUERY_VB_ANONYMOUS_ENTITY: u8 = 210;
/// The entity is an `ITypeDefinition`.
const ENTITY_TYPE_DEFINITION: u8 = 1;
/// `(entity as ITypeDefinition ?? entity.DeclaringTypeDefinition).IsAnonymousTypeDeclaredAsNamedType()`.
const ENTITY_DECLARED_ANONYMOUS: u8 = 2;
const BOTH: u8 = ENTITY_TYPE_DEFINITION | ENTITY_DECLARED_ANONYMOUS;

/// The answer to `QUERY_VB_ANONYMOUS_ENTITY`.
struct Entity {
    flags: u8,
    name: Vec<u16>,
}

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    if let Some(root) = tree.root {
        rename(tree, oracle, root);
    }
    Ok(())
}

fn rename(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, root: SyntaxId) {
    let nodes = tree.descendants_and_self(root);
    for &id in &nodes {
        if tree.kind(id) != kind::IDENTIFIER {
            continue;
        }
        let Scalar::Str(name) = &tree.node(id).scalars[identifier::scalar::NAME] else {
            continue;
        };
        if !name.contains(&DOLLAR) {
            continue;
        }
        // A field declaration carries its symbol on the FieldDeclaration, not on the
        // VariableInitializer holding the name.
        let parent = tree.node(id).parent;
        let entity = parent.and_then(|p| entity(tree, oracle, p)).or_else(|| {
            parent
                .and_then(|p| tree.node(p).parent)
                .and_then(|gp| entity(tree, oracle, gp))
        });
        let Some(entity) = entity else {
            continue;
        };
        if entity.name != *name || entity.flags & ENTITY_DECLARED_ANONYMOUS == 0 {
            continue;
        }
        let renamed = name
            .iter()
            .map(|&c| if c == DOLLAR { u16::from(b'_') } else { c })
            .collect();
        tree.node_mut(id).scalars[identifier::scalar::NAME] = Scalar::Str(renamed);
    }

    for &id in &nodes {
        if tree.kind(id) != kind::TYPE_DECLARATION {
            continue;
        }
        if !entity(tree, oracle, id).is_some_and(|e| e.flags & BOTH == BOTH) {
            continue;
        }
        for text in [
            " A VB anonymous type. Its properties are settable and only those declared 'Key'",
            " take part in Equals and GetHashCode, so it cannot be written as a C# anonymous",
            " type and is declared here instead.",
        ] {
            let c = new_comment(tree, COMMENT_SINGLE_LINE, text.encode_utf16().collect());
            add_leading_trivia(tree, id, c);
        }
    }
}

/// The first `ResolveResult` annotation of `id` and the symbol id `AstNode.GetSymbol` reads from
/// it (`None` for none); `None` where the node has no resolve result.
pub(crate) fn resolve_result(
    tree: &SyntaxTree,
    oracle: &dyn SyntaxOracle,
    id: SyntaxId,
) -> Option<(u32, Option<u32>)> {
    tree.node(id)
        .annotations
        .iter()
        .find_map(|&h| q::node_symbol(oracle, h).map(|s| (h, s)))
}

/// `AstNode.GetSymbol` of `id`, as a symbol id.
pub(crate) fn symbol(tree: &SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> Option<u32> {
    resolve_result(tree, oracle, id).and_then(|(_, s)| s)
}

/// `node.GetSymbol() as IEntity`, as far as the transform needs it.
fn entity(tree: &SyntaxTree, oracle: &dyn SyntaxOracle, id: SyntaxId) -> Option<Entity> {
    let mut query = vec![QUERY_VB_ANONYMOUS_ENTITY];
    uvar(&mut query, u64::from(symbol(tree, oracle, id)?));
    let b = oracle.ask(&query)?;
    let (&flags, name) = b.split_first()?;
    Some(Entity {
        flags,
        name: utf16(name),
    })
}

/// Little-endian UTF-16 code units.
pub(crate) fn utf16(bytes: &[u8]) -> Vec<u16> {
    bytes
        .as_chunks::<2>()
        .0
        .iter()
        .map(|c| u16::from_le_bytes(*c))
        .collect()
}

/// `new Comment(content, type)`, without parent.
pub(crate) fn new_comment(tree: &mut SyntaxTree, comment_type: i64, content: Vec<u16>) -> SyntaxId {
    let c = tree.new_node(kind::COMMENT);
    let scalars = &mut tree.node_mut(c).scalars;
    scalars[comment::scalar::COMMENT_TYPE] = Scalar::Int(comment_type);
    scalars[comment::scalar::CONTENT] = Scalar::Str(content);
    c
}

/// `node.AddLeadingTrivia(trivia)`: appends the parentless `trivia`.
pub(crate) fn add_leading_trivia(tree: &mut SyntaxTree, node: SyntaxId, trivia: SyntaxId) {
    tree.node_mut(trivia).parent = Some(node);
    tree.node_mut(node).leading_trivia.push(trivia);
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax::uvar;
    use crate::csharp::syntax_oracle::QUERY_NODE_SYMBOL;
    use crate::csharp::syntax_schema::{field_declaration, type_declaration, variable_initializer};

    /// Handle `h` resolves to symbol `h`; symbol 0 is the anonymous type `VB$AnonymousType_0`,
    /// symbol 1 its field `$x`, symbol 2 an unrelated field `$y`.
    struct Oracle;

    impl SyntaxOracle for Oracle {
        fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
            let mut out = Vec::new();
            match query[0] {
                QUERY_NODE_SYMBOL => uvar(&mut out, u64::from(query[1]) + 1),
                QUERY_VB_ANONYMOUS_ENTITY => {
                    let (flags, name) = match query[1] {
                        0 => (3, "VB$AnonymousType_0"),
                        1 => (2, "$x"),
                        2 => (0, "$y"),
                        _ => return None,
                    };
                    out.push(flags);
                    out.extend(name.encode_utf16().flat_map(u16::to_le_bytes));
                }
                _ => return None,
            }
            Some(out)
        }
    }

    fn ident(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
        let i = tree.new_node(kind::IDENTIFIER);
        tree.node_mut(i).scalars[identifier::scalar::NAME] =
            Scalar::Str(name.encode_utf16().collect());
        i
    }

    fn field(tree: &mut SyntaxTree, symbol: u32, name: &str) -> SyntaxId {
        let f = tree.new_node(kind::FIELD_DECLARATION);
        tree.node_mut(f).annotations = vec![symbol];
        let v = tree.new_node(kind::VARIABLE_INITIALIZER);
        let i = ident(tree, name);
        tree.set_child(v, variable_initializer::slot::NAME_TOKEN, Some(i));
        tree.insert_child(f, field_declaration::slot::VARIABLES, 0, v);
        i
    }

    fn name(tree: &SyntaxTree, id: SyntaxId) -> String {
        tree.node(id).scalars[identifier::scalar::NAME]
            .as_str()
            .unwrap()
    }

    #[test]
    fn declared_anonymous_types_are_renamed_and_commented() {
        let mut tree = SyntaxTree::default();
        let t = tree.new_node(kind::TYPE_DECLARATION);
        tree.node_mut(t).annotations = vec![0];
        let type_name = ident(&mut tree, "VB$AnonymousType_0");
        tree.set_child(t, type_declaration::slot::NAME_TOKEN, Some(type_name));
        let x = field(&mut tree, 1, "$x");
        let y = field(&mut tree, 2, "$y");
        for (i, n) in [x, y].into_iter().enumerate() {
            let f = tree.node(tree.node(n).parent.unwrap()).parent.unwrap();
            tree.insert_child(t, type_declaration::slot::MEMBERS, i, f);
        }
        tree.root = Some(t);
        rename(&mut tree, &Oracle, t);
        assert_eq!(name(&tree, type_name), "VB_AnonymousType_0");
        assert_eq!(name(&tree, x), "_x");
        assert_eq!(name(&tree, y), "$y");
        let trivia = &tree.node(t).leading_trivia;
        assert_eq!(trivia.len(), 3);
        assert_eq!(tree.node(trivia[0]).parent, Some(t));
    }
}
