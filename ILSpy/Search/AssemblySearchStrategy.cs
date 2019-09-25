using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
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
		readonly AssemblySearchKind searchKind;

		public AssemblySearchStrategy(string term, IProducerConsumerCollection<SearchResult> resultQueue, AssemblySearchKind searchKind)
			: this(resultQueue, new[] { term }, searchKind)
		{
		}

		public AssemblySearchStrategy(IProducerConsumerCollection<SearchResult> resultQueue, string[] terms, AssemblySearchKind searchKind)
			: base(resultQueue, terms)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (searchKind == AssemblySearchKind.NameOrFileName) {
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
			switch (kind) {
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

			switch (kind) {
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
