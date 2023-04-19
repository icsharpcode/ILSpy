namespace ICSharpCode.ILSpy.Debugger.UI
{
    [ExportMainMenuCommand(Menu = "_Debugger", Header = "Show _Breakpoints", MenuCategory = "View", MenuOrder = 8.0)]
    public class BookmarkManagerPanelCommand : SimpleCommand
    {
        public override void Execute(object parameter)
        {
            BreakpointPanel.Instance.Show();
        }
    }
}
