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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Switching to another assembly list replaces the whole tree, and deleting every assembly
/// empties the one that is showing. Both leave the selection and the navigation history
/// pointing at nodes that are no longer anywhere in the tree.
/// </summary>
[TestFixture]
public class SwitchListAndDeleteAllTests
{
	static async Task GiveTheHistorySomethingToHoldAsync(ViewModels.MainWindowViewModel vm)
	{
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var methods = typeNode.Children.OfType<MethodTreeNode>().ToList();
		vm.AssemblyTreeModel.SelectNode(methods.First(m => m.MethodDefinition.Name == "AsEnumerable"));
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		// NavigationHistory collapses selections inside 0.5s into one entry.
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectNode(methods.First(m => m.MethodDefinition.Name == "Empty"));
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty("the test needs history to lose");
	}

	[AvaloniaTest]
	public async Task Switching_To_Another_Assembly_List_Drops_The_Selection_And_History()
	{
		// The whole tree is replaced, so every history entry and every selected node belongs to
		// a tree that is no longer on screen.

		// Arrange - boot, build up history in the default list.
		var (_, vm) = await TestHarness.BootAsync(3);
		await GiveTheHistorySomethingToHoldAsync(vm);

		// Act - create another list and switch to it, as the assembly-list dropdown does.
		var listManager = AppComposition.Current.GetExport<SettingsService>().AssemblyListManager;
		listManager.CreateList("test-list");
		vm.AssemblyTreeModel.ActiveListName = "test-list";
		await Waiters.WaitForIdleAsync();

		// Assert - nothing from the previous list is still held on to.
		vm.AssemblyTreeModel.SelectedItems.Should().BeEmpty("the nodes belong to the previous tree");
		vm.DockWorkspace.BackHistory.Should().BeEmpty("every entry points into the previous tree");
		vm.AssemblyTreeModel.Root!.Children.Should().BeEmpty("a freshly created list holds no assemblies");
		var tab = vm.DockWorkspace.ActiveDecompilerTab;
		tab.Should().NotBeNull();
		tab!.Text.Should().BeEmpty("the member it showed belongs to the previous list");
		tab.Title.Should().Be("(unnamed)", "a tab that shows nothing must not still name what it showed");
	}

	[AvaloniaTest]
	public async Task Ctrl_A_Then_Delete_Drops_The_Selection_History_And_Tab()
	{
		// The real gestures: SharpTreeView handles Ctrl+A (SelectAll) and Delete
		// (DeleteSelection) itself, so driving the model's SelectNodes/DeleteCore instead would
		// skip whatever the view contributes to clearing up.

		// Arrange - boot, decompile something so there is a tab and history to lose, then give
		// the tree keyboard focus.
		var (window, vm) = await TestHarness.BootAsync(3);
		await GiveTheHistorySomethingToHoldAsync(vm);
		// Collapse again: Ctrl+A selects every visible row, and the tree is normally collapsed
		// to its assemblies when a user reaches for it.
		foreach (var node in vm.AssemblyTreeModel.Root!.Children)
			node.IsExpanded = false;
		await Waiters.WaitForIdleAsync();
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var tree = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
		tree.Focus();
		Dispatcher.UIThread.RunJobs();

		// Act - Ctrl+A, then Delete.
		int selectedBefore = vm.AssemblyTreeModel.SelectedItems.Count;
		HeadlessWindowExtensions.KeyPress(window, Key.A, RawInputModifiers.Control, PhysicalKey.A, null);
		Waiters.PumpUI();
		vm.AssemblyTreeModel.SelectedItems.Count.Should().BeGreaterThan(selectedBefore,
			"Ctrl+A must reach the tree and select more than the one row that was selected");
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty("the history has to survive up to the delete");
		HeadlessWindowExtensions.KeyPress(window, Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);
		Waiters.PumpUI();
		await Waiters.WaitForAsync(() => vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length == 0,
			timeout: TimeSpan.FromSeconds(20));
		await Waiters.WaitForIdleAsync();

		// Assert - nothing that described the deleted assemblies is left behind.
		vm.AssemblyTreeModel.SelectedItems.Should().BeEmpty("the selected rows were deleted");
		vm.DockWorkspace.BackHistory.Should().BeEmpty("every entry pointed into a deleted assembly");
		var tab = vm.DockWorkspace.ActiveDecompilerTab;
		tab.Should().NotBeNull();
		tab!.Text.Should().BeEmpty("the member it showed is gone");
		tab.Title.Should().Be("(unnamed)", "a tab that shows nothing must not still name what it showed");
	}
}
