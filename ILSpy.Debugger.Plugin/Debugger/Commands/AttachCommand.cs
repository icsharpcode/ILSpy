using ICSharpCode.ILSpy.Debugger.UI;
using System.Windows;

namespace ICSharpCode.ILSpy.Debugger.Commands
{

    [ExportMainMenuCommand(Menu = "_Debugger", MenuCategory = "Start", Header = "Attach to _running application", MenuOrder = 1.0)]
    internal sealed class AttachCommand : DebuggerCommand
    {

        public override void Execute(object parameter)
        {
            if (!DebuggerCommand.CurrentDebugger.IsDebugging)
            {
                if (DebuggerSettings.Instance.ShowWarnings)
                {
                    MessageBox.Show("Warning: When attaching to an application, some local variables might not be available. If possible, use the \"Start Executable\" command.", "Attach to a process", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                AttachToProcessWindow attachToProcessWindow = new AttachToProcessWindow
                {
                    Owner = MainWindow.Instance
                };
                if (attachToProcessWindow.ShowDialog() == true)
                {
                    base.StartAttaching(attachToProcessWindow.SelectedProcess);
                }
            }
        }
    }
}
