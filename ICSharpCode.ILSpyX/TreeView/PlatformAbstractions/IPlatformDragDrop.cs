namespace ICSharpCode.ILSpyX.TreeView.PlatformAbstractions
{
	public interface IPlatformDragDrop
	{
		XPlatDragDropEffects DoDragDrop(object dragSource, IPlatformDataObject data, XPlatDragDropEffects allowedEffects);
	}
}
