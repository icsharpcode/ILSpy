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

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.DebugTables;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class EmbeddedPdbTreeTests
{
	// audit (2026-05-12): catches changing the "Debug Metadata (Embedded)" label string in
	// DebugDirectoryTreeNode.cs (~line 90) — the Text equality assertion in the test fails.
	[AvaloniaTest]
	public async Task Expanding_DebugDirectory_Surfaces_Embedded_PDB_Metadata_Subtree()
	{
		// Modern .NET SDK Debug builds default to DebugType=embedded, so the test DLL
		// itself ships with an embedded portable PDB. Loading it confirms expanding the
		// host module's Debug Directory exposes a "Debug Metadata (Embedded)" node whose
		// own Tables subtree contains the PDB-only tables (Document and friends) — the
		// canonical entry point for browsing debug metadata in the live app.

		var (_, vm) = await TestHarness.BootAsync();

		// ICSharpCode.Decompiler ships with an embedded portable PDB (no separate .pdb in
		// the build output) — use it as the fixture rather than the test DLL itself, which
		// the test SDK builds with portable + sidecar PDB.
		var testDllPath = typeof(ICSharpCode.Decompiler.Metadata.MetadataFile).Assembly.Location;
		var loaded = await vm.OpenAssemblyAsync(testDllPath);
		TestCapture.Step("opened-decompiler-dll");

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		// AssemblyTreeNode now surfaces two MetadataTreeNode children for assemblies with an
		// embedded PDB (host metadata + a top-level PDB sibling). This test cares about the
		// nested-under-DebugDirectory path, so pick the host metadata explicitly by label.
		var metadataNode = assemblyNode.GetChild<MetadataTreeNode>(n => (string?)n.Text == Resources.Metadata);

		var debugDirectoryNode = metadataNode.GetChild<DebugDirectoryTreeNode>();
		debugDirectoryNode.EnsureLazyChildren();
		TestCapture.Step("debug-directory-expanded");

		// The embedded-PDB sub-tree is a MetadataTreeNode whose own children include a
		// MetadataTablesTreeNode listing the debug-only tables.
		var embeddedPdbNode = debugDirectoryNode.Children.OfType<MetadataTreeNode>().SingleOrDefault();
		(embeddedPdbNode is null).Should().BeFalse(
			"the test DLL ships with an embedded portable PDB, so the Debug Directory must expose its metadata as a nested MetadataTreeNode");
		embeddedPdbNode!.Text.Should().Be("Debug Metadata (Embedded)");

		var pdbTables = embeddedPdbNode.GetChild<MetadataTablesTreeNode>();
		pdbTables.EnsureLazyChildren();
		pdbTables.Children.OfType<DocumentTableTreeNode>().Should().NotBeEmpty(
			"the embedded PDB's Tables subtree must expose the Document table — the lead debug-only table that always has rows in a populated PDB");
	}

	[AvaloniaTest]
	public async Task Embedded_PDB_Metadata_Is_Exposed_As_A_Top_Level_Sibling_Of_The_Host_Metadata_Node()
	{
		// Sibling-discoverability path: the embedded PDB's metadata should also be reachable
		// directly under the AssemblyTreeNode (as a sibling of the host module's "Metadata"
		// folder), not only nested under DebugDirectory. Without this, users have to drill
		// three levels deep to reach PDB metadata — same content, harder to find.

		var (_, vm) = await TestHarness.BootAsync();

		var testDllPath = typeof(ICSharpCode.Decompiler.Metadata.MetadataFile).Assembly.Location;
		var loaded = await vm.OpenAssemblyAsync(testDllPath);
		TestCapture.Step("opened-decompiler-dll");

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		assemblyNode.EnsureLazyChildren();
		TestCapture.Step("assembly-node-expanded");

		var topLevelMetadataNodes = assemblyNode.Children.OfType<MetadataTreeNode>().ToList();
		topLevelMetadataNodes.Should().HaveCount(2,
			"the assembly node must expose two MetadataTreeNode children: the host's PE metadata "
			+ "and the embedded portable PDB's metadata as a sibling");
		topLevelMetadataNodes.Select(n => n.Text.ToString())
			.Should().Contain("Debug Metadata (Embedded)",
				"the embedded PDB sibling must use the same label as the nested one for consistency");
	}
}
