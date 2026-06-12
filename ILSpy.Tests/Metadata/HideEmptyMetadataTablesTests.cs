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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class HideEmptyMetadataTablesTests
{
	[AvaloniaTest]
	public async Task Default_Setting_Hides_Tables_With_Zero_Rows()
	{
		// Default DisplaySettings.HideEmptyMetadataTables = true: every child of the
		// Tables sub-tree must correspond to a non-zero CLI table.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.HideEmptyMetadataTables = true;

		var (_, vm) = await TestHarness.BootAsync();

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var tables = LoadTablesNode(vm, coreLibName, out var metadata);

		tables.Children.OfType<MetadataTableTreeNode>().Should().AllSatisfy(t =>
			metadata.GetTableRowCount(t.Kind).Should().BeGreaterThan(0,
				"with HideEmptyMetadataTables=true, every visible table must have at least one row"));
	}

	[AvaloniaTest]
	public async Task Disabling_The_Setting_Surfaces_Every_Table_Even_When_Empty()
	{
		// Flip HideEmptyMetadataTables off and confirm a table that's known empty for
		// CoreLib (FieldRva, which is rare in modern managed code) shows up among the
		// children.

		var settings = AppComposition.Current.GetExport<SettingsService>().DisplaySettings;
		settings.HideEmptyMetadataTables = false;
		try
		{
			var (_, vm) = await TestHarness.BootAsync();

			var coreLibName = typeof(object).Assembly.GetName().Name!;
			var tables = LoadTablesNode(vm, coreLibName, out var _);

			var visibleKinds = tables.Children.OfType<MetadataTableTreeNode>()
				.Select(t => t.Kind).ToHashSet();
			visibleKinds.Should().Contain(System.Enum.GetValues<TableIndex>(),
				"with HideEmptyMetadataTables=false, every CLI TableIndex must surface as a child");
		}
		finally
		{
			settings.HideEmptyMetadataTables = true;
		}
	}

	static MetadataTablesTreeNode LoadTablesNode(MainWindowViewModel vm, string assemblyName,
		out System.Reflection.Metadata.MetadataReader metadata)
	{
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(assemblyName);
		var tables = assemblyNode
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();
		tables.EnsureLazyChildren();
		TestCapture.Step("tables-tree-expanded");
		metadata = assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!.Metadata;
		return tables;
	}
}
