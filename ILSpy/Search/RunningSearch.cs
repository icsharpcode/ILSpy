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

		/// <summary>
		/// Fires on the UI thread when the run finishes (success, error, or cancellation).
		/// Always fires exactly once — the search-pane VM uses it to flip its IsSearching
		/// flag back to false. Subscribers are invoked AFTER the drain loop has flushed
		/// the queue so post-completion result reads see the final state.
		/// </summary>
		public event Action<RunningSearch>? Completed;

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
				// Parallelise per-assembly: each strategy.Search is CPU-bound metadata walk,
				// each GetMetadataFileAsync is I/O. Running them sequentially makes early
				// rows arrive instantly then pause while later assemblies load — looks like
				// a stall to the user. The shared ConcurrentQueue is thread-safe; the strategy
				// has no mutable per-call state so concurrent Search invocations on different
				// modules are safe.
				await Parallel.ForEachAsync(
					assemblies,
					new ParallelOptions {
						MaxDegreeOfParallelism = Math.Min(4, Math.Max(1, Environment.ProcessorCount)),
						CancellationToken = ct,
					},
					async (assembly, token) => {
						MetadataFile? module;
						try
						{
							module = await assembly.GetMetadataFileAsync().ConfigureAwait(false);
						}
						catch (Exception ex) when (IsExpectedLoadFailure(ex))
						{
							// Broken DLL / missing reference — already surfaced in the assembly
							// tree with AssemblyWarning, so don't double-report here.
							return;
						}
						if (module == null)
							return;
						try
						{
							strategy.Search(module, token);
						}
						catch (OperationCanceledException)
						{
							throw;
						}
						catch
						{
							// One assembly's strategy walker hit something it didn't expect
							// (malformed metadata, missing dependency, internal Debug.Assert
							// outside Debug-build defaults, …). Skip just that assembly so the
							// rest of the search keeps streaming results from healthy ones —
							// without this catch, ONE bad assembly faults the entire run and
							// the user sees the partial results stall after a handful of hits.
						}
					}).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				// Expected on user-driven term/mode change, or when the cap fires below.
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
			// Lightweight parser: split on whitespace, pull off the two scope prefixes
			// (inassembly: / innamespace:) when they appear. The full WPF prefix DSL
			// (/regex/, =exact, ~fuzzy, t: / m:, @token, …) is out of scope; plain
			// keyword + scope-to context-menu integration covers the common cases.
			var tokens = (searchTerm ?? string.Empty)
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			var keywords = new List<string>(tokens.Length);
			string? inAssembly = null;
			string? inNamespace = null;
			foreach (var token in tokens)
			{
				if (token.StartsWith("inassembly:", StringComparison.OrdinalIgnoreCase))
					inAssembly = token.Substring("inassembly:".Length).Trim('"');
				else if (token.StartsWith("innamespace:", StringComparison.OrdinalIgnoreCase))
					inNamespace = token.Substring("innamespace:".Length).Trim('"');
				else
					keywords.Add(token);
			}
			return new SearchRequest {
				Mode = mode,
				Keywords = keywords.ToArray(),
				SearchResultFactory = resultFactory,
				// MemberSearchStrategy.Search resolves a type system via
				// module.GetTypeSystemWithDecompilerSettingsOrNull(request.DecompilerSettings);
				// passing null short-circuits to zero results. Default settings are fine for
				// search — we only need the type system to materialise.
				DecompilerSettings = new DecompilerSettings(),
				FullNameSearch = false,
				OmitGenerics = false,
				// IsInNamespaceOrAssembly: null means "no filter, accept everything";
				// the EMPTY STRING would restrict to the global namespace, matching almost
				// nothing. The scope-to context-menu entries set these via the DSL prefixes
				// parsed above.
				InNamespace = inNamespace!,
				InAssembly = inAssembly!,
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
							// Defensive: by the time this Post body runs, the run that
							// produced the batch may already have been cancelled (user kept
							// typing) and the sink Cleared and re-used by a fresh run.
							// Without this check, stale results would leak into the new
							// search's visible list.
							if (ct.IsCancellationRequested)
								return;
							foreach (var r in batch)
							{
								if (sink.Count >= MaxResults)
									break;
								sink.Add(r);
							}
						});
					}
					if (runTask is { IsCompleted: true } && queue.IsEmpty)
					{
						RaiseCompletedOnUIThread();
						return;
					}
					await Task.Delay(50, ct).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected when the run is replaced or cleared. Still raise Completed so the
				// pane's IsSearching flag flips off — cancellation is a kind of completion
				// from the VM's perspective.
				RaiseCompletedOnUIThread();
			}
		}

		void RaiseCompletedOnUIThread()
		{
			var handler = Completed;
			if (handler == null)
				return;
			Dispatcher.UIThread.Post(() => handler(this));
		}
	}
}
