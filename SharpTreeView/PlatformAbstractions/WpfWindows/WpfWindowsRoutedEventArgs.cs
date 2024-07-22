using System.Windows;

namespace ICSharpCode.TreeView.PlatformAbstractions.WpfWindows
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
