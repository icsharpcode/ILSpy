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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

[TestFixture]
public class BookmarkGutterTests
{
	[AvaloniaTest]
	public async Task Gutter_Clicks_Toggle_Bookmarks_And_A_Removed_Line_Hides_Its_Hover_Preview_Until_The_Pointer_Leaves()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();

		await vm.ShowSampleAsync(typeof(DecompileSample));

		var view = await window.WaitForComponent<DecompilerTextView>();
		await Waiters.WaitForIdleAsync();
		window.UpdateLayout();

		var margin = view.Editor.TextArea.LeftMargins.OfType<BookmarkMargin>().Single();
		margin.Bounds.Width.Should().BeGreaterThan(0, "the bookmark gutter must reserve hit-testable space");
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

		// A right-click in the gutter must not toggle: the gesture is button-gated.
		window.MouseDown(point0, MouseButton.Right);
		window.MouseUp(point0, MouseButton.Right);
		Dispatcher.UIThread.RunJobs();
		manager.Bookmarks.Should().BeEmpty("a right-click in the gutter must not toggle a bookmark");

		// A left-click at the same coordinate adds the bookmark on that line; the pointer stays on
		// the line.
		int pressed = 0;
		margin.AddHandler(InputElement.PointerPressedEvent, (_, _) => pressed++, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
		window.MouseDown(point0, MouseButton.Left);
		window.MouseUp(point0, MouseButton.Left);
		Dispatcher.UIThread.RunJobs();
		pressed.Should().BeGreaterThan(0, "the real pointer event must hit the bookmark gutter");
		manager.Bookmarks.Should().ContainSingle();
		view.GetLineForBookmark(manager.Bookmarks[0]).Should().Be(line0);

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

	// Regression: the editor reuses one TextDocument and only swaps its text on navigation, so the
	// gutter's cached glyph map must rebuild off the document's content (DebugInfo/References), not off
	// the document reference -- otherwise switching nodes leaves the gutter showing the previous (or no)
	// document's bookmarks.
	[AvaloniaTest]
	public async Task Gutter_Glyph_Map_Rebuilds_When_The_Displayed_Document_Changes()
	{
		var (window, vm) = await TestHarness.BootAsync(3);
		var manager = AppComposition.Current.GetExport<BookmarkManager>();
		manager.Clear();
		async Task<DecompilerTextView> Show(System.Type sample)
		{
			await vm.ShowSampleAsync(sample);
			var shown = await window.WaitForComponent<DecompilerTextView>();
			await Waiters.WaitForIdleAsync();
			return shown;
		}

		var view = await Show(typeof(DecompileSample));
		var margin = view.Editor.TextArea.LeftMargins.OfType<BookmarkMargin>().Single();
		var toggle = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.BookmarkToggle));

		// A body (IL-offset) anchor, so it only resolves inside the first sample type -- a token/line
		// anchor on the type would also resolve in a document that merely references it.
		Bookmark? bookmark = null;
		foreach (int candidate in Enumerable.Range(1, view.Editor.Document.LineCount).Where(view.CanToggleBookmarkAtLine))
		{
			int candidateOffset = view.Editor.Document.GetLineByNumber(candidate).Offset;
			view.Editor.TextArea.Caret.Offset = candidateOffset;
			toggle.Execute(new TextViewContext { TextView = view, TextLocation = candidateOffset });
			Dispatcher.UIThread.RunJobs();
			var candidateBookmark = manager.Bookmarks.Single();
			if (candidateBookmark.Kind == BookmarkKind.Body)
			{
				bookmark = candidateBookmark;
				break;
			}
			manager.Clear();
		}
		bookmark.Should().NotBeNull("the sample type has a method-body line to anchor to");
		int sampleLine = view.GetLineForBookmark(bookmark!)!.Value;
		margin.GlyphLinesForTest().Keys.Should().Contain(sampleLine, "the gutter shows the bookmark on its own document");

		// Navigate to another type. The preview tab reuses the same view (and gutter), only swapping the
		// document text, so the gutter must drop the stale glyph: the first type's bookmark is not here.
		var otherView = await Show(typeof(DecompileSampleOther));
		otherView.Should().BeSameAs(view, "the preview tab reuses the view, so this exercises the stale cache");
		margin.GlyphLinesForTest().Should().BeEmpty("the gutter must rebuild for the other type, where the bookmark does not resolve");

		// Returning to the bookmarked document must surface its glyph again.
		await Show(typeof(DecompileSample));
		margin.GlyphLinesForTest().Keys.Should().Contain(view.GetLineForBookmark(bookmark!)!.Value,
			"returning to the bookmarked document must show its glyph again");
	}
}
