// Copyright (c) 2026 Christoph Wille
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

#nullable enable

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answer of the engine's <c>FixNameCollisions</c>
	/// (<c>csharp/transforms/fix_name_collisions.rs</c>, which documents the kind).
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte QueryPrivateField = 224;

		/// <summary>The answer to a query of <c>FixNameCollisions</c>; null where it does not apply.</summary>
		byte[]? AnswerFixNameCollisions(byte[] query)
		{
			int pos = 1;
			int id = (int)ReadUVar(query, ref pos);
			if (id < 0 || id >= symbols.Count)
				return null;
			return new[] { symbols[id] is IField { Accessibility: Accessibility.Private } ? (byte)1 : (byte)0 };
		}
	}
}
