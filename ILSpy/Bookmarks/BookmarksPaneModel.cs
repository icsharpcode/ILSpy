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
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// The dockable Bookmarks pane: a flat, editable list of bookmarks plus a toolbar of bookmark
	/// actions. The list is the manager's live collection; navigation and per-document actions are
	/// delegated to the navigator and to the active decompiler tab.
	/// </summary>
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 1, IsVisibleByDefault = false)]
	[Shared]
	public partial class BookmarksPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "Bookmarks";

		readonly BookmarkManager manager;
		readonly BookmarkNavigator navigator;

		[ObservableProperty]
		private Bookmark? selectedBookmark;

		[ImportingConstructor]
		public BookmarksPaneModel(BookmarkManager manager, BookmarkNavigator navigator)
		{
			this.manager = manager;
			this.navigator = navigator;
			Id = PaneContentId;
			Title = Resources.Bookmarks;
		}

		/// <summary>The live bookmark list, bound by the grid.</summary>
		public ObservableCollection<Bookmark> Bookmarks => manager.Bookmarks;

		/// <summary>Double-click / Enter on a row: select it and jump to its location.</summary>
		public Task ActivateAsync(Bookmark bookmark)
		{
			SelectedBookmark = bookmark;
			return navigator.NavigateToAsync(bookmark);
		}

		[RelayCommand]
		void Toggle() => ActiveTab?.ToggleBookmarkAtCaret?.Invoke();

		[RelayCommand]
		void Next() => NavigateRelative(1);

		[RelayCommand]
		void Previous() => NavigateRelative(-1);

		[RelayCommand]
		void NextInFile() => ActiveTab?.NavigateBookmarkInFile?.Invoke(true);

		[RelayCommand]
		void PreviousInFile() => ActiveTab?.NavigateBookmarkInFile?.Invoke(false);

		[RelayCommand]
		void Delete()
		{
			if (SelectedBookmark is { } bookmark)
				manager.Remove(bookmark);
		}

		[RelayCommand]
		void Disable()
		{
			if (SelectedBookmark is { } bookmark)
				bookmark.Enabled = !bookmark.Enabled;
		}

		[RelayCommand]
		async Task Export()
		{
			var path = await FilePickers.SaveAsync(BookmarkFileFilter, "ILSpy.Bookmarks.json", Resources.BookmarkExportTitle);
			if (path != null)
				manager.Export(path);
		}

		[RelayCommand]
		async Task Import()
		{
			var path = await FilePickers.OpenAsync(BookmarkFileFilter, Resources.BookmarkImportTitle);
			if (path == null)
				return;

			BookmarkImportMode mode = BookmarkImportMode.Replace;
			if (Bookmarks.Count > 0)
			{
				if (await BookmarkDialogs.AskImportModeAsync() is not { } chosen)
					return;
				mode = chosen;
			}
			manager.Import(path, mode);
		}

		const string BookmarkFileFilter = "Bookmarks (*.json)|*.json|All Files|*.*";

		static DecompilerTabPageModel? ActiveTab => AppComposition.TryGetExport<DockWorkspace>()?.ActiveDecompilerTab;

		// Jumps to the next/previous ENABLED bookmark relative to the selected one, wrapping around.
		// Disabled bookmarks remain in the list (and the gutter) but are skipped while stepping.
		void NavigateRelative(int delta)
		{
			int index = SelectedBookmark != null ? Bookmarks.IndexOf(SelectedBookmark) : -1;
			if (NextEnabledIndex(Bookmarks.Select(b => b.Enabled).ToList(), index, delta) is { } next)
				_ = ActivateAsync(Bookmarks[next]);
		}

		/// <summary>
		/// The index of the next enabled item <paramref name="delta"/> steps from
		/// <paramref name="selectedIndex"/> (skipping disabled, wrapping around), or null when no
		/// enabled item exists. A negative <paramref name="selectedIndex"/> (nothing selected) starts
		/// just outside the list so the first step lands on the first / last candidate.
		/// </summary>
		internal static int? NextEnabledIndex(IReadOnlyList<bool> enabled, int selectedIndex, int delta)
		{
			int count = enabled.Count;
			if (count == 0)
				return null;
			int start = selectedIndex >= 0 ? selectedIndex : (delta > 0 ? -1 : count);
			for (int step = 1; step <= count; step++)
			{
				int candidate = ((start + delta * step) % count + count) % count;
				if (enabled[candidate])
					return candidate;
			}
			return null;
		}
	}
}
