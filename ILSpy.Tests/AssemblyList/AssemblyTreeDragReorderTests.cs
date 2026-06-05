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

using Avalonia.Controls;
using Avalonia.Controls.DataGridDragDrop;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ILSpy.AssemblyTree;
using ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
[Ignore("Assembly drag-reorder on the ListBox-based SharpTreeView is not yet implemented (these "
	+ "test the old ProDataGrid RowDropHandler mechanism). Tracked as a follow-up; the reorder "
	+ "logic in AssemblyRowDropHandler is retained to port onto the new drag pipeline.")]
public class AssemblyTreeDragReorderTests
{
	[AvaloniaTest]
	public async Task AssemblyListPane_Enables_Row_Reorder_On_The_DataGrid()
	{
		// Mirrors WPF's SharpTreeView AllowDropOrder=True — the assembly tree must opt in to
		// ProDataGrid's row-drag-drop machinery (otherwise the handler we wire below is never
		// asked to validate anything).
		var (window, vm) = await TestHarness.BootAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		grid.CanUserReorderRows.Should().BeTrue();
		grid.RowDragHandle.Should().Be(DataGridRowDragHandle.Row,
			"the assembly tree has no row-headers so the drag gesture must originate from the row body");
		// Regression — ProDataGrid's row-drag controller short-circuits when IsReadOnly is true
		// (DataGridRowDragDropController.ShouldHandlePointer + DataGridHierarchicalRowReorderHandler
		// both bail on grid.IsReadOnly). Read-only intent moved onto the column instead so cells
		// stay uneditable without disabling drag.
		grid.IsReadOnly.Should().BeFalse();
		grid.Columns[0].IsReadOnly.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task AssemblyListPane_Wires_AssemblyRowDropHandler_With_The_Live_AssemblyList()
	{
		// The pane owns the handler instance — it builds one from the model's AssemblyList so
		// dropping into the grid mutates the same list that file-open and Unload mutate.
		var (window, vm) = await TestHarness.BootAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		grid.RowDropHandler.Should().BeOfType<AssemblyRowDropHandler>();
	}

	[AvaloniaTest]
	public async Task Dropping_An_Assembly_After_Another_Reorders_The_AssemblyList()
	{
		// End-to-end on the live drop handler: simulate "drag row[1] After row[0]" and verify
		// the underlying AssemblyList reordered. The handler is responsible for turning
		// HierarchicalNode wrappers (or bare AssemblyTreeNodes) into LoadedAssembly refs and
		// calling AssemblyList.Move with the correct insert index.
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var before = list.GetAssemblies();
		var first = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>()
			.First(n => n.LoadedAssembly == before[0]);
		var second = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>()
			.First(n => n.LoadedAssembly == before[1]);

		var handler = (AssemblyRowDropHandler)grid.RowDropHandler;
		// "Drop first AFTER second" → ordering should become [second, first, ...rest].
		var args = MakeArgs(items: new object[] { first }, target: second,
			position: DataGridRowDropPosition.After);
		handler.Validate(args).Should().BeTrue();
		handler.Execute(args).Should().BeTrue();
		TestCapture.Step("after-reorder-first-after-second");

		var after = list.GetAssemblies();
		after[0].Should().BeSameAs(before[1]);
		after[1].Should().BeSameAs(before[0]);

		// Restore so subsequent tests run against the original order.
		list.Move(new[] { after[1] }, 0);
	}

	[AvaloniaTest]
	public async Task Validate_Rejects_Inside_Position()
	{
		// "Inside" would mean dropping one assembly as a child of another — there's no such
		// relationship in the model, so the handler must refuse it (the grid then renders the
		// "not allowed" cursor).
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var topLevel = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().ToArray();
		var handler = (AssemblyRowDropHandler)grid.RowDropHandler;

		var args = MakeArgs(items: new object[] { topLevel[1] }, target: topLevel[0],
			position: DataGridRowDropPosition.Inside);
		handler.Validate(args).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Validate_Rejects_Non_TopLevel_Target()
	{
		// Dropping onto a child of an assembly (a namespace or type) must not reorder anything
		// — that target doesn't live in AssemblyList at all.
		var (window, vm) = await TestHarness.BootAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var topLevel = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().First();
		topLevel.IsExpanded = true;
		TestCapture.Step("top-level-expanded");
		var childNode = topLevel.Children.First();

		var handler = (AssemblyRowDropHandler)grid.RowDropHandler;
		var args = MakeArgs(items: new object[] { topLevel }, target: childNode,
			position: DataGridRowDropPosition.Before);
		handler.Validate(args).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Validate_Rejects_Dragging_Non_Assembly_Nodes()
	{
		// Sub-nodes (namespaces, types, etc.) must not be picked up by the reorder gesture —
		// only top-level AssemblyTreeNodes are eligible source items.
		var (window, vm) = await TestHarness.BootAsync(2);
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<DataGrid>();

		var topLevel = vm.AssemblyTreeModel.Root!.Children.OfType<AssemblyTreeNode>().ToArray();
		topLevel[0].IsExpanded = true;
		TestCapture.Step("first-assembly-expanded");
		var childOfFirst = topLevel[0].Children.First();

		var handler = (AssemblyRowDropHandler)grid.RowDropHandler;
		var args = MakeArgs(items: new object[] { childOfFirst }, target: topLevel[1],
			position: DataGridRowDropPosition.Before);
		handler.Validate(args).Should().BeFalse();
	}

	static DataGridRowDropEventArgs MakeArgs(
		object[] items, object target, DataGridRowDropPosition position)
		=> new(
			grid: null!,
			targetList: null,
			items: items,
			sourceIndices: System.Array.Empty<int>(),
			targetItem: target,
			targetIndex: 0,
			insertIndex: 0,
			targetRow: null,
			position: position,
			isSameGrid: true,
			requestedEffect: DragDropEffects.Move,
			dragEventArgs: null!);
}
