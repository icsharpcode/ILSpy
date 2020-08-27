// Copyright (c) 2018 Daniel Grunwald
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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Semantics
{
	/// <summary>
	/// Resolve result for a C# 7 tuple literal.
	/// </summary>
	public class TupleResolveResult : ResolveResult
	{
		public ImmutableArray<ResolveResult> Elements { get; }

		public TupleResolveResult(ICompilation compilation,
			ImmutableArray<ResolveResult> elements,
			ImmutableArray<string> elementNames = default(ImmutableArray<string>),
			IModule valueTupleAssembly = null)
		: base(GetTupleType(compilation, elements, elementNames, valueTupleAssembly))
		{
			this.Elements = elements;
		}

		public override IEnumerable<ResolveResult> GetChildResults()
		{
			return Elements;
		}

		static IType GetTupleType(ICompilation compilation, ImmutableArray<ResolveResult> elements, ImmutableArray<string> elementNames, IModule valueTupleAssembly)
		{
			if (elements.Any(e => e.Type.Kind == TypeKind.None || e.Type.Kind == TypeKind.Null))
				return SpecialType.NoType;
			else
				return new TupleType(compilation, elements.Select(e => e.Type).ToImmutableArray(), elementNames, valueTupleAssembly);
		}
	}
}
