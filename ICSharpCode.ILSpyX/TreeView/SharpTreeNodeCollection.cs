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
using System.Diagnostics;
using System.Linq;

#nullable disable

namespace ICSharpCode.ILSpyX.TreeView
{
	/// <summary>
	/// Collection that validates that inserted nodes do not have another parent.
	/// </summary>
	public sealed class SharpTreeNodeCollection : IList<SharpTreeNode>, INotifyCollectionChanged
	{
		readonly SharpTreeNode parent;
		List<SharpTreeNode> list = new List<SharpTreeNode>();
		bool isRaisingEvent;

		public SharpTreeNodeCollection(SharpTreeNode parent)
		{
			this.parent = parent;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			Debug.Assert(!isRaisingEvent);
			isRaisingEvent = true;
			try
			{
				parent.OnChildrenChanged(e);
				CollectionChanged?.Invoke(this, e);
			}
			finally
			{
				isRaisingEvent = false;
			}
		}

		void ThrowOnReentrancy()
		{
			if (isRaisingEvent)
				throw new InvalidOperationException();
		}

		void ThrowIfValueIsNullOrHasParent(SharpTreeNode node)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (node.modelParent != null)
				throw new ArgumentException("The node already has a parent", "node");
		}

		public SharpTreeNode this[int index] {
			get {
				return list[index];
			}
			set {
				ThrowOnReentrancy();
				var oldItem = list[index];
				if (oldItem == value)
					return;
				ThrowIfValueIsNullOrHasParent(value);
				list[index] = value;
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem, index));
			}
		}

		public int Count {
			get { return list.Count; }
		}

		bool ICollection<SharpTreeNode>.IsReadOnly {
			get { return false; }
		}

		public int IndexOf(SharpTreeNode node)
		{
			if (node == null || node.modelParent != parent)
				return -1;
			else
				return list.IndexOf(node);
		}

		public void Insert(int index, SharpTreeNode node)
		{
			ThrowOnReentrancy();
			ThrowIfValueIsNullOrHasParent(node);
			list.Insert(index, node);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, index));
		}

		public void InsertRange(int index, IEnumerable<SharpTreeNode> nodes)
		{
			if (nodes == null)
				throw new ArgumentNullException("nodes");
			ThrowOnReentrancy();
			List<SharpTreeNode> newNodes = nodes.ToList();
			if (newNodes.Count == 0)
				return;
			foreach (SharpTreeNode node in newNodes)
			{
				ThrowIfValueIsNullOrHasParent(node);
			}
			list.InsertRange(index, newNodes);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newNodes, index));
		}

		public void RemoveAt(int index)
		{
			ThrowOnReentrancy();
			var oldItem = list[index];
			list.RemoveAt(index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItem, index));
		}

		public void RemoveRange(int index, int count)
		{
			ThrowOnReentrancy();
			if (count == 0)
				return;
			var oldItems = list.GetRange(index, count);
			list.RemoveRange(index, count);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, index));
		}

		public void Add(SharpTreeNode node)
		{
			ThrowOnReentrancy();
			ThrowIfValueIsNullOrHasParent(node);
			list.Add(node);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, node, list.Count - 1));
		}

		public void AddRange(IEnumerable<SharpTreeNode> nodes)
		{
			InsertRange(this.Count, nodes);
		}

		public void Clear()
		{
			ThrowOnReentrancy();
			var oldList = list;
			list = new List<SharpTreeNode>();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldList, 0));
		}

		public bool Contains(SharpTreeNode node)
		{
			return IndexOf(node) >= 0;
		}

		public void CopyTo(SharpTreeNode[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public bool Remove(SharpTreeNode item)
		{
			int pos = IndexOf(item);
			if (pos >= 0)
			{
				RemoveAt(pos);
				return true;
			}
			else
			{
				return false;
			}
		}

		public IEnumerator<SharpTreeNode> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void RemoveAll(Predicate<SharpTreeNode> match)
		{
			if (match == null)
				throw new ArgumentNullException("match");
			ThrowOnReentrancy();
			int firstToRemove = 0;
			for (int i = 0; i < list.Count; i++)
			{
				bool removeNode;
				isRaisingEvent = true;
				try
				{
					removeNode = match(list[i]);
				}
				finally
				{
					isRaisingEvent = false;
				}
				if (!removeNode)
				{
					if (firstToRemove < i)
					{
						RemoveRange(firstToRemove, i - firstToRemove);
						i = firstToRemove - 1;
					}
					else
					{
						firstToRemove = i + 1;
					}
					Debug.Assert(firstToRemove == i + 1);
				}
			}
			if (firstToRemove < list.Count)
			{
				RemoveRange(firstToRemove, list.Count - firstToRemove);
			}
		}
	}
}
