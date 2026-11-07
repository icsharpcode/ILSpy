//! A canonical listing of the main module's type definitions as the type system sees them.
//!
//! `ICSharpCode.Decompiler.Rust.Tests/TypeSystemListingEqualityTests.cs` produces the same format from the
//! link-compiled `DecompilerTypeSystem` (`TypeSystemOptions.None`); `TypeSystemListingEquality`
//! diffs the two. Any change here must be mirrored there.
//!
//! One line per TypeDef row:
//! `T <reflection name> kind=<kind> known=<code> decl=<name|-> tparams=<n> enum=<type|-> base=[a|b]`
//! where each type is `<reflection name>@<defining assembly or ?>`.
//! A part that fails to decode prints `!bad`.

use super::compilation::{Compilation, ModuleId, TypeDefId, TypeKind};
use super::types::{TypeData, TypeSystem};
use crate::metadata::tables::TableId;

pub fn listing(comp: &Compilation<'_>) -> String {
    let mut ts = TypeSystem::new(comp);
    let main = ModuleId(0);
    let mut out = String::new();
    for row in 1..=comp.module(main).md.tables.rows(TableId::TypeDef) {
        let def = TypeDefId { module: main, row };
        let line = match comp.type_def(def) {
            Err(_) => "T !bad".to_owned(),
            Ok(info) => {
                let decl = match info.declaring {
                    0 => "-".to_owned(),
                    outer => {
                        let outer = ts.intern(TypeData::Def(TypeDefId {
                            module: main,
                            row: outer,
                        }));
                        ts.reflection_name(outer)
                    }
                };
                let underlying = match (info.kind, info.enum_underlying) {
                    (TypeKind::Enum, Some(code)) => {
                        let t = ts.known_type(code);
                        located(&ts, t)
                    }
                    _ => "-".to_owned(),
                };
                let bases = match ts.direct_base_types(def) {
                    Ok(bases) => {
                        let names: Vec<String> = bases.iter().map(|b| located(&ts, *b)).collect();
                        format!("[{}]", names.join("|"))
                    }
                    Err(_) => "!bad".to_owned(),
                };
                format!(
                    "T {} kind={} known={} decl={decl} tparams={} enum={underlying} base={bases}",
                    info.full_name.reflection_name(),
                    info.kind.name(),
                    info.known.name(),
                    info.type_parameter_count
                )
            }
        };
        out.push_str(&line);
        out.push('\n');
    }
    out
}

/// `name@assembly` of the defining module, `name@?` when the type resolved to nothing.
fn located(ts: &TypeSystem<'_, '_>, t: super::types::TypeId) -> String {
    let module = ts
        .definition(t)
        .map_or("?", |def| ts.comp.module(def.module).assembly_name.as_str());
    format!("{}@{module}", ts.reflection_name(t))
}
