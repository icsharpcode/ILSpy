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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

using ICSharpCode.ILSpyX.Search;
using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.Omnibar
{
	/// <summary>
	/// The decompiler's address-bar: a breadcrumb of the current node that turns into a search box
	/// when the user types. Hosted at the top of the decompiled-content view; one instance per
	/// document tab, each owning its own <see cref="OmnibarViewModel"/>.
	/// </summary>
	public partial class Omnibar : UserControl
	{
		readonly OmnibarViewModel viewModel = new();

		public Omnibar()
		{
			// Per-instance VM (not inherited from the hosting document's DataContext) so each tab's
			// bar tracks its own node and search.
			DataContext = viewModel;
			InitializeComponent();

			viewModel.PropertyChanged += OnViewModelPropertyChanged;
			viewModel.Suggestions.CollectionChanged += OnSuggestionsChanged;

			SearchInput.KeyDown += OnSearchInputKeyDown;
			SuggestionsList.KeyDown += OnSuggestionsKeyDown;
			SuggestionsList.Tapped += OnSuggestionTapped;
			ChevronList.Tapped += OnChevronItemTapped;
			ChevronList.KeyDown += OnChevronKeyDown;
		}

		/// <summary>Points the breadcrumb at the document's current node.</summary>
		public void SetNode(SharpTreeNode? node) => viewModel.SetNode(node);

		/// <summary>Drops the bar into search mode and focuses the query box (Ctrl+L from the host).</summary>
		public void FocusSearch()
		{
			viewModel.EnterSearch();
			Dispatcher.UIThread.Post(() => {
				SearchInput.Focus();
				SearchInput.SelectAll();
			});
		}

		void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(OmnibarViewModel.Mode))
			{
				if (viewModel.Mode == OmnibarMode.Search)
				{
					Dispatcher.UIThread.Post(() => SearchInput.Focus());
				}
				else
				{
					SuggestionsPopup.IsOpen = false;
				}
			}
		}

		void OnSuggestionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			SuggestionsPopup.IsOpen = viewModel.Mode == OmnibarMode.Search && viewModel.Suggestions.Count > 0;
		}

		void OnSegmentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (sender is Control { Tag: BreadcrumbSegment segment })
				viewModel.NavigateSegment(segment);
		}

		void OnEnterSearchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
			=> FocusSearch();

		// Vertical scrolling is disabled on the breadcrumb, so the wheel is otherwise idle here;
		// map it to horizontal scroll so the trail can be panned without hunting for the thin
		// overlay thumb.
		void OnBreadcrumbWheel(object? sender, PointerWheelEventArgs e)
		{
			var max = BreadcrumbScroll.ScrollBarMaximum.X;
			if (max <= 0)
				return;
			// A wheel notch is one unit; crumbs are far wider, so scale up for a usable step.
			const double step = 48;
			var delta = (e.Delta.Y != 0 ? e.Delta.Y : e.Delta.X) * step;
			var x = Math.Clamp(BreadcrumbScroll.Offset.X - delta, 0, max);
			BreadcrumbScroll.Offset = BreadcrumbScroll.Offset.WithX(x);
			e.Handled = true;
		}

		void OnClearSearchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			viewModel.SearchText = string.Empty;
			SearchInput.Focus();
		}

		void OnSearchInputKeyDown(object? sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					viewModel.ExitSearch();
					e.Handled = true;
					break;
				case Key.Down when viewModel.Suggestions.Count > 0:
					SuggestionsPopup.IsOpen = true;
					SuggestionsList.SelectedIndex = 0;
					SuggestionsList.Focus();
					e.Handled = true;
					break;
				case Key.Enter:
					ActivateSuggestion(
						SuggestionsList.SelectedItem as SearchResult ?? viewModel.Suggestions.FirstOrDefault(),
						inNewTabPage: e.KeyModifiers.HasFlag(KeyModifiers.Control));
					e.Handled = true;
					break;
			}
		}

		void OnSuggestionsKeyDown(object? sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					ActivateSuggestion(SuggestionsList.SelectedItem as SearchResult,
						inNewTabPage: e.KeyModifiers.HasFlag(KeyModifiers.Control));
					e.Handled = true;
					break;
				case Key.Escape:
					viewModel.ExitSearch();
					e.Handled = true;
					break;
				case Key.Up when SuggestionsList.SelectedIndex <= 0:
					SearchInput.Focus();
					e.Handled = true;
					break;
			}
		}

		void OnSuggestionTapped(object? sender, TappedEventArgs e)
			=> ActivateSuggestion(SuggestionsList.SelectedItem as SearchResult, inNewTabPage: false);

		void ActivateSuggestion(SearchResult? result, bool inNewTabPage)
		{
			if (result == null)
				return;
			SuggestionsPopup.IsOpen = false;
			viewModel.Activate(result, inNewTabPage);
		}

		// The crumb whose children the chevron popup is currently showing; used to refilter as the
		// user types in the popup's filter box.
		System.Collections.Generic.IReadOnlyList<BreadcrumbSegment> chevronChildren
			= Array.Empty<BreadcrumbSegment>();

		void OnChevronClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (sender is not Control { Tag: BreadcrumbSegment segment } button)
				return;
			chevronChildren = viewModel.GetChildSegments(segment);
			ChevronFilter.Text = string.Empty;
			ChevronList.ItemsSource = chevronChildren;
			ChevronPopup.PlacementTarget = button;
			ChevronPopup.IsOpen = chevronChildren.Count > 0;
			if (ChevronPopup.IsOpen)
				Dispatcher.UIThread.Post(() => ChevronFilter.Focus());
		}

		void OnChevronFilterChanged(object? sender, TextChangedEventArgs e)
		{
			var filter = ChevronFilter.Text ?? string.Empty;
			ChevronList.ItemsSource = filter.Length == 0
				? chevronChildren
				: chevronChildren
					.Where(c => c.Text.Contains(filter, StringComparison.OrdinalIgnoreCase))
					.ToArray();
		}

		void OnChevronItemTapped(object? sender, TappedEventArgs e)
			=> NavigateToChevronSelection();

		void OnChevronKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				NavigateToChevronSelection();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				ChevronPopup.IsOpen = false;
				e.Handled = true;
			}
		}

		void NavigateToChevronSelection()
		{
			if (ChevronList.SelectedItem is not BreadcrumbSegment segment)
				return;
			ChevronPopup.IsOpen = false;
			viewModel.NavigateSegment(segment);
		}
	}
}
