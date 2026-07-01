// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.DebugSteps;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Maps syntax tree nodes to character ranges in the rendered text.
	/// </summary>
	internal sealed class NodeLookup
	{
		readonly Dictionary<object, TextRange> nodes = new(ReferenceEqualityComparer.Instance);

		public bool TryGetRange(object node, out TextRange range)
			=> nodes.TryGetValue(node, out range);

		public void AddNode(object node, int start, int length)
		{
			if (length <= 0)
				return;
			var range = new TextRange(start, length);
			nodes[node] = range;
			// Bridge only the debug-step marker: it is the sole annotation the resolver ever looks up
			// (it rides CopyAnnotationsFrom so a replaced node's step still resolves to the emitted
			// text). Indexing every annotation would add dead keys never queried, and would let a
			// shared annotation resolve to whichever node rendered last.
			if (node is IAnnotatable annotatable)
			{
				foreach (var annotation in annotatable.Annotations)
				{
					if (annotation is DebugStepMarker)
						nodes[annotation] = range;
				}
			}
		}
	}
}
