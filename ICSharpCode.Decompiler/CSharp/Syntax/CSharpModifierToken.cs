// 
// CSharpModifierToken.cs
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

using System;
using System.Collections.Immutable;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public class CSharpModifierToken : CSharpTokenNode
	{
		Modifiers modifier;

		public Modifiers Modifier {
			get { return modifier; }
			set {
				ThrowIfFrozen();
				this.modifier = value;
			}
		}

		public override TextLocation EndLocation {
			get {
				return new(StartLocation.Line, StartLocation.Column + GetModifierLength(Modifier));
			}
		}

		public override string ToString(CSharpFormattingOptions formattingOptions)
		{
			return GetModifierName(Modifier);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			CSharpModifierToken o = other as CSharpModifierToken;
			return o != null && this.modifier == o.modifier;
		}

		// Not worth using a dictionary for such few elements.
		// This table is sorted in the order that modifiers should be output when generating code.
		public static ImmutableArray<Modifiers> AllModifiers { get; } = ImmutableArray.Create(
			Modifiers.Public, Modifiers.Private, Modifiers.Protected, Modifiers.Internal,
			Modifiers.New,
			Modifiers.Unsafe,
			Modifiers.Abstract, Modifiers.Virtual, Modifiers.Sealed, Modifiers.Static, Modifiers.Override,
			Modifiers.Readonly, Modifiers.Volatile,
			Modifiers.Ref,
			Modifiers.Extern, Modifiers.Partial, Modifiers.Const,
			Modifiers.Async,
			Modifiers.Any
		);

		public CSharpModifierToken(TextLocation location, Modifiers modifier) : base(location, null)
		{
			this.Modifier = modifier;
		}

		public static string GetModifierName(Modifiers modifier)
		{
			return modifier switch {
				Modifiers.Private => "private",
				Modifiers.Internal => "internal",
				Modifiers.Protected => "protected",
				Modifiers.Public => "public",
				Modifiers.Abstract => "abstract",
				Modifiers.Virtual => "virtual",
				Modifiers.Sealed => "sealed",
				Modifiers.Static => "static",
				Modifiers.Override => "override",
				Modifiers.Readonly => "readonly",
				Modifiers.Const => "const",
				Modifiers.New => "new",
				Modifiers.Partial => "partial",
				Modifiers.Extern => "extern",
				Modifiers.Volatile => "volatile",
				Modifiers.Unsafe => "unsafe",
				Modifiers.Async => "async",
				Modifiers.Ref => "ref",
				Modifiers.Any =>
					// even though it's used for pattern matching only, 'any' needs to be in this list to be usable in the AST
					"any",
				_ => throw new NotSupportedException("Invalid value for Modifiers")
			};
		}

		public static int GetModifierLength(Modifiers modifier)
		{
			return modifier switch {
				Modifiers.Private => "private".Length,
				Modifiers.Internal => "internal".Length,
				Modifiers.Protected => "protected".Length,
				Modifiers.Public => "public".Length,
				Modifiers.Abstract => "abstract".Length,
				Modifiers.Virtual => "virtual".Length,
				Modifiers.Sealed => "sealed".Length,
				Modifiers.Static => "static".Length,
				Modifiers.Override => "override".Length,
				Modifiers.Readonly => "readonly".Length,
				Modifiers.Const => "const".Length,
				Modifiers.New => "new".Length,
				Modifiers.Partial => "partial".Length,
				Modifiers.Extern => "extern".Length,
				Modifiers.Volatile => "volatile".Length,
				Modifiers.Unsafe => "unsafe".Length,
				Modifiers.Async => "async".Length,
				Modifiers.Ref => "ref".Length,
				Modifiers.Any =>
					// even though it's used for pattern matching only, 'any' needs to be in this list to be usable in the AST
					"any".Length,
				_ => throw new NotSupportedException("Invalid value for Modifiers")
			};
		}

		public static Modifiers GetModifierValue(string modifier)
		{
			return modifier switch {
				"private" => Modifiers.Private,
				"internal" => Modifiers.Internal,
				"protected" => Modifiers.Protected,
				"public" => Modifiers.Public,
				"abstract" => Modifiers.Abstract,
				"virtual" => Modifiers.Virtual,
				"sealed" => Modifiers.Sealed,
				"static" => Modifiers.Static,
				"override" => Modifiers.Override,
				"readonly" => Modifiers.Readonly,
				"const" => Modifiers.Const,
				"new" => Modifiers.New,
				"partial" => Modifiers.Partial,
				"extern" => Modifiers.Extern,
				"volatile" => Modifiers.Volatile,
				"unsafe" => Modifiers.Unsafe,
				"async" => Modifiers.Async,
				"ref" => Modifiers.Ref,
				"any" =>
					// even though it's used for pattern matching only, 'any' needs to be in this list to be usable in the AST
					Modifiers.Any,
				_ => throw new NotSupportedException("Invalid value for Modifiers")
			};
		}
	}
}