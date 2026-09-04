// Copyright (c) 2026 Siegfried Pammer
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
using System.Linq;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Settings;

/// <summary>
/// A settings file ILSpy cannot parse is the only copy of everything the user put in it - assembly
/// lists above all, which people build up over years and edit by hand. Replacing it with defaults
/// on the next save destroys that, without a word and without a copy (issue #2919).
/// </summary>
[TestFixture]
public class MalformedSettingsFileTests
{
	const string MalformedSettings = """
		<ILSpy version="9.0.0.0">
		  <AssemblyList name="List 1">
		    <Assembly>C:\important\one.dll</Assembly>
		    <Assembly>C:\important\two.dll</Assembly>
		</ILSpy>
		""";

	Func<string>? savedProvider;
	string tempDir = "";
	string configFile = "";

	[SetUp]
	public void SetUp()
	{
		savedProvider = ILSpySettings.SettingsFilePathProvider;
		tempDir = Path.Combine(Path.GetTempPath(), "ILSpyMalformedSettings_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		configFile = Path.Combine(tempDir, "ILSpy.xml");
		ILSpySettings.SettingsFilePathProvider = () => configFile;
	}

	[TearDown]
	public void TearDown()
	{
		ILSpySettings.SettingsFilePathProvider = savedProvider;
		try
		{
			Directory.Delete(tempDir, recursive: true);
		}
		catch
		{
			// best effort
		}
	}

	static void SaveSomething()
	{
		ILSpySettings.Load().SaveSettings(new XElement("TestSection", new XAttribute("value", "1")));
	}

	[Test]
	public void WhatCouldNotBeParsedIsKept()
	{
		File.WriteAllText(configFile, MalformedSettings);

		SaveSomething();

		var backups = Directory.GetFiles(tempDir).Where(f => f != configFile).ToList();
		Assert.That(backups, Has.Count.EqualTo(1), "the unparseable file is kept alongside the new one");
		Assert.That(File.ReadAllText(backups[0]), Is.EqualTo(MalformedSettings), "kept as it was");
	}

	[Test]
	public void SettingsAreStillWritten()
	{
		File.WriteAllText(configFile, MalformedSettings);

		SaveSomething();

		var written = XDocument.Load(configFile);
		Assert.That(written.Root!.Element("TestSection"), Is.Not.Null, "the save itself goes through");
	}

	[Test]
	public void AnEarlierBackupIsNotOverwritten()
	{
		// Two bad startups in a row must not cost the first file, which is the one with the data.
		File.WriteAllText(configFile, MalformedSettings);
		SaveSomething();
		File.WriteAllText(configFile, "<ILSpy><Broken/>");
		SaveSomething();

		var backups = Directory.GetFiles(tempDir).Where(f => f != configFile).ToList();
		Assert.That(backups, Has.Count.EqualTo(2));
		Assert.That(backups.Select(File.ReadAllText), Has.One.EqualTo(MalformedSettings));
	}

	[Test]
	public void AGoodFileIsLeftAlone()
	{
		File.WriteAllText(configFile, "<ILSpy version=\"9.0.0.0\"><Existing keep=\"yes\" /></ILSpy>");

		SaveSomething();

		Assert.That(Directory.GetFiles(tempDir), Has.Length.EqualTo(1), "nothing is copied aside");
		var written = XDocument.Load(configFile);
		Assert.That(written.Root!.Element("Existing"), Is.Not.Null, "and the settings are still there");
	}
}
