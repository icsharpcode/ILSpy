using System;
using System.Collections.Concurrent;
using System.Threading;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Search
{
	class MemberSearchStrategy : AbstractSearchStrategy
	{
		readonly MemberSearchKind searchKind;

		public MemberSearchStrategy(Language language, string term, IProducerConsumerCollection<SearchResult> resultQueue, MemberSearchKind searchKind = MemberSearchKind.All)
			: this(language, resultQueue, new[] { term }, searchKind)
		{
		}

		public MemberSearchStrategy(Language language, IProducerConsumerCollection<SearchResult> resultQueue, string[] terms, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(language, resultQueue, terms)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var metadata = module.Metadata;
			var typeSystem = module.GetTypeSystemOrNull();
			if (typeSystem == null) return;

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Type) {
				foreach (var handle in metadata.TypeDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					OnFoundResult(type);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Method) {
				foreach (var handle in metadata.MethodDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					// TODO use method semantics to skip accessors
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					OnFoundResult(method);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Field) {
				foreach (var handle in metadata.FieldDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					OnFoundResult(field);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Property) {
				foreach (var handle in metadata.PropertyDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					OnFoundResult(property);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Event) {
				foreach (var handle in metadata.EventDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch);
					if (!IsMatch(languageSpecificName))
						continue;
					var @event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					OnFoundResult(@event);
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
