//! `ICSharpCode.Decompiler.Tests/Semantics/OverloadResolutionTests.cs`.
//!
//! `MakeMethod(typeof(A), 1)` is `method(c, &["A"], &["int"])`: by-value parameters of the
//! given types followed by optional parameters of the given types (the managed default
//! values only contribute their type). `Is.SameAs(m)` compares `Method`s, which are equal
//! when they are the same definition or fake under the same substitution.

mod common;

use std::rc::Rc;

use common::csharp::{compilation, definition_methods, parameter, ref_assembly_compilation, ty_in};
use ilspy_decompiler::semantics::{
    Conversion, Conversions, Lambda, Method, OverloadResolution as OR,
    OverloadResolutionErrors as E, Parameter, ResolveResult,
};
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::members::{ConstValue, RefKind};
use ilspy_decompiler::typesystem::types::{TypeData, TypeId, TypeSystem};

const OUTER: &[&str] = &["OverloadResolutionTests"];

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

fn t(c: &mut Conversions<'_, '_, '_>, s: &str) -> TypeId {
    ty_in(c.ts, s, OUTER)
}

/// `MakeArgumentList(typeof(A), ...)`.
fn args(c: &mut Conversions<'_, '_, '_>, types: &[&str]) -> Vec<ResolveResult> {
    types
        .iter()
        .map(|s| ResolveResult::Expression(t(c, s)))
        .collect()
}

fn fake(c: &mut Conversions<'_, '_, '_>, parameters: Vec<Parameter>) -> Method {
    let void = c.ts.known_type(KnownTypeCode::Void);
    Method::fake("Method", parameters, void)
}

/// `MakeMethod(...)`: required parameters of the types `required`, then optional ones of the
/// types `optional`.
fn method(c: &mut Conversions<'_, '_, '_>, required: &[&str], optional: &[&str]) -> Method {
    let mut parameters = Vec::new();
    for s in required {
        parameters.push(parameter(t(c, s), RefKind::None, false, false));
    }
    for s in optional {
        parameters.push(parameter(t(c, s), RefKind::None, false, true));
    }
    fake(c, parameters)
}

/// `MakeParamsMethod(...)`: the last parameter becomes a non-optional `params` parameter.
fn params_method(c: &mut Conversions<'_, '_, '_>, types: &[&str]) -> Method {
    let mut parameters: Vec<Parameter> = types
        .iter()
        .map(|s| parameter(t(c, s), RefKind::None, false, false))
        .collect();
    let last = parameters.last_mut().expect("a params parameter");
    last.is_params = true;
    fake(c, parameters)
}

/// `MakeByRefMethodIn(c, typeof(T), kind)`.
fn by_ref_method(c: &mut Conversions<'_, '_, '_>, ty: &str, kind: RefKind) -> Method {
    let element = t(c, ty);
    let by_ref = c.ts.intern(TypeData::ByRef(element));
    fake(c, vec![parameter(by_ref, kind, false, false)])
}

/// `MakeInMethodIn(c, typeof(T))`.
fn in_method(c: &mut Conversions<'_, '_, '_>, ty: &str) -> Method {
    by_ref_method(c, ty, RefKind::In)
}

/// A lambda of the form `() => default(returnType)`.
struct MockLambda {
    inferred_return_type: TypeId,
    parameters: Vec<Parameter>,
}

impl MockLambda {
    fn resolve_result(inferred_return_type: TypeId) -> ResolveResult {
        Self::with_parameters(inferred_return_type, Vec::new())
    }

    fn with_parameters(inferred_return_type: TypeId, parameters: Vec<Parameter>) -> ResolveResult {
        ResolveResult::Lambda(Rc::new(MockLambda {
            inferred_return_type,
            parameters,
        }))
    }
}

impl Lambda for MockLambda {
    fn has_parameter_list(&self) -> bool {
        true
    }

    fn is_anonymous_method(&self) -> bool {
        false
    }

    fn is_implicitly_typed(&self) -> bool {
        false
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
        self.inferred_return_type
    }

    fn is_valid(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        _parameter_types: &[TypeId],
        return_type: TypeId,
    ) -> Conversion {
        conversions.implicit_type(self.inferred_return_type, return_type)
    }
}

#[test]
fn prefer_int_over_uint() {
    Fixture::new().run(|c| {
        let a = args(c, &["ushort"]);
        let mut r = OR::new(a, None, Vec::new());
        let c1 = method(c, &["int"], &[]);
        assert_eq!(r.add_candidate(c, c1.clone()), E::NONE);
        let m = method(c, &["uint"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(!r.is_ambiguous());
        assert_eq!(r.best_candidate(), Some(&c1));
    });
}

#[test]
fn prefer_uint_over_long_from_int_literal() {
    Fixture::new().run(|c| {
        let int = c.ts.known_type(KnownTypeCode::Int32);
        let a = vec![ResolveResult::Constant {
            ty: int,
            value: ConstValue::Int32(1),
        }];
        let mut r = OR::new(a, None, Vec::new());
        let c1 = method(c, &["uint"], &[]);
        assert_eq!(r.add_candidate(c, c1.clone()), E::NONE);
        let m = method(c, &["long"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(!r.is_ambiguous());
        assert_eq!(r.best_candidate(), Some(&c1));
    });
}

#[test]
fn nullable_int_and_nullable_uint_is_ambiguous() {
    Fixture::new().run(|c| {
        let a = args(c, &["ushort?"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = method(c, &["int?"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        let m = method(c, &["uint?"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert_eq!(r.best_candidate_errors(c), E::AMBIGUOUS_MATCH);

        // then adding a matching overload solves the ambiguity:
        let m = method(c, &["ushort?"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert_eq!(r.best_candidate_errors(c), E::NONE);
        assert!(r.best_candidate_ambiguous_with().is_none());
    });
}

#[test]
fn params_method_matches_empty_argument_list() {
    Fixture::new().run(|c| {
        let mut r = OR::new(Vec::new(), None, Vec::new());
        let m = params_method(c, &["int[]"]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(r.best_candidate_is_expanded_form());
    });
}

#[test]
fn params_method_matches_one_argument_in_expanded_form() {
    Fixture::new().run(|c| {
        let a = args(c, &["int"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = params_method(c, &["int[]"]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(r.best_candidate_is_expanded_form());
    });
}

#[test]
fn params_method_matches_in_unexpanded_form() {
    Fixture::new().run(|c| {
        let a = args(c, &["int[]"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = params_method(c, &["int[]"]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(!r.best_candidate_is_expanded_form());
    });
}

#[test]
fn less_arguments_passed_to_params_is_better() {
    Fixture::new().run(|c| {
        let a = args(c, &["int", "int", "int"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = params_method(c, &["int[]"]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        let m = params_method(c, &["int", "int[]"]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(!r.is_ambiguous());
        assert_eq!(
            r.best_candidate()
                .expect("a best candidate")
                .parameters
                .len(),
            2
        );
    });
}

#[test]
fn call_invalid_params_declaration() {
    Fixture::new().run(|c| {
        let a = args(c, &["int[,]"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = params_method(c, &["int"]);
        assert_eq!(r.add_candidate(c, m), E::ARGUMENT_TYPE_MISMATCH);
        assert!(!r.best_candidate_is_expanded_form());
    });
}

#[test]
fn prefer_method_without_optional_parameters() {
    Fixture::new().run(|c| {
        let m1 = method(c, &[], &[]);
        let m2 = method(c, &[], &["int"]);

        let mut r = OR::new(Vec::new(), None, Vec::new());
        assert_eq!(r.add_candidate(c, m1.clone()), E::NONE);
        assert_eq!(r.add_candidate(c, m2), E::NONE);
        assert!(!r.is_ambiguous());
        assert_eq!(r.best_candidate(), Some(&m1));
    });
}

#[test]
fn skeet_evil_overload_resolution() {
    // http://msmvps.com/blogs/jon_skeet/archive/2010/11/02/evil-code-overload-resolution-workaround.aspx
    Fixture::new().run(|c| {
        let container = t(c, "SkeetEvilOverloadResolutionTestCase");
        let foo = definition_methods(c.ts, container, "Foo");
        let resolved_m1 = foo[0].clone();
        let resolved_m2 = foo[1].clone();
        let resolved_m3 = foo[2].clone();

        // Call: Foo<int>();
        let int = t(c, "int");
        let mut o = OR::new(Vec::new(), None, vec![int]);
        assert_eq!(o.add_candidate(c, resolved_m1.clone()), E::NONE);
        assert_eq!(
            o.add_candidate(c, resolved_m2.clone()),
            E::CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT
        );
        assert_eq!(o.best_candidate(), Some(&resolved_m1));

        // Call: Foo<string>();
        let string = t(c, "string");
        let mut o = OR::new(Vec::new(), None, vec![string]);
        assert_eq!(
            o.add_candidate(c, resolved_m1.clone()),
            E::CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT
        );
        assert_eq!(o.add_candidate(c, resolved_m2.clone()), E::NONE);
        assert_eq!(o.best_candidate(), Some(&resolved_m2));

        // Call: Foo<int?>();
        let nullable_int = t(c, "int?");
        let mut o = OR::new(Vec::new(), None, vec![nullable_int]);
        assert_eq!(
            o.add_candidate(c, resolved_m1),
            E::CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT
        );
        assert_eq!(
            o.add_candidate(c, resolved_m2),
            E::CONSTRUCTED_TYPE_DOES_NOT_SATISFY_CONSTRAINT
        );
        assert_eq!(o.add_candidate(c, resolved_m3.clone()), E::NONE);
        assert_eq!(o.best_candidate(), Some(&resolved_m3));
    });
}

#[test]
fn better_conversion_by_lambda_return_value() {
    Fixture::new().run(|c| {
        let m1 = method(c, &["Func<long>"], &[]);
        let m2 = method(c, &["Func<int>"], &[]);

        // M(() => default(byte));
        let byte = c.ts.known_type(KnownTypeCode::Byte);
        let a = vec![MockLambda::resolve_result(byte)];

        let mut r = OR::new(a, None, Vec::new());
        assert_eq!(r.add_candidate(c, m1), E::NONE);
        assert_eq!(r.add_candidate(c, m2.clone()), E::NONE);
        assert_eq!(r.best_candidate(), Some(&m2));
        assert_eq!(r.best_candidate_errors(c), E::NONE);
    });
}

#[test]
fn better_conversion_by_lambda_return_value_expression_tree() {
    Fixture::new().run(|c| {
        let m1 = method(c, &["Func<long>"], &[]);
        let m2 = method(c, &["Expression<Func<int>>"], &[]);

        // M(() => default(byte));
        let byte = c.ts.known_type(KnownTypeCode::Byte);
        let a = vec![MockLambda::resolve_result(byte)];

        let mut r = OR::new(a, None, Vec::new());
        assert_eq!(r.add_candidate(c, m1), E::NONE);
        assert_eq!(r.add_candidate(c, m2.clone()), E::NONE);
        assert_eq!(r.best_candidate(), Some(&m2));
        assert_eq!(r.best_candidate_errors(c), E::NONE);
    });
}

#[test]
fn lambda_delegate_and_expression_tree_overloads_are_ambiguous() {
    Fixture::new().run(|c| {
        let m1 = method(c, &["Func<int>"], &[]);
        let m2 = method(c, &["Expression<Func<int>>"], &[]);

        // M(() => default(int));
        let int = c.ts.known_type(KnownTypeCode::Int32);
        let a = vec![MockLambda::resolve_result(int)];

        let mut r = OR::new(a, None, Vec::new());
        assert_eq!(r.add_candidate(c, m1), E::NONE);
        assert_eq!(r.add_candidate(c, m2), E::NONE);
        assert_eq!(r.best_candidate_errors(c), E::AMBIGUOUS_MATCH);
    });
}

#[test]
#[ignore = "Overload Resolution bug"]
fn better_function_member_is_not_transitive() {
    Fixture::new().run(|c| {
        let container = t(c, "BetterFunctionMemberIsNotTransitiveTestCase");

        let string = c.ts.known_type(KnownTypeCode::String);
        let unknown = c.ts.intern(TypeData::UnknownSpecial);
        let mut arg = Parameter::of_type(unknown);
        arg.name = "arg".to_owned();
        let a = vec![MockLambda::with_parameters(string, vec![arg])];

        let mut r = OR::new(a, None, Vec::new());
        let methods = definition_methods(c.ts, container, "Method");
        for m in &methods {
            assert_eq!(r.add_candidate(c, m.clone()), E::NONE);
        }

        assert_eq!(r.best_candidate(), methods.last());
    });
}

// First-class span betterness

#[test]
fn read_only_span_overloads_with_unrelated_element_types_are_ambiguous() {
    // C# 14 spec, 12.6.4.7: ReadOnlySpan<E1> is a better conversion target than
    // ReadOnlySpan<E2> only if an implicit conversion exists from ReadOnlySpan<E1>
    // to ReadOnlySpan<E2> - the span types, not the element types. No span conversion
    // relates ReadOnlySpan<int> and ReadOnlySpan<long>, so neither target is better
    // and the call is ambiguous; Roslyn reports CS0121.
    Fixture::ref_assembly().run(|c| {
        let a = args(c, &["ConvertibleToBothReadOnlySpans"]);
        let mut r = OR::new(a, None, Vec::new());
        let m = method(c, &["ReadOnlySpan<int>"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        let m = method(c, &["ReadOnlySpan<long>"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(r.is_ambiguous());
    });
}

#[test]
fn in_read_only_span_parameter_binds_an_array_without_in_but_not_with_in() {
    // Roslyn: OnlyIn(arr) compiles - a value argument may bind to an 'in' parameter
    // through the implicit span conversion (a temporary is created). OnlyIn(in arr)
    // is CS1503: an explicit 'in' argument must have the parameter's own type.
    Fixture::ref_assembly().run(|c| {
        let array = t(c, "int[]");

        let mut implicit_in = OR::new(vec![ResolveResult::Expression(array)], None, Vec::new());
        let m = in_method(c, "ReadOnlySpan<int>");
        assert_eq!(implicit_in.add_candidate(c, m), E::NONE);

        let by_ref = ResolveResult::by_reference(c.ts, array, RefKind::In);
        let mut explicit_in = OR::new(vec![by_ref], None, Vec::new());
        let m = in_method(c, "ReadOnlySpan<int>");
        assert_ne!(explicit_in.add_candidate(c, m), E::NONE);
    });
}

#[test]
fn in_read_only_span_parameter_binds_an_identity_argument_with_and_without_in() {
    Fixture::ref_assembly().run(|c| {
        let ros = t(c, "ReadOnlySpan<int>");

        let mut implicit_in = OR::new(vec![ResolveResult::Expression(ros)], None, Vec::new());
        let m = in_method(c, "ReadOnlySpan<int>");
        assert_eq!(implicit_in.add_candidate(c, m), E::NONE);

        let by_ref = ResolveResult::by_reference(c.ts, ros, RefKind::In);
        let mut explicit_in = OR::new(vec![by_ref], None, Vec::new());
        let m = in_method(c, "ReadOnlySpan<int>");
        assert_eq!(explicit_in.add_candidate(c, m), E::NONE);
    });
}

#[test]
fn by_value_overload_preferred_over_in_overload_without_in_at_the_call() {
    // Roslyn: for F(ReadOnlySpan<int>) vs F(in ReadOnlySpan<int>), a call without 'in'
    // picks the by-value overload - both for an identity argument and through the
    // span conversion from int[].
    Fixture::ref_assembly().run(|c| {
        for arg in ["ReadOnlySpan<int>", "int[]"] {
            let a = args(c, &[arg]);
            let mut r = OR::new(a, None, Vec::new());
            let by_value = method(c, &["ReadOnlySpan<int>"], &[]);
            assert_eq!(r.add_candidate(c, by_value.clone()), E::NONE);
            let m = in_method(c, "ReadOnlySpan<int>");
            assert_eq!(r.add_candidate(c, m), E::NONE);
            assert!(!r.is_ambiguous(), "argument {arg}");
            assert_eq!(r.best_candidate(), Some(&by_value), "argument {arg}");
        }
    });
}

#[test]
fn ref_and_out_parameters_never_bind_through_a_span_conversion() {
    // Roslyn: CS1503 for both 'M(ref arr)' against 'ref ReadOnlySpan<int>' and
    // 'M(out arr)' against 'out ReadOnlySpan<int>' - ref and out demand the
    // parameter's own type; the span conversion does not apply. A value argument
    // without the keyword is a passing-mode mismatch regardless of conversions.
    Fixture::ref_assembly().run(|c| {
        let array = t(c, "int[]");
        for kind in [RefKind::Ref, RefKind::Out] {
            let by_ref = ResolveResult::by_reference(c.ts, array, kind);
            let mut by_ref_argument = OR::new(vec![by_ref], None, Vec::new());
            let m = by_ref_method(c, "ReadOnlySpan<int>", kind);
            assert_ne!(by_ref_argument.add_candidate(c, m), E::NONE, "{kind:?}");

            let mut value_argument =
                OR::new(vec![ResolveResult::Expression(array)], None, Vec::new());
            let m = by_ref_method(c, "ReadOnlySpan<int>", kind);
            assert_ne!(value_argument.add_candidate(c, m), E::NONE, "{kind:?}");
        }
    });
}

#[test]
fn in_overload_is_the_only_candidate_with_in_at_the_call() {
    // Roslyn: F(in ros) picks the in-overload; the by-value overload cannot take an
    // 'in' argument.
    Fixture::ref_assembly().run(|c| {
        let ros = t(c, "ReadOnlySpan<int>");
        let by_ref = ResolveResult::by_reference(c.ts, ros, RefKind::In);
        let mut r = OR::new(vec![by_ref], None, Vec::new());
        let m = method(c, &["ReadOnlySpan<int>"], &[]);
        assert_ne!(r.add_candidate(c, m), E::NONE);
        let in_overload = in_method(c, "ReadOnlySpan<int>");
        assert_eq!(r.add_candidate(c, in_overload.clone()), E::NONE);
        assert_eq!(r.best_candidate(), Some(&in_overload));
    });
}

#[test]
fn read_only_span_of_string_preferred_over_read_only_span_of_object() {
    // The positive direction of the same rule: string[] converts to both targets, and
    // the covariant span conversion ReadOnlySpan<string> -> ReadOnlySpan<object>
    // exists, so ReadOnlySpan<string> is the better target.
    Fixture::ref_assembly().run(|c| {
        let a = args(c, &["string[]"]);
        let mut r = OR::new(a, None, Vec::new());
        let better = method(c, &["ReadOnlySpan<string>"], &[]);
        assert_eq!(r.add_candidate(c, better.clone()), E::NONE);
        let m = method(c, &["ReadOnlySpan<object>"], &[]);
        assert_eq!(r.add_candidate(c, m), E::NONE);
        assert!(!r.is_ambiguous());
        assert_eq!(r.best_candidate(), Some(&better));
    });
}

/// `ValidateConstraints` for `Nullable<U>` where `U : struct`: a type parameter with a
/// `struct` or `new()` constraint has the dummy parameterless constructor
/// `AbstractTypeParameter.GetConstructors` yields, so it satisfies `Nullable<T>`'s constraints.
#[test]
fn struct_constrained_type_parameter_satisfies_nullable_constraints() {
    let f = Fixture::new();
    f.run(|c| {
        let nullable = c.ts.known_type(KnownTypeCode::NullableOfT);
        let t = c.ts.type_arguments(nullable)[0];
        let u = common::csharp::type_parameter(c.ts, true, 0, "U", false, true, true, Vec::new());
        assert!(
            ilspy_decompiler::semantics::overload_resolution::validate_constraints(
                c, t, u, None, None
            )
        );
    });
}
