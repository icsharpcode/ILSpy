namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/StepOut.png", MenuCategory = "SteppingArea", Header = "Step out", IsEnabled = false, MenuOrder = 5.0)]
    internal sealed class StepOutCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging && !DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                base.Execute(null);
                DebuggerCommand.CurrentDebugger.StepOut();
            }
        }
    }
}
