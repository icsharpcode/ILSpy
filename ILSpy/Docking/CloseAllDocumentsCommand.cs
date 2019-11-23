using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Docking
{
	[ExportMainMenuCommand(Header = nameof(Resources.Window_CloseAllDocuments), Menu = nameof(Resources._Window))]
	class CloseAllDocumentsCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.CloseAllDocuments();
		}
	}

	[ExportMainMenuCommand(Header = nameof(Resources.Window_ResetLayout), Menu = nameof(Resources._Window))]
	class ResetLayoutCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ResetLayout();
		}
	}
}
