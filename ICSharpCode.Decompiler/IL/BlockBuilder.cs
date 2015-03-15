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
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder
	{
		readonly Mono.Cecil.Cil.MethodBody body;
		readonly IDecompilerTypeSystem typeSystem;

		public BlockBuilder(Mono.Cecil.Cil.MethodBody body, IDecompilerTypeSystem typeSystem)
		{
			Debug.Assert(body != null);
			Debug.Assert(typeSystem != null);
			this.body = body;
			this.typeSystem = typeSystem;
		}
		
		List<TryInstruction> tryInstructionList = new List<TryInstruction>();
		Dictionary<int, BlockContainer> handlerContainers = new Dictionary<int, BlockContainer>();
		
		void CreateContainerStructure()
		{
			List<TryCatch> tryCatchList = new List<TryCatch>();
			foreach (var eh in body.ExceptionHandlers) {
				var tryRange = new Interval(eh.TryStart.Offset, eh.TryEnd != null ? eh.TryEnd.Offset : body.CodeSize);
				var handlerBlock = new BlockContainer();
				handlerBlock.ILRange = new Interval(eh.HandlerStart.Offset, eh.HandlerEnd != null ? eh.HandlerEnd.Offset : body.CodeSize);
				handlerBlock.Blocks.Add(new Block());
				handlerContainers.Add(handlerBlock.ILRange.Start, handlerBlock);
				
				if (eh.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Fault || eh.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Finally) {
					var tryBlock = new BlockContainer();
					tryBlock.ILRange = tryRange;
					if (eh.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Finally)
						tryInstructionList.Add(new TryFinally(tryBlock, handlerBlock));
					else
						tryInstructionList.Add(new TryFault(tryBlock, handlerBlock));
					continue;
				}
				// 
				var tryCatch = tryCatchList.FirstOrDefault(tc => tc.TryBlock.ILRange == tryRange);
				if (tryCatch == null) {
					var tryBlock = new BlockContainer();
					tryBlock.ILRange = tryRange;
					tryCatch = new TryCatch(tryBlock);
					tryCatchList.Add(tryCatch);
					tryInstructionList.Add(tryCatch);
				}

				var variable = new ILVariable(VariableKind.Exception, typeSystem.Resolve(eh.CatchType), handlerBlock.ILRange.Start);
				variable.Name = "ex";
				handlerBlock.EntryPoint.Instructions.Add(new LdLoc(variable));
				
				ILInstruction filter;
				if (eh.HandlerType == Mono.Cecil.Cil.ExceptionHandlerType.Filter) {
					var filterBlock = new BlockContainer();
					filterBlock.ILRange = new Interval(eh.FilterStart.Offset, eh.HandlerStart.Offset);
					filterBlock.Blocks.Add(new Block());
					filterBlock.EntryPoint.Instructions.Add(new LdLoc(variable));
					handlerContainers.Add(filterBlock.ILRange.Start, filterBlock);
					filter = filterBlock;
				} else {
					filter = new LdcI4(1);
				}

				tryCatch.Handlers.Add(new TryCatchHandler(filter, handlerBlock, variable));
			}
			if (tryInstructionList.Count > 0) {
				tryInstructionList = tryInstructionList.OrderBy(tc => tc.TryBlock.ILRange.Start).ThenByDescending(tc => tc.TryBlock.ILRange.End).ToList();
				nextTry = tryInstructionList[0];
			}
		}
		
		int currentTryIndex;
		TryInstruction nextTry;
		
		BlockContainer currentContainer;
		Block currentBlock;
		Stack<BlockContainer> containerStack = new Stack<BlockContainer>();
		
		public BlockContainer CreateBlocks(List<ILInstruction> instructions, BitArray incomingBranches)
		{
			CreateContainerStructure();
			var mainContainer = new BlockContainer();
			mainContainer.ILRange = new Interval(0, body.CodeSize);
			currentContainer = mainContainer;

			foreach (var inst in instructions) {
				int start = inst.ILRange.Start;
				if (currentBlock == null || incomingBranches[start]) {
					// Finish up the previous block
					FinalizeCurrentBlock(start, fallthrough: true);
					// Leave nested containers if necessary
					while (start >= currentContainer.ILRange.End) {
						currentContainer = containerStack.Pop();
					}
					// Enter a handler if necessary
					BlockContainer handlerContainer;
					if (handlerContainers.TryGetValue(start, out handlerContainer)) {
						containerStack.Push(currentContainer);
						currentContainer = handlerContainer;
						currentBlock = handlerContainer.EntryPoint;
					} else {
						// Create the new block
						currentBlock = new Block();
						currentContainer.Blocks.Add(currentBlock);
					}
					currentBlock.ILRange = new Interval(start, start);
				}
				while (nextTry != null && start == nextTry.TryBlock.ILRange.Start) {
					currentBlock.Instructions.Add(nextTry);
					containerStack.Push(currentContainer);
					currentContainer = (BlockContainer)nextTry.TryBlock;
					currentBlock = new Block();
					currentContainer.Blocks.Add(currentBlock);
					currentBlock.ILRange = new Interval(start, start);
					
					nextTry = tryInstructionList.ElementAtOrDefault(++currentTryIndex);
				}
				currentBlock.Instructions.Add(inst);
				if (inst.HasFlag(InstructionFlags.EndPointUnreachable))
					FinalizeCurrentBlock(inst.ILRange.End, fallthrough: false);
			}
			FinalizeCurrentBlock(body.CodeSize, fallthrough: false);
			containerStack.Clear();
			ConnectBranches(mainContainer);
			return mainContainer;
		}

		private void FinalizeCurrentBlock(int currentILOffset, bool fallthrough)
		{
			if (currentBlock == null)
				return;
			currentBlock.ILRange = new Interval(currentBlock.ILRange.Start, currentILOffset);
			if (fallthrough)
				currentBlock.Instructions.Add(new Branch(currentILOffset));
			currentBlock = null;
		}

		void ConnectBranches(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.Branch:
					var branch = (Branch)inst;
					branch.TargetBlock = FindBranchTarget(branch.TargetILOffset);
					break;
				case OpCode.BlockContainer:
					var container = (BlockContainer)inst;
					containerStack.Push(container);
					foreach (var block in container.Blocks)
						ConnectBranches(block);
					containerStack.Pop();
					break;
				default:
					foreach (var child in inst.Children)
						ConnectBranches(child);
					break;
			}
		}

		Block FindBranchTarget(int targetILOffset)
		{
			foreach (var container in containerStack) {
				foreach (var block in container.Blocks) {
					if (block.ILRange.Start == targetILOffset)
						return block;
				}
			}
			throw new InvalidOperationException("Could not find block for branch target");
		}
	}
}


/* Inlining logic: } else {
						bool finished;
						var inlinedInst = inst.Inline(InstructionFlags.None, instructionStack, out finished);
						if (inlinedInst.HasFlag(InstructionFlags.MayBranch)) {
							// Values currently on the stack might be used on both sides of the branch,
							// so we can't inline them.
							FlushInstructionStack();
						}
						if (inlinedInst.ResultType == StackType.Void) {
							// We cannot directly push instructions onto the stack if they don't produce
							// a result.
							if (finished && instructionStack.Count > 0) {
								// Wrap the instruction on top of the stack into an inline block,
								// and append our void-typed instruction to the end of that block.
								var headInst = instructionStack.Pop();
								var block = headInst as Block ?? new Block { Instructions = { headInst } };
								block.Instructions.Add(inlinedInst);
								instructionStack.Push(block);
							} else {
								// We can't move incomplete instructions into a nested block
								// or the instruction stack was empty
								FlushInstructionStack();
								currentBlock.Instructions.Add(inst);
							}
						} else {
							// Instruction has a result, so we can push it on the stack normally
							instructionStack.Push(inlinedInst);
						}
					}
		private void FlushInstructionStack()
		{
			if (instructionStack != null && instructionStack.Count > 0) {
				// Flush instruction stack
				currentBlock.Instructions.AddRange(instructionStack.Reverse());
				instructionStack.Clear();
			}
		}*/
