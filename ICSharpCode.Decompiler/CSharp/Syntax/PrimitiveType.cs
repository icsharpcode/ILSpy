// 
// FullTypeName.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>predefined_type ::= 'bool' | 'byte' | 'char' | 'decimal' | 'double' | 'float' | 'int' | 'long' | 'object' | 'sbyte' | 'short' | 'string' | 'uint' | 'ulong' | 'ushort'</c> (C# grammar §12.8.7.1)
	/// <c>simple_type ::= numeric_type | 'bool'</c> (C# grammar §8.3.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class PrimitiveType : AstType
	{
		string keyword = string.Empty;

		public string Keyword {
			get { return keyword; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				keyword = value;
			}
		}

		// Derived from Keyword (which DoMatch already compares); exclude it so the match
		// doesn't redundantly re-run the keyword switch.
		[ExcludeFromMatch]
		public KnownTypeCode KnownTypeCode {
			get { return GetTypeCodeForPrimitiveType(this.Keyword); }
		}

		public PrimitiveType()
		{
		}

		public PrimitiveType(string keyword)
		{
			this.Keyword = keyword;
		}

		public PrimitiveType(string keyword, TextLocation location)
		{
			this.Keyword = keyword;
			StorePrintStart(location);
		}

		// StartLocation comes from the base (stored at print time); only the end needs deriving.
		public override TextLocation EndLocation {
			get {
				return new TextLocation(StartLocation.Line, StartLocation.Column + keyword.Length);
			}
		}

		public override string ToString(CSharpFormattingOptions? formattingOptions)
		{
			return Keyword;
		}

		public static KnownTypeCode GetTypeCodeForPrimitiveType(string keyword)
		{
			switch (keyword)
			{
				case "string":
					return KnownTypeCode.String;
				case "int":
					return KnownTypeCode.Int32;
				case "uint":
					return KnownTypeCode.UInt32;
				case "object":
					return KnownTypeCode.Object;
				case "bool":
					return KnownTypeCode.Boolean;
				case "sbyte":
					return KnownTypeCode.SByte;
				case "byte":
					return KnownTypeCode.Byte;
				case "short":
					return KnownTypeCode.Int16;
				case "ushort":
					return KnownTypeCode.UInt16;
				case "long":
					return KnownTypeCode.Int64;
				case "ulong":
					return KnownTypeCode.UInt64;
				case "float":
					return KnownTypeCode.Single;
				case "double":
					return KnownTypeCode.Double;
				case "decimal":
					return KnownTypeCode.Decimal;
				case "char":
					return KnownTypeCode.Char;
				case "void":
					return KnownTypeCode.Void;
				default:
					return KnownTypeCode.None;
			}
		}
	}
}

