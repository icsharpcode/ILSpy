// Copyright (c) 2026 Piero Viano
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
using System.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Dependencies;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class DependencyGraphMermaidWriterTests
{
	// Unresolved nodes need no metadata, which lets the graph be built by hand.
	static AssemblyDependencyGraph CreateGraph(params string[] names)
	{
		var nodes = names.Select((n, i) => new AssemblyDependencyNode(i, n, n + ", Version=1.0.0.0", new Version(1, 0, 0, 0), null, isRoot: i == 0)).ToList();
		var edges = Enumerable.Range(1, names.Length - 1)
			.Select(i => new AssemblyDependencyEdge(0, i, names[i], new Version(0, 9, 0, 0), isVersionMismatch: i == 1))
			.ToList();
		return new AssemblyDependencyGraph(nodes, edges);
	}

	[Test]
	public void Mermaid_Text_Is_A_Left_To_Right_Graph_With_Nodes_Edges_And_Classes()
	{
		var mermaid = DependencyGraphMermaidWriter.ToMermaid(CreateGraph("App", "Lib", "Other"));
		var lines = mermaid.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToList();

		lines[0].Should().Be("graph LR");
		lines.Should().Contain("n0[\"App 1.0.0.0 (unresolved)\"]");
		lines.Should().Contain("n0 -->|\"v0.9.0.0\"| n1", "a version mismatch labels the edge with the referenced version");
		lines.Should().Contain("n0 --> n2", "a matching version leaves the edge unlabelled");
		lines.Should().Contain("class n0 root;");
		lines.Should().Contain("class n2 unresolved;");
	}

	[Test]
	public void Labels_Escape_Characters_That_Would_Break_The_Mermaid_Syntax()
	{
		DependencyGraphMermaidWriter.EscapeLabel("a\"b<c>d&e#f|g`h\ni")
			.Should().Be("a#quot;b#lt;c#gt;d#amp;e#35;f#124;g#96;h i");

		var mermaid = DependencyGraphMermaidWriter.ToMermaid(CreateGraph("Evil\"]--> x[\"pwn", "Lib"));
		mermaid.Should().Contain("n0[\"Evil#quot;]--#gt; x[#quot;pwn 1.0.0.0 (unresolved)\"]");
		mermaid.Should().NotContain("x[\"pwn");
	}

	[Test]
	public void Html_Page_Embeds_The_Encoded_Diagram_And_Loads_Mermaid()
	{
		var graph = CreateGraph("A<b>", "Lib");
		var html = DependencyGraphMermaidWriter.ToHtml(graph, "Deps of <A&B>");

		html.Should().StartWith("<!DOCTYPE html>");
		html.Should().Contain("<title>Deps of &lt;A&amp;B&gt;</title>");
		html.Should().Contain("<pre class=\"mermaid\">");
		html.Should().Contain("graph LR");
		html.Should().Contain("n0 --&gt;", "the diagram text is HTML-encoded inside the page");
		html.Should().NotContain("<b>");
		html.Should().Contain($"<script src=\"{DependencyGraphMermaidWriter.MermaidScriptUrl}\"></script>");
		html.Should().Contain("mermaid.initialize(");
	}
}
