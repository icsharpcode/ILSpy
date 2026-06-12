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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchResultSortOrderTests
{
	[AvaloniaTest]
	public async Task When_SortResults_Is_False_Results_Are_Ordered_By_Name_Not_Fitness()
	{
		// The "Sort results by fitness" checkbox in Display Settings must reach the search
		// pipeline. Default is true (rank by Fitness desc); flipping to false must rank by
		// Name asc (StringComparer.Ordinal). Mirrors WPF's SearchPane.xaml.cs:288-290
		// which captures the comparer at start-of-search based on DisplaySettings.SortResults.

		await TestHarness.BootAsync();

		var settings = AppComposition.Current.GetExport<SettingsService>();
		var originalSortResults = settings.DisplaySettings.SortResults;

		try
		{
			settings.DisplaySettings.SortResults = false;

			var search = AppComposition.Current.GetExport<SearchPaneModel>();
			search.SearchTerm = string.Empty;
			search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
			search.SearchTerm = "Enumerable";

			await Waiters.WaitForAsync(
				() => !search.IsSearching && search.Results.Count >= 2,
				timeout: TimeSpan.FromSeconds(30));

			TestCapture.Step("search-results-name-sorted");
			// Drop assembly/namespace results — they share a unit Fitness with all peers in
			// their bucket, so any tie-break is ambiguous between fitness- and name-sort.
			// Member results are where Fitness varies (1/Name.Length), so their order is
			// what distinguishes the two comparers.
			var names = search.Results.OfType<MemberSearchResult>()
				.Select(r => r.Name)
				.ToList();

			names.Should().HaveCountGreaterThan(1,
				"need at least two member hits to compare orderings");
			names.Should().BeInAscendingOrder(StringComparer.Ordinal,
				"with SortResults=false the pane must rank by Name ordinal-asc, not by Fitness");

			search.SearchTerm = string.Empty;
		}
		finally
		{
			settings.DisplaySettings.SortResults = originalSortResults;
		}
	}
}
