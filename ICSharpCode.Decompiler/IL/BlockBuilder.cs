using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder(Mono.Cecil.Cil.MethodBody body, bool instructionInlining)
	{
		BlockContainer currentContainer;
		Block currentBlock;
		Stack<ILInstruction> instructionStack = (instructionInlining ? new Stack<ILInstruction>() : null);

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
						var inlinedInst = inst.Inline(InstructionFlags.None, instructionStack, out bool finished);
						if (inlinedInst is Branch) {
							// Values currently on the stack might be used on both sides of the branch,
							// so we can't inline them.
							FlushInstructionStack();
						}
						if (inlinedInst.NoResult) {
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
					if (!inst.IsEndReachable)
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
				currentBlock.Instructions.Add(new Branch(OpCode.Branch, currentILOffset));
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
