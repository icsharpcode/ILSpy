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

using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class DynamicIsEventAssignmentTransform : IStatementTransform
	{
		// A dynamic 'd.Event += b' lowers to a runtime is-event check that dispatches between the event
		// accessor and a compound assignment. That diamond reaches this transform either as an if/else or,
		// when the optimizer returns the result, as two leaves. Each shape has its own collapse method.
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (TryCollapseIfElseDiamond(block, pos, context))
				return;
			TryCollapseReturnForm(block, pos, context);
		}

		/// <summary>
		/// stloc V_1(dynamic.isevent (target))
		/// if (logic.not(ldloc V_1)) Block IL_004a {
		/// 	stloc V_2(dynamic.getmember B(target))
		/// }
		/// [stloc copyOfValue(value)]
		/// if (logic.not(ldloc V_1)) Block IL_0149 {
		/// 	dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(ldloc V_2, value))
		/// } else Block IL_0151 {
		/// 	dynamic.invokemember.invokespecial.discard add_B(target, value)
		/// }
		/// =>
		/// dynamic.compound.op (dynamic.getmember B(target), value)
		/// </summary>
		/// <remarks>
		/// Only the cached lowering reaches here; ExpressionTransforms.TransformDynamicAddAssignOrRemoveAssign
		/// collapses the uncached if/else directly, where dynamic.isevent is already the condition. When the
		/// result is used the same diamond is nested as a value-if inside the consuming expression, which the
		/// Descendants walk still finds.
		/// </remarks>
		static bool TryCollapseIfElseDiamond(Block block, int pos, StatementTransformContext context)
		{
			if (!MatchCachePrefix(block, pos, out var isEvent, out var flagVar, out var getMemberVar, out var getMemberInst, out var diamondPos, out var valueVariable, out var value))
				return false;
			// Descendants is a post-order walk including the node itself, so it finds the diamond whether it
			// is the top-level statement or nested inside a call argument or leave value.
			foreach (var descendant in block.Instructions[diamondPos].Descendants)
			{
				if (!MatchIsEventAssignmentIfInstruction(descendant, isEvent, flagVar, getMemberVar, out var setMemberInst, out var getMemberVarUse, out var isEventConditionUse))
					continue;
				context.Step("DynamicIsEventAssignmentTransform", block.Instructions[pos]);
				// Collapse duplicate condition
				getMemberVarUse.ReplaceWith(getMemberInst);
				isEventConditionUse.ReplaceWith(isEvent);
				block.Instructions.RemoveRange(pos, 2);
				// Reuse ExpressionTransforms
				ExpressionTransforms.TransformDynamicSetMemberInstruction(setMemberInst, context);
				// The match guarantees the collapse; if it ever failed the inlined isevent would leak.
				if (!ExpressionTransforms.TransformDynamicAddAssignOrRemoveAssign((IfInstruction)descendant, context))
				{
					Debug.Fail("dynamic is-event assignment matched but could not be collapsed to a compound assignment");
				}
				InlineCopyOfValue(block, pos, valueVariable, value);
				context.EndStep(block.Instructions[pos]);
				// Re-run from the collapsed statement so the following transforms visit the new compound.
				context.RequestRerun(pos);
				return true;
			}
			return false;
		}

		/// <summary>
		/// The two-leaves form the optimizer emits instead of an if/else when the result is returned.
		/// Cached (getmember still behind the flag cache):
		/// stloc V_1(dynamic.isevent (target))
		/// if (logic.not(ldloc V_1)) Block { stloc V_2(dynamic.getmember B(target)) }
		/// [stloc copyOfValue(value)]
		/// if (ldloc V_1) Block { leave X(dynamic.invokemember add_B(target, value)) }
		/// leave X(dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(ldloc V_2, value)))
		/// =>
		/// leave X(dynamic.compound.op (dynamic.getmember B(target), value))
		///
		/// Uncached (dynamic.isevent inlined into the condition, compound leave already collapsed, so only
		/// the redundant is-event branch remains to drop):
		/// if (dynamic.isevent (target)) Block { leave X(dynamic.invokemember add_B(target, value)) }
		/// leave X(dynamic.compound.op AddAssign(dynamic.getmember B(target), value))
		/// =>
		/// leave X(dynamic.compound.op AddAssign(dynamic.getmember B(target), value))
		/// </summary>
		static void TryCollapseReturnForm(Block block, int pos, StatementTransformContext context)
		{
			bool cached = MatchCachePrefix(block, pos, out var isEvent, out var flagVar, out var getMemberVar, out var getMemberInst, out var diamondPos, out var valueVariable, out var value);
			DynamicSetMemberInstruction returnSetMember = null;
			ILInstruction returnGetMemberUse = null;
			if (!(diamondPos + 1 < block.Instructions.Count && (cached
					? MatchIsEventReturnAssignment(block.Instructions[diamondPos], block.Instructions[diamondPos + 1], isEvent, flagVar, getMemberVar, out returnSetMember, out returnGetMemberUse)
					: MatchUncachedIsEventReturnAssignment(block.Instructions[diamondPos], block.Instructions[diamondPos + 1]))))
				return;
			context.Step("DynamicIsEventAssignmentTransform (return form)", block.Instructions[pos]);
			// Drop the 'if (isevent) leave add' branch; the compound leave stays and becomes the result.
			block.Instructions.RemoveAt(diamondPos);
			if (cached)
			{
				returnGetMemberUse.ReplaceWith(getMemberInst);
				block.Instructions.RemoveRange(pos, 2);
				ExpressionTransforms.TransformDynamicSetMemberInstruction(returnSetMember, context);
				InlineCopyOfValue(block, pos, valueVariable, value);
			}
			context.EndStep(block.Instructions[pos]);
			context.RequestRerun(pos);
		}

		/// <summary>
		/// The optional flag/getmember cache prefix of the cached lowering:
		/// stloc V_1(dynamic.isevent (target))
		/// if (logic.not(ldloc V_1)) Block { stloc V_2(dynamic.getmember B(target)) }
		/// [stloc copyOfValue(value)]
		/// Reports <paramref name="diamondPos"/> = the index just past the prefix where the diamond starts.
		/// Returns false with <paramref name="diamondPos"/> = <paramref name="pos"/> for the uncached
		/// lowering, which has no prefix.
		/// </summary>
		static bool MatchCachePrefix(Block block, int pos, out DynamicIsEventInstruction isEvent,
			out ILVariable flagVar, out ILVariable getMemberVar, out DynamicGetMemberInstruction getMemberInst,
			out int diamondPos, out ILVariable valueVariable, out ILInstruction value)
		{
			isEvent = null;
			flagVar = null;
			getMemberVar = null;
			getMemberInst = null;
			diamondPos = pos;
			valueVariable = null;
			value = null;
			if (!(pos + 3 < block.Instructions.Count && block.Instructions[pos].MatchStLoc(out flagVar, out var inst) && inst is DynamicIsEventInstruction ev))
				return false;
			isEvent = ev;
			if (!(flagVar.IsSingleDefinition && flagVar.LoadCount == 2))
				return false;
			if (!MatchLhsCacheIfInstruction(block.Instructions[pos + 1], flagVar, out var cacheStore))
				return false;
			if (!(cacheStore.MatchStLoc(out getMemberVar, out inst) && inst is DynamicGetMemberInstruction gm))
				return false;
			getMemberInst = gm;
			diamondPos = pos + 2;
			// [stloc copyOfValue(value)]: an optional temporary loaded once per diamond branch. The entry
			// guard already ensures pos + 3 is in range.
			if (block.Instructions[diamondPos].MatchStLoc(out valueVariable, out value)
				&& valueVariable.IsSingleDefinition && valueVariable.LoadCount == 2
				&& valueVariable.LoadInstructions.All(ld => ld.Parent is DynamicInstruction))
			{
				diamondPos++;
			}
			else
			{
				valueVariable = null;
				value = null;
			}
			return true;
		}

		/// <summary>
		/// After the collapse the copy-of-value temporary is loaded exactly once (its use in the dropped
		/// add branch is gone), so inline it and remove its store, which sits at <paramref name="pos"/>.
		/// </summary>
		static void InlineCopyOfValue(Block block, int pos, ILVariable valueVariable, ILInstruction value)
		{
			if (valueVariable is { StoreCount: 1, AddressCount: 0, LoadCount: 1 } && value != null)
			{
				valueVariable.LoadInstructions[0].ReplaceWith(value);
				block.Instructions.RemoveAt(pos);
			}
		}

		/// <summary>
		/// dynamic.invokemember add_B/remove_B(target, value): the event accessor paired with a compound,
		/// its name being 'add_'/'remove_' + member for the operation and its two arguments the same target
		/// and value as the compound.
		/// </summary>
		static bool MatchEventAccessor(DynamicInvokeMemberInstruction addInvoke, ILInstruction target, ILInstruction value, ExpressionType operation, string memberName)
		{
			string accessor = operation switch {
				ExpressionType.AddAssign => "add_" + memberName,
				ExpressionType.SubtractAssign => "remove_" + memberName,
				_ => null
			};
			return addInvoke.Name == accessor
				&& addInvoke.Arguments[0].Match(target).Success
				&& addInvoke.Arguments[1].Match(value).Success;
		}

		/// <summary>
		/// The two-leaves diamond shared by the cached and uncached return lowerings:
		/// if (condition) Block { leave X(dynamic.invokemember add_B(target, value)) }
		/// leave X(compound)
		/// </summary>
		static bool MatchIsEventReturnDiamond(ILInstruction ifInst, ILInstruction leaveInst,
			out ILInstruction condition, out DynamicInvokeMemberInstruction addInvoke, out ILInstruction compoundValue)
		{
			condition = null;
			addInvoke = null;
			compoundValue = null;
			if (!ifInst.MatchIfInstruction(out condition, out var trueInst))
				return false;
			if (!Block.Unwrap(trueInst).MatchLeave(out var target, out var addValue))
				return false;
			if (!leaveInst.MatchLeave(target, out compoundValue))
				return false;
			addInvoke = addValue as DynamicInvokeMemberInstruction;
			return addInvoke != null && addInvoke.Arguments.Count == 2;
		}

		/// <summary>
		/// if (dynamic.isevent (target)) Block { leave X(dynamic.invokemember add_B(target, value)) }
		/// leave X(dynamic.compound.op AddAssign(dynamic.getmember B(target), value))
		/// </summary>
		static bool MatchUncachedIsEventReturnAssignment(ILInstruction ifInst, ILInstruction leaveInst)
		{
			if (!MatchIsEventReturnDiamond(ifInst, leaveInst, out var condition, out var addInvoke, out var compoundValue))
				return false;
			if (!(condition is DynamicIsEventInstruction isEvent))
				return false;
			if (!(compoundValue is DynamicCompoundAssign compound && compound.Target is DynamicGetMemberInstruction getMember))
				return false;
			return isEvent.Argument.Match(getMember.Target).Success
				&& MatchEventAccessor(addInvoke, getMember.Target, compound.Value, compound.Operation, getMember.Name);
		}

		/// <summary>
		/// if (ldloc V_1) Block { leave X(dynamic.invokemember add_B(target, value)) }
		/// leave X(dynamic.setmember.compound B(target, dynamic.binary.operator AddAssign(ldloc V_2, value)))
		/// </summary>
		/// <remarks>
		/// <paramref name="getMemberVarUse"/> is the 'ldloc V_2' cache load the caller rewrites to the real
		/// dynamic.getmember before the setmember collapses to a compound assignment.
		/// </remarks>
		static bool MatchIsEventReturnAssignment(ILInstruction ifInst, ILInstruction leaveInst,
			DynamicIsEventInstruction isEvent, ILVariable flagVar, ILVariable getMemberVar,
			out DynamicSetMemberInstruction setMemberInst, out ILInstruction getMemberVarUse)
		{
			setMemberInst = null;
			getMemberVarUse = null;
			return MatchIsEventReturnDiamond(ifInst, leaveInst, out var condition, out var addInvoke, out var compoundValue)
				&& condition.MatchLdLoc(flagVar)
				&& MatchCachedEventCompound(compoundValue, addInvoke, isEvent, getMemberVar, out setMemberInst, out getMemberVarUse);
		}

		/// <summary>
		/// dynamic.setmember.compound B(target, dynamic.binary.operator op(ldloc V_2, value))
		/// paired with dynamic.invokemember add_B/remove_B(target, value).
		/// V_2 is the getmember cache load, returned as <paramref name="getMemberVarUse"/>.
		/// </summary>
		static bool MatchCachedEventCompound(ILInstruction compound, DynamicInvokeMemberInstruction addInvoke,
			DynamicIsEventInstruction isEvent, ILVariable getMemberVar,
			out DynamicSetMemberInstruction setMemberInst, out ILInstruction getMemberVarUse)
		{
			setMemberInst = null;
			getMemberVarUse = null;
			if (!(compound is DynamicSetMemberInstruction setMember) || !isEvent.Argument.Match(setMember.Target).Success)
				return false;
			if (!(setMember.Value is DynamicBinaryOperatorInstruction binOp && binOp.Left.MatchLdLoc(getMemberVar)))
				return false;
			if (!MatchEventAccessor(addInvoke, setMember.Target, binOp.Right, binOp.Operation, setMember.Name))
				return false;
			setMemberInst = setMember;
			getMemberVarUse = binOp.Left;
			return true;
		}

		/// <summary>
		/// Matches both the expression and statement form of:
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
			if (MatchFlagEqualsZero(condition, flagVar))
			{
				condition.MatchCompEquals(out isEventConditionUse, out _);
			}
			else if (condition.MatchLdLoc(flagVar))
			{
				(trueInst, falseInst) = (falseInst, trueInst);
				isEventConditionUse = condition;
			}
			else
				return false;
			// trueInst is now the compound branch, falseInst the add/remove accessor invoke.
			return Block.Unwrap(falseInst) is DynamicInvokeMemberInstruction addInvoke && addInvoke.Arguments.Count == 2
				&& MatchCachedEventCompound(Block.Unwrap(trueInst), addInvoke, isEvent, getMemberVar, out setMemberInst, out getMemberVarUse);
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
