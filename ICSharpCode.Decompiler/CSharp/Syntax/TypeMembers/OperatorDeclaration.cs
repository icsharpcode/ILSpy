// 
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

#nullable enable

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

	/// <summary>
	/// <c>operator_declaration ::= attribute_section* modifier+ type ( type '.' )? 'operator' 'checked'? operator_token '(' parameter* ')' ( block | ';' )</c> (C# grammar §15.10.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class OperatorDeclaration : EntityDeclaration
	{
		public const string OperatorKeyword = "operator";
		public const string CheckedKeyword = "checked";

		// Unary operators

		// Unary and Binary operators

		// Binary operators

		public const string ExplicitKeyword = "explicit";
		public const string ImplicitKeyword = "implicit";

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

		[Slot("AttributeSection")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Type")]
		public override partial AstType ReturnType { get; set; }

		/// <summary>
		/// Gets/Sets the type reference of the interface that is explicitly implemented.
		/// Null if this member is not an explicit interface implementation.
		/// </summary>
		[Slot("PrivateImplementationType")]
		public partial AstType? PrivateImplementationType { get; set; }

		public OperatorType OperatorType { get; set; }

		[Slot("Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		[Slot("Body")]
		public partial BlockStatement? Body { get; set; }

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

		public override string Name {
			get { return GetName(this.OperatorType)!; }
			set { throw new NotSupportedException(); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Identifier NameToken {
			get { return null!; }
			set { throw new NotSupportedException(); }
		}
	}
}
