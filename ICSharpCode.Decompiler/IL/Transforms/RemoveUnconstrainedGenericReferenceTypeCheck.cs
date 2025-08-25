// Copyright (c) 2025 Siegfried Pammer
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

using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	///	[stloc address(...)]
	///	stloc temp(default.value ``0)
	///	if (comp.o(ldloc temp == ldnull)) Block IL_002a {
	///		stloc temp(ldobj ``0(ldloc address))
	///		stloc address(ldloca temp)
	///	}
	///	stloc V_i(expr_i)
	///	...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), ldloc V_i ...))...
	///	
	///=>
	///
	///	[stloc address(...)]
	/// stloc address(ldobj.if.ref(ldloc address))
	///	stloc V_i(expr_i)
	///	...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), ldloc V_i ...))...
	///	
	/// Then ldobj.if.ref in the call is redundant because any object reference was already loaded into an immutable temporary.
	/// So we can removed and inlining of the arguments (V_i) becomes possible.
	/// 
	/// Finally the newly created ldobj.if.ref is inlined into the place where the old ldobj.if.ref was.
	/// 
	/// =>
	/// 
	///	[stloc address(...)]
	///	...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), expr_i ...))...
	/// 
	/// </summary>
	class RemoveUnconstrainedGenericReferenceTypeCheck : IStatementTransform
	{
		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			int startPos = pos;
			// stloc temp(default.value ``0)
			if (!block.Instructions[pos].MatchStLoc(out var temp, out var defaultValue))
				return;
			if (!defaultValue.MatchDefaultValue(out var type))
				return;
			if (temp.StoreCount != 2 || temp.LoadCount != 1 || temp.AddressCount != 1)
				return;
			pos++;
			// if (comp.o(ldloc temp == ldnull)) Block IL_002a {
			// 	stloc temp(ldobj ``0(ldloc address))
			// 	stloc address(ldloca temp)
			// }
			if (block.Instructions.ElementAtOrDefault(pos) is not IfInstruction ifInst)
				return;
			if (!ifInst.Condition.MatchCompEqualsNull(out var tempLoadForNullCheck))
				return;
			if (!tempLoadForNullCheck.MatchLdLoc(temp))
				return;
			if (ifInst.TrueInst is not Block dereferenceBlock)
				return;
			if (ifInst.FalseInst is not Nop)
				return;
			if (dereferenceBlock.Instructions is not [StLoc tempStore, StLoc addressReassign])
				return;
			if (tempStore.Variable != temp)
				return;
			if (!tempStore.Value.MatchLdObj(out var addressLoadForLdObj, type))
				return;
			if (!addressLoadForLdObj.MatchLdLoc(addressReassign.Variable))
				return;
			if (!addressReassign.Value.MatchLdLoca(temp))
				return;
			var address = addressReassign.Variable;
			if (address.StoreCount != 2 || address.LoadCount != 2 || address.AddressCount != 0)
				return;
			pos++;
			// pos now is the first store to V_i
			// ...(constrained[``0].callvirt Method(ldobj.if.ref ``0(ldloc address), ldloc V_i ...))...
			var callTarget = address.LoadInstructions.Single(l => addressLoadForLdObj != l);
			if (callTarget.Parent is not LdObjIfRef { Parent: CallVirt call } ldobjIfRef)
				return;
			if (call.Arguments.Count == 0 || call.Arguments[0] != ldobjIfRef || !type.Equals(call.ConstrainedTo))
				return;
			ILInstruction containingStmt = call;
			while (containingStmt.Parent != block)
			{
				if (containingStmt.Parent == null)
					return;
				containingStmt = containingStmt.Parent;
			}
			if (containingStmt.ChildIndex < pos)
				return;
			// check if we can inline all temporaries used in the call:
			int temporaryInitIndex = containingStmt.ChildIndex - 1;
			List<(ILInstruction, ILInstruction)> replacements = new();
			for (int argIndex = call.Arguments.Count - 1; argIndex > 0 && temporaryInitIndex >= pos; argIndex--)
			{
				var argument = call.Arguments[argIndex];
				switch (argument)
				{
					case LdLoc load:
						if (block.Instructions[temporaryInitIndex].MatchStLoc(load.Variable, out var expr) && ILInlining.VariableCanBeUsedForInlining(load.Variable))
						{
							if (!ILInlining.CanMoveIntoCallVirt(expr, containingStmt, call, argument))
							{
								return;
							}
							replacements.Add((argument, expr));
							temporaryInitIndex--;
						}
						break;
				}
			}
			// all stores to V_i processed?
			if (temporaryInitIndex != pos - 1)
			{
				return;
			}
			context.Step("RemoveUnconstrainedGenericReferenceTypeCheck", block.Instructions[startPos]);
			foreach (var (argument, expr) in replacements)
			{
				argument.ReplaceWith(expr);
			}
			block.Instructions.RemoveRange(startPos, containingStmt.ChildIndex - startPos);
		}
	}
}