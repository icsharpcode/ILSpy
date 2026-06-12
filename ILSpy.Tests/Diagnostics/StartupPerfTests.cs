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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Hand-run benchmarks for startup-related code paths under load. Marked
/// <see cref="ExplicitAttribute"/> so the regular test suite skips them — they're
/// expensive (file copies + waiting on metadata loads) and the timings depend on
/// the host machine's IO + thread-pool contention. Run via:
///   <c>dotnet test ILSpy.Tests --filter "Category=Performance"</c>
/// or invoke a single benchmark by name. Output is written to NUnit's TestContext
/// log so the timings show up next to each test in the runner.
/// </summary>
[TestFixture]
[Category("Performance")]
public class StartupPerfTests
{
	const int LargeListAssemblyCount = 200;

	[Explicit("Perf benchmark — emits timings for manual inspection")]
	[AvaloniaTest]
	public async Task BindTree_With_Large_AssemblyList_Reports_Phase_Timings()
	{
		// Simulates the user-perceived startup of a saved list with many assemblies. The
		// composition is shared across tests, so we can't measure cold-start of MainWindow
		// — instead we time the phases that dominate startup with N assemblies: opening the
		// LoadedAssembly entries, settling the AssemblyList, and waiting for every metadata
		// load to finish. Compare these numbers against the same run after a code change to
		// see if startup got better or worse.

		// Arrange — N temp copies of CoreLib so OpenAssembly can't dedupe them.
		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpy.PerfTest", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var sourcePath = typeof(object).Assembly.Location;
			TestContext.Out.WriteLine($"Cloning {LargeListAssemblyCount} copies of {sourcePath} to {tempDir}");
			var swCopy = Stopwatch.StartNew();
			var copies = new string[LargeListAssemblyCount];
			for (int i = 0; i < LargeListAssemblyCount; i++)
			{
				copies[i] = Path.Combine(tempDir, $"copy{i:D4}.dll");
				File.Copy(sourcePath, copies[i]);
			}
			swCopy.Stop();
			TestContext.Out.WriteLine($"  file copy: {swCopy.ElapsedMilliseconds} ms");

			// Boot the window + wait for the standard 3 assemblies (CoreLib + Uri + Linq) to
			// settle so they don't pollute the per-assembly timing.
			var window = AppComposition.Current.GetExport<MainWindow>();
			window.Show();
			var vm = (MainWindowViewModel)window.DataContext!;
			await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
			var baselineCount = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length;
			TestContext.Out.WriteLine($"Baseline: {baselineCount} assemblies after window opened");

			// Act 1 — fire OpenAssembly for every clone. Each call constructs a LoadedAssembly
			// and queues a Task.Run(LoadAsync), so this is essentially the cost of the
			// in-memory bookkeeping (path canonicalisation, dictionary insert, list append).
			var swOpen = Stopwatch.StartNew();
			foreach (var path in copies)
				vm.AssemblyTreeModel.AssemblyList!.OpenAssembly(path);
			swOpen.Stop();
			TestContext.Out.WriteLine($"OpenAssembly x{LargeListAssemblyCount}: {swOpen.ElapsedMilliseconds} ms "
				+ $"({swOpen.ElapsedMilliseconds / (double)LargeListAssemblyCount:0.##} ms/asm)");

			// Act 2 — wait for the AssemblyList to actually contain all the new entries (the
			// add is dispatched onto the UI thread when called from a worker thread; we're on
			// the UI thread here so the add was synchronous, but we still need to wait for the
			// AssemblyListTreeNode to project the new children into the tree).
			var swSettle = Stopwatch.StartNew();
			await Waiters.WaitForAsync(
				() => vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length >= baselineCount + LargeListAssemblyCount,
				timeout: TimeSpan.FromSeconds(60));
			swSettle.Stop();
			TestContext.Out.WriteLine($"AssemblyList settled to {baselineCount + LargeListAssemblyCount}: {swSettle.ElapsedMilliseconds} ms");

			// Act 3 — wait for every assembly's metadata load to complete. This is the work
			// that the async-restore path defers when the user has a saved selection.
			var allAssemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies();
			var swLoad = Stopwatch.StartNew();
			await Task.WhenAll(allAssemblies.Select(a => a.GetLoadResultAsync()));
			swLoad.Stop();
			TestContext.Out.WriteLine($"All {allAssemblies.Length} GetLoadResultAsync awaited: {swLoad.ElapsedMilliseconds} ms "
				+ $"({swLoad.ElapsedMilliseconds / (double)allAssemblies.Length:0.##} ms/asm)");

			// Act 4 — the assembly-tree pane should still be responsive. Measure how long it
			// takes the AssemblyListPane to produce a fresh SharpTreeView descendant from this point
			// (proxy for "tree is interactive").
			var pane = await window.WaitForComponent<AssemblyListPane>();
			var swGrid = Stopwatch.StartNew();
			var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();
			swGrid.Stop();
			TestContext.Out.WriteLine($"SharpTreeView descendant available: {swGrid.ElapsedMilliseconds} ms");

			// Sanity assertions — large enough to never trip on slow machines, but tight
			// enough to flag a 10× regression.
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length
				.Should().BeGreaterThanOrEqualTo(baselineCount + LargeListAssemblyCount);
			swOpen.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30),
				"opening N LoadedAssembly entries is in-memory work and should never get this slow");

			// Surface the StartupLog elapsed (ms since process start) so the test output also
			// captures the big-picture timing alongside the per-phase deltas above.
			AppLog.Mark("StartupPerfTests benchmark completed");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* test cleanup must never fail */ }
		}
	}

	/// <summary>
	/// CI-runnable variant of <see cref="BindTree_With_Large_AssemblyList_Reports_Phase_Timings"/>.
	/// Same shape but with a much smaller assembly count (8 vs 200) so it can run inside the
	/// regular suite — catches order-of-magnitude regressions in the open + load pipeline
	/// without the 30+ second cost of the full benchmark. NOT [Explicit] on purpose.
	/// </summary>
	[AvaloniaTest]
	public async Task BindTree_With_Small_AssemblyList_Settles_In_Reasonable_Time()
	{
		const int Copies = 8;
		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpy.PerfTest.CI", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var sourcePath = typeof(object).Assembly.Location;
			var clones = new string[Copies];
			for (int i = 0; i < Copies; i++)
			{
				clones[i] = Path.Combine(tempDir, $"copy{i:D2}.dll");
				File.Copy(sourcePath, clones[i]);
			}

			var window = AppComposition.Current.GetExport<MainWindow>();
			window.Show();
			var vm = (MainWindowViewModel)window.DataContext!;
			await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
			var baselineCount = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length;

			var swTotal = Stopwatch.StartNew();
			foreach (var path in clones)
				vm.AssemblyTreeModel.AssemblyList!.OpenAssembly(path);
			await Waiters.WaitForAsync(
				() => vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length >= baselineCount + Copies,
				timeout: TimeSpan.FromSeconds(60));
			swTotal.Stop();

			// Relaxed CI threshold — 8 assemblies should never take more than 15s, even on a
			// shared CI runner. A 10× regression of the open-and-settle pipeline trips this.
			swTotal.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(15),
				"opening + settling 8 LoadedAssembly entries should complete in well under 15s");
			vm.AssemblyTreeModel.AssemblyList!.GetAssemblies().Length
				.Should().BeGreaterThanOrEqualTo(baselineCount + Copies);
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* cleanup must never fail */ }
		}
	}

	const int ResponsivenessAssemblyCount = 200;

	[Explicit("Perf benchmark — emits dispatcher-latency stats for manual inspection")]
	[AvaloniaTest]
	public async Task UI_Stays_Responsive_While_Many_Assemblies_Load()
	{
		// Probes <see cref="Dispatcher.UIThread"/> latency at <see cref="DispatcherPriority.Background"/>
		// continuously while a flood of assemblies is added and their metadata loads on the
		// background thread pool. If the UI thread blocks (e.g. someone re-introduces a
		// .GetAwaiter().GetResult() on the saved-path restore), latency spikes and the test
		// reports it. Run with:
		//   dotnet test --filter "FullyQualifiedName~UI_Stays_Responsive"

		// Setup: clone CoreLib N times.
		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpy.PerfTest", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var sourcePath = typeof(object).Assembly.Location;
			var copies = new string[ResponsivenessAssemblyCount];
			for (int i = 0; i < ResponsivenessAssemblyCount; i++)
			{
				copies[i] = Path.Combine(tempDir, $"copy{i:D4}.dll");
				File.Copy(sourcePath, copies[i]);
			}

			var window = AppComposition.Current.GetExport<MainWindow>();
			window.Show();
			var vm = (MainWindowViewModel)window.DataContext!;
			await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

			// Trigger the heavy load: each OpenAssembly queues a Task.Run(LoadAsync), so the
			// metadata IO + parsing happens on the thread pool — the UI thread is free to keep
			// pumping the dispatcher.
			var swLoad = Stopwatch.StartNew();
			foreach (var path in copies)
				vm.AssemblyTreeModel.AssemblyList!.OpenAssembly(path);
			var allAssemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies();
			var loadTasks = allAssemblies.Select(a => a.GetLoadResultAsync()).ToArray();

			// Probe the dispatcher from the UI thread itself. Each iteration:
			//   1) await an InvokeAsync(noop, Background) — this measures how long the
			//      dispatcher takes to service a low-priority callback
			//   2) await Task.Delay(20) — yields to the dispatcher between samples
			// If the UI thread blocks synchronously (e.g. someone re-introduces a
			// .GetAwaiter().GetResult() call in the load path), that block manifests as a
			// huge step in the InvokeAsync latency for that iteration.
			var latencies = new List<long>();
			var probeStart = Stopwatch.StartNew();
			while (!loadTasks.All(t => t.IsCompleted) && probeStart.Elapsed < TimeSpan.FromMinutes(2))
			{
				var sw = Stopwatch.StartNew();
				await Dispatcher.UIThread.InvokeAsync(static () => { }, DispatcherPriority.Background);
				sw.Stop();
				latencies.Add(sw.ElapsedMilliseconds);
				await Task.Delay(20);
			}
			swLoad.Stop();

			if (latencies.Count == 0)
			{
				Assert.Fail("dispatcher probe collected no samples — load may have completed before the probe could run");
				return;
			}

			latencies.Sort();
			long max = latencies[^1];
			long p50 = latencies[latencies.Count / 2];
			long p95 = latencies[(int)(latencies.Count * 0.95)];
			double mean = latencies.Average();

			TestContext.Out.WriteLine($"Load completed in {swLoad.ElapsedMilliseconds} ms across {ResponsivenessAssemblyCount} assemblies.");
			TestContext.Out.WriteLine($"Dispatcher Background-priority latency over {latencies.Count} samples:");
			TestContext.Out.WriteLine($"  max  {max} ms");
			TestContext.Out.WriteLine($"  p95  {p95} ms");
			TestContext.Out.WriteLine($"  p50  {p50} ms");
			TestContext.Out.WriteLine($"  mean {mean:0} ms");

			// p95 keeps the threshold honest — occasional one-off spikes (GC, JIT, the assembly
			// post-load Add-to-collection notification) are fine. 200 ms is the boundary where
			// the user starts perceiving "stutter"; a sync block on the UI thread would push
			// p95 into the seconds.
			p95.Should().BeLessThan(200,
				$"the UI thread must stay responsive while {ResponsivenessAssemblyCount} assemblies load — "
				+ $"p95 latency >= 200 ms means something is blocking the dispatcher");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* test cleanup must never fail */ }
		}
	}
}
