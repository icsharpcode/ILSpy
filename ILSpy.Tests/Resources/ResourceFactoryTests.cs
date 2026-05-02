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

using System.Text;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;

using ILSpy.AppEnv;
using ILSpy.TreeNodes;
using ILSpy.Views;

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
	[TestCase("logo.png")]
	[TestCase("logo.gif")]
	[TestCase("logo.bmp")]
	[TestCase("logo.jpg")]
	[TestCase("favicon.ico")]
	[TestCase("pointer.cur")]
	[TestCase("strings.resources")]
	public void Typed_Resource_Names_Route_To_Specialised_Node(string name)
	{
		EnsureComposition();
		// Tiny payload — node creation must not depend on stream contents being parseable.
		var payload = Encoding.UTF8.GetBytes("<root />");
		var node = ResourceEntryNode.Create(new ByteArrayResource(name, payload));

		// Anything ending in a known extension must NOT fall back to the generic node — the
		// dispatcher's whole job is to pick the right typed handler. Compare by exact type
		// (not BeOfType) since SharpTreeNode has a custom .Should() extension in this assembly.
		node.GetType().Should().NotBe(typeof(ResourceTreeNode),
			$"resource '{name}' should be routed to a specialised node, not the generic fallback");
	}

	[AvaloniaTest]
	public void Unknown_Extension_Falls_Back_To_Generic_Resource_Node()
	{
		EnsureComposition();
		var node = ResourceEntryNode.Create(new ByteArrayResource("data.bin", new byte[] { 1, 2, 3 }));
		node.GetType().Should().Be(typeof(ResourceTreeNode));
	}
}
