// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Linq;
using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.NuGetFeeds;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.NuGetFeeds;

/// <summary>
/// Pins the persistence contract of the "Open from NuGet feed" dialog's settings section:
/// the nuget.org feed is always available even after loading damaged settings XML, the
/// feed MRU stays bounded and free of case-duplicates, and the selected feed plus the
/// prerelease toggle survive a save/load round-trip.
/// </summary>
[TestFixture]
public class NuGetFeedSettingsTests
{
	const string CustomFeed = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json";

	[Test]
	public void Defaults_Offer_The_NuGetOrg_Feed_Selected_Without_Prerelease()
	{
		var settings = new NuGetFeedSettings();

		settings.Feeds.Should().Equal(new[] { NuGetFeedSettings.DefaultFeed },
			"a fresh install must offer exactly the official nuget.org V3 feed");
		settings.SelectedFeed.Should().Be(NuGetFeedSettings.DefaultFeed);
		settings.IncludePrerelease.Should().BeFalse("stable-only is the conventional default");
	}

	[Test]
	public void SaveToXml_LoadFromXml_RoundTrips_Feeds_Selection_And_Prerelease()
	{
		var settings = new NuGetFeedSettings();
		settings.RememberFeed(CustomFeed);
		settings.SelectedFeed = CustomFeed;
		settings.IncludePrerelease = true;

		var restored = new NuGetFeedSettings();
		restored.LoadFromXml(settings.SaveToXml());

		restored.Feeds.Should().Equal(settings.Feeds);
		restored.SelectedFeed.Should().Be(CustomFeed);
		restored.IncludePrerelease.Should().BeTrue();
	}

	[Test]
	public void LoadFromXml_On_An_Empty_Section_Falls_Back_To_Defaults()
	{
		var settings = new NuGetFeedSettings();
		settings.RememberFeed(CustomFeed);
		settings.SelectedFeed = CustomFeed;
		settings.IncludePrerelease = true;

		settings.LoadFromXml(new XElement("NuGetFeedSettings"));

		settings.Feeds.Should().Equal(new[] { NuGetFeedSettings.DefaultFeed });
		settings.SelectedFeed.Should().Be(NuGetFeedSettings.DefaultFeed);
		settings.IncludePrerelease.Should().BeFalse();
	}

	[Test]
	public void LoadFromXml_Ignores_Garbage_Content_And_Keeps_The_Default_Feed_Present()
	{
		var settings = new NuGetFeedSettings();
		var section = new XElement("NuGetFeedSettings",
			new XElement("Feeds",
				new XElement("Feed", "   "),
				new XElement("Feed", CustomFeed)),
			new XElement("SelectedFeed", "https://feed.example/that/was/never/remembered"),
			new XElement("IncludePrerelease", "not-a-bool"));

		settings.LoadFromXml(section);

		settings.Feeds.Should().Contain(NuGetFeedSettings.DefaultFeed,
			"the nuget.org feed must survive any settings file content");
		settings.Feeds.Should().Contain(CustomFeed);
		settings.Feeds.Should().NotContain(f => string.IsNullOrWhiteSpace(f));
		settings.Feeds.Should().Contain(settings.SelectedFeed,
			"the selection must always point at an offered feed");
		settings.IncludePrerelease.Should().BeFalse("malformed booleans fall back to the default");
	}

	[Test]
	public void RememberFeed_Dedupes_Case_Insensitively_And_Ignores_Blank_Urls()
	{
		var settings = new NuGetFeedSettings();

		settings.RememberFeed(CustomFeed);
		settings.RememberFeed(CustomFeed.ToUpperInvariant());
		settings.RememberFeed("  " + CustomFeed + "  ");
		settings.RememberFeed("");
		settings.RememberFeed("   ");

		settings.Feeds.Should().HaveCount(2, "the default feed plus one custom feed, no duplicates");
		settings.Feeds.Count(f => string.Equals(f, CustomFeed, System.StringComparison.OrdinalIgnoreCase))
			.Should().Be(1);
	}

	[Test]
	public void RememberFeed_Caps_The_List_But_Never_Evicts_The_Default_Feed()
	{
		var settings = new NuGetFeedSettings();
		for (int i = 0; i < 25; i++)
			settings.RememberFeed($"https://feed{i}.example/v3/index.json");

		settings.Feeds.Count.Should().BeLessThanOrEqualTo(NuGetFeedSettings.MaxFeeds);
		settings.Feeds.Should().Contain(NuGetFeedSettings.DefaultFeed);
		settings.Feeds.Should().Contain("https://feed24.example/v3/index.json",
			"the most recently used feed must survive the cap");
	}
}
