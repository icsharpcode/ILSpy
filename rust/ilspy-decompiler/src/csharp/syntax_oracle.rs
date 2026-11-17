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

/// An oracle that knows nothing, for trees without host objects.
pub struct NoOracle;

impl SyntaxOracle for NoOracle {
    fn ask(&self, _query: &[u8]) -> Option<Vec<u8>> {
        None
    }
}
