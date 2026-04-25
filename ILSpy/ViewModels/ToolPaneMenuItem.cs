// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using CommunityToolkit.Mvvm.ComponentModel;

using Dock.Model.Controls;
using Dock.Model.Core;

namespace ILSpy.ViewModels
{
	public class ToolPaneMenuItem : ObservableObject
	{
		readonly ToolPaneModel pane;
		readonly IFactory factory;

		public ToolPaneMenuItem(ToolPaneModel pane, IFactory factory)
		{
			this.pane = pane;
			this.factory = factory;
			factory.DockableHidden += OnFactoryVisibilityChanged;
			factory.DockableRestored += OnFactoryVisibilityChanged;
		}

		public string? Title => pane.Title;

		public bool IsPaneVisible {
			get => pane.Owner is not IRootDock;
			set {
				if (value == IsPaneVisible)
					return;
				if (value)
					factory.RestoreDockable(pane);
				else
					factory.HideDockable(pane);
				OnPropertyChanged();
			}
		}

		void OnFactoryVisibilityChanged(object? sender, System.EventArgs e)
		{
			OnPropertyChanged(nameof(IsPaneVisible));
		}
	}
}
