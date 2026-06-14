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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode(hasNullNode: true, hasPatternPlaceholder: true)]
	public abstract partial class AstNode : AbstractAnnotatable, IFreezable, INode, ICloneable
	{
		// the Root role must be available when creating the null nodes, so we can't put it in the Roles class
		internal static readonly Role<AstNode?> RootRole = new Role<AstNode?>("Root", null);

		AstNode? parent;
		// Flattened index of this node within its parent's child-index space (-1 when unparented).
		// Recomputed lazily by the parent (EnsureChildIndices) after a structural mutation.
		internal int childIndex = -1;

		// Flags, from least significant to most significant bits:
		// - Role.RoleIndexBits: role index
		protected uint flags = RootRole.Index;
		// Derived classes may also use a few bits,
		// for example Identifier uses 1 bit for IsVerbatim

		const uint roleIndexMask = (1u << Role.RoleIndexBits) - 1;
		protected const int AstNodeFlagsUsedBits = Role.RoleIndexBits;

		public abstract NodeType NodeType {
			get;
		}

		public virtual bool IsNull {
			get {
				return false;
			}
		}

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
		sealed class NodeTrivia : ICloneable
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
			if (trivia == null)
				throw new ArgumentNullException(nameof(trivia));
			NodeTrivia holder = GetOrCreateTrivia();
			(holder.Leading ??= new List<Trivia>()).Add(trivia);
		}

		/// <summary>
		/// Inserts trivia at the start of this node's leading trivia, before any already attached. Used for
		/// a file header comment that must precede generated leading directives such as <c>#define</c>.
		/// </summary>
		public void PrependLeadingTrivia(Trivia trivia)
		{
			if (trivia == null)
				throw new ArgumentNullException(nameof(trivia));
			NodeTrivia holder = GetOrCreateTrivia();
			(holder.Leading ??= new List<Trivia>()).Insert(0, trivia);
		}

		public void AddTrailingTrivia(Trivia trivia)
		{
			if (trivia == null)
				throw new ArgumentNullException(nameof(trivia));
			NodeTrivia holder = GetOrCreateTrivia();
			(holder.Trailing ??= new List<Trivia>()).Add(trivia);
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

		public Role Role {
			get {
				return Role.GetByIndex(flags & roleIndexMask);
			}
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				if (!value.IsValid(this))
					throw new ArgumentException("This node is not valid in the new role.");
				SetRole(value);
			}
		}

		internal uint RoleIndex {
			get { return flags & roleIndexMask; }
		}

		void SetRole(Role role)
		{
			flags = (flags & ~roleIndexMask) | role.Index;
		}

		public AstNode? NextSibling {
			get {
				if (parent == null)
					return null;
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

		public IEnumerable<AstNode> Children {
			get {
				AstNode? next;
				for (AstNode? cur = FirstChild; cur != null; cur = next)
				{
					Debug.Assert(cur.parent == this);
					// Remember next before yielding cur.
					// This allows removing/replacing nodes while iterating.
					next = cur.NextSibling;
					yield return cur;
				}
			}
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
		/// Gets the first child with the specified role.
		/// Returns the role's null object if the child is not found.
		/// </summary>
		public T GetChildByRole<T>(Role<T> role) where T : AstNode?
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				if (GetChildSlot(i) == role)
				{
					AstNode? c = GetChild(i);
					return c != null ? (T)c : role.NullObject;
				}
			}
			return role.NullObject;
		}

		public T? GetParent<T>() where T : AstNode
		{
			return Ancestors.OfType<T>().FirstOrDefault();
		}

		public AstNode? GetParent(Func<AstNode, bool>? pred)
		{
			return pred != null ? Ancestors.FirstOrDefault(pred) : Ancestors.FirstOrDefault();
		}

		public AstNodeCollection<T> GetChildrenByRole<T>(Role<T> role) where T : AstNode
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			AstNodeCollection? collection = GetCollectionByRole(role);
			// A node has no children of a role it does not declare a collection slot for. Reads of such
			// a role (e.g. the parameters of a non-indexer property) get a detached empty collection;
			// writes go through AddChild/SetChildByRole, which reject a missing role.
			return collection != null ? (AstNodeCollection<T>)collection : new AstNodeCollection<T>(this, role);
		}

		protected void SetChildByRole<T>(Role<T> role, T newChild) where T : AstNode
		{
			SetChildByRoleUntyped(role, newChild);
		}

		#region Slot storage contract
		// Each concrete node's slots form a flattened child-index space, in source-declaration order.
		// A single slot occupies exactly one index (even when empty, where GetChild returns null); a
		// collection slot occupies a contiguous run of its current length. The generator emits these
		// four members per node from its [Slot] partial-property declarations; nodes without children
		// (and the null/placeholder nodes) keep the zero-child defaults below.

		internal virtual int GetChildCount() => 0;

		internal virtual AstNode? GetChild(int index) => throw new ArgumentOutOfRangeException(nameof(index));

		internal virtual void SetChild(int index, AstNode? value) => throw new ArgumentOutOfRangeException(nameof(index));

		internal virtual Role GetChildSlot(int index) => throw new ArgumentOutOfRangeException(nameof(index));

		// Returns the collection occupying the slot with the given role, or null if there is none.
		// Overridden by the generator for nodes that have collection slots.
		internal virtual AstNodeCollection? GetCollectionByRole(Role role) => null;

		// Deep-copies this node's children into the (memberwise-cloned) copy, which initially shares
		// this node's child references. Overridden by the generator for nodes that have slots.
		internal virtual void CloneChildrenInto(AstNode copy) { }

		// Whether this node's children currently carry correct flattened childIndex values. Cleared on
		// every structural mutation and restored lazily on the first index read, so bulk construction
		// (many appends with no interleaved index reads) renumbers once instead of once per mutation.
		bool childIndicesValid = true;

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
		/// Recursively verifies the slot structure of this subtree (DEBUG only): every child's Parent
		/// points back here, its stored flattened index matches its slot position, and its runtime type
		/// is valid in the slot's role. The transform pipeline runs this after each transform, the
		/// analog of <c>ILInstruction.CheckInvariant</c>, so a transform that corrupts the tree fails at
		/// the exact transform rather than as a downstream output diff.
		/// </summary>
		[System.Diagnostics.Conditional("DEBUG")]
		internal void CheckInvariant()
		{
			EnsureChildIndices();
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				AstNode? child = GetChild(i);
				if (child == null)
					continue; // empty optional single slot
				Debug.Assert(child.parent == this, "child's Parent must point back to this node");
				Debug.Assert(child.childIndex == i, "child's flattened index must match its slot position");
				Debug.Assert(GetChildSlot(i).IsValid(child), "child's type must be valid in its slot's role");
				child.CheckInvariant();
			}
		}

		// Writes a single-slot backing field: detaches the old child, attaches the new one, renumbers.
		// A null or null-object value empties the slot. Called by generated single-slot property setters.
		internal void SetChildNode<T>(ref T? field, T? value, Role role) where T : AstNode
		{
			T? newValue = (value == null || value.IsNull) ? null : value;
			if (field == newValue)
				return;
			if (newValue != null)
			{
				if (newValue == this)
					throw new ArgumentException("Cannot add a node to itself as a child.", nameof(value));
				if (newValue.parent != null)
				{
					// Allow lifting a node out of the subtree being replaced, e.g.
					// "assignment.Right = ((BinaryOperatorExpression)assignment.Right).Right;".
					if (field != null && newValue.Ancestors.Contains(field))
						newValue.Remove();
					else
						throw new ArgumentException("Node is already used in another tree.", nameof(value));
				}
			}
			field?.ClearParentAndIndex();
			field = newValue;
			newValue?.SetParentAndRole(this, role);
			InvalidateChildIndices();
		}

		internal void SetParentAndRole(AstNode newParent, Role role)
		{
			parent = newParent;
			SetRole(role);
		}

		internal void ClearParentAndIndex()
		{
			parent = null;
			childIndex = -1;
		}
		#endregion

		public void AddChild<T>(T child, Role<T> role) where T : AstNode
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			if (child == null || child.IsNull)
				return;
			AddChildUnsafe(child, role);
		}

		public void AddChildWithExistingRole(AstNode? child)
		{
			if (child == null || child.IsNull)
				return;
			AddChildUnsafe(child, child.Role);
		}

		/// <summary>
		/// Adds a child into the slot matching <paramref name="role"/> (appending to a collection slot,
		/// or filling a single slot).
		/// </summary>
		internal void AddChildUnsafe(AstNode child, Role role)
		{
			AstNodeCollection? collection = GetCollectionByRole(role);
			if (collection != null)
				collection.AddNode(child);
			else
				SetChildByRoleUntyped(role, child);
		}

		// Sets the single slot matching the role (used by the non-generic mutation API). Symmetric with
		// GetChildByRole, which returns the null object for a role this node does not declare a slot for:
		// a node has no child of a role it has no slot for, so writing one is a no-op.
		internal void SetChildByRoleUntyped(Role role, AstNode? child)
		{
			int count = GetChildCount();
			for (int i = 0; i < count; i++)
			{
				if (GetChildSlot(i) == role)
				{
					SetChild(i, child == null || child.IsNull ? null : child);
					return;
				}
			}
			throw new InvalidOperationException($"{GetType().Name} has no slot for role '{role}'.");
		}

		public void InsertChildBefore<T>(AstNode? nextSibling, T child, Role<T> role) where T : AstNode
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			if (child == null || child.IsNull)
				return;
			AstNodeCollection? collection = GetCollectionByRole(role);
			if (collection != null)
				collection.InsertNodeBefore((nextSibling == null || nextSibling.IsNull) ? null : nextSibling, child);
			else
				SetChildByRoleUntyped(role, child);
		}

		internal void InsertChildBeforeUnsafe(AstNode nextSibling, AstNode child, Role role)
		{
			AstNodeCollection? collection = GetCollectionByRole(role);
			if (collection != null)
				collection.InsertNodeBefore(nextSibling, child);
			else
				SetChildByRoleUntyped(role, child);
		}

		public void InsertChildAfter<T>(AstNode? prevSibling, T child, Role<T> role) where T : AstNode
		{
			if (role == null)
				throw new ArgumentNullException(nameof(role));
			if (child == null || child.IsNull)
				return;
			AstNodeCollection? collection = GetCollectionByRole(role);
			if (collection != null)
				collection.InsertNodeAfter((prevSibling == null || prevSibling.IsNull) ? null : prevSibling, child);
			else
				SetChildByRoleUntyped(role, child);
		}

		/// <summary>
		/// Removes this node from its parent.
		/// </summary>
		public void Remove()
		{
			if (parent == null)
				return;
			parent.EnsureChildIndices();
			Role role = parent.GetChildSlot(childIndex);
			AstNodeCollection? collection = parent.GetCollectionByRole(role);
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
			if (newNode == null || newNode.IsNull)
			{
				Remove();
				return;
			}
			if (newNode == this)
				return; // nothing to do...
			if (parent == null)
			{
				throw new InvalidOperationException(this.IsNull ? "Cannot replace the null nodes" : "Cannot replace the root node");
			}
			parent.EnsureChildIndices();
			Role role = parent.GetChildSlot(childIndex);
			// Because this method doesn't statically check the new node's type with the role,
			// we perform a runtime test:
			if (!role.IsValid(newNode))
			{
				throw new ArgumentException(string.Format("The new node '{0}' is not valid in the role {1}", newNode.GetType().Name, role.ToString()), nameof(newNode));
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

		public AstNode? ReplaceWith(Func<AstNode, AstNode?> replaceFunction)
		{
			if (replaceFunction == null)
				throw new ArgumentNullException(nameof(replaceFunction));
			if (parent == null)
			{
				throw new InvalidOperationException(this.IsNull ? "Cannot replace the null nodes" : "Cannot replace the root node");
			}
			AstNode oldParent = parent;
			AstNode? oldSuccessor = NextSibling;
			Role oldRole = this.Role;
			Remove();
			AstNode? replacement = replaceFunction(this);
			if (oldSuccessor != null && oldSuccessor.parent != oldParent)
				throw new InvalidOperationException("replace function changed nextSibling of node being replaced?");
			if (!(replacement == null || replacement.IsNull))
			{
				if (replacement.parent != null)
					throw new InvalidOperationException("replace function must return the root of a tree");
				if (!oldRole.IsValid(replacement))
				{
					throw new InvalidOperationException(string.Format("The new node '{0}' is not valid in the role {1}", replacement.GetType().Name, oldRole.ToString()));
				}

				if (oldSuccessor != null)
					oldParent.InsertChildBeforeUnsafe(oldSuccessor, replacement, oldRole);
				else
					oldParent.AddChildUnsafe(replacement, oldRole);
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
			// Deep-copy the children (CloneChildrenInto first drops the shallow field copies that
			// MemberwiseClone left pointing at this node's children).
			CloneChildrenInto(copy);
			// Finally, clone the annotation, if necessary
			copy.CloneAnnotations();
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
		// filters all non c# nodes (comments, white spaces or pre processor directives)
		public AstNode? GetCSharpNodeBefore(AstNode node)
		{
			var n = node.PrevSibling;
			while (n != null)
			{
				if (n.Role != Roles.Comment)
					return n;
				n = n.GetPrevNode();
			}
			return null;
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
			if (IsNull)
				return "";
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

		public override void AddAnnotation(object annotation)
		{
			if (this.IsNull)
				throw new InvalidOperationException("Cannot add annotations to the null node");
			base.AddAnnotation(annotation);
		}

		internal string DebugToString()
		{
			if (IsNull)
				return "Null";
			string text = ToString();
			text = text.TrimEnd().Replace("\t", "").Replace(Environment.NewLine, " ");
			if (text.Length > 100)
				return text.Substring(0, 97) + "...";
			else
				return text;
		}
	}
}
