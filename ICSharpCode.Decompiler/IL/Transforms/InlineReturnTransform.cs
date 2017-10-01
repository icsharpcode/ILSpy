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
	/// This transform duplicates return blocks if they return a local variable that was assigned right before thie return.
	/// </summary>
	class InlineReturnTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			var instructionsToModify = new List<(BlockContainer, Block, Branch)>();
			var possibleReturnVars = new Queue<(ILVariable, Block)>();
			var tempList = new List<(BlockContainer, Block, Branch)>();

			foreach (var leave in function.Descendants.OfType<Leave>()) {
				if (!(leave.Parent is Block b && b.Instructions.Count == 1))
					continue;
				if (!leave.Value.MatchLdLoc(out var returnVar) || returnVar.Kind != VariableKind.Local)
					continue;
				possibleReturnVars.Enqueue((returnVar, b));
			}

			while (possibleReturnVars.Count > 0) {
				var (returnVar, leaveBlock) = possibleReturnVars.Dequeue();
				bool transform = true;
				foreach (StLoc store in returnVar.StoreInstructions.OfType<StLoc>()) {
					if (!(store.Parent is Block storeBlock)) {
						transform = false;
						break;
					} 
					if (store.ChildIndex + 2 != storeBlock.Instructions.Count) {
						transform = false;
						break;
					}
					if (!(storeBlock.Instructions[store.ChildIndex + 1] is Branch br)) {
						transform = false;
						break;
					}
					if (br.TargetBlock != leaveBlock) {
						transform = false;
						break;
					}
					var targetBlockContainer = BlockContainer.FindClosestContainer(store);
					if (targetBlockContainer == null) {
						transform = false;
						break;
					}
					tempList.Add((targetBlockContainer, leaveBlock, br));
				}
				if (transform)
					instructionsToModify.AddRange(tempList);
				tempList.Clear();
			}

			foreach (var (container, block, br) in instructionsToModify) {
				var newBlock = (Block)block.Clone();
				container.Blocks.Add(newBlock);
				br.TargetBlock = newBlock;
			}
		}
	}
}
