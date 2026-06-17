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

using Avalonia.Controls;
using Avalonia.Input;

namespace ICSharpCode.ILSpy.Bookmarks
{
	public partial class BookmarksPane : UserControl
	{
		public BookmarksPane()
		{
			InitializeComponent();
			BookmarkGrid.DoubleTapped += OnRowDoubleTapped;
		}

		// Double-click anywhere on a row jumps to its location. The Name column stays editable
		// through the DataGrid's own cell editing (it does not start on a double-tap).
		void OnRowDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (DataContext is BookmarksPaneModel model && BookmarkGrid.SelectedItem is Bookmark bookmark)
			{
				_ = model.ActivateAsync(bookmark);
				e.Handled = true;
			}
		}
	}
}
