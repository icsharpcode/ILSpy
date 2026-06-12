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
public class PEHeaderTreeTests
{
	[AvaloniaTest]
	public async Task MetadataTreeNode_Surfaces_Five_PE_Header_Children_For_A_PE_Assembly()
	{
		// The Metadata folder under a PE assembly should expose five PE-format leaves:
		// DOS / COFF / Optional / DataDirectories / DebugDirectory. Order mirrors WPF so a
		// user shifting between hosts sees the same tree shape. Each node is the entry point
		// to a header / table view; Phase 1 renders text, Phase 2 swaps to a DataGrid.

		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();
		metadataNode.EnsureLazyChildren();
		TestCapture.Step("metadata-node-expanded");

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

		var (_, vm) = await TestHarness.BootAsync();

		var dosNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<DosHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(dosNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid");

		tab.Title.Should().Be("DOS Header");
		tab.Items.Should().HaveCount(31);
		tab.Columns.Select(c => c.Tag).Should().Contain(["Member", "Offset", "Size", "Value", "Meaning"]);

		var firstRow = (Entry)tab.Items[0];
		firstRow.Member.Should().Be("e_magic");
		firstRow.Meaning.Should().Be("Magic Number (MZ)");

		var lastRow = (Entry)tab.Items[^1];
		lastRow.Member.Should().Be("e_lfanew");
	}

	[AvaloniaTest]
	public async Task Switching_From_Metadata_Node_Back_To_Entity_Node_Reactivates_Decompiler_Tab()
	{
		// Regression: with a metadata tab active, clicking back on a regular entity node was
		// leaving the metadata tab on screen because the docking host activated the
		// decompiler tab but never restored it as the user-visible page. Both round-trip
		// directions should swap the active dockable to the right tab type.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var dosNode = assemblyNode
			.GetChild<MetadataTreeNode>()
			.GetChild<DosHeaderTreeNode>();

		// Step 1 — pick a regular entity node (the assembly itself decompiles to its
		// AssemblyDef metadata via the decompiler text path). Decompiler tab should be active.
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("decompiler-tab");

		// Step 2 — pick the DOS-header metadata node. Metadata tab takes the active slot.
		vm.AssemblyTreeModel.SelectNode(dosNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid");

		// Step 3 — back to the entity node. The decompiler tab must come back into view.
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		var decompilerTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("decompiler-tab-restored");
		decompilerTab.Should().NotBeNull();

		var mainTab = ((ICSharpCode.ILSpy.Docking.ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.TextView.DecompilerTabPageModel>();
	}

	[AvaloniaTest]
	public async Task Starting_With_Metadata_Then_Picking_An_Entity_Brings_Up_The_Decompiler_Tab()
	{
		// Same regression as above but entered through the metadata side first — clicking
		// DOS Header before any entity has been selected, then back to an entity. Verifies
		// the docking host can lazily create and activate the decompiler tab even when the
		// document area starts out holding only the metadata grid.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var dosNode = assemblyNode
			.GetChild<MetadataTreeNode>()
			.GetChild<DosHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(dosNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid");

		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("decompiler-tab");

		var mainTab = ((ICSharpCode.ILSpy.Docking.ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.TextView.DecompilerTabPageModel>();
	}

	[AvaloniaTest]
	public async Task Round_Tripping_Metadata_To_Entity_Reuses_The_Same_Document_Instance()
	{
		// The document area holds a single persistent ContentTabPage for the lifetime of
		// the app; only its inner Content swaps between decompiler / metadata viewmodels.
		// Keeping the Document instance stable is what makes the visual swap actually
		// happen on content-type changes — replacing the dockable in place leaves the
		// previous inner view rendered (the user would see a "Decompiling" spinner that
		// never goes away because the prior decompiler tab is still on screen).

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var metadataNode = assemblyNode.GetChild<MetadataTreeNode>();
		var dosNode = metadataNode.GetChild<DosHeaderTreeNode>();
		var coffNode = metadataNode.GetChild<CoffHeaderTreeNode>();
		var factoryFactory = (ICSharpCode.ILSpy.Docking.ILSpyDockFactory)vm.DockWorkspace.Factory;
		var documents = factoryFactory.Documents!;
		var mainTab = factoryFactory.MainTab!;

		vm.AssemblyTreeModel.SelectNode(dosNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid");
		documents.VisibleDockables!.Should().HaveCount(1).And.Contain(mainTab);
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.ViewModels.MetadataTablePageModel>();

		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("decompiler-tab");
		documents.VisibleDockables!.Should().HaveCount(1).And.Contain(mainTab);
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.TextView.DecompilerTabPageModel>();

		vm.AssemblyTreeModel.SelectNode(coffNode);
		await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("coff-header-grid");
		documents.VisibleDockables!.Should().HaveCount(1).And.Contain(mainTab);
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.ViewModels.MetadataTablePageModel>();

		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		documents.VisibleDockables!.Should().HaveCount(1).And.Contain(mainTab);
		mainTab.Content.Should().BeOfType<ICSharpCode.ILSpy.TextView.DecompilerTabPageModel>();
	}

	[AvaloniaTest]
	public async Task Selecting_A_Second_Metadata_Node_Reuses_The_Existing_Grid_Tab()
	{
		// Clicking DOS Header → COFF Header → DOS Header should leave the dock with a
		// single metadata tab whose state has been updated each time, mirroring how the
		// decompiler tab reuses itself across tree-node selections. Otherwise every click
		// piles up a new dockable and the tab strip grows without bound.

		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();

		var dosNode = metadataNode.GetChild<DosHeaderTreeNode>();
		var coffNode = metadataNode.GetChild<CoffHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(dosNode);
		var firstTab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid");
		firstTab.Title.Should().Be("DOS Header");

		vm.AssemblyTreeModel.SelectNode(coffNode);
		var secondTab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("coff-header-grid");
		secondTab.Should().BeSameAs(firstTab, "metadata clicks reuse the existing grid tab");
		secondTab.Title.Should().Be("COFF Header");

		vm.AssemblyTreeModel.SelectNode(dosNode);
		var thirdTab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("dos-header-grid-again");
		thirdTab.Should().BeSameAs(firstTab);
		thirdTab.Title.Should().Be("DOS Header");

		var mainTab = ((ICSharpCode.ILSpy.Docking.ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		mainTab.Content.Should().BeSameAs(firstTab, "the inner metadata viewmodel is reused in place");
	}

	[AvaloniaTest]
	public async Task CoffHeaderTreeNode_Opens_A_DataGrid_Tab_With_Machine_And_Characteristics_Rows()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var coffNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<CoffHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(coffNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("coff-header-grid");

		tab.Title.Should().Be("COFF Header");
		var members = tab.Items.Cast<Entry>().Select(e => e.Member).ToList();
		members.Should().Contain(["Machine", "Number of Sections", "Characteristics"]);
	}
}
