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
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// The rich flags tooltip breaks an enum value into per-bit groups: multiple-choice groups render a
/// checkbox per flag with the set bits checked, single-choice groups render the selected member of a
/// mutually exclusive sub-range. (Regression: the Avalonia port had only a plain-text fallback.)
/// </summary>
[TestFixture]
public class FlagsTooltipTests
{
	[AvaloniaTest]
	public void MultipleChoiceGroup_Checks_The_Set_Bits()
	{
		var group = FlagGroup.CreateMultipleChoiceGroup(typeof(MethodAttributes), "Flags:",
			(int)MethodAttributes.Static, (int)MethodAttributes.Static, includeAll: false);

		var view = (StackPanel)group.Build();
		var statik = view.Children.OfType<CheckBox>()
			.Single(c => ((string?)c.Content)!.StartsWith("Static"));
		statik.IsChecked.Should().BeTrue("the Static bit is set in the selected value");
	}

	[AvaloniaTest]
	public void SingleChoiceGroup_Shows_The_Selected_Member()
	{
		var group = FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Accessibility: ",
			(int)MethodAttributes.MemberAccessMask, (int)MethodAttributes.Public, includeAny: false);

		group.SelectedFlag.Name.Should().StartWith("Public");
	}

	[AvaloniaTest]
	public void ForTypeAttributes_Includes_The_Type_Forwarder_Bit()
	{
		// 0x00200000 is the ECMA-335 Forwarder bit (II.23.1.15), set on type-forwarder
		// ExportedType rows. System.Reflection.TypeAttributes has no member for it, so the
		// enum-driven flag enumeration alone would leave it invisible in the tooltip.
		var tooltip = FlagsTooltip.ForTypeAttributes((TypeAttributes)0x00200000);

		var flags = tooltip.Groups.OfType<MultipleChoiceGroup>().Single().Flags;
		var forwarder = flags.Single(f => f.Name.StartsWith("IsTypeForwarder"));
		forwarder.IsSelected.Should().BeTrue("the Forwarder bit is set in the value");
	}

	[AvaloniaTest]
	public void ForTypeAttributes_Leaves_The_Type_Forwarder_Bit_Unchecked_When_Clear()
	{
		var tooltip = FlagsTooltip.ForTypeAttributes(TypeAttributes.Public | TypeAttributes.Sealed);

		var flags = tooltip.Groups.OfType<MultipleChoiceGroup>().Single().Flags;
		var forwarder = flags.Single(f => f.Name.StartsWith("IsTypeForwarder"));
		forwarder.IsSelected.Should().BeFalse("the Forwarder bit is not set in the value");
	}

	[AvaloniaTest]
	public void MetadataCellTooltip_Renders_A_FlagsTooltip_As_A_Control()
	{
		var entry = new EntryWithFlags();
		var resolved = MetadataCellTooltip.Resolve(entry, "Attributes");
		resolved.Should().BeAssignableTo<Control>("a FlagsTooltip value renders as the rich control");
	}

	sealed class EntryWithFlags
	{
		public MethodAttributes Attributes => MethodAttributes.Public | MethodAttributes.Static;

		public object AttributesTooltip => new FlagsTooltip {
			FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Accessibility: ",
				(int)MethodAttributes.MemberAccessMask, (int)(Attributes & MethodAttributes.MemberAccessMask), includeAny: false),
			FlagGroup.CreateMultipleChoiceGroup(typeof(MethodAttributes), "Flags:",
				~(int)MethodAttributes.MemberAccessMask, (int)Attributes, includeAll: false),
		};
	}
}
