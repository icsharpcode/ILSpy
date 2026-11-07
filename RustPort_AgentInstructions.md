# Rust port of ICSharpCode.Decompiler: agent instructions

This file is the working checklist for the coding agent that performs the port. Read
`RustPort_HumanLecture.md` first: it holds the architecture, the decisions already taken and
the reasons. This file holds only the ordered steps, their gates and their rules.

## Rules for the agent

1. **One step, one commit.** Each checklist item below is exactly one commit. The commit
   subject names the step (suggested subjects are given). Do not batch steps.
2. **Keep both planning files current and commit them with the step.** When a step changes a
   decision, a size estimate, a gate or the order, edit this file and the lecture in the same
   commit. Tick the item (`- [x]`) in the commit that completes it. Record every architectural
   decision, every accepted divergence and every measured number in the lecture, not in chat.
3. **Gates are mandatory.** A step is done only when its gate passes. Paste the gate command
   and its result summary into the commit body. A red gate means the step is not done; do not
   tick it and do not move on.
4. **FORK items stop the agent.** Items marked FORK are decisions only a human may take.
   Stop, present the options with evidence, wait. Never decide a FORK on your own.
5. **Never edit these projects' sources:** `ICSharpCode.Decompiler/`,
   `ICSharpCode.Decompiler.PowerShell/`, `ILSpy.AddIn*/`. The one permitted exception, decided
   by the human, is the three `ProjectReference` lines in
   `ICSharpCode.ILSpyCmd/ICSharpCode.ILSpyCmd.csproj` (see the lecture, "Decisions").
   `ICSharpCode.Decompiler.Tests` gets its `ProjectReference` switched and nothing else; if a
   test cannot compile against the shim, that is a shim defect, not a reason to edit the test.
6. **Follow `AGENTS.md`** for everything else: license header on every new `.cs` file,
   `Assisted-by:` trailer on every commit, `restore.ps1` / `build.ps1` / `updatedeps.ps1`,
   committed `packages.lock.json` for every new project, the format hook, ASCII-only, en-US,
   `--report-trx` on every test run, no `--no-verify` on commits that touch `.cs`.
7. **Rust hygiene:** `cargo fmt --check`, `cargo clippy --all-targets -- -D warnings` and
   `cargo test` are part of every gate from step 2 on. Rust code is idiomatic Rust: arenas and
   ids instead of parent-pointer object graphs, enums instead of class hierarchies, `Result`
   instead of exceptions, no `unsafe` outside the ffi crate. Changing an algorithm is allowed
   when the text oracle still passes; record the change in the per-transform allowlist and the
   lecture.
8. **Honesty of the ratchet.** `rust/port-status.json` records how much of the pipeline Rust
   serves. It may only go up. The shim test that enforces it is never weakened. Fallback to the
   managed engine is counted, never hidden.
9. **Scope discipline.** Do not port what the lecture says stays managed (output visitor,
   disassembler, PDB reader and writer, project and solution writers, documentation, Debug
   Steps, BamlDecompiler's IL pass, the browsing type system). Do not add features.

## Vocabulary

- **original**: `ICSharpCode.Decompiler` (untouched).
- **shim**: `ICSharpCode.Decompiler.Rust`, a C# project that link-compiles the original's
  sources and forks exactly one file, `CSharp/CSharpDecompiler.cs`, to add two seams.
- **engine**: the Rust crates under `rust/` (`ilspy-decompiler`, `ilspy-decompiler-ffi`).
- **seam 1**: per method body, the ILAst after the first `N` IL transforms comes from Rust,
  the remaining transforms run managed.
- **seam 2**: per decompile request, the C# AST after the first `K` AST transforms comes from
  Rust, the remaining AST transforms run managed.
- **materializer**: C# code that turns the engine's wire format into the link-compiled C#
  object model (`ILFunction` / `ILInstruction` for seam 1, `SyntaxTree` with annotations for
  seam 2).
- **ratchet**: `rust/port-status.json` with `{ "il_prefix": N, "ast_prefix": K,
  "methods_served_pct": ... }`, enforced by a shim test.
- **strict**: environment variable `ILSPY_RUST_STRICT=1` disables all fallback; a request
  the engine cannot serve then fails the test.
- **fixture assemblies**: the compiled outputs of `ICSharpCode.Decompiler.Tests/TestCases/**`
  (Pretty, ILPretty, Correctness, Ugly, VBPretty, PdbGen) as produced by a test run, plus
  `ICSharpCode.Decompiler.dll` itself.

## Standard gates

- **G-dotnet**: `./build.ps1 -Configuration Debug`, `./build.ps1 -Configuration Release`,
  `dotnet test --solution ILSpy.slnx --report-trx` all green, and
  `dotnet-format whitespace --verify-no-changes ILSpy.slnx` clean.
- **G-rust**: in `rust/`: `cargo fmt --check`, `cargo clippy --all-targets -- -D warnings`,
  `cargo test` green.
- **G-oracle**: `dotnet test --project ICSharpCode.Decompiler.Tests --report-trx` green
  (Pretty, ILPretty, Correctness, Ugly, VBPretty, Disassembler, PdbGen, plus the API tests).
- **G-ilast(N)**: shim test `IlAstPrefixEquality` over every method body of every fixture
  assembly: `WriteTo()` of the materialized Rust ILAst after transform prefix `N` equals
  `WriteTo()` of the managed ILAst after the same prefix, except entries in
  `rust/ilast-divergence-allowlist.json` (each with transform name, fixture, method and reason).
- **G-ast(K)**: shim test `AstPrefixEquality` over every fixture request: materialized Rust
  AST after AST prefix `K`, then managed transforms `K+1..`, printed with `CSharpOutputVisitor`,
  equals the fully managed output.
- **G-ratchet**: shim test `PortStatusRatchet` passes (values in `rust/port-status.json` are
  not lower than what the run measured, and the committed file is updated when they rise).
- **G-ui**: `./runuitests.ps1` green.

## Checklist

### Phase 0: scaffolding

- [x] **0. FORK (taken).** The human has decided: relax the ilspycmd guardrail to its three
  `ProjectReference` lines; engine-only end state; incremental two-seam fallback; names
  `ilspy-decompiler` / `ilspy-decompiler-ffi` / `ICSharpCode.Decompiler.Rust`. These are
  recorded in the lecture. No commit for this item.

- [x] **1. Shim scaffold and reference switch.** Size S. Subject: `Add ICSharpCode.Decompiler.Rust shim and switch consumers to it`.
  - Create `ICSharpCode.Decompiler.Rust/ICSharpCode.Decompiler.Rust.csproj`: netstandard2.0,
    cloned from the original csproj (generator analyzer reference, `ICSharpCode.Decompiler.snk`,
    `ReferenceAssemblyAnnotator` plus the `Microsoft.NETCore.App.Ref` `PackageDownload`,
    `STEP` and `CheckForOverflowUnderflow` for Debug, `WarningsAsErrors=nullable`,
    `AllowUnsafeBlocks`, the same `RuntimeIdentifiers`, `RestoreLockedMode`), `IsPackable`
    true with `PackageId` `ICSharpCode.Decompiler.Rust` and **`AssemblyName`
    `ICSharpCode.Decompiler`** (tests open `ICSharpCode.Decompiler.dll` from their output
    directory and `TransitiveReferenceTests` asserts the reference name; see the lecture, 3.1).
    `<Compile Include="..\ICSharpCode.Decompiler\**\*.cs" Exclude="..\ICSharpCode.Decompiler\bin\**;..\ICSharpCode.Decompiler\obj\**;..\ICSharpCode.Decompiler\CSharp\CSharpDecompiler.cs;..\ICSharpCode.Decompiler\Properties\DecompilerVersionInfo.template.cs" />`
    and a `ProjectReference` to the original with `ReferenceOutputAssembly="false"`,
    `Private="false"` and `PrivateAssets="all"` so `Properties/DecompilerVersionInfo.cs` is
    generated before the shim compiles while the original stays out of every consumer's
    transitive graph and out of the shim's nuspec.
  - Add `ICSharpCode.Decompiler.Rust/Properties/AssemblyInfo.cs` mirroring the original's
    attributes (version, COM visibility) and granting `InternalsVisibleTo` to
    `ICSharpCode.Decompiler.Tests`, `ILSpy.Tests` and `ICSharpCode.Decompiler.Rust.Tests` with
    the same public key (the original's `AssemblyInfo.cs` is excluded from the link glob
    because it lives in the original's project).
  - Copy `CSharp/CSharpDecompiler.cs` to `ICSharpCode.Decompiler.Rust/CSharp/CSharpDecompiler.cs`
    unchanged except two marked blocks `// RUST-SEAM-1 BEGIN/END` (around the
    `function.RunTransforms(...)` call) and `// RUST-SEAM-2 BEGIN/END` (around the AST
    transform loop), both empty for now.
  - Create `ICSharpCode.Decompiler.Rust.Tests` (NUnit on Microsoft.Testing.Platform, same
    shape as `ICSharpCode.Decompiler.Tests.csproj`) with one test: strip the seam blocks from
    the shim copy and assert byte equality with the original file. Upstream drift in
    `CSharpDecompiler.cs` is then a red test.
  - Switch `ProjectReference` from the original to the shim in: `ICSharpCode.ILSpyX`,
    `ICSharpCode.BamlDecompiler`, `ICSharpCode.ILSpyCmd` (permitted exception), `ILSpy`,
    `ILSpy.ReadyToRun`, `TestPlugin`, `ICSharpCode.Decompiler.Tests`,
    `ILSpy.BamlDecompiler.Tests`. Leave `ICSharpCode.Decompiler.PowerShell`,
    `ICSharpCode.Decompiler.AotTest` and the add-ins on the original.
  - Add the two new projects to `ILSpy.slnx`, `ILSpy.Desktop.slnf` and `ILSpy.XPlat.slnf`.
    Run `updatedeps.ps1`, commit the new `packages.lock.json` files.
  - Gate: G-dotnet. Everything still runs on managed code; this commit proves the
    link-compile and the reference switch alone keep the whole repository green.

- [x] **2. Rust workspace and FFI handshake.** Size S. Subject: `Add the Rust engine workspace and the FFI handshake`.
  Done as: toolchain pinned to `1.99.1`; `unsafe_code = "deny"` as a workspace lint, allowed
  only in the ffi crate; `ilspy_session_new(ctx)` without a callbacks pointer and
  `ilspy_last_error()` thread-local without a session argument (lecture 3.4); the cdylib is a
  `None` item declared at evaluation time and cargo runs with `--locked`.
  - `rust/Cargo.toml` workspace with `ilspy-decompiler` (lib) and `ilspy-decompiler-ffi`
    (`crate-type = ["cdylib"]`, `[lib] name = "ilspy_decompiler_ffi"`), `rust/Cargo.lock`
    committed, repo-root `rust-toolchain.toml` (pinned stable, components rustfmt and
    clippy), `rust/target/` in `.gitignore`, `[profile.release]` with `debug-assertions` and
    `overflow-checks` on and a `[profile.ship]` without them, `panic = "unwind"` everywhere.
  - ffi exports `ilspy_version`, `ilspy_session_new`, `ilspy_session_free`, `ilspy_last_error`,
    every export wrapped in `catch_unwind`, as specified in the lecture ("FFI contract").
  - MSBuild target `BuildRustEngine` in the shim csproj (`BeforeTargets="BeforeBuild"`): maps
    `$(RuntimeIdentifier)` to a target triple (empty means host), runs `cargo build` with
    profile `release` for Debug and `ship` for Release, copies the cdylib into `$(OutDir)` and
    registers it as `None` with `CopyToOutputDirectory` and `CopyToPublishDirectory`
    `PreserveNewest` so it flows to every consumer's output and publish directory.
  - Shim: `Interop/NativeMethods.cs` (`[DllImport("ilspy_decompiler_ffi")]`), `RustSession`
    wrapper. Shim test: `ilspy_version` returns the crate version.
  - Gate: G-dotnet, G-rust.

### Phase 1: metadata and type system in Rust

- [x] **3. ECMA-335 metadata reader.** Size M. Subject: `Add the Rust ECMA-335 metadata reader`.
  - `ilspy-decompiler/src/metadata/`: PE headers and sections (enough to find the CLI header
    and resolve RVAs), metadata root and streams, all tables with correct row sizes and coded
    indices, heaps, signature blobs (field, method, property, local, method spec, type spec),
    custom attribute blobs, method bodies with exception handlers. No PDB reading.
  - Gate: G-rust with tests that decode every row of every table of every fixture assembly
    (env `ILSPY_FIXTURE_DIR`, defaulting to the test cases' output directory plus the newest
    shared framework of the `dotnet` on `PATH`), plus a shim test `MetadataListingEquality`
    that calls a debug export `ilspy_debug_list_members` and diffs a canonical listing of
    every table row, decoded signature, cross-table lookup and method body against the same
    listing produced from System.Reflection.Metadata (not `MetadataModule`; lecture 3.7).
  - Done as: format defined in `rust/ilspy-decompiler/src/metadata/listing.rs`, mirrored in
    `ICSharpCode.Decompiler.Rust.Tests/MetadataListing.cs`. Custom attribute values are
    decoded in Rust (with a resolver for enum types) but compared with SRM only in step 4,
    when cross-assembly enum resolution exists.
  - Practical note: the `AvaloniaUI.DiagnosticsSupport` assets in `ILSpy.csproj` depend on the
    configuration at restore time. After `./build.ps1 -Configuration Release` (which restores),
    run `./restore.ps1` again before `./build.ps1 -Configuration Debug --no-restore`, or the
    Debug build fails with CS1061 `WithDeveloperTools`.

- **4. Type system.** Size L, split into 4a-4k (one commit each, each with its own gate) so
  the week of work lands in reviewable pieces. Scope for all of them: interned `TypeId`,
  `TypeDefId`, `MethodId`, `FieldId`, `PropertyId`, `EventId`, `ModuleId`; `TypeData` enum;
  lazy per-`TypeDef` decoding; substitution; base types and inheritance; known types;
  attribute semantics (nullable, dynamic, tuple names, native int, readonly, scoped,
  extension, compiler-generated, `in`/`ref readonly`); specialized members as interned
  `(definition, substitution)` pairs. The reference closure is computed by the host and
  passed as an ordered module table with per-AssemblyRef/ModuleRef targets (lecture 3.8);
  there is no `resolve_assembly` callback.
  Every gate below includes G-rust and a `TypeSystemListingEquality` run (shim test) over the
  corpus (`Corpus.ManagedImages`) against the link-compiled `DecompilerTypeSystem`.
  - [x] **4a. Compilation and type definitions.** Subject: `Add the Rust type system: compilation and type definitions`.
    Module table, name lookup with forwarders and the cycle guard, `TypeDefInfo` (full name,
    kind, known type code, declaring type, type parameter count, enum underlying type),
    interned types from signatures with modifiers stripped, `DirectBaseTypes`. Listing
    (`typesystem/listing.rs`): one line per main-module TypeDef with base and enum types as
    `ReflectionName@DefiningAssembly`, under `TypeSystemOptions.None`.
  - [x] **4b. Member definitions.** Subject: `Add member definitions to the Rust type system`.
    Fields, methods (symbol kind, accessibility, modifiers, body, accessor owner), parameters
    (gaps, `__arglist`, ref kind, optional, constants including decimal constants), type
    parameters with owners (nested types share the outer ones) and constraints, properties
    and events with their accessors. Listing: one line per Field, MethodDef, Property and
    Event row plus type parameter lines, still `TypeSystemOptions.None`. Facts that need later
    steps stay out of the listing until then: property symbol kind and accessibility
    (indexers need `DefaultMemberAttribute`, explicit implementations need 4c, accessibility
    needs base members), attributes, `IsInitOnly`, explicit interface implementations.
  - [x] **4c. Member references and specialization.** Subject: `Resolve member references in the Rust type system`. `MetadataModule.ResolveEntity` /
    `ResolveMethod` / `ResolveField` for MemberRef, MethodSpec and TypeSpec parents:
    signature-based member lookup in the declaring type, varargs, specialized members as
    interned `(definition, substitution)` pairs, `TypeParameterSubstitution`. Listing: one
    line per MemberRef and MethodSpec row of the main module (resolved member, declaring type,
    substituted signature). The IL reader (step 5) depends on this.
  - [x] **4d. Type attribute semantics.** Subject: `Apply type attributes in the Rust type system`.
    `ApplyAttributeTypeVisitor` (dynamic, tuple element names, nullability, native integers
    with and without the attribute, modifier removal) at every site the managed code runs
    it, with nullable contexts and `NullablePublicOnly` per entity; signatures decode raw
    first (`from_sig_raw`, modifiers kept) because the dynamic index counts modifiers.
    Function pointer identity and kind, `in` / `ref readonly` parameters and decimal
    constants also follow the options. The host passes the module's final
    `TypeSystemOptions` (it strips `NativeIntegersWithoutAttribute` below net7). Listing:
    types print structurally (tuples, nullability, function pointer signatures) and run
    under both `None` and `Default`.
  - [x] **4e. Entity flags under Default.** Subject: `Add option-dependent entity flags to the Rust type system`.
    `IsByRefLike`, readonly structs, `HasExtensions`, `IsStatic`, `IsExtensionMethod`,
    `ThisIsRefReadOnly`, `ReturnTypeIsRefReadOnly` (methods, fields, properties), `IsInitOnly`,
    `GetThisParameterScope`, `IsParams` (arrays and params collections), parameter lifetimes
    and `GetEffectiveScope` (`scoped`, `UnscopedRef`, `RefSafetyRules(11)`), unmanaged and
    nullability constraints. Listing extended with these flags, run under `None` and `Default`.
    Moved to their consumers: `ExtensionInfo` (C# 14 extension blocks) to step 68, where the
    AST builder uses it; runtime async (`MethodImplAsync` handling) to step 5, where the IL
    reader uses it.
  - [x] **4f. Custom attribute values.** Subject: `Decode custom attribute values in the Rust type system`.
    `typesystem::attrvalue` (a port of SRM's `CustomAttributeDecoder` over type-system types,
    since its argument types come from the type provider rather than from a name resolver) plus
    `typename`/`reflection` (`TypeName.TryParse` and `ReflectionHelper.ParseReflectionName`);
    decoded values equal SRM's `DecodeValue` (via the managed type provider) for every
    attribute of every corpus image; then `CoClassAttribute` constructors (4c), tested on a
    `MetadataBuilder` fixture because no compiler emits such a reference.
  - The unit suites `TypeSystemLoaderTests` (143 tests, against the compiled
    `ICSharpCode.Decompiler.Tests.dll` with the legacy `mscorlib`) and `ReflectionHelperTests`
    (45) are ported to `cargo test` (`rust/ilspy-decompiler/tests/`). The loader suite asserts
    facts beyond the listing (attribute lists, inheritance, member lookup on parameterized
    types), so it lands with the features in 4g-4k; each of those adds its facts to the
    listing as well, so the corpus equality covers them too.
  - [x] **4g. Fixture harness, options per module, ReflectionHelperTests.** Subject: `Port ReflectionHelperTests to the Rust type system`.
    `TypeSystemOptions` per module (the loader fixture gives `mscorlib` `OnlyPublicAPI`),
    the `OnlyPublicAPI` member filter, parse contexts for `` `n `` / ``` ``n ```, the
    cargo fixture harness (`tests/common`), `ReflectionHelperTests` (all but the null-input
    test, which a `&str` cannot express) and the loader tests the current API expresses.
  - [x] **4h. Member modifiers and inheritance.** Subject: `Add member modifiers and inheritance to the Rust type system`.
    `IsVirtual`/`IsOverride`/`IsOverridable`/`IsSealed`/`IsAbstract` for methods, properties
    and events, property and event accessibility (partial overrides take the base
    property's), property symbol kind (indexers by `DefaultMemberAttribute`, explicit
    implementations by the implemented property), explicit interface implementations
    (MethodImpl rows), `DirectBaseTypes` of every type, `GetAllBaseTypes` /
    `GetNonInterfaceBaseTypes`, `InheritanceHelper.GetBaseMembers`; listing; the loader tests
    on them. Comes before attributes because property attributes need indexers and explicit
    implementations, and parameter attributes the closure-parameter check.
  - [x] **4i. Attribute lists.** Subject: `Add attribute lists to the Rust type system`. `AttributeListBuilder` with the pseudo-attributes
    (Serializable, ComImport, SpecialName, StructLayout, DllImport, PreserveSig, MethodImpl,
    FieldOffset, NonSerialized, MarshalAs, In/Out, Optional, DefaultParameterValue,
    IndexerName, security declarations, TypeForwardedTo, AssemblyVersion), the
    option-dependent ignore list, `HasAttribute`/`GetAttribute(KnownAttribute)`, return-type,
    parameter and type parameter attributes, the constructor of built-in attributes; listing
    line per entity; the loader tests on attributes.
  - [ ] **4j. Member lookup.** `GetMembersHelper` (`GetMethods`/`GetProperties`/`GetFields`/
    `GetEvents`/`GetNestedTypes`/`GetConstructors` on definitions and parameterized types,
    through base types), `GetDelegateInvokeMethod`; the loader tests on them.
  - [ ] **4k. Specialization API.** `Specialize` with composed substitutions, specialized type
    parameters owned by the specialized method, member equality; the remaining loader tests.

### Phase 2: ILAst in Rust, seam 1

- [ ] **5. IL reader, ILAst arena, wire format v1, ILAst materializer.** Size L. Subject: `Add the Rust IL reader and the ILAst materializer`.
  - `il/ast.rs`: one arena per top-level method, `NodeId`, `Node { inst, parent, slot, index,
    il_range, flags }`, the 108-variant `Inst` enum produced by a declarative macro from one
    table (the Rust counterpart of `IL/Instructions.tt`), `Variable`, `replace`, `set_child`,
    `detach`, `insert_in_block`, `remove_from_block`, `descendants_post` (snapshot), cached
    flags invalidated by a generation counter, `check_invariant`. `il/reader.rs` ports
    `IL/ILReader.cs` including the stack-type inference and the `BlockBuilder`.
  - `wire/` writer: length-prefixed binary, varints, string table, version header; v1 carries
    `ILFunction`, variables and all node kinds.
  - Shim: `Interop/IlAstMaterializer.cs` builds link-compiled `ILFunction` objects from v1.
  - Also from 4e: runtime async (`TypeSystemOptions.RuntimeAsync`, `MethodImplAsync`), which
    the IL reader consults.
  - Gate: G-rust; shim test `IlAstReadEquality`: for every method body in every fixture
    assembly, `WriteTo()` of the materialized function equals `WriteTo()` of the managed
    `ILReader` output, with and without debug symbols (the `get_debug_info` callback bridges
    the link-compiled `IDebugInfoProvider`).

- [ ] **6. Seam 1 at N = 0, ratchet, strict mode.** Size S. Subject: `Wire seam 1 of the shim to the Rust engine`.
  - In the shim's `CSharpDecompiler.cs` seam 1 block: ask the engine for the ILAst after the
    first `N` IL transforms (`N` from `rust/port-status.json`), materialize, run the managed
    transforms from `N` on. Status `Unsupported` falls back to the managed path for that
    method and increments a counter; `Error`/`Panic` become a `DecompilerException` through
    the existing per-member error path; strict mode turns `Unsupported` into a failure.
  - Add `rust/port-status.json`, the `PortStatusRatchet` test, the `IlAstPrefixEquality`
    harness (which builds truncated `BlockILTransform` / `StatementTransform` instances so
    nested transforms get their own prefix index), and the empty
    `rust/ilast-divergence-allowlist.json`.
  - Gate: G-dotnet, G-oracle, G-ratchet, G-ilast(0).

- [ ] **7-66. One IL transform per commit, in pipeline order.** Subject pattern:
  `Port <TransformName> to Rust (IL prefix <N>)`. Each commit ports the transform, bumps
  `il_prefix` in the ratchet, and passes G-rust, G-oracle, G-ilast(N) and G-ratchet.
  Divergences that the text oracle accepts go into the allowlist with a reason. Transforms
  that repeat in the pipeline are ported once and the later occurrences are ratchet-only
  commits (size S) that verify the prefix. The nested groups keep their driver shape: a
  post-order block driver over a CFG for `BlockILTransform`, a statement driver with rerun
  for `StatementTransform`.
  - [ ] 7. ControlFlowSimplification (M)
  - [ ] 8. SplitVariables (M)
  - [ ] 9. ILInlining (L)
  - [ ] 10. InlineReturnTransform (S)
  - [ ] 11. RemoveInfeasiblePathTransform (S)
  - [ ] 12. DetectPinnedRegions (M)
  - [ ] 13. YieldReturnDecompiler (L)
  - [ ] 14. AsyncAwaitDecompiler (L)
  - [ ] 15. DetectCatchWhenConditionBlocks (S)
  - [ ] 16. DetectExitPoints (M)
  - [ ] 17. LdLocaDupInitObjTransform (S)
  - [ ] 18. EarlyExpressionTransforms (M)
  - [ ] 19. SplitVariables, second occurrence (S, ratchet only)
  - [ ] 20. RemoveDeadVariableInit (S)
  - [ ] 21. ControlFlowSimplification, second occurrence (S, ratchet only)
  - [ ] 22. DynamicCallSiteTransform (M)
  - [ ] 23. SwitchDetection (L)
  - [ ] 24. SwitchOnStringTransform (M)
  - [ ] 25. SwitchOnNullableTransform (M)
  - [ ] 26. SplitVariables, third occurrence (S, ratchet only)
  - [ ] 27. IntroduceRefReadOnlyModifierOnLocals (S)
  - [ ] 28. BlockILTransform driver and LoopDetection (L)
  - [ ] 29. DetectExitPoints, second occurrence (S, ratchet only)
  - [ ] 30. PatternMatchingTransform (M)
  - [ ] 31. ConditionDetection, inside the second BlockILTransform (L)
  - [ ] 32. **FORK.** Review all allowlisted divergences accumulated so far with the human
        before continuing. Decide whether any of them must be removed (algorithm aligned back
        to the original) or whether the allowlist policy needs tightening. No commit unless
        the review changes files.
  - [ ] 33. LockTransform (M)
  - [ ] 34. UsingTransform (M)
  - [ ] 35. CachedDelegateInitialization (S)
  - [ ] 36. CachedReadOnlySpanInitialization (S)
  - [ ] 37. StatementTransform driver and ILInlining with AllowInliningOfLdloca (M)
  - [ ] 38. ExpressionTransforms (L)
  - [ ] 39. DynamicIsEventAssignmentTransform (S)
  - [ ] 40. TransformAssignment (L)
  - [ ] 41. NullCoalescingTransform (M)
  - [ ] 42. NullableLiftingStatementTransform (M)
  - [ ] 43. NullPropagationStatementTransform (M)
  - [ ] 44. TransformArrayInitializers (M)
  - [ ] 45. TransformCollectionAndObjectInitializers (M)
  - [ ] 46. TransformExpressionTrees (L)
  - [ ] 47. IndexRangeTransform (M)
  - [ ] 48. DeconstructionTransform (M)
  - [ ] 49. NamedArgumentTransform (S)
  - [ ] 50. RemoveUnconstrainedGenericReferenceTypeCheck (S)
  - [ ] 51. UserDefinedLogicTransform (S)
  - [ ] 52. InterpolatedStringTransform (M)
  - [ ] 53. ProxyCallReplacer (S)
  - [ ] 54. FixRemainingIncrements (S)
  - [ ] 55. CopyPropagation (S)
  - [ ] 56. DelegateConstruction, LocalFunctionDecompiler and TransformDisplayClassUsage,
        ported together because they share state (L)
  - [ ] 57. HighLevelLoopTransform (M)
  - [ ] 58. ReduceNestingTransform (M)
  - [ ] 59. RemoveRedundantReturn (S)
  - [ ] 60. IntroduceDynamicTypeOnLocals (S)
  - [ ] 61. IntroduceNativeIntTypeOnLocals (S)
  - [ ] 62. IntroduceScopedModifierOnLocals (S)
  - [ ] 63. ExpandNestedConditionals (S)
  - [ ] 64. AssignVariableNames (M; port `Humanizer` pluralization rules as data, not as a port
        of the vendored library)
  - [ ] 65. `EarlyILTransforms` and `DecompileBodyForAnalysis` paths (the speculative
        pipelines used by `TransformDisplayClassUsage` and the analyzers) served by the engine
        where they run inside a decompile request (M)
  - [ ] 66. Seam 1 strict by default in the test run; `il_prefix` becomes the full pipeline
        length. Subject: `Make seam 1 strict`. Gate: G-dotnet, G-oracle, G-ui with
        `ILSPY_RUST_STRICT=1`. (S)

### Phase 3: semantics, C# AST and AST transforms in Rust, seam 2

- [ ] **67. Semantics.** Size L. Subject: `Port the C# resolver, conversions and overload resolution to Rust`.
  - `semantics/`: `ResolveResult` enum, `conversions.rs`, `overload_resolution.rs`,
    `type_inference.rs`, `member_lookup.rs`, the parts of `CSharpResolver` the expression
    builder uses. Pure spec algorithms, ported 1:1.
  - Gate: G-rust with `ConversionTests` (130), `ExplicitConversionTest` (67),
    `TypeInferenceTests` (66), `OverloadResolutionTests` (22) ported as `cargo test`.

- [ ] **68. C# AST arena, expression and statement builders, wire v2, AST materializer, seam 2 at K = 0.** Size L. Subject: `Add the Rust C# AST builders and the AST materializer`.
  - `csharp/ast/`: arena with roles, one struct per node kind, a side table of annotations
    (symbol, resolve result, IL node id, variable id, let-identifier, implicit return, member
    initializer markers). `expression_builder.rs`, `call_builder.rs`,
    `statement_builder.rs`, `translated_expression.rs`.
  - Wire v2: AST, `ResolveResult` variants consulted by the AST transforms and
    `TextTokenWriter` (Member, Invocation, Local, Conversion, MethodGroup, Constant, Type,
    Namespace, Operator, Dynamic*, Lambda, Tuple, ByReference, This); entity references as
    `(module id, token, generic argument list)`; a `TypeRef` tree.
  - Also from 4e: `ExtensionInfo` (C# 14 extension blocks, `TypeSystemOptions.ExtensionMembers`),
    which the AST builders consult, with its loader test `ExtensionEverything`.
  - Shim: `Interop/AstMaterializer.cs` resolves entity references through
    `MetadataModule.ResolveEntity` and `Specialize`, builds the link-compiled `SyntaxTree`.
  - Seam 2 block: request-level; caller transforms positioned before the built-in list are
    matched by type name against a small recognized set (`RemoveEmbeddedAttributes`,
    `RemoveCompilerAttribute`, `RemoveNamespaceMy` from the tests become engine flags);
    unknown early transforms force managed fallback for the request. Requests with
    `RecordSteps` set always run managed.
  - Gate: G-dotnet, G-oracle, G-ratchet, G-ast(0).

- [ ] **69-84. One AST transform per commit, in order.** Subject pattern:
  `Port <TransformName> to Rust (AST prefix <K>)`. Gate per commit: G-rust, G-oracle,
  G-ast(K), G-ratchet.
  - [ ] 69. PatternStatementTransform, with hand-written matchers instead of a port of
        `CSharp/Syntax/PatternMatching` (L)
  - [ ] 70. RewriteVisualBasicWithEventsInInitializeComponent (S)
  - [ ] 71. ReplaceMethodCallsWithOperators (M)
  - [ ] 72. IntroduceUnsafeModifier (S)
  - [ ] 73. AddCheckedBlocks (M)
  - [ ] 74. DeclareVariables (L)
  - [ ] 75. TransformFieldAndConstructorInitializers (M)
  - [ ] 76. PrettifyAssignments (M)
  - [ ] 77. IntroduceUsingDeclarations (L)
  - [ ] 78. IntroduceQueryExpressions (L)
  - [ ] 79. CombineQueryExpressions (M)
  - [ ] 80. NormalizeBlockStatements (S)
  - [ ] 81. FlattenSwitchBlocks (S)
  - [ ] 82. RenameVisualBasicAnonymousTypes (S)
  - [ ] 83. FixNameCollisions (M)
  - [ ] 84. AddXmlDocumentationTransform stays managed: ratchet-only commit that records
        `ast_prefix` as "all but AddXmlDocumentationTransform" and documents why (it only adds
        comments from `IDocumentationProvider`, which the UI implements) (S)

- [ ] **85. Seam 2 strict by default.** Size S. Subject: `Make seam 2 strict`.
  - Gate: G-dotnet, G-oracle, G-ui, `ILSpy.BamlDecompiler.Tests`, PdbGen, all with
    `ILSPY_RUST_STRICT=1`. Seam 1 stays available only for the prefix-equality harness.

### Phase 4: UI, CI, performance

- [ ] **86. UI pass.** Size M. Subject: `Verify the ILSpy UI on the Rust engine`.
  - Debug Steps (`RecordSteps`, `StepLimit`) route to the managed engine by design; verify
    the pane still works. Verify `SelectCtorTransform` / `SelectFieldTransform`, bookmarks,
    PDB generation, go-to-definition and tooltips on materialized entity references
    (including specialized members), cancellation of a long decompile, and the error comment
    produced by a forced engine panic. Record the manual smoke list and results in the
    lecture.
  - Gate: G-ui, plus the smoke list in the lecture ticked.

- [ ] **87. CI and packaging.** Size M. Subject: `Build the Rust engine in CI and ship it with ILSpy`.
  - `.github/workflows/build-ilspy.yml`: Rust toolchain step per job (reads
    `rust-toolchain.toml`, cache keyed on `rust/Cargo.lock`), targets per job as listed in
    the lecture, `cargo fmt --check`, `clippy`, `cargo test` before `dotnet build`; Linux job
    gets one extra `dotnet test --project ICSharpCode.Decompiler.Rust.Tests` line; the
    ilspycmd RID package job and the NuGet pack job carry the native assets under
    `runtimes/<rid>/native/`.
  - Regenerate `BuildTools/ILSpy-x64.msi.filelist` (and the VSIX file list if it changes)
    with `BuildTools/create-filelists.ps1`.
  - Gate: every job of `build-ilspy.yml` green on the PR, `git diff --exit-code` after the
    file list regeneration clean.

- [ ] **88. Performance comparison.** Size M. Subject: `Measure the Rust engine against the managed decompiler`.
  - Implement the methodology in the lecture ("Performance comparison"): `--perf` and
    `--side` in `TestTools/decompdiff.cs`, the counting allocator behind `ilspy_stats`,
    `rust_elapsed_ns` in the response header, `criterion` benches under
    `rust/ilspy-decompiler/benches/`.
  - Record CPU (P50, P95, total) and memory (peak RSS, Rust peak live, managed allocated
    bytes) for the three reference inputs and the top-200 NuGet corpus in the lecture.
  - Gate: numbers in the lecture, `cargo bench` runs, decompdiff report attached to the PR.

- [ ] **89. FORK: optional tail.** Present to the human, with the measured numbers: port the
  disassembler, port `CSharpOutputVisitor` for standalone Rust output, add a Rust portable
  PDB reader and assembly resolver for `cargo`-only use, port `WholeProjectDecompiler`. Each
  is a separate decision and a separate checklist to be appended here.
