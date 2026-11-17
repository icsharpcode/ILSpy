//! `TransformFieldAndConstructorInitializers`: field, property and event initializers at the start
//! of constructors move to their declarations, `this(...)`/`base(...)` calls become constructor
//! initializers, a suitable single constructor becomes a primary constructor, and implicit
//! constructors are removed.
//!
//! What the transform knows about symbols and annotations it asks the host (queries 112..123;
//! symbols are ids of the host's symbol table, see `syntax_oracle::QUERY_NODE_SYMBOL`):
//! - 112 `CURRENT_TYPE`: the transform context's `CurrentTypeDefinition`, as its id plus one (0 for
//!   none).
//! - 113 `TYPE` (type id): a `TYPE_*` flags byte, the row number of the type's first method
//!   (LEB128), and the record's primary constructor as id plus one.
//! - 114 `METHOD` (symbol id; no answer for a non-method): a `METHOD_*` flags byte, the
//!   `Accessibility` byte, the metadata row number, the parameter count, the declaring type
//!   definition as id plus one.
//! - 115 `RECORD` (type id, method id): `RecordDecompiler.IsCopyConstructor`, a byte.
//! - 116 `BACKING_STORES` (type id, constructor id): per parameter of the constructor (count
//!   first) a presence byte in `RecordDecompiler.GetParameterToBackingStoreMap()`, then the
//!   property and the field as ids plus one.
//! - 117 `MEMBER` (symbol id): a `MEMBER_*` flags byte, the backing field of a property
//!   (`PatternStatementTransform.TryGetBackingField`) as id plus one.
//! - 118 `DEPENDS_ON_PARAMETERS` (function handle plus one, count, handles): whether an
//!   `ILInstruction` among the handles has a descendant using a parameter of the function, a byte.
//! - 119 `RESOLVE_RESULT` (handle; no answer for a non-`ResolveResult`): an `RR_*` flags byte; for
//!   an `ILVariableResolveResult` then the variable's kind byte, its index plus one (0 for none or
//!   negative), its object id and its name as UTF-16 code units.
//! - 120 `DECIMAL_MISMATCH` (constant handle plus one, expression handle plus one; 0 means
//!   `ErrorResolveResult.UnknownError`): whether the constant is a compile-time constant and the
//!   expression a decimal constant of another value, a byte.
//! - 121 `ATTRIBUTES` (entity id, field byte): the attributes of the entity as
//!   `TypeSystemAstBuilder.ConvertAttribute` builds them (for a field without those
//!   `PatternStatementTransform` removes from auto properties), a count and each as a length and a
//!   subtree whose handles are host-built objects (annotation 98).
//! - 122 `VALUES_EQUAL` (handle, handle): `object.Equals` of two literal values, a byte.
//! - 123 `DOCUMENTATION` (method id): with `ShowXmlDocumentation` and a documentation provider,
//!   1 where the method's documentation differs from its type's, 2 where it has any.
//!
//! Annotations it creates: 112 a `MemberInitializerInOtherConstructorsAnnotation` (payload a
//! count and the initializer subtrees in the wire format), 96 a new `ILVariableResolveResult` of a
//! variable object id, 98 a host-built object (both shared with `DeclareVariables`).

use std::collections::{HashMap, HashSet};

use crate::csharp::syntax::{Scalar, SlotValue, SyntaxId, SyntaxTree, uvar};
use crate::csharp::syntax_oracle::{self as q, MemberRole, SyntaxOracle};
use crate::csharp::syntax_schema::{
    accessor, assignment_expression, attribute_section, block_statement, cast_expression,
    constructor_declaration, constructor_initializer, event_declaration, expression_statement,
    field_declaration, identifier, identifier_expression, invocation_ast_type,
    invocation_expression, kind, member_reference_expression, object_create_expression,
    parameter_declaration, property_declaration, type_declaration, variable_initializer,
};
use crate::csharp::transforms::introduce_unsafe_modifier::is_unsafe;
use crate::typesystem::types::TypeSystem;

const QUERY_CURRENT_TYPE: u8 = 112;
const QUERY_TYPE: u8 = 113;
const QUERY_METHOD: u8 = 114;
const QUERY_RECORD: u8 = 115;
const QUERY_BACKING_STORES: u8 = 116;
const QUERY_MEMBER: u8 = 117;
const QUERY_DEPENDS_ON_PARAMETERS: u8 = 118;
const QUERY_RESOLVE_RESULT: u8 = 119;
const QUERY_DECIMAL_MISMATCH: u8 = 120;
const QUERY_ATTRIBUTES: u8 = 121;
const QUERY_VALUES_EQUAL: u8 = 122;
const QUERY_DOCUMENTATION: u8 = 123;

const ANNOTATION_OTHER_CONSTRUCTORS: u8 = 112;
const ANNOTATION_VARIABLE: u8 = 96;
const ANNOTATION_MINTED: u8 = 98;

const TYPE_STRUCT: u8 = 1;
const TYPE_ABSTRACT: u8 = 2;
const TYPE_RECORD: u8 = 4;
const TYPE_BEFORE_FIELD_INIT: u8 = 8;
const TYPE_HAS_RECORD_DECOMPILER: u8 = 16;

const METHOD_STATIC: u8 = 1;
const METHOD_CONSTRUCTOR: u8 = 2;
const METHOD_DECLARING_TYPE_STRUCT: u8 = 4;

const MEMBER_FIELD: u8 = 1;
const MEMBER_CONST: u8 = 2;
const MEMBER_GENERATED_PRIMARY_CONSTRUCTOR_BACKING_FIELD: u8 = 4;
const MEMBER_DECLARING_TYPE_STRUCT: u8 = 8;

const RR_REFERENCES_INSTANCE_MEMBER: u8 = 1;
const RR_MEMBER_RESOLVE_RESULT: u8 = 2;
const RR_DEFAULT_VALUE_CONSTANT: u8 = 4;
const RR_VARIABLE: u8 = 8;

const DOC_DIFFERS_FROM_TYPE: u8 = 1;
const DOC_PRESENT: u8 = 2;

// Syntax and type system enum values.
const ASSIGN: i64 = 0;
const MODIFIERS_PROTECTED: i64 = 0x0004;
const MODIFIERS_PUBLIC: i64 = 0x0008;
const MODIFIERS_UNSAFE: i64 = 0x8000;
const INITIALIZER_BASE: i64 = 1;
const INITIALIZER_THIS: i64 = 2;
const ACCESSIBILITY_PROTECTED: u8 = 3;
const ACCESSIBILITY_PUBLIC: u8 = 6;
const VARIABLE_KIND_PARAMETER: u8 = 6;

/// The built-in AST transform `TransformFieldAndConstructorInitializers`.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let setting = |name| q::setting(oracle, name).unwrap_or(false);
    let mut t = Transform {
        oracle,
        always_move_initializer: setting("AlwaysMoveInitializer"),
        field_keyword: setting("FieldKeyword"),
        primary_constructors_for_non_records: setting(
            "UsePrimaryConstructorSyntaxForNonRecordTypes",
        ),
        struct_field_initializers: setting("StructDefaultConstructorsAndFieldInitializers"),
        created_variables: HashMap::new(),
    };
    let current = t
        .ask(QUERY_CURRENT_TYPE, &[0])
        .and_then(|mut a| a.id_plus_one());
    if let Some(current) = current {
        let members: Vec<SyntaxId> = tree
            .all_children(root)
            .into_iter()
            .filter(|&c| is_entity_declaration(tree.kind(c)))
            .collect();
        let declaration = (tree.kind(root) == kind::TYPE_DECLARATION).then_some(root);
        t.transform_declaration(tree, current, root, declaration, &members)?;
    }
    let declarations: Vec<SyntaxId> = tree
        .descendants_and_self(root)
        .into_iter()
        .skip(1)
        .filter(|&n| tree.kind(n) == kind::TYPE_DECLARATION)
        .collect();
    for declaration in declarations {
        let definition = t
            .symbol(tree, declaration)
            .ok_or("a type declaration without type definition")?;
        let members = tree
            .children(declaration, type_declaration::slot::MEMBERS)
            .to_vec();
        t.transform_declaration(tree, definition, declaration, Some(declaration), &members)?;
    }
    Ok(())
}

#[derive(Clone, Copy, PartialEq, Eq)]
enum InitializerKind {
    Static,
    Instance,
    Primary,
}

/// An `ILVariable` of an `ILVariableResolveResult`.
#[derive(Clone, Debug)]
struct Variable {
    kind: u8,
    index: Option<usize>,
    id: u64,
    name: Vec<u16>,
}

struct ResolveResult {
    flags: u8,
    variable: Option<Variable>,
}

struct TypeFacts {
    flags: u8,
    first_method_row: u64,
    record_primary_constructor: Option<u32>,
}

struct MethodFacts {
    flags: u8,
    accessibility: u8,
    row: u64,
    parameter_count: usize,
    declaring_type: Option<u32>,
}

struct InitializerStatement {
    statement: SyntaxId,
    member: u32,
    initializer: SyntaxId,
    depends_on_constructor_body: bool,
}

/// `InitializerSequence`.
#[derive(Default)]
struct Sequence {
    statements: Vec<InitializerStatement>,
    /// `StatementToOtherCtorsMap`: per lifted statement, its copies in the other constructors and
    /// their initializers.
    other_constructors: HashMap<SyntaxId, Vec<(SyntaxId, SyntaxId)>>,
    has_duplicate_assignments: bool,
    is_unsafe: bool,
    covers_full_body: bool,
}

/// The property and the field a primary constructor parameter stores to, by parameter index.
type BackingStores = HashMap<usize, (Option<u32>, Option<u32>)>;

/// `ConstructorInitializerAnalyzer`.
struct Analysis {
    type_definition: u32,
    facts: TypeFacts,
    declaration: Option<SyntaxId>,
    member_to_declaration: HashMap<u32, SyntaxId>,
    /// `PrimaryConstructorParameterToBackingStoreMap`, by parameter index.
    parameter_to_backing_store: Option<BackingStores>,
    backing_field_to_variable: Option<HashMap<u32, Variable>>,
    instance_initializers: Option<Sequence>,
    static_initializers: Option<Sequence>,
    primary_initializers: Option<Sequence>,
    static_constructor: Option<SyntaxId>,
    primary_constructor: Option<(u32, SyntaxId)>,
    instance_constructors: Vec<SyntaxId>,
}

/// An answer of the host, read front to back.
struct Answer {
    bytes: Vec<u8>,
    pos: usize,
}

impl Answer {
    fn byte(&mut self) -> Option<u8> {
        let b = *self.bytes.get(self.pos)?;
        self.pos += 1;
        Some(b)
    }

    fn uvar(&mut self) -> Option<u64> {
        let mut v = 0u64;
        let mut shift = 0;
        loop {
            let b = self.byte()?;
            v |= u64::from(b & 0x7F) << shift;
            if b & 0x80 == 0 {
                return Some(v);
            }
            shift += 7;
            if shift > 63 {
                return None;
            }
        }
    }

    /// An id written plus one, 0 for none.
    fn id_plus_one(&mut self) -> Option<u32> {
        self.uvar()?.checked_sub(1).map(|v| v as u32)
    }

    fn utf16_rest(&mut self) -> Vec<u16> {
        let rest = &self.bytes[self.pos..];
        self.pos = self.bytes.len();
        rest.as_chunks::<2>()
            .0
            .iter()
            .map(|c| u16::from_le_bytes(*c))
            .collect()
    }
}

struct Transform<'o> {
    oracle: &'o dyn SyntaxOracle,
    always_move_initializer: bool,
    field_keyword: bool,
    primary_constructors_for_non_records: bool,
    struct_field_initializers: bool,
    /// The `ILVariableResolveResult`s this transform created, by handle.
    created_variables: HashMap<u32, Variable>,
}

impl Transform<'_> {
    fn ask(&self, kind: u8, args: &[u64]) -> Option<Answer> {
        let mut query = vec![kind];
        for &a in args {
            uvar(&mut query, a);
        }
        self.oracle
            .ask(&query)
            .map(|bytes| Answer { bytes, pos: 0 })
    }

    /// `AstNode.GetSymbol`: the symbol of the node's first `ResolveResult` annotation.
    fn symbol(&self, tree: &SyntaxTree, id: SyntaxId) -> Option<u32> {
        for &h in &tree.node(id).annotations {
            if h >= tree.host_handles {
                // shortcut: the annotations this transform created are no host objects yet, and
                // only its variables are known as resolve results; upgrade if another carries a symbol.
                if self.created_variables.contains_key(&h) {
                    return None;
                }
                continue;
            }
            if let Some(symbol) = q::node_symbol(self.oracle, h) {
                return symbol;
            }
        }
        None
    }

    /// The facts of the host's `ResolveResult` `h`, `None` for any other object.
    fn resolve_result_of_handle(&self, tree: &SyntaxTree, h: u32) -> Option<ResolveResult> {
        if h >= tree.host_handles {
            return self.created_variables.get(&h).map(|v| ResolveResult {
                flags: RR_VARIABLE,
                variable: Some(v.clone()),
            });
        }
        let mut a = self.ask(QUERY_RESOLVE_RESULT, &[u64::from(h)])?;
        let flags = a.byte()?;
        let variable = if flags & RR_VARIABLE != 0 {
            let kind = a.byte()?;
            let index = a.uvar()?.checked_sub(1).map(|i| i as usize);
            let id = a.uvar()?;
            Some(Variable {
                kind,
                index,
                id,
                name: a.utf16_rest(),
            })
        } else {
            None
        };
        Some(ResolveResult { flags, variable })
    }

    /// `AstNode.Annotation<ResolveResult>()`: the handle and facts of the first.
    fn resolve_result(&self, tree: &SyntaxTree, id: SyntaxId) -> Option<(u32, ResolveResult)> {
        tree.node(id)
            .annotations
            .iter()
            .find_map(|&h| self.resolve_result_of_handle(tree, h).map(|rr| (h, rr)))
    }

    /// `Annotation<ILVariableResolveResult>()?.Variable`.
    fn variable(&self, tree: &SyntaxTree, id: SyntaxId) -> Option<Variable> {
        tree.node(id)
            .annotations
            .iter()
            .find_map(|&h| self.resolve_result_of_handle(tree, h)?.variable)
    }

    fn member_facts(&self, symbol: u32) -> Option<q::MemberFacts> {
        q::member_facts(self.oracle, symbol)
    }

    /// `(node.GetSymbol() as IMember)?.MemberDefinition`.
    fn member_definition(&self, tree: &SyntaxTree, id: SyntaxId) -> Option<u32> {
        let facts = self.member_facts(self.symbol(tree, id)?)?;
        (facts.role != MemberRole::Other).then_some(facts.member_definition)
    }

    fn member_flags(&self, member: u32) -> (u8, Option<u32>) {
        self.ask(QUERY_MEMBER, &[u64::from(member)])
            .and_then(|mut a| Some((a.byte()?, a.id_plus_one())))
            .unwrap_or((0, None))
    }

    fn type_facts(&self, definition: u32) -> Result<TypeFacts, String> {
        let mut a = self
            .ask(QUERY_TYPE, &[u64::from(definition)])
            .ok_or("the host knows no such type definition")?;
        let short = || "a short type answer".to_owned();
        Ok(TypeFacts {
            flags: a.byte().ok_or_else(short)?,
            first_method_row: a.uvar().ok_or_else(short)?,
            record_primary_constructor: a.id_plus_one(),
        })
    }

    fn method_facts(&self, method: u32) -> Option<MethodFacts> {
        let mut a = self.ask(QUERY_METHOD, &[u64::from(method)])?;
        Some(MethodFacts {
            flags: a.byte()?,
            accessibility: a.byte()?,
            row: a.uvar()?,
            parameter_count: a.uvar()? as usize,
            declaring_type: a.id_plus_one(),
        })
    }

    /// The symbol and facts of the method a declaration declares (`(IMethod)GetSymbol()`).
    fn declared_method(
        &self,
        tree: &SyntaxTree,
        declaration: SyntaxId,
    ) -> Result<(u32, MethodFacts), String> {
        let symbol = self
            .symbol(tree, declaration)
            .ok_or("a constructor declaration without method")?;
        let facts = self
            .method_facts(symbol)
            .ok_or("a constructor declaration whose symbol is no method")?;
        Ok((symbol, facts))
    }

    fn documentation(&self, method: u32) -> u8 {
        self.ask(QUERY_DOCUMENTATION, &[u64::from(method)])
            .and_then(|mut a| a.byte())
            .unwrap_or(0)
    }

    fn values_equal(&self, a: u32, b: u32) -> bool {
        a == b
            || self
                .ask(QUERY_VALUES_EQUAL, &[u64::from(a), u64::from(b)])
                .and_then(|mut x| x.byte())
                == Some(1)
    }

    fn transform_declaration(
        &mut self,
        tree: &mut SyntaxTree,
        definition: u32,
        node: SyntaxId,
        declaration: Option<SyntaxId>,
        members: &[SyntaxId],
    ) -> Result<(), String> {
        let mut a = Analysis {
            type_definition: definition,
            facts: self.type_facts(definition)?,
            declaration,
            member_to_declaration: HashMap::new(),
            parameter_to_backing_store: None,
            backing_field_to_variable: None,
            instance_initializers: None,
            static_initializers: None,
            primary_initializers: None,
            static_constructor: None,
            primary_constructor: None,
            instance_constructors: Vec::new(),
        };
        if !self.analyze(tree, &mut a, members)? {
            return Ok(());
        }

        fn usable(s: &Option<Sequence>) -> Option<&Sequence> {
            s.as_ref().filter(|s| !s.has_duplicate_assignments)
        }
        if let Some(s) = usable(&a.primary_initializers) {
            self.move_field_initializers(tree, &a, s, InitializerKind::Primary)?;
        } else if let Some(s) = usable(&a.instance_initializers) {
            self.move_field_initializers(tree, &a, s, InitializerKind::Instance)?;
        }
        if let Some(s) = usable(&a.static_initializers) {
            self.move_field_initializers(tree, &a, s, InitializerKind::Static)?;
        }

        for &ctor in members {
            if tree.kind(ctor) == kind::CONSTRUCTOR_DECLARATION {
                self.move_constructor_initializer(tree, ctor)?;
            }
        }

        self.remove_implicit_constructor(tree, &a)?;

        let Some(backing_fields) = a.backing_field_to_variable.as_ref() else {
            return Ok(());
        };
        let identifiers: Vec<SyntaxId> = tree
            .descendants_and_self(node)
            .into_iter()
            .skip(1)
            .filter(|&n| tree.kind(n) == kind::IDENTIFIER)
            .collect();
        for ident in identifiers {
            let Some(parent) = tree.node(ident).parent else {
                continue;
            };
            let Some(symbol) = self.symbol(tree, parent) else {
                continue;
            };
            let Some(facts) = self.member_facts(symbol) else {
                continue;
            };
            if facts.role != MemberRole::Field {
                continue;
            }
            let Some(v) = backing_fields.get(&facts.member_definition) else {
                continue;
            };
            let kept: Vec<u32> = tree
                .node(parent)
                .annotations
                .iter()
                .copied()
                .filter(|&h| {
                    self.resolve_result_of_handle(tree, h)
                        .is_none_or(|rr| rr.flags & RR_MEMBER_RESOLVE_RESULT == 0)
                })
                .collect();
            let mut payload = Vec::new();
            uvar(&mut payload, v.id);
            let rr = tree.new_annotation(ANNOTATION_VARIABLE, payload);
            self.created_variables.insert(rr, v.clone());
            tree.node_mut(parent).annotations = kept;
            tree.node_mut(parent).annotations.push(rr);
            let replacement = new_identifier(tree, &v.name);
            tree.replace(ident, replacement);
        }
        Ok(())
    }

    /// `ConstructorInitializerAnalyzer.Analyze`.
    fn analyze(
        &self,
        tree: &SyntaxTree,
        a: &mut Analysis,
        members: &[SyntaxId],
    ) -> Result<bool, String> {
        for &m in members {
            if let Some(symbol) = self.symbol(tree, m)
                && self
                    .member_facts(symbol)
                    .is_some_and(|f| f.role != MemberRole::Other)
            {
                a.member_to_declaration.insert(symbol, m);
            }
        }

        if self.field_keyword {
            // Constructor stores to the backing field of a field-backed property become the
            // property's initializer.
            for &pd in members {
                if tree.kind(pd) != kind::PROPERTY_DECLARATION {
                    continue;
                }
                let Some(symbol) = self.symbol(tree, pd) else {
                    continue;
                };
                if self
                    .member_facts(symbol)
                    .is_none_or(|f| f.role != MemberRole::Property)
                {
                    continue;
                }
                if let (_, Some(backing_field)) = self.member_flags(symbol) {
                    a.member_to_declaration.entry(backing_field).or_insert(pd);
                }
            }
        }

        let is_struct_type = a.facts.flags & TYPE_STRUCT != 0;
        let has_record = a.facts.flags & TYPE_HAS_RECORD_DECOMPILER != 0;
        let mut not_chained_with_this = Vec::new();
        let mut all_constructors = Vec::new();

        for &ctor in members {
            if tree.kind(ctor) != kind::CONSTRUCTOR_DECLARATION {
                continue;
            }
            let (method, facts) = self.declared_method(tree, ctor)?;
            if Some(method) == a.facts.record_primary_constructor {
                a.primary_constructor = Some((method, ctor));
                a.parameter_to_backing_store =
                    Some(self.record_backing_stores(a.type_definition, method));
                not_chained_with_this.push(ctor);
            } else if facts.flags & METHOD_STATIC != 0 {
                a.static_constructor = Some(ctor);
            } else if has_record
                && self
                    .ask(
                        QUERY_RECORD,
                        &[u64::from(a.type_definition), u64::from(method)],
                    )
                    .and_then(|mut x| x.byte())
                    == Some(1)
            {
                // The record's copy constructor keeps its body.
            } else {
                let statement = first_non_empty_statement(tree, ctor);
                let this_call = statement.and_then(|s| {
                    match_this_call(tree, s, facts.flags & METHOD_DECLARING_TYPE_STRUCT != 0)
                });
                all_constructors.push(ctor);
                if this_call
                    .is_some_and(|(target, _)| tree.kind(target) == kind::THIS_REFERENCE_EXPRESSION)
                {
                    continue;
                }
                not_chained_with_this.push(ctor);
            }
        }

        let expected_accessibility = if a.facts.flags & TYPE_ABSTRACT != 0 {
            ACCESSIBILITY_PROTECTED
        } else {
            ACCESSIBILITY_PUBLIC
        };

        if self.primary_constructors_for_non_records
            && !has_record
            && not_chained_with_this.len() == 1
        {
            let ctor = not_chained_with_this[0];
            let (method, facts) = self.declared_method(tree, ctor)?;
            let initializer = self.analyze_sequence(tree, a, ctor, &facts)?;
            if initializer.covers_full_body
                && !initializer.has_duplicate_assignments
                && !initializer.statements.is_empty()
                && !initializer
                    .statements
                    .iter()
                    .any(|s| self.references_instance_member(tree, s.initializer))
                && facts.accessibility == expected_accessibility
            {
                let mut transform = facts.row == a.facts.first_method_row;
                if facts.parameter_count == 0 {
                    transform = false;
                }
                for s in &initializer.statements {
                    let (flags, _) = self.member_flags(s.member);
                    if flags & MEMBER_FIELD == 0
                        || flags & MEMBER_GENERATED_PRIMARY_CONSTRUCTOR_BACKING_FIELD == 0
                    {
                        continue;
                    }
                    if let Some(v) = self.variable(tree, s.initializer)
                        && v.kind == VARIABLE_KIND_PARAMETER
                        && let Some(index) = v.index
                    {
                        a.parameter_to_backing_store
                            .get_or_insert_default()
                            .insert(index, (None, Some(s.member)));
                        a.backing_field_to_variable
                            .get_or_insert_default()
                            .insert(s.member, v);
                        transform = true;
                    }
                }
                if self.documentation(method) & DOC_DIFFERS_FROM_TYPE != 0 {
                    transform = false;
                }
                if transform {
                    a.parameter_to_backing_store.get_or_insert_default();
                    a.primary_constructor = Some((method, ctor));
                    a.primary_initializers = Some(initializer);
                }
            }
        }

        if let Some(ctor) = a.static_constructor {
            let (_, facts) = self.declared_method(tree, ctor)?;
            a.static_initializers = Some(self.analyze_sequence(tree, a, ctor, &facts)?);
        }

        if let Some((_, ctor)) = a.primary_constructor
            && a.primary_initializers.is_none()
        {
            let (_, facts) = self.declared_method(tree, ctor)?;
            a.primary_initializers = Some(self.analyze_sequence(tree, a, ctor, &facts)?);
        }

        if !not_chained_with_this.is_empty()
            && (!is_struct_type
                || (self.struct_field_initializers && a.facts.flags & TYPE_RECORD == 0))
        {
            let first = not_chained_with_this[0];
            let is_primary = a.primary_constructor.map(|(_, d)| d) == Some(first);
            let mut sequence = if is_primary {
                a.primary_initializers
                    .take()
                    .ok_or("a primary constructor without initializers")?
            } else {
                let (_, facts) = self.declared_method(tree, first)?;
                self.analyze_sequence(tree, a, first, &facts)?
            };
            let mut matches_all = true;
            for &other in &not_chained_with_this[1..] {
                if !self.sequence_matches(tree, &mut sequence, other) {
                    matches_all = false;
                    break;
                }
            }
            if is_primary {
                a.primary_initializers = Some(sequence);
                if !matches_all {
                    return Ok(false);
                }
            } else if matches_all
                && !sequence
                    .statements
                    .iter()
                    .any(|s| s.depends_on_constructor_body)
            {
                a.instance_initializers = Some(sequence);
            }
        }

        a.instance_constructors = all_constructors;
        Ok(true)
    }

    fn record_backing_stores(&self, definition: u32, ctor: u32) -> BackingStores {
        let mut map = HashMap::new();
        let Some(mut x) = self.ask(
            QUERY_BACKING_STORES,
            &[u64::from(definition), u64::from(ctor)],
        ) else {
            return map;
        };
        let count = x.uvar().unwrap_or(0) as usize;
        for i in 0..count {
            if x.byte() == Some(1) {
                let property = x.id_plus_one();
                let field = x.id_plus_one();
                map.insert(i, (property, field));
            }
        }
        map
    }

    /// The local `ReferencesInstanceMember` of `Analyze`.
    fn references_instance_member(&self, tree: &SyntaxTree, expression: SyntaxId) -> bool {
        tree.descendants_and_self(expression)
            .into_iter()
            .filter(|&n| tree.node(n).schema().name.ends_with("Expression"))
            .any(|n| {
                self.resolve_result(tree, n)
                    .is_some_and(|(_, rr)| rr.flags & RR_REFERENCES_INSTANCE_MEMBER != 0)
            })
    }

    /// `InitializerSequence.Analyze`.
    fn analyze_sequence(
        &self,
        tree: &SyntaxTree,
        a: &Analysis,
        ctor: SyntaxId,
        facts: &MethodFacts,
    ) -> Result<Sequence, String> {
        let mut sequence = Sequence {
            is_unsafe: has_modifier(tree, ctor, MODIFIERS_UNSAFE),
            ..Sequence::default()
        };
        let mut initialized = HashSet::new();
        let function = tree.node(ctor).annotations.iter().copied().find(|&h| {
            h < tree.host_handles && matches!(q::il_instruction(self.oracle, h), Some(Some(_)))
        });
        let is_struct = facts.flags & METHOD_DECLARING_TYPE_STRUCT != 0;
        let only_move_constants = !self.always_move_initializer
            && a.facts.flags & TYPE_BEFORE_FIELD_INIT == 0
            && facts.flags & METHOD_STATIC != 0;
        let mut skipped = false;

        let mut statement = first_non_empty_statement(tree, ctor);
        while let Some(s) = statement {
            let Some((field_access, initializer)) = match_member_initializer(tree, s) else {
                break;
            };
            let Some(member) = self.member_definition(tree, field_access) else {
                break;
            };
            if !self.can_have_initializer(tree, a, member) {
                break;
            }
            if only_move_constants {
                let (flags, _) = self.member_flags(member);
                if flags & (MEMBER_FIELD | MEMBER_CONST) != MEMBER_FIELD | MEMBER_CONST {
                    skipped = true;
                    statement = next_non_empty_statement(tree, s);
                    continue;
                }
            }
            if !initialized.insert(member) {
                sequence.has_duplicate_assignments = true;
            }
            let depends = self.depends_on_parameters(tree, initializer, function);
            sequence.statements.push(InitializerStatement {
                statement: s,
                member,
                initializer,
                depends_on_constructor_body: depends,
            });
            statement = next_non_empty_statement(tree, s);
        }

        if !skipped {
            match statement {
                None => sequence.covers_full_body = true,
                Some(s) => {
                    if match_this_call(tree, s, is_struct).is_some() {
                        sequence.covers_full_body = next_non_empty_statement(tree, s).is_none();
                    }
                }
            }
        }
        Ok(sequence)
    }

    fn depends_on_parameters(
        &self,
        tree: &SyntaxTree,
        initializer: SyntaxId,
        function: Option<u32>,
    ) -> bool {
        let handles: Vec<u64> = tree
            .node(initializer)
            .annotations
            .iter()
            .filter(|&&h| h < tree.host_handles)
            .map(|&h| u64::from(h))
            .collect();
        let mut args = vec![
            function.map_or(0, |f| u64::from(f) + 1),
            handles.len() as u64,
        ];
        args.extend(handles);
        self.ask(QUERY_DEPENDS_ON_PARAMETERS, &args)
            .and_then(|mut x| x.byte())
            == Some(1)
    }

    /// `InitializerSequence.CanHaveInitializer`.
    fn can_have_initializer(&self, tree: &SyntaxTree, a: &Analysis, member: u32) -> bool {
        let Some(&declaration) = a.member_to_declaration.get(&member) else {
            return true;
        };
        match tree.kind(declaration) {
            kind::FIELD_DECLARATION | kind::EVENT_DECLARATION => true,
            kind::PROPERTY_DECLARATION => {
                is_automatic_property(tree, declaration)
                    || self.member_flags(member).0 & MEMBER_FIELD != 0
            }
            _ => false,
        }
    }

    /// `InitializerSequence.IsMatch`.
    fn sequence_matches(&self, tree: &SyntaxTree, sequence: &mut Sequence, ctor: SyntaxId) -> bool {
        if tree
            .child(ctor, constructor_declaration::slot::BODY)
            .is_none()
        {
            return false;
        }
        let mut other = first_non_empty_statement(tree, ctor);
        for entry in &sequence.statements {
            let Some(other_statement) = other else {
                return false;
            };
            let Some((field_access, other_initializer)) =
                match_member_initializer(tree, other_statement)
            else {
                return false;
            };
            if self.member_definition(tree, field_access) != Some(entry.member) {
                return false;
            }
            if !self.same_syntax(tree, entry.initializer, other_initializer) {
                return false;
            }
            sequence
                .other_constructors
                .entry(entry.statement)
                .or_default()
                .push((other_statement, other_initializer));
            other = next_non_empty_statement(tree, other_statement);
        }
        true
    }

    /// `AstNode.IsMatch` of two trees without pattern nodes: same kinds, scalars and children,
    /// ignoring annotations, trivia, locations and `Identifier.IsVerbatim`; a literal compares
    /// only its value.
    fn same_syntax(&self, tree: &SyntaxTree, a: SyntaxId, b: SyntaxId) -> bool {
        let (na, nb) = (tree.node(a), tree.node(b));
        if na.kind != nb.kind {
            return false;
        }
        let schema = na.schema();
        for (i, s) in schema.scalars.iter().enumerate() {
            if na.kind == kind::PRIMITIVE_EXPRESSION && s.name != "Value"
                || na.kind == kind::IDENTIFIER && s.name == "IsVerbatim"
                || s.ty == "TextLocation"
            {
                continue;
            }
            let equal = match (&na.scalars[i], &nb.scalars[i]) {
                (Scalar::Object(x), Scalar::Object(y)) => self.values_equal(*x, *y),
                (Scalar::Str(x), Scalar::NullStr) | (Scalar::NullStr, Scalar::Str(x)) => {
                    x.is_empty()
                }
                (x, y) => x == y,
            };
            if !equal {
                return false;
            }
        }
        na.slots.iter().zip(&nb.slots).all(|pair| match pair {
            (SlotValue::Single(None), SlotValue::Single(None)) => true,
            (SlotValue::Single(Some(x)), SlotValue::Single(Some(y))) => {
                self.same_syntax(tree, *x, *y)
            }
            (SlotValue::Many(xs), SlotValue::Many(ys)) => {
                xs.len() == ys.len()
                    && xs
                        .iter()
                        .zip(ys)
                        .all(|(x, y)| self.same_syntax(tree, *x, *y))
            }
            _ => false,
        })
    }

    /// `InitializerSequence.IsDefaultValueInitializer`.
    fn is_default_value_initializer(&self, tree: &SyntaxTree, initializer: SyntaxId) -> bool {
        tree.kind(initializer) == kind::DEFAULT_VALUE_EXPRESSION
            || self
                .resolve_result(tree, initializer)
                .is_some_and(|(_, rr)| rr.flags & RR_DEFAULT_VALUE_CONSTANT != 0)
    }

    /// `MoveFieldInitializersToDeclarations`.
    fn move_field_initializers(
        &self,
        tree: &mut SyntaxTree,
        a: &Analysis,
        sequence: &Sequence,
        initializer_kind: InitializerKind,
    ) -> Result<(), String> {
        for entry in &sequence.statements {
            let initializer = entry.initializer;
            let Some(&declaration) = a.member_to_declaration.get(&entry.member) else {
                // A primary-constructor parameter assignment without a separate declaration is
                // redundant; otherwise the member is outside this (partial) tree and the
                // assignment stays.
                if initializer_kind == InitializerKind::Primary {
                    tree.detach(entry.statement);
                }
                continue;
            };
            match tree.kind(declaration) {
                kind::FIELD_DECLARATION => {
                    let v = single(tree.children(declaration, field_declaration::slot::VARIABLES))?;
                    let existing = tree.child(v, variable_initializer::slot::INITIALIZER);
                    match existing {
                        None => tree.set_child(
                            v,
                            variable_initializer::slot::INITIALIZER,
                            Some(initializer),
                        ),
                        Some(existing) if initializer_kind == InitializerKind::Static => {
                            // A decimal constant already has its initializer; a different value
                            // keeps the assignment.
                            let constant = self.resolve_result(tree, existing).map(|(h, _)| h);
                            let expression = self.resolve_result(tree, initializer).map(|(h, _)| h);
                            let plus_one = |h: Option<u32>| h.map_or(0, |h| u64::from(h) + 1);
                            if self
                                .ask(
                                    QUERY_DECIMAL_MISMATCH,
                                    &[plus_one(constant), plus_one(expression)],
                                )
                                .and_then(|mut x| x.byte())
                                == Some(1)
                            {
                                continue;
                            }
                        }
                        Some(_) => {}
                    }
                }
                kind::PROPERTY_DECLARATION => {
                    let (flags, _) = self.member_flags(entry.member);
                    let struct_field = flags & (MEMBER_FIELD | MEMBER_DECLARING_TYPE_STRUCT)
                        == MEMBER_FIELD | MEMBER_DECLARING_TYPE_STRUCT;
                    if struct_field && self.is_default_value_initializer(tree, initializer) {
                        // Recompilation regenerates the zero-initialization of an auto-default
                        // struct's backing fields, so the store is dropped.
                    } else if tree
                        .child(declaration, property_declaration::slot::INITIALIZER)
                        .is_none()
                    {
                        tree.set_child(
                            declaration,
                            property_declaration::slot::INITIALIZER,
                            Some(initializer),
                        );
                    }
                }
                kind::EVENT_DECLARATION => {
                    let v = single(tree.children(declaration, event_declaration::slot::VARIABLES))?;
                    if tree
                        .child(v, variable_initializer::slot::INITIALIZER)
                        .is_none()
                    {
                        tree.set_child(
                            v,
                            variable_initializer::slot::INITIALIZER,
                            Some(initializer),
                        );
                    }
                }
                _ => continue,
            }
            tree.detach(entry.statement);

            if let Some(others) = sequence.other_constructors.get(&entry.statement) {
                for &(other_statement, _) in others {
                    tree.detach(other_statement);
                }
                // The copies let each constructor that runs the initializer keep its breakpoint.
                if !others.is_empty() {
                    let mut payload = Vec::new();
                    uvar(&mut payload, others.len() as u64);
                    for &(_, other_initializer) in others {
                        tree.encode_node(other_initializer, &mut payload);
                    }
                    let h = tree.new_annotation(ANNOTATION_OTHER_CONSTRUCTORS, payload);
                    tree.node_mut(initializer).annotations.push(h);
                }
            }

            if sequence.is_unsafe && is_unsafe(tree, self.oracle, initializer) {
                add_modifier(tree, declaration, MODIFIERS_UNSAFE);
            }
        }
        Ok(())
    }

    /// `MoveConstructorInitializer`.
    fn move_constructor_initializer(
        &self,
        tree: &mut SyntaxTree,
        ctor: SyntaxId,
    ) -> Result<(), String> {
        let (_, method) = self.declared_method(tree, ctor)?;
        if tree
            .child(ctor, constructor_declaration::slot::BODY)
            .is_none()
        {
            return Ok(());
        }
        let statement = first_non_empty_statement(tree, ctor);
        let is_value_type = method.flags & METHOD_DECLARING_TYPE_STRUCT != 0;
        let Some(statement) = statement else {
            return Ok(());
        };
        let Some((_, invocation)) = match_this_call(tree, statement, is_value_type) else {
            return Ok(());
        };
        let Some(called) = self
            .symbol(tree, invocation)
            .and_then(|s| self.method_facts(s))
            .filter(|f| f.flags & METHOD_CONSTRUCTOR != 0)
        else {
            return Ok(());
        };
        let initializer_type = if called.declaring_type == method.declaring_type {
            INITIALIZER_THIS
        } else {
            INITIALIZER_BASE
        };
        let ci = tree.new_node(kind::CONSTRUCTOR_INITIALIZER);
        tree.node_mut(ci).scalars[constructor_initializer::scalar::CONSTRUCTOR_INITIALIZER_TYPE] =
            Scalar::Int(initializer_type);
        let arguments_slot = if tree.kind(invocation) == kind::INVOCATION_EXPRESSION {
            invocation_expression::slot::ARGUMENTS
        } else {
            object_create_expression::slot::ARGUMENTS
        };
        move_children(
            tree,
            invocation,
            arguments_slot,
            ci,
            constructor_initializer::slot::ARGUMENTS,
        );
        let argument_count = tree
            .children(ci, constructor_initializer::slot::ARGUMENTS)
            .len();
        if !(initializer_type == INITIALIZER_BASE && argument_count == 0) {
            copy_annotations(tree, invocation, ci);
            tree.set_child(ctor, constructor_declaration::slot::INITIALIZER, Some(ci));
        }
        tree.detach(statement);
        Ok(())
    }

    /// `RemoveImplicitConstructor`.
    fn remove_implicit_constructor(
        &self,
        tree: &mut SyntaxTree,
        a: &Analysis,
    ) -> Result<(), String> {
        let Some(declaration) = a.declaration else {
            return Ok(());
        };
        let is_struct = a.facts.flags & TYPE_STRUCT != 0;

        if let Some((primary, primary_decl)) = a.primary_constructor {
            let facts = self
                .method_facts(primary)
                .ok_or("a primary constructor that is no method")?;
            let has_primary = facts.parameter_count > 0
                || tree
                    .child(primary_decl, constructor_declaration::slot::INITIALIZER)
                    .is_some()
                || is_struct;
            tree.node_mut(declaration).scalars[type_declaration::scalar::HAS_PRIMARY_CONSTRUCTOR] =
                Scalar::Bool(has_primary);

            // The parameters go right after the type parameters (or the name), in their order.
            let parameters = tree
                .children(primary_decl, constructor_declaration::slot::PARAMETERS)
                .to_vec();
            for (i, p) in parameters.into_iter().enumerate() {
                tree.insert_child(
                    declaration,
                    type_declaration::slot::PRIMARY_CONSTRUCTOR_PARAMETERS,
                    i,
                    p,
                );
            }

            let empty = HashMap::new();
            let stores = a.parameter_to_backing_store.as_ref().unwrap_or(&empty);
            let parameters = tree
                .children(
                    declaration,
                    type_declaration::slot::PRIMARY_CONSTRUCTOR_PARAMETERS,
                )
                .to_vec();
            for pd in parameters {
                let index = self
                    .variable(tree, pd)
                    .and_then(|v| v.index)
                    .ok_or("a primary constructor parameter without parameter variable")?;
                let Some(&(property, field)) = stores.get(&index) else {
                    continue;
                };
                if let Some(property) = property {
                    self.add_attribute_section(tree, pd, property, false, "property")?;
                }
                if let Some(field) = field {
                    self.add_attribute_section(tree, pd, field, true, "field")?;
                }
            }

            if has_modifier(tree, primary_decl, MODIFIERS_UNSAFE) {
                add_modifier(tree, declaration, MODIFIERS_UNSAFE);
            }

            if let Some(initializer) =
                tree.child(primary_decl, constructor_declaration::slot::INITIALIZER)
                && let Some(&base_type) = tree
                    .children(declaration, type_declaration::slot::BASE_TYPES)
                    .first()
            {
                let invocation_type = tree.new_node(kind::INVOCATION_AST_TYPE);
                tree.replace(base_type, invocation_type);
                tree.set_child(
                    invocation_type,
                    invocation_ast_type::slot::BASE_TYPE,
                    Some(base_type),
                );
                move_children(
                    tree,
                    initializer,
                    constructor_initializer::slot::ARGUMENTS,
                    invocation_type,
                    invocation_ast_type::slot::ARGUMENTS,
                );
            }

            tree.detach(primary_decl);
        }

        if let Some(static_ctor) = a.static_constructor
            && a.facts.flags & TYPE_BEFORE_FIELD_INIT != 0
            && tree
                .child(static_ctor, constructor_declaration::slot::BODY)
                .is_some_and(|b| {
                    tree.children(b, block_statement::slot::STATEMENTS)
                        .is_empty()
                })
        {
            tree.detach(static_ctor);
        }

        let [ctor] = a.instance_constructors[..] else {
            return Ok(());
        };
        let (method, facts) = self.declared_method(tree, ctor)?;
        if is_struct && facts.parameter_count == 0 && a.instance_initializers.is_some() {
            // A struct constructor with initializers is not optional.
            return Ok(());
        }
        let mut expected = if a.facts.flags & TYPE_ABSTRACT != 0 {
            MODIFIERS_PROTECTED
        } else {
            MODIFIERS_PUBLIC
        };
        if has_modifier(tree, ctor, MODIFIERS_UNSAFE) {
            expected |= MODIFIERS_UNSAFE;
        }
        let is_empty = modifiers(tree, ctor) == expected
            && tree
                .children(ctor, constructor_declaration::slot::ATTRIBUTES)
                .is_empty()
            && tree
                .children(ctor, constructor_declaration::slot::PARAMETERS)
                .is_empty()
            && tree
                .child(ctor, constructor_declaration::slot::INITIALIZER)
                .is_none()
            && tree
                .child(ctor, constructor_declaration::slot::BODY)
                .is_some_and(|b| {
                    tree.children(b, block_statement::slot::STATEMENTS)
                        .is_empty()
                });
        if is_empty && self.documentation(method) & DOC_PRESENT == 0 {
            tree.detach(ctor);
        }
        Ok(())
    }

    /// Adds `[target: ...]` with the attributes of `entity` to the parameter `pd`, unless it has
    /// none.
    fn add_attribute_section(
        &self,
        tree: &mut SyntaxTree,
        pd: SyntaxId,
        entity: u32,
        is_field: bool,
        target: &str,
    ) -> Result<(), String> {
        let mut x = self
            .ask(QUERY_ATTRIBUTES, &[u64::from(entity), u64::from(is_field)])
            .ok_or("the host did not convert the attributes")?;
        let count = x.uvar().ok_or("a short attribute answer")?;
        if count == 0 {
            return Ok(());
        }
        let section = tree.new_node(kind::ATTRIBUTE_SECTION);
        let name: Vec<u16> = target.encode_utf16().collect();
        let target = new_identifier(tree, &name);
        tree.set_child(
            section,
            attribute_section::slot::ATTRIBUTE_TARGET_TOKEN,
            Some(target),
        );
        for i in 0..count as usize {
            let length = x.uvar().ok_or("a short attribute answer")? as usize;
            let bytes = x
                .bytes
                .get(x.pos..x.pos + length)
                .ok_or("a short attribute answer")?
                .to_vec();
            x.pos += length;
            let first = tree.nodes.len();
            let attribute = tree.decode_subtree(&bytes)?;
            attach_minted(tree, first);
            tree.insert_child(section, attribute_section::slot::ATTRIBUTES, i, attribute);
        }
        let n = tree
            .children(pd, parameter_declaration::slot::ATTRIBUTES)
            .len();
        tree.insert_child(pd, parameter_declaration::slot::ATTRIBUTES, n, section);
        Ok(())
    }
}

/// Makes the handles of the nodes decoded from index `first` on refer to the host-built objects.
fn attach_minted(tree: &mut SyntaxTree, first: usize) {
    let minted = |tree: &mut SyntaxTree, h: u32| {
        let mut payload = Vec::new();
        uvar(&mut payload, u64::from(h));
        tree.new_annotation(ANNOTATION_MINTED, payload)
    };
    for n in first..tree.nodes.len() {
        let id = SyntaxId(n as u32);
        let annotations = tree.node(id).annotations.clone();
        let mapped = annotations.into_iter().map(|h| minted(tree, h)).collect();
        tree.node_mut(id).annotations = mapped;
        for i in 0..tree.node(id).scalars.len() {
            if let Scalar::Object(h) = tree.node(id).scalars[i] {
                let m = minted(tree, h);
                tree.node_mut(id).scalars[i] = Scalar::Object(m);
            }
        }
    }
}

fn single(children: &[SyntaxId]) -> Result<SyntaxId, String> {
    match children {
        [one] => Ok(*one),
        _ => Err("a declaration without exactly one variable".to_owned()),
    }
}

/// Moves the children of the collection `from_slot` of `from` to the end of `to_slot` of `to`.
fn move_children(
    tree: &mut SyntaxTree,
    from: SyntaxId,
    from_slot: usize,
    to: SyntaxId,
    to_slot: usize,
) {
    for c in tree.children(from, from_slot).to_vec() {
        let n = tree.children(to, to_slot).len();
        tree.insert_child(to, to_slot, n, c);
    }
}

/// `CopyAnnotationsFrom`: the annotations after the target's own, the trivia deep-copied.
fn copy_annotations(tree: &mut SyntaxTree, from: SyntaxId, to: SyntaxId) {
    let annotations = tree.node(from).annotations.clone();
    tree.node_mut(to).annotations.extend(annotations);
    let leading = tree.node(from).leading_trivia.clone();
    let trailing = tree.node(from).trailing_trivia.clone();
    for (list, is_leading) in [(leading, true), (trailing, false)] {
        for t in list {
            let copy = tree.deep_clone(t);
            tree.node_mut(copy).parent = Some(to);
            let node = tree.node_mut(to);
            if is_leading {
                node.leading_trivia.push(copy);
            } else {
                node.trailing_trivia.push(copy);
            }
        }
    }
}

fn modifiers(tree: &SyntaxTree, declaration: SyntaxId) -> i64 {
    // Every entity declaration has `Modifiers` as its first scalar.
    tree.node(declaration).scalars[constructor_declaration::scalar::MODIFIERS]
        .as_int()
        .unwrap_or(0)
}

fn has_modifier(tree: &SyntaxTree, declaration: SyntaxId, modifier: i64) -> bool {
    modifiers(tree, declaration) & modifier == modifier
}

fn add_modifier(tree: &mut SyntaxTree, declaration: SyntaxId, modifier: i64) {
    let m = modifiers(tree, declaration) | modifier;
    tree.node_mut(declaration).scalars[constructor_declaration::scalar::MODIFIERS] = Scalar::Int(m);
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

/// `PropertyDeclaration.IsAutomaticProperty`.
fn is_automatic_property(tree: &SyntaxTree, pd: SyntaxId) -> bool {
    [
        property_declaration::slot::GETTER,
        property_declaration::slot::SETTER,
    ]
    .iter()
    .all(|&slot| {
        tree.child(pd, slot)
            .is_none_or(|a| tree.child(a, accessor::slot::BODY).is_none())
    })
}

/// `ctor.Body?.Statements.GetFirstNonEmptyStatementOrDefault()`.
fn first_non_empty_statement(tree: &SyntaxTree, ctor: SyntaxId) -> Option<SyntaxId> {
    let body = tree.child(ctor, constructor_declaration::slot::BODY)?;
    tree.children(body, block_statement::slot::STATEMENTS)
        .iter()
        .copied()
        .find(|&s| tree.kind(s) != kind::EMPTY_STATEMENT)
}

/// `GetNextNonEmptyStatement` of a statement in a block.
fn next_non_empty_statement(tree: &SyntaxTree, statement: SyntaxId) -> Option<SyntaxId> {
    let parent = tree.node(statement).parent?;
    let SlotValue::Many(siblings) = &tree.node(parent).slots[tree.slot_of(statement)?] else {
        return None;
    };
    let i = siblings.iter().position(|&s| s == statement)?;
    siblings[i + 1..]
        .iter()
        .copied()
        .find(|&s| tree.kind(s) != kind::EMPTY_STATEMENT)
}

fn assignment_operator(tree: &SyntaxTree, assignment: SyntaxId) -> i64 {
    tree.node(assignment).scalars[assignment_expression::scalar::OPERATOR]
        .as_int()
        .unwrap_or(-1)
}

/// `memberInitializerPattern`: `this.member = initializer;` or `member = initializer;`, as the
/// field access and the initializer.
fn match_member_initializer(
    tree: &SyntaxTree,
    statement: SyntaxId,
) -> Option<(SyntaxId, SyntaxId)> {
    if tree.kind(statement) != kind::EXPRESSION_STATEMENT {
        return None;
    }
    let assignment = tree.child(statement, expression_statement::slot::EXPRESSION)?;
    if tree.kind(assignment) != kind::ASSIGNMENT_EXPRESSION
        || assignment_operator(tree, assignment) != ASSIGN
    {
        return None;
    }
    let left = tree.child(assignment, assignment_expression::slot::LEFT)?;
    let right = tree.child(assignment, assignment_expression::slot::RIGHT)?;
    let is_field_access = match tree.kind(left) {
        kind::MEMBER_REFERENCE_EXPRESSION => {
            tree.child(left, member_reference_expression::slot::TARGET)
                .is_some_and(|t| tree.kind(t) == kind::THIS_REFERENCE_EXPRESSION)
                && tree
                    .children(left, member_reference_expression::slot::TYPE_ARGUMENTS)
                    .is_empty()
        }
        kind::IDENTIFIER_EXPRESSION => tree
            .children(left, identifier_expression::slot::TYPE_ARGUMENTS)
            .is_empty(),
        _ => false,
    };
    is_field_access.then_some((left, right))
}

fn is_this_or_base(tree: &SyntaxTree, id: SyntaxId) -> bool {
    matches!(
        tree.kind(id),
        kind::THIS_REFERENCE_EXPRESSION | kind::BASE_REFERENCE_EXPRESSION
    )
}

/// `ThisCallClassPattern` (`this..ctor(...);`, `base..ctor(...);`, possibly with a cast target)
/// or for a value type `ThisCallStructPattern` (`this = new TSelf(...);`), as the target and the
/// invocation.
fn match_this_call(
    tree: &SyntaxTree,
    statement: SyntaxId,
    is_struct: bool,
) -> Option<(SyntaxId, SyntaxId)> {
    if tree.kind(statement) != kind::EXPRESSION_STATEMENT {
        return None;
    }
    let expression = tree.child(statement, expression_statement::slot::EXPRESSION)?;
    if is_struct {
        if tree.kind(expression) != kind::ASSIGNMENT_EXPRESSION
            || assignment_operator(tree, expression) != ASSIGN
        {
            return None;
        }
        let target = tree.child(expression, assignment_expression::slot::LEFT)?;
        let creation = tree.child(expression, assignment_expression::slot::RIGHT)?;
        let matches = tree.kind(target) == kind::THIS_REFERENCE_EXPRESSION
            && tree.kind(creation) == kind::OBJECT_CREATE_EXPRESSION
            && tree
                .child(creation, object_create_expression::slot::TYPE)
                .is_some()
            && tree
                .child(creation, object_create_expression::slot::INITIALIZER)
                .is_none();
        return matches.then_some((target, creation));
    }
    if tree.kind(expression) != kind::INVOCATION_EXPRESSION {
        return None;
    }
    let member = tree.child(expression, invocation_expression::slot::TARGET)?;
    if tree.kind(member) != kind::MEMBER_REFERENCE_EXPRESSION
        || !tree
            .children(member, member_reference_expression::slot::TYPE_ARGUMENTS)
            .is_empty()
    {
        return None;
    }
    let name = tree.child(member, member_reference_expression::slot::MEMBER_NAME_TOKEN)?;
    if tree.node(name).scalars[identifier::scalar::NAME]
        .as_str()
        .as_deref()
        != Some(".ctor")
    {
        return None;
    }
    let mut target = tree.child(member, member_reference_expression::slot::TARGET)?;
    if tree.kind(target) == kind::CAST_EXPRESSION {
        tree.child(target, cast_expression::slot::TYPE)?;
        target = tree.child(target, cast_expression::slot::EXPRESSION)?;
    }
    is_this_or_base(tree, target).then_some((target, expression))
}

/// `Identifier.Create(name)`: a leading `@` makes a verbatim identifier.
fn new_identifier(tree: &mut SyntaxTree, name: &[u16]) -> SyntaxId {
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

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_oracle::NoOracle;

    fn utf16(s: &str) -> Vec<u16> {
        s.encode_utf16().collect()
    }

    /// A constructor whose body holds `statements`.
    fn constructor(tree: &mut SyntaxTree, statements: &[SyntaxId]) -> SyntaxId {
        let ctor = tree.new_node(kind::CONSTRUCTOR_DECLARATION);
        let body = tree.new_node(kind::BLOCK_STATEMENT);
        for (i, &s) in statements.iter().enumerate() {
            tree.insert_child(body, block_statement::slot::STATEMENTS, i, s);
        }
        tree.set_child(ctor, constructor_declaration::slot::BODY, Some(body));
        ctor
    }

    fn statement(tree: &mut SyntaxTree, expression: SyntaxId) -> SyntaxId {
        let s = tree.new_node(kind::EXPRESSION_STATEMENT);
        tree.set_child(s, expression_statement::slot::EXPRESSION, Some(expression));
        s
    }

    fn this_ctor_call(tree: &mut SyntaxTree, cast: bool) -> SyntaxId {
        let mut target = tree.new_node(kind::BASE_REFERENCE_EXPRESSION);
        if cast {
            let c = tree.new_node(kind::CAST_EXPRESSION);
            let ty = tree.new_node(kind::SIMPLE_TYPE);
            tree.set_child(c, cast_expression::slot::TYPE, Some(ty));
            tree.set_child(c, cast_expression::slot::EXPRESSION, Some(target));
            target = c;
        }
        let member = tree.new_node(kind::MEMBER_REFERENCE_EXPRESSION);
        tree.set_child(
            member,
            member_reference_expression::slot::TARGET,
            Some(target),
        );
        let name = new_identifier(tree, &utf16(".ctor"));
        tree.set_child(
            member,
            member_reference_expression::slot::MEMBER_NAME_TOKEN,
            Some(name),
        );
        let call = tree.new_node(kind::INVOCATION_EXPRESSION);
        tree.set_child(call, invocation_expression::slot::TARGET, Some(member));
        call
    }

    fn assignment(tree: &mut SyntaxTree, left: SyntaxId, right: SyntaxId) -> SyntaxId {
        let a = tree.new_node(kind::ASSIGNMENT_EXPRESSION);
        tree.set_child(a, assignment_expression::slot::LEFT, Some(left));
        tree.set_child(a, assignment_expression::slot::RIGHT, Some(right));
        a
    }

    fn identifier_expression(tree: &mut SyntaxTree, name: &str) -> SyntaxId {
        let e = tree.new_node(kind::IDENTIFIER_EXPRESSION);
        let i = new_identifier(tree, &utf16(name));
        tree.set_child(e, identifier_expression::slot::IDENTIFIER_TOKEN, Some(i));
        e
    }

    #[test]
    fn constructor_calls_match_through_casts_and_skip_empty_statements() {
        let mut tree = SyntaxTree::default();
        let empty = tree.new_node(kind::EMPTY_STATEMENT);
        let call = this_ctor_call(&mut tree, true);
        let call_statement = statement(&mut tree, call);
        let ctor = constructor(&mut tree, &[empty, call_statement]);
        assert_eq!(first_non_empty_statement(&tree, ctor), Some(call_statement));
        let (target, invocation) = match_this_call(&tree, call_statement, false).unwrap();
        assert_eq!(tree.kind(target), kind::BASE_REFERENCE_EXPRESSION);
        assert_eq!(invocation, call);
        assert!(match_this_call(&tree, call_statement, true).is_none());
        assert_eq!(next_non_empty_statement(&tree, call_statement), None);
    }

    #[test]
    fn member_initializers_need_a_plain_assignment_to_a_field_access() {
        let mut tree = SyntaxTree::default();
        let left = identifier_expression(&mut tree, "x");
        let right = identifier_expression(&mut tree, "y");
        let a = assignment(&mut tree, left, right);
        let s = statement(&mut tree, a);
        assert_eq!(match_member_initializer(&tree, s), Some((left, right)));
        tree.node_mut(a).scalars[assignment_expression::scalar::OPERATOR] = Scalar::Int(1);
        assert_eq!(match_member_initializer(&tree, s), None);
    }

    #[test]
    fn syntax_matches_ignore_verbatim_and_null_strings() {
        let mut tree = SyntaxTree::default();
        let a = identifier_expression(&mut tree, "x");
        let b = identifier_expression(&mut tree, "@x");
        let c = identifier_expression(&mut tree, "z");
        let t = Transform {
            oracle: &NoOracle,
            always_move_initializer: false,
            field_keyword: false,
            primary_constructors_for_non_records: false,
            struct_field_initializers: false,
            created_variables: HashMap::new(),
        };
        assert!(t.same_syntax(&tree, a, b));
        assert!(!t.same_syntax(&tree, a, c));
        let e1 = tree.new_node(kind::IDENTIFIER);
        let e2 = tree.new_node(kind::IDENTIFIER);
        tree.node_mut(e2).scalars[identifier::scalar::NAME] = Scalar::NullStr;
        assert!(t.same_syntax(&tree, e1, e2));
    }
}
