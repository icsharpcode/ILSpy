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

using System;
using System.Composition;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Resolves <c>metadata://</c> hyperlinks in the decompiler output to a metadata-tree
	/// node under the containing assembly. The reference's <see cref="Handle"/> picks
	/// which metadata table to surface; the user lands on the table itself and can
	/// scroll/select the specific row.
	/// </summary>
	[Export(typeof(IProtocolHandler))]
	[Shared]
	public sealed class MetadataProtocolHandler : IProtocolHandler
	{
		readonly AssemblyTreeModel assemblyTreeModel;

		[ImportingConstructor]
		public MetadataProtocolHandler(AssemblyTreeModel assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		public ILSpyTreeNode? Resolve(string protocol, MetadataFile module, Handle handle, out bool newTabPage)
		{
			newTabPage = true;
			if (protocol != "metadata")
				return null;
			// FindTreeNode resolves a MetadataFile to its assembly node, including one nested in a
			// package or bundle, where the node is a grandchild of a folder node rather than a
			// child of the root.
			if (assemblyTreeModel.FindTreeNode(module) is not AssemblyTreeNode assemblyNode)
				return null;
			assemblyNode.EnsureLazyChildren();
			var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().FirstOrDefault();
			if (metadataNode == null)
				return null;
			// Drill into the matching metadata table when the handle is table-backed. Heap
			// handles (String / UserString / Blob / Guid) have no per-table node, so we fall
			// back to the per-assembly Metadata folder — the user can expand the right heap
			// sibling from there.
			return (ILSpyTreeNode?)metadataNode.FindNodeByHandleKind(handle.Kind) ?? metadataNode;
		}
	}
}
