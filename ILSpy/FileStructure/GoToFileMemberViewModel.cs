// Copyright (c) 2026 Piero Viano
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
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// The Go to File Member popup: the members of one document, narrowed by a case-insensitive
	/// substring filter as the user types. The first match is preselected so Enter accepts it.
	/// </summary>
	public sealed partial class GoToFileMemberViewModel : ObservableObject
	{
		readonly IReadOnlyList<FileMemberEntry> allEntries;

		[ObservableProperty]
		private string filterText = string.Empty;

		[ObservableProperty]
		private FileMemberEntry? selectedItem;

		public GoToFileMemberViewModel(DecompilerTabPageModel document)
		{
			Document = document ?? throw new ArgumentNullException(nameof(document));
			allEntries = FileMemberCollector.Collect(document);
			ApplyFilter();
		}

		public DecompilerTabPageModel Document { get; }

		/// <summary>The entries matching <see cref="FilterText"/>, in document order.</summary>
		public ObservableCollection<FileMemberEntry> Items { get; } = new();

		/// <summary>Raised when the popup should close: after a successful accept, or on cancel.</summary>
		public event EventHandler? CloseRequested;

		partial void OnFilterTextChanged(string value) => ApplyFilter();

		void ApplyFilter()
		{
			Items.Clear();
			foreach (var entry in allEntries)
			{
				if (FilterText.Length == 0 || entry.DisplayText.Contains(FilterText, StringComparison.OrdinalIgnoreCase))
					Items.Add(entry);
			}
			SelectedItem = Items.Count > 0 ? Items[0] : null;
		}

		/// <summary>Moves the selection by <paramref name="delta"/> rows, clamped to the list.</summary>
		public void MoveSelection(int delta)
		{
			if (Items.Count == 0)
				return;
			int index = SelectedItem == null ? -1 : Items.IndexOf(SelectedItem);
			SelectedItem = Items[Math.Clamp(index + delta, 0, Items.Count - 1)];
		}

		/// <summary>
		/// Navigates the document to the selected member and asks the popup to close. Returns false
		/// (and stays open) when nothing is selected or the document has no editor attached.
		/// </summary>
		public bool Accept()
		{
			if (SelectedItem is not { } entry || Document.NavigateToOffset is not { } navigate)
				return false;
			navigate(entry.Offset);
			CloseRequested?.Invoke(this, EventArgs.Empty);
			return true;
		}

		/// <summary>Closes the popup without navigating.</summary>
		public void Cancel() => CloseRequested?.Invoke(this, EventArgs.Empty);
	}
}
