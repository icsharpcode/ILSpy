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
		ILAstLanguage language;

		DebugSteps()
		{
			InitializeComponent();

			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged += FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged += SelectionChanged;

			if (MainWindow.Instance.CurrentLanguage is ILAstLanguage l) {
				l.StepperUpdated += ILAstStepperUpdated;
				language = l;
				ILAstStepperUpdated(null, null);
			}
		}

		private void SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Dispatcher.Invoke(() => tree.ItemsSource = null);
		}

		private void FilterSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
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
		}

		private void ILAstStepperUpdated(object sender, EventArgs e)
		{
			if (language == null) return;
			Dispatcher.Invoke(() => tree.ItemsSource = language.Stepper.Steps);
		}

		public static void Show()
		{
			MainWindow.Instance.ShowInTopPane("Debug Steps", new DebugSteps());
		}

		void IPane.Closed()
		{
			MainWindow.Instance.SessionSettings.FilterSettings.PropertyChanged -= FilterSettings_PropertyChanged;
			MainWindow.Instance.SelectionChanged -= SelectionChanged;
			if (language != null) {
				language.StepperUpdated -= ILAstStepperUpdated;
			}
		}

		private void tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Stepper.Node n = (Stepper.Node)tree.SelectedItem;
			if (n == null) return;
			var window = MainWindow.Instance;
			window.TextView.DecompileAsync(window.CurrentLanguage, window.SelectedNodes, new DecompilationOptions() { StepLimit = n.Step + 1 });
		}
	}
}
