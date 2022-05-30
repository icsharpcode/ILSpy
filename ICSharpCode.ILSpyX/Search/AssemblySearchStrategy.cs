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
using System.IO;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX.Search
{
	class AssemblySearchStrategy : AbstractSearchStrategy
	{
		readonly AssemblySearchKind searchKind;

		public AssemblySearchStrategy(SearchRequest request,
			IProducerConsumerCollection<SearchResult> resultQueue, AssemblySearchKind searchKind)
			: base(request, resultQueue)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (searchKind == AssemblySearchKind.NameOrFileName)
			{
				string localName = GetNameToMatch(module, AssemblySearchKind.Name);
				string fileName = Path.GetFileName(GetNameToMatch(module, AssemblySearchKind.FilePath));
				if (IsMatch(localName) || IsMatch(fileName))
					OnFoundResult(module);
				return;
			}

			string name = GetNameToMatch(module, searchKind);
			if (IsMatch(name))
				OnFoundResult(module);
		}

		string GetNameToMatch(PEFile module, AssemblySearchKind kind)
		{
			switch (kind)
			{
				case AssemblySearchKind.FullName:
					return module.FullName;
				case AssemblySearchKind.Name:
					return module.Name;
				case AssemblySearchKind.FilePath:
					return module.FileName;
			}

			if (!module.IsAssembly)
				return null;

			var metadata = module.Metadata;
			var definition = module.Metadata.GetAssemblyDefinition();

			switch (kind)
			{
				case AssemblySearchKind.Culture:
					if (definition.Culture.IsNil)
						return "neutral";
					return metadata.GetString(definition.Culture);
				case AssemblySearchKind.Version:
					return definition.Version.ToString();
				case AssemblySearchKind.PublicKey:
					return module.Metadata.GetPublicKeyToken();
				case AssemblySearchKind.HashAlgorithm:
					return definition.HashAlgorithm.ToString();
				case AssemblySearchKind.Flags:
					return definition.Flags.ToString();
			}

			return null;
		}

		void OnFoundResult(PEFile module)
		{
			OnFoundResult(searchRequest.SearchResultFactory.Create(module));
		}
	}

	enum AssemblySearchKind
	{
		NameOrFileName,
		Name,
		FullName,
		FilePath,
		Culture,
		Version,
		PublicKey,
		HashAlgorithm,
		Flags
	}
}
