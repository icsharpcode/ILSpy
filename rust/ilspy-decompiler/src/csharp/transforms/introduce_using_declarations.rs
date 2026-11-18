//! `IntroduceUsingDeclarations` (`CSharp/Transforms/IntroduceUsingDeclarations.cs`): collects the
//! namespaces the tree refers to, inserts a `using` declaration for each, and rewrites every
//! type reference into the form the resolver needs at its place (`FullyQualifyAmbiguousTypeNames`).
//!
//! The engine walks the tree; the resolver stays on the host
//! (`SyntaxRoundTrip.IntroduceUsingDeclarations.cs`). A resolver scope is a host-side resolver
//! plus `TypeSystemAstBuilder`, named by a scope id; scope 0 is the root scope, and the engine
//! derives a new scope at each namespace, type and member declaration. Query kinds (a kind byte,
//! then unsigned LEB128 integers and texts as a length and UTF-8):
//!
//! - 140 `QUERY_CURRENT_NAMESPACE`: the namespace of the context's current type definition, empty
//!   for none, as a text.
//! - 141 `QUERY_IMPORT` (handle, rule byte): what an annotation adds to the imported namespaces.
//!   No answer where the object is not of the class the rule reads; else 0 for nothing, or 1 and
//!   the namespace (a text). Rules: 1 `CSharpInvocationResolveResult` of an extension method
//!   invocation (any `ResolveResult` is of the class), 2 `ForeachAnnotation` with an extension
//!   `GetEnumerator` call, 3 `MatchInstruction` with a method, 4 `CallInstruction` of an extension
//!   method named `Add`. The namespace is that of the method's declaring type.
//! - 142 `QUERY_USING_SCOPE` (count, namespaces as texts): sorts the namespaces as the managed
//!   transform does (culture-sensitive, on the host), builds the `UsingScope` of those it resolves
//!   and the root scope 0 from it; answers the count and the sorted namespaces.
//! - 143 `QUERY_SCOPE` (parent scope id, frame byte, count, items): a scope derived from the
//!   parent; frame 0 a namespace declaration (items: the identifiers as texts), 1 a type
//!   declaration and 2 a member declaration (items: the node's annotation handles). Answers the
//!   new scope id.
//! - 144 `QUERY_CONVERT` (scope id, handle, `NameLookupMode` byte, attribute byte): for a
//!   `TypeResolveResult`, its type as the scope's builder writes it (`ConvertAttributeType` where
//!   the attribute byte is 1), as a subtree whose handles are annotations of kind
//!   `ANNOTATION_MINTED`; no answer for any other object.
//!
//! The engine creates annotation kind 140, the `UsingScope` of the last `QUERY_USING_SCOPE`
//! (empty payload), and kind 98 (`DeclareVariables`' minted objects, payload the index).

use std::collections::{HashMap, HashSet};

use super::introduce_unsafe_modifier::new_identifier;
use crate::csharp::syntax::{SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{SyntaxOracle, setting, type_resolve_result_name};
use crate::csharp::syntax_schema::{
    array_initializer_expression, kind, member_type, simple_type, type_declaration,
};
use crate::typesystem::types::TypeSystem;

const QUERY_CURRENT_NAMESPACE: u8 = 140;
const QUERY_IMPORT: u8 = 141;
const QUERY_USING_SCOPE: u8 = 142;
const QUERY_SCOPE: u8 = 143;
const QUERY_CONVERT: u8 = 144;

const ANNOTATION_USING_SCOPE: u8 = 140;
/// The host-built object of an index (`SyntaxRoundTrip.DeclareVariables.cs`).
const ANNOTATION_MINTED: u8 = 98;

const RULE_EXTENSION_INVOCATION: u8 = 1;
const RULE_FOREACH: u8 = 2;
const RULE_MATCH: u8 = 3;
const RULE_ADD_CALL: u8 = 4;

const FRAME_NAMESPACE: u8 = 0;
const FRAME_TYPE: u8 = 1;
const FRAME_MEMBER: u8 = 2;

// `NameLookupMode` values.
const LOOKUP_EXPRESSION: u8 = 0;
const LOOKUP_TYPE: u8 = 2;
const LOOKUP_TYPE_IN_USING_DECLARATION: u8 = 3;
const LOOKUP_BASE_TYPE_REFERENCE: u8 = 4;

/// The built-in AST transform `IntroduceUsingDeclarations`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let current = {
        let b = oracle
            .ask(&[QUERY_CURRENT_NAMESPACE])
            .ok_or("IntroduceUsingDeclarations needs the transform context")?;
        Reader(&b).text()?
    };
    let mut imports = FindRequiredImports {
        oracle,
        current_namespace: current,
        imported: HashSet::new(),
    };
    imports.visit(tree, root);

    let using_declarations = setting(oracle, "UsingDeclarations")
        .ok_or("IntroduceUsingDeclarations needs the transform context")?;
    let mut q = vec![QUERY_USING_SCOPE];
    let requested: Vec<String> = if using_declarations {
        imports.imported.into_iter().collect()
    } else {
        Vec::new()
    };
    uvar(&mut q, requested.len() as u64);
    for ns in &requested {
        push_text(&mut q, ns);
    }
    let b = oracle.ask(&q).ok_or("the host built no using scope")?;
    let mut r = Reader(&b);
    let count = r.uvar()?;
    let mut sorted = Vec::with_capacity(count as usize);
    for _ in 0..count {
        sorted.push(r.text()?);
    }

    if using_declarations {
        let members = tree
            .node(root)
            .schema()
            .slots
            .iter()
            .position(|s| s.kind == "Member" && s.collection)
            .ok_or("the root has no member slot for using declarations")?;
        // Inserted in reverse order, each at the start of the member list.
        for ns in &sorted {
            let import = namespace_type(tree, ns);
            let using = tree.new_node(kind::USING_DECLARATION);
            tree.set_child(
                using,
                crate::csharp::syntax_schema::using_declaration::slot::IMPORT,
                Some(import),
            );
            tree.insert_child(root, members, 0, using);
        }
    }

    let h = tree.new_annotation(ANNOTATION_USING_SCOPE, Vec::new());
    tree.node_mut(root).annotations.push(h);

    let mut qualify = Qualify {
        oracle,
        ignore_using_scope: !using_declarations,
        scope: 0,
        in_primary_constructor: false,
        minted: HashMap::new(),
    };
    qualify.visit(tree, root)
}

/// `new SimpleType(parts[0])` qualified by `MemberType`s for the other parts of `ns`.
fn namespace_type(tree: &mut SyntaxTree, ns: &str) -> SyntaxId {
    let mut parts = ns.split('.');
    let first = parts.next().unwrap_or_default();
    let mut ty = tree.new_node(kind::SIMPLE_TYPE);
    let ident = new_identifier(tree, first);
    tree.set_child(ty, simple_type::slot::IDENTIFIER_TOKEN, Some(ident));
    for part in parts {
        let member = tree.new_node(kind::MEMBER_TYPE);
        tree.set_child(member, member_type::slot::TARGET, Some(ty));
        let ident = new_identifier(tree, part);
        tree.set_child(member, member_type::slot::MEMBER_NAME_TOKEN, Some(ident));
        ty = member;
    }
    ty
}

/// The name of the identifier token in the single slot `slot` of `id`, empty for none.
fn token_name(tree: &SyntaxTree, id: SyntaxId, slot: usize) -> String {
    tree.child(id, slot)
        .and_then(|t| tree.node(t).scalars.first().and_then(|s| s.as_str()))
        .unwrap_or_default()
}

/// `NamespaceDeclaration.Identifiers`.
fn namespace_identifiers(tree: &SyntaxTree, ns: SyntaxId) -> Vec<String> {
    let mut result = Vec::new();
    let mut ty = tree.child(
        ns,
        crate::csharp::syntax_schema::namespace_declaration::slot::NAMESPACE_NAME,
    );
    while let Some(t) = ty.filter(|&t| tree.kind(t) == kind::MEMBER_TYPE) {
        result.push(token_name(tree, t, member_type::slot::MEMBER_NAME_TOKEN));
        ty = tree.child(t, member_type::slot::TARGET);
    }
    if let Some(t) = ty.filter(|&t| tree.kind(t) == kind::SIMPLE_TYPE) {
        result.push(token_name(tree, t, simple_type::slot::IDENTIFIER_TOKEN));
    }
    result.reverse();
    result
}

/// `NamespaceDeclaration.BuildQualifiedName`.
fn build_qualified_name(name1: &str, name2: &str) -> String {
    if name1.is_empty() {
        name2.to_owned()
    } else if name2.is_empty() {
        name1.to_owned()
    } else {
        format!("{name1}.{name2}")
    }
}

struct FindRequiredImports<'a> {
    oracle: &'a dyn SyntaxOracle,
    current_namespace: String,
    imported: HashSet<String>,
}

impl FindRequiredImports<'_> {
    fn is_parent_of_current_namespace(&self, ns: &str) -> bool {
        ns.is_empty()
            || self
                .current_namespace
                .strip_prefix(ns)
                .is_some_and(|rest| rest.is_empty() || rest.starts_with('.'))
    }

    fn add_imported_namespace(&mut self, ns: Option<String>) {
        if let Some(ns) = ns
            && !self.is_parent_of_current_namespace(&ns)
        {
            self.imported.insert(ns);
        }
    }

    /// The namespace `rule` takes from the first annotation of `id` of the class the rule reads
    /// (`Annotation<T>`).
    fn rule_namespace(&self, tree: &SyntaxTree, id: SyntaxId, rule: u8) -> Option<String> {
        for &h in &tree.node(id).annotations {
            let mut q = vec![QUERY_IMPORT];
            uvar(&mut q, u64::from(h));
            q.push(rule);
            if let Some(b) = self.oracle.ask(&q) {
                let mut r = Reader(&b);
                return match r.byte().ok()? {
                    0 => None,
                    _ => r.text().ok(),
                };
            }
        }
        None
    }

    fn visit(&mut self, tree: &SyntaxTree, id: SyntaxId) {
        match tree.kind(id) {
            kind::SIMPLE_TYPE => {
                let ns = tree
                    .node(id)
                    .annotations
                    .iter()
                    .find_map(|&h| type_resolve_result_name(self.oracle, h))
                    .map(|(ns, _)| ns);
                self.add_imported_namespace(ns);
            }
            kind::NAMESPACE_DECLARATION => {
                let old = self.current_namespace.clone();
                for ident in namespace_identifiers(tree, id) {
                    self.current_namespace = build_qualified_name(&self.current_namespace, &ident);
                }
                self.visit_children(tree, id);
                self.current_namespace = old;
                return;
            }
            kind::INVOCATION_EXPRESSION => {
                let ns = self.rule_namespace(tree, id, RULE_EXTENSION_INVOCATION);
                self.add_imported_namespace(ns);
            }
            kind::FOREACH_STATEMENT => {
                let ns = self.rule_namespace(tree, id, RULE_FOREACH);
                self.add_imported_namespace(ns);
            }
            kind::PARENTHESIZED_VARIABLE_DESIGNATION | kind::TUPLE_EXPRESSION => {
                let ns = self.rule_namespace(tree, id, RULE_MATCH);
                self.add_imported_namespace(ns);
            }
            kind::ARRAY_INITIALIZER_EXPRESSION => {
                for &item in tree.children(id, array_initializer_expression::slot::ELEMENTS) {
                    let ns = self.rule_namespace(tree, item, RULE_ADD_CALL);
                    self.add_imported_namespace(ns);
                }
            }
            _ => {}
        }
        self.visit_children(tree, id);
    }

    fn visit_children(&mut self, tree: &SyntaxTree, id: SyntaxId) {
        for c in tree.all_children(id) {
            self.visit(tree, c);
        }
    }
}

/// `FullyQualifyAmbiguousTypeNamesVisitor`.
struct Qualify<'a> {
    oracle: &'a dyn SyntaxOracle,
    ignore_using_scope: bool,
    /// The host's resolver scope of the current position.
    scope: u64,
    in_primary_constructor: bool,
    /// The annotation handle of each minted object index.
    minted: HashMap<u32, u32>,
}

impl Qualify<'_> {
    fn derive_scope(&self, frame: u8, items: &[u8], count: usize) -> Result<u64, String> {
        let mut q = vec![QUERY_SCOPE];
        uvar(&mut q, self.scope);
        q.push(frame);
        uvar(&mut q, count as u64);
        q.extend_from_slice(items);
        let b = self.oracle.ask(&q).ok_or("the host derived no scope")?;
        Reader(&b).uvar()
    }

    fn annotation_scope(&self, tree: &SyntaxTree, id: SyntaxId, frame: u8) -> Result<u64, String> {
        let annotations = &tree.node(id).annotations;
        let mut items = Vec::new();
        for &h in annotations {
            uvar(&mut items, u64::from(h));
        }
        self.derive_scope(frame, &items, annotations.len())
    }

    /// Visits the children of `id` in the scope `scope`.
    fn visit_in(&mut self, tree: &mut SyntaxTree, id: SyntaxId, scope: u64) -> Result<(), String> {
        let previous = std::mem::replace(&mut self.scope, scope);
        let result = self.visit_children(tree, id);
        self.scope = previous;
        result
    }

    fn visit(&mut self, tree: &mut SyntaxTree, id: SyntaxId) -> Result<(), String> {
        let k = tree.kind(id);
        match k {
            kind::NAMESPACE_DECLARATION if !self.ignore_using_scope => {
                let idents = namespace_identifiers(tree, id);
                let mut items = Vec::new();
                for ident in &idents {
                    push_text(&mut items, ident);
                }
                let scope = self.derive_scope(FRAME_NAMESPACE, &items, idents.len())?;
                self.visit_in(tree, id, scope)
            }
            kind::TYPE_DECLARATION if !self.ignore_using_scope => {
                if tree.node(id).scalars[type_declaration::scalar::HAS_PRIMARY_CONSTRUCTOR]
                    .as_bool()
                    == Some(true)
                {
                    self.in_primary_constructor = true;
                    let parameters = tree
                        .children(id, type_declaration::slot::PRIMARY_CONSTRUCTOR_PARAMETERS)
                        .to_vec();
                    let result = parameters.into_iter().try_for_each(|p| self.visit(tree, p));
                    self.in_primary_constructor = false;
                    result?;
                }
                let scope = self.annotation_scope(tree, id, FRAME_TYPE)?;
                self.visit_in(tree, id, scope)
            }
            kind::PARAMETER_DECLARATION => {
                // Primary constructor parameters are visited in the scope outside their type.
                let parent_is_type = tree
                    .node(id)
                    .parent
                    .is_some_and(|p| tree.kind(p) == kind::TYPE_DECLARATION);
                if self.in_primary_constructor || !parent_is_type {
                    self.visit_children(tree, id)?;
                }
                Ok(())
            }
            kind::METHOD_DECLARATION
            | kind::ACCESSOR
            | kind::CONSTRUCTOR_DECLARATION
            | kind::DESTRUCTOR_DECLARATION
            | kind::OPERATOR_DECLARATION
                if !self.ignore_using_scope =>
            {
                let scope = self.annotation_scope(tree, id, FRAME_MEMBER)?;
                self.visit_in(tree, id, scope)
            }
            kind::SIMPLE_TYPE => self.visit_simple_type(tree, id),
            _ => self.visit_children(tree, id),
        }
    }

    fn visit_children(&mut self, tree: &mut SyntaxTree, id: SyntaxId) -> Result<(), String> {
        for c in tree.all_children(id) {
            self.visit(tree, c)?;
        }
        Ok(())
    }

    fn visit_simple_type(&mut self, tree: &mut SyntaxTree, id: SyntaxId) -> Result<(), String> {
        let mode = name_lookup_mode(tree, id);
        let attribute = tree
            .node(id)
            .parent
            .is_some_and(|p| tree.kind(p) == kind::ATTRIBUTE);
        let annotations = tree.node(id).annotations.clone();
        // The first annotation the host converts is the node's `TypeResolveResult`.
        for h in annotations {
            let mut q = vec![QUERY_CONVERT];
            uvar(&mut q, self.scope);
            uvar(&mut q, u64::from(h));
            q.push(mode);
            q.push(u8::from(attribute));
            if let Some(b) = self.oracle.ask(&q) {
                let source = SyntaxTree::decode(&b, 0)?;
                let minted = &mut self.minted;
                let replacement = tree
                    .import(source, |t, index| {
                        *minted.entry(index).or_insert_with(|| {
                            let mut payload = Vec::new();
                            uvar(&mut payload, u64::from(index));
                            t.new_annotation(ANNOTATION_MINTED, payload)
                        })
                    })
                    .ok_or("the host converted a type to nothing")?;
                tree.replace(id, replacement);
                return Ok(());
            }
        }
        self.visit_children(tree, id)
    }
}

fn is_ast_type(k: u16) -> bool {
    matches!(
        k,
        kind::SIMPLE_TYPE
            | kind::MEMBER_TYPE
            | kind::PRIMITIVE_TYPE
            | kind::COMPOSED_TYPE
            | kind::TUPLE_AST_TYPE
            | kind::FUNCTION_POINTER_AST_TYPE
            | kind::INVOCATION_AST_TYPE
    )
}

/// `AstType.GetNameLookupMode`, then expression mode for a type under a `TypeReferenceExpression`.
fn name_lookup_mode(tree: &SyntaxTree, id: SyntaxId) -> u8 {
    let mut outermost = id;
    while let Some(p) = tree
        .node(outermost)
        .parent
        .filter(|&p| is_ast_type(tree.kind(p)))
    {
        outermost = p;
    }
    let parent = tree.node(outermost).parent;
    let parent_kind = parent.map(|p| tree.kind(p));
    let mut mode = LOOKUP_TYPE;
    if matches!(
        parent_kind,
        Some(kind::USING_DECLARATION | kind::USING_ALIAS_DECLARATION)
    ) {
        mode = LOOKUP_TYPE_IN_USING_DECLARATION;
    } else if let (Some(p), Some(slot)) = (parent, tree.parent_slot(outermost))
        && tree.node(p).schema().slots[slot].kind == "BaseType"
    {
        let grandparent_kind = tree.node(p).parent.map(|g| tree.kind(g));
        if parent_kind == Some(kind::TYPE_DECLARATION)
            || (parent_kind == Some(kind::CONSTRAINT)
                && grandparent_kind == Some(kind::TYPE_DECLARATION))
        {
            mode = LOOKUP_BASE_TYPE_REFERENCE;
        }
    }
    if mode == LOOKUP_TYPE && parent_kind == Some(kind::TYPE_REFERENCE_EXPRESSION) {
        mode = LOOKUP_EXPRESSION;
    }
    mode
}

// shortcut: texts cross as UTF-8, so a lone surrogate in a namespace name becomes U+FFFD on both
// sides; send UTF-16 if obfuscated namespace names ever need to survive.
fn push_text(q: &mut Vec<u8>, text: &str) {
    uvar(q, text.len() as u64);
    q.extend_from_slice(text.as_bytes());
}

struct Reader<'a>(&'a [u8]);

impl Reader<'_> {
    fn byte(&mut self) -> Result<u8, String> {
        let (&b, rest) = self.0.split_first().ok_or("the answer ends early")?;
        self.0 = rest;
        Ok(b)
    }

    fn uvar(&mut self) -> Result<u64, String> {
        let mut v = 0u64;
        let mut shift = 0;
        loop {
            let b = self.byte()?;
            v |= u64::from(b & 0x7F) << shift;
            if b & 0x80 == 0 {
                return Ok(v);
            }
            shift += 7;
            if shift > 63 {
                return Err("an overlong integer in the answer".to_owned());
            }
        }
    }

    fn text(&mut self) -> Result<String, String> {
        let len = self.uvar()? as usize;
        let (s, rest) = self
            .0
            .split_at_checked(len)
            .ok_or("the answer ends early")?;
        self.0 = rest;
        Ok(String::from_utf8_lossy(s).into_owned())
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_schema::{
        namespace_declaration, syntax_tree, type_reference_expression,
    };

    #[test]
    fn namespace_types_round_trip_through_identifiers() {
        let mut tree = SyntaxTree::default();
        let ns = tree.new_node(kind::NAMESPACE_DECLARATION);
        let name = namespace_type(&mut tree, "System.Collections.Generic");
        tree.set_child(ns, namespace_declaration::slot::NAMESPACE_NAME, Some(name));
        assert_eq!(
            namespace_identifiers(&tree, ns),
            ["System", "Collections", "Generic"]
        );
        assert_eq!(build_qualified_name("", "A"), "A");
        assert_eq!(build_qualified_name("A", ""), "A");
        assert_eq!(build_qualified_name("A", "B"), "A.B");
    }

    #[test]
    fn parent_namespaces_of_the_current_one() {
        let find = FindRequiredImports {
            oracle: &crate::csharp::syntax_oracle::NoOracle,
            current_namespace: "A.Bc".to_owned(),
            imported: HashSet::new(),
        };
        assert!(find.is_parent_of_current_namespace(""));
        assert!(find.is_parent_of_current_namespace("A"));
        assert!(find.is_parent_of_current_namespace("A.Bc"));
        assert!(!find.is_parent_of_current_namespace("A.B"));
        assert!(!find.is_parent_of_current_namespace("B"));
    }

    #[test]
    fn lookup_modes_follow_the_outermost_type() {
        let mut tree = SyntaxTree::default();
        let root = tree.new_node(kind::SYNTAX_TREE);
        let using = tree.new_node(kind::USING_DECLARATION);
        tree.insert_child(root, syntax_tree::slot::MEMBERS, 0, using);
        let import = namespace_type(&mut tree, "A.B");
        tree.set_child(
            using,
            crate::csharp::syntax_schema::using_declaration::slot::IMPORT,
            Some(import),
        );
        let inner = tree.child(import, member_type::slot::TARGET).unwrap();
        assert_eq!(
            name_lookup_mode(&tree, inner),
            LOOKUP_TYPE_IN_USING_DECLARATION
        );

        let ty = tree.new_node(kind::TYPE_DECLARATION);
        let base = namespace_type(&mut tree, "Base");
        tree.insert_child(ty, type_declaration::slot::BASE_TYPES, 0, base);
        assert_eq!(name_lookup_mode(&tree, base), LOOKUP_BASE_TYPE_REFERENCE);

        let tre = tree.new_node(kind::TYPE_REFERENCE_EXPRESSION);
        let t = namespace_type(&mut tree, "T");
        tree.set_child(tre, type_reference_expression::slot::TYPE, Some(t));
        assert_eq!(name_lookup_mode(&tree, t), LOOKUP_EXPRESSION);

        let alone = namespace_type(&mut tree, "U");
        assert_eq!(name_lookup_mode(&tree, alone), LOOKUP_TYPE);
    }
}
