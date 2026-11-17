//! What the engine learns about the host objects a syntax tree refers to by handle (annotations,
//! literal values): it asks the host, which answers from the object itself. A transform that
//! needs another fact about an annotation adds a query kind here and its answer in the host's
//! `SyntaxOracle.Answer` (`Interop/SyntaxRoundTrip.cs`).
//!
//! A query is its kind byte, the handle (unsigned LEB128), and any arguments; an answer is
//! bytes whose layout the kind defines, or none where the question does not apply to the object.

/// Answers queries about the host objects of a syntax tree.
pub trait SyntaxOracle {
    /// The host's answer to `query`, `None` where the question does not apply.
    fn ask(&self, query: &[u8]) -> Option<Vec<u8>>;
}

/// The class name of the object (`GetType().Name`), as UTF-8.
pub const QUERY_CLASS_NAME: u8 = 0;
/// For a `ResolveResult`: the reflection name of its `Type` (`IType.ReflectionName`), as UTF-8.
pub const QUERY_RESOLVE_RESULT_TYPE: u8 = 1;
/// For a `ResolveResult` with a symbol (`GetSymbol()`): the symbol kind (`SymbolKind` as one
/// byte), then the reflection name of the declaring type (empty for none) and the symbol's name,
/// each as a length (LEB128) and UTF-8.
pub const QUERY_SYMBOL: u8 = 2;
/// For one of the `AddCheckedBlocks` annotations: 0 for `CheckedAnnotation`, 1 for
/// `UncheckedAnnotation`, 2 for `ExplicitUncheckedAnnotation`, as one byte.
pub const QUERY_CHECKED_STATE: u8 = 80;
/// The `CheckForOverflowUnderflow` setting of the request, as one byte (0 or 1); the handle is
/// ignored. No answer where the host did not pass its settings.
pub const QUERY_CHECK_FOR_OVERFLOW_UNDERFLOW: u8 = 81;

// The query kinds of `DeclareVariables` (96..=111); `transforms::declare_variables` documents
// their arguments and answers.
/// What an annotation is to `DeclareVariables`: an ILVariable, a scope, a function, ...
pub const QUERY_DV_ANNOTATION: u8 = 96;
/// The facts of an ILVariable, by its object id.
pub const QUERY_DV_VARIABLE: u8 = 97;
/// The decompiler settings `DeclareVariables` reads.
pub const QUERY_DV_SETTINGS: u8 = 98;
/// `ExpressionBuilder.HidesVariableWithName(function, "_")`, by the function's object id.
pub const QUERY_DV_HIDES_DISCARD: u8 = 99;
/// Registers the temporary of an invalid expression statement; its object id.
pub const QUERY_DV_TEMPORARY_VARIABLE: u8 = 100;
/// `TypeSystemAstBuilder.ConvertType` of an ILVariable's type, as a subtree.
pub const QUERY_DV_VARIABLE_TYPE: u8 = 101;
/// `TypeSystemAstBuilder.ConvertType` of `System.Runtime.CompilerServices.Unsafe`, as a subtree.
pub const QUERY_DV_UNSAFE_TYPE: u8 = 102;
/// The assignments of a `DeconstructInstruction` annotation.
pub const QUERY_DV_DECONSTRUCTION: u8 = 103;
/// `StatementBuilder.TranslateDeconstructionDesignation` of a `DeconstructInstruction`, as a
/// subtree.
pub const QUERY_DV_DESIGNATION: u8 = 104;
/// The tag of every host annotation, one byte per handle.
pub const QUERY_DV_ANNOTATION_TAGS: u8 = 105;
/// For a `ResolveResult` (none for any other object): one byte of `UNSAFE_*` flags, the facts
/// `IntroduceUnsafeModifier` reads about it. A type is unsafe if it is a pointer or function
/// pointer, or a by-reference or array type of an unsafe element type.
pub const QUERY_UNSAFE_FACTS: u8 = 64;

/// The resolve result's type is unsafe.
pub const UNSAFE_TYPE: u8 = 1;
/// A `MemberResolveResult` whose member is an `IParameterizedMember`.
pub const UNSAFE_PARAMETERIZED_MEMBER: u8 = 2;
/// A `MethodGroupResolveResult`.
pub const UNSAFE_METHOD_GROUP: u8 = 4;
/// The parameterized member (or the chosen method of the method group) has a parameter of
/// unsafe type.
pub const UNSAFE_PARAMETER: u8 = 8;
/// The parameterized member (or the chosen method of the method group) has an unsafe return
/// type.
pub const UNSAFE_RETURN_TYPE: u8 = 16;
/// An `OperatorResolveResult` whose first operand is of pointer type.
pub const UNSAFE_POINTER_OPERAND: u8 = 32;

fn query(kind: u8, handle: u32) -> Vec<u8> {
    let mut q = vec![kind];
    let mut v = handle;
    loop {
        let b = (v & 0x7F) as u8;
        v >>= 7;
        if v == 0 {
            q.push(b);
            break;
        }
        q.push(b | 0x80);
    }
    q
}

/// The class name of the host object `handle`.
pub fn class_name(oracle: &dyn SyntaxOracle, handle: u32) -> Option<String> {
    oracle
        .ask(&query(QUERY_CLASS_NAME, handle))
        .map(|b| String::from_utf8_lossy(&b).into_owned())
}

/// The reflection name of the type of the resolve result `handle`.
pub fn resolve_result_type(oracle: &dyn SyntaxOracle, handle: u32) -> Option<String> {
    oracle
        .ask(&query(QUERY_RESOLVE_RESULT_TYPE, handle))
        .map(|b| String::from_utf8_lossy(&b).into_owned())
}

/// The `UNSAFE_*` flags of the host object `handle`, `None` if it is no resolve result.
pub fn unsafe_facts(oracle: &dyn SyntaxOracle, handle: u32) -> Option<u8> {
    oracle
        .ask(&query(QUERY_UNSAFE_FACTS, handle))
        .and_then(|b| b.first().copied())
}

/// The symbol of a resolve result: its kind (`SymbolKind` value), declaring type and name.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct SymbolInfo {
    pub kind: u8,
    pub declaring_type: String,
    pub name: String,
}

/// The symbol of the resolve result `handle`.
pub fn symbol(oracle: &dyn SyntaxOracle, handle: u32) -> Option<SymbolInfo> {
    let b = oracle.ask(&query(QUERY_SYMBOL, handle))?;
    let (&kind, mut rest) = b.split_first()?;
    let mut text = || -> Option<String> {
        let mut len = 0usize;
        let mut shift = 0;
        loop {
            let (&x, r) = rest.split_first()?;
            rest = r;
            len |= usize::from(x & 0x7F) << shift;
            if x & 0x80 == 0 {
                break;
            }
            shift += 7;
        }
        let (s, r) = rest.split_at_checked(len)?;
        rest = r;
        Some(String::from_utf8_lossy(s).into_owned())
    };
    let declaring_type = text()?;
    let name = text()?;
    Some(SymbolInfo {
        kind,
        declaring_type,
        name,
    })
}

/// For a `ResolveResult`: the `KnownTypeCode` of the definition of its `Type` and of the
/// definition of the type's nullable underlying type (0 for none), `IsByRefLike`,
/// `IsReferenceType` (0 false, 1 true, 2 unknown), and whether it is an
/// `InvocationResolveResult` of `string.Concat`; one byte each.
pub const QUERY_RESOLVE_RESULT_FACTS: u8 = 48;
/// For a `ResolveResult` whose symbol (as `AstNode.GetSymbol` reads it) is a method: its name,
/// its full name (each a length and UTF-8), the `KnownTypeCode` of its declaring type, whether
/// `ILInlining.MethodRequiresCopyForReadonlyLValue` holds (a byte), its type argument count
/// (LEB128), whether the first type argument is a type parameter with the default constructor
/// constraint (a byte), its parameter count (LEB128), and per parameter the `KnownTypeCode` of
/// its type, `IsParams`, whether the type is an `ArrayType` and the `KnownTypeCode` of the
/// element type (one byte each).
pub const QUERY_METHOD: u8 = 49;
/// For a `ResolveResult` with a method symbol, with an operator name argument (a length and
/// UTF-8): whether the method's declaring type has an operator of that name (a byte).
pub const QUERY_HAS_OPERATOR: u8 = 50;
/// Not about a host object (handle 0), with a setting name argument (a length and UTF-8): the
/// boolean `DecompilerSettings` property of that name in the transform context (a byte).
pub const QUERY_SETTING: u8 = 51;
/// For a `ResolveResult` with a method symbol, with an argument (LEB128) that selects the
/// method's first type argument (0), its return type (1) or the type of parameter `i` (`2 + i`):
/// that type as the transform context's `TypeSystemAstBuilder.ConvertType` builds it, as the
/// table number of its handles (LEB128), their count (LEB128), and the subtree in the wire
/// format of the host's tree.
pub const QUERY_CONVERT_TYPE: u8 = 52;
/// For an `ILInstruction`: the `ILFunctionKind` of an `ILFunction`, 0xFF for any other
/// instruction (a byte).
pub const QUERY_IL_INSTRUCTION: u8 = 53;
/// For a `TypeResolveResult`: the namespace and the name of its type, each a length and UTF-8.
pub const QUERY_TYPE_RESOLVE_RESULT_NAME: u8 = 54;

fn push_text(q: &mut Vec<u8>, text: &str) {
    let mut v = text.len();
    loop {
        let b = (v & 0x7F) as u8;
        v >>= 7;
        if v == 0 {
            q.push(b);
            break;
        }
        q.push(b | 0x80);
    }
    q.extend_from_slice(text.as_bytes());
}

/// Reads the answers of the queries above.
struct AnswerReader<'a>(&'a [u8]);

impl AnswerReader<'_> {
    fn byte(&mut self) -> Option<u8> {
        let (&b, rest) = self.0.split_first()?;
        self.0 = rest;
        Some(b)
    }

    fn uvar(&mut self) -> Option<usize> {
        let mut v = 0usize;
        let mut shift = 0;
        loop {
            let b = self.byte()?;
            v |= usize::from(b & 0x7F) << shift;
            if b & 0x80 == 0 {
                return Some(v);
            }
            shift += 7;
        }
    }

    fn text(&mut self) -> Option<String> {
        let len = self.uvar()?;
        let (s, rest) = self.0.split_at_checked(len)?;
        self.0 = rest;
        Some(String::from_utf8_lossy(s).into_owned())
    }
}

/// The facts of `QUERY_RESOLVE_RESULT_FACTS`.
#[derive(Clone, Copy, Debug, Default, PartialEq, Eq)]
pub struct ResolveResultFacts {
    pub known_type: u8,
    pub underlying_known_type: u8,
    pub is_by_ref_like: bool,
    pub is_reference_type: Option<bool>,
    pub is_string_concat_invocation: bool,
}

/// The facts about the resolve result `handle`, `None` where it is no `ResolveResult`.
pub fn resolve_result_facts(oracle: &dyn SyntaxOracle, handle: u32) -> Option<ResolveResultFacts> {
    let b = oracle.ask(&query(QUERY_RESOLVE_RESULT_FACTS, handle))?;
    let mut r = AnswerReader(&b);
    Some(ResolveResultFacts {
        known_type: r.byte()?,
        underlying_known_type: r.byte()?,
        is_by_ref_like: r.byte()? != 0,
        is_reference_type: match r.byte()? {
            0 => Some(false),
            1 => Some(true),
            _ => None,
        },
        is_string_concat_invocation: r.byte()? != 0,
    })
}

/// A parameter in `MethodInfo`.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct ParameterInfo {
    pub known_type: u8,
    pub is_params: bool,
    /// The `KnownTypeCode` of the element type where the type is an `ArrayType`.
    pub array_element: Option<u8>,
}

/// The facts of `QUERY_METHOD`.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct MethodInfo {
    pub name: String,
    pub full_name: String,
    pub declaring_known_type: u8,
    pub requires_copy_for_readonly_lvalue: bool,
    pub type_argument_count: usize,
    pub first_type_argument_is_instantiable: bool,
    pub parameters: Vec<ParameterInfo>,
}

/// The method symbol of the resolve result `handle`, `None` where it has none.
pub fn method(oracle: &dyn SyntaxOracle, handle: u32) -> Option<MethodInfo> {
    let b = oracle.ask(&query(QUERY_METHOD, handle))?;
    let mut r = AnswerReader(&b);
    let name = r.text()?;
    let full_name = r.text()?;
    let declaring_known_type = r.byte()?;
    let requires_copy_for_readonly_lvalue = r.byte()? != 0;
    let type_argument_count = r.uvar()?;
    let first_type_argument_is_instantiable = r.byte()? != 0;
    let count = r.uvar()?;
    let mut parameters = Vec::with_capacity(count);
    for _ in 0..count {
        let known_type = r.byte()?;
        let is_params = r.byte()? != 0;
        let is_array = r.byte()? != 0;
        let element = r.byte()?;
        parameters.push(ParameterInfo {
            known_type,
            is_params,
            array_element: is_array.then_some(element),
        });
    }
    Some(MethodInfo {
        name,
        full_name,
        declaring_known_type,
        requires_copy_for_readonly_lvalue,
        type_argument_count,
        first_type_argument_is_instantiable,
        parameters,
    })
}

/// Whether the declaring type of the method of the resolve result `handle` has an operator
/// named `name`.
pub fn has_operator(oracle: &dyn SyntaxOracle, handle: u32, name: &str) -> bool {
    let mut q = query(QUERY_HAS_OPERATOR, handle);
    push_text(&mut q, name);
    oracle.ask(&q).is_some_and(|b| b.first() == Some(&1))
}

/// The boolean decompiler setting `name` of the transform, `None` where the host has no
/// transform context.
pub fn setting(oracle: &dyn SyntaxOracle, name: &str) -> Option<bool> {
    let mut q = query(QUERY_SETTING, 0);
    push_text(&mut q, name);
    oracle.ask(&q).map(|b| b.first() == Some(&1))
}

/// The type syntax the host builds for a type of the method of the resolve result `handle`
/// (`which` as in `QUERY_CONVERT_TYPE`): the table number of its handles, their count and the
/// encoded subtree.
pub fn convert_type(
    oracle: &dyn SyntaxOracle,
    handle: u32,
    which: usize,
) -> Option<(usize, u32, Vec<u8>)> {
    let mut q = query(QUERY_CONVERT_TYPE, handle);
    q.extend_from_slice(&query(0, which as u32)[1..]);
    let b = oracle.ask(&q)?;
    let mut r = AnswerReader(&b);
    let table = r.uvar()?;
    let count = r.uvar()? as u32;
    Some((table, count, r.0.to_vec()))
}

/// For the host object `handle`: `None` where it is no `ILInstruction`, else `Some(kind)` with
/// the `ILFunctionKind` of an `ILFunction` and `None` for any other instruction.
pub fn il_instruction(oracle: &dyn SyntaxOracle, handle: u32) -> Option<Option<u8>> {
    let b = oracle.ask(&query(QUERY_IL_INSTRUCTION, handle))?;
    Some(b.first().copied().filter(|&k| k != 0xFF))
}

/// The namespace and name of the type of the `TypeResolveResult` `handle`.
pub fn type_resolve_result_name(
    oracle: &dyn SyntaxOracle,
    handle: u32,
) -> Option<(String, String)> {
    let b = oracle.ask(&query(QUERY_TYPE_RESOLVE_RESULT_NAME, handle))?;
    let mut r = AnswerReader(&b);
    Some((r.text()?, r.text()?))
}

/// The checked state of the `AddCheckedBlocks` annotation `handle` (see `QUERY_CHECKED_STATE`);
/// `None` for any other object.
pub fn checked_state(oracle: &dyn SyntaxOracle, handle: u32) -> Option<u8> {
    oracle
        .ask(&query(QUERY_CHECKED_STATE, handle))?
        .first()
        .copied()
}

/// The request's `CheckForOverflowUnderflow` setting, `None` where the host did not pass it.
pub fn check_for_overflow_underflow(oracle: &dyn SyntaxOracle) -> Option<bool> {
    oracle
        .ask(&query(QUERY_CHECK_FOR_OVERFLOW_UNDERFLOW, 0))?
        .first()
        .map(|&b| b != 0)
}

// Queries 32..47 (`RewriteVisualBasicWithEventsInInitializeComponent`). The host keeps a table of
// the symbols it names; a symbol id is an index into it, and two ids are equal exactly where the
// symbols are (`Equals`). Except for `QUERY_NODE_SYMBOL`, these queries take a symbol id where the
// others take a handle.

/// For a `ResolveResult`: the symbol `AstNode.GetSymbol` takes from it (`ChosenMethod` for a
/// method group), as its id plus one, 0 for none. No answer for any other object.
pub const QUERY_NODE_SYMBOL: u8 = 32;
/// For a symbol id: its role (`MemberRole`), the id of its `MemberDefinition` (its own for a
/// non-member), a flags byte (`MEMBER_STATIC`, ...), the parameter count of a property (LEB128)
/// and the name as UTF-16 code units.
pub const QUERY_MEMBER_FACTS: u8 = 33;
/// For a property id: the id of its backing field (`PatternStatementTransform.TryGetBackingField`)
/// where that field has an `AccessedThroughProperty` attribute; no answer otherwise.
pub const QUERY_ACCESSED_THROUGH_PROPERTY_FIELD: u8 = 34;
/// For a property id and a field id: one byte, whether the field is declared in the property's
/// type definition and has its type (ignoring nullability).
pub const QUERY_FIELD_MATCHES_PROPERTY: u8 = 35;
/// For a field id: the string arguments of its single-argument
/// `Microsoft.VisualBasic.CompilerServices.AccessedThroughPropertyAttribute`s, as a count and
/// each as a count of UTF-16 code units and the units.
pub const QUERY_ACCESSED_THROUGH_PROPERTY_NAMES: u8 = 36;
/// For a member id: its return type as the transform context's `TypeSystemAstBuilder` writes it,
/// as the index of the host's first object for its handles (LEB128), then the node in the wire
/// format of `syntax.rs`, whose handles count from that index (`ANNOTATION_HOST_OBJECT`).
pub const QUERY_RETURN_TYPE_SYNTAX: u8 = 37;

/// The annotation `new MemberResolveResult(null, member)`; payload: the member's symbol id.
pub const ANNOTATION_MEMBER_RESOLVE_RESULT: u8 = 32;
/// The annotation `new TypeResolveResult(member.ReturnType)`; payload: the member's symbol id.
pub const ANNOTATION_RETURN_TYPE_RESOLVE_RESULT: u8 = 33;
/// An object the host made while answering `QUERY_RETURN_TYPE_SYNTAX`; payload: its index.
pub const ANNOTATION_HOST_OBJECT: u8 = 34;

/// `MemberFacts::flags`: the member is static.
pub const MEMBER_STATIC: u8 = 1;
/// `MemberFacts::flags`: the property has a setter.
pub const MEMBER_HAS_SETTER: u8 = 2;
/// `MemberFacts::flags`: the method is a Windows Forms `InitializeComponent`
/// (`CSharpDecompiler.IsWindowsFormsInitializeComponentMethod`).
pub const MEMBER_WINDOWS_FORMS_INITIALIZE_COMPONENT: u8 = 4;

/// The interface a symbol implements, as far as `QUERY_MEMBER_FACTS` tells them apart.
#[derive(Clone, Copy, Debug, PartialEq, Eq)]
pub enum MemberRole {
    Other,
    Property,
    Field,
    Event,
    Method,
}

/// The answer to `QUERY_MEMBER_FACTS`.
#[derive(Clone, Debug, PartialEq, Eq)]
pub struct MemberFacts {
    pub role: MemberRole,
    pub member_definition: u32,
    pub flags: u8,
    pub parameter_count: u32,
    pub name: Vec<u16>,
}

fn query_args(kind: u8, args: &[u32]) -> Vec<u8> {
    let mut q = vec![kind];
    for &a in args {
        super::syntax::uvar(&mut q, u64::from(a));
    }
    q
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

fn read_utf16(bytes: &[u8]) -> Vec<u16> {
    bytes
        .as_chunks::<2>()
        .0
        .iter()
        .map(|c| u16::from_le_bytes([c[0], c[1]]))
        .collect()
}

/// `QUERY_NODE_SYMBOL`: `None` where the object `handle` is no `ResolveResult`, else its symbol id.
pub fn node_symbol(oracle: &dyn SyntaxOracle, handle: u32) -> Option<Option<u32>> {
    let b = oracle.ask(&query(QUERY_NODE_SYMBOL, handle))?;
    let id = read_uvar(&mut b.as_slice())?;
    Some(id.checked_sub(1).map(|id| id as u32))
}

/// `QUERY_MEMBER_FACTS` of the symbol `id`.
pub fn member_facts(oracle: &dyn SyntaxOracle, id: u32) -> Option<MemberFacts> {
    let b = oracle.ask(&query_args(QUERY_MEMBER_FACTS, &[id]))?;
    let mut rest = b.as_slice();
    let (&role, r) = rest.split_first()?;
    rest = r;
    let role = match role {
        1 => MemberRole::Property,
        2 => MemberRole::Field,
        3 => MemberRole::Event,
        4 => MemberRole::Method,
        _ => MemberRole::Other,
    };
    let member_definition = read_uvar(&mut rest)? as u32;
    let (&flags, r) = rest.split_first()?;
    rest = r;
    let parameter_count = read_uvar(&mut rest)? as u32;
    Some(MemberFacts {
        role,
        member_definition,
        flags,
        parameter_count,
        name: read_utf16(rest),
    })
}

/// `QUERY_ACCESSED_THROUGH_PROPERTY_FIELD` of the property `id`.
pub fn accessed_through_property_field(oracle: &dyn SyntaxOracle, id: u32) -> Option<u32> {
    let b = oracle.ask(&query_args(QUERY_ACCESSED_THROUGH_PROPERTY_FIELD, &[id]))?;
    read_uvar(&mut b.as_slice()).map(|v| v as u32)
}

/// `QUERY_FIELD_MATCHES_PROPERTY` of the property `property` and the field `field`.
pub fn field_matches_property(oracle: &dyn SyntaxOracle, property: u32, field: u32) -> bool {
    oracle
        .ask(&query_args(
            QUERY_FIELD_MATCHES_PROPERTY,
            &[property, field],
        ))
        .is_some_and(|b| b.first() == Some(&1))
}

/// `QUERY_ACCESSED_THROUGH_PROPERTY_NAMES` of the field `id`.
pub fn accessed_through_property_names(oracle: &dyn SyntaxOracle, id: u32) -> Vec<Vec<u16>> {
    let Some(b) = oracle.ask(&query_args(QUERY_ACCESSED_THROUGH_PROPERTY_NAMES, &[id])) else {
        return Vec::new();
    };
    let mut rest = b.as_slice();
    let mut names = Vec::new();
    let count = read_uvar(&mut rest).unwrap_or(0);
    for _ in 0..count {
        let Some(len) = read_uvar(&mut rest) else {
            break;
        };
        let Some((s, r)) = rest.split_at_checked(len as usize * 2) else {
            break;
        };
        rest = r;
        names.push(read_utf16(s));
    }
    names
}

/// `QUERY_RETURN_TYPE_SYNTAX` of the member `id`: the index of the host's first object for the
/// node's handles, and the node's bytes.
pub fn return_type_syntax(oracle: &dyn SyntaxOracle, id: u32) -> Option<(u32, Vec<u8>)> {
    let b = oracle.ask(&query_args(QUERY_RETURN_TYPE_SYNTAX, &[id]))?;
    let mut rest = b.as_slice();
    let base = read_uvar(&mut rest)? as u32;
    Some((base, rest.to_vec()))
}

/// An oracle that knows nothing, for trees without host objects.
pub struct NoOracle;

impl SyntaxOracle for NoOracle {
    fn ask(&self, _query: &[u8]) -> Option<Vec<u8>> {
        None
    }
}
