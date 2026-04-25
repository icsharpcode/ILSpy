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

using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;

using ICSharpCode.ILSpyX.TreeView;

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		public AssemblyListPane()
		{
			InitializeComponent();
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
				// Force lazy children to load BEFORE returning the collection. ProDataGrid
				// queries ChildrenSelector while expanding, before propagating IsExpanded to
				// the source via IsExpandedSetter, so SharpTreeNode.LazyLoading wouldn't have
				// triggered yet otherwise -- and an empty Children collection causes the grid
				// to revert the expansion immediately.
				ChildrenSelector = node => {
					node.EnsureLazyChildren();
					return node.Children;
				},
				IsLeafSelector = node => !node.ShowExpander,
				IsExpandedSelector = node => node.IsExpanded,
				IsExpandedSetter = (node, val) => node.IsExpanded = val,
				AutoExpandRoot = true,
			};

			var hierarchicalModel = new HierarchicalModel<SharpTreeNode>(options);
			hierarchicalModel.SetRoots((IEnumerable)root.Children);

			TreeGrid.HierarchicalModel = hierarchicalModel;
		}
	}
}
