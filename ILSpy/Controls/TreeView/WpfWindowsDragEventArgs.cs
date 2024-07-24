using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	public class WpfWindowsDragEventArgs : IPlatformDragEventArgs
	{
		private readonly DragEventArgs _eventArgs;

		public WpfWindowsDragEventArgs(DragEventArgs eventArgs)
		{
			_eventArgs = eventArgs;
		}

		public XPlatDragDropEffects Effects { get => (XPlatDragDropEffects)_eventArgs.Effects; set => _eventArgs.Effects = (DragDropEffects)value; }

		public IPlatformDataObject Data => new WpfWindowsDataObject(_eventArgs.Data);
	}
}
