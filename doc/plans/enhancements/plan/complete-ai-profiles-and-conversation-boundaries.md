# Complete AI Profiles and Conversation Boundaries

Status: Ready for implementation
Created: 2026-08-19
Parent plan: [Multiple AI Provider Profiles and Models](multiple-ai-provider-profiles.md)
Related plan: [AI pane word wrap and follow-tail](ai-pane-word-wrap-and-follow-tail.md)
Scope: Finish the remaining implementation after the shared profile, migration, selection, and snapshot foundation already present in the repository.

## 1. Purpose and current status

This plan is the next implementation phase. It is intentionally narrower than the parent profile plan. Do not reimplement completed foundation work. The implementer must first preserve the existing behavior, then close the remaining user-visible and API migration gaps.

### Already implemented or substantially implemented

- Schema 2 profile persistence, profile/model validation, deterministic repair, and legacy singleton migration.
- `AIProfile`, provider descriptors/catalog, credential IDs, and credential migration foundation.
- `AISelectionService`, `AISelection`, `AISelectionSnapshot`, readiness reasons, immediate selection persistence, and deterministic profile deletion fallback.
- Snapshot overloads and tests in the provider/explanation path.
- Markdown controls, word wrapping, code-fence handling, scroll-threshold math, and math tests.
- `AIOutputPaneModel` resolving an immutable selection snapshot for its request path.

### Remaining implementation work

1. Replace singleton-oriented AI Settings UI with an isolated master-detail profile/model draft editor.
2. Add shared profile/model selectors and exact readiness actions to AI Chat.
3. Replace flat chat history with versioned, target-bound conversations and safe legacy-history handling.
4. Migrate every remaining AI consumer to an immutable `AISelectionSnapshot` captured at request start.
5. Remove mutable `CreateAsync(AISettings, ...)` and remaining mutable-settings facades after callers migrate.
6. Add follow-tail controller-policy tests and execute the manual UI regression matrix.
7. Update parent-plan status/checklists only after all verification gates pass.

Do not mark this phase complete when only the settings UI or only chat works. The shared-selection contract is complete only when all consumers use the same immutable target.

## 2. Scope fence

### In scope

- AI Settings profile/model CRUD, ordering, drafts, validation, secure-key UX, and connection diagnostics.
- Shared selection controls in AI Chat.
- Readiness state and Open AI Settings navigation.
- Versioned chat history with target identity and legacy migration.
- Consumer and provider-factory API migration.
- Focused unit/integration tests, build verification, and manual UI checks.

### Out of scope

- Automatic model discovery, provider account sign-in, billing, usage reporting, or profile import/export.
- Changes to unrelated Options pages, docking architecture, or provider protocol implementations except where required by the new factory contract.
- Cancelling in-flight requests because selection/profile state changed. An in-flight request keeps its immutable snapshot and may finish.
- Rewriting old conversation messages when profile names, endpoints, keys, or models are edited.
- Broad refactoring of `ContextBuilder`; move only target resolution out of mutable settings and retain global context preferences.

## 3. Contracts that must remain true

Carry these invariants from the parent plan into every work package. Add tests whenever a package could violate one.

### Profiles and models

- At least one profile and one model always exist after load/repair.
- Profile IDs are stable forever. Rename, reorder, duplication, and model edits never change an existing profile ID.
- Profile names are trimmed and unique case-insensitively. Model names are trimmed and unique case-insensitively within a profile.
- Profile and model order is persisted. Reordering never changes identity.
- Each profile remembers its last-selected model. Activating a profile restores it if valid; otherwise select its first model.
- The only profile/model cannot be deleted. Active deletion selects the next visible item, wrapping to the first where required.

### Drafts and credentials

- Settings editor changes are isolated in a draft. Draft changes never affect requests, shared selection, or persisted XML until Save succeeds.
- Stored keys are never displayed, copied to clipboard, serialized, logged, or included in exceptions. The UI shows only stored/not stored.
- Missing required keys block requests, not profile Save.
- Credential operations use stable profile credential IDs, not provider names, except the idempotent legacy migration lookup.
- Secure-store/XML operations are retryable and idempotent. A failed secret operation leaves the prior usable state intact where possible.

### Selection and requests

- Every request captures one immutable `AISelectionSnapshot` before provider creation and before asynchronous work starts.
- Selection changes, profile edits, key replacement, reorder, and deletion affect future requests only.
- No feature passes a live `AISettings` instance to the provider factory or request service after the migration gate.
- Readiness errors identify the exact reason and provide an Open AI Settings action.

### Conversations

- Conversation target identity is the tuple `(ProfileId, ProviderType, Endpoint, Model)`.
- A change to any target-identity field starts a new conversation.
- Profile rename, profile reorder, model reorder, key replacement, and diagnostics do not start a new conversation.
- Deleted-target conversations remain readable and are read-only. Continuing requires a new conversation under the active target.
- Legacy flat history remains readable but is never sent to any provider.

## 4. Target data contracts

Use existing types where present; extend them only when the contract below is missing. Avoid duplicate representations.

### `AISelectionSnapshot`

Required immutable fields: `ProfileId`, `ProfileName`, `ProviderType`, `Endpoint`, `Model`, `CredentialId`, resolved `ApiKey` (runtime only), and global request preferences (`MaxContextTokens`, `StreamResponses`, `SendIL`, `SendCallGraph`).

The snapshot may contain the resolved secret only in memory. Never serialize or expose it through UI properties.

### `AIConversationTarget`

Required persisted/display fields: `ProfileId`, profile-name snapshot, provider type, endpoint, model. Implement a value comparison that uses only the four identity fields above. Profile name is display metadata and must not participate in identity comparison.

### Versioned `ChatHistory`

Persist the following shape (property names may follow repository serializer conventions):

```json
{
  "SchemaVersion": 2,
  "AssemblyPath": "...",
  "Conversations": [
    {
      "Id": "...",
      "Target": {
        "ProfileId": "...",
        "ProfileName": "Default",
        "ProviderType": "openai",
        "Endpoint": "https://api.openai.com",
        "Model": "gpt-4o"
      },
      "Messages": [],
      "ReadOnly": false
    }
  ]
}
```

Loading a legacy document containing flat `Messages` creates one `ReadOnly`/unknown-target legacy conversation for display only. Starting a message on it must first create a new conversation using the active snapshot.

## 5. Dependency graph and execution order

```text
WP1 contracts + baseline tests
  ├─> WP2 settings editor + credential UX
  ├─> WP3 chat selectors/readiness
  └─> WP4 versioned history and target transitions
WP2 + WP3 + WP4 ─> WP5 migrate all consumers to snapshots
WP5 ─> WP6 remove mutable factory/facades and run search gate
WP1 ─> WP7 follow-tail controller tests and manual UI matrix
WP6 + WP7 ─> verification and documentation closeout
```

Do not start WP5 until the target/snapshot and conversation contracts compile. Do not remove the mutable factory overload until all callers are migrated and tests have a snapshot fake.

## 6. Preflight and baseline

Run from repository root. Every shell command in this plan is prefixed with `rtk` per repository instructions. Record results in the implementation handoff.

```bash
rtk git status --short
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIEditorScrollStateTests' --verbosity minimal
```

Expected baseline from the prior audit: application build passes; focused AI tests pass except known platform-specific skips; the full `ILSpy.Tests` suite has previously hung and must not be reported as passing unless it completes in this phase. If the baseline is red, classify each failure as pre-existing or introduced before proceeding.

## 7. Work package 1: stabilize contracts and test seams

### Dependencies

None. This package defines the seams used by all later packages.

### Files to inspect/modify

- `ICSharpCode.ILSpyX/AI/AISelectionTypes.cs`
- `ICSharpCode.ILSpyX/AI/AISelectionService.cs`
- `ICSharpCode.ILSpyX/AI/ChatHistory.cs`
- `ICSharpCode.ILSpyX.Tests/AI/AISelectionServiceTests.cs`
- `ICSharpCode.ILSpyX.Tests/AI/ChatHistoryTests.cs`
- Add focused contract tests beside the existing AI tests if no suitable file exists.

### Implementation sequence

1. Confirm `AISelectionSnapshot` has all immutable request fields and no mutable `AISettings` reference. Add a stable `AIConversationTarget` and target-identity comparison if absent.
2. Define the history model as versioned document + conversation records. Keep JSON loading tolerant of missing/new fields.
3. Define explicit transitions for active target changes, assembly changes, Clear/New Conversation, and pane disposal. Each transition must cancel only the current chat request, save the old conversation, then load/create the new one in that order.
4. Define a readiness result contract usable by both settings and chat. Preserve exact existing `AIReadinessReason` values and messages where already implemented.
5. Add tests for target equality, non-identity changes, legacy read-only behavior, and deterministic new-conversation creation.

### Verification

```bash
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AISelectionServiceTests|FullyQualifiedName~ChatHistoryTests' --verbosity minimal
```

### Completion gate

Contracts compile, legacy JSON loads without exception, target identity tests pass, and no UI or provider code depends on draft objects.

### Do not proceed if

- A target comparison includes profile display name or credential/key value.
- Legacy flat messages can reach a provider without explicit new-conversation creation.
- The new history model silently drops unreadable legacy data.

## 8. Work package 2: AI Settings profile/model editor

### Dependencies

WP1 contracts; existing `AISelectionService.SaveProfileAsync`, deletion, validation, and credential migration behavior.

### Files to modify/create

- `ILSpy/Options/AISettingsViewModel.cs`
- `ILSpy/Options/AISettingsPanel.axaml`
- `ILSpy/Options/AISettingsPanel.axaml.cs` only if event/navigation glue is required
- Existing converter/resource files only when necessary for list selection, validation, or password input
- `ICSharpCode.ILSpyX.Tests/AI/AISelectionServiceTests.cs` and/or a new `ILSpy.Tests/Options/AISettingsViewModelTests.cs`

### Required view-model shape

Expose an observable ordered profile collection, selected profile row, isolated `AIProfileDraft`, observable ordered model draft collection, validation errors, key status, and commands: Add, Duplicate, Delete, Move Up, Move Down, Add Model, Edit/Rename Model, Delete Model, Move Model Up, Move Model Down, Save, Cancel, Replace Key, Remove Key, Test Connection, Cancel Test Connection, and Open Settings (the last is consumed by chat if a shared navigation command is available).

The draft must copy non-secret values from the saved profile. Secret input is a separate transient field. Loading a stored key must update only a boolean/status indicator; never put the stored value into a text box.

### Implementation sequence

1. Replace direct bindings to singleton `Settings.Provider`, `Settings.BaseUrl`, and `Settings.Model` with profile list + selected draft bindings. Keep global privacy/context settings bound through existing settings mechanisms.
2. Populate provider options from `AIProviderCatalog`, including Anthropic. Bind endpoint defaults, key requirement, and readiness hints to descriptor metadata rather than provider-name conditionals.
3. Implement Add and Duplicate as unsaved drafts. Generate a unique trimmed name; assign a new ID only when the draft is committed, or preserve a draft ID that is never persisted until Save. Do not write secure storage during Add/Duplicate.
4. Implement model CRUD and ordering. Reject blank/duplicate names and deletion of the final model. Keep remembered/active model references valid after rename/delete.
5. Implement Save/Cancel. Validate structure first; execute secure-key replacement/removal using `profile.CredentialId`; persist metadata only after the secure operation succeeds; retain draft and prior saved state on failure. Handle old-secret cleanup with the existing retry/marker contract.
6. Implement profile delete confirmation and deterministic selection fallback through `AISelectionService.DeleteProfileAsync`. Refresh the list only after commit succeeds.
7. Rework Test Connection to resolve a snapshot for the draft/current selected profile and selected model without mutating saved settings. Results are diagnostic only and become stale after endpoint/provider/model/key edits.
8. Clear transient key input after Save or Cancel. Ensure exception/status text never includes key material.
9. Add keyboard/accessibility labels and disabled states for invalid commands; keep destructive actions explicit.

### Tests

- Add/Cancel leaves XML, selection, and secure storage unchanged.
- Duplicate gets a new stable ID, copied non-secret fields, and no copied key.
- Save trims and validates names/models; duplicate names are rejected case-insensitively.
- Model rename preserves order and active reference.
- Last-model and only-profile deletions are rejected.
- Active deletion selects next item/wraps correctly.
- Replace/remove key uses credential ID; stored key value never appears in view-model properties or exception text.
- Save failure leaves previous metadata/key usable.
- Anthropic appears and descriptor controls required-key behavior.

### Verification

```bash
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AISettings|FullyQualifiedName~AISelection' --verbosity minimal
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
```

### Completion gate

A user can create, duplicate, edit, reorder, save, cancel, and delete profiles/models without mutating saved state prematurely. Secure keys remain opaque and profile-ID keyed.

### Do not proceed if

- The editor two-way binds directly to a saved `AIProfile` and can leak unsaved edits into requests.
- Stored secrets are loaded into `ApiKeyInput`.
- Save can persist metadata while a replacement-key write failed.

## 9. Work package 3: chat selectors and readiness UX

### Dependencies

WP1 contract; WP2 shared selection service and profile/model data.

### Files to modify

- `ILSpy/AI/AIChatPaneModel.cs`
- `ILSpy/AI/AIChatPane.axaml`
- `ILSpy/AI/AIChatPane.axaml.cs` only if navigation or lifecycle hooks require it
- Existing application navigation/service interfaces for Open AI Settings, if present
- `ICSharpCode.ILSpyX.Tests/AI/ChatHistoryTests.cs` and chat view-model tests if an established test seam exists

### Implementation sequence

1. Import/use the shared selection service. Expose profile and model selector collections, selected IDs/names, readiness state, readiness message, and an Open AI Settings command.
2. Keep selectors enabled even when readiness is blocked. Changing selection calls `ApplySelectionAsync`, persists immediately, and refreshes readiness.
3. Before Send, resolve one `AISelectionSnapshot`; if not ready, do not add a user/assistant message, show the exact reason, and offer Open AI Settings.
4. Replace `providerFactory.CreateAsync(settingsService.AISettings, ...)` with snapshot creation. Build request messages from the active target-bound conversation only.
5. Subscribe to selection changes. On target-identity change: cancel current request, save current conversation, create/load a fresh conversation for the new target, clear transient input/error state, and never send old messages to the new provider. Rename/reorder/key replacement must not trigger this transition.
6. Preserve normal user cancellation. An in-flight request keeps its captured snapshot even if selection changes or its profile is deleted.
7. Keep status/error messages actionable and free of key data.

### Tests

- Selector changes persist and update readiness.
- Missing consent/key/endpoint/model blocks Send without adding messages.
- Open AI Settings command is available for blocked readiness.
- Target change saves old conversation and starts a new one.
- Rename/reorder/key replacement does not start a new conversation.
- Stale completion from a canceled/old request cannot append to the new conversation.

### Verification

```bash
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~ChatHistory|FullyQualifiedName~AISelection' --verbosity minimal
```

### Completion gate

Chat visibly selects the shared profile/model, reports exact readiness, and cannot cross-send context between different target identities.

### Do not proceed if

- Chat still reads `settingsService.AISettings` for provider/model/key at Send time.
- Selection change only clears the visible list without persisting/saving the old conversation.
- A late stream chunk can mutate a conversation that is no longer active.

## 10. Work package 4: versioned target-bound history

### Dependencies

WP1 target/history contracts; WP3 chat lifecycle requirements.

### Files to modify/create

- `ICSharpCode.ILSpyX/AI/ChatHistory.cs`
- `ICSharpCode.ILSpyX.Tests/AI/ChatHistoryTests.cs`
- `ILSpy/AI/AIChatPaneModel.cs`
- Any export helper/tests that construct `new ChatHistory { Messages = ... }`

### Implementation sequence

1. Add `SchemaVersion`, conversation collection, target metadata, read-only marker, and active conversation ID while retaining compatibility properties only when needed for deserialization.
2. Implement load repair: missing file -> empty history; malformed JSON -> empty history with non-secret diagnostic; schema 1 flat history -> readable legacy conversation marked unknown/read-only; malformed individual conversation -> skip only that record.
3. Implement atomic save through temp file + replace where repository conventions permit. Preserve existing unauthorized/I/O handling.
4. Add helpers to find/create a conversation by target identity and to mark deleted targets read-only.
5. Update Markdown export to identify target metadata without exporting credential IDs or secrets.
6. Update chat save/load ordering around assembly selection, target selection, Clear, New Conversation, and Dispose.
7. Add bounded history size behavior per conversation; do not accidentally trim unrelated retained conversations.

### Tests

- Schema 2 round trip preserves order, IDs, targets, messages, and read-only flags.
- Legacy flat history loads and exports, but cannot produce provider request messages.
- Different endpoint/provider/model/profile IDs produce different conversations.
- Profile rename and key replacement preserve conversation identity.
- Deleted target remains readable/read-only.
- Malformed JSON does not crash pane construction or erase an unrelated valid file during save.

### Verification

```bash
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~ChatHistory' --verbosity minimal
```

### Completion gate

History is backward-compatible, target-bound, and safe against context leakage. Export remains useful while excluding secrets and credential identifiers.

### Do not proceed if

- Legacy messages are silently treated as current-target messages.
- Target equality uses mutable profile name or API key.
- Save can truncate conversations or lose valid records after a malformed record.

## 11. Work package 5: migrate all AI consumers to snapshots

### Dependencies

WP2 shared selection service; WP3 snapshot resolution behavior; WP4 conversation contract.

### Files/call sites to migrate

- `ICSharpCode.ILSpyX/AI/AIExplanationService.cs`
- `ICSharpCode.ILSpyX/AI/RenameSuggester.cs`
- `ICSharpCode.ILSpyX/AI/BatchRenameSuggester.cs`
- `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`
- `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs`
- `ICSharpCode.ILSpyX/Analyzers/AnalyzerContext.cs` and analyzer composition
- `ILSpy/AI/ExplainDialog*.cs`
- `ILSpy/AI/RenameDialog*.cs`
- `ILSpy/AI/BatchRenameDialog*.cs`
- `ILSpy/AI/AssemblySummaryContextMenuEntry.cs`
- `ILSpy/AI/GenerateDocsContextMenuEntry.cs`
- `ILSpy/AI/ExplainContextMenuEntry.cs`
- `ILSpy/AI/RenameAssistantContextMenuEntry.cs`
- `ILSpy/Search/SearchPaneModel.cs`
- `ILSpy/AI/AIOutputPaneModel.cs` (verify existing snapshot path and remove any fallback)
- `ILSpy/AI/AIChatPaneModel.cs`
- Related composition/factory registrations and tests.

### Implementation sequence

1. Add/import the shared selection service at each composition boundary. Keep global context preferences in a separate immutable value or service.
2. At each public operation, resolve/capture the snapshot once and pass it through the operation. Do not resolve repeatedly inside streaming loops.
3. Change service constructors and static methods from `AISettings` to snapshot/resolver contracts. Keep temporary adapters only within one package and mark them for deletion in WP6.
4. Update visibility/configuration checks to call readiness evaluation, not provider-name/key fields. Preserve user-facing disabled/hidden behavior where appropriate, but show exact readiness in Chat.
5. Update dialogs and context-menu entries to handle `AIConfigurationException` and cancellation without exposing secrets.
6. Update search/analyzer paths, including background operations, so each request captures a snapshot at operation start and does not observe later settings edits.
7. Update all tests/fakes to assert the factory receives the expected snapshot profile ID, endpoint, model, and preferences.

### Migration search gate

Run after edits and inspect every result, not just the count:

```bash
rtk rg -n 'CreateAsync\((settings|Settings|AISettings)|new AIExplanationService\([^s]|AISettings\? settings|AISettings settings|settingsService\.AISettings|SaveKeyAsync\([^p]|DeleteKeyAsync\([^p' --glob '*.cs' ICSharpCode.ILSpyX ILSpy
```

Allowed remaining `AISettings` references are persistence/settings UI, migration, global context preference handling, tests of persistence, and explicit compatibility adapters scheduled for removal in WP6. No request path may remain in the result.

### Verification

```bash
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI' --verbosity minimal
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
```

### Completion gate

Every AI feature resolves one immutable snapshot per operation. Selection/profile edits cannot alter an already-started request.

### Do not proceed if

- Any background request still accepts a live `AISettings`.
- A service resolves a new snapshot for each stream chunk or helper call.
- Visibility checks and request execution disagree about readiness.

## 12. Work package 6: remove mutable factory API and compatibility facades

### Dependencies

WP5 migration search gate is clean except explicitly documented persistence/UI code.

### Files to modify

- `ICSharpCode.ILSpyX/AI/AIProviderFactory.cs`
- `ICSharpCode.ILSpyX/AI/AIExplanationService.cs`
- Remaining service/facade files found by the migration search
- `ICSharpCode.ILSpyX.Tests/AI/AIProviderFactorySnapshotTests.cs`
- `ICSharpCode.ILSpyX.Tests/AI/AIExplanationServiceTests.cs`
- All affected test fakes and composition registrations

### Implementation sequence

1. Delete `IAIProviderFactory.CreateAsync(AISettings, CancellationToken)` and its implementation after all callers use snapshots.
2. Delete constructors that accept mutable `AISettings` from request services. If a compatibility overload is needed for tests, replace it with a test-only factory/resolver rather than production API.
3. Make provider validation capability-aware and profile credential-ID based.
4. Ensure provider creation does not mutate settings, cache keys into settings, or read provider-name keys.
5. Update all tests to prove snapshot immutability: mutate settings/profile after `CreateAsync(snapshot)` begins and assert the provider request uses the original snapshot.
6. Run the repository-wide search gate again and document any intentional settings references.

### Verification

```bash
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIProviderFactory|FullyQualifiedName~AIExplanationService|FullyQualifiedName~AISelection' --verbosity minimal
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
rtk rg -n 'CreateAsync\(AISettings|CreateAsync\(settingsService\.AISettings|new AIExplanationService\([^s]|SaveKeyAsync\(.*Provider|DeleteKeyAsync\(.*Provider' --glob '*.cs' ICSharpCode.ILSpyX ILSpy
```

The final search must return no production request-path matches.

### Completion gate

The only provider-factory production contract accepts an immutable snapshot. Build and focused AI tests pass.

### Do not proceed if

- Removing the overload requires reintroducing a mutable-settings path elsewhere.
- A compatibility adapter can still send a request without readiness/snapshot resolution.

## 13. Work package 7: follow-tail controller policy and UI regression coverage

### Dependencies

Existing `AIEditorScrollState`/`AIFollowTailController`; independent of profile work but required for phase closeout.

### Files to modify

- `ILSpy.Tests/AI/AIEditorScrollStateTests.cs`
- `ILSpy/AI/Controls/AIEditorScrollState.cs` only if tests expose a policy bug
- Existing UI test/manual checklist location, or this plan's manual matrix during handoff

### Required controller-policy tests

Add a test seam around controller state transitions if direct Avalonia `ScrollViewer` construction is impractical. Cover: active append scrolls to tail; inactive append retains position; Clear/New Stream resets following state; completion does not force scroll; returning to bottom resumes following; detach/reattach does not restore stale state; a delayed restore cannot override a newer attach.

Keep existing 24-DIP math tests unchanged. If controller behavior cannot be unit tested without a UI thread, add the smallest testable policy object and one Avalonia integration test rather than weakening assertions.

### Verification

```bash
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIEditorScrollStateTests' --verbosity minimal
```

### Manual UI regression matrix

Run on the supported desktop platform and record pass/fail: streaming output while already at bottom; scroll upward then append; append while pane inactive; Clear; start a new stream; completion; scroll to bottom to resume following; word wrapping on/off; theme and font-size changes; pane close/reopen; switching assembly and target profile/model during a stream. Check that text does not jump unexpectedly, controls remain usable, and no old conversation content is sent to a new target.

### Completion gate

All controller-policy tests pass and the manual matrix has no unexplained regressions.

### Do not proceed if

- Completion or pane activation forces a scroll when the user intentionally scrolled up.
- A delayed restore can move a newly attached viewer.
- Word-wrap/theme changes break follow-tail state.

## 14. Cross-package verification

Run after all work packages.

```bash
rtk git diff --check
rtk dotnet build ILSpy/ILSpy.csproj --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AIEditorScrollStateTests' --verbosity minimal
rtk rg -n 'CreateAsync\(AISettings|CreateAsync\(settingsService\.AISettings|new AIExplanationService\([^s]|AISettings\? settings|AISettings settings' --glob '*.cs' ICSharpCode.ILSpyX ILSpy
rtk git status --short
```

Inspect every failed test. Do not hide failures with broader filters, retries, or ignored assertions. The full `ILSpy.Tests` suite is optional only if it completes; if it hangs again, report it as an unresolved verification limitation with the command and observed behavior.

## 15. Failure handling and rollback

- Preserve unrelated user changes. Before each package, re-run `rtk git status --short`.
- If a package changes persisted XML or secure storage behavior and tests fail, stop at that package; do not continue into consumer migration.
- For secure-store failures, retain old metadata/key, surface an actionable status, and leave a retry marker according to the existing migration contract. Never delete the old key before the replacement and metadata commit are confirmed.
- For history migration failures, keep the original file untouched until the new version is serialized successfully.
- For stale streaming results, gate UI updates on request/conversation identity plus cancellation state. Do not solve by mutating provider instances.
- If a compatibility overload is temporarily retained, annotate its exact callers and removal package; it is not phase-complete while production request paths use it.
- If a test cannot run due to platform prerequisites, record the limitation and add a deterministic unit-level substitute where possible.

## 16. Definition of done

### Product behavior

- [ ] AI Settings supports isolated profile/model drafts with CRUD, ordering, validation, Save/Cancel, and secure-key status/replacement/removal.
- [ ] Anthropic and all provider descriptors are available through metadata-driven UI.
- [ ] Chat exposes shared profile/model selectors, exact readiness state, and Open AI Settings navigation.
- [ ] Chat history is versioned and target-bound; legacy history is readable but never sent.
- [ ] Target changes isolate conversations; rename/reorder/key replacement preserve them.
- [ ] Deleted-target conversations are readable and read-only.
- [ ] All AI consumers use immutable snapshots captured at request start.
- [ ] Mutable `CreateAsync(AISettings, ...)` and production mutable-settings facades are removed.
- [ ] Follow-tail controller policy is covered by tests and manual UI checks.

### Engineering quality

- [ ] Focused AI tests pass.
- [ ] Application build passes with no new warnings/errors.
- [ ] `git diff --check` passes.
- [ ] Repository-wide migration search has no unapproved mutable request paths.
- [ ] No key material appears in XML, JSON, logs, status text, exceptions, or test output.
- [ ] Parent plan status/checklists are updated only after the above gates pass.

## 17. Documentation closeout

At completion, update `doc/plans/enhancements/plan/multiple-ai-provider-profiles.md` to distinguish delivered work from any intentionally deferred items. Add a short completion note to `doc/plans/enhancements/PROJECT-SUMMARY.md` if that file tracks active plans. Include exact test commands, pass/skip counts, manual UI results, and any full-suite hang/platform limitation. Do not claim the parent plan is complete while any Definition of Done checkbox remains unresolved.
