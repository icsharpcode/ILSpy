using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy
{
	public partial class DebugSteps : UserControl, IPane
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
			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged += FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged += SelectionChanged;
			writingOptions.PropertyChanged += WritingOptions_PropertyChanged;

			if (MainWindow.Instance.CurrentLanguage is ILAstLanguage l) {
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

		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Dispatcher.Invoke(() => {
				tree.ItemsSource = null;
				lastSelectedStep = int.MaxValue;
			});
		}

		private void FilterSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
#if DEBUG
			if (e.PropertyName == "Language") {
				if (language != null) {
					language.StepperUpdated -= ILAstStepperUpdated;
				}
				if (MainWindow.Instance.CurrentLanguage is ILAstLanguage l) {
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
			if (language == null) return;
			Dispatcher.Invoke(() => {
				tree.ItemsSource = language.Stepper.Steps;
				lastSelectedStep = int.MaxValue;
			});
#endif
		}

		public static void Show()
		{
			DockWorkspace.Instance.ToolPanes.Add(DebugStepsPaneModel.Instance);
		}

		void IPane.Closed()
		{
#if DEBUG
			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged -= FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged -= SelectionChanged;
			writingOptions.PropertyChanged -= WritingOptions_PropertyChanged;
			if (language != null) {
				language.StepperUpdated -= ILAstStepperUpdated;
			}
#endif
		}

		private void ShowStateAfter_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null) return;
			DecompileAsync(n.EndStep);
		}

		private void ShowStateBefore_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null) return;
			DecompileAsync(n.BeginStep);
		}

		private void DebugStep_Click(object sender, RoutedEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null) return;
			DecompileAsync(n.BeginStep, true);
		}

		int lastSelectedStep = int.MaxValue;

		void DecompileAsync(int step, bool isDebug = false)
		{
			lastSelectedStep = step;
			var window = MainWindow.Instance;
			var state = window.TextView.GetState();
			window.TextView.DecompileAsync(window.CurrentLanguage, window.SelectedNodes,
				new DecompilationOptions(window.CurrentLanguageVersion) {
					StepLimit = step,
					IsDebug = isDebug,
					TextViewState = state
				});
		}

		private void tree_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Return) {
				if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
					ShowStateBefore_Click(sender, e);
				else
					ShowStateAfter_Click(sender, e);
				e.Handled = true;
			}
		}
	}
}