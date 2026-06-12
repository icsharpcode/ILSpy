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
using System.Collections.Specialized;
using System.Linq;

using Avalonia.Controls;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Analyzers
{
	public partial class AnalyzerTreeView : UserControl
	{
		AnalyzerTreeViewModel? boundModel;
		ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder? selectionBinder;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		public AnalyzerTreeView()
		{
			InitializeComponent();
			var registry = AppComposition.TryGetExport<ContextMenuEntryRegistry>();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());
		}


		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			Tree.ContextMenu = menu;
		}

		void OnContextMenuOpening(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			var built = BuildContextMenuForCurrentState(contextMenuEntries);
			if (built == null)
			{
				e.Cancel = true;
				return;
			}
			menu.Items.Clear();
			foreach (var item in built.Items.OfType<Control>().ToArray())
			{
				built.Items.Remove(item);
				menu.Items.Add(item);
			}
		}

		internal ContextMenu? BuildContextMenuForCurrentState(IReadOnlyList<IContextMenuEntryExport> entries)
			=> ContextMenuProvider.Build(entries, CreateContextMenuContext());

		TextViewContext CreateContextMenuContext()
		{
			var nodes = boundModel?.SelectedItems.ToArray() ?? Array.Empty<SharpTreeNode>();
			return new TextViewContext {
				TreeGrid = Tree,
				SelectedTreeNodes = nodes,
			};
		}

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
