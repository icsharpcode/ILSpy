using System;

namespace ICSharpCode.ILSpyX.TreeView.PlatformAbstractions
{
	//
	// Summary:
	//     Specifies the effects of a drag-and-drop operation.
	[Flags]
	public enum XPlatDragDropEffects
	{
		//
		// Summary:
		//     Scrolling is about to start or is currently occurring in the drop target.
		Scroll = int.MinValue,
		//
		// Summary:
		//     The data is copied, removed from the drag source, and scrolled in the drop target.
		All = -2147483645,
		//
		// Summary:
		//     The drop target does not accept the data.
		None = 0,
		//
		// Summary:
		//     The data is copied to the drop target.
		Copy = 1,
		//
		// Summary:
		//     The data from the drag source is moved to the drop target.
		Move = 2,
		//
		// Summary:
		//     The data from the drag source is linked to the drop target.
		Link = 4
	}
}
