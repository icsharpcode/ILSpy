// Copyright (c) 2016 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// IL uses 'pinned locals' to prevent the GC from moving objects.
	/// 
	/// C#:
	/// <code>
	/// fixed (int* s = &amp;arr[index]) { use(s); use(s); }
	/// </code>
	/// 
	/// <para>Gets translated into IL:</para>
	/// <code>
	/// pinned local P : System.Int32&amp;
	/// 
	/// stloc(P, ldelema(arr, index))
	/// call use(conv ref->i(ldloc P))
	/// call use(conv ref->i(ldloc P))
	/// stloc(P, conv i4->u(ldc.i4 0))
	/// </code>
	/// 
	/// In C#, the only way to pin something is to use a fixed block
	/// (or to mess around with GCHandles).
	/// But fixed blocks are scoped, so we need to detect the region affected by the pin.
	/// To ensure we'll be able to collect all blocks in that region, we perform this transform
	/// early, before building any other control flow constructs that aren't as critical for correctness.
	/// 
	/// This means this transform must run before LoopDetection.
	/// To make our detection job easier, we must run after variable inlining.
	/// </summary>
	public class DetectPinRegions : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var container in function.Descendants.OfType<BlockContainer>()) {
				SplitBlocksAtWritesToPinnedLocals(container);
				DetectNullSafeArrayToPointer(container);
				foreach (var block in container.Blocks)
					Run(block);
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
			}
		}
		
		/// <summary>
		/// Ensures that every write to a pinned local is followed by a branch instruction.
		/// This ensures the 'pinning region' does not involve any half blocks, which makes it easier to extract.
		/// </summary>
		void SplitBlocksAtWritesToPinnedLocals(BlockContainer container)
		{
			for (int i = 0; i < container.Blocks.Count; i++) {
				var block = container.Blocks[i];
				for (int j = 0; j < block.Instructions.Count - 1; j++) {
					var inst = block.Instructions[j];
					ILVariable v;
					if (inst.MatchStLoc(out v) && v.Kind == VariableKind.PinnedLocal && block.Instructions[j + 1].OpCode != OpCode.Branch) {
						// split block after j:
						var newBlock = new Block();
						for (int k = j + 1; k < block.Instructions.Count; k++) {
							newBlock.Instructions.Add(block.Instructions[k]);
						}
						newBlock.ILRange = newBlock.Instructions[0].ILRange;
						block.Instructions.RemoveRange(j + 1, newBlock.Instructions.Count);
						block.Instructions.Add(new Branch(newBlock));
						container.Blocks.Insert(i + 1, newBlock);
					}
				}
			}
		}

		#region null-safe array to pointer
		void DetectNullSafeArrayToPointer(BlockContainer container)
		{
			// Detect the following pattern:
			//   ...
			//   stloc V(ldloc S)
			//   if (comp(ldloc S == ldnull)) br B_null_or_empty
			//   br B_not_null
			// }
			// Block B_not_null {
			//   if (conv i->i4 (ldlen(ldloc V))) br B_not_null_and_not_empty
			//   br B_null_or_empty
			// }
			// Block B_not_null_and_not_empty {
			//   stloc P(ldelema(ldloc V, ldc.i4 0, ...))
			//   br B_target
			// }
			// Block B_null_or_empty {
			//   stloc P(conv i4->u(ldc.i4 0))
			//   br B_target
			// }
			// And convert the whole thing into:
			//   ...
			//   stloc P(array.to.pointer(V))
			//   br B_target
			bool modified = false;
			for (int i = 0; i < container.Blocks.Count; i++) {
				var block = container.Blocks[i];
				ILVariable v, p;
				Block targetBlock;
				if (IsNullSafeArrayToPointerPattern(block, out v, out p, out targetBlock)) {
					block.Instructions[block.Instructions.Count - 2] = new StLoc(p, new ArrayToPointer(new LdLoc(v)));
					((Branch)block.Instructions.Last()).TargetBlock = targetBlock;
					modified = true;
				}
			}
			if (modified) {
				container.Blocks.RemoveAll(b => b.IncomingEdgeCount == 0); // remove blocks made unreachable
			}
		}
		
		bool IsNullSafeArrayToPointerPattern(Block block, out ILVariable v, out ILVariable p, out Block targetBlock)
		{
			v = null;
			p = null;
			targetBlock = null;
			// ...
			// if (comp(ldloc V == ldnull)) br B_null_or_empty
			// br B_not_null
			var ifInst = block.Instructions.SecondToLastOrDefault() as IfInstruction;
			if (ifInst == null)
				return false;
			var condition = ifInst.Condition as Comp;
			if (!(condition != null && condition.Kind == ComparisonKind.Equality && condition.Left.MatchLdLoc(out v) && condition.Right.MatchLdNull()))
				return false;
			if (v.Kind == VariableKind.StackSlot) {
				// If the variable is a stack slot, that might be due to an inline assignment,
				// so check the previous instruction:
				var previous = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;
				if (previous != null && previous.Value.MatchLdLoc(v)) {
					// stloc V(ldloc S)
					// if (comp(ldloc S == ldnull)) ...
					v = previous.Variable;
				}
			}
			Block nullOrEmptyBlock, notNullBlock;
			return ifInst.TrueInst.MatchBranch(out nullOrEmptyBlock)
				&& ifInst.FalseInst.MatchNop()
				&& nullOrEmptyBlock.Parent == block.Parent
				&& IsNullSafeArrayToPointerNullOrEmptyBlock(nullOrEmptyBlock, out p, out targetBlock)
				&& block.Instructions.Last().MatchBranch(out notNullBlock)
				&& notNullBlock.Parent == block.Parent
				&& IsNullSafeArrayToPointerNotNullBlock(notNullBlock, v, p, nullOrEmptyBlock, targetBlock);
		}

		bool IsNullSafeArrayToPointerNotNullBlock(Block block, ILVariable v, ILVariable p, Block nullOrEmptyBlock, Block targetBlock)
		{
			// Block B_not_null {
			//   if (conv i->i4 (ldlen(ldloc V))) br B_not_null_and_not_empty
			//   br B_null_or_empty
			// }
			ILInstruction condition, trueInst, array;
			Block notNullAndNotEmptyBlock;
			return block.Instructions.Count == 2
				&& block.Instructions[0].MatchIfInstruction(out condition, out trueInst)
				&& condition.UnwrapConv(ConversionKind.Truncate).MatchLdLen(StackType.I, out array)
				&& array.MatchLdLoc(v)
				&& trueInst.MatchBranch(out notNullAndNotEmptyBlock)
				&& notNullAndNotEmptyBlock.Parent == block.Parent
				&& IsNullSafeArrayToPointerNotNullAndNotEmptyBlock(notNullAndNotEmptyBlock, v, p, targetBlock)
				&& block.Instructions[1].MatchBranch(nullOrEmptyBlock);
		}
		
		bool IsNullSafeArrayToPointerNotNullAndNotEmptyBlock(Block block, ILVariable v, ILVariable p, Block targetBlock)
		{
			// Block B_not_null_and_not_empty {
			//   stloc P(ldelema(ldloc V, ldc.i4 0, ...))
			//   br B_target
			// }
			ILInstruction value;
			return block.Instructions.Count == 2
				&& block.Instructions[0].MatchStLoc(p, out value)
				&& value.OpCode == OpCode.LdElema
				&& ((LdElema)value).Array.MatchLdLoc(v)
				&& ((LdElema)value).Indices.All(i => i.MatchLdcI4(0))
				&& block.Instructions[1].MatchBranch(targetBlock);
		}
		
		bool IsNullSafeArrayToPointerNullOrEmptyBlock(Block block, out ILVariable p, out Block targetBlock)
		{
			p = null;
			targetBlock = null;
			// Block B_null_or_empty {
			//   stloc P(conv i4->u(ldc.i4 0))
			//   br B_target
			// }
			ILInstruction value;
			return block.Instructions.Count == 2
				&& block.Instructions[0].MatchStLoc(out p, out value)
				&& p.Kind == VariableKind.PinnedLocal
				&& IsNullOrZero(value)
				&& block.Instructions[1].MatchBranch(out targetBlock);
		}
		#endregion

		void Run(Block block)
		{
			// After SplitBlocksAtWritesToPinnedLocals(), only the second-to-last instruction in each block
			// can be a write to a pinned local.
			var stLoc = block.Instructions.SecondToLastOrDefault() as StLoc;
			if (stLoc == null || stLoc.Variable.Kind != VariableKind.PinnedLocal)
				return;
			// stLoc is a store to a pinned local.
			if (IsNullOrZero(stLoc.Value))
				return; // ignore unpin instructions
			// stLoc is a store that starts a new pinned region
			
			// Collect the blocks to be moved into the region:
			BlockContainer sourceContainer = (BlockContainer)block.Parent;
			int[] reachedEdgesPerBlock = new int[sourceContainer.Blocks.Count];
			Queue<Block> workList = new Queue<Block>();
			Block entryBlock = ((Branch)block.Instructions.Last()).TargetBlock;
			if (entryBlock.Parent == sourceContainer) {
				reachedEdgesPerBlock[entryBlock.ChildIndex]++;
				workList.Enqueue(entryBlock);
				while (workList.Count > 0) {
					Block workItem = workList.Dequeue();
					StLoc workStLoc = workItem.Instructions.SecondToLastOrDefault() as StLoc;
					int instructionCount;
					if (workStLoc != null && workStLoc.Variable == stLoc.Variable && IsNullOrZero(workStLoc.Value)) {
						// found unpin instruction: only consider branches prior to that instruction
						instructionCount = workStLoc.ChildIndex;
					} else {
						instructionCount = workItem.Instructions.Count;
					}
					for (int i = 0; i < instructionCount; i++) {
						foreach (var branch in workItem.Instructions[i].Descendants.OfType<Branch>()) {
							if (branch.TargetBlock.Parent == sourceContainer) {
								Debug.Assert(branch.TargetBlock != block);
								if (reachedEdgesPerBlock[branch.TargetBlock.ChildIndex]++ == 0) {
									// detected first edge to that block: add block as work item
									workList.Enqueue(branch.TargetBlock);
								}
							}
						}
					}
				}
				
				// Validate that all uses of a block consistently are inside or outside the pinned region.
				// (we cannot do this anymore after we start moving blocks around)
				for (int i = 0; i < sourceContainer.Blocks.Count; i++) {
					if (reachedEdgesPerBlock[i] != 0 && reachedEdgesPerBlock[i] != sourceContainer.Blocks[i].IncomingEdgeCount) {
						return;
					}
				}
				
				BlockContainer body = new BlockContainer();
				for (int i = 0; i < sourceContainer.Blocks.Count; i++) {
					if (reachedEdgesPerBlock[i] > 0) {
						body.Blocks.Add(sourceContainer.Blocks[i]); // move block into body
						sourceContainer.Blocks[i] = new Block(); // replace with dummy block
						// we'll delete the dummy block later
					}
				}
				
				block.Instructions[block.Instructions.Count - 2] = new PinnedRegion(stLoc, body);
			} else {
				// we didn't find a single block to be added to the pinned region
				block.Instructions[block.Instructions.Count - 2] = new PinnedRegion(stLoc, new Nop());
			}
			block.Instructions.RemoveAt(block.Instructions.Count - 1);
		}

		static bool IsNullOrZero(ILInstruction inst)
		{
			var conv = inst as Conv;
			if (conv != null) {
				inst = conv.Argument;
			}
			return inst.MatchLdcI4(0) || inst.MatchLdNull();
		}
	}
}
