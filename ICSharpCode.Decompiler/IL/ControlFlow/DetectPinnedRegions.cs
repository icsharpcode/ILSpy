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
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

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
				context.CancellationToken.ThrowIfCancellationRequested();
				SplitBlocksAtWritesToPinnedLocals(container);
				DetectNullSafeArrayToPointer(container);
				foreach (var block in container.Blocks)
					CreatePinnedRegion(block);
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
			}
			// Sometimes there's leftover writes to the original pinned locals
			foreach (var block in function.Descendants.OfType<Block>()) {
				context.CancellationToken.ThrowIfCancellationRequested();
				for (int i = 0; i < block.Instructions.Count; i++) {
					var stloc = block.Instructions[i] as StLoc;
					if (stloc != null && stloc.Variable.Kind == VariableKind.PinnedLocal && stloc.Variable.LoadCount == 0 && stloc.Variable.AddressCount == 0) {
						if (SemanticHelper.IsPure(stloc.Value.Flags)) {
							block.Instructions.RemoveAt(i--);
						} else {
							stloc.ReplaceWith(stloc.Value);
						}
					}
				}
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

		#region CreatePinnedRegion
		bool CreatePinnedRegion(Block block)
		{
			// After SplitBlocksAtWritesToPinnedLocals(), only the second-to-last instruction in each block
			// can be a write to a pinned local.
			var stLoc = block.Instructions.SecondToLastOrDefault() as StLoc;
			if (stLoc == null || stLoc.Variable.Kind != VariableKind.PinnedLocal)
				return false;
			// stLoc is a store to a pinned local.
			if (IsNullOrZero(stLoc.Value))
				return false; // ignore unpin instructions
			// stLoc is a store that starts a new pinned region
			
			// Collect the blocks to be moved into the region:
			BlockContainer sourceContainer = (BlockContainer)block.Parent;
			int[] reachedEdgesPerBlock = new int[sourceContainer.Blocks.Count];
			Queue<Block> workList = new Queue<Block>();
			Block entryBlock = ((Branch)block.Instructions.Last()).TargetBlock;
			if (entryBlock.Parent != sourceContainer) {
				// we didn't find a single block to be added to the pinned region
				return false;
			}
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
					return false;
				}
			}
			
			BlockContainer body = new BlockContainer();
			for (int i = 0; i < sourceContainer.Blocks.Count; i++) {
				if (reachedEdgesPerBlock[i] > 0) {
					var innerBlock = sourceContainer.Blocks[i];
					Branch br = innerBlock.Instructions.LastOrDefault() as Branch;
					if (br != null && br.TargetContainer == sourceContainer && reachedEdgesPerBlock[br.TargetBlock.ChildIndex] == 0) {
						// branch that leaves body.
						// Should have an instruction that resets the pin; delete that instruction:
						StLoc innerStLoc = innerBlock.Instructions.SecondToLastOrDefault() as StLoc;
						if (innerStLoc != null && innerStLoc.Variable == stLoc.Variable && IsNullOrZero(innerStLoc.Value)) {
							innerBlock.Instructions.RemoveAt(innerBlock.Instructions.Count - 2);
						}
					}
					
					body.Blocks.Add(innerBlock); // move block into body
					sourceContainer.Blocks[i] = new Block(); // replace with dummy block
					// we'll delete the dummy block later
				}
			}
			
			stLoc.ReplaceWith(new PinnedRegion(stLoc.Variable, stLoc.Value, body));
			block.Instructions.RemoveAt(block.Instructions.Count - 1); // remove branch into body
			ProcessPinnedRegion((PinnedRegion)block.Instructions.Last());
			return true;
		}

		static bool IsNullOrZero(ILInstruction inst)
		{
			while (inst is Conv conv) {
				inst = conv.Argument;
			}
			return inst.MatchLdcI4(0) || inst.MatchLdNull();
		}
		#endregion
		
		#region ProcessPinnedRegion
		/// <summary>
		/// After a pinned region was detected; process its body; replacing the pin variable
		/// with a native pointer as far as possible.
		/// </summary>
		void ProcessPinnedRegion(PinnedRegion pinnedRegion)
		{
			BlockContainer body = (BlockContainer)pinnedRegion.Body;
			if (pinnedRegion.Variable.Type.Kind == TypeKind.ByReference) {
				// C# doesn't support a "by reference" variable, so replace it with a native pointer
				ILVariable oldVar = pinnedRegion.Variable;
				ILVariable newVar = new ILVariable(
					VariableKind.PinnedLocal,
					new PointerType(((ByReferenceType)oldVar.Type).ElementType),
					oldVar.Index);
				newVar.Name = oldVar.Name;
				newVar.HasGeneratedName = oldVar.HasGeneratedName;
				oldVar.Function.Variables.Add(newVar);
				ReplacePinnedVar(oldVar, newVar, pinnedRegion);
			} else if (pinnedRegion.Variable.Type.IsKnownType(KnownTypeCode.String)) {
				// fixing a string
				ILVariable nativeVar;
				ILInstruction initInst;
				// stloc nativeVar(conv o->i (ldloc pinnedVar))
				// if (comp(ldloc nativeVar == conv i4->i <sign extend>(ldc.i4 0))) br targetBlock
				// br adjustOffsetToStringData
				Block targetBlock, adjustOffsetToStringData;
				if (body.EntryPoint.IncomingEdgeCount == 1
				    && body.EntryPoint.Instructions.Count == 3
				    && body.EntryPoint.Instructions[0].MatchStLoc(out nativeVar, out initInst)
				    && nativeVar.Type.GetStackType() == StackType.I
				    && nativeVar.StoreCount == 2
				    && initInst.UnwrapConv(ConversionKind.StopGCTracking).MatchLdLoc(pinnedRegion.Variable)
				    && IsBranchOnNull(body.EntryPoint.Instructions[1], nativeVar, out targetBlock)
				    && targetBlock.Parent == body
					&& body.EntryPoint.Instructions[2].MatchBranch(out adjustOffsetToStringData)
				    && adjustOffsetToStringData.Parent == body && adjustOffsetToStringData.IncomingEdgeCount == 1
					&& IsOffsetToStringDataBlock(adjustOffsetToStringData, nativeVar, targetBlock))
				{
					// remove old entry point
					body.Blocks.RemoveAt(0);
					body.Blocks.RemoveAt(adjustOffsetToStringData.ChildIndex);
					// make targetBlock the new entry point
					body.Blocks.RemoveAt(targetBlock.ChildIndex);
					body.Blocks.Insert(0, targetBlock);
					pinnedRegion.Init = new ArrayToPointer(pinnedRegion.Init);
					
					ILVariable otherVar;
					ILInstruction otherVarInit;
					// In optimized builds, the 'nativeVar' may end up being a stack slot,
					// and only gets assigned to a real variable after the offset adjustment.
					if (nativeVar.Kind == VariableKind.StackSlot && nativeVar.LoadCount == 1
					    && body.EntryPoint.Instructions[0].MatchStLoc(out otherVar, out otherVarInit)
					    && otherVarInit.MatchLdLoc(nativeVar)
					    && otherVar.IsSingleDefinition)
					{
						body.EntryPoint.Instructions.RemoveAt(0);
						nativeVar = otherVar;
					}
					ILVariable newVar;
					if (nativeVar.Kind == VariableKind.Local) {
						newVar = new ILVariable(VariableKind.PinnedLocal, nativeVar.Type, nativeVar.Index);
						newVar.Name = nativeVar.Name;
						newVar.HasGeneratedName = nativeVar.HasGeneratedName;
						nativeVar.Function.Variables.Add(newVar);
						ReplacePinnedVar(nativeVar, newVar, pinnedRegion);
					} else {
						newVar = nativeVar;
					}
					ReplacePinnedVar(pinnedRegion.Variable, newVar, pinnedRegion);
				}
			}
			// Detect nested pinned regions:
			foreach (var block in body.Blocks)
				CreatePinnedRegion(block);
			body.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
		}

		void ReplacePinnedVar(ILVariable oldVar, ILVariable newVar, ILInstruction inst)
		{
			Debug.Assert(newVar.StackType == StackType.I);
			if (inst is Conv conv && conv.Kind == ConversionKind.StopGCTracking && conv.Argument.MatchLdLoc(oldVar) && conv.ResultType == newVar.StackType) {
				// conv ref->i (ldloc oldVar)
				//  => ldloc newVar
				conv.AddILRange(conv.Argument.ILRange);
				conv.ReplaceWith(new LdLoc(newVar) { ILRange = conv.ILRange });
				return;
			}
			if (inst is IInstructionWithVariableOperand iwvo && iwvo.Variable == oldVar) {
				iwvo.Variable = newVar;
				if (inst is StLoc stloc && oldVar.Type.Kind == TypeKind.ByReference) {
					stloc.Value = new Conv(stloc.Value, PrimitiveType.I, false, Sign.None);
				}
				if ((inst is LdLoc || inst is StLoc) && !IsSlotAcceptingBothManagedAndUnmanagedPointers(inst.SlotInfo) && oldVar.StackType != StackType.I) {
					// wrap inst in Conv, so that the stack types match up
					var children = inst.Parent.Children;
					children[inst.ChildIndex] = new Conv(inst, PrimitiveType.I, false, Sign.None);
				}
			} else if (inst.MatchLdStr(out var val) && val == "Is this ILSpy?") {
				inst.ReplaceWith(new LdStr("This is ILSpy!")); // easter egg ;)
				return;
			}
			foreach (var child in inst.Children) {
				ReplacePinnedVar(oldVar, newVar, child);
			}
		}

		private bool IsSlotAcceptingBothManagedAndUnmanagedPointers(SlotInfo slotInfo)
		{
			return slotInfo == Block.InstructionSlot || slotInfo == LdObj.TargetSlot || slotInfo == StObj.TargetSlot;
		}

		bool IsBranchOnNull(ILInstruction condBranch, ILVariable nativeVar, out Block targetBlock)
		{
			targetBlock = null;
			// if (comp(ldloc nativeVar == conv i4->i <sign extend>(ldc.i4 0))) br targetBlock
			ILInstruction condition, trueInst, left, right;
			return condBranch.MatchIfInstruction(out condition, out trueInst)
				&& condition.MatchCompEquals(out left, out right)
				&& left.MatchLdLoc(nativeVar) && IsNullOrZero(right)
				&& trueInst.MatchBranch(out targetBlock);
		}
		
		bool IsOffsetToStringDataBlock(Block block, ILVariable nativeVar, Block targetBlock)
		{
			// stloc nativeVar(add(ldloc nativeVar, conv i4->i <sign extend>(call [Accessor System.Runtime.CompilerServices.RuntimeHelpers.get_OffsetToStringData():System.Int32]())))
			// br IL_0011
			ILInstruction left, right, value;
			return block.Instructions.Count == 2
				&& block.Instructions[0].MatchStLoc(nativeVar, out value)
				&& value.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out left, out right)
				&& left.MatchLdLoc(nativeVar)
				&& IsOffsetToStringDataCall(right)
				&& block.Instructions[1].MatchBranch(targetBlock);
		}

		bool IsOffsetToStringDataCall(ILInstruction inst)
		{
			Call call = inst.UnwrapConv(ConversionKind.SignExtend) as Call;
			return call != null && call.Method.FullName == "System.Runtime.CompilerServices.RuntimeHelpers.get_OffsetToStringData";
		}
		#endregion
	}
}
