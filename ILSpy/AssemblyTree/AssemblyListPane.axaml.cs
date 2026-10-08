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
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder? selectionBinder;
		LanguageSettings? languageSettings;
		readonly TreeContextMenuController contextMenu;

		public AssemblyListPane()
		{
			InitializeComponent();
			Loaded += (_, _) => {
				if (DataContext is AssemblyTreeModel m)
					m.MarkTreeReady();
			};
			// MMB opens a new tab; the right-click context target is the TreeContextMenuController's.
			// Drag-reorder + file drop are owned by SharpTreeView (delegated to the tree nodes).
			Tree.AddHandler(PointerPressedEvent, OnTreePointerPressed, RoutingStrategies.Tunnel);
			contextMenu = new TreeContextMenuController(Tree,
				() => (DataContext as AssemblyTreeModel)?.SelectedItems ?? (IReadOnlyList<SharpTreeNode>)Array.Empty<SharpTreeNode>());
			var registry = AppComposition.TryGetExport<ContextMenuEntryRegistry>();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());

			languageSettings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings.LanguageSettings;
			if (languageSettings != null)
				languageSettings.PropertyChanged += OnLanguageSettingsChanged;
		}


		void OnLanguageSettingsChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(LanguageSettings.ShowApiLevel))
				return;
			// Re-apply the API-level filter in place; the flattener drops anything newly hidden.
			if (DataContext is AssemblyTreeModel { Root: ILSpyTreeNode root })
				root.RefreshRealizedFilter();
		}



		#region Context menu

		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
			=> contextMenu.Attach(entries);

		internal ContextMenu? BuildContextMenuForCurrentState(IReadOnlyList<IContextMenuEntryExport> entries)
			=> contextMenu.Build(entries);

		internal ContextMenu? BuildContextMenuForCurrentState(
			IReadOnlyList<IContextMenuEntryExport> entries, SharpTreeNode? rightClickedNode)
			=> contextMenu.Build(entries, rightClickedNode);

		void OnTreePointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (e.Source is Visual hit
				&& e.GetCurrentPoint(hit).Properties.IsMiddleButtonPressed
				&& hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node is ILSpyTreeNode node)
			{
				OpenNodeInNewTab(node);
				e.Handled = true;
			}
		}

		#endregion

		#region Selection sync

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			selectionBinder?.Dispose();
			selectionBinder = null;
			if (DataContext is AssemblyTreeModel model)
			{
				model.PropertyChanged += Model_PropertyChanged;
				if (model.Root != null)
				{
					Tree.Root = model.Root;
					WireDropSelection(model);
				}
				selectionBinder = new ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder(
					Tree, model.SelectedItems, model.BatchSelectionChange);
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is AssemblyTreeModel model
				&& e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				Tree.Root = model.Root;
				WireDropSelection(model);
				// The flattener rebuilt; re-apply the model selection to the new rows.
				selectionBinder?.Refresh();
			}
		}

		// The drop logic lives on AssemblyListTreeNode (open + move); selecting the result is a view
		// concern, so the node delegates it back here, where we resolve the assemblies to tree nodes
		// and push them through the model selection (which the TreeSelectionBinder reflects).
		void WireDropSelection(AssemblyTreeModel model)
		{
			if (model.Root is not AssemblyListTreeNode listRoot)
				return;
			listRoot.SelectAssembliesAfterDrop = assemblies => {
				var nodes = assemblies
					.Select(listRoot.FindAssemblyNode)
					.Where(n => n != null)
					.Cast<SharpTreeNode>()
					.ToList();
				if (nodes.Count == 0)
					return;
				// Go through the batched setter, not a raw Clear()+Add(): the latter flashes a
				// transient empty selection that poisons the grid-sync deferred guard (the tree
				// stops following tab activation).
				model.SelectNodes(nodes);
			};
		}

		#endregion


		internal void OpenNodeInNewTab(ILSpyTreeNode node)
		{
			// Composition unavailable in design-time previews -> no-op.
			AppComposition.TryGetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()?.OpenNodeInNewTab(node);
		}
	}
}
