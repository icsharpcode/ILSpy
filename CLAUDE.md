# CLAUDE.md

Guidance for Claude Code (and future Claude sessions) when working on ILSpy.

## What this codebase is

ILSpy is a cross-platform .NET assembly browser / decompiler built on **Avalonia 12**, on top of the cross-platform `ICSharpCode.ILSpyX` and `ICSharpCode.Decompiler` core libraries.

## Tech stack

- **Avalonia 12** (not 11.x).
- **AvaloniaEdit** for the decompiled-code text view.
- **Dock** (wieslawsoltes/Dock) for the panel layout. NuGet id ≠ CLR namespace — `Dock.Controls.Recycling` lives in `Avalonia.Controls.Recycling`; decompile before guessing xmlns.
- **Avalonia.Xaml.Behaviors** for attached-behaviour glue.
- **Avalonia.ExtendedToolkit** (mameolan) for controls not in Avalonia core.
- **Simple** theme (not Fluent). Check `App.axaml` / csproj before assuming Fluent.
- **Microsoft.Extensions.DependencyInjection** + **System.Composition** MEF directly, with a small bridge.
- Central package management is enabled — every `PackageReference` needs a matching `PackageVersion` in `Directory.Packages.props`.
- Target framework: `net10.0` (cross-platform) for the main app. The test projects target `net11.0` (and `net11.0-windows` for tests that intentionally exercise Windows-only behaviour) so they run on the runtime the `net11.0` preview build SDK ships, without installing a separate `net10.0` runtime in CI. `TestPlugin` stays `net10.0` (it is loaded as a library by the net11 test host, so its TFM need not track the test projects').

## Project structure

Avalonia UI:
- `ILSpy/` — the Avalonia UI app
- `ILSpy.Tests/` — headless Avalonia UI tests (Avalonia.Headless.NUnit)
- `ILSpy.Tests.Windows/` — Windows-only UI tests (OS-gated; `net11.0-windows`)
- `ILSpy.ReadyToRun/` — ReadyToRun-viewer plugin.

Cross-platform core (decompiler engine + shared support):
- `ICSharpCode.Decompiler/` — core decompiler library (multi-targeted, cross-platform)
- `ICSharpCode.ILSpyX/` — shared UI-host-agnostic support library
- `ICSharpCode.BamlDecompiler/` — BAML parsing library
- `ICSharpCode.ILSpyCmd/` — CLI front-end (`ilspycmd`)
- `ICSharpCode.Decompiler.Tests/` + `ICSharpCode.Decompiler.TestRunner/` — decompiler test suite and its out-of-process runner
- `ICSharpCode.Decompiler.PowerShell/` — PowerShell cmdlets (`netstandard2.0`)

Test support:
- `TestPlugin/` — sample plugin exercising the plugin-loading system (`net10.0`)
- `TestFixtures.Resources/` — generates resource fixtures consumed by the decompiler tests

Windows-only frontends, packaging, and tests:
- `ILSpy.AddIn/`, `ILSpy.AddIn.VS2022/` — Visual Studio add-ins (`net472`)
- `ILSpy.Installer/` — WiX installer (`net472`)
- `ILSpy.BamlDecompiler.Tests/` — BAML-decompiler tests, still WPF/Windows-bound (`net11.0-windows`)

Solutions & filters: `ILSpy.sln` builds everything; `ILSpy.XPlat.slnf` is the decompiler libs + `ilspycmd` + their tests (no UI — the Linux CI target); `ILSpy.Desktop.slnf` is the UI plus its dependencies and tests; `ILSpy.Installer.sln` / `ILSpy.VSExtensions.sln` cover the legacy packaging.

## ILSpy-tests submodule

- `ILSpy-tests/` is a **git submodule** (`https://github.com/icsharpcode/ILSpy-tests`, branch `master`) holding large real-world assemblies and pre-built fixtures used by the heavyweight decompiler tests — the round-trip suite (`ICSharpCode.Decompiler.Tests/RoundtripAssembly.cs`) and a few IL-pretty cases (e.g. `FSharp/FSharp.Core.dll`).
- **It is not checked out by default**, because it is large. Tests that need it call `Assert.Ignore` when the directory is absent (see `RoundtripAssembly`/`ILPrettyTestRunner`), so the rest of the suite runs without it — a green local run does **not** mean the round-trip tests ran. To run them, populate it first: `git submodule update --init ILSpy-tests` (or clone it separately to that path).
- **It opts out of the host build settings on purpose.** The submodule ships its own `ILSpy-tests/Directory.Build.props` that sets `TreatWarningsAsErrors=false`; its mere presence also stops MSBuild's upward `Directory.Build.props` search at the submodule root, so the repo-wide warnings-as-errors (and other root build properties) don't leak into the fixtures, which intentionally contain warning-generating code. Don't delete that file or "fix" warnings inside the fixtures.
- **Bumping it** is a normal submodule pointer update: check out the desired commit inside `ILSpy-tests/`, then commit the changed submodule gitlink in the host repo (subject like "Bump ILSpy-tests: ...").

## Code conventions

- **Comments must stand on their own, with no memory of how the code was written.** A comment must make sense to someone reading the file cold -- with no knowledge of the chat, PR, commit, or agent session that produced it. Describe the code as it is now; never reference "the change", "the previous version", "the old approach", "as requested", "we discussed", "now we", a step that was removed, or anything else that only means something inside the conversation that wrote it. If you can't explain it without that context, it isn't a code comment.
- **Never undo edits you didn't make this session.** If a file carries modifications made outside the session -- by a human, or surfaced by the harness as "modified outside the session / by the user or a linter" -- treat them as intentional. Do not revert, overwrite, "clean up", or discard them, even when they look unrelated, wrong, or in the way of your change, without explicit confirmation first. Work around them or ask; never silently drop someone else's work.
- **TDD for new features.** Write the failing test, show it red, implement, show it green. Never skip the red step.
- **Strict pattern matchers.** Decompilation pattern matchers should default to `∀` over the structurally guaranteed shape — loosen only when a legitimate fixture is rejected (capture it as a regression test first).
- **No silent returns in tests.** When an expected component is missing, assert and fail. Never `return` early to bypass the assertion.
- **ASCII-only in code and comments.** Do not use non-ASCII characters (em-dash, smart quotes, arrows, math symbols, etc.) unless genuinely necessary for what the code expresses.
- **en-US English** in all source code, comments, identifiers, and log/error strings.

## Build / restore

- **Use the repo-root pwsh scripts for these tasks, not raw `dotnet` commands** -- they carry the flags that keep the repo consistent and all target `ILSpy.sln` (extra args are forwarded):
  - **restore** -> `restore.ps1`
  - **build** -> `build.ps1` (`-Configuration Debug|Release`)
  - **update deps / regenerate lock files** -> `updatedeps.ps1`
  - **format** -> `BuildTools/format.ps1`
  - **clean** -> `clean.ps1`; **publish** -> `publish.ps1`
- **Why this matters:** `restore.ps1` and `updatedeps.ps1` pass `-p:RestoreEnablePackagePruning=false`, so they keep every `packages.lock.json` whole. A bare `dotnet restore`/`dotnet build` (which restores implicitly) **prunes** the lock files -- silently stripping the transitive/all-RID/DiaSymReader entries the repo deliberately carries -- and surfaces as a spurious `packages.lock.json` diff. Restore with `restore.ps1`, then build with `build.ps1 --no-restore`; if a bare build ever leaves a pruned `packages.lock.json` modified, discard it (`git checkout -- <path>`).
- **Every project generates a `packages.lock.json`** (`RestorePackagesWithLockFile` is set in the root `Directory.Build.props`); CI NuGet caching keys off these. After adding or bumping a `PackageReference`/`PackageVersion`, regenerate the lock files with `updatedeps.ps1` (i.e. `dotnet restore ILSpy.sln --force-evaluate -p:RestoreEnablePackagePruning=false`) and commit them. The core libraries additionally set `RestoreLockedMode`, so a plain restore there fails until the lock file is refreshed.
- The pre-commit hook runs `dotnet format` on the **whole solution** -- it IS the formatter. **Always let the hook run; never commit `.cs` with `--no-verify`.** Bypassing it lands unformatted code and forces history-wide reformat rebases later. `--no-verify` is acceptable only for commits that touch no `.cs` (e.g. `.yml`/`.md`-only).
- **Line endings are a Windows-only concern.** The repo is `* text=auto`, so blobs are stored LF and git renormalizes on commit. On **Windows**, save new files CRLF so the hook's `git add -u` doesn't churn EOLs. On **Linux/macOS**, leave new files LF and do **not** `unix2dos` -- forcing CRLF there makes the whole working tree show as phantom-modified in `git status` (and is normalized back to LF on commit anyway).
- **Partial commits need stash-and-pop.** The format hook only auto-formats when the staged set equals the working tree. Stash unstaged remainder before committing a subset.

## Commit workflow

- **Subject** is a succinct phrase describing the change. Target <= 72 chars. No area prefix; the subject itself should make the change clear.
- **Body explains the *why*** and the non-diff context only: the constraint, the prior incident, the decision, what was tried and rejected, the invariant that motivated the change. Keep it short — one short paragraph is usually enough. The diff already shows the *what* — don't restate it, and don't enumerate per-file changes.
- `Fix #NNNN: ...` closes an issue. `#NNNN` references one without closing.
- **Small follow-up fixes against a commit still on the branch get squashed back** via `git commit --fixup` + `git rebase --autosquash`, not appended as "fix X" commits.
- **en-US English** in subject and body. ASCII-only unless a non-ASCII character is genuinely required for what the message describes.
- **AI attribution: use `Assisted-by:`, not `Co-Authored-By:`.** Following the Linux kernel's coding-assistants guidance (https://docs.kernel.org/process/coding-assistants.html#attribution), an AI-assisted commit ends with a trailer of the form `Assisted-by: AGENT_NAME:MODEL_VERSION:HARNESS` — agent, model id, and the harness that ran it, colon-separated, e.g. `Assisted-by: Claude:claude-opus-4-8:Claude Code`. Use the session's actual model id and harness. Don't list analysis/build tools. Do **not** add a `Co-Authored-By:` line for the AI, and an AI agent **must not** add a `Signed-off-by:` (only a human can certify the DCO).

## Test discipline

- Always run the test suite with `--report-trx` so failures survive: `dotnet test --solution ILSpy.sln --report-trx` (the repo pins Microsoft.Testing.Platform in `global.json`; the bare `dotnet test <sln>` form is the old VSTest syntax). Don't dismiss failures as flaky without first reproducing in isolation, then running repeatedly.
- The decompiler test suite (test kinds, fixture structure, how to write tests, the compiler-matrix model) is documented in [ICSharpCode.Decompiler.Tests/CLAUDE.md](ICSharpCode.Decompiler.Tests/CLAUDE.md).
- After matcher / rewriter edits, **run the relevant tests, not just the build.** `dotnet build` green ≠ behaviour correct.

## Investigating dependencies

- **Decompile NuGet packages with this repo's `ilspycmd`** to inspect dependency internals — don't grep binaries.

## Onboarding new contributors

If you're picking up this codebase fresh, read `Program.cs` → `App.axaml.cs` → `Views/MainWindow.axaml.cs` to trace startup, then `AssemblyTree/AssemblyListPane.axaml.cs` and `TextView/DecompilerTextView.axaml.cs` for the two main panes. The MEF composition graph in `AppEnv/AppComposition.cs` wires the rest.
