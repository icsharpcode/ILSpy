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
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform for constructing the NullCoalescingInstruction (if.notnull(a,b), or in C#: ??)
	/// Note that this transform only handles the case where a,b are reference types.
	/// 
	/// The ?? operator for nullable value types is handled by NullableLiftingTransform.
	/// </summary>
	class NullCoalescingTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!TransformRefTypes(block, pos, context)) {
				if (!TransformThrowExpressionValueTypes(block, pos, context)) {
					TransformThrowExpressionOnAddress(block, pos, context);
				}
			}
		}

		/// <summary>
		/// Handles NullCoalescingInstruction case 1: reference types.
		/// </summary>
		bool TransformRefTypes(Block block, int pos, StatementTransformContext context)
		{
			if (!(block.Instructions[pos] is StLoc stloc))
				return false;
			if (stloc.Variable.Kind != VariableKind.StackSlot)
				return false;
			if (!block.Instructions[pos + 1].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!(condition.MatchCompEquals(out var left, out var right) && left.MatchLdLoc(stloc.Variable) && right.MatchLdNull()))
				return false;
			trueInst = Block.Unwrap(trueInst);
			// stloc s(valueInst)
			// if (comp(ldloc s == ldnull)) {
			//		stloc s(fallbackInst)
			// }
			// =>
			// stloc s(if.notnull(valueInst, fallbackInst))
			if (trueInst.MatchStLoc(stloc.Variable, out var fallbackValue)) {
				context.Step("NullCoalescingTransform: simple (reference types)", stloc);
				stloc.Value = new NullCoalescingInstruction(NullCoalescingKind.Ref, stloc.Value, fallbackValue);
				block.Instructions.RemoveAt(pos + 1); // remove if instruction
				ILInlining.InlineOneIfPossible(block, pos, InliningOptions.None, context);
				return true;
			}
			// sometimes the compiler generates:
			// stloc s(valueInst)
			// if (comp(ldloc s == ldnull)) {
			//		stloc v(fallbackInst)
			//      stloc s(ldloc v)
			// }
			// v must be single-assign and single-use.
			if (trueInst is Block trueBlock && trueBlock.Instructions.Count == 2
				&& trueBlock.Instructions[0].MatchStLoc(out var temporary, out fallbackValue)
				&& temporary.IsSingleDefinition && temporary.LoadCount == 1
				&& trueBlock.Instructions[1].MatchStLoc(stloc.Variable, out var useOfTemporary)
				&& useOfTemporary.MatchLdLoc(temporary)) {
				context.Step("NullCoalescingTransform: with temporary variable (reference types)", stloc);
				stloc.Value = new NullCoalescingInstruction(NullCoalescingKind.Ref, stloc.Value, fallbackValue);
				block.Instructions.RemoveAt(pos + 1); // remove if instruction
				ILInlining.InlineOneIfPossible(block, pos, InliningOptions.None, context);
				return true;
			}
			// stloc obj(valueInst)
			// if (comp(ldloc obj == ldnull)) {
			//		throw(...)
			// }
			// =>
			// stloc obj(if.notnull(valueInst, throw(...)))
			if (context.Settings.ThrowExpressions && trueInst is Throw throwInst) {
				context.Step("NullCoalescingTransform (reference types + throw expression)", stloc);
				throwInst.resultType = StackType.O;
				stloc.Value = new NullCoalescingInstruction(NullCoalescingKind.Ref, stloc.Value, throwInst);
				block.Instructions.RemoveAt(pos + 1); // remove if instruction
				ILInlining.InlineOneIfPossible(block, pos, InliningOptions.None, context);
				return true;
			}
			return false;
		}

		delegate bool PatternMatcher(ILInstruction input, out ILInstruction output);

		/// <summary>
		/// stloc v(value)
		/// if (logic.not(call get_HasValue(ldloca v))) throw(...)
		/// ... Call(arg1, arg2, call GetValueOrDefault(ldloca v), arg4) ...
		/// =>
		/// ... Call(arg1, arg2, if.notnull(value, throw(...)), arg4) ...
		/// 
		/// -or-
		/// 
		/// stloc v(value)
		/// stloc s(ldloca v)
		/// if (logic.not(call get_HasValue(ldloc s))) throw(...)
		/// ... Call(arg1, arg2, call GetValueOrDefault(ldloc s), arg4) ...
		/// =>
		/// ... Call(arg1, arg2, if.notnull(value, throw(...)), arg4) ...
		/// </summary>
		bool TransformThrowExpressionValueTypes(Block block, int pos, StatementTransformContext context)
		{
			if (pos + 2 >= block.Instructions.Count)
				return false;
			if (!(block.Instructions[pos] is StLoc stloc))
				return false;
			int offset = 1;
			ILVariable v = stloc.Variable;
			// alternative pattern using stack slot containing address of v
			if (block.Instructions[pos + offset] is StLoc addrCopyStore
				&& addrCopyStore.Variable.Kind == VariableKind.StackSlot
				&& addrCopyStore.Value.MatchLdLoca(v)
				&& pos + 3 < block.Instructions.Count)
			{
				offset++;
				// v turns into s in the pattern above.
				v = addrCopyStore.Variable;
				if (!(v.StoreCount == 1 && v.LoadCount == 2 && v.AddressCount == 0))
					return false;
			} else {
				if (!(v.StoreCount == 1 && v.LoadCount == 0 && v.AddressCount == 2))
					return false;
			}
			if (!block.Instructions[pos + offset].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!(Block.Unwrap(trueInst) is Throw throwInst))
				return false;
			if (!condition.MatchLogicNot(out var arg))
				return false;
			if (!MatchNullableCall(arg, NullableLiftingTransform.MatchHasValueCall))
				return false;
			var throwInstParent = throwInst.Parent;
			var throwInstChildIndex = throwInst.ChildIndex;
			var nullCoalescingWithThrow = new NullCoalescingInstruction(
				NullCoalescingKind.NullableWithValueFallback,
				stloc.Value,
				throwInst);
			var resultType = NullableType.GetUnderlyingType(v.Type).GetStackType();
			nullCoalescingWithThrow.UnderlyingResultType = resultType;
			var result = ILInlining.FindLoadInNext(block.Instructions[pos + offset + 1], v, nullCoalescingWithThrow, InliningOptions.None);
			if (result.Type == ILInlining.FindResultType.Found
				&& MatchNullableCall(result.LoadInst.Parent, NullableLiftingTransform.MatchGetValueOrDefault))
			{
				context.Step("NullCoalescingTransform (value types + throw expression)", stloc);
				throwInst.resultType = resultType;
				result.LoadInst.Parent.ReplaceWith(nullCoalescingWithThrow);
				block.Instructions.RemoveRange(pos, offset + 1); // remove store(s) and if instruction
				return true;
			} else {
				// reset the primary position (see remarks on ILInstruction.Parent)
				stloc.Value = stloc.Value;
				var children = throwInstParent.Children;
				children[throwInstChildIndex] = throwInst;
				return false;
			}

			bool MatchNullableCall(ILInstruction input, PatternMatcher matcher)
			{
				if (!matcher(input, out var loadInst))
					return false;
				if (offset == 1) { // Pattern 1
					if (!loadInst.MatchLdLoca(v))
						return false;
				} else {
					if (!loadInst.MatchLdLoc(v))
						return false;
				}
				return true;
			}
		}

		/// <summary>
		/// stloc s(addressOfValue)
		/// if (logic.not(call get_HasValue(ldloc s))) throw(...)
		/// ... Call(arg1, arg2, call GetValueOrDefault(ldloc s), arg4) ...
		/// =>
		/// ... Call(arg1, arg2, if.notnull(value, throw(...)), arg4) ...
		/// </summary>
		bool TransformThrowExpressionOnAddress(Block block, int pos, StatementTransformContext context)
		{
			if (pos + 2 >= block.Instructions.Count)
				return false;
			if (!(block.Instructions[pos] is StLoc stloc))
				return false;
			var s = stloc.Variable;
			if (s.Kind != VariableKind.StackSlot)
				return false;
			if (!(s.StoreCount == 1 && s.LoadCount == 2 && s.AddressCount == 0))
				return false;
			if (!(s.Type is ByReferenceType byRef && byRef.ElementType.IsReferenceType == false))
				return false;
			if (!block.Instructions[pos + 1].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!(Block.Unwrap(trueInst) is Throw throwInst))
				return false;
			if (!condition.MatchLogicNot(out var arg))
				return false;
			if (!MatchNullableCall(arg, NullableLiftingTransform.MatchHasValueCall))
				return false;
			var throwInstParent = throwInst.Parent;
			var throwInstChildIndex = throwInst.ChildIndex;
			var ldobj = new LdObj(stloc.Value, byRef.ElementType);
			var nullCoalescingWithThrow = new NullCoalescingInstruction(
				NullCoalescingKind.NullableWithValueFallback,
				ldobj,
				throwInst);
			var resultType = NullableType.GetUnderlyingType(byRef.ElementType).GetStackType();
			nullCoalescingWithThrow.UnderlyingResultType = resultType;
			var result = ILInlining.FindLoadInNext(block.Instructions[pos + 2], s, nullCoalescingWithThrow, InliningOptions.None);
			if (result.Type == ILInlining.FindResultType.Found
				&& MatchNullableCall(result.LoadInst.Parent, NullableLiftingTransform.MatchGetValueOrDefault)) {
				context.Step("NullCoalescingTransform (address + throw expression)", stloc);
				throwInst.resultType = resultType;
				result.LoadInst.Parent.ReplaceWith(nullCoalescingWithThrow);
				block.Instructions.RemoveRange(pos, 2); // remove store and if instruction
				return true;
			} else {
				// reset the primary position (see remarks on ILInstruction.Parent)
				stloc.Value = stloc.Value;
				var children = throwInstParent.Children;
				children[throwInstChildIndex] = throwInst;
				return false;
			}

			bool MatchNullableCall(ILInstruction input, PatternMatcher matcher)
			{
				if (!matcher(input, out var loadInst))
					return false;
				if (!loadInst.MatchLdLoc(s))
					return false;
				return true;
			}
		}
	}
}
