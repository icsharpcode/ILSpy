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

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Pins the SessionSettings ↔ XML shape that backs <c>FilterStatePersistence</c>'s
/// cross-session cache. Without these tests a refactor of <c>LoadFromXml</c> /
/// <c>SaveToXml</c> could silently break the persistence round-trip — the schema
/// drift test alone wouldn't catch a wiring change at the SessionSettings level.
/// </summary>
[TestFixture]
public class SessionSettingsFilterStatesTests
{
	[Test]
	public void Round_Trip_Preserves_Multiple_Entries_Across_Multiple_Pages()
	{
		// Two different metadata-table pages each with one filtered column. Verifies the
		// outer <FilterStates>/<Page>/<Column> nesting AND that the inner FilterState
		// payload (an opaque XElement to SessionSettings) survives unmolested.
		var original = new SessionSettings();
		original.FilterStates[("ICSharpCode.ILSpy.Metadata.CorTables.TypeDefEntry", "Attributes")] =
			new XElement("FilterState",
				new XElement("Flag",
					new XAttribute("name", "Sealed"),
					new XAttribute("state", "Required")));
		original.FilterStates[("ICSharpCode.ILSpy.Metadata.CorTables.MethodDefEntry", "Attributes")] =
			new XElement("FilterState",
				new XElement("Mutex",
					new XAttribute("group", "MemberAccess"),
					new XElement("Value", "6")));

		var xml = original.SaveToXml();
		var restored = new SessionSettings();
		restored.LoadFromXml(xml);

		restored.FilterStates.Should().HaveCount(2);
		restored.FilterStates[("ICSharpCode.ILSpy.Metadata.CorTables.TypeDefEntry", "Attributes")]
			.Element("Flag")!.Attribute("name")!.Value.Should().Be("Sealed");
		restored.FilterStates[("ICSharpCode.ILSpy.Metadata.CorTables.MethodDefEntry", "Attributes")]
			.Element("Mutex")!.Element("Value")!.Value.Should().Be("6");
	}

	[Test]
	public void Empty_FilterStates_Does_Not_Emit_The_Container_Element()
	{
		// We don't want to bloat ILSpy.xml with an empty <FilterStates /> wrapper every
		// time a user closes the app without touching any filter dropdowns.
		var settings = new SessionSettings();

		var xml = settings.SaveToXml();

		xml.Element("FilterStates").Should().BeNull(
			"omit the container entirely when no filter has been persisted");
	}

	[Test]
	public void LoadFromXml_Tolerates_Missing_FilterStates_Section()
	{
		// Settings files written by versions before this feature have no <FilterStates>;
		// LoadFromXml must accept them silently and leave FilterStates empty.
		var settings = new SessionSettings();
		var legacyXml = new XElement("SessionSettings",
			new XElement("HideEmptyMetadataTables", "true"));

		var act = () => settings.LoadFromXml(legacyXml);

		act.Should().NotThrow();
		settings.FilterStates.Should().BeEmpty();
	}
}
