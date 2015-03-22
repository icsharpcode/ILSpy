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

namespace ICSharpCode.Decompiler.IL.Transforms
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
		/// An additional exit node is used to signal when a block potentially falls through
		/// to the endpoint of the BlockContainer.
		/// Return statements, exceptions, or branches leaving the block container are not
		/// modeled by the control flow graph.
		/// </summary>
		static ControlFlowNode BuildCFG(BlockContainer bc)
		{
			ControlFlowNode[] nodes = new ControlFlowNode[bc.Blocks.Count];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = new ControlFlowNode { UserData = bc.Blocks[i] };
			}
			ControlFlowNode exit = new ControlFlowNode();
			
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
				if (!block.HasFlag(InstructionFlags.EndPointUnreachable))
					sourceNode.AddEdgeTo(exit);
			}
			
			if (nodes[0].Predecessors.Count != 0) {
				// Create artificial entry point without predecessors:
				ControlFlowNode entry = new ControlFlowNode();
				entry.AddEdgeTo(nodes[0]);
				return entry;
			} else {
				return nodes[0];
			}
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
			var entryPoint = BuildCFG(blockContainer);
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
				// Sort blocks in the loop in reverse post-order to make the output look a bit nicer.
				// (if the loop doesn't contain nested loops, this is a topological sort)
				loop.Sort((a, b) => b.PostOrderNumber.CompareTo(a.PostOrderNumber));
				Debug.Assert(loop[0] == h); // TODO: is this guaranteed after sorting?
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
