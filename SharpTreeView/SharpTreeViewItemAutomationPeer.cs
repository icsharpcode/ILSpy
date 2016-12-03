using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
