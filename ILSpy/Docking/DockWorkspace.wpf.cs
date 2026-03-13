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

using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Threading;

using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	/// <summary>
	/// WPF-specific extensions to <see cref="DockWorkspace"/>.
	/// </summary>
	partial class DockWorkspace
	{
		private DockingManager DockingManager => exportProvider.GetExportedValue<DockingManager>();

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
	}
}