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
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class UserDefinedLogicTransform : IStatementTransform
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
			if (trueInst.OpCode == OpCode.Nop)
			{
				trueInst = block.Instructions[pos + 1];
			}
			else if (falseInst.OpCode == OpCode.Nop)
			{
				falseInst = block.Instructions[pos + 1];
			}
			else
			{
				return false;
			}
			if (trueInst.MatchReturn(out var trueValue) && falseInst.MatchReturn(out var falseValue))
			{
				var transformed = Transform(condition, trueValue, falseValue);
				if (transformed == null)
				{
					transformed = TransformDynamic(condition, trueValue, falseValue);
				}
				if (transformed != null)
				{
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
			if (storeValue is Call call)
			{
				if (!MatchBitwiseCall(call, s, conditionMethodName))
					return false;
				if (s.IsUsedWithin(call.Arguments[1]))
					return false;
				context.Step("User-defined short-circuiting logic operator (legacy pattern)", condition);
				((StLoc)block.Instructions[pos]).Value = new UserDefinedLogicOperator(call.Method, lhsInst, call.Arguments[1])
					.WithILRange(call);
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
			result.AddILRange(condition);
			result.AddILRange(trueInst);
			result.AddILRange(call);
			return result;
		}

		public static ILInstruction TransformDynamic(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst)
		{
			// Check condition:
			System.Linq.Expressions.ExpressionType unaryOp;
			if (condition.MatchLdLoc(out var lhsVar))
			{
				// if (ldloc lhsVar) box bool(ldloc lhsVar) else dynamic.binary.operator.logic Or(ldloc lhsVar, rhsInst)
				//    -> dynamic.logic.operator OrElse(ldloc lhsVar, rhsInst)
				if (trueInst is Box box && box.Type.IsKnownType(KnownTypeCode.Boolean))
				{
					unaryOp = System.Linq.Expressions.ExpressionType.IsTrue;
					trueInst = box.Argument;
				}
				else if (falseInst is Box box2 && box2.Type.IsKnownType(KnownTypeCode.Boolean))
				{
					// negate condition and swap true/false
					unaryOp = System.Linq.Expressions.ExpressionType.IsFalse;
					falseInst = trueInst;
					trueInst = box2.Argument;
				}
				else
				{
					return null;
				}
			}
			else if (condition is DynamicUnaryOperatorInstruction unary)
			{
				// if (dynamic.unary.operator IsFalse(ldloc lhsVar)) ldloc lhsVar else dynamic.binary.operator.logic And(ldloc lhsVar, rhsInst)
				//    -> dynamic.logic.operator AndAlso(ldloc lhsVar, rhsInst)
				unaryOp = unary.Operation;
				if (!unary.Operand.MatchLdLoc(out lhsVar))
					return null;
			}
			else if (MatchCondition(condition, out lhsVar, out string operatorMethodName))
			{
				// if (call op_False(ldloc s)) box S(ldloc s) else dynamic.binary.operator.logic And(ldloc s, rhsInst))
				if (operatorMethodName == "op_True")
				{
					unaryOp = System.Linq.Expressions.ExpressionType.IsTrue;
				}
				else
				{
					Debug.Assert(operatorMethodName == "op_False");
					unaryOp = System.Linq.Expressions.ExpressionType.IsFalse;
				}
				var callParamType = ((Call)condition).Method.Parameters.Single().Type.SkipModifiers();
				if (callParamType.IsReferenceType == false)
				{
					// If lhs is a value type, eliminate the boxing instruction.
					if (trueInst is Box box && NormalizeTypeVisitor.TypeErasure.EquivalentTypes(box.Type, callParamType))
					{
						trueInst = box.Argument;
					}
					else if (trueInst.OpCode == OpCode.LdcI4)
					{
						// special case, handled below in 'check trueInst'
					}
					else
					{
						return null;
					}
				}
			}
			else
			{
				return null;
			}

			// Check trueInst:
			DynamicUnaryOperatorInstruction rhsUnary;
			if (trueInst.MatchLdLoc(lhsVar))
			{
				// OK, typical pattern where the expression evaluates to 'dynamic'
				rhsUnary = null;
			}
			else if (trueInst.MatchLdcI4(1) && unaryOp == System.Linq.Expressions.ExpressionType.IsTrue)
			{
				// logic.or(IsTrue(lhsVar), IsTrue(lhsVar | rhsInst))
				// => IsTrue(lhsVar || rhsInst)
				rhsUnary = falseInst as DynamicUnaryOperatorInstruction;
				if (rhsUnary != null)
				{
					if (rhsUnary.Operation != System.Linq.Expressions.ExpressionType.IsTrue)
						return null;
					falseInst = rhsUnary.Operand;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}

			System.Linq.Expressions.ExpressionType expectedBitop;
			System.Linq.Expressions.ExpressionType logicOp;
			if (unaryOp == System.Linq.Expressions.ExpressionType.IsFalse)
			{
				expectedBitop = System.Linq.Expressions.ExpressionType.And;
				logicOp = System.Linq.Expressions.ExpressionType.AndAlso;
			}
			else if (unaryOp == System.Linq.Expressions.ExpressionType.IsTrue)
			{
				expectedBitop = System.Linq.Expressions.ExpressionType.Or;
				logicOp = System.Linq.Expressions.ExpressionType.OrElse;
			}
			else
			{
				return null;
			}

			// Check falseInst:
			if (!(falseInst is DynamicBinaryOperatorInstruction binary))
				return null;
			if (binary.Operation != expectedBitop)
				return null;
			if (!binary.Left.MatchLdLoc(lhsVar))
				return null;
			var logicInst = new DynamicLogicOperatorInstruction(binary.BinderFlags, logicOp, binary.CallingContext,
				binary.LeftArgumentInfo, binary.Left, binary.RightArgumentInfo, binary.Right)
				.WithILRange(binary);
			if (rhsUnary != null)
			{
				rhsUnary.Operand = logicInst;
				return rhsUnary;
			}
			else
			{
				return logicInst;
			}
		}
	}
}
