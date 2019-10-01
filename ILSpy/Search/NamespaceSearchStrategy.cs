using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Search
{
	class NamespaceSearchStrategy : AbstractSearchStrategy
	{
		public NamespaceSearchStrategy(string term, IProducerConsumerCollection<SearchResult> resultQueue)
			: this(resultQueue, new[] { term })
		{
		}

		public NamespaceSearchStrategy(IProducerConsumerCollection<SearchResult> resultQueue, string[] terms)
			: base(resultQueue, terms)
		{
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null) return;

			var root = ((MetadataModule)typeSystem.MainModule).RootNamespace;
			Search(module, root);
		}

		private void Search(PEFile module, INamespace ns)
		{
			if (ns.Types.Any()) {
				if (IsMatch(ns.FullName.Length == 0 ? "-" : ns.FullName))
					OnFoundResult(module, ns);
			}

			foreach (var child in ns.ChildNamespaces)
				Search(module, child);
		}

		void OnFoundResult(PEFile module, INamespace ns)
		{
			var name = ns.FullName.Length == 0 ? "-" : ns.FullName;
			var result = new NamespaceSearchResult {
				Namespace = ns,
				Name = name,
				Fitness = 1.0f / name.Length,
				Location = module.Name,
				Assembly = module.FullName,
			};
			OnFoundResult(result);
		}
	}
}