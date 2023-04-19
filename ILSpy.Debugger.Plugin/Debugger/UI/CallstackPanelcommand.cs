namespace ICSharpCode.ILSpy.Debugger.UI
{
    [ExportMainMenuCommand(Menu = "_Debugger", Header = "Show _Callstack", MenuCategory = "View", MenuOrder = 9.0)]
    public class CallstackPanelcommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            CallStackPanel.Instance.Show();
        }
    }
}
