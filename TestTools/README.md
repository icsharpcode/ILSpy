# TestTools

Standalone tools that run the decompiler over real-world assemblies, to find defects the
in-repo test suite cannot: it decompiles fixtures we wrote, these decompile what the world ships.

| tool | question it answers |
|---|---|
| `nugetfuzz.cs` | Does the decompiler *crash* on real code? (asserts, exceptions, IL warnings) |
| `decompdiff.cs` | Did a change make the *output* better or worse? (readability across two builds) |
| `nuget-top.ps1` | Where do I get a corpus? (downloads the most-downloaded packages) |

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

## nuget-top

Downloads the most-downloaded packages on nuget.org, with their dependency closures, and writes the
list of lib directories they resolved to - a ready-made corpus for either tool:

```pwsh
./nuget-top.ps1 -Count 200
./nuget-top.ps1 -Count 50 -Skip 200      # extend an existing corpus further down the ranking
./nuget-top.ps1 -Count 200 -ListOnly     # just the ids, download nothing
```

The ids come from an empty query against the search service, which orders by download count. The
download itself is `nugetfuzz --download-only`, so package selection, TFM matching and the
dependency walk behave exactly as they do in a sweep - only the decompiling is skipped.

The corpus is written as a list of directories rather than one root, because a package already
restored on this machine is used from the machine-wide NuGet cache instead of being copied. Pass the
list with `@`, which both tools understand and which needs no help from the shell:

```pwsh
dotnet run decompdiff.cs -- --old master --new my-branch -o report @crawl/top-200.corpus.txt
```

## decompdiff

Decompiles a corpus with **two** builds of `ICSharpCode.Decompiler` side by side (separate
`AssemblyLoadContext`s, driven through the stable `CSharpDecompiler(string, DecompilerSettings)`
API, so arbitrary version pairs work) and reports how the output differs. Correctness is what the
round-trip tests check; this checks readability. Exit code 1 means the new side has errors the old
side did not.

```pwsh
dotnet run decompdiff.cs -- --old master --new fix/my-branch -o report ~/.cache/nugetfuzz
dotnet run decompdiff.cs -- --old ../../ILSpy-master --new . -o report ~/.cache/nugetfuzz
dotnet run decompdiff.cs -- --old v9.1.dll --new v11.dll --refs <dir> corpus.dll
```

A corpus argument is a dll, a directory scanned recursively for dlls, or `@file` listing either one
per line (`#` comments allowed).

An `--old`/`--new` argument is a path to `ICSharpCode.Decompiler.dll`, an ILSpy checkout, or a
commit-ish. A checkout is restored and built in Release on demand. **Watch the timestamp in the
header line**: an existing Release build is reused as-is; pass `--build` to force a rebuild.

A commit-ish (branch, tag, sha, `FETCH_HEAD`) is resolved against the repository the tool is run
from and checked out into a worktree under `~/.cache/decompdiff/<repo>/<commit>`, so diffing two
commits needs no checkouts prepared by hand. Paths win over refs, so a branch that shares its name
with a directory has to be spelled as a path. The worktrees are kept: a rerun reuses the Release
build already in one, which is what dominates the runtime. They live outside the repository and
`git worktree remove` (or deleting the cache directory) is enough to clean them up. To diff a pull
request, fetch it first:

```pwsh
git fetch origin pull/4071/head
dotnet run decompdiff.cs -- --old origin/master --new FETCH_HEAD -o report ~/.cache/nugetfuzz
```

The report directory gets `summary.md`, a self-contained `index.html` with inline diffs, and the
changed types dumped under `old/` and `new/` for `git diff --no-index report/old report/new`.

Each assembly is decompiled from a staging directory holding its neighbours plus the transitive
closure of its references, found in the reference-assembly pack matching each assembly's own target
framework (`Microsoft.NETCore.App.Ref` and the Windows-desktop / ASP.NET packs for .NET Core targets,
`Microsoft.NETFramework.ReferenceAssemblies` for classic net4x), then `--refs` directories and the
NuGet cache. Getting the pack right matters as much as it does for `nugetfuzz`: binding a net9.0
assembly against the net4x packs resolves mscorlib but not `ValueTask` or the async method builders,
and every async method then decompiles as a raw state machine. Both sides read the same staging
directory, so anything still unresolved degrades them identically and the diff stays meaningful.
Unresolved references are listed in the summary.

## Windows notes

Both tools run on Windows, with two differences worth knowing:

- Staging uses symbolic links, which Windows only grants to elevated processes or with Developer
  Mode enabled. Without it, the files are copied instead - correct, just slower and more disk.
- Report file names are truncated with a hash appended when a namespace-qualified type name would
  push the path past the 260-character limit that applies unless long paths are enabled.

## Ignored output

`crawl/`, `logs/`, generated reports and `bin/`/`obj/` are gitignored: they are run artifacts that
grow into the gigabytes.
