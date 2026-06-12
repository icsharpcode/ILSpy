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

#if DEBUG
using System.IO;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The CFG viewer ultimately shells out to <c>dot</c>, which can't run reliably in
/// CI. These tests pin the GraphViz-DOT emitter on its own — the pure-text shape
/// of <c>digraph G { node [...]; edge -> edge; }</c> that <c>dot</c> consumes.
/// </summary>
[TestFixture]
public class GraphVizGraphTests
{
	[Test]
	public void Save_Writes_Digraph_Header_With_Default_Node_Font_Size()
	{
		// Every GraphVizGraph must start with `digraph G {` + the default `node [fontsize=16];`
		// preamble. That preamble is what makes the generated DOT readable when dot renders it.
		var g = new ICSharpCode.ILSpy.Util.GraphVizGraph();
		var sw = new StringWriter();
		g.Save(sw);
		var dot = sw.ToString();
		dot.Should().Contain("digraph G {");
		dot.Should().Contain("node [fontsize = 16];");
		dot.Should().Contain("}");
	}

	[Test]
	public void Nodes_Render_With_Id_Label_And_Shape()
	{
		// A node emits as `<id> [label="...", shape=...]` — the box shape is what the
		// CFG viewer relies on so every basic block reads as a rectangle.
		var g = new ICSharpCode.ILSpy.Util.GraphVizGraph();
		g.AddNode(new ICSharpCode.ILSpy.Util.GraphVizNode(42) { label = "BB.42", shape = "box" });
		var sw = new StringWriter();
		g.Save(sw);
		var dot = sw.ToString();
		dot.Should().Contain("42 [");
		dot.Should().Contain("label=\"BB.42\"");
		dot.Should().Contain("shape=box");
	}

	[Test]
	public void Edges_Render_Source_Target_With_Optional_Color()
	{
		// Successor edges have no color (rendered as black); dominator edges use green
		// — that's how the CFG viewer distinguishes the two relationship overlays.
		var g = new ICSharpCode.ILSpy.Util.GraphVizGraph();
		g.AddEdge(new ICSharpCode.ILSpy.Util.GraphVizEdge(1, 2));
		g.AddEdge(new ICSharpCode.ILSpy.Util.GraphVizEdge(1, 2) { color = "green" });
		var sw = new StringWriter();
		g.Save(sw);
		var dot = sw.ToString();
		dot.Should().Contain("1 -> 2 [");
		dot.Should().Contain("color=green");
	}

	[Test]
	public void Label_With_Quote_Is_Escaped_For_DOT_Syntax()
	{
		// A literal `"` inside a label would otherwise terminate the DOT string token
		// early. The escaper turns it into `\"` so dot parses the whole label as one token.
		var g = new ICSharpCode.ILSpy.Util.GraphVizGraph();
		g.AddNode(new ICSharpCode.ILSpy.Util.GraphVizNode(0) { label = "a \"quoted\" label" });
		var sw = new StringWriter();
		g.Save(sw);
		sw.ToString().Should().Contain("\"a \\\"quoted\\\" label\"");
	}
}
#endif
