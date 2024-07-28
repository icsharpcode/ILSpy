using System.Windows;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	public class WpfWindowsDataObject : IPlatformDataObject
	{
		private readonly IDataObject _dataObject;

		public WpfWindowsDataObject(IDataObject dataObject)
		{
			_dataObject = dataObject;
		}

		public object GetData(string format)
		{
			return _dataObject.GetData(format);
		}

		public bool GetDataPresent(string format)
		{
			return _dataObject.GetDataPresent(format);
		}

		public void SetData(string format, object data)
		{
			_dataObject.SetData(format, data);
		}
	}
}
