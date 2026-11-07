//! Reads every fixture assembly completely: every column of every row of every table, the
//! full listing (signatures, lookups, method bodies) and every custom attribute value.
//!
//! Fixtures come from `ILSPY_FIXTURE_DIR` (a path list). By default they are the compiled test
//! cases under `ICSharpCode.Decompiler.Tests/TestCases` (present after a run of
//! `ICSharpCode.Decompiler.Tests`) plus the newest shared framework of the `dotnet` on `PATH`,
//! so a fresh checkout still has a corpus. The test fails when it finds no assemblies.

use std::path::{Path, PathBuf};

use ilspy_decompiler::metadata::bytes::Bytes;
use ilspy_decompiler::metadata::custom_attribute::{self, ModuleResolver};
use ilspy_decompiler::metadata::signature;
use ilspy_decompiler::metadata::tables::{Coded, Col, TableId, rows};
use ilspy_decompiler::metadata::{Error, Metadata, listing};

fn fixture_dirs() -> Vec<PathBuf> {
    match std::env::var_os("ILSPY_FIXTURE_DIR") {
        Some(dirs) => std::env::split_paths(&dirs).collect(),
        None => {
            let mut dirs = vec![
                Path::new(env!("CARGO_MANIFEST_DIR"))
                    .join("../../ICSharpCode.Decompiler.Tests/TestCases"),
            ];
            dirs.extend(newest_shared_framework());
            dirs
        }
    }
}

/// `<dotnet root>/shared/Microsoft.NETCore.App/<newest version>` of the `dotnet` on `PATH`.
fn newest_shared_framework() -> Option<PathBuf> {
    let exe = if cfg!(windows) {
        "dotnet.exe"
    } else {
        "dotnet"
    };
    let path = std::env::var_os("PATH")?;
    let root = std::env::split_paths(&path)
        .map(|dir| dir.join(exe))
        .find(|candidate| candidate.is_file())
        .and_then(|exe| std::fs::canonicalize(exe).ok())?
        .parent()?
        .to_path_buf();
    let mut versions: Vec<PathBuf> = std::fs::read_dir(root.join("shared/Microsoft.NETCore.App"))
        .ok()?
        .flatten()
        .map(|e| e.path())
        .filter(|p| p.is_dir())
        .collect();
    // Lexical order is enough to pick one stable directory; it need not be the newest release.
    versions.sort();
    versions.pop()
}

fn collect(dir: &Path, out: &mut Vec<PathBuf>) {
    let Ok(entries) = std::fs::read_dir(dir) else {
        return;
    };
    for entry in entries.flatten() {
        let path = entry.path();
        if path.is_dir() {
            collect(&path, out);
        } else if path
            .extension()
            .and_then(|e| e.to_str())
            .is_some_and(|e| e.eq_ignore_ascii_case("dll") || e.eq_ignore_ascii_case("exe"))
        {
            out.push(path);
        }
    }
}

/// Checks every cell; returns a description of the first bad one.
fn check_cells(md: &Metadata<'_>) -> Result<(), String> {
    let t = &md.tables;
    for &table in TableId::ALL {
        for row in 1..=t.rows(table) {
            for (col, &kind) in table.columns().iter().enumerate() {
                let value = t.cell(table, row, col).map_err(|e| e.to_string())?;
                let ok = match kind {
                    Col::U16 | Col::U32 => true,
                    Col::Str => md
                        .strings
                        .get(ilspy_decompiler::metadata::heaps::StrIdx(value))
                        .is_ok(),
                    Col::Blob => md
                        .blobs
                        .get(ilspy_decompiler::metadata::heaps::BlobIdx(value))
                        .is_ok(),
                    Col::Guid => md
                        .guids
                        .get(ilspy_decompiler::metadata::heaps::GuidIdx(value))
                        .is_ok(),
                    // List columns may point one past the end of the target table.
                    Col::Table(target) => value <= t.rows(target) + 1,
                    Col::Coded(kind) => Coded { kind, raw: value }
                        .decode()
                        .is_ok_and(|(target, r)| r <= t.rows(target)),
                };
                if !ok {
                    return Err(format!(
                        "{} row {row} column {col} = {value:#x}",
                        table.name()
                    ));
                }
            }
        }
    }
    Ok(())
}

/// Older F# compilers put field signatures into StandAloneSig. System.Reflection.Metadata
/// rejects those (the listing prints `!bad`, like the C# side); the blob itself must still be
/// a valid field signature.
fn is_field_sig_in_stand_alone_sig(md: &Metadata<'_>, line: &str) -> bool {
    let Some(rest) = line.strip_prefix("STANDALONESIG 11") else {
        return false;
    };
    let Ok(row) = u32::from_str_radix(&rest[..6], 16) else {
        return false;
    };
    md.get::<rows::StandAloneSig>(row)
        .and_then(|r| md.blob(r.signature))
        .and_then(|blob| signature::decode_field(&mut Bytes::new(blob)))
        .is_ok()
}

#[test]
fn every_fixture_assembly_reads_completely() {
    let mut files = Vec::new();
    for dir in fixture_dirs() {
        collect(&dir, &mut files);
    }
    assert!(
        !files.is_empty(),
        "no fixture assemblies under {:?}; run ICSharpCode.Decompiler.Tests first or set ILSPY_FIXTURE_DIR",
        fixture_dirs()
    );

    let (mut read, mut not_managed, mut attributes, mut unresolved_enums) = (0, 0, 0usize, 0usize);
    let mut failures = Vec::new();
    for path in &files {
        let image = std::fs::read(path).expect("fixture is readable");
        let md = match Metadata::parse(&image) {
            Ok(md) => md,
            Err(Error::NotManaged) => {
                not_managed += 1;
                continue;
            }
            Err(e) => {
                failures.push(format!("{}: {e}", path.display()));
                continue;
            }
        };
        read += 1;
        if let Err(e) = check_cells(&md) {
            failures.push(format!("{}: {e}", path.display()));
        }
        let text = listing::listing(&image);
        if let Some(line) = text
            .lines()
            .find(|l| l.contains("!bad") && !is_field_sig_in_stand_alone_sig(&md, l))
        {
            failures.push(format!("{}: {line}", path.display()));
        }
        let resolver = ModuleResolver::new(&md);
        for row in 1..=md.tables.rows(TableId::CustomAttribute) {
            let ca = md.get::<rows::CustomAttribute>(row).expect("row exists");
            attributes += 1;
            match custom_attribute::decode(&md, &ca, &resolver) {
                Ok(_) => {}
                Err(Error::Unsupported(_)) => unresolved_enums += 1,
                Err(e) => failures.push(format!("{}: custom attribute {row}: {e}", path.display())),
            }
        }
    }
    eprintln!(
        "{read} assemblies read, {not_managed} not managed, {attributes} custom attributes \
         ({unresolved_enums} need an enum from another assembly)"
    );
    assert!(
        read > 0,
        "no managed assemblies among {} files",
        files.len()
    );
    assert!(
        failures.is_empty(),
        "{} failures:\n{}",
        failures.len(),
        failures.join("\n")
    );
}
