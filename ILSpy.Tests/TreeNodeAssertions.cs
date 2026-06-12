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

using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.Tests;

public static class TreeNodeAssertionsExtensions
{
	public static TreeNodeAssertions Should(this SharpTreeNode subject) => new(subject);
}

public class TreeNodeAssertions
{
	public SharpTreeNode Subject { get; }

	public TreeNodeAssertions(SharpTreeNode subject)
	{
		Subject = subject ?? throw new ArgumentNullException(nameof(subject));
	}

	public TreeNodeBeAssertions Be() => new(Subject);
}

public class TreeNodeBeAssertions
{
	static readonly TimeSpan ScrollPollTimeout = TimeSpan.FromSeconds(3);

	public SharpTreeNode Subject { get; }

	public TreeNodeBeAssertions(SharpTreeNode subject)
	{
		Subject = subject;
	}

	public AndConstraint<TreeNodeBeAssertions> ScrolledIntoView(string because = "", params object[] becauseArgs)
		=> AssertScrollState(centered: false, because, becauseArgs);

	public AndConstraint<TreeNodeBeAssertions> CenteredInView(string because = "", params object[] becauseArgs)
		=> AssertScrollState(centered: true, because, becauseArgs);

	AndConstraint<TreeNodeBeAssertions> AssertScrollState(bool centered, string because, object[] becauseArgs)
	{
		var grid = TryFindGrid(out var failure);
		if (grid is null)
		{
			false.Should().BeTrue(
				$"cannot verify scroll state for {Describe()}: {failure}{(string.IsNullOrEmpty(because) ? "" : " " + string.Format(because, becauseArgs))}");
			return new AndConstraint<TreeNodeBeAssertions>(this);
		}

		var scrollViewer = grid.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault()
			?? throw new InvalidOperationException("SharpTreeView has no ScrollViewer in its visual tree.");

		// ScrollIntoView is posted at Background priority — give it time to realise the row.
		var deadline = DateTime.UtcNow + ScrollPollTimeout;
		ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem? row = null;
		bool ok = false;
		while (DateTime.UtcNow < deadline)
		{
			Dispatcher.UIThread.RunJobs();
			row = FindRow(grid, Subject);
			if (row != null && IsInViewport(row, scrollViewer)
				&& (!centered || IsRoughlyCentered(row, scrollViewer)))
			{
				ok = true;
				break;
			}
		}

		ok.Should().BeTrue(
			$"{Describe()} should be {(centered ? "centred" : "scrolled into view")}{(string.IsNullOrEmpty(because) ? "" : ", " + string.Format(because, becauseArgs))}, " +
			BuildState(scrollViewer, row));

		return new AndConstraint<TreeNodeBeAssertions>(this);
	}

	static ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView? TryFindGrid(out string? failure)
	{
		var window = FindActiveWindow();
		if (window is null)
		{
			failure = "no active Avalonia Window";
			return null;
		}

		var pane = window.GetVisualDescendants().OfType<AssemblyListPane>().FirstOrDefault();
		if (pane is null)
		{
			failure = "AssemblyListPane is not in the visual tree (the assembly tree pane isn't shown)";
			return null;
		}

		var grid = pane.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>().FirstOrDefault();
		if (grid is null)
		{
			failure = "SharpTreeView not found inside AssemblyListPane";
			return null;
		}

		failure = null;
		return grid;
	}

	static Window? FindActiveWindow()
	{
		// ApplicationLifetime is null in headless; the [Shared] MainWindow export gives us
		// the same instance the test resolved.
		try
		{
			return AppComposition.Current.GetExport<MainWindow>();
		}
		catch
		{
			return null;
		}
	}

	static ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem? FindRow(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView grid, SharpTreeNode target)
	{
		foreach (var row in grid.GetVisualDescendants().OfType<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem>())
		{
			if (ReferenceEquals(row.DataContext, target))
				return row;
		}
		return null;
	}

	static bool IsInViewport(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem row, ScrollViewer scrollViewer)
	{
		var topLeft = row.TranslatePoint(new Point(0, 0), scrollViewer);
		if (topLeft is null)
			return false;
		var top = topLeft.Value.Y;
		var bottom = top + row.Bounds.Height;
		return top >= 0 && bottom <= scrollViewer.Viewport.Height + 0.5;
	}

	static bool IsRoughlyCentered(ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem row, ScrollViewer scrollViewer)
	{
		var topLeft = row.TranslatePoint(new Point(0, 0), scrollViewer);
		if (topLeft is null)
			return false;
		var rowMid = topLeft.Value.Y + row.Bounds.Height / 2;
		var viewportMid = scrollViewer.Viewport.Height / 2;
		// Accept rows clamped at the start/end of the list as "as centred as possible".
		var nearTopClamp = scrollViewer.Offset.Y <= 0.5;
		var nearBottomClamp = scrollViewer.Offset.Y >= scrollViewer.Extent.Height - scrollViewer.Viewport.Height - 0.5;
		return Math.Abs(rowMid - viewportMid) <= row.Bounds.Height || nearTopClamp || nearBottomClamp;
	}

	string BuildState(ScrollViewer scrollViewer, ICSharpCode.ILSpy.Controls.TreeView.SharpTreeViewItem? row)
	{
		if (row is null)
			return $"but no SharpTreeViewItem has been realised for it (offset={scrollViewer.Offset}, viewport={scrollViewer.Viewport}, extent={scrollViewer.Extent})";
		var topLeft = row.TranslatePoint(new Point(0, 0), scrollViewer);
		var top = topLeft?.Y;
		var bottom = top + row.Bounds.Height;
		return $"but its row sits at {top:0.#}..{bottom:0.#} relative to the viewport " +
			$"(viewport height {scrollViewer.Viewport.Height:0.#}, offset {scrollViewer.Offset.Y:0.#}, extent {scrollViewer.Extent.Height:0.#})";
	}

	string Describe()
	{
		var text = Subject.Text?.ToString();
		return string.IsNullOrEmpty(text) ? Subject.GetType().Name : $"\"{text}\"";
	}
}
