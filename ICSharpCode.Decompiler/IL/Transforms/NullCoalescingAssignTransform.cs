// Copyright (c) 2025 Daniel Grunwald
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform for constructing the NullCoalescingCompoundAssign (if.notnull.compound(a,b), or in C#: ??=)
	/// </summary>
	public class NullCoalescingAssignTransform : IStatementTransform
	{
		/// <summary>
		/// Run as a expression-transform for a NullCoalescingInstruction that was already detected.
		/// 
		/// 	if.notnull(load(x), store(x, fallback))
		///   =>
		///     if.notnull.compound(x, fallback)
		/// </summary>
		public static bool RunForExpression(NullCoalescingInstruction nci, StatementTransformContext context)
		{
			if (nci.Kind != NullCoalescingKind.Ref)
				return false;
			ILInstruction load = nci.ValueInst;
			ILInstruction store = nci.FallbackInst;

			if (!TransformAssignment.IsCompoundStore(store, out var storeType, out var rhsValue, context.TypeSystem))
				return false;
			if (!TransformAssignment.IsMatchingCompoundLoad(load, store, out var target, out var targetKind, out var finalizeMatch))
				return false;

			context.Step("Null coalescing assignment: ref types (expression transform)", nci);
			nci.ReplaceWith(new NullCoalescingCompoundAssign(target, targetKind, nci.Kind, rhsValue, storeType).WithILRange(nci));
			return true;
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (TransformRefTypes(block, pos, context))
				return;
			if (TransformValueTypes(block, pos, context))
				return;
		}

		/// <summary>
		/// case 1: ref local of class type, return value of ??= not used.
		/// 
		///    if (comp.o(ldobj System.Object(ldloc reference) == ldnull)) Block {
		///       stobj System.Object(ldloc reference, ldstr "Hello")
		///    }
		///  =>
		///    if.notnull.ref.compound.address.new(ldloc reference, ldstr "Hello")
		/// 
		/// Note: case 3 (class type and return value of ??= is used) is handled as ExpressionTransform.
		/// </summary>
		static bool TransformRefTypes(Block block, int pos, StatementTransformContext context)
		{
			if (!block.Instructions[pos].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			trueInst = Block.Unwrap(trueInst);
			if (!condition.MatchCompEqualsNull(out var loadInCondition))
				return false;
			if (!TransformAssignment.IsCompoundStore(trueInst, out var storeType, out var rhsValue, context.TypeSystem))
				return false;
			if (!TransformAssignment.IsMatchingCompoundLoad(loadInCondition, trueInst, out var target, out var targetKind, out var finalizeMatch))
				return false;
			context.Step("Null coalescing assignment: ref types", condition);
			var result = new NullCoalescingCompoundAssign(target, targetKind, NullCoalescingKind.Ref, rhsValue, storeType);
			result.AddILRange(block.Instructions[pos]);
			result.AddILRange(trueInst);
			block.Instructions[pos] = result;
			finalizeMatch?.Invoke(context);
			return true;
		}

		/// <summary>
		/// case 2: ref local of value type, return value of ??= not used.
		/// 
		///    stloc temp1(call GetValueOrDefault(ldloc reference))
		///    if (logic.not(call get_HasValue(ldloc reference))) Block {
		///      stloc temp2(ldc.i4 42)
		///      stobj System.Nullable`1[[System.Int32]](ldloc reference, newobj Nullable..ctor(ldloc temp2))
		///    }
		///  =>
		///    if.notnull.nullablewithvaluefallback.compound.address.new(ldloc reference, ldc.i4 42)
		/// 
		/// 
		/// case 4: ref local of value type, return value of ??= used.
		/// 
		///    stloc temp1(call GetValueOrDefault(ldloc reference))
		///    if (logic.not(call get_HasValue(ldloc reference))) Block {
		///      stloc temp2(ldc.i4 42)
		///      stobj System.Nullable`1[[System.Int32]](ldloc reference, newobj Nullable..ctor(ldloc temp2))
		///      stloc resultVar(ldloc temp2)
		///    } else Block {
		///      stloc resultVar(ldloc temp1)
		///    }
		///  =>
		///    if.notnull.nullablewithvaluefallback.compound.address.new(ldloc reference, ldc.i4 42)
		/// </summary>
		static bool TransformValueTypes(Block block, int pos, StatementTransformContext context)
		{
			// stloc valueOrDefault(call GetValueOrDefault(ldloc reference))
			if (!block.Instructions[pos].MatchStLoc(out var temp1, out var getValueOrDefaultCall))
				return false;
			if (!(temp1.IsSingleDefinition && temp1.LoadCount <= 1))
				return false;
			if (!NullableLiftingTransform.MatchGetValueOrDefault(getValueOrDefaultCall, out ILInstruction load1))
				return false;

			// if (logic.not(call get_HasValue(ldloc reference)))
			if (!block.Instructions[pos + 1].MatchIfInstructionPositiveCondition(out var condition, out var hasValueInst, out var noValueInst))
				return false;
			if (!NullableLiftingTransform.MatchHasValueCall(condition, out ILInstruction load2))
				return false;

			// Check the else block (HasValue returned true)
			hasValueInst = Block.Unwrap(hasValueInst);
			ILVariable? resultVar = null;
			if (temp1.LoadCount == 0)
			{
				// resulting value not used: hasValueInst must no Nop
				if (!hasValueInst.MatchNop())
					return false;
			}
			else if (temp1.LoadCount == 1)
			{
				// stloc resultVar(ldloc temp1)
				if (!hasValueInst.MatchStLoc(out resultVar, out var temp1Load))
					return false;
				if (!temp1Load.MatchLdLoc(temp1))
					return false;
			}

			// Check the then block (HasValue returned false)
			if (!CheckNoValueBlock(noValueInst as Block, resultVar, context, out var storeInst, out var storeType, out var rhsValue))
				return false;
			// Checks loads for consistency with the storeInst:
			if (!IsMatchingCompoundLdloca(load1, storeInst, rhsValue, out var target, out var targetKind, out var needToRecombineLocals1))
				return false;
			if (!IsMatchingCompoundLdloca(load2, storeInst, rhsValue, out target, out targetKind, out var needToRecombineLocals2))
				return false;
			Debug.Assert(needToRecombineLocals1 == needToRecombineLocals2);

			context.Step("Null coalescing assignment: value types", condition);
			ILInstruction result = new NullCoalescingCompoundAssign(target, targetKind, NullCoalescingKind.NullableWithValueFallback, rhsValue, storeType);
			result.AddILRange(block.Instructions[pos]);
			result.AddILRange(block.Instructions[pos + 1]);
			if (resultVar != null)
			{
				result = new StLoc(resultVar, result);
				context.RequestRerun(); // The new StLoc could allow inlining.
			}
			block.Instructions[pos] = result;
			block.Instructions.RemoveAt(pos + 1);
			if (needToRecombineLocals1 || needToRecombineLocals2)
			{
				Debug.Assert(((LdLoca)load1).Variable == ((LdLoca)load2).Variable);
				Debug.Assert(needToRecombineLocals1 == needToRecombineLocals2);
				context.Function.RecombineVariables(((LdLoca)load1).Variable, ((StLoc)storeInst).Variable);
			}
			return true;
		}

		/// Called with a block like: Block {
		///      stloc temp2(ldc.i4 42)
		///      stobj Nullable[T](ldloc reference, newobj Nullable..ctor(ldloc temp2))
		///      stloc resultVar(ldloc temp2)
		///    }
		static bool CheckNoValueBlock(Block? block, ILVariable? resultVar, StatementTransformContext context,
			[NotNullWhen(true)] out ILInstruction? storeInst, [NotNullWhen(true)] out IType? storeType, [NotNullWhen(true)] out ILInstruction? rhsValue)
		{
			storeInst = null;
			storeType = null;
			rhsValue = null;
			if (block == null)
				return false;

			int pos = 0;
			// stloc temp2(ldc.i4 42)
			if (block.Instructions.ElementAtOrDefault(pos) is not StLoc temp2Store)
				return false;
			if (!(temp2Store.Variable.IsSingleDefinition && temp2Store.Variable.LoadCount == (resultVar != null ? 2 : 1)))
				return false;
			rhsValue = temp2Store.Value;
			pos += 1;

			// stobj Nullable[T](ldloc reference, newobj Nullable..ctor(ldloc temp2))
			storeInst = block.Instructions.ElementAtOrDefault(pos);
			if (!TransformAssignment.IsCompoundStore(storeInst, out storeType, out var storeValue, context.TypeSystem))
				return false;
			if (!NullableLiftingTransform.MatchNullableCtor(storeValue, out _, out var temp2Load))
				return false;
			if (!temp2Load.MatchLdLoc(temp2Store.Variable))
				return false;
			pos += 1;

			if (resultVar != null)
			{
				// stloc resultVar(ldloc temp2)
				var resultStore = block.Instructions.ElementAtOrDefault(pos);
				if (!(resultStore != null && resultStore.MatchStLoc(resultVar, out temp2Load)))
					return false;
				if (!temp2Load.MatchLdLoc(temp2Store.Variable))
					return false;
				pos++;
			}
			return pos == block.Instructions.Count;
		}

		static bool IsMatchingCompoundLdloca(ILInstruction load, ILInstruction store, ILInstruction reorderedInst, [NotNullWhen(true)] out ILInstruction? target, out CompoundTargetKind targetKind, out bool needToRecombineLocals)
		{
			needToRecombineLocals = false;
			// Store was already validated by TransformAssignment.IsCompoundStore().
			// This function acts similar to TransformAssignment.IsMatchingCompoundLoad(),
			// except that it tests that `load` loads the address that was used in `store`.
			// 'reorderedInst' is the rhsValue, which originally occurs between the load and store, but
			// our transform will move the store's address-load in front of 'reorderedInst'.
			if (store is StObj stobj)
			{
				target = stobj.Target;
				targetKind = CompoundTargetKind.Address;
				return IsSameAddress(load, target, reorderedInst);
			}
			else if (store is StLoc stloc && load is LdLoca ldloca && ILVariableEqualityComparer.Instance.Equals(ldloca.Variable, stloc.Variable))
			{
				target = load;
				targetKind = CompoundTargetKind.Address;
				needToRecombineLocals = ldloca.Variable != stloc.Variable;
				return true;
			}
			target = null;
			targetKind = default;
			return false;
		}

		static bool IsSameAddress(ILInstruction addr1, ILInstruction addr2, ILInstruction reorderedInst)
		{
			if (addr1.MatchLdLoc(out var refVar))
			{
				return refVar.AddressCount == 0
					&& addr2.MatchLdLoc(refVar)
					&& !refVar.IsWrittenWithin(reorderedInst);
			}
			else if (addr1.MatchLdLoca(out var targetVar))
			{
				return addr2.MatchLdLoca(targetVar);
			}
			else if (addr1.MatchLdFlda(out var instance1, out var field1) && addr2.MatchLdFlda(out var instance2, out var field2))
			{
				return field1.Equals(field2) && IsSameAddress(instance1, instance2, reorderedInst);
			}
			else if (addr1.MatchLdsFlda(out field1) && addr2.MatchLdsFlda(out field2))
			{
				return field1.Equals(field2);
			}
			return false;
		}
	}
}
