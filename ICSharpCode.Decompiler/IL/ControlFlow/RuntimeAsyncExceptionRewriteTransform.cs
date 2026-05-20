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
using ICSharpCode.Decompiler.Util;

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
			//
			// The captured obj local is shared across every catch handler that opts into the runtime-async
			// rethrow protocol, so per-handler filter normalization must NOT do a function-wide remap of
			// reads of obj — that would tag every reference (in both this handler's own dispatch idiom and
			// every other handler's dispatch idiom) with the same handler variable. Instead, record the
			// obj variable per handler and let TryRewriteTryCatch / the multi-handler transform remap it
			// scoped to the moved catch body.
			var filterObjByHandler = new Dictionary<TryCatchHandler, ILVariable>();
			foreach (var handler in function.Descendants.OfType<TryCatchHandler>().ToArray())
			{
				if (NormalizeRuntimeAsyncFilter(handler, function, filterObjByHandler, context))
					changed = true;
			}

			foreach (var tryCatch in function.Descendants.OfType<TryCatch>().ToArray())
			{
				if (tryCatch.Parent is not Block parentBlock)
					continue;
				if (parentBlock.Parent is not BlockContainer container)
					continue;

				if (tryCatch.Handlers.Count == 1)
				{
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

					if (TryRewriteTryCatch(tryCatch, handler, handlerBlock, parentBlock, container, filterObjByHandler, context))
					{
						changed = true;
						continue;
					}
				}
				else if (tryCatch.Handlers.Count >= 2)
				{
					if (TryRewriteMultiHandlerTryCatch(tryCatch, parentBlock, container, filterObjByHandler, context))
					{
						changed = true;
						continue;
					}
				}
			}

			// Recognize the "early return from inside a try via a flag local" pattern that runtime-async
			// emits whenever a return statement crosses an enclosing try-finally with await.
			foreach (var tryFinally in function.Descendants.OfType<TryFinally>().ToArray())
			{
				if (TryRewriteFlagBasedEarlyReturn(tryFinally, context))
					changed = true;
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
		// where tmpVar is the isinst-result local set in the filter's entry block. The captured `obj`
		// local is shared across every catch handler in the multi-handler form, so it must NOT be
		// remapped function-wide here; we record it per handler and let the body rewriter scope the
		// remap to each handler's catch body.
		static bool NormalizeRuntimeAsyncFilter(TryCatchHandler handler, ILFunction function,
			Dictionary<TryCatchHandler, ILVariable> filterObjByHandler, ILTransformContext context)
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

			// Remap reads of the per-handler synthesized variables (tmp2 / typedEx) to handler.Variable.
			// Both are unique per handler so a function-wide remap is safe.
			if (tmp2Var != null)
				RemapVariableReads(function, tmp2Var, handler.Variable);
			if (typedExVar != null)
				RemapVariableReads(function, typedExVar, handler.Variable);

			// Optimized builds inline `castclass T(ldloc obj)` directly into the user filter expression
			// instead of stashing it in a typedEx local. Remap obj reads within the filter only.
			RemapVariableReads(handler.Filter, objVar, handler.Variable);

			// The obj variable is shared between handlers in the multi-handler form; the body rewriter
			// remaps it scoped per handler.
			filterObjByHandler[handler] = objVar;

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
			var flagInitStore = FindFlagInitStore(parentBlock, tryCatch, objectVariable, v => v.MatchLdNull());
			if (flagInitStore == null)
				return false;

			// Every outward exit of the try body must branch to `continuation`. The runtime-async
			// lowering rewrites every return / fallthrough to `br continuation` and routes throws
			// through the synthetic catch, so a Leave or a Branch to anything else means we're not
			// looking at a lowered shape. A try body with no outward exit at all is also fine —
			// the user wrote `try { throw ...; } finally { await ... }`, which lowers to a try body
			// whose only exit is the throw (handled by the synthetic catch).
			foreach (var inst in tryCatch.TryBlock.Descendants.OfType<IBranchOrLeaveInstruction>())
			{
				// Skip intra-tryBody control flow: inst.TargetContainer being tryBody itself or any
				// container nested inside tryBody means control stays within tryBody. Only when the
				// target container is a strict ancestor of tryBody does the instruction exit it.
				if (inst.TargetContainer.IsDescendantOf(tryCatch.TryBlock))
					continue;
				if (inst is Branch branch && branch.TargetBlock == continuation)
					continue;
				return false;
			}

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
			// it from the if-instruction that previously held it. Skip when the try body always
			// throws — the resulting TryFinally's endpoint is unreachable and a successor
			// instruction after it would put a non-final unreachable-endpoint instruction in the
			// block, violating the block invariant.
			if (!tryFinally.HasFlag(InstructionFlags.EndPointUnreachable))
			{
				if (leaveBlock != null)
					parentBlock.Instructions.Add(new Branch(leaveBlock).WithILRange(afterFinallyExit));
				else
					parentBlock.Instructions.Add(afterFinallyExit);
			}

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

		// Scan instructions before `tryCatch` for the runtime-async flag-init store. The lowering
		// inserts an `stloc obj(ldnull)` (try-finally) or `stloc num(ldc.i4 0)` (try-catch) before
		// the try region; the catch handler overwrites it, and the continuation reads it to decide
		// whether an exception occurred (and which case). Match by `referenceVar`'s slot/kind/type
		// rather than identity — after SplitVariables, the pre-init's ILVariable may differ from
		// the in-handler one. Returns null when no matching store is found.
		static StLoc FindFlagInitStore(Block parentBlock, TryCatch tryCatch, ILVariable referenceVar, Predicate<ILInstruction> matchInitValue)
		{
			int tryIndex = tryCatch.ChildIndex;
			for (int i = 0; i < tryIndex; i++)
			{
				if (parentBlock.Instructions[i] is not StLoc s)
					continue;
				if (s.Variable.Index != referenceVar.Index || s.Variable.Kind != referenceVar.Kind)
					continue;
				if (!s.Variable.Type.Equals(referenceVar.Type))
					continue;
				if (!matchInitValue(s.Value))
					continue;
				return s;
			}
			return null;
		}

		// True when `leave` exits `container` itself or any ancestor container — i.e. control transfers
		// out of `container`. Replaces the older `IsLeavingFunction` gate, which only matched the
		// top-level case; nested runtime-async patterns also leave to intermediate containers.
		static bool LeaveExitsContainer(Leave leave, BlockContainer container)
		{
			if (leave.TargetContainer == container)
				return true;
			return container.IsDescendantOf(leave.TargetContainer);
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
			Block parentBlock, BlockContainer container,
			Dictionary<TryCatchHandler, ILVariable> filterObjByHandler, ILTransformContext context)
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
			var flagInitStore = FindFlagInitStore(parentBlock, tryCatch, numVariable, v => v.MatchLdcI4(0));
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
			// `objectVariable` here is the handler's body-level obj (only present for non-filter catches);
			// `filterObj` is the obj recorded by NormalizeRuntimeAsyncFilter when the catch carries a filter.
			if (objectVariable != null)
				ReplaceVariableReadsWithHandlerVariable(handler.Body, objectVariable, handler.Variable);
			if (temporaryVariable != null)
				ReplaceVariableReadsWithHandlerVariable(handler.Body, temporaryVariable, handler.Variable);
			if (filterObjByHandler.TryGetValue(handler, out var filterObj))
				ReplaceVariableReadsWithHandlerVariable(handler.Body, filterObj, handler.Variable);

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

		// Multi-handler runtime-async try-catch:
		//
		//   stloc num(0)
		//   .try { ... ; br continuation }
		//   catch ex_1 : T_1 when (...) { [stloc tmp1(ex_1);] [stloc obj(...);] stloc num(K_1); br continuation }
		//   catch ex_2 : T_2 when (...) { [stloc tmp2(ex_2);] [stloc obj(...);] stloc num(K_2); br continuation }
		//   ...
		// Block continuation {
		//   switch (ldloc num) { case [K_i..K_i+1): br case_K_i ... ; default: leave outer }
		// }
		// Block case_K_i { <user catch body for handler i> }
		//
		// =>
		//   .try { ... ; br continuation }
		//   catch ex_1 : T_1 when (...) { ...case body 1 inlined, with obj/tmp reads remapped to ex_1... }
		//   catch ex_2 : T_2 when (...) { ...case body 2 inlined, with obj/tmp reads remapped to ex_2... }
		//   ...
		// Block continuation {
		//   leave outer
		// }
		static bool TryRewriteMultiHandlerTryCatch(TryCatch tryCatch, Block parentBlock, BlockContainer container,
			Dictionary<TryCatchHandler, ILVariable> filterObjByHandler, ILTransformContext context)
		{
			Block continuation = null;
			ILVariable numVariable = null;
			var seenK = new HashSet<int>();
			var handlerInfos = new List<(TryCatchHandler handler, int k, ILVariable bodyObj, ILVariable bodyTmp)>();

			foreach (var handler in tryCatch.Handlers)
			{
				if (handler.Body is not BlockContainer hb || hb.Blocks.Count != 1)
					return false;
				var hblock = hb.Blocks[0];
				if (hblock.Instructions.Count < 2)
					return false;
				if (!hblock.Instructions.Last().MatchBranch(out var br))
					return false;
				if (continuation == null)
					continuation = br;
				else if (continuation != br)
					return false;
				if (continuation.Parent != container)
					return false;

				if (!hblock.Instructions[^2].MatchStLoc(out var nv, out var nval))
					return false;
				if (!nval.MatchLdcI4(out int k))
					return false;
				if (!nv.Type.IsKnownType(KnownTypeCode.Int32))
					return false;
				if (numVariable == null)
					numVariable = nv;
				else if (numVariable != nv)
					return false;
				if (!seenK.Add(k))
					return false;

				ILVariable bodyObj = null, bodyTmp = null;
				int prefix = hblock.Instructions.Count - 2;
				if (prefix >= 1 && hblock.Instructions[prefix - 1].MatchStLoc(out var v, out var val))
				{
					if (val.MatchLdLoc(handler.Variable))
					{
						bodyObj = v;
						prefix--;
					}
					else if (val.MatchLdLoc(out var tmpV) && prefix >= 2
						&& hblock.Instructions[prefix - 2].MatchStLoc(out var tmpV2, out var tmpVal)
						&& tmpV == tmpV2 && tmpVal.MatchLdLoc(handler.Variable))
					{
						bodyTmp = tmpV;
						bodyObj = v;
						prefix -= 2;
					}
					else
					{
						return false;
					}
				}
				if (prefix != 0)
					return false;

				handlerInfos.Add((handler, k, bodyObj, bodyTmp));
			}

			// Pre-try: stloc num(ldc.i4 0).
			var flagInitStore = FindFlagInitStore(parentBlock, tryCatch, numVariable, v => v.MatchLdcI4(0));
			if (flagInitStore == null)
				return false;
			// Block continuation { switch (ldloc num) { case K: br case_K ... ; default: leave outer } }
			// — or, for a small number of handlers (typically 2) where Roslyn emits an if-chain
			// instead of a switch — a chain of `if (num == K_i) br case_K_i` blocks ending in a leave.
			if (!MatchSwitchDispatch(continuation, numVariable, out var caseBlocks, out var defaultExit)
				&& !MatchIfChainDispatch(continuation, numVariable, container, out caseBlocks, out defaultExit))
				return false;
			// Every K we recorded must have a case in the switch.
			foreach (var info in handlerInfos)
				if (!caseBlocks.ContainsKey(info.k))
					return false;

			context.StepStartGroup("Rewrite runtime-async multi-handler try-catch", tryCatch);
			var cfg = new ControlFlowGraph(container, context.CancellationToken);

			foreach (var info in handlerInfos)
			{
				var caseBody = caseBlocks[info.k];
				var handler = info.handler;
				var handlerBody = (BlockContainer)handler.Body;
				var handlerEntry = handlerBody.Blocks[0];

				// Move case-body and dominated blocks into the handler's body container.
				AwaitInFinallyTransform.MoveDominatedBlocksToContainer(caseBody, null, cfg, handlerBody, context);

				// Replace the handler-entry block (still holds the prefix + num=K + branch) with a
				// single `br caseBody`, since caseBody is now at index >= 1 in handlerBody.
				handlerEntry.Instructions.Clear();
				handlerEntry.Instructions.Add(new Branch(caseBody));

				// Remap the per-handler synthesized variables to the handler.Variable inside the moved body.
				if (info.bodyObj != null)
					ReplaceVariableReadsWithHandlerVariable(handler.Body, info.bodyObj, handler.Variable);
				if (info.bodyTmp != null)
					ReplaceVariableReadsWithHandlerVariable(handler.Body, info.bodyTmp, handler.Variable);
				if (filterObjByHandler.TryGetValue(handler, out var filterObj))
					ReplaceVariableReadsWithHandlerVariable(handler.Body, filterObj, handler.Variable);

				foreach (var b in handler.Body.Descendants.OfType<Block>().ToArray())
					ReplaceDispatchIdiomWithRethrow(b, handler.Variable, context);
			}

			// Replace continuation with the default exit (leave outer). Clone the default-exit so
			// we don't worry about whose tree it currently belongs to (the switch instruction we're
			// tearing down, or a later block in an if-chain dispatch). Clear the clone's ILRange —
			// it now sits at a different location than the original, so reusing the source offset
			// would produce wrong sequence points.
			var defaultExitClone = defaultExit.Clone();
			defaultExitClone.SetILRange(new Interval());
			continuation.Instructions.Clear();
			continuation.Instructions.Add(defaultExitClone);

			// Remove the pre-try `stloc num(0)`.
			parentBlock.Instructions.RemoveAt(flagInitStore.ChildIndex);

			context.StepEndGroup(keepIfEmpty: true);
			return true;
		}

		// For 2-handler multi-catches, Roslyn emits an if-chain rather than a switch:
		//   Block continuation {
		//     if (ldloc num == K_1) br case_K_1
		//     br nextBlock
		//   }
		//   Block nextBlock {
		//     if (ldloc num == K_2) br case_K_2
		//     <leave outer | br defaultExitBlock>
		//   }
		// Where the chain may extend beyond two if-blocks.
		static bool MatchIfChainDispatch(Block continuation, ILVariable numVariable,
			BlockContainer container, out Dictionary<int, Block> caseBlocks, out ILInstruction defaultExit)
		{
			caseBlocks = new Dictionary<int, Block>();
			defaultExit = null;
			var visited = new HashSet<Block>();
			var current = continuation;
			while (true)
			{
				if (!visited.Add(current))
					return false;
				if (current.Instructions.Count != 2)
					return false;
				if (current.Instructions[0] is not IfInstruction ifInst)
					return false;
				if (!ifInst.Condition.MatchCompEquals(out var lhs, out var rhs)
					|| !lhs.MatchLdLoc(numVariable)
					|| !rhs.MatchLdcI4(out int k))
					return false;
				if (!ifInst.TrueInst.MatchBranch(out var caseBlock))
					return false;
				if (caseBlocks.ContainsKey(k))
					return false;
				caseBlocks[k] = caseBlock;
				var fallthrough = current.Instructions[1];
				if (fallthrough is Leave directLeave && IsLeaveToContainerOrAncestor(directLeave, container))
				{
					defaultExit = directLeave;
					return caseBlocks.Count > 0;
				}
				if (!fallthrough.MatchBranch(out var nextBlock) || nextBlock.Parent != container)
					return false;
				// One-instruction leave block ends the chain — typical for "switch default" / "no case matched".
				if (nextBlock.Instructions.Count == 1 && nextBlock.Instructions[0] is Leave finalLeave
					&& IsLeaveToContainerOrAncestor(finalLeave, container))
				{
					defaultExit = finalLeave;
					return caseBlocks.Count > 0;
				}
				current = nextBlock;
			}
		}

		// Block continuation { switch (ldloc num) { case [K..K+1): br case_K ... ; default: <leave outer | br end> } }
		static bool MatchSwitchDispatch(Block continuation, ILVariable numVariable,
			out Dictionary<int, Block> caseBlocks, out ILInstruction defaultExit)
		{
			caseBlocks = null;
			defaultExit = null;
			if (continuation.Instructions.Count != 1)
				return false;
			if (continuation.Instructions[0] is not SwitchInstruction switchInst)
				return false;
			if (!switchInst.Value.MatchLdLoc(numVariable))
				return false;

			var defaultSection = switchInst.GetDefaultSection();
			if (defaultSection == null)
				return false;
			if (defaultSection.Body is Leave defLeave && defLeave.IsLeavingFunction)
				defaultExit = defLeave;
			else if (defaultSection.Body is Branch)
				defaultExit = defaultSection.Body;
			else
				return false;

			caseBlocks = new Dictionary<int, Block>();
			foreach (var section in switchInst.Sections)
			{
				if (section == defaultSection)
					continue;
				if (!section.Body.MatchBranch(out var caseBlock))
					return false;
				// Each non-default section's labels must cover exactly one integer value (K).
				if (section.Labels.Count() != 1)
					return false;
				caseBlocks[(int)section.Labels.Intervals[0].Start] = caseBlock;
			}
			return true;
		}

		// Block continuation {
		//   Variant A: if (comp.i4(num != 1)) <exit>; br catchBody
		//   Variant B: if (comp.i4(num == 1)) br catchBody; <exit>
		// }
		// <exit> is either a direct Leave that exits `container` (or any ancestor) — which is the
		// top-level shape where "no exception" leaves the function — or a Branch to a leave-block
		// in `container`. The Branch form arises when the try-catch is nested (e.g. inside an
		// outer try-finally), where "no exception" branches to the outer try-block's exit point.
		static bool MatchCatchEntryCheck(Block continuation, ILVariable numVariable, BlockContainer container,
			out Block catchBodyEntry, out ILInstruction afterCatchExit)
		{
			catchBodyEntry = null;
			afterCatchExit = null;

			if (continuation.Instructions.Count != 2)
				return false;
			if (continuation.Instructions[0] is not IfInstruction ifInst)
				return false;

			// Equals form: if (num == 1) br catchBody ; <exit>
			if (ifInst.Condition.MatchCompEquals(out var lhs, out var rhs)
				&& lhs.MatchLdLoc(numVariable) && rhs.MatchLdcI4(1)
				&& ifInst.TrueInst is Branch eqBranch
				&& IsContainerExit(continuation.Instructions[1], container))
			{
				catchBodyEntry = eqBranch.TargetBlock;
				afterCatchExit = continuation.Instructions[1];
				return catchBodyEntry?.Parent == container;
			}

			// Not-equals form: if (num != 1) <exit> ; br catchBody
			if (ifInst.Condition.MatchCompNotEquals(out lhs, out rhs)
				&& lhs.MatchLdLoc(numVariable) && rhs.MatchLdcI4(1)
				&& IsContainerExit(ifInst.TrueInst, container)
				&& continuation.Instructions[1] is Branch neBranch)
			{
				catchBodyEntry = neBranch.TargetBlock;
				afterCatchExit = ifInst.TrueInst;
				return catchBodyEntry?.Parent == container;
			}

			return false;
		}

		// True when `inst` transfers control out of `container`. Accepts three shapes that all
		// arise from runtime-async lowering:
		//  - direct `Leave` to `container` or any ancestor;
		//  - cross-container `Branch` whose target lives in a strict ancestor of `container` (the
		//    inner-try-catch-inside-outer-try-finally case, where Roslyn emits a single branch that
		//    spans the inner container);
		//  - `Branch` to a one-instruction leave-block in `container` (the indirected canonical
		//    leave-via-helper-block form).
		static bool IsContainerExit(ILInstruction inst, BlockContainer container)
		{
			if (inst is Leave leave)
				return LeaveExitsContainer(leave, container);
			if (inst is Branch br && br.TargetBlock != null)
			{
				var targetContainer = br.TargetBlock.Parent as BlockContainer;
				if (targetContainer == null)
					return false;
				if (targetContainer != container && container.IsDescendantOf(targetContainer))
					return true;
				if (targetContainer == container
					&& br.TargetBlock.Instructions.Count == 1
					&& br.TargetBlock.Instructions[0] is Leave brLeave)
				{
					return LeaveExitsContainer(brLeave, container);
				}
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

		// Recognize the runtime-async lowering of an early return that crosses a try-finally.
		// Roslyn rewrites `return value;` inside a try-block as:
		//     stloc capture(value)
		//     stloc flag(K)
		//     leave-try   (i.e. let the finally run, then exit the try-finally)
		// followed by post-try logic of the form:
		//     if (flag == K) leave outer (capture)
		//
		// Detect that pattern around a TryFinally we just produced and rewrite each capture-set-flag-and-leave
		// site into a direct `leave outer (value)`, then drop the flag/post-flag-check machinery. The leave
		// still passes through the TryFinally so the user's finally body runs before the function returns,
		// which is the intended source-level semantics of `return` from inside a try-finally.
		static bool TryRewriteFlagBasedEarlyReturn(TryFinally tryFinally, ILTransformContext context)
		{
			if (tryFinally.Parent is not Block parentBlock)
				return false;
			if (parentBlock.Parent is not BlockContainer container)
				return false;
			// The TryFinally is followed in parentBlock by either a `br checkBlock` or an inline
			// IfInstruction (when Roslyn merges the flag check into the parent block). Identify the
			// (block, startIndex) pair where the flag-check sequence lives.
			int tryFinallyIdx = tryFinally.ChildIndex;
			if (tryFinallyIdx == parentBlock.Instructions.Count - 1)
				return false;

			Block checkBlock;
			int checkStartIndex;
			bool checkInline;
			if (parentBlock.Instructions[tryFinallyIdx + 1] is Branch brToCheck
				&& brToCheck.TargetBlock?.Parent == container)
			{
				checkBlock = brToCheck.TargetBlock;
				checkStartIndex = 0;
				checkInline = false;
			}
			else if (parentBlock.Instructions[tryFinallyIdx + 1] is IfInstruction)
			{
				checkBlock = parentBlock;
				checkStartIndex = tryFinallyIdx + 1;
				checkInline = true;
			}
			else
			{
				return false;
			}
			// The flag-check sequence is two consecutive instructions: an IfInstruction guarding on
			// `flagVar` and a fall-through. Identify the "early" path (taken when flag == K) and the
			// "normal" path (the other) — each can be either a Branch to a block (we then need to
			// follow it via ResolveEarlyReturnValue), or a direct Leave / Throw.
			if (checkStartIndex + 1 >= checkBlock.Instructions.Count)
				return false;
			if (checkBlock.Instructions[checkStartIndex] is not IfInstruction ifInst)
				return false;

			ILVariable flagVar;
			int targetK;
			ILInstruction earlyAction, normalAction;
			if (ifInst.Condition.MatchCompEquals(out var lhs, out var rhs)
				&& lhs.MatchLdLoc(out flagVar) && rhs.MatchLdcI4(out targetK))
			{
				earlyAction = ifInst.TrueInst;
				normalAction = checkBlock.Instructions[checkStartIndex + 1];
			}
			else if (ifInst.Condition.MatchCompNotEquals(out lhs, out rhs)
				&& lhs.MatchLdLoc(out flagVar) && rhs.MatchLdcI4(out targetK))
			{
				normalAction = ifInst.TrueInst;
				earlyAction = checkBlock.Instructions[checkStartIndex + 1];
			}
			else
			{
				return false;
			}
			if (!flagVar.Type.IsKnownType(KnownTypeCode.Int32))
				return false;

			// Resolve the early-action into a list of instructions to splice into each flag-setter.
			List<ILInstruction> earlyActionTemplate;
			if (earlyAction is Leave earlyLeave && IsLeaveToContainerOrAncestor(earlyLeave, container))
			{
				earlyActionTemplate = new List<ILInstruction> { earlyLeave };
			}
			else if (earlyAction is Branch earlyBranch
				&& earlyBranch.TargetBlock?.Parent == container
				&& TryGetEarlyActionTemplate(earlyBranch.TargetBlock, container, out earlyActionTemplate))
			{
				// templated
			}
			else
			{
				return false;
			}

			// Find flag-setter blocks anywhere inside the try-block (including nested containers).
			// Shape: zero or more capture-forwarding stores, then `stloc flagVar(K); leave tryBlock`.
			// Walking descendants picks up the post-rewrite spliced inner-flag-setters in the nested
			// try-finally case — those live inside the inner try-block container but still leave to
			// the outer try-block (because the splice inlined the outer's flag-set + leave).
			if (tryFinally.TryBlock is not BlockContainer tryBlockContainer)
				return false;
			var flagSetters = new List<Block>();
			foreach (var b in tryBlockContainer.Descendants.OfType<Block>())
			{
				int n = b.Instructions.Count;
				if (n < 2)
					continue;
				if (b.Instructions[n - 2] is not StLoc setStore
					|| !FlagVariableMatches(setStore.Variable, flagVar)
					|| !setStore.Value.MatchLdcI4(targetK))
				{
					continue;
				}
				if (b.Instructions[n - 1] is not Leave leaveFromTry
					|| leaveFromTry.TargetContainer != tryBlockContainer)
				{
					continue;
				}
				flagSetters.Add(b);
			}
			if (flagSetters.Count == 0)
				return false;

			// Verify flagVar is only set in flag-setters and the pre-try init (`stloc flagVar(0)`).
			foreach (var store in flagVar.StoreInstructions.OfType<StLoc>())
			{
				if (flagSetters.Any(fs => fs.Instructions.Contains(store)))
					continue;
				if (store.Parent == parentBlock && store.Value.MatchLdcI4(0))
					continue;
				return false;
			}

			context.StepStartGroup("Reduce runtime-async flag-based early return", tryFinally);

			// Replace each flag-setter's leave-tryBlock with a leave directly to leaveTargetContainer
			// (and, for value-returning sites, with the captured value). The capture-forwarding stores
			// before the flag setter stay; downstream cleanup drops them once the read in `earlyBlock`
			// disappears. Clear the spliced clones' top-level ILRange so each splice site keeps its
			// own sequence points instead of all claiming the source offset of the early-action.
			foreach (var fs in flagSetters)
			{
				int n = fs.Instructions.Count;
				// Drop the stloc flagVar(K) and the trailing leave-tryBlock. Splice in a clone of
				// the early-action template so the flag-setter's capture-forwarding stores flow
				// directly into the early-action's stores + final leave.
				fs.Instructions.RemoveAt(n - 1);
				fs.Instructions.RemoveAt(n - 2);
				foreach (var inst in earlyActionTemplate)
				{
					var spliced = inst.Clone();
					spliced.SetILRange(new Interval());
					fs.Instructions.Add(spliced);
				}
			}

			// Replace the flag-check sequence with the normal-path action. The early-path is now
			// taken by the rewritten flag-setters themselves, so the only remaining successor is
			// the normal path. Clone the normal-action so we don't have to worry about whose tree
			// it currently belongs to (it may be a child of the if-instruction we're tearing down),
			// and clear its ILRange — the clone lives at a different location than the original.
			var normalActionClone = normalAction.Clone();
			normalActionClone.SetILRange(new Interval());
			if (checkInline)
			{
				parentBlock.Instructions.RemoveAt(checkStartIndex + 1);
				parentBlock.Instructions.RemoveAt(checkStartIndex);
				parentBlock.Instructions.Add(normalActionClone);
			}
			else
			{
				checkBlock.Instructions.Clear();
				checkBlock.Instructions.Add(normalActionClone);
			}

			// After the splice each flag-setter exits via a Leave that doesn't return to the
			// surrounding try-block, so any block whose only entry was the now-cleared check-block
			// is now unreachable. Drop those so the HasFlag check below sees the post-rewrite truth.
			tryBlockContainer.SortBlocks(deleteUnreachableBlocks: true);

			// After rewrite, the TryFinally may have unreachable endpoint (all flag-setter sites
			// became Leaves to an ancestor container, so the try-body no longer has a Leave-to-tryBlock).
			// The post-TryFinally instructions in parentBlock are then dead — and leaving them in place
			// would put a non-final unreachable-endpoint instruction in the block, violating the block
			// invariant. Drop everything after the TryFinally in that case.
			if (tryFinally.HasFlag(InstructionFlags.EndPointUnreachable))
			{
				while (parentBlock.Instructions.Count > tryFinally.ChildIndex + 1)
					parentBlock.Instructions.RemoveAt(parentBlock.Instructions.Count - 1);
			}

			// Drop the pre-try `stloc flagVar(0)`.
			for (int i = 0; i < parentBlock.Instructions.Count; i++)
			{
				if (parentBlock.Instructions[i] is StLoc s
					&& FlagVariableMatches(s.Variable, flagVar)
					&& s.Value.MatchLdcI4(0))
				{
					parentBlock.Instructions.RemoveAt(i);
					i--;
				}
			}

			context.StepEndGroup(keepIfEmpty: true);
			return true;
		}

		// True when `candidate` is the same slot/kind/type as `flagVar` — handles SplitVariables splitting
		// the pre-init off from the in-try set.
		static bool FlagVariableMatches(ILVariable candidate, ILVariable flagVar)
		{
			if (candidate == flagVar)
				return true;
			return candidate.Index == flagVar.Index
				&& candidate.Kind == flagVar.Kind
				&& candidate.Type.IsKnownType(KnownTypeCode.Int32);
		}

		// Build a template of instructions to splice into each flag-setter when the early-action is
		// a Branch to a helper block. Follows a chain of "stores + br to next block" until reaching
		// a block whose last instruction is a Leave to `container` or an ancestor. The final template
		// is the concatenation of all the stores encountered, followed by the terminating Leave.
		//
		// Shapes covered:
		//   - [Leave container (value?)]                       — direct leave
		//   - [br leaveBlock]; leaveBlock = [Leave container]  — one-hop forwarding
		//   - [stloc returnVar(...); br leaveBlock]            — indirected value return
		//   - [stloc r(...); br fwd]; fwd = [stloc f(K); Leave outer]
		//                                                      — nested forwarding (sets the
		//     enclosing try-finally's flag and leaves outer, so the next round of this transform
		//     picks it up at that outer level)
		static bool TryGetEarlyActionTemplate(Block earlyBlock, BlockContainer container, out List<ILInstruction> template)
		{
			template = new List<ILInstruction>();
			var visited = new HashSet<Block>();
			var current = earlyBlock;
			while (true)
			{
				if (!visited.Add(current))
				{
					template = null;
					return false;
				}
				if (current.Instructions.Count == 0)
				{
					template = null;
					return false;
				}
				for (int i = 0; i < current.Instructions.Count - 1; i++)
					template.Add(current.Instructions[i]);
				var last = current.Instructions[^1];
				if (last is Leave leave && IsLeaveToContainerOrAncestor(leave, container))
				{
					template.Add(leave);
					return true;
				}
				if (last.MatchBranch(out var next))
				{
					current = next;
					continue;
				}
				template = null;
				return false;
			}
		}

		// Resolve `earlyBlock` to (captureVar?, leaveTargetContainer). Used by the simpler shapes;
		// callers that need to handle the multi-instruction nested-forwarding form use
		// `TryGetEarlyActionTemplate` instead.
		//   - direct leave:           `[Leave container (value?)]`
		//   - direct branch-to-leave: `[br leaveBlock]` where `leaveBlock = [Leave container (void)]`
		//   - indirected:             `[stloc returnVar(ldloc capture); br leaveBlock]` where
		//                             `leaveBlock = [Leave container (ldloc returnVar)]`
		// The target container can be the function body (top-level return) or any ancestor of the
		// surrounding container — break/continue across a try-finally lower to a Leave to the loop
		// or its parent.
		static bool ResolveEarlyReturnValue(Block earlyBlock, BlockContainer container,
			out ILVariable captureVar, out BlockContainer leaveTargetContainer)
		{
			captureVar = null;
			leaveTargetContainer = null;

			// Direct leave: earlyBlock is just `Leave container (value?)`.
			if (earlyBlock.Instructions.Count == 1
				&& earlyBlock.Instructions[0] is Leave directLeave
				&& IsLeaveToContainerOrAncestor(directLeave, container))
			{
				leaveTargetContainer = directLeave.TargetContainer;
				if (directLeave.Value.MatchLdLoc(out captureVar))
					return true;
				if (directLeave.Value.OpCode == OpCode.Nop)
				{
					captureVar = null;
					return true;
				}
				return false;
			}

			// Direct branch-to-leave (void return shape): earlyBlock is `[br leaveBlock]` where
			// `leaveBlock = [Leave container (void)]`.
			if (earlyBlock.Instructions.Count == 1
				&& earlyBlock.Instructions[0].MatchBranch(out var brTarget)
				&& brTarget.Instructions.Count == 1
				&& brTarget.Instructions[0] is Leave brLeave
				&& IsLeaveToContainerOrAncestor(brLeave, container)
				&& brLeave.Value.OpCode == OpCode.Nop)
			{
				leaveTargetContainer = brLeave.TargetContainer;
				captureVar = null;
				return true;
			}

			// Indirected shape: earlyBlock copies the capture into a returnVar and branches to a
			// one-instruction `Leave container (ldloc returnVar)` block.
			if (earlyBlock.Instructions.Count != 2)
				return false;
			if (!earlyBlock.Instructions[0].MatchStLoc(out var returnVar, out var rvValue))
				return false;
			if (!rvValue.MatchLdLoc(out captureVar))
				return false;
			if (!earlyBlock.Instructions[1].MatchBranch(out var leaveBlock))
				return false;
			if (leaveBlock.Instructions.Count != 1)
				return false;
			if (leaveBlock.Instructions[0] is not Leave finalLeave)
				return false;
			if (!IsLeaveToContainerOrAncestor(finalLeave, container))
				return false;
			if (!finalLeave.Value.MatchLdLoc(returnVar))
				return false;
			leaveTargetContainer = finalLeave.TargetContainer;
			return true;
		}

		// True when `leave` exits the immediate `container` or any of its ancestor BlockContainers.
		// Self-leave (TargetContainer == container) is required for the top-level return case where
		// the early-return leaves the function body container itself.
		static bool IsLeaveToContainerOrAncestor(Leave leave, BlockContainer container)
		{
			if (leave.TargetContainer == container)
				return true;
			return container.IsDescendantOf(leave.TargetContainer);
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
