// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Application-scoped AI selection service: loads and validates profiles, owns the shared
	/// active selection, applies selector changes with immediate persistence, resolves immutable
	/// request snapshots, and applies the deterministic deletion fallback. Unsaved profile
	/// editor drafts never flow through this service; resolution reads saved state only.
	/// </summary>
	[Export]
	[Shared]
#pragma warning disable MEF009 // Multiple constructors by design: public for MEF, internal for testing
	public sealed class AISelectionService : IDisposable
#pragma warning restore MEF009
	{
		readonly AISettings settings;
		readonly SecureKeyStorage keyStorage;
		readonly Func<Task>? persistAsync;
		readonly AICredentialMigration credentialMigration;
		readonly SemaphoreSlim commitLock = new(1, 1);

		[ImportingConstructor]
		public AISelectionService(AISelectionHost host)
			: this(
				(host ?? throw new ArgumentNullException(nameof(host))).Settings ?? throw new InvalidOperationException("The AI selection host must provide AI settings."),
				new SecureKeyStorage(),
				host.PersistAsync)
		{
		}

		internal AISelectionService(AISettings settings, SecureKeyStorage keyStorage, Func<Task>? persistAsync = null)
		{
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			this.persistAsync = persistAsync;
			credentialMigration = new AICredentialMigration(keyStorage);
		}

		/// <summary>Raised once per committed saved-state change (selection, delete, save).</summary>
		public event EventHandler? SelectionChanged;

		public IReadOnlyList<AIProfile> Profiles => settings.Profiles;

		public AIProfile ActiveProfile => settings.ActiveProfile;

		/// <summary>Returns whether the active saved selection is structurally usable and has a
		/// credential hint when the provider requires one. This synchronous gate is for UI
		/// enablement only; requests must still call <see cref="ResolveSnapshotAsync"/>.</summary>
		public bool CanAttemptRequest {
			get {
				AIConfigurationState structural = EvaluateStructuralReadiness();
				if (!structural.IsReady)
					return false;
				AIProviderDescriptor provider = AIProviderCatalog.Get(settings.ActiveProfile.ProviderType);
				return provider.KeyRequirement != AIProviderKeyRequirement.Required
					|| !string.IsNullOrWhiteSpace(settings.ApiKey)
					|| settings.ActiveProfile.HasStoredKey;
			}
		}

		public AISelection ActiveSelection => new(settings.ActiveProfileId, settings.ActiveProfile.ResolveModel());

		/// <summary>
		/// Migrates any pending legacy credential before profiles serve requests. Safe to call
		/// repeatedly; completes quickly when nothing is pending.
		/// </summary>
		public Task EnsureCredentialMigrationAsync(CancellationToken cancellationToken = default)
		{
			return credentialMigration.EnsureMigratedAsync(settings, cancellationToken);
		}

		/// <summary>Validation status of the active selection, with the exact blocking reason.</summary>
		public async Task<AIConfigurationState> EvaluateReadinessAsync(CancellationToken cancellationToken = default)
		{
			AIConfigurationState structural = EvaluateStructuralReadiness();
			if (!structural.IsReady)
				return structural;

			AIProfile profile = settings.ActiveProfile;
			AIProviderDescriptor provider = AIProviderCatalog.Get(profile.ProviderType);
			if (provider.KeyRequirement == AIProviderKeyRequirement.None)
				return AIConfigurationState.Ready();
			if (!string.IsNullOrWhiteSpace(settings.ApiKey))
				return AIConfigurationState.Ready();
			if (!profile.HasStoredKey)
				return provider.KeyRequirement == AIProviderKeyRequirement.Optional
					? AIConfigurationState.Ready()
					: AIConfigurationState.NotReady(AIReadinessReason.MissingApiKey,
						$"Profile '{profile.Name}' has no API key. Add one in AI settings.");

			SecureKeyLookupResult lookup = await keyStorage.TryLoadKeyAsync(profile.CredentialId, cancellationToken).ConfigureAwait(false);
			switch (lookup.Status)
			{
				case SecureKeyLookupStatus.Unavailable:
					return AIConfigurationState.NotReady(AIReadinessReason.SecureStoreUnavailable,
						"Secure API-key storage is unavailable on this system.");
				case SecureKeyLookupStatus.Found:
					return AIConfigurationState.Ready();
				default:
					return provider.KeyRequirement == AIProviderKeyRequirement.Optional
						? AIConfigurationState.Ready()
						: AIConfigurationState.NotReady(AIReadinessReason.MissingApiKey,
							$"Profile '{profile.Name}' has no API key. Add one in AI settings.");
			}
		}

		AIConfigurationState EvaluateStructuralReadiness()
		{
			if (!settings.PrivacyConsentAccepted)
				return AIConfigurationState.NotReady(AIReadinessReason.PrivacyConsentRequired,
					"Accept the privacy notice in AI settings before using AI.");
			if (settings.Profiles.Count == 0)
				return AIConfigurationState.NotReady(AIReadinessReason.MissingProfile,
					"No AI profile exists. Create one in AI settings.");
			AIProfile profile = settings.ActiveProfile;
			if (!AIProviderCatalog.TryGet(profile.ProviderType, out _))
				return AIConfigurationState.NotReady(AIReadinessReason.InvalidProvider,
					$"Provider type '{profile.ProviderType}' is not supported.");
			if (!TryValidateEndpoint(profile.BaseUrl))
				return AIConfigurationState.NotReady(AIReadinessReason.InvalidEndpoint,
					$"Profile '{profile.Name}' has no valid endpoint. Set an absolute HTTP(S) URL in AI settings.");
			if (string.IsNullOrWhiteSpace(profile.ResolveModel()))
				return AIConfigurationState.NotReady(AIReadinessReason.MissingModel,
					$"Profile '{profile.Name}' has no model. Add one in AI settings.");
			return AIConfigurationState.Ready();
		}

		/// <summary>
		/// Resolves the immutable request snapshot for the active selection, loading the stored
		/// credential. Throws <see cref="AIConfigurationException"/> when not ready.
		/// </summary>
		public async Task<AISelectionSnapshot> ResolveSnapshotAsync(CancellationToken cancellationToken = default)
		{
			AIConfigurationState state = await EvaluateReadinessAsync(cancellationToken).ConfigureAwait(false);
			if (!state.IsReady)
				throw new AIConfigurationException(state.Message);

			AIProfile profile = settings.ActiveProfile;
			string? apiKey = settings.ApiKey;
			if (string.IsNullOrWhiteSpace(apiKey) && profile.HasStoredKey)
			{
				SecureKeyLookupResult lookup = await keyStorage.TryLoadKeyAsync(profile.CredentialId, cancellationToken).ConfigureAwait(false);
				if (lookup.Status == SecureKeyLookupStatus.Found)
					apiKey = lookup.Value;
			}

			return new AISelectionSnapshot {
				ProfileId = profile.Id,
				ProfileName = profile.Name,
				ProviderType = profile.ProviderType,
				Endpoint = profile.BaseUrl,
				Model = profile.ResolveModel(),
				ApiKey = apiKey,
				CredentialId = profile.CredentialId,
				MaxContextTokens = settings.MaxContextTokens,
				StreamResponses = settings.StreamResponses,
				SendIL = settings.SendIL,
				SendCallGraph = settings.SendCallGraph,
			};
		}

		/// <summary>
		/// Applies a selector change and persists it immediately, independently of any unsaved
		/// profile editor draft. Blank model restores the profile's remembered model.
		/// </summary>
		public async Task ApplySelectionAsync(string profileId, string model, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(profileId);
			await commitLock.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				AIProfile? profile = settings.Profiles.FirstOrDefault(p => p.Id == profileId);
				if (profile == null)
					throw new AIConfigurationException($"Profile '{profileId}' does not exist.");

				string resolvedModel = string.IsNullOrWhiteSpace(model) ? profile.ResolveModel() : model.Trim();
				if (!profile.Models.Contains(resolvedModel, StringComparer.OrdinalIgnoreCase))
					throw new AIConfigurationException($"Model '{resolvedModel}' is not listed in profile '{profile.Name}'.");

				profile.LastSelectedModel = resolvedModel;
				settings.ActiveProfileId = profile.Id;
				await PersistAsync().ConfigureAwait(false);
			}
			finally
			{
				commitLock.Release();
			}
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Deletes a profile after removing its credential. Missing secrets count as success; a
		/// secret-deletion failure aborts metadata deletion and leaves the profile unchanged.
		/// Active deletion applies the deterministic fallback selection.
		/// </summary>
		public async Task DeleteProfileAsync(string profileId, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(profileId);
			await commitLock.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				int index = settings.Profiles.ToList().FindIndex(p => p.Id == profileId);
				if (index < 0)
					throw new AIConfigurationException($"Profile '{profileId}' does not exist.");
				if (settings.Profiles.Count == 1)
					throw new AIConfigurationException("The only profile cannot be deleted.");

				AIProfile profile = settings.Profiles[index];
				if (profile.HasStoredKey)
					await keyStorage.DeleteKeyAsync(profile.CredentialId, cancellationToken).ConfigureAwait(false);

				bool wasActive = settings.ActiveProfileId == profile.Id;
				settings.Profiles.RemoveAt(index);
				settings.NotifyProfilesChanged();
				if (wasActive)
				{
					int fallbackIndex = index < settings.Profiles.Count ? index : 0;
					AIProfile fallback = settings.Profiles[fallbackIndex];
					settings.ActiveProfileId = fallback.Id;
				}
				await PersistAsync().ConfigureAwait(false);
			}
			finally
			{
				commitLock.Release();
			}
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Commits a new or edited profile (already validated for structure and unique name),
		/// then persists. Determines whether it is an add (no existing id) or an update.
		/// </summary>
		public async Task SaveProfileAsync(AIProfile savedProfile, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(savedProfile);
			IReadOnlyList<string> errors = savedProfile.Validate();
			if (errors.Count > 0)
				throw new AIConfigurationException(string.Join(" ", errors));
			if (settings.Profiles.Any(p => p.Id != savedProfile.Id
				&& string.Equals(p.Name, savedProfile.Name, StringComparison.OrdinalIgnoreCase)))
				throw new AIConfigurationException($"A profile named '{savedProfile.Name}' already exists.");

			await commitLock.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				int index = settings.Profiles.ToList().FindIndex(p => p.Id == savedProfile.Id);
				if (index < 0)
				{
					settings.Profiles.Add(savedProfile);
					if (settings.Profiles.Count == 1)
						settings.ActiveProfileId = savedProfile.Id;
				}
				else
				{
					settings.Profiles[index] = savedProfile;
				}
				settings.NotifyProfilesChanged();
				await PersistAsync().ConfigureAwait(false);
			}
			finally
			{
				commitLock.Release();
			}
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>Moves a profile one position up or down. Selection is unaffected.</summary>
		public async Task MoveProfileAsync(string profileId, int delta, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(profileId);
			await commitLock.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				int index = settings.Profiles.ToList().FindIndex(p => p.Id == profileId);
				if (index < 0)
					throw new AIConfigurationException($"Profile '{profileId}' does not exist.");
				int target = index + Math.Sign(delta);
				if (target < 0 || target >= settings.Profiles.Count)
					return;
				(settings.Profiles[index], settings.Profiles[target]) = (settings.Profiles[target], settings.Profiles[index]);
				settings.NotifyProfilesChanged();
				await PersistAsync().ConfigureAwait(false);
			}
			finally
			{
				commitLock.Release();
			}
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		async Task PersistAsync()
		{
			if (persistAsync != null)
				await persistAsync().ConfigureAwait(false);
		}

		public void Dispose()
		{
			commitLock.Dispose();
		}

		static bool TryValidateEndpoint(string? endpoint)
		{
			return Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri)
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}
	}

	/// <summary>
	/// Composition bridge that supplies the live settings instance and persistence callback to
	/// <see cref="AISelectionService"/> without coupling ILSpyX to the desktop SettingsService.
	/// Implemented in the ILSpy application layer.
	/// </summary>
	public abstract class AISelectionHost
	{
		public abstract Settings.AISettings Settings { get; }

		public virtual Func<Task>? PersistAsync => null;
	}
}
