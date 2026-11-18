//! `GetFullAssemblyName` and `IsDirectImportOf` over the legacy `System.Core` and `mscorlib`.

mod common;

use ilspy_decompiler::metadata::tables::TableId;
use ilspy_decompiler::typesystem::assembly_identity::{
    assembly_ref_full_name, full_assembly_name, is_direct_import_of,
};
use ilspy_decompiler::typesystem::compilation::{ModuleId, TypeDefId};
use ilspy_decompiler::typesystem::names::{FullTypeName, TopLevelTypeName};
use ilspy_decompiler::typesystem::options;

const MSCORLIB: &str =
    "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

fn compilation() -> ilspy_decompiler::typesystem::compilation::Compilation<'static> {
    common::compilation(&[
        (common::system_core(), options::DEFAULT),
        (common::mscorlib(), options::DEFAULT),
    ])
}

fn find(
    comp: &ilspy_decompiler::typesystem::compilation::Compilation<'_>,
    module: ModuleId,
    ns: &str,
    name: &str,
) -> (TypeDefId, FullTypeName) {
    let full = FullTypeName::top_level(TopLevelTypeName::new(ns, name, 0));
    let def = comp
        .find_type_definition(module, &full, &mut Vec::new())
        .expect("metadata")
        .expect("the type");
    (def, full)
}

#[test]
fn full_names_carry_the_public_key_token() {
    let comp = compilation();
    // The assembly definition hashes its public key; the reference stores the token.
    assert_eq!(
        full_assembly_name(&comp.module(ModuleId(1)).md).as_deref(),
        Some(MSCORLIB)
    );
    let core = &comp.module(ModuleId(0)).md;
    let names: Vec<String> = (1..=core.tables.rows(TableId::AssemblyRef))
        .filter_map(|row| assembly_ref_full_name(core, row))
        .collect();
    assert!(names.iter().any(|n| n == MSCORLIB), "{names:?}");
}

#[test]
fn a_referenced_assembly_is_a_direct_import() {
    let comp = compilation();
    let (object, name) = find(&comp, ModuleId(1), "System", "Object");
    assert!(is_direct_import_of(&comp, object, &name, ModuleId(0)));
    let (func, name) = find(&comp, ModuleId(0), "System.Linq", "Enumerable");
    assert!(!is_direct_import_of(&comp, func, &name, ModuleId(1)));
}
