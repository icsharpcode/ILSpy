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

using System.Collections.Generic;
using System.Linq;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;

using ILSpy.Commands;
using ILSpy.TextView;
using ILSpy.ViewModels;

namespace ILSpy.Docking
{
	public class ILSpyDockFactory : Factory
	{
		readonly IReadOnlyList<ToolPaneEntry> panes;

		public IDocumentDock? Documents { get; private set; }

		/// <summary>
		/// The single persistent Document the host puts in <see cref="Documents"/>. Its
		/// <see cref="ContentTabPage.Content"/> swaps between viewmodels (decompiler text /
		/// metadata grid) on tree-node selection — DockWorkspace owns the population.
		/// </summary>
		public ContentTabPage? MainTab { get; private set; }

		public ILSpyDockFactory(ToolPaneRegistry registry)
		{
			this.panes = registry.Panes;
		}

		public override IRootDock CreateLayout()
		{
			var documents = new DocumentDock {
				Id = "Documents",
				Title = "Documents",
				IsCollapsable = false,
				Proportion = 0.6,
				// Dock's theme template binds against DockCapabilityPolicy.{CanDrag, CanDrop,
				// CanPin, CanClose} on every dock. Pre-populate so the binding sees a non-null
				// source on first evaluation; otherwise every dock startup logs ~6 [Binding]
				// errors as the chrome template is realised.
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
			Documents = documents;

			MainTab = new ContentTabPage { Title = "(no selection)" };
			documents.VisibleDockables = CreateList<IDockable>(MainTab);
			documents.ActiveDockable = MainTab;

			ToolDock? leftToolDock = BuildToolDock("LeftTools", ToolPaneAlignment.Left, 0.25);
			ToolDock? topToolDock = BuildToolDock("TopTools", ToolPaneAlignment.Top, 0.2);
			ToolDock? rightToolDock = BuildToolDock("RightTools", ToolPaneAlignment.Right, 0.25);
			ToolDock? bottomToolDock = BuildToolDock("BottomTools", ToolPaneAlignment.Bottom, 0.2);

			// Vertical column: top tool dock (if any), documents, bottom tool dock (if any),
			// with splitters between siblings.
			var verticalChildren = new List<IDockable>();
			if (topToolDock != null)
			{
				verticalChildren.Add(topToolDock);
				verticalChildren.Add(new ProportionalDockSplitter { Id = "TopSplitter" });
			}
			verticalChildren.Add(documents);
			if (bottomToolDock != null)
			{
				verticalChildren.Add(new ProportionalDockSplitter { Id = "BottomSplitter" });
				verticalChildren.Add(bottomToolDock);
			}

			var rightVertical = new ProportionalDock {
				Id = "MiddleColumn",
				Orientation = Orientation.Vertical,
				Proportion = ComputeMiddleColumnProportion(leftToolDock, rightToolDock),
				VisibleDockables = CreateList(verticalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			// Horizontal row: left tool dock, middle column, right tool dock — splitters
			// between siblings only.
			var horizontalChildren = new List<IDockable>();
			if (leftToolDock != null)
			{
				horizontalChildren.Add(leftToolDock);
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "LeftSplitter" });
			}
			horizontalChildren.Add(rightVertical);
			if (rightToolDock != null)
			{
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "RightSplitter" });
				horizontalChildren.Add(rightToolDock);
			}

			var horizontal = new ProportionalDock {
				Id = "MainLayout",
				Orientation = Orientation.Horizontal,
				VisibleDockables = CreateList(horizontalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			var root = CreateRootDock();
			root.Id = "Root";
			root.IsCollapsable = false;
			root.VisibleDockables = CreateList<IDockable>(horizontal);
			root.DefaultDockable = horizontal;
			root.ActiveDockable = horizontal;
			root.DockCapabilityPolicy = new DockCapabilityPolicy();

			return root;
		}

		ToolDock? BuildToolDock(string id, ToolPaneAlignment alignment, double proportion)
		{
			var dockables = panes
				.Where(p => p.Metadata.Alignment == alignment)
				.OrderBy(p => p.Metadata.Order)
				.Select(p => (IDockable)p.Pane)
				.ToArray();
			if (dockables.Length == 0)
				return null;
			return new ToolDock {
				Id = id,
				Proportion = proportion,
				VisibleDockables = CreateList(dockables),
				ActiveDockable = dockables[0],
				Alignment = alignment switch {
					ToolPaneAlignment.Top => Alignment.Top,
					ToolPaneAlignment.Right => Alignment.Right,
					ToolPaneAlignment.Bottom => Alignment.Bottom,
					_ => Alignment.Left,
				},
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
		}

		static double ComputeMiddleColumnProportion(ToolDock? left, ToolDock? right)
		{
			double remaining = 1.0;
			if (left != null)
				remaining -= left.Proportion;
			if (right != null)
				remaining -= right.Proportion;
			return remaining > 0 ? remaining : 0.5;
		}
	}
}
