// Copyright (c) 2017 Siegfried Pammer
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
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// This transform duplicates return blocks if they return a local variable that was assigned right before the return.
	/// </summary>
	class InlineReturnTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			var instructionsToModify = new List<(BlockContainer, Block, Branch)>();

			// Process all leave instructions in a leave-block, that is a block consisting solely of a leave instruction.
			foreach (var leave in function.Descendants.OfType<Leave>())
			{
				if (!(leave.Parent is Block leaveBlock && leaveBlock.Instructions.Count == 1))
					continue;
				// Skip, if the leave instruction has no value or the value is not a load of a local variable.
				if (!leave.Value.MatchLdLoc(out var returnVar) || returnVar.Kind != VariableKind.Local)
					continue;
				// If all instructions can be modified, add item to the global list.
				if (CanModifyInstructions(returnVar, leaveBlock, out var list))
					instructionsToModify.AddRange(list);
			}

			foreach (var (container, b, br) in instructionsToModify)
			{
				Block block = b;
				// if there is only one branch to this return block, move it to the matching container.
				// otherwise duplicate the return block.
				if (block.IncomingEdgeCount == 1)
				{
					block.Remove();
				}
				else
				{
					block = (Block)block.Clone();
				}
				container.Blocks.Add(block);
				// adjust the target of the branch to the newly created block.
				br.TargetBlock = block;
			}
		}

		/// <summary>
		/// Determines a list of all store instructions that write to a given <paramref name="returnVar"/>.
		/// Returns false if any of these instructions does not meet the following criteria:
		/// - must be a stloc
		/// - must be a direct child of a block
		/// - must be the penultimate instruction
		/// - must be followed by a branch instruction to <paramref name="leaveBlock"/>
		/// - must have a BlockContainer as ancestor.
		/// Returns true, if all instructions meet these criteria, and <paramref name="instructionsToModify"/> contains a list of 3-tuples.
		/// Each tuple consists of the target block container, the leave block, and the branch instruction that should be modified.
		/// </summary>
		static bool CanModifyInstructions(ILVariable returnVar, Block leaveBlock, out List<(BlockContainer, Block, Branch)> instructionsToModify)
		{
			instructionsToModify = new List<(BlockContainer, Block, Branch)>();
			foreach (var inst in returnVar.StoreInstructions)
			{
				if (!(inst is StLoc store))
					return false;
				if (!(store.Parent is Block storeBlock))
					return false;
				if (store.ChildIndex + 2 != storeBlock.Instructions.Count)
					return false;
				if (!(storeBlock.Instructions[store.ChildIndex + 1] is Branch br))
					return false;
				if (br.TargetBlock != leaveBlock)
					return false;
				var targetBlockContainer = BlockContainer.FindClosestContainer(store);
				if (targetBlockContainer == null)
					return false;
				instructionsToModify.Add((targetBlockContainer, leaveBlock, br));
			}

			return true;
		}
	}
}
