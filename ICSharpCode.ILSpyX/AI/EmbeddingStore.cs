using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>Small dependency-free vector cache for local semantic search.</summary>
	public sealed class EmbeddingStore
	{
		readonly Dictionary<string, float[]> vectors = new(StringComparer.Ordinal);
		public void Add(string key, string text) => vectors[key] = Vectorize(text);
		public IReadOnlyList<(string Key, float Score)> Search(string query, int limit = 20)
		{
			var queryVector = Vectorize(query);
			return vectors.Select(pair => (pair.Key, Score: Cosine(queryVector, pair.Value))).OrderByDescending(pair => pair.Score).Take(limit).ToArray();
		}
		static float[] Vectorize(string text)
		{
			var vector = new float[128];
			foreach (string token in text.Split(new[] { ' ', '\t', '\r', '\n', '.', ':', '/', '<', '>', '(', ')', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
				vector[(uint)StringComparer.OrdinalIgnoreCase.GetHashCode(token) % (uint)vector.Length]++;
			return vector;
		}
		static float Cosine(float[] left, float[] right)
		{
			float dot = 0, l = 0, r = 0;
			for (int i = 0; i < left.Length; i++) { dot += left[i] * right[i]; l += left[i] * left[i]; r += right[i] * right[i]; }
			return l == 0 || r == 0 ? 0 : dot / MathF.Sqrt(l * r);
		}
	}
}
