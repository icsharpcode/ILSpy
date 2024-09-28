// Copyright (c) 2018 Siegfried Pammer
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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Improves code quality by duplicating keyword exits to reduce nesting and restoring IL order.
	/// </summary>
	/// <remarks>
	/// ConditionDetection and DetectSwitchBody both have aggressive inlining policies for else blocks and default cases respectively.
	/// This can lead to excessive indentation when the entire rest of the method/loop is included in the else block/default case.
	/// When an If/SwitchInstruction is followed immediately by a keyword exit, the exit can be moved into the child blocks
	/// allowing the else block or default case to be moved after the if/switch as all prior cases exit.
	/// Most importantly, this transformation does not change the IL order of any code.
	///
	/// ConditionDetection also has a block exit priority system to assist exit point reduction which in some cases ignores IL order.
	/// After HighLevelLoopTransform has run, all structures have been detected and preference can be returned to maintaining IL ordering.
	/// </remarks>
	public class ReduceNestingTransform : IILTransform
	{
		private ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			Visit((BlockContainer)function.Body, null);

			foreach (var node in function.Descendants.OfType<TryFinally>())
			{
				EliminateRedundantTryFinally(node, context);
			}
		}

		private void Visit(BlockContainer container, Block? continueTarget)
		{
			switch (container.Kind)
			{
				case ContainerKind.Loop:
				case ContainerKind.While:
					continueTarget = container.EntryPoint;
					break;
				case ContainerKind.DoWhile:
				case ContainerKind.For:
					continueTarget = container.Blocks.Last();
					break;
			}

			for (int i = 0; i < container.Blocks.Count; i++)
			{
				var block = container.Blocks[i];
				// Note: it's possible for additional blocks to be appended to the container
				// by the Visit() call; but there should be no other changes to the Blocks collection.
				Visit(block, continueTarget);
				Debug.Assert(container.Blocks[i] == block);
			}
		}

		/// <summary>
		/// Visits a block in context
		/// </summary>
		/// <param name="block"></param>
		/// <param name="continueTarget">Marks the target block of continue statements.</param>
		/// <param name="nextInstruction">The instruction following the end point of the block. Can only be null if the end point is unreachable.</param>
		private void Visit(Block block, Block? continueTarget, ILInstruction? nextInstruction = null)
		{
			Debug.Assert(block.HasFlag(InstructionFlags.EndPointUnreachable) || nextInstruction != null);

			// process each instruction in the block.
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				//  Transformations may be applied to the current and following instructions but already processed instructions will not be changed
				var inst = block.Instructions[i];

				// the next instruction to be executed. Transformations will change the next instruction, so this is a method instead of a variable
				ILInstruction NextInsn() => block.Instructions.ElementAtOrDefault(i + 1) ?? nextInstruction;

				switch (inst)
				{
					case BlockContainer container:
						// visit the contents of the container
						Visit(container, continueTarget);

						// reduce nesting in switch blocks
						if (container.Kind == ContainerKind.Switch &&
							CanDuplicateExit(NextInsn(), continueTarget, out var keywordExit1) &&
							ReduceSwitchNesting(block, container, keywordExit1))
						{
							RemoveRedundantExit(block, nextInstruction);
						}
						break;
					case IfInstruction ifInst:
						ImproveILOrdering(block, ifInst, continueTarget);

						// reduce nesting in if/else blocks
						if (CanDuplicateExit(NextInsn(), continueTarget, out var keywordExit2) && ReduceNesting(block, ifInst, keywordExit2))
							RemoveRedundantExit(block, nextInstruction);

						// visit content blocks
						if (ifInst.TrueInst is Block trueBlock)
							Visit(trueBlock, continueTarget, NextInsn());

						if (ifInst.FalseInst is Block falseBlock)
						{
							if (ifInst.TrueInst.HasFlag(InstructionFlags.EndPointUnreachable))
							{
								ExtractElseBlock(ifInst);
								break;
							}

							Visit(falseBlock, continueTarget, NextInsn());
						}
						break;
					default:
						// blocks can only exit containers via leave instructions, not fallthrough, so the only relevant context is `continueTarget`
						VisitContainers(inst, continueTarget);

						// reducing nesting inside Try/Using/Lock etc, may make the endpoint unreachable. 
						// This should only happen by replacing a Leave with the exit instruction we're about to delete, but I can't see a good way to assert this
						// This would be better placed in ReduceNesting, but it's more difficult to find the affected instructions/blocks there than here
						if (i == block.Instructions.Count - 2 && inst.HasFlag(InstructionFlags.EndPointUnreachable))
						{
							context.Step("Remove unreachable exit", block.Instructions.Last());
							block.Instructions.RemoveLast();

							// This would be the right place to check and fix the redundant continue; in TestCases.Pretty.ReduceNesting.BreakLockInLoop
							// but doing so would require knowledge of what `inst` is, and how it works. (eg. to target the try block and not catch or finally blocks)
						}
						break;
				}
			}
		}

		// search for child containers to reduce nesting in
		private void VisitContainers(ILInstruction inst, Block continueTarget)
		{
			switch (inst)
			{
				case ILFunction _:
					break; // assume inline ILFunctions are already transformed
				case BlockContainer cont:
					Visit(cont, continueTarget);
					break;
				default:
					foreach (var child in inst.Children)
						VisitContainers(child, continueTarget);
					break;
			}
		}

		/// <summary>
		/// For an if statement with an unreachable end point and no else block,
		/// inverts to match IL order of the first statement of each branch
		/// </summary>
		private void ImproveILOrdering(Block block, IfInstruction ifInst, Block? continueTarget)
		{
			if (!block.HasFlag(InstructionFlags.EndPointUnreachable)
				|| !ifInst.TrueInst.HasFlag(InstructionFlags.EndPointUnreachable)
				|| !ifInst.FalseInst.MatchNop())
				return;

			Debug.Assert(ifInst != block.Instructions.Last());

			var trueRangeStart = ConditionDetection.GetStartILOffset(ifInst.TrueInst, out bool trueRangeIsEmpty);
			var falseRangeStart = ConditionDetection.GetStartILOffset(block.Instructions[block.Instructions.IndexOf(ifInst) + 1], out bool falseRangeIsEmpty);
			if (trueRangeIsEmpty || falseRangeIsEmpty || falseRangeStart >= trueRangeStart)
				return;

			if (block.Instructions.Last() is Leave leave && !leave.IsLeavingFunction && leave.TargetContainer.Kind == ContainerKind.Normal)
			{
				// non-keyword leave. Can't move out of the last position in the block (fall-through) without introducing goto, unless it can be replaced with a keyword (return/continue)
				if (!CanDuplicateExit(block.Instructions.Last(), continueTarget, out var keywordExit))
					return;

				context.Step("Replace leave with keyword exit", ifInst.TrueInst);
				block.Instructions.Last().ReplaceWith(keywordExit.Clone());
			}

			ConditionDetection.InvertIf(block, ifInst, context);
		}

		/// <summary>
		/// Reduce Nesting in if/else statements by duplicating an exit instruction.
		/// Does not affect IL order
		/// </summary>
		private bool ReduceNesting(Block block, IfInstruction ifInst, ILInstruction exitInst)
		{
			// start tallying stats for heuristics from then and else-if blocks
			int maxStatements = 0, maxDepth = 0;
			UpdateStats(ifInst.TrueInst, ref maxStatements, ref maxDepth);

			// if (cond) { ... } exit;
			if (ifInst.FalseInst.MatchNop())
			{
				// a separate heuristic to ShouldReduceNesting as there is visual balancing to be performed based on number of statments
				if (maxDepth < 2)
					return false;

				//   ->
				// if (!cond) exit;
				// ...; exit;
				EnsureEndPointUnreachable(block, exitInst);
				Debug.Assert(ifInst == block.Instructions.SecondToLastOrDefault());

				// use the same exit the block has. If the block already has one (such as a leave from a try), keep it in place
				EnsureEndPointUnreachable(ifInst.TrueInst, block.Instructions.Last());
				ConditionDetection.InvertIf(block, ifInst, context);

				// ensure the exit inst of the if instruction is a keyword
				Debug.Assert(!(ifInst.TrueInst is Block));
				if (!ifInst.TrueInst.Match(exitInst).Success)
				{
					Debug.Assert(ifInst.TrueInst is Leave);
					context.Step("Replace leave with keyword exit", ifInst.TrueInst);
					ifInst.TrueInst.ReplaceWith(exitInst.Clone());
				}
				return true;
			}

			// else-if trees are considered as a single group from the root IfInstruction
			if (GetElseIfParent(ifInst) != null)
				return false;

			// find the else block and tally stats for each else-if block
			while (Block.Unwrap(ifInst.FalseInst) is IfInstruction elseIfInst)
			{
				UpdateStats(elseIfInst.TrueInst, ref maxStatements, ref maxDepth);
				ifInst = elseIfInst;
			}

			if (!ShouldReduceNesting(ifInst.FalseInst, maxStatements, maxDepth))
				return false;

			// extract the else block and insert exit points all the way up the else-if tree
			do
			{
				var elseIfInst = GetElseIfParent(ifInst);

				// if (cond) { ... } else { ... } exit;
				//   ->
				// if (cond) { ...; exit; }
				// ...; exit;
				EnsureEndPointUnreachable(ifInst.TrueInst, exitInst);
				if (ifInst.FalseInst.HasFlag(InstructionFlags.EndPointUnreachable))
				{
					Debug.Assert(ifInst.HasFlag(InstructionFlags.EndPointUnreachable));
					Debug.Assert(ifInst.Parent == block);
					int removeAfter = ifInst.ChildIndex + 1;
					if (removeAfter < block.Instructions.Count)
					{
						// Remove all instructions that ended up dead
						// (this should just be exitInst itself)
						Debug.Assert(block.Instructions.SecondToLastOrDefault() == ifInst);
						Debug.Assert(block.Instructions.Last() == exitInst);
						block.Instructions.RemoveRange(removeAfter, block.Instructions.Count - removeAfter);
					}
				}
				ExtractElseBlock(ifInst);
				ifInst = elseIfInst;
			} while (ifInst != null);

			return true;
		}

		/// <summary>
		/// Reduce Nesting in switch statements by replacing break; in cases with the block exit, and extracting the default case
		/// Does not affect IL order
		/// </summary>
		private bool ReduceSwitchNesting(Block parentBlock, BlockContainer switchContainer, ILInstruction exitInst)
		{
			// break; from outer container cannot be brought inside the switch as the meaning would change
			if (exitInst is Leave leave && !leave.IsLeavingFunction)
				return false;

			// find the default section, and ensure it has only one incoming edge
			var switchInst = (SwitchInstruction)switchContainer.EntryPoint.Instructions.Single();
			var defaultSection = switchInst.Sections.MaxBy(s => s.Labels.Count());
			if (!defaultSection.Body.MatchBranch(out var defaultBlock) || defaultBlock.IncomingEdgeCount != 1)
				return false;
			if (defaultBlock.Parent != switchContainer)
				return false;

			// tally stats for heuristic from each case block
			int maxStatements = 0, maxDepth = 0;
			foreach (var section in switchInst.Sections)
				if (section != defaultSection && section.Body.MatchBranch(out var caseBlock) && caseBlock.Parent == switchContainer)
					UpdateStats(caseBlock, ref maxStatements, ref maxDepth);

			if (!ShouldReduceNesting(defaultBlock, maxStatements, maxDepth))
				return false;

			Debug.Assert(defaultBlock.HasFlag(InstructionFlags.EndPointUnreachable));

			// ensure the default case dominator tree has no exits (branches to other cases)
			var cfg = new ControlFlowGraph(switchContainer, context.CancellationToken);
			var defaultNode = cfg.GetNode(defaultBlock);
			var defaultTree = TreeTraversal.PreOrder(defaultNode, n => n.DominatorTreeChildren).ToList();
			if (defaultTree.SelectMany(n => n.Successors).Any(n => !defaultNode.Dominates(n)))
				return false;

			if (defaultTree.Count > 1 && !(parentBlock.Parent is BlockContainer))
				return false;

			context.Step("Extract default case of switch", switchContainer);

			// if the switch container is followed by an instruction, it must be a Leave from a try/pinned/etc or exitInst
			// When it's a leave from a container, it's better to let the extracted default block 'fall through' rather than duplicating whatever
			// instruction eventually follows the container
			if (parentBlock.Instructions.SecondToLastOrDefault() == switchContainer)
			{
				if (defaultBlock.Instructions.Last().MatchLeave(switchContainer))
					defaultBlock.Instructions.Last().ReplaceWith(parentBlock.Instructions.Last());

				parentBlock.Instructions.RemoveLast();
			}

			// replace all break; statements with the exitInst
			var leaveInstructions = switchContainer.Descendants.Where(inst => inst.MatchLeave(switchContainer));
			foreach (var leaveInst in leaveInstructions.ToArray())
				leaveInst.ReplaceWith(exitInst.Clone());

			// replace the default section branch with a break;
			defaultSection.Body.ReplaceWith(new Leave(switchContainer));

			// remove all default blocks from the switch container
			var defaultBlocks = defaultTree.Select(c => (Block)c.UserData).ToList();
			foreach (var block in defaultBlocks)
				switchContainer.Blocks.Remove(block);

			Debug.Assert(parentBlock.Instructions.Last() == switchContainer);
			parentBlock.Instructions.AddRange(defaultBlock.Instructions);

			// add any additional blocks from the default case to the parent container
			Debug.Assert(defaultBlocks[0] == defaultBlock);
			if (defaultBlocks.Count > 1)
			{
				var parentContainer = (BlockContainer)parentBlock.Parent;
				int insertAt = parentContainer.Blocks.IndexOf(parentBlock) + 1;
				foreach (var block in defaultBlocks.Skip(1))
					parentContainer.Blocks.Insert(insertAt++, block);
			}

			return true;
		}

		/// <summary>
		/// Checks if an exit is a duplicable keyword exit (return; break; continue;)
		/// </summary>
		private bool CanDuplicateExit(ILInstruction exit, Block? continueTarget, out ILInstruction keywordExit)
		{
			keywordExit = exit;
			if (exit != null && exit.MatchBranch(continueTarget))
				return true;  // keyword is continue

			if (!(exit is Leave leave && leave.Value.MatchNop()))
				return false; // don't duplicate valued returns

			if (leave.IsLeavingFunction || leave.TargetContainer.Kind != ContainerKind.Normal)
				return true; // keyword is return || break

			// leave from a try/pinned/lock etc, check if the target (the instruction following the target container) is duplicable, if so, set keywordExit to that
			ILInstruction leavingInst = leave.TargetContainer;
			Debug.Assert(!leavingInst.HasFlag(InstructionFlags.EndPointUnreachable));
			while (!(leavingInst.Parent is Block b) || leavingInst == b.Instructions.Last())
			{
				if (leavingInst.Parent is TryFinally tryFinally)
				{
					if (leavingInst.SlotInfo == TryFinally.FinallyBlockSlot)
					{ // cannot duplicate leaves from finally containers
						Debug.Assert(leave.TargetContainer == tryFinally.FinallyBlock); //finally cannot have control flow
						return false;
					}
					if (tryFinally.HasFlag(InstructionFlags.EndPointUnreachable))
					{ // finally block changes return value/throws an exception? Yikes. Lets leave it alone
						Debug.Assert(tryFinally.FinallyBlock.HasFlag(InstructionFlags.EndPointUnreachable));
						return false;
					}
				}
				else if (leavingInst.Parent is TryFault tryFault && leavingInst.SlotInfo == TryFault.FaultBlockSlot)
				{ // cannot duplicate leaves from fault containers either
					Debug.Assert(leave.TargetContainer == tryFault.FaultBlock);
					return false;
				}

				leavingInst = leavingInst.Parent;
				Debug.Assert(!leavingInst.HasFlag(InstructionFlags.EndPointUnreachable));
				Debug.Assert(!(leavingInst is ILFunction));
			}

			var block = (Block)leavingInst.Parent;
			var targetInst = block.Instructions[block.Instructions.IndexOf(leavingInst) + 1];
			return CanDuplicateExit(targetInst, continueTarget, out keywordExit);
		}

		/// <summary>
		/// Ensures the end point of a block is unreachable by duplicating and appending the [exit] instruction following the end point
		/// </summary>
		/// <param name="inst">The instruction/block of interest</param>
		/// <param name="fallthroughExit">The next instruction to be executed (provided inst does not exit)</param>
		private void EnsureEndPointUnreachable(ILInstruction inst, ILInstruction fallthroughExit)
		{
			if (!(inst is Block block))
			{
				Debug.Assert(inst.HasFlag(InstructionFlags.EndPointUnreachable));
				return;
			}

			if (!block.HasFlag(InstructionFlags.EndPointUnreachable))
			{
				context.Step("Duplicate block exit", fallthroughExit);
				block.Instructions.Add(fallthroughExit.Clone());
			}
		}

		/// <summary>
		/// Removes a redundant block exit instruction.
		/// </summary>
		private void RemoveRedundantExit(Block block, ILInstruction implicitExit)
		{
			if (block.Instructions.Last().Match(implicitExit).Success)
			{
				context.Step("Remove redundant exit", block.Instructions.Last());
				block.Instructions.RemoveLast();
			}
		}

		/// <summary>
		/// Determines if an IfInstruction is an else-if and returns the preceeding (parent) IfInstruction
		///
		/// [else-]if (parent-cond) else { ifInst }
		/// </summary>
		private IfInstruction? GetElseIfParent(IfInstruction ifInst)
		{
			Debug.Assert(ifInst.Parent is Block);
			if (Block.Unwrap(ifInst.Parent) == ifInst && // only instruction in block
					ifInst.Parent.Parent is IfInstruction elseIfInst && // parent of block is an IfInstruction
					elseIfInst.FalseInst == ifInst.Parent) // part of the false branch not the true branch
				return elseIfInst;

			return null;
		}

		/// <summary>
		/// Adds a code path to the current heuristic tally
		/// </summary>
		private void UpdateStats(ILInstruction inst, ref int maxStatements, ref int maxDepth)
		{
			int numStatements = 0;
			ComputeStats(inst, ref numStatements, ref maxDepth, 0);
			maxStatements = Math.Max(numStatements, maxStatements);
		}

		/// <summary>
		/// Recursively computes the number of statements and maximum nested depth of an instruction
		/// </summary>
		private void ComputeStats(ILInstruction inst, ref int numStatements, ref int maxDepth, int currentDepth, bool isStatement = true)
		{
			if (isStatement)
				numStatements++;

			if (currentDepth > maxDepth)
			{
				Debug.Assert(isStatement);
				maxDepth = currentDepth;
			}

			// enumerate children statements and containers
			switch (inst)
			{
				case Block block:
					if (isStatement)
						numStatements--; // don't count blocks as statements

					// add each child as a statement (unless we're a named block)
					foreach (var child in block.Instructions)
						ComputeStats(child, ref numStatements, ref maxDepth, currentDepth, block.Kind != BlockKind.CallWithNamedArgs && block.Kind != BlockKind.CallInlineAssign);

					// final instruction as an expression
					ComputeStats(block.FinalInstruction, ref numStatements, ref maxDepth, currentDepth, false);
					break;
				case BlockContainer container:
					if (!isStatement)
						numStatements++; //always add a statement for a container in an expression

					var containerBody = container.EntryPoint;
					if (container.Kind == ContainerKind.For || container.Kind == ContainerKind.While)
					{
						Debug.Assert(isStatement);

						if (!container.MatchConditionBlock(container.EntryPoint, out _, out containerBody))
							throw new NotSupportedException("Invalid condition block in loop.");
					}

					// don't count implicit leave. Can't avoid counting for loop initializers but close enough, for loops can have an extra statement of visual weight
					var lastInst = containerBody.Instructions.Last();
					if ((container.Kind == ContainerKind.For || container.Kind == ContainerKind.DoWhile) && lastInst.MatchBranch(container.Blocks.Last()) ||
						(container.Kind == ContainerKind.Loop || container.Kind == ContainerKind.While) && lastInst.MatchBranch(container.Blocks[0]) ||
						 container.Kind == ContainerKind.Normal && lastInst.MatchLeave(container) ||
						 container.Kind == ContainerKind.Switch) // SwitchInstructyion always counts as a statement anyway, so no need to count the container as well
						numStatements--;

					// add the nested body
					ComputeStats(containerBody, ref numStatements, ref maxDepth, currentDepth + 1);
					break;
				case IfInstruction ifInst when ifInst.ResultType == StackType.Void:
					Debug.Assert(isStatement);
					// nested then instruction
					ComputeStats(ifInst.TrueInst, ref numStatements, ref maxDepth, currentDepth + 1);

					// include all nested else-if instructions at the same depth
					var elseInst = ifInst.FalseInst;
					while (Block.Unwrap(elseInst) is IfInstruction elseIfInst)
					{
						numStatements++;
						ComputeStats(elseIfInst.TrueInst, ref numStatements, ref maxDepth, currentDepth + 1);
						elseInst = elseIfInst.FalseInst;
					}

					// include all nested else instruction
					ComputeStats(elseInst, ref numStatements, ref maxDepth, currentDepth + 1);
					break;
				case SwitchSection section:
					Debug.Assert(!isStatement); // labels are just children of the SwitchInstruction
					numStatements++; // add a statement for each case label

					// add all the case blocks at the current depth
					// most formatters indent switch blocks twice, but we don't want this heuristic to be based on formatting
					// so we remain conservative and only include the increase in depth from the container and not the labels
					if (section.Body.MatchBranch(out var caseBlock) && caseBlock.Parent == section.Parent.Parent.Parent)
						ComputeStats(caseBlock, ref numStatements, ref maxDepth, currentDepth);
					break;
				case ILFunction func:
					int bodyStatements = 0;
					int bodyMaxDepth = maxDepth;
					ComputeStats(func.Body, ref bodyStatements, ref bodyMaxDepth, currentDepth);
					if (bodyStatements >= 2)
					{ // don't count inline functions
						numStatements += bodyStatements;
						maxDepth = bodyMaxDepth;
					}
					break;
				default:
					// search each child instruction. Containers will contain statements and contribute to stats
					int subStatements = 0;
					foreach (var child in inst.Children)
						ComputeStats(child, ref subStatements, ref maxDepth, currentDepth, false);

					numStatements += subStatements;
					if (isStatement && subStatements > 0)
						numStatements--; // don't count the first container, only its contents, because this statement is already counted
					break;
			}
		}

		/// <summary>
		/// Heuristic to determine whether it is worth duplicating exits into the preceeding sibling blocks (then/else-if/case)
		/// in order to reduce the nesting of inst by 1
		/// </summary>
		/// <param name="inst">The instruction heading the nested candidate block</param>
		/// <param name="maxStatements">The number of statements in the largest sibling block</param>
		/// <param name="maxDepth">The relative depth of the most nested statement in the sibling blocks</param>
		/// <returns></returns>
		private bool ShouldReduceNesting(ILInstruction inst, int maxStatements, int maxDepth)
		{
			int maxStatements2 = 0, maxDepth2 = 0;
			UpdateStats(inst, ref maxStatements2, ref maxDepth2);
			// if the max depth is 2, always reduce nesting (total depth 3 or more)
			// if the max depth is 1, reduce nesting if this block is the largest
			// otherwise reduce nesting only if this block is twice as large as any other
			return maxDepth2 >= 2 || maxDepth2 >= 1 && maxStatements2 > maxStatements || maxStatements2 >= 2 * maxStatements;
		}

		/// <summary>
		/// if (cond) { ...; exit; } else { ... }
		/// ...;
		///   ->
		/// if (cond) { ...; exit; }
		/// ...;
		/// ...;
		/// </summary>
		/// <param name="ifInst"></param>
		private void ExtractElseBlock(IfInstruction ifInst)
		{
			Debug.Assert(ifInst.TrueInst.HasFlag(InstructionFlags.EndPointUnreachable));
			var block = (Block)ifInst.Parent;
			var falseBlock = (Block)ifInst.FalseInst;

			context.Step("Extract else block", ifInst);
			int insertAt = block.Instructions.IndexOf(ifInst) + 1;
			for (int i = 0; i < falseBlock.Instructions.Count; i++)
				block.Instructions.Insert(insertAt++, falseBlock.Instructions[i]);

			ifInst.FalseInst = new Nop();
		}

		private void EliminateRedundantTryFinally(TryFinally tryFinally, ILTransformContext context)
		{
			/* The C# compiler sometimes generates try-finally structures for fixed statements.
				After our transforms runs, these are redundant and can be removed.
				.try BlockContainer {
					Block IL_001a (incoming: 1) {
						PinnedRegion ...
					}
				} finally BlockContainer {
					Block IL_003e (incoming: 1) {
						leave IL_003e (nop)
					}
				}
				==> PinnedRegion
			*/
			if (!(tryFinally.FinallyBlock is BlockContainer finallyContainer))
				return;
			if (!finallyContainer.SingleInstruction().MatchLeave(finallyContainer))
				return;
			// Finally is empty and redundant. But we'll delete the block only if there's a PinnedRegion.
			if (!(tryFinally.TryBlock is BlockContainer tryContainer))
				return;
			if (tryContainer.Blocks.Count != 1)
				return;
			var tryBlock = tryContainer.Blocks[0];
			if (tryBlock.Instructions.Count == 1)
			{
				if (tryBlock.Instructions[0] is PinnedRegion pinnedRegion)
				{
					context.Step("Removing try-finally around PinnedRegion", pinnedRegion);
					tryFinally.ReplaceWith(pinnedRegion);
				}
			}
			else if (tryBlock.Instructions.Count == 2)
			{
				if (tryBlock.Instructions[0] is PinnedRegion pinnedRegion &&
					tryBlock.Instructions[1].MatchLeave(tryContainer))
				{
					context.Step("Removing try-finally around PinnedRegion", pinnedRegion);
					tryFinally.ReplaceWith(pinnedRegion);
				}
			}
		}
	}
}
