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

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;

using ILSpy.Analyzers;
using ILSpy.AssemblyTree;
using ILSpy.Search;
using ILSpy.TextView;

namespace ILSpy.Docking
{
	public class ILSpyDockFactory : Factory
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly SearchPaneModel searchPaneModel;
		readonly AnalyzerTreeViewModel analyzerTreeViewModel;

		public IDocumentDock? Documents { get; private set; }

		public DecompilerTabPageModel? InitialDecompilerTab { get; private set; }

		public ILSpyDockFactory(
			AssemblyTreeModel assemblyTreeModel,
			SearchPaneModel searchPaneModel,
			AnalyzerTreeViewModel analyzerTreeViewModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.searchPaneModel = searchPaneModel;
			this.analyzerTreeViewModel = analyzerTreeViewModel;
		}

		public override IRootDock CreateLayout()
		{
			var leftToolDock = new ToolDock {
				Id = "LeftTools",
				Proportion = 0.25,
				VisibleDockables = CreateList<IDockable>(assemblyTreeModel),
				ActiveDockable = assemblyTreeModel,
				Alignment = Alignment.Left,
			};

			var topToolDock = new ToolDock {
				Id = "TopTools",
				Proportion = 0.2,
				VisibleDockables = CreateList<IDockable>(searchPaneModel),
				ActiveDockable = searchPaneModel,
				Alignment = Alignment.Top,
			};

			var documents = new DocumentDock {
				Id = "Documents",
				Title = "Documents",
				IsCollapsable = false,
				Proportion = 0.6,
			};
			Documents = documents;

			// Initial decompiler tab is added lazily on first selection (DockWorkspace.ShowSelectedNode).
			InitialDecompilerTab = new DecompilerTabPageModel { Title = "(no selection)" };

			var bottomToolDock = new ToolDock {
				Id = "BottomTools",
				Proportion = 0.2,
				VisibleDockables = CreateList<IDockable>(analyzerTreeViewModel),
				ActiveDockable = analyzerTreeViewModel,
				Alignment = Alignment.Bottom,
			};

			var rightVertical = new ProportionalDock {
				Id = "RightArea",
				Orientation = Orientation.Vertical,
				Proportion = 0.75,
				VisibleDockables = CreateList<IDockable>(
					topToolDock,
					new ProportionalDockSplitter { Id = "TopSplitter" },
					documents,
					new ProportionalDockSplitter { Id = "BottomSplitter" },
					bottomToolDock),
			};

			var horizontal = new ProportionalDock {
				Id = "MainLayout",
				Orientation = Orientation.Horizontal,
				VisibleDockables = CreateList<IDockable>(
					leftToolDock,
					new ProportionalDockSplitter { Id = "LeftSplitter" },
					rightVertical),
			};

			var root = CreateRootDock();
			root.Id = "Root";
			root.IsCollapsable = false;
			root.VisibleDockables = CreateList<IDockable>(horizontal);
			root.DefaultDockable = horizontal;
			root.ActiveDockable = horizontal;

			return root;
		}
	}
}
