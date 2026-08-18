# ILSpy AI Configuration

Domain language for configuring and selecting AI services used by ILSpy features.

## Language

**AI Profile**:
A named AI service configuration containing one provider type, one API endpoint, one credential, and an ordered set of models. Each profile has an identity that remains stable when its name changes.
_Avoid_: Provider, account, connection

**Provider Type**:
The protocol family ILSpy uses to communicate with an AI service, such as OpenAI-compatible or Anthropic. Multiple AI profiles may share the same provider type.
_Avoid_: Provider, profile

**Model**:
A model name available through one AI profile. The same model name in different profiles identifies distinct selectable targets.
_Avoid_: Provider, profile

**Active AI Selection**:
The application-wide pairing of one AI profile and one of its models used by all AI features. If the active profile is deleted, ILSpy deterministically selects the next available profile.
_Avoid_: Chat model, pane-local selection, default provider

**AI Selection Controls**:
Compact profile and model selectors in the AI Chat pane that edit the shared Active AI Selection; profile creation and other management remain in AI settings.
_Avoid_: pane-local selection, chat-only selector

**Request Selection Snapshot**:
The immutable AI Profile and Model pairing captured when an AI request starts. Later changes to Active AI Selection affect future requests only.
_Avoid_: live selection, mutable request target

**Profile Secret**:
The API credential stored in the operating system's secure credential store and associated with one AI Profile identity. Removing a profile requires successful removal of its Profile Secret or confirmation that no secret exists.
_Avoid_: shared provider key, serialized API key

**Deterministic Selection Fallback**:
The rule that preserves a valid Active AI Selection after deletion: choose the immediately following item in visible order, or wrap to the first remaining item when the deleted item was last.
_Avoid_: arbitrary fallback, last-used fallback

**AI Configuration State**:
The validation status of the Active AI Selection, including endpoint, provider requirements, model, and credential readiness. A non-ready state blocks AI requests and offers navigation to AI settings.
_Avoid_: connection state, chat-only error

**Profile Validity**:
Whether an AI Profile has valid non-secret structure: identity, unique display name, provider type, endpoint, and at least one valid model. Credential readiness is separate, so a valid profile may be saved without a required Profile Secret but cannot serve requests.
_Avoid_: connection success, credential readiness

**Conversation Target Record**:
The immutable profile identity, profile-name snapshot, provider type, and model name attached to an AI Chat conversation. Profile renames do not rewrite it, and profile deletion leaves the conversation readable but not directly resumable.
_Avoid_: live profile label, mutable history target

**Conversation Target Boundary**:
An AI Chat conversation belongs to the profile identity, provider type, endpoint, and model selected when it was created. Changing any of those target attributes starts a new conversation so prior context is not sent to a different target.
_Avoid_: cross-provider history, shared conversation context

**Profile Draft**:
An unsaved AI Profile being edited or created in AI settings. Drafts have a temporary stable identity for editor state but create or update secure credentials only when validation succeeds on Save.
_Avoid_: half-saved profile, persisted draft

**Immediate Active Selection Persistence**:
The Active AI Selection is saved as soon as a profile or model selector changes, independently of unsaved profile-editor drafts.
_Avoid_: settings-save selection, pane-local selection

**Per-Profile Model Memory**:
Each AI Profile remembers its last-selected model. Activating that profile restores the remembered model when it remains available; otherwise deterministic model fallback chooses a valid replacement.
_Avoid_: global last model, provider-wide model memory

**Draft Isolation**:
Unsaved profile edits do not affect AI requests or persisted Active AI Selection. The last saved profile remains authoritative until a complete Save succeeds.
_Avoid_: live editor state, partial configuration

**Pending Credential Migration**:
A legacy AI credential remains authoritative until it has been confirmed under the migrated AI Profile identity. The legacy credential is removed only after that confirmation.
_Avoid_: failed profile, discarded legacy key
