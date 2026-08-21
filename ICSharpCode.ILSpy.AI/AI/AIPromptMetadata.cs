// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;

using YamlDotNet.Serialization;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>Metadata parsed from an external AI prompt file.</summary>
	public sealed class AIPromptMetadata
	{
		[YamlMember(Alias = "description")]
		public string Description { get; set; } = string.Empty;

		[YamlMember(Alias = "applies_to_models")]
		public List<string>? AppliesToModels { get; set; }

		[YamlMember(Alias = "author")]
		public string? Author { get; set; }

		[YamlMember(Alias = "updated_at")]
		public string? UpdatedAt { get; set; }

		[YamlMember(Alias = "temperature_hint")]
		public double? TemperatureHint { get; set; }

		[YamlMember(Alias = "max_tokens_hint")]
		public int? MaxTokensHint { get; set; }

		[YamlMember(Alias = "version")]
		public int? Version { get; set; }
	}
}
