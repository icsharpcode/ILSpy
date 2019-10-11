using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockWorkspace : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public static DockWorkspace Instance { get; } = new DockWorkspace();

		private DockWorkspace()
		{
			Documents.Add(new DocumentModel());
			ToolPanes.Add(AssemblyListPaneModel.Instance);
		}

		public PaneCollection<DocumentModel> Documents { get; } = new PaneCollection<DocumentModel>();

		public PaneCollection<ToolPaneModel> ToolPanes { get; } = new PaneCollection<ToolPaneModel>();

		public void Remove(PaneModel model)
		{
			Documents.Remove(model as DocumentModel);
			ToolPanes.Remove(model as ToolPaneModel);
		}

		private DocumentModel _activeDocument = null;
		public DocumentModel ActiveDocument {
			get {
				return _activeDocument;
			}
			set {
				if (_activeDocument != value) {
					_activeDocument = value;
					RaisePropertyChanged(nameof(ActiveDocument));
				}
			}
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
