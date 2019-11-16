using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockWorkspace : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public static DockWorkspace Instance { get; } = new DockWorkspace();

		private DockWorkspace()
		{
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

		public void ShowText(AvalonEditTextOutput textOutput)
		{
			GetTextView().ShowText(textOutput);
		}

		public DecompilerTextView GetTextView()
		{
			return ((DecompiledDocumentModel)ActiveDocument).TextView;
		}

		public DecompilerTextViewState GetState()
		{
			return GetTextView().GetState();
		}

		public Task<T> RunWithCancellation<T>(Func<CancellationToken, Task<T>> taskCreation)
		{
			return GetTextView().RunWithCancellation(taskCreation);
		}

		internal void ShowNodes(AvalonEditTextOutput output, TreeNodes.ILSpyTreeNode[] nodes, IHighlightingDefinition highlighting)
		{
			GetTextView().ShowNodes(output, nodes, highlighting);
		}
	}
}
