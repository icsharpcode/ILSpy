# TestTools

Two standalone tools that run the decompiler over real-world assemblies, to find defects the
in-repo test suite cannot: it decompiles fixtures we wrote, these decompile what the world ships.

| tool | question it answers |
|---|---|
| `nugetfuzz.cs` | Does the decompiler *crash* on real code? (asserts, exceptions, IL warnings) |
| `decompdiff.cs` | Did a change make the *output* better or worse? (readability across two builds) |

Both are [file-based apps](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/sdk#file-based-apps):
single `.cs` files run directly by the SDK, no project, no solution entry. They are not built by
`ILSpy.sln` and not run by CI. Requirements: the .NET SDK from `global.json` (or newer) and, for
the sweep script, PowerShell 7 (`pwsh`) - both cross-platform.

`Directory.Build.props` / `Directory.Packages.props` in this folder are intentionally near-empty:
they stop MSBuild's upward search, so the repo-wide warnings-as-errors, lock-file and central
package management settings do not reach these tools.

## nugetfuzz

Downloads NuGet packages, resolves their dependency closure, picks a matching lib TFM, and
decompiles every type of every assembly, reporting `Debug.Assert` failures, exceptions and
`//IL_xxxx:` warning comments. Exit code 0 means no finding.

```pwsh
dotnet run nugetfuzz.cs -- Newtonsoft.Json Serilog@3.1.1
dotnet run nugetfuzz.cs -- @packagelist.txt      # one id per line, # comments allowed
dotnet run nugetfuzz.cs -- --report crawl/findings.jsonl [out.html]
```

Reference assemblies are fetched as needed: `Microsoft.NETCore.App.Ref` and the Windows-desktop /
ASP.NET packs for .NET Core targets, `Microsoft.NETFramework.ReferenceAssemblies` for classic
net4x. Getting these right matters - binding a WPF assembly against the stub facades in
`NETCore.App.Ref` collapses whole type hierarchies to `Unknown` and invents hundreds of bogus
warnings, so treat a sudden warning spike as a reference problem until proven otherwise.

Environment variables: `NUGETFUZZ_VERBOSE` (per-type progress), `NUGETFUZZ_DUMP=<dir>` (write the
decompiled C#), `NUGETFUZZ_LEDGER=<file>` (append findings as JSONL instead of writing a
per-run HTML report), `NUGETFUZZ_HTML=<file>` (report path), `NUGET_PACKAGES` (package cache).

### Sweeping the whole catalog

`nugetfuzz-all.ps1` walks the nuget.org catalog and runs `nugetfuzz.cs` on every package id it
has not seen. It is resumable - the page cursor and the seen-id list live in `crawl/`, so an
interrupted sweep continues where it stopped:

```pwsh
./nugetfuzz-all.ps1                            # everything, from the cursor
./nugetfuzz-all.ps1 -MaxPages 5 -MaxPackages 50
```

Findings from every package land in `crawl/findings.jsonl`; render the aggregate at any time,
including while the sweep is still running, with `--report`. Logs of failed runs are kept in
`logs/`, successful ones are deleted. The package cache (`~/.cache/nugetfuzz`) is capped at 20 GB
by default (`-CacheCapMB`) and pruned least-recently-used, because `decompdiff` uses it as a corpus.

## decompdiff

Decompiles a corpus with **two** builds of `ICSharpCode.Decompiler` side by side (separate
`AssemblyLoadContext`s, driven through the stable `CSharpDecompiler(string, DecompilerSettings)`
API, so arbitrary version pairs work) and reports how the output differs. Correctness is what the
round-trip tests check; this checks readability. Exit code 1 means the new side has errors the old
side did not.

```pwsh
dotnet run decompdiff.cs -- --old ../../ILSpy-master --new . -o report ~/.cache/nugetfuzz
dotnet run decompdiff.cs -- --old v9.1.dll --new v11.dll --refs <dir> corpus.dll
```

An `--old`/`--new` argument is either a path to `ICSharpCode.Decompiler.dll` or an ILSpy checkout,
which is restored and built in Release on demand. **Watch the timestamp in the header line**: an
existing Release build is reused as-is; pass `--build` to force a rebuild.

The report directory gets `summary.md`, a self-contained `index.html` with inline diffs, and the
changed types dumped under `old/` and `new/` for `git diff --no-index report/old report/new`.

Each assembly is decompiled from a staging directory holding its neighbours plus the transitive
closure of its references, found in `--refs` directories, the NuGet cache and the .NET Framework
reference packs. Both sides read the same staging directory, so anything still unresolved degrades
them identically and the diff stays meaningful. Unresolved references are listed in the summary.

## Windows notes

Both tools run on Windows, with two differences worth knowing:

- Staging uses symbolic links, which Windows only grants to elevated processes or with Developer
  Mode enabled. Without it, the files are copied instead - correct, just slower and more disk.
- Report file names are truncated with a hash appended when a namespace-qualified type name would
  push the path past the 260-character limit that applies unless long paths are enabled.

## Ignored output

`crawl/`, `logs/`, generated reports and `bin/`/`obj/` are gitignored: they are run artifacts that
grow into the gigabytes.
