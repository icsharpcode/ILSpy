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
	public class DetectPinnedRegions : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var container in function.Descendants.OfType<BlockContainer>()) {
				SplitBlocksAtWritesToPinnedLocals(container);
				foreach (var block in container.Blocks)
					Run(block);
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
			}
		}
		
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

		void Run(Block block)
		{
			// After SplitBlocksAtWritesToPinnedLocals(), only the second-to-last instruction in each block
			// can be a write to a pinned local.
			var stLoc = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 2) as StLoc;
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
					StLoc workStLoc = workItem.Instructions.ElementAtOrDefault(workItem.Instructions.Count - 2) as StLoc;
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
