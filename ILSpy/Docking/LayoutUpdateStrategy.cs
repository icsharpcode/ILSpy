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
