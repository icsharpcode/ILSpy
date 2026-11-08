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
