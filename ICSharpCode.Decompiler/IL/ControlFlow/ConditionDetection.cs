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

using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Detects 'if' structure and other non-loop aspects of control flow.
	/// </summary>
	/// <remarks>
	/// Order dependency: should run after loop detection.
	/// Blocks should be basic blocks prior to this transform.
	/// After this transform, they will be extended basic blocks.
	/// </remarks>
	public class ConditionDetection : IBlockTransform
	{
		BlockTransformContext context;

		/// <summary>
		/// Builds structured control flow for the block associated with the control flow node.
		/// </summary>
		/// <remarks>
		/// After a block was processed, it should use structured control flow
		/// and have just a single 'regular' exit point (last branch instruction in the block)
		/// </remarks>
		public void Run(Block block, BlockTransformContext context)
		{
			this.context = context;

			// We only embed blocks into this block if they aren't referenced anywhere else,
			// so those blocks are dominated by this block.
			// BlockILTransform thus guarantees that the blocks being embedded are already
			// fully processed.

			var cfgNode = context.ControlFlowNode;
			Debug.Assert(cfgNode.UserData == block);

			// Because this transform runs at the beginning of the block transforms,
			// we know that `block` is still a (non-extended) basic block.

			// Last instruction is one with unreachable endpoint
			// (guaranteed by combination of BlockContainer and Block invariants)
			Debug.Assert(block.Instructions.Last().HasFlag(InstructionFlags.EndPointUnreachable));
			ILInstruction exitInst = block.Instructions.Last();

			// Previous-to-last instruction might have conditional control flow,
			// usually an IfInstruction with a branch:
			IfInstruction ifInst = block.Instructions.SecondToLastOrDefault() as IfInstruction;
			if (ifInst != null && ifInst.FalseInst.OpCode == OpCode.Nop) {
				HandleIfInstruction(cfgNode, block, ifInst, ref exitInst);
			} else {
				SwitchInstruction switchInst = block.Instructions.SecondToLastOrDefault() as SwitchInstruction;
				if (switchInst != null) {
					HandleSwitchInstruction(cfgNode, block, switchInst, ref exitInst);
				}
			}
			if (IsUsableBranchToChild(cfgNode, exitInst)) {
				// "...; goto usableblock;"
				// -> embed target block in this block
				var targetBlock = ((Branch)exitInst).TargetBlock;
				Debug.Assert(exitInst == block.Instructions.Last());
				block.Instructions.RemoveAt(block.Instructions.Count - 1);
				block.Instructions.AddRange(targetBlock.Instructions);
				targetBlock.Remove();
			}
		}

		private void HandleIfInstruction(ControlFlowNode cfgNode, Block block, IfInstruction ifInst, ref ILInstruction exitInst)
		{
			if (IsBranchToLaterTarget(ifInst.TrueInst, exitInst)) {
				// "if (c) goto lateBlock; goto earlierBlock;"
				// -> "if (!c)" goto earlierBlock; goto lateBlock;
				// This reordering should make the if structure correspond more closely to the original C# source code
				context.Step("Negate if", ifInst);
				block.Instructions[block.Instructions.Count - 1] = ifInst.TrueInst;
				ifInst.TrueInst = exitInst;
				exitInst = block.Instructions.Last();
				ifInst.Condition = new LogicNot(ifInst.Condition);
			}

			ILInstruction trueExitInst;
			if (IsUsableBranchToChild(cfgNode, ifInst.TrueInst)) {
				// "if (...) goto targetBlock; exitInst;"
				// -> "if (...) { targetBlock } exitInst;"
				context.Step("Inline block as then-branch", ifInst);
				var targetBlock = ((Branch)ifInst.TrueInst).TargetBlock;
				// The targetBlock was already processed, we can embed it into the if statement:
				targetBlock.Remove();
				ifInst.TrueInst = targetBlock;
				ILInstruction nestedCondition, nestedTrueInst;
				while (targetBlock.Instructions.Count > 0
					&& targetBlock.Instructions[0].MatchIfInstruction(out nestedCondition, out nestedTrueInst))
				{
					nestedTrueInst = UnpackBlockContainingOnlyBranch(nestedTrueInst);
					if (CompatibleExitInstruction(exitInst, nestedTrueInst)) {
						// "if (...) { if (nestedCondition) goto exitPoint; ... } goto exitPoint;"
						// -> "if (... && !nestedCondition) { ... } goto exitPoint;"
						context.Step("Combine 'if (cond1 && !cond2)' in then-branch", ifInst);
						ifInst.Condition = IfInstruction.LogicAnd(ifInst.Condition, new LogicNot(nestedCondition));
						targetBlock.Instructions.RemoveAt(0);
						// Update targetBlock label now that we've removed the first instruction
						if (targetBlock.Instructions.FirstOrDefault()?.ILRange.IsEmpty == false) {
							int offset = targetBlock.Instructions[0].ILRange.Start;
							targetBlock.ILRange = new Interval(offset, offset);
						}
						continue; // try to find more nested conditions
					} else {
						break;
					}
				}

				trueExitInst = targetBlock.Instructions.LastOrDefault();
				if (CompatibleExitInstruction(exitInst, trueExitInst)) {
					// "if (...) { ...; goto exitPoint } goto exitPoint;"
					// -> "if (...) { ... } goto exitPoint;"
					context.Step("Remove redundant 'goto exitPoint;' in then-branch", ifInst);
					targetBlock.Instructions.RemoveAt(targetBlock.Instructions.Count - 1);
					trueExitInst = null;
					if (targetBlock.Instructions.Count == 1 && targetBlock.Instructions[0].MatchIfInstruction(out nestedCondition, out nestedTrueInst)) {
						// "if (...) { if (nestedCondition) nestedTrueInst; } exitInst;"
						// --> "if (... && nestedCondition) nestedTrueInst; } exitInst"
						context.Step("Combine if conditions into logic.and (in then-branch)", ifInst);
						ifInst.Condition = IfInstruction.LogicAnd(ifInst.Condition, nestedCondition);
						ifInst.TrueInst = nestedTrueInst;
						trueExitInst = (nestedTrueInst as Block)?.Instructions.LastOrDefault();
					}
				}
			} else {
				trueExitInst = ifInst.TrueInst;
			}
			if (IsUsableBranchToChild(cfgNode, exitInst)) {
				var targetBlock = ((Branch)exitInst).TargetBlock;
				var falseExitInst = targetBlock.Instructions.LastOrDefault();
				if (CompatibleExitInstruction(trueExitInst, falseExitInst)) {
					// if (...) { ...; goto exitPoint; } goto nextBlock; nextBlock: ...; goto exitPoint;
					// -> if (...) { ... } else { ... } goto exitPoint;
					context.Step("Inline block as else-branch", ifInst);
					targetBlock.Instructions.RemoveAt(targetBlock.Instructions.Count - 1);
					targetBlock.Remove();
					ifInst.FalseInst = targetBlock;
					exitInst = block.Instructions[block.Instructions.Count - 1] = falseExitInst;
					Block trueBlock = ifInst.TrueInst as Block;
					if (trueBlock != null) {
						Debug.Assert(trueExitInst == trueBlock.Instructions.Last());
						trueBlock.Instructions.RemoveAt(trueBlock.Instructions.Count - 1);
					} else {
						Debug.Assert(trueExitInst == ifInst.TrueInst);
						ifInst.TrueInst = new Nop { ILRange = ifInst.TrueInst.ILRange };
					}
				}
			}
			if (IsEmpty(ifInst.TrueInst)) {
				// prefer empty true-branch to empty-else branch
				context.Step("Swap empty then-branch with else-branch", ifInst);
				var oldTrue = ifInst.TrueInst;
				ifInst.TrueInst = ifInst.FalseInst;
				ifInst.FalseInst = new Nop { ILRange = oldTrue.ILRange };
				ifInst.Condition = new LogicNot(ifInst.Condition);

				// After swapping, it's possible that we can introduce a short-circuit operator:
				Block trueBlock = ifInst.TrueInst as Block;
				ILInstruction nestedCondition, nestedTrueInst;
				if (trueBlock != null && trueBlock.Instructions.Count == 1
					&& trueBlock.FinalInstruction is Nop
					&& trueBlock.Instructions[0].MatchIfInstruction(out nestedCondition, out nestedTrueInst)) {
					// if (cond) if (nestedCond) nestedTrueInst
					// ==> if (cond && nestedCond) nestedTrueInst
					context.Step("Combine if conditions into logic.and (after branch swapping)", ifInst);
					ifInst.Condition = IfInstruction.LogicAnd(ifInst.Condition, nestedCondition);
					ifInst.TrueInst = nestedTrueInst;
				}
			} else if (ifInst.FalseInst.OpCode != OpCode.Nop && ifInst.FalseInst.ILRange.Start < ifInst.TrueInst.ILRange.Start) {
				// swap true and false branches of if/else construct,
				// to bring them in the same order as the IL code
				context.Step("Swap then-branch with else-branch", ifInst);
				var oldTrue = ifInst.TrueInst;
				ifInst.TrueInst = ifInst.FalseInst;
				ifInst.FalseInst = oldTrue;
				ifInst.Condition = new LogicNot(ifInst.Condition);
			}
		}

		static bool IsEmpty(ILInstruction inst)
		{
			var block = inst as Block;
			return block != null && block.Instructions.Count == 0 && block.FinalInstruction is Nop
				|| inst is Nop;
		}

		private ILInstruction UnpackBlockContainingOnlyBranch(ILInstruction inst)
		{
			Block block = inst as Block;
			if (block != null && block.Instructions.Count == 1 && block.FinalInstruction is Nop && IsBranchOrLeave(block.Instructions[0]))
				return block.Instructions.Single();
			else
				return inst;
		}

		bool IsBranchToLaterTarget(ILInstruction inst1, ILInstruction inst2)
		{
			Block block1 = null, block2 = null;
			if (inst1.MatchBranch(out block1) && inst2.MatchBranch(out block2)) {
				return block1.ILRange.Start > block2.ILRange.Start;
			}
			BlockContainer container1, container2;
			if (inst1.MatchLeave(out container1) && container1.Parent is TryInstruction) {
				// 'leave tryBlock' is considered to have a later target than
				// any branch within the container, and also a later target
				// than a return instruction.
				// This is necessary to avoid "goto" statements in the
				// ExceptionHandling.ConditionalReturnInThrow test.
				if (!inst2.MatchLeave(out container2))
					container2 = block2?.Parent as BlockContainer;
				return container2 == null || container2.IsDescendantOf(container1);
			}
			return false;
		}

		bool IsUsableBranchToChild(ControlFlowNode cfgNode, ILInstruction potentialBranchInstruction)
		{
			Branch br = potentialBranchInstruction as Branch;
			if (br == null)
				return false;
			var targetBlock = br.TargetBlock;
			return targetBlock.Parent == context.Container && cfgNode.Dominates(context.GetNode(targetBlock))
				&& targetBlock.IncomingEdgeCount == 1 && targetBlock.FinalInstruction.OpCode == OpCode.Nop;
		}

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
				case OpCode.Return:
					Return ret1 = (Return)exit1;
					Return ret2 = (Return)exit2;
					return ret1.ReturnValue == null && ret2.ReturnValue == null;
				default:
					return false;
			}
		}

		private void HandleSwitchInstruction(ControlFlowNode cfgNode, Block block, SwitchInstruction sw, ref ILInstruction exitInst)
		{
			Debug.Assert(sw.DefaultBody is Nop);
			// First, move blocks into the switch section
			foreach (var section in sw.Sections) {
				if (IsUsableBranchToChild(cfgNode, section.Body)) {
					// case ...: goto targetBlock;
					var targetBlock = ((Branch)section.Body).TargetBlock;
					targetBlock.Remove();
					section.Body = targetBlock;
				}
			}
			// Move the code following the switch into the default section
			if (IsUsableBranchToChild(cfgNode, exitInst)) {
				// switch(...){} goto targetBlock;
				// ---> switch(..) { default: { targetBlock } }
				var targetBlock = ((Branch)exitInst).TargetBlock;
				targetBlock.Remove();
				sw.DefaultBody = targetBlock;
				if (IsBranchOrLeave(targetBlock.Instructions.Last())) {
					exitInst = block.Instructions[block.Instructions.Count - 1] = targetBlock.Instructions.Last();
					targetBlock.Instructions.RemoveAt(targetBlock.Instructions.Count - 1);
				} else {
					exitInst = null;
					block.Instructions.RemoveAt(block.Instructions.Count - 1);
				}
			}
			// Remove compatible exitInsts from switch sections:
			foreach (var section in sw.Sections) {
				Block sectionBlock = section.Body as Block;
				if (sectionBlock != null && exitInst == null && IsBranchOrLeave(sectionBlock.Instructions.Last())) {
					exitInst = sectionBlock.Instructions.Last();
					sectionBlock.Instructions.RemoveAt(sectionBlock.Instructions.Count - 1);
					block.Instructions.Add(exitInst);
				} else if (sectionBlock != null && CompatibleExitInstruction(exitInst, sectionBlock.Instructions.Last())) {
					sectionBlock.Instructions.RemoveAt(sectionBlock.Instructions.Count - 1);
				}
			}
			sw.Sections.ReplaceList(sw.Sections.OrderBy(s => s.Body.ILRange.Start));
		}

		private bool IsBranchOrLeave(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.Branch:
				case OpCode.Leave:
					return true;
				default:
					return false;
			}
		}
	}
}
