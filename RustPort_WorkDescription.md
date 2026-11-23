# Rust port of ICSharpCode.Decompiler: work description

This is the description a pull request for branch `christophwille/rustafari` would carry. No PR
was opened.

The branch ports the decompiler engine of `ICSharpCode.Decompiler` to Rust and runs ILSpy, ilspycmd
and the whole test suite on it. It has 329 commits on top of master `ea1b2ca83`, written between
2026-11-07 and 2026-11-23 by an AI agent (Claude) under the direction and decisions of a human.

## 1. Summary

- **What the engine does.** A Rust engine (`rust/`, about 134,000 lines) reads metadata and IL
  itself, builds the ILAst, and runs all 59 leaves of the IL transform pipeline. It translates
  method bodies into the C# syntax tree, and runs all 16 built-in AST transforms on the finished
  tree.
- **How .NET reaches it.** A C# shim, `ICSharpCode.Decompiler.Rust`, keeps the public API by
  link-compiling the unmodified sources of `ICSharpCode.Decompiler`. It forks exactly one file
  (`CSharp/CSharpDecompiler.cs`) to hand the work to the engine through a C ABI.
- **Who uses it.** ILSpy, ilspycmd, ILSpyX, BamlDecompiler and every test project reference the
  shim instead of the original. The PowerShell module and the Native AOT gate stay on the
  original.
- **Fidelity.** Every test project runs strict (`ILSPY_RUST_STRICT=1` by default in tests): a
  request the engine cannot serve is an error, not a silent fallback.
  - All suites are green locally and on CI.
  - The ratchet (`rust/port-status.json`) records `il_prefix 59`, `ast_prefix 16`, and 100 % of
    methods and bodies served on its corpus.
- **Speed.** The engine is 1.5x to 7.1x slower than the managed decompiler: 5.6x on a 39,011-type
  NuGet corpus, and 2.7x the peak memory on long runs. It is functionally complete but not
  optimized; see section 8.
- **Not in Rust, by decision.** The browsing type system the UI uses, the output visitor and
  token writers, the PDB reader and writer, the project and solution writers, documentation,
  Debug Steps, the disassembler and BamlDecompiler's IL pass stay in C#. They walk managed
  objects, and porting them buys the UI nothing (lecture section 2).

## 2. Documentation map

| Document | What it is | Read it for |
|---|---|---|
| `RustPort_Instructions.txt` | The human's original brief, 27 lines, unchanged since 2026-11-07 | The starting point: what was asked, the guardrails, the expected deliverables |
| `RustPort_AgentInstructions.md` | The executable plan, about 930 lines. Items 0 to 89 as a checklist, split into sub-items as the work went on (`68a`-`68z8`, `68y1`-`68y89`, `85a`-`85f`); the gates (G-rust, G-dotnet, G-oracle, G-ilast(N), G-ast(K), G-ratchet, G-ui); the rules for the agent | What was planned, in which order, how each step was gated, and what is ticked. Only `68z6` (the rest of `RecordDecompiler`) and its parent items `68z` and `68` are open |
| `RustPort_HumanLecture.md` | The design record for senior developers, about 5,500 lines | See the breakdown below |
| `RustPort_PerformanceComparison.md` | CPU and memory of the engine against the managed decompiler, measured twice (before and after the conversion operator hotspot fix) | Numbers, where the time goes, memory, and the ranked "What to optimize first" list |
| `RustPort_ExistingCsharpImplNotes.md` | What the port found in the existing C# code | Bugs, order-dependent behavior and test-infrastructure problems in the managed decompiler and its tests, with suggestions for its maintainers |
| `RustPort_WorkDescription.md` | This document | The overview: map, layout, build, history, plan vs actual, open ends |

How `RustPort_HumanLecture.md` is organized:

- **Section 1:** what the brief asked and what the code base said about it (sizes, how the oracle
  and the UI use the API, the ilspycmd CS0433 cliff).
- **Section 2:** the decision table, one row per challenged instruction.
- **Section 3.1 to 3.6:** the architecture: shim, seams and fallback, wire format and
  materializers, FFI contract, Rust internals, oracle tooling.
- **Section 3.7 to 3.233:** one entry per executed step, each with what was built, what was
  found, the decisions taken and the gate results. Useful starting points:
  - 3.22: seam 1;
  - 3.48: the allowlist FORK review;
  - 3.85: the body-level seam 2 decision;
  - 3.221: the syntax tree round trip;
  - 3.223 to 3.231: strict mode and the body gaps;
  - 3.232: the UI pass;
  - 3.233: CI and packaging.
- **Section 4:** the alternatives evaluated.
- **Section 5:** plan and sizes.
- **Section 6:** the performance methodology (results are in the comparison document).
- **Sections 7 and 8:** the CI plan and the risks as planned (partly superseded; see section 9).

## 3. What is where on disk

Sizes: the original `ICSharpCode.Decompiler` is 582 C# files, 174,751 lines.

### 3.1 New

**`rust/`: the Cargo workspace** (edition 2024, toolchain pinned to 1.99.1 by
`rust-toolchain.toml` at the repo root).

- `ilspy-decompiler/`: the engine library, 189 files, about 132,700 lines, 597 `#[test]`
  functions. Its `src/` modules:
  - `metadata/`: its own ECMA-335 reader (PE, tables, heaps, signatures, custom attribute
    blobs, IL bodies, a listing used as an oracle).
  - `typesystem/`: an interned type system (`TypeId` handles into a per-request
    `TypeSystem`), the module table (`compilation.rs`), member definitions and references,
    attributes, specialization, inheritance, lookup, C# 14 extension info, task types.
  - `il/`: the ILAst arena (`ast.rs`), the IL reader, block builder, pattern helpers,
    `control_flow/` (loops, conditions, switches, the yield and async state machines, await in
    catch and finally) and `transforms/` (52 files, the IL transform pipeline;
    `transforms/mod.rs` holds `PIPELINE`).
  - `flow_analysis/`: data flow, definite assignment, reaching definitions.
  - `semantics/`: C# conversions, overload resolution, type inference, decompiled lambdas.
  - `csharp/`: the C# AST arena (`ast.rs`, `syntax.rs`, a generated `syntax_schema.rs`), the
    builders (expression, statement, call, member access, lambda, switch, foreach,
    initializer, unsafe code, delegates, dynamic), the declaration syntax
    (`entity_syntax.rs`, `type_syntax.rs`, `constant_syntax.rs`), the member walk and
    hiding, and `transforms/` (17 files, the 16 AST transforms with `ast_transforms.rs` as
    the driver).
  - `wire/`: the binary wire formats (ILAst out, resolver and oracle questions, syntax tree
    round trip).
  - `util/`, `lib.rs`: `IL_PREFIX` and `AST_PREFIX`, the served prefixes.
- `ilspy-decompiler/tests/`: integration tests ported from the managed suites:
  - type system loader, reflection helper, conversions, explicit conversions, overload
    resolution, type inference;
  - the IL reader;
  - a metadata corpus test;
  - assembly identity.
- `ilspy-decompiler/benches/stages.rs`: criterion benches per engine stage.
- `ilspy-decompiler-ffi/`: the `cdylib` `ilspy_decompiler_ffi` with 22 `extern "C"` exports,
  `catch_unwind` around each, the host callbacks (resolver, syntax oracle, debug info) and
  `stats.rs` (a counting allocator and timers behind `ilspy_stats`).
- Data files:
  - `port-status.json`: the ratchet.
  - `ilast-divergence-allowlist.json`: 69 accepted ILAst differences at intermediate
    prefixes, with reasons.
- Cargo profiles in `rust/Cargo.toml`:
  - `release` (with debug assertions and overflow checks) is used by Debug .NET builds;
  - `ship` (unchecked) is used by Release builds and packages;
  - `bench` measures what ships.
- `rust/.cargo/config.toml`: the cross linker for linux-arm64.

**`ICSharpCode.Decompiler.Rust/`: the shim** (netstandard2.0; `AssemblyName` is
`ICSharpCode.Decompiler` with the original's key and version; `PackageId`
`ICSharpCode.Decompiler.Rust`). 27 files, about 13,400 lines.

- `ICSharpCode.Decompiler.Rust.csproj`:
  - compiles `..\ICSharpCode.Decompiler\**\*.cs` minus the forked file;
  - runs cargo in the `BuildRustEngine` target before every build;
  - copies the native library to every consumer's output;
  - with `-p:RustNativeAssetsDir=...`, packs the per-RID engines under
    `runtimes/<rid>/native/`.
- `CSharp/CSharpDecompiler.cs`: the one forked file (3,119 lines against the original's 2,975).
  Its `RUST-SEAM` blocks call into `Interop/`. A test strips the blocks and asserts byte
  equality with the original.
- `Interop/`:
  - `NativeMethods.cs`: the DllImports.
  - `RustSession.cs`: a `SafeHandle`.
  - `RustEngine.cs`: one engine per type system; the requests.
  - `RustModuleTable.cs`: the host-built module table.
  - `RustSettings.cs`: settings as bit sets.
  - `RustSeam1.cs`: the per-method ILAst handover.
  - `IlAstMaterializer.cs`: wire to managed `ILFunction`.
  - `IlTransformPrefix.cs`: where a prefix boundary is.
  - `RustSeam2.cs`: bodies, declarations, member walks and the request-level checks.
  - `RustResolver.cs`: answers the engine's resolver questions with the managed
    `CSharpResolver`.
  - `RustDebugInfo.cs`: bridges `IDebugInfoProvider`.
  - `SyntaxRoundTrip*.cs`: the syntax tree round trip and the per-transform oracle answers.
  - `SyntaxSchema.cs` and the generated `SyntaxSchema.Order.cs`: the wire schema of the AST
    node classes.
  - `RustStats.cs`: the engine counters.

**`ICSharpCode.Decompiler.Rust.Tests/`: the engine's .NET tests** (23 files, about 3,800 lines,
about 80,000 test cases on the full corpus):

- the FFI handshake;
- the fork drift test (`ForkedSourceDriftTests`);
- the ratchet (`PortStatusRatchetTests`);
- metadata, type system, ILAst read and ILAst prefix equality against the managed code over a
  corpus of the shim, the .NET shared framework and every compiled `TestCases` image;
- AST prefix and per-transform AST equality;
- `MemberIsHidden` and record member walk equality;
- body gap tests (`BodyGapTests`, one sample method per construct);
- the seam 2 operator, span, range and vararg tests;
- the schema tests;
- `RustStrictByDefault.cs`, the module initializer that every test project links in.

Other new files:

- `ILSpy.Tests/Languages/RustEngineUiTests.cs`: the UI pass on the engine: Debug Steps,
  constructor and field selection, bookmarks, PDB, tooltips and navigation, cancellation, and a
  forced engine panic.
- `BuildTools/WindowsCITests.slnf`: every test project except the engine's, for the Windows CI
  job.
- `TestTools/perfcompare.ps1`: the two-build performance harness.
- `rust-toolchain.toml`.
- The `RustPort_*` documents.

### 3.2 Changed

- **The reference switch.** Every consumer's `ProjectReference` from `ICSharpCode.Decompiler` to
  `ICSharpCode.Decompiler.Rust`: ILSpy, ILSpyX, BamlDecompiler, ilspycmd, ILSpy.ReadyToRun,
  TestPlugin and all test projects. The lock files changed with them.
- **`ICSharpCode.ILSpyCmd.csproj`:** the reference line, plus `TrimmerRootAssembly
  ICSharpCode.Decompiler` for the Native AOT packages. Lecture 2 records the human's decision to
  relax the "do not touch ilspycmd" guardrail; the trimmer line was added in item 87.
- **Test projects:** one `Compile Include` of `RustStrictByDefault.cs` each, beside the
  reference switch: `ICSharpCode.Decompiler.Tests`, ILSpyCmd.Tests, ILSpy.Tests,
  ILSpy.Tests.Windows and both BamlDecompiler test projects.
- **`ILSpy.Tests/AssemblyList/SwitchListAndDeleteAllTests.cs`:** the cherry-picked upstream fix
  of PR #4355 (the Ctrl+A hang), nothing else.
- **Solutions:** `ILSpy.slnx`, `ILSpy.XPlat.slnf` and `ILSpy.Desktop.slnf` gained the shim and
  its tests.
- **`.github/workflows/build-ilspy.yml`:**
  - a `RustNative` job builds the engine per RID;
  - the packaging jobs consume those builds;
  - toolchain installation everywhere, and fmt/clippy/cargo test on Linux;
  - the Rust engine tests run on Linux only;
  - the switches `RunTests` and `ILSPY_RUST_CORPUS`.
- **The other workflows:** moved to the GA .NET 11 SDK; `actions.lock` pins the new action.
- **`BuildTools/ILSpy-x64.msi.filelist` and `ILSpy.AddIn.VS2022.vsix.filelist`:** gain the
  native library. They also pick up drift that predates the branch.
- **`TestTools/decompdiff.cs`:** `--perf` and `--side`. `TestTools/README.md` and
  `allocview.cs` changed slightly. `.gitignore` covers `rust/target`.

### 3.3 Not changed

- `ICSharpCode.Decompiler/`, `ICSharpCode.Decompiler.PowerShell/` and `ILSpy.AddIn*/`.
- `ICSharpCode.Decompiler.AotTest/`, which still gates the original.
- The test sources of `ICSharpCode.Decompiler.Tests`: only its csproj changed.

## 4. How a decompile runs now

1. **Load.** The UI or ilspycmd loads assemblies with the link-compiled managed type system, as
   before.
2. **Session.** On the first decompile of a type system, `RustEngine.Create` builds a module
   table: the main module and its reference closure, each passed as a pointer to the
   in-memory PE image, zero-copy. The engine opens a session over it. A type system without a
   corlib (`MinimalCorlib`) stays managed, counted.
3. **Seam 1, per method body.** The shim asks the engine for the ILAst after the first `N` IL
   transforms (`N` = 59, all of them), and materializes it into managed `ILFunction` objects.
   Resolver questions during the IL transforms come back to the host's `CSharpResolver` through
   a callback.
4. **Body translation, still per method.** The same request translates the ILAst into the C#
   body. Overload and member lookups that need the full managed resolver are answered by
   callback.
5. **Declarations.** Member and type declarations, the member walk, `MemberIsHidden` and the
   `new` modifier come from engine requests. The host assembles the type declaration around
   them.
6. **Seam 2, per request.** The finished `SyntaxTree` goes to the engine as a wire tree with
   annotation handles. The engine runs the 16 AST transforms; questions about annotations go
   back through the "syntax oracle" callback. The tree comes back and replaces the managed one.
7. **Output.** Caller transforms, the output visitor, token writers, PDB writer and project
   writers run managed, unchanged.

A request the engine refuses throws in strict mode and falls back to the managed code otherwise.
Each such refusal is counted (`RustSeam1`/`RustSeam2` counters, `RustSeam2.LastFallbackReason`).

## 5. Build and run locally

### 5.1 Prerequisites

- **.NET:** the .NET 11 SDK (GA; `global.json` allows 11.0.0 and later). PowerShell 7 for the
  repo scripts.
- **Rust:** rustup. The pinned toolchain (1.99.1 with rustfmt and clippy) installs itself on
  first use from `rust-toolchain.toml`; run `rustup toolchain install` once from the repo root.
- **Native linker:**
  - Windows: the MSVC build tools (VS Build Tools with the C++ workload).
  - Linux: gcc. For a linux-arm64 cross build also `gcc-aarch64-linux-gnu` and
    `libc6-dev-arm64-cross`.
  - macOS: the Xcode command line tools.
- **Optional `ILSpy-tests` submodule** (`git submodule update --init ILSpy-tests`) for the
  assembly round trips. The cargo tests need its `dotnet/legacy` folder.

### 5.2 Build

```
pwsh ./restore.ps1
pwsh ./build.ps1 Debug --no-restore      # or: pwsh ./build.ps1 -Configuration Release
```

- **Cargo runs with every build.** The shim's `BuildRustEngine` target runs
  `cargo build --locked -p ilspy-decompiler-ffi` before every build of the shim. A Debug .NET
  build uses the cargo `release` profile, optimized and with debug assertions; Release uses
  `ship`.
- **Where the library goes.** The first build compiles the engine, a few minutes; later builds
  are incremental. The native library lands in `rust/target/<profile>/` and is copied next to
  every consumer's output (`ilspy_decompiler_ffi.dll`, `libilspy_decompiler_ffi.so` or `.dylib`).
- **Debug after Release.** After a Release `build.ps1`, run `restore.ps1` before the next Debug
  `--no-restore` build. Otherwise ILSpy fails with CS1061 `WithDeveloperTools`.
- **Engine checks** (from `rust/`): `cargo fmt --check`, `cargo clippy --locked --all-targets
  -- -D warnings`.

### 5.3 Run

- **ILSpy:** `ILSpy/bin/Debug/net10.0/ILSpy.exe` (or `dotnet run --project ILSpy`).
- **ilspycmd:** `ICSharpCode.ILSpyCmd/bin/Debug/net10.0/ilspycmd.exe <assembly>`.
  - `--ilast` and `--after-transform` show the ILAst in Debug builds, as before.
  - `ILSPY_RUST_STRICT=1` makes every refused request an error comment in the output, which is
    the quickest way to see what the engine does not serve for a given assembly.
- **The engine is always on.** There is no runtime switch to turn it off; the comparison
  baseline is the original `ICSharpCode.Decompiler` build (`decompdiff`, `perfcompare.ps1`).

### 5.4 Test

```
dotnet test --solution ILSpy.slnx --report-trx
```

- **Strict by default.** Every test project runs strict (`RustStrictByDefault` sets
  `ILSPY_RUST_STRICT=1` unless it is already set). `ILSPY_RUST_STRICT=0` turns fallback back on.
- **Run time.** The whole solution takes 3 to 4 hours on a 24-core machine. Most of it is
  `ICSharpCode.Decompiler.Rust.Tests` with its full corpus (about 80,000 cases), which runs
  in parallel with the decompiler suite. A filtered run of that project still costs about 15
  minutes, because NUnit runs the AST prefix sweep anyway (see section 6).
- **The engine tests with the CI corpus** (ICSharpCode.Decompiler.dll only), about 20 minutes:

  ```
  ILSPY_RUST_CORPUS=decompiler dotnet test --project ICSharpCode.Decompiler.Rust.Tests/ICSharpCode.Decompiler.Rust.Tests.csproj --report-trx
  ```

- **Quick checks** after an engine change:

  ```
  dotnet test --project ICSharpCode.Decompiler.Rust.Tests/ICSharpCode.Decompiler.Rust.Tests.csproj --report-trx --filter "FullyQualifiedName~BodyGapTests|FullyQualifiedName~Seam2|FullyQualifiedName~PortStatusRatchet"
  ```

- **UI tests:** `./runuitests.ps1` runs 4 shards.
- **The cargo tests** need three fixtures from a built decompiler test project (from `rust/`):

  ```
  ILSPY_REF_SYSTEM_RUNTIME=<repo>/ICSharpCode.Decompiler.Tests/bin/Debug/net11.0/win-x64/netfx/5.0.0/ref/net5.0/System.Runtime.dll
  ILSPY_LEGACY_REFERENCE_DIR=<repo>/ILSpy-tests/dotnet/legacy
  ILSPY_TYPESYSTEM_TEST_ASSEMBLY=<repo>/ICSharpCode.Decompiler.Tests/bin/Debug/net11.0/win-x64/ICSharpCode.Decompiler.Tests.dll
  cargo test --locked
  ```

### 5.5 Measure

The full commands are in `RustPort_PerformanceComparison.md`, "Reproduce". In short:

- build Release;
- run `TestTools/perfcompare.ps1 -Old <original dll> -New <shim dll> -Runs 3 <inputs>`;
- for stage benches, `cargo bench --bench stages` with `ILSPY_BENCH_INPUTS` and
  `ILSPY_BENCH_REFS`.

### 5.6 CI

`build-ilspy.yml` runs everything on push and PR:

- **Build:** the per-RID engines, the Windows Debug and Release builds with the installer, VSIX
  and NuGet packages (including `ICSharpCode.Decompiler.Rust.nupkg`), the Linux and macOS
  bundles, the ilspycmd RID packages, and the AOT gate.
- **Tests, Windows:** the Windows test step runs `BuildTools/WindowsCITests.slnf`, everything
  except the Rust engine tests.
- **Tests, Linux:** the Linux job runs the decompiler tests, the cargo tests, the Rust engine
  tests with `ILSPY_RUST_CORPUS=decompiler`, the ilspycmd tests and the BAML tests.
- **Switches:** set `RunTests: 'false'` in the workflow env to iterate on packaging without
  tests.
- **Green run 45175084742:** Windows test steps 24 and 31 minutes (master: 15), Linux
  decompiler tests 6 minutes (master: 6), Rust engine tests 16 minutes.

## 6. What went wrong along the way

Roughly in the order it happened.

**Planning and the brief**

- **ilspycmd.** "Do not touch ilspycmd" could not hold together with re-pointing ILSpyX: CS0433,
  every decompiler type twice. The human relaxed the guardrail (lecture 1.4, 2).
- **Assembly name.** The planned "assembly named after the project" broke tests that open
  `ICSharpCode.Decompiler.dll` by file name. The shim took the original's identity instead,
  which means the two must never share an output directory (lecture 3.1).

**Tooling and environment on the development machine**

- **Line endings.**
  - Checkouts and stashes write CRLF while the index is LF.
  - Multi-line `perl`/`sed` edits silently did nothing on CRLF files. One missing pipeline stage
    showed up only as a panic ("range end index 12 out of range").
  - Git Bash `sed`/`awk` turn CRLF files into LF. Every scripted edit needed an explicit EOL
    check, and mass conversion was ruled out because it leaves phantom modifications.
- **MAX_PATH.** Correctness tests fail from deep worktree paths (unnormalized relative path to
  the test runner over 260 characters). Worktrees had to be mapped to a short drive letter with
  `subst`.
- **Worktrees.**
  - Agent worktrees start from master and had to be reset to the branch first.
  - They lack the compiled `TestCases` images, so the AST sweep there covered less until the
    image set was copied in.
  - `cargo test` in a worktree needs the three fixture paths of the main checkout.
  - `git worktree remove` sometimes left locked empty directories behind.
- **Disk space.** The development disk ran about 90 % full, so every worktree was deleted right
  after its merge. `TestResults/merged` grew to 6.8 GB of TRX files over the weeks.
- **Build trees.** A project-level build writes `bin/Any CPU/...` and a solution build writes
  `bin/Debug/...`; `--no-build` test runs picked up stale binaries until that was understood.
- **Restore.** A Release restore breaks the next Debug build (CS1061 `WithDeveloperTools`), so
  `restore.ps1` is needed in between.
- **Profiling.** No native sampling profiler was set up. `dotnet-trace` attributes engine time
  only to `UNMANAGED_CODE_TIME`. The one hotspot fixed so far was found with temporary,
  hand-placed timers.

**Test time and parallelism**

- **Gate time.**
  - A full gate run took about 40 minutes at first, and later 3 to 4 hours with the full
    engine corpus.
  - One full AST prefix sweep took about 2.5 hours on the loaded machine.
  - At step 68y42 the human asked to batch several construct groups per full gate, departing
    from "one step, one commit".
- **Filtered runs are not cheap.** Any filtered `dotnet test` over the Rust test project still
  executes the parameterized AST prefix sweep for every image, about 15 minutes, because of
  NUnit's parameterized-case behavior. Root cause not found.
- **Shared fixture files.** In a solution run, `ICSharpCode.Decompiler.Tests` rewrites fixture
  images (PdbGen writes `*.expected.dll` into the source tree) while the engine tests read them:
  - file locks, and briefly deleted files;
  - CS2012 failures;
  - the engine tests now read with a minute of retries.
- **Load flakes** that always passed alone:
  - the `NewtonsoftJson_net45` round trip (runner failure, cause unknown);
  - six PdbGen tests (file locks);
  - the process explorer UI tests (60 s budgets);
  - the base types tree node thread-affinity race in `Back_History`.
  Each was rerun alone, several times, before being called a flake.
- **The Ctrl+A hang.** `Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab` hung on master
  too: a 12,000-character tab title laid out by headless stub fonts. It was excluded from gate
  runs for two weeks, until the upstream fix (PR #4355) was cherry-picked.
- **Strict exceptions looked like missing bodies.** A strict refusal inside a body is caught by
  `DecompileBody`'s error handler, so it surfaced as an output difference with the reason in a
  comment. The worklist for the body gaps had to come from a tally of those comments
  (ilspycmd over all 3,260 test images) instead of from failing tests.

**Session interruptions**

- **Crash.** The development computer crashed while three agents were working on body-gap round
  2 (2026-11-19). The work was uncommitted in their worktrees; the agents were resumed with their
  context instead of restarted.
- **Pauses.** The human paused the work several times: after full gates, and for a reboot while
  a CI run was being watched. Memory notes and the plan kept the state across sessions and
  context compactions.

**Strictness**

- **The first strict run.** The first matrix run with seam 2 strict failed about 2,000 tests. The
  ratchet corpus used default settings and one compiler, which hid most gaps (lecture 3.223).
  Closing them took six rounds of parallel agents (85a to 85f).
- **Silent divergences.** A `Task.Run(() => {...})` overload resolution gave `Run<T>(Func<T>)`
  instead of `Run(Action)`. Some divergences are only visible through equality tests, not text
  tests. This one was fixed in round 4.

**CI (item 87)**

- **`startup_failure`.** The first push ended in `startup_failure` without a log: a new action
  was missing from `actions.lock`.
- **linux-arm64 cross build.** It failed twice:
  - cargo was run with `--manifest-path` from the repo root, so it did not read
    `rust/.cargo/config.toml`;
  - `crti.o` was missing (`libc6-dev-arm64-cross`).
- **Native AOT ilspycmd.** It failed twice:
  - the trimmer removed the AST members the schema reflects over;
  - AOT has no `MemberInfo.MetadataToken`. The member order is now generated.
- **File lists.** The MSI and VSIX file lists had drifted before the branch. The verification
  step runs in the Debug job, which builds neither, so it never noticed.
- **First run with tests.** It failed on all three operating systems:
  - the engine refused `(await f()).Item1` in ilspycmd's tests, which decompile themselves, and
    the refusal's error body crashed the Debug test host through a managed PDB writer
    assert;
  - two engine tests needed fixtures a fresh checkout has not compiled;
  - the engine test sweep ran over 3 hours on Windows and over 4 hours on Linux.
- **Second run.** The Windows test step took 4.5 hours, against 15 minutes on master. On a
  4-core runner all test hosts run at once, and the engine's parallel sweeps slowed every suite
  about tenfold. UI tests with 60 s budgets failed.
- **Third run.** Still 2.5 hours. Per-test timings showed the cause was not load: four generated
  conversion round trips took 145 minutes instead of 5.5. That was the engine hotspot described
  in the performance document. After the fix the Windows test step takes 24 to 31 minutes.
- **Watching the runs.** `gh run watch` lost its connection to the GitHub API once during a
  multi-hour run.

## 7. Plan vs what was actually done

The plan was rewritten as the work found things out. Every change is recorded in the lecture
entry of its step; this is the list.

**The brief (planning, lecture 1-2, 4)**

- **Shim.** The plan's "proxy that keeps the public surface" became a link-compiled shim with
  one forked file. A proxy over FFI would be a second implementation of about 760 types.
- **Tests.** "Only switch the test project reference" holds, with a caveat. The about 1,550 API
  tests of the decompiler suite test the link-compiled C# layers, not Rust. The semantic suites
  were therefore also ported to `cargo test`.
- **Scope.** "Port all of ICSharpCode.Decompiler" became "engine only", by human decision. The
  UI-facing layers stay C#.
- **Naming.** `libicsdecompiler`/`ICSharpCode.DecompilerLib` became `ilspy-decompiler`,
  `ilspy-decompiler-ffi` and `ICSharpCode.Decompiler.Rust`, by human decision.
- **Approach.** A big-bang port was replaced by an incremental two-seam fallback with a
  ratchet, by human decision.
- **ilspycmd.** The guardrail was relaxed, by human decision.

**Scaffolding, metadata, type system (items 1-4)**

- **Assembly identity.** The shim uses the original's assembly identity (item 1).
- **FFI.**
  - The planned nine exports became 22 (read function, entity syntax, member walk, member
    hiding, `new` modifier, field data comment, syntax transforms, settings names, prefixes,
    stats, debug hooks).
  - `ilspy_decompile`, `ilspy_session_add_module` and `ilspy_cancel` were never needed in that
    form.
  - Settings travel as bit sets of names the engine declares, instead of reflected name/value
    pairs.
- **Metadata oracle.** Item 3 compares against System.Reflection.Metadata instead of
  `MetadataModule`.
- **Module table.** Item 4a replaced the planned `resolve_assembly` callback with a host-built
  module table: closure policy is resolver behavior the UI already configures.
- **Item 4 split into 4a to 4k.**
  - 4h and 4i were swapped.
  - C# 14 `ExtensionInfo` moved to 68z4b and runtime async to item 5.
  - Specialized members have no identity in Rust, so identity checks became owner-kind checks.

**ILAst and seam 1 (items 5-66)**

- **Item 5 split into 5a to 5d.** The debug info callback lost its module id and writes into an
  engine buffer.
- **Seam 1** became three, later five, marked blocks, not one. The drift test forbids replacing
  lines, and `dotnet format` re-indents brace-less `if`s. The managed IL reader still runs
  before the seam.
- **The allowlist FORK (item 32).** The human kept the 78 ConditionDetection entries, expecting
  each to leave with its transform. The allowlist peaked at 4,473 entries and settled at 69.
  Those 69 come from rewrites inside `ExpressionTransforms` that ConditionDetection triggers;
  the premise did not hold for them.
- **Item 56** became three gated commits instead of one.
- **Item 64 (Humanizer)** was not ported; singularization is a host callback, by user choice.
- **Item 65 (analysis pipeline)** is served with a reader flag. Generic methods and traced
  requests stay managed.
- **Item 66 (strict mode).** "The final state runs strict by default" became "every test project
  runs strict by default" through a linked module initializer. Product code is not strict.

**C# AST and seam 2 (items 67-68)**

- **Item 67** ported more tests than planned (273 pass, 8 ignored as in the managed suites).
  Member lookup was deferred to item 68.
- **Item 68.** The planned single request-level seam 2 (about 30,000 managed lines that would
  all have to be exact before the first green gate) was replaced by three human decisions:
  1. bodies first, counted per body;
  2. types and constants as syntax through the managed builder until 68z;
  3. one commit per construct group.
- **68a to 68x and 68y1 to 68y89** were the construct groups. Human and user decisions inside
  them:
  - the lambda resolve result ported (68y10);
  - a type system change approved (68y21);
  - the builder works on a copy of the function (68y22);
  - member lookup forwards through decorators (68y28);
  - from 68y32 on, design decisions follow the agent's recommendation.
- **Disambiguation.** The Disambiguator re-resolves through the host's resolver, by human
  decision (68b). Porting name lookup and `MemberLookup` was announced as a later step and never
  scheduled.
- **68z** was split into 68z1 to 68z8 by agent recommendation. 68z3 and 68z4 were split further,
  and the `.override` forwarders moved twice.
- **68z8, the syntax tree round trip,** replaced 68z4c6 (type declarations assembled in the
  engine) when the user made speed the priority:
  - the host sends the finished tree;
  - the engine runs the AST transforms and returns it;
  - annotations travel as handles answered by a callback.
  68z4c6 was marked superseded by human decision on 2026-11-23. The planned
  `Interop/AstMaterializer.cs` does not exist; `SyntaxRoundTrip*.cs` does.
- **68z6 (`RecordDecompiler`)** was moved before item 75, then unblocked by the round trip. Only
  its member order was ported (85d); the rest is open.
- **68z7** (`params ReadOnlySpan` in an inline array) was found late and served in 85e.

**AST transforms (items 69-84)**

- **Ported in parallel.** Instead of one transform per commit in order, agents ported them in
  parallel against `AstTransformEqualityTests`. They were served in three steps of `AST_PREFIX`,
  6, 11 and 16, each under the full sweep (user decision for speed).
- **Annotations.** At prefix 6 the sweep found that chained transforms ask about annotations an
  earlier engine transform created. The engine now hands new annotations to the host after
  every transform.
- **Early caller transforms.** At prefix 16, requests with recognized early caller transforms
  turned out to be refused. The host now runs those transforms managed before the round trip,
  as a shortcut.
- **Item 84, `AddXmlDocumentationTransform`, was ported after all.** Lecture section 5 had listed
  it as staying managed.

**Strictness (item 85)**

- **The switch.** A separate `ILSPY_RUST_STRICT_SEAM2` switch came first. Its first matrix run
  failed about 2,000 tests, and the gaps became 85a to 85f.
- **The worklist** became a strict ilspycmd tally over all test images. It was worked by 2 to 4
  agents per round in parallel worktrees with disjoint wire-kind ranges.
- **Remaining fallbacks.** Round 4 left two counted fallbacks (unresolved methods and fields),
  which the human accepted. 85f served them anyway.
- **Merged.** The seam 2 switch was merged into `ILSPY_RUST_STRICT`.
- **Seam 1** is the production path, not "only for the prefix-equality harness" as item 85's
  text says.

**UI, performance, CI (items 86-89)**

- **Item 86.** The smoke list became headless UI tests. It found the `MinimalCorlib` gap, which
  is now a counted shortcut. The human cherry-picked the upstream Ctrl+A test fix.
- **Order.** Item 88 ran before item 87, at the human's request.
- **Item 88.**
  - `rust_elapsed_ns` became a process-wide counter instead of a response header field.
  - The results went into their own document, measured twice.
  - Item 89 (the optional-tail FORK: disassembler, output visitor, Rust PDB reader,
    `WholeProjectDecompiler`) was made obsolete by the human. The optimization list replaces it.
- **Item 87, compared with the planned CI:**
  - `rustup` instead of the `setup-rust-toolchain` action, and no Rust cache;
  - fmt, clippy and cargo test only on Linux;
  - a `RustNative` job for five RIDs;
  - packaging through `RustNativeAssetsDir`;
  - a `RunTests` switch (tests off while bringing up packaging, as the human asked);
  - `ILSPY_RUST_CORPUS=decompiler`;
  - the engine tests excluded from the Windows job;
  - two engine fixes found by CI (await tuple names, the conversion operator hotspot);
  - the move to the GA SDK, at the human's request.

## 8. Engine speed

Measured 2026-11-23 on a 24-thread Ryzen AI 9 HX 370, Release, single thread, after the
conversion operator fix (details in `RustPort_PerformanceComparison.md`):

| Input | Managed | Engine | Ratio |
|---|---|---|---|
| ICSharpCode.Decompiler.dll | 12.1 s | 86.2 s | 7.1x |
| System.Reflection.Metadata.dll | 4.5 s | 6.7 s | 1.5x |
| ILSpy.dll | 9.0 s | 29.0 s | 3.2x |
| top-200 NuGet corpus | 265 s | 1,491 s | 5.6x |

- **Where the time is.** 88 to 92 % of the engine's time is inside Rust. Marshaling is 5 to
  11 %, callbacks 3 to 7 %.
- **Allocation.** The engine allocates about 3,000 to 4,000 times per request (0.74 TB over the
  corpus).
- **Memory.** Peak RSS is 9 to 19 % higher on single assemblies, and 2.7x on the corpus run
  (one large assembly's session peak, no leak).
- **The managed heap is not relieved.** 4 to 7 % more managed allocation.

The ranked optimization list (none done except the first finding):

1. **Find more lookups that resolve whole member lists.** The fixed hotspot resolved every method
   of two types (and their bases) for every argument of every overload candidate, only to keep a
   few `op_Implicit` methods. Fixing it with a name filter and a per-request cache halved the
   corpus time. `get_methods`, `get_properties`, and the lookups behind overload resolution,
   extension methods and operator candidates may have the same shape. Per-stage timers (as used
   for this hotspot) find them.
2. **Profile.** Run a sampling profiler (ETW/`wpr` on Windows, `perf` or `samply` on Linux) over
   `decompdiff --side new` with symbols for `ilspy_decompiler_ffi`.
3. **IL read per session, not per body.** 360 to 600 us per body today is the cost of a whole
   managed type at P50. Cache signature decoding and member and type resolution across the
   bodies of a session.
4. **Cut repeated work.** There are 3.6 requests and 7 FFI calls per body. Avoid reading or
   transforming a body twice (seam 1, seam 2, lambdas re-read per container). Do one syntax tree
   round trip per type instead of per request, so per-request caches live longer.
5. **Reduce allocation churn.** Use arenas or bump allocation per request, reuse buffers across
   requests, intern strings once per session.
6. **Build settings.** `codegen-units = 1` and LTO in the `ship` profile, typically 10 to 20 %.
7. **Bound the session peak on huge assemblies.** Free per-type structures after each type.

A structural cost sits outside that list: the managed `ILReader` still runs before seam 1, and
the shim computes several managed decisions beside the engine's to count differences (section
9.3). Removing that duplicated work is part of any serious optimization.

## 9. Left dangling, not done, unoptimized

### 9.1 Open plan items

- **68z6, the rest of `RecordDecompiler`.** About 1,460 managed lines are still not ported:
  - which generated record members the declaration leaves out (`MethodIsGenerated`,
    `PropertyIsGenerated`);
  - the analysis the AST transforms read.
  The engine asks the host's `RecordDecompiler` through the oracle. Plan items 68z and 68 stay
  unticked until it is done.

### 9.2 Things that stay managed or call back into the host

These work, but they mean the engine is not self-contained:

- **Resolver callbacks.** Name lookup, `MemberLookup`, method group lookups,
  `CanTransformToExtensionMethodCall` and the Disambiguator's re-resolution are answered by the
  host's `CSharpResolver` (lecture 3.86). Porting them was announced as a later step and never
  scheduled.
- **Other callbacks:**
  - Humanizer singularization;
  - documentation text;
  - debug info (`IDebugInfoProvider`);
  - the syntax oracle for annotations.
- **The host assembles type declarations;** the engine supplies the members and runs the AST
  transforms (68z8 instead of 68z4c6).
- **Requests that stay managed by design:**
  - Debug Steps (`RecordSteps`);
  - requests with `ILTransformExecuted` or `AstTransformExecuted` tracing;
  - a changed built-in transform list;
  - an unknown early caller transform;
  - the analysis of generic methods;
  - a type system without a corlib (`MinimalCorlib`).
- **Recognized early caller transforms** run managed ahead of the round trip instead of as
  engine flags (`RustSeam2.cs:777`).
- **Synthesized members.** Dynamic `FakeField`/`FakeMethod`/`FakeProperty` and `MemberIsHidden`
  for members of other modules are decided by the host.

### 9.3 Duplicated work in production

- **IL is read twice.** The managed `ILReader` runs before seam 1 (`CSharpDecompiler.cs`), so
  every method is read by both.
- **Decisions are made twice.** For `MemberIsHidden`, the member walk, `DefaultMember` handling
  and the `new` modifier, the shim computes the managed decision next to the engine's and counts
  the differences (`RustSeam2.cs:530-600`). That is useful while porting, and pure overhead once
  trusted.

### 9.4 Missing pieces

- **No in-engine cancellation.** The engine never returns `Status::Cancelled`, and there is no
  `ilspy_cancel`. Cancellation takes effect between engine requests, which the UI test accepts
  ("stops before the end of the type"). A long single request, such as one huge method, cannot
  be interrupted. The planned per-request `AtomicBool` was not built.
- **No kill switch.** There is no runtime switch to turn the engine off in a shipped build.
  `ILSPY_RUST_STRICT` only toggles fallback. Going back means building against the original
  `ICSharpCode.Decompiler`.
- **`MinimalCorlib` is not modeled in the engine.** With "auto-load references" off, a type
  system without a corlib stays managed (counted).
- **Options per module.** The FFI applies one value of the type system options to every module;
  a function pointer shortcut reads the main module's options (`typesystem/types.rs:798`).
- **The divergence allowlist is not empty.** `rust/ilast-divergence-allowlist.json` still has 69
  entries:
  - 65 ConditionDetection: NullableLifting, array/span initializer and inline-array rewrites
    that `InvertIf`'s `ExpressionTransforms` call performs but the engine's ConditionDetection
    does not;
  - 4 LockTransform: Mono finally use counts.
  The C# output agrees, but the ILAst at those prefixes differs.

### 9.5 Code-level shortcuts

63 `shortcut:` comments in the engine and the shim each name a limit and when to upgrade.
`git grep -n "shortcut:"` lists them. The kinds:

- **Fall back instead of translating:**
  - a throw expression in `??`;
  - stackalloc initializers with gaps;
  - some boxed delegate targets;
  - attributes on constraint types;
  - an internal base type of another module (InternalsVisibleTo is not read);
  - a null of a type parameter's type;
  - sorted attributes that differ only by culture-sensitive string comparison.
- **Approximations that agree on the test corpus:**
  - Unicode categories in identifier checks;
  - top-level nullability in constructor lookups;
  - `IEnumerable<T>` instead of the `GetEnumerator` pattern for foreach element types;
  - type inference and operator overload resolution inside a few IL transforms (marked "port
    with item 67", never revisited).
- **"Give up" where managed throws:** some array initializer, dynamic call site and yield return
  shapes.
- **Constant folding:** overflowing checked operations, floating point, bitwise and enum
  folding are not ported (`expression_builder.rs:3152`).

Under strict mode, no test reaches a counted fallback. Product builds are not strict, so in the
field such fallbacks are silent apart from the counters.

### 9.6 Tests and CI

- **The full equality corpus is a local gate only.** CI sweeps ICSharpCode.Decompiler.dll alone.
  A change can pass CI and still break the framework or fixture images. Run the full
  Rust.Tests project locally before merging engine changes.
- **`BuildTools/WindowsCITests.slnf` lists test projects by hand.** A new test project must be
  added there, or it never runs on Windows CI.
- **Slower Windows CI.** The Windows CI test step is 24 to 31 minutes against 15 on master.
- **Known load flakes** (all pass alone):
  - `NewtonsoftJson_net45`;
  - PdbGen file locks;
  - the process explorer 60 s budgets;
  - `Back_History`.
  `RustPort_ExistingCsharpImplNotes.md` section E has the details.
- **Packaging and test gaps:**
  - The Native AOT gate covers the original only. The shim's AOT problems were found by the
    ilspycmd RID package smoke test.
  - The macOS dylib is not code-signed (a risk listed in the plan, not addressed).
  - The arm64 Windows and Linux builds are produced and packaged, but no test runs on arm64.
  - A RID-less output carries one architecture per OS (win-x64, linux-x64, osx-arm64); arm64
    hosts need the RID-specific packages (`shortcut:` in the shim's csproj).
- **The last full local run** of every test project, all strict, was on b04f03ac7 (after the UI
  pass). Engine changes since then were gated by CI (whose engine sweep covers one image), the
  filtered engine tests and the conversion round trips: the await tuple names, the conversion
  operator fix, and the new body gap test.

### 9.7 Stale spots in the documents

- `RustPort_AgentInstructions.md` item 88 still quotes the first measurement ("1.9x to 11x").
- Lecture section 6 shows the first measurement.
- Lecture section 7 describes `setup-rust-toolchain` and a Rust cache, which CI does not use.
- Lecture section 5 lists `AddXmlDocumentationTransform` among the managed parts, but it was
  ported.
- Item 85's gate text calls seam 1 "only for the prefix-equality harness".

`RustPort_PerformanceComparison.md` and this document are current.

## 10. Suggested review path

1. **The decision record:** lecture sections 1 to 3.6, then 3.22 (seam 1), 3.85 (body-level
   seam 2), 3.221 (round trip) and 3.233 (CI).
2. **The managed side of the boundary:** `ICSharpCode.Decompiler.Rust/CSharp/CSharpDecompiler.cs`
   (search for `RUST-SEAM`), then `Interop/RustEngine.cs`, `RustSeam1.cs`, `RustSeam2.cs`.
3. **The native side:** `rust/ilspy-decompiler-ffi/src/lib.rs`, the boundary seen from Rust.
4. **The engine:**
   - the pipeline, `rust/ilspy-decompiler/src/il/transforms/mod.rs` (`PIPELINE`);
   - one transform end to end, for example `il/transforms/using_transform.rs` beside the C#
     `UsingTransform.cs`;
   - the body translation entry, `csharp/statement_builder.rs` (`convert_body`).
5. **The open ends:** sections 8 and 9 above.
6. **The findings in the C# code:** `RustPort_ExistingCsharpImplNotes.md`, which are useful
   independently of the port.
