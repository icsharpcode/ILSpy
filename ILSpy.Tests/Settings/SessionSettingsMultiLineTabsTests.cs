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

using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Settings;

/// <summary>
/// The document-tab layout mode persists across sessions: it defaults to single-line (false) and
/// round-trips through the SessionSettings XML.
/// </summary>
[TestFixture]
public class SessionSettingsMultiLineTabsTests
{
	static SessionSettings Load(XElement section)
	{
		var settings = new SessionSettings();
		settings.LoadFromXml(section);
		return settings;
	}

	[Test]
	public void Defaults_To_Single_Line_When_Absent()
	{
		Load(new XElement("SessionSettings")).MultiLineDocumentTabs.Should().BeFalse();
	}

	[Test]
	public void Round_Trips_Through_Xml()
	{
		var settings = Load(new XElement("SessionSettings"));
		settings.MultiLineDocumentTabs = true;

		var reloaded = Load(settings.SaveToXml());

		reloaded.MultiLineDocumentTabs.Should().BeTrue("the persisted mode must survive save + load");
	}
}
