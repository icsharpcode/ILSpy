// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.TextView
{
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public class ViewState : IEquatable<ViewState>
	{
		public HashSet<ILSpyTreeNode>? DecompiledNodes;
		public Uri? ViewedUri;

		public virtual bool Equals(ViewState? other)
		{
			return other != null
				&& ViewedUri == other.ViewedUri
				&& NullSafeSetEquals(DecompiledNodes, other.DecompiledNodes);

			static bool NullSafeSetEquals(HashSet<ILSpyTreeNode>? a, HashSet<ILSpyTreeNode>? b)
			{
				if (a == b)
					return true;
				if (a == null || b == null)
					return false;
				return a.SetEquals(b);
			}
		}

		protected virtual string GetDebuggerDisplay()
		{
			return $"Nodes = {DecompiledNodes?.Count.ToString() ?? "<null>"}, ViewedUri = {ViewedUri?.ToString() ?? "<null>"}";
		}
	}
}
