// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class AICredentialMigrationTests
	{
		[Test]
		public async Task PendingMigration_MovesLegacyKeyUnderProfileIdentity()
		{
			var backend = new FakeBackend();
			backend.Keys["anthropic"] = "sk-legacy-key";
			var settings = new AISettings();
			settings.LoadFromXml(new System.Xml.Linq.XElement("AISettings",
				new System.Xml.Linq.XElement("Provider", "anthropic"),
				new System.Xml.Linq.XElement("ApiKeyPlaceholder", "ref")));
			string profileCredentialId = settings.Profiles[0].CredentialId;
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);

			settings.CredentialMigrationPending.Should().BeFalse("a confirmed profile key completes the migration");
			backend.Keys.Should().ContainKey(profileCredentialId);
			backend.Keys[profileCredentialId].Should().Be("sk-legacy-key");
			backend.Keys.Should().NotContainKey("anthropic", "the legacy entry is removed after confirmation");
			settings.Profiles[0].HasStoredKey.Should().BeTrue();
		}

		[Test]
		public async Task PendingMigration_WithoutLegacyKey_CompletesWithoutWriting()
		{
			var backend = new FakeBackend();
			var settings = new AISettings();
			settings.LoadFromXml(new System.Xml.Linq.XElement("AISettings",
				new System.Xml.Linq.XElement("Provider", "openai"),
				new System.Xml.Linq.XElement("ApiKeyPlaceholder", "ref")));
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);

			settings.CredentialMigrationPending.Should().BeFalse();
			backend.Keys.Should().BeEmpty();
		}

		[Test]
		public async Task UnavailableStore_KeepsPendingMarkerAndLegacyKey()
		{
			var backend = new FakeBackend { IsUnavailable = true };
			backend.Keys["openai"] = "sk-legacy-key";
			var settings = new AISettings();
			settings.LoadFromXml(new System.Xml.Linq.XElement("AISettings",
				new System.Xml.Linq.XElement("Provider", "openai"),
				new System.Xml.Linq.XElement("ApiKeyPlaceholder", "ref")));
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);

			settings.CredentialMigrationPending.Should().BeTrue("migration retries on a later run");
			backend.Keys.Should().ContainKey("openai", "the legacy key remains authoritative until migration confirms");
		}

		[Test]
		public async Task CompletedMigration_IsNeverRetried()
		{
			var backend = new FakeBackend();
			var settings = new AISettings();
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);

			backend.SaveCount.Should().Be(0);
			backend.LoadCount.Should().Be(0, "a fresh default profile needs no migration work");
		}

		[Test]
		public async Task Migration_IsIdempotentAcrossRuns()
		{
			var backend = new FakeBackend();
			backend.Keys["openai"] = "sk-legacy-key";
			var settings = new AISettings();
			settings.LoadFromXml(new System.Xml.Linq.XElement("AISettings",
				new System.Xml.Linq.XElement("Provider", "openai"),
				new System.Xml.Linq.XElement("ApiKeyPlaceholder", "ref")));
			string profileCredentialId = settings.Profiles[0].CredentialId;
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);
			await migration.EnsureMigratedAsync(settings);

			backend.Keys.Should().HaveCount(1);
			backend.Keys[profileCredentialId].Should().Be("sk-legacy-key");
		}

		[Test]
		public async Task MigratedKeyIsNeverWrittenToXml()
		{
			var backend = new FakeBackend();
			backend.Keys["openai"] = "sk-legacy-key";
			var settings = new AISettings();
			settings.LoadFromXml(new System.Xml.Linq.XElement("AISettings",
				new System.Xml.Linq.XElement("Provider", "openai"),
				new System.Xml.Linq.XElement("ApiKeyPlaceholder", "ref")));
			var migration = new AICredentialMigration(new SecureKeyStorage(backend));

			await migration.EnsureMigratedAsync(settings);

			settings.SaveToXml().ToString().Should().NotContain("sk-legacy-key");
		}

		sealed class FakeBackend : ISecureKeyStorageBackend
		{
			public Dictionary<string, string> Keys { get; } = new();
			public bool IsUnavailable { get; init; }
			public int SaveCount { get; private set; }
			public int LoadCount { get; private set; }

			public Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				SaveCount++;
				Keys[provider] = key;
				return Task.CompletedTask;
			}

			public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				LoadCount++;
				return Task.FromResult(Keys.TryGetValue(provider, out string? key)
					? SecureKeyStorageBackendReadResult.Found(key)
					: SecureKeyStorageBackendReadResult.NotFound);
			}

			public Task DeleteAsync(string provider, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				Keys.Remove(provider);
				return Task.CompletedTask;
			}

			void ThrowIfUnavailable()
			{
				if (IsUnavailable)
					throw new SecureKeyStorageUnavailableException("test backend unavailable");
			}
		}
	}
}
