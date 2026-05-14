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
			// Enter as the keyboard equivalent of double-tap so users navigating the list
			// with arrow keys can activate without reaching for the mouse.
			if (e.Key != Key.Enter)
				return;
			if (SearchResults.SelectedItem is SearchResult result && DataContext is SearchPaneModel vm)
			{
				vm.Activate(result);
				e.Handled = true;
			}
		}
	}
}
