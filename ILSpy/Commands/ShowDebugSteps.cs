#if DEBUG

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportMainMenuCommand(Menu = nameof(Resources._View),  Header = nameof(Resources._ShowDebugSteps),  MenuOrder = 5000)]
	class ShowDebugSteps : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DebugSteps.Show();
		}
	}
}
#endif