// 
// ComposedType.cs
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
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>array_type ::= non_array_type rank_specifier+</c> (C# grammar §8.2.1)
	/// <c>pointer_type ::= dataptr_type | funcptr_type | voidptr_type</c> (C# grammar §24.3.1)
	/// <c>nullable_reference_type ::= non_nullable_reference_type nullable_type_annotation</c> (C# grammar §8.2.1)
	/// <c>nullable_value_type ::= non_nullable_value_type nullable_type_annotation</c> (C# grammar §8.3.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ComposedType : AstType
	{
		public const string RefKeyword = "ref";
		public const string ReadonlyKeyword = "readonly";
		public const string NullableToken = "?";
		public const string PointerToken = "*";
		[Slot("AttributeSection")]
		public partial AstNodeCollection<AttributeSection> Attributes { get; }

		/// <summary>
		/// Gets/sets whether this type has a 'ref' specifier.
		/// This is used for C# 7 ref locals/ref return.
		/// Parameters use ParameterDeclaration.ParameterModifier instead.
		/// </summary>
		public bool HasRefSpecifier { get; set; }

		/// <summary>
		/// Gets/sets whether this type has a 'readonly' specifier.
		/// This is used for C# 7.2 'ref readonly' locals/ref return.
		/// Parameters use ParameterDeclaration.ParameterModifier instead.
		/// </summary>
		public bool HasReadOnlySpecifier { get; set; }

		[Slot("Type")]
		public partial AstType BaseType { get; set; }

		public bool HasNullableSpecifier { get; set; }

		// Non-negativity is verified by CheckInvariant rather than an eager setter guard.
		public int PointerRank { get; set; }

		[Slot("ArraySpecifier")]
		public partial AstNodeCollection<ArraySpecifier> ArraySpecifiers { get; }

		public override string ToString(CSharpFormattingOptions? formattingOptions)
		{
			StringBuilder b = new StringBuilder();
			if (this.HasRefSpecifier)
				b.Append("ref ");
			if (this.HasReadOnlySpecifier)
				b.Append("readonly ");
			b.Append(this.BaseType.ToString());
			if (this.HasNullableSpecifier)
				b.Append('?');
			b.Append('*', this.PointerRank);
			foreach (var arraySpecifier in this.ArraySpecifiers)
			{
				b.Append('[');
				b.Append(',', arraySpecifier.Dimensions - 1);
				b.Append(']');
			}
			return b.ToString();
		}

		public override AstType MakePointerType()
		{
			if (ArraySpecifiers.Any())
			{
				return base.MakePointerType();
			}
			else
			{
				this.PointerRank++;
				return this;
			}
		}

		public override AstType MakeArrayType(int dimensions)
		{
			InsertChildBefore(this.ArraySpecifiers.FirstOrDefault(), new ArraySpecifier(dimensions), Slots.ArraySpecifier);
			return this;
		}

		public override AstType MakeRefType()
		{
			this.HasRefSpecifier = true;
			return this;
		}

		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			// PointerRank is the number of '*' specifiers; a transform that corrupts it
			// (e.g. an unbalanced decrement) is caught here at the offending transform.
			Debug.Assert(PointerRank >= 0, "ComposedType.PointerRank must not be negative");
		}
	}

	/// <summary>
	/// <c>rank_specifier ::= '[' ','* ']'</c> (C# grammar §8.2.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ArraySpecifier : AstNode
	{
		public ArraySpecifier()
		{
		}

		public ArraySpecifier(int dimensions)
		{
			this.Dimensions = dimensions;
		}

		public int Dimensions { get; set; } = 1;

		public override string ToString(CSharpFormattingOptions? formattingOptions)
		{
			return "[" + new string(',', this.Dimensions - 1) + "]";
		}

		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			// A rank specifier always has at least one dimension ('[]' is rank 1); the output relies on it.
			Debug.Assert(Dimensions >= 1, "ArraySpecifier.Dimensions must be at least 1");
		}
	}
}
