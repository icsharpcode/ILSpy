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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Extensions;
using ICSharpCode.ILSpyX.Search;

using ILSpy.Languages;

namespace ILSpy.Search
{
	/// <summary>
	/// Orchestrates one search across the loaded <see cref="AssemblyList"/>. Walks
	/// the assemblies serially on a background <see cref="Task"/>, pushes results into
	/// a thread-safe queue, and the UI thread drains that queue once per render frame
	/// with a wall-clock budget — the same shape as the WPF SearchPane.
	/// </summary>
	internal sealed class RunningSearch
	{
		const int MaxResults = 1000;
		// 60 fps tick. Matches the cadence WPF's CompositionTarget.Rendering gives.
		const int RefreshIntervalMs = 16;
		// Hard cap on time spent inserting per tick. Keeps the UI responsive even
		// when the queue is jammed with thousands of late-arriving hits.
		const int RefreshTimeBudgetMs = 10;

		readonly IReadOnlyList<LoadedAssembly> assemblies;
		readonly SearchMode mode;
		readonly string searchTerm;
		readonly Language language;
		readonly ApiVisibility apiVisibility;
		readonly ISearchResultFactory resultFactory;
		readonly ObservableCollection<SearchResult> sink;
		readonly ConcurrentQueue<SearchResult> queue = new();
		readonly CancellationTokenSource cts = new();
		DispatcherTimer? drainTimer;
		Task? runTask;
		volatile bool externalCancel;
		bool capReached;
		bool completedRaised;

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
		/// Fires once on the UI thread when the run finishes (success, error, cap, or
		/// external cancel). The pane VM uses it to flip its IsSearching flag back.
		/// </summary>
		public event Action<RunningSearch>? Completed;

		public void Start()
		{
			var token = cts.Token;
			runTask = Task.Run(() => RunSearch(token), token);
			// Per-frame UI-thread drain. Render priority puts it alongside layout/render
			// work rather than ahead of input. The timer ticks on the UI thread, so the
			// drain body never needs Dispatcher.UIThread.Post — one less hop per batch.
			drainTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(RefreshIntervalMs),
				DispatcherPriority.Render,
				OnDrainTick);
			drainTimer.Start();
		}

		public void Cancel()
		{
			// externalCancel distinguishes "user typed a new char" from "cap was hit
			// internally". External cancel bails the drain immediately so we don't keep
			// inserting into a sink the VM has handed off to a fresh RunningSearch.
			externalCancel = true;
			cts.Cancel();
			drainTimer?.Stop();
			drainTimer = null;
			RaiseCompletedIfFirst();
		}

		void RunSearch(CancellationToken ct)
		{
			try
			{
				var request = BuildRequest();
				var strategy = GetStrategy(request);
				if (strategy == null)
					return;
				// Serial walk: per-assembly metadata walk is allocation-dominated, and 4
				// parallel producers fighting for the ConcurrentQueue + the resulting UI
				// batching jitter end up slower than serial in practice.
				foreach (var assembly in assemblies)
				{
					if (ct.IsCancellationRequested)
						break;
					MetadataFile? module;
					try
					{
						// Block here — we're already on a worker thread (Task.Run) and
						// this matches the WPF call shape. ConfigureAwait in an async
						// state machine would just add overhead for the same effect.
						module = assembly.GetMetadataFileAsync().GetAwaiter().GetResult();
					}
					catch (OperationCanceledException)
					{
						throw;
					}
					catch
					{
						// Load failure (broken DLL, missing reference, IO problem). Already
						// surfaced in the assembly tree via the AssemblyWarning icon — skip
						// this assembly and continue with the next.
						continue;
					}
					if (module == null)
						continue;
					try
					{
						strategy.Search(module, ct);
					}
					catch (OperationCanceledException)
					{
						throw;
					}
					catch
					{
						// One bad assembly's strategy walker (malformed metadata, missing
						// dependency, internal assert) — keep the run going instead of
						// faulting the whole search.
					}
				}
			}
			catch (OperationCanceledException)
			{
				// Expected on user-driven term/mode change, or when the cap fires.
			}
			catch
			{
				// Run-wide failure (parser, strategy construction). Treat as a no-result run.
			}
		}

		void OnDrainTick(object? sender, EventArgs e)
		{
			// External cancel: the VM has cleared and re-used 'sink' for a new run.
			// Inserting now would contaminate someone else's results — bail.
			if (externalCancel)
			{
				drainTimer?.Stop();
				drainTimer = null;
				return;
			}

			// 10ms wall-clock budget per tick. While there's still budget left and
			// the visible cap hasn't been hit, drain as much of the queue as we can.
			// Stopwatch is high-resolution; Environment.TickCount has ~15ms grain
			// on Windows which is too coarse for a 10ms budget.
			var sw = Stopwatch.StartNew();
			while (sink.Count < MaxResults && sw.ElapsedMilliseconds < RefreshTimeBudgetMs)
			{
				if (!queue.TryDequeue(out var result))
					break;
				// Sorted insert: results land in fitness order so the most relevant hit
				// rises to the top while later, less relevant matches are still streaming
				// in. ObservableCollection<T> implements IList<T>, so the InsertSorted
				// extension binary-searches and Inserts at the right index — O(log n)
				// compare + O(n) shift.
				sink.InsertSorted(result, SearchResult.ComparerByFitness);
			}

			if (sink.Count >= MaxResults)
			{
				if (!capReached)
				{
					capReached = true;
					// Stop the producer — anything it pushes from here would just be
					// discarded, so why burn CPU on it.
					cts.Cancel();
				}
				// Drain-discard any items still queued: without this the termination
				// condition (runTask done + queue empty) never fires and the timer
				// spins forever after the cap is hit.
				while (queue.TryDequeue(out _))
				{ }
			}

			if (runTask is { IsCompleted: true } && queue.IsEmpty)
			{
				drainTimer?.Stop();
				drainTimer = null;
				RaiseCompletedIfFirst();
			}
		}

		void RaiseCompletedIfFirst()
		{
			if (completedRaised)
				return;
			completedRaised = true;
			Completed?.Invoke(this);
		}

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
	}
}
