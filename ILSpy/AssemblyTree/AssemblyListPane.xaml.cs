using System.ComponentModel.Composition;
using System.Windows;

using ICSharpCode.ILSpyX.TreeView;

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
			}
			else if (e.Property == SelectedItemProperty)
			{
				if (e.NewValue is not SharpTreeNode treeNode)
					return;

				FocusNode(treeNode);
			}
		}
	}
}
