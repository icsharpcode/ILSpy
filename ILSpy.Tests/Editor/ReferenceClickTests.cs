// Copyright (c) 2026 Siegfried Pammer
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
using Avalonia.Media;
using Avalonia.VisualTree;

using AvaloniaEdit;
using AvaloniaEdit.Rendering;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Pins the reference-link click gesture in the decompiled view: navigation happens on
/// mouse-UP and only when the pointer did not drag, so text that belongs to a link can
/// still be selected by press-and-drag.
/// </summary>
[TestFixture]
public class ReferenceClickTests
{
	static async Task<(MainWindow Window, DecompilerTextView View, DecompilerTabPageModel Tab)> SetupAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		vm.AssemblyTreeModel.SelectNode(stringNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		Waiters.PumpUI(window);
		window.UpdateLayout();
		view.Editor.TextArea.TextView.EnsureVisualLines();
		return (window, view, tab);
	}

	/// <summary>
	/// Picks a navigable reference (cross-document, not a definition) within the visible
	/// top of the document and returns a window-space point inside its text.
	/// </summary>
	static (ReferenceSegment Segment, Point Point) FindVisibleReference(
		MainWindow window, DecompilerTextView view, DecompilerTabPageModel tab)
	{
		var textView = view.Editor.TextArea.TextView;
		var segment = tab.References!
			.First(r => r.Reference != null && r.Kind == ReferenceMode.Link && !r.IsDefinition);
		var line = view.Editor.Document.GetLineByOffset(segment.StartOffset);
		view.Editor.ScrollTo(line.LineNumber, segment.StartOffset - line.Offset + 1);
		window.UpdateLayout();
		textView.EnsureVisualLines();
		var position = new TextViewPosition(line.LineNumber, segment.StartOffset - line.Offset + 2);
		var visual = textView.GetVisualPosition(position, VisualYPosition.LineMiddle) - textView.ScrollOffset;
		var point = textView.TranslatePoint(visual, window)!.Value;
		return (segment, point);
	}

	[AvaloniaTest]
	public async Task Dragging_Over_A_Link_Selects_Text_Without_Navigating()
	{
		var (window, view, tab) = await SetupAsync();
		var (_, point) = FindVisibleReference(window, view, tab);

		var navigated = false;
		tab.NavigateRequested += (_, _) => navigated = true;

		window.MouseDown(point, MouseButton.Left);
		window.MouseMove(point + new Point(60, 0));
		window.MouseUp(point + new Point(60, 0), MouseButton.Left);

		navigated.Should().BeFalse("a press-and-drag over a link is a text selection, not a click");
		view.Editor.TextArea.Selection.IsEmpty.Should().BeFalse(
			"dragging across link text must produce a selection");
	}

	[AvaloniaTest]
	public async Task Stationary_Click_On_A_Link_Navigates()
	{
		var (window, view, tab) = await SetupAsync();
		var (segment, point) = FindVisibleReference(window, view, tab);

		var navigated = false;
		NavigateRequestedEventArgs? args = null;
		tab.NavigateRequested += (_, e) => {
			navigated = true;
			args = e;
		};

		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		navigated.Should().BeTrue("a click without dragging follows the link");
		args.Should().NotBeNull();
		args!.Segment.Should().BeSameAs(segment);
		args.Reference.Should().BeSameAs(segment.Reference);
		args.Source.Should().BeNull();
		args.InNewTabPage.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Middle_Click_On_A_Link_Navigates_In_New_Tab()
	{
		var (window, view, tab) = await SetupAsync();
		var (segment, point) = FindVisibleReference(window, view, tab);

		NavigateRequestedEventArgs? args = null;
		tab.NavigateRequested += (_, e) => args = e;

		window.MouseDown(point, MouseButton.Middle);
		window.MouseUp(point, MouseButton.Middle);

		args.Should().NotBeNull();
		args!.Segment.Should().BeSameAs(segment);
		args.Reference.Should().BeSameAs(segment.Reference);
		args.InNewTabPage.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Ctrl_Middle_Click_On_A_Link_Navigates_In_New_Tab()
	{
		var (window, view, tab) = await SetupAsync();
		var (_, point) = FindVisibleReference(window, view, tab);

		NavigateRequestedEventArgs? args = null;
		tab.NavigateRequested += (_, e) => args = e;

		window.MouseDown(point, MouseButton.Middle, RawInputModifiers.Control);
		window.MouseUp(point, MouseButton.Middle, RawInputModifiers.Control);

		args.Should().NotBeNull();
		args!.InNewTabPage.Should().BeTrue("middle-click chooses the target tab; Ctrl only forces navigation over highlighting");
	}

	[AvaloniaTest]
	public async Task Click_On_A_Link_Then_Moving_The_Mouse_Does_Not_Select_Text()
	{
		var (window, view, tab) = await SetupAsync();
		var (_, point) = FindVisibleReference(window, view, tab);
		tab.NavigateRequested += (_, _) => { };

		// A stationary click navigates; the gesture must end cleanly so that afterwards moving
		// the mouse with no button held does not extend a selection (issue #3793).
		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);
		window.MouseMove(point + new Point(60, 0));

		view.Editor.TextArea.Selection.IsEmpty.Should().BeTrue(
			"moving the mouse after a click, with no button held, must not select text");
	}

	/// <summary>
	/// Scrolls the document until a navigable reference sits behind <paramref name="overlay"/>
	/// (a visual that covers part of the text area) and returns a point inside the reference's
	/// text where a click hit-tests to the overlay rather than the text.
	/// </summary>
	static (ReferenceSegment Segment, Point Point) FindReferenceBehind(
		MainWindow window, DecompilerTextView view, DecompilerTabPageModel tab, Visual overlay)
	{
		var textView = view.Editor.TextArea.TextView;
		foreach (var segment in tab.References!.Where(r => r.Reference != null && r.Kind == ReferenceMode.Link && !r.IsDefinition))
		{
			var line = view.Editor.Document.GetLineByOffset(segment.StartOffset);
			// Put the line at the very top of the viewport, where the overlay lives.
			view.Editor.ScrollTo(line.LineNumber, segment.StartOffset - line.Offset + 1, VisualYPosition.LineTop,
				referencedVerticalViewPortOffset: 0, minimumScrollFraction: 0);
			window.UpdateLayout();
			textView.EnsureVisualLines();
			for (int offset = 1; offset <= segment.Length; offset++)
			{
				var position = new TextViewPosition(line.LineNumber, segment.StartOffset - line.Offset + offset);
				var visual = textView.GetVisualPosition(position, VisualYPosition.LineMiddle) - textView.ScrollOffset;
				var point = textView.TranslatePoint(visual, window)!.Value;
				// Only the overlay's child controls are hit-testable, not its whole rectangle, so the
				// hit test is the criterion rather than the overlay's bounds.
				if (window.GetVisualAt(point) is { } hit && (hit == overlay || overlay.IsVisualAncestorOf(hit)))
					return (segment, point);
			}
		}
		Assert.Fail("no navigable reference can be scrolled behind the overlay");
		return default;
	}

	[AvaloniaTest]
	public async Task Click_On_The_Search_Panel_Does_Not_Follow_The_Link_Behind_It()
	{
		var (window, view, tab) = await SetupAsync();

		// Issue #4245: the Ctrl+F overlay is a visual child of the TextArea, so its clicks route
		// through the same TextArea handlers that resolve reference clicks by pointer position.
		var panel = view.SearchPanel;
		panel.Open();
		window.UpdateLayout();
		Waiters.PumpUI(window);
		var segment = tab.References!.First(r => r.Reference != null && r.Kind == ReferenceMode.Link && !r.IsDefinition);
		var line = view.Editor.Document.GetLineByOffset(segment.StartOffset);
		// Align the hit-testable search input with a known reference. The panel fills the text area,
		// but its controls are right-aligned and platform font metrics can otherwise leave every
		// available reference outside the controls.
		view.Editor.ScrollTo(line.LineNumber, segment.StartOffset - line.Offset + 1, VisualYPosition.LineTop,
			referencedVerticalViewPortOffset: 0, minimumScrollFraction: 0);
		window.UpdateLayout();
		view.Editor.TextArea.TextView.EnsureVisualLines();
		var position = new TextViewPosition(line.LineNumber, segment.StartOffset - line.Offset + 1);
		var visual = view.Editor.TextArea.TextView.GetVisualPosition(position, VisualYPosition.LineMiddle)
			- view.Editor.TextArea.TextView.ScrollOffset;
		var referencePoint = view.Editor.TextArea.TextView.TranslatePoint(visual, window)!.Value;
		var searchInput = panel.GetVisualDescendants().OfType<Avalonia.Controls.TextBox>().First();
		var searchInputPoint = searchInput.TranslatePoint(default, window)!.Value;
		panel.RenderTransform = new TranslateTransform(
			referencePoint.X - searchInputPoint.X - searchInput.Bounds.Width / 2,
			referencePoint.Y - searchInputPoint.Y - searchInput.Bounds.Height / 2);
		window.UpdateLayout();
		// The hit tests below (and the click) are answered from the rendered frame, so the moved
		// panel has to be drawn before they can see it at its new place.
		Waiters.PumpUI(window);
		var (_, point) = FindReferenceBehind(window, view, tab, panel);

		var navigated = false;
		tab.NavigateRequested += (_, _) => navigated = true;

		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		navigated.Should().BeFalse("a click on the find panel must not follow the reference it covers");
		panel.IsClosed.Should().BeFalse("the find panel stays open after being clicked");
	}
}
