//! Per-stage benchmarks of the engine over real assemblies (RustPort_HumanLecture.md,
//! "Performance comparison methodology").
//!
//! Inputs come from the environment, separated by the platform's path separator:
//! - `ILSPY_BENCH_INPUTS`: the assemblies to measure, e.g. `ICSharpCode.Decompiler.dll`,
//!   `System.Reflection.Metadata.dll` 9.0.0 (net462) and a net10 assembly, as the AOT gate uses;
//! - `ILSPY_BENCH_REFS` (optional): directories to resolve their references in by simple name,
//!   after each input's own directory (e.g. the `Microsoft.NETCore.App.Ref` pack).
//!
//! Without `ILSPY_BENCH_INPUTS` every bench is skipped with a message.
//!
//! Per input the stages are: metadata parsing, the compilation (type system load), reading
//! every method body into the ILAst, the IL transforms (`IL_PREFIX` of them), the C# body
//! translation (expression building) and the wire serialization of the transformed functions.
//! Each stage is timed alone: what it consumes is produced outside the measured time. The
//! settings are the `DecompilerSettings` defaults as far as `SETTING_NAMES` covers them, with no
//! host resolver (no extension methods) and no debug info.

use std::collections::{HashMap, HashSet, VecDeque};
use std::hint::black_box;
use std::panic::{self, AssertUnwindSafe};
use std::path::{Path, PathBuf};
use std::time::{Duration, Instant};

use criterion::{Criterion, criterion_group, criterion_main};
use ilspy_decompiler::il::reader::{ReadFunction, ReaderOptions, read_il};
use ilspy_decompiler::il::transforms::{self, Context, SETTING_NAMES, Settings};
use ilspy_decompiler::metadata::Metadata;
use ilspy_decompiler::metadata::tables::{TableId, rows};
use ilspy_decompiler::typesystem::compilation::{Compilation, ModuleId, ModuleInput};
use ilspy_decompiler::typesystem::members::MethodDefId;
use ilspy_decompiler::typesystem::options;
use ilspy_decompiler::typesystem::types::TypeSystem;
use ilspy_decompiler::wire::ilast::ModuleWriter;

fn env_paths(name: &str) -> Vec<PathBuf> {
    std::env::var_os(name)
        .map(|v| {
            std::env::split_paths(&v)
                .filter(|p| !p.as_os_str().is_empty())
                .collect()
        })
        .unwrap_or_default()
}

/// `DecompilerSettings` defaults: every setting the engine reads is on by default except these.
fn default_settings() -> Settings {
    const OFF: [&str; 3] = [
        "AggressiveInlining",
        "AggressiveScalarReplacementOfAggregates",
        "AlwaysQualifyMemberReferences",
    ];
    let (mut low, mut high) = (u64::MAX, u64::MAX);
    for (i, name) in SETTING_NAMES.iter().enumerate() {
        if OFF.contains(name) {
            match i.checked_sub(64) {
                None => low &= !(1 << i),
                Some(j) => high &= !(1 << j),
            }
        }
    }
    Settings::from_bits(low, high)
}

fn assembly_name(md: &Metadata<'_>) -> String {
    if md.tables.rows(TableId::Assembly) == 0 {
        return String::new();
    }
    md.get::<rows::Assembly>(1)
        .ok()
        .and_then(|a| md.string(a.name).ok().map(|s| s.into_owned()))
        .unwrap_or_default()
}

fn reference_names(md: &Metadata<'_>) -> Vec<String> {
    (1..=md.tables.rows(TableId::AssemblyRef))
        .map(|row| {
            md.get::<rows::AssemblyRef>(row)
                .ok()
                .and_then(|r| md.string(r.name).ok().map(|s| s.into_owned()))
                .unwrap_or_default()
        })
        .collect()
}

/// The images of `main` and the transitive closure of its references found in `dirs`, main first.
fn load_closure(main: &Path, dirs: &[PathBuf]) -> Vec<&'static [u8]> {
    let find = |name: &str| {
        dirs.iter()
            .map(|d| d.join(format!("{name}.dll")))
            .find(|p| p.is_file())
    };
    let mut images: Vec<&'static [u8]> = Vec::new();
    let mut seen = HashSet::new();
    let mut queue = VecDeque::from([main.to_path_buf()]);
    while let Some(path) = queue.pop_front() {
        let Ok(bytes) = std::fs::read(&path) else {
            continue;
        };
        let image: &'static [u8] = Vec::leak(bytes);
        let Ok(md) = Metadata::parse(image) else {
            continue;
        };
        if !seen.insert(assembly_name(&md).to_ascii_lowercase()) {
            continue;
        }
        for name in reference_names(&md) {
            if !seen.contains(&name.to_ascii_lowercase())
                && let Some(p) = find(&name)
            {
                queue.push_back(p);
            }
        }
        images.push(image);
    }
    images
}

/// The compilation of `images` (main first); AssemblyRefs resolve by simple name among them.
fn compilation(images: &[&'static [u8]]) -> Compilation<'static> {
    let mds: Vec<Metadata<'static>> = images
        .iter()
        .map(|i| Metadata::parse(i).expect("metadata"))
        .collect();
    let index: HashMap<String, u32> = mds
        .iter()
        .enumerate()
        .map(|(i, md)| (assembly_name(md).to_ascii_lowercase(), i as u32))
        .collect();
    let inputs = images
        .iter()
        .zip(&mds)
        .map(|(image, md)| ModuleInput {
            image,
            assembly_name: assembly_name(md),
            assembly_refs: reference_names(md)
                .iter()
                .map(|n| index.get(&n.to_ascii_lowercase()).map(|&i| ModuleId(i)))
                .collect(),
            module_refs: vec![None; md.tables.rows(TableId::ModuleRef) as usize],
            options: options::DEFAULT,
        })
        .collect();
    Compilation::new(inputs).expect("compilation")
}

const READER: ReaderOptions<'static> = ReaderOptions {
    use_ref_locals_for_accurate_order_of_evaluation: true,
    debug_info: None,
};

fn method(row: u32) -> MethodDefId {
    MethodDefId {
        module: ModuleId(0),
        row,
    }
}

fn read(ts: &mut TypeSystem<'_, '_>, row: u32) -> Option<ReadFunction> {
    let comp = ts.comp;
    let md = &comp.module(ModuleId(0)).md;
    let body = md
        .method_body(md.get::<rows::MethodDef>(row).ok()?.rva)
        .ok()?;
    read_il(ts, method(row), body, READER).ok()
}

fn transform(ts: &mut TypeSystem<'_, '_>, f: &mut ReadFunction, settings: Settings) {
    let mut ctx = Context {
        ts,
        settings,
        reader: READER,
        rerun_requested: false,
        removed_variables: Vec::new(),
        resolver: None,
        root_names: None,
    };
    transforms::run_prefix(f, &mut ctx, ilspy_decompiler::IL_PREFIX);
}

fn read_all(ts: &mut TypeSystem<'_, '_>, bodies: &[u32]) -> Vec<(u32, ReadFunction)> {
    bodies
        .iter()
        .filter_map(|&row| read(ts, row).map(|f| (row, f)))
        .collect()
}

fn transform_all(
    ts: &mut TypeSystem<'_, '_>,
    bodies: &[u32],
    s: Settings,
) -> Vec<(u32, ReadFunction)> {
    let mut fs = read_all(ts, bodies);
    for (_, f) in &mut fs {
        transform(ts, f, s);
    }
    fs
}

/// The MethodDef rows whose body is read, transformed and serialized without an error or a
/// panic, and the subset of them the engine also translates into C# without a host (the
/// others it leaves to the managed side as unsupported); the rest are left out.
fn servable_bodies(comp: &Compilation<'_>, settings: Settings) -> (Vec<u32>, Vec<u32>) {
    let md = &comp.module(ModuleId(0)).md;
    let rows_with_body: Vec<u32> = (1..=md.tables.rows(TableId::MethodDef))
        .filter(|&row| md.get::<rows::MethodDef>(row).is_ok_and(|m| m.rva != 0))
        .collect();
    let hook = panic::take_hook();
    panic::set_hook(Box::new(|_| {}));
    let mut ts = TypeSystem::new(comp);
    let mut translatable = Vec::new();
    let good: Vec<u32> = rows_with_body
        .iter()
        .copied()
        .filter(|&row| {
            panic::catch_unwind(AssertUnwindSafe(|| {
                let Some(mut f) = read(&mut ts, row) else {
                    return false;
                };
                transform(&mut ts, &mut f, settings);
                let translated = ilspy_decompiler::csharp::convert_body(
                    &mut ts,
                    &f,
                    settings,
                    READER.use_ref_locals_for_accurate_order_of_evaluation,
                    None,
                );
                if translated.is_ok() {
                    translatable.push(row);
                }
                let mut w = ModuleWriter::new(&ts);
                w.function(row, &f, None).is_ok()
            }))
            .unwrap_or(false)
        })
        .collect();
    panic::set_hook(hook);
    eprintln!(
        "  {} of {} method bodies read, transformed and serialized; {} of them translated",
        good.len(),
        rows_with_body.len(),
        translatable.len()
    );
    translatable.retain(|row| good.contains(row));
    (good, translatable)
}

fn stages(c: &mut Criterion) {
    let inputs = env_paths("ILSPY_BENCH_INPUTS");
    if inputs.is_empty() {
        eprintln!(
            "ILSPY_BENCH_INPUTS is not set: no stage benchmarks run. Set it to the assemblies to \
             measure (and ILSPY_BENCH_REFS to the directories holding their references)."
        );
        return;
    }
    let refs = env_paths("ILSPY_BENCH_REFS");
    let settings = default_settings();
    for input in inputs {
        let name = input
            .file_stem()
            .map(|s| s.to_string_lossy().into_owned())
            .unwrap_or_default();
        let mut dirs: Vec<PathBuf> = input.parent().map(Path::to_path_buf).into_iter().collect();
        dirs.extend(refs.iter().cloned());
        let images = load_closure(&input, &dirs);
        let Some(&main) = images.first() else {
            eprintln!("{}: not a readable assembly, skipped", input.display());
            continue;
        };
        eprintln!("{name}: {} modules in the compilation", images.len());
        let comp = compilation(&images);
        let (bodies, translatable) = servable_bodies(&comp, settings);

        let mut group = c.benchmark_group(&name);
        group.sample_size(10);
        group.bench_function("metadata_load", |b| {
            b.iter(|| Metadata::parse(black_box(main)).expect("metadata"))
        });
        group.bench_function("type_system_load", |b| {
            b.iter(|| compilation(black_box(&images)))
        });
        group.bench_function("il_read", |b| {
            b.iter(|| read_all(&mut TypeSystem::new(&comp), &bodies))
        });
        // Each stage below runs on what the stages before it produced, outside the timed part.
        group.bench_function("il_transforms", |b| {
            b.iter_custom(|iters| {
                let mut ts = TypeSystem::new(&comp);
                let mut total = Duration::ZERO;
                for _ in 0..iters {
                    let mut fs = read_all(&mut ts, &bodies);
                    let start = Instant::now();
                    for (_, f) in &mut fs {
                        transform(&mut ts, f, settings);
                    }
                    total += start.elapsed();
                }
                total
            })
        });
        group.bench_function("expression_building", |b| {
            b.iter_custom(|iters| {
                let mut ts = TypeSystem::new(&comp);
                let use_ref_locals = READER.use_ref_locals_for_accurate_order_of_evaluation;
                let mut total = Duration::ZERO;
                for _ in 0..iters {
                    let fs = transform_all(&mut ts, &translatable, settings);
                    let start = Instant::now();
                    for (_, f) in &fs {
                        let _ = black_box(ilspy_decompiler::csharp::convert_body(
                            &mut ts,
                            f,
                            settings,
                            use_ref_locals,
                            None,
                        ));
                    }
                    total += start.elapsed();
                }
                total
            })
        });
        group.bench_function("serialization", |b| {
            let mut ts = TypeSystem::new(&comp);
            let fs = transform_all(&mut ts, &bodies, settings);
            b.iter(|| {
                let mut w = ModuleWriter::new(&ts);
                for (row, f) in &fs {
                    w.function(*row, f, None).expect("written before");
                }
                w.finish()
            })
        });
        group.finish();
    }
}

criterion_group!(benches, stages);
criterion_main!(benches);
