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
using ILSpy.Metadata;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class PEHeaderTreeTests
{
	[AvaloniaTest]
	public async Task MetadataTreeNode_Surfaces_Five_PE_Header_Children_For_A_PE_Assembly()
	{
		// The Metadata folder under a PE assembly should expose five PE-format leaves:
		// DOS / COFF / Optional / DataDirectories / DebugDirectory. Order mirrors WPF so a
		// user shifting between hosts sees the same tree shape. Each node is the entry point
		// to a header / table view; Phase 1 renders text, Phase 2 swaps to a DataGrid.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();

		metadataNode.Children.OfType<DosHeaderTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<CoffHeaderTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<OptionalHeaderTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<DataDirectoriesTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<DebugDirectoryTreeNode>().Should().ContainSingle();

		var orderedTypes = metadataNode.Children.Take(5).Select(c => c.GetType()).ToList();
		orderedTypes.Should().Equal(
			typeof(DosHeaderTreeNode),
			typeof(CoffHeaderTreeNode),
			typeof(OptionalHeaderTreeNode),
			typeof(DataDirectoriesTreeNode),
			typeof(DebugDirectoryTreeNode));
	}

	[AvaloniaTest]
	public async Task DosHeaderTreeNode_Decompiles_To_A_Table_With_All_31_DOS_Header_Fields()
	{
		// DOS header is a fixed 64-byte block at offset 0; the 31 fields are well-known and
		// stable across every PE file. Selecting the node should produce a text table that
		// every reader can scan, with the magic-number row visible by both its raw value and
		// its semantic meaning.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var dosNode = metadataNode.Children.OfType<DosHeaderTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(dosNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("e_magic");
		tab.Text.Should().Contain("Magic Number (MZ)");
		tab.Text.Should().Contain("e_lfanew");
		tab.Text.Should().Contain("File address of new exe header");
		// All 31 DOS-header field rows should be present — count e_ prefixes on member rows.
		var fieldCount = System.Text.RegularExpressions.Regex.Matches(tab.Text, @"\be_[a-z0-9_\[\]]+").Count;
		fieldCount.Should().BeGreaterThanOrEqualTo(31);
	}

	[AvaloniaTest]
	public async Task CoffHeaderTreeNode_Decompiles_To_A_Table_Including_Machine_And_Characteristics()
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
		var coffNode = metadataNode.Children.OfType<CoffHeaderTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(coffNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Machine");
		tab.Text.Should().Contain("Number of Sections");
		tab.Text.Should().Contain("Characteristics");
	}
}
