namespace ICSharpCode.ILSpy.ViewModels
{
	public class DebugStepsPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "debugStepsPane";

		public static DebugStepsPaneModel Instance { get; } = new DebugStepsPaneModel();

		public override PanePosition DefaultPosition => PanePosition.Top;

		private DebugStepsPaneModel()
		{
			ContentId = PaneContentId;
			Title = Properties.Resources.DebugSteps;
		}
	}
}
