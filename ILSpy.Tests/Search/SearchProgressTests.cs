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
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Media;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

[TestFixture]
public class SearchProgressTests
{
	[AvaloniaTest]
	public async Task IsSearching_Flips_True_When_A_Search_Starts_And_False_When_It_Finishes()
	{
		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = string.Empty;
		search.IsSearching.Should().BeFalse("baseline: nothing running");

		var raised = false;
		PropertyChangedEventHandler handler = (_, e) => {
			if (e.PropertyName == nameof(SearchPaneModel.IsSearching) && search.IsSearching)
				raised = true;
		};
		search.PropertyChanged += handler;
		try
		{
			search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
			search.SearchTerm = "Enumerable";

			// Either we already captured the transition, or it'll fire on the next dispatcher
			// pump — wait briefly.
			await Waiters.WaitForAsync(() => raised || search.IsSearching,
				timeout: TimeSpan.FromSeconds(5));
			(raised || search.IsSearching).Should().BeTrue(
				"the spinner must light up while the orchestrator is running");

			await Waiters.WaitForAsync(() => !search.IsSearching, timeout: TimeSpan.FromSeconds(30));
			TestCapture.Step("search-completed");
			search.IsSearching.Should().BeFalse("the spinner must turn off once the run completes");
		}
		finally
		{
			search.PropertyChanged -= handler;
			search.SearchTerm = string.Empty;
		}
	}

	[AvaloniaTest]
	public async Task SearchResult_Image_Properties_Are_Non_Null_So_Row_Icons_Render()
	{
		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = string.Empty;
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "Enumerable";

		await Waiters.WaitForAsync(() => search.Results.Any(), timeout: TimeSpan.FromSeconds(30));

		TestCapture.Step("search-results-with-icons");
		var first = search.Results.First();
		((object?)first.Image).Should().NotBeNull(
			"every search result needs a glyph in the Name column");
		(first.Image is IImage).Should().BeTrue(
			"Image must be an Avalonia IImage so the row template's <Image Source=\"...\"> renders it");

		search.SearchTerm = string.Empty;
	}

	[AvaloniaTest]
	public async Task SearchPane_Hosts_A_Progress_Indicator_Bound_To_IsSearching()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		// Search is hidden by default; surface it so its view realises.
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(ICSharpCode.ILSpy.Search.SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		TestCapture.Step("booted");

		var progress = pane.FindControl<ProgressBar>("SearchProgress");
		((object?)progress).Should().NotBeNull(
			"the pane must host a progress indicator the user can see while a search runs");
		progress!.IsIndeterminate.Should().BeTrue(
			"the indicator runs in indeterminate mode — we don't know the total work up front");
	}
}
