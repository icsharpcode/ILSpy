namespace ICSharpCode.ILSpy.ViewModels
{
	public class SearchPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "searchPane";

		public static SearchPaneModel Instance { get; } = new SearchPaneModel();

		private SearchPaneModel()
		{
			ContentId = PaneContentId;
			Title = Properties.Resources.SearchPane_Search;
			IsCloseable = true;
		}
	}
}
