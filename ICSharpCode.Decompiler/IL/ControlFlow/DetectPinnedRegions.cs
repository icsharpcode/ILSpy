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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

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
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			foreach (var container in function.Descendants.OfType<BlockContainer>())
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				DetectNullSafeArrayToPointerOrCustomRefPin(container);
				SplitBlocksAtWritesToPinnedLocals(container);
				foreach (var block in container.Blocks)
					DetectPinnedRegion(block);
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
			}
			// Sometimes there's leftover writes to the original pinned locals
			foreach (var block in function.Descendants.OfType<Block>())
			{
				context.CancellationToken.ThrowIfCancellationRequested();
				for (int i = 0; i < block.Instructions.Count; i++)
				{
					var stloc = block.Instructions[i] as StLoc;
					if (stloc != null && stloc.Variable.Kind == VariableKind.PinnedLocal && stloc.Variable.LoadCount == 0 && stloc.Variable.AddressCount == 0)
					{
						if (SemanticHelper.IsPure(stloc.Value.Flags))
						{
							block.Instructions.RemoveAt(i--);
						}
						else
						{
							stloc.ReplaceWith(stloc.Value);
						}
					}
				}
			}
			this.context = null;
		}

		/// <summary>
		/// Ensures that every write to a pinned local is followed by a branch instruction.
		/// This ensures the 'pinning region' does not involve any half blocks, which makes it easier to extract.
		/// </summary>
		void SplitBlocksAtWritesToPinnedLocals(BlockContainer container)
		{
			for (int i = 0; i < container.Blocks.Count; i++)
			{
				var block = container.Blocks[i];
				for (int j = 0; j < block.Instructions.Count - 1; j++)
				{
					var inst = block.Instructions[j];
					if (inst.MatchStLoc(out ILVariable v, out var value) && v.Kind == VariableKind.PinnedLocal)
					{
						if (block.Instructions[j + 1].OpCode != OpCode.Branch)
						{
							// split block after j:
							context.Step("Split block after pinned local write", inst);
							var newBlock = new Block();
							for (int k = j + 1; k < block.Instructions.Count; k++)
							{
								newBlock.Instructions.Add(block.Instructions[k]);
							}
							newBlock.AddILRange(newBlock.Instructions[0]);
							block.Instructions.RemoveRange(j + 1, newBlock.Instructions.Count);
							block.Instructions.Add(new Branch(newBlock));
							container.Blocks.Insert(i + 1, newBlock);
						}
						// in case of re-pinning (e.g. C++/CLI assignment to pin_ptr variable),
						// it's possible for the new value to be dependent on the old.
						if (v.IsUsedWithin(value))
						{
							// In this case, we need to un-inline the uses of the pinned local
							// so that they are split off into the block prior to the pinned local write
							var temp = context.Function.RegisterVariable(VariableKind.StackSlot, v.Type);
							block.Instructions.Insert(j++, new StLoc(temp, new LdLoc(v)));
							foreach (var descendant in value.Descendants)
							{
								if (descendant.MatchLdLoc(v))
								{
									descendant.ReplaceWith(new LdLoc(temp).WithILRange(descendant));
								}
							}
						}
						if (j > 0)
						{
							// split block before j:
							context.Step("Split block before pinned local write", inst);
							var newBlock = new Block();
							newBlock.Instructions.Add(block.Instructions[j]);
							newBlock.Instructions.Add(block.Instructions[j + 1]);
							newBlock.AddILRange(newBlock.Instructions[0]);
							Debug.Assert(block.Instructions.Count == j + 2);
							block.Instructions.RemoveRange(j, 2);
							block.Instructions.Insert(j, new Branch(newBlock));
							container.Blocks.Insert(i + 1, newBlock);
						}
					}
				}
			}
		}

		#region null-safe array to pointer
		void DetectNullSafeArrayToPointerOrCustomRefPin(BlockContainer container)
		{
			bool modified = false;
			for (int i = 0; i < container.Blocks.Count; i++)
			{
				var block = container.Blocks[i];
				if (IsNullSafeArrayToPointerPattern(block, out ILVariable v, out ILVariable p, out Block targetBlock))
				{
					context.Step("NullSafeArrayToPointerPattern", block);
					ILInstruction arrayToPointer = new GetPinnableReference(new LdLoc(v), null);
					if (p.StackType != StackType.Ref)
					{
						arrayToPointer = new Conv(arrayToPointer, p.StackType.ToPrimitiveType(), false, Sign.None);
					}
					block.Instructions[block.Instructions.Count - 2] = new StLoc(p, arrayToPointer)
						.WithILRange(block.Instructions[block.Instructions.Count - 2]);
					((Branch)block.Instructions.Last()).TargetBlock = targetBlock;
					modified = true;
				}
				else if (IsCustomRefPinPattern(block, out ILInstruction ldlocMem, out var callGPR, out v, out var stlocPtr, out targetBlock))
				{
					context.Step("CustomRefPinPattern", block);
					ILInstruction gpr;
					if (context.Settings.PatternBasedFixedStatement)
					{
						gpr = new GetPinnableReference(ldlocMem, callGPR.Method);
					}
					else
					{
						gpr = new IfInstruction(
							condition: new Comp(ComparisonKind.Inequality, Sign.None, ldlocMem, new LdNull()),
							trueInst: callGPR,
							falseInst: new Conv(new LdcI4(0), PrimitiveType.Ref, checkForOverflow: false, inputSign: Sign.None)
						);
					}
					block.Instructions[block.Instructions.Count - 2] = new StLoc(v, gpr)
						.WithILRange(block.Instructions[block.Instructions.Count - 2]);
					block.Instructions.Insert(block.Instructions.Count - 1, stlocPtr);
					((Branch)block.Instructions.Last()).TargetBlock = targetBlock;
					modified = true;
				}
			}
			if (modified)
			{
				container.Blocks.RemoveAll(b => b.IncomingEdgeCount == 0); // remove blocks made unreachable
			}
		}

		// Detect the following pattern:
		//      if (comp.o(ldloc mem != ldnull)) br notNullBlock
		//      br nullBlock
		//  }
		//
		//  Block nullBlock (incoming: 1) {
		//      stloc ptr(conv i4->u <zero extend>(ldc.i4 0))
		//      br targetBlock
		//  }
		//
		//  Block notNullBlock (incoming: 1) {
		//      stloc V_1(call GetPinnableReference(ldloc mem))
		//      stloc ptr(conv ref->u (ldloc V_1))
		//      br targetBlock
		//  }
		// It will be replaced with:
		//      stloc V_1(get.pinnable.reference(ldloc mem))
		//      stloc ptr(conv ref->u (ldloc V_1))
		//      br targetBlock
		private bool IsCustomRefPinPattern(Block block, out ILInstruction ldlocMem, out CallInstruction callGPR,
			out ILVariable v, out StLoc ptrAssign, out Block targetBlock)
		{
			ldlocMem = null;
			callGPR = null;
			v = null;
			ptrAssign = null;
			targetBlock = null;
			//      if (comp.o(ldloc mem != ldnull)) br on_not_null
			//      br on_null
			if (!block.MatchIfAtEndOfBlock(out var ifCondition, out var trueInst, out var falseInst))
				return false;
			if (!ifCondition.MatchCompNotEqualsNull(out ldlocMem))
				return false;
			if (!SemanticHelper.IsPure(ldlocMem.Flags))
				return false;
			if (!trueInst.MatchBranch(out Block notNullBlock) || notNullBlock.Parent != block.Parent)
				return false;
			if (!falseInst.MatchBranch(out Block nullBlock) || nullBlock.Parent != block.Parent)
				return false;

			//  Block nullBlock (incoming: 1) {
			//      stloc ptr(conv i4->u <zero extend>(ldc.i4 0))
			//      br targetBlock
			//  }
			if (nullBlock.IncomingEdgeCount != 1)
				return false;
			if (nullBlock.Instructions.Count != 2)
				return false;
			if (!nullBlock.Instructions[0].MatchStLoc(out var ptr, out var nullPointerInst))
				return false;
			if (!nullPointerInst.MatchLdcI(0))
				return false;
			if (!nullBlock.Instructions[1].MatchBranch(out targetBlock))
				return false;

			//  Block notNullBlock (incoming: 1) {
			//      stloc V_1(call GetPinnableReference(ldloc mem))
			//      stloc ptr(conv ref->u (ldloc V_1))
			//      br targetBlock
			//  }
			if (notNullBlock.IncomingEdgeCount != 1)
				return false;
			if (notNullBlock.Instructions.Count != 3)
				return false;
			// stloc V_1(call GetPinnableReference(ldloc mem))
			if (!notNullBlock.Instructions[0].MatchStLoc(out v, out var value))
				return false;
			if (v.Kind != VariableKind.PinnedLocal)
				return false;
			callGPR = value as CallInstruction;
			if (callGPR == null || callGPR.Arguments.Count != 1)
				return false;
			if (callGPR.Method.Name != "GetPinnableReference")
				return false;
			if (!ldlocMem.Match(callGPR.Arguments[0]).Success)
				return false;
			// stloc ptr(conv ref->u (ldloc V_1))
			ptrAssign = notNullBlock.Instructions[1] as StLoc;
			if (ptrAssign == null || ptrAssign.Variable != ptr)
				return false;
			if (!ptrAssign.Value.UnwrapConv(ConversionKind.StopGCTracking).MatchLdLoc(v))
				return false;
			// br targetBlock
			if (!notNullBlock.Instructions[2].MatchBranch(targetBlock))
				return false;
			return true;
		}

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
			bool usingPreviousVar = false;
			if (v.Kind == VariableKind.StackSlot)
			{
				// If the variable is a stack slot, that might be due to an inline assignment,
				// so check the previous instruction:
				var previous = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;
				if (previous != null && previous.Value.MatchLdLoc(v))
				{
					// stloc V(ldloc S)
					// if (comp(ldloc S == ldnull)) ...
					v = previous.Variable;
					usingPreviousVar = true;
				}
			}
			if (!ifInst.TrueInst.MatchBranch(out Block nullOrEmptyBlock))
				return false;
			if (!ifInst.FalseInst.MatchNop())
				return false;
			if (nullOrEmptyBlock.Parent != block.Parent)
				return false;
			if (!IsNullSafeArrayToPointerNullOrEmptyBlock(nullOrEmptyBlock, out p, out targetBlock))
				return false;
			if (!(p.Kind == VariableKind.PinnedLocal || (usingPreviousVar && v.Kind == VariableKind.PinnedLocal)))
				return false;
			if (!block.Instructions.Last().MatchBranch(out Block notNullBlock))
				return false;
			if (notNullBlock.Parent != block.Parent)
				return false;
			return IsNullSafeArrayToPointerNotNullBlock(notNullBlock, v, p, nullOrEmptyBlock, targetBlock);
		}

		bool IsNullSafeArrayToPointerNotNullBlock(Block block, ILVariable v, ILVariable p, Block nullOrEmptyBlock, Block targetBlock)
		{
			// Block B_not_null {
			//   if (conv i->i4 (ldlen(ldloc V))) br B_not_null_and_not_empty
			//   br B_null_or_empty
			// }
			if (block.Instructions.Count != 2)
				return false;
			if (!block.Instructions[0].MatchIfInstruction(out ILInstruction condition, out ILInstruction trueInst))
				return false;
			var falseInst = block.Instructions[1];
			if (condition is Comp comp && comp.Right.MatchLdcI(0))
			{
				if (comp.Kind == ComparisonKind.Equality)
				{
					// if (len == 0): effectively negates the condition
					condition = comp.Left;
					ExtensionMethods.Swap(ref trueInst, ref falseInst);
				}
				else if (comp.Kind == ComparisonKind.Inequality)
				{
					// if (len != 0): comparison is redundant (equivalent to implicit non-zero check)
					condition = comp.Left;
				}
				else
				{
					return false;
				}
			}
			condition = condition.UnwrapConv(ConversionKind.Truncate);
			if (condition.MatchLdLen(StackType.I, out ILInstruction array))
			{
				// OK
			}
			else if (condition is CallInstruction call && call.Method.Name == "get_Length")
			{
				// Used instead of ldlen for multi-dimensional arrays
				if (!call.Method.DeclaringType.IsKnownType(KnownTypeCode.Array))
					return false;
				if (call.Arguments.Count != 1)
					return false;
				array = call.Arguments[0];
			}
			else
			{
				return false;
			}
			if (!array.MatchLdLoc(v))
				return false;
			if (!trueInst.MatchBranch(out Block notNullAndNotEmptyBlock))
				return false;
			if (notNullAndNotEmptyBlock.Parent != block.Parent)
				return false;
			if (!IsNullSafeArrayToPointerNotNullAndNotEmptyBlock(notNullAndNotEmptyBlock, v, p, targetBlock))
				return false;
			return falseInst.MatchBranch(nullOrEmptyBlock);
		}

		bool IsNullSafeArrayToPointerNotNullAndNotEmptyBlock(Block block, ILVariable v, ILVariable p, Block targetBlock)
		{
			// Block B_not_null_and_not_empty {
			//   stloc P(ldelema(ldloc V, ldc.i4 0, ...))
			//   br B_target
			// }
			if (block.Instructions.Count != 2)
				return false;
			if (!block.Instructions[0].MatchStLoc(out var p2, out ILInstruction value))
				return false;
			if (p != p2)
			{
				// If the pointer is unused, the variable P might have been split.
				if (p.LoadCount == 0 && p.AddressCount == 0 && p2.LoadCount == 0 && p2.AddressCount == 0)
				{
					if (!ILVariableEqualityComparer.Instance.Equals(p, p2))
						return false;
				}
				else
				{
					return false;
				}
			}
			if (v.Kind == VariableKind.PinnedLocal)
			{
				value = value.UnwrapConv(ConversionKind.StopGCTracking);
			}
			if (!(value is LdElema ldelema))
				return false;
			if (!ldelema.Array.MatchLdLoc(v))
				return false;
			if (!ldelema.Indices.All(i => i.MatchLdcI4(0)))
				return false;
			return block.Instructions[1].MatchBranch(targetBlock);
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
				&& (p.Kind == VariableKind.PinnedLocal || p.Kind == VariableKind.Local)
				&& IsNullOrZero(value)
				&& block.Instructions[1].MatchBranch(out targetBlock);
		}
		#endregion

		#region CreatePinnedRegion
		bool DetectPinnedRegion(Block block)
		{
			// After SplitBlocksAtWritesToPinnedLocals(), only the second-to-last instruction in each block
			// can be a write to a pinned local.
			var stLoc = block.Instructions.SecondToLastOrDefault() as StLoc;
			if (stLoc == null || stLoc.Variable.Kind != VariableKind.PinnedLocal)
				return false;
			// stLoc is a store to a pinned local.
			if (IsNullOrZero(stLoc.Value))
			{
				return false; // ignore unpin instructions
			}
			if (stLoc.Variable.Type.IsReferenceType == false)
			{
				// `pinned` flag has no effect on value types (#2148)
				return false;
			}
			// stLoc is a store that starts a new pinned region

			context.StepStartGroup($"DetectPinnedRegion {stLoc.Variable.Name}", block);
			try
			{
				return CreatePinnedRegion(block, stLoc);
			}
			finally
			{
				context.StepEndGroup(keepIfEmpty: true);
			}
		}

		bool CreatePinnedRegion(Block block, StLoc stLoc)
		{
			// Collect the blocks to be moved into the region:
			BlockContainer sourceContainer = (BlockContainer)block.Parent;
			int[] reachedEdgesPerBlock = new int[sourceContainer.Blocks.Count];
			Queue<Block> workList = new Queue<Block>();
			Block entryBlock = ((Branch)block.Instructions.Last()).TargetBlock;
			if (entryBlock.Parent != sourceContainer)
			{
				// we didn't find a single block to be added to the pinned region
				return false;
			}
			if (entryBlock.Instructions[0].MatchStLoc(stLoc.Variable, out _))
			{
				// pinned region has empty body
			}
			else
			{
				reachedEdgesPerBlock[entryBlock.ChildIndex]++;
				workList.Enqueue(entryBlock);
			}
			while (workList.Count > 0)
			{
				Block workItem = workList.Dequeue();
				foreach (var branch in workItem.Descendants.OfType<Branch>())
				{
					if (branch.TargetBlock.Parent == sourceContainer)
					{
						if (branch.TargetBlock.Instructions[0].MatchStLoc(stLoc.Variable, out _))
						{
							// Found unpin instruction
							continue;
						}
						Debug.Assert(branch.TargetBlock != block);
						if (reachedEdgesPerBlock[branch.TargetBlock.ChildIndex]++ == 0)
						{
							// detected first edge to that block: add block as work item
							workList.Enqueue(branch.TargetBlock);
						}
					}
				}
			}

			// Validate that all uses of a block consistently are inside or outside the pinned region.
			// (we cannot do this anymore after we start moving blocks around)
			bool cloneBlocks = false;
			for (int i = 0; i < sourceContainer.Blocks.Count; i++)
			{
				if (reachedEdgesPerBlock[i] != 0 && reachedEdgesPerBlock[i] != sourceContainer.Blocks[i].IncomingEdgeCount)
				{
					// Don't abort in this case, we still need to somehow represent the pinned variable with a fixed statement.
					// We'll duplicate the code so that it can be both inside and outside the pinned region.
					cloneBlocks = true;
					break;
				}
			}

			context.Step("CreatePinnedRegion", block);
			BlockContainer body = new BlockContainer();
			Block[] clonedBlocks = cloneBlocks ? new Block[sourceContainer.Blocks.Count] : null;
			for (int i = 0; i < sourceContainer.Blocks.Count; i++)
			{
				if (reachedEdgesPerBlock[i] > 0)
				{
					var innerBlock = sourceContainer.Blocks[i];
					if (cloneBlocks)
					{
						innerBlock = (Block)innerBlock.Clone();
						clonedBlocks[i] = innerBlock;
					}
					Branch br = innerBlock.Instructions.LastOrDefault() as Branch;
					if (br != null && br.TargetBlock.IncomingEdgeCount == 1
						&& br.TargetContainer == sourceContainer && reachedEdgesPerBlock[br.TargetBlock.ChildIndex] == 0)
					{
						// branch that leaves body.
						// The target block should have an instruction that resets the pin; delete that instruction:
						StLoc unpin = br.TargetBlock.Instructions.First() as StLoc;
						if (unpin != null && unpin.Variable == stLoc.Variable && IsNullOrZero(unpin.Value))
						{
							br.TargetBlock.Instructions.RemoveAt(0);
						}
					}
					// move block into body
					if (sourceContainer.Blocks[i] == entryBlock)
					{
						// ensure entry point comes first
						body.Blocks.Insert(0, innerBlock);
					}
					else
					{
						body.Blocks.Add(innerBlock);
					}
					if (!cloneBlocks)
					{
						sourceContainer.Blocks[i] = new Block(); // replace with dummy block
																 // we'll delete the dummy block later
					}
				}
			}
			if (body.Blocks.Count == 0)
			{
				// empty body, the entryBlock itself doesn't belong into the pinned region
				Debug.Assert(reachedEdgesPerBlock[entryBlock.ChildIndex] == 0);
				var bodyBlock = new Block();
				bodyBlock.SetILRange(stLoc);
				bodyBlock.Instructions.Add(new Branch(entryBlock));
				body.Blocks.Add(bodyBlock);
			}

			var pinnedRegion = new PinnedRegion(stLoc.Variable, stLoc.Value, body).WithILRange(stLoc);
			stLoc.ReplaceWith(pinnedRegion);
			block.Instructions.RemoveAt(block.Instructions.Count - 1); // remove branch into body

			if (cloneBlocks)
			{
				// Adjust branches between cloned blocks.
				foreach (var branch in body.Descendants.OfType<Branch>())
				{
					if (branch.TargetContainer == sourceContainer)
					{
						int i = branch.TargetBlock.ChildIndex;
						if (clonedBlocks[i] != null)
						{
							branch.TargetBlock = clonedBlocks[i];
						}
					}
				}
				// Replace unreachable blocks in sourceContainer with dummy blocks:
				bool[] isAlive = new bool[sourceContainer.Blocks.Count];
				List<int> duplicatedBlockStartOffsets = new List<int>();
				foreach (var remainingBlock in sourceContainer.TopologicalSort(deleteUnreachableBlocks: true))
				{
					isAlive[remainingBlock.ChildIndex] = true;
					if (clonedBlocks[remainingBlock.ChildIndex] != null)
					{
						duplicatedBlockStartOffsets.Add(remainingBlock.StartILOffset);
					}
				}
				for (int i = 0; i < isAlive.Length; i++)
				{
					if (!isAlive[i])
						sourceContainer.Blocks[i] = new Block();
				}
				// we'll delete the dummy blocks later
				Debug.Assert(duplicatedBlockStartOffsets.Count > 0);
				duplicatedBlockStartOffsets.Sort();
				context.Function.Warnings.Add("The blocks "
					+ string.Join(", ", duplicatedBlockStartOffsets.Select(o => $"IL_{o:x4}"))
					+ $" are reachable both inside and outside the pinned region starting at IL_{stLoc.StartILOffset:x4}."
					+ " ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.");
			}

			ProcessPinnedRegion(pinnedRegion);
			return true;
		}

		static bool IsNullOrZero(ILInstruction inst)
		{
			while (inst is Conv conv)
			{
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
			if (pinnedRegion.Variable.Type.Kind == TypeKind.ByReference)
			{
				// C# doesn't support a "by reference" variable, so replace it with a native pointer
				context.Step("Replace pinned ref-local with native pointer", pinnedRegion);
				ILVariable oldVar = pinnedRegion.Variable;
				IType elementType = ((ByReferenceType)oldVar.Type).ElementType;
				if (elementType.Kind == TypeKind.Pointer && pinnedRegion.Init.MatchLdFlda(out _, out var field)
					&& ((PointerType)elementType).ElementType.Equals(field.Type))
				{
					// Roslyn 2.6 (C# 7.2) uses type "int*&" for the pinned local referring to a
					// fixed field of type "int".
					// Remove the extra level of indirection.
					elementType = ((PointerType)elementType).ElementType;
				}
				ILVariable newVar = new ILVariable(
					VariableKind.PinnedRegionLocal,
					new PointerType(elementType),
					oldVar.Index);
				newVar.Name = oldVar.Name;
				newVar.HasGeneratedName = oldVar.HasGeneratedName;
				oldVar.Function.Variables.Add(newVar);
				ReplacePinnedVar(oldVar, newVar, pinnedRegion);
				UseExistingVariableForPinnedRegion(pinnedRegion);
			}
			else if (pinnedRegion.Variable.Type.Kind == TypeKind.Array)
			{
				context.Step("Replace pinned array with native pointer", pinnedRegion);
				MoveArrayToPointerToPinnedRegionInit(pinnedRegion);
				UseExistingVariableForPinnedRegion(pinnedRegion);
			}
			else if (pinnedRegion.Variable.Type.IsKnownType(KnownTypeCode.String))
			{
				// fixing a string
				HandleStringToPointer(pinnedRegion);
			}
			// Detect nested pinned regions:
			BlockContainer body = (BlockContainer)pinnedRegion.Body;
			foreach (var block in body.Blocks)
				DetectPinnedRegion(block);
			body.Blocks.RemoveAll(b => b.Instructions.Count == 0); // remove dummy blocks
			body.SetILRange(body.EntryPoint);
			if (pinnedRegion.Variable.Kind != VariableKind.PinnedRegionLocal)
			{
				Debug.Assert(pinnedRegion.Variable.Kind == VariableKind.PinnedLocal);
				pinnedRegion.Variable.Kind = VariableKind.PinnedRegionLocal;
			}
		}

		private void MoveArrayToPointerToPinnedRegionInit(PinnedRegion pinnedRegion)
		{
			// Roslyn started marking the array variable as pinned,
			// and then uses array.to.pointer immediately within the region.
			Debug.Assert(pinnedRegion.Variable.Type.Kind == TypeKind.Array);
			// Find the single load of the variable within the pinnedRegion:
			LdLoc ldloc = null;
			foreach (var inst in pinnedRegion.Descendants.OfType<IInstructionWithVariableOperand>())
			{
				if (inst.Variable == pinnedRegion.Variable && inst != pinnedRegion)
				{
					if (ldloc != null)
						return; // more than 1 variable access
					ldloc = inst as LdLoc;
					if (ldloc == null)
						return; // variable access that is not LdLoc
				}
			}
			if (ldloc == null)
				return;
			if (!(ldloc.Parent is GetPinnableReference arrayToPointer))
				return;
			if (!(arrayToPointer.Parent is Conv conv && conv.Kind == ConversionKind.StopGCTracking))
				return;
			Debug.Assert(arrayToPointer.IsDescendantOf(pinnedRegion));
			ILVariable oldVar = pinnedRegion.Variable;
			ILVariable newVar = new ILVariable(
				VariableKind.PinnedRegionLocal,
				new PointerType(((ArrayType)oldVar.Type).ElementType),
				oldVar.Index);
			newVar.Name = oldVar.Name;
			newVar.HasGeneratedName = oldVar.HasGeneratedName;
			oldVar.Function.Variables.Add(newVar);
			pinnedRegion.Variable = newVar;
			pinnedRegion.Init = new GetPinnableReference(pinnedRegion.Init, arrayToPointer.Method).WithILRange(arrayToPointer);
			conv.ReplaceWith(new LdLoc(newVar).WithILRange(conv));
		}

		void ReplacePinnedVar(ILVariable oldVar, ILVariable newVar, ILInstruction inst)
		{
			Debug.Assert(newVar.StackType == StackType.I);
			if (inst is Conv conv && conv.Kind == ConversionKind.StopGCTracking && conv.Argument.MatchLdLoc(oldVar) && conv.ResultType == newVar.StackType)
			{
				// conv ref->i (ldloc oldVar)
				//  => ldloc newVar
				conv.AddILRange(conv.Argument);
				conv.ReplaceWith(new LdLoc(newVar).WithILRange(conv));
				return;
			}
			if (inst is IInstructionWithVariableOperand iwvo && iwvo.Variable == oldVar)
			{
				iwvo.Variable = newVar;
				if (inst is StLoc stloc && oldVar.Type.Kind == TypeKind.ByReference)
				{
					stloc.Value = new Conv(stloc.Value, PrimitiveType.I, false, Sign.None);
				}
				if ((inst is LdLoc || inst is StLoc) && !IsSlotAcceptingBothManagedAndUnmanagedPointers(inst.SlotInfo) && oldVar.StackType != StackType.I)
				{
					// wrap inst in Conv, so that the stack types match up
					var children = inst.Parent.Children;
					children[inst.ChildIndex] = new Conv(inst, oldVar.StackType.ToPrimitiveType(), false, Sign.None);
				}
			}
			else if (inst.MatchLdStr(out var val) && val == "Is this ILSpy?")
			{
				inst.ReplaceWith(new LdStr("This is ILSpy!")); // easter egg ;)
				return;
			}
			foreach (var child in inst.Children)
			{
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

		void HandleStringToPointer(PinnedRegion pinnedRegion)
		{
			Debug.Assert(pinnedRegion.Variable.Type.IsKnownType(KnownTypeCode.String));
			BlockContainer body = (BlockContainer)pinnedRegion.Body;
			if (body.EntryPoint.IncomingEdgeCount != 1)
				return;
			// stloc nativeVar(conv o->i (ldloc pinnedVar))
			// if (comp(ldloc nativeVar == conv i4->i <sign extend>(ldc.i4 0))) br targetBlock
			// br adjustOffsetToStringData
			ILVariable newVar;
			if (!body.EntryPoint.Instructions[0].MatchStLoc(out ILVariable nativeVar, out ILInstruction initInst))
			{
				// potentially a special case with legacy csc and an unused pinned variable:
				if (pinnedRegion.Variable.AddressCount == 0 && pinnedRegion.Variable.LoadCount == 0)
				{
					var charPtr = new PointerType(context.TypeSystem.FindType(KnownTypeCode.Char));
					newVar = new ILVariable(VariableKind.PinnedRegionLocal, charPtr, pinnedRegion.Variable.Index);
					newVar.Name = pinnedRegion.Variable.Name;
					newVar.HasGeneratedName = pinnedRegion.Variable.HasGeneratedName;
					pinnedRegion.Variable.Function.Variables.Add(newVar);
					pinnedRegion.Variable = newVar;
					pinnedRegion.Init = new GetPinnableReference(pinnedRegion.Init, null);
				}
				return;
			}
			if (body.EntryPoint.Instructions.Count != 3)
			{
				return;
			}

			if (nativeVar.Type.GetStackType() != StackType.I)
				return;
			if (!initInst.UnwrapConv(ConversionKind.StopGCTracking).MatchLdLoc(pinnedRegion.Variable))
				return;
			if (!IsBranchOnNull(body.EntryPoint.Instructions[1], nativeVar, out Block targetBlock))
				return;
			if (!body.EntryPoint.Instructions[2].MatchBranch(out Block adjustOffsetToStringData))
				return;
			if (!(adjustOffsetToStringData.Parent == body && adjustOffsetToStringData.IncomingEdgeCount == 1
					&& IsOffsetToStringDataBlock(adjustOffsetToStringData, nativeVar, targetBlock)))
				return;
			context.Step("Handle pinned string (with adjustOffsetToStringData)", pinnedRegion);
			if (targetBlock.Parent == body)
			{
				// remove old entry point
				body.Blocks.RemoveAt(0);
				body.Blocks.RemoveAt(adjustOffsetToStringData.ChildIndex);
				// make targetBlock the new entry point
				body.Blocks.RemoveAt(targetBlock.ChildIndex);
				body.Blocks.Insert(0, targetBlock);
			}
			else
			{
				// pinned region has empty body, immediately jumps to targetBlock which is outside
				body.Blocks[0].Instructions.Clear();
				body.Blocks.RemoveRange(1, body.Blocks.Count - 1);
				body.Blocks[0].Instructions.Add(new Branch(targetBlock));
			}
			pinnedRegion.Init = new GetPinnableReference(pinnedRegion.Init, null);

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
			if (nativeVar.Kind == VariableKind.Local)
			{
				newVar = new ILVariable(VariableKind.PinnedRegionLocal, nativeVar.Type, nativeVar.Index);
				newVar.Name = nativeVar.Name;
				newVar.HasGeneratedName = nativeVar.HasGeneratedName;
				nativeVar.Function.Variables.Add(newVar);
				ReplacePinnedVar(nativeVar, newVar, pinnedRegion);
			}
			else
			{
				newVar = nativeVar;
			}
			ReplacePinnedVar(pinnedRegion.Variable, newVar, pinnedRegion);
		}

		bool IsOffsetToStringDataBlock(Block block, ILVariable nativeVar, Block targetBlock)
		{
			// stloc nativeVar(add(ldloc nativeVar, conv i4->i <sign extend>(call [Accessor System.Runtime.CompilerServices.RuntimeHelpers.get_OffsetToStringData():System.Int32]())))
			// br IL_0011
			if (block.Instructions.Count != 2)
				return false;
			ILInstruction value;
			if (nativeVar.IsSingleDefinition && nativeVar.LoadCount == 2)
			{
				// If there are no loads (except for the two in the string-to-pointer pattern),
				// then we might have split nativeVar:
				if (!block.Instructions[0].MatchStLoc(out var otherVar, out value))
					return false;
				if (!(otherVar.IsSingleDefinition && otherVar.LoadCount == 0))
					return false;
			}
			else if (nativeVar.StoreCount == 2)
			{
				// normal case with non-split variable
				if (!block.Instructions[0].MatchStLoc(nativeVar, out value))
					return false;
			}
			else
			{
				return false;
			}
			if (!value.MatchBinaryNumericInstruction(BinaryNumericOperator.Add, out ILInstruction left, out ILInstruction right))
				return false;
			if (!left.MatchLdLoc(nativeVar))
				return false;
			if (!IsOffsetToStringDataCall(right))
				return false;
			return block.Instructions[1].MatchBranch(targetBlock);
		}

		bool IsOffsetToStringDataCall(ILInstruction inst)
		{
			Call call = inst.UnwrapConv(ConversionKind.SignExtend) as Call;
			return call != null && call.Method.FullName == "System.Runtime.CompilerServices.RuntimeHelpers.get_OffsetToStringData";
		}

		/// <summary>
		/// Modifies a pinned region to eliminate an extra local variable that roslyn tends to generate.
		/// </summary>
		void UseExistingVariableForPinnedRegion(PinnedRegion pinnedRegion)
		{
			// PinnedRegion V_1(..., BlockContainer {
			// Block IL_0000(incoming: 1) {
			//    stloc V_0(ldloc V_1)
			//    ...
			if (!(pinnedRegion.Body is BlockContainer body))
				return;
			if (pinnedRegion.Variable.LoadCount != 1)
				return;
			if (!body.EntryPoint.Instructions[0].MatchStLoc(out var v, out var init))
				return;
			if (!init.MatchLdLoc(pinnedRegion.Variable))
				return;
			if (!(v.IsSingleDefinition && v.Type.Equals(pinnedRegion.Variable.Type)))
				return;
			if (v.Kind != VariableKind.Local)
				return;
			// replace V_1 with V_0
			v.Kind = VariableKind.PinnedRegionLocal;
			pinnedRegion.Variable = v;
			body.EntryPoint.Instructions.RemoveAt(0);
		}
		#endregion
	}
}
