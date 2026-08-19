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
	public class AISelectionServiceTests
	{
		[Test]
		public async Task EvaluateReadiness_RequiresConsentFirst()
		{
			var service = CreateService();

			AIConfigurationState state = await service.EvaluateReadinessAsync();

			state.IsReady.Should().BeFalse();
			state.Reason.Should().Be(AIReadinessReason.PrivacyConsentRequired);
		}

		[Test]
		public async Task EvaluateReadiness_ReportsMissingKeyForRequiredProvider()
		{
			var service = CreateService(consent: true);

			AIConfigurationState state = await service.EvaluateReadinessAsync();

			state.Reason.Should().Be(AIReadinessReason.MissingApiKey);
			state.Message.Should().Contain("key");
		}

		[Test]
		public async Task EvaluateReadiness_OllamaNeedsNoKey()
		{
			var service = CreateService(out AISettings settings, consent: true);
			settings.ActiveProfile.ProviderType = "ollama";
			settings.ActiveProfile.BaseUrl = "http://localhost:11434";

			AIConfigurationState state = await service.EvaluateReadinessAsync();

			state.IsReady.Should().BeTrue();
		}

		[Test]
		public async Task EvaluateReadiness_CustomProviderAllowsMissingKey()
		{
			var service = CreateService(out AISettings settings, consent: true);
			settings.ActiveProfile.ProviderType = "custom";

			AIConfigurationState state = await service.EvaluateReadinessAsync();

			state.IsReady.Should().BeTrue();
		}

		[Test]
		public async Task EvaluateReadiness_ReportsInvalidEndpoint()
		{
			var service = CreateService(out AISettings settings, consent: true);
			settings.ActiveProfile.ProviderType = "custom";
			settings.ActiveProfile.BaseUrl = "not a uri";

			AIConfigurationState state = await service.EvaluateReadinessAsync();

			state.Reason.Should().Be(AIReadinessReason.InvalidEndpoint);
		}

		[Test]
		public async Task ResolveSnapshot_CapturesImmutableTargetAndCredentials()
		{
			var backend = new FakeBackend();
			backend.Keys["profile-placeholder"] = "unused";
			var service = CreateService(out AISettings settings, consent: true, backend: backend);
			AIProfile profile = settings.ActiveProfile;
			backend.Keys[profile.CredentialId] = "sk-profile-key";
			profile.HasStoredKey = true;
			settings.MaxContextTokens = 16000;

			AISelectionSnapshot snapshot = await service.ResolveSnapshotAsync();

			snapshot.ProfileId.Should().Be(profile.Id);
			snapshot.ProviderType.Should().Be("openai");
			snapshot.Endpoint.Should().Be("https://api.openai.com");
			snapshot.Model.Should().Be("gpt-4o");
			snapshot.ApiKey.Should().Be("sk-profile-key");
			snapshot.MaxContextTokens.Should().Be(16000);

			settings.ActiveProfile.BaseUrl = "https://changed.example.test";
			snapshot.Endpoint.Should().Be("https://api.openai.com", "later settings edits cannot mutate a resolved snapshot");
		}

		[Test]
		public async Task ResolveSnapshot_WithoutConsentThrowsConfigurationError()
		{
			var service = CreateService();

			await FluentActions.Awaiting(() => service.ResolveSnapshotAsync())
				.Should().ThrowAsync<AIConfigurationException>();
		}

		[Test]
		public async Task ResolveSnapshot_DistinguishesUnavailableStoreFromMissingKey()
		{
			var backend = new FakeBackend { IsUnavailable = true };
			var service = CreateService(out AISettings settings, consent: true, backend: backend);
			settings.ActiveProfile.HasStoredKey = true;

			await FluentActions.Awaiting(() => service.ResolveSnapshotAsync())
				.Should().ThrowAsync<AIConfigurationException>()
				.WithMessage("*unavailable*");
		}

		[Test]
		public async Task ApplySelection_RestoresPerProfileModelMemoryAndPersists()
		{
			var service = CreateService(out AISettings settings, out List<int> persistCalls, consent: true);
			var second = AIProfile.Create(AIProviderCatalog.Get("anthropic"));
			second.Name = "Work";
			second.Models.Add("claude-sonnet");
			second.LastSelectedModel = "claude-sonnet";
			settings.Profiles.Add(second);

			await service.ApplySelectionAsync(second.Id, string.Empty);

			settings.ActiveProfileId.Should().Be(second.Id);
			service.ActiveSelection.Model.Should().Be("claude-sonnet", "per-profile model memory is restored");
			persistCalls.Should().HaveCount(1, "selector changes persist immediately");

			await service.ApplySelectionAsync(second.Id, AIProviderCatalog.Get("anthropic").DefaultModel);

			second.LastSelectedModel.Should().Be(AIProviderCatalog.Get("anthropic").DefaultModel);
		}

		[Test]
		public async Task ApplySelection_UnknownProfileIsRejected()
		{
			var service = CreateService(out _, consent: true);

			await FluentActions.Awaiting(() => service.ApplySelectionAsync("missing-id", "gpt-4o"))
				.Should().ThrowAsync<AIConfigurationException>();
		}

		[Test]
		public async Task DeleteProfile_ActiveDeletionSelectsFollowingProfile()
		{
			var service = CreateService(out AISettings settings, consent: true, backend: new FakeBackend());
			var first = settings.Profiles[0];
			var second = AIProfile.Create(AIProviderCatalog.Get("ollama"));
			second.Name = "Second";
			settings.Profiles.Add(second);
			settings.ActiveProfileId = first.Id;

			await service.DeleteProfileAsync(first.Id);

			settings.ActiveProfileId.Should().Be(second.Id, "deleting the active profile selects the following visible profile");
		}

		[Test]
		public async Task DeleteProfile_ActiveLastDeletionWrapsToFirstRemaining()
		{
			var service = CreateService(out AISettings settings, consent: true, backend: new FakeBackend());
			var first = settings.Profiles[0];
			var second = AIProfile.Create(AIProviderCatalog.Get("ollama"));
			second.Name = "Second";
			var third = AIProfile.Create(AIProviderCatalog.Get("openai"));
			third.Name = "Third";
			settings.Profiles.Add(second);
			settings.Profiles.Add(third);
			settings.ActiveProfileId = third.Id;

			await service.DeleteProfileAsync(third.Id);

			settings.ActiveProfileId.Should().Be(first.Id, "deleting the last active profile wraps to the first remaining profile");
		}

		[Test]
		public async Task DeleteProfile_LastProfileCannotBeDeleted()
		{
			var service = CreateService(out AISettings settings, consent: true);

			await FluentActions.Awaiting(() => service.DeleteProfileAsync(settings.Profiles[0].Id))
				.Should().ThrowAsync<AIConfigurationException>()
				.WithMessage("*only profile*");
			settings.Profiles.Should().HaveCount(1);
		}

		[Test]
		public async Task DeleteProfile_DeletesSecretBeforeMetadata()
		{
			var backend = new FakeBackend { FailOnDelete = true };
			var service = CreateService(out AISettings settings, consent: true, backend: backend);
			var doomed = AIProfile.Create(AIProviderCatalog.Get("openai"));
			doomed.Name = "Doomed";
			doomed.HasStoredKey = true;
			settings.Profiles.Add(doomed);
			backend.Keys[doomed.CredentialId] = "sk-doomed";

			await FluentActions.Awaiting(() => service.DeleteProfileAsync(doomed.Id))
				.Should().ThrowAsync<SecureKeyStorageUnavailableException>();

			settings.Profiles.Should().Contain(doomed, "a failed secret deletion aborts metadata deletion");
		}

		[Test]
		public async Task SelectionChanged_PublishesOneNotificationPerApply()
		{
			var service = CreateService(out AISettings settings, consent: true);
			int notifications = 0;
			service.SelectionChanged += (_, _) => notifications++;

			await service.ApplySelectionAsync(settings.Profiles[0].Id, "gpt-4o");

			notifications.Should().Be(1);
		}

		static AISelectionService CreateService(bool consent = false, FakeBackend? backend = null)
		{
			return CreateService(out _, consent, backend);
		}

		static AISelectionService CreateService(out AISettings settings, bool consent = false, FakeBackend? backend = null)
		{
			return CreateService(out settings, out _, consent, backend);
		}

		static AISelectionService CreateService(out AISettings settings, out List<int> persistCalls, bool consent = false, FakeBackend? backend = null)
		{
			settings = new AISettings { PrivacyConsentAccepted = consent };
			persistCalls = new List<int>();
			List<int> calls = persistCalls;
			return new AISelectionService(settings, new SecureKeyStorage(backend ?? new FakeBackend()), () => {
				calls.Add(1);
				return Task.CompletedTask;
			});
		}

		sealed class FakeBackend : ISecureKeyStorageBackend
		{
			public Dictionary<string, string> Keys { get; } = new();
			public bool IsUnavailable { get; init; }
			public bool FailOnDelete { get; init; }

			public Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				Keys[provider] = key;
				return Task.CompletedTask;
			}

			public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
			{
				ThrowIfUnavailable();
				return Task.FromResult(Keys.TryGetValue(provider, out string? key)
					? SecureKeyStorageBackendReadResult.Found(key)
					: SecureKeyStorageBackendReadResult.NotFound);
			}

			public Task DeleteAsync(string provider, CancellationToken cancellationToken)
			{
				if (FailOnDelete)
					throw new SecureKeyStorageUnavailableException("delete failed");
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
