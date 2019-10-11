namespace ICSharpCode.ILSpy.ViewModels
{
	public class AnalyzerPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "analyzerPane";

		public static AnalyzerPaneModel Instance { get; } = new AnalyzerPaneModel();

		private AnalyzerPaneModel()
		{
			ContentId = PaneContentId;
			Title = Properties.Resources.Analyze;
		}
	}
}
