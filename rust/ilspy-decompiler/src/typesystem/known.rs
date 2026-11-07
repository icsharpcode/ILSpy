//! Known types (the counterpart of `KnownTypeCode` / `KnownTypeReference`).

use super::names::TopLevelTypeName;

macro_rules! known_types {
    ($( $code:ident = $value:literal $( => $ns:literal, $name:literal, $tpc:literal )? ),* $(,)?) => {
        /// Discriminants equal the managed `KnownTypeCode` values.
        #[derive(Clone, Copy, Debug, PartialEq, Eq, Hash)]
        #[repr(u8)]
        pub enum KnownTypeCode { $( $code = $value ),* }

        impl KnownTypeCode {
            pub const ALL: &'static [KnownTypeCode] = &[ $( KnownTypeCode::$code ),* ];

            pub fn name(self) -> &'static str {
                match self { $( KnownTypeCode::$code => stringify!($code) ),* }
            }

            /// `(namespace, name, type parameter count)`, `None` for `None`.
            pub fn type_name(self) -> Option<(&'static str, &'static str, u32)> {
                match self {
                    $( KnownTypeCode::$code => known_types!(@name $( $ns, $name, $tpc )?) ),*
                }
            }
        }
    };
    (@name) => { None };
    (@name $ns:literal, $name:literal, $tpc:literal) => { Some(($ns, $name, $tpc)) };
}

known_types! {
    None = 0,
    Object = 1 => "System", "Object", 0,
    DBNull = 2 => "System", "DBNull", 0,
    Boolean = 3 => "System", "Boolean", 0,
    Char = 4 => "System", "Char", 0,
    SByte = 5 => "System", "SByte", 0,
    Byte = 6 => "System", "Byte", 0,
    Int16 = 7 => "System", "Int16", 0,
    UInt16 = 8 => "System", "UInt16", 0,
    Int32 = 9 => "System", "Int32", 0,
    UInt32 = 10 => "System", "UInt32", 0,
    Int64 = 11 => "System", "Int64", 0,
    UInt64 = 12 => "System", "UInt64", 0,
    Single = 13 => "System", "Single", 0,
    Double = 14 => "System", "Double", 0,
    Decimal = 15 => "System", "Decimal", 0,
    DateTime = 16 => "System", "DateTime", 0,
    String = 18 => "System", "String", 0,
    Void = 19 => "System", "Void", 0,
    Type = 20 => "System", "Type", 0,
    Array = 21 => "System", "Array", 0,
    Attribute = 22 => "System", "Attribute", 0,
    ValueType = 23 => "System", "ValueType", 0,
    Enum = 24 => "System", "Enum", 0,
    Delegate = 25 => "System", "Delegate", 0,
    MulticastDelegate = 26 => "System", "MulticastDelegate", 0,
    Exception = 27 => "System", "Exception", 0,
    IntPtr = 28 => "System", "IntPtr", 0,
    UIntPtr = 29 => "System", "UIntPtr", 0,
    IEnumerable = 30 => "System.Collections", "IEnumerable", 0,
    IEnumerator = 31 => "System.Collections", "IEnumerator", 0,
    IEnumerableOfT = 32 => "System.Collections.Generic", "IEnumerable", 1,
    IEnumeratorOfT = 33 => "System.Collections.Generic", "IEnumerator", 1,
    ICollection = 34 => "System.Collections", "ICollection", 0,
    ICollectionOfT = 35 => "System.Collections.Generic", "ICollection", 1,
    IList = 36 => "System.Collections", "IList", 0,
    IListOfT = 37 => "System.Collections.Generic", "IList", 1,
    IReadOnlyCollectionOfT = 38 => "System.Collections.Generic", "IReadOnlyCollection", 1,
    IReadOnlyListOfT = 39 => "System.Collections.Generic", "IReadOnlyList", 1,
    Task = 40 => "System.Threading.Tasks", "Task", 0,
    TaskOfT = 41 => "System.Threading.Tasks", "Task", 1,
    ValueTask = 42 => "System.Threading.Tasks", "ValueTask", 0,
    ValueTaskOfT = 43 => "System.Threading.Tasks", "ValueTask", 1,
    NullableOfT = 44 => "System", "Nullable", 1,
    IDisposable = 45 => "System", "IDisposable", 0,
    IAsyncDisposable = 46 => "System", "IAsyncDisposable", 0,
    INotifyCompletion = 47 => "System.Runtime.CompilerServices", "INotifyCompletion", 0,
    ICriticalNotifyCompletion = 48 => "System.Runtime.CompilerServices", "ICriticalNotifyCompletion", 0,
    TypedReference = 49 => "System", "TypedReference", 0,
    IFormattable = 50 => "System", "IFormattable", 0,
    FormattableString = 51 => "System", "FormattableString", 0,
    DefaultInterpolatedStringHandler = 52 => "System.Runtime.CompilerServices", "DefaultInterpolatedStringHandler", 0,
    SpanOfT = 53 => "System", "Span", 1,
    ReadOnlySpanOfT = 54 => "System", "ReadOnlySpan", 1,
    MemoryOfT = 55 => "System", "Memory", 1,
    Unsafe = 56 => "System.Runtime.CompilerServices", "Unsafe", 0,
    IAsyncEnumerableOfT = 57 => "System.Collections.Generic", "IAsyncEnumerable", 1,
    IAsyncEnumeratorOfT = 58 => "System.Collections.Generic", "IAsyncEnumerator", 1,
    Index = 59 => "System", "Index", 0,
    Range = 60 => "System", "Range", 0,
    RuntimeArgumentHandle = 61 => "System", "RuntimeArgumentHandle", 0,
    RuntimeTypeHandle = 62 => "System", "RuntimeTypeHandle", 0,
    RuntimeFieldHandle = 63 => "System", "RuntimeFieldHandle", 0,
    RuntimeMethodHandle = 64 => "System", "RuntimeMethodHandle", 0,
}

impl KnownTypeCode {
    pub fn top_level_name(self) -> Option<TopLevelTypeName> {
        self.type_name()
            .map(|(ns, name, tpc)| TopLevelTypeName::new(ns, name, tpc))
    }

    /// The known type a top-level name denotes, `None` if it is not one.
    pub fn from_top_level(name: &TopLevelTypeName) -> KnownTypeCode {
        Self::ALL
            .iter()
            .copied()
            .find(|code| {
                code.type_name()
                    .is_some_and(|(ns, n, tpc)| name.ns == ns && name.name == n && name.tpc == tpc)
            })
            .unwrap_or(KnownTypeCode::None)
    }
}
