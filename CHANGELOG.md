# Changelog

This project does not keep an exhaustive release log; this file only records user-facing
highlights that are worth noting between releases.

## AI Chat - Wired /explain and /rename Commands

### New features
- `/explain` now runs the dedicated explanation pipeline in the chat: the selected symbol's
  full token-budgeted decompilation context (decompiled C#, attributes, interfaces, string
  literals, optional callers/callees and IL) is sent with the `explanation` system prompt,
  and the answer streams into the conversation as an assistant message.
- `/explain <focus text>` optionally focuses the explanation (for example
  `/explain focus on the locking strategy`).
- `/rename` now invokes the rename assistant in the chat: it renders ranked name
  suggestions with confidence and reasoning as an assistant message, plus a pointer to
  "Suggest Name with AI" for applying a suggestion.
- `/rename <hint>` optionally passes a naming hint to the suggester (for example
  `/rename prefer a Header prefix`).
- Both commands explain what they need when no symbol is selected, and `/rename` friendly
  declines symbols that do not look obfuscated, matching the context-menu behavior.

### Improvements
- Slash commands are no longer rewritten into plain-English chat prompts; the unused
  command-expansion fallback was removed.
- The `/audit` and `/summary` command handlers share one command runner with the new
  commands (busy state, cancellation, and error handling behave identically).

### Technical details
- `IAIChatFeatureCommands` gained `RunExplainAsync` (streaming) and `RunRenameAsync`
  (one-shot) implemented by `AIChatFeatureCommands`, which resolves the selected entity,
  re-resolves it in a fresh decompiler type system, and calls `AIExplanationService` /
  `RenameSuggester`.
- The entity-to-decompiler plumbing (`AIEntityDecompilation`) is shared with the
  AI Output pane instead of being duplicated per feature.
- `AIExplanationService.ExplainContextStreamingAsync` accepts an optional focus text and
  `RenameSuggester.SuggestAsync` an optional user naming hint.

## AI Prompt Externalization

### New features
- All AI system prompts now live in externalized `.prompt` files under
  `ICSharpCode.ILSpyX/AI/prompts/` and can be edited without recompiling; changes take
  effect on the next start of ILSpy.
- Model-specific prompt variations (for example `explanation.opus.prompt`) are selected
  at runtime through exact, case-sensitive `applies_to_models` matching, with
  lexicographic file-name precedence.
- When the prompt directory is missing or a file fails to parse, features fall back to
  build-time embedded prompts, so the AI features keep working.

### Improvements
- The `BuildTools/PromptEmbedder` generator refreshes the embedded fallback prompts on
  every ILSpyX build and offers a `--check` mode for CI staleness validation.
- The `.prompt` file format, variation selection rules, and per-prompt documentation are
  described in `ICSharpCode.ILSpyX/AI/prompts/README.md`.

### Technical details
- A new `AIPromptProvider` singleton loads and caches prompts per prompt ID and model ID
  and serves all eight consumers: explanation, rename, chat, security, security_audit,
  generate_docs, search, and assembly_summary.

## AI Panes - Markdown Syntax Highlighting

### New features
- AI responses now show markdown syntax highlighting (headings, emphasis, lists, and fenced
  code blocks) in the AI Output, AI Chat, and Explain-with-AI surfaces.
- Fenced code blocks in AI responses can be opened in a new decompiler tab with full syntax
  highlighting: select "Open in Decompiler" from the editor context menu or press
  Ctrl+Shift+O while the caret is inside a block.
- "Copy Code Block" (or Ctrl+Shift+C) copies just the code, without the surrounding fence
  markers.

### Improvements
- Faster streaming in the AI Output and Chat panes (buffered document updates instead of a
  full replace per token).
- The AI panes respect the active Light/Dark theme and the user font settings, matching the
  main decompiler text view.
- Text selection and copy now work in every AI pane.

### Technical details
- Replaced plain TextBox controls with an AvaloniaEdit-based editor that reuses the existing
  markdown highlighting definition.
- Added the Markdig parser to extract fenced code blocks for the Open/Copy actions.
- The shared editor ships a right-click context menu and fits how the theme and font systems
  already surface in the decompiler view.

