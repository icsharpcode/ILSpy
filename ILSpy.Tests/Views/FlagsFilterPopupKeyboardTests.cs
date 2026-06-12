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

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata.Filters;
using ICSharpCode.ILSpy.Views.Filters;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views.Filters;

/// <summary>
/// Pins arrow-key navigation between chips inside <see cref="FlagsFilterPopup"/>.
/// The popup is composed of one <see cref="MutexChipGroup"/> per axis plus an
/// <see cref="IndependentFlagGroup"/>; without explicit directional-navigation wiring
/// the user can only Tab between every focusable element (chips + Clear button), which
/// is awkward for a power-user filter. Arrow keys should hop chip-by-chip.
/// </summary>
[TestFixture]
public class FlagsFilterPopupKeyboardTests
{
	[System.Flags]
	enum SampleAttrs : uint
	{
		Public = 1,
		Private = 2,
		VisibilityMask = 0x3,
		Static = 0x10,
		Abstract = 0x20,
	}

	[AvaloniaTest]
	public void Right_Arrow_From_The_First_Chip_Moves_Focus_To_The_Next_Chip_In_The_Same_Group()
	{
		// Build the popup, push it onto a window so focus management works, focus the
		// first chip, send Right. Default Avalonia DirectionalNavigation should carry
		// focus through siblings inside the WrapPanel.
		var schema = FlagsSchemaInferer.For(typeof(SampleAttrs));
		var state = new FilterState(schema);
		var popup = new FlagsFilterPopup(state);
		var window = new Window { Content = popup };
		window.Show();

		var chips = popup.GetVisualDescendants().OfType<ToggleButton>().ToList();
		chips.Should().HaveCountGreaterThan(1, "test relies on at least two chips being present");
		var first = chips[0];
		first.Focus();
		first.IsFocused.Should().BeTrue("setup precondition — first chip must accept focus before we exercise arrow nav");

		window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, keySymbol: null);

		var newlyFocused = chips.SingleOrDefault(c => c.IsFocused);
		newlyFocused.Should().NotBeNull("Right arrow must transfer focus to a sibling chip");
		ReferenceEquals(newlyFocused, first).Should().BeFalse(
			"Right arrow must move focus AWAY from the first chip, not stay on it");
	}

	[AvaloniaTest]
	public void Down_Arrow_From_A_Chip_In_The_First_Group_Lands_On_A_Chip_In_The_Next_Group()
	{
		// Vertical traversal: pressing Down from a chip in the first mutex group must move
		// into the second group (or the independent-flags group if there's only one mutex
		// group). Without arrow-traversal wiring the focus would stay inside the same
		// WrapPanel because WrapPanel is a horizontal-flow container.
		var schema = FlagsSchemaInferer.For(typeof(SampleAttrs));
		var state = new FilterState(schema);
		var popup = new FlagsFilterPopup(state);
		var window = new Window { Content = popup };
		window.Show();

		var groups = popup.GetVisualDescendants()
			.OfType<UserControl>()
			.Where(c => c is MutexChipGroup || c is IndependentFlagGroup)
			.ToList();
		groups.Should().HaveCountGreaterThan(1,
			"test relies on at least two filter groups (one mutex + one independent, or two mutex)");

		// First group is a MutexChipGroup full of ToggleButton chips; second group is an
		// IndependentFlagGroup full of Button pills. Allow either control type as a valid
		// landing slot so we're testing the cross-group jump, not the chip type.
		var firstGroupChips = groups[0].GetVisualDescendants().OfType<ToggleButton>().ToList();
		var secondGroupTargets = groups[1].GetVisualDescendants()
			.Where(d => d is ToggleButton || d is Button)
			.ToList();
		var anchor = firstGroupChips[0];
		anchor.Focus();
		anchor.IsFocused.Should().BeTrue();

		window.KeyPress(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, keySymbol: null);

		var focusedNow = popup.GetVisualDescendants()
			.OfType<Control>()
			.FirstOrDefault(c => c.IsFocused && (c is ToggleButton || c is Button));
		focusedNow.Should().NotBeNull("Down arrow must keep focus on some chip in the popup");
		secondGroupTargets.Contains(focusedNow!).Should().BeTrue(
			"Down arrow from the first group must land on a chip/pill in the next group");
	}
}
