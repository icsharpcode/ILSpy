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
using System.Collections;
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

using ILSpy.TreeNodes;

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		// Suppresses the SelectionChanged → model bounce while we're pushing the model's
		// selection into the DataGrid.
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
			// SelectedItem is a wrapper over SelectedItems on the model — touching it would
			// Clear+Add and clobber Ctrl-click multi-selection. Only mutate the collection.
			BeginSync();
			try
			{
				var current = new System.Collections.Generic.HashSet<SharpTreeNode>();
				foreach (var item in TreeGrid.SelectedItems)
				{
					var node = item is HierarchicalNode hn && hn.Item is SharpTreeNode t ? t
						: item as SharpTreeNode;
					if (node != null)
						current.Add(node);
				}
				for (int i = model.SelectedItems.Count - 1; i >= 0; i--)
				{
					if (!current.Contains(model.SelectedItems[i]))
						model.SelectedItems.RemoveAt(i);
				}
				foreach (var node in current)
				{
					if (!model.SelectedItems.Contains(node))
						model.SelectedItems.Add(node);
				}
			}
			finally
			{
				EndSync();
			}
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

			var path = new System.Collections.Generic.List<SharpTreeNode>();
			for (var n = target; n.Parent != null; n = n.Parent)
				path.Add(n);
			path.Reverse();

			// Each ancestor must be expanded before FindNode can locate the next level's wrapper.
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
			// Pass the HierarchicalNode wrapper (DataGrid.IndexOf is against the wrappers, not
			// the underlying SharpTreeNodes), and defer past Expand's pending child-realization
			// notifications — synchronously the wrapper isn't in the visible list yet.
			var scrollTarget = hNode!;
			Dispatcher.UIThread.Post(
				() => CenterRowInView(scrollTarget),
				DispatcherPriority.Background);
			EndSync();
		}

		// DataGrid.ScrollIntoView only brings the row to the nearest viewport edge; we want
		// the row centred so the user's eye lands on it. Skip the move when the row is already
		// fully in view — re-centring an in-view row would yank the viewport on every selection
		// (e.g. user clicks a visible row, or Ctrl+O selects a freshly-loaded top-level entry).
		void CenterRowInView(HierarchicalNode node)
		{
			var scrollViewer = TreeGrid.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer is null)
				return;

			// Cheap pre-check: if the row's already realised AND fully visible, leave the
			// viewport alone. ScrollIntoView (next call) would otherwise drag it to the edge,
			// and our centring step would then drag it again.
			var existingRow = TreeGrid.GetVisualDescendants().OfType<DataGridRow>()
				.FirstOrDefault(r => ReferenceEquals(r.DataContext, node));
			if (existingRow is { IsVisible: true }
				&& existingRow.TranslatePoint(new Point(0, 0), scrollViewer) is { } existingTop
				&& existingTop.Y >= 0
				&& existingTop.Y + existingRow.Bounds.Height <= scrollViewer.Viewport.Height)
				return;

			TreeGrid.ScrollIntoView(node, TreeGrid.Columns[0]);
			// ScrollIntoView only changes ScrollViewer.Offset; the row becomes a realised
			// DataGridRow during the next layout pass. Force it now so we can read the row's
			// bounds for centring.
			TreeGrid.UpdateLayout();

			var row = TreeGrid.GetVisualDescendants().OfType<DataGridRow>()
				.FirstOrDefault(r => ReferenceEquals(r.DataContext, node));
			if (row is null)
				return;

			var rowTopInViewer = row.TranslatePoint(new Point(0, 0), scrollViewer);
			if (rowTopInViewer is null)
				return;

			var desiredTop = (scrollViewer.Viewport.Height - row.Bounds.Height) / 2;
			var newOffsetY = scrollViewer.Offset.Y + (rowTopInViewer.Value.Y - desiredTop);
			var maxOffset = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
			newOffsetY = Math.Clamp(newOffsetY, 0, maxOffset);
			scrollViewer.Offset = new Vector(scrollViewer.Offset.X, newOffsetY);
		}

		void BeginSync() => syncingSelection = true;

		// Released on a Background dispatch tick so any DataGrid SelectionChanged the Expand /
		// SelectedItem / ScrollIntoView calls trigger doesn't bounce back into the model.
		void EndSync() => Dispatcher.UIThread.Post(
			() => syncingSelection = false,
			DispatcherPriority.Background);


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
