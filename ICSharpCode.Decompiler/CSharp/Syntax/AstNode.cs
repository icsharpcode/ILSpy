#nullable enable
// 
// AstNode.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode(hasPatternPlaceholder: true)]
	public abstract partial class AstNode : AbstractAnnotatable, INode, ICloneable
	{
		AstNode? parent;
		// Flattened index of this node within its parent's child-index space (-1 when unparented).
		// Recomputed lazily by the parent (EnsureChildIndices) after a structural mutation.
		internal int childIndex = -1;

		TextLocation startLocation = TextLocation.Empty;
		TextLocation endLocation = TextLocation.Empty;

		// Source locations are assigned at print time (the output visitor brackets every node with
		// StartNode/EndNode and InsertMissingTokensDecorator records the writer's position) rather than
		// computed by recursion to child token positions. A node's span runs from its first printed
		// token to its last; leaf nodes that print as a single token override these with their own value.
		public virtual TextLocation StartLocation {
			get {
				return startLocation;
			}
		}

		public virtual TextLocation EndLocation {
			get {
				return endLocation;
			}
		}

		internal void StorePrintStart(TextLocation value)
		{
			startLocation = value;
		}

		internal void StorePrintEnd(TextLocation value)
		{
			endLocation = value;
		}

		#region Trivia
		// Comments and preprocessor directives attached to this node, kept off the child-index space.
		// The holder lives in the annotation channel, so a node without trivia (the overwhelmingly
		// common case) costs nothing extra, and CloneAnnotations copies it for free.
		internal sealed class NodeTrivia : ICloneable
		{
			public List<Trivia>? Leading;
			public List<Trivia>? Trailing;

			public object Clone()
			{
				return new NodeTrivia {
					Leading = Leading?.Select(t => (Trivia)t.Clone()).ToList(),
					Trailing = Trailing?.Select(t => (Trivia)t.Clone()).ToList(),
				};
			}
		}

		/// <summary>Trivia printed before this node (in insertion order); empty when none.</summary>
		public IEnumerable<Trivia> LeadingTrivia {
			get { return Annotation<NodeTrivia>()?.Leading ?? Enumerable.Empty<Trivia>(); }
		}

		/// <summary>Trivia printed after this node (in insertion order); empty when none.</summary>
		public IEnumerable<Trivia> TrailingTrivia {
			get { return Annotation<NodeTrivia>()?.Trailing ?? Enumerable.Empty<Trivia>(); }
		}

		public void AddLeadingTrivia(Trivia trivia)
		{
			NodeTrivia holder = GetOrCreateTrivia();
			var list = holder.Leading ??= new List<Trivia>();
			InsertTrivia(list, list.Count, trivia);
		}

		/// <summary>
		/// Inserts trivia at the start of this node's leading trivia, before any already attached. Used for
		/// a file header comment that must precede generated leading directives such as <c>#define</c>.
		/// </summary>
		public void PrependLeadingTrivia(Trivia trivia)
		{
			NodeTrivia holder = GetOrCreateTrivia();
			InsertTrivia(holder.Leading ??= new List<Trivia>(), 0, trivia);
		}

		public void AddTrailingTrivia(Trivia trivia)
		{
			NodeTrivia holder = GetOrCreateTrivia();
			var list = holder.Trailing ??= new List<Trivia>();
			InsertTrivia(list, list.Count, trivia);
		}

		// The single mutation path for attaching trivia: validate, insert, then reindex from the
		// insertion point so every trivia's parent/list/index state is consistent for any position.
		void InsertTrivia(List<Trivia> list, int index, Trivia trivia)
		{
			ValidateNewTrivia(trivia);
			list.Insert(index, trivia);
			ReindexTrivia(this, list, index);
		}

		void ValidateNewTrivia(Trivia trivia)
		{
			if (trivia == null)
				throw new ArgumentNullException(nameof(trivia));
			if (trivia == this)
				throw new ArgumentException("Cannot add a node to itself as trivia.", nameof(trivia));
			if (trivia.parent != null)
				throw new ArgumentException("Node is already used in another tree.", nameof(trivia));
		}

		static void ReindexTrivia(AstNode parent, List<Trivia> list, int start)
		{
			for (int i = start; i < list.Count; i++)
				list[i].SetTriviaParent(parent, list, i);
		}

		// Points the trivia held in this node's annotation back at this node. Clone needs this:
		// the cloned NodeTrivia's deep-copied trivia still carry the source owner's parent state.
		void ReparentTrivia()
		{
			var holder = Annotation<NodeTrivia>();
			if (holder == null)
				return;
			if (holder.Leading != null)
				ReindexTrivia(this, holder.Leading, 0);
			if (holder.Trailing != null)
				ReindexTrivia(this, holder.Trailing, 0);
		}

		// Deep-copies another node's trivia onto this node, appending to any trivia already present.
		// The NodeTrivia holder must never be shared between nodes: each trivia's parent points at
		// its single owning node, so an annotation-copying operation clones the trivia instead.
		internal void CopyTriviaFrom(AstNode other)
		{
			var otherHolder = other.Annotation<NodeTrivia>();
			if (otherHolder == null)
				return;
			if (otherHolder.Leading != null)
			{
				foreach (var trivia in otherHolder.Leading)
					AddLeadingTrivia((Trivia)trivia.Clone());
			}
			if (otherHolder.Trailing != null)
			{
				foreach (var trivia in otherHolder.Trailing)
					AddTrailingTrivia((Trivia)trivia.Clone());
			}
		}

		NodeTrivia GetOrCreateTrivia()
		{
			NodeTrivia? holder = Annotation<NodeTrivia>();
			if (holder == null)
			{
				holder = new NodeTrivia();
				AddAnnotation(holder);
			}
			return holder;
		}
		#endregion

		public AstNode? Parent {
			get { return parent; }
		}

		public AstNode? NextSibling {
			get {
				if (parent == null)
					return null;
				if (this is Trivia { triviaSiblings: { } siblings })
					return childIndex + 1 < siblings.Count ? siblings[childIndex + 1] : null;
				// Inline the validity check: sibling navigation is one of the hottest operations, and the
				// indices are almost always already current, so skip the (non-inlinable) call when valid.
				if (!parent.childIndicesValid)
					parent.EnsureChildIndices();
				int count = parent.GetChildCount();
				for (int i = childIndex + 1; i < count; i++)
				{
					AstNode? c = parent.GetChild(i);
					if (c != null)
						return c;
				}
				return null;
			}
		}

		public AstNode? PrevSibling {
			get {
				if (parent == null)
					return null;
				if (this is Trivia { triviaSiblings: { } siblings })
					return childIndex > 0 ? siblings[childIndex - 1] : null;
				if (!parent.childIndicesValid)
					parent.EnsureChildIndices();
				for (int i = childIndex - 1; i >= 0; i--)
				{
					AstNode? c = parent.GetChild(i);
					if (c != null)
						return c;
				}
				return null;
			}
		}

		public AstNode? FirstChild {
			get {
				int count = GetChildCount();
				for (int i = 0; i < count; i++)
				{
					AstNode? c = GetChild(i);
					if (c != null)
						return c;
				}
				return null;
			}
		}

		public AstNode? LastChild {
			get {
				for (int i = GetChildCount() - 1; i >= 0; i--)
				{
					AstNode? c = GetChild(i);
					if (c != null)
						return c;
				}
				return null;
			}
		}

		public bool HasChildren {
			get {
				return FirstChild != null;
			}
		}

		/// <summary>
		/// The children of this node in document order. The collection and its enumerator are structs, so
		/// a <c>foreach</c> over <see cref="Children"/> allocates nothing; only enumeration through the
		/// <see cref="IEnumerable{T}"/> interface (e.g. LINQ) boxes the enumerator. The enumerator captures
		/// the successor before handing out each child, so the loop body may remove or replace the current
		/// child mid-traversal (the behavior the visitor and transforms rely on).
		/// </summary>
		public ChildrenCollection Children => new ChildrenCollection(this);

		public readonly struct ChildrenCollection : IReadOnlyList<AstNode>
		{
			readonly AstNode node;

			internal ChildrenCollection(AstNode node) => this.node = node;

			public ChildEnumerator GetEnumerator() => new ChildEnumerator(node);
			IEnumerator<AstNode> IEnumerable<AstNode>.GetEnumerator() => new ChildEnumerator(node);
			IEnumerator IEnumerable.GetEnumerator() => new ChildEnumerator(node);

			public int Count {
				get {
					int count = 0;
					for (ChildEnumerator e = GetEnumerator(); e.MoveNext();)
						count++;
					return count;
				}
			}

			public AstNode this[int index] {
				get {
					int i = 0;
					foreach (AstNode child in this)
					{
						if (i++ == index)
							return child;
					}
					throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
		}

		/// <summary>
		/// Zero-allocation enumerator over a node's children. It captures each child's successor before
		/// the child is handed out, so removing or replacing the current child in the loop body does not
		/// disturb the traversal -- the same guarantee the old linked-list enumerator gave.
		/// </summary>
		public struct ChildEnumerator : IEnumerator<AstNode>
		{
			readonly AstNode node;
			AstNode? current;
			AstNode? next;
			bool started;

			internal ChildEnumerator(AstNode node)
			{
				this.node = node;
				this.current = null;
				this.next = null;
				this.started = false;
			}

			public readonly AstNode Current => current!;
			readonly object IEnumerator.Current => current!;

			public bool MoveNext()
			{
				current = started ? next : node.FirstChild;
				started = true;
				if (current == null)
					return false;
				// Remember the successor before the child is yielded, so the loop body may remove or
				// replace 'current' without losing our place.
				next = current.NextSibling;
				return true;
			}

			public void Reset()
			{
				current = null;
				next = null;
				started = false;
			}

			public readonly void Dispose() { }
		}

		/// <summary>
		/// Gets the ancestors of this node (excluding this node itself)
		/// </summary>
		public IEnumerable<AstNode> Ancestors {
			get {
				for (AstNode? cur = parent; cur != null; cur = cur.parent)
				{
					yield return cur;
				}
			}
		}

		/// <summary>
		/// Gets the ancestors of this node (including this node itself)
		/// </summary>
		public IEnumerable<AstNode> AncestorsAndSelf {
			get {
				for (AstNode? cur = this; cur != null; cur = cur.parent)
				{
					yield return cur;
				}
			}
		}

		/// <summary>
		/// Gets all descendants of this node (excluding this node itself) in pre-order.
		/// </summary>
		public IEnumerable<AstNode> Descendants {
			get { return GetDescendantsImpl(false); }
		}

		/// <summary>
		/// Gets all descendants of this node (including this node itself) in pre-order.
		/// </summary>
		public IEnumerable<AstNode> DescendantsAndSelf {
			get { return GetDescendantsImpl(true); }
		}

		public IEnumerable<AstNode> DescendantNodes(Func<AstNode, bool>? descendIntoChildren = null)
		{
			return GetDescendantsImpl(false, descendIntoChildren);
		}

		public IEnumerable<AstNode> DescendantNodesAndSelf(Func<AstNode, bool>? descendIntoChildren = null)
		{
			return GetDescendantsImpl(true, descendIntoChildren);
		}

		IEnumerable<AstNode> GetDescendantsImpl(bool includeSelf, Func<AstNode, bool>? descendIntoChildren = null)
		{
			if (includeSelf)
			{
				yield return this;
				if (descendIntoChildren != null && !descendIntoChildren(this))
					yield break;
			}

			Stack<AstNode?> nextStack = new Stack<AstNode?>();
			nextStack.Push(null);
			AstNode? pos = FirstChild;
			while (pos != null)
			{
				// Remember next before yielding pos.
				// This allows removing/replacing nodes while iterating.
				AstNode? posNext = pos.NextSibling;
				if (posNext != null)
					nextStack.Push(posNext);
				yield return pos;
				AstNode? posFirstChild = pos.FirstChild;
				if (posFirstChild != null && (descendIntoChildren == null || descendIntoChildren(pos)))
					pos = posFirstChild;
				else
					pos = nextStack.Pop();
			}
		}

		/// <summary>
		/// Gets the first child occupying <paramref name="slot"/>, or null if the slot is empty (or the
		/// node declares no such slot). <paramref name="slot"/> is a canonical <c>Slots</c> kind; the
		/// result type is inferred from it.
		/// </summary>
		public T? GetChild<T>(CSharpSlotInfo<T> slot) where T : AstNode
		{
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				if (GetChildSlotInfo(i).Kind == slot)
				{
					return GetChild(i) as T;
				}
			}
			return null;
		}

		public T? GetParent<T>() where T : AstNode
		{
			return Ancestors.OfType<T>().FirstOrDefault();
		}

		public AstNode? GetParent(Func<AstNode, bool>? pred)
		{
			return pred != null ? Ancestors.FirstOrDefault(pred) : Ancestors.FirstOrDefault();
		}

		/// <summary>Gets the collection occupying <paramref name="slot"/>; the element type is inferred from the slot.</summary>
		public AstNodeCollection<T> GetChildren<T>(CSharpSlotInfo<T> slot) where T : AstNode
		{
			AstNodeCollection? collection = GetCollectionByKind(slot);
			// A node has no children of a kind it does not declare a collection slot for. Reads of such
			// a kind (e.g. the parameters of a non-indexer property) get a detached empty collection;
			// writes go through AddChild/SetChild, which reject a missing slot.
			return collection != null ? (AstNodeCollection<T>)collection : new AstNodeCollection<T>(this, slot);
		}

		/// <summary>Sets the single child occupying <paramref name="slot"/>; the child type is inferred from the slot.</summary>
		protected void SetChild<T>(CSharpSlotInfo<T> slot, T? newChild) where T : AstNode => SetChildByKindUntyped(slot, newChild);

		#region Slot storage contract
		// Each concrete node's slots form a flattened child-index space, in source-declaration order.
		// A single slot occupies exactly one index (even when empty, where GetChild returns null); a
		// collection slot occupies a contiguous run of its current length. The generator emits these
		// four members per node from its [Slot] partial-property declarations; nodes without children
		// (and the placeholder nodes) keep the zero-child defaults below.

		internal virtual int GetChildCount() => 0;

		internal virtual AstNode? GetChild(int index) => throw new ArgumentOutOfRangeException(nameof(index));

		internal virtual void SetChild(int index, AstNode? value) => throw new ArgumentOutOfRangeException(nameof(index));

		// The CSharpSlotInfo for the slot at the given flattened index; generator-emitted per leaf.
		internal virtual CSharpSlotInfo GetChildSlotInfo(int index) => throw new ArgumentOutOfRangeException(nameof(index));

		/// <summary>
		/// The slot this node occupies in its parent, or null if it has no parent. Compare against a
		/// node type's generated slot statics (e.g. <c>node.Slot == BinaryOperatorExpression.LeftSlot</c>).
		/// </summary>
		public CSharpSlotInfo? Slot {
			get {
				if (parent == null)
					return null;
				// Trivia occupies no child slot; its childIndex indexes its owning trivia list instead.
				if (this is Trivia { triviaSiblings: not null })
					return null;
				parent.EnsureChildIndices();
				return parent.GetChildSlotInfo(childIndex);
			}
		}

		// Returns the collection occupying the slot with the given kind, or null if there is none.
		// Overridden by the generator for nodes that have collection slots.
		internal virtual AstNodeCollection? GetCollectionByKind(CSharpSlotInfo kind) => null;

		// Deep-copies this node's children into the (memberwise-cloned) copy, which initially shares
		// this node's child references. Overridden by the generator for nodes that have slots.
		internal virtual void CloneChildrenInto(AstNode copy) { }

		// Whether this node's children currently carry correct flattened childIndex values. Cleared on
		// every structural mutation and restored lazily on the first index read, so bulk construction
		// (many appends with no interleaved index reads) renumbers once instead of once per mutation.
		bool childIndicesValid = true;

		// Whether this node's children currently carry correct flattened childIndex values, so a
		// collection can decide between maintaining them incrementally and invalidating the whole set.
		internal bool ChildIndicesValid => childIndicesValid;

		// Marks this node's children's flattened indices stale. Called by the slot setters and the
		// collection after any structural change.
		internal void InvalidateChildIndices()
		{
			childIndicesValid = false;
		}

		// Assigns each child its flattened index if a mutation has invalidated them. The flattened-index
		// arithmetic lives in the generated GetChild/GetChildCount, which depend only on the backing
		// fields, so this is well-defined regardless of the current childIndex values.
		void EnsureChildIndices()
		{
			if (childIndicesValid)
				return;
			childIndicesValid = true;
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				AstNode? c = GetChild(i);
				if (c != null)
					c.childIndex = i;
			}
		}

		/// <summary>
		/// Recursively verifies the slot structure of this subtree (DEBUG only):
		/// <list type="bullet">
		/// <item>every required (non-optional) single slot holds a child, so a transform that drops a
		/// mandatory node is caught here rather than as malformed output;</item>
		/// <item>each present child points its <see cref="Parent"/> back at this node;</item>
		/// <item>its stored flattened index matches its slot position; and</item>
		/// <item>its runtime type is valid for the slot.</item>
		/// </list>
		/// The transform pipeline runs this after each transform, the analog of
		/// <c>ILInstruction.CheckInvariant</c>, so a transform that corrupts the tree fails at the exact
		/// transform rather than as a downstream output diff. The other tree invariants (no node reused
		/// across trees, no node parented to itself) are enforced eagerly by the child setters and follow
		/// from the parent back-pointer check above. A node type overrides this (calling <c>base</c>) to
		/// assert its own constraints -- the scalar invariants its setters enforce eagerly, e.g.
		/// <c>ComposedType.PointerRank &gt;= 0</c>.
		/// </summary>
		[System.Diagnostics.Conditional("DEBUG")]
		internal virtual void CheckInvariant()
		{
			EnsureChildIndices();
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				CSharpSlotInfo slot = GetChildSlotInfo(i);
				AstNode? child = GetChild(i);
				if (child == null)
				{
					// A single slot reads null only when empty; that is valid only if the slot is optional.
					// (Collection slots never yield a null index, so this branch is always a single slot.)
					Debug.Assert(slot.IsOptional, $"required slot '{slot.Name}' on {GetType().Name} must not be empty");
					continue;
				}
				Debug.Assert(child.parent == this, "child's Parent must point back to this node");
				Debug.Assert(child.childIndex == i, "child's flattened index must match its slot position");
				Debug.Assert(slot.ChildType.IsInstanceOfType(child), "child's type must be valid in its slot");
				child.CheckInvariant();
			}
			var trivia = Annotation<NodeTrivia>();
			if (trivia != null)
			{
				CheckTriviaInvariant(trivia.Leading);
				CheckTriviaInvariant(trivia.Trailing);
			}
		}

		// Trivia lives outside the slot space but carries the same parent/index invariants, scoped
		// to the Leading/Trailing list that holds it.
		[System.Diagnostics.Conditional("DEBUG")]
		void CheckTriviaInvariant(List<Trivia>? list)
		{
			if (list == null)
				return;
			for (int i = 0; i < list.Count; i++)
			{
				Trivia t = list[i];
				Debug.Assert(t.parent == this, "trivia's Parent must point back to its owning node");
				Debug.Assert(t.triviaSiblings == list, "trivia's sibling list must be the list that holds it");
				Debug.Assert(t.childIndex == i, "trivia's index must match its position in the trivia list");
			}
		}

		// Self-reference and two-tree guards shared by the single-slot setters; lifts a node out of the
		// subtree currently being replaced (e.g. "x.Right = ((BinaryOperatorExpression)x.Right).Right;").
		void ValidateNewSingleChild<T>(T? value, T? oldChild) where T : AstNode
		{
			if (value == null)
				return;
			if (value == this)
				throw new ArgumentException("Cannot add a node to itself as a child.", nameof(value));
			if (value.parent != null)
			{
				if (oldChild != null && value.Ancestors.Contains(oldChild))
					value.Remove();
				else
					throw new ArgumentException("Node is already used in another tree.", nameof(value));
			}
		}

		// Writes a single-slot backing field when the slot's flattened index is not statically known
		// (a single slot following a collection). A single slot always occupies the same index, so an
		// in-place replace keeps it (carry the old child's index); a set or clear leaves the new child's
		// index unknown here and invalidates, to be reassigned by the next EnsureChildIndices.
		internal void SetChildNode<T>(ref T? field, T? value) where T : AstNode
		{
			if (field == value)
				return;
			ValidateNewSingleChild(value, field);
			T? oldField = field;
			int oldChildIndex = oldField?.childIndex ?? -1;
			oldField?.ClearParentAndIndex();
			field = value;
			value?.SetParent(this);
			if (oldField != null && value != null)
				value.childIndex = oldChildIndex;
			else
				InvalidateChildIndices();
		}

		// Writes a single-slot backing field whose flattened index is known (the generated setter passes
		// it when no collection precedes the slot, and SetChild passes its argument). Filling, clearing,
		// or replacing a single slot moves no other child, so assign the index directly and never
		// invalidate -- childIndex stays maintained by construction, as in the IL AST.
		internal void SetChildNode<T>(ref T? field, T? value, int index) where T : AstNode
		{
			if (field == value)
				return;
			ValidateNewSingleChild(value, field);
			field?.ClearParentAndIndex();
			field = value;
			if (value != null)
			{
				value.SetParent(this);
				value.childIndex = index;
			}
		}

		internal void SetParent(AstNode newParent)
		{
			parent = newParent;
		}

		internal void ClearParentAndIndex()
		{
			parent = null;
			if (this is Trivia trivia)
				trivia.triviaSiblings = null;
			childIndex = -1;
		}
		#endregion

		public void AddChild<T>(T child, CSharpSlotInfo kind) where T : AstNode
		{
			if (child == null)
				return;
			AddChildUnsafe(child, kind);
		}

		/// <summary>
		/// Adds a child into the slot matching <paramref name="kind"/> (appending to a collection slot,
		/// or filling a single slot).
		/// </summary>
		internal void AddChildUnsafe(AstNode child, CSharpSlotInfo kind)
		{
			AstNodeCollection? collection = GetCollectionByKind(kind);
			if (collection != null)
				collection.AddNode(child);
			else
				SetChildByKindUntyped(kind, child);
		}

		// Sets the single slot matching the kind (used by the non-generic mutation API). Unlike
		// GetChild, which returns null when this node declares no slot of that kind, writing a
		// child to a kind the node has no slot for throws.
		internal void SetChildByKindUntyped(CSharpSlotInfo kind, AstNode? child)
		{
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				if (GetChildSlotInfo(i).Kind == kind)
				{
					SetChild(i, child);
					return;
				}
			}
			throw new InvalidOperationException($"{GetType().Name} has no slot of kind '{kind}'.");
		}

		public void InsertChildBefore<T>(AstNode? nextSibling, T child, CSharpSlotInfo kind) where T : AstNode
		{
			if (child == null)
				return;
			AstNodeCollection? collection = GetCollectionByKind(kind);
			if (collection != null)
				collection.InsertNodeBefore(nextSibling, child);
			else
				SetChildByKindUntyped(kind, child);
		}

		internal void InsertChildBeforeUnsafe(AstNode nextSibling, AstNode child, CSharpSlotInfo kind)
		{
			AstNodeCollection? collection = GetCollectionByKind(kind);
			if (collection != null)
				collection.InsertNodeBefore(nextSibling, child);
			else
				SetChildByKindUntyped(kind, child);
		}

		public void InsertChildAfter<T>(AstNode? prevSibling, T child, CSharpSlotInfo kind) where T : AstNode
		{
			if (child == null)
				return;
			AstNodeCollection? collection = GetCollectionByKind(kind);
			if (collection != null)
				collection.InsertNodeAfter(prevSibling, child);
			else
				SetChildByKindUntyped(kind, child);
		}

		/// <summary>
		/// Removes this node from its parent.
		/// </summary>
		public void Remove()
		{
			if (parent == null)
				return;
			if (this is Trivia { triviaSiblings: { } siblings })
			{
				var oldParent = parent;
				int oldIndex = childIndex;
				siblings.RemoveAt(oldIndex);
				ClearParentAndIndex();
				ReindexTrivia(oldParent, siblings, oldIndex);
				return;
			}
			parent.EnsureChildIndices();
			CSharpSlotInfo kind = parent.GetChildSlotInfo(childIndex).Kind!;
			AstNodeCollection? collection = parent.GetCollectionByKind(kind);
			if (collection != null)
				collection.RemoveNode(this);
			else
				parent.SetChild(childIndex, null);
		}

		/// <summary>
		/// Replaces this node with the new node.
		/// </summary>
		public void ReplaceWith(AstNode? newNode)
		{
			if (newNode == null)
			{
				Remove();
				return;
			}
			if (newNode == this)
				return; // nothing to do...
			if (parent == null)
			{
				throw new InvalidOperationException("Cannot replace the root node");
			}
			ThrowIfTrivia();
			parent.EnsureChildIndices();
			CSharpSlotInfo slot = parent.GetChildSlotInfo(childIndex);
			// Because this method doesn't statically check the new node's type with the slot,
			// we perform a runtime test:
			if (!slot.ChildType.IsInstanceOfType(newNode))
			{
				throw new ArgumentException(string.Format("The new node '{0}' is not valid in the slot {1}", newNode.GetType().Name, slot.ToString()), nameof(newNode));
			}
			if (newNode.parent != null)
			{
				// newNode is used within this tree?
				if (newNode.Ancestors.Contains(this))
				{
					// e.g. "parenthesizedExpr.ReplaceWith(parenthesizedExpr.Expression);"
					// enable automatic removal
					newNode.Remove();
				}
				else
				{
					throw new ArgumentException("Node is already used in another tree.", nameof(newNode));
				}
			}
			parent.SetChild(childIndex, newNode);
		}

		// Attached trivia has no child slot to substitute into: its childIndex indexes the owning
		// trivia list, which the slot arithmetic in ReplaceWith must never touch. Trivia is replaced
		// by removing it and attaching new trivia to the owning node.
		void ThrowIfTrivia()
		{
			if (this is Trivia { triviaSiblings: not null })
				throw new InvalidOperationException("Cannot replace trivia; remove it and attach the replacement to the owning node instead.");
		}

		public AstNode? ReplaceWith(Func<AstNode, AstNode?> replaceFunction)
		{
			if (replaceFunction == null)
				throw new ArgumentNullException(nameof(replaceFunction));
			if (parent == null)
			{
				throw new InvalidOperationException("Cannot replace the root node");
			}
			ThrowIfTrivia();
			AstNode oldParent = parent;
			AstNode? oldSuccessor = NextSibling;
			CSharpSlotInfo? oldSlot = this.Slot;
			CSharpSlotInfo? oldKind = oldSlot?.Kind;
			Remove();
			AstNode? replacement = replaceFunction(this);
			if (oldSuccessor != null && oldSuccessor.parent != oldParent)
				throw new InvalidOperationException("replace function changed nextSibling of node being replaced?");
			if (replacement != null && oldKind != null)
			{
				if (replacement.parent != null)
					throw new InvalidOperationException("replace function must return the root of a tree");
				if (oldSlot != null && !oldSlot.ChildType.IsInstanceOfType(replacement))
				{
					throw new InvalidOperationException(string.Format("The new node '{0}' is not valid in the slot {1}", replacement.GetType().Name, oldSlot.ToString()));
				}

				if (oldSuccessor != null)
					oldParent.InsertChildBeforeUnsafe(oldSuccessor, replacement, oldKind);
				else
					oldParent.AddChildUnsafe(replacement, oldKind);
			}
			return replacement;
		}

		/// <summary>
		/// Clones the whole subtree starting at this AST node.
		/// </summary>
		/// <remarks>Annotations are copied over to the new nodes; and any annotations implementing ICloneable will be cloned.</remarks>
		public AstNode Clone()
		{
			AstNode copy = (AstNode)MemberwiseClone();
			copy.parent = null;
			copy.childIndex = -1;
			// MemberwiseClone copied the source's trivia-list reference; the copy is detached.
			if (copy is Trivia copiedTrivia)
				copiedTrivia.triviaSiblings = null;
			// Deep-copy the children (CloneChildrenInto first drops the shallow field copies that
			// MemberwiseClone left pointing at this node's children).
			CloneChildrenInto(copy);
			// Finally, clone the annotation, if necessary
			copy.CloneAnnotations();
			copy.ReparentTrivia();
			return copy;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		public abstract void AcceptVisitor(IAstVisitor visitor);

		public abstract T AcceptVisitor<T>(IAstVisitor<T> visitor);

		public abstract S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data);

		#region Pattern Matching
		protected static bool MatchString(string? pattern, string? text)
		{
			return PatternMatching.Pattern.MatchString(pattern, text);
		}

		/// <summary>
		/// Matches two optional children: both absent (null), or both present and matching.
		/// When the pattern side is present, its own DoMatch decides (e.g. an OptionalNode matches an
		/// absent candidate). When it is absent, the candidate must be absent too.
		/// </summary>
		protected static bool MatchOptional(AstNode? thisChild, AstNode? otherChild, PatternMatching.Match match)
		{
			return thisChild != null ? thisChild.DoMatch(otherChild, match) : otherChild == null;
		}

		protected internal abstract bool DoMatch(AstNode? other, PatternMatching.Match match);

		bool PatternMatching.INode.DoMatch(PatternMatching.INode? other, PatternMatching.Match match)
		{
			AstNode? o = other as AstNode;
			// try matching if other is null, or if other is an AstNode
			return (other == null || o != null) && DoMatch(o, match);
		}

		bool PatternMatching.INode.DoMatchCollection(System.Collections.Generic.IReadOnlyList<PatternMatching.INode> other, int pos, PatternMatching.Match match, PatternMatching.BacktrackingInfo backtrackingInfo)
		{
			PatternMatching.INode? raw = pos < other.Count ? other[pos] : null;
			AstNode? o = raw as AstNode;
			// matches a single element: succeeds only if the element is absent or an AstNode
			return (raw == null || o != null) && DoMatch(o, match);
		}

		#endregion

		public AstNode? GetNextNode()
		{
			if (NextSibling != null)
				return NextSibling;
			// Trivia navigation ends at its owning list: leading trivia precedes its owner in document
			// order, so continuing into the owner's sibling space would skip the owner's whole subtree.
			if (this is Trivia)
				return null;
			if (Parent != null)
				return Parent.GetNextNode();
			return null;
		}

		/// <summary>
		/// Gets the next node which fullfills a given predicate
		/// </summary>
		/// <returns>The next node.</returns>
		/// <param name="pred">The predicate.</param>
		public AstNode? GetNextNode(Func<AstNode, bool> pred)
		{
			var next = GetNextNode();
			while (next != null && !pred(next))
				next = next.GetNextNode();
			return next;
		}

		public AstNode? GetPrevNode()
		{
			if (PrevSibling != null)
				return PrevSibling;
			// Trivia navigation ends at its owning list; see GetNextNode.
			if (this is Trivia)
				return null;
			if (Parent != null)
				return Parent.GetPrevNode();
			return null;
		}

		/// <summary>
		/// Gets the previous node which fullfills a given predicate
		/// </summary>
		/// <returns>The next node.</returns>
		/// <param name="pred">The predicate.</param>
		public AstNode? GetPrevNode(Func<AstNode, bool> pred)
		{
			var prev = GetPrevNode();
			while (prev != null && !pred(prev))
				prev = prev.GetPrevNode();
			return prev;
		}

		/// <summary>
		/// Gets the next sibling which fullfills a given predicate
		/// </summary>
		/// <returns>The next node.</returns>
		/// <param name="pred">The predicate.</param>
		public AstNode? GetNextSibling(Func<AstNode, bool> pred)
		{
			var next = NextSibling;
			while (next != null && !pred(next))
				next = next.NextSibling;
			return next;
		}

		/// <summary>
		/// Gets the next sibling which fullfills a given predicate
		/// </summary>
		/// <returns>The next node.</returns>
		/// <param name="pred">The predicate.</param>
		public AstNode? GetPrevSibling(Func<AstNode, bool> pred)
		{
			var prev = PrevSibling;
			while (prev != null && !pred(prev))
				prev = prev.PrevSibling;
			return prev;
		}

		/// <summary>
		/// Gets the node as formatted C# output.
		/// </summary>
		/// <param name='formattingOptions'>
		/// Formatting options.
		/// </param>
		public virtual string ToString(CSharpFormattingOptions? formattingOptions)
		{
			var w = new StringWriter();
			AcceptVisitor(new CSharpOutputVisitor(w, formattingOptions ?? FormattingOptionsFactory.CreateMono()));
			return w.ToString();
		}

		public sealed override string ToString()
		{
			return ToString(null);
		}

		/// <summary>
		/// Returns true, if the given coordinates (line, column) are in the node.
		/// </summary>
		/// <returns>
		/// True, if the given coordinates are between StartLocation and EndLocation (exclusive); otherwise, false.
		/// </returns>
		public bool Contains(int line, int column)
		{
			return Contains(new TextLocation(line, column));
		}

		/// <summary>
		/// Returns true, if the given coordinates are in the node.
		/// </summary>
		/// <returns>
		/// True, if location is between StartLocation and EndLocation (exclusive); otherwise, false.
		/// </returns>
		public bool Contains(TextLocation location)
		{
			return this.StartLocation <= location && location < this.EndLocation;
		}

		/// <summary>
		/// Returns true, if the given coordinates (line, column) are in the node.
		/// </summary>
		/// <returns>
		/// True, if the given coordinates are between StartLocation and EndLocation (inclusive); otherwise, false.
		/// </returns>
		public bool IsInside(int line, int column)
		{
			return IsInside(new TextLocation(line, column));
		}

		/// <summary>
		/// Returns true, if the given coordinates are in the node.
		/// </summary>
		/// <returns>
		/// True, if location is between StartLocation and EndLocation (inclusive); otherwise, false.
		/// </returns>
		public bool IsInside(TextLocation location)
		{
			return this.StartLocation <= location && location <= this.EndLocation;
		}

		internal string DebugToString()
		{
			string text = ToString();
			text = text.TrimEnd().Replace("\t", "").Replace(Environment.NewLine, " ");
			if (text.Length > 100)
				return text.Substring(0, 97) + "...";
			else
				return text;
		}
	}
}
