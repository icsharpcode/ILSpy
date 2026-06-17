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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// Navigates to a bookmark's location: it makes sure the assembly is loaded (loading it from
	/// disk if it dropped out of the list), resolves the token to an entity, selects the matching
	/// tree node, and asks the text view to scroll to the exact line once the document is shown. A
	/// bookmark whose assembly is gone (or whose token no longer matches) offers to remove itself.
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

			if (await loaded.GetMetadataFileOrNullAsync() == null
				|| loaded.GetTypeSystemOrNull()?.MainModule is not MetadataModule mainModule)
			{
				await OfferRemoveAsync(bookmark);
				return;
			}

			IEntity? entity = null;
			try
			{
				entity = mainModule.ResolveEntity(MetadataTokens.EntityHandle((int)bookmark.Token));
			}
			catch
			{
				// A malformed token throws inside the resolver; treated as "not found" below.
			}

			// The assembly is present, but the token no longer resolves -- e.g. the file on disk was
			// rebuilt and the tokens shifted. That is not the "assembly missing" case, so abort
			// quietly instead of nagging with the removal dialog.
			if (entity == null)
				return;

			// Find the nearest navigable tree node: the entity itself, or the closest enclosing type
			// that has one. Compiler-generated members (local functions, lambdas, their display
			// classes) have no node of their own, so walk up to the user-visible type containing them.
			var node = assemblyTree.FindTreeNode(entity);
			for (var type = entity.DeclaringTypeDefinition; node == null && type != null; type = type.DeclaringTypeDefinition)
				node = assemblyTree.FindTreeNode(type);
			if (node == null)
				return;

			// Hand the target line to the text view: it positions the caret once this node's
			// document (and its IL-offset map) has landed.
			if (AppComposition.TryGetExport<DockWorkspace>()?.ActiveDecompilerTab is { } tab)
				tab.PendingBookmark = bookmark;
			assemblyTree.SelectNode(node);
		}

		async Task OfferRemoveAsync(Bookmark bookmark)
		{
			if (await BookmarkDialogs.ConfirmRemoveMissingAsync())
				manager.Remove(bookmark);
		}
	}
}
