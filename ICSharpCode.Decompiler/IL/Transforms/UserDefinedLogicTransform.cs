// Copyright (c) 2018 Daniel Grunwald
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
using System.Text;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class UserDefinedLogicTransform : IStatementTransform
	{
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			if (LegacyPattern(block, pos, context))
				return;
			if (RoslynOptimized(block, pos, context))
				return;
		}

		bool RoslynOptimized(Block block, int pos, StatementTransformContext context)
		{
			// Roslyn, optimized pattern in combination with return statement:
			//   if (logic.not(call op_False(ldloc lhsVar))) leave IL_0000 (call op_BitwiseAnd(ldloc lhsVar, rhsInst))
			//   leave IL_0000(ldloc lhsVar)
			// ->
			//   user.logic op_BitwiseAnd(ldloc lhsVar, rhsInst)
			if (!block.Instructions[pos].MatchIfInstructionPositiveCondition(out var condition, out var trueInst, out var falseInst))
				return false;
			if (trueInst.OpCode == OpCode.Nop) {
				trueInst = block.Instructions[pos + 1];
			} else if (falseInst.OpCode == OpCode.Nop) {
				falseInst = block.Instructions[pos + 1];
			} else {
				return false;
			}
			if (trueInst.MatchReturn(out var trueValue) && falseInst.MatchReturn(out var falseValue)) {
				var transformed = Transform(condition, trueValue, falseValue);
				if (transformed != null) {
					context.Step("User-defined short-circuiting logic operator (optimized return)", condition);
					((Leave)block.Instructions[pos + 1]).Value = transformed;
					block.Instructions.RemoveAt(pos);
					return true;
				}
			}
			return false;
		}

		bool LegacyPattern(Block block, int pos, StatementTransformContext context)
		{
			// Legacy csc pattern:
			//   stloc s(lhsInst)
			//   if (logic.not(call op_False(ldloc s))) Block {
			//     stloc s(call op_BitwiseAnd(ldloc s, rhsInst))
			//   }
			// ->
			//   stloc s(user.logic op_BitwiseAnd(lhsInst, rhsInst))
			if (!block.Instructions[pos].MatchStLoc(out var s, out var lhsInst))
				return false;
			if (!(s.Kind == VariableKind.StackSlot))
				return false;
			if (!(block.Instructions[pos + 1] is IfInstruction ifInst))
				return false;
			if (!ifInst.Condition.MatchLogicNot(out var condition))
				return false;
			if (!(MatchCondition(condition, out var s2, out string conditionMethodName) && s2 == s))
				return false;
			if (ifInst.FalseInst.OpCode != OpCode.Nop)
				return false;
			var trueInst = Block.Unwrap(ifInst.TrueInst);
			if (!trueInst.MatchStLoc(s, out var storeValue))
				return false;
			if (storeValue is Call call) {
				if (!MatchBitwiseCall(call, s, conditionMethodName))
					return false;
				if (s.IsUsedWithin(call.Arguments[1]))
					return false;
				context.Step("User-defined short-circuiting logic operator (legacy pattern)", condition);
				((StLoc)block.Instructions[pos]).Value = new UserDefinedLogicOperator(call.Method, lhsInst, call.Arguments[1]) {
					ILRange = call.ILRange
				};
				block.Instructions.RemoveAt(pos + 1);
				context.RequestRerun(); // the 'stloc s' may now be eligible for inlining
				return true;
			}
			return false;
		}

		static bool MatchCondition(ILInstruction condition, out ILVariable v, out string name)
		{
			v = null;
			name = null;
			if (!(condition is Call call && call.Method.IsOperator && call.Arguments.Count == 1 && !call.IsLifted))
				return false;
			name = call.Method.Name;
			if (!(name == "op_True" || name == "op_False"))
				return false;
			return call.Arguments[0].MatchLdLoc(out v);
		}

		static bool MatchBitwiseCall(Call call, ILVariable v, string conditionMethodName)
		{
			if (!(call != null && call.Method.IsOperator && call.Arguments.Count == 2 && !call.IsLifted))
				return false;
			if (!call.Arguments[0].MatchLdLoc(v))
				return false;

			return conditionMethodName == "op_False" && call.Method.Name == "op_BitwiseAnd"
				|| conditionMethodName == "op_True" && call.Method.Name == "op_BitwiseOr";
		}

		/// <summary>
		///       if (call op_False(ldloc lhsVar)) ldloc lhsVar else call op_BitwiseAnd(ldloc lhsVar, rhsInst)
		///    -> user.logic op_BitwiseAnd(ldloc lhsVar, rhsInst)
		/// or
		///       if (call op_True(ldloc lhsVar)) ldloc lhsVar else call op_BitwiseOr(ldloc lhsVar, rhsInst)
		///    -> user.logic op_BitwiseOr(ldloc lhsVar, rhsInst)
		/// </summary>
		public static ILInstruction Transform(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst)
		{
			if (!MatchCondition(condition, out var lhsVar, out var conditionMethodName))
				return null;
			if (!trueInst.MatchLdLoc(lhsVar))
				return null;
			var call = falseInst as Call;
			if (!MatchBitwiseCall(call, lhsVar, conditionMethodName))
				return null;

			var result = new UserDefinedLogicOperator(call.Method, call.Arguments[0], call.Arguments[1]);
			result.AddILRange(condition.ILRange);
			result.AddILRange(trueInst.ILRange);
			result.AddILRange(call.ILRange);
			return result;
		}
	}
}
