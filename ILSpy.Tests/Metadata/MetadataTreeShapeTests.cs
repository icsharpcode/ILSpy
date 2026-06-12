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
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataTreeShapeTests
{
	[AvaloniaTest]
	public async Task AssemblyTreeNode_Surfaces_A_Metadata_Folder_For_Every_Loaded_PE()
	{
		// Each loaded PE assembly should expose a "Metadata" folder under its tree node so
		// users can navigate into the raw CLI tables / heaps / PE headers. Verifies the folder
		// is created, sits before the References folder (matching WPF ordering), and is a
		// MetadataTreeNode instance ready to lazy-load its children.

		// Arrange — boot, wait for assemblies, expand CoreLib (always a PE).
		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();

		// Assert — exactly one MetadataTreeNode child and it precedes the References folder.
		var metadataNode = assemblyNode.GetChild<MetadataTreeNode>();
		var children = assemblyNode.Children.ToList();
		TestCapture.Step("corelib-children");
		children.IndexOf(metadataNode).Should().BeLessThan(
			children.IndexOf(assemblyNode.Children.OfType<ReferenceFolderTreeNode>().Single()),
			"Metadata folder should sit above References, mirroring WPF AssemblyTreeNode ordering");
	}

	[AvaloniaTest]
	public async Task MetadataTreeNode_Decompiles_To_A_Summary_Of_Tables_And_Metadata_Kind()
	{
		// Selecting the Metadata folder should produce a text summary listing the metadata
		// kind, version, and a row count per non-empty CLI table. WPF emits this via
		// DumpMetadataInfo; the Avalonia port piggybacks on the existing decompiler tab
		// pipeline rather than introducing a new tab type for v1.

		// Arrange — boot, find the Metadata folder under CoreLib.
		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();

		// Act — select it through the helper that mirrors a click (loads lazy children + assigns
		// SelectedItem), wait for the decompile to finish.
		vm.AssemblyTreeModel.SelectNode(metadataNode);
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("metadata-summary-text");

		// Assert — text contains the metadata kind line and at least one table row count.
		tab.Text.Should().Contain("MetadataKind:");
		tab.Text.Should().Contain("MetadataVersion:");
		tab.Text.Should().Contain("Tables:");
		// CoreLib has thousands of TypeDef rows; pick a phrase from the table list that's
		// effectively guaranteed for any non-empty PE.
		tab.Text.Should().MatchRegex(@"\bTypeDef: \d+ rows\b");
	}
}
