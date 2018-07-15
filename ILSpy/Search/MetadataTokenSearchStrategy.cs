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

			switch (searchTermToken.Kind) {
				case HandleKind.TypeDefinition:
					var type = ((MetadataModule)typeSystem.MainModule).GetDefinition((TypeDefinitionHandle)searchTermToken);
					addResult(ResultFromEntity(type));
					break;
			}
		}
	}
}
