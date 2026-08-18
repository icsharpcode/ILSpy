// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Moves a legacy provider-keyed credential to the migrated profile identity. The legacy
	/// entry remains authoritative until the profile key is written and read back; only then is
	/// the legacy entry removed and the migration marker completed. Every step is idempotent,
	/// so an interrupted migration retries safely on a later run. Key values are never logged,
	/// serialized, or included in exceptions.
	/// </summary>
	public sealed class AICredentialMigration
	{
		readonly SecureKeyStorage keyStorage;

		public AICredentialMigration(SecureKeyStorage keyStorage)
		{
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
		}

		public async Task EnsureMigratedAsync(AISettings settings, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(settings);
			if (!settings.CredentialMigrationPending)
				return;

			AIProfile profile = settings.Profiles.FirstOrDefault(p => p.HasStoredKey) ?? settings.ActiveProfile;
			string legacyCredentialId = profile.ProviderType;
			string profileCredentialId = profile.CredentialId;

			SecureKeyLookupResult legacy = await keyStorage.TryLoadKeyAsync(legacyCredentialId, cancellationToken).ConfigureAwait(false);
			if (legacy.Status == SecureKeyLookupStatus.Unavailable)
				return; // retry on a later run; the legacy key stays authoritative
			if (legacy.Status == SecureKeyLookupStatus.NotFound || string.IsNullOrWhiteSpace(legacy.Value))
			{
				settings.MarkCredentialMigrationComplete();
				return;
			}

			try
			{
				await keyStorage.SaveKeyAsync(profileCredentialId, legacy.Value, cancellationToken).ConfigureAwait(false);
			}
			catch (SecureKeyStorageUnavailableException)
			{
				return;
			}

			SecureKeyLookupResult confirmation = await keyStorage.TryLoadKeyAsync(profileCredentialId, cancellationToken).ConfigureAwait(false);
			if (confirmation.Status != SecureKeyLookupStatus.Found || confirmation.Value != legacy.Value)
				return; // not confirmed; keep the legacy entry and retry later

			try
			{
				await keyStorage.DeleteKeyAsync(legacyCredentialId, cancellationToken).ConfigureAwait(false);
			}
			catch (SecureKeyStorageUnavailableException)
			{
				return; // cleanup retries later; the profile key is already in place
			}

			settings.MarkCredentialMigrationComplete();
		}
	}
}
