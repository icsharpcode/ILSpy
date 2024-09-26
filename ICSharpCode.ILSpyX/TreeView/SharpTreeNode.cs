﻿// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Linq;

#nullable disable

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpyX.TreeView
{
	public partial class SharpTreeNode : INotifyPropertyChanged
	{
		protected static ITreeNodeImagesProvider ImagesProvider { get; private set; }
		public static void SetImagesProvider(ITreeNodeImagesProvider provider) => ImagesProvider = provider;

		SharpTreeNodeCollection modelChildren;
		internal SharpTreeNode modelParent;
		bool isVisible = true;

		void UpdateIsVisible(bool parentIsVisible, bool updateFlattener)
		{
			bool newIsVisible = parentIsVisible && !isHidden;
			if (isVisible != newIsVisible)
			{
				isVisible = newIsVisible;

				// invalidate the augmented data
				SharpTreeNode node = this;
				while (node != null && node.totalListLength >= 0)
				{
					node.totalListLength = -1;
					node = node.listParent;
				}
				// Remember the removed nodes:
				List<SharpTreeNode> removedNodes = null;
				if (updateFlattener && !newIsVisible)
				{
					removedNodes = VisibleDescendantsAndSelf().ToList();
				}
				// also update the model children:
				UpdateChildIsVisible(false);

				// Validate our invariants:
				if (updateFlattener)
					CheckRootInvariants();

				// Tell the flattener about the removed nodes:
				if (removedNodes != null)
				{
					var flattener = GetListRoot().treeFlattener;
					if (flattener != null)
					{
						flattener.NodesRemoved(GetVisibleIndexForNode(this), removedNodes);
						foreach (var n in removedNodes)
							n.OnIsVisibleChanged();
					}
				}
				// Tell the flattener about the new nodes:
				if (updateFlattener && newIsVisible)
				{
					var flattener = GetListRoot().treeFlattener;
					if (flattener != null)
					{
						flattener.NodesInserted(GetVisibleIndexForNode(this), VisibleDescendantsAndSelf());
						foreach (var n in VisibleDescendantsAndSelf())
							n.OnIsVisibleChanged();
					}
				}
			}
		}

		protected virtual void OnIsVisibleChanged() { }

		void UpdateChildIsVisible(bool updateFlattener)
		{
			if (modelChildren != null && modelChildren.Count > 0)
			{
				bool showChildren = isVisible && isExpanded;
				foreach (SharpTreeNode child in modelChildren)
				{
					child.UpdateIsVisible(showChildren, updateFlattener);
				}
			}
		}

		#region Main

		public SharpTreeNode()
		{
		}

		public SharpTreeNodeCollection Children {
			get {
				if (modelChildren == null)
					modelChildren = new SharpTreeNodeCollection(this);
				return modelChildren;
			}
		}

		public SharpTreeNode Parent {
			get { return modelParent; }
		}

		public virtual object Text {
			get { return null; }
		}

		public virtual object Icon {
			get { return null; }
		}

		public virtual object ToolTip {
			get { return null; }
		}

		public int Level {
			get { return Parent != null ? Parent.Level + 1 : 0; }
		}

		public bool IsRoot {
			get { return Parent == null; }
		}

		bool isHidden;

		public bool IsHidden {
			get { return isHidden; }
			set {
				if (isHidden != value)
				{
					isHidden = value;
					if (modelParent != null)
						UpdateIsVisible(modelParent.isVisible && modelParent.isExpanded, true);
					RaisePropertyChanged(nameof(IsHidden));
					if (Parent != null)
						Parent.RaisePropertyChanged(nameof(ShowExpander));
				}
			}
		}

		/// <summary>
		/// Return true when this node is not hidden and when all parent nodes are expanded and not hidden.
		/// </summary>
		public bool IsVisible {
			get { return isVisible; }
		}

		bool isSelected;

		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value)
				{
					isSelected = value;
					RaisePropertyChanged(nameof(IsSelected));
				}
			}
		}

		#endregion

		#region OnChildrenChanged
		public virtual void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (SharpTreeNode node in e.OldItems)
				{
					Debug.Assert(node.modelParent == this);
					node.modelParent = null;
					Debug.WriteLine("Removing {0} from {1}", node, this);
					SharpTreeNode removeEnd = node;
					while (removeEnd.modelChildren != null && removeEnd.modelChildren.Count > 0)
						removeEnd = removeEnd.modelChildren.Last();

					List<SharpTreeNode> removedNodes = null;
					int visibleIndexOfRemoval = 0;
					if (node.isVisible)
					{
						visibleIndexOfRemoval = GetVisibleIndexForNode(node);
						removedNodes = node.VisibleDescendantsAndSelf().ToList();
					}

					RemoveNodes(node, removeEnd);

					if (removedNodes != null)
					{
						var flattener = GetListRoot().treeFlattener;
						if (flattener != null)
						{
							flattener.NodesRemoved(visibleIndexOfRemoval, removedNodes);
						}
					}
				}
			}
			if (e.NewItems != null)
			{
				SharpTreeNode insertionPos;
				if (e.NewStartingIndex == 0)
					insertionPos = null;
				else
					insertionPos = modelChildren[e.NewStartingIndex - 1];

				foreach (SharpTreeNode node in e.NewItems)
				{
					Debug.Assert(node.modelParent == null);
					node.modelParent = this;
					node.UpdateIsVisible(isVisible && isExpanded, false);
					//Debug.WriteLine("Inserting {0} after {1}", node, insertionPos);

					while (insertionPos != null && insertionPos.modelChildren != null && insertionPos.modelChildren.Count > 0)
					{
						insertionPos = insertionPos.modelChildren.Last();
					}
					InsertNodeAfter(insertionPos ?? this, node);

					insertionPos = node;
					if (node.isVisible)
					{
						var flattener = GetListRoot().treeFlattener;
						if (flattener != null)
						{
							flattener.NodesInserted(GetVisibleIndexForNode(node), node.VisibleDescendantsAndSelf());
						}
					}
				}
			}

			RaisePropertyChanged(nameof(ShowExpander));
			RaiseIsLastChangedIfNeeded(e);
		}
		#endregion

		#region Expanding / LazyLoading

		public virtual object ExpandedIcon {
			get { return Icon; }
		}

		public virtual bool ShowExpander {
			get { return LazyLoading || Children.Any(c => !c.isHidden); }
		}

		bool isExpanded;

		public bool IsExpanded {
			get { return isExpanded; }
			set {
				if (isExpanded != value)
				{
					isExpanded = value;
					if (isExpanded)
					{
						EnsureLazyChildren();
						OnExpanding();
					}
					else
					{
						OnCollapsing();
					}
					UpdateChildIsVisible(true);
					RaisePropertyChanged(nameof(IsExpanded));
				}
			}
		}

		protected virtual void OnExpanding() { }
		protected virtual void OnCollapsing() { }

		bool lazyLoading;

		public bool LazyLoading {
			get { return lazyLoading; }
			set {
				lazyLoading = value;
				if (lazyLoading)
				{
					IsExpanded = false;
					if (canExpandRecursively)
					{
						canExpandRecursively = false;
						RaisePropertyChanged(nameof(CanExpandRecursively));
					}
				}
				RaisePropertyChanged(nameof(LazyLoading));
				RaisePropertyChanged(nameof(ShowExpander));
			}
		}

		bool canExpandRecursively = true;

		/// <summary>
		/// Gets whether this node can be expanded recursively.
		/// If not overridden, this property returns false if the node is using lazy-loading, and true otherwise.
		/// </summary>
		public virtual bool CanExpandRecursively {
			get { return canExpandRecursively; }
		}

		public virtual bool ShowIcon {
			get { return Icon != null; }
		}

		protected virtual void LoadChildren()
		{
			throw new NotSupportedException(GetType().Name + " does not support lazy loading");
		}

		/// <summary>
		/// Ensures the children were initialized (loads children if lazy loading is enabled)
		/// </summary>
		public void EnsureLazyChildren()
		{
			if (LazyLoading)
			{
				LazyLoading = false;
				LoadChildren();
			}
		}

		#endregion

		#region Ancestors / Descendants

		public IEnumerable<SharpTreeNode> Descendants()
		{
			return TreeTraversal.PreOrder(this.Children, n => n.Children);
		}

		public IEnumerable<SharpTreeNode> DescendantsAndSelf()
		{
			return TreeTraversal.PreOrder(this, n => n.Children);
		}

		internal IEnumerable<SharpTreeNode> VisibleDescendants()
		{
			return TreeTraversal.PreOrder(this.Children.Where(c => c.isVisible), n => n.Children.Where(c => c.isVisible));
		}

		public IEnumerable<SharpTreeNode> VisibleDescendantsAndSelf()
		{
			return TreeTraversal.PreOrder(this, n => n.Children.Where(c => c.isVisible));
		}

		public IEnumerable<SharpTreeNode> Ancestors()
		{
			for (SharpTreeNode n = this.Parent; n != null; n = n.Parent)
				yield return n;
		}

		public IEnumerable<SharpTreeNode> AncestorsAndSelf()
		{
			for (SharpTreeNode n = this; n != null; n = n.Parent)
				yield return n;
		}

		#endregion

		#region Editing

		public virtual bool IsEditable {
			get { return false; }
		}

		bool isEditing;

		public bool IsEditing {
			get { return isEditing; }
			set {
				if (isEditing != value)
				{
					isEditing = value;
					RaisePropertyChanged(nameof(IsEditing));
				}
			}
		}

		public virtual string LoadEditText()
		{
			return null;
		}

		public virtual bool SaveEditText(string value)
		{
			return true;
		}

		#endregion

		#region Checkboxes

		public virtual bool IsCheckable {
			get { return false; }
		}

		bool? isChecked;

		public bool? IsChecked {
			get { return isChecked; }
			set {
				SetIsChecked(value, true);
			}
		}

		void SetIsChecked(bool? value, bool update)
		{
			if (isChecked != value)
			{
				isChecked = value;

				if (update)
				{
					if (IsChecked != null)
					{
						foreach (var child in Descendants())
						{
							if (child.IsCheckable)
							{
								child.SetIsChecked(IsChecked, false);
							}
						}
					}

					foreach (var parent in Ancestors())
					{
						if (parent.IsCheckable)
						{
							if (!parent.TryValueForIsChecked(true))
							{
								if (!parent.TryValueForIsChecked(false))
								{
									parent.SetIsChecked(null, false);
								}
							}
						}
					}
				}

				RaisePropertyChanged(nameof(IsChecked));
			}
		}

		bool TryValueForIsChecked(bool? value)
		{
			if (Children.Where(n => n.IsCheckable).All(n => n.IsChecked == value))
			{
				SetIsChecked(value, false);
				return true;
			}
			return false;
		}

		#endregion

		#region Cut / Copy / Paste / Delete

		public bool IsCut { get { return false; } }
		/*
			static List<SharpTreeNode> cuttedNodes = new List<SharpTreeNode>();
			static IDataObject cuttedData;
			static EventHandler requerySuggestedHandler; // for weak event
	
			static void StartCuttedDataWatcher()
			{
				requerySuggestedHandler = new EventHandler(CommandManager_RequerySuggested);
				CommandManager.RequerySuggested += requerySuggestedHandler;
			}
	
			static void CommandManager_RequerySuggested(object sender, EventArgs e)
			{
				if (cuttedData != null && !Clipboard.IsCurrent(cuttedData)) {
					ClearCuttedData();
				}
			}
	
			static void ClearCuttedData()
			{
				foreach (var node in cuttedNodes) {
					node.IsCut = false;
				}
				cuttedNodes.Clear();
				cuttedData = null;
			}
	
			//static public IEnumerable<SharpTreeNode> PurifyNodes(IEnumerable<SharpTreeNode> nodes)
			//{
			//    var list = nodes.ToList();
			//    var array = list.ToArray();
			//    foreach (var node1 in array) {
			//        foreach (var node2 in array) {
			//            if (node1.Descendants().Contains(node2)) {
			//                list.Remove(node2);
			//            }
			//        }
			//    }
			//    return list;
			//}
	
			bool isCut;
	
			public bool IsCut
			{
				get { return isCut; }
				private set
				{
					isCut = value;
					RaisePropertyChanged("IsCut");
				}
			}
	
			internal bool InternalCanCut()
			{
				return InternalCanCopy() && InternalCanDelete();
			}
	
			internal void InternalCut()
			{
				ClearCuttedData();
				cuttedData = Copy(ActiveNodesArray);
				Clipboard.SetDataObject(cuttedData);
	
				foreach (var node in ActiveNodes) {
					node.IsCut = true;
					cuttedNodes.Add(node);
				}
			}
	
			internal bool InternalCanCopy()
			{
				return CanCopy(ActiveNodesArray);
			}
	
			internal void InternalCopy()
			{
				Clipboard.SetDataObject(Copy(ActiveNodesArray));
			}
	
			internal bool InternalCanPaste()
			{
				return CanPaste(Clipboard.GetDataObject());
			}
	
			internal void InternalPaste()
			{
				Paste(Clipboard.GetDataObject());
	
				if (cuttedData != null) {
					DeleteCore(cuttedNodes.ToArray());
					ClearCuttedData();
				}
			}
		 */

		public virtual bool CanDelete()
		{
			return false;
		}

		public virtual void Delete()
		{
			throw new NotSupportedException(GetType().Name + " does not support deletion");
		}

		public virtual void DeleteCore()
		{
			throw new NotSupportedException(GetType().Name + " does not support deletion");
		}

		public virtual IPlatformDataObject Copy(SharpTreeNode[] nodes)
		{
			throw new NotSupportedException(GetType().Name + " does not support copy/paste or drag'n'drop");
		}

		/*
			public virtual bool CanCopy(SharpTreeNode[] nodes)
			{
				return false;
			}
	
			public virtual bool CanPaste(IDataObject data)
			{
				return false;
			}
	
			public virtual void Paste(IDataObject data)
			{
				EnsureLazyChildren();
				Drop(data, Children.Count, DropEffect.Copy);
			}
		 */
		#endregion

		#region Drag and Drop
		public virtual bool CanDrag(SharpTreeNode[] nodes)
		{
			return false;
		}

		public virtual void StartDrag(object dragSource, SharpTreeNode[] nodes, IPlatformDragDrop dragdropManager)
		{
			XPlatDragDropEffects effects = XPlatDragDropEffects.All;
			if (!nodes.All(n => n.CanDelete()))
				effects &= ~XPlatDragDropEffects.Move;

			XPlatDragDropEffects result = dragdropManager.DoDragDrop(dragSource, Copy(nodes), effects);
			if (result == XPlatDragDropEffects.Move)
			{
				foreach (SharpTreeNode node in nodes)
					node.DeleteCore();
			}
		}

		public virtual bool CanDrop(IPlatformDragEventArgs e, int index)
		{
			return false;
		}

		public void InternalDrop(IPlatformDragEventArgs e, int index)
		{
			if (LazyLoading)
			{
				EnsureLazyChildren();
				index = Children.Count;
			}

			Drop(e, index);
		}

		public virtual void Drop(IPlatformDragEventArgs e, int index)
		{
			throw new NotSupportedException(GetType().Name + " does not support Drop()");
		}
		#endregion

		#region IsLast (for TreeView lines)

		public bool IsLast {
			get {
				return Parent == null
					|| Parent.Children.Count == 0
					|| Parent.Children[^1] == this;
			}
		}

		void RaiseIsLastChangedIfNeeded(NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewStartingIndex == Children.Count - 1)
					{
						if (Children.Count > 1)
						{
							Children[Children.Count - 2].RaisePropertyChanged(nameof(IsLast));
						}
						Children[Children.Count - 1].RaisePropertyChanged(nameof(IsLast));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					if (e.OldStartingIndex == Children.Count)
					{
						if (Children.Count > 0)
						{
							Children[Children.Count - 1].RaisePropertyChanged(nameof(IsLast));
						}
					}
					break;
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		#endregion

		/// <summary>
		/// Gets called when the item is double-clicked.
		/// </summary>
		public virtual void ActivateItem(IPlatformRoutedEventArgs e)
		{
		}

		/// <summary>
		/// Gets called when the item is clicked with the middle mouse button.
		/// </summary>
		public virtual void ActivateItemSecondary(IPlatformRoutedEventArgs e)
		{
		}

		public override string ToString()
		{
			// used for keyboard navigation
			object text = this.Text;
			return text != null ? text.ToString() : string.Empty;
		}
	}
}
