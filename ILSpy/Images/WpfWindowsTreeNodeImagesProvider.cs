using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy
{
	public class WpfWindowsTreeNodeImagesProvider : ITreeNodeImagesProvider
	{
		public object Assembly => Images.Assembly;
	}
}
