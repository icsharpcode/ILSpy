#if DEBUG

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportMainMenuCommand(Menu = nameof(Resources._View),  Header = nameof(Resources._ShowDebugSteps),  MenuOrder = 5000)]
	class ShowDebugSteps : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ToolPanes.Add(DebugStepsPaneModel.Instance);
		}
	}
}
#endif