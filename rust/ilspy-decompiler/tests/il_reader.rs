//! The IL reader over every method body of the compiled test assembly. Equality with the
//! managed reader is checked by the shim's `IlAstReadEquality`; these tests keep the reader
//! total (no errors, no panics) and its structure plausible without the .NET side.

mod common;

use ilspy_decompiler::il::ast::{Inst, VariableKind};
use ilspy_decompiler::il::reader::{ReaderOptions, read_il};
use ilspy_decompiler::metadata::tables::{TableId, rows};
use ilspy_decompiler::typesystem::compilation::ModuleId;
use ilspy_decompiler::typesystem::members::MethodDefId;
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::types::TypeSystem;

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
