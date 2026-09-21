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
using System.Collections.Specialized;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Assemblies arrive in bursts while the assembly the user clicked is decompiled and its
/// references are loaded. The search must not restart per arrival: each restart cancels a walk
/// that is already competing with the decompilation for the thread pool.
/// </summary>
[TestFixture]
public class SearchListSettleTests
{
	const int BurstSize = 10;

	static async Task<(SearchPaneModel search, AssemblyList list, Func<int> searchesStarted)> ArrangeAsync()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var search = AppComposition.Current.GetExport<SearchPaneModel>();

		int started = 0;
		search.PropertyChanged += (_, e) => {
			if (e.PropertyName == nameof(SearchPaneModel.IsSearching) && search.IsSearching)
				started++;
		};
		search.SearchTerm = "Object";
		await PumpAsync(TimeSpan.FromMilliseconds(400));   // let the typing debounce elapse
		started = 0;                                        // count only what the burst causes
		return (search, vm.AssemblyTreeModel.AssemblyList!, () => started);
	}

	static async Task PumpAsync(TimeSpan duration)
	{
		var until = DateTime.UtcNow + duration;
		do
		{
			await Task.Delay(20);
			Dispatcher.UIThread.RunJobs();
		} while (DateTime.UtcNow < until);
	}

	static void SendAdd(AssemblyList list, bool autoLoaded)
	{
		var added = new LoadedAssembly(list, typeof(int).Assembly.Location) { IsAutoLoaded = autoLoaded };
		MessageBus.Send(typeof(SearchListSettleTests), new CurrentAssemblyListChangedEventArgs(
			new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { added }, 0)));
	}

	[AvaloniaTest]
	public async Task ABurstOfAddsStartsOneSearch()
	{
		var (_, list, searchesStarted) = await ArrangeAsync();

		// Not pumped between sends: the settle timer only fires when dispatcher jobs run, so
		// the burst stays a burst no matter how slow the machine is.
		for (int i = 0; i < BurstSize; i++)
		{
			SendAdd(list, autoLoaded: false);
		}
		await PumpAsync(TimeSpan.FromMilliseconds(900));

		TestContext.Out.WriteLine($"burst of {BurstSize}: {searchesStarted()} searches in total");
		searchesStarted().Should().Be(1,
			$"a burst of {BurstSize} assembly additions is one search, not {BurstSize}");
	}

	[AvaloniaTest]
	public async Task AutoLoadedAssembliesAreSearchedToo()
	{
		var (_, list, searchesStarted) = await ArrangeAsync();

		SendAdd(list, autoLoaded: true);
		await PumpAsync(TimeSpan.FromMilliseconds(900));

		searchesStarted().Should().Be(1,
			"an auto-loaded dependency can contain matches; skipping it left the results silently "
			+ "incomplete, which is what the settle replaces");
	}
}
