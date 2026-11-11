# Porting ICSharpCode.Decompiler to Rust: architecture and plan

Audience: senior developers who know ILSpy's decompiler pipeline and are comfortable with
Rust. This document records what was found when the port was planned, which instructions of
the original brief survived contact with the code base, the decisions taken, the architecture
that follows from them, and the plan. `RustPort_AgentInstructions.md` holds the executable
checklist. Both files are living documents: the agent performing the port updates them with
every step.

## 1. The brief, and what the code base says about it

The brief asked for:

1. an idiomatic Rust port of `ICSharpCode.Decompiler`;
2. a C# shim project that keeps the public surface and namespaces so that callers only switch
   a project reference;
3. `ICSharpCode.Decompiler.Tests` as the oracle, with ideally only its project reference
   changed;
4. no edits to `ICSharpCode.Decompiler`, `ICSharpCode.ILSpyCmd`,
   `ICSharpCode.Decompiler.PowerShell`, `ILSpy.AddIn.VS2022`;
5. the UI chain re-pointed to the shim so ILSpy runs on the port at the end;
6. Rust-idiomatic data structures, with algorithm changes allowed;
7. every instruction challenged.

Instruction 7 was taken literally. Instructions 2, 3 and 4 do not survive unchanged.

### 1.1 Size and shape of the library

| Area | Lines | Notes |
|---|---|---|
| IL/Transforms | 26,040 | about 60 transforms in 39 top-level pipeline entries |
| CSharp (ExpressionBuilder, StatementBuilder, CallBuilder, CSharpDecompiler) | 17,544 | IL AST to C# AST |
| CSharp/Syntax | 15,717 | 138 node kinds, generator-emitted match/visitor/slot code |
| IL (reader, variables, generated Instructions.cs) | 14,259 | 108 ILAst instruction kinds from a T4 template |
| IL/ControlFlow | 11,418 | loops, conditions, switches, state machines |
| TypeSystem + Implementation | 20,311 | the type system the UI browses with |
| CSharp/Resolver | 9,524 | conversions, overload resolution, type inference |
| IL/Instructions (hand-written halves) | 9,149 | |
| CSharp/Transforms | 7,509 | 16 AST transforms |
| CSharp/OutputVisitor | 7,308 | printing, token writers |
| Util, Metadata (+LightJson), Disassembler, Semantics, ProjectDecompiler, Documentation, FlowAnalysis, DebugInfo, Output, Solution, Humanizer, DebugSteps | about 33,000 | |
| **Total** | **174,763** | 583 files, netstandard2.0, about 760 public types |

The library's only runtime dependencies are `System.Reflection.Metadata` (SRM) and
`System.Collections.Immutable`. SRM is not an implementation detail: `MetadataFile.Metadata`
returns a `MetadataReader`, `PEFile.Reader` a `PEReader`, `IEntity.MetadataToken` an
`EntityHandle`, and `MetadataModule.Resolve*` take handles. Any "same public surface" promise
therefore includes SRM objects.

### 1.2 How the oracle uses the API

`ICSharpCode.Decompiler.Tests` has two very different halves.

About 308 test cases are **text oracles**: Pretty (132), ILPretty (73), Correctness (45), Ugly
(19), VBPretty (18), Disassembler (7), Roundtrip (14). All of them go through
`Helpers/Tester.cs` (`DecompileCSharp`, line 1048): `new PEFile`, `UniversalAssemblyResolver`,
`new DecompilerTypeSystem(module, resolver, settings)`, `new CSharpDecompiler(typeSystem,
settings)`, three test-defined `IAstTransform`s inserted at index 0 of `AstTransforms`,
`DecompileWholeModuleAsSingleFile(sortTypes: true)`, then `CSharpOutputVisitor`. PdbGen (37)
adds `PortablePdbWriter` on top of the same path.

About 1,550 test cases are **API tests** on the object model: `TypeSystemLoaderTests` (144),
`ReflectionHelperTests` (46), the Semantics suites on `CSharpResolver` and `CSharpConversions`
(285), `IdStringProviderTests` (239), `CSharpAmbience` and `ILAmbience` (210),
`InsertParenthesesVisitor`, `PatternMatching` (20), Trivia, `DataFlowTest` (builds an
`ILFunction` by hand), `DebugStepRecordingTests` (custom `IILTransform`, `Stepper`),
ProjectDecompiler (82), Metadata (35), Util (73). The library grants `InternalsVisibleTo` to
this test project and to `ILSpy.Tests`. The tests also reference `ICSharpCode.ILSpyX` for
`DebugInfoUtils`.

So "only change the project reference" is only true for the text half if the shim exposes the
full, internals-visible object model. It is true for the whole suite only if that object model
is the real C# one.

### 1.3 How the UI uses the API

Every consumer walks and extends the object model:

- `Output/TextTokenWriter.cs` (line 120 on) reads `GetSymbol()`, `ResolveResult` annotations,
  `NewObj` and `DynamicInvokeConstructorInstruction` annotations off AST nodes to emit
  navigable references. The UI's `CSharpHighlightingTokenWriter` and
  `BookmarkDebugInfoCollector` subclass `DecoratingTokenWriter`; the latter reads
  `ILInstruction` annotations and walks to the owning `ILFunction`.
- `ILSpy/Languages/CSharpLanguage.cs` implements `IAstTransform` twice (select a single
  constructor or field out of a decompiled type) and subclasses `WholeProjectDecompiler`.
- `SequencePointBuilder` and `DebugInfo/PortablePdbWriter.cs` read `BlockContainer`
  annotations, `ILFunction.Method`, `MoveNextMethod`, variables and their state machine fields.
- The Debug Steps pane uses `Stepper`, `RecordSteps`, `StepLimit` and ILAst text dumps.
- `ICSharpCode.ILSpyX.LoadedAssembly` implements `IAssemblyResolver`; images come from
  bundles, packages and streams, not only from files. `PortableDebugInfoProvider` and
  `MonoCecilDebugInfoProvider` implement `IDebugInfoProvider`.
- `ICSharpCode.BamlDecompiler` implements `IDecompilerTypeSystem`, `IModule` and
  `ITypeDefinition` synthetically and runs
  `function.RunTransforms(CSharpDecompiler.GetILTransforms(), context)` on its own
  `ILFunction` (`Rewrite/ConnectionIdRewritePass.cs:191`).
- `ILSpy/Options/DecompilerSettingsViewModel.cs:49` reflects over `DecompilerSettings`
  properties and their `[Description]`/`[Category]` attributes.
- `MetadataFile` appears in 95 UI files and 44 ILSpyX files; analyzers and search strategies
  are written against `ITypeDefinition` and `IMethod`.

### 1.4 The cliff hidden in instruction 4

`ICSharpCode.ILSpyCmd/ICSharpCode.ILSpyCmd.csproj` (lines 95-97) references `ICSharpCode.ILSpyX`,
`ICSharpCode.Decompiler` and `ICSharpCode.BamlDecompiler` directly. If ILSpyX or BamlDecompiler
switch to a shim that defines the same namespaces, ilspycmd sees every decompiler type twice and
fails with CS0433. "Do not touch ilspycmd" and "re-point ILSpyX" cannot both hold.

### 1.5 Rust ecosystem

There is no Rust IL-to-C# decompiler to build on. ECMA-335 readers exist only as pre-1.0 crates
(`dotnetdll`, `clrmeta`, `windows-metadata`); none carries signature semantics, generic
instantiation, attribute semantics or portable PDB support at the level the decompiler needs.
Binding generators (`csbindgen`, `interoptopus`, `uniffi-bindgen-cs`) exist but the C ABI
surface here is nine functions. The NuGet convention for native assets is
`runtimes/<rid>/native/`.

## 2. Challenges to the brief and the decisions taken

| Instruction | Finding | Decision |
|---|---|---|
| Shim keeps the public surface | A proxy over FFI would have to reproduce about 760 types with identity semantics, mutable trees, callbacks in both directions, SRM handles and internals visible to tests. That is a second implementation, not a shim. | The shim **link-compiles the original sources** (no copies, no edits) and forks exactly one file, `CSharp/CSharpDecompiler.cs`, to insert two seams. The public surface is kept because it is the original code. |
| Only switch the test project reference | True once the shim is the original code plus seams. The API half of the suite then tests the link-compiled C# layers, not Rust. | Accept and say so. The Rust crate gets `cargo test` ports of the semantic suites (type system loader, reflection helper, conversions, type inference, overload resolution). The text half is the oracle for the engine. |
| Do not touch ilspycmd | Impossible together with re-pointing ILSpyX (CS0433). Alternatives: link-compiled twin projects `ICSharpCode.ILSpyX.Rust` / `ICSharpCode.BamlDecompiler.Rust` with distinct assembly names, or relaxing the guardrail. | **Human decision: relax the guardrail.** Only the three `ProjectReference` lines in ilspycmd's csproj change. ilspycmd, its tests and its RID packages then run on the engine too, which adds `ICSharpCode.ILSpyCmd.Tests` as a second oracle. PowerShell and the AOT gate stay on the original. |
| Port ICSharpCode.Decompiler (all of it) | The UI is a .NET application; it needs a managed metadata reader and type system for tree nodes, analyzers, search and the metadata tables view regardless. Everything downstream of the AST (output visitor, token writers, PDB writer, project and solution writers, Debug Steps, BamlDecompiler's IL pass) walks managed objects. Porting those buys the UI nothing and forces an FFI object model. | **Human decision: engine only.** Rust owns IL reader, ILAst, IL transforms, semantics, C# AST construction and AST transforms, with its own internal metadata reader and type system. The browsing layers, output visitor, PDB, project/solution writers, documentation, Debug Steps, disassembler and BamlDecompiler's IL pass stay C#. |
| Progress visible per commit | A big-bang port keeps the .NET suite unchanged for months. | **Human decision: incremental two-seam fallback** with a committed ratchet, one transform per commit, the full suite green at every commit. |
| Names: `libicsdecompiler`, `ICSharpCode.DecompilerLib` | Cargo adds the `lib` prefix itself; crate names are kebab-case; the engine's NuGet title is "ILSpy Decompiler Engine". | **Human decision:** crates `ilspy-decompiler` and `ilspy-decompiler-ffi` (cdylib `ilspy_decompiler_ffi`), shim `ICSharpCode.Decompiler.Rust`, tests `ICSharpCode.Decompiler.Rust.Tests`. |
| Rust-idiomatic data structures, algorithms may change | Agreed, with one constraint: the pipeline shape (ordered transforms, block driver, statement driver with rerun) is what the transforms assume and what the differential oracle keys on. | Idiomatic internals, same pipeline shape. Algorithm divergences are allowed when the text oracle passes and are recorded in an allowlist. |

## 3. Architecture

### 3.1 The shim: link-compiled original plus one forked file

`ICSharpCode.Decompiler.Rust.csproj` is a clone of the original csproj (netstandard2.0,
generator analyzer reference, strong-name key, `ReferenceAssemblyAnnotator` plus the
`Microsoft.NETCore.App.Ref` download for nullability, `STEP` and overflow checks in Debug,
`WarningsAsErrors=nullable`, same `RuntimeIdentifiers`, locked restore) whose compile items are
`..\ICSharpCode.Decompiler\**\*.cs` minus `bin`, `obj`, `CSharp\CSharpDecompiler.cs`,
`Properties\AssemblyInfo.cs` (its attributes name the original) and the version-info template.
`Properties\DecompilerVersionInfo.cs` is generated into the original's folder by its build and
is gitignored; the shim links it and takes a `ProjectReference` with
`ReferenceOutputAssembly="false"` on the original purely for build ordering.

Why netstandard2.0 and not net10: the sources define `System.Index` and several
`System.Diagnostics.CodeAnalysis` attributes unguarded (`Util/Index.cs`, `NRTAttributes.cs`,
`TrimAttributes.cs`). On a .NET Core target those are CS0436 and, with warnings as errors,
fatal.

`CSharp/CSharpDecompiler.cs` is the only diverging copy. It is `public class`, not partial, and
the original must not change, so the shim carries its own copy with two marked blocks:
`RUST-SEAM-1` around `function.RunTransforms(...)` (line 2440 of the original; as built, three blocks, section 3.22) and
`RUST-SEAM-2` around the AST transform loop. A shim test strips the marked blocks and asserts
byte equality with the original file, so upstream changes to `CSharpDecompiler.cs` show up as
a red test rather than a silent fork.

**Assembly identity (decided in item 1).** The shim's `AssemblyName` is `ICSharpCode.Decompiler`
with the original's key and version; only the package id (`ICSharpCode.Decompiler.Rust`) and
the project name differ. The plan first assumed an assembly named after the project. Item 1
found that the oracle forbids it: `StepperTesting`, `ProjectFileWriter*Tests` and
`WholeProjectDecompilerTests` open `ICSharpCode.Decompiler.dll` from the test output directory,
and `ILSpy.BamlDecompiler.Tests/TransitiveReferenceTests` asserts a reference named
`ICSharpCode.Decompiler`; none of them may be edited. Keeping the identity has two good side
effects: third-party plugins compiled against the `ICSharpCode.Decompiler` NuGet bind to the
shim with no second identity in the process, and `BuildTools/ILSpy-x64.msi.filelist` keeps its
decompiler line. Consequence to accept: the original and the shim must never meet on one
reference path or in one output directory, because they are indistinguishable by name. The
shim's build-ordering `ProjectReference` to the original therefore carries
`ReferenceOutputAssembly="false" Private="false" PrivateAssets="all"`; the lock files of every
consumer confirm the original is gone from their graphs, and a hash check of
`ICSharpCode.Decompiler.dll` in `ILSpy/bin`, `ICSharpCode.ILSpyCmd/bin` and
`ICSharpCode.Decompiler.Tests/bin` shows the shim's build (PowerShell keeps the original's).
The type-forwarding alternative still fails with CS0433 as soon as the original is on the same
reference path.

### 3.2 Two seams and an honest fallback

The managed pipeline per method is: `ILReader` -> `ILFunction` -> `RunTransforms(IL
transforms)` -> `StatementBuilder.ConvertAsBlock` -> C# AST; then per request: AST transforms
-> caller transforms -> output visitor. The engine replaces this from the front, prefix by
prefix.

**Seam 1 (per method body).** The shim asks the engine for the ILAst of a method after the
first `N` IL transforms, materializes it into link-compiled `ILFunction` objects, and runs
the managed transforms from `N` on. IL transforms are effectively pure in the method and its
module (the only `DecompileRun` use in `IL/` is a pass-through in `YieldReturnDecompiler`, plus
`UsingScope` for name lookup), so a prefix handover is sound. A per-method `Unsupported`
answer falls back to the managed reader and transforms for that method and is counted. An
`Error` or `Panic` answer lands in the existing per-member error path, so the output shows the
usual `Error decompiling @06000xxx ...` comment with the Rust message and backtrace.

**Seam 2 (per request).** After the statement builder, the engine returns the C# AST after the
first `K` AST transforms; the shim materializes it and runs managed transforms `K+1..16`, then
the caller's transforms, then whatever the caller does with the `SyntaxTree`. AST transforms
need the whole annotated tree, so member-level mixing is impossible here and fallback is per
`Decompile*` call. One ordering hazard: `Tester.cs` inserts its three transforms at index 0,
before `IntroduceUsingDeclarations`; running them after the engine's built-ins changes the
`using` output. Rule: early caller transforms are matched by type name against a small
recognized set that becomes engine flags; any other early transform forces managed fallback
for the request. Requests with `RecordSteps` set (the Debug Steps pane) always run managed.

**Ratchet.** `rust/port-status.json` records `il_prefix`, `ast_prefix` and the share of
methods the engine served in the last oracle run. A shim test fails when a run measures less
than the committed values. `ILSPY_RUST_STRICT=1` disables fallback entirely; the final state
runs strict by default.

### 3.3 Wire format and materializers

The decision that shapes everything: **the engine does not return text.** A token stream was
considered and rejected because every public entry point returns a `SyntaxTree` and every
consumer walks it (section 1.3). The payload is (i) the ILAst of a function and (ii) the C#
AST with its annotations. The shim materializes both into the link-compiled object model, and
everything downstream (output visitor, UI token writers, bookmarks, sequence points, PDB
writer, caller transforms) runs unchanged. `CSharpOutputVisitor` is therefore not ported: a
Rust printer would have no consumer, since the UI needs `TokenWriter` callbacks with
`AstNode` arguments.

Format: hand-rolled length-prefixed binary with varints, a string table and a version header.
Not FlatBuffers (a third toolchain on CI), not JSON (whole-module ASTs are tens of megabytes).
Version 1 carries `ILFunction`, variables and all node kinds. Version 2 adds the AST, the
`ResolveResult` variants that the AST transforms and `TextTokenWriter` consult (Member,
Invocation, Local, Conversion, MethodGroup, Constant, Type, Namespace, Operator, the dynamic
variants, Lambda, Tuple, ByReference, This; everything else collapses to a typed result),
annotations (IL node id, variable id, let-identifier, implicit return, initializer markers),
entity references as `(module id, metadata token, generic argument list)` and a small `TypeRef`
tree (Def, Inst, Array, Pointer, ByRef, FnPtr, TypeParam, Tuple, Modified, Special). The
materializer resolves entity references through `MetadataModule.ResolveEntity` and
`Specialize(TypeParameterSubstitution)`. Two hand-maintained kind enumerations (Rust and C#)
with a test asserting the lists match.

### 3.4 FFI contract

Hand-written `extern "C"` in `ilspy-decompiler-ffi`, classic `[DllImport]` on the netstandard2.0
shim, callbacks as `[UnmanagedFunctionPointer(Cdecl)]` delegates kept alive in the session
object. No binding generator: nine declarations do not justify a toolchain.

Exports: `ilspy_version`, `ilspy_session_new(ctx)`, `ilspy_session_free`,
`ilspy_session_add_module(session, image_ptr, image_len, name) -> module_id`,
`ilspy_decompile(session, request_ptr, request_len, out Buf) -> Status`, `ilspy_buf_free`,
`ilspy_cancel(session, request_id)`, `ilspy_last_error()`, `ilspy_stats(session)`.

As built in item 2: `ilspy_session_new` takes only `ctx`; the host-callback table is added
by the item that first calls back (item 4 for `resolve_assembly`, item 5 for
`get_debug_info`), so its layout is fixed when its first user exists instead of guessed now.
`ilspy_last_error` reads a thread-local slot and takes no session: a failed
`ilspy_session_new` has no session to store the message in, and concurrent requests on one
`Send + Sync` session would overwrite each other's message in a per-session slot. The pointer
stays valid until the next export call on the same thread. The shim wraps the session in a
`SafeHandle` (`Interop/RustSession.cs`) so it is freed exactly once, also on finalization.

Modules: superseded in item 4a (section 3.8). Instead of a `resolve_assembly` callback the
host passes the whole module table, each image as the `PEReader.GetEntireImage()` pointer and
length (zero copy, the `MetadataFile` keeps the image alive, the engine never frees inputs)
plus per-reference target indices. `get_debug_info(ctx, module_id, method_token, kind,
out Buf)` bridges the five-member `IDebugInfoProvider`, so the engine needs no PDB reader and
ILSpyX's Mono.Cecil provider for Windows PDBs keeps working. As built in item 5d (section
3.21) the callback has no module id and writes into an engine-owned buffer.

Threading and cancellation: `Session: Send + Sync`, immutable module data behind a read lock,
per-request arenas, an `Arc<AtomicBool>` per request polled where the managed code calls
`ThrowIfCancellationRequested`, wired to `CancellationToken.Register`.

Memory: engine-allocated output buffers that the shim copies into a `byte[]` and frees
immediately; callback outputs pinned by the shim for the duration of the call and copied by
the engine.

Errors: `catch_unwind` in every export, `panic = "unwind"`, a panic hook that captures a
backtrace. `Status { Ok, Unsupported, Cancelled, Error, Panic }` maps to fallback,
`OperationCanceledException`, and `DecompilerException` wrapping a `RustEngineException`, so
`CSharpDecompiler.GetErrorHeadline` keeps its format.

Settings: the shim reflects over `DecompilerSettings` into name/value pairs; the engine rejects
unknown names so a new setting cannot be silently ignored.

### 3.5 Rust internals

```
rust/
  Cargo.toml  Cargo.lock  (rust-toolchain.toml lives at the repository root)
  ilspy-decompiler/src/
    metadata/    pe, tables, heaps, coded_index, signature, custom_attribute, il_body
    typesystem/  types, entities, resolve, known, substitution, inheritance, attributes
    il/          reader, ast, variable, slot, flags, visit, write
                 control_flow/ (cfg, dominance, loop_detection, condition_detection, switch_*, exits)
                 transforms/   (one file per transform)
    semantics/   resolve_result, conversions, overload_resolution, type_inference, member_lookup
    csharp/      ast/, expression_builder, call_builder, statement_builder, translated_expression,
                 transforms/ (one file per transform), decompiler, settings
    wire/        writer, ilast, ast, request
    session.rs
  ilspy-decompiler-ffi/src/lib.rs
```

**Metadata reader.** Own implementation of tables, heaps, coded indices, signature and custom
attribute blobs, method bodies with exception handlers and the minimal PE needed to find them.
Pre-decoding in C# and shipping a decoded type system across the boundary was weighed and
rejected: that serializer would be larger than the reader, and the engine needs lazy access to
attributes, nested types, constraints and overload sets across all referenced assemblies.

**Type system.** Interned ids (`TypeId`, `TypeDefId`, `MethodId`, `FieldId`, `PropertyId`,
`EventId`, `ModuleId`) so that structural `IType.Equals` becomes `==` and ids are `Copy`.

```rust
pub enum TypeData {
    Def(TypeDefId),
    Param { owner: TypeParamOwner, index: u16 },
    Inst { generic: TypeId, args: Box<[TypeId]> },
    Array { elem: TypeId, rank: u8 }, Pointer(TypeId), ByRef(TypeId),
    FnPtr(Box<FnPtrSig>), Modified { ty: TypeId, modifier: TypeId, required: bool },
    Tuple { elems: Box<[TypeId]>, names: Option<Box<[Option<Arc<str>>]>> },
    Nullable { ty: TypeId, nullability: Nullability },
    Special(SpecialType),
}
```

Members of a `TypeDef` are decoded lazily once (`OnceLock`); specialized members are interned
`(definition, substitution)` pairs, the counterpart of `SpecializedMethod`.

**ILAst.** One arena per top-level method; nested lambdas and local functions are ordinary
nodes in the same arena, because `DelegateConstruction` and `LocalFunctionDecompiler` move
instructions between functions and that must be a parent update, not a cross-arena copy.

```rust
pub struct Function { nodes: Vec<Node>, vars: Vec<Variable>, root: NodeId, method: MethodId, generation: u32 }
pub struct Node { inst: Inst, parent: Option<NodeId>, slot: SlotId, index: u32, il_range: Interval, flags: Cell<Option<Flags>> }
pub enum Inst { /* 108 variants generated by a declarative macro from one table */ }

impl Function {
    pub fn alloc(&mut self, inst: Inst) -> NodeId;
    pub fn replace(&mut self, old: NodeId, new: NodeId);        // ILInstruction.ReplaceWith
    pub fn set_child(&mut self, parent: NodeId, slot: SlotId, index: u32, child: NodeId);
    pub fn detach(&mut self, id: NodeId);
    pub fn insert_in_block(&mut self, block: NodeId, pos: usize, inst: NodeId);
    pub fn remove_from_block(&mut self, block: NodeId, pos: usize) -> NodeId;
    pub fn descendants_post(&self, id: NodeId) -> Vec<NodeId>;   // snapshot, like Descendants
    pub fn flags(&self, id: NodeId) -> Flags;                    // cached, generation-invalidated
}
```

Variable load, store and address counts are maintained centrally in `set_child` and `detach`,
which is where the C# code does it (`SetChildInstruction` to `ILVariable.AddLoadInstruction`).
Pattern helpers are free functions (`match_stloc(f, id) -> Option<(VarId, NodeId)>`). The
pipeline shape is preserved: `IlTransform`, a post-order `BlockTransform` driver over a CFG
indexed by block id, and a `StatementTransform` driver with rerun. `context.Step(...)` becomes
a `tracing` event or nothing; `Stepper` is not ported.

**C# AST.** Same arena idea with roles, one struct per node kind, child ids, a generic
`children()` for the serializer, annotations in a side table. `CSharp/Syntax/PatternMatching`
(Backreference, Choice, Repeat) is not ported; `PatternStatementTransform` gets hand-written
matchers, and the 20 pattern-matching tests keep running against the link-compiled C#.

**Semantics.** `ResolveResult` is an immutable enum. Conversions, overload resolution and type
inference are pure specification algorithms over the type system and port 1:1.

**Where plain trees are fine.** Decoded signatures before interning, `LongSet`, `Interval`,
switch labels, `Conversion`, `ResolveResult`, custom attribute values and `FullTypeName` are
ordinary `Box` or `Rc` values. `Rc<RefCell>` only for per-request caches, never inside the two
trees.

### 3.6 Oracle and differential tooling

- The text half of `ICSharpCode.Decompiler.Tests` runs unchanged through the shim (reference
  switch only). `ICSharpCode.ILSpyCmd.Tests` becomes a second text oracle once ilspycmd is
  re-pointed.
- ILAst equality per transform prefix needs no Rust printer: the shim materializes the engine's
  ILAst and calls the link-compiled `ILFunction.WriteTo` on both sides, one printer, no format
  drift. `ilspycmd --ilast --after-transform` cannot stop inside the `BlockILTransform` and
  `StatementTransform` groups, so the shim's harness builds truncated group instances to give
  nested transforms their own prefix index.
- Where the engine legitimately diverges (allowed by the brief), the divergence goes into
  `rust/ilast-divergence-allowlist.json` with transform, fixture, method and reason; the text
  oracle decides. Dump diffing stops once the IL phase runs strict; after that only the text
  oracle and the `decompdiff` corpus matter.
- API-level tests keep running against the link-compiled C#. They are honest about what they
  test. The engine gets `cargo test` ports of the type system loader, reflection helper,
  conversion, type inference and overload resolution suites.

### 3.7 The metadata reader and its oracle (item 3)

The plan's gate compared a listing of types, members and signatures against the
link-compiled `MetadataModule`. That comparison would test the type system rather than the
reader: `MetadataModule` turns signatures into tuples, `dynamic`, `nint`, nullable reference
types and strips modifiers unless `KeepModifiers` is set, all of which is item 4's scope. The
exact managed counterpart of a metadata reader is System.Reflection.Metadata (SRM), so the
item-3 oracle is SRM; `MetadataModule` becomes the oracle in item 4.

The reader therefore reproduces SRM's observable behavior, including its edge cases, verified
by decompiling the net11 shared-framework `System.Reflection.Metadata.dll` with this repo's
`ilspycmd`:

- Section data for an RVA is `min(VirtualSize, SizeOfRawData)` bytes of the containing
  section; method bodies read only the first extra data section.
- Coded indices keep their table when the row is 0 (`02000000`, not `00000000`); an invalid tag
  fails only the column that holds it.
- Lookups use SRM's own algorithms on the raw column: binary search (and range expansion
  around the hit) for NestedClass, ClassLayout, FieldLayout, FieldRVA, FieldMarshal, Constant,
  ImplMap, MethodSemantics, InterfaceImpl and GenericParamConstraint; linear search for
  EventMap and PropertyMap. Unsorted tables thus give the same answer SRM gives. Only
  CustomAttribute is re-sorted by SRM, and the listing enumerates that table by row.
- Signature kinds 0-5 and 9 all mean "method"; type sequences (locals, method specs, generic
  arguments) must not be empty; `CLASS`/`VALUETYPE` may not name a TypeSpec, modifiers may.
- The `#Strings` heap is trimmed to one trailing NUL before its size is reported.

The listing (`rust/ilspy-decompiler/src/metadata/listing.rs`, mirrored in
`ICSharpCode.Decompiler.Rust.Tests/MetadataListing.cs`) prints table and heap sizes, every
row of every table with every column, decoded signatures, member ranges through the pointer
tables, the lookups above, and method bodies with exception clauses and an FNV-1a hash of the
IL. A column SRM rejects prints `!bad` on both sides.

Measured at item 3 (Windows, Debug): `MetadataListingEquality` passes for 3,455 images (the
decompiler itself, every file of the net11 shared framework, 3,260 compiled test case
fixtures). The Rust corpus test reads the same fixtures cell by cell and decodes 142,066
custom attributes; 18,565 of them need an enum type from another assembly and are counted, not
failed, until item 4 resolves them. One real-world quirk surfaced: older F# compilers store a
field signature in StandAloneSig (`FSharp.Core.dll`, row 1, `06 08`); SRM rejects it, the
listing prints `!bad` on both sides, and the corpus test accepts exactly that case.

### 3.8 The compilation is a host-built module table (item 4a)

The plan had the engine ask the host to resolve each assembly reference
(`resolve_assembly` returning image bytes). Reading `DecompilerTypeSystem.InitializeCoreAsync`
showed how much policy that would push into Rust: the reference closure walks exported types
and module references, adds implicit references by target framework, repairs cyclic type
forwarders by re-resolving next to the forwarding assembly, keeps the highest version per
assembly name (a repaired file wins outright), and adds a `MinimalCorlib` for missing known
types. `FindModuleByReference` then matches full assembly names (public-key tokens from SHA-1,
retargetable flags) before falling back to short names. All of that is resolver behavior the
UI already configures (bundles, packages, `ILSpyX.LoadedAssembly`).

Decision: the host computes the closure with the link-compiled code and passes the outcome.
`Interop/RustModuleTable.cs` describes every metadata module of a managed `ICompilation` in
compilation order, with the module each AssemblyRef and ModuleRef row resolves to. The engine
only resolves names inside that table: `GetDeclaringModule` per TypeRef, top-level lookup
(last row wins, as in `MetadataFile.GetTypeDefinition`), forwarders keyed by full name,
nested types first-match by name and total type parameter count, forwarder chains matched
case-insensitively by short name with the managed busy-guard semantics, and the
first-module-wins search when a scope resolves to nothing. Agreement on the closure is then
true by construction, and the equality test isolates name resolution. `MinimalCorlib` has no
image; names that only it defines stay unknown in the engine, which the listing marks as
`@?` on both sides.

Measured at item 4a (Windows, Debug): `TypeSystemListingEquality` passes for 3,442 managed
images (each loaded with `UniversalAssemblyResolver` and its full closure), comparing per
TypeDef the reflection name, kind, known type code, declaring type, type parameter count,
enum underlying type and the direct base types with their defining assembly.

### 3.9 Member definitions (item 4b)

Members follow `MetadataMethod`, `MetadataField`, `MetadataParameter`, `MetadataProperty`,
`MetadataEvent` and `MetadataTypeParameter` rule for rule. Facts worth knowing because they
are not what one would guess:

- A member's declaring type is SRM's `BinarySearchForSlot` over the TypeDef list columns (the
  last of several TypeDefs sharing a list start wins), not a range lookup.
- Nested types do not own their leading type parameters: indices below the enclosing type's
  count are the enclosing type's parameter objects, so `TypeParamOwner` points there.
- Decimal constants on optional parameters are decoded from `DecimalConstantAttribute` even
  under `TypeSystemOptions.None`; only fields gate them on the option.
- Raisers are not C# accessors (`MethodSemanticsLookup` filters them out), so a raiser's
  symbol kind is `Method`.
- Without `TypeSystemOptions.FunctionPointers`, `delegate*` keeps its own type but
  `GetDefinition()` answers `UIntPtr`'s definition.
- A property whose accessor has no declaring type throws a `NullReferenceException` in the
  managed code; both listings print `!bad` for it.

Measured at item 4b: `TypeSystemListingEquality` passes for the same 3,442 images with one
line per TypeDef, Field, MethodDef, Property and Event row and per type parameter list.

### 3.10 Member references (item 4c)

`typesystem/resolve.rs` ports `ResolveMethod`, `ResolveMethodReference`,
`ResolveMethodSpecification`, `ResolveFieldReference`, `FindMethod`, `CreateFakeMethod` with
`GuessFakeMethodAccessor`, `SpecializedMember.DeclaringType`, `TypeParameterSubstitution` and
`VarArgInstanceMethod`. A resolved member is its origin (definition, implicit struct
constructor or fake) plus the substitution, with the substituted signature computed once:
the plan's "interned `(definition, substitution)` pair" as a value, which is all the IL reader
needs. Signatures are matched after `NormalizeTypeVisitor` (type parameters become dummies of
their owner kind, unknown types forget reference-type knowledge); function pointers carry
their full identity (calling convention, custom conventions, ref-readonly return, parameter
ref kinds) because `FunctionPointerType.Equals` compares all of it.

Upstream behavior mirrored deliberately, because the oracle decides:

- `GetFilteredAccessors` yields an event's remover when its invoker matches the name, so a
  lookup can land on the remover, or fail with a null candidate when there is none.
- `VarArgInstanceMethod` drops the last parameter of the method it wraps even when that is not
  `__arglist` (a fake method has no `__arglist`), and fails on a method without parameters.
- A parent that is a generic type definition yields a specialization by its own type
  parameters, so the declaring type prints as `G`1[[`0]]` rather than `G`1`.

Not ported yet: constructors of `ComImport` interfaces, which the managed code takes from the
`CoClassAttribute` type; the engine reports `Unsupported` there. No corpus image reaches it;
item 4f (custom attribute values) provides what it needs.

Measured at item 4c: the same 3,442 images pass with one more line per MemberRef and per
MethodSpec row.

### 3.11 Type attribute semantics (item 4d)

`typesystem/attrs.rs` ports `ApplyAttributeTypeVisitor`. The design point that forced a
restructuring: the dynamic attribute's index counts custom modifiers, so modifiers must still
be present when the visitor runs. Decoding is therefore two-stage like the managed code:
`from_sig_raw` is `TypeProvider` (modifiers kept as `TypeData::Modified`), and the visitor
runs only where the managed code calls `ApplyAttributesToType`, with the same attribute
source, options and nullable context at each site: base types and interface implementations
(the type's or the InterfaceImpl's attributes, oblivious context), fields, method parameters
and return types (return parameter attributes, then a property's own attributes overriding
them), events, constraints, declaring types of member references (no dynamic, tuple, nullable
or native-int semantics on the type itself, but applied to its type arguments), fake members
and method specification arguments (no attributes). Member lookup still compares the raw
signature, normalized. The vararg types of a call site stay raw, modifiers included, as in
`VarArgInstanceMethod`.

Per entity, `OptionsForEntity` drops nullability below the module's `NullablePublicOnly`
threshold using effective accessibility (intersected through enclosing types); properties and
events use their declaring type, as the managed code does to avoid recursion. Nullable
contexts follow `NullableContextAttribute` from method to type to enclosing type to module.

The effective `TypeSystemOptions` come from the host: `DecompilerTypeSystem` removes
`NativeIntegersWithoutAttribute` below net7 after detecting the target framework, which is
host policy like the reference closure (section 3.8).

Two specialization details that only nullability makes visible: a generic method specialized
by its class alone gets fresh type parameters of its own, and substituting a `T!` by such a
parameter drops the `!` (only `T?` survives), because `TypeParameterSubstitution` always
replaces an annotated type parameter whose owner kind it covers.

The listing now prints types structurally (tuple element names, nullability marks, function
pointer calling convention, custom conventions, ref-readonly return and parameter ref kinds)
and runs twice. Measured at item 4d: `TypeSystemListingEquality` under `None` and under
`Default` both pass for the 3,442 images.

### 3.12 Custom attribute values (item 4f)

The step 3 decoder (`metadata::custom_attribute`) resolves argument types by name, which is
enough for metadata-level questions such as `RefSafetyRules(11)`. The decompiler's values come
from `CustomAttribute.DecodeValue(module.TypeProvider)`, where the provider decides what an
argument is: `System.Type` is recognized by its resolved definition, an enum argument's wire
format is the primitive behind the resolved enum (`GetUnderlyingEnumType`, which throws when
the enum cannot be resolved), and serialized type names go through
`ReflectionHelper.ParseReflectionName`. `typesystem/attrvalue.rs` is therefore a second, exact
port of SRM's `CustomAttributeDecoder` over `TypeId`s, including its quirks: `CLASS` and
`VALUETYPE` read as a type handle, a generic attribute's type parameters are read from the
parent `TypeSpec` (with `SkipType`'s recursive `CLASS` handling), tagged objects carry their
actual type, and a count of -1 is a null array. `typename.rs` ports `TypeName.TryParse` (the
SRM parser `ParseReflectionName` uses, node limit included) and `reflection.rs` the
resolution: assembly-qualified names look in that module only, others in compilation order,
with type forwarders followed and `` `n `` / ``` ``n ``` as dummy type parameters.

`CoClassAttribute` constructors close the gap left in 4c: `new IFoo()` on a ComImport
interface resolves to the constructors of the CoClass, re-declared by the interface, with a
busy guard for an interface that names itself (or a cycle) as its CoClass. Compilers emit
`newobj` on the CoClass directly, so the test builds its fixture with `MetadataBuilder`: a
constructor reference whose parent is the interface.

The listing gains one line per CustomAttribute row with the fixed and named arguments.
Measured at item 4f: `TypeSystemListingEquality` under `None` and `Default` pass for the 3,442
images, and the CoClass fixture equals the managed listing.

### 3.13 Ported unit suites and options per module (item 4g)

The managed `TypeSystemLoaderTests` and `ReflectionHelperTests` move to `cargo test`, so the
Rust type system has unit coverage that runs without .NET. They read the same fixtures: the
legacy reference assemblies from the `ILSpy-tests` submodule and the compiled
`ICSharpCode.Decompiler.Tests.dll`, located by `tests/common` (environment overrides, then the
default paths, failing with the path when a fixture is missing). `typeof(X)` becomes the
reflection name of `X`.

The loader fixture gives the test assembly `TypeSystemOptions.Default` and `mscorlib`
`Default | OnlyPublicAPI`, so options are per module now, as on `MetadataModule`: every read
names the module of the entity it serves. `OnlyPublicAPI` filters a type's fields, methods,
properties (by a visible accessor) and events (by a visible adder), which also changes what
member references resolve to. The FFI still applies one value to every module, as
`DecompilerTypeSystem` does. One read keeps the main module's options: whether a function
pointer type acts as `UIntPtr` depends on the module that created it, which the interned
type does not record.

The loader suite asserts much more than the listing covers: attribute lists with
pseudo-attributes, virtual/override modifiers and explicit implementations, member lookup
through base and parameterized types, and the specialization API with specialized type
parameters. It is therefore ported alongside those features (4h-4k), and each adds its facts
to the listing so the corpus checks them as well. At item 4g: 44 reflection tests and the
first 12 loader tests pass.

### 3.14 Member modifiers and inheritance (item 4h)

`typesystem/inheritance.rs` ports what the decompiler asks about a member's place in the type
hierarchy. Modifiers come from MethodAttributes as on `MetadataMethod` (an IL-sealed
interface implementation is not "sealed" in C# terms), and a property or event takes those
of its first accessor. Explicit implementations are the MethodImpl rows of the declaring
type, found by SRM's class-range search and resolved in the type's own generic context; a
property's or event's are the accessor owners of its accessor's. A property is an indexer
when it is the type's `DefaultMember` with parameters; an explicitly implemented one takes
the kind of the property it implements.

One rule reaches across the hierarchy: a property that overrides only one accessor copies
the accessibility of the nearest non-override base property, so `GetBaseMembers` comes with
it. It walks the non-interface base types derived-first (`BaseTypeCollector`'s post-order,
with its cycle guard), takes the same-named non-private members of each base seen through
its type arguments, and compares signatures as `SignatureComparer.Ordinal` does: symbol kind,
name, method type parameter count, and parameter types normalized with method type
parameters as dummies but class type parameters kept.

Attribute lists were planned first but depend on this work: a property's `IndexerName`
pseudo-attribute needs indexers and explicit implementations, so the order of 4h and 4i is
swapped. The listing now prints, per type, all base types, the non-interface ones and the
default member name, and per member the modifiers, explicit implementations and (for
properties) base members. Measured at item 4h: `TypeSystemListingEquality` under `None` and
`Default` pass for the 3,442 images; 45 loader tests pass in Rust.

### 3.15 Attribute lists (item 4i)

`typesystem/attributes.rs` returns what `GetAttributes()` returns on each entity, in the same
order: first the pseudo-attributes the managed type system synthesizes from metadata
(Serializable, ComImport and SpecialName from flags, StructLayout from layout flags and
ClassLayout, DllImport from ImplMap, PreserveSig and MethodImpl from implementation flags,
FieldOffset, NonSerialized, MarshalAs from the marshalling descriptor, Optional,
DefaultParameterValue, In and Out on parameters, IndexerName on renamed indexers,
AssemblyVersion and TypeForwardedTo on the assembly), then the custom attributes that the
type system options do not already consume (`IgnoreAttribute`: dynamic, tuple names,
nullability, `ref readonly`, `scoped` and so on), then the security declarations. Built-in
attributes find their constructor by exact parameter types, as `DefaultAttribute` does, and
name their arguments field or property by looking for a field on the attribute type.

Two details cross into the decompiler proper. Whether a parameter's default value becomes
`DefaultParameterValue` depends on the later parameters, skipping closure parameters of
local functions, so `IsClosureParameter` and `IsPotentialClosure` are ported here. And a
nested type's type parameters are its outer type's objects in the managed type system, so
their attributes come from the outer type's GenericParam rows, which the corpus found in
C# 14 extension blocks.

The listing gains an attribute-list line per type (with its type parameters), method (with
return, parameter and type parameter lists), field, property (with parameters) and event,
and the assembly and module lists. Measured at item 4i: `TypeSystemListingEquality` under
`None` and `Default` pass for the 3,442 images; 98 loader tests pass in Rust.

### 3.16 Member lookup (item 4j)

`typesystem/lookup.rs` ports `GetMembersHelper`. A lookup visits the type itself, or its
non-interface base types in base-first order (so `object`'s methods come first), and returns
each one's declared members; through a parameterized type, methods, constructors and
accessors are specialized by its arguments, and properties, fields and events are reported
with the parameterized type as their declaring type. Method lookups with type arguments keep
only methods with that many type parameters and specialize them by those as well. Nested
types found through a type take its arguments, or its own type parameters, followed by
explicit arguments or `UnboundTypeArgument`s (a new type variant). Two managed asymmetries are
kept: on `void` every member lookup is empty, but nested types still come from its base
types; and a definition's own nested types are definitions, not parameterized.

The listing gains a lookup line per type: inherited nested types, constructors, methods,
properties, fields, events and the delegate `Invoke`. Measured at item 4j:
`TypeSystemListingEquality` under `None` and `Default` pass for the 3,442 images; 132 loader
tests pass in Rust.

### 3.17 Specialization (item 4k)

`typesystem/specialize.rs` completes the member model. Specializing an already specialized
method composes the substitutions as `TypeParameterSubstitution.Compose` does (the existing
argument lists with the new substitution applied, a missing list taken from the new one) and
applies the result to the definition, so `def<B, A>.M<A>` specialized by `<int, string>` equals
`def<string, int>.M<int>`, and the identity substitution changes nothing. Methods compare equal
when definition and substitution do.

One managed behavior is deliberately not reproduced: a specialized method in the managed type
system owns fresh `SpecializedTypeParameter`s, so its parameters are not its definition's. In
Rust a specialized method has no identity to own anything, so its type parameters are the
definition's, with constraints seen through the class substitution. What the decompiler
observes of that difference, the dropped `T!` annotation, is already handled at the
substitution (section 3.11), and the ported tests check owner kinds where the managed ones
check object identity.

`IsReferenceType` of a type parameter now follows its constraints: value-type and class
constraints decide directly, otherwise the effective base class does (class constraints,
including other type parameters' effective base classes through nullability annotations, the
derived-most one winning). The listing prints it per type parameter, which found the
annotation case. Measured at item 4k: `TypeSystemListingEquality` under `None` and `Default`
pass for the 3,442 images; all 137 active loader tests and 44 reflection tests pass in Rust,
completing step 4.

### 3.18 The IL value model (item 5a)

Step 5 (the IL reader and the ILAst) is split like step 4. Its first piece is the vocabulary
the reader reasons in: `StackType` (whose declaration order decides which type wins when two
branches meet with different stack types), `PrimitiveType` (carrying SRM's primitive codes),
`Sign`, and `IType.GetStackType()` / `ToPrimitiveType()`. The latter two follow the managed
kind rules, including that a parameterized or annotated type takes its definition's kind: a
generic type whose definition is unresolved is `Unknown`, not a value type. Both are printed
per field and return type in the type listing, so the corpus checks them. `Interval`,
`LongInterval` and `LongSet` (switch labels, IL ranges) are ported with their managed unit
tests and keep the managed trick of a wrapping exclusive end to include the maximum value.

Only the opcodes the reader produces are modeled in step 5; each later step adds the opcodes
its transform creates, together with their materializer cases, so the Rust model never holds
an instruction kind that no gate exercises.

### 3.19 The ILAst arena (item 5b)

`il/ast.rs` holds one `Function` per method: a vector of nodes addressed by `NodeId`, each
with its instruction, parent, child index and IL range, and a vector of variables addressed
by `VarId`. Instructions are an enum whose variants name their children by `NodeId`; the
operations the reader and the block builder need (adding a node adopts its children,
`replace`, inserting into and truncating block, container and argument lists, a post-order
snapshot of descendants) keep parent links and child indices consistent.

Each kind's result type, flags and inferred type follow the generated `Instructions.cs` and
the hand-written partial classes, including the asymmetries the reader relies on: the end
point of an `if` is unreachable only if both arms' are; a container's only if no `leave`
targets it; a checked conversion may throw although that is not one of its direct flags.
Flags are computed on demand rather than cached, which is enough for the reader; caching with
invalidation arrives when a transform makes it worth having. Method, field and type operands
keep the metadata token they were read from next to their resolution, so the materializer
can resolve them again in the function's generic context and get the managed objects
directly. These are checked by Rust unit tests here and, from 5c on, against the managed
reader through the materializer.

### 3.20 The IL reader, wire v1 and the ILAst materializer (item 5c)

`il/reader.rs` ports `ILReader` almost line by line: blocks are imported from a queue, each
with an input stack, and re-imported when a merge widens a stack slot's type. Values wait
on an expression stack until a branch, a side effect or the end of a block flushes them into
stack slot variables. At the end, the slots that meet on an edge are unified, stack
adjustments are inserted where their types differ, and the representatives are named `S_n`
in visiting order. `il/block_builder.rs` ports `BlockBuilder` (try, catch, filter, finally
and fault containers, branch connection, VB "on error" dispatchers), and `finish` ports the
rest of `ReadIL`: the variable order and the topological sort that drops unreachable blocks.
Runtime async (`MethodImplAsync` under `TypeSystemOptions.RuntimeAsync`) is part of this
step and not of 5d, because the .NET 11 runtime libraries in the corpus are compiled with
it. It unpacks the return type with a port of `TaskType.UnpackAnyTask`.

The port keeps the managed quirks that the output shows, rather than fixing them:
- `BlockBuilder` never empties its container stack after placing the blocks, so branch
  lookup also searches the containers left over from that pass.
- `IsTask` does not recognize a `Task<T>` that is wrapped in a nullability annotation.
- `Warn` and the `InvalidBranch` messages reuse the managed text, because they are printed.

A condition where the managed reader throws, such as an index outside the branch-target
bitset, becomes an error for that method only. A panic is also caught per method, so one bad
body does not hide the others.

Wire v1 (`wire/`) begins with `ILSW` and a version, followed by a string table and a type
table. Types are structural trees: Def, Inst, Array, Pointer, ByRef, Pinned, TypeParam,
Unknown, the special types, FnPtr, Modified, Tuple and Annotated. Each entry refers only to
earlier ones, so the C# reader (`WireReader`) builds the link-compiled `IType`s in one pass.
Methods and fields travel as their metadata tokens, and the materializer resolves them again
in the function's generic context. A function carries its header, its variables in
`ILFunction.Variables` order, its warnings and sequence point candidates, a block table, and
then the body in pre-order. The block table exists because `Block.Kind` is readonly: the
materializer creates every block up front, so branches can point at blocks that come later.
The table also lists blocks that are only reachable as branch targets: the topological sort
can drop an outer block that only an inner container branches to, and the managed printout
still names it. `Interop/IlAstMaterializer.cs` rebuilds each node through the same
constructors the reader uses, adds the variables, and connects the function with `AddRef`,
which also produces the load, store and incoming-edge counts.

The gate, `IlAstReadEquality`, prints both functions with `WriteTo` (IL ranges on), plus the
local signature length and the sequence point candidates, and compares the printouts for
every method body of every corpus image. A method that fails must fail on both sides.
`PrintoutCoversReaderOutput` checks that the printout really contains stack slots, exception
slots, try blocks, handlers, incoming counts and sequence points. The first corpus run also
caught a performance bug: appending to a block relinked all its children, which made reading
quadratic.

### 3.21 Debug symbols (item 5d)

The engine still has no PDB reader. The IL reader asks a `DebugInfo` trait
(`il/debug_info.rs`) for the two things `ILReader` takes from `IDebugInfoProvider`: a
local's name (`TryGetName`) and its `PdbExtraTypeInfo` (dynamic flags and tuple element
names, `TryGetExtraTypeInfo`). The other provider members (sequence points, variables,
source file name) serve the disassembler and the PDB writer, which stay managed. As built,
the callback is `DebugInfoCallback(ctx, method_token, kind, index, buf, cap) -> len` and
carries no module id: a decompiler has one provider, for its main module. The host writes
into an engine-owned buffer and returns the full length (-1 for no answer); a longer answer
makes the engine grow the buffer and ask again. Nothing has to stay pinned across the call,
and a provider exception is captured in `Interop/RustDebugInfo.cs` and rethrown after the
native call returns, because it cannot unwind through Rust frames. Extra type info travels
as little-endian counts (-1 for null) followed by flag bytes and UTF-8 names.

`IlAstReadEqualityWithDebugSymbols` runs over every corpus image with a PDB next to it or
embedded, plus the shim test assembly and ICSharpCode.ILSpyX. Their PDBs are built with them
and cover real-world locals. The provider is ILSpyX's `DebugInfoUtils.LoadSymbols`, the one
the UI uses, so the Mono.Cecil path for Windows PDBs (the net40 VariableNaming fixtures) is
covered too. The shim test project references ILSpyX for this. Measured at this step: 59
images with symbols, all equal; the whole shim suite has 13,853 tests and takes about 3.6
minutes.

A managed quirk this step keeps: PDB tuple element names never reach a local, because
`DecodeLocalSignature` already turns a `ValueTuple` into a `TupleType` and
`ApplyAttributeTypeVisitor` has no `VisitTupleType`, so it only visits the elements. The
`(ILFunction? Function, string? Error)` local in the test assembly prints without names on
both sides. `WriteTo` does not print tuple names, so the gate's printout now adds one line per
variable with its full type. That makes tuple names and `dynamic` part of the comparison in
both `IlAstReadEquality` variants.

### 3.22 Seam 1 at N = 0 (item 6)

**The seam blocks.** The drift test only allows code to be added inside `RUST-SEAM` blocks,
never to replace a line, and `dotnet format` re-indents a statement placed under a brace-less
`if` or `else`. So the original `function.RunTransforms(SelectILTransforms(...), context)`
line cannot be made conditional. Seam 1 therefore has three blocks:
- a field `rustSuspendedILTransforms`;
- the handover before that line;
- a restore after it.

When the engine serves a method, the handover does four things:
- replaces `function` with the materialized engine function;
- moves the declaration's `ILFunction` and parameter annotations to it;
- rebuilds the context with `new ILTransformContext(context, function)`;
- runs the remaining managed transforms.

It then swaps the decompiler's `ilTransforms` for an empty list, so the original line only
checks invariants, and the restore puts the list back. If a member throws between the swap and
the restore, the next member's handover restores the list first. The drift test now checks the
four blocks in file order (1, 2, 1, 1).

**Where a handover may happen.** An IL prefix counts the pipeline's leaves:
- a `BlockILTransform` counts its pre-order and post-order transforms;
- a `StatementTransform` counts its children;
- every other transform counts as one.

The standard pipeline has 59 leaves: the 58 items 7 to 64 minus the FORK item 32, plus two,
because item 56 covers three transforms. A block group interleaves its leaves per block, so the
engine can only take over at a boundary between top-level transforms (`IlTransformPrefix.
TopLevelBoundary`). The `IlAstPrefixEquality` harness instead truncates groups, so every leaf
has its own index for comparison.

**When the seam falls back.** It serves only when the leading transforms of the decompiler's
list have the leaf types of `CSharpDecompiler.GetILTransforms()`. It compares type names, not
transform options. With `RecordSteps` set (the Debug Steps pane) it asks for prefix 0, so every
step stays visible. A type system whose main module has no in-memory PE image falls back, and
each fallback is counted. Strict mode (`ILSPY_RUST_STRICT=1`) turns a fallback into a
`RustEngineException`. An engine error or panic surfaces the same way, through the existing
per-member error comment.

**The session.** `RustEngine` keeps one engine session per type system in a
`ConditionalWeakTable`. `ilspy_session_load` builds the compilation once. The engine copies
everything except the images, which the `PEFile`s keep alive. The session can serve threads in
parallel because `Compilation` is `Sync`, and each call gets its own `TypeSystem`. The ratchet
run found that `CSharpDecompiler(string fileName)` loads references with
`PEStreamOptions.PrefetchMetadata`, which leaves no whole image in memory. Such a reference is
now passed as its bare metadata block, and the engine recognizes it by the `BSJB` signature.
The main module still needs its whole image, for the method bodies.

**Known costs at N = 0.** The managed reader still runs before the seam, because the handover
can only come after it. Each method is therefore read twice, and a method the managed reader
rejects never reaches the engine.

**The ratchet.** `rust/port-status.json` is `il_prefix` 0, `ast_prefix` 0 and
`methods_served_pct` 100.0. That share is measured by decompiling the shim test assembly and
ICSharpCode.ILSpyX. The committed values must equal the measured ones, so a drop fails, and so
does a rise that has not been committed. `rust/ilast-divergence-allowlist.json` starts empty.

**Solution test runs.** In `dotnet test --solution`, ICSharpCode.Decompiler.Tests recompiles the
fixtures while the shim suite reads them. The shim tests used to memory-map the fixtures and
never release them, which made `csc` fail. They now read each fixture into memory, retrying
while the compiler holds the file. One UI test,
`SwitchListAndDeleteAllTests.Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab`, hangs on
this machine on `master` as well, in Avalonia font fallback. The gate run excludes it.

### 3.23 Use counts and ControlFlowSimplification (item 7, IL prefix 1)

**Use counts.** The first transform needs what the managed arena maintains implicitly:
`LoadCount`, `StoreCount` and `AddressCount` per variable, `IncomingEdgeCount` per block and
`LeaveCount` per container. `il/refs.rs` ports the managed reference counting. Each node has a
`ref_count`, the reader connects the body where `ReadIL` calls `function.AddRef()`, and every
editing operation adjusts the counts:
- `replace`, `insert`, `truncate` and `set_list` (with `remove_at`, `retain`, `remove` and
  `remove_block` built on them) connect what they add before they disconnect what they drop.
  That way `stloc.ReplaceWith(stloc.Value)` keeps the value counted.
- Changing an operand goes through `set_branch_target` and `set_variable`.
- A container's entry point carries the implicit incoming edge, which moves when its block
  list changes.

`leave_count` is now the counter rather than a tree scan. A container outside the function
tree therefore counts no leaves, exactly as in the managed code. The reader equality still
holds with this change.

**The managed details the port keeps, because the output shows them.**
- `Block.Remove()` is `SwapRemoveAt`: the container's last block takes the place of a return
  block moved into a try container. A plain `remove` changed the block order in 83 images.
- The managed transforms iterate `Descendants` lazily while they edit the tree. The port takes
  a post-order snapshot and skips nodes that the edits disconnected, which visits the same
  nodes for the edits this transform makes. Later transforms that insert nodes ahead of the
  cursor need a closer look.
- `RemoveAt(i++)` visits the same index again after a removal.
- The ordering in `SortSwitchSections` is stable, and sections that neither branch nor leave
  sort first.

**Settings.** The engine names the `DecompilerSettings` it reads
(`il::transforms::SETTING_NAMES`, exported as `ilspy_setting_names`). The shim looks each name
up by reflection, fails on a name without a boolean property, and sends the values as a bit
set (`Interop/RustSettings.cs`). This version of the "reject unknown names" rule is checked
when the code is written: an engine setting the shim cannot map is an exception. Item 7 needs
`RemoveDeadStores` and `SortSwitchSections`. `IlAstPrefixEqualityWithFlippedSettings` runs the
gate with every engine setting flipped, over the `Switch*` fixtures and ICSharpCode.Decompiler.

**The pipeline.** `il::transforms::PIPELINE` lists the ported leaves in run order, and a unit
test pins its length to `IL_PREFIX`. `wire::ilast::read_function` runs the prefix the host
asks for.

### 3.24 Data flow analysis and SplitVariables (item 8, IL prefix 2)

`flow_analysis/data_flow.rs` ports `DataFlowVisitor`:
- a generic forward analysis with a worklist per container, in block order;
- branches and leaves that run `finally` blocks;
- the exception state of each try block;
- the condition evaluation of `if`.

It is generic over a `State` trait (join semi-lattice with bottom) and an `Analysis` trait
(what loads and stores do). The managed states share objects by reference in a few places. The
port clones where the managed code clones and moves where it only renames a reference, so each
join sees the same contents. An instruction with control-flow flags and no case of its own
panics, like the managed `NotImplementedException`; the per-method error path catches it.
`flow_analysis/reaching_definitions.rs` ports `ReachingDefinitionsVisitor`: a bit set over all
stores of the analyzed variables, in tree order, plus an uninitialized pseudo store per
variable.

SplitVariables needs the function's variable list (`ILFunction.Variables`): it adds the split
variables and then drops the dead ones. So `PIPELINE` entries now take the whole
`ReadFunction`. `ILVariableCollection.RemoveAt` also moves the last variable into the gap,
like `Block.Remove`, and the port does the same.

The first G-ilast(2) run differed in 25 images, all on ref struct locals:
- `ByReferenceType.IsByRefLike` is true in the managed type system, so a call returning
  `ref T` keeps its receiver's local whole. The Rust `is_by_ref_like` now says so as well.
- `accessor_owner` returns the owner's metadata token, not its row. The `Span<T>` indexer
  exemption therefore needs the table check and the row mask.

### 3.25 ILInlining (item 9, IL prefix 3)

`il/transforms/il_inlining.rs` ports the part of `ILInlining` that the top-level pipeline
step reaches, which runs with `InliningOptions.None`:
- `InlineAllInBlock`, `InlineOne` and `FindLoadInNext`;
- the slot rules `CanInlineIntoSlot` (with the overrides of `Block`, `BlockContainer`,
  `Comp` and `IsInst`) and `SatisfiesSlotRestrictionForInlining` (`StObj`);
- `NonAggressiveInlineInto`, and `SemanticHelper.MayReorder` in `il/semantic_helper.rs`.

Inlining an `ldloca` (`IsGeneratedTemporaryForAddressOf`) is only reachable with
`AllowInliningOfLdloca`. The statement transform sets that option, so the port returns
"cannot inline" without it and panics with it, which marks the gap. Named arguments and
deconstruction need options that this step never sets.

Three fixes came out of the gate:
- `SwitchInstruction.Type` is printed, and non-aggressive inlining into a switch value sets
  it. The Rust `Switch` now carries `ty`, and wire v1 sends it as a type index + 1 (0 for
  none) before the value.
- The managed `CallIndirect` has the function pointer as child 0, before the arguments. The
  Rust arena had it last, which changed the inlining search order and the slot check.
  `children` and `set_child` now follow the managed order. The wire format was unaffected
  because it encodes the operands explicitly.
- `IsInst.CanInlineIntoSlot` also accepts a parent `unbox.any` (`as T?`), a parent
  comparison with null (`is T`) and a parent control-flow block. The first port stopped
  reading the override before those cases.

### 3.26 Variable instruction lists and InlineReturnTransform (item 10, IL prefix 4)

`InlineReturnTransform` walks `returnVar.StoreInstructions` and appends a moved or copied
return block per store, so the order of that list reaches the output. The arena now keeps
the managed lists, `LoadInstructions`, `StoreInstructions` and `AddressInstructions`, in
place of the counters. The order follows the managed rules:
- `Connected` runs the base (the children) before the node's own registration, so a subtree
  connects in post-order.
- `Disconnected` unregisters the node before its children.
- A removal swaps the last entry into the gap, tracked per node in `var_list_index`.

The collection operations keep their managed sequencing:
- `ReplaceList` adds each new item before it releases the old item at the same position.
- `RemoveAll`, `RemoveAt`, `Clear` and `SwapRemoveAt` release only what they remove.
- `CombineBlockWithNextBlock` adds the moved instructions before it clears the old block
  (`AddRange` and then `Clear`).

A unit test pins post-order connection and swap-removal. The transforms ported so far did not
depend on these orders; the reader and prefix equalities still hold with them.

### 3.27 RemoveInfeasiblePathTransform (item 11, IL prefix 5)

The transform sets the internal `ILVariable.RemoveIfRedundant`, which SplitVariables reads to
split such stack slots like locals. The flag is not printed, but managed transforms after the
handover read it. So it travels as variable flag bit 16 in wire v1, and the gate's printout
marks it on each variable line. `BlockContainer.SortBlocks(deleteUnreachableBlocks: true)`
reuses the reader's topological sort.

### 3.28 DetectPinnedRegions (item 12, IL prefix 6)

**New instructions and fields.**
- The Rust ILAst gains `PinnedRegion` (a store instruction: use lists, data flow and
  reaching definitions treat it like `stloc`, around its body) and `GetPinnableReference`
  (with an optional method).
- `If` gets its `CSharpType`. That drives its result type, as in the managed code, where a
  typed `if` is an expression.
- `ReadFunction` counts `RegisterVariable`'s `I_n` names. The private
  `ILFunction.helperVariableCount` travels in the function header, and the materializer sets
  it by reflection, so managed transforms after the handover continue the numbering.

**Wire v1 additions.** Opcodes 62 and 63, the `if` type, and the helper counter.

**New engine setting.** `PatternBasedFixedStatement`. The flipped-settings gate now also
covers the `UnsafeCode*` fixtures.

**Managed semantics the gate exposed in shared code.**
- `ByReferenceType` and `PointerType` answer null to `IsReferenceType`, not false. The pinned
  `char&` locals of the runtime were skipped as value types.
- `InstructionCollection.RemoveAll` releases each rejected item before it tests the next one.
  So a block reachable only from a block removed earlier in the same pass has lost that edge
  by the time it is tested, and goes too. `Function::retain` now does the same.
- `MatchLdcI` looks through sign extensions, and through zero extensions of an `I4`.
  `SwitchDetection.AdjustLabels` uses the same matcher.

Instruction equality (`ILInstruction.Match(...).Success` without pattern nodes) is
`Function::structurally_equal`. It compares the kind and the operands, then the children
recursively.

### 3.29 YieldReturnDecompiler (item 13, IL prefix 7)

**Several functions, several arenas.** The managed transform reads the enumerator's
constructor, `get_Current`, `GetEnumerator`, `Dispose`, `MoveNext` and the finally methods
into separate `ILFunction`s, then moves instructions and variables between them. In Rust
each of these reads gets an arena of its own: `read_il_in_context` with the state-machine
generic context (lecture 3.28's `class_tail`), followed by `run_early_il_transforms`.
`MoveNext()` is converted inside its own arena, and its old tree is released there. Then
`Function::import` appends the arena to the iterator's arena by shifting node and variable
ids. Finally bodies are imported when a try-finally takes them.

The reference counts match the managed ones:
- An instruction that moves into the new body is still listed by its old block. Releasing
  the old root disconnects it.
- Setting the new body connects it again, in post-order, so the variable lists come out in
  the managed order.
- Mono's inlined finally functions are imported without a release, as the managed code
  never releases them.

**Supporting ports.**
- `SymbolicExecution`, `StateRangeAnalysis` (all four modes), `LongDict` and
  `SwitchAnalysis.MakeSetWhereComparisonIsTrue`.
- `CopyPropagation.Propagate`, which the transform calls on its own; the pipeline step comes
  later.
- `OrderedMap`, which mirrors `Dictionary` enumeration order (insertion order, removals keep
  the rest in order).
- A raw-IL check for `TransformDisplayClassUsage.ValidateConstructor`. It tells Mono's
  parameterless enumerator constructor from legacy Visual Basic's. Only the parameterless
  form is reachable from a `newobj` without arguments.

**Wire v1 additions.**
- `YieldReturn` (opcode 64).
- The iterator flags byte and the `MoveNext` method in the header.
- Each variable's `StateMachineField`.
- Member tokens carry a state-machine-context bit, so the host resolves them in the
  concatenated generic context.

**New engine setting.** `YieldReturn`.

**Managed semantics the gate exposed in shared code.**
- `ILReader` with an explicit generic context specializes the method for it. `this` of
  `MoveNext()` is `d__N<``0>`, not the class's own `d__N<`0>`. `Reader::new` now calls
  `specialize` the same way.
- ILInlining always inlines into `yield return`, and always inlines a load of a variable
  that has a state machine field into a variable that has none.

**Seam 1 annotations.** After this step an iterator function also holds `MoveNext()`'s
`this` parameter, as in the managed pipeline. The managed code annotates the declaration's
parameters before any transform runs, so it never meets the duplicate index -1. The seam
annotates after the prefix, so `RustSeam1` keeps the first parameter per index (the reader's
own come first) instead of calling `AddAnnotationsToDeclaration`, which maps by index and
throws on the duplicate.

**Debug-only differences.** In DEBUG builds the managed code inserts commented `nop`s (state
ranges, `state == N`). The early transforms at the end of the step delete them, so the port
leaves them out.

### 3.30 AsyncAwaitDecompiler (item 14, IL prefix 8)

**One step, many managed classes.** The transform calls into code that later pipeline items
own. Those pieces are ported here, and their own items then add only the pipeline step:
- `RemoveDeadVariableInit` (item 20), with `DefiniteAssignmentVisitor` for
  `ResetUsesInitialValueFlag`.
- `EarlyExpressionTransforms.StObjToStLoc` (item 18) and
  `TypeUtils.IsCompatibleTypeForMemoryAccess`.
- `DynamicCallSiteTransform.RunOnBasicBlock` (item 22), with
  `TransformArrayInitializers.HandleSimpleArrayInitializer` (one dimension) and
  `TransformExpressionTrees.MatchGetTypeFromHandle`.

New infrastructure:
- `ControlFlowGraph` with dominance (Cooper, Harvey and Kennedy) and the reachable-exit sets,
  which `AwaitInCatchTransform`, `AwaitInFinallyTransform` and later `LoopDetection` use.
- The runtime-async path: `TranslateThisCopyAccess`, `RuntimeAsyncManualAwaitTransform` and
  `RuntimeAsyncExceptionRewriteTransform`. The runtime images of .NET 11 compile many methods
  as runtime-async, so this path is not optional.
- `TaskType.IsNonGenericTaskType` / `IsGenericTaskType`, which return the builder's name.

**The `MoveNext()` arena.** As for iterators, `MoveNext()` is read and analyzed in an arena of
its own and then imported. The main try block becomes the function body while it is still
connected in the imported tree. Releasing that tree's root afterwards leaves the try block
connected, so the variable lists keep the order `MoveNext()` gave them, as in the managed
code. `set_body` also clears the body's parent link, because the function is not a node.

**New instructions.**
- `Await`. Its `GetAwaiter` and `GetResult` methods are either methods or the `dynamic` fake
  methods of a dynamic await, which the materializer builds by reflection (`FakeMethod` is
  internal).
- The eleven `DynamicInstruction` kinds, as one `Inst::Dynamic` with a kind. The operands
  differ only in which of the name, operation, type and argument infos a kind uses.

**Wire v1 additions.**
- Opcode 65 `Await`, and opcode 66 `Dynamic`.
- The `AsyncDebugInfo` in the function header.
- Opcode 255, a reference to a node already written. The managed runtime-async try-finally
  rewrite leaves one `leave` instruction in two blocks. Sent twice, the instruction would
  become two instances in the materializer, and the returned variable's load count would be
  one higher than the managed one. With the reference, both trees share it.

**New engine settings.** `AsyncAwait`, `AsyncEnumerator`, `AwaitInCatchFinally`, `Dynamic`.

**Managed semantics the gate exposed in shared code.**
- ILInlining always inlines an `await`.
- `SplitVariables` counts `await(ldloca V)` as an immediate address use.

**Changes to the gate harness.**
- The managed reference runs the transforms one by one instead of calling `RunTransforms`,
  so it gets what a Release pipeline produces. The Debug-only invariant checks reject the
  shared instruction above and throw out of the reference.
- An assertion listener reports failed `Debug.Assert`s and continues. The default listener
  terminates the test host.

### 3.31 DetectCatchWhenConditionBlocks (item 15, IL prefix 9)

**What it does.** A catch filter that tests the exception type becomes
`catch (T ex) when (...)`:
- The handler variable takes the tested type. As in `ILVariable.Type`, the stack type cannot
  change, and both matchers check it first.
- The type test leaves the filter.
- Copies of the exception variable inside the handler (`stloc b(ldloc ex)` and
  `stloc b(castclass T(ldloc ex))`) are replaced by the variable itself.

**Supporting ports.**
- The Visual Basic `On Error` filter shape.
- `EarlyExpressionTransforms.FixComparisonKindLdNull`.
- `MatchCompUnsignedZero` and `Block.GetContainingStatement`.

The transform needed nothing new from the arena, the wire or the materializer, and the gate
passed on the first run.

### 3.32 DetectExitPoints (item 16, IL prefix 10)

**What it does.** A jump out of a container to the instruction after the container becomes a
`leave` of the container. When the container ends its block, the exit is chosen from the jumps
out of it, the first one that does not leave the function, and is appended after the
container.

**Port notes.**
- The managed `ILVisitor` walks blocks by index while the parent block can grow, because a
  chosen exit is appended there and is then visited too. The port walks the live lists the
  same way.
- The managed sentinels `ExitNotYetDetermined` and `NoExit` are an `Exit` enum.
- An exit that an inner container already replaced reaches the outer containers as a stale
  node. The managed code would then replace whatever now sits in its old slot. The port skips
  it; this case did not occur in the corpus.

The no-op red run failed 1,069 of 3,550 equality cases; the port passed all of them on its
first run.

### 3.33 LdLocaDupInitObjTransform (item 17, IL prefix 11)

Roslyn's `ldloca; dup; initobj` (`stloc s(ldloca v); stobj T(ldloc s, default.value T)`)
becomes legacy csc's order (`stloc v(default.value T); stloc s(ldloca v)`), so later uses of
`s` can inline. The managed code assigns two list slots through the indexer, and each
assignment connects the new item before it releases the old one. `Function::replace` has the
same order, so the moved `stloc` disconnects and reconnects as in the managed code.

### 3.34 EarlyExpressionTransforms (item 18, IL prefix 12)

**What it does.** A post-order visitor (pre-order for `newobj`):
- Ordered comparisons with null become (in)equalities, and `box T(x) ==/!= null` of a type
  parameter compares `x`.
- `stobj` and `ldobj` through `ldloca v` become `stloc v` and `ldloc v`.
- `ldobj(ldflda*(addressof(ldloc v)))` reads through `ldloca v`.
- `AsyncHelpers.Await(value)` in a runtime-async method becomes `await`.
- `new decimal(...)` with constant arguments becomes `ldc.decimal`.

`StObjToStLoc` was already ported for item 14, into this module; the visitor now calls it
too.

**New instruction.** `LdcDecimal`, carried as the four words of `decimal.GetBits` (wire opcode
67). The materializer rebuilds the value with `new decimal(int[])`, so the scale and sign bits
survive as written in the IL.

The no-op red run failed 2,082 of 3,550 equality cases, the most of any step so far, because
nearly every method has some `ldloca` access. The port passed on its first run.

### 3.35 SplitVariables again (item 19, IL prefix 13)

A ratchet-only step: the second `SplitVariables` in the pipeline is the ported transform. It
surfaced one change to shared code.

**Removed nodes keep a stale parent.** In the managed tree, `ReplaceWith` and the removals of
`InstructionCollection` leave the removed instruction's `Parent` as it was. Until now the arena
cleared it.

On one runtime-async method of `System.Net.Quic`, a connected branch targets a block that the
runtime-async rewrite had removed from its container. The managed data flow visitor still
reaches that block's stale container through `Parent`, and only `Debug.Fail`s when the
container has no work list. The port panicked instead.

The arena now keeps the stale parent, as the managed one does:
- The places that used a cleared parent to mean "detached" test connectivity or list
  membership instead.
- The data flow visitor treats a missing work list as the managed code does.

### 3.36 RemoveDeadVariableInit (item 20, IL prefix 14)

The transform was ported with item 14, which calls it from the async/await cleanup. This
step adds it to the pipeline:
- It resets `UsesInitialValue` from the definite assignment analysis.
- It removes stores to variables that are never read, keeping a side-effecting value. Dead
  stores are removed in iterators and async methods, or when `RemoveDeadStores` is set.

The no-op red run failed 853 of 3,550 equality cases; the pipeline step passed on its first
run.

### 3.37 ControlFlowSimplification again (item 21, IL prefix 15)

A ratchet-only step: the second `ControlFlowSimplification`, without aggressive duplication
of return blocks, is the ported transform. It cleans up after `SplitVariables` and
`RemoveDeadVariableInit` (for example, branch-to-leave chains they make possible). The no-op
red run failed 1,597 of 3,550 equality cases; the step passed on its first run.

### 3.38 DynamicCallSiteTransform (item 22, IL prefix 16)

The call-site matching, the dynamic instructions and their wire form came with item 14, whose
dynamic awaits need `RunOnBasicBlock`. This step adds `Run` for the whole function:
- It collects the call sites initialized in any block.
- It transforms their invokes anywhere in the function body.
- It sorts the containers it changed, so the unreachable init blocks go.

Two shortcuts carry over, each noted in the code:
- A second initialization of the same cache field throws in the managed `Dictionary.Add`;
  here the first one wins.
- A `CSharpArgumentInfo` without a matching delegate parameter makes the call site not match.

The no-op red run failed 149 of 3,550 equality cases; the step passed on its first run.

### 3.39 SwitchDetection (item 23, IL prefix 17)

The C# compiler emits a switch with non-contiguous labels as a tree of comparisons, sometimes
around IL switches. `SwitchAnalysis` walks that tree from the end of each block:
- It tracks the set of values the tested variable can still have on each edge.
- Every leaf becomes a section, and sections that branch to the same target merge.

`SwitchDetection` decides whether a switch reads better than the if statements. The heuristics
need the container's control flow graph and the loops around the block (`LoopContext`, so that
continue targets are not cases). The `Visited` flags of the managed graph nodes are local bit
vectors here, and the recursive searches use explicit stacks.

Three additions outside the transform:
- **Two new settings,** `SparseIntegerSwitch` and `RemoveDeadCode`, become bits 8 and 9 of the
  settings word.
- **Two switch-section flags,** `HasNullLabel` and `IsCompilerGeneratedDefaultSection`, travel
  as one flags byte in the wire.
- **Constructors without a token.** The switch-expression default case becomes
  `throw newobj SwitchExpressionException(v)`, and that constructor is found by type lookup,
  so no metadata token names it. The wire sends token 0 followed by the declaring type and the
  parameter types, and the host looks the constructor up again.

The no-op red run failed 531 of 3,550 equality cases; the step passed on its first run.

### 3.40 SwitchOnStringTransform (item 24, IL prefix 18)

Every C# compiler generation lowers `switch (string)` differently, and the transform
recognizes six shapes:
- cascading `op_Equality` ifs (also VB's `CompareString` and span `SequenceEqual`);
- the C# 1 `string.IsInterned` chain;
- the legacy `Hashtable` lookup, whose initializers at the method start are removed once
  every switch that uses them is transformed;
- the legacy `Dictionary<string, int>` lookup;
- Roslyn's switch over `ComputeStringHash`;
- Roslyn's switch on the length, then on one distinguishing char.

Each becomes a switch over a new instruction, `string.to.int`, which maps the string values to
section labels. The instruction joined the arena and the wire (opcode 68). Its keys travel as
UTF-16 code units, like `ldstr`, so lone surrogates survive. Two more settings,
`SwitchStatementOnString` and `SwitchOnReadOnlySpanChar`, become bits 10 and 11.

The managed code mutates several things through shared references, and the port keeps each
of them visible:
- the hashtable label lists that `AddNullSection` extends;
- the sections of an old switch that the new one adopts;
- the default branch whose target moves past a null check.

The local functions of the length-and-char matcher share their captured state through a small
struct.

Index decrements use wrapping arithmetic. The managed loop index can go to -1 just before the
loop ends, and wrapping reproduces that end of the loop.

The no-op red run failed 185 of 3,550 equality cases; the step passed on its first run.

### 3.41 SwitchOnNullableTransform (item 25, IL prefix 19)

A switch on a nullable value compiles to a `HasValue` check that branches to the null case,
and a switch on `GetValueOrDefault()`. The transform folds both into one lifted switch: the
old switch's sections plus a section with `HasNullLabel`. It matches two shapes:
- the legacy one, with temporaries for the address and the value;
- Roslyn's, where the `GetValueOrDefault` call may be inlined into the switch.

Switch instructions gained their `IsLifted` flag in the arena and the wire, and
`LiftNullables` became bit 12 of the settings word. The section flags came over the wire
in item 23 already, in anticipation of this step.

The no-op red run failed 68 of 3,550 equality cases; the step passed on its first run.

### 3.42 SplitVariables, third occurrence (item 26, IL prefix 20)

A ratchet-only step: the third `SplitVariables` is the ported transform. The switch transforms
just before it remove stores and copies, which leaves more variables whose uses fall apart
into independent groups. The no-op red run failed 1,153 of 3,550 equality cases; the step
passed on its first run.

### 3.43 IntroduceRefReadOnlyModifierOnLocals (item 27, IL prefix 21)

A by-ref local becomes `ref readonly` in either of two cases:
- C# allows one of its stores only into a readonly reference.
- One of its stores reaches, through field addresses, a `readonly.ldelema`, whose semantics
  a plain `ref` would change.

The test is `ILInlining.IsReadonlyReference`, which the port adds next to the inlining code,
where later inlining steps will need it as well. It needs three attributes the type system did
not expose yet:
- `initonly` on fields;
- `[IsReadOnly]` on `ref readonly` fields;
- `return_read_only` on methods.

The flags already decode them, so each is a small accessor. Variables already carried
`IsRefReadOnly` in the wire.

The no-op red run failed 69 of 3,550 equality cases; the step passed on its first run.

### 3.44 BlockILTransform and LoopDetection (item 28, IL prefix 22)

`BlockILTransform` is the first nested group of the pipeline. For each container, it does
three things:
- builds the control flow graph once, before any transform runs;
- walks the dominator tree in post-order;
- calls the per-block transforms on each block.

The Rust driver (`il/transforms/block_transform.rs`) takes the transforms as function pointers
and carries the graph's `Visited` flags as a vector in its context, because LoopDetection
keeps state there across blocks. The second `BlockILTransform` (item 31 on) reuses the driver.

**LoopDetection** works in three steps:
- A block with a back edge from a block it dominates heads a loop: the natural loop plus the
  blocks dominated by the head up to a single exit point.
- The exit point is found on the post-dominator tree of a reversed graph. When that fails,
  the loop extends heuristically to reduce the number of exits.
- The loop's blocks move into a new loop container. The body of a switch moves into a switch
  container the same way.

The managed recursions over the graph (dominator-tree walks, predecessor searches) became
explicit stacks in the same visiting order, so deep graphs cannot overflow the native stack.

**The equality reference.** The test's managed reference stands for a Release pipeline, but
`BlockILTransform` runs `Block.CheckInvariant` (Debug only) before each block. On two methods,
that check threw on a tree the switch transforms leave with an instruction shared by two
blocks, so the reference reported an error where the engine (correctly) did not. The
reference now drives the block transforms itself without those checks, as it already did for
the top-level pipeline, and the allowlist stays empty.

The no-op red run failed 1,670 of 3,550 equality cases; the port passed all but those two on
its first run.

### 3.45 DetectExitPoints, second occurrence (item 29, IL prefix 23)

A ratchet-only step: the second `DetectExitPoints` is the ported transform. The loop and
switch containers that LoopDetection creates end in a block, so jumps out of them to the
following instruction become `leave` instructions of the new container. The no-op red run
failed 1,487 of 3,550 equality cases; the step passed on its first run.

### 3.46 PatternMatchingTransform (item 30, IL prefix 24)

An `isinst` test whose result is stored in a variable becomes a type pattern,
`match.type[T].notnull(v = x)`, when the variable is used only where the test succeeded. The
"only where" check is dominance in the container's graph. Value types follow the same idea
with their `isinst` and `unbox.any` pair. With recursive patterns on, property and field
reads of the matched variable then fold into sub-patterns:
- var patterns, null checks and constant comparisons;
- `HasValue` checks of nullable properties, which become null patterns or C#-lifted
  comparisons.

Two pieces joined the arena and the wire:
- the `match` instruction, a store to its variable, so the use counts treat it like
  `stloc`;
- the lifting kind of `comp`, so `match_logic_not` now requires an unlifted comparison, as
  in the managed matcher.

Four settings became bits 13 to 16.

**One divergence on the first run.** Two methods got a property var pattern in Rust but not
in managed. The check compares the variable's type with the property's `ReturnType`, which
carries the property's nullability annotations; the port first used the getter's return
type, which does not. The type system now answers the property type itself
(`getter_property_type`), substituted by the getter's class arguments.

The no-op red run failed 381 of 3,550 equality cases.

### 3.47 ConditionDetection (item 31, IL prefix 25)

The second `BlockILTransform` reuses the driver from item 28; at this prefix it runs
ConditionDetection alone. Blocks reached only through one branch move into the if (or after
the block whose unconditional exit they are). Then the ifs get:
- the exits that read best: break, continue and return keywords against branches and
  leaves;
- merged common exits;
- swapped empty then-branches;
- `&&` short-circuits;
- the branch order of the IL.

**The step was larger than its size.** `InvertIf` runs `ExpressionTransforms` on the inverted
statement, and almost every remaining difference after the first port came from that. The
statement visitor skips control-flow blocks and container bodies, so only the condition and
the non-block branches are visited, but that is still most of ExpressionTransforms. The port
(`il/transforms/expression_transforms.rs`) covers:
- negations pushed into comparisons and logic and/or;
- `comp(x != 0)` of a bool;
- the unsigned and `ldlen` comparisons;
- conversion and box clean-ups, array index clean-ups;
- `IndexRangeTransform.HandleLdElema` (`a[^1]`);
- decimal field constants;
- `ldobj.if.ref`;
- the canonical logic and/or and the conditional operator;
- the `match ? true : false` collapse;
- shift masks;
- `DetectImprovedTypeForForBitOp`;
- the catch variable and catch-when rewrites (a try-catch can be the exit an inverted if
  takes);
- switch expressions.

The arena and the wire gained the pieces these need:
- the result type of a switch expression;
- `WithSystemIndex` on `ldelema`;
- `CSharpResultType` on binary instructions;
- the exception specifier IL range of catch handlers.

Three settings became bits 17 to 19.

**Allowlisted.** Three groups of ExpressionTransforms calls stay unported. Each is a transform
with its own step later in the plan:
- NullableLiftingTransform: 62 methods;
- the array and span initializer rewrites: 12 methods;
- InlineArrayTransform: 4 methods.

The 78 methods are in the allowlist, with the reason, for the FORK review. The managed
pipeline runs the same rewrites in its later ExpressionTransforms pass, and the text oracle
agrees.

**An arena fix.** Removing items from a list re-adopted the items left (their parent was reset
to the list's owner). The managed collection only renumbers the items that are still its
children, so an instruction moved elsewhere keeps its new parent. ConditionDetection was the
first transform to edit a list after moving its items out, and the walk from an exit to its
block then failed.

The no-op red run failed 2,635 of 3,550 equality cases; the first port without
ExpressionTransforms 1,978.

### 3.48 The FORK review and LockTransform (items 32 and 33, IL prefix 26)

**The FORK.** The review covered the 78 methods allowlisted in item 31: 62 NullableLifting,
12 array and span initializer, 4 InlineArray. The human decided to keep them. Each group
leaves the allowlist in the step that ports its transform, and that step only passes once
its entries are gone. The policy stays as it is.

**Pipeline leaves in groups.** LockTransform is the second leaf of the second
`BlockILTransform`. The managed truncation gives such a group one truncated copy that runs
every leaf it keeps on each block. The engine's pipeline was a flat list of functions, so
two leaves would have meant two driver passes. Each leaf is now either a whole-function
transform or a block transform tagged with its group, and consecutive leaves of one group
run in one pass of the block driver.

**The port.** `il/transforms/lock_transform.rs` matches the five compiler shapes of `lock`
(Roslyn, the C# 4 flag forms, the C# 4 iterator form, C# 2, mcs) in the managed order. It
walks the block backwards from the first already-transformed instruction, as the managed
loop does. The arena, the wire (opcode 70) and the host gained the `lock` instruction, and
`LockStatement` became settings bit 20. Its body is an exit slot for DetectExitPoints, its
expression is the one inlining slot, and the data flow visits both in order.

**Allowlisted.** Four methods: the mcs iterators `YieldReturnInLock1` and `YieldReturnInLock2`,
each built with and without optimizations. Their finally body comes from Mono's separate finally method, which
YieldReturnDecompiler inlines without releasing it. When the lock drops the finally, managed
still counts its loads, because that detached function holds a reference. The engine counts
the same. The host cannot, because it rebuilds the counts from the tree the wire sends. Only
the variable use counts differ.

The no-op red run failed 151 of 3,550 equality cases.

### 3.49 UsingTransform (item 34, IL prefix 27)

UsingTransform is the third leaf of the second block group. It turns a store followed by a
try-finally that disposes the stored variable into a `using` instruction, in three shapes:
- the C# shape, with the store before the try;
- the VB shape, with the store as the first statement of the try;
- `await using`, whose finally awaits `DisposeAsync`.

The dispose check covers:
- null checks and type checks of reference types (including an inlined `isinst` and the
  `match` instruction of PatternMatchingTransform);
- nullable resources (`HasValue` and `GetValueOrDefault`);
- boxed value types;
- ref structs with their own `Dispose`.

The arena, the wire (opcode 71) and the host gained the `using` instruction. Like
`pinned.region`, it is a store of its variable. Settings bits 21 to 23 are `UsingStatement`,
`AsyncUsingAndForEachStatement` and `IntroduceRefModifiersOnStructs`. ILInlining gained
`IsUsedAsThisPointerInCall` and `IsPassedToInParameter`, which item 37 needs as well. The
`obj?.Dispose()` shapes need `nullable.rewrap`, which the engine has not ported yet, so
they never match.

**Reference equality.** For a ref struct, the managed matcher compares the declaring type of
the `Dispose` call with the variable's type using `==`. For interfaces this is reference
equality. A type definition is one object, but each signature creates its own instance of a
parameterized type. So a generic ref struct such as `ValueListBuilder<char>` never matches
in managed. The engine's types are interned, so the port allows only type definitions.

**Shifted tokens.** The `ICSharpCode.Decompiler.dll` fixture is the assembly the tests run,
and it contains the host's interop code. The new wire reader method in the materializer
shifted the tokens of all later methods by one. The allowlist keys methods by token, so 8
entries no longer matched their methods. They were remapped by method name. Any host
change that adds methods can do the same.

The no-op red run failed 756 of 3,550 equality cases.

### 3.50 CachedDelegateInitialization (item 35, IL prefix 28)

The compilers cache the delegates of lambdas and method groups. The cache lives in a static
field (the older C# compilers and Mono), in a local (Roslyn), or in a stack slot (VB). The
transform removes the `if (cache == null) cache = new D(...)` check and puts the delegate
construction where the cache is read. It covers all seven managed shapes, including the
discarded method-group conversion, which keeps the construction and drops the caching.
`AnonymousMethods` became settings bit 24. The construction matcher went into its own
module, `il/transforms/delegate_construction.rs`, where the rest of DelegateConstruction
will follow in item 56. Without `ldvirtdelegate` and nested functions, it matches
`newobj D(target, ldftn/ldvirtftn M)`.

**Kinds of parameterized types.** The matcher accepts a delegate type, or a type of unknown
kind. A parameterized type takes its kind from its generic type. So `Action<Hidden>` over
an `Action` the fixture cannot resolve counts as unknown, and managed accepts it. The first
port asked for the type definition, found none and rejected it.

The no-op red run failed 64 of 3,550 equality cases.

### 3.51 CachedReadOnlySpanInitialization (item 36, IL prefix 29)

On frameworks without `RuntimeHelpers.CreateSpan`, Roslyn caches the array behind a
`ReadOnlySpan<T>` literal in a field of `<PrivateImplementationDetails>`. The transform
matches the load of the cache, the `if (V == null)` that fills it and the write-back. The
load then gets the initializer and the if goes away, so the output never names the cache
field, which C# cannot express. `ArrayInitializers` became settings bit 25.

The no-op red run failed 481 of 3,550 equality cases, almost all in the runtime assemblies,
which use such span literals throughout.

### 3.52 StatementTransform and ILInlining with ldloca (item 37, IL prefix 30)

The last leaf of the second block group is a `StatementTransform`. Its children run on each
statement of a block, from the last one not yet transformed down to the first. A child can
ask for a position to be run again. The engine's pipeline gained statement leaves.
Consecutive statement leaves of a group form one statement group, which the block driver
runs like any other post-order transform (`il/transforms/statement_transform.rs`). The
first child is ILInlining with `AllowInliningOfLdloca`, which brings several pieces the
early inlining stage never needed:
- `OptionsForBlock`: aggressive inlining in catch-when filters, in constructor initializers
  (before the chained `base(...)`/`this(...)` call, an offset the function computes once and
  keeps) and under `AggressiveInlining`, the new settings bit 26;
- `IsGeneratedTemporaryForAddressOf`: an `ldloca` of a temporary becomes `addressof(value)`
  when the temporary is the `this` of a call, an `in` argument, the argument of a
  `ReadOnlySpan<T>` constructor or the target of an mcs field read. This depends on the
  expression's classification (r-value, mutable or read-only l-value) and on whether the
  method would copy a read-only `this`;
- the parents that `NonAggressiveInlineInto` was missing: lifted comparisons, `string.to.int`
  as a switch value, `match`, `ldelema` with `System.Index`, `get.pinnable.reference`.

**The test harness.** In Debug builds, the managed `StatementTransform` checks the
invariants of every statement after every child. On one runtime method that check reads
past the end of a stale child list and throws, which a Release build never does. The
reference already ran the block driver itself, without its Debug checks. Now it runs the
statement driver the same way, as a Release build does.

**Allowlisted.** Inlining shortens then-branches, so ConditionDetection inverts more ifs
in the parent blocks. Each inverted if runs ExpressionTransforms, whose NullableLifting part
is not ported. Eleven methods of `ICSharpCode.Decompiler.dll` joined the NullableLifting
group, with its reason.

The no-op red run failed 2,131 of 3,550 equality cases. The first port failed 1,175, mostly
because ldloca inlining was still missing.

### 3.53 ExpressionTransforms in the statement group (item 38, IL prefix 31)

The second leaf of the statement group runs the visitor from item 31 on every statement.
Before that, the visitor only ran on the ifs ConditionDetection inverted. Its rerun requests
now reach the statement driver: after a store to a local, after a conditional operator,
after a switch expression and after a compound assignment moved to the top level. A flag on
the context carries them; `RunOnSingleStatement` drops it, as the managed method does with
its fresh statement context.

The no-op red run failed 2,392 of 3,550 equality cases. About 13,500 method and settings
pairs differed, so the step ported the rest of the visitor, in order of impact:
- `TransformAssignment.HandleCompoundAssign`, with three new instructions: numeric,
  user-defined (operators and `string.Concat`) and dynamic compound assignments. These
  account for about 7,000 differences. The rest of TransformAssignment stays with item 40.
- `newobj Span<T>(localloc(n), count)` becomes `localloc.span T(count)`, a new instruction
  (also with a stackalloc initializer block).
- `ldvirtdelegate`, a new instruction, and `GetValueOrDefault(a, b)` as `a ?? b`
  (`if.notnull`, a new instruction).
- InlineArrayTransform, with `ldelema.inlinearray`, a new instruction; it has no step of
  its own.
- The dynamic set-member and set-index compound rewrites, and the dynamic add/remove event
  pattern.

Settings bits 27 to 32 are `MakeAssignmentExpressions`, `IntroduceIncrementAndDecrement`,
`CheckedOperators`, `NativeIntegers`, `UnsignedRightShift` and `InlineArrays`. The host
materializer now builds a transform context with the request's settings, because the
constructor of the numeric compound assignment checks its operands against them in Debug
builds.

**Gaps that new instructions exposed.** Older ports had left out cases for instructions
the arena did not have yet:
- the increment-block test of ConditionDetection (`IsSimpleStatement` accepts compound
  assignments, so a loop's increment block is recognised);
- ILInlining's targets: getters under a compound assignment, `localloc.span`,
  `ldelema.inlinearray`, nullables into `??`, and dynamic compound assignments;
- `MatchDelegateConstruction` with `ldvirtdelegate`.

Each shows up as a divergence in methods that never used the new instruction directly: the
changed tree makes ConditionDetection or ILInlining choose differently. A variable removed by
`RecombineVariables` leaves the list by swap-remove, as `ILVariableCollection.Remove` does.

**Allowlisted.** ExpressionTransforms calls transforms that have their own steps later in the
plan. Following the FORK decision, their divergences wait for those steps:
- NullableLiftingTransform and NullPropagationTransform, items 42 and 43: 3,413 methods;
- the span and CreateSpan initializers of TransformArrayInitializers, item 44: 633 methods;
- UserDefinedLogicTransform, item 51: 296 methods;
- HandleEmptyWith of TransformCollectionAndObjectInitializers, item 45: 48 methods.

A harness mode sorted every difference by comparing counts of each group's instructions in
the two printed trees. No difference was left without a group. `ICSharpCode.Decompiler.dll`
entries now name their method by signature, because the host's new reader methods shift
that fixture's tokens with every step that adds instructions. The allowlist check became a
hash lookup.

### 3.54 DynamicIsEventAssignmentTransform (item 39, IL prefix 32)

A dynamic `d.Event += handler` lowers to a runtime check: if the member is an event, the
code calls its `add_` accessor, otherwise it runs a dynamic compound assignment. The
compiler caches the is-event flag and the member's current value in locals and may copy the
value into a temporary. The diamond shows up as an if/else, possibly nested in an argument,
or, when the result is returned, as two leaves. The transform matches both, removes the
cache, and reuses the two rewrites from item 38 (the set-member compound and the add/remove
pattern) to leave one `dynamic.compound` instruction. It then reruns from the collapsed
statement.

The no-op red run failed 16 of 3,550 equality cases; the port passed on its first run.

### 3.55 TransformAssignment (item 40, IL prefix 33)

Item 38 already brought `HandleCompoundAssign`, the part ExpressionTransforms calls. As a
statement transform, TransformAssignment adds the rest:
- inline assignments into fields and array elements (`stloc l(stobj(target, value))`),
  into setter calls (a `CallInlineAssign` block around the call, with a new stack slot for
  the value) and into locals (`stloc s(stloc l(value))`);
- the increment and decrement operators: post-increment with an inline store, the plain
  post-increment (`stloc tmp(x); x = tmp + 1`), and pre-increment with an inline store.

The plain post-increment needs the instruction before the load. Roslyn sometimes computes an
address twice (`stloc S_0(ldloc ref); ... stobj(ldloc S_0, ...)`), and
`IsMatchingCompoundLoad` accepts that duplicated computation only when it knows the store of
`S_0`. A sign mismatch can be repaired by giving the target the opposite sign (`SwapSign`), as
the managed transform does.

Rerun requests from transforms that run without the statement context, such as
HandleCompoundAssign inside an inline assignment, now go to the statement driver after
every child, not only after ExpressionTransforms.

Two gaps showed up:
- `NonAggressiveInlineInto` always inlines compound assignments into the next statement.
  Without that case `a[this._size++] = x` kept its temporaries.
- An unresolved `set_X` is guessed to be the setter of a fake property (with more than one
  parameter, an indexer), as `MetadataModule.GuessFakeMethodAccessor` does. Inline
  assignments and compound stores into such setters depend on it.

The no-op red run failed 957 of 3,550 equality cases; the first port 104.

### 3.56 NullCoalescingTransform (item 41, IL prefix 34)

The transform turns null checks into `??` (`if.notnull`, the instruction item 38 added):
- `stloc s(value); if (s == null) stloc s(fallback)` becomes `stloc s(value ?? fallback)`.
  This also works with a single-use temporary in the if, and with a throw instead of the
  fallback (a throw expression, under the new settings bit 34, `ThrowExpressions`).
- A null guard of a parameter that the compiler hoisted in front of a constructor
  initializer folds into the argument as `param ?? throw ...`.
- `if (!v.HasValue) throw ...` before `v.GetValueOrDefault()` becomes
  `value ?? throw ...` for nullable value types.

The last two build the `??` tentatively to ask ILInlining's `FindLoadInNext` whether it may
move into the next statement. Building it re-parents the operands, so a failed attempt puts
them back into their old slots, as the managed code does through its `Children` setters.

The no-op red run failed 284 of 3,550 equality cases; the port matched on its first run.

### 3.57 NullableLiftingTransform (item 42, IL prefix 35)

C# lifts operators to nullable operands: for `int? a, b`, `a + b` is null when either side is
null. The compiler spells that out as `HasValue` checks and `GetValueOrDefault()` calls; the
transform folds them back into one lifted instruction:
- `a.HasValue & b.HasValue ? new int?(a.GetValueOrDefault() + b.GetValueOrDefault()) : null`
  becomes `binary.add.lifted(a, b)`; conversions, `~` and user-defined operators lift the
  same way.
- Comparisons lift too (`comp.lifted`), including the equality special cases where the
  compiler compares the values first and the `HasValue` flags second.
- `bool?` `&` and `|` become the three-valued `3vl.bool.and`/`3vl.bool.or`.

It runs in two places, as in the managed code: as its own statement transform, and from
ExpressionTransforms on comparisons, ifs and `bit.and` of bools. The lifted forms needed new
wire flags (`BinaryNumeric`, `Conv`, `BitNot` and calls carry whether they are lifted), and
the earlier ports that match operators now reject the lifted ones where the managed code
checks `IsLifted` (compound assignments, increments, `FixComparisonKindLdNull`).

The managed transform first tries NullPropagationTransform (`?.`), which is item 43. Until
then those methods stay allowlisted under that item; the rest now match, and the allowlist
shrinks from 4,473 to 3,771 entries.

The no-op red run failed 111 of 3,550 equality cases; the port then matched every case
outside the allowlist.

### 3.58 NullPropagationTransform (item 43, IL prefix 36)

The transform recovers the null-conditional operator `?.`:
- `v != null ? v.M() : null` becomes `v?.M()`.
- `if (v != null) v.M();` becomes `v?.M();`.
- `v.HasValue ? v.GetValueOrDefault().M() : default` handles nullable value types the
  same way.
- With a value-type result and a non-null fallback, it becomes `v?.M() ?? fallback`.

`?.` needed two new instructions:
- `nullable.unwrap` (wire opcode 81) marks where a null input leaves the chain.
- `nullable.rewrap` (82) marks where the chain resumes with null.

Inlining, the flags and the data flow visitor learned their rules: an unwrap is a
conditional jump to its enclosing rewrap. UsingTransform now also matches the
`obj?.Dispose()` form its `finally` takes after this step. NullableLiftingTransform calls
the transform first, as the managed code does; that removes the last item 42 shortcut.

The managed code also asks the C# resolver whether a static extension method call can be
written as `x?.M()`. The IL pipeline the tests compare runs without a using scope, where the
resolver finds no extension methods, so the port rejects static calls (a shortcut until the
semantics step, item 67). The new settings bit 35 is `NullPropagation`.

The no-op red run failed 488 of 3,550 equality cases. The port then matched every case
outside the allowlist, which shrinks from 3,771 to 1,049 entries; two newly visible array
initializer cases are allowlisted for item 44.

### 3.59 TransformArrayInitializers (item 44, IL prefix 37)

The compiler fills a new array element by element, or copies constant data into it with
`RuntimeHelpers.InitializeArray`. The transform folds these back into one array initializer
block (`new int[] { 1, 2, 3 }`):
- one- and multi-dimensional arrays, including element stores mixed with constant data and
  nested (jagged) arrays;
- `stackalloc` buffers filled by `cpblk`, `initblk` or stores at increasing offsets;
- `ReadOnlySpan<T>` over field data (`new ReadOnlySpan<T>(ptr, n)` and
  `RuntimeHelpers.CreateSpan<T>`), which ExpressionTransforms turns into an array
  initializer or a UTF-8 string literal (`ldstr.utf8`, the new wire opcode 83).

The constant data sits in a field with an RVA. The port reads it from the PE section, sized
like the managed `FieldValueSizeDecoder` (primitives, pointers, and the class size of a
struct), always from the main module, as the managed code does.

Two pieces of infrastructure came with this step:
- `ILInstruction.Extract`, the opposite of inlining. It moves an expression out into a
  store to a new stack slot; the `stackalloc` initializer uses it for a buffer that goes
  straight into a `Span<T>` constructor.
- `Function::register_variable`. The span rewrites run inside ExpressionTransforms, which
  sees only the function tree, so the `I_n` counter moved into `Function`. New variables wait
  in a pending list that the transform drivers append to the variable list after each step,
  which keeps the managed registration order.

The settings bits 36 and 37 are `StackAllocInitializers` and `Utf8StringLiterals`.

The no-op red run (with the expression transform hooks already in place, item 44 entries
removed) failed 835 of 3,550 equality cases. The first green run left 22 divergences: the
`stackalloc` element stores keep their own type (`RewrapStore` uses the store's type, not
the element type it is given), and buffers that feed a `Span<T>` need the extraction. After
both fixes the port matched every case outside the allowlist, which shrinks from 1,049 to
413 entries.

### 3.60 TransformCollectionAndObjectInitializers (item 45, IL prefix 38)

After `stloc v(newobj T(...))` the compiler emits one statement per initializer item. The
transform collects them into an initializer block:
- `Add` calls on a collection;
- property setters and field stores, also nested (`{ A = { B = 1 } }`);
- indexers, also in dictionary initializers, whose index values sit in single-assigned
  locals first.

It keeps the access path of each item (the chain of members from `v`) and a stack of the
members already initialized at each level, so that no member is set twice and a collection
level takes only `Add` calls. Indices compare with the structural `ILInstruction.Match`
that `structurally_equal` already ports. `<Clone>$` of a record becomes a `with` initializer
block, from ExpressionTransforms (`HandleEmptyWith`); the transform then extends it with the
assignments that follow.

Several checks rely on the C# resolver, which the IL pipeline the tests compare creates
without a current type or using scope:
- An extension `Add` method never applies, since there are no extension methods without a
  using scope.
- A setter counts as accessible only when it is public, or internal to the main module.
  InternalsVisibleTo is a shortcut until the semantics step, item 67.
- A generic `Add` method needs C# type inference from its own parameter types. The port
  checks that every type parameter occurs in a parameter, which is when that inference
  succeeds (also a shortcut for item 67).

The type system gained `IsRecord`, anonymous type detection, tuple-compatible types and the
full `IsPotentialClosure`.

The new settings bits 38 to 41 are `WithExpressions`, `UseObjectCreationOfGenericTypeParameter`,
`ObjectOrCollectionInitializers` and `DictionaryInitializers`.

The no-op red run (with the `HandleEmptyWith` hook in place, item 45 entries removed) failed
507 of 3,550 equality cases. The first green run left one divergence: two variables in
swapped order. The block driver applied TransformAssignment's variable removals only after
the whole step; the managed collection removes at once. The statement driver now applies
queued removals after each transform, before the new variables join the list. Then the
port matched every case outside the allowlist, which shrinks from 413 to 365 entries.

### 3.61 TransformExpressionTrees (item 46, IL prefix 39)

For a lambda converted to `Expression<T>`, the compiler emits calls to the
`System.Linq.Expressions.Expression` factories:
- `Parameter` for each parameter;
- `Add`, `Call`, `Property`, `Constant` and the other factories for the body;
- finally `Lambda`.

The transform turns these calls back into an `ILFunction` lambda (kind `ExpressionTree`,
`.ET` in the ILAst) whose body is ordinary ILAst. Later the C# backend prints it as
`x => x.Name`.

The engine had no nested functions, so this step adds them:
- `Inst::Function`, a lambda with its own parameters, variables, helper counter and body
  container (wire opcode 84). Its flags do not propagate out of the body, as in the
  managed `ILFunction`.
- `expression.tree.cast` (opcode 85).
- `CaptureScope` on variables. It travels after the body on the wire, because it refers
  to containers.

The variables of nested functions travel in a second list after the top-level ones, and
each function names its own.

The managed transform works in two phases. Matching builds a tree of `Func<ILInstruction>`
builders without touching the function, and only a full match runs them. The port keeps
that shape with boxed `FnOnce` builders over an environment that holds the function, the
context and the transform state.

The C# semantics it needs (implicit conversions for `Coalesce`, reference conversions for
`Condition`, user-defined comparison operators) are approximated for the operand types
expression trees produce, as a shortcut until the semantics step (item 67). One detail
mattered: the managed resolver ignores the operators that the framework declares on `bool`,
`char`, the numeric types and `decimal`, since C# treats them as built-in. Without that
filter a `decimal` comparison became an `op_GreaterThan` call.

The new settings bit 42 is `ExpressionTrees`.

The no-op red run failed 54 of 3,550 equality cases (all ExpressionTrees fixtures). The
port then matched every case outside the allowlist, which stays at 365 entries.

### 3.62 IndexRangeTransform (item 47, IL prefix 40)

C# lowers `list[^i]` and `span[a..b]` to plain arithmetic:
- `int length = list.Count; list[length - i]` for indices from the end;
- `int start = a.GetOffset(length); span.Slice(start, b.GetOffset(length) - start)` for
  ranges.

The statement part of the transform recognizes these runs of stores and folds them back
into one call with a `System.Index` or `System.Range` argument. It also extends a slice it
folded earlier from a partial view of the pattern. Item 21 already ported the array part
(`HandleLdElema`).

The folded call names a `SyntheticRangeIndexAccessor`: the indexer or `Slice` method with
an index or range parameter in place of its integers, a method that exists only in the
decompiler. It travels as a flag on the call's method operand (the type and whether it
slices). The host wraps the resolved underlying method in the managed class, as it already
does for lifted operators.

`Range.All` is the first property getter the engine names by lookup rather than by token,
so the host's lookup now includes accessors. Two inlining checks the transform needs,
`CanInlineInto` and `CanMoveInto`, joined the inlining module.

The no-op red run failed 19 of 3,550 equality cases (the IndexRangeTest fixtures). The port
then matched every case outside the allowlist, which stays at 365 entries.

### 3.63 DeconstructionTransform (item 48, IL prefix 41)

`var (a, b) = point;` compiles to a `Deconstruct` call with `out` temporaries, and
`var (a, b) = tuple;` to reads of `Item1` and `Item2`. Both may be nested and followed by
conversions and assignments. The transform folds such a run of statements back into one
`deconstruct` instruction (wire opcode 86) with four parts:
- the init stores;
- a pattern of `match.deconstruct` or `match.tuple` nodes over `deconstruct.result`
  elements (opcode 87);
- the conversions;
- the assignments.

Matching never changes the block. As in the managed transform, every rewrite waits in a
list of delayed actions until the whole sequence matched. The port keeps those actions as
an enum (set a variable type, add an assignment, forward a single-use element through a
fresh variable, and so on), because the managed closures read transform state at the time
they run.

The new instruction keeps its collection (the init stores) before three fixed children.
The arena's list operations used to handle only a collection after fixed children (or
before one final instruction), so they now renumber any number of trailing children.
`FindLoadInNext` gained the `FindDeconstruction` result, which lets a preceding store become
an init store.

The C# conversion approximations from item 46 moved into a shared module that also reports
the kind of a conversion (identity, numeric, nullable, reference, boxing). The transform
accepts a matched `conv` only as the lowering of an identity, reference or numeric
conversion, as the managed `IsCompatibleImplicitConversion` does. These approximations
remain a shortcut until the semantics step (item 67).

The new settings bit 43 is `Deconstruction`.

The no-op red run failed 53 of 3,550 equality cases. The first green run threw in the
materializer, which filled the init collection before the pattern was set; the managed
collection renumbers the fixed children on every insert. With that fixed the port matched
every case outside the allowlist, which stays at 365 entries.

### 3.64 NamedArgumentTransform (item 49, IL prefix 42)

C# evaluates arguments left to right, but a named argument can be written in another
position: `M(b: F(), a: G())` evaluates `F()` first. When a stored value would have to be
inlined into an argument after others that it may not be reordered with, the transform
introduces a named argument instead. The argument moves into a store at the front of a
`CallWithNamedArgs` block (after the `this_arg` store of an instance call), and the stored
value then inlines as usual.

The transform itself is one line, an inlining with the `IntroduceNamedArguments` option. The
work is in ILInlining:
- a `NamedArgument` result of `FindLoadInNext`;
- the two searches that produce it, into a call whose argument can be named or into an
  existing named-argument block;
- the introduction of the named argument before the inlining.

The managed checks for whether an argument can be named carry over: not before the this
pointer, operators, accessors other than indexers, varargs, delegate or anonymous type
constructors, and parameter names that are not valid identifiers. The `this_arg` variable
is the first one a transform registers under a given name in code that sees only the
function, so `Function` gained a named registration.

The new settings bit 44 is `NamedArguments`.

The no-op red run failed 124 of 3,550 equality cases. The port then matched every case
outside the allowlist, which stays at 365 entries.

### 3.65 RemoveUnconstrainedGenericReferenceTypeCheck (item 50, IL prefix 43)

For a constrained virtual call on a value of an unconstrained type parameter, Roslyn first
copies a reference-type value into a temporary, so that the arguments cannot change which
object the call goes to. The pattern is:
- `default(T)` stored in a temporary;
- an `== null` test on that temporary, whose branch copies the value and redirects the
  address;
- one store per argument;
- the call through `ldobj.if.ref`.

That form already keeps the receiver stable, so the transform drops the check. The argument
stores then inline back into the call. This needs `CanMoveIntoCallVirt`, a variant of
`CanMoveInto` that ignores the `this` argument when it checks reordering; the inlining
module now has both, built on one helper.

The no-op red run failed 25 of 3,550 equality cases. The port then matched every case
outside the allowlist, which stays at 365 entries.

### 3.66 UserDefinedLogicTransform (item 51, IL prefix 44)

A type can overload `&&` and `||` through `op_False`/`op_BitwiseAnd` and
`op_True`/`op_BitwiseOr`. The compiler lowers `a && b` to `op_False(a) ? a : op_BitwiseAnd(a,
b)` and spells it out as an if, in different shapes:
- legacy csc, as a stack slot store;
- Roslyn, as an expression;
- optimized Roslyn, as two returns.

`&&` and `||` on dynamic operands are lowered the same way, through the dynamic `IsFalse`,
`IsTrue`, `And` and `Or` operations. The transform folds all of these back. The statement
transform handles the legacy and the return shapes. ExpressionTransforms handles the
expression shape on every if, which is where the 296 allowlist entries of item 51 came from.

Two instructions are new:
- `user.logic`, wire opcode 88;
- `dynamic.logic.operator`, a new kind of the existing dynamic instruction.

In both, only the left operand always runs. The flags combine the right operand as a branch,
inlining stops at it, and the data flow visitor joins the state after it, as for `??`. The
call writer's method encoding moved into a helper, which `user.logic` reuses.

The red run (with the ExpressionTransforms hook in place, item 51 entries removed) failed 21
of 3,550 equality cases. The port then matched every case outside the allowlist, which
shrinks from 365 to 69 entries.

### 3.67 InterpolatedStringTransform (item 52, IL prefix 45)

Since C# 10, `$"a{x}b"` compiles to calls on a `DefaultInterpolatedStringHandler` local:
- the constructor with the literal length and the hole count;
- `AppendLiteral` and `AppendFormatted` per part;
- `ToStringAndClear` where the string is used.

The transform moves those calls into a `Block InterpolatedString` that replaces the
`ToStringAndClear` call and has it as its final instruction. The C# backend prints that
block as the interpolated string. The handler variable becomes an initializer target. All
calls must be ones the backend can print: the known overloads with literal format
arguments. Each must also be able to move into the statement that holds the
`ToStringAndClear` call, which `FindLoadInNext` checks.

This step is the first whose prefix ends the statement transform group. The shim rounds the
prefix down to a top-level boundary, so the oracle now runs everything since item 30 through
the engine, and it found three problems the IL prefix equality test could not show:
- The materializer built the binary instruction of a `NumericCompoundAssign` with placeholder
  operands. The constructor's Debug assertion inspects the right operand for pointer
  targets, so it now gets the value, as in TransformAssignment.
- With a using scope, the real decompiler's resolver finds extension methods. The
  null-propagation and collection-initializer checks of items 43 and 45 assumed it never
  does, which is only true in the compared IL pipeline. A first fix reported such methods
  as unsupported and let the shim fall back to the managed pipeline, but the ratchet's
  served share then dropped below 100 percent. Instead, the engine now asks the host. A
  second callback on `ilspy_session_read_function`, next to the debug-info one, takes a
  small wire message: the instantiated method in the `call_method` encoding, and for the
  null-propagation check the target type, the target variable's and its store's types, and
  the argument types. The host decodes it in the decompiled method's generic context and
  answers with its own `CSharpResolver`, so the answer is the managed one by construction.
  Without the callback (the IL prefix equality harness) no extension method is found, as
  in the compared pipeline. Porting the resolver itself stays with the semantics step
  (item 67).
- The collection-initializer check reads the `ExtensionMethodsInCollectionInitializers`
  setting (bit 46) before it asks the resolver.

The new settings bit 45 is `StringInterpolation`.

The no-op red run failed 70 of 3,550 equality cases. The port then matched every case
outside the allowlist, which stays at 69 entries.

### 3.68 ProxyCallReplacer (item 53, IL prefix 46)

An async method that calls `base.M()` cannot do so from its state machine. The compiler
emits a proxy method such as `<>n__0` on the class: its body only calls the base method
with the proxy's own parameters. The transform finds calls to such compiler-generated
proxies of the current or an outer class. It reads each proxy's IL and runs the early IL
transforms over it, as the state machine decompilers do for their methods. If the body is
one forwarding call, the transform replaces the proxy call with a call to the forwarded
method. The arguments, the IL range and the "stack was empty" flag stay those of the proxy
call.

The forwarded method's token belongs to the proxy, not to the decompiled method. It
resolves in the proxy's generic context, and the managed code then specializes the result
by the proxy call's substitution. The engine computes the same method with its type
system's `specialize_method`. For the host, a method operand gained an optional
`specialized_by` link to the proxy call's operand, and the wire writes that operand first.
The materializer resolves the token in `new GenericContext(proxy)` and calls
`Specialize(proxy.Substitution)`, so the host reproduces the managed steps exactly. The
engine changes the call node in place rather than building a new one. Its children and IL
range then stay as they were, which is what `ReplaceList` and `AddILRange` give the
managed code.

The no-op red run failed 24 of 3,550 equality cases. The port then matched every case,
and the allowlist stays at 69 entries.

### 3.69 FixRemainingIncrements (item 54, IL prefix 47)

TransformAssignment turns a user-defined `op_Increment` or `op_Decrement` call into a
compound assignment when it finds the variable being incremented. Roslyn sometimes
optimizes that variable away and emits `Console.WriteLine(UserType.op_Increment(a + b))`.
This transform fixes the calls that are left (decimal excepted, which
ReplaceMethodCallsWithOperators handles). A call stored straight into a variable,
`stloc V(call op_Increment(x))`, becomes `stloc V(x)` followed by a compound assignment to
`V`. Any other call has its argument extracted into a new variable, as
TransformArrayInitializers did in item 44. That variable takes the operator's parameter
type, and the call becomes a compound assignment to it.

Two details matter for equality. The managed code collects the calls in `Descendants`
order, which is post-order (children first, the node itself last), and each extraction
registers a variable. The port walks the tree in the same order, so the new variables get
the same numbers and positions.
The new compound assignment takes the call's IL range.

The no-op red run failed 9 of 3,550 equality cases. The port then matched every case, and
the allowlist stays at 69 entries.

### 3.70 CopyPropagation (item 55, IL prefix 48)

The state machine decompilers already used `Propagate` (items 13 and 14). This step adds the
pipeline transform around it. For every control-flow block, in post-order (`Descendants`), it looks at
single-definition stores. A dead store to a stack slot is removed when its value is pure.
Otherwise only its value stays, as a statement. Other stores are copied into their loads
when `CanPerformCopyPropagation` allows it. That covers address loads (ldelema and ldflda
only without `UseRefLocalsForAccurateOrderOfEvaluation`) and loads of single-definition
variables or parameters into stack slots or ref locals. It also covers flag-free leaf
instructions stored to stack slots. A variable whose IL local the function lists in several
parts (split variables, compared with `ILVariableEqualityComparer`) is never copied.

One detail needs care: the expression-tree lambdas of item 46 already exist at this point.
`DoPropagate` adds its `C_n` temporaries to `v.Function.Variables`, so for a block inside
such a lambda the port adds them to that lambda's variable list, not the top-level one.
The split-variable set comes from the top-level list only, as in the managed code.

The no-op red run failed 586 of 3,550 equality cases. The port then matched every case, and
the allowlist stays at 69 entries.

### 3.71 DelegateConstruction (item 56, first leaf, IL prefix 49)

Item 56 has three pipeline leaves. With the human's agreement it lands as three gated
commits, one per leaf. Each leaf is a top-level boundary, so the oracle checks each one on
its own: at this prefix the managed LocalFunctionDecompiler and TransformDisplayClassUsage
already run on the lambdas the engine built.

DelegateConstruction turns `newobj D(target, ldftn M)` of an anonymous method into a
nested function holding `M`'s body. The managed code reads the body, puts the lambda in
the tree, runs every IL transform before itself and CombineExitsTransform over it, replaces
its `this` with the delegate target, and recurses into nested lambdas. Three design
decisions, taken with the human, shape the port:

- **Explicit generic contexts.** A lambda is read in a context built from the type
  arguments of its delegate target (`GenericContextFromTypeArguments`): the display class's
  type parameters stand for the caller's. The type system gained a table of such contexts,
  and `GenericContext` can point into it. Member operands carry a context index on the wire
  instead of the old state-machine bit: 0 for the function's own, 1 for the state machine
  context, `n + 2` for explicit context `n`. Each function message starts with the table,
  and the host rebuilds every entry with `new GenericContext(class, method)`. An index on
  each operand, rather than a context switch by tree position, survives the later
  transforms that move outer instructions into lambdas. The state machine decompilers now
  derive their context from the function they work on, so async lambdas read their
  `MoveNext` in the lambda's context, as `CreateILAst` does.
- **Separate arena, then import.** The lambda is read into its own `ReadFunction`, and the
  prefix runs there. The existing arena import, which the state machine decompilers use,
  then moves it into the function's arena. This is equivalent to the managed order because
  the body refers only to its own variables until the target replacement afterwards.
- **Three gated commits**, as above.

Several smaller pieces came with it. A nested `Function` instruction gained a reader
header: method, context, sizes, iterator and async state, warnings and sequence points.
The host builds it with `new ILFunction(definition.Specialize(context.ToSubstitution()),
...)`, as `ReadIL` does. `CapturedVariables` became explicit per function, on the arena and
on the wire, because DelegateConstruction sets capture scopes without the expression-tree
rule that derived the list from them. `ReadFunction` records its kind, because
`RemoveDead` drops an unused `this` of a non-top-level function. The reader now sees
parameter types through an explicit context, like `method.Parameters` of the specialized
method. CombineExitsTransform and the local-function name test (`IsLocalFunctionMethod`)
were ported too, both needed here.

The no-op red run failed 739 of 3,550 equality cases. The port then matched every case,
and the allowlist stays at 69 entries.

### 3.72 LocalFunctionDecompiler (item 56, second leaf, IL prefix 50)

LocalFunctionDecompiler finds every use of a compiler-generated local function method: a
call, an `ldftn`, or a delegate construction. It reads each method once into a nested
function of kind LocalFunction, running every IL transform before itself, DelegateConstruction
included. It then replaces the function's `this` and closure parameters with the display
classes the use sites pass. It decides where the function is declared, moves it to the
function owning that scope, and turns the use sites into calls of the reduced method, the
`LocalFunctionMethod` without the compiler-generated parameters.

Local functions are children of an `ILFunction`, after its body. The engine's nested
`Function` instruction gained that child list, a declaration scope and the reduced method's
counts. The top-level function, which has no instruction of its own, keeps its local
functions on the arena, each one a root. A use site's method operand can name the local
function it refers to. The host then builds `new LocalFunctionMethod(read method, ...)
.Specialize(useSite.Method.Substitution)`, which is the managed expression; the type's
equality is by value, so separately built instances compare equal. `ldftn` now uses the call
method encoding, so it can carry that reference too. A local function without a body
(`extern`, as the generated P/Invoke stubs in the runtime libraries use) gets a `nop` body
and the use site's method, as in the managed code. Two settings bits were added: 47
`LocalFunctions` and 48 `StaticLocalFunctions`.

Two findings came out of the corpus. First, `IsLocalFunctionDisplayClass` decodes each local
function signature with a `FindTypeDecoder` but then asks `ParameterTypes.Any()`, so any
parameter counts. Because of that, decompiling a state machine's `MoveNext` directly turns
the transform off. The port copies the test as it is. Second, `ILInstruction.Descendants`
is a post-order walk, children first and the node itself last. FixRemainingIncrements,
CopyPropagation and DelegateConstruction had been ported with a pre-order walk; the
difference only showed here, in the order local functions are read. All four now walk in
post-order, and sections 3.69 and 3.70 were corrected.

The oracle found one more difference that the equality printer cannot show. A generic local
function's own type parameters, in the managed code, are the use-site method's
`TypeParameters`. Those are specialized type parameters, whose constraints are seen through
the caller's type arguments: in `ZZ<T3>() where T3 : T2` the constraint is the caller's
`T2`. The engine had sent the definition's type parameters, so the C# backend inserted
casts. An explicit context can now name a member token whose method type parameters it
uses from some index on. Inside the local function, those type parameters are a new owner
kind, "method type parameter `i` of explicit context `e`", which the host takes from the
context. Since such a type cannot be built before the function header defines its
contexts, the type table is now length-prefixed and decoded on first use.

The no-op red run failed 204 of 3,550 equality cases. The port then matched every case,
and the allowlist stays at 69 entries.

### 3.73 TransformDisplayClassUsage (item 56, third leaf, IL prefix 51)

TransformDisplayClassUsage finishes what the two leaves before it and the expression-tree
step started: the fields of display classes become local variables. A display class is a
variable initialized by an empty (or field-copying) constructor call, or a display struct,
whose uses are all field accesses or copies of it. The analysis walks the variables of every
function and validates each candidate's uses. A field initialized from a parameter, `this`
or another display class is replaced by that variable; any other field gets a new variable
in the function that owns the display class. The transform then visits the tree the way
`ILVisitor.Default` does, taking each next sibling before visiting a child. It turns field
accesses into `ldloca` of the new variables and adds those variables to the captured
variables of every function between the use and the declaring one. It also removes
initializers that propagation made redundant, and moves the stores of an object
initializer after it. Last, the initial-value flags of every function are reset.

That last step needed two pieces of the definite assignment analysis the engine lacked.
The managed visitor handles a lambda at its declaration, visiting its body and then
restoring the state before it. It visits local functions in the joined states of their
uses until nothing changes. It also evaluates a `match` instruction into a state for each
outcome. No flow analysis had run after pattern matching or local functions before, so the
port had needed none of the three. With them in place, the reset runs per function, on
that function's own variable list. The managed `RegisterVariable` adds a variable to the
list immediately, so the engine adds the top-level function's pending variables before the
reset. `ValidateConstructor` is now complete, including the one-parameter copy
constructor, and the yield-return decompiler uses the same check. A use of a local function
is static, as `LocalFunctionMethod.IsStatic` always is, so its arguments line up with its
parameters. The settings bit 49 `AggressiveScalarReplacementOfAggregates` was added.

The no-op red run failed 514 of 3,550 equality cases. The port then matched every case,
and the allowlist stays at 69 entries.

### 3.74 HighLevelLoopTransform (item 57, IL prefix 52)

HighLevelLoopTransform turns the plain loops of loop detection into `while`, `do-while` and
`for` loops, which only means a container kind and a block order that the C# backend
reads. A while loop starts with `if (!condition) leave loop` (or has its body nested in
`if (condition)`, which InvertIf flips first). The condition is negated, and the rest of
the entry block moves into a new body block. A for loop is a while loop with an increment
block of simple statements, or one whose body ends in a numeric increment of a variable
the condition reads; that increment then moves into a block of its own. A do-while loop
collects the conditions before the exit at the end of a block into one `if`.

The port reuses ConditionDetection's InvertIf and ExtractBlock and the increment-block
match ported with it. It walks `Descendants` over the body and then the top-level local
functions, in post-order, the order the managed tree has. Two settings bits were added: 50
`ForStatement` and 51 `DoWhileStatement`.

The no-op red run failed 1,526 of 3,550 equality cases. The port matched every case on the
first run, and the allowlist stays at 69 entries.

### 3.75 ReduceNestingTransform (item 58, IL prefix 53)

ReduceNestingTransform undoes the deep nesting that ConditionDetection and switch detection
leave, without changing the IL order of any code. When an `if` or a switch is followed by a
keyword exit (`return`, `break`, `continue`, or a leave whose target is followed by one),
the exit is copied into the branches, so the else block or default case can follow the
statement instead. A statement-count and depth heuristic decides whether that pays off. If
statements with an unreachable end point are also inverted back to IL order, and an empty
`try-finally` around a pinned region (which the C# compiler emits for `fixed`) is removed.

The port reuses InvertIf and the container's control flow graph with its dominator tree,
which the default-case move needs. Two details follow the managed code literally.
`MaxBy(s => s.Labels.Count())` takes the first section with the most label values, and
`LongSet.Count()` gives that number on both sides. `ShouldReduceNesting` takes the
siblings' depth but never reads it. Like the managed transform, the nesting pass visits
only the function body, while the try-finally clean-up walks `Descendants`, local functions
included.

The no-op red run failed 508 of 3,550 equality cases. The port matched every case on the
first run, and the allowlist stays at 69 entries.

### 3.76 RemoveRedundantReturn (item 59, IL prefix 54)

RemoveRedundantReturn works like the exit point detection, but only on `leave`
instructions that leave a whole function returning `void` (or an iterator, where they are
`yield break`). A `return` at the very end of such a function, possibly inside try blocks,
`using`, `lock`, pinned regions or the branches of a final `if`, is removed or turned into
a leave of its own container. It then falls through to the end instead. The port visits
every function the managed `Descendants.OfType<ILFunction>()` does, the nested ones first
and the top-level one last. It takes a nested function's return type from its reader
header (the async return type when there is one) or from the lambda.

The no-op red run failed 909 of 3,550 equality cases. The port matched every case on the
first run, and the allowlist stays at 69 entries.

### 3.77 IntroduceDynamicTypeOnLocals (item 60, IL prefix 55)

The compiler stores `dynamic` values in `object` locals. Their uses show the type: an operand
the dynamic binder sees with its runtime type (its argument info lacks
`UseCompileTimeType`), or an awaited value whose `GetResult` belongs to `dynamic`. Such a
local becomes `dynamic`, and so do the other locals split from the same IL local. The port
needed `DynamicInstruction.GetArgumentInfoOfChild`: no argument info for a conversion or an
event test, one extra info for an invocation with a static target type and for a constructor
call (whose first info describes the type), otherwise the child's own index. The engine keeps
each kind's argument info list as the managed instruction does, so the mapping carries over
directly.

The no-op red run failed 49 of 3,550 equality cases. The port matched every case on the first
run, and the allowlist stays at 69 entries.

### 3.78 IntroduceNativeIntTypeOnLocals (item 61, IL prefix 56)

With `NativeIntegers`, an `IntPtr` or `UIntPtr` local that is used as a native integer (an
operand of arithmetic or `~` computed as `I`, or an argument to an `nint`/`nuint` parameter)
or assigned one (from such arithmetic, a conversion to `I`, or a value of type `nint` or
`nuint`) becomes `nint` or `nuint`. The other locals split from the same IL local follow.
The port reads the underlying result type of a lifted operator as the managed
`UnderlyingResultType` does: the non-lifted result of a binary operator and the lifted
underlying type of `~`.

The no-op red run failed 26 of 3,550 equality cases. The port matched every case on the first
run, and the allowlist stays at 69 entries.

### 3.79 IntroduceScopedModifierOnLocals (item 62, IL prefix 57)

The C# `scoped` modifier is erased from IL and PDBs. This transform recovers it on ref locals
and ref struct locals. It computes the ref-safe context (for ref locals) or safe context (for
ref struct locals) of the declaration initializer and of each later store. Field stores into
the local and calls that can capture into it as a writable ref struct argument count too.
The contexts, narrowest first, are the current method, return-only and the calling method. A
narrower later store means the declaration needed `scoped`. Because a local's context
depends on the contexts of the locals it is assigned from, the computation iterates to a
fixed point. It applies only to methods of modules with the updated escape rules
(`[module: RefSafetyRules(11)]`).

The type system port already carried what the rules ask of signatures: parameter reference
kinds and effective scopes, `[UnscopedRef]`, `allows ref struct` and the `this` scope of
methods. Variables gained the two flags (`IsScoped` and `IsScopedWithoutInitializer`), as
bits of the variable record on the wire. The settings bit 52 `ScopedRef` was added.

The no-op red run failed 17 of 3,550 equality cases. The port matched every case on the first
run, and the allowlist stays at 69 entries.

### 3.80 ExpandNestedConditionals (item 63, IL prefix 58)

ExpressionTransforms collapses `if (c) a = x; else a = y;` into a conditional operator, and an
else-if ladder therefore comes back as one long expression. This transform expands such a
store back into if-else once it nests more than one conditional operator. A conditional that
flows into a position naming a type (a parameter, a return value, a field, a local) is first
extracted into a variable of that type. It runs at the end, after the transforms that need
the single expression, and only in top-level and local functions that are not constructors.

`InferExpectedType` (what a position expects, the counterpart of `InferType`) was ported for
it. The extraction port now registers its temporaries in the function that holds the
extracted instruction, as `ILExtraction` does, and so does TransformDisplayClassUsage; both
go through one `register_variable_in`. `IsInConstructorInitializer` is always false here,
because an expanded function is never a constructor, so the port leaves the check out and
says why.

The no-op red run failed 40 of 3,550 equality cases. The port matched every case on the
first run, and the allowlist stays at 69 entries.

### 3.81 AssignVariableNames (item 64, IL prefix 59)

AssignVariableNames gives every variable a valid, unique C# name. It is the last IL
transform, so IL prefix 59 is the whole pipeline. Most of it is local: names come from the
debug info, from the stores and loads of each variable, from the called method or field, or
from the variable's type. The exception is the root scope. Before naming anything, the
managed transform collects the names it must not reuse: the lower-case members of the
declaring type (inherited members too), the parameters of a primary constructor, and the
lower-case types of the namespace, of the enclosing types and of the using scope. The engine
has no using scope and no resolved namespace, so the host computes these lists
(`RustEngine.RootNames`) and passes them with each read as two length-prefixed lists. The
engine only avoids the member names and also reserves the rest, as the managed code does.

Local functions get their source names here too, so `ILFunction.Name` and the name of a
`LocalFunctionMethod` now travel on the wire, and the host uses them for the function and
for every call that targets it.

The item asked for the `Humanizer` pluralization rules to be ported as data. The IL transform
never pluralizes: only `GenerateForeachVariableName` and `GenerateVariableName` do, and the
C# AST transforms call those in Phase 3. The rules stay unported until then. Two documented
shortcuts approximate the Unicode categories that `CleanUpVariableName` and the identifier
check use.

The no-op red run failed 2,574 of 3,550 equality cases. The last divergences were names
derived from types: a type parameter, an unresolved type and a generic instance must answer
`IType.Name` and `IType.Kind` as the managed type system does (a generic instance takes its
kind from its generic type). After those fixes every case matched.

### 3.82 The analysis pipeline (item 65)

RecordDecompiler and AutoEventDecompiler recognize compiler-generated members by decompiling
their bodies through `CSharpDecompiler.DecompileBodyForAnalysis`. That pipeline runs the
standard IL transforms through the last block transform, then CombineExitsTransform, under
C# 1 settings and without debug info. The shim now serves it through seam 1 too. The seam
block computes the same truncated list, hands its leaf count to the engine as the IL prefix,
and sets a reader flag that tells the engine to finish the body the analysis way. Fallback is
counted like any other seam 1 fallback.

The flag does more than append CombineExitsTransform. The managed analysis reads the method
in a context of its declaring type's parameters only, and that explicit context specializes
the method's signature through a substitution. A substitution turns `T!` (a not-null
annotated class type parameter) into plain `T`, so a parameter of such a type prints
differently from a normal read. The engine reproduces this by reading through an explicit
context of the class type parameters, which its reader already supports for the state
machine decompilers. A generic method would lose its own type parameters in that context, so
the seam sends generic methods to the managed pipeline, and the test asserts this.

The `EarlyILTransforms` pipelines (inside YieldReturnDecompiler, AsyncAwaitDecompiler and
ProxyCallReplacer) needed no work: when the engine serves a method, they run inside the
engine as part of the transforms that call them. They run managed only when the whole method
falls back.

The new `AnalysisBodyEquality` test compares the served analysis body with the managed
pipeline over the record and event fixtures (146 images). With the flag ignored, 6 failed;
with CombineExitsTransform alone, 1 still failed (the `T!` signature); with the explicit
context, all passed. The fork drift test now expects five seam blocks.

Serving the analysis pipeline also removed the last managed IL transforms that ran while the
engine served a method at the full IL prefix. The oracle test
`TransformsKeywordEmitsPerTransformEvents` caught this: with the `Transforms` keyword of
`DecompilerEventSource` on, no `ILTransformExecuted` event fired. It had only passed because the
analysis pipeline still ran managed. Seam 1 now treats per-transform tracing like the Debug
Steps pane: it hands over only the reader, so every transform runs managed and reports its own
event. The other option, timing each transform in the engine and having the host emit the
events from the wire, would keep tracing on the engine path but needs a wire extension. It is
left for when someone profiles the engine through this event source.

### 3.83 Strict by default (item 66)

The test projects that load the shim now run seam 1 strict unless told otherwise. A file
linked into each of them, `RustStrictByDefault.cs`, holds a module initializer that sets
`ILSPY_RUST_STRICT=1` when the variable is unset. A module initializer runs before any test
in the assembly, so no fixture can see the lenient mode by accident. Processes a test starts,
such as the decompiler test runner, inherit the variable. To get the counted fallback back,
for example to measure what the engine still misses, set the variable yourself before the
run (`ILSPY_RUST_STRICT=0`). The initializer leaves an explicit value alone.

No code outside the test projects changed. The IL prefix was already the full pipeline (59),
and no test relied on fallback: the ratchet test counts served and fallen-back methods
itself. The gates ran with the new default: every test project passed, including the UI suite
except one test, `Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab`, which hangs on this
machine on master as well (a font fallback loop in Avalonia) and was excluded.

### 3.84 Semantics (item 67)

`src/semantics/` is a 1:1 port of the managed C# semantics the expression builder will need:
`CSharpConversions` (`conversions.rs`), `Conversion` (`conversion.rs`), `TypeInference`
(`type_inference.rs`), `OverloadResolution` with constraint validation
(`overload_resolution.rs`) and `MethodGroupResolveResult.PerformOverloadResolution`. The
resolve results they consume are a `ResolveResult` enum. Anonymous functions are a `Lambda`
trait, because their body only exists in the caller. `Method` is the `IMethod` view overload
resolution needs: the definition's parameters and type parameters plus the substitution. It
is built from a metadata method or created without metadata (`FakeMethod`). Two methods are
equal when they are the same definition under the same substitution. `Conversions` borrows
the type system mutably and caches type-to-type implicit conversions, like the managed
instance does.

The type system gained what the algorithms ask for. `TypeData` has `Null`, `NoType` and
`Intersection`, each with a wire tag the host decodes to the managed special type. Type
parameters can now exist without metadata (`TypeParamOwner::Synthetic`,
`TypeSystem::new_type_parameter`), with constraints set after creation so they can refer to
each other. Arrays report their base types (`System.Array`, and `IList<T>` and
`IReadOnlyList<T>` for one dimension). `normalize_with` takes the settings of a
`NormalizeTypeVisitor`, so type erasure and the type-merging key are the managed visitors.
The tuple element helper moved from the deconstruction transform into the type system.

The IL transforms used an approximation of the implicit conversions (`csharp_semantics.rs`).
Its identity test compared member-lookup normalizations, which replace type parameters with
dummies. It is now a thin facade over `Conversions`, so the transforms see the same
conversions as the managed transforms, and the IL prefix tests still pass.

The gate is the four managed suites, ported as cargo tests by C#-type-syntax helpers in
`tests/common/csharp.rs` (`ty(ts, "List<string>[]")`): `ConversionTests` (129 tests),
`ExplicitConversionTest` (66), `TypeInferenceTests` (65) and `OverloadResolutionTests` (21).
The managed suites are larger than the counts in the plan, and every test was ported. 273
pass. The 8 ignored ones are `[Ignore]`d in the managed suites with the same reasons. Two
port bugs showed up as red tests and were fixed. First, a default literal was not a
compile-time constant (the managed `DefaultLiteralResolveResult` is a null constant). Second,
the all-results inference fixed to one candidate instead of their intersection, which is
why `Intersection` exists.

The plan also lists member lookup and the parts of `CSharpResolver` the expression builder
uses. Which parts those are is only known once the builder exists, and no gate test covers
them. They come with item 68, where the first caller needs them.

### 3.85 Seam 2 at body level (item 68a)

Item 68 as planned was one request-level seam: the whole syntax tree from the engine, gated by
G-ast(0). About 30,000 managed lines (the three builders, the syntax tree, the type system AST
builder, the resolver's operators) would have had to be exact before the first green gate.
The human took three decisions instead:

- Seam 2 grows at body level first. The engine translates each method body after seam 1;
  declarations stay managed. A body with a construct the engine does not translate falls back
  to the managed statement builder, and the fallback is counted. Request-level seam 2 comes
  last (68z).
- Types and constants become syntax in the materializer, through the managed
  `TypeSystemAstBuilder`, which chooses the shortest name with the resolver and using scope as
  the original does. Its port into the engine comes with 68z, before the engine-only end state.
- Item 68 lands as several commits, one construct group each, every one gated.

The engine side lives in `src/csharp/`: an arena of syntax nodes (`ast.rs`) with the
annotations later steps read (the IL instructions, the resolve result and the implicit
return), and `statement_builder.rs`, `expression_builder.rs` and `translated_expression.rs`
mirroring the managed builders. `convert_body` answers with the block or with the reason it
does not translate the body. Seam 1 asks for the translation only when the engine ran the
whole IL pipeline (not for Debug Steps or per-transform tracing, and not without member
bodies). The translation travels as a trailer after each function in the IL wire, now
version 2 (`wire/ast.rs`). It shares the wire's string and type tables. It refers to
instructions by their completion-order index, which the IL materializer already keeps for
shared nodes, so every annotation points at the same `ILInstruction` objects the managed
transforms see. Variables refer to the function's variable list the same way. Constants are
sent as the constant, and the host expands them with `ExpressionBuilder.ConvertConstantValue`.
The null, no-type and intersection types got wire tags for later resolve results.

On the host, `IlAstMaterializer` decodes the trailer into `RustBodyNode`s. `RustSeam2.ServeBody`
turns them into syntax with the managed `ExpressionBuilder` of the statement builder the
decompiler created. The annotations go on in the order the managed builder adds them. The
seam block in the fork jumps past the managed `ConvertAsBlock` with a `goto` to a label in a
second seam block. An `if` without braces around the original line would have been re-indented
by the format hook, which would break the drift test. Seam 2 fallback is not affected by
`ILSPY_RUST_STRICT` until item 85.

68a translates the function body container (one block without labels), `return`, the
implicit final return, `nop`, `ldc.i4` (adjusted to an int, uint or bool type hint), `ldstr`,
`ldnull` and `ldloc`. It translates only the identity case of `ConvertTo`. On the ratchet
corpus that serves 3.5 % of the bodies; `rust/port-status.json` records it as
`bodies_served_pct`. The ratchet test prints the fallback reasons by frequency, which sets
the order of the next construct groups: `ldobj` (243), calls as statements (170), `stobj`
(169), `if` (164) and `stloc` (142) lead. G-ast(0) (`AstPrefixEqualityTests`, 3261 images)
decompiles every fixture image and ICSharpCode.Decompiler.dll with and without the engine's
body translations and compares the text. It passes. The red step was the ratchet test
failing with no body served through seam 2.

### 3.86 Field access through the host's resolver (item 68b)

Every member reference of `ExpressionBuilder` goes through `Disambiguator`. It finds the
shortest spelling that still resolves back to the member: unqualified, qualified with the
target, then with the target cast, and for calls also with type arguments and argument
casts. Each candidate is re-resolved with the managed `CSharpResolver`, which needs member
lookup, the using scope and overload resolution. The human decided that the engine ports
the `Disambiguator`'s control flow and asks the host for the re-resolution, as the IL steps
already ask it about extension methods. Porting `CSharpResolver`'s name lookup and
`MemberLookup` into the engine is a later, separately gated step before the engine-only end
state.

The question goes over the same resolver callback of `ilspy_session_read_function` (query
kinds 2 and 3 of `wire::ilast::field_reference_query`): the field, and for a qualified
spelling the target as `this` or `base` (with its non-virtual flag), a type name or another
expression, each with its type. The host answers with the resolver the statement builder's
`ExpressionBuilder` creates: the compilation, the using scope of the current type's
namespace, the current type and the current member. That is a second resolver on
`RustResolver`, built by seam 1 from the decompilation context, because the IL steps'
resolver has no current type. `MemberLookup` only checks whether the target is a
`ThisResolveResult`, so the host gives it a placeholder `this` variable.

68b translates `ldobj` and `stobj` through a managed reference produced by `ldflda` or
`ldsflda`, the field reference itself (`TranslateTarget` for a reference-type target,
`base` and a static field's type, `RequiresQualifier` with the new settings bit 53
`AlwaysQualifyMemberReferences`), assignments, `stloc` to variables that are not stack
slots (the managed builder retypes stack slots as a side effect), and expression
statements. Annotations now record whether the IL instructions or the resolve result came
first, because the managed builders add them in both orders. The `DelayExceptions` flag of
`ldflda` does not change the translation: the reader sets it for every `ldfld`. Backing
fields of automatic properties and events, the C# 14 `field` keyword, value-type targets and
casts of the target fall back for now. The first G-ast(0) run of this group failed 61 images:
the backing-field guard knew only the C# name `<Name>k__BackingField`, not the VB `_Name` of
a property `Name`, which `PatternStatementTransform` recognizes as well, so VB automatic
properties lost their `{ get; set; }` form. The guard now covers both patterns.

The share of bodies seam 2 serves rose from 3.5 % to 8.0 %. The largest fallback groups are
now calls (329 `call`, 100 `callvirt`), backing fields (301) and `if` statements (169).

### 3.87 Backing fields (item 68c)

`ConvertField` treats a field differently depending on the declarations around the body.
The backing field of an automatic event is written as the event, unless the body is one of
that event's own accessors and only when `AutoEventDecompiler` judges the event automatic,
which runs the analysis pipeline over its accessors. The backing field of the property
whose accessor is decompiled becomes the C# 14 `field` keyword. The backing field of another
automatic property takes its qualifier from the property. A field named `field` inside a
property accessor is always qualified. All of these read the decompilation context's current
member and the host's type system.

The engine asks the host which branch applies (resolver query kind 4: the field, and whether
its target is the `this` instance or absent). The host answers with the same checks
`ConvertField` makes, as an integer the engine decodes into `FieldKind`: an ordinary field,
the keyword, a field named `field` in an accessor, or the backing field of the automatic
property or event at a metadata row. The engine then builds the reference itself: the event
from its Event row (name, static accessors, type), a `MemberResolveResult` without target
for the keyword, and the property's name and staticness for `RequiresQualifier`. The host
side's inputs (resolver, decompilation context, decompile run and type system) travel
together as `RustBodyContext`, which seam 1 builds. This replaces the name guards of 68b,
which only kept such bodies out.

Automatic-property accessors are among the most common bodies, so the share seam 2 serves
rose from 8.0 % to 28.3 %. Calls are now by far the largest fallback group (336 `call`,
100 `callvirt`), followed by `if` statements (169).

### 3.88 Implicit conversions and constant folding (item 68d)

`TranslatedExpression.ConvertTo` now follows the managed order up to the cast. It returns the
expression unchanged for equal types (up to nullability and tuple names), for a `void` or
unknown target, and, with `allowImplicitConversion`, when the ported `CSharpConversions`
finds an implicit conversion. Before that it asks `ResolveCast`, which is now in the engine
as far as `ConvertTo` reads it. The explicit conversion is classified so a user-defined one
stays a conversion, and a constant is folded into the target type (an `int` returned from a
`long` method becomes `0L`, `null` returned as `string` a string constant). The folded
constant goes to the host as a `ConstantValue` node, which `ConvertConstantValue` expands as
before, carrying the original expression's IL instructions.

The folding uses `CSharpPrimitiveCast` without overflow checking for integral, char and bool
constants. Floating point and decimal constants, native integers, and the special cases
before the cast (bool to and from integers, `IntPtr`, pointers, references) fall back, as
does every conversion that needs a cast. The share rose from 28.3 % to 29.9 %; calls remain
the largest group (338 `call`, 100 `callvirt`).

### 3.89 Method calls (item 68e)

`call` and `callvirt` now translate in the engine for the plain case of `CallBuilder.Build`:
a non-generic method with by-value arguments and no `params` or optional parameters,
written as `target.Name(arguments)` or `Name(arguments)`. Every special form that
`CallBuilder` writes with syntax of its own (delegates, tuples, string concatenation and
interpolation, accessors, operators, extension methods, span helpers), generic methods,
tail and constrained calls fall back and are counted under their own reason.

The managed builder decides how much of a call to spell out with `Disambiguator.ForCall`:
it resolves the current spelling back and escalates (drop the names of bool literals, cast
the arguments, require the target, cast the target, ...) until the name resolves to the
method called. The engine runs the same steps. As decided for calls ("engine resolves, host
looks up"), the host only answers the name lookup: a new resolver query returns the method
group `ResolveSimpleName` or `MemberLookup` finds, as metadata handles grouped by declaring
type. The engine specializes the lists by the lookup type's base types and runs its ported
overload resolution (item 67) over them; a virtual call also accepts an override of the
method found. A step that would insert a cast falls back, since casts are not translated
yet. Answers of variable size made the resolver callback take an answer buffer and its
capacity; the engine asks again with a larger buffer when the answer does not fit.

On the host, the invocation's `CSharpInvocationResolveResult` is rebuilt from the target's
resolve result and the method the engine names, with the arguments' resolve results (a
named argument contributes its expression's). The share rose from 29.9 % to 38.5 %. The
largest groups now are `if` statements (183), accessor calls (92) and `newobj` (60).

### 3.90 Property access (item 68f)

An accessor call is written as a member access (`HandleAccessorCall`) when it is the getter
or setter of a property without parameters; the engine now translates those as
`target.Name` or `Name`, and a setter as the assignment of its value to that. The value is
translated and converted like any argument, then split off, since it is not part of the
member reference. Indexers, parameterized properties, event accessors and the properties of
`Range` and `Index` (which `HandleRangeConstruction` writes as range syntax first) fall back.

Whether the property is written with its target follows `Disambiguator.ForAccessor`, which
for an access without arguments only qualifies and then casts the target. Like a field, the
check is a name lookup, not an overload resolution, so the host answers it in one question:
it looks the property's name up unqualified or on the target and applies
`IsAppropriateCallTarget`. The answer distinguishes the accessor's own property from another
member the call may still write (the virtual declaration an override hides); the second
falls back, because the managed resolve result names the member found.

The getter's `MemberResolveResult` is rebuilt on the host from the accessor's
`AccessorOwner`. A setter's assignment carries a `TypeResolveResult` of the property type
instead of the `OperatorResolveResult` of a plain assignment. The share rose from 38.5 % to
40.7 %; `if` statements (188) and `newobj` (62) are now the largest groups.

### 3.91 If statements and logical not (item 68g)

`VisitIfInstruction` is short: the condition through `TranslateCondition`, the branches
through `Convert`, no `else` for a `nop`. The branches are usually blocks without a
container, which `VisitBlock` turns into a block statement of their instructions; the engine
now does both. The block falls back when an instruction is a store followed by a `while`
container, the shape `TransformToForeachWithoutDispose` rewrites, since loops are not
translated yet.

`TranslateCondition` translates with a `bool` type hint and then calls `ConvertToBoolean`,
which leaves a `bool` alone and otherwise builds a comparison with zero; only the first case
is taken. Most conditions are comparisons (`comp`), and the cheapest of those need no
resolver: `TranslateCeq` writes the comparison of a `bool` with 0 or 1 as the `bool` or its
negation. That is also how the ILAst spells logical not. The negation is `!` with an
`OperatorResolveResult` whose operand the host takes from the child, like an assignment's,
so the assignment resolve result became a general operator one. When the comparison is
dropped, the `comp` instruction itself is annotated nowhere, as in the managed builder.

Every other comparison falls back, because it needs `ResolveBinaryOperator`, which is not in
the engine yet. The share rose from 40.7 % to 41.4 %; comparisons (99) are now the largest
group, ahead of `if` instructions in expressions (82) and `newobj` (63).

### 3.92 Equality comparisons (item 68h)

`TranslateCeq` asks `CSharpResolver.ResolveBinaryOperator` for the operator and keeps its
`OperatorResolveResult` when it is a built-in one whose first operand has the comparison's
stack type; otherwise it casts the operands and asks again. The full operator resolution
(about 700 lines of the resolver plus the 1,175 lines of `CSharpOperators`) is not in the
engine. This step ports only the part of it that ends in a built-in `==` or `!=` without
converting either operand, because only then are the resolve result's operands the
operands' own resolve results, which the host can take from the children:

- both operands of the same primitive type of at least `int` (`int`, `uint`, `long`,
  `ulong`, `float`, `double`), which binary numeric promotion leaves alone, or both `bool`;
- both `object`, the reference comparison's parameter type;
- a reference, interface or array compared with the null literal.

Before it asks the resolver, `TranslateCeq` writes a string or delegate compared with `null`
as a built-in comparison directly; the engine does the same. Everything else falls back
under its own reason: user-defined operators (checked as any method of that name on the
operand types or their base types, which is stricter than overload resolution), enums,
nullable and lifted comparisons, pointers, native integers, type parameters, two
constants (which the resolver folds), and operands that numeric promotion or the reference
comparison would convert. The constant on one side is first adjusted to the other side's
type, as `TryUniteEqualityOperandType` does.

The share rose from 41.4 % to 43.0 %. Comparisons are no longer a large group;
`if` instructions in expressions (89), `newobj` (68) and `throw` (60) lead now.

### 3.93 Throw statements and object creation (item 68i)

`VisitThrow` translates the exception with `Exception` as type hint and does not convert it;
a rethrow is a `throw;` without expression. Most thrown exceptions are created on the spot,
so `newobj` came in the same step.

`CallBuilder.Build` for `newobj` first takes the forms with syntax of their own: delegate
construction, tuple construction, stackalloc spans, ranges and indices, anonymous types, and
in some type system modes the native integer an `IntPtr` constructor creates. The engine
falls back for all of them, recognizing them conservatively by the declaring type (an
anonymous type by its name alone, which also catches the `VB$AnonymousType` classes of VB;
the first gate run missed those and lost VB's `new { ... }` syntax). An
ordinary constructor call shares the argument list with method calls, which became a helper
of its own, and is checked with `Disambiguator.ForConstructorCall`: drop the names of bool
literals, then cast the arguments, while overload resolution does not pick the constructor
called. The candidates are the declaring type's constructors that the decompiled method can
access, which needs `MemberLookup.IsAccessible`; following the decision for calls (the engine
resolves, the host looks up), the host lists them in a new resolver question, in the method
group's encoding, and the engine runs its overload resolution over them. A constructor
without metadata row, such as the implicit one of a struct, makes the host answer with
nothing and the call fall back.

The host builds the `ObjectCreateExpression` with `ConvertType` of the declaring type and a
`CSharpInvocationResolveResult` without target. The share rose from 43.0 % to 47.8 %; `if`
instructions in expressions (91), generic method calls (61) and string concatenation (47)
lead now.

### 3.94 Logic operators and conditional expressions (item 68j)

In an expression, the ILAst `if` stands for three C# forms. `if (a) b else 0` is `a && b`
and `if (a) 1 else b` is `a || b`, provided `b` is a `bool` (the ILAst operators may yield
another value when `b` runs) or the whole is only used as a condition
(`IsInConditionSlot`, which the engine computes from the node's parent and slot). The
operator gets a plain `ResolveResult` of `bool`, the one resolve result kind the wire format
did not have yet. A non-bool right operand in a condition slot needs `ConvertToBoolean`'s
comparison with zero and falls back.

Everything else becomes the conditional operator, after the constant branch is adjusted to
the other branch's type. `ResolveConditional` converts the branches to their common type;
the engine takes only branches that already have the same type, where neither conversion
does anything and the result is an `OperatorResolveResult` of the conditional over the
three operands' own resolve results. Branches of different types, `null`, `ref` and
`dynamic` branches, and three constants (which the resolver folds) fall back.

The share rose from 47.8 % to 48.3 %. Most of the 91 `if` instructions only moved the reason
further in: pattern matching (`match`, 26) and relational comparisons (20) now show up,
behind generic method calls (64) and string concatenation (50).

### 3.95 Generic method calls (item 68k)

For a generic method, `Disambiguator.ForCall` first asks whether type inference from the
arguments reaches the method definition's type parameters at all
(`WriteTypeArgumentsInferenceCannotReach`, with the expression builder's improved inference
algorithm), and writes the type arguments from the start if not; a failed inference during
the probes is repaired the same way. The engine passes the written type arguments to its
overload resolution, and the host's method group lookup now receives them too, since
`MemberLookup` filters by them. The written name carries them as `AstType`s the host
converts, through two new node kinds. For the calls the engine translates, inference does
reach the type arguments, so they are written only as the late `RequireTypeArguments` step.

The gate runs found three differences, and each is now handled. First, where inference does
not reach, the engine's result can differ from the managed one: the managed type system gives
the base types a class inherits through an interface implementation (`DepthFirstAstVisitor<T>
: IAstVisitor<T>`) the nullability annotations of that implementation, the engine's
`all_base_types` does not, so inferring `T` for an annotated `IAstVisitor<T>` parameter from
such a class fails in the engine only. Until the base types carry those annotations, a call
whose type arguments inference does not reach falls back (4 bodies of the corpus). Second, the
host's lookup can find the override of the method called (a virtual generic method called
through `this` in the class that overrides it); the managed builder accepts it through the
base member chain and names it in the resolve result, so the engine now falls back there
instead of escalating to a longer spelling, which it did for non-generic calls as well. Third,
the ported constraint check rejected a type parameter of the calling method for a `struct`
constraint (calling a method on `Nullable<T>` from a method with `T : struct`):
`AbstractTypeParameter.GetConstructors` yields a dummy parameterless constructor for such a
parameter, which the engine's constructor lookup does not; `validate_constraints` now asks the
type parameter's flags, with a test.

Two more cases fall back by design. Type arguments that may contain anonymous types are left
out or pinned by the managed builder; the engine recognizes such types by name. And the managed
resolve result names the method overload resolution found, whose type arguments come from
inference and can differ from the call's in tuple names or nullability; the engine only accepts
a found method whose type arguments are exactly the call's.

The share stayed at 48.3 %: the bodies with generic calls (64) now fall back further in,
mostly on extension method calls (49) and by-reference arguments (23), which the next steps
take on.

### 3.96 Extension method calls (item 68l)

The managed call builder first disambiguates an extension method call as the static call it
is in IL, then `TryUseExtensionMethodSyntax` rewrites it as `first.Name(rest)` when the first
argument is written first and positionally, using declarations are on (otherwise the
method's namespace is not imported and no member access can find it), and
`CSharpResolver.CanTransformToExtensionMethodCall` finds the method again from the first
argument with the remaining arguments and names. The engine does the same, and the last
check is a new resolver question answered by the expression builder's resolver: it is a
member lookup that includes the extension methods in scope, which depends on the using
scope the host has. The arguments go over as expressions or constants, since constants
convert more freely (a literal `0` to an enum). A null literal or `ref` first argument,
which needs a cast or the `ref` extension rules, falls back. `UsingDeclarations` became
settings bit 55.

The resolve result keeps the first argument as target and marks the extension method
invocation; its arguments are the rest. The share rose from 48.3 % to 48.6 %. Many of these
calls are LINQ with lambdas, so `the instruction Function` (36) now shows up, behind string
concatenation (50) and `??` (47).

### 3.97 string.Concat (item 68m)

Item 68e treated every `string.Concat` and `string.Format` call as a form with syntax of its
own. For `Concat` that was too broad: `CallBuilder.Build` only rewrites the span-based
overload whose parameters are all `ReadOnlySpan<char>` (into `+` over the original operands);
every other `Concat` is an ordinary call, which the AST transform
`ReplaceMethodCallsWithOperators` turns into `+` later, on the managed side. The engine now
falls back only for the span-based overload. `Format` stays apart, since it may become an
interpolated string. The share rose from 48.6 % to 49.8 %; `??` (47), member access on value
types (45) and lambdas (43) lead now.

### 3.98 Member access on value types (item 68n)

A member of a value type is reached through a managed reference: the address of a local
(`ldloca`), of a field, or a by-reference variable such as `this` in a struct method.
`TranslateTarget` translates the target with a by-reference type hint, requires a reference
of the member's declaring type (casting otherwise, issue #1333), and writes `(ref x).member`
as `x.member`. The engine now does the same, and translates the two missing sources of
`ref` expressions: `ldloca` (the name, with the instruction on the name and the `ref` around
it) and the load of a by-reference variable (the name with the element type's resolve
result, inside a `ref` with a `ByReferenceResolveResult`). Storing to a by-reference
variable is a ref reassignment and still falls back, as do targets that need a reference
conversion and pointers.

Taking the `ref` away is `UnwrapChild`, which moves the IL instructions of the dropped node
onto the child. The engine's version did not, which did not matter while no `ref` carried
an instruction; a by-reference variable's load puts its instruction on the `ref`, so it does
now. The share rose from 49.8 % to 50.9 %, past half of the corpus bodies.

The first gate run found a difference this step only exposed: a `null` argument converted
to a reference parameter type (`Enumerable.Repeat<string>(null, n)`). The engine modelled
the converted constant's resolve result as a `string` constant, while the managed
`ConvertConstantValue` keeps the null literal's own resolve result (`NullType`) when no cast
goes around it. Type inference then found `T` in the engine only, and the engine left out a
type argument the managed builder writes. The folded constant now gets the null literal's
type there, as the host's node has.

## 4. Alternatives evaluated

| Option | Pros | Cons | Outcome |
|---|---|---|---|
| Full public-API proxy over FFI | Literal reading of the brief | about 760 types; identity and mutability; callbacks both ways; SRM handles in signatures; internals visible to tests; chatty FFI | Rejected |
| Engine returns text or a token stream | Smallest FFI payload; no materializer | Every entry point returns `SyntaxTree`; `TextTokenWriter`, bookmarks, PDB writer, caller transforms all walk the tree | Rejected |
| Type-forwarding shim over the original | No duplicated code | CS0433 as soon as the original and the shim meet on one reference path (ilspycmd) | Rejected |
| Link-compiled shim with one forked file | No copies, no edits, full surface, internals, red test on drift | Same assembly identity as the original, so the two must never share a reference path or output directory | **Chosen** |
| Twin projects for ILSpyX and BamlDecompiler | Guardrail intact, no process loads both | Two more csproj files; ilspycmd never sees the engine | Rejected by the human in favor of relaxing the guardrail |
| Big-bang port, flip at the end | Simpler shim, no intermediate materializers | .NET suite unchanged for months; progress visible only in Rust test counts | Rejected |
| Incremental two-seam fallback with ratchet | Suite green at every commit; per-transform gates; honest metric | Materializers needed early; one forked file to keep in sync | **Chosen** |
| Pre-1.0 Rust metadata crates | Less code | No signature semantics, generics, attributes, PDB; unmaintained risk | Rejected, own reader |
| Pre-decode metadata in C#, ship to Rust | Smaller Rust scope | Serializer larger than a reader; lazy cross-assembly access impossible | Rejected |
| Portable PDB reader in Rust | Self-contained engine | about 1k lines for nothing the UI cannot already provide; Windows PDBs via Mono.Cecil would be lost | Rejected, callback |
| Binding generators (csbindgen, interoptopus, uniffi) | Less typing | Toolchain for nine functions; uniffi brings its own runtime | Rejected |
| FlatBuffers or JSON wire format | Tooling | Third toolchain on CI; JSON too large for whole-module ASTs | Rejected, hand-rolled binary |
| net10 shim | Modern target | Unguarded polyfills in the sources become CS0436 under warnings-as-errors | Rejected, netstandard2.0 |

## 5. Plan and sizes

The checklist in `RustPort_AgentInstructions.md` has 89 items in five phases. Sizes are
relative (S under a day of agent work, M a few days, L about a week, the IL transform phase
as a whole is multi-week).

1. **Scaffolding** (items 1-2): shim, reference switch, Rust workspace, FFI handshake. The
   first commit alone proves that link-compiling and the reference switch keep the whole
   repository green on managed code.
2. **Metadata and type system** (3-4): own reader, interned type system, ported unit tests.
3. **ILAst and seam 1** (5-66): IL reader, wire v1, materializer, seam at `N = 0`, then one
   transform per commit in pipeline order with ILAst prefix equality plus the text oracle as
   gate, a human review of accepted divergences after `ConditionDetection`, strict mode at
   the end.
4. **Semantics, AST and seam 2** (67-85): resolver and conversions, AST builders, wire v2,
   materializer, seam at `K = 0`, one AST transform per commit, strict mode at the end.
5. **UI, CI, performance** (86-89): UI smoke pass, CI and packaging, measurements, and a
   human decision on the optional tail (disassembler, output visitor, Rust PDB reader and
   resolver, project decompiler).

Things that stay managed on purpose: the browsing type system and metadata layer, output
visitor and token writers, disassembler, PDB reader and writer, project and solution writers,
documentation, Debug Steps, BamlDecompiler's connection-id pass, `AddXmlDocumentationTransform`.

## 6. Bonus: performance comparison methodology

Goal: CPU and memory of `ICSharpCode.Decompiler` versus `ICSharpCode.Decompiler.Rust` on the
same inputs, with the engine's own cost separated from marshaling and materialization.

- **Two-build harness.** `TestTools/decompdiff.cs` already loads two decompiler builds in
  separate `AssemblyLoadContext`s and drives `CSharpDecompiler(string, DecompilerSettings)`
  plus `DecompileTypeAsString` by full type name, so it works with `--new
  <path>\ICSharpCode.Decompiler.Rust.dll` as is (always pass the dll path; its checkout mode
  only knows the original csproj). Native library probing from an ALC-loaded assembly is
  unverified; if default probing fails, add a `LoadUnmanagedDll` override in its load context.
  Extend it with `--perf` (per-type `Stopwatch`, `GC.GetTotalAllocatedBytes(precise: true)`
  delta, `Process.PeakWorkingSet64` per side, written to `perf.json` and a column in the
  report) and `--side old|new` so each side runs in its own process (JIT warm-up and GC
  interference otherwise poison the numbers). Warm up on the first assembly and discard it,
  run with `-j 1` for timing.
- **Rust heap.** Managed allocation counters do not see the Rust heap, so peak RSS is the
  memory number that matters, plus a counting global allocator in the ffi crate exposed via
  `ilspy_stats` (bytes allocated, peak live).
- **Marshaling cost.** The response header carries `rust_elapsed_ns`; the shim measures wall
  clock around `ilspy_decompile` plus materialization and reports the difference as marshaling
  and materialization, with bytes per request. If that exceeds roughly 20 percent of end to
  end, it is the first thing to optimize (string table reuse, pre-sized node lists).
- **Allocation attribution.** `TestTools/allocview.cs` works unchanged (its area table keys on
  the `ICSharpCode.Decompiler.*` namespace prefixes the shim keeps); add a prefix for the
  shim's interop namespace so materializer allocations get their own area.
- **Engine-side.** `criterion` benches under `rust/ilspy-decompiler/benches/` per stage
  (metadata load, type system warm-up, IL read, each transform, expression building,
  serialization) over the three inputs the AOT gate uses (`ICSharpCode.Decompiler.dll`,
  `System.Reflection.Metadata.dll` 9.0.0, one net10 assembly); baselines committed as numbers
  in this document, not as files.
- **Report.** CPU as P50, P95 and total per corpus; memory as peak RSS and Rust peak live;
  three runs, median; inputs: the three reference assemblies and the top-200 NuGet corpus from
  `TestTools/nuget-top.ps1`.

Results (to be filled in by item 88):

| Input | Managed CPU total | Engine CPU total | Marshaling share | Managed peak RSS | Engine peak RSS | Rust peak live |
|---|---|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | | | | | | |
| System.Reflection.Metadata.dll | | | | | | |
| net10 reference assembly | | | | | | |
| top-200 NuGet corpus | | | | | | |

## 7. Bonus: CI so the main build action passes

Minimal changes to `.github/workflows/build-ilspy.yml` (and nothing else in CI):

- `rust-toolchain.toml` at the repository root, not under `rust/`: rustup resolves it upward
  from the working directory, and the MSBuild `Exec` runs from the shim's directory. Pinned
  stable channel, components `rustfmt` and `clippy`. Targets are added per job.
- After `setup-dotnet` in every job: `actions-rust-lang/setup-rust-toolchain@v1` (reads the
  toolchain file, wraps `rust-cache` keyed on `rust/Cargo.lock`, `cache-workspaces: rust`)
  with targets: Windows `x86_64-pc-windows-msvc` and `aarch64-pc-windows-msvc` (ARM64 MSVC
  linking on the `windows-2025-vs2026` image is unverified; if absent, the VS ARM64 build tools
  component is needed for the win-arm64 cross publish); Linux `x86_64-unknown-linux-gnu` and
  `aarch64-unknown-linux-gnu` with `gcc-aarch64-linux-gnu` installed and
  `rust/.cargo/config.toml` naming `aarch64-linux-gnu-gcc` as linker; macOS
  `aarch64-apple-darwin` (host). Then `cargo fmt --check`, `cargo clippy --all-targets --
  -D warnings`, `cargo test` in `rust/` before `dotnet build`, so Rust failures fail fast.
- MSBuild: the shim's `BuildRustEngine` target maps `$(RuntimeIdentifier)` to a triple (empty
  means host), builds with profile `release` for Debug and `ship` for Release, copies the
  cdylib to `$(OutDir)` and registers it as `None` with `CopyToOutputDirectory` and
  `CopyToPublishDirectory`, so `publish.ps1`'s `dotnet publish -r <rid>` directories carry it
  and the zip, WiX harvest (`ILSpy.Installer/setup.cs` harvests `*.dll` from the publish
  folder), deb, rpm, AppImage and the macOS bundle target pick it up unchanged. Whether the RID
  global property reaches the shim through a `ProjectReference` on `dotnet publish -r` is
  unverified; test locally with `-r win-arm64` during item 87. The macOS dylib may need code
  signing alongside the app bundle.
- `BuildTools/ILSpy-x64.msi.filelist` lines 43-45 (`ICSharpCode.BamlDecompiler.dll`,
  `ICSharpCode.Decompiler.dll`, `ICSharpCode.ILSpyX.dll`) stay as they are (the shim keeps the
  assembly name, lecture 3.1) and `ilspy_decompiler_ffi.dll` joins the list. Regenerate with
  `BuildTools/create-filelists.ps1`; the Debug job gates on `git diff --exit-code`. The VSIX
  file list may change too because the VSIX embeds the ILSpy distribution; unverified, check
  the first CI run.
- `.gitignore` gets `rust/target/`; `rust/Cargo.lock` is committed (this builds a binary
  artifact). The unanchored `bin/` pattern would hide a Rust `src/bin/` directory; the engine
  has none.
- The pre-commit hook runs `dotnet format whitespace` over `ILSpy.slnx`; it is C#-only and
  unaffected, and it covers the shim's own `.cs` files because the shim is in the solution.
- `dotnet test --solution ILSpy.slnx` picks the shim test project up automatically; the Linux
  job's explicit `dotnet test --project` lines need one more line for it.
- Because the human relaxed the ilspycmd guardrail, the `IlspyCmdRidPackages` job and the
  `Pack NuGets` step (ILSpyX and BamlDecompiler now depend on the shim package) must carry the
  native assets under `runtimes/<rid>/native/`. The `DecompilerAotGate` stays on the original
  and is unchanged. The PowerShell cmdlets stay on the original and are unchanged.

## 8. Risks

- **Materializer fidelity is load-bearing.** The whole incremental plan rests on the ILAst and
  AST materializers reaching full annotation fidelity. If that stalls, member-level mixing
  collapses to request-level fallback and the ratchet stops moving. Mitigation: the equality
  harnesses (`WriteTo` for ILAst, end-to-end output for AST) are built before any transform is
  ported and run over every fixture method.
- **Allowed divergence can hide drift.** The allowlist makes algorithm changes legal; the human
  review after `ConditionDetection` (item 32) is there to keep it from becoming a dumping
  ground.
- **One forked file.** `CSharpDecompiler.cs` in the shim must track upstream; the drift test
  turns that into a visible failure, but someone has to merge.
- **Indistinguishable twins.** The shim and the original share assembly name, key and version
  (lecture 3.1). A project that ends up with both on its reference path gets whichever MSBuild
  copies last, silently. Only the `PrivateAssets="all"` build-ordering reference keeps them apart.
- **Known baseline hang (found in item 1).** `ILSpy.Tests`
  `SwitchListAndDeleteAllTests.Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab` hangs
  on this Windows machine at the plan's base commit 11fa859ef as well, so it is not caused by
  the port. G-ui runs exclude `FullyQualifiedName~SwitchListAndDeleteAllTests` until it is
  fixed upstream; re-check at every G-ui gate whether the exclusion is still needed.
- **Unverified build facts**: ARM64 MSVC linker on the CI image, RID flow into the shim on
  `dotnet publish -r`, native probing for ALC-loaded assemblies in `decompdiff`, the VSIX file
  list, macOS code signing of the dylib.
- **Scale.** About 140k of the 175k lines describe behavior that is in scope for Rust (IL, control flow, transforms,
  builders, syntax, resolver, semantics, metadata, type system). The per-transform cadence is
  what keeps that reviewable.

## 9. Sources

- Repository facts: `ICSharpCode.Decompiler/CSharp/CSharpDecompiler.cs` (pipelines at lines
  89 and 271, seams at 2440 and 2457), `ICSharpCode.Decompiler/Output/TextTokenWriter.cs`,
  `ICSharpCode.Decompiler/IL/Instructions/ILInstruction.cs`,
  `ICSharpCode.Decompiler.Tests/Helpers/Tester.cs`,
  `ICSharpCode.ILSpyCmd/ICSharpCode.ILSpyCmd.csproj`, `.github/workflows/build-ilspy.yml`,
  `BuildTools/ILSpy-x64.msi.filelist`, `TestTools/README.md`,
  `ICSharpCode.Decompiler.AotTest/README.md`.
- Rust ECMA-335 crates: https://docs.rs/dotnetdll/latest/dotnetdll/ ,
  https://github.com/coconutbird/clrmeta , https://crates.io/crates/windows-metadata
- C# binding generators: https://docs.rs/crate/interoptopus_csharp/latest ,
  https://github.com/mozilla/cbindgen/issues/846 (csbindgen discussion)
- Native NuGet packaging pattern (`runtimes/<rid>/native` from a cargo matrix build):
  https://docs.rs/crate/rustwright-core/latest/source/.github/workflows/release-nuget.yml ,
  https://www.nuget.org/packages/ManifoldRust
- Existing Rust wrapper that hosts the managed decompiler rather than porting it:
  https://www.skills.sh/akiselev/ghidra-cli/ilspy-cli
