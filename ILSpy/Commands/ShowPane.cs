using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportMainMenuCommand(Menu = nameof(Resources._Window), Header = nameof(Resources._Assemblies), MenuCategory = "pane", MenuOrder = 5000)]
	class ShowAssemblies : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ShowToolPane(AssemblyListPaneModel.PaneContentId);
		}
	}

	[ExportMainMenuCommand(Menu = nameof(Resources._Window), Header = nameof(Resources._Analyzer), MenuCategory = "pane", MenuOrder = 5000)]
	class ShowAnalyzer : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ShowToolPane(AnalyzerPaneModel.PaneContentId);
		}
	}

#if DEBUG
	[ExportMainMenuCommand(Menu = nameof(Resources._Window), Header = nameof(Resources._ShowDebugSteps), MenuCategory = "pane", MenuOrder = 5000)]
	class ShowDebugSteps : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			DockWorkspace.Instance.ShowToolPane(DebugStepsPaneModel.PaneContentId);
		}
	}
#endif
}