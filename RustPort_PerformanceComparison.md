# Rust engine vs. managed decompiler: performance and memory

Item 88 of `RustPort_AgentInstructions.md`. First measured 2026-11-22 on branch
`christophwille/rustafari` at 1b7fc69a9, with the methodology of `RustPort_HumanLecture.md`
section 6 and the tooling it describes (`TestTools/perfcompare.ps1`, `decompdiff --perf --side`,
`ilspy_stats`, `rust/ilspy-decompiler/benches/stages.rs`). Measured again 2026-11-23 at
0b854fc83, after the first engine hotspot was fixed (8144c2911, see "The conversion operator
hotspot"). The tables show the second measurement; the first one is kept where it explains what
changed.

## Summary

- **CPU: the engine is 1.5x to 7x slower than the managed decompiler.** It is 1.5x slower on
  `System.Reflection.Metadata`, 3.2x on `ILSpy.dll`, 7.1x on `ICSharpCode.Decompiler.dll` and
  5.6x on the top-200 NuGet corpus. Before the hotspot fix it was 1.9x, 5.1x, 9.6x and 11.2x.
- **One fix halved the corpus time.** User-defined conversion operators were looked up by
  resolving every method of both types for every argument of every overload candidate. Finding
  them by name first and keeping them per request took the engine's corpus time from 2,894 s to
  1,491 s and its Rust allocations from 1.46 TB to 0.74 TB.
- **The time is still spent inside Rust.** Rust accounts for 88 to 92 percent of end-to-end
  time, except on `System.Reflection.Metadata`, where it is 80 percent. Marshaling and
  materialization take 5 to 11 percent. Callbacks into the host take 3 to 7 percent.
- **The slowdown is broad, not a few hot types.** The 10 slowest types explain only 19 to 27
  percent of the extra time on the two larger inputs. The median per-type ratio grows with the
  size of the input and its reference closure: 1.3x, 2.5x, 6.7x. Over the corpus the median
  per-assembly ratio is 5.1x.
- **The engine allocates a lot of short-lived memory.** Each request allocates about 0.1 to 0.4
  MB in 1,100 to 4,100 allocations: 0.74 TB in 5.7 billion allocations over the corpus.
- **Memory: peak RSS is close on single assemblies.** It is 9 to 19 percent higher. Over a long
  corpus run it is 2.7x higher (477 MB against 1,286 MB). That peak is one large assembly's
  session (Rust peak live 1,040 MB); live Rust memory at the end of the run is 0.7 MB, so
  nothing leaks.
- **The managed heap is not relieved.** The host allocates 4 to 7 percent more managed memory
  with the engine than without it, because the type system, the resolver behind the callbacks,
  AST materialization and output stay managed.
- **The engine's ILAst stages alone cost as much as the whole managed decompile.** On
  `ICSharpCode.Decompiler.dll`, IL reading takes 5.6 s and the IL transforms 7.0 s; the managed
  decompile takes 12.1 s in total. That is 13 of the 76 s inside Rust; the other 63 s are in
  stages the benches cannot isolate (see "Where the time goes").

The port is functionally complete: every test runs strict on the engine. Performance is the
open item. One hotspot has been found and fixed by timing the engine's stages by hand; the
numbers point at several more specific causes rather than at the language (see "What to
optimize first").

## Setup

| | |
|---|---|
| Machine | AMD Ryzen AI 9 HX 370, 24 logical CPUs, 95 GB RAM, Windows 11 Pro 10.0.26300 |
| .NET | SDK 11.0.100; `decompdiff` runs on .NET 11 |
| Rust | rustc 1.99.1; native library built with the cargo `ship` profile (release, no debug assertions, no overflow checks, default `codegen-units`, no LTO) |
| Managed side | `ICSharpCode.Decompiler/bin/Release/netstandard2.0/ICSharpCode.Decompiler.dll` |
| Engine side | `ICSharpCode.Decompiler.Rust/bin/Release/netstandard2.0/ICSharpCode.Decompiler.dll` (the shim) with `ilspy_decompiler_ffi.dll` from `rust/target/ship` |
| Settings | `DecompilerSettings` defaults; strict mode off (no test project initializer runs in `decompdiff`), so any fallback would be counted, not thrown |
| Harness | `perfcompare.ps1`: each side in its own `decompdiff --side` process, `-j 1`, warm-up on the first assembly and discarded |
| Runs | Reference inputs: 3 runs per side, median. Corpus: 1 run per side (about 25 minutes for the engine side alone; across 39,011 types the totals and percentiles barely move between runs) |

Inputs:

- `ICSharpCode.Decompiler.dll`: the Release netstandard2.0 build, 902 types and 15,620 method
  bodies.
- `System.Reflection.Metadata.dll` 9.0.0, lib/net462: 406 types and 3,552 bodies.
- `ILSpy.dll`, the Release net10.0 build: 521 types and 7,695 bodies. This is the "net10
  assembly" input. The AOT gate uses its own small test executable for that slot; `ILSpy.dll`
  is larger and has a deep reference closure (Avalonia, Dock), which makes it the more telling
  input.
- Top-200 NuGet corpus: `TestTools/nuget-top.ps1 -Count 200` with dependency closures. It has
  163 lib directories and 172 assemblies, of which 4 fail to load the same way on both sides.
  That leaves 39,011 types.

## End-to-end results

CPU is wall time of the type decompiles at `-j 1`, so it equals CPU time on one core. The
ratio before the hotspot fix is in parentheses.

| Input | Managed CPU total | Engine CPU total | Ratio | Marshaling share | Managed peak RSS | Engine peak RSS | Rust peak live |
|---|---|---|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | 12.1 s | 86.2 s (116.8 s) | 7.1x (9.6x) | 4.8 % | 210 MB | 228 MB | 17 MB |
| System.Reflection.Metadata.dll | 4.5 s | 6.7 s (8.5 s) | 1.5x (1.9x) | 11.3 % | 120 MB | 143 MB | 8 MB |
| ILSpy.dll (net10) | 9.0 s | 29.0 s (39.6 s) | 3.2x (5.1x) | 6.5 % | 226 MB | 248 MB | 24 MB |
| top-200 NuGet corpus | 265 s | 1,491 s (2,894 s) | 5.6x (11.2x) | 5.4 % | 477 MB | 1,286 MB | 1,040 MB |

The managed side of `ILSpy.dll` took 9.0 s in this measurement and 7.8 s in the first one; the
other managed totals moved by under 3 percent. Part of that input's lower ratio is that noise.

Per-type time and managed allocation:

| Input | Types | Managed P50 | Managed P95 | Engine P50 | Engine P95 | Managed allocated, managed / engine side |
|---|---|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | 902 | 5.4 ms | 51 ms | 32.0 ms (37.0) | 374 ms (567) | 3,590 MB / 3,808 MB |
| System.Reflection.Metadata.dll | 406 | 4.9 ms | 42 ms | 7.4 ms (7.2) | 65 ms (77) | 589 MB / 625 MB |
| ILSpy.dll (net10) | 521 | 7.2 ms | 59 ms | 20.6 ms (24.8) | 230 ms (349) | 1,837 MB / 1,965 MB |
| top-200 NuGet corpus | 39,011 | 2.2 ms | 22 ms | 10.5 ms (15.2) | 132 ms (251) | 86,625 MB / 90,055 MB |

The fix mostly shortened the slow tail: the engine's P95 dropped by a third to a half, its P50
by up to a third.

## The conversion operator hotspot

The CI test runs found it. On a 4-core Windows runner the assembly round-trip tests took 493
minutes of test time against 29 on master. Nearly all of it was four generated conversion round
trips: `ExplicitConversions` took 145 minutes against 5.5. Locally, round-tripping
`ExplicitConversions_32` took 573 s on the engine against 11 s managed.

No sampling profiler for native code is set up on this machine, and `dotnet-trace` attributes
the time only to `UNMANAGED_CODE_TIME` under `RustEngine.ReadFunction`. Temporary timers around
the stages of `wire::ilast::read_function` and then around the functions of the call builder,
overload resolution and conversions narrowed it down:

- IL read 18 ms, IL transforms 11 ms, C# translation of the body 1,220 ms, of which host
  callbacks took 30 ms.
- Within the translation: calls 1,170 ms, of which overload resolution 1,140 ms, of which
  `Conversions::conversion_operators` 1,080 ms.

`conversion_operators` collects the user-defined `op_Implicit` (and `op_Explicit`) operators of
two types. It called `get_methods` on each type, which resolves every method of the type and
its base types, signature included, and then kept the few with the operator name. Each overload
candidate's argument conversions ask for it, so a `Console.WriteLine(x)` with 19 candidates
resolved thousands of methods. The fix (8144c2911) filters the MethodDef rows by name before a
signature is decoded (`get_methods_named`) and keeps each type's operators for the request
(`conversion_operator_methods`). The `Conv_I1` type of `ExplicitConversions_32` went from 113 s
to 8.4 s.

None of the first measurement's inputs had shown it as a single hot type, because every call
with user-defined conversions in its candidates' parameter types paid a little: the fix took a
quarter off `ICSharpCode.Decompiler.dll` and half off the corpus.

## Engine counters

The engine-side numbers are from `RustStats` (the counting allocator and the timers in the ffi
crate's `run`), first run of each input. They are deltas after the warm-up, except the live and
peak bytes. The first measurement is in parentheses where it differs.

| Counter | ICSharpCode.Decompiler.dll | top-200 corpus |
|---|---|---|
| Requests (shim to engine) | 56,014 | 1,854,804 |
| FFI calls (including `buf_free`) | 111,128 | 3,670,933 |
| Time in requests, end to end | 82.4 s (113.3 s) | 1,388 s (2,791 s) |
| Time inside Rust, without callbacks | 75.8 s, 92 % (106.4 s, 94 %) | 1,255 s, 90 % (2,651 s, 95 %) |
| Callbacks into the host | 2.7 s (3 %) | 52 s (4 %) |
| Marshaling and materialization | 3.9 s (5 %) | 80 s (6 %) |
| Request bytes / response bytes | 12.9 MB / 30.9 MB | 309 MB / 798 MB |
| Rust bytes allocated | 16.2 GB (32.1 GB) | 737 GB (1,464 GB) |
| Rust allocations | 213 million (328 million) | 5,660 million (10,968 million) |
| Per request: bytes / allocations | 282 KB / 3,808 (560 KB / 5,852) | 388 KB / 3,051 (771 KB / 5,913) |
| Rust live at end / peak live | 4 MB / 18 MB | 0.7 MB / 1,040 MB |

The wire is small: about 0.8 KB per request in and 0.5 KB out. Moving data across the boundary
is therefore not the problem. The problem is what the engine does per request, which allocates
hundreds of times as much as it sends back. Callback and marshaling time did not change with the
fix, so their share grew as the time inside Rust shrank.

## Where the time goes

Criterion benches (`cargo bench --bench stages`) time each engine stage alone over the three
reference inputs. A stage's input is produced outside the timed part. Medians, from the first
measurement; the benches run without the host's resolver, so the hotspot fix does not reach
them:

| Stage | ICSharpCode.Decompiler.dll | System.Reflection.Metadata.dll | ILSpy.dll |
|---|---|---|---|
| metadata load (lazy) | 0.6 us | 0.6 us | 0.6 us |
| type system load (lazy) | 34 us | 50 us | 463 us |
| IL read, all bodies | 5.63 s (360 us/body) | 0.58 s (164 us/body) | 4.63 s (602 us/body) |
| IL transforms, all bodies | 7.02 s (450 us/body) | 0.83 s (233 us/body) | 3.25 s (423 us/body) |
| expression building | 19 ms (923 bodies) | 3.7 ms (110 bodies) | 7.8 ms (144 bodies) |
| serialization of the ILAst | 295 ms | 45 ms | 144 ms |

Bench limits, from the crate API:

- Metadata parsing and the type system are lazy, so their cost lands in IL read. That stage is
  "IL read plus whatever type-system work the reader triggers".
- The IL transforms are one block; per-transform timing would need one `run_prefix(k)` per
  step.
- Without the host's resolver, expression building translates only 110 to 923 of the bodies,
  and overload resolution, where the hotspot was, never runs. That row is not representative
  of the shim's seam 2 work.
- The seam 2 syntax-tree transforms need a tree from the host and are not benched.

Reading the two together:

1. **IL read is far too slow.** 360 to 600 us per body is about the time the whole managed
   pipeline takes for a whole type at P50 (2 to 7 ms with 10 to 20 bodies). IL read grows with
   the reference closure: `ILSpy.dll` is 602 us/body against 164 us/body for
   `System.Reflection.Metadata`. That matches the per-type ratios above, so the reader most
   likely does type-system work, such as resolving signatures across modules, per body instead
   of once per session.
2. **The ILAst stages explain only part of the gap.** On `ICSharpCode.Decompiler.dll`, IL read
   plus transforms is 12.7 s of the 76 s inside Rust. The rest is in what the shim asks for
   beyond that: seam 2 expression building with the resolver (where the hotspot was), the
   syntax-tree round trip of the 16 AST transforms, and the declaration and member-walk
   requests. It is also possibly in repeated work: there are 56,014 requests for 15,620 bodies,
   about 3.6 per body, and 111,128 FFI calls. The stage timers that found the hotspot can split
   this further.
3. **Allocation churn is a cost of its own.** About 3,800 allocations per request at roughly
   75 bytes each means the allocator is on the hot path. In the managed decompiler the same
   objects are bump-allocated and collected in gen 0. Arena allocation per request, or reusing
   the per-session structures, is the usual fix.

### Slowest types by extra time (first run)

| Input | Type | Managed | Engine | Extra |
|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | `CSharp.ExpressionBuilder` | 465 ms | 3,520 ms | 3,055 ms |
| ICSharpCode.Decompiler.dll | `CSharp.CSharpDecompiler` | 201 ms | 1,707 ms | 1,507 ms |
| ICSharpCode.Decompiler.dll | `CSharp.Transforms.PatternStatementTransform` | 96 ms | 1,516 ms | 1,420 ms |
| ICSharpCode.Decompiler.dll | `CSharp.StatementBuilder` | 124 ms | 1,421 ms | 1,297 ms |
| ICSharpCode.Decompiler.dll | `CSharp.Syntax.TypeSystemAstBuilder` | 202 ms | 1,467 ms | 1,265 ms |
| System.Reflection.Metadata.dll | `Ecma335.MetadataBuilder` | 295 ms | 661 ms | 366 ms |
| ILSpy.dll | `TextView.DecompilerTextView` | 446 ms | 1,416 ms | 970 ms |
| ILSpy.dll | `Controls.TreeView.SharpTreeView` | 180 ms | 874 ms | 694 ms |

In the first measurement some small types were 30 to 60 times slower: `IdStringProvider` (71 ms
against 2,631 ms), `ILAmbience` (55 ms against 2,440 ms), `TextWriterTokenWriter` and
`SessionSettings`. They have few bodies but most likely many calls to heavily overloaded
methods of the reference closure (such as `StringBuilder.Append` and `TextWriter.Write`),
where the hotspot cost most. The
large types with many bodies now lead the list.

### Corpus: slowest assemblies by extra time

| Assembly | Managed | Engine | Ratio (first measurement) |
|---|---|---|---|
| DocumentFormat.OpenXml.dll | 27.3 s | 212.3 s | 7.8x (10.6x) |
| Microsoft.Azure.Cosmos.Client.dll | 10.4 s | 91.1 s | 8.7x (14.3x) |
| BouncyCastle.Cryptography.dll | 15.5 s | 77.8 s | 5.0x (9.0x) |
| BouncyCastle.Crypto.dll | 11.1 s | 63.1 s | 5.7x |
| SixLabors.ImageSharp.dll | 10.9 s | 62.1 s | 5.7x (15.6x) |
| Microsoft.EntityFrameworkCore.dll | 9.1 s | 58.1 s | 6.4x (12.9x) |

Spread of the per-assembly ratio over the 109 assemblies whose managed time is over 200 ms:
minimum 2.6x, first quartile 4.2x, median 5.1x, third quartile 6.7x, maximum 10.8x. In the
first measurement it was 4.7x, 8.2x, 10.8x, 14.1x and 37.6x.

## Memory

- **Peak RSS, single assembly:** +9 % (`ICSharpCode.Decompiler.dll`), +19 %
  (`System.Reflection.Metadata.dll`) and +10 % (`ILSpy.dll`). The Rust heap peaks at 8 to 24 MB
  and adds the native library's own pages (5.8 MB image) on top of an unchanged managed heap.
- **Peak RSS, corpus:** 1,286 MB against 477 MB. Rust peak live reaches 1,040 MB, and live
  bytes are back to 0.7 MB at the end. So it is a peak while one session is alive, not a leak:
  the largest assemblies (`DocumentFormat.OpenXml.dll` and similar) hold their per-session
  structures in Rust for the whole assembly. In the UI, with one assembly decompiled at a time,
  this shows up as a few hundred MB extra while a large assembly is open. The fix did not change
  the peak, because it removed short-lived allocations only.
- **Managed allocation:** 4 to 7 percent more with the engine. Nothing managed is saved, because
  the type system, the resolver, materialized ASTs, the transforms that stay managed and the
  output visitor all run in .NET either way. The materialized trees and the wire buffers are
  the extra.

## Marshaling

The marshaling share is 4.8 to 11.3 percent: request time minus time inside Rust minus
callbacks. Its absolute time is unchanged; the share grew because the time inside Rust
shrank. It is under the 20 percent threshold, so string-table reuse and pre-sized node lists are
not the priority yet. It is highest on `System.Reflection.Metadata.dll`, where the engine itself
is fastest.

## What to optimize first

Ranked by expected gain. Only the conversion operator lookup has been done; each other point is
a separate piece of work.

1. **Look for more lookups that resolve whole member lists.** The fixed hotspot resolved every
   method of a type to filter by name. `get_methods`, `get_properties` and the member lookups
   behind overload resolution, extension methods and operator candidates may have the same
   shape elsewhere. Timing a slow type's request stage by stage, as for the hotspot, finds
   them; per-request caches like `conversion_operator_methods` fix them.
2. **Profile one engine run.** Run `decompdiff --side new` on `ICSharpCode.Decompiler.dll` under
   a sampling profiler (ETW via `wpr` on Windows, `perf` or `samply` on Linux) with symbols for
   `ilspy_decompiler_ffi`. The hand-placed timers need a guess where to look; a profile does
   not.
3. **Make IL read per-session, not per-body, for type-system work.** Cache signature decoding,
   member and type resolution across bodies of one session. The bench gives a direct
   before/after number (360 to 600 us per body now, against a P50 of 2 to 7 ms for a whole
   type in the managed decompiler).
4. **Cut repeated work per body.** There are 3.6 requests and 7 FFI calls per body. Check
   whether a body is read or transformed more than once (seam 1 prefix, seam 2 body, lambdas
   and local functions reread for each containing method), and whether the 16 syntax-tree
   transforms could run in one round trip per type. Per-request caches such as the operator
   cache would then also live longer.
5. **Reduce allocation churn.** Use an arena or bump allocator per request, reuse buffers
   across requests of a session, and intern strings once per session. The counting allocator
   gives the before/after number.
6. **Build settings.** `codegen-units = 1` and `lto = "thin"` or `"fat"` in the `ship` profile
   usually give 10 to 20 percent. They are cheap to try, but small next to the points above.
7. **Bound the session's peak on huge assemblies.** Free per-type structures after each type
   where they are not needed again, so the 1 GB corpus peak drops toward the single-assembly
   numbers.

## Reproduce

```
pwsh ./build.ps1 -Configuration Release --no-restore
pwsh TestTools/nuget-top.ps1 -Count 200
pwsh TestTools/perfcompare.ps1 -Old ICSharpCode.Decompiler/bin/Release/netstandard2.0/ICSharpCode.Decompiler.dll `
  -New ICSharpCode.Decompiler.Rust/bin/Release/netstandard2.0/ICSharpCode.Decompiler.dll -Runs 3 `
  ICSharpCode.Decompiler/bin/Release/netstandard2.0/ICSharpCode.Decompiler.dll `
  <nuget>/system.reflection.metadata/9.0.0/lib/net462/System.Reflection.Metadata.dll `
  ILSpy/bin/Release/net10.0/ILSpy.dll
pwsh TestTools/perfcompare.ps1 -Old <same> -New <same> -Runs 1 "@TestTools/crawl/top-200.corpus.txt"
```

Benches, from `rust/`: set `ILSPY_BENCH_INPUTS` to the three dlls and `ILSPY_BENCH_REFS` to their
reference directories, separated by `;`. The reference directories are the netstandard.library
2.0.3 `ref` directory, the net462 reference-assembly pack and its `Facades`, the
`Microsoft.NETCore.App` 10.0 shared runtime and `ILSpy/bin/Release/net10.0`. Then run
`cargo bench --bench stages`.

A Release `build.ps1` restores the Release package assets; run `restore.ps1` before the next
Debug build.

## Item 89

Item 89 was a fork to present to the human with the measured numbers: port the disassembler,
port `CSharpOutputVisitor`, a Rust PDB reader and resolver, and `WholeProjectDecompiler`. This
document replaces it. With the engine at 1.5x to 7x the managed CPU time, none of those ports
pays off before the optimizations above. Porting more stages would also not make the
existing ones faster.
