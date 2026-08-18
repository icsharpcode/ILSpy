// Copyright (c) 2026 Masroor

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
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>Uses the configured provider to identify common security risks in a type.</summary>
	[ExportAnalyzer(Header = "Security Risks (AI)", Order = 1000)]
	[Shared]
	public sealed class AISecurityAnalyzer : IAnalyzer
	{
		const string SystemPrompt = "You identify security vulnerabilities in decompiled .NET code. Return only valid JSON: [{\"type\": string, \"method\": string, \"issue\": string, \"severity\": \"Critical\"|\"High\"|\"Medium\"|\"Low\", \"line\": number}]. Report only plausible SQL injection, hardcoded credentials, weak cryptography, path traversal, unsafe deserialization, dangerous P/Invoke, or equivalent issues. Do not invent issues.";

		public bool Show(ISymbol? symbol) => symbol is ITypeDefinition or IMethod;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
			=> AnalyzeAsync(analyzedSymbol, context).GetAwaiter().GetResult();

		async System.Threading.Tasks.Task<IReadOnlyList<ISymbol>> AnalyzeAsync(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			if (context.AISettings is not { } settings || context.AIProviderFactory is not { } providerFactory)
				throw new AIConfigurationException("AI security analysis is unavailable until AI settings are configured.");
			ITypeDefinition type = analyzedSymbol switch {
				ITypeDefinition definition => definition,
				IMethod method when method.DeclaringTypeDefinition is { } declaringType => declaringType,
				_ => throw new InvalidOperationException("Security analysis requires a type or method.")
			};
			MetadataFile module = type.ParentModule?.MetadataFile ?? throw new InvalidOperationException("The selected type has no decompilable module.");
			var decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(true), new DecompilerSettings()) {
				CancellationToken = context.CancellationToken
			};
			var service = new AIExplanationService(settings, providerFactory);
			var findings = new List<ISymbol>();
			foreach (ITypeDefinition current in type.ParentModule!.Compilation.GetAllTypeDefinitions().Where(candidate => candidate.ParentModule == type.ParentModule))
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				DecompilationContext decompilationContext = new ContextBuilder(settings).Build(current, decompiler);
				string prompt = "Analyze this type for security risks.\n\n" + decompilationContext.ToMarkdown();
				var response = new List<string>();
				await foreach (string chunk in service.CompleteStreamingAsync(SystemPrompt, prompt, context.CancellationToken).ConfigureAwait(false))
					response.Add(chunk);
				findings.AddRange(ParseFindings(string.Concat(response), current));
			}
			return findings.Cast<ISymbol>().ToArray();
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
				if (string.IsNullOrWhiteSpace(item.Issue))
					continue;
				IEntity target = FindTarget(type, item.Method) ?? type;
				findings.Add(new AISecurityFinding(target, item.Type ?? "Security risk", item.Issue.Trim(), item.Severity ?? "Medium", Math.Max(0, item.Line)));
			}
			return findings;
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
		}
	}
}
