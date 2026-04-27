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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;

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
	}
}
