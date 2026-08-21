# AI Assembly Extraction — Desktop Smoke Verification

Recorded on 2026-08-21 from the macOS/arm64 development host.

## Automated smoke coverage

- `dotnet build ILSpy.Desktop.slnf --configuration Release --no-restore` passes with zero warnings and errors.
- `AICompositionTests` resolves `AISelectionService`, `SecureKeyStorage`, and `IAIProviderFactory` through the real `AppComposition` container and verifies shared lifetime behavior.
- Desktop AI/settings tests pass, including the portable settings model adapter, chat/output surfaces, rename UI models, markdown editor, and composition paths.
- The analyzer registry test verifies the desktop `AISecurityAnalyzerAdapter` is exported with header `Security Risks (AI)`.
- The generated prompt output contains all eight prompt files plus `README.md` next to the desktop output.

## Manual workflow status

The following workflows remain platform/manual verification items and were not executed against a live provider on this host:

| Workflow | Status | Notes |
|---|---|---|
| AI settings profile create/edit/reorder/select/delete and restart persistence | Not run manually | Covered by model, adapter, and desktop settings tests; requires interactive desktop session for visual confirmation. |
| API-key omission from settings XML | Automated | `AISettingsModelTests` and `AISettingsSectionTests` assert secret values never appear in serialized XML. |
| Provider connection failure/success | Not run | Requires a controlled test endpoint; no production endpoint or credential is used. Existing provider tests use fake HTTP handlers. |
| Invalid non-loopback plain HTTP rejection | Automated | Existing OpenAI provider validation tests remain green after extraction. |
| Chat pane normal message and streamed rendering | Automated regression | Existing desktop chat/output/feature-command tests pass; no live provider session was used. |
| `/explain`, `/rename`, `/summary`, `/audit` routing | Automated regression | Existing desktop command and context-menu tests pass; live provider output was not exercised. |
| Context/menu visibility and rename overlay behavior | Automated regression | Existing desktop menu, language, and annotation tests pass; `RenameAnnotationTransform` remains manually registered and metadata is not mutated. |
| External prompt reload behavior | Build/output verified | Prompt files are copied beside the application and embedded fallback is generated in `ICSharpCode.ILSpy.AI`; live restart/reload observation remains manual. |

## Platform caveats

- This host is macOS 25.5.0 arm64. Windows DPAPI secure-storage smoke paths and Windows-only analyzer/desktop behavior require the Windows CI job; Linux Secret Service behavior requires the Linux CI job.
- No external AI provider, API key, or production service was contacted during verification.
