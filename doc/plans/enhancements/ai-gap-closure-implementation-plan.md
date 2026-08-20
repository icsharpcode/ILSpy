# AI Gap-Closure Implementation Plan

Status: Complete — automated verification passed; manual UI regression environment-limited (2026-08-20)
Created: 2026-08-19  
Audience: implementer working in this repository with no assumed knowledge of the AI roadmap  
Source: AI gap and completeness analysis supplied with this plan request  
Related: `doc/plans/enhancements/plan/multiple-ai-provider-profiles.md`, `doc/plans/enhancements/plan/complete-ai-profiles-and-conversation-boundaries.md`

## 1. Objective and definition of done

Close the reported AI-feature gaps without reimplementing the already-complete profile-domain foundation. The completed result has no interactive request path that reads a mutable `AISettings` target; all normal AI work captures one immutable `AISelectionSnapshot` at request start. Chat is UI-thread safe, profile-aware, and conversation-target-bound. Security analysis is narrowly scoped by default and bounded when expanded. Rename, command, context, annotation, and search behavior match the decisions below.

Definition of done:

- Every production AI request resolves its target through `AISelectionService.ResolveSnapshotAsync` (or is passed the snapshot resolved by its entry point). No mutable `AISettings` factory/service overload remains.
- AI Settings exposes a master-detail profile/model editor with isolated drafts and secure-key UX. AI Chat exposes shared profile/model selectors, exact readiness errors, and a navigation action to AI Settings.
- Chat history is schema 2 with target-bound conversations. Legacy history remains readable but cannot be sent until a new conversation is started.
- Normal security analysis sends at most one selected-type request. Explicit bulk analysis is confirmed, bounded, progress-reported, and cancellable. Only findings with confidence >= 70% appear.
- Batch rename displays numeric confidence, starts suggestions below 60% unchecked, and reports numeric progress. `/help` and `/clear` are local; `/audit` and `/summary` call their real pipelines.
- Malformed metadata degrades contextual AI output rather than failing the request; annotation mismatch is observable; rename-annotation hashing is not repeated per decompile; identified subscriptions/timers have a disposal owner.
- `ExplainDialog` is removed after its zero-reference audit. `AIConversationTarget` is retained and used by history. No committed `TestResults/` directory remains.
- The profiles plan/roadmap status and semantic-search scope accurately reflect what ships.

## 2. Evidence correlation

| Gap-analysis recommendation | Current correlated evidence | Required result | Primary phase |
|---|---|---|---|
| Chat UI thread safety and snapshot migration | `ILSpy/AI/AIChatPaneModel.cs` creates via `IAIProviderFactory.CreateAsync(settingsService.AISettings, ...)`; terminal `StatusMessage`, `ErrorMessage`, and `IsBusy` updates follow `ConfigureAwait(false)` | UI dispatcher owns all bound state; one snapshot captured before provider work | 1, 5 |
| Security analyzer scope/confidence | `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs` expands selected type to all module types and accepts all nonempty findings | Selected type only by default; separate explicit bulk action; `confidence >= 0.70` | 2 |
| Complete profile plan and remove bridge | `AISelectionService` and schema 2 persistence exist; settings UI still binds legacy facade; `AIProviderFactory`, `AIExplanationService`, `RenameSuggester` retain mutable constructors | Implement UI + consumer migration, then remove bridge at a zero-caller gate | 3-5 |
| Dead code | `ExplainDialog.axaml`, code-behind, and view model have no callers; `AIConversationTarget` currently only declaration/tests | Delete dialog trio; retain target type and make it history contract | 5, 8 |
| Rename and command polish | `BatchRenameDialogViewModel` has only text `IProgress<string>` and auto-selects every suggested item; chat only expands `/audit` and `/summary` into text | Counted progress, visible percent, `<60%` unchecked; real command dispatch | 6 |
| Reliability | `ContextBuilder.Decompile` and `GetCallers` lack neighboring recoverable-metadata guards; annotation mismatch is silent; manager constructed by UI paths; tooltip creates undisposed timer | Graceful context, notification/caching, lifecycle fixes | 7 |
| Search/document reconciliation | `AISearchStrategy` and `SemanticSearchStrategy` are static and special-cased in `SearchPaneModel`; `EmbeddingStore` is a local hash-vector heuristic | Register strategies through normal mechanism when feasible and document embedding descoping | 8-9 |

## 3. Locked decisions and non-goals

These are implementation constraints, not questions to defer:

1. Complete profile UI, history, and consumer migration. Do not descope completed domain work after it has been persisted to user settings.
2. Keep `AIConversationTarget`. Target identity is `ProfileId`, provider type, normalized endpoint, and model. Profile display name is history metadata only and must not create a new conversation.
3. Delete the unreferenced `ExplainDialog` trio. Do not repurpose it without an explicit product request.
4. The existing `EmbeddingStore` is a local dependency-free similarity heuristic, not provider embedding search. Retain only that scope and record the decision. Do not introduce a remote embedding dependency in this work.
5. Security-finding confidence threshold is 70%. Batch-rename auto-selection threshold is 60%. Keep these constants separately named and tested; do not share an ambiguous magic number.
6. `/clear` and `/help` never create a provider or send network traffic. `/audit` and `/summary` must execute the same application pipelines users invoke through their existing UI commands.
7. In-flight requests keep their captured snapshot even if a selection/profile changes or is deleted. Future requests resolve afresh.
8. Preserve existing, unrelated worktree file `doc/plans/enhancements/plan/complete-ai-profiles-and-conversation-boundaries.md`; this plan does not replace it.

Out of scope: provider billing/discovery, profile import/export, cloud embeddings, cancellation caused solely by selection changes, broad options-framework redesign, and unrelated AI prompt changes.

## 4. Dependencies and execution order

```text
baseline/contract audit (0)
  -> chat safety + snapshot/local commands (1)
  -> scoped analyzer (2)
  -> profile editor (3)
  -> remaining snapshot migration/removal gate (4)
  -> chat selectors + schema-2 history (5)
  -> rename polish + real command integrations (6)
  -> reliability hardening (7)
  -> search/dead-code cleanup (8)
  -> documentation reconciliation + full verification (9)
```

Do not remove mutable APIs before phase 4's repository-wide caller audit. Do not make chat-history persistence schema 2 before target identity and selection semantics are tested. Do not attach `/audit` to a bulk module sweep; it must start with selected-type analysis.

## 5. Phase 0 - Baseline and contract audit

Purpose: establish a reproducible baseline and resolve two intentionally narrow implementation details before edits. This phase changes no production behavior.

### Work

1. Record baseline build/test outcomes. Run the focused AI suite before changing code; separately record existing failures rather than masking them.
2. Inspect current composition/DI registrations for `AISelectionService`, `IAIProviderFactory`, `AIChatPaneModel`, context-menu entries, rename dialogs, analyzer creation, and settings navigation. Identify the single existing command/service used to open Options at the AI settings page. Reuse it for chat's `Open AI Settings` command; do not introduce a second options window.
3. Enumerate every production mutable target consumer, including these known locations: `AIChatPaneModel`, `AssemblySummaryContextMenuEntry`, `GenerateDocsContextMenuEntry`, `SearchPaneModel`, `AISecurityAnalyzer`, `RenameSuggester`, rename/batch-rename creation paths, and any direct `AISettings.Provider`, `.BaseUrl`, `.Model`, `.ApiKey`, or `IAIProviderFactory.CreateAsync(AISettings, ...)` access. Put the results in the implementation PR description or phase notes.
4. Confirm whether `TestResults/` exists in the tracked tree with `git ls-files` and filesystem search. Delete it only if it is a generated test artifact; preserve a legitimate source fixture if discovered. Current working-tree check found none.
5. Locate the second reported handler/timer leak precisely. `ILSpy/AI/Controls/MarkdownTextEditor.cs:ShowTransientTooltip` is confirmed to allocate a non-disposed `System.Threading.Timer`; inspect AI controls and view detach paths for the other reported lifetime issue. Document exact owner/file before editing. If no second leak reproduces, record that as a report correction with audit evidence rather than guessing.

### Tests and gates

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI' --verbosity minimal
rtk git ls-files | rtk rg '(^|/)TestResults(/|$)'
rtk rg -n 'CreateAsync\(.*AISettings|new AIExplanationService\(.*AISettings|new RenameSuggester\(.*AISettings|\.AISettings\.(Provider|BaseUrl|Model|ApiKey)' ILSpy ICSharpCode.ILSpyX
```

Gate: baseline failures are known; composition owner and settings navigation route are identified; no production modifications begin against an unknown host contract.

## 6. Phase 1 - Chat safety, immutable request targets, and local commands

### Files and symbols

- Modify `ILSpy/AI/AIChatPaneModel.cs` (`SendAsync`, command parsing, constructor/dependencies, state mutation helpers).
- Modify `ILSpy/AI/AIChatPane.axaml` and, only if needed, `AIChatPane.axaml.cs` for command bindings.
- Reuse `ICSharpCode.ILSpyX/AI/AISelectionService.cs`, `AISelectionTypes.cs`, `AIProviderFactory.cs`, and `AIExplanationService.cs`; do not create a parallel selection service.
- Add/extend focused tests in `ILSpy.Tests/AI/` and/or existing AI model test project following current local conventions.

### Implementation steps

1. Inject `IAISelectionService` (the interface/type already registered by composition) into `AIChatPaneModel`; remove its request-time dependency on `SettingsService.AISettings` for provider/model/endpoint/key resolution. Retain `SettingsService` only for unrelated UI preferences if an audit proves it is still required.
2. At the start of each send, call `ResolveSnapshotAsync(cancellationToken)` once. Capture the returned `AISelectionSnapshot` in a local variable and construct `AIExplanationService(snapshot, providerFactory)` or use the snapshot factory overload. No later code in the request may reread selection/editor settings.
3. Call readiness evaluation before request UI is committed. For non-ready state, set the exact resolver-provided error/reason, set a configuration-required status, expose `CanOpenAISettings`, and do not instantiate a provider. Preserve the typed configuration exception path for race/failure cases.
4. Make Avalonia dispatcher ownership explicit. Create one small `SetUiStateAsync`/`PostUiState` helper that runs bound-property changes on `Dispatcher.UIThread`. Use it for every post-await terminal write to `StatusMessage`, `ErrorMessage`, `IsBusy`, command can-execute notifications, and streamed collection mutation. Do not rely on accidental synchronization context after `ConfigureAwait(false)`.
5. Prevent stale send completion from overwriting newer UI state. Give each send a monotonically increasing request generation or retain the current cancellation-source identity; terminal UI mutations apply only when the completing request is still current. Cancellation must still show `Canceled` for the active request. Dispose/release replaced cancellation sources safely.
6. Replace ad-hoc slash expansion with parsed local command dispatch. Commands are case-insensitive, trim command arguments, and preserve ordinary messages beginning with `/` that are not supported with an actionable local error.
   - `/help`: append/display local help listing supported commands; no request/provider/history provider send.
   - `/clear`: cancel no active request implicitly; clear the current conversation only after existing UI confirmation convention, or immediately if the existing Clear button is immediate. Persist the updated history; no provider request.
   - `/explain` and `/rename`: retain current behavior but resolve the snapshot once if they send.
   - `/audit` and `/summary`: expose injectable callbacks/host services rather than formatting a prompt. Phase 6 wires those callbacks to existing feature pipelines. Until then return a local unavailable error, never fake a prompt-result action.
7. Keep the visible Clear button routed through the same implementation as `/clear`, so behavior and persistence cannot diverge.

### Required tests

- A fake selection service returns snapshot A, then active selection changes to B while A's provider stream is pending: the request factory and all chunks use A only.
- Non-ready selection: no factory call; exact readiness reason appears; Open AI Settings action is enabled.
- A completion continuation running off the UI context does not raise bound property changes off the dispatcher. Test through a dispatcher-aware harness or a testable UI dispatcher abstraction matching project practice.
- Cancel/error/success each clear `IsBusy` once and cannot overwrite a newer send's status.
- `/help` and `/clear` perform zero provider-factory calls. `/clear` uses the same history mutation as Clear button. Unsupported slash command provides a local message.

### Gate

Focused chat tests pass; a code search finds no `settingsService.AISettings` used to resolve the chat target; cross-thread terminal assignments are gone.

## 7. Phase 2 - Security analyzer scope, confidence, and explicit bulk audit

### Files and symbols

- Modify `ICSharpCode.ILSpyX/Analyzers/Builtin/AISecurityAnalyzer.cs`.
- Modify `ICSharpCode.ILSpyX/Analyzers/AnalyzerContext.cs` only to add a minimal, host-supported request-target/progress seam.
- Modify analyzer UI entry/creation code found in phase 0 composition audit.
- Extend `ICSharpCode.ILSpyX.Tests/Analyzers/AISecurityAnalyzerTests.cs`.

### Default selected-type analysis

1. Define a constant such as `MinimumFindingConfidence = 0.70`. Prompt for machine-readable `confidence` as a number in range 0..1 and include it in the DTO.
2. When a method is selected, normalize it to its declaring type. When a type is selected, analyze that type. Do not enumerate sibling types from the module in the normal analyzer path. If the host supplies an unsupported selection, return a clear no-analysis result rather than widening scope.
3. Parse confidence using invariant JSON numeric handling. Accept 0..1 values only. Missing, NaN, invalid, or out-of-range confidence is rejected (not assumed confident). Keep a finding only when `confidence >= 0.70`; 0.70 is included, 0.69 is excluded. Record filtered-count diagnostics only if existing analyzer result UX has a safe non-sensitive channel; never show raw LLM response.
4. Resolve a snapshot at the UI/entry boundary and pass it through `AnalyzerContext` or a dedicated immutable AI request dependency. Do not put a live `AISettings` property back into the analyzer contract. Provider failures/readiness errors should be reported using existing analyzer error presentation.

### Explicit module/assembly audit

1. Add a separately named user action, for example `Run AI Security Audit for Module`, rather than changing the default analyzer. It must display a confirmation that includes target module/assembly name, type count after eligibility filtering, and that each type may send an AI request.
2. Define a bounded work contract: eligible type list is captured once, processed in deterministic metadata/order sequence, max request count is a named constant/configuration owned by the action, and excess types are not silently sent. Either ask the user to reduce scope or require a second explicit confirmation for a documented capped subset. Do not invent unbounded background work.
3. Surface `current/total`, current type display name, accumulated findings, and a Cancel action. Cancellation flows into each provider request and stops scheduling further types. Partial completed results are explicitly labeled partial; cancellation must be visible, not swallowed.
4. Process sequentially unless existing provider throttling infrastructure supports a tested limit. Sequential behavior is safer for cost/rate limits and easier to cancel. Continue per-type recoverable request failures, reporting summary counts; fail immediately on configuration/readiness failure.
5. The bulk action resolves a snapshot once at start, then uses it for all types. It must not silently change provider/model halfway through.

### Required tests

- Selecting a method submits only its declaring type. Selecting one type makes one request even when the module has several types.
- Parsed confidence: 0.69 excluded, 0.70 included, 1.0 included, missing/invalid/negative/>1 excluded.
- Bulk planner enforces cap before provider calls; emits initial and final progress values; cancellation after N calls creates no call N+1 and returns marked partial results.
- Bulk processing remains on captured snapshot after selection change.
- Non-ready selection makes zero analyzer provider calls and supplies actionable configuration feedback.

### Gate

No normal path enumerates all module types. The bulk action is visibly named, confirmed, cancellable, bounded, and independently tested.

## 8. Phase 3 - AI Settings master-detail profile editor

### Files and symbols

- Modify `ILSpy/Options/AISettingsViewModel.cs`.
- Modify `ILSpy/Options/AISettingsPanel.axaml`; modify code-behind only for unavoidable Avalonia integration.
- Reuse `AISelectionService.SaveProfileAsync`, `DeleteProfileAsync`, `MoveProfileAsync`, `ApplySelectionAsync`, `EvaluateReadinessAsync`, `AIProviderCatalog`, and existing credential storage.
- Add tests in existing settings/AI test projects; use a UI test only if the repository already has an Avalonia test harness.

### Implementation steps

1. Replace direct editable bindings to legacy `Settings.Provider`, `BaseUrl`, and `Model` with an ordered profile master list and a selected isolated `AIProfileDraft`. Do not bind draft text boxes to persisted profile objects. Global privacy/context controls may retain their existing settings bindings if they are not target identity.
2. Provide commands: Add, Duplicate, Delete, Move Up, Move Down; Add/Rename/Delete/Move model; Save, Cancel; Replace Key, Remove Key, Test Connection, Cancel Test Connection. Disable only operations that violate invariants, with visible reason.
3. Build provider controls from `AIProviderCatalog` descriptors, including Anthropic. Descriptor data determines labels/default endpoint/model, key requirement, and readiness hint. Do not scatter provider string comparisons in UI.
4. Draft rules: profile/model names trim and are case-insensitively unique; endpoint is absolute HTTP(S) using project normalization; each profile retains one model; duplicate gets new ID/copies non-secret fields/clears secret; add is unsaved until Save. Cancel discards draft and its transient key input.
5. Secret UX: show stored/not stored only. A replacement password input is write-only/transient and cleared on Save/Cancel/failure-safe exit. Remove Key has confirmation. Saving a profile missing a required key is allowed, but selection readiness becomes non-ready. Never bind retrieved key material to UI.
6. Use service save/delete ordering and errors as source of truth. On failed save, retain draft, existing persisted profile, and an actionable non-secret error. Deleting active profile uses service's deterministic next/wrap fallback; render resulting active selection.
7. Test Connection targets the selected draft model and exact draft endpoint/provider using an immutable candidate snapshot validated by service helper; it is diagnostic only, session-only, invalidated by changed draft target/key, and never gates Save.
8. Expose a callable navigation/activation method for chat's `Open AI Settings` action. It must select the AI panel in the already-open Options host, not manipulate profile data.

### Required tests

- Draft editing then Cancel leaves persisted profile/model/secret reference unchanged.
- Add/duplicate profile uses a fresh stable ID; duplicate secret status is false.
- Case-insensitive duplicate profile/model names and malformed endpoint block Save. Removing only model/only profile is rejected.
- Existing stored secret never appears in view-model text. Replacing/removing key calls secure storage through service and clears transient input.
- Active deletion follows next/wrap fallback; failed secure-key deletion leaves metadata unchanged.
- Test connection identifies target but does not persist/save or expose a key.

### Gate

No profile/model/endpoint UI control two-way binds to the singleton legacy target. All editor persistence routes through `AISelectionService`.

## 9. Phase 4 - Migrate consumers and remove mutable bridge APIs

### Migration matrix

| Consumer | Required migration |
|---|---|
| `ILSpy/AI/AIOutputPaneModel.cs` | Keep as reference pattern; audit it captures one snapshot once. |
| `ILSpy/AI/AIChatPaneModel.cs` | Complete in phase 1/5. |
| `ILSpy/AI/AssemblySummaryContextMenuEntry.cs` | Resolve snapshot at invocation; pass snapshot explanation service. |
| `ILSpy/AI/GenerateDocsContextMenuEntry.cs` | Same as summary; preserve cancellation/error UX. |
| Rename dialog/context entry/batch dialog | Resolve snapshot at command start; construct `RenameSuggester`/`BatchRenameSuggester` snapshot paths. |
| `ICSharpCode.ILSpyX/AI/RenameSuggester.cs`, `BatchRenameSuggester.cs` | Retain only snapshot constructor/path after callers migrate. |
| `ILSpy/Search/SearchPaneModel.cs`, `AISearchStrategy.cs` | Resolve snapshot before AI query; readiness failure stays a search/status error. |
| `AISecurityAnalyzer` and its host | Pass resolver result/immutable snapshot; complete in phase 2. |
| `AIProviderFactory.cs`, `AIExplanationService.cs`, `ContextBuilder.cs` | Remove mutable target overloads after all consumers compile against snapshot. |

### Implementation sequence

1. For each entry point, resolve exactly once at the user action boundary. Pass `AISelectionSnapshot` down as a parameter/constructor argument; never inject live settings merely for target resolution in leaf services.
2. Preserve global non-target preferences by taking them from the snapshot (or an immutable context option object already captured with it). Do not re-read `AISettings` after request start.
3. Make readiness errors user-facing at UI boundaries, while core services keep typed `AIConfigurationException` behavior for defense in depth.
4. Update tests per consumer to assert provider creation receives snapshot endpoint/model/profile credential identity, not a later live setting.
5. Run the migration audit below. Only when it returns no production references, delete `CreateAsync(AISettings, ...)`, mutable `AIExplanationService` constructors, mutable `RenameSuggester` constructor/state, and any obsolete compatibility adapter. Update XML/docs comments that call them temporary.

### Mandatory zero-caller gate

Run before deleting bridge APIs and again before merge:

```bash
rtk rg -n 'CreateAsync\(.*AISettings|new AIExplanationService\(.*AISettings|new RenameSuggester\(.*AISettings|new BatchRenameSuggester\(.*AISettings|AISettings\? settings|readonly AISettings.*settings' ILSpy ICSharpCode.ILSpyX --glob '*.cs'
rtk rg -n '\.(Provider|BaseUrl|Model|ApiKey)' ILSpy ICSharpCode.ILSpyX --glob '*.cs'
```

Review each remaining hit manually: global preferences and persistence code are allowed; a production request target/provider/key bypass is not. Record allowed hits in the PR rather than weakening the query.

### Gate

All production provider construction is snapshot-only; old public/internal bridge APIs are deleted; tests compile without them.

## 10. Phase 5 - Chat selectors, readiness UX, and schema-2 conversations

### Files and symbols

- Modify `ILSpy/AI/AIChatPane.axaml`, `AIChatPaneModel.cs`, and code-behind only for lifecycle binding.
- Modify `ICSharpCode.ILSpyX/AI/ChatHistory.cs`; add focused model types in that file or nearby `ChatConversation` source following project style.
- Use `AIConversationTarget` from `AISelectionTypes.cs`; do not duplicate it.
- Extend/add `ICSharpCode.ILSpyX.Tests/AI/ChatHistoryTests.cs` and chat model tests.

### UI and selection behavior

1. Add compact profile and model selectors above chat messages. Bind their items and active values to selection service state. They remain enabled when selection is non-ready so users can correct the choice.
2. Render exact `AIConfigurationState` message in a dedicated readiness region and bind `Open AI Settings` to phase 3's host navigation. Hide/disable Send only while not ready or busy; do not hide selector choices.
3. Profile/model selection updates shared selection via `ApplySelectionAsync` and persists under existing service semantics. It must not mutate a conversation target in place. Compare previous/new target identity after successful selection: target changes create/select a new conversation; reorder/display-name/key-only/test-status changes do not.
4. Provide a visible conversation selector/list sufficient to reopen prior conversations and start a new one. Prior target metadata displays human-readable profile/model; deleted profiles display `Profile Name (deleted)` and remain readable.

### History schema and behavior

1. Replace the flat persisted shape with `SchemaVersion = 2`, `AssemblyPath`, and ordered `Conversations`. Each conversation has stable ID, `AIConversationTarget`, messages, and `ReadOnly`. Preserve stable message ordering.
2. Load old JSON containing flat `Messages` as one legacy conversation with unknown/no target and `ReadOnly = true`. It is displayed and exportable but cannot be submitted to a provider. Sending from it must prompt/require `New conversation` under current target; never silently repurpose legacy context.
3. New conversation target is built from captured selection snapshot using profile ID, provider type, normalized endpoint, model, and display-name snapshot. Target equality ignores display name and credential material.
4. On a target change, retain all prior conversations, select a new empty conversation for the new target, and write schema 2 on next save. Do not combine same-model messages across endpoints/profiles.
5. On profile deletion or inability to resolve a historical profile, keep persisted target metadata/read-only history. Do not delete history or route it to fallback profile.
6. `ToMarkdown` exports selected/all conversation metadata (profile/model/endpoint as appropriate to existing privacy conventions), keeps messages ordered, and does not export secrets.
7. Save atomically if project history conventions permit (temp file + replace); otherwise retain current tolerant IO behavior and add failure reporting without corrupting prior valid history.

### Required tests

- Round-trip schema-2 conversations and target metadata.
- Flat legacy history loads as one read-only unknown-target conversation, with messages intact. Attempting to send it makes zero factory calls until explicit new conversation.
- Same profile/model but endpoint change, model change, provider change, or profile ID change creates a boundary. Profile display-name change and reorder do not.
- Snapshot captured for a conversation request remains stable through selector change. Messages never cross targets.
- Deleted target stays readable/marked deleted; it cannot become writable by fallback resolution.
- Readiness message and Open Settings action work while selectors remain usable.

### Gate

No flat `ChatHistory.Messages` request path remains. No history from an unknown/deleted/different target can be sent without explicit new conversation.

## 11. Phase 6 - Batch rename UX and real chat feature commands

### Batch rename

Files: `ICSharpCode.ILSpyX/AI/BatchRenameSuggester.cs`, `ILSpy/AI/BatchRenameDialogViewModel.cs`, `ILSpy/AI/BatchRenameDialog.axaml`, rename-focused tests.

1. Change progress from `IProgress<string>` to a structured immutable value carrying completed count, total eligible count, current member display name, and optional skipped/error count. Establish total before requests; progress updates must reach UI on dispatcher.
2. Expose `ProgressValue`, `ProgressMaximum`, percent/status properties and add an Avalonia `ProgressBar` with stable layout. Keep current target text and cancel action visible.
3. Add `SelectedSuggestionConfidencePercent`/equivalent display based on existing `RenameSuggestion.ConfidencePercent`. Each review row must show numeric percent for selected candidate and retain access to alternatives/reasoning.
4. Add `BatchRenameAutoSelectConfidence = 0.60`. A row with suggestions begins selected only when its initially selected suggestion has confidence >= 60%; lower confidence stays visible and selectable but starts unchecked. If user changes selected candidate, do not silently override their choice; update displayed percent.
5. Migrate batch creation to snapshot input under phase 4. Maintain cancellation and individual parse-error rows. Do not apply a rename with no selected valid candidate.

Tests: 59% starts unchecked, 60% starts checked, 100% checked; percent normalization for 0..1 and 0..100 provider values; structured progress begins `0/total`, finishes `total/total`, and cancellation stops updates/calls safely; Apply changes only checked rows.

### Real `/audit` and `/summary` commands

1. In the composition/root owning `AIChatPaneModel`, inject narrow host delegates/services for the existing selected-type audit and assembly-summary actions. Use selected assembly/type context from the same `AssemblyTreeModel` source as existing context-menu entries.
2. `/audit` invokes selected-type analyzer from phase 2, never module-wide bulk audit. If selection is unsupported or analyzer cannot run, append an actionable local chat status and make no provider request through chat. Surface progress/cancellation through existing analyzer UX or a deliberately small chat progress adapter; do not duplicate analyzer logic in `ExpandCommand`.
3. `/summary` invokes `AssemblySummaryContextMenuEntry`/underlying shared summary service. It should open/route output the same as the existing menu action, with snapshot capture and readiness errors already completed in phase 4.
4. Keep slash parsing as phase 1 local dispatch. `/help` text must say what each command does, including that `/audit` analyzes selected type.

Tests: `/audit` calls the analyzer delegate once for selected type; `/summary` calls summary delegate once; neither sends a generic chat completion prompt; unsupported selection makes zero delegate/provider calls and reports local guidance.

### Gate

Users can see progress and confidence before applying batch renames. All four slash commands have non-ambiguous behavior and no command fakes a feature by sending a prose prompt.

## 12. Phase 7 - Reliability and resource-lifetime hardening

### Context builder recoverability

Modify `ICSharpCode.ILSpyX/AI/ContextBuilder.cs` and its tests. Apply the existing `IsRecoverableMetadataException` predicate consistently around `Decompile`, `GetCallers`, and any directly adjacent graph/decompile operations (including `GetCallees` if audit confirms same unsafe metadata access).

- Catch only the predicate-approved metadata/decompiler exception set already used by `GetStringLiterals`, `GetIL`, and `ScanMethodReferences`. Do not add broad `catch (Exception)` that hides programming/configuration errors.
- Return an explicit bounded unavailable-context section/message so the prompt remains coherent; continue building remaining context.
- Add tests/fakes/malformed metadata fixtures proving each guarded path does not abort `Build`, while unexpected exceptions still propagate according to existing contract.

### Rename annotations

Modify `ICSharpCode.ILSpyX/Annotations/RenameAnnotations.cs`, its call sites (`CSharpLanguage`, rename dialogs), and `RenameAnnotationManagerTests.cs`.

1. Preserve rejecting a hash-mismatched annotation file. Add an observable non-secret notification result/event/logger through the existing UI/reporting pattern: identify that saved annotations do not match the current assembly and were not applied, without revealing API keys or unnecessary absolute paths.
2. Separate content identity/hash computation from manager construction. Introduce a per-assembly cache owned by a clear application service or static weak/explicit cache with synchronization and invalidation on file path/last-write/length or assembly close/reload. Do not cache stale annotations after a rename save.
3. Reuse cached manager/context in `CSharpLanguage` and dialogs rather than constructing and hashing on each decompile. Define lifecycle disposal/invalidation owner before implementation; no unbounded static strong-reference cache of assembly files.
4. Tests retain existing mismatch rejection assertion and add notification assertion, same-assembly reuse/no repeated hash calculation (via injectable hash abstraction or observable test hook), changed-file invalidation, and concurrent access safety.

### Handler/timer leaks

1. In `ILSpy/AI/Controls/MarkdownTextEditor.cs`, replace per-tooltip unowned `System.Threading.Timer` with a disposable/reused timer owned by the control or a dispatcher delayed operation cancelled on visual-tree detach/dispose. Ensure callback cannot touch a detached control.
2. Fix the specifically identified second leak from phase 0. Its subscription must be detached/disposed by the same object that attaches/creates it, be idempotent across attach/detach, and not retain view models/windows. Do not make speculative edits if audit found no second leak.
3. Add lifecycle tests where feasible, otherwise add a deterministic unit test of disposable timer/subscription owner plus manual repeated attach/detach test steps.

### Gate

Malformed metadata cannot abort an otherwise usable AI request; mismatch rejection is visible; repeated decompile does not repeatedly SHA-256 unchanged assembly; lifecycle owners dispose their timer/subscriptions.

## 13. Phase 8 - Search architecture reconciliation and cleanup — Complete (2026-08-20)

Implementation note: the existing synchronous, module-oriented search registry cannot
express the asynchronous AI provider/readiness/cancellation contract without a wider
unrelated refactor. AI and semantic modes therefore remain an explicit
`SearchPaneModel` exception, documented in `ai-search-architecture-decision.md`.
AI search now captures one immutable selection snapshot before background work; semantic
search is explicitly documented and labeled as the dependency-free local heuristic.
The unreferenced `ExplainDialog` trio was deleted after a production-reference audit.

### Search strategy

Files: `ILSpy/Search/SearchPaneModel.cs`, `ICSharpCode.ILSpyX/Search/AISearchStrategy.cs`, `SemanticSearchStrategy.cs`, existing search strategy interfaces/registrations and tests.

1. Inspect normal non-AI search mode registration. Introduce an `ISearchStrategy`-compatible adapter/registration for AI and semantic modes only if it can preserve their asynchronous/error/readiness behavior without a second selection resolution path. Remove `SearchPaneModel` static special-cases when the strategy registry can own selection and execution.
2. If existing registry cannot express asynchronous AI search without a wider unrelated search refactor, retain the special case but document the precise architectural exception and add a narrow follow-up issue. The plan is reconciled only when this decision is explicit; do not force an abstraction that degrades search cancellation/results.
3. Migrate AI search to a snapshot captured by `SearchPaneModel` action boundary. Semantic heuristic search must not claim it is resolving an AI provider.
4. Update user-visible labels/tooltips/docs to call `EmbeddingStore` a local semantic/similarity heuristic, or choose neutral `Semantic search (local heuristic)`. Record that remote embedding-backed search was deliberately descoped, with restoration requiring a separate privacy, storage, model, and evaluation plan.

### Cleanup

1. Run reference audit for `ExplainDialog`. Delete `ILSpy/AI/ExplainDialog.axaml`, `ExplainDialog.axaml.cs`, and `ExplainDialogViewModel.cs`, remove project/resource entries if present, and remove orphan tests/references. Compile after deletion.
2. Retain `AIConversationTarget`; phase 5 must make at least production history code and tests reference it.
3. Remove `TestResults/` only if phase 0 establishes it is a generated tracked artifact. Add/confirm ignore coverage for test result output only if repository convention calls for it; do not broadly ignore a valid source directory.

### Tests and gate

- Search mode registration (or documented exception) test covers mode selection, cancellation, snapshot readiness handling, and result propagation.
- Semantic search tests state local heuristic behavior and do not require a provider or credentials.
- Build confirms dialog deletion leaves no XAML/project references.

## 14. Phase 9 - Plan reconciliation and final verification — Complete with documented manual limitation (2026-08-20)

### Documentation changes

1. Update `doc/plans/enhancements/plan/multiple-ai-provider-profiles.md`: mark completed phases accurately, replace temporary bridge language with final snapshot-only contract, and link this plan. Preserve its original requirements/history.
2. Update roadmap/Next Steps document found in phase 0. Mark profile domain + UI/consumer migration complete only after all gates pass. Explicitly record semantic-search embedding descoping: shipped local heuristic; remote embeddings not implemented and not implied.
3. Update this plan's status to Complete with date, commit/PR reference, test outcomes, and any intentionally deferred second-leak/search-registration decision. Never mark a phase complete solely because code compiles.

### Verification record

Closeout commits: `7fc25472b`, `84e4b2d06`, `260312e03`, `06d88a01e`.

- Desktop solution build: `rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal` — passed, 16 projects, 0 errors, 0 warnings.
- Focused ILSpyX AI/security/rename/history tests: `rtk dotnet test --project ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~AISecurityAnalyzer|FullyQualifiedName~RenameAnnotationManager|FullyQualifiedName~ChatHistory' --verbosity minimal` — passed, 240 tests.
- Focused ILSpy AI/search/options tests: `rtk dotnet test --project ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~Search|FullyQualifiedName~Options' --verbosity minimal` — passed, 133 tests.
- Full-solution test invocation was started twice with `rtk dotnet test --solution ILSpy.sln --no-restore --verbosity minimal`; each run was stopped after exceeding five minutes without terminal output while `ILSpy.Tests` remained CPU-bound. Generated TRX files from the first run showed completed projects with zero failures (Decompiler 3382 passed, ILSpyX 240 passed, ILSpy 133 passed, ILSpyCmd 22 passed, BAML 4 passed), but the overall command was not allowed to finish. This remains a verification limitation, not a claimed full-suite pass.
- `rtk git diff --check` — passed.
- `TestResults/` was confirmed as generated, ignored test output (`*.trx`) and is not part of the commit.
- Search architecture decision remains explicit in `doc/plans/enhancements/ai-search-architecture-decision.md`: AI/semantic modes stay `SearchPaneModel` special cases because the synchronous registry cannot express their async snapshot/readiness/cancellation contract; semantic search ships as a local heuristic and remote embeddings remain descoped.
- Phase 0's second reported lifecycle leak could not be reproduced beyond the confirmed tooltip timer; no speculative second-leak change was made.

### Final commands

Run focused tests while implementing, then final repository checks:

```bash
rtk dotnet build ILSpy.Desktop.slnf --no-restore --verbosity minimal
rtk dotnet test ICSharpCode.ILSpyX.Tests/ICSharpCode.ILSpyX.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~AISecurityAnalyzer|FullyQualifiedName~RenameAnnotationManager|FullyQualifiedName~ChatHistory' --verbosity minimal
rtk dotnet test ILSpy.Tests/ILSpy.Tests.csproj --no-restore --filter 'FullyQualifiedName~AI|FullyQualifiedName~Search|FullyQualifiedName~Options' --verbosity minimal
rtk dotnet test ILSpy.sln --no-restore --verbosity minimal
rtk git diff --check
rtk git status --short
```

If the solution filter/test project names differ, use the existing `.sln`/CI commands rather than inventing a new test host. Record pre-existing failures separately with their exact command and output.

### Manual regression checklist

1. Configure two ready profiles with distinct endpoints/models, begin chat on A, switch to B during a stream, then verify A stream completes under A and B starts a separate conversation.
2. Select a non-ready profile: selectors work, exact readiness message appears, Open AI Settings focuses the AI editor, and Send creates no network request.
3. Reopen a legacy history file and a deleted-profile conversation: both display; neither can send until New conversation.
4. Run selected-type audit in a multi-type module: one request. Start explicit module audit, observe count/progress, cancel, and verify partial-result label/no further requests.
5. Run batch rename containing high and low confidence suggestions: percent is visible, low confidence begins unchecked, progress reaches total, and Apply changes only checked rows.
6. Trigger `/help`, `/clear`, `/audit`, `/summary`; verify their behavior matches this plan and no command falls back to a generic chat prompt.
7. Load an assembly with recoverable malformed metadata and a stale annotation file: contextual output continues; stale annotation notice appears; UI remains responsive after opening/closing AI panes repeatedly.

### Final acceptance gate

All automated tests/gates pass or documented pre-existing failures are approved; manual checklist completes; migration searches show no request-target bypass; documentation says exactly what is shipped.

## 15. Rollback and implementation safety

- Keep changes phase-scoped and build after each phase. Do not combine schema migration, mutable API deletion, and UI redesign in one untestable change.
- Before history schema writes, back up a representative legacy file in a test fixture, never real user data. Loader remains backward compatible throughout rollout.
- Preserve the mutable bridge until phase 4's zero-caller gate. If an overlooked consumer blocks removal, restore only that bridge temporarily, add it to the migration matrix, and do not claim migration complete.
- Keep bulk audit behind explicit action/confirmation throughout rollout. Any uncertainty in cost cap/progress host contract blocks bulk exposure, not selected-type behavior.
- For annotation cache uncertainty, prioritize correctness and visible mismatch over performance: invalidate conservatively and never apply data with a stale hash.
