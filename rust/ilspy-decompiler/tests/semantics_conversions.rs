//! `ICSharpCode.Decompiler.Tests/Semantics/ConversionTests.cs`.
//!
//! `ImplicitConversion(typeof(A), typeof(B))` is `implicit(c, "A", "B")`; the placeholder
//! classes `Null`, `Dynamic`, `NInt` and `NUInt` are the types `null`, `dynamic`, `nint` and
//! `nuint`. Fixture types declared in `ICSharpCode.Decompiler.Tests.TypeSystem` are named by
//! their reflection names (see `tsn`).

mod common;

use std::rc::Rc;

use common::csharp::{
    compilation, method_type_parameter, methods, ref_assembly_compilation, tuple, ty_in,
    type_parameter,
};
use ilspy_decompiler::semantics::{
    Conversion as C, Conversions, Lambda, Method, MethodGroup, MethodList, OverloadResolution,
    OverloadResolutionErrors, Parameter, ResolveResult,
};
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::members::ConstValue;
use ilspy_decompiler::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};

const OUTER: &[&str] = &["ConversionTest"];

struct Fixture {
    comp: Compilation<'static>,
}

impl Fixture {
    fn new() -> Fixture {
        Fixture {
            comp: compilation(),
        }
    }

    /// `RefAssemblyCompilation.Instance`.
    fn ref_assembly() -> Fixture {
        Fixture {
            comp: ref_assembly_compilation(),
        }
    }

    fn run<R>(&self, f: impl FnOnce(&mut Conversions<'_, '_, '_>) -> R) -> R {
        let mut ts = TypeSystem::new(&self.comp);
        let mut conversions = Conversions::new(&mut ts);
        f(&mut conversions)
    }
}

/// The reflection name of a type in `ICSharpCode.Decompiler.Tests.TypeSystem`.
fn tsn(name: &str) -> String {
    format!("ICSharpCode.Decompiler.Tests.TypeSystem.{name}")
}

/// `MethodGroupConversionTestCases.<name>`.
fn mgc(name: &str) -> String {
    tsn(&format!("MethodGroupConversionTestCases+{name}"))
}

/// `UserDefinedConversionTestCases.<name>`.
fn udc(name: &str) -> String {
    tsn(&format!("UserDefinedConversionTestCases+{name}"))
}

fn t(c: &mut Conversions<'_, '_, '_>, s: &str) -> TypeId {
    ty_in(c.ts, s, OUTER)
}

fn known(c: &mut Conversions<'_, '_, '_>, code: KnownTypeCode) -> TypeId {
    c.ts.known_type(code)
}

fn null_type(c: &mut Conversions<'_, '_, '_>) -> TypeId {
    c.ts.intern(TypeData::Null)
}

fn implicit(c: &mut Conversions<'_, '_, '_>, from: &str, to: &str) -> C {
    let from = t(c, from);
    let to = t(c, to);
    c.implicit_type(from, to)
}

fn explicit(c: &mut Conversions<'_, '_, '_>, from: &str, to: &str) -> C {
    let from = t(c, from);
    let to = t(c, to);
    c.explicit_type(from, to)
}

/// `new ConstantResolveResult(compilation.FindType(value.GetType()), value)`.
fn constant_rr(c: &mut Conversions<'_, '_, '_>, value: ConstValue) -> ResolveResult {
    let code = match value {
        ConstValue::Int32(_) => KnownTypeCode::Int32,
        ConstValue::UInt32(_) => KnownTypeCode::UInt32,
        ConstValue::Int64(_) => KnownTypeCode::Int64,
        ConstValue::UInt16(_) => KnownTypeCode::UInt16,
        _ => panic!("unexpected constant {value:?}"),
    };
    ResolveResult::Constant {
        ty: known(c, code),
        value,
    }
}

/// `ConstantConversion(value, typeof(to))`.
fn constant(c: &mut Conversions<'_, '_, '_>, value: ConstValue, to: &str) -> C {
    let rr = constant_rr(c, value);
    let to = t(c, to);
    c.implicit(&rr, to)
}

/// `IntegerLiteralConversion(value, typeof(to))`.
fn int_lit(c: &mut Conversions<'_, '_, '_>, value: ConstValue, to: &str) -> bool {
    constant(c, value, to).is_valid()
}

/// `BetterConversion(typeof(s), typeof(t1), typeof(t2))`.
fn better(c: &mut Conversions<'_, '_, '_>, s: &str, t1: &str, t2: &str) -> u8 {
    let s = t(c, s);
    let t1 = t(c, t1);
    let t2 = t(c, t2);
    c.better_conversion_from_type(s, t1, t2)
}

/// `BetterConversion(value, typeof(t1), typeof(t2))`.
fn better_const(c: &mut Conversions<'_, '_, '_>, value: ConstValue, t1: &str, t2: &str) -> u8 {
    let rr = constant_rr(c, value);
    let t1 = t(c, t1);
    let t2 = t(c, t2);
    c.better_conversion(&rr, t1, t2)
}

/// A method group of the methods named `name` in `declaring` with the target `target`
/// (`new ResolveResult(declaring)` by default).
fn group(
    c: &mut Conversions<'_, '_, '_>,
    declaring: TypeId,
    name: &str,
    target: Option<ResolveResult>,
    extension_methods: Option<Vec<Method>>,
    type_arguments: Vec<TypeId>,
) -> ResolveResult {
    let list = methods(c.ts, declaring, name);
    ResolveResult::MethodGroup(Rc::new(MethodGroup {
        target: target.unwrap_or(ResolveResult::Expression(declaring)),
        name: name.to_owned(),
        method_lists: vec![MethodList {
            declaring,
            methods: list,
        }],
        type_arguments,
        extension_methods: extension_methods.map(|e| vec![e]).unwrap_or_default(),
    }))
}

/// `MethodGroupConversion(typeof(declaring), name, typeof(delegate), target, extensionMethods,
/// typeArguments)`.
fn mg_conv_with(
    c: &mut Conversions<'_, '_, '_>,
    declaring: &str,
    name: &str,
    delegate: &str,
    target: Option<ResolveResult>,
    extension_methods: Option<Vec<Method>>,
    type_arguments: Vec<TypeId>,
) -> C {
    let declaring = t(c, declaring);
    let rr = group(
        c,
        declaring,
        name,
        target,
        extension_methods,
        type_arguments,
    );
    let delegate = t(c, delegate);
    c.implicit(&rr, delegate)
}

fn mg_conv(c: &mut Conversions<'_, '_, '_>, declaring: &str, name: &str, delegate: &str) -> C {
    mg_conv_with(c, declaring, name, delegate, None, None, Vec::new())
}

/// `IMethod.FullName`.
fn method_full_name(c: &mut Conversions<'_, '_, '_>, m: &Method) -> String {
    let declaring = m.declaring.expect("a declaring type");
    format!("{}.{}", c.ts.full_name(declaring), m.name)
}

fn method(conversion: &C) -> &Method {
    conversion.method().expect("a method")
}

/// The type of the single parameter of the conversion's method.
fn single_parameter_full_name(c: &mut Conversions<'_, '_, '_>, conversion: &C) -> String {
    let m = method(conversion);
    assert_eq!(m.parameters.len(), 1, "a single parameter");
    c.ts.full_name(m.parameters[0].ty)
}

fn single_type_argument_is_int32(c: &mut Conversions<'_, '_, '_>, conversion: &C) -> bool {
    let args = method(conversion)
        .method_args
        .as_ref()
        .expect("type arguments");
    assert_eq!(args.len(), 1, "a single type argument");
    c.ts.is_known(args[0], KnownTypeCode::Int32)
}

fn before(conversion: &C) -> &C {
    conversion
        .conversion_before_user_defined_operator()
        .expect("a user-defined conversion")
}

fn after(conversion: &C) -> &C {
    conversion
        .conversion_after_user_defined_operator()
        .expect("a user-defined conversion")
}

/// A class type parameter (`SymbolKind.TypeDefinition`).
fn class_tp(
    c: &mut Conversions<'_, '_, '_>,
    index: u32,
    name: &str,
    reference_type: bool,
    value_type: bool,
    constraints: Vec<TypeId>,
) -> TypeId {
    type_parameter(
        c.ts,
        false,
        index,
        name,
        reference_type,
        value_type,
        false,
        constraints,
    )
}

/// A method type parameter (`SymbolKind.Method`).
fn method_tp(
    c: &mut Conversions<'_, '_, '_>,
    index: u32,
    name: &str,
    reference_type: bool,
    constraints: Vec<TypeId>,
) -> TypeId {
    type_parameter(
        c.ts,
        true,
        index,
        name,
        reference_type,
        false,
        false,
        constraints,
    )
}

fn array(c: &mut Conversions<'_, '_, '_>, elem: TypeId) -> TypeId {
    c.ts.intern(TypeData::Array {
        elem,
        rank: 1,
        nullability: Nullability::Oblivious,
    })
}

#[test]
fn identity_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "char", "char"), C::IDENTITY);
        assert_eq!(implicit(c, "string", "string"), C::IDENTITY);
        assert_eq!(implicit(c, "object", "object"), C::IDENTITY);
        assert_eq!(implicit(c, "bool", "char"), C::None);
        assert_eq!(implicit(c, "dynamic", "dynamic"), C::IDENTITY);
        assert_eq!(implicit(c, "?", "?"), C::IDENTITY);
        let null = null_type(c);
        assert_eq!(c.implicit_type(null, null), C::IDENTITY);
    });
}

#[test]
fn dynamic_identity_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "object", "dynamic"), C::IDENTITY);
        assert_eq!(implicit(c, "dynamic", "object"), C::IDENTITY);
    });
}

#[test]
fn complex_dynamic_identity_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "List<object>", "List<dynamic>"), C::IDENTITY);
        assert_eq!(implicit(c, "List<dynamic>", "List<object>"), C::IDENTITY);
        assert_eq!(implicit(c, "List<string>", "List<dynamic>"), C::None);
        assert_eq!(implicit(c, "List<dynamic>", "List<string>"), C::None);

        assert_eq!(
            implicit(c, "List<List<dynamic>[]>", "List<List<object>[]>"),
            C::IDENTITY
        );
        assert_eq!(
            implicit(c, "List<List<object>[]>", "List<List<dynamic>[]>"),
            C::IDENTITY
        );
        assert_eq!(
            implicit(c, "List<List<object>[,]>", "List<List<dynamic>[]>"),
            C::None
        );
    });
}

#[test]
fn tuple_identity_conversions() {
    Fixture::new().run(|c| {
        let int = known(c, KnownTypeCode::Int32);
        let string = known(c, KnownTypeCode::String);
        let ab = tuple(c.ts, &[int, string], &[Some("a"), Some("b")]);
        let ac = tuple(c.ts, &[int, string], &[Some("a"), Some("c")]);
        assert_eq!(c.implicit_type(ab, ac), C::IDENTITY);

        let ba = tuple(c.ts, &[string, int], &[Some("a"), Some("b")]);
        assert_eq!(c.implicit_type(ab, ba), C::None);
    });
}

#[test]
fn tuple_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(
            implicit(c, "(int, string)", "(long, object)"),
            C::tuple(vec![C::IMPLICIT_NUMERIC, C::IMPLICIT_REFERENCE])
        );
        assert_eq!(
            implicit(c, "ValueTuple<float>", "ValueTuple<double>"),
            C::tuple(vec![C::IMPLICIT_NUMERIC])
        );
    });
}

#[test]
fn explicit_tuple_conversion_classifies_elements_as_casts() {
    Fixture::new().run(|c| {
        // Every element of this tuple also converts implicitly, so an implicit tuple
        // conversion exists...
        let ic = implicit(c, "(DateTime, int)", "(DateTimeOffset, long)");
        assert!(ic.is_tuple());
        assert!(ic.is_implicit());
        let elements = ic.element_conversions().expect("element conversions");
        assert!(elements[0].is_user_defined());
        assert!(elements[0].is_implicit());

        // ...but a cast must not use it: Roslyn's ClassifyConversionFromTypeForCast discards
        // an ImplicitTuple result and re-classifies the elements as cast conversions, so the
        // DateTime element becomes a user-defined explicit conversion.
        let ec = explicit(c, "(DateTime, int)", "(DateTimeOffset, long)");
        assert!(ec.is_tuple());
        assert!(ec.is_explicit());
        let elements = ec.element_conversions().expect("element conversions");
        assert!(elements[0].is_user_defined());
        assert!(elements[0].is_explicit());
        assert_eq!(elements[1], C::IMPLICIT_NUMERIC);
    });
}

#[test]
fn primitive_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "char", "ushort"), C::IMPLICIT_NUMERIC);
        assert_eq!(implicit(c, "byte", "char"), C::None);
        assert_eq!(implicit(c, "int", "long"), C::IMPLICIT_NUMERIC);
        assert_eq!(implicit(c, "long", "int"), C::None);
        assert_eq!(implicit(c, "int", "float"), C::IMPLICIT_NUMERIC);
        assert_eq!(implicit(c, "bool", "float"), C::None);
        assert_eq!(implicit(c, "float", "double"), C::IMPLICIT_NUMERIC);
        assert_eq!(implicit(c, "float", "decimal"), C::None);
        assert_eq!(implicit(c, "char", "long"), C::IMPLICIT_NUMERIC);
        assert_eq!(implicit(c, "uint", "long"), C::IMPLICIT_NUMERIC);
    });
}

#[test]
fn enumeration_conversion() {
    Fixture::new().run(|c| {
        let zero = constant_rr(c, ConstValue::Int32(0));
        let one = constant_rr(c, ConstValue::Int32(1));
        let to = t(c, "StringComparison");
        assert_eq!(c.implicit(&zero, to), C::enumeration(true, false));
        assert_eq!(c.implicit(&one, to), C::None);
    });
}

#[test]
fn nullable_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "char", "ushort?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "byte", "char?"), C::None);
        assert_eq!(implicit(c, "int", "long?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "long", "int?"), C::None);
        assert_eq!(implicit(c, "int", "float?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "bool", "float?"), C::None);
        assert_eq!(implicit(c, "float", "double?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "float", "decimal?"), C::None);
    });
}

#[test]
fn nullable_conversions2() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "char?", "ushort?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "byte?", "char?"), C::None);
        assert_eq!(implicit(c, "int?", "long?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "long?", "int?"), C::None);
        assert_eq!(implicit(c, "int?", "float?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "bool?", "float?"), C::None);
        assert_eq!(implicit(c, "float?", "double?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(implicit(c, "float?", "decimal?"), C::None);
    });
}

#[test]
fn nullable_enumeration_conversion() {
    Fixture::new().run(|c| {
        let zero = constant_rr(c, ConstValue::Int32(0));
        let one = constant_rr(c, ConstValue::Int32(1));
        let to = t(c, "StringComparison?");
        assert_eq!(c.implicit(&zero, to), C::enumeration(true, true));
        assert_eq!(c.implicit(&one, to), C::None);
    });
}

#[test]
fn null_literal_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "null", "int?"), C::NULL_LITERAL);
        assert_eq!(implicit(c, "null", "char?"), C::NULL_LITERAL);
        assert_eq!(implicit(c, "null", "int"), C::None);
        assert_eq!(implicit(c, "null", "object"), C::NULL_LITERAL);
        assert_eq!(implicit(c, "null", "dynamic"), C::NULL_LITERAL);
        assert_eq!(implicit(c, "null", "string"), C::NULL_LITERAL);
        assert_eq!(implicit(c, "null", "int[]"), C::NULL_LITERAL);
    });
}

#[test]
fn simple_reference_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "string", "object"), C::IMPLICIT_REFERENCE);
        assert_eq!(
            implicit(c, "BitArray", "ICollection"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "IList", "IEnumerable"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "object", "string"), C::None);
        assert_eq!(implicit(c, "ICollection", "BitArray"), C::None);
        assert_eq!(implicit(c, "IEnumerable", "IList"), C::None);
    });
}

#[test]
fn conversion_to_dynamic() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "string", "dynamic"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "int", "dynamic"), C::BOXING);
    });
}

#[test]
fn conversion_from_dynamic() {
    Fixture::new().run(|c| {
        // There is no conversion from the type 'dynamic' to other types (except the identity
        // conversion to object). Such conversions only exist from dynamic expressions.
        assert_eq!(implicit(c, "dynamic", "string"), C::None);
        assert_eq!(implicit(c, "dynamic", "int"), C::None);

        let dynamic = c.ts.intern(TypeData::Dynamic);
        let dynamic_rr = ResolveResult::Expression(dynamic);
        let string = known(c, KnownTypeCode::String);
        let int = known(c, KnownTypeCode::Int32);
        assert_eq!(c.implicit(&dynamic_rr, string), C::IMPLICIT_DYNAMIC);
        assert_eq!(c.implicit(&dynamic_rr, int), C::IMPLICIT_DYNAMIC);
    });
}

#[test]
fn parameterized_type_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(
            implicit(c, "List<string>", "ICollection<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "IList<string>", "ICollection<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "List<string>", "ICollection<object>"), C::None);
        assert_eq!(implicit(c, "IList<string>", "ICollection<object>"), C::None);
    });
}

#[test]
fn array_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "string[]", "object[]"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "string[,]", "object[,]"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "string[]", "object[,]"), C::None);
        assert_eq!(implicit(c, "object[]", "string[]"), C::None);

        assert_eq!(
            implicit(c, "string[]", "IList<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "string[,]", "IList<string>"), C::None);
        assert_eq!(
            implicit(c, "string[]", "IList<object>"),
            C::IMPLICIT_REFERENCE
        );

        assert_eq!(implicit(c, "string[]", "Array"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "string[]", "ICloneable"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "Array", "string[]"), C::None);
        assert_eq!(implicit(c, "object", "object[]"), C::None);
    });
}

#[test]
fn variance_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(
            implicit(c, "List<string>", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "List<object>", "IEnumerable<string>"), C::None);
        assert_eq!(
            implicit(c, "IEnumerable<string>", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "ICollection<string>", "ICollection<object>"),
            C::None
        );

        assert_eq!(
            implicit(c, "Comparer<object>", "IComparer<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "Comparer<object>", "IComparer<Array>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "Comparer<object>", "Comparer<string>"), C::None);

        assert_eq!(implicit(c, "List<object>", "IEnumerable<string>"), C::None);
        assert_eq!(
            implicit(c, "IEnumerable<string>", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );

        assert_eq!(
            implicit(
                c,
                "Func<ICollection, ICollection>",
                "Func<IList, IEnumerable>"
            ),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(
                c,
                "Func<IEnumerable, IList>",
                "Func<ICollection, ICollection>"
            ),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(
                c,
                "Func<ICollection, ICollection>",
                "Func<IEnumerable, IList>"
            ),
            C::None
        );
        assert_eq!(
            implicit(
                c,
                "Func<IList, IEnumerable>",
                "Func<ICollection, ICollection>"
            ),
            C::None
        );
    });
}

#[test]
fn implicit_pointer_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "null", "int*"), C::IMPLICIT_POINTER);
        assert_eq!(implicit(c, "int*", "void*"), C::IMPLICIT_POINTER);
    });
}

#[test]
fn no_conversion_from_pointer_type_to_object() {
    Fixture::new().run(|c| {
        assert_eq!(implicit(c, "int*", "object"), C::None);
        assert_eq!(implicit(c, "int*", "dynamic"), C::None);
    });
}

/// The rows of the native-integer conversion table in
/// https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md, checked as
/// `explicit(from, to)` (`native_is_from`) or `explicit(to, from)`.
fn check_native(
    c: &mut Conversions<'_, '_, '_>,
    native: &str,
    native_is_from: bool,
    rows: &[(&str, C)],
) {
    for (other, expected) in rows {
        let actual = if native_is_from {
            explicit(c, native, other)
        } else {
            explicit(c, other, native)
        };
        assert_eq!(&actual, expected, "{native} / {other}");
    }
}

#[test]
fn conversion_to_nint() {
    Fixture::new().run(|c| {
        check_native(
            c,
            "nint",
            false,
            &[
                ("object", C::UNBOXING),
                ("void*", C::EXPLICIT_POINTER),
                ("sbyte", C::IMPLICIT_NUMERIC),
                ("byte", C::IMPLICIT_NUMERIC),
                ("short", C::IMPLICIT_NUMERIC),
                ("ushort", C::IMPLICIT_NUMERIC),
                ("int", C::IMPLICIT_NUMERIC),
                ("uint", C::EXPLICIT_NUMERIC),
                ("long", C::EXPLICIT_NUMERIC),
                ("ulong", C::EXPLICIT_NUMERIC),
                ("char", C::IMPLICIT_NUMERIC),
                ("float", C::EXPLICIT_NUMERIC),
                ("double", C::EXPLICIT_NUMERIC),
                ("decimal", C::EXPLICIT_NUMERIC),
                ("IntPtr", C::IDENTITY),
                ("UIntPtr", C::None),
            ],
        );
    });
}

#[test]
fn conversion_to_nuint() {
    Fixture::new().run(|c| {
        check_native(
            c,
            "nuint",
            false,
            &[
                ("object", C::UNBOXING),
                ("void*", C::EXPLICIT_POINTER),
                ("sbyte", C::EXPLICIT_NUMERIC),
                ("byte", C::IMPLICIT_NUMERIC),
                ("short", C::EXPLICIT_NUMERIC),
                ("ushort", C::IMPLICIT_NUMERIC),
                ("int", C::EXPLICIT_NUMERIC),
                ("uint", C::IMPLICIT_NUMERIC),
                ("long", C::EXPLICIT_NUMERIC),
                ("ulong", C::EXPLICIT_NUMERIC),
                ("char", C::IMPLICIT_NUMERIC),
                ("float", C::EXPLICIT_NUMERIC),
                ("double", C::EXPLICIT_NUMERIC),
                ("decimal", C::EXPLICIT_NUMERIC),
                ("IntPtr", C::None),
                ("UIntPtr", C::IDENTITY),
            ],
        );
    });
}

#[test]
fn conversion_from_nint() {
    Fixture::new().run(|c| {
        check_native(
            c,
            "nint",
            true,
            &[
                ("object", C::BOXING),
                ("void*", C::EXPLICIT_POINTER),
                ("nuint", C::EXPLICIT_NUMERIC),
                ("sbyte", C::EXPLICIT_NUMERIC),
                ("byte", C::EXPLICIT_NUMERIC),
                ("short", C::EXPLICIT_NUMERIC),
                ("ushort", C::EXPLICIT_NUMERIC),
                ("int", C::EXPLICIT_NUMERIC),
                ("uint", C::EXPLICIT_NUMERIC),
                ("long", C::IMPLICIT_NUMERIC),
                ("ulong", C::EXPLICIT_NUMERIC),
                ("char", C::EXPLICIT_NUMERIC),
                ("float", C::IMPLICIT_NUMERIC),
                ("double", C::IMPLICIT_NUMERIC),
                ("decimal", C::IMPLICIT_NUMERIC),
                ("IntPtr", C::IDENTITY),
                ("UIntPtr", C::None),
            ],
        );
    });
}

#[test]
fn conversion_from_nuint() {
    Fixture::new().run(|c| {
        check_native(
            c,
            "nuint",
            true,
            &[
                ("object", C::BOXING),
                ("void*", C::EXPLICIT_POINTER),
                ("nint", C::EXPLICIT_NUMERIC),
                ("sbyte", C::EXPLICIT_NUMERIC),
                ("byte", C::EXPLICIT_NUMERIC),
                ("short", C::EXPLICIT_NUMERIC),
                ("ushort", C::EXPLICIT_NUMERIC),
                ("int", C::EXPLICIT_NUMERIC),
                ("uint", C::EXPLICIT_NUMERIC),
                ("long", C::EXPLICIT_NUMERIC),
                ("ulong", C::IMPLICIT_NUMERIC),
                ("char", C::EXPLICIT_NUMERIC),
                ("float", C::IMPLICIT_NUMERIC),
                ("double", C::IMPLICIT_NUMERIC),
                ("decimal", C::IMPLICIT_NUMERIC),
                ("IntPtr", C::None),
                ("UIntPtr", C::IDENTITY),
            ],
        );
    });
}

#[test]
fn nint_enum_conversion() {
    Fixture::new().run(|c| {
        let explicit_enum = C::enumeration(false, false);
        assert_eq!(explicit(c, "nint", "StringComparison"), explicit_enum);
        assert_eq!(explicit(c, "nuint", "StringComparison"), explicit_enum);
        assert_eq!(explicit(c, "StringComparison", "nint"), explicit_enum);
        assert_eq!(explicit(c, "StringComparison", "nuint"), explicit_enum);
    });
}

#[test]
fn integer_literal_to_nint_conversions() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "nint"));
        assert!(int_lit(c, ConstValue::Int32(-1), "nint"));
        assert!(!int_lit(c, ConstValue::UInt32(u32::MAX), "nint"));
        assert!(!int_lit(c, ConstValue::Int64(i64::MAX), "nint"));
    });
}

#[test]
fn integer_literal_to_nuint_conversions() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "nuint"));
        assert!(!int_lit(c, ConstValue::Int32(-1), "nuint"));
        assert!(int_lit(c, ConstValue::UInt32(u32::MAX), "nuint"));
        assert!(!int_lit(c, ConstValue::Int64(i64::MAX), "nuint"));
    });
}

#[test]
fn unconstrained_type_parameter() {
    Fixture::new().run(|c| {
        let tp = class_tp(c, 0, "T", false, false, Vec::new());
        let t2 = class_tp(c, 1, "T2", false, false, Vec::new());
        let tm = method_type_parameter(c.ts, 0, "TM");
        let null = null_type(c);
        let object = known(c, KnownTypeCode::Object);
        let dynamic = c.ts.intern(TypeData::Dynamic);
        let value_type = t(c, "ValueType");

        assert_eq!(c.implicit_type(null, tp), C::None);
        assert_eq!(c.implicit_type(tp, object), C::BOXING);
        assert_eq!(c.implicit_type(tp, dynamic), C::BOXING);
        assert_eq!(c.implicit_type(tp, value_type), C::None);

        assert_eq!(c.implicit_type(tp, tp), C::IDENTITY);
        assert_eq!(c.implicit_type(t2, tp), C::None);
        assert_eq!(c.implicit_type(tp, t2), C::None);
        assert_eq!(c.implicit_type(tp, tm), C::None);
        assert_eq!(c.implicit_type(tm, tp), C::None);
    });
}

/// `null -> T`, `T -> object`, `T -> dynamic` and `T -> ValueType`.
fn check_type_parameter_basics(c: &mut Conversions<'_, '_, '_>, tp: TypeId, expected: [C; 4]) {
    let null = null_type(c);
    let object = known(c, KnownTypeCode::Object);
    let dynamic = c.ts.intern(TypeData::Dynamic);
    let value_type = t(c, "ValueType");
    let [from_null, to_object, to_dynamic, to_value_type] = expected;
    assert_eq!(c.implicit_type(null, tp), from_null);
    assert_eq!(c.implicit_type(tp, object), to_object);
    assert_eq!(c.implicit_type(tp, dynamic), to_dynamic);
    assert_eq!(c.implicit_type(tp, value_type), to_value_type);
}

#[test]
fn type_parameter_with_reference_type_constraint() {
    Fixture::new().run(|c| {
        let tp = class_tp(c, 0, "T", true, false, Vec::new());
        check_type_parameter_basics(
            c,
            tp,
            [
                C::NULL_LITERAL,
                C::IMPLICIT_REFERENCE,
                C::IMPLICIT_REFERENCE,
                C::None,
            ],
        );
    });
}

#[test]
fn type_parameter_with_value_type_constraint() {
    Fixture::new().run(|c| {
        let tp = class_tp(c, 0, "T", false, true, Vec::new());
        check_type_parameter_basics(c, tp, [C::None, C::BOXING, C::BOXING, C::BOXING]);
    });
}

#[test]
fn type_parameter_with_class_constraint() {
    Fixture::new().run(|c| {
        let string_comparer = t(c, "StringComparer");
        let tp = class_tp(c, 0, "T", false, false, vec![string_comparer]);
        check_type_parameter_basics(
            c,
            tp,
            [
                C::NULL_LITERAL,
                C::IMPLICIT_REFERENCE,
                C::IMPLICIT_REFERENCE,
                C::None,
            ],
        );
        assert_eq!(c.implicit_type(tp, string_comparer), C::IMPLICIT_REFERENCE);
        let icomparer = t(c, "IComparer");
        assert_eq!(c.implicit_type(tp, icomparer), C::IMPLICIT_REFERENCE);
        let icomparer_int = t(c, "IComparer<int>");
        assert_eq!(c.implicit_type(tp, icomparer_int), C::None);
        let icomparer_string = t(c, "IComparer<string>");
        assert_eq!(c.implicit_type(tp, icomparer_string), C::IMPLICIT_REFERENCE);
    });
}

#[test]
fn type_parameter_with_interface_constraint() {
    Fixture::new().run(|c| {
        let ilist = t(c, "IList");
        let tp = class_tp(c, 0, "T", false, false, vec![ilist]);
        check_type_parameter_basics(c, tp, [C::None, C::BOXING, C::BOXING, C::None]);
        assert_eq!(c.implicit_type(tp, ilist), C::BOXING);
        let ienumerable = t(c, "IEnumerable");
        assert_eq!(c.implicit_type(tp, ienumerable), C::BOXING);
    });
}

#[test]
fn user_defined_implicit_conversion() {
    Fixture::new().run(|c| {
        let conv = implicit(c, "DateTime", "DateTimeOffset");
        assert!(conv.is_implicit() && conv.is_user_defined());
        assert_eq!(method_full_name(c, method(&conv)), "System.DateTimeOffset.op_Implicit");

        assert_eq!(implicit(c, "DateTimeOffset", "DateTime"), C::None);

        let class_implementing_idisposable = t(c, &tsn("ClassImplementingIDisposable"));
        let generic_struct_instance = t(
            c,
            &format!(
                "{}<IDisposable>",
                tsn("GenericStructWithIDisposableConstraintAndImplicitConversion")
            ),
        );

        // C => S<I>
        let c2 = c.implicit_type(class_implementing_idisposable, generic_struct_instance);
        assert!(c2.is_implicit() && c2.is_user_defined());
        assert_eq!(
            method_full_name(c, method(&c2)),
            "ICSharpCode.Decompiler.Tests.TypeSystem.GenericStructWithIDisposableConstraintAndImplicitConversion.op_Implicit"
        );

        assert_eq!(
            c.implicit_type(generic_struct_instance, class_implementing_idisposable),
            C::None
        );
    });
}

#[test]
fn user_defined_implicit_nullable_conversion() {
    Fixture::new().run(|c| {
        // User-defined conversion followed by nullable conversion
        let conv = implicit(c, "DateTime", "DateTimeOffset?");
        assert!(conv.is_valid() && conv.is_user_defined());
        assert!(!conv.is_lifted());
        // Lifted user-defined conversion
        let conv = implicit(c, "DateTime?", "DateTimeOffset?");
        assert!(conv.is_valid() && conv.is_user_defined() && conv.is_lifted());
        // User-defined conversion doesn't drop the nullability
        let conv = implicit(c, "DateTime?", "DateTimeOffset");
        assert!(!conv.is_valid());
    });
}

#[test]
fn integer_literal_to_enum_conversions() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "LoaderOptimization"));
        assert!(int_lit(c, ConstValue::Int64(0), "LoaderOptimization"));
        assert!(int_lit(c, ConstValue::Int32(0), "LoaderOptimization?"));
        assert!(!int_lit(c, ConstValue::Int32(0), "string"));
        assert!(!int_lit(c, ConstValue::Int32(1), "LoaderOptimization"));
    });
}

#[test]
fn implicit_constant_expression_conversion() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "int"));
        assert!(int_lit(c, ConstValue::Int32(0), "ushort"));
        assert!(int_lit(c, ConstValue::Int32(0), "sbyte"));

        assert!(int_lit(c, ConstValue::Int32(-1), "int"));
        assert!(!int_lit(c, ConstValue::Int32(-1), "ushort"));
        assert!(int_lit(c, ConstValue::Int32(-1), "sbyte"));

        assert!(int_lit(c, ConstValue::Int32(200), "int"));
        assert!(int_lit(c, ConstValue::Int32(200), "ushort"));
        assert!(!int_lit(c, ConstValue::Int32(200), "sbyte"));
    });
}

#[test]
fn implicit_long_constant_expression_conversion() {
    Fixture::new().run(|c| {
        assert!(!int_lit(c, ConstValue::Int64(0), "int"));
        assert!(!int_lit(c, ConstValue::Int64(0), "short"));
        assert!(int_lit(c, ConstValue::Int64(0), "long"));
        assert!(int_lit(c, ConstValue::Int64(0), "ulong"));

        assert!(int_lit(c, ConstValue::Int64(-1), "long"));
        assert!(!int_lit(c, ConstValue::Int64(-1), "ulong"));
    });
}

#[test]
fn implicit_constant_expression_conversion_to_nullable() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "uint?"));
        assert!(int_lit(c, ConstValue::Int32(0), "short?"));
        assert!(int_lit(c, ConstValue::Int32(0), "byte?"));

        assert!(!int_lit(c, ConstValue::Int32(-1), "uint?"));
        assert!(int_lit(c, ConstValue::Int32(-1), "short?"));
        assert!(!int_lit(c, ConstValue::Int32(-1), "byte?"));

        assert!(int_lit(c, ConstValue::Int32(200), "uint?"));
        assert!(int_lit(c, ConstValue::Int32(200), "short?"));
        assert!(int_lit(c, ConstValue::Int32(200), "byte?"));

        assert!(!int_lit(c, ConstValue::Int64(0), "uint?"));
        assert!(int_lit(c, ConstValue::Int64(0), "long?"));
        assert!(int_lit(c, ConstValue::Int64(0), "ulong?"));

        assert!(int_lit(c, ConstValue::Int64(-1), "long?"));
        assert!(!int_lit(c, ConstValue::Int64(-1), "ulong?"));
    });
}

#[test]
fn implicit_constant_expression_conversion_number_interfaces() {
    Fixture::new().run(|c| {
        assert!(int_lit(c, ConstValue::Int32(0), "IFormattable"));
        assert!(int_lit(c, ConstValue::Int32(0), "IComparable<int>"));
        assert!(!int_lit(c, ConstValue::Int32(0), "IComparable<short>"));
        assert!(!int_lit(c, ConstValue::Int32(0), "IComparable<long>"));
    });
}

#[test]
fn better_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(better(c, "string", "string", "object"), 1);
        assert_eq!(better(c, "string", "object", "IComparable<string>"), 2);
        assert_eq!(
            better(c, "string", "IEnumerable<char>", "IComparable<string>"),
            0
        );
    });
}

#[test]
fn better_primitive_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(better(c, "short", "int", "long"), 1);
        assert_eq!(better(c, "short", "int", "uint"), 1);
        assert_eq!(better(c, "ushort", "uint", "int"), 2);
        assert_eq!(better(c, "char", "short", "int"), 1);
        assert_eq!(better(c, "char", "ushort", "int"), 1);
        assert_eq!(better(c, "sbyte", "long", "ulong"), 1);
        assert_eq!(better(c, "byte", "ushort", "short"), 2);

        assert_eq!(better_const(c, ConstValue::Int32(1), "sbyte", "byte"), 1);
        assert_eq!(better_const(c, ConstValue::Int32(1), "ushort", "sbyte"), 2);
    });
}

#[test]
fn better_nullable_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(better(c, "byte", "int", "uint?"), 0);
        assert_eq!(better(c, "byte?", "int?", "uint?"), 0);
        assert_eq!(better(c, "byte", "ushort?", "uint?"), 1);
        assert_eq!(better(c, "byte?", "ulong?", "uint?"), 2);
        assert_eq!(better(c, "byte", "ushort?", "uint"), 0);
        assert_eq!(better(c, "byte", "ushort?", "int"), 0);
        assert_eq!(better(c, "byte", "ulong?", "uint"), 2);
        assert_eq!(better(c, "byte", "ulong?", "int"), 0);
        assert_eq!(better(c, "ushort?", "long?", "int?"), 2);
        assert_eq!(better(c, "sbyte", "int?", "uint?"), 0);
    });
}

#[test]
fn expansive_inheritance() {
    Fixture::new().run(|c| {
        // interface A<in U> { }
        // interface B<X> : A<A<B<X>>> { }
        // Finding a conversion B<double> -> A<B<string>> must terminate even though the
        // base-type substitution keeps producing ever-larger types.
        let cases = tsn("ExpansiveInheritanceTestCases");
        let type1 = t(c, &format!("{cases}+B<double>"));
        let type2 = t(c, &format!("{cases}+A<{cases}+B<string>>"));
        assert!(!c.implicit_type(type1, type2).is_valid());
    });
}

/// `T` (method index 0, constrained to `U`) and `U` (method index 1), with the reference type
/// constraint on each as given.
fn t_constrained_to_u(
    c: &mut Conversions<'_, '_, '_>,
    t_class: bool,
    u_class: bool,
) -> (TypeId, TypeId) {
    let u = method_tp(c, 1, "U", u_class, Vec::new());
    let tp = method_tp(c, 0, "T", t_class, vec![u]);
    (tp, u)
}

#[test]
fn implicit_type_parameter_conversion() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where T : U  =>  "U u = t;" is a boxing conversion
        // (e.g. T = int, U = object)
        let (tp, u) = t_constrained_to_u(c, false, false);
        assert_eq!(c.implicit_type(tp, u), C::BOXING);
    });
}

#[test]
fn invalid_implicit_type_parameter_conversion() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where U : T  =>  "U u = t;" is invalid
        // (the constraint points the wrong way)
        let tp = method_tp(c, 0, "T", false, Vec::new());
        let u = method_tp(c, 1, "U", false, vec![tp]);
        assert_eq!(c.implicit_type(tp, u), C::None);
    });
}

#[test]
fn implicit_type_parameter_array_conversion() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where T : U  =>  "U[] u = t;" is invalid
        // (e.g. T = int, U = object: int[] is not convertible to object[])
        let (tp, u) = t_constrained_to_u(c, false, false);
        let (ta, ua) = (array(c, tp), array(c, u));
        assert_eq!(c.implicit_type(ta, ua), C::None);
    });
}

#[test]
fn implicit_type_parameter_conversion_with_class_constraint() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where T : class, U where U : class  =>  "U u = t;" is a reference
        // conversion
        let (tp, u) = t_constrained_to_u(c, true, true);
        assert_eq!(c.implicit_type(tp, u), C::IMPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_array_conversion_with_class_constraint() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where T : class, U where U : class  =>  "U[] u = t;" is a
        // covariant array conversion (both are known to be reference types)
        let (tp, u) = t_constrained_to_u(c, true, true);
        let (ta, ua) = (array(c, tp), array(c, u));
        assert_eq!(c.implicit_type(ta, ua), C::IMPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_conversion_with_class_constraint_only_on_t() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where T : class, U  =>  "U u = t;" is a reference conversion
        // (T is known to be a reference type, so no boxing is involved)
        let (tp, u) = t_constrained_to_u(c, true, false);
        assert_eq!(c.implicit_type(tp, u), C::IMPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_array_conversion_with_class_constraint_only_on_t() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where T : class, U  =>  "U[] u = t;" is a covariant array
        // conversion
        let (tp, u) = t_constrained_to_u(c, true, false);
        let (ta, ua) = (array(c, tp), array(c, u));
        assert_eq!(c.implicit_type(ta, ua), C::IMPLICIT_REFERENCE);
    });
}

#[test]
fn method_group_conversion_void() {
    Fixture::new().run(|c| {
        // delegate void D();
        // D d = M;  with  public static void M() {}
        let conv = mg_conv(c, &mgc("VoidStatic"), "M", &mgc("DVoid"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert!(!conv.delegate_captures_first_argument());
        assert!(conv.method().is_some());
    });
}

#[test]
fn method_group_conversion_void_instance_method() {
    Fixture::new().run(|c| {
        // delegate void D();
        // D d = M;  with  public void M() {}  (instance method)
        let conv = mg_conv(c, &mgc("VoidInstance"), "M", &mgc("DVoid"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert!(conv.delegate_captures_first_argument());
        assert!(conv.method().is_some());
    });
}

#[test]
fn method_group_conversion_matching_signature() {
    Fixture::new().run(|c| {
        // delegate object D(int argument);
        // D d = M;  with  public static object M(int argument) {...}
        let conv = mg_conv(c, &mgc("ReturnObjectFromInt"), "M", &mgc("DObjInt"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_invalid_return_type() {
    Fixture::new().run(|c| {
        // delegate object D(int argument);
        // D d = M;  with  public static int M(int argument) {...}
        // int -> object is a boxing conversion, not an identity/reference conversion, so the
        // method is not delegate-compatible.
        let conv = mg_conv(c, &mgc("ReturnIntFromInt"), "M", &mgc("DObjInt"));
        assert!(!conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_covariant_return_type() {
    Fixture::new().run(|c| {
        // delegate object D(int argument);
        // D d = M;  with  public static string M(int argument) {...}
        let conv = mg_conv(c, &mgc("ReturnStringFromInt"), "M", &mgc("DObjInt"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_ref_argument_types_equal() {
    Fixture::new().run(|c| {
        // delegate void D(ref object o);
        // D d = M;  with  public static void M(ref object o) {}
        let conv = mg_conv(c, &mgc("RefObjParam"), "M", &mgc("DRefObj"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_ref_argument_object_vs_dynamic() {
    Fixture::new().run(|c| {
        // delegate void D(ref object o);
        // D d = M;  with  public static void M(ref dynamic o) {}
        // ref parameters require an identity conversion, and object <-> dynamic IS an
        // identity conversion, so this is valid (Roslyn accepts it).
        let conv = mg_conv(c, &mgc("RefDynamicParam"), "M", &mgc("DRefObj"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_ref_vs_out() {
    Fixture::new().run(|c| {
        // delegate void D(ref object o);
        // D d = M;  with  public static void M(out object o) {...}
        let conv = mg_conv(c, &mgc("OutObjParam"), "M", &mgc("DRefObj"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn method_group_conversion_ref_vs_normal() {
    Fixture::new().run(|c| {
        // delegate void D(ref object o);
        // D d = M;  with  public static void M(object o) {}
        let conv = mg_conv(c, &mgc("ObjParam"), "M", &mgc("DRefObj"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn method_group_conversion_normal_vs_out() {
    Fixture::new().run(|c| {
        // delegate void D(object o);
        // D d = M;  with  public static void M(out object o) {...}
        let conv = mg_conv(c, &mgc("OutObjParam"), "M", &mgc("DObj"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn method_group_conversion_matching_normal_parameter() {
    Fixture::new().run(|c| {
        // delegate void D(object o);
        // D d = M;  with  public static void M(object o) {}
        let conv = mg_conv(c, &mgc("ObjParam"), "M", &mgc("DObj"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_identity_conversion() {
    Fixture::new().run(|c| {
        // delegate void D(object o);
        // D d = M;  with  public static void M(dynamic o) {}
        // object -> dynamic is an identity conversion, so M is delegate-compatible.
        let conv = mg_conv(c, &mgc("DynamicParam"), "M", &mgc("DObj"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_contravariance() {
    Fixture::new().run(|c| {
        // delegate void D(string o);
        // D d = M;  with  public static void M(object o) {}
        let conv = mg_conv(c, &mgc("ObjParam"), "M", &mgc("DStr"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
    });
}

#[test]
#[ignore = "Not sure if this conversion should be valid or not... NR and mcs both accept it as valid, csc treats it as invalid"]
fn method_group_conversion_no_contravariance_dynamic() {
    Fixture::new().run(|c| {
        // delegate void D(string o);
        // D d = M;  with  public static void M(dynamic o) {}
        let conv = mg_conv(c, &mgc("DynamicParam"), "M", &mgc("DStr"));
        assert!(conv.is_method_group());
    });
}

#[test]
fn method_group_conversion_exact_match_is_better() {
    Fixture::new().run(|c| {
        // delegate void D(string a);
        // D d = M;  with  static void M(object x) {}  and  static void M(string x = null) {}
        let conv = mg_conv(c, &mgc("OverloadObjectOrOptionalString"), "M", &mgc("DStr"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert_eq!(single_parameter_full_name(c, &conv), "System.String");
    });
}

#[test]
fn method_group_conversion_cannot_leave_out_optional_parameters() {
    Fixture::new().run(|c| {
        // delegate void D(string a);
        // D d = M;  with  static void M(object x) {}  and  static void M(string x, string y = null) {}
        // The two-parameter overload cannot bind to a one-parameter delegate.
        let conv = mg_conv(
            c,
            &mgc("OverloadObjectOrStringPlusOptional"),
            "M",
            &mgc("DStr"),
        );
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert_eq!(single_parameter_full_name(c, &conv), "System.Object");
    });
}

#[test]
fn method_group_conversion_cannot_use_expanded_params() {
    Fixture::new().run(|c| {
        // delegate void D(string a);
        // D d = M;  with  static void M(object x) {}  and  static void M(params string[] x) {}
        // The params overload only matches in expanded form, which method group conversions
        // do not use.
        let conv = mg_conv(c, &mgc("OverloadObjectOrParamsString"), "M", &mgc("DStr"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert_eq!(single_parameter_full_name(c, &conv), "System.Object");
    });
}

#[test]
fn method_group_conversion_extension_method() {
    Fixture::new().run(|c| {
        // static class Ext { public static void M(this string s, int x) {} }
        // delegate void D(int a);
        // string s = ""; D d = s.M;
        let string = known(c, KnownTypeCode::String);
        let ext_type = t(c, &tsn("MethodGroupConversionExt"));
        let mut ext = methods(c.ts, ext_type, "M");
        assert_eq!(ext.len(), 1, "a single extension method M");
        let extension_method = ext.remove(0);
        let conv = mg_conv_with(
            c,
            "string",
            "M",
            &mgc("DInt"),
            Some(ResolveResult::Expression(string)),
            Some(vec![extension_method]),
            Vec::new(),
        );
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert!(conv.delegate_captures_first_argument());
    });
}

#[test]
fn method_group_conversion_extension_method_used_as_static_method() {
    Fixture::new().run(|c| {
        // static class Ext { public static void M(this string s, int x) {} }
        // delegate void D(string s, int a);
        // D d = Ext.M;
        let conv = mg_conv(c, &tsn("MethodGroupConversionExt"), "M", &mgc("DStrInt"));
        assert!(conv.is_valid());
        assert!(conv.is_method_group());
        assert!(!conv.delegate_captures_first_argument());
    });
}

#[test]
fn method_group_conversion_object_to_dynamic() {
    Fixture::new().run(|c| {
        // Action<dynamic> x = F;  with  public void F(object o) {}
        let conv = mg_conv(c, &mgc("ObjParamInstance"), "F", "Action<dynamic>");
        assert!(conv.is_valid());
    });
}

#[test]
fn method_group_conversion_object_to_dynamic_generic_argument() {
    Fixture::new().run(|c| {
        // Action<List<dynamic>> x = F;  with  public void F(List<object> l) {}
        let conv = mg_conv(
            c,
            &mgc("ObjListParamInstance"),
            "F",
            "Action<List<dynamic>>",
        );
        assert!(conv.is_valid());
    });
}

#[test]
fn method_group_conversion_object_to_dynamic_return_value() {
    Fixture::new().run(|c| {
        // Func<dynamic> x = F;  with  public object F() {...}
        let conv = mg_conv(c, &mgc("ObjReturnInstance"), "F", "Func<dynamic>");
        assert!(conv.is_valid());
    });
}

#[test]
fn method_group_conversion_dynamic_to_object() {
    Fixture::new().run(|c| {
        // Action<object> x = F;  with  public void F(dynamic o) {}
        let conv = mg_conv(c, &mgc("DynamicParamInstance"), "F", "Action<object>");
        assert!(conv.is_valid());
    });
}

#[test]
fn method_group_conversion_dynamic_to_object_generic_argument() {
    Fixture::new().run(|c| {
        // Action<List<object>> x = F;  with  public void F(List<dynamic> l) {}
        let conv = mg_conv(
            c,
            &mgc("DynamicListParamInstance"),
            "F",
            "Action<List<object>>",
        );
        assert!(conv.is_valid());
    });
}

#[test]
fn method_group_conversion_dynamic_to_object_return_value() {
    Fixture::new().run(|c| {
        // Func<object> x = F;  with  public dynamic F() {...}
        let conv = mg_conv(c, &mgc("DynamicReturnInstance"), "F", "Func<object>");
        assert!(conv.is_valid());
    });
}

#[test]
fn user_defined_int_literal_via_uint_to_custom_struct() {
    Fixture::new().run(|c| {
        // struct T { public static implicit operator T(uint a) {...} }
        // T t = 1;
        let conv = constant(c, ConstValue::Int32(1), &udc("TFromUInt"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
    });
}

#[test]
fn user_defined_null_literal_via_string_to_custom_struct() {
    Fixture::new().run(|c| {
        // struct T { public static implicit operator T(string a) {...} }
        // T t = null;
        let conv = implicit(c, "null", &udc("TFromString"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
    });
}

#[test]
fn user_defined_can_use_lifted_even_if_return_type_already_nullable() {
    Fixture::new().run(|c| {
        // struct S { public static implicit operator short?(S s) {...} }
        // int? i = s;  with s of type S?
        let conv = implicit(c, &format!("{}?", udc("SToNullableShort")), "int?");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(conv.is_lifted());
    });
}

/// The name of the first parameter of the conversion's operator.
fn first_parameter_name(conversion: &C) -> &str {
    &method(conversion).parameters[0].name
}

#[test]
fn user_defined_implicit_conversion_picks_exact_source_type_if_possible() {
    Fixture::new().run(|c| {
        // Convertible has operators from int ("i") and short ("s");
        // Convertible a = 33;
        let conv = constant(c, ConstValue::Int32(33), &udc("ConvertibleFromIntOrShort"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "i");
    });
}

#[test]
fn user_defined_implicit_conversion_picks_most_encompassed_source_type() {
    Fixture::new().run(|c| {
        // Convertible has operators from long ("l") and uint ("ui");
        // Convertible a = (ushort)33;
        let conv = constant(c, ConstValue::UInt16(33), &udc("ConvertibleFromLongOrUInt"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "ui");
    });
}

#[test]
fn user_defined_implicit_conversion_no_most_encompassed_source_type_is_invalid() {
    Fixture::new().run(|c| {
        // Convertible has operators from ulong and int; neither source type encompasses the
        // other, so the conversion from ushort is ambiguous.
        let conv = constant(c, ConstValue::UInt16(33), &udc("ConvertibleFromULongOrInt"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_implicit_conversion_picks_exact_target_type_if_possible() {
    Fixture::new().run(|c| {
        // Convertible has operators to int ("i") and short ("s");
        // int a = new Convertible();
        let conv = implicit(c, &udc("ConvertibleToIntOrShort"), "int");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "i");
    });
}

#[test]
fn user_defined_implicit_conversion_picks_most_encompassing_target_type() {
    Fixture::new().run(|c| {
        // Convertible has operators to int ("i") and ushort ("us");
        // ulong a = new Convertible();  -- only ushort converts implicitly to ulong
        let conv = implicit(c, &udc("ConvertibleToIntOrUShort"), "ulong");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "us");
    });
}

#[test]
fn user_defined_implicit_conversion_no_most_encompassing_target_type_is_invalid() {
    Fixture::new().run(|c| {
        // Convertible has operators to uint and short; neither target type encompasses the
        // other, so the conversion to long is ambiguous.
        let conv = implicit(c, &udc("ConvertibleToUIntOrShort"), "long");
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_implicit_conversion_ambiguous_is_invalid() {
    Fixture::new().run(|c| {
        // Both AmbiguousA and AmbiguousB declare implicit operator AmbiguousB(AmbiguousA).
        let conv = implicit(c, &udc("AmbiguousA"), &udc("AmbiguousB"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_implicit_conversion_defined_nullable_takes_precedence_over_lifted() {
    Fixture::new().run(|c| {
        // struct Convertible declares operators from int ("i") and from int? ("ni");
        // Convertible? a = (int?)33;  -- the user-defined nullable operator wins over the
        // lifted form of the int operator.
        let conv = implicit(c, "int?", &format!("{}?", udc("NullableConvertible")));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(!conv.is_lifted());
        assert_eq!(first_parameter_name(&conv), "ni");
    });
}

#[test]
fn user_defined_implicit_conversion_uint_constant() {
    Fixture::new().run(|c| {
        // Convertible has operators from long ("l") and uint ("ui");
        // Convertible a = 33;  -- the constant 33 converts to uint, which is more specific
        let conv = constant(c, ConstValue::Int32(33), &udc("ConvertibleFromLongOrUInt"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "ui");
    });
}

#[test]
fn user_defined_implicit_conversion_nullable_uint_constant() {
    Fixture::new().run(|c| {
        // Convertible has operators from long? ("l") and uint? ("ui");
        // Convertible a = 33;
        let conv = constant(
            c,
            ConstValue::Int32(33),
            &udc("ConvertibleFromNullableLongOrNullableUInt"),
        );
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "ui");
    });
}

#[test]
fn user_defined_implicit_conversion_use_short_result_because_nullable_cannot_be_unpacked() {
    Fixture::new().run(|c| {
        // operators to int? ("i") and short ("s");
        // int x = new Test();  -- int? cannot be unpacked to int, so short is used
        let conv = implicit(c, &udc("ToNullableIntOrShort"), "int");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        let ret = method(&conv).return_type;
        assert_eq!(c.ts.full_name(ret), "System.Int16");
    });
}

#[test]
fn user_defined_implicit_conversion_short_or_nullable_byte_target() {
    Fixture::new().run(|c| {
        // operators to short ("s") and byte? ("b");
        // int? x = new Test();
        let conv = implicit(c, &udc("ToShortOrNullableByte"), "int?");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        let ret = method(&conv).return_type;
        assert_eq!(c.ts.full_name(ret), "System.Int16");
    });
}

#[test]
fn user_defined_implicit_conversion_byte_or_nullable_short_target() {
    Fixture::new().run(|c| {
        // operators to byte ("b") and short? ("s");
        // int? x = new Test();
        let conv = implicit(c, &udc("ToByteOrNullableShort"), "int?");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "s");
    });
}

#[test]
fn user_defined_implicit_conversion_int_or_nullable_long_source() {
    Fixture::new().run(|c| {
        // operators from int ("i") and long? ("l"); source is short
        let conv = implicit(c, "short", &udc("FromIntOrNullableLong"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "i");
    });
}

#[test]
fn user_defined_implicit_conversion_nullable_int_or_long_source() {
    Fixture::new().run(|c| {
        // operators from int? ("i") and long ("l"); source is short:
        // neither int? nor long is more specific, so the conversion is ambiguous
        let conv = implicit(c, "short", &udc("FromNullableIntOrLong"));
        assert!(!conv.is_valid());
        assert!(conv.is_user_defined());
    });
}

#[test]
fn user_defined_implicit_conversion_nullable_int_or_long_constant_source() {
    Fixture::new().run(|c| {
        // operators from int? ("i") and long ("l"); source is the constant 1
        let conv = constant(c, ConstValue::Int32(1), &udc("FromNullableIntOrLong"));
        assert!(!conv.is_valid());
        assert!(conv.is_user_defined());
    });
}

#[test]
fn user_defined_implicit_conversion_nullable_int_or_nullable_long_source() {
    Fixture::new().run(|c| {
        // operators from int? ("i") and long? ("l"); source is short
        let conv = implicit(c, "short", &udc("FromNullableIntOrNullableLong"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(first_parameter_name(&conv), "i");
    });
}

/// A fake method named M with a single parameter `x` of the given type.
fn make_unary_method(c: &mut Conversions<'_, '_, '_>, parameter_type: &str) -> Method {
    let ty = t(c, parameter_type);
    let void = known(c, KnownTypeCode::Void);
    let mut p = Parameter::of_type(ty);
    p.name = "x".to_owned();
    Method::fake("M", vec![p], void)
}

#[test]
fn prefer_user_defined_conversion_over_reference_conversion() {
    Fixture::new().run(|c| {
        // M(new AA()) with overloads M(object) and M(string), where AA has an implicit
        // conversion to string, picks M(string) -- not because user-defined conversions are
        // better, but because string is a better conversion target.
        let arg = t(c, &udc("ConvertibleToString"));
        let mut or =
            OverloadResolution::new(vec![ResolveResult::Expression(arg)], None, Vec::new());
        let m_object = make_unary_method(c, "object");
        let m_string = make_unary_method(c, "string");
        assert_eq!(
            or.add_candidate(c, m_object),
            OverloadResolutionErrors::NONE
        );
        assert_eq!(
            or.add_candidate(c, m_string.clone()),
            OverloadResolutionErrors::NONE
        );
        assert!(!or.is_ambiguous());
        assert_eq!(or.best_candidate(), Some(&m_string));
    });
}

#[test]
fn prefer_ambiguous_conversion_over_reference_conversion() {
    Fixture::new().run(|c| {
        // Ambiguous conversions are a compiler error; but they are not preventing the
        // overload from being chosen.

        // The user-defined conversion AmbiguousA -> AmbiguousB is ambiguous (declared in both
        // classes) and therefore invalid...
        let conv = implicit(c, &udc("AmbiguousA"), &udc("AmbiguousB"));
        assert!(conv.is_user_defined());
        assert!(!conv.is_valid());

        // ...but M(new AmbiguousA()) with overloads M(AmbiguousB) and M(object) still picks
        // M(AmbiguousB), because AmbiguousB is a better conversion target than object.
        let arg = t(c, &udc("AmbiguousA"));
        let mut or =
            OverloadResolution::new(vec![ResolveResult::Expression(arg)], None, Vec::new());
        let m_ambiguous_b = make_unary_method(c, &udc("AmbiguousB"));
        let m_object = make_unary_method(c, "object");
        or.add_candidate(c, m_ambiguous_b.clone());
        or.add_candidate(c, m_object);
        assert_eq!(or.best_candidate(), Some(&m_ambiguous_b));
    });
}

#[test]
fn user_defined_implicit_conversion_conversion_before_user_defined_operator_is_correct() {
    Fixture::new().run(|c| {
        // Convertible has an operator from long; converting an int goes
        // int -> long -> Convertible.
        let conv = implicit(c, "int", &udc("ConvertibleFromLong"));
        assert!(conv.is_valid());
        assert!(before(&conv).is_implicit());
        assert!(before(&conv).is_numeric());
        assert!(before(&conv).is_valid());
        assert!(after(&conv).is_identity());
    });
}

#[test]
fn user_defined_implicit_conversion_conversion_after_user_defined_operator_is_correct() {
    Fixture::new().run(|c| {
        // Convertible has an operator to int; converting to long goes
        // Convertible -> int -> long.
        let conv = implicit(c, &udc("ConvertibleToInt"), "long");
        assert!(conv.is_valid());
        assert!(before(&conv).is_identity());
        assert!(after(&conv).is_implicit());
        assert!(after(&conv).is_numeric());
        assert!(after(&conv).is_valid());
    });
}

#[test]
fn user_defined_implicit_conversion_is_implicit() {
    Fixture::new().run(|c| {
        // Bug icsharpcode/NRefactory#183: conversions from constant expressions were
        // incorrectly marked as explicit
        let conv = constant(c, ConstValue::Int32(7), &udc("JsNumber"));
        assert!(conv.is_valid());
        assert!(conv.is_implicit());
        assert!(!conv.is_explicit());
        assert_eq!(before(&conv), &C::IDENTITY);
        assert_eq!(after(&conv), &C::IDENTITY);
    });
}

#[test]
fn tuple_identity_conversion_with_underlying_value_tuple() {
    Fixture::new().run(|c| {
        // C# standard 10.2.2: identity conversion between a tuple type and the corresponding
        // constructed ValueTuple<...> type
        let int = known(c, KnownTypeCode::Int32);
        let string = known(c, KnownTypeCode::String);
        let tuple_type = tuple(c.ts, &[int, string], &[Some("a"), Some("b")]);
        let value_tuple_type = t(c, "ValueTuple<int, string>");
        assert_eq!(c.implicit_type(tuple_type, value_tuple_type), C::IDENTITY);
        assert_eq!(c.implicit_type(value_tuple_type, tuple_type), C::IDENTITY);
    });
}

#[test]
fn identity_conversion_nullable_reference_type() {
    Fixture::new().run(|c| {
        // C# standard 10.2.2: identity conversion between T and T? for any reference type T
        let string = known(c, KnownTypeCode::String);
        let nullable_string = c.ts.change_nullability(string, Nullability::Nullable);
        assert_eq!(c.implicit_type(string, nullable_string), C::IDENTITY);
        assert_eq!(c.implicit_type(nullable_string, string), C::IDENTITY);
    });
}

#[test]
fn numeric_conversion_matrix() {
    Fixture::new().run(|c| {
        // C# standard 10.2.3 (implicit numeric conversions) and 10.3.2 (explicit numeric
        // conversions). The implicit table below is transcribed from 10.2.3; every other pair
        // of distinct numeric types must be an explicit numeric conversion.
        let all = [
            "sbyte", "byte", "short", "ushort", "int", "uint", "long", "ulong", "char", "float",
            "double", "decimal",
        ];
        let implicit_targets = |from: &str| -> &'static [&'static str] {
            match from {
                "sbyte" => &["short", "int", "long", "float", "double", "decimal"],
                "byte" => &[
                    "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal",
                ],
                "short" => &["int", "long", "float", "double", "decimal"],
                "ushort" => &["int", "uint", "long", "ulong", "float", "double", "decimal"],
                "int" => &["long", "float", "double", "decimal"],
                "uint" => &["long", "ulong", "float", "double", "decimal"],
                "long" => &["float", "double", "decimal"],
                "ulong" => &["float", "double", "decimal"],
                "char" => &[
                    "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal",
                ],
                "float" => &["double"],
                "double" | "decimal" => &[],
                _ => unreachable!(),
            }
        };
        for from in all {
            for to in all {
                let pair = format!("{from} -> {to}");
                if from == to {
                    assert_eq!(implicit(c, from, to), C::IDENTITY, "{pair}");
                } else if implicit_targets(from).contains(&to) {
                    assert_eq!(implicit(c, from, to), C::IMPLICIT_NUMERIC, "{pair}");
                    assert_eq!(explicit(c, from, to), C::IMPLICIT_NUMERIC, "{pair}");
                } else {
                    assert_eq!(implicit(c, from, to), C::None, "{pair}");
                    assert_eq!(explicit(c, from, to), C::EXPLICIT_NUMERIC, "{pair}");
                }
            }
        }
    });
}

#[test]
fn interpolated_string_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.2.5: an interpolated string expression converts to
        // System.IFormattable and System.FormattableString
        let string = known(c, KnownTypeCode::String);
        let interpolated = ResolveResult::InterpolatedString(string);
        let iformattable = known(c, KnownTypeCode::IFormattable);
        let formattable_string = known(c, KnownTypeCode::FormattableString);
        assert_eq!(
            c.implicit(&interpolated, iformattable),
            C::IMPLICIT_INTERPOLATED_STRING
        );
        assert_eq!(
            c.implicit(&interpolated, formattable_string),
            C::IMPLICIT_INTERPOLATED_STRING
        );
        assert_eq!(c.implicit(&interpolated, string), C::IDENTITY);
    });
}

#[test]
fn identity_derived_nullable_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.2.6/10.6.1: nullable conversion derived from the identity conversion
        assert_eq!(implicit(c, "int", "int?"), C::IMPLICIT_NULLABLE);
    });
}

#[test]
fn delegate_to_system_delegate_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.2.8: from any delegate_type to System.Delegate and the interfaces it
        // implements
        assert_eq!(implicit(c, "Action", "Delegate"), C::IMPLICIT_REFERENCE);
        assert_eq!(
            implicit(c, "Action", "MulticastDelegate"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "Action", "ICloneable"), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, "Delegate", "Action"), C::None);
    });
}

#[test]
fn class_to_base_class_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.2.8: from any class_type S to any class_type T, provided S is derived
        // from T
        let derived = tsn("UserDefinedExplicitConversionTestCases+DerivedClass");
        let base = tsn("UserDefinedExplicitConversionTestCases+BaseClass");
        assert_eq!(implicit(c, &derived, &base), C::IMPLICIT_REFERENCE);
        assert_eq!(implicit(c, &base, &derived), C::None);
    });
}

#[test]
fn array_to_ireadonly_list_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.2.8: from S[] to IReadOnlyList<T> and its base interfaces
        assert_eq!(
            implicit(c, "string[]", "IReadOnlyList<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "string[]", "IReadOnlyList<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "string[]", "IReadOnlyCollection<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(implicit(c, "string[,]", "IReadOnlyList<string>"), C::None);
    });
}

#[test]
fn variance_conversion_with_dynamic_type_argument() {
    Fixture::new().run(|c| {
        // C# standard 10.2.8: implicit reference conversion via a variance-convertible type
        // where the type arguments differ by the object/dynamic identity conversion
        assert_eq!(
            implicit(c, "IEnumerable<string>", "IEnumerable<dynamic>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            implicit(c, "List<string>", "IEnumerable<dynamic>"),
            C::IMPLICIT_REFERENCE
        );
    });
}

#[test]
fn boxing_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.2.9
        assert_eq!(implicit(c, "int", "object"), C::BOXING);
        assert_eq!(implicit(c, "int", "ValueType"), C::BOXING);
        assert_eq!(implicit(c, "StringComparison", "Enum"), C::BOXING);
        assert_eq!(implicit(c, "int", "IFormattable"), C::BOXING);
        assert_eq!(implicit(c, "int", "IComparable<int>"), C::BOXING);
        assert_eq!(implicit(c, "int", "IComparable<string>"), C::None);
        // nullable value types box to the reference types their underlying type boxes to
        assert_eq!(implicit(c, "int?", "object"), C::BOXING);
        assert_eq!(implicit(c, "int?", "IFormattable"), C::BOXING);
        assert_eq!(implicit(c, "StringComparison?", "Enum"), C::BOXING);
    });
}

#[test]
fn boxing_conversion_via_variance() {
    Fixture::new().run(|c| {
        // C# standard 10.2.9: boxing to an interface that the implemented interface is
        // variance-convertible to
        let s = tsn("StructImplementingIEnumerableOfString");
        assert_eq!(implicit(c, &s, "IEnumerable<string>"), C::BOXING);
        assert_eq!(implicit(c, &s, "IEnumerable<object>"), C::BOXING);
        assert_eq!(implicit(c, &s, "IEnumerable"), C::BOXING);
        assert_eq!(implicit(c, &s, "IEnumerable<int>"), C::None);
    });
}

#[test]
fn implicit_constant_expression_conversion_to_uint64() {
    Fixture::new().run(|c| {
        // C# standard 10.2.11: an int constant expression converts to ulong provided its value
        // is non-negative
        assert!(int_lit(c, ConstValue::Int32(0), "ulong"));
        assert!(int_lit(c, ConstValue::Int32(200), "ulong"));
        assert!(!int_lit(c, ConstValue::Int32(-1), "ulong"));
    });
}

#[test]
fn type_parameter_conversion_via_variance() {
    Fixture::new().run(|c| {
        // C# standard 10.2.12 (last bullet group): conversions from T via a
        // variance-convertible interface of its effective base class / interface set
        let list_string = t(c, "List<string>");
        let tp = method_tp(c, 0, "T", false, vec![list_string]);
        let ienumerable_object = t(c, "IEnumerable<object>");
        assert_eq!(
            c.implicit_type(tp, ienumerable_object),
            C::IMPLICIT_REFERENCE
        );

        let ienumerable_string = t(c, "IEnumerable<string>");
        let t2 = method_tp(c, 0, "T", false, vec![ienumerable_string]);
        assert_eq!(c.implicit_type(t2, ienumerable_object), C::BOXING);
    });
}

#[test]
fn tuple_literal_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.2.13: implicit conversion from a tuple literal, using the implicit
        // conversions of the element expressions
        let int = known(c, KnownTypeCode::Int32);
        let string = known(c, KnownTypeCode::String);
        let byte = known(c, KnownTypeCode::Byte);
        let null = null_type(c);
        // a TupleResolveResult with a null-typed element has the type NoType
        let no_type = c.ts.intern(TypeData::NoType);
        let literal = |value: i32| ResolveResult::Tuple {
            ty: no_type,
            elements: vec![
                ResolveResult::Constant {
                    ty: int,
                    value: ConstValue::Int32(value),
                },
                ResolveResult::Constant {
                    ty: null,
                    value: ConstValue::Null,
                },
            ],
        };
        let byte_string = tuple(c.ts, &[byte, string], &[None, None]);
        let byte_string_int = tuple(c.ts, &[byte, string, int], &[None, None, None]);

        // (2, null) -> (byte, string)
        assert_eq!(
            c.implicit(&literal(2), byte_string),
            C::tuple(vec![C::IMPLICIT_CONSTANT_EXPRESSION, C::NULL_LITERAL])
        );
        // arity mismatch
        assert_eq!(c.implicit(&literal(2), byte_string_int), C::None);
        // (300, null) -> (byte, string): 300 is out of range for byte
        assert_eq!(c.implicit(&literal(300), byte_string), C::None);
    });
}

#[test]
#[ignore = "Known bug: CSharpConversions does not support nullable conversions derived from tuple conversions, but csc accepts them"]
fn lifted_tuple_conversions() {
    Fixture::new().run(|c| {
        // csc accepts the nullable conversions derived from tuple conversions:
        //   (int, string) t = (1, "one");
        //   (long, object)? a = t;
        //   (int, string)? tn = t;
        //   (long, object)? b = tn;
        assert!(
            implicit(c, "(int, string)", "(long, object)?").is_valid(),
            "(int, string) -> (long, object)?"
        );
        assert!(
            implicit(c, "(int, string)?", "(long, object)?").is_valid(),
            "(int, string)? -> (long, object)?"
        );
    });
}

#[test]
fn user_defined_implicit_conversion_operator_declared_in_base_class_of_source() {
    Fixture::new().run(|c| {
        // C# standard 10.5.4: the set of considered operator declarations includes the base
        // classes of the source type
        let conv = implicit(c, &udc("DerivedFromOperatorInBaseClass"), "string");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        let declaring = method(&conv).declaring.expect("a declaring type");
        let full_name = c.ts.full_name(declaring);
        assert_eq!(full_name.rsplit('.').next(), Some("OperatorInBaseClass"));
    });
}

#[test]
fn default_literal_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.2.16: an implicit conversion exists from a default_literal to any type
        let no_type = c.ts.intern(TypeData::NoType);
        let default_literal = ResolveResult::DefaultLiteral(no_type);
        // a default_value_expression is a constant expression (C# standard 12.8.21)
        assert!(default_literal.is_compile_time_constant());
        let int = known(c, KnownTypeCode::Int32);
        let string = known(c, KnownTypeCode::String);
        let nullable_int = t(c, "int?");
        assert_eq!(c.implicit(&default_literal, int), C::DEFAULT_LITERAL);
        assert_eq!(c.implicit(&default_literal, string), C::DEFAULT_LITERAL);
        assert_eq!(
            c.implicit(&default_literal, nullable_int),
            C::DEFAULT_LITERAL
        );
        let tp = method_type_parameter(c.ts, 0, "T");
        assert_eq!(c.implicit(&default_literal, tp), C::DEFAULT_LITERAL);
        // explicit conversions include all implicit conversions, so (T)default is also valid
        assert_eq!(c.explicit(&default_literal, int), C::DEFAULT_LITERAL);
    });
}

#[test]
#[ignore = "C# standard 10.2.18 is not implemented: no ResolveResult represents a switch expression; the decompiler converts each arm separately in ILAst"]
fn switch_expression_conversion() {
    // C# standard 10.2.18: an implicit conversion exists from a switch_expression to every
    // type T to which all arm expressions implicitly convert. Once the semantic model gains a
    // switch-expression ResolveResult, this test should assert that the conversion exists iff
    // every arm converts to the target type.
    panic!("Switch expression conversions are not implemented.");
}

#[test]
fn throw_expression_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.2.17: throw expressions convert to any type
        let string = known(c, KnownTypeCode::String);
        let int = known(c, KnownTypeCode::Int32);
        assert_eq!(
            c.implicit(&ResolveResult::Throw, string),
            C::THROW_EXPRESSION
        );
        assert_eq!(c.implicit(&ResolveResult::Throw, int), C::THROW_EXPRESSION);
    });
}

#[test]
fn standard_implicit_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.4.2: standard implicit conversions exclude user-defined conversions
        let pairs = [
            ("int", "long", C::IMPLICIT_NUMERIC),
            ("string", "object", C::IMPLICIT_REFERENCE),
            ("DateTime", "DateTimeOffset", C::None),
        ];
        for (from, to, expected) in pairs {
            let from_t = t(c, from);
            let to_t = t(c, to);
            assert_eq!(
                c.standard_implicit(from_t, to_t),
                expected,
                "{from} -> {to}"
            );
        }
    });
}

#[test]
fn method_group_conversion_generic_method_type_inference() {
    Fixture::new().run(|c| {
        // C# standard 10.8: delegate parameter types are used to infer the type arguments of
        // a generic method group
        // delegate int D(string s, int i);
        // D d = F;  with  static T F<T>(string s, T t)  -- T=int is inferred
        let conv = mg_conv(c, &mgc("GenericMethods"), "F", &mgc("DStrIntRetInt"));
        assert!(conv.is_valid());
        assert!(single_type_argument_is_int32(c, &conv));
    });
}

#[test]
fn method_group_conversion_generic_method_explicit_type_arguments() {
    Fixture::new().run(|c| {
        // C# standard 10.8:
        // delegate int E();
        // E e = G<int>;  with  static T G<T>()
        let int = known(c, KnownTypeCode::Int32);
        let conv = mg_conv_with(
            c,
            &mgc("GenericMethods"),
            "G",
            &mgc("DRetInt"),
            None,
            None,
            vec![int],
        );
        assert!(conv.is_valid());
        assert!(single_type_argument_is_int32(c, &conv));
    });
}

#[test]
fn method_group_conversion_cannot_infer_from_return_type() {
    Fixture::new().run(|c| {
        // C# standard 10.8: the return type of the delegate is not used for inference
        // E e = G;  with  static T G<T>()
        let conv = mg_conv(c, &mgc("GenericMethods"), "G", &mgc("DRetInt"));
        assert!(!conv.is_valid());
    });
}

/// Stands in for a lambda or anonymous method whose body is an expression of a fixed type.
struct TestLambda {
    body_return_type: TypeId,
    parameters: Vec<Parameter>,
    has_parameter_list: bool,
    is_anonymous_method: bool,
    is_implicitly_typed: bool,
}

impl TestLambda {
    fn new(body_return_type: TypeId, parameters: Vec<Parameter>) -> TestLambda {
        TestLambda {
            body_return_type,
            parameters,
            has_parameter_list: true,
            is_anonymous_method: false,
            is_implicitly_typed: false,
        }
    }

    fn rr(self) -> ResolveResult {
        ResolveResult::Lambda(Rc::new(self))
    }
}

impl Lambda for TestLambda {
    fn has_parameter_list(&self) -> bool {
        self.has_parameter_list
    }

    fn is_anonymous_method(&self) -> bool {
        self.is_anonymous_method
    }

    fn is_implicitly_typed(&self) -> bool {
        self.is_implicitly_typed
    }

    fn is_async(&self) -> bool {
        false
    }

    fn parameters(&self) -> &[Parameter] {
        &self.parameters
    }

    fn inferred_return_type(
        &self,
        _conversions: &mut Conversions<'_, '_, '_>,
        _parameter_types: Option<&[TypeId]>,
    ) -> TypeId {
        self.body_return_type
    }

    fn is_valid(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        _parameter_types: &[TypeId],
        return_type: TypeId,
    ) -> C {
        conversions.implicit_type(self.body_return_type, return_type)
    }
}

/// `new DefaultParameter(type, "x")`.
fn param_x(ty: TypeId) -> Parameter {
    let mut p = Parameter::of_type(ty);
    p.name = "x".to_owned();
    p
}

#[test]
fn anonymous_function_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.7.1: compatibility of an anonymous function with a delegate type.
        // TestLambda stands in for a lambda whose body is an expression of the given type;
        // the signature checks under test here are performed by the conversions themselves.
        let int = known(c, KnownTypeCode::Int32);
        let lambda = || TestLambda::new(int, vec![param_x(int)]).rr();

        // (int x) => intExpr  is compatible with Func<int, int> and Func<int, double>
        let func_int_int = t(c, "Func<int, int>");
        assert!(c.implicit(&lambda(), func_int_int).is_valid());
        let func_int_double = t(c, "Func<int, double>");
        assert!(c.implicit(&lambda(), func_int_double).is_valid());
        // parameter count mismatch
        let func_int = t(c, "Func<int>");
        assert_eq!(c.implicit(&lambda(), func_int), C::None);
        // an explicitly typed parameter must have the delegate's parameter type
        let func_double_int = t(c, "Func<double, int>");
        assert_eq!(c.implicit(&lambda(), func_double_int), C::None);
        // an implicitly typed parameter list is incompatible with ref/out parameters
        let unknown = c.ts.intern(TypeData::UnknownSpecial);
        let implicitly_typed = TestLambda {
            is_implicitly_typed: true,
            ..TestLambda::new(int, vec![param_x(unknown)])
        };
        let d_ref_obj = t(c, &mgc("DRefObj"));
        assert_eq!(c.implicit(&implicitly_typed.rr(), d_ref_obj), C::None);
        // an anonymous method without a signature accepts any parameter list without out
        // parameters
        let anonymous_method = TestLambda {
            has_parameter_list: false,
            is_anonymous_method: true,
            ..TestLambda::new(int, Vec::new())
        };
        assert!(c.implicit(&anonymous_method.rr(), func_int_int).is_valid());
    });
}

#[test]
fn lambda_to_expression_tree_conversion() {
    Fixture::new().run(|c| {
        // C# standard 10.7.1: a lambda expression compatible with D is compatible with
        // Expression<D>; anonymous methods are not.
        let int = known(c, KnownTypeCode::Int32);
        let expression = t(c, "Expression<Func<int, int>>");
        let lambda = TestLambda::new(int, vec![param_x(int)]).rr();
        assert!(c.implicit(&lambda, expression).is_valid());
        let anonymous_method = TestLambda {
            has_parameter_list: false,
            is_anonymous_method: true,
            ..TestLambda::new(int, Vec::new())
        };
        assert_eq!(c.implicit(&anonymous_method.rr(), expression), C::None);
    });
}

// ---- First-class span conversions ----

/// `SpanConversion(typeof(from), typeof(to))` over `RefAssemblyCompilation`.
fn span_conversion(c: &mut Conversions<'_, '_, '_>, from: &str, to: &str) -> C {
    let from = ty_in(c.ts, from, &[]);
    let to = ty_in(c.ts, to, &[]);
    c.implicit_type(from, to)
}

#[test]
fn implicit_span_conversions() {
    Fixture::ref_assembly().run(|c| {
        assert_eq!(
            span_conversion(c, "string", "ReadOnlySpan<char>"),
            C::IMPLICIT_SPAN,
            "the span conversion must win over String's own op_Implicit: user-defined \
             conversions are not considered between span-convertible types"
        );
        assert_eq!(
            span_conversion(c, "string[]", "Span<string>"),
            C::IMPLICIT_SPAN
        );
        assert_eq!(
            span_conversion(c, "string[]", "ReadOnlySpan<object>"),
            C::IMPLICIT_SPAN
        );
        assert_eq!(
            span_conversion(c, "Span<string>", "ReadOnlySpan<object>"),
            C::IMPLICIT_SPAN
        );
        assert_eq!(
            span_conversion(c, "ReadOnlySpan<string>", "ReadOnlySpan<object>"),
            C::IMPLICIT_SPAN
        );
    });
}

#[test]
fn no_implicit_span_conversion_without_element_covariance() {
    Fixture::ref_assembly().run(|c| {
        // Roslyn: CS0029 - no conversion at all relates these.
        assert_eq!(span_conversion(c, "int[]", "ReadOnlySpan<long>"), C::None);

        // Roslyn: CS0266 - only an EXPLICIT (span) conversion exists. In particular the
        // user-defined route via Span<object>.op_Implicit(object[]) plus array covariance must
        // not be considered, because a span conversion exists for the pair.
        assert_eq!(span_conversion(c, "string[]", "Span<object>"), C::None);
    });
}

/// `SpanMethodGroupConversion(target, typeof(delegate))`: `target.M` where `M` is the
/// extension method of `SpanReceiverExtensionTestCase`.
fn span_method_group_conversion(
    c: &mut Conversions<'_, '_, '_>,
    target: TypeId,
    delegate: &str,
) -> C {
    let ext_type = ty_in(c.ts, &tsn("SpanReceiverExtensionTestCase"), &[]);
    let mut ext = methods(c.ts, ext_type, "M");
    assert_eq!(ext.len(), 1, "a single extension method M");
    let extension_method = ext.remove(0);
    let rr = group(
        c,
        target,
        "M",
        Some(ResolveResult::Expression(target)),
        Some(vec![extension_method]),
        Vec::new(),
    );
    let delegate = ty_in(c.ts, delegate, &[]);
    c.implicit(&rr, delegate)
}

#[test]
fn method_group_conversion_span_conversion_on_the_receiver_is_not_considered() {
    Fixture::ref_assembly().run(|c| {
        // C# 14 first-class spans: "span conversion is not considered when overload
        // resolution is performed for a method group conversion". For 'str.M' with M being an
        // extension on ReadOnlySpan<char>, Roslyn reports CS0123, even though the invocation
        // 'str.M()' is legal.
        let string = known(c, KnownTypeCode::String);
        assert_eq!(span_method_group_conversion(c, string, "Action"), C::None);
    });
}

#[test]
fn method_group_conversion_identity_receiver_on_span_extension_still_converts() {
    Fixture::ref_assembly().run(|c| {
        // With an identity-typed receiver the method group conversion stays legal.
        let span = ty_in(c.ts, "ReadOnlySpan<char>", &[]);
        let conv = span_method_group_conversion(c, span, "Action");
        assert!(conv.is_method_group());
        assert!(conv.is_valid());
    });
}

/// `DecompiledLambdaResolveResult.IsValid`: the explicit parameter types have to match up to
/// identity, a match beyond nullability is recorded, and the return type converts from the
/// inferred one.
#[test]
fn decompiled_lambda_conversions() {
    use ilspy_decompiler::semantics::decompiled_lambda::DecompiledLambda;
    Fixture::new().run(|c| {
        let int = known(c, KnownTypeCode::Int32);
        let double = known(c, KnownTypeCode::Double);
        let object = known(c, KnownTypeCode::Object);
        let dynamic = c.ts.intern(TypeData::Dynamic);
        let lambda = |param: TypeId| DecompiledLambda {
            delegate_type: object,
            inferred_return_type: int,
            return_type: int,
            parameters: vec![param_x(param)],
            attempted_conversion_with_type_mismatch: std::cell::Cell::new(false),
            implicitly_typed: false,
        };

        // (int x) => int converts to Func<int, int> and Func<int, double>.
        let l = lambda(int);
        assert!(l.is_valid(c, &[int], int).is_anonymous_function());
        assert!(l.is_valid(c, &[int], double).is_anonymous_function());
        assert!(!l.attempted_conversion_with_type_mismatch.get());
        let func_int_double = t(c, "Func<int, double>");
        assert!(
            c.implicit(&Rc::new(lambda(int)).rr(), func_int_double)
                .is_valid()
        );

        // Another parameter type: no conversion, and the attempt is recorded.
        assert_eq!(l.is_valid(c, &[double], int), C::None);
        assert!(l.attempted_conversion_with_type_mismatch.get());

        // dynamic and object are identical but not equal: valid, and recorded.
        let l = lambda(object);
        assert!(l.is_valid(c, &[dynamic], int).is_anonymous_function());
        assert!(l.attempted_conversion_with_type_mismatch.get());

        // A return type the inferred type does not convert to.
        let string = known(c, KnownTypeCode::String);
        assert_eq!(lambda(int).is_valid(c, &[int], string), C::None);

        // An implicitly typed lambda (of anonymous-type parameters) can convert with other
        // parameter types, and the attempt is recorded.
        let l = DecompiledLambda {
            implicitly_typed: true,
            ..lambda(int)
        };
        assert!(l.is_valid(c, &[double], int).is_anonymous_function());
        assert!(l.attempted_conversion_with_type_mismatch.get());
    });
}

trait IntoLambdaRr {
    fn rr(self) -> ResolveResult;
}

impl IntoLambdaRr for Rc<ilspy_decompiler::semantics::decompiled_lambda::DecompiledLambda> {
    fn rr(self) -> ResolveResult {
        ResolveResult::Lambda(self)
    }
}

/// `NullabilityAnnotatedType` answers member queries as its underlying type: a lambda converts
/// to `Action!` (a non-generic delegate type of a nullable-annotated library) as to `Action`.
#[test]
fn lambda_converts_to_annotated_delegate_type() {
    use ilspy_decompiler::semantics::decompiled_lambda::DecompiledLambda;
    Fixture::new().run(|c| {
        let void = known(c, KnownTypeCode::Void);
        let action = t(c, "System.Action");
        let annotated = c.ts.change_nullability(action, Nullability::NotNullable);
        assert!(matches!(c.ts.data(annotated), TypeData::Annotated { .. }));
        let lambda = Rc::new(DecompiledLambda {
            delegate_type: action,
            inferred_return_type: void,
            return_type: void,
            parameters: Vec::new(),
            attempted_conversion_with_type_mismatch: std::cell::Cell::new(false),
            implicitly_typed: false,
        })
        .rr();
        assert!(c.implicit(&lambda, action).is_valid());
        assert!(c.implicit(&lambda, annotated).is_valid());
    });
}
