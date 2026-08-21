// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.Search
{
	/// <summary>Interprets a natural-language query against a bounded symbol vocabulary.</summary>
	public static class AISearchStrategy
	{
		///<summary>Runs the search against an immutable resolved target.</summary>
		public static Task<IReadOnlyList<IEntity>> SearchAsync(IEnumerable<MetadataFile> modules, string query, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(snapshot);
			string systemPrompt = AIPromptProvider.Instance.GetSystemPrompt("search", snapshot.Model);
			return SearchCoreAsync(modules, query, systemPrompt, providerFactory, ct => providerFactory.CreateAsync(snapshot, ct), cancellationToken);
		}

		static async Task<IReadOnlyList<IEntity>> SearchCoreAsync(IEnumerable<MetadataFile> modules, string query, string systemPrompt, IAIProviderFactory providerFactory, Func<CancellationToken, Task<ILLMProvider>> createProvider, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(modules);
			ArgumentNullException.ThrowIfNull(query);
			ArgumentNullException.ThrowIfNull(providerFactory);
			var candidates = modules.SelectMany(GetCandidates).GroupBy(e => e.FullName, StringComparer.Ordinal).Select(g => g.First()).Take(50).ToArray();
			if (candidates.Length == 0)
				return Array.Empty<IEntity>();
			string vocabulary = string.Join("\n", candidates.Select(e => e.FullName));
			var provider = await createProvider(cancellationToken).ConfigureAwait(false);
			var request = new LLMRequest(systemPrompt, new[] { new LLMMessage("user", $"Query: {query}\n\nCandidates:\n{vocabulary}") }, 1024, 0.1);
			var text = new System.Text.StringBuilder();
			await foreach (var chunk in provider.CompleteAsync(request, cancellationToken).ConfigureAwait(false))
				text.Append(chunk);
			string json = text.ToString().Trim();
			if (json.StartsWith("```", StringComparison.Ordinal))
				json = json[(json.IndexOf('\n') + 1)..].TrimEnd('`', '\r', '\n');
			try
			{
				var names = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
				var map = candidates.ToDictionary(e => e.FullName, StringComparer.Ordinal);
				return names.Where(map.ContainsKey).Select(name => map[name]).ToArray();
			}
			catch (JsonException) { return Array.Empty<IEntity>(); }
		}

		static IEnumerable<IEntity> GetCandidates(MetadataFile module)
		{
			var compilation = module.GetTypeSystemWithDecompilerSettingsOrNull(new ICSharpCode.Decompiler.DecompilerSettings());
			if (compilation is null)
				yield break;
			foreach (var type in compilation.MainModule.TypeDefinitions)
			{
				yield return type;
				foreach (var member in type.Members.OfType<IMethod>())
					yield return member;
			}
		}
	}
}
