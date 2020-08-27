// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.FlowAnalysis
{
	/// <summary>
	/// Represents a block in the control flow graph.
	/// </summary>
	[DebuggerDisplay("CFG UserIndex={UserIndex}, UserData={UserData}")]
	public class ControlFlowNode
	{
		/// <summary>
		/// User index, can be used to look up additional information in an array.
		/// </summary>
		public int UserIndex;

		/// <summary>
		/// User data.
		/// </summary>
		public object UserData;

		/// <summary>
		/// Visited flag, used in various algorithms.
		/// </summary>
		public bool Visited;

		/// <summary>
		/// Gets the node index in a post-order traversal of the control flow graph, starting at the
		/// entry point. This field gets computed by dominance analysis.
		/// </summary>
		public int PostOrderNumber;

		/// <summary>
		/// Gets whether this node is reachable. Requires that dominance is computed!
		/// </summary>
		public bool IsReachable {
			get { return DominatorTreeChildren != null; }
		}

		/// <summary>
		/// Gets the immediate dominator (the parent in the dominator tree).
		/// Null if dominance has not been calculated; or if the node is unreachable.
		/// </summary>
		public ControlFlowNode ImmediateDominator { get; internal set; }

		/// <summary>
		/// List of children in the dominator tree.
		/// Null if dominance has not been calculated; or if the node is unreachable.
		/// </summary>
		public List<ControlFlowNode> DominatorTreeChildren { get; internal set; }

		/// <summary>
		/// List of incoming control flow edges.
		/// </summary>
		public readonly List<ControlFlowNode> Predecessors = new List<ControlFlowNode>();

		/// <summary>
		/// List of outgoing control flow edges.
		/// </summary>
		public readonly List<ControlFlowNode> Successors = new List<ControlFlowNode>();

		public void AddEdgeTo(ControlFlowNode target)
		{
			this.Successors.Add(target);
			target.Predecessors.Add(this);
		}

		public void TraversePreOrder(Func<ControlFlowNode, IEnumerable<ControlFlowNode>> children, Action<ControlFlowNode> visitAction)
		{
			if (Visited)
				return;
			Visited = true;
			visitAction(this);
			foreach (ControlFlowNode t in children(this))
				t.TraversePreOrder(children, visitAction);
		}

		public void TraversePostOrder(Func<ControlFlowNode, IEnumerable<ControlFlowNode>> children, Action<ControlFlowNode> visitAction)
		{
			if (Visited)
				return;
			Visited = true;
			foreach (ControlFlowNode t in children(this))
				t.TraversePostOrder(children, visitAction);
			visitAction(this);
		}

		/// <summary>
		/// Gets whether <c>this</c> dominates <paramref name="node"/>.
		/// </summary>
		public bool Dominates(ControlFlowNode node)
		{
			// TODO: this can be made O(1) by numbering the dominator tree
			ControlFlowNode tmp = node;
			while (tmp != null)
			{
				if (tmp == this)
					return true;
				tmp = tmp.ImmediateDominator;
			}
			return false;
		}

#if DEBUG
		internal static GraphVizGraph ExportGraph(IReadOnlyList<ControlFlowNode> nodes, Func<ControlFlowNode, string> labelFunc = null)
		{
			if (labelFunc == null)
			{
				labelFunc = node => {
					var block = node.UserData as IL.Block;
					return block != null ? block.Label : node.UserData?.ToString();
				};
			}
			GraphVizGraph g = new GraphVizGraph();
			GraphVizNode[] n = new GraphVizNode[nodes.Count];
			for (int i = 0; i < n.Length; i++)
			{
				n[i] = new GraphVizNode(nodes[i].UserIndex);
				n[i].shape = "box";
				n[i].label = labelFunc(nodes[i]);
				g.AddNode(n[i]);
			}
			foreach (var source in nodes)
			{
				foreach (var target in source.Successors)
				{
					g.AddEdge(new GraphVizEdge(source.UserIndex, target.UserIndex));
				}
				if (source.ImmediateDominator != null)
				{
					g.AddEdge(
						new GraphVizEdge(source.ImmediateDominator.UserIndex, source.UserIndex) {
							color = "green"
						});
				}
			}
			return g;
		}
#endif
	}
}
