# Markdown AI Panes - Design Rationale

**For:** Understanding the "why" behind the implementation approach  
**Created:** 2026-08-18  
**Related:** `markdown-ai-panes.md` (main plan)

---

## The Question

ILSpy's AI panes show markdown responses from AI providers, but currently render them as plain text. Users see raw markdown syntax (`## Heading`, `**bold**`, ` ```csharp`) instead of formatted content.

How should we render markdown in AI responses?

---

## Options Considered

### Option 1: Markdown.Avalonia (Full Rendering)

**What it is:** Third-party library that renders markdown as formatted HTML-like output (bold text, styled headings, formatted tables, clickable links).

**Pros:**
- ✅ Beautiful output (looks like a document)
- ✅ Users see formatted text, not source
- ✅ Tables render as tables
- ✅ Links could be clickable

**Cons:**
- ❌ New dependency (adds ~500KB to build)
- ❌ Not designed for streaming (rerenders entire document on each chunk)
- ❌ Unknown theme integration (may need custom CSS for Light/Dark)
- ❌ Unknown font integration (may not respect `DisplaySettings`)
- ❌ Per-block scrolling (wrap prose, scroll code) may not be supported
- ❌ Extracting code blocks harder (no access to fence boundaries)
- ❌ More complex implementation (unfamiliar API)

**Risk level:** HIGH - Unknown integration points, potential performance issues, requires investigation spike.

---

### Option 2: AvaloniaEdit with Markdown Syntax Highlighting (CHOSEN)

**What it is:** Use AvaloniaEdit (already in ILSpy) with the "MarkDown" syntax highlighting definition (already registered in `HighlightingService.cs:44`).

**Pros:**
- ✅ Zero new dependencies (AvaloniaEdit already referenced)
- ✅ Fast streaming (just append to `TextDocument`)
- ✅ Proven theme integration (reuse `ThemeAwareHighlightingColorizer` pattern)
- ✅ Proven font integration (reuse `DecompilerTextEditor` pattern)
- ✅ Free text selection, copy, search
- ✅ Simple implementation (copy existing patterns)
- ✅ Code fence extraction trivial (parse the source markdown)
- ✅ Enables "Open in Decompiler" feature (use `ShowTextInNewTab` API)

**Cons:**
- ❌ Users see colored markdown source (not rendered output)
- ❌ Tables remain ASCII art (but colored)
- ❌ Links are colored but not clickable
- ❌ Bold/italic shown as `**text**` and `_text_` (but colored)

**Risk level:** LOW - All integration points proven, performance known, implementation straightforward.

---

### Option 3: Custom Renderer with AvaloniaEdit for Code Blocks

**What it is:** Hybrid approach - render prose with Markdown.Avalonia, embed AvaloniaEdit instances for code blocks.

**Pros:**
- ✅ Best of both worlds (formatted prose + syntax-highlighted code)

**Cons:**
- ❌ Complex implementation (two rendering systems)
- ❌ Unknown performance (embedded editors may be heavy)
- ❌ Streaming becomes very complex
- ❌ Still requires Markdown.Avalonia (adds dependency)

**Risk level:** VERY HIGH - Novel approach, many unknowns, significant engineering effort.

---

## Decision: Option 2 (AvaloniaEdit + Syntax Highlighting)

**Reasoning:**

1. **Pragmatism over perfection**
   - Syntax-highlighted markdown is **10x better than plain black text**
   - Users can distinguish headings, code blocks, emphasis at a glance
   - Going from 0% to 80% with low risk beats going from 0% to 100% with high risk

2. **Leverage existing infrastructure**
   - ILSpy already uses AvaloniaEdit everywhere (proven, battle-tested)
   - Theme integration already works (`ThemeAwareHighlightingColorizer`)
   - Font integration already works (`DecompilerTextEditor` pattern)
   - Markdown highlighting already registered (just needs to be used)

3. **Fast streaming is critical**
   - AI responses stream token-by-token
   - AvaloniaEdit handles this naturally (`Document.Insert()`)
   - Markdown.Avalonia would require rerendering entire document on each chunk

4. **Enables the killer feature**
   - "Open in Decompiler" for C# code blocks is ILSpy's unique value-add
   - Requires parsing markdown source to extract fences
   - With syntax highlighting, the source IS the display - trivial to parse
   - With rendering, we'd need to keep source separate and sync state

5. **Incremental path forward**
   - Phase 1-3: Basic highlighting (2-4 hours)
   - Test with real users
   - If feedback demands full rendering, add Option 3 later
   - But syntax highlighting alone may be "good enough"

---

## Trade-offs Accepted

### Trade-off 1: ASCII Tables

**What we give up:** Tables render as ASCII art:
```
| Column 1 | Column 2 |
|----------|----------|
| Value    | Value    |
```

**Why it's acceptable:**
- AI responses rarely contain complex tables
- When they do, ASCII tables are still readable
- Syntax highlighting makes rows/columns distinguishable
- Users can copy/paste into a markdown viewer if needed

### Trade-off 2: Non-Clickable Links

**What we give up:** Links show as `[text](url)` instead of clickable.

**Why it's acceptable:**
- AI responses rarely contain many links
- Users can copy/paste URLs
- Could be added later (intercept clicks on URL patterns)
- Not the primary use case (AI explains code, not web browsing)

### Trade-off 3: No Bold/Italic Rendering

**What we give up:** Bold shows as `**text**`, italic as `_text_`.

**Why it's acceptable:**
- Syntax highlighting makes emphasis markers visually distinct
- Users understand markdown conventions
- Emphasis is less critical than code block highlighting
- Still more readable than plain black text

---

## Why Not "Do Both"?

**Question:** Why not add both AvaloniaEdit (for streaming) AND Markdown.Avalonia (for final render)?

**Answer:**
1. **Complexity:** Two controls, two code paths, state synchronization
2. **UX confusion:** When does it switch? Does user control it?
3. **Maintenance:** Two systems to keep themed, bug-free, tested
4. **YAGNI:** Build the simple thing first, add complexity only if users demand it

**Better path:**
1. Ship syntax highlighting (low risk, fast)
2. Gather user feedback
3. If users strongly want rendering, add a toggle later
4. Most likely outcome: users find syntax highlighting sufficient

---

## The "Open in Decompiler" Feature

**Why this is the killer feature:**

ILSpy is a .NET decompiler. When AI generates C# code examples, users want to:
1. See the code with full syntax highlighting
2. Copy it
3. **Open it in a decompiler tab** to explore it like any other code

**How syntax highlighting enables it:**

```markdown
Here's how to implement that:

```csharp
public class Example {
    public void Method() { }
}
```

This uses the factory pattern.
```

1. Parse the markdown source (trivial with `Markdig`)
2. Extract the C# fence content
3. Create `AvaloniaEditTextOutput` with the code
4. Call `DockWorkspace.ShowTextInNewTab()` (already exists!)
5. User gets a frozen tab with C# syntax highlighting

**With full markdown rendering:**
- Would need to keep source separately
- Would need to map click coordinates to fence boundaries
- Would need to reconstruct code from rendered output
- Much more complex

---

## Performance Considerations

### Streaming Performance

**AvaloniaEdit approach:**
```csharp
foreach (string chunk in stream) {
    document.Insert(document.TextLength, chunk); // O(1) append
}
```
- Fast: Just appending to a rope data structure
- No flicker: Document reuses existing styled spans
- Scalable: Handles 10k+ line responses

**Markdown.Avalonia approach:**
```csharp
foreach (string chunk in stream) {
    fullMarkdown += chunk;
    markdownControl.Markdown = fullMarkdown; // Full re-parse + re-render
}
```
- Slow: Parses and renders entire document each chunk
- Flicker: Entire control rebuilds on each update
- Degrades: O(n²) behavior as document grows

### Memory Considerations

**AvaloniaEdit:**
- One `TextDocument` per pane
- Syntax highlighting is lazy (only visible lines)
- Proven to scale to large files

**Markdown.Avalonia:**
- Unknown memory profile
- May create many WPF/Avalonia controls for formatted output
- Untested at ILSpy scale

---

## Future Evolution Path

If user feedback demands full rendering, the evolution path is:

**Phase 1 (Shipped):**
- AvaloniaEdit with markdown syntax highlighting
- "Open in Decompiler" for code blocks
- All AI panes updated

**Phase 2 (If requested):**
- Add toggle: "Render markdown" checkbox in Display Settings
- When enabled, show Markdown.Avalonia
- When disabled, show AvaloniaEdit (current behavior)
- Users choose based on preference

**Phase 3 (If still needed):**
- Hybrid rendering: Markdown.Avalonia for prose, embedded AvaloniaEdit for code
- Complex but gives best of both worlds
- Only do this if Phase 2 shows strong demand

**Most likely outcome:** Phase 1 ships, users are happy, Phase 2/3 never needed.

---

## Validation from Codebase

**Evidence that this approach is correct:**

1. **Markdown highlighting already registered:**
   ```csharp
   // ILSpy/TextView/HighlightingService.cs:44
   HighlightingManager.Instance.RegisterHighlighting(
       new HighlightingDefinition("MarkDown", ...) // <-- ALREADY EXISTS
   );
   ```

2. **AvaloniaEdit already themed:**
   ```csharp
   // ILSpy/TextView/DecompilerTextEditor.cs
   // Subscribes to ThemeManager.Current.ThemeChanged
   // Applies DisplaySettings.SelectedFont and SelectedFontSize
   // Binds Background to ILSpy.EditorBackground resource
   ```

3. **ShowTextInNewTab already exists:**
   ```csharp
   // ILSpy/Docking/DockWorkspace.cs:1138
   public ContentTabPage ShowTextInNewTab(string title, AvaloniaEditTextOutput output)
   {
       // Creates frozen tab, applies syntax highlighting, opens it
   }
   ```

**Conclusion:** All pieces are in place. We're just connecting them.

---

## Comparison to Other Decompilers

### dnSpy (predecessor to ILSpy)
- No AI features (project discontinued)
- Used AvaloniaEdit for all text display

### JetBrains dotPeek
- No AI features (yet)
- Uses custom editor (ReSharper platform)

### ILSpy (current)
- Has AI features (explain, rename, chat)
- Uses plain TextBox (no highlighting)
- **After this plan:** Uses AvaloniaEdit with markdown highlighting
- **Unique feature:** Open AI-generated code in decompiler tabs

**Competitive advantage:** Other decompilers don't have AI integration. ILSpy does, and this plan makes it first-class.

---

## Risk Analysis

### Low Risk Items (Already Proven)
- ✅ AvaloniaEdit integration (used everywhere in ILSpy)
- ✅ Theme integration (ThemeManager + ThemeAwareHighlightingColorizer)
- ✅ Font integration (DecompilerTextEditor pattern)
- ✅ Streaming performance (Document.Insert is fast)
- ✅ Code fence extraction (Markdig is proven)
- ✅ Opening new tabs (ShowTextInNewTab is proven)

### Medium Risk Items (Need Testing)
- ⚠️ Markdown highlighting readability (might need color adjustments)
- ⚠️ Streaming flicker (might need buffering)
- ⚠️ Memory leaks (event handler cleanup is critical)

### High Risk Items (None)
- No high-risk items in this approach

**Overall Risk Level:** LOW

---

## User Expectations

**What users expect from "markdown support":**
- See structure (headings, lists, emphasis) visually
- Distinguish code from prose
- Copy/paste text easily
- Fast, responsive UI

**What users DON'T necessarily expect:**
- Pixel-perfect rendering like GitHub
- Clickable links (AI responses are explanatory, not navigation)
- Formatted tables (rarely used in AI responses)

**This approach delivers what users actually need.**

---

## Summary

**Why AvaloniaEdit + Syntax Highlighting?**
1. ✅ Low risk (proven technology)
2. ✅ Fast implementation (12-16 hours)
3. ✅ Zero new dependencies
4. ✅ Great streaming performance
5. ✅ Enables "Open in Decompiler" feature
6. ✅ Incremental path forward (can add rendering later if needed)
7. ✅ 80% solution with 20% effort

**The pragmatic choice that fits ILSpy's architecture, leverages existing infrastructure, and delivers immediate value to users.**

---

**END OF RATIONALE**
