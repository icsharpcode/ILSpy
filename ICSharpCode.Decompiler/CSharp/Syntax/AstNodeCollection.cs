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
using System.Diagnostics;

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
		readonly CSharpSlotInfo kind;
		// Allocated lazily on first Add: a collection slot that is accessed but stays empty (the common
		// case for Attributes, TypeArguments, constraints, ...) then costs only this wrapper, not a List.
		List<T>? list;

		// When this is the parent's only collection and its last slot, the collection occupies the
		// contiguous flattened range [baseIndex, baseIndex + Count) with nothing after it, so an element's
		// flattened childIndex is exactly baseIndex + its local position. That lets mutations maintain
		// childIndex incrementally (no full re-index) and IndexOf run in O(1). Other shapes (multiple
		// collections, or a slot following this one) fall back to invalidate-and-rebuild.
		readonly int baseIndex;
		readonly bool supportsIncremental;

		public AstNodeCollection(AstNode parent, CSharpSlotInfo kind)
			: this(parent, kind, 0, false)
		{
		}

		public AstNodeCollection(AstNode parent, CSharpSlotInfo kind, int baseIndex, bool supportsIncremental)
		{
			this.parent = parent ?? throw new ArgumentNullException(nameof(parent));
			this.kind = kind;
			this.baseIndex = baseIndex;
			this.supportsIncremental = supportsIncremental;
		}

		// Reassigns the flattened childIndex of every element from 'start' to the end after a shift,
		// keeping the parent's indices valid without a full rebuild. Only meaningful on the incremental
		// fast-path (the collection occupies [baseIndex, baseIndex + Count) with nothing after it).
		void ReindexFrom(int start)
		{
			for (int i = start; i < list!.Count; i++)
				list[i].childIndex = baseIndex + i;
		}

		public int Count {
			get { return list?.Count ?? 0; }
		}

		private protected override int NodeCount => list?.Count ?? 0;
		private protected override INode NodeAt(int index) => list![index];

		public T this[int index] {
			get { return list![index]; }
			set {
				T old = list![index];
				if (old == value)
					return;
				ValidateNewChild(value);
				int oldChildIndex = old.childIndex;
				old.ClearParentAndIndex();
				list[index] = value;
				value.SetParent(parent);
				// Replacing an element in place keeps its flattened position, so no index changes: carry
				// the old element's index to the new one rather than invalidating (and rebuilding) the
				// parent's whole index set. A stale carried value is corrected by the next renumber.
				value.childIndex = oldChildIndex;
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
			(list ??= new List<T>()).Add(element);
			element.SetParent(parent);
			// Appending leaves every existing index unchanged, so just index the new element instead of
			// invalidating; otherwise fall back to a rebuild.
			if (supportsIncremental && parent.ChildIndicesValid)
				element.childIndex = baseIndex + list.Count - 1;
			else
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
			foreach (T node in list?.ToArray() ?? Array.Empty<T>())
			{
				node.Remove();
				targetCollection.Add(node);
			}
		}

		public bool Contains(T element)
		{
			return element != null && element.Parent == parent && list != null && list.Contains(element);
		}

		public int IndexOf(T element)
		{
			if (element == null || element.Parent != parent)
				return -1;
			// O(1) when the indices are current and this collection owns a contiguous flattened range:
			// the element's local position is its childIndex minus our base.
			if (supportsIncremental && parent.ChildIndicesValid && list != null)
			{
				int local = element.childIndex - baseIndex;
				if (local >= 0 && local < list.Count && ReferenceEquals(list[local], element))
					return local;
			}
			return list?.IndexOf(element) ?? -1;
		}

		public bool Remove(T element)
		{
			int index = IndexOf(element);
			if (index < 0)
				return false;
			bool incremental = supportsIncremental && parent.ChildIndicesValid;
			list!.RemoveAt(index);
			element.ClearParentAndIndex();
			// The elements after the removed one shifted down by one; renumber just them rather than
			// invalidating (and later rebuilding) the parent's whole index set.
			if (incremental)
				ReindexFrom(index);
			else
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
			list?.CopyTo(array, arrayIndex);
		}

		public void Clear()
		{
			if (list == null)
				return;
			foreach (T item in list)
				item.ClearParentAndIndex();
			list.Clear();
			parent.InvalidateChildIndices();
		}

		public IEnumerable<T> Detach()
		{
			T[] items = list?.ToArray() ?? Array.Empty<T>();
			Clear();
			return items;
		}

		/// <summary>
		/// Returns the first element for which the predicate returns true,
		/// or null if no such object is found.
		/// </summary>
		public T? FirstOrNull(Func<T, bool>? predicate = null)
		{
			if (list != null)
				foreach (T item in list)
					if (predicate == null || predicate(item))
						return item;
			return null;
		}

		/// <summary>
		/// Returns the last element for which the predicate returns true,
		/// or null if no such object is found.
		/// </summary>
		public T? LastOrNull(Func<T, bool>? predicate = null)
		{
			T? result = null;
			if (list != null)
				foreach (T item in list)
					if (predicate == null || predicate(item))
						result = item;
			return result;
		}

		bool ICollection<T>.IsReadOnly {
			get { return false; }
		}

		// Returned by value, so a foreach over a concrete collection allocates nothing; only enumeration
		// through the IEnumerable<T> interface (e.g. LINQ) boxes it.
		public Enumerator GetEnumerator() => new Enumerator(list);

		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(list);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(list);

		/// <summary>
		/// Enumerates the collection while tolerating removal or replacement of the current element during
		/// the loop body -- the behavior transforms that mutate a collection mid-foreach rely on. Before
		/// yielding an element it captures the following one, then resumes on that captured successor
		/// located by identity, so the walk follows node identity hand-over-hand like the old linked-list
		/// enumerator: removing or replacing the current element advances to the captured successor (a
		/// replacement is not re-visited), and inserting or removing other elements only shifts the
		/// backing-list indices, which the identity resume absorbs. The one mutation it cannot follow is
		/// removing the captured successor itself during the body -- that element is gone, so enumeration
		/// stops there (the linked-list enumerator lost track of it too).
		/// </summary>
		public struct Enumerator : IEnumerator<T>
		{
			readonly List<T>? list;
			int pos;        // index at which 'current' was found when it was yielded
			T? current;     // the element handed out by the last MoveNext
			T? next;        // the element that followed 'current' at the moment it was yielded
			bool started;

			internal Enumerator(List<T>? list)
			{
				this.list = list;
				this.pos = 0;
				this.current = null;
				this.next = null;
				this.started = false;
			}

			public readonly T Current => current!;
			readonly object? IEnumerator.Current => current;

			public bool MoveNext()
			{
				List<T>? list = this.list;
				if (list == null)
				{
					current = null;
					return false;
				}
				if (!started)
				{
					started = true;
					if (list.Count == 0)
						return false;
					pos = 0;
				}
				else
				{
					if (next == null)
					{
						current = null;
						return false;
					}
					// Resume on the successor captured before the body ran, located by identity. The two
					// fast paths cover the common shapes -- the successor still sits at pos+1, or 'current'
					// was removed so the successor dropped into pos -- and any other mutation falls back to
					// an O(n) identity search. Each fast path is guarded by ReferenceEquals(next), so it can
					// only fire when the successor genuinely sits there; landing the cursor anywhere else
					// would be the only way to skip or re-yield an element.
					int resume;
					if (pos + 1 < list.Count && ReferenceEquals(list[pos + 1], next))
						resume = pos + 1;
					else if (pos < list.Count && ReferenceEquals(list[pos], next))
						resume = pos;
					else
						resume = list.IndexOf(next);
					// Runtime guard against the cursor mislocating: the chosen index must equal the
					// authoritative identity position (also -1 == -1 when the successor was removed). It
					// holds for every mutation the body can make today, and the decompiler test suite runs
					// it after every transform, so a future change that lets the cursor skip or re-yield an
					// element fails loudly here instead of silently corrupting the output.
					Debug.Assert(resume == list.IndexOf(next),
						"AstNodeCollection enumerator lost track of its position during a mid-enumeration mutation.");
					if (resume < 0)
					{
						current = null;
						next = null;
						return false;
					}
					pos = resume;
				}
				current = list[pos];
				next = pos + 1 < list.Count ? list[pos + 1] : null;
				return true;
			}

			public void Reset()
			{
				pos = 0;
				current = null;
				next = null;
				started = false;
			}

			public readonly void Dispose() { }
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
			Insert(index < 0 ? Count : index, newItem);
		}

		void Insert(int index, T newItem)
		{
			if (newItem == null)
				return;
			ValidateNewChild(newItem);
			bool incremental = supportsIncremental && parent.ChildIndicesValid;
			(list ??= new List<T>()).Insert(index, newItem);
			newItem.SetParent(parent);
			// The new element and everything after it occupy fresh positions; renumber from the insertion
			// point rather than invalidating the parent's whole index set.
			if (incremental)
				ReindexFrom(index);
			else
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
