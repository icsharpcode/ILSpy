// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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

using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace ICSharpCode.TreeView
{
	class SharpTreeViewItemAutomationPeer  : FrameworkElementAutomationPeer, IExpandCollapseProvider
	{
		internal SharpTreeViewItemAutomationPeer(SharpTreeViewItem owner)
			: base(owner)
		{
			SharpTreeViewItem.DataContextChanged += OnDataContextChanged;
			SharpTreeNode node = SharpTreeViewItem.DataContext as SharpTreeNode;
			if (node == null) return;
			
			node.PropertyChanged += OnPropertyChanged;
		}
		private SharpTreeViewItem  SharpTreeViewItem { get { return (SharpTreeViewItem)base.Owner; } }
		protected override AutomationControlType GetAutomationControlTypeCore()
		{
			return AutomationControlType.TreeItem;
		}

		public override object GetPattern(PatternInterface patternInterface)
		{
			if (patternInterface == PatternInterface.ExpandCollapse)
				return this;
			return base.GetPattern(patternInterface);
		}
		
		public void Collapse()
		{
		}

		public void Expand()
		{
		}

		public ExpandCollapseState ExpandCollapseState {
			get {
				SharpTreeNode node = SharpTreeViewItem.DataContext as SharpTreeNode;
				if (node == null || !node.ShowExpander)
					return ExpandCollapseState.LeafNode;
				return node.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
			}
		}
		
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsExpanded") return;
			SharpTreeNode node =  sender as SharpTreeNode;
			if (node == null ||  node.Children.Count == 0) return;
			bool newValue = node.IsExpanded;
			bool oldValue = !newValue;
			RaisePropertyChangedEvent(
				ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
				oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
				newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
		}
		
		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			SharpTreeNode oldNode = e.OldValue as SharpTreeNode;
			if (oldNode != null)
				oldNode.PropertyChanged -= OnPropertyChanged;
			SharpTreeNode newNode = e.NewValue as SharpTreeNode;
			if (newNode != null)
				newNode.PropertyChanged += OnPropertyChanged;
		}
	}
}
