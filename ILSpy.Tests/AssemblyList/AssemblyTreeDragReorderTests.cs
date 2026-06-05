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

using AwesomeAssertions;

using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;

using NUnit.Framework;

using DropPosition = ILSpy.AssemblyTree.AssemblyListPane.DropPosition;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeDragReorderTests
{
	static AssemblyTreeNode[] TopLevel(AssemblyTreeModel model)
		=> model.Root!.Children.OfType<AssemblyTreeNode>().ToArray();

	[AvaloniaTest]
	public async Task CanReorder_Accepts_A_TopLevel_Assembly_Dropped_After_Another()
	{
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var top = TopLevel(vm.AssemblyTreeModel);

		pane.CanReorder(new[] { top[0] }, top[1], DropPosition.After).Should().BeTrue(
			"a top-level assembly may be reordered before/after another");
	}

	[AvaloniaTest]
	public async Task Dropping_An_Assembly_After_Another_Reorders_The_AssemblyList()
	{
		// End-to-end on the reorder path: "drag assembly[0] After assembly[1]" must move it through
		// AssemblyList.Move (the same persistence path file-open / Unload use).
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var before = list.GetAssemblies();
		var first = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>()
			.First(n => n.LoadedAssembly == before[0]);
		var second = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>()
			.First(n => n.LoadedAssembly == before[1]);

		pane.ReorderAssemblies(new[] { first }, second, DropPosition.After).Should().BeTrue();
		TestCapture.Step("after-reorder-first-after-second");

		var after = list.GetAssemblies();
		after[0].Should().BeSameAs(before[1]);
		after[1].Should().BeSameAs(before[0]);

		// Restore the original order for following tests.
		list.Move(new[] { after[1] }, 0);
	}

	[AvaloniaTest]
	public async Task CanReorder_Rejects_Dropping_A_Node_Onto_Itself()
	{
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var top = TopLevel(vm.AssemblyTreeModel);

		pane.CanReorder(new[] { top[0] }, top[0], DropPosition.After).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task CanReorder_Rejects_Append_Without_A_Before_After_Target()
	{
		// Append (a drop on empty space / a non-assembly row, where the hit-test yields no target)
		// is an open, not a reorder.
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var top = TopLevel(vm.AssemblyTreeModel);

		pane.CanReorder(new[] { top[0] }, top[1], DropPosition.Append).Should().BeFalse();
		pane.CanReorder(new[] { top[0] }, null, DropPosition.Before).Should().BeFalse();
	}
}
