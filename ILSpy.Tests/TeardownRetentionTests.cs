// Copyright (c) 2026 Christoph Wille
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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

// Most tests in this suite show a MainWindow, and the per-test teardown closes it and rebuilds
// the composition container. Anything that still reaches a closed window - a static event, a
// shared XAML resource with a subscriber, an animation on the render clock, the app-level menu -
// keeps that test's whole app graph (view-models, tree, loaded assemblies; about 13 MB) alive
// for the rest of the run, and over the suite that is enough to push a 16 GB CI runner into
// paging. Rather than asserting the absence of each known anchor, this test performs the
// teardown itself and checks that the window is actually collectable afterwards.
[TestFixture]
public class TeardownRetentionTests
{
	[AvaloniaTest]
	public async Task A_Main_Window_Closed_By_The_Teardown_Is_Collectable()
	{
		var window = ShowMainWindow();
		// Let the assembly loads the window started run to completion first: each one posts its
		// completion to the dispatcher, and one posted after the teardown would hold the tree (and
		// with it the window) until the next test pumps it - a false positive, not retention.
		await Waiters.WaitForAsync(static () => AllAssembliesLoaded());

		ResetAppStateAttribute.TearDownTestState();
		// What the next test's BeforeTest does: the fresh container drops the [Shared] MainWindow.
		AppComposition.CreateContainer();

		// The closed window's final composition batch (its target's disposal) references it until
		// the compositor has committed and rendered it, and commits are throttled behind the
		// previous batch's completion, which comes back through the thread pool - so keep pumping
		// the dispatcher (and the headless render loop, which only ticks on request) while polling.
		await Waiters.WaitForAsync(() => IsCollected(window), TimeSpan.FromSeconds(10),
			"the closed MainWindow to become unreachable once its container is gone");
	}

	static bool IsCollected(WeakReference window)
	{
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
		return !window.IsAlive;
	}

	static bool AllAssembliesLoaded()
	{
		var assemblies = AppComposition.Current.GetExport<AssemblyTreeModel>().AssemblyList?.GetAssemblies();
		return assemblies is { Length: > 0 } && assemblies.All(a => a.IsLoaded);
	}

	// The window must not be referenced from this test's own frame while the GC runs.
	[MethodImpl(MethodImplOptions.NoInlining)]
	static WeakReference ShowMainWindow()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		Dispatcher.UIThread.RunJobs();
		return new WeakReference(window);
	}
}
