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

/// An oracle that knows nothing, for trees without host objects.
pub struct NoOracle;

impl SyntaxOracle for NoOracle {
    fn ask(&self, _query: &[u8]) -> Option<Vec<u8>> {
        None
    }
}
