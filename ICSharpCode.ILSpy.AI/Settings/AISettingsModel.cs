// Copyright (c) 2026 Dr. Masroor Ehsan
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Settings for AI/LLM integration. Schema 2 persists an ordered collection of AI profiles,
	/// one active profile/model selection, credential-migration state, and application-wide
	/// context/privacy preferences. The legacy singleton members (Provider, BaseUrl, Model)
	/// remain as a facade over the active profile until all consumers migrate.
	/// </summary>
	/// <remarks>
	/// Portable model: the desktop host wraps it in an <c>ISettingsSection</c> adapter that owns
	/// the section registration; this type owns the XML translation itself so the persisted
	/// schema (element names, ordering, default omission) stays with the state it serializes.
	/// </remarks>
	public class AISettingsModel : INotifyPropertyChanged
	{
		public const string DefaultProvider = "openai";
		public const int DefaultMaxContextTokens = 32000;
		public const int MinimumMaxContextTokens = 4000;
		public const int MaximumMaxContextTokens = 128000;

		/// <summary>The persistence schema version written by <see cref="SaveToXml"/>.</summary>
		public const int CurrentSchemaVersion = 2;

		/// <summary>The XML element name of the settings section that serializes this model.</summary>
		public const string SectionElementName = "AISettings";

		string apiKey = string.Empty;
		string apiKeyPlaceholder = string.Empty;
		int maxContextTokens = DefaultMaxContextTokens;
		bool streamResponses = true;
		bool wordWrap = true;
		bool sendIL;
		bool sendCallGraph;
		bool privacyConsentAccepted;
		string activeProfileId = string.Empty;
		bool credentialMigrationPending;

		public AISettingsModel()
		{
			Profiles.Add(CreateDefaultProfile());
			activeProfileId = Profiles[0].Id;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Ordered, user-managed AI profiles. XML order is the persisted order.
		/// Mutations go through the shared selection service; <see cref="NotifyProfilesChanged"/>
		/// raises the change notification.
		/// </summary>
		public ObservableCollection<AIProfile> Profiles { get; } = new();

		/// <summary>Id of the active profile. Always identifies an element of <see cref="Profiles"/>.</summary>
		public string ActiveProfileId {
			get => activeProfileId;
			set {
				string candidate = value ?? string.Empty;
				if (Profiles.All(p => p.Id != candidate))
					candidate = Profiles.Count > 0 ? Profiles[0].Id : string.Empty;
				SetProperty(ref activeProfileId, candidate);
			}
		}

		/// <summary>
		/// True while a legacy provider credential still awaits confirmation under the migrated
		/// profile identity. The legacy key remains authoritative until then.
		/// </summary>
		public bool CredentialMigrationPending {
			get => credentialMigrationPending;
			private set => SetProperty(ref credentialMigrationPending, value);
		}

		/// <summary>The schema version understood by the loaded settings.</summary>
		public int SchemaVersion => CurrentSchemaVersion;

		/// <summary>
		/// The active profile; never null while <see cref="Profiles"/> is non-empty.
		/// </summary>
		public AIProfile ActiveProfile => Profiles.Count == 0
			? throw new InvalidOperationException("AI settings must always contain at least one profile.")
			: Profiles.FirstOrDefault(p => p.Id == activeProfileId) ?? Profiles[0];

		public void NotifyProfilesChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profiles)));
		}

		public void MarkCredentialMigrationComplete()
		{
			CredentialMigrationPending = false;
		}

		/// <summary>
		/// Legacy facade: provider type of the active profile.
		/// </summary>
		public string Provider {
			get => ActiveProfile.ProviderType;
			set {
				string normalizedProvider = NormalizeProvider(value);
				AIProfile profile = ActiveProfile;
				string previousProvider = profile.ProviderType;
				string previousBaseUrl = profile.BaseUrl;
				string previousModel = ResolveLegacyModel(profile);
				if (normalizedProvider == previousProvider)
					return;

				profile.ProviderType = normalizedProvider;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Provider)));

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
		/// Legacy facade: non-secret hint that the active profile has a stored credential.
		/// Reads derive from the profile's HasStoredKey flag; writes keep the hint in sync.
		/// </summary>
		public string ApiKeyPlaceholder {
			get => ActiveProfile.HasStoredKey ? NormalizePlaceholder(apiKeyPlaceholder) : string.Empty;
			set {
				string normalized = value ?? string.Empty;
				apiKeyPlaceholder = normalized;
				ActiveProfile.HasStoredKey = normalized.Length != 0;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ApiKeyPlaceholder)));
			}
		}

		/// <summary>Legacy facade: endpoint of the active profile.</summary>
		public string BaseUrl {
			get => ActiveProfile.BaseUrl;
			set {
				if (ActiveProfile.BaseUrl != (value ?? string.Empty))
				{
					ActiveProfile.BaseUrl = value ?? string.Empty;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BaseUrl)));
				}
			}
		}

		/// <summary>Legacy facade: selected model of the active profile.</summary>
		public string Model {
			get => ResolveLegacyModel(ActiveProfile);
			set {
				AIProfile profile = ActiveProfile;
				string normalized = value ?? string.Empty;
				if (profile.LastSelectedModel == normalized)
					return;
				profile.LastSelectedModel = normalized;
				if (normalized.Length != 0
					&& !profile.Models.Contains(normalized, StringComparer.OrdinalIgnoreCase))
					profile.Models.Add(normalized);
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model)));
			}
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

		/// <summary>Persisted markdown word-wrap preference for AI surfaces.</summary>
		public bool WordWrap {
			get => wordWrap;
			set => SetProperty(ref wordWrap, value);
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
			Profiles.Clear();
			activeProfileId = string.Empty;
			credentialMigrationPending = false;
			ApiKey = string.Empty;
			apiKeyPlaceholder = string.Empty;

			if (section is null)
			{
				MaxContextTokens = DefaultMaxContextTokens;
				StreamResponses = true;
				WordWrap = true;
				SendIL = false;
				SendCallGraph = false;
				PrivacyConsentAccepted = false;
				Profiles.Add(CreateDefaultProfile());
				ActiveProfileId = Profiles[0].Id;
				return;
			}

			MaxContextTokens = ReadPositiveInt32(section, nameof(MaxContextTokens), DefaultMaxContextTokens);
			StreamResponses = ReadBoolean(section, nameof(StreamResponses), true);
			WordWrap = ReadBoolean(section, nameof(WordWrap), true);
			SendIL = ReadBoolean(section, nameof(SendIL), false);
			SendCallGraph = ReadBoolean(section, nameof(SendCallGraph), false);
			PrivacyConsentAccepted = ReadBoolean(section, nameof(PrivacyConsentAccepted), false);

			XElement? profilesElement = section.Element("Profiles");
			if (profilesElement != null)
			{
				foreach (XElement profileElement in profilesElement.Elements("Profile"))
					Profiles.Add(ReadProfile(profileElement));
				RepairProfiles();
				CredentialMigrationPending = string.Equals(
					(string?)section.Element("CredentialMigration")?.Attribute("State"),
					"Pending", StringComparison.OrdinalIgnoreCase);
				ActiveProfileId = ReadString(section, nameof(ActiveProfileId), Profiles[0].Id);
				return;
			}

			MigrateLegacySection(section);
			ActiveProfileId = Profiles[0].Id;
		}

		public XElement SaveToXml()
		{
			var profilesElement = new XElement("Profiles",
				Profiles.Select(p => new XElement("Profile",
					new XAttribute("Id", p.Id),
					new XAttribute("Name", p.Name),
					new XElement("ProviderType", p.ProviderType),
					new XElement("BaseUrl", p.BaseUrl),
					new XElement("HasStoredKey", p.HasStoredKey),
					new XElement("LastSelectedModel", p.LastSelectedModel),
					new XElement("Models", p.Models.Select(m => new XElement("Model", m))))));

			return new XElement(SectionElementName,
				new XElement("SchemaVersion", CurrentSchemaVersion),
				new XElement(nameof(ActiveProfileId), ActiveProfileId),
				new XElement(nameof(MaxContextTokens), MaxContextTokens),
				new XElement(nameof(StreamResponses), StreamResponses),
				new XElement(nameof(WordWrap), WordWrap),
				new XElement(nameof(SendIL), SendIL),
				new XElement(nameof(SendCallGraph), SendCallGraph),
				new XElement(nameof(PrivacyConsentAccepted), PrivacyConsentAccepted),
				profilesElement,
				new XElement("CredentialMigration",
					new XAttribute("State", CredentialMigrationPending ? "Pending" : "Complete")));
		}

		void MigrateLegacySection(XElement section)
		{
			string provider = NormalizeProvider((string?)section.Element("Provider"));
			if (!IsSupportedProvider(provider))
				provider = DefaultProvider;
			string baseUrl = ReadString(section, nameof(BaseUrl), GetDefaultBaseUrl(provider));
			string model = ReadString(section, nameof(Model), GetDefaultModel(provider));
			string placeholder = ReadString(section, "ApiKeyPlaceholder", string.Empty);

			var profile = new AIProfile {
				Name = "Default",
				ProviderType = provider,
				BaseUrl = baseUrl,
				LastSelectedModel = model,
				HasStoredKey = placeholder.Length != 0,
			};
			profile.Id = Guid.NewGuid().ToString("N");
			profile.Models.Add(model);
			Profiles.Add(profile);
			apiKeyPlaceholder = placeholder;
			CredentialMigrationPending = placeholder.Length != 0;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Provider)));
		}

		static AIProfile ReadProfile(XElement element)
		{
			var profile = new AIProfile {
				Id = ((string?)element.Attribute("Id") ?? string.Empty).Trim(),
				Name = ((string?)element.Attribute("Name") ?? string.Empty).Trim(),
				ProviderType = ((string?)element.Element("ProviderType") ?? string.Empty).Trim().ToLowerInvariant(),
				BaseUrl = ((string?)element.Element("BaseUrl") ?? string.Empty).Trim(),
				LastSelectedModel = ((string?)element.Element("LastSelectedModel") ?? string.Empty).Trim(),
				HasStoredKey = bool.TryParse((string?)element.Element("HasStoredKey"), out bool hasKey) && hasKey,
			};
			XElement? models = element.Element("Models");
			if (models != null)
			{
				var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (XElement modelElement in models.Elements("Model"))
				{
					string model = ((string?)modelElement ?? string.Empty).Trim();
					if (model.Length != 0 && seen.Add(model))
						profile.Models.Add(model);
				}
			}
			return profile;
		}

		void RepairProfiles()
		{
			var ids = new HashSet<string>(StringComparer.Ordinal);
			var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < Profiles.Count; i++)
			{
				AIProfile profile = Profiles[i];
				if (profile.Id.Length == 0 || !ids.Add(profile.Id))
					profile.Id = Guid.NewGuid().ToString("N");
				if (!AIProviderCatalog.TryGet(profile.ProviderType, out AIProviderDescriptor? descriptor))
				{
					descriptor = AIProviderCatalog.Get(DefaultProvider);
					profile.ProviderType = descriptor.Id;
				}
				if (!IsAbsoluteHttpEndpoint(profile.BaseUrl))
					profile.BaseUrl = descriptor.DefaultBaseUrl;
				if (profile.Models.Count == 0)
					profile.Models.Add(descriptor.DefaultModel);
				profile.Name = MakeUniqueName(profile.Name, names);
				if (string.IsNullOrWhiteSpace(profile.LastSelectedModel)
					|| !profile.Models.Contains(profile.LastSelectedModel, StringComparer.OrdinalIgnoreCase))
					profile.LastSelectedModel = profile.Models[0];
			}
			if (Profiles.Count == 0)
				Profiles.Add(CreateDefaultProfile());
		}

		static string MakeUniqueName(string name, HashSet<string> taken)
		{
			if (name.Length != 0 && taken.Add(name))
				return name;
			for (int i = 1; ; i++)
			{
				string candidate = "Profile " + i;
				if (taken.Add(candidate))
					return candidate;
			}
		}

		static bool IsAbsoluteHttpEndpoint(string? endpoint)
		{
			return Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri)
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
		}

		static AIProfile CreateDefaultProfile()
		{
			AIProfile profile = AIProfile.Create(AIProviderCatalog.Get(DefaultProvider));
			profile.Name = "Default";
			return profile;
		}

		static string ResolveLegacyModel(AIProfile profile)
		{
			return profile.ResolveModel();
		}

		static string NormalizePlaceholder(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? "configured" : value;
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
			return NormalizeProvider(provider) is "openai" or "anthropic" or "ollama" or "custom";
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
