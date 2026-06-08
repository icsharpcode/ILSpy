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

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using ICSharpCode.ILSpyX.Search;

namespace ILSpy.Search
{
	public partial class SearchPane : UserControl
	{
		SearchPaneModel? boundModel;

		public SearchPane()
		{
			InitializeComponent();
			SearchResults.DoubleTapped += OnResultDoubleTapped;
			SearchResults.KeyDown += OnResultKeyDown;
			SearchResults.AddHandler(PointerPressedEvent, OnResultsPointerPressed, RoutingStrategies.Tunnel);
			SearchResults.AddHandler(PointerReleasedEvent, OnResultsPointerReleased, RoutingStrategies.Tunnel);
			SearchInput.KeyDown += OnSearchInputKeyDown;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// Mode accelerators: Ctrl+T/M/S jump the picker to Type / Member / Constant. These
			// mirror the previous app's shortcuts and work from anywhere in the pane (the search
			// box doesn't consume bare Ctrl+letter chords, so the event bubbles up to here).
			if (e.Handled || e.KeyModifiers != KeyModifiers.Control)
				return;
			if (DataContext is not SearchPaneModel vm)
				return;
			switch (e.Key)
			{
				case Key.T:
					vm.SelectMode(SearchMode.Type);
					e.Handled = true;
					break;
				case Key.M:
					vm.SelectMode(SearchMode.Member);
					e.Handled = true;
					break;
				case Key.S:
					vm.SelectMode(SearchMode.Literal);
					e.Handled = true;
					break;
			}
		}

		void OnSearchInputKeyDown(object? sender, KeyEventArgs e)
		{
			// Down arrow drops into the result list, selecting the first row and moving
			// keyboard focus there so the user can keep arrowing through hits without the
			// mouse. No-op when there are no results to step into.
			if (e.Key == Key.Down)
			{
				if (DataContext is SearchPaneModel vm && vm.Results.Count > 0)
				{
					SearchResults.SelectedIndex = 0;
					SearchResults.Focus();
					e.Handled = true;
				}
				return;
			}

			// Escape mirrors the clear-X button. Goes through the bound view-model so the
			// orchestrator's cancel-and-restart path fires the same way it does when the
			// user backspaces the box empty by hand.
			if (e.Key != Key.Escape)
				return;
			if (DataContext is SearchPaneModel vmEsc && vmEsc.SearchTerm.Length > 0)
			{
				vmEsc.SearchTerm = string.Empty;
				e.Handled = true;
			}
		}

		void OnClearSearchClicked(object? sender, RoutedEventArgs e)
		{
			// Empty the query and return focus to the input so the user can start typing
			// straight away. Setting SearchTerm = "" triggers the same cancel-and-restart
			// path as deleting the text by hand.
			if (DataContext is SearchPaneModel vm)
				vm.SearchTerm = string.Empty;
			SearchInput.Focus();
			e.Handled = true;
		}

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			if (boundModel != null)
				boundModel.FocusRequested -= OnFocusRequested;
			boundModel = DataContext as SearchPaneModel;
			if (boundModel != null)
				boundModel.FocusRequested += OnFocusRequested;
		}

		void OnFocusRequested()
		{
			// Post the focus shift instead of running synchronously: ShowSearchCommand fires
			// in the middle of SetActiveDockable, when the pane may not yet be the focusable
			// visual root. A dispatcher tick lets the activation settle so .Focus() actually
			// takes — without it the focus call no-ops because the TextBox isn't visible yet.
			Dispatcher.UIThread.Post(() => SearchInput.Focus(), DispatcherPriority.Input);
		}

		void OnResultDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (SearchResults.SelectedItem is SearchResult result && DataContext is SearchPaneModel vm)
			{
				vm.Activate(result);
				e.Handled = true;
			}
		}

		void OnResultKeyDown(object? sender, KeyEventArgs e)
		{
			// Up from the top of the list hands focus back to the search box (and clears the
			// selection on the way out) so the box and the list feel like one continuous strip.
			if (e.Key == Key.Up && SearchResults.SelectedIndex == 0)
			{
				SearchResults.SelectedIndex = -1;
				SearchInput.Focus();
				e.Handled = true;
				return;
			}

			// Enter as the keyboard equivalent of double-tap so users navigating the list
			// with arrow keys can activate without reaching for the mouse. Ctrl+Enter opens
			// the result in a new tab instead of reusing the active one.
			if (e.Key != Key.Enter)
				return;
			if (SearchResults.SelectedItem is SearchResult result && DataContext is SearchPaneModel vm)
			{
				vm.Activate(result, e.KeyModifiers.HasFlag(KeyModifiers.Control));
				e.Handled = true;
			}
		}

		void OnResultsPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			// Middle-click doesn't select a row on its own, so claim the row under the cursor
			// here; the matching release then activates it in a new tab.
			if (!e.GetCurrentPoint(SearchResults).Properties.IsMiddleButtonPressed)
				return;
			if ((e.Source as Control)?.DataContext is SearchResult result)
				SearchResults.SelectedItem = result;
		}

		void OnResultsPointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			// Middle-click opens the result in a new tab, matching the tree's open-in-new-tab gesture.
			if (e.InitialPressMouseButton != MouseButton.Middle)
				return;
			if (SearchResults.SelectedItem is SearchResult result && DataContext is SearchPaneModel vm)
			{
				vm.Activate(result, inNewTabPage: true);
				e.Handled = true;
			}
		}
	}
}
