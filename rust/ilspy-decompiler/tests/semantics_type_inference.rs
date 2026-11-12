//! `ICSharpCode.Decompiler.Tests/Semantics/TypeInferenceTests.cs`.
//!
//! `compilation.FindType(typeof(X))` is `t(ts, "X")` (the fixture types nest in
//! `TypeInferenceTests`), an open generic definition `typeof(X<>)` is `def(ts, "X`1")` by
//! reflection name, and `new ParameterizedType(def, args)` is `inst(ts, def, args)`.
//! `FindTypeInBounds` with `ImprovedReturnAllResults` returns an `IntersectionType` of all
//! candidates; `find_types_in_bounds` returns the candidates themselves.

mod common;

use std::rc::Rc;

use common::csharp::{
    compilation, method_group, method_type_parameter, ref_assembly_compilation, tuple, ty_in,
    type_parameter,
};
use ilspy_decompiler::metadata::tables::TableId;
use ilspy_decompiler::semantics::{
    Conversion, Conversions, Lambda, Parameter, ResolveResult, TypeInference,
    TypeInferenceAlgorithm,
};
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::fnptr::FnPtrType;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::members::RefKind;
use ilspy_decompiler::typesystem::types::{Nullability, TypeData, TypeId, TypeSystem};

const OUTER: &[&str] = &["TypeInferenceTests"];

type Ti<'x, 't, 'c, 'a> = TypeInference<'x, 't, 'c, 'a>;

fn run_in<R>(comp: &Compilation<'static>, f: impl FnOnce(&mut Ti<'_, '_, '_, '_>) -> R) -> R {
    let mut ts = TypeSystem::new(comp);
    let mut conversions = Conversions::new(&mut ts);
    let mut ti = TypeInference::new(&mut conversions);
    f(&mut ti)
}

/// A fresh `TypeInference` over `new SimpleCompilation(TestAssembly, Mscorlib, SystemCore)`.
fn run<R>(f: impl FnOnce(&mut Ti<'_, '_, '_, '_>) -> R) -> R {
    run_in(&compilation(), f)
}

/// A fresh `TypeInference` over `RefAssemblyCompilation.Instance`.
fn run_ref<R>(f: impl FnOnce(&mut Ti<'_, '_, '_, '_>) -> R) -> R {
    run_in(&ref_assembly_compilation(), f)
}

fn t(ts: &mut TypeSystem<'_, '_>, s: &str) -> TypeId {
    ty_in(ts, s, OUTER)
}

fn known(ts: &mut TypeSystem<'_, '_>, code: KnownTypeCode) -> TypeId {
    ts.known_type(code)
}

/// The open generic type definition with this reflection name (relative names are nested in
/// `TypeInferenceTests`).
fn def(ts: &mut TypeSystem<'_, '_>, name: &str) -> TypeId {
    let full = if name.contains('.') {
        name.to_owned()
    } else {
        format!("ICSharpCode.Decompiler.Tests.Semantics.TypeInferenceTests+{name}")
    };
    let d = ts
        .parse_reflection_name(&full)
        .unwrap_or_else(|_| panic!("{full} not found"));
    assert!(ts.definition(d).is_some(), "{full} not found");
    d
}

fn inst(ts: &mut TypeSystem<'_, '_>, generic: TypeId, args: &[TypeId]) -> TypeId {
    ts.intern(TypeData::Inst {
        generic,
        args: args.into(),
    })
}

fn array_with(ts: &mut TypeSystem<'_, '_>, elem: TypeId, nullability: Nullability) -> TypeId {
    ts.intern(TypeData::Array {
        elem,
        rank: 1,
        nullability,
    })
}

fn array(ts: &mut TypeSystem<'_, '_>, elem: TypeId) -> TypeId {
    array_with(ts, elem, Nullability::Oblivious)
}

fn by_ref_type(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> TypeId {
    ts.intern(TypeData::ByRef(t))
}

fn unknown(ts: &mut TypeSystem<'_, '_>) -> TypeId {
    ts.intern(TypeData::UnknownSpecial)
}

fn expr(t: TypeId) -> ResolveResult {
    ResolveResult::Expression(t)
}

/// `new ByReferenceResolveResult(new ResolveResult(t), ReferenceKind.Ref)`.
fn ref_arg(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> ResolveResult {
    ResolveResult::by_reference(ts, t, RefKind::Ref)
}

fn mtp(ts: &mut TypeSystem<'_, '_>, index: u32, name: &str) -> TypeId {
    method_type_parameter(ts, index, name)
}

fn infer(
    ti: &mut Ti<'_, '_, '_, '_>,
    tps: &[TypeId],
    args: &[ResolveResult],
    param_types: &[TypeId],
) -> (Vec<TypeId>, bool) {
    ti.infer_type_arguments(tps, args, param_types, None)
}

// ---- Type Inference ----

#[test]
fn array_to_enumerable() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let enumerable = known(ts, KnownTypeCode::IEnumerableOfT);
        let arg = array(ts, string);
        let param = inst(ts, enumerable, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn array_to_read_only_list() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let list = known(ts, KnownTypeCode::IReadOnlyListOfT);
        let arg = array(ts, string);
        let param = inst(ts, list, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn enumerable_to_array_in_contravariant_type() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let enumerable = def(ts, "System.Collections.Generic.IEnumerable`1");
        let comparer = def(ts, "System.Collections.Generic.IComparer`1");
        let enumerable_of_string = inst(ts, enumerable, &[string]);
        let comparer_of_ienumerable_of_string = inst(ts, comparer, &[enumerable_of_string]);
        let tp_array = array(ts, tp);
        let comparer_of_tp_array = inst(ts, comparer, &[tp_array]);
        let (r, success) = infer(
            ti,
            &[tp],
            &[expr(comparer_of_ienumerable_of_string)],
            &[comparer_of_tp_array],
        );
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn infer_from_object_and_from_null_literal() {
    run(|ti| {
        // M<T>(T a, T b);
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let object = known(ts, KnownTypeCode::Object);
        let null = ts.intern(TypeData::Null);
        // M(new object(), null);
        let (r, success) = infer(ti, &[tp], &[expr(object), expr(null)], &[tp, tp]);
        assert_eq!(r, vec![object]);
        assert!(success);
    });
}

#[test]
fn array_to_list_with_array_covariance() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let object = known(ts, KnownTypeCode::Object);
        let string = known(ts, KnownTypeCode::String);
        let list = known(ts, KnownTypeCode::IListOfT);
        // void M<T>(IList<T> a, T b);
        // M(new string[0], new object());
        let arg = array(ts, string);
        let param = inst(ts, list, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg), expr(object)], &[param, tp]);
        assert_eq!(r, vec![object]);
        assert!(success);
    });
}

#[test]
fn ienumerable_covariance_with_dynamic() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let enumerable = def(ts, "System.Collections.Generic.IEnumerable`1");
        let ienumerable_of_t = inst(ts, enumerable, &[tp]);
        let string = known(ts, KnownTypeCode::String);
        let ienumerable_of_string = inst(ts, enumerable, &[string]);
        let dynamic = ts.intern(TypeData::Dynamic);
        let ienumerable_of_dynamic = inst(ts, enumerable, &[dynamic]);
        // static T M<T>(IEnumerable<T> x, IEnumerable<T> y) {}
        // M(IEnumerable<dynamic>, IEnumerable<string>); -> should infer T=dynamic, no ambiguity
        let (r, success) = infer(
            ti,
            &[tp],
            &[expr(ienumerable_of_dynamic), expr(ienumerable_of_string)],
            &[ienumerable_of_t, ienumerable_of_t],
        );
        assert_eq!(r, vec![dynamic]);
        assert!(success);
    });
}

// ---- Inference with Method Groups ----

#[test]
fn cannot_infer_from_method_parameter_types() {
    run(|ti| {
        // static void M<A, B>(Func<A, B> f) {}
        // M(int.Parse); // type inference fails
        let ts = &mut *ti.conversions.ts;
        let a = mtp(ts, 0, "A");
        let b = mtp(ts, 1, "B");
        let decl = t(ts, "int");
        let argument = method_group(
            ts,
            decl,
            "Parse",
            Some(ResolveResult::Type(decl)),
            Vec::new(),
        );
        let func = def(ts, "System.Func`2");
        let param = inst(ts, func, &[a, b]);
        let (_, success) = infer(ti, &[a, b], &[argument], &[param]);
        assert!(!success);
    });
}

#[test]
fn infer_from_method_return_type() {
    run(|ti| {
        // static void M<T>(Func<T> f) {}
        // M(Console.ReadKey); // type inference produces ConsoleKeyInfo
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let decl = t(ts, "Console");
        let argument = method_group(
            ts,
            decl,
            "ReadKey",
            Some(ResolveResult::Type(decl)),
            Vec::new(),
        );
        let func = def(ts, "System.Func`1");
        let param = inst(ts, func, &[tp]);
        let expected = t(ts, "ConsoleKeyInfo");
        let (r, success) = infer(ti, &[tp], &[argument], &[param]);
        assert_eq!(r, vec![expected]);
        assert!(success);
    });
}

// ---- Inference with Lambda ----

/// An implicitly typed lambda whose body infers to `inferred_return_type` when its
/// parameters have the `expected_parameter_types`.
struct MockImplicitLambda {
    expected_parameter_types: Vec<TypeId>,
    inferred_return_type: TypeId,
    parameters: Vec<Parameter>,
    is_async: bool,
}

impl MockImplicitLambda {
    fn resolve_result(
        ts: &mut TypeSystem<'_, '_>,
        expected_parameter_types: Vec<TypeId>,
        inferred_return_type: TypeId,
        is_async: bool,
    ) -> ResolveResult {
        // UnknownType because this lambda is implicitly typed
        let u = unknown(ts);
        let parameters = expected_parameter_types
            .iter()
            .enumerate()
            .map(|(i, _)| Parameter {
                name: format!("X{i}"),
                ..Parameter::of_type(u)
            })
            .collect();
        ResolveResult::Lambda(Rc::new(MockImplicitLambda {
            expected_parameter_types,
            inferred_return_type,
            parameters,
            is_async,
        }))
    }
}

impl Lambda for MockImplicitLambda {
    fn has_parameter_list(&self) -> bool {
        true
    }
    fn is_anonymous_method(&self) -> bool {
        false
    }
    fn is_implicitly_typed(&self) -> bool {
        true
    }
    fn is_async(&self) -> bool {
        self.is_async
    }
    fn parameters(&self) -> &[Parameter] {
        &self.parameters
    }
    fn inferred_return_type(
        &self,
        _conversions: &mut Conversions<'_, '_, '_>,
        parameter_types: Option<&[TypeId]>,
    ) -> TypeId {
        assert_eq!(
            parameter_types,
            Some(&self.expected_parameter_types[..]),
            "Parameters types passed to MockImplicitLambda ({:?}) => {:?}",
            self.expected_parameter_types,
            self.inferred_return_type
        );
        self.inferred_return_type
    }
    fn is_valid(
        &self,
        conversions: &mut Conversions<'_, '_, '_>,
        parameter_types: &[TypeId],
        return_type: TypeId,
    ) -> Conversion {
        assert_eq!(parameter_types, &self.expected_parameter_types[..]);
        conversions.implicit_type(self.inferred_return_type, return_type)
    }
}

/// An explicitly typed lambda returning `inferred_return_type`.
struct MockExplicitLambda {
    inferred_return_type: TypeId,
    parameters: Vec<Parameter>,
    is_async: bool,
}

impl MockExplicitLambda {
    fn resolve_result(
        parameter_types: &[TypeId],
        inferred_return_type: TypeId,
        is_async: bool,
    ) -> ResolveResult {
        let parameters = parameter_types
            .iter()
            .enumerate()
            .map(|(i, &ty)| Parameter {
                name: format!("X{i}"),
                ..Parameter::of_type(ty)
            })
            .collect();
        ResolveResult::Lambda(Rc::new(MockExplicitLambda {
            inferred_return_type,
            parameters,
            is_async,
        }))
    }
}

impl Lambda for MockExplicitLambda {
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
        self.is_async
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
fn test_lambda_inference() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let x = mtp(ts, 0, "X");
        let y = mtp(ts, 1, "Y");
        let z = mtp(ts, 2, "Z");
        let func = def(ts, "System.Func`2");
        let fxy = inst(ts, func, &[x, y]);
        let fyz = inst(ts, func, &[y, z]);
        let string = known(ts, KnownTypeCode::String);
        let int = known(ts, KnownTypeCode::Int32);
        let single = known(ts, KnownTypeCode::Single);
        // Signature:  M<X,Y,Z>(X x, Func<X,Y> y, Func<Y,Z> z) {}
        // Invocation: M(default(string), s => default(int), t => default(float));
        let arguments = [
            expr(string),
            MockImplicitLambda::resolve_result(ts, vec![string], int, false),
            MockImplicitLambda::resolve_result(ts, vec![int], single, false),
        ];
        let (r, success) = infer(ti, &[x, y, z], &arguments, &[x, fxy, fyz]);
        assert_eq!(r, vec![string, int, single]);
        assert!(success);
    });
}

#[test]
fn convert_all_lambda_inference() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let class_t = type_parameter(ts, false, 0, "T", false, false, false, Vec::new());
        let r_tp = mtp(ts, 0, "R");
        let converter = def(ts, "System.Converter`2");
        let param = inst(ts, converter, &[class_t, r_tp]);
        let string = known(ts, KnownTypeCode::String);
        let int = known(ts, KnownTypeCode::Int32);
        // Signature:  List<T>.ConvertAll<R>(Converter<T, R> converter);
        // Invocation: listOfString.ConvertAll(s => default(int));
        let arguments = [MockImplicitLambda::resolve_result(
            ts,
            vec![string],
            int,
            false,
        )];
        let class_type_arguments = [string];
        let (r, success) =
            ti.infer_type_arguments(&[r_tp], &arguments, &[param], Some(&class_type_arguments));
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

#[test]
fn infer_from_implicit_async_lambda() {
    run(|ti| {
        // Signature:  M<T>(Func<int, Task<T>> f)
        // Invocation: M(async x => x + 1);
        // An async lambda's inferred return type is already wrapped in Task<>,
        // so lower-bound inference of Task<int> against Task<T> yields T = int.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let int = known(ts, KnownTypeCode::Int32);
        let task = def(ts, "System.Threading.Tasks.Task`1");
        let task_of_int = inst(ts, task, &[int]);
        let task_of_t = inst(ts, task, &[tp]);
        let func = def(ts, "System.Func`2");
        let param = inst(ts, func, &[int, task_of_t]);
        let arguments = [MockImplicitLambda::resolve_result(
            ts,
            vec![int],
            task_of_int,
            true,
        )];
        let (r, success) = infer(ti, &[tp], &arguments, &[param]);
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

#[test]
fn infer_from_explicit_async_lambda() {
    run(|ti| {
        // Signature:  M<T>(Func<int, Task<T>> f)
        // Invocation: M(async (int x) => x + 1);
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let int = known(ts, KnownTypeCode::Int32);
        let task = def(ts, "System.Threading.Tasks.Task`1");
        let task_of_int = inst(ts, task, &[int]);
        let task_of_t = inst(ts, task, &[tp]);
        let func = def(ts, "System.Func`2");
        let param = inst(ts, func, &[int, task_of_t]);
        let arguments = [MockExplicitLambda::resolve_result(
            &[int],
            task_of_int,
            true,
        )];
        let (r, success) = infer(ti, &[tp], &arguments, &[param]);
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

#[test]
fn nullable_pick() {
    run(|ti| {
        // Signature:  Pick<T>(T? a, T? b)
        // Invocation: Pick(default(int?), default(long?)); -> infers T = long
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let nullable = known(ts, KnownTypeCode::NullableOfT);
        let nullable_of_t = inst(ts, nullable, &[tp]);
        let a = t(ts, "int?");
        let b = t(ts, "long?");
        let long = known(ts, KnownTypeCode::Int64);
        let (r, success) = infer(
            ti,
            &[tp],
            &[expr(a), expr(b)],
            &[nullable_of_t, nullable_of_t],
        );
        assert_eq!(r, vec![long]);
        assert!(success);
    });
}

#[test]
fn co_contra_pick() {
    run(|ti| {
        // Signature:  Pick<T>(ICo<T> a, IContra<T> b)
        // Invocation: Pick(default(ICo<string>), default(IContra<object>));
        //
        // String and Object are both valid choices; and csc ends up picking object,
        // even though the C# specification says it should pick string:
        // 7.5.2.11 Fixing - both string and object are in the candidate set;
        // string has a conversion to object (the other candidate),
        // object doesn't have that; so string should be chosen as the result.
        //
        // We follow the csc behavior.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let co = def(ts, "ICo`1");
        let contra = def(ts, "IContra`1");
        let a = t(ts, "ICo<string>");
        let b = t(ts, "IContra<object>");
        let p1 = inst(ts, co, &[tp]);
        let p2 = inst(ts, contra, &[tp]);
        let object = known(ts, KnownTypeCode::Object);
        let (r, success) = infer(ti, &[tp], &[expr(a), expr(b)], &[p1, p2]);
        assert_eq!(r, vec![object]);
        assert!(success);
    });
}

/// Bug 9300 - Unknown Resolve Error
#[test]
fn test_bug9300() {
    run(|ti| {
        // Signature:  Foo<T>(T a, IContra<T> b)
        // Invocation: Foo(new ConvertibleToString(), default(IContra<string>));
        // The lower bound ConvertibleToString and the upper bound string can both
        // only be satisfied by string, via the user-defined implicit conversion.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let a = t(ts, "ConvertibleToString");
        let b = t(ts, "IContra<string>");
        let p2 = inst(ts, contra, &[tp]);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[expr(a), expr(b)], &[tp, p2]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn generic_argument_implicitly_convertible_to_and_from_another_type_list() {
    run(|ti| {
        // Signature:  F<K>(IList<K> a, K b)
        // Invocation: F(new List<MyConvertible>(), 1);
        // IList<K> is invariant, so the first argument gives the exact bound
        // MyConvertible; the lower bound int is compatible with it through the
        // user-defined implicit conversion, so inference succeeds.
        let ts = &mut *ti.conversions.ts;
        let k = mtp(ts, 0, "K");
        let list = known(ts, KnownTypeCode::IListOfT);
        let a = t(ts, "List<MyConvertible>");
        let int = known(ts, KnownTypeCode::Int32);
        let p1 = inst(ts, list, &[k]);
        let expected = t(ts, "MyConvertible");
        let (r, success) = infer(ti, &[k], &[expr(a), expr(int)], &[p1, k]);
        assert_eq!(r, vec![expected]);
        assert!(success);
    });
}

#[test]
fn generic_argument_implicitly_convertible_to_and_from_another_type_ienumerable() {
    run(|ti| {
        // Signature:  F<K>(IEnumerable<K> a, K b)
        // Invocation: F(new List<MyConvertible>(), 1);
        // With the covariant IEnumerable<K> there is no exact bound, only the two
        // lower bounds MyConvertible and int. Since both are implicitly convertible
        // to each other, neither candidate is better and inference fails.
        let ts = &mut *ti.conversions.ts;
        let k = mtp(ts, 0, "K");
        let enumerable = known(ts, KnownTypeCode::IEnumerableOfT);
        let a = t(ts, "List<MyConvertible>");
        let int = known(ts, KnownTypeCode::Int32);
        let p1 = inst(ts, enumerable, &[k]);
        let (_, success) = infer(ti, &[k], &[expr(a), expr(int)], &[p1, k]);
        assert!(!success);
    });
}

// ---- Input type inferences (spec 12.6.3.7) ----

#[test]
fn ref_parameter_uses_exact_inference() {
    run(|ti| {
        // Signature:  M<T>(ref List<T> x)
        // Invocation: M(ref listOfString);
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let list = def(ts, "System.Collections.Generic.List`1");
        let list_of_string = t(ts, "List<string>");
        let arg = ref_arg(ts, list_of_string);
        let list_of_t = inst(ts, list, &[tp]);
        let param = by_ref_type(ts, list_of_t);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[arg], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn ref_parameter_does_not_use_lower_bound_inference() {
    run(|ti| {
        // Signature:  M<T>(ref IList<T> x)
        // Invocation: M(ref listOfString); with a List<string> variable
        // A reference parameter requires an exact inference, so the base-type walk
        // of lower-bound inference must not apply and no bound is found for T.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let ilist = known(ts, KnownTypeCode::IListOfT);
        let list_of_string = t(ts, "List<string>");
        let arg = ref_arg(ts, list_of_string);
        let ilist_of_t = inst(ts, ilist, &[tp]);
        let param = by_ref_type(ts, ilist_of_t);
        let (_, success) = infer(ti, &[tp], &[arg], &[param]);
        assert!(!success);
    });
}

#[test]
#[ignore = "Not implemented: a value argument passed to an 'in' parameter must produce a lower-bound inference (spec 12.6.3.7); currently no bound at all is inferred because every by-reference parameter takes the exact-inference path."]
fn in_parameter_with_value_argument_uses_lower_bound_inference() {
    run(|ti| {
        // Signature:  M<T>(in T x)
        // Invocation: M(5); -> rvalue argument, T = int
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let int = known(ts, KnownTypeCode::Int32);
        let param = by_ref_type(ts, tp);
        let (r, success) = infer(ti, &[tp], &[expr(int)], &[param]);
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

// ---- Tuple literal inferences (spec 12.6.3.7) ----

#[test]
#[ignore = "Not implemented: elementwise input type inference from a tuple literal (spec 12.6.3.7); the literal is currently inferred through its tuple type, which makes conflicting exact element bounds instead of elementwise lower-bound inferences."]
fn tuple_literal_input_type_inference() {
    run_ref(|ti| {
        // Signature:  M<T>((T, T) t)
        // Invocation: M((1, 2L)); -> csc infers T = long
        // A tuple literal infers elementwise: a lower-bound inference is made from
        // each element to the corresponding element type, giving the bounds
        // { int, long } and the fixed type long. Treating the literal like a value
        // of type (int, long) would instead produce conflicting exact bounds.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let tuple_of_tt = tuple(ts, &[tp, tp], &[None, None]);
        let int = known(ts, KnownTypeCode::Int32);
        let long = known(ts, KnownTypeCode::Int64);
        let literal = ResolveResult::Tuple {
            ty: tuple(ts, &[int, long], &[None, None]),
            elements: vec![expr(int), expr(long)],
        };
        let (r, success) = infer(ti, &[tp], &[literal], &[tuple_of_tt]);
        assert_eq!(r, vec![long]);
        assert!(success);
    });
}

// ---- Tuple element name merging ----
// The C# standard does not mention tuple element names in type inference;
// csc merges names when bounds differ only by them: names are kept where all
// bounds agree and dropped where they conflict (MergeTupleNames in Roslyn's
// MethodTypeInference.cs).

/// `(int, string)` with these element names.
fn make_tuple_type(ts: &mut TypeSystem<'_, '_>, a: Option<&str>, b: Option<&str>) -> TypeId {
    let int = known(ts, KnownTypeCode::Int32);
    let string = known(ts, KnownTypeCode::String);
    tuple(ts, &[int, string], &[a, b])
}

/// `delegate*<return_type>` with the default calling convention.
fn make_function_pointer_type(ts: &mut TypeSystem<'_, '_>, return_type: TypeId) -> TypeId {
    ts.intern(TypeData::FnPtr(Box::new(FnPtrType {
        calling_convention: 0,
        custom_conventions: Box::new([]),
        ret: return_type,
        ret_ref_readonly: false,
        params: Box::new([]),
        ref_kinds: Box::new([]),
    })))
}

#[test]
fn best_common_type_merges_tuple_element_names() {
    run_ref(|ti| {
        // var m = cond ? (a: 1, b: "x") : (a: 2, c: "y"); -> (int a, string)
        let ts = &mut *ti.conversions.ts;
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let expected = make_tuple_type(ts, Some("a"), None);
        let (r, success) = ti.best_common_type(&[expr(ab), expr(ac)]);
        assert_eq!(r, expected);
        assert!(success);
    });
}

#[test]
fn best_common_type_merges_function_pointer_tuple_element_names() {
    run_ref(|ti| {
        let ts = &mut *ti.conversions.ts;
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let a_ = make_tuple_type(ts, Some("a"), None);
        let fab = make_function_pointer_type(ts, ab);
        let fac = make_function_pointer_type(ts, ac);
        let expected = make_function_pointer_type(ts, a_);
        let (r, success) = ti.best_common_type(&[expr(fab), expr(fac)]);
        assert_eq!(r, expected);
        assert!(success);
    });
}

#[test]
fn fixing_merges_tuple_element_names_of_exact_and_lower_bounds() {
    run_ref(|ti| {
        // Signature:  M<T>(IList<T> x, T y)
        // Invocation: M(listOfAB, valueAC); -> T = (int a, string)
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let list = known(ts, KnownTypeCode::IListOfT);
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let list_of_ab = inst(ts, list, &[ab]);
        let list_of_t = inst(ts, list, &[tp]);
        let expected = make_tuple_type(ts, Some("a"), None);
        let (r, success) = infer(ti, &[tp], &[expr(list_of_ab), expr(ac)], &[list_of_t, tp]);
        assert_eq!(r, vec![expected]);
        assert!(success);
    });
}

/// `M<T>(T x, T y)` called with arguments of these types: the single inferred type.
fn infer_single(ti: &mut Ti<'_, '_, '_, '_>, arg_type1: TypeId, arg_type2: TypeId) -> TypeId {
    let tp = mtp(ti.conversions.ts, 0, "T");
    let mut inference = TypeInference::new(&mut *ti.conversions);
    let (result, success) =
        inference.infer_type_arguments(&[tp], &[expr(arg_type1), expr(arg_type2)], &[tp, tp], None);
    assert!(success);
    assert_eq!(result.len(), 1);
    result[0]
}

#[test]
fn fixing_merges_nested_tuple_element_names() {
    run_ref(|ti| {
        // Signature:  M<T>(T x, T y)
        // Invocation: M(listOfAB, listOfAC);   -> T = IList<(int a, string)>
        //             M(arrayOfAB, arrayOfAC); -> T = (int a, string)[]
        let ts = &mut *ti.conversions.ts;
        let list = known(ts, KnownTypeCode::IListOfT);
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let a_ = make_tuple_type(ts, Some("a"), None);
        let list_ab = inst(ts, list, &[ab]);
        let list_ac = inst(ts, list, &[ac]);
        let list_a_ = inst(ts, list, &[a_]);
        let array_ab = array(ts, ab);
        let array_ac = array(ts, ac);
        let array_a_ = array(ts, a_);
        assert_eq!(infer_single(ti, list_ab, list_ac), list_a_);
        assert_eq!(infer_single(ti, array_ab, array_ac), array_a_);
    });
}

#[test]
fn fixing_merges_tuple_element_names_across_lower_and_upper_bounds() {
    run_ref(|ti| {
        // Signature:  M<T>(T x, Action<T> y)
        // Invocation: M(listOfAB, actionOfListOfAC); -> T = IList<(int a, string)>
        // Action<in T> is contravariant, so the second argument produces an upper bound
        // while the first produces a lower bound.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let list = known(ts, KnownTypeCode::IListOfT);
        let action = def(ts, "System.Action`1");
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let a_ = make_tuple_type(ts, Some("a"), None);
        let list_of_ac = inst(ts, list, &[ac]);
        let list_of_ab = inst(ts, list, &[ab]);
        let action_of_list_of_ac = inst(ts, action, &[list_of_ac]);
        let action_of_t = inst(ts, action, &[tp]);
        let expected = inst(ts, list, &[a_]);
        let (r, success) = infer(
            ti,
            &[tp],
            &[expr(list_of_ab), expr(action_of_list_of_ac)],
            &[tp, action_of_t],
        );
        assert_eq!(r, vec![expected]);
        assert!(success);
    });
}

#[test]
fn fixing_merges_tuple_element_names_through_equal_nullability_annotations() {
    run_ref(|ti| {
        // Signature:  M<T>(T x, T y)
        // Invocation: M(nullableListOfAB, nullableListOfAC); -> T = IList<(int a, string)>?
        //             M(nullableArrayOfAB, nullableArrayOfAC); -> T = (int a, string)[]?
        let ts = &mut *ti.conversions.ts;
        let list = known(ts, KnownTypeCode::IListOfT);
        let nullable_list_of = |ts: &mut TypeSystem<'_, '_>, e: TypeId| {
            let l = inst(ts, list, &[e]);
            ts.change_nullability(l, Nullability::Nullable)
        };
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let a_ = make_tuple_type(ts, Some("a"), None);
        let l_ab = nullable_list_of(ts, ab);
        let l_ac = nullable_list_of(ts, ac);
        let l_a_ = nullable_list_of(ts, a_);
        let arr_ab = array_with(ts, ab, Nullability::Nullable);
        let arr_ac = array_with(ts, ac, Nullability::Nullable);
        let arr_a_ = array_with(ts, a_, Nullability::Nullable);
        assert_eq!(infer_single(ti, l_ab, l_ac), l_a_);
        assert_eq!(infer_single(ti, arr_ab, arr_ac), arr_a_);
    });
}

#[test]
fn fixing_merges_bounds_that_differ_in_nullability() {
    run_ref(|ti| {
        // Signature:  M<T>(T x, T y)
        // Invocation: M(nullableArrayOfString, arrayOfString);
        // Merging nullability in this covariant position should result in T=string[]? (the nullable array type).
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let nullable_array = array_with(ts, string, Nullability::Nullable);
        let plain_array = array(ts, string);
        let (r, success) = infer(
            ti,
            &[tp],
            &[expr(nullable_array), expr(plain_array)],
            &[tp, tp],
        );
        assert!(success);
        assert_eq!(r, vec![nullable_array]);
    });
}

#[test]
fn fixing_merges_tuple_element_names_of_multiple_exact_bounds() {
    run_ref(|ti| {
        // Signature:  M<T>(ref T x, ref T y)
        // Invocation: M(ref ab, ref ac); -> T = (int a, string)
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let ab = make_tuple_type(ts, Some("a"), Some("b"));
        let ac = make_tuple_type(ts, Some("a"), Some("c"));
        let expected = make_tuple_type(ts, Some("a"), None);
        let arg1 = ref_arg(ts, ab);
        let arg2 = ref_arg(ts, ac);
        let ref_t = by_ref_type(ts, tp);
        let (r, success) = infer(ti, &[tp], &[arg1, arg2], &[ref_t, ref_t]);
        assert_eq!(r, vec![expected]);
        assert!(success);
    });
}

// ---- Explicit parameter type inferences (spec 12.6.3.9) ----

#[test]
fn explicit_lambda_parameter_types_give_exact_bounds() {
    run(|ti| {
        // Signature:  M<T>(Func<T, bool> f)
        // Invocation: M((string s) => true);
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let boolean = known(ts, KnownTypeCode::Boolean);
        let func = def(ts, "System.Func`2");
        let param = inst(ts, func, &[tp, boolean]);
        let arguments = [MockExplicitLambda::resolve_result(
            &[string],
            boolean,
            false,
        )];
        let (r, success) = infer(ti, &[tp], &arguments, &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

// ---- Exact inferences (spec 12.6.3.10) ----

#[test]
fn exact_inference_unwraps_nullable() {
    run(|ti| {
        // Signature:  M<T>(ref T? x)
        // Invocation: M(ref nullableInt); -> T = int
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let nullable = known(ts, KnownTypeCode::NullableOfT);
        let nullable_int = t(ts, "int?");
        let arg = ref_arg(ts, nullable_int);
        let nullable_of_t = inst(ts, nullable, &[tp]);
        let param = by_ref_type(ts, nullable_of_t);
        let int = known(ts, KnownTypeCode::Int32);
        let (r, success) = infer(ti, &[tp], &[arg], &[param]);
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

#[test]
fn exact_inference_on_array_elements() {
    run(|ti| {
        // Signature:  M<T>(ref T[] x)
        // Invocation: M(ref stringArray); -> T = string
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let string_array = array(ts, string);
        let arg = ref_arg(ts, string_array);
        let tp_array = array(ts, tp);
        let param = by_ref_type(ts, tp_array);
        let (r, success) = infer(ti, &[tp], &[arg], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

// ---- Lower-bound inferences (spec 12.6.3.11) ----

#[test]
fn array_to_collection() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let collection = known(ts, KnownTypeCode::ICollectionOfT);
        let arg = array(ts, string);
        let param = inst(ts, collection, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn array_to_read_only_collection() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let roc = known(ts, KnownTypeCode::IReadOnlyCollectionOfT);
        let arg = array(ts, string);
        let param = inst(ts, roc, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn lower_bound_inference_requires_unique_base_type() {
    run(|ti| {
        // Signature:  M<T>(IInv<T> x)
        // Invocation: M(new DoubleImpl()); with DoubleImpl : IInv<int>, IInv<string>
        // No inference is made because the implemented IInv<> instantiation is
        // not unique, so T has no bounds and inference fails.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let inv = def(ts, "IInv`1");
        let arg = t(ts, "DoubleImpl");
        let param = inst(ts, inv, &[tp]);
        let (_, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert!(!success);
    });
}

#[test]
fn lower_bound_inference_value_type_element_is_exact() {
    run(|ti| {
        // Signature:  M<T>(IEnumerable<T> a, T b)
        // Invocation: M(intSequence, 2L);
        // Even though IEnumerable<T> is covariant, the element type int is a value
        // type, so an exact inference is made for it. The lower bound long is not
        // implicitly convertible to the exact bound int, so inference fails.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let enumerable = known(ts, KnownTypeCode::IEnumerableOfT);
        let a = t(ts, "IEnumerable<int>");
        let long = known(ts, KnownTypeCode::Int64);
        let p1 = inst(ts, enumerable, &[tp]);
        let (_, success) = infer(ti, &[tp], &[expr(a), expr(long)], &[p1, tp]);
        assert!(!success);
    });
}

// ---- Upper-bound inferences (spec 12.6.3.12) ----

#[test]
fn upper_bound_inference_keeps_direction_for_covariance() {
    run(|ti| {
        // Signature:  M<T>(IContra<ICo<T>> x)
        // Invocation: M(default(IContra<ICo<string>>)); -> T = string
        // The contravariant outer interface turns the element inference into an
        // upper-bound inference from ICo<string> to ICo<T>; the covariant inner
        // interface keeps the upper-bound direction for T.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let co = def(ts, "ICo`1");
        let arg = t(ts, "IContra<ICo<string>>");
        let co_t = inst(ts, co, &[tp]);
        let param = inst(ts, contra, &[co_t]);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn upper_bound_inference_flips_to_lower_bound_for_contravariance() {
    run(|ti| {
        // Signature:  M<T>(IContra<IContra<T>> x)
        // Invocation: M(default(IContra<IContra<string>>)); -> T = string
        // Two levels of contravariance: the upper-bound inference from
        // IContra<string> to IContra<T> flips back to a lower-bound inference
        // from string to T.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let arg = t(ts, "IContra<IContra<string>>");
        let contra_t = inst(ts, contra, &[tp]);
        let param = inst(ts, contra, &[contra_t]);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn upper_bound_inference_on_array_elements() {
    run(|ti| {
        // Signature:  M<T>(IContra<T[]> x)
        // Invocation: M(default(IContra<string[]>)); -> T = string
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let arg = t(ts, "IContra<string[]>");
        let tp_array = array(ts, tp);
        let param = inst(ts, contra, &[tp_array]);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn upper_bound_inference_from_array_interface_to_array() {
    run(|ti| {
        // Signature:  M<T>(IContra<T[]> x)
        // Invocation: M(default(IContra<IEnumerable<string>>)); -> T = string
        // Upper-bound inference from IEnumerable<string> to T[] uses the
        // array-interface rule elementwise.
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let arg = t(ts, "IContra<IEnumerable<string>>");
        let tp_array = array(ts, tp);
        let param = inst(ts, contra, &[tp_array]);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![string]);
        assert!(success);
    });
}

#[test]
fn upper_bound_inference_unwraps_nullable() {
    run(|ti| {
        // Signature:  M<T>(IContra<T?> x)
        // Invocation: M(default(IContra<int?>)); -> T = int
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let contra = def(ts, "IContra`1");
        let nullable = known(ts, KnownTypeCode::NullableOfT);
        let arg = t(ts, "IContra<int?>");
        let nullable_of_t = inst(ts, nullable, &[tp]);
        let param = inst(ts, contra, &[nullable_of_t]);
        let int = known(ts, KnownTypeCode::Int32);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert_eq!(r, vec![int]);
        assert!(success);
    });
}

// ---- Fixing (spec 12.6.3.13) ----

#[test]
fn fixing_fails_on_conflicting_exact_bounds() {
    run(|ti| {
        // Signature:  M<T>(ref List<T> a, ref List<T> b)
        // Invocation: M(ref listOfString, ref listOfObject);
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let list = def(ts, "System.Collections.Generic.List`1");
        let list_of_t = inst(ts, list, &[tp]);
        let ref_list_of_t = by_ref_type(ts, list_of_t);
        let los = t(ts, "List<string>");
        let loo = t(ts, "List<object>");
        let a1 = ref_arg(ts, los);
        let a2 = ref_arg(ts, loo);
        let (_, success) = infer(ti, &[tp], &[a1, a2], &[ref_list_of_t, ref_list_of_t]);
        assert!(!success);
    });
}

// ---- Best common type (spec 12.6.3.17) ----

fn best_common_type(ti: &mut Ti<'_, '_, '_, '_>, types: &[TypeId]) -> (TypeId, bool) {
    let expressions: Vec<ResolveResult> = types.iter().map(|&t| expr(t)).collect();
    ti.best_common_type(&expressions)
}

#[test]
fn best_common_type_int_and_short() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let short = known(ts, KnownTypeCode::Int16);
        let int = known(ts, KnownTypeCode::Int32);
        let (r, success) = best_common_type(ti, &[short, int]);
        assert_eq!(r, int);
        assert!(success);
    });
}

#[test]
fn best_common_type_null_and_string() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let null = ts.intern(TypeData::Null);
        let string = known(ts, KnownTypeCode::String);
        let (r, success) = best_common_type(ti, &[null, string]);
        assert_eq!(r, string);
        assert!(success);
    });
}

#[test]
fn best_common_type_null_and_int() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let null = ts.intern(TypeData::Null);
        let int = known(ts, KnownTypeCode::Int32);
        let (r, success) = best_common_type(ti, &[null, int]);
        assert_eq!(r, int);
        // By my read of the C# spec, the best common type is really the non-nullable `int`.
        // It's only a following step that will report an error if the argument expressions
        // are not convertible to the common type.
        assert!(success);
    });
}

#[test]
fn best_common_type_string_and_object() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let string = known(ts, KnownTypeCode::String);
        let object = known(ts, KnownTypeCode::Object);
        let (r, success) = best_common_type(ti, &[string, object]);
        assert_eq!(r, object);
        assert!(success);
    });
}

#[test]
fn best_common_type_string_and_dynamic() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let string = known(ts, KnownTypeCode::String);
        let dynamic = ts.intern(TypeData::Dynamic);
        let (r, success) = best_common_type(ti, &[string, dynamic]);
        assert_eq!(r, dynamic);
        assert!(success);
    });
}

#[test]
fn best_common_type_object_and_dynamic() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let object = known(ts, KnownTypeCode::Object);
        let dynamic = ts.intern(TypeData::Dynamic);
        let (r, success) = best_common_type(ti, &[object, dynamic]);
        assert_eq!(r, dynamic);
        assert!(success);
    });
}

#[test]
fn best_common_type_dynamic_and_object() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let dynamic = ts.intern(TypeData::Dynamic);
        let object = known(ts, KnownTypeCode::Object);
        let (r, success) = best_common_type(ti, &[dynamic, object]);
        assert_eq!(r, dynamic);
        assert!(success);
    });
}

#[test]
fn best_common_type_object_and_nullable_string() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let object = known(ts, KnownTypeCode::Object);
        let string = known(ts, KnownTypeCode::String);
        let object_nn = ts.change_nullability(object, Nullability::NotNullable);
        let string_n = ts.change_nullability(string, Nullability::Nullable);
        let expected = ts.change_nullability(object, Nullability::Nullable);
        let (r, success) = best_common_type(ti, &[object_nn, string_n]);
        assert_eq!(r, expected);
        assert!(success);
    });
}

#[test]
fn best_common_type_object_and_nullable_object() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let object = known(ts, KnownTypeCode::Object);
        let object_nn = ts.change_nullability(object, Nullability::NotNullable);
        let object_n = ts.change_nullability(object, Nullability::Nullable);
        let (r, success) = best_common_type(ti, &[object_nn, object_n]);
        assert_eq!(r, object_n);
        assert!(success);
    });
}

// ---- FindTypeInBounds ----

/// The types for C# type syntax, sorted by reflection name.
fn resolve(ts: &mut TypeSystem<'_, '_>, types: &[&str]) -> Vec<TypeId> {
    let unknown = unknown(ts);
    let mut r: Vec<TypeId> = types.iter().map(|s| t(ts, s)).collect();
    for &x in &r {
        assert_ne!(x, unknown);
    }
    r.sort_by_key(|&x| ts.reflection_name(x));
    r
}

/// Every candidate `FindTypeInBounds` reports with `ImprovedReturnAllResults`, sorted by
/// reflection name.
fn find_all_types_in_bounds(
    ti: &mut Ti<'_, '_, '_, '_>,
    lower_bounds: &[TypeId],
    upper_bounds: &[TypeId],
) -> Vec<TypeId> {
    ti.algorithm = TypeInferenceAlgorithm::ImprovedReturnAllResults;
    let t = ti.find_type_in_bounds(lower_bounds, upper_bounds);
    let mut r = expand_intersections(ti.conversions.ts, t);
    r.sort_by_key(|&x| ti.conversions.ts.reflection_name(x));
    r
}

/// The types an intersection stands for, expanding intersections in type arguments into
/// every combination.
fn expand_intersections(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> Vec<TypeId> {
    match ts.data(t).clone() {
        TypeData::Intersection(types) => types
            .iter()
            .flat_map(|&x| expand_intersections(ts, x))
            .collect(),
        TypeData::Inst { generic, args } => {
            let type_arguments: Vec<Vec<TypeId>> =
                args.iter().map(|&a| expand_intersections(ts, a)).collect();
            all_combinations(&type_arguments)
                .into_iter()
                .map(|ta| inst(ts, generic, &ta))
                .collect()
        }
        _ => vec![t],
    }
}

/// Performs the combinatorial explosion.
fn all_combinations(type_arguments: &[Vec<TypeId>]) -> Vec<Vec<TypeId>> {
    let mut result = vec![Vec::new()];
    for choices in type_arguments {
        result = result
            .into_iter()
            .flat_map(|prefix| {
                choices.iter().map(move |&c| {
                    let mut r = prefix.clone();
                    r.push(c);
                    r
                })
            })
            .collect();
    }
    result
}

/// Reflection names, for readable assertion messages.
fn names(ts: &TypeSystem<'_, '_>, types: &[TypeId]) -> Vec<String> {
    types.iter().map(|&x| ts.reflection_name(x)).collect()
}

fn assert_types_in_bounds(
    ti: &mut Ti<'_, '_, '_, '_>,
    lower: &[&str],
    upper: &[&str],
    expected: &[&str],
) {
    let ts = &mut *ti.conversions.ts;
    let lower = resolve(ts, lower);
    let upper = resolve(ts, upper);
    let expected = resolve(ts, expected);
    let actual = find_all_types_in_bounds(ti, &lower, &upper);
    let ts = &*ti.conversions.ts;
    assert_eq!(names(ts, &actual), names(ts, &expected));
    assert_eq!(actual, expected);
}

#[test]
fn list_of_short_and_int() {
    run(|ti| assert_types_in_bounds(ti, &["List<short>", "List<int>"], &[], &["IList"]));
}

#[test]
fn list_of_string_and_object() {
    // The covariant IReadOnlyList<object> (added in .NET 4.5) is more specific than
    // IEnumerable<object>, so it replaces it in the result set.
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &["List<string>", "List<object>"],
            &[],
            &["IList", "IReadOnlyList<object>"],
        )
    });
}

#[test]
fn list_of_list_of_string_and_object() {
    // As in ListOfStringAndObject, the covariant IReadOnlyList<T> replaces IEnumerable<T>
    // on both nesting levels.
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &["List<List<string>>", "List<List<object>>"],
            &[],
            &[
                "IList",
                "IReadOnlyList<IList>",
                "IReadOnlyList<IReadOnlyList<object>>",
            ],
        )
    });
}

#[test]
fn short_and_int() {
    run(|ti| assert_types_in_bounds(ti, &["short", "int"], &[], &["int"]));
}

#[test]
fn string_and_version() {
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &["string", "Version"],
            &[],
            &["ICloneable", "IComparable"],
        )
    });
}

#[test]
fn common_sub_type_clonable_comparable() {
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &[],
            &["ICloneable", "IComparable"],
            &["string", "Version"],
        )
    });
}

#[test]
fn enumerable_of_string_and_version() {
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &["IList<string>", "IList<Version>"],
            &[],
            &["IEnumerable<ICloneable>", "IEnumerable<IComparable>"],
        )
    });
}

#[test]
fn common_sub_type_ienumerable_clonable_ienumerable_comparable() {
    run(|ti| {
        assert_types_in_bounds(
            ti,
            &[],
            &["IEnumerable<ICloneable>", "IEnumerable<IComparable>"],
            &["IEnumerable<string>", "IEnumerable<Version>"],
        )
    });
}

#[test]
fn common_sub_type_ienumerable_clonable_ienumerable_comparable_list() {
    run(|ti| {
        // ReadOnlyCollectionBuilder<T> appears because the test compilation includes
        // System.Core, which declares it as another public implementation of both
        // IList and IList<T>.
        let ts = &mut *ti.conversions.ts;
        let lower = resolve(ts, &[]);
        let upper = resolve(
            ts,
            &[
                "IEnumerable<ICloneable>",
                "IEnumerable<IComparable>",
                "IList",
            ],
        );
        let expected = resolve(
            ts,
            &[
                "List<string>",
                "List<Version>",
                "Collection<string>",
                "Collection<Version>",
                "ReadOnlyCollection<string>",
                "ReadOnlyCollection<Version>",
                "System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<string>",
                "System.Runtime.CompilerServices.ReadOnlyCollectionBuilder<Version>",
            ],
        );
        let mut types_in_bounds = find_all_types_in_bounds(ti, &lower, &upper);
        // As this finds all derived types, the result set contains compiler-generated types like <>z__ReadOnlyArray`1.
        // We filter those out to make the test more robust against changes.
        let ts = &*ti.conversions.ts;
        types_in_bounds.retain(|&x| {
            let d = ts.definition(x).expect("a type definition");
            !ts.has_compiler_generated(d.module, TableId::TypeDef, d.row)
        });
        assert_eq!(names(ts, &types_in_bounds), names(ts, &expected));
        assert_eq!(types_in_bounds, expected);
    });
}

// ---- First-class span type inference ----

/// `M<T>(parameter_types)` called with `arguments` in `RefAssemblyCompilation`.
fn infer_span(
    parameter_types: impl FnOnce(&mut TypeSystem<'_, '_>, TypeId) -> Vec<TypeId>,
    arguments: impl FnOnce(&mut TypeSystem<'_, '_>) -> Vec<ResolveResult>,
    check: impl FnOnce(&mut TypeSystem<'_, '_>, Vec<TypeId>, bool),
) {
    run_ref(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let args = arguments(ts);
        let params = parameter_types(ts, tp);
        let (r, success) = infer(ti, &[tp], &args, &params);
        check(ti.conversions.ts, r, success);
    });
}

fn span_of(ts: &mut TypeSystem<'_, '_>, element: TypeId) -> TypeId {
    let span = known(ts, KnownTypeCode::SpanOfT);
    inst(ts, span, &[element])
}

fn read_only_span_of(ts: &mut TypeSystem<'_, '_>, element: TypeId) -> TypeId {
    let span = known(ts, KnownTypeCode::ReadOnlySpanOfT);
    inst(ts, span, &[element])
}

#[test]
fn span_argument_alone_infers_its_element_type() {
    infer_span(
        |ts, tp| vec![span_of(ts, tp)],
        |ts| {
            let string = known(ts, KnownTypeCode::String);
            vec![expr(span_of(ts, string))]
        },
        |ts, r, success| {
            assert_eq!(r, vec![known(ts, KnownTypeCode::String)]);
            assert!(success);
        },
    );
}

#[test]
fn span_argument_gives_an_exact_bound_conflicting_lower_bound_fails_inference() {
    // M<T>(Span<T>, T) called with (Span<string>, object): Span<T> is invariant, so the
    // span argument contributes an EXACT bound (C# 14 spec, 12.6.3.10: "If V is a
    // Span<V1>, then an exact inference is made"). The conflicting lower bound object
    // must fail inference; Roslyn reports CS0411 for this call.
    infer_span(
        |ts, tp| vec![span_of(ts, tp), tp],
        |ts| {
            let string = known(ts, KnownTypeCode::String);
            let object = known(ts, KnownTypeCode::Object);
            vec![expr(span_of(ts, string)), expr(object)]
        },
        |_, _, success| assert!(!success),
    );
}

#[test]
fn array_argument_for_span_parameter_gives_an_exact_bound_conflicting_lower_bound_fails_inference()
{
    // Same as above with a string[] argument: the array-to-Span conversion requires
    // identity element types, so the bound is exact. Roslyn reports CS0411.
    infer_span(
        |ts, tp| vec![span_of(ts, tp), tp],
        |ts| {
            let string = known(ts, KnownTypeCode::String);
            let object = known(ts, KnownTypeCode::Object);
            vec![expr(array(ts, string)), expr(object)]
        },
        |_, _, success| assert!(!success),
    );
}

#[test]
fn span_argument_for_read_only_span_parameter_gives_a_lower_bound() {
    // M<T>(ReadOnlySpan<T>, T) called with (Span<string>, object): ReadOnlySpan is
    // covariance-convertible, the span argument contributes a LOWER bound, and T=object
    // wins. Roslyn compiles this with T=object.
    infer_span(
        |ts, tp| vec![read_only_span_of(ts, tp), tp],
        |ts| {
            let string = known(ts, KnownTypeCode::String);
            let object = known(ts, KnownTypeCode::Object);
            vec![expr(span_of(ts, string)), expr(object)]
        },
        |ts, r, success| {
            assert_eq!(r, vec![known(ts, KnownTypeCode::Object)]);
            assert!(success);
        },
    );
}

#[test]
fn array_argument_for_read_only_span_parameter_gives_a_lower_bound() {
    infer_span(
        |ts, tp| vec![read_only_span_of(ts, tp), tp],
        |ts| {
            let string = known(ts, KnownTypeCode::String);
            let object = known(ts, KnownTypeCode::Object);
            vec![expr(array(ts, string)), expr(object)]
        },
        |ts, r, success| {
            assert_eq!(r, vec![known(ts, KnownTypeCode::Object)]);
            assert!(success);
        },
    );
}

/// `TupleUnderlyingTypeOrSelf` drops nullability, also the annotation `ChangeNullability`
/// puts on the generic type of a parameterized type, so `IEnumerator<string>!` (a return type
/// of a nullable-annotated library) still matches `IEnumerator<T>`.
#[test]
fn annotated_parameterized_type_to_its_generic_type() {
    run(|ti| {
        let ts = &mut *ti.conversions.ts;
        let tp = mtp(ts, 0, "T");
        let string = known(ts, KnownTypeCode::String);
        let enumerator = known(ts, KnownTypeCode::IEnumeratorOfT);
        let plain = inst(ts, enumerator, &[string]);
        let arg = ts.change_nullability(plain, Nullability::NotNullable);
        assert!(matches!(ts.data(arg), TypeData::Inst { .. }));
        let param = inst(ts, enumerator, &[tp]);
        let (r, success) = infer(ti, &[tp], &[expr(arg)], &[param]);
        assert!(success);
        assert_eq!(r, vec![string]);
    });
}
