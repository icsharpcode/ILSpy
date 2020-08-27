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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	class AwaitInCatchTransform
	{
		public static void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AwaitInCatchFinally)
				return;
			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			// analyze all try-catch statements in the function
			foreach (var tryCatch in function.Descendants.OfType<TryCatch>().ToArray())
			{
				if (!(tryCatch.Parent?.Parent is BlockContainer container))
					continue;
				// Detect all handlers that contain an await expression
				AnalyzeHandlers(tryCatch.Handlers, out var catchHandlerIdentifier, out var transformableCatchBlocks);
				var cfg = new ControlFlowGraph(container, context.CancellationToken);
				if (transformableCatchBlocks.Count > 0)
					changedContainers.Add(container);
				foreach (var result in transformableCatchBlocks)
				{
					var node = cfg.GetNode(result.RealCatchBlockEntryPoint);

					context.StepStartGroup("Inline catch block with await", result.Handler);

					// Remove the IfInstruction from the jump table and eliminate all branches to the block.
					var jumpTableBlock = (Block)result.JumpTableEntry.Parent;
					context.Step("Remove jump-table entry", result.JumpTableEntry);
					jumpTableBlock.Instructions.RemoveAt(result.JumpTableEntry.ChildIndex);

					foreach (var branch in tryCatch.Descendants.OfType<Branch>())
					{
						if (branch.TargetBlock == jumpTableBlock)
						{
							if (result.NextBlockOrExitContainer is BlockContainer exitContainer)
							{
								context.Step("branch jumpTableBlock => leave exitContainer", branch);
								branch.ReplaceWith(new Leave(exitContainer));
							}
							else
							{
								context.Step("branch jumpTableBlock => branch nextBlock", branch);
								branch.ReplaceWith(new Branch((Block)result.NextBlockOrExitContainer));
							}
						}
					}

					// Add the real catch block entry-point to the block container
					var catchBlockHead = ((BlockContainer)result.Handler.Body).Blocks.Last();

					result.RealCatchBlockEntryPoint.Remove();
					((BlockContainer)result.Handler.Body).Blocks.Insert(0, result.RealCatchBlockEntryPoint);

					// Remove the generated catch block
					catchBlockHead.Remove();

					// Inline all blocks that are dominated by the entrypoint of the real catch block
					foreach (var n in cfg.cfg)
					{
						if (((Block)n.UserData).Parent == result.Handler.Body)
							continue;
						if (node.Dominates(n))
						{
							MoveBlock((Block)n.UserData, (BlockContainer)result.Handler.Body);
						}
					}

					// Remove unreachable pattern blocks
					// TODO : sanity check
					if (result.NextBlockOrExitContainer is Block nextBlock && nextBlock.IncomingEdgeCount == 0)
					{
						List<Block> dependentBlocks = new List<Block>();
						Block current = nextBlock;

						do
						{
							foreach (var branch in current.Descendants.OfType<Branch>())
							{
								dependentBlocks.Add(branch.TargetBlock);
							}

							current.Remove();
							dependentBlocks.Remove(current);
							current = dependentBlocks.FirstOrDefault(b => b.IncomingEdgeCount == 0);
						} while (current != null);
					}

					// Remove all assignments to the common object variable that stores the exception object.
					if (result.ObjectVariableStore != null)
					{
						foreach (var load in result.ObjectVariableStore.Variable.LoadInstructions.ToArray())
						{
							if (load.Parent is CastClass cc && cc.Type == result.Handler.Variable.Type)
								cc.ReplaceWith(new LdLoc(result.Handler.Variable));
							else
								load.ReplaceWith(new LdLoc(result.Handler.Variable));
						}
					}

					context.StepEndGroup(keepIfEmpty: true);
				}
			}

			// clean up all modified containers
			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		static void MoveBlock(Block block, BlockContainer target)
		{
			block.Remove();
			target.Blocks.Add(block);
		}

		/// <summary>
		/// Analyzes all catch handlers and returns every handler that follows the await catch handler pattern.
		/// </summary>
		static bool AnalyzeHandlers(InstructionCollection<TryCatchHandler> handlers, out ILVariable catchHandlerIdentifier, out List<(int Id, TryCatchHandler Handler, Block RealCatchBlockEntryPoint, ILInstruction NextBlockOrExitContainer, IfInstruction JumpTableEntry, StLoc ObjectVariableStore)> transformableCatchBlocks)
		{
			transformableCatchBlocks = new List<(int Id, TryCatchHandler Handler, Block RealCatchBlockEntryPoint, ILInstruction NextBlockOrExitContainer, IfInstruction JumpTableEntry, StLoc ObjectVariableStore)>();
			catchHandlerIdentifier = null;
			foreach (var handler in handlers)
			{
				if (!MatchAwaitCatchHandler((BlockContainer)handler.Body, out int id, out var identifierVariable, out var realEntryPoint, out var nextBlockOrExitContainer, out var jumpTableEntry, out var objectVariableStore))
					continue;
				if (id < 1 || (catchHandlerIdentifier != null && identifierVariable != catchHandlerIdentifier))
					continue;
				catchHandlerIdentifier = identifierVariable;
				transformableCatchBlocks.Add((id, handler, realEntryPoint, nextBlockOrExitContainer, jumpTableEntry, objectVariableStore));
			}
			return transformableCatchBlocks.Count > 0;
		}

		/// <summary>
		/// Matches the await catch handler pattern:
		/// stloc V_3(ldloc E_100)	- copy exception variable to a temporary
		/// stloc V_6(ldloc V_3)	- store exception in 'global' object variable
		/// stloc V_5(ldc.i4 2)		- store id of catch block in 'identifierVariable'
		/// br IL_0075				- jump out of catch block to the head of the catch-handler jump table
		/// </summary>
		static bool MatchAwaitCatchHandler(BlockContainer container, out int id, out ILVariable identifierVariable, out Block realEntryPoint, out ILInstruction nextBlockOrExitContainer, out IfInstruction jumpTableEntry, out StLoc objectVariableStore)
		{
			id = default(int);
			identifierVariable = null;
			realEntryPoint = null;
			jumpTableEntry = null;
			objectVariableStore = null;
			nextBlockOrExitContainer = null;
			var catchBlock = container.EntryPoint;
			if (catchBlock.Instructions.Count < 2 || catchBlock.Instructions.Count > 4)
				return false;
			if (!catchBlock.Instructions.Last().MatchBranch(out var jumpTableStartBlock))
				return false;
			if (catchBlock.Instructions.Count > 2 && catchBlock.Instructions[catchBlock.Instructions.Count - 3] is StLoc stloc)
			{
				objectVariableStore = stloc;
			}
			var identifierVariableAssignment = catchBlock.Instructions.SecondToLastOrDefault();
			if (!identifierVariableAssignment.MatchStLoc(out identifierVariable, out var value) || !value.MatchLdcI4(out id))
				return false;
			// analyze jump table:
			// [stloc identifierVariableCopy(identifierVariable)]
			// if (comp(identifierVariable == id)) br realEntryPoint
			// br jumpTableEntryBlock
			ILVariable identifierVariableCopy;
			if (jumpTableStartBlock.Instructions.Count == 3)
			{
				if (!jumpTableStartBlock.Instructions[0].MatchStLoc(out identifierVariableCopy, out var identifierVariableLoad) || !identifierVariableLoad.MatchLdLoc(identifierVariable))
					return false;
			}
			else if (jumpTableStartBlock.Instructions.Count == 2)
			{
				identifierVariableCopy = identifierVariable;
			}
			else
				return false;
			var jumpTableEntryBlock = jumpTableStartBlock;
			do
			{
				if (!(jumpTableEntryBlock.Instructions.SecondToLastOrDefault() is IfInstruction ifInst))
					return false;
				ILInstruction lastInst = jumpTableEntryBlock.Instructions.Last();
				if (ifInst.Condition.MatchCompEquals(out var left, out var right))
				{
					if (!ifInst.TrueInst.MatchBranch(out realEntryPoint))
						return false;
					if (!lastInst.MatchBranch(out jumpTableEntryBlock))
					{
						if (!lastInst.MatchLeave((BlockContainer)lastInst.Parent.Parent))
							return false;
					}
				}
				else if (ifInst.Condition.MatchCompNotEquals(out left, out right))
				{
					if (!lastInst.MatchBranch(out realEntryPoint))
						return false;
					if (!ifInst.TrueInst.MatchBranch(out jumpTableEntryBlock))
					{
						if (!ifInst.TrueInst.MatchLeave((BlockContainer)lastInst.Parent.Parent))
							return false;
					}
				}
				else
				{
					return false;
				}
				if (!left.MatchLdLoc(identifierVariableCopy))
					return false;
				if (right.MatchLdcI4(id))
				{
					nextBlockOrExitContainer = jumpTableEntryBlock ?? lastInst.Parent.Parent;
					jumpTableEntry = ifInst;
					return true;
				}
			} while (jumpTableEntryBlock?.Instructions.Count == 2);
			return false;
		}
	}
}
