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
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.ILSpyX.Dependencies
{
	/// <summary>A point in layout coordinates.</summary>
	public readonly record struct LayoutPoint(double X, double Y);

	/// <summary>An axis-aligned rectangle in layout coordinates.</summary>
	public readonly record struct LayoutRect(double X, double Y, double Width, double Height)
	{
		public double Right => X + Width;
		public double Bottom => Y + Height;
		public double CenterY => Y + Height / 2;

		public bool Contains(double x, double y) => x >= X && x <= Right && y >= Y && y <= Bottom;
	}

	/// <summary>Spacing parameters for <see cref="LayeredLayout"/>.</summary>
	public sealed class LayeredLayoutOptions
	{
		public double NodeHeight { get; init; } = 28;
		public double DefaultNodeWidth { get; init; } = 120;
		/// <summary>Horizontal gap between two adjacent layers.</summary>
		public double LayerSpacing { get; init; } = 80;
		/// <summary>Vertical gap between two adjacent nodes of the same layer.</summary>
		public double NodeSpacing { get; init; } = 16;
		/// <summary>Empty border around the drawing.</summary>
		public double Margin { get; init; } = 16;
		/// <summary>Number of alternating barycenter sweeps used to reduce edge crossings.</summary>
		public int SweepIterations { get; init; } = 24;
	}

	/// <summary>The routed polyline of one input edge.</summary>
	public sealed class LayeredLayoutEdge
	{
		internal LayeredLayoutEdge(int from, int to, IReadOnlyList<LayoutPoint> points, bool isReversed)
		{
			From = from;
			To = to;
			Points = points;
			IsReversed = isReversed;
		}

		public int From { get; }
		public int To { get; }

		/// <summary>
		/// Polyline from the <see cref="From"/> node to the <see cref="To"/> node: the first point
		/// lies on the border of the source rectangle, the last on the border of the target.
		/// </summary>
		public IReadOnlyList<LayoutPoint> Points { get; }

		/// <summary>True when the edge closes a cycle and therefore points against the layer direction.</summary>
		public bool IsReversed { get; }
	}

	/// <summary>Result of <see cref="LayeredLayout.Compute"/>.</summary>
	public sealed class LayeredLayoutResult
	{
		internal LayeredLayoutResult(IReadOnlyList<int> layers, IReadOnlyList<LayoutRect> nodes,
			IReadOnlyList<LayeredLayoutEdge> edges, int layerCount, int crossingCount, double width, double height)
		{
			Layers = layers;
			Nodes = nodes;
			Edges = edges;
			LayerCount = layerCount;
			CrossingCount = crossingCount;
			Width = width;
			Height = height;
		}

		/// <summary>Layer (column) index of each input node.</summary>
		public IReadOnlyList<int> Layers { get; }

		/// <summary>Rectangle of each input node.</summary>
		public IReadOnlyList<LayoutRect> Nodes { get; }

		/// <summary>One routed edge per distinct, non-self-loop input edge, in input order.</summary>
		public IReadOnlyList<LayeredLayoutEdge> Edges { get; }

		public int LayerCount { get; }

		/// <summary>Number of segment crossings between adjacent layers in the chosen ordering.</summary>
		public int CrossingCount { get; }

		public double Width { get; }
		public double Height { get; }

		/// <summary>Index of the node whose rectangle contains the point, or -1.</summary>
		public int HitTest(double x, double y)
		{
			for (int i = 0; i < Nodes.Count; i++)
			{
				if (Nodes[i].Contains(x, y))
					return i;
			}
			return -1;
		}
	}

	/// <summary>
	/// A simple Sugiyama-style layered layout, laid out left to right: cycles are broken by
	/// reversing DFS back edges, nodes are assigned to layers by longest path from the sources,
	/// long edges are split by virtual nodes, and the order inside each layer is improved by
	/// alternating barycenter sweeps that keep the ordering with the fewest crossings. The result
	/// depends only on the input (node and edge order included), so it is deterministic.
	/// </summary>
	public static class LayeredLayout
	{
		public static LayeredLayoutResult Compute(int nodeCount, IReadOnlyList<(int From, int To)> edges,
			IReadOnlyList<double>? nodeWidths = null, LayeredLayoutOptions? options = null)
		{
			if (nodeCount < 0)
				throw new ArgumentOutOfRangeException(nameof(nodeCount));
			ArgumentNullException.ThrowIfNull(edges);
			if (nodeWidths != null && nodeWidths.Count != nodeCount)
				throw new ArgumentException("One width per node is required.", nameof(nodeWidths));
			options ??= new LayeredLayoutOptions();

			// Distinct, non-self-loop edges in input order.
			var inputEdges = new List<(int From, int To)>();
			var seen = new HashSet<(int, int)>();
			foreach (var (from, to) in edges)
			{
				if ((uint)from >= (uint)nodeCount || (uint)to >= (uint)nodeCount)
					throw new ArgumentOutOfRangeException(nameof(edges), $"Edge {from}->{to} references a missing node.");
				if (from != to && seen.Add((from, to)))
					inputEdges.Add((from, to));
			}

			bool[] reversed = FindBackEdges(nodeCount, inputEdges);
			var dag = inputEdges.Select((e, i) => reversed[i] ? (From: e.To, To: e.From) : e).ToList();
			int[] layer = AssignLayers(nodeCount, dag);
			int layerCount = nodeCount == 0 ? 0 : layer.Max() + 1;

			// Vertex ids: real nodes first, then virtual nodes splitting edges that span several layers.
			var vertexLayer = new List<int>(layer);
			var chains = new List<List<int>>();
			var preds = new List<List<int>>();
			var succs = new List<List<int>>();
			for (int i = 0; i < nodeCount; i++)
			{
				preds.Add(new List<int>());
				succs.Add(new List<int>());
			}
			foreach (var (from, to) in dag)
			{
				var chain = new List<int> { from };
				for (int l = layer[from] + 1; l < layer[to]; l++)
				{
					int dummy = vertexLayer.Count;
					vertexLayer.Add(l);
					preds.Add(new List<int>());
					succs.Add(new List<int>());
					chain.Add(dummy);
				}
				chain.Add(to);
				for (int k = 0; k + 1 < chain.Count; k++)
				{
					succs[chain[k]].Add(chain[k + 1]);
					preds[chain[k + 1]].Add(chain[k]);
				}
				chains.Add(chain);
			}

			var order = new List<List<int>>();
			for (int l = 0; l < layerCount; l++)
				order.Add(new List<int>());
			for (int v = 0; v < vertexLayer.Count; v++)
				order[vertexLayer[v]].Add(v);

			order = ReduceCrossings(order, preds, succs, vertexLayer.Count, options.SweepIterations, out int crossings);

			// Coordinates.
			double Width(int v) => v < nodeCount ? (nodeWidths?[v] ?? options.DefaultNodeWidth) : 0;
			double Height(int v) => v < nodeCount ? options.NodeHeight : 0;

			var layerX = new double[layerCount];
			var layerWidth = new double[layerCount];
			double x = options.Margin;
			for (int l = 0; l < layerCount; l++)
			{
				layerX[l] = x;
				layerWidth[l] = order[l].Count == 0 ? 0 : order[l].Max(Width);
				x += layerWidth[l] + options.LayerSpacing;
			}
			double totalWidth = layerCount == 0 ? 0 : layerX[layerCount - 1] + layerWidth[layerCount - 1] + options.Margin;

			var stackHeight = new double[layerCount];
			for (int l = 0; l < layerCount; l++)
			{
				var vs = order[l];
				stackHeight[l] = vs.Sum(Height) + Math.Max(0, vs.Count - 1) * options.NodeSpacing;
			}
			double maxStack = layerCount == 0 ? 0 : stackHeight.Max();
			double totalHeight = layerCount == 0 ? 0 : maxStack + 2 * options.Margin;

			var vertexY = new double[vertexLayer.Count];
			var rects = new LayoutRect[nodeCount];
			for (int l = 0; l < layerCount; l++)
			{
				double y = options.Margin + (maxStack - stackHeight[l]) / 2;
				foreach (int v in order[l])
				{
					double h = Height(v);
					vertexY[v] = y + h / 2;
					if (v < nodeCount)
						rects[v] = new LayoutRect(layerX[l], y, Width(v), h);
					y += h + options.NodeSpacing;
				}
			}

			var routed = new List<LayeredLayoutEdge>(inputEdges.Count);
			for (int i = 0; i < inputEdges.Count; i++)
			{
				var chain = chains[i];
				var points = new List<LayoutPoint>();
				var source = rects[chain[0]];
				points.Add(new LayoutPoint(source.Right, source.CenterY));
				for (int k = 1; k + 1 < chain.Count; k++)
				{
					int d = chain[k];
					int l = vertexLayer[d];
					points.Add(new LayoutPoint(layerX[l], vertexY[d]));
					points.Add(new LayoutPoint(layerX[l] + layerWidth[l], vertexY[d]));
				}
				var target = rects[chain[chain.Count - 1]];
				points.Add(new LayoutPoint(target.X, target.CenterY));
				if (reversed[i])
					points.Reverse();
				routed.Add(new LayeredLayoutEdge(inputEdges[i].From, inputEdges[i].To, points, reversed[i]));
			}

			return new LayeredLayoutResult(layer, rects, routed, layerCount, crossings, totalWidth, totalHeight);
		}

		/// <summary>
		/// Marks the edges that point back to a node on the current DFS path. Reversing exactly
		/// those makes the graph acyclic. Nodes and their out-edges are visited in input order.
		/// </summary>
		static bool[] FindBackEdges(int nodeCount, List<(int From, int To)> edges)
		{
			var outEdges = new List<int>[nodeCount];
			for (int i = 0; i < nodeCount; i++)
				outEdges[i] = new List<int>();
			for (int i = 0; i < edges.Count; i++)
				outEdges[edges[i].From].Add(i);

			var reversed = new bool[edges.Count];
			var state = new byte[nodeCount]; // 0 = unvisited, 1 = on stack, 2 = done
			var stack = new Stack<(int Node, int Next)>();
			for (int start = 0; start < nodeCount; start++)
			{
				if (state[start] != 0)
					continue;
				stack.Push((start, 0));
				state[start] = 1;
				while (stack.Count > 0)
				{
					var (node, next) = stack.Pop();
					if (next < outEdges[node].Count)
					{
						stack.Push((node, next + 1));
						int edge = outEdges[node][next];
						int to = edges[edge].To;
						if (state[to] == 1)
						{
							reversed[edge] = true;
						}
						else if (state[to] == 0)
						{
							state[to] = 1;
							stack.Push((to, 0));
						}
					}
					else
					{
						state[node] = 2;
					}
				}
			}
			return reversed;
		}

		/// <summary>Longest-path layering: a node sits one layer right of its furthest predecessor.</summary>
		static int[] AssignLayers(int nodeCount, List<(int From, int To)> dag)
		{
			var layer = new int[nodeCount];
			var inDegree = new int[nodeCount];
			var outEdges = new List<int>[nodeCount];
			for (int i = 0; i < nodeCount; i++)
				outEdges[i] = new List<int>();
			foreach (var (from, to) in dag)
			{
				outEdges[from].Add(to);
				inDegree[to]++;
			}
			var ready = new Queue<int>();
			for (int i = 0; i < nodeCount; i++)
			{
				if (inDegree[i] == 0)
					ready.Enqueue(i);
			}
			while (ready.Count > 0)
			{
				int node = ready.Dequeue();
				foreach (int to in outEdges[node])
				{
					layer[to] = Math.Max(layer[to], layer[node] + 1);
					if (--inDegree[to] == 0)
						ready.Enqueue(to);
				}
			}
			return layer;
		}

		static List<List<int>> ReduceCrossings(List<List<int>> order, List<List<int>> preds, List<List<int>> succs,
			int vertexCount, int iterations, out int bestCrossings)
		{
			var position = new int[vertexCount];
			UpdatePositions(order, position);
			var best = Copy(order);
			bestCrossings = CountCrossings(order, succs, position);
			for (int iter = 0; iter < iterations && bestCrossings > 0; iter++)
			{
				bool down = iter % 2 == 0;
				if (down)
				{
					for (int l = 1; l < order.Count; l++)
						SortByBarycenter(order[l], preds, position);
				}
				else
				{
					for (int l = order.Count - 2; l >= 0; l--)
						SortByBarycenter(order[l], succs, position);
				}
				int crossings = CountCrossings(order, succs, position);
				if (crossings < bestCrossings)
				{
					bestCrossings = crossings;
					best = Copy(order);
				}
			}
			return best;
		}

		static void SortByBarycenter(List<int> layer, List<List<int>> neighbors, int[] position)
		{
			var keyed = layer
				.Select(v => (Vertex: v, Key: neighbors[v].Count == 0 ? position[v] : neighbors[v].Average(n => (double)position[n])))
				.OrderBy(t => t.Key)
				.ThenBy(t => position[t.Vertex])
				.Select(t => t.Vertex)
				.ToList();
			for (int i = 0; i < keyed.Count; i++)
			{
				layer[i] = keyed[i];
				position[keyed[i]] = i;
			}
		}

		static int CountCrossings(List<List<int>> order, List<List<int>> succs, int[] position)
		{
			int crossings = 0;
			for (int l = 0; l + 1 < order.Count; l++)
			{
				var segments = new List<(int Upper, int Lower)>();
				foreach (int v in order[l])
				{
					foreach (int w in succs[v])
						segments.Add((position[v], position[w]));
				}
				for (int i = 0; i < segments.Count; i++)
				{
					for (int j = i + 1; j < segments.Count; j++)
					{
						var a = segments[i];
						var b = segments[j];
						if ((a.Upper < b.Upper && a.Lower > b.Lower) || (a.Upper > b.Upper && a.Lower < b.Lower))
							crossings++;
					}
				}
			}
			return crossings;
		}

		static void UpdatePositions(List<List<int>> order, int[] position)
		{
			foreach (var layer in order)
			{
				for (int i = 0; i < layer.Count; i++)
					position[layer[i]] = i;
			}
		}

		static List<List<int>> Copy(List<List<int>> order) => order.Select(l => new List<int>(l)).ToList();
	}
}
