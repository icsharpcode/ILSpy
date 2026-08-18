# Markdown AI Panes - Quick Start Guide

**For:** Less capable models implementing the plan  
**Created:** 2026-08-18  
**Main Plan:** `markdown-ai-panes.md`

---

## Before You Start

**Read these files first:**
1. `/Volumes/OSCOO1TB/repos/ILSpy/CLAUDE.md` - Repo conventions
2. `/Volumes/OSCOO1TB/repos/ILSpy/doc/plans/enhancements/markdown-ai-panes.md` - Full plan

**Key conventions:**
- Use `restore.ps1` and `build.ps1` (not raw `dotnet` commands)
- Always run the pre-commit hook (formats code automatically)
- Commit after each work package (use provided commit messages)
- Copyright headers: `// Copyright (c) 2026 Masroor` for new files

---

## Implementation Order

**Do phases in sequence. Do not skip ahead.**

### Phase 1: Foundation (START HERE)
1. Create `ILSpy/AI/Controls/MarkdownTextEditor.cs` and `.axaml`
2. Test it works in isolation (create test window if needed)
3. Replace `StreamingTextControl.axaml` and `.axaml.cs`
4. Update `AIOutputPaneModel.cs` streaming logic
5. Test AI Output pane works with markdown highlighting

**Checkpoint:** AI Output pane shows syntax-highlighted markdown. Commit 3-4 times during Phase 1.

### Phase 2: Chat Pane
1. Create `ILSpy/AI/Controls/ChatMessageControl.axaml` and `.axaml.cs`
2. Update `AIChatPane.axaml` to use it
3. Test chat pane works

**Checkpoint:** AI Chat pane shows syntax-highlighted messages. Commit 2 times.

### Phase 3: Dialog
1. Update `ExplainDialog.axaml`
2. Test dialog works

**Checkpoint:** Explain dialog shows syntax-highlighted markdown. Commit once.

### Phase 4: Code Fence Parsing
1. Add Markdig package to `Directory.Packages.props`
2. Add PackageReference to `ICSharpCode.ILSpyX.csproj`
3. Run `./updatedeps.ps1`
4. Create `ICSharpCode.ILSpyX/AI/MarkdownCodeFenceExtractor.cs`
5. Test the extractor works

**Checkpoint:** Can extract C# code fences from markdown. Commit 2 times.

### Phase 5: Open in Decompiler
1. Add context menu to `MarkdownTextEditor`
2. Wire context menu to `DockWorkspace.ShowTextInNewTab()`
3. Test opening code fences in new tabs

**Checkpoint:** Right-click C# fence → "Open in Decompiler" works. Commit 2-3 times.

### Phase 6: Polish
1. Update CHANGELOG or release notes
2. Add keyboard shortcuts (optional)
3. Add status feedback (optional)

**Checkpoint:** Feature is polished. Commit 1-2 times.

### Phase 7: Testing
1. Run all tests in the test matrix
2. Fix any bugs found
3. Final commit

**Checkpoint:** All tests pass. Ship it.

---

## Critical Files Reference

**Files you'll create:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`
- `ILSpy/AI/Controls/MarkdownTextEditor.axaml`
- `ILSpy/AI/Controls/ChatMessageControl.cs`
- `ILSpy/AI/Controls/ChatMessageControl.axaml`
- `ICSharpCode.ILSpyX/AI/MarkdownCodeFenceExtractor.cs`

**Files you'll modify:**
- `ILSpy/AI/StreamingTextControl.axaml`
- `ILSpy/AI/StreamingTextControl.axaml.cs`
- `ILSpy/AI/AIOutputPaneModel.cs`
- `ILSpy/AI/AIChatPane.axaml`
- `ILSpy/AI/ExplainDialog.axaml`
- `Directory.Packages.props`
- `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`

**Files to reference (DON'T MODIFY):**
- `ILSpy/TextView/DecompilerTextEditor.cs` - Pattern to copy for theme integration
- `ILSpy/TextView/HighlightingService.cs` - How to get "MarkDown" highlighting
- `ILSpy/Themes/ThemeManager.cs` - Theme system
- `ILSpy/Options/DisplaySettings.cs` - Font settings
- `ILSpy/Docking/DockWorkspace.cs` - How to open new tabs

---

## Common Mistakes to Avoid

### 1. Wrong Font Configuration
**WRONG:**
```csharp
FontFamily = new FontFamily("Consolas"); // Hardcoded
```

**RIGHT:**
```csharp
var displaySettings = TryGetDisplaySettings();
if (displaySettings != null && !string.IsNullOrEmpty(displaySettings.SelectedFont))
    FontFamily = new FontFamily(displaySettings.SelectedFont);
```

### 2. Memory Leaks
**WRONG:**
```csharp
public MarkdownTextEditor()
{
    ThemeManager.Current.ThemeChanged += OnThemeChanged; // Subscribed in constructor
}
```

**RIGHT:**
```csharp
protected override void OnAttachedToVisualTree(...)
{
    base.OnAttachedToVisualTree(e);
    ThemeManager.Current.ThemeChanged += OnThemeChanged; // Subscribe when attached
}

protected override void OnDetachedFromVisualTree(...)
{
    ThemeManager.Current.ThemeChanged -= OnThemeChanged; // UNSUBSCRIBE when detached
    base.OnDetachedFromVisualTree(e);
}
```

### 3. Wrong Build Commands
**WRONG:**
```bash
dotnet restore
dotnet build
```

**RIGHT:**
```bash
./restore.ps1
./build.ps1
```

### 4. Skipping Pre-Commit Hook
**WRONG:**
```bash
git commit --no-verify -m "..."
```

**RIGHT:**
```bash
git commit -m "..."  # Let the hook format the code
```

### 5. Wrong Syntax Highlighting Name
**WRONG:**
```csharp
SyntaxHighlighting = HighlightingService.GetDefinition("Markdown"); // Wrong case
```

**RIGHT:**
```csharp
SyntaxHighlighting = HighlightingService.GetDefinition("MarkDown"); // Exactly as registered
```

---

## Testing Checklist (Run After Each Phase)

### After Phase 1:
- [ ] Open ILSpy
- [ ] Right-click a method → "Explain with AI"
- [ ] Verify AI Output pane shows colored markdown (not plain black text)
- [ ] Verify headings are colored differently than body text
- [ ] Verify code blocks are colored
- [ ] Switch theme (Options → Display Settings → Theme) and verify colors update

### After Phase 2:
- [ ] Open AI Chat pane
- [ ] Send a message
- [ ] Verify response shows colored markdown
- [ ] Verify streaming works (text appears incrementally)
- [ ] Verify old messages still show correctly

### After Phase 3:
- [ ] Right-click a method → "Explain with AI"
- [ ] Verify dialog shows colored markdown

### After Phase 4:
- [x] Write a unit test with markdown containing C# fences (see `ICSharpCode.ILSpyX.Tests/AI/MarkdownCodeFenceExtractorTests.cs`)
- [x] Verify `MarkdownCodeFenceExtractor.ExtractCSharpFences()` returns correct fences
- [x] Verify `fence.Code` contains code without backticks

### After Phase 5:
- [ ] Right-click inside a C# code fence in AI Output pane
- [ ] Select "Open in Decompiler"
- [ ] Verify new tab opens with C# syntax highlighting
- [ ] Verify code is correct

---

## Debugging Tips

### "Markdown highlighting not appearing"
1. Check `HighlightingService.GetDefinition("MarkDown")` returns non-null
2. Verify `SyntaxHighlighting` property is set in constructor
3. Check theme integration - try both Light and Dark themes

### "Font not updating"
1. Verify `OnAttachedToVisualTree` subscribes to `displaySettings.PropertyChanged`
2. Verify `OnDetachedFromVisualTree` unsubscribes
3. Check `ApplyFontSettings()` is called on property changes

### "Memory leak / pane won't close"
1. Check all event subscriptions have matching unsubscribes in `OnDetachedFromVisualTree`
2. Verify `PropertyChanged` subscriptions are cleaned up
3. Use Visual Studio memory profiler to find leaks

### "Context menu doesn't appear"
1. Verify `ContextMenu` property is set in constructor
2. Check right-click is not consumed by parent control
3. Test with simple "Copy" menu item first

### "Open in Decompiler does nothing"
1. Verify `DockWorkspace` is found via `AppComposition.TryGetExport<DockWorkspace>()`
2. Check `ShowTextInNewTab()` is called (add breakpoint)
3. Verify `AvaloniaEditTextOutput.SyntaxExtensionOverride` is set to `".cs"`

---

## Build and Test Commands

```bash
# Clean build from scratch
./clean.ps1
./restore.ps1
./build.ps1

# Run tests
dotnet test --solution ILSpy.sln --report-trx

# Format code (run before committing)
dotnet format ILSpy.sln

# Or just let the pre-commit hook do it:
git add .
git commit -m "Your message"  # Hook runs dotnet format automatically
```

---

## When You Get Stuck

**If implementation is unclear:**
1. Read the detailed phase in `markdown-ai-panes.md`
2. Look at the reference files listed above
3. Search for similar patterns in the codebase

**If tests fail:**
1. Check the "Common Mistakes" section above
2. Run the debugging tips
3. Read error messages carefully (they're usually accurate)

**If you need to roll back:**
1. Use git to revert specific commits
2. Each phase is independent (can revert Phase 5 without breaking Phase 1-4)

---

## Success Criteria

You're done when:
- [ ] All 7 phases are complete
- [ ] All tests in the test matrix pass
- [ ] No memory leaks detected
- [ ] Theme switching works correctly
- [ ] Font changes apply immediately
- [ ] "Open in Decompiler" works for C# fences
- [ ] Existing functionality (Copy, Clear, Cancel) still works

---

**Good luck! Start with Phase 1 and work sequentially.**
