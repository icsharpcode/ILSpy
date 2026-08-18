# Changelog

This project does not keep an exhaustive release log; this file only records user-facing
highlights that are worth noting between releases.

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

