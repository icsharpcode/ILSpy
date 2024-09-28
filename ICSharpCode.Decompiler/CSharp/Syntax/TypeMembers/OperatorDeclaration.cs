﻿// 
// OperatorDeclaration.cs
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
using System.ComponentModel;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public enum OperatorType
	{
		// Unary operators
		LogicalNot,
		OnesComplement,
		Increment,
		CheckedIncrement,
		Decrement,
		CheckedDecrement,
		True,
		False,

		UnaryPlus,
		UnaryNegation,
		CheckedUnaryNegation,

		// Binary operators
		Addition,
		CheckedAddition,
		Subtraction,
		CheckedSubtraction,
		Multiply,
		CheckedMultiply,
		Division,
		CheckedDivision,
		Modulus,
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
		LeftShift,
		RightShift,
		UnsignedRightShift,
		Equality,
		Inequality,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual,

		// Implicit and Explicit
		Implicit,
		Explicit,
		CheckedExplicit
	}

	public class OperatorDeclaration : EntityDeclaration
	{
		public static readonly TokenRole OperatorKeywordRole = new TokenRole("operator");
		public static readonly TokenRole CheckedKeywordRole = new TokenRole("checked");

		// Unary operators
		public static readonly TokenRole LogicalNotRole = new TokenRole("!");
		public static readonly TokenRole OnesComplementRole = new TokenRole("~");
		public static readonly TokenRole IncrementRole = new TokenRole("++");
		public static readonly TokenRole DecrementRole = new TokenRole("--");
		public static readonly TokenRole TrueRole = new TokenRole("true");
		public static readonly TokenRole FalseRole = new TokenRole("false");

		// Unary and Binary operators
		public static readonly TokenRole AdditionRole = new TokenRole("+");
		public static readonly TokenRole SubtractionRole = new TokenRole("-");

		// Binary operators
		public static readonly TokenRole MultiplyRole = new TokenRole("*");
		public static readonly TokenRole DivisionRole = new TokenRole("/");
		public static readonly TokenRole ModulusRole = new TokenRole("%");
		public static readonly TokenRole BitwiseAndRole = new TokenRole("&");
		public static readonly TokenRole BitwiseOrRole = new TokenRole("|");
		public static readonly TokenRole ExclusiveOrRole = new TokenRole("^");
		public static readonly TokenRole LeftShiftRole = new TokenRole("<<");
		public static readonly TokenRole RightShiftRole = new TokenRole(">>");
		public static readonly TokenRole UnsignedRightShiftRole = new TokenRole(">>>");
		public static readonly TokenRole EqualityRole = new TokenRole("==");
		public static readonly TokenRole InequalityRole = new TokenRole("!=");
		public static readonly TokenRole GreaterThanRole = new TokenRole(">");
		public static readonly TokenRole LessThanRole = new TokenRole("<");
		public static readonly TokenRole GreaterThanOrEqualRole = new TokenRole(">=");
		public static readonly TokenRole LessThanOrEqualRole = new TokenRole("<=");

		public static readonly TokenRole ExplicitRole = new TokenRole("explicit");
		public static readonly TokenRole ImplicitRole = new TokenRole("implicit");

		static readonly string[][] names;

		static OperatorDeclaration()
		{
			names = new string[(int)OperatorType.CheckedExplicit + 1][];
			names[(int)OperatorType.LogicalNot] = new string[] { "!", "op_LogicalNot" };
			names[(int)OperatorType.OnesComplement] = new string[] { "~", "op_OnesComplement" };
			names[(int)OperatorType.Increment] = new string[] { "++", "op_Increment" };
			names[(int)OperatorType.CheckedIncrement] = new string[] { "++", "op_CheckedIncrement" };
			names[(int)OperatorType.Decrement] = new string[] { "--", "op_Decrement" };
			names[(int)OperatorType.CheckedDecrement] = new string[] { "--", "op_CheckedDecrement" };
			names[(int)OperatorType.True] = new string[] { "true", "op_True" };
			names[(int)OperatorType.False] = new string[] { "false", "op_False" };
			names[(int)OperatorType.UnaryPlus] = new string[] { "+", "op_UnaryPlus" };
			names[(int)OperatorType.UnaryNegation] = new string[] { "-", "op_UnaryNegation" };
			names[(int)OperatorType.CheckedUnaryNegation] = new string[] { "-", "op_CheckedUnaryNegation" };
			names[(int)OperatorType.Addition] = new string[] { "+", "op_Addition" };
			names[(int)OperatorType.CheckedAddition] = new string[] { "+", "op_CheckedAddition" };
			names[(int)OperatorType.Subtraction] = new string[] { "-", "op_Subtraction" };
			names[(int)OperatorType.CheckedSubtraction] = new string[] { "-", "op_CheckedSubtraction" };
			names[(int)OperatorType.Multiply] = new string[] { "*", "op_Multiply" };
			names[(int)OperatorType.CheckedMultiply] = new string[] { "*", "op_CheckedMultiply" };
			names[(int)OperatorType.Division] = new string[] { "/", "op_Division" };
			names[(int)OperatorType.CheckedDivision] = new string[] { "/", "op_CheckedDivision" };
			names[(int)OperatorType.Modulus] = new string[] { "%", "op_Modulus" };
			names[(int)OperatorType.BitwiseAnd] = new string[] { "&", "op_BitwiseAnd" };
			names[(int)OperatorType.BitwiseOr] = new string[] { "|", "op_BitwiseOr" };
			names[(int)OperatorType.ExclusiveOr] = new string[] { "^", "op_ExclusiveOr" };
			names[(int)OperatorType.LeftShift] = new string[] { "<<", "op_LeftShift" };
			names[(int)OperatorType.RightShift] = new string[] { ">>", "op_RightShift" };
			names[(int)OperatorType.UnsignedRightShift] = new string[] { ">>>", "op_UnsignedRightShift" };
			names[(int)OperatorType.Equality] = new string[] { "==", "op_Equality" };
			names[(int)OperatorType.Inequality] = new string[] { "!=", "op_Inequality" };
			names[(int)OperatorType.GreaterThan] = new string[] { ">", "op_GreaterThan" };
			names[(int)OperatorType.LessThan] = new string[] { "<", "op_LessThan" };
			names[(int)OperatorType.GreaterThanOrEqual] = new string[] { ">=", "op_GreaterThanOrEqual" };
			names[(int)OperatorType.LessThanOrEqual] = new string[] { "<=", "op_LessThanOrEqual" };
			names[(int)OperatorType.Implicit] = new string[] { "implicit", "op_Implicit" };
			names[(int)OperatorType.Explicit] = new string[] { "explicit", "op_Explicit" };
			names[(int)OperatorType.CheckedExplicit] = new string[] { "explicit", "op_CheckedExplicit" };
		}

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Operator; }
		}

		/// <summary>
		/// Gets/Sets the type reference of the interface that is explicitly implemented.
		/// Null node if this member is not an explicit interface implementation.
		/// </summary>
		public AstType PrivateImplementationType {
			get { return GetChildByRole(PrivateImplementationTypeRole); }
			set { SetChildByRole(PrivateImplementationTypeRole, value); }
		}

		OperatorType operatorType;

		public OperatorType OperatorType {
			get { return operatorType; }
			set {
				ThrowIfFrozen();
				operatorType = value;
			}
		}

		public CSharpTokenNode OperatorToken {
			get { return GetChildByRole(OperatorKeywordRole); }
		}

		public CSharpTokenNode OperatorTypeToken {
			get { return GetChildByRole(GetRole(OperatorType)); }
		}

		public CSharpTokenNode LParToken {
			get { return GetChildByRole(Roles.LPar); }
		}

		public AstNodeCollection<ParameterDeclaration> Parameters {
			get { return GetChildrenByRole(Roles.Parameter); }
		}

		public CSharpTokenNode RParToken {
			get { return GetChildByRole(Roles.RPar); }
		}

		public BlockStatement Body {
			get { return GetChildByRole(Roles.Body); }
			set { SetChildByRole(Roles.Body, value); }
		}

		/// <summary>
		/// Gets the operator type from the method name, or null, if the method does not represent one of the known operator types.
		/// </summary>
		public static OperatorType? GetOperatorType(string methodName)
		{
			for (int i = 0; i < names.Length; ++i)
			{
				if (names[i][1] == methodName)
					return (OperatorType)i;
			}

			return null;
		}

		public static TokenRole GetRole(OperatorType type)
		{
			switch (type)
			{
				case OperatorType.LogicalNot:
					return LogicalNotRole;
				case OperatorType.OnesComplement:
					return OnesComplementRole;
				case OperatorType.Increment:
				case OperatorType.CheckedIncrement:
					return IncrementRole;
				case OperatorType.Decrement:
				case OperatorType.CheckedDecrement:
					return DecrementRole;
				case OperatorType.True:
					return TrueRole;
				case OperatorType.False:
					return FalseRole;

				case OperatorType.Addition:
				case OperatorType.CheckedAddition:
				case OperatorType.UnaryPlus:
					return AdditionRole;
				case OperatorType.Subtraction:
				case OperatorType.CheckedSubtraction:
				case OperatorType.UnaryNegation:
				case OperatorType.CheckedUnaryNegation:
					return SubtractionRole;

				case OperatorType.Multiply:
				case OperatorType.CheckedMultiply:
					return MultiplyRole;
				case OperatorType.Division:
				case OperatorType.CheckedDivision:
					return DivisionRole;
				case OperatorType.Modulus:
					return ModulusRole;
				case OperatorType.BitwiseAnd:
					return BitwiseAndRole;
				case OperatorType.BitwiseOr:
					return BitwiseOrRole;
				case OperatorType.ExclusiveOr:
					return ExclusiveOrRole;
				case OperatorType.LeftShift:
					return LeftShiftRole;
				case OperatorType.RightShift:
					return RightShiftRole;
				case OperatorType.UnsignedRightShift:
					return UnsignedRightShiftRole;
				case OperatorType.Equality:
					return EqualityRole;
				case OperatorType.Inequality:
					return InequalityRole;
				case OperatorType.GreaterThan:
					return GreaterThanRole;
				case OperatorType.LessThan:
					return LessThanRole;
				case OperatorType.GreaterThanOrEqual:
					return GreaterThanOrEqualRole;
				case OperatorType.LessThanOrEqual:
					return LessThanOrEqualRole;

				case OperatorType.Implicit:
					return ImplicitRole;
				case OperatorType.Explicit:
				case OperatorType.CheckedExplicit:
					return ExplicitRole;

				default:
					throw new System.ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the method name for the operator type. ("op_Addition", "op_Implicit", etc.)
		/// </summary>
		public static string? GetName(OperatorType? type)
		{
			if (type == null)
				return null;
			return names[(int)type][1];
		}

		/// <summary>
		/// Gets whether the operator type is a C# 11 "operator checked".
		/// </summary>
		public static bool IsChecked(OperatorType type)
		{
			return type switch {
				OperatorType.CheckedAddition => true,
				OperatorType.CheckedSubtraction => true,
				OperatorType.CheckedMultiply => true,
				OperatorType.CheckedDivision => true,
				OperatorType.CheckedUnaryNegation => true,
				OperatorType.CheckedIncrement => true,
				OperatorType.CheckedDecrement => true,
				OperatorType.CheckedExplicit => true,
				_ => false,
			};
		}

		/// <summary>
		/// Gets the token for the operator type ("+", "implicit", etc.).
		/// Does not include the "checked" modifier.
		/// </summary>
		public static string GetToken(OperatorType type)
		{
			return names[(int)type][0];
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitOperatorDeclaration(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitOperatorDeclaration(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitOperatorDeclaration(this, data);
		}

		public override string Name {
			get { return GetName(this.OperatorType); }
			set { throw new NotSupportedException(); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Identifier NameToken {
			get { return Identifier.Null; }
			set { throw new NotSupportedException(); }
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			OperatorDeclaration? o = other as OperatorDeclaration;
			return o != null && this.MatchAttributesAndModifiers(o, match)
				&& this.PrivateImplementationType.DoMatch(o.PrivateImplementationType, match)
				&& this.OperatorType == o.OperatorType
				&& this.ReturnType.DoMatch(o.ReturnType, match)
				&& this.Parameters.DoMatch(o.Parameters, match) && this.Body.DoMatch(o.Body, match);
		}
	}
}
