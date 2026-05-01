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

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MainWindowTests
{
	[AvaloniaTest]
	public void MainWindow_Resolves_From_Composition_And_Shows()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();

		window.IsVisible.Should().BeTrue();
		window.Title.Should().Be("ILSpy");
		window.DataContext.Should().BeOfType<MainWindowViewModel>();
	}

	[AvaloniaTest]
	public async Task Selecting_AsEnumerable_Method_Decompiles_Into_Document_View()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		methodNode.MethodDefinition.IsExtensionMethod.Should().BeTrue();

		vm.AssemblyTreeModel.SelectedItem = methodNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("IEnumerable<TSource>");
		tab.Text.Should().Contain("return source");

		methodNode.Should().Be().CenteredInView();
	}

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
	public async Task Selecting_Assembly_Node_Emits_Header_And_Assembly_Attributes()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectedItem = assemblyNode;

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("// " + assemblyNode.LoadedAssembly.FileName);
		tab.Text.Should().Contain("// System.Linq, Version=");
		tab.Text.Should().Contain("// Global type: <Module>");
		tab.Text.Should().Contain("// Architecture: ");
		tab.Text.Should().Contain("// Runtime: ");
		tab.Text.Should().Contain("[assembly: AssemblyVersion(");
	}

	[AvaloniaTest]
	public async Task Opening_Assembly_Adds_It_To_The_Tree()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var newAsmPath = typeof(System.Net.Http.HttpClient).Assembly.Location;
		var initialCount = vm.AssemblyTreeModel.AssemblyList!.Count;

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var openCommand = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._Open))
			.CreateExport().Value;
		openCommand.Execute(newAsmPath);

		await Waiters.WaitForAsync(() =>
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Any(a =>
				string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase)));

		var loaded = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => string.Equals(a.FileName, newAsmPath, System.StringComparison.OrdinalIgnoreCase));
		await loaded.GetLoadResultAsync();

		vm.AssemblyTreeModel.AssemblyList!.Count.Should().Be(initialCount + 1);
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		node.LoadedAssembly.Should().BeSameAs(loaded);
		node.IsSelected.Should().BeTrue();
	}
}
