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

using ILSpy.AppEnv;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompilerViewTests
{
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
	public async Task Selecting_References_Folder_Decompiles_References_Listing()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var refFolder = assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single();

		vm.AssemblyTreeModel.SelectedItem = refFolder;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Detected TargetFramework-Id:");
		tab.Text.Should().Contain("Detected RuntimePack:");
		tab.Text.Should().Contain("Referenced assemblies (in metadata order):");
		tab.Text.Should().Contain("System.Runtime");
		tab.Text.Should().Contain("Assembly load log including transitive references:");
	}

	[AvaloniaTest]
	public async Task Selecting_Type_Node_Decompiles_Type_Definition()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");

		vm.AssemblyTreeModel.SelectedItem = typeNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("class Version");
		tab.Text.Should().Contain("Major");
		tab.Text.Should().Contain("Minor");
	}

	[AvaloniaTest]
	public async Task Selecting_Property_Node_Decompiles_Property()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Version");
		typeNode.EnsureLazyChildren();
		var propertyNode = typeNode.Children.OfType<PropertyTreeNode>()
			.Single(p => p.PropertyDefinition.Name == "Major");

		vm.AssemblyTreeModel.SelectedItem = propertyNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("public int Major");
	}

	[AvaloniaTest]
	public async Task Selecting_Field_Node_Decompiles_Field()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.Math");
		typeNode.EnsureLazyChildren();
		var fieldNode = typeNode.Children.OfType<FieldTreeNode>()
			.Single(f => f.FieldDefinition.Name == "PI");

		vm.AssemblyTreeModel.SelectedItem = fieldNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("public const double PI");
		tab.Text.Should().Contain("3.14159");
	}

	[AvaloniaTest]
	public async Task Selecting_Event_Node_Decompiles_Event()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLib, "System", "System.AppDomain");
		typeNode.EnsureLazyChildren();
		var eventNode = typeNode.Children.OfType<EventTreeNode>()
			.Single(e => e.EventDefinition.Name == "ProcessExit");

		vm.AssemblyTreeModel.SelectedItem = eventNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("event EventHandler");
		tab.Text.Should().Contain("ProcessExit");
	}

	[AvaloniaTest]
	public async Task Selecting_Namespace_Node_Decompiles_Namespace()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var coreLib = typeof(object).Assembly.GetName().Name!;
		var namespaceNode = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			coreLib, "System.Runtime.Versioning");

		vm.AssemblyTreeModel.SelectedItem = namespaceNode;
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("TargetFrameworkAttribute");
		tab.Text.Should().Contain("SupportedOSPlatformAttribute");
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
}
