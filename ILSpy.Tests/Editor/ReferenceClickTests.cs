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
/// Pins the reference-link click gesture in the decompiled view (WPF parity): navigation
/// happens on mouse-UP and only when the pointer did not drag, so text that belongs to a
/// link can still be selected by press-and-drag.
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
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
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
			.First(r => r.Reference != null && !r.IsLocal && !r.IsDefinition);
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
		tab.NavigateRequested += _ => navigated = true;

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
		var (_, point) = FindVisibleReference(window, view, tab);

		var navigated = false;
		tab.NavigateRequested += _ => navigated = true;

		window.MouseDown(point, MouseButton.Left);
		window.MouseUp(point, MouseButton.Left);

		navigated.Should().BeTrue("a click without dragging follows the link");
	}
}
