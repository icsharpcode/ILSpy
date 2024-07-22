namespace ICSharpCode.TreeView.PlatformAbstractions
{
	public interface IPlatformDataObject
	{
		bool GetDataPresent(string format);
		object GetData(string format);

		void SetData(string format, object data);
	}
}
