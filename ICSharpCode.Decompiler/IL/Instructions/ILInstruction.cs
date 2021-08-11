// Copyright (c) 2014 Daniel Grunwald
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using ICSharpCode.Decompiler.IL.Patterns;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	internal enum ILPhase
	{
		/// <summary>
		/// Reading the individual instructions.
		/// * Variables don't have scopes yet as the ILFunction is not created yet.
		/// * Branches point to IL offsets, not blocks.
		/// </summary>
		InILReader,
		/// <summary>
		/// The usual invariants are established.
		/// </summary>
		Normal,
		/// <summary>
		/// Special phase within the async-await decompiler, where a few selected invariants
		/// are temporarily suspended. (see Leave.CheckInvariant)
		/// </summary>
		InAsyncAwait
	}

	/// <summary>
	/// Represents a decoded IL instruction
	/// </summary>
	public abstract partial class ILInstruction
	{
		public readonly OpCode OpCode;

		protected ILInstruction(OpCode opCode)
		{
			this.OpCode = opCode;
		}

		protected void ValidateChild(ILInstruction? inst)
		{
			if (inst == null)
				throw new ArgumentNullException(nameof(inst));
			Debug.Assert(!this.IsDescendantOf(inst), "ILAst must form a tree");
			// If a call to ReplaceWith() triggers the "ILAst must form a tree" assertion,
			// make sure to read the remarks on the ReplaceWith() method.
		}

		internal static void DebugAssert([DoesNotReturnIf(false)] bool b)
		{
			Debug.Assert(b);
		}

		internal static void DebugAssert([DoesNotReturnIf(false)] bool b, string msg)
		{
			Debug.Assert(b, msg);
		}

		[Conditional("DEBUG")]
		internal virtual void CheckInvariant(ILPhase phase)
		{
			foreach (var child in Children)
			{
				Debug.Assert(child.Parent == this);
				Debug.Assert(this.GetChild(child.ChildIndex) == child);
				// if child flags are invalid, parent flags must be too
				// exception: nested ILFunctions (lambdas)
				Debug.Assert(this is ILFunction || child.flags != invalidFlags || this.flags == invalidFlags);
				Debug.Assert(child.IsConnected == this.IsConnected);
				child.CheckInvariant(phase);
			}
			Debug.Assert((this.DirectFlags & ~this.Flags) == 0, "All DirectFlags must also appear in this.Flags");
		}

		/// <summary>
		/// Gets whether this node is a descendant of <paramref name="possibleAncestor"/>.
		/// Also returns true if <c>this</c>==<paramref name="possibleAncestor"/>.
		/// </summary>
		/// <remarks>
		/// This method uses the <c>Parent</c> property, so it may produce surprising results
		/// when called on orphaned nodes or with a possibleAncestor that contains stale positions
		/// (see remarks on Parent property).
		/// </remarks>
		public bool IsDescendantOf(ILInstruction possibleAncestor)
		{
			for (ILInstruction? ancestor = this; ancestor != null; ancestor = ancestor.Parent)
			{
				if (ancestor == possibleAncestor)
					return true;
			}
			return false;
		}

		public ILInstruction? GetCommonParent(ILInstruction other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));

			ILInstruction? a = this;
			ILInstruction? b = other;

			int levelA = a.CountAncestors();
			int levelB = b.CountAncestors();

			while (levelA > levelB)
			{
				a = a!.Parent;
				levelA--;
			}

			while (levelB > levelA)
			{
				b = b!.Parent;
				levelB--;
			}

			while (a != b)
			{
				a = a!.Parent;
				b = b!.Parent;
			}

			return a;
		}

		/// <summary>
		/// Returns whether this appears before other in a post-order walk of the whole tree.
		/// </summary>
		public bool IsBefore(ILInstruction other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));

			ILInstruction a = this;
			ILInstruction b = other;

			int levelA = a.CountAncestors();
			int levelB = b.CountAncestors();

			int originalLevelA = levelA;
			int originalLevelB = levelB;

			while (levelA > levelB)
			{
				a = a.Parent!;
				levelA--;
			}

			while (levelB > levelA)
			{
				b = b.Parent!;
				levelB--;
			}

			if (a == b)
			{
				// a or b is a descendant of the other,
				// whichever node has the higher level comes first in post-order walk.
				return originalLevelA > originalLevelB;
			}

			while (a.Parent != b.Parent)
			{
				a = a.Parent!;
				b = b.Parent!;
			}

			// now a and b have the same parent or are both root nodes
			return a.ChildIndex < b.ChildIndex;
		}

		private int CountAncestors()
		{
			int level = 0;
			for (ILInstruction? ancestor = this; ancestor != null; ancestor = ancestor.Parent)
			{
				level++;
			}
			return level;
		}

		/// <summary>
		/// Gets the stack type of the value produced by this instruction.
		/// </summary>
		public abstract StackType ResultType { get; }

		/* Not sure if it's a good idea to offer this on all instructions --
		 *   e.g. ldloc for a local of type `int?` would return StackType.O (because it's not a lifted operation),
		 *   even though the underlying type is int = StackType.I4.
		/// <summary>
		/// Gets the underlying result type of the value produced by this instruction.
		/// 
		/// If this is a lifted operation, the ResultType will be `StackType.O` (because Nullable{T} is a struct),
		/// and UnderlyingResultType will be result type of the corresponding non-lifted operation.
		/// 
		/// If this is not a lifted operation, the underlying result type is equal to the result type.
		/// </summary>
		public virtual StackType UnderlyingResultType { get => ResultType; }
		*/

		internal static StackType CommonResultType(StackType a, StackType b)
		{
			if (a == StackType.I || b == StackType.I)
				return StackType.I;
			Debug.Assert(a == b);
			return a;
		}

#if DEBUG
		/// <summary>
		/// Gets whether this node (or any subnode) was modified since the last <c>ResetDirty()</c> call.
		/// </summary>
		/// <remarks>
		/// IsDirty is used by the StatementTransform, and must not be used by individual transforms within the loop.
		/// </remarks>
		internal bool IsDirty { get; private set; }

		/// <summary>
		/// Marks this node (and all subnodes) as <c>IsDirty=false</c>.
		/// </summary>
		internal void ResetDirty()
		{
			foreach (ILInstruction inst in Descendants)
				inst.IsDirty = false;
		}
#endif

		[Conditional("DEBUG")]
		protected private void MakeDirty()
		{
#if DEBUG
			for (ILInstruction? inst = this; inst != null && !inst.IsDirty; inst = inst.parent)
			{
				inst.IsDirty = true;
			}
#endif
		}

		const InstructionFlags invalidFlags = (InstructionFlags)(-1);

		InstructionFlags flags = invalidFlags;

		/// <summary>
		/// Gets the flags describing the behavior of this instruction.
		/// This property computes the flags on-demand and caches them
		/// until some change to the ILAst invalidates the cache.
		/// </summary>
		/// <remarks>
		/// Flag cache invalidation makes use of the <c>Parent</c> property,
		/// so it is possible for this property to return a stale value
		/// if the instruction contains "stale positions" (see remarks on Parent property).
		/// </remarks>
		public InstructionFlags Flags {
			get {
				if (flags == invalidFlags)
				{
					flags = ComputeFlags();
				}
				return flags;
			}
		}

		/// <summary>
		/// Returns whether the instruction (or one of its child instructions) has at least one of the specified flags.
		/// </summary>
		public bool HasFlag(InstructionFlags flags)
		{
			return (this.Flags & flags) != 0;
		}

		/// <summary>
		/// Returns whether the instruction (without considering child instructions) has at least one of the specified flags.
		/// </summary>
		public bool HasDirectFlag(InstructionFlags flags)
		{
			return (this.DirectFlags & flags) != 0;
		}

		protected void InvalidateFlags()
		{
			for (ILInstruction? inst = this; inst != null && inst.flags != invalidFlags; inst = inst.parent)
				inst.flags = invalidFlags;
		}

		protected abstract InstructionFlags ComputeFlags();

		/// <summary>
		/// Gets the flags for this instruction only, without considering the child instructions.
		/// </summary>
		public abstract InstructionFlags DirectFlags { get; }

		/// <summary>
		/// Gets the ILRange for this instruction alone, ignoring the operands.
		/// </summary>
		private Interval ILRange;

		public void AddILRange(Interval newRange)
		{
			this.ILRange = CombineILRange(this.ILRange, newRange);
		}

		protected static Interval CombineILRange(Interval oldRange, Interval newRange)
		{
			if (oldRange.IsEmpty)
			{
				return newRange;
			}
			if (newRange.IsEmpty)
			{
				return oldRange;
			}
			if (newRange.Start <= oldRange.Start)
			{
				if (newRange.End < oldRange.Start)
				{
					return newRange; // use the earlier range
				}
				else
				{
					// join overlapping ranges
					return new Interval(newRange.Start, Math.Max(newRange.End, oldRange.End));
				}
			}
			else if (newRange.Start <= oldRange.End)
			{
				// join overlapping ranges
				return new Interval(oldRange.Start, Math.Max(newRange.End, oldRange.End));
			}
			return oldRange;
		}

		public void AddILRange(ILInstruction sourceInstruction)
		{
			AddILRange(sourceInstruction.ILRange);
		}

		public void SetILRange(ILInstruction sourceInstruction)
		{
			ILRange = sourceInstruction.ILRange;
		}

		public void SetILRange(Interval range)
		{
			ILRange = range;
		}

		public int StartILOffset => ILRange.Start;

		public int EndILOffset => ILRange.End;

		public bool ILRangeIsEmpty => ILRange.IsEmpty;

		public IEnumerable<Interval> ILRanges => new[] { ILRange };

		public void WriteILRange(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
		}

		/// <summary>
		/// Writes the ILAst to the text output.
		/// </summary>
		public abstract void WriteTo(ITextOutput output, ILAstWritingOptions options);

		public override string ToString()
		{
			var output = new PlainTextOutput();
			WriteTo(output, new ILAstWritingOptions());
			if (!ILRange.IsEmpty)
			{
				output.Write(" at IL_" + ILRange.Start.ToString("x4"));
			}
			return output.ToString();
		}

		/// <summary>
		/// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
		/// </summary>
		public abstract void AcceptVisitor(ILVisitor visitor);

		/// <summary>
		/// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
		/// </summary>
		public abstract T AcceptVisitor<T>(ILVisitor<T> visitor);

		/// <summary>
		/// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
		/// </summary>
		public abstract T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context);

		/// <summary>
		/// Gets the child nodes of this instruction.
		/// </summary>
		/// <remarks>
		/// The ChildrenCollection does not actually store the list of children,
		/// it merely allows accessing the children stored in the various slots.
		/// </remarks>
		public ChildrenCollection Children {
			get {
				return new ChildrenCollection(this);
			}
		}

		protected abstract int GetChildCount();
		protected abstract ILInstruction GetChild(int index);
		protected abstract void SetChild(int index, ILInstruction value);
		protected abstract SlotInfo GetChildSlot(int index);

		#region ChildrenCollection + ChildrenEnumerator
		public readonly struct ChildrenCollection : IReadOnlyList<ILInstruction>
		{
			readonly ILInstruction inst;

			internal ChildrenCollection(ILInstruction inst)
			{
				Debug.Assert(inst != null);
				this.inst = inst!;
			}

			public int Count {
				get { return inst.GetChildCount(); }
			}

			public ILInstruction this[int index] {
				get { return inst.GetChild(index); }
				set { inst.SetChild(index, value); }
			}

			public ChildrenEnumerator GetEnumerator()
			{
				return new ChildrenEnumerator(inst);
			}

			IEnumerator<ILInstruction> IEnumerable<ILInstruction>.GetEnumerator()
			{
				return GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

#if DEBUG
		int activeEnumerators;

		[Conditional("DEBUG")]
		internal void StartEnumerator()
		{
			activeEnumerators++;
		}

		[Conditional("DEBUG")]
		internal void StopEnumerator()
		{
			Debug.Assert(activeEnumerators > 0);
			activeEnumerators--;
		}
#endif

		[Conditional("DEBUG")]
		internal void AssertNoEnumerators()
		{
#if DEBUG
			Debug.Assert(activeEnumerators == 0);
#endif
		}

		/// <summary>
		/// Enumerator over the children of an ILInstruction.
		/// Warning: even though this is a struct, it is invalid to copy:
		/// the number of constructor calls must match the number of dispose calls.
		/// </summary>
		public struct ChildrenEnumerator : IEnumerator<ILInstruction>
		{
			ILInstruction? inst;
			readonly int end;
			int pos;

			internal ChildrenEnumerator(ILInstruction inst)
			{
				DebugAssert(inst != null);
				this.inst = inst;
				this.pos = -1;
				this.end = inst!.GetChildCount();
#if DEBUG
				inst.StartEnumerator();
#endif
			}

			public ILInstruction Current {
				get {
					return inst!.GetChild(pos);
				}
			}

			public bool MoveNext()
			{
				return ++pos < end;
			}

			public void Dispose()
			{
#if DEBUG
				if (inst != null)
				{
					inst.StopEnumerator();
					inst = null;
				}
#endif
			}

			object System.Collections.IEnumerator.Current {
				get { return this.Current; }
			}

			void System.Collections.IEnumerator.Reset()
			{
				pos = -1;
			}
		}
		#endregion

		/// <summary>
		/// Replaces this ILInstruction with the given replacement instruction.
		/// </summary>
		/// <remarks>
		/// It is temporarily possible for a node to be used in multiple places in the ILAst,
		/// this method only replaces this node at its primary position (see remarks on <see cref="Parent"/>).
		/// 
		/// This means you cannot use ReplaceWith() to wrap an instruction in another node.
		/// For example, <c>node.ReplaceWith(new BitNot(node))</c> will first call the BitNot constructor,
		/// which sets <c>node.Parent</c> to the BitNot instance.
		/// The ReplaceWith() call then attempts to set <c>BitNot.Argument</c> to the BitNot instance,
		/// which creates a cyclic ILAst. Meanwhile, node's original parent remains unmodified.
		/// 
		/// The solution in this case is to avoid using <c>ReplaceWith</c>.
		/// If the parent node is unknown, the following trick can be used:
		/// <code>
		/// node.Parent.Children[node.ChildIndex] = new BitNot(node);
		/// </code>
		/// Unlike the <c>ReplaceWith()</c> call, this will evaluate <c>node.Parent</c> and <c>node.ChildIndex</c>
		/// before the <c>BitNot</c> constructor is called, thus modifying the expected position in the ILAst.
		/// </remarks>
		public void ReplaceWith(ILInstruction replacement)
		{
			Debug.Assert(parent!.GetChild(ChildIndex) == this);
			if (replacement == this)
				return;
			parent.SetChild(ChildIndex, replacement);
		}

		/// <summary>
		/// Returns all descendants of the ILInstruction in post-order.
		/// (including the ILInstruction itself)
		/// </summary>
		/// <remarks>
		/// Within a loop 'foreach (var node in inst.Descendants)', it is illegal to
		/// add or remove from the child collections of node's ancestors, as those are
		/// currently being enumerated.
		/// Note that it is valid to modify node's children as those were already previously visited.
		/// As a special case, it is also allowed to replace node itself with another node.
		/// </remarks>
		public IEnumerable<ILInstruction> Descendants {
			get {
				// Copy of TreeTraversal.PostOrder() specialized for ChildrenEnumerator
				// We could potentially eliminate the stack by using Parent/ChildIndex,
				// but that makes it difficult to reason about the behavior in the cases
				// where Parent/ChildIndex is not accurate (stale positions), especially
				// if the ILAst is modified during enumeration.
				Stack<ChildrenEnumerator> stack = new Stack<ChildrenEnumerator>();
				ChildrenEnumerator enumerator = new ChildrenEnumerator(this);
				try
				{
					while (true)
					{
						while (enumerator.MoveNext())
						{
							var element = enumerator.Current;
							stack.Push(enumerator);
							enumerator = new ChildrenEnumerator(element);
						}
						enumerator.Dispose();
						if (stack.Count > 0)
						{
							enumerator = stack.Pop();
							yield return enumerator.Current;
						}
						else
						{
							break;
						}
					}
				}
				finally
				{
					enumerator.Dispose();
					while (stack.Count > 0)
					{
						stack.Pop().Dispose();
					}
				}
				yield return this;
			}
		}

		/// <summary>
		/// Gets the ancestors of this node (including the node itself as first element).
		/// </summary>
		public IEnumerable<ILInstruction> Ancestors {
			get {
				for (ILInstruction? node = this; node != null; node = node.Parent)
				{
					yield return node;
				}
			}
		}

		/// <summary>
		/// Number of parents that refer to this instruction and are connected to the root.
		/// Usually is 0 for unconnected nodes and 1 for connected nodes, but may temporarily increase to 2
		/// when the ILAst is re-arranged (e.g. within SetChildInstruction),
		/// or possibly even more (re-arrangement with stale positions).
		/// </summary>
		byte refCount;

		internal void AddRef()
		{
			if (refCount++ == 0)
			{
				Connected();
			}
		}

		internal void ReleaseRef()
		{
			Debug.Assert(refCount > 0);
			if (--refCount == 0)
			{
				Disconnected();
			}
		}

		/// <summary>
		/// Gets whether this ILInstruction is connected to the root node of the ILAst.
		/// </summary>
		/// <remarks>
		/// This property returns true if the ILInstruction is reachable from the root node
		/// of the ILAst; it does not make use of the <c>Parent</c> field so the considerations
		/// about orphaned nodes and stale positions don't apply.
		/// </remarks>
		protected internal bool IsConnected {
			get { return refCount > 0; }
		}

		/// <summary>
		/// Called after the ILInstruction was connected to the root node of the ILAst.
		/// </summary>
		protected virtual void Connected()
		{
			foreach (var child in Children)
				child.AddRef();
		}

		/// <summary>
		/// Called after the ILInstruction was disconnected from the root node of the ILAst.
		/// </summary>
		protected virtual void Disconnected()
		{
			foreach (var child in Children)
				child.ReleaseRef();
		}

		ILInstruction? parent;

		/// <summary>
		/// Gets the parent of this ILInstruction.
		/// </summary>
		/// <remarks>
		/// It is temporarily possible for a node to be used in multiple places in the ILAst
		/// (making the ILAst a DAG instead of a tree).
		/// The <c>Parent</c> and <c>ChildIndex</c> properties are written whenever
		/// a node is stored in a slot.
		/// The node's occurrence in that slot is termed the "primary position" of the node,
		/// and all other (older) uses of the nodes are termed "stale positions".
		/// 
		/// A consistent ILAst must not contain any stale positions.
		/// Debug builds of ILSpy check the ILAst for consistency after every IL transform.
		/// 
		/// If a slot containing a node is overwritten with another node, the <c>Parent</c>
		/// and <c>ChildIndex</c> of the old node are not modified.
		/// This allows overwriting stale positions to restore consistency of the ILAst.
		/// 
		/// If a "primary position" is overwritten, the <c>Parent</c> of the old node also remains unmodified.
		/// This makes the old node an "orphaned node".
		/// Orphaned nodes may later be added back to the ILAst (or can just be garbage-collected).
		/// 
		/// Note that is it is possible (though unusual) for a stale position to reference an orphaned node.
		/// </remarks>
		public ILInstruction? Parent {
			get { return parent; }
		}

		/// <summary>
		/// Gets the index of this node in the <c>Parent.Children</c> collection.
		/// </summary>
		/// <remarks>
		/// It is temporarily possible for a node to be used in multiple places in the ILAst,
		/// this property returns the index of the primary position of this node (see remarks on <see cref="Parent"/>).
		/// </remarks>
		public int ChildIndex { get; internal set; } = -1;

		/// <summary>
		/// Gets information about the slot in which this instruction is stored.
		/// (i.e., the relation of this instruction to its parent instruction)
		/// </summary>
		/// <remarks>
		/// It is temporarily possible for a node to be used in multiple places in the ILAst,
		/// this property returns the slot of the primary position of this node (see remarks on <see cref="Parent"/>).
		/// 
		/// Precondition: this node must not be orphaned.
		/// </remarks>
		public SlotInfo? SlotInfo {
			get {
				if (parent == null)
					return null;
				Debug.Assert(parent.GetChild(this.ChildIndex) == this);
				return parent.GetChildSlot(this.ChildIndex);
			}
		}

		/// <summary>
		/// Replaces a child of this ILInstruction.
		/// </summary>
		/// <param name="childPointer">Reference to the field holding the child</param>
		/// <param name="newValue">New child</param>
		/// <param name="index">Index of the field in the Children collection</param>
		protected internal void SetChildInstruction<T>(ref T childPointer, T newValue, int index)
			where T : ILInstruction?
		{
			T oldValue = childPointer;
			Debug.Assert(oldValue == GetChild(index));
			if (oldValue == newValue && newValue?.parent == this && newValue.ChildIndex == index)
				return;
			childPointer = newValue;
			if (newValue != null)
			{
				newValue.parent = this;
				newValue.ChildIndex = index;
			}
			InvalidateFlags();
			MakeDirty();
			if (refCount > 0)
			{
				// The new value may be a subtree of the old value.
				// We first call AddRef(), then ReleaseRef() to prevent the subtree
				// that stays connected from receiving a Disconnected() notification followed by a Connected() notification.
				if (newValue != null)
					newValue.AddRef();
				if (oldValue != null)
					oldValue.ReleaseRef();
			}
		}

		/// <summary>
		/// Called when a new child is added to a InstructionCollection.
		/// </summary>
		protected internal void InstructionCollectionAdded(ILInstruction newChild)
		{
			Debug.Assert(GetChild(newChild.ChildIndex) == newChild);
			Debug.Assert(!this.IsDescendantOf(newChild), "ILAst must form a tree");
			// If a call to ReplaceWith() triggers the "ILAst must form a tree" assertion,
			// make sure to read the remarks on the ReplaceWith() method.
			newChild.parent = this;
			if (refCount > 0)
				newChild.AddRef();
		}

		/// <summary>
		/// Called when a child is removed from a InstructionCollection.
		/// </summary>
		protected internal void InstructionCollectionRemoved(ILInstruction oldChild)
		{
			if (refCount > 0)
				oldChild.ReleaseRef();
		}

		/// <summary>
		/// Called when a series of add/remove operations on the InstructionCollection is complete.
		/// </summary>
		protected internal virtual void InstructionCollectionUpdateComplete()
		{
			InvalidateFlags();
			MakeDirty();
		}

		/// <summary>
		/// Creates a deep clone of the ILInstruction.
		/// </summary>
		/// <remarks>
		/// It is valid to clone nodes with stale positions (see remarks on <c>Parent</c>);
		/// the result of such a clone will not contain any stale positions (nodes at
		/// multiple positions will be cloned once per position).
		/// </remarks>
		public abstract ILInstruction Clone();

		/// <summary>
		/// Creates a shallow clone of the ILInstruction.
		/// </summary>
		/// <remarks>
		/// Like MemberwiseClone(), except that the new instruction starts as disconnected.
		/// </remarks>
		protected ILInstruction ShallowClone()
		{
			ILInstruction inst = (ILInstruction)MemberwiseClone();
			// reset refCount and parent so that the cloned instruction starts as disconnected
			inst.refCount = 0;
			inst.parent = null;
			inst.flags = invalidFlags;
#if DEBUG
			inst.activeEnumerators = 0;
#endif
			return inst;
		}

		/// <summary>
		/// Attempts to match the specified node against the pattern.
		/// </summary>
		/// <c>this</c>: The syntactic pattern.
		/// <param name="node">The syntax node to test against the pattern.</param>
		/// <returns>
		/// Returns a match object describing the result of the matching operation.
		/// Check the <see cref="Match.Success"/> property to see whether the match was successful.
		/// For successful matches, the match object allows retrieving the nodes that were matched with the captured groups.
		/// </returns>
		public Match Match(ILInstruction node)
		{
			Match match = new Match();
			match.Success = PerformMatch(node, ref match);
			return match;
		}

		/// <summary>
		/// Attempts matching this instruction against the other instruction.
		/// </summary>
		/// <param name="other">The instruction to compare with.</param>
		/// <param name="match">The match object, used to store global state during the match (such as the results of capture groups).</param>
		/// <returns>Returns whether the (partial) match was successful.
		/// If the method returns true, it adds the capture groups (if any) to the match.
		/// If the method returns false, the match object may remain in a partially-updated state and
		/// needs to be restored before it can be reused.</returns>
		protected internal abstract bool PerformMatch(ILInstruction? other, ref Match match);

		/// <summary>
		/// Attempts matching this instruction against a list of other instructions (or a part of said list).
		/// </summary>
		/// <param name="listMatch">Stores state about the current list match.</param>
		/// <param name="match">The match object, used to store global state during the match (such as the results of capture groups).</param>
		/// <returns>Returns whether the (partial) match was successful.
		/// If the method returns true, it updates listMatch.SyntaxIndex to point to the next node that was not part of the match,
		/// and adds the capture groups (if any) to the match.
		/// If the method returns false, the listMatch and match objects remain in a partially-updated state and need to be restored
		/// before they can be reused.</returns>
		protected internal virtual bool PerformMatch(ref ListMatch listMatch, ref Match match)
		{
			// Base implementation expects the node to match a single element.
			// Any patterns matching 0 or more than 1 element must override this method.
			if (listMatch.SyntaxIndex < listMatch.SyntaxList.Count)
			{
				if (PerformMatch(listMatch.SyntaxList[listMatch.SyntaxIndex], ref match))
				{
					listMatch.SyntaxIndex++;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Extracts the this instruction.
		///   The instruction is replaced with a load of a new temporary variable;
		///   and the instruction is moved to a store to said variable at block-level.
		///   Returns the new variable.
		/// 
		/// If extraction is not possible, the ILAst is left unmodified and the function returns null.
		/// May return null if extraction is not possible.
		/// </summary>
		public ILVariable Extract(ILTransformContext context)
		{
			return Transforms.ExtractionContext.Extract(this, context);
		}

		/// <summary>
		/// Prepares "extracting" a descendant instruction out of this instruction.
		/// This is the opposite of ILInlining. It may involve re-compiling high-level constructs into lower-level constructs.
		/// </summary>
		/// <returns>True if extraction is possible; false otherwise.</returns>
		internal virtual bool PrepareExtract(int childIndex, Transforms.ExtractionContext ctx)
		{
			if (!GetChildSlot(childIndex).CanInlineInto)
			{
				return false;
			}
			// Check whether re-ordering with predecessors is valid:
			for (int i = childIndex - 1; i >= 0; --i)
			{
				ILInstruction predecessor = GetChild(i);
				if (!GetChildSlot(i).CanInlineInto)
				{
					return false;
				}
				ctx.RegisterMoveIfNecessary(predecessor);
			}
			return true;
		}

		/// <summary>
		/// Gets whether the specified instruction may be inlined into the specified slot.
		/// Note: this does not check whether reordering with the previous slots is valid; only wheter the target slot supports inlining at all!
		/// </summary>
		internal virtual bool CanInlineIntoSlot(int childIndex, ILInstruction expressionBeingMoved)
		{
			return GetChildSlot(childIndex).CanInlineInto;
		}
	}

	public interface IInstructionWithTypeOperand
	{
		IType Type { get; }
	}

	public interface IInstructionWithFieldOperand
	{
		IField Field { get; }
	}

	public interface IInstructionWithMethodOperand
	{
		IMethod? Method { get; }
	}

	public interface ILiftableInstruction
	{
		/// <summary>
		/// Gets whether the instruction was lifted; that is, whether is accepts
		/// potentially nullable arguments.
		/// </summary>
		bool IsLifted { get; }

		/// <summary>
		/// If the instruction is lifted and returns a nullable result,
		/// gets the underlying result type.
		/// 
		/// Note that not all lifted instructions return a nullable result:
		/// C# comparisons always return a bool!
		/// </summary>
		StackType UnderlyingResultType { get; }
	}
}
