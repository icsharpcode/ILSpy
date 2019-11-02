using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.ILSpy.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace ICSharpCode.ILSpy.Docking
{
	public class LayoutUpdateStrategy : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			if (destinationContainer?.FindParent<LayoutFloatingWindow>() != null)
				return false;

			PanePosition targetPosition = anchorableToShow.Content is PaneModel model ? model.DefaultPosition : PanePosition.Document;

			switch (targetPosition) {
				case PanePosition.Top:
				case PanePosition.Bottom:
				case PanePosition.Left:
				case PanePosition.Right:
					var pane = GetOrCreatePane(layout, targetPosition.ToString());
					if (pane == null)
						return false;
					anchorableToShow.CanDockAsTabbedDocument = false;
					pane.Children.Add(anchorableToShow);
					return true;
				case PanePosition.Document:
					var documentPane = GetOrCreateDocumentPane(layout);
					if (documentPane == null)
						return false;
					documentPane.Children.Add(anchorableToShow);
					return true;
				default:
					throw new NotSupportedException($"Enum value {targetPosition} is not supported");
			}
		}

		private LayoutAnchorablePane GetOrCreatePane(LayoutRoot layout, string name)
		{
			var pane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Name == name + "Pane");
			if (pane != null)
				return pane;
			var layoutPanel = layout.Children.OfType<LayoutPanel>().FirstOrDefault();
			if (layoutPanel == null) {
				layout.RootPanel = new LayoutPanel() { Orientation = Orientation.Horizontal };
			}
			if (layoutPanel.Orientation != Orientation.Horizontal) {
				layoutPanel.Orientation = Orientation.Horizontal;
			}
			LayoutAnchorablePane result = null;
			switch (name) {
				case "Top":
				case "Bottom":
					var centerLayoutPanel = layoutPanel.Children.OfType<LayoutPanel>().FirstOrDefault();
					if (centerLayoutPanel == null) {
						layoutPanel.Children.Insert(0, centerLayoutPanel = new LayoutPanel() { Orientation = Orientation.Vertical });
					}
					if (centerLayoutPanel.Orientation != Orientation.Vertical) {
						centerLayoutPanel.Orientation = Orientation.Vertical;
					}
					if (name == "Top")
						centerLayoutPanel.Children.Insert(0, result = new LayoutAnchorablePane { Name = name + "Pane", DockMinHeight = 250 });
					else
						centerLayoutPanel.Children.Add(result = new LayoutAnchorablePane { Name = name + "Pane", DockMinHeight = 250 });
					return result;
				case "Left":
				case "Right":
					if (name == "Left")
						layoutPanel.Children.Insert(0, result = new LayoutAnchorablePane { Name = name + "Pane", DockMinWidth = 250 });
					else
						layoutPanel.Children.Add(result = new LayoutAnchorablePane { Name = name + "Pane", DockMinWidth = 250 });
					return result;
				default:
					throw new NotImplementedException();
			}
		}

		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
		}

		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			if (destinationContainer?.FindParent<LayoutFloatingWindow>() != null)
				return false;

			var documentPane = GetOrCreateDocumentPane(layout);
			if (documentPane == null)
				return false;
			documentPane.Children.Add(anchorableToShow);
			return true;
		}

		private LayoutDocumentPane GetOrCreateDocumentPane(LayoutRoot layout)
		{
			var pane = layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
			if (pane != null)
				return pane;
			var layoutPanel = layout.Children.OfType<LayoutPanel>().FirstOrDefault();
			if (layoutPanel == null) {
				layout.RootPanel = new LayoutPanel() { Orientation = Orientation.Horizontal };
			}
			if (layoutPanel.Orientation != Orientation.Horizontal) {
				layoutPanel.Orientation = Orientation.Horizontal;
			}
			var centerLayoutPanel = layoutPanel.Children.OfType<LayoutPanel>().FirstOrDefault();
			if (centerLayoutPanel == null) {
				layoutPanel.Children.Insert(0, centerLayoutPanel = new LayoutPanel() { Orientation = Orientation.Vertical });
			}
			if (centerLayoutPanel.Orientation != Orientation.Vertical) {
				centerLayoutPanel.Orientation = Orientation.Vertical;
			}
			LayoutDocumentPane result;
			centerLayoutPanel.Children.Add(result = new LayoutDocumentPane());
			return result;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
		}
	}
}
