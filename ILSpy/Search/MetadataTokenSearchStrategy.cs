using System;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Search
{
	class MetadataTokenSearchStrategy : AbstractSearchStrategy
	{
		readonly EntityHandle searchTermToken;

		public MetadataTokenSearchStrategy(Language language, Action<SearchResult> addResult, params string[] terms)
			: base(language, addResult, terms)
		{
			if (terms.Length == 1) {
				int.TryParse(terms[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var token);
				searchTermToken = MetadataTokenHelpers.EntityHandleOrNil(token);
			}
		}

		public override void Search(PEFile module)
		{
			if (searchTermToken.IsNil) return;
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null) return;
			var metadataModule = (MetadataModule)typeSystem.MainModule;
			int row = module.Metadata.GetRowNumber(searchTermToken);

			switch (searchTermToken.Kind) {
				case HandleKind.TypeDefinition:
					if (row < 1 || row > module.Metadata.TypeDefinitions.Count)
						break;
					var type = metadataModule.GetDefinition((TypeDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(type));
					break;
				case HandleKind.MethodDefinition:
					if (row < 1 || row > module.Metadata.MethodDefinitions.Count)
						break;
					var method = metadataModule.GetDefinition((MethodDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(method));
					break;
				case HandleKind.FieldDefinition:
					if (row < 1 || row > module.Metadata.FieldDefinitions.Count)
						break;
					var field = metadataModule.GetDefinition((FieldDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(field));
					break;
				case HandleKind.PropertyDefinition:
					if (row < 1 || row > module.Metadata.PropertyDefinitions.Count)
						break;
					var property = metadataModule.GetDefinition((PropertyDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(property));
					break;
				case HandleKind.EventDefinition:
					if (row < 1 || row > module.Metadata.EventDefinitions.Count)
						break;
					var @event = metadataModule.GetDefinition((EventDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(@event));
					break;
			}
		}
	}
}
