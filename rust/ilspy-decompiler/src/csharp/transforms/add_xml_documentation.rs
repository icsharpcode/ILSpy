//! `AddXmlDocumentationTransform`: the XML documentation of each declared entity becomes `///`
//! comments in front of its declaration, one per line, with the common indentation of the
//! first line removed and trailing blank lines dropped.
//!
//! The documentation provider stays on the host. The transform's query kinds (of 238..=251):
//! - 238 `QUERY_XML_DOCUMENTATION_ENABLED` (no arguments): one byte, whether
//!   `ShowXmlDocumentation` is set and the decompile run has a documentation provider.
//! - 239 `QUERY_XML_DOCUMENTATION` (symbol id, see `syntax_oracle::QUERY_NODE_SYMBOL`): a tag
//!   byte, then UTF-16 code units: 0 where the symbol is no `IEntity` or has no documentation
//!   (a parameterized property's first accessor falls back to the property's), 1 and the
//!   documentation, 2 and the text of the comment the transform writes for the `XmlException`
//!   the provider threw.

use super::introduce_unsafe_modifier::is_entity_declaration;
use super::rename_visual_basic_anonymous_types::{
    add_leading_trivia, new_comment, resolve_result, utf16,
};
use crate::csharp::syntax::{SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::SyntaxOracle;
use crate::csharp::syntax_schema::kind;
use crate::typesystem::types::TypeSystem;

pub(crate) const QUERY_XML_DOCUMENTATION_ENABLED: u8 = 238;
pub(crate) const QUERY_XML_DOCUMENTATION: u8 = 239;

/// `CommentType.Documentation`.
const COMMENT_DOCUMENTATION: i64 = 2;

const CR: u16 = b'\r' as u16;
const LF: u16 = b'\n' as u16;

/// `IAstTransform.Run`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let enabled = oracle
        .ask(&[QUERY_XML_DOCUMENTATION_ENABLED])
        .is_some_and(|b| b.first() == Some(&1));
    if let (true, Some(root)) = (enabled, tree.root) {
        add_documentation(tree, oracle, root);
    }
    Ok(())
}

fn add_documentation(tree: &mut SyntaxTree, oracle: &dyn SyntaxOracle, root: SyntaxId) {
    for entity in tree.descendants_and_self(root) {
        let k = tree.kind(entity);
        if !is_entity_declaration(k) && k != kind::ACCESSOR {
            continue;
        }
        let Some((rr, Some(symbol))) = resolve_result(tree, oracle, entity) else {
            continue;
        };
        let mut query = vec![QUERY_XML_DOCUMENTATION];
        uvar(&mut query, u64::from(symbol));
        let Some(answer) = oracle.ask(&query) else {
            continue;
        };
        match answer.split_first() {
            Some((1, doc)) => insert_documentation(tree, entity, rr, &utf16(doc)),
            Some((2, message)) => {
                for line in utf16(message)
                    .split(|&c| c == CR || c == LF)
                    .filter(|l| !l.is_empty())
                {
                    let c = new_comment(tree, COMMENT_DOCUMENTATION, line.to_vec());
                    add_leading_trivia(tree, root, c);
                }
                return;
            }
            _ => {}
        }
    }
}

/// The lines `StringReader.ReadLine` reads from `text`.
fn read_lines(text: &[u16]) -> Vec<&[u16]> {
    let mut lines = Vec::new();
    let mut i = 0;
    while i < text.len() {
        let start = i;
        while i < text.len() && text[i] != CR && text[i] != LF {
            i += 1;
        }
        lines.push(&text[start..i]);
        if i < text.len() {
            i += if text[i] == CR && text.get(i + 1) == Some(&LF) {
                2
            } else {
                1
            };
        }
    }
    lines
}

/// `char.IsWhiteSpace` of a UTF-16 code unit.
fn is_white_space(c: u16) -> bool {
    char::from_u32(u32::from(c)).is_some_and(char::is_whitespace)
}

fn is_null_or_white_space(line: &[u16]) -> bool {
    line.iter().all(|&c| is_white_space(c))
}

/// `InsertXmlDocumentation`: each comment carries `node`'s resolve result `rr`.
fn insert_documentation(tree: &mut SyntaxTree, node: SyntaxId, rr: u32, doc: &[u16]) {
    let lines = read_lines(doc);
    let Some(first) = lines.iter().position(|l| !is_null_or_white_space(l)) else {
        return;
    };
    let first_line = lines[first];
    let indentation = &first_line[..first_line
        .iter()
        .position(|&c| !is_white_space(c))
        .unwrap_or(first_line.len())];
    let mut skipped_white_space_lines = 0;
    // Copy all lines from input to output, except for empty lines at the end.
    for &line in &lines[first..] {
        if is_null_or_white_space(line) {
            skipped_white_space_lines += 1;
            continue;
        }
        for _ in 0..skipped_white_space_lines {
            let empty = new_comment(tree, COMMENT_DOCUMENTATION, Vec::new());
            tree.node_mut(empty).annotations.push(rr);
            add_leading_trivia(tree, node, empty);
        }
        skipped_white_space_lines = 0;
        let line = line.strip_prefix(indentation).unwrap_or(line);
        let mut content = vec![u16::from(b' ')];
        content.extend_from_slice(line);
        let comment = new_comment(tree, COMMENT_DOCUMENTATION, content);
        tree.node_mut(comment).annotations.push(rr);
        add_leading_trivia(tree, node, comment);
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax::Scalar;
    use crate::csharp::syntax_schema::comment;

    fn u(s: &str) -> Vec<u16> {
        s.encode_utf16().collect()
    }

    fn contents(tree: &SyntaxTree, node: SyntaxId) -> Vec<String> {
        tree.node(node)
            .leading_trivia
            .iter()
            .map(|&c| match &tree.node(c).scalars[comment::scalar::CONTENT] {
                Scalar::Str(s) => String::from_utf16_lossy(s),
                _ => panic!("a comment without content"),
            })
            .collect()
    }

    #[test]
    fn lines_split_like_a_string_reader() {
        let text = u("a\r\nb\rc\n\nd\n");
        let lines: Vec<String> = read_lines(&text)
            .into_iter()
            .map(String::from_utf16_lossy)
            .collect();
        assert_eq!(lines, ["a", "b", "c", "", "d"]);
        assert!(read_lines(&[]).is_empty());
        assert_eq!(read_lines(&u("\n")).len(), 1);
    }

    #[test]
    fn documentation_loses_its_indentation_and_trailing_blank_lines() {
        let mut tree = SyntaxTree::default();
        let method = tree.new_node(kind::METHOD_DECLARATION);
        let doc = u("\n    <summary>\n      Text\n\n\u{3000}\n  x\n    </summary>\n  \n\n");
        insert_documentation(&mut tree, method, 7, &doc);
        assert_eq!(
            contents(&tree, method),
            [" <summary>", "   Text", "", "", "   x", " </summary>"]
        );
        let first = tree.node(method).leading_trivia[0];
        assert_eq!(tree.node(first).annotations, vec![7]);
        assert_eq!(tree.node(first).parent, Some(method));
    }
}
