using ICSharpCode.ILSpy.Debugger.Bookmarks;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/ContinueDebugging.png", MenuCategory = "SteppingArea", Header = "Continue debugging", InputGestureText = "F5", IsEnabled = false, MenuOrder = 2.0)]
    internal sealed class ContinueDebuggingCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging && !DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                CurrentLineBookmark.Remove();
                DebuggerCommand.CurrentDebugger.Continue();
                MainWindow.Instance.SetStatus("Running...", Brushes.Black);
            }
        }
    }
}
