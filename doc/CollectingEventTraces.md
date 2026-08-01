# Collecting event traces

ILSpy emits performance trace events through two `EventSource` providers:

| Provider | Instruments |
| --- | --- |
| `ICSharpCode.Decompiler` | Decompilation pipeline: per-type/per-member decompilation, type system init, assembly resolution, whole-project decompilation, per-transform timing |
| `ICSharpCode.ILSpyX` | Frontend support: assembly loading, reference resolution, search, analyzers, bundle/zip extraction, PDB loading |

Events are Start/Stop pairs, so trace viewers derive durations and nesting from the
event timestamps. Tracing is off by default and costs nothing until a session attaches.

## Keywords and levels

Enable only what you need; the keyword masks combine with bitwise OR.

`ICSharpCode.Decompiler` (all = `0x1F`):

| Keyword | Mask | Level |
| --- | --- | --- |
| Decompilation (per type/member) | `0x1` | Informational (4) |
| TypeSystem | `0x2` | Informational (4) |
| AssemblyResolver | `0x4` | Informational (4) |
| ProjectDecompiler | `0x8` | Informational (4) |
| Transforms (per IL/AST transform, high volume) | `0x10` | Verbose (5) |

`ICSharpCode.ILSpyX` (all = `0x3F`):

| Keyword | Mask | Level |
| --- | --- | --- |
| AssemblyLoad | `0x1` | Informational (4) |
| Resolver | `0x2` | Informational (4) |
| Search | `0x4` | Informational (4) |
| Analyzers | `0x8` | Informational (4) |
| Packages (per-entry extraction is Verbose) | `0x10` | Informational (4) / Verbose (5) |
| DebugInfo | `0x20` | Informational (4) |

Provider specs below use the `Name:KeywordMask:Level` syntax.

## All platforms: dotnet-trace (EventPipe)

Works identically on Windows, Linux and macOS.

```
dotnet tool install --global dotnet-trace
```

Attach to a running ILSpy instance (start ILSpy first, interact while collecting,
Ctrl+C to stop):

```
dotnet-trace ps
dotnet-trace collect -p <pid> --providers "ICSharpCode.Decompiler:0x1F:5,ICSharpCode.ILSpyX:0x3F:5"
```

Or launch ILSpy under the collector to also capture startup (assembly list loading):

```
dotnet-trace collect --providers "ICSharpCode.Decompiler:0x1F:5,ICSharpCode.ILSpyX:0x3F:5" -- <path-to>/ILSpy
```

Viewing the resulting `.nettrace` file:

- Windows: open it in PerfView (Events view) or Visual Studio.
- Cross-platform: `dotnet-trace convert --format speedscope trace.nettrace` and open
  the output at <https://speedscope.app>, or `--format chromium` for `about:tracing`
  in a Chromium browser.

## Windows alternative: ETW

PerfView (`choco install perfview`) collects the same providers over ETW, which also
gives you the Start/Stop duration pairing in the Events view:

```
PerfView "/onlyProviders=*ICSharpCode.Decompiler,*ICSharpCode.ILSpyX" run ILSpy.exe
```

Other options:

- `PerfView collect "/onlyProviders=..."` to attach machine-wide instead of launching.
- `wpr`/Windows Performance Analyzer with a custom profile listing the two providers.
