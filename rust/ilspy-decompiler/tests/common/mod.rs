//! Fixtures shared by the suites ported from `ICSharpCode.Decompiler.Tests/TypeSystem`.
//!
//! - The legacy reference assemblies (`mscorlib.dll`, `System.Core.dll`) come from
//!   `ILSPY_LEGACY_REFERENCE_DIR`, by default `ILSpy-tests/dotnet/legacy` (the `ILSpy-tests`
//!   submodule, as for the managed suites).
//! - The compiled `ICSharpCode.Decompiler.Tests.dll` (whose
//!   `TypeSystem/TypeSystemLoaderTests.TestCase.cs` types the loader suite inspects) comes from
//!   `ILSPY_TYPESYSTEM_TEST_ASSEMBLY`, by default the newest build under
//!   `ICSharpCode.Decompiler.Tests/bin`.
//!
//! A missing fixture fails the test with the path it looked for.

#![allow(dead_code)]

use std::path::{Path, PathBuf};

use ilspy_decompiler::metadata::Metadata;
use ilspy_decompiler::metadata::tables::{TableId, rows};
use ilspy_decompiler::typesystem::compilation::{Compilation, ModuleId, ModuleInput};

fn repo_root() -> PathBuf {
    Path::new(env!("CARGO_MANIFEST_DIR")).join("../..")
}

fn existing(path: PathBuf, hint: &str) -> PathBuf {
    assert!(
        path.is_file(),
        "fixture {} not found ({hint})",
        path.display()
    );
    path
}

fn legacy(name: &str) -> PathBuf {
    let dir = std::env::var_os("ILSPY_LEGACY_REFERENCE_DIR")
        .map(PathBuf::from)
        .unwrap_or_else(|| repo_root().join("ILSpy-tests/dotnet/legacy"));
    existing(
        dir.join(name),
        "initialize the ILSpy-tests submodule or set ILSPY_LEGACY_REFERENCE_DIR",
    )
}

pub fn mscorlib() -> PathBuf {
    legacy("mscorlib.dll")
}

pub fn system_core() -> PathBuf {
    legacy("System.Core.dll")
}

fn find_newest(dir: &Path, file: &str, best: &mut Option<(std::time::SystemTime, PathBuf)>) {
    let Ok(entries) = std::fs::read_dir(dir) else {
        return;
    };
    for entry in entries.flatten() {
        let path = entry.path();
        if path.is_dir() {
            find_newest(&path, file, best);
        } else if path.file_name().is_some_and(|n| n == file)
            && let Ok(modified) = entry.metadata().and_then(|m| m.modified())
            && best.as_ref().is_none_or(|(t, _)| modified > *t)
        {
            *best = Some((modified, path));
        }
    }
}

pub fn test_assembly() -> PathBuf {
    const NAME: &str = "ICSharpCode.Decompiler.Tests.dll";
    if let Some(path) = std::env::var_os("ILSPY_TYPESYSTEM_TEST_ASSEMBLY") {
        return existing(PathBuf::from(path), "ILSPY_TYPESYSTEM_TEST_ASSEMBLY");
    }
    let bin = repo_root().join("ICSharpCode.Decompiler.Tests/bin");
    let mut best = None;
    find_newest(&bin, NAME, &mut best);
    match best {
        Some((_, path)) => path,
        None => existing(
            bin.join(NAME),
            "build ICSharpCode.Decompiler.Tests or set ILSPY_TYPESYSTEM_TEST_ASSEMBLY",
        ),
    }
}

fn assembly_name(md: &Metadata<'_>) -> String {
    if md.tables.rows(TableId::Assembly) == 0 {
        return String::new();
    }
    md.get::<rows::Assembly>(1)
        .and_then(|a| md.string(a.name).map(|s| s.into_owned()))
        .expect("assembly name")
}

/// A compilation of `modules` (main module first), each with its `TypeSystemOptions`.
/// AssemblyRefs resolve by simple name among the given modules, as a host whose reference
/// closure is exactly this set would resolve them.
pub fn compilation(modules: &[(PathBuf, u32)]) -> Compilation<'static> {
    let images: Vec<&'static [u8]> = modules
        .iter()
        .map(|(path, _)| &*Vec::leak(std::fs::read(path).expect("read fixture")))
        .collect();
    let names: Vec<String> = images
        .iter()
        .map(|image| assembly_name(&Metadata::parse(image).expect("metadata")))
        .collect();
    let inputs = images
        .iter()
        .zip(modules)
        .zip(&names)
        .map(|((image, (_, options)), name)| {
            let md = Metadata::parse(image).expect("metadata");
            let assembly_refs = (1..=md.tables.rows(TableId::AssemblyRef))
                .map(|row| {
                    let r = md.get::<rows::AssemblyRef>(row).expect("assembly ref");
                    let target = md.string(r.name).expect("assembly ref name");
                    names
                        .iter()
                        .position(|n| n.eq_ignore_ascii_case(&target))
                        .map(|i| ModuleId(i as u32))
                })
                .collect();
            ModuleInput {
                image,
                assembly_name: name.clone(),
                assembly_refs,
                module_refs: vec![None; md.tables.rows(TableId::ModuleRef) as usize],
                options: *options,
            }
        })
        .collect();
    Compilation::new(inputs).expect("compilation")
}
