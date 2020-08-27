// Copyright (c) 2014 Daniel Grunwald
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.FlowAnalysis
{
	/// <summary>
	/// Description of Dominance.
	/// </summary>
	public static class Dominance
	{
		/// <summary>
		/// Computes the dominator tree.
		/// </summary>
		/// <remarks>
		/// Precondition: the dominance tree is not already computed for some nodes reachable from entryPoint
		/// (i.e. ImmediateDominator and DominatorTreeChildren are both null),
		/// and the visited flag is false for any nodes reachable from entryPoint.
		/// 
		/// Postcondition: a dominator tree is constructed for all nodes reachable from entryPoint,
		/// and the visited flag remains false.
		/// </remarks>
		public static void ComputeDominance(ControlFlowNode entryPoint, CancellationToken cancellationToken = default(CancellationToken))
		{
			// A Simple, Fast Dominance Algorithm
			// Keith D. Cooper, Timothy J. Harvey and Ken Kennedy

			var nodes = new List<ControlFlowNode>();
			entryPoint.TraversePostOrder(n => n.Successors, nodes.Add);
			Debug.Assert(nodes.Last() == entryPoint);
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].PostOrderNumber = i;
			}

			// For the purpose of this algorithm, make the entry point its own dominator.
			// We'll reset it back to null at the end of this function.
			entryPoint.ImmediateDominator = entryPoint;
			bool changed;
			do
			{
				changed = false;

				cancellationToken.ThrowIfCancellationRequested();

				// For all nodes b except the entry point (in reverse post-order)
				for (int i = nodes.Count - 2; i >= 0; i--)
				{
					ControlFlowNode b = nodes[i];
					// Compute new immediate dominator:
					ControlFlowNode newIdom = null;
					foreach (var p in b.Predecessors)
					{
						// Ignore predecessors that were not processed yet
						if (p.ImmediateDominator != null)
						{
							if (newIdom == null)
								newIdom = p;
							else
								newIdom = FindCommonDominator(p, newIdom);
						}
					}
					// The reverse post-order ensures at least one of our predecessors was processed.
					Debug.Assert(newIdom != null);
					if (newIdom != b.ImmediateDominator)
					{
						b.ImmediateDominator = newIdom;
						changed = true;
					}
				}
			} while (changed);
			// Create dominator tree for all reachable nodes:
			foreach (ControlFlowNode node in nodes)
			{
				if (node.ImmediateDominator != null)
					node.DominatorTreeChildren = new List<ControlFlowNode>();
			}
			entryPoint.ImmediateDominator = null;
			foreach (ControlFlowNode node in nodes)
			{
				// Create list of children in dominator tree
				if (node.ImmediateDominator != null)
					node.ImmediateDominator.DominatorTreeChildren.Add(node);
				// Also reset the visited flag
				node.Visited = false;
			}
		}

		/// <summary>
		/// Returns the common ancestor of a and b in the dominator tree.
		/// 
		/// Precondition: a and b are part of the same dominator tree.
		/// </summary>
		public static ControlFlowNode FindCommonDominator(ControlFlowNode a, ControlFlowNode b)
		{
			while (a != b)
			{
				while (a.PostOrderNumber < b.PostOrderNumber)
					a = a.ImmediateDominator;
				while (b.PostOrderNumber < a.PostOrderNumber)
					b = b.ImmediateDominator;
			}
			return a;
		}

		/// <summary>
		/// Computes a BitSet where
		/// <c>result[i] == true</c> iff cfg[i] is reachable and there is some node that is
		/// reachable from cfg[i] but not dominated by cfg[i].
		/// 
		/// This is similar to "does cfg[i] have a non-empty dominance frontier?",
		/// except that it uses non-strict dominance where the definition of dominance frontiers
		/// uses "strictly dominates".
		/// 
		/// Precondition:
		///  Dominance was computed for cfg and <c>cfg[i].UserIndex == i</c> for all i.
		/// </summary>
		public static BitSet MarkNodesWithReachableExits(ControlFlowNode[] cfg)
		{
#if DEBUG
			for (int i = 0; i < cfg.Length; i++)
			{
				Debug.Assert(cfg[i].UserIndex == i);
			}
#endif
			BitSet nonEmpty = new BitSet(cfg.Length);
			foreach (var j in cfg)
			{
				// If j is a join-point (more than one incoming edge):
				// `j.IsReachable && j.ImmediateDominator == null` is the root node, which counts as an extra incoming edge
				if (j.IsReachable && (j.Predecessors.Count >= 2 || (j.Predecessors.Count >= 1 && j.ImmediateDominator == null)))
				{
					// Add j to frontier of all predecessors and their dominators up to j's immediate dominator.
					foreach (var p in j.Predecessors)
					{
						for (var runner = p; runner != j.ImmediateDominator && runner != j && runner != null; runner = runner.ImmediateDominator)
						{
							nonEmpty.Set(runner.UserIndex);
						}
					}
				}
			}
			return nonEmpty;
		}
	}
}
