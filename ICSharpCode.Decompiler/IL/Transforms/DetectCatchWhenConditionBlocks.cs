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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class DetectCatchWhenConditionBlocks : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var catchBlock in function.Descendants.OfType<TryCatchHandler>())
			{
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
					if (instructions.Count == 3)
					{
						// stloc temp(isinst exceptionType(ldloc exceptionVar))
						// if (comp(ldloc temp != ldnull)) br whenConditionBlock
						// br falseBlock
						context.Step($"Detected catch-when for {catchBlock.Variable.Name} (extra store)", instructions[0]);
						((StLoc)instructions[0]).Value = exceptionSlot;
						instructions[1].ReplaceWith(new Branch(whenConditionBlock));
						instructions.RemoveAt(2);
						container.SortBlocks(deleteUnreachableBlocks: true);
					}
					else if (instructions.Count == 2)
					{
						// if (comp(isinst exceptionType(ldloc exceptionVar) != ldnull)) br whenConditionBlock
						// br falseBlock
						context.Step($"Detected catch-when for {catchBlock.Variable.Name}", instructions[0]);
						instructions[0].ReplaceWith(new Branch(whenConditionBlock));
						instructions.RemoveAt(1);
						container.SortBlocks(deleteUnreachableBlocks: true);
					}

					PropagateExceptionVariable(context, catchBlock);
				}
			}
		}

		/// <summary>
		/// catch E_189 : 0200007C System.Exception when (BlockContainer {
		/// 	Block IL_0079 (incoming: 1) {
		/// 		stloc S_30(ldloc E_189)
		/// 		br IL_0085
		/// 	}
		/// 
		/// 	Block IL_0085 (incoming: 1) {
		/// 		stloc I_1(ldloc S_30)
		/// where S_30 and I_1 are single definition
		/// =>
		/// copy-propagate E_189 to replace all uses of S_30 and I_1
		/// </summary>
		static void PropagateExceptionVariable(ILTransformContext context, TryCatchHandler handler)
		{
			var exceptionVariable = handler.Variable;
			if (!exceptionVariable.IsSingleDefinition)
				return;
			context.StepStartGroup(nameof(PropagateExceptionVariable));
			int i = 0;
			while (i < exceptionVariable.LoadInstructions.Count)
			{
				var load = exceptionVariable.LoadInstructions[i];

				if (!load.IsDescendantOf(handler))
				{
					i++;
					continue;
				}

				// We are only interested in store "statements" copying the exception variable
				// without modifying it.
				var statement = LocalFunctionDecompiler.GetStatement(load);
				if (!(statement is StLoc stloc))
				{
					i++;
					continue;
				}
				// simple copy case:
				// stloc b(ldloc a)
				if (stloc.Value == load)
				{
					PropagateExceptionInstance(stloc);
				}
				// if the type of the cast-class instruction matches the exceptionType,
				// this cast can be removed without losing any side-effects.
				// Note: this would also hold true iff exceptionType were an exception derived
				// from cc.Type, however, we are more restrictive to match the pattern exactly.
				// stloc b(castclass exceptionType(ldloc a))
				else if (stloc.Value is CastClass cc && cc.Type.Equals(exceptionVariable.Type) && cc.Argument == load)
				{
					stloc.Value = load;
					PropagateExceptionInstance(stloc);
				}
				else
				{
					i++;
				}

				void PropagateExceptionInstance(StLoc store)
				{
					foreach (var load in store.Variable.LoadInstructions.ToArray())
					{
						if (!load.IsDescendantOf(handler))
							continue;
						load.ReplaceWith(new LdLoc(exceptionVariable).WithILRange(load));
					}
					if (store.Variable.LoadCount == 0 && store.Parent is Block block)
					{
						block.Instructions.RemoveAt(store.ChildIndex);
					}
					else
					{
						i++;
					}
				}
			}
			context.StepEndGroup(keepIfEmpty: false);
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
			if (entryPoint.Instructions.Count == 3)
			{
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
				if ((left.MatchLdNull() && right.MatchLdLoc(temp)) || (right.MatchLdNull() && left.MatchLdLoc(temp)))
				{
					return branch.MatchBranch(out whenConditionBlock);
				}
			}
			else if (entryPoint.Instructions.Count == 2)
			{
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
				if (right.MatchLdNull())
				{
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