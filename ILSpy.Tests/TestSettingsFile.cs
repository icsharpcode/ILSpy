// Copyright (c) 2026 Christoph Wille
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

using System.IO;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.Updates;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Points <see cref="ILSpySettings"/> at a fresh settings file for the current test session.
/// The file is seeded with automatic update checks switched off: MainWindowViewModel fires
/// the startup update check from its constructor, and with the production default (enabled,
/// never checked) every test that builds the main window would otherwise issue a real HTTP
/// request to the update feed.
/// </summary>
static class TestSettingsFile
{
	public static void Create(string sessionDir)
	{
		var configPath = Path.Combine(sessionDir, "ILSpy.xml");
		var updateSettings = new UpdateSettings { AutomaticUpdateCheckEnabled = false };
		new XDocument(new XElement("ILSpy", updateSettings.SaveToXml())).Save(configPath);
		ILSpySettings.SettingsFilePathProvider = () => configPath;
	}
}
