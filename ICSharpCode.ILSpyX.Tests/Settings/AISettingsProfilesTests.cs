// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Linq;
using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.Settings
{
	[TestFixture]
	public class AISettingsProfilesTests
	{
		[Test]
		public void NewSettings_ContainsSingleDefaultOpenAiProfile()
		{
			var settings = new AISettings();

			settings.SchemaVersion.Should().Be(2);
			settings.Profiles.Should().HaveCount(1);
			AIProfile profile = settings.Profiles[0];
			profile.Id.Should().NotBeNullOrWhiteSpace();
			profile.Name.Should().Be("Default");
			profile.ProviderType.Should().Be("openai");
			profile.Models.Should().Equal("gpt-4o");
			settings.ActiveProfileId.Should().Be(profile.Id);
			settings.PrivacyConsentAccepted.Should().BeFalse();
		}

		[Test]
		public void SaveToXml_WritesSchema2ShapeWithoutSecrets()
		{
			var settings = new AISettings();
			settings.ApiKey = "secret-api-key";
			AIProfile profile = settings.Profiles[0];
			profile.HasStoredKey = true;

			XElement xml = settings.SaveToXml();

			xml.Element("SchemaVersion")!.Value.Should().Be("2");
			xml.Element("ActiveProfileId")!.Value.Should().Be(profile.Id);
			xml.Element("Provider").Should().BeNull("schema 2 replaces the singleton provider element");
			xml.Element("ApiKeyPlaceholder").Should().BeNull("schema 2 has no legacy placeholder element");
			xml.ToString().Should().NotContain("secret-api-key");

			XElement profiles = xml.Element("Profiles")!;
			XElement profileXml = profiles.Elements("Profile").Should().ContainSingle().Subject;
			profileXml.Attribute("Id")!.Value.Should().Be(profile.Id);
			profileXml.Attribute("Name")!.Value.Should().Be("Default");
			profileXml.Element("ProviderType")!.Value.Should().Be("openai");
			profileXml.Element("HasStoredKey")!.Value.Should().Be("true");
			profileXml.Element("Models")!.Elements("Model").Select(m => m.Value).Should().Equal("gpt-4o");
		}

		[Test]
		public void LegacySingletonSettings_MigrateToOneDefaultProfile()
		{
			var xml = new XElement("AISettings",
				new XElement("Provider", "anthropic"),
				new XElement("ApiKeyPlaceholder", "stored-key-reference"),
				new XElement("BaseUrl", "https://staging.example.test"),
				new XElement("Model", "claude-special"),
				new XElement("PrivacyConsentAccepted", "true"));
			var settings = new AISettings();

			settings.LoadFromXml(xml);

			settings.Profiles.Should().HaveCount(1);
			AIProfile profile = settings.Profiles[0];
			profile.Name.Should().Be("Default");
			profile.ProviderType.Should().Be("anthropic");
			profile.BaseUrl.Should().Be("https://staging.example.test");
			profile.Models.Should().Equal("claude-special");
			profile.LastSelectedModel.Should().Be("claude-special");
			profile.HasStoredKey.Should().BeTrue("a legacy key placeholder means a credential exists");
			settings.ActiveProfileId.Should().Be(profile.Id);
			settings.CredentialMigrationPending.Should().BeTrue("the secure key still sits under the legacy provider id");
			settings.PrivacyConsentAccepted.Should().BeTrue("legacy global preferences survive migration");
		}

		[Test]
		public void LegacySettings_WithBlankFields_UseProviderDefaults()
		{
			var xml = new XElement("AISettings", new XElement("Provider", "ollama"));
			var settings = new AISettings();

			settings.LoadFromXml(xml);

			AIProfile profile = settings.Profiles.Should().ContainSingle().Subject;
			profile.ProviderType.Should().Be("ollama");
			profile.BaseUrl.Should().Be("http://localhost:11434");
			profile.Models.Should().Equal("llama3:70b");
			profile.HasStoredKey.Should().BeFalse();
			settings.CredentialMigrationPending.Should().BeFalse();
		}

		[Test]
		public void Migration_IsIdempotentAcrossSaveLoadCycles()
		{
			var xml = new XElement("AISettings",
				new XElement("Provider", "anthropic"),
				new XElement("Model", "claude-special"));
			var settings = new AISettings();
			settings.LoadFromXml(xml);

			var reloaded = new AISettings();
			reloaded.LoadFromXml(settings.SaveToXml());
			var thirdPass = new AISettings();
			thirdPass.LoadFromXml(reloaded.SaveToXml());

			thirdPass.Profiles.Should().HaveCount(1, "repeated loads must not create duplicate profiles");
			thirdPass.Profiles[0].Id.Should().Be(settings.Profiles[0].Id, "the migrated profile id stays stable");
			thirdPass.Profiles[0].Models.Should().Equal("claude-special");
		}

		[Test]
		public void LoadFromXml_Schema2_RoundTripsProfilesInOrder()
		{
			var xml = XElement.Parse("""
				<AISettings>
				  <SchemaVersion>2</SchemaVersion>
				  <ActiveProfileId>p2</ActiveProfileId>
				  <MaxContextTokens>5000</MaxContextTokens>
				  <StreamResponses>false</StreamResponses>
				  <WordWrap>false</WordWrap>
				  <SendIL>true</SendIL>
				  <SendCallGraph>true</SendCallGraph>
				  <PrivacyConsentAccepted>true</PrivacyConsentAccepted>
				  <Profiles>
				    <Profile Id="p1" Name="First">
				      <ProviderType>openai</ProviderType>
				      <BaseUrl>https://api.openai.com</BaseUrl>
				      <HasStoredKey>true</HasStoredKey>
				      <LastSelectedModel>gpt-4o-mini</LastSelectedModel>
				      <Models><Model>gpt-4o</Model><Model>gpt-4o-mini</Model></Models>
				    </Profile>
				    <Profile Id="p2" Name="Second">
				      <ProviderType>ollama</ProviderType>
				      <BaseUrl>http://localhost:11434</BaseUrl>
				      <HasStoredKey>false</HasStoredKey>
				      <LastSelectedModel>llama3:70b</LastSelectedModel>
				      <Models><Model>llama3:70b</Model></Models>
				    </Profile>
				  </Profiles>
				  <CredentialMigration State="Pending" />
				</AISettings>
				""");
			var settings = new AISettings();

			settings.LoadFromXml(xml);

			settings.Profiles.Select(p => p.Id).Should().Equal("p1", "p2");
			settings.ActiveProfileId.Should().Be("p2");
			settings.Profiles[0].Models.Should().Equal("gpt-4o", "gpt-4o-mini");
			settings.Profiles[0].HasStoredKey.Should().BeTrue();
			settings.MaxContextTokens.Should().Be(5000);
			settings.StreamResponses.Should().BeFalse();
			settings.WordWrap.Should().BeFalse();
			settings.SendIL.Should().BeTrue();
			settings.SendCallGraph.Should().BeTrue();
			settings.CredentialMigrationPending.Should().BeTrue();

			XElement saved = settings.SaveToXml();
			saved.Element("Profiles")!.Elements("Profile").Select(p => p.Attribute("Id")!.Value)
				.Should().Equal(new[] { "p1", "p2" }, "profile order is preserved");
		}

		[Test]
		public void LoadFromXml_RepairsMalformedProfilesWithoutDroppingValidOnes()
		{
			var xml = XElement.Parse("""
				<AISettings>
				  <SchemaVersion>2</SchemaVersion>
				  <ActiveProfileId>missing</ActiveProfileId>
				  <Profiles>
				    <Profile Id="dup">
				      <ProviderType>openai</ProviderType>
				      <BaseUrl>https://api.openai.com</BaseUrl>
				      <Models><Model>gpt-4o</Model></Models>
				    </Profile>
				    <Profile Id="dup" Name="  ">
				      <ProviderType>made-up</ProviderType>
				      <BaseUrl>not a uri</BaseUrl>
				      <Models />
				    </Profile>
				  </Profiles>
				</AISettings>
				""");
			var settings = new AISettings();

			settings.LoadFromXml(xml);

			settings.Profiles.Should().HaveCount(2, "valid profiles are not discarded");
			settings.Profiles.Select(p => p.Id).Distinct().Should().HaveCount(2, "duplicate ids are repaired");
			settings.Profiles.All(p => !string.IsNullOrWhiteSpace(p.Id)).Should().BeTrue();
			AIProfile broken = settings.Profiles[1];
			broken.Name.Should().NotBeNullOrWhiteSpace("blank names are repaired");
			settings.Profiles.Select(p => p.Name).Distinct(System.StringComparer.OrdinalIgnoreCase).Should().HaveCount(2);
			broken.ProviderType.Should().Be("openai", "unsupported provider types reset to a supported default");
			broken.BaseUrl.Should().Be("https://api.openai.com", "invalid endpoints reset to the provider default");
			broken.Models.Should().NotBeEmpty("an empty model list gets the provider default");
			settings.ActiveProfileId.Should().BeOneOf(settings.Profiles.Select(p => p.Id).ToArray());
		}

		[Test]
		public void LoadFromXml_Schema2WithoutActiveOrProfiles_FallsBackToMinimumValidProfile()
		{
			var xml = XElement.Parse("""
				<AISettings>
				  <SchemaVersion>2</SchemaVersion>
				</AISettings>
				""");
			var settings = new AISettings();

			settings.LoadFromXml(xml);

			settings.Profiles.Should().HaveCount(1);
			settings.Profiles[0].ProviderType.Should().Be("openai");
			settings.ActiveProfileId.Should().Be(settings.Profiles[0].Id);
		}

		[Test]
		public void CompleteCredentialMigration_PersistsCompleteMarker()
		{
			var settings = new AISettings();
			settings.LoadFromXml(new XElement("AISettings",
				new XElement("Provider", "openai"),
				new XElement("ApiKeyPlaceholder", "ref")));

			settings.CredentialMigrationPending.Should().BeTrue();
			settings.MarkCredentialMigrationComplete();
			settings.CredentialMigrationPending.Should().BeFalse();

			var reloaded = new AISettings();
			reloaded.LoadFromXml(settings.SaveToXml());
			reloaded.CredentialMigrationPending.Should().BeFalse("a completed migration is never retried");
		}

		[Test]
		public void LegacyFacade_MapsToActiveProfile()
		{
			var settings = new AISettings();

			settings.Provider.Should().Be(settings.Profiles[0].ProviderType);
			settings.Provider = "ollama";
			settings.Profiles[0].ProviderType.Should().Be("ollama");
			settings.BaseUrl.Should().Be("http://localhost:11434");
			settings.Model.Should().Be("llama3:70b");
			settings.BaseUrl = "http://other-host:11434";
			settings.Profiles[0].BaseUrl.Should().Be("http://other-host:11434");
		}
	}
}
