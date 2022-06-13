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
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Search
{
	public class ResourceSearchStrategy : AbstractSearchStrategy
	{
		protected readonly bool searchInside;
		protected readonly ApiVisibility apiVisibility;
		protected readonly ITreeNodeFactory treeNodeFactory;

		public ResourceSearchStrategy(ApiVisibility apiVisibility, SearchRequest request, IProducerConsumerCollection<SearchResult> resultQueue)
			: base(request, resultQueue)
		{
			this.treeNodeFactory = request.TreeNodeFactory;
			this.apiVisibility = apiVisibility;
			this.searchInside = true;
		}

		protected bool CheckVisibility(Resource resource)
		{
			if (apiVisibility == ApiVisibility.All)
				return true;

			if (apiVisibility == ApiVisibility.PublicOnly && (resource.Attributes & ManifestResourceAttributes.VisibilityMask) == ManifestResourceAttributes.Private)
				return false;

			return true;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var resourcesNode = treeNodeFactory.CreateResourcesList(module);

			foreach (Resource resource in module.Resources)
				Search(module, resource, resourcesNode, treeNodeFactory.Create(resource), cancellationToken);
		}

		void Search(PEFile module, Resource resource, ITreeNode parent, ITreeNode node, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (node is IResourcesFileTreeNode treeNode)
			{
				if (!CheckVisibility(treeNode.Resource))
					return;
				resource = treeNode.Resource;
			}

			if (node.Text is string s && IsMatch(s))
				OnFoundResult(module, resource, node, parent);

			if (!searchInside)
				return;

			node.EnsureLazyChildren();
			foreach (var child in node.Children)
				Search(module, resource, node, child, cancellationToken);
		}

		void OnFoundResult(PEFile module, Resource resource, ITreeNode node, ITreeNode parent)
		{
			OnFoundResult(searchRequest.SearchResultFactory.Create(module, resource, node, parent));
		}
	}
}
