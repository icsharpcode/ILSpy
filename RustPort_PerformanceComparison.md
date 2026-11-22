# Rust engine vs. managed decompiler: performance and memory

Item 88 of `RustPort_AgentInstructions.md`. Measured 2026-11-22 on branch
`christophwille/rustafari` at 1b7fc69a9, with the methodology of `RustPort_HumanLecture.md`
section 6 and the tooling it describes (`TestTools/perfcompare.ps1`, `decompdiff --perf --side`,
`ilspy_stats`, `rust/ilspy-decompiler/benches/stages.rs`).

## Summary

- **CPU: the engine is 1.9x to 11x slower than the managed decompiler.** It is 1.9x slower on
  `System.Reflection.Metadata`, 5.1x on `ILSpy.dll`, 9.6x on `ICSharpCode.Decompiler.dll` and
  11.2x on the top-200 NuGet corpus.
- **The time is spent inside Rust.** Rust accounts for 91 to 92 percent of end-to-end time,
  except on `System.Reflection.Metadata`, where it is 77 percent. Marshaling and
  materialization take 3 to 9 percent, well under the 20 percent the methodology names as the
  threshold for optimizing them first. Callbacks into the host take 2 to 3 percent.
- **The slowdown is broad, not a few hot types.** The 10 slowest types explain only 21 to 44
  percent of the extra time. The median per-type ratio grows with the size of the input and its
  reference closure: 1.4x, 4.2x, 7.5x. Over the corpus the median per-assembly ratio is 10.9x.
- **The engine allocates a lot of short-lived memory.** Each request allocates about 0.6 to 0.8
  MB in about 5,900 allocations: 1.46 TB in 11 billion allocations over the corpus.
- **Memory: peak RSS is close on single assemblies.** It is 3 to 19 percent higher. Over a
  long corpus run it is 2.6x higher (505 MB against 1,291 MB). That peak is one large
  assembly's session (Rust peak live 1,040 MB); live Rust memory at the end of the run is
  1.4 MB, so nothing leaks.
- **The managed heap is not relieved.** The host allocates 5 to 7 percent more managed memory
  with the engine than without it, because the type system, the resolver behind the callbacks,
  AST materialization and output stay managed.
- **The engine's ILAst stages alone cost as much as the whole managed decompile.** On
  `ICSharpCode.Decompiler.dll`, IL reading takes 5.6 s and the IL transforms 7.0 s; the managed
  decompile takes 12.1 s in total. That is 13 of the 106 s inside Rust; the other 93 s are in
  stages the benches cannot isolate (see "Where the time goes").

The port is functionally complete: every test runs strict on the engine. Performance is the
open item. The port has not been profiled or optimized yet, and the numbers point at several
specific causes rather than at the language (see "What to optimize first").

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
| Runs | Reference inputs: 3 runs per side, median. Corpus: 1 run per side (about 50 minutes for the engine side alone; across 39,011 types the totals and percentiles barely move between runs) |

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

CPU is wall time of the type decompiles at `-j 1`, so it equals CPU time on one core.

| Input | Managed CPU total | Engine CPU total | Ratio | Marshaling share | Managed peak RSS | Engine peak RSS | Rust peak live |
|---|---|---|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | 12.1 s | 116.8 s | 9.6x | 3.7 % | 215 MB | 222 MB | 17 MB |
| System.Reflection.Metadata.dll | 4.6 s | 8.5 s | 1.9x | 8.6 % | 119 MB | 142 MB | 8 MB |
| ILSpy.dll (net10) | 7.8 s | 39.6 s | 5.1x | 4.6 % | 225 MB | 247 MB | 25 MB |
| top-200 NuGet corpus | 259 s | 2,894 s | 11.2x | 3.0 % | 505 MB | 1,291 MB | 1,040 MB |

Per-type time and managed allocation:

| Input | Types | Managed P50 | Managed P95 | Engine P50 | Engine P95 | Managed allocated, managed / engine side |
|---|---|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | 902 | 5.2 ms | 52 ms | 37.0 ms | 567 ms | 3,572 MB / 3,838 MB |
| System.Reflection.Metadata.dll | 406 | 4.6 ms | 41 ms | 7.2 ms | 77 ms | 588 MB / 627 MB |
| ILSpy.dll (net10) | 521 | 6.1 ms | 55 ms | 24.8 ms | 349 ms | 1,837 MB / 1,966 MB |
| top-200 NuGet corpus | 39,011 | 2.2 ms | 22 ms | 15.2 ms | 251 ms | 85,465 MB / 89,863 MB |

## Engine counters

The engine-side numbers are from `RustStats` (the counting allocator and the timers in the ffi
crate's `run`), first run of each input. They are deltas after the warm-up, except the live and
peak bytes.

| Counter | ICSharpCode.Decompiler.dll | top-200 corpus |
|---|---|---|
| Requests (shim to engine) | 56,014 | 1,854,804 |
| FFI calls (including `buf_free`) | 111,128 | 3,670,932 |
| Time in requests, end to end | 113.3 s | 2,791 s |
| Time inside Rust, without callbacks | 106.4 s (94 %) | 2,651 s (95 %) |
| Callbacks into the host | 2.9 s (3 %) | 53 s (2 %) |
| Marshaling and materialization | 4.0 s (4 %) | 87 s (3 %) |
| Request bytes / response bytes | 12.9 MB / 30.9 MB | 309 MB / 798 MB |
| Rust bytes allocated | 32.1 GB | 1,464 GB |
| Rust allocations | 328 million | 10,968 million |
| Per request: bytes / allocations | 574 KB / 5,852 | 789 KB / 5,913 |
| Rust live at end / peak live | 4 MB / 17 MB | 1.4 MB / 1,040 MB |

The wire is small: about 0.8 KB per request in and 0.5 KB out. Moving data across the boundary
is therefore not the problem. The problem is what the engine does per request, which allocates
about a thousand times as much as it sends back.

## Where the time goes

Criterion benches (`cargo bench --bench stages`) time each engine stage alone over the three
reference inputs. A stage's input is produced outside the timed part. Medians:

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
  so that row is not representative of the shim's seam 2 work.
- The seam 2 syntax-tree transforms need a tree from the host and are not benched.

Reading the two together:

1. **IL read is far too slow.** 360 to 600 us per body is about the time the whole managed
   pipeline takes for a whole type at P50 (2 to 6 ms with 10 to 20 bodies). IL read grows with
   the reference closure: `ILSpy.dll` is 602 us/body against 164 us/body for
   `System.Reflection.Metadata`. That matches the per-type ratios above, so the reader most
   likely does type-system work, such as resolving signatures across modules, per body instead
   of once per session.
2. **The ILAst stages explain only part of the gap.** On `ICSharpCode.Decompiler.dll`, IL read
   plus transforms is 12.7 s of the 106 s inside Rust. The rest is in what the shim asks for
   beyond that: seam 2 expression building with the resolver, the syntax-tree round trip of the
   16 AST transforms, and the declaration and member-walk requests. It is also possibly in
   repeated work: there are 56,014 requests for 15,620 bodies, about 3.6 per body, and 111,128
   FFI calls. The benches cannot split this; a sampling profile of one `decompdiff --side new`
   run can.
3. **Allocation churn is a cost of its own.** About 5,900 allocations per request at
   roughly 100 bytes each means the allocator is on the hot path. In the managed decompiler the
   same objects are bump-allocated and collected in gen 0. Arena allocation per request, or
   reusing the per-session structures, is the usual fix.

### Slowest types by extra time (first run)

| Input | Type | Managed | Engine | Extra |
|---|---|---|---|---|
| ICSharpCode.Decompiler.dll | `CSharp.ExpressionBuilder` | 406 ms | 5,425 ms | 5,020 ms |
| ICSharpCode.Decompiler.dll | `Documentation.IdStringProvider` | 71 ms | 2,631 ms | 2,560 ms |
| ICSharpCode.Decompiler.dll | `IL.ILAmbience` | 55 ms | 2,440 ms | 2,385 ms |
| ICSharpCode.Decompiler.dll | `CSharp.CSharpDecompiler` | 212 ms | 2,213 ms | 2,001 ms |
| ICSharpCode.Decompiler.dll | `CSharp.OutputVisitor.TextWriterTokenWriter` | 32 ms | 1,883 ms | 1,851 ms |
| System.Reflection.Metadata.dll | `Ecma335.MetadataBuilder` | 305 ms | 734 ms | 428 ms |
| ILSpy.dll | `TextView.DecompilerTextView` | 278 ms | 1,748 ms | 1,470 ms |
| ILSpy.dll | `SessionSettings` | 52 ms | 1,238 ms | 1,186 ms |

Some small types are 30 to 60 times slower: `IdStringProvider`, `ILAmbience`,
`TextWriterTokenWriter` and `SessionSettings`. They have few bodies but touch many types of the
reference closure. That again points at per-request type-system or resolver work rather than at
body size.

### Corpus: slowest assemblies by extra time

| Assembly | Managed | Engine | Ratio |
|---|---|---|---|
| DocumentFormat.OpenXml.dll | 24.9 s | 264.4 s | 10.6x |
| SixLabors.ImageSharp.dll | 10.6 s | 164.4 s | 15.6x |
| Microsoft.Azure.Cosmos.Client.dll | 10.2 s | 146.0 s | 14.3x |
| BouncyCastle.Cryptography.dll | 15.4 s | 138.3 s | 9.0x |
| MongoDB.Driver.dll | 9.4 s | 130.5 s | 13.8x |
| Microsoft.EntityFrameworkCore.dll | 8.8 s | 112.7 s | 12.9x |

Spread of the per-assembly ratio over the 108 assemblies whose managed time is over 200 ms:
minimum 4.7x, first quartile 8.2x, median 10.9x, third quartile 14.2x, maximum 37.6x.

## Memory

- **Peak RSS, single assembly:** +3 % (`ICSharpCode.Decompiler.dll`), +19 %
  (`System.Reflection.Metadata.dll`) and +10 % (`ILSpy.dll`). The Rust heap peaks at 8 to 25 MB
  and adds the native library's own pages (5.8 MB image) on top of an unchanged managed heap.
- **Peak RSS, corpus:** 1,291 MB against 505 MB. Rust peak live reaches 1,040 MB, and live
  bytes are back to 1.4 MB at the end. So it is a peak while one session is alive, not a leak:
  the largest assemblies (`DocumentFormat.OpenXml.dll` and similar) hold their per-session
  structures in Rust for the whole assembly. In the UI, with one assembly decompiled at a time,
  this shows up as a few hundred MB extra while a large assembly is open.
- **Managed allocation:** 5 to 7 percent more with the engine. Nothing managed is saved, because
  the type system, the resolver, materialized ASTs, the transforms that stay managed and the
  output visitor all run in .NET either way. The materialized trees and the wire buffers are
  the extra.

## Marshaling

The marshaling share is 3.0 to 8.6 percent: request time minus time inside Rust minus
callbacks. It is under the 20 percent threshold, so string-table reuse and pre-sized node lists
are not the priority. It is highest on `System.Reflection.Metadata.dll`, where the engine itself
is fastest.

## What to optimize first

Ranked by expected gain. None of this has been done; each is a separate piece of work.

1. **Profile one engine run.** Run `decompdiff --side new` on `ICSharpCode.Decompiler.dll` under
   a sampling profiler (ETW via `wpr` on Windows, `perf` or `samply` on Linux) with symbols for
   `ilspy_decompiler_ffi`. Every point below is an inference from counters and benches. A
   profile replaces them with facts.
2. **Make IL read per-session, not per-body, for type-system work.** Cache signature decoding,
   member and type resolution across bodies of one session. The bench gives a direct
   before/after number (360 to 600 us per body now, against a P50 of 2 to 6 ms for a whole
   type in the managed decompiler).
3. **Cut repeated work per body.** There are 3.6 requests and 7 FFI calls per body. Check
   whether a body is read or transformed more than once (seam 1 prefix, seam 2 body, lambdas
   and local functions reread for each containing method), and whether the 16 syntax-tree
   transforms could run in one round trip per type.
4. **Reduce allocation churn.** Use an arena or bump allocator per request, reuse buffers
   across requests of a session, and intern strings once per session. The counting allocator
   gives the before/after number.
5. **Build settings.** `codegen-units = 1` and `lto = "thin"` or `"fat"` in the `ship` profile
   usually give 10 to 20 percent. They are cheap to try, but small next to the points above.
6. **Bound the session's peak on huge assemblies.** Free per-type structures after each type
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

## Item 89

Item 89 was a fork to present to the human with the measured numbers: port the disassembler,
port `CSharpOutputVisitor`, a Rust PDB reader and resolver, and `WholeProjectDecompiler`. This
document replaces it. With the engine at 2x to 11x the managed CPU time, none of those ports
pays off before the optimizations above. Porting more stages would also not make the
existing ones faster.
