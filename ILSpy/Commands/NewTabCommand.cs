using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportToolbarCommand(ToolTip = "New tab", ToolbarIcon = "Images/New.png", ToolbarCategory = "Open", ToolbarOrder = -1)]
	[ExportMainMenuCommand(Menu = "_File", Header = "_New tab", MenuIcon = "Images/New.png", MenuCategory = "Open", MenuOrder = -1)]
	class NewTabCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			var tabControls = App.ExportProvider.GetExportedValue<MainTabView>();
			tabControls.ViewModel.AddDecompilerTab(new DecompilerTextView());
		}
	}
}
