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
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Search
{
	public class MemberSearchStrategy : AbstractEntitySearchStrategy
	{
		readonly MemberSearchKind searchKind;

		public MemberSearchStrategy(ILanguage language, ApiVisibility apiVisibility, SearchRequest searchRequest,
			IProducerConsumerCollection<SearchResult> resultQueue, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(language, apiVisibility, searchRequest, resultQueue)
		{
			this.searchKind = searchKind;
		}

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var metadata = module.Metadata;
			var typeSystem = module.GetTypeSystemWithDecompilerSettingsOrNull(searchRequest.DecompilerSettings);
			if (typeSystem == null)
				return;

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Type)
			{
				foreach (var handle in metadata.TypeDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var type = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(type) || !IsInNamespaceOrAssembly(type))
						continue;
					OnFoundResult(type);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Method)
			{
				foreach (var handle in metadata.MethodDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(method) || !IsInNamespaceOrAssembly(method))
						continue;
					OnFoundResult(method);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Field)
			{
				foreach (var handle in metadata.FieldDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(field) || !IsInNamespaceOrAssembly(field))
						continue;
					OnFoundResult(field);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Property)
			{
				foreach (var handle in metadata.PropertyDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (languageSpecificName != null && !IsMatch(languageSpecificName))
						continue;
					var property = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(property) || !IsInNamespaceOrAssembly(property))
						continue;
					OnFoundResult(property);
				}
			}

			if (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Member || searchKind == MemberSearchKind.Event)
			{
				foreach (var handle in metadata.EventDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					string languageSpecificName = language.GetEntityName(module, handle, fullNameSearch, omitGenerics);
					if (!IsMatch(languageSpecificName))
						continue;
					var @event = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
					if (!CheckVisibility(@event) || !IsInNamespaceOrAssembly(@event))
						continue;
					OnFoundResult(@event);
				}
			}
		}
	}

	public enum MemberSearchKind
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
