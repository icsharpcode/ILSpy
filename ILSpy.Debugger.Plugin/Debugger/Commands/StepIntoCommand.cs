namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuIcon = "Images/StepInto.png", MenuCategory = "SteppingArea", Header = "Step into", InputGestureText = "F11", IsEnabled = false, MenuOrder = 3.0)]
    internal sealed class StepIntoCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging && !DebuggerCommand.CurrentDebugger.IsProcessRunning)
            {
                base.Execute(null);
                DebuggerCommand.CurrentDebugger.StepInto();
            }
        }
    }
}
