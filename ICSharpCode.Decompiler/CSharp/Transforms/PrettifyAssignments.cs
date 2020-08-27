// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Simplifies "x = x op y" into "x op= y" where possible.
	/// </summary>
	/// <remarks>
	/// Because the two "x" in "x = x op y" may refer to different ILVariables,
	/// this transform must run after DeclareVariables.
	/// 
	/// It must also run after ReplaceMethodCallsWithOperators (so that it can work for custom operator, too);
	/// and after AddCheckedBlocks (because "for (;; x = unchecked(x op y))" cannot be transformed into "x += y").
	/// </remarks>
	class PrettifyAssignments : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context;

		public override void VisitAssignmentExpression(AssignmentExpression assignment)
		{
			base.VisitAssignmentExpression(assignment);
			// Combine "x = x op y" into "x op= y"
			BinaryOperatorExpression binary = assignment.Right as BinaryOperatorExpression;
			if (binary != null && assignment.Operator == AssignmentOperatorType.Assign)
			{
				if (CanConvertToCompoundAssignment(assignment.Left) && assignment.Left.IsMatch(binary.Left))
				{
					assignment.Operator = GetAssignmentOperatorForBinaryOperator(binary.Operator);
					if (assignment.Operator != AssignmentOperatorType.Assign)
					{
						// If we found a shorter operator, get rid of the BinaryOperatorExpression:
						assignment.CopyAnnotationsFrom(binary);
						assignment.Right = binary.Right;
					}
				}
			}
			if (context.Settings.IntroduceIncrementAndDecrement && assignment.Operator == AssignmentOperatorType.Add || assignment.Operator == AssignmentOperatorType.Subtract)
			{
				// detect increment/decrement
				var rr = assignment.Right.GetResolveResult();
				if (rr.IsCompileTimeConstant && rr.Type.IsCSharpPrimitiveIntegerType() && CSharpPrimitiveCast.Cast(rr.Type.GetTypeCode(), 1, false).Equals(rr.ConstantValue))
				{
					// only if it's not a custom operator
					if (assignment.Annotation<IL.CallInstruction>() == null && assignment.Annotation<IL.UserDefinedCompoundAssign>() == null && assignment.Annotation<IL.DynamicCompoundAssign>() == null)
					{
						UnaryOperatorType type;
						// When the parent is an expression statement, pre- or post-increment doesn't matter;
						// so we can pick post-increment which is more commonly used (for (int i = 0; i < x; i++))
						if (assignment.Parent is ExpressionStatement)
							type = (assignment.Operator == AssignmentOperatorType.Add) ? UnaryOperatorType.PostIncrement : UnaryOperatorType.PostDecrement;
						else
							type = (assignment.Operator == AssignmentOperatorType.Add) ? UnaryOperatorType.Increment : UnaryOperatorType.Decrement;
						assignment.ReplaceWith(new UnaryOperatorExpression(type, assignment.Left.Detach()).CopyAnnotationsFrom(assignment));
					}
				}
			}
		}

		public static AssignmentOperatorType GetAssignmentOperatorForBinaryOperator(BinaryOperatorType bop)
		{
			switch (bop)
			{
				case BinaryOperatorType.Add:
					return AssignmentOperatorType.Add;
				case BinaryOperatorType.Subtract:
					return AssignmentOperatorType.Subtract;
				case BinaryOperatorType.Multiply:
					return AssignmentOperatorType.Multiply;
				case BinaryOperatorType.Divide:
					return AssignmentOperatorType.Divide;
				case BinaryOperatorType.Modulus:
					return AssignmentOperatorType.Modulus;
				case BinaryOperatorType.ShiftLeft:
					return AssignmentOperatorType.ShiftLeft;
				case BinaryOperatorType.ShiftRight:
					return AssignmentOperatorType.ShiftRight;
				case BinaryOperatorType.BitwiseAnd:
					return AssignmentOperatorType.BitwiseAnd;
				case BinaryOperatorType.BitwiseOr:
					return AssignmentOperatorType.BitwiseOr;
				case BinaryOperatorType.ExclusiveOr:
					return AssignmentOperatorType.ExclusiveOr;
				default:
					return AssignmentOperatorType.Assign;
			}
		}

		static bool CanConvertToCompoundAssignment(Expression left)
		{
			MemberReferenceExpression mre = left as MemberReferenceExpression;
			if (mre != null)
				return IsWithoutSideEffects(mre.Target);
			IndexerExpression ie = left as IndexerExpression;
			if (ie != null)
				return IsWithoutSideEffects(ie.Target) && ie.Arguments.All(IsWithoutSideEffects);
			UnaryOperatorExpression uoe = left as UnaryOperatorExpression;
			if (uoe != null && uoe.Operator == UnaryOperatorType.Dereference)
				return IsWithoutSideEffects(uoe.Expression);
			return IsWithoutSideEffects(left);
		}

		static bool IsWithoutSideEffects(Expression left)
		{
			return left is ThisReferenceExpression || left is IdentifierExpression || left is TypeReferenceExpression || left is BaseReferenceExpression;
		}

		void IAstTransform.Run(AstNode node, TransformContext context)
		{
			this.context = context;
			try
			{
				node.AcceptVisitor(this);
			}
			finally
			{
				this.context = null;
			}
		}
	}
}
