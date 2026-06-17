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

using System;
using System.IO;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AppEnv;

// ConfigurationFiles.GetPath underpins where the dock layout and the bookmark list are
// stored: as JSON sidecars in the same directory as ILSpy.xml. These tests pin that
// "next to the settings file" contract and the headless fallback.
[TestFixture]
public class ConfigurationFilesTests
{
	Func<string>? savedProvider;

	[SetUp]
	public void SaveProvider() => savedProvider = ILSpySettings.SettingsFilePathProvider;

	[TearDown]
	public void RestoreProvider() => ILSpySettings.SettingsFilePathProvider = savedProvider;

	[Test]
	public void GetPath_returns_sidecar_in_settings_directory()
	{
		var settingsDir = Path.Combine(Path.GetTempPath(), "ILSpyConfigTest");
		ILSpySettings.SettingsFilePathProvider = () => Path.Combine(settingsDir, "ILSpy.xml");

		ConfigurationFiles.GetPath("ILSpy.Bookmarks.json")
			.Should().Be(Path.Combine(settingsDir, "ILSpy.Bookmarks.json"));
	}

	[Test]
	public void GetPath_falls_back_to_bare_name_when_provider_is_absent()
	{
		ILSpySettings.SettingsFilePathProvider = null;

		ConfigurationFiles.GetPath("ILSpy.Bookmarks.json").Should().Be("ILSpy.Bookmarks.json");
	}
}
