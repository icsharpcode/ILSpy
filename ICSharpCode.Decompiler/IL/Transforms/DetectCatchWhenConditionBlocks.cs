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
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class DetectCatchWhenConditionBlocks : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var catchBlock in function.Descendants.OfType<TryCatchHandler>()) {
				if (catchBlock.Filter is BlockContainer container
					&& MatchCatchWhenEntryPoint(catchBlock.Variable, container, container.EntryPoint,
						out var exceptionType, out var exceptionSlot, out var whenConditionBlock)
					&& exceptionType.GetStackType() == catchBlock.Variable.StackType)
				{
					// set exceptionType
					catchBlock.Variable.Type = exceptionType;
					// Block entryPoint (incoming: 1)  {
					//   stloc temp(isinst exceptionType(ldloc exceptionVar))
					//   if (comp(ldloc temp != ldnull)) br whenConditionBlock
					//   br falseBlock
					// }
					// =>
					// Block entryPoint (incoming: 1)  {
					//   stloc temp(ldloc exceptionSlot)  
					//   br whenConditionBlock
					// }
					var instructions = container.EntryPoint.Instructions;
					if (instructions.Count == 3) {
						// stloc temp(isinst exceptionType(ldloc exceptionVar))
						// if (comp(ldloc temp != ldnull)) br whenConditionBlock
						// br falseBlock
						((StLoc)instructions[0]).Value = exceptionSlot;
						instructions[1].ReplaceWith(new Branch(whenConditionBlock));
						instructions.RemoveAt(2);
						container.SortBlocks(deleteUnreachableBlocks: true);
					} else if (instructions.Count == 2) {
						// if (comp(isinst exceptionType(ldloc exceptionVar) != ldnull)) br whenConditionBlock
						// br falseBlock
						instructions[0].ReplaceWith(new Branch(whenConditionBlock));
						instructions.RemoveAt(1);
						container.SortBlocks(deleteUnreachableBlocks: true);
					}
				}
			}
		}

		/// <summary>
		/// Block entryPoint (incoming: 1)  {
		///   stloc temp(isinst exceptionType(ldloc exceptionVar))
		///   if (comp(ldloc temp != ldnull)) br whenConditionBlock
		///   br falseBlock
		/// }
		/// </summary>
		bool MatchCatchWhenEntryPoint(ILVariable exceptionVar, BlockContainer container, Block entryPoint, out IType exceptionType, out ILInstruction exceptionSlot, out Block whenConditionBlock)
		{
			exceptionType = null;
			exceptionSlot = null;
			whenConditionBlock = null;
			if (entryPoint == null || entryPoint.IncomingEdgeCount != 1)
				return false;
			if (entryPoint.Instructions.Count == 3) {
				// stloc temp(isinst exceptionType(ldloc exceptionVar))
				// if (comp(ldloc temp != ldnull)) br whenConditionBlock
				// br falseBlock
				if (!entryPoint.Instructions[0].MatchStLoc(out var temp, out var isinst) ||
					temp.Kind != VariableKind.StackSlot || !isinst.MatchIsInst(out exceptionSlot, out exceptionType))
					return false;
				if (!exceptionSlot.MatchLdLoc(exceptionVar))
					return false;
				if (!entryPoint.Instructions[1].MatchIfInstruction(out var condition, out var branch))
					return false;
				if (!condition.MatchCompNotEquals(out var left, out var right))
					return false;
				if (!entryPoint.Instructions[2].MatchBranch(out var falseBlock) || !MatchFalseBlock(container, falseBlock, out var returnVar, out var exitBlock))
					return false;
				if ((left.MatchLdNull() && right.MatchLdLoc(temp)) || (right.MatchLdNull() && left.MatchLdLoc(temp))) {
					return branch.MatchBranch(out whenConditionBlock);
				}
			} else if (entryPoint.Instructions.Count == 2) {
				// if (comp(isinst exceptionType(ldloc exceptionVar) != ldnull)) br whenConditionBlock
				// br falseBlock
				if (!entryPoint.Instructions[0].MatchIfInstruction(out var condition, out var branch))
					return false;
				if (!condition.MatchCompNotEquals(out var left, out var right))
					return false;
				if (!entryPoint.Instructions[1].MatchBranch(out var falseBlock) || !MatchFalseBlock(container, falseBlock, out var returnVar, out var exitBlock))
					return false;
				if (!left.MatchIsInst(out exceptionSlot, out exceptionType))
					return false;
				if (!exceptionSlot.MatchLdLoc(exceptionVar))
					return false;
				if (right.MatchLdNull()) {
					return branch.MatchBranch(out whenConditionBlock);
				}
			}
			return false;
		}

		/// <summary>
		/// Block falseBlock (incoming: 1)  {
		///   stloc returnVar(ldc.i4 0)
		///   br exitBlock
		/// }
		/// </summary>
		bool MatchFalseBlock(BlockContainer container, Block falseBlock, out ILVariable returnVar, out Block exitBlock)
		{
			returnVar = null;
			exitBlock = null;
			if (falseBlock.IncomingEdgeCount != 1 || falseBlock.Instructions.Count != 2)
				return false;
			return falseBlock.Instructions[0].MatchStLoc(out returnVar, out var zero) &&
				zero.MatchLdcI4(0) && falseBlock.Instructions[1].MatchBranch(out exitBlock) &&
				MatchExitBlock(container, exitBlock, returnVar);
		}

		/// <summary>
		/// Block exitBlock(incoming: 2) {
		///   leave container(ldloc returnVar)
		/// }
		/// </summary>
		bool MatchExitBlock(BlockContainer container, Block exitBlock, ILVariable returnVar)
		{
			if (exitBlock.IncomingEdgeCount != 2 || exitBlock.Instructions.Count != 1)
				return false;
			return exitBlock.Instructions[0].MatchLeave(container, out var value) &&
				value.MatchLdLoc(returnVar);
		}
	}
}