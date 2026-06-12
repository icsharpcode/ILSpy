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
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class TypedMetadataTableTreeTests
{
	[AvaloniaTest]
	public async Task AssemblyRefTableTreeNode_Opens_A_Grid_With_One_Row_Per_AssemblyReference()
	{
		// CoreLib's AssemblyRef table is empty (CoreLib is the bottom of the dep chain), so
		// target System.Linq instead. Selecting the typed table node opens a metadata-grid
		// tab whose Items list mirrors the table's rows; a known reference's display name
		// must show up on at least one row.

		var (_, vm) = await TestHarness.BootAsync(3);

		var assemblyRefNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq")
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<AssemblyRefTableTreeNode>();

		assemblyRefNode.Kind.Should().Be(TableIndex.AssemblyRef);
		assemblyRefNode.RowCount.Should().BeGreaterThan(0);

		vm.AssemblyTreeModel.SelectNode(assemblyRefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("assemblyref-grid");

		tab.Title.Should().Contain("AssemblyRef"); // title now leads with the token-kind byte
		tab.Items.Should().HaveCount(assemblyRefNode.RowCount);
		var names = tab.Items.Cast<AssemblyRefTableTreeNode.AssemblyRefEntry>()
			.Select(e => e.Name)
			.ToList();
		names.Should().IntersectWith(new[] { "System.Runtime", "netstandard", "System.Private.CoreLib", "System.Threading" });
	}

	[AvaloniaTest]
	public async Task TypeDefTableTreeNode_Opens_A_Grid_Including_System_Object()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		typeDefNode.Kind.Should().Be(TableIndex.TypeDef);
		typeDefNode.RowCount.Should().BeGreaterThan(1000);

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");

		tab.Title.Should().Contain("TypeDef"); // title now leads with the token-kind byte
		tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.Should().Contain(e => e.Name == "Object");
	}

	[AvaloniaTest]
	public async Task FieldAndMethodTables_Are_Available_As_Typed_Leaves()
	{
		// Field / MethodDef are by far the largest tables in any real assembly. The typed
		// leaves should report the right kind and expose row data through the grid view.

		var (_, vm) = await TestHarness.BootAsync();

		var tablesNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();
		tablesNode.EnsureLazyChildren();
		TestCapture.Step("tables-tree-expanded");

		tablesNode.Children.OfType<FieldTableTreeNode>().Should().ContainSingle();
		tablesNode.Children.OfType<MethodTableTreeNode>().Should().ContainSingle();
		tablesNode.Children.OfType<ParamTableTreeNode>().Should().ContainSingle();
		tablesNode.Children.OfType<MemberRefTableTreeNode>().Should().ContainSingle();
		tablesNode.Children.OfType<CustomAttributeTableTreeNode>().Should().ContainSingle();
		tablesNode.Children.OfType<AssemblyTableTreeNode>().Should().ContainSingle();

		var methodNode = tablesNode.Children.OfType<MethodTableTreeNode>().Single();
		vm.AssemblyTreeModel.SelectNode(methodNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("methoddef-grid");
		tab.Title.Should().Contain("MethodDef"); // title now leads with the token-kind byte
		tab.Columns.Select(c => c.Tag).Should().Contain("RID");
	}

	[AvaloniaTest]
	public async Task ModuleTableTreeNode_Opens_A_Grid_With_The_Single_Module_Row()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var moduleNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<ModuleTableTreeNode>();

		moduleNode.RowCount.Should().Be(1, "Module is a one-row table by spec");

		vm.AssemblyTreeModel.SelectNode(moduleNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("module-grid");

		tab.Title.Should().Contain("Module"); // title now leads with the token-kind byte
		tab.Items.Should().HaveCount(1);
		var row = (ModuleTableTreeNode.ModuleEntry)tab.Items[0];
		row.Name.Should().EndWith(".dll");
	}
}
