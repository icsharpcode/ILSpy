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

using System.Diagnostics;
using System.Diagnostics.Tracing;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Search;

namespace ICSharpCode.ILSpyX.Instrumentation
{
	/// <summary>
	/// Outcome reported in the AssemblyResolveStop event.
	/// </summary>
	public enum AssemblyResolveOutcome
	{
		NotFound = 0,
		FoundInList = 1,
		LoadedFromDisk = 2,
		SimilarNameMatch = 3,
		ProvidedByParentResolver = 4,
	}

	/// <summary>
	/// Performance tracing for assembly loading, reference resolution, search, analyzers and
	/// package extraction.
	///
	/// The provider is consumable via ETW (PerfView) on Windows and via EventPipe
	/// (dotnet-trace) on all platforms. Start/Stop event pairs let trace viewers compute
	/// durations and nesting from the event timestamps; call sites therefore do not measure
	/// elapsed time themselves except for the high-volume single-shot events (per-entry
	/// extraction), which carry an explicit elapsedMs payload.
	///
	/// Where payload extraction would allocate on a hot path, call sites use the
	/// strongly-typed [NonEvent] overloads below, which check IsEnabled() before computing
	/// anything. The remaining [Event] methods are cheap enough to call unconditionally:
	/// WriteEvent no-ops internally when the provider is disabled.
	/// </summary>
	[EventSource(Name = "ICSharpCode.ILSpyX")]
	public sealed class ILSpyXEventSource : EventSource
	{
		public static class Keywords
		{
			/// <summary>Loading assembly files from disk (file loaders, PE parsing).</summary>
			public const EventKeywords AssemblyLoad = (EventKeywords)0x1;
			/// <summary>Assembly reference resolution and assembly-list lookup construction.</summary>
			public const EventKeywords Resolver = (EventKeywords)0x2;
			/// <summary>Per-module search strategy execution.</summary>
			public const EventKeywords Search = (EventKeywords)0x4;
			/// <summary>Analyzer scope determination (referencing-modules scans).</summary>
			public const EventKeywords Analyzers = (EventKeywords)0x8;
			/// <summary>Bundle/zip package opening and per-entry extraction (extraction is Verbose).</summary>
			public const EventKeywords Packages = (EventKeywords)0x10;
			/// <summary>Debug symbol (PDB) loading.</summary>
			public const EventKeywords DebugInfo = (EventKeywords)0x20;
		}

		[Event(1, Level = EventLevel.Informational, Keywords = Keywords.AssemblyLoad)]
		public void AssemblyLoadStart(string fileName)
		{
			WriteEvent(1, fileName);
		}

		[Event(2, Level = EventLevel.Informational, Keywords = Keywords.AssemblyLoad)]
		public void AssemblyLoadStop(string fileName, string loaderName, bool success)
		{
			WriteEvent(2, fileName, loaderName, success);
		}

		[Event(3, Level = EventLevel.Informational, Keywords = Keywords.Resolver)]
		public void AssemblyResolveStart(string referenceName)
		{
			WriteEvent(3, referenceName);
		}

		/// <param name="referenceName">Full name of the assembly reference.</param>
		/// <param name="outcome">One of the <see cref="AssemblyResolveOutcome"/> values.</param>
		[Event(4, Level = EventLevel.Informational, Keywords = Keywords.Resolver)]
		public void AssemblyResolveStop(string referenceName, int outcome)
		{
			WriteEvent(4, referenceName, outcome);
		}

		[Event(5, Level = EventLevel.Informational, Keywords = Keywords.Resolver)]
		public void SnapshotLookupBuildStart(int assemblyCount)
		{
			WriteEvent(5, assemblyCount);
		}

		[Event(6, Level = EventLevel.Informational, Keywords = Keywords.Resolver)]
		public void SnapshotLookupBuildStop(int assemblyCount)
		{
			WriteEvent(6, assemblyCount);
		}

		[Event(7, Level = EventLevel.Informational, Keywords = Keywords.DebugInfo)]
		public void DebugInfoLoadStart(string assemblyFileName)
		{
			WriteEvent(7, assemblyFileName);
		}

		[Event(8, Level = EventLevel.Informational, Keywords = Keywords.DebugInfo)]
		public void DebugInfoLoadStop(string assemblyFileName, string providerKind)
		{
			WriteEvent(8, assemblyFileName, providerKind);
		}

		[Event(9, Level = EventLevel.Informational, Keywords = Keywords.Search)]
		public void SearchModuleStart(string moduleName, string strategyName)
		{
			WriteEvent(9, moduleName, strategyName);
		}

		[Event(10, Level = EventLevel.Informational, Keywords = Keywords.Search)]
		public void SearchModuleStop(string moduleName, string strategyName)
		{
			WriteEvent(10, moduleName, strategyName);
		}

		[Event(11, Level = EventLevel.Informational, Keywords = Keywords.Analyzers)]
		public void AnalyzerScopeStart(string analyzedEntityName)
		{
			WriteEvent(11, analyzedEntityName);
		}

		[Event(12, Level = EventLevel.Informational, Keywords = Keywords.Analyzers)]
		public void AnalyzerScopeStop(string analyzedEntityName, int modulesInScope)
		{
			WriteEvent(12, analyzedEntityName, modulesInScope);
		}

		[Event(13, Level = EventLevel.Informational, Keywords = Keywords.Packages)]
		public void PackageOpened(string fileName, string packageKind, int entryCount)
		{
			WriteEvent(13, fileName, packageKind, entryCount);
		}

		[Event(14, Level = EventLevel.Verbose, Keywords = Keywords.Packages)]
		public void PackageEntryExtracted(string entryName, long bytes, double elapsedMs)
		{
			WriteEvent(14, entryName, bytes, elapsedMs);
		}

		// Strongly-typed entry points for hot paths: they check IsEnabled() before
		// extracting payloads, so a disabled provider costs a single branch and zero
		// allocations.

		[NonEvent]
		public void AssemblyResolveStart(IAssemblyReference reference)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Resolver))
				AssemblyResolveStart(reference.FullName);
		}

		[NonEvent]
		public void AssemblyResolveStop(IAssemblyReference reference, AssemblyResolveOutcome outcome)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Resolver))
				AssemblyResolveStop(reference.FullName, (int)outcome);
		}

		[NonEvent]
		public void SearchModuleStart(MetadataFile module, AbstractSearchStrategy strategy)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Search))
				SearchModuleStart(module.Name, strategy.GetType().Name);
		}

		[NonEvent]
		public void SearchModuleStop(MetadataFile module, AbstractSearchStrategy strategy)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Search))
				SearchModuleStop(module.Name, strategy.GetType().Name);
		}

		/// <summary>
		/// Fractional milliseconds elapsed since a <see cref="Stopwatch.GetTimestamp"/> value.
		/// </summary>
		[NonEvent]
		public static double ElapsedMilliseconds(long startTimestamp)
		{
			return (Stopwatch.GetTimestamp() - startTimestamp) * 1000.0 / Stopwatch.Frequency;
		}

		/// <summary>
		/// Gate for the analyzer scope span: AnalyzerScope only wraps its lazily-evaluated
		/// module enumeration in the tracing iterator when a session is attached.
		/// </summary>
		[NonEvent]
		public bool IsAnalyzerTracingEnabled()
		{
			return IsEnabled(EventLevel.Informational, Keywords.Analyzers);
		}

		/// <summary>
		/// Gate for the per-entry extraction events: extraction sites capture this once so
		/// they skip the Stopwatch.GetTimestamp() calls when no Verbose session is attached.
		/// </summary>
		[NonEvent]
		public bool IsPackageExtractionTracingEnabled()
		{
			return IsEnabled(EventLevel.Verbose, Keywords.Packages);
		}

		public static readonly ILSpyXEventSource Log = new ILSpyXEventSource();
	}
}
