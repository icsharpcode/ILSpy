using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	class ToolPaneCommand : SimpleCommand
	{
		readonly string contentId;

		public ToolPaneCommand(string contentId)
		{
			this.contentId = contentId;
		}

		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ShowToolPane(contentId);
		}
	}

	class TabPageCommand : SimpleCommand
	{
		readonly TabPageModel model;

		public TabPageCommand(TabPageModel model)
		{
			this.model = model;
		}

		public override void Execute(object parameter)
		{
			var workspace = DockWorkspace.Instance;

			// ensure the tab control is focused before setting the active tab page, else the tab will not be focused
			workspace.ActiveTabPage?.Focus();
			// reset first, else clicking on the already active tab will not focus the tab and the menu checkmark will not be updated
			workspace.ActiveTabPage = null;
			workspace.ActiveTabPage = model;
		}
	}
}