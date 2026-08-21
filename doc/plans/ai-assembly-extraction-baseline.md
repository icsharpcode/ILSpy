# AI Assembly Extraction — Baseline Verification

> Task 0.2 of `ai-assembly-extraction-implementation-plan.md`. Recorded before any implementation source moved.

## Environment

| Item | Value |
|---|---|
| OS | macOS 25.5.0 (Darwin), arm64 (Mac mini) |
| SDK | `11.0.100-preview.7.26381.103` (matches `global.json`) |
| Commit | `5b86e9401` (manifest commit; tree = `94ae3d3f0` + manifest doc) |

## Results

| Step | Command | Result |
|---|---|---|
| Restore (locked) | `dotnet restore ILSpy.Desktop.slnf --locked-mode` | Success; **no lock-file changes** (`git status` clean for all `packages.lock.json`) |
| Build | `dotnet build ILSpy.Desktop.slnf --configuration Release --no-restore` | Success, **0 warnings, 0 errors** (~21 s) |
| ILSpyX AI tests | `dotnet test --project ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --configuration Release --no-build --filter "FullyQualifiedName~AI"` | **Passed**: 253 total — 251 succeeded, 0 failed, 2 skipped (~5 s) |
| Desktop AI/settings tests | `dotnet test --project ILSpy.Tests/ILSpy.Tests.csproj --configuration Release --no-build --filter "FullyQualifiedName~AI|FullyQualifiedName~AISettings"` | **Passed**: 35 total — 34 succeeded, 0 failed, 1 skipped (~5 s) |

## Pre-existing platform gates (not failures)

- `ICSharpCode.ILSpyX.Tests/AI/SecureKeyStorageSmokeTests.cs` carries `[Platform(Include = ...)]` attributes per host: the `Win` and `Linux` variants skip on macOS (the 2 skipped above); the `MacOsX` Keychain variant runs here and passed. CI executes the Windows leg in the Windows job only.
- The desktop suite runs as `net11.0` (`ILSpy.Tests.dll`); the ILSpyX suite as `net10.0`. Both pass on arm64.

## Interpretation

The baseline is green on macOS/arm64 for everything this migration can observe locally. There are **no pre-existing failures** to distinguish from migration regressions on this host. Windows-only behavior (DPAPI smoke tests, `ExportAnalyzerAttributeTests` inventory — executed in the Windows CI solution-wide test run) is not exercised locally; the plan's Task 3.3 updates that test when `AISecurityAnalyzer` moves.
