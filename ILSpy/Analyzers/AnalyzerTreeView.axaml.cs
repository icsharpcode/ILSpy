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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ILSpy.AppEnv;

namespace ILSpy.Analyzers
{
	public partial class AnalyzerTreeView : UserControl
	{
		bool syncingSelection;
		AnalyzerTreeViewModel? boundModel;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		// Shared tree keyboard gestures (Left/Right, numpad, type-ahead) -- same as the assembly tree.
		readonly ILSpy.Controls.TreeKeyboardController treeKeyboard;

		public AnalyzerTreeView()
		{
			InitializeComponent();
			TreeGrid.DoubleTapped += OnTreeGridDoubleTapped;
			treeKeyboard = new ILSpy.Controls.TreeKeyboardController(TreeGrid);
			var registry = TryGetContextMenuRegistry();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());
		}

		static ContextMenuEntryRegistry? TryGetContextMenuRegistry()
		{
			try
			{
				return AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
			}
			catch
			{
				return null;
			}
		}

		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			TreeGrid.ContextMenu = menu;
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
				TreeGrid = TreeGrid,
				SelectedTreeNodes = nodes,
			};
		}

		void OnTreeGridDoubleTapped(object? sender, TappedEventArgs e)
		{
			// Walk up the visual tree to find the row's HierarchicalNode wrapper, unwrap it
			// to the SharpTreeNode, then route to ActivateItem so the entity row navigates
			// back to its node in the assembly tree.
			var visual = e.Source as Visual;
			while (visual != null && visual.DataContext is not HierarchicalNode)
				visual = visual.GetVisualParent();
			if (visual?.DataContext is not HierarchicalNode hn)
				return;
			if (hn.Item is not SharpTreeNode node)
				return;
			node.ActivateItem(new StubRoutedEventArgs());
			e.Handled = true;
		}

		sealed class StubRoutedEventArgs : IPlatformRoutedEventArgs
		{
			public bool Handled { get; set; }
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
			BindTree(model.Root);
			model.SelectedItems.CollectionChanged += OnModelSelectionChanged;
		}

		void DetachFromModel()
		{
			if (boundModel == null)
				return;
			boundModel.SelectedItems.CollectionChanged -= OnModelSelectionChanged;
			boundModel = null;
		}

		void BindTree(SharpTreeNode root)
		{
			var options = new HierarchicalOptions<SharpTreeNode> {
				ChildrenSelector = node => {
					node.EnsureLazyChildren();
					return node.Children;
				},
				IsLeafSelector = node => !node.ShowExpander,
				VirtualizeChildren = false,
				// Two-way sync of SharpTreeNode.IsExpanded with the row chevron.
				IsExpandedPropertyPath = nameof(SharpTreeNode.IsExpanded),
			};
			var hierarchicalModel = new HierarchicalModel<SharpTreeNode>(options);
			hierarchicalModel.SetRoots(root.Children);
			TreeGrid.HierarchicalModel = hierarchicalModel;
		}

		void OnTreeGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (syncingSelection || boundModel == null)
				return;
			syncingSelection = true;
			try
			{
				var current = new HashSet<SharpTreeNode>();
				foreach (var item in TreeGrid.SelectedItems)
				{
					var node = item is HierarchicalNode hn && hn.Item is SharpTreeNode t ? t
						: item as SharpTreeNode;
					if (node != null)
						current.Add(node);
				}
				for (int i = boundModel.SelectedItems.Count - 1; i >= 0; i--)
				{
					if (!current.Contains(boundModel.SelectedItems[i]))
						boundModel.SelectedItems.RemoveAt(i);
				}
				foreach (var node in current)
				{
					if (!boundModel.SelectedItems.Contains(node))
						boundModel.SelectedItems.Add(node);
				}
			}
			finally
			{
				syncingSelection = false;
			}
		}

		void OnModelSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (syncingSelection || boundModel == null)
				return;
			syncingSelection = true;
			try
			{
				var current = TreeGrid.SelectedItems.OfType<object>().ToList();
				foreach (var item in current)
					TreeGrid.SelectedItems.Remove(item);
				if (TreeGrid.HierarchicalModel is IHierarchicalModel model)
				{
					foreach (var node in boundModel.SelectedItems)
					{
						var hNode = model.FindNode(node);
						if (hNode != null)
							TreeGrid.SelectedItems.Add(hNode);
					}
				}
			}
			finally
			{
				syncingSelection = false;
			}
		}
	}
}
