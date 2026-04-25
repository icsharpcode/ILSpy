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

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		public AssemblyListPane()
		{
			InitializeComponent();
			TreeGrid.DoubleTapped += OnTreeGridDoubleTapped;
		}

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
					BindTree(model.Root);
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AssemblyTreeModel.Root) && sender is AssemblyTreeModel model && model.Root != null)
				BindTree(model.Root);
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
