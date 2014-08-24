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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Visitor that applies a list of transformations to the IL Ast.
	/// </summary>
	/// <remarks>
	/// The base class performs:
	/// - variable inlining
	/// - cleanup after branch inlining
	/// - removal of unnecessary blocks and block containers
	/// </remarks>
	public class TransformingVisitor : ILVisitor<ILInstruction>
	{
		protected override ILInstruction Default(ILInstruction inst)
		{
			inst.TransformChildren(this);
			return inst;
		}
		
		/*
		protected internal override ILInstruction VisitBranch(Branch inst)
		{
			// If this branch is the only edge to the target block, we can inline the target block here:
			if (inst.TargetBlock.IncomingEdgeCount == 1 && inst.PopCount == 0) {
				return inst.TargetBlock;
			}
			return base.VisitBranch(inst);
		}
		 */
		
		protected bool removeNops;
		
		protected internal override ILInstruction VisitBlock(Block block)
		{
			Stack<ILInstruction> stack = new Stack<ILInstruction>();
			List<ILInstruction> output = new List<ILInstruction>();
			for (int i = 0; i < block.Instructions.Count; i++) {
				var inst = block.Instructions[i];
				int stackCountBefore;
				bool finished;
				do {
					inst = inst.AcceptVisitor(this);
					stackCountBefore = stack.Count;
					inst = inst.Inline(InstructionFlags.None, stack, out finished);
				} while (stack.Count != stackCountBefore); // repeat transformations when something was inlined
				if (inst.HasFlag(InstructionFlags.MayBranch)) {
					// Values currently on the stack might be used on both sides of the branch,
					// so we can't inline them.
					FlushInstructionStack(stack, output);
				}
				if (inst.ResultType == StackType.Void) {
					// We cannot directly push instructions onto the stack if they don't produce
					// a result.
					if (finished && stack.Count > 0) {
						// Wrap the instruction on top of the stack into an inline block,
						// and append our void-typed instruction to the end of that block.
						var headInst = stack.Pop();
						var nestedBlock = headInst as Block ?? new Block {
							Instructions = { headInst },
							ILRange = headInst.ILRange
						};
						nestedBlock.Instructions.Add(inst);
						stack.Push(nestedBlock);
					} else {
						// We can't move incomplete instructions into a nested block
						// or the instruction stack was empty
						FlushInstructionStack(stack, output);
						output.Add(inst);
					}
				} else {
					// Instruction has a result, so we can push it on the stack normally
					stack.Push(inst);
				}
			}
			FlushInstructionStack(stack, output);
			if (!(block.Parent is BlockContainer)) {
				if (output.Count == 1)
					return output[0];
			}
			block.Instructions.ReplaceList(output);
			return block;
		}

		void FlushInstructionStack(Stack<ILInstruction> stack, List<ILInstruction> output)
		{
			output.AddRange(stack.Reverse());
			stack.Clear();
		}
		
		protected internal override ILInstruction VisitBlockContainer(BlockContainer container)
		{
			container.TransformChildren(this);
			
			// VisitBranch() 'steals' blocks from containers. Remove all blocks that were stolen from the block list:
			Debug.Assert(container.EntryPoint.IncomingEdgeCount > 0);
			container.Blocks.RemoveAll(b => b.IncomingEdgeCount == 0);
			
			// If the container only contains a single block, and the block contents do not jump back to the block start,
			// we can remove the container.
			if (container.Blocks.Count == 1 && container.EntryPoint.IncomingEdgeCount == 1) {
				// If the block has only one instruction, we can remove the block too
				if (container.EntryPoint.Instructions.Count == 1)
					return container.EntryPoint.Instructions[0];
				return container.EntryPoint;
			}
			return container;
		}
	}
}
