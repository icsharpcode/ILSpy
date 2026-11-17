//! `DeclareVariables` (`CSharp/Transforms/DeclareVariables.cs`): inserts the declarations of the
//! local variables the tree uses, at the innermost scope that holds all their uses, combined
//! with the first assignment or as `out var` where possible.
//!
//! The ILVariables, BlockContainers and ILFunctions the transform reasons about stay host
//! objects; the engine knows them by object ids the host hands out, and asks the host
//! (`SyntaxRoundTrip.DeclareVariables.cs`) through these query kinds, each a kind byte and an
//! unsigned LEB128 argument:
//!
//! - 105 `QUERY_DV_ANNOTATION_TAGS` (argument ignored): one tag byte per host handle, 0 other,
//!   1 a `ResolveResult`, 2 an `ILVariableResolveResult`, 3 a `BlockContainer`, 4 an
//!   `ILFunction`, 5 a `DeconstructInstruction`, 6 the `UseImplicitlyTypedOutAnnotation`.
//! - 96 `QUERY_DV_ANNOTATION` (handle): the tag, then for 2 the variable id; for 3 the scope
//!   (container id, flags: 1 the entry point has more than one incoming edge, 2 the parent is
//!   an `ILFunction`); for 4 the function id, flags (1 a local function, 2 no parent), a
//!   presence byte and the scope of its body if that is a container, and the ids of the
//!   captured variables of a local function (count, ids).
//! - 97 `QUERY_DV_VARIABLE` (variable id): the name (length, UTF-8), the `VariableKind` byte,
//!   flags (1 `UsesInitialValue`, 2 `InitialValueIsInitialized`, 4 `IsRefReadOnly`, 8
//!   `IsScoped`, 16 `IsScopedWithoutInitializer`, 32 the type is by-ref-like, 64 the type
//!   contains an anonymous type), the capture scope moved out to the closest relevant scope
//!   (container id plus one, 0 for none), and the load, store and address counts.
//! - 98 `QUERY_DV_SETTINGS`: bits 1 `Discards`, 2 `AnonymousTypes`, 4
//!   `SeparateLocalVariableDeclarations`, 8 `ScopedRef`, 16 `DefaultLiterals`, 32 `OutVariables`.
//! - 99 `QUERY_DV_HIDES_DISCARD` (function id): 1 where a variable named `_` is in scope.
//! - 100 `QUERY_DV_TEMPORARY_VARIABLE` (function id, then the count and handles of the
//!   expression's annotations): registers the temporary for an expression statement that is not
//!   a valid statement expression, named as the managed transform names it; its variable id.
//! - 101 `QUERY_DV_VARIABLE_TYPE` (variable id), 102 `QUERY_DV_UNSAFE_TYPE`: the type syntax
//!   the host's `TypeSystemAstBuilder` builds, as a subtree.
//! - 103 `QUERY_DV_DECONSTRUCTION` (handle of a `DeconstructInstruction`): 1 where it has no
//!   init and no conversions, then per assignment a state (0 no `stloc`, 1 the variable's type
//!   differs from the loaded one, 2 it equals it, 3 the value is no `ldloc`) and, unless 0, the
//!   variable id.
//! - 104 `QUERY_DV_DESIGNATION` (handle of a `DeconstructInstruction`): the deconstruction
//!   designation, as a subtree.
//!
//! A subtree answer is a node in the wire format whose handles index objects the host keeps for
//! the engine, each attached with an annotation of kind 98. The annotation kinds the engine
//! creates (payload an unsigned LEB128): 96 a new `ILVariableResolveResult` of the variable id,
//! 97 an `OutVarResolveResult` of the variable's type, 98 the host-built object of that index,
//! 99 `ErrorResolveResult.UnknownError`.

use std::collections::{HashMap, HashSet};
use std::rc::Rc;

use crate::csharp::syntax::{Scalar, SlotValue, SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::{
    QUERY_DV_ANNOTATION, QUERY_DV_ANNOTATION_TAGS, QUERY_DV_DECONSTRUCTION, QUERY_DV_DESIGNATION,
    QUERY_DV_HIDES_DISCARD, QUERY_DV_SETTINGS, QUERY_DV_TEMPORARY_VARIABLE, QUERY_DV_UNSAFE_TYPE,
    QUERY_DV_VARIABLE, QUERY_DV_VARIABLE_TYPE, SyntaxOracle,
};
use crate::csharp::syntax_schema::{
    KINDS, assignment_expression, block_statement, comment, composed_type, declaration_expression,
    default_value_expression, direction_expression, expression_statement, for_statement,
    identifier, identifier_expression, invocation_expression, kind, lambda_expression,
    member_reference_expression, out_var_declaration_expression, return_statement, simple_type,
    type_reference_expression, unary_operator_expression, variable_declaration_statement,
    variable_initializer,
};
use crate::typesystem::types::TypeSystem;

const ANNOTATION_VARIABLE: u8 = 96;
const ANNOTATION_OUT_VAR: u8 = 97;
const ANNOTATION_MINTED: u8 = 98;
const ANNOTATION_UNKNOWN_ERROR: u8 = 99;

// `VariableKind` values.
const LOCAL: u8 = 0;
const PINNED_LOCAL: u8 = 1;
const PINNED_REGION_LOCAL: u8 = 2;
const USING_LOCAL: u8 = 3;
const FOREACH_LOCAL: u8 = 4;
const PARAMETER: u8 = 6;
const EXCEPTION_STACK_SLOT: u8 = 7;
const EXCEPTION_LOCAL: u8 = 8;
const STACK_SLOT: u8 = 9;
const PATTERN_LOCAL: u8 = 12;

// `QUERY_DV_VARIABLE` flags.
const USES_INITIAL_VALUE: u32 = 1;
const INITIAL_VALUE_IS_INITIALIZED: u32 = 2;
const IS_REF_READ_ONLY: u32 = 4;
const IS_SCOPED: u32 = 8;
const IS_SCOPED_WITHOUT_INITIALIZER: u32 = 16;
const IS_BY_REF_LIKE: u32 = 32;
const CONTAINS_ANONYMOUS_TYPE: u32 = 64;

// `QUERY_DV_SETTINGS` bits.
const DISCARDS: u32 = 1;
const ANONYMOUS_TYPES: u32 = 2;
const SEPARATE_LOCAL_VARIABLE_DECLARATIONS: u32 = 4;
const SCOPED_REF: u32 = 8;
const DEFAULT_LITERALS: u32 = 16;
const OUT_VARIABLES: u32 = 32;

// `VariableInitKind` flags (the managed enum, combined with `|=`).
const NEEDS_DEFAULT_VALUE: u8 = 1;
const NEEDS_SKIP_INIT: u8 = 2;

// Syntax enum values.
const ASSIGN: i64 = 0;
const FIELD_DIRECTION_OUT: i64 = 1;
const COMMENT_MULTI_LINE: i64 = 1;
const MODIFIERS_SCOPED: i64 = 0x80000;

/// The built-in AST transform `DeclareVariables`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let mut dv = DeclareVariables {
        host: Host::new(tree, oracle)?,
        decls: Vec::new(),
        dict: HashMap::new(),
        tracking: Vec::new(),
    };
    dv.ensure_expression_statements_are_valid(tree, root)?;
    dv.find_insertion_points(tree, root)?;
    dv.resolve_collisions(tree)?;
    dv.insert_deconstruction_variable_declarations(tree)?;
    dv.insert_variable_declarations(tree)?;
    dv.update_annotations(tree, root)
}

/// A scope (`BlockContainer`) by its object id.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
struct Scope {
    id: u64,
    /// `EntryPoint.IncomingEdgeCount > 1`.
    loop_entry: bool,
    parent_is_function: bool,
}

impl Scope {
    fn is_relevant(self) -> bool {
        self.loop_entry || self.parent_is_function
    }
}

#[derive(Debug)]
struct Function {
    id: u64,
    local_function: bool,
    is_root: bool,
    body: Option<Scope>,
    captured: Vec<u64>,
}

/// What an annotation is to the transform.
#[derive(Clone, Debug)]
enum Ann {
    Other,
    ResolveResult,
    Variable(u64),
    Container(Scope),
    Function(Rc<Function>),
    Deconstruct,
    ImplicitOut,
}

impl Ann {
    fn is_resolve_result(&self) -> bool {
        matches!(self, Ann::ResolveResult | Ann::Variable(_))
    }
}

#[derive(Debug)]
struct Variable {
    name: String,
    kind: u8,
    flags: u32,
    capture_scope: Option<u64>,
    load_count: u64,
    store_count: u64,
    address_count: u64,
}

/// The answers of the host, cached, and the annotations the engine created.
struct Host<'a> {
    oracle: &'a dyn SyntaxOracle,
    tags: Vec<u8>,
    annotations: HashMap<u32, Ann>,
    variables: HashMap<u64, Rc<Variable>>,
    settings: Option<u32>,
    /// The handles of the host-built objects, by their index.
    minted: HashMap<u64, u32>,
}

/// Reads an answer.
struct Answer {
    bytes: Vec<u8>,
    pos: usize,
}

impl Answer {
    fn u8(&mut self) -> Result<u8, String> {
        let b = *self
            .bytes
            .get(self.pos)
            .ok_or("a DeclareVariables answer ends early")?;
        self.pos += 1;
        Ok(b)
    }

    fn uvar(&mut self) -> Result<u64, String> {
        let mut v = 0u64;
        let mut shift = 0;
        loop {
            let b = self.u8()?;
            v |= u64::from(b & 0x7F) << shift;
            if b & 0x80 == 0 {
                return Ok(v);
            }
            shift += 7;
            if shift > 63 {
                return Err("an overlong integer in a DeclareVariables answer".to_owned());
            }
        }
    }

    fn text(&mut self) -> Result<String, String> {
        let len = self.uvar()? as usize;
        let s = self
            .bytes
            .get(self.pos..self.pos + len)
            .ok_or("a DeclareVariables answer ends early")?;
        self.pos += len;
        Ok(String::from_utf8_lossy(s).into_owned())
    }

    fn scope(&mut self) -> Result<Scope, String> {
        let id = self.uvar()?;
        let flags = self.u8()?;
        Ok(Scope {
            id,
            loop_entry: flags & 1 != 0,
            parent_is_function: flags & 2 != 0,
        })
    }
}

fn uvar(out: &mut Vec<u8>, mut v: u64) {
    loop {
        let b = (v & 0x7F) as u8;
        v >>= 7;
        if v == 0 {
            out.push(b);
            return;
        }
        out.push(b | 0x80);
    }
}

fn payload(v: u64) -> Vec<u8> {
    let mut p = Vec::new();
    uvar(&mut p, v);
    p
}

impl<'a> Host<'a> {
    fn new(tree: &SyntaxTree, oracle: &'a dyn SyntaxOracle) -> Result<Self, String> {
        let tags = if tree.host_handles == 0 {
            Vec::new()
        } else {
            oracle
                .ask(&[QUERY_DV_ANNOTATION_TAGS, 0])
                .ok_or("the host does not tell the annotation tags")?
        };
        if tags.len() != tree.host_handles as usize {
            return Err(format!(
                "{} annotation tags for {} handles",
                tags.len(),
                tree.host_handles
            ));
        }
        Ok(Host {
            oracle,
            tags,
            annotations: HashMap::new(),
            variables: HashMap::new(),
            settings: None,
            minted: HashMap::new(),
        })
    }

    fn ask(&self, kind: u8, args: &[u64]) -> Result<Answer, String> {
        let mut q = vec![kind];
        for &a in args {
            uvar(&mut q, a);
        }
        let bytes = self
            .oracle
            .ask(&q)
            .ok_or_else(|| format!("the host does not answer the DeclareVariables query {kind}"))?;
        Ok(Answer { bytes, pos: 0 })
    }

    fn annotation(&mut self, handle: u32) -> Result<Ann, String> {
        if let Some(a) = self.annotations.get(&handle) {
            return Ok(a.clone());
        }
        let ann = match self.tags.get(handle as usize) {
            // shortcut: host-built objects (in type syntax and designations) count as "other",
            // which holds while no node the transform inspects is built by the host.
            None => Ann::Other,
            Some(0) => Ann::Other,
            Some(1) => Ann::ResolveResult,
            Some(5) => Ann::Deconstruct,
            Some(6) => Ann::ImplicitOut,
            Some(_) => {
                let mut a = self.ask(QUERY_DV_ANNOTATION, &[u64::from(handle)])?;
                match a.u8()? {
                    2 => Ann::Variable(a.uvar()?),
                    3 => Ann::Container(a.scope()?),
                    4 => {
                        let id = a.uvar()?;
                        let flags = a.u8()?;
                        let body = if a.u8()? != 0 { Some(a.scope()?) } else { None };
                        let n = a.uvar()?;
                        let mut captured = Vec::new();
                        for _ in 0..n {
                            captured.push(a.uvar()?);
                        }
                        Ann::Function(Rc::new(Function {
                            id,
                            local_function: flags & 1 != 0,
                            is_root: flags & 2 != 0,
                            body,
                            captured,
                        }))
                    }
                    t => return Err(format!("an annotation of the unexpected tag {t}")),
                }
            }
        };
        self.annotations.insert(handle, ann.clone());
        Ok(ann)
    }

    fn variable(&mut self, id: u64) -> Result<Rc<Variable>, String> {
        if let Some(v) = self.variables.get(&id) {
            return Ok(v.clone());
        }
        let mut a = self.ask(QUERY_DV_VARIABLE, &[id])?;
        let v = Rc::new(Variable {
            name: a.text()?,
            kind: a.u8()?,
            flags: a.uvar()? as u32,
            capture_scope: a.uvar()?.checked_sub(1),
            load_count: a.uvar()?,
            store_count: a.uvar()?,
            address_count: a.uvar()?,
        });
        self.variables.insert(id, v.clone());
        Ok(v)
    }

    fn setting(&mut self, bit: u32) -> Result<bool, String> {
        let bits = match self.settings {
            Some(b) => b,
            None => {
                let b = self.ask(QUERY_DV_SETTINGS, &[0])?.uvar()? as u32;
                self.settings = Some(b);
                b
            }
        };
        Ok(bits & bit != 0)
    }

    /// The annotations of `node`, classified.
    fn annotations_of(&mut self, tree: &SyntaxTree, node: SyntaxId) -> Result<Vec<Ann>, String> {
        tree.node(node)
            .annotations
            .iter()
            .map(|&h| self.annotation(h))
            .collect()
    }

    /// `node.Annotation<ResolveResult>()`, as the handle and what it is.
    fn resolve_result(
        &mut self,
        tree: &SyntaxTree,
        node: SyntaxId,
    ) -> Result<Option<(u32, Ann)>, String> {
        for &h in &tree.node(node).annotations {
            let a = self.annotation(h)?;
            if a.is_resolve_result() {
                return Ok(Some((h, a)));
            }
        }
        Ok(None)
    }

    /// `node.GetResolveResult() as ILVariableResolveResult`, as the variable id.
    fn il_variable(&mut self, tree: &SyntaxTree, node: SyntaxId) -> Result<Option<u64>, String> {
        Ok(match self.resolve_result(tree, node)? {
            Some((_, Ann::Variable(v))) => Some(v),
            _ => None,
        })
    }

    fn container(&mut self, tree: &SyntaxTree, node: SyntaxId) -> Result<Option<Scope>, String> {
        for a in self.annotations_of(tree, node)? {
            if let Ann::Container(s) = a {
                return Ok(Some(s));
            }
        }
        Ok(None)
    }

    fn function(
        &mut self,
        tree: &SyntaxTree,
        node: SyntaxId,
    ) -> Result<Option<Rc<Function>>, String> {
        for a in self.annotations_of(tree, node)? {
            if let Ann::Function(f) = a {
                return Ok(Some(f));
            }
        }
        Ok(None)
    }

    fn new_annotation(&mut self, tree: &mut SyntaxTree, kind: u8, value: u64, ann: Ann) -> u32 {
        let h = tree.new_annotation(kind, payload(value));
        self.annotations.insert(h, ann);
        h
    }

    fn variable_annotation(&mut self, tree: &mut SyntaxTree, variable: u64) -> u32 {
        self.new_annotation(tree, ANNOTATION_VARIABLE, variable, Ann::Variable(variable))
    }

    /// Reads a subtree the host built and attaches its objects.
    fn subtree(&mut self, tree: &mut SyntaxTree, a: Answer) -> Result<SyntaxId, String> {
        let root = tree.decode_subtree(&a.bytes[a.pos..])?;
        for n in nodes_with_trivia(tree, root) {
            let annotations = tree.node(n).annotations.clone();
            let mapped = annotations
                .into_iter()
                .map(|h| self.minted_handle(tree, h))
                .collect();
            tree.node_mut(n).annotations = mapped;
            for i in 0..tree.node(n).scalars.len() {
                if let Scalar::Object(h) = tree.node(n).scalars[i] {
                    let m = self.minted_handle(tree, h);
                    tree.node_mut(n).scalars[i] = Scalar::Object(m);
                }
            }
        }
        Ok(root)
    }

    fn minted_handle(&mut self, tree: &mut SyntaxTree, index: u32) -> u32 {
        let index = u64::from(index);
        if let Some(&h) = self.minted.get(&index) {
            return h;
        }
        let h = self.new_annotation(tree, ANNOTATION_MINTED, index, Ann::Other);
        self.minted.insert(index, h);
        h
    }

    fn variable_type(&mut self, tree: &mut SyntaxTree, variable: u64) -> Result<SyntaxId, String> {
        let a = self.ask(QUERY_DV_VARIABLE_TYPE, &[variable])?;
        self.subtree(tree, a)
    }
}

/// The nodes of the subtree of `id`, trivia included.
fn nodes_with_trivia(tree: &SyntaxTree, id: SyntaxId) -> Vec<SyntaxId> {
    let mut result = Vec::new();
    let mut stack = vec![id];
    while let Some(n) = stack.pop() {
        result.push(n);
        let node = tree.node(n);
        stack.extend(node.leading_trivia.iter().copied());
        stack.extend(node.trailing_trivia.iter().copied());
        stack.extend(tree.all_children(n));
    }
    result
}

/// A position immediately before `node`, which is `level` deep in the tree.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
struct InsertionPoint {
    level: i64,
    node: SyntaxId,
}

impl InsertionPoint {
    fn up(self, tree: &SyntaxTree) -> Result<Self, String> {
        Ok(InsertionPoint {
            level: self.level - 1,
            node: parent(tree, self.node)?,
        })
    }

    fn up_to(mut self, tree: &SyntaxTree, level: i64) -> Result<Self, String> {
        while self.level > level {
            self = self.up(tree)?;
        }
        Ok(self)
    }
}

fn parent(tree: &SyntaxTree, node: SyntaxId) -> Result<SyntaxId, String> {
    tree.node(node)
        .parent
        .ok_or_else(|| "an insertion point without parent".to_owned())
}

struct VariableToDeclare {
    variable: u64,
    default_initialization: u8,
    source_order: usize,
    insertion_point: InsertionPoint,
    first_use: SyntaxId,
    replacement_due_to_collision: Option<usize>,
    involved_in_collision: bool,
    declared_in_deconstruction: bool,
}

/// A node of `FindInsertionPoints` whose children are being visited.
struct Frame {
    node: SyntaxId,
    level: i64,
    children: Vec<SyntaxId>,
    next: usize,
    /// Whether entering the node pushed a scope onto the tracking stack.
    pushed: bool,
}

enum Replacement {
    /// `int v; v = expr;` to `int v = expr;`, built when the replacements are performed.
    Combine {
        old: SyntaxId,
        assignment: SyntaxId,
        ty: SyntaxId,
        decl: usize,
    },
    Node {
        old: SyntaxId,
        new: SyntaxId,
    },
}

struct DeclareVariables<'a> {
    host: Host<'a>,
    /// `variableDict`, in insertion order.
    decls: Vec<VariableToDeclare>,
    dict: HashMap<u64, usize>,
    tracking: Vec<(InsertionPoint, Scope)>,
}

fn kind_name(tree: &SyntaxTree, node: SyntaxId) -> &'static str {
    KINDS[tree.kind(node) as usize].name
}

fn is_statement(tree: &SyntaxTree, node: SyntaxId) -> bool {
    kind_name(tree, node).ends_with("Statement")
}

fn is_expression(tree: &SyntaxTree, node: SyntaxId) -> bool {
    kind_name(tree, node).ends_with("Expression")
}

/// The kind of the slot `node` is in (`node.Slot?.Kind`).
fn slot_kind(tree: &SyntaxTree, node: SyntaxId) -> Option<&'static str> {
    let p = tree.node(node).parent?;
    let pn = tree.node(p);
    pn.slots
        .iter()
        .position(|s| match s {
            SlotValue::Single(c) => *c == Some(node),
            SlotValue::Many(cs) => cs.contains(&node),
        })
        .map(|i| pn.schema().slots[i].kind)
}

fn int_scalar(tree: &SyntaxTree, node: SyntaxId, i: usize) -> i64 {
    tree.node(node).scalars[i].as_int().unwrap_or(0)
}

fn bool_scalar(tree: &SyntaxTree, node: SyntaxId, i: usize) -> bool {
    tree.node(node).scalars[i].as_bool().unwrap_or(false)
}

fn to_utf16(s: &str) -> Vec<u16> {
    s.encode_utf16().collect()
}

/// `Identifier.Create(name)`.
fn new_identifier(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
    let id = tree.new_node(kind::IDENTIFIER);
    let (name, verbatim) = match name.strip_prefix('@') {
        Some(rest) => (rest, true),
        None => (name, false),
    };
    tree.node_mut(id).scalars[identifier::scalar::NAME] = Scalar::Str(to_utf16(name));
    tree.node_mut(id).scalars[identifier::scalar::IS_VERBATIM] = Scalar::Bool(verbatim);
    id
}

fn identifier_expression(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
    let e = tree.new_node(kind::IDENTIFIER_EXPRESSION);
    let i = new_identifier(tree, name);
    tree.set_child(e, identifier_expression::slot::IDENTIFIER_TOKEN, Some(i));
    e
}

fn simple_type(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
    let t = tree.new_node(kind::SIMPLE_TYPE);
    let i = new_identifier(tree, name);
    tree.set_child(t, simple_type::slot::IDENTIFIER_TOKEN, Some(i));
    t
}

fn variable_initializer(
    tree: &mut SyntaxTree,
    name: &str,
    initializer: Option<SyntaxId>,
) -> SyntaxId {
    let v = tree.new_node(kind::VARIABLE_INITIALIZER);
    let i = new_identifier(tree, name);
    tree.set_child(v, variable_initializer::slot::NAME_TOKEN, Some(i));
    tree.set_child(v, variable_initializer::slot::INITIALIZER, initializer);
    v
}

fn variable_declaration(
    tree: &mut SyntaxTree,
    ty: SyntaxId,
    name: &str,
    initializer: Option<SyntaxId>,
) -> (SyntaxId, SyntaxId) {
    let vds = tree.new_node(kind::VARIABLE_DECLARATION_STATEMENT);
    tree.set_child(vds, variable_declaration_statement::slot::TYPE, Some(ty));
    let init = variable_initializer(tree, name, initializer);
    tree.insert_child(
        vds,
        variable_declaration_statement::slot::VARIABLES,
        0,
        init,
    );
    (vds, init)
}

fn out_var_declaration(tree: &mut SyntaxTree, ty: SyntaxId, name: &str) -> (SyntaxId, SyntaxId) {
    let ovd = tree.new_node(kind::OUT_VAR_DECLARATION_EXPRESSION);
    tree.set_child(ovd, out_var_declaration_expression::slot::TYPE, Some(ty));
    let v = variable_initializer(tree, name, None);
    tree.set_child(ovd, out_var_declaration_expression::slot::VARIABLE, Some(v));
    (ovd, v)
}

/// `Unsafe.SkipInit(argument);` as a statement.
fn skip_init_statement(
    tree: &mut SyntaxTree,
    unsafe_type: SyntaxId,
    argument: SyntaxId,
) -> SyntaxId {
    let type_ref = tree.new_node(kind::TYPE_REFERENCE_EXPRESSION);
    tree.set_child(
        type_ref,
        type_reference_expression::slot::TYPE,
        Some(unsafe_type),
    );
    let member = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
    tree.set_child(
        member,
        member_reference_expression::slot::TARGET,
        Some(type_ref),
    );
    let name = new_identifier(tree, "SkipInit");
    tree.set_child(
        member,
        member_reference_expression::slot::MEMBER_NAME_TOKEN,
        Some(name),
    );
    let call = tree.new_node(kind::INVOCATION_EXPRESSION);
    tree.set_child(call, invocation_expression::slot::TARGET, Some(member));
    tree.insert_child(call, invocation_expression::slot::ARGUMENTS, 0, argument);
    let stmt = tree.new_node(kind::EXPRESSION_STATEMENT);
    tree.set_child(stmt, expression_statement::slot::EXPRESSION, Some(call));
    stmt
}

/// `AstNode.ReplaceWith`.
fn replace_with(tree: &mut SyntaxTree, old: SyntaxId, new: SyntaxId) -> Result<(), String> {
    if tree.node(old).parent.is_none() {
        return Err("Cannot replace the root node".to_owned());
    }
    tree.replace(old, new);
    Ok(())
}

/// `parent.InsertChildBefore(next, child, Slots.Statement)`.
fn insert_statement_before(
    tree: &mut SyntaxTree,
    parent: SyntaxId,
    next: SyntaxId,
    child: SyntaxId,
) -> Result<(), String> {
    let pn = tree.node(parent);
    let slot = pn
        .schema()
        .slots
        .iter()
        .position(|s| s.collection && s.kind == "Statement")
        .ok_or_else(|| format!("no statement collection in {}", pn.schema().name))?;
    let index = tree
        .children(parent, slot)
        .iter()
        .position(|&c| c == next)
        .ok_or("the insertion point is not in the statement collection")?;
    tree.insert_child(parent, slot, index, child);
    Ok(())
}

/// `IsValidInStatementExpression`.
fn is_valid_in_statement_expression(tree: &SyntaxTree, expr: SyntaxId) -> bool {
    match tree.kind(expr) {
        kind::INVOCATION_EXPRESSION
        | kind::OBJECT_CREATE_EXPRESSION
        | kind::ASSIGNMENT_EXPRESSION
        | kind::ERROR_EXPRESSION => true,
        kind::UNARY_OPERATOR_EXPRESSION => {
            match int_scalar(tree, expr, unary_operator_expression::scalar::OPERATOR) {
                // Increment, Decrement, PostIncrement, PostDecrement, Await
                5..=8 | 11 => true,
                // NullConditionalRewrap
                13 => tree
                    .child(expr, unary_operator_expression::slot::EXPRESSION)
                    .is_some_and(|e| is_valid_in_statement_expression(tree, e)),
                _ => false,
            }
        }
        _ => false,
    }
}

/// `VariableNeedsDeclaration`.
fn variable_needs_declaration(kind: u8) -> bool {
    !matches!(
        kind,
        PINNED_REGION_LOCAL
            | PARAMETER
            | EXCEPTION_LOCAL
            | EXCEPTION_STACK_SLOT
            | USING_LOCAL
            | FOREACH_LOCAL
            | PATTERN_LOCAL
    )
}

/// `FindCommonParent`: the point in the common parent, `old` as it comes first.
fn find_common_parent(
    tree: &SyntaxTree,
    old: InsertionPoint,
    new: InsertionPoint,
) -> Result<InsertionPoint, String> {
    let mut old = old.up_to(tree, new.level)?;
    let mut new = new.up_to(tree, old.level)?;
    while tree.node(old.node).parent != tree.node(new.node).parent {
        old = old.up(tree)?;
        new = new.up(tree)?;
    }
    Ok(old)
}

impl DeclareVariables<'_> {
    fn ensure_expression_statements_are_valid(
        &mut self,
        tree: &mut SyntaxTree,
        root: SyntaxId,
    ) -> Result<(), String> {
        let mut stack = vec![root];
        while let Some(stmt) = stack.pop() {
            if tree.kind(stmt) == kind::EXPRESSION_STATEMENT {
                self.ensure_valid(tree, stmt)?;
            }
            stack.extend(tree.all_children(stmt).into_iter().rev());
        }
        Ok(())
    }

    fn ensure_valid(&mut self, tree: &mut SyntaxTree, stmt: SyntaxId) -> Result<(), String> {
        let Some(expr) = tree.child(stmt, expression_statement::slot::EXPRESSION) else {
            return Ok(());
        };
        if tree.kind(expr) == kind::DIRECTION_EXPRESSION
            && let Some(inner) = tree.child(expr, direction_expression::slot::EXPRESSION)
            && is_valid_in_statement_expression(tree, inner)
        {
            tree.set_child(stmt, expression_statement::slot::EXPRESSION, Some(inner));
            return Ok(());
        }
        if is_valid_in_statement_expression(tree, expr) {
            return Ok(());
        }
        let function = self.root_function(tree, stmt)?;
        let left = if self.host.setting(DISCARDS)?
            && self.host.ask(QUERY_DV_HIDES_DISCARD, &[function])?.u8()? == 0
        {
            identifier_expression(tree, "_")
        } else {
            let mut args = vec![function, tree.node(expr).annotations.len() as u64];
            args.extend(tree.node(expr).annotations.iter().map(|&h| u64::from(h)));
            let v = self.host.ask(QUERY_DV_TEMPORARY_VARIABLE, &args)?.uvar()?;
            let name = self.host.variable(v)?.name.clone();
            let id = identifier_expression(tree, &name);
            let rr = self.host.variable_annotation(tree, v);
            tree.node_mut(id).annotations.push(rr);
            id
        };
        let assignment = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        tree.set_child(assignment, assignment_expression::slot::LEFT, Some(left));
        tree.set_child(assignment, assignment_expression::slot::RIGHT, Some(expr));
        tree.set_child(
            stmt,
            expression_statement::slot::EXPRESSION,
            Some(assignment),
        );
        Ok(())
    }

    /// The ILFunction without parent among the annotations of the ancestors of `node`.
    fn root_function(&mut self, tree: &SyntaxTree, node: SyntaxId) -> Result<u64, String> {
        let mut a = tree.node(node).parent;
        while let Some(n) = a {
            for ann in self.host.annotations_of(tree, n)? {
                if let Ann::Function(f) = ann
                    && f.is_root
                {
                    return Ok(f.id);
                }
            }
            a = tree.node(n).parent;
        }
        Err("Sequence contains no matching element".to_owned())
    }

    /// `FindInsertionPoints`, without recursion.
    fn find_insertion_points(&mut self, tree: &SyntaxTree, root: SyntaxId) -> Result<(), String> {
        let mut stack = vec![self.enter(tree, root, 0)?];
        while let Some(top) = stack.last_mut() {
            if top.next < top.children.len() {
                let child = top.children[top.next];
                top.next += 1;
                let level = top.level + 1;
                let frame = self.enter(tree, child, level)?;
                stack.push(frame);
                continue;
            }
            let Frame {
                node,
                level,
                pushed,
                ..
            } = stack.pop().unwrap();
            if tree.kind(node) == kind::IDENTIFIER_EXPRESSION {
                self.identifier_found(tree, node, level)?;
            }
            if pushed {
                self.tracking.pop();
            }
        }
        Ok(())
    }

    /// Enters `node` in `FindInsertionPoints`: tracks it if it is a scope that loops and function
    /// bodies are compared with (an expression-bodied lambda through its function's body).
    fn enter(&mut self, tree: &SyntaxTree, node: SyntaxId, level: i64) -> Result<Frame, String> {
        let mut pushed = false;
        match self.host.container(tree, node)? {
            Some(scope) if scope.is_relevant() => {
                self.tracking.push((InsertionPoint { level, node }, scope));
                pushed = true;
            }
            _ => {
                if tree.kind(node) == kind::LAMBDA_EXPRESSION
                    && let Some(body) = tree.child(node, lambda_expression::slot::BODY)
                    && is_expression(tree, body)
                    && let Some(f) = self.host.function(tree, node)?
                    && let Some(scope) = f.body
                {
                    self.tracking.push((
                        InsertionPoint {
                            level: level + 1,
                            node: body,
                        },
                        scope,
                    ));
                    pushed = true;
                }
            }
        }
        Ok(Frame {
            node,
            level,
            children: tree.all_children(node),
            next: 0,
            pushed,
        })
    }

    fn identifier_found(
        &mut self,
        tree: &SyntaxTree,
        ident: SyntaxId,
        level: i64,
    ) -> Result<(), String> {
        if let Some(v) = self.host.il_variable(tree, ident)?
            && variable_needs_declaration(self.host.variable(v)?.kind)
        {
            return self.find_insertion_point_for_variable(tree, v, ident, level);
        }
        if let Some(f) = self.host.function(tree, ident)?
            && f.local_function
        {
            for &v in &f.captured {
                if variable_needs_declaration(self.host.variable(v)?.kind) {
                    self.find_insertion_point_for_variable(tree, v, ident, level)?;
                }
            }
        }
        Ok(())
    }

    fn find_insertion_point_for_variable(
        &mut self,
        tree: &SyntaxTree,
        variable: u64,
        ident: SyntaxId,
        level: i64,
    ) -> Result<(), String> {
        let facts = self.host.variable(variable)?;
        let tracking = &self.tracking;
        let mut start = tracking.len() as isize - 1;
        let new_point = match facts.capture_scope {
            Some(capture) if start > 0 && capture != tracking[start as usize].1.id => {
                while start > 0 && tracking[start as usize].1.id != capture {
                    start -= 1;
                }
                tracking[start as usize + 1].0
            }
            _ => {
                let mut point = InsertionPoint { level, node: ident };
                if facts.flags & USES_INITIAL_VALUE != 0 {
                    // Declared outside of any loop, as the variable may carry its value from one
                    // iteration to the next; uninitialized variables start at the function.
                    while start >= 0 {
                        let (p, scope) = tracking[start as usize];
                        if scope.loop_entry {
                            point = p;
                        } else if scope.parent_is_function {
                            break;
                        }
                        start -= 1;
                    }
                }
                point
            }
        };
        if let Some(&i) = self.dict.get(&variable) {
            let old = self.decls[i].insertion_point;
            self.decls[i].insertion_point = find_common_parent(tree, old, new_point)?;
        } else {
            let default_initialization = if facts.flags & USES_INITIAL_VALUE == 0 {
                0
            } else if facts.flags & INITIAL_VALUE_IS_INITIALIZED != 0 {
                NEEDS_DEFAULT_VALUE
            } else {
                NEEDS_SKIP_INIT
            };
            self.dict.insert(variable, self.decls.len());
            self.decls.push(VariableToDeclare {
                variable,
                default_initialization,
                source_order: self.decls.len(),
                insertion_point: new_point,
                first_use: ident,
                replacement_due_to_collision: None,
                involved_in_collision: false,
                declared_in_deconstruction: false,
            });
        }
        Ok(())
    }

    fn name(&mut self, i: usize) -> Result<String, String> {
        Ok(self.host.variable(self.decls[i].variable)?.name.clone())
    }

    fn resolve_collisions(&mut self, tree: &SyntaxTree) -> Result<(), String> {
        let mut by_name: HashMap<String, Vec<usize>> = HashMap::new();
        for i in 0..self.decls.len() {
            // Declarations go into blocks (or an expression-bodied lambda, which becomes one, or
            // the first initializer of a for loop that assigns the variable).
            loop {
                let point = self.decls[i].insertion_point;
                let p = parent(tree, point.node)?;
                if tree.kind(p) == kind::BLOCK_STATEMENT
                    || (tree.kind(p) == kind::LAMBDA_EXPRESSION && is_expression(tree, point.node))
                {
                    break;
                }
                if tree.kind(p) == kind::FOR_STATEMENT
                    && tree
                        .children(p, for_statement::slot::INITIALIZERS)
                        .first()
                        .copied()
                        == Some(point.node)
                    && self.matching_assignment(tree, i)?.is_some()
                {
                    break;
                }
                self.decls[i].insertion_point = point.up(tree)?;
            }
            let name = self.name(i)?;
            for prev in by_name.get(&name).cloned().unwrap_or_default() {
                if self.decls[prev].replacement_due_to_collision.is_some() {
                    continue;
                }
                let prev_point = self.decls[prev].insertion_point;
                let point = self.decls[i].insertion_point;
                let point1 = prev_point.up_to(tree, point.level)?;
                let point2 = point.up_to(tree, prev_point.level)?;
                if tree.node(point1.node).parent == tree.node(point2.node).parent {
                    self.decls[i].involved_in_collision = true;
                    self.decls[prev].replacement_due_to_collision = Some(i);
                    if self.decls[prev].source_order < self.decls[i].source_order {
                        self.decls[i].insertion_point = point1;
                        self.decls[i].source_order = self.decls[prev].source_order;
                        self.decls[i].first_use = self.decls[prev].first_use;
                    } else {
                        self.decls[i].insertion_point = point2;
                    }
                    self.decls[i].default_initialization |= self.decls[prev].default_initialization;
                }
            }
            by_name.entry(name).or_default().push(i);
        }
        Ok(())
    }

    fn insert_deconstruction_variable_declarations(
        &mut self,
        tree: &mut SyntaxTree,
    ) -> Result<(), String> {
        let mut groups: Vec<(SyntaxId, Vec<usize>)> = Vec::new();
        for (i, d) in self.decls.iter().enumerate() {
            let key = d.insertion_point.node;
            match groups.iter_mut().find(|(k, _)| *k == key) {
                Some((_, members)) => members.push(i),
                None => groups.push((key, vec![i])),
            }
        }
        let mut used = HashSet::new();
        for (key, members) in groups {
            if tree.kind(key) != kind::EXPRESSION_STATEMENT {
                continue;
            }
            let Some(assignment) = tree.child(key, expression_statement::slot::EXPRESSION) else {
                continue;
            };
            if tree.kind(assignment) != kind::ASSIGNMENT_EXPRESSION
                || int_scalar(tree, assignment, assignment_expression::scalar::OPERATOR) != ASSIGN
            {
                continue;
            }
            let Some(left) = tree.child(assignment, assignment_expression::slot::LEFT) else {
                continue;
            };
            if tree.kind(left) != kind::TUPLE_EXPRESSION {
                continue;
            }
            used.clear();
            let mut deconstruct = None;
            for &h in &tree.node(assignment).annotations {
                if let Ann::Deconstruct = self.host.annotation(h)? {
                    deconstruct = Some(h);
                    break;
                }
            }
            let Some(deconstruct) = deconstruct else {
                continue;
            };
            let mut a = self
                .host
                .ask(QUERY_DV_DECONSTRUCTION, &[u64::from(deconstruct)])?;
            if a.u8()? == 0 {
                continue;
            }
            let count = a.uvar()?;
            let mut all = true;
            for _ in 0..count {
                let state = a.u8()?;
                if state == 0 {
                    all = false;
                    break;
                }
                let v = a.uvar()?;
                if !members.iter().any(|&m| {
                    self.decls[m].variable == v
                        && self.decls[m].replacement_due_to_collision.is_none()
                }) || !used.insert(v)
                {
                    all = false;
                    break;
                }
                match state {
                    1 => {
                        all = false;
                        break;
                    }
                    2 => {}
                    _ => return Err("Unable to cast the value of a deconstruction to LdLoc".into()),
                }
                if !matches!(self.host.variable(v)?.kind, STACK_SLOT | LOCAL) {
                    all = false;
                    break;
                }
            }
            if !all {
                continue;
            }
            let a = self
                .host
                .ask(QUERY_DV_DESIGNATION, &[u64::from(deconstruct)])?;
            let designation = self.host.subtree(tree, a)?;
            let declaration = tree.new_node(kind::DECLARATION_EXPRESSION);
            let var = simple_type(tree, "var");
            tree.set_child(declaration, declaration_expression::slot::TYPE, Some(var));
            tree.set_child(
                declaration,
                declaration_expression::slot::DESIGNATION,
                Some(designation),
            );
            replace_with(tree, left, declaration)?;
            for v in &used {
                let i = self.dict[v];
                self.decls[i].declared_in_deconstruction = true;
            }
        }
        Ok(())
    }

    /// `IsMatchingAssignment`: the assignment `v = expr` at the insertion point.
    fn matching_assignment(
        &mut self,
        tree: &SyntaxTree,
        i: usize,
    ) -> Result<Option<SyntaxId>, String> {
        let next = self.decls[i].insertion_point.node;
        let assignment = if tree.kind(next) == kind::ASSIGNMENT_EXPRESSION {
            next
        } else if tree.kind(next) == kind::EXPRESSION_STATEMENT
            && let Some(e) = tree.child(next, expression_statement::slot::EXPRESSION)
            && tree.kind(e) == kind::ASSIGNMENT_EXPRESSION
        {
            e
        } else {
            return Ok(None);
        };
        if int_scalar(tree, assignment, assignment_expression::scalar::OPERATOR) != ASSIGN {
            return Ok(None);
        }
        let Some(left) = tree.child(assignment, assignment_expression::slot::LEFT) else {
            return Ok(None);
        };
        if tree.kind(left) != kind::IDENTIFIER_EXPRESSION
            || !tree
                .children(left, identifier_expression::slot::TYPE_ARGUMENTS)
                .is_empty()
        {
            return Ok(None);
        }
        let ident = tree
            .child(left, identifier_expression::slot::IDENTIFIER_TOKEN)
            .and_then(|t| tree.node(t).scalars[identifier::scalar::NAME].as_str())
            .unwrap_or_default();
        Ok((ident == self.name(i)?).then_some(assignment))
    }

    fn combine_declaration_and_initializer(
        &mut self,
        tree: &SyntaxTree,
        i: usize,
    ) -> Result<bool, String> {
        if self.host.variable(self.decls[i].variable)?.flags & IS_BY_REF_LIKE != 0 {
            return Ok(true);
        }
        let next = self.decls[i].insertion_point.node;
        if tree.node(next).parent.map(|p| tree.kind(p)) == Some(kind::LAMBDA_EXPRESSION) {
            // An expression-bodied lambda's body: the separate declaration turns it into a block.
            return Ok(false);
        }
        if slot_kind(tree, next) == Some("ForInitializer") {
            return Ok(true);
        }
        Ok(!self.host.setting(SEPARATE_LOCAL_VARIABLE_DECLARATIONS)?)
    }

    /// The variable declaration (following merges) `variable` ends up in.
    fn resolve_variable_to_declare(&self, variable: Option<u64>) -> Option<usize> {
        let mut i = *self.dict.get(&variable?)?;
        while let Some(r) = self.decls[i].replacement_due_to_collision {
            i = r;
        }
        Some(i)
    }

    /// `IsReferencedWithinDeclaringCall`.
    fn is_referenced_within_declaring_call(
        &mut self,
        tree: &SyntaxTree,
        dir: SyntaxId,
        i: usize,
    ) -> Result<bool, String> {
        let Some(call) = tree.node(dir).parent else {
            return Ok(false);
        };
        for argument in tree.all_children(call) {
            if argument == dir {
                continue;
            }
            for n in tree.descendants_and_self(argument) {
                if tree.kind(n) == kind::IDENTIFIER_EXPRESSION {
                    let v = self.host.il_variable(tree, n)?;
                    if self.resolve_variable_to_declare(v) == Some(i) {
                        return Ok(true);
                    }
                }
            }
        }
        Ok(false)
    }

    /// `CanBeDeclaredAsOutVariable`: the `out` direction expression of the first use.
    fn can_be_declared_as_out_variable(
        &mut self,
        tree: &SyntaxTree,
        i: usize,
    ) -> Result<Option<SyntaxId>, String> {
        let d = &self.decls[i];
        let Some(dir) = tree.node(d.first_use).parent else {
            return Ok(None);
        };
        if tree.kind(dir) != kind::DIRECTION_EXPRESSION
            || int_scalar(tree, dir, direction_expression::scalar::FIELD_DIRECTION)
                != FIELD_DIRECTION_OUT
        {
            return Ok(None);
        }
        let (first_use, next, init) = (
            d.first_use,
            d.insertion_point.node,
            d.default_initialization,
        );
        if !self.host.setting(OUT_VARIABLES)? || init != 0 {
            return Ok(None);
        }
        let mut node = Some(first_use);
        while let Some(n) = node {
            if slot_kind(tree, n) == Some("EmbeddedStatement") {
                return Ok(None);
            }
            match tree.kind(n) {
                kind::IF_ELSE_STATEMENT | kind::EXPRESSION_STATEMENT => {
                    return Ok((n == next).then_some(dir));
                }
                _ if is_statement(tree, n) => return Ok(None),
                kind::LAMBDA_EXPRESSION => {
                    return Ok(
                        (tree.child(n, lambda_expression::slot::BODY) == Some(next)).then_some(dir)
                    );
                }
                _ => {}
            }
            node = tree.node(n).parent;
        }
        Ok(None)
    }

    fn insert_variable_declarations(&mut self, tree: &mut SyntaxTree) -> Result<(), String> {
        let mut replacements = Vec::new();
        for i in 0..self.decls.len() {
            if self.decls[i].replacement_due_to_collision.is_some()
                || self.decls[i].declared_in_deconstruction
            {
                continue;
            }
            let variable = self.decls[i].variable;
            let facts = self.host.variable(variable)?;
            let anonymous =
                self.host.setting(ANONYMOUS_TYPES)? && facts.flags & CONTAINS_ANONYMOUS_TYPE != 0;
            let assignment = if self.combine_declaration_and_initializer(tree, i)? {
                self.matching_assignment(tree, i)?
            } else {
                None
            };
            if let Some(assignment) = assignment {
                let ty = if anonymous {
                    simple_type(tree, "var")
                } else {
                    self.host.variable_type(tree, variable)?
                };
                if facts.flags & IS_REF_READ_ONLY != 0
                    && tree.kind(ty) == kind::COMPOSED_TYPE
                    && bool_scalar(tree, ty, composed_type::scalar::HAS_REF_SPECIFIER)
                {
                    tree.node_mut(ty).scalars[composed_type::scalar::HAS_READ_ONLY_SPECIFIER] =
                        Scalar::Bool(true);
                }
                if facts.kind == PINNED_LOCAL {
                    let c = tree.new_node(kind::COMMENT);
                    tree.node_mut(c).scalars[comment::scalar::COMMENT_TYPE] =
                        Scalar::Int(COMMENT_MULTI_LINE);
                    tree.node_mut(c).scalars[comment::scalar::CONTENT] =
                        Scalar::Str(to_utf16("pinned"));
                    tree.node_mut(c).parent = Some(ty);
                    tree.node_mut(ty).trailing_trivia.push(c);
                }
                replacements.push(Replacement::Combine {
                    old: self.decls[i].insertion_point.node,
                    assignment,
                    ty,
                    decl: i,
                });
            } else if let Some(dir) = self.can_be_declared_as_out_variable(tree, i)? {
                let is_out_var = anonymous
                    || (self.has_implicitly_typed_out(tree, dir)?
                        && !self.is_referenced_within_declaring_call(tree, dir, i)?);
                let ty = if is_out_var {
                    simple_type(tree, "var")
                } else {
                    self.host.variable_type(tree, variable)?
                };
                let name = if self.host.setting(DISCARDS)?
                    && facts.load_count == 0
                    && facts.store_count == 0
                    && facts.address_count == 1
                {
                    "_".to_owned()
                } else {
                    facts.name.clone()
                };
                let (ovd, var) = out_var_declaration(tree, ty, &name);
                let rr = self.host.variable_annotation(tree, variable);
                tree.node_mut(var).annotations.push(rr);
                self.copy_annotations_from(tree, ovd, dir);
                if is_out_var {
                    let mut kept = Vec::new();
                    for h in tree.node(ovd).annotations.clone() {
                        if !self.host.annotation(h)?.is_resolve_result() {
                            kept.push(h);
                        }
                    }
                    let out_rr = self.host.new_annotation(
                        tree,
                        ANNOTATION_OUT_VAR,
                        variable,
                        Ann::ResolveResult,
                    );
                    kept.push(out_rr);
                    tree.node_mut(ovd).annotations = kept;
                }
                replacements.push(Replacement::Node { old: dir, new: ovd });
            } else {
                self.insert_separate_declaration(tree, i, &facts)?;
            }
        }
        // Performed at the end, so that no node is replaced while a declaration still refers
        // to it.
        for r in replacements {
            match r {
                Replacement::Node { old, new } => replace_with(tree, old, new)?,
                Replacement::Combine {
                    old,
                    assignment,
                    ty,
                    decl,
                } => {
                    let left = tree.child(assignment, assignment_expression::slot::LEFT);
                    let right = tree.child(assignment, assignment_expression::slot::RIGHT);
                    let name = self.name(decl)?;
                    let (vds, init) = variable_declaration(tree, ty, &name, right);
                    let mut annotations = Vec::new();
                    let left_rr = match left {
                        Some(l) => self.host.resolve_result(tree, l)?.map(|(h, _)| h),
                        None => None,
                    };
                    annotations.push(match left_rr {
                        Some(h) => h,
                        None => self.host.new_annotation(
                            tree,
                            ANNOTATION_UNKNOWN_ERROR,
                            0,
                            Ann::ResolveResult,
                        ),
                    });
                    for n in left.into_iter().chain([assignment]) {
                        for h in tree.node(n).annotations.clone() {
                            if !self.host.annotation(h)?.is_resolve_result() {
                                annotations.push(h);
                            }
                        }
                    }
                    tree.node_mut(init).annotations.extend(annotations);
                    let variable = self.decls[decl].variable;
                    if self.host.setting(SCOPED_REF)?
                        && self.host.variable(variable)?.flags & IS_SCOPED != 0
                    {
                        add_scoped(tree, vds);
                    }
                    replace_with(tree, old, vds)?;
                }
            }
        }
        Ok(())
    }

    fn has_implicitly_typed_out(
        &mut self,
        tree: &SyntaxTree,
        dir: SyntaxId,
    ) -> Result<bool, String> {
        for a in self.host.annotations_of(tree, dir)? {
            if let Ann::ImplicitOut = a {
                return Ok(true);
            }
        }
        Ok(false)
    }

    /// `node.CopyAnnotationsFrom(other)`: the annotations, and copies of the trivia.
    fn copy_annotations_from(&mut self, tree: &mut SyntaxTree, node: SyntaxId, other: SyntaxId) {
        let annotations = tree.node(other).annotations.clone();
        tree.node_mut(node).annotations.extend(annotations);
        for leading in [true, false] {
            let list = if leading {
                tree.node(other).leading_trivia.clone()
            } else {
                tree.node(other).trailing_trivia.clone()
            };
            for t in list {
                let c = tree.clone_subtree(t);
                tree.node_mut(c).parent = Some(node);
                if leading {
                    tree.node_mut(node).leading_trivia.push(c);
                } else {
                    tree.node_mut(node).trailing_trivia.push(c);
                }
            }
        }
    }

    fn insert_separate_declaration(
        &mut self,
        tree: &mut SyntaxTree,
        i: usize,
        facts: &Variable,
    ) -> Result<(), String> {
        let variable = self.decls[i].variable;
        let ty = self.host.variable_type(tree, variable)?;
        let initializer = if self.decls[i].default_initialization == NEEDS_DEFAULT_VALUE {
            let d = tree.new_node(kind::DEFAULT_VALUE_EXPRESSION);
            if !self.host.setting(DEFAULT_LITERALS)? {
                let t = tree.clone_subtree(ty);
                tree.set_child(d, default_value_expression::slot::TYPE, Some(t));
            }
            Some(d)
        } else {
            None
        };
        let (vds, init) = variable_declaration(tree, ty, &facts.name, initializer);
        if self.host.setting(SCOPED_REF)? && facts.flags & IS_SCOPED_WITHOUT_INITIALIZER != 0 {
            add_scoped(tree, vds);
        }
        let rr = self.host.variable_annotation(tree, variable);
        tree.node_mut(init).annotations.push(rr);
        let next = self.decls[i].insertion_point.node;
        if let Some(lambda) = tree.node(next).parent
            && tree.kind(lambda) == kind::LAMBDA_EXPRESSION
        {
            let body = tree.child(lambda, lambda_expression::slot::BODY);
            let ret = tree.new_node(kind::RETURN_STATEMENT);
            tree.set_child(ret, return_statement::slot::EXPRESSION, body);
            let block = tree.new_node(kind::BLOCK_STATEMENT);
            tree.insert_child(block, block_statement::slot::STATEMENTS, 0, ret);
            tree.set_child(lambda, lambda_expression::slot::BODY, Some(block));
        }
        if let Some(p) = tree.node(next).parent
            && tree.kind(p) == kind::RETURN_STATEMENT
        {
            self.decls[i].insertion_point = self.decls[i].insertion_point.up(tree)?;
        }
        let next = self.decls[i].insertion_point.node;
        let insertion_parent = tree
            .node(next)
            .parent
            .ok_or("Variable insertion point has no parent.")?;
        if self.decls[i].default_initialization == NEEDS_SKIP_INIT {
            let a = self.host.ask(QUERY_DV_UNSAFE_TYPE, &[0])?;
            let unsafe_type = self.host.subtree(tree, a)?;
            if self.host.setting(OUT_VARIABLES)? {
                let t = tree.clone_subtree(ty);
                let (ovd, var) = out_var_declaration(tree, t, &facts.name);
                let rr = self.host.variable_annotation(tree, variable);
                tree.node_mut(var).annotations.push(rr);
                let stmt = skip_init_statement(tree, unsafe_type, ovd);
                insert_statement_before(tree, insertion_parent, next, stmt)?;
            } else {
                insert_statement_before(tree, insertion_parent, next, vds)?;
                let id = identifier_expression(tree, &facts.name);
                let rr = self.host.variable_annotation(tree, variable);
                tree.node_mut(id).annotations.push(rr);
                let dir = tree.new_node(kind::DIRECTION_EXPRESSION);
                tree.node_mut(dir).scalars[direction_expression::scalar::FIELD_DIRECTION] =
                    Scalar::Int(FIELD_DIRECTION_OUT);
                tree.set_child(dir, direction_expression::slot::EXPRESSION, Some(id));
                let stmt = skip_init_statement(tree, unsafe_type, dir);
                insert_statement_before(tree, insertion_parent, next, stmt)?;
            }
        } else {
            insert_statement_before(tree, insertion_parent, next, vds)?;
        }
        Ok(())
    }

    /// `UpdateAnnotations`: the variables merged into another refer to the one declared.
    fn update_annotations(&mut self, tree: &mut SyntaxTree, root: SyntaxId) -> Result<(), String> {
        for n in tree.descendants_and_self(root).into_iter().skip(1) {
            if !matches!(
                tree.kind(n),
                kind::IDENTIFIER_EXPRESSION | kind::VARIABLE_INITIALIZER
            ) {
                continue;
            }
            let Some(v) = self.host.il_variable(tree, n)? else {
                continue;
            };
            if !variable_needs_declaration(self.host.variable(v)?.kind) {
                continue;
            }
            let mut i = *self.dict.get(&v).ok_or_else(|| {
                "The given key was not present in the dictionary (UpdateAnnotations).".to_owned()
            })?;
            if self.decls[i].replacement_due_to_collision.is_none() {
                continue;
            }
            while let Some(r) = self.decls[i].replacement_due_to_collision {
                i = r;
            }
            let mut kept = Vec::new();
            for h in tree.node(n).annotations.clone() {
                if !matches!(self.host.annotation(h)?, Ann::Variable(_)) {
                    kept.push(h);
                }
            }
            let rr = self.host.variable_annotation(tree, self.decls[i].variable);
            kept.push(rr);
            tree.node_mut(n).annotations = kept;
        }
        Ok(())
    }
}

fn add_scoped(tree: &mut SyntaxTree, vds: SyntaxId) {
    let m = int_scalar(tree, vds, variable_declaration_statement::scalar::MODIFIERS);
    tree.node_mut(vds).scalars[variable_declaration_statement::scalar::MODIFIERS] =
        Scalar::Int(m | MODIFIERS_SCOPED);
}

#[cfg(test)]
mod tests {
    use super::*;

    /// `{ a; { b; } }` with `c` under `b`: the levels and nodes the points refer to.
    fn sample() -> (SyntaxTree, SyntaxId, SyntaxId, SyntaxId, SyntaxId) {
        let mut tree = SyntaxTree::default();
        let outer = tree.new_node(kind::BLOCK_STATEMENT);
        let a = tree.new_node(kind::EXPRESSION_STATEMENT);
        let inner = tree.new_node(kind::BLOCK_STATEMENT);
        let b = tree.new_node(kind::EXPRESSION_STATEMENT);
        let c = identifier_expression(&mut tree, "x");
        tree.set_child(b, expression_statement::slot::EXPRESSION, Some(c));
        tree.insert_child(inner, block_statement::slot::STATEMENTS, 0, b);
        tree.insert_child(outer, block_statement::slot::STATEMENTS, 0, a);
        tree.insert_child(outer, block_statement::slot::STATEMENTS, 1, inner);
        tree.root = Some(outer);
        (tree, a, inner, b, c)
    }

    #[test]
    fn the_common_parent_is_the_first_point_at_the_shared_level() {
        let (tree, a, inner, _, c) = sample();
        let old = InsertionPoint { level: 1, node: a };
        let new = InsertionPoint { level: 3, node: c };
        assert_eq!(
            find_common_parent(&tree, old, new).unwrap(),
            InsertionPoint { level: 1, node: a }
        );
        assert_eq!(
            find_common_parent(&tree, new, old).unwrap(),
            InsertionPoint {
                level: 1,
                node: inner
            }
        );
    }

    #[test]
    fn statements_go_before_the_insertion_point() {
        let (mut tree, a, inner, _, _) = sample();
        let outer = tree.root.unwrap();
        let ty = simple_type(&mut tree, "int");
        let (vds, _) = variable_declaration(&mut tree, ty, "x", None);
        insert_statement_before(&mut tree, outer, inner, vds).unwrap();
        assert_eq!(
            tree.children(outer, block_statement::slot::STATEMENTS),
            &[a, vds, inner]
        );
        assert_eq!(slot_kind(&tree, vds), Some("Statement"));
        assert!(insert_statement_before(&mut tree, vds, a, inner).is_err());
    }

    #[test]
    fn statement_expressions() {
        let mut tree = SyntaxTree::default();
        let unary = tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
        let rewrap = tree.new_node(kind::UNARY_OPERATOR_EXPRESSION);
        tree.node_mut(rewrap).scalars[unary_operator_expression::scalar::OPERATOR] =
            Scalar::Int(13);
        tree.set_child(
            rewrap,
            unary_operator_expression::slot::EXPRESSION,
            Some(unary),
        );
        for (op, valid) in [(1, false), (5, true), (8, true), (9, false), (11, true)] {
            tree.node_mut(unary).scalars[unary_operator_expression::scalar::OPERATOR] =
                Scalar::Int(op);
            assert_eq!(is_valid_in_statement_expression(&tree, unary), valid);
            assert_eq!(is_valid_in_statement_expression(&tree, rewrap), valid);
        }
        let ident = identifier_expression(&mut tree, "@class");
        assert!(!is_valid_in_statement_expression(&tree, ident));
        let token = tree
            .child(ident, identifier_expression::slot::IDENTIFIER_TOKEN)
            .unwrap();
        assert_eq!(
            tree.node(token).scalars[identifier::scalar::NAME].as_str(),
            Some("class".to_owned())
        );
        assert_eq!(
            tree.node(token).scalars[identifier::scalar::IS_VERBATIM],
            Scalar::Bool(true)
        );
    }

    #[test]
    fn a_cloned_subtree_is_detached_and_deep() {
        let (mut tree, _, inner, b, _) = sample();
        let c = tree.new_node(kind::COMMENT);
        tree.node_mut(c).parent = Some(b);
        tree.node_mut(b).leading_trivia.push(c);
        let copy = tree.clone_subtree(inner);
        assert_eq!(tree.node(copy).parent, None);
        let b2 = tree.children(copy, block_statement::slot::STATEMENTS)[0];
        assert_ne!(b2, b);
        assert_eq!(tree.node(b2).parent, Some(copy));
        let c2 = tree.node(b2).leading_trivia[0];
        assert_ne!(c2, c);
        assert_eq!(tree.node(c2).parent, Some(b2));
        assert_eq!(tree.descendants_and_self(copy).len(), 4);
    }
}
