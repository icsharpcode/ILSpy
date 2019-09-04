using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace ICSharpCode.ILSpy.Docking
{
	public static class DockingHelper
	{
		public static void DockHorizontal(LayoutContent layoutContent, ILayoutElement paneRelativeTo, GridLength dockHeight, bool dockBefore = false)
		{
			if (paneRelativeTo is ILayoutDocumentPane parentDocumentPane) {
				var parentDocumentGroup = paneRelativeTo.FindParent<LayoutDocumentPaneGroup>();
				if (parentDocumentGroup == null) {
					var grandParent = parentDocumentPane.Parent as ILayoutContainer;
					parentDocumentGroup = new LayoutDocumentPaneGroup() { Orientation = System.Windows.Controls.Orientation.Vertical };
					grandParent.ReplaceChild(paneRelativeTo, parentDocumentGroup);
					parentDocumentGroup.Children.Add(parentDocumentPane);
				}
				parentDocumentGroup.Orientation = System.Windows.Controls.Orientation.Vertical;
				int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
				var layoutDocumentPane = new LayoutDocumentPane(layoutContent) { DockHeight = dockHeight };
				parentDocumentGroup.InsertChildAt(dockBefore ? indexOfParentPane : indexOfParentPane + 1, layoutDocumentPane);
				layoutContent.IsActive = true;
				layoutContent.Root.CollectGarbage();
				Application.Current.MainWindow.Dispatcher.Invoke(() => {

					layoutDocumentPane.DockHeight = dockHeight;
				}, System.Windows.Threading.DispatcherPriority.Loaded);
			}
		}
	}
}
