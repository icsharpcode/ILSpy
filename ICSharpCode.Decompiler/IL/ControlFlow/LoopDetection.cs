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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.FlowAnalysis;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Detect loops in IL AST.
	/// </summary>
	/// <remarks>
	/// Transform ordering:
	/// * LoopDetection should run before other control flow structures are detected.
	/// * Blocks should be basic blocks (not extended basic blocks) so that the natural loops
	/// don't include more instructions than strictly necessary.
	/// * (depending on future loop detection improvements:) Loop detection should run after the 'return block' is duplicated (OptimizingTransform).
	/// </remarks>
	public class LoopDetection : IILTransform
	{
		#region Construct Control Flow Graph
		/// <summary>
		/// Constructs a control flow graph for the blocks in the given block container.
		/// The graph nodes will have the same indices as the blocks in the block container.
		/// Return statements, exceptions, or branches leaving the block container are not
		/// modeled by the control flow graph.
		/// </summary>
		internal static ControlFlowNode[] BuildCFG(BlockContainer bc)
		{
			ControlFlowNode[] nodes = new ControlFlowNode[bc.Blocks.Count];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = new ControlFlowNode { UserData = bc.Blocks[i] };
			}
			
			// Create edges:
			for (int i = 0; i < bc.Blocks.Count; i++) {
				var block = bc.Blocks[i];
				var sourceNode = nodes[i];
				foreach (var branch in block.Descendants.OfType<Branch>()) {
					if (branch.TargetBlock.Parent == bc) {
						sourceNode.AddEdgeTo(nodes[bc.Blocks.IndexOf(branch.TargetBlock)]);
					} else {
						// Note: edges into different block containers are ignored:
						// Either they point to a nested block container in the source block,
						// in which case we can ignore them for control flow purposes;
						// or they jump to a parent block container, in which case they act
						// like a return statement or exceptional exit.
					}
				}
			}
			
			return nodes;
		}
		#endregion
		
		/// <summary>
		/// Run loop detection for all block containers in the function (including nested lambda functions).
		/// </summary>
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var blockContainer in function.Descendants.OfType<BlockContainer>()) {
				Run(blockContainer, context);
			}
		}
		
		/// <summary>
		/// Run loop detection for blocks in the block container.
		/// </summary>
		public void Run(BlockContainer blockContainer, ILTransformContext context)
		{
			var cfg = BuildCFG(blockContainer);
			var entryPoint = cfg[0];
			Dominance.ComputeDominance(entryPoint, context.CancellationToken);
			FindLoops(entryPoint);
		}
		
		/// <summary>
		/// Recurse into the dominator tree and find back edges/natural loops.
		/// </summary>
		/// <remarks>
		/// A back edge is an edge t->h so that h dominates t.
		/// The natural loop of the back edge is the smallest set of nodes that includes the back edge
		/// and has no predecessors outside the set except for the predecessor of the header.
		/// 
		/// Preconditions:
		/// * dominance was computed for h
		/// * all blocks in the dominator subtree starting at h are in the same BlockContainer
		/// * the visited flag is set to false
		/// </remarks>
		void FindLoops(ControlFlowNode h)
		{
			List<ControlFlowNode> loop = null;
			foreach (var t in h.Predecessors) {
				if (h.Dominates(t)) {
					// h->t is a back edge, and h is a loop header
					// Add the natural loop of t->h to the loop.
					if (loop == null) {
						loop = new List<ControlFlowNode>();
						loop.Add(h);
						// Mark loop header as visited so that the pre-order traversal
						// stops at the loop header.
						h.Visited = true;
					}
					t.TraversePreOrder(n => n.Predecessors, loop.Add);
				}
			}
			if (loop != null) {
				// loop now is the union of all natural loops with loop head h.
				// Try to extend the loop to reduce the number of exit points:
				ExtendLoop(h, loop, h);
				
				// Sort blocks in the loop in reverse post-order to make the output look a bit nicer.
				// (if the loop doesn't contain nested loops, this is a topological sort)
				loop.Sort((a, b) => b.PostOrderNumber.CompareTo(a.PostOrderNumber));
				Debug.Assert(loop[0] == h);
				foreach (var node in loop) {
					node.Visited = false; // reset visited flag so that we can find nested loops
					Debug.Assert(h.Dominates(node), "The loop body must be dominated by the loop head");
				}
				ConstructLoop(loop);
			}
			// Recurse into the dominator tree to find other possible loop heads
			foreach (var child in h.DominatorTreeChildren) {
				FindLoops(child);
			}
		}
		
		/// <summary>
		/// Given a natural loop, add additional CFG nodes to the loop in order
		/// to reduce the number of exit points out of the loop.
		/// We do this because C# only allows reaching a single exit point (with 'break'
		/// statements or when the loop condition evaluates to false), so we'd have
		/// to introduce 'goto' statements for any additional exit points.
		/// 
		/// Definition: a loop exit point is a CFG node that is not itself part of the loop,
		/// but has at least one predecessor which is part of the loop.
		/// 
		/// Nodes can only be added to the loop if they are dominated by the loop head.
		/// When adding a node to the loop, we implicitly also add all of that node's predecessors
		/// to the loop. (this ensures that the loop keeps its single entry point)
		/// 
		/// Adding a node to the loop has two effects on the the number of exit points:
		/// * exit points that were added to the loop are no longer exit points, thus reducing the total number of exit points
		/// * successors of the newly added nodes might be new, additional exit points
		/// 
		/// The loop extension algorithm proceeds traverses the loop head's dominator tree in pre-order.
		/// For each candidate node, we detect whether adding it to the loop reduces the number of exit points.
		/// If it does, the candidate is added to the loop.
		/// </summary>
		/// <remarks>
		/// Requires and maintains the invariant that a node is marked as visited iff it is contained in the loop.
		/// 
		/// Note: I don't think this works reliably to minimize the number of exit points,
		/// it's just a heuristic that should reduce the number of exit points in most cases.
		/// I think what we're really looking for is a minimum vertex cut of the following flow graph:
		/// * all nodes that are part of the natural loop are combined into a single node (the source node)
		/// * all control flow nodes that are dominated by the loop head (but not part of the loop)
		///   are nodes in the graph
		/// * all nodes that in the loop's dominance frontier are nodes in the graph
		/// * connections are as usual in the CFG
		/// * the nodes in the loop's dominance frontier are additionally connected to the sink node.
		/// 
		/// Also, if the only way to leave the loop is through 'ret' or 'leave' instructions, or 'br' instructions
		/// that leave the block container, this method has the effect of adding more code than necessary to the loop,
		/// as those instructions do not have corresponding control flow edges.
		/// Ideally, 'leave' and 'br' should be also considered exit points; and if there are no other exit points,
		/// we can afford to introduce an additional exit point so that 'ret' instructions and nested infinite loops
		/// don't have to be moved into the loop.
		/// </remarks>
		void ExtendLoop(ControlFlowNode loopHead, List<ControlFlowNode> loop, ControlFlowNode candidate)
		{
			Debug.Assert(candidate.Visited == loop.Contains(candidate));
			if (!candidate.Visited) {
				// This node not yet part of the loop, but might be added
				List<ControlFlowNode> additionalNodes = new List<ControlFlowNode>();
				// Find additionalNodes nodes and mark them as visited.
				candidate.TraversePreOrder(n => n.Predecessors, additionalNodes.Add);
				// This means Visited now represents the candiate extended loop.
				// Determine new exit points that are reachable from the additional nodes
				// (note: some of these might have previously been exit points, too)
				var newExitPoints = additionalNodes.SelectMany(n => n.Successors).Where(n => !n.Visited).ToHashSet();
				// Make visited represent the unextended loop, so that we can measure the exit points
				// in the old state.
				foreach (var node in additionalNodes)
					node.Visited = false;
				// Measure number of added and removed exit points
				int removedExitPoints = additionalNodes.Count(IsExitPoint);
				int addedExitPoints = newExitPoints.Count(n => !IsExitPoint(n));
				if (removedExitPoints > addedExitPoints) {
					// We can reduce the number of exit points by adding the candidate node to the loop.
					candidate.TraversePreOrder(n => n.Predecessors, loop.Add);
				}
			}
			// Pre-order traversal of dominator tree
			foreach (var node in candidate.DominatorTreeChildren) {
				ExtendLoop(loopHead, loop, node);
			}
		}
		
		/// <summary>
		/// Gets whether 'node' is an exit point for the loop marked by the Visited flag.
		/// </summary>
		bool IsExitPoint(ControlFlowNode node)
		{
			if (node.Visited)
				return false; // nodes in the loop are not exit points
			foreach (var pred in node.Predecessors) {
				if (pred.Visited)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Move the blocks associated with the loop into a new block container.
		/// </summary>
		void ConstructLoop(List<ControlFlowNode> loop)
		{
			Block oldEntryPoint = (Block)loop[0].UserData;
			BlockContainer oldContainer = (BlockContainer)oldEntryPoint.Parent;
			
			BlockContainer loopContainer = new BlockContainer();
			Block newEntryPoint = new Block();
			loopContainer.Blocks.Add(newEntryPoint);
			// Move contents of oldEntryPoint to newEntryPoint
			// (we can't move the block itself because it might be the target of branch instructions outside the loop)
			newEntryPoint.Instructions.ReplaceList(oldEntryPoint.Instructions);
			newEntryPoint.FinalInstruction = oldEntryPoint.FinalInstruction;
			newEntryPoint.ILRange = oldEntryPoint.ILRange;
			oldEntryPoint.Instructions.ReplaceList(new[] { loopContainer });
			oldEntryPoint.FinalInstruction = new Nop();
			
			// Move other blocks into the loop body: they're all dominated by the loop header,
			// and thus cannot be the target of branch instructions outside the loop.
			for (int i = 1; i < loop.Count; i++) {
				Block block = (Block)loop[i].UserData;
				Debug.Assert(block.Parent == oldContainer);
				loopContainer.Blocks.Add(block);
			}
			// Remove all blocks that were moved into the body from the old container
			oldContainer.Blocks.RemoveAll(b => b.Parent != oldContainer);
			
			// Rewrite branches within the loop from oldEntryPoint to newEntryPoint:
			foreach (var branch in loopContainer.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock == oldEntryPoint)
					branch.TargetBlock = newEntryPoint;
			}
		}
	}
}
