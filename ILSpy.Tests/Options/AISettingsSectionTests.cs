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

using System.ComponentModel;
using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Options
{
	/// <summary>
	/// Characterization of the desktop <see cref="AISettingsSection"/> adapter: the XML section
	/// contract the pre-extraction <c>ICSharpCode.ILSpyX.Settings.AISettings</c> section exposed
	/// to the settings host must be preserved byte-for-byte (element name, schema-2 shape,
	/// ordering, secret omission).
	/// </summary>
	[TestFixture]
	public class AISettingsSectionTests
	{
		[Test]
		public void Section_ImplementsHostContractUnderLegacyElementName()
		{
			var section = new AISettingsSection();

			section.Should().BeAssignableTo<ISettingsSection>();
			section.SectionName.Should().Be("AISettings");
			section.Model.Should().NotBeNull();
		}

		[Test]
		public void LoadAndSave_RoundTripsThroughTheAdapter()
		{
			var section = new AISettingsSection();
			section.Model.Provider = "anthropic";
			section.Model.BaseUrl = "https://example.test";
			section.Model.Model = "custom-model";
			section.Model.MaxContextTokens = 16000;
			section.Model.SendIL = true;
			section.Model.PrivacyConsentAccepted = true;

			var reloaded = new AISettingsSection();
			reloaded.LoadFromXml(section.SaveToXml());

			reloaded.Model.Provider.Should().Be("anthropic");
			reloaded.Model.BaseUrl.Should().Be("https://example.test");
			reloaded.Model.Model.Should().Be("custom-model");
			reloaded.Model.MaxContextTokens.Should().Be(16000);
			reloaded.Model.SendIL.Should().BeTrue();
			reloaded.Model.PrivacyConsentAccepted.Should().BeTrue();
		}

		[Test]
		public void SaveToXml_WritesSchema2ShapeWithoutSecrets()
		{
			var section = new AISettingsSection();
			section.Model.ApiKey = "secret-api-key";

			var xml = section.SaveToXml().ToString(SaveOptions.DisableFormatting);

			xml.Should().NotContain("secret-api-key");
			xml.Should().Contain("<SchemaVersion>2</SchemaVersion>", "schema version must stay stable across the extraction");
			xml.Should().Contain("CredentialMigration");
		}

		[Test]
		public void SaveToXml_ReplacesExistingSectionElementLikeTheSettingsHostDoes()
		{
			var section = new AISettingsSection();
			var root = new XElement("Configuration",
				new XElement("AISettings", new XElement("Stale", true)));

			var existing = root.Element(section.SectionName);
			existing!.ReplaceWith(section.SaveToXml());

			root.Element("AISettings")!.Element("Stale").Should().BeNull();
			root.Element("AISettings")!.Element("SchemaVersion").Should().NotBeNull();
		}

		[Test]
		public void ModelChanges_ForwardAsSectionPropertyChanges()
		{
			var section = new AISettingsSection();
			var changed = new System.Collections.Generic.List<string?>();
			section.PropertyChanged += PropertyChanged;

			section.Model.WordWrap = false;
			section.Model.MaxContextTokens = 8000;

			changed.Should().Contain(nameof(AISettingsModel.WordWrap));
			changed.Should().Contain(nameof(AISettingsModel.MaxContextTokens));

			void PropertyChanged(object? sender, PropertyChangedEventArgs e)
			{
				sender.Should().BeSameAs(section, "the settings host identifies sections, not the wrapped model");
				changed.Add(e.PropertyName);
			}
		}
	}
}
