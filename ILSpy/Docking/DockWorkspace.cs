﻿// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockWorkspace : ObservableObject, ILayoutUpdateStrategy
	{
		private static SessionSettings SessionSettings => SettingsService.Instance.SessionSettings;

		public static readonly DockWorkspace Instance = new();

		private DockWorkspace()
		{
			this.TabPages.CollectionChanged += Documents_CollectionChanged;
			MessageBus<CurrentAssemblyListChangedEventArgs>.Subscribers += (sender, e) => CurrentAssemblyList_Changed(sender, e);
		}

		private void CurrentAssemblyList_Changed(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems == null)
			{
				return;
			}
			foreach (var tab in TabPages.ToArray())
			{
				var state = tab.GetState();
				if (state == null || state.DecompiledNodes == null)
				{
					continue;
				}
				bool found = false;
				foreach (var node in state.DecompiledNodes)
				{
					var assemblyNode = node.Ancestors().OfType<TreeNodes.AssemblyTreeNode>().LastOrDefault();
					if (assemblyNode != null && !e.OldItems.Contains(assemblyNode.LoadedAssembly))
					{
						found = true;
						break;
					}
				}
				if (!found && TabPages.Count > 1)
				{
					TabPages.Remove(tab);
				}
			}
		}

		private void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var collection = (PaneCollection<TabPageModel>)sender;
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				ActiveTabPage = e.NewItems?[0] as TabPageModel;
			}

			bool canClose = collection.Count > 1;
			foreach (var item in collection)
			{
				item.IsCloseable = canClose;
			}
		}

		public PaneCollection<TabPageModel> TabPages { get; } = new PaneCollection<TabPageModel>();

		public ObservableCollection<ToolPaneModel> ToolPanes { get; } = new ObservableCollection<ToolPaneModel>();

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
			if (model is TabPageModel document)
				TabPages.Remove(document);
			if (model is ToolPaneModel tool)
				tool.IsVisible = false;
		}

		private TabPageModel activeTabPage = null;
		public TabPageModel ActiveTabPage {
			get {
				return activeTabPage;
			}
			set {
				if (!SetProperty(ref activeTabPage, value))
				{
					return;
				}

				var state = value.GetState();
				if (state != null)
				{
					if (state.DecompiledNodes != null)
					{
						MainWindow.Instance.SelectNodes(state.DecompiledNodes,
							inNewTabPage: false, setFocus: true, changingActiveTab: true);
					}
					else
					{
						MainWindow.Instance.NavigateTo(new(state.ViewedUri, null));
					}
				}
				MessageBus.Send(this, new DockWorkspaceActiveTabPageChangedEventArgs());
			}
		}

		public void InitializeLayout(DockingManager manager)
		{
			manager.LayoutUpdateStrategy = this;
			XmlLayoutSerializer serializer = new XmlLayoutSerializer(manager);
			serializer.LayoutSerializationCallback += LayoutSerializationCallback;
			try
			{
				SessionSettings.DockLayout.Deserialize(serializer);
			}
			finally
			{
				serializer.LayoutSerializationCallback -= LayoutSerializationCallback;
			}
		}

		void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
		{
			switch (e.Model)
			{
				case LayoutAnchorable la:
					e.Content = ToolPanes.FirstOrDefault(p => p.ContentId == la.ContentId);
					e.Cancel = e.Content == null;
					la.CanDockAsTabbedDocument = false;
					if (!e.Cancel)
					{
						e.Cancel = ((ToolPaneModel)e.Content).IsVisible;
						((ToolPaneModel)e.Content).IsVisible = true;
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

		internal void ShowNodes(AvalonEditTextOutput output, TreeNodes.ILSpyTreeNode[] nodes, IHighlightingDefinition highlighting)
		{
			ActiveTabPage.ShowTextView(textView => textView.ShowNodes(output, nodes, highlighting));
		}

		internal void CloseAllTabs()
		{
			foreach (var doc in TabPages.ToArray())
			{
				if (doc.IsCloseable)
					TabPages.Remove(doc);
			}
		}

		internal void ResetLayout()
		{
			foreach (var pane in ToolPanes)
			{
				pane.IsVisible = false;
			}
			CloseAllTabs();
			SessionSettings.DockLayout.Reset();
			InitializeLayout(MainWindow.Instance.dockManager);
			MainWindow.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)MainWindow.Instance.RefreshDecompiledView);
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
