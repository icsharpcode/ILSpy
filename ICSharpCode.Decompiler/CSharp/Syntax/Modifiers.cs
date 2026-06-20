//
// Modifiers.cs
//
// Author:
//   Mike Krüger <mkrueger@novell.com>
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.Collections.Immutable;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[Flags]
	public enum Modifiers
	{
		None = 0,

		Private = 0x0001,
		Internal = 0x0002,
		Protected = 0x0004,
		Public = 0x0008,

		Abstract = 0x0010,
		Virtual = 0x0020,
		Sealed = 0x0040,
		Static = 0x0080,
		Override = 0x0100,
		Readonly = 0x0200,
		Const = 0x0400,
		New = 0x0800,
		Partial = 0x1000,

		Extern = 0x2000,
		Volatile = 0x4000,
		Unsafe = 0x8000,
		Async = 0x10000,
		Ref = 0x20000,
		Required = 0x40000,

		VisibilityMask = Private | Internal | Protected | Public,

		/// <summary>
		/// Special value used to match any modifiers during pattern matching.
		/// </summary>
		Any = unchecked((int)0x80000000)
	}

	public static class CSharpModifiers
	{
		// Not worth using a dictionary for such few elements.
		// This table is sorted in the order that modifiers should be output when generating code.
		public static ImmutableArray<Modifiers> AllModifiers { get; } = ImmutableArray.Create(
			Modifiers.Public, Modifiers.Private, Modifiers.Protected, Modifiers.Internal,
			Modifiers.New,
			Modifiers.Unsafe,
			Modifiers.Static, Modifiers.Abstract, Modifiers.Virtual, Modifiers.Sealed, Modifiers.Override,
			Modifiers.Required, Modifiers.Readonly, Modifiers.Volatile,
			Modifiers.Ref,
			Modifiers.Extern, Modifiers.Partial, Modifiers.Const,
			Modifiers.Async,
			Modifiers.Any
		);

		public static string GetModifierName(Modifiers modifier)
		{
			switch (modifier)
			{
				case Modifiers.Private:
					return "private";
				case Modifiers.Internal:
					return "internal";
				case Modifiers.Protected:
					return "protected";
				case Modifiers.Public:
					return "public";
				case Modifiers.Abstract:
					return "abstract";
				case Modifiers.Virtual:
					return "virtual";
				case Modifiers.Sealed:
					return "sealed";
				case Modifiers.Static:
					return "static";
				case Modifiers.Override:
					return "override";
				case Modifiers.Readonly:
					return "readonly";
				case Modifiers.Const:
					return "const";
				case Modifiers.New:
					return "new";
				case Modifiers.Partial:
					return "partial";
				case Modifiers.Extern:
					return "extern";
				case Modifiers.Volatile:
					return "volatile";
				case Modifiers.Unsafe:
					return "unsafe";
				case Modifiers.Async:
					return "async";
				case Modifiers.Ref:
					return "ref";
				case Modifiers.Required:
					return "required";
				case Modifiers.Any:
					// even though it's used for pattern matching only, 'any' needs to be in this list to be usable in the AST
					return "any";
				default:
					throw new NotSupportedException("Invalid value for Modifiers");
			}
		}
	}
}
