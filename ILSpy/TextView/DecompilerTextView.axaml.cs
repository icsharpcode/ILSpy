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

namespace ILSpy.TextView
{
	public partial class DecompilerTextView : UserControl
	{
		// Two-track output of BuildHoverContent: rich content opens the sticky Popup with the
		// WPF distance corridor; plain content uses Avalonia's ToolTip attached property.
		readonly record struct HoverContent(Control Control, bool IsRich);

		// Local-reference highlight colours, mirroring ILSpy/Themes/generic.xaml. Use the same
		// light-yellow "GreenYellow" for matches and a softer green for the actual definition.
		static readonly Color LocalMatchBackground = Colors.GreenYellow;
		static readonly Color LocalDefinitionBackground = Color.FromArgb(0x80, 0xA0, 0xFF, 0xA0);

		// Stay-open corridor for the rich popup: as long as the pointer is closer than
		// `distanceToPopupLimit` to the popup edges, the popup stays. The limit shrinks toward
		// the pointer's last best distance (bounded by MaxMovementAwayFromPopup) so the user
		// has room to "reach for" the popup but can't drift away from it.
		const double MaxMovementAwayFromPopup = 5;

		readonly ReferenceElementGenerator referenceElementGenerator;
		readonly TextMarkerService textMarkerService;
		readonly List<TextMarker> localReferenceMarks = new();
		RichTextColorizer? activeColorizer;
		FoldingManager? activeFoldingManager;
		ReferenceSegment? lastTooltipSegment;
		readonly Popup richPopup;
		double distanceToPopupLimit;

		public DecompilerTextView()
		{
			InitializeComponent();

			// One generator lives for the lifetime of the view; we only swap its References
			// collection per document. The predicate filters out anything that should not be
			// clickable — references with a null target slip through unconditionally otherwise.
			referenceElementGenerator = new ReferenceElementGenerator(static segment => segment.Reference != null);
			referenceElementGenerator.ReferenceClicked += OnReferenceClicked;
			Editor.TextArea.TextView.ElementGenerators.Add(referenceElementGenerator);

			// Background renderer that paints local-reference highlights. Lives once for the
			// lifetime of the view; the marks themselves are cleared and rebuilt per click.
			textMarkerService = new TextMarkerService(Editor.TextArea.TextView);
			Editor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);

			// Hover detection mirrors WPF ILSpy: subscribe to AvaloniaEdit's built-in
			// PointerHover routed event (fires after the cursor settles inside a small box for
			// ~400 ms) and resolve the segment fresh at hover-time using the live pointer
			// position. PointerMoved still drives the rich popup's distance corridor.
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
		}

		protected override void OnDataContextChanged(System.EventArgs e)
		{
			base.OnDataContextChanged(e);
			if (DataContext is DecompilerTabPageModel model)
			{
				model.PropertyChanged += OnModelPropertyChanged;
				ApplyDocument(model);
			}
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
			if (model.Foldings is { Count: > 0 } foldings)
			{
				activeFoldingManager = FoldingManager.Install(Editor.TextArea);
				activeFoldingManager.UpdateFoldings(foldings.OrderBy(f => f.StartOffset), -1);
			}

			referenceElementGenerator.References = model.References;
			Editor.TextArea.TextView.Redraw();

			Editor.ScrollToHome();
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

		// Mirror WPF DecompilerTextView.GetPositionFromMousePosition: live cursor + full
		// ScrollOffset, queried at hover-event time. Returns null if the pointer is past the
		// end of the line (so we don't snap back to the last visual column when the user is
		// hovering empty trailing space — matches WPF's behaviour exactly).
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
			// Mirrors WPF DecompilerTextView.TextViewMouseHover: the existing popup gets a veto
			// (it can refuse to be replaced if it's "interacting" with the user — keyboard focus
			// in WPF), then we resolve a fresh segment under the live pointer.
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

		// Mirrors WPF DecompilerTextView.TryCloseExistingPopup. Closes any open tooltip / rich
		// popup before a new hover takes over. Returns false when a rich popup wants to stay
		// open — at which point the caller should bail rather than steal it. <paramref name="mouseClick"/>
		// forces the close even if the popup is interacting with the user (used on click, on
		// document change, etc.).
		bool TryCloseExistingPopup(bool mouseClick)
		{
			if (richPopup.IsOpen)
			{
				// TODO: WPF refuses to close here when the FlowDocumentTooltip has keyboard
				// focus (the user is reading / clicking links inside it). We don't track
				// keyboard focus on the popup yet, so the popup is always replaceable.
				_ = mouseClick;
				CloseRichPopup();
				return true;
			}
			CloseTooltip();
			return true;
		}

		void OnTextViewPointerHoverStopped(object? sender, PointerEventArgs e)
		{
			// Plain tooltips close on hover stop. The rich popup's lifetime is governed by the
			// distance corridor on PointerMoved, mirroring WPF's TextEditorMouseMove.
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
				var docProvider = XmlDocLoader.LoadDocumentation(metadata);
				if (docProvider == null)
					return;
				var documentation = docProvider.GetDocumentation(entity.GetIdString());
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
