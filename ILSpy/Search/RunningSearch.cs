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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Search;

using ILSpy.Languages;

namespace ILSpy.Search
{
	/// <summary>
	/// Orchestrates one search across the loaded <see cref="AssemblyList"/>. Builds a
	/// <see cref="SearchRequest"/>, picks the matching <see cref="AbstractSearchStrategy"/>,
	/// runs it on a background task, and streams each emitted <see cref="SearchResult"/>
	/// into the supplied UI-thread <see cref="ObservableCollection{T}"/> via
	/// <see cref="Dispatcher.UIThread"/>.
	/// </summary>
	internal sealed class RunningSearch
	{
		const int MaxResults = 1000;

		readonly IReadOnlyList<LoadedAssembly> assemblies;
		readonly SearchMode mode;
		readonly string searchTerm;
		readonly Language language;
		readonly ApiVisibility apiVisibility;
		readonly ISearchResultFactory resultFactory;
		readonly ObservableCollection<SearchResult> sink;
		readonly ConcurrentQueue<SearchResult> queue = new();
		readonly CancellationTokenSource cts = new();
		Task? runTask;

		public RunningSearch(
			IReadOnlyList<LoadedAssembly> assemblies,
			string searchTerm,
			SearchMode mode,
			Language language,
			ApiVisibility apiVisibility,
			ISearchResultFactory resultFactory,
			ObservableCollection<SearchResult> sink)
		{
			this.assemblies = assemblies;
			this.searchTerm = searchTerm;
			this.mode = mode;
			this.language = language;
			this.apiVisibility = apiVisibility;
			this.resultFactory = resultFactory;
			this.sink = sink;
		}

		public bool IsCompleted => runTask is { IsCompleted: true };

		public void Start()
		{
			var token = cts.Token;
			// Task.Run with an async lambda wraps the inner Task so runTask only completes
			// after RunSearchAsync actually finishes — without the wrapping, runTask would
			// complete the moment Task.Run returned the inner Task to the caller, and the
			// drain loop would exit before any results landed.
			runTask = Task.Run(async () => await RunSearchAsync(token).ConfigureAwait(false), token);
			_ = DrainQueueAsync(token);
		}

		public void Cancel()
		{
			cts.Cancel();
		}

		async Task RunSearchAsync(CancellationToken ct)
		{
			try
			{
				var request = BuildRequest();
				var strategy = GetStrategy(request);
				if (strategy == null)
					return;
				foreach (var assembly in assemblies)
				{
					ct.ThrowIfCancellationRequested();
					// Force the load so search hits the metadata even for assemblies the user
					// hasn't expanded in the tree yet. Failed loads are already surfaced in
					// the assembly tree with the AssemblyWarning icon — don't double-report
					// here, just skip so the search keeps streaming results from the
					// healthy assemblies.
					MetadataFile? module;
					try
					{
						module = await assembly.GetMetadataFileAsync().ConfigureAwait(false);
					}
					catch (Exception ex) when (IsExpectedLoadFailure(ex))
					{
						continue;
					}
					if (module == null)
						continue;
					strategy.Search(module, ct);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected on user-driven term/mode change.
			}
		}

		static bool IsExpectedLoadFailure(Exception ex) => ex is
			BadImageFormatException
			or IOException
			or InvalidOperationException
			or UnauthorizedAccessException
			or NotSupportedException
			or ReflectionTypeLoadException;

		SearchRequest BuildRequest()
		{
			// Minimal keyword parser: split on whitespace. The WPF pane supports a richer
			// prefix DSL (inassembly:, t:, /regex/, =exact, ~fuzzy, …) — that lands as a
			// follow-up. Plain-keyword search covers the common case end-to-end.
			var keywords = (searchTerm ?? string.Empty)
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			return new SearchRequest {
				Mode = mode,
				Keywords = keywords,
				SearchResultFactory = resultFactory,
				// MemberSearchStrategy.Search resolves a type system via
				// module.GetTypeSystemWithDecompilerSettingsOrNull(request.DecompilerSettings);
				// passing null short-circuits to zero results. Default settings are fine for
				// search — we only need the type system to materialise, not specific
				// decompiler behaviour.
				DecompilerSettings = new DecompilerSettings(),
				FullNameSearch = false,
				OmitGenerics = false,
				// IsInNamespaceOrAssembly treats null as "no filter, accept everything";
				// the EMPTY STRING would restrict to the global namespace (Namespace.Length
				// == 0), which matches almost nothing. The DSL prefixes that flip these on
				// (innamespace:, inassembly:) are out of scope for the minimal parser.
				InNamespace = null!,
				InAssembly = null!,
			};
		}

		AbstractSearchStrategy? GetStrategy(SearchRequest request)
		{
			if (request.Keywords.Length == 0 && request.RegEx is null)
				return null;
			return mode switch {
				SearchMode.TypeAndMember => new MemberSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Type => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Type),
				SearchMode.Member => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Member),
				SearchMode.Method => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Method),
				SearchMode.Field => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Field),
				SearchMode.Property => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Property),
				SearchMode.Event => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Event),
				SearchMode.Literal => new LiteralSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Token => new MetadataTokenSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Assembly => new AssemblySearchStrategy(request, queue, AssemblySearchKind.NameOrFileName),
				SearchMode.Namespace => new NamespaceSearchStrategy(request, queue),
				// Resource search needs ITreeNodeFactory infrastructure — deferred to a follow-up.
				_ => null,
			};
		}

		async Task DrainQueueAsync(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					var batch = new List<SearchResult>();
					while (queue.TryDequeue(out var result) && batch.Count < 100)
						batch.Add(result);
					if (batch.Count > 0)
					{
						Dispatcher.UIThread.Post(() => {
							foreach (var r in batch)
							{
								if (sink.Count >= MaxResults)
									break;
								sink.Add(r);
							}
						});
					}
					if (runTask is { IsCompleted: true } && queue.IsEmpty)
						return;
					await Task.Delay(50, ct).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected when the run is replaced or cleared.
			}
		}
	}
}
