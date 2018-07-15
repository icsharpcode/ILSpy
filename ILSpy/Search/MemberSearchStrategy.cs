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
					var td = metadata.GetTypeDefinition(handle);
					string languageSpecificName = null;
					ITypeDefinition type = null;
					if (needsLanguageSpecificNames) {
						type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
						languageSpecificName = GetLanguageSpecificName(type, fullName: true);
					}
					if (!IsMatch(module.Metadata, td.Name, languageSpecificName))
						continue;
					if (type == null) {
						type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					}
					addResult(ResultFromEntity(type));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Method) {
				foreach (var handle in metadata.MethodDefinitions) {
					// TODO use method semantics to skip accessors
					var md = metadata.GetMethodDefinition(handle);
					string languageSpecificName = null;
					IMethod method = null;
					if (needsLanguageSpecificNames) {
						method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
						languageSpecificName = GetLanguageSpecificName(method, fullName: true);
					}
					if (!IsMatch(module.Metadata, md.Name, languageSpecificName))
						continue;
					if (method == null) {
						method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					}
					addResult(ResultFromEntity(method));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Field) {
				foreach (var handle in metadata.FieldDefinitions) {
					var fd = metadata.GetFieldDefinition(handle);
					string languageSpecificName = null;
					IField field = null;
					if (needsLanguageSpecificNames) {
						field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
						languageSpecificName = GetLanguageSpecificName(field, fullName: true);
					}
					if (!IsMatch(module.Metadata, fd.Name, languageSpecificName))
						continue;
					if (field == null) {
						field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					}
					addResult(ResultFromEntity(field));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Property) {
				foreach (var handle in metadata.PropertyDefinitions) {
					var pd = metadata.GetPropertyDefinition(handle);
					string languageSpecificName = null;
					IProperty property = null;
					if (needsLanguageSpecificNames) {
						property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
						languageSpecificName = GetLanguageSpecificName(property, fullName: true);
					}
					if (!IsMatch(module.Metadata, pd.Name, languageSpecificName))
						continue;
					if (property == null) {
						property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					}
					addResult(ResultFromEntity(property));
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Event) {
				foreach (var handle in metadata.EventDefinitions) {
					var ed = metadata.GetEventDefinition(handle);
					string languageSpecificName = null;
					IEvent @event = null;
					if (needsLanguageSpecificNames) {
						@event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
						languageSpecificName = GetLanguageSpecificName(@event, fullName: true);
					}
					if (!IsMatch(module.Metadata, ed.Name, languageSpecificName))
						continue;
					if (@event == null) {
						@event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					}
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
