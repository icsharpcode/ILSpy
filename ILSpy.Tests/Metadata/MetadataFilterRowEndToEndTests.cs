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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataFilterRowEndToEndTests
{
	sealed class Row
	{
		public int RID { get; set; }
		public string Name { get; set; } = "";
	}

	sealed class FlagsRow
	{
		public int RID { get; set; }
		public TypeAttributes Attributes { get; set; }
	}

	/// <summary>
	/// The funnel icon hosting the attached filter flyout for the Attributes column.
	/// </summary>
	static Border FindAttributesFunnel(Visual headerPanel) =>
		headerPanel.GetVisualDescendants().OfType<Border>()
			.First(b => ToolTip.GetTip(b) as string == "Filter Attributes");

	/// <summary>
	/// Forces the flyout's internal popup into the owner window's overlay layer — the
	/// configuration the app actually runs with (Program.cs sets
	/// X11PlatformOptions.OverlayPopups = true), where all popup input flows through the
	/// owner window's pipeline. The style reaches the popup through the logical tree.
	/// </summary>
	static void ForceOverlayPopups(Window window) =>
		window.Styles.Add(new Style(x => x.OfType<Popup>()) {
			Setters = { new Setter(Popup.ShouldUseOverlayLayerProperty, true) },
		});

	[AvaloniaTest]
	public void Clicking_Inside_The_Popup_Never_Sorts_The_Column_Of_A_Real_DataGrid()
	{
		// Full assembly of the real parts: an actual DataGrid with CanUserSortColumns
		// (as MetadataTablePage.axaml configures it), the builder's columns, the overlay
		// popup host the app runs with, and real input. Opening the popup via the funnel
		// and clicking a chip inside it must not raise DataGrid.Sorting.
		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<FlagsRow>(page);

		var grid = new DataGrid {
			ItemsSource = new System.Collections.Generic.List<FlagsRow> {
				new() { RID = 1, Attributes = TypeAttributes.Public },
				new() { RID = 2, Attributes = TypeAttributes.Sealed },
			},
			CanUserSortColumns = true,
		};
		foreach (var column in page.Columns)
			grid.Columns.Add(column);
		var window = new Window { Content = grid, Width = 1000, Height = 600 };
		ForceOverlayPopups(window);
		window.Show();
		window.UpdateLayout();

		int sortingRaised = 0;
		grid.Sorting += (_, _) => sortingRaised++;
		// An unhandled press reaching a column header is user-visible even without a
		// sort: DataGridColumnHeader sets IsPressed and flashes its :pressed background.
		// These listeners mirror the header's own subscriptions (bubble, skip handled),
		// so any hit here is a press the header would visibly react to.
		int headerUnhandledPresses = 0, headerUnhandledReleases = 0;
		foreach (var header in grid.GetVisualDescendants().OfType<DataGridColumnHeader>())
		{
			header.AddHandler(InputElement.PointerPressedEvent, (_, _) => headerUnhandledPresses++);
			header.AddHandler(InputElement.PointerReleasedEvent, (_, _) => headerUnhandledReleases++);
		}

		var attributesColumn = page.Columns.Single(c => (string?)c.Tag == "Attributes");
		var headerPanel = (StackPanel)attributesColumn.Header!;

		// Open the flyout with a real click on the funnel icon (the Border carrying the
		// "Filter Attributes" tooltip).
		var funnel = FindAttributesFunnel(headerPanel);
		var flyout = (Flyout)FlyoutBase.GetAttachedFlyout(funnel)!;
		var funnelCenter = funnel.TranslatePoint(new Point(funnel.Bounds.Width / 2, funnel.Bounds.Height / 2), window)!.Value;
		window.MouseDown(funnelCenter, MouseButton.Left);
		window.MouseUp(funnelCenter, MouseButton.Left);
		flyout.IsOpen.Should().BeTrue("setup precondition — the funnel click must open the flyout");
		window.UpdateLayout();
		Dispatcher.UIThread.RunJobs();
		sortingRaised.Should().Be(0, "the funnel click that opens the flyout must not sort the column");

		// Click every interactive control inside the flyout: mutex chips (ToggleButton),
		// tri-state pills and Clear (Button), plus the non-interactive hint TextBlock.
		// The mode ComboBox goes last because clicking it opens its own dropdown over
		// the other controls.
		// The flyout body scrolls (MaxHeight 400) and TypeAttributes has more chip groups
		// than fit, so restrict the sweep to controls whose center actually lies within
		// the flyout's on-screen rect — clicking a scrolled-out control's nominal position
		// is a click outside the flyout and legitimately light-dismisses.
		var popupContent = (Visual)flyout.Content!;
		Rect PopupRect()
		{
			var topLeft = popupContent.TranslatePoint(default, window)!.Value;
			return new Rect(topLeft, popupContent.Bounds.Size);
		}
		var targets = popupContent.GetVisualDescendants()
			.Where(v => v is Button || (v is TextBlock { Text: { } hintText } && hintText.StartsWith("Click a chip")))
			.Concat(popupContent.GetVisualDescendants().Where(v => v is ComboBox))
			.Cast<Control>()
			.ToList();
		targets.Should().NotBeEmpty("setup precondition — the popup must expose clickable controls");
		int clicked = 0;
		foreach (var target in targets)
		{
			// Recompute at click time: a previous click may have re-flowed the summary
			// line and shifted everything below it.
			window.UpdateLayout();
			var center = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), window);
			if (center is not { } point || !PopupRect().Contains(point))
				continue;
			clicked++;
			window.MouseDown(point, MouseButton.Left);
			window.MouseUp(point, MouseButton.Left);
			flyout.IsOpen.Should().BeTrue(
				$"clicking the {target.GetType().Name} inside the flyout must not close it");
			// ProcessSort is dispatched via Dispatcher.UIThread.Post — drain per click so
			// a leaked sort is attributed to the control that caused it.
			Dispatcher.UIThread.RunJobs();
			sortingRaised.Should().Be(0,
				$"clicking the {target.GetType().Name} inside the popup must not sort the column");
			headerUnhandledPresses.Should().Be(0,
				$"clicking the {target.GetType().Name} inside the popup must not deliver an unhandled press to a column header (the header would flash its pressed visual)");
			headerUnhandledReleases.Should().Be(0,
				$"clicking the {target.GetType().Name} inside the popup must not deliver an unhandled release to a column header");
		}
		clicked.Should().BeGreaterThan(2, "setup precondition — the sweep must actually click several visible controls");

		// ProcessSort is dispatched via Dispatcher.UIThread.Post — drain the queue so a
		// leaked sort would actually fire before the assertion.
		Dispatcher.UIThread.RunJobs();

		sortingRaised.Should().Be(0,
			"neither opening the filter popup nor clicking controls inside it may sort the column");
	}


	[AvaloniaTest]
	public void Typing_Into_The_Header_TextBox_Drives_The_Matching_ColumnFilter_Text()
	{
		// Direct end-to-end of the column-builder's filter wiring: locate the TextBox
		// inside the column header that Populate built, write text to it, and confirm the
		// matching ColumnFilter.Text follows. This is the bridge that was silently broken
		// on the running app — typing produced visible text but no propagation to the
		// filter model.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);

		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerPanel = (StackPanel)nameColumn.Header!;
		var headerBox = headerPanel.GetLogicalDescendants().OfType<TextBox>().Single();
		var nameFilter = page.ColumnFilters.Single(f => f.ColumnName == "Name");

		headerBox.Text = "System";

		nameFilter.Text.Should().Be("System",
			"setting the header TextBox's Text must propagate to the ColumnFilter via the TextProperty observable");
	}

	[AvaloniaTest]
	public void Setting_ColumnFilter_Text_Pushes_Back_To_The_Header_TextBox()
	{
		// Reverse direction: model → view. When the page programmatically clears or seeds a
		// filter (e.g. session restore), the visible TextBox must reflect it.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);

		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerBox = ((StackPanel)nameColumn.Header!).GetLogicalDescendants().OfType<TextBox>().Single();
		var nameFilter = page.ColumnFilters.Single(f => f.ColumnName == "Name");

		nameFilter.Text = "System";

		headerBox.Text.Should().Be("System",
			"the filter's PropertyChanged handler must push new values back into the TextBox");
	}

	[AvaloniaTest]
	public void Setting_The_Header_TextBox_Refreshes_The_DataGridCollectionView_Filter()
	{
		// The view's DataGridCollectionView re-evaluates its predicate on every
		// ColumnFilterChanged event raised by the page. This test stubs out that event
		// path: it confirms the header TextBox → filter.Text → page.ColumnFilterChanged
		// chain fires end-to-end so the live grid would refresh its visible row set.

		var page = new MetadataTablePageModel();
		MetadataColumnBuilder.Populate<Row>(page);
		var nameColumn = page.Columns.Single(c => (string?)c.Tag == "Name");
		var headerBox = ((StackPanel)nameColumn.Header!).GetLogicalDescendants().OfType<TextBox>().Single();

		var fired = 0;
		page.ColumnFilterChanged += () => fired++;

		headerBox.Text = "System";

		fired.Should().BeGreaterThan(0,
			"the event the view subscribes to for refreshing the grid must fire when the header TextBox changes");
	}
}
