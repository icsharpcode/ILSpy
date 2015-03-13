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
		
		sealed class InliningStack : Stack<ILInstruction>, IInlineContext
		{
			/// <summary>
			/// Indicates whether inlining was success for at least one
			/// peek or pop instruction.
			/// </summary>
			internal bool didInline;
			
			/// <summary>
			/// Indicates whether inlining encountered a peek or pop instruction
			/// that could not be inlined.
			/// </summary>
			internal bool error;
			
			ILInstruction IInlineContext.Peek(InstructionFlags flagsBefore)
			{
				error = true;
				return null;
			}
			
			ILInstruction IInlineContext.Pop(InstructionFlags flagsBefore)
			{
				if (error)
					return null;
				if (base.Count > 0 && SemanticHelper.MayReorder(flagsBefore, base.Peek().Flags)) {
					didInline = true;
					return base.Pop();
				}
				error = true;
				return null;
			}
		}
		
		ILInstruction DoInline(InliningStack stack, ILInstruction inst)
		{
			do {
				inst = inst.AcceptVisitor(this);
				stack.didInline = false;
				stack.error = false;
				inst = inst.Inline(InstructionFlags.None, stack);
				// An error implies that a peek or pop instruction wasn't replaced
				// But even if we replaced all peek/pop instructions, we might have replaced them with
				// another peek or pop instruction, so MayPeek/MayPop might still be set after
				// we finish without error!
				Debug.Assert(!stack.error || inst.HasFlag(InstructionFlags.MayPeek | InstructionFlags.MayPop));
			} while (stack.didInline); // repeat transformations when something was inlined
			return inst;
		}
		
		protected internal override ILInstruction VisitBlock(Block block)
		{
			var stack = new InliningStack();
			List<ILInstruction> output = new List<ILInstruction>();
			for (int i = 0; i < block.Instructions.Count; i++) {
				var inst = block.Instructions[i];
				inst = DoInline(stack, inst);
				if (inst.HasFlag(InstructionFlags.MayBranch | InstructionFlags.MayPop
				                 | InstructionFlags.MayReadEvaluationStack | InstructionFlags.MayWriteEvaluationStack)) {
					// Values currently on the stack might be used on both sides of the branch,
					// so we can't inline them.
					// We also have to flush the stack if the instruction still accesses the evaluation stack,
					// no matter whether in phase-1 or phase-2.
					FlushInstructionStack(stack, output);
				} else if (inst.ResultType == StackType.Void && stack.Count > 0) {
					// For void instructions on non-empty stack, we can create a new inline block (or add to an existing one)
					// This works even when inst involves Peek.
					ILInstruction headInst = stack.Pop();
					Block inlineBlock = headInst as Block;
					if (inlineBlock == null || inlineBlock.FinalInstruction.OpCode != OpCode.Pop) {
						inlineBlock = new Block {
							Instructions = { headInst },
							ILRange = new Interval(headInst.ILRange.Start, headInst.ILRange.Start),
							FinalInstruction = new Pop(headInst.ResultType)
						};
					}
					inlineBlock.Instructions.Add(inst);
					inst = inlineBlock;
				}
				if (inst.HasFlag(InstructionFlags.MayPeek)) {
					// Prevent instruction from being inlined if it was peeked at.
					FlushInstructionStack(stack, output);
				}
				if (inst.ResultType == StackType.Void) {
					// We can't add void instructions to the stack, so flush the stack
					// and directly add the instruction to the output.
					FlushInstructionStack(stack, output);
					output.Add(inst);
				} else {
					// Instruction has a result, so we can push it on the stack normally
					stack.Push(inst);
				}
			}
			// Allow inlining into the final instruction
			if (block.FinalInstruction.OpCode == OpCode.Pop && stack.Count > 0 && IsInlineBlock(stack.Peek())) {
				// Don't inline an inline block into the final pop instruction:
				// doing so would result in infinite recursion.
			} else {
				// regular inlining into the final instruction
				block.FinalInstruction = DoInline(stack, block.FinalInstruction);
			}
			FlushInstructionStack(stack, output);
			block.Instructions.ReplaceList(output);
			if (!(block.Parent is BlockContainer)) {
				return TrySimplifyBlock(block);
			}
			return block;
		}

		bool IsInlineBlock(ILInstruction inst)
		{
			Block block = inst as Block;
			return block != null && block.FinalInstruction.OpCode == OpCode.Pop;
		}
		
		void FlushInstructionStack(Stack<ILInstruction> stack, List<ILInstruction> output)
		{
			foreach (var inst in stack.Reverse()) {
				AddToOutput(inst, output);
			}
			stack.Clear();
		}
		
		void AddToOutput(ILInstruction inst, List<ILInstruction> output)
		{
			// Unpack inline blocks that would become direct children of the parent block
			if (IsInlineBlock(inst)) {
				foreach (var nestedInst in ((Block)inst).Instructions) {
					AddToOutput(nestedInst, output);
				}
			} else {
				output.Add(inst);
			}
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
				return TrySimplifyBlock(container.EntryPoint);
			}
			return container;
		}
		
		/// <summary>
		/// If a block has only one instruction, replace it with that instruction.
		/// </summary>
		ILInstruction TrySimplifyBlock(Block block)
		{
			// If the block has only one instruction, we can remove the block too
			// (but only if this doesn't change the pop-order in the phase 1 evaluation of the parent block)
			if (block.Instructions.Count == 0) {
				if (!block.FinalInstruction.HasFlag(InstructionFlags.MayPeek | InstructionFlags.MayPop))
					return block.FinalInstruction;
			} else if (block.Instructions.Count == 1 && block.FinalInstruction.OpCode == OpCode.Nop) {
				if (block.Instructions[0].ResultType == StackType.Void && !block.Instructions[0].HasFlag(InstructionFlags.MayPeek | InstructionFlags.MayPop))
					return block.Instructions[0];
			}
			return block;
		}
	}
}
