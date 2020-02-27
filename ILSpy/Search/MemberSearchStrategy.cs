using System;
using System.Collections.Concurrent;
using System.Threading;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Search
{
	class MemberSearchStrategy : AbstractEntitySearchStrategy
	{
		readonly MemberSearchKind searchKind;

		public MemberSearchStrategy(Language language, ApiVisibility apiVisibility, string term, IProducerConsumerCollection<SearchResult> resultQueue, MemberSearchKind searchKind = MemberSearchKind.All)
			: this(language, apiVisibility, resultQueue, new[] { term }, searchKind)
		{
		}

		public MemberSearchStrategy(Language language, ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, string[] terms, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(language, apiVisibility, resultQueue, terms)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var metadata = module.Metadata;
			var typeSystem = module.GetTypeSystemWithCurrentOptionsOrNull();
			if (typeSystem == null) return;

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Type) {
				foreach (var handle in metadata.TypeDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(type)) continue;
					OnFoundResult(type);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Method) {
				foreach (var handle in metadata.MethodDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(method)) continue;
					OnFoundResult(method);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Field) {
				foreach (var handle in metadata.FieldDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(field)) continue;
					OnFoundResult(field);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Property) {
				foreach (var handle in metadata.PropertyDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(property)) continue;
					OnFoundResult(property);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Event) {
				foreach (var handle in metadata.EventDefinitions) {
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (!IsMatch(languageSpecificName))
						continue;
					var @event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(@event)) continue;
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
