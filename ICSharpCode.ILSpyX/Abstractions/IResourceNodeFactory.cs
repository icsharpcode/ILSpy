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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpyX.Abstractions
{
	/// <summary>
	/// This interface allows plugins to create custom nodes for resources.
	/// </summary>
	public interface IResourceNodeFactory
	{
		// Null means "this factory doesn't claim the resource"; callers iterate factories until
		// one returns non-null and otherwise fall back to a generic node.
		ITreeNode? CreateNode(Resource resource);

		/// <summary>
		/// Builds a node for a serialized-object entry inside an embedded <c>.resources</c>
		/// file (e.g. an <c>ImageListStreamer</c> blob). Default no-op — only factories that
		/// understand a specific serialized payload type need to override this.
		/// </summary>
		ITreeNode? CreateNode(string key, ResourceSerializedObject serializedObject) => null;
	}
}
