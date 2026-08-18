# Markdown Rendering in AI Panes - Implementation Plan

**Status:** Ready for Implementation  
**Created:** 2026-08-18  
**Last Updated:** 2026-08-18  
**Approach:** AvaloniaEdit with markdown syntax highlighting

---

## Executive Summary

Replace plain TextBox controls in AI panes with AvaloniaEdit TextEditor instances that provide markdown syntax highlighting. This gives users colored, readable markdown (headings, code blocks, emphasis) without requiring a full markdown renderer, leveraging existing ILSpy infrastructure for theming, fonts, and streaming.

**Key Benefits:**
- ✅ No new dependencies (AvaloniaEdit already referenced)
- ✅ Fast streaming (append to document, no re-render)
- ✅ Theme integration (reuse existing `ThemeAwareHighlightingColorizer`)
- ✅ Free selection, copy, search
- ✅ Foundation for "Open in decompiler" feature

**Trade-offs Accepted:**
- Users see syntax-highlighted markdown source (not rendered HTML-like output)
- Tables remain ASCII art (but colored)
- Links are colored but not clickable (acceptable for MVP)

---

## Architecture Overview

### Current State

Three AI panes show plain text in TextBox controls:

1. **AIOutputPane** (`ILSpy/AI/AIOutputPane.axaml`)
   - Uses `StreamingTextControl` (wraps plain TextBox)
   - Shows single AI response (explain symbol, rename suggestions)
   - Streaming via `StringBuilder` + `Response` property updates

2. **AIChatPane** (`ILSpy/AI/AIChatPane.axaml`)
   - Uses `ListBox` with `TextBlock` items
   - Shows conversation history (role + content per message)
   - Streaming updates individual message `Content` property

3. **ExplainDialog** (`ILSpy/AI/ExplainDialog.axaml`)
   - Direct TextBox in modal dialog
   - Shows AI explanation of selected symbol

### Target State

All three panes use AvaloniaEdit with markdown syntax highlighting:

1. **MarkdownTextEditor** - New reusable control
   - Inherits theme integration from `DecompilerTextEditor` pattern
   - Applies "MarkDown" syntax highlighting (already registered)
   - Read-only, word-wrapped, supports streaming via `TextDocument.Insert()`

2. **StreamingMarkdownControl** - Drop-in replacement for `StreamingTextControl`
   - Wraps `MarkdownTextEditor`
   - Exposes `AppendText(string)` method for streaming
   - Maintains same public API for minimal view model changes

3. **ChatMessageControl** - New control for chat messages
   - Displays role + markdown-highlighted content
   - Replaces `TextBlock` in `AIChatPane.axaml` ListBox template

### Component Relationships

```
AIOutputPaneModel
  └─> AIOutputPane.axaml
        └─> StreamingMarkdownControl
              └─> MarkdownTextEditor (AvaloniaEdit)

AIChatPaneModel
  └─> AIChatPane.axaml
        └─> ListBox[ChatMessage]
              └─> ChatMessageControl (per item)
                    └─> MarkdownTextEditor (AvaloniaEdit)

ExplainDialogViewModel
  └─> ExplainDialog.axaml
        └─> MarkdownTextEditor (direct)
```

---

## Phase 1: Foundation - MarkdownTextEditor Control

**Goal:** Create the core reusable control that all panes will use.

**Estimated Time:** 2-3 hours  
**Commit Frequency:** After each file creation/test pass

### Work Package 1.1: Create MarkdownTextEditor Control

**Files to Create:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`
- `ILSpy/AI/Controls/MarkdownTextEditor.axaml`

**Implementation:**

#### File: `ILSpy/AI/Controls/MarkdownTextEditor.cs`

```csharp
// Copyright (c) 2026 Masroor
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.AI.Controls
{
	/// <summary>
	/// Read-only AvaloniaEdit TextEditor configured for markdown syntax highlighting.
	/// Integrates with ILSpy's theme system and font settings.
	/// </summary>
	public class MarkdownTextEditor : TextEditor
	{
		DisplaySettings? displaySettings;

		public MarkdownTextEditor()
		{
			IsReadOnly = true;
			WordWrap = true;
			ShowLineNumbers = false;
			
			// Apply markdown syntax highlighting
			var highlighting = HighlightingService.GetDefinition("MarkDown");
			if (highlighting != null)
				SyntaxHighlighting = highlighting;
			
			// Initialize with default document
			Document = new TextDocument();
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			
			// Wire to theme system
			ThemeManager.Current.ThemeChanged += OnThemeChanged;
			
			// Wire to font settings
			displaySettings = TryGetDisplaySettings();
			if (displaySettings != null)
			{
				ApplyFontSettings();
				displaySettings.PropertyChanged += OnDisplaySettingsChanged;
			}
			
			// Apply theme-aware background
			ApplyThemeResources();
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			if (displaySettings != null)
			{
				displaySettings.PropertyChanged -= OnDisplaySettingsChanged;
				displaySettings = null;
			}
			ThemeManager.Current.ThemeChanged -= OnThemeChanged;
			base.OnDetachedFromVisualTree(e);
		}

		DisplaySettings? TryGetDisplaySettings()
		{
			try
			{
				return AppEnv.AppComposition.TryGetExport<SettingsService>()?.DisplaySettings;
			}
			catch
			{
				return null;
			}
		}

		void OnThemeChanged(object? sender, EventArgs e)
		{
			ApplyThemeResources();
			TextArea.TextView.Redraw();
		}

		void OnDisplaySettingsChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(DisplaySettings.SelectedFont)
			    or nameof(DisplaySettings.SelectedFontSize)
			    or nameof(DisplaySettings.EditorZoomFactor))
			{
				ApplyFontSettings();
			}
		}

		void ApplyFontSettings()
		{
			if (displaySettings == null)
				return;

			if (!string.IsNullOrEmpty(displaySettings.SelectedFont))
				FontFamily = new FontFamily(displaySettings.SelectedFont);

			if (displaySettings.SelectedFontSize > 0)
				FontSize = EditorZoom.EffectiveFontSize(displaySettings);
		}

		void ApplyThemeResources()
		{
			// Bind to ILSpy.EditorBackground dynamic resource
			var bgResource = Application.Current?.TryGetResource("ILSpy.EditorBackground", 
				ThemeManager.Current.IsDarkTheme ? Avalonia.Styling.ThemeVariant.Dark : Avalonia.Styling.ThemeVariant.Light,
				out var bgValue);
			
			if (bgResource == true && bgValue is IBrush bgBrush)
				Background = bgBrush;

			// Bind selection brush
			var selResource = Application.Current?.TryGetResource("ILSpy.EditorSelectionBrush",
				ThemeManager.Current.IsDarkTheme ? Avalonia.Styling.ThemeVariant.Dark : Avalonia.Styling.ThemeVariant.Light,
				out var selValue);
			
			if (selResource == true && selValue is IBrush selBrush)
				TextArea.SelectionBrush = selBrush;
		}
	}
}
```

#### File: `ILSpy/AI/Controls/MarkdownTextEditor.axaml`

```xml
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="using:ICSharpCode.ILSpy.AI.Controls">
	<Design.PreviewWith>
		<local:MarkdownTextEditor Width="400" Height="300" />
	</Design.PreviewWith>

	<Style Selector="local|MarkdownTextEditor">
		<Setter Property="BorderThickness" Value="0" />
		<Setter Property="Padding" Value="4" />
	</Style>
</Styles>
```

**Testing Checklist:**
- [ ] Create a test window/dialog with MarkdownTextEditor
- [ ] Load sample markdown text with headings, code blocks, lists, emphasis
- [ ] Verify syntax highlighting is applied (headings colored, code blocks colored)
- [ ] Switch theme (Light ↔ Dark) and verify colors update
- [ ] Change font in Options → Display Settings and verify it applies
- [ ] Verify word wrap works
- [ ] Verify text is selectable and copyable
- [ ] Verify control works in both Light and Dark themes

**Commit Point:** "Add MarkdownTextEditor control for AI panes"

---

### Work Package 1.2: Create StreamingMarkdownControl

**Goal:** Drop-in replacement for existing `StreamingTextControl`.

**Files to Modify:**
- `ILSpy/AI/StreamingTextControl.axaml`
- `ILSpy/AI/StreamingTextControl.axaml.cs`

**Implementation:**

#### File: `ILSpy/AI/StreamingTextControl.axaml` (REPLACE CONTENT)

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:ICSharpCode.ILSpy.AI"
             xmlns:controls="using:ICSharpCode.ILSpy.AI.Controls"
             x:Class="ICSharpCode.ILSpy.AI.StreamingTextControl">
	<controls:MarkdownTextEditor x:Name="Editor" />
</UserControl>
```

#### File: `ILSpy/AI/StreamingTextControl.axaml.cs` (REPLACE CONTENT)

```csharp
// Copyright (c) 2026 Masroor
using Avalonia.Controls;
using AvaloniaEdit.Document;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Wraps MarkdownTextEditor for streaming text updates.
	/// Drop-in replacement for the old TextBox-based StreamingTextControl.
	/// </summary>
	public partial class StreamingTextControl : UserControl
	{
		public StreamingTextControl()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Sets the complete text content (replaces existing content).
		/// </summary>
		public string Text {
			get => Editor.Document?.Text ?? string.Empty;
			set {
				if (Editor.Document == null)
					Editor.Document = new TextDocument();
				Editor.Document.Text = value ?? string.Empty;
				ScrollToEnd();
			}
		}

		/// <summary>
		/// Appends text to the end of the document (for streaming).
		/// More efficient than replacing the entire Text property.
		/// </summary>
		public void AppendText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return;
			
			if (Editor.Document == null)
				Editor.Document = new TextDocument();
			
			Editor.Document.Insert(Editor.Document.TextLength, text);
			ScrollToEnd();
		}

		/// <summary>
		/// Clears all text from the editor.
		/// </summary>
		public void Clear()
		{
			if (Editor.Document != null)
				Editor.Document.Text = string.Empty;
		}

		void ScrollToEnd()
		{
			// Scroll to bottom to follow streaming text
			Editor.ScrollToEnd();
		}
	}
}
```

**Testing Checklist:**
- [ ] Replace `StreamingTextControl` in `AIOutputPane.axaml` (no other changes yet)
- [ ] Run ILSpy and trigger an AI explanation
- [ ] Verify markdown syntax highlighting appears
- [ ] Verify streaming works (text appears incrementally)
- [ ] Verify text is selectable/copyable
- [ ] Verify Copy button still works

**Commit Point:** "Replace StreamingTextControl with MarkdownTextEditor"

---

### Work Package 1.3: Update AIOutputPaneModel for Streaming

**Goal:** Use `AppendText()` for better streaming performance.

**Files to Modify:**
- `ILSpy/AI/AIOutputPaneModel.cs`

**Changes:**

Currently (`ConsumeAsync` method, lines 181-206):
```csharp
var response = new StringBuilder();
// ... foreach chunk
response.Append(chunk);
await Dispatcher.UIThread.InvokeAsync(() => {
    if (ReferenceEquals(cancellation, requestCancellation))
        Response = response.ToString(); // Full text replacement every chunk
});
```

**Replace with buffered approach:**

```csharp
async Task ConsumeAsync(Func<CancellationToken, IAsyncEnumerable<string>> streamFactory, CancellationTokenSource requestCancellation)
{
    var response = new StringBuilder();
    logger.LogDebug("Starting to consume AI response stream");
    int chunkCount = 0;
    int chunksSinceUpdate = 0;
    const int UpdateInterval = 5; // Rerender every 5 chunks to reduce flicker
    
    await foreach (string chunk in streamFactory(requestCancellation.Token).ConfigureAwait(false))
    {
        if (string.IsNullOrEmpty(chunk))
            continue;
        chunkCount++;
        chunksSinceUpdate++;
        response.Append(chunk);
        logger.LogTrace("Received chunk #{ChunkNumber}, length: {Length}", chunkCount, chunk.Length);
        
        if (chunksSinceUpdate >= UpdateInterval)
        {
            await Dispatcher.UIThread.InvokeAsync(() => {
                if (ReferenceEquals(cancellation, requestCancellation))
                    Response = response.ToString();
            });
            chunksSinceUpdate = 0;
        }
    }
    
    // Final update to ensure last chunks are displayed
    logger.LogInformation("AI response stream complete. Total chunks: {ChunkCount}, total length: {Length}", chunkCount, response.Length);
    await Dispatcher.UIThread.InvokeAsync(() => {
        if (ReferenceEquals(cancellation, requestCancellation))
        {
            Response = response.ToString();
            IsComplete = response.Length != 0;
            StatusMessage = response.Length == 0 ? "The provider returned an empty response." : "Complete";
        }
    });
}
```

**Alternative (more efficient):** If the above still causes flicker, use `AppendText()` directly:

1. Add a reference to the `StreamingTextControl` in `AIOutputPane.axaml.cs`
2. Call `streamingTextControl.AppendText(chunk)` directly instead of updating `Response` property

**Testing Checklist:**
- [ ] Trigger AI explanation for a method
- [ ] Verify streaming appears smooth (no excessive flicker)
- [ ] Verify final response is complete
- [ ] Verify Cancel button works mid-stream
- [ ] Verify Copy button works after completion

**Commit Point:** "Optimize AIOutputPaneModel streaming for MarkdownTextEditor"

---

## Phase 2: AIChatPane Integration

**Goal:** Apply markdown highlighting to chat message history.

**Estimated Time:** 1-2 hours  
**Commit Frequency:** After each functional change

### Work Package 2.1: Create ChatMessageControl

**Goal:** Reusable control for displaying a single chat message with markdown highlighting.

**Files to Create:**
- `ILSpy/AI/Controls/ChatMessageControl.axaml`
- `ILSpy/AI/Controls/ChatMessageControl.axaml.cs`

**Implementation:**

#### File: `ILSpy/AI/Controls/ChatMessageControl.axaml`

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:ICSharpCode.ILSpy.AI.Controls"
             x:Class="ICSharpCode.ILSpy.AI.Controls.ChatMessageControl">
	<Border Padding="8" Margin="0,4" Background="#22000000" CornerRadius="4">
		<DockPanel>
			<!-- Role label -->
			<TextBlock DockPanel.Dock="Top" 
			           Text="{Binding Role}" 
			           FontWeight="Bold" 
			           Margin="0,0,0,4" />
			
			<!-- Message content with markdown highlighting -->
			<local:MarkdownTextEditor x:Name="ContentEditor"
			                          MinHeight="40"
			                          MaxHeight="500"
			                          BorderThickness="0"
			                          Background="Transparent" />
		</DockPanel>
	</Border>
</UserControl>
```

#### File: `ILSpy/AI/Controls/ChatMessageControl.axaml.cs`

```csharp
// Copyright (c) 2026 Masroor
using Avalonia.Controls;
using AvaloniaEdit.Document;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.AI.Controls
{
	public partial class ChatMessageControl : UserControl
	{
		public ChatMessageControl()
		{
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
		}

		void OnDataContextChanged(object? sender, EventArgs e)
		{
			if (DataContext is ChatMessage message)
			{
				UpdateContent(message.Content);
				message.PropertyChanged += OnMessagePropertyChanged;
			}
		}

		void OnMessagePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ChatMessage.Content) && sender is ChatMessage message)
			{
				UpdateContent(message.Content);
			}
		}

		void UpdateContent(string content)
		{
			if (ContentEditor.Document == null)
				ContentEditor.Document = new TextDocument();
			ContentEditor.Document.Text = content ?? string.Empty;
		}
	}
}
```

**Testing Checklist:**
- [ ] Create test window with sample ChatMessage instances
- [ ] Verify role label displays
- [ ] Verify markdown content is syntax-highlighted
- [ ] Verify updates when Content property changes (streaming simulation)

**Commit Point:** "Add ChatMessageControl with markdown highlighting"

---

### Work Package 2.2: Update AIChatPane to Use ChatMessageControl

**Goal:** Replace plain TextBlock with ChatMessageControl in message list.

**Files to Modify:**
- `ILSpy/AI/AIChatPane.axaml`

**Changes:**

Current template (line 14-16):
```xml
<ListBox ItemsSource="{Binding Messages}">
  <ListBox.ItemTemplate><DataTemplate><Border Padding="8" Margin="0,4" Background="#22000000"><StackPanel><TextBlock Text="{Binding Role}" FontWeight="Bold"/><TextBlock Text="{Binding Content}" TextWrapping="Wrap"/></StackPanel></Border></DataTemplate></ListBox.ItemTemplate>
</ListBox>
```

**Replace with:**

```xml
<ListBox ItemsSource="{Binding Messages}" 
         SelectionMode="Single"
         Background="Transparent">
  <ListBox.ItemTemplate>
    <DataTemplate>
      <controls:ChatMessageControl />
    </DataTemplate>
  </ListBox.ItemTemplate>
  <!-- Remove item container chrome -->
  <ListBox.Styles>
    <Style Selector="ListBoxItem">
      <Setter Property="Padding" Value="0" />
      <Setter Property="Margin" Value="0" />
      <Setter Property="Background" Value="Transparent" />
    </Style>
    <Style Selector="ListBoxItem:selected /template/ ContentPresenter">
      <Setter Property="Background" Value="Transparent" />
    </Style>
  </ListBox.Styles>
</ListBox>
```

Add namespace at top of file:
```xml
xmlns:controls="using:ICSharpCode.ILSpy.AI.Controls"
```

**Testing Checklist:**
- [ ] Open AI Chat pane
- [ ] Send a message with markdown (headings, code blocks, lists)
- [ ] Verify markdown is syntax-highlighted in the response
- [ ] Verify streaming updates work (assistant message builds incrementally)
- [ ] Verify scrolling works smoothly
- [ ] Verify Clear command works
- [ ] Verify Export command still works

**Commit Point:** "Replace AIChatPane TextBlock with ChatMessageControl"

---

## Phase 3: ExplainDialog Integration

**Goal:** Apply markdown highlighting to the modal explanation dialog.

**Estimated Time:** 30 minutes  
**Commit Frequency:** Single commit after testing

### Work Package 3.1: Update ExplainDialog

**Files to Modify:**
- `ILSpy/AI/ExplainDialog.axaml`

**Changes:**

Current (contains direct TextBox):
```xml
<TextBox Text="{Binding Response}" 
         IsReadOnly="True" 
         AcceptsReturn="True" 
         TextWrapping="Wrap" />
```

**Replace with:**

```xml
<controls:MarkdownTextEditor x:Name="ContentEditor" />
```

Add namespace:
```xml
xmlns:controls="using:ICSharpCode.ILSpy.AI.Controls"
```

**Files to Modify:**
- `ILSpy/AI/ExplainDialogViewModel.cs`

**Changes:**

If the dialog binds `Response` property to the TextBox, you'll need to update the dialog code-behind to set the MarkdownTextEditor's Document.Text instead.

**Option A (if binding works):** Keep existing Response property binding - the MarkdownTextEditor can be made bindable.

**Option B (manual update):** In `ExplainDialog.axaml.cs`, subscribe to ViewModel Response changes:

```csharp
protected override void OnDataContextChanged(EventArgs e)
{
    base.OnDataContextChanged(e);
    if (DataContext is ExplainDialogViewModel vm)
    {
        vm.PropertyChanged += (s, args) => {
            if (args.PropertyName == nameof(ExplainDialogViewModel.Response))
            {
                ContentEditor.Document.Text = vm.Response;
            }
        };
    }
}
```

**Testing Checklist:**
- [ ] Right-click a method → "Explain with AI"
- [ ] Verify markdown syntax highlighting in the dialog
- [ ] Verify streaming works (if dialog supports streaming)
- [ ] Verify Copy/Close buttons work
- [ ] Test in both Light and Dark themes

**Commit Point:** "Add markdown highlighting to ExplainDialog"

---

## Phase 4: Code Fence Parsing Foundation

**Goal:** Lay groundwork for "Open in decompiler" feature by parsing markdown for code fences.

**Estimated Time:** 2 hours  
**Commit Frequency:** After parser implementation + tests

### Work Package 4.1: Add Markdig Package

**Goal:** Add markdown parser for extracting code fences.

**Files to Modify:**
- `Directory.Packages.props`

**Changes:**

Add after existing package versions (around line 85):
```xml
<PackageVersion Include="Markdig" Version="0.40.0" />
```

**Files to Modify:**
- `ICSharpCode.ILSpyX/ICSharpCode.ILSpyX.csproj`

Add PackageReference:
```xml
<PackageReference Include="Markdig" />
```

**Run:**
```bash
./updatedeps.ps1
```

**Commit Point:** "Add Markdig package for markdown parsing"

---

### Work Package 4.2: Create MarkdownCodeFenceExtractor

**Goal:** Utility class for extracting code fences from markdown text.

**Files to Create:**
- `ICSharpCode.ILSpyX/AI/MarkdownCodeFenceExtractor.cs`

**Implementation:**

```csharp
// Copyright (c) 2026 Masroor
using System;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Syntax;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Extracts code fences from markdown text for analysis and interaction.
	/// </summary>
	public static class MarkdownCodeFenceExtractor
	{
		/// <summary>
		/// Represents a code fence extracted from markdown.
		/// </summary>
		public sealed class CodeFence
		{
			/// <summary>Language identifier (e.g., "csharp", "il", "xml"), or null if unspecified.</summary>
			public string? Language { get; init; }
			
			/// <summary>The code content (without fence markers).</summary>
			public string Code { get; init; } = string.Empty;
			
			/// <summary>Zero-based line number where the fence starts in the source markdown.</summary>
			public int StartLine { get; init; }
			
			/// <summary>Zero-based line number where the fence ends in the source markdown.</summary>
			public int EndLine { get; init; }
			
			/// <summary>Is this a C# code fence?</summary>
			public bool IsCSharp => 
				Language != null && 
				(Language.Equals("csharp", StringComparison.OrdinalIgnoreCase) ||
				 Language.Equals("cs", StringComparison.OrdinalIgnoreCase) ||
				 Language.Equals("c#", StringComparison.OrdinalIgnoreCase));
			
			/// <summary>Is this an IL code fence?</summary>
			public bool IsIL => 
				Language != null && 
				Language.Equals("il", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Extracts all code fences from the given markdown text.
		/// </summary>
		public static IReadOnlyList<CodeFence> Extract(string markdown)
		{
			if (string.IsNullOrEmpty(markdown))
				return Array.Empty<CodeFence>();

			var document = Markdown.Parse(markdown);
			var fences = new List<CodeFence>();

			foreach (var block in document.Descendants<FencedCodeBlock>())
			{
				fences.Add(new CodeFence {
					Language = block.Info,
					Code = block.Lines.ToString(),
					StartLine = block.Line,
					EndLine = block.Line + block.Lines.Count
				});
			}

			return fences;
		}

		/// <summary>
		/// Extracts only C# code fences from the given markdown text.
		/// </summary>
		public static IReadOnlyList<CodeFence> ExtractCSharpFences(string markdown)
		{
			return Extract(markdown).Where(f => f.IsCSharp).ToList();
		}

		/// <summary>
		/// Extracts only IL code fences from the given markdown text.
		/// </summary>
		public static IReadOnlyList<CodeFence> ExtractILFences(string markdown)
		{
			return Extract(markdown).Where(f => f.IsIL).ToList();
		}
	}
}
```

**Testing Checklist:**
- [ ] Create unit test with sample markdown containing multiple fences
- [ ] Verify C# fences are detected (csharp, cs, c#)
- [ ] Verify IL fences are detected
- [ ] Verify fences without language tag are captured (Language = null)
- [ ] Verify line numbers are correct
- [ ] Verify code content excludes fence markers

**Commit Point:** "Add MarkdownCodeFenceExtractor for parsing code blocks"

---

## Phase 5: "Open in Decompiler" Feature

**Goal:** Allow users to open C# code fences in a new decompiler tab.

**Estimated Time:** 3-4 hours  
**Commit Frequency:** After UI + command implementation

### Work Package 5.1: Add Context Menu to MarkdownTextEditor

**Goal:** Right-click on code fence → "Open in Decompiler"

**Files to Modify:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`

**Changes:**

Add property and method:

```csharp
/// <summary>
/// Event raised when user wants to open a code fence in the decompiler.
/// </summary>
public event EventHandler<CodeFenceEventArgs>? OpenCodeFenceRequested;

public class CodeFenceEventArgs : EventArgs
{
	public required MarkdownCodeFenceExtractor.CodeFence Fence { get; init; }
	public required string SourceMarkdown { get; init; }
}

/// <summary>
/// Finds the code fence at the current caret position and raises OpenCodeFenceRequested.
/// </summary>
public void OpenCodeFenceAtCaret()
{
	if (Document == null)
		return;

	string markdown = Document.Text;
	var fences = MarkdownCodeFenceExtractor.ExtractCSharpFences(markdown);
	
	if (fences.Count == 0)
		return;

	// Find fence containing current line
	int currentLine = Document.GetLineByOffset(CaretOffset).LineNumber - 1; // Convert to 0-based
	var fence = fences.FirstOrDefault(f => currentLine >= f.StartLine && currentLine <= f.EndLine);
	
	if (fence != null)
	{
		OpenCodeFenceRequested?.Invoke(this, new CodeFenceEventArgs {
			Fence = fence,
			SourceMarkdown = markdown
		});
	}
}
```

Add context menu in constructor:

```csharp
public MarkdownTextEditor()
{
	// ... existing initialization ...
	
	// Add context menu
	var contextMenu = new ContextMenu();
	var openInDecompilerItem = new MenuItem { Header = "Open in Decompiler" };
	openInDecompilerItem.Click += (s, e) => OpenCodeFenceAtCaret();
	contextMenu.Items.Add(openInDecompilerItem);
	
	var copyItem = new MenuItem { Header = "Copy" };
	copyItem.Click += (s, e) => Copy();
	contextMenu.Items.Add(copyItem);
	
	ContextMenu = contextMenu;
}
```

**Testing Checklist:**
- [ ] Right-click inside a C# code fence
- [ ] Verify "Open in Decompiler" appears in context menu
- [ ] Right-click outside a code fence
- [ ] Verify context menu still works (Copy option)

**Commit Point:** "Add context menu to MarkdownTextEditor with 'Open in Decompiler'"

---

### Work Package 5.2: Implement OpenCodeFence Command

**Goal:** Wire the context menu action to `DockWorkspace.ShowTextInNewTab()`.

**Files to Modify:**
- `ILSpy/AI/StreamingTextControl.axaml.cs`
- `ILSpy/AI/Controls/ChatMessageControl.axaml.cs`
- `ILSpy/AI/ExplainDialog.axaml.cs`

**Implementation Pattern (same for all three):**

```csharp
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpyX.AI;

// In constructor:
Editor.OpenCodeFenceRequested += OnOpenCodeFenceRequested;

void OnOpenCodeFenceRequested(object? sender, MarkdownTextEditor.CodeFenceEventArgs e)
{
	var dockWorkspace = AppEnv.AppComposition.TryGetExport<DockWorkspace>();
	if (dockWorkspace == null)
		return;

	var output = new AvaloniaEditTextOutput();
	output.Title = "AI Code Snippet";
	output.SyntaxExtensionOverride = e.Fence.IsCSharp ? ".cs" 
		: e.Fence.IsIL ? ".il" 
		: ".txt";
	
	// Write the code
	output.Write(e.Fence.Code);
	
	// Open in new frozen tab
	dockWorkspace.ShowTextInNewTab("AI Code Snippet", output);
}
```

**Testing Checklist:**
- [ ] Right-click C# fence in AIOutputPane → "Open in Decompiler"
- [ ] Verify new tab opens with C# syntax highlighting
- [ ] Verify code is correctly displayed
- [ ] Verify tab is frozen (doesn't get replaced by tree navigation)
- [ ] Test same flow in AIChatPane
- [ ] Test same flow in ExplainDialog
- [ ] Test with IL code fence (verify IL highlighting)

**Commit Point:** "Wire 'Open in Decompiler' to DockWorkspace.ShowTextInNewTab"

---

### Work Package 5.3: Add "Copy Code Block" Button (Optional Enhancement)

**Goal:** Quick-copy button for code fences without opening in decompiler.

**Files to Modify:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`

**Implementation:**

Add to context menu:

```csharp
var copyCodeItem = new MenuItem { Header = "Copy Code Block" };
copyCodeItem.Click += (s, e) => CopyCodeFenceAtCaret();
contextMenu.Items.Add(copyCodeItem);

public void CopyCodeFenceAtCaret()
{
	if (Document == null)
		return;

	string markdown = Document.Text;
	var fences = MarkdownCodeFenceExtractor.Extract(markdown);
	
	if (fences.Count == 0)
		return;

	int currentLine = Document.GetLineByOffset(CaretOffset).LineNumber - 1;
	var fence = fences.FirstOrDefault(f => currentLine >= f.StartLine && currentLine <= f.EndLine);
	
	if (fence != null)
	{
		Application.Current?.Clipboard?.SetTextAsync(fence.Code);
	}
}
```

**Testing Checklist:**
- [ ] Right-click code fence → "Copy Code Block"
- [ ] Paste in external editor
- [ ] Verify only code is copied (no fence markers)
- [ ] Verify markdown with multiple fences copies the correct one

**Commit Point:** "Add 'Copy Code Block' context menu action"

---

## Phase 6: Polish and Documentation

**Goal:** Final touches, documentation, and user-facing improvements.

**Estimated Time:** 1-2 hours

### Work Package 6.1: Update CHANGELOG

**Files to Modify:**
- `CHANGELOG.md` (if exists) or create release notes

**Content:**

```markdown
## AI Panes - Markdown Syntax Highlighting

### New Features
- AI responses now display with markdown syntax highlighting (headings, code blocks, emphasis)
- Code fences in AI responses can be opened in new decompiler tabs with full syntax highlighting
- Right-click context menu on code blocks: "Open in Decompiler", "Copy Code Block"

### Improvements
- Faster streaming performance in AI Output and Chat panes
- Better theme integration for AI panes (respects Light/Dark theme)
- Text selection and copy now work in all AI panes

### Technical Details
- Replaced plain TextBox controls with AvaloniaEdit TextEditor
- Uses existing ILSpy markdown syntax highlighting (no new dependencies)
- Leverages ThemeManager and DisplaySettings for consistent theming
```

**Commit Point:** "Document markdown highlighting feature in CHANGELOG"

---

### Work Package 6.2: Add Keyboard Shortcuts (Optional)

**Goal:** Power user shortcuts for common actions.

**Suggested Shortcuts:**
- `Ctrl+Shift+O` - Open code fence at caret in decompiler
- `Ctrl+Shift+C` - Copy code block at caret

**Files to Modify:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`

**Implementation:**

```csharp
protected override void OnKeyDown(KeyEventArgs e)
{
	base.OnKeyDown(e);
	
	if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift))
	{
		if (e.Key == Key.O)
		{
			OpenCodeFenceAtCaret();
			e.Handled = true;
		}
		else if (e.Key == Key.C)
		{
			CopyCodeFenceAtCaret();
			e.Handled = true;
		}
	}
}
```

**Testing Checklist:**
- [ ] Place caret in code fence
- [ ] Press Ctrl+Shift+O
- [ ] Verify new tab opens with code
- [ ] Press Ctrl+Shift+C
- [ ] Verify code copied to clipboard

**Commit Point:** "Add keyboard shortcuts for code fence actions"

---

### Work Package 6.3: Add Tooltip/Status Feedback

**Goal:** Visual feedback when actions are performed.

**Files to Modify:**
- `ILSpy/AI/Controls/MarkdownTextEditor.cs`

**Implementation:**

```csharp
void ShowStatusMessage(string message)
{
	// Option A: Use ILSpy's status bar if accessible
	var mainWindow = this.FindAncestorOfType<MainWindow>();
	mainWindow?.ShowStatus(message); // If such method exists
	
	// Option B: Show tooltip
	ToolTip.SetTip(this, message);
	ToolTip.SetIsOpen(this, true);
	
	// Auto-hide after 2 seconds
	var timer = new System.Threading.Timer(_ => {
		Dispatcher.UIThread.Post(() => ToolTip.SetIsOpen(this, false));
	}, null, 2000, System.Threading.Timeout.Infinite);
}

// Update methods:
public void CopyCodeFenceAtCaret()
{
	// ... existing code ...
	if (fence != null)
	{
		Application.Current?.Clipboard?.SetTextAsync(fence.Code);
		ShowStatusMessage("Code block copied to clipboard");
	}
}

void OnOpenCodeFenceRequested(...)
{
	// ... existing code ...
	dockWorkspace.ShowTextInNewTab("AI Code Snippet", output);
	ShowStatusMessage("Opened code in new tab");
}
```

**Testing Checklist:**
- [ ] Copy code block → verify status message appears
- [ ] Open in decompiler → verify status message appears
- [ ] Verify tooltip auto-hides after 2 seconds

**Commit Point:** "Add status feedback for code fence actions"

---

## Phase 7: Testing and Bug Fixes

**Goal:** Comprehensive testing across all scenarios.

**Estimated Time:** 2-3 hours

### Test Matrix

| Scenario | AIOutputPane | AIChatPane | ExplainDialog |
|----------|--------------|------------|---------------|
| Markdown headings highlighted | ⬜ | ⬜ | ⬜ |
| Code fences highlighted | ⬜ | ⬜ | ⬜ |
| Lists and emphasis colored | ⬜ | ⬜ | ⬜ |
| Streaming works smoothly | ⬜ | ⬜ | N/A |
| Text selectable/copyable | ⬜ | ⬜ | ⬜ |
| Light theme correct | ⬜ | ⬜ | ⬜ |
| Dark theme correct | ⬜ | ⬜ | ⬜ |
| Font changes apply | ⬜ | ⬜ | ⬜ |
| "Open in Decompiler" works | ⬜ | ⬜ | ⬜ |
| "Copy Code Block" works | ⬜ | ⬜ | ⬜ |
| Context menu appears | ⬜ | ⬜ | ⬜ |
| Keyboard shortcuts work | ⬜ | ⬜ | ⬜ |

### Common Issues to Watch For

1. **Memory Leaks**
   - Verify event handlers are unsubscribed in `OnDetachedFromVisualTree`
   - Check `PropertyChanged` subscriptions are cleaned up
   - Test opening/closing panes repeatedly

2. **Theme Switching**
   - Open AI pane in Light theme
   - Switch to Dark theme via Options
   - Verify colors update without restart
   - Verify no visual artifacts

3. **Font Scaling**
   - Change font size in Options → Display Settings
   - Verify AI panes update immediately
   - Verify text remains readable at min/max sizes

4. **Streaming Edge Cases**
   - Cancel mid-stream
   - Empty response
   - Very long response (>10k chars)
   - Rapid successive requests

5. **Code Fence Detection**
   - Fence without language tag
   - Multiple fences of same language
   - Nested code-like content (not in fence)
   - Incomplete fence (missing closing ```)

**Commit Point:** "Fix identified bugs and edge cases"

---

## Testing Checklist (Pre-Merge)

### Functional Testing

**AIOutputPane:**
- [ ] Explain a method → verify markdown highlighting
- [ ] Explain a property → verify streaming works
- [ ] Cancel mid-stream → verify clean cancellation
- [ ] Copy button → verify copies raw markdown
- [ ] Clear button → verify pane resets

**AIChatPane:**
- [ ] Send message with markdown → verify highlighting in response
- [ ] Send multiple messages → verify history displays correctly
- [ ] Export chat → verify .md file is valid
- [ ] Clear chat → verify messages are removed
- [ ] Scroll to old messages → verify rendering is correct

**ExplainDialog:**
- [ ] Right-click method → Explain with AI
- [ ] Verify markdown highlighting in dialog
- [ ] Close dialog → verify no memory leak
- [ ] Open multiple times → verify consistent behavior

**Code Fence Actions:**
- [ ] Right-click C# fence → "Open in Decompiler"
- [ ] Verify new tab opens with C# highlighting
- [ ] Right-click IL fence → "Open in Decompiler"
- [ ] Verify IL highlighting applied
- [ ] "Copy Code Block" → verify code copied without fence markers
- [ ] Keyboard shortcut Ctrl+Shift+O → verify works

### Theme Testing

- [ ] Start in Light theme → verify AI panes are readable
- [ ] Switch to Dark theme → verify colors update immediately
- [ ] Switch back to Light → verify no artifacts
- [ ] Options → Display Settings → Font → verify updates apply
- [ ] Options → Display Settings → Font Size → verify updates apply

### Performance Testing

- [ ] Request AI response for large type (>1000 lines)
- [ ] Verify streaming remains smooth
- [ ] Verify UI remains responsive during streaming
- [ ] Monitor memory usage (should not leak)
- [ ] Open/close panes repeatedly (should not leak)

### Cross-Platform Testing (if applicable)

- [ ] Windows - verify all features work
- [ ] macOS - verify all features work
- [ ] Linux - verify all features work

---

## Rollback Plan

If critical issues are discovered post-merge:

1. **Partial Rollback:** Revert specific phases
   - Phase 5 (Code Fence Actions) can be reverted independently
   - Phase 2 (AIChatPane) can be reverted independently
   - Phase 3 (ExplainDialog) can be reverted independently

2. **Full Rollback:** Revert to plain TextBox
   - Revert `StreamingTextControl` changes
   - Remove `MarkdownTextEditor` and `ChatMessageControl`
   - Users get plain text but functionality restored

3. **Fallback Strategy:** Add toggle in settings
   - Add `DisplaySettings.UseMarkdownHighlighting` boolean
   - Default = true (new behavior)
   - Users can opt-out if issues arise

---

## Future Enhancements (Not in Current Plan)

**Priority: Low**
- Clickable links in markdown (intercept URL clicks)
- Rendered tables (custom syntax highlighting for table rows)
- Collapsible code fences (folding support)
- Diff highlighting for code comparisons
- Export individual code fences to files
- "Open all C# fences in tabs" bulk action
- Custom markdown color scheme separate from code highlighting

**Priority: Medium**
- Mermaid diagram rendering (if AI generates diagrams)
- LaTeX math rendering (if AI generates formulas)
- Image embedding (if AI sends image URLs)

**Priority: High** (consider for next iteration)
- Full markdown rendering with Markdown.Avalonia (parallel to syntax highlighting)
- Toggle between rendered and source views
- Hybrid approach: render prose, syntax-highlight code fences

---

## Success Criteria

This implementation is considered successful when:

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

## Estimated Total Time

- Phase 1: 2-3 hours (Foundation)
- Phase 2: 1-2 hours (AIChatPane)
- Phase 3: 0.5 hours (ExplainDialog)
- Phase 4: 2 hours (Code Fence Parsing)
- Phase 5: 3-4 hours (Open in Decompiler)
- Phase 6: 1-2 hours (Polish)
- Phase 7: 2-3 hours (Testing)

**Total: 12-16 hours** across 7 phases

**Recommended approach:** Implement phases 1-3 first (core functionality), test thoroughly, then add phases 4-5 (code fence features) in a second iteration.

---

## Notes for Implementation

1. **Commit Frequency:** Commit after each work package (12-15 commits total)
2. **Branch Strategy:** Create feature branch `feature/markdown-ai-panes`
3. **PR Strategy:** Can split into two PRs:
   - PR 1: Phases 1-3 (Core markdown highlighting)
   - PR 2: Phases 4-5 (Code fence actions)
4. **Code Review Focus:**
   - Memory leak prevention (event handler cleanup)
   - Theme integration correctness
   - Performance of streaming updates
   - Code fence parsing accuracy

---

**END OF PLAN**
