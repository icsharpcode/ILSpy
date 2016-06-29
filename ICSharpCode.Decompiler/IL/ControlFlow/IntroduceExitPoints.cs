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

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Control flow transform: use 'leave' instructions instead of 'br' where possible.
	/// </summary>
	public class IntroduceExitPoints : ILVisitor, IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			function.AcceptVisitor(this);
		}
		
		static readonly Nop NoExit = new Nop();
		static readonly Return ReturnExit = new Return();
		
		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children)
				child.AcceptVisitor(this);
		}
		
		BlockContainer currentContainer;
		ILInstruction currentExit = NoExit;
		
		protected internal override void VisitBlockContainer(BlockContainer container)
		{
			var oldExit = currentExit;
			var oldContainer = currentContainer;
			var thisExit = GetExit(container);
			currentExit = thisExit;
			currentContainer = container;
			base.VisitBlockContainer(container);
			if (thisExit == null && currentExit != null) {
				Debug.Assert(!currentExit.MatchLeave(currentContainer));
				ILInstruction inst = container;
				// traverse up to the block (we'll always find one because GetExit only returns null if there's a block)
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
		
		/// <summary>
		/// Gets the next instruction after <paramref name="inst"/> is executed.
		/// Returns NoExit when the next instruction cannot be identified;
		/// returns <c>null</c> when the end of a Block is reached (so that we could insert an arbitrary instruction)
		/// </summary>
		ILInstruction GetExit(ILInstruction inst)
		{
			SlotInfo slot = inst.SlotInfo;
			if (slot == Block.InstructionSlot) {
				Block block = (Block)inst.Parent;
				return block.Instructions.ElementAtOrDefault(inst.ChildIndex + 1);
			} else if (slot == TryInstruction.TryBlockSlot || slot == TryCatchHandler.BodySlot || slot == TryCatch.HandlerSlot || slot == PinnedRegion.BodySlot) {
				return GetExit(inst.Parent);
			} else if (slot == ILFunction.BodySlot) {
				return ReturnExit;
			}
			return NoExit;
		}
		
		protected internal override void VisitBlock(Block block)
		{
			// Don't use foreach loop, because the children might add to the block
			for (int i = 0; i < block.Instructions.Count; i++) {
				block.Instructions[i].AcceptVisitor(this);
			}
		}
		
		void HandleExit(ILInstruction inst)
		{
			if (currentExit == null) {
				currentExit = inst;
				inst.ReplaceWith(new Leave(currentContainer) { ILRange = inst.ILRange });
			} else if (ConditionDetection.CompatibleExitInstruction(inst, currentExit)) {
				inst.ReplaceWith(new Leave(currentContainer) { ILRange = inst.ILRange });
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
		
		protected internal override void VisitReturn(Return inst)
		{
			if (inst.ReturnValue == null) {
				HandleExit(inst);
			} else {
				base.VisitReturn(inst);
			}
		}
	}
}
