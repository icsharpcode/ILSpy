// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Loads external AI prompts and selects model-specific variants with embedded fallbacks.
	/// </summary>
	public sealed class AIPromptProvider
	{
		private static readonly Lazy<AIPromptProvider> _instance = new(() => new AIPromptProvider());
		private readonly Dictionary<string, string> _cache = new(StringComparer.Ordinal);
		private readonly object _cacheLock = new();
		private readonly string _promptsDirectory;

		public static AIPromptProvider Instance => _instance.Value;

		private AIPromptProvider()
		{
			var assemblyLocation = typeof(AIPromptProvider).Assembly.Location;
			if (!string.IsNullOrEmpty(assemblyLocation))
			{
				var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
				_promptsDirectory = Path.Combine(assemblyDirectory ?? AppContext.BaseDirectory, "AI", "prompts");
			}
			else
			{
				_promptsDirectory = Path.Combine(AppContext.BaseDirectory, "AI", "prompts");
			}
		}

		/// <summary>
		/// Gets the system prompt for the specified prompt ID and optional model ID.
		/// </summary>
		/// <param name="promptId">Prompt identifier, for example, explanation.</param>
		/// <param name="modelId">Optional model ID for variant selection.</param>
		/// <returns>System prompt text, or an embedded fallback if the external prompt is unavailable.</returns>
		public string GetSystemPrompt(string promptId, string? modelId = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(promptId);
			var cacheKey = $"{promptId}:{modelId ?? "<null>"}";

			lock (_cacheLock)
			{
				if (_cache.TryGetValue(cacheKey, out var cached))
					return cached;
			}

			string prompt = Directory.Exists(_promptsDirectory)
				? LoadFromDirectory(promptId, modelId)
				: GetEmbeddedFallback(promptId);

			lock (_cacheLock)
			{
				if (_cache.TryGetValue(cacheKey, out var cached))
					return cached;
				_cache[cacheKey] = prompt;
				return prompt;
			}
		}

		private string LoadFromDirectory(string promptId, string? modelId)
		{
			var baseName = $"{promptId}.prompt";
			var allFiles = Directory.GetFiles(_promptsDirectory, $"{promptId}.*.prompt", SearchOption.TopDirectoryOnly)
				.Concat(new[] { Path.Combine(_promptsDirectory, baseName) })
				.Where(File.Exists)
				.OrderBy(file => Path.GetFileName(file), StringComparer.Ordinal)
				.ToList();

			if (!string.IsNullOrEmpty(modelId))
			{
				foreach (var file in allFiles)
				{
					if (Path.GetFileName(file).Equals(baseName, StringComparison.Ordinal))
						continue;

					var (metadata, promptText) = ParsePromptFile(file, promptId);
					if (metadata?.AppliesToModels?.Contains(modelId, StringComparer.Ordinal) == true)
						return promptText;
				}
			}

			var baseFile = Path.Combine(_promptsDirectory, baseName);
			if (File.Exists(baseFile))
				return ParsePromptFile(baseFile, promptId).promptText;

			return GetEmbeddedFallback(promptId);
		}

		private static (AIPromptMetadata? metadata, string promptText) ParsePromptFile(string filePath, string fallbackPromptId)
		{
			try
			{
				var content = File.ReadAllText(filePath, Encoding.UTF8);
				if (content.StartsWith("\uFEFF", StringComparison.Ordinal))
					content = content[1..];
				content = content.Replace("\r\n", "\n");

				var firstSeparator = content.IndexOf("\n---\n", StringComparison.Ordinal);
				if (firstSeparator < 0)
					return (null, content.Trim());

				var yamlBlock = content[..firstSeparator];
				var promptText = content[(firstSeparator + 5)..];
				if (!yamlBlock.StartsWith("---\n", StringComparison.Ordinal))
					return (null, content.Trim());

				yamlBlock = yamlBlock[4..];
				var deserializer = new DeserializerBuilder()
					.WithNamingConvention(UnderscoredNamingConvention.Instance)
					.IgnoreUnmatchedProperties()
					.Build();
				return (deserializer.Deserialize<AIPromptMetadata>(yamlBlock), promptText.Trim());
			}
			catch
			{
				return (null, GetEmbeddedFallback(fallbackPromptId));
			}
		}

		private static string GetEmbeddedFallback(string promptId) => EmbeddedPrompts.Get(promptId);
	}
}
