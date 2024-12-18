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
using System.Collections.Specialized;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Composition;
using TomsToolbox.Essentials;
using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Docking
{
	[Export]
	[Shared]
	public class DockWorkspace : ObservableObject, ILayoutUpdateStrategy
	{
		private readonly IExportProvider exportProvider;

		private readonly ObservableCollection<TabPageModel> tabPages = [];
		private ReadOnlyCollection<ToolPaneModel> toolPanes;

		readonly SessionSettings sessionSettings;

		private DockingManager DockingManager => exportProvider.GetExportedValue<DockingManager>();

		public DockWorkspace(SettingsService settingsService, IExportProvider exportProvider)
		{
			this.exportProvider = exportProvider;

			sessionSettings = settingsService.SessionSettings;

			this.tabPages.CollectionChanged += TabPages_CollectionChanged;
			TabPages = new(tabPages);

			MessageBus<CurrentAssemblyListChangedEventArgs>.Subscribers += (sender, e) => CurrentAssemblyList_Changed(sender, e);
		}

		private void CurrentAssemblyList_Changed(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems is not { } oldItems)
				return;

			foreach (var tab in tabPages.ToArray())
			{
				var state = tab.GetState();
				var decompiledNodes = state?.DecompiledNodes;
				if (decompiledNodes == null)
					continue;

				bool found = decompiledNodes
					.Select(node => node.Ancestors().OfType<TreeNodes.AssemblyTreeNode>().LastOrDefault())
					.ExceptNullItems()
					.Any(assemblyNode => !oldItems.Contains(assemblyNode.LoadedAssembly));

				if (!found)
				{
					tabPages.Remove(tab);
				}
			}

			if (tabPages.Count == 0)
			{
				AddTabPage();
			}
		}

		private void TabPages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (e.NewItems?[0] is TabPageModel model)
				{
					ActiveTabPage = model;
					model.IsActive = true;
					model.IsVisible = true;
				}
			}

			bool canClose = tabPages.Count > 1;

			foreach (var item in tabPages)
			{
				item.IsCloseable = canClose;
			}
		}

		public void AddTabPage(TabPageModel tabPage = null)
		{
			tabPages.Add(tabPage ?? exportProvider.GetExportedValue<TabPageModel>());
		}

		public ReadOnlyObservableCollection<TabPageModel> TabPages { get; }

		public ReadOnlyCollection<ToolPaneModel> ToolPanes => toolPanes ??= exportProvider
			.GetExportedValues<ToolPaneModel>("ToolPane")
			.OrderBy(item => item.Title)
			.ToArray()
			.AsReadOnly();

		public bool ShowToolPane(string contentId)
		{
			var pane = ToolPanes.FirstOrDefault(p => p.ContentId == contentId);
			if (pane != null)
			{
				pane.Show();
				return true;
			}
			return false;
		}

		public void Remove(PaneModel model)
		{
			switch (model)
			{
				case TabPageModel document:
					tabPages.Remove(document);
					break;
				case ToolPaneModel tool:
					tool.IsVisible = false;
					break;
			}
		}

		private TabPageModel activeTabPage = null;
		public TabPageModel ActiveTabPage {
			get {
				return activeTabPage;
			}
			set {
				if (!SetProperty(ref activeTabPage, value))
					return;

				var state = value?.GetState();
				if (state == null)
					return;

				MessageBus.Send(this, new ActiveTabPageChangedEventArgs(value?.GetState()));
			}
		}

		public PaneModel ActivePane {
			get => DockingManager.ActiveContent as PaneModel;
			set => DockingManager.ActiveContent = value;
		}

		public void InitializeLayout()
		{
			if (tabPages.Count == 0)
			{
				// Make sure there is at least one tab open
				AddTabPage();
			}

			DockingManager.LayoutUpdateStrategy = this;
			XmlLayoutSerializer serializer = new XmlLayoutSerializer(DockingManager);
			serializer.LayoutSerializationCallback += LayoutSerializationCallback;
			try
			{
				sessionSettings.DockLayout.Deserialize(serializer);
			}
			finally
			{
				serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
			}

			DockingManager.SetBinding(DockingManager.AnchorablesSourceProperty, new Binding(nameof(ToolPanes)));
			DockingManager.SetBinding(DockingManager.DocumentsSourceProperty, new Binding(nameof(TabPages)));
		}

		void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			switch (e.Model)
			{
				case LayoutAnchorable la:
					e.Content = this.ToolPanes.FirstOrDefault(p => p.ContentId == la.ContentId);
					e.Cancel = e.Content == null;
					la.CanDockAsTabbedDocument = false;
					if (e.Content is ToolPaneModel toolPaneModel)
					{
						e.Cancel = toolPaneModel.IsVisible;
						toolPaneModel.IsVisible = true;
					}
					break;
				default:
					e.Cancel = true;
					break;
			}
		}

		public void ShowText(AvalonEditTextOutput textOutput)
		{
			ActiveTabPage.ShowTextView(textView => textView.ShowText(textOutput));
		}

		public Task<T> RunWithCancellation<T>(Func<CancellationToken, Task<T>> taskCreation)
		{
			return ActiveTabPage.ShowTextViewAsync(textView => textView.RunWithCancellation(taskCreation));
		}

		public Task<T> RunWithCancellation<T>(Func<CancellationToken, Task<T>> taskCreation, string progressTitle)
		{
			return ActiveTabPage.ShowTextViewAsync(textView => textView.RunWithCancellation(taskCreation, progressTitle));
		}

		internal void ShowNodes(AvalonEditTextOutput output, TreeNodes.ILSpyTreeNode[] nodes, IHighlightingDefinition highlighting)
		{
			ActiveTabPage.ShowTextView(textView => textView.ShowNodes(output, nodes, highlighting));
		}

		internal void CloseAllTabs()
		{
			var activePage = ActiveTabPage;

			tabPages.RemoveWhere(page => page != activePage);
		}

		internal void ResetLayout()
		{
			foreach (var pane in ToolPanes)
			{
				pane.IsVisible = false;
			}
			CloseAllTabs();
			sessionSettings.DockLayout.Reset();
			InitializeLayout();

			App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, () => MessageBus.Send(this, new ResetLayoutEventArgs()));
		}

		static readonly PropertyInfo previousContainerProperty = typeof(LayoutContent).GetProperty("PreviousContainer", BindingFlags.NonPublic | BindingFlags.Instance);

		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			if (!(anchorableToShow.Content is LegacyToolPaneModel legacyContent))
				return false;
			anchorableToShow.CanDockAsTabbedDocument = false;

			LayoutAnchorablePane previousContainer;
			switch (legacyContent.Location)
			{
				case LegacyToolPaneLocation.Top:
					previousContainer = GetContainer<SearchPaneModel>();
					previousContainer.Children.Add(anchorableToShow);
					return true;
				case LegacyToolPaneLocation.Bottom:
					previousContainer = GetContainer<AnalyzerTreeViewModel>();
					previousContainer.Children.Add(anchorableToShow);
					return true;
				default:
					return false;
			}

			LayoutAnchorablePane GetContainer<T>()
			{
				var anchorable = layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(x => x.Content is T)
					?? layout.Hidden.First(x => x.Content is T);
				return (LayoutAnchorablePane)previousContainerProperty.GetValue(anchorable) ?? (LayoutAnchorablePane)anchorable.Parent;
			}
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
			anchorableShown.IsActive = true;
			anchorableShown.IsSelected = true;
		}

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}

		// Dummy property to make the XAML designer happy, the model is provided by the AvalonDock PaneStyleSelectors, not by the DockWorkspace, but the designer assumes the data context in the PaneStyleSelectors is the DockWorkspace.
		public PaneModel Model { get; } = null;
	}
}
