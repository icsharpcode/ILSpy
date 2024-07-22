namespace ICSharpCode.TreeView.PlatformAbstractions
{
	public interface IPlatformDragDrop
	{
		XPlatDragDropEffects DoDragDrop(object dragSource, object data, XPlatDragDropEffects allowedEffects);
	}
}
