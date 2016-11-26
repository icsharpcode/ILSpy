using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.ILSpy
{

	/// <summary>
	/// Interaktionslogik für DebugSteps.xaml
	/// </summary>
	public partial class DebugSteps : UserControl, IPane
	{
#if DEBUG
		ILAstLanguage language;
#endif

		DebugSteps()
		{
			InitializeComponent();

#if DEBUG
			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged += FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged += SelectionChanged;

			if (MainWindow.Instance.CurrentLanguage is ILAstLanguage l) {
				l.StepperUpdated += ILAstStepperUpdated;
				language = l;
				ILAstStepperUpdated(null, null);
			}
#endif
		}

		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Dispatcher.Invoke(() => tree.ItemsSource = null);
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
			Dispatcher.Invoke(() => tree.ItemsSource = language.Stepper.Steps);
#endif
		}

		public static void Show()
		{
			MainWindow.Instance.ShowInTopPane("Debug Steps", new DebugSteps());
		}

		void IPane.Closed()
		{
#if DEBUG
			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged -= FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged -= SelectionChanged;
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

		void DecompileAsync(int step, bool isDebug = false)
		{
			var window = MainWindow.Instance;
			var state = window.TextView.GetState();
			window.TextView.DecompileAsync(window.CurrentLanguage, window.SelectedNodes,
				new DecompilationOptions() {
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