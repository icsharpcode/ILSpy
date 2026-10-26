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
using System.ComponentModel;
using System.Linq;

using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Documents;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace ICSharpCode.ILSpy.Options
{
	public partial class OptionsPageView : UserControl
	{
		static readonly AttachedProperty<string?> RenderedTextProperty =
			AvaloniaProperty.RegisterAttached<OptionsPageView, TextBlock, string?>("RenderedText");
		static readonly AttachedProperty<string?> RenderedSearchProperty =
			AvaloniaProperty.RegisterAttached<OptionsPageView, TextBlock, string?>("RenderedSearch");

		OptionsPageModel? model;
		bool highlightQueued;
		bool scrollToFirstHit;

		public OptionsPageView()
		{
			InitializeComponent();
			SearchBox.KeyDown += OnSearchBoxKeyDown;
			// A page the search just selected, and the Decompiler page's rebuilt item list,
			// only get their labels in a later layout pass; re-run the highlight after each
			// pass while a search is active so those labels are picked up too.
			PageTabs.LayoutUpdated += (_, _) => {
				if (model is { SearchText.Length: > 0 })
					ScheduleHighlight();
			};
		}

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			if (model != null)
				model.PropertyChanged -= OnModelPropertyChanged;
			model = DataContext as OptionsPageModel;
			if (model != null)
				model.PropertyChanged += OnModelPropertyChanged;
		}

		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Both a new search text and a page switch change which labels are on screen.
			if (e.PropertyName is nameof(OptionsPageModel.SearchText) or nameof(OptionsPageModel.SelectedPage))
			{
				scrollToFirstHit = true;
				ScheduleHighlight();
			}
		}

		/// <summary>Runs <see cref="HighlightHits"/> once at Background priority, after the
		/// pending layout pass, no matter how many triggers fire in between.</summary>
		void ScheduleHighlight()
		{
			if (highlightQueued)
				return;
			highlightQueued = true;
			Dispatcher.UIThread.Post(() => {
				highlightQueued = false;
				HighlightHits();
			}, DispatcherPriority.Background);
		}

		void HighlightHits()
		{
			var text = model?.SearchText ?? string.Empty;
			TextBlock? firstHit = null;
			// Walking the visual tree reaches CheckBox content, Expander and
			// HeaderedContentControl headers through their template TextBlocks, so no
			// per-control-type rules are needed.
			foreach (var label in PageTabs.GetVisualDescendants().OfType<TextBlock>())
			{
				var labelText = label.GetValue(RenderedTextProperty) ?? label.Text;
				bool hit = text.Length > 0 && OptionsPageModel.Contains(labelText, text);
				if (hit)
				{
					HighlightLabel(label, labelText!, text);
					firstHit ??= label;
				}
				else
				{
					label.Classes.Set("search-hit", false);
					if (label.Inlines != null)
						label.Inlines = null;
					label.ClearValue(RenderedTextProperty);
					label.ClearValue(RenderedSearchProperty);
				}
			}
			// Scroll once per change, not after every layout pass, or it would fight the
			// user's own scrolling.
			if (firstHit != null && scrollToFirstHit)
			{
				scrollToFirstHit = false;
				firstHit.BringIntoView();
			}
		}

		static void HighlightLabel(TextBlock label, string labelText, string searchText)
		{
			if (label.GetValue(RenderedSearchProperty) == searchText && label.Inlines != null)
				return;

			var inlines = new InlineCollection();
			int offset = 0;
			while (offset < labelText.Length)
			{
				int match = labelText.IndexOf(searchText, offset, StringComparison.CurrentCultureIgnoreCase);
				if (match < 0)
				{
					inlines.Add(new Run(labelText[offset..]));
					break;
				}
				if (match > offset)
					inlines.Add(new Run(labelText[offset..match]));
				var hit = new Run(labelText.Substring(match, searchText.Length));
				hit.Classes.Add("search-hit");
				inlines.Add(hit);
				offset = match + searchText.Length;
			}

			label.SetValue(RenderedTextProperty, labelText);
			label.SetValue(RenderedSearchProperty, searchText);
			label.Inlines = inlines;
			label.Classes.Set("search-hit", false);
		}

		void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape && model is { SearchText.Length: > 0 })
			{
				model.SearchText = string.Empty;
				e.Handled = true;
			}
		}

		void OnClearSearchClicked(object? sender, RoutedEventArgs e)
		{
			if (model != null)
				model.SearchText = string.Empty;
			SearchBox.Focus();
		}
	}
}
