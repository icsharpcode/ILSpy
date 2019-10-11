namespace ICSharpCode.ILSpy.ViewModels
{
	public class AssemblyListPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "assemblyListPane";

		public static AssemblyListPaneModel Instance { get; } = new AssemblyListPaneModel();

		private AssemblyListPaneModel()
		{
			Title = "Assemblies";
			ContentId = PaneContentId;
			IsCloseable = false;
		}
	}
}
