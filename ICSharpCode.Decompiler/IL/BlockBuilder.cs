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

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder
	{
		readonly Mono.Cecil.Cil.MethodBody body;
		readonly Stack<ILInstruction> instructionStack;
		
		public BlockBuilder(Mono.Cecil.Cil.MethodBody body, bool instructionInlining)
		{
			this.body = body;
			this.instructionStack = (instructionInlining ? new Stack<ILInstruction>() : null);
		}
		
		BlockContainer currentContainer;
		Block currentBlock;
		
		public BlockContainer CreateBlocks(List<ILInstruction> instructions, BitArray incomingBranches)
		{
			currentContainer = new BlockContainer();

			incomingBranches[0] = true;	// see entrypoint as incoming branch

			foreach (var inst in instructions) {
				int start = inst.ILRange.Start;
				if (incomingBranches[start]) {
					// Finish up the previous block
					FinalizeCurrentBlock(start, fallthrough: true);
					// Create the new block
					currentBlock = new Block();
					currentContainer.Blocks.Add(currentBlock);
					currentBlock.ILRange = new Interval(start, start);
				}
				if (currentBlock != null) {
					if (instructionStack == null) {
						// inlining disabled
						currentBlock.Instructions.Add(inst);
					} else {
						bool finished;
						var inlinedInst = inst.Inline(InstructionFlags.None, instructionStack, out finished);
						if (inlinedInst is Branch) {
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
					if (inst.HasFlag(InstructionFlags.EndPointUnreachable))
						FinalizeCurrentBlock(inst.ILRange.End, fallthrough: false);
				}
			}
			FinalizeCurrentBlock(body.CodeSize, fallthrough: false);
			return currentContainer;
		}

		private void FinalizeCurrentBlock(int currentILOffset, bool fallthrough)
		{
			if (currentBlock == null)
				return;
			FlushInstructionStack();
			currentBlock.ILRange = new Interval(currentBlock.ILRange.Start, currentILOffset);
			if (fallthrough)
				currentBlock.Instructions.Add(new Branch(currentILOffset));
			currentBlock = null;
		}

		private void FlushInstructionStack()
		{
			if (instructionStack != null && instructionStack.Count > 0) {
				// Flush instruction stack
				currentBlock.Instructions.AddRange(instructionStack.Reverse());
				instructionStack.Clear();
			}
		}
	}
}
