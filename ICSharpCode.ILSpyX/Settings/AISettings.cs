// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
	/// <summary>
	/// Settings for AI/LLM integration.
	/// </summary>
	public class AISettings : ISettingsSection
	{
		public const string DefaultProvider = "openai";
		public const int DefaultMaxContextTokens = 32000;
		public const int MinimumMaxContextTokens = 4000;
		public const int MaximumMaxContextTokens = 128000;

		string provider = DefaultProvider;
		string apiKey = string.Empty;
		string apiKeyPlaceholder = string.Empty;
		string baseUrl = GetDefaultBaseUrl(DefaultProvider);
		string model = GetDefaultModel(DefaultProvider);
		int maxContextTokens = DefaultMaxContextTokens;
		bool streamResponses = true;
		bool sendIL;
		bool sendCallGraph;
		bool privacyConsentAccepted;

		public event PropertyChangedEventHandler? PropertyChanged;

		public XName SectionName => "AISettings";

		/// <summary>
		/// Configured provider identifier. Phase 0 implements the OpenAI-compatible provider;
		/// Anthropic, Ollama, and custom values retain their settings for later providers.
		/// </summary>
		public string Provider {
			get => provider;
			set {
				string normalizedProvider = NormalizeProvider(value);
				string previousProvider = provider;
				string previousBaseUrl = BaseUrl;
				string previousModel = Model;
				if (!SetProperty(ref provider, normalizedProvider))
					return;

				// Provider changes update untouched defaults, but preserve an explicit custom
				// endpoint/model. This keeps a user-configured compatible endpoint intact.
				if (string.IsNullOrWhiteSpace(previousBaseUrl)
					|| string.Equals(previousBaseUrl, GetDefaultBaseUrl(previousProvider), StringComparison.OrdinalIgnoreCase))
					BaseUrl = GetDefaultBaseUrl(normalizedProvider);
				if (string.IsNullOrWhiteSpace(previousModel)
					|| string.Equals(previousModel, GetDefaultModel(previousProvider), StringComparison.OrdinalIgnoreCase))
					Model = GetDefaultModel(normalizedProvider);
			}
		}

		/// <summary>
		/// Runtime API key. This value is never serialized by <see cref="SaveToXml"/>.
		/// </summary>
		public string ApiKey {
			get => apiKey;
			set => SetProperty(ref apiKey, value ?? string.Empty);
		}

		/// <summary>
		/// Non-secret reference used by secure key storage.
		/// </summary>
		public string ApiKeyPlaceholder {
			get => apiKeyPlaceholder;
			set => SetProperty(ref apiKeyPlaceholder, value ?? string.Empty);
		}

		public string BaseUrl {
			get => baseUrl;
			set => SetProperty(ref baseUrl, value ?? string.Empty);
		}

		public string Model {
			get => model;
			set => SetProperty(ref model, value ?? string.Empty);
		}

		public int MaxContextTokens {
			get => maxContextTokens;
			set => SetProperty(ref maxContextTokens, value > 0 ? value : DefaultMaxContextTokens);
		}

		/// <summary>
		/// Persisted response-streaming preference for the AI output experience.
		/// </summary>
		public bool StreamResponses {
			get => streamResponses;
			set => SetProperty(ref streamResponses, value);
		}

		/// <summary>
		/// Persisted opt-in for adding IL to context requests.
		/// </summary>
		public bool SendIL {
			get => sendIL;
			set => SetProperty(ref sendIL, value);
		}

		/// <summary>
		/// Persisted opt-in for adding callers and callees to context requests.
		/// </summary>
		public bool SendCallGraph {
			get => sendCallGraph;
			set => SetProperty(ref sendCallGraph, value);
		}

		/// <summary>
		/// Indicates that the user accepted the AI data-sharing notice. AI features must
		/// remain disabled until this value is true.
		/// </summary>
		public bool PrivacyConsentAccepted {
			get => privacyConsentAccepted;
			set => SetProperty(ref privacyConsentAccepted, value);
		}

		public void LoadFromXml(XElement section)
		{
			if (section is null)
			{
				Provider = DefaultProvider;
				ApiKey = string.Empty;
				ApiKeyPlaceholder = string.Empty;
				BaseUrl = GetDefaultBaseUrl(DefaultProvider);
				Model = GetDefaultModel(DefaultProvider);
				MaxContextTokens = DefaultMaxContextTokens;
				StreamResponses = true;
				SendIL = false;
				SendCallGraph = false;
				PrivacyConsentAccepted = false;
				return;
			}

			Provider = ReadString(section, nameof(Provider), DefaultProvider);
			ApiKey = string.Empty;
			ApiKeyPlaceholder = ReadString(section, nameof(ApiKeyPlaceholder), string.Empty);
			BaseUrl = ReadString(section, nameof(BaseUrl), GetDefaultBaseUrl(Provider));
			Model = ReadString(section, nameof(Model), GetDefaultModel(Provider));
			MaxContextTokens = ReadPositiveInt32(section, nameof(MaxContextTokens), DefaultMaxContextTokens);
			StreamResponses = ReadBoolean(section, nameof(StreamResponses), true);
			SendIL = ReadBoolean(section, nameof(SendIL), false);
			SendCallGraph = ReadBoolean(section, nameof(SendCallGraph), false);
			PrivacyConsentAccepted = ReadBoolean(section, nameof(PrivacyConsentAccepted), false);
		}

		public XElement SaveToXml()
		{
			return new XElement(SectionName,
				new XElement(nameof(Provider), Provider),
				new XElement(nameof(ApiKeyPlaceholder), ApiKeyPlaceholder),
				new XElement(nameof(BaseUrl), BaseUrl),
				new XElement(nameof(Model), Model),
				new XElement(nameof(MaxContextTokens), MaxContextTokens),
				new XElement(nameof(StreamResponses), StreamResponses),
				new XElement(nameof(SendIL), SendIL),
				new XElement(nameof(SendCallGraph), SendCallGraph),
				new XElement(nameof(PrivacyConsentAccepted), PrivacyConsentAccepted));
		}

		public static string NormalizeProvider(string? value)
		{
			return string.IsNullOrWhiteSpace(value) ? DefaultProvider : value.Trim().ToLowerInvariant();
		}

		public static string GetDefaultBaseUrl(string provider)
		{
			return provider switch {
				"anthropic" => "https://api.anthropic.com",
				"ollama" => "http://localhost:11434",
				_ => "https://api.openai.com"
			};
		}

		public static string GetDefaultModel(string provider)
		{
			return provider switch {
				"anthropic" => "claude-opus-4-8",
				"ollama" => "llama3:70b",
				_ => "gpt-4o"
			};
		}

		public static bool IsSupportedProvider(string? provider)
		{
			return NormalizeProvider(provider) is "openai" or "ollama" or "custom";
		}

		static string ReadString(XElement section, string name, string defaultValue)
		{
			string? value = (string?)section.Element(name);
			return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
		}

		static int ReadPositiveInt32(XElement section, string name, int defaultValue)
		{
			return int.TryParse((string?)section.Element(name), out int value) && value > 0 ? value : defaultValue;
		}

		static bool ReadBoolean(XElement section, string name, bool defaultValue)
		{
			return bool.TryParse((string?)section.Element(name), out bool value) ? value : defaultValue;
		}

		bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (Equals(field, value))
				return false;
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
