//! `ICSharpCode.Decompiler.Tests/TypeSystem/TypeSystemLoaderTests.cs`: the compiled test
//! assembly (`TypeSystemOptions.Default`) with the legacy `mscorlib`
//! (`Default | OnlyPublicAPI`), as the managed fixture sets them up.
//!
//! `compilation.FindType(typeof(X))` is expressed as `ParseReflectionName` of the CLR full name
//! of `X`.

mod common;

use ilspy_decompiler::metadata::tables::rows;
use ilspy_decompiler::typesystem::attrs::{accessibility_name, type_accessibility};
use ilspy_decompiler::typesystem::compilation::{Compilation, TypeDefId, TypeKind};
use ilspy_decompiler::typesystem::inheritance::{MemberDef, MemberRef, Modifiers};
use ilspy_decompiler::typesystem::members::{
    MethodDefId, SymbolKind, has_body, method_accessibility,
};
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use ilspy_decompiler::typesystem::types::{TypeData, TypeId, TypeSystem};

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

fn single<T>(items: Vec<T>, what: &str) -> T {
    assert_eq!(items.len(), 1, "expected exactly one {what}");
    items.into_iter().next().unwrap()
}

fn method_named(ts: &mut TypeSystem<'_, '_>, def: TypeDefId, name: &str) -> MethodDefId {
    let found = methods(ts, def)
        .into_iter()
        .filter_map(|(m, _)| match m.origin {
            MethodOrigin::Def(id) if m.name == name => Some(id),
            _ => None,
        })
        .collect();
    single(found, name)
}

fn properties(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> Vec<MemberDef> {
    ts.comp
        .visible_properties(def)
        .unwrap()
        .into_iter()
        .map(|r| MemberDef::Property(def.module, r))
        .collect()
}

fn events(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> Vec<MemberDef> {
    ts.comp
        .visible_events(def)
        .unwrap()
        .into_iter()
        .map(|r| MemberDef::Event(def.module, r))
        .collect()
}

fn name(ts: &TypeSystem<'_, '_>, m: MemberDef) -> String {
    ts.member_name(m).unwrap()
}

fn named(ts: &TypeSystem<'_, '_>, members: Vec<MemberDef>, wanted: &str) -> MemberDef {
    let found = members
        .into_iter()
        .filter(|&m| name(ts, m) == wanted)
        .collect();
    single(found, wanted)
}

/// Getter and setter of a property, adder and remover of an event.
fn accessors(ts: &TypeSystem<'_, '_>, m: MemberDef) -> [Option<MemberDef>; 2] {
    let (module, first, second) = match m {
        MemberDef::Property(module, row) => {
            let (get, set, _) = ts.comp.module(module).md.property_accessors(row);
            (module, get, set)
        }
        MemberDef::Event(module, row) => {
            let (add, remove, _, _) = ts.comp.module(module).md.event_accessors(row);
            (module, add, remove)
        }
        _ => panic!("not a property or event"),
    };
    [first, second].map(|row| (row != 0).then_some(MemberDef::Method(MethodDefId { module, row })))
}

fn getter(ts: &TypeSystem<'_, '_>, p: MemberDef) -> MemberDef {
    accessors(ts, p)[0].expect("getter / adder")
}

fn setter(ts: &TypeSystem<'_, '_>, p: MemberDef) -> MemberDef {
    accessors(ts, p)[1].expect("setter / remover")
}

fn method_id(m: MemberDef) -> MethodDefId {
    match m {
        MemberDef::Method(id) => id,
        _ => panic!("not a method"),
    }
}

fn access(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> &'static str {
    accessibility_name(ts.member_accessibility(m).unwrap())
}

fn mods(ts: &TypeSystem<'_, '_>, m: MemberDef) -> Modifiers {
    ts.member_modifiers(m).unwrap()
}

fn body(ts: &TypeSystem<'_, '_>, m: MemberDef) -> bool {
    let id = method_id(m);
    has_body(
        &ts.comp
            .module(id.module)
            .md
            .get::<rows::MethodDef>(id.row)
            .unwrap(),
    )
}

fn symbol(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> SymbolKind {
    ts.member_symbol_kind(m).unwrap()
}

fn parameter_names(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> Vec<String> {
    let params = match m {
        MemberDef::Method(id) => ts.method_signature(id).1,
        MemberDef::Property(module, row) => ts.property_signature(module, row).unwrap().1,
        _ => panic!("no parameters"),
    };
    params.into_iter().map(|p| p.name).collect()
}

/// `IMember.ExplicitlyImplementedInterfaceMembers`.
fn explicit(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> Vec<MemberRef> {
    match m {
        MemberDef::Method(id) => ts
            .explicit_implementations(id)
            .unwrap()
            .into_iter()
            .map(|r| match (r.origin, r.declaring) {
                (MethodOrigin::Def(d), Some(declaring)) => MemberRef {
                    def: MemberDef::Method(d),
                    declaring,
                },
                _ => panic!("explicit implementation of an unresolved method"),
            })
            .collect(),
        MemberDef::Property(module, row) => {
            ts.property_explicit_implementations(module, row).unwrap()
        }
        MemberDef::Event(module, row) => ts.event_explicit_implementations(module, row).unwrap(),
        MemberDef::Field(..) => Vec::new(),
    }
}

fn is_explicit(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> bool {
    let accessor = match m {
        MemberDef::Method(id) => Some(id),
        MemberDef::Property(module, row) => ts.property_any_accessor(module, row),
        MemberDef::Event(module, row) => ts.event_any_accessor(module, row),
        MemberDef::Field(..) => None,
    };
    accessor.is_some_and(|a| ts.is_explicit_interface_implementation(a).unwrap())
}

/// `IMember.ReflectionName`.
fn reflection_names(ts: &TypeSystem<'_, '_>, refs: &[MemberRef]) -> Vec<String> {
    refs.iter()
        .map(|r| format!("{}.{}", ts.reflection_name(r.declaring), name(ts, r.def)))
        .collect()
}

fn type_accessibility_of(ts: &TypeSystem<'_, '_>, def: TypeDefId) -> &'static str {
    accessibility_name(type_accessibility(type_attributes(ts, def)))
}

fn method_members(
    ts: &mut TypeSystem<'_, '_>,
    def: TypeDefId,
    constructors: bool,
) -> Vec<MemberDef> {
    methods(ts, def)
        .into_iter()
        .filter(|(_, k)| constructors || *k != SymbolKind::Constructor)
        .filter_map(|(m, _)| match m.origin {
            MethodOrigin::Def(id) => Some(MemberDef::Method(id)),
            _ => None,
        })
        .collect()
}

#[test]
fn property_with_protected_setter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = named(&ts, properties(&ts, def), "PropertyWithProtectedSetter");
    let [get, set] = accessors(&ts, p).map(|a| a.expect("both accessors"));
    assert_eq!(access(&mut ts, p), "Public");
    assert_eq!(access(&mut ts, get), "Public");
    assert_eq!(access(&mut ts, set), "Protected");
}

#[test]
fn property_with_private_getter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = named(&ts, properties(&ts, def), "PropertyWithPrivateGetter");
    let [get, set] = accessors(&ts, p).map(|a| a.expect("both accessors"));
    assert_eq!(access(&mut ts, p), "Public");
    assert_eq!(access(&mut ts, get), "Private");
    assert_eq!(access(&mut ts, set), "Public");
    assert!(body(&ts, get));
}

#[test]
fn property_without_setter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = named(&ts, properties(&ts, def), "PropertyWithoutSetter");
    let [get, set] = accessors(&ts, p);
    let get = get.expect("getter");
    assert!(set.is_none());
    assert_eq!(access(&mut ts, p), "Public");
    assert_eq!(access(&mut ts, get), "Public");
}

fn indexer_of(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> MemberDef {
    let found = properties(ts, def)
        .into_iter()
        .filter(|&p| symbol(ts, p) == SymbolKind::Indexer)
        .collect();
    single(found, "indexer")
}

#[test]
fn indexer() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = indexer_of(&mut ts, def);
    assert_eq!(name(&ts, p), "Item");
    assert_eq!(parameter_names(&mut ts, p), ["index"]);
}

#[test]
fn indexer_getter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = indexer_of(&mut ts, def);
    let get = getter(&ts, p);
    assert_eq!(symbol(&mut ts, get), SymbolKind::Accessor);
    assert_eq!(name(&ts, get), "get_Item");
    assert_eq!(access(&mut ts, get), "Public");
    assert_eq!(parameter_names(&mut ts, get), ["index"]);
    let ret = ts.method_signature(method_id(get)).0;
    assert_eq!(ts.reflection_name(ret), "System.String");
    let MemberDef::Property(_, row) = p else {
        unreachable!()
    };
    assert_eq!(
        ts.accessor_owner(method_id(get)).map(|(o, _)| o),
        Some(0x1700_0000 | row)
    );
}

#[test]
fn indexer_setter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = indexer_of(&mut ts, def);
    let set = setter(&ts, p);
    assert_eq!(symbol(&mut ts, set), SymbolKind::Accessor);
    assert_eq!(name(&ts, set), "set_Item");
    assert_eq!(access(&mut ts, set), "Public");
    assert_eq!(parameter_names(&mut ts, set), ["index", "value"]);
    let ret = ts.method_signature(method_id(set)).0;
    assert_eq!(ts.reflection_name(ret), "System.Void");
}

fn assert_abstract_public(ts: &mut TypeSystem<'_, '_>, m: MemberDef) {
    let mods = mods(ts, m);
    assert!(mods.is_abstract);
    assert!(mods.is_overridable);
    assert!(!mods.is_override);
    assert_eq!(access(ts, m), "Public");
}

#[test]
fn flags_on_interface_members_are_correct() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IInterfaceWithProperty");
    let p = single(properties(&ts, def), "property");
    assert_ne!(symbol(&mut ts, p), SymbolKind::Indexer);
    assert_abstract_public(&mut ts, p);
    for accessor in accessors(&ts, p).map(|a| a.expect("accessor")) {
        assert_abstract_public(&mut ts, accessor);
        assert!(!body(&ts, accessor));
    }

    let def = test_type(&mut ts, "IInterfaceWithIndexers");
    let two = properties(&ts, def)
        .into_iter()
        .filter(|&p| parameter_names(&mut ts, p).len() == 2)
        .collect();
    let p = single(two, "two-parameter indexer");
    assert_eq!(symbol(&mut ts, p), SymbolKind::Indexer);
    assert_abstract_public(&mut ts, p);
    for accessor in accessors(&ts, p).map(|a| a.expect("accessor")) {
        assert_abstract_public(&mut ts, accessor);
    }

    let def = test_type(&mut ts, "IHasEvent");
    let e = single(events(&ts, def), "event");
    assert_abstract_public(&mut ts, e);
    for accessor in accessors(&ts, e).map(|a| a.expect("accessor")) {
        assert_abstract_public(&mut ts, accessor);
    }

    let def = type_def(&mut ts, "System.IDisposable");
    let m = single(method_members(&mut ts, def, true), "method");
    assert_abstract_public(&mut ts, m);
}

fn assert_accessors_overridable_not_override(ts: &mut TypeSystem<'_, '_>, type_name: &str) {
    let def = test_type(ts, type_name);
    let p = named(ts, properties(ts, def), "Prop");
    for accessor in accessors(ts, p).map(|a| a.expect("accessor")) {
        assert!(!mods(ts, accessor).is_override);
        assert!(mods(ts, accessor).is_overridable);
    }
}

#[test]
fn interface_property_accessors_should_not_be_overrides() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    assert_accessors_overridable_not_override(&mut ts, "IInterfaceWithProperty");
}

#[test]
fn virtual_property_accessors_should_not_be_overrides() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    assert_accessors_overridable_not_override(&mut ts, "ClassWithVirtualProperty");
}

#[test]
fn class_that_overrides_getter_only() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatOverridesGetterOnly");
    let p = named(&ts, properties(&ts, def), "Prop");
    assert_eq!(access(&mut ts, p), "Public");
    let get = getter(&ts, p);
    assert_eq!(access(&mut ts, get), "Public");
}

#[test]
fn class_that_overrides_setter_only() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatOverridesSetterOnly");
    let p = named(&ts, properties(&ts, def), "Prop");
    assert_eq!(access(&mut ts, p), "Public");
    let set = setter(&ts, p);
    assert_eq!(access(&mut ts, set), "Protected");
}

#[test]
fn property_that_implements_interface_is_not_virtual() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatImplementsProperty");
    let p = named(&ts, properties(&ts, def), "Prop");
    let mods = mods(&ts, p);
    assert!(!mods.is_virtual);
    assert!(!mods.is_overridable);
    assert!(!mods.is_sealed);
}

#[test]
fn property_sealed_override() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatOverridesAndSealsVirtualProperty");
    let p = named(&ts, properties(&ts, def), "Prop");
    let mods = mods(&ts, p);
    assert!(!mods.is_virtual);
    assert!(mods.is_override);
    assert!(mods.is_sealed);
    assert!(!mods.is_overridable);
}

#[test]
fn explicit_indexer_implementation_returns_the_correct_members() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatImplementsIndexersExplicitly");
    let props = properties(&ts, def);
    assert!(!props.is_empty());
    for p in props {
        assert_eq!(symbol(&mut ts, p), SymbolKind::Indexer);
        assert_eq!(explicit(&mut ts, p).len(), 1);
        for accessor in accessors(&ts, p).map(|a| a.expect("accessor")) {
            assert_eq!(explicit(&mut ts, accessor).len(), 1);
        }
    }
}

#[test]
fn explicit_disposable_implementation() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ExplicitDisposableImplementation");
    let m = single(method_members(&mut ts, def, false), "method");
    assert!(is_explicit(&mut ts, m));
    let implemented = single(explicit(&mut ts, m), "implemented member");
    let full = format!(
        "{}.{}",
        ts.full_name(implemented.declaring),
        name(&ts, implemented.def)
    );
    assert_eq!(full, "System.IDisposable.Dispose");
}

fn is_by_ref(ts: &TypeSystem<'_, '_>, t: TypeId) -> bool {
    matches!(ts.data(t), TypeData::ByRef(_))
}

#[test]
fn explicit_generic_interface_implementation() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ExplicitGenericInterfaceImplementation");
    let generic_of_string = ts
        .parse_reflection_name(&format!("{NS}.IGenericInterface`1[[System.String]]"))
        .unwrap();
    let mut seen = Vec::new();
    for m in method_members(&mut ts, def, false) {
        let second = ts.method_signature(method_id(m)).1[1].ty;
        let by_ref = is_by_ref(&ts, second);
        assert!(is_explicit(&mut ts, m));
        let implemented = ts.explicit_implementations(method_id(m)).unwrap();
        let implemented = single(implemented, "implemented method");
        assert_eq!(implemented.declaring, Some(generic_of_string));
        assert_eq!(is_by_ref(&ts, implemented.params().unwrap()[1]), by_ref);
        seen.push(by_ref);
    }
    seen.sort();
    assert_eq!(seen, [false, true]);
}

#[test]
fn explicitly_implemented_properties_should_be_reported_as_being_implemented() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatImplementsPropertyExplicitly");
    let p = single(properties(&ts, def), "property");
    let prop = explicit(&mut ts, p);
    assert_eq!(
        reflection_names(&ts, &prop),
        [format!("{NS}.IInterfaceWithProperty.Prop")]
    );
    let get_accessor = getter(&ts, p);
    let get = explicit(&mut ts, get_accessor);
    assert_eq!(
        reflection_names(&ts, &get),
        [format!("{NS}.IInterfaceWithProperty.get_Prop")]
    );
    let set_accessor = setter(&ts, p);
    let set = explicit(&mut ts, set_accessor);
    assert_eq!(
        reflection_names(&ts, &set),
        [format!("{NS}.IInterfaceWithProperty.set_Prop")]
    );
}

#[test]
fn explicitly_implemented_properties_should_have_explicitly_implemented_accessors() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatImplementsPropertyExplicitly");
    let p = single(properties(&ts, def), "property");
    assert!(is_explicit(&mut ts, p));
    let get = getter(&ts, p);
    let set = setter(&ts, p);
    assert!(is_explicit(&mut ts, get));
    assert!(is_explicit(&mut ts, set));
}

#[test]
fn explicitly_implemented_events_should_be_reported_as_being_implemented() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassThatImplementsEventExplicitly");
    let e = single(events(&ts, def), "event");
    let ev = explicit(&mut ts, e);
    assert_eq!(
        reflection_names(&ts, &ev),
        [format!("{NS}.IHasEvent.Event")]
    );
    let add_accessor = getter(&ts, e);
    let add = explicit(&mut ts, add_accessor);
    assert_eq!(
        reflection_names(&ts, &add),
        [format!("{NS}.IHasEvent.add_Event")]
    );
    let remove_accessor = setter(&ts, e);
    let remove = explicit(&mut ts, remove_accessor);
    assert_eq!(
        reflection_names(&ts, &remove),
        [format!("{NS}.IHasEvent.remove_Event")]
    );
}

#[test]
fn members_declared_in_derived_interfaces_do_not_implement_base_members() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IShadowTestDerived");
    let method = MemberDef::Method(method_named(&mut ts, def, "Method"));
    let indexer = indexer_of(&mut ts, def);
    let prop = named(&ts, properties(&ts, def), "Prop");
    let evt = named(&ts, events(&ts, def), "Evt");
    let mut members = vec![method];
    for m in [indexer, prop, evt] {
        members.push(m);
        members.extend(accessors(&ts, m).map(|a| a.expect("accessor")));
    }
    for m in members {
        assert!(explicit(&mut ts, m).is_empty(), "{}", name(&ts, m));
    }
}

#[test]
fn indexer_non_default_name() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IndexerNonDefaultName");
    let p = indexer_of(&mut ts, def);
    assert_eq!(name(&ts, p), "Foo");
}

#[test]
fn inheritance_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let c = type_def(&mut ts, "System.SystemException");
    let c2 = type_def(&mut ts, "System.Exception");
    let first = ts.direct_base_types(c).unwrap()[0];
    assert_eq!(ts.full_name(first), "System.Exception");
    assert_eq!(ts.definition(first), Some(c2));
    let t = ts.intern_def(c);
    let all: Vec<String> = ts
        .all_base_types(t)
        .unwrap()
        .into_iter()
        .map(|b| ts.reflection_name(b))
        .collect();
    assert_eq!(
        all,
        [
            "System.Object",
            "System.Runtime.Serialization.ISerializable",
            "System.Runtime.InteropServices._Exception",
            "System.Exception",
            "System.SystemException"
        ]
    );
}

fn assert_static_members(
    ts: &mut TypeSystem<'_, '_>,
    members: Vec<MemberDef>,
    expected: [(&str, bool); 4],
) {
    for (wanted, is_static) in expected {
        let m = named(ts, members.clone(), wanted);
        assert_eq!(mods(ts, m).is_static, is_static, "{wanted}");
        for accessor in accessors(ts, m).map(|a| a.expect("accessor")) {
            assert_eq!(
                mods(ts, accessor).is_static,
                is_static,
                "{}",
                name(ts, accessor)
            );
        }
    }
}

#[test]
fn staticity_of_event_accessors() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithStaticAndNonStaticMembers");
    let evs = events(&ts, def);
    assert_static_members(
        &mut ts,
        evs,
        [
            ("Event1", true),
            ("Event2", false),
            ("Event3", true),
            ("Event4", false),
        ],
    );
}

#[test]
fn staticity_of_property_accessors() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithStaticAndNonStaticMembers");
    let props = properties(&ts, def);
    assert_static_members(
        &mut ts,
        props,
        [
            ("Prop1", true),
            ("Prop2", false),
            ("Prop3", true),
            ("Prop4", false),
        ],
    );
}

#[test]
fn property_accessors_have_body() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithStaticAndNonStaticMembers");
    for p in properties(&ts, def) {
        for accessor in accessors(&ts, p).map(|a| a.expect("accessor")) {
            assert!(body(&ts, accessor), "{}", name(&ts, accessor));
        }
    }
}

#[test]
fn event_accessor_names() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithStaticAndNonStaticMembers");
    for (event, add, remove) in [
        ("Event1", "add_Event1", "remove_Event1"),
        ("Event3", "add_Event3", "remove_Event3"),
    ] {
        let e = named(&ts, events(&ts, def), event);
        assert_eq!(name(&ts, getter(&ts, e)), add);
        assert_eq!(name(&ts, setter(&ts, e)), remove);
    }
}

#[test]
fn event_accessor_have_body() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithStaticAndNonStaticMembers");
    for e in events(&ts, def) {
        for accessor in accessors(&ts, e).map(|a| a.expect("accessor")) {
            assert!(body(&ts, accessor), "{}", name(&ts, accessor));
        }
    }
}

#[test]
fn value_collection_count_modifiers() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(
        &mut ts,
        "System.Collections.Generic.Dictionary`2+ValueCollection",
    );
    assert_eq!(type_accessibility_of(&ts, def), "Public");
    assert!(is_sealed(&ts, def));
    assert!(!is_abstract(&ts, def));
    assert!(!ts.type_flags(def).is_static);
    let count = named(&ts, properties(&ts, def), "Count");
    assert_eq!(access(&mut ts, count), "Public");
    // Sealed on the IL level, but in C# a normal non-virtual member implementing an interface.
    let mods = mods(&ts, count);
    assert!(!mods.is_sealed);
    assert!(!mods.is_virtual);
    assert!(!mods.is_abstract);
}

#[test]
fn math_acos_modifiers() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Math");
    assert_eq!(type_accessibility_of(&ts, def), "Public");
    assert!(is_sealed(&ts, def));
    assert!(is_abstract(&ts, def));
    assert!(ts.type_flags(def).is_static);
    let acos = MemberDef::Method(method_named(&mut ts, def, "Acos"));
    assert_eq!(access(&mut ts, acos), "Public");
    let mods = mods(&ts, acos);
    assert!(mods.is_static);
    assert!(!mods.is_abstract);
    assert!(!mods.is_sealed);
    assert!(!mods.is_virtual);
    assert!(!mods.is_override);
}

#[test]
fn encoding_modifiers() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Text.Encoding");
    assert_eq!(type_accessibility_of(&ts, def), "Public");
    assert!(!is_sealed(&ts, def));
    assert!(is_abstract(&ts, def));

    let get_decoder = MemberDef::Method(method_named(&mut ts, def, "GetDecoder"));
    assert_eq!(access(&mut ts, get_decoder), "Public");
    let m = mods(&ts, get_decoder);
    assert!(!m.is_static && !m.is_abstract && !m.is_sealed && m.is_virtual && !m.is_override);

    let get_max_byte_count = MemberDef::Method(method_named(&mut ts, def, "GetMaxByteCount"));
    assert_eq!(access(&mut ts, get_max_byte_count), "Public");
    let m = mods(&ts, get_max_byte_count);
    assert!(!m.is_static && m.is_abstract && !m.is_sealed && !m.is_virtual && !m.is_override);

    let fallback = named(&ts, properties(&ts, def), "EncoderFallback");
    assert_eq!(access(&mut ts, fallback), "Public");
    let m = mods(&ts, fallback);
    assert!(!m.is_static && !m.is_abstract && !m.is_sealed && !m.is_virtual && !m.is_override);
}

fn assert_get_decoder_overrides(type_name: &str, sealed: bool) {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, type_name);
    assert_eq!(type_accessibility_of(&ts, def), "Public");
    assert_eq!(is_sealed(&ts, def), sealed);
    assert!(!is_abstract(&ts, def));
    let get_decoder = MemberDef::Method(method_named(&mut ts, def, "GetDecoder"));
    assert_eq!(access(&mut ts, get_decoder), "Public");
    let m = mods(&ts, get_decoder);
    assert!(!m.is_static && !m.is_abstract && !m.is_sealed && !m.is_virtual && m.is_override);
}

#[test]
fn unicode_encoding_modifiers() {
    assert_get_decoder_overrides("System.Text.UnicodeEncoding", false);
}

#[test]
fn utf32_encoding_modifiers() {
    assert_get_decoder_overrides("System.Text.UTF32Encoding", true);
}

/// For the members named `wanted` or ending in `.wanted` (optionally their accessor `which`):
/// how many interface members each implements explicitly (sorted), and those members.
fn explicit_counts(
    ts: &mut TypeSystem<'_, '_>,
    members: &[MemberDef],
    wanted: &str,
    which: Option<usize>,
) -> (Vec<usize>, Vec<MemberRef>) {
    let mut counts = Vec::new();
    let mut implemented = Vec::new();
    for &m in members {
        let n = name(ts, m);
        if n != wanted && !n.ends_with(&format!(".{wanted}")) {
            continue;
        }
        let target = match which {
            Some(i) => accessors(ts, m)[i].expect("accessor"),
            None => m,
        };
        let refs = explicit(ts, target);
        counts.push(refs.len());
        implemented.extend(refs);
    }
    counts.sort();
    (counts, implemented)
}

#[test]
fn explicit_implementation() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ExplicitImplementationTests");
    let idef = test_type(&mut ts, "IExplicitImplementationTests");

    let ms = method_members(&mut ts, def, true);
    let imethod = MemberDef::Method(method_named(&mut ts, idef, "M"));
    let (counts, implemented) = explicit_counts(&mut ts, &ms, "M", None);
    assert_eq!(counts, [0, 1]);
    assert_eq!(single(implemented, "implemented M").def, imethod);

    let props = properties(&ts, def);
    let evs = events(&ts, def);
    let groups = [
        ("P", properties(&ts, idef), &props),
        ("Item", properties(&ts, idef), &props),
        ("E", events(&ts, idef), &evs),
    ];
    for (wanted, interface_members, members) in groups {
        let imember = named(&ts, interface_members, wanted);
        let (counts, implemented) = explicit_counts(&mut ts, members, wanted, None);
        assert_eq!(counts, [0, 1], "{wanted}");
        assert_eq!(single(implemented, wanted).def, imember);
        for which in 0..2 {
            let (counts, implemented) = explicit_counts(&mut ts, members, wanted, Some(which));
            assert_eq!(counts, [0, 1], "{wanted} accessor {which}");
            assert_eq!(
                single(implemented, wanted).def,
                accessors(&ts, imember)[which].unwrap()
            );
        }
    }
}
