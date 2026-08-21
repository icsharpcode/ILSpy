// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// A named AI service configuration: one provider type, one endpoint, a credential
	/// reference, and an ordered list of manually managed models. The identity (<see cref="Id"/>)
	/// is generated once and never changes, including after rename or duplication.
	/// </summary>
	public sealed class AIProfile
	{
		public string Id { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string ProviderType { get; set; } = "openai";

		public string BaseUrl { get; set; } = string.Empty;

		public List<string> Models { get; } = new();

		public string LastSelectedModel { get; set; } = string.Empty;

		/// <summary>
		/// Non-secret hint that a credential exists in secure storage for this profile.
		/// The secure store remains authoritative; this flag is for display only.
		/// </summary>
		public bool HasStoredKey { get; set; }

		/// <summary>The canonical secure-storage credential identifier for this profile.</summary>
		public string CredentialId => SecureKeyStorage.ProfileCredentialId(Id);

		public static AIProfile Create(AIProviderDescriptor provider)
		{
			ArgumentNullException.ThrowIfNull(provider);
			return new AIProfile {
				Id = Guid.NewGuid().ToString("N"),
				ProviderType = provider.Id,
				BaseUrl = provider.DefaultBaseUrl,
				Models = { provider.DefaultModel },
				LastSelectedModel = provider.DefaultModel
			};
		}

		public AIProfile Clone()
		{
			var clone = new AIProfile {
				Id = Id,
				Name = Name,
				ProviderType = ProviderType,
				BaseUrl = BaseUrl,
				LastSelectedModel = LastSelectedModel,
				HasStoredKey = HasStoredKey
			};
			clone.Models.AddRange(Models);
			return clone;
		}

		/// <summary>
		/// Creates a copy with a new identity and no credential. Metadata and models carry
		/// over; the secret is never copied between profiles.
		/// </summary>
		public AIProfile Duplicate()
		{
			AIProfile duplicate = Clone();
			duplicate.Id = Guid.NewGuid().ToString("N");
			duplicate.HasStoredKey = false;
			return duplicate;
		}

		/// <summary>Trims the name, endpoint, and model names before validation and storage.</summary>
		public void Normalize()
		{
			Name = Name?.Trim() ?? string.Empty;
			BaseUrl = BaseUrl?.Trim() ?? string.Empty;
			ProviderType = ProviderType?.Trim().ToLowerInvariant() ?? string.Empty;
			for (int i = 0; i < Models.Count; i++)
				Models[i] = Models[i]?.Trim() ?? string.Empty;
			LastSelectedModel = LastSelectedModel?.Trim() ?? string.Empty;
		}

		/// <summary>Structural validation result: empty when the profile is well formed.</summary>
		public IReadOnlyList<string> Validate()
		{
			var errors = new List<string>();
			if (string.IsNullOrWhiteSpace(Name))
				errors.Add("A profile name is required.");
			if (!AIProviderCatalog.TryGet(ProviderType, out _))
				errors.Add($"Provider type '{ProviderType}' is not supported.");
			if (!IsAbsoluteHttpEndpoint(BaseUrl))
				errors.Add("The endpoint must be an absolute HTTP or HTTPS URI.");
			if (Models.Count == 0)
				errors.Add("At least one model is required.");
			for (int i = 0; i < Models.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(Models[i]))
					errors.Add("Model names cannot be blank.");
			}
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string model in Models.Where(m => !string.IsNullOrWhiteSpace(m)))
			{
				if (!seen.Add(model))
					errors.Add($"Model '{model}' is listed more than once.");
			}
			return errors;
		}

		/// <summary>
		/// The profile's remembered model when it is still listed, otherwise the first
		/// model in order.
		/// </summary>
		public string ResolveModel()
		{
			if (!string.IsNullOrWhiteSpace(LastSelectedModel)
				&& Models.Contains(LastSelectedModel, StringComparer.OrdinalIgnoreCase))
				return LastSelectedModel;
			return Models.Count > 0 ? Models[0] : string.Empty;
		}

		static bool IsAbsoluteHttpEndpoint(string? endpoint)
		{
			return Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri)
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}
	}
}
