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

using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// Navigates to a bookmark's location: it makes sure the assembly is loaded (loading it from
	/// disk if it dropped out of the list), restores the captured tree node, and asks the text view
	/// to scroll to the exact line once the document is shown. A bookmark whose assembly is gone
	/// offers to remove itself.
	/// </summary>
	[Export]
	[Shared]
	public sealed class BookmarkNavigator
	{
		readonly BookmarkManager manager;

		[ImportingConstructor]
		public BookmarkNavigator(BookmarkManager manager)
		{
			this.manager = manager;
		}

		public async Task NavigateToAsync(Bookmark bookmark)
		{
			var assemblyTree = AppComposition.TryGetExport<AssemblyTreeModel>();
			if (assemblyTree?.AssemblyList is not { } list)
				return;

			var loaded = list.FindAssembly(bookmark.FileName);
			if (loaded == null)
			{
				// Only the navigate action (not startup loading) reaches disk; a vanished file prompts.
				if (!File.Exists(bookmark.FileName))
				{
					await OfferRemoveAsync(bookmark);
					return;
				}
				loaded = list.OpenAssembly(bookmark.FileName);
			}

			if (await loaded.GetMetadataFileOrNullAsync() == null)
			{
				await OfferRemoveAsync(bookmark);
				return;
			}

			// Restore the selected tree node captured with the bookmark when it is still present.
			var node = bookmark.ViewState?.SelectedTreeNodePath is { Count: > 0 } path
				? assemblyTree.FindNodeByPath(path.ToArray(), returnBestMatch: false)
				: null;
			if (node == null)
				return;

			if (bookmark.ViewState?.RenderedLayoutSettings is { } layoutSettings
				&& AppComposition.TryGetExport<SettingsService>() is { } settingsService)
			{
				layoutSettings.ApplyTo(settingsService.DisplaySettings);
			}

			// Select the node and hand the target line to the decompiler view it lands in. Routing
			// through DockWorkspace (rather than setting PendingBookmark on the active decompiler tab
			// here) covers the case where the currently active content is not a decompiler tab -- a
			// metadata table, the Options page, the About page -- so there is no active decompiler tab
			// to position directly.
			if (AppComposition.TryGetExport<DockWorkspace>() is { } dockWorkspace)
				dockWorkspace.NavigateToBookmark(node, bookmark);
			else
				assemblyTree.SelectNode(node);
		}

		async Task OfferRemoveAsync(Bookmark bookmark)
		{
			if (await BookmarkDialogs.ConfirmRemoveMissingAsync())
				manager.Remove(bookmark);
		}
	}
}
