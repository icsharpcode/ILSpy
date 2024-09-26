﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Wpf.Composition.Mef;

namespace ICSharpCode.ILSpy
{
	[DataTemplate(typeof(DebugStepsPaneModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class DebugSteps : UserControl
	{
		static readonly ILAstWritingOptions writingOptions = new ILAstWritingOptions {
			UseFieldSugar = true,
			UseLogicOperationSugar = true
		};

		public static ILAstWritingOptions Options => writingOptions;

#if DEBUG
		ILAstLanguage language;
#endif
		public DebugSteps()
		{
			InitializeComponent();

#if DEBUG
			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_PropertyChanged(sender, e);
			MessageBus<AssemblyTreeSelectionChangedEventArgs>.Subscribers += SelectionChanged;

			writingOptions.PropertyChanged += WritingOptions_PropertyChanged;

			if (SettingsService.Instance.SessionSettings.LanguageSettings.Language is ILAstLanguage l)
			{
				l.StepperUpdated += ILAstStepperUpdated;
				language = l;
				ILAstStepperUpdated(null, null);
			}
#endif
		}

		private void WritingOptions_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			DecompileAsync(lastSelectedStep);
		}

		private void SelectionChanged(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() => {
				tree.ItemsSource = null;
				lastSelectedStep = int.MaxValue;
			});
		}

		private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
#if DEBUG
			if (sender is not LanguageSettings)
				return;

			if (e.PropertyName == nameof(LanguageSettings.Language))
			{
				if (language != null)
				{
					language.StepperUpdated -= ILAstStepperUpdated;
				}
				if (SettingsService.Instance.SessionSettings.LanguageSettings.Language is ILAstLanguage l)
				{
					l.StepperUpdated += ILAstStepperUpdated;
					language = l;
					ILAstStepperUpdated(null, null);
				}
			}
#endif
		}

		private void ILAstStepperUpdated(object sender, EventArgs e)
		{
#if DEBUG
			if (language == null)
				return;
			Dispatcher.Invoke(() => {
				tree.ItemsSource = language.Stepper.Steps;
				lastSelectedStep = int.MaxValue;
			});
#endif
		}

		private void ShowStateAfter_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null)
				return;
			DecompileAsync(n.EndStep);
		}

		private void ShowStateBefore_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null)
				return;
			DecompileAsync(n.BeginStep);
		}

		private void DebugStep_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null)
				return;
			DecompileAsync(n.BeginStep, true);
		}

		int lastSelectedStep = int.MaxValue;

		void DecompileAsync(int step, bool isDebug = false)
		{
			lastSelectedStep = step;
			var window = MainWindow.Instance;
			var state = DockWorkspace.Instance.ActiveTabPage.GetState();
			DockWorkspace.Instance.ActiveTabPage.ShowTextViewAsync(textView => textView.DecompileAsync(window.AssemblyTreeModel.CurrentLanguage, window.AssemblyTreeModel.SelectedNodes,
				new DecompilationOptions(window.AssemblyTreeModel.CurrentLanguageVersion, SettingsService.Instance.DecompilerSettings, SettingsService.Instance.DisplaySettings) {
					StepLimit = step,
					IsDebug = isDebug,
					TextViewState = state as TextView.DecompilerTextViewState
				}));
		}

		private void tree_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return)
			{
				if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
					ShowStateBefore_Click(sender, e);
				else
					ShowStateAfter_Click(sender, e);
				e.Handled = true;
			}
		}
	}
}