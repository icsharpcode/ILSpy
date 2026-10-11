# allocview

Where do the bytes go during a decompiler run? This answers it two ways: a flamegraph of
allocations by call path, and a text summary by area of the decompiler (ILAst, type system,
C# AST, ...).

Nothing has to be built into ILSpy for this. The .NET runtime writes the trace itself when a few
environment variables are set, and `allocview.cs` turns that trace into something you can look at.

## Prerequisites

- The .NET SDK from the repo's `global.json`, or newer.
- Network access on the first run: `allocview.cs` restores one NuGet package
  (`Microsoft.Diagnostics.Tracing.TraceEvent`) to read the trace.
- Something to profile. `ilspycmd` from this repo works well:
  `dotnet build ICSharpCode.ILSpyCmd/ICSharpCode.ILSpyCmd.csproj -c Release`

## 1. Collect a trace

Set three variables and run the program. The trace is written when the process **exits**.

Linux/macOS:

```bash
DOTNET_EnableEventPipe=1 \
DOTNET_EventPipeConfig=Microsoft-Windows-DotNETRuntime:0x1:5 \
DOTNET_EventPipeOutputPath=$PWD/alloc.nettrace \
  ICSharpCode.ILSpyCmd/bin/Release/net10.0/ilspycmd SomeAssembly.dll -o /tmp/out
```

Windows (PowerShell):

```pwsh
$env:DOTNET_EnableEventPipe=1
$env:DOTNET_EventPipeConfig="Microsoft-Windows-DotNETRuntime:0x1:5"
$env:DOTNET_EventPipeOutputPath="$PWD\alloc.nettrace"
ICSharpCode.ILSpyCmd\bin\Release\net10.0\ilspycmd.exe SomeAssembly.dll -o C:\temp\out
Remove-Item Env:DOTNET_EnableEventPipe   # or the next .NET process you start is traced too
```

`0x1` is the GC keyword and `5` is verbose; that pair is what turns on the allocation events.
Nothing else in the trace is used.

This works for **any** .NET process, not just `ilspycmd` - the same variables in front of the
ILSpy UI trace a whole session (close the app to flush the file), and in front of `dotnet test`
they trace a test run. If you would rather use a tool, this is equivalent:

```bash
dotnet-trace collect --profile gc-verbose -o alloc.nettrace -- <program> <args>
```

## 2. Convert it

```bash
cd TestTools
dotnet run allocview.cs -- ../alloc.nettrace
```

Options: `-o <file>` for the output path (default: the trace's name with `.speedscope.json`),
`--types <n>` for how many types the text summary lists (default 25).

Expect roughly: a 39 MB trace becomes a 17 MB `.etlx` index beside it and a 12 MB
`.speedscope.json`. The `.etlx` is where resolving call stacks happens; it is kept so a second
run is fast, and it is safe to delete along with the trace.

## 3. Look at it

The text summary is printed straight away:

```
by area
   34.2%        1.4 GB  Type system
   24.4%      995.5 MB  BCL
   22.0%      897.0 MB  ILAst
    ...
top 25 types
   10.2%      413.7 MB  <ICSharpCode-Decompiler-TypeSystem-INamespace-get_Types>d__18
    6.2%      253.8 MB  ChildrenEnumerator[]
    ...
```

For the flamegraph, open <https://www.speedscope.app> and drop the `.speedscope.json` on it. It is
a static page: the file is read in your browser and not uploaded anywhere.

Reading it:

- The horizontal axis is **bytes allocated**, not time. The profile declares `unit: "bytes"`, so
  speedscope labels the values as sizes.
- **Left Heavy** is the tab you want: it merges identical call paths, so the widest bar is the
  biggest allocator. **Time Order** keeps the samples in the order they happened, which is worth a
  look only to see one phase of the run dominating another.
- Each stack ends in a frame like `ILAst: ICSharpCode.Decompiler.IL.Block[]`, so the widest bars
  read as "this call path allocated this much of this type".
- Click a frame to zoom; the **Sandwich** tab ranks individual frames by total bytes underneath
  them, which is the quickest way to find the one method responsible for a hot area.

## How allocations are attributed to an area

By the decompiler namespace mentioned anywhere in the type's name, so a
`Stack<ILInstruction.ChildrenEnumerator>` counts as ILAst work rather than as a collection - the
generic argument is what the allocation exists for. The runtime reports nested and
compiler-generated types without a namespace (`ChildrenEnumerator[]`, `<get_Descendants>d__116`),
and those are attributed instead by the innermost decompiler frame on the stack, which names the
code that wanted the object.

The area list lives at the top of `allocview.cs`; add a prefix there if you want a subsystem split
out on its own.

## Caveats

- **These are samples.** The runtime raises one event per ~100 KB allocated per type. Proportions
  are meaningful; absolute totals are an estimate, and a type that never reaches 100 KB may not
  appear at all. (On the run above the sampled total came within a percent of the GC's own
  `GetTotalAllocatedBytes`, so the estimate is a good one - but do not quote the bytes as exact.)
- Allocation events say nothing about **lifetime**. A large number here can be short-lived garbage
  that costs a gen-0 collection and nothing else. For what is actually retained, take a heap
  snapshot instead (`dotnet-gcdump collect -p <pid>`).
- Tracing slows the process down and the trace grows with the run, so profile one assembly rather
  than a corpus.

## Other tools that read the same trace

- **PerfView** (Windows) opens the `.nettrace` directly; its *GC Heap Alloc Ignore Free* view is
  the same data with a full caller tree. It knows nothing about ILSpy's areas.
- **Visual Studio** (Windows) opens `.nettrace`, and its .NET Object Allocation Tracking profiler
  measures the same thing without a trace file.
- **dotMemory** / Rider has allocation profiling and runs on Linux.

`dotnet-trace convert --format Speedscope` also exists, but it produces a profile weighted by
**time**, not by bytes - which is why this tool writes the speedscope file itself.
