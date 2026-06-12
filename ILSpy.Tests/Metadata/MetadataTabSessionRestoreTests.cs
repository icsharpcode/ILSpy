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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataTabSessionRestoreTests
{
	[AvaloniaTest]
	public async Task Selecting_A_Metadata_Table_Saves_Its_Path_And_Round_Trips_Through_FindNodeByPath()
	{
		// SessionSettings.ActiveTreeViewPath records the selection on every change so a
		// restart (or a tree-path lookup) can land back on the same node. The metadata
		// sub-tree's ToString() values must be stable for that round-trip to work.

		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		TestCapture.Step("selected-typedef-table");

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings;
		var savedPath = settings.ActiveTreeViewPath;
		savedPath.Should().NotBeNull("selecting a tree node must populate SessionSettings.ActiveTreeViewPath");
		savedPath!.Should().EndWith(new[] { "Metadata: Metadata", "Tables", "TypeDef" },
			"the saved path mirrors the metadata sub-tree's stable ToString chain");

		// Round-trip the saved path through the tree's lookup helper — what the restart
		// code path uses. The walked node must be the very same TypeDef table node.
		var restored = vm.AssemblyTreeModel.FindNodeByPath(savedPath, returnBestMatch: false);
		ReferenceEquals(restored, typeDefNode).Should().BeTrue(
			"FindNodeByPath must resolve the saved path back to the original metadata-table node");
	}

	[AvaloniaTest]
	public async Task Embedded_PDB_Metadata_Path_Round_Trips_Through_FindNodeByPath()
	{
		// Embedded PDB sub-trees nest a second MetadataTreeNode under DebugDirectoryTreeNode.
		// The path-restore must walk through both Metadata folders and land on a debug-only
		// table.

		var (_, vm) = await TestHarness.BootAsync();

		var testDllPath = typeof(ICSharpCode.Decompiler.Metadata.MetadataFile).Assembly.Location;
		var loaded = await vm.OpenAssemblyAsync(testDllPath);
		TestCapture.Step("opened-decompiler-dll");

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		// AssemblyTreeNode surfaces the embedded PDB's metadata as a second top-level
		// MetadataTreeNode sibling, so disambiguate by label — this test exercises the
		// nested-under-DebugDirectory path.
		var documentTable = assemblyNode
			.GetChild<MetadataTreeNode>(n => (string?)n.Text == ICSharpCode.ILSpy.Properties.Resources.Metadata)
			.GetChild<DebugDirectoryTreeNode>()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<ICSharpCode.ILSpy.Metadata.DebugTables.DocumentTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(documentTable);
		TestCapture.Step("selected-document-table");

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings;
		var savedPath = settings.ActiveTreeViewPath;
		savedPath!.Should().Contain("Metadata: Debug Metadata (Embedded)",
			"the embedded PDB's MetadataTreeNode title must appear in the saved path so a restart can re-walk into it");
		savedPath.Should().EndWith(new[] { "Document" });

		var restored = vm.AssemblyTreeModel.FindNodeByPath(savedPath, returnBestMatch: false);
		ReferenceEquals(restored, documentTable).Should().BeTrue();
	}
}
