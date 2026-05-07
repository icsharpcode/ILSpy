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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class TypedMetadataTableTreeTests
{
	[AvaloniaTest]
	public async Task AssemblyRefTableTreeNode_Decompiles_To_A_Row_Per_AssemblyReference()
	{
		// CoreLib's AssemblyRef table holds the assemblies it depends on (typically a
		// handful: System.Runtime, System.Threading, …). Selecting the typed table node
		// should render one row per reference with the reference's display name visible.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// CoreLib has zero AssemblyRefs (it's the bottom of the dep chain), so target
		// System.Linq instead — same approach as the broader assembly-tree tests.
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var assemblyRefNode = tablesNode.Children.OfType<AssemblyRefTableTreeNode>().Single();

		assemblyRefNode.Kind.Should().Be(TableIndex.AssemblyRef);
		assemblyRefNode.RowCount.Should().BeGreaterThan(0);

		vm.AssemblyTreeModel.SelectNode(assemblyRefNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("AssemblyRef");
		tab.Text.Should().Contain("Name");
		tab.Text.Should().Contain("Version");
		// CoreLib references almost always include System.Runtime or netstandard.
		tab.Text.Should().MatchRegex(@"\b(System\.Runtime|netstandard|System\.Private\.CoreLib|System\.Threading)\b");
	}

	[AvaloniaTest]
	public async Task TypeDefTableTreeNode_Decompiles_To_A_Row_Per_Type_Definition()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();

		typeDefNode.Kind.Should().Be(TableIndex.TypeDef);
		// CoreLib has thousands of type definitions.
		typeDefNode.RowCount.Should().BeGreaterThan(1000);

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("TypeDef");
		// Object is the very first entry every CLI assembly produces (after <Module>).
		tab.Text.Should().Contain("Object");
	}

	[AvaloniaTest]
	public async Task ModuleTableTreeNode_Decompiles_To_A_Single_Row_Carrying_The_Module_Name()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var moduleNode = tablesNode.Children.OfType<ModuleTableTreeNode>().Single();

		moduleNode.RowCount.Should().Be(1, "Module is a one-row table by spec");

		vm.AssemblyTreeModel.SelectNode(moduleNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Module");
		tab.Text.Should().Contain(".dll");
	}
}
