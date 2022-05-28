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

using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpyX.Search
{
	class NamespaceSearchStrategy : AbstractSearchStrategy
	{
		public NamespaceSearchStrategy(SearchRequest request, IProducerConsumerCollection<SearchResult> resultQueue)
			: base(request, resultQueue)
		{
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var typeSystem = module.GetTypeSystemWithDecompilerSettingsOrNull(searchRequest.DecompilerSettings);
			if (typeSystem == null)
				return;

			var root = ((MetadataModule)typeSystem.MainModule).RootNamespace;
			Search(module, root);
		}

		private void Search(PEFile module, INamespace ns)
		{
			if (ns.Types.Any())
			{
				if (IsMatch(ns.FullName.Length == 0 ? "-" : ns.FullName))
					OnFoundResult(module, ns);
			}

			foreach (var child in ns.ChildNamespaces)
				Search(module, child);
		}

		void OnFoundResult(PEFile module, INamespace ns)
		{
			OnFoundResult(searchRequest.SearchResultFactory.Create(module, ns));
		}
	}
}