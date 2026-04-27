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

using System.Collections;
using System.ComponentModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

using ILSpy.TreeNodes;

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		// Set while propagating model.SelectedItem → DataGrid.SelectedItem so the resulting
		// SelectionChanged event doesn't bounce right back into the model.
		bool syncingSelection;

		public AssemblyListPane()
		{
			InitializeComponent();
			TreeGrid.DoubleTapped += OnTreeGridDoubleTapped;
		}

		void OnTreeGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (syncingSelection || DataContext is not AssemblyTreeModel model)
				return;
			SharpTreeNode? selected = null;
			if (TreeGrid.SelectedItem is HierarchicalNode node && node.Item is SharpTreeNode tree)
				selected = tree;
			else if (TreeGrid.SelectedItem is SharpTreeNode direct)
				selected = direct;
			if (model.SelectedItem == selected)
				return;
			model.SelectedItem = selected;
		}

		void SyncSelectionFromModel(SharpTreeNode? target)
		{
			if (target == null)
			{
				if (TreeGrid.SelectedItem != null)
				{
					BeginSync();
					TreeGrid.SelectedItem = null!;
					EndSync();
				}
				return;
			}
			if (TreeGrid.HierarchicalModel is not IHierarchicalModel hm)
				return;

			// Build the SharpTreeNode path from a root (a child of the hidden AssemblyListTreeNode)
			// down to the target.
			var path = new System.Collections.Generic.List<SharpTreeNode>();
			for (var n = target; n.Parent != null; n = n.Parent)
				path.Add(n);
			path.Reverse();

			// Walk top-down: ProDataGrid only materialises children of expanded nodes, so each
			// ancestor must be expanded before FindNode can locate the next level's wrapper.
			BeginSync();
			HierarchicalNode? hNode = null;
			for (int i = 0; i < path.Count; i++)
			{
				hNode = hm.FindNode(path[i]);
				if (hNode == null)
				{
					EndSync();
					return;
				}
				if (i < path.Count - 1 && !hNode.IsExpanded)
					hm.Expand(hNode);
			}

			if (ReferenceEquals(TreeGrid.SelectedItem, target))
			{
				EndSync();
				return;
			}
			TreeGrid.SelectedItem = target;
			TreeGrid.ScrollIntoView(target, TreeGrid.Columns[0]);
			EndSync();
		}

		void BeginSync() => syncingSelection = true;

		// Hold the guard across one dispatch tick so any deferred SelectionChanged emits the
		// DataGrid raises in response to our programmatic Expand / SelectedItem / ScrollIntoView
		// calls don't bounce back into model.SelectedItem and cancel the navigation.
		void EndSync() => global::Avalonia.Threading.Dispatcher.UIThread.Post(
			() => syncingSelection = false,
			global::Avalonia.Threading.DispatcherPriority.Background);


		void OnTreeGridDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (TreeGrid.HierarchicalModel is not IHierarchicalModel model)
				return;
			var visual = e.Source as Visual;
			while (visual != null && visual.DataContext is not HierarchicalNode)
				visual = visual.GetVisualParent();
			if (visual?.DataContext is HierarchicalNode node && !node.IsLeaf)
			{
				model.Toggle(node);
				e.Handled = true;
			}
		}

		protected override void OnDataContextChanged(System.EventArgs e)
		{
			base.OnDataContextChanged(e);

			if (DataContext is AssemblyTreeModel model)
			{
				model.PropertyChanged += Model_PropertyChanged;
				if (model.Root != null)
				{
					BindTree(model.Root);
					// Push any already-set selection (e.g. one restored from SessionSettings
					// before the pane subscribed to PropertyChanged) into the DataGrid.
					if (model.SelectedItem != null)
						SyncSelectionFromModel(model.SelectedItem);
				}
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is not AssemblyTreeModel model)
				return;
			if (e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				BindTree(model.Root);
				SyncSelectionFromModel(model.SelectedItem);
			}
			else if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
			{
				SyncSelectionFromModel(model.SelectedItem);
			}
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
			};

			var hierarchicalModel = new HierarchicalModel<SharpTreeNode>(options);
			hierarchicalModel.SetRoots((IEnumerable)root.Children);

			TreeGrid.HierarchicalModel = hierarchicalModel;
		}
	}
}
