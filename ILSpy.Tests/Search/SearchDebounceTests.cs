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
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Typing a term must not start a search per keystroke. Each restart cancels the walk in progress
/// and starts another on the thread pool, so every character typed used to cost a search that only
/// the last one could finish.
/// </summary>
[TestFixture]
public class SearchDebounceTests
{
	const string Term = "SomeTypeNameToLookFor";

	static async Task<(SearchPaneModel vm, Func<int> startedSearches)> ShowPaneAsync()
	{
		var (window, _) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()
			.ShowToolPane(SearchPaneModel.PaneContentId);
		var pane = await window.WaitForComponent<SearchPane>();
		var vm = (SearchPaneModel)pane.DataContext!;

		// A search that actually starts turns IsSearching on; counting those counts the walks
		// handed to the thread pool.
		int started = 0;
		vm.PropertyChanged += (_, e) => {
			if (e.PropertyName == nameof(SearchPaneModel.IsSearching) && vm.IsSearching)
				started++;
		};
		return (vm, () => started);
	}

	/// <summary>Drains the dispatcher so queued timer callbacks run.</summary>
	static async Task PumpAsync(TimeSpan duration)
	{
		var until = DateTime.UtcNow + duration;
		do
		{
			await Task.Delay(20);
			Dispatcher.UIThread.RunJobs();
		} while (DateTime.UtcNow < until);
	}

	[AvaloniaTest]
	public async Task TypingATermStartsOneSearch()
	{
		var (vm, startedSearches) = await ShowPaneAsync();

		// One property change per character, as the text box raises them. The dispatcher is
		// deliberately not pumped in between: a DispatcherTimer only fires when jobs run, so
		// this cannot depend on how fast the machine gets through the loop.
		for (int i = 1; i <= Term.Length; i++)
		{
			vm.SearchTerm = Term.Substring(0, i);
		}

		await PumpAsync(TimeSpan.FromMilliseconds(600));

		startedSearches().Should().Be(1,
			$"typing {Term.Length} characters is one search, not {Term.Length}");
	}

	[AvaloniaTest]
	public async Task ChoosingASearchModeStartsAtOnce()
	{
		var (vm, startedSearches) = await ShowPaneAsync();
		vm.SearchTerm = "T";
		await PumpAsync(TimeSpan.FromMilliseconds(400));
		int afterTyping = startedSearches();

		vm.SelectedSearchMode = vm.SearchModes.First(m => m.Mode != vm.SelectedSearchMode.Mode);
		Dispatcher.UIThread.RunJobs();

		startedSearches().Should().Be(afterTyping + 1,
			"picking a mode is a single deliberate act, so it is not debounced");
	}
}
