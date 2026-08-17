// Copyright (c) 2026 Masroor
using System;
using System.Net.Http;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	public interface IAIProviderFactory
	{
		Task<ILLMProvider> CreateAsync(AISettings settings, CancellationToken cancellationToken = default);
	}

	public sealed class AIConfigurationException : Exception
	{
		public AIConfigurationException(string message) : base(message) { }
	}

	/// <summary>Creates the configured provider without exposing secure-store or HttpClient details to UI code.</summary>
	[Export(typeof(IAIProviderFactory))]
	[Shared]
	public sealed class AIProviderFactory : IAIProviderFactory, IDisposable
	{
		readonly SecureKeyStorage keyStorage;
		readonly HttpClient httpClient;
		readonly bool ownsHttpClient;

		public AIProviderFactory()
			: this(new SecureKeyStorage(), new HttpClient(), true)
		{
		}

		internal AIProviderFactory(SecureKeyStorage keyStorage, HttpClient httpClient, bool ownsHttpClient = false)
		{
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			this.ownsHttpClient = ownsHttpClient;
		}

		public async Task<ILLMProvider> CreateAsync(AISettings settings, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(settings);
			if (!settings.PrivacyConsentAccepted)
				throw new AIConfigurationException("Accept the privacy notice before using AI.");

			string provider = AISettings.NormalizeProvider(settings.Provider);
			if (!AISettings.IsSupportedProvider(provider))
				throw new AIConfigurationException($"Provider '{provider}' is not supported in this version.");
			if (string.IsNullOrWhiteSpace(settings.BaseUrl))
				throw new AIConfigurationException("Configure an AI endpoint.");
			if (string.IsNullOrWhiteSpace(settings.Model))
				throw new AIConfigurationException("Configure an AI model.");

			string? apiKey = settings.ApiKey;
			if (string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(settings.ApiKeyPlaceholder))
			{
				SecureKeyLookupResult result = await keyStorage.TryLoadKeyAsync(provider, cancellationToken).ConfigureAwait(false);
				if (result.Status == SecureKeyLookupStatus.Unavailable)
					throw new AIConfigurationException("Secure API-key storage is unavailable.");
				apiKey = result.Value;
				if (!string.IsNullOrWhiteSpace(apiKey))
					settings.ApiKey = apiKey;
			}

			if (provider is not "ollama" && string.IsNullOrWhiteSpace(apiKey))
				throw new AIConfigurationException("Configure an API key before using this provider.");

			return new Providers.OpenAIProvider(settings.BaseUrl, apiKey, settings.Model, httpClient);
		}

		public void Dispose()
		{
			if (ownsHttpClient)
				httpClient.Dispose();
		}
	}
}
