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

using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblyTreeDragReorderTests
{
	// Reorder + file drop are handled generically by SharpTreeView, which delegates to
	// AssemblyListTreeNode.CanDrop/Drop. The drag payload is always assembly file paths, so a reorder
	// re-opens (deduping to the existing LoadedAssembly) and moves to the drop index.
	static AvaloniaPlatformDragEventArgs ReorderArgs(params string[] fileNames)
	{
		var data = new AvaloniaDataObject();
		data.SetData(AssemblyTreeNode.DataFormat, fileNames);
		return new AvaloniaPlatformDragEventArgs(data);
	}

	[AvaloniaTest]
	public async Task Dropping_An_Assembly_After_Another_Reorders_The_AssemblyList()
	{
		var (_, vm) = await TestHarness.BootAsync(2);
		var list = vm.AssemblyTreeModel.AssemblyList!;
		var root = (AssemblyListTreeNode)vm.AssemblyTreeModel.Root!;

		var before = list.GetAssemblies();
		var first = root.FindAssemblyNode(before[0])!;
		var second = root.FindAssemblyNode(before[1])!;

		// "Drop first AFTER second" -> insert at second's index + 1.
		var args = ReorderArgs(first.LoadedAssembly.FileName);
		int afterSecond = root.Children.IndexOf(second) + 1;
		root.CanDrop(args, afterSecond).Should().BeTrue();
		root.Drop(args, afterSecond);
		TestCapture.Step("after-reorder-first-after-second");

		// Robust to other assemblies in the shared list: second must now come before first.
		var after = list.GetAssemblies();
		System.Array.IndexOf(after, before[1]).Should().BeLessThan(System.Array.IndexOf(after, before[0]),
			"dropping first after second puts second ahead of first");

		list.Move(new[] { before[0] }, 0); // restore original order
	}

	[AvaloniaTest]
	public async Task CanDrop_Accepts_Assembly_File_Path_Payload()
	{
		var (_, vm) = await TestHarness.BootAsync(2);
		var root = (AssemblyListTreeNode)vm.AssemblyTreeModel.Root!;
		root.CanDrop(ReorderArgs("anything.dll"), 0).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task CanDrop_Rejects_When_There_Is_No_Assembly_Or_File_Payload()
	{
		var (_, vm) = await TestHarness.BootAsync(2);
		var root = (AssemblyListTreeNode)vm.AssemblyTreeModel.Root!;
		root.CanDrop(new AvaloniaPlatformDragEventArgs(new AvaloniaDataObject()), 0).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task AssemblyTreeNode_CanDrag_Only_Allows_TopLevel_NonPackage_Assemblies()
	{
		var (_, vm) = await TestHarness.BootAsync(2);
		var root = (AssemblyListTreeNode)vm.AssemblyTreeModel.Root!;
		var top = root.Children.OfType<AssemblyTreeNode>().ToArray();

		top[0].CanDrag(new ICSharpCode.ILSpyX.TreeView.SharpTreeNode[] { top[0], top[1] })
			.Should().BeTrue("top-level non-package assemblies are draggable");

		top[0].IsExpanded = true;
		await Waiters.WaitForAsync(() => top[0].Children.Count > 0);
		var child = top[0].Children[0];
		top[0].CanDrag(new[] { child }).Should().BeFalse("sub-nodes are not draggable");
	}
}
