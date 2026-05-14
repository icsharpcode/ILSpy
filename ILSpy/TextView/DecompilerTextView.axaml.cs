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

using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ILSpy;
using ILSpy.AppEnv;
using ILSpy.Options;

namespace ILSpy.TextView
{
	public partial class DecompilerTextView : UserControl
	{
		// Two-track output of BuildHoverContent: rich content opens the sticky Popup with the
		// pointer-distance corridor; plain content uses Avalonia's ToolTip attached property.
		readonly record struct HoverContent(Control Control, bool IsRich);

		// Local-reference highlight colours: light-yellow "GreenYellow" for matches and a
		// softer green for the actual definition.
		static readonly Color LocalMatchBackground = Colors.GreenYellow;
		static readonly Color LocalDefinitionBackground = Color.FromArgb(0x80, 0xA0, 0xFF, 0xA0);

		// Stay-open corridor for the rich popup: as long as the pointer is closer than
		// `distanceToPopupLimit` to the popup edges, the popup stays. The limit shrinks toward
		// the pointer's last best distance (bounded by MaxMovementAwayFromPopup) so the user
		// has room to "reach for" the popup but can't drift away from it.
		const double MaxMovementAwayFromPopup = 5;

		readonly ReferenceElementGenerator referenceElementGenerator;
		readonly UIElementGenerator uiElementGenerator;
		readonly TextMarkerService textMarkerService;
		readonly BracketHighlightRenderer bracketHighlightRenderer;
		readonly List<TextMarker> localReferenceMarks = new();
		readonly List<AvaloniaEdit.Rendering.VisualLineElementGenerator> activeCustomGenerators = new();
		RichTextColorizer? activeColorizer;
		FoldingManager? activeFoldingManager;
		DisplaySettings? currentDisplaySettings;
		ReferenceSegment? lastTooltipSegment;
		ReferenceSegment? lastRightClickedSegment;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();
		readonly Popup richPopup;
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

			// One generator lives for the lifetime of the view; we only swap its References
			// collection per document. The predicate filters out anything that should not be
			// clickable — references with a null target slip through unconditionally otherwise.
			referenceElementGenerator = new ReferenceElementGenerator(static segment => segment.Reference != null);
			referenceElementGenerator.ReferenceClicked += OnReferenceClicked;
			Editor.TextArea.TextView.ElementGenerators.Add(referenceElementGenerator);

			uiElementGenerator = new UIElementGenerator();
			Editor.TextArea.TextView.ElementGenerators.Add(uiElementGenerator);

			// Background renderer that paints local-reference highlights. Lives once for the
			// lifetime of the view; the marks themselves are cleared and rebuilt per click.
			textMarkerService = new TextMarkerService(Editor.TextArea.TextView);
			Editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);

			// Bracket-pair highlight: also a BackgroundRenderer; paints a soft outline
			// around the (.../[.../{... matching its pair when the caret sits next to one.
			// SetHighlight runs on every caret-position-changed event below.
			bracketHighlightRenderer = new BracketHighlightRenderer(Editor.TextArea.TextView);

			// Subscribe to AvaloniaEdit's built-in PointerHover routed event (fires after the
			// cursor settles inside a small box for ~400 ms) and resolve the segment fresh at
			// hover-time using the live pointer position. PointerMoved still drives the rich
			// popup's distance corridor.
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

			// Context menu — captures the segment under the pointer at right-click time so
			// entries like "Show in metadata" can dispatch through the same Reference field
			// the assembly-tree's right-click already populates.
			Editor.TextArea.TextView.AddHandler(InputElement.PointerPressedEvent,
				OnTextViewPointerPressedForContextMenu,
				RoutingStrategies.Tunnel,
				handledEventsToo: true);
			AttachContextMenu(TryGetContextMenuEntries());

			// AvaloniaEdit's hyperlinks raise OpenUriEvent (bubbling); the default class handler
			// on Window passes the URI to Process.Start. Intercept first so internal schemes
			// (the About page's "resource:" URIs) route through the tab model instead.
			Editor.TextArea.TextView.AddHandler(
				AvaloniaEdit.Rendering.VisualLineLinkText.OpenUriEvent,
				OnHyperlinkOpenUri,
				RoutingStrategies.Bubble,
				handledEventsToo: false);

			richPopup = new Popup {
				PlacementTarget = Editor.TextArea.TextView,
				// Pointer placement: Avalonia anchors the popup at the current cursor location
				// when Open() is called, with a small offset below to avoid covering the token.
				Placement = PlacementMode.Pointer,
				HorizontalOffset = 0,
				VerticalOffset = 16,
				IsLightDismissEnabled = false,
			};
			richPopup.Closed += OnRichPopupClosed;
			// Popup.Open uses PlacementTarget to find its TopLevel, so the popup itself doesn't
			// need to be in any visual tree of ours.

			WireUpDisplaySettings();

			// Push caret + scroll positions onto the bound tab model on every change so
			// DockWorkspace can read the live state when recording a navigation away. Cheap
			// — both events fire only on actual user motion and the model write is two
			// integer / double assignments.
			Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			Editor.TextArea.TextView.ScrollOffsetChanged += OnScrollOffsetChanged;

			// Ctrl+Wheel zoom (matches WPF's ZoomScrollViewer.OnPreviewMouseWheel). Tunnel
			// routing so we see it before AvaloniaEdit's scroll handler; Handled=true on
			// hit suppresses the scroll. Ctrl+0/Plus/Minus are Avalonia-side extras —
			// keyboard zoom is a standard modern-editor expectation that WPF ILSpy doesn't
			// happen to ship.
			Editor.AddHandler(InputElement.PointerWheelChangedEvent,
				OnEditorPointerWheelChanged,
				RoutingStrategies.Tunnel,
				handledEventsToo: false);
			Editor.KeyDown += OnEditorKeyDownForZoom;
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

		void OnEditorKeyDownForZoom(object? sender, KeyEventArgs e)
		{
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
			if (DataContext is DecompilerTabPageModel m)
				m.LastKnownCaretOffset = Editor.TextArea.Caret.Offset;
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

		void OnScrollOffsetChanged(object? sender, EventArgs e)
		{
			if (DataContext is DecompilerTabPageModel m)
			{
				m.LastKnownVerticalOffset = Editor.VerticalOffset;
				m.LastKnownHorizontalOffset = Editor.HorizontalOffset;
			}
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
			if (DataContext is DecompilerTabPageModel model)
			{
				model.PropertyChanged += OnModelPropertyChanged;
				// Foldings have no per-change event on AvaloniaEdit; instead DockWorkspace asks
				// the view for a fresh snapshot at navigate-away time. Assigning the delegate
				// every DataContext-change handles both the first attach and an ABA reattach.
				model.CaptureFoldingsState = () => SnapshotFoldingsInto(model);
				ApplyDocument(model);
			}
		}

		void SnapshotFoldingsInto(DecompilerTabPageModel model)
		{
			if (activeFoldingManager is { } manager)
				model.LastKnownFoldings = FoldingsViewState.Capture(manager.AllFoldings);
			else
				model.LastKnownFoldings = null;
		}

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
				ApplyDocument(model);
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

		void ApplyDocument(DecompilerTabPageModel model)
		{
			// A new decompile invalidates any tooltip resolved against the previous document —
			// force-close even if the popup currently wants to stay (mouseClick: true).
			TryCloseExistingPopup(mouseClick: true);
			ClearLocalReferenceMarks();
			Editor.SyntaxHighlighting = HighlightingService.GetByExtension(model.SyntaxExtension);
			Editor.Document.Text = model.Text;

			// Swap the semantic-highlighting colorizer. AvaloniaEdit only exposes Add/Remove on
			// LineTransformers, so we keep a reference to the previous one.
			var transformers = Editor.TextArea.TextView.LineTransformers;
			if (activeColorizer != null)
			{
				transformers.Remove(activeColorizer);
				activeColorizer = null;
			}
			if (model.HighlightingModel is { } richModel)
			{
				activeColorizer = new RichTextColorizer(richModel);
				transformers.Add(activeColorizer);
			}

			// Folding markers in the gutter: install lazily so editors with no foldings stay
			// chrome-free. UpdateFoldings expects offsets sorted ascending.
			if (activeFoldingManager != null)
			{
				FoldingManager.Uninstall(activeFoldingManager);
				activeFoldingManager = null;
			}
			// Consume the pending snapshot up-front so it can't bleed into a later refresh
			// even when the new document has zero foldings to apply it to (e.g. a namespace
			// summary page that follows a method-body navigation).
			var pendingFoldings = model.PendingFoldings;
			model.PendingFoldings = null;
			if (model.Foldings is { Count: > 0 } foldings)
			{
				activeFoldingManager = FoldingManager.Install(Editor.TextArea);
				var ordered = foldings.OrderBy(f => f.StartOffset).ToList();
				// Project the saved snapshot onto the freshly-built list so each NewFolding's
				// DefaultClosed reflects the previous state BEFORE UpdateFoldings installs them.
				// The checksum inside Restore guards against the document having shifted under
				// our feet (Refresh / settings change / etc).
				if (pendingFoldings is { } pending)
					FoldingsViewState.Restore(ordered, pending);
				activeFoldingManager.UpdateFoldings(ordered, -1);
			}
			else if (model.SyntaxExtension == ".xml" && !string.IsNullOrEmpty(model.Text))
			{
				// XML resources don't go through the C# decompiler, so the model has no
				// pre-collected foldings. AvaloniaEdit ships XmlFoldingStrategy that walks the
				// document once and emits one fold per element body.
				activeFoldingManager = FoldingManager.Install(Editor.TextArea);
				new XmlFoldingStrategy().UpdateFoldings(activeFoldingManager, Editor.Document);
			}

			referenceElementGenerator.References = model.References;
			uiElementGenerator.UIElements = model.UIElements;

			// Re-apply any pending reference highlight against the fresh References collection.
			// The analyzer-result-activation path sets HighlightedReference before the navigated
			// node's decompile finishes; this is where the marks finally land on the new text.
			if (model.HighlightedReference != null)
				HighlightLocalReferences(model, model.HighlightedReference);

			// Restore caret + scroll position captured when the user previously navigated
			// away from this node. Back/Forward (and the dropdown-history "GoTo") set the
			// Pending* fields just before triggering the decompile; here we read them once
			// and clear them so a subsequent user-initiated decompile doesn't jump the
			// cursor again. Clamp to the new document length — text-changed-since-capture
			// is the common case for Refresh.
			if (model.PendingCaretOffset is { } caret)
			{
				model.PendingCaretOffset = null;
				Editor.TextArea.Caret.Offset = Math.Clamp(caret, 0, Editor.Document.TextLength);
				Editor.TextArea.Caret.BringCaretToView();
			}
			if (model.PendingVerticalOffset is { } vy)
			{
				model.PendingVerticalOffset = null;
				Editor.ScrollToVerticalOffset(vy);
			}
			if (model.PendingHorizontalOffset is { } hx)
			{
				model.PendingHorizontalOffset = null;
				Editor.ScrollToHorizontalOffset(hx);
			}

			// Swap any tab-contributed visual-line element generators (e.g. About-page hyperlink
			// generators). Tracked separately from the always-on referenceElementGenerator and
			// uiElementGenerator so the two sets don't collide on tab change.
			var generators = Editor.TextArea.TextView.ElementGenerators;
			foreach (var g in activeCustomGenerators)
				generators.Remove(g);
			activeCustomGenerators.Clear();
			if (model.CustomElementGenerators is { Count: > 0 } modelGenerators)
			{
				foreach (var g in modelGenerators)
				{
					generators.Add(g);
					activeCustomGenerators.Add(g);
				}
			}

			Editor.TextArea.TextView.Redraw();

			Editor.ScrollToHome();
		}

		void OnHyperlinkOpenUri(object? sender, AvaloniaEdit.Rendering.OpenUriRoutedEventArgs e)
		{
			if (DataContext is DecompilerTabPageModel model && model.RaiseOpenUriRequested(e.Uri))
				e.Handled = true;
		}

		void OnReferenceClicked(ReferenceSegment segment)
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
			if (DataContext is not DecompilerTabPageModel model || model.References == null)
				return;
			var pos = GetPositionFromPointer(e);
			if (pos == null)
				return;
			var offset = Editor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
			var segment = model.References.FindSegmentsContaining(offset).FirstOrDefault();
			if (segment?.Reference == null)
				return;

			var content = BuildHoverContent(model, segment);
			if (content == null)
				return;
			lastTooltipSegment = segment;
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
				// TODO: refuse to close while the popup has keyboard focus (the user is
				// reading / clicking links inside it). Not tracked yet — for now the popup
				// is always replaceable.
				_ = mouseClick;
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
			// popup itself — the user is reaching for it. Plain tooltips always close.
			if (richPopup.IsOpen && richPopup.Child is { IsPointerOver: true })
				return;
			TryCloseExistingPopup(mouseClick: false);
		}

		// Open* both assume the caller has already closed any existing popup / tooltip via
		// TryCloseExistingPopup — so they don't bother defending against a leftover popup.

		void OpenPlainTooltip(Control content)
		{
			ToolTip.SetTip(Editor.TextArea.TextView, content);
			ToolTip.SetIsOpen(Editor.TextArea.TextView, true);
		}

		void OpenRichPopup(Control content)
		{
			richPopup.Child = content;
			richPopup.Open();
			distanceToPopupLimit = double.PositiveInfinity;
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
			lastTooltipSegment = null;
			ToolTip.SetIsOpen(Editor.TextArea.TextView, false);
		}

		void CloseRichPopup()
		{
			if (richPopup.IsOpen)
				richPopup.Close();
		}

		void OnRichPopupClosed(object? sender, EventArgs e)
		{
			lastTooltipSegment = null;
			distanceToPopupLimit = double.PositiveInfinity;
		}

		HoverContent? BuildHoverContent(DecompilerTabPageModel model, ReferenceSegment segment)
		{
			var language = model.Language;
			var resolved = ResolveEntity(model, segment.Reference);
			switch (resolved)
			{
				case IEntity entity when language != null:
					var rich = language.GetRichTextTooltip(entity);
					if (rich == null || string.IsNullOrEmpty(rich.Text))
						return null;
					var renderer = new DocumentationRenderer(
						new CSharpAmbience(),
						new FontFamily("Consolas, Menlo, Monospace"),
						12);
					renderer.AddSignatureBlock(rich);
					AppendXmlDocumentation(renderer, entity);
					return new HoverContent(renderer.CreateView(), IsRich: true);
				case OpCodeInfo op:
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

		static void AppendXmlDocumentation(DocumentationRenderer renderer, IEntity entity)
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
				// First-cut: no cref resolver, so <see cref="..."/> falls back to printing the
				// raw cref text. Wiring resolution against the visible assembly list is a follow-up.
				renderer.AddXmlDocumentation(documentation, entity, resolver: null);
			}
			catch (XmlException)
			{
				// Malformed .xml — render signature only.
			}
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
					if (list == null)
						return null;
					var resolvedModule = unresolved.ResolveAssembly(list);
					if (resolvedModule == null)
						return null;
					var token = MetadataTokenHelpers.TryAsEntityHandle(MetadataTokens.GetToken(unresolved.Handle));
					if (token == null)
						return null;
					var typeSystem = new DecompilerTypeSystem(resolvedModule, resolvedModule.GetAssemblyResolver(), TypeSystemOptions.Default | TypeSystemOptions.Uncached);
					return typeSystem.MainModule.ResolveEntity(token.Value);
				default:
					return null;
			}
		}
	}
}
