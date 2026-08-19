// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.AI.Providers;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class AIProviderFactorySnapshotTests
	{
		[Test]
		public async Task Snapshot_CreatesOpenAiCompatibleProvider()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			ILLMProvider provider = await factory.CreateAsync(Snapshot("openai", apiKey: "sk-test"));

			provider.Should().BeOfType<OpenAIProvider>();
		}

		[Test]
		public async Task Snapshot_CreatesAnthropicProviderFromCapability()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			ILLMProvider provider = await factory.CreateAsync(Snapshot("anthropic", apiKey: "sk-ant-test"));

			provider.Should().BeOfType<AnthropicProvider>();
		}

		[Test]
		public async Task Snapshot_OllamaNeedsNoKey()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			ILLMProvider provider = await factory.CreateAsync(Snapshot("ollama", apiKey: null));

			provider.Should().BeOfType<OpenAIProvider>();
		}

		[Test]
		public async Task Snapshot_UnsupportedProviderFailsDeterministically()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			await FluentActions.Awaiting(() => factory.CreateAsync(Snapshot("made-up", apiKey: "x")))
				.Should().ThrowAsync<AIConfigurationException>();
		}

		[Test]
		public async Task Snapshot_RequiredProviderWithoutKeyFailsDeterministically()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			await FluentActions.Awaiting(() => factory.CreateAsync(Snapshot("openai", apiKey: null)))
				.Should().ThrowAsync<AIConfigurationException>()
				.WithMessage("*API key*");
		}

		[Test]
		public async Task Snapshot_BlankEndpointAndModelFailDeterministically()
		{
			using var factory = new AIProviderFactory(loggerFactory: null);

			await FluentActions.Awaiting(() => factory.CreateAsync(Snapshot("openai", apiKey: "x", endpoint: " ")))
				.Should().ThrowAsync<AIConfigurationException>();
			await FluentActions.Awaiting(() => factory.CreateAsync(Snapshot("openai", apiKey: "x", model: " ")))
				.Should().ThrowAsync<AIConfigurationException>();
		}

		[Test]
		public async Task LegacyAdapter_LoadsMigratedProfileCredential()
		{
			var backend = new RecordingBackend();
			string credentialId;
			var settings = new AISettings {
				PrivacyConsentAccepted = true
			};
			settings.ApiKeyPlaceholder = "configured";
			credentialId = settings.ActiveProfile.CredentialId;
			backend.Keys[credentialId] = "sk-migrated-key";
			var storage = new SecureKeyStorage(backend);
			using var factory = new AIProviderFactory(storage, new System.Net.Http.HttpClient());

			ILLMProvider provider = await factory.CreateAsync(settings);

			provider.Should().BeOfType<OpenAIProvider>();
			backend.LoadCount.Should().BeGreaterThan(0, "the adapter resolves the profile credential id");
		}

		static AISelectionSnapshot Snapshot(string providerType, string? apiKey, string endpoint = "https://api.openai.com", string model = "gpt-4o")
		{
			return new AISelectionSnapshot {
				ProfileId = "p1",
				ProfileName = "Default",
				ProviderType = providerType,
				Endpoint = endpoint,
				Model = model,
				ApiKey = apiKey,
				CredentialId = "profile-abc"
			};
		}

		sealed class RecordingBackend : ISecureKeyStorageBackend
		{
			public System.Collections.Generic.Dictionary<string, string> Keys { get; } = new();
			public int LoadCount { get; private set; }

			public Task SaveAsync(string provider, string key, System.Threading.CancellationToken cancellationToken)
			{
				Keys[provider] = key;
				return Task.CompletedTask;
			}

			public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, System.Threading.CancellationToken cancellationToken)
			{
				LoadCount++;
				return Task.FromResult(Keys.TryGetValue(provider, out string? key)
					? SecureKeyStorageBackendReadResult.Found(key)
					: SecureKeyStorageBackendReadResult.NotFound);
			}

			public Task DeleteAsync(string provider, System.Threading.CancellationToken cancellationToken)
			{
				Keys.Remove(provider);
				return Task.CompletedTask;
			}
		}
	}
}
