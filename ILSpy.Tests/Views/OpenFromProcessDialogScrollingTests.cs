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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Tests.Processes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Scrolling the process list with a trackpad must be boring: the list may not resize
/// itself, drift sideways, or move its own end around. A DataGrid could not manage that -
/// its scroll extent was derived from the current scroll offset, so an offset that was
/// briefly too large inflated the extent, which permitted a larger offset still, and the
/// end of the list ran away from the user (measured in the running app: a 1500px list
/// reporting 8735px and still growing). Hence the plain ListBox, whose virtualizing panel
/// keeps the extent a function of the items alone. These tests pin the properties that made
/// the grid unusable, so a future switch back has to answer for them.
/// </summary>
[TestFixture]
public class OpenFromProcessDialogScrollingTests
{
	const int RowCount = 60;

	static OpenFromProcessDialog CreateDialogWithManyProcesses(int count = RowCount)
	{
		var explorer = new FakeProcessExplorer();
		for (int i = 0; i < count; i++)
		{
			explorer.ProcessesToReturn.Add(FakeProcessExplorer.Process(
				1000 + i, "dotnet", i % 7 == 3
					? $"Avalonia.BuildServices.486cb255276d45e9bb0ff8b0{i:D4}"
					: "MSBuild"));
		}
		return new OpenFromProcessDialog(explorer);
	}

	// The deltas a macOS trackpad actually sends: many tiny fractional events, each with a
	// small sideways component next to the vertical one. Nothing here is a whole row, which
	// is the point - whole-row steps hid every defect these tests cover.
	static readonly double[] StepsY = { 0.02, 0.04, 0.06, 0.04, 0.02, 0.08, 0.12, 0.1, 0.06, 0.04 };
	static readonly double[] StepsX = { 0, 0, 0.02, 0, -0.02, 0, 0.04, 0, 0, -0.02 };

	[AvaloniaTest]
	public async Task Trackpad_Scrolling_The_Process_List_Keeps_The_List_Steady()
	{
		var dialog = CreateDialogWithManyProcesses();
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == RowCount);

		var list = dialog.FindControl<ListBox>("ProcessesList")!;
		var item = await list.WaitForComponent<ListBoxItem>();
		Dispatcher.UIThread.RunJobs();

		var scroller = list.GetVisualDescendants().OfType<ScrollViewer>().First();
		var horizontalScrollBar = list.GetVisualDescendants().OfType<ScrollBar>()
			.Single(s => s.Name == "PART_HorizontalScrollBar");
		double rowHeight = item.Bounds.Height;

		var contentHeights = new HashSet<double>();
		var viewportHeights = new HashSet<double>();
		var horizontalOffsets = new HashSet<double>();
		int horizontalScrollBarShown = 0;
		double maxOffsetY = 0;
		var point = list.Bounds.Center;

		// Far more events than the list is long, so it spends most of them pushed against
		// the end - where an extent that follows the offset would run away.
		for (int i = 0; i < 900; i++)
		{
			dialog.MouseWheel(point, new Vector(StepsX[i % StepsX.Length], -StepsY[i % StepsY.Length]));
			Dispatcher.UIThread.RunJobs();

			if (horizontalScrollBar.IsVisible)
				horizontalScrollBarShown++;
			contentHeights.Add(scroller.Extent.Height);
			viewportHeights.Add(scroller.Viewport.Height);
			horizontalOffsets.Add(scroller.Offset.X);
			maxOffsetY = System.Math.Max(maxOffsetY, scroller.Offset.Y);
		}

		horizontalScrollBarShown.Should().Be(0,
			"the columns are laid out to fit the viewport, so there is nothing to reach sideways");
		horizontalOffsets.Should().Equal(new[] { 0.0 },
			"a vertical scroll must not shift the rows sideways, however much the fingers drift");
		viewportHeights.Should().HaveCount(1,
			"a scroll bar appearing and disappearing resizes the rows area under the pointer");
		contentHeights.Should().HaveCount(1,
			"the scrollable range must not move while the user is scrolling through it");
		contentHeights.Single().Should().BeApproximately(RowCount * rowHeight, 1.0,
			"the list is exactly as tall as its rows - anything larger lets the view scroll "
			+ "past the last row and snap back");
		maxOffsetY.Should().BeApproximately(RowCount * rowHeight - scroller.Viewport.Height, 1.0,
			"scrolling to the end must land on the last row and stop there");
	}

	[AvaloniaTest]
	public async Task The_Last_Process_Is_Reachable_And_Fully_Visible()
	{
		var dialog = CreateDialogWithManyProcesses();
		dialog.Show();
		var vm = (OpenFromProcessDialogViewModel)dialog.DataContext!;
		await Waiters.WaitForAsync(() => vm.Processes.Count == RowCount);
		var list = dialog.FindControl<ListBox>("ProcessesList")!;
		await list.WaitForComponent<ListBoxItem>();
		Dispatcher.UIThread.RunJobs();

		var point = list.Bounds.Center;
		for (int i = 0; i < 900; i++)
		{
			dialog.MouseWheel(point, new Vector(StepsX[i % StepsX.Length], -StepsY[i % StepsY.Length]));
			Dispatcher.UIThread.RunJobs();
		}

		var scroller = list.GetVisualDescendants().OfType<ScrollViewer>().First();
		var lastRow = list.GetRealizedContainers().OfType<ListBoxItem>()
			.SingleOrDefault(c => ReferenceEquals(c.DataContext, vm.Processes[^1]));

		lastRow.Should().NotBeNull("the last process must be reachable by scrolling");
		var bottom = lastRow!.TranslatePoint(new Point(0, lastRow.Bounds.Height), scroller)!.Value.Y;
		bottom.Should().BeLessThanOrEqualTo(scroller.Viewport.Height + 1,
			"the last row must come to rest inside the viewport, not half under its edge");
	}
}
