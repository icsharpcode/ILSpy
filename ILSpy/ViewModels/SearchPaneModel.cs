namespace ICSharpCode.ILSpy.ViewModels
{
	public class SearchPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "searchPane";

		public static SearchPaneModel Instance { get; } = new SearchPaneModel();

		public override PanePosition DefaultPosition => PanePosition.Top;

		private SearchPaneModel()
		{
			ContentId = PaneContentId;
			Title = Properties.Resources.SearchPane_Search;
			IsCloseable = true;
		}
	}
}
