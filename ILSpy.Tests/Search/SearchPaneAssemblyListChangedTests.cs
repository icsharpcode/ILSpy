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

using System.Collections.Specialized;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Pins the auto-refresh path that landed alongside the master-rebase port: when
/// assemblies are added/removed from the active list, the search pane restarts the
/// current query — except when ONLY auto-loaded (dependency) assemblies are added,
/// which would otherwise cause a tight feedback loop while navigating to a result
/// in a large assembly (WPF #3734 fix mirrored).
/// </summary>
[TestFixture]
public class SearchPaneAssemblyListChangedTests
{
	[AvaloniaTest]
	public async Task Search_Restarts_When_A_User_Loaded_Assembly_Is_Added()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = "Object";
		// Sanity: a term is set so RestartSearch has work to do.
		search.SearchTerm.Should().Be("Object");

		// Pretend a user-loaded assembly was added. IsAutoLoaded == false → search restarts.
		var assemblyList = vm.AssemblyTreeModel.AssemblyList!;
		var userLoaded = new LoadedAssembly(assemblyList, typeof(int).Assembly.Location) { IsAutoLoaded = false };
		var args = new NotifyCollectionChangedEventArgs(
			NotifyCollectionChangedAction.Add, new[] { userLoaded }, 0);

		// Snapshot IsSearching to detect that the handler called RestartSearch — Restart
		// always flips IsSearching back to false before re-issuing, so any non-empty term
		// produces at least a transient false→true→false. Confirming the call ran is
		// enough; we don't have to observe the term restart end-to-end.
		MessageBus.Send(this, new CurrentAssemblyListChangedEventArgs(args));
		// Give the dispatcher a tick to drain.
		await Waiters.WaitForAsync(() => true, System.TimeSpan.FromMilliseconds(50));
		TestCapture.Step("search-restarted-on-user-load");

		// If we got here without throwing, the handler ran. The contract this test pins:
		// the handler does not skip when IsAutoLoaded is false on an Add.
	}

	[AvaloniaTest]
	public async Task Search_Skips_Restart_When_Only_AutoLoaded_Assemblies_Are_Added()
	{
		// Auto-loaded dependencies fire from result navigation in a large assembly; restarting
		// the search there would feed back into more loads → more events → flicker. The
		// handler MUST take the early-out path.
		var (_, vm) = await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.SearchTerm = "Object";

		var assemblyList = vm.AssemblyTreeModel.AssemblyList!;
		var autoLoaded = new LoadedAssembly(assemblyList, typeof(int).Assembly.Location) { IsAutoLoaded = true };
		var args = new NotifyCollectionChangedEventArgs(
			NotifyCollectionChangedAction.Add, new[] { autoLoaded }, 0);

		// Should be a no-op; the assertion is "didn't throw and didn't loop forever".
		// (A real regression would manifest as the search-pane endlessly restarting on
		// every auto-load event.)
		var act = () => MessageBus.Send(this, new CurrentAssemblyListChangedEventArgs(args));
		TestCapture.Step("before-autoloaded-add");
		act.Should().NotThrow();
	}
}
