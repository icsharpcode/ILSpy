# AI Assembly Extraction — Final Validation

Recorded on 2026-08-21 after the extraction implementation was completed.

## Environment

- macOS 25.5.0, arm64
- .NET SDK `11.0.100-preview.7.26381.103` (`global.json`)
- Locked restore/build commands run from the repository root

## Final restore and build

| Command | Result |
|---|---|
| `dotnet restore ILSpy.XPlat.slnf --locked-mode` | Passed; no lock-file changes after the final restore |
| `dotnet restore ILSpy.Desktop.slnf --locked-mode` | Passed; no lock-file changes after the final restore |
| `dotnet build ILSpy.XPlat.slnf --configuration Release --no-restore` | Passed, 0 errors |
| `dotnet build ILSpy.Desktop.slnf --configuration Release --no-restore` | Passed, 0 errors |

## Tests

| Project | Result |
|---|---|
| `ICSharpCode.ILSpy.AI.Tests` | **205 succeeded, 0 failed, 2 platform-gated skips** (207 total) |
| `ICSharpCode.ILSpy.AI.Decompiler.Tests` | **55 succeeded, 0 failed, 0 skipped** |
| `ILSpy.Tests` | **1,240 succeeded, 0 failed, 4 pre-existing platform/performance skips** (1,244 total) |
| `ICSharpCode.ILSpyX.Tests` | **1 succeeded, 0 failed, 0 skipped** — the former AI tests were moved into the two new module projects; this retained assembly smoke test keeps the solution-wide test target non-empty. |

The desktop suite includes the extracted-AI composition, settings adapter, analyzer registry, chat/output, commands, rename, markdown, annotation, and options coverage. The new module suites include provider/configuration/credential/prompt tests and decompiler context, explanation, rename, security, search, and annotation tests.

## Package boundary inspection

`dotnet pack` succeeded for both new packages:

- `ICSharpCode.ILSpy.AI.11.0.0.9632.nupkg`
- `ICSharpCode.ILSpy.AI.Decompiler.11.0.0.9632.nupkg`

The AI package nuspec contains provider/configuration dependencies only (`Markdig`, `Microsoft.Extensions.Logging.Abstractions`, `System.Composition.AttributedModel`, `YamlDotNet`, plus the framework-provided dependency pin). It has no `ICSharpCode.Decompiler`, `ICSharpCode.ILSpyX`, Avalonia, Dock, or desktop dependency.

The decompiler package nuspec depends on `ICSharpCode.Decompiler` and `ICSharpCode.ILSpy.AI` and contains no ILSpyX, Avalonia, Dock, or desktop dependency. Prompt assets are included in the AI package and embedded fallback source is compiled into the assembly.

## Source-boundary audit

- No `ICSharpCode.ILSpyX/AI` implementation source remains for the scoped portable or decompiler-aware AI features.
- No `ICSharpCode.ILSpyX/Annotations` scoped AI annotation source remains.
- No production/test source imports `ICSharpCode.ILSpyX.AI` or `ICSharpCode.ILSpyX.Annotations`.
- `ICSharpCode.ILSpyX.csproj` no longer references either extracted AI project.
- `ICSharpCode.ILSpy.AI` has no product reference to the decompiler, ILSpyX, or desktop.
- `ICSharpCode.ILSpy.AI.Decompiler` references only `ICSharpCode.ILSpy.AI` and `ICSharpCode.Decompiler` among product projects.
- Avalonia views, controls, dialogs, view models, options panels, desktop commands, context-menu entries, and `AISelectionSettingsHost` remain in `ILSpy`.
- `AISecurityAnalyzerAdapter` preserves the `Security Risks (AI)` analyzer export through the desktop analyzer registry.
- `RenameAnnotationTransform` retains its original type name and manual registration path.

## Platform/manual caveats

- macOS execution verified the Keychain path; Windows DPAPI and Linux Secret Service smoke paths remain platform-gated for their existing CI hosts.
- No live provider, production endpoint, API key, or external AI service was contacted. Manual interactive workflow limitations and exact actions are documented in `ai-assembly-extraction-manual-verification.md`.
- `.zcode/` remains an existing untracked workspace directory and was not modified or committed.
