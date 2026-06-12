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
using System.Threading.Tasks;

using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataFilterTests
{
	sealed class SampleEntry
	{
		public int RID { get; set; }
		public string Name { get; set; } = "";
		public string Culture { get; set; } = "";
	}

	[Test]
	public void MatchesFilters_Returns_True_When_All_Column_Filters_Are_Empty()
	{
		// An empty filter set is the identity case — every row passes.
		var entry = new SampleEntry { RID = 1, Name = "System.Runtime" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name"),
			new ColumnFilter("RID"),
		}).Should().BeTrue();
	}

	[Test]
	public void MatchesFilters_Returns_True_When_Each_Filter_Matches_Its_Column_Case_Insensitively()
	{
		// Per-column filters AND together: every non-empty filter must hit its own column,
		// not just any column. Case-insensitive substring match.
		var entry = new SampleEntry { RID = 1, Name = "System.Runtime", Culture = "neutral" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "system" },
			new ColumnFilter("Culture") { Text = "NEUTRAL" },
		}).Should().BeTrue();
	}

	[Test]
	public void MatchesFilters_Returns_False_When_A_Filter_Does_Not_Match_Its_Own_Column()
	{
		// Anchored to a single column: "system" in Culture must not match a row whose
		// Culture is "neutral", even though Name contains "System".
		var entry = new SampleEntry { Name = "System.Runtime", Culture = "neutral" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Culture") { Text = "system" },
		}).Should().BeFalse();
	}

	[Test]
	public void MatchesFilters_Returns_False_When_Any_AndEd_Filter_Misses()
	{
		// All filters must match — one missing column is enough to drop the row.
		var entry = new SampleEntry { Name = "System.Runtime", Culture = "neutral" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "system" },
			new ColumnFilter("Culture") { Text = "invariant" },
		}).Should().BeFalse();
	}

	[Test]
	public void Setting_ColumnFilter_Text_Raises_ColumnFilterChanged_On_The_Page()
	{
		// The view subscribes to the page's ColumnFilterChanged event to drive its
		// DataGridCollectionView.Refresh — without this forwarding the filter row would be
		// invisible to the grid and typing would have no effect on visible rows.
		var page = new MetadataTablePageModel();
		var filter = new ColumnFilter("Name");
		page.ColumnFilters.Add(filter);
		var fired = 0;
		page.ColumnFilterChanged += () => fired++;

		filter.Text = "System";

		fired.Should().Be(1);
	}

	[Test]
	public void Removing_A_ColumnFilter_Stops_Forwarding_Its_Text_Changes()
	{
		// When Populate clears the collection on a schema swap, the previously-attached
		// filters must stop driving refreshes — otherwise stale headers from a former tab
		// would keep firing into the new view's CollectionView.
		var page = new MetadataTablePageModel();
		var filter = new ColumnFilter("Name");
		page.ColumnFilters.Add(filter);
		page.ColumnFilters.Remove(filter);
		var fired = 0;
		page.ColumnFilterChanged += () => fired++;

		filter.Text = "System";

		fired.Should().Be(0);
	}

	[Test]
	public void Filter_Wrapped_In_Slashes_Is_Parsed_As_Case_Insensitive_Regex()
	{
		// `/pattern/` opts a filter into regex mode — same as Vim / less. Substring
		// match (default) wouldn't accept `^System` or `Runtime$`; regex mode does.
		var entry = new SampleEntry { Name = "System.Runtime" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "/^System/" },
		}).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "/runtime$/" },
		}).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "/^Runtime/" },
		}).Should().BeFalse();
	}

	[Test]
	public void Unparseable_Regex_Falls_Back_To_Substring_Match()
	{
		// A malformed pattern (e.g. unclosed group) shouldn't take down the filter — the
		// predicate must keep matching as if the user had typed plain text.
		var entry = new SampleEntry { Name = "System(Runtime" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "/(unclosed/" },
		}).Should().BeFalse(
			"the literal string '/(unclosed/' is not a substring of 'System(Runtime', so the fallback substring path rejects the row");
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Name") { Text = "(Runtime" },
		}).Should().BeTrue(
			"plain-text '(Runtime' (no slashes) matches as a literal substring");
	}

	[Flags]
	enum SampleFlags { None = 0, Public = 1, Static = 2, Sealed = 4, Private = 8 }

	sealed class FlagsEntry
	{
		public SampleFlags Attributes { get; set; }
	}

	static ICSharpCode.ILSpy.Metadata.Filters.FilterState NewFlagsState() =>
		new(ICSharpCode.ILSpy.Metadata.Filters.FlagsSchemaInferer.For(typeof(SampleFlags)));

	[Test]
	public void Empty_FlagsState_Matches_Every_Row()
	{
		// The unfiltered default — no required, no excluded, every mutex group "any".
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public | SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = NewFlagsState() } }).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.None },
			new[] { new ColumnFilter("Attributes") { FlagsState = NewFlagsState() } }).Should().BeTrue();
	}

	[Test]
	public void Required_Independent_Flag_Narrows_To_Rows_With_That_Bit_Set()
	{
		var state = NewFlagsState();
		state.SetFlagState(nameof(SampleFlags.Public), ICSharpCode.ILSpy.Metadata.Filters.TriState.Required);
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public | SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeFalse();
	}

	[Test]
	public void Multiple_Required_Flags_AND_By_Default()
	{
		// Without an explicit mode the schema starts in MatchMode.All — every required
		// flag must be set. Public | Static row passes; Public alone doesn't.
		var state = NewFlagsState();
		state.SetFlagState(nameof(SampleFlags.Public), ICSharpCode.ILSpy.Metadata.Filters.TriState.Required);
		state.SetFlagState(nameof(SampleFlags.Static), ICSharpCode.ILSpy.Metadata.Filters.TriState.Required);
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public | SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeFalse();
	}

	[Test]
	public void Excluded_Flag_Hides_Rows_With_That_Bit_Set()
	{
		var state = NewFlagsState();
		state.SetFlagState(nameof(SampleFlags.Static), ICSharpCode.ILSpy.Metadata.Filters.TriState.Excluded);
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public | SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = state } }).Should().BeFalse();
	}

	[Test]
	public void FlagsState_And_Text_Filter_AND_Together_On_The_Same_Column()
	{
		var state = NewFlagsState();
		state.SetFlagState(nameof(SampleFlags.Public), ICSharpCode.ILSpy.Metadata.Filters.TriState.Required);
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public | SampleFlags.Static },
			new[] { new ColumnFilter("Attributes") { FlagsState = state, Text = "Static" } }).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(
			new FlagsEntry { Attributes = SampleFlags.Public },
			new[] { new ColumnFilter("Attributes") { FlagsState = state, Text = "Static" } }).Should().BeFalse();
	}

	sealed class NumericEntry
	{
		[ColumnInfo("X8")]
		public int Token { get; set; }
	}

	[Test]
	public void Filter_With_Comparison_Operator_Matches_Numerically_On_Integer_Column()
	{
		var entry = new NumericEntry { Token = 0x06000010 };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Token") { Text = ">0x06000000" },
		}).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Token") { Text = "<0x06000000" },
		}).Should().BeFalse();
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Token") { Text = "=0x06000010" },
		}).Should().BeTrue();
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Token") { Text = ">=0x06000010" },
		}).Should().BeTrue();
	}

	[Test]
	public void Filter_With_Plain_Hex_Or_Decimal_Falls_Back_To_Substring_On_Integer_Columns()
	{
		// Without an operator the user is just searching — substring on the formatted
		// value (which is what the column displays) is the most predictable behavior.
		// "06000010" matches the formatted "06000010" of Token=0x06000010.
		var entry = new NumericEntry { Token = 0x06000010 };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("Token") { Text = "06000010" },
		}).Should().BeTrue();
	}

	[Test]
	public void MatchesFilters_Returns_False_When_The_Filtered_Column_Does_Not_Exist_On_The_Row()
	{
		// A filter targeting a non-existent property never matches; surfacing this as "no
		// match" prevents stale filter rows from silently passing rows of a different shape.
		var entry = new SampleEntry { Name = "X" };
		MetadataTablePageModel.MatchesFilters(entry, new[] {
			new ColumnFilter("DoesNotExist") { Text = "anything" },
		}).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Typing_Into_The_Rendered_Header_TextBox_Filters_The_DataGrid_View()
	{
		// End-to-end: navigate to TypeDef, locate the rendered TextBox in the Name column's
		// header, write into it, and confirm the DataGrid's effective row count drops. We
		// assert on the live DataGridCollectionView's Count (what the user actually sees on
		// screen) — not on the predicate evaluated over Items, which can pass even when the
		// CollectionView's Filter is silently null.
		var (window, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");

		// The header layout swaps in the per-column TextBox only on hover or when the
		// filter is active (the label and the input share the same slot so the header
		// height stays one row tall). Force the model TextBox visible up-front so the
		// rendered visual tree realises it; the hover/auto-show wiring is exercised in
		// its own UI-state tests.
		var nameColumn = tab.Columns.Single(c => (string?)c.Tag == "Name");
		var modelHeader = (StackPanel)nameColumn.Header!;
		var modelHeaderBox = modelHeader.GetLogicalDescendants().OfType<TextBox>().Single();
		var modelHeaderLabel = modelHeader.GetLogicalDescendants().OfType<TextBlock>()
			.Single(tb => tb.Text == "Name");
		modelHeaderBox.IsVisible = true;
		modelHeaderLabel.IsVisible = false;

		var metadataPage = await window.WaitForComponent<MetadataTablePage>();
		var grid = await metadataPage.WaitForComponent<DataGrid>();
		await grid.WaitForComponent<DataGridColumnHeader>();

		var headerBox = grid.GetVisualDescendants().OfType<TextBox>()
			.FirstOrDefault(tb => tb.FindAncestorOfType<DataGridColumnHeader>() is { } owner
				&& owner.Content is StackPanel sp
				&& sp.GetLogicalDescendants().OfType<TextBlock>()
					.Any(t => t.Text == "Name"));
		headerBox.Should().NotBeNull(
			"the column-builder bakes a TextBox into the Name column's header — it must reach the rendered visual tree");
		ReferenceEquals(headerBox, modelHeaderBox).Should().BeTrue(
			"DataGrid must render the StackPanel Header directly without re-templating; otherwise the property-changed handler is on a different TextBox than the user types into");

		var view = grid.ItemsSource as DataGridCollectionView;
		view.Should().NotBeNull("the metadata grid wires its ItemsSource to a DataGridCollectionView so the per-column filter can re-evaluate");
		var totalRows = tab.Items.Count;
		totalRows.Should().BeGreaterThan(0, "TypeDef must have rows for filtering to be observable");
		view!.Count.Should().Be(totalRows, "every row should be visible before any filter is set");

		var nameFilter = tab.ColumnFilters.Single(f => f.ColumnName == "Name");

		// Set the rendered TextBox's Text — the same code path a user keystroke takes
		// (TextProperty AvaloniaObject change → builder's PropertyChanged handler →
		// ColumnFilter.Text → page.ColumnFilterChanged → view.Refresh()).
		headerBox!.Text = "System";
		TestCapture.Step("filtered-by-system");

		nameFilter.Text.Should().Be("System",
			"setting the rendered TextBox.Text must propagate to ColumnFilter.Text");
		var expectedVisible = tab.Items.Count(e => MetadataTablePageModel.MatchesFilters(e, tab.ColumnFilters));
		expectedVisible.Should().BeLessThan(totalRows,
			"the predicate must hide at least one TypeDef row when Name contains 'System'");
		view.Count.Should().Be(expectedVisible,
			"the live DataGridCollectionView's count must reflect the per-column filter — typing into the header must shrink the visible-row set, not just update the model");

		// Clearing through the rendered TextBox must restore every row.
		headerBox.Text = "";
		TestCapture.Step("filter-cleared");
		nameFilter.Text.Should().BeEmpty();
		view.Count.Should().Be(totalRows,
			"clearing the filter must restore every row in the visible grid view");
	}
}
