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

		public ContainerKind Kind { get; set; }
		public StackType ExpectedResultType { get; set; }

		int leaveCount;

		/// <summary>
		/// Gets the number of 'leave' instructions that target this BlockContainer.
		/// </summary>
		public int LeaveCount {
			get => leaveCount;
			internal set {
				leaveCount = value;
				InvalidateFlags();
			}
		}
		
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

		public BlockContainer(ContainerKind kind = ContainerKind.Normal, StackType expectedResultType = StackType.Void) : base(OpCode.BlockContainer)
		{
			this.Kind = kind;
			this.Blocks = new InstructionCollection<Block>(this, 0);
			this.ExpectedResultType = expectedResultType;
		}

		public override ILInstruction Clone()
		{
			BlockContainer clone = new BlockContainer();
			clone.AddILRange(this);
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
			WriteILRange(output, options);
			output.WriteLocalReference("BlockContainer", this, isDefinition: true);
			output.Write(' ');
			switch (Kind) {
				case ContainerKind.Loop:
					output.Write("(while-true) ");
					break;
				case ContainerKind.Switch:
					output.Write("(switch) ");
					break;
				case ContainerKind.While:
					output.Write("(while) ");
					break;
				case ContainerKind.DoWhile:
					output.Write("(do-while) ");
					break;
				case ContainerKind.For:
					output.Write("(for) ");
					break;
			}
			output.MarkFoldStart("{...}");
			output.WriteLine("{");
			output.Indent();
			foreach (var inst in Blocks) {
				if (inst.Parent == this) {
					inst.WriteTo(output, options);
				} else {
					output.Write("stale reference to ");
					output.WriteLocalReference(inst.Label, inst);
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
			Debug.Assert(Blocks.Count > 0 && EntryPoint == Blocks[0]);
			Debug.Assert(!IsConnected || EntryPoint?.IncomingEdgeCount >= 1);
			Debug.Assert(EntryPoint == null || Parent is ILFunction || !HasILRange);
			Debug.Assert(Blocks.All(b => b.HasFlag(InstructionFlags.EndPointUnreachable)));
			Debug.Assert(Blocks.All(b => b.Kind == BlockKind.ControlFlow)); // this also implies that the blocks don't use FinalInstruction
			Debug.Assert(TopologicalSort(deleteUnreachableBlocks: true).Count == Blocks.Count, "Container should not have any unreachable blocks");
			Block bodyStartBlock;
			switch (Kind) {
				case ContainerKind.Normal:
					break;
				case ContainerKind.Loop:
					Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
					break;
				case ContainerKind.Switch:
					Debug.Assert(EntryPoint.Instructions.Count == 1);
					Debug.Assert(EntryPoint.Instructions[0] is SwitchInstruction);
					Debug.Assert(EntryPoint.IncomingEdgeCount == 1);
					break;
				case ContainerKind.While:
					Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
					Debug.Assert(Blocks.Count >= 2);
					Debug.Assert(MatchConditionBlock(EntryPoint, out _, out bodyStartBlock));
					Debug.Assert(bodyStartBlock == Blocks[1]);
					break;
				case ContainerKind.DoWhile:
					Debug.Assert(EntryPoint.IncomingEdgeCount > 1);
					Debug.Assert(Blocks.Count >= 2);
					Debug.Assert(MatchConditionBlock(Blocks.Last(), out _, out bodyStartBlock));
					Debug.Assert(bodyStartBlock == EntryPoint);
					break;
				case ContainerKind.For:
					Debug.Assert(EntryPoint.IncomingEdgeCount == 2);
					Debug.Assert(Blocks.Count >= 3);
					Debug.Assert(MatchConditionBlock(EntryPoint, out _, out bodyStartBlock));
					Debug.Assert(MatchIncrementBlock(Blocks.Last()));
					Debug.Assert(bodyStartBlock == Blocks[1]);
					break;
			}
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
		/// Topologically sort the blocks.
		/// The new order is returned without modifying the BlockContainer.
		/// </summary>
		/// <param name="deleteUnreachableBlocks">If true, unreachable blocks are not included in the new order.</param>
		public List<Block> TopologicalSort(bool deleteUnreachableBlocks = false)
		{
			// Visit blocks in post-order
			BitSet visited = new BitSet(Blocks.Count);
			List<Block> postOrder = new List<Block>();
			Visit(EntryPoint);
			postOrder.Reverse();
			if (!deleteUnreachableBlocks) {
				for (int i = 0; i < Blocks.Count; i++) {
					if (!visited[i])
						postOrder.Add(Blocks[i]);
				}
			}
			return postOrder;

			void Visit(Block block)
			{
				Debug.Assert(block.Parent == this);
				if (!visited[block.ChildIndex]) {
					visited[block.ChildIndex] = true;

					foreach (var branch in block.Descendants.OfType<Branch>()) {
						if (branch.TargetBlock.Parent == this) {
							Visit(branch.TargetBlock);
						}
					}

					postOrder.Add(block);
				}
			};
		}

		/// <summary>
		/// Topologically sort the blocks.
		/// </summary>
		/// <param name="deleteUnreachableBlocks">If true, delete unreachable blocks.</param>
		public void SortBlocks(bool deleteUnreachableBlocks = false)
		{
			if (Blocks.Count < 2)
				return;

			var newOrder = TopologicalSort(deleteUnreachableBlocks);
			Debug.Assert(newOrder[0] == Blocks[0]);
			Blocks.ReplaceList(newOrder);
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

		public bool MatchConditionBlock(Block block, out ILInstruction condition, out Block bodyStartBlock)
		{
			condition = null;
			bodyStartBlock = null;
			if (block.Instructions.Count != 1)
				return false;
			if (!block.Instructions[0].MatchIfInstruction(out condition, out var trueInst, out var falseInst))
				return false;
			return falseInst.MatchLeave(this) && trueInst.MatchBranch(out bodyStartBlock);
		}

		public bool MatchIncrementBlock(Block block)
		{
			if (block.Instructions.Count == 0)
				return false;
			if (!block.Instructions.Last().MatchBranch(EntryPoint))
				return false;
			return true;
		}

		/// <summary>
		/// If the container consists of a single block with a single instruction,
		/// returns that instruction.
		/// Otherwise returns the block, or the container itself if it has multiple blocks.
		/// </summary>
		public ILInstruction SingleInstruction()
		{
			if (Blocks.Count != 1)
				return this;
			if (Blocks[0].Instructions.Count != 1)
				return Blocks[0];
			return Blocks[0].Instructions[0];
		}
	}

	public enum ContainerKind
	{
		/// <summary>
		/// Normal container that contains control-flow blocks.
		/// </summary>
		Normal,
		/// <summary>
		/// A while-true loop.
		/// Continue is represented as branch to entry-point.
		/// Return/break is represented as leave.
		/// </summary>
		Loop,
		/// <summary>
		/// Container that has a switch instruction as entry-point.
		/// Goto case is represented as branch.
		/// Break is represented as leave.
		/// </summary>
		Switch,
		/// <summary>
		/// while-loop.
		/// The entry-point is a block consisting of a single if instruction
		/// that if true: jumps to the head of the loop body,
		/// if false: leaves the block.
		/// Continue is a branch to the entry-point.
		/// Break is a leave.
		/// </summary>
		While,
		/// <summary>
		/// do-while-loop.
		/// The entry-point is a block that is the head of the loop body.
		/// The last block consists of a single if instruction
		/// that if true: jumps to the head of the loop body,
		/// if false: leaves the block.
		/// Only the last block is allowed to jump to the entry-point.
		/// Continue is a branch to the last block.
		/// Break is a leave.
		/// </summary>
		DoWhile,
		/// <summary>
		/// for-loop.
		/// The entry-point is a block consisting of a single if instruction
		/// that if true: jumps to the head of the loop body,
		/// if false: leaves the block.
		/// The last block is the increment block.
		/// Only the last block is allowed to jump to the entry-point.
		/// Continue is a branch to the last block.
		/// Break is a leave.
		/// </summary>
		For
	}
}
