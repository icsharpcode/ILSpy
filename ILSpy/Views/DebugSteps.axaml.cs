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

using System;
using System.ComponentModel;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using ICSharpCode.Decompiler.IL.Transforms;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.Languages;
using ILSpy.TextView;
using ILSpy.Util;
using ILSpy.ViewModels;

namespace ILSpy.Views
{
	public partial class DebugSteps : UserControl
	{
		ILAstLanguage? activeLanguage;
		LanguageService? languageService;
		int lastSelectedStep = int.MaxValue;

		public DebugSteps()
		{
			InitializeComponent();

			MessageBus<AssemblyTreeSelectionChangedEventArgs>.Subscribers += OnSelectionChanged;
			DebugStepsPaneModel.WritingOptions.PropertyChanged += OnWritingOptionsChanged;

			// Language flips go through LanguageService.CurrentLanguage (not through
			// LanguageSettings.LanguageId), so subscribe directly here — the MessageBus path
			// would only cover settings-section changes.
			languageService = TryGetExport<LanguageService>();
			if (languageService != null)
				languageService.PropertyChanged += OnLanguageServiceChanged;

			TryAttachToCurrentLanguage();
		}

		void OnLanguageServiceChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(LanguageService.CurrentLanguage))
				Dispatcher.UIThread.Post(TryAttachToCurrentLanguage);
		}

		void TryAttachToCurrentLanguage()
		{
			var languageService = TryGetExport<LanguageService>();
			if (languageService?.CurrentLanguage is ILAstLanguage il)
				AttachToLanguage(il);
		}

		void AttachToLanguage(ILAstLanguage language)
		{
			DetachFromLanguage();
			activeLanguage = language;
			language.StepperUpdated += OnStepperUpdated;
			RebindStepsTree(language.Stepper);
		}

		void DetachFromLanguage()
		{
			if (activeLanguage != null)
			{
				activeLanguage.StepperUpdated -= OnStepperUpdated;
				activeLanguage = null;
			}
		}

		void OnStepperUpdated(object? sender, EventArgs e)
		{
			Dispatcher.UIThread.Post(() => {
				if (activeLanguage != null)
					RebindStepsTree(activeLanguage.Stepper);
				lastSelectedStep = int.MaxValue;
			});
		}

		void RebindStepsTree(Stepper stepper)
		{
			StepsTree.ItemsSource = stepper.Steps;
		}

		void OnSelectionChanged(object? sender, AssemblyTreeSelectionChangedEventArgs e)
		{
			// When the user picks a new tree node, the previous run's stepper is stale —
			// blank the tree until the next decompile populates it.
			Dispatcher.UIThread.Post(() => {
				StepsTree.ItemsSource = null;
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

		void OnShowStateBefore(object? sender, RoutedEventArgs e)
		{
			if (StepsTree.SelectedItem is Stepper.Node node)
				RequestRedecompile(node.BeginStep, isDebug: false);
		}

		void OnShowStateAfter(object? sender, RoutedEventArgs e)
		{
			if (StepsTree.SelectedItem is Stepper.Node node)
				RequestRedecompile(node.EndStep, isDebug: false);
		}

		void OnDebugStep(object? sender, RoutedEventArgs e)
		{
			if (StepsTree.SelectedItem is Stepper.Node node)
				RequestRedecompile(node.BeginStep, isDebug: true);
		}

		void OnStepsTreeKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Return)
				return;
			if ((e.KeyModifiers & KeyModifiers.Shift) != 0)
				OnShowStateBefore(sender, e);
			else
				OnShowStateAfter(sender, e);
			e.Handled = true;
		}

		void RequestRedecompile(int stepLimit, bool isDebug)
		{
			lastSelectedStep = stepLimit;
			var activeTab = TryGetExport<DockWorkspace>()?.ActiveDecompilerTab;
			activeTab?.RestartDecompileWithStepLimit(stepLimit, isDebug);
		}

		static T? TryGetExport<T>() where T : class
		{
			try
			{ return AppComposition.Current.GetExport<T>(); }
			catch
			{ return null; }
		}
	}
}

#endif
