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
using System.Linq;
using System.Reflection;

using AwesomeAssertions;

using ILSpy.Metadata.Filters;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class FlagsSchemaInfererTests
{
	[Flags]
	enum AllIndependent { None = 0, A = 1, B = 2, C = 4, D = 8 }

	[Flags]
	enum SingleMutex
	{
		ColourMask = 0x03,
		Black = 0x00,
		Red = 0x01,
		Green = 0x02,
		Blue = 0x03,
		Sparkly = 0x10,
	}

	[Test]
	public void Pure_Independent_Enum_Has_No_Mutex_Groups()
	{
		var schema = FlagsSchemaInferer.For(typeof(AllIndependent));

		schema.MutexGroups.Should().BeEmpty();
		schema.IndependentFlags.Select(f => (f.Name, f.Bit)).Should().Equal(
			("A", 1u), ("B", 2u), ("C", 4u), ("D", 8u));
	}

	[Test]
	public void Mask_Backed_Bits_Land_In_Mutex_Group_And_Other_Bits_Stay_Independent()
	{
		var schema = FlagsSchemaInferer.For(typeof(SingleMutex));

		schema.MutexGroups.Should().HaveCount(1);
		var colour = schema.MutexGroups.Single();
		colour.Name.Should().Be("Colour");
		colour.Mask.Should().Be(0x03u);
		colour.Values.Select(v => (v.Label, v.Value)).Should().Equal(
			("(none)", 0u),
			("Red (0001)", 0x01u),
			("Green (0002)", 0x02u),
			("Blue (0003)", 0x03u));

		schema.IndependentFlags.Select(f => (f.Name, f.Bit)).Should().Equal(
			("Sparkly", 0x10u));
	}

	[Test]
	public void TypeAttributes_Surfaces_All_Four_Conventional_Mutex_Groups()
	{
		// Snapshot the canonical .NET enum the metadata grid filters most often. The
		// inferer must walk the *Mask fields and partition every non-zero value field
		// into the first containing mask; bits that don't fit any mask are independent.
		var schema = FlagsSchemaInferer.For(typeof(TypeAttributes));

		var groupNames = schema.MutexGroups.Select(g => g.Name).ToList();
		groupNames.Should().Contain(new[] { "Visibility", "Layout", "ClassSemantics", "StringFormat" });

		var visibility = schema.MutexGroups.Single(g => g.Name == "Visibility");
		visibility.Mask.Should().Be(0x07u);
		visibility.Values.Select(v => v.Value).Should().BeEquivalentTo(
			new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 });

		var layout = schema.MutexGroups.Single(g => g.Name == "Layout");
		layout.Mask.Should().Be(0x18u);
		// AutoLayout (0), SequentialLayout (0x08), ExplicitLayout (0x10), and -- added in .NET 11 --
		// ExtendedLayout (0x18, both layout bits set). The inferer surfaces whatever the live enum
		// declares inside the mask, so the snapshot tracks the host runtime's TypeAttributes.
		layout.Values.Select(v => v.Value).Should().BeEquivalentTo(
			new uint[] { 0, 0x08, 0x10, 0x18 });

		// Independent flags: Abstract, Sealed, SpecialName, Import, Serializable, etc.
		var independentNames = schema.IndependentFlags.Select(f => f.Name).ToList();
		independentNames.Should().Contain(new[] {
			nameof(TypeAttributes.Abstract),
			nameof(TypeAttributes.Sealed),
			nameof(TypeAttributes.SpecialName),
			nameof(TypeAttributes.Import),
		});

		// And no mask fields leak into either bucket as values.
		schema.IndependentFlags.Select(f => f.Name).Should().NotContain("VisibilityMask");
		schema.IndependentFlags.Select(f => f.Name).Should().NotContain("LayoutMask");
	}

	[Test]
	public void MethodAttributes_Has_Member_Access_Mutex_Plus_Independent_Modifiers()
	{
		var schema = FlagsSchemaInferer.For(typeof(MethodAttributes));

		var memberAccess = schema.MutexGroups.Single(g => g.Name == "MemberAccess");
		memberAccess.Mask.Should().Be(0x0007u);
		memberAccess.Values.Select(v => v.Value).Should().BeEquivalentTo(
			new uint[] { 0, 1, 2, 3, 4, 5, 6 });

		schema.IndependentFlags.Select(f => f.Name).Should().Contain(new[] {
			nameof(MethodAttributes.Static),
			nameof(MethodAttributes.Final),
			nameof(MethodAttributes.Virtual),
			nameof(MethodAttributes.Abstract),
		});
	}

	[Test]
	public void Schema_Is_Cached_So_Repeat_Lookups_Hit_The_Same_Instance()
	{
		// Reflection cost is paid once per enum for the lifetime of the process. A
		// second call must return the exact same FlagsSchema, not a fresh one.
		FlagsSchemaInferer.For(typeof(TypeAttributes))
			.Should().BeSameAs(FlagsSchemaInferer.For(typeof(TypeAttributes)));
	}

	[Test]
	public void Aliased_Enum_Members_Sharing_A_Value_Render_Once()
	{
		// MethodAttributes.PrivateScope and Privatescope (or any duplicate-value field
		// pair) must not produce two checkboxes for the same bit. The inferer dedupes
		// by value; the first declaration wins.
		var schema = FlagsSchemaInferer.For(typeof(MethodAttributes));
		var memberAccess = schema.MutexGroups.Single(g => g.Name == "MemberAccess");
		memberAccess.Values.Select(v => v.Value).Should().OnlyHaveUniqueItems();
	}
}
