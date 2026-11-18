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

using ICSharpCode.Decompiler.CSharp.Transforms;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The annotation kind of the engine's <c>CombineQueryExpressions</c>
	/// (<c>csharp/transforms/combine_query_expressions.rs</c>); its queries are those of
	/// <c>SyntaxRoundTrip.IntroduceQueryExpressions.cs</c>.
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte AnnotationLetIdentifier = 168;

		static object? CreateCombineQueryExpressionsAnnotation(byte kind)
			=> kind == AnnotationLetIdentifier ? new LetIdentifierAnnotation() : null;
	}
}
