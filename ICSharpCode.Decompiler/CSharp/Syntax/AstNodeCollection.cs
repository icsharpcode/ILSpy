// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Non-generic base of <see cref="AstNodeCollection{T}"/>, letting the <see cref="AstNode"/> base
	/// manipulate a collection slot (remove an element by reference) without knowing its element type.
	/// </summary>
	public abstract class AstNodeCollection
	{
		internal abstract void AddNode(AstNode node);
		internal abstract void InsertNodeBefore(AstNode? existing, AstNode node);
		internal abstract void InsertNodeAfter(AstNode? existing, AstNode node);
		internal abstract bool RemoveNode(AstNode node);

		// The pattern matcher consumes a collection as an IReadOnlyList<INode>. The view is a separate
		// (cached) object rather than the collection itself, because having AstNodeCollection<T>
		// implement IEnumerable<INode> as well as IEnumerable<T> would make every LINQ call on a typed
		// collection ambiguous.
		private protected abstract int NodeCount { get; }
		private protected abstract INode NodeAt(int index);

		IReadOnlyList<INode>? nodeListView;
		internal IReadOnlyList<INode> AsNodeList() => nodeListView ??= new NodeListView(this);

		sealed class NodeListView : IReadOnlyList<INode>
		{
			readonly AstNodeCollection collection;
			public NodeListView(AstNodeCollection collection) => this.collection = collection;
			public INode this[int index] => collection.NodeAt(index);
			public int Count => collection.NodeCount;
			public IEnumerator<INode> GetEnumerator()
			{
				int count = collection.NodeCount;
				for (int i = 0; i < count; i++)
					yield return collection.NodeAt(i);
			}
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
	}

	/// <summary>
	/// Represents the children of an <see cref="AstNode"/> that occupy one collection slot.
	/// The elements are stored in a list owned by the collection; the parent's flattened child-index
	/// space contains them as a contiguous run, renumbered lazily by the parent after a mutation.
	/// </summary>
	public class AstNodeCollection<T> : AstNodeCollection, ICollection<T>, IReadOnlyList<T>
		where T : AstNode
	{
		readonly AstNode parent;
		readonly SlotKind kind;
		readonly List<T> list = new List<T>();

		public AstNodeCollection(AstNode parent, SlotKind kind)
		{
			this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
			this.kind = kind;
		}

		public int Count {
			get { return list.Count; }
		}

		private protected override int NodeCount => list.Count;
		private protected override INode NodeAt(int index) => list[index];

		public T this[int index] {
			get { return list[index]; }
			set {
				T old = list[index];
				if (old == value)
					return;
				ValidateNewChild(value);
				old.ClearParentAndIndex();
				list[index] = value;
				value.SetParent(parent);
				parent.InvalidateChildIndices();
			}
		}

		void ValidateNewChild(T child)
		{
			if (child == null)
				throw new ArgumentNullException(nameof(child));
			if (child == parent)
				throw new ArgumentException("Cannot add a node to itself as a child.", nameof(child));
			if (child.Parent != null)
				throw new ArgumentException("Node is already used in another tree.", nameof(child));
		}

		public void Add(T element)
		{
			if (element == null)
				return;
			ValidateNewChild(element);
			list.Add(element);
			element.SetParent(parent);
			parent.InvalidateChildIndices();
		}

		public void AddRange(IEnumerable<T> nodes)
		{
			// Evaluate 'nodes' first, since it might change when we add the new children
			// Example: collection.AddRange(collection);
			if (nodes != null)
			{
				foreach (T node in nodes is ICollection<T> ? nodes : new List<T>(nodes))
					Add(node);
			}
		}

		public void AddRange(T[] nodes)
		{
			// Fast overload for arrays - we don't need to create a copy
			if (nodes != null)
			{
				foreach (T node in nodes)
					Add(node);
			}
		}

		public void ReplaceWith(IEnumerable<T> nodes)
		{
			// Evaluate 'nodes' first, since it might change when we call Clear()
			// Example: collection.ReplaceWith(collection);
			List<T>? newNodes = nodes != null ? new List<T>(nodes) : null;
			Clear();
			if (newNodes != null)
			{
				foreach (T node in newNodes)
					Add(node);
			}
		}

		public void MoveTo(ICollection<T> targetCollection)
		{
			if (targetCollection == null)
				throw new ArgumentNullException(nameof(targetCollection));
			foreach (T node in list.ToArray())
			{
				node.Remove();
				targetCollection.Add(node);
			}
		}

		public bool Contains(T element)
		{
			return element != null && element.Parent == parent && list.Contains(element);
		}

		public int IndexOf(T element)
		{
			if (element == null || element.Parent != parent)
				return -1;
			return list.IndexOf(element);
		}

		public bool Remove(T element)
		{
			int index = IndexOf(element);
			if (index < 0)
				return false;
			list.RemoveAt(index);
			element.ClearParentAndIndex();
			parent.InvalidateChildIndices();
			return true;
		}

		internal override void AddNode(AstNode node)
		{
			Add((T)node);
		}

		internal override void InsertNodeBefore(AstNode? existing, AstNode node)
		{
			// 'existing' may belong to a different slot (e.g. the node after the last collection
			// element); in that case it is not found here and the new node is appended.
			if (existing is T e && IndexOf(e) >= 0)
				InsertBefore(e, (T)node);
			else
				Add((T)node);
		}

		internal override void InsertNodeAfter(AstNode? existing, AstNode node)
		{
			if (existing is T e && IndexOf(e) >= 0)
				InsertAfter(e, (T)node);
			else
				Insert(0, (T)node);
		}

		internal override bool RemoveNode(AstNode node)
		{
			return node is T typed && Remove(typed);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public void Clear()
		{
			foreach (T item in list)
				item.ClearParentAndIndex();
			list.Clear();
			parent.InvalidateChildIndices();
		}

		public IEnumerable<T> Detach()
		{
			T[] items = list.ToArray();
			Clear();
			return items;
		}

		/// <summary>
		/// Returns the first element for which the predicate returns true,
		/// or null if no such object is found.
		/// </summary>
		public T FirstOrNull(Func<T, bool>? predicate = null)
		{
			foreach (T item in list)
				if (predicate == null || predicate(item))
					return item;
			return null!;
		}

		/// <summary>
		/// Returns the last element for which the predicate returns true,
		/// or null if no such object is found.
		/// </summary>
		public T LastOrNull(Func<T, bool>? predicate = null)
		{
			T result = null!;
			foreach (T item in list)
				if (predicate == null || predicate(item))
					result = item;
			return result;
		}

		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

		// Tolerates removing or replacing the current element during enumeration (the common
		// transform pattern), matching the previous linked-list collection's behavior.
		public IEnumerator<T> GetEnumerator()
		{
			int pos = 0;
			while (pos < list.Count)
			{
				T cur = list[pos];
				yield return cur;
				if (pos < list.Count && ReferenceEquals(list[pos], cur))
					pos++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#region Equals and GetHashCode implementation
		public override int GetHashCode()
		{
			return parent.GetHashCode() ^ kind.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			return obj is AstNodeCollection<T> other && this.parent == other.parent && this.kind == other.kind;
		}
		#endregion

		internal bool DoMatch(AstNodeCollection<T> other, Match match)
		{
			// Both collections are already the per-slot child lists, so the matcher walks them by index.
			return Pattern.DoMatchCollection(AsNodeList(), other.AsNodeList(), match);
		}

		public void InsertAfter(T? existingItem, T newItem)
		{
			// A null existingItem yields IndexOf == -1, so the new item is inserted at the front.
			Insert(IndexOf(existingItem!) + 1, newItem);
		}

		public void InsertBefore(T? existingItem, T newItem)
		{
			// A null existingItem yields IndexOf == -1, so the new item is appended.
			int index = IndexOf(existingItem!);
			Insert(index < 0 ? list.Count : index, newItem);
		}

		void Insert(int index, T newItem)
		{
			if (newItem == null)
				return;
			ValidateNewChild(newItem);
			list.Insert(index, newItem);
			newItem.SetParent(parent);
			parent.InvalidateChildIndices();
		}

		/// <summary>
		/// Applies the <paramref name="visitor"/> to all nodes in this collection.
		/// </summary>
		public void AcceptVisitor(IAstVisitor visitor)
		{
			foreach (T item in this)
				item.AcceptVisitor(visitor);
		}
	}
}
