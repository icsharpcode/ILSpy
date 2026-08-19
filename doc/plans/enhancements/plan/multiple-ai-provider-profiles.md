# Multiple AI Provider Profiles and Models

Status: In progress — profile/conversation implementation delivered; manual UI/full-suite verification pending  
Created: 2026-08-18  
Scope: Shared application-level AI target selection for every ILSpy AI feature.

## Executive summary

Implementation note (August 19, 2026): the shared profile, selection, snapshot, settings-editor, and target-bound chat work is implemented on `master`. Focused AI/build verification passes; manual UI regression and the historically hanging full `ILSpy.Tests` suite remain to be executed before changing this parent plan to Complete.

Replace the singleton AI connection settings with a versioned collection of user-managed AI profiles. Each profile has a stable ID, unique display name, provider type, absolute HTTP(S) endpoint, secure credential reference, and ordered manually managed model names. One application-wide active selection (profile plus model) is used by chat and every other AI feature. Profile editing stays in AI Settings; compact profile/model selectors are added to the AI Chat pane.

The implementation must preserve existing settings and secrets, isolate unsaved drafts, never expose stored keys, and make every request use an immutable target snapshot. Chat conversations become target-bound records so a provider/model change cannot send old context to a different service.

## Goals

- Support multiple profiles for OpenAI, Anthropic, Ollama, and OpenAI-compatible custom endpoints.
- Keep provider and model lists manually managed; no model discovery.
- Persist profile order, model order, active profile, and each profile's last-selected model.
- Keep one shared selection across all AI features.
- Preserve and migrate existing singleton settings and provider-key storage safely.
- Provide deterministic deletion fallback and explicit readiness errors.
- Keep credentials in OS secure storage, keyed by stable profile identity.
- Preserve chat history while enforcing conversation target boundaries.
- Add focused unit, integration, migration, persistence, and UI tests.

## Non-goals

- Provider account synchronization, sign-in, billing, usage reporting, or automatic model discovery.
- Import/export of profiles. Future export must exclude credentials.
- Reworking the general options framework or unrelated settings sections.
- Cancelling in-flight requests when selection changes or a profile is deleted.
- Rewriting historical conversations when profile names or settings change.

## Current-state analysis

AISettings.cs currently stores one mutable Provider, BaseUrl, Model, runtime ApiKey, and non-secret ApiKeyPlaceholder, plus application-wide context/privacy preferences. SaveToXml does not serialize the key. Existing defaults are OpenAI (https://api.openai.com, gpt-4o), Anthropic (https://api.anthropic.com, claude-opus-4-8), and Ollama (http://localhost:11434, llama3:70b); custom currently uses OpenAI defaults.

AIProviderFactory.CreateAsync currently accepts mutable AISettings and loads a key by normalized provider name. This cannot distinguish two profiles of one provider and allows a request to observe settings changed after it started.

SettingsService exposes live sections and saves them at application exit. Options pages bind two-way to live instances and have no page-level Apply contract. AI Settings therefore needs a narrow draft/commit path rather than a broad options-system redesign.

AIChatPaneModel currently creates providers from singleton settings and stores one flat Messages list in ChatHistory. Other consumers include output, explain, rename, batch rename, summary, search, analyzer, context-builder, and AI service classes; all must resolve the shared target through one service.

## Settled behavior and invariants

### Profiles

- At least one profile always exists.
- Profile IDs are generated once and never change, including after rename or duplication.
- Display names are required, trimmed, and unique case-insensitively.
- Profile order is user-controlled and persisted.
- Add creates an unsaved OpenAI draft with a unique New Profile-style name, default endpoint, one default model, and no secure-store write. Cancel discards draft and ID.
- Duplicate creates a new ID, copies non-secret fields/models, clears the secret, and appends after Save.
- New and duplicated profiles append after Save.
- Profile reorder does not change selection or restart chat.

### Models

- Model names are required, trimmed, and unique case-insensitively within a profile.
- At least one model remains in every profile.
- Add, rename, remove, and reorder are manual operations.
- Rename preserves position and updates active/remembered references atomically.
- Removing an active model selects the immediately following visible model, or the first remaining model when deleting the last.
- Removing the only model is rejected.

### Shared selection

- Active AI Selection is one application-wide profile/model pair used by all AI features.
- Each profile remembers its last-selected model. Activating a profile restores it when valid; otherwise use the first model in order.
- Selector changes persist immediately and independently of unsaved settings drafts.
- Chat selectors remain usable for non-ready configurations.
- A non-ready active selection blocks requests and shows the exact reason plus an Open AI Settings action.

### Deletion

- The only profile cannot be deleted.
- Deleting an active profile selects the immediately following visible profile, or wraps to the first remaining profile when deleting the last.
- Delete the profile secret before removing metadata. Missing secret counts as success. Any deletion failure leaves the profile unchanged and shows an actionable error.
- Deletion does not cancel an in-flight request; its immutable snapshot may finish. Future requests cannot resolve the deleted profile.

### Requests and diagnostics

- Every request captures an immutable profile/model configuration snapshot at start.
- Selection, profile edits, key replacement, reorder, and deletion affect future requests only.
- Normal user cancellation remains available.
- Test Connection is optional diagnostics, targets only the currently selected model in the profile editor, identifies exact profile/model, and never gates Save or use. Results are session-only and become stale after endpoint/provider/model/key changes.

## Target domain and data model

Introduce minimal immutable/runtime types in ICSharpCode.ILSpyX/AI or Settings:

- AIProfile: stable Id, Name, ProviderType, BaseUrl, ordered Models, LastSelectedModel, and non-secret HasStoredKey hint.
- AIProviderDescriptor: persisted provider ID, friendly label, endpoint/model defaults, key requirement (Required, Optional, None), and implementation capability.
- AISelection: active profile ID plus model name.
- AISelectionSnapshot: immutable resolved target containing profile ID, provider type, endpoint, model, credential lookup ID, and global request preferences needed by the provider/context builder.
- AIConfigurationState: Ready or a precise non-ready reason (privacy consent, missing profile/model, invalid endpoint/provider, missing required key, secure-store unavailable).
- AIConversationTarget: immutable history metadata: profile ID, profile-name snapshot, provider type, endpoint, and model.

Keep global context/privacy preferences (MaxContextTokens, streaming, Include IL, call graph, privacy consent) application-wide. Do not pass mutable AISettings into provider or feature services after migration.

## Provider capability matrix

| Persisted ID | UI label | Implementation | Endpoint | Key |
|---|---|---|---|---|
| openai | OpenAI | OpenAI-compatible | Required absolute HTTP(S) | Required |
| anthropic | Anthropic | Anthropic provider | Required absolute HTTP(S) | Required |
| ollama | Ollama | OpenAI-compatible | Required absolute HTTP(S); HTTP allowed locally | None |
| custom | Custom OpenAI-compatible | OpenAI-compatible | Required absolute HTTP(S) | Optional |

All profiles require an endpoint and at least one model. Preserve these IDs in persisted data. Friendly labels are UI-only. Capability metadata, not provider-name conditionals spread through the UI, controls key visibility and readiness.

## Persisted XML schema

Use XML order for profile/model order. HasStoredKey is a non-secret hint; secure storage remains authoritative. Do not serialize API keys, drafts, test results, or transient errors.

    <AISettings>
      <SchemaVersion>2</SchemaVersion>
      <ActiveProfileId>profile-guid</ActiveProfileId>
      <MaxContextTokens>32000</MaxContextTokens>
      <StreamResponses>true</StreamResponses>
      <SendIL>false</SendIL>
      <SendCallGraph>false</SendCallGraph>
      <PrivacyConsentAccepted>false</PrivacyConsentAccepted>
      <Profiles>
        <Profile Id="profile-guid" Name="Default">
          <ProviderType>openai</ProviderType>
          <BaseUrl>https://api.openai.com</BaseUrl>
          <HasStoredKey>true</HasStoredKey>
          <LastSelectedModel>gpt-4o</LastSelectedModel>
          <Models><Model>gpt-4o</Model></Models>
        </Profile>
      </Profiles>
      <CredentialMigration State="Complete" />
    </AISettings>

Serialize no API key, draft, test result, or transient error. Normalize and validate on load; repair malformed collections to a valid minimum profile without discarding valid profiles.

## Versioned legacy migration

Implement an idempotent migration from schema 0/1 singleton fields to schema 2:

1. Read legacy provider, endpoint, model, and key placeholder; preserve global preferences.
2. Create one Default profile with a generated stable ID. Use provider defaults for blank legacy model/endpoint. The legacy model becomes the first model when present.
3. Set that profile/model active.
4. Legacy secure keys are keyed by provider ID. Read them without logging or serializing values.
5. Save profile metadata and, after confirming the new profile-ID key is stored, mark migration complete and delete the legacy provider-key entry.
6. Until confirmation, mark Pending Credential Migration; retain the legacy key and retry on later load/save. Never create duplicate profiles on retry.
7. If no legacy settings exist, create a valid OpenAI default profile and leave privacy consent false.

Migration must be non-destructive and resilient to interruption. A schema/version marker prevents duplicate conversion.

## Secure-key identity and failure handling

Change SecureKeyStorage terminology from provider to credential/profile key ID where touched. Use a canonical identifier such as profile-{guid:N}; it satisfies the backend allowed-character and length limits and cannot collide with raw legacy provider IDs. Keep legacy IDs only for migration lookup.

- UI displays only whether a key is stored; never reveals or recovers a key into a text field.
- Untouched key input preserves the existing secret. Entered input replaces it only on Save.
- Explicit Remove Key requires confirmation and is allowed even when the provider requires a key; the profile then becomes non-ready.
- Changing provider type or endpoint preserves the secret but marks diagnostics stale.
- Delete profile: delete secret first; missing secret is successful; failure aborts metadata deletion.
- Save sequence: validate draft, write replacement secret, persist metadata, then remove old secret. On write/metadata failure retain old profile and old secret. If old-secret cleanup fails after successful metadata save, keep the new state active and show a retryable warning.

XML and OS secure storage are not one transaction. Implement compensating rollback/retry, persist a pending cleanup/migration marker where needed, and make operations idempotent. Never log keys, include them in exceptions, or serialize them.

## Selection service and immutable snapshots

Add an application-scoped IAISelectionService (or equivalent AI configuration service) owned by SettingsService/composition. Responsibilities:

- Load and validate profiles and active selection.
- Expose observable profile/model collections and AIConfigurationState.
- Apply selector changes with immediate persistence.
- Resolve an AISelectionSnapshot for a request, including secure credential lookup and global preferences.
- Apply deterministic profile/model deletion fallback.
- Publish one settings-change notification for consumers.

The service distinguishes saved state from ProfileDraft. Draft edits are isolated until Save commits metadata and credential changes as one user-visible operation. Resolution never reads editor objects.

## Provider factory and service API changes

Replace the current CreateAsync(AISettings, CancellationToken) contract with one accepting an immutable AISelectionSnapshot (or resolver-owned request target). The factory validates privacy, provider capability, endpoint, model, and credential readiness, then creates Anthropic or OpenAI-compatible providers. It must not mutate settings or cache a mutable API key back into AISettings.

Update AIExplanationService, rename/summarize/batch services, context builders, search/analyzer integrations, output pane, dialogs, and chat to resolve the snapshot through the shared service. Pass global context preferences separately or include them as immutable snapshot fields. Preserve cancellation tokens and existing provider abstractions.

## UI plan

### AI Settings master-detail editor

Extend AISettingsViewModel.cs and AISettingsPanel.axaml with:

- Ordered profile list and Add, Duplicate, Move Up, Move Down, Delete commands.
- Detail editor for name, provider type, endpoint, models, remembered model, key status, Replace Key, Remove Key, Test Connection, Save, and Cancel.
- Provider capability metadata controls labels, key requirement, defaults, and readiness hints.
- Model list supports add/edit/remove/reorder. Inline validation prevents invalid Save.
- Save commits draft only after structural validation and secure-store sequence completes. On failure retain draft and saved state.
- Delete confirmation explains deterministic fallback and history effect.

Do not bind editor controls directly to the live persisted profile. Existing global context/privacy controls may remain two-way or use the AI service's narrow commit path.

### AI Chat selectors and readiness

Add compact profile and model selectors at the top of AIChatPane.axaml. They edit shared selection, show friendly names, and remain enabled for non-ready configurations. Show exact readiness error with Open AI Settings. A successful profile/model change starts a new conversation; reorder-only changes do not.

### Key UX

Use password/secure input for replacement, never display the stored value, show only stored/not stored, and clear replacement input after Save or Cancel. Remove requires confirmation. Test output identifies profile and selected model but never includes credentials.

## Chat history and conversation boundaries

Replace the flat history shape with a backward-compatible versioned document:

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

Load legacy flat Messages as one legacy/unknown-target conversation. It remains readable but is never sent to a provider until the user starts a new conversation. Retain prior conversations when selection changes.

Target identity includes profile ID, provider type, endpoint, and model. Changes to any of those start a new conversation. Profile display-name changes, model reorder, key replacement, and test status do not. Store immutable profile-name metadata for display. Deleted profiles remain readable as Profile Name (deleted); reopened deleted-target history is read-only. Continuing it requires an explicit new conversation under the active selection. Markdown export should include target metadata where useful but is not profile export.

## Consumer migration matrix

| Consumer | Required change |
|---|---|
| ILSpy/AI/AIChatPaneModel.cs | Selectors, target-bound conversations, snapshot per send, readiness UI, no cross-target context |
| ILSpy/AI/AIOutputPaneModel.cs | Resolve shared snapshot per operation |
| Explain/rename/batch rename dialogs and context entries | Resolve snapshot through shared service; preserve cancellation/errors |
| AssemblySummaryContextMenuEntry.cs and GenerateDocsContextMenuEntry.cs | Use shared target |
| SearchPaneModel.cs and AISearchStrategy.cs | Use shared target and readiness handling |
| Analyzer AI entries and AnalyzerContext.cs | Use shared target; no mutable singleton passed |
| AIExplanationService.cs, RenameSuggester.cs, BatchRenameSuggester.cs | Accept immutable snapshot/resolver result |
| ContextBuilder.cs | Keep global context preferences separate from target |
| AIProviderFactory.cs | Profile-ID credential lookup and capability-aware validation |

Search all AISettings, CreateAsync, provider/model reads, and secure-key calls after migration; no feature may bypass the selection service.

## Validation and error handling

- Trim names, endpoints, and model names before validation/storage.
- Names unique case-insensitively; models unique case-insensitively within profile.
- Endpoint must parse as an absolute http or https URI. HTTP is allowed for local services.
- Provider ID must be supported and have capability metadata.
- At least one model per profile and one profile overall.
- Structural invalidity blocks Save. Missing required credential does not block Save but blocks active requests.
- Readiness messages name the exact missing/invalid field and offer Open AI Settings.
- Secure-store unavailable is distinct from missing key.
- Connection-test failures are diagnostic only and do not disable Save/use.
- Handle malformed XML/JSON, duplicate IDs, missing active IDs, and deleted active targets by deterministic repair and notification.

## Concurrency and state transitions

- Serialize AI configuration commits to prevent interleaved profile/key writes.
- Capture snapshots before provider creation and before reading request messages.
- Selection changes publish after persistence succeeds; failed persistence leaves the prior selection active.
- Draft Save runs off the UI thread for secure-store/file I/O, then applies observable state on the UI thread.
- Profile deletion and migration operations are idempotent and retryable.
- In-flight requests retain snapshot endpoint/model/key and may complete after deletion. Do not mutate provider instances from settings notifications.
- Normal cancellation remains scoped to the initiating request.

## Phased work packages

### Phase 1: Domain and persistence foundation

Files: ICSharpCode.ILSpyX/Settings/AISettings.cs, new profile/descriptor/selection types, XML helpers, ILSpy/SettingsService.cs.

Implement schema 2 serialization, validation, defaults, deterministic repair, observable shared selection, and immediate selection persistence. Keep global preferences intact.

### Phase 2: Secure storage and migration

Files: ICSharpCode.ILSpyX/AI/SecureKeyStorage.cs, platform backends, migration helpers, settings tests.

Add profile credential IDs, legacy lookup, pending migration/cleanup markers, compensating rollback, and no-secret logging tests.

### Phase 3: Provider resolution contract

Files: ICSharpCode.ILSpyX/AI/AIProviderFactory.cs, provider tests/fakes, AI service contracts.

Introduce immutable snapshots, capability registry, readiness errors, profile-ID lookup, and update all factory tests.

### Phase 4: AI Settings editor

Files: ILSpy/Options/AISettingsViewModel.cs, ILSpy/Options/AISettingsPanel.axaml, supporting converters/templates/resources.

Implement master-detail CRUD, drafts, model ordering, key UX, validation, Save/Cancel, deterministic deletion confirmation, and selected-model connection tests.

### Phase 5: Shared consumer migration

Files listed in the consumer matrix plus AIOutputPaneModel and dialogs.

Route every AI operation through the selection service, separate global preferences, and preserve existing cancellation/error behavior.

### Phase 6: Chat selectors and history

Files: ILSpy/AI/AIChatPaneModel.cs, ILSpy/AI/AIChatPane.axaml, ICSharpCode.ILSpyX/AI/ChatHistory.cs, new conversation/target types, export code.

Add selectors, target transitions, versioned history migration, deleted-profile read-only behavior, and snapshot-per-request semantics.

### Phase 7: Integration, cleanup, and compatibility

Remove singleton provider/key/model call paths only after all consumers migrate. Update documentation and diagnostics. Verify old schema loads, saves as schema 2, and retains global preferences.

## Test strategy

### Unit tests

- Profile/name/endpoint/model validation and case-insensitive uniqueness.
- Defaults and provider capability matrix, including Anthropic visibility and Ollama/custom key rules.
- Profile/model ordering and deterministic deletion fallback.
- Per-profile model memory and active-selection persistence.
- Draft isolation, duplicate behavior, rename reference updates, and minimum-count guards.
- XML round-trip, malformed data repair, schema idempotence, and legacy migration with blank/nonblank fields.
- Secure-key canonicalization, profile-ID isolation, no-secret serialization/logging, replacement rollback, deletion failure, stale cleanup, and pending migration retry.
- Immutable snapshot behavior under selection/configuration/deletion changes.
- Provider factory readiness and selected-model connection-test targeting.
- Chat-history schema migration, target boundaries, deleted-profile display/read-only behavior, and markdown target metadata.

### Integration/UI tests

- Settings Save failure leaves persisted state and old secret unchanged while draft remains.
- Selector changes immediately persist and propagate to chat/output/explain/search/analyzers.
- Non-ready selector remains usable and opens AI Settings from exact error.
- Switching target starts a new conversation without sending prior messages.
- In-flight request completes against its original snapshot after selection/profile deletion.
- Add, duplicate, reorder, rename, remove, key replacement/removal, and connection diagnostics.

Update existing AISettingsTests, SecureKeyStorageTests, AIExplanationServiceTests, provider fakes, context-builder tests, and add focused selection/history/migration suites. No key value may appear in test snapshots or assertion messages.

## Acceptance criteria

- Existing singleton configurations load into exactly one stable-ID profile without data loss.
- Multiple profiles of the same provider coexist with independent secure keys.
- At least one profile and one model are always enforced.
- Active profile/model is shared by every AI feature and persists immediately.
- Each profile restores its remembered model.
- Active deletion fallback is deterministic and tested.
- Unsaved drafts cannot affect requests or selection.
- Stored keys are never revealed, serialized, logged, or copied between profiles.
- Required-key absence blocks only active requests, with exact actionable readiness UI.
- Test Connection checks only the editor's selected model and never gates use.
- Every request uses an immutable snapshot; in-flight behavior is stable.
- Chat history preserves old conversations and prevents cross-target context leakage.
- Legacy XML and secure credentials migrate idempotently with retry/rollback behavior.
- Existing global AI preferences and privacy consent retain semantics.
- All AI consumers use the shared resolver; no singleton provider/model/key bypass remains.

## Rollout and backward compatibility

Ship schema migration and resolver compatibility before switching UI consumers. Keep a temporary adapter only if needed for in-repo composition points; remove it after all consumers migrate. Existing chat JSON must continue loading. Schema 2 is a forward migration; if downgrade support is required, add an explicit backup policy rather than silently flattening profiles.

## Risks and mitigations

| Risk | Mitigation |
|---|---|
| XML and secure store diverge | Ordered writes, compensating rollback, pending cleanup/migration markers, idempotent retry |
| Key leakage | Stable profile IDs, secure-only values, redacted diagnostics, absence tests |
| Cross-target prompt leakage | Immutable snapshot plus conversation boundary and transition tests |
| Options live binding mutates saved state | Dedicated ProfileDraft and AI-specific commit path |
| Deleted profile referenced by history/request | Preserve snapshots; mark history read-only/deleted; block only future resolution |
| Provider capability drift | Central descriptor registry and persisted IDs |
| Missed consumer | Repository-wide search for old factory/settings/key APIs plus compile/test gate |

## Out of scope and follow-ups

Import/export profiles, automatic model discovery, provider authentication flows, usage dashboards, and multi-target conversations are follow-ups. Future profile export must omit secure credentials. Existing chat markdown export may gain target metadata without becoming a profile export format.

## Implementation checklist

- [x] Keep root CONTEXT.md glossary-only.
- [ ] Define profile, provider capability, selection, snapshot, readiness, and conversation target contracts.
- [ ] Implement schema 2 XML and idempotent singleton migration.
- [ ] Implement profile-ID secure-key storage and compensating failure handling.
- [ ] Add shared application-scoped selection service with immediate persistence.
- [ ] Replace mutable AISettings provider-factory API with immutable snapshot API.
- [ ] Migrate every AI consumer in the matrix.
- [ ] Build AI Settings master-detail draft editor and model CRUD.
- [ ] Add key replacement/removal UX with redaction guarantees.
- [ ] Add Chat profile/model selectors and readiness navigation.
- [ ] Version chat history and enforce target boundaries/deleted-history behavior.
- [ ] Add migration, persistence, secure storage, selection, provider, history, and UI tests.
- [ ] Run repository-wide searches for legacy singleton call paths.
- [ ] Run dotnet build and focused test suites before implementation handoff.
