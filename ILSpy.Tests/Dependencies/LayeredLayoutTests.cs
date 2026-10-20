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
public class LayeredLayoutTests
{
	[Test]
	public void Longest_Path_Layering_Places_Each_Node_After_Its_Furthest_Predecessor()
	{
		// 0 -> 1 -> 2 plus the shortcut 0 -> 2: node 2 must land in layer 2, not 1.
		var result = LayeredLayout.Compute(3, [(0, 1), (1, 2), (0, 2)]);

		result.Layers.Should().Equal(0, 1, 2);
		result.LayerCount.Should().Be(3);
	}

	[Test]
	public void Every_Acyclic_Edge_Points_To_A_Later_Layer_And_Long_Edges_Are_Routed_Through_Each_Layer()
	{
		var edges = new (int, int)[] { (0, 1), (1, 2), (2, 3), (0, 3), (4, 3) };
		var result = LayeredLayout.Compute(5, edges);

		foreach (var edge in result.Edges)
		{
			edge.IsReversed.Should().BeFalse();
			result.Layers[edge.To].Should().BeGreaterThan(result.Layers[edge.From]);
			var source = result.Nodes[edge.From];
			var target = result.Nodes[edge.To];
			edge.Points[0].Should().Be(new LayoutPoint(source.Right, source.CenterY), "an edge leaves the right side of its source");
			edge.Points[^1].Should().Be(new LayoutPoint(target.X, target.CenterY), "an edge enters the left side of its target");
			int span = result.Layers[edge.To] - result.Layers[edge.From];
			edge.Points.Count.Should().Be(2 + 2 * (span - 1), "each skipped layer adds an entry and an exit point");
		}
	}

	[Test]
	public void Cycles_Are_Broken_By_Reversing_A_Back_Edge()
	{
		var result = LayeredLayout.Compute(3, [(0, 1), (1, 2), (2, 0)]);

		result.Edges.Should().HaveCount(3);
		result.Edges.Count(e => e.IsReversed).Should().Be(1);
		var back = result.Edges.Single(e => e.IsReversed);
		back.From.Should().Be(2);
		back.To.Should().Be(0);
		result.Layers[back.From].Should().BeGreaterThan(result.Layers[back.To]);
		// The polyline still runs from the edge's source to its target.
		var source = result.Nodes[back.From];
		var target = result.Nodes[back.To];
		back.Points[0].Should().Be(new LayoutPoint(source.X, source.CenterY));
		back.Points[^1].Should().Be(new LayoutPoint(target.Right, target.CenterY));
	}

	[Test]
	public void Self_Loops_And_Duplicate_Edges_Are_Dropped()
	{
		var result = LayeredLayout.Compute(2, [(0, 0), (0, 1), (0, 1)]);

		result.Edges.Should().ContainSingle().Which.Should().Match<LayeredLayoutEdge>(e => e.From == 0 && e.To == 1);
	}

	[Test]
	public void Barycenter_Ordering_Removes_An_Avoidable_Crossing()
	{
		// Index order puts 2 above 3 in the second layer, so 0->3 and 1->2 would cross.
		var result = LayeredLayout.Compute(4, [(0, 3), (1, 2)]);

		result.CrossingCount.Should().Be(0);
		result.Nodes[3].Y.Should().BeLessThan(result.Nodes[2].Y, "the target of the upper source moves up");
	}

	[Test]
	public void Nodes_Do_Not_Overlap_And_Layers_Advance_Left_To_Right()
	{
		var edges = Enumerable.Range(1, 8).Select(i => (0, i)).Concat(Enumerable.Range(1, 7).Select(i => (i, 9))).ToArray();
		var widths = Enumerable.Range(0, 10).Select(i => 60.0 + 10 * i).ToArray();
		var result = LayeredLayout.Compute(10, edges, widths);

		for (int i = 0; i < 10; i++)
		{
			result.Nodes[i].Width.Should().Be(widths[i]);
			for (int j = i + 1; j < 10; j++)
			{
				var a = result.Nodes[i];
				var b = result.Nodes[j];
				bool disjoint = a.Right <= b.X || b.Right <= a.X || a.Bottom <= b.Y || b.Bottom <= a.Y;
				disjoint.Should().BeTrue($"nodes {i} and {j} must not overlap");
				if (result.Layers[i] < result.Layers[j])
					a.Right.Should().BeLessThan(b.X);
			}
		}
		result.Nodes.Max(n => n.Right).Should().BeLessThanOrEqualTo(result.Width);
		result.Nodes.Max(n => n.Bottom).Should().BeLessThanOrEqualTo(result.Height);
	}

	[Test]
	public void Layout_Is_Deterministic()
	{
		var edges = new (int, int)[] { (0, 4), (1, 3), (2, 5), (3, 6), (4, 6), (5, 3), (6, 1), (0, 6), (2, 4) };
		var first = LayeredLayout.Compute(7, edges);
		var second = LayeredLayout.Compute(7, edges);

		second.Nodes.Should().Equal(first.Nodes);
		second.Layers.Should().Equal(first.Layers);
		second.Edges.Select(e => string.Join(";", e.Points)).Should().Equal(first.Edges.Select(e => string.Join(";", e.Points)));
	}

	[Test]
	public void HitTest_Finds_The_Node_Under_A_Point()
	{
		var result = LayeredLayout.Compute(2, [(0, 1)]);
		var target = result.Nodes[1];

		result.HitTest(target.X + 1, target.CenterY).Should().Be(1);
		result.HitTest(-5, -5).Should().Be(-1);
	}

	[Test]
	public void Empty_Graph_Has_No_Extent()
	{
		var result = LayeredLayout.Compute(0, Array.Empty<(int, int)>());

		result.Nodes.Should().BeEmpty();
		result.Width.Should().Be(0);
		result.Height.Should().Be(0);
	}
}
