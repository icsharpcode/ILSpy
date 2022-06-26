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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Left Operator= Right
	/// </summary>
	public class AssignmentExpression : Expression
	{
		// reuse roles from BinaryOperatorExpression
		public readonly static Role<Expression> LeftRole = BinaryOperatorExpression.LeftRole;
		public readonly static Role<Expression> RightRole = BinaryOperatorExpression.RightRole;

		public readonly static TokenRole AssignRole = new("=");
		public readonly static TokenRole AddRole = new("+=");
		public readonly static TokenRole SubtractRole = new("-=");
		public readonly static TokenRole MultiplyRole = new("*=");
		public readonly static TokenRole DivideRole = new("/=");
		public readonly static TokenRole ModulusRole = new("%=");
		public readonly static TokenRole ShiftLeftRole = new("<<=");
		public readonly static TokenRole ShiftRightRole = new(">>=");
		public readonly static TokenRole BitwiseAndRole = new("&=");
		public readonly static TokenRole BitwiseOrRole = new("|=");
		public readonly static TokenRole ExclusiveOrRole = new("^=");

		public AssignmentExpression()
		{
		}

		public AssignmentExpression(Expression left, Expression right)
		{
			this.Left = left;
			this.Right = right;
		}

		public AssignmentExpression(Expression left, AssignmentOperatorType op, Expression right)
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}

		public AssignmentOperatorType Operator {
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
			visitor.VisitAssignmentExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitAssignmentExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitAssignmentExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			AssignmentExpression o = other as AssignmentExpression;
			return o != null && (this.Operator == AssignmentOperatorType.Any || this.Operator == o.Operator)
				&& this.Left.DoMatch(o.Left, match) && this.Right.DoMatch(o.Right, match);
		}

		public static TokenRole GetOperatorRole(AssignmentOperatorType op)
		{
			return op switch {
				AssignmentOperatorType.Assign => AssignRole,
				AssignmentOperatorType.Add => AddRole,
				AssignmentOperatorType.Subtract => SubtractRole,
				AssignmentOperatorType.Multiply => MultiplyRole,
				AssignmentOperatorType.Divide => DivideRole,
				AssignmentOperatorType.Modulus => ModulusRole,
				AssignmentOperatorType.ShiftLeft => ShiftLeftRole,
				AssignmentOperatorType.ShiftRight => ShiftRightRole,
				AssignmentOperatorType.BitwiseAnd => BitwiseAndRole,
				AssignmentOperatorType.BitwiseOr => BitwiseOrRole,
				AssignmentOperatorType.ExclusiveOr => ExclusiveOrRole,
				_ => throw new NotSupportedException("Invalid value for AssignmentOperatorType")
			};
		}

		/// <summary>
		/// Gets the binary operator for the specified compound assignment operator.
		/// Returns null if 'op' is not a compound assignment.
		/// </summary>
		public static BinaryOperatorType? GetCorrespondingBinaryOperator(AssignmentOperatorType op)
		{
			return op switch {
				AssignmentOperatorType.Assign => null,
				AssignmentOperatorType.Add => BinaryOperatorType.Add,
				AssignmentOperatorType.Subtract => BinaryOperatorType.Subtract,
				AssignmentOperatorType.Multiply => BinaryOperatorType.Multiply,
				AssignmentOperatorType.Divide => BinaryOperatorType.Divide,
				AssignmentOperatorType.Modulus => BinaryOperatorType.Modulus,
				AssignmentOperatorType.ShiftLeft => BinaryOperatorType.ShiftLeft,
				AssignmentOperatorType.ShiftRight => BinaryOperatorType.ShiftRight,
				AssignmentOperatorType.BitwiseAnd => BinaryOperatorType.BitwiseAnd,
				AssignmentOperatorType.BitwiseOr => BinaryOperatorType.BitwiseOr,
				AssignmentOperatorType.ExclusiveOr => BinaryOperatorType.ExclusiveOr,
				_ => throw new NotSupportedException("Invalid value for AssignmentOperatorType")
			};
		}

		public static ExpressionType GetLinqNodeType(AssignmentOperatorType op, bool checkForOverflow)
		{
			return op switch {
				AssignmentOperatorType.Assign => ExpressionType.Assign,
				AssignmentOperatorType.Add => checkForOverflow
					? ExpressionType.AddAssignChecked
					: ExpressionType.AddAssign,
				AssignmentOperatorType.Subtract => checkForOverflow
					? ExpressionType.SubtractAssignChecked
					: ExpressionType.SubtractAssign,
				AssignmentOperatorType.Multiply => checkForOverflow
					? ExpressionType.MultiplyAssignChecked
					: ExpressionType.MultiplyAssign,
				AssignmentOperatorType.Divide => ExpressionType.DivideAssign,
				AssignmentOperatorType.Modulus => ExpressionType.ModuloAssign,
				AssignmentOperatorType.ShiftLeft => ExpressionType.LeftShiftAssign,
				AssignmentOperatorType.ShiftRight => ExpressionType.RightShiftAssign,
				AssignmentOperatorType.BitwiseAnd => ExpressionType.AndAssign,
				AssignmentOperatorType.BitwiseOr => ExpressionType.OrAssign,
				AssignmentOperatorType.ExclusiveOr => ExpressionType.ExclusiveOrAssign,
				_ => throw new NotSupportedException("Invalid value for AssignmentOperatorType")
			};
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
