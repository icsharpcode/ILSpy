using System;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Search
{
	class MemberSearchStrategy : AbstractSearchStrategy
	{
		readonly MemberSearchKind searchKind;

		public MemberSearchStrategy(Language language, Action<SearchResult> addResult, string term, MemberSearchKind searchKind = MemberSearchKind.All)
			: this(language, addResult, new[] { term }, searchKind)
		{
		}

		public MemberSearchStrategy(Language language, Action<SearchResult> addResult, string[] terms, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(language, addResult, terms)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module)
		{
			var metadata = module.Metadata;
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null) return;

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Type) {
				foreach (var handle in metadata.TypeDefinitions) {
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					addResult(ResultFromEntity(type));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Method) {
				foreach (var handle in metadata.MethodDefinitions) {
					// TODO use method semantics to skip accessors
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					addResult(ResultFromEntity(method));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Field) {
				foreach (var handle in metadata.FieldDefinitions) {
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					addResult(ResultFromEntity(field));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Property) {
				foreach (var handle in metadata.PropertyDefinitions) {
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					addResult(ResultFromEntity(property));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Event) {
				foreach (var handle in metadata.EventDefinitions) {
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (!IsMatch(languageSpecificName))
						continue;
					var @event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					addResult(ResultFromEntity(@event));
				}
			}
		}
	}

	enum MemberSearchKind
	{
		All,
		Type,
		Member,
		Field,
		Property,
		Event,
		Method
	}
}
