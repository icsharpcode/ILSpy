// 
// AssignmentExpression.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#nullable enable

using System;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Operator precedence is not represented in the syntax tree; required parentheses are reconstructed by <see cref="ICSharpCode.Decompiler.CSharp.OutputVisitor.InsertParenthesesVisitor"/>.
	/// <c>assignment_expression ::= expression assignment_operator expression</c> (C# grammar §12.24)
	/// <c>assignment_operator ::= '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&lt;&lt;=' | '&gt;&gt;=' | '&gt;&gt;&gt;=' | '&amp;=' | '|=' | '^='</c>
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class AssignmentExpression : Expression
	{
		public const string AssignToken = "=";
		public const string AddToken = "+=";
		public const string SubtractToken = "-=";
		public const string MultiplyToken = "*=";
		public const string DivideToken = "/=";
		public const string ModulusToken = "%=";
		public const string ShiftLeftToken = "<<=";
		public const string ShiftRightToken = ">>=";
		public const string UnsignedShiftRightToken = ">>>=";
		public const string BitwiseAndToken = "&=";
		public const string BitwiseOrToken = "|=";
		public const string ExclusiveOrToken = "^=";

		// Simple assignment convenience: Operator defaults to Assign. The (left, op, right) form is generated.
		public AssignmentExpression(Expression left, Expression right)
		{
			this.Left = left;
			this.Right = right;
		}

		[Slot("Left")]
		public partial Expression Left { get; set; }

		public AssignmentOperatorType Operator { get; set; }

		[Slot("Right")]
		public partial Expression Right { get; set; }

		public static string GetOperatorToken(AssignmentOperatorType op)
		{
			switch (op)
			{
				case AssignmentOperatorType.Assign:
					return AssignToken;
				case AssignmentOperatorType.Add:
					return AddToken;
				case AssignmentOperatorType.Subtract:
					return SubtractToken;
				case AssignmentOperatorType.Multiply:
					return MultiplyToken;
				case AssignmentOperatorType.Divide:
					return DivideToken;
				case AssignmentOperatorType.Modulus:
					return ModulusToken;
				case AssignmentOperatorType.ShiftLeft:
					return ShiftLeftToken;
				case AssignmentOperatorType.ShiftRight:
					return ShiftRightToken;
				case AssignmentOperatorType.UnsignedShiftRight:
					return UnsignedShiftRightToken;
				case AssignmentOperatorType.BitwiseAnd:
					return BitwiseAndToken;
				case AssignmentOperatorType.BitwiseOr:
					return BitwiseOrToken;
				case AssignmentOperatorType.ExclusiveOr:
					return ExclusiveOrToken;
				default:
					throw new NotSupportedException("Invalid value for AssignmentOperatorType");
			}
		}

		/// <summary>
		/// Gets the binary operator for the specified compound assignment operator.
		/// Returns null if 'op' is not a compound assignment.
		/// </summary>
		public static BinaryOperatorType? GetCorrespondingBinaryOperator(AssignmentOperatorType op)
		{
			switch (op)
			{
				case AssignmentOperatorType.Assign:
					return null;
				case AssignmentOperatorType.Add:
					return BinaryOperatorType.Add;
				case AssignmentOperatorType.Subtract:
					return BinaryOperatorType.Subtract;
				case AssignmentOperatorType.Multiply:
					return BinaryOperatorType.Multiply;
				case AssignmentOperatorType.Divide:
					return BinaryOperatorType.Divide;
				case AssignmentOperatorType.Modulus:
					return BinaryOperatorType.Modulus;
				case AssignmentOperatorType.ShiftLeft:
					return BinaryOperatorType.ShiftLeft;
				case AssignmentOperatorType.ShiftRight:
					return BinaryOperatorType.ShiftRight;
				case AssignmentOperatorType.UnsignedShiftRight:
					return BinaryOperatorType.UnsignedShiftRight;
				case AssignmentOperatorType.BitwiseAnd:
					return BinaryOperatorType.BitwiseAnd;
				case AssignmentOperatorType.BitwiseOr:
					return BinaryOperatorType.BitwiseOr;
				case AssignmentOperatorType.ExclusiveOr:
					return BinaryOperatorType.ExclusiveOr;
				default:
					throw new NotSupportedException("Invalid value for AssignmentOperatorType");
			}
		}

		public static ExpressionType GetLinqNodeType(AssignmentOperatorType op, bool checkForOverflow)
		{
			switch (op)
			{
				case AssignmentOperatorType.Assign:
					return ExpressionType.Assign;
				case AssignmentOperatorType.Add:
					return checkForOverflow ? ExpressionType.AddAssignChecked : ExpressionType.AddAssign;
				case AssignmentOperatorType.Subtract:
					return checkForOverflow ? ExpressionType.SubtractAssignChecked : ExpressionType.SubtractAssign;
				case AssignmentOperatorType.Multiply:
					return checkForOverflow ? ExpressionType.MultiplyAssignChecked : ExpressionType.MultiplyAssign;
				case AssignmentOperatorType.Divide:
					return ExpressionType.DivideAssign;
				case AssignmentOperatorType.Modulus:
					return ExpressionType.ModuloAssign;
				case AssignmentOperatorType.ShiftLeft:
					return ExpressionType.LeftShiftAssign;
				case AssignmentOperatorType.ShiftRight:
					return ExpressionType.RightShiftAssign;
				case AssignmentOperatorType.UnsignedShiftRight:
					return ExpressionType.Extension;
				case AssignmentOperatorType.BitwiseAnd:
					return ExpressionType.AndAssign;
				case AssignmentOperatorType.BitwiseOr:
					return ExpressionType.OrAssign;
				case AssignmentOperatorType.ExclusiveOr:
					return ExpressionType.ExclusiveOrAssign;
				default:
					throw new NotSupportedException("Invalid value for AssignmentOperatorType");
			}
		}

		public static AssignmentOperatorType? GetAssignmentOperatorTypeFromExpressionType(ExpressionType expressionType)
		{
			switch (expressionType)
			{
				case ExpressionType.AddAssign:
				case ExpressionType.AddAssignChecked:
					return AssignmentOperatorType.Add;
				case ExpressionType.AndAssign:
					return AssignmentOperatorType.BitwiseAnd;
				case ExpressionType.DivideAssign:
					return AssignmentOperatorType.Divide;
				case ExpressionType.ExclusiveOrAssign:
					return AssignmentOperatorType.ExclusiveOr;
				case ExpressionType.LeftShiftAssign:
					return AssignmentOperatorType.ShiftLeft;
				case ExpressionType.ModuloAssign:
					return AssignmentOperatorType.Modulus;
				case ExpressionType.MultiplyAssign:
				case ExpressionType.MultiplyAssignChecked:
					return AssignmentOperatorType.Multiply;
				case ExpressionType.OrAssign:
					return AssignmentOperatorType.BitwiseOr;
				case ExpressionType.RightShiftAssign:
					return AssignmentOperatorType.ShiftRight;
				case ExpressionType.SubtractAssign:
				case ExpressionType.SubtractAssignChecked:
					return AssignmentOperatorType.Subtract;
				default:
					return null;
			}
		}
	}

	public enum AssignmentOperatorType
	{
		/// <summary>left = right</summary>
		Assign,

		/// <summary>left += right</summary>
		Add,
		/// <summary>left -= right</summary>
		Subtract,
		/// <summary>left *= right</summary>
		Multiply,
		/// <summary>left /= right</summary>
		Divide,
		/// <summary>left %= right</summary>
		Modulus,

		/// <summary>left &lt;&lt;= right</summary>
		ShiftLeft,
		/// <summary>left >>= right</summary>
		ShiftRight,
		/// <summary>left >>>= right</summary>
		UnsignedShiftRight,

		/// <summary>left &amp;= right</summary>
		BitwiseAnd,
		/// <summary>left |= right</summary>
		BitwiseOr,
		/// <summary>left ^= right</summary>
		ExclusiveOr,

		/// <summary>Any operator (for pattern matching)</summary>
		Any
	}
}
