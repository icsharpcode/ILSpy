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
using System.ComponentModel;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Compare
{
	public partial class CompareView : UserControl
	{
		CompareTabPageModel? boundModel;

		public CompareView()
		{
			InitializeComponent();
		}

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			DetachFromModel();
			if (DataContext is CompareTabPageModel model)
				AttachToModel(model);
		}

		void AttachToModel(CompareTabPageModel model)
		{
			boundModel = model;
			model.PropertyChanged += OnModelPropertyChanged;
			Rebind();
		}

		void DetachFromModel()
		{
			if (boundModel == null)
				return;
			boundModel.PropertyChanged -= OnModelPropertyChanged;
			boundModel = null;
		}

		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// ShowIdentical toggles which rows are filtered out — re-running the cascade
			// through Rebind makes the DataGrid re-evaluate visibility on the next layout
			// pass without us having to walk the tree by hand.
			if (e.PropertyName == nameof(CompareTabPageModel.ShowIdentical))
				Rebind();
		}

		void Rebind()
		{
			var root = boundModel?.RootEntry;
			if (root == null)
				return;
			root.EnsureChildrenFiltered();

			var options = new HierarchicalOptions<SharpTreeNode> {
				ChildrenSelector = node => {
					if (node is TreeNodes.ILSpyTreeNode ilspy)
						ilspy.EnsureChildrenFiltered();
					else
						node.EnsureLazyChildren();
					return FilterHidden(node.Children);
				},
				IsLeafSelector = node => !node.ShowExpander,
				VirtualizeChildren = false,
				IsExpandedPropertyPath = nameof(SharpTreeNode.IsExpanded),
			};
			var model = new HierarchicalModel<SharpTreeNode>(options);
			model.SetRoots(FilterHidden(root.Children));
			DiffGrid.HierarchicalModel = model;
		}

		// ShowIdentical=false hides identical rows by setting IsHidden=true via
		// ComparisonEntryTreeNode.Filter, but the DataGrid's HierarchicalModel only sees
		// what ChildrenSelector / SetRoots feed it — IsHidden alone has no effect on the
		// rendered row set. Filter the children here so the cascade-set IsHidden flags
		// actually reach the grid. Same pattern AssemblyListPane.FilterChildren uses for
		// the assembly tree's ShowApiLevel cascade.
		static System.Collections.Generic.IEnumerable<SharpTreeNode> FilterHidden(System.Collections.Generic.IEnumerable<SharpTreeNode> children)
			=> children.Where(c => c is not TreeNodes.ILSpyTreeNode it || !it.IsHidden).ToList();
	}
}
