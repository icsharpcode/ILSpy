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
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.Search
{
	/// <summary>Interprets a natural-language query against a bounded symbol vocabulary.</summary>
	public static class AISearchStrategy
	{
		public static async Task<IReadOnlyList<IEntity>> SearchAsync(IEnumerable<MetadataFile> modules, string query, AISettings settings, IAIProviderFactory providerFactory, CancellationToken cancellationToken = default)
		{
			var candidates = modules.SelectMany(GetCandidates).GroupBy(e => e.FullName, StringComparer.Ordinal).Select(g => g.First()).Take(50).ToArray();
			if (candidates.Length == 0)
				return Array.Empty<IEntity>();
			string vocabulary = string.Join("\n", candidates.Select(e => e.FullName));
			var provider = await providerFactory.CreateAsync(settings, cancellationToken).ConfigureAwait(false);
			var request = new LLMRequest("Given these method and type signatures, which ones match the query? Return only a JSON array of fully-qualified names.", new[] { new LLMMessage("user", $"Query: {query}\n\nCandidates:\n{vocabulary}") }, 1024, 0.1);
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
