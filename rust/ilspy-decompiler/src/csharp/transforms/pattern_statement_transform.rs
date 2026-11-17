//! `PatternStatementTransform`: the statement patterns the C# compiler lowers (`for` and
//! `foreach` loops over arrays, inline arrays and multi-dimensional arrays, destructors, nested
//! `try`), automatic properties and events and the `field` keyword, cascading `if`, conditional
//! operator chains, pattern-based `fixed` and enhanced `using`.
//!
//! The visit is `ContextTrackingVisitor`'s with the transform's own `VisitChildren`: a child is
//! visited again as long as its visit returns another node. The patterns are matched by hand,
//! each with the full shape `PatternMatching` compares. What the tree does not hold (IL
//! variables, symbols, types, settings, the declaration points `DeclareVariables` finds) comes
//! from the host through the syntax oracle, in the query kinds 16 to 30 of this transform; the
//! annotations it creates are of the kinds 16 and 17.

use std::collections::{HashMap, HashSet};

use crate::csharp::ast::BinaryOp;
use crate::csharp::entity_syntax::modifiers;
use crate::csharp::syntax::{Scalar, SlotValue, SyntaxId, SyntaxTree};
use crate::csharp::syntax_oracle::SyntaxOracle;
use crate::csharp::syntax_schema::{
    accessor, assignment_expression, attribute, attribute_section, binary_operator_expression,
    block_statement, destructor_declaration, direction_expression, expression_statement,
    field_declaration, fixed_statement, for_statement, foreach_statement, identifier,
    identifier_expression, if_else_statement, indexer_expression, invocation_expression, kind as k,
    member_reference_expression, member_type, method_declaration, primitive_expression,
    primitive_type, property_declaration, return_statement, simple_type,
    single_variable_designation, try_catch_statement, unary_operator_expression, using_statement,
    variable_initializer, while_statement,
};
use crate::csharp::transforms::replace_method_calls_with_operators::ANNOTATION_ANSWERED;
use crate::typesystem::types::TypeSystem;

/// The first `ResolveResult` annotation: kind, IL variable, symbol (`GetSymbol`), type, member.
pub const QUERY_RESOLVE: u8 = 16;
/// The settings of the transform context, its current type definition and method.
pub const QUERY_SETTINGS: u8 = 17;
/// The facts of an IL variable.
pub const QUERY_VARIABLE: u8 = 18;
/// Whether an IL variable may become the variable of a foreach over the loop of a block
/// container, as far as the IL tells.
pub const QUERY_FOREACH_LOCAL: u8 = 19;
/// Where `DeclareVariables` declares an IL variable, and whether it was merged.
pub const QUERY_DECLARATION: u8 = 20;
/// The facts of a type.
pub const QUERY_TYPE: u8 = 21;
/// The syntax of a type (`TypeSystemAstBuilder.ConvertType`).
pub const QUERY_CONVERT_TYPE: u8 = 22;
/// The facts of a symbol.
pub const QUERY_MEMBER: u8 = 23;
/// The facts of a property.
pub const QUERY_PROPERTY: u8 = 24;
/// The facts of a field.
pub const QUERY_FIELD: u8 = 25;
/// Whether a field is the backing field of a property (`IsPropertyBackingField`).
pub const QUERY_PROPERTY_FIELD: u8 = 26;
/// Whether a field is the backing field of an event (`IsEventBackingFieldDeclaration`).
pub const QUERY_EVENT_FIELD: u8 = 27;
/// A literal value: as an `int`, and as `int.TryParse` of its text.
pub const QUERY_LITERAL: u8 = 28;
/// Whether two literal values are equal (`object.Equals`).
pub const QUERY_LITERAL_EQUALS: u8 = 29;
/// `StoreSurvivesAsInitializer` of a field definition: whether a constructor store to it can
/// move into an initializer or a property assignment.
pub const QUERY_STORE_SURVIVES: u8 = 30;

/// `new ILVariableResolveResult(v, v.Type)` of a variable that becomes a foreach variable.
pub const ANNOTATION_FOREACH_VARIABLE: u8 = 16;
/// `new MemberResolveResult(old.TargetResult, property)` of a backing field use.
pub const ANNOTATION_PROPERTY_REFERENCE: u8 = 17;

/// `AssignmentOperatorType`.
const ASSIGN: i64 = 0;
/// `UnaryOperatorType`.
const UNARY_NOT: i64 = 1;
const UNARY_ADDRESS_OF: i64 = 10;

type R<T> = Result<T, String>;

/// Runs the transform on the whole tree.
pub fn run(
    _ts: &mut TypeSystem<'_, '_>,
    tree: &mut SyntaxTree,
    oracle: &dyn SyntaxOracle,
) -> Result<(), String> {
    let Some(root) = tree.root else {
        return Ok(());
    };
    let mut t = Pst::new(tree, oracle)?;
    t.visit(root)?;
    Ok(())
}

#[derive(Clone, Copy, Debug, Default)]
struct Resolve {
    is_rr: bool,
    is_variable: bool,
    is_member: bool,
    is_type: bool,
    is_block_container: bool,
    variable: Option<u32>,
    symbol: Option<u32>,
    ty: Option<u32>,
    member: Option<u32>,
    container: Option<u32>,
}

#[derive(Clone, Debug)]
struct VarFacts {
    local_or_stack_slot: bool,
    needs_declaration: bool,
    single_definition: bool,
    stores: u64,
    loads: u64,
    addresses: u64,
    ty: Option<u32>,
    name: String,
}

#[derive(Clone, Debug)]
struct TypeFacts {
    is_array: bool,
    is_string: bool,
    /// The dimensions of an `ArrayType`, 0 for another type.
    dimensions: u64,
    by_ref_like: bool,
    is_reference: Option<bool>,
    contains_anonymous: bool,
    inline_array_length: Option<u64>,
    full_name: String,
    compiler_generated_attribute: bool,
    debugger_browsable_attribute: bool,
}

#[derive(Clone, Copy, Debug, PartialEq, Eq)]
enum Category {
    Other,
    Method,
    Property,
    Field,
    Event,
    TypeDefinition,
    Type,
}

#[derive(Clone, Debug)]
struct MemberFacts {
    category: Category,
    name: String,
    definition: Option<u32>,
    is_static: bool,
    compiler_generated: bool,
    declaring_type_definition: Option<u32>,
    declaring_type_full_name: String,
    is_constructor: bool,
    accessor_owner: Option<u32>,
}

#[derive(Clone, Copy, Debug)]
struct PropertyFacts {
    can_get: bool,
    can_set: bool,
    getter_compiler_generated: bool,
    has_setter: bool,
    setter_compiler_generated: bool,
    setter_readonly: bool,
    backing_field: Option<u32>,
}

#[derive(Clone, Copy, Debug)]
struct FieldFacts {
    accessed_through_property: bool,
    auto_property: Option<u32>,
}

#[derive(Clone, Copy, Debug)]
struct LiteralFacts {
    int32: Option<i64>,
    parsed: Option<i64>,
}

#[derive(Clone, Copy, Debug, Default)]
struct Settings {
    for_statement: bool,
    foreach_statement: bool,
    inline_arrays: bool,
    anonymous_types: bool,
    automatic_properties: bool,
    field_keyword: bool,
    getter_only_automatic_properties: bool,
    automatic_events: bool,
    pattern_based_fixed_statement: bool,
    use_enhanced_using: bool,
}

/// Reads an answer of the host.
struct Cursor<'a> {
    bytes: &'a [u8],
    pos: usize,
}

impl Cursor<'_> {
    fn byte(&mut self) -> R<u8> {
        let b = *self
            .bytes
            .get(self.pos)
            .ok_or("the host's answer ends early")?;
        self.pos += 1;
        Ok(b)
    }

    fn flag(&mut self) -> R<bool> {
        Ok(self.byte()? != 0)
    }

    fn uvar(&mut self) -> R<u64> {
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
                return Err("an overlong integer in the host's answer".to_owned());
            }
        }
    }

    /// A reference plus one, 0 for none.
    fn opt_ref(&mut self) -> R<Option<u32>> {
        Ok(match self.uvar()? {
            0 => None,
            v => Some((v - 1) as u32),
        })
    }

    fn zigzag(&mut self) -> R<i64> {
        let v = self.uvar()?;
        Ok(((v >> 1) as i64) ^ -((v & 1) as i64))
    }

    fn text(&mut self) -> R<String> {
        let len = self.uvar()? as usize;
        let end = self.pos + len;
        let s = self
            .bytes
            .get(self.pos..end)
            .ok_or("the host's answer ends early")?;
        self.pos = end;
        Ok(String::from_utf8_lossy(s).into_owned())
    }
}

fn push_uvar(out: &mut Vec<u8>, mut v: u64) {
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

fn is_statement(kind: u16) -> bool {
    matches!(
        kind,
        k::BLOCK_STATEMENT
            | k::BREAK_STATEMENT
            | k::CHECKED_STATEMENT
            | k::CONTINUE_STATEMENT
            | k::DO_WHILE_STATEMENT
            | k::EMPTY_STATEMENT
            | k::EXPRESSION_STATEMENT
            | k::FIXED_STATEMENT
            | k::FOR_STATEMENT
            | k::FOREACH_STATEMENT
            | k::GOTO_CASE_STATEMENT
            | k::GOTO_DEFAULT_STATEMENT
            | k::GOTO_STATEMENT
            | k::IF_ELSE_STATEMENT
            | k::LABEL_STATEMENT
            | k::LOCAL_FUNCTION_DECLARATION_STATEMENT
            | k::LOCK_STATEMENT
            | k::RETURN_STATEMENT
            | k::SWITCH_STATEMENT
            | k::THROW_STATEMENT
            | k::TRY_CATCH_STATEMENT
            | k::UNCHECKED_STATEMENT
            | k::UNSAFE_STATEMENT
            | k::USING_STATEMENT
            | k::VARIABLE_DECLARATION_STATEMENT
            | k::WHILE_STATEMENT
            | k::YIELD_BREAK_STATEMENT
            | k::YIELD_RETURN_STATEMENT
    )
}

fn is_expression(kind: u16) -> bool {
    matches!(
        kind,
        k::ANONYMOUS_METHOD_EXPRESSION
            | k::ANONYMOUS_TYPE_CREATE_EXPRESSION
            | k::ARRAY_CREATE_EXPRESSION
            | k::ARRAY_INITIALIZER_EXPRESSION
            | k::AS_EXPRESSION
            | k::ASSIGNMENT_EXPRESSION
            | k::BASE_REFERENCE_EXPRESSION
            | k::BINARY_OPERATOR_EXPRESSION
            | k::CAST_EXPRESSION
            | k::CHECKED_EXPRESSION
            | k::CONDITIONAL_EXPRESSION
            | k::DECLARATION_EXPRESSION
            | k::DEFAULT_VALUE_EXPRESSION
            | k::DIRECTION_EXPRESSION
            | k::ERROR_EXPRESSION
            | k::IDENTIFIER_EXPRESSION
            | k::INDEXER_EXPRESSION
            | k::INTERPOLATED_STRING_EXPRESSION
            | k::INVOCATION_EXPRESSION
            | k::IS_EXPRESSION
            | k::LAMBDA_EXPRESSION
            | k::MEMBER_REFERENCE_EXPRESSION
            | k::NAMED_ARGUMENT_EXPRESSION
            | k::NAMED_EXPRESSION
            | k::NULL_REFERENCE_EXPRESSION
            | k::OBJECT_CREATE_EXPRESSION
            | k::OUT_VAR_DECLARATION_EXPRESSION
            | k::PARENTHESIZED_EXPRESSION
            | k::POINTER_REFERENCE_EXPRESSION
            | k::PRIMITIVE_EXPRESSION
            | k::QUERY_EXPRESSION
            | k::RECURSIVE_PATTERN_EXPRESSION
            | k::SIZE_OF_EXPRESSION
            | k::STACK_ALLOC_EXPRESSION
            | k::SWITCH_EXPRESSION
            | k::THIS_REFERENCE_EXPRESSION
            | k::THROW_EXPRESSION
            | k::TUPLE_EXPRESSION
            | k::TYPE_OF_EXPRESSION
            | k::TYPE_REFERENCE_EXPRESSION
            | k::UNARY_OPERATOR_EXPRESSION
            | k::UNCHECKED_EXPRESSION
            | k::UNDOCUMENTED_EXPRESSION
            | k::WITH_INITIALIZER_EXPRESSION
    )
}

fn is_entity_declaration(kind: u16) -> bool {
    matches!(
        kind,
        k::ACCESSOR
            | k::CONSTRUCTOR_DECLARATION
            | k::CUSTOM_EVENT_DECLARATION
            | k::DELEGATE_DECLARATION
            | k::DESTRUCTOR_DECLARATION
            | k::ENUM_MEMBER_DECLARATION
            | k::EVENT_DECLARATION
            | k::EXTENSION_DECLARATION
            | k::FIELD_DECLARATION
            | k::FIXED_FIELD_DECLARATION
            | k::INDEXER_DECLARATION
            | k::METHOD_DECLARATION
            | k::OPERATOR_DECLARATION
            | k::PROPERTY_DECLARATION
            | k::TYPE_DECLARATION
    )
}

/// `NameCouldBeBackingFieldOfAutomaticProperty`: the property name in `<Name>k__BackingField`
/// (C#) or `_Name` (VB), as `^(<(?<name>.+)>k__BackingField|_(?<name>.+))$` captures it.
fn backing_field_property_name(name: &str) -> Option<&str> {
    if let Some(inner) = name
        .strip_prefix('<')
        .and_then(|n| n.strip_suffix(">k__BackingField"))
        && !inner.is_empty()
    {
        return Some(inner);
    }
    name.strip_prefix('_').filter(|n| !n.is_empty())
}

/// What a match of a pattern with a `Repeat` of statements captured.
struct ForeachMatch {
    item: SyntaxId,
    index: SyntaxId,
    statements: Vec<SyntaxId>,
}

struct Pst<'t, 'o> {
    tree: &'t mut SyntaxTree,
    oracle: &'o dyn SyntaxOracle,
    settings: Settings,
    current_type: Option<u32>,
    current_method: Option<u32>,
    /// The variables that became foreach variables; their kind is `ForeachLocal` from then on.
    foreach_locals: HashSet<u32>,
    outside_reference_root: Option<SyntaxId>,
    outside_reference_verdicts: HashMap<u32, bool>,
    resolves: HashMap<u32, Resolve>,
    variables: HashMap<u32, VarFacts>,
    types: HashMap<u32, TypeFacts>,
    members: HashMap<u32, MemberFacts>,
    properties: HashMap<u32, PropertyFacts>,
    fields: HashMap<u32, FieldFacts>,
    declarations: HashMap<u32, (SyntaxId, bool)>,
    literals: HashMap<u32, LiteralFacts>,
}

impl<'t, 'o> Pst<'t, 'o> {
    fn new(tree: &'t mut SyntaxTree, oracle: &'o dyn SyntaxOracle) -> R<Self> {
        let mut t = Pst {
            tree,
            oracle,
            settings: Settings::default(),
            current_type: None,
            current_method: None,
            foreach_locals: HashSet::new(),
            outside_reference_root: None,
            outside_reference_verdicts: HashMap::new(),
            resolves: HashMap::new(),
            variables: HashMap::new(),
            types: HashMap::new(),
            members: HashMap::new(),
            properties: HashMap::new(),
            fields: HashMap::new(),
            declarations: HashMap::new(),
            literals: HashMap::new(),
        };
        let answer = t.need(QUERY_SETTINGS, &[])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let bits = c.uvar()?;
        let bit = |i: u32| bits & (1 << i) != 0;
        t.settings = Settings {
            for_statement: bit(0),
            foreach_statement: bit(1),
            inline_arrays: bit(2),
            anonymous_types: bit(3),
            automatic_properties: bit(4),
            field_keyword: bit(5),
            getter_only_automatic_properties: bit(6),
            automatic_events: bit(7),
            pattern_based_fixed_statement: bit(8),
            use_enhanced_using: bit(9),
        };
        t.current_type = c.opt_ref()?;
        t.current_method = c.opt_ref()?;
        Ok(t)
    }

    // ---- The host ----

    fn ask(&self, kind: u8, args: &[u64]) -> Option<Vec<u8>> {
        let mut q = vec![kind];
        for &a in args {
            push_uvar(&mut q, a);
        }
        self.oracle.ask(&q)
    }

    fn need(&self, kind: u8, args: &[u64]) -> R<Vec<u8>> {
        self.ask(kind, args).ok_or_else(|| {
            format!("the host did not answer the PatternStatementTransform query {kind}")
        })
    }

    fn resolve(&mut self, handle: u32) -> R<Resolve> {
        if let Some(r) = self.resolves.get(&handle) {
            return Ok(*r);
        }
        let answer = self.need(QUERY_RESOLVE, &[u64::from(handle)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let flags = c.byte()?;
        let r = Resolve {
            is_rr: flags & 1 != 0,
            is_variable: flags & 2 != 0,
            is_member: flags & 4 != 0,
            is_type: flags & 8 != 0,
            is_block_container: flags & 16 != 0,
            variable: c.opt_ref()?,
            symbol: c.opt_ref()?,
            ty: c.opt_ref()?,
            member: c.opt_ref()?,
            container: c.opt_ref()?,
        };
        self.resolves.insert(handle, r);
        Ok(r)
    }

    /// The first annotation of `node` that `pick` accepts.
    fn first_annotation(
        &mut self,
        node: SyntaxId,
        pick: fn(&Resolve) -> bool,
    ) -> R<Option<(u32, Resolve)>> {
        for h in self.tree.node(node).annotations.clone() {
            let r = self.resolve(h)?;
            if pick(&r) {
                return Ok(Some((h, r)));
            }
        }
        Ok(None)
    }

    /// `node.Annotation<ResolveResult>()`.
    fn resolve_result(&mut self, node: SyntaxId) -> R<Option<Resolve>> {
        Ok(self.first_annotation(node, |r| r.is_rr)?.map(|(_, r)| r))
    }

    /// `GetILVariable`.
    fn il_variable(&mut self, node: SyntaxId) -> R<Option<u32>> {
        Ok(self
            .resolve_result(node)?
            .filter(|r| r.is_variable)
            .and_then(|r| r.variable))
    }

    /// `GetSymbol` of a node that is no type.
    fn symbol(&mut self, node: SyntaxId) -> R<Option<u32>> {
        Ok(self.resolve_result(node)?.and_then(|r| r.symbol))
    }

    /// `GetSymbol() as T` for the symbol category of `T`.
    fn symbol_of(&mut self, node: SyntaxId, category: Category) -> R<Option<u32>> {
        let Some(s) = self.symbol(node)? else {
            return Ok(None);
        };
        Ok((self.member(s)?.category == category).then_some(s))
    }

    /// `GetSymbol` of a type node: the type of its `TypeResolveResult`.
    fn type_symbol(&mut self, node: SyntaxId) -> R<Option<u32>> {
        Ok(self
            .first_annotation(node, |r| r.is_type)?
            .and_then(|(_, r)| r.ty))
    }

    fn variable(&mut self, v: u32) -> R<VarFacts> {
        if !self.variables.contains_key(&v) {
            let answer = self.need(QUERY_VARIABLE, &[u64::from(v)])?;
            let mut c = Cursor {
                bytes: &answer,
                pos: 0,
            };
            let facts = VarFacts {
                local_or_stack_slot: c.flag()?,
                needs_declaration: c.flag()?,
                single_definition: c.flag()?,
                stores: c.uvar()?,
                loads: c.uvar()?,
                addresses: c.uvar()?,
                ty: c.opt_ref()?,
                name: c.text()?,
            };
            self.variables.insert(v, facts);
        }
        let mut facts = self.variables[&v].clone();
        if self.foreach_locals.contains(&v) {
            // `VariableKind.ForeachLocal` needs no declaration.
            facts.local_or_stack_slot = false;
            facts.needs_declaration = false;
        }
        Ok(facts)
    }

    fn type_facts(&mut self, t: u32) -> R<TypeFacts> {
        if let Some(f) = self.types.get(&t) {
            return Ok(f.clone());
        }
        let answer = self.need(QUERY_TYPE, &[u64::from(t)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let facts = TypeFacts {
            is_array: c.flag()?,
            is_string: c.flag()?,
            dimensions: c.uvar()?,
            by_ref_like: c.flag()?,
            is_reference: match c.byte()? {
                0 => None,
                1 => Some(false),
                _ => Some(true),
            },
            contains_anonymous: c.flag()?,
            inline_array_length: match c.uvar()? {
                0 => None,
                v => Some(v - 1),
            },
            full_name: c.text()?,
            compiler_generated_attribute: c.flag()?,
            debugger_browsable_attribute: c.flag()?,
        };
        self.types.insert(t, facts.clone());
        Ok(facts)
    }

    fn member(&mut self, s: u32) -> R<MemberFacts> {
        if let Some(f) = self.members.get(&s) {
            return Ok(f.clone());
        }
        let answer = self.need(QUERY_MEMBER, &[u64::from(s)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let facts = MemberFacts {
            category: match c.byte()? {
                1 => Category::Method,
                2 => Category::Property,
                3 => Category::Field,
                4 => Category::Event,
                5 => Category::TypeDefinition,
                6 => Category::Type,
                _ => Category::Other,
            },
            name: c.text()?,
            definition: c.opt_ref()?,
            is_static: c.flag()?,
            compiler_generated: c.flag()?,
            declaring_type_definition: c.opt_ref()?,
            declaring_type_full_name: c.text()?,
            is_constructor: c.flag()?,
            accessor_owner: c.opt_ref()?,
        };
        self.members.insert(s, facts.clone());
        Ok(facts)
    }

    /// `MemberDefinition` of a member.
    fn definition(&mut self, s: u32) -> R<u32> {
        self.member(s)?
            .definition
            .ok_or_else(|| "a member without definition".to_owned())
    }

    fn property(&mut self, p: u32) -> R<PropertyFacts> {
        if let Some(f) = self.properties.get(&p) {
            return Ok(*f);
        }
        let answer = self.need(QUERY_PROPERTY, &[u64::from(p)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let facts = PropertyFacts {
            can_get: c.flag()?,
            can_set: c.flag()?,
            getter_compiler_generated: c.flag()?,
            has_setter: c.flag()?,
            setter_compiler_generated: c.flag()?,
            setter_readonly: c.flag()?,
            backing_field: c.opt_ref()?,
        };
        self.properties.insert(p, facts);
        Ok(facts)
    }

    fn field(&mut self, f: u32) -> R<FieldFacts> {
        if let Some(facts) = self.fields.get(&f) {
            return Ok(*facts);
        }
        let answer = self.need(QUERY_FIELD, &[u64::from(f)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let facts = FieldFacts {
            accessed_through_property: c.flag()?,
            auto_property: c.opt_ref()?,
        };
        self.fields.insert(f, facts);
        Ok(facts)
    }

    fn ask_flag(&self, kind: u8, args: &[u64]) -> R<bool> {
        Ok(self.need(kind, args)?.first().is_some_and(|&b| b != 0))
    }

    fn literal(&mut self, handle: u32) -> R<LiteralFacts> {
        if let Some(f) = self.literals.get(&handle) {
            return Ok(*f);
        }
        let answer = self.need(QUERY_LITERAL, &[u64::from(handle)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let int32 = if c.flag()? { Some(c.zigzag()?) } else { None };
        let parsed = if c.flag()? { Some(c.zigzag()?) } else { None };
        let facts = LiteralFacts { int32, parsed };
        self.literals.insert(handle, facts);
        Ok(facts)
    }

    /// `declareVariables.GetDeclarationPoint(v)` and `WasMerged(v)`.
    fn declaration(&mut self, v: u32) -> R<(SyntaxId, bool)> {
        if let Some(d) = self.declarations.get(&v) {
            return Ok(*d);
        }
        let answer = self.need(QUERY_DECLARATION, &[u64::from(v)])?;
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let d = (SyntaxId(c.uvar()? as u32), c.flag()?);
        self.declarations.insert(v, d);
        Ok(d)
    }

    /// `TypeSystemAstBuilder.ConvertType`.
    fn convert_type(&mut self, t: u32) -> R<SyntaxId> {
        let answer = self.need(QUERY_CONVERT_TYPE, &[u64::from(t)])?;
        // The answer is the handle table the host keeps for the subtree, the table's size and
        // the subtree, whose handles become annotations of the objects in that table.
        let mut c = Cursor {
            bytes: &answer,
            pos: 0,
        };
        let table = c.uvar()?;
        let count = c.uvar()? as u32;
        let sub = SyntaxTree::decode(&answer[c.pos..], count)?;
        self.tree
            .import(sub, |tree, h| {
                let mut payload = Vec::new();
                push_uvar(&mut payload, table);
                push_uvar(&mut payload, u64::from(h));
                tree.new_annotation(ANNOTATION_ANSWERED, payload)
            })
            .ok_or_else(|| "an empty type syntax from the host".to_owned())
    }

    // ---- The tree ----

    fn kind(&self, n: SyntaxId) -> u16 {
        self.tree.kind(n)
    }

    fn is(&self, n: Option<SyntaxId>, kind: u16) -> Option<SyntaxId> {
        n.filter(|&n| self.tree.kind(n) == kind)
    }

    fn parent(&self, n: SyntaxId) -> Option<SyntaxId> {
        self.tree.node(n).parent
    }

    fn child(&self, n: SyntaxId, slot: usize) -> Option<SyntaxId> {
        self.tree.child(n, slot)
    }

    fn children(&self, n: SyntaxId, slot: usize) -> Vec<SyntaxId> {
        self.tree.children(n, slot).to_vec()
    }

    fn scalar_int(&self, n: SyntaxId, scalar: usize) -> i64 {
        self.tree.node(n).scalars[scalar].as_int().unwrap_or(0)
    }

    fn set_scalar(&mut self, n: SyntaxId, scalar: usize, value: Scalar) {
        self.tree.node_mut(n).scalars[scalar] = value;
    }

    fn first_child(&self, n: SyntaxId) -> Option<SyntaxId> {
        self.tree.all_children(n).first().copied()
    }

    /// `NextSibling`: the next child of the parent, in child order.
    fn next_sibling(&self, n: SyntaxId) -> Option<SyntaxId> {
        let parent = self.parent(n)?;
        let mut found = false;
        for slot in &self.tree.node(parent).slots {
            let list: &[SyntaxId] = match slot {
                SlotValue::Single(Some(c)) => std::slice::from_ref(c),
                SlotValue::Single(None) => &[],
                SlotValue::Many(cs) => cs,
            };
            for &c in list {
                if found {
                    return Some(c);
                }
                found = c == n;
            }
        }
        None
    }

    /// `GetNextStatement`.
    fn next_statement(&self, n: SyntaxId) -> Option<SyntaxId> {
        let mut next = self.next_sibling(n);
        while let Some(s) = next {
            if is_statement(self.kind(s)) {
                return Some(s);
            }
            next = self.next_sibling(s);
        }
        None
    }

    /// `GetNextNonEmptyStatement`.
    fn next_non_empty_statement(&self, n: SyntaxId) -> Option<SyntaxId> {
        let mut next = self.next_statement(n);
        while let Some(s) = next {
            if self.kind(s) != k::EMPTY_STATEMENT {
                return Some(s);
            }
            next = self.next_statement(s);
        }
        None
    }

    /// `GetFirstNonEmptyStatementOrDefault` of the statements of a block.
    fn first_non_empty_statement(&self, block: SyntaxId) -> Option<SyntaxId> {
        self.tree
            .children(block, block_statement::slot::STATEMENTS)
            .iter()
            .copied()
            .find(|&s| self.kind(s) != k::EMPTY_STATEMENT)
    }

    /// The descendants of `n` in pre-order, without `n`; `descend` decides whether to go into
    /// the children of a descendant.
    fn descendants_where(&self, n: SyntaxId, descend: fn(u16) -> bool) -> Vec<SyntaxId> {
        let mut result = Vec::new();
        let mut stack: Vec<SyntaxId> = self.tree.all_children(n).into_iter().rev().collect();
        while let Some(c) = stack.pop() {
            result.push(c);
            if descend(self.kind(c)) {
                stack.extend(self.tree.all_children(c).into_iter().rev());
            }
        }
        result
    }

    fn descendants(&self, n: SyntaxId) -> Vec<SyntaxId> {
        self.descendants_where(n, |_| true)
    }

    fn descendants_and_self(&self, n: SyntaxId) -> Vec<SyntaxId> {
        self.tree.descendants_and_self(n)
    }

    fn ancestors(&self, n: SyntaxId) -> Vec<SyntaxId> {
        let mut result = Vec::new();
        let mut cur = self.parent(n);
        while let Some(p) = cur {
            result.push(p);
            cur = self.parent(p);
        }
        result
    }

    fn identifier_name(&self, ident: SyntaxId) -> String {
        self.tree.node(ident).scalars[identifier::scalar::NAME]
            .as_str()
            .unwrap_or_default()
    }

    /// The name in the identifier token slot `slot` of `n`.
    fn name(&self, n: SyntaxId, slot: usize) -> Option<String> {
        self.child(n, slot).map(|i| self.identifier_name(i))
    }

    /// `Identifier.Create(name)`.
    fn new_identifier(&mut self, name: &str) -> SyntaxId {
        let id = self.tree.new_node(k::IDENTIFIER);
        let (name, verbatim) = match name.strip_prefix('@') {
            Some(rest) => (rest, true),
            None => (name, false),
        };
        self.set_scalar(
            id,
            identifier::scalar::NAME,
            Scalar::Str(name.encode_utf16().collect()),
        );
        self.set_scalar(id, identifier::scalar::IS_VERBATIM, Scalar::Bool(verbatim));
        id
    }

    /// Sets the name in the identifier token slot `slot` of `n` (a new token).
    fn set_name(&mut self, n: SyntaxId, slot: usize, name: &str) {
        let ident = self.new_identifier(name);
        self.tree.set_child(n, slot, Some(ident));
    }

    fn add_child(&mut self, n: SyntaxId, slot: usize, child: SyntaxId) {
        let index = self.tree.children(n, slot).len();
        self.tree.insert_child(n, slot, index, child);
    }

    fn modifiers(&self, n: SyntaxId, scalar: usize) -> u32 {
        self.scalar_int(n, scalar) as u32
    }

    fn remove_modifier(&mut self, n: SyntaxId, scalar: usize, modifier: u32) {
        let m = self.modifiers(n, scalar) & !modifier;
        self.set_scalar(n, scalar, Scalar::Int(i64::from(m)));
    }

    /// `CopyAnnotationsFrom`: the annotations of `from` added to `to`, its trivia copied.
    fn copy_annotations(&mut self, to: SyntaxId, from: SyntaxId) {
        let annotations = self.tree.node(from).annotations.clone();
        self.tree.node_mut(to).annotations.extend(annotations);
        for leading in [true, false] {
            let trivia = if leading {
                self.tree.node(from).leading_trivia.clone()
            } else {
                self.tree.node(from).trailing_trivia.clone()
            };
            for t in trivia {
                let copy = self.tree.clone_subtree(t);
                self.tree.node_mut(copy).parent = Some(to);
                if leading {
                    self.tree.node_mut(to).leading_trivia.push(copy);
                } else {
                    self.tree.node_mut(to).trailing_trivia.push(copy);
                }
            }
        }
    }

    /// `ReplaceWith`; replacing the root is the error the host throws.
    fn replace(&mut self, old: SyntaxId, new: SyntaxId) -> R<()> {
        if self.parent(old).is_none() {
            return Err("Cannot replace the root node".to_owned());
        }
        self.tree.detach(new);
        self.tree.replace(old, new);
        Ok(())
    }

    // ---- Pattern pieces ----

    /// `new IdentifierExpression(Pattern.AnyString)`.
    fn any_identifier(&self, n: Option<SyntaxId>) -> Option<SyntaxId> {
        self.is(n, k::IDENTIFIER_EXPRESSION).filter(|&n| {
            self.tree
                .children(n, identifier_expression::slot::TYPE_ARGUMENTS)
                .is_empty()
        })
    }

    fn identifier_expression_name(&self, n: SyntaxId) -> String {
        self.name(n, identifier_expression::slot::IDENTIFIER_TOKEN)
            .unwrap_or_default()
    }

    /// `new IdentifierExpression(name)`, or `IdentifierExpressionBackreference` of the identifier
    /// expression `name` names.
    fn identifier_named(&self, n: Option<SyntaxId>, name: &str) -> bool {
        self.any_identifier(n)
            .is_some_and(|n| self.identifier_expression_name(n) == name)
    }

    /// `new ExpressionStatement(new AssignmentExpression { Operator = op, ... })`: left, right.
    fn assignment_statement(
        &self,
        n: Option<SyntaxId>,
        op: Option<i64>,
    ) -> Option<(Option<SyntaxId>, Option<SyntaxId>)> {
        let s = self.is(n, k::EXPRESSION_STATEMENT)?;
        let a = self.is(
            self.child(s, expression_statement::slot::EXPRESSION),
            k::ASSIGNMENT_EXPRESSION,
        )?;
        if let Some(op) = op
            && self.scalar_int(a, assignment_expression::scalar::OPERATOR) != op
        {
            return None;
        }
        Some((
            self.child(a, assignment_expression::slot::LEFT),
            self.child(a, assignment_expression::slot::RIGHT),
        ))
    }

    fn binary(&self, n: Option<SyntaxId>, op: BinaryOp) -> Option<(SyntaxId, SyntaxId)> {
        let b = self.is(n, k::BINARY_OPERATOR_EXPRESSION)?;
        if self.scalar_int(b, binary_operator_expression::scalar::OPERATOR) != op as i64 {
            return None;
        }
        Some((
            self.child(b, binary_operator_expression::slot::LEFT)?,
            self.child(b, binary_operator_expression::slot::RIGHT)?,
        ))
    }

    /// `new PrimitiveExpression(value)` of an `int`.
    fn int_literal(&mut self, n: Option<SyntaxId>, value: i64) -> R<bool> {
        let Some(n) = self.is(n, k::PRIMITIVE_EXPRESSION) else {
            return Ok(false);
        };
        let Some(h) = self.literal_handle(n) else {
            return Ok(false);
        };
        Ok(self.literal(h)?.int32 == Some(value))
    }

    fn literal_handle(&self, n: SyntaxId) -> Option<u32> {
        match self.tree.node(n).scalars[primitive_expression::scalar::VALUE] {
            Scalar::Object(h) => Some(h),
            _ => None,
        }
    }

    /// `new MemberReferenceExpression(target, name)`: the target.
    fn member_reference(&self, n: Option<SyntaxId>, name: &str) -> Option<SyntaxId> {
        let m = self.is(n, k::MEMBER_REFERENCE_EXPRESSION)?;
        if self
            .name(m, member_reference_expression::slot::MEMBER_NAME_TOKEN)
            .as_deref()
            != Some(name)
            || !self
                .tree
                .children(m, member_reference_expression::slot::TYPE_ARGUMENTS)
                .is_empty()
        {
            return None;
        }
        self.child(m, member_reference_expression::slot::TARGET)
    }

    /// A block statement whose statements are exactly one statement: that statement.
    fn single_statement_block(&self, n: Option<SyntaxId>) -> Option<SyntaxId> {
        let b = self.is(n, k::BLOCK_STATEMENT)?;
        match self.tree.children(b, block_statement::slot::STATEMENTS) {
            [s] => Some(*s),
            _ => None,
        }
    }

    /// `Backreference`: `IsMatch` of two nodes, the structural comparison of `DoMatch`.
    fn nodes_match(&mut self, a: Option<SyntaxId>, b: Option<SyntaxId>) -> R<bool> {
        let (a, b) = match (a, b) {
            (None, None) => return Ok(true),
            (Some(a), Some(b)) => (a, b),
            _ => return Ok(false),
        };
        let kind = self.kind(a);
        if kind != self.kind(b) {
            return Ok(false);
        }
        if kind == k::PRIMITIVE_EXPRESSION {
            let (Some(x), Some(y)) = (self.literal_handle(a), self.literal_handle(b)) else {
                return Ok(false);
            };
            return self.ask_flag(QUERY_LITERAL_EQUALS, &[u64::from(x), u64::from(y)]);
        }
        let schema = self.tree.node(a).schema();
        for (i, s) in schema.scalars.iter().enumerate() {
            let excluded = s.ty == "TextLocation"
                || (kind == k::IDENTIFIER && s.name == "IsVerbatim")
                || (kind == k::ACCESSOR && s.name == "IsInitOnly");
            if !excluded && self.tree.node(a).scalars[i] != self.tree.node(b).scalars[i] {
                return Ok(false);
            }
        }
        for (i, s) in schema.slots.iter().enumerate() {
            if s.name == "NameToken"
                && (kind == k::CONSTRUCTOR_DECLARATION || kind == k::DESTRUCTOR_DECLARATION)
            {
                continue;
            }
            let (x, y) = (
                self.tree.node(a).slots[i].clone(),
                self.tree.node(b).slots[i].clone(),
            );
            match (x, y) {
                (SlotValue::Single(x), SlotValue::Single(y)) => {
                    if !self.nodes_match(x, y)? {
                        return Ok(false);
                    }
                }
                (SlotValue::Many(xs), SlotValue::Many(ys)) => {
                    if xs.len() != ys.len() {
                        return Ok(false);
                    }
                    for (x, y) in xs.into_iter().zip(ys) {
                        if !self.nodes_match(Some(x), Some(y))? {
                            return Ok(false);
                        }
                    }
                }
                _ => return Ok(false),
            }
        }
        Ok(true)
    }

    // ---- The visitor ----

    fn visit(&mut self, n: SyntaxId) -> R<SyntaxId> {
        match self.kind(n) {
            k::EXPRESSION_STATEMENT => {
                if let Some(r) = self.transform_foreach_on_multi_dim_array(n)? {
                    return Ok(r);
                }
                if let Some(r) = self.transform_for(n)? {
                    return Ok(r);
                }
                self.visit_children(n)
            }
            k::FOR_STATEMENT => {
                if let Some(r) = self.transform_foreach_on_array(n)? {
                    return Ok(r);
                }
                if let Some(r) = self.transform_foreach_on_inline_array(n)? {
                    return Ok(r);
                }
                self.visit_children(n)
            }
            k::IF_ELSE_STATEMENT => {
                self.simplify_cascading_if_else_statements(n);
                self.visit_children(n)
            }
            k::PROPERTY_DECLARATION => {
                let s = self.settings;
                if (s.automatic_properties || s.field_keyword)
                    && (self.child(n, property_declaration::slot::SETTER).is_some()
                        || s.getter_only_automatic_properties)
                {
                    self.transform_automatic_property(n)?;
                }
                self.visit_children(n)
            }
            k::EVENT_DECLARATION => {
                self.visit_event_declaration(n)?;
                self.visit_children(n)
            }
            k::METHOD_DECLARATION => {
                if let Some(d) = self.transform_destructor(n)? {
                    return Ok(d);
                }
                self.visit_method(n)
            }
            k::DESTRUCTOR_DECLARATION => {
                if let Some(d) = self.transform_destructor_body(n) {
                    return Ok(d);
                }
                self.visit_method(n)
            }
            k::CONSTRUCTOR_DECLARATION | k::OPERATOR_DECLARATION | k::ACCESSOR => {
                self.visit_method(n)
            }
            k::TYPE_DECLARATION => {
                let old = self.current_type;
                self.current_type = self.symbol_of(n, Category::TypeDefinition)?;
                let result = self.visit_children(n);
                self.current_type = old;
                result
            }
            k::TRY_CATCH_STATEMENT => {
                self.transform_try_catch_finally(n);
                self.visit_children(n)
            }
            k::IDENTIFIER => {
                let s = self.settings;
                if (s.automatic_properties || s.field_keyword)
                    && let Some(new) = self.replace_backing_field_usage(n)?
                {
                    self.replace(n, new)?;
                    return Ok(new);
                }
                self.visit_children(n)
            }
            k::BINARY_OPERATOR_EXPRESSION => self.visit_binary_operator_expression(n),
            k::UNARY_OPERATOR_EXPRESSION => self.visit_unary_operator_expression(n),
            k::FIXED_STATEMENT => {
                self.visit_fixed_statement(n)?;
                self.visit_children(n)
            }
            k::USING_STATEMENT => self.visit_using_statement(n),
            _ => self.visit_children(n),
        }
    }

    /// `VisitChildren`: each child is visited again as long as its visit returns another node.
    fn visit_children(&mut self, n: SyntaxId) -> R<SyntaxId> {
        let mut child = self.first_child(n);
        while let Some(mut c) = child {
            loop {
                let old = c;
                c = self.visit(c)?;
                if c == old {
                    break;
                }
            }
            child = self.next_sibling(c);
        }
        Ok(n)
    }

    /// The visit of a method-like declaration, with its symbol as the current method.
    fn visit_method(&mut self, n: SyntaxId) -> R<SyntaxId> {
        let old = self.current_method;
        self.current_method = self.symbol_of(n, Category::Method)?;
        let result = self.visit_children(n);
        self.current_method = old;
        result
    }

    // ---- for ----

    /// `TransformFor`.
    fn transform_for(&mut self, node: SyntaxId) -> R<Option<SyntaxId>> {
        if !self.settings.for_statement {
            return Ok(None);
        }
        // $variable = $initializer;
        let Some((left, right)) = self.assignment_statement(Some(node), Some(ASSIGN)) else {
            return Ok(None);
        };
        let Some(variable_node) = self.any_identifier(left) else {
            return Ok(None);
        };
        if right.is_none() {
            return Ok(None);
        }
        let variable = self.il_variable(variable_node)?;
        let Some(next) = self.next_non_empty_statement(node) else {
            return Ok(None);
        };
        if self.kind(next) == k::FOR_STATEMENT
            && self.for_statement_uses_variable(next, variable)?
        {
            self.tree.detach(node);
            self.tree
                .insert_child(next, for_statement::slot::INITIALIZERS, 0, node);
            return Ok(Some(next));
        }
        let Some((ident, iterator, statements)) = self.match_for_pattern(next)? else {
            return Ok(None);
        };
        if variable != self.il_variable(ident)? {
            return Ok(None);
        }
        let loop_ = next;
        if let Some(v) = variable {
            let ty = self.variable(v)?.ty;
            if let Some(ty) = ty
                && self.type_facts(ty)?.by_ref_like
                && self.is_variable_used_after(loop_, v)?
            {
                return Ok(None);
            }
        }
        if self.iterator_variables_declared_inside_loop_body(iterator)? {
            return Ok(None);
        }
        let continues = self
            .descendants_where(loop_, descend_into_statement)
            .into_iter()
            .any(|s| self.kind(s) == k::CONTINUE_STATEMENT);
        if continues {
            return Ok(None);
        }
        self.tree.detach(node);
        let new_body = self.tree.new_node(k::BLOCK_STATEMENT);
        for stmt in statements {
            self.tree.detach(stmt);
            self.add_child(new_body, block_statement::slot::STATEMENTS, stmt);
        }
        let for_statement = self.tree.new_node(k::FOR_STATEMENT);
        self.copy_annotations(for_statement, loop_);
        self.add_child(for_statement, for_statement::slot::INITIALIZERS, node);
        let condition = self.child(loop_, while_statement::slot::CONDITION);
        self.tree
            .set_child(for_statement, for_statement::slot::CONDITION, condition);
        self.tree.detach(iterator);
        self.add_child(for_statement, for_statement::slot::ITERATORS, iterator);
        self.tree.set_child(
            for_statement,
            for_statement::slot::EMBEDDED_STATEMENT,
            Some(new_body),
        );
        self.replace(loop_, for_statement)?;
        Ok(Some(for_statement))
    }

    /// `forPattern` on `next`: the condition's identifier, the iterator and the statements
    /// before it.
    fn match_for_pattern(
        &mut self,
        next: SyntaxId,
    ) -> R<Option<(SyntaxId, SyntaxId, Vec<SyntaxId>)>> {
        if self.kind(next) != k::WHILE_STATEMENT {
            return Ok(None);
        }
        let Some(condition) = self.is(
            self.child(next, while_statement::slot::CONDITION),
            k::BINARY_OPERATOR_EXPRESSION,
        ) else {
            return Ok(None);
        };
        let Some(ident) =
            self.any_identifier(self.child(condition, binary_operator_expression::slot::LEFT))
        else {
            return Ok(None);
        };
        if self
            .child(condition, binary_operator_expression::slot::RIGHT)
            .is_none()
        {
            return Ok(None);
        }
        let Some(body) = self.is(
            self.child(next, while_statement::slot::EMBEDDED_STATEMENT),
            k::BLOCK_STATEMENT,
        ) else {
            return Ok(None);
        };
        let mut statements = self.children(body, block_statement::slot::STATEMENTS);
        let Some(iterator) = statements.pop() else {
            return Ok(None);
        };
        let Some((left, right)) = self.assignment_statement(Some(iterator), None) else {
            return Ok(None);
        };
        if right.is_none() || !self.nodes_match(Some(ident), left)? {
            return Ok(None);
        }
        Ok(Some((ident, iterator, statements)))
    }

    fn for_statement_uses_variable(
        &mut self,
        statement: SyntaxId,
        variable: Option<u32>,
    ) -> R<bool> {
        if let Some(condition) = self.child(statement, for_statement::slot::CONDITION)
            && self.any_identifier_with_variable(condition, variable)?
        {
            return Ok(true);
        }
        for iterator in self.children(statement, for_statement::slot::ITERATORS) {
            if self.any_identifier_with_variable(iterator, variable)? {
                return Ok(true);
            }
        }
        Ok(false)
    }

    /// Whether an identifier expression in `n` or its descendants has `GetILVariable() == variable`.
    fn any_identifier_with_variable(&mut self, n: SyntaxId, variable: Option<u32>) -> R<bool> {
        for d in self.descendants_and_self(n) {
            if self.kind(d) == k::IDENTIFIER_EXPRESSION && self.il_variable(d)? == variable {
                return Ok(true);
            }
        }
        Ok(false)
    }

    fn is_variable_used_after(&mut self, loop_: SyntaxId, variable: u32) -> R<bool> {
        let mut sibling = self.next_sibling(loop_);
        while let Some(s) = sibling {
            if self.any_identifier_with_variable(s, Some(variable))? {
                return Ok(true);
            }
            sibling = self.next_sibling(s);
        }
        Ok(false)
    }

    fn iterator_variables_declared_inside_loop_body(&mut self, iterator: SyntaxId) -> R<bool> {
        for id in self.descendants_and_self(iterator) {
            if self.kind(id) != k::IDENTIFIER_EXPRESSION {
                continue;
            }
            let Some(v) = self.il_variable(id)? else {
                continue;
            };
            if !self.variable(v)?.needs_declaration {
                continue;
            }
            let (point, _) = self.declaration(v)?;
            if self.parent(point) == self.parent(iterator) {
                return Ok(true);
            }
        }
        Ok(false)
    }

    // ---- foreach ----

    /// The `Initializers`, `Iterators` and body shape the array and inline array patterns share:
    /// `for ($index = 0; <condition>; $index = $index + 1) { $item = <element>; ... }`.
    /// The condition's right operand and the element are returned to the caller to match.
    fn match_counting_loop(
        &mut self,
        for_statement: SyntaxId,
    ) -> R<Option<(ForeachMatch, SyntaxId, Option<SyntaxId>)>> {
        let initializers = self.children(for_statement, for_statement::slot::INITIALIZERS);
        let [initializer] = initializers[..] else {
            return Ok(None);
        };
        let Some((left, right)) = self.assignment_statement(Some(initializer), Some(ASSIGN)) else {
            return Ok(None);
        };
        let Some(index) = self.any_identifier(left) else {
            return Ok(None);
        };
        if !self.int_literal(right, 0)? {
            return Ok(None);
        }
        let index_name = self.identifier_expression_name(index);
        let Some((condition_left, condition_right)) = self.binary(
            self.child(for_statement, for_statement::slot::CONDITION),
            BinaryOp::LessThan,
        ) else {
            return Ok(None);
        };
        if !self.identifier_named(Some(condition_left), &index_name) {
            return Ok(None);
        }
        let iterators = self.children(for_statement, for_statement::slot::ITERATORS);
        let [iterator] = iterators[..] else {
            return Ok(None);
        };
        if !self.is_increment(Some(iterator), &index_name)? {
            return Ok(None);
        }
        let Some(body) = self.is(
            self.child(for_statement, for_statement::slot::EMBEDDED_STATEMENT),
            k::BLOCK_STATEMENT,
        ) else {
            return Ok(None);
        };
        let statements = self.children(body, block_statement::slot::STATEMENTS);
        let Some((&first, rest)) = statements.split_first() else {
            return Ok(None);
        };
        let Some((left, element)) = self.assignment_statement(Some(first), Some(ASSIGN)) else {
            return Ok(None);
        };
        let Some(item) = self.any_identifier(left) else {
            return Ok(None);
        };
        Ok(Some((
            ForeachMatch {
                item,
                index,
                statements: rest.to_vec(),
            },
            condition_right,
            element,
        )))
    }

    /// `$index = $index + 1;`
    fn is_increment(&mut self, n: Option<SyntaxId>, index_name: &str) -> R<bool> {
        let Some((left, right)) = self.assignment_statement(n, Some(ASSIGN)) else {
            return Ok(false);
        };
        if !self.identifier_named(left, index_name) {
            return Ok(false);
        }
        let Some((add_left, add_right)) = self.binary(right, BinaryOp::Add) else {
            return Ok(false);
        };
        Ok(self.identifier_named(Some(add_left), index_name)
            && self.int_literal(Some(add_right), 1)?)
    }

    /// `VariableCanBeUsedAsForeachLocal`.
    fn variable_can_be_used_as_foreach_local(&mut self, item: u32, loop_: SyntaxId) -> R<bool> {
        if !self.variable(item)?.local_or_stack_slot {
            return Ok(false);
        }
        let container = self
            .first_annotation(loop_, |r| r.is_block_container)?
            .and_then(|(_, r)| r.container);
        let container_arg = container.map_or(0, |c| u64::from(c) + 1);
        if !self.ask_flag(QUERY_FOREACH_LOCAL, &[u64::from(item), container_arg])? {
            return Ok(false);
        }
        let (point, merged) = self.declaration(item)?;
        Ok(self.ancestors(point).contains(&loop_) && !merged)
    }

    /// The `VariableType` of a new foreach statement over `item`.
    fn foreach_variable_type(&mut self, item: &VarFacts) -> R<SyntaxId> {
        let ty = item.ty.ok_or("an IL variable without type")?;
        if self.settings.anonymous_types && self.type_facts(ty)?.contains_anonymous {
            let var = self.tree.new_node(k::SIMPLE_TYPE);
            self.set_name(var, simple_type::slot::IDENTIFIER_TOKEN, "var");
            return Ok(var);
        }
        self.convert_type(ty)
    }

    /// The new foreach statement over `item` (`VariableType`, `VariableDesignation`,
    /// `InExpression`, `EmbeddedStatement`), with the annotation that makes `item` its variable.
    fn new_foreach(
        &mut self,
        item: u32,
        variable_type: SyntaxId,
        in_expression: SyntaxId,
        body: SyntaxId,
    ) -> R<SyntaxId> {
        let facts = self.variable(item)?;
        let foreach = self.tree.new_node(k::FOREACH_STATEMENT);
        self.tree.set_child(
            foreach,
            foreach_statement::slot::VARIABLE_TYPE,
            Some(variable_type),
        );
        let designation = self.tree.new_node(k::SINGLE_VARIABLE_DESIGNATION);
        self.set_name(
            designation,
            single_variable_designation::slot::IDENTIFIER_TOKEN,
            &facts.name,
        );
        self.tree.set_child(
            foreach,
            foreach_statement::slot::VARIABLE_DESIGNATION,
            Some(designation),
        );
        self.tree.detach(in_expression);
        self.tree.set_child(
            foreach,
            foreach_statement::slot::IN_EXPRESSION,
            Some(in_expression),
        );
        self.tree.detach(body);
        self.tree.set_child(
            foreach,
            foreach_statement::slot::EMBEDDED_STATEMENT,
            Some(body),
        );
        Ok(foreach)
    }

    /// `itemVariable.Kind = ForeachLocal` and the `ILVariableResolveResult` annotation on the
    /// designation.
    fn make_foreach_variable(&mut self, item: u32, foreach: SyntaxId) -> R<()> {
        self.foreach_locals.insert(item);
        let mut payload = Vec::new();
        push_uvar(&mut payload, u64::from(item));
        let handle = self
            .tree
            .new_annotation(ANNOTATION_FOREACH_VARIABLE, payload);
        let ty = self.variable(item)?.ty;
        self.resolves.insert(
            handle,
            Resolve {
                is_rr: true,
                is_variable: true,
                variable: Some(item),
                ty,
                ..Resolve::default()
            },
        );
        let designation = self
            .child(foreach, foreach_statement::slot::VARIABLE_DESIGNATION)
            .ok_or("a foreach statement without designation")?;
        self.tree.node_mut(designation).annotations.push(handle);
        Ok(())
    }

    /// `TransformForeachOnArray`.
    fn transform_foreach_on_array(&mut self, for_statement: SyntaxId) -> R<Option<SyntaxId>> {
        if !self.settings.foreach_statement {
            return Ok(None);
        }
        let Some((m, condition_right, element)) = self.match_counting_loop(for_statement)? else {
            return Ok(None);
        };
        // $index < $array.Length
        let Some(array) =
            self.any_identifier(self.member_reference(Some(condition_right), "Length"))
        else {
            return Ok(None);
        };
        let array_name = self.identifier_expression_name(array);
        let index_name = self.identifier_expression_name(m.index);
        // $item = $array[$index]
        let Some(indexer) = self.is(element, k::INDEXER_EXPRESSION) else {
            return Ok(None);
        };
        if !self.identifier_named(
            self.child(indexer, indexer_expression::slot::TARGET),
            &array_name,
        ) {
            return Ok(None);
        }
        match self
            .tree
            .children(indexer, indexer_expression::slot::ARGUMENTS)
        {
            [a] if self.identifier_named(Some(*a), &index_name) => {}
            _ => return Ok(None),
        }
        let item = self.il_variable(m.item)?;
        let index = self.il_variable(m.index)?;
        let array_variable = self.il_variable(array)?;
        let (Some(item), Some(index), Some(array_variable)) = (item, index, array_variable) else {
            return Ok(None);
        };
        let array_type = self
            .variable(array_variable)?
            .ty
            .ok_or("an IL variable without type")?;
        let array_type = self.type_facts(array_type)?;
        if !array_type.is_array && !array_type.is_string {
            return Ok(None);
        }
        if !self.variable_can_be_used_as_foreach_local(item, for_statement)? {
            return Ok(None);
        }
        let index = self.variable(index)?;
        if index.stores != 2 || index.loads != 3 || index.addresses != 0 {
            return Ok(None);
        }
        let body = self.tree.new_node(k::BLOCK_STATEMENT);
        for statement in m.statements {
            self.tree.detach(statement);
            self.add_child(body, block_statement::slot::STATEMENTS, statement);
        }
        let item_facts = self.variable(item)?;
        let variable_type = self.foreach_variable_type(&item_facts)?;
        let foreach = self.new_foreach(item, variable_type, array, body)?;
        self.copy_annotations(foreach, for_statement);
        self.make_foreach_variable(item, foreach)?;
        self.replace(for_statement, foreach)?;
        Ok(Some(foreach))
    }

    /// `TransformForeachOnInlineArray`.
    fn transform_foreach_on_inline_array(
        &mut self,
        for_statement: SyntaxId,
    ) -> R<Option<SyntaxId>> {
        if !self.settings.foreach_statement || !self.settings.inline_arrays {
            return Ok(None);
        }
        let Some((m, length, element)) = self.match_counting_loop(for_statement)? else {
            return Ok(None);
        };
        if self.kind(length) != k::PRIMITIVE_EXPRESSION || element.is_none() {
            return Ok(None);
        }
        let item = self.il_variable(m.item)?;
        let index = self.il_variable(m.index)?;
        let (Some(item), Some(index)) = (item, index) else {
            return Ok(None);
        };
        // The loop body must start with `item = InlineArrayElementRef(ref buffer, index)`.
        let Some(element_access) = self.is(element, k::INVOCATION_EXPRESSION) else {
            return Ok(None);
        };
        let Some(helper) = self.symbol_of(element_access, Category::Method)? else {
            return Ok(None);
        };
        let helper = self.member(helper)?;
        if helper.declaring_type_full_name != "<PrivateImplementationDetails>"
            || !matches!(
                helper.name.as_str(),
                "InlineArrayElementRef" | "InlineArrayElementRefReadOnly"
            )
        {
            return Ok(None);
        }
        let arguments = self.children(element_access, invocation_expression::slot::ARGUMENTS);
        let [first, last] = arguments[..] else {
            return Ok(None);
        };
        // arg0: `ref buffer`, arg1: the loop index.
        let Some(direction) = self.is(Some(first), k::DIRECTION_EXPRESSION) else {
            return Ok(None);
        };
        let Some(buffer_identifier) = self.is(
            self.child(direction, direction_expression::slot::EXPRESSION),
            k::IDENTIFIER_EXPRESSION,
        ) else {
            return Ok(None);
        };
        let Some(buffer) = self.il_variable(buffer_identifier)? else {
            return Ok(None);
        };
        if self.kind(last) != k::IDENTIFIER_EXPRESSION || self.il_variable(last)? != Some(index) {
            return Ok(None);
        }
        let buffer_type = self
            .variable(buffer)?
            .ty
            .ok_or("an IL variable without type")?;
        let Some(array_length) = self.type_facts(buffer_type)?.inline_array_length else {
            return Ok(None);
        };
        let Some(h) = self.literal_handle(length) else {
            return Ok(None);
        };
        if self.literal(h)?.int32 != i64::try_from(array_length).ok() {
            return Ok(None);
        }
        if !self.variable_can_be_used_as_foreach_local(item, for_statement)? {
            return Ok(None);
        }
        let index = self.variable(index)?;
        if index.stores != 2 || index.loads != 3 || index.addresses != 0 {
            return Ok(None);
        }
        // Take the buffer reference for the `in` expression before dropping the element access.
        self.tree.detach(buffer_identifier);
        let body = self
            .child(for_statement, for_statement::slot::EMBEDDED_STATEMENT)
            .ok_or("a for statement without body")?;
        if let Some(&first) = self
            .tree
            .children(body, block_statement::slot::STATEMENTS)
            .first()
        {
            self.tree.detach(first);
        }
        let item_facts = self.variable(item)?;
        let variable_type = self.foreach_variable_type(&item_facts)?;
        let foreach = self.new_foreach(item, variable_type, buffer_identifier, body)?;
        self.copy_annotations(foreach, for_statement);
        self.make_foreach_variable(item, foreach)?;
        self.replace(for_statement, foreach)?;
        Ok(Some(foreach))
    }

    /// `$variable = $collection.<method>($index);`: the variable, the collection and the index
    /// literal.
    fn match_bound_assignment(
        &self,
        n: Option<SyntaxId>,
        method: &str,
    ) -> Option<(SyntaxId, SyntaxId, SyntaxId)> {
        let (left, right) = self.assignment_statement(n, Some(ASSIGN))?;
        let variable = self.any_identifier(left)?;
        let call = self.is(right, k::INVOCATION_EXPRESSION)?;
        let collection = self.any_identifier(self.member_reference(
            self.child(call, invocation_expression::slot::TARGET),
            method,
        ))?;
        match self
            .tree
            .children(call, invocation_expression::slot::ARGUMENTS)
        {
            [index] if self.kind(*index) == k::PRIMITIVE_EXPRESSION => {
                Some((variable, collection, *index))
            }
            _ => None,
        }
    }

    /// `int.TryParse` of the text of a literal.
    fn parsed_literal(&mut self, n: SyntaxId) -> R<Option<i64>> {
        match self.literal_handle(n) {
            Some(h) => Ok(self.literal(h)?.parsed),
            None => Ok(None),
        }
    }

    /// `MatchLowerBound`.
    fn match_lower_bound(
        &mut self,
        index_num: usize,
        collection: u32,
        statement: SyntaxId,
    ) -> R<(bool, Option<u32>)> {
        let Some((variable, coll, index)) =
            self.match_bound_assignment(Some(statement), "GetLowerBound")
        else {
            return Ok((false, None));
        };
        match self.parsed_literal(index)? {
            Some(i) if i == index_num as i64 => {}
            _ => return Ok((false, None)),
        }
        let index = self.il_variable(variable)?;
        Ok((self.il_variable(coll)? == Some(collection), index))
    }

    /// `forOnArrayMultiDimPattern`: the index, the upper bound, the first body statement and
    /// the rest.
    fn match_multi_dim_loop(
        &mut self,
        n: Option<SyntaxId>,
    ) -> R<Option<(SyntaxId, SyntaxId, Vec<SyntaxId>)>> {
        let Some(f) = self.is(n, k::FOR_STATEMENT) else {
            return Ok(None);
        };
        if !self
            .tree
            .children(f, for_statement::slot::INITIALIZERS)
            .is_empty()
        {
            return Ok(None);
        }
        let Some((left, right)) = self.binary(
            self.child(f, for_statement::slot::CONDITION),
            BinaryOp::LessThanOrEqual,
        ) else {
            return Ok(None);
        };
        let (Some(index), Some(upper_bound)) = (
            self.any_identifier(Some(left)),
            self.any_identifier(Some(right)),
        ) else {
            return Ok(None);
        };
        let index_name = self.identifier_expression_name(index);
        let iterators = self.children(f, for_statement::slot::ITERATORS);
        let [iterator] = iterators[..] else {
            return Ok(None);
        };
        if !self.is_increment(Some(iterator), &index_name)? {
            return Ok(None);
        }
        let Some(body) = self.is(
            self.child(f, for_statement::slot::EMBEDDED_STATEMENT),
            k::BLOCK_STATEMENT,
        ) else {
            return Ok(None);
        };
        let statements = self.children(body, block_statement::slot::STATEMENTS);
        let Some((&first, rest)) = statements.split_first() else {
            return Ok(None);
        };
        Ok(Some((upper_bound, first, rest.to_vec())))
    }

    /// `MatchForeachOnMultiDimArray`: the foreach variable, the statements and the lower bounds.
    #[allow(clippy::type_complexity)]
    fn match_foreach_on_multi_dim_array(
        &mut self,
        upper_bounds: &[Option<u32>],
        collection: u32,
        first_initializer: SyntaxId,
    ) -> R<Option<(SyntaxId, Vec<SyntaxId>, Vec<Option<u32>>)>> {
        let mut i = 0;
        let mut lower_bounds = vec![None; upper_bounds.len()];
        let mut stmt = first_initializer;
        let mut statements = Vec::new();
        while i < upper_bounds.len() {
            let (matched, index) = self.match_lower_bound(i, collection, stmt)?;
            if !matched {
                break;
            }
            let next = self.next_non_empty_statement(stmt);
            let Some((upper_bound, lower_bound_assign, rest)) = self.match_multi_dim_loop(next)?
            else {
                return Ok(None);
            };
            if upper_bounds[i] != self.il_variable(upper_bound)? {
                return Ok(None);
            }
            stmt = lower_bound_assign;
            statements = rest;
            lower_bounds[i] = index;
            i += 1;
        }
        let collection_type = self
            .variable(collection)?
            .ty
            .ok_or("an IL variable without type")?;
        if !self.type_facts(collection_type)?.is_array {
            return Ok(None);
        }
        // $variable = $collection[$index1, $index2, ...];
        let Some((left, right)) = self.assignment_statement(Some(stmt), Some(ASSIGN)) else {
            return Ok(None);
        };
        let Some(variable) = self.any_identifier(left) else {
            return Ok(None);
        };
        let Some(indexer) = self.is(right, k::INDEXER_EXPRESSION) else {
            return Ok(None);
        };
        let Some(collection2) =
            self.any_identifier(self.child(indexer, indexer_expression::slot::TARGET))
        else {
            return Ok(None);
        };
        let arguments = self.children(indexer, indexer_expression::slot::ARGUMENTS);
        if arguments
            .iter()
            .any(|&a| self.any_identifier(Some(a)).is_none())
        {
            return Ok(None);
        }
        if self.il_variable(collection2)? != Some(collection) {
            return Ok(None);
        }
        Ok(Some((variable, statements, lower_bounds)))
    }

    /// `TransformForeachOnMultiDimArray`.
    fn transform_foreach_on_multi_dim_array(
        &mut self,
        expression_statement: SyntaxId,
    ) -> R<Option<SyntaxId>> {
        if !self.settings.foreach_statement {
            return Ok(None);
        }
        let mut stmt = Some(expression_statement);
        let mut collection: Option<u32> = None;
        let mut upper_bounds: Option<Vec<Option<u32>>> = None;
        let mut statements_to_delete = Vec::new();
        let mut collection_node = None;
        let mut i = 0;
        // first we look for all the upper bound initializations
        while let Some((variable, coll, index)) = self.match_bound_assignment(stmt, "GetUpperBound")
        {
            let current = stmt.expect("a matched statement");
            match &upper_bounds {
                None => {
                    collection = self.il_variable(coll)?;
                    let dimensions = match collection {
                        Some(c) => {
                            let ty = self.variable(c)?.ty.ok_or("an IL variable without type")?;
                            self.type_facts(ty)?.dimensions
                        }
                        None => 0,
                    };
                    if dimensions == 0 {
                        break;
                    }
                    upper_bounds = Some(vec![None; dimensions as usize]);
                }
                Some(_) => statements_to_delete.push(Some(current)),
            }
            collection_node = Some(coll);
            if self.il_variable(coll)? != collection {
                break;
            }
            match self.parsed_literal(index)? {
                Some(parsed) if parsed == i as i64 => {}
                _ => break,
            }
            let v = self.il_variable(variable)?;
            if let Some(bounds) = upper_bounds.as_mut() {
                bounds[i] = v;
            }
            stmt = self.next_non_empty_statement(current);
            i += 1;
            let len = upper_bounds.as_ref().map_or(0, Vec::len);
            if !(stmt.is_some() && upper_bounds.is_some() && i < len) {
                break;
            }
        }
        let Some(upper_bounds) = upper_bounds else {
            return Ok(None);
        };
        let (Some(Some(_)), Some(collection), Some(stmt)) = (upper_bounds.last(), collection, stmt)
        else {
            return Ok(None);
        };
        // An upper bound without IL variable fails the managed transform with a
        // NullReferenceException; that never happens for compiler output.
        if upper_bounds.iter().any(Option::is_none) {
            return Ok(None);
        }
        let Some((foreach_variable, statements, lower_bounds)) =
            self.match_foreach_on_multi_dim_array(&upper_bounds, collection, stmt)?
        else {
            return Ok(None);
        };
        statements_to_delete.push(Some(stmt));
        // The matched multi-dimensional foreach pattern guarantees a statement after stmt.
        statements_to_delete.push(self.next_non_empty_statement(stmt));
        let Some(item) = self.il_variable(foreach_variable)? else {
            return Ok(None);
        };
        let item_facts = self.variable(item)?;
        if !item_facts.single_definition || !item_facts.local_or_stack_slot {
            return Ok(None);
        }
        for ub in upper_bounds.iter().flatten() {
            let f = self.variable(*ub)?;
            if !(f.single_definition && f.loads == 1) {
                return Ok(None);
            }
        }
        // A lower bound without IL variable fails the managed transform with a
        // NullReferenceException; that never happens for compiler output.
        for lb in &lower_bounds {
            let Some(lb) = lb else {
                return Ok(None);
            };
            let f = self.variable(*lb)?;
            if !(f.stores == 2 && f.loads == 3 && f.addresses == 0) {
                return Ok(None);
            }
        }
        let body = self.tree.new_node(k::BLOCK_STATEMENT);
        for statement in statements {
            self.tree.detach(statement);
            self.add_child(body, block_statement::slot::STATEMENTS, statement);
        }
        let variable_type = self.foreach_variable_type(&item_facts)?;
        let in_expression = collection_node.ok_or("no collection")?;
        let foreach = self.new_foreach(item, variable_type, in_expression, body)?;
        for statement in statements_to_delete {
            let statement =
                statement.ok_or("no statement after the multi-dimensional array pattern")?;
            self.tree.detach(statement);
        }
        self.make_foreach_variable(item, foreach)?;
        self.replace(expression_statement, foreach)?;
        Ok(Some(foreach))
    }

    // ---- Automatic properties ----

    /// `CanTransformToAutomaticProperty`.
    fn can_transform_to_automatic_property(
        &mut self,
        property: u32,
        accessors_must_be_compiler_generated: bool,
    ) -> R<bool> {
        let p = self.property(property)?;
        if !p.can_get {
            return Ok(false);
        }
        if accessors_must_be_compiler_generated && !p.getter_compiler_generated {
            return Ok(false);
        }
        if p.has_setter {
            if accessors_must_be_compiler_generated && !p.setter_compiler_generated {
                return Ok(false);
            }
            if p.setter_readonly {
                return Ok(false);
            }
        }
        Ok(true)
    }

    /// The `fieldReference` of the automatic property patterns, if one matches.
    fn match_automatic_property(&mut self, p: SyntaxId) -> R<Option<SyntaxId>> {
        // Attributes, Modifiers, Name: any; ReturnType present; no initializer, no expression body.
        if self
            .child(p, property_declaration::slot::RETURN_TYPE)
            .is_none()
            || self
                .child(p, property_declaration::slot::INITIALIZER)
                .is_some()
            || self
                .child(p, property_declaration::slot::EXPRESSION_BODY)
                .is_some()
        {
            return Ok(None);
        }
        let Some(getter) = self.child(p, property_declaration::slot::GETTER) else {
            return Ok(None);
        };
        // { return $fieldReference; }
        let Some(ret) = self.is(
            self.single_statement_block(self.child(getter, accessor::slot::BODY)),
            k::RETURN_STATEMENT,
        ) else {
            return Ok(None);
        };
        let Some(field_reference) = self.child(ret, return_statement::slot::EXPRESSION) else {
            return Ok(None);
        };
        match self.child(p, property_declaration::slot::SETTER) {
            None => Ok(Some(field_reference)),
            Some(setter) => {
                // { $fieldReference = value; }
                let statement =
                    self.single_statement_block(self.child(setter, accessor::slot::BODY));
                let Some((left, right)) = self.assignment_statement(statement, Some(ASSIGN)) else {
                    return Ok(None);
                };
                if !self.nodes_match(Some(field_reference), left)?
                    || !self.identifier_named(right, "value")
                {
                    return Ok(None);
                }
                Ok(Some(field_reference))
            }
        }
    }

    /// `TransformAutomaticProperty`; the property stays, so the visit goes on as usual.
    fn transform_automatic_property(&mut self, p: SyntaxId) -> R<()> {
        let Some(property) = self.symbol_of(p, Category::Property)? else {
            return Ok(());
        };
        if self.has_accessed_through_property_field_attribute(p) {
            return Ok(());
        }
        if self.settings.field_keyword {
            return self.transform_field_backed_property(p, property);
        }
        if !self.can_transform_to_automatic_property(property, false)? {
            return Ok(());
        }
        let mut field = None;
        if let Some(field_reference) = self.match_automatic_property(p)? {
            field = self.symbol_of(field_reference, Category::Field)?;
        }
        let Some(field) = field else {
            return Ok(());
        };
        if backing_field_property_name(&self.member(field)?.name).is_none() {
            return Ok(());
        }
        // In generic types the accessor bodies reference the field specialized by the
        // type's own type parameters; the field declaration's symbol is the definition.
        let field = self.definition(field)?;
        if self.field(field)?.accessed_through_property {
            return Ok(());
        }
        let setter = self.child(p, property_declaration::slot::SETTER);
        if setter.is_some_and(|s| {
            self.modifiers(s, accessor::scalar::MODIFIERS) & modifiers::READONLY != 0
        }) || (self.modifiers(p, property_declaration::scalar::MODIFIERS) & modifiers::READONLY
            != 0
            && setter.is_some())
        {
            return Ok(());
        }
        if self.ask_flag(
            QUERY_PROPERTY_FIELD,
            &[u64::from(property), u64::from(field)],
        )? {
            let getter = self.child(p, property_declaration::slot::GETTER);
            if let Some(getter) = getter {
                self.remove_compiler_generated_attribute(getter, accessor::slot::ATTRIBUTES)?;
                self.tree.set_child(getter, accessor::slot::BODY, None);
            }
            if let Some(setter) = setter {
                self.remove_compiler_generated_attribute(setter, accessor::slot::ATTRIBUTES)?;
                self.tree.set_child(setter, accessor::slot::BODY, None);
            }
            self.remove_modifier(
                p,
                property_declaration::scalar::MODIFIERS,
                modifiers::READONLY,
            );
            if let Some(getter) = getter {
                self.remove_modifier(getter, accessor::scalar::MODIFIERS, modifiers::READONLY);
            }
            self.move_backing_field_declaration(p, field)?;
        }
        Ok(())
    }

    /// Removes the declaration of `field` next to the property `p` and adds its attributes, but
    /// those of a compiler-generated field, as `field:` attributes of the property.
    fn move_backing_field_declaration(&mut self, p: SyntaxId, field: u32) -> R<()> {
        let Some(parent) = self.parent(p) else {
            return Ok(());
        };
        let mut field_decl = None;
        for c in self.tree.all_children(parent) {
            if self.kind(c) == k::FIELD_DECLARATION && self.symbol(c)? == Some(field) {
                field_decl = Some(c);
                break;
            }
        }
        let Some(field_decl) = field_decl else {
            return Ok(());
        };
        self.tree.detach(field_decl);
        // Add C# 7.3 attributes on backing field:
        self.remove_attribute(field_decl, field_declaration::slot::ATTRIBUTES, |t| {
            t.compiler_generated_attribute
        })?;
        self.remove_attribute(field_decl, field_declaration::slot::ATTRIBUTES, |t| {
            t.debugger_browsable_attribute
        })?;
        for section in self.children(field_decl, field_declaration::slot::ATTRIBUTES) {
            self.set_name(
                section,
                attribute_section::slot::ATTRIBUTE_TARGET_TOKEN,
                "field",
            );
            self.tree.detach(section);
            self.add_child(p, property_declaration::slot::ATTRIBUTES, section);
        }
        Ok(())
    }

    /// The attribute sections of `n` in `slot` without the attributes whose type `is_known`
    /// accepts (`CSharpDecompiler.RemoveAttribute`), and without sections left empty.
    fn remove_attribute(
        &mut self,
        n: SyntaxId,
        slot: usize,
        is_known: fn(&TypeFacts) -> bool,
    ) -> R<()> {
        for section in self.children(n, slot) {
            for attr in self.children(section, attribute_section::slot::ATTRIBUTES) {
                let Some(ty) = self.child(attr, attribute::slot::TYPE) else {
                    continue;
                };
                if let Some(t) = self.type_symbol(ty)?
                    && is_known(&self.type_facts(t)?)
                {
                    self.tree.detach(attr);
                }
            }
            if self
                .tree
                .children(section, attribute_section::slot::ATTRIBUTES)
                .is_empty()
            {
                self.tree.detach(section);
            }
        }
        Ok(())
    }

    /// `RemoveCompilerGeneratedAttribute`.
    fn remove_compiler_generated_attribute(&mut self, n: SyntaxId, slot: usize) -> R<()> {
        self.remove_attribute(n, slot, |t| {
            t.full_name == "System.Runtime.CompilerServices.CompilerGeneratedAttribute"
        })
    }

    /// `HasAccessedThroughPropertyFieldAttribute`.
    fn has_accessed_through_property_field_attribute(&self, p: SyntaxId) -> bool {
        self.tree
            .children(p, property_declaration::slot::ATTRIBUTES)
            .iter()
            .any(|&section| {
                self.name(section, attribute_section::slot::ATTRIBUTE_TARGET_TOKEN)
                    .as_deref()
                    == Some("field")
                    && self
                        .tree
                        .children(section, attribute_section::slot::ATTRIBUTES)
                        .iter()
                        .any(|&attr| {
                            let text = self.type_text(self.child(attr, attribute::slot::TYPE));
                            text.ends_with("AccessedThroughProperty")
                                || text.ends_with("AccessedThroughPropertyAttribute")
                        })
            })
    }

    /// The end of `AstType.ToString()` that decides a name suffix: the name of a simple or
    /// member type without type arguments, nothing for other type syntax (whose text ends in
    /// a keyword, `>`, `?`, `]`, `*` or `)`).
    fn type_text(&self, ty: Option<SyntaxId>) -> String {
        let Some(ty) = ty else {
            return String::new();
        };
        match self.kind(ty) {
            k::SIMPLE_TYPE
                if self
                    .tree
                    .children(ty, simple_type::slot::TYPE_ARGUMENTS)
                    .is_empty() =>
            {
                self.name(ty, simple_type::slot::IDENTIFIER_TOKEN)
                    .unwrap_or_default()
            }
            k::MEMBER_TYPE
                if self
                    .tree
                    .children(ty, member_type::slot::TYPE_ARGUMENTS)
                    .is_empty() =>
            {
                self.name(ty, member_type::slot::MEMBER_NAME_TOKEN)
                    .unwrap_or_default()
            }
            _ => String::new(),
        }
    }

    /// `TransformFieldBackedProperty`.
    fn transform_field_backed_property(&mut self, p: SyntaxId, property: u32) -> R<()> {
        let Some(field) = self.property(property)?.backing_field else {
            return Ok(());
        };
        if self.field(field)?.accessed_through_property {
            return Ok(());
        }
        if !self.outside_references_are_expressible(p, field)? {
            // The field stays declared, so the "field" keyword references emitted by
            // ExpressionBuilder.ConvertField have to become ordinary field references again.
            let field_name = self.member(field)?.name;
            for ie in self.descendants(p) {
                if self.kind(ie) != k::IDENTIFIER_EXPRESSION
                    || self.identifier_expression_name(ie) != "field"
                {
                    continue;
                }
                if let Some(referenced) = self.symbol_of(ie, Category::Field)?
                    && self.definition(referenced)? == field
                {
                    self.set_name(
                        ie,
                        identifier_expression::slot::IDENTIFIER_TOKEN,
                        &field_name,
                    );
                }
            }
            return Ok(());
        }
        let getter = self.child(p, property_declaration::slot::GETTER);
        let setter = self.child(p, property_declaration::slot::SETTER);
        if self.settings.automatic_properties {
            // VB auto-properties do not mark their accessors [CompilerGenerated]; they are
            // recognized by their "_<PropertyName>" backing field instead.
            let field_name = self.member(field)?.name;
            let property_name = self.member(property)?.name;
            let must_be_compiler_generated = field_name != format!("_{property_name}");
            self.collapse_trivial_accessor(getter, true, field, must_be_compiler_generated)?;
            // A readonly setter cannot become an auto-accessor; readonly getters collapse fine
            // because auto-getters are implicitly readonly.
            let setter_readonly = setter.is_some_and(|s| {
                self.modifiers(s, accessor::scalar::MODIFIERS) & modifiers::READONLY != 0
            });
            let property_readonly = self.modifiers(p, property_declaration::scalar::MODIFIERS)
                & modifiers::READONLY
                != 0;
            if !setter_readonly && !(property_readonly && setter.is_some()) {
                self.collapse_trivial_accessor(setter, false, field, must_be_compiler_generated)?;
            }
        }
        let has_body = |t: &Self, a: Option<SyntaxId>| {
            a.is_some_and(|a| t.child(a, accessor::slot::BODY).is_some())
        };
        if !has_body(self, getter) && !has_body(self, setter) {
            // The property became a full auto-property; readonly is implied.
            self.remove_modifier(
                p,
                property_declaration::scalar::MODIFIERS,
                modifiers::READONLY,
            );
            if let Some(getter) = getter {
                self.remove_modifier(getter, accessor::scalar::MODIFIERS, modifiers::READONLY);
            }
        }
        self.move_backing_field_declaration(p, field)
    }

    /// `CollapseTrivialAccessor` with `trivialFieldGetterBody` (`getter`) or
    /// `trivialFieldSetterBody`.
    fn collapse_trivial_accessor(
        &mut self,
        accessor: Option<SyntaxId>,
        getter: bool,
        field: u32,
        must_be_compiler_generated: bool,
    ) -> R<()> {
        let Some(accessor) = accessor else {
            return Ok(());
        };
        let Some(body) = self.child(accessor, accessor::slot::BODY) else {
            return Ok(());
        };
        if must_be_compiler_generated {
            let method = self.symbol_of(accessor, Category::Method)?;
            match method {
                Some(m) if self.member(m)?.compiler_generated => {}
                _ => return Ok(()),
            }
        }
        let statement = self.single_statement_block(Some(body));
        let field_reference = if getter {
            // { return field; }
            let Some(ret) = self.is(statement, k::RETURN_STATEMENT) else {
                return Ok(());
            };
            self.child(ret, return_statement::slot::EXPRESSION)
        } else {
            // { field = value; }
            let Some((left, right)) = self.assignment_statement(statement, Some(ASSIGN)) else {
                return Ok(());
            };
            if !self.identifier_named(right, "value") {
                return Ok(());
            }
            left
        };
        let Some(field_reference) = field_reference else {
            return Ok(());
        };
        if !self.identifier_named(Some(field_reference), "field") {
            return Ok(());
        }
        match self.symbol_of(field_reference, Category::Field)? {
            Some(referenced) if self.definition(referenced)? == field => {}
            _ => return Ok(()),
        }
        self.remove_compiler_generated_attribute(accessor, accessor::slot::ATTRIBUTES)?;
        // Auto-accessors are implicitly readonly.
        self.remove_modifier(accessor, accessor::scalar::MODIFIERS, modifiers::READONLY);
        // Clearing the accessor body turns it into an auto-property accessor.
        self.tree.set_child(accessor, accessor::slot::BODY, None);
        Ok(())
    }

    /// `OutsideReferencesAreExpressible`, with the verdicts computed once per tree.
    fn outside_references_are_expressible(&mut self, node: SyntaxId, field: u32) -> R<bool> {
        let root = self.ancestors(node).last().copied().unwrap_or(node);
        if self.outside_reference_root != Some(root) {
            self.outside_reference_root = Some(root);
            self.outside_reference_verdicts = self.build_outside_reference_index(root)?;
        }
        let definition = self.definition(field)?;
        Ok(self
            .outside_reference_verdicts
            .get(&definition)
            .copied()
            .unwrap_or(true))
    }

    /// `BuildOutsideReferenceIndex`.
    fn build_outside_reference_index(&mut self, root: SyntaxId) -> R<HashMap<u32, bool>> {
        let mut verdicts = HashMap::new();
        for node in self.descendants(root) {
            if !matches!(
                self.kind(node),
                k::IDENTIFIER_EXPRESSION | k::MEMBER_REFERENCE_EXPRESSION
            ) {
                continue;
            }
            let Some(referenced) = self.symbol_of(node, Category::Field)? else {
                continue;
            };
            let definition = self.definition(referenced)?;
            // A reference inside the accessors of the field's own property is exactly what
            // the "field" keyword expresses; anything else is an outside reference.
            let ancestors = self.ancestors(node);
            let owner_declaration = ancestors
                .iter()
                .copied()
                .find(|&a| self.kind(a) == k::PROPERTY_DECLARATION);
            if let Some(owner_declaration) = owner_declaration
                && let Some(owner) = self.symbol_of(owner_declaration, Category::Property)?
                && let Some(owner_field) = self.property(owner)?.backing_field
                && definition == self.definition(owner_field)?
            {
                continue;
            }
            let enclosing = ancestors
                .iter()
                .copied()
                .find(|&a| is_entity_declaration(self.kind(a)));
            let enclosing_method = match enclosing {
                Some(e) => self.symbol_of(e, Category::Method)?,
                None => None,
            };
            if !self.is_constructor_store(node, definition, enclosing_method)?
                || !self.ask_flag(QUERY_STORE_SURVIVES, &[u64::from(definition)])?
            {
                verdicts.insert(definition, false);
            } else {
                verdicts.entry(definition).or_insert(true);
            }
        }
        Ok(verdicts)
    }

    /// `IsConstructorStore`.
    fn is_constructor_store(
        &mut self,
        node: SyntaxId,
        field: u32,
        enclosing_method: Option<u32>,
    ) -> R<bool> {
        let mut current = node;
        let mut via_deconstruction = false;
        loop {
            let Some(parent) = self.parent(current) else {
                return Ok(false);
            };
            if self.kind(parent) == k::ASSIGNMENT_EXPRESSION
                && self.scalar_int(parent, assignment_expression::scalar::OPERATOR) == ASSIGN
                && self.child(parent, assignment_expression::slot::LEFT) == Some(current)
            {
                break;
            }
            if self.kind(parent) == k::TUPLE_EXPRESSION {
                via_deconstruction = true;
                current = parent;
                continue;
            }
            return Ok(false);
        }
        if via_deconstruction {
            match self.field(field)?.auto_property {
                Some(property) if !self.property(property)?.can_set => {}
                _ => return Ok(false),
            }
        }
        let Some(ctor) = enclosing_method else {
            return Ok(false);
        };
        let ctor = self.member(ctor)?;
        let field = self.member(field)?;
        Ok(ctor.is_constructor
            && ctor.is_static == field.is_static
            && ctor.declaring_type_definition == field.declaring_type_definition)
    }

    /// `ReplaceBackingFieldUsage`: the new identifier, if the use of a backing field becomes a
    /// use of its property.
    fn replace_backing_field_usage(&mut self, identifier: SyntaxId) -> R<Option<SyntaxId>> {
        let Some(parent) = self.parent(identifier) else {
            return Ok(None);
        };
        let Some((mrr_handle, mrr)) = self.first_annotation(parent, |r| r.is_member)? else {
            return Ok(None);
        };
        let Some(field) = mrr.member else {
            return Ok(None);
        };
        let field_facts = self.member(field)?;
        if field_facts.category != Category::Field {
            return Ok(None);
        }
        let Some(property) = self.field(field)?.auto_property else {
            return Ok(None);
        };
        if let Some(method) = self.current_method
            && self.member(method)?.accessor_owner == Some(property)
        {
            return Ok(None);
        }
        if self.settings.field_keyword
            && !self.backing_field_will_be_removed(property, field, parent)?
        {
            return Ok(None);
        }
        let property_facts = self.property(property)?;
        let property_name = self.member(property)?.name;
        let vb_field =
            field_facts.compiler_generated && field_facts.name == format!("_{property_name}");
        if self.settings.automatic_properties
            && self.can_transform_to_automatic_property(property, !vb_field)?
        {
            if !property_facts.can_set && !self.settings.getter_only_automatic_properties {
                return Ok(None);
            }
        } else if self.settings.field_keyword
            && !property_facts.can_set
            && self.is_constructor_store(parent, field, self.current_method)?
        {
            // A setter-less field-backed property keeping explicit accessors is still
            // assignable by name inside a constructor of its declaring type.
        } else {
            // The store stays a field reference, which TransformFieldAndConstructorInitializers
            // lifts into the property initializer.
            return Ok(None);
        }
        let annotations = self.tree.node(parent).annotations.clone();
        let mut kept = Vec::with_capacity(annotations.len());
        for h in annotations {
            if !self.resolve(h)?.is_member {
                kept.push(h);
            }
        }
        let mut payload = Vec::new();
        push_uvar(&mut payload, u64::from(mrr_handle));
        push_uvar(&mut payload, u64::from(property));
        let handle = self
            .tree
            .new_annotation(ANNOTATION_PROPERTY_REFERENCE, payload);
        // The type of the new resolve result is the property's, which no query of this
        // transform asks for on a member reference.
        self.resolves.insert(
            handle,
            Resolve {
                is_rr: true,
                is_member: true,
                symbol: Some(property),
                member: Some(property),
                ..Resolve::default()
            },
        );
        kept.push(handle);
        self.tree.node_mut(parent).annotations = kept;
        Ok(Some(self.new_identifier(&property_name)))
    }

    /// `BackingFieldWillBeRemoved`.
    fn backing_field_will_be_removed(
        &mut self,
        property: u32,
        field: u32,
        node: SyntaxId,
    ) -> R<bool> {
        if !self.settings.field_keyword {
            return Ok(false);
        }
        let p = self.property(property)?;
        if !(p.can_set || self.settings.getter_only_automatic_properties) {
            return Ok(false);
        }
        let Some(backing_field) = p.backing_field else {
            return Ok(false);
        };
        if self.definition(field)? != self.definition(backing_field)? {
            return Ok(false);
        }
        self.outside_references_are_expressible(node, backing_field)
    }

    // ---- Automatic events ----

    fn visit_event_declaration(&mut self, ev: SyntaxId) -> R<()> {
        if !self.settings.automatic_events {
            return Ok(());
        }
        let Some(event) = self.symbol_of(ev, Category::Event)? else {
            return Ok(());
        };
        let Some(parent) = self.parent(ev) else {
            return Ok(());
        };
        for fd in self.tree.all_children(parent) {
            if self.kind(fd) != k::FIELD_DECLARATION {
                continue;
            }
            if self
                .tree
                .children(fd, field_declaration::slot::VARIABLES)
                .len()
                > 1
            {
                continue;
            }
            let Some(field) = self.symbol_of(fd, Category::Field)? else {
                continue;
            };
            if self.ask_flag(QUERY_EVENT_FIELD, &[u64::from(field), u64::from(event)])? {
                self.tree.detach(fd);
                break;
            }
        }
        Ok(())
    }

    // ---- Destructor ----

    /// `MatchDestructorBody`: the try block of `try { ... } finally { base.Finalize(); }`.
    fn match_destructor_body(&self, body: SyntaxId) -> Option<SyntaxId> {
        let statement = self.first_non_empty_statement(body)?;
        if self.kind(statement) != k::TRY_CATCH_STATEMENT
            || self.next_non_empty_statement(statement).is_some()
        {
            return None;
        }
        if !self
            .tree
            .children(statement, try_catch_statement::slot::CATCH_CLAUSES)
            .is_empty()
        {
            return None;
        }
        let try_block = self.child(statement, try_catch_statement::slot::TRY_BLOCK)?;
        let finally_block = self.child(statement, try_catch_statement::slot::FINALLY_BLOCK)?;
        let finalize_call = self.first_non_empty_statement(finally_block)?;
        if self.next_non_empty_statement(finalize_call).is_some() {
            return None;
        }
        // base.Finalize();
        let s = self.is(Some(finalize_call), k::EXPRESSION_STATEMENT)?;
        let call = self.is(
            self.child(s, expression_statement::slot::EXPRESSION),
            k::INVOCATION_EXPRESSION,
        )?;
        if !self
            .tree
            .children(call, invocation_expression::slot::ARGUMENTS)
            .is_empty()
        {
            return None;
        }
        self.is(
            self.member_reference(
                self.child(call, invocation_expression::slot::TARGET),
                "Finalize",
            ),
            k::BASE_REFERENCE_EXPRESSION,
        )?;
        Some(try_block)
    }

    /// `MovePlaceholderComments`.
    fn move_placeholder_comments(&mut self, old_body: SyntaxId, new_body: SyntaxId) {
        let anchor = self
            .tree
            .children(new_body, block_statement::slot::STATEMENTS)
            .first()
            .copied();
        for placeholder in self.children(old_body, block_statement::slot::STATEMENTS) {
            if self.kind(placeholder) != k::EMPTY_STATEMENT {
                continue;
            }
            self.tree.detach(placeholder);
            let statements = self
                .tree
                .children(new_body, block_statement::slot::STATEMENTS);
            let index = anchor
                .and_then(|a| statements.iter().position(|&s| s == a))
                .unwrap_or(statements.len());
            self.tree.insert_child(
                new_body,
                block_statement::slot::STATEMENTS,
                index,
                placeholder,
            );
        }
    }

    /// `TransformDestructor`.
    fn transform_destructor(&mut self, method: SyntaxId) -> R<Option<SyntaxId>> {
        // destructorPattern
        let is_void = self
            .is(
                self.child(method, method_declaration::slot::RETURN_TYPE),
                k::PRIMITIVE_TYPE,
            )
            .is_some_and(|t| {
                self.tree.node(t).scalars[primitive_type::scalar::KEYWORD]
                    .as_str()
                    .as_deref()
                    == Some("void")
            });
        if !is_void
            || self
                .name(method, method_declaration::slot::NAME_TOKEN)
                .as_deref()
                != Some("Finalize")
            || self
                .child(
                    method,
                    method_declaration::slot::PRIVATE_IMPLEMENTATION_TYPE,
                )
                .is_some()
            || !self
                .tree
                .children(method, method_declaration::slot::TYPE_PARAMETERS)
                .is_empty()
            || !self
                .tree
                .children(method, method_declaration::slot::PARAMETERS)
                .is_empty()
            || !self
                .tree
                .children(method, method_declaration::slot::CONSTRAINTS)
                .is_empty()
        {
            return Ok(None);
        }
        let Some(old_body) = self.child(method, method_declaration::slot::BODY) else {
            return Ok(None);
        };
        let Some(try_block) = self.match_destructor_body(old_body) else {
            return Ok(None);
        };
        let dd = self.tree.new_node(k::DESTRUCTOR_DECLARATION);
        for section in self.children(method, method_declaration::slot::ATTRIBUTES) {
            self.tree.detach(section);
            self.add_child(dd, destructor_declaration::slot::ATTRIBUTES, section);
        }
        self.copy_annotations(dd, method);
        let m = self.modifiers(method, method_declaration::scalar::MODIFIERS)
            & !(modifiers::PROTECTED | modifiers::OVERRIDE);
        self.set_scalar(
            dd,
            destructor_declaration::scalar::MODIFIERS,
            Scalar::Int(i64::from(m)),
        );
        self.move_placeholder_comments(old_body, try_block);
        self.tree.detach(try_block);
        self.tree
            .set_child(dd, destructor_declaration::slot::BODY, Some(try_block));
        // A destructor only appears inside a type declaration, so the context tracker has an
        // enclosing type at this point.
        let current_type = self
            .current_type
            .ok_or("a Finalize method outside a type declaration")?;
        let name = self.member(current_type)?.name;
        self.set_name(dd, destructor_declaration::slot::NAME_TOKEN, &name);
        self.replace(method, dd)?;
        Ok(Some(dd))
    }

    /// `TransformDestructorBody`.
    fn transform_destructor_body(&mut self, dtor: SyntaxId) -> Option<SyntaxId> {
        let old_body = self.child(dtor, destructor_declaration::slot::BODY)?;
        let try_block = self.match_destructor_body(old_body)?;
        self.move_placeholder_comments(old_body, try_block);
        self.tree.detach(try_block);
        self.tree
            .set_child(dtor, destructor_declaration::slot::BODY, Some(try_block));
        Some(dtor)
    }

    // ---- Try-catch-finally ----

    /// `TransformTryCatchFinally`: `try { try {} catch {} } finally {}` merged.
    fn transform_try_catch_finally(&mut self, try_finally: SyntaxId) {
        if !self
            .tree
            .children(try_finally, try_catch_statement::slot::CATCH_CLAUSES)
            .is_empty()
            || self
                .child(try_finally, try_catch_statement::slot::FINALLY_BLOCK)
                .is_none()
        {
            return;
        }
        let Some(try_catch) = self.is(
            self.single_statement_block(
                self.child(try_finally, try_catch_statement::slot::TRY_BLOCK),
            ),
            k::TRY_CATCH_STATEMENT,
        ) else {
            return;
        };
        let Some(inner_try) = self.child(try_catch, try_catch_statement::slot::TRY_BLOCK) else {
            return;
        };
        if self
            .child(try_catch, try_catch_statement::slot::FINALLY_BLOCK)
            .is_some()
        {
            return;
        }
        self.tree.detach(inner_try);
        self.tree.set_child(
            try_finally,
            try_catch_statement::slot::TRY_BLOCK,
            Some(inner_try),
        );
        for clause in self.children(try_catch, try_catch_statement::slot::CATCH_CLAUSES) {
            self.tree.detach(clause);
            self.add_child(
                try_finally,
                try_catch_statement::slot::CATCH_CLAUSES,
                clause,
            );
        }
    }

    // ---- Cascading if-else, logic operators ----

    /// `SimplifyCascadingIfElseStatements`.
    fn simplify_cascading_if_else_statements(&mut self, node: SyntaxId) {
        if self
            .child(node, if_else_statement::slot::CONDITION)
            .is_none()
            || self
                .child(node, if_else_statement::slot::TRUE_STATEMENT)
                .is_none()
        {
            return;
        }
        let Some(else_if) = self.is(
            self.single_statement_block(self.child(node, if_else_statement::slot::FALSE_STATEMENT)),
            k::IF_ELSE_STATEMENT,
        ) else {
            return;
        };
        if self
            .child(else_if, if_else_statement::slot::CONDITION)
            .is_none()
            || self
                .child(else_if, if_else_statement::slot::TRUE_STATEMENT)
                .is_none()
        {
            return;
        }
        self.tree.detach(else_if);
        self.tree.set_child(
            node,
            if_else_statement::slot::FALSE_STATEMENT,
            Some(else_if),
        );
    }

    /// `VisitBinaryOperatorExpression`: `a && (b && c)` becomes `(a && b) && c`.
    fn visit_binary_operator_expression(&mut self, expr: SyntaxId) -> R<SyntaxId> {
        let op = self.scalar_int(expr, binary_operator_expression::scalar::OPERATOR);
        if (op == BinaryOp::ConditionalAnd as i64 || op == BinaryOp::ConditionalOr as i64)
            && let Some(b_and_c) = self.is(
                self.child(expr, binary_operator_expression::slot::RIGHT),
                k::BINARY_OPERATOR_EXPRESSION,
            )
            && self.scalar_int(b_and_c, binary_operator_expression::scalar::OPERATOR) == op
        {
            // A conditional-and/or operator always has both operands present.
            let b = self
                .child(b_and_c, binary_operator_expression::slot::LEFT)
                .ok_or("a conditional operator without left operand")?;
            let c = self
                .child(b_and_c, binary_operator_expression::slot::RIGHT)
                .ok_or("a conditional operator without right operand")?;
            self.tree.detach(b);
            self.tree.detach(c);
            self.tree.detach(b_and_c);
            self.replace(expr, b_and_c)?;
            self.tree
                .set_child(b_and_c, binary_operator_expression::slot::LEFT, Some(expr));
            self.tree
                .set_child(b_and_c, binary_operator_expression::slot::RIGHT, Some(c));
            self.tree
                .set_child(expr, binary_operator_expression::slot::RIGHT, Some(b));
            return self.visit_children(b_and_c);
        }
        self.visit_children(expr)
    }

    /// `VisitUnaryOperatorExpression`: `!(a == b)` becomes `a != b`.
    fn visit_unary_operator_expression(&mut self, expr: SyntaxId) -> R<SyntaxId> {
        if self.scalar_int(expr, unary_operator_expression::scalar::OPERATOR) == UNARY_NOT
            && let Some(binary) = self.is(
                self.child(expr, unary_operator_expression::slot::EXPRESSION),
                k::BINARY_OPERATOR_EXPRESSION,
            )
            && self.scalar_int(binary, binary_operator_expression::scalar::OPERATOR)
                == BinaryOp::Equality as i64
        {
            self.set_scalar(
                binary,
                binary_operator_expression::scalar::OPERATOR,
                Scalar::Int(BinaryOp::InEquality as i64),
            );
            self.tree.detach(binary);
            self.replace(expr, binary)?;
            return self.visit_binary_operator_expression(binary);
        }
        self.visit_children(expr)
    }

    // ---- fixed, using ----

    /// `VisitFixedStatement`: `fixed (T* p = &x.GetPinnableReference())` of a value type
    /// becomes `fixed (T* p = x)`.
    fn visit_fixed_statement(&mut self, fixed: SyntaxId) -> R<()> {
        if !self.settings.pattern_based_fixed_statement {
            return Ok(());
        }
        for v in self.children(fixed, fixed_statement::slot::VARIABLES) {
            let initializer = self.child(v, variable_initializer::slot::INITIALIZER);
            let Some(unary) = self.is(initializer, k::UNARY_OPERATOR_EXPRESSION) else {
                continue;
            };
            if self.scalar_int(unary, unary_operator_expression::scalar::OPERATOR)
                != UNARY_ADDRESS_OF
            {
                continue;
            }
            let Some(call) = self.is(
                self.child(unary, unary_operator_expression::slot::EXPRESSION),
                k::INVOCATION_EXPRESSION,
            ) else {
                continue;
            };
            if !self
                .tree
                .children(call, invocation_expression::slot::ARGUMENTS)
                .is_empty()
            {
                continue;
            }
            let Some(target) = self.member_reference(
                self.child(call, invocation_expression::slot::TARGET),
                "GetPinnableReference",
            ) else {
                continue;
            };
            let ty = self.resolve_result(target)?.and_then(|r| r.ty);
            let is_reference = match ty {
                Some(t) => self.type_facts(t)?.is_reference,
                None => None,
            };
            if is_reference == Some(false) {
                self.tree.detach(target);
                self.tree
                    .set_child(v, variable_initializer::slot::INITIALIZER, Some(target));
            }
        }
        Ok(())
    }

    /// `VisitUsingStatement`: a using statement that ends its block becomes a using declaration.
    fn visit_using_statement(&mut self, using: SyntaxId) -> R<SyntaxId> {
        self.visit_children(using)?;
        if !self.settings.use_enhanced_using {
            return Ok(using);
        }
        if self.next_non_empty_statement(using).is_some()
            || self
                .parent(using)
                .is_none_or(|p| self.kind(p) != k::BLOCK_STATEMENT)
        {
            return Ok(using);
        }
        if self
            .is(
                self.child(using, using_statement::slot::RESOURCE_ACQUISITION),
                k::VARIABLE_DECLARATION_STATEMENT,
            )
            .is_none()
        {
            return Ok(using);
        }
        self.set_scalar(
            using,
            using_statement::scalar::IS_ENHANCED,
            Scalar::Bool(true),
        );
        Ok(using)
    }
}

/// `DescendIntoStatement`: not into expressions, expression statements and nested loops.
fn descend_into_statement(kind: u16) -> bool {
    !(is_expression(kind)
        || matches!(
            kind,
            k::EXPRESSION_STATEMENT
                | k::WHILE_STATEMENT
                | k::FOREACH_STATEMENT
                | k::DO_WHILE_STATEMENT
                | k::FOR_STATEMENT
        ))
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::csharp::syntax_oracle::NoOracle;

    #[test]
    fn backing_field_names() {
        assert_eq!(
            backing_field_property_name("<Name>k__BackingField"),
            Some("Name")
        );
        assert_eq!(backing_field_property_name("_Name"), Some("Name"));
        assert_eq!(backing_field_property_name("<>k__BackingField"), None);
        assert_eq!(backing_field_property_name("_"), None);
        assert_eq!(backing_field_property_name("name"), None);
        // `.+` is greedy: the last `>k__BackingField` ends the name.
        assert_eq!(
            backing_field_property_name("<a>k__BackingField>k__BackingField"),
            Some("a>k__BackingField")
        );
        // The VB form takes everything after the first underscore.
        assert_eq!(backing_field_property_name("__x"), Some("_x"));
    }

    /// A block with an empty statement, a statement and a trailing empty statement.
    fn block(tree: &mut SyntaxTree) -> (SyntaxId, [SyntaxId; 3]) {
        let b = tree.new_node(k::BLOCK_STATEMENT);
        let e1 = tree.new_node(k::EMPTY_STATEMENT);
        let s = tree.new_node(k::BREAK_STATEMENT);
        let e2 = tree.new_node(k::EMPTY_STATEMENT);
        for (i, c) in [e1, s, e2].into_iter().enumerate() {
            tree.insert_child(b, block_statement::slot::STATEMENTS, i, c);
        }
        (b, [e1, s, e2])
    }

    struct SettingsOnly;

    impl SyntaxOracle for SettingsOnly {
        fn ask(&self, query: &[u8]) -> Option<Vec<u8>> {
            (query[0] == QUERY_SETTINGS).then(|| vec![0, 0, 0])
        }
    }

    #[test]
    fn statement_navigation_skips_empty_statements() {
        let mut tree = SyntaxTree::default();
        let (b, [e1, s, e2]) = block(&mut tree);
        let t = Pst::new(&mut tree, &SettingsOnly).unwrap();
        assert_eq!(t.first_non_empty_statement(b), Some(s));
        assert_eq!(t.next_non_empty_statement(e1), Some(s));
        assert_eq!(t.next_non_empty_statement(s), None);
        assert_eq!(t.next_sibling(s), Some(e2));
        assert_eq!(t.descendants(b), vec![e1, s, e2]);
        assert!(Pst::new(&mut SyntaxTree::default(), &NoOracle).is_err());
    }

    #[test]
    fn placeholders_move_before_the_first_statement() {
        let mut tree = SyntaxTree::default();
        let (old, [e1, s, e2]) = block(&mut tree);
        let new = tree.new_node(k::BLOCK_STATEMENT);
        let first = tree.new_node(k::RETURN_STATEMENT);
        tree.insert_child(new, block_statement::slot::STATEMENTS, 0, first);
        let mut t = Pst::new(&mut tree, &SettingsOnly).unwrap();
        t.move_placeholder_comments(old, new);
        assert_eq!(
            tree.children(new, block_statement::slot::STATEMENTS),
            &[e1, e2, first]
        );
        assert_eq!(tree.children(old, block_statement::slot::STATEMENTS), &[s]);
    }

    #[test]
    fn conditional_and_chains_reassociate_to_the_left() {
        // a && (b && c) under an expression statement
        let mut tree = SyntaxTree::default();
        let stmt = tree.new_node(k::EXPRESSION_STATEMENT);
        let outer = tree.new_node(k::BINARY_OPERATOR_EXPRESSION);
        let inner = tree.new_node(k::BINARY_OPERATOR_EXPRESSION);
        let [a, b, c] = [0, 1, 2].map(|_| tree.new_node(k::NULL_REFERENCE_EXPRESSION));
        let and = Scalar::Int(BinaryOp::ConditionalAnd as i64);
        tree.node_mut(outer).scalars[binary_operator_expression::scalar::OPERATOR] = and.clone();
        tree.node_mut(inner).scalars[binary_operator_expression::scalar::OPERATOR] = and;
        tree.set_child(inner, binary_operator_expression::slot::LEFT, Some(b));
        tree.set_child(inner, binary_operator_expression::slot::RIGHT, Some(c));
        tree.set_child(outer, binary_operator_expression::slot::LEFT, Some(a));
        tree.set_child(outer, binary_operator_expression::slot::RIGHT, Some(inner));
        tree.set_child(stmt, expression_statement::slot::EXPRESSION, Some(outer));
        tree.root = Some(stmt);
        let mut t = Pst::new(&mut tree, &SettingsOnly).unwrap();
        t.visit(stmt).unwrap();
        assert_eq!(
            tree.child(stmt, expression_statement::slot::EXPRESSION),
            Some(inner)
        );
        assert_eq!(
            tree.child(inner, binary_operator_expression::slot::LEFT),
            Some(outer)
        );
        assert_eq!(
            tree.child(inner, binary_operator_expression::slot::RIGHT),
            Some(c)
        );
        assert_eq!(
            tree.child(outer, binary_operator_expression::slot::LEFT),
            Some(a)
        );
        assert_eq!(
            tree.child(outer, binary_operator_expression::slot::RIGHT),
            Some(b)
        );
    }
}
