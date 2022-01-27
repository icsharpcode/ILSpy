using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Docking
{
	[ExportMainMenuCommand(Header = nameof(Resources.Window_CloseAllDocuments), ParentMenuID = nameof(Resources._Window))]
	class CloseAllDocumentsCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.CloseAllTabs();
		}
	}

	[ExportMainMenuCommand(Header = nameof(Resources.Window_ResetLayout), ParentMenuID = nameof(Resources._Window))]
	class ResetLayoutCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ResetLayout();
		}
	}
}
