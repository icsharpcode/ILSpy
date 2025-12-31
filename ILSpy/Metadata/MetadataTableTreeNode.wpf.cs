using System.Windows.Controls;
using System.Windows.Threading;

namespace ICSharpCode.ILSpy.Metadata
{
    partial class MetadataTableTreeNode
    {

		protected void ScrollRowIntoView(DataGrid view, int row)
		{
			if (!view.IsLoaded)
			{
				view.Loaded += View_Loaded;
			}
			else
			{
				View_Loaded(view, new System.Windows.RoutedEventArgs());
			}
			if (view.Items.Count > row && row >= 0)
				view.Dispatcher.BeginInvoke(() => view.SelectItem(view.Items[row]), DispatcherPriority.Background);
		}

		private void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			DataGrid view = (DataGrid)sender;
			var sv = view.FindVisualChild<ScrollViewer>();
			sv.ScrollToVerticalOffset(scrollTarget - 1);
			view.Loaded -= View_Loaded;
			this.scrollTarget = default;
		}
    }
}