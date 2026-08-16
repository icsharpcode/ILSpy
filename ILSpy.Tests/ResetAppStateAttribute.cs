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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Per-test app-state reset for the headless suite.
/// <para>
/// We run Avalonia with <c>AvaloniaTestIsolationLevel.PerAssembly</c> (see TestAppBuilder): the
/// Avalonia runtime + dispatcher are built ONCE per assembly, which removes the intermittent
/// "calling thread cannot access this object" race that PerTest re-initialization caused
/// (HeadlessUnitTestSession.EnsureIsolatedApplication rebuilding the Compositor inside NUnit's
/// isolated ExecutionContext). To keep each test fully isolated despite the shared runtime, this
/// action rebuilds the MEF container (a fresh MainWindow / AssemblyTreeModel / DockWorkspace /
/// SettingsService) and points settings at a fresh file before every test.
/// </para>
/// <para>
/// The AvaloniaTest command hoists <see cref="ITestAction"/> Before/After onto the Avalonia
/// dispatcher thread, immediately around the test body, so the container assigned here is the one
/// the test body resolves. Self-contained on purpose (no dependency on TestApp internals) so the
/// one file can be dropped into every commit of the build sweep.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ResetAppStateAttribute : Attribute, ITestAction
{
	static readonly string SessionsRoot = Path.Combine(Path.GetTempPath(), "ILSpy.Tests");

	string? previousSessionDir;

	public ActionTargets Targets => ActionTargets.Test;

	public void BeforeTest(ITest test)
	{
		// Clean up the prior test's settings dir lazily (deferred so the file is no longer locked).
		DeletePreviousSession();

		var dir = Path.Combine(SessionsRoot, Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		previousSessionDir = dir;
		var configPath = Path.Combine(dir, "ILSpy.xml");
		ILSpySettings.SettingsFilePathProvider = () => configPath;

		// Ensure the one-time plugin/resolver setup has happened. Under PerAssembly,
		// TestApp.OnFrameworkInitializationCompleted only runs when Avalonia is first built (the
		// first [AvaloniaTest]); a plain [Test] running earlier would otherwise hit CreateContainer
		// before RegisterPluginResolver. RegisterPluginResolver is idempotent.
		AppComposition.RegisterPluginResolver();

		// Fresh container -> fresh [Shared] singletons. CreateContainer disposes the prior one.
		AppComposition.CreateContainer();
	}

	public void AfterTest(ITest test)
	{
		// The AvaloniaTest command hoists Before/After onto the dispatcher thread ONLY for
		// [AvaloniaTest]s. For plain [Test]s this runs on the NUnit worker thread, where touching
		// the dispatcher (RunJobs / Window.Close) would throw VerifyAccess -- and a plain test
		// created no windows, so there is nothing to clean up. Guard on dispatcher access.
		if (Application.Current == null || !Dispatcher.UIThread.CheckAccess())
			return;

		TearDownTestState();
	}

	// Everything the per-test teardown does on the dispatcher thread; exposed so a test can
	// perform the teardown itself and check what it leaves behind (see TeardownRetentionTests).
	internal static void TearDownTestState()
	{
		// Drive background work to quiescence BEFORE the next test rebuilds the composition. A test
		// that triggers a decompile spawns a Task.Run plus dispatcher continuations and rarely awaits
		// them to completion; left running, that continuation lands during the next test and reads
		// the swapped-in AppComposition.Current -- the dominant source of order-dependent flakiness.
		// Cancel the in-flight decompiles and pump the dispatcher until they unwind (bounded).
		DrainPendingWork();

		// Close any windows the test showed so their view-models (alive and weakly subscribed to
		// MessageBus) can't react to events raised by later tests, then drain once more. A window
		// left open also outlives its container: the compositor keeps every open top level
		// reachable, and with it the view-models, the assembly tree and the loaded assemblies -
		// about 13 MB per test, which over the suite is what pushed the CI runner into paging.
		foreach (var window in openWindows.ToArray())
		{
			DetachFlyouts(window);
			window.Close();
		}
		Dispatcher.UIThread.RunJobs();
	}

	// Avalonia's Button subscribes to its flyout's Opened/Closed events when its template is
	// applied and unsubscribes only when the Flyout property changes, not when the button leaves
	// the tree. Dock's ToolChromeControl theme gives every tool pane's chrome button the same
	// MenuFlyout resource, so that one shared flyout would keep the visual tree of every window
	// this suite ever showed alive. Clearing the property before the window closes is what
	// makes the button let go.
	static void DetachFlyouts(Window window)
	{
		foreach (var button in window.GetVisualDescendants().OfType<Button>())
		{
			if (button.Flyout != null)
				button.Flyout = null;
		}
	}

	// The headless host runs the app without an application lifetime, so nothing tracks the
	// windows the tests show. These are the same class handlers ClassicDesktopStyleApplicationLifetime
	// installs to maintain its Windows list.
	static readonly List<Window> openWindows = new();

	static ResetAppStateAttribute()
	{
		Window.WindowOpenedEvent.AddClassHandler(typeof(Window), (sender, _) => {
			if (sender is Window window && !openWindows.Contains(window))
				openWindows.Add(window);
		});
		Window.WindowClosedEvent.AddClassHandler(typeof(Window), (sender, _) => {
			if (sender is Window window)
				openWindows.Remove(window);
		});
	}

	static void DrainPendingWork()
	{
		Task quiesce;
		try
		{
			quiesce = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>().CancelPendingOperationsAsync();
		}
		catch
		{
			// Composition not available (a plain [Test], or a boot that never built the workspace).
			Dispatcher.UIThread.RunJobs();
			return;
		}

		// We're on the dispatcher thread, so we can't block-await: pump the queue (which runs the
		// cancelled decompiles' continuations) and let the worker threads post, until everything has
		// settled or a safety deadline passes.
		var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
		while (!quiesce.IsCompleted && DateTime.UtcNow < deadline)
		{
			Dispatcher.UIThread.RunJobs();
			Thread.Sleep(5);
		}
		Dispatcher.UIThread.RunJobs();
	}

	void DeletePreviousSession()
	{
		if (previousSessionDir == null)
			return;
		try
		{
			if (Directory.Exists(previousSessionDir))
				Directory.Delete(previousSessionDir, recursive: true);
		}
		catch { /* best-effort */ }
		previousSessionDir = null;
	}
}
