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
using System.Resources.Extensions;
using System.Windows.Forms;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.ImageList;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// End-to-end coverage of the ImageList tree-node integration: factory dispatch,
/// lazy frame materialisation, and inline preview emission. The decoder unit tests
/// in <see cref="ImageListDecoderTests"/> cover the byte-format pipeline; these tests
/// cover the wiring around it.
/// </summary>
[TestFixture]
public class ImageListResourceEntryNodeTests
{
	// Same pattern as ResourceFactoryTests in the cross-platform suite: touch the
	// composition host once so ILSpyTreeNode's static ResourceNodeFactories list is
	// populated. AvaloniaTest boots the headless app, then this forces App.Initialize
	// → AppComposition.Initialize.
	static void EnsureComposition() => AppComposition.Current.GetExport<MainWindow>();

	[AvaloniaTest]
	public void ResourcesFileTreeNode_Surfaces_ImageListResourceEntryNode_For_ImageListStreamer()
	{
		// Build a .resources blob containing one ImageListStreamer entry, feed it through
		// the factory pipeline, expect a specialised ImageListResourceEntryNode rather than
		// the generic "<serialized>" fall-back.
		EnsureComposition();
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 3);
		var resourcesBytes = BuildResourcesWithSerializedEntry("MyImages", fixture.NrbfBlob, fixture.TypeName);

		var node = (ResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("strings.resources", resourcesBytes));
		node.EnsureLazyChildren();

		node.Children.Should().HaveCount(1, "the .resources file held exactly one entry");
		((object?)node.Children[0]).Should().BeOfType<ImageListResourceEntryNode>(
			"an ImageListStreamer typed entry should be claimed by ImageListResourceNodeFactory");
	}

	[AvaloniaTest]
	public void ResourcesFileTreeNode_Falls_Through_For_Unhandled_Serialised_Types()
	{
		// A serialized entry whose type-name isn't ImageListStreamer must NOT be claimed by
		// the ImageList factory; it should land in the generic OtherEntries collection so the
		// existing serialized-object representation still renders it.
		EnsureComposition();
		var unrelatedNrbf = ImageListFixtures.BuildNrbfClassWithByteArrayMember(
			"My.Custom.SerializedType", "MyAssembly", "Data", new byte[] { 1, 2, 3, 4 });
		var resourcesBytes = BuildResourcesWithSerializedEntry(
			"MyData", unrelatedNrbf, "My.Custom.SerializedType, MyAssembly");

		var node = (ResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("strings.resources", resourcesBytes));
		node.EnsureLazyChildren();

		node.Children.OfType<ImageListResourceEntryNode>().Should().BeEmpty(
			"non-ImageListStreamer serialized entries must not be claimed by the ImageList factory");
		node.OtherEntries.Should().HaveCount(1).And.Contain(e => e.Type.StartsWith("My.Custom.SerializedType"));
	}

	[AvaloniaTest]
	public void ImageListResourceEntryNode_LoadChildren_Materialises_One_Child_Per_Frame()
	{
		// LoadChildren should decode the NRBF blob lazily and produce one frame node per
		// frame, regardless of which depth the source ImageList used.
		EnsureComposition();
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 5);
		var node = new ImageListResourceEntryNode(
			"MyImages",
			() => new MemoryStream(fixture.NrbfBlob, writable: false));

		node.EnsureLazyChildren();

		node.Children.Should().HaveCount(5);
		node.Children.Should().AllBeOfType<ImageListFrameNode>(
			"every child of an ImageList parent node represents one decoded frame");
	}

	[AvaloniaTest]
	public void ImageListResourceEntryNode_Decompile_Writes_Inline_Preview_For_Parent_Selection()
	{
		// Selecting the parent itself (not a child frame) must produce an inline preview row.
		// We don't realise the Func<Control> here — the assertion is just "Decompile registered
		// one UI element for the parent view".
		EnsureComposition();
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 4);
		var node = new ImageListResourceEntryNode(
			"MyImages",
			() => new MemoryStream(fixture.NrbfBlob, writable: false));
		var output = new AvaloniaEditTextOutput();
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.UIElements.Should().HaveCount(1,
			"the parent's Decompile contributes exactly one inline preview panel");
	}

	/// <summary>
	/// Builds a .resources file containing a single serialized-object entry whose payload is
	/// the supplied NRBF blob. Uses <see cref="PreserializedResourceWriter"/> so we don't have
	/// to drive the obsolete <c>BinaryFormatter</c> just to wrap a payload in the right
	/// .resources type-code envelope.
	/// </summary>
	static byte[] BuildResourcesWithSerializedEntry(string key, byte[] nrbfBlob, string typeName)
	{
		using var ms = new MemoryStream();
		using (var writer = new PreserializedResourceWriter(ms))
		{
			// SYSLIB0011 — the API takes BinaryFormatter-shaped bytes (which our hand-rolled
			// NRBF blob is, by design) and packs them into a .resources type-code entry.
			// "Obsolete" just warns that consumers will need a deserializer that can handle
			// NRBF — exactly what ResourcesFile does. Suppress the warning at the call site.
#pragma warning disable SYSLIB0011
			writer.AddBinaryFormattedResource(key, nrbfBlob, typeName);
#pragma warning restore SYSLIB0011
			writer.Generate();
		}
		return ms.ToArray();
	}
}
