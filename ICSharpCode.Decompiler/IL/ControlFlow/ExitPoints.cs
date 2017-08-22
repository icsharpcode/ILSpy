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
using System.Diagnostics;
using ICSharpCode.Decompiler.IL.Transforms;
using System.Threading;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Detect suitable exit points for BlockContainers.
	/// 
	/// An "exit point" is an instruction that causes control flow
	/// to leave the container (a branch or leave instruction).
	/// 
	/// If an "exit point" instruction is placed immediately following a
	/// block container, each equivalent exit point within the container
	/// can be replaced with a "leave container" instruction.
	/// 
	/// This transform performs this replacement: any exit points
	/// equivalent to the exit point following the container are
	/// replaced with a leave instruction.
	/// Additionally, if the container is not yet followed by an exit point,
	/// but has room to introduce such an exit point (i.e. iff the container's 
	/// end point is currently unreachable), we pick one of the non-return
	/// exit points within the container, move it to the position following the
	/// container, and replace all instances within the container with a leave
	/// instruction.
	/// 
	/// This makes it easier for the following transforms to construct
	/// control flow that falls out of blocks instead of using goto/break statements.
	/// </summary>
	public class DetectExitPoints : ILVisitor, IILTransform
	{
		static readonly Nop ExitNotYetDetermined = new Nop();
		static readonly Nop NoExit = new Nop();

		bool canIntroduceExitForReturn;

		public DetectExitPoints(bool canIntroduceExitForReturn)
		{
			this.canIntroduceExitForReturn = canIntroduceExitForReturn;
		}

		/// <summary>
		/// Gets the next instruction after <paramref name="inst"/> is executed.
		/// Returns NoExit when the next instruction cannot be identified;
		/// returns <c>null</c> when the end of a Block is reached (so that we could insert an arbitrary instruction)
		/// </summary>
		internal static ILInstruction GetExit(ILInstruction inst)
		{
			SlotInfo slot = inst.SlotInfo;
			if (slot == Block.InstructionSlot) {
				Block block = (Block)inst.Parent;
				return block.Instructions.ElementAtOrDefault(inst.ChildIndex + 1) ?? ExitNotYetDetermined;
			} else if (slot == TryInstruction.TryBlockSlot
				|| slot == TryCatchHandler.BodySlot
				|| slot == TryCatch.HandlerSlot
				|| slot == PinnedRegion.BodySlot)
			{
				return GetExit(inst.Parent);
			}
			return NoExit;
		}

		/// <summary>
		/// Returns true iff exit1 and exit2 are both exit instructions
		/// (branch or leave) and both represent the same exit.
		/// </summary>
		internal static bool CompatibleExitInstruction(ILInstruction exit1, ILInstruction exit2)
		{
			if (exit1 == null || exit2 == null || exit1.OpCode != exit2.OpCode)
				return false;
			switch (exit1.OpCode) {
				case OpCode.Branch:
					Branch br1 = (Branch)exit1;
					Branch br2 = (Branch)exit2;
					return br1.TargetBlock == br2.TargetBlock;
				case OpCode.Leave:
					Leave leave1 = (Leave)exit1;
					Leave leave2 = (Leave)exit2;
					return leave1.TargetContainer == leave2.TargetContainer;
				default:
					return false;
			}
		}

		CancellationToken cancellationToken;
		BlockContainer currentContainer;

		/// <summary>
		/// The instruction that will be executed next after leaving the currentContainer.
		/// <c>null</c> means the container is last in its parent block, and thus does not
		/// yet have any leave instructions. This means we can move any exit instruction of
		/// our choice our of the container and replace it with a leave instruction.
		/// </summary>
		ILInstruction currentExit;

		public void Run(ILFunction function, ILTransformContext context)
		{
			cancellationToken = context.CancellationToken;
			currentExit = NoExit;
			function.AcceptVisitor(this);
		}
		
		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children)
				child.AcceptVisitor(this);
		}
		
		protected internal override void VisitBlockContainer(BlockContainer container)
		{
			var oldExit = currentExit;
			var oldContainer = currentContainer;
			var thisExit = GetExit(container);
			currentExit = thisExit;
			currentContainer = container;
			base.VisitBlockContainer(container);
			if (thisExit == ExitNotYetDetermined && currentExit != ExitNotYetDetermined) {
				// This transform determined an exit point.
				Debug.Assert(!currentExit.MatchLeave(currentContainer));
				ILInstruction inst = container;
				// traverse up to the block (we'll always find one because GetExit
				// only returns ExitNotYetDetermined if there's a block)
				while (inst.Parent.OpCode != OpCode.Block)
					inst = inst.Parent;
				Block block = (Block)inst.Parent;
				block.Instructions.Add(currentExit);
			} else {
				Debug.Assert(thisExit == currentExit);
			}
			currentExit = oldExit;
			currentContainer = oldContainer;
		}

		protected internal override void VisitBlock(Block block)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// Don't use foreach loop, because the children might add to the block
			for (int i = 0; i < block.Instructions.Count; i++) {
				block.Instructions[i].AcceptVisitor(this);
			}
		}

		void HandleExit(ILInstruction inst)
		{
			if (currentExit == ExitNotYetDetermined && CanIntroduceAsExit(inst)) {
				currentExit = inst;
				inst.ReplaceWith(new Leave(currentContainer) { ILRange = inst.ILRange });
			} else if (CompatibleExitInstruction(inst, currentExit)) {
				inst.ReplaceWith(new Leave(currentContainer) { ILRange = inst.ILRange });
			}
		}

		private bool CanIntroduceAsExit(ILInstruction inst)
		{
			if (currentContainer.LeaveCount > 0) {
				// if we're re-running on a block container that already has an exit,
				// we can't introduce any additional exits
				return false;
			}
			if (inst is Leave l && l.IsLeavingFunction) {
				return canIntroduceExitForReturn;
			} else {
				return true;
			}
		}

		protected internal override void VisitBranch(Branch inst)
		{
			if (!inst.TargetBlock.IsDescendantOf(currentContainer)) {
				HandleExit(inst);
			}
		}

		protected internal override void VisitLeave(Leave inst)
		{
			HandleExit(inst);
		}
	}

	/*
	/// <summary>
	/// Like DetectExitPoints, but only uses existing exit points
	/// (by replacing compatible instructions with Leave),
	/// without introducing any exit points even if it were possible.
	/// </summary>
	class UseExitPoints : IBlockTransform
	{
		BlockContainer currentContainer;
		ILInstruction exitPoint;

		public void Run(Block block, BlockTransformContext context)
		{
			Debug.Assert(block.Parent == context.Container);
			currentContainer = context.Container;
			exitPoint = DetectExitPoints.GetExit(context.Container);
			Visit(block);
		}

		void Visit(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.Leave:
				case OpCode.Branch:
					if (DetectExitPoints.CompatibleExitInstruction(inst, exitPoint)) {
						inst.ReplaceWith(new Leave(currentContainer) { ILRange = inst.ILRange });
					}
					break;
				case OpCode.Block:
					// This is a post-order block transform; skip over nested blocks
					// as those are already processed.
					return;
			}
			foreach (var c in inst.Children) {
				Visit(c);
			}
		}
	}
	*/
}
