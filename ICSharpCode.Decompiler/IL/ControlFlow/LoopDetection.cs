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
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.NRefactory.Utils;

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
	/// * Loop detection should run after the 'return block' is duplicated (ControlFlowSimplification).
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
			for (int i = 0; i < bc.Blocks.Count; i++) {
				nodes[i] = new ControlFlowNode { UserIndex = i, UserData = bc.Blocks[i] };
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
		
		ILTransformContext context;
		
		ControlFlowNode[] cfg;
		
		/// <summary>
		/// nodeHasReachableExit[i] == true iff there is a path from cfg[i] to a node not dominated by cfg[i],
		/// or if there is a path from cfg[i] to a branch/leave instruction leaving the currentBlockContainer.
		/// </summary>
		BitSet nodeHasReachableExit;
		
		/// <summary>
		/// nodeHasDirectExitOutOfContainer[i] == true iff cfg[i] directly contains a branch/leave instruction leaving the currentBlockContainer.
		/// </summary>
		BitSet nodeHasDirectExitOutOfContainer;
		
		/// <summary>Block container corresponding to the current cfg.</summary>
		BlockContainer currentBlockContainer;
		
		/// <summary>
		/// Run loop detection for blocks in the block container.
		/// </summary>
		public void Run(BlockContainer blockContainer, ILTransformContext context)
		{
			this.context = context;
			this.currentBlockContainer = blockContainer;
			this.cfg = BuildCFG(blockContainer);
			this.nodeHasReachableExit = null; // will be computed on-demand
			this.nodeHasDirectExitOutOfContainer = null; // will be computed on-demand
			
			var entryPoint = cfg[0];
			Dominance.ComputeDominance(entryPoint, context.CancellationToken);
			FindLoops(entryPoint);
			
			this.cfg = null;
			this.nodeHasReachableExit = null;
			this.nodeHasDirectExitOutOfContainer = null;
			this.currentBlockContainer = null;
			this.context = null;
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
				ExtendLoop(h, loop);
				
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
		
		#region ExtendLoop
		/// <summary>
		/// Given a natural loop, add additional CFG nodes to the loop in order
		/// to reduce the number of exit points out of the loop.
		/// We do this because C# only allows reaching a single exit point (with 'break'
		/// statements or when the loop condition evaluates to false), so we'd have
		/// to introduce 'goto' statements for any additional exit points.
		/// </summary>
		/// <remarks>
		/// Definition:
		/// A "reachable exit" is a branch/leave target that is reachable from the loop,
		/// but not dominated by the loop head. A reachable exit may or may not have a
		/// corresponding CFG node (depending on whether it is a block in the current block container).
		///   -> reachable exits are leaving the code region dominated by the loop
		/// 
		/// Definition:
		/// A loop "exit point" is a CFG node that is not itself part of the loop,
		/// but has at least one predecessor which is part of the loop.
		///   -> exit points are leaving the loop itself
		/// 
		/// Nodes can only be added to the loop if they are dominated by the loop head.
		/// When adding a node to the loop, we must also add all of that node's predecessors
		/// to the loop. (this ensures that the loop keeps its single entry point)
		/// 
		/// Goal: If possible, find a set of nodes that can be added to the loop so that there
		/// remains only a single exit point.
		/// Add as little code as possible to the loop to reach this goal.
		/// 
		/// This means we need to partition the set of nodes dominated by the loop entry point
		/// into two sets (in-loop and out-of-loop).
		/// Constraints:
		///  * the loop head itself is in-loop
		///  * there must not be any edge from an out-of-loop node to an in-loop node
		///    -> all predecessors of in-loop nodes are also in-loop
		///    -> all nodes in a cycle are part of the same partition
		/// Optimize:
		///  * use only a single exit point if at all possible
		///  * minimize the amount of code in the in-loop partition
		///    (thus: maximize the amount of code in the out-of-loop partition)
		///   "amount of code" could be measured as:
		///     * number of basic blocks
		///     * number of instructions directly in those basic blocks (~= number of statements)
		///     * number of instructions in those basic blocks (~= number of expressions)
		///       (we currently use the number of statements)
		/// 
		/// Observations:
		///  * If a node is in-loop, so are all its ancestors in the dominator tree (up to the loop entry point)
		///  * If there are no exits reachable from a node (i.e. all paths from that node lead to a return/throw instruction),
		///    it is valid to put the dominator tree rooted at that node into either partition independently of
		///    any other nodes except for the ancestors in the dominator tree.
		///       (exception: the loop head itself must always be in-loop)
		/// 
		/// There are two different cases we need to consider:
		/// a) There are no exits reachable at all from the loop head.
		///    ->  it is possible to create a loop with zero exit points by adding all nodes
		///        dominated by the loop to the loop.
		///    -> the only way to exit the loop is by "return;" or "throw;"
		/// b) There are some exits reachable from the loop head.
		/// 
		/// In case 1, we can pick a single exit point freely by picking any node that has no reachable exits
		/// (other than the loop head).
		/// All nodes dominated by the exit point are out-of-loop, all other nodes are in-loop.
		/// Maximizing the amount of code in the out-of-loop partition is thus simple: sum up the amount of code
		/// over the dominator tree and pick the node with the maximum amount of code.
		/// 
		/// In case 2, we need to pick our exit point so that all paths from the loop head
		/// to the reachable exits run through that exit point.
		/// 
		/// This is a form of postdominance where the reachable exits are considered exit nodes,
		/// while "return;" or "throw;" instructions are not considered exit nodes.
		/// 
		/// Using this form of postdominance, we are looking for an exit point that post-dominates all nodes in the natural loop.
		/// --> a common ancestor in post-dominator tree.
		/// To minimize the amount of code in-loop, we pick the lowest common ancestor.
		/// All nodes dominated by the exit point are out-of-loop, all other nodes are in-loop.
		/// (using normal dominance as in case 1, not post-dominance!)
		/// 
		/// If it is impossible to use a single exit point for the loop, the lowest common ancestor will be the fake "exit node"
		/// used by the post-dominance analysis. In this case, we fall back to the old heuristic algorithm.
		/// 
		/// Precondition: Requires that a node is marked as visited iff it is contained in the loop.
		/// </remarks>
		void ExtendLoop(ControlFlowNode loopHead, List<ControlFlowNode> loop)
		{
			ComputeNodesWithReachableExits();
			ControlFlowNode exitPoint = FindExitPoint(loopHead, loop);
			Debug.Assert(!loop.Contains(exitPoint), "Cannot pick an exit point that is part of the natural loop");
			if (exitPoint != null) {
				foreach (var node in TreeTraversal.PreOrder(loopHead, n => (n != exitPoint) ? n.DominatorTreeChildren : null)) {
					// TODO: did FindExitPoint really not touch the visited flag?
					if (node != exitPoint && !node.Visited) {
						loop.Add(node);
					}
				}
			} else {
				// TODO: did FindExitPoint really not touch the visited flag?
				ExtendLoopHeuristic(loopHead, loop, loopHead);
			}
		}
		
		void ComputeNodesWithReachableExits()
		{
			if (nodeHasReachableExit != null)
				return;
			nodeHasReachableExit = Dominance.MarkNodesWithReachableExits(cfg);
			nodeHasDirectExitOutOfContainer = new BitSet(cfg.Length);
			// Also mark the nodes that exit the block container altogether.
			// Invariant: leaving[n.UserIndex] == true implies leaving[n.ImmediateDominator.UserIndex] == true
			var leaving = new BitSet(cfg.Length);
			foreach (var node in cfg) {
				if (leaving[node.UserIndex])
					continue;
				if (LeavesCurrentBlockContainer((Block)node.UserData)) {
					nodeHasDirectExitOutOfContainer.Set(node.UserIndex);
					for (ControlFlowNode p = node; p != null; p = p.ImmediateDominator) {
						if (leaving[p.UserIndex]) {
							// we can stop marking when we've reached an already-marked node
							break;
						}
						leaving.Set(p.UserIndex);
					}
				}
			}
			nodeHasReachableExit.UnionWith(leaving);
		}

		bool LeavesCurrentBlockContainer(Block block)
		{
			foreach (var branch in block.Descendants.OfType<Branch>()) {
				if (!branch.TargetBlock.IsDescendantOf(currentBlockContainer)) {
					// control flow that isn't internal to the block container
					return true;
				}
			}
			foreach (var leave in block.Descendants.OfType<Leave>()) {
				if (!leave.TargetContainer.IsDescendantOf(block)) {
					return true;
				}
			}
			return false;
		}
		
		ControlFlowNode FindExitPoint(ControlFlowNode loopHead, IReadOnlyList<ControlFlowNode> naturalLoop)
		{
			if (!nodeHasReachableExit[loopHead.UserIndex]) {
				// Case 1:
				// There are no nodes n so that loopHead dominates a predecessor of n but not n itself
				// -> we could build a loop with zero exit points.
				ControlFlowNode exitPoint = null;
				int exitPointCodeAmount = -1;
				foreach (var node in loopHead.DominatorTreeChildren) {
					PickExitPoint(node, ref exitPoint, ref exitPointCodeAmount);
				}
				return exitPoint;
			} else {
				// Case 2:
				// We need to pick our exit point so that all paths from the loop head
				// to the reachable exits run through that exit point.
				var revCfg = PrepareReverseCFG(loopHead);
				//ControlFlowNode.ExportGraph(cfg).Show("cfg");
				//ControlFlowNode.ExportGraph(revCfg).Show("rev");
				ControlFlowNode commonAncestor = revCfg[loopHead.UserIndex];
				Debug.Assert(commonAncestor.IsReachable);
				foreach (ControlFlowNode cfgNode in naturalLoop) {
					ControlFlowNode revNode = revCfg[cfgNode.UserIndex];
					if (revNode.IsReachable) {
						commonAncestor = Dominance.FindCommonDominator(commonAncestor, revNode);
					}
				}
				ControlFlowNode exitPoint;
				while (commonAncestor.UserIndex >= 0) {
					exitPoint = cfg[commonAncestor.UserIndex];
					Debug.Assert(exitPoint.Visited == naturalLoop.Contains(exitPoint));
					if (exitPoint.Visited) {
						commonAncestor = commonAncestor.ImmediateDominator;
						continue;
					} else {
						return exitPoint;
					}
				}
				// least common dominator is the artificial exit node
				return null;
			}
		}

		/// <summary>
		/// Pick exit point by picking any node that has no reachable exits.
		/// 
		/// Maximizing the amount of code in the out-of-loop partition is thus simple: sum up the amount of code
		/// over the dominator tree and pick the node with the maximum amount of code.
		/// </summary>
		/// <returns>Code amount in <paramref name="node"/> and its dominated nodes.</returns>
		int PickExitPoint(ControlFlowNode node, ref ControlFlowNode exitPoint, ref int exitPointCodeAmount)
		{
			int codeAmount = ((Block)node.UserData).Children.Count;
			foreach (var child in node.DominatorTreeChildren) {
				codeAmount += PickExitPoint(child, ref exitPoint, ref exitPointCodeAmount);
			}
			if (codeAmount > exitPointCodeAmount && !nodeHasReachableExit[node.UserIndex]) {
				// dominanceFrontier(node) is empty
				// -> there are no nodes n so that `node` dominates a predecessor of n but not n itself
				// -> there is no control flow out of `node` back into the loop, so it's usable as exit point
				exitPoint = node;
				exitPointCodeAmount = codeAmount;
			}
			return codeAmount;
		}
		
		ControlFlowNode[] PrepareReverseCFG(ControlFlowNode loopHead)
		{
			ControlFlowNode[] cfg = this.cfg;
			ControlFlowNode[] rev = new ControlFlowNode[cfg.Length + 1];
			for (int i = 0; i < cfg.Length; i++) {
				rev[i] = new ControlFlowNode { UserIndex = i, UserData = cfg[i].UserData };
			}
			ControlFlowNode exitNode = new ControlFlowNode { UserIndex = -1 };
			rev[cfg.Length] = exitNode;
			for (int i = 0; i < cfg.Length; i++) {
				if (!loopHead.Dominates(cfg[i]))
					continue;
				// Add reverse edges for all edges in cfg
				foreach (var succ in cfg[i].Successors) {
					if (loopHead.Dominates(succ)) {
						rev[succ.UserIndex].AddEdgeTo(rev[i]);
					} else {
						exitNode.AddEdgeTo(rev[i]);
					}
				}
				if (nodeHasDirectExitOutOfContainer[i]) {
					exitNode.AddEdgeTo(rev[i]);
				}
			}
			Dominance.ComputeDominance(exitNode, context.CancellationToken);
			return rev;
		}
		#endregion
		
		#region ExtendLoop (fall-back heuristic)
		/// <summary>
		/// This function implements a heuristic algorithm that tries to reduce the number of exit
		/// points. It is only used as fall-back when it is impossible to use a single exit point.
		/// </summary>
		/// <remarks>
		/// This heuristic loop extension algorithm traverses the loop head's dominator tree in pre-order.
		/// For each candidate node, we detect whether adding it to the loop reduces the number of exit points.
		/// If it does, the candidate is added to the loop.
		/// 
		/// Adding a node to the loop has two effects on the the number of exit points:
		/// * exit points that were added to the loop are no longer exit points, thus reducing the total number of exit points
		/// * successors of the newly added nodes might be new, additional exit points
		/// 
		/// Requires and maintains the invariant that a node is marked as visited iff it is contained in the loop.
		/// </remarks>
		void ExtendLoopHeuristic(ControlFlowNode loopHead, List<ControlFlowNode> loop, ControlFlowNode candidate)
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
				ExtendLoopHeuristic(loopHead, loop, node);
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
		#endregion
		
		/// <summary>
		/// Move the blocks associated with the loop into a new block container.
		/// </summary>
		void ConstructLoop(List<ControlFlowNode> loop)
		{
			Block oldEntryPoint = (Block)loop[0].UserData;
			
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
				Debug.Assert(block.ChildIndex != 0);
				var oldParent = ((BlockContainer)block.Parent);
				int oldChildIndex = block.ChildIndex;
				loopContainer.Blocks.Add(block);
				oldParent.Blocks.SwapRemoveAt(oldChildIndex);
			}
			
			// Rewrite branches within the loop from oldEntryPoint to newEntryPoint:
			foreach (var branch in loopContainer.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock == oldEntryPoint)
					branch.TargetBlock = newEntryPoint;
			}
		}
	}
}
