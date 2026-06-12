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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Extensions;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Search
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
		readonly IComparer<SearchResult> sortComparer;
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
			ObservableCollection<SearchResult> sink,
			IComparer<SearchResult> sortComparer)
		{
			this.assemblies = assemblies;
			this.searchTerm = searchTerm;
			this.mode = mode;
			this.language = language;
			this.apiVisibility = apiVisibility;
			this.resultFactory = resultFactory;
			this.sink = sink;
			this.sortComparer = sortComparer;
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
				// Sorted insert against the run's captured comparer. The Display-Settings
				// "Sort results by fitness" checkbox picks between ComparerByFitness (the
				// default — shorter names rise to the top) and ComparerByName (ordinal asc).
				// ObservableCollection<T> implements IList<T>, so the InsertSorted extension
				// binary-searches and Inserts at the right index — O(log n) compare + O(n) shift.
				sink.InsertSorted(result, sortComparer);
			}

			if (sink.Count >= MaxResults)
			{
				if (!capReached)
				{
					capReached = true;
					// Stop the producer — anything it pushes from here would just be
					// discarded, so why burn CPU on it.
					cts.Cancel();
					// Tell the user the list was truncated (appended after the sorted results; its
					// null Reference makes it a no-op to click).
					sink.Add(new SearchResult {
						Name = ICSharpCode.ILSpy.Properties.Resources.SearchAbortedMoreThan1000ResultsFound,
						Location = string.Empty,
						Assembly = string.Empty,
						Image = null!,
						LocationImage = null!,
						AssemblyImage = null!,
					});
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

		internal SearchRequest BuildRequest()
		{
			var request = ParseInput(searchTerm ?? string.Empty, mode);
			request.SearchResultFactory = resultFactory;
			// The search strategies resolve a type system via
			// module.GetTypeSystemWithDecompilerSettingsOrNull(request.DecompilerSettings),
			// which caches ONE type system per options value on the LoadedAssembly. Derive the
			// settings from the user's current options so search hits the same cached instance
			// the tree resolves through, instead of materialising a second type system per
			// module (and disagreeing with the tree about settings-dependent entity shapes).
			request.DecompilerSettings = AppEnv.AppComposition.TryGetExport<SettingsService>()
				?.CreateEffectiveDecompilerSettings() ?? new DecompilerSettings();
			return request;
		}

		/// <summary>
		/// Parses the user's freeform query into a <see cref="SearchRequest"/>. Recognises
		/// the prefix DSL surfaced in the watermark:
		/// <list type="bullet">
		/// <item><c>t:</c>, <c>tm:</c>, <c>m:</c>, <c>md:</c>, <c>f:</c>, <c>p:</c>, <c>e:</c>,
		///       <c>c:</c>, <c>n:</c>, <c>r:</c> — set <see cref="SearchRequest.Mode"/>.</item>
		/// <item><c>a:</c>, <c>af:</c>, <c>an:</c> — assembly mode + <see cref="AssemblySearchKind"/>.</item>
		/// <item><c>@</c> (single character) — metadata-token mode.</item>
		/// <item><c>inassembly:</c>, <c>innamespace:</c> — scope filters.</item>
		/// <item><c>/.../</c> — regex match via <see cref="SearchRequest.RegEx"/>.</item>
		/// <item>Double-quoted tokens — survive whitespace split, quotes stripped from the term.</item>
		/// </list>
		/// Auto-sets <see cref="SearchRequest.FullNameSearch"/> when any keyword has a dot,
		/// and <see cref="SearchRequest.OmitGenerics"/> when any keyword lacks generic angles
		/// and IL-arity backtick. The match operators <c>=</c> / <c>+</c> / <c>-</c> / <c>~</c>
		/// are NOT consumed here — they live on as keyword prefixes for
		/// <see cref="AbstractSearchStrategy.IsMatch"/> to interpret per term.
		/// </summary>
		internal static SearchRequest ParseInput(string input, SearchMode defaultMode)
		{
			var request = new SearchRequest { Mode = defaultMode };
			var keywords = new List<string>();
			Regex? regex = null;
			foreach (var part in TokenizeQuoted(input))
			{
				string? prefix;
				string term;
				if (part.StartsWith("@", StringComparison.Ordinal))
				{
					prefix = "@";
					term = part.Substring(1);
				}
				else
				{
					// Find the colon that opens a prefix. It has to come before any quote
					// or regex slash — otherwise it's a colon inside the term itself.
					int quoteOrSlash = part.IndexOfAny(new[] { '"', '/' });
					int searchEnd = quoteOrSlash < 0 ? part.Length : quoteOrSlash;
					int colon = part.IndexOf(':', 0, searchEnd);
					if (colon > 0)
					{
						prefix = part.Substring(0, colon);
						term = part.Substring(colon + 1);
					}
					else
					{
						prefix = null;
						term = part;
					}
				}

				// Strip surrounding double quotes from the term, if present.
				if (term.Length >= 2 && term[0] == '"' && term[^1] == '"')
					term = term.Substring(1, term.Length - 2);

				// If the prefix was followed by nothing useful, drop it and treat the whole
				// part as a keyword instead.
				if (prefix != null && term.Length == 0)
				{
					prefix = null;
					term = part;
				}

				// Term goes into keywords/regex regardless of prefix — the prefix only flips
				// mode/scope, the keyword is still what gets matched.
				if (regex == null && term.StartsWith("/", StringComparison.Ordinal) && term.Length > 1)
				{
					// Wrapped /regex/ form. Closing slash is optional; if present, strip it.
					var innerEnd = term.EndsWith("/", StringComparison.Ordinal) && term.Length > 2
						? term.Length - 1
						: term.Length;
					var inner = term.Substring(1, innerEnd - 1);
					if (inner.Contains("\\."))
						request.FullNameSearch = true;
					regex = TryCreateRegex(inner);
				}
				else
				{
					if (term.Contains('.'))
						request.FullNameSearch = true;
					keywords.Add(term);
				}
				if (!(term.Contains('<') || term.Contains('`')))
					request.OmitGenerics = true;
				// Angle brackets are characteristic of compiler-generated names
				// (`<X>d__N`, `<>c__DisplayClass`, etc.) and rare in everyday API
				// names — treat them as the user opting in to private /
				// compiler-generated entities even when the global visibility
				// setting is PublicOnly.
				if (term.Contains('<') || term.Contains('>'))
					request.IncludePrivateApi = true;

				switch (prefix?.ToUpperInvariant())
				{
					case "@":
						request.Mode = SearchMode.Token;
						break;
					case "INNAMESPACE":
						if (request.InNamespace == null)
							request.InNamespace = term;
						break;
					case "INASSEMBLY":
						if (request.InAssembly == null)
							request.InAssembly = term;
						break;
					case "A":
						request.AssemblySearchKind = AssemblySearchKind.NameOrFileName;
						request.Mode = SearchMode.Assembly;
						break;
					case "AF":
						request.AssemblySearchKind = AssemblySearchKind.FilePath;
						request.Mode = SearchMode.Assembly;
						break;
					case "AN":
						request.AssemblySearchKind = AssemblySearchKind.FullName;
						request.Mode = SearchMode.Assembly;
						break;
					case "N":
						request.Mode = SearchMode.Namespace;
						break;
					case "TM":
						request.Mode = SearchMode.TypeAndMember;
						break;
					case "T":
						request.Mode = SearchMode.Type;
						break;
					case "M":
						request.Mode = SearchMode.Member;
						break;
					case "MD":
						request.Mode = SearchMode.Method;
						break;
					case "F":
						request.Mode = SearchMode.Field;
						break;
					case "P":
						request.Mode = SearchMode.Property;
						break;
					case "E":
						request.Mode = SearchMode.Event;
						break;
					case "C":
						request.Mode = SearchMode.Literal;
						break;
					case "R":
						request.Mode = SearchMode.Resource;
						break;
				}
			}
			request.Keywords = keywords.ToArray();
			request.RegEx = regex;
			return request;
		}

		static Regex? TryCreateRegex(string pattern)
		{
			try
			{
				return new Regex(pattern, RegexOptions.Compiled);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		/// <summary>
		/// Splits the input on whitespace, treating double-quoted spans as a single token
		/// (so <c>"hello world"</c> stays together). Quotes inside the token are kept so the
		/// prefix-detection step can still tell a colon-inside-quotes (<c>t:"a:b"</c>) from
		/// a prefix-opening colon.
		/// </summary>
		static IEnumerable<string> TokenizeQuoted(string input)
		{
			var sb = new System.Text.StringBuilder();
			bool inQuotes = false;
			foreach (var c in input)
			{
				if (c == '"')
				{
					inQuotes = !inQuotes;
					sb.Append(c);
				}
				else if (!inQuotes && char.IsWhiteSpace(c))
				{
					if (sb.Length > 0)
					{
						yield return sb.ToString();
						sb.Clear();
					}
				}
				else
				{
					sb.Append(c);
				}
			}
			if (sb.Length > 0)
				yield return sb.ToString();
		}

		AbstractSearchStrategy? GetStrategy(SearchRequest request)
		{
			if (request.Keywords.Length == 0 && request.RegEx is null)
				return null;
			return request.Mode switch {
				SearchMode.TypeAndMember => new MemberSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Type => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Type),
				SearchMode.Member => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Member),
				SearchMode.Method => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Method),
				SearchMode.Field => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Field),
				SearchMode.Property => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Property),
				SearchMode.Event => new MemberSearchStrategy(language, apiVisibility, request, queue, MemberSearchKind.Event),
				SearchMode.Literal => new LiteralSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Token => new MetadataTokenSearchStrategy(language, apiVisibility, request, queue),
				SearchMode.Assembly => new AssemblySearchStrategy(request, queue, request.AssemblySearchKind),
				SearchMode.Namespace => new NamespaceSearchStrategy(request, queue),
				// Resource search needs ITreeNodeFactory infrastructure — deferred to a follow-up.
				_ => null,
			};
		}
	}
}
