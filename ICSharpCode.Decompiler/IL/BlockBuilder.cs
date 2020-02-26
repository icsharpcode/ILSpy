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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder
	{
		readonly MethodBodyBlock body;
		readonly Dictionary<ExceptionRegion, ILVariable> variableByExceptionHandler;

		/// <summary>
		/// Gets/Sets whether to create extended basic blocks instead of basic blocks.
		/// The default is <c>false</c>.
		/// </summary>
		public bool CreateExtendedBlocks;
		
		internal BlockBuilder(MethodBodyBlock body,
		                      Dictionary<ExceptionRegion, ILVariable> variableByExceptionHandler)
		{
			Debug.Assert(body != null);
			Debug.Assert(variableByExceptionHandler != null);
			this.body = body;
			this.variableByExceptionHandler = variableByExceptionHandler;
		}
		
		List<TryInstruction> tryInstructionList = new List<TryInstruction>();
		Dictionary<int, BlockContainer> handlerContainers = new Dictionary<int, BlockContainer>();
		
		void CreateContainerStructure()
		{
			List<TryCatch> tryCatchList = new List<TryCatch>();
			foreach (var eh in body.ExceptionRegions) {
				var tryRange = new Interval(eh.TryOffset, eh.TryOffset + eh.TryLength);
				var handlerBlock = new BlockContainer();
				handlerBlock.AddILRange(new Interval(eh.HandlerOffset, eh.HandlerOffset + eh.HandlerLength));
				handlerBlock.Blocks.Add(new Block());
				handlerContainers.Add(handlerBlock.StartILOffset, handlerBlock);
				
				if (eh.Kind == ExceptionRegionKind.Fault || eh.Kind == ExceptionRegionKind.Finally) {
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
				if (tryCatch == null) {
					var tryBlock = new BlockContainer();
					tryBlock.AddILRange(tryRange);
					tryCatch = new TryCatch(tryBlock);
					tryCatch.AddILRange(tryRange);
					tryCatchList.Add(tryCatch);
					tryInstructionList.Add(tryCatch);
				}

				ILInstruction filter;
				if (eh.Kind == System.Reflection.Metadata.ExceptionRegionKind.Filter) {
					var filterBlock = new BlockContainer(expectedResultType: StackType.I4);
					filterBlock.AddILRange(new Interval(eh.FilterOffset, eh.HandlerOffset));
					filterBlock.Blocks.Add(new Block());
					handlerContainers.Add(filterBlock.StartILOffset, filterBlock);
					filter = filterBlock;
				} else {
					filter = new LdcI4(1);
				}

				var handler = new TryCatchHandler(filter, handlerBlock, variableByExceptionHandler[eh]);
				handler.AddILRange(filter);
				handler.AddILRange(handlerBlock);
				tryCatch.Handlers.Add(handler);
				tryCatch.AddILRange(handler);
			}
			if (tryInstructionList.Count > 0) {
				tryInstructionList = tryInstructionList.OrderBy(tc => tc.TryBlock.StartILOffset).ThenByDescending(tc => tc.TryBlock.EndILOffset).ToList();
				nextTry = tryInstructionList[0];
			}
		}
		
		int currentTryIndex;
		TryInstruction nextTry;
		
		BlockContainer currentContainer;
		Block currentBlock;
		Stack<BlockContainer> containerStack = new Stack<BlockContainer>();
		
		public void CreateBlocks(BlockContainer mainContainer, List<ILInstruction> instructions, BitArray incomingBranches, CancellationToken cancellationToken)
		{
			CreateContainerStructure();
			mainContainer.SetILRange(new Interval(0, body.GetCodeSize()));
			currentContainer = mainContainer;
			if (instructions.Count == 0) {
				currentContainer.Blocks.Add(new Block {
					Instructions = {
						new InvalidBranch("Empty body found. Decompiled assembly might be a reference assembly.")
					}
				});
				return;
			}

			foreach (var inst in instructions) {
				cancellationToken.ThrowIfCancellationRequested();
				int start = inst.StartILOffset;
				if (currentBlock == null || (incomingBranches[start] && !IsStackAdjustment(inst))) {
					// Finish up the previous block
					FinalizeCurrentBlock(start, fallthrough: true);
					// Leave nested containers if necessary
					while (start >= currentContainer.EndILOffset) {
						currentContainer = containerStack.Pop();
						currentBlock = currentContainer.Blocks.Last();
						// this container is skipped (i.e. the loop will execute again)
						// set ILRange to the last instruction offset inside the block.
						if (start >= currentContainer.EndILOffset) {
							Debug.Assert(currentBlock.ILRangeIsEmpty);
							currentBlock.AddILRange(new Interval(currentBlock.StartILOffset, start));
						}
					}
					// Enter a handler if necessary
					BlockContainer handlerContainer;
					if (handlerContainers.TryGetValue(start, out handlerContainer)) {
						containerStack.Push(currentContainer);
						currentContainer = handlerContainer;
						currentBlock = handlerContainer.EntryPoint;
					} else {
						FinalizeCurrentBlock(start, fallthrough: false);
						// Create the new block
						currentBlock = new Block();
						currentContainer.Blocks.Add(currentBlock);
					}
					currentBlock.SetILRange(new Interval(start, start));
				}
				while (nextTry != null && start == nextTry.TryBlock.StartILOffset) {
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
			while (containerStack.Count > 0) {
				currentContainer = containerStack.Pop();
				currentBlock = currentContainer.Blocks.Last();
				FinalizeCurrentBlock(mainContainer.EndILOffset, fallthrough: false);
			}
			ConnectBranches(mainContainer, cancellationToken);
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
			if (fallthrough) {
				if (currentBlock.Instructions.LastOrDefault() is SwitchInstruction switchInst && switchInst.Sections.Last().Body.MatchNop()) {
					// Instead of putting the default branch after the switch instruction
					switchInst.Sections.Last().Body = new Branch(currentILOffset);
					Debug.Assert(switchInst.HasFlag(InstructionFlags.EndPointUnreachable));
				} else {
					currentBlock.Instructions.Add(new Branch(currentILOffset));
				}
			}
			currentBlock = null;
		}

		void ConnectBranches(ILInstruction inst, CancellationToken cancellationToken)
		{
			switch (inst) {
				case Branch branch:
					cancellationToken.ThrowIfCancellationRequested();
					Debug.Assert(branch.TargetBlock == null);
					branch.TargetBlock = FindBranchTarget(branch.TargetILOffset);
					if (branch.TargetBlock == null) {
						branch.ReplaceWith(new InvalidBranch("Could not find block for branch target "
							+ Disassembler.DisassemblerHelpers.OffsetToString(branch.TargetILOffset)).WithILRange(branch));
					}
					break;
				case Leave leave:
					// ret (in void method) = leave(mainContainer)
					// endfinally = leave(null)
					if (leave.TargetContainer == null) {
						// assign the finally/filter container
						leave.TargetContainer = containerStack.Peek();
						leave.Value = ILReader.Cast(leave.Value, leave.TargetContainer.ExpectedResultType, null, leave.StartILOffset);
					}
					break;
				case BlockContainer container:
					containerStack.Push(container);
					foreach (var block in container.Blocks) {
						cancellationToken.ThrowIfCancellationRequested();
						ConnectBranches(block, cancellationToken);
						if (block.Instructions.Count == 0 || !block.Instructions.Last().HasFlag(InstructionFlags.EndPointUnreachable)) {
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
			foreach (var container in containerStack) {
				foreach (var block in container.Blocks) {
					if (block.StartILOffset == targetILOffset)
						return block;
				}
			}
			return null;
		}
	}
}
