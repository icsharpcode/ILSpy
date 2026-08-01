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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Instrumentation;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Instrumentation;

[TestFixture]
public class ILSpyXEventSourceTests
{
	/// <summary>
	/// Captures all events the "ICSharpCode.ILSpyX" provider emits while the listener is
	/// alive. Events are recorded process-wide, so assertions must filter by payload to stay
	/// robust under parallel test runs.
	/// </summary>
	sealed class RecordingListener : EventListener
	{
		readonly ConcurrentQueue<(string EventName, Dictionary<string, object?> Payload)> events = new();

		public RecordingListener(EventLevel level, EventKeywords keywords)
		{
			EnableEvents(ILSpyXEventSource.Log, level, keywords);
		}

		protected override void OnEventWritten(EventWrittenEventArgs eventData)
		{
			var payload = new Dictionary<string, object?>();
			if (eventData.PayloadNames != null && eventData.Payload != null)
			{
				for (int i = 0; i < eventData.PayloadNames.Count; i++)
				{
					payload[eventData.PayloadNames[i]] = eventData.Payload[i];
				}
			}
			events.Enqueue((eventData.EventName ?? "", payload));
		}

		public List<Dictionary<string, object?>> EventsNamed(string eventName)
		{
			return events.Where(e => e.EventName == eventName).Select(e => e.Payload).ToList();
		}
	}

	[Test]
	public void ManifestIsValid()
	{
		string? manifest = EventSource.GenerateManifest(typeof(ILSpyXEventSource), typeof(ILSpyXEventSource).Assembly.Location, EventManifestOptions.Strict);
		Assert.That(manifest, Is.Not.Null.And.Not.Empty);
	}

	[Test]
	public void FiringEveryEventProducesNoEventSourceErrors()
	{
		// The provider is process-wide and other tests load/search assemblies concurrently,
		// so every string payload carries this marker and the count assertions filter on it.
		// SnapshotLookupBuildStart/Stop have no string payload; their sentinel assemblyCount
		// is distinctive enough (real snapshots in these tests stay far below it).
		const string marker = "SchemaSmokeTest.";
		const int lookupSentinel = 987654;
		using var listener = new RecordingListener(EventLevel.Verbose, EventKeywords.All);
		var log = ILSpyXEventSource.Log;
		log.AssemblyLoadStart(marker + "test.dll");
		log.AssemblyLoadStop(marker + "test.dll", "PEFileLoader", true);
		log.AssemblyResolveStart(marker + "Reference");
		log.AssemblyResolveStop(marker + "Reference", (int)AssemblyResolveOutcome.FoundInList);
		log.SnapshotLookupBuildStart(lookupSentinel);
		log.SnapshotLookupBuildStop(lookupSentinel);
		log.DebugInfoLoadStart(marker + "test.dll");
		log.DebugInfoLoadStop(marker + "test.dll", "PortableDebugInfoProvider");
		log.SearchModuleStart(marker + "TestModule", "MemberSearchStrategy");
		log.SearchModuleStop(marker + "TestModule", "MemberSearchStrategy");
		log.AnalyzerScopeStart(marker + "MyType");
		log.AnalyzerScopeStop(marker + "MyType", 3);
		log.PackageOpened(marker + "app.zip", "zip", 12);
		log.PackageEntryExtracted(marker + "lib/test.dll", 4096L, 0.5);

		// A mismatch between an [Event] method's signature and its WriteEvent call surfaces
		// as an "EventSourceMessage" error event on the same provider.
		Assert.That(listener.EventsNamed("EventSourceMessage"), Is.Empty);

		string[] expected = {
			"AssemblyLoadStart", "AssemblyLoadStop",
			"AssemblyResolveStart", "AssemblyResolveStop",
			"DebugInfoLoadStart", "DebugInfoLoadStop",
			"SearchModuleStart", "SearchModuleStop",
			"AnalyzerScopeStart", "AnalyzerScopeStop",
			"PackageOpened",
			"PackageEntryExtracted",
		};
		foreach (string eventName in expected)
		{
			var markedEvents = listener.EventsNamed(eventName)
				.Where(p => p.Values.OfType<string>().Any(v => v.StartsWith(marker, StringComparison.Ordinal)))
				.ToList();
			Assert.That(markedEvents, Has.Count.EqualTo(1), eventName);
		}

		foreach (string eventName in new[] { "SnapshotLookupBuildStart", "SnapshotLookupBuildStop" })
		{
			var markedEvents = listener.EventsNamed(eventName)
				.Where(p => (int)p["assemblyCount"]! == lookupSentinel)
				.ToList();
			Assert.That(markedEvents, Has.Count.EqualTo(1), eventName);
		}
	}

	[Test]
	public void LoadingAssemblyEmitsLoadAndResolveEvents()
	{
		using var listener = new RecordingListener(EventLevel.Informational,
			ILSpyXEventSource.Keywords.AssemblyLoad | ILSpyXEventSource.Keywords.Resolver);
		string location = typeof(ILSpyXEventSourceTests).Assembly.Location;

		var assemblyList = new AssemblyList();
		var asm = assemblyList.OpenAssembly(location);
		var module = asm.GetMetadataFileOrNull();
		Assert.That(module, Is.Not.Null);

		var loadStarts = listener.EventsNamed("AssemblyLoadStart")
			.Where(p => (string?)p["fileName"] == location).ToList();
		var loadStops = listener.EventsNamed("AssemblyLoadStop")
			.Where(p => (string?)p["fileName"] == location).ToList();
		Assert.That(loadStarts, Has.Count.EqualTo(1));
		Assert.That(loadStops, Has.Count.EqualTo(1));
		Assert.That((bool)loadStops[0]["success"]!, Is.True);
		Assert.That((string?)loadStops[0]["loaderName"], Is.Not.Empty);

		var resolver = asm.GetAssemblyResolver(loadOnDemand: false);
		var reference = module!.AssemblyReferences.First();
		resolver.Resolve(reference);

		var resolveStarts = listener.EventsNamed("AssemblyResolveStart")
			.Where(p => (string?)p["referenceName"] == reference.FullName).ToList();
		var resolveStops = listener.EventsNamed("AssemblyResolveStop")
			.Where(p => (string?)p["referenceName"] == reference.FullName).ToList();
		Assert.That(resolveStarts, Has.Count.EqualTo(1));
		Assert.That(resolveStops, Has.Count.EqualTo(1));
		int outcome = (int)resolveStops[0]["outcome"]!;
		Assert.That(outcome, Is.InRange((int)AssemblyResolveOutcome.NotFound, (int)AssemblyResolveOutcome.ProvidedByParentResolver));

		// The first resolve against a snapshot builds the assembly lookup table.
		var lookupStarts = listener.EventsNamed("SnapshotLookupBuildStart");
		var lookupStops = listener.EventsNamed("SnapshotLookupBuildStop");
		Assert.That(lookupStarts, Is.Not.Empty);
		Assert.That(lookupStops, Has.Count.EqualTo(lookupStarts.Count));
		Assert.That(lookupStarts.Select(p => (int)p["assemblyCount"]!), Has.All.GreaterThanOrEqualTo(1));
	}

	[Test]
	public void LoadingAssemblyWithDebugSymbolsEmitsDebugInfoEvents()
	{
		using var listener = new RecordingListener(EventLevel.Informational, ILSpyXEventSource.Keywords.DebugInfo);
		string location = typeof(ILSpyXEventSourceTests).Assembly.Location;

		var assemblyList = new AssemblyList {
			UseDebugSymbols = true
		};
		var asm = assemblyList.OpenAssembly(location);
		Assert.That(asm.GetMetadataFileOrNull(), Is.Not.Null);

		var starts = listener.EventsNamed("DebugInfoLoadStart")
			.Where(p => (string?)p["assemblyFileName"] == location).ToList();
		var stops = listener.EventsNamed("DebugInfoLoadStop")
			.Where(p => (string?)p["assemblyFileName"] == location).ToList();
		Assert.That(starts, Has.Count.EqualTo(1));
		Assert.That(stops, Has.Count.EqualTo(1));
		// The test assembly ships a portable PDB next to it.
		Assert.That((string?)stops[0]["providerKind"], Is.EqualTo("PortableDebugInfoProvider"));
	}

	[AvaloniaTest]
	public async Task RunningASearchEmitsSearchModuleEvents()
	{
		using var listener = new RecordingListener(EventLevel.Informational, ILSpyXEventSource.Keywords.Search);
		await TestHarness.BootAsync();

		var search = AppComposition.Current.GetExport<SearchPaneModel>();
		search.Results.Clear();
		search.SelectedSearchMode = search.SearchModes.First(m => m.Mode == SearchMode.Type);
		search.SearchTerm = "Enumerable";
		await Waiters.WaitForAsync(() => search.Results.Count > 0, timeout: TimeSpan.FromSeconds(30));

		// The search keeps walking the remaining assemblies after the first result;
		// wait until every started module span has closed.
		await Waiters.WaitForAsync(
			() => {
				var startCount = listener.EventsNamed("SearchModuleStart").Count;
				return startCount > 0 && listener.EventsNamed("SearchModuleStop").Count == startCount;
			},
			timeout: TimeSpan.FromSeconds(30));

		var starts = listener.EventsNamed("SearchModuleStart");
		var stops = listener.EventsNamed("SearchModuleStop");
		Assert.That(starts, Is.Not.Empty);
		Assert.That(stops, Has.Count.EqualTo(starts.Count));
		Assert.That(starts.Select(p => (string?)p["strategyName"]), Has.All.EqualTo("MemberSearchStrategy"));
		Assert.That(starts.Select(p => (string?)p["moduleName"]), Has.All.Not.Empty);
	}

	[Test]
	public void AnalyzerScopeEnumerationEmitsScopeEvents()
	{
		using var listener = new RecordingListener(EventLevel.Informational, ILSpyXEventSource.Keywords.Analyzers);

		var assemblyList = new AssemblyList();
		var asm = assemblyList.OpenAssembly(typeof(ILSpyXEventSourceTests).Assembly.Location);
		var typeSystem = new DecompilerTypeSystem(asm.GetMetadataFileOrNull()!, asm.GetAssemblyResolver());
		var typeDef = typeSystem.FindType(typeof(ILSpyXEventSourceTests)).GetDefinition()!;

		var scope = new AnalyzerScope(assemblyList, typeDef);
		var modules = scope.GetModulesInScope(CancellationToken.None).ToList();
		Assert.That(modules, Is.Not.Empty);

		var starts = listener.EventsNamed("AnalyzerScopeStart")
			.Where(p => ((string?)p["analyzedEntityName"])?.Contains(nameof(ILSpyXEventSourceTests)) == true).ToList();
		var stops = listener.EventsNamed("AnalyzerScopeStop")
			.Where(p => ((string?)p["analyzedEntityName"])?.Contains(nameof(ILSpyXEventSourceTests)) == true).ToList();
		Assert.That(starts, Has.Count.EqualTo(1));
		Assert.That(stops, Has.Count.EqualTo(1));
		Assert.That((int)stops[0]["modulesInScope"]!, Is.EqualTo(modules.Count));
	}

	[Test]
	public void ZipPackageEmitsOpenAndExtractionEvents()
	{
		using var listener = new RecordingListener(EventLevel.Verbose, ILSpyXEventSource.Keywords.Packages);

		string zipPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
		byte[] content = new byte[128];
		try
		{
			using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
			{
				var entry = archive.CreateEntry("lib/test.bin");
				using var stream = entry.Open();
				stream.Write(content, 0, content.Length);
			}

			var package = LoadedPackage.FromZipFile(zipPath);
			var opened = listener.EventsNamed("PackageOpened")
				.Where(p => (string?)p["fileName"] == zipPath).ToList();
			Assert.That(opened, Has.Count.EqualTo(1));
			Assert.That((string?)opened[0]["packageKind"], Is.EqualTo("zip"));
			Assert.That((int)opened[0]["entryCount"]!, Is.EqualTo(1));

			using var entryStream = package.Entries.Single().TryOpenStream();
			Assert.That(entryStream, Is.Not.Null);

			var extracted = listener.EventsNamed("PackageEntryExtracted")
				.Where(p => (string?)p["entryName"] == "lib/test.bin").ToList();
			Assert.That(extracted, Has.Count.EqualTo(1));
			Assert.That((long)extracted[0]["bytes"]!, Is.EqualTo(content.Length));
			Assert.That((double)extracted[0]["elapsedMs"]!, Is.GreaterThanOrEqualTo(0.0));
		}
		finally
		{
			File.Delete(zipPath);
		}
	}
}
