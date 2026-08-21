// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.AI.Decompiler
{
	public sealed record RenameSuggestion(string Name, double Confidence, string Reasoning)
	{
		public int ConfidencePercent => (int)Math.Round(Math.Clamp(Confidence <= 1 ? Confidence * 100 : Confidence, 0, 100));
	}

	public sealed class RenameSuggestionParseException : Exception
	{
		public RenameSuggestionParseException(string message, string rawText, Exception? innerException = null) : base(message, innerException)
		{
			RawText = rawText;
		}

		public string RawText { get; }
	}

	/// <summary>Builds a bounded symbol context and asks the configured provider for ranked names.</summary>
	public sealed class RenameSuggester
	{
		static readonly Regex GeneratedName = new("^(?:method|class|field|property|event|delegate|type)_\\d+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		readonly AISelectionSnapshot snapshot;
		readonly IAIProviderFactory providerFactory;

		///<summary>Creates a suggester bound to an immutable request target.</summary>
		public RenameSuggester(AISelectionSnapshot snapshot, IAIProviderFactory providerFactory)
		{
			this.snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
		}

		int MaxContextTokens => snapshot.MaxContextTokens;

		ContextBuilder CreateContextBuilder()
		{
			return new ContextBuilder(snapshot);
		}

		AIExplanationService CreateExplanationService()
		{
			return new AIExplanationService(snapshot, providerFactory);
		}

		public async Task<IReadOnlyList<RenameSuggestion>> SuggestAsync(IEntity entity, CSharpDecompiler decompiler, CancellationToken cancellationToken = default)
			=> await SuggestAsync(entity, decompiler, additionalContext: null, cancellationToken).ConfigureAwait(false);

		public async Task<IReadOnlyList<RenameSuggestion>> SuggestAsync(
			IEntity entity,
			CSharpDecompiler decompiler,
			string? additionalContext,
			CancellationToken cancellationToken = default)
			=> await SuggestAsync(entity, decompiler, additionalContext, namingHint: null, cancellationToken).ConfigureAwait(false);

		public async Task<IReadOnlyList<RenameSuggestion>> SuggestAsync(
			IEntity entity,
			CSharpDecompiler decompiler,
			string? additionalContext,
			string? namingHint,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			ArgumentNullException.ThrowIfNull(decompiler);
			if (!IsLikelyObfuscated(entity.Name))
				throw new ArgumentException("The selected symbol does not look obfuscated.", nameof(entity));

			DecompilationContext context = CreateContextBuilder().Build(entity, decompiler);
			string prompt = $"Suggest names for this symbol.\n\nCurrent name: {entity.Name}\nKind: {entity.SymbolKind}\nReflection name: {entity.ReflectionName}\n\n{context.ToMarkdown()}";
			if (!string.IsNullOrWhiteSpace(additionalContext))
			{
				int relatedBudget = Math.Max(128, MaxContextTokens / 5);
				prompt += "\n\nPreviously proposed renames:\n" + TokenCounter.TruncateToTokenBudget(additionalContext, relatedBudget, isCode: false);
			}
			if (!string.IsNullOrWhiteSpace(namingHint))
			{
				int hintBudget = Math.Max(128, MaxContextTokens / 5);
				prompt += "\n\nNaming hint from the user: " + TokenCounter.TruncateToTokenBudget(namingHint, hintBudget, isCode: false);
			}
			var service = CreateExplanationService();
			var chunks = new List<string>();
			await foreach (string chunk in service.CompleteStreamingAsync(AIPromptProvider.Instance.GetSystemPrompt("rename", snapshot.Model), prompt, cancellationToken).ConfigureAwait(false))
				chunks.Add(chunk);
			return ParseSuggestions(string.Concat(chunks));
		}

		public static bool IsLikelyObfuscated(string? name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return false;
			name = name.Trim();
			if (name.Length <= 2 || GeneratedName.IsMatch(name) || name.All(char.IsDigit))
				return true;
			if (name.StartsWith("<>", StringComparison.Ordinal) || name.StartsWith("<", StringComparison.Ordinal))
				return true;
			int letters = name.Count(char.IsLetter);
			int digits = name.Count(char.IsDigit);
			return digits > 0 && letters > 0 && name.Length <= 8 && digits >= letters;
		}

		public static IReadOnlyList<RenameSuggestion> ParseSuggestions(string response)
		{
			if (string.IsNullOrWhiteSpace(response))
				throw new RenameSuggestionParseException("The provider returned an empty response.", response);
			string json = response.Trim();
			if (json.StartsWith("```", StringComparison.Ordinal))
			{
				int firstNewline = json.IndexOf('\n');
				int lastFence = json.LastIndexOf("```", StringComparison.Ordinal);
				if (firstNewline >= 0 && lastFence > firstNewline)
					json = json[(firstNewline + 1)..lastFence].Trim();
			}

			try
			{
				var items = JsonSerializer.Deserialize<List<RenameSuggestionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Expected an array.");
				var result = items
					.Where(item => item is not null && IsValidIdentifier(item.Name))
					.Select(item => new RenameSuggestion(item.Name.Trim(), item.Confidence, item.Reasoning?.Trim() ?? string.Empty))
					.GroupBy(item => item.Name, StringComparer.Ordinal)
					.Select(group => group.First())
					.Take(5)
					.ToArray();
				if (result.Length == 0)
					throw new JsonException("No valid suggestions were returned.");
				return result;
			}
			catch (RenameSuggestionParseException) { throw; }
			catch (Exception exception) when (exception is JsonException or NotSupportedException)
			{
				throw new RenameSuggestionParseException("The provider returned invalid rename JSON.", response, exception);
			}
		}

		public static bool IsValidIdentifier(string? value)
		{
			if (string.IsNullOrWhiteSpace(value) || !(value[0] == '_' || char.IsLetter(value[0])))
				return false;
			for (int i = 1; i < value.Length; i++)
				if (!(value[i] == '_' || char.IsLetterOrDigit(value[i])))
					return false;
			return !ICSharpCode.Decompiler.CSharp.OutputVisitor.CSharpOutputVisitor.IsKeyword(value);
		}

		sealed class RenameSuggestionDto
		{
			public string Name { get; set; } = string.Empty;
			public double Confidence { get; set; }
			public string? Reasoning { get; set; }
		}
	}
}
