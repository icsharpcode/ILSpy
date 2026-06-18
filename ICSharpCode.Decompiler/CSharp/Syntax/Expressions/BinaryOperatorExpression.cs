// 
// BinaryOperatorExpression.cs
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

using System;
using System.Linq.Expressions;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Operator precedence is not represented in the syntax tree; required parentheses are reconstructed by <see cref="ICSharpCode.Decompiler.CSharp.OutputVisitor.InsertParenthesesVisitor"/>.
	/// <c>binary_operator_expression ::= expression binary_operator expression</c> (C# grammar §12.13-§12.18, precedence-flattened)
	/// <c>binary_operator ::= '*' | '/' | '%' | '+' | '-' | '&lt;&lt;' | '&gt;&gt;' | '&gt;&gt;&gt;' | '&lt;' | '&gt;' | '&lt;=' | '&gt;=' | '==' | '!=' | '&amp;' | '^' | '|' | '&amp;&amp;' | '||' | '??' | '..'</c>
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class BinaryOperatorExpression : Expression
	{
		public const string BitwiseAndToken = "&";
		public const string BitwiseOrToken = "|";
		public const string ConditionalAndToken = "&&";
		public const string ConditionalOrToken = "||";
		public const string ExclusiveOrToken = "^";
		public const string GreaterThanToken = ">";
		public const string GreaterThanOrEqualToken = ">=";
		public const string EqualityToken = "==";
		public const string InEqualityToken = "!=";
		public const string LessThanToken = "<";
		public const string LessThanOrEqualToken = "<=";
		public const string AddToken = "+";
		public const string SubtractToken = "-";
		public const string MultiplyToken = "*";
		public const string DivideToken = "/";
		public const string ModulusToken = "%";
		public const string ShiftLeftToken = "<<";
		public const string ShiftRightToken = ">>";
		public const string UnsignedShiftRightToken = ">>>";
		public const string NullCoalescingToken = "??";
		public const string RangeToken = "..";
		public const string IsKeyword = IsExpression.IsKeyword;

		[Slot("Left")]
		public partial Expression? Left { get; set; }

		public BinaryOperatorType Operator { get; set; }

		[Slot("Right")]
		public partial Expression? Right { get; set; }

		public static string GetOperatorToken(BinaryOperatorType op)
		{
			switch (op)
			{
				case BinaryOperatorType.BitwiseAnd:
					return BitwiseAndToken;
				case BinaryOperatorType.BitwiseOr:
					return BitwiseOrToken;
				case BinaryOperatorType.ConditionalAnd:
					return ConditionalAndToken;
				case BinaryOperatorType.ConditionalOr:
					return ConditionalOrToken;
				case BinaryOperatorType.ExclusiveOr:
					return ExclusiveOrToken;
				case BinaryOperatorType.GreaterThan:
					return GreaterThanToken;
				case BinaryOperatorType.GreaterThanOrEqual:
					return GreaterThanOrEqualToken;
				case BinaryOperatorType.Equality:
					return EqualityToken;
				case BinaryOperatorType.InEquality:
					return InEqualityToken;
				case BinaryOperatorType.LessThan:
					return LessThanToken;
				case BinaryOperatorType.LessThanOrEqual:
					return LessThanOrEqualToken;
				case BinaryOperatorType.Add:
					return AddToken;
				case BinaryOperatorType.Subtract:
					return SubtractToken;
				case BinaryOperatorType.Multiply:
					return MultiplyToken;
				case BinaryOperatorType.Divide:
					return DivideToken;
				case BinaryOperatorType.Modulus:
					return ModulusToken;
				case BinaryOperatorType.ShiftLeft:
					return ShiftLeftToken;
				case BinaryOperatorType.ShiftRight:
					return ShiftRightToken;
				case BinaryOperatorType.UnsignedShiftRight:
					return UnsignedShiftRightToken;
				case BinaryOperatorType.NullCoalescing:
					return NullCoalescingToken;
				case BinaryOperatorType.Range:
					return RangeToken;
				case BinaryOperatorType.IsPattern:
					return IsKeyword;
				default:
					throw new NotSupportedException("Invalid value for BinaryOperatorType");
			}
		}

		public static ExpressionType GetLinqNodeType(BinaryOperatorType op, bool checkForOverflow)
		{
			switch (op)
			{
				case BinaryOperatorType.BitwiseAnd:
					return ExpressionType.And;
				case BinaryOperatorType.BitwiseOr:
					return ExpressionType.Or;
				case BinaryOperatorType.ConditionalAnd:
					return ExpressionType.AndAlso;
				case BinaryOperatorType.ConditionalOr:
					return ExpressionType.OrElse;
				case BinaryOperatorType.ExclusiveOr:
					return ExpressionType.ExclusiveOr;
				case BinaryOperatorType.GreaterThan:
					return ExpressionType.GreaterThan;
				case BinaryOperatorType.GreaterThanOrEqual:
					return ExpressionType.GreaterThanOrEqual;
				case BinaryOperatorType.Equality:
					return ExpressionType.Equal;
				case BinaryOperatorType.InEquality:
					return ExpressionType.NotEqual;
				case BinaryOperatorType.LessThan:
					return ExpressionType.LessThan;
				case BinaryOperatorType.LessThanOrEqual:
					return ExpressionType.LessThanOrEqual;
				case BinaryOperatorType.Add:
					return checkForOverflow ? ExpressionType.AddChecked : ExpressionType.Add;
				case BinaryOperatorType.Subtract:
					return checkForOverflow ? ExpressionType.SubtractChecked : ExpressionType.Subtract;
				case BinaryOperatorType.Multiply:
					return checkForOverflow ? ExpressionType.MultiplyChecked : ExpressionType.Multiply;
				case BinaryOperatorType.Divide:
					return ExpressionType.Divide;
				case BinaryOperatorType.Modulus:
					return ExpressionType.Modulo;
				case BinaryOperatorType.ShiftLeft:
					return ExpressionType.LeftShift;
				case BinaryOperatorType.ShiftRight:
					return ExpressionType.RightShift;
				case BinaryOperatorType.NullCoalescing:
					return ExpressionType.Coalesce;
				case BinaryOperatorType.Range:
				case BinaryOperatorType.UnsignedShiftRight:
					return ExpressionType.Extension;
				default:
					throw new NotSupportedException("Invalid value for BinaryOperatorType");
			}
		}
	}

	public enum BinaryOperatorType
	{
		/// <summary>
		/// Any binary operator (used in pattern matching)
		/// </summary>
		Any,

		// We avoid 'logical or' on purpose, because it's not clear if that refers to the bitwise
		// or to the short-circuiting (conditional) operator:
		// MCS and old NRefactory used bitwise='|', logical='||'
		// but the C# spec uses logical='|', conditional='||'
		/// <summary>left &amp; right</summary>
		BitwiseAnd,
		/// <summary>left | right</summary>
		BitwiseOr,
		/// <summary>left &amp;&amp; right</summary>
		ConditionalAnd,
		/// <summary>left || right</summary>
		ConditionalOr,
		/// <summary>left ^ right</summary>
		ExclusiveOr,

		/// <summary>left &gt; right</summary>
		GreaterThan,
		/// <summary>left &gt;= right</summary>
		GreaterThanOrEqual,
		/// <summary>left == right</summary>
		Equality,
		/// <summary>left != right</summary>
		InEquality,
		/// <summary>left &lt; right</summary>
		LessThan,
		/// <summary>left &lt;= right</summary>
		LessThanOrEqual,

		/// <summary>left + right</summary>
		Add,
		/// <summary>left - right</summary>
		Subtract,
		/// <summary>left * right</summary>
		Multiply,
		/// <summary>left / right</summary>
		Divide,
		/// <summary>left % right</summary>
		Modulus,

		/// <summary>left &lt;&lt; right</summary>
		ShiftLeft,
		/// <summary>left &gt;&gt; right</summary>
		ShiftRight,
		/// <summary>left &gt;&gt;&gt; right</summary>
		UnsignedShiftRight,

		/// <summary>left ?? right</summary>
		NullCoalescing,
		/// <summary>left .. right</summary>
		/// <remarks>left and right are optional and may be null</remarks>
		Range,

		/// <summary>left is right</summary>
		/// <remarks>right must be a pattern</remarks>
		IsPattern,
	}
}
