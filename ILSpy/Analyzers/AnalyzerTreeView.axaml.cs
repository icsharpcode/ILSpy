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

using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia.Controls;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Controls.TreeView;

namespace ICSharpCode.ILSpy.Analyzers
{
	public partial class AnalyzerTreeView : UserControl
	{
		AnalyzerTreeViewModel? boundModel;
		ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder? selectionBinder;
		readonly TreeContextMenuController contextMenu;

		public AnalyzerTreeView()
		{
			InitializeComponent();
			contextMenu = new TreeContextMenuController(Tree,
				() => boundModel?.SelectedItems ?? (IReadOnlyList<SharpTreeNode>)Array.Empty<SharpTreeNode>());
			var registry = AppComposition.TryGetExport<ContextMenuEntryRegistry>();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());
		}

		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
			=> contextMenu.Attach(entries);

		internal ContextMenu? BuildContextMenuForCurrentState(IReadOnlyList<IContextMenuEntryExport> entries)
			=> contextMenu.Build(entries);

		internal ContextMenu? BuildContextMenuForCurrentState(
			IReadOnlyList<IContextMenuEntryExport> entries, SharpTreeNode? rightClickedNode)
			=> contextMenu.Build(entries, rightClickedNode);

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			DetachFromModel();
			if (DataContext is AnalyzerTreeViewModel model)
				AttachToModel(model);
		}

		void AttachToModel(AnalyzerTreeViewModel model)
		{
			boundModel = model;
			Tree.Root = model.Root;
			selectionBinder = new ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder(Tree, model.SelectedItems);
		}

		void DetachFromModel()
		{
			selectionBinder?.Dispose();
			selectionBinder = null;
			boundModel = null;
		}
	}
}
