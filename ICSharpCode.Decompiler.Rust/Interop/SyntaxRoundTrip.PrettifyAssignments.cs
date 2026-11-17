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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answers of the engine's <c>PrettifyAssignments</c>
	/// (<c>csharp/transforms/prettify_assignments.rs</c>, which documents each kind). Each query
	/// carries the annotation handles of one or two nodes, each list as a count and the handles.
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte PrettifyAssignmentsFirstQuery = 126;
		const byte QueryPrettifyImplicitlyConvertible = 126;
		const byte QueryPrettifyConstantOne = 127;
		const byte QueryPrettifyCustomOperator = 128;
		const byte PrettifyAssignmentsLastQuery = 139;

		byte[]? AnswerPrettifyAssignments(byte[] query)
		{
			int pos = 1;
			switch (query[0])
			{
				case QueryPrettifyImplicitlyConvertible:
				{
					if (context == null)
						return null;
					var rhs = ResolveResultOf(ReadAnnotations(query, ref pos));
					var expectedType = ResolveResultOf(ReadAnnotations(query, ref pos)).Type;
					var conversions = CSharpConversions.Get(context.TypeSystem);
					return new[] { conversions.ImplicitConversion(rhs, expectedType).IsImplicit ? (byte)1 : (byte)0 };
				}
				case QueryPrettifyConstantOne:
				{
					var rr = ResolveResultOf(ReadAnnotations(query, ref pos));
					bool one = rr.IsCompileTimeConstant
						&& (rr.Type.IsCSharpPrimitiveIntegerType() || rr.Type.IsKnownType(KnownTypeCode.Single) || rr.Type.IsKnownType(KnownTypeCode.Double))
						&& CSharpPrimitiveCast.Cast(rr.Type.GetTypeCode(), 1, false).Equals(rr.ConstantValue);
					return new[] { one ? (byte)1 : (byte)0 };
				}
				case QueryPrettifyCustomOperator:
				{
					var annotations = ReadAnnotations(query, ref pos);
					bool custom = annotations.Any(a => a is IL.CallInstruction or IL.UserDefinedCompoundAssign or IL.DynamicCompoundAssign);
					return new[] { custom ? (byte)1 : (byte)0 };
				}
				default:
					return null;
			}
		}

		/// <summary>The host objects of a list of annotation handles in a query.</summary>
		List<object> ReadAnnotations(byte[] query, ref int pos)
		{
			int count = (int)ReadUVar(query, ref pos);
			var result = new List<object>(count);
			for (int i = 0; i < count; i++)
			{
				int handle = (int)ReadUVar(query, ref pos);
				if (handle < handles.Count)
					result.Add(handles[handle]);
			}
			return result;
		}

		/// <summary><c>AstNode.GetResolveResult</c> of a node with <paramref name="annotations"/>.</summary>
		static ResolveResult ResolveResultOf(List<object> annotations)
		{
			return annotations.OfType<ResolveResult>().FirstOrDefault() ?? ErrorResolveResult.UnknownError;
		}
	}
}
