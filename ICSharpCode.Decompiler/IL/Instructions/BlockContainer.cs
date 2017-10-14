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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A container of IL blocks.
	/// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
	/// and only branches within this container may reference the blocks in this container.
	/// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
	/// and the same holds for every block within the container.
	/// 
	/// All blocks in the container must perform unconditional control flow (falling through to the block end is not allowed).
	/// To exit the block container, use the 'leave' instruction.
	/// </summary>
	partial class BlockContainer : ILInstruction
	{
		public static readonly SlotInfo BlockSlot = new SlotInfo("Block", isCollection: true);
		public readonly InstructionCollection<Block> Blocks;

		public StackType ExpectedResultType { get; set; }
		
		/// <summary>
		/// Gets the number of 'leave' instructions that target this BlockContainer.
		/// </summary>
		public int LeaveCount { get; internal set; }
		
		Block entryPoint;

		/// <summary>
		/// Gets the container's entry point. This is the first block in the Blocks collection.
		/// </summary>
		public Block EntryPoint {
			get { return entryPoint; }
			private set {
				if (entryPoint != null && IsConnected)
					entryPoint.IncomingEdgeCount--;
				entryPoint = value;
				if (entryPoint != null && IsConnected)
					entryPoint.IncomingEdgeCount++;
			}
		}

		public BlockContainer(StackType expectedResultType = StackType.Void) : base(OpCode.BlockContainer)
		{
			this.Blocks = new InstructionCollection<Block>(this, 0);
			this.ExpectedResultType = expectedResultType;
		}

		public override ILInstruction Clone()
		{
			BlockContainer clone = new BlockContainer();
			clone.ILRange = this.ILRange;
			clone.Blocks.AddRange(this.Blocks.Select(block => (Block)block.Clone()));
			// Adjust branch instructions to point to the new container
			foreach (var branch in clone.Descendants.OfType<Branch>()) {
				if (branch.TargetBlock != null && branch.TargetBlock.Parent == this)
					branch.TargetBlock = clone.Blocks[branch.TargetBlock.ChildIndex];
			}
			foreach (var leave in clone.Descendants.OfType<Leave>()) {
				if (leave.TargetContainer == this)
					leave.TargetContainer = clone;
			}
			return clone;
		}
		
		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			this.EntryPoint = this.Blocks.FirstOrDefault();
		}
		
		protected override void Connected()
		{
			base.Connected();
			if (entryPoint != null)
				entryPoint.IncomingEdgeCount++;
		}
		
		protected override void Disconnected()
		{
			base.Disconnected();
			if (entryPoint != null)
				entryPoint.IncomingEdgeCount--;
		}
		
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
			output.WriteDefinition("BlockContainer", this);
			output.Write(' ');
			output.MarkFoldStart("{...}");
			output.WriteLine("{");
			output.Indent();
			foreach (var inst in Blocks) {
				if (inst.Parent == this) {
					inst.WriteTo(output, options);
				} else {
					output.Write("stale reference to ");
					output.WriteReference(inst.Label, inst, isLocal: true);
				}
				output.WriteLine();
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
			output.MarkFoldEnd();
		}
		
		protected override int GetChildCount()
		{
			return Blocks.Count;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			return Blocks[index];
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (Blocks[index] != value)
				throw new InvalidOperationException("Cannot replace blocks in BlockContainer");
		}

		protected override SlotInfo GetChildSlot(int index)
		{
			return BlockSlot;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(EntryPoint == Blocks[0]);
			Debug.Assert(!IsConnected || EntryPoint.IncomingEdgeCount >= 1);
			Debug.Assert(Blocks.All(b => b.HasFlag(InstructionFlags.EndPointUnreachable)));
			Debug.Assert(Blocks.All(b => b.Type == BlockType.ControlFlow)); // this also implies that the blocks don't use FinalInstruction
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			InstructionFlags flags = InstructionFlags.ControlFlow;
			foreach (var block in Blocks) {
				flags |= block.Flags;
			}
			// The end point of the BlockContainer is only reachable if there's a leave instruction
			if (LeaveCount == 0)
				flags |= InstructionFlags.EndPointUnreachable;
			else
				flags &= ~InstructionFlags.EndPointUnreachable;
			return flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}
		
		/// <summary>
		/// Sort the blocks in reverse post-order over the control flow graph between the blocks.
		/// </summary>
		public void SortBlocks(bool deleteUnreachableBlocks = false)
		{
			if (Blocks.Count < 2)
				return;

			// Visit blocks in post-order
			BitSet visited = new BitSet(Blocks.Count);
			List<Block> postOrder = new List<Block>();
			
			Action<Block> visit = null;
			visit = delegate(Block block) {
				Debug.Assert(block.Parent == this);
				if (!visited[block.ChildIndex]) {
					visited[block.ChildIndex] = true;

					foreach (var branch in block.Descendants.OfType<Branch>()) {
						if (branch.TargetBlock.Parent == this) {
							visit(branch.TargetBlock);
						}
					}

					postOrder.Add(block);
				}
			};
			visit(EntryPoint);
			
			postOrder.Reverse();
			if (!deleteUnreachableBlocks) {
				for (int i = 0; i < Blocks.Count; i++) {
					if (!visited[i])
						postOrder.Add(Blocks[i]);
				}
			}
			Debug.Assert(postOrder[0] == Blocks[0]);
			Blocks.ReplaceList(postOrder);
		}

		public static BlockContainer FindClosestContainer(ILInstruction inst)
		{
			while (inst != null) {
				if (inst is BlockContainer bc)
					return bc;
				inst = inst.Parent;
			}
			return null;
		}

		public static BlockContainer FindClosestSwitchContainer(ILInstruction inst)
		{
			while (inst != null) {
				if (inst is BlockContainer bc && bc.entryPoint.Instructions.FirstOrDefault() is SwitchInstruction)
					return bc;
				inst = inst.Parent;
			}
			return null;
		}
	}
}
