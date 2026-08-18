// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>Which party implements the wire protocol for a provider type.</summary>
	public enum AIProviderImplementation
	{
		OpenAICompatible,
		Anthropic
	}

	/// <summary>Whether a provider type requires a credential before requests can run.</summary>
	public enum AIProviderKeyRequirement
	{
		Required,
		Optional,
		None
	}

	/// <summary>
	/// Capability metadata for one supported provider type: friendly UI label, endpoint and
	/// model defaults, key requirement, and wire-protocol implementation. Persisted data always
	/// refers to providers by <see cref="Id"/>; labels are display-only.
	/// </summary>
	public sealed record AIProviderDescriptor(
		string Id,
		string Label,
		string DefaultBaseUrl,
		string DefaultModel,
		AIProviderKeyRequirement KeyRequirement,
		AIProviderImplementation Implementation);

	/// <summary>
	/// Registry of the provider types ILSpy supports. Capability lookups go through this
	/// catalog instead of provider-name conditionals scattered through the UI.
	/// </summary>
	public static class AIProviderCatalog
	{
		static readonly AIProviderDescriptor openai = new(
			"openai", "OpenAI", "https://api.openai.com", "gpt-4o",
			AIProviderKeyRequirement.Required, AIProviderImplementation.OpenAICompatible);

		static readonly AIProviderDescriptor anthropic = new(
			"anthropic", "Anthropic", "https://api.anthropic.com", "claude-opus-4-8",
			AIProviderKeyRequirement.Required, AIProviderImplementation.Anthropic);

		static readonly AIProviderDescriptor ollama = new(
			"ollama", "Ollama", "http://localhost:11434", "llama3:70b",
			AIProviderKeyRequirement.None, AIProviderImplementation.OpenAICompatible);

		static readonly AIProviderDescriptor custom = new(
			"custom", "Custom OpenAI-compatible", "https://api.openai.com", "gpt-4o",
			AIProviderKeyRequirement.Optional, AIProviderImplementation.OpenAICompatible);

		public static IReadOnlyList<AIProviderDescriptor> All { get; } = new[] { openai, anthropic, ollama, custom };

		public static AIProviderDescriptor Get(string id)
		{
			return TryGet(id, out AIProviderDescriptor? descriptor)
				? descriptor
				: throw new ArgumentException($"Provider type '{id}' is not supported.", nameof(id));
		}

		public static bool TryGet(string? id, out AIProviderDescriptor descriptor)
		{
			string normalized = string.IsNullOrWhiteSpace(id) ? string.Empty : id.Trim().ToLowerInvariant();
			foreach (AIProviderDescriptor candidate in All)
			{
				if (candidate.Id == normalized)
				{
					descriptor = candidate;
					return true;
				}
			}
			descriptor = null!;
			return false;
		}

		public static bool IsSupported(string? id)
		{
			return TryGet(id, out _);
		}
	}
}
