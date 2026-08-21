// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>Precise reason the active AI selection cannot serve requests.</summary>
	public enum AIReadinessReason
	{
		Ready,
		PrivacyConsentRequired,
		MissingProfile,
		MissingModel,
		InvalidEndpoint,
		InvalidProvider,
		MissingApiKey,
		SecureStoreUnavailable
	}

	/// <summary>
	/// Validation status of the active AI selection. A non-ready state blocks AI requests and
	/// names the exact problem so the UI can offer navigation to AI settings.
	/// </summary>
	public sealed record AIConfigurationState(AIReadinessReason Reason, string Message)
	{
		public bool IsReady => Reason == AIReadinessReason.Ready;

		public static AIConfigurationState Ready()
		{
			return new AIConfigurationState(AIReadinessReason.Ready, string.Empty);
		}

		public static AIConfigurationState NotReady(AIReadinessReason reason, string message)
		{
			ArgumentNullException.ThrowIfNull(message);
			return new AIConfigurationState(reason, message);
		}
	}

	/// <summary>The application-wide pairing of one profile and one of its models.</summary>
	public sealed record AISelection(string ProfileId, string Model);

	/// <summary>
	/// Immutable resolved request target captured when an AI request starts: profile identity,
	/// provider type, endpoint, model, resolved credential, and the global request preferences
	/// the provider/context builder need. Later settings changes affect future requests only.
	/// </summary>
	public sealed record AISelectionSnapshot
	{
		public string ProfileId { get; init; } = string.Empty;

		public string ProfileName { get; init; } = string.Empty;

		public string ProviderType { get; init; } = string.Empty;

		public string Endpoint { get; init; } = string.Empty;

		public string Model { get; init; } = string.Empty;

		public string? ApiKey { get; init; }

		public string CredentialId { get; init; } = string.Empty;

		public int MaxContextTokens { get; init; } = 32000;

		public bool StreamResponses { get; init; } = true;

		public bool SendIL { get; init; }

		public bool SendCallGraph { get; init; }
	}

	/// <summary>
	/// Immutable conversation-target metadata attached to a chat conversation. A conversation
	/// belongs to the profile identity, provider type, endpoint, and model selected when it was
	/// created; profile renames update neither the boundary nor the stored name snapshot.
	/// </summary>
	public sealed record AIConversationTarget(
		string ProfileId,
		string ProfileName,
		string ProviderType,
		string Endpoint,
		string Model)
	{
		public bool BelongsTo(string profileId, string providerType, string endpoint, string model)
		{
			return string.Equals(ProfileId, profileId, StringComparison.Ordinal)
				&& string.Equals(ProviderType, providerType, StringComparison.Ordinal)
				&& string.Equals(Endpoint, endpoint, StringComparison.Ordinal)
				&& string.Equals(Model, model, StringComparison.OrdinalIgnoreCase);
		}
	}
}
