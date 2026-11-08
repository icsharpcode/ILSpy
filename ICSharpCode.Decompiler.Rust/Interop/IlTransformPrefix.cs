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
using System.Reflection;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The IL transform pipeline flattened to its leaves in run order: a
	/// <see cref="BlockILTransform"/> contributes its pre-order and post-order transforms, a
	/// <see cref="StatementTransform"/> its children, any other transform itself. An IL prefix
	/// <c>N</c> is the first <c>N</c> leaves.
	/// </summary>
	internal static class IlTransformPrefix
	{
		static readonly FieldInfo statementChildren =
			typeof(StatementTransform).GetField("children", BindingFlags.Instance | BindingFlags.NonPublic)!;

		internal static IStatementTransform[] Children(StatementTransform transform)
			=> (IStatementTransform[])statementChildren.GetValue(transform)!;

		/// <summary>The leaves of one transform, in run order.</summary>
		public static IEnumerable<object> Leaves(object transform) => transform switch {
			BlockILTransform block => block.PreOrderTransforms.Concat(block.PostOrderTransforms).SelectMany(Leaves),
			StatementTransform statement => Children(statement),
			_ => [transform],
		};

		/// <summary>
		/// The largest prefix of at most <paramref name="prefix"/> leaves that ends between two
		/// top-level transforms of <paramref name="pipeline"/>, and the number of top-level
		/// transforms it covers. Only such a prefix can be handed over: a block transform group
		/// interleaves its leaves per block, so its leaves cannot be split between two engines.
		/// </summary>
		public static int TopLevelBoundary(IReadOnlyList<IILTransform> pipeline, int prefix, out int topLevelCount)
		{
			int leaves = 0;
			topLevelCount = 0;
			foreach (var transform in pipeline)
			{
				int next = leaves + Leaves(transform).Count();
				if (next > prefix)
					break;
				leaves = next;
				topLevelCount++;
			}
			return leaves;
		}

		/// <summary>
		/// Whether the first <paramref name="topLevelCount"/> transforms of
		/// <paramref name="pipeline"/> have the leaves of the standard pipeline
		/// (<see cref="CSharp.CSharpDecompiler.GetILTransforms"/>), the one the engine ports.
		/// </summary>
		public static bool MatchesStandardPipeline(IReadOnlyList<IILTransform> pipeline, int topLevelCount)
		{
			static IEnumerable<string> Names(IEnumerable<IILTransform> transforms, int count)
				=> transforms.Take(count).SelectMany(Leaves).Select(t => t.GetType().FullName!);
			return Names(pipeline, topLevelCount).SequenceEqual(Names(CSharp.CSharpDecompiler.GetILTransforms(), topLevelCount));
		}
	}
}
