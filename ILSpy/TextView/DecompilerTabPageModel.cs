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

using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.TextView
{
	/// <summary>
	/// A document tab that hosts decompiled output for a single tree node. Re-decompiles when
	/// <see cref="CurrentNode"/> changes; previous in-flight decompilations are cancelled so a
	/// rapid tree-selection sweep doesn't pile up background work.
	/// </summary>
	public sealed partial class DecompilerTabPageModel : TabPageModel
	{
		CancellationTokenSource? activeCts;

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
				ILSpy.AppEnv.StartupLog.Mark("DecompilerTabPageModel: first non-empty Text set");
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
		private IReadOnlyList<KeyValuePair<int, Lazy<Control>>>? uIElements;

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

		/// <summary>
		/// True for tabs whose content is a static page (e.g. About) rather than the result
		/// of decompiling a tree-node selection. Static tabs are excluded from the lookup
		/// that resolves "the current decompile target", so subsequent tree-node clicks open
		/// or reuse a different tab and leave the static content intact.
		/// </summary>
		public bool IsStaticContent { get; set; }

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
				foreach (var n in currentNodes)
					n.PropertyChanged += OnCurrentNodePropertyChanged;
				_ = DecompileAsync();
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
			var baseTitle = ComposeBaseTitle();
			Title = IsDecompiling ? ComposeSpinnerTitle(0, baseTitle) : baseTitle;
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
			=> taskbarProgress ??= TryGetExport<TaskbarProgressService>();

		static T? TryGetExport<T>() where T : class
		{
			try
			{ return AppEnv.AppComposition.Current.GetExport<T>(); }
			catch { return null; }
		}

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
			_ = DecompileAsync();
		}

		async Task DecompileAsync()
		{
			activeCts?.Cancel();
			var cts = activeCts = new CancellationTokenSource();
			var nodes = currentNodes;
			var language = Language;
			if (nodes.Count == 0 || language == null)
			{
				Text = string.Empty;
				IsDecompiling = false;
				return;
			}

			var newSyntaxExtension = language.FileExtension;
			IsDecompiling = true;

			// Spinner appears as a glyph prefix on the tab title while the decompile runs;
			// editor state is left untouched so cancellation falls back cleanly.
			Title = ComposeSpinnerTitle(0, ComposeBaseTitle());
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
				var (output, _) = await Task.Run(() => {
					var output = new AvaloniaEditTextOutput();
					var options = decompilerSettings != null
						? new DecompilationOptions(decompilerSettings) {
							CancellationToken = cts.Token,
							StepLimit = stepLimit,
							IsDebug = isDebug,
						}
						: new DecompilationOptions {
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
					catch (Exception ex)
					{
						output.WriteLine();
						output.WriteLine("/* Decompilation failed:");
						output.WriteLine(ex.ToString());
						output.WriteLine("*/");
					}
					return (output, cts.Token);
				}, cts.Token).ConfigureAwait(true);

				if (cts.Token.IsCancellationRequested)
					return;

				var rendered = output.GetText();
				var model = output.HighlightingModel;
				var collectedFoldings = output.Foldings;
				var collectedReferences = output.References;
				var collectedLookup = output.DefinitionLookup;
				var collectedUIElements = output.UIElements;
				// Resource nodes (XML/XAML/…) override the highlighter so their content reads as
				// the embedded format, not as the active language.
				var effectiveSyntaxExtension = output.SyntaxExtensionOverride ?? newSyntaxExtension;
				await Dispatcher.UIThread.InvokeAsync(() => {
					// Re-read Text now (instead of capturing it before decompile started) — for
					// freshly-opened assemblies, Text only has the rich "(version, tfm)" form
					// after the load completes during decompile.
					Title = ComposeBaseTitle();
					SyntaxExtension = effectiveSyntaxExtension;
					HighlightingModel = model;
					Foldings = collectedFoldings;
					References = collectedReferences;
					DefinitionLookup = collectedLookup;
					UIElements = collectedUIElements;
					Text = rendered;
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
							Title = ComposeBaseTitle();
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
				Title = ComposeSpinnerTitle(frame++, ComposeBaseTitle());
			}
		}

		// Pulls the live DecompilerSettings via MEF and returns a clone for this run. MEF
		// resolve is wrapped in try/catch so design-time / minimal test hosts that bypass
		// composition fall back to default settings rather than throwing.
		static ICSharpCode.Decompiler.DecompilerSettings? TryGetLiveDecompilerSettings()
		{
			try
			{
				return AppEnv.AppComposition.Current.GetExport<SettingsService>().DecompilerSettings.Clone();
			}
			catch
			{
				return null;
			}
		}
	}
}
