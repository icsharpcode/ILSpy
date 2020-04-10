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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ICSharpCode.TreeView
{
	sealed class TreeFlattener : IList, INotifyCollectionChanged
	{
		/// <summary>
		/// The root node of the flat list tree.
		/// Tjis is not necessarily the root of the model!
		/// </summary>
		internal SharpTreeNode root;
		readonly bool includeRoot;
		readonly object syncRoot = new object();
		
		public TreeFlattener(SharpTreeNode modelRoot, bool includeRoot)
		{
			this.root = modelRoot;
			while (root.listParent != null)
				root = root.listParent;
			root.treeFlattener = this;
			this.includeRoot = includeRoot;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		
		public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}
		
		public void NodesInserted(int index, IEnumerable<SharpTreeNode> nodes)
		{
			if (!includeRoot) index--;
			foreach (SharpTreeNode node in nodes) {
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, index++));
			}
		}
		
		public void NodesRemoved(int index, IEnumerable<SharpTreeNode> nodes)
		{
			if (!includeRoot) index--;
			foreach (SharpTreeNode node in nodes) {
				RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, node, index));
			}
		}
		
		public void Stop()
		{
			Debug.Assert(root.treeFlattener == this);
			root.treeFlattener = null;
		}
		
		public object this[int index] {
			get {
				if (index < 0 || index >= this.Count)
					throw new ArgumentOutOfRangeException();
				return SharpTreeNode.GetNodeByVisibleIndex(root, includeRoot ? index : index + 1);
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		public int Count {
			get {
				return includeRoot ? root.GetTotalListLength() : root.GetTotalListLength() - 1;
			}
		}
		
		public int IndexOf(object item)
		{
			SharpTreeNode node = item as SharpTreeNode;
			if (node != null && node.IsVisible && node.GetListRoot() == root) {
				if (includeRoot)
					return SharpTreeNode.GetVisibleIndexForNode(node);
				else
					return SharpTreeNode.GetVisibleIndexForNode(node) - 1;
			} else {
				return -1;
			}
		}
		
		bool IList.IsReadOnly {
			get { return true; }
		}
		
		bool IList.IsFixedSize {
			get { return false; }
		}
		
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		
		object ICollection.SyncRoot {
			get {
				return syncRoot;
			}
		}
		
		void IList.Insert(int index, object item)
		{
			throw new NotSupportedException();
		}
		
		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
		
		int IList.Add(object item)
		{
			throw new NotSupportedException();
		}
		
		void IList.Clear()
		{
			throw new NotSupportedException();
		}
		
		public bool Contains(object item)
		{
			return IndexOf(item) >= 0;
		}
		
		public void CopyTo(Array array, int arrayIndex)
		{
			foreach (object item in this)
				array.SetValue(item, arrayIndex++);
		}
		
		void IList.Remove(object item)
		{
			throw new NotSupportedException();
		}
		
		public IEnumerator GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++) {
				yield return this[i];
			}
		}
	}
}
