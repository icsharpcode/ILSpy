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
				
			resourcesNode.EnsureLazyChildren();
			foreach (var node in resourcesNode.Children)
				Search(module, null, resourcesNode, node, cancellationToken);
			return;
		}

		void Search(PEFile module, object reference, SharpTreeNode parent, SharpTreeNode node, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (node is ResourceTreeNode treeNode) {
				if (!CheckVisibility(treeNode.Resource))
					return;
				reference = treeNode.Resource;
			}

			if (node.Text != null && IsMatch((string)node.Text))
				OnFoundResult(module, reference, node, parent);

			if (!searchInside)
				return;

			node.EnsureLazyChildren();
			foreach (var child in node.Children)
				Search(module, reference, node, child, cancellationToken);
		}

		void OnFoundResult(PEFile module, object reference, SharpTreeNode node, SharpTreeNode parent)
		{
			var result = new SearchResult {
				Reference = reference,
				Fitness = 1f,
				Image = (ImageSource)node.Icon,
				Name = (string)node.Text,
				LocationImage = (ImageSource)parent.Icon,
				Location = (string)parent.Text,
				AssemblyImage = Images.Assembly,
				Assembly = module.Name,
				ToolTip = module.FileName,
			};
			OnFoundResult(result);
		}
	}
}
