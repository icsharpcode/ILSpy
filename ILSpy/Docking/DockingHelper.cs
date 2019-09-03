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
				parentDocumentGroup.InsertChildAt(dockBefore ? indexOfParentPane : indexOfParentPane + 1, new LayoutDocumentPane(layoutContent) { DockHeight = dockHeight });
				layoutContent.IsActive = true;
				layoutContent.Root.CollectGarbage();
			}
		}
	}
}
