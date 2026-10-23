// Copyright (c) 2026 Christoph Wille
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AvaloniaEdit.Document;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Editor;

/// <summary>
/// The editor's scroll increments follow the text metrics and the viewport, the way the
/// WPF AvalonEdit text view did: a wheel notch moves a few lines (the same distance
/// horizontally), a scrollbar track click moves one page. AvaloniaEdit's own TextView reports
/// fixed 10x50 px steps and 10x100 px pages, which made Shift+wheel crawl (icsharpcode/ILSpy#4173)
/// and track clicks move barely more than the arrow buttons (icsharpcode/ILSpy#4219).
/// </summary>
[TestFixture]
public class EditorScrollIncrementTests
{
	// A large font keeps the line-based increments far away from the old fixed pixel constants,
	// so a regression to 10/50/100 px cannot pass by coincidence, while the window stays tall
	// enough for a page to hold more lines than a wheel notch scrolls. The font is applied after
	// the editor is attached, because attaching re-applies the display-settings font.
	const double FontSize = 40;

	// The metrics branch on the desktop. Pin the Windows baseline so the assertions hold on every
	// CI runner, and restore the real platform afterwards; macOS cases flip the seam themselves.
	bool isWindows, isMacOS;

	[SetUp]
	public void PinWindowsPlatform()
	{
		(isWindows, isMacOS) = (SystemScrollSettings.IsWindows, SystemScrollSettings.IsMacOS);
		SystemScrollSettings.IsWindows = true;
		SystemScrollSettings.IsMacOS = false;
	}

	[TearDown]
	public void RestorePlatform()
	{
		SystemScrollSettings.IsWindows = isWindows;
		SystemScrollSettings.IsMacOS = isMacOS;
	}

	static (Window Window, DecompilerTextEditor Editor, ScrollViewer ScrollViewer) ShowOverflowingEditor()
	{
		var text = string.Join("\n", Enumerable.Range(1, 200).Select(i => $"line {i} " + new string('x', 300)));
		var editor = new DecompilerTextEditor { Document = new TextDocument(text) };
		var window = new Window { Width = 300, Height = 600, Content = editor };
		window.Show();
		editor.FontSize = FontSize;
		// The TextView publishes its extent from its own measure pass and the ScrollViewer picks it
		// up on the next one, so pump layout and render twice before reading scroll metrics.
		for (int i = 0; i < 2; i++)
		{
			window.UpdateLayout();
			Dispatcher.UIThread.RunJobs();
			AvaloniaHeadlessPlatform.ForceRenderTimerTick();
			Dispatcher.UIThread.RunJobs();
		}

		var scrollViewer = editor.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
		scrollViewer.Should().NotBeNull("the TextEditor template installs a ScrollViewer around the TextArea");
		scrollViewer!.Extent.Height.Should().BeGreaterThan(scrollViewer.Viewport.Height, $"the document must overflow vertically (editor bounds {editor.Bounds}, viewport {scrollViewer.Viewport})");
		scrollViewer.Extent.Width.Should().BeGreaterThan(scrollViewer.Viewport.Width, "the document must overflow horizontally");
		editor.TextArea.TextView.DefaultLineHeight.Should().BeGreaterThan(30, "the test font must produce tall lines");
		return (window, editor, scrollViewer);
	}

	[AvaloniaTest]
	public void Wheel_Step_Is_WheelScrollLines_Lines_On_Both_Axes()
	{
		var (_, editor, _) = ShowOverflowingEditor();
		var scrollable = (ILogicalScrollable)editor.TextArea;
		double expected = SystemScrollSettings.WheelScrollLines * editor.TextArea.TextView.DefaultLineHeight;

		scrollable.ScrollSize.Height.Should().BeApproximately(expected, 0.01, "a wheel notch scrolls the system's number of lines");
		scrollable.ScrollSize.Width.Should().BeApproximately(expected, 0.01, "Shift+wheel scrolls the same distance horizontally (#4173)");
	}

	[AvaloniaTest]
	public void Page_Step_Is_One_Viewport_And_Larger_Than_The_Arrow_Step()
	{
		var (_, editor, scrollViewer) = ShowOverflowingEditor();
		var scrollable = (ILogicalScrollable)editor.TextArea;

		scrollable.PageScrollSize.Should().Be(scrollViewer.Viewport, "a page is the visible area");
		scrollViewer.LargeChange.Height.Should().BeApproximately(scrollViewer.Viewport.Height, 0.01, "the ScrollViewer derives LargeChange from PageScrollSize");
		scrollViewer.LargeChange.Height.Should().BeGreaterThan(scrollViewer.SmallChange.Height, "a track click must move further than an arrow click (#4219)");
	}

	[AvaloniaTest]
	public void Shift_Wheel_Scrolls_Horizontally_By_One_Wheel_Step()
	{
		var (window, editor, scrollViewer) = ShowOverflowingEditor();
		double expected = SystemScrollSettings.WheelScrollLines * editor.TextArea.TextView.DefaultLineHeight;
		var point = editor.TranslatePoint(new Point(100, 100), window) ?? new Point(100, 100);

		window.MouseWheel(point, new Vector(0, -1), RawInputModifiers.Shift);
		Dispatcher.UIThread.RunJobs();

		scrollViewer.Offset.X.Should().BeApproximately(expected, 0.01, "one Shift+wheel notch moves one wheel step horizontally (#4173)");
		scrollViewer.Offset.Y.Should().Be(0, "Shift+wheel must not scroll vertically");
	}

	// Avalonia.Native divides trackpad deltas by 50 px and mouse-wheel deltas by 5 AppKit line
	// units before they reach us, and neither carries a precision flag. A fixed 50 px step is the
	// one value that keeps trackpads 1:1 at every zoom level, so on macOS the step must not follow
	// the font.
	[AvaloniaTest]
	public void On_MacOS_The_Wheel_Step_Is_A_Fixed_50_Pixels_On_Both_Axes()
	{
		SystemScrollSettings.IsWindows = false;
		SystemScrollSettings.IsMacOS = true;
		var (_, editor, _) = ShowOverflowingEditor();
		var scrollable = (ILogicalScrollable)editor.TextArea;

		scrollable.ScrollSize.Should().Be(new Size(50, 50), "macOS trackpads report pixels / 50, so 50 px per unit tracks the finger 1:1 regardless of font size");
	}

	[AvaloniaTest]
	public void On_MacOS_Shift_Wheel_Scrolls_Horizontally_By_50_Pixels()
	{
		SystemScrollSettings.IsWindows = false;
		SystemScrollSettings.IsMacOS = true;
		var (window, editor, scrollViewer) = ShowOverflowingEditor();
		var point = editor.TranslatePoint(new Point(100, 100), window) ?? new Point(100, 100);

		window.MouseWheel(point, new Vector(0, -1), RawInputModifiers.Shift);
		Dispatcher.UIThread.RunJobs();

		scrollViewer.Offset.X.Should().BeApproximately(50, 0.01, "one unit of Shift+wheel moves 50 px on macOS (#4173)");
	}

	// AppKit's NSScrollView.pageScroll and GTK's 0.9 page_increment keep part of the previous page
	// in view after a page scroll, and the macOS HIG suggests one line of overlap; Windows (WPF)
	// pages by the full viewport.
	[AvaloniaTest]
	public void Off_Windows_A_Page_Keeps_One_Line_Of_The_Previous_Page_Visible()
	{
		SystemScrollSettings.IsWindows = false;
		var (_, editor, scrollViewer) = ShowOverflowingEditor();
		var scrollable = (ILogicalScrollable)editor.TextArea;
		double lineHeight = editor.TextArea.TextView.DefaultLineHeight;
		var viewport = scrollViewer.Viewport;

		scrollable.PageScrollSize.Should().Be(new Size(viewport.Width - lineHeight, viewport.Height - lineHeight), "a page scroll keeps one line of overlap on macOS and Linux");
		scrollViewer.LargeChange.Height.Should().BeApproximately(viewport.Height - lineHeight, 0.01, "the ScrollViewer derives LargeChange from PageScrollSize");
	}

	[AvaloniaTest]
	public void ScrollBar_Page_Down_Scrolls_One_Viewport()
	{
		var (_, _, scrollViewer) = ShowOverflowingEditor();
		var verticalBar = scrollViewer.GetVisualDescendants().OfType<ScrollBar>()
			.FirstOrDefault(b => b.Orientation == Orientation.Vertical);
		verticalBar.Should().NotBeNull("the ScrollViewer template has a vertical ScrollBar");

		verticalBar!.PageDown();
		Dispatcher.UIThread.RunJobs();

		scrollViewer.Offset.Y.Should().BeApproximately(scrollViewer.Viewport.Height, 0.01, "a track click pages by one viewport (#4219)");
	}
}
