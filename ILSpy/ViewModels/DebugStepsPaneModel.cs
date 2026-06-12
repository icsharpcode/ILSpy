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

#if DEBUG

using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Bottom-aligned tool pane that surfaces the step tree from the active
	/// <see cref="Languages.IDebugStepProvider"/> language — ILAst (one step per IL transform) or
	/// C# (one step per AST transform). The ViewModel owns the cross-language / cross-decompile
	/// state (active language, current Stepper.Steps list, per-language options) so it doesn't
	/// matter when the matching View materialises — the View just binds to <see cref="Steps"/>
	/// and lights up whenever the current language is a step provider and a decompile finishes.
	///
	/// Compiled only in Debug builds — Release users don't see the pane or the languages
	/// that populate it.
	/// </summary>
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 1, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class DebugStepsPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "DebugSteps";

		readonly LanguageService? languageService;

		IDebugStepProvider? activeLanguage;
		int lastSelectedStep = int.MaxValue;

		/// <summary>
		/// App-wide ILAst writing options shared between the BlockIL language (which reads
		/// them while emitting the transformed IL) and the DebugSteps view (whose four
		/// checkboxes toggle their values). Static singleton state because the language is
		/// MEF-shared and decompiles on background tasks that have no view-model reference.
		/// </summary>
		public static ILAstWritingOptions WritingOptions { get; } = new() {
			UseFieldSugar = true,
			UseLogicOperationSugar = true,
		};

		/// <summary>
		/// Options controls for the active step-provider language (e.g. ILAst's writing options),
		/// or null when the language has none. The view selects a template by runtime type, so the
		/// options shown swap with the language.
		/// </summary>
		[ObservableProperty]
		object? options;

		/// <summary>
		/// Currently displayed list of recorded transform steps. Re-assigned (not mutated)
		/// whenever the active step provider's <see cref="Stepper"/> reports a new run, so
		/// late-binding views pick up the latest list via the observable change.
		/// </summary>
		[ObservableProperty]
		IList<Stepper.Node>? steps;

		/// <summary>Two-way bound to the TreeView's selected item.</summary>
		[ObservableProperty]
		Stepper.Node? selectedStep;

		/// <summary>
		/// True while the current language is an <see cref="IDebugStepProvider"/>. When false,
		/// the view replaces the step tree with a "not available" note instead of leaving the
		/// previous language's stale tree (whose commands would trigger pointless re-decompiles).
		/// </summary>
		[ObservableProperty]
		bool isAvailable;

		public IRelayCommand ShowStateBeforeCommand { get; }
		public IRelayCommand ShowStateAfterCommand { get; }
		public IRelayCommand DebugStepCommand { get; }

		/// <summary>Design-time / fallback ctor — no dependencies wired.</summary>
		public DebugStepsPaneModel()
		{
			Id = PaneContentId;
			Title = "Debug Steps";
			ShowStateBeforeCommand = new RelayCommand(() => RequestRedecompile(SelectedStep?.BeginStep ?? int.MaxValue, isDebug: false));
			ShowStateAfterCommand = new RelayCommand(() => RequestRedecompile(SelectedStep?.EndStep ?? int.MaxValue, isDebug: false));
			DebugStepCommand = new RelayCommand(() => {
				// "Debug this step" relies on Stepper.Step calling Debugger.Break() when
				// step == StepLimit — which is a silent no-op without a debugger attached.
				// Attempt to attach one first so the gesture actually does something for the
				// common case of running ILSpy from a terminal. On Windows this pops
				// the JIT-debugger prompt; on Linux/macOS Debugger.Launch is a no-op so the
				// break is still silent — log a hint so the user knows why.
				if (!System.Diagnostics.Debugger.IsAttached)
				{
					if (!System.Diagnostics.Debugger.Launch())
						AppEnv.AppLog.Mark("DebugStep: Debugger.Launch returned false; the upcoming Stepper.Step break is a no-op without a debugger attached.");
				}
				RequestRedecompile(SelectedStep?.BeginStep ?? int.MaxValue, isDebug: true);
			});
		}

		[ImportingConstructor]
		public DebugStepsPaneModel(LanguageService languageService) : this()
		{
			this.languageService = languageService;
			// DockWorkspace is resolved lazily inside RequestRedecompile — taking it as an
			// ImportingConstructor argument creates a MEF cycle (DockWorkspace imports
			// ToolPaneRegistry which materialises this VM which would import DockWorkspace).
			// Lazy lookup at command-execution time breaks the cycle.

			// Language flips go through LanguageService.CurrentLanguage; the BlockILLanguage
			// pumps StepperUpdated when its decompile finishes. The selection-changed event
			// is the signal that the user picked a new tree node — clear the step list so
			// the previous run's nodes aren't shown against a fresh selection. All three
			// subscriptions live here in the long-lived VM so the View's lifecycle (lazy /
			// deferred / re-realised) can't lose them.
			languageService.PropertyChanged += OnLanguageServiceChanged;
			MessageBus<AssemblyTreeSelectionChangedEventArgs>.Subscribers += OnSelectionChanged;
			WritingOptions.PropertyChanged += OnWritingOptionsChanged;

			TryAttachToCurrentLanguage();
		}

		void OnLanguageServiceChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LanguageService.CurrentLanguage))
				Dispatcher.UIThread.Post(TryAttachToCurrentLanguage);
		}

		void TryAttachToCurrentLanguage()
		{
			if (languageService?.CurrentLanguage is IDebugStepProvider il)
				AttachToLanguage(il);
			else
				DetachFromLanguage();
		}

		void AttachToLanguage(IDebugStepProvider language)
		{
			if (ReferenceEquals(activeLanguage, language))
			{
				// Same language instance — just refresh the steps in case a decompile happened
				// while we were detached.
				Steps = activeLanguage!.Stepper.Steps;
				IsAvailable = true;
				return;
			}
			DetachFromLanguage();
			activeLanguage = language;
			language.StepperUpdated += OnStepperUpdated;
			Steps = language.Stepper.Steps;
			Options = language.StepOptions;
			IsAvailable = true;
		}

		void DetachFromLanguage()
		{
			IsAvailable = false;
			Steps = null;
			if (activeLanguage != null)
			{
				activeLanguage.StepperUpdated -= OnStepperUpdated;
				activeLanguage = null;
				Options = null;
			}
		}

		void OnStepperUpdated(object? sender, System.EventArgs e)
		{
			Dispatcher.UIThread.Post(() => {
				if (activeLanguage != null)
				{
					Steps = activeLanguage.Stepper.Steps;
					lastSelectedStep = int.MaxValue;
				}
			});
		}

		void OnSelectionChanged(object? sender, AssemblyTreeSelectionChangedEventArgs e)
		{
			// User picked a new tree node — the previous run's stepper is stale until the
			// next decompile populates it.
			Dispatcher.UIThread.Post(() => {
				Steps = null;
				lastSelectedStep = int.MaxValue;
			});
		}

		void OnWritingOptionsChanged(object? sender, PropertyChangedEventArgs e)
		{
			// Toggling a checkbox should refresh the editor view so the user sees the effect
			// immediately. Triggers the same path as picking "Show state after this step" on
			// whatever step was last viewed (defaults to int.MaxValue → full run).
			RequestRedecompile(lastSelectedStep, isDebug: false);
		}

		void RequestRedecompile(int stepLimit, bool isDebug)
		{
			lastSelectedStep = stepLimit;
			// Composition unavailable in design-time previews; the gesture is a no-op there.
			var dock = AppComposition.TryGetExport<DockWorkspace>();
			dock?.ActiveDecompilerTab?.RestartDecompileWithStepLimit(stepLimit, isDebug);
		}
	}
}

#endif
