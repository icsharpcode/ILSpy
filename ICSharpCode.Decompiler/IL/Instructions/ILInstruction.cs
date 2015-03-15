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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.Decompiler.IL
{
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
		
		internal static void ValidateArgument(ILInstruction inst)
		{
			if (inst == null)
				throw new ArgumentNullException("inst");
			if (inst.ResultType == StackType.Void)
				throw new ArgumentException("Argument must not be of type void", "inst");
		}
		
		internal void ValidateChild(ILInstruction inst)
		{
			if (inst == null)
				throw new ArgumentNullException("inst");
			Debug.Assert(!this.IsDescendantOf(inst), "ILAst must form a tree");
		}
		
		[Conditional("DEBUG")]
		internal virtual void CheckInvariant()
		{
			foreach (var child in Children) {
				Debug.Assert(child.Parent == this);
				// if child flags are invalid, parent flags must be too
				Debug.Assert(child.flags != invalidFlags || this.flags == invalidFlags);
				Debug.Assert(child.IsConnected == this.IsConnected);
				child.CheckInvariant();
			}
		}
		
		/// <summary>
		/// Gets whether this node is a descendant of <paramref name="possibleAncestor"/>.
		/// Also returns true if <c>this</c>==<paramref name="possibleAncestor"/>.
		/// </summary>
		public bool IsDescendantOf(ILInstruction possibleAncestor)
		{
			for (ILInstruction ancestor = this; ancestor != null; ancestor = ancestor.Parent) {
				if (ancestor == possibleAncestor)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the stack type of the value produced by this instruction.
		/// </summary>
		public abstract StackType ResultType { get; }
		
		internal static StackType CommonResultType(StackType a, StackType b)
		{
			if (a == StackType.I || b == StackType.I)
				return StackType.I;
			Debug.Assert(a == b);
			return a;
		}
		
		const InstructionFlags invalidFlags = (InstructionFlags)(-1);
		
		InstructionFlags flags = invalidFlags;
		
		public InstructionFlags Flags {
			get {
				if (flags == invalidFlags) {
					flags = ComputeFlags();
				}
				return flags;
			}
		}

		/// <summary>
		/// Returns whether the instruction has at least one of the specified flags.
		/// </summary>
		public bool HasFlag(InstructionFlags flags)
		{
			return (this.Flags & flags) != 0;
		}
		
		protected void InvalidateFlags()
		{
			for (ILInstruction inst = this; inst != null && inst.flags != invalidFlags; inst = inst.parent)
				inst.flags = invalidFlags;
		}
		
		protected abstract InstructionFlags ComputeFlags();
		
		/// <summary>
		/// Gets the ILRange for this instruction alone, ignoring the operands.
		/// </summary>
		public Interval ILRange;

		/// <summary>
		/// Writes the ILAst to the text output.
		/// </summary>
		public abstract void WriteTo(ITextOutput output);

		public override string ToString()
		{
			var output = new PlainTextOutput();
			WriteTo(output);
			return output.ToString();
		}
		
		/// <summary>
		/// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
		/// </summary>
		public abstract T AcceptVisitor<T>(ILVisitor<T> visitor);
		
		/// <summary>
		/// Gets the child nodes of this instruction.
		/// </summary>
		public abstract IEnumerable<ILInstruction> Children { get; }
		
		/// <summary>
		/// Transforms the children of this instruction by applying the specified visitor.
		/// </summary>
		public abstract void TransformChildren(ILVisitor<ILInstruction> visitor);
		
		/// <summary>
		/// Returns all descendants of the ILInstruction.
		/// </summary>
		public IEnumerable<ILInstruction> Descendants {
			get {
				return TreeTraversal.PreOrder(Children, inst => inst.Children);
			}
		}
		
		/// <summary>
		/// Attempts inlining from the inline context into this instruction.
		/// </summary>
		/// <param name="flagsBefore">Combined instruction flags of the instructions
		/// that the instructions getting inlined would get moved over.</param>
		/// <param name="context">The inline context providing the values on the evaluation stack.</param>
		/// <returns>
		/// Returns the modified ILInstruction after inlining is complete.
		/// Note that inlining modifies the AST in-place, so this method usually returns <c>this</c>
		/// (unless <c>this</c> should be replaced by another node)
		/// </returns>
		/// <remarks>
		/// Inlining from an inline context representing the actual evaluation stack
		/// is equivalent to phase-1 execution of the instruction.
		/// </remarks>
		internal abstract ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context);

		/// <summary>
		/// Transforms the evaluation stack 'pop' and 'peek' instructions into local copy.
		/// </summary>
		internal abstract void TransformStackIntoVariables(TransformStackIntoVariablesState state);
		
		/// <summary>
		/// Number of parents that refer to this instruction and are connected to the root.
		/// Usually is 0 for unconnected nodes and 1 for connected nodes, but may temporarily increase to 2
		/// when the ILAst is re-arranged (e.g. within SetChildInstruction).
		/// </summary>
		byte refCount;
		
		internal void AddRef()
		{
			if (refCount++ == 0) {
				Connected();
			}
		}
		
		internal void ReleaseRef()
		{
			Debug.Assert(refCount > 0);
			if (--refCount == 0) {
				Disconnected();
			}
		}
		
		protected bool IsConnected {
			get { return refCount > 0; }
		}
		
		protected virtual void Connected()
		{
			foreach (var child in Children)
				child.AddRef();
		}
		
		protected virtual void Disconnected()
		{
			foreach (var child in Children)
				child.ReleaseRef();
		}
		
		ILInstruction parent;
		
		/// <summary>
		/// Gets the parent of this ILInstruction.
		/// </summary>
		public ILInstruction Parent {
			get { return parent; }
		}
		
		protected void SetChildInstruction(ref ILInstruction childPointer, ILInstruction newValue)
		{
			if (childPointer == newValue)
				return;
			if (refCount > 0) {
				// The new value may be a subtree of the old value.
				// We first call AddRef(), then ReleaseRef() to prevent the subtree
				// that stays connected from receiving a Disconnected() notification followed by a Connected() notification.
				newValue.AddRef();
				childPointer.ReleaseRef();
			}
			childPointer = newValue;
			newValue.parent = this;
			InvalidateFlags();
		}
		
		/// <summary>
		/// Called when a new child is added to a InstructionCollection.
		/// </summary>
		protected internal void InstructionCollectionAdded(ILInstruction newChild)
		{
			if (refCount > 0)
				newChild.AddRef();
			newChild.parent = this;
		}
		
		/// <summary>
		/// Called when a child is removed from a InstructionCollection.
		/// </summary>
		protected internal void InstructionCollectionRemoved(ILInstruction newChild)
		{
			if (refCount > 0)
				newChild.ReleaseRef();
		}
		
		/// <summary>
		/// Called when a series of add/remove operations on the InstructionCollection is complete.
		/// </summary>
		protected internal virtual void InstructionCollectionUpdateComplete()
		{
			InvalidateFlags();
		}
	}
}
