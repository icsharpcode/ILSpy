using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	class ToolPaneCommand(string contentId, DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			dockWorkspace.ShowToolPane(contentId);
		}
	}

	class TabPageCommand(TabPageModel model, DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			// ensure the tab control is focused before setting the active tab page, else the tab will not be focused
			dockWorkspace.ActiveTabPage?.Focus();
			// reset first, else clicking on the already active tab will not focus the tab and the menu checkmark will not be updated
			dockWorkspace.ActiveTabPage = null;
			dockWorkspace.ActiveTabPage = model;
		}
	}
}