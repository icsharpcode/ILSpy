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

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata.Filters;
using ICSharpCode.ILSpy.Views.Filters;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class FlagsFilterPopupTests
{
	static (FilterState State, FlagsFilterPopup Popup) BuildPopup()
	{
		var schema = FlagsSchemaInferer.For(typeof(TypeAttributes));
		var state = new FilterState(schema);
		var popup = new FlagsFilterPopup(state);
		// Avalonia controls only realise their visual tree once hosted in a TopLevel —
		// park the popup inside a hidden window so the descendant lookups in these tests
		// see materialised children.
		var host = new Window { Content = popup, Width = 400, Height = 600 };
		host.Show();
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		return (state, popup);
	}

	[AvaloniaTest]
	public void Toggling_A_Visibility_Chip_Off_Sets_The_Mutex_Selection_To_The_Remaining_Values()
	{
		// Initial state: <Any> on, every visibility chip on. Click "Public (0001)" off
		// → selection becomes the seven remaining visibility values, <Any> deactivates.
		var (state, popup) = BuildPopup();
		var publicChip = FindVisibilityChip(popup, "Public (0001)");
		publicChip.IsChecked = false;

		state.MutexSelections["Visibility"].Should().NotBeNull();
		state.MutexSelections["Visibility"]!.Should().NotContain(0x01u);
		state.MutexSelections["Visibility"]!.Should().Contain(0x02u); // NestedPublic still on
	}

	[AvaloniaTest]
	public void Toggling_All_Visibility_Chips_Off_Collapses_Back_To_Any()
	{
		// Spec: "Selecting none re-activates 'any.'" The UI never lets the state stay
		// at an empty selection — it auto-resets to null.
		var (state, popup) = BuildPopup();
		var chips = popup.GetVisualDescendants().OfType<ToggleButton>()
			.Where(c => c.Content?.ToString() != "<Any>")
			.ToList();
		// Take the first group's worth of chips (Visibility comes first in the schema).
		var visibilityChipCount = state.Schema.MutexGroups[0].Values.Count;
		foreach (var chip in chips.Take(visibilityChipCount))
			chip.IsChecked = false;
		state.MutexSelections["Visibility"].Should().BeNull(
			"the chips collapsed every value off — that flips back to <Any>");
	}

	[AvaloniaTest]
	public void Tri_State_Pill_Cycles_DontCare_To_Required_To_Excluded_To_DontCare()
	{
		var (state, popup) = BuildPopup();
		var sealedPill = FindFlagPill(popup, nameof(TypeAttributes.Sealed));

		state.FlagStates[nameof(TypeAttributes.Sealed)].Should().Be(TriState.DontCare);

		Click(sealedPill);
		state.FlagStates[nameof(TypeAttributes.Sealed)].Should().Be(TriState.Required);

		Click(sealedPill);
		state.FlagStates[nameof(TypeAttributes.Sealed)].Should().Be(TriState.Excluded);

		Click(sealedPill);
		state.FlagStates[nameof(TypeAttributes.Sealed)].Should().Be(TriState.DontCare);
	}

	[AvaloniaTest]
	public void Clear_Resets_Every_Group_And_Flag_To_Default()
	{
		var (state, popup) = BuildPopup();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create(0x01u));
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.IndependentMode = MatchMode.Any;

		popup.Clear();

		state.MutexSelections["Visibility"].Should().BeNull();
		state.FlagStates[nameof(TypeAttributes.Sealed)].Should().Be(TriState.DontCare);
		state.IndependentMode.Should().Be(MatchMode.All);
	}

	[AvaloniaTest]
	public void Predicate_Summary_Updates_When_State_Changes()
	{
		// The summary line at the bottom of the popup is computed from the same model
		// CompiledFilter consumes — they stay in sync via FilterStatePresenter.
		var (state, popup) = BuildPopup();
		var summaryBlock = popup.GetVisualDescendants().OfType<TextBlock>()
			.First(b => b.Text != null && b.Text.Contains("(any)"));

		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);

		summaryBlock.Text.Should().Be("Sealed");
	}

	static ToggleButton FindVisibilityChip(FlagsFilterPopup popup, string label) =>
		popup.GetVisualDescendants().OfType<ToggleButton>()
			.First(c => c.Content?.ToString() == label);

	static Button FindFlagPill(FlagsFilterPopup popup, string flagName)
	{
		// Each independent-flag row is a DockPanel with a Button (pill) followed by a
		// TextBlock carrying the flag name. Find the matching textblock and walk to its
		// sibling button.
		var label = popup.GetVisualDescendants().OfType<TextBlock>()
			.First(b => b.Text == flagName);
		var dock = label.GetVisualAncestors().OfType<DockPanel>().First();
		return dock.GetVisualDescendants().OfType<Button>().First();
	}

	static void Click(Button b) =>
		b.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
}
