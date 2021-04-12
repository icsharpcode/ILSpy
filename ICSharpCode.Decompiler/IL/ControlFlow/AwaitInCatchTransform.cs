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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	class AwaitInCatchTransform
	{
		readonly struct CatchBlockInfo
		{
			public readonly int Id;
			public readonly TryCatchHandler Handler;
			public readonly Block RealCatchBlockEntryPoint;
			public readonly ILInstruction NextBlockOrExitContainer;
			public readonly ILInstruction JumpTableEntry;
			public readonly ILVariable ObjectVariable;

			public CatchBlockInfo(int id, TryCatchHandler handler, Block realCatchBlockEntryPoint,
				ILInstruction nextBlockOrExitContainer, ILInstruction jumpTableEntry, ILVariable objectVariable)
			{
				Id = id;
				Handler = handler;
				RealCatchBlockEntryPoint = realCatchBlockEntryPoint;
				NextBlockOrExitContainer = nextBlockOrExitContainer;
				JumpTableEntry = jumpTableEntry;
				ObjectVariable = objectVariable;
			}
		}

		public static void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AwaitInCatchFinally)
				return;
			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();
			HashSet<Block> removedBlocks = new HashSet<Block>();

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
				SwitchInstruction switchInstructionOpt = null;
				foreach (var result in transformableCatchBlocks)
				{
					removedBlocks.Clear();
					var node = cfg.GetNode(result.RealCatchBlockEntryPoint);

					context.StepStartGroup($"Inline catch block with await (at {result.Handler.Variable.Name})", result.Handler);

					// Remove the IfInstruction from the jump table and eliminate all branches to the block.
					switch (result.JumpTableEntry)
					{
						case IfInstruction jumpTableEntry:
							var jumpTableBlock = (Block)jumpTableEntry.Parent;
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
							break;
						case SwitchSection jumpTableEntry:
							Debug.Assert(switchInstructionOpt == null || jumpTableEntry.Parent == switchInstructionOpt);
							switchInstructionOpt = (SwitchInstruction)jumpTableEntry.Parent;
							break;
					}

					// Add the real catch block entry-point to the block container
					var catchBlockHead = ((BlockContainer)result.Handler.Body).Blocks.Last();

					result.RealCatchBlockEntryPoint.Remove();
					((BlockContainer)result.Handler.Body).Blocks.Insert(0, result.RealCatchBlockEntryPoint);

					// Remove the generated catch block
					catchBlockHead.Remove();

					TransformAsyncThrowToThrow(context, removedBlocks, result.RealCatchBlockEntryPoint);

					// Inline all blocks that are dominated by the entrypoint of the real catch block
					foreach (var n in cfg.cfg)
					{
						Block block = (Block)n.UserData;

						if (node.Dominates(n))
						{
							TransformAsyncThrowToThrow(context, removedBlocks, block);

							if (block.Parent == result.Handler.Body)
								continue;

							if (!removedBlocks.Contains(block))
							{
								context.Step("Move block", result.Handler.Body);
								MoveBlock(block, (BlockContainer)result.Handler.Body);
							}
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
					if (result.ObjectVariable != result.Handler.Variable)
					{
						foreach (var load in result.ObjectVariable.LoadInstructions.ToArray())
						{
							if (!load.IsDescendantOf(result.Handler))
								continue;

							if (load.Parent is CastClass cc && cc.Type.Equals(result.Handler.Variable.Type))
							{
								cc.ReplaceWith(new LdLoc(result.Handler.Variable).WithILRange(cc).WithILRange(load));
							}
							else
							{
								load.ReplaceWith(new LdLoc(result.Handler.Variable).WithILRange(load));
							}
						}
					}

					context.StepEndGroup(keepIfEmpty: true);
				}

				if (switchInstructionOpt != null && switchInstructionOpt.Parent is Block b && b.IncomingEdgeCount > 0)
				{
					var defaultSection = switchInstructionOpt.GetDefaultSection();

					foreach (var branch in container.Descendants.OfType<Branch>())
					{
						if (branch.TargetBlock != b)
							continue;
						branch.ReplaceWith(defaultSection.Body.Clone());
					}
				}
			}

			// clean up all modified containers
			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		private static void TransformAsyncThrowToThrow(ILTransformContext context, HashSet<Block> removedBlocks, Block block)
		{
			ILVariable v = null;
			if (MatchExceptionCaptureBlock(context, block,
				ref v, out StLoc typedExceptionVariableStore,
				out Block captureBlock, out Block throwBlock))
			{
				context.Step($"ExceptionDispatchInfo.Capture({v.Name}).Throw() => throw;", typedExceptionVariableStore);
				block.Instructions.RemoveRange(typedExceptionVariableStore.ChildIndex + 1, 2);
				captureBlock.Remove();
				throwBlock.Remove();
				removedBlocks.Add(captureBlock);
				removedBlocks.Add(throwBlock);
				typedExceptionVariableStore.ReplaceWith(new Rethrow());
			}
		}

		static void MoveBlock(Block block, BlockContainer target)
		{
			block.Remove();
			target.Blocks.Add(block);
		}

		/// <summary>
		/// Analyzes all catch handlers and returns every handler that follows the await catch handler pattern.
		/// </summary>
		static bool AnalyzeHandlers(InstructionCollection<TryCatchHandler> handlers, out ILVariable catchHandlerIdentifier,
			out List<CatchBlockInfo> transformableCatchBlocks)
		{
			transformableCatchBlocks = new List<CatchBlockInfo>();
			catchHandlerIdentifier = null;
			foreach (var handler in handlers)
			{
				if (!MatchAwaitCatchHandler(handler, out int id, out var identifierVariable,
					out var realEntryPoint, out var nextBlockOrExitContainer, out var jumpTableEntry,
					out var objectVariable))
				{
					continue;
				}

				if (id < 1 || (catchHandlerIdentifier != null && identifierVariable != catchHandlerIdentifier))
				{
					continue;
				}

				catchHandlerIdentifier = identifierVariable;
				transformableCatchBlocks.Add(new(id, handler, realEntryPoint, nextBlockOrExitContainer, jumpTableEntry, objectVariable ?? handler.Variable));
			}
			return transformableCatchBlocks.Count > 0;
		}

		/// <summary>
		/// Matches the await catch handler pattern:
		/// [stloc V_3(ldloc E_100)	- copy exception variable to a temporary]
		/// stloc V_6(ldloc V_3)	- store exception in 'global' object variable
		/// stloc V_5(ldc.i4 2)		- store id of catch block in 'identifierVariable'
		/// br IL_0075				- jump out of catch block to the head of the catch-handler jump table
		/// </summary>
		static bool MatchAwaitCatchHandler(TryCatchHandler handler, out int id, out ILVariable identifierVariable,
			out Block realEntryPoint, out ILInstruction nextBlockOrExitContainer,
			out ILInstruction jumpTableEntry, out ILVariable objectVariable)
		{
			id = 0;
			identifierVariable = null;
			realEntryPoint = null;
			jumpTableEntry = null;
			objectVariable = null;
			nextBlockOrExitContainer = null;
			var exceptionVariable = handler.Variable;
			var catchBlock = ((BlockContainer)handler.Body).EntryPoint;
			ILInstruction value;
			switch (catchBlock.Instructions.Count)
			{
				case 3:
					if (!catchBlock.Instructions[0].MatchStLoc(out objectVariable, out value))
						return false;
					if (!value.MatchLdLoc(exceptionVariable))
						return false;
					break;
				case 4:
					if (!catchBlock.Instructions[0].MatchStLoc(out var temporaryVariable, out value))
						return false;
					if (!value.MatchLdLoc(exceptionVariable))
						return false;
					if (!catchBlock.Instructions[1].MatchStLoc(out objectVariable, out value))
						return false;
					if (!value.MatchLdLoc(temporaryVariable))
						return false;
					break;
				default:
					// if the exception variable is not used at all (e.g., catch (Exception))
					// the "exception-variable-assignment" is omitted completely.
					// This can happen in optimized code.
					break;
			}
			if (!catchBlock.Instructions.Last().MatchBranch(out var jumpTableStartBlock))
				return false;
			var identifierVariableAssignment = catchBlock.Instructions.SecondToLastOrDefault();
			if (identifierVariableAssignment == null)
				return false;
			if (!identifierVariableAssignment.MatchStLoc(out identifierVariable, out value) || !value.MatchLdcI4(out id))
				return false;
			// analyze jump table:
			switch (jumpTableStartBlock.Instructions.Count)
			{
				case 3:
					// stloc identifierVariableCopy(identifierVariable)
					// if (comp(identifierVariable == id)) br realEntryPoint
					// br jumpTableEntryBlock
					if (!jumpTableStartBlock.Instructions[0].MatchStLoc(out var identifierVariableCopy, out var identifierVariableLoad)
						|| !identifierVariableLoad.MatchLdLoc(identifierVariable))
					{
						return false;
					}
					return ParseIfJumpTable(id, jumpTableStartBlock, identifierVariableCopy, out realEntryPoint, out nextBlockOrExitContainer, out jumpTableEntry);
				case 2:
					// if (comp(identifierVariable == id)) br realEntryPoint
					// br jumpTableEntryBlock
					return ParseIfJumpTable(id, jumpTableStartBlock, identifierVariable, out realEntryPoint, out nextBlockOrExitContainer, out jumpTableEntry);
				case 1:
					if (jumpTableStartBlock.Instructions[0] is not SwitchInstruction switchInst)
					{
						return false;
					}

					return ParseSwitchJumpTable(id, switchInst, identifierVariable, out realEntryPoint, out nextBlockOrExitContainer, out jumpTableEntry);
				default:
					return false;
			}

			bool ParseSwitchJumpTable(int id, SwitchInstruction jumpTable, ILVariable identifierVariable, out Block realEntryPoint, out ILInstruction nextBlockOrExitContainer, out ILInstruction jumpTableEntry)
			{
				realEntryPoint = null;
				nextBlockOrExitContainer = null;
				jumpTableEntry = null;

				if (!jumpTable.Value.MatchLdLoc(identifierVariable))
					return false;

				var defaultSection = jumpTable.GetDefaultSection();

				foreach (var section in jumpTable.Sections)
				{
					if (!section.Labels.Contains(id))
						continue;
					if (!section.Body.MatchBranch(out realEntryPoint))
						return false;
					if (defaultSection.Body.MatchBranch(out var t))
						nextBlockOrExitContainer = t;
					else if (defaultSection.Body.MatchLeave(out var t2))
						nextBlockOrExitContainer = t2;
					jumpTableEntry = section;
					return true;
				}

				return false;
			}

			bool ParseIfJumpTable(int id, Block jumpTableEntryBlock, ILVariable identifierVariable, out Block realEntryPoint, out ILInstruction nextBlockOrExitContainer, out ILInstruction jumpTableEntry)
			{
				realEntryPoint = null;
				nextBlockOrExitContainer = null;
				jumpTableEntry = null;
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
					if (!left.MatchLdLoc(identifierVariable))
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

		// Block beforeThrowBlock {
		// 	[before throw]
		// 	stloc typedExceptionVariable(isinst System.Exception(ldloc objectVariable))
		// 	if (comp.o(ldloc typedExceptionVariable != ldnull)) br captureBlock
		// 	br throwBlock
		// }
		// 
		// Block throwBlock {
		// 	throw(ldloc objectVariable)
		// }
		// 
		// Block captureBlock {
		// 	callvirt Throw(call Capture(ldloc typedExceptionVariable))
		// 	br nextBlock
		// }
		// =>
		// throw(ldloc result.Handler.Variable)
		internal static bool MatchExceptionCaptureBlock(ILTransformContext context, Block block,
			ref ILVariable objectVariable, out StLoc typedExceptionVariableStore, out Block captureBlock, out Block throwBlock)
		{
			bool DerivesFromException(IType t) => t.GetAllBaseTypes().Any(ty => ty.IsKnownType(KnownTypeCode.Exception));

			captureBlock = null;
			throwBlock = null;
			typedExceptionVariableStore = null;

			var typedExceptionVariableStLoc = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;

			if (typedExceptionVariableStLoc == null
				|| !typedExceptionVariableStLoc.Value.MatchIsInst(out var arg, out var type)
				|| !DerivesFromException(type)
				|| !arg.MatchLdLoc(out var v))
			{
				return false;
			}

			if (objectVariable == null)
			{
				objectVariable = v;
			}
			else if (!objectVariable.Equals(v))
			{
				return false;
			}

			typedExceptionVariableStore = typedExceptionVariableStLoc;

			if (!block.Instructions[block.Instructions.Count - 2].MatchIfInstruction(out var condition, out var trueInst))
				return false;

			ILInstruction lastInstr = block.Instructions.Last();
			if (!lastInstr.MatchBranch(out throwBlock))
				return false;

			if (condition.MatchCompNotEqualsNull(out arg)
				&& trueInst is Branch branchToCapture)
			{
				if (!arg.MatchLdLoc(typedExceptionVariableStore.Variable))
					return false;
				captureBlock = branchToCapture.TargetBlock;
			}
			else
			{
				return false;
			}

			if (throwBlock.IncomingEdgeCount != 1
				|| throwBlock.Instructions.Count != 1
				|| !(throwBlock.Instructions[0].MatchThrow(out var ov) && ov.MatchLdLoc(objectVariable)))
			{
				return false;
			}

			if (captureBlock.IncomingEdgeCount != 1
				|| captureBlock.Instructions.Count != 2
				|| !MatchCaptureThrowCalls(captureBlock.Instructions[0]))
			{
				return false;
			}

			return true;

			bool MatchCaptureThrowCalls(ILInstruction inst)
			{
				var exceptionDispatchInfoType = context.TypeSystem.FindType(typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo));
				if (inst is not CallVirt callVirt || callVirt.Arguments.Count != 1)
					return false;

				if (callVirt.Arguments[0] is not Call call || call.Arguments.Count != 1
					|| !call.Arguments[0].MatchLdLoc(typedExceptionVariableStLoc.Variable))
				{
					return false;
				}

				return callVirt.Method.Name == "Throw"
					&& callVirt.Method.DeclaringType.Equals(exceptionDispatchInfoType)
					&& call.Method.Name == "Capture"
					&& call.Method.DeclaringType.Equals(exceptionDispatchInfoType);
			}
		}
	}
}
