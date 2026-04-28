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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ILSpy.Languages;

namespace ILSpy.TextView
{
	public partial class DecompilerTextView : UserControl
	{
		// Local-reference highlight colours, mirroring ILSpy/Themes/generic.xaml. Use the same
		// light-yellow "GreenYellow" for matches and a softer green for the actual definition.
		static readonly Color LocalMatchBackground = Colors.GreenYellow;
		static readonly Color LocalDefinitionBackground = Color.FromArgb(0x80, 0xA0, 0xFF, 0xA0);

		readonly ReferenceElementGenerator referenceElementGenerator;
		readonly TextMarkerService textMarkerService;
		readonly List<TextMarker> localReferenceMarks = new();
		RichTextColorizer? activeColorizer;
		FoldingManager? activeFoldingManager;

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

			// Hover tooltip — driven by Avalonia's built-in ToolTip system (delay, dismissal,
			// positioning all handled for us). We just dynamically update ToolTip.Tip on the
			// inner TextView based on what's under the pointer. AvaloniaEdit's PointerHover is
			// routed; AddHandler with handledEventsToo=true ensures we still see events that
			// earlier handlers marked handled.
			ToolTip.SetPlacement(Editor.TextArea.TextView, PlacementMode.Pointer);
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

		void OnTextViewPointerHover(object? sender, PointerEventArgs e)
		{
			if (DataContext is not DecompilerTabPageModel model || model.References == null)
				return;
			var pos = Editor.TextArea.TextView.GetPosition(e.GetPosition(Editor.TextArea.TextView));
			if (pos == null)
				return;
			var offset = Editor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
			var segment = model.References.FindSegmentsContaining(offset).FirstOrDefault();
			if (segment?.Reference == null)
				return;

			var content = BuildHoverContent(model, segment);
			if (content == null)
				return;
			ToolTip.SetTip(Editor.TextArea.TextView, content);
			ToolTip.SetIsOpen(Editor.TextArea.TextView, true);
		}

		void OnTextViewPointerHoverStopped(object? sender, PointerEventArgs e)
		{
			ToolTip.SetIsOpen(Editor.TextArea.TextView, false);
			ToolTip.SetTip(Editor.TextArea.TextView, null);
		}

		Control? BuildHoverContent(DecompilerTabPageModel model, ReferenceSegment segment)
		{
			var language = model.Language;
			var resolved = ResolveEntity(model, segment.Reference);
			var block = new SelectableTextBlock {
				FontFamily = new FontFamily("Consolas, Menlo, Monospace"),
				FontSize = 12,
				MaxWidth = 600,
				TextWrapping = TextWrapping.Wrap,
				Inlines = new InlineCollection(),
			};
			switch (resolved)
			{
				case IEntity entity when language != null:
					var rich = language.GetRichTextTooltip(entity);
					if (rich == null || string.IsNullOrEmpty(rich.Text))
						return null;
					AppendRichText(block.Inlines, rich);
					break;
				case OpCodeInfo op:
					block.Inlines.Add(new Run($"{op.Name} (0x{op.Code:x})"));
					break;
				default:
					return null;
			}
			return block;
		}

		// Render a RichText (text + per-section HighlightingColor) into an Avalonia Inlines
		// collection — one Run per uniformly-coloured span. Mirrors the WPF RichText.ToTextBlock
		// path but uses Avalonia inlines instead of WPF Runs.
		static void AppendRichText(InlineCollection inlines, RichText rich)
		{
			foreach (var section in rich.GetHighlightedSections(0, rich.Length))
			{
				var run = new Run(rich.Text.Substring(section.Offset, section.Length));
				if (section.Color is { } color)
				{
					if (color.Foreground?.GetBrush(null) is { } fg)
						run.Foreground = fg;
					if (color.Background?.GetBrush(null) is { } bg)
						run.Background = bg;
					if (color.FontWeight is { } weight)
						run.FontWeight = weight;
					if (color.FontStyle is { } style)
						run.FontStyle = style;
				}
				inlines.Add(run);
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
