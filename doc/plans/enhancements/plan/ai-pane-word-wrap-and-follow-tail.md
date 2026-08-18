# AI Pane Word Wrap and Follow-Tail Scrolling

**Status:** Ready for implementation  
**Created:** 2026-08-19  
**Scope:** AI Output, AI Chat, Explain, and AI Settings

## Summary

Add one persisted **AI Word Wrap Preference**. It defaults to enabled, is controlled from AI Settings, and applies live to AI Output, AI Chat, and Explain. Disabling it exposes horizontal scrolling for long lines.

Repair scrolling while preserving intentional streaming auto-scroll. Replace unconditional `ScrollToEnd()` with **AI Follow-Tail Scrolling**: appended content remains visible only while the relevant viewport is at or within 24 device-independent pixels of its bottom. User scrolling upward suspends follow-tail; returning to the bottom resumes it.

## Goals

- Persist a word-wrap setting with default `true`.
- Add its only UI control to AI Settings beside `Stream responses`.
- Update all attached markdown editors immediately when the setting changes.
- Retain horizontal navigation for unwrapped long lines.
- Keep streaming output following the tail only while the user is already at the bottom.
- Give each surface one vertical scroll owner.
- Preserve viewport and follow-tail state across a wrap toggle.
- Preserve current streaming, cancellation, chat history cap (100 messages), and virtualization behavior.

## Non-goals

- No new markdown renderer or editor dependency.
- No changes to streaming cadence, providers, request behavior, or chat history format.
- No global decompiler-editor scroll change.
- No forced scroll on completed or non-streaming responses.

## Settled behavior

### Word wrap

- Shared application-level preference applies to AI Output, every AI Chat message, and Explain.
- New installations, missing XML fields, malformed XML values, and settings defaults resolve to enabled.
- Changes apply live to every visible editor; reopening panes is unnecessary.
- With wrapping off, AI Output and Explain use their editor horizontal scrollbar. Chat message editors may horizontally scroll, but never become independent vertical scroll containers.
- Wrap changes retain the approximate reading position. A surface that was following tail goes to the new tail after reflow; a surface manually scrolled away stays manually scrolled away.

### Follow-tail

- Threshold: `24` DIP from bottom.
- State is per streaming surface.
- At stream start, activate follow-tail only if the viewport is at/near its bottom.
- While active, stream updates move the surface to its new bottom.
- Scrolling above the threshold deactivates it. No later streaming update may change the viewport until it becomes near-bottom again.
- Returning within the threshold reactivates it for subsequent chunks.
- AI Output follow-tail owns the output editor viewport.
- AI Chat follow-tail owns the conversation `ListBox` viewport, never a message editor.
- Clear/new response resets state from the resulting viewport. Completion does not cause a later forced scroll.

## Current failure analysis

`AISettings` has no word-wrap property. `MarkdownTextEditor` hard-codes `WordWrap = true`, therefore no persistence or user control exists.

`StreamingTextControl.axaml.cs` invokes `Editor.ScrollToEnd()` for every response replacement and append. This directly overrides manual scrolling. `AIOutputPane.axaml` also wraps an AvaloniaEdit editor in an outer `ScrollViewer`, leaving competing vertical scroll owners.

`AIChatPane` uses a `ListBox` for conversation scrolling, but `ChatMessageControl.axaml` sets `MaxHeight=360` on each editor. That creates nested vertical scrolling. `ChatMessageControl.axaml.cs` also performs `SetText()` for every growing `ChatMessage.Content` snapshot, which can reset editor layout and offsets.

`DecompilerTextView.axaml.cs` is the local precedent: AvaloniaEdit's public offset APIs are currently ineffective, so it locates the editor template `ScrollViewer` through the visual tree and sets its `Offset` directly. Reuse that narrow approach for AI controls.

## Ownership

| Concern | Owner |
|---|---|
| XML default/load/save | `ICSharpCode.ILSpyX/Settings/AISettings.cs` |
| AI Settings checkbox | `ILSpy/Options/AISettingsPanel.axaml` |
| Live wrap subscription | `ILSpy/AI/Controls/MarkdownTextEditor.cs` |
| Editor viewer discovery / offset logic | New small AI internal scroll helper |
| Output follow-tail | `ILSpy/AI/StreamingTextControl.axaml.cs` |
| Output vertical scrollbar | Embedded AvaloniaEdit editor |
| Chat follow-tail / vertical scrollbar | Conversation `ListBox` in `AIChatPane` |
| Chat message text and horizontal scrollbar | `ChatMessageControl` / `MarkdownTextEditor` |
| Explain wrap behavior | Shared `MarkdownTextEditor` behavior |

## Implementation phases

### 1. Persist the preference

Files:

- `ICSharpCode.ILSpyX/Settings/AISettings.cs`
- `ICSharpCode.ILSpyX.Tests/Settings/AISettingsTests.cs`

Changes:

1. Add `bool wordWrap = true` and `public bool WordWrap`, using existing `SetProperty`.
2. Set `WordWrap = true` in `LoadFromXml(null)`.
3. Load with `ReadBoolean(section, nameof(WordWrap), true)`. This makes absent and malformed legacy values enabled.
4. Emit `<WordWrap>` from `SaveToXml()`. No schema version is needed for one backward-compatible additive field.
5. Add tests for constructor default, null-load default, missing-element default, malformed fallback, false/true XML round-trips, and notification.

### 2. Expose it in AI Settings

Files:

- `ILSpy/Options/AISettingsPanel.axaml`
- `ILSpy/Options/AISettingsViewModel.cs` only if its property filtering proves necessary

Changes:

1. Add `<CheckBox Content="Word wrap" IsChecked="{Binding Settings.WordWrap, Mode=TwoWay}" />` adjacent to `Stream responses`.
2. Retain the existing live binding to `SettingsService.AISettings`; do not add a pane-local setting or Apply flow.
3. Add `nameof(AISettings.WordWrap)` to view-model invalidation only if an existing filter requires it. It does not need a dedicated wrapper property.

### 3. Make MarkdownTextEditor observe the setting

Files:

- `ILSpy/AI/Controls/MarkdownTextEditor.cs`
- New `ILSpy/AI/Controls/AIEditorScrollState.cs` or similarly narrow internal helper

Changes:

1. Preserve constructor `WordWrap = true` as pre-attachment fallback.
2. On attachment, resolve live `AISettings` through the existing application composition/settings pattern and subscribe to `PropertyChanged`.
3. On `WordWrap` changes, apply the property to the editor. Unsubscribe symmetrically on detachment to support recycled controls and prevent leaks.
4. In the helper, find the AvaloniaEdit template `ScrollViewer` with `GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault()`, matching `DecompilerTextView`. Cache only while attached.
5. Define `IsNearBottom` as `max(0, Extent.Height - (Offset.Y + Viewport.Height)) <= 24`.
6. Capture horizontal/vertical offsets and active follow-tail before a wrap change. Apply wrapping, await/defer through the next UI layout, then restore the tail if active; otherwise restore clamped offsets. Keep inactive state inactive even if reflow happens to place it near the bottom.
7. Keep helper UI-thread-only. It should expose only capture, bottom detection, restore, and conditional bottom movement; no singleton or timer-driven auto-scroll.

### 4. Repair AI Output scroll ownership and follow-tail

Files:

- `ILSpy/AI/StreamingTextControl.axaml.cs`
- `ILSpy/AI/StreamingTextControl.axaml` if named parts are needed
- `ILSpy/AI/AIOutputPane.axaml`
- `ILSpy/AI/AIOutputPaneModel.cs` only if a narrow stream lifecycle signal is needed

Changes:

1. Remove unconditional `Editor.ScrollToEnd()` calls from `TextProperty` updates and `AppendText`.
2. Give `StreamingTextControl` one follow-tail controller for its editor. Before document mutation, retain the controller state. After replacement/append and layout, move to bottom only while active.
3. Keep `AppendText` efficient. For whole-response snapshots that must call `SetText`, capture and restore state so the replacement cannot reset user position. Prefer known suffix append only when it can be proven safe.
4. Treat clear/new request as a new lifecycle: clear document, reset controller, then initialize it from resulting viewport.
5. Remove the outer `ScrollViewer` in `AIOutputPane.axaml`. The embedded editor becomes sole owner of both axes. Preserve existing dock layout, buttons, status, and errors.
6. Verify output editor stretches to pane and displays horizontal scrollbar when wrap is off.

### 5. Repair AI Chat vertical ownership and follow-tail

Files:

- `ILSpy/AI/AIChatPane.axaml`
- `ILSpy/AI/AIChatPane.axaml.cs` if needed to obtain ListBox's real viewer
- `ILSpy/AI/Controls/ChatMessageControl.axaml`
- `ILSpy/AI/Controls/ChatMessageControl.axaml.cs`
- `ILSpy/AI/AIChatPaneModel.cs` only for a narrow stream-start/end notification

Changes:

1. Remove `MaxHeight=360` from `ContentEditor`. Ensure message editors measure to their full wrapped height and have no vertical scrollbar ownership.
2. Retain editor horizontal scrolling for `WordWrap=false`.
3. Locate the conversation `ListBox` scroll viewer after template application and attach a follow-tail controller there. Its offset-change handler must only update state, never move the viewport.
4. At assistant stream start, initialize state from list bottom. After each relevant message layout update, move the list to bottom only if active. Dispatcher-post only after the ListBox has measured its new extent.
5. Remove any per-message auto-scroll behavior. Do not use repeated `BringIntoView`; it bypasses user intent and can reintroduce snapping.
6. Update streamed message content without destroying scroll state. If snapshots are always cumulative, append their verified suffix. Otherwise capture/restore the message editor's horizontal state around `SetText`. The list controller remains responsible for vertical following.
7. Preserve `MaxMessages = 100` and virtualization; do not replace `ListBox` with an eager stack panel.

### 6. Explain and cross-surface state

Files:

- `ILSpy/AI/ExplainDialog.axaml` only if explicit scrollbar visibility needs adjustment
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`

Changes:

1. No Explain-specific setting binding. It inherits live wrapping through `MarkdownTextEditor`.
2. Confirm Explain remains its own editor scroll owner and gets automatic horizontal scrolling with wrap disabled.
3. Do not add streaming follow-tail to Explain unless existing behavior has a distinct streaming path; this request scopes follow-tail to AI Output and active AI Chat responses.

### 7. Tests and verification

Automated tests:

1. Extend `AISettingsTests` as described in phase 1.
2. Unit-test extracted pure scroll-state math: exact bottom, 24 DIP threshold, 24.1 DIP inactive, negative/coerced remaining height, and inactive-to-active return.
3. Unit-test controller policy: following append scrolls; inactive append retains position; clear/restart resets state; completion does not force scroll.
4. Add focused Avalonia control tests only where the solution already has appropriate UI-test infrastructure; avoid brittle pixel tests for state logic.

Manual regression matrix:

| Scenario | Expected result |
|---|---|
| New/default AI settings | `Word wrap` shown and checked |
| Toggle wrap off/on while panes visible | Output, Chat, Explain update immediately |
| Wrap off with long lines | Horizontal scrolling available; no nested chat vertical scrolling |
| Output stream at bottom | Latest text remains visible |
| Scroll output upward during stream | Later chunks do not move viewport to top or bottom |
| Return output to bottom | Later chunks follow tail again |
| Chat assistant stream at bottom | Conversation list follows tail |
| Scroll chat upward during stream | List remains stable; individual messages do not steal wheel scrolling |
| Return chat to bottom | Follow-tail resumes on later chunk |
| Toggle wrap while scrolled upward | Approximate reading position retained; no follow-tail reactivation |
| Toggle wrap at bottom | Follow-tail remains active after reflow |
| Reopen/recycle panes | No duplicate handlers or repeated scroll operations |

## Acceptance criteria

- `AISettings.WordWrap` persists, defaults to true, and gracefully reads legacy XML.
- AI Settings is the only configuration surface for word wrap.
- Every attached markdown editor changes wrapping live.
- No normal AI streaming path calls unconditional `ScrollToEnd()`.
- Upward manual scrolling is respected during output and chat streams.
- Follow-tail remains functional at the bottom and resumes after returning to bottom.
- AI Output has one vertical owner: its editor.
- AI Chat has one vertical owner: conversation `ListBox`.
- Wrap changes preserve user reading position and captured follow-tail state.
- Existing copy, clear, cancellation, streaming, history cap, and virtualization behavior continues working.

## Risks and mitigation

| Risk | Mitigation |
|---|---|
| Current AvaloniaEdit public scroll methods are ineffective | Access template `ScrollViewer.Offset` using codebase precedent. |
| Full snapshot replacements reset layout | Capture/restore state; append verified suffixes where safe. |
| List virtualization delays extent update | Perform conditional tail movement after layout via UI dispatcher. |
| Removing output outer ScrollViewer changes sizing | Verify dock layout; add normal sizing only if required, never a second viewer. |
| Settings subscriptions outlive recycled controls | Attach/detach symmetrically and test reopening panes. |
| Wrap reflow changes extents abruptly | Preserve captured state explicitly; clamp restored offsets. |

## Rollback

Changes are isolated to AI settings, AI controls, and pane XAML. Removing the `WordWrap` field/binding and reverting the scroll controller restores current behavior. Older binaries ignore the additive XML field.
