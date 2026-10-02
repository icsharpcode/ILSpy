// Copyright (c) 2026 Piero Viano
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// View for <see cref="DependencyGraphViewModel"/> (resolved by the view locator's
	/// <c>*ViewModel</c> to <c>*View</c> convention): a toolbar with the graph summary, a
	/// zoom-to-fit button and the Mermaid export, above a <see cref="DependencyGraphControl"/>.
	/// </summary>
	public sealed class DependencyGraphView : UserControl
	{
		readonly DependencyGraphControl graphControl = new();
		readonly TextBlock summary = new() { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 12, 0) };
		DependencyGraphViewModel? model;

		public DependencyGraphView()
		{
			var fit = new Button { Content = "Zoom to Fit", Margin = new Thickness(2) };
			fit.Click += (_, _) => graphControl.ZoomToFit();
			var export = new Button { Content = ICSharpCode.ILSpy.Properties.Resources.ExportDependencyDiagramMermaid, Margin = new Thickness(2) };
			export.Click += (_, _) => {
				if (model != null)
					DependencyDiagramActions.ExportMermaidAsync(model.Roots, path: null).HandleExceptions();
			};
			var toolbar = new StackPanel {
				Orientation = Orientation.Horizontal,
				Margin = new Thickness(4),
				Children = { summary, fit, export },
			};
			DockPanel.SetDock(toolbar, Avalonia.Controls.Dock.Top);
			graphControl.NodeClicked += index => model?.SelectInTree(index);
			graphControl.NodeDoubleClicked += index => model?.OpenInNewTab(index);
			Content = new DockPanel { Children = { toolbar, graphControl } };
		}

		/// <summary>The drawing surface (exposed for UI tests).</summary>
		public DependencyGraphControl GraphControl => graphControl;

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			model = DataContext as DependencyGraphViewModel;
			graphControl.Graph = model?.Graph;
			summary.Text = model?.Summary ?? string.Empty;
		}
	}
}
