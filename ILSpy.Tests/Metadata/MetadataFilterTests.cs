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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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
	public async Task ColumnFilter_Reduces_Visible_Rows_To_Those_Whose_Column_Contains_The_Substring()
	{
		// Integration check: navigate to the TypeDef table (guaranteed populated for any
		// loaded module) and confirm that setting a Name-column filter shrinks the visible
		// row set to entries whose Name contains the filter, and clearing it restores all.

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		var totalCount = tab.Items.Count;
		totalCount.Should().BeGreaterThan(0);

		var nameFilter = tab.ColumnFilters.Single(f => f.ColumnName == "Name");
		nameFilter.Text = "System";

		var visible = tab.Items.Where(e => MetadataTablePageModel.MatchesFilters(e, tab.ColumnFilters)).ToList();
		visible.Should().NotBeEmpty("at least one TypeDef row should mention 'System'");
		visible.Count.Should().BeLessThan(totalCount, "the filter must hide at least one row");

		nameFilter.Text = "";
		var afterClear = tab.Items.Where(e => MetadataTablePageModel.MatchesFilters(e, tab.ColumnFilters)).Count();
		afterClear.Should().Be(totalCount);
	}
}
