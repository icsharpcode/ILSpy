// Copyright (c) 2026 Masroor
using System;
using System.Net.Http;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.Settings;
using Microsoft.Extensions.Logging;

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
#pragma warning disable MEF009 // Multiple constructors by design: public for MEF, internal for testing
#pragma warning disable MEF002 // AllowDefault import is required for optional ILoggerFactory
	public sealed class AIProviderFactory : IAIProviderFactory, IDisposable
#pragma warning restore MEF002
#pragma warning restore MEF009
	{
		readonly SecureKeyStorage keyStorage;
		readonly HttpClient httpClient;
		readonly bool ownsHttpClient;
		readonly ILoggerFactory? loggerFactory;

		[ImportingConstructor]
		public AIProviderFactory(
#pragma warning disable MEF002 // AllowDefault import is required for optional ILoggerFactory
			[Import(AllowDefault = true)] ILoggerFactory? loggerFactory
#pragma warning restore MEF002
		)
		{
			this.keyStorage = new SecureKeyStorage();
			this.httpClient = new HttpClient();
			this.ownsHttpClient = true;
			this.loggerFactory = loggerFactory;
		}

		internal AIProviderFactory(SecureKeyStorage keyStorage, HttpClient httpClient, ILoggerFactory? loggerFactory = null)
		{
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			this.ownsHttpClient = false;
			this.loggerFactory = loggerFactory;
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

			return provider == "anthropic"
				? new Providers.AnthropicProvider(settings.BaseUrl, apiKey!, settings.Model, httpClient)
				: new Providers.OpenAIProvider(settings.BaseUrl, apiKey, settings.Model, httpClient, loggerFactory);
		}

		public void Dispose()
		{
			if (ownsHttpClient)
				httpClient.Dispose();
		}
	}
}
