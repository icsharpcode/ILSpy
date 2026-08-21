// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AI;

namespace ICSharpCode.ILSpy.AI.Decompiler
{
	/// <summary>
	/// Uses the configured provider to identify common security risks in a type. The desktop
	/// assembly owns the <c>IAnalyzer</c> adapter because that analyzer contract belongs to ILSpyX.
	/// </summary>
	public sealed class AISecurityAnalyzer
	{
		public const double MinimumFindingConfidence = 0.70;

		/// <summary>Runs the normal single-type security analyzer pipeline for a captured AI target.</summary>
		public async Task<IReadOnlyList<AISecurityFinding>> AnalyzeSelectedTypeAsync(
			ITypeDefinition type,
			AISelectionSnapshot snapshot,
			IAIProviderFactory providerFactory,
			IProgress<AISecurityAuditProgress>? progress = null,
			CancellationToken cancellationToken = default,
			string promptId = "security")
		{
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(snapshot);
			ArgumentNullException.ThrowIfNull(providerFactory);
			MetadataFile module = type.ParentModule?.MetadataFile ?? throw new InvalidOperationException("The selected type has no decompilable module.");
			var decompiler = new CSharpDecompiler(module.FileName, new DecompilerSettings()) { CancellationToken = cancellationToken };
			var service = new AIExplanationService(snapshot, providerFactory);
			progress?.Report(new AISecurityAuditProgress(0, 1, type.FullName, 0, 0, false));
			cancellationToken.ThrowIfCancellationRequested();
			DecompilationContext decompilationContext = new ContextBuilder(snapshot).Build(type, decompiler);
			string prompt = "Analyze this type for security risks.\n\n" + decompilationContext.ToMarkdown();
			var response = new List<string>();
			await foreach (string chunk in service.CompleteStreamingAsync(AIPromptProvider.Instance.GetSystemPrompt(promptId, snapshot.Model), prompt, cancellationToken).ConfigureAwait(false))
				response.Add(chunk);
			IReadOnlyList<AISecurityFinding> findings = ParseFindings(string.Concat(response), type);
			progress?.Report(new AISecurityAuditProgress(1, 1, type.FullName, findings.Count, 0, false));
			return findings;
		}

		public static IReadOnlyList<AISecurityFinding> ParseFindings(string response, ITypeDefinition type)
		{
			ArgumentNullException.ThrowIfNull(type);
			string json = response.Trim();
			if (json.StartsWith("```", StringComparison.Ordinal))
			{
				int newline = json.IndexOf('\n');
				int fence = json.LastIndexOf("```", StringComparison.Ordinal);
				if (newline >= 0 && fence > newline)
					json = json[(newline + 1)..fence].Trim();
			}
			var items = JsonSerializer.Deserialize<List<FindingDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
				?? throw new JsonException("Expected a JSON array.");
			var findings = new List<AISecurityFinding>();
			foreach (FindingDto item in items)
			{
				if (string.IsNullOrWhiteSpace(item.Issue) || !TryReadConfidence(item.Confidence, out double confidence) || confidence < MinimumFindingConfidence || confidence > 1)
					continue;
				IEntity target = FindTarget(type, item.Method) ?? type;
				findings.Add(new AISecurityFinding(target, item.Type ?? "Security risk", item.Issue.Trim(), item.Severity ?? "Medium", Math.Max(0, item.Line), confidence));
			}
			return findings;
		}

		static bool TryReadConfidence(JsonElement value, out double confidence)
		{
			confidence = 0;
			return value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out confidence)
				&& !double.IsNaN(confidence) && !double.IsInfinity(confidence);
		}

		static IEntity? FindTarget(ITypeDefinition type, string? methodName)
		{
			if (string.IsNullOrWhiteSpace(methodName))
				return null;
			return type.Methods.FirstOrDefault(method => string.Equals(method.Name, methodName, StringComparison.Ordinal)
				|| method.FullName.Contains(methodName, StringComparison.Ordinal));
		}

		sealed class FindingDto
		{
			public string? Type { get; set; }
			public string? Method { get; set; }
			public string? Issue { get; set; }
			public string? Severity { get; set; }
			public int Line { get; set; }
			public JsonElement Confidence { get; set; }
		}
	}
}
