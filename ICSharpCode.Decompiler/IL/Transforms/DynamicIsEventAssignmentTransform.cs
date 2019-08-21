// Copyright (c) 2019 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class DynamicIsEventAssignmentTransform : IStatementTransform
	{
		/// stloc V_1(dynamic.isevent (target))
		/// if (logic.not(ldloc V_1)) Block IL_004a {
		/// 	stloc V_2(dynamic.getmember B(target))
		/// }
		/// [stloc copyOfValue(value)]
		/// if (logic.not(ldloc V_1)) Block IL_0149 {
		/// 	dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(ldloc V_2,  value))
		/// } else Block IL_0151 {
		/// 	dynamic.invokemember.invokespecial.discard add_B(target, value)
		/// }
		/// =>
		/// if (logic.not(dynamic.isevent (target))) Block IL_0149 {
		/// 	dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(dynamic.getmember B(target),  value))
		/// } else Block IL_0151 {
		/// 	dynamic.invokemember.invokespecial.discard add_B(target, value)
		/// }
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!(pos + 3 < block.Instructions.Count && block.Instructions[pos].MatchStLoc(out var flagVar, out var inst) && inst is DynamicIsEventInstruction isEvent))
				return;
			if (!(flagVar.IsSingleDefinition && flagVar.LoadCount == 2))
				return;
			if (!MatchLhsCacheIfInstruction(block.Instructions[pos + 1], flagVar, out var dynamicGetMemberStore))
				return;
			if (!(dynamicGetMemberStore.MatchStLoc(out var getMemberVar, out inst) && inst is DynamicGetMemberInstruction getMemberInst))
				return;
			int offset = 2;
			if (block.Instructions[pos + offset].MatchStLoc(out var valueVariable)
				&& pos + 4 < block.Instructions.Count && valueVariable.IsSingleDefinition && valueVariable.LoadCount == 2
				&& valueVariable.LoadInstructions.All(ld => ld.Parent is DynamicInstruction)) {
				offset++;
			}
			foreach (var descendant in block.Instructions[pos + offset].Descendants) {
				if (!MatchIsEventAssignmentIfInstruction(descendant, isEvent, flagVar, getMemberVar, out var setMemberInst, out var getMemberVarUse, out var isEventConditionUse))
					continue;
				context.Step("DynamicIsEventAssignmentTransform", block.Instructions[pos]);
				// Collapse duplicate condition
				getMemberVarUse.ReplaceWith(getMemberInst);
				isEventConditionUse.ReplaceWith(isEvent);
				block.Instructions.RemoveRange(pos, 2);
				// Reuse ExpressionTransforms
				ExpressionTransforms.TransformDynamicSetMemberInstruction(setMemberInst, context);
				context.RequestRerun();
				break;
			}
		}

		/// <summary>
		/// if (logic.not(ldloc V_1)) Block IL_0149 {
		/// 	dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(ldloc V_2,  value))
		/// } else Block IL_0151 {
		/// 	dynamic.invokemember.invokespecial.discard add_B(target, value)
		/// }
		/// </summary>
		static bool MatchIsEventAssignmentIfInstruction(ILInstruction ifInst, DynamicIsEventInstruction isEvent, ILVariable flagVar, ILVariable getMemberVar,
			out DynamicSetMemberInstruction setMemberInst, out ILInstruction getMemberVarUse, out ILInstruction isEventConditionUse)
		{
			setMemberInst = null;
			getMemberVarUse = null;
			isEventConditionUse = null;
			if (!ifInst.MatchIfInstruction(out var condition, out var trueInst, out var falseInst))
				return false;
			if (MatchFlagEqualsZero(condition, flagVar)) {
				if (!condition.MatchCompEquals(out var left, out _))
					return false;
				isEventConditionUse = left;
			} else if (condition.MatchLdLoc(flagVar)) {
				var tmp = trueInst;
				trueInst = falseInst;
				falseInst = tmp;
				isEventConditionUse = condition;
			} else
				return false;
			setMemberInst = Block.Unwrap(trueInst) as DynamicSetMemberInstruction;
			if (setMemberInst == null)
				return false;
			if (!isEvent.Argument.Match(setMemberInst.Target).Success)
				return false;
			if (!(Block.Unwrap(falseInst) is DynamicInvokeMemberInstruction invokeMemberInst && invokeMemberInst.Arguments.Count == 2))
				return false;
			if (!isEvent.Argument.Match(invokeMemberInst.Arguments[0]).Success)
				return false;
			if (!(setMemberInst.Value is DynamicBinaryOperatorInstruction binOp && binOp.Left.MatchLdLoc(getMemberVar)))
				return false;
			getMemberVarUse = binOp.Left;
			return true;
		}

		/// <summary>
		/// if (logic.not(ldloc V_1)) Block IL_004a {
		/// 	stloc V_2(dynamic.getmember B(target))
		/// }
		/// </summary>
		static bool MatchLhsCacheIfInstruction(ILInstruction ifInst, ILVariable flagVar, out StLoc cacheStore)
		{
			cacheStore = null;
			if (!ifInst.MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!MatchFlagEqualsZero(condition, flagVar))
				return false;
			cacheStore = Block.Unwrap(trueInst) as StLoc;
			return cacheStore != null;
		}

		static bool MatchFlagEqualsZero(ILInstruction condition, ILVariable flagVar)
		{
			return condition.MatchCompEquals(out var left, out var right)
				&& left.MatchLdLoc(flagVar)
				&& right.MatchLdcI4(0);
		}
	}
}
