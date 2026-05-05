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
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class NavigationTests
{
	[AvaloniaTest]
	public async Task Back_Navigation_Restores_Previously_Selected_Node()
	{
		// Selecting two methods in succession records both in the navigation history; pressing
		// the Back command must restore the previously-selected node, re-decompile it, and
		// scroll the tree so that node is back in view.

		// Arrange — boot the window, wait for assemblies, expand Enumerable, capture two methods
		// that we'll bounce between.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		// expand typeNode
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		// Act 1 — select AsEnumerable, wait for it to decompile.
		// select firstMethod
		// select firstMethod
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		// wait for decompile to finish
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		firstTab.Text.Should().Contain("AsEnumerable");

		// NavigationHistory collapses selections that happen within 0.5s into one entry. Wait
		// past that window so the second selection records a real back-history entry.
		await Task.Delay(600);

		// Act 2 — select Empty, wait for its decompile.
		// select secondMethod
		// select secondMethod
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		// wait for the predicate
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, secondMethod));
		// wait for decompile to finish
		var secondTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		secondTab.Text.Should().Contain("Empty");

		// Act 3 — fire NavigateBack.
		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
		// execute vm.DockWorkspace.NavigateBackCommand
		vm.DockWorkspace.NavigateBackCommand.Execute(null);

		// Assert — selection restores to AsEnumerable, the document re-decompiles to its body,
		// and the row is centred back into view.
		// wait for the predicate
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod));
		// wait for decompile to finish
		var restoredTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		restoredTab.Text.Should().Contain("AsEnumerable");
		restoredTab.Text.Should().Contain("return source");

		firstMethod.Should().Be().CenteredInView();
	}

	[AvaloniaTest]
	public async Task Back_SplitButton_Dropdown_Lists_History_And_Jumps_To_Selected_Entry()
	{
		// The Back button on the toolbar is a SplitButton whose chevron drops a list of recent
		// history entries. Verifies that the menu populates newest-first from the back stack,
		// and that picking a non-immediate entry jumps multiple steps in one click (with the
		// displaced entries pushed onto the forward stack).

		// Arrange — boot, wait for assemblies, expand Enumerable, capture three methods.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		// expand typeNode
		typeNode.IsExpanded = true;
		var methodA = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var methodB = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		var methodC = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Range");

		// Act 1 — three distinct selections with >0.6s gaps so each lands as its own entry on
		// the back stack (NavigationHistory collapses sub-0.5s rapid succession into one entry).
		// select methodA
		vm.AssemblyTreeModel.SelectNode(methodA);
		// wait for decompile to finish
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		// select methodB
		vm.AssemblyTreeModel.SelectNode(methodB);
		// wait for decompile to finish
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		// select methodC
		vm.AssemblyTreeModel.SelectNode(methodC);
		// wait for decompile to finish
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Act 2 — open the Back SplitButton's flyout. The Opening handler populates the menu
		// from the current back history.
		var backSplit = window.GetVisualDescendants().OfType<SplitButton>()
			.Single(sb => sb.Name == "BackSplitButton");
		var flyout = (MenuFlyout)backSplit.Flyout!;
		// open flyout on backSplit
		flyout.ShowAt(backSplit);
		// wait for the predicate
		// wait for the predicate
		await Waiters.WaitForAsync(() => flyout.Items.OfType<MenuItem>().Count() >= 2);

		// Assert 1 — newest-first ordering: index 0 is the immediate previous selection
		// (methodB), index 1 is the one before that (methodA). Each menu item carries a
		// TreeNodeEntry wrapping the original tree node.
		var items = flyout.Items.OfType<MenuItem>().ToList();
		((string)items[0].Header!).Should().Be((string)methodB.Text);
		((string)items[1].Header!).Should().Be((string)methodA.Text);
		items[1].CommandParameter.Should().BeOfType<global::ILSpy.Navigation.TreeNodeEntry>();
		var entry = (global::ILSpy.Navigation.TreeNodeEntry)items[1].CommandParameter!;
		ReferenceEquals(entry.Node, methodA).Should().BeTrue();

		// Act 3 — multi-step jump: clicking methodA pops two entries off the back stack in one go.
		items[1].Command!.Execute(items[1].CommandParameter);
		// wait for the predicate
		// wait for the predicate
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, methodA));

		// Assert 2 — the two displaced entries (methodC, methodB) are now on the forward stack,
		// so Forward becomes available.
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue();
	}
}
