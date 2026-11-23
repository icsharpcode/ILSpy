# What the Rust port taught us about the managed decompiler

Porting `ICSharpCode.Decompiler` to Rust meant reading most of the IL pipeline, the C# AST
builders, the resolver and the AST transforms line by line, and comparing the port against the
managed code on thousands of assemblies (branch `christophwille/rustafari`, 2026-11-07 to
2026-11-23). Along the way the port found places where the managed code is wrong, inconsistent,
order-dependent, or hard to test. This document collects them for the maintainers of the C#
code. None of them was changed: the port's rules forbade edits to `ICSharpCode.Decompiler/`, and
where the managed behavior decides the output, the Rust engine reproduces it on purpose.

How to read the evidence tags:

- **[verified]**: the C# code was read in the current tree and does what the entry says.
- **[observed]**: the behavior was seen while running the code (a test, a crash, a decompile).
- **[reported]**: the entry comes from the port's notes (lecture section or commit) and the C#
  was not reread for this document.

References: lecture sections are in `RustPort_HumanLecture.md` (for example "L 3.49"), commits
are on the branch. Line numbers are from the current tree. Nothing here was fixed or
reproduced by changing managed code; "verified" means read, not run with a patch.

Contents:

- A. Bugs and wrong output
- B. Quirks the port reproduces on purpose
- C. Nondeterminism and ordering dependencies
- D. Performance
- E. Tests and test infrastructure
- F. API and design
- G. Build, trimming and Native AOT
- Suggested order of work

---

## A. Bugs and wrong output

### A1. `PortablePdbWriter` asserts and kills the process when a body was replaced by error text

[verified code, observed crash]

- **What happens:** `DebugInfo/PortablePdbWriter.cs:325` calls
  `Debug.Assert(false, "Duplicate sequence point definition detected: ...")` when a method is
  seen twice. `CSharpDecompiler.DecompileBody` adds the annotations, including the
  `ILFunction`, to the declaration (`AddAnnotationsToDeclaration`, called around line 2417)
  before the transforms run. When an exception is caught (around line 2496), the body is
  replaced by an error-comment block, but the `ILFunction` annotation stays.
- **Observed:** CI run 45079560877. `ilspycmd -genpdb` on `ICSharpCode.ILSpyCmd.Tests.dll` hit
  this. One body had failed (an engine refusal in strict mode), and the Debug test host was
  terminated: `Process terminated. Assertion failed. Duplicate sequence point definition
  detected: 06000167`. The test project has no trace listener, so the assert is fatal (see E6).
  The exact path to the second registration was not traced.
- **Why it matters:**
  - A single decompile failure in a large assembly makes PDB generation crash a Debug host.
  - In a Release host it writes a PDB from an inconsistent tree.
  - Any decompiler exception, not only the engine's, can trigger it.
- **Suggestion:**
  - When the error handler replaces the body, also drop the `ILFunction` annotation and the
    parameter annotations, or have the PDB writer skip error bodies.
  - Turn the duplicate case into a counted warning instead of an assert.
  - Add a regression test that runs `-genpdb` over a method whose decompilation throws.

### A2. `GetFilteredAccessors` returns the remover where it means the invoker

[verified]

- **What happens:** `TypeSystem/Implementation/MetadataTypeDefinition.cs:769-771`:

  ```csharp
  var invoker = ev.InvokeAccessor;
  if (invoker != null && (filter == null || filter(invoker)))
      yield return remover;
  ```

  This is a copy-paste slip.
- **Why it matters:** an event with a `.fire` accessor yields its remover twice and never its
  raiser. If there is no remover, it yields `null`, and a lookup that filters for the raiser
  gets the wrong method.
- **Port note:** the port reproduces it deliberately (L 3.10).
- **Suggestion:** `yield return invoker;`, plus a type system test with a `.fire` accessor
  (IL fixture).

### A3. `IsLocalFunctionDisplayClass` counts any parameter as a closure parameter

[verified]

- **What happens:** `IL/Transforms/LocalFunctionDecompiler.cs:879`:

  ```csharp
  md.DecodeSignature(new FindTypeDecoder(typeHandle, module), default).ParameterTypes.Any()
  ```

  `FindTypeDecoder` produces one `bool` per parameter, telling whether it mentions the type.
  `.Any()` without a predicate is true for every local function with at least one parameter,
  whatever its types.
- **Why it matters:** more types count as local function display classes than intended. One
  consequence the port hit: decompiling an iterator's `MoveNext` on its own switches the
  transform off.
- **Port note:** the port copies the test as it is (L 3.71).
- **Suggestion:** `.ParameterTypes.Any(found => found)`, then rerun the LocalFunctions and
  state machine fixtures.

### A4. A lambda is never "implicitly typed" for overload resolution

[verified]

- **What happens:** in the lambda translation of `CSharp/ExpressionBuilder.cs`,
  `ame.Parameters.MoveTo(lambda.Parameters)` (line 2699) runs first. Later (line 2727)
  `new DecompiledLambdaResolveResult(..., isImplicitlyTyped: ame.Parameters.Any(p => p.Type is
  null))` reads the collection that was just emptied, so the flag is always false.
- **Why it matters:** type inference and conversions treat implicitly typed lambdas differently
  (`TypeInference.cs`, `CSharpConversions.cs`). Today's output is right only because the rest
  of the pipeline is tuned to this value.
- **Port note:** the engine first computed the flag correctly, and then some `Queryable.Select`
  candidates resolved differently. It now copies the managed value (L 3.228, commit aeb53a302).
- **Suggestion:** compute the flag from `lambda.Parameters`, or before the move. Expect test
  churn and review it, especially query expressions and anonymous types.

### A5. `UsingTransform` compares `IType` by reference for ref structs

[verified code, reported effect]

- **What happens:** `IL/Transforms/UsingTransform.cs:362` compares
  `call.Method.DeclaringType == objVar.Type` by reference. Each signature creates its own
  `ParameterizedType` instance.
- **Why it matters:** a generic ref struct with its own `Dispose`, such as
  `ValueListBuilder<char>`, is never turned into a `using` (L 3.49).
- **Suggestion:** compare with `.Equals`, and add a fixture with a generic ref struct
  `using`.

### A6. The ILAst can share one instruction between two parents

[reported]

- **What happens:**
  - The runtime-async try-finally rewrite leaves one `leave` instruction in two blocks (L 3.30,
    commit ca9f27b8a).
  - The switch transforms leave an instruction in two blocks on two methods of the runtime
    libraries (L 3.44, commit 1a8507bea).
- **Why it matters:**
  - The tree invariant breaks. Debug's `Block.CheckInvariant` rejects these trees; Release
    accepts them silently.
  - Use counts are off by one. The port needed a "reference to an already written node" wire
    opcode just to reproduce the managed load counts.
- **Suggestion:** clone the instruction where it is placed a second time, and run the decompiler
  suite in Debug with invariant checks after each transform (see E7).

### A7. Stale `Parent` pointers of removed instructions reach the data flow visitor

[verified code, reported trigger]

- **What happens:** `ReplaceWith` and the `InstructionCollection` removals leave the removed
  node's `Parent` set. On a runtime-async method of `System.Net.Quic`, a connected branch
  targets a block the rewrite removed. `FlowAnalysis/DataFlowVisitor.cs:437-445` then finds no
  work list for the stale container and only calls `Debug.Fail("Failed to find target
  BlockContainer")` (L 3.35, commit 3c964e93b).
- **Why it matters:** in Release the data flow state for that branch is silently dropped; in
  Debug it asserts.
- **Suggestion:** clear `Parent` on removal (or assert `IsConnected` where it matters), and find
  the transform that leaves a live branch into a removed block.

### A8. `StatementTransform.Run` behaves differently in Debug and Release

[verified]

- **What happens:** `IL/Transforms/StatementTransform.cs:126-142`. Under `#if DEBUG` the loop
  walks `block.Instructions[pos]` from `pos` up to `rerunPosition` to reset dirty flags and
  check invariants. Release just sets `pos = rerunPosition`.
- **Observed in the port:** on one runtime library method a transform shrank the block, so the
  Debug loop indexed past its end. Debug threw where Release did not (L 3.52, commit 883ca3eea).
- **Why it matters:** Debug and Release are not behavior-equivalent, and a rerun position can
  point past the block.
- **Suggestion:** clamp or validate `rerunPosition` against `block.Instructions.Count` in both
  builds, and find the transform that requests a stale rerun position.

### A9. Latent exceptions where a pattern should just not match

[verified code, reachability not checked]

The port turned each of these into "no match" and recorded a `shortcut:` comment. Managed code
would throw an exception that the surrounding code does not expect.

- **`IL/Transforms/DynamicCallSiteTransform.cs:203`:** `callsites.Add(callSiteCacheField, ...)`
  throws `ArgumentException` when the same cache field is initialized twice. A
  `CSharpArgumentInfo` without a matching delegate parameter also throws.
- **`IL/ControlFlow/YieldReturnDecompiler.cs`:** on Mono and VB iterators,
  `finallyMethodToStateRange` stays `default` (null, line 624). `ConvertBody` still calls
  `finallyMethodToStateRange.ContainsKey(...)` (line 901) for any one-argument call on `this`.
  That is a `NullReferenceException`, not the `SymbolicAnalysisFailedException` the handler
  expects.
- **`TypeSystem/TaskType.cs:95-98`:** `builderType = (IType)arg.Value` accepts a null `typeof`
  argument of `[AsyncMethodBuilder]` and fails later.
- **`IL/Transforms/FixRemainingIncrements.cs:79`:** `Debug.Fail` when `Extract` fails.

**Suggestion:** guarded "pattern does not match" paths: `TryAdd`,
`finallyMethodToStateRange?.ContainsKey(...) == true`, and null checks.

### A10. `CallUnsafeIntrinsic` writes an `Unsafe` type the module cannot see

[verified]

- **What happens:** `CSharp/ExpressionBuilder.cs:1542` uses
  `compilation.FindType(KnownTypeCode.Unsafe)`. When the module does not reference
  `System.Runtime.CompilerServices.Unsafe` (FSharp.Core, for example), the result is an unknown,
  fully qualified type name (commit 069d7c2bb).
- **Why it matters:** the output names a type the decompiled project cannot compile against.
- **Suggestion:** fall back to pointer code, or emit a comment, when the type does not resolve.

### A11. Mono iterators with `lock`: use counts include detached code

[reported]

- **What happens:** `YieldReturnDecompiler` inlines Mono's separate finally method without
  releasing it. When `LockTransform` later drops the finally, the detached function still holds
  references, so variable use counts stay too high (L 3.48, fixtures `YieldReturnInLock1/2`).
- **Port note:** this is one of the 4 `LockTransform` entries still in the port's divergence
  allowlist.
- **Suggestion:** `ReleaseRef` the inlined finally function's instructions after inlining.

### A12. `TaskType.IsTask` misses a nullability-annotated `Task<T>`

[verified]

- **What happens:** `TypeSystem/TaskType.cs:68-79` ends in `return type is ParameterizedType;`.
  A `NullabilityAnnotatedType` around `Task<T>` fails the test (L 3.20).
- **Suggestion:** unwrap nullability first.

### A13. Tuple element names from the PDB never reach locals

[verified]

- **What happens:** `DecodeLocalSignature` already produces a `TupleType`.
  `TypeSystem/ApplyAttributeTypeVisitor.cs` has no `VisitTupleType` override, so the
  `TupleElementNames` that `PdbExtraTypeInfo` supplies for locals are dropped (L 3.21).
- **Why it matters:** with a PDB, `(int A, string B) x` decompiles as `(int, string) x`.
- **Suggestion:** add `VisitTupleType` that applies the names, plus a PDB test.

### A14. `(await f()).Item1`: tuple names of an awaited result are lost

[observed]

- **What happens:** the managed builder types an `await` with the return type of the IL's
  `GetResult` method (`ExpressionBuilder.cs:4598`,
  `new ResolveResult(inst.GetResultMethod?.ReturnType ...)`). The method reference in IL is
  `TaskAwaiter<ValueTuple<int, string, string>>::GetResult`, which carries no element names.
- **Observed:** on `ICSharpCode.ILSpyCmd.Tests.dll`, the source
  `Assert.That(result.ExitCode, ...)` with an inlined `result` decompiles to
  `Assert.That((await CliTestRunner.RunAsync(...)).Item1, ...)`, although `RunAsync` is declared
  as `Task<(int ExitCode, string Output, string Error)>`.
- **Port note:** the engine reproduces this on purpose (commit 942de70d5).
- **Suggestion:** take the awaited type's names from the operand's type (the declared return
  type of `RunAsync`), as the C# compiler does. Add a Pretty test with an awaited named tuple
  whose result is inlined.

---

## B. Quirks the port reproduces on purpose

Each of these decides output, so the Rust engine copies it exactly. Each is a candidate for a
fix (with test churn) or for a one-line comment saying it is intended. B2, B4, B8 and B16 look
accidental.

| # | Managed behavior | Where | Source |
|---|---|---|---|
| B1 | `BlockBuilder` never empties its container stack after placing blocks, so `FindBranchTarget` also searches containers left over from that pass | `IL/BlockBuilder.cs:126, 223-230` | verified; L 3.20 |
| B2 | `InstructionCollection.RemoveAll` releases each rejected item before testing the next one, so a block reachable only from a block removed in the same pass loses its edge and is removed too: the result depends on order | `IL/Instructions/InstructionCollection.cs:330-357` | verified; L 3.28 |
| B3 | `ByReferenceType.IsReferenceType` and `PointerType.IsReferenceType` return `null`, not `false` (pinned `char&` locals were first taken for value types in the port) | `TypeSystem/ByReferenceType.cs:41`, `PointerType.cs:41` | verified; L 3.28 |
| B4 | `ReduceNestingTransform.ShouldReduceNesting(block, maxStatements, maxDepth)` never reads `maxDepth`; the default case is picked by `MaxBy(s => s.Labels.Count())`, the first section with the most labels | `IL/Transforms/ReduceNestingTransform.cs:305, 593-601` | verified; L 3.78 |
| B5 | `VarArgInstanceMethod` drops the last parameter even when it is not `__arglist`, and fails on a method without parameters | `TypeSystem/VarArgInstanceMethod.cs:43, 54, 117` | verified; L 3.10 |
| B6 | A generic type definition used as a parent yields a specialization by its own type parameters (printed as `` G`1[[`0]] ``) | type system | reported; L 3.10 |
| B7 | On `void` every member lookup is empty, but nested types still come from its base types; a definition's own nested types are definitions, not parameterized | `GetMembersHelper` | reported; L 3.16 |
| B8 | `SignatureBlobComparer.EqualsMethodSignature` skips custom modifiers only at a slot's top level, and `TypesAreEqual` asserts that it never sees one | `Metadata/SignatureBlobComparer.cs:54-60, 117-121, 161` | verified; L 3.213 |
| B9 | `AddAnnotationsToDeclaration` maps parameters with `ToDictionary(v => v.Index)`, which throws on a duplicate index (an iterator's `MoveNext` `this` at index -1 once the function is annotated after the transforms) | `CSharp/CSharpDecompiler.cs:2537-2547` | verified; L 3.29 |
| B10 | `ILVariableCollection.RemoveAt` swap-removes, so variable order depends on removal history | `IL/Instructions/ILVariableCollection.cs:92-99` | verified; L 3.53 |
| B11 | In Debug builds `YieldReturnDecompiler` inserts commented `nop`s with the state ranges; later transforms delete them, but the Debug and Release ILAst differ in between (visible in Debug Steps) | `YieldReturnDecompiler` | reported; L 3.29 |
| B12 | `MatchLdcI` looks through sign extensions and through zero extensions of an `I4`, and `SwitchDetection.AdjustLabels` relies on that | `IL/Instructions/PatternMatching.cs` | partly verified; L 3.28 |
| B13 | `ILInlining` always inlines an `await`; `SplitVariables` counts `await(ldloca V)` as an immediate address use | `ILInlining`, `SplitVariables` | reported; L 3.30 |
| B14 | `ResolveBinaryOperator` never constant-folds `nint`/`nuint` operands; a constant compared with the null literal is not folded | `CSharp/Resolver/CSharpResolver.cs:850-860` | verified for nint; L 3.228, L 3.231 |
| B15 | The managed builder retypes stack slots as a side effect of translating `stloc` | `ExpressionBuilder` | reported; L 3.88 |
| B16 | `FlattenSwitchBlocks` changes the tree while it enumerates `rootNode.Descendants`, and reads a section's children only after handling it, so the result depends on how the lazy walk sees moved nodes | `CSharp/Transforms/FlattenSwitchBlocks.cs:34-46` | verified; commit 084814f02 |
| B17 | `TranslateSwitch` leaves a local function declared in a switch section undeclared | `StatementBuilder.TranslateSwitch` | reported; L 3.231 |
| B18 | Older F# compilers put a field signature into StandAloneSig (FSharp.Core, row 1, blob `06 08`); SRM rejects it, so callers must tolerate it | metadata | reported; L 3.7 |
| B19 | `ILVariable.RemoveIfRedundant` (internal) and the private `ILFunction.helperVariableCount` change later transforms, but `WriteTo` prints neither | `IL/ILVariable.cs`, `IL/Instructions/ILFunction.cs` | reported; L 3.27, L 3.28 |
| B20 | `ConditionDetection` inverts `if`s through `InvertIf`, which runs `ExpressionTransforms` on the statement in the middle of the block pipeline. Its NullableLifting, array/span-initializer and inline-array rewrites thus already happen inside ConditionDetection, long before the `ExpressionTransforms` leaf | `IL/ControlFlow/ConditionDetection.cs`, `IL/Transforms/ExpressionTransforms.cs` | reported; L 3.47; the 65 ConditionDetection entries in `rust/ilast-divergence-allowlist.json` |

---

## C. Nondeterminism and ordering dependencies

### C1. Culture-dependent sorting is still in the output path

[verified]

- **What happens:**
  - `CSharp/Transforms/IntroduceUsingDeclarations.cs:60-62`:
    `.OrderBy(n => n.StartsWith("System", StringComparison.Ordinal)).ThenByDescending(n => n)`.
    The second key uses `Comparer<string>.Default`, which compares with the current culture.
  - `CSharp/Syntax/TypeSystemAstBuilder.cs`, `SortAttributes` and `CompareAttribute` (around
    line 857): attribute arguments are compared with `IComparable.CompareTo`, which is
    culture-sensitive for strings. Type and argument names use `CompareOrdinal`.
- **Why it matters:** `using` order and sorted attribute order can depend on the host culture,
  or on ICU versus invariant globalization. PR #4268 made output culture-independent and enabled
  CA1309/CA1310/CA1311 (`.editorconfig:288-290`). Those analyzers do not flag a key-selector
  `OrderBy` without a comparer, or `IComparable.CompareTo`.
- **Port note:** the engine leaves both cases to the host for this reason (commits d60db16df,
  930c6c612).
- **Suggestion:** pass `StringComparer.Ordinal`, special-case `string` in `CompareAttribute`, and
  add a test under `tr-TR` and one under invariant globalization.

### C2. Output depends on `Dictionary` enumeration order

[verified]

- **What happens:** `YieldReturnDecompiler.DecompileFinallyBlocks` enumerates
  `finallyMethodToStateRange.Keys` (line 1212). `FindFinallyMethod` iterates it (line 1363)
  after `Remove` calls (line 1326). The order and the "first match" rely on `Dictionary`
  enumerating in insertion order after removals, which is an undocumented implementation
  detail.
- **Port note:** the Rust state range analysis notes exactly this
  (`state_range_analysis.rs:46`).
- **Suggestion:** use a list or an ordered dictionary wherever the order can reach the output.
  The swap-remove in B10 is the same kind of dependency.

### C3. `ILInstruction.Descendants` is post-order, and transforms depend on it

[reported]

- **What happens:** `FixRemainingIncrements`, `CopyPropagation` and `DelegateConstruction`
  register new variables in the order they meet nodes in `Descendants`, children first. That
  order fixes the `I_n`/`C_n` numbering and the order in which local functions are read
  (L 3.69-3.71).
- **Port note:** the port first walked pre-order, and only the local function order showed the
  difference.
- **Suggestion:** document the order on `Descendants`, or name it `DescendantsPostOrder`.

### C4. Caller transforms inserted at index 0 silently depend on position

[verified]

- **What happens:** `ICSharpCode.Decompiler.Tests/Helpers/Tester.cs:1061-1063` inserts
  `RemoveNamespaceMy`, `RemoveCompilerAttribute` and `RemoveEmbeddedAttributes` at index 0 of
  `AstTransforms`.
- **Why it matters:** running them after `IntroduceUsingDeclarations` changes the `using`
  output. The pipeline is a positional mutable list with hidden coupling between entries.
- **Suggestion:** named phases or insertion points ("before IntroduceUsingDeclarations"), so a
  caller's intent is explicit and checkable.

---

## D. Performance

The port found no managed hotspot. Every slow path it found was in the engine:

- a quadratic block append;
- a `MethodSemanticsLookup` rebuilt per call;
- `conversion_operators` resolving whole member lists.

What the work gives the C# maintainers is a baseline and one technique.

- **Managed baseline, Release, single thread** (`RustPort_PerformanceComparison.md`):

  | Input | CPU total | Peak RSS | Managed allocation |
  |---|---|---|---|
  | `ICSharpCode.Decompiler.dll` | 12.1 s | 210 MB | 3,590 MB for 902 types (about 4 MB per type) |
  | `System.Reflection.Metadata.dll` 9.0.0 | 4.5 s | 120 MB | 589 MB |
  | `ILSpy.dll` | 9.0 s | 226 MB | 1,837 MB |
  | top-200 NuGet corpus, 39,011 types | 265 s | 477 MB | 86.6 GB |

  Per-type P50 is 2 to 7 ms and P95 22 to 59 ms. About 4 MB allocated per type is high enough to
  deserve an allocation profile; nobody has made one.
- **Test time on master:**
  - The four generated `ExplicitConversions` round trips (`RoundtripAssembly`) take 4 to 5.5
    minutes each of test time in the Windows Debug job, and the whole round-trip fixture is 29
    minutes.
  - Round-tripping `ExplicitConversions_32` takes 11 s managed locally.
  - These fixtures are the only inputs with thousands of calls, each with many overloads whose
    parameter types have user-defined conversions. They are a good stress test for the
    resolver.
- **A shape worth looking for:** a member lookup that resolves (decodes) every member of a type
  and its bases only to keep a few by name. In the engine this cost 50x on the conversion
  fixtures. The managed `GetMethods(filter)` filters lazily over already-decoded member lists,
  so the same cost is not known to apply. It was not measured.
- **Tooling note:** `dotnet-trace` shows native engine time only as `UNMANAGED_CODE_TIME`. For a
  mixed managed/native profile on Windows, use ETW (`wpr`) with symbols.

---

## E. Tests and test infrastructure

### E1. PdbGen writes build outputs into the source `TestCases` tree

[verified code, observed failures]

- **What happens:** `ICSharpCode.Decompiler.Tests/PdbGenerationTestRunner.cs:720-729` compiles
  `<TestCases>/PdbGen/<name>.expected.dll` and `.pdb` next to the sources on every run. The
  fixture is `[Parallelizable(ParallelScope.All)]`.
- **Observed:** any second reader of `TestCases` (another test host, an IDE, an indexer) finds
  the files locked or briefly deleted:
  - CS2012 "file locked by a concurrent test host" (commits f8a2a4ab1, c2a6cb640, 7347e83ae);
  - "PdbGen images read while being rewritten" (commit 6fe8d34b9);
  - six PdbGen tests failed in one full run on file locks, and passed alone.
  - The Rust tests now retry `File.ReadAllBytes` for up to a minute (`Corpus.ReadFile`).
- **Suggestion:** compile fixtures into a per-run output directory (under `obj/` or temp),
  never into the source tree.

### E2. The `NewtonsoftJson_net45` round trip fails under load, cause unknown

[observed]

- **What happens:** `RoundtripAssembly.cs` runs the decompiled Newtonsoft.Json tests with
  `nunit3-console.exe`.
- **Observed:** in full parallel solution runs it failed several times with a runner failure and
  no compile errors, and passed alone each time (commits 4695fb901, bb5c7dd9a, 3fdd22f3b,
  7329bdbae, 8473bf197, 1b7fc69a9, 6fe8d34b9).
- **Suggestion:** put the console runner's exit code and stderr into the assertion message, so
  the next failure explains itself.

### E3. Wall-clock budgets in UI tests fail under CPU load

[verified budgets, observed failures]

- **What happens:** `ILSpy.Tests/Processes/ProcessExplorerTests.cs` and
  `ILSpy.Tests.Windows/Processes/NetFrameworkProcessesTests.cs` use 60 s
  `CancellationTokenSource`s for process enumeration.
- **Observed:**
  - Locally, `The_Explorer_Routes_A_Framework_Process_To_The_Module_Scan` and
    `The_Entry_Assembly_Of_The_Current_Process_Resolves_To_A_Real_File` failed under load and
    passed alone.
  - On the 4-core Windows runner, next to a CPU-heavy suite, three process explorer tests ran
    15 to 19 minutes and were cancelled.
  - The bookmark navigation test's 60 s `WaitForDecompiledTextAsync` also expired.
- **Why it matters:** these failures say "the machine was busy", not "the code is wrong", and
  they cost a CI run each time.
- **Suggestion:** scale budgets by an environment factor on CI, or wait on progress instead of
  wall time. Process enumeration on a machine with many short-lived processes (the decompiler
  tests start compilers and test runners) is slow on its own.

### E4. A thread-affinity race in the base types tree node

[reported]

- **What happens:** `NavigationTests.Back_History_Dropdown_Uses_NavigationText_For_Generic_Tree_Nodes`
  fails occasionally under load with a thread-affinity error from `BaseTypesTreeNode`, and
  passes alone (commits 8473bf197, 1b7fc69a9).
- **Suggestion:** check where `BaseTypesTreeNode` loads children off the UI thread.

### E5. Multi-selection tab titles have no length limit

[observed, fixed upstream for the test]

- **What happened:** `SwitchListAndDeleteAllTests.Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab`
  hung on master on this machine. The UI thread spun in Avalonia font fallback. PR #4355
  (cherry-picked as 239edbc33) found the cause:
  - auto-load had pulled in 214 assemblies;
  - Ctrl+A joined their names into a tab title of about 12,000 characters;
  - the headless stub fonts took minutes to lay it out.
- **Suggestion:** the test is fixed, but the product still builds an unbounded title. Cap it
  (for example "N assemblies").

### E6. A `Debug.Assert` kills test hosts that install no listener

[verified]

- **What happens:** only `ICSharpCode.Decompiler.Tests` installs a trace listener that turns
  `Debug.Fail` into a test failure (`TestTraceListener.cs`). The Rust tests have one too
  (`AssertionListener.cs`). `ICSharpCode.ILSpyCmd.Tests` and the UI test projects run decompiler
  code without one, so an assert ends the whole test host. That is how A1 surfaced: a crashed
  host and 11 recovered results, instead of one failed test.
- **Suggestion:** link the same listener into every test project that runs decompiler code.

### E7. Debug-only invariant checks reject trees that Release produces

[reported]

- **What happens:** the port's managed reference drives the IL pipeline itself. In Debug,
  `Block.CheckInvariant` and the statement driver's checks threw on the trees of A6 and A8
  (L 3.30, L 3.44, L 3.52). The trees came from .NET runtime libraries in the port's corpus;
  the decompiler suite's own fixtures apparently do not contain these shapes, since its Debug
  runs pass.
- **Suggestion:** run a Debug decompile over the .NET runtime libraries (as the port's corpus
  does) in a periodic job, and treat each invariant failure as a bug.

### E8. ILAst dumps hide state that changes later transforms

[reported]

- **What happens:** `ILFunction.WriteTo` does not print:
  - tuple element names;
  - full variable types;
  - `RemoveIfRedundant`;
  - `helperVariableCount`.
- **Port note:** the port's equality gate had to add one line per variable to compare them
  (L 3.21, L 3.27).
- **Suggestion:** a verbose `ILAstWritingOptions` mode, so `--ilast` and Debug Steps show
  everything a later transform reads.

### E9. Text oracles miss whole classes of differences

[observed]

The decompiler suite proved to be a good oracle for output, with clear blind spots:

- **Default settings, one compiler:** a suite driven by one corpus with default settings and
  one compiler hides most differences. When the engine was first held to all test cases in
  strict mode, about 2,000 tests failed that a single-compiler, default-settings corpus had not
  shown (L 3.223). The managed suite is broad because of its compiler matrix. Keep it, and
  treat new settings as needing matrix coverage.
- **Error comments look like missing bodies:** a strict exception inside a body is caught by
  `DecompileBody`'s error handler and becomes an error comment. The test then fails as an
  output difference, and the reason sits in the comment, easy to miss (L 3.227). A test mode
  that rethrows from `DecompileBody` would make such failures direct.
- **Self-decompiling tests catch real bugs:** ilspycmd's tests decompile the test assembly
  itself, and that found A1 and A14. Self-decompile tests are cheap and worth having in more
  places.

### E10. Local environment traps

[observed]

- **MAX_PATH:** Correctness tests fail with "Failed to start a process ...
  ICSharpCode.Decompiler.TestRunner.exe" (error 2) from deep checkouts, because the tests build
  an unnormalized relative path that exceeds MAX_PATH. `Path.GetFullPath` before starting the
  process would fix it.
- **Debug after Release:** a Release `build.ps1` restores Release assets. A following Debug
  `--no-restore` build then fails with CS1061 `WithDeveloperTools`; run `restore.ps1` in
  between.
- **Test filters (MTP):** `dotnet test --filter` exits 8 when a project matched no tests, and
  `--treenode-filter`/`--filter-class` give exit 5. The per-assembly summary is the truth.
- **Two output trees:** a solution-level build writes `bin/Debug`, a project-level build with
  `-p:Platform="Any CPU"` writes `bin/Any CPU/Debug`, and either can be stale when tests run
  with `--no-build`.

### E11. `CancelPendingAsync` returns a task that ends Canceled even on success

[verified]

- **What happens:** `ILSpy/TextView/DecompilerTabPageModel.cs:525-543`. `pendingDecompile` is
  `decompile.ContinueWith(..., TaskContinuationOptions.OnlyOnFaulted | ExecuteSynchronously)`,
  so it is Canceled whenever the decompile did not fault. `DockWorkspace.CancelPendingOperationsAsync`
  wraps these tasks in `Task.WhenAll`.
- **Why it matters:** the only caller (the test reset attribute) polls `IsCompleted`, so nothing
  breaks today. Anyone who awaits it gets a `TaskCanceledException` after a successful
  decompile.
- **Suggestion:** keep the decompile task itself, or use a continuation without an `OnlyOn*`
  filter that only logs faults.

### E12. The MSI and VSIX file lists drift unnoticed

[observed]

- **What happens:** `BuildTools/ILSpy-x64.msi.filelist` and `ILSpy.AddIn.VS2022.vsix.filelist`
  had drifted on master before the branch started. The "Verify package contents" step runs
  only in the Debug job, which builds no MSI or VSIX, so it checks nothing (commit 35c0acf40).
- **Suggestion:** run the check in the Release job that builds the installers.

### E13. Running every test host at once starves a 4-core runner

[observed]

- **What happens:** the Windows CI job runs `dotnet test --solution` with all test hosts in
  parallel on a 4-core runner. With one more CPU-heavy host, every suite slowed about tenfold
  (the decompiler suite went from 15 minutes to 4.5 hours), and timing-sensitive UI tests failed.
- **Earlier evidence:** server GC in the decompiler suite once starved the others the same way,
  which is why the job sets `DOTNET_gcServer: 0`.
- **Suggestion:** cap the number of test hosts that run at once on CI, or run the heavy suites in
  their own step.

---

## F. API and design

### F1. `CSharpDecompiler` has no extension points, so an alternate pipeline must fork it

[verified]

- **What happens:** `CSharpDecompiler` is a `public class`, not `partial`, and has no hook
  between "IL read", "IL transforms", "statement builder" and "AST transforms". The port had to
  fork the file (`ICSharpCode.Decompiler.Rust/CSharp/CSharpDecompiler.cs`) and keep it in sync
  with a drift test that strips the marked blocks and compares bytes.
- **Two side effects of the fork:**
  - `dotnet format` re-indents a statement under a brace-less `if`, so the seams had to be
    blocks around `function.RunTransforms(...)` and the AST transform loop.
  - A `goto` skips `ConvertAsBlock` (L 3.22, L 3.87).
- **Suggestion:** virtual or pluggable steps ("produce the `ILFunction` after N transforms",
  "produce the body", "run the AST transforms"), or make the class `partial`. Both would also
  help the Debug Steps and analysis features, which already need partial pipelines.

### F2. Internals that outside pipelines need

[reported]

- **What happens:** the materializer that rebuilds managed ILAst from the engine's output must:
  - set the private `ILFunction.helperVariableCount` by reflection;
  - create the internal `FakeMethod` by reflection;
  - create blocks with their final `Kind` up front, because `Block.Kind` is read-only
    (L 3.20, L 3.28, L 3.30).
- **Suggestion:** internal-but-settable APIs, or a small builder for ILAst construction, so
  tools that build ILAst (also `BamlDecompiler`) do not depend on private members.

### F3. IL transforms are effectively pure; say so

[reported]

- **What happens:** IL transforms depend only on the method and its module. The only
  `DecompileRun` use in `IL/` is a pass-through in `YieldReturnDecompiler`, plus `UsingScope`
  for name lookup (L 3.2).
- **Suggestion:** document and test this invariant. It makes per-method caching and parallel
  decompilation safe, and it is what made a per-method handover to the engine possible.

### F4. Tests that depend on the decompiler's file name

[reported]

- **What happens:**
  - `StepperTesting`, `ProjectFileWriter*Tests` and `WholeProjectDecompilerTests` open
    `ICSharpCode.Decompiler.dll` from the output directory.
  - `TransitiveReferenceTests` asserts a reference named `ICSharpCode.Decompiler`.
- **Consequence:** any alternative implementation must keep the original's name, key and
  version, and must never meet the original on one reference path (L 3.1).
- **Suggestion:** get the assembly from `typeof(CSharpDecompiler).Assembly` in tests.

### F5. AST slot metadata lacks declaration order

[verified]

- **What happens:** the shim builds its syntax-tree schema by reflection over the AST node
  classes (slot fields, properties, backing fields) and used to sort them by `MetadataToken`.
- **Suggestion:** have the slot source generator in `ICSharpCode.Decompiler.Generators` emit the
  scalar properties and child slots in declaration order, so consumers need no reflection
  (see G2).

---

## G. Build, trimming and Native AOT

### G1. Unguarded polyfills block a modern target framework

[verified]

- **What happens:** `Util/Index.cs` (a `System.Index` in namespace `System`, guarded only by
  `SYSTEM_PRIVATE_CORELIB`), `NRTAttributes.cs` and `TrimAttributes.cs` define BCL types
  unconditionally. On a .NET Core target framework they become CS0436, which is fatal with
  warnings as errors (L 3.1).
- **Suggestion:** wrap them in `#if !NETCOREAPP` (or the matching feature symbols), so the
  library can also target net10 and later.

### G2. Reflection over AST members breaks under trimming and Native AOT

[observed]

- **What happens:**
  - The trimmed ilspycmd lost the AST node members the shim's schema reflects over ("SyntaxTree
    has no slot Members"); `TrimmerRootAssembly ICSharpCode.Decompiler` fixed it.
  - Native AOT has no `MemberInfo.MetadataToken`, so the member order is now generated into
    `Interop/SyntaxSchema.Order.cs` (commits 64bfec31a, 81a907157).
- **Lesson:** consumers that reflect over the AST types need generated metadata (F5).
- **Gate gap:** `ICSharpCode.Decompiler.AotTest` roots the whole assembly, so it cannot catch
  trimming of members that an unrooted consumer reflects over.

### G3. An action missing from `actions.lock` fails the workflow without a log

[observed]

- **What happens:** a new `uses:` that is not pinned in `.github/workflows/actions.lock` ends the
  run in `startup_failure` with no job log at all.
- **Suggestion:** document it in `.github/` and in AGENTS.md (`gh actions-lock` regenerates the
  file).

---

## Suggested order of work

1. **A1 and E6.** These are crashes that hide everything else. Remove the `ILFunction`
   annotation from error bodies, and install the assert listener in every test project.
2. **The one-line bugs, each with a test:** A2, A3, A5, A12, A13 and A14.
3. **A4.** Lambda implicit typing, with a review of the test churn.
4. **C1.** Culture-independent ordering, plus a `tr-TR` test.
5. **E1 and E12.** Fixture outputs out of the source tree, and the file-list check moved to the
   Release job.
6. **A6, A7, A8 and E7.** A Debug sweep over the runtime libraries, with invariant failures
   treated as bugs.
7. **The B list.** Fix or document each entry.
8. **F1.** Extension points in `CSharpDecompiler`, if an alternate pipeline (Rust or other) is
   to be maintained alongside.
