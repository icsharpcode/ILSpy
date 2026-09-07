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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// What a wholesale change to the assembly list does to the panes that show its content.
/// Clearing the list and sorting it both raise a Reset on the collection, but they mean
/// opposite things: after a clear nothing that was on screen exists any more, while a sort
/// only reorders and everything on screen is still valid.
/// </summary>
[TestFixture]
public class AssemblyListResetTests
{
	static async Task<MethodTreeNode> DecompileAMethodAsync(ViewModels.MainWindowViewModel vm)
	{
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(method);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("AsEnumerable", "the pane must hold something before it can go stale");
		return method;
	}

	[AvaloniaTest]
	public async Task Clearing_The_Assembly_List_Empties_The_Decompiled_View()
	{
		// Clear() raises Reset, whose OldItems is null. The pane used to keep showing the last
		// decompiled member over an empty list, while removing the same assemblies one at a
		// time cleaned it up correctly.

		// Arrange - boot and decompile something.
		var (_, vm) = await TestHarness.BootAsync(3);
		await DecompileAMethodAsync(vm);

		// Act - clear the whole list, as the "Clear assembly list" command does.
		vm.AssemblyTreeModel.AssemblyList!.Clear();
		await Waiters.WaitForIdleAsync();

		// Assert - nothing from the cleared assemblies is left on screen.
		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Should().BeEmpty();
		var tab = vm.DockWorkspace.ActiveDecompilerTab;
		if (tab != null)
			tab.Text.Should().NotContain("AsEnumerable", "the assembly it came from is gone");
	}

	[AvaloniaTest]
	public async Task Sorting_The_Assembly_List_Keeps_The_Navigation_History()
	{
		// Sort() rebuilt the collection through Clear() + AddRange, so it raised the same Reset
		// a wholesale clear does and every history entry was dropped with it - even though
		// sorting removes nothing.

		// Arrange - boot, decompile two members so there is a back entry to lose.
		var (_, vm) = await TestHarness.BootAsync(3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var first = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var second = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		vm.AssemblyTreeModel.SelectNode(first);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		// NavigationHistory collapses selections inside 0.5s into one entry.
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectNode(second);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, second));
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty("the test needs history to survive");
		int assemblyCount = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length;

		// Act - sort the list.
		vm.AssemblyTreeModel.SortAssemblyList();
		await Waiters.WaitForIdleAsync();

		// Assert - the list is only reordered, so nothing it held became invalid.
		vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Should().HaveCount(assemblyCount);
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty("sorting reorders the list, it removes nothing");
	}
}
