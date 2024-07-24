namespace ICSharpCode.ILSpyX.TreeView.PlatformAbstractions
{
	public interface IPlatformDragEventArgs
	{
		XPlatDragDropEffects Effects { get; set; }
		IPlatformDataObject Data { get; }
	}
}
