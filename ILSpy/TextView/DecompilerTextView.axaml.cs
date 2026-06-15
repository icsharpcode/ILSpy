// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	public partial class DecompilerTextView : UserControl
	{
		// Two-track output of BuildHoverContent: rich content opens the sticky Popup with the
		// pointer-distance corridor; plain content uses Avalonia's ToolTip attached property.
		internal readonly record struct HoverContent(Control Control, bool IsRich);

		// Local-reference highlight colours: light-yellow "GreenYellow" for matches and a
		// softer green for the actual definition.
		static readonly Color LocalMatchBackground = Colors.GreenYellow;
		static readonly Color LocalDefinitionBackground = Color.FromArgb(0x80, 0xA0, 0xFF, 0xA0);

		// Stay-open corridor for the rich popup: as long as the pointer is closer than
		// `distanceToPopupLimit` to the popup edges, the popup stays. The limit shrinks toward
		// the pointer's last best distance (bounded by MaxMovementAwayFromPopup) so the user
		// has room to "reach for" the popup but can't drift away from it.
		const double MaxMovementAwayFromPopup = 5;

		// Assigned once in the ctor's Setup* helpers (so not readonly -- the compiler can't prove
		// single-assignment across methods); never reassigned afterwards.
		ReferenceElementGenerator referenceElementGenerator = null!;
		UIElementGenerator uiElementGenerator = null!;
		TextMarkerService textMarkerService = null!;
		BracketHighlightRenderer bracketHighlightRenderer = null!;
		readonly List<TextMarker> localReferenceMarks = new();
		readonly List<AvaloniaEdit.Rendering.VisualLineElementGenerator> activeCustomGenerators = new();
		RichTextColorizer? activeColorizer;
		FoldingManager? activeFoldingManager;
		// Outgoing model tracked so OnDataContextChanged can detach handlers and snapshot
		// folding state before the new DataContext takes over -- Avalonia doesn't surface
		// the previous DataContext to the event handler, and Dock recycles the view across
		// tabs (see App.axaml ControlRecyclingKey).
		DecompilerTabPageModel? boundModel;
		// The editor's template ScrollViewer. AvaloniaEdit's TextEditor.ScrollViewer is internal,
		// and its ScrollToVerticalOffset/ScrollToHorizontalOffset are no-ops in 12.0.0
		// (AvaloniaUI/AvaloniaEdit#594) -- so we reach the ScrollViewer ourselves and set Offset.
		// TODO: drop this field and use Editor.ScrollToVerticalOffset once #594 ships and we bump the package.
		ScrollViewer? editorScrollViewer;
		DisplaySettings? currentDisplaySettings;
		ReferenceSegment? lastRightClickedSegment;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();
		Popup richPopup = null!;
		double distanceToPopupLimit;

		/// <summary>
		/// AvaloniaEdit's Ctrl+F search overlay, attached to the editor's TextArea at
		/// construction time. Public for tests; runtime users go through Ctrl+F.
		/// </summary>
		public AvaloniaEdit.Search.SearchPanel SearchPanel { get; }

		public DecompilerTextView()
		{
			InitializeComponent();

			// Ctrl+F find overlay. SearchPanel.Install registers the SearchInputHandler with
			// the TextArea's nested-handler chain so the gesture surfaces a search bar without
			// us wiring KeyBindings manually. Stored for tests; runtime path is the gesture.
			SearchPanel = AvaloniaEdit.Search.SearchPanel.Install(Editor);

			// AvaloniaEdit defaults to "Ctrl+Click to follow hyperlink" on its built-in
			// LinkElementGenerator and propagates that flag onto every VisualLineLinkText it
			// constructs (including those produced by user-added LinkElementGenerator subclasses
			// like the About-page resource generator). Decompiler output uses hyperlinks
			// extensively for in-app navigation — a plain click is the expected affordance.
			Editor.Options.RequireControlModifierForHyperlinkClick = false;

			SetupElementGenerators();
			SetupBackgroundRenderers();
			SetupHoverHandlers();
			SetupContextMenuAndHyperlinks();
			SetupRichPopup();
			WireUpDisplaySettings();

			// Caret-position changes drive the bracket-pair highlight. View state (caret + scroll
			// + foldings) is NOT pushed onto the model here -- DockWorkspace pulls it on demand via
			// the model's CaptureViewState delegate when it records a navigation away, so a
			// programmatic caret move (AvaloniaEdit jumps the caret to the end on a text replace)
			// is never mistaken for the user's position.
			Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

			SetupZoomAndCopy();

			// Ctrl+L focuses the omnibar into search mode (browser address-bar gesture). Tunnel so
			// it wins before AvaloniaEdit's own key handling while focus is anywhere in the editor.
			AddHandler(KeyDownEvent, OnPreviewKeyDownForOmnibar, RoutingStrategies.Tunnel);
		}

		void OnPreviewKeyDownForOmnibar(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.L && e.KeyModifiers == KeyModifiers.Control && Omnibar.IsVisible)
			{
				Omnibar.FocusSearch();
				e.Handled = true;
			}
		}

		// One generator lives for the lifetime of the view; we only swap its References collection
		// per document. The predicate filters out anything that should not be clickable -- references
		// with a null target slip through unconditionally otherwise.
		void SetupElementGenerators()
		{
			referenceElementGenerator = new ReferenceElementGenerator(static segment => segment.Reference != null);
			Editor.TextArea.TextView.ElementGenerators.Add(referenceElementGenerator);

			uiElementGenerator = new UIElementGenerator();
			Editor.TextArea.TextView.ElementGenerators.Add(uiElementGenerator);

			// Reference navigation fires on pointer-RELEASE without drag (WPF parity: the WPF
			// view used TextArea.PreviewMouseDown/Up the same way), so a press-and-drag over a
			// link starts a text selection instead of navigating away. Tunnel routing mirrors
			// WPF's Preview events and sees the gesture before AvaloniaEdit's own handlers.
			Editor.TextArea.AddHandler(InputElement.PointerPressedEvent,
				OnTextAreaPointerPressedForReferenceClick,
				RoutingStrategies.Tunnel,
				handledEventsToo: true);
			Editor.TextArea.AddHandler(InputElement.PointerReleasedEvent,
				OnTextAreaPointerReleasedForReferenceClick,
				RoutingStrategies.Tunnel,
				handledEventsToo: true);
		}

		// Position of the last left-button press, in this control's coordinates; null while no
		// press is in flight. The release compares against it to tell a click from a drag.
		Point? referenceClickStart;

		// WPF used SystemParameters.MinimumHorizontal/VerticalDragDistance, which default to 4.
		const double MinimumDragDistance = 4;

		void OnTextAreaPointerPressedForReferenceClick(object? sender, PointerPressedEventArgs e)
		{
			referenceClickStart = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed
				? e.GetPosition(this)
				: null;
		}

		void OnTextAreaPointerReleasedForReferenceClick(object? sender, PointerReleasedEventArgs e)
		{
			var start = referenceClickStart;
			referenceClickStart = null;
			if (start == null || e.InitialPressMouseButton != MouseButton.Left)
				return;
			var delta = e.GetPosition(this) - start.Value;
			if (Math.Abs(delta.X) >= MinimumDragDistance || Math.Abs(delta.Y) >= MinimumDragDistance)
				return;

			var segment = GetReferenceSegmentAtPointer(e);
			if (segment?.Reference == null)
			{
				// A click on empty space dismisses the local-reference highlight.
				ClearLocalReferenceMarks();
				return;
			}
			// Cancel the caret-click selection AvaloniaEdit started on press, and stop its
			// release processing, so the navigation's caret move doesn't grow a selection
			// between the old anchor and the new position.
			Editor.TextArea.ClearSelection();
			e.Handled = true;
			OnReferenceClicked(segment);
		}

		// Background renderers that live for the view's lifetime: the local-reference highlight (marks
		// cleared/rebuilt per click) and the bracket-pair outline (repainted on every caret move).
		void SetupBackgroundRenderers()
		{
			textMarkerService = new TextMarkerService(Editor.TextArea.TextView);
			Editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
			bracketHighlightRenderer = new BracketHighlightRenderer(Editor.TextArea.TextView);
		}

		// Subscribe to AvaloniaEdit's built-in PointerHover routed event (fires after the cursor
		// settles inside a small box for ~400 ms) and resolve the segment fresh at hover-time using
		// the live pointer position. PointerMoved still drives the rich popup's distance corridor.
		void SetupHoverHandlers()
		{
			ToolTip.SetPlacement(Editor.TextArea.TextView, PlacementMode.Pointer);
			ToolTip.SetBetweenShowDelay(Editor.TextArea.TextView, 0);
			Editor.TextArea.TextView.AddHandler(
				AvaloniaEdit.Rendering.TextView.PointerHoverEvent,
				OnTextViewPointerHover,
				RoutingStrategies.Bubble,
				handledEventsToo: true);
			Editor.TextArea.TextView.AddHandler(
				AvaloniaEdit.Rendering.TextView.PointerHoverStoppedEvent,
				OnTextViewPointerHoverStopped,
				RoutingStrategies.Bubble,
				handledEventsToo: true);
			Editor.TextArea.TextView.PointerMoved += OnTextViewPointerMoved;
			Editor.TextArea.TextView.PointerExited += OnTextViewPointerExited;
		}

		void SetupContextMenuAndHyperlinks()
		{
			// Context menu -- captures the segment under the pointer at right-click time so entries
			// like "Show in metadata" can dispatch through the same Reference field the assembly
			// tree's right-click already populates.
			Editor.TextArea.TextView.AddHandler(InputElement.PointerPressedEvent,
				OnTextViewPointerPressedForContextMenu,
				RoutingStrategies.Tunnel,
				handledEventsToo: true);
			AttachContextMenu(TryGetContextMenuEntries());

			// AvaloniaEdit's hyperlinks raise OpenUriEvent (bubbling); the default class handler on
			// Window passes the URI to Process.Start. Intercept first so internal schemes (the About
			// page's "resource:" URIs) route through the tab model instead.
			Editor.TextArea.TextView.AddHandler(
				AvaloniaEdit.Rendering.VisualLineLinkText.OpenUriEvent,
				OnHyperlinkOpenUri,
				RoutingStrategies.Bubble,
				handledEventsToo: false);
		}

		void SetupRichPopup()
		{
			richPopup = RichHoverPopup;
			richPopup.PlacementTarget = Editor.TextArea.TextView;
			// Pointer placement: Avalonia anchors the popup at the current cursor location when
			// Open() is called, with a small offset below to avoid covering the token.
			richPopup.Placement = PlacementMode.Pointer;
			richPopup.HorizontalOffset = 0;
			richPopup.VerticalOffset = 16;
			richPopup.IsLightDismissEnabled = false;
			richPopup.Closed += OnRichPopupClosed;
		}

		void SetupZoomAndCopy()
		{
			// Ctrl+Wheel zoom (matches WPF's ZoomScrollViewer.OnPreviewMouseWheel). Tunnel routing so
			// we see it before AvaloniaEdit's scroll handler; Handled=true on hit suppresses the
			// scroll. Ctrl+0/Plus/Minus are Avalonia-side extras -- keyboard zoom is a standard
			// modern-editor expectation that WPF ILSpy doesn't happen to ship.
			Editor.AddHandler(InputElement.PointerWheelChangedEvent,
				OnEditorPointerWheelChanged,
				RoutingStrategies.Tunnel,
				handledEventsToo: false);
			Editor.KeyDown += OnEditorKeyDownForZoom;

			// Ctrl+C copies as text + syntax-coloured HTML so a paste into an HTML target keeps the
			// highlighting. Tunnel so we run before AvaloniaEdit's plain-text copy keybinding and can
			// suppress it; falls through (Handled stays false) when there's no selection.
			Editor.AddHandler(InputElement.KeyDownEvent, OnEditorKeyDownForCopy,
				RoutingStrategies.Tunnel, handledEventsToo: false);
		}

		void OnEditorKeyDownForCopy(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.C && e.KeyModifiers == KeyModifiers.Control
				&& HtmlClipboardCopy.Copy(Editor, SemanticHighlightingModel))
			{
				e.Handled = true;
			}
		}

		/// <summary>ILSpy's semantic highlighting (decompiler reference / theme colours) for the current
		/// document, so an HTML copy can include it on top of the xshd syntax colours.</summary>
		internal RichTextModel? SemanticHighlightingModel => boundModel?.HighlightingModel;

		// ThemeManager.Current is a process-lived singleton, so subscribing to its ThemeChanged in
		// the constructor and never detaching would root every DecompilerTextView for the lifetime of
		// the process -- one leaked view per decompiler tab. Bind the handler to the visual-tree
		// lifetime instead: subscribe while attached, drop it on detach. Dock hides and re-shows tab
		// content, so this re-subscribes on every attach.
		protected override void OnAttachedToVisualTree(global::Avalonia.VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			ICSharpCode.ILSpy.Themes.ThemeManager.Current.ThemeChanged += OnThemeChangedRebuildHighlighting;
		}

		protected override void OnDetachedFromVisualTree(global::Avalonia.VisualTreeAttachmentEventArgs e)
		{
			ICSharpCode.ILSpy.Themes.ThemeManager.Current.ThemeChanged -= OnThemeChangedRebuildHighlighting;
			base.OnDetachedFromVisualTree(e);
		}

		// A theme switch re-colours the shared named HighlightingColors in place, but the
		// semantic RichTextModel cloned them at decompile time (RichTextModel.SetHighlighting
		// clones). Rebuild the model from the captured spans -- which still reference the live,
		// re-coloured instances -- so the already-decompiled output repaints with the new palette.
		void OnThemeChangedRebuildHighlighting(object? sender, System.EventArgs e)
		{
			if (boundModel?.HighlightingSpans is not { Count: > 0 } spans)
				return;
			var model = new RichTextModel();
			foreach (var (start, length, color) in spans)
				model.SetHighlighting(start, length, color);
			SetColorizer(model);
			Editor.TextArea.TextView.Redraw();
		}

		// Swap the semantic-highlighting colorizer to one for <paramref name="model"/>, or remove it
		// when null. AvaloniaEdit only exposes Add/Remove on LineTransformers, so we hold a reference
		// to the live one. Shared by ApplyDocument (per-document model) and the theme-change rebuild.
		void SetColorizer(RichTextModel? model)
		{
			var transformers = Editor.TextArea.TextView.LineTransformers;
			if (activeColorizer != null)
			{
				transformers.Remove(activeColorizer);
				activeColorizer = null;
			}
			if (model != null)
			{
				activeColorizer = new RichTextColorizer(model);
				transformers.Add(activeColorizer);
			}
		}

		void OnEditorPointerWheelChanged(object? sender, PointerWheelEventArgs e)
		{
			if ((e.KeyModifiers & KeyModifiers.Control) != KeyModifiers.Control)
				return;
			if (currentDisplaySettings == null)
				return;
			var step = e.Delta.Y > 0 ? (System.Func<double, double>)EditorZoom.ZoomIn : EditorZoom.ZoomOut;
			currentDisplaySettings.SelectedFontSize = step(currentDisplaySettings.SelectedFontSize);
			e.Handled = true;
		}

		/// <summary>True when the active document has any code foldings (gates the folding menu entries).</summary>
		public bool HasFoldings => activeFoldingManager is { } mgr && mgr.AllFoldings.Any();

		/// <summary>Number of currently-collapsed folds (test observation point for the folding commands).</summary>
		internal int FoldedFoldingCount => activeFoldingManager is { } mgr ? mgr.AllFoldings.Count(f => f.IsFolded) : 0;

		/// <summary>Toggles the innermost fold containing the caret (the "Toggle folding" command / Ctrl+M).</summary>
		public void ToggleFoldingAtCaret()
		{
			if (activeFoldingManager is not { } mgr)
				return;
			var caret = Editor.TextArea.Caret.Offset;
			FoldingSection? target = null;
			foreach (var f in mgr.AllFoldings)
			{
				if (f.StartOffset <= caret && caret <= f.EndOffset)
				{
					if (target == null || f.StartOffset > target.StartOffset)
						target = f;
				}
			}
			if (target != null)
				target.IsFolded = !target.IsFolded;
		}

		/// <summary>Collapses every fold when any is open, otherwise expands them all ("Toggle all folding"
		/// / Ctrl+Shift+M).</summary>
		public void ToggleAllFoldings()
		{
			if (activeFoldingManager is not { } mgr)
				return;
			bool anyOpen = false;
			foreach (var f in mgr.AllFoldings)
			{
				if (!f.IsFolded)
				{ anyOpen = true; break; }
			}
			foreach (var f in mgr.AllFoldings)
				f.IsFolded = anyOpen;
		}

		void OnEditorKeyDownForZoom(object? sender, KeyEventArgs e)
		{
			// Folding keyboard commands (Ctrl+M family). Mirrors typical IDE conventions:
			// Ctrl+M Ctrl+M-ish toggle for the fold nearest the caret; Ctrl+Shift+M for
			// collapse-all / expand-all by parity check. Implemented as single-key chords
			// (Ctrl+M / Ctrl+Shift+M) — full two-key chord support would need a key-state
			// machine wired through the TextArea which AvaloniaEdit doesn't ship.
			if ((e.KeyModifiers & KeyModifiers.Control) == KeyModifiers.Control && e.Key == Key.M)
			{
				if (activeFoldingManager is not null)
				{
					if ((e.KeyModifiers & KeyModifiers.Shift) == KeyModifiers.Shift)
						ToggleAllFoldings();
					else
						ToggleFoldingAtCaret();
					e.Handled = true;
					return;
				}
			}

			if ((e.KeyModifiers & KeyModifiers.Control) != KeyModifiers.Control)
				return;
			if (currentDisplaySettings == null)
				return;
			// OemPlus is the unmodified key on the same physical button as "+"; with Shift it
			// produces "+", without it produces "=". Accept both so Ctrl+Plus and Ctrl+=
			// (which most users actually type) both zoom in. Add (numeric keypad) covers numpad.
			switch (e.Key)
			{
				case Key.OemPlus:
				case Key.Add:
					currentDisplaySettings.SelectedFontSize = EditorZoom.ZoomIn(currentDisplaySettings.SelectedFontSize);
					e.Handled = true;
					break;
				case Key.OemMinus:
				case Key.Subtract:
					currentDisplaySettings.SelectedFontSize = EditorZoom.ZoomOut(currentDisplaySettings.SelectedFontSize);
					e.Handled = true;
					break;
				case Key.D0:
				case Key.NumPad0:
					currentDisplaySettings.SelectedFontSize = EditorZoom.Reset();
					e.Handled = true;
					break;
			}
		}

		void OnCaretPositionChanged(object? sender, EventArgs e)
		{
			UpdateBracketHighlight();
		}

		void UpdateBracketHighlight()
		{
			// Skip when the user disabled the feature in Display Settings, and clear any
			// existing highlight so a previous one doesn't linger after the toggle flips.
			if (currentDisplaySettings is { HighlightMatchingBraces: false })
			{
				bracketHighlightRenderer.SetHighlight(null);
				return;
			}
			var language = (DataContext as DecompilerTabPageModel)?.Language;
			var searcher = language?.BracketSearcher ?? DefaultBracketSearcher.DefaultInstance;
			var result = searcher.SearchBracket(Editor.Document, Editor.TextArea.Caret.Offset);
			bracketHighlightRenderer.SetHighlight(result);
		}

		/// <summary>
		/// Mirrors the live <see cref="DisplaySettings"/> into the editor on construction and on
		/// every <c>PropertyChanged</c> after that. Lets the Options Display panel produce
		/// immediate visual feedback without a re-decompile.
		/// </summary>
		void WireUpDisplaySettings()
		{
			var settings = TryGetDisplaySettings();
			if (settings == null)
				return;
			currentDisplaySettings = settings;
			ZoomButtons.Bind(settings);
			ApplyAllDisplaySettings(settings);
			settings.PropertyChanged += (_, e) => ApplyDisplaySetting(settings, e.PropertyName);
		}

		static DisplaySettings? TryGetDisplaySettings()
		{
			try
			{ return AppComposition.Current.GetExport<SettingsService>().DisplaySettings; }
			catch { return null; }
		}

		void ApplyAllDisplaySettings(DisplaySettings s)
		{
			ApplyDisplaySetting(s, nameof(DisplaySettings.SelectedFont));
			ApplyDisplaySetting(s, nameof(DisplaySettings.SelectedFontSize));
			ApplyDisplaySetting(s, nameof(DisplaySettings.ShowLineNumbers));
			ApplyDisplaySetting(s, nameof(DisplaySettings.EnableWordWrap));
			ApplyDisplaySetting(s, nameof(DisplaySettings.HighlightCurrentLine));
			ApplyDisplaySetting(s, nameof(DisplaySettings.IndentationSize));
			ApplyDisplaySetting(s, nameof(DisplaySettings.IndentationUseTabs));
			ApplyDisplaySetting(s, nameof(DisplaySettings.EnableOmnibar));
		}

		void ApplyDisplaySetting(DisplaySettings s, string? propertyName)
		{
			switch (propertyName)
			{
				case nameof(DisplaySettings.SelectedFont):
					if (!string.IsNullOrEmpty(s.SelectedFont))
						Editor.FontFamily = new FontFamily(s.SelectedFont);
					break;
				case nameof(DisplaySettings.SelectedFontSize):
					if (s.SelectedFontSize > 0)
						Editor.FontSize = s.SelectedFontSize;
					break;
				case nameof(DisplaySettings.ShowLineNumbers):
					Editor.ShowLineNumbers = s.ShowLineNumbers;
					break;
				case nameof(DisplaySettings.EnableWordWrap):
					Editor.WordWrap = s.EnableWordWrap;
					break;
				case nameof(DisplaySettings.HighlightCurrentLine):
					Editor.Options.HighlightCurrentLine = s.HighlightCurrentLine;
					break;
				case nameof(DisplaySettings.IndentationSize):
					if (s.IndentationSize > 0)
						Editor.Options.IndentationSize = s.IndentationSize;
					break;
				case nameof(DisplaySettings.IndentationUseTabs):
					Editor.Options.ConvertTabsToSpaces = !s.IndentationUseTabs;
					break;
				case nameof(DisplaySettings.EnableOmnibar):
					// Off by default; the Options Display "Tab options" toggle shows/hides the bar
					// live without a re-decompile, per text view.
					Omnibar.IsVisible = s.EnableOmnibar;
					break;
			}
		}

		static IReadOnlyList<IContextMenuEntryExport> TryGetContextMenuEntries()
		{
			try
			{ return AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries; }
			catch { return Array.Empty<IContextMenuEntryExport>(); }
		}

		/// <summary>
		/// Replaces the active context-menu entries. Tests bypass the MEF registry by
		/// calling this directly with stub entries.
		/// </summary>
		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			Editor.TextArea.TextView.ContextMenu = menu;
		}

		void OnTextViewPointerPressedForContextMenu(object? sender, PointerPressedEventArgs e)
		{
			if (!e.GetCurrentPoint(Editor.TextArea.TextView).Properties.IsRightButtonPressed)
				return;
			var pos = GetPositionFromPointer(e);
			if (pos == null || DataContext is not DecompilerTabPageModel model || model.References == null)
			{
				lastRightClickedSegment = null;
				return;
			}
			var offset = Editor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
			lastRightClickedSegment = model.References.FindSegmentsContaining(offset).FirstOrDefault();
		}

		void OnContextMenuOpening(object? sender, CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			var ctx = new TextViewContext {
				TextView = this,
				Reference = lastRightClickedSegment,
			};
			menu.Items.Clear();

			// Copy / Select All come from MEF-exported entries in EditorCommands.cs (Category =
			// "Editor", Order 100/110 so they appear at the top). The remaining
			// MEF-discovered entries (Show in metadata, etc.) follow.
			var built = ContextMenuProvider.Build(contextMenuEntries, ctx);
			if (built == null)
			{
				e.Cancel = true;
				return;
			}
			foreach (var item in built.Items.Cast<Control>().ToArray())
			{
				built.Items.Remove(item);
				menu.Items.Add(item);
			}
		}

		protected override void OnDataContextChanged(System.EventArgs e)
		{
			base.OnDataContextChanged(e);

			// Detach the outgoing model's handlers before binding to the new one. Avalonia doesn't
			// surface the previous DataContext to this event, so we track it ourselves.
			if (boundModel is { } previous && !ReferenceEquals(previous, DataContext))
			{
				previous.PropertyChanged -= OnModelPropertyChanged;
				previous.CaptureViewState = null;
			}

			boundModel = DataContext as DecompilerTabPageModel;
			if (boundModel is { } model)
			{
				model.PropertyChanged += OnModelPropertyChanged;
				// Let DockWorkspace pull the live view state from this editor on demand when it
				// records a navigation away. (Re)assigning every DataContext-change handles both
				// the first attach and an ABA reattach.
				model.CaptureViewState = GetCurrentViewState;
				ApplyDocument(model);
				// Point the breadcrumb at this tab's node (the bar owns its own VM, so feed it the
				// node rather than letting it inherit the document DataContext).
				Omnibar.SetNode(model.CurrentNode);
			}
			else
			{
				Omnibar.SetNode(null);
			}
		}

		// Reads the editor's live caret + scroll + expanded-foldings state. Invoked by
		// DockWorkspace via DecompilerTabPageModel.CaptureViewState when it writes a navigation
		// record, so the captured position reflects exactly what the user is looking at now.
		DecompilerTextViewState GetCurrentViewState() => new(
			Editor.TextArea.Caret.Offset,
			Editor.VerticalOffset,
			Editor.HorizontalOffset,
			activeFoldingManager is { } manager ? FoldingsViewState.Capture(manager.AllFoldings) : null);

		// The editor's template ScrollViewer (cached once found). ?? re-evaluates while null so it
		// resolves once the template is applied.
		ScrollViewer? EditorScrollViewer =>
			editorScrollViewer ??= Editor.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();


		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Only rebuild when Text changes — DecompileAsync sets HighlightingModel and Foldings
			// just before Text, so reading them at this point gets the matching state. Reacting
			// to each property fires several intermediate ApplyDocuments where the text and the
			// folding offsets disagree.
			if (sender is DecompilerTabPageModel model
				&& (e.PropertyName == nameof(DecompilerTabPageModel.Text)
					|| e.PropertyName == nameof(DecompilerTabPageModel.SyntaxExtension)))
			{
				// Restore caret/scroll/foldings only on the Text change (the final content);
				// SyntaxExtension fires an intermediate rebuild that must not move the scroll.
				ApplyDocument(model, restoreViewState: e.PropertyName == nameof(DecompilerTabPageModel.Text));
			}
			// Keep the breadcrumb in step when the tab re-targets a different node (the model
			// raises this when CurrentNodes changes).
			else if (sender is DecompilerTabPageModel nodeModel
				&& e.PropertyName == nameof(DecompilerTabPageModel.CurrentNode))
			{
				Omnibar.SetNode(nodeModel.CurrentNode);
			}
			// HighlightedReference can change independently of a re-decompile (e.g. the analyzer
			// pane sets it while the user is already on the right tab). Apply it directly without
			// rebuilding the document.
			else if (sender is DecompilerTabPageModel m
				&& e.PropertyName == nameof(DecompilerTabPageModel.HighlightedReference))
			{
				ApplyHighlightedReference(m);
			}
		}

		void ApplyHighlightedReference(DecompilerTabPageModel model)
		{
			if (model.HighlightedReference == null)
			{
				ClearLocalReferenceMarks();
				return;
			}
			HighlightLocalReferences(model, model.HighlightedReference);
		}

		// restoreViewState: only the Text change carries the final decompiled content, so only it
		// should consume PendingViewState and restore (or reset) caret/scroll/foldings. The
		// intermediate SyntaxExtension change rebuilds the display but must NOT touch the scroll,
		// or it resets to the top before the Text pass restores -- a visible flicker on Back.
		void ApplyDocument(DecompilerTabPageModel model, bool restoreViewState = true)
		{
			// A new decompile invalidates any tooltip resolved against the previous document —
			// force-close even if the popup currently wants to stay (mouseClick: true).
			TryCloseExistingPopup(mouseClick: true);
			ClearLocalReferenceMarks();
			Editor.SyntaxHighlighting = HighlightingService.GetByExtension(model.SyntaxExtension);
			Editor.Document.Text = model.Text;

			// Swap the semantic-highlighting colorizer for this document's model (null clears it).
			SetColorizer(model.HighlightingModel);

			// Consume the pending view state up-front so it can't bleed into a later refresh even
			// when the new document has zero foldings to apply it to (e.g. a namespace summary page
			// that follows a method-body navigation). PendingViewState is set by Back/Forward/GoTo
			// navigation; null on a fresh navigation, in which case the caret/scroll reset below
			// puts the new document at the top.
			var pendingState = restoreViewState ? model.PendingViewState : null;
			if (restoreViewState)
				model.PendingViewState = null;

			RebuildFoldings(model, pendingState);

			referenceElementGenerator.References = model.References;
			uiElementGenerator.UIElements = model.UIElements;

			// Re-apply any pending reference highlight against the fresh References collection.
			// The analyzer-result-activation path sets HighlightedReference before the navigated
			// node's decompile finishes; this is where the marks finally land on the new text.
			if (model.HighlightedReference != null)
				HighlightLocalReferences(model, model.HighlightedReference);

			if (restoreViewState)
				RestoreOrResetViewState(pendingState);

			SwapCustomElementGenerators(model.CustomElementGenerators);

			Editor.TextArea.TextView.Redraw();
		}

		// Folding markers in the gutter: uninstall the previous manager, then install fresh foldings
		// from the model -- clamped to the document, ordered, with the pending snapshot's open/closed
		// state projected on -- or fall back to the XML folding strategy for XML resources. Installing
		// lazily keeps editors with no foldings chrome-free.
		void RebuildFoldings(DecompilerTabPageModel model, DecompilerTextViewState? pendingState)
		{
			if (activeFoldingManager != null)
			{
				FoldingManager.Uninstall(activeFoldingManager);
				activeFoldingManager = null;
			}
			if (model.Foldings is { Count: > 0 } foldings)
			{
				// Defensive clamp: drop foldings that fall outside [0, TextLength]. Without this, a
				// stale Foldings collection from an earlier decompile combined with a shorter (or
				// empty) document -- see DecompilerTabPageModel.DecompileAsync's empty-nodes
				// short-circuit -- would trip AvaloniaEdit's "Folding must be within document
				// boundary" guard inside CreateFolding.
				int textLength = Editor.Document.TextLength;
				var ordered = foldings
					.Where(f => f.StartOffset >= 0 && f.EndOffset <= textLength && f.StartOffset < f.EndOffset)
					.OrderBy(f => f.StartOffset)
					.ToList();
				if (ordered.Count > 0)
				{
					activeFoldingManager = FoldingManager.Install(Editor.TextArea);
					// Project the saved snapshot onto the freshly-built list so each NewFolding's
					// DefaultClosed reflects the previous state BEFORE UpdateFoldings installs them.
					// The checksum inside Restore guards against the document having shifted under our
					// feet (Refresh / settings change / etc).
					if (pendingState?.Foldings is { } pending)
						FoldingsViewState.Restore(ordered, pending);
					activeFoldingManager.UpdateFoldings(ordered, -1);
				}
			}
			else if (model.SyntaxExtension == ".xml" && !string.IsNullOrEmpty(model.Text))
			{
				// XML resources don't go through the C# decompiler, so the model has no
				// pre-collected foldings. AvaloniaEdit ships XmlFoldingStrategy that walks the
				// document once and emits one fold per element body.
				activeFoldingManager = FoldingManager.Install(Editor.TextArea);
				new XmlFoldingStrategy().UpdateFoldings(activeFoldingManager, Editor.Document);
			}
		}

		// Restore caret + scroll for Back/Forward/GoTo (pendingState carries the position captured
		// when the user left this node), or reset to the top on a fresh navigation: the editor is
		// reused across navigations and AvaloniaEdit keeps the previous document's caret/scroll when
		// the text is replaced. Caret is clamped to the new document length (the text may have changed
		// since capture, e.g. Refresh). AvaloniaEdit's ScrollTo*Offset are no-ops in 12.0.0
		// (AvaloniaUI/AvaloniaEdit#594), so set the ScrollViewer's Offset directly; Avalonia re-coerces
		// it against the scroll extent on the next layout, so it sticks before the document is measured.
		void RestoreOrResetViewState(DecompilerTextViewState? pendingState)
		{
			if (pendingState is { } state)
			{
				Editor.TextArea.Caret.Offset = Math.Clamp(state.CaretOffset, 0, Editor.Document.TextLength);
				if (EditorScrollViewer is { } scrollViewer)
					scrollViewer.Offset = new Vector(state.HorizontalOffset, state.VerticalOffset);
			}
			else
			{
				Editor.TextArea.Caret.Offset = 0;
				if (EditorScrollViewer is { } scrollViewer)
					scrollViewer.Offset = default;
			}
		}

		// Swap any tab-contributed visual-line element generators (e.g. About-page hyperlink
		// generators). Tracked separately from the always-on referenceElementGenerator and
		// uiElementGenerator so the two sets don't collide on tab change.
		void SwapCustomElementGenerators(IReadOnlyList<AvaloniaEdit.Rendering.VisualLineElementGenerator>? modelGenerators)
		{
			var generators = Editor.TextArea.TextView.ElementGenerators;
			foreach (var g in activeCustomGenerators)
				generators.Remove(g);
			activeCustomGenerators.Clear();
			if (modelGenerators is { Count: > 0 })
			{
				foreach (var g in modelGenerators)
				{
					generators.Add(g);
					activeCustomGenerators.Add(g);
				}
			}
		}

		void OnHyperlinkOpenUri(object? sender, AvaloniaEdit.Rendering.OpenUriRoutedEventArgs e)
		{
			if (DataContext is DecompilerTabPageModel model && model.RaiseOpenUriRequested(e.Uri))
				e.Handled = true;
		}

		ReferenceSegment? GetReferenceSegmentAtPointer(PointerEventArgs e)
		{
			if (DataContext is not DecompilerTabPageModel model || model.References == null)
				return null;
			var pos = GetPositionFromPointer(e);
			if (pos == null)
				return null;
			var offset = Editor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
			return model.References.FindSegmentsContaining(offset).FirstOrDefault();
		}

		internal void OnReferenceClicked(ReferenceSegment segment)
		{
			if (DataContext is not DecompilerTabPageModel model || segment.Reference == null)
				return;

			// Local references stay inside this document — paint every match and let the user
			// scrub through them. Cross-document references clear any existing marks since the
			// view is about to refresh anyway.
			if (segment.IsLocal)
			{
				HighlightLocalReferences(model, segment.Reference);
				return;
			}
			ClearLocalReferenceMarks();

			// In-document jumps win when the definition is in this same view.
			if (model.DefinitionLookup is { } lookup)
			{
				var definitionPos = lookup.GetDefinitionPosition(segment.Reference);
				if (definitionPos >= 0)
				{
					Editor.TextArea.Caret.Offset = definitionPos;
					Editor.TextArea.Caret.BringCaretToView();
					// Brief animated rectangle around the new caret position so the user spots
					// where the jump landed — particularly useful for long methods where the
					// scroll jump alone isn't enough to anchor attention.
					CaretHighlightAdorner.DisplayCaretHighlightAnimation(Editor.TextArea);
					Dispatcher.UIThread.Post(() => Editor.TextArea.Focus());
					return;
				}
			}

			// Otherwise let the host route the reference (assembly-tree navigation, new tab, ...).
			model.RaiseNavigateRequested(segment);
		}

		void HighlightLocalReferences(DecompilerTabPageModel model, object reference)
		{
			ClearLocalReferenceMarks();
			if (model.References == null)
				return;
			foreach (var r in model.References)
			{
				if (!ReferenceEquals(reference, r.Reference) && !reference.Equals(r.Reference))
					continue;
				var mark = textMarkerService.Create(r.StartOffset, r.Length);
				mark.BackgroundColor = r.IsDefinition ? LocalDefinitionBackground : LocalMatchBackground;
				localReferenceMarks.Add(mark);
			}
		}

		void ClearLocalReferenceMarks()
		{
			foreach (var mark in localReferenceMarks)
				textMarkerService.Remove(mark);
			localReferenceMarks.Clear();
		}

		// Live cursor + full ScrollOffset, queried at hover-event time. Returns null if the
		// pointer is past the end of the line so we don't snap back to the last visual
		// column when the user is hovering empty trailing space.
		AvaloniaEdit.TextViewPosition? GetPositionFromPointer(PointerEventArgs e)
		{
			var textView = Editor.TextArea.TextView;
			var pos = textView.GetPosition(e.GetPosition(textView) + textView.ScrollOffset);
			if (pos == null)
				return null;
			var lineLength = Editor.Document.GetLineByNumber(pos.Value.Line).Length + 1;
			if (pos.Value.Column >= lineLength)
				return null;
			return pos;
		}

		void OnTextViewPointerHover(object? sender, PointerEventArgs e)
		{
			// The existing popup gets a veto (it can refuse to be replaced if it's
			// interacting with the user), then we resolve a fresh segment under the live
			// pointer.
			if (!TryCloseExistingPopup(mouseClick: false))
				return;
			if (DataContext is not DecompilerTabPageModel model)
				return;
			var segment = GetReferenceSegmentAtPointer(e);
			if (segment?.Reference == null)
				return;

			var content = BuildHoverContent(model, segment);
			if (content == null)
				return;
			if (content.Value.IsRich)
				OpenRichPopup(content.Value.Control);
			else
				OpenPlainTooltip(content.Value.Control);
		}

		// Closes any open tooltip / rich popup before a new hover takes over. Returns false
		// when a rich popup wants to stay open — at which point the caller should bail
		// rather than steal it. <paramref name="mouseClick"/> forces the close even if the
		// popup is interacting with the user (used on click, on document change, etc.).
		bool TryCloseExistingPopup(bool mouseClick)
		{
			if (richPopup.IsOpen)
			{
				// While the pointer is over the popup or keyboard focus is inside it (the
				// user is reading, selecting text to copy, or reaching for a link), only a
				// forced close (document change) may take it down.
				if (!mouseClick && richPopup.Child is { } child
					&& (child.IsPointerOver || child.IsKeyboardFocusWithin))
				{
					return false;
				}
				CloseRichPopup();
				return true;
			}
			CloseTooltip();
			return true;
		}

		void OnTextViewPointerHoverStopped(object? sender, PointerEventArgs e)
		{
			// Plain tooltips close on hover stop. The rich popup's lifetime is governed by
			// the distance corridor on PointerMoved.
			if (!richPopup.IsOpen)
				CloseTooltip();
		}

		void OnTextViewPointerMoved(object? sender, PointerEventArgs e)
		{
			if (!richPopup.IsOpen)
				return;
			// While the rich popup is open, PointerMoved drives the WPF "distance corridor" —
			// the popup stays as long as the pointer doesn't move further from its edges than
			// the shrinking limit allows.
			double distance = GetDistanceToPopup(e);
			if (distance > distanceToPopupLimit)
				TryCloseExistingPopup(mouseClick: false);
			else
				distanceToPopupLimit = Math.Min(distanceToPopupLimit, distance + MaxMovementAwayFromPopup);
		}

		void OnTextViewPointerExited(object? sender, PointerEventArgs e)
		{
			// Don't close the rich popup if the pointer just moved from the editor onto the
			// popup itself — the user is reaching for it. The overlay popup delivers the
			// editor's exit BEFORE the popup child's IsPointerOver flips, so the flag alone
			// would close the popup right under the pointer; the distance check covers that
			// ordering. Plain tooltips always close.
			if (richPopup.IsOpen
				&& (richPopup.Child is { IsPointerOver: true } || GetDistanceToPopup(e) <= MaxMovementAwayFromPopup))
			{
				return;
			}
			TryCloseExistingPopup(mouseClick: false);
		}

		// Open* both assume the caller has already closed any existing popup / tooltip via
		// TryCloseExistingPopup — so they don't bother defending against a leftover popup.

		void OpenPlainTooltip(Control content)
		{
			ToolTip.SetTip(Editor.TextArea.TextView, content);
			ToolTip.SetIsOpen(Editor.TextArea.TextView, true);
		}

		internal void OpenRichPopup(Control content)
		{
			richPopup.Child = content;
			// While the pointer is over the popup, the editor no longer receives the moves
			// that drive the distance corridor — the popup's lifetime is governed by leaving
			// its own content instead.
			content.PointerExited += OnRichPopupContentPointerExited;
			richPopup.Open();
			distanceToPopupLimit = double.PositiveInfinity;
		}

		void OnRichPopupContentPointerExited(object? sender, PointerEventArgs e)
		{
			// A selection drag that sweeps past the popup edge keeps the pointer captured
			// inside the popup — that's a wide swipe, not a leave. The over/focus vetoes in
			// TryCloseExistingPopup don't see the capture, so check it here.
			if (e.Pointer.Captured is Visual captured && richPopup.Child is { } child
				&& (ReferenceEquals(captured, child) || captured.GetVisualAncestors().Contains(child)))
			{
				return;
			}
			TryCloseExistingPopup(mouseClick: false);
		}

		double GetDistanceToPopup(PointerEventArgs e)
		{
			if (richPopup.Child is not Control child)
				return 0;
			try
			{
				var screen = Editor.TextArea.TextView.PointToScreen(e.GetPosition(Editor.TextArea.TextView));
				var local = child.PointToClient(screen);
				var size = child.Bounds.Size;
				double x = local.X < 0 ? -local.X : (local.X > size.Width ? local.X - size.Width : 0);
				double y = local.Y < 0 ? -local.Y : (local.Y > size.Height ? local.Y - size.Height : 0);
				return Math.Sqrt(x * x + y * y);
			}
			catch (ArgumentException)
			{
				// PointToClient throws if the popup child isn't connected — treat as far away.
				return double.PositiveInfinity;
			}
		}

		void CloseTooltip()
		{
			ToolTip.SetIsOpen(Editor.TextArea.TextView, false);
		}

		void CloseRichPopup()
		{
			if (richPopup.IsOpen)
			{
				if (richPopup.Child is { } child)
					child.PointerExited -= OnRichPopupContentPointerExited;
				richPopup.Close();
			}
		}

		void OnRichPopupClosed(object? sender, EventArgs e)
		{
			distanceToPopupLimit = double.PositiveInfinity;
		}

		internal HoverContent? BuildHoverContent(DecompilerTabPageModel model, ReferenceSegment segment)
		{
			var language = model.Language;
			var resolved = ResolveEntity(model, segment.Reference);
			switch (resolved)
			{
				case IEntity entity when language != null:
					var rich = language.GetRichTextTooltip(entity);
					if (rich == null || string.IsNullOrEmpty(rich.Text))
						return null;
					var renderer = CreateTooltipRenderer();
					renderer.AddSignatureBlock(rich);
					AppendXmlDocumentation(renderer, entity);
					return new HoverContent(renderer.CreateView(), IsRich: true);
				case OpCodeInfo op:
					// Opcode docs are published as field documentation on
					// System.Reflection.Emit.OpCodes, so the mscorlib provider serves them.
					var opDocs = XmlDocLoader.MscorlibDocumentation
						?.GetDocumentation("F:System.Reflection.Emit.OpCodes." + op.EncodedName);
					if (opDocs != null)
					{
						var opRenderer = CreateTooltipRenderer();
						opRenderer.AddSignatureBlock(new RichText($"{op.Name} (0x{op.Code:x})"));
						opRenderer.AddXmlDocumentation(opDocs, declaringEntity: null, resolver: ResolveDocReference);
						return new HoverContent(opRenderer.CreateView(), IsRich: true);
					}
					var opBlock = new SelectableTextBlock {
						FontFamily = new FontFamily("Consolas, Menlo, Monospace"),
						FontSize = 12,
						MaxWidth = 600,
						TextWrapping = TextWrapping.Wrap,
						Inlines = new InlineCollection { new Run($"{op.Name} (0x{op.Code:x})") },
					};
					return new HoverContent(opBlock, IsRich: false);
				default:
					return null;
			}
		}

		// Hover tooltips share one renderer setup: monospace signature font, and any followed
		// documentation link closes the popup so the navigation is visible underneath.
		DocumentationRenderer CreateTooltipRenderer()
		{
			var renderer = new DocumentationRenderer(
				new CSharpAmbience(),
				new FontFamily("Consolas, Menlo, Monospace"),
				12);
			renderer.LinkClicked += (_, _) => CloseRichPopup();
			return renderer;
		}

		void AppendXmlDocumentation(DocumentationRenderer renderer, IEntity entity)
		{
			try
			{
				if (entity.ParentModule?.MetadataFile is not { } metadata)
					return;
				// XmlDocLoader handles every layout the decompiler library knows about: .xml
				// beside the .dll, .NET Framework reference-assemblies paths, and (recently)
				// the modern .NET ref pack at dotnet/packs/Microsoft.NETCore.App.Ref/...
				var documentation = XmlDocLoader.LoadDocumentation(metadata)?.GetDocumentation(entity.GetIdString());
				if (documentation == null)
					return;
				renderer.AddXmlDocumentation(documentation, entity, resolver: ResolveDocReference);
			}
			catch (XmlException)
			{
				// Malformed .xml — render signature only.
			}
		}

		/// <summary>
		/// Resolves a doc-comment cref id string (e.g. <c>T:System.String</c>) against the
		/// assembly list the current tab decompiled from.
		/// </summary>
		IEntity? ResolveDocReference(string idString)
		{
			if (DataContext is not DecompilerTabPageModel model || model.CurrentNode is not { } node)
				return null;
			var list = node.AncestorsAndSelf().OfType<TreeNodes.AssemblyListTreeNode>().FirstOrDefault()?.AssemblyList;
			return list == null ? null : AssemblyTree.AssemblyTreeModel.FindEntityInRelevantAssemblies(idString, list.GetAssemblies());
		}

		object? ResolveEntity(DecompilerTabPageModel model, object? reference)
		{
			switch (reference)
			{
				case IEntity entity:
					return entity;
				case OpCodeInfo op:
					return op;
				case EntityReference unresolved:
					if (DataContext is not DecompilerTabPageModel m || m.CurrentNode is not { } node)
						return null;
					var list = node.AncestorsAndSelf().OfType<TreeNodes.AssemblyListTreeNode>().FirstOrDefault()?.AssemblyList;
					return list == null ? null : unresolved.Resolve(list);
				default:
					return null;
			}
		}
	}
}
