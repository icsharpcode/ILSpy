// 
// UnaryOperatorExpression.cs
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
	/// <code>
	/// unary_operator_expression ::=
	///       unary_operator expression
	///     | expression postfix_unary_operator
	/// </code>
	/// (C# grammar §12.9, §12.8.16)
	/// <c>unary_operator ::= '+' | '-' | '!' | '~' | '++' | '--' | '*' | '&amp;' | '^' | 'await'</c>
	/// <c>postfix_unary_operator ::= '++' | '--' | '!' | '?'</c>
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class UnaryOperatorExpression : Expression
	{
		public const string NotToken = "!";
		public const string BitNotToken = "~";
		public const string MinusToken = "-";
		public const string PlusToken = "+";
		public const string IncrementToken = "++";
		public const string DecrementToken = "--";
		public const string DereferenceToken = "*";
		public const string AddressOfToken = "&";
		public const string AwaitKeyword = "await";
		public const string NullConditionalToken = "?";
		public const string SuppressNullableWarningToken = "!";
		public const string IndexFromEndToken = "^";
		public const string PatternNotKeyword = "not";
		public UnaryOperatorType Operator { get; set; }

		[Slot("Expression")]
		public partial Expression Expression { get; set; }

		public static string? GetOperatorToken(UnaryOperatorType op)
		{
			switch (op)
			{
				case UnaryOperatorType.Not:
					return NotToken;
				case UnaryOperatorType.BitNot:
					return BitNotToken;
				case UnaryOperatorType.Minus:
					return MinusToken;
				case UnaryOperatorType.Plus:
					return PlusToken;
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
					return IncrementToken;
				case UnaryOperatorType.PostDecrement:
				case UnaryOperatorType.Decrement:
					return DecrementToken;
				case UnaryOperatorType.Dereference:
					return DereferenceToken;
				case UnaryOperatorType.AddressOf:
					return AddressOfToken;
				case UnaryOperatorType.Await:
					return AwaitKeyword;
				case UnaryOperatorType.NullConditional:
					return NullConditionalToken;
				case UnaryOperatorType.NullConditionalRewrap:
				case UnaryOperatorType.IsTrue:
					return null; // no syntax
				case UnaryOperatorType.SuppressNullableWarning:
					return SuppressNullableWarningToken;
				case UnaryOperatorType.IndexFromEnd:
					return IndexFromEndToken;
				case UnaryOperatorType.PatternNot:
					return PatternNotKeyword;
				case UnaryOperatorType.PatternRelationalLessThan:
					return BinaryOperatorExpression.LessThanToken;
				case UnaryOperatorType.PatternRelationalLessThanOrEqual:
					return BinaryOperatorExpression.LessThanOrEqualToken;
				case UnaryOperatorType.PatternRelationalGreaterThan:
					return BinaryOperatorExpression.GreaterThanToken;
				case UnaryOperatorType.PatternRelationalGreaterThanOrEqual:
					return BinaryOperatorExpression.GreaterThanOrEqualToken;
				default:
					throw new NotSupportedException("Invalid value for UnaryOperatorType");
			}
		}

		public static ExpressionType GetLinqNodeType(UnaryOperatorType op, bool checkForOverflow)
		{
			switch (op)
			{
				case UnaryOperatorType.Not:
					return ExpressionType.Not;
				case UnaryOperatorType.BitNot:
					return ExpressionType.OnesComplement;
				case UnaryOperatorType.Minus:
					return checkForOverflow ? ExpressionType.NegateChecked : ExpressionType.Negate;
				case UnaryOperatorType.Plus:
					return ExpressionType.UnaryPlus;
				case UnaryOperatorType.Increment:
					return ExpressionType.PreIncrementAssign;
				case UnaryOperatorType.Decrement:
					return ExpressionType.PreDecrementAssign;
				case UnaryOperatorType.PostIncrement:
					return ExpressionType.PostIncrementAssign;
				case UnaryOperatorType.PostDecrement:
					return ExpressionType.PostDecrementAssign;
				case UnaryOperatorType.Dereference:
				case UnaryOperatorType.AddressOf:
				case UnaryOperatorType.Await:
				case UnaryOperatorType.SuppressNullableWarning:
				case UnaryOperatorType.IndexFromEnd:
				case UnaryOperatorType.PatternNot:
				case UnaryOperatorType.PatternRelationalLessThan:
				case UnaryOperatorType.PatternRelationalLessThanOrEqual:
				case UnaryOperatorType.PatternRelationalGreaterThan:
				case UnaryOperatorType.PatternRelationalGreaterThanOrEqual:
					return ExpressionType.Extension;
				default:
					throw new NotSupportedException("Invalid value for UnaryOperatorType");
			}
		}
	}

	public enum UnaryOperatorType
	{
		/// <summary>
		/// Any unary operator (used in pattern matching)
		/// </summary>
		Any,

		/// <summary>Logical not (!a)</summary>
		Not,
		/// <summary>Bitwise not (~a)</summary>
		BitNot,
		/// <summary>Unary minus (-a)</summary>
		Minus,
		/// <summary>Unary plus (+a)</summary>
		Plus,
		/// <summary>Pre increment (++a)</summary>
		Increment,
		/// <summary>Pre decrement (--a)</summary>
		Decrement,
		/// <summary>Post increment (a++)</summary>
		PostIncrement,
		/// <summary>Post decrement (a--)</summary>
		PostDecrement,
		/// <summary>Dereferencing (*a)</summary>
		Dereference,
		/// <summary>Get address (&amp;a)</summary>
		AddressOf,
		/// <summary>C# 5.0 await</summary>
		Await,
		/// <summary>C# 6 null-conditional operator.
		/// Occurs as target of member reference or indexer expressions
		/// to indicate <c>?.</c> or <c>?[]</c>.
		/// Corresponds to <c>nullable.unwrap</c> in ILAst.
		/// </summary>
		NullConditional,
		/// <summary>
		/// Wrapper around a primary expression containing a null conditional operator.
		/// Corresponds to <c>nullable.rewrap</c> in ILAst.
		/// This has no syntax in C#, but the node is used to ensure parentheses are inserted where necessary.
		/// </summary>
		NullConditionalRewrap,
		/// <summary>
		/// Implicit call of "operator true".
		/// </summary>
		IsTrue,
		/// <summary>
		/// C# 8 postfix ! operator (dammit operator)
		/// </summary>
		SuppressNullableWarning,
		/// <summary>
		/// C# 8 prefix ^ operator
		/// </summary>
		IndexFromEnd,
		/// <summary>
		/// C# 9 not pattern
		/// </summary>
		PatternNot,
		/// <summary>
		/// C# 9 relational pattern
		/// </summary>
		PatternRelationalLessThan,
		/// <summary>
		/// C# 9 relational pattern
		/// </summary>
		PatternRelationalLessThanOrEqual,
		/// <summary>
		/// C# 9 relational pattern
		/// </summary>
		PatternRelationalGreaterThan,
		/// <summary>
		/// C# 9 relational pattern
		/// </summary>
		PatternRelationalGreaterThanOrEqual,
	}
}
