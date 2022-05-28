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
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Search
{
	class MetadataTokenSearchStrategy : AbstractEntitySearchStrategy
	{
		readonly EntityHandle searchTermToken;

		public MetadataTokenSearchStrategy(ILanguage language, ApiVisibility apiVisibility, SearchRequest request,
			IProducerConsumerCollection<SearchResult> resultQueue)
			: base(language, apiVisibility, request, resultQueue)
		{
			var terms = request.Keywords;
			if (terms.Length == 1)
			{
				int.TryParse(terms[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var token);
				searchTermToken = MetadataTokenHelpers.EntityHandleOrNil(token);
			}
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (searchTermToken.IsNil)
				return;
			var typeSystem = module.GetTypeSystemWithDecompilerSettingsOrNull(searchRequest.DecompilerSettings);
			if (typeSystem == null)
				return;
			var metadataModule = (MetadataModule)typeSystem.MainModule;
			int row = module.Metadata.GetRowNumber(searchTermToken);

			switch (searchTermToken.Kind)
			{
				case HandleKind.TypeDefinition:
					if (row < 1 || row > module.Metadata.TypeDefinitions.Count)
						break;
					var type = metadataModule.GetDefinition((TypeDefinitionHandle)searchTermToken);
					if (!CheckVisibility(type) || !IsInNamespaceOrAssembly(type))
						break;
					OnFoundResult(type);
					break;
				case HandleKind.MethodDefinition:
					if (row < 1 || row > module.Metadata.MethodDefinitions.Count)
						break;
					var method = metadataModule.GetDefinition((MethodDefinitionHandle)searchTermToken);
					if (!CheckVisibility(method) || !IsInNamespaceOrAssembly(method))
						break;
					OnFoundResult(method);
					break;
				case HandleKind.FieldDefinition:
					if (row < 1 || row > module.Metadata.FieldDefinitions.Count)
						break;
					var field = metadataModule.GetDefinition((FieldDefinitionHandle)searchTermToken);
					if (!CheckVisibility(field) || !IsInNamespaceOrAssembly(field))
						break;
					OnFoundResult(field);
					break;
				case HandleKind.PropertyDefinition:
					if (row < 1 || row > module.Metadata.PropertyDefinitions.Count)
						break;
					var property = metadataModule.GetDefinition((PropertyDefinitionHandle)searchTermToken);
					if (!CheckVisibility(property) || !IsInNamespaceOrAssembly(property))
						break;
					OnFoundResult(property);
					break;
				case HandleKind.EventDefinition:
					if (row < 1 || row > module.Metadata.EventDefinitions.Count)
						break;
					var @event = metadataModule.GetDefinition((EventDefinitionHandle)searchTermToken);
					if (!CheckVisibility(@event) || !IsInNamespaceOrAssembly(@event))
						break;
					OnFoundResult(@event);
					break;
			}
		}
	}
}
