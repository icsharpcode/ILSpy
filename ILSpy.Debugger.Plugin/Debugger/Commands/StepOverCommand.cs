namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/StepOver.png", MenuCategory = "SteppingArea", Header = "Step over", InputGestureText = "F10", IsEnabled = false, MenuOrder = 4.0)]
    internal sealed class StepOverCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging && !DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                base.Execute(null);
                DebuggerCommand.CurrentDebugger.StepOver();
            }
        }
    }
}
