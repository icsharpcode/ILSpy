# Phase 2: Streaming and Enhanced Context

**Status:** Implemented  
**Dependencies:** Phase 1 first user-facing features  
**Goal:** Replace blocking AI output with a responsive dockable pane and provide optional IL, literals, attributes, and bounded call-graph context.

## Scope

### In scope

- Dispatcher-safe streaming from `IAsyncEnumerable<string>` into a dockable output pane.
- Cancellation of provider enumeration and in-flight HTTP requests.
- Clear and copy actions for generated output.
- Explain-with-AI integration with the output pane.
- Assembly-tree action for assembly summaries.
- Optional IL and call-graph context controlled by `AISettings`.
- String-literal and attribute extraction from the selected symbol.
- Deterministic context-budget trimming with lower-priority metadata removed before C# code.
- Focused tests for streaming orchestration, enhanced context, cancellation, and request errors.

### Out of scope

- Follow-up chat turns.
- Markdown rendering or syntax highlighting in the output pane.
- Anthropic-specific provider implementation.
- Rename, security analysis, semantic search, and other Phase 3/4 features.

## Implementation Notes

- The output pane is hidden by default and exported through the existing tool-pane registry. Its layout state uses the standard Dock persistence path.
- Stream consumption runs outside the Avalonia UI thread; only property updates are dispatched to `Dispatcher.UIThread`.
- Overlapping requests are isolated by cancellation-source identity, so an older request cannot clear or overwrite a newer request.
- Assembly-summary context construction runs on the same background streaming boundary as provider enumeration.
- Provider failures are classified before they reach the UI. Cancellation remains distinguishable from request failure, and provider response bodies are not surfaced.
- Call-graph results are bounded to ten deterministic entries and only invocation opcodes are treated as callees.

## Acceptance Criteria

- [x] Streaming response chunks appear in arrival order without blocking the UI.
- [x] Cancel stops the current request and reports cancellation without a false provider error.
- [x] A new request gets a fresh cancellation scope and is not affected by the prior request's cleanup.
- [x] Clear removes the current target, response, error, and status state.
- [x] Copy is available only for a non-empty completed response and copies the exact displayed text.
- [x] Explain-with-AI displays output in the dockable pane.
- [x] Assembly summaries are available only for a single selected assembly-tree node.
- [x] Summary context includes assembly identity/version, target framework, namespaces, public type count, entry point, attributes, and largest public types.
- [x] Optional IL and call-graph settings are honored.
- [x] Context trimming preserves the configured token budget and removes optional context before truncating code.
- [x] AI-filtered `net10.0` tests pass without network access.

## Implementation Record

**Implemented:** August 17, 2026

- Phase 2 implementation landed in the existing AI feature commits, with follow-up hardening for streaming error classification, stale-request cleanup, background consumption, metadata-safe entry-point handling, and deterministic context extraction.
- Validation: `ICSharpCode.ILSpyX.Tests` AI filter passes **98/98** on `net10.0` with the installed SDK `10.0.400`.
- Validation: `dotnet build ILSpy.sln --no-restore` passes with 19 projects, 0 errors, and 0 warnings under the installed SDK `11.0.100-preview.7.26381.103`.

**Document Version:** 1.0  
**Created:** August 17, 2026
