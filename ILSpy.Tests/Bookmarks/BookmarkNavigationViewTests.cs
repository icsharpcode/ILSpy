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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AvaloniaEdit.Rendering;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

[TestFixture]
public class BookmarkNavigationViewTests
{
	// Regression for the bookmark hand-off when the active content is not a decompiler tab.
	// Activating a bookmark from a metadata table (or Options / About) must still select the saved
	// node, scroll its decompiled document to the bookmarked line, and play the one-shot highlight --
	// the pending bookmark has to reach the decompiler model that ends up displaying the node, not
	// the (absent) currently-active decompiler tab.
	[AvaloniaTest]
	public async Task Navigating_From_NonDecompiler_Content_Scrolls_To_And_Highlights_The_Bookmark()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();

		// Decompile System.Object and bookmark a line below the top so a scroll is observable.
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(objectNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		int bookmarkLine = Enumerable.Range(1, view.Editor.Document.LineCount)
			.Where(view.CanToggleBookmarkAtLine)
			.Skip(3)
			.First();
		bookmarkLine.Should().BeGreaterThan(1, "the bookmark must sit below the top so the scroll is observable");
		int offset = view.Editor.Document.GetLineByNumber(bookmarkLine).Offset;
		// Put the caret on the bookmarked line before toggling, as Ctrl+B / a gutter click would, so
		// the bookmark's captured view state agrees with its anchor.
		view.Editor.TextArea.Caret.Offset = offset;
		AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.BookmarkToggle))
			.Execute(new TextViewContext { TextView = view, TextLocation = offset });
		Dispatcher.UIThread.RunJobs();
		manager.Bookmarks.Should().ContainSingle();
		var bookmark = manager.Bookmarks[0];

		// Switch the active content to a metadata table: now there is no active decompiler tab.
		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();
		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();
		vm.DockWorkspace.ActiveDecompilerTab.Should().BeNull("the metadata table must be the active content");

		// Activate the bookmark from there.
		await AppComposition.Current.GetExport<BookmarkNavigator>().NavigateToAsync(bookmark);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		view = await window.WaitForComponent<DecompilerTextView>();
		// Wait until the one-shot highlight has registered, then assert without any further delay: the
		// adorner self-dismisses after an ~800 ms lifetime, so a fixed-length pump on a loaded CI runner
		// can outlast it and observe an empty collection. The same deferred apply also lands the caret.
		await Waiters.WaitForAsync(() => view.Editor.TextArea.TextView.BackgroundRenderers
			.OfType<LineHighlightAdorner>().Any());

		int targetLine = view.GetLineForBookmark(bookmark) ?? -1;
		targetLine.Should().BeGreaterThan(1, "the bookmark resolves to a line below the top in the freshly shown document");

		// P1: the caret/scroll landed on the bookmark's line rather than the default top.
		view.Editor.TextArea.Caret.Line.Should().Be(targetLine,
			"bookmark navigation from non-decompiler content must scroll to the saved line");

		// P2: the one-shot line highlight is playing on the freshly shown view.
		view.Editor.TextArea.TextView.BackgroundRenderers.OfType<LineHighlightAdorner>()
			.Should().ContainSingle("the destination line must be highlighted after the content switch");
	}

	// Regression: when the active tab is frozen, navigating to a bookmark in a different node must
	// open a fresh preview tab, decompile the node, and still scroll to + highlight the bookmark.
	// This exercises the fresh-decompile hand-off (PendingBookmark consumed in the document-apply
	// step), distinct from re-showing an already-decompiled node.
	[AvaloniaTest]
	public async Task Navigating_To_Bookmark_With_A_Frozen_Active_Tab_Opens_And_Positions_A_Fresh_Preview()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();
		var coreLibName = typeof(object).Assembly.GetName().Name!;

		// Bookmark a line in System.String.
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		vm.AssemblyTreeModel.SelectNode(stringNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = await window.WaitForComponent<DecompilerTextView>();
		await PumpLayoutAsync();

		int bookmarkLine = Enumerable.Range(1, view.Editor.Document.LineCount)
			.Where(view.CanToggleBookmarkAtLine)
			.Skip(3)
			.First();
		bookmarkLine.Should().BeGreaterThan(1);
		int offset = view.Editor.Document.GetLineByNumber(bookmarkLine).Offset;
		view.Editor.TextArea.Caret.Offset = offset;
		AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.BookmarkToggle))
			.Execute(new TextViewContext { TextView = view, TextLocation = offset });
		Dispatcher.UIThread.RunJobs();
		var bookmark = manager.Bookmarks.Should().ContainSingle().Subject;

		// Show a different node, then freeze that tab so the next navigation must spawn a fresh preview.
		var objectNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(objectNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.DockWorkspace.FreezeCurrentTab();

		// Activate the bookmark: a fresh preview tab must decompile System.String and land on the line.
		await AppComposition.Current.GetExport<BookmarkNavigator>().NavigateToAsync(bookmark);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var activeModel = vm.DockWorkspace.ActiveDecompilerTab;
		activeModel.Should().NotBeNull("navigation must surface a decompiler tab");

		// Wait until the fresh preview's view exists and its one-shot highlight has registered, then
		// assert without any further delay: the adorner self-dismisses after an ~800 ms lifetime, so a
		// fixed-length pump on a loaded CI runner can outlast it and observe an empty collection.
		DecompilerTextView? ActiveView() => window.GetVisualDescendants().OfType<DecompilerTextView>()
			.FirstOrDefault(v => ReferenceEquals(v.DataContext, activeModel));
		await Waiters.WaitForAsync(() => ActiveView()?.Editor.TextArea.TextView.BackgroundRenderers
			.OfType<LineHighlightAdorner>().Any() == true);
		var activeView = ActiveView()!;

		int targetLine = activeView.GetLineForBookmark(bookmark) ?? -1;
		targetLine.Should().BeGreaterThan(1, "the fresh preview shows System.String with the bookmarked line below the top");
		activeView.Editor.TextArea.Caret.Line.Should().Be(targetLine,
			"opening a fresh preview for a frozen-tab navigation must still scroll to the bookmark");
		activeView.Editor.TextArea.TextView.BackgroundRenderers.OfType<LineHighlightAdorner>()
			.Should().ContainSingle("the destination line must be highlighted in the fresh preview");
	}

	static async Task PumpLayoutAsync()
	{
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}
	}
}
