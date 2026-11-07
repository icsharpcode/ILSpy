//! `ICSharpCode.Decompiler.Tests/TypeSystem/ReflectionHelperTests.cs` over the legacy
//! `mscorlib`.
//!
//! `compilation.FindType(typeof(X))` is expressed as `ParseReflectionName` of the CLR full name
//! of `X`, which names the same type. `ParseNullReflectionName` has no counterpart, because a
//! `&str` cannot be null.

mod common;

use ilspy_decompiler::metadata::tables::rows;
use ilspy_decompiler::typesystem::compilation::Compilation;
use ilspy_decompiler::typesystem::members::MethodDefId;
use ilspy_decompiler::typesystem::names::split_type_parameter_count;
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::reflection::top_level_from_reflection;
use ilspy_decompiler::typesystem::types::{TypeData, TypeParamOwner, TypeSystem};

fn mscorlib() -> Compilation<'static> {
    common::compilation(&[(common::mscorlib(), options::DEFAULT)])
}

/// The reflection name of what `name` parses to, or `None` when parsing fails.
fn parse(name: &str) -> Option<String> {
    let comp = mscorlib();
    let mut ts = TypeSystem::new(&comp);
    let t = ts.parse_reflection_name(name).ok()?;
    Some(ts.reflection_name(t))
}

fn find_type(full_name: &str) -> String {
    parse(full_name).unwrap_or_else(|| panic!("{full_name} does not parse"))
}

fn assert_find_type(full_name: &str) {
    assert_eq!(find_type(full_name), full_name);
}

#[test]
fn get_inner_class() {
    assert_find_type("System.Environment+SpecialFolder");
}

#[test]
fn get_generic_class_1() {
    assert_find_type("System.Action`1");
}

#[test]
fn get_generic_class_2() {
    assert_find_type("System.Action`2");
}

#[test]
fn get_inner_class_in_generic_class_1() {
    assert_find_type("System.Collections.Generic.Dictionary`2+ValueCollection");
}

#[test]
fn get_inner_class_in_generic_class_2() {
    assert_find_type("System.Collections.Generic.Dictionary`2+ValueCollection+Enumerator");
}

#[test]
fn find_type_reflection_name_inner_class() {
    assert_eq!(
        find_type("System.Environment+SpecialFolder"),
        "System.Environment+SpecialFolder"
    );
}

#[test]
fn find_type_reflection_name_unbound_generic_class() {
    assert_eq!(find_type("System.Action`1"), "System.Action`1");
    assert_eq!(find_type("System.Action`2"), "System.Action`2");
}

#[test]
fn find_type_reflection_name_bound_generic_class() {
    assert_eq!(
        find_type("System.Action`1[[System.String, mscorlib]]"),
        "System.Action`1[[System.String]]"
    );
    assert_eq!(
        find_type("System.Action`2[[System.Int32, mscorlib],[System.Int16, mscorlib]]"),
        "System.Action`2[[System.Int32],[System.Int16]]"
    );
}

#[test]
fn find_type_reflection_name_nullable_type() {
    assert_eq!(
        find_type("System.Nullable`1[[System.Int32, mscorlib]]"),
        "System.Nullable`1[[System.Int32]]"
    );
}

#[test]
fn find_type_reflection_name_inner_class_in_unbound_generic_type() {
    assert_eq!(
        find_type("System.Collections.Generic.Dictionary`2+ValueCollection"),
        "System.Collections.Generic.Dictionary`2+ValueCollection"
    );
}

#[test]
fn find_type_reflection_name_inner_class_in_bound_generic_type() {
    assert_eq!(
        find_type(
            "System.Collections.Generic.Dictionary`2+KeyCollection[[System.String, mscorlib],[System.Int32, mscorlib]]"
        ),
        "System.Collections.Generic.Dictionary`2+KeyCollection[[System.String],[System.Int32]]"
    );
}

#[test]
fn find_type_reflection_name_array_type() {
    assert_find_type("System.Int32[]");
}

#[test]
fn find_type_reflection_name_multidimensional_array_type() {
    assert_find_type("System.Int32[,]");
}

#[test]
fn find_type_reflection_name_jagged_multidimensional_array_type() {
    // typeof(int[,][,,]).FullName
    assert_find_type("System.Int32[,,][,]");
}

#[test]
fn find_type_reflection_name_pointer_type() {
    assert_find_type("System.Int32*");
}

#[test]
fn find_type_reflection_name_by_reference_type() {
    assert_find_type("System.Int32&");
}

#[test]
fn parse_reflection_name() {
    assert_eq!(find_type("System.Int32"), "System.Int32");
    assert_eq!(find_type("System.Int32&"), "System.Int32&");
    assert_eq!(find_type("System.Int32*&"), "System.Int32*&");
    assert_eq!(
        find_type(
            "System.Int32, System.Private.CoreLib, Version=11.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"
        ),
        "System.Int32"
    );
    assert_eq!(
        find_type("System.Action`1[[System.String]]"),
        "System.Action`1[[System.String]]"
    );
    assert_eq!(
        find_type("System.Action`1[[System.String, mscorlib]]"),
        "System.Action`1[[System.String]]"
    );
    assert_eq!(
        find_type(
            "System.Int32[,,][,], System.Private.CoreLib, Version=11.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"
        ),
        "System.Int32[,,][,]"
    );
    assert_eq!(
        find_type("System.Environment+SpecialFolder"),
        "System.Environment+SpecialFolder"
    );
}

#[test]
fn parse_open_generic_reflection_name() {
    const NAME: &str = "System.Converter`2[[`0],[``0]]";
    assert_eq!(find_type(NAME), NAME);

    let comp = mscorlib();
    let mut ts = TypeSystem::new(&comp);
    let list = ts
        .parse_reflection_name("System.Collections.Generic.List`1")
        .ok()
        .and_then(|t| ts.definition(t))
        .expect("List`1");
    let md = &comp.module(list.module).md;
    let convert_all = md
        .type_methods(list.row)
        .expect("methods")
        .into_iter()
        .filter(|&row| {
            md.get::<rows::MethodDef>(row)
                .and_then(|m| md.string(m.name).map(|n| n == "ConvertAll"))
                .unwrap_or(false)
        })
        .map(|row| MethodDefId {
            module: list.module,
            row,
        })
        .collect::<Vec<_>>();
    let [convert_all] = convert_all[..] else {
        panic!("expected one ConvertAll, found {}", convert_all.len());
    };
    let ctx = ts.method_context(convert_all).expect("context");
    let converter = ts.parse_reflection_name_in(NAME, ctx).expect("parses");
    assert_eq!(ts.reflection_name(converter), NAME);
    // In the method's context the parameters are List<T>'s T and ConvertAll's TOutput.
    let TypeData::Inst { args, .. } = ts.data(converter).clone() else {
        panic!("Converter`2 is not instantiated");
    };
    assert!(matches!(
        ts.data(args[0]),
        TypeData::TypeParam {
            owner: TypeParamOwner::Type(def),
            index: 0
        } if *def == list
    ));
    assert!(matches!(
        ts.data(args[1]),
        TypeData::TypeParam {
            owner: TypeParamOwner::Method(m),
            index: 0
        } if *m == convert_all
    ));
}

#[test]
fn array_of_type_parameter() {
    assert_eq!(find_type("`0[,]"), "`0[,]");
}

#[test]
fn parse_invalid_reflection_name_1() {
    assert_eq!(parse(""), None);
}

#[test]
fn parse_invalid_reflection_name_2() {
    assert_eq!(find_type("`"), "`");
}

#[test]
fn parse_invalid_reflection_name_3() {
    assert_eq!(find_type("``"), "``");
}

#[test]
fn parse_invalid_reflection_name_4() {
    assert_eq!(find_type("System.Action`A"), "System.Action`A");
}

#[test]
fn parse_invalid_reflection_name_5() {
    assert_eq!(parse("System.Environment+"), None);
}

#[test]
fn parse_invalid_reflection_name_5b() {
    assert_eq!(find_type("System.Environment+`"), "System.Environment+`");
}

#[test]
fn parse_invalid_reflection_name_6() {
    assert_eq!(parse("System.Int32["), None);
}

#[test]
fn parse_invalid_reflection_name_7() {
    assert_eq!(find_type("System.Int32[`]"), "System.Int32");
}

#[test]
fn parse_invalid_reflection_name_8() {
    assert_eq!(parse("System.Int32[,"), None);
}

#[test]
fn parse_invalid_reflection_name_9() {
    assert_eq!(parse("System.Int32]"), None);
}

#[test]
fn parse_invalid_reflection_name_10() {
    assert_eq!(parse("System.Int32*a"), None);
}

#[test]
fn parse_invalid_reflection_name_11() {
    assert_eq!(parse("System.Action`1[[]]"), None);
}

#[test]
fn parse_invalid_reflection_name_12() {
    assert_eq!(parse("System.Action`1[[System.Int32]a]"), None);
}

#[test]
fn parse_invalid_reflection_name_13() {
    assert_eq!(parse("System.Action`1[[System.Int32],]"), None);
}

#[test]
fn parse_invalid_reflection_name_14() {
    assert_eq!(parse("System.Action`1[[System.Int32]"), None);
}

#[test]
fn parse_invalid_reflection_name_15() {
    assert_eq!(parse("System.Action`1[[System.Int32"), None);
}

#[test]
fn parse_invalid_reflection_name_16() {
    assert_eq!(parse("System.Action`1[[System.Int32],[System.String"), None);
}

#[test]
fn split_type_parameter_count_from_name() {
    assert_eq!(split_type_parameter_count("List`1"), ("List", 1));
    assert_eq!(
        split_type_parameter_count("Dictionary`2"),
        ("Dictionary", 2)
    );
    assert_eq!(split_type_parameter_count("Foo`12"), ("Foo", 12));
}

#[test]
fn split_type_parameter_count_without_backtick() {
    assert_eq!(split_type_parameter_count("String"), ("String", 0));
}

#[test]
fn split_type_parameter_count_uses_the_last_backtick() {
    assert_eq!(
        split_type_parameter_count("Outer`1+Inner`2"),
        ("Outer`1+Inner", 2)
    );
}

#[test]
fn split_type_parameter_count_keeps_name_when_suffix_is_not_a_plain_number() {
    for name in [
        "Foo`",
        "Foo`x",
        "Foo`+1",
        "Foo`2147483648",
        "Foo`99999999999999999999",
    ] {
        assert_eq!(split_type_parameter_count(name), (name, 0));
    }
}

#[test]
fn top_level_type_name_parses_namespace_name_and_arity() {
    let t = top_level_from_reflection("System.Collections.Generic.List`1");
    assert_eq!(
        (t.ns.as_str(), t.name.as_str(), t.tpc),
        ("System.Collections.Generic", "List", 1)
    );
}

#[test]
fn top_level_type_name_without_namespace() {
    let t = top_level_from_reflection("List`1");
    assert_eq!((t.ns.as_str(), t.name.as_str(), t.tpc), ("", "List", 1));
}

#[test]
fn top_level_type_name_without_arity() {
    let t = top_level_from_reflection("System.String");
    assert_eq!(
        (t.ns.as_str(), t.name.as_str(), t.tpc),
        ("System", "String", 0)
    );
}

#[test]
fn top_level_type_name_ignores_backtick_inside_the_namespace() {
    let t = top_level_from_reflection("A`1.B");
    assert_eq!((t.ns.as_str(), t.name.as_str(), t.tpc), ("A`1", "B", 0));
}
