# Phase 1: First User-Facing Features

**Status:** Implemented (manual smoke test pending)  
**Dependencies:** Phase 0 foundation implemented; AI-filtered tests run on `net10.0`  
**Goal:** Expose the first usable AI workflow in ILSpy: configure a provider, explicitly accept the data-sharing notice, select a symbol, and request a blocking explanation.

## Scope

Phase 1 delivers the smallest end-to-end AI feature without introducing streaming UI or a dockable output pane.

### In scope

- AI Assistant options page.
- Provider, endpoint, model, context, and privacy settings editing.
- Secure API-key save/load/delete integration.
- Explicit privacy-consent gating for all AI actions.
- `Explain with AI` context-menu action for methods, types, properties, and fields.
- Basic decompilation-context construction and blocking response dialog.
- Copying the completed explanation to the system clipboard.
- Unit and UI-adjacent tests for settings, gating, orchestration, cancellation, errors, and clipboard behavior where the host permits.

### Out of scope

- Token-by-token UI streaming.
- Dockable AI output pane.
- Follow-up chat.
- Rich Markdown rendering or syntax highlighting.
- IL and call-graph extraction beyond the Phase 0 context-builder contract.
- Anthropic-specific provider implementation.
- Batch rename, security audit, semantic search, or other later-phase features.

## Existing Foundation and Integration Seams

Phase 1 builds on the implemented Phase 0 contracts. Do not duplicate provider, settings, key-storage, or context-budget logic.

| Capability | Existing seam | Phase 1 use |
|---|---|---|
| Persisted AI settings | `ICSharpCode.ILSpyX/Settings/AISettings.cs` | Bind the options page directly to the live `SettingsService.AISettings` section. |
| Settings page discovery | `ILSpy/Options/IOptionPage.cs`, `ExportOptionPageAttribute`, `OptionsPageModel` | Export the AI page through the existing named MEF contract. |
| Secure key handling | `ICSharpCode.ILSpyX/AI/SecureKeyStorage.cs` and backends | Keep the runtime key out of XML; persist only the non-secret placeholder. |
| Provider abstraction | `ICSharpCode.ILSpyX/AI/ILLMProvider.cs`, `LLMRequest`, `LLMMessage` | Construct one request and consume the provider's async chunks to completion. |
| OpenAI-compatible provider | `ICSharpCode.ILSpyX/AI/Providers/OpenAIProvider.cs` | Use for OpenAI, Ollama, and compatible endpoints already supported by Phase 0. |
| Context generation | `ICSharpCode.ILSpyX/AI/ContextBuilder.cs`, `DecompilationContext` | Build bounded Markdown context for the selected entity. |
| Context-menu discovery | `ILSpy/ContextMenuEntry.cs`, `ContextMenuProvider` | Export an entry implementing `IContextMenuEntry`; use `TextViewContext` to resolve the target. |
| Existing UI commands/bindings | Avalonia option panels and dialog/window patterns under `ILSpy/Options` and `ILSpy/Views` | Follow repository MVVM, MEF, resource, and dispatcher conventions. |

## User Flow

1. User opens **Options → AI Assistant**.
2. User selects provider, endpoint, and model, enters an API key, and chooses context options.
3. UI explains that decompiled code will be sent to the configured provider.
4. User checks the required privacy-consent checkbox.
5. User clicks **Test Connection**; the UI reports success or a safe, actionable error.
6. User right-clicks a supported symbol in the assembly tree or decompiler view.
7. User selects **Explain with AI**.
8. The dialog shows a busy state while the request completes.
9. The dialog displays the full explanation, or a non-sensitive error message.
10. User may cancel the request or copy the completed explanation to the clipboard.

## Design Decisions

### Consent is a hard gate

`PrivacyConsentAccepted` must be `true` before an AI request can start. API-key presence alone is insufficient. The settings page may be opened and edited without consent, but **Test Connection** and **Explain with AI** remain disabled until consent is accepted.

When the user unchecks consent, active or future AI actions must be disabled immediately. The implementation must not silently re-enable consent because a key or provider is configured.

### API keys never enter XML

The options page edits the runtime `AISettings.ApiKey` value, but saving settings must continue to use `SecureKeyStorage`. Persist only `ApiKeyPlaceholder` or equivalent non-secret state. Clearing the key must delete the secure-store entry and clear the placeholder.

### Phase 1 is blocking, not streaming

`ExplainDialog` consumes the provider's `IAsyncEnumerable<string>` until completion and joins chunks into one response. This intentionally leaves dispatcher-based incremental rendering for Phase 2. Cancellation must still flow through the provider and HTTP request.

### Keep provider construction behind a small seam

The viewmodel/context-menu entry must not own global `HttpClient` lifetime or duplicate endpoint rules. Use a small injectable factory/service seam so tests can supply a fake `ILLMProvider` without network access. The production composition root may construct `OpenAIProvider` from the live AI settings.

### Avoid leaking secrets in errors

Do not display or log API keys, authorization headers, complete request payloads, or raw provider error bodies without bounding and sanitizing them. User-facing errors should preserve useful status categories such as authentication, rate limit, endpoint, cancellation, and generic failure.

## Work Packages

### 1.1 AI Assistant options page

**Files:**

- `ILSpy/Options/AISettingsViewModel.cs`
- `ILSpy/Options/AISettingsPanel.axaml`
- `ILSpy/Options/AISettingsPanel.axaml.cs` (only if required by existing Avalonia conventions)
- `ILSpy/Options/AISettingsViewModelTests.cs` or the nearest existing test project
- Resource files only if visible strings require localization in this phase.

**Implementation:**

- Export the viewmodel with `[ExportOptionPage(Order = 100)]` and implement `IOptionPage`.
- Expose a `Title` of `AI Assistant` through the repository's resource conventions.
- Load the live `SettingsService.AISettings` instance in `Load`.
- Reset the AI section to built-in defaults in `LoadDefaults`; reset consent to `false`.
- Bind provider selection to `AISettings.Provider` with these Phase 1 values: OpenAI, Ollama, and Custom. Retain future provider identifiers without crashing or losing persisted values.
- Bind model and context settings directly to the live section. Use a bounded context-token control with a 4,000–128,000 range and a 32,000 default.
- Show the base URL when the selected provider needs a custom endpoint. Keep OpenAI's standard endpoint as the default.
- Use a password control for the API key. Never display the stored key after reload; show a non-secret configured indicator or placeholder.
- Provide a clear/remove-key action that deletes the secure-store entry and clears the runtime key.
- Bind `SendIL`, `SendCallGraph`, and `StreamResponses`. Keep IL and call-graph options disabled or clearly marked as future capability until the corresponding extraction exists.
- Show the privacy notice and require `PrivacyConsentAccepted`.
- Enable **Test Connection** only when provider configuration is valid and consent is accepted.
- Surface secure-store unavailability as a configuration error; do not offer an unsafe file fallback.
- Keep all edits consistent with the existing options model: changes are live and persisted by `SettingsService.Save`.

**Acceptance criteria:**

- MEF discovers the page and places it after existing option pages.
- XML round-trip preserves non-secret settings and consent.
- API key is absent from serialized XML.
- Provider changes apply correct default endpoint/model values without overwriting an explicit custom endpoint unexpectedly.
- Test Connection has loading, success, cancellation, and failure states.
- AI actions remain disabled until consent is accepted.
- Clear-key behavior removes both runtime and secure-store values.

### 1.2 Provider factory and request orchestration

**Files:**

- `ILSpy/AI/IAIProviderFactory.cs` or the smallest equivalent seam.
- `ILSpy/AI/AIProviderFactory.cs`.
- `ILSpy/AI/AIExplanationService.cs`.
- Existing AI tests under `ICSharpCode.ILSpyX.Tests/AI` or `ILSpy.Tests` as appropriate.

**Implementation:**

- Resolve the configured provider from `AISettings`; Phase 1 supports the existing OpenAI-compatible implementation for OpenAI, Ollama, and Custom endpoints.
- Reject unsupported provider identifiers with a clear configuration error instead of silently selecting a different provider.
- Build `LLMRequest` from the selected entity's bounded `DecompilationContext`.
- Use a stable system prompt that asks for a concise explanation, identifies uncertainty, and does not instruct the model to execute code.
- Snapshot all request messages before starting enumeration.
- Consume all returned chunks for Phase 1 and concatenate them in order.
- Propagate cancellation and classify expected provider failures for presentation by the dialog.
- Keep network and provider access out of Avalonia code-behind.

**Acceptance criteria:**

- Unit tests use a fake `ILLMProvider`; no test contacts a live endpoint.
- Correct model, max-token, system prompt, and context are sent to the provider seam.
- Cancellation stops enumeration and reaches the provider.
- Authentication, rate-limit, not-found, malformed-response, and generic failures map to safe user-facing messages.
- Unsupported providers fail deterministically.

### 1.3 Explain with AI context-menu entry

**Files:**

- `ILSpy/AI/ExplainContextMenuEntry.cs`
- `ILSpy/AI/ExplainContextMenuEntryTests.cs` or the nearest host test project.
- `ILSpy/ContextMenuEntry.cs` only if a shared target-resolution helper is required.

**Implementation:**

- Export `[ExportContextMenuEntry(Header = "Explain with AI", Category = "AI", Order = 1000)]` and mark the entry shared where required by the registry.
- Resolve a single target entity from `TextViewContext` for methods, types, properties, and fields.
- Support assembly-tree selection and decompiler-reference selection only when the existing context supplies a resolvable `IEntity`.
- Return invisible for unsupported targets; return disabled when target resolution succeeds but consent/configuration is incomplete.
- Check both `AISettings.PrivacyConsentAccepted` and usable provider configuration before enabling the command.
- Open `ExplainDialog` with the resolved entity and a cancellation token source.
- Do not perform network work during `IsVisible` or `IsEnabled`.

**Acceptance criteria:**

- Entry appears only for supported symbols.
- Entry is disabled when consent is false, key/configuration is missing, or secure storage is unavailable.
- Entry does not leak target source or key material while constructing menu state.
- Execute opens one dialog for one target and owns cancellation for that request.

### 1.4 Blocking explanation dialog

**Files:**

- `ILSpy/AI/ExplainDialogViewModel.cs`
- `ILSpy/AI/ExplainDialog.axaml`
- `ILSpy/AI/ExplainDialog.axaml.cs` only for unavoidable view lifecycle/clipboard hooks.
- Dialog/viewmodel tests in the host test project.

**Implementation:**

- Present target name and a clear notice that the request sends decompiled context to the configured provider.
- Start the request after the dialog opens, not while building the context menu.
- Show `Explaining…` and disable duplicate actions while waiting.
- Accumulate provider chunks into a response buffer and display the completed response in a scrollable, selectable text control.
- Provide **Cancel**; cancellation must close or return the dialog to an idle/error state without treating cancellation as a provider failure.
- Provide **Copy to Clipboard** only when a response exists. Show a short confirmation without replacing the response.
- Use dispatcher-safe updates if provider enumeration completes off the UI thread.
- Dispose/cancel request resources when the dialog closes.

**Acceptance criteria:**

- Dialog remains responsive while waiting.
- Success displays the complete response in arrival order.
- Cancellation does not show a false error and does not leave a running request.
- Copy returns the exact displayed response.
- Error text is bounded, actionable, and contains no API key or authorization data.
- Reopening the dialog starts a new request with a new cancellation scope.

### 1.5 Tests and validation

**Files:**

- `ICSharpCode.ILSpyX.Tests/AI/*`
- `ILSpy.Tests/AI/*` or the host test location selected by the existing test architecture.
- `doc/plans/phase-1-first-features.md` implementation record after completion.

**Required tests:**

- AI settings defaults, XML round-trip, consent reset, and API-key omission.
- Secure key save/load/clear integration through a fake backend.
- Provider factory selection and unsupported-provider handling.
- Explanation request construction and chunk concatenation.
- Consent/configuration gating for settings and context-menu entry.
- Target resolution for method, type, property, field, unsupported node, and no-selection cases.
- Cancellation before request, during enumeration, and on dialog close.
- Provider error classification without secret leakage.
- Clipboard copy and empty-response behavior where the host abstraction allows deterministic testing.

**Validation commands:**

```bash
# Run AI tests with the installed .NET 10 SDK from a directory outside the repo root
cd /tmp
dotnet run --project /Volumes/OSCOO1TB/repos/ILSpy/ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj -- \
  --filter 'FullyQualifiedName~AI' --report-trx
```

Expected baseline after Phase 0 retargeting: 86 AI tests pass on `net10.0`. Phase 1 must increase coverage without requiring network access.

For repository-wide validation, use the repository's pinned SDK and test runner once the environment has .NET SDK 11 available. The AI project itself remains targeted at `net10.0`; do not retarget it to match unrelated repository test projects.

## Dependency Order

```text
1.1 Settings UI ───────────────┐
                               ├─→ 1.2 Provider/orchestration ─→ 1.4 Explanation dialog
1.3 Context-menu target seam ──┘                         └──────→ 1.5 Tests/validation
```

Recommended execution order:

1. Confirm `SettingsService.AISettings` exposure and secure-key lifecycle.
2. Implement the provider factory/orchestration seam with fake-provider tests.
3. Implement the settings page and wire Test Connection through the same seam.
4. Implement target resolution and the context-menu entry.
5. Implement the blocking dialog and clipboard action.
6. Run focused tests, then full AI-filtered tests.
7. Perform manual cross-platform UI and secure-storage smoke tests.

## Privacy and Security Checklist

- [x] Consent is false by default and required for every AI action.
- [x] API keys never serialize into XML, logs, exceptions, test snapshots, or clipboard content.
- [x] Secure-store unavailable state is explicit; no plaintext fallback exists.
- [x] HTTP endpoints retain Phase 0 validation, including rejection of unsafe non-loopback plain HTTP.
- [x] Provider error bodies remain bounded and sanitized before display.
- [x] Context contains only the selected symbol and opted-in metadata.
- [x] IL and call-graph data remain opt-in and are not sent when unchecked.
- [x] Cancellation stops in-flight requests and clears dialog-owned resources.
- [x] Tests use fake HTTP/provider seams and never require a real API key.

## Phase 1 Completion Criteria

- [x] AI Assistant options page is MEF-discovered and usable.
- [x] Settings persist correctly; API keys remain secure.
- [x] Privacy consent gates Test Connection and Explain with AI.
- [x] Explain with AI works for methods, types, properties, and fields.
- [x] Blocking dialog handles success, cancellation, and provider errors.
- [x] Completed explanations can be copied to the clipboard.
- [x] AI-filtered `net10.0` tests pass.
- [ ] Manual smoke test confirms no request starts before consent.
- [x] Implementation record and roadmap status are updated.

## Follow-up to Phase 2

Phase 2 may replace the blocking response accumulation with dispatcher-driven streaming and a dockable output pane. Phase 1 must keep the provider boundary asynchronous and cancellation-aware so that migration does not require changing context-menu or settings contracts.

---

**Document Version:** 1.0  
**Created:** 2026-08-17

## Implementation Record

**Implemented:** August 17, 2026

- Added `AIProviderFactory` and `AIExplanationService` in `ICSharpCode.ILSpyX/AI`; provider creation is consent/configuration gated and uses the existing OpenAI-compatible provider.
- Added MEF-discovered `AISettingsViewModel` and Avalonia panel with secure key save/load/delete, consent gating, provider/model/endpoint/context controls, and cancellable connection testing.
- Added `ExplainContextMenuEntry`, `ExplainDialogViewModel`, and blocking `ExplainDialog` for type/method/property/field entities, including cancellation and clipboard copy.
- Added focused orchestration, cancellation, error-sanitization, provider-factory, and settings-preservation tests.
- Validation: `ICSharpCode.ILSpyX.Tests` AI filter passes **92/92** on `net10.0`; `ILSpy.csproj` builds with **0 errors** under installed SDK `10.0.400`.
- Manual desktop smoke testing remains pending because this environment has no GUI session and repository `global.json` requests SDK `11.0.0`, which is not installed.
