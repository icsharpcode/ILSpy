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

using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ResourceFactoryTests
{
	// Touch the composition host once so ILSpyTreeNode's static `ResourceNodeFactories`
	// initialiser succeeds. AvaloniaTest already boots the app builder; resolving the
	// MainWindow ensures App.Initialize has run and AppComposition.Current is wired.
	static void EnsureComposition() => AppComposition.Current.GetExport<MainWindow>();

	[AvaloniaTest]
	[TestCase("schema.xsd")]
	[TestCase("config.xml")]
	[TestCase("transform.xslt")]
	[TestCase("Window.xaml")]
	[TestCase("MainWindow.baml")]
	[TestCase("logo.png")]
	[TestCase("logo.gif")]
	[TestCase("logo.bmp")]
	[TestCase("logo.jpg")]
	[TestCase("favicon.ico")]
	[TestCase("pointer.cur")]
	[TestCase("strings.resources")]
	[TestCase("!AvaloniaResources")]
	public void Typed_Resource_Names_Route_To_Specialised_Node(string name)
	{
		// Each known resource extension (.xsd/.xml/.png/.bmp/.ico/.cur/.resources/...) has a
		// dedicated ResourceNodeFactory that produces a richer node than the generic
		// ResourceTreeNode fallback. Verifies the dispatcher picks the typed handler for every
		// recognised extension regardless of the stream's actual contents.

		// Arrange — boot composition so the static ResourceNodeFactories list is populated; build
		// a tiny dummy payload (typed-routing must be name-based, not content-based).
		EnsureComposition();
		var payload = Encoding.UTF8.GetBytes("<root />");

		// Act — dispatch the resource through the factory pipeline.
		var node = ResourceEntryNode.Create(new ByteArrayResource(name, payload));

		// Assert — the produced node is anything *but* the generic fallback. Compare by exact
		// type (not BeOfType) since SharpTreeNode has a custom .Should() extension in this
		// assembly that hides BeOfType.
		node.GetType().Should().NotBe(typeof(ResourceTreeNode),
			$"resource '{name}' should be routed to a specialised node, not the generic fallback");
	}

	[AvaloniaTest]
	public void Unknown_Extension_Falls_Back_To_Generic_Resource_Node()
	{
		// Resources whose extension isn't in any factory's claim list must fall back to the
		// plain ResourceTreeNode (which renders as a hex/text dump) rather than throwing or
		// silently dropping the entry.

		// Arrange — boot composition; pick an extension nobody handles.
		EnsureComposition();

		// Act — dispatch the unknown-extension resource.
		var node = ResourceEntryNode.Create(new ByteArrayResource("data.bin", new byte[] { 1, 2, 3 }));

		// Assert — exactly the generic ResourceTreeNode comes back.
		node.GetType().Should().Be(typeof(ResourceTreeNode));
	}

	[AvaloniaTest]
	public void Resources_File_Decompile_Emits_UIElements_For_String_And_Object_Tables()
	{
		// A .resources file should render inline as two DataGrids (one for string entries, one
		// for non-string objects) sitting alongside the inherited Save button. Drives the
		// ResourcesFileTreeNode.Decompile implementation that emits AddUIElement calls instead
		// of plain comment lines.

		// Arrange — boot composition, build a fixture .resources stream with mixed string and
		// numeric entries, force lazy children, grab the active language for WriteCommentLine.
		EnsureComposition();
		var node = (ResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("strings.resources", BuildResources(
				new (string, object)[] {
					("stringKey", "hello"),
					("anotherKey", "world"),
					("intKey", 42),
					("doubleKey", 3.14),
				})));
		node.EnsureLazyChildren();
		var output = new AvaloniaEditTextOutput();
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		// Act — decompile the resource node into the AvaloniaEdit text output.
		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		// Assert — exactly three UI elements (string grid + object grid + inherited Save button)
		// and they all materialise as Avalonia Controls.
		output.UIElements.Should().HaveCount(3,
			".resources should render a string table + object table inline alongside the Save button");
		var realised = output.UIElements.Select(kv => kv.Value()).ToList();
		realised.Should().ContainItemsAssignableTo<Control>();
	}

	[AvaloniaTest]
	public void Resources_File_WriteResX_Round_Trips_String_And_Object_Entries()
	{
		// ResourcesFileTreeNode.WriteResX is the format converter behind the "Save as ResX"
		// option in the resource Save dialog. Verifies it produces well-formed ResX XML with
		// both string and non-string entries faithfully serialised.

		// Arrange — boot composition; build a fixture .resources stream with one string and one
		// integer entry; force lazy children so the resource enumeration is realised.
		EnsureComposition();
		var node = (ResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("strings.resources", BuildResources(
				new (string, object)[] {
					("greeting", "hello"),
					("answer", 42),
				})));
		node.EnsureLazyChildren();

		// Act — write the ResX XML into a memory stream and decode it.
		var ms = new MemoryStream();
		node.WriteResX(ms);
		var resx = Encoding.UTF8.GetString(ms.ToArray());

		// Assert — both entries land with proper name + value markup (ResX is XML).
		resx.Should().Contain("<data name=\"greeting\"");
		resx.Should().Contain("<value>hello</value>");
		resx.Should().Contain("<data name=\"answer\"");
		resx.Should().Contain("<value>42</value>");
	}

	[Test]
	public void FilePickers_ParseFilter_Splits_Pipe_Separated_Display_And_Patterns()
	{
		// FilePickers.ParseFilter translates the legacy WPF "Display|*.ext|Display|*.ext"
		// filter string into Avalonia FilePickerFileType records. Verifies the splitter pairs
		// up display name with the matching glob pattern correctly.

		// Arrange + Act — feed a two-format filter string through the parser.
		var types = ICSharpCode.ILSpy.Commands.FilePickers.ParseFilter(
			"Resources file (*.resources)|*.resources|Resource XML (*.resx)|*.resx");

		// Assert — two file types come back, each with the right display name and pattern.
		types.Should().HaveCount(2);
		types[0].Name.Should().Be("Resources file (*.resources)");
		types[0].Patterns.Should().BeEquivalentTo(new[] { "*.resources" });
		types[1].Name.Should().Be("Resource XML (*.resx)");
		types[1].Patterns.Should().BeEquivalentTo(new[] { "*.resx" });
	}

	[AvaloniaTest]
	public void AvaloniaResources_Unpacks_Packed_Files_Into_Child_Nodes()
	{
		// The !AvaloniaResources blob packs every Avalonia resource behind a single index. The
		// node must unpack it into one child per packed file (sorted naturally) so each file can
		// be viewed and saved on its own, mirroring how .resources files are unpacked.

		// Arrange — boot composition; build a blob with three packed files (deliberately out of
		// order to exercise the natural sort) and dispatch it.
		EnsureComposition();
		var node = (AvaloniaResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("!AvaloniaResources", BuildAvaloniaResources(
				("/Views/MainWindow.axaml", Encoding.UTF8.GetBytes("<Window/>")),
				("/App.axaml", Encoding.UTF8.GetBytes("<Application/>")),
				("/assets/logo.png", new byte[] { 0x89, 0x50, 0x4E, 0x47 }))));

		// Act — force the lazy children to load.
		node.EnsureLazyChildren();

		// Assert — one child per packed file, keyed by the rooted path and naturally sorted.
		var names = node.Children.Cast<ResourceEntryNode>().Select(c => c.Text.ToString()).ToList();
		names.Should().Equal("/App.axaml", "/assets/logo.png", "/Views/MainWindow.axaml");
	}

	static byte[] BuildAvaloniaResources(params (string Path, byte[] Data)[] files)
	{
		var ms = new MemoryStream();
		var data = new MemoryStream();
		using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
		{
			bw.Write(0); // index length placeholder, patched below
			bw.Write(2); // BinaryCurrentVersion
			bw.Write(files.Length);
			foreach (var (path, bytes) in files)
			{
				bw.Write(path);
				bw.Write((int)data.Position);
				bw.Write(bytes.Length);
				data.Write(bytes, 0, bytes.Length);
			}
		}
		int indexLength = (int)(ms.Length - 4);
		ms.Position = 0;
		using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			bw.Write(indexLength);
		ms.Position = ms.Length;
		data.Position = 0;
		data.CopyTo(ms);
		return ms.ToArray();
	}

	static byte[] BuildResources((string Key, object Value)[] entries)
	{
		var ms = new MemoryStream();
		using (var writer = new ResourceWriter(ms))
		{
			foreach (var (k, v) in entries)
				writer.AddResource(k, v);
		}
		return ms.ToArray();
	}

	[AvaloniaTest]
	[TestCase("Themes/Generic.baml")]
	[TestCase("logo.png")]
	[TestCase("favicon.ico")]
	public void Byte_Array_Entries_Route_Through_The_Factory_Pipeline(string name)
	{
		// .resources / !AvaloniaResources containers unpack their entries as raw byte arrays.
		// Those entries must go through the same factory dispatch as top-level resources, so a
		// .baml entry gets the BAML-to-XAML decompiler view, an image its viewer, and so on —
		// not the generic byte-count node.
		EnsureComposition();

		var node = ResourceEntryNode.Create(name, new byte[] { 0x00, 0x01 });

		node.GetType().Should().NotBe(typeof(ResourceEntryNode),
			$"entry '{name}' should be routed to a specialised node, not the generic entry node");
	}

	[AvaloniaTest]
	public void Resources_File_Decompile_Lists_Stream_Entries()
	{
		// Binary entries inside a .resources file only show up as tree children; the container's
		// text view must also list them by name so the content is discoverable without expanding
		// the tree.
		EnsureComposition();
		var node = (ResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("app.g.resources", BuildResources(
				new (string, object)[] {
					("assets/data.bin", new byte[] { 1, 2, 3 }),
					("greeting", "hello"),
				})));
		node.EnsureLazyChildren();
		var output = new AvaloniaEditTextOutput();
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.GetText().Should().Contain("assets/data.bin");
	}

	[AvaloniaTest]
	public void AvaloniaResources_Decompile_Lists_Packed_Entries()
	{
		// The !AvaloniaResources container packs files behind one index; its text view must
		// enumerate the packed paths, mirroring the .resources entry listing.
		EnsureComposition();
		var node = (AvaloniaResourcesFileTreeNode)ResourceEntryNode.Create(
			new ByteArrayResource("!AvaloniaResources", BuildAvaloniaResources(
				("/App.axaml", Encoding.UTF8.GetBytes("<Application/>")),
				("/assets/logo.png", new byte[] { 0x89, 0x50, 0x4E, 0x47 }))));
		var output = new AvaloniaEditTextOutput();
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.GetText().Should().Contain("/App.axaml");
		output.GetText().Should().Contain("/assets/logo.png");
	}
}
