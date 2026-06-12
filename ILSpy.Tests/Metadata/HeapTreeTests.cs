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

using ICSharpCode.ILSpy.Metadata;

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

		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();
		metadataNode.EnsureLazyChildren();
		TestCapture.Step("metadata-node-expanded");

		metadataNode.Children.OfType<StringHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<UserStringHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<GuidHeapTreeNode>().Should().ContainSingle();
		metadataNode.Children.OfType<BlobHeapTreeNode>().Should().ContainSingle();
	}

	[AvaloniaTest]
	public async Task StringHeapTreeNode_Opens_Grid_Tab_With_Offset_Length_Value_Columns()
	{
		// CoreLib's #Strings holds tens of thousands of entries; the DataGrid surfaces every
		// row through Avalonia's built-in row virtualisation, so unlike the Phase 1 text
		// dump there's no preview cap and no truncation footer.

		var (_, vm) = await TestHarness.BootAsync();

		var heapNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<StringHeapTreeNode>();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("string-heap-grid");

		tab.Title.Should().Be("String Heap");
		tab.Columns.Select(c => c.Tag).Should().Equal("Offset", "Length", "Value");
		// CoreLib's #Strings is well over a thousand entries; the grid handles them all.
		tab.Items.Should().HaveCountGreaterThan(1000);
	}

	[AvaloniaTest]
	public async Task GuidHeapTreeNode_Opens_Grid_Tab_With_Index_Length_Value_Columns()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var heapNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<GuidHeapTreeNode>();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("guid-heap-grid");

		tab.Title.Should().Be("Guid Heap");
		tab.Columns.Select(c => c.Tag).Should().Equal("Index", "Length", "Value");
		// Each entry's Value is the parsed GUID's lowercase canonical form.
		var first = (GuidHeapTreeNode.GuidHeapEntry)tab.Items[0];
		first.Value.Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
	}

	[AvaloniaTest]
	public async Task BlobHeapTreeNode_Opens_Grid_Tab_With_Offset_Length_Value_Columns()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var heapNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<BlobHeapTreeNode>();

		vm.AssemblyTreeModel.SelectNode(heapNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("blob-heap-grid");

		tab.Title.Should().Be("Blob Heap");
		tab.Columns.Select(c => c.Tag).Should().Equal("Offset", "Length", "Value");
	}
}
