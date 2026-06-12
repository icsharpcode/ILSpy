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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeSortTests
{
	[AvaloniaTest]
	public async Task Sorting_Keeps_The_Selected_Assembly_Selected()
	{
		// Sorting rebuilds every top-level assembly node. The naive rebuild dropped the selection
		// entirely and snapped the list back to the top -- the user saw the tree reshuffle out from
		// under them. SortAssemblyList must re-select the same assembly afterwards.
		var (window, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		var list = model.AssemblyList!;
		var pane = await window.WaitForComponent<AssemblyListPane>();
		await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		// Open extra assemblies whose names are deliberately out of sorted order so the sort moves
		// real rows around.
		var src = typeof(object).Assembly.Location;
		var temp = new[] { "ILSpy.SortTest.zzz", "ILSpy.SortTest.mmm", "ILSpy.SortTest.aaa" }
			.Select(n => Path.Combine(Path.GetTempPath(), n + ".dll")).ToArray();
		try
		{
			foreach (var p in temp)
			{
				File.Copy(src, p, true);
				list.OpenAssembly(p);
			}
			await Waiters.WaitForAsync(() => list.GetAssemblies().Count(a => a.FileName.Contains("SortTest")) == 3);

			// Select the "mmm" assembly -- it lands in the middle after sorting.
			var target = model.Root!.Children.OfType<AssemblyTreeNode>()
				.First(n => n.LoadedAssembly.FileName.Contains("SortTest.mmm"));
			model.SelectNode(target);
			await Waiters.WaitForAsync(() => ReferenceEquals(model.SelectedItem, target));
			var selectedAssembly = target.LoadedAssembly;
			TestCapture.Step("mmm-selected-before-sort");

			model.SortAssemblyList();
			Dispatcher.UIThread.RunJobs();
			TestCapture.Step("after-sort");

			// The selection survives and points at the same assembly (a fresh node for it).
			model.SelectedItems.Should().HaveCount(1, "the sort must not drop the selection");
			var selectedNode = model.SelectedItem as AssemblyTreeNode;
			Assert.That(selectedNode, Is.Not.Null, "the surviving selection is an assembly node");
			selectedNode!.LoadedAssembly.Should().BeSameAs(selectedAssembly,
				"the same assembly must stay selected across the sort");

			// And the list is actually sorted by short name.
			var names = list.GetAssemblies().Select(a => a.ShortName).ToArray();
			names.Should().BeInAscendingOrder(StringComparer.CurrentCulture,
				"SortAssemblyList orders the list by ShortName");
		}
		finally
		{
			foreach (var a in list.GetAssemblies().Where(a => a.FileName.Contains("SortTest")).ToArray())
				list.Unload(a);
			foreach (var p in temp)
				try
				{ File.Delete(p); }
				catch { /* best-effort */ }
		}
	}

	[AvaloniaTest]
	public async Task Sorting_With_No_Selection_Does_Not_Throw()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var model = vm.AssemblyTreeModel;
		model.SelectNode(null);
		await Waiters.WaitForAsync(() => model.SelectedItems.Count == 0);

		model.Invoking(m => m.SortAssemblyList()).Should().NotThrow();
		Dispatcher.UIThread.RunJobs();
	}
}
