//! The IL reader over every method body of the compiled test assembly. Equality with the
//! managed reader is checked by the shim's `IlAstReadEquality`; these tests keep the reader
//! total (no errors, no panics) and its structure plausible without the .NET side.

mod common;

use ilspy_decompiler::il::ast::{Inst, VariableKind};
use ilspy_decompiler::il::debug_info::{DebugInfo, ExtraTypeInfo};
use ilspy_decompiler::il::reader::{ReaderOptions, read_il};
use ilspy_decompiler::metadata::tables::{TableId, rows};
use ilspy_decompiler::typesystem::compilation::ModuleId;
use ilspy_decompiler::typesystem::known::KnownTypeCode;
use ilspy_decompiler::typesystem::members::MethodDefId;
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::types::{TypeData, TypeSystem};

#[test]
fn reads_every_body_of_the_test_assembly() {
    let comp = common::compilation(&[
        (common::test_assembly(), options::DEFAULT),
        (common::mscorlib(), options::DEFAULT),
    ]);
    let mut ts = TypeSystem::new(&comp);
    let module = ModuleId(0);
    let md = &comp.module(module).md;
    let (mut read, mut try_catches, mut stack_slots) = (0, 0, 0);
    for row in 1..=md.tables.rows(TableId::MethodDef) {
        let rva = md.get::<rows::MethodDef>(row).unwrap().rva;
        if rva == 0 {
            continue;
        }
        let body = md.method_body(rva).unwrap();
        let options = ReaderOptions {
            use_ref_locals_for_accurate_order_of_evaluation: true,
            debug_info: None,
        };
        let r = read_il(&mut ts, MethodDefId { module, row }, body, options)
            .unwrap_or_else(|e| panic!("MethodDef row {row}: {e}"));
        let f = &r.function;
        let root = f.body.expect("function body");
        try_catches += f
            .descendants_post(root)
            .iter()
            .filter(|&&n| matches!(f.inst(n), Inst::TryCatch { .. }))
            .count();
        for &v in &r.variables {
            let var = f.var(v);
            if var.kind == VariableKind::StackSlot {
                assert!(
                    var.name.starts_with("S_"),
                    "row {row}: stack slot {}",
                    var.name
                );
                stack_slots += 1;
            }
        }
        read += 1;
    }
    assert!(read > 1000, "only {read} bodies");
    assert!(try_catches > 0);
    assert!(stack_slots > 0);
}

/// Names every local `n{index}` except local 0, whose whitespace name counts as missing, and
/// marks every local's type as dynamic.
struct FakeDebugInfo;

impl DebugInfo for FakeDebugInfo {
    fn local_name(&self, _: u32, index: i32) -> Option<String> {
        Some(if index == 0 {
            " ".to_owned()
        } else {
            format!("n{index}")
        })
    }

    fn extra_type_info(&self, _: u32, _: i32) -> Option<ExtraTypeInfo> {
        Some(ExtraTypeInfo {
            tuple_element_names: None,
            dynamic_flags: Some(vec![true; 16]),
        })
    }
}

#[test]
fn debug_info_names_and_types_the_locals() {
    let comp = common::compilation(&[
        (common::test_assembly(), options::DEFAULT),
        (common::mscorlib(), options::DEFAULT),
    ]);
    let mut ts = TypeSystem::new(&comp);
    let module = ModuleId(0);
    let md = &comp.module(module).md;
    let object = ts.known_type(KnownTypeCode::Object);
    let (mut named, mut generated, mut dynamic) = (0, 0, 0);
    for row in 1..=md.tables.rows(TableId::MethodDef) {
        let rva = md.get::<rows::MethodDef>(row).unwrap().rva;
        if rva == 0 {
            continue;
        }
        let body = md.method_body(rva).unwrap();
        let plain = ReaderOptions::default();
        let with_symbols = ReaderOptions {
            debug_info: Some(&FakeDebugInfo),
            ..plain
        };
        let id = MethodDefId { module, row };
        let without = read_il(&mut ts, id, md.method_body(rva).unwrap(), plain).unwrap();
        let r = read_il(&mut ts, id, body, with_symbols).unwrap();
        for (&v, &w) in r.variables.iter().zip(&without.variables) {
            let (var, plain_var) = (r.function.var(v), without.function.var(w));
            if !matches!(var.kind, VariableKind::Local | VariableKind::PinnedLocal) {
                continue;
            }
            let index = var.index.unwrap();
            if index == 0 {
                assert_eq!(var.name, "V_0");
                assert!(var.has_generated_name);
                generated += 1;
            } else {
                assert_eq!(var.name, format!("n{index}"));
                assert!(!var.has_generated_name);
                named += 1;
            }
            if plain_var.ty == object {
                assert!(matches!(ts.data(var.ty), TypeData::Dynamic), "row {row}");
                dynamic += 1;
            }
        }
    }
    assert!(named > 0 && generated > 0 && dynamic > 0);
}

/// The served IL transform prefix over every body: no panic, and every variable a connected
/// instruction uses is in the function's variable list (which the wire writer indexes).
#[test]
fn early_transforms_run_on_every_body() {
    use ilspy_decompiler::il::transforms::{Context, Settings, run_prefix};
    let comp = common::compilation(&[
        (common::test_assembly(), options::DEFAULT),
        (common::mscorlib(), options::DEFAULT),
    ]);
    let mut ts = TypeSystem::new(&comp);
    let module = ModuleId(0);
    let md = &comp.module(module).md;
    let mut split = 0;
    for row in 1..=md.tables.rows(TableId::MethodDef) {
        let rva = md.get::<rows::MethodDef>(row).unwrap().rva;
        if rva == 0 {
            continue;
        }
        let body = md.method_body(rva).unwrap();
        let id = MethodDefId { module, row };
        let mut r = read_il(&mut ts, id, body, ReaderOptions::default()).unwrap();
        let before = r.variables.len();
        let mut ctx = Context {
            ts: &mut ts,
            settings: Settings::default(),
        };
        run_prefix(&mut r, &mut ctx, ilspy_decompiler::IL_PREFIX);
        let listed: std::collections::HashSet<_> = r.variables.iter().copied().collect();
        let root = r.function.body.expect("body");
        for n in r.function.descendants_post(root) {
            if let Some(v) = r.function.variable_of(n) {
                assert!(
                    listed.contains(&v),
                    "row {row}: {} is used but not listed",
                    r.function.var(v).name
                );
            }
        }
        split += usize::from(r.variables.len() != before);
    }
    assert!(split > 0, "no function changed its variables");
}
