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
using ICSharpCode.Decompiler.Util;

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
	public class LoopDetection : IBlockTransform
	{
		BlockTransformContext context;
		
		/// <summary>Block container corresponding to the current cfg.</summary>
		BlockContainer currentBlockContainer;
		
		/// <summary>
		/// Check whether 'block' is a loop head; and construct a loop instruction
		/// (nested BlockContainer) if it is.
		/// </summary>
		public void Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			// LoopDetection runs early enough so that block should still
			// be in the original container at this point.
			Debug.Assert(block.Parent == context.ControlFlowGraph.Container);
			this.currentBlockContainer = context.ControlFlowGraph.Container;

			// Because this is a post-order block transform, we can assume that
			// any nested loops within this loop have already been constructed.

			if (block.Instructions.Last() is SwitchInstruction switchInst) {
				// Switch instructions support "break;" just like loops
				DetectSwitchBody(block, switchInst);
			}

			ControlFlowNode h = context.ControlFlowNode; // CFG node for our potential loop head
			Debug.Assert(h.UserData == block);
			Debug.Assert(!TreeTraversal.PreOrder(h, n => n.DominatorTreeChildren).Any(n => n.Visited));

			List<ControlFlowNode> loop = null;
			foreach (var t in h.Predecessors) {
				if (h.Dominates(t)) {
					// h->t is a back edge, and h is a loop header
					// Add the natural loop of t->h to the loop.

					// Definitions:
					// * A back edge is an edge t->h so that h dominates t.
					// * The natural loop of the back edge is the smallest set of nodes
					//   that includes the back edge and has no predecessors outside the set
					//   except for the predecessor of the header.

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
				var headBlock = (Block)h.UserData;
				context.Step($"Construct loop with head {headBlock.Label}", headBlock);
				// loop now is the union of all natural loops with loop head h.
				// Try to extend the loop to reduce the number of exit points:
				ExtendLoop(h, loop, out var exitPoint);

				// Sort blocks in the loop in reverse post-order to make the output look a bit nicer.
				// (if the loop doesn't contain nested loops, this is a topological sort)
				loop.Sort((a, b) => b.PostOrderNumber.CompareTo(a.PostOrderNumber));
				Debug.Assert(loop[0] == h);
				foreach (var node in loop) {
					node.Visited = false; // reset visited flag so that we can find outer loops
					Debug.Assert(h.Dominates(node) || !node.IsReachable, "The loop body must be dominated by the loop head");
				}
				ConstructLoop(loop, exitPoint);
			}
		}

		/// <summary>
		/// Recurse into the dominator tree and find back edges/natural loops.
		/// </summary>
		/// <remarks>
		/// 
		/// Preconditions:
		/// * dominance was computed for h
		/// * all blocks in the dominator subtree starting at h are in the same BlockContainer
		/// * the visited flag is set to false
		/// </remarks>
		void FindLoops(ControlFlowNode h)
		{
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
		///    it is valid to put the group of nodes dominated by that node into either partition independently of
		///    any other nodes except for the ancestors in the dominator tree.
		///       (exception: the loop head itself must always be in-loop)
		/// 
		/// There are two different cases we need to consider:
		/// 1) There are no exits reachable at all from the loop head.
		///    ->  it is possible to create a loop with zero exit points by adding all nodes
		///        dominated by the loop to the loop.
		///    -> the only way to exit the loop is by "return;" or "throw;"
		/// 2) There are some exits reachable from the loop head.
		/// 
		/// In case 1, we can pick a single exit point freely by picking any node that has no reachable exits
		/// (other than the loop head).
		/// All nodes dominated by the exit point are out-of-loop, all other nodes are in-loop.
		/// See PickExitPoint() for the heuristic that picks the exit point in this case.
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
		void ExtendLoop(ControlFlowNode loopHead, List<ControlFlowNode> loop, out ControlFlowNode exitPoint)
		{
			exitPoint = FindExitPoint(loopHead, loop);
			Debug.Assert(!loop.Contains(exitPoint), "Cannot pick an exit point that is part of the natural loop");
			if (exitPoint != null) {
				// Either we are in case 1 and just picked an exit that maximizes the amount of code
				// outside the loop, or we are in case 2 and found an exit point via post-dominance.
				var ep = exitPoint;
				foreach (var node in TreeTraversal.PreOrder(loopHead, n => (n != ep) ? n.DominatorTreeChildren : null)) {
					if (node != exitPoint && !node.Visited) {
						loop.Add(node);
					}
				}
			} else {
				// We are in case 2, but could not find a suitable exit point.
				// Heuristically try to minimize the number of exit points
				// (but we'll always end up with more than 1 exit and will require goto statements).
				ExtendLoopHeuristic(loopHead, loop, loopHead);
			}
		}
		
		/// <summary>
		/// Finds a suitable single exit point for the specified loop.
		/// </summary>
		/// <remarks>This method must not write to the Visited flags on the CFG.</remarks>
		ControlFlowNode FindExitPoint(ControlFlowNode loopHead, IReadOnlyList<ControlFlowNode> naturalLoop)
		{
			if (!context.ControlFlowGraph.HasReachableExit(loopHead)) {
				// Case 1:
				// There are no nodes n so that loopHead dominates a predecessor of n but not n itself
				// -> we could build a loop with zero exit points.
				ControlFlowNode exitPoint = null;
				int exitPointILOffset = -1;
				foreach (var node in loopHead.DominatorTreeChildren) {
					PickExitPoint(node, ref exitPoint, ref exitPointILOffset);
				}
				return exitPoint;
			} else {
				// Case 2:
				// We need to pick our exit point so that all paths from the loop head
				// to the reachable exits run through that exit point.
				var cfg = context.ControlFlowGraph.cfg;
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
		/// In the common case where the code was compiled with a compiler that emits IL code
		/// in source order (like the C# compiler), we can find the "real" exit point
		/// by simply picking the block with the highest IL offset.
		/// So let's do that instead of maximizing amount of code.
		/// </summary>
		/// <returns>Code amount in <paramref name="node"/> and its dominated nodes.</returns>
		/// <remarks>This method must not write to the Visited flags on the CFG.</remarks>
		void PickExitPoint(ControlFlowNode node, ref ControlFlowNode exitPoint, ref int exitPointILOffset)
		{
			Block block = (Block)node.UserData;
			if (block.ILRange.Start > exitPointILOffset
				&& !context.ControlFlowGraph.HasReachableExit(node)
				&& ((Block)node.UserData).Parent == currentBlockContainer)
			{
				// HasReachableExit(node) == false
				// -> there are no nodes n so that `node` dominates a predecessor of n but not n itself
				// -> there is no control flow out of `node` back into the loop, so it's usable as exit point

				// Additionally, we require that the block wasn't already moved into a nested loop,
				// since there's no way to jump into the middle of that loop when we need to exit.
				// NB: this is the only reason why we detect nested loops before outer loops:
				// If we detected the outer loop first, the outer loop might pick an exit point
				// that prevents us from finding a nice exit for the inner loops, causing
				// unnecessary gotos.
				exitPoint = node;
				exitPointILOffset = block.ILRange.Start;
				return; // don't visit children, they are likely to have even later IL offsets and we'd end up
				// moving almost all of the code into the loop.
			}
			foreach (var child in node.DominatorTreeChildren) {
				PickExitPoint(child, ref exitPoint, ref exitPointILOffset);
			}
		}
		
		ControlFlowNode[] PrepareReverseCFG(ControlFlowNode loopHead)
		{
			ControlFlowNode[] cfg = context.ControlFlowGraph.cfg;
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
				if (context.ControlFlowGraph.HasDirectExitOutOfContainer(cfg[i])) {
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
		void ConstructLoop(List<ControlFlowNode> loop, ControlFlowNode exitPoint)
		{
			Block oldEntryPoint = (Block)loop[0].UserData;
			Block exitTargetBlock = (Block)exitPoint?.UserData;

			BlockContainer loopContainer = new BlockContainer();
			Block newEntryPoint = new Block();
			loopContainer.Blocks.Add(newEntryPoint);
			// Move contents of oldEntryPoint to newEntryPoint
			// (we can't move the block itself because it might be the target of branch instructions outside the loop)
			newEntryPoint.Instructions.ReplaceList(oldEntryPoint.Instructions);
			newEntryPoint.ILRange = oldEntryPoint.ILRange;
			oldEntryPoint.Instructions.ReplaceList(new[] { loopContainer });
			if (exitTargetBlock != null)
				oldEntryPoint.Instructions.Add(new Branch(exitTargetBlock));

			MoveBlocksIntoContainer(loop, loopContainer);

			// Rewrite branches within the loop from oldEntryPoint to newEntryPoint:
			foreach (var branch in loopContainer.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock == oldEntryPoint) {
					branch.TargetBlock = newEntryPoint;
				} else if (branch.TargetBlock == exitTargetBlock) {
					branch.ReplaceWith(new Leave(loopContainer) { ILRange = branch.ILRange });
				}
			}
		}

		private void MoveBlocksIntoContainer(List<ControlFlowNode> loop, BlockContainer loopContainer)
		{
			// Move other blocks into the loop body: they're all dominated by the loop header,
			// and thus cannot be the target of branch instructions outside the loop.
			for (int i = 1; i < loop.Count; i++) {
				Block block = (Block)loop[i].UserData;
				// some blocks might already be in use by nested loops that were detected earlier;
				// don't move those (they'll be implicitly moved when the block containing the
				// nested loop container is moved).
				if (block.Parent == currentBlockContainer) {
					Debug.Assert(block.ChildIndex != 0);
					int oldChildIndex = block.ChildIndex;
					loopContainer.Blocks.Add(block);
					currentBlockContainer.Blocks.SwapRemoveAt(oldChildIndex);
				}
			}
			for (int i = 1; i < loop.Count; i++) {
				// Verify that we moved all loop blocks into the loop container.
				// If we wanted to move any blocks already in use by a nested loop,
				// this means we check that the whole nested loop got moved.
				Block block = (Block)loop[i].UserData;
				Debug.Assert(block.IsDescendantOf(loopContainer));
			}
		}

		private void DetectSwitchBody(Block block, SwitchInstruction switchInst)
		{
			Debug.Assert(block.Instructions.Last() == switchInst);
			ControlFlowNode h = context.ControlFlowNode; // CFG node for our potential loop head
			Debug.Assert(h.UserData == block);
			Debug.Assert(!TreeTraversal.PreOrder(h, n => n.DominatorTreeChildren).Any(n => n.Visited));

			var nodesInSwitch = new List<ControlFlowNode>();
			nodesInSwitch.Add(h);
			h.Visited = true;
			ExtendLoop(h, nodesInSwitch, out var exitPoint);

			context.Step("Create BlockContainer for switch", switchInst);
			// Sort blocks in the loop in reverse post-order to make the output look a bit nicer.
			// (if the loop doesn't contain nested loops, this is a topological sort)
			nodesInSwitch.Sort((a, b) => b.PostOrderNumber.CompareTo(a.PostOrderNumber));
			Debug.Assert(nodesInSwitch[0] == h);
			foreach (var node in nodesInSwitch) {
				node.Visited = false; // reset visited flag so that we can find outer loops
				Debug.Assert(h.Dominates(node) || !node.IsReachable, "The switch body must be dominated by the switch head");
			}

			BlockContainer switchContainer = new BlockContainer();
			Block newEntryPoint = new Block();
			newEntryPoint.ILRange = switchInst.ILRange;
			switchContainer.Blocks.Add(newEntryPoint);
			newEntryPoint.Instructions.Add(switchInst);
			block.Instructions[block.Instructions.Count - 1] = switchContainer;

			Block exitTargetBlock = (Block)exitPoint?.UserData;
			if (exitTargetBlock != null) {
				block.Instructions.Add(new Branch(exitTargetBlock));
			}

			MoveBlocksIntoContainer(nodesInSwitch, switchContainer);

			// Rewrite branches within the loop from oldEntryPoint to newEntryPoint:
			foreach (var branch in switchContainer.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock == exitTargetBlock) {
					branch.ReplaceWith(new Leave(switchContainer) { ILRange = branch.ILRange });
				}
			}
		}
	}
}
