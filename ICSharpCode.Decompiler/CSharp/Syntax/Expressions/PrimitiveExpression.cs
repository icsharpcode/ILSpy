// 
// PrimitiveExpression.cs
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

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Form of a C# literal.
	/// </summary>
	public enum LiteralFormat : byte
	{
		None,
		DecimalNumber,
		HexadecimalNumber,
		BinaryNumber,
		StringLiteral,
		VerbatimStringLiteral,
		CharLiteral,
		Utf8Literal,
	}

	/// <summary>
	/// Represents a literal value.
	/// <code>
	/// literal ::=
	///       boolean_literal
	///     | integer_literal
	///     | real_literal
	///     | character_literal
	///     | string_literal
	///     | null_literal
	/// </code>
	/// (C# grammar §6.4.5.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class PrimitiveExpression : Expression
	{
		public static readonly object AnyValue = new object();

		TextLocation startLocation;
		TextLocation endLocation;
		public override TextLocation StartLocation => startLocation;
		public override TextLocation EndLocation => endLocation;

		internal void SetLocation(TextLocation startLocation, TextLocation endLocation)
		{
			this.startLocation = startLocation;
			this.endLocation = endLocation;
		}

		object value;

		public object Value {
			get { return this.value; }
			set {
				this.value = value;
			}
		}

		public LiteralFormat Format { get; set; }

		public PrimitiveExpression(object value)
		{
			this.value = value;
		}

		public PrimitiveExpression(object value, LiteralFormat format)
		{
			this.value = value;
			this.Format = format;
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			PrimitiveExpression? o = other as PrimitiveExpression;
			return o != null && (this.Value == AnyValue || object.Equals(this.Value, o.Value));
		}
	}
}
