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

using System;
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
	}

	/// <summary>
	/// Represents a literal value.
	/// </summary>
	public class PrimitiveExpression : Expression
	{
		public static readonly object AnyValue = new object();
		
		TextLocation startLocation;
		public override TextLocation StartLocation {
			get {
				return startLocation;
			}
		}
		
		internal void SetLocation(TextLocation startLocation, TextLocation endLocation)
		{
			ThrowIfFrozen();
			this.startLocation = startLocation;
			this.endLocation = endLocation;
		}
		
		string literalValue;
		TextLocation? endLocation;
		public override TextLocation EndLocation {
			get {
				if (!endLocation.HasValue) {
					endLocation = value is string ? AdvanceLocation (StartLocation, literalValue ?? "") :
						new TextLocation (StartLocation.Line, StartLocation.Column + (literalValue ?? "").Length);
				}
				return endLocation.Value;
			}
		}
		
		object value;
		LiteralFormat format;

		public object Value {
			get { return this.value; }
			set {
				ThrowIfFrozen();
				this.value = value;
			}
		}
		
		public void SetValue(object value, string literalValue)
		{
			if (value == null)
				throw new ArgumentNullException();
			ThrowIfFrozen();
			this.value = value;
			this.literalValue = literalValue;
		}

		public LiteralFormat Format {
			get {  return format;}
			set {
				ThrowIfFrozen();
				format = value;
			}
		}

		public PrimitiveExpression (object value)
		{
			this.Value = value;
		}
		
		public PrimitiveExpression (object value, LiteralFormat format)
		{
			this.Value = value;
			this.format = format;
		}
		
		public override void AcceptVisitor (IAstVisitor visitor)
		{
			visitor.VisitPrimitiveExpression (this);
		}
		
		public override T AcceptVisitor<T> (IAstVisitor<T> visitor)
		{
			return visitor.VisitPrimitiveExpression (this);
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitPrimitiveExpression (this, data);
		}

		unsafe static TextLocation AdvanceLocation(TextLocation startLocation, string str)
		{
			int line = startLocation.Line;
			int col  = startLocation.Column;
			fixed (char* start = str) {
				char* p = start;
				char* endPtr = start + str.Length;
				while (p < endPtr) {
					var nl = NewLine.GetDelimiterLength(*p, () => {
						char* nextp = p + 1;
						if (nextp < endPtr)
							return *nextp;
						return '\0';
					});
					if (nl > 0) {
						line++;
						col = 1;
						if (nl == 2)
							p++;
					} else {
						col++;
					}
					p++;
				}
			}
			return new TextLocation (line, col);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			PrimitiveExpression o = other as PrimitiveExpression;
			return o != null && (this.Value == AnyValue || object.Equals(this.Value, o.Value));
		}
	}
}
