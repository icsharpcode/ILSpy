namespace ICSharpCode.ILSpy.ViewModels
{
	public class DocumentModel : PaneModel
	{
		public override PanePosition DefaultPosition => PanePosition.Document;

		public DocumentModel()
		{
			ContentId = "document";
			Title = "View";
			IsCloseable = false;
		}
	}
}
