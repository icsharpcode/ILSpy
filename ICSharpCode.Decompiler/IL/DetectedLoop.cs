// Copyright (c) 2017 Siegfried Pammer
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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	public enum LoopKind { While, DoWhile, For }

	public class DetectedLoop
	{
		public BlockContainer Container { get; }
		public LoopKind Kind { get; private set; }
		public ILInstruction Condition { get; private set; }
		public Block IncrementBlock { get; private set; }
		public Block ContinueJumpTarget { get; private set; } // jumps to this block are "continue;" jumps
		public ILInstruction Body { get; private set; }       // null in case of DoWhile
		public Block[] AdditionalBlocks { get; private set; } // blocks to be merged into the loop body

		private DetectedLoop(BlockContainer container)
		{
			this.Container = container;
		}

		/// <summary>
		/// Gets whether the statement is 'simple' (usable as for loop iterator):
		/// Currently we only accept calls and assignments.
		/// </summary>
		private static bool IsSimpleStatement(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.Call:
				case OpCode.CallVirt:
				case OpCode.NewObj:
				case OpCode.StLoc:
				case OpCode.StObj:
				case OpCode.CompoundAssignmentInstruction:
					return true;
				default:
					return false;
			}
		}

		public static DetectedLoop DetectLoop(BlockContainer container)
		{
			return new DetectedLoop(container).DetectLoopInternal();
		}

		private static Block FindDoWhileConditionBlock(BlockContainer container, out ILInstruction condition)
		{
			condition = null;
			foreach (var b in container.Blocks) {
				if (b.Instructions.Last().MatchBranch(container.EntryPoint)) {
					// potentially the do-while-condition block
					int i = b.Instructions.Count - 2;
					while (i >= 0 && b.Instructions[i] is IfInstruction ifInst
						&& ifInst.TrueInst.MatchLeave(container) && ifInst.FalseInst.MatchNop()) {
						if (condition == null)
							condition = new LogicNot(ifInst.Condition);
						else
							condition = IfInstruction.LogicAnd(new LogicNot(ifInst.Condition), condition);
						i--;
					}
					if (i == -1) {
						return b;
					}
				}
			}
			return null;
		}

		private DetectedLoop DetectLoopInternal()
		{
			Kind = LoopKind.While;
			ContinueJumpTarget = Container.EntryPoint;
			if (Container.EntryPoint.Instructions.Count == 2
				&& Container.EntryPoint.Instructions[0].MatchIfInstruction(out var conditionInst, out var trueInst)
				&& Container.EntryPoint.Instructions[1].MatchLeave(Container)) {
				// detected while(condition)-loop or for-loop
				// we have to check if there's an increment block before converting the loop body using ConvertAsBlock(trueInst)
				// and set the continueJumpTarget to correctly convert 'br incrementBlock' instructions to continue; 
				IncrementBlock = null;
				if (Container.EntryPoint.IncomingEdgeCount == 2) {
					IncrementBlock = Container.Blocks.SingleOrDefault(
						b => b.Instructions.Last().MatchBranch(Container.EntryPoint)
							 && b.Instructions.SkipLast(1).All(IsSimpleStatement));
					if (IncrementBlock != null)
						ContinueJumpTarget = IncrementBlock;
				}
				Condition = conditionInst;
				Body = trueInst;
				if (IncrementBlock != null) {
					// for-loop
					Kind = LoopKind.For;
					AdditionalBlocks = Container.Blocks.Skip(1).Where(b => b != IncrementBlock).ToArray();
				} else {
					AdditionalBlocks = Container.Blocks.Skip(1).ToArray();
				}
			} else {
				// do-while or while(true)-loop
				if (Container.EntryPoint.IncomingEdgeCount == 2) {
					Block conditionBlock = FindDoWhileConditionBlock(Container, out var conditionInst2);
					if (conditionBlock != null) {
						Kind = LoopKind.DoWhile;
						ContinueJumpTarget = conditionBlock;
						Body = null;
						Condition = conditionInst2;
						AdditionalBlocks = Container.Blocks.Where(b => b != conditionBlock).ToArray();
					}
				}
			}
			return this;
		}
	}
}
