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
public class HeapTreeTests
{
	[AvaloniaTest]
	public async Task MetadataTreeNode_Surfaces_Four_Heap_Children_For_A_PE_Assembly()
	{
		// CLI metadata divides ancillary data into four heaps: #Strings (member/type names),
		// #US (literal user strings emitted by ldstr), #GUID (16-byte module/type IDs), and
		// #Blob (signatures, marshalling info, custom-attr blobs). All four should be
		// reachable from the Metadata folder regardless of whether the file's debug
		// metadata is present.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();

		metadataNode.Children.OfType<StringHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<UserStringHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<GuidHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<BlobHeapTreeNode>().Should().ContainSingle();
	}

	[AvaloniaTest]
	public async Task StringHeapTreeNode_Decompiles_Header_Plus_Preview_Of_Entries()
	{
		// CoreLib's #Strings holds tens of thousands of entries; rendering all of them as
		// text would bloat the tab and bog down the editor. Phase 1 dumps a capped preview
		// (header + first N rows + truncation footer) so the user can scan the start of the
		// heap; Phase 2 swaps to a virtualising DataGrid for the full set.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var heapNode = metadataNode.Children.OfType<StringHeapTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("String Heap");
		tab.Text.Should().Contain("Offset");
		tab.Text.Should().Contain("Length");
		tab.Text.Should().Contain("Value");
		tab.Text.Should().Contain("more entries", "CoreLib's #Strings is well over the preview cap");
		// The preview includes a (count entries) header — should match e.g. "(24573 entries)".
		tab.Text.Should().MatchRegex(@"\(\d{3,} entries\)");
	}

	[AvaloniaTest]
	public async Task GuidHeapTreeNode_Decompiles_Header_Plus_Index_Length_Value_Columns()
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
		var heapNode = metadataNode.Children.OfType<GuidHeapTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Guid Heap");
		tab.Text.Should().Contain("Index");
		tab.Text.Should().Contain("Length");
		tab.Text.Should().MatchRegex(@"\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b",
			"the GUID heap should contain at least one parsed GUID");
	}

	[AvaloniaTest]
	public async Task BlobHeapTreeNode_Decompiles_Header_Plus_Preview_Of_Hex_Encoded_Entries()
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
		var heapNode = metadataNode.Children.OfType<BlobHeapTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		tab.Text.Should().Contain("Blob Heap");
		tab.Text.Should().Contain("Offset");
		tab.Text.Should().Contain("Length");
		tab.Text.Should().Contain("Value");
	}
}
