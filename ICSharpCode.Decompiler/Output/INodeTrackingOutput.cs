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

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Optional capability of an <see cref="ITextOutput"/> that records the rendered text span
	/// of written nodes, so a node can later be mapped back to its character range (used for
	/// debug-step highlighting). Outputs that don't track nodes simply don't implement this.
	/// </summary>
	public interface INodeTrackingOutput
	{
		/// <summary>Marks the start offset of <paramref name="node"/>'s rendered span.</summary>
		void MarkNodeStart(object node);

		/// <summary>
		/// Marks the end of <paramref name="node"/>'s rendered span; pairs with the most recent
		/// <see cref="MarkNodeStart"/> for the same node.
		/// </summary>
		void MarkNodeEnd(object node);
	}

	public static class NodeTrackingOutputExtensions
	{
		/// <summary>
		/// Records the start of <paramref name="node"/>'s rendered span when <paramref name="output"/>
		/// supports node tracking; a cheap no-op otherwise. Safe to call unconditionally from any
		/// node-writing code path.
		/// </summary>
		public static void MarkNodeStart(this ITextOutput output, object node)
		{
			(output as INodeTrackingOutput)?.MarkNodeStart(node);
		}

		/// <summary>
		/// Records the end of <paramref name="node"/>'s rendered span when <paramref name="output"/>
		/// supports node tracking; a cheap no-op otherwise. Must pair with <see cref="MarkNodeStart"/>.
		/// </summary>
		public static void MarkNodeEnd(this ITextOutput output, object node)
		{
			(output as INodeTrackingOutput)?.MarkNodeEnd(node);
		}
	}
}
