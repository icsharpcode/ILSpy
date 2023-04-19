using ICSharpCode.ILSpy.Debugger.UI;
using Microsoft.Win32;

namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/application-x-executable.png", MenuCategory = "Start", Header = "Debug an _executable", MenuOrder = 0.0)]
    [ExportToolbarCommand(ToolTip = "Debug an executable", ToolbarIcon = "Images/application-x-executable.png", ToolbarCategory = "Debugger", Tag = "Debugger", ToolbarOrder = 0.0)]
    internal sealed class DebugExecutableCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (!DebuggerCommand.CurrentDebugger.IsDebugging)
            {
                if (DebuggerSettings.Instance.AskForArguments)
                {
                    ExecuteProcessWindow executeProcessWindow = new ExecuteProcessWindow
                    {
                        Owner = MainWindow.Instance
                    };
                    if (executeProcessWindow.ShowDialog() == true)
                    {
                        string selectedExecutable = executeProcessWindow.SelectedExecutable;
                        MainWindow.Instance.OpenFiles(new string[]
                        {
                            selectedExecutable
                        }, false);
                        base.StartExecutable(selectedExecutable, executeProcessWindow.WorkingDirectory, executeProcessWindow.Arguments);
                        return;
                    }
                }
                else
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = ".NET Executable (*.exe) | *.exe",
                        RestoreDirectory = true,
                        DefaultExt = "exe"
                    };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        string fileName = openFileDialog.FileName;
                        MainWindow.Instance.OpenFiles(new string[]
                        {
                            fileName
                        }, false);
                        base.StartExecutable(fileName, null, null);
                    }
                }
            }
        }
    }
}
