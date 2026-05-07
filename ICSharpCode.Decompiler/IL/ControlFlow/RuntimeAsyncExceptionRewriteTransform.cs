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

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Reverses Roslyn's runtime-async lowering of try-catch-with-await and
	/// try-finally-with-await:
	///
	/// try-catch with await is lowered to a flag-flat shape where the catch handler stores
	/// a captured object plus a "did the catch fire" int flag, and the original catch body
	/// runs after the protected region inside an <c>if (flag == 1)</c> guard.
	///
	/// try-finally with await is lowered to a try-catch[object] whose handler stores the
	/// exception, followed by the original finally body inlined, followed by a
	/// <c>if (obj != null) ExceptionDispatchInfo.Capture((obj as Exception) ?? throw obj).Throw()</c>
	/// rethrow idiom.
	///
	/// This transform reverses both shapes so that the surrounding pipeline sees ordinary
	/// TryCatch / TryFinally instructions.
	/// </summary>
	static class RuntimeAsyncExceptionRewriteTransform
	{
		public static void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AwaitInCatchFinally)
				return;

			bool changed = false;

			// Pre-pass: normalize runtime-async catch filters that wrap a type-test + obj-store.
			// `catch (T ex) when (filter)` is lowered to `catch object when ({ isinst T; obj=ex; <filter> })`,
			// even when T is `object` (i.e. the source is `catch when (filter)`). After this pre-pass the
			// catch handler looks structurally identical to a non-filter catch, so the body matcher in
			// TryRewriteTryCatch can run unchanged.
			foreach (var handler in function.Descendants.OfType<TryCatchHandler>().ToArray())
			{
				if (NormalizeRuntimeAsyncFilter(handler, function, context))
					changed = true;
			}

			foreach (var tryCatch in function.Descendants.OfType<TryCatch>().ToArray())
			{
				if (tryCatch.Handlers.Count != 1)
					continue;
				if (tryCatch.Parent is not Block parentBlock)
					continue;
				if (parentBlock.Parent is not BlockContainer container)
					continue;
				if (tryCatch.ChildIndex != parentBlock.Instructions.Count - 1)
					continue;

				var handler = tryCatch.Handlers[0];
				if (handler.Body is not BlockContainer handlerBody)
					continue;
				if (handlerBody.Blocks.Count != 1)
					continue;
				var handlerBlock = handlerBody.Blocks[0];

				if (TryRewriteTryFinally(tryCatch, handler, handlerBlock, parentBlock, container, context))
				{
					changed = true;
					continue;
				}

				if (TryRewriteTryCatch(tryCatch, handler, handlerBlock, parentBlock, container, context))
				{
					changed = true;
					continue;
				}
			}

			if (changed)
			{
				foreach (var c in function.Body.Descendants.OfType<BlockContainer>())
					c.SortBlocks(deleteUnreachableBlocks: true);
			}
		}

		// Strip the runtime-async obj-store machinery from the entry of the filter's trueBody.
		// After this runs, the filter has the same shape as a state-machine-async catch-when filter,
		// and DetectCatchWhenConditionBlocks (the next EarlyILTransform) handles isinst recognition,
		// retyping handler.Variable to the user's catch type, and propagating it through trivial
		// stloc copies inside the handler. We only need to peel off the runtime-async-specific
		// captured-object prefix.
		//
		// Shape we strip from trueBody:
		//   [stloc tmp2(ldloc tmpVar);]                  optional copy
		//   stloc obj(ldloc tmp_or_tmp2)
		//   [stloc typedEx(castclass T'(ldloc obj));]    optional, when source has a typed catch var
		// where tmpVar is the isinst-result local set in the filter's entry block.
		static bool NormalizeRuntimeAsyncFilter(TryCatchHandler handler, ILFunction function, ILTransformContext context)
		{
			if (!handler.Variable.Type.IsKnownType(KnownTypeCode.Object))
				return false;
			if (handler.Filter is not BlockContainer filterContainer)
				return false;

			// Find the isinst-result temp via the standard catch-when entry shape:
			//   stloc tmp(isinst T(ldloc handlerVar)); if (tmp != null) br trueBody; br falseBody
			var entry = filterContainer.EntryPoint;
			if (entry.Instructions.Count != 3)
				return false;
			if (!entry.Instructions[0].MatchStLoc(out var tmpVar, out var tmpValue))
				return false;
			if (!tmpValue.MatchIsInst(out var isinstArg, out _))
				return false;
			if (!isinstArg.MatchLdLoc(handler.Variable))
				return false;
			if (entry.Instructions[1] is not IfInstruction ifInst)
				return false;
			if (!ifInst.Condition.MatchCompNotEqualsNull(out var condArg) || !condArg.MatchLdLoc(tmpVar))
				return false;
			if (ifInst.TrueInst is not Branch trueBranch)
				return false;
			var trueBody = trueBranch.TargetBlock;
			int n = trueBody.Instructions.Count;
			if (n < 1)
				return false;

			// Recognize the prefix.
			int prefix = 0;
			ILVariable tmp2Var = null;
			ILVariable objVar;
			if (!trueBody.Instructions[0].MatchStLoc(out var first, out var firstValue))
				return false;
			if (!firstValue.MatchLdLoc(tmpVar))
				return false;
			if (n > 1
				&& trueBody.Instructions[1].MatchStLoc(out var second, out var secondValue)
				&& secondValue.MatchLdLoc(first))
			{
				tmp2Var = first;
				objVar = second;
				prefix = 2;
			}
			else
			{
				objVar = first;
				prefix = 1;
			}

			ILVariable typedExVar = null;
			if (prefix < n
				&& trueBody.Instructions[prefix] is StLoc castStore
				&& castStore.Value is CastClass castClass
				&& castClass.Argument.MatchLdLoc(objVar))
			{
				typedExVar = castStore.Variable;
				prefix++;
			}

			context.StepStartGroup("Strip runtime-async catch-filter prefix", handler);

			trueBody.Instructions.RemoveRange(0, prefix);

			// Remap reads of the synthesized obj / tmp2 / typedEx variables to handler.Variable
			// throughout the function (the rethrow-dispatch idiom in the post-catch flow keys on obj).
			RemapVariableReads(function, objVar, handler.Variable);
			if (tmp2Var != null)
				RemapVariableReads(function, tmp2Var, handler.Variable);
			if (typedExVar != null)
				RemapVariableReads(function, typedExVar, handler.Variable);

			context.StepEndGroup(keepIfEmpty: true);
			return true;
		}

		static void RemapVariableReads(ILInstruction root, ILVariable from, ILVariable to)
		{
			foreach (var ldloc in root.Descendants.OfType<LdLoc>().ToArray())
			{
				if (ldloc.Variable != from)
					continue;
				// Drop redundant castclass to the new handler type that may now wrap the load.
				if (ldloc.Parent is CastClass cc && cc.Type.Equals(to.Type))
					cc.ReplaceWith(new LdLoc(to).WithILRange(cc).WithILRange(ldloc));
				else
					ldloc.ReplaceWith(new LdLoc(to).WithILRange(ldloc));
			}
			foreach (var stloc in root.Descendants.OfType<StLoc>().ToArray())
			{
				if (stloc.Variable == from && stloc.Parent is Block parent)
					parent.Instructions.RemoveAt(stloc.ChildIndex);
			}
		}

		// stloc obj(ldnull)
		// .try { ... ; br continuation }
		// catch ex : object when (ldc.i4 1) {
		//     [stloc tmp(ldloc ex);]
		//     stloc obj(ldloc tmp_or_ex)
		//     br continuation
		// }
		// Block continuation {
		//     <finally body>
		//     if (obj == null) leave outer
		//     <isinst-Exception + conditional throw via ExceptionDispatchInfo>
		// }
		// =>
		// .try { ... ; leave finallyContainer }
		// finally { <finally body> }
		// Block continuation {
		//     leave outer
		// }
		static bool TryRewriteTryFinally(TryCatch tryCatch, TryCatchHandler handler, Block handlerBlock,
			Block parentBlock, BlockContainer container, ILTransformContext context)
		{
			if (!handler.Variable.Type.IsKnownType(KnownTypeCode.Object))
				return false;

			// match catch body: [stloc tmp(ldloc ex);] stloc obj(ldloc tmp_or_ex); br continuation
			// Reuse the helper from AwaitInFinallyTransform — same shape on both lowerings.
			if (!AwaitInFinallyTransform.MatchObjectStoreCatchHandler((BlockContainer)handler.Body,
				handler.Variable, out var objectVariable, out var continuation))
			{
				return false;
			}
			if (!objectVariable.Type.IsKnownType(KnownTypeCode.Object))
				return false;
			if (continuation.Parent != container)
				return false;

			// Pre-try: somewhere among the instructions preceding the TryCatch we expect
			// an `stloc obj(ldnull)`. Other (unrelated) stores may be interleaved.
			var flagInitStore = FindFlagInitStore(parentBlock, tryCatch,
				s => s.Variable == objectVariable && s.Value.MatchLdNull());
			if (flagInitStore == null)
				return false;

			// Every outward exit of the try body must branch to `continuation`. The runtime-async
			// lowering rewrites every return / fallthrough to `br continuation` and routes throws
			// through the synthetic catch, so a Leave or a Branch to anything else means we're not
			// looking at a lowered shape. We also require at least one such exit, to reject try
			// bodies with no outward control flow at all.
			bool seenExit = false;
			foreach (var inst in tryCatch.TryBlock.Descendants.OfType<IBranchOrLeaveInstruction>())
			{
				// Skip intra-tryBody control flow: inst.TargetContainer being tryBody itself or any
				// container nested inside tryBody means control stays within tryBody. Only when the
				// target container is a strict ancestor of tryBody does the instruction exit it.
				if (inst.TargetContainer.IsDescendantOf(tryCatch.TryBlock))
					continue;
				if (inst is Branch branch && branch.TargetBlock == continuation)
					seenExit = true;
				else
					return false;
			}
			if (!seenExit)
				return false;

			// Find the dispatch idiom at the end of the finally body.
			// Pattern: a block ending with "if (obj == null) leave outer; br dispatchHead"
			// where dispatchHead is the block containing the isinst Exception + ExceptionDispatchInfo idiom.
			if (!FindFinallyDispatchExit(continuation, objectVariable, container,
				out var finallyExitBlock, out var dispatchBlocks, out var afterFinallyExit))
				return false;

			context.StepStartGroup("Rewrite runtime-async try-finally", tryCatch);

			// Determine whether the if-true branch of the finally exit is a direct Leave-with-value
			// or a Branch to a one-instruction leave block. In the latter case, that block stays
			// in the outer container so it follows the new TryFinally and provides the return value.
			Block leaveBlock = null;
			if (afterFinallyExit is Branch brToLeave)
				leaveBlock = brToLeave.TargetBlock;

			// Build the CFG once, before any structural changes. The dominator analysis below
			// uses this snapshot to identify which blocks belong to the finally body.
			var cfg = new ControlFlowGraph(container, context.CancellationToken);

			// Build a new BlockContainer for the finally body.
			var finallyContainer = new BlockContainer().WithILRange(handler.Body);

			// Replace the TryCatch with a TryFinally now so that the move below places blocks into
			// the freshly attached finallyContainer (which needs a parent to satisfy invariants).
			BlockContainer tryBlockContainer = (BlockContainer)tryCatch.TryBlock;
			var tryFinally = new TryFinally(tryBlockContainer, finallyContainer).WithILRange(tryCatch);
			tryCatch.ReplaceWith(tryFinally);

			// Move the finally body — all blocks dominated by `continuation`, stopping at
			// `finallyExitBlock` (so dispatchHead, captureBlock, throwBlock, and the leave block
			// stay in the outer container).
			AwaitInFinallyTransform.MoveDominatedBlocksToContainer(continuation, finallyExitBlock,
				cfg, finallyContainer, context);

			// Strip the trailing dispatch-check (last 2 instructions: if + br dispatchHead) from
			// the finally exit block, and end with `leave finallyContainer`.
			RewriteFinallyExit(finallyExitBlock, finallyContainer);

			// Redirect try-block branches that targeted `continuation` to `Leave(tryBlockContainer)`
			// so normal try-block completion runs the finally, then resumes after the TryFinally.
			foreach (var br in tryBlockContainer.Descendants.OfType<Branch>().ToArray())
			{
				if (br.TargetBlock == continuation)
					br.ReplaceWith(new Leave(tryBlockContainer).WithILRange(br));
			}

			// Append a successor instruction so the parent block remains EndPointUnreachable.
			// If there was a separate leave-block, branch to it (it stays in the outer container);
			// otherwise reuse the original Leave-with-value — RewriteFinallyExit already detached
			// it from the if-instruction that previously held it.
			if (leaveBlock != null)
				parentBlock.Instructions.Add(new Branch(leaveBlock).WithILRange(afterFinallyExit));
			else
				parentBlock.Instructions.Add(afterFinallyExit);

			// Remove the pre-init `stloc obj(ldnull)`. Also remove any dead
			// `stloc <int>(ldc.i4 0)` immediately preceding the TryFinally — the runtime-async
			// lowering pre-allocates a flag local even for try-finally where it's never read,
			// and leaving it between the resource store and the TryFinally would block
			// UsingTransform from recognizing the using/await foreach pattern.
			parentBlock.Instructions.RemoveAt(flagInitStore.ChildIndex);
			RemoveDeadFlagStores(parentBlock, tryFinally);

			// Dispatch blocks are now unreachable; SortBlocks at the end of Run will drop them.
			context.StepEndGroup(keepIfEmpty: true);
			return true;
		}

		// Scan instructions before `tryCatch` for the runtime-async flag-init store that matches
		// `predicate`. The lowering inserts an `stloc obj(ldnull)` (try-finally) or
		// `stloc num(ldc.i4 0)` (try-catch) before the try region; the catch handler overwrites it,
		// the continuation reads it to decide whether an exception occurred (and which case).
		// Returns null when no matching store is found.
		static StLoc FindFlagInitStore(Block parentBlock, TryCatch tryCatch, Predicate<StLoc> predicate)
		{
			int tryIndex = tryCatch.ChildIndex;
			for (int i = 0; i < tryIndex; i++)
			{
				if (parentBlock.Instructions[i] is StLoc s && predicate(s))
					return s;
			}
			return null;
		}

		// Remove `stloc v(ldc.i4 0)` instructions immediately preceding `tryFinally` whose target
		// variable is never read.
		static void RemoveDeadFlagStores(Block parentBlock, TryFinally tryFinally)
		{
			while (tryFinally.ChildIndex > 0
				&& parentBlock.Instructions[tryFinally.ChildIndex - 1] is StLoc deadStore
				&& deadStore.Value.MatchLdcI4(0)
				&& deadStore.Variable.LoadCount == 0)
			{
				parentBlock.Instructions.RemoveAt(deadStore.ChildIndex);
			}
		}

		// Locate the "if (obj == null) leave outer; br dispatchHead" finally-exit block, plus all dispatch blocks.
		static bool FindFinallyDispatchExit(Block start, ILVariable objectVariable, BlockContainer container,
			out Block finallyExitBlock, out List<Block> dispatchBlocks, out ILInstruction afterFinallyExit)
		{
			finallyExitBlock = null;
			dispatchBlocks = null;
			afterFinallyExit = null;

			// Walk reachable blocks until we find a block whose body matches the finally-exit shape.
			var visited = new HashSet<Block>();
			var queue = new Queue<Block>();
			queue.Enqueue(start);
			while (queue.Count > 0)
			{
				var b = queue.Dequeue();
				if (!visited.Add(b))
					continue;
				if (MatchFinallyExitBlock(b, objectVariable, out var dispatchHead, out afterFinallyExit))
				{
					finallyExitBlock = b;
					if (CollectDispatchBlocks(dispatchHead, objectVariable, out dispatchBlocks))
						return true;
					return false;
				}
				foreach (var br in b.Descendants.OfType<Branch>())
				{
					if (br.TargetBlock?.Parent == container)
						queue.Enqueue(br.TargetBlock);
				}
			}
			return false;
		}

		// Match a tail of the form
		//     if (comp.o(ldloc obj == ldnull)) <leaveOuter | br anyBlock>
		//     br dispatchHead
		// (trailing two instructions of the block; instructions before are part of the finally body).
		// The if-true branch may go directly to a function leave, to a one-instruction leave block, or to
		// any other block in the outer container (e.g. an early-return flag check from a nested catch).
		static bool MatchFinallyExitBlock(Block block, ILVariable objectVariable, out Block dispatchHead, out ILInstruction afterFinallyExit)
		{
			dispatchHead = null;
			afterFinallyExit = null;

			if (block.Instructions.Count < 2)
				return false;
			if (block.Instructions[^2] is not IfInstruction ifInst)
				return false;
			if (!ifInst.Condition.MatchCompEqualsNull(out var arg) || !arg.MatchLdLoc(objectVariable))
				return false;
			afterFinallyExit = ifInst.TrueInst;
			if (afterFinallyExit is Leave leaveOuter && leaveOuter.IsLeavingFunction)
			{
				// direct leave OK
			}
			else if (afterFinallyExit is Branch brTarget)
			{
				// Branch to another block — could be a single-instruction leave block (canonical) or
				// a non-trivial successor (e.g. an early-return check from a nested catch).
				if (brTarget.TargetBlock.Instructions.Count == 1
					&& brTarget.TargetBlock.Instructions[0] is Leave leaveOuter2
					&& leaveOuter2.IsLeavingFunction)
				{
					afterFinallyExit = leaveOuter2;
				}
				// otherwise: keep the Branch as afterFinallyExit so the post-TryFinally successor is wired up
			}
			else
			{
				return false;
			}
			if (!block.Instructions[^1].MatchBranch(out dispatchHead))
				return false;
			return true;
		}

		// Block dispatchHead {
		//     stloc tmp(isinst Exception(ldloc obj))
		//     if (comp.o(ldloc tmp != ldnull)) br captureBlock
		//     br throwBlock
		// }
		// Block captureBlock { callvirt Throw(call Capture(ldloc tmp)); leave outer }
		// Block throwBlock { throw(ldloc obj) }
		static bool CollectDispatchBlocks(Block dispatchHead, ILVariable objectVariable, out List<Block> dispatchBlocks)
		{
			dispatchBlocks = null;
			if (dispatchHead.Instructions.Count != 3)
				return false;
			if (!dispatchHead.Instructions[0].MatchStLoc(out var typedExVar, out var typedExValue))
				return false;
			if (!typedExValue.MatchIsInst(out var isInstArg, out _))
				return false;
			if (!isInstArg.MatchLdLoc(objectVariable))
				return false;
			if (dispatchHead.Instructions[1] is not IfInstruction ifInst)
				return false;
			if (!ifInst.Condition.MatchCompNotEqualsNull(out var notNullArg) || !notNullArg.MatchLdLoc(typedExVar))
				return false;
			if (ifInst.TrueInst is not Branch toCapture)
				return false;
			if (dispatchHead.Instructions[2] is not Branch toThrow)
				return false;

			dispatchBlocks = new List<Block> { dispatchHead, toCapture.TargetBlock, toThrow.TargetBlock };
			return true;
		}

		static void RewriteFinallyExit(Block finallyExitBlock, BlockContainer finallyContainer)
		{
			// Strip the trailing 2 instructions (if + br dispatchHead) and append `leave finallyContainer`.
			// The new Leave occupies the same end-of-finally position the removed dispatch check sat at,
			// so inherit the IL range from both to keep source mapping aligned.
			var ifInst = finallyExitBlock.Instructions[^2];
			var brInst = finallyExitBlock.Instructions[^1];
			finallyExitBlock.Instructions.RemoveRange(finallyExitBlock.Instructions.Count - 2, 2);
			var leave = new Leave(finallyContainer).WithILRange(ifInst);
			leave.AddILRange(brInst);
			finallyExitBlock.Instructions.Add(leave);
		}

		static List<Block> CollectReachable(Block entry, List<Block> exclude)
		{
			var excludeSet = new HashSet<Block>(exclude);
			var visited = new HashSet<Block>();
			var result = new List<Block>();
			var queue = new Queue<Block>();
			queue.Enqueue(entry);
			while (queue.Count > 0)
			{
				var b = queue.Dequeue();
				if (!visited.Add(b))
					continue;
				if (excludeSet.Contains(b))
					continue;
				if (b.Parent != entry.Parent)
					continue;
				result.Add(b);
				foreach (var br in b.Descendants.OfType<Branch>())
				{
					if (br.TargetBlock != null && br.TargetBlock.Parent == entry.Parent)
						queue.Enqueue(br.TargetBlock);
				}
			}
			return result;
		}

		// stloc num(0)
		// .try { ... ; br continuation }
		// catch ex : T when (ldc.i4 1) {
		//     [stloc tmp(ldloc ex);]
		//     [stloc obj(ldloc tmp_or_ex);]
		//     stloc num(1)
		//     br continuation
		// }
		// Block continuation {
		//     if (comp.i4(num != 1)) leave outer ; or "if (num == 1) br catchBody; leave outer"
		//     br catchBody
		// }
		// Block catchBody { ... }
		// =>
		// .try { ... ; br continuation }
		// catch ex : T { ...catchBody, with reads of obj as ex... }
		// Block continuation {
		//     leave outer
		// }
		static bool TryRewriteTryCatch(TryCatch tryCatch, TryCatchHandler handler, Block handlerBlock,
			Block parentBlock, BlockContainer container, ILTransformContext context)
		{
			// Match catch body: [stloc tmp(ldloc ex);] [stloc obj(ldloc tmp);] stloc num(1); br continuation
			if (handlerBlock.Instructions.Count < 2)
				return false;
			if (!handlerBlock.Instructions.Last().MatchBranch(out var continuation))
				return false;
			if (continuation.Parent != container)
				return false;

			ILVariable numVariable;
			ILInstruction numStore = handlerBlock.Instructions[^2];
			if (!numStore.MatchStLoc(out numVariable, out var numValue))
				return false;
			if (!numValue.MatchLdcI4(1))
				return false;
			if (!numVariable.Type.IsKnownType(KnownTypeCode.Int32))
				return false;

			// Collect optional tmp/obj stores (everything before the num=1 store)
			ILVariable objectVariable = null;
			ILVariable temporaryVariable = null;
			int prefixCount = handlerBlock.Instructions.Count - 2;
			if (prefixCount >= 1)
			{
				// Last prefix instruction may be: stloc obj(ldloc tmp_or_ex)
				if (handlerBlock.Instructions[prefixCount - 1].MatchStLoc(out var v, out var val))
				{
					if (val.MatchLdLoc(handler.Variable))
					{
						// Direct stloc obj(ldloc ex) — no tmp variable
						objectVariable = v;
						prefixCount--;
					}
					else if (val.MatchLdLoc(out var tmpV) && prefixCount >= 2
						&& handlerBlock.Instructions[prefixCount - 2].MatchStLoc(out var tmpV2, out var tmpVal)
						&& tmpV == tmpV2 && tmpVal.MatchLdLoc(handler.Variable))
					{
						temporaryVariable = tmpV;
						objectVariable = v;
						prefixCount -= 2;
					}
					else
					{
						return false;
					}
				}
			}
			if (prefixCount != 0)
				return false;

			// Pre-try: somewhere before the TryCatch we expect `stloc num(ldc.i4 0)`.
			var flagInitStore = FindFlagInitStore(parentBlock, tryCatch,
				s => s.Variable == numVariable && s.Value.MatchLdcI4(0));
			if (flagInitStore == null)
				return false;

			// Continuation must contain a "num == 1" check that branches to the catch body, or
			// alternatively a "num != 1" check that leaves outer.
			if (!MatchCatchEntryCheck(continuation, numVariable, container,
				out var catchBodyEntry, out var afterCatchExit))
				return false;

			context.StepStartGroup("Rewrite runtime-async try-catch", tryCatch);

			// Move catch body blocks (those dominated by catchBodyEntry within `container`) into the handler.
			var bodyBlocks = CollectReachable(catchBodyEntry, new List<Block>());
			foreach (var b in bodyBlocks)
			{
				b.Remove();
			}

			// Replace handler's existing block (which was just the prefix + num=1 + branch)
			// with the catch body. Preserve the original branch target redirection: branches that
			// targeted `continuation` from inside the body now target the new continuation
			// (the `leave outer` block which is `continuation` itself, after we strip its catch-entry-check).
			handlerBlock.Instructions.Clear();
			handlerBlock.Instructions.Add(new Branch(catchBodyEntry));
			foreach (var b in bodyBlocks)
				((BlockContainer)handler.Body).Blocks.Add(b);

			// Replace reads of `obj` (and `tmp`) inside the moved catch body with reads of handler.Variable.
			if (objectVariable != null)
				ReplaceVariableReadsWithHandlerVariable(handler.Body, objectVariable, handler.Variable);
			if (temporaryVariable != null)
				ReplaceVariableReadsWithHandlerVariable(handler.Body, temporaryVariable, handler.Variable);

			// Inside the moved blocks, locate any leftover dispatch idiom (originating from `throw;`)
			// and replace it with `Rethrow`.
			foreach (var b in handler.Body.Descendants.OfType<Block>().ToArray())
				ReplaceDispatchIdiomWithRethrow(b, handler.Variable, context);

			// Strip the catch-entry check from `continuation` — replace with a raw `leave outer`.
			// Clear() already detached `afterCatchExit` (it was either continuation.Instructions[1]
			// or a child of the now-detached if-instruction), so we can re-add it directly.
			continuation.Instructions.Clear();
			continuation.Instructions.Add(afterCatchExit);

			// Remove the pre-try `stloc num(0)`.
			parentBlock.Instructions.RemoveAt(flagInitStore.ChildIndex);

			context.StepEndGroup(keepIfEmpty: true);
			return true;
		}

		// Block continuation {
		//   Variant A: if (comp.i4(num != 1)) leave outer; br catchBody
		//   Variant B: if (comp.i4(num == 1)) br catchBody; leave outer
		// }
		static bool MatchCatchEntryCheck(Block continuation, ILVariable numVariable, BlockContainer container,
			out Block catchBodyEntry, out ILInstruction afterCatchExit)
		{
			catchBodyEntry = null;
			afterCatchExit = null;

			if (continuation.Instructions.Count != 2)
				return false;
			if (continuation.Instructions[0] is not IfInstruction ifInst)
				return false;

			// Equals form: if (num == 1) br catchBody ; <fallthrough leave outer>
			if (ifInst.Condition.MatchCompEquals(out var lhs, out var rhs)
				&& lhs.MatchLdLoc(numVariable) && rhs.MatchLdcI4(1)
				&& ifInst.TrueInst is Branch eqBranch
				&& continuation.Instructions[1] is Leave eqLeave && eqLeave.IsLeavingFunction)
			{
				catchBodyEntry = eqBranch.TargetBlock;
				afterCatchExit = eqLeave;
				return catchBodyEntry?.Parent == container;
			}

			// Not-equals form: if (num != 1) leave outer ; br catchBody
			if (ifInst.Condition.MatchCompNotEquals(out lhs, out rhs)
				&& lhs.MatchLdLoc(numVariable) && rhs.MatchLdcI4(1)
				&& ifInst.TrueInst is Leave neLeave && neLeave.IsLeavingFunction
				&& continuation.Instructions[1] is Branch neBranch)
			{
				catchBodyEntry = neBranch.TargetBlock;
				afterCatchExit = neLeave;
				return catchBodyEntry?.Parent == container;
			}

			return false;
		}

		static void ReplaceVariableReadsWithHandlerVariable(ILInstruction root, ILVariable from, ILVariable to)
		{
			foreach (var ldloc in root.Descendants.OfType<LdLoc>().ToArray())
			{
				if (ldloc.Variable != from)
					continue;
				// If parent is a CastClass to handler.Variable.Type or a base, inline directly.
				if (ldloc.Parent is CastClass cc && cc.Type.Equals(to.Type))
				{
					cc.ReplaceWith(new LdLoc(to).WithILRange(cc).WithILRange(ldloc));
				}
				else
				{
					ldloc.ReplaceWith(new LdLoc(to).WithILRange(ldloc));
				}
			}
			foreach (var stloc in root.Descendants.OfType<StLoc>().ToArray())
			{
				if (stloc.Variable == from)
				{
					// Drop dead stores to the synthesized variable.
					if (stloc.Parent is Block parentBlock)
					{
						parentBlock.Instructions.RemoveAt(stloc.ChildIndex);
					}
				}
			}
		}

		static void ReplaceDispatchIdiomWithRethrow(Block block, ILVariable handlerVariable, ILTransformContext context)
		{
			// Reuse AwaitInCatchTransform.MatchExceptionCaptureBlock through the block-tail shape:
			//   stloc typedExVar(isinst Exception(ldloc handlerVariable))
			//   if (comp.o(ldloc typedExVar != ldnull)) br captureBlock
			//   br throwBlock
			// Block captureBlock { callvirt Throw(call Capture(ldloc typedExVar)); leave/br }
			// Block throwBlock { throw(ldloc handlerVariable) }
			ILVariable v = handlerVariable;
			if (AwaitInCatchTransform.MatchExceptionCaptureBlock(context, block, ref v,
				out var typedExceptionVariableStore, out var captureBlock, out var throwBlock))
			{
				if (v != handlerVariable)
					return;
				// The Rethrow stands in for the whole dispatch idiom (this block's tail + the
				// capture/throw blocks). Capture IL ranges from each component before the
				// removals detach them, so source mapping stays anchored to the original bytes.
				var rethrow = new Rethrow().WithILRange(typedExceptionVariableStore);
				rethrow.AddILRange(block.Instructions[typedExceptionVariableStore.ChildIndex + 1]);
				rethrow.AddILRange(block.Instructions[typedExceptionVariableStore.ChildIndex + 2]);
				foreach (var inst in captureBlock.Instructions)
					rethrow.AddILRange(inst);
				foreach (var inst in throwBlock.Instructions)
					rethrow.AddILRange(inst);
				block.Instructions.RemoveRange(typedExceptionVariableStore.ChildIndex + 1, 2);
				captureBlock.Remove();
				throwBlock.Remove();
				typedExceptionVariableStore.ReplaceWith(rethrow);
			}
		}
	}
}
