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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Baml;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

using IResourceFileHandler = ICSharpCode.ILSpy.Languages.IResourceFileHandler;
using ResourceFileHandlerContext = ICSharpCode.ILSpy.Languages.ResourceFileHandlerContext;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class BamlResourceTests
{
	// Boot composition once per test so AppComposition.Current is live for the MEF lookups
	// the factory/handler depend on. Resolving the MainWindow type forces App.Initialize.
	static void EnsureComposition() => AppComposition.Current.GetExport<MainWindow>();

	[AvaloniaTest]
	public void Factory_Creates_BamlResourceEntryNode_For_Baml_Resource()
	{
		// The MEF-discovered BamlResourceNodeFactory must claim any `.baml`-named resource
		// and wrap it in a BamlResourceEntryNode, regardless of the bytes inside the stream.
		// Routing is name-based, mirroring the XML / image / cursor factories — actual BAML
		// parsing happens lazily when the user selects the node.

		// Arrange — boot composition; build a dummy .baml-named resource (bytes don't matter
		// for the routing decision).
		EnsureComposition();
		var resource = new ByteArrayResource("Pages/MyWindow.baml", new byte[] { 0, 1, 2 });

		// Act — dispatch through the central resource factory pipeline.
		var node = ResourceEntryNode.Create(resource);

		// Assert — comes back as a BamlResourceEntryNode, not the generic fallback.
		node.GetType().Should().Be(typeof(BamlResourceEntryNode));
	}

	[AvaloniaTest]
	public void Factory_Ignores_Non_Baml_Resource_Names()
	{
		// The BAML factory must only claim `.baml` resources. Anything else flows through
		// to other factories or the generic ResourceTreeNode fallback. A misfire here would
		// hijack arbitrary streams and crash on first selection (XamlDecompiler.Decompile
		// throws on non-BAML input).

		// Arrange + Act — feed a non-baml name straight to the BAML factory.
		var factory = new BamlResourceNodeFactory();
		var node = factory.CreateNode(new ByteArrayResource("readme.txt", new byte[] { 0, 1, 2 }));

		// Assert — factory yields null so the dispatcher tries the next candidate.
		node.Should().BeNull();
	}

	[AvaloniaTest]
	public void File_Handler_Claims_Baml_Extension_Case_Insensitive()
	{
		// During project export the handler is consulted via CanHandle; first claim wins.
		// The match is case-insensitive on the extension (`.BAML` from an obfuscated assembly
		// must still produce a XAML output).

		// Arrange — boot composition; build a context with a default DecompilationOptions
		// (the handler doesn't touch the options for CanHandle).
		EnsureComposition();
		var handler = new BamlResourceFileHandler();
		var context = new ResourceFileHandlerContext(new DecompilationOptions(new DecompilerSettings()));

		// Act + Assert — three positive variants and two rejections.
		handler.CanHandle("MainWindow.baml", context).Should().BeTrue();
		handler.CanHandle("themes/Generic.BAML", context).Should().BeTrue();
		handler.CanHandle("Resources/Strings.baml", context).Should().BeTrue();
		handler.CanHandle("readme.txt", context).Should().BeFalse();
		handler.CanHandle("Window.xaml", context).Should().BeFalse();
	}

	[AvaloniaTest]
	public void File_Handler_Reports_Page_Entry_Type()
	{
		// EntryType drives the MSBuild item group the produced file lands in. BAML→XAML
		// must go under `<Page>` so MSBuild's XAML compiler picks it up and regenerates the
		// matching .g.cs partial on rebuild — same as the WPF side.

		// Arrange + Act + Assert — exact string match (case-sensitive; MSBuild element names
		// are case-insensitive in practice but the canonical form is "Page").
		new BamlResourceFileHandler().EntryType.Should().Be("Page");
	}

	[AvaloniaTest]
	public void Mef_Discovers_Baml_Factory_And_File_Handler()
	{
		// Both exports must surface through AppComposition. If `[Export]` / `[Shared]` got
		// dropped during a future refactor the pane would silently dump raw bytes for .baml
		// and project export would emit a bare `EmbeddedResource` instead of a `<Page>` —
		// detect that regression here.

		// Arrange — boot composition.
		EnsureComposition();

		// Act — pull every resource-node-factory and every file-handler from the container.
		var factories = AppComposition.Current.GetExports<IResourceNodeFactory>().ToList();
		var handlers = AppComposition.Current.GetExports<IResourceFileHandler>().ToList();

		// Assert — at least one of each is our BAML implementation.
		factories.Should().ContainItemsAssignableTo<BamlResourceNodeFactory>();
		handlers.Should().ContainItemsAssignableTo<BamlResourceFileHandler>();
	}
}
