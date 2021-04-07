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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	class AwaitInFinallyTransform
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
				// 	} catch exceptionVariable : 02000078 System.Object when (ldc.i4 1) BlockContainer {
				// 		Block IL_004a (incoming: 1) {
				// 			stloc objectVariable(ldloc exceptionVariable)
				// 			br finallyBlock
				// 		}
				// 
				// 	}
				// }
				// 
				// Block finallyBlock (incoming: 2) {
				// 	if (comp.o(ldloc b == ldnull)) br afterFinallyBlock
				// 	br finallyBlockContinuation
				// }
				// 
				// Block finallyBlockContinuation (incoming: 1) {
				// 	await(addressof System.Threading.Tasks.ValueTask(callvirt DisposeAsync(ldloc b)))
				// 	br afterFinallyBlock
				// }
				// 
				// Block afterFinallyBlock (incoming: 2) {
				// 	stloc V_1(ldloc objectVariable)
				// 	if (comp.o(ldloc V_1 == ldnull)) br IL_00ea
				// 	br IL_00cf
				// }

				// await in finally uses a single catch block with catch-type object
				if (tryCatch.Handlers.Count != 1)
				{
					continue;
				}
				var handler = tryCatch.Handlers[0];
				var exceptionVariable = handler.Variable;
				if (handler.Body is not BlockContainer catchBlockContainer)
					continue;
				if (!exceptionVariable.Type.IsKnownType(KnownTypeCode.Object))
					continue;
				// Matches the await finally pattern:
				// [stloc V_3(ldloc E_100)	- copy exception variable to a temporary]
				// stloc V_6(ldloc V_3)	- store exception in 'global' object variable
				// br IL_0075				- jump out of catch block to the head of the finallyBlock
				var catchBlockEntry = catchBlockContainer.EntryPoint;
				ILVariable objectVariable;
				switch (catchBlockEntry.Instructions.Count)
				{
					case 2:
						if (!catchBlockEntry.Instructions[0].MatchStLoc(out objectVariable, out var value))
							continue;
						if (!value.MatchLdLoc(exceptionVariable))
							continue;
						break;
					case 3:
						if (!catchBlockEntry.Instructions[0].MatchStLoc(out var temporaryVariable, out value))
							continue;
						if (!value.MatchLdLoc(exceptionVariable))
							continue;
						if (!catchBlockEntry.Instructions[1].MatchStLoc(out objectVariable, out value))
							continue;
						if (!value.MatchLdLoc(temporaryVariable))
							continue;
						break;
					default:
						continue;
				}
				if (!catchBlockEntry.Instructions[catchBlockEntry.Instructions.Count - 1].MatchBranch(out var entryPointOfFinally))
					continue;
				// globalCopyVar should only be used once, at the end of the finally-block
				if (objectVariable.LoadCount != 1 || objectVariable.StoreCount > 2)
					continue;

				var beforeExceptionCaptureBlock = (Block)LocalFunctionDecompiler.GetStatement(objectVariable.LoadInstructions[0])?.Parent;
				if (beforeExceptionCaptureBlock == null)
					continue;

				var (afterFinallyBlock, capturePatternStart, objectVariableCopy) = FindBlockAfterFinally(context, beforeExceptionCaptureBlock, objectVariable);
				if (afterFinallyBlock == null || capturePatternStart == null)
					continue;

				var initOfIdentifierVariable = tryCatch.Parent.Children.ElementAtOrDefault(tryCatch.ChildIndex - 1) as StLoc;
				if (initOfIdentifierVariable == null || !initOfIdentifierVariable.Value.MatchLdcI4(0))
					continue;

				var identifierVariable = initOfIdentifierVariable.Variable;

				context.StepStartGroup("Inline finally block with await", tryCatch.Handlers[0]);
				var cfg = new ControlFlowGraph(container, context.CancellationToken);
				changedContainers.Add(container);

				context.StepStartGroup("Move blocks to state assignments");
				Dictionary<int, Block> identifierValueTargets = new Dictionary<int, Block>();

				foreach (var load in identifierVariable.LoadInstructions.ToArray())
				{
					var statement = LocalFunctionDecompiler.GetStatement(load);
					var block = (Block)statement.Parent;

					if (!statement.MatchIfInstruction(out var cond, out var branchToTarget))
					{
						block.Instructions.RemoveAt(statement.ChildIndex);
						continue;
					}

					if (block.Instructions.LastOrDefault() is not Branch otherBranch)
					{
						block.Instructions.RemoveAt(statement.ChildIndex);
						continue;
					}

					if (cond.MatchCompEquals(out var left, out var right)
						&& left == load && right.MatchLdcI4(out int value)
						&& branchToTarget.MatchBranch(out var targetBlock))
					{
						identifierValueTargets.Add(value, targetBlock);
						block.Instructions.RemoveAt(statement.ChildIndex);
					}
					else if (cond.MatchCompNotEquals(out left, out right)
						&& left == load && right.MatchLdcI4(out value))
					{
						identifierValueTargets.Add(value, otherBranch.TargetBlock);
						block.Instructions.RemoveAt(statement.ChildIndex + 1);
						statement.ReplaceWith(otherBranch);
					}
					else
					{
						block.Instructions.RemoveAt(statement.ChildIndex);
					}
				}

				var removedBlocks = new List<Block>();

				foreach (var store in identifierVariable.StoreInstructions.OfType<StLoc>().ToArray())
				{
					if (!store.Value.MatchLdcI4(out int value))
						continue;
					var statement = LocalFunctionDecompiler.GetStatement(store);
					var parent = (Block)statement.Parent;
					if (value <= 0)
					{
						parent.Instructions.RemoveAt(statement.ChildIndex);
						continue;
					}
					if (!identifierValueTargets.TryGetValue(value, out var targetBlock))
					{
						store.ReplaceWith(new Nop() { Comment = $"Could not find matching block for id {value}" });
						continue;
					}
					var targetContainer = BlockContainer.FindClosestContainer(statement);
					context.Step($"Move block with id={value} {targetBlock.Label} to IL_{store.StartILOffset}", statement);
					parent.Instructions.RemoveAt(statement.ChildIndex + 1);
					store.ReplaceWith(new Branch(targetBlock));

					MoveDominatedBlocksToContainer(targetBlock, null, cfg, targetContainer, removedBlocks);
				}


				context.StepEndGroup(keepIfEmpty: true);

				var finallyContainer = new BlockContainer().WithILRange(catchBlockContainer);
				tryCatch.ReplaceWith(new TryFinally(tryCatch.TryBlock, finallyContainer).WithILRange(tryCatch.TryBlock));

				context.Step("Move blocks into finally", finallyContainer);
				MoveDominatedBlocksToContainer(entryPointOfFinally, beforeExceptionCaptureBlock, cfg, finallyContainer, removedBlocks);

				if (beforeExceptionCaptureBlock.Instructions.Count >= 3)
				{
					if (beforeExceptionCaptureBlock.Instructions.SecondToLastOrDefault().MatchIfInstruction(out var cond, out var brInst)
						&& beforeExceptionCaptureBlock.Instructions.LastOrDefault() is Branch branch
						&& beforeExceptionCaptureBlock.Instructions[beforeExceptionCaptureBlock.Instructions.Count - 3].MatchStLoc(objectVariableCopy, out var value)
						&& value.MatchLdLoc(objectVariable))
					{
						if (cond.MatchCompEqualsNull(out var arg) && arg.MatchLdLoc(objectVariableCopy))
						{
							context.Step("Simplify end of finally", beforeExceptionCaptureBlock);
							beforeExceptionCaptureBlock.Instructions.RemoveRange(beforeExceptionCaptureBlock.Instructions.Count - 3, 2);
							branch.ReplaceWith(new Leave(finallyContainer).WithILRange(branch));
						}
						else if (cond.MatchCompNotEqualsNull(out arg) && arg.MatchLdLoc(objectVariableCopy))
						{
							context.Step("Simplify end of finally", beforeExceptionCaptureBlock);
							beforeExceptionCaptureBlock.Instructions.RemoveRange(beforeExceptionCaptureBlock.Instructions.Count - 3, 2);
							branch.ReplaceWith(new Leave(finallyContainer).WithILRange(branch));
						}
					}
				}

				foreach (var branch in container.Descendants.OfType<Branch>())
				{
					if (branch.TargetBlock == entryPointOfFinally && branch.IsDescendantOf(tryCatch.TryBlock))
					{
						context.Step("branch to finally => branch after finally", branch);
						branch.ReplaceWith(new Branch(afterFinallyBlock).WithILRange(branch));
					}
					else if (branch.TargetBlock == capturePatternStart)
					{
						if (branch.IsDescendantOf(finallyContainer))
						{
							context.Step("branch out of finally container => leave finally container", branch);
							branch.ReplaceWith(new Leave(finallyContainer).WithILRange(branch));
						}
					}
				}

				context.StepEndGroup(keepIfEmpty: true);
			}

			context.Step("Clean up", function);

			// clean up all modified containers
			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);

			((BlockContainer)function.Body).SortBlocks(deleteUnreachableBlocks: true);

			void MoveDominatedBlocksToContainer(Block newEntryPoint, Block endBlock, ControlFlowGraph graph,
				BlockContainer targetContainer, List<Block> removedBlocks)
			{
				var node = graph.GetNode(newEntryPoint);
				var endNode = endBlock == null ? null : graph.GetNode(endBlock);

				MoveBlock(newEntryPoint, targetContainer);

				foreach (var n in graph.cfg)
				{
					Block block = (Block)n.UserData;

					if (node.Dominates(n))
					{
						if (endNode != null && endNode != n && endNode.Dominates(n))
							continue;

						if (block.Parent == targetContainer)
							continue;

						if (!removedBlocks.Contains(block))
						{
							MoveBlock(block, targetContainer);
						}
					}
				}
			}

			void MoveBlock(Block block, BlockContainer target)
			{
				context.Step($"Move {block.Label} to container at IL_{target.StartILOffset:x4}", target);
				block.Remove();
				target.Blocks.Add(block);
			}
		}

		static (Block, Block, ILVariable) FindBlockAfterFinally(ILTransformContext context, Block block, ILVariable objectVariable)
		{
			int count = block.Instructions.Count;
			if (count < 3)
				return default;

			if (!block.Instructions[count - 3].MatchStLoc(out var objectVariableCopy, out var value))
				return default;

			if (!value.MatchLdLoc(objectVariable))
				return default;

			if (!block.Instructions[count - 2].MatchIfInstruction(out var cond, out var afterExceptionCaptureBlockBranch))
				return default;

			if (!afterExceptionCaptureBlockBranch.MatchBranch(out var afterExceptionCaptureBlock))
				return default;

			if (!block.Instructions[count - 1].MatchBranch(out var exceptionCaptureBlock))
				return default;

			if (cond.MatchCompEqualsNull(out var arg))
			{
				if (!arg.MatchLdLoc(objectVariableCopy))
					return default;
			}
			else if (cond.MatchCompNotEqualsNull(out arg))
			{
				if (!arg.MatchLdLoc(objectVariableCopy))
					return default;
				(afterExceptionCaptureBlock, exceptionCaptureBlock) = (exceptionCaptureBlock, afterExceptionCaptureBlock);
			}
			else
			{
				return default;
			}

			if (!AwaitInCatchTransform.MatchExceptionCaptureBlock(context, exceptionCaptureBlock,
				ref objectVariableCopy, out var store, out _, out _))
			{
				return default;
			}

			var exceptionCaptureBlockStart = LocalFunctionDecompiler.GetStatement(store);
			if (exceptionCaptureBlockStart == null)
				return default;


			return (afterExceptionCaptureBlock, (Block)exceptionCaptureBlockStart.Parent, objectVariableCopy);
		}
	}
}
