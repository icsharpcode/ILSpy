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

/// <summary>
/// Pins the typed-children breakdown of <see cref="DebugDirectoryTreeNode"/>: CodeView
/// + PdbChecksum entries get specialised tree nodes that surface their fields, other
/// entry types fall back to a generic node with a hex-dump <c>Decompile</c>. CoreLib's
/// PE always ships at least a CodeView entry plus a Reproducible entry, which lets us
/// validate both the typed and the fallback paths against a real assembly.
/// </summary>
[TestFixture]
public class DebugDirectoryChildNodesTests
{
	[AvaloniaTest]
	public async Task DebugDirectoryTreeNode_Surfaces_CodeViewTreeNode_For_The_CodeView_Entry()
	{
		var debugDir = await GetDebugDirectoryForCoreLib();

		// CoreLib ships at least one CodeView entry (private + public PDB descriptions
		// can produce multiple); the contract is "every CodeView entry surfaces as a
		// typed child", not a specific count.
		debugDir.Children.OfType<CodeViewTreeNode>().Should().NotBeEmpty(
			"every System.Private.CoreLib build ships at least one CodeView debug-directory entry");
	}

	[AvaloniaTest]
	public async Task DebugDirectoryTreeNode_Surfaces_PdbChecksumTreeNode_For_PdbChecksum_Entries()
	{
		var debugDir = await GetDebugDirectoryForCoreLib();

		debugDir.Children.OfType<PdbChecksumTreeNode>().Should().NotBeEmpty(
			"modern CoreLib builds ship one or more PdbChecksum debug-directory entries");
		var sum = debugDir.Children.OfType<PdbChecksumTreeNode>().First();
		sum.AlgorithmName.Should().NotBeNullOrEmpty();
		sum.Checksum.Should().NotBeNullOrEmpty();
	}

	[AvaloniaTest]
	public async Task DebugDirectoryTreeNode_Falls_Back_To_Generic_Entry_For_Unspecialised_Types()
	{
		// Reproducible entries are zero-payload and have no specialised viewer; they should
		// still surface as DebugDirectoryEntryTreeNode children so the user can see them in
		// the tree alongside the typed entries.
		var debugDir = await GetDebugDirectoryForCoreLib();

		var generic = debugDir.Children.OfType<DebugDirectoryEntryTreeNode>()
			.Where(n => n.GetType() == typeof(DebugDirectoryEntryTreeNode))
			.ToList();
		generic.Should().NotBeEmpty("expected at least one fallback entry (e.g. Reproducible)");
	}

	[AvaloniaTest]
	public async Task Every_Entry_In_The_PE_Debug_Directory_Becomes_A_Child_Node()
	{
		// Sanity check: no entry is silently dropped. The child count must match the raw
		// directory's entry count exactly (modulo embedded-PDB BadImageFormat skips, which
		// don't happen on healthy CoreLib).
		var (debugDir, peEntryCount) = await GetDebugDirectoryAndRawCountForCoreLib();

		debugDir.Children.Count.Should().Be(peEntryCount);
	}

	// --- helpers ---------------------------------------------------------------------------

	static async Task<DebugDirectoryTreeNode> GetDebugDirectoryForCoreLib()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var debugDir = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<DebugDirectoryTreeNode>();
		debugDir.EnsureLazyChildren();
		TestCapture.Step("debug-directory-children");
		return debugDir;
	}

	static async Task<(DebugDirectoryTreeNode node, int rawCount)> GetDebugDirectoryAndRawCountForCoreLib()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = (ICSharpCode.Decompiler.Metadata.PEFile)assemblyNode.LoadedAssembly.GetMetadataFileOrNull()!;
		int rawCount = module.Reader.ReadDebugDirectory().Length;

		var debugDir = assemblyNode
			.GetChild<MetadataTreeNode>()
			.GetChild<DebugDirectoryTreeNode>();
		debugDir.EnsureLazyChildren();
		TestCapture.Step("debug-directory-children");
		return (debugDir, rawCount);
	}
}
