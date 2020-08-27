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
using System.Text;

using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// If possible, transforms plain ILAst loops into while (condition), do-while and for-loops.
	/// For the invariants of the transforms <see cref="BlockContainer.CheckInvariant(ILPhase)"/>.
	/// </summary>
	public class HighLevelLoopTransform : IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;

			foreach (var loop in function.Descendants.OfType<BlockContainer>())
			{
				if (loop.Kind != ContainerKind.Loop)
					continue;
				if (MatchWhileLoop(loop, out var condition, out var loopBody))
				{
					if (context.Settings.ForStatement)
						MatchForLoop(loop, condition, loopBody);
					continue;
				}
				if (context.Settings.DoWhileStatement && MatchDoWhileLoop(loop))
					continue;
			}
		}

		bool MatchWhileLoop(BlockContainer loop, out IfInstruction condition, out Block loopBody)
		{
			// ConditionDetection favours leave inside if and branch at end of block
			// while-loop:
			// if (!loop-condition) leave loop-container
			// ...
			condition = null;
			loopBody = loop.EntryPoint;
			if (!(loopBody.Instructions[0] is IfInstruction ifInstruction))
				return false;

			if (!ifInstruction.FalseInst.MatchNop())
				return false;

			if (UsesVariableCapturedInLoop(loop, ifInstruction.Condition))
				return false;

			condition = ifInstruction;
			if (!ifInstruction.TrueInst.MatchLeave(loop))
			{
				// sometimes the loop-body is nested within the if
				// if (loop-condition) { loop-body }
				// leave loop-container

				if (loopBody.Instructions.Count != 2 || !loop.EntryPoint.Instructions.Last().MatchLeave(loop))
					return false;

				if (!ifInstruction.TrueInst.HasFlag(InstructionFlags.EndPointUnreachable))
					((Block)ifInstruction.TrueInst).Instructions.Add(new Leave(loop));

				ConditionDetection.InvertIf(loopBody, ifInstruction, context);
			}

			context.Step("Transform to while (condition) loop: " + loop.EntryPoint.Label, loop);
			loop.Kind = ContainerKind.While;
			//invert comparison
			ifInstruction.Condition = Comp.LogicNot(ifInstruction.Condition);
			ifInstruction.FalseInst = ifInstruction.TrueInst;
			//move the rest of the body into a new block
			loopBody = ConditionDetection.ExtractBlock(loop.EntryPoint, 1, loop.EntryPoint.Instructions.Count);
			loop.Blocks.Insert(1, loopBody);
			if (!loopBody.HasFlag(InstructionFlags.EndPointUnreachable))
				loopBody.Instructions.Add(new Leave(loop));

			ifInstruction.TrueInst = new Branch(loopBody);
			ExpressionTransforms.RunOnSingleStatement(ifInstruction, context);

			// Analyze conditions and decide whether to move some of them out of the condition block:
			/*var conditions = new List<ILInstruction>();
			SplitConditions(condition.Condition, conditions);
			// Break apart conditions that could be a MoveNext call followed by a Current accessor call:
			if (MightBeHeaderOfForEach(loop, conditions)) {
				ifInstruction.Condition = conditions[0];
				foreach (var cond in conditions.Skip(1).Reverse()) {
					IfInstruction inst;
					loopBody.Instructions.Insert(0, inst = new IfInstruction(Comp.LogicNot(cond), new Leave(loop)));
					ExpressionTransforms.RunOnSingleStatment(inst, context);
				}
			}*/

			return true;
		}

		bool MightBeHeaderOfForEach(BlockContainer loop, List<ILInstruction> conditions)
		{
			if (conditions.Count <= 1)
				return false;
			if (!(conditions[0] is CallInstruction moveNextCall && moveNextCall.Method.Name == "MoveNext"
				&& conditions[1].Descendants.Any(IsGetCurrentCall)))
				return false;
			return loop.Parent?.Parent?.Parent is UsingInstruction;

			bool IsGetCurrentCall(ILInstruction inst)
			{
				return inst is CallInstruction getterCall
					&& getterCall.Method.IsAccessor
					&& getterCall.Method.Name == "get_Current";
			}
		}

		void SplitConditions(ILInstruction expression, List<ILInstruction> conditions)
		{
			if (expression.MatchLogicAnd(out var l, out var r))
			{
				SplitConditions(l, conditions);
				SplitConditions(r, conditions);
			}
			else
			{
				conditions.Add(expression);
			}
		}

		/// <summary>
		/// Matches a do-while loop and performs the following transformations:
		/// - combine all compatible conditions into one IfInstruction.
		/// - extract conditions into a condition block, or move the existing condition block to the end.
		/// </summary>
		bool MatchDoWhileLoop(BlockContainer loop)
		{
			(List<IfInstruction> conditions, ILInstruction exit, bool swap, bool split, bool unwrap) = AnalyzeDoWhileConditions(loop);
			// not a do-while loop, exit.
			if (conditions == null || conditions.Count == 0)
				return false;
			context.Step("Transform to do-while loop: " + loop.EntryPoint.Label, loop);
			Block conditionBlock;
			// first we remove all extracted instructions from the original block.
			var originalBlock = (Block)exit.Parent;
			if (unwrap)
			{
				// we found a condition block nested in a condition that is followed by a return statement:
				// we flip the condition and swap the blocks
				Debug.Assert(originalBlock.Parent is IfInstruction);
				var returnCondition = (IfInstruction)originalBlock.Parent;
				var topLevelBlock = (Block)returnCondition.Parent;
				Debug.Assert(topLevelBlock.Parent == loop);
				var leaveFunction = topLevelBlock.Instructions[returnCondition.ChildIndex + 1];
				Debug.Assert(leaveFunction.MatchReturn(out _));
				returnCondition.Condition = Comp.LogicNot(returnCondition.Condition);
				returnCondition.TrueInst = leaveFunction;
				// simplify the condition:
				ExpressionTransforms.RunOnSingleStatement(returnCondition, context);
				topLevelBlock.Instructions.RemoveAt(returnCondition.ChildIndex + 1);
				topLevelBlock.Instructions.AddRange(originalBlock.Instructions);
				originalBlock = topLevelBlock;
				split = true;
			}
			originalBlock.Instructions.RemoveRange(originalBlock.Instructions.Count - conditions.Count - 1, conditions.Count + 1);
			// we need to split the block:
			if (split)
			{
				// add a new block at the end and add a branch to the new block.
				conditionBlock = new Block();
				loop.Blocks.Add(conditionBlock);
				originalBlock.Instructions.Add(new Branch(conditionBlock));
			}
			else
			{
				// move the condition block to the end.
				conditionBlock = originalBlock;
				loop.Blocks.MoveElementToEnd(originalBlock);
			}
			// combine all conditions and the exit instruction into one IfInstruction:
			IfInstruction condition = null;
			conditionBlock.AddILRange(exit);
			foreach (var inst in conditions)
			{
				conditionBlock.AddILRange(inst);
				if (condition == null)
				{
					condition = inst;
					if (swap)
					{
						// branches must be swapped and condition negated:
						condition.Condition = Comp.LogicNot(condition.Condition);
						condition.FalseInst = condition.TrueInst;
						condition.TrueInst = exit;
					}
					else
					{
						condition.FalseInst = exit;
					}
				}
				else
				{
					if (swap)
					{
						condition.Condition = IfInstruction.LogicAnd(Comp.LogicNot(inst.Condition), condition.Condition);
					}
					else
					{
						condition.Condition = IfInstruction.LogicAnd(inst.Condition, condition.Condition);
					}
				}
			}
			// insert the combined conditions into the condition block:
			conditionBlock.Instructions.Add(condition);
			// simplify the condition:
			ExpressionTransforms.RunOnSingleStatement(condition, context);
			// transform complete
			loop.Kind = ContainerKind.DoWhile;
			return true;
		}

		static (List<IfInstruction> conditions, ILInstruction exit, bool swap, bool split, bool unwrap) AnalyzeDoWhileConditions(BlockContainer loop)
		{
			// we iterate over all blocks from the bottom, because the entry-point
			// should only be considered as condition block, if there are no other blocks.
			foreach (var block in loop.Blocks.Reverse())
			{
				// first we match the end of the block:
				if (MatchDoWhileConditionBlock(loop, block, out bool swap, out bool unwrapCondtionBlock, out Block conditionBlock))
				{
					// now collect all instructions that are usable as loop conditions
					var conditions = CollectConditions(loop, conditionBlock, swap);
					// split only if the block is either the entry-point or contains other instructions as well.
					var split = conditionBlock == loop.EntryPoint || conditionBlock.Instructions.Count > conditions.Count + 1; // + 1 is the final leave/branch.
					return (conditions, conditionBlock.Instructions.Last(), swap, split, unwrapCondtionBlock);
				}
			}
			return (null, null, false, false, false);
		}

		/// <summary>
		/// Returns a list of all IfInstructions that can be used as loop conditon, i.e.,
		/// that have no false-instruction and have leave loop (if swapped) or branch entry-point as true-instruction.
		/// </summary>
		static List<IfInstruction> CollectConditions(BlockContainer loop, Block block, bool swap)
		{
			var list = new List<IfInstruction>();
			int i = block.Instructions.Count - 2;
			while (i >= 0 && block.Instructions[i] is IfInstruction ifInst)
			{
				if (!ifInst.FalseInst.MatchNop())
					break;
				if (UsesVariableCapturedInLoop(loop, ifInst.Condition))
					break;
				if (swap)
				{
					if (!ifInst.TrueInst.MatchLeave(loop))
						break;
					list.Add(ifInst);
				}
				else
				{
					if (!ifInst.TrueInst.MatchBranch(loop.EntryPoint))
						break;
					list.Add(ifInst);
				}
				i--;
			}

			return list;
		}

		static bool UsesVariableCapturedInLoop(BlockContainer loop, ILInstruction condition)
		{
			foreach (var inst in condition.Descendants.OfType<IInstructionWithVariableOperand>())
			{
				if (inst.Variable.CaptureScope == loop)
					return true;
			}
			return false;
		}

		static bool MatchDoWhileConditionBlock(BlockContainer loop, Block block, out bool swapBranches, out bool unwrapCondtionBlock, out Block conditionBlock)
		{
			// match the end of the block:
			// if (condition) branch entry-point else nop
			// leave loop
			// -or-
			// if (condition) leave loop else nop
			// branch entry-point
			swapBranches = false;
			unwrapCondtionBlock = false;
			conditionBlock = block;
			// empty block?
			if (block.Instructions.Count < 2)
				return false;
			var last = block.Instructions.Last();
			var ifInstruction = block.Instructions.SecondToLastOrDefault() as IfInstruction;
			// no IfInstruction or already transformed?
			if (ifInstruction == null || !ifInstruction.FalseInst.MatchNop())
				return false;
			// the block ends in a return statement preceeded by an IfInstruction
			// take a look at the nested block and check if that might be a condition block
			if (last.MatchReturn(out _) && ifInstruction.TrueInst is Block nestedConditionBlock)
			{
				if (nestedConditionBlock.Instructions.Count < 2)
					return false;
				last = nestedConditionBlock.Instructions.Last();
				ifInstruction = nestedConditionBlock.Instructions.SecondToLastOrDefault() as IfInstruction;
				if (ifInstruction == null || !ifInstruction.FalseInst.MatchNop())
					return false;
				unwrapCondtionBlock = true;
				conditionBlock = nestedConditionBlock;
			}
			// if the last instruction is a branch
			// we assume the branch instructions need to be swapped.
			if (last.MatchBranch(loop.EntryPoint))
				swapBranches = true;
			else if (last.MatchLeave(loop))
				swapBranches = false;
			else
				return false;
			// match the IfInstruction
			if (swapBranches)
			{
				if (!ifInstruction.TrueInst.MatchLeave(loop))
					return false;
			}
			else
			{
				if (!ifInstruction.TrueInst.MatchBranch(loop.EntryPoint))
					return false;
			}
			return true;
		}

		// early match before block containers have been constructed
		internal static bool MatchDoWhileConditionBlock(Block block, out Block target1, out Block target2)
		{
			target1 = target2 = null;
			if (block.Instructions.Count < 2)
				return false;

			var last = block.Instructions.Last();
			if (!(block.Instructions.SecondToLastOrDefault() is IfInstruction ifInstruction) || !ifInstruction.FalseInst.MatchNop())
				return false;

			return (ifInstruction.TrueInst.MatchBranch(out target1) || ifInstruction.TrueInst.MatchReturn(out var _)) &&
				   (last.MatchBranch(out target2) || last.MatchReturn(out var _));
		}

		internal static Block GetIncrementBlock(BlockContainer loop, Block whileLoopBody) =>
			loop.Blocks.SingleOrDefault(b => b != whileLoopBody
										  && b.Instructions.Last().MatchBranch(loop.EntryPoint)
										  && b.Instructions.SkipLast(1).All(IsSimpleStatement));

		internal static bool MatchIncrementBlock(Block block, out Block loopHead) =>
			block.Instructions.Last().MatchBranch(out loopHead)
			&& block.Instructions.SkipLast(1).All(IsSimpleStatement);

		bool MatchForLoop(BlockContainer loop, IfInstruction whileCondition, Block whileLoopBody)
		{
			// for loops have exactly two incoming edges at the entry point.
			if (loop.EntryPoint.IncomingEdgeCount != 2)
				return false;
			// try to find an increment block:
			// consists of simple statements only.
			var incrementBlock = GetIncrementBlock(loop, whileLoopBody);
			if (incrementBlock != null)
			{
				// we found a possible increment block, just make sure, that there are at least three blocks:
				// - condition block
				// - loop body
				// - increment block
				if (incrementBlock.Instructions.Count <= 1 || loop.Blocks.Count < 3)
					return false;
				context.Step("Transform to for loop: " + loop.EntryPoint.Label, loop);
				// move the block to the end of the loop:
				loop.Blocks.MoveElementToEnd(incrementBlock);
				loop.Kind = ContainerKind.For;
			}
			else
			{
				// we need to move the increment statements into its own block:
				// last must be a branch entry-point
				var last = whileLoopBody.Instructions.LastOrDefault();
				var secondToLast = whileLoopBody.Instructions.SecondToLastOrDefault();
				if (last == null || secondToLast == null)
					return false;
				if (!last.MatchBranch(loop.EntryPoint))
					return false;
				// we only deal with 'numeric' increments
				if (!MatchIncrement(secondToLast, out var incrementVariable))
					return false;
				// the increment variable must be local/stack variable
				if (incrementVariable.Kind == VariableKind.Parameter)
					return false;
				// split conditions:
				var conditions = new List<ILInstruction>();
				SplitConditions(whileCondition.Condition, conditions);
				IfInstruction forCondition = null;
				int numberOfConditions = 0;
				foreach (var condition in conditions)
				{
					// the increment variable must be used in the condition
					if (!condition.Descendants.Any(inst => inst.MatchLdLoc(incrementVariable)))
						break;
					// condition should not contain an assignment
					if (condition.Descendants.Any(IsAssignment))
						break;
					if (forCondition == null)
					{
						forCondition = new IfInstruction(condition, whileCondition.TrueInst, whileCondition.FalseInst);
					}
					else
					{
						forCondition.Condition = IfInstruction.LogicAnd(forCondition.Condition, condition);
					}
					numberOfConditions++;
				}
				if (numberOfConditions == 0)
					return false;
				context.Step("Transform to for loop: " + loop.EntryPoint.Label, loop);
				// split condition block:
				whileCondition.ReplaceWith(forCondition);
				ExpressionTransforms.RunOnSingleStatement(forCondition, context);
				for (int i = conditions.Count - 1; i >= numberOfConditions; i--)
				{
					IfInstruction inst;
					whileLoopBody.Instructions.Insert(0, inst = new IfInstruction(Comp.LogicNot(conditions[i]), new Leave(loop)));
					ExpressionTransforms.RunOnSingleStatement(inst, context);
				}
				// create a new increment block and add it at the end:
				int secondToLastIndex = secondToLast.ChildIndex;
				var newIncremenBlock = new Block();
				loop.Blocks.Add(newIncremenBlock);
				// move the increment instruction:
				newIncremenBlock.Instructions.Add(secondToLast);
				newIncremenBlock.Instructions.Add(last);
				newIncremenBlock.AddILRange(secondToLast);
				whileLoopBody.Instructions.RemoveRange(secondToLastIndex, 2);
				whileLoopBody.Instructions.Add(new Branch(newIncremenBlock));
				// complete transform.
				loop.Kind = ContainerKind.For;
			}
			return true;
		}

		bool IsAssignment(ILInstruction inst)
		{
			if (inst is StLoc)
				return true;
			if (inst is CompoundAssignmentInstruction)
				return true;
			return false;
		}

		/// <summary>
		/// Returns true if the instruction is stloc v(add(ldloc v, arg))
		/// or compound.assign(ldloca v, arg)
		/// </summary>
		public static bool MatchIncrement(ILInstruction inst, out ILVariable variable)
		{
			if (inst.MatchStLoc(out variable, out var value))
			{
				if (value.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out var left, out var right))
				{
					return left.MatchLdLoc(variable);
				}
			}
			else if (inst is CompoundAssignmentInstruction cai)
			{
				return cai.TargetKind == CompoundTargetKind.Address && cai.Target.MatchLdLoca(out variable);
			}
			return false;
		}

		/// <summary>
		/// Gets whether the statement is 'simple' (usable as for loop iterator):
		/// Currently we only accept calls and assignments.
		/// </summary>
		static bool IsSimpleStatement(ILInstruction inst)
		{
			switch (inst.OpCode)
			{
				case OpCode.Call:
				case OpCode.CallVirt:
				case OpCode.NewObj:
				case OpCode.StLoc:
				case OpCode.StObj:
				case OpCode.NumericCompoundAssign:
				case OpCode.UserDefinedCompoundAssign:
					return true;
				default:
					return false;
			}
		}
	}
}
