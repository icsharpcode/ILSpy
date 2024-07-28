using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	public class WpfWindowsDragDropManager : IPlatformDragDrop
	{
		public XPlatDragDropEffects DoDragDrop(object dragSource, object data, XPlatDragDropEffects allowedEffects)
		{
			return (XPlatDragDropEffects)DragDrop.DoDragDrop(dragSource as DependencyObject, data, (DragDropEffects)allowedEffects);
		}
	}
}