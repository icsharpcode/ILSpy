using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Search
{
	class ResourceSearchStrategy : AbstractSearchStrategy
	{
		protected readonly bool searchInside;
		protected readonly ApiVisibility apiVisibility;

		public ResourceSearchStrategy(ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, string term)
			: this(apiVisibility, resultQueue, new[] { term })
		{
		}

		public ResourceSearchStrategy(ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, string[] terms)
			: base(resultQueue, terms)
		{
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
			var resourcesNode = new ResourceListTreeNode(module);

			foreach (Resource resource in module.Resources)
				Search(module, resource, resourcesNode, ResourceTreeNode.Create(resource), cancellationToken);
		}

		void Search(PEFile module, Resource resource, SharpTreeNode parent, SharpTreeNode node, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (node is ResourceTreeNode treeNode) {
				if (!CheckVisibility(treeNode.Resource))
					return;
				resource = treeNode.Resource;
			}

			if (node.Text != null && IsMatch((string)node.Text))
				OnFoundResult(module, resource, node, parent);

			if (!searchInside)
				return;

			node.EnsureLazyChildren();
			foreach (var child in node.Children)
				Search(module, resource, node, child, cancellationToken);
		}

		void OnFoundResult(PEFile module, Resource resource, SharpTreeNode node, SharpTreeNode parent)
		{
			var name = (string)node.Text;
			var result = new ResourceSearchResult {
				Resource = resource,
				Fitness = 1.0f / name.Length,
				Image = (ImageSource)node.Icon,
				Name = name,
				LocationImage = (ImageSource)parent.Icon,
				Location = (string)parent.Text,
				Assembly = module.FullName,
				ToolTip = module.FileName,
			};
			OnFoundResult(result);
		}
	}
}
