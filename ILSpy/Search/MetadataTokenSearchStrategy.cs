using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Search
{
	class MetadataTokenSearchStrategy : AbstractEntitySearchStrategy
	{
		readonly EntityHandle searchTermToken;

		public MetadataTokenSearchStrategy(Language language, ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, params string[] terms)
			: base(language, apiVisibility, resultQueue, terms)
		{
			if (terms.Length == 1) {
				int.TryParse(terms[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var token);
				searchTermToken = MetadataTokenHelpers.EntityHandleOrNil(token);
			}
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (searchTermToken.IsNil) return;
			var typeSystem = module.GetTypeSystemWithCurrentOptionsOrNull();
			if (typeSystem == null) return;
			var metadataModule = (MetadataModule)typeSystem.MainModule;
			int row = module.Metadata.GetRowNumber(searchTermToken);

			switch (searchTermToken.Kind) {
				case HandleKind.TypeDefinition:
					if (row < 1 || row > module.Metadata.TypeDefinitions.Count)
						break;
					var type = metadataModule.GetDefinition((TypeDefinitionHandle)searchTermToken);
					if (!CheckVisibility(type)) break;
					OnFoundResult(type);
					break;
				case HandleKind.MethodDefinition:
					if (row < 1 || row > module.Metadata.MethodDefinitions.Count)
						break;
					var method = metadataModule.GetDefinition((MethodDefinitionHandle)searchTermToken);
					if (!CheckVisibility(method)) break;
					OnFoundResult(method);
					break;
				case HandleKind.FieldDefinition:
					if (row < 1 || row > module.Metadata.FieldDefinitions.Count)
						break;
					var field = metadataModule.GetDefinition((FieldDefinitionHandle)searchTermToken);
					if (!CheckVisibility(field)) break;
					OnFoundResult(field);
					break;
				case HandleKind.PropertyDefinition:
					if (row < 1 || row > module.Metadata.PropertyDefinitions.Count)
						break;
					var property = metadataModule.GetDefinition((PropertyDefinitionHandle)searchTermToken);
					if (!CheckVisibility(property)) break;
					OnFoundResult(property);
					break;
				case HandleKind.EventDefinition:
					if (row < 1 || row > module.Metadata.EventDefinitions.Count)
						break;
					var @event = metadataModule.GetDefinition((EventDefinitionHandle)searchTermToken);
					if (!CheckVisibility(@event)) break;
					OnFoundResult(@event);
					break;
			}
		}
	}
}
