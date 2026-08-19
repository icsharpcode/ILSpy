// Copyright (c) 2026 Dr. Masroor Ehsan

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Analyzers
{
	/// <summary>A navigable security finding returned by the AI security analyzer.</summary>
	public sealed class AISecurityFinding : ISymbol
	{
		public AISecurityFinding(IEntity target, string type, string issue, string severity, int line, double confidence = 1.0)
		{
			Target = target;
			Type = type;
			Issue = issue;
			Severity = NormalizeSeverity(severity);
			Line = line;
			Confidence = confidence;
		}

		public IEntity Target { get; }
		public string Type { get; }
		public string Issue { get; }
		public string Severity { get; }
		public int Line { get; }
		public double Confidence { get; }
		public SymbolKind SymbolKind => SymbolKind.None;
		public string Name => $"{Severity}: {Issue}";

		static string NormalizeSeverity(string value)
			=> value?.Trim().ToLowerInvariant() switch {
				"critical" => "Critical",
				"high" => "High",
				"medium" => "Medium",
				"low" => "Low",
				_ => "Medium"
			};
	}
}
