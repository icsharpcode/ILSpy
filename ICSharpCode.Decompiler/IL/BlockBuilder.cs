using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class BlockBuilder(private readonly Mono.Cecil.Cil.MethodBody body, bool instructionInlining)
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
						currentBlock.Instructions.Add(inst);
					} else {
						var inlinedInst = inst.Inline(InstructionFlags.None, instructionStack, out bool finished);
						instructionStack.Push(inlinedInst);
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
			if (instructionStack != null && instructionStack.Count > 0) {
				// Flush instruction stack
				currentBlock.Instructions.AddRange(instructionStack.Reverse());
				instructionStack.Clear();
			}
			currentBlock.ILRange = new Interval(currentBlock.ILRange.Start, currentILOffset);
			if (fallthrough)
				currentBlock.Instructions.Add(new Branch(OpCode.Branch, currentILOffset));
			currentBlock = null;
		}
	}
}
