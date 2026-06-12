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

namespace ICSharpCode.ILSpy.ViewModels
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
			// Visible only when the pane currently sits in a real (non-root) dock's visible
			// dockables. A pane that is hidden by default and never shown has a null Owner — the
			// old `Owner is not IRootDock` test reported that as visible, so the Window-menu toggle
			// thought it was already open and "closed" it (a no-op) instead of showing it.
			get => pane.Owner is IDock owner
				&& owner is not IRootDock
				&& owner.VisibleDockables?.Contains(pane) == true;
			set {
				if (value == IsPaneVisible)
					return;
				if (value)
				{
					// RestoreDockable only un-hides a dockable that was previously shown and then
					// hidden; a pane that is hidden by default (e.g. Debug Steps) was never placed in
					// the layout, so there is nothing to restore. ShowToolPane materialises the pane
					// AND (re)creates its home dock, so it works in both cases.
					if (factory is Docking.ILSpyDockFactory ilspyFactory)
						ilspyFactory.ShowToolPane(pane.Id);
					else
						factory.RestoreDockable(pane);
				}
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
