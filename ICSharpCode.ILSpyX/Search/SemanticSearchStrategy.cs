// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;

namespace ICSharpCode.ILSpyX.Search
{
	public static class SemanticSearchStrategy
	{
		/// <summary>Searches with the dependency-free local similarity heuristic; no provider or credentials are used.</summary>
		public static IReadOnlyList<IEntity> Search(IEnumerable<MetadataFile> modules, string query, int limit = 20)
		{
			var store = new EmbeddingStore();
			var entities = modules.SelectMany(module => GetCandidates(module)).ToArray();
			foreach (var entity in entities)
				store.Add(entity.FullName, entity.FullName + " " + entity.Name);
			var map = entities.GroupBy(e => e.FullName, System.StringComparer.Ordinal).ToDictionary(g => g.Key, g => g.First(), System.StringComparer.Ordinal);
			return store.Search(query, limit).Where(result => result.Score > 0).Select(result => map[result.Key]).ToArray();
		}
		static IEnumerable<IEntity> GetCandidates(MetadataFile module)
		{
			var compilation = module.GetTypeSystemWithDecompilerSettingsOrNull(new ICSharpCode.Decompiler.DecompilerSettings());
			if (compilation is null)
				yield break;
			foreach (var type in compilation.MainModule.TypeDefinitions)
			{
				yield return type;
				foreach (var method in type.Methods)
					yield return method;
			}
		}
	}
}
