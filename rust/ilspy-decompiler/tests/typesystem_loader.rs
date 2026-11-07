//! `ICSharpCode.Decompiler.Tests/TypeSystem/TypeSystemLoaderTests.cs`: the compiled test
//! assembly (`TypeSystemOptions.Default`) with the legacy `mscorlib`
//! (`Default | OnlyPublicAPI`), as the managed fixture sets them up.
//!
//! `compilation.FindType(typeof(X))` is expressed as `ParseReflectionName` of the CLR full name
//! of `X`.

mod common;

use ilspy_decompiler::metadata::tables::rows;
use ilspy_decompiler::typesystem::attributes::Attribute;
use ilspy_decompiler::typesystem::attrs::{accessibility_name, type_accessibility};
use ilspy_decompiler::typesystem::attrvalue::ArgValue;
use ilspy_decompiler::typesystem::compilation::{Compilation, ModuleId, TypeDefId, TypeKind};
use ilspy_decompiler::typesystem::flags::ScopedKind;
use ilspy_decompiler::typesystem::inheritance::{MemberDef, MemberRef, Modifiers};
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::lookup::MemberOptions;
use ilspy_decompiler::typesystem::members::{
    ConstValue, MethodDefId, ParamInfo, RefKind, SymbolKind, has_body, method_accessibility,
};
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::resolve::{MethodOrigin, ResolvedMethod};
use ilspy_decompiler::typesystem::types::{TypeData, TypeId, TypeParamOwner, TypeSystem};

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

fn attr_names(ts: &TypeSystem<'_, '_>, attrs: &[Attribute]) -> Vec<String> {
    attrs.iter().map(|a| ts.full_name(a.ty)).collect()
}

fn has_attr(ts: &TypeSystem<'_, '_>, attrs: &[Attribute], full_name: &str) -> bool {
    attr_names(ts, attrs).iter().any(|n| n == full_name)
}

const SPECIAL_NAME: &str = "System.Runtime.CompilerServices.SpecialNameAttribute";

fn member_attrs(ts: &mut TypeSystem<'_, '_>, m: MemberDef) -> Vec<Attribute> {
    match m {
        MemberDef::Method(id) => ts.method_attributes(id),
        MemberDef::Property(module, row) => ts.property_attributes(module, row),
        MemberDef::Event(module, row) => ts.event_attributes(module, row),
        MemberDef::Field(module, row) => ts.field_attributes(module, row),
    }
    .unwrap()
}

/// `IParameter.GetAttributes()` of parameter `index` of a method.
fn param_attrs(ts: &mut TypeSystem<'_, '_>, m: MethodDefId, index: usize) -> Vec<Attribute> {
    let params = ts.method_signature(m).1;
    let declaring = ts.method_declaring(m).unwrap();
    ts.parameter_attributes(m.module, &params, index, declaring)
        .unwrap()
}

fn single_param(ts: &mut TypeSystem<'_, '_>, m: MethodDefId) -> ParamInfo {
    single(ts.method_signature(m).1, "parameter")
}

fn field_named(ts: &TypeSystem<'_, '_>, def: TypeDefId, wanted: &str) -> MemberDef {
    let fields = ts
        .comp
        .visible_fields(def)
        .unwrap()
        .into_iter()
        .map(|r| MemberDef::Field(def.module, r))
        .collect();
    named(ts, fields, wanted)
}

fn string(s: &str) -> ArgValue {
    ArgValue::String(s.encode_utf16().collect())
}

fn assert_no_special_name_and_body(
    ts: &mut TypeSystem<'_, '_>,
    m: MemberDef,
    attribute_count: usize,
) {
    let attrs = member_attrs(ts, m);
    assert_eq!(attrs.len(), attribute_count, "{:?}", attr_names(ts, &attrs));
    assert!(!has_attr(ts, &attrs, SPECIAL_NAME));
    assert!(body(ts, m));
    assert_eq!(ts.accessor_owner(method_id(m)), None);
}

#[test]
fn simple_public_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "SimplePublicClass");
    let t = ts.intern_def(def);
    assert_eq!(
        comp.type_def(def).unwrap().full_name.name(),
        "SimplePublicClass"
    );
    assert_eq!(ts.full_name(t), format!("{NS}.SimplePublicClass"));
    assert_eq!(comp.type_def(def).unwrap().full_name.top.ns, NS);
    assert_eq!(ts.reflection_name(t), format!("{NS}.SimplePublicClass"));
    assert_eq!(type_accessibility_of(&ts, def), "Public");
    assert!(!is_abstract(&ts, def));
    assert!(!is_sealed(&ts, def));
    assert!(!ts.type_flags(def).is_static);
    let attrs = ts.type_attributes(def).unwrap();
    assert!(!has_attr(&ts, &attrs, SPECIAL_NAME));
}

#[test]
fn simple_public_class_method_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "SimplePublicClass");
    let m = method_named(&mut ts, def, "Method");
    let member = MemberDef::Method(m);
    assert_eq!(ts.method_declaring(m).unwrap(), Some(def));
    assert_eq!(access(&mut ts, member), "Public");
    assert_eq!(symbol(&mut ts, member), SymbolKind::Method);
    assert!(!mods(&ts, member).is_virtual);
    assert!(!mods(&ts, member).is_static);
    assert!(ts.method_signature(m).1.is_empty());
    assert_no_special_name_and_body(&mut ts, member, 0);
}

#[test]
fn simple_public_class_ctor_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "SimplePublicClass");
    let m = single(
        methods(&mut ts, def)
            .into_iter()
            .filter(|(_, k)| *k == SymbolKind::Constructor)
            .filter_map(|(m, _)| match m.origin {
                MethodOrigin::Def(id) => Some(id),
                _ => None,
            })
            .collect(),
        "constructor",
    );
    let member = MemberDef::Method(m);
    assert_eq!(name(&ts, member), ".ctor");
    assert_eq!(access(&mut ts, member), "Public");
    assert!(!mods(&ts, member).is_virtual);
    assert!(!mods(&ts, member).is_static);
    assert!(ts.method_signature(m).1.is_empty());
    assert_no_special_name_and_body(&mut ts, member, 0);
}

fn destructor_of(ts: &mut TypeSystem<'_, '_>, def: TypeDefId) -> MethodDefId {
    single(
        methods(ts, def)
            .into_iter()
            .filter(|(_, k)| *k == SymbolKind::Destructor)
            .filter_map(|(m, _)| match m.origin {
                MethodOrigin::Def(id) => Some(id),
                _ => None,
            })
            .collect(),
        "destructor",
    )
}

#[test]
fn simple_public_class_dtor_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "SimplePublicClass");
    let m = destructor_of(&mut ts, def);
    let member = MemberDef::Method(m);
    assert_eq!(name(&ts, member), "Finalize");
    assert_eq!(access(&mut ts, member), "Protected");
    assert!(!mods(&ts, member).is_virtual);
    assert!(!mods(&ts, member).is_static);
    assert!(ts.method_signature(m).1.is_empty());
    assert_no_special_name_and_body(&mut ts, member, 1);
}

#[test]
fn dtor_in_derived_class() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "Derived`2");
    let m = destructor_of(&mut ts, def);
    let member = MemberDef::Method(m);
    assert_eq!(name(&ts, member), "Finalize");
    assert_eq!(ts.method_declaring(m).unwrap(), Some(def));
    assert_eq!(access(&mut ts, member), "Protected");
    assert!(!mods(&ts, member).is_virtual);
    assert!(!mods(&ts, member).is_static);
    assert!(ts.method_signature(m).1.is_empty());
    assert_no_special_name_and_body(&mut ts, member, 0);
}

#[test]
fn private_finalize_method_is_not_a_dtor() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "TypeTestAttribute");
    let m = method_named(&mut ts, def, "Finalize");
    let member = MemberDef::Method(m);
    assert_eq!(access(&mut ts, member), "Private");
    assert_eq!(symbol(&mut ts, member), SymbolKind::Method);
    assert!(!mods(&ts, member).is_virtual);
    assert!(!mods(&ts, member).is_static);
    assert!(ts.method_signature(m).1.is_empty());
    assert_no_special_name_and_body(&mut ts, member, 0);
}

#[test]
fn dynamic_type() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "DynamicTest");
    let MemberDef::Field(module, row) = field_named(&ts, def, "DynamicField") else {
        unreachable!()
    };
    let (field_type, _) = ts.field_type(module, row);
    assert!(matches!(ts.data(field_type), TypeData::Dynamic));
    let p = single(properties(&ts, def), "property");
    let MemberDef::Property(module, row) = p else {
        unreachable!()
    };
    let (ret, _) = ts.property_signature(module, row).unwrap();
    assert!(matches!(ts.data(ret), TypeData::Dynamic));
    assert!(member_attrs(&mut ts, p).is_empty());
}

#[test]
fn dynamic_type_in_generics() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "DynamicTest");
    let m1 = method_named(&mut ts, def, "DynamicGenerics1");
    let (ret, params) = ts.method_signature(m1);
    assert_eq!(
        ts.reflection_name(ret),
        "System.Collections.Generic.List`1[[dynamic]]"
    );
    assert_eq!(
        ts.reflection_name(params[0].ty),
        "System.Action`3[[System.Object],[dynamic[]],[System.Object]]"
    );
    for (method, expected) in [
        (
            "DynamicGenerics2",
            "System.Action`3[[System.Object],[dynamic],[System.Object]]",
        ),
        (
            "DynamicGenerics3",
            "System.Action`3[[System.Int32],[dynamic],[System.Object]]",
        ),
        (
            "DynamicGenerics4",
            "System.Action`3[[System.Int32[]],[dynamic],[System.Object]]",
        ),
        (
            "DynamicGenerics5",
            "System.Action`3[[System.Int32*[]],[dynamic],[System.Object]]",
        ),
        (
            "DynamicGenerics6",
            "System.Action`3[[System.Object],[dynamic],[System.Object]]&",
        ),
        (
            "DynamicGenerics7",
            "System.Action`3[[System.Int32[][,]],[dynamic],[System.Object]]",
        ),
    ] {
        let m = method_named(&mut ts, def, method);
        let p = ts.method_signature(m).1[0].ty;
        assert_eq!(ts.reflection_name(p), expected, "{method}");
    }
}

#[test]
fn dynamic_parameter_has_no_attributes() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "DynamicTest");
    let m = method_named(&mut ts, def, "DynamicGenerics1");
    assert!(param_attrs(&mut ts, m, 0).is_empty());
}

#[test]
fn lifetime_annotations() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "LifetimeAnnotationTests");
    let p = |ts: &mut TypeSystem<'_, '_>, m: &str| {
        let id = method_named(ts, def, m);
        single_param(ts, id)
    };
    let explicit_ref = p(&mut ts, "ExplicitScopedRef");
    assert_eq!(explicit_ref.declared_scope, ScopedKind::ScopedRef);
    assert_eq!(explicit_ref.effective_scope, ScopedKind::ScopedRef);
    assert!(!explicit_ref.unscoped_ref);
    let explicit_value = p(&mut ts, "ExplicitScopedValue");
    assert_eq!(explicit_value.declared_scope, ScopedKind::ScopedValue);
    assert_eq!(explicit_value.effective_scope, ScopedKind::ScopedValue);
    assert!(!explicit_value.unscoped_ref);
    let implicit_ref = p(&mut ts, "ImplicitScopedRef");
    assert_eq!(implicit_ref.declared_scope, ScopedKind::None);
    assert_eq!(implicit_ref.effective_scope, ScopedKind::ScopedRef);
    assert!(!implicit_ref.unscoped_ref);
    let unscoped_method = method_named(&mut ts, def, "UnscopedRef");
    let unscoped = single_param(&mut ts, unscoped_method);
    assert_eq!(unscoped.declared_scope, ScopedKind::None);
    assert_eq!(unscoped.effective_scope, ScopedKind::None);
    assert!(unscoped.unscoped_ref);
    let attrs = param_attrs(&mut ts, unscoped_method, 0);
    assert!(has_attr(
        &ts,
        &attrs,
        "System.Diagnostics.CodeAnalysis.UnscopedRefAttribute"
    ));

    // A params parameter of a ref struct type is implicitly scoped (the rule
    // `new DefaultParameter(type, "values", isParams: true)` exercises).
    let ty = ts.intern_def(def);
    assert!(ts.is_ref_like_or_allows(ty));

    const UNSCOPED_REF: &str = "System.Diagnostics.CodeAnalysis.UnscopedRefAttribute";
    let instance = method_named(&mut ts, def, "InstanceMethod");
    assert_eq!(
        ts.method_flags(instance, None).this_scope,
        ScopedKind::ScopedRef
    );
    let attrs = member_attrs(&mut ts, MemberDef::Method(instance));
    assert!(!has_attr(&ts, &attrs, UNSCOPED_REF));
    let unscoped_method = method_named(&mut ts, def, "UnscopedMethod");
    assert_eq!(
        ts.method_flags(unscoped_method, None).this_scope,
        ScopedKind::None
    );
    let attrs = member_attrs(&mut ts, MemberDef::Method(unscoped_method));
    assert!(has_attr(&ts, &attrs, UNSCOPED_REF));

    let prop = named(&ts, properties(&ts, def), "UnscopedProperty");
    let get = method_id(getter(&ts, prop));
    let owner = ts.accessor_owner(get).map(|(o, _)| o);
    assert_eq!(ts.method_flags(get, owner).this_scope, ScopedKind::None);
    let attrs = member_attrs(&mut ts, prop);
    assert!(has_attr(&ts, &attrs, UNSCOPED_REF));
    let prop = named(&ts, properties(&ts, def), "AccessorUnscopedProperty");
    let get = method_id(getter(&ts, prop));
    let owner = ts.accessor_owner(get).map(|(o, _)| o);
    assert_eq!(ts.method_flags(get, owner).this_scope, ScopedKind::None);
    let attrs = member_attrs(&mut ts, MemberDef::Method(get));
    assert!(has_attr(&ts, &attrs, UNSCOPED_REF));

    let static_method = method_named(&mut ts, def, "ExplicitScopedRef");
    assert_eq!(
        ts.method_flags(static_method, None).this_scope,
        ScopedKind::None
    );
    let class = test_type(&mut ts, "SimplePublicClass");
    let class_method = method_named(&mut ts, class, "Method");
    assert_eq!(
        ts.method_flags(class_method, None).this_scope,
        ScopedKind::None
    );

    // The legacy mscorlib has no RefSafetyRulesAttribute, so out is not implicitly scoped.
    let int32 = type_def(&mut ts, "System.Int32");
    let try_parse = methods(&mut ts, int32)
        .into_iter()
        .filter_map(|(m, _)| match m.origin {
            MethodOrigin::Def(id) if m.name == "TryParse" => Some(id),
            _ => None,
        })
        .filter(|&id| {
            let params = ts.method_signature(id).1;
            params.len() == 2 && params[1].ref_kind == RefKind::Out
        })
        .collect();
    let try_parse = single(try_parse, "TryParse(string, out int)");
    assert_eq!(
        ts.method_signature(try_parse).1[1].effective_scope,
        ScopedKind::None
    );
}

#[test]
fn lifetime_annotations_can_be_disabled() {
    let comp = common::compilation(&[
        (
            common::test_assembly(),
            options::DEFAULT & !options::SCOPED_REF,
        ),
        (
            common::mscorlib(),
            options::DEFAULT | options::ONLY_PUBLIC_API,
        ),
    ]);
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "LifetimeAnnotationTests");
    let m = method_named(&mut ts, def, "ExplicitScopedRef");
    let p = single_param(&mut ts, m);
    assert_eq!(p.declared_scope, ScopedKind::None);
    assert_eq!(p.effective_scope, ScopedKind::None);
    let attrs = param_attrs(&mut ts, m, 0);
    assert!(has_attr(
        &ts,
        &attrs,
        "System.Runtime.CompilerServices.ScopedRefAttribute"
    ));
    let instance = method_named(&mut ts, def, "InstanceMethod");
    assert_eq!(ts.method_flags(instance, None).this_scope, ScopedKind::None);
}

fn main_assembly_attributes(ts: &mut TypeSystem<'_, '_>) -> Vec<Attribute> {
    ts.assembly_attributes(ModuleId(0)).unwrap()
}

#[test]
fn assembly_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attrs = main_assembly_attributes(&mut ts);
    let type_test = single(
        attrs
            .into_iter()
            .filter(|a| ts.full_name(a.ty) == format!("{NS}.TypeTestAttribute"))
            .collect(),
        "TypeTestAttribute",
    );
    assert_eq!(type_test.fixed.len(), 3);
    assert_eq!(type_test.fixed[0].value, ArgValue::Int32(42));
    let ArgValue::Type(ty) = type_test.fixed[1].value else {
        panic!("typeof(Action<>) is not a type");
    };
    assert!(!matches!(ts.data(ty), TypeData::Inst { .. }));
    assert_eq!(ts.full_name(ty), "System.Action");
    assert_eq!(ts.type_parameter_count(ty), 1);
    let ArgValue::Type(crt) = type_test.fixed[2].value else {
        panic!("typeof(IDictionary<,>) is not a type");
    };
    let TypeData::Inst { args, .. } = ts.data(crt).clone() else {
        panic!("not a parameterized type");
    };
    assert_eq!(ts.full_name(crt), "System.Collections.Generic.IDictionary");
    assert_eq!(ts.full_name(args[0]), "System.String");
    assert_eq!(ts.full_name(args[1]), "System.Collections.Generic.IList");
    let TypeData::Inst { args: inner, .. } = ts.data(args[1]).clone() else {
        panic!("IList<TestAttribute> is not parameterized");
    };
    let test_attribute = single(inner.to_vec(), "type argument");
    let TypeData::Unknown { name, .. } = ts.data(test_attribute) else {
        panic!("TestAttribute is not an unknown type");
    };
    assert_eq!(name.name(), "TestAttribute");
}

#[test]
fn type_forwarded_to_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attrs = main_assembly_attributes(&mut ts);
    let forward = single(
        attrs
            .into_iter()
            .filter(|a| {
                ts.full_name(a.ty) == "System.Runtime.CompilerServices.TypeForwardedToAttribute"
            })
            .collect(),
        "TypeForwardedToAttribute",
    );
    let arg = single(forward.fixed, "argument");
    let ArgValue::Type(rt) = arg.value else {
        panic!("not a type");
    };
    assert_eq!(ts.reflection_name(rt), "System.Func`2");
}

#[test]
fn property_with_private_setter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "PropertyTest");
    let p = named(&ts, properties(&ts, def), "PropertyWithPrivateSetter");
    let [get, set] = accessors(&ts, p).map(|a| a.expect("both accessors"));
    assert_eq!(access(&mut ts, p), "Public");
    assert_eq!(access(&mut ts, get), "Public");
    assert_eq!(access(&mut ts, set), "Private");
    assert!(body(&ts, get));
    for m in [p, get, set] {
        let attrs = member_attrs(&mut ts, m);
        assert!(!has_attr(&ts, &attrs, SPECIAL_NAME));
    }
}

#[test]
fn serializable_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let attrs = ts.type_attributes(def).unwrap();
    assert_eq!(attr_names(&ts, &attrs), ["System.SerializableAttribute"]);
}

#[test]
fn non_serialized_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let f = field_named(&ts, def, "NonSerializedField");
    let attrs = member_attrs(&mut ts, f);
    assert_eq!(attr_names(&ts, &attrs), ["System.NonSerializedAttribute"]);
}

#[test]
fn explicit_struct_layout_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ExplicitFieldLayoutStruct");
    let attr = single(ts.type_attributes(def).unwrap(), "attribute");
    assert_eq!(
        ts.full_name(attr.ty),
        "System.Runtime.InteropServices.StructLayoutAttribute"
    );
    let arg = single(attr.fixed, "fixed argument");
    assert_eq!(
        ts.full_name(arg.ty),
        "System.Runtime.InteropServices.LayoutKind"
    );
    assert_eq!(arg.value, ArgValue::Int32(2));
    let char_set = &attr.named[0];
    assert_eq!(char_set.name.as_deref(), Some("CharSet"));
    assert_eq!(
        ts.full_name(char_set.arg.ty),
        "System.Runtime.InteropServices.CharSet"
    );
    assert_eq!(char_set.arg.value, ArgValue::Int32(3));
    let pack = &attr.named[1];
    assert_eq!(pack.name.as_deref(), Some("Pack"));
    assert_eq!(ts.full_name(pack.arg.ty), "System.Int32");
    assert_eq!(pack.arg.value, ArgValue::Int32(8));
}

#[test]
fn field_offset_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ExplicitFieldLayoutStruct");
    for (field, offset) in [("Field0", 0), ("Field100", 100)] {
        let f = field_named(&ts, def, field);
        let attr = single(member_attrs(&mut ts, f), "attribute");
        assert_eq!(
            ts.full_name(attr.ty),
            "System.Runtime.InteropServices.FieldOffsetAttribute"
        );
        let arg = single(attr.fixed, "argument");
        assert_eq!(ts.full_name(arg.ty), "System.Int32");
        assert_eq!(arg.value, ArgValue::Int32(offset));
    }
}

fn dll_import(ts: &mut TypeSystem<'_, '_>, method: &str) -> Attribute {
    let def = test_type(ts, "NonCustomAttributes");
    let m = method_named(ts, def, method);
    let attr = single(member_attrs(ts, MemberDef::Method(m)), "attribute");
    assert_eq!(
        ts.full_name(attr.ty),
        "System.Runtime.InteropServices.DllImportAttribute"
    );
    assert_eq!(attr.fixed[0].value, string("unmanaged.dll"));
    attr
}

#[test]
fn dll_import_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attr = dll_import(&mut ts, "DllMethod");
    assert_eq!(
        single(attr.named, "named argument").arg.value,
        ArgValue::Int32(3)
    );
}

#[test]
fn dll_import_attribute_with_preserve_sig_false() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attr = dll_import(&mut ts, "DoNotPreserveSig");
    assert_eq!(
        single(attr.named, "named argument").arg.value,
        ArgValue::Bool(false)
    );
}

#[test]
fn preserve_sig_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let m = method_named(&mut ts, def, "PreserveSigAsAttribute");
    let attr = single(member_attrs(&mut ts, MemberDef::Method(m)), "attribute");
    assert_eq!(
        ts.full_name(attr.ty),
        "System.Runtime.InteropServices.PreserveSigAttribute"
    );
    assert!(attr.fixed.is_empty());
    assert!(attr.named.is_empty());
}

#[test]
fn in_out_parameters_on_ref_method() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let m = method_named(&mut ts, def, "DllMethod");
    let p = single_param(&mut ts, m);
    assert_eq!(p.ref_kind, RefKind::Ref);
    let attrs = param_attrs(&mut ts, m, 0);
    assert_eq!(
        attr_names(&ts, &attrs),
        [
            "System.Runtime.InteropServices.InAttribute",
            "System.Runtime.InteropServices.OutAttribute"
        ]
    );
}

#[test]
fn marshal_as_attribute_on_method() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let m = method_named(&mut ts, def, "DllMethod");
    let attr = single(ts.return_type_attributes(m).unwrap(), "return attribute");
    assert_eq!(single(attr.fixed, "argument").value, ArgValue::Int32(2));
}

/// The single parameter of `ParameterTests.<method>`, with its attributes.
fn parameter_test(ts: &mut TypeSystem<'_, '_>, method: &str) -> (ParamInfo, Vec<Attribute>) {
    let def = test_type(ts, "ParameterTests");
    let m = method_named(ts, def, method);
    let p = single_param(ts, m);
    let attrs = param_attrs(ts, m, 0);
    (p, attrs)
}

#[test]
fn method_with_out_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithOutParameter");
    assert!(!p.optional);
    assert_eq!(p.ref_kind, RefKind::Out);
    assert!(attrs.is_empty());
    assert!(is_by_ref(&ts, p.ty));
}

#[test]
fn method_with_ref_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithRefParameter");
    assert!(!p.optional);
    assert_eq!(p.ref_kind, RefKind::Ref);
    assert!(attrs.is_empty());
    assert!(is_by_ref(&ts, p.ty));
}

#[test]
fn method_with_in_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithInParameter");
    assert!(!p.optional);
    assert_eq!(p.ref_kind, RefKind::In);
    assert!(attrs.is_empty());
    assert!(is_by_ref(&ts, p.ty));
}

#[test]
fn method_with_params_array() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithParamsArray");
    assert!(!p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(p.is_params);
    assert!(attrs.is_empty());
    assert!(matches!(ts.data(p.ty), TypeData::Array { .. }));
}

#[test]
fn method_with_optional_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithOptionalParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    assert!(attrs.is_empty());
    assert_eq!(p.value, ConstValue::Int32(4));
}

#[test]
fn method_with_explicit_optional_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, _) = parameter_test(&mut ts, "MethodWithExplicitOptionalParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(!p.has_constant_in_signature);
}

#[test]
fn method_with_enum_optional_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithEnumOptionalParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    assert!(attrs.is_empty());
    // StringComparison.OrdinalIgnoreCase
    assert_eq!(p.value, ConstValue::Int32(5));
}

#[test]
fn method_with_optional_nullable_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, attrs) = parameter_test(&mut ts, "MethodWithOptionalNullableParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    assert!(attrs.is_empty());
    assert_eq!(p.value, ConstValue::Null);
}

#[test]
fn method_with_optional_long_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, _) = parameter_test(&mut ts, "MethodWithOptionalLongParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    assert_eq!(p.value, ConstValue::Int64(1));
}

#[test]
fn method_with_optional_nullable_long_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, _) = parameter_test(&mut ts, "MethodWithOptionalNullableLongParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    assert_eq!(p.value, ConstValue::Int64(1));
}

#[test]
fn method_with_optional_decimal_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (p, _) = parameter_test(&mut ts, "MethodWithOptionalDecimalParameter");
    assert!(p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(p.has_constant_in_signature);
    // 1M: lo 1, mid 0, hi 0, scale 0, positive
    assert_eq!(p.value, ConstValue::Decimal([1, 0, 0, 0]));
}

fn assert_arglist_parameter(ts: &TypeSystem<'_, '_>, p: &ParamInfo) {
    assert!(!p.optional);
    assert_eq!(p.ref_kind, RefKind::None);
    assert!(!p.is_params);
    assert!(matches!(ts.data(p.ty), TypeData::ArgList));
    assert_eq!(p.name, "");
}

#[test]
fn var_args_method() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ParameterTests");
    let m = method_named(&mut ts, def, "VarArgsMethod");
    let p = single_param(&mut ts, m);
    assert_arglist_parameter(&ts, &p);
}

#[test]
fn var_args_ctor() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "VarArgsCtor");
    let ctor = single(ts.definition_constructors(def).unwrap(), "constructor");
    let MethodOrigin::Def(id) = ctor.origin else {
        panic!("implicit constructor");
    };
    let p = single_param(&mut ts, id);
    assert_arglist_parameter(&ts, &p);
}

#[test]
fn com_interface_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "IAssemblyEnum");
    let attrs = ts.type_attributes(def).unwrap();
    let com_import = attr_names(&ts, &attrs)
        .into_iter()
        .filter(|n| n == "System.Runtime.InteropServices.ComImportAttribute")
        .count();
    assert_eq!(com_import, 1);
    let m = single(method_members(&mut ts, def, true), "method");
    assert_eq!(name(&ts, m), "GetNextAssembly");
    assert_eq!(access(&mut ts, m), "Public");
    let mods = mods(&ts, m);
    assert!(mods.is_abstract);
    assert!(!mods.is_virtual);
    assert!(!mods.is_sealed);
}

fn params_attribute_argument(index: usize) -> (String, ArgValue) {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ParamsAttribute");
    let attr = single(ts.type_attributes(def).unwrap(), "attribute");
    let ArgValue::Array(items) = single(attr.fixed, "argument").value else {
        panic!("not an array");
    };
    assert_eq!(items.len(), 5);
    let item = items[index].clone();
    (ts.full_name(item.ty), item.value)
}

#[test]
fn params_attribute_integer() {
    assert_eq!(
        params_attribute_argument(0),
        ("System.Int32".to_owned(), ArgValue::Int32(1))
    );
}

#[test]
fn params_attribute_enum() {
    // StringComparison.CurrentCulture
    assert_eq!(
        params_attribute_argument(1),
        ("System.StringComparison".to_owned(), ArgValue::Int32(0))
    );
}

#[test]
fn params_attribute_null_reference() {
    assert_eq!(params_attribute_argument(2).1, ArgValue::Null);
}

#[test]
fn params_attribute_double() {
    assert_eq!(
        params_attribute_argument(3),
        (
            "System.Double".to_owned(),
            ArgValue::Double(4.0f64.to_bits())
        )
    );
}

#[test]
fn params_attribute_string() {
    assert_eq!(
        params_attribute_argument(4),
        ("System.String".to_owned(), string("Test"))
    );
}

#[test]
fn params_attribute_property() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ParamsAttribute");
    let prop = named(&ts, properties(&ts, def), "Property");
    let attr = single(member_attrs(&mut ts, prop), "attribute");
    assert_eq!(ts.definition(attr.ty), Some(def));
    let ArgValue::Array(elements) = single(attr.fixed, "argument").value else {
        panic!("not an array");
    };
    assert!(elements.is_empty());
    let named_arg = single(attr.named, "named argument");
    assert_eq!(named_arg.name, Some(name(&ts, prop)));
    let ArgValue::Array(items) = named_arg.arg.value else {
        panic!("not an array");
    };
    assert_eq!(items.len(), 2);
}

#[test]
fn params_attribute_getter_return_type() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ParamsAttribute");
    let prop = named(&ts, properties(&ts, def), "Property");
    let get = getter(&ts, prop);
    assert!(member_attrs(&mut ts, get).is_empty());
    assert_eq!(ts.return_type_attributes(method_id(get)).unwrap().len(), 1);
}

#[test]
fn double_attribute_implicit_numeric_conversion() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "DoubleAttribute");
    let attr = single(ts.type_attributes(def).unwrap(), "attribute");
    let arg = single(attr.fixed, "argument");
    assert_eq!(ts.reflection_name(arg.ty), "System.Double");
    assert_eq!(arg.value, ArgValue::Double(1.0f64.to_bits()));
}

#[test]
fn attributes_using_nested_members() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithAttributesUsingNestedMembers");
    let nested = |ts: &TypeSystem<'_, '_>, outer: TypeDefId, wanted: &str| {
        let found = comp
            .nested_types(outer)
            .into_iter()
            .filter(|&n| ts.comp.type_def(n).unwrap().full_name.name() == wanted)
            .collect();
        single(found, wanted)
    };
    let inner = nested(&ts, def, "Inner");
    let my_attribute = nested(&ts, def, "MyAttribute");
    let check = |ts: &mut TypeSystem<'_, '_>,
                 attrs: Vec<Attribute>,
                 number: i32,
                 inner: TypeDefId,
                 my_attribute: TypeDefId| {
        let by_name = |ts: &TypeSystem<'_, '_>, wanted: &str| {
            let found = attrs
                .iter()
                .filter(|a| {
                    ts.definition(a.ty)
                        .is_some_and(|d| ts.comp.type_def(d).unwrap().full_name.name() == wanted)
                })
                .cloned()
                .collect();
            single(found, wanted)
        };
        let type_test = by_name(ts, "TypeTestAttribute");
        assert_eq!(type_test.fixed[0].value, ArgValue::Int32(number));
        let ArgValue::Type(t) = type_test.fixed[1].value else {
            panic!("not a type");
        };
        assert_eq!(ts.definition(t), Some(inner));
        assert_eq!(
            ts.definition(by_name(ts, "MyAttribute").ty),
            Some(my_attribute)
        );
    };
    let attrs = ts.type_attributes(def).unwrap();
    check(&mut ts, attrs, 42, inner, my_attribute);
    let prop = named(&ts, properties(&ts, def), "P");
    let attrs = member_attrs(&mut ts, prop);
    check(&mut ts, attrs, 42, inner, my_attribute);
    let attributed_inner = nested(&ts, def, "AttributedInner");
    let attrs = ts.type_attributes(attributed_inner).unwrap();
    check(&mut ts, attrs, 42, inner, my_attribute);
    let attributed_inner2 = nested(&ts, def, "AttributedInner2");
    let inner2 = nested(&ts, attributed_inner2, "Inner");
    let my_attribute2 = nested(&ts, attributed_inner2, "MyAttribute");
    let attrs = ts.type_attributes(attributed_inner2).unwrap();
    check(&mut ts, attrs, 43, inner2, my_attribute2);
}

#[test]
fn class_with_attribute_on_type_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ClassWithAttributeOnTypeParameter`1");
    let gp = single(
        comp.module(def.module)
            .md
            .generic_params(
                ilspy_decompiler::metadata::tables::TableId::TypeDef,
                def.row,
            )
            .collect(),
        "type parameter",
    );
    let attr = single(
        ts.type_parameter_attributes(def.module, gp).unwrap(),
        "attribute",
    );
    let attr_def = ts.definition(attr.ty).expect("attribute type");
    assert_eq!(
        comp.type_def(attr_def).unwrap().full_name.name(),
        "DoubleAttribute"
    );
}

fn void_attribute(ts: &mut TypeSystem<'_, '_>, full_name: &str) -> Attribute {
    let void = type_def(ts, "System.Void");
    let attrs = ts.type_attributes(void).unwrap();
    single(
        attrs
            .into_iter()
            .filter(|a| ts.full_name(a.ty) == full_name)
            .collect(),
        full_name,
    )
}

fn constructor_parameter_count(ts: &mut TypeSystem<'_, '_>, a: &Attribute) -> usize {
    let ctor = match &a.ctor {
        Some(c) => c.clone(),
        None => ts.builtin_constructor(a).unwrap().expect("constructor"),
    };
    ctor.params().unwrap().len()
}

#[test]
fn void_serializable_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attr = void_attribute(&mut ts, "System.SerializableAttribute");
    assert_eq!(constructor_parameter_count(&mut ts, &attr), 0);
    assert!(attr.fixed.is_empty());
    assert!(attr.named.is_empty());
}

#[test]
fn void_struct_layout_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attr = void_attribute(
        &mut ts,
        "System.Runtime.InteropServices.StructLayoutAttribute",
    );
    assert_eq!(constructor_parameter_count(&mut ts, &attr), 1);
    assert_eq!(
        single(attr.fixed.clone(), "argument").value,
        ArgValue::Int32(0)
    );
    let size = single(attr.named, "named argument");
    assert_eq!(size.name.as_deref(), Some("Size"));
    assert_eq!(size.arg.value, ArgValue::Int32(1));
}

#[test]
fn void_com_visible_attribute() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let attr = void_attribute(
        &mut ts,
        "System.Runtime.InteropServices.ComVisibleAttribute",
    );
    assert_eq!(constructor_parameter_count(&mut ts, &attr), 1);
    assert_eq!(
        single(attr.fixed.clone(), "argument").value,
        ArgValue::Bool(true)
    );
    assert!(attr.named.is_empty());
}

#[test]
fn has_special_name() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "NonCustomAttributes");
    let method = MemberDef::Method(method_named(&mut ts, def, "SpecialNameMethod"));
    let property = named(&ts, properties(&ts, def), "SpecialNameProperty");
    let event = named(&ts, events(&ts, def), "SpecialNameEvent");
    let field = field_named(&ts, def, "SpecialNameField");
    for m in [method, property, event, field] {
        let attrs = member_attrs(&mut ts, m);
        assert!(has_attr(&ts, &attrs, SPECIAL_NAME), "{}", name(&ts, m));
    }
    for nested in ["SpecialNameClass", "SpecialNameStruct"] {
        let found = comp
            .nested_types(def)
            .into_iter()
            .filter(|&n| comp.type_def(n).unwrap().full_name.name() == nested)
            .collect();
        let t = single(found, nested);
        let attrs = ts.type_attributes(t).unwrap();
        assert!(has_attr(&ts, &attrs, SPECIAL_NAME), "{nested}");
    }
}

fn assert_constant_field(
    ts: &mut TypeSystem<'_, '_>,
    def: TypeDefId,
    field: &str,
    expected: ConstValue,
) {
    let f = field_named(ts, def, field);
    let MemberDef::Field(module, row) = f else {
        unreachable!()
    };
    assert!(ts.field_is_const(module, row), "{field}");
    assert_eq!(ts.field_constant(module, row), expected, "{field}");
    assert!(member_attrs(ts, f).is_empty(), "{field}");
}

#[test]
fn constant_fields_created_with_new() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ConstantFieldTest");
    for (field, value) in [
        ("CNewb", ConstValue::Byte(0)),
        ("CNewsb", ConstValue::SByte(0)),
        ("CNewc", ConstValue::Char(0)),
        ("CNews", ConstValue::Int16(0)),
        ("CNewus", ConstValue::UInt16(0)),
        ("CNewi", ConstValue::Int32(0)),
        ("CNewui", ConstValue::UInt32(0)),
        ("CNewl", ConstValue::Int64(0)),
        ("CNewul", ConstValue::UInt64(0)),
        ("CNewd", ConstValue::Double(0f64.to_bits())),
        ("CNewf", ConstValue::Single(0f32.to_bits())),
        ("CNewm", ConstValue::Decimal([0, 0, 0, 0])),
    ] {
        assert_constant_field(&mut ts, def, field, value);
    }
}

#[test]
fn constant_fields_size_of() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "ConstantFieldTest");
    for (field, size) in [
        ("SOsb", 1),
        ("SOb", 1),
        ("SOs", 2),
        ("SOus", 2),
        ("SOi", 4),
        ("SOui", 4),
        ("SOl", 8),
        ("SOul", 8),
        ("SOc", 2),
        ("SOf", 4),
        ("SOd", 8),
        ("SObl", 1),
        ("SOe", 2),
    ] {
        assert_constant_field(&mut ts, def, field, ConstValue::Int32(size));
    }
}

fn find(ts: &mut TypeSystem<'_, '_>, reflection_name: &str) -> TypeId {
    ts.parse_reflection_name(reflection_name)
        .unwrap_or_else(|_| panic!("{reflection_name} does not parse"))
}

fn test_find(ts: &mut TypeSystem<'_, '_>, name: &str) -> TypeId {
    find(ts, &format!("{NS}.{name}"))
}

fn resolved_accessibility(ts: &TypeSystem<'_, '_>, m: &ResolvedMethod) -> &'static str {
    match m.origin {
        MethodOrigin::Def(id) => accessibility_name(ts.method_accessibility_of(id).unwrap()),
        // The implicit constructor of a struct is public.
        MethodOrigin::DummyConstructor(_) => "Public",
        MethodOrigin::Fake => panic!("unresolved method"),
    }
}

fn nested_names(ts: &mut TypeSystem<'_, '_>, t: TypeId) -> Vec<String> {
    ts.get_nested_types(t, None, MemberOptions::default())
        .unwrap()
        .into_iter()
        .map(|n| ts.reflection_name(n))
        .collect()
}

#[test]
fn get_nested_types_from_base_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let d = test_type(&mut ts, "Derived`2");
    let base = single(ts.direct_base_types(d).unwrap(), "base type");
    assert_eq!(ts.reflection_name(base), format!("{NS}.Base`1[[`1]]"));
    let expected = [format!("{NS}.Base`1+Nested`1[[`1],[]]")];
    assert_eq!(nested_names(&mut ts, base), expected);
    let derived = ts.intern_def(d);
    assert_eq!(nested_names(&mut ts, derived), expected);
}

#[test]
fn parameterized_type_get_nested_types_from_base_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let d = test_find(&mut ts, "Derived`2[[System.String],[System.Int32]]");
    assert_eq!(
        nested_names(&mut ts, d),
        [format!("{NS}.Base`1+Nested`1[[System.Int32],[]]")]
    );
}

fn constructors_of(ts: &mut TypeSystem<'_, '_>, name: &str) -> Vec<ResolvedMethod> {
    let t = test_find(ts, name);
    ts.get_constructors(t, MemberOptions::DECLARED).unwrap()
}

#[test]
fn default_constructor_added_to_struct() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let ctors = constructors_of(&mut ts, "MyStructWithCtor");
    assert_eq!(ctors.len(), 2);
    for c in &ctors {
        assert!(!c.is_static);
        assert_eq!(ts.reflection_name(c.ret), "System.Void");
        assert_eq!(resolved_accessibility(&ts, c), "Public");
    }
}

#[test]
fn no_default_constructor_added_to_struct() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let ctors = constructors_of(&mut ts, "MyStructWithDefaultCtor");
    assert_eq!(ctors.len(), 1);
    for c in &ctors {
        assert!(!c.is_static);
        assert_eq!(ts.reflection_name(c.ret), "System.Void");
        assert_eq!(resolved_accessibility(&ts, c), "Public");
    }
}

#[test]
fn no_default_constructor_added_to_class() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let ctor = single(constructors_of(&mut ts, "MyClassWithCtor"), "constructor");
    assert_eq!(resolved_accessibility(&ts, &ctor), "Private");
    assert_eq!(ctor.params().unwrap().len(), 1);
}

#[test]
fn default_constructor_on_abstract_class_is_protected() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let ctor = single(constructors_of(&mut ts, "AbstractClass"), "constructor");
    assert!(ctor.params().unwrap().is_empty());
    assert_eq!(resolved_accessibility(&ts, &ctor), "Protected");
}

#[test]
fn generic_property_getter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(&mut ts, "GenericClass`2[[System.String],[System.Object]]");
    let prop = single(
        ts.get_properties(t, MemberOptions::default())
            .unwrap()
            .into_iter()
            .filter(|p| name(&ts, p.def) == "Property")
            .collect(),
        "Property",
    );
    let getter_def = method_id(getter(&ts, prop.def));
    let get = single(
        ts.get_accessors(t, MemberOptions::default())
            .unwrap()
            .into_iter()
            .flatten()
            .filter(|m| m.origin == MethodOrigin::Def(getter_def))
            .collect(),
        "getter",
    );
    assert_eq!(ts.reflection_name(get.ret), "System.String");
    assert_ne!(get.accessor_kind, 0);
    let MemberDef::Property(_, row) = prop.def else {
        unreachable!()
    };
    assert_eq!(
        ts.accessor_owner(getter_def).map(|(o, _)| o),
        Some(0x1700_0000 | row)
    );
    assert_eq!(get.declaring, Some(prop.declaring));
}

#[test]
fn generic_delegate_get_invoke_method() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(
        &mut ts,
        "GenericDelegate`2[[System.String],[System.Object]]",
    );
    let m = ts.get_delegate_invoke_method(t).unwrap().expect("Invoke");
    assert_eq!(m.name, "Invoke");
    assert_eq!(ts.full_name(m.ret), "System.Object");
    assert_eq!(ts.full_name(m.params().unwrap()[0]), "System.String");
}

#[test]
fn specialized_method_accessor_owner() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(&mut ts, "GenericClass`2[[System.String],[System.Object]]");
    let method = single(
        ts.get_methods(t, None, MemberOptions::default())
            .unwrap()
            .into_iter()
            .filter(|m| m.name == "GetIndex")
            .collect(),
        "GetIndex",
    );
    assert_eq!(method.accessor_kind, 0);
}

#[test]
fn explicit_implementation_of_unified_methods() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(
        &mut ts,
        "ExplicitGenericInterfaceImplementationWithUnifiableMethods`2[[System.Int32],[System.Int32]]",
    );
    let TypeData::Inst { args, .. } = ts.data(t).clone() else {
        unreachable!()
    };
    let mut explicit_methods = Vec::new();
    for m in ts.get_methods(t, None, MemberOptions::default()).unwrap() {
        if let MethodOrigin::Def(id) = m.origin
            && ts.is_explicit_interface_implementation(id).unwrap()
        {
            explicit_methods.push((m, id));
        }
    }
    assert_eq!(explicit_methods.len(), 2);
    for (method, id) in explicit_methods {
        // A specialized method's implemented members are the definition's, specialized alike.
        let implemented = ts.explicit_implementations(id).unwrap();
        let interface_definition = single(implemented, "implemented method");
        let interface_method =
            ts.specialize(interface_definition.clone(), Some(args.to_vec()), None);
        assert_eq!(
            ts.reflection_name(single(method.params().unwrap(), "parameter")),
            "System.Int32"
        );
        assert_eq!(
            ts.reflection_name(single(interface_method.params().unwrap(), "parameter")),
            "System.Int32"
        );
        let generic_param = single(ts.method_signature(id).1, "parameter").ty;
        let MethodOrigin::Def(interface_id) = interface_definition.origin else {
            panic!("unresolved interface method");
        };
        let interface_generic_param = single(ts.method_signature(interface_id).1, "parameter").ty;
        assert!(matches!(ts.data(generic_param), TypeData::TypeParam { .. }));
        assert!(matches!(
            ts.data(interface_generic_param),
            TypeData::TypeParam { .. }
        ));
        assert_eq!(
            ts.reflection_name(interface_generic_param),
            ts.reflection_name(generic_param)
        );
    }
}

#[test]
fn marshal_tests() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(&mut ts, "IMarshalAsTests");
    let count = ts
        .get_methods(t, None, MemberOptions::default())
        .unwrap()
        .into_iter()
        .filter(|m| m.name == "GetCollectionByQuery2")
        .count();
    assert_eq!(count, 1);
}

#[test]
fn nested_class_in_generic_class_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let value_collection = type_def(
        &mut ts,
        "System.Collections.Generic.Dictionary`2+ValueCollection",
    );
    let dictionary = find(
        &mut ts,
        "System.Collections.Generic.Dictionary`2[[System.String],[System.Int32]]",
    );
    let values = single(
        ts.get_properties(dictionary, MemberOptions::default())
            .unwrap()
            .into_iter()
            .filter(|p| name(&ts, p.def) == "Values")
            .collect(),
        "Values",
    );
    let MemberDef::Property(module, row) = values.def else {
        unreachable!()
    };
    let TypeData::Inst { args, .. } = ts.data(values.declaring).clone() else {
        panic!("not seen through Dictionary<string, int>");
    };
    let (ret, _) = ts.property_signature(module, row).unwrap();
    let ret = ts.substitute(ret, Some(&args), None);
    assert_eq!(
        ts.reflection_name(ret),
        "System.Collections.Generic.Dictionary`2+ValueCollection[[System.String],[System.Int32]]"
    );
    assert_eq!(ts.definition(ret), Some(value_collection));
}

#[test]
fn test_nullable_default_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let t = test_find(&mut ts, "ClassWithMethodThatHasNullableDefaultParameter");
    let foo = single(
        ts.get_methods(t, None, MemberOptions::default())
            .unwrap()
            .into_iter()
            .filter(|m| m.name == "Foo")
            .collect(),
        "Foo",
    );
    let MethodOrigin::Def(id) = foo.origin else {
        panic!("unresolved Foo");
    };
    assert_eq!(single_param(&mut ts, id).value, ConstValue::Int32(42));
}

#[test]
fn void_has_no_members() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let void = type_def(&mut ts, "System.Void");
    assert_eq!(comp.type_def(void).unwrap().kind, TypeKind::Void);
    let t = ts.intern_def(void);
    let all = MemberOptions::default();
    assert!(ts.get_methods(t, None, all).unwrap().is_empty());
    assert!(ts.get_properties(t, all).unwrap().is_empty());
    assert!(ts.get_events(t, all).unwrap().is_empty());
    assert!(ts.get_fields(t, all).unwrap().is_empty());
}

fn type_params(
    ts: &mut TypeSystem<'_, '_>,
    owner: MemberDef,
) -> Vec<ilspy_decompiler::typesystem::members::TypeParamInfo> {
    use ilspy_decompiler::metadata::tables::TableId;
    match owner {
        MemberDef::Method(id) => ts.type_parameters(id.module, TableId::MethodDef, id.row),
        _ => panic!("not a method"),
    }
    .unwrap()
}

fn class_type_params(
    ts: &mut TypeSystem<'_, '_>,
    def: TypeDefId,
) -> Vec<ilspy_decompiler::typesystem::members::TypeParamInfo> {
    ts.type_parameters(
        def.module,
        ilspy_decompiler::metadata::tables::TableId::TypeDef,
        def.row,
    )
    .unwrap()
}

fn type_param(ts: &mut TypeSystem<'_, '_>, owner: TypeParamOwner, index: u32) -> TypeId {
    ts.intern(TypeData::TypeParam { owner, index })
}

#[test]
fn test_class_type_parameters() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericClass`2");
    let tps = class_type_params(&mut ts, def);
    assert_eq!(ts.class_param_owner(def, 0), TypeParamOwner::Type(def));
    assert_eq!(ts.class_param_owner(def, 1), TypeParamOwner::Type(def));
    let second = type_param(&mut ts, TypeParamOwner::Type(def), 1);
    assert_eq!(tps[0].constraints[0], second);
}

#[test]
fn test_method() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericClass`2");
    let m = method_named(&mut ts, def, "TestMethod");
    let tps = type_params(&mut ts, MemberDef::Method(m));
    assert_eq!(tps[0].name, "K");
    assert_eq!(tps[1].name, "V");
    assert_eq!(
        ts.reflection_name(tps[0].constraints[0]),
        "System.IComparable`1[[``1]]"
    );
    let first = type_param(&mut ts, TypeParamOwner::Method(m), 0);
    assert_eq!(tps[1].constraints[0], first);
}

#[test]
fn get_index() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericClass`2");
    let m = method_named(&mut ts, def, "GetIndex");
    let tps = type_params(&mut ts, MemberDef::Method(m));
    assert_eq!(tps[0].name, "T");
    let t = type_param(&mut ts, TypeParamOwner::Method(m), 0);
    let constraint = tps[0].constraints[0];
    let TypeData::Inst { generic, args } = ts.data(constraint).clone() else {
        panic!("constraint is not parameterized");
    };
    let generic = ts.definition(generic).expect("IEquatable`1");
    assert_eq!(
        comp.type_def(generic).unwrap().full_name.name(),
        "IEquatable"
    );
    assert_eq!(comp.type_def(generic).unwrap().type_parameter_count, 1);
    assert_eq!(&args[..], [t]);
    assert_eq!(ts.method_signature(m).1[0].ty, t);
}

#[test]
fn generic_enum() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericClass`2+NestedEnum");
    assert_eq!(comp.type_def(def).unwrap().type_parameter_count, 2);
}

fn self_parameterized(
    ts: &mut TypeSystem<'_, '_>,
    generic: TypeDefId,
    params_of: TypeDefId,
) -> TypeId {
    let count = ts.comp.type_def(params_of).unwrap().type_parameter_count;
    let args: Vec<TypeId> = (0..count)
        .map(|i| {
            let owner = ts.class_param_owner(params_of, i);
            type_param(ts, owner, i)
        })
        .collect();
    let generic = ts.intern_def(generic);
    ts.intern(TypeData::Inst {
        generic,
        args: args.into(),
    })
}

#[test]
fn field_in_generic_class_with_nested_enum_type() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let class = test_type(&mut ts, "GenericClass`2");
    let enum_class = test_type(&mut ts, "GenericClass`2+NestedEnum");
    let MemberDef::Field(module, row) = field_named(&ts, class, "EnumField") else {
        unreachable!()
    };
    let expected = self_parameterized(&mut ts, enum_class, class);
    assert_eq!(ts.field_type(module, row).0, expected);
}

#[test]
fn generic_enum_member_return_type() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let enum_class = test_type(&mut ts, "GenericClass`2+NestedEnum");
    let MemberDef::Field(module, row) = field_named(&ts, enum_class, "EnumMember") else {
        unreachable!()
    };
    let expected = self_parameterized(&mut ts, enum_class, enum_class);
    assert_eq!(ts.field_type(module, row).0, expected);
}

#[test]
fn enum_fields_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let e = test_type(&mut ts, "MyEnum");
    let rows = comp.visible_fields(e).unwrap();
    let md = &comp.module(e.module).md;
    let flags = |row: u32| md.get::<rows::Field>(row).unwrap().flags;
    let value_field = rows[0];
    let fields = &rows[1..];
    assert_eq!(fields.len(), 5);
    assert_eq!(
        name(&ts, MemberDef::Field(e.module, value_field)),
        "value__"
    );
    let int16 = type_def(&mut ts, "System.Int16");
    let int16 = ts.intern_def(int16);
    assert_eq!(ts.field_type(e.module, value_field).0, int16);
    assert_eq!(
        access(&mut ts, MemberDef::Field(e.module, value_field)),
        "Public"
    );
    assert_eq!(ts.field_constant(e.module, value_field), ConstValue::Null);
    assert!(!ts.field_is_const(e.module, value_field));
    assert_eq!(flags(value_field) & 0x10, 0);
    let enum_type = ts.intern_def(e);
    for &f in fields {
        assert_ne!(flags(f) & 0x10, 0);
        assert!(ts.field_is_const(e.module, f));
        assert_eq!(access(&mut ts, MemberDef::Field(e.module, f)), "Public");
        assert_eq!(ts.field_type(e.module, f).0, enum_type);
        assert!(matches!(
            ts.field_constant(e.module, f),
            ConstValue::Int16(_)
        ));
    }
    let expected = [
        ("First", 0),
        ("Second", 1),
        ("Flag1", 0x10),
        ("Flag2", 0x20),
        ("CombinedFlags", 0x30),
    ];
    for (&f, (n, v)) in fields.iter().zip(expected) {
        assert_eq!(name(&ts, MemberDef::Field(e.module, f)), n);
        assert_eq!(ts.field_constant(e.module, f), ConstValue::Int16(v));
    }
}

fn constant_of(ts: &mut TypeSystem<'_, '_>, field: &str) -> ConstValue {
    let def = test_type(ts, "ConstantFieldTest");
    let MemberDef::Field(module, row) = field_named(ts, def, field) else {
        unreachable!()
    };
    assert!(ts.field_is_const(module, row), "{field}");
    ts.field_constant(module, row)
}

#[test]
fn constant_enum_from_this_assembly() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    assert_eq!(
        constant_of(&mut ts, "EnumFromThisAssembly"),
        ConstValue::Int16(1)
    );
}

#[test]
fn constant_enum_from_another_assembly() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    // StringComparison.OrdinalIgnoreCase
    assert_eq!(
        constant_of(&mut ts, "EnumFromAnotherAssembly"),
        ConstValue::Int32(5)
    );
}

#[test]
fn default_of_enum() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    assert_eq!(constant_of(&mut ts, "DefaultOfEnum"), ConstValue::Int16(0));
}

#[test]
fn generic_delegate_variance() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericDelegate`2");
    let tps = class_type_params(&mut ts, def);
    assert_eq!(tps[0].variance, "Contravariant");
    assert_eq!(tps[1].variance, "Covariant");
    let second = type_param(&mut ts, TypeParamOwner::Type(def), 1);
    assert_eq!(tps[0].constraints.first(), Some(&second));
}

#[test]
fn inner_class_in_generic_class_is_referenced_using_parameterized_type() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "OuterGeneric`1");
    let inner = format!("{NS}.OuterGeneric`1+Inner[[`0]]");
    for (field, expected) in [
        ("Field1", inner.clone()),
        ("Field2", inner.clone()),
        ("Field3", format!("{NS}.OuterGeneric`1+Inner[[{inner}]]")),
    ] {
        let MemberDef::Field(module, row) = field_named(&ts, def, field) else {
            unreachable!()
        };
        let ty = ts.field_type(module, row).0;
        assert_eq!(ts.reflection_name(ty), expected, "{field}");
    }
}

#[test]
fn inner_class_in_generic_class_type_parameter_owner() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let outer = test_type(&mut ts, "OuterGeneric`1");
    let inner = test_type(&mut ts, "OuterGeneric`1+Inner");
    assert_eq!(ts.class_param_owner(inner, 0), TypeParamOwner::Type(outer));
}

#[test]
fn inner_class_in_generic_class_references_the_outer_class_field() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let inner = test_type(&mut ts, "OuterGeneric`1+Inner");
    let fields = comp.visible_fields(inner).unwrap();
    let f = single(fields, "field");
    let ty = ts.field_type(inner.module, f).0;
    assert_eq!(ts.reflection_name(ty), format!("{NS}.OuterGeneric`1[[`0]]"));
}

#[test]
fn inner_class_in_generic_class_references_the_outer_class_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let inner = test_type(&mut ts, "OuterGeneric`1+Inner");
    let ctor = single(ts.definition_constructors(inner).unwrap(), "constructor");
    let ty = single(ctor.params().unwrap(), "parameter");
    assert_eq!(ts.reflection_name(ty), format!("{NS}.OuterGeneric`1[[`0]]"));
}

#[test]
fn pointer_type_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.IntPtr");
    let m = method_named(&mut ts, def, "ToPointer");
    let ret = ts.method_signature(m).0;
    assert_eq!(ts.reflection_name(ret), "System.Void*");
    let TypeData::Pointer(elem) = *ts.data(ret) else {
        panic!("not a pointer");
    };
    assert_eq!(ts.full_name(elem), "System.Void");
}

#[test]
fn generic_property_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Collections.Generic.Comparer`1");
    let MemberDef::Property(module, row) = named(&ts, properties(&ts, def), "Default") else {
        unreachable!()
    };
    let (ret, _) = ts.property_signature(module, row).unwrap();
    let TypeData::Inst { args, .. } = ts.data(ret).clone() else {
        panic!("not parameterized");
    };
    assert_eq!(ts.full_name(ret), "System.Collections.Generic.Comparer");
    let t = type_param(&mut ts, TypeParamOwner::Type(def), 0);
    assert_eq!(args[0], t);
}

#[test]
fn inner_class_reference_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = type_def(&mut ts, "System.Environment");
    let get_folder_path = methods(&mut ts, def)
        .into_iter()
        .find_map(|(m, _)| match m.origin {
            MethodOrigin::Def(id) if m.name == "GetFolderPath" => Some(id),
            _ => None,
        })
        .expect("GetFolderPath");
    let rt = ts.method_signature(get_folder_path).1[0].ty;
    let special_folder = single(
        comp.nested_types(def)
            .into_iter()
            .filter(|&n| comp.type_def(n).unwrap().full_name.name() == "SpecialFolder")
            .collect(),
        "SpecialFolder",
    );
    assert_eq!(ts.definition(rt), Some(special_folder));
    assert!(matches!(ts.data(rt), TypeData::Def(_)));
}

#[test]
fn extension_method_test() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "StaticClass");
    let m = method_named(&mut ts, def, "Extension");
    assert!(mods(&ts, MemberDef::Method(m)).is_static);
    assert!(ts.method_flags(m, None).extension);
    // `ReducedFrom` is null on a definition; the Rust type system has no reduced forms.
    assert!(ts.type_flags(def).has_extensions);
}

#[test]
fn constraints_on_override_are_inherited() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "Derived`2");
    let m = method_named(&mut ts, def, "GenericMethodWithConstraints");
    let tp = single(type_params(&mut ts, MemberDef::Method(m)), "type parameter");
    assert_eq!(tp.name, "Y");
    assert!(!tp.value_type);
    assert!(!tp.reference_type);
    assert!(tp.default_ctor);
    let names: Vec<String> = tp
        .constraints
        .iter()
        .map(|&c| ts.reflection_name(c))
        .collect();
    assert_eq!(
        names,
        [
            "System.Collections.Generic.IComparer`1[[`1]]",
            "System.Object"
        ]
    );
}

// Specialized members have no identity of their own in the Rust type system: their type
// parameters are the definition's (`TypeParamOwner::Method` of the MethodDef), where the
// managed tests check `Owner` with `Is.SameAs` against the specialized method.

fn get_index_definition(ts: &mut TypeSystem<'_, '_>) -> (MethodDefId, ResolvedMethod) {
    let def = test_type(ts, "GenericClass`2");
    let id = method_named(ts, def, "GetIndex");
    (id, ts.method_definition(id).unwrap())
}

fn assert_get_index_type_parameter(
    ts: &mut TypeSystem<'_, '_>,
    id: MethodDefId,
    m: &ResolvedMethod,
) {
    let tps = ts.method_type_parameters(m).unwrap();
    assert_eq!(tps[0].name, "T");
    let t = type_param(ts, TypeParamOwner::Method(id), 0);
    assert_eq!(m.type_args[0], t);
    let TypeData::Inst { generic, args } = ts.data(tps[0].constraints[0]).clone() else {
        panic!("constraint is not parameterized");
    };
    let generic = ts.definition(generic).expect("IEquatable`1");
    assert_eq!(
        ts.comp.type_def(generic).unwrap().full_name.name(),
        "IEquatable"
    );
    assert_eq!(ts.comp.type_def(generic).unwrap().type_parameter_count, 1);
    assert_eq!(&args[..], [t]);
    assert_eq!(m.params().unwrap()[0], t);
}

#[test]
fn get_index_specialized_type_parameter() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (id, definition) = get_index_definition(&mut ts);
    let int16 = ts.known_type(KnownTypeCode::Int16);
    let int32 = ts.known_type(KnownTypeCode::Int32);
    let m = ts
        .specialize_method(&definition, &(Some(vec![int16, int32]), None))
        .unwrap();
    assert_get_index_type_parameter(&mut ts, id, &m);
}

#[test]
fn get_index_double_specialization() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericClass`2");
    let (_, definition) = get_index_definition(&mut ts);
    let a = type_param(&mut ts, TypeParamOwner::Type(def), 0);
    let b = type_param(&mut ts, TypeParamOwner::Type(def), 1);
    let int32 = ts.known_type(KnownTypeCode::Int32);
    let string = ts.known_type(KnownTypeCode::String);
    // GenericClass<B, A>.GetIndex<A>
    let m1 = ts
        .specialize_method(&definition, &(Some(vec![b, a]), Some(vec![a])))
        .unwrap();
    // GenericClass<string, int>.GetIndex<int>
    let m2 = ts
        .specialize_method(&m1, &(Some(vec![int32, string]), None))
        .unwrap();
    let m12 = ts
        .specialize_method(&definition, &(Some(vec![string, int32]), Some(vec![int32])))
        .unwrap();
    assert!(ts.same_method(&m2, &m12), "{m2:?} != {m12:?}");
}

fn generic_class_get_index(
    ts: &mut TypeSystem<'_, '_>,
    method_args: Option<&[TypeId]>,
) -> ResolvedMethod {
    let t = test_find(ts, "GenericClass`2[[System.String],[System.Object]]");
    single(
        ts.get_methods(t, method_args, MemberOptions::default())
            .unwrap()
            .into_iter()
            .filter(|m| m.name == "GetIndex")
            .collect(),
        "GetIndex",
    )
}

#[test]
fn specialized_get_index_to_member_reference() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (id, _) = get_index_definition(&mut ts);
    let method = generic_class_get_index(&mut ts, None);
    assert!(method.class_args.is_some(), "not specialized");
    assert_get_index_type_parameter(&mut ts, id, &method);
    let own: Vec<TypeId> = (0..ts.method_type_parameters(&method).unwrap().len() as u32)
        .map(|i| type_param(&mut ts, TypeParamOwner::Method(id), i))
        .collect();
    assert_eq!(method.type_args, own);
}

#[test]
fn specialized_get_index_specialize_with_identity_has_no_effect() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let (id, _) = get_index_definition(&mut ts);
    let dummy = type_param(&mut ts, TypeParamOwner::DummyMethod, 0);
    // GenericClass<string, object>.GetIndex<!!0>()
    let method = generic_class_get_index(&mut ts, Some(&[dummy]));
    let own = type_param(&mut ts, TypeParamOwner::Method(id), 0);
    assert_ne!(method.type_args[0], own);
    assert_eq!(method.type_args[0], dummy);
    let method2 = ts.specialize_method(&method, &(None, None)).unwrap();
    assert_ne!(method2.type_args[0], own);
    assert_eq!(method2.type_args[0], dummy);
    assert!(ts.same_method(&method2, &method));
}

#[test]
fn generic_delegate_reference_type_constraints() {
    let comp = loader();
    let mut ts = TypeSystem::new(&comp);
    let def = test_type(&mut ts, "GenericDelegate`2");
    let tps = class_type_params(&mut ts, def);
    assert!(!tps[0].reference_type);
    assert!(tps[1].reference_type);
    let t0 = type_param(&mut ts, TypeParamOwner::Type(def), 0);
    let t1 = type_param(&mut ts, TypeParamOwner::Type(def), 1);
    assert_eq!(ts.is_reference_type(t0), None);
    assert_eq!(ts.is_reference_type(t1), Some(true));
}
