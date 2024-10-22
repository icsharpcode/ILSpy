using System.Composition;

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Docking
{
	[ExportMainMenuCommand(Header = nameof(Resources.Window_CloseAllDocuments), ParentMenuID = nameof(Resources._Window))]
	[Shared]
	class CloseAllDocumentsCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.CloseAllTabs();
		}
	}

	[ExportMainMenuCommand(Header = nameof(Resources.Window_ResetLayout), ParentMenuID = nameof(Resources._Window))]
	[Shared]
	class ResetLayoutCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ResetLayout();
		}
	}
}
