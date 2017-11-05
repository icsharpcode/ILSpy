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
using System.Text;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// If possible, transforms plain ILAst loops into while (condition), do-while and for-loops.
	/// </summary>
	public class HighLevelLoopTransform : IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;

			foreach (var loop in function.Descendants.OfType<BlockContainer>()) {
				if (loop.Kind != ContainerKind.Loop)
					continue;
				if (MatchWhileLoop(loop, out var condition, out var loopBody)) {
					MatchForLoop(loop, condition, loopBody);
					continue;
				}
				if (MatchDoWhileLoop(loop))
					continue;
			}
		}

		bool MatchWhileLoop(BlockContainer loop, out ILInstruction condition, out Block loopBody)
		{
			// while-loop:
			// if (loop-condition) br loop-content-block
			// leave loop-container
			// -or-
			// if (loop-condition) block content-block
			// leave loop-container
			condition = null;
			loopBody = null;
			if (loop.EntryPoint.Instructions.Count != 2)
				return false;
			if (!(loop.EntryPoint.Instructions[0] is IfInstruction ifInstruction))
				return false;
			if (!ifInstruction.FalseInst.MatchNop())
				return false;
			condition = ifInstruction.Condition;
			var trueInst = ifInstruction.TrueInst;
			if (!loop.EntryPoint.Instructions[1].MatchLeave(loop))
				return false;
			if (trueInst is Block b) {
				context.Step("Transform to while (condition) loop", loop);
				loopBody = b;
				trueInst.ReplaceWith(new Branch(loopBody));
				loop.Blocks.Insert(1, loopBody);
				if (!loopBody.HasFlag(InstructionFlags.EndPointUnreachable))
					loopBody.Instructions.Add(new Leave(loop));
			} else if (trueInst is Branch br) {
				context.Step("Transform to while (condition) loop", loop);
				loopBody = br.TargetBlock;
			} else {
				return false;
			}
			ifInstruction.FalseInst = loop.EntryPoint.Instructions[1];
			loop.EntryPoint.Instructions.RemoveAt(1);
			loop.Kind = ContainerKind.While;
			return true;
		}

		bool MatchDoWhileLoop(BlockContainer loop)
		{
			if (loop.EntryPoint.Instructions.Count < 2)
				return false;
			var last = loop.EntryPoint.Instructions.Last();
			var ifInstruction = loop.EntryPoint.Instructions.SecondToLastOrDefault() as IfInstruction;
			if (ifInstruction == null || !ifInstruction.FalseInst.MatchNop())
				return false;
			bool swapBranches = false;
			ILInstruction condition = ifInstruction.Condition;
			while (condition.MatchLogicNot(out var arg)) {
				swapBranches = !swapBranches;
				condition = arg;
			}
			if (swapBranches) {
				if (!ifInstruction.TrueInst.MatchLeave(loop))
					return false;
				if (!last.MatchBranch(loop.EntryPoint))
					return false;
				context.Step("Transform to do-while loop", loop);
				ifInstruction.FalseInst = ifInstruction.TrueInst;
				ifInstruction.TrueInst = last;
				ifInstruction.Condition = condition;
			} else {
				if (!ifInstruction.TrueInst.MatchBranch(loop.EntryPoint))
					return false;
				if (!last.MatchLeave(loop))
					return false;
				context.Step("Transform to do-while loop", loop);
				ifInstruction.Condition = condition;
				ifInstruction.FalseInst = last;
			}
			Block conditionBlock = new Block();
			loop.Blocks.Add(conditionBlock);
			loop.EntryPoint.Instructions.RemoveRange(ifInstruction.ChildIndex, 2);
			conditionBlock.Instructions.Add(ifInstruction);
			conditionBlock.AddILRange(ifInstruction.ILRange);
			conditionBlock.AddILRange(last.ILRange);
			loop.EntryPoint.Instructions.Add(new Branch(conditionBlock));
			loop.Kind = ContainerKind.DoWhile;
			return false;
		}

		bool MatchForLoop(BlockContainer loop, ILInstruction condition, Block whileLoopBody)
		{
			if (loop.EntryPoint.IncomingEdgeCount != 2)
				return false;
			var incrementBlock = loop.Blocks.SingleOrDefault(
				b => b != whileLoopBody
					 && b.Instructions.Last().MatchBranch(loop.EntryPoint)
					 && b.Instructions.SkipLast(1).All(IsSimpleStatement));
			if (incrementBlock != null) {
				if (incrementBlock.Instructions.Count <= 1 || loop.Blocks.Count < 3)
					return false;
				context.Step("Transform to for loop", loop);
				loop.Blocks.MoveElementToEnd(incrementBlock);
				loop.Kind = ContainerKind.For;
			} else {
				var last = whileLoopBody.Instructions.LastOrDefault();
				var secondToLast = whileLoopBody.Instructions.SecondToLastOrDefault();
				if (last == null || secondToLast == null)
					return false;
				if (!last.MatchBranch(loop.EntryPoint))
					return false;
				if (!MatchIncrement(secondToLast, out var incrementVariable))
					return false;
				if (!condition.Descendants.Any(inst => inst.MatchLdLoc(incrementVariable)))
					return false;
				context.Step("Transform to for loop", loop);
				int secondToLastIndex = secondToLast.ChildIndex;
				var newIncremenBlock = new Block();
				loop.Blocks.Add(newIncremenBlock);
				newIncremenBlock.Instructions.Add(secondToLast);
				newIncremenBlock.Instructions.Add(last);
				newIncremenBlock.AddILRange(secondToLast.ILRange);
				whileLoopBody.Instructions.RemoveRange(secondToLastIndex, 2);
				whileLoopBody.Instructions.Add(new Branch(newIncremenBlock));
				loop.Kind = ContainerKind.For;
			}
			return true;
		}

		public static bool MatchIncrement(ILInstruction inst, out ILVariable variable)
		{
			if (!inst.MatchStLoc(out variable, out var value))
				return false;
			if (!value.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out var left, out var right)) {
				if (value is CompoundAssignmentInstruction cai) {
					left = cai.Target;
				} else return false;
			}
			return left.MatchLdLoc(variable);
		}

		/// <summary>
		/// Gets whether the statement is 'simple' (usable as for loop iterator):
		/// Currently we only accept calls and assignments.
		/// </summary>
		static bool IsSimpleStatement(ILInstruction inst)
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
	}
}
