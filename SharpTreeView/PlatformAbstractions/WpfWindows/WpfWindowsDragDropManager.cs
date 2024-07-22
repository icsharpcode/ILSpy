using System.Windows;

namespace ICSharpCode.TreeView.PlatformAbstractions.WpfWindows
{
	public class WpfWindowsDragDropManager : IPlatformDragDrop
	{
		public XPlatDragDropEffects DoDragDrop(object dragSource, object data, XPlatDragDropEffects allowedEffects)
		{
			return (XPlatDragDropEffects)DragDrop.DoDragDrop(dragSource as DependencyObject, data, (DragDropEffects)allowedEffects);
		}
	}
}