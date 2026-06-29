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

using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using AvaloniaEdit.Rendering;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

[TestFixture]
public class BookmarkGutterTests
{
	[AvaloniaTest]
	public async Task Clicking_Bookmark_Gutter_Toggles_The_Visible_Line()
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
		window.UpdateLayout();

		var margin = view.Editor.TextArea.LeftMargins.OfType<BookmarkMargin>().Single();
		margin.Bounds.Width.Should().BeGreaterThan(0, "the bookmark gutter must reserve hit-testable space");

		var textView = view.Editor.TextArea.TextView;
		textView.EnsureVisualLines();
		var visualLine = textView.VisualLines.First(line => view.CanToggleBookmarkAtLine(line.FirstDocumentLine.LineNumber));
		int documentLine = visualLine.FirstDocumentLine.LineNumber;
		double y = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.LineMiddle) - textView.VerticalOffset;
		y.Should().BeInRange(0, margin.Bounds.Height, "the selected visual line must be inside the gutter bounds");
		var point = margin.TranslatePoint(new Point(margin.Bounds.Width / 2, y), window);
		point.Should().NotBeNull("the gutter line coordinate must map into the test window");

		int pressed = 0;
		margin.AddHandler(InputElement.PointerPressedEvent, (_, _) => pressed++, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

		window.MouseDown(point!.Value, MouseButton.Left);
		window.MouseUp(point.Value, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();

		pressed.Should().BeGreaterThan(0, "the real pointer event must hit the bookmark gutter");
		manager.Bookmarks.Should().ContainSingle();
		view.GetLineForBookmark(manager.Bookmarks[0]).Should().Be(documentLine);

		window.MouseDown(point.Value, MouseButton.Left);
		window.MouseUp(point.Value, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();

		manager.Bookmarks.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task RightClicking_Bookmark_Gutter_Does_Not_Toggle()
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
		window.UpdateLayout();

		var margin = view.Editor.TextArea.LeftMargins.OfType<BookmarkMargin>().Single();
		var textView = view.Editor.TextArea.TextView;
		textView.EnsureVisualLines();
		var visualLine = textView.VisualLines.First(line => view.CanToggleBookmarkAtLine(line.FirstDocumentLine.LineNumber));
		double y = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.LineMiddle) - textView.VerticalOffset;
		var point = margin.TranslatePoint(new Point(margin.Bounds.Width / 2, y), window);
		point.Should().NotBeNull("the gutter line coordinate must map into the test window");

		window.MouseDown(point!.Value, MouseButton.Right);
		window.MouseUp(point.Value, MouseButton.Right);
		Dispatcher.UIThread.RunJobs();

		manager.Bookmarks.Should().BeEmpty("a right-click in the gutter must not toggle a bookmark");

		// The same coordinate still toggles on a left-click, proving the gesture is button-gated
		// rather than disabled at that line.
		window.MouseDown(point.Value, MouseButton.Left);
		window.MouseUp(point.Value, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();

		manager.Bookmarks.Should().ContainSingle("a left-click at the same coordinate still toggles");
	}

	[AvaloniaTest]
	public async Task Removing_A_Bookmark_From_The_Gutter_Hides_Its_Hover_Preview_Until_The_Pointer_Leaves_The_Line()
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
		window.UpdateLayout();

		var margin = view.Editor.TextArea.LeftMargins.OfType<BookmarkMargin>().Single();
		var textView = view.Editor.TextArea.TextView;
		textView.EnsureVisualLines();
		var bookmarkable = textView.VisualLines
			.Where(line => view.CanToggleBookmarkAtLine(line.FirstDocumentLine.LineNumber))
			.Take(2)
			.ToArray();
		bookmarkable.Should().HaveCount(2, "two bookmarkable lines are needed to leave and re-enter the removed line");

		Point GutterPoint(VisualLine line)
		{
			double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.LineMiddle) - textView.VerticalOffset;
			var p = margin.TranslatePoint(new Point(margin.Bounds.Width / 2, y), window);
			p.Should().NotBeNull("the gutter line coordinate must map into the test window");
			return p!.Value;
		}

		int line0 = bookmarkable[0].FirstDocumentLine.LineNumber;
		int line1 = bookmarkable[1].FirstDocumentLine.LineNumber;
		Point point0 = GutterPoint(bookmarkable[0]);
		Point point1 = GutterPoint(bookmarkable[1]);

		// Hovering an empty bookmarkable line previews a ghost glyph.
		window.MouseMove(point0);
		Dispatcher.UIThread.RunJobs();
		margin.HoverPreviewLine.Should().Be(line0, "hovering a bookmarkable line previews its glyph");

		// Click adds the bookmark; the pointer stays on the line.
		window.MouseDown(point0, MouseButton.Left);
		window.MouseUp(point0, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();
		manager.Bookmarks.Should().ContainSingle();

		// Clicking again removes it: the line must visibly empty rather than redraw a ghost.
		window.MouseDown(point0, MouseButton.Left);
		window.MouseUp(point0, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();
		manager.Bookmarks.Should().BeEmpty();
		margin.HoverPreviewLine.Should().Be(-1, "removing via the gutter hides the preview");

		// A jitter that stays on the just-removed line must NOT bring the preview back.
		window.MouseMove(new Point(point0.X + 1, point0.Y));
		Dispatcher.UIThread.RunJobs();
		margin.HoverPreviewLine.Should().Be(-1, "moving within the same line keeps the removed line's preview hidden");

		// Moving onto a different line resumes normal hover.
		window.MouseMove(point1);
		Dispatcher.UIThread.RunJobs();
		margin.HoverPreviewLine.Should().Be(line1, "a different line hovers normally");

		// Returning to the original line now shows its preview again.
		window.MouseMove(point0);
		Dispatcher.UIThread.RunJobs();
		margin.HoverPreviewLine.Should().Be(line0, "leaving and re-entering the line restores its preview");
	}
}
