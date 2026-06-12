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
using Avalonia.Media;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataNodeIconParityTests
{
	[AvaloniaTest]
	public async Task Metadata_Tree_Nodes_Use_The_WPF_Equivalent_Icons()
	{
		// The PE/metadata tree nodes had distinct icons in WPF (a Metadata glyph for the root,
		// a Header glyph for the PE-header nodes, a Heap glyph for the heaps, a table-group
		// glyph for "Tables", folder glyphs for the directory containers). The Avalonia port
		// had collapsed several of these onto the generic MetadataTable icon. This guards the
		// restored per-node parity.

		// Arrange — boot, expand CoreLib's Metadata folder.
		var (_, vm) = await TestHarness.BootAsync();

		var metadataNode = vm.AssemblyTreeModel.FindCoreLib().GetChild<MetadataTreeNode>();
		metadataNode.EnsureLazyChildren();
		TestCapture.Step("metadata-node-expanded");
		var children = metadataNode.Children;

		// Assert — root + PE header nodes.
		metadataNode.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.Metadata, "the Metadata root uses the Metadata glyph");
		children.OfType<DosHeaderTreeNode>().Single().Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.Header);
		children.OfType<CoffHeaderTreeNode>().Single().Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.Header);
		children.OfType<OptionalHeaderTreeNode>().Single().Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.Header);

		// Assert — the directory-container nodes use the list-folder glyphs (closed + open on
		// expand), as WPF shipped. These are the folder-with-list-lines glyph, distinct from the
		// generic empty folder; the port had collapsed them onto the plain folder icon.
		var dataDirs = children.OfType<DataDirectoriesTreeNode>().Single();
		dataDirs.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.ListFolder);
		dataDirs.ExpandedIcon.Should().BeSameAs(ICSharpCode.ILSpy.Images.ListFolderOpen);

		var debugDir = children.OfType<DebugDirectoryTreeNode>().Single();
		debugDir.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.ListFolder);
		debugDir.ExpandedIcon.Should().BeSameAs(ICSharpCode.ILSpy.Images.ListFolderOpen);

		// Assert — "Tables" uses the table-group glyph, distinct from a single table.
		var tables = children.OfType<MetadataTablesTreeNode>().Single();
		tables.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.MetadataTableGroup);
		tables.Icon.Should().NotBeSameAs(ICSharpCode.ILSpy.Images.MetadataTable);

		// Assert — every heap node (String/UserString/Guid/Blob) uses the Heap glyph.
		var heaps = children.OfType<MetadataHeapTreeNode>().ToList();
		heaps.Should().NotBeEmpty("CoreLib exposes the String/UserString/Guid/Blob heaps");
		heaps.Should().OnlyContain(h => ReferenceEquals(h.Icon, ICSharpCode.ILSpy.Images.Heap));

		// Assert — the individual table nodes still use the single-table glyph (base unchanged).
		tables.EnsureLazyChildren();
		var aTable = tables.Children.OfType<MetadataTableTreeNode>().First();
		aTable.Icon.Should().BeSameAs(ICSharpCode.ILSpy.Images.MetadataTable);

		// Assert — the newly ported assets actually load (LoadSvg yields a usable image, not null).
		foreach (var img in new IImage[] { ICSharpCode.ILSpy.Images.Metadata, ICSharpCode.ILSpy.Images.Header, ICSharpCode.ILSpy.Images.Heap, ICSharpCode.ILSpy.Images.MetadataTableGroup, ICSharpCode.ILSpy.Images.ListFolder, ICSharpCode.ILSpy.Images.ListFolderOpen })
		{
			((object?)img).Should().NotBeNull();
			img.Size.Width.Should().BeGreaterThan(0, "a ported SVG must decode to a non-zero-size image");
		}
	}
}
