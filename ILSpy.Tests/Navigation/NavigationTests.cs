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
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var firstMethod = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		var secondMethod = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");

		vm.AssemblyTreeModel.SelectedItem = firstMethod;
		var firstTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		firstTab.Text.Should().Contain("AsEnumerable");

		// NavigationHistory collapses selections that happen within 0.5s into one entry. Wait
		// past that window so the second selection records a real back-history entry.
		await Task.Delay(600);

		vm.AssemblyTreeModel.SelectedItem = secondMethod;
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, secondMethod));
		var secondTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		secondTab.Text.Should().Contain("Empty");

		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
		vm.DockWorkspace.NavigateBackCommand.Execute(null);

		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, firstMethod));
		var restoredTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		restoredTab.Text.Should().Contain("AsEnumerable");
		restoredTab.Text.Should().Contain("return source");

		firstMethod.Should().Be().CenteredInView();
	}

	[AvaloniaTest]
	public async Task Back_SplitButton_Dropdown_Lists_History_And_Jumps_To_Selected_Entry()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodA = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		var methodB = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty");
		var methodC = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Range");

		// Three distinct selections with >0.6s gaps so each lands as its own entry on the back stack
		// (NavigationHistory collapses sub-0.5s rapid succession into one entry).
		vm.AssemblyTreeModel.SelectedItem = methodA;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectedItem = methodB;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		await Task.Delay(600);
		vm.AssemblyTreeModel.SelectedItem = methodC;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		// Open the Back SplitButton's flyout — the Opening handler populates the menu from the
		// current back history.
		var backSplit = window.GetVisualDescendants().OfType<SplitButton>()
			.Single(sb => sb.Name == "BackSplitButton");
		var flyout = (MenuFlyout)backSplit.Flyout!;
		flyout.ShowAt(backSplit);
		await Waiters.WaitForAsync(() => flyout.Items.OfType<MenuItem>().Count() >= 2);

		// Newest-first: index 0 is the immediate previous selection (methodB),
		// index 1 is the one before that (methodA).
		var items = flyout.Items.OfType<MenuItem>().ToList();
		((string)items[0].Header!).Should().Be((string)methodB.Text);
		((string)items[1].Header!).Should().Be((string)methodA.Text);
		items[1].CommandParameter.Should().BeSameAs(methodA);

		// Multi-step jump: clicking methodA pops two entries off the back stack in one go.
		items[1].Command!.Execute(items[1].CommandParameter);
		await Waiters.WaitForAsync(() => ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, methodA));

		// The two displaced entries (methodC, methodB) are now on the forward stack, so Forward
		// becomes available.
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue();
	}
}
