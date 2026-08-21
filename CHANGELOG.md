# Changelog

This project does not keep an exhaustive release log; this file only records user-facing
highlights that are worth noting between releases.

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

