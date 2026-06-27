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
using System.Reflection;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AvaloniaEdit.Document;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

[TestFixture]
public class BookmarkContextMenuTests
{
	[AvaloniaTest]
	public async Task Toggle_Bookmark_Entry_Acts_On_The_Right_Clicked_Line_Not_The_Caret()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		for (int i = 0; i < 8; i++)
		{
			Dispatcher.UIThread.RunJobs();
			await Task.Delay(25);
		}

		var bookmarkableLines = Enumerable.Range(1, view.Editor.Document.LineCount)
			.Where(view.CanToggleBookmarkAtLine)
			.Take(2)
			.ToArray();
		bookmarkableLines.Should().HaveCount(2, "two distinct bookmarkable lines are needed to tell the caret from the click");

		int caretLine = bookmarkableLines[0];
		int clickedLine = bookmarkableLines[1];
		view.Editor.TextArea.Caret.Offset = view.Editor.Document.GetLineByNumber(caretLine).Offset;
		int clickedOffset = view.Editor.Document.GetLineByNumber(clickedLine).Offset;

		var toggle = AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.BookmarkToggle));
		toggle.Execute(new TextViewContext { TextView = view, TextLocation = clickedOffset });
		Dispatcher.UIThread.RunJobs();

		manager.Bookmarks.Should().ContainSingle();
		view.GetLineForBookmark(manager.Bookmarks[0]).Should().Be(clickedLine);
		manager.Bookmarks[0].LocationNodeName.Should().Be("System.Object");
	}

	[AvaloniaTest]
	public async Task Navigation_Does_Not_Fall_Back_To_Entity_When_Saved_Tree_Path_Is_Missing()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();

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

		int line = Enumerable.Range(1, view.Editor.Document.LineCount)
			.First(view.CanToggleBookmarkAtLine);
		int offset = view.Editor.Document.GetLineByNumber(line).Offset;
		AppComposition.Current.GetExport<ContextMenuEntryRegistry>()
			.GetEntry(nameof(Resources.BookmarkToggle))
			.Execute(new TextViewContext { TextView = view, TextLocation = offset });
		Dispatcher.UIThread.RunJobs();

		manager.Bookmarks.Should().ContainSingle();
		var bookmark = manager.Bookmarks[0];
		bookmark.ViewState.Should().NotBeNull();
		bookmark.ViewState = bookmark.ViewState! with { SelectedTreeNodePath = new[] { "Missing assembly", "Missing type" } };

		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		vm.AssemblyTreeModel.SelectNode(stringNode);

		await AppComposition.Current.GetExport<BookmarkNavigator>().NavigateToAsync(bookmark);

		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeSameAs(stringNode);
	}

	[AvaloniaTest]
	public async Task Queued_Bookmark_Scroll_Ignores_A_Replaced_Document()
	{
		var (window, _) = await TestHarness.BootAsync(3);
		var view = await window.WaitForComponent<DecompilerTextView>();
		view.Editor.Document = new TextDocument(string.Join("\n", Enumerable.Range(1, 30).Select(i => $"line {i}")));

		var scrollToLine = typeof(DecompilerTextView).GetMethod("ScrollToLine", BindingFlags.Instance | BindingFlags.NonPublic);
		scrollToLine.Should().NotBeNull();
		scrollToLine!.Invoke(view, new object?[] { 20, null });

		view.Editor.Document = new TextDocument("replacement");
		Dispatcher.UIThread.RunJobs();
	}
}
