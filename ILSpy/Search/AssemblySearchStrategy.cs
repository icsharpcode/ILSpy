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
	class AssemblySearchStrategy : AbstractSearchStrategy
	{
		public AssemblySearchStrategy(IProducerConsumerCollection<SearchResult> resultQueue, string term)
			: this(resultQueue, new[] { term })
		{
		}

		public AssemblySearchStrategy(IProducerConsumerCollection<SearchResult> resultQueue, string[] terms)
			: base(resultQueue, terms)
		{
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (IsMatch(module.FullName))
				OnFoundResult(module);
		}

		void OnFoundResult(PEFile module)
		{
			var result = new AssemblySearchResult {
				Module = module,
				Fitness = 1.0f / module.Name.Length,
				Name = module.Name,
				Location = module.FileName,
				Assembly = module.FullName,
				ToolTip = module.FileName,
			};
			OnFoundResult(result);
		}
	}
}
