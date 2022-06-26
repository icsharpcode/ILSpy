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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Left Operator Right
	/// </summary>
	public class BinaryOperatorExpression : Expression
	{
		public readonly static TokenRole BitwiseAndRole = new("&");
		public readonly static TokenRole BitwiseOrRole = new("|");
		public readonly static TokenRole ConditionalAndRole = new("&&");
		public readonly static TokenRole ConditionalOrRole = new("||");
		public readonly static TokenRole ExclusiveOrRole = new("^");
		public readonly static TokenRole GreaterThanRole = new(">");
		public readonly static TokenRole GreaterThanOrEqualRole = new(">=");
		public readonly static TokenRole EqualityRole = new("==");
		public readonly static TokenRole InEqualityRole = new("!=");
		public readonly static TokenRole LessThanRole = new("<");
		public readonly static TokenRole LessThanOrEqualRole = new("<=");
		public readonly static TokenRole AddRole = new("+");
		public readonly static TokenRole SubtractRole = new("-");
		public readonly static TokenRole MultiplyRole = new("*");
		public readonly static TokenRole DivideRole = new("/");
		public readonly static TokenRole ModulusRole = new("%");
		public readonly static TokenRole ShiftLeftRole = new("<<");
		public readonly static TokenRole ShiftRightRole = new(">>");
		public readonly static TokenRole NullCoalescingRole = new("??");
		public readonly static TokenRole RangeRole = new("..");
		public readonly static TokenRole IsKeywordRole = IsExpression.IsKeywordRole;

		public readonly static Role<Expression> LeftRole = new("Left", Null);
		public readonly static Role<Expression> RightRole = new("Right", Null);

		public BinaryOperatorExpression()
		{
		}

		public BinaryOperatorExpression(Expression left, BinaryOperatorType op, Expression right)
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}

		public BinaryOperatorType Operator {
			get;
			set;
		}

		public Expression Left {
			get { return GetChildByRole(LeftRole); }
			set { SetChildByRole(LeftRole, value); }
		}

		public CSharpTokenNode OperatorToken {
			get { return GetChildByRole(GetOperatorRole(Operator)); }
		}

		public Expression Right {
			get { return GetChildByRole(RightRole); }
			set { SetChildByRole(RightRole, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitBinaryOperatorExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitBinaryOperatorExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitBinaryOperatorExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			BinaryOperatorExpression o = other as BinaryOperatorExpression;
			return o != null && (this.Operator == BinaryOperatorType.Any || this.Operator == o.Operator)
				&& this.Left.DoMatch(o.Left, match) && this.Right.DoMatch(o.Right, match);
		}

		public static TokenRole GetOperatorRole(BinaryOperatorType op)
		{
			return op switch {
				BinaryOperatorType.BitwiseAnd => BitwiseAndRole,
				BinaryOperatorType.BitwiseOr => BitwiseOrRole,
				BinaryOperatorType.ConditionalAnd => ConditionalAndRole,
				BinaryOperatorType.ConditionalOr => ConditionalOrRole,
				BinaryOperatorType.ExclusiveOr => ExclusiveOrRole,
				BinaryOperatorType.GreaterThan => GreaterThanRole,
				BinaryOperatorType.GreaterThanOrEqual => GreaterThanOrEqualRole,
				BinaryOperatorType.Equality => EqualityRole,
				BinaryOperatorType.InEquality => InEqualityRole,
				BinaryOperatorType.LessThan => LessThanRole,
				BinaryOperatorType.LessThanOrEqual => LessThanOrEqualRole,
				BinaryOperatorType.Add => AddRole,
				BinaryOperatorType.Subtract => SubtractRole,
				BinaryOperatorType.Multiply => MultiplyRole,
				BinaryOperatorType.Divide => DivideRole,
				BinaryOperatorType.Modulus => ModulusRole,
				BinaryOperatorType.ShiftLeft => ShiftLeftRole,
				BinaryOperatorType.ShiftRight => ShiftRightRole,
				BinaryOperatorType.NullCoalescing => NullCoalescingRole,
				BinaryOperatorType.Range => RangeRole,
				BinaryOperatorType.IsPattern => IsKeywordRole,
				_ => throw new NotSupportedException("Invalid value for BinaryOperatorType")
			};
		}

		public static ExpressionType GetLinqNodeType(BinaryOperatorType op, bool checkForOverflow)
		{
			return op switch {
				BinaryOperatorType.BitwiseAnd => ExpressionType.And,
				BinaryOperatorType.BitwiseOr => ExpressionType.Or,
				BinaryOperatorType.ConditionalAnd => ExpressionType.AndAlso,
				BinaryOperatorType.ConditionalOr => ExpressionType.OrElse,
				BinaryOperatorType.ExclusiveOr => ExpressionType.ExclusiveOr,
				BinaryOperatorType.GreaterThan => ExpressionType.GreaterThan,
				BinaryOperatorType.GreaterThanOrEqual => ExpressionType.GreaterThanOrEqual,
				BinaryOperatorType.Equality => ExpressionType.Equal,
				BinaryOperatorType.InEquality => ExpressionType.NotEqual,
				BinaryOperatorType.LessThan => ExpressionType.LessThan,
				BinaryOperatorType.LessThanOrEqual => ExpressionType.LessThanOrEqual,
				BinaryOperatorType.Add => checkForOverflow ? ExpressionType.AddChecked : ExpressionType.Add,
				BinaryOperatorType.Subtract => checkForOverflow
					? ExpressionType.SubtractChecked
					: ExpressionType.Subtract,
				BinaryOperatorType.Multiply => checkForOverflow
					? ExpressionType.MultiplyChecked
					: ExpressionType.Multiply,
				BinaryOperatorType.Divide => ExpressionType.Divide,
				BinaryOperatorType.Modulus => ExpressionType.Modulo,
				BinaryOperatorType.ShiftLeft => ExpressionType.LeftShift,
				BinaryOperatorType.ShiftRight => ExpressionType.RightShift,
				BinaryOperatorType.NullCoalescing => ExpressionType.Coalesce,
				BinaryOperatorType.Range => ExpressionType.Extension,
				_ => throw new NotSupportedException("Invalid value for BinaryOperatorType")
			};
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

		/// <summary>left ?? right</summary>
		NullCoalescing,
		/// <summary>left .. right</summary>
		/// <remarks>left and right are optional = may be Expression.Null</remarks>
		Range,

		/// <summary>left is right</summary>
		/// <remarks>right must be a pattern</remarks>
		IsPattern,
	}
}
