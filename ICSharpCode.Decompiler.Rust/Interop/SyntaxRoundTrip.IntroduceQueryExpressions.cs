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

using System;
using System.IO;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answer and annotation kinds of the engine's <c>IntroduceQueryExpressions</c>
	/// (<c>csharp/transforms/introduce_query_expressions.rs</c>, which documents each kind).
	/// Variables travel as the object ids of <c>SyntaxRoundTrip.DeclareVariables.cs</c>.
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte QueryQueryExpressionsAnnotation = 154;

		const byte AnnotationQueryGroupClause = 154;
		const byte AnnotationQueryJoinClause = 155;

		byte[]? AnswerQueryExpressionsAnnotation(byte[] query)
		{
			int pos = 1;
			int handle = (int)ReadUVar(query, ref pos);
			if (handle < 0 || handle >= handles.Count)
				return null;
			object target = handles[handle];
			int flags = (target is ResolveResult ? 1 : 0)
				| (target is DecompiledLambdaResolveResult { AttemptedConversionWithTypeMismatch: true } ? 2 : 0)
				| (target is ILVariableResolveResult ? 4 : 0)
				| (target is MemberResolveResult ? 8 : 0)
				| (target is ILFunction ? 16 : 0);
			var s = new MemoryStream();
			s.WriteByte((byte)flags);
			if (target is ILVariableResolveResult { Variable: var v })
			{
				UVar(s, (ulong)ObjectId(v));
				string name = v.Name ?? string.Empty;
				UVar(s, (ulong)name.Length);
				WriteUtf16(s, name);
			}
			return s.ToArray();
		}

		object? CreateQueryExpressionsAnnotation(byte kind, ArraySegment<byte> payload)
		{
			if (kind is not (AnnotationQueryGroupClause or AnnotationQueryJoinClause))
				return null;
			byte[] b = payload.Array!;
			int pos = payload.Offset;
			var first = FunctionAt(b, ref pos);
			var second = FunctionAt(b, ref pos);
			return kind == AnnotationQueryGroupClause
				? new QueryGroupClauseAnnotation(first!, second!)
				: new QueryJoinClauseAnnotation(first!, second!);
		}

		/// <summary>The <c>ILFunction</c> of a handle plus one, null for 0.</summary>
		ILFunction? FunctionAt(byte[] b, ref int pos)
		{
			int h = (int)ReadUVar(b, ref pos);
			return h == 0 ? null : (ILFunction)Lookup(h - 1);
		}
	}
}
