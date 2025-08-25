using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	public class WpfWindowsDragDropManager : IPlatformDragDrop
	{
		public XPlatDragDropEffects DoDragDrop(object dragSource, IPlatformDataObject data, XPlatDragDropEffects allowedEffects)
		{
			return (XPlatDragDropEffects)DragDrop.DoDragDrop(dragSource as DependencyObject, data.UnderlyingDataObject, (DragDropEffects)allowedEffects);
		}
	}
}