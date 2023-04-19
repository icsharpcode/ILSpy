namespace ICSharpCode.ILSpy.Debugger.Commands
{
    [ExportMainMenuCommand(Menu = "_Debugger", MenuCategory = "SteppingArea", Header = "_Detach from running application", IsEnabled = false, MenuOrder = 6.0)]
    internal sealed class DetachCommand : DebuggerCommand
    {
        public override void Execute(object parameter)
        {
            if (DebuggerCommand.CurrentDebugger.IsDebugging)
            {
                DebuggerCommand.CurrentDebugger.Detach();
                base.EnableDebuggerUI(true);
                DebuggerCommand.CurrentDebugger.DebugStopped -= base.OnDebugStopped;
            }
        }
    }
}
