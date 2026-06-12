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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataCellTooltipTests
{
	sealed class EntryWithTooltips
	{
		public string Name { get; set; } = "Foo";
		public string NameTooltip => $"heap-offset for \"{Name}\"";

		public int Flags { get; set; } = 0x42;
		// Non-string tooltip values flow through ToString() — the helper just normalises
		// whatever the entry exposes into a string, leaving styled-tooltip rendering for
		// a later phase.
		public object FlagsTooltip => $"Flags=0x{Flags:X4}";
	}

	sealed class EntryWithoutTooltips
	{
		public string Name { get; set; } = "Bar";
	}

	[Test]
	public void Resolve_Returns_String_Tooltip_When_Property_Is_Present()
	{
		// The convention is `{columnName}Tooltip` — looked up by reflection on the bound row.
		var entry = new EntryWithTooltips { Name = "System.Object" };
		MetadataCellTooltip.Resolve(entry, "Name").Should().Be("heap-offset for \"System.Object\"");
	}

	[Test]
	public void Resolve_Returns_Null_When_The_Tooltip_Property_Is_Missing()
	{
		// Many entry types expose tooltips for only a subset of columns; missing means "no
		// tooltip", not "throw".
		var entry = new EntryWithoutTooltips();
		MetadataCellTooltip.Resolve(entry, "Name").Should().BeNull();
	}

	[Test]
	public void Resolve_Stringifies_Non_String_Tooltip_Values()
	{
		// Some WPF entries return rich FlagsTooltip objects; stringification is the safe
		// fallback so the helper still produces something usable.
		var entry = new EntryWithTooltips { Flags = 0x1234 };
		MetadataCellTooltip.Resolve(entry, "Flags").Should().Be("Flags=0x1234");
	}

	[Test]
	public void Resolve_Returns_Null_When_The_Item_Is_Null()
	{
		MetadataCellTooltip.Resolve(null!, "Name").Should().BeNull();
	}
}
