using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	public class WpfWindowsRoutedEventArgs : IPlatformRoutedEventArgs
	{
		private readonly RoutedEventArgs _eventArgs;

		public WpfWindowsRoutedEventArgs(RoutedEventArgs eventArgs)
		{
			_eventArgs = eventArgs;
		}

		public bool Handled { get => _eventArgs.Handled; set => _eventArgs.Handled = value; }
	}
}
