// Copyright (c) 2026 Siegfried Pammer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Semantics
{
	/// <summary>
	/// Represents a typeless default literal `default`, which is implicitly convertible
	/// to any type (C# standard 10.2.16 default literal conversions).
	/// </summary>
	class DefaultLiteralResolveResult : ResolveResult
	{
		/// <summary>
		/// The type of the "default(T)" expression the literal was shortened from; it is restored
		/// wherever the surrounding syntax stops supplying that very type.
		/// <see cref="SpecialType.UnknownType"/> if the literal does not stand for a shortened
		/// expression.
		/// </summary>
		public readonly IType ShortenedFrom;

		public DefaultLiteralResolveResult() : this(SpecialType.UnknownType)
		{
		}

		public DefaultLiteralResolveResult(IType shortenedFrom) : base(SpecialType.NoType)
		{
			this.ShortenedFrom = shortenedFrom;
		}

		// A default_value_expression is a constant expression (C# standard 12.8.21);
		// like the null literal, the typeless default literal is modeled as a constant
		// with value null (the target-typed value is only known after conversion).
		public override bool IsCompileTimeConstant {
			get { return true; }
		}

		public override object ConstantValue {
			get { return null; }
		}
	}
}
