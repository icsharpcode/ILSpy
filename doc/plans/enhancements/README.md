# Markdown AI Panes - Implementation Plan Index

**Project:** ILSpy AI Panes Markdown Highlighting  
**Created:** 2026-08-18  
**Status:** Ready for Implementation

---

## Document Suite

This enhancement is documented across three complementary files:

### 1. **Main Implementation Plan** (`markdown-ai-panes.md`)
- **Audience:** Implementer (human or AI)
- **Size:** 38KB, ~1,350 lines
- **Purpose:** Step-by-step implementation guide with code samples
- **Contents:**
  - 7 phases with 15+ work packages
  - Detailed code snippets for every file
  - Testing checklists per phase
  - Commit messages for each work package
  - Estimated time per phase (12-16 hours total)
  - Complete test matrix
  - Rollback plan

**Start here** if you're implementing the feature.

---

### 2. **Quick Start Guide** (`markdown-ai-panes-quick-start.md`)
- **Audience:** Less capable models or junior developers
- **Size:** 8.2KB, ~300 lines
- **Purpose:** Simplified checklist with common pitfalls
- **Contents:**
  - Phase-by-phase checklist (do this, then this, then this)
  - Critical files reference (create vs modify vs reference)
  - Common mistakes to avoid with correct patterns
  - Testing checklist per phase
  - Debugging tips for common issues
  - Build and test commands

**Use this** if the main plan feels overwhelming or you're new to ILSpy.

---

### 3. **Design Rationale** (`markdown-ai-panes-rationale.md`)
- **Audience:** Reviewers, maintainers, future developers
- **Size:** 12KB, ~380 lines
- **Purpose:** Explains the "why" behind design decisions
- **Contents:**
  - 3 options evaluated (Markdown.Avalonia, AvaloniaEdit, Hybrid)
  - Why AvaloniaEdit was chosen (low risk, proven integration)
  - Trade-offs accepted (ASCII tables, non-clickable links)
  - Performance analysis (streaming, memory)
  - Risk analysis (all low-risk items)
  - Future evolution path (if users request full rendering)
  - Competitive advantage (unique to ILSpy)

**Read this** to understand why decisions were made.

---

## Implementation Workflow

```
1. READ: markdown-ai-panes-rationale.md
   └─> Understand the approach and trade-offs

2. READ: CLAUDE.md (repo conventions)
   └─> Understand commit style, build commands, file headers

3. IMPLEMENT: Follow markdown-ai-panes.md
   ├─> Phase 1: Foundation (MarkdownTextEditor)
   ├─> Phase 2: AIChatPane
   ├─> Phase 3: ExplainDialog
   ├─> Phase 4: Code Fence Parsing
   ├─> Phase 5: Open in Decompiler
   ├─> Phase 6: Polish
   └─> Phase 7: Testing

4. REFERENCE: markdown-ai-panes-quick-start.md
   └─> When stuck, check common mistakes and debugging tips

5. COMMIT: After each work package
   └─> Use provided commit messages (12-15 commits total)
```

---

## Key Decision: AvaloniaEdit with Markdown Syntax Highlighting

**Approach:** Use AvaloniaEdit (already in ILSpy) with the "MarkDown" syntax highlighting definition (already registered).

**Why:**
- ✅ Zero new dependencies
- ✅ Fast streaming (just append to document)
- ✅ Proven theme integration (reuse existing patterns)
- ✅ Proven font integration (reuse existing patterns)
- ✅ Enables "Open in Decompiler" feature

**Trade-offs:**
- ❌ Users see colored markdown source (not rendered HTML-like output)
- ❌ Tables remain ASCII art (but colored)
- ❌ Links are colored but not clickable

**Result:** 80% solution with 20% effort, low risk, fast implementation.

---

## Technical Approach

### Phase 1: Foundation
Create `MarkdownTextEditor` control:
- Inherits from `AvaloniaEdit.TextEditor`
- Applies "MarkDown" syntax highlighting
- Integrates with `ThemeManager` (Light/Dark themes)
- Integrates with `DisplaySettings` (font family, size)
- Read-only, word-wrapped, supports streaming

### Phase 2-3: Integration
Replace plain TextBox/TextBlock in:
- `AIOutputPane` (via `StreamingTextControl`)
- `AIChatPane` (via `ChatMessageControl`)
- `ExplainDialog` (direct replacement)

### Phase 4-5: Code Fence Features
Add Markdig package:
- Parse markdown to extract code fences
- Detect C# fences (`csharp`, `cs`, `c#`)
- Context menu: "Open in Decompiler"
- Use `DockWorkspace.ShowTextInNewTab()` API
- Open code with C# syntax highlighting in frozen tab

---

## Success Criteria

Implementation is complete when:

1. ✅ All AI panes display markdown with syntax highlighting
2. ✅ Users can select and copy text from AI responses
3. ✅ Streaming performance is smooth (no excessive flicker)
4. ✅ Theme switching works correctly (Light ↔ Dark)
5. ✅ Font settings are respected in all AI panes
6. ✅ "Open in Decompiler" works for C# code fences
7. ✅ No memory leaks when opening/closing panes
8. ✅ Existing functionality (Copy, Clear, Cancel, Export) still works
9. ✅ No new dependencies added beyond Markdig
10. ✅ All tests pass without regressions

---

## Estimated Effort

- **Phase 1:** 2-3 hours (Foundation)
- **Phase 2:** 1-2 hours (AIChatPane)
- **Phase 3:** 0.5 hours (ExplainDialog)
- **Phase 4:** 2 hours (Code Fence Parsing)
- **Phase 5:** 3-4 hours (Open in Decompiler)
- **Phase 6:** 1-2 hours (Polish)
- **Phase 7:** 2-3 hours (Testing)

**Total:** 12-16 hours across 7 phases, 12-15 commits

---

## Files Created

**New Controls:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs` + `.axaml`
- `ILSpy/AI/Controls/ChatMessageControl.cs` + `.axaml`

**New Utilities:**
- `ICSharpCode.ILSpyX/AI/MarkdownCodeFenceExtractor.cs`

**Total:** 5 new files

---

## Files Modified

**AI Panes:**
- `ILSpy/AI/StreamingTextControl.axaml` + `.axaml.cs`
- `ILSpy/AI/AIOutputPaneModel.cs`
- `ILSpy/AI/AIChatPane.axaml`
- `ILSpy/AI/ExplainDialog.axaml`

**Dependencies:**
- `Directory.Packages.props`
- `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`

**Total:** 6 modified files

---

## Dependencies Added

**Only one:** `Markdig` (markdown parser)
- Used for extracting code fences from markdown
- Mature, well-maintained library
- ~200KB addition to build

**Not added:**
- Markdown.Avalonia (not needed for syntax highlighting approach)
- Any other markdown renderers

---

## Testing Strategy

**Unit Tests:**
- `MarkdownCodeFenceExtractor` - fence extraction logic

**Integration Tests:**
- MarkdownTextEditor theme integration
- MarkdownTextEditor font integration
- Streaming behavior in AIOutputPane
- Context menu in all three panes

**Manual Tests:**
- Test matrix covering all panes × all features
- Theme switching (Light ↔ Dark)
- Font changes
- Streaming performance
- Memory leak detection (open/close panes repeatedly)

---

## Risk Mitigation

**Low Risk Items (Already Proven):**
- AvaloniaEdit integration ✅
- Theme integration ✅
- Font integration ✅
- Streaming performance ✅
- Code fence extraction ✅
- Opening new tabs ✅

**Medium Risk Items:**
- Markdown highlighting readability → Test in both themes, adjust colors if needed
- Streaming flicker → Buffer updates (update every N chunks)
- Memory leaks → Follow OnAttached/OnDetached pattern strictly

**High Risk Items:** None

---

## Rollback Plan

**Partial Rollback:**
- Phase 5 (Code Fence Actions) can be reverted independently
- Phase 2 (AIChatPane) can be reverted independently
- Phase 3 (ExplainDialog) can be reverted independently

**Full Rollback:**
- Revert `StreamingTextControl` changes
- Remove `MarkdownTextEditor` and `ChatMessageControl`
- Users get plain text but functionality restored

**Fallback Strategy:**
- Add `DisplaySettings.UseMarkdownHighlighting` toggle
- Default = true (new behavior)
- Users can opt-out if issues arise

---

## Future Enhancements

**Not in current plan (decide after user feedback):**

**Priority: Medium**
- Clickable links (intercept URL pattern clicks)
- "Copy all code blocks" bulk action
- Custom markdown color scheme

**Priority: Low**
- Full markdown rendering with Markdown.Avalonia (as toggle)
- Rendered tables
- Collapsible code fences
- Export individual code fences to files

**Priority: High** (consider for next iteration if feedback demands)
- Full markdown rendering alongside syntax highlighting (toggle view)
- Hybrid approach: render prose, syntax-highlight code

---

## Related Documentation

**ILSpy Docs:**
- `/Volumes/OSCOO1TB/repos/ILSpy/CLAUDE.md` - Repo conventions
- `/Volumes/OSCOO1TB/repos/ILSpy/ICSharpCode.Decompiler.Tests/CLAUDE.md` - Test suite guide

**Reference Files:**
- `ILSpy/TextView/DecompilerTextEditor.cs` - Theme integration pattern
- `ILSpy/TextView/HighlightingService.cs` - Syntax highlighting registration
- `ILSpy/Themes/ThemeManager.cs` - Theme system
- `ILSpy/Docking/DockWorkspace.cs` - Tab management API

---

## Questions?

**For implementation questions:**
→ Read the detailed phase in `markdown-ai-panes.md`

**For "why" questions:**
→ Read `markdown-ai-panes-rationale.md`

**For quick reference:**
→ Check `markdown-ai-panes-quick-start.md`

**If still stuck:**
→ Look at reference files listed above
→ Search for similar patterns in the codebase

---

## Ready to Start?

1. Read `markdown-ai-panes-rationale.md` (understand the approach)
2. Read `/Volumes/OSCOO1TB/repos/ILSpy/CLAUDE.md` (understand repo conventions)
3. Open `markdown-ai-panes.md` and start Phase 1
4. Keep `markdown-ai-panes-quick-start.md` handy for reference

**Good luck!**

---

**Last Updated:** 2026-08-18  
**Status:** Ready for Implementation  
**Estimated Effort:** 12-16 hours  
**Risk Level:** LOW
