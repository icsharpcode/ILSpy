using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockWorkspace : INotifyPropertyChanged
	{
		private SessionSettings sessionSettings;

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
					if (value is DecompiledDocumentModel ddm) {
						this.sessionSettings.FilterSettings.Language = ddm.Language;
						this.sessionSettings.FilterSettings.LanguageVersion = ddm.LanguageVersion;
					}
					RaisePropertyChanged(nameof(ActiveDocument));
				}
			}
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
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

		internal void LoadSettings(SessionSettings sessionSettings)
		{
			this.sessionSettings = sessionSettings;
			sessionSettings.FilterSettings.PropertyChanged += FilterSettings_PropertyChanged;
		}

		private void FilterSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ActiveDocument is DecompiledDocumentModel ddm) {
				if (e.PropertyName == "Language" || e.PropertyName == "LanguageVersion") {
					ddm.Language = sessionSettings.FilterSettings.Language;
					ddm.LanguageVersion = sessionSettings.FilterSettings.LanguageVersion;
				}
			}
		}
	}
}
