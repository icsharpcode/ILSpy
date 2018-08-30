using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Interaction logic for MainTabView.xaml
	/// </summary>
	[Export, PartCreationPolicy(CreationPolicy.Shared)]
	public partial class MainTabView : UserControl
	{
		public MainTabViewModel ViewModel => this.DataContext as MainTabViewModel;

		public MainTabView()
		{
			InitializeComponent();
		}
	}
}
