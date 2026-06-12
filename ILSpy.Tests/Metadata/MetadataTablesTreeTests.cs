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

using ICSharpCode.ILSpy.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataTablesTreeTests
{
	[AvaloniaTest]
	public async Task MetadataTreeNode_Surfaces_A_Tables_Container_Between_PE_Headers_And_Heaps()
	{
		// "Tables" is the synthetic folder that, once expanded, exposes one leaf per
		// non-empty CLI metadata table. Mirrors WPF tree shape: PE headers → Tables → heaps.
		// Phase 1 lazy-loads its children with a generic placeholder per table; Phase 1e
		// replaces them with per-table typed leaves carrying the actual row data.

		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();
		metadataNode.EnsureLazyChildren();
		TestCapture.Step("metadata-node-expanded");

		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		var children = metadataNode.Children.ToList();
		var debugDirIndex = children.IndexOf(metadataNode.Children.OfType<DebugDirectoryTreeNode>().Single());
		var stringHeapIndex = children.IndexOf(metadataNode.Children.OfType<StringHeapTreeNode>().Single());
		children.IndexOf(tablesNode).Should()
			.BeGreaterThan(debugDirIndex, "Tables sits below the PE-header leaves")
			.And.BeLessThan(stringHeapIndex, "Tables sits above the heap leaves");
	}

	[AvaloniaTest]
	public async Task MetadataTablesTreeNode_LazyLoads_One_Child_Per_NonEmpty_Table()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var tablesNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();
		tablesNode.EnsureLazyChildren();
		TestCapture.Step("tables-tree-expanded");

		var tableChildren = tablesNode.Children.OfType<MetadataTableTreeNode>().ToList();
		// CoreLib has well over 10 non-empty tables (Module, TypeRef, TypeDef, Field, ...).
		tableChildren.Should().HaveCountGreaterThan(10);
		// Module / TypeDef / MethodDef are present in every CLI assembly.
		tableChildren.Select(c => c.Kind).Should().Contain([TableIndex.Module, TableIndex.TypeDef, TableIndex.MethodDef]);
		// Each child reports a positive row count (matches the heuristic that empty tables are filtered out).
		tableChildren.Should().AllSatisfy(c => c.RowCount.Should().BeGreaterThan(0));
	}

	[AvaloniaTest]
	public async Task MetadataTablesTreeNode_Decompiles_To_A_Per_Table_Row_Count_Summary()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var tablesNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();

		vm.AssemblyTreeModel.SelectNode(tablesNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("tables-summary-text");

		tab.Text.Should().Contain("Tables");
		tab.Text.Should().MatchRegex(@"\bTypeDef\s*[:|]\s*\d+");
	}
}
