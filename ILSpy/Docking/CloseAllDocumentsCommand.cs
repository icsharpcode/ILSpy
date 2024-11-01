using System.Composition;

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Docking
{
	[ExportMainMenuCommand(Header = nameof(Resources.Window_CloseAllDocuments), ParentMenuID = nameof(Resources._Window))]
	[Shared]
	class CloseAllDocumentsCommand(DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			dockWorkspace.CloseAllTabs();
		}
	}

	[ExportMainMenuCommand(Header = nameof(Resources.Window_ResetLayout), ParentMenuID = nameof(Resources._Window))]
	[Shared]
	class ResetLayoutCommand(DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			dockWorkspace.ResetLayout();
		}
	}
}
