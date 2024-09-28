﻿// 
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

using System;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Operator Expression
	/// </summary>
	public class UnaryOperatorExpression : Expression
	{
		public readonly static TokenRole NotRole = new TokenRole("!");
		public readonly static TokenRole BitNotRole = new TokenRole("~");
		public readonly static TokenRole MinusRole = new TokenRole("-");
		public readonly static TokenRole PlusRole = new TokenRole("+");
		public readonly static TokenRole IncrementRole = new TokenRole("++");
		public readonly static TokenRole DecrementRole = new TokenRole("--");
		public readonly static TokenRole DereferenceRole = new TokenRole("*");
		public readonly static TokenRole AddressOfRole = new TokenRole("&");
		public readonly static TokenRole AwaitRole = new TokenRole("await");
		public readonly static TokenRole NullConditionalRole = new TokenRole("?");
		public readonly static TokenRole SuppressNullableWarningRole = new TokenRole("!");
		public readonly static TokenRole IndexFromEndRole = new TokenRole("^");
		public readonly static TokenRole PatternNotRole = new TokenRole("not");

		public UnaryOperatorExpression()
		{
		}

		public UnaryOperatorExpression(UnaryOperatorType op, Expression expression)
		{
			this.Operator = op;
			this.Expression = expression;
		}

		public UnaryOperatorType Operator {
			get;
			set;
		}

		public CSharpTokenNode OperatorToken {
			get { return GetChildByRole(GetOperatorRole(Operator)); }
		}

		public Expression Expression {
			get { return GetChildByRole(Roles.Expression); }
			set { SetChildByRole(Roles.Expression, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitUnaryOperatorExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitUnaryOperatorExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitUnaryOperatorExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			UnaryOperatorExpression? o = other as UnaryOperatorExpression;
			return o != null && (this.Operator == UnaryOperatorType.Any || this.Operator == o.Operator)
				&& this.Expression.DoMatch(o.Expression, match);
		}

		public static TokenRole? GetOperatorRole(UnaryOperatorType op)
		{
			switch (op)
			{
				case UnaryOperatorType.Not:
					return NotRole;
				case UnaryOperatorType.BitNot:
					return BitNotRole;
				case UnaryOperatorType.Minus:
					return MinusRole;
				case UnaryOperatorType.Plus:
					return PlusRole;
				case UnaryOperatorType.Increment:
				case UnaryOperatorType.PostIncrement:
					return IncrementRole;
				case UnaryOperatorType.PostDecrement:
				case UnaryOperatorType.Decrement:
					return DecrementRole;
				case UnaryOperatorType.Dereference:
					return DereferenceRole;
				case UnaryOperatorType.AddressOf:
					return AddressOfRole;
				case UnaryOperatorType.Await:
					return AwaitRole;
				case UnaryOperatorType.NullConditional:
					return NullConditionalRole;
				case UnaryOperatorType.NullConditionalRewrap:
				case UnaryOperatorType.IsTrue:
					return null; // no syntax
				case UnaryOperatorType.SuppressNullableWarning:
					return SuppressNullableWarningRole;
				case UnaryOperatorType.IndexFromEnd:
					return IndexFromEndRole;
				case UnaryOperatorType.PatternNot:
					return PatternNotRole;
				case UnaryOperatorType.PatternRelationalLessThan:
					return BinaryOperatorExpression.LessThanRole;
				case UnaryOperatorType.PatternRelationalLessThanOrEqual:
					return BinaryOperatorExpression.LessThanOrEqualRole;
				case UnaryOperatorType.PatternRelationalGreaterThan:
					return BinaryOperatorExpression.GreaterThanRole;
				case UnaryOperatorType.PatternRelationalGreaterThanOrEqual:
					return BinaryOperatorExpression.GreaterThanOrEqualRole;
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
