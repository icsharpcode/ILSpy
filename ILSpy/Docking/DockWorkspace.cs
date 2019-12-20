// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockWorkspace : INotifyPropertyChanged
	{
		private SessionSettings sessionSettings;

		public event PropertyChangedEventHandler PropertyChanged;

		public static DockWorkspace Instance { get; } = new DockWorkspace();

		private DockWorkspace()
		{
			this.TabPages.CollectionChanged += Documents_CollectionChanged;
		}

		private void Documents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var collection = (PaneCollection<TabPageModel>)sender;
			bool canClose = collection.Count > 1;
			foreach (var item in collection) {
				item.IsCloseable = canClose;
			}
		}

		public PaneCollection<TabPageModel> TabPages { get; } = new PaneCollection<TabPageModel>();

		private ToolPaneModel[] toolPanes;
		public IEnumerable<ToolPaneModel> ToolPanes {
			get {
				if (toolPanes == null) {
					toolPanes = new ToolPaneModel[] {
						AssemblyListPaneModel.Instance,
						SearchPaneModel.Instance,
						AnalyzerPaneModel.Instance,
#if DEBUG
						DebugStepsPaneModel.Instance,
#endif
					};
				}
				return toolPanes;
			}
		}

		public void Remove(PaneModel model)
		{
			if (model is TabPageModel document)
				TabPages.Remove(document);
			if (model is ToolPaneModel tool)
				tool.IsVisible = false;
		}

		private TabPageModel _activeTabPage = null;
		public TabPageModel ActiveTabPage {
			get {
				return _activeTabPage;
			}
			set {
				if (_activeTabPage != value) {
					_activeTabPage = value;
					this.sessionSettings.FilterSettings.Language = value.Language;
					this.sessionSettings.FilterSettings.LanguageVersion = value.LanguageVersion;
					var state = value.GetState();
					if (state != null)
						MainWindow.Instance.SelectNodes(state.DecompiledNodes);
					RaisePropertyChanged(nameof(ActiveTabPage));
				}
			}
		}

		public void InitializeLayout(Xceed.Wpf.AvalonDock.DockingManager manager)
		{
			XmlLayoutSerializer serializer = new XmlLayoutSerializer(manager);
			serializer.LayoutSerializationCallback += LayoutSerializationCallback;
			try {
				sessionSettings.DockLayout.Deserialize(serializer);
			} finally {
				serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
			}
		}

		void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			switch (e.Model) {
				case LayoutAnchorable la:
					switch (la.ContentId) {
						case AssemblyListPaneModel.PaneContentId:
							e.Content = AssemblyListPaneModel.Instance;
							break;
						case SearchPaneModel.PaneContentId:
							e.Content = SearchPaneModel.Instance;
							break;
						case AnalyzerPaneModel.PaneContentId:
							e.Content = AnalyzerPaneModel.Instance;
							break;
#if DEBUG
						case DebugStepsPaneModel.PaneContentId:
							e.Content = DebugStepsPaneModel.Instance;
							break;
#endif
						default:
							e.Cancel = true;
							break;
					}
					la.CanDockAsTabbedDocument = false;
					if (!e.Cancel) {
						e.Cancel = ((ToolPaneModel)e.Content).IsVisible;
						((ToolPaneModel)e.Content).IsVisible = true;
					}
					break;
				default:
					e.Cancel = true;
					break;
			}
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void ShowText(AvalonEditTextOutput textOutput)
		{
			ActiveTabPage.ShowTextView(textView => textView.ShowText(textOutput));
		}

		public Task<T> RunWithCancellation<T>(Func<CancellationToken, Task<T>> taskCreation)
		{
			return ActiveTabPage.ShowTextViewAsync(textView => textView.RunWithCancellation(taskCreation));
		}

		internal void ShowNodes(AvalonEditTextOutput output, TreeNodes.ILSpyTreeNode[] nodes, IHighlightingDefinition highlighting)
		{
			ActiveTabPage.ShowTextView(textView => textView.ShowNodes(output, nodes, highlighting));
		}

		internal void LoadSettings(SessionSettings sessionSettings)
		{
			this.sessionSettings = sessionSettings;
			sessionSettings.FilterSettings.PropertyChanged += FilterSettings_PropertyChanged;
		}

		private void FilterSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Language") {
				ActiveTabPage.Language = sessionSettings.FilterSettings.Language;
				if (sessionSettings.FilterSettings.Language.HasLanguageVersions) {
					sessionSettings.FilterSettings.LanguageVersion = ActiveTabPage.LanguageVersion;
				}
			} else if (e.PropertyName == "LanguageVersion") {
				ActiveTabPage.LanguageVersion = sessionSettings.FilterSettings.LanguageVersion;
			}
		}

		internal void CloseAllTabs()
		{
			foreach (var doc in TabPages.ToArray()) {
				if (doc.IsCloseable)
					TabPages.Remove(doc);
			}
		}

		internal void ResetLayout()
		{
			foreach (var pane in ToolPanes) {
				pane.IsVisible = false;
			}
			CloseAllTabs();
			sessionSettings.DockLayout.Reset();
			InitializeLayout(MainWindow.Instance.DockManager);
			MainWindow.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)MainWindow.Instance.RefreshDecompiledView);
		}
	}
}
