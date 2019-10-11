using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace ICSharpCode.ILSpy.Docking
{
	public static class DockingHelper
	{
		public static bool Dock(LayoutRoot root, LayoutContent layoutContent, PanePosition position)
		{
			var documentPane = root.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
			if ((position == PanePosition.Top) || (position == PanePosition.Bottom)) {
				return DockHorizontal(layoutContent, documentPane, new GridLength(200), position == PanePosition.Top);
			} else if ((position == PanePosition.Left) || (position == PanePosition.Right)) {
				return DockVertical(layoutContent as LayoutAnchorable, documentPane, new GridLength(400), position == PanePosition.Left);
			}
			return false;
		}

		public static bool DockHorizontal(LayoutContent layoutContent, ILayoutElement paneRelativeTo, GridLength dockHeight, bool dockBefore = false)
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
				Application.Current.MainWindow.Dispatcher.Invoke(
					() => layoutDocumentPane.DockHeight = dockHeight,
					System.Windows.Threading.DispatcherPriority.Loaded);
				return true;
			}

			return false;
		}

		public static bool DockVertical(LayoutAnchorable anchorable, ILayoutElement paneRelativeTo, GridLength dockWidth, bool dockBefore = false)
		{
			if (paneRelativeTo is ILayoutDocumentPane parentDocumentPane) {
				var grandParent = parentDocumentPane.Parent as LayoutPanel;
				var targetAnchorablePane = grandParent.Children.OfType<LayoutAnchorablePane>().FirstOrDefault();
				if (targetAnchorablePane == null) {
					targetAnchorablePane = new LayoutAnchorablePane() {
						DockWidth = new GridLength(400)
					};
					int targetIndex = dockBefore ? 0 : grandParent.ChildrenCount;
					grandParent.InsertChildAt(targetIndex, targetAnchorablePane);
				}
				grandParent.Orientation = Orientation.Horizontal;
				targetAnchorablePane.Children.Add(anchorable);

				anchorable.IsActive = true;
				anchorable.Root.CollectGarbage();
				Application.Current.MainWindow.Dispatcher.Invoke(
					() => targetAnchorablePane.DockWidth = dockWidth,
					System.Windows.Threading.DispatcherPriority.Loaded);
				return true;
			}

			return false;
		}
	}
}
