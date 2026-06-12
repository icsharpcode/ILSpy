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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// The search results must render in a grid with clickable, sortable column headers
/// (Name / Location / Assembly), so the user can re-order hits after a search instead of being
/// stuck with the fixed fitness/name streaming order. This guards the wiring we own — the column
/// set, their <c>SortMemberPath</c>s, and that sorting is enabled; the header-click reordering
/// itself is Avalonia's DataGrid sort (used identically by OpenFromGacDialog/MetadataTablePage).
/// </summary>
[TestFixture]
public class SearchPaneSortableColumnsTests
{
	[AvaloniaTest]
	public async Task Results_Render_In_A_Grid_With_Sortable_Name_Location_Assembly_Columns()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();

		var grid = pane.FindControl<DataGrid>("SearchResults");
		((object?)grid).Should().NotBeNull("the results must be hosted in a DataGrid so the column headers are sortable");
		grid!.CanUserSortColumns.Should().BeTrue("the grid must allow the user to sort by clicking a header");

		// One sortable column per visible field, keyed to the SearchResult property it shows.
		var byPath = grid.Columns.ToDictionary(c => c.SortMemberPath);
		foreach (var (path, header) in new[] {
			(nameof(SearchResult.Name), Resources.Name),
			(nameof(SearchResult.Location), Resources.Location),
			(nameof(SearchResult.Assembly), Resources.Assembly),
		})
		{
			byPath.Should().ContainKey(path, $"there must be a column sorted by {path}");
			var column = byPath[path];
			column.CanUserSort.Should().BeTrue($"the {path} header must be clickable to sort");
			column.Header.Should().Be(header, $"the {path} column header text");
			column.Width.IsAuto.Should().BeFalse(
				$"the {path} column must not be Auto-width: Auto re-measures to the widest realised "
				+ "cell, so the column visibly jumps as rows virtualise in and out on scroll");
		}
	}
}
