//! `ICSharpCode.Decompiler.Tests/TypeSystem/TypeSystemLoaderTests.cs`: the compiled test
//! assembly (`TypeSystemOptions.Default`) with the legacy `mscorlib`
//! (`Default | OnlyPublicAPI`), as the managed fixture sets them up.
//!
//! `compilation.FindType(typeof(X))` is expressed as `ParseReflectionName` of the CLR full name
//! of `X`.

mod common;

use ilspy_decompiler::metadata::tables::rows;
use ilspy_decompiler::typesystem::compilation::{Compilation, TypeDefId, TypeKind};
use ilspy_decompiler::typesystem::members::{SymbolKind, method_accessibility};
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use ilspy_decompiler::typesystem::types::TypeSystem;

const NS: &str = "ICSharpCode.Decompiler.Tests.TypeSystem";

fn loader() -> Compilation<'static> {
    common::compilation(&[
        (common::test_assembly(), options::DEFAULT),
        (
            common::mscorlib(),
            options::DEFAULT | options::ONLY_PUBLIC_API,
        ),
    ])
}

fn type_def(ts: &mut TypeSystem<'_, '_>, reflection_name: &str) -> TypeDefId {
    ts.parse_reflection_name(reflection_name)
        .ok()
        .and_then(|t| ts.definition(t))
        .unwrap_or_else(|| panic!("{reflection_name} not found"))
}

fn test_type(ts: &mut TypeSystem<'_, '_>, name: &str) -> TypeDefId {
    type_def(ts, &format!("{NS}.{name}"))
}

fn methods(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> Vec<(ResolvedMethod, SymbolKind)> {
    ts.definition_methods(def).expect("methods")
}

fn method_flags(ts: &TypeSystem<'_, '_>, m: &ResolvedMethod) -> u16 {
    let MethodOrigin::Def(id) = m.origin else {
        panic!("{} has no MethodDef", m.name);
    };
    let md = &ts.comp.module(id.module).md;
    md.get::<rows::MethodDef>(id.row).expect("method").flags
}

fn type_attributes(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> u32 {
    ts.comp.type_def(def).expect("type").flags
}

fn is_abstract(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    type_attributes(ts, def) & 0x80 != 0
}

fn is_sealed(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> bool {
    type_attributes(ts, def) & 0x100 != 0
}

#[test]
fn enum_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let e = test_type(&mut ts, "MyEnum");
    assert_eq!(comp.type_def(e).unwrap().kind, TypeKind::Enum);
    let t = ts.intern_def(e);
    assert_eq!(ts.is_reference_type(t), Some(false));
    let underlying = ts.known_type(comp.type_def(e).unwrap().enum_underlying.expect("enum"));
    assert_eq!(ts.reflection_name(underlying), "System.Int16");
    let bases: Vec<String> = ts
        .direct_base_types(e)
        .unwrap()
        .iter()
        .map(|&b| ts.reflection_name(b))
        .collect();
    assert_eq!(bases, ["System.Enum"]);
}

#[test]
fn interface_should_derive_from_object() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IInterfaceWithProperty");
    let bases = ts.direct_base_types(def).unwrap();
    assert_eq!(bases.len(), 1, "should have exactly one direct base type");
    assert_eq!(ts.reflection_name(bases[0]), "System.Object");
}

#[test]
fn interface_should_derive_from_object_2() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IShadowTestDerived");
    let base = test_type(&mut ts, "IShadowTestBase");
    let bases = ts.direct_base_types(def).unwrap();
    assert_eq!(bases.len(), 2, "should have exactly two direct base types");
    assert_eq!(ts.reflection_name(bases[0]), "System.Object");
    assert_eq!(ts.definition(bases[1]), Some(base));
}

#[test]
fn check_interface_direct_base_types() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IDerived");
    let base1 = test_type(&mut ts, "IBase1");
    let base2 = test_type(&mut ts, "IBase2");
    let bases = ts.direct_base_types(def).unwrap();
    assert_eq!(
        bases.len(),
        3,
        "should have exactly three direct base types"
    );
    assert_eq!(ts.reflection_name(bases[0]), "System.Object");
    assert_eq!(ts.definition(bases[1]), Some(base1));
    assert_eq!(ts.definition(bases[2]), Some(base2));
}

#[test]
fn static_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "StaticClass");
    assert!(is_abstract(&ts, def));
    assert!(is_sealed(&ts, def));
    assert!(ts.type_flags(def).is_static);
}

#[test]
fn no_default_constructor_on_static_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "StaticClass");
    assert!(ts.definition_constructors(def).unwrap().is_empty());
}

#[test]
fn accessibility_tests() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "AccessibilityTest");
    let methods = methods(&mut ts, def);
    let accessibility = |name: &str| {
        let [m] = &methods
            .iter()
            .filter(|(m, _)| m.name == name)
            .map(|(m, _)| m)
            .collect::<Vec<_>>()[..]
        else {
            panic!("expected one method {name}");
        };
        method_accessibility(method_flags(&ts, m))
    };
    assert_eq!(accessibility("Public"), "Public");
    assert_eq!(accessibility("Internal"), "Internal");
    assert_eq!(accessibility("ProtectedInternal"), "ProtectedOrInternal");
    assert_eq!(accessibility("InternalProtected"), "ProtectedOrInternal");
    assert_eq!(accessibility("Protected"), "Protected");
    assert_eq!(accessibility("Private"), "Private");
    assert_eq!(accessibility("None"), "Private");
}

#[test]
fn date_time_default_constructor() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.DateTime");
    let explicit = methods(&mut ts, def)
        .iter()
        .filter(|(m, k)| *k == SymbolKind::Constructor && !m.is_static && m.base_params.is_empty())
        .count();
    assert_eq!(explicit, 1);
    let ctors = ts.definition_constructors(def).unwrap();
    assert_eq!(ctors.iter().filter(|m| m.base_params.is_empty()).count(), 1);
}

#[test]
fn no_encoding_info_default_constructor() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Text.EncodingInfo");
    // EncodingInfo only has an internal constructor, hidden by OnlyPublicAPI.
    assert!(
        !methods(&mut ts, def)
            .iter()
            .any(|(_, k)| *k == SymbolKind::Constructor)
    );
    // and no implicit constructor is added.
    assert!(ts.definition_constructors(def).unwrap().is_empty());
}

#[test]
fn static_modifier_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Environment");
    assert!(is_abstract(&ts, def), "class should be abstract");
    assert!(is_sealed(&ts, def), "class should be sealed");
    assert!(ts.type_flags(def).is_static, "class should be static");
}

#[test]
fn nested_types_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Environment+SpecialFolder");
    let t = ts.intern_def(def);
    assert_eq!(ts.full_name(t), "System.Environment.SpecialFolder");
    assert_eq!(ts.reflection_name(t), "System.Environment+SpecialFolder");
}

#[test]
fn find_redirected_type() {
    let comp = common::compilation(&[
        (common::system_core(), options::DEFAULT),
        (common::mscorlib(), options::DEFAULT),
    ]);
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Func`2, System.Core");
    assert_eq!(comp.module(def.module).assembly_name, "mscorlib");
}
