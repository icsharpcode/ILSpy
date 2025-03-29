// Copyright (c) 2021 Daniel Grunwald, Siegfried Pammer
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

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class RemoveInfeasiblePathTransform : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			foreach (var container in function.Descendants.OfType<BlockContainer>())
			{
				bool changed = false;
				foreach (var block in container.Blocks)
				{
					changed |= RemoveInfeasiblePath(block, context);
					changed |= RemoveUnconstrainedGenericReferenceTypeCheck(block, context);
				}

				if (changed)
				{
					container.SortBlocks(deleteUnreachableBlocks: true);
				}
			}
		}

		/// <summary>
		/// Block IL_0018 (incoming: *) {
		///     stloc s(ldc.i4 1)
		///     br IL_0019
		/// }
		/// 
		/// Block IL_0019 (incoming: > 1) {
		///     if (logic.not(ldloc s)) br IL_0027
		///     br IL_001d
		/// }
		/// 
		/// replace br IL_0019 with br IL_0027
		/// </summary>
		private bool RemoveInfeasiblePath(Block block, ILTransformContext context)
		{
			if (!MatchBlock1(block, out var s, out int value, out var br))
				return false;
			if (!MatchBlock2(br.TargetBlock, s, value, out var exitInst))
				return false;
			context.Step("RemoveInfeasiblePath", br);
			br.ReplaceWith(exitInst.Clone());
			s.RemoveIfRedundant = true;
			return true;
		}

		// Block IL_0018 (incoming: *) {
		//  stloc s(ldc.i4 1)
		//  br IL_0019
		// }
		private bool MatchBlock1(Block block, [NotNullWhen(true)] out ILVariable? variable, out int constantValue, [NotNullWhen(true)] out Branch? branch)
		{
			variable = null;
			constantValue = 0;
			branch = null;
			if (block.Instructions.Count != 2)
				return false;
			if (block.Instructions[0] is not StLoc {
				Variable: { Kind: VariableKind.StackSlot } s,
				Value: LdcI4 { Value: 0 or 1 } valueInst
			})
			{
				return false;
			}
			if (block.Instructions[1] is not Branch br)
				return false;
			variable = s;
			constantValue = valueInst.Value;
			branch = br;
			return true;
		}

		// Block IL_0019 (incoming: > 1) {
		//     if (logic.not(ldloc s)) br IL_0027
		//     br IL_001d
		// }
		bool MatchBlock2(Block block, ILVariable s, int constantValue, [NotNullWhen(true)] out ILInstruction? exitInst)
		{
			exitInst = null;
			if (block.Instructions.Count != 2)
				return false;
			if (block.IncomingEdgeCount <= 1)
				return false;
			if (!block.MatchIfAtEndOfBlock(out var load, out var trueInst, out var falseInst))
				return false;
			if (!load.MatchLdLoc(s))
				return false;
			exitInst = constantValue != 0 ? trueInst : falseInst;
			return exitInst is Branch or Leave { Value: Nop };
		}

		/// <summary>
		/// Block entryPoint (incoming: _) {
		///     [...]
		/// 	stloc S_0(...)
		/// 	stobj ``0(ldloca V_0, default.value ``0)
		/// 	if (comp.o(box ``0(ldloc V_0) != ldnull)) br invocationBlock
		/// 	br dereferenceBlock
		/// }
		/// 
		/// Block dereferenceBlock (incoming: 1) {
		/// 	stloc V_0(ldobj ``0(ldloc S_0))
		/// 	stloc S_0(ldloca V_0)
		/// 	br invocationBlock
		/// }
		/// 
		/// Block invocationBlock (incoming: 2) {
		///		[...]
		/// 	... (constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc S_0), ...))
		/// }
		/// </summary>
		private bool RemoveUnconstrainedGenericReferenceTypeCheck(Block entryPoint, ILTransformContext context)
		{
			// if (comp.o(box ``0(ldloc temporary) != ldnull)) br invocationBlock
			// br dereferenceBlock
			if (!entryPoint.MatchIfAtEndOfBlock(out var condition, out var invocationBlockBranch, out var dereferenceBlockBranch))
			{
				return false;
			}
			if (!condition.MatchCompNotEqualsNull(out var arg))
			{
				return false;
			}
			if (!arg.MatchBox(out arg, out var type))
			{
				return false;
			}
			if (!arg.MatchLdLoc(out var temp) || temp.StoreCount != 2 || temp.AddressCount != 2)
			{
				return false;
			}
			// stobj ``0(ldloca V_0, default.value ``0)
			var store = entryPoint.Instructions.ElementAtOrDefault(entryPoint.Instructions.Count - 3);
			if (store == null || !store.MatchStObj(out var target, out var value, out var storeType))
			{
				return false;
			}
			if (!target.MatchLdLoca(temp) || !value.MatchDefaultValue(out var defaultValueType))
			{
				return false;
			}
			if (!defaultValueType.Equals(storeType) || !storeType.Equals(type))
			{
				return false;
			}
			// stloc S_0(...)
			store = entryPoint.Instructions.ElementAtOrDefault(entryPoint.Instructions.Count - 4);
			if (store == null || !store.MatchStLoc(out var stackSlot, out var thisValue))
			{
				return false;
			}
			// check dereferenceBlock
			if (!dereferenceBlockBranch.MatchBranch(out var dereferenceBlock) || !invocationBlockBranch.MatchBranch(out var invocationBlock))
			{
				return false;
			}
			if (invocationBlock.IncomingEdgeCount != 2)
			{
				return false;
			}
			if (dereferenceBlock.IncomingEdgeCount != 1)
			{
				return false;
			}

			// stloc V_0(ldobj ``0(ldloc S_0))
			// stloc S_0(ldloca V_0)
			// br invocationBlock
			if (dereferenceBlock.Instructions is not [StLoc deref, StLoc addressLoad, Branch br])
			{
				return false;
			}
			if (deref.Variable != temp || addressLoad.Variable != stackSlot)
			{
				return false;
			}
			if (!deref.Value.MatchLdObj(out var stackSlotTarget, out var loadType))
			{
				return false;
			}
			if (!stackSlotTarget.MatchLdLoc(stackSlot) || !loadType.Equals(type))
			{
				return false;
			}
			if (!addressLoad.Value.MatchLdLoca(temp))
			{
				return false;
			}
			if (br?.TargetBlock != invocationBlock)
			{
				return false;
			}
			// ... (constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc S_0), ...))
			if (stackSlot.StoreCount != 2 || stackSlot.LoadCount != 2 || stackSlot.AddressCount != 0)
			{
				return false;
			}
			var callTarget = stackSlot.LoadInstructions.SingleOrDefault(l => stackSlotTarget != l);
			if (callTarget?.Parent is not LdObjIfRef { Parent: CallVirt call } ldobjIfRef)
			{
				return false;
			}
			if (call.Arguments.Count == 0 || call.Arguments[0] != ldobjIfRef || !storeType.Equals(call.ConstrainedTo))
			{
				return false;
			}
			context.Step("RemoveUnconstrainedGenericReferenceTypeCheck", store);
			ldobjIfRef.ImplicitDeference = true;
			entryPoint.Instructions.RemoveRange(entryPoint.Instructions.Count - 3, 3);
			entryPoint.Instructions.Add(invocationBlockBranch);
			return true;
		}
	}
}
