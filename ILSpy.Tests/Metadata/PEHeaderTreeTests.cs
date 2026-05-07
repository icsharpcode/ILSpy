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
	public async Task DosHeaderTreeNode_Opens_A_DataGrid_Tab_With_Entry_Columns_And_31_Rows()
	{
		// DOS header is a fixed 64-byte block at offset 0; the 31 fields are well-known and
		// stable across every PE file. Selecting the node opens a metadata-table tab — one
		// row per field, columns reflected from the Entry shape (Member / Offset / Size /
		// Value / Meaning).

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
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		tab.Title.Should().Be("DOS Header");
		tab.Items.Should().HaveCount(31);
		tab.Columns.Select(c => c.Header.ToString()).Should().Contain(["Member", "Offset", "Size", "Value", "Meaning"]);

		var firstRow = (Entry)tab.Items[0];
		firstRow.Member.Should().Be("e_magic");
		firstRow.Meaning.Should().Be("Magic Number (MZ)");

		var lastRow = (Entry)tab.Items[^1];
		lastRow.Member.Should().Be("e_lfanew");
	}

	[AvaloniaTest]
	public async Task CoffHeaderTreeNode_Opens_A_DataGrid_Tab_With_Machine_And_Characteristics_Rows()
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
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		tab.Title.Should().Be("COFF Header");
		var members = tab.Items.Cast<Entry>().Select(e => e.Member).ToList();
		members.Should().Contain(["Machine", "Number of Sections", "Characteristics"]);
	}
}
