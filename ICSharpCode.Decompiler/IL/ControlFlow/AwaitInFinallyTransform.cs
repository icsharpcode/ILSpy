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
using System.Text;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

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
			foreach (var tryCatch in function.Descendants.OfType<TryCatch>().ToArray()) {
				if (!(tryCatch.Parent?.Parent is BlockContainer container))
					continue;
				// Detect all handlers that contain an await expression
				AnalyzeHandlers(tryCatch.Handlers, out var catchHandlerIdentifier, out var transformableCatchBlocks);
				var cfg = new ControlFlowGraph(container, context.CancellationToken);
				if (transformableCatchBlocks.Count > 0)
					changedContainers.Add(container);
				foreach (var result in transformableCatchBlocks) {
					var node = cfg.GetNode(result.RealCatchBlockEntryPoint);

					context.Step("Inline catch block with await", result.Handler);

					// Remove the IfInstruction from the jump table and eliminate all branches to the block.
					var jumpTableBlock = (Block)result.JumpTableEntry.Parent;
					jumpTableBlock.Instructions.RemoveAt(result.JumpTableEntry.ChildIndex);

					foreach (var branch in tryCatch.Descendants.OfType<Branch>()) {
						if (branch.TargetBlock == jumpTableBlock) {
							if (result.NextBlockOrExitContainer is BlockContainer exitContainer) {
								branch.ReplaceWith(new Leave(exitContainer));
							} else {
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
					foreach (var n in cfg.cfg) {
						if (((Block)n.UserData).Parent == result.Handler.Body)
							continue;
						if (node.Dominates(n)) {
							MoveBlock((Block)n.UserData, (BlockContainer)result.Handler.Body);
						}
					}

					// Remove all assignments to the common object variable that stores the exception object.
					if (result.ObjectVariableStore != null) {
						foreach (var load in result.ObjectVariableStore.Variable.LoadInstructions.ToArray()) {
							if (load.Parent is CastClass cc && cc.Type == result.Handler.Variable.Type)
								cc.ReplaceWith(new LdLoc(result.Handler.Variable));
							else
								load.ReplaceWith(new LdLoc(result.Handler.Variable));
						}
					}
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
			foreach (var handler in handlers) {
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
			if (catchBlock.Instructions.Count > 2 && catchBlock.Instructions[catchBlock.Instructions.Count - 3] is StLoc stloc) {
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
			if (jumpTableStartBlock.Instructions.Count == 3) {
				if (!jumpTableStartBlock.Instructions[0].MatchStLoc(out identifierVariableCopy, out var identifierVariableLoad) || !identifierVariableLoad.MatchLdLoc(identifierVariable))
					return false;
			} else if (jumpTableStartBlock.Instructions.Count == 2) {
				identifierVariableCopy = identifierVariable;
			} else return false;
			var jumpTableEntryBlock = jumpTableStartBlock;
			do {
				if (!(jumpTableEntryBlock.Instructions.SecondToLastOrDefault() is IfInstruction ifInst))
					return false;
				ILInstruction lastInst = jumpTableEntryBlock.Instructions.Last();
				if (ifInst.Condition.MatchCompEquals(out var left, out var right)) {
					if (!ifInst.TrueInst.MatchBranch(out realEntryPoint))
						return false;
					if (!lastInst.MatchBranch(out jumpTableEntryBlock)) {
						if (!lastInst.MatchLeave((BlockContainer)lastInst.Parent.Parent))
							return false;
					}
				} else if (ifInst.Condition.MatchCompNotEquals(out left, out right)) {
					if (!lastInst.MatchBranch(out realEntryPoint))
						return false;
					if (!ifInst.TrueInst.MatchBranch(out jumpTableEntryBlock)) {
						if (!ifInst.TrueInst.MatchLeave((BlockContainer)lastInst.Parent.Parent))
							return false;
					}
				} else {
					return false;
				}
				if (!left.MatchLdLoc(identifierVariableCopy))
					return false;
				if (right.MatchLdcI4(id)) {
					nextBlockOrExitContainer = jumpTableEntryBlock ?? lastInst.Parent.Parent;
					jumpTableEntry = ifInst;
					return true;
				}
			} while (jumpTableEntryBlock?.Instructions.Count == 2);
			return false;
		}
	}

	class AwaitInFinallyTransform
	{
		public static void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AwaitInCatchFinally)
				return;
			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			// analyze all try-catch statements in the function
			foreach (var tryCatch in function.Descendants.OfType<TryCatch>().ToArray()) {
				if (!(tryCatch.Parent?.Parent is BlockContainer container))
					continue;
				// await in finally uses a single catch block with catch-type object
				if (tryCatch.Handlers.Count != 1 || !(tryCatch.Handlers[0].Body is BlockContainer catchBlockContainer) || !tryCatch.Handlers[0].Variable.Type.IsKnownType(KnownTypeCode.Object))
					continue;
				// and consists of an assignment to a temporary that is used outside the catch block
				// and a jump to the finally block
				var block = catchBlockContainer.EntryPoint;
				if (block.Instructions.Count < 2 || !block.Instructions[0].MatchStLoc(out var globalCopyVar, out var value) || !value.MatchLdLoc(tryCatch.Handlers[0].Variable))
					continue;
				if (block.Instructions.Count == 3) {
					if (!block.Instructions[1].MatchStLoc(out var globalCopyVarTemp, out value) || !value.MatchLdLoc(globalCopyVar))
						continue;
					globalCopyVar = globalCopyVarTemp;
				}
				if (!block.Instructions[block.Instructions.Count - 1].MatchBranch(out var entryPointOfFinally))
					continue;
				// globalCopyVar should only be used once, at the end of the finally-block
				if (globalCopyVar.LoadCount != 1 || globalCopyVar.StoreCount > 2)
					continue;
				var tempStore = globalCopyVar.LoadInstructions[0].Parent as StLoc;
				if (tempStore == null || !MatchExceptionCaptureBlock(tempStore, out var exitOfFinally, out var afterFinally, out var blocksToRemove))
					continue;
				if (!MatchAfterFinallyBlock(ref afterFinally, blocksToRemove, out bool removeFirstInstructionInAfterFinally))
					continue;
				var cfg = new ControlFlowGraph(container, context.CancellationToken);
				var exitOfFinallyNode = cfg.GetNode(exitOfFinally);
				var entryPointOfFinallyNode = cfg.GetNode(entryPointOfFinally);

				var additionalBlocksInFinally = new HashSet<Block>();
				var invalidExits = new List<ControlFlowNode>();

				TraverseDominatorTree(entryPointOfFinallyNode);

				void TraverseDominatorTree(ControlFlowNode node)
				{
					if (entryPointOfFinallyNode != node) {
						if (entryPointOfFinallyNode.Dominates(node))
							additionalBlocksInFinally.Add((Block)node.UserData);
						else
							invalidExits.Add(node);
					}

					if (node == exitOfFinallyNode)
						return;

					foreach (var child in node.DominatorTreeChildren) {
						TraverseDominatorTree(child);
					}
				}

				if (invalidExits.Any())
					continue;

				context.Step("Inline finally block with await", tryCatch.Handlers[0]);

				foreach (var blockToRemove in blocksToRemove) {
					blockToRemove.Remove();
				}
				var finallyContainer = new BlockContainer();
				entryPointOfFinally.Remove();
				if (removeFirstInstructionInAfterFinally)
					afterFinally.Instructions.RemoveAt(0);
				changedContainers.Add(container);
				var outer = BlockContainer.FindClosestContainer(container.Parent);
				if (outer != null) changedContainers.Add(outer);
				finallyContainer.Blocks.Add(entryPointOfFinally);
				finallyContainer.AddILRange(entryPointOfFinally);
				exitOfFinally.Instructions.RemoveRange(tempStore.ChildIndex, 3);
				exitOfFinally.Instructions.Add(new Leave(finallyContainer));
				foreach (var branchToFinally in container.Descendants.OfType<Branch>()) {
					if (branchToFinally.TargetBlock == entryPointOfFinally)
						branchToFinally.ReplaceWith(new Branch(afterFinally));
				}
				foreach (var newBlock in additionalBlocksInFinally) {
					newBlock.Remove();
					finallyContainer.Blocks.Add(newBlock);
					finallyContainer.AddILRange(newBlock);
				}
				tryCatch.ReplaceWith(new TryFinally(tryCatch.TryBlock, finallyContainer).WithILRange(tryCatch.TryBlock));
			}

			// clean up all modified containers
			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		/// <summary>
		/// Block finallyHead (incoming: 2) {
		///		[body of finally]
		/// 	stloc V_4(ldloc V_1)
		/// 	if (comp(ldloc V_4 == ldnull)) br afterFinally
		/// 	br typeCheckBlock
		/// }
		/// 
		/// Block typeCheckBlock (incoming: 1) {
		/// 	stloc S_110(isinst System.Exception(ldloc V_4))
		/// 	if (comp(ldloc S_110 != ldnull)) br captureBlock
		/// 	br throwBlock
		/// }
		/// 
		/// Block throwBlock (incoming: 1) {
		/// 	throw(ldloc V_4)
		/// }
		/// 
		/// Block captureBlock (incoming: 1) {
		/// 	callvirt Throw(call Capture(ldloc S_110))
		/// 	br afterFinally
		/// }
		/// 
		/// Block afterFinally (incoming: 2) {
		/// 	stloc V_1(ldnull)
		/// 	[after finally]
		/// }
		/// </summary>
		static bool MatchExceptionCaptureBlock(StLoc tempStore, out Block endOfFinally, out Block afterFinally, out List<Block> blocksToRemove)
		{
			afterFinally = null;
			endOfFinally = (Block)tempStore.Parent;
			blocksToRemove = new List<Block>();
			int count = endOfFinally.Instructions.Count;
			if (tempStore.ChildIndex != count - 3)
				return false;
			if (!(endOfFinally.Instructions[count - 2] is IfInstruction ifInst))
				return false;
			if (!endOfFinally.Instructions.Last().MatchBranch(out var typeCheckBlock))
				return false;
			if (!ifInst.TrueInst.MatchBranch(out afterFinally))
				return false;
			// match typeCheckBlock
			if (typeCheckBlock.Instructions.Count != 3)
				return false;
			if (!typeCheckBlock.Instructions[0].MatchStLoc(out var castStore, out var cast)
				|| !cast.MatchIsInst(out var arg, out var type) || !type.IsKnownType(KnownTypeCode.Exception) || !arg.MatchLdLoc(tempStore.Variable))
				return false;
			if (!typeCheckBlock.Instructions[1].MatchIfInstruction(out var cond, out var jumpToCaptureBlock))
				return false;
			if (!cond.MatchCompNotEqualsNull(out arg) || !arg.MatchLdLoc(castStore))
				return false;
			if (!typeCheckBlock.Instructions[2].MatchBranch(out var throwBlock))
				return false;
			if (!jumpToCaptureBlock.MatchBranch(out var captureBlock))
				return false;
			// match throwBlock
			if (throwBlock.Instructions.Count != 1 || !throwBlock.Instructions[0].MatchThrow(out arg) || !arg.MatchLdLoc(tempStore.Variable))
				return false;
			// match captureBlock
			if (captureBlock.Instructions.Count != 2)
				return false;
			if (!captureBlock.Instructions[1].MatchBranch(afterFinally))
				return false;
			if (!(captureBlock.Instructions[0] is CallVirt callVirt) || callVirt.Method.FullName != "System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw" || callVirt.Arguments.Count != 1)
				return false;
			if (!(callVirt.Arguments[0] is Call call) || call.Method.FullName != "System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture" || call.Arguments.Count != 1)
				return false;
			if (!call.Arguments[0].MatchLdLoc(castStore))
				return false;
			blocksToRemove.Add(typeCheckBlock);
			blocksToRemove.Add(throwBlock);
			blocksToRemove.Add(captureBlock);
			return true;
		}

		static bool MatchAfterFinallyBlock(ref Block afterFinally, List<Block> blocksToRemove, out bool removeFirstInstructionInAfterFinally)
		{
			removeFirstInstructionInAfterFinally = false;
			if (afterFinally.Instructions.Count < 2)
				return false;
			ILVariable globalCopyVarSplitted;
			switch (afterFinally.Instructions[0]) {
				case IfInstruction ifInst:
					if (ifInst.Condition.MatchCompEquals(out var load, out var ldone) && ldone.MatchLdcI4(1) && load.MatchLdLoc(out var variable)) {
						if (!ifInst.TrueInst.MatchBranch(out var targetBlock))
							return false;
						blocksToRemove.Add(afterFinally);
						afterFinally = targetBlock;
						return true;
					} else if (ifInst.Condition.MatchCompNotEquals(out load, out ldone) && ldone.MatchLdcI4(1) && load.MatchLdLoc(out variable)) {
						if (!afterFinally.Instructions[1].MatchBranch(out var targetBlock))
							return false;
						blocksToRemove.Add(afterFinally);
						afterFinally = targetBlock;
						return true;
					}
					return false;
				case LdLoc ldLoc:
					if (ldLoc.Variable.LoadCount != 1 || ldLoc.Variable.StoreCount != 1)
						return false;
					if (!afterFinally.Instructions[1].MatchStLoc(out globalCopyVarSplitted, out var ldnull) || !ldnull.MatchLdNull())
						return false;
					removeFirstInstructionInAfterFinally = true;
					break;
				case StLoc stloc:
					globalCopyVarSplitted = stloc.Variable;
					if (!stloc.Value.MatchLdNull())
						return false;
					break;
				default:
					return false;
			}
			if (globalCopyVarSplitted.StoreCount != 1 || globalCopyVarSplitted.LoadCount != 0)
				return false;
			return true;
		}
	}
}
