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
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Threading;

using AvaloniaEdit.Document;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// A document tab that hosts decompiled output for a single tree node. Re-decompiles when
	/// <see cref="CurrentNode"/> changes; previous in-flight decompilations are cancelled so a
	/// rapid tree-selection sweep doesn't pile up background work.
	/// </summary>
	public sealed partial class DecompilerTabPageModel : ContentPageModel
	{
		CancellationTokenSource? activeCts;
		// The in-flight fire-and-forget decompile, kept so a caller (e.g. teardown / shutdown) can
		// cancel it and await its completion instead of letting it post continuations later.
		Task? pendingDecompile;

		[ObservableProperty]
		private string text = string.Empty;

		// Diagnostics: stamp the first non-empty Text assignment so we can correlate when
		// decompiled output becomes visible against the tree-render path. The check is
		// idempotent — only the first assignment for the lifetime of the process gets marked.
		static int firstTextMarked;
		partial void OnTextChanged(string? oldValue, string newValue)
		{
			if (!string.IsNullOrEmpty(newValue)
				&& System.Threading.Interlocked.Exchange(ref firstTextMarked, 1) == 0)
			{
				ICSharpCode.ILSpy.AppEnv.AppLog.Mark("DecompilerTabPageModel: first non-empty Text set");
			}
		}

		/// <summary>
		/// File extension driving syntax highlighting (e.g. ".cs"). Updated alongside <see cref="Text"/>.
		/// </summary>
		[ObservableProperty]
		private string syntaxExtension = ".cs";

		/// <summary>
		/// Semantic-highlighting spans collected during decompilation; the view feeds this into
		/// AvaloniaEdit's <c>RichTextColorizer</c>.
		/// </summary>
		[ObservableProperty]
		private RichTextModel? highlightingModel;

		/// <summary>
		/// The highlighting spans behind <see cref="HighlightingModel"/>, referencing the live
		/// named colours (the model itself clones them). The view rebuilds the model from these on
		/// a theme switch so the re-coloured palette reaches the already-decompiled output.
		/// </summary>
		public System.Collections.Generic.IReadOnlyList<(int Start, int Length, AvaloniaEdit.Highlighting.HighlightingColor Color)>? HighlightingSpans { get; set; }

		/// <summary>
		/// Multi-line fold ranges collected by the decompiler (member bodies, attribute blocks,
		/// hidden compiler-generated regions, …). Fed to a <c>FoldingManager</c> on the editor.
		/// </summary>
		[ObservableProperty]
		private IReadOnlyList<NewFolding>? foldings;

		/// <summary>
		/// True while a decompilation is in flight. The view shows an indeterminate progress bar
		/// in the header while this is set.
		/// </summary>
		[ObservableProperty]
		private bool isDecompiling;

		/// <summary>
		/// Title displayed inside the wait adorner while a long-running operation runs. Defaults
		/// to the localised "Decompiling…" string; <see cref="RunWithCancellation"/> overrides it
		/// for non-decompile tasks (e.g. "Creating diagram…", "Building CFG…") so the user knows
		/// what's actually running.
		/// </summary>
		[ObservableProperty]
		private string progressTitle = ICSharpCode.ILSpy.Properties.Resources.Decompiling;

		/// <summary>
		/// True while the progress bar is indeterminate. A long-running operation that reports
		/// <see cref="DecompilationProgress"/> with a positive unit count (project export) flips this
		/// off so the bar becomes determinate; an in-place decompile leaves it on.
		/// </summary>
		[ObservableProperty]
		private bool progressIsIndeterminate = true;

		/// <summary>Total units to process (the project's file count) for the determinate bar.</summary>
		[ObservableProperty]
		private double progressMaximum;

		/// <summary>Units completed so far.</summary>
		[ObservableProperty]
		private double progressValue;

		/// <summary>
		/// The unit currently being processed (e.g. the file being written) plus an "N of M" count,
		/// shown under the progress bar.
		/// </summary>
		[ObservableProperty]
		private string? progressStatus;

		/// <summary>
		/// Applies a <see cref="DecompilationProgress"/> report from a long-running operation (wired
		/// via <see cref="Docking.DockWorkspace.RunInNewTabAsync"/>). Marshaled to the UI thread by the
		/// <see cref="System.Progress{T}"/> that produced it.
		/// </summary>
		public void ReportProgress(DecompilationProgress progress)
		{
			if (progress.Title is { Length: > 0 } title)
				ProgressTitle = title;
			if (progress.TotalUnits > 0)
			{
				ProgressIsIndeterminate = false;
				ProgressMaximum = progress.TotalUnits;
				ProgressValue = progress.UnitsCompleted;
				ProgressStatus = progress.Status is { Length: > 0 } status
					? $"{status} ({progress.UnitsCompleted} of {progress.TotalUnits})"
					: $"{progress.UnitsCompleted} of {progress.TotalUnits}";
			}
			else
			{
				ProgressIsIndeterminate = true;
				ProgressStatus = progress.Status;
			}
		}

		void ResetProgress()
		{
			ProgressIsIndeterminate = true;
			ProgressMaximum = 0;
			ProgressValue = 0;
			ProgressStatus = null;
		}

		/// <summary>
		/// Hyperlink targets emitted alongside the decompiled text. Cleared between decompiles.
		/// </summary>
		[ObservableProperty]
		private TextSegmentCollection<ReferenceSegment>? references;

		/// <summary>
		/// Maps reference targets to their definition offsets in <see cref="Text"/>; used to
		/// jump within the same document.
		/// </summary>
		[ObservableProperty]
		private DefinitionLookup? definitionLookup;

		/// <summary>
		/// Inline UI elements (<see cref="ISmartTextOutput.AddUIElement"/>), in offset order.
		/// Fed to <see cref="UIElementGenerator"/> by the text view.
		/// </summary>
		[ObservableProperty]
		private IReadOnlyList<KeyValuePair<int, Func<Control>>>? uIElements;

		/// <summary>
		/// Custom <see cref="VisualLineElementGenerator"/>s the writer attached (e.g. a
		/// regex-based hyperlink generator). The text view installs them alongside the
		/// document and removes them again when the document changes.
		/// </summary>
		[ObservableProperty]
		private IReadOnlyList<VisualLineElementGenerator>? customElementGenerators;

		/// <summary>
		/// Reference whose every match in <see cref="Text"/> should get the local-reference
		/// background mark applied after the next decompile completes. Set by the analyzer
		/// pane when the user activates a result row: the row's parent-chain analysed entity
		/// goes here so the user can see which call site in the decompiled output corresponds
		/// to their result selection. The decompiler text view watches this property and
		/// applies the highlight both on assignment and after each fresh decompile, so the
		/// mark survives the async gap between setting it and the new <see cref="Text"/>
		/// landing.
		/// </summary>
		[ObservableProperty]
		private object? highlightedReference;

		/// <summary>
		/// Pulls the editor's current view state (caret, scroll, expanded foldings) on demand.
		/// Set by the text view when it binds to this model; invoked by
		/// <see cref="Docking.DockWorkspace"/> when it records a navigation away from this tab.
		/// Null until a view is attached. Reading on demand -- rather than mirroring every caret /
		/// scroll event onto the model -- avoids capturing the programmatic caret-to-end that
		/// AvaloniaEdit performs when the document text is replaced.
		/// </summary>
		public System.Func<DecompilerTextViewState>? CaptureViewState { get; set; }

		/// <summary>
		/// View state to restore on the next document apply. Set by <see cref="Docking.DockWorkspace"/>
		/// when Back/Forward/GoTo lands on an entry that carries a captured state; consumed (and
		/// cleared) by the text view in its <c>ApplyDocument</c> path so the value survives the
		/// async gap between the navigation firing and the decompile finishing.
		/// </summary>
		public DecompilerTextViewState? PendingViewState { get; set; }

		/// <summary>
		/// Fired when the user clicks a cross-document reference. The host (DockWorkspace)
		/// resolves the target on the assembly tree side.
		/// </summary>
		public event System.Action<ReferenceSegment>? NavigateRequested;

		internal void RaiseNavigateRequested(ReferenceSegment segment)
			=> NavigateRequested?.Invoke(segment);

		/// <summary>
		/// Fired when the user activates an AvaloniaEdit hyperlink. Subscribers return
		/// <see langword="true"/> if they handled the URI; the text view then suppresses the
		/// default <see cref="System.Diagnostics.Process.Start(System.Diagnostics.ProcessStartInfo)"/>
		/// fallback that AvaloniaEdit would otherwise run for the URI.
		/// </summary>
		public event System.Func<System.Uri, bool>? OpenUriRequested;

		internal bool RaiseOpenUriRequested(System.Uri uri)
		{
			var handlers = OpenUriRequested;
			if (handlers == null)
				return false;
			foreach (System.Func<System.Uri, bool> handler in handlers.GetInvocationList())
			{
				if (handler(uri))
					return true;
			}
			return false;
		}

		IReadOnlyList<ILSpyTreeNode> currentNodes = System.Array.Empty<ILSpyTreeNode>();

		// Snapshot of the tab title derived from currentNodes' Text values, taken on the
		// UI thread at the moment CurrentNodes is set (and refreshed when a node raises
		// PropertyChanged(Text)). Spinner ticks, post-decompile InvokeAsync, and the
		// cancel-cleanup path read this cached string instead of re-walking node.Text ->
		// LoadedAssembly.Text -> metadata, which AVs when an assembly was unloaded
		// between selection and the title update.
		string cachedBaseTitle = "(unnamed)";

		// IsStaticContent is inherited from ContentPageModel: true for static pages (e.g. About)
		// excludes this tab from the "current decompile target" lookup, so later tree-node
		// clicks reuse a different tab and leave the static content intact.

		[RelayCommand]
		void CancelDecompilation()
		{
			activeCts?.Cancel();
		}

		/// <summary>
		/// Single-selection convenience wrapper over <see cref="CurrentNodes"/> — get returns
		/// the first node (or null), set replaces the list with the single node.
		/// </summary>
		public ILSpyTreeNode? CurrentNode {
			get => currentNodes.Count > 0 ? currentNodes[0] : null;
			set => CurrentNodes = value == null
				? System.Array.Empty<ILSpyTreeNode>()
				: new[] { value };
		}

		/// <summary>
		/// All tree nodes whose decompiled output appears in this tab. Setting fires a fresh
		/// <see cref="DecompileAsync"/> that iterates each node and writes its output, blank
		/// line in between. The first node drives the tab title.
		/// </summary>
		public IReadOnlyList<ILSpyTreeNode> CurrentNodes {
			get => currentNodes;
			set {
				ArgumentNullException.ThrowIfNull(value);
				if (currentNodes.SequenceEqual(value))
					return;
				foreach (var n in currentNodes)
					n.PropertyChanged -= OnCurrentNodePropertyChanged;
				currentNodes = value.ToArray();
				cachedBaseTitle = ComposeBaseTitle();
				foreach (var n in currentNodes)
					n.PropertyChanged += OnCurrentNodePropertyChanged;
				// Let host chrome (the omnibar breadcrumb) react to the tab re-targeting a node.
				// Nothing else relied on the absence of this notification.
				OnPropertyChanged(nameof(CurrentNodes));
				OnPropertyChanged(nameof(CurrentNode));
				StartDecompile();
			}
		}

		void OnCurrentNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Tree-node Text can change after we capture the title (e.g. AssemblyTreeNode swaps
			// from ShortName to "ShortName (version, tfm)" once the assembly finishes loading).
			// While a decompile is running the spinner prefixes the title — keep the prefix and
			// just refresh the suffix; otherwise replace the title outright.
			if (e.PropertyName != nameof(ILSpyTreeNode.Text))
				return;
			cachedBaseTitle = ComposeBaseTitle();
			Title = IsDecompiling ? ComposeSpinnerTitle(0, cachedBaseTitle) : cachedBaseTitle;
		}

		string ComposeBaseTitle()
		{
			if (currentNodes.Count == 0)
				return "(unnamed)";
			return string.Join(", ", currentNodes.Select(n => n.Text?.ToString() ?? "(unnamed)"));
		}

		// Resolved lazily so unit tests / design-time previews that bypass the composition host
		// still construct cleanly.
		TaskbarProgressService? taskbarProgress;
		TaskbarProgressService? TaskbarProgress
			=> taskbarProgress ??= AppEnv.AppComposition.TryGetExport<TaskbarProgressService>();

		public DecompilerTabPageModel()
		{
			Title = "Empty";
		}

		public Language Language { get; set; } = null!;

		// Per-run overrides consumed by DecompileAsync. Reset to defaults at the start of
		// every run; set non-default by RestartDecompileWithStepLimit before kicking off a
		// debug-stepper decompile. Set on the UI thread only — no inter-thread access.
		int pendingStepLimit = int.MaxValue;
		bool pendingIsDebug;

		/// <summary>
		/// Output-length safety limits (characters): a decompile that produces more than the active
		/// limit is stopped and replaced with a "too much code" message rather than hanging/OOMing the
		/// UI. The user can re-run at the extended limit or save to disk. Mirrors the previous version.
		/// </summary>
		public const int DefaultOutputLengthLimit = 5_000_000;
		public const int ExtendedOutputLengthLimit = 75_000_000;
		int pendingOutputLengthLimit = DefaultOutputLengthLimit;

		/// <summary>
		/// Force a fresh decompile of the same nodes with the IL-transform pipeline halted
		/// after <paramref name="stepLimit"/> steps. Used by the Debug Steps pane to render
		/// the partial IL after a chosen step (or full pipeline when <paramref name="stepLimit"/>
		/// is <see cref="int.MaxValue"/>). <paramref name="isDebug"/> toggles the transforms'
		/// verbose-debug emission. No-op when there's nothing currently being decompiled.
		/// </summary>
		public void RestartDecompileWithStepLimit(int stepLimit, bool isDebug)
		{
			pendingStepLimit = stepLimit;
			pendingIsDebug = isDebug;
			StartDecompile();
		}

		/// <summary>Re-runs the current decompile with a larger output-length limit (the "Display code
		/// anyway" button on the too-much-code message).</summary>
		public void RestartDecompileWithOutputLimit(int outputLengthLimit)
		{
			pendingOutputLengthLimit = outputLengthLimit;
			StartDecompile();
		}

		/// <summary>
		/// Forces a fresh decompilation of the current nodes, bypassing the <see cref="CurrentNodes"/>
		/// dedup. Used by <c>ForceRefreshActiveTab</c> when a display setting or freshly resolved
		/// dependencies changed but the selected node did not. No-op when nothing is being decompiled.
		/// </summary>
		public void Redecompile()
		{
			if (CurrentNodes.Count > 0)
				StartDecompile();
		}

		// Built (off the UI thread) when a decompile blows past its output limit: a short message plus
		// a "Display code anyway" button (retry at the extended limit, only offered for the normal
		// limit) and a "Save code" button. The button click handlers run back on the UI thread.
		AvaloniaEditTextOutput BuildOutputLengthExceededMessage(int outputLengthLimit)
		{
			var output = new AvaloniaEditTextOutput();
			bool wasNormalLimit = outputLengthLimit == DefaultOutputLengthLimit;
			output.WriteLine(wasNormalLimit
				? "You have selected too much code for it to be displayed automatically."
				: "You have selected too much code; it cannot be displayed here.");
			output.WriteLine();
			if (wasNormalLimit)
			{
				output.AddButton(Images.ViewCode, ICSharpCode.ILSpy.Properties.Resources.DisplayCode,
					(_, _) => RestartDecompileWithOutputLimit(ExtendedOutputLengthLimit));
				output.WriteLine();
			}
			output.AddButton(Images.Save, ICSharpCode.ILSpy.Properties.Resources.SaveCode,
				(_, _) => SaveCurrentNode());
			output.WriteLine();
			return output;
		}

		void SaveCurrentNode()
		{
			if (CurrentNode is not { } node)
				return;
			// Best-effort: no save path available without a composition host (minimal/design-time host).
			var languageService = AppEnv.AppComposition.TryGetExport<Languages.LanguageService>();
			var dockWorkspace = AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>();
			if (languageService is null || dockWorkspace is null)
				return;
			ICSharpCode.ILSpy.Commands.SaveCodeHelper.SaveNodeAsync(node, languageService, dockWorkspace).HandleExceptions();
		}

		// Fire-and-forget wrapper around DecompileAsync that observes the resulting Task.
		// Without this, exceptions raised by the dispatched property setters (e.g. the
		// PropertyChanged subscribers in DecompilerTextView) become UnobservedTaskException
		// faults that the finalizer thread rethrows much later, hiding the originating bug.
		void StartDecompile()
		{
			pendingDecompile = DecompileAsync().ContinueWith(t => {
				if (t.Exception is { } ex)
					System.Diagnostics.Debug.WriteLine($"DecompileAsync faulted: {ex.Flatten()}");
			}, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
		}

		/// <summary>
		/// Cancels any in-flight decompile and returns a task that completes once it has unwound, so
		/// callers can drive the work to quiescence instead of leaving a background continuation to
		/// land later (e.g. on app shutdown, or between headless tests where it would otherwise post
		/// into the next test's freshly-rebuilt composition).
		/// </summary>
		internal Task CancelPendingAsync()
		{
			activeCts?.Cancel();
			return pendingDecompile ?? Task.CompletedTask;
		}

		// Process-wide counter so the AppLog markers can distinguish "first decompile after
		// boot" (carries the JIT / pipeline-init cold cost) from subsequent runs. Incremented
		// at the top of every DecompileAsync invocation including the empty-node short-circuit
		// path — that way a no-op restore still bumps the count and the next real decompile is
		// labelled #2 rather than #1.
		static int decompileInvocationCount;

		async Task DecompileAsync()
		{
			var callNumber = System.Threading.Interlocked.Increment(ref decompileInvocationCount);
			using var _phase = ICSharpCode.ILSpy.AppEnv.AppLog.Phase($"DecompileAsync #{callNumber}");
			activeCts?.Cancel();
			var cts = activeCts = new CancellationTokenSource();
			var nodes = currentNodes;
			var language = Language;
			if (nodes.Count == 0 || language == null)
			{
				// Clear the per-decompile artefacts alongside Text. If we leave Foldings /
				// HighlightingModel / References pointing at the previous decompile's state,
				// the next ApplyDocument fires (via SyntaxExtension or Text setter) tries to
				// install offsets against the empty document and AvaloniaEdit throws
				// "Folding must be within document boundary". This path is hit by
				// DockWorkspace.OnLanguagePropertyChanged's CurrentNode toggle-through-null.
				HighlightingModel = null;
				HighlightingSpans = null;
				Foldings = null;
				References = null;
				DefinitionLookup = null;
				UIElements = null;
				Text = string.Empty;
				IsDecompiling = false;
				return;
			}

			var newSyntaxExtension = language.FileExtension;
			ProgressTitle = ICSharpCode.ILSpy.Properties.Resources.Decompiling;
			IsDecompiling = true;

			// Spinner appears as a glyph prefix on the tab title while the decompile runs;
			// editor state is left untouched so cancellation falls back cleanly.
			Title = ComposeSpinnerTitle(0, cachedBaseTitle);
			TaskbarProgress?.SetState(TaskbarProgressState.Indeterminate);
			_ = RunSpinnerAsync(cts.Token);

			try
			{
				// Pull a fresh clone of the live DecompilerSettings so Options-panel commits
				// reach the next decompile without restart. Cloning isolates this run's
				// settings from concurrent edits in an open Options tab.
				var decompilerSettings = TryGetLiveDecompilerSettings();
				// Snapshot the per-run debug-stepper overrides so the inner closure sees a
				// stable value, and reset the fields to defaults so the NEXT decompile runs
				// at full fidelity unless RestartDecompileWithStepLimit sets them again.
				var stepLimit = pendingStepLimit;
				var isDebug = pendingIsDebug;
				pendingStepLimit = int.MaxValue;
				pendingIsDebug = false;
				var outputLengthLimit = pendingOutputLengthLimit;
				pendingOutputLengthLimit = DefaultOutputLengthLimit;
				AvaloniaEditTextOutput output;
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase($"DecompileAsync #{callNumber}: Task.Run decompile body ({nodes.Count} node(s), language={language.Name})"))
				{
					(output, _) = await Task.Run(() => {
						var output = new AvaloniaEditTextOutput { LengthLimit = outputLengthLimit };
						// decompilerSettings is null only in design-time / minimal test hosts
						// without composition; fall back to defaults there.
						var options = new DecompilationOptions(
							decompilerSettings ?? new ICSharpCode.Decompiler.DecompilerSettings()) {
							CancellationToken = cts.Token,
							StepLimit = stepLimit,
							IsDebug = isDebug,
						};
						try
						{
							for (int i = 0; i < nodes.Count; i++)
							{
								if (cts.Token.IsCancellationRequested)
									break;
								if (i > 0)
									output.WriteLine();
								nodes[i].Decompile(language, output, options);
							}
						}
						catch (OperationCanceledException)
						{
							// expected on cancel — just return whatever we got
						}
						catch (OutputLengthExceededException)
						{
							// The decompile produced more text than the limit allows -- replace it with
							// a message offering to display anyway (extended limit) or save to disk.
							output = BuildOutputLengthExceededMessage(outputLengthLimit);
						}
						catch (Exception ex)
						{
							output.WriteLine();
							output.WriteLine("/* Decompilation failed:");
							output.WriteLine(ex.ToString());
							output.WriteLine("*/");
						}
						return (output, cts.Token);
					}, cts.Token).ConfigureAwait(true);
				}

				if (cts.Token.IsCancellationRequested)
					return;

				string rendered;
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase($"DecompileAsync #{callNumber}: collect output (GetText + collateral)"))
					rendered = output.GetText();
				// Resource nodes (XML/XAML/…) override the highlighter so their content reads as
				// the embedded format, not as the active language.
				var effectiveSyntaxExtension = output.SyntaxExtensionOverride ?? newSyntaxExtension;
				ICSharpCode.ILSpy.AppEnv.AppLog.Mark($"DecompileAsync #{callNumber}: {rendered.Length} chars, {(output.Foldings?.Count ?? 0)} foldings, {(output.References?.Count ?? 0)} refs");
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase($"DecompileAsync #{callNumber}: Dispatcher.InvokeAsync (apply Text + props, triggers ApplyDocument)"))
					await Dispatcher.UIThread.InvokeAsync(() => {
						Title = cachedBaseTitle;
						ApplyOutput(output, effectiveSyntaxExtension, rendered);
					});
			}
			catch (OperationCanceledException)
			{
				// stale request — drop silently
			}
			finally
			{
				// Hide the wait adorner whether we finished, were cancelled, or aborted: a stale
				// "Decompiling…" overlay is far worse than leaving the previous output visible.
				// Skip the reset if a newer request has already taken over (activeCts is rotated
				// at the top of DecompileAsync).
				if (ReferenceEquals(activeCts, cts))
				{
					void StopSpinner()
					{
						IsDecompiling = false;
						// If we cancelled before producing fresh output, drop the spinner glyph
						// from the title — the editor still shows the previous decompile.
						if (currentNodes.Count > 0)
							Title = cachedBaseTitle;
						TaskbarProgress?.SetState(TaskbarProgressState.None);
					}
					if (Dispatcher.UIThread.CheckAccess())
						StopSpinner();
					else
						await Dispatcher.UIThread.InvokeAsync(StopSpinner);
				}
			}
		}

		// Braille round-spinner: the dot pattern walks around the cell so each frame reads as
		// a different orientation of a small spinning circle.
		static readonly char[] SpinnerFrames = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];
		static readonly TimeSpan SpinnerInterval = TimeSpan.FromMilliseconds(80);

		static string ComposeSpinnerTitle(int frame, string baseTitle)
			=> $"{SpinnerFrames[frame % SpinnerFrames.Length]} {baseTitle}";

		async Task RunSpinnerAsync(CancellationToken token)
		{
			int frame = 1;
			while (!token.IsCancellationRequested)
			{
				try
				{
					await Task.Delay(SpinnerInterval, token).ConfigureAwait(true);
				}
				catch (OperationCanceledException)
				{
					return;
				}
				if (token.IsCancellationRequested || !IsDecompiling)
					return;
				Title = ComposeSpinnerTitle(frame++, cachedBaseTitle);
			}
		}

		/// <summary>
		/// Runs <paramref name="taskCreation"/> with the wait adorner visible and a custom
		/// <paramref name="progressTitle"/> (defaults to "Decompiling…"). Any in-flight
		/// decompile or prior <see cref="RunWithCancellation"/> task is cancelled first so the
		/// wait UI represents exactly the latest request. The returned task completes with the
		/// caller's value once the user-supplied task finishes, faults, or cancels — and the
		/// wait adorner is restored either way.
		/// </summary>
		public async Task<T> RunWithCancellation<T>(
			Func<CancellationToken, Task<T>> taskCreation,
			string? progressTitle = null)
		{
			ArgumentNullException.ThrowIfNull(taskCreation);
			activeCts?.Cancel();
			var cts = activeCts = new CancellationTokenSource();
			ProgressTitle = progressTitle ?? ICSharpCode.ILSpy.Properties.Resources.Decompiling;
			ResetProgress();
			IsDecompiling = true;
			TaskbarProgress?.SetState(TaskbarProgressState.Indeterminate);
			// Animate the braille spinner over the tab's own title (the operation name) so the tab
			// strip advertises the running work, exactly like an in-place decompile does.
			cachedBaseTitle = Title;
			Title = ComposeSpinnerTitle(0, cachedBaseTitle);
			_ = RunSpinnerAsync(cts.Token);
			try
			{
				return await taskCreation(cts.Token).ConfigureAwait(true);
			}
			finally
			{
				if (ReferenceEquals(activeCts, cts))
				{
					void StopSpinner()
					{
						IsDecompiling = false;
						Title = cachedBaseTitle;
						ResetProgress();
						TaskbarProgress?.SetState(TaskbarProgressState.None);
						ProgressTitle = ICSharpCode.ILSpy.Properties.Resources.Decompiling;
					}
					if (Dispatcher.UIThread.CheckAccess())
						StopSpinner();
					else
						await Dispatcher.UIThread.InvokeAsync(StopSpinner);
				}
			}
		}

		/// <summary>
		/// Pushes <paramref name="output"/> directly into the editor — bypasses
		/// <see cref="DecompileAsync"/>. Cancels any in-flight decompile so the new content
		/// isn't overwritten when the cancelled task's continuation lands. Used for
		/// command-driven reports (Create Diagram, Generate PDB, etc.) that produce text
		/// without going through a tree-node selection.
		/// </summary>
		public void ShowText(AvaloniaEditTextOutput output)
		{
			ArgumentNullException.ThrowIfNull(output);
			activeCts?.Cancel();
			activeCts = null;
			ApplyOutput(output,
				output.SyntaxExtensionOverride ?? Language?.FileExtension ?? string.Empty,
				output.GetText());
			currentNodes = System.Array.Empty<ILSpyTreeNode>();
			Title = string.IsNullOrEmpty(output.Title) ? "(report)" : output.Title;
		}

		// Push a finished output's text and its collateral (semantic highlighting, foldings,
		// references, definition lookup, embedded UI elements) onto this page. Text is assigned LAST
		// because setting it triggers the view's ApplyDocument, which reads the other properties --
		// they must already be in place. The syntax extension and text are passed in so the decompile
		// path can compute the (possibly expensive) GetText off the UI thread; everything else is read
		// straight off the now-immutable output. Shared by ShowText and the decompile apply step.
		void ApplyOutput(AvaloniaEditTextOutput output, string syntaxExtension, string text)
		{
			SyntaxExtension = syntaxExtension;
			HighlightingModel = output.HighlightingModel;
			HighlightingSpans = output.HighlightingSpans;
			Foldings = output.Foldings;
			References = output.References;
			DefinitionLookup = output.DefinitionLookup;
			UIElements = output.UIElements;
			Text = text;
		}

		// Pulls the effective DecompilerSettings (clone + Display options + toolbar language
		// version) for this run. Resolves via TryGetExport so design-time / minimal test hosts
		// that bypass composition fall back to default settings rather than throwing.
		static ICSharpCode.Decompiler.DecompilerSettings? TryGetLiveDecompilerSettings()
			=> AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings();
	}
}
