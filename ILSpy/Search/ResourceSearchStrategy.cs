using System.Collections.Concurrent;
using System.Threading;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Search
{
	class ResourceSearchStrategy : AbstractSearchStrategy
	{
		public ResourceSearchStrategy(Language language, ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, params string[] terms)
			: base(language, apiVisibility, resultQueue, terms)
		{

		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			foreach (var resource in module.Resources) {
				if (IsMatch(resource.Name)) {
					OnFoundResult(resource);
				}
			}
		}
	}
}
