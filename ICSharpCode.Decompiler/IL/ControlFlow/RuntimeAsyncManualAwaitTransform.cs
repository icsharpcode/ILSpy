// Copyright (c) 2026 Siegfried Pammer
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

using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Collapses the runtime-async manual await pattern (GetAwaiter / get_IsCompleted /
	/// AsyncHelpers.UnsafeAwaitAwaiter / GetResult across three blocks) into a single IL
	/// Await instruction, matching what the state-machine async pipeline emits.
	/// </summary>
	static class RuntimeAsyncManualAwaitTransform
	{
		public static void Run(ILFunction function, ILTransformContext context)
		{
			bool changed = false;
			foreach (var block in function.Descendants.OfType<Block>().ToArray())
			{
				if (DetectRuntimeAsyncManualAwait(block, context))
					changed = true;
			}
			if (changed)
			{
				foreach (var c in function.Body.Descendants.OfType<BlockContainer>())
					c.SortBlocks(deleteUnreachableBlocks: true);
			}
		}

		// Block X (head) {
		//     [stloc awaitable(<value>);]                       (optional: when GetAwaiter takes an address)
		//     stloc awaiter(call GetAwaiter(<expr>))
		//     if (call get_IsCompleted(ldloca awaiter)) br completedBlock
		//     br pauseBlock
		// }
		// Block pauseBlock {
		//     call AsyncHelpers.UnsafeAwaitAwaiter[<T>](ldloc awaiter)
		//     br completedBlock
		// }
		// Block completedBlock {
		//     call GetResult(ldloca awaiter)                     (possibly inlined into a containing expression)
		//     ...
		// }
		// =>
		// Block X { ...; br completedBlock }
		// Block completedBlock { <Await(value)>; ... }
		static bool DetectRuntimeAsyncManualAwait(Block block, ILTransformContext context)
		{
			if (block.Instructions.Count < 3)
				return false;

			if (!block.Instructions[^3].MatchStLoc(out var awaiterVar, out var awaiterValue))
				return false;
			if (awaiterValue is not CallInstruction getAwaiterCall)
				return false;
			if (getAwaiterCall.Method.Name != "GetAwaiter" || getAwaiterCall.Arguments.Count != 1)
				return false;

			if (block.Instructions[^2] is not IfInstruction ifInst)
				return false;
			var condition = ifInst.Condition;
			if (condition.MatchLogicNot(out var negated))
				condition = negated;
			if (condition is not CallInstruction isCompletedCall)
				return false;
			if (isCompletedCall.Method.Name != "get_IsCompleted" || isCompletedCall.Arguments.Count != 1)
				return false;
			if (!isCompletedCall.Arguments[0].MatchLdLoca(awaiterVar))
				return false;

			Block completedBlock, pauseBlock;
			if (ifInst.TrueInst.MatchBranch(out var trueBranchTarget) && block.Instructions[^1].MatchBranch(out var falseBranchTarget))
			{
				if (negated != null)
				{
					// if (!IsCompleted) br pauseBlock; br completedBlock — flipped
					pauseBlock = trueBranchTarget;
					completedBlock = falseBranchTarget;
				}
				else
				{
					// if (IsCompleted) br completedBlock; br pauseBlock — canonical
					completedBlock = trueBranchTarget;
					pauseBlock = falseBranchTarget;
				}
			}
			else
			{
				return false;
			}

			// Block pauseBlock { call AsyncHelpers.UnsafeAwaitAwaiter(ldloc awaiter); br completedBlock }
			if (pauseBlock.Instructions.Count != 2)
				return false;
			if (pauseBlock.Instructions[0] is not Call pauseCall)
				return false;
			if (!EarlyExpressionTransforms.IsAsyncHelpersMethod(pauseCall.Method, "UnsafeAwaitAwaiter") || pauseCall.Arguments.Count != 1)
				return false;
			if (!pauseCall.Arguments[0].MatchLdLoc(awaiterVar))
				return false;
			if (!pauseBlock.Instructions[1].MatchBranch(out var pauseTail) || pauseTail != completedBlock)
				return false;

			// completedBlock starts with: GetResult(ldloca awaiter), possibly inlined.
			if (completedBlock.Instructions.Count < 1)
				return false;
			var getResultCall = ILInlining.FindFirstInlinedCall(completedBlock.Instructions[0]);
			if (getResultCall == null)
				return false;
			if (getResultCall.Method.Name != "GetResult" || getResultCall.Arguments.Count != 1)
				return false;
			if (!getResultCall.Arguments[0].MatchLdLoca(awaiterVar))
				return false;

			// Determine the awaitable expression: the original GetAwaiter argument (often `ldloca tmp`)
			// or, if `tmp` is a fresh temporary set immediately above, the value it was set to.
			ILInstruction awaitable;
			bool removeTemporaryStore = false;
			if (getAwaiterCall.Arguments[0].MatchLdLoca(out var tmpVar)
				&& block.Instructions.Count >= 4
				&& block.Instructions[^4].MatchStLoc(tmpVar, out var tmpValue)
				&& tmpVar.StoreCount == 1
				&& tmpVar.LoadCount == 0
				&& tmpVar.AddressCount == 1)
			{
				awaitable = tmpValue;
				removeTemporaryStore = true;
			}
			else
			{
				awaitable = getAwaiterCall.Arguments[0];
			}

			context.Step("Reduce manual await pattern to Await", block);

			// Capture IL ranges before we remove the suspend machinery. The new Await stands in
			// for the whole pattern (temp store + GetAwaiter + IsCompleted check + UnsafeAwaitAwaiter
			// + GetResult), and the new `br completedBlock` replaces the original `br pauseBlock`.
			var oldTailBranch = block.Instructions[^1];
			var awaitInst = new Await(awaitable.Clone()).WithILRange(getResultCall);
			if (removeTemporaryStore)
				awaitInst.AddILRange(block.Instructions[^4]);
			awaitInst.AddILRange(block.Instructions[^3]);
			awaitInst.AddILRange(block.Instructions[^2]);
			foreach (var inst in pauseBlock.Instructions)
				awaitInst.AddILRange(inst);
			awaitInst.GetAwaiterMethod = getAwaiterCall.Method;
			awaitInst.GetResultMethod = getResultCall.Method;

			// Remove the trailing 3 (or 4) instructions of the head block; replace with `br completedBlock`.
			int removeCount = removeTemporaryStore ? 4 : 3;
			block.Instructions.RemoveRange(block.Instructions.Count - removeCount, removeCount);
			block.Instructions.Add(new Branch(completedBlock).WithILRange(oldTailBranch));

			getResultCall.ReplaceWith(awaitInst);

			return true;
		}
	}
}
