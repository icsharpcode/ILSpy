//! `ICSharpCode.Decompiler.Tests/Semantics/ExplicitConversionTest.cs`.
//!
//! `ExplicitConversion(typeof(A), typeof(B))` is `explicit(c, "A", "B")`; the placeholder
//! class `Dynamic` is the type `dynamic`. The fixture types declared in
//! `TypeSystem/TypeSystemTestCase.cs` are named through `ud("X")` (nested in
//! `UserDefinedExplicitConversionTestCases`) or their full reflection name.

mod common;

use common::csharp::{compilation, ref_assembly_compilation, ty_in, type_parameter};
use ilspy_decompiler::semantics::{Conversion as C, Conversions, ResolveResult};
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::members::ConstValue;
use ilspy_decompiler::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};

const OUTER: &[&str] = &["ExplicitConversionsTest"];

const TS_NAMESPACE: &str = "ICSharpCode.Decompiler.Tests.TypeSystem";

struct Fixture {
    comp: Compilation<'static>,
}

impl Fixture {
    fn new() -> Fixture {
        Fixture {
            comp: compilation(),
        }
    }

    fn span() -> Fixture {
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

/// A type nested in `UserDefinedExplicitConversionTestCases`, with an optional suffix
/// such as `?`.
fn ud(name: &str) -> String {
    format!("{TS_NAMESPACE}.UserDefinedExplicitConversionTestCases+{name}")
}

fn t(c: &mut Conversions<'_, '_, '_>, s: &str) -> TypeId {
    ty_in(c.ts, s, OUTER)
}

fn explicit(c: &mut Conversions<'_, '_, '_>, from: &str, to: &str) -> C {
    let from = t(c, from);
    let to = t(c, to);
    c.explicit_type(from, to)
}

/// `ExplicitConstantConversion`: a constant of the value's type converted to `to`.
fn explicit_constant(c: &mut Conversions<'_, '_, '_>, value: ConstValue, to: &str) -> C {
    let code = match value {
        ConstValue::Int32(_) => KnownTypeCode::Int32,
        ConstValue::UInt16(_) => KnownTypeCode::UInt16,
        ConstValue::Int64(_) => KnownTypeCode::Int64,
        _ => panic!("unsupported constant {value:?}"),
    };
    let ty = c.ts.known_type(code);
    let to = t(c, to);
    c.explicit(&ResolveResult::Constant { ty, value }, to)
}

fn array(c: &mut Conversions<'_, '_, '_>, elem: TypeId) -> TypeId {
    c.ts.intern(TypeData::Array {
        elem,
        rank: 1,
        nullability: Nullability::Oblivious,
    })
}

/// `new DefaultTypeParameter(compilation, SymbolKind.Method, index, name, ...)`.
fn tp(
    c: &mut Conversions<'_, '_, '_>,
    index: u32,
    name: &str,
    reference_type: bool,
    value_type: bool,
    constraints: Vec<TypeId>,
) -> TypeId {
    type_parameter(
        c.ts,
        true,
        index,
        name,
        reference_type,
        value_type,
        false,
        constraints,
    )
}

fn method_param_name(conv: &C, index: usize) -> String {
    conv.method().expect("a method").parameters[index]
        .name
        .clone()
}

#[test]
fn pointer_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "int*", "short"), C::EXPLICIT_POINTER);
        assert_eq!(explicit(c, "short", "void*"), C::EXPLICIT_POINTER);

        assert_eq!(explicit(c, "void*", "int*"), C::EXPLICIT_POINTER);
        assert_eq!(explicit(c, "long*", "byte*"), C::EXPLICIT_POINTER);
    });
}

#[test]
fn conversion_from_dynamic() {
    Fixture::new().run(|c| {
        // Explicit dynamic conversion is for resolve results only; otherwise it's an
        // explicit reference / unboxing conversion.
        assert_eq!(explicit(c, "dynamic", "string"), C::EXPLICIT_REFERENCE);
        assert_eq!(explicit(c, "dynamic", "int"), C::UNBOXING);

        let dynamic = c.ts.intern(TypeData::Dynamic);
        let dynamic_rr = ResolveResult::Expression(dynamic);
        let string = t(c, "string");
        let int = t(c, "int");
        assert_eq!(c.explicit(&dynamic_rr, string), C::EXPLICIT_DYNAMIC);
        assert_eq!(c.explicit(&dynamic_rr, int), C::EXPLICIT_DYNAMIC);
    });
}

#[test]
fn numeric_conversions() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "sbyte", "uint"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "sbyte", "char"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "byte", "char"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "byte", "sbyte"), C::EXPLICIT_NUMERIC);
        // if an implicit conversion exists, the explicit conversion is that one
        assert_eq!(explicit(c, "byte", "int"), C::IMPLICIT_NUMERIC);
        assert_eq!(explicit(c, "double", "float"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "double", "decimal"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "decimal", "double"), C::EXPLICIT_NUMERIC);
        assert_eq!(explicit(c, "int", "decimal"), C::IMPLICIT_NUMERIC);

        assert_eq!(explicit(c, "bool", "int"), C::None);
        assert_eq!(explicit(c, "int", "bool"), C::None);
    });
}

#[test]
fn enumeration_conversions() {
    Fixture::new().run(|c| {
        let e = C::enumeration(false, false);
        assert_eq!(explicit(c, "sbyte", "StringComparison"), e);
        assert_eq!(explicit(c, "char", "StringComparison"), e);
        assert_eq!(explicit(c, "int", "StringComparison"), e);
        assert_eq!(explicit(c, "decimal", "StringComparison"), e);
        assert_eq!(explicit(c, "StringComparison", "char"), e);
        assert_eq!(explicit(c, "StringComparison", "int"), e);
        assert_eq!(explicit(c, "StringComparison", "decimal"), e);
        assert_eq!(explicit(c, "StringComparison", "StringSplitOptions"), e);
    });
}

#[test]
fn nullable_conversion_based_on_identity_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(
            explicit(c, "ArraySegment<dynamic>?", "ArraySegment<object>?"),
            C::IDENTITY
        );
        assert_eq!(
            explicit(c, "ArraySegment<dynamic>", "ArraySegment<object>?"),
            C::IMPLICIT_NULLABLE
        );
        assert_eq!(
            explicit(c, "ArraySegment<dynamic>?", "ArraySegment<object>"),
            C::EXPLICIT_NULLABLE
        );
    });
}

#[test]
fn nullable_conversion_based_on_implicit_numeric_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "int?", "long?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(explicit(c, "int", "long?"), C::IMPLICIT_LIFTED_NUMERIC);
        assert_eq!(explicit(c, "int?", "long"), C::EXPLICIT_LIFTED_NUMERIC);
    });
}

#[test]
fn nullable_conversion_based_on_implicit_enumeration_conversion() {
    Fixture::new().run(|c| {
        let int = c.ts.known_type(KnownTypeCode::Int32);
        let zero = ResolveResult::Constant {
            ty: int,
            value: ConstValue::Int32(0),
        };
        let one = ResolveResult::Constant {
            ty: int,
            value: ConstValue::Int32(1),
        };
        let to = t(c, "StringComparison?");
        assert_eq!(c.explicit(&zero, to), C::enumeration(true, true));
        assert_eq!(c.explicit(&one, to), C::enumeration(false, true));
    });
}

#[test]
fn nullable_conversion_based_on_explicit_numeric_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "int?", "short?"), C::EXPLICIT_LIFTED_NUMERIC);
        assert_eq!(explicit(c, "int", "short?"), C::EXPLICIT_LIFTED_NUMERIC);
        assert_eq!(explicit(c, "int?", "short"), C::EXPLICIT_LIFTED_NUMERIC);
    });
}

#[test]
fn nullable_conversion_based_on_explicit_enumeration_conversion() {
    Fixture::new().run(|c| {
        // explicit lifted enumeration conversion
        let e = C::enumeration(false, true);
        assert_eq!(explicit(c, "int?", "StringComparison?"), e);
        assert_eq!(explicit(c, "int", "StringComparison?"), e);
        assert_eq!(explicit(c, "int?", "StringComparison"), e);

        assert_eq!(explicit(c, "StringComparison?", "int?"), e);
        assert_eq!(explicit(c, "StringComparison", "int?"), e);
        assert_eq!(explicit(c, "StringComparison?", "int"), e);

        assert_eq!(explicit(c, "StringComparison?", "StringSplitOptions?"), e);
        assert_eq!(explicit(c, "StringComparison", "StringSplitOptions?"), e);
        assert_eq!(explicit(c, "StringComparison?", "StringSplitOptions"), e);
    });
}

#[test]
fn explicit_reference_conversion_sealed_class() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "object", "string"), C::EXPLICIT_REFERENCE);
        assert_eq!(
            explicit(c, "IEnumerable<char>", "string"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "IEnumerable<int>", "string"), C::None);
        assert_eq!(explicit(c, "IEnumerable<object>", "string"), C::None);
        assert_eq!(
            explicit(c, "string", "IEnumerable<char>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "string", "IEnumerable<int>"), C::None);
        assert_eq!(explicit(c, "string", "IEnumerable<object>"), C::None);
    });
}

#[test]
fn explicit_reference_conversion_non_sealed_class() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "object", "List<string>"), C::EXPLICIT_REFERENCE);
        assert_eq!(
            explicit(c, "IEnumerable<object>", "List<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<string>", "List<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<int>", "List<string>"),
            C::EXPLICIT_REFERENCE
        );

        assert_eq!(
            explicit(c, "List<string>", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "List<string>", "IEnumerable<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "List<string>", "IEnumerable<int>"),
            C::EXPLICIT_REFERENCE
        );

        assert_eq!(explicit(c, "List<string>", "List<object>"), C::None);
        assert_eq!(explicit(c, "List<string>", "List<int>"), C::None);
    });
}

#[test]
fn explicit_reference_conversion_interfaces() {
    Fixture::new().run(|c| {
        assert_eq!(
            explicit(c, "IEnumerable<string>", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<int>", "IEnumerable<object>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<object>", "IEnumerable<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<object>", "IEnumerable<int>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<object>", "IConvertible"),
            C::EXPLICIT_REFERENCE
        );
    });
}

#[test]
fn explicit_reference_conversion_arrays() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "object[]", "string[]"), C::EXPLICIT_REFERENCE);
        assert_eq!(explicit(c, "dynamic[]", "string[]"), C::EXPLICIT_REFERENCE);
        assert_eq!(explicit(c, "object[]", "object[,]"), C::None);
        assert_eq!(explicit(c, "object[]", "int[]"), C::None);
        assert_eq!(explicit(c, "short[]", "int[]"), C::None);
        assert_eq!(explicit(c, "Array", "int[]"), C::EXPLICIT_REFERENCE);
    });
}

#[test]
fn explicit_reference_conversion_interface_to_array() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "ICloneable", "int[]"), C::EXPLICIT_REFERENCE);
        assert_eq!(
            explicit(c, "IEnumerable<string>", "string[]"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<object>", "string[]"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<string>", "object[]"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<string>", "dynamic[]"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "IEnumerable<int>", "int[]"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "IEnumerable<string>", "object[,]"), C::None);
        assert_eq!(explicit(c, "IEnumerable<short>", "object[]"), C::None);
    });
}

#[test]
fn explicit_reference_conversion_array_to_interface() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "int[]", "ICloneable"), C::IMPLICIT_REFERENCE);
        assert_eq!(
            explicit(c, "string[]", "IEnumerable<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "string[]", "IEnumerable<object>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "object[]", "IEnumerable<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "dynamic[]", "IEnumerable<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "int[]", "IEnumerable<int>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "object[,]", "IEnumerable<string>"), C::None);
        assert_eq!(explicit(c, "object[]", "IEnumerable<short>"), C::None);
    });
}

#[test]
fn explicit_reference_conversion_delegates() {
    Fixture::new().run(|c| {
        assert_eq!(
            explicit(c, "MulticastDelegate", "Action"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "Delegate", "Action"), C::EXPLICIT_REFERENCE);
        assert_eq!(explicit(c, "ICloneable", "Action"), C::EXPLICIT_REFERENCE);
        assert_eq!(
            explicit(c, "System.Threading.ThreadStart", "Action"),
            C::None
        );
    });
}

#[test]
fn explicit_reference_conversion_generic_delegates() {
    Fixture::new().run(|c| {
        assert_eq!(
            explicit(c, "Action<object>", "Action<string>"),
            C::IMPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "Action<string>", "Action<object>"),
            C::EXPLICIT_REFERENCE
        );

        assert_eq!(
            explicit(c, "Func<object>", "Func<string>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(
            explicit(c, "Func<string>", "Func<object>"),
            C::IMPLICIT_REFERENCE
        );

        assert_eq!(
            explicit(c, "Action<IFormattable>", "Action<IConvertible>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "Action<IFormattable>", "Action<int>"), C::None);
        assert_eq!(
            explicit(c, "Action<string>", "Action<IEnumerable<int>>"),
            C::EXPLICIT_REFERENCE
        );

        assert_eq!(
            explicit(c, "Func<IFormattable>", "Func<IConvertible>"),
            C::EXPLICIT_REFERENCE
        );
        assert_eq!(explicit(c, "Func<IFormattable>", "Func<int>"), C::None);
        assert_eq!(
            explicit(c, "Func<string>", "Func<IEnumerable<int>>"),
            C::None
        );
        assert_eq!(
            explicit(c, "Func<string>", "Func<IEnumerable<int>>"),
            C::None
        );
    });
}

#[test]
fn unboxing_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "object", "int"), C::UNBOXING);
        assert_eq!(explicit(c, "object", "decimal"), C::UNBOXING);
        assert_eq!(explicit(c, "ValueType", "int"), C::UNBOXING);
        assert_eq!(explicit(c, "IFormattable", "int"), C::UNBOXING);
        assert_eq!(explicit(c, "IEnumerable<object>", "int"), C::None);
        assert_eq!(explicit(c, "Enum", "StringComparison"), C::UNBOXING);
        assert_eq!(explicit(c, "Enum", "int"), C::None);
    });
}

#[test]
fn lifted_unboxing_conversion() {
    Fixture::new().run(|c| {
        assert_eq!(explicit(c, "object", "int?"), C::UNBOXING);
        assert_eq!(explicit(c, "object", "decimal?"), C::UNBOXING);
        assert_eq!(explicit(c, "ValueType", "int?"), C::UNBOXING);
        assert_eq!(explicit(c, "IFormattable", "int?"), C::UNBOXING);
        assert_eq!(explicit(c, "IEnumerable<object>", "int?"), C::None);
        assert_eq!(explicit(c, "Enum", "StringComparison?"), C::UNBOXING);
        assert_eq!(explicit(c, "Enum", "int?"), C::None);
    });
}

#[test]
fn object_to_type_parameter() {
    Fixture::new().run(|c| {
        // void M<T>(object o) { T t = (T)o; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let object = c.ts.known_type(KnownTypeCode::Object);
        assert_eq!(c.explicit_type(object, tt), C::UNBOXING);
    });
}

#[test]
fn unrelated_class_to_type_parameter() {
    Fixture::new().run(|c| {
        // void M<T>(string o) { T t = (T)o; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let string = c.ts.known_type(KnownTypeCode::String);
        assert_eq!(c.explicit_type(string, tt), C::None);
    });
}

#[test]
fn inteface_to_type_parameter() {
    Fixture::new().run(|c| {
        // void M<T>(IDisposable o) { T t = (T)o; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let disposable = t(c, "IDisposable");
        assert_eq!(c.explicit_type(disposable, tt), C::UNBOXING);
    });
}

#[test]
fn type_parameter_to_interface() {
    Fixture::new().run(|c| {
        // void M<T>(T t) { IDisposable d = (IDisposable)t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let disposable = t(c, "IDisposable");
        assert_eq!(c.explicit_type(tt, disposable), C::BOXING);
    });
}

#[test]
fn value_type_to_type_parameter() {
    Fixture::new().run(|c| {
        // void M<T>(ValueType o) where T : struct { T t = (T)o; }
        let tt = tp(c, 0, "T", false, true, Vec::new());
        let value_type = t(c, "ValueType");
        assert_eq!(c.explicit_type(value_type, tt), C::UNBOXING);
    });
}

#[test]
fn invalid_type_parameter_conversion() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) { U u = (U)t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let u = tp(c, 1, "U", false, false, Vec::new());
        assert_eq!(c.explicit_type(tt, u), C::None);
    });
}

#[test]
fn type_parameter_conversion1() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where T : U { U u = (U)t; }
        let u = tp(c, 1, "U", false, false, Vec::new());
        let tt = tp(c, 0, "T", false, false, vec![u]);
        assert_eq!(c.explicit_type(tt, u), C::BOXING);
    });
}

#[test]
fn type_parameter_conversion1_array() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where T : U { U[] u = (U[])t; }
        let u = tp(c, 1, "U", false, false, Vec::new());
        let tt = tp(c, 0, "T", false, false, vec![u]);
        let (ta, ua) = (array(c, tt), array(c, u));
        assert_eq!(c.explicit_type(ta, ua), C::None);
    });
}

#[test]
fn type_parameter_conversion2() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where U : T { U u = (U)t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let u = tp(c, 1, "U", false, false, vec![tt]);
        assert_eq!(c.explicit_type(tt, u), C::UNBOXING);
    });
}

#[test]
fn type_parameter_conversion2_array() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where U : T { U[] u = (U[])t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let u = tp(c, 1, "U", false, false, vec![tt]);
        let (ta, ua) = (array(c, tt), array(c, u));
        assert_eq!(c.explicit_type(ta, ua), C::None);
    });
}

#[test]
fn implicit_type_parameter_conversion_with_class_constraint() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where T : class where U : class, T { U u = (U)t; }
        let tt = tp(c, 0, "T", true, false, Vec::new());
        let u = tp(c, 1, "U", true, false, vec![tt]);
        assert_eq!(c.explicit_type(tt, u), C::EXPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_array_conversion_with_class_constraint() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where T : class where U : class, T { U[] u = (U[])t; }
        let tt = tp(c, 0, "T", true, false, Vec::new());
        let u = tp(c, 1, "U", true, false, vec![tt]);
        let (ta, ua) = (array(c, tt), array(c, u));
        assert_eq!(c.explicit_type(ta, ua), C::EXPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_conversion_with_class_constraint_only_on_t() {
    Fixture::new().run(|c| {
        // void M<T, U>(T t) where U : class, T { U u = (U)t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let u = tp(c, 1, "U", true, false, vec![tt]);
        assert_eq!(c.explicit_type(tt, u), C::EXPLICIT_REFERENCE);
    });
}

#[test]
fn implicit_type_parameter_array_conversion_with_class_constraint_only_on_t() {
    Fixture::new().run(|c| {
        // void M<T, U>(T[] t) where U : class, T { U[] u = (U[])t; }
        let tt = tp(c, 0, "T", false, false, Vec::new());
        let u = tp(c, 1, "U", true, false, vec![tt]);
        let (ta, ua) = (array(c, tt), array(c, u));
        assert_eq!(c.explicit_type(ta, ua), C::EXPLICIT_REFERENCE);
    });
}

#[test]
fn simple_user_defined_conversion() {
    Fixture::new().run(|c| {
        // C1 c1 = (C1)c2;  with  explicit operator C1(C2 c2)
        let conv = explicit(c, &ud("SimpleSource"), &ud("SimpleTarget"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(conv.method().expect("a method").name, "op_Explicit");
    });
}

#[test]
fn explicit_reference_conversion_followed_by_user_defined_conversion() {
    Fixture::new().run(|c| {
        // class S : B, explicit operator T(S s);
        // T t = (T)b;  with b of type B needs the explicit reference conversion B -> S first
        let conv = explicit(c, &ud("BaseClass"), &ud("TFromDerived"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
    });
}

#[test]
fn implicit_user_defined_conversion_followed_by_explicit_numeric_conversion() {
    Fixture::new().run(|c| {
        // struct T { implicit operator float(T t) }
        // int x = (int)t;
        let conv = explicit(c, &ud("TImplicitToFloat"), "int");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        // even though the user-defined conversion is implicit, the combined conversion is
        // explicit
        assert!(conv.is_explicit());
    });
}

#[test]
fn both_direct_conversion_and_base_class_conversion_available() {
    Fixture::new().run(|c| {
        // class S : B, T with explicit operators from S ("s") and from B ("b");
        // T t = (T)b;  picks the operator from B
        let conv = explicit(c, &ud("BaseClass"), &ud("TFromDerivedOrBase"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        let params = &conv.method().expect("a method").parameters;
        assert_eq!(params.len(), 1);
        assert_eq!(params[0].name, "b");
    });
}

#[test]
fn user_defined_explicit_conversion_picks_exact_source_type_if_possible() {
    Fixture::new().run(|c| {
        // explicit operators from int ("i") and short ("s");
        // (Convertible)33
        let conv = explicit_constant(c, ConstValue::Int32(33), &ud("ExplicitFromIntOrShort"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "i");
    });
}

#[test]
fn user_defined_explicit_conversion_picks_most_encompassed_source_type_if_possible() {
    Fixture::new().run(|c| {
        // explicit operators from long ("l") and uint ("ui");
        // (Convertible)(ushort)33
        let conv = explicit_constant(c, ConstValue::UInt16(33), &ud("ExplicitFromLongOrUInt"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "ui");
    });
}

#[test]
fn user_defined_explicit_conversion_picks_most_encompassing_source_type() {
    Fixture::new().run(|c| {
        // explicit operators from int ("i") and ushort ("us");
        // (Convertible)(long)33
        let conv = explicit_constant(c, ConstValue::Int64(33), &ud("ExplicitFromIntOrUShort"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "i");
    });
}

#[test]
fn user_defined_explicit_conversion_no_most_encompassing_source_type_is_invalid() {
    Fixture::new().run(|c| {
        // explicit operators from uint and short; neither source type encompasses the
        // other, so the conversion from long is ambiguous.
        let conv = explicit_constant(c, ConstValue::Int64(33), &ud("ExplicitFromUIntOrShort"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_explicit_conversion_picks_exact_target_type_if_possible() {
    Fixture::new().run(|c| {
        // explicit operators to int ("i") and short ("s");
        // (int)new Convertible()
        let conv = explicit(c, &ud("ExplicitToIntOrShort"), "int");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "i");
    });
}

#[test]
fn user_defined_explicit_conversion_picks_most_encompassing_target_type_if_possible() {
    Fixture::new().run(|c| {
        // explicit operators to int ("i") and ushort ("us");
        // (ulong)new Convertible()
        let conv = explicit(c, &ud("ExplicitToIntOrUShort"), "ulong");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "us");
    });
}

#[test]
fn user_defined_explicit_conversion_picks_most_encompassed_target_type() {
    Fixture::new().run(|c| {
        // explicit operators to long ("l") and uint ("ui");
        // (ushort)new Convertible()
        let conv = explicit(c, &ud("ExplicitToLongOrUInt"), "ushort");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "ui");
    });
}

#[test]
fn user_defined_explicit_conversion_no_most_encompassed_target_type_is_invalid() {
    Fixture::new().run(|c| {
        // explicit operators to ulong and int; neither target type encompasses the other,
        // so the conversion to ushort is ambiguous.
        let conv = explicit(c, &ud("ExplicitToULongOrInt"), "ushort");
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_explicit_conversion_ambiguous_is_invalid() {
    Fixture::new().run(|c| {
        // Both ExplicitAmbiguousA and ExplicitAmbiguousB declare
        // explicit operator ExplicitAmbiguousB(ExplicitAmbiguousA).
        let conv = explicit(c, &ud("ExplicitAmbiguousA"), &ud("ExplicitAmbiguousB"));
        assert!(!conv.is_valid());
    });
}

#[test]
fn user_defined_explicit_conversion_lifted() {
    Fixture::new().run(|c| {
        // struct Convertible { explicit operator Convertible(int i) }
        // (Convertible?)i  with i of type int?
        let conv = explicit(c, "int?", &ud("ExplicitFromInt?"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(conv.is_lifted());
    });
}

#[test]
fn user_defined_explicit_conversion_followed_by_implicit_nullable_conversion() {
    Fixture::new().run(|c| {
        // struct Convertible { explicit operator Convertible(int i) }
        // (Convertible?)i  with i of type int
        let conv = explicit(c, "int", &ud("ExplicitFromInt?"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(!conv.is_lifted());
    });
}

#[test]
fn user_defined_explicit_conversion_explicit_nullable_then_user_defined() {
    Fixture::new().run(|c| {
        // struct Convertible with explicit operators from int ("i") and from int? ("ni");
        // (Convertible)i  with i of type int? unwraps the nullable and uses the int operator
        let conv = explicit(c, "int?", &ud("ExplicitNullableConvertible"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(!conv.is_lifted());
        assert_eq!(method_param_name(&conv, 0), "i");
    });
}

#[test]
fn user_defined_explicit_conversion_defined_nullable_takes_precedence_over_lifted() {
    Fixture::new().run(|c| {
        // struct Convertible with explicit operators from int ("i") and from int? ("ni");
        // (Convertible?)(int?)33  -- the user-defined nullable operator wins over the
        // lifted form of the int operator.
        let conv = explicit(c, "int?", &ud("ExplicitNullableConvertible?"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(!conv.is_lifted());
        assert_eq!(method_param_name(&conv, 0), "ni");
    });
}

#[test]
fn user_defined_explicit_conversion_uint_constant() {
    Fixture::new().run(|c| {
        // explicit operators from long ("l") and uint ("ui");
        // (Convertible)33  -- the constant 33 converts to uint, which is more specific
        let conv = explicit_constant(c, ConstValue::Int32(33), &ud("ExplicitFromLongOrUInt"));
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "ui");
    });
}

#[test]
fn user_defined_explicit_conversion_nullable_uint_constant() {
    Fixture::new().run(|c| {
        // explicit operators from long? ("l") and uint? ("ui");
        // (Convertible)33
        let conv = explicit_constant(
            c,
            ConstValue::Int32(33),
            &ud("ExplicitFromNullableLongOrNullableUInt"),
        );
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "ui");
    });
}

#[test]
fn use_defined_explicit_conversion_lifted() {
    Fixture::new().run(|c| {
        // The conversion of user_defined_explicit_conversion_lifted, through the
        // ResolveResult-based entry point.
        let from = t(c, "int?");
        let to = t(c, &ud("ExplicitFromInt?"));
        let conv = c.explicit(&ResolveResult::Expression(from), to);
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert!(conv.is_lifted());
    });
}

#[test]
fn user_defined_explicit_conversion_short_or_nullable_byte_target() {
    Fixture::new().run(|c| {
        // explicit operators to short ("s") and byte? ("b");
        // (int?)new Test()
        let conv = explicit(c, &ud("ExplicitToShortOrNullableByte"), "int?");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        let return_type = conv.method().expect("a method").return_type;
        assert_eq!(c.ts.full_name(return_type), "System.Int16");
    });
}

#[test]
fn user_defined_explicit_conversion_byte_or_nullable_short_target() {
    Fixture::new().run(|c| {
        // explicit operators to byte ("b") and short? ("s");
        // (int?)new Test()
        let conv = explicit(c, &ud("ExplicitToByteOrNullableShort"), "int?");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "s");
    });
}

#[test]
fn explicit_conversion_operators_can_override_applicable_implicit_ones() {
    Fixture::new().run(|c| {
        // struct Convertible { explicit operator int(Convertible ci); implicit operator
        // short(Convertible cs); }
        // int i = (int)new Convertible();  -- csc uses the explicit conversion operator
        let conv = explicit(c, &ud("ExplicitIntImplicitShort"), "int");
        assert!(conv.is_valid());
        assert!(conv.is_user_defined());
        assert_eq!(method_param_name(&conv, 0), "ci");
    });
}

#[test]
fn user_defined_explicit_conversion_conversion_before_user_defined_operator_is_correct() {
    Fixture::new().run(|c| {
        // implicit operator Convertible(int l); casting a long goes long -> int ->
        // Convertible, with an explicit numeric conversion before the operator.
        let conv = explicit(c, "long", &ud("ImplicitFromInt"));
        assert!(conv.is_valid());
        let before = conv
            .conversion_before_user_defined_operator()
            .expect("before");
        assert!(before.is_valid());
        assert!(before.is_explicit());
        assert!(before.is_numeric());
        let after = conv
            .conversion_after_user_defined_operator()
            .expect("after");
        assert!(after.is_identity());
    });
}

#[test]
fn user_defined_explicit_conversion_conversion_after_user_defined_operator_is_correct() {
    Fixture::new().run(|c| {
        // implicit operator long(Convertible i); casting to int goes Convertible -> long ->
        // int, with an explicit numeric conversion after the operator.
        let conv = explicit(c, &ud("ImplicitToLong"), "int");
        assert!(conv.is_valid());
        let before = conv
            .conversion_before_user_defined_operator()
            .expect("before");
        assert!(before.is_identity());
        let after = conv
            .conversion_after_user_defined_operator()
            .expect("after");
        assert!(after.is_valid());
        assert!(after.is_explicit());
        assert!(after.is_numeric());
    });
}

#[test]
fn enumeration_conversions_with_floating_point_types() {
    Fixture::new().run(|c| {
        // C# standard 10.3.3: explicit enumeration conversions include float and double
        let e = C::enumeration(false, false);
        assert_eq!(explicit(c, "float", "StringComparison"), e);
        assert_eq!(explicit(c, "double", "StringComparison"), e);
        assert_eq!(explicit(c, "StringComparison", "float"), e);
        assert_eq!(explicit(c, "StringComparison", "double"), e);
    });
}

#[test]
fn explicit_tuple_conversions() {
    Fixture::new().run(|c| {
        // C# standard 10.3.6: explicit tuple conversion with explicit element conversions
        assert_eq!(
            explicit(c, "(long, object)", "(int, string)"),
            C::tuple(vec![C::EXPLICIT_NUMERIC, C::EXPLICIT_REFERENCE])
        );
        // arity mismatch
        assert_eq!(explicit(c, "(long, object)", "ValueTuple<int>"), C::None);
    });
}

#[test]
fn unboxing_conversion_via_variance() {
    Fixture::new().run(|c| {
        // C# standard 10.3.7: unboxing from an interface that is variance-convertible
        // to/from an interface implemented by the value type
        let s = format!("{TS_NAMESPACE}.StructImplementingIEnumerableOfString");
        assert_eq!(explicit(c, "IEnumerable<string>", &s), C::UNBOXING);
        assert_eq!(explicit(c, "IEnumerable<object>", &s), C::UNBOXING);
        assert_eq!(explicit(c, "IEnumerable<int>", &s), C::None);
    });
}

#[test]
#[ignore = "Known bug: CSharpConversions does not support nullable conversions derived from tuple conversions, but csc accepts them"]
fn explicit_lifted_tuple_conversions() {
    Fixture::new().run(|c| {
        // csc accepts the explicit nullable conversions derived from tuple conversions:
        //   (int, string)? tn = ...;
        //   (long, object) c = ((long, object))tn;
        //   (int, string)? d = ((int, string)?)lo;  with lo of type (long, object)?
        assert!(
            explicit(c, "(int, string)?", "(long, object)").is_valid(),
            "(int, string)? -> (long, object)"
        );
        assert!(
            explicit(c, "(long, object)?", "(int, string)?").is_valid(),
            "(long, object)? -> (int, string)?"
        );
    });
}

#[test]
#[ignore = "Known bug: CSharpConversions rejects explicit user-defined conversions to a nullable target when the operator result needs an explicit numeric conversion, but csc accepts them"]
fn user_defined_explicit_conversion_explicit_numeric_conversion_to_nullable_target() {
    Fixture::new().run(|c| {
        // csc accepts:
        //   int? d = (int?)new ImplicitToLong();       -- UD op to long, then explicit long -> int?
        //   int? e = (int?)new ExplicitToLongOrUInt(); -- UD ops to long/uint, then explicit -> int?
        let conv = explicit(c, &ud("ImplicitToLong"), "int?");
        assert!(conv.is_valid(), "(int?)ImplicitToLong");
        let conv2 = explicit(c, &ud("ExplicitToLongOrUInt"), "int?");
        assert!(conv2.is_valid(), "(int?)ExplicitToLongOrUInt");
    });
}

#[test]
fn explicit_type_parameter_conversion_from_effective_base_class() {
    Fixture::new().run(|c| {
        // C# standard 10.3.8: for T known to be a reference type, explicit reference
        // conversions exist from the effective base class (and its base classes) to T,
        // and from T to interfaces it does not implement
        let comparer = t(c, "StringComparer");
        let tt = tp(c, 0, "T", false, false, vec![comparer]);
        assert_eq!(c.explicit_type(comparer, tt), C::EXPLICIT_REFERENCE);
        let object = c.ts.known_type(KnownTypeCode::Object);
        assert_eq!(c.explicit_type(object, tt), C::EXPLICIT_REFERENCE);
        let convertible = t(c, "IConvertible");
        assert_eq!(c.explicit_type(tt, convertible), C::EXPLICIT_REFERENCE);
    });
}

// ---- First-class span conversions ----

#[test]
fn explicit_span_conversion_covariant_array_to_span() {
    Fixture::span().run(|c| {
        // C# 14 first-class spans: an explicit span conversion exists from an array to
        // Span<U>/ReadOnlySpan<U> when an explicit reference conversion relates the element
        // types, and user-defined operators are not considered for such pairs. Roslyn
        // compiles '(Span<string>)objectArray', and reports CS0266 (explicit conversion
        // exists) for 'Span<object> s = stringArray;'.
        let downcast = explicit(c, "object[]", "Span<string>");
        assert!(downcast.is_valid());
        assert!(!downcast.is_user_defined());

        let downcast_ros = explicit(c, "object[]", "ReadOnlySpan<string>");
        assert!(downcast_ros.is_valid());
        assert!(!downcast_ros.is_user_defined());

        let upcast = explicit(c, "string[]", "Span<object>");
        assert!(upcast.is_valid());
        assert!(!upcast.is_user_defined());
    });
}

#[test]
fn no_explicit_span_conversion_without_element_reference_conversion() {
    Fixture::span().run(|c| {
        // Roslyn: CS0030 - int[] and Span<long>/ReadOnlySpan<long> are unrelated; the
        // user-defined operator route (op_Implicit(long[])) must not resurrect the cast.
        assert!(!explicit(c, "int[]", "Span<long>").is_valid());
        assert!(!explicit(c, "int[]", "ReadOnlySpan<long>").is_valid());
    });
}
