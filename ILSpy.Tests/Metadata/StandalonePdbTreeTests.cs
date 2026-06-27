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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.DebugTables;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class StandalonePdbTreeTests
{
	[AvaloniaTest]
	public async Task Opening_A_Standalone_Portable_PDB_Surfaces_Its_Metadata_Tables_And_Heaps()
	{
		// A standalone .pdb (no companion assembly) loads as a metadata-only MetadataFile of
		// kind ProgramDebugDatabase. Its tree node must expand to the PDB's metadata tables and
		// heaps directly under the file node; before the fix the node had an expander but zero
		// children (the "empty tree" symptom).

		// The test SDK builds this assembly with a sidecar portable PDB, so use that PDB on its
		// own as the standalone fixture.
		var pdbPath = Path.ChangeExtension(typeof(StandalonePdbTreeTests).Assembly.Location, ".pdb");
		File.Exists(pdbPath).Should().BeTrue(
			$"the test assembly must ship a sidecar portable PDB to use as a standalone fixture (looked for '{pdbPath}')");

		var (_, vm) = await TestHarness.BootAsync();

		var loaded = await vm.OpenAssemblyAsync(pdbPath);
		TestCapture.Step("opened-standalone-pdb");

		var pdbNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(loaded.ShortName);
		pdbNode.EnsureLazyChildren();
		TestCapture.Step("pdb-node-expanded");

		pdbNode.Children.Should().NotBeEmpty(
			"a standalone portable PDB must populate its tree node instead of rendering empty");

		var tables = pdbNode.GetChild<MetadataTablesTreeNode>();
		tables.EnsureLazyChildren();
		tables.Children.OfType<DocumentTableTreeNode>().Should().NotBeEmpty(
			"the standalone PDB's Tables subtree must expose the Document table — the lead debug-only table that always has rows in a populated PDB");

		// The heap nodes are also surfaced directly (string/userstring/guid/blob).
		pdbNode.Children.OfType<StringHeapTreeNode>().Should().ContainSingle();
		pdbNode.Children.OfType<BlobHeapTreeNode>().Should().ContainSingle();
	}
}
