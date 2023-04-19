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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder
	{
		readonly MethodBodyBlock body;
		readonly Dictionary<ExceptionRegion, ILVariable> variableByExceptionHandler;
		readonly ICompilation compilation;

		/// <summary>
		/// Gets/Sets whether to create extended basic blocks instead of basic blocks.
		/// The default is <c>false</c>.
		/// </summary>
		public bool CreateExtendedBlocks;

		internal BlockBuilder(MethodBodyBlock body,
							  Dictionary<ExceptionRegion, ILVariable> variableByExceptionHandler,
							  ICompilation compilation)
		{
			Debug.Assert(body != null);
			Debug.Assert(variableByExceptionHandler != null);
			Debug.Assert(compilation != null);
			this.body = body;
			this.variableByExceptionHandler = variableByExceptionHandler;
			this.compilation = compilation;
		}

		List<TryInstruction> tryInstructionList = new List<TryInstruction>();
		readonly Dictionary<int, BlockContainer> handlerContainers = new Dictionary<int, BlockContainer>();

		void CreateContainerStructure()
		{
			List<TryCatch> tryCatchList = new List<TryCatch>();
			foreach (var eh in body.ExceptionRegions)
			{
				var tryRange = new Interval(eh.TryOffset, eh.TryOffset + eh.TryLength);
				var handlerBlock = new BlockContainer();
				handlerBlock.AddILRange(new Interval(eh.HandlerOffset, eh.HandlerOffset + eh.HandlerLength));
				handlerBlock.Blocks.Add(new Block());
				handlerContainers.Add(handlerBlock.StartILOffset, handlerBlock);

				if (eh.Kind == ExceptionRegionKind.Fault || eh.Kind == ExceptionRegionKind.Finally)
				{
					var tryBlock = new BlockContainer();
					tryBlock.AddILRange(tryRange);
					if (eh.Kind == ExceptionRegionKind.Finally)
						tryInstructionList.Add(new TryFinally(tryBlock, handlerBlock).WithILRange(tryRange));
					else
						tryInstructionList.Add(new TryFault(tryBlock, handlerBlock).WithILRange(tryRange));
					continue;
				}
				// 
				var tryCatch = tryCatchList.FirstOrDefault(tc => tc.TryBlock.ILRanges.SingleOrDefault() == tryRange);
				if (tryCatch == null)
				{
					var tryBlock = new BlockContainer();
					tryBlock.AddILRange(tryRange);
					tryCatch = new TryCatch(tryBlock);
					tryCatch.AddILRange(tryRange);
					tryCatchList.Add(tryCatch);
					tryInstructionList.Add(tryCatch);
				}

				ILInstruction filter;
				if (eh.Kind == System.Reflection.Metadata.ExceptionRegionKind.Filter)
				{
					var filterBlock = new BlockContainer(expectedResultType: StackType.I4);
					filterBlock.AddILRange(new Interval(eh.FilterOffset, eh.HandlerOffset));
					filterBlock.Blocks.Add(new Block());
					handlerContainers.Add(filterBlock.StartILOffset, filterBlock);
					filter = filterBlock;
				}
				else
				{
					filter = new LdcI4(1);
				}

				var handler = new TryCatchHandler(filter, handlerBlock, variableByExceptionHandler[eh]);
				handler.AddILRange(filter);
				handler.AddILRange(handlerBlock);
				tryCatch.Handlers.Add(handler);
				tryCatch.AddILRange(handler);
			}
			if (tryInstructionList.Count > 0)
			{
				tryInstructionList = tryInstructionList.OrderBy(tc => tc.TryBlock.StartILOffset).ThenByDescending(tc => tc.TryBlock.EndILOffset).ToList();
				nextTry = tryInstructionList[0];
			}
		}

		int currentTryIndex;
		TryInstruction nextTry;

		BlockContainer currentContainer;
		Block currentBlock;
		readonly Stack<BlockContainer> containerStack = new Stack<BlockContainer>();

		public void CreateBlocks(BlockContainer mainContainer, List<ILInstruction> instructions, BitSet incomingBranches, CancellationToken cancellationToken)
		{
			CreateContainerStructure();
			mainContainer.SetILRange(new Interval(0, body.GetCodeSize()));
			currentContainer = mainContainer;
			if (instructions.Count == 0)
			{
				currentContainer.Blocks.Add(new Block {
					Instructions = {
						new InvalidBranch("Empty body found. Decompiled assembly might be a reference assembly.")
					}
				});
				return;
			}

			foreach (var inst in instructions)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int start = inst.StartILOffset;
				if (currentBlock == null || (incomingBranches[start] && !IsStackAdjustment(inst)))
				{
					// Finish up the previous block
					FinalizeCurrentBlock(start, fallthrough: true);
					// Leave nested containers if necessary
					while (start >= currentContainer.EndILOffset)
					{
						currentContainer = containerStack.Pop();
						currentBlock = currentContainer.Blocks.Last();
						// this container is skipped (i.e. the loop will execute again)
						// set ILRange to the last instruction offset inside the block.
						if (start >= currentContainer.EndILOffset)
						{
							Debug.Assert(currentBlock.ILRangeIsEmpty);
							currentBlock.AddILRange(new Interval(currentBlock.StartILOffset, start));
						}
					}
					// Enter a handler if necessary
					if (handlerContainers.TryGetValue(start, out BlockContainer handlerContainer))
					{
						containerStack.Push(currentContainer);
						currentContainer = handlerContainer;
						currentBlock = handlerContainer.EntryPoint;
					}
					else
					{
						FinalizeCurrentBlock(start, fallthrough: false);
						// Create the new block
						currentBlock = new Block();
						currentContainer.Blocks.Add(currentBlock);
					}
					currentBlock.SetILRange(new Interval(start, start));
				}
				while (nextTry != null && start == nextTry.TryBlock.StartILOffset)
				{
					currentBlock.Instructions.Add(nextTry);
					containerStack.Push(currentContainer);
					currentContainer = (BlockContainer)nextTry.TryBlock;
					currentBlock = new Block();
					currentContainer.Blocks.Add(currentBlock);
					currentBlock.SetILRange(new Interval(start, start));

					nextTry = tryInstructionList.ElementAtOrDefault(++currentTryIndex);
				}
				currentBlock.Instructions.Add(inst);
				if (inst.HasFlag(InstructionFlags.EndPointUnreachable))
					FinalizeCurrentBlock(inst.EndILOffset, fallthrough: false);
				else if (!CreateExtendedBlocks && inst.HasFlag(InstructionFlags.MayBranch))
					FinalizeCurrentBlock(inst.EndILOffset, fallthrough: true);
			}
			FinalizeCurrentBlock(mainContainer.EndILOffset, fallthrough: false);
			// Finish up all containers
			while (containerStack.Count > 0)
			{
				currentContainer = containerStack.Pop();
				currentBlock = currentContainer.Blocks.Last();
				FinalizeCurrentBlock(mainContainer.EndILOffset, fallthrough: false);
			}
			ConnectBranches(mainContainer, cancellationToken);
			CreateOnErrorDispatchers();
		}

		static bool IsStackAdjustment(ILInstruction inst)
		{
			return inst is StLoc stloc && stloc.IsStackAdjustment;
		}

		private void FinalizeCurrentBlock(int currentILOffset, bool fallthrough)
		{
			if (currentBlock == null)
				return;
			Debug.Assert(currentBlock.ILRangeIsEmpty);
			currentBlock.SetILRange(new Interval(currentBlock.StartILOffset, currentILOffset));
			if (fallthrough)
			{
				if (currentBlock.Instructions.LastOrDefault() is SwitchInstruction switchInst && switchInst.Sections.Last().Body.MatchNop())
				{
					// Instead of putting the default branch after the switch instruction
					switchInst.Sections.Last().Body = new Branch(currentILOffset);
					Debug.Assert(switchInst.HasFlag(InstructionFlags.EndPointUnreachable));
				}
				else
				{
					currentBlock.Instructions.Add(new Branch(currentILOffset));
				}
			}
			currentBlock = null;
		}

		void ConnectBranches(ILInstruction inst, CancellationToken cancellationToken)
		{
			switch (inst)
			{
				case Branch branch:
					cancellationToken.ThrowIfCancellationRequested();
					Debug.Assert(branch.TargetBlock == null);
					branch.TargetBlock = FindBranchTarget(branch.TargetILOffset);
					if (branch.TargetBlock == null)
					{
						branch.ReplaceWith(new InvalidBranch("Could not find block for branch target "
							+ Disassembler.DisassemblerHelpers.OffsetToString(branch.TargetILOffset)).WithILRange(branch));
					}
					break;
				case Leave leave:
					// ret (in void method) = leave(mainContainer)
					// endfinally = leave(null)
					if (leave.TargetContainer == null)
					{
						// assign the finally/filter container
						leave.TargetContainer = containerStack.Peek();
						leave.Value = ILReader.Cast(leave.Value, leave.TargetContainer.ExpectedResultType, null, leave.StartILOffset);
					}
					break;
				case BlockContainer container:
					containerStack.Push(container);
					// Note: FindBranchTarget()/CreateBranchTargetForOnErrorJump() may append to container.Blocks while we are iterating.
					// Don't process those artificial blocks here.
					int blockCount = container.Blocks.Count;
					for (int i = 0; i < blockCount; i++)
					{
						cancellationToken.ThrowIfCancellationRequested();
						var block = container.Blocks[i];
						ConnectBranches(block, cancellationToken);
						if (block.Instructions.Count == 0 || !block.Instructions.Last().HasFlag(InstructionFlags.EndPointUnreachable))
						{
							block.Instructions.Add(new InvalidBranch("Unexpected end of block"));
						}
					}
					containerStack.Pop();
					break;
				default:
					foreach (var child in inst.Children)
						ConnectBranches(child, cancellationToken);
					break;
			}
		}

		Block FindBranchTarget(int targetILOffset)
		{
			foreach (var container in containerStack)
			{
				foreach (var block in container.Blocks)
				{
					if (block.StartILOffset == targetILOffset && !block.ILRangeIsEmpty)
						return block;
				}
				if (container.SlotInfo == TryCatchHandler.BodySlot)
				{
					// catch handler is allowed to branch back into try block (VB On Error)
					TryCatch tryCatch = (TryCatch)container.Parent.Parent;
					if (tryCatch.TryBlock.StartILOffset < targetILOffset && targetILOffset < tryCatch.TryBlock.EndILOffset)
					{
						return CreateBranchTargetForOnErrorJump(tryCatch, targetILOffset);
					}
				}
			}
			return null;
		}

		readonly Dictionary<TryCatch, OnErrorDispatch> onErrorDispatchers = new Dictionary<TryCatch, OnErrorDispatch>();

		class OnErrorDispatch
		{
			public readonly ILVariable Variable;
			public readonly HashSet<int> TargetILOffsets = new HashSet<int>();
			public readonly List<Branch> Branches = new List<Branch>();

			public OnErrorDispatch(ILVariable variable)
			{
				Debug.Assert(variable != null);
				this.Variable = variable;
			}
		}

		/// Create a new block that sets a helper variable and then branches to the start of the try-catch
		Block CreateBranchTargetForOnErrorJump(TryCatch tryCatch, int targetILOffset)
		{
			if (!onErrorDispatchers.TryGetValue(tryCatch, out var dispatch))
			{
				var int32 = compilation.FindType(KnownTypeCode.Int32);
				var newDispatchVar = new ILVariable(VariableKind.Local, int32, StackType.I4);
				newDispatchVar.Name = $"try{tryCatch.StartILOffset:x4}_dispatch";
				dispatch = new OnErrorDispatch(newDispatchVar);
				onErrorDispatchers.Add(tryCatch, dispatch);
			}
			dispatch.TargetILOffsets.Add(targetILOffset);

			Block block = new Block();
			block.Instructions.Add(new StLoc(dispatch.Variable, new LdcI4(targetILOffset)));
			var branch = new Branch(tryCatch.TryBlock.StartILOffset);
			block.Instructions.Add(branch);
			dispatch.Branches.Add(branch);
			containerStack.Peek().Blocks.Add(block);
			return block;
		}

		/// New variables introduced for the "on error" dispatchers
		public IEnumerable<ILVariable> OnErrorDispatcherVariables => onErrorDispatchers.Values.Select(d => d.Variable);

		void CreateOnErrorDispatchers()
		{
			foreach (var (tryCatch, dispatch) in onErrorDispatchers)
			{
				Block block = (Block)tryCatch.Parent;
				// Before the regular entry point of the try-catch, insert an. instruction that resets the dispatcher variable
				block.Instructions.Insert(tryCatch.ChildIndex, new StLoc(dispatch.Variable, new LdcI4(-1)));
				// Split the block, so that we can introduce branches that jump directly into the try block
				int splitAt = tryCatch.ChildIndex;
				Block newBlock = new Block();
				newBlock.AddILRange(tryCatch);
				newBlock.Instructions.AddRange(block.Instructions.Skip(splitAt));
				block.Instructions.RemoveRange(splitAt, block.Instructions.Count - splitAt);
				block.Instructions.Add(new Branch(newBlock));
				((BlockContainer)block.Parent).Blocks.Add(newBlock);
				// Update the branches that jump directly into the try block
				foreach (var b in dispatch.Branches)
				{
					b.TargetBlock = newBlock;
				}

				// Inside the try-catch, create the dispatch switch
				BlockContainer tryBody = (BlockContainer)tryCatch.TryBlock;
				Block dispatchBlock = new Block();
				dispatchBlock.AddILRange(new Interval(tryCatch.StartILOffset, tryCatch.StartILOffset + 1));
				var switchInst = new SwitchInstruction(new LdLoc(dispatch.Variable));
				switchInst.AddILRange(new Interval(tryCatch.StartILOffset, tryCatch.StartILOffset + 1));
				foreach (int offset in dispatch.TargetILOffsets)
				{
					var targetBlock = tryBody.Blocks.FirstOrDefault(b => b.StartILOffset == offset && !b.ILRangeIsEmpty);
					ILInstruction branchInst;
					if (targetBlock == null)
					{
						branchInst = new InvalidBranch("Could not find block for branch target "
													+ Disassembler.DisassemblerHelpers.OffsetToString(offset));
					}
					else
					{
						branchInst = new Branch(targetBlock);
					}
					switchInst.Sections.Add(new SwitchSection { Labels = new LongSet(offset), Body = branchInst });
				}
				var usedLabels = new LongSet(dispatch.TargetILOffsets.Select(offset => LongInterval.Inclusive(offset, offset)));
				switchInst.Sections.Add(new SwitchSection { Labels = usedLabels.Invert(), Body = new Branch(tryBody.EntryPoint) });
				dispatchBlock.Instructions.Add(new InvalidExpression("ILSpy has introduced the following switch to emulate a goto from catch-block to try-block") { Severity = "Note" });
				dispatchBlock.Instructions.Add(switchInst);

				tryBody.Blocks.Insert(0, dispatchBlock);
			}
		}
	}
}
