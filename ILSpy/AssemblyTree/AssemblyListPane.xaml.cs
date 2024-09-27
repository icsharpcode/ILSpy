using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;

using TomsToolbox.Wpf.Composition.Mef;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	/// <summary>
	/// Interaction logic for AssemblyListPane.xaml
	/// </summary>
	[DataTemplate(typeof(AssemblyTreeModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class AssemblyListPane
	{
		public AssemblyListPane()
		{
			InitializeComponent();

			ContextMenuProvider.Add(this);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == DataContextProperty)
			{
				if (e.NewValue is not AssemblyTreeModel model)
					return;

				model.SetActiveView(this);

				// If there is already a selected item in the model, we need to scroll it into view, so it can be selected in the UI.
				var selected = model.SelectedItem;
				if (selected != null)
				{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Background, () => {
						ScrollIntoView(selected);
						this.SelectedItem = selected;
					});
				}
			}
		}
	}
}
