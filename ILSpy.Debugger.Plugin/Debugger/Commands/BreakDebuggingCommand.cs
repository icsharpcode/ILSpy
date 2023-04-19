using System.Windows.Media;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/Break.png", MenuCategory = "SteppingArea", Header = "Break", IsEnabled = false, MenuOrder = 2.1)]
    internal sealed class BreakDebuggingCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging && DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                DebuggerCommand.CurrentDebugger.Break();
                MainWindow.Instance.SetStatus("Debugging...", Brushes.Red);
            }
        }
    }
}
