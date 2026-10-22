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

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

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

		// Arrange — boot the window, wait for assemblies, expand the sample type, capture two methods
		// that we'll bounce between.
		var (_, vm) = await TestHarness.BootAsync(3);

		var typeNode = await vm.OpenSampleAsync(typeof(DecompileSample));
		typeNode.IsExpanded = true;
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == nameof(DecompileSample.Describe));
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == nameof(DecompileSample.Twice));

		// Act 1 — select the first method, wait for it to decompile.
		vm.AssemblyTreeModel.SelectNode(firstMethod);
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		firstTab.Text.Should().Contain(nameof(DecompileSample.Describe));
		TestCapture.Step("first-method-decompiled");

		// Act 2 — select the second method, wait for its decompile.
		vm.AssemblyTreeModel.SelectNode(secondMethod);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, secondMethod));
		var secondTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		secondTab.Text.Should().Contain(nameof(DecompileSample.Twice));
		TestCapture.Step("second-method-decompiled");

		// Give the assembly pane the focus: the tab being navigated is already the active
		// document, so Back must not move the active pane to it (WPF kept the current view focused).
		vm.DockWorkspace.ShowToolPane(AssemblyTreeModel.PaneContentId);
		var activePane = vm.DockWorkspace.Layout.FocusedDockable;
		activePane.Should().NotBeNull("showing the assembly pane must make it the focused dockable");

		// Act 3 — fire NavigateBack, which is what the Alt+Left key binding and the View menu invoke.
		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
		vm.DockWorkspace.NavigateBackCommand.Execute(null);
		TestCapture.Step("navigated-back");

		// Assert — selection restores to the first method, the document re-decompiles to its body,
		// the row is centred back into view, and the active pane stays put.
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod));
		var restoredTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		restoredTab.Text.Should().Contain(nameof(DecompileSample.Describe));
		restoredTab.Text.Should().Contain("return Name + Count");

		firstMethod.Should().Be().CenteredInView();
		vm.DockWorkspace.Layout.FocusedDockable.Should().BeSameAs(activePane,
			"navigating back must not move the active pane to the editor");
	}

	[AvaloniaTest]
	public async Task Back_SplitButton_Dropdown_Lists_History_And_Jumps_To_Selected_Entry()
	{
		// The Back button on the toolbar is a SplitButton whose chevron drops a list of recent
		// history entries. Verifies that the menu populates newest-first from the back stack,
		// and that picking a non-immediate entry jumps multiple steps in one click (with the
		// displaced entries pushed onto the forward stack).

		// Arrange — boot, wait for assemblies, expand the sample type, capture three methods.
		var (window, vm) = await TestHarness.BootAsync(3);

		var typeNode = await vm.OpenSampleAsync(typeof(DecompileSample));
		typeNode.IsExpanded = true;
		var methodA = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == nameof(DecompileSample.Describe));
		var methodB = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == nameof(DecompileSample.Twice));
		var methodC = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == nameof(DecompileSample.Reset));

		// Act 1 — three distinct selections, each landing as its own entry on the back stack.
		vm.AssemblyTreeModel.SelectNode(methodA);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.AssemblyTreeModel.SelectNode(methodB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.AssemblyTreeModel.SelectNode(methodC);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("third-method-decompiled");

		// Act 2 — open the Back SplitButton's flyout. The Opening handler populates the menu
		// from the current back history.
		var backSplit = window.GetVisualDescendants().OfType<SplitButton>()
			.Single(sb => sb.Name == "BackSplitButton");
		var flyout = (MenuFlyout)backSplit.Flyout!;
		// open flyout on backSplit
		flyout.ShowAt(backSplit);
		await Waiters.WaitForAsync(() => flyout.Items.OfType<MenuItem>().Count() >= 2);
		TestCapture.Step("back-history-flyout-open");

		// Assert 1 — newest-first ordering: index 0 is the immediate previous selection
		// (methodB), index 1 is the one before that (methodA). Each menu item carries a
		// TreeNodeEntry wrapping the original tree node, and the header reads the richer
		// NavigationText, which disambiguates "Empty" from other "Empty" methods by
		// prefixing the declaring type.
		var items = flyout.Items.OfType<MenuItem>().ToList();
		((string)items[0].Header!).Should().Be((string)methodB.NavigationText);
		((string)items[1].Header!).Should().Be((string)methodA.NavigationText);
		((string)items[0].Header!).Should().Contain(nameof(DecompileSample),
			"NavigationText must include the declaring type");
		items[1].CommandParameter.Should().BeOfType<ICSharpCode.ILSpy.Navigation.TreeNodeEntry>();
		var entry = (ICSharpCode.ILSpy.Navigation.TreeNodeEntry)items[1].CommandParameter!;
		ReferenceEquals(entry.Node, methodA).Should().BeTrue();

		// Act 3 — multi-step jump: clicking methodA pops two entries off the back stack in one go.
		items[1].Command!.Execute(items[1].CommandParameter);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, methodA));
		TestCapture.Step("jumped-to-as-enumerable");

		// Assert 2 — the two displaced entries (methodC, methodB) are now on the forward stack,
		// so Forward becomes available.
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Back_History_Dropdown_Uses_NavigationText_For_Generic_Tree_Nodes()
	{
		// Bare Text on grouping nodes ("Base Types", "Derived Types", "References") reads
		// ambiguously in the back-history dropdown — the user can't tell which type's bases
		// they're looking at. ILSpyTreeNode.NavigationText carries the richer disambiguated
		// form ("Base Types (System.Exception)"), and TreeNodeEntry.DisplayText must use it
		// when the node implements ILSpyTreeNode.

		// Arrange — boot, expand a type that has a base type so we can select its
		// BaseTypesTreeNode child.
		var (window, vm) = await TestHarness.BootAsync();

		var typeNode = await vm.OpenSampleAsync(typeof(DecompileSample));
		typeNode.IsExpanded = true;
		var baseTypes = typeNode.Children.OfType<BaseTypesTreeNode>().Single();

		// Act — select the grouping node, wait for decompile, navigate elsewhere so the
		// grouping lands on the back stack.
		vm.AssemblyTreeModel.SelectNode(baseTypes);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("sample-type-decompiled");

		// Open the back-history flyout and read the entry header.
		var backSplit = window.GetVisualDescendants().OfType<SplitButton>()
			.Single(sb => sb.Name == "BackSplitButton");
		var flyout = (MenuFlyout)backSplit.Flyout!;
		flyout.ShowAt(backSplit);
		await Waiters.WaitForAsync(() => flyout.Items.OfType<MenuItem>().Any());
		TestCapture.Step("back-history-flyout-open");

		// Assert — the entry shows the richer "Base Types (System.Exception)" form, NOT the
		// bare "Base Types" Text.
		var items = flyout.Items.OfType<MenuItem>().ToList();
		var baseTypesEntry = items.First(i =>
			((string)i.Header!).StartsWith((string)baseTypes.Text));
		((string)baseTypesEntry.Header!).Should().Contain(nameof(DecompileSample),
			"the dropdown header must disambiguate generic grouping nodes via NavigationText");
		((string)baseTypesEntry.Header!).Should().NotBe((string)baseTypes.Text,
			"falling back to bare Text would leave the grouping nodes indistinguishable");
	}
}
