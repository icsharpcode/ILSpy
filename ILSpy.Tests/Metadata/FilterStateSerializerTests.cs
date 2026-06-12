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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata.Filters;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// Round-trips a <see cref="FilterState"/> through XML — the storage shape behind
/// <see cref="FilterStatePersistence"/>. Tests cover the empty case (so persistence
/// doesn't write noise to ILSpy.xml when the user clears a filter), the three mutator
/// surfaces (mutex selections, tri-state flag states, independent-mode), and schema-
/// drift resilience (XML references groups/flags the current schema no longer carries).
/// </summary>
[TestFixture]
public class FilterStateSerializerTests
{
	[Test]
	public void ToXml_Empty_FilterState_Produces_Element_With_No_Constraint_Children()
	{
		// Newly-constructed state with every chip = Any and every flag = DontCare must
		// round-trip to an empty <FilterState /> so SessionSettings doesn't bloat with
		// no-op entries.
		var state = new FilterState(BuildSchema());

		var xml = FilterStatePersistence.ToXml(state);

		xml.Name.LocalName.Should().Be("FilterState");
		xml.Elements("Mutex").Should().BeEmpty();
		xml.Elements("Flag").Should().BeEmpty();
		// IndependentMode = All (default) is implicit; only Any is written explicitly.
		xml.Element("IndependentMode")?.Value.Should().NotBe("All");
	}

	[Test]
	public void RoundTrip_Preserves_Mutex_Selection_Across_Multiple_Values()
	{
		// Mutex chip groups support multi-selection — the user can pick "Public" + "Family"
		// from VisibilityMask, and that pair must survive a session restart.
		var schema = BuildSchema();
		var original = new FilterState(schema);
		original.SetMutexSelection("Visibility",
			ImmutableHashSet.Create<uint>(2u, 4u));

		var xml = FilterStatePersistence.ToXml(original);
		var restored = new FilterState(schema);
		FilterStatePersistence.ApplyXml(restored, xml);

		restored.MutexSelections["Visibility"].Should().BeEquivalentTo(new uint[] { 2u, 4u });
	}

	[Test]
	public void RoundTrip_Preserves_TriState_Flag_States()
	{
		// Required / Excluded / DontCare each have distinct semantics in the filter
		// predicate, so all three need to round-trip exactly.
		var schema = BuildSchema();
		var original = new FilterState(schema);
		original.SetFlagState("Sealed", TriState.Required);
		original.SetFlagState("Abstract", TriState.Excluded);
		// "Static" deliberately left as DontCare — must NOT be re-emitted as a Flag entry.

		var xml = FilterStatePersistence.ToXml(original);
		var restored = new FilterState(schema);
		FilterStatePersistence.ApplyXml(restored, xml);

		restored.FlagStates["Sealed"].Should().Be(TriState.Required);
		restored.FlagStates["Abstract"].Should().Be(TriState.Excluded);
		restored.FlagStates["Static"].Should().Be(TriState.DontCare);
	}

	[Test]
	public void RoundTrip_Preserves_IndependentMode_When_It_Differs_From_Default()
	{
		var schema = BuildSchema();
		var original = new FilterState(schema) { IndependentMode = MatchMode.Any };

		var xml = FilterStatePersistence.ToXml(original);
		var restored = new FilterState(schema);
		FilterStatePersistence.ApplyXml(restored, xml);

		restored.IndependentMode.Should().Be(MatchMode.Any);
	}

	[Test]
	public void ApplyXml_Ignores_Mutex_Entries_For_Groups_Missing_From_The_Current_Schema()
	{
		// Schema drift: an older .ILSpy.xml might mention a group the new schema dropped.
		// Apply should silently skip it instead of throwing — the user otherwise loses
		// every saved filter on a schema upgrade.
		var xml = new System.Xml.Linq.XElement("FilterState",
			new System.Xml.Linq.XElement("Mutex",
				new System.Xml.Linq.XAttribute("group", "GoneFromSchema"),
				new System.Xml.Linq.XElement("Value", "1")));
		var state = new FilterState(BuildSchema());

		var act = () => FilterStatePersistence.ApplyXml(state, xml);

		act.Should().NotThrow();
		state.IsEmpty.Should().BeTrue("the unknown group entry must be silently ignored");
	}

	[Test]
	public void ApplyXml_Ignores_Flag_Entries_For_Flags_Missing_From_The_Current_Schema()
	{
		var xml = new System.Xml.Linq.XElement("FilterState",
			new System.Xml.Linq.XElement("Flag",
				new System.Xml.Linq.XAttribute("name", "FlagThatNoLongerExists"),
				new System.Xml.Linq.XAttribute("state", "Required")));
		var state = new FilterState(BuildSchema());

		var act = () => FilterStatePersistence.ApplyXml(state, xml);

		act.Should().NotThrow();
		state.IsEmpty.Should().BeTrue();
	}

	// --- helpers ---------------------------------------------------------------------------

	[System.Flags]
	enum SampleAttrs : uint
	{
		Public = 1,
		Family = 2,
		Internal = 4,
		Private = 8,
		VisibilityMask = 0xF,
		Static = 0x10,
		Abstract = 0x20,
		Sealed = 0x40,
	}

	static FlagsSchema BuildSchema()
	{
		// Build the schema by inferring it off our test enum, matching what production code
		// does for real metadata enums. Saves us from authoring the schema by hand and
		// keeps the test contract aligned with the live FlagsSchemaInferer surface.
		return FlagsSchemaInferer.For(typeof(SampleAttrs));
	}
}
