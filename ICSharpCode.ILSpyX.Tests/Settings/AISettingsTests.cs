// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;
using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.Settings
{
	[TestFixture]
	public class AISettingsTests
	{
		[Test]
		public void Defaults_AreValid()
		{
			var settings = new AISettings();

			settings.Should().BeAssignableTo<ISettingsSection>();
			settings.Provider.Should().Be("openai");
			settings.BaseUrl.Should().Be("https://api.openai.com");
			settings.Model.Should().Be("gpt-4o");
			settings.MaxContextTokens.Should().Be(32000);
			settings.MaxContextTokens.Should().BePositive();
			settings.StreamResponses.Should().BeTrue();
			settings.WordWrap.Should().BeTrue();
			settings.SendIL.Should().BeFalse();
			settings.SendCallGraph.Should().BeFalse();
			settings.PrivacyConsentAccepted.Should().BeFalse();
		}

		[TestCase("anthropic", "https://api.anthropic.com", "claude-opus-4-8")]
		[TestCase("ollama", "http://localhost:11434", "llama3:70b")]
		public void MissingProviderSettings_UseProviderDefaults(string provider, string baseUrl, string model)
		{
			var settings = new AISettings();

			settings.LoadFromXml(new XElement("AISettings", new XElement("Provider", provider)));

			settings.BaseUrl.Should().Be(baseUrl);
			settings.Model.Should().Be(model);
		}

		[Test]
		public void ChangingProvider_PreservesExplicitEndpointAndModel()
		{
			var settings = new AISettings {
				BaseUrl = "https://proxy.example.test",
				Model = "proxy-model"
			};

			settings.Provider = "custom";

			settings.BaseUrl.Should().Be("https://proxy.example.test");
			settings.Model.Should().Be("proxy-model");
		}

		[Test]
		public void SaveAndLoad_RoundTripsPersistedValues()
		{
			var original = new AISettings {
				Provider = "anthropic",
				ApiKeyPlaceholder = "stored-key-reference",
				BaseUrl = "https://example.test",
				Model = "custom-model",
				MaxContextTokens = 16000,
				StreamResponses = false,
				WordWrap = false,
				SendIL = true,
				SendCallGraph = true,
				PrivacyConsentAccepted = true
			};

			var loaded = new AISettings();
			loaded.LoadFromXml(original.SaveToXml());

			loaded.Provider.Should().Be(original.Provider);
			loaded.ApiKeyPlaceholder.Should().Be("configured", "the legacy placeholder facade derives a non-secret hint from the stored-key flag");
			loaded.ApiKeyPlaceholder.Should().NotBe("stored-key-reference", "placeholder values are runtime-only and never serialized");
			loaded.Profiles[0].HasStoredKey.Should().BeTrue();
			loaded.BaseUrl.Should().Be(original.BaseUrl);
			loaded.Model.Should().Be(original.Model);
			loaded.MaxContextTokens.Should().Be(original.MaxContextTokens);
			loaded.StreamResponses.Should().Be(original.StreamResponses);
			loaded.WordWrap.Should().Be(original.WordWrap);
			loaded.SendIL.Should().Be(original.SendIL);
			loaded.SendCallGraph.Should().Be(original.SendCallGraph);
			loaded.PrivacyConsentAccepted.Should().Be(original.PrivacyConsentAccepted);
		}

		[Test]
		public void SaveToXml_NeverSerializesApiKey()
		{
			var settings = new AISettings { ApiKey = "secret-api-key" };

			var xml = settings.SaveToXml().ToString(SaveOptions.DisableFormatting);

			xml.Should().NotContain("secret-api-key");
			settings.SaveToXml().Element(nameof(AISettings.ApiKey)).Should().BeNull();
		}

		[Test]
		public void LoadFromXml_NullUsesDefaults()
		{
			var settings = new AISettings { Provider = "anthropic", MaxContextTokens = 1000 };

			settings.LoadFromXml(null!);

			settings.Provider.Should().Be("openai");
			settings.MaxContextTokens.Should().Be(32000);
			settings.WordWrap.Should().BeTrue();
		}

		[Test]
		public void LoadFromXml_MalformedValuesKeepSafeDefaults()
		{
			var settings = new AISettings();

			settings.LoadFromXml(new XElement("AISettings",
				new XElement("Provider", "anthropic"),
				new XElement("MaxContextTokens", "not-a-number"),
				new XElement("StreamResponses", "not-a-boolean"),
				new XElement("WordWrap", "not-a-boolean"),
				new XElement("SendIL", "not-a-boolean"),
				new XElement("PrivacyConsentAccepted", "not-a-boolean")));

			settings.Provider.Should().Be("anthropic");
			settings.BaseUrl.Should().Be("https://api.anthropic.com");
			settings.Model.Should().Be("claude-opus-4-8");
			settings.MaxContextTokens.Should().Be(32000);
			settings.MaxContextTokens.Should().BePositive();
			settings.StreamResponses.Should().BeTrue();
			settings.WordWrap.Should().BeTrue();
			settings.SendIL.Should().BeFalse();
			settings.PrivacyConsentAccepted.Should().BeFalse();
		}

		[Test]
		public void MaxContextTokens_IsAlwaysPositive()
		{
			var settings = new AISettings();

			settings.MaxContextTokens = 0;
			settings.MaxContextTokens.Should().BePositive();
			settings.MaxContextTokens = -10;
			settings.MaxContextTokens.Should().BePositive();

			settings.LoadFromXml(new XElement("AISettings", new XElement("MaxContextTokens", "0")));
			settings.MaxContextTokens.Should().BePositive();
		}

		[Test]
		public void PropertyChanges_RaiseNotifications()
		{
			var settings = new AISettings();
			var changed = new List<string?>();
			settings.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

			settings.Provider = "anthropic";
			settings.MaxContextTokens = 16000;
			settings.WordWrap = false;

			changed.Should().Contain("Provider");
			changed.Should().Contain("MaxContextTokens");
			changed.Should().Contain("WordWrap");
		}
	}
}
