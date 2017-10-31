// Copyright (c) 2015 Siegfried Pammer
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
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Constructs compound assignments and inline assignments.
	/// </summary>
	public class TransformAssignment : IBlockTransform
	{
		BlockTransformContext context;
		
		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (TransformPostIncDecOperatorOnAddress(block, i) || TransformPostIncDecOnStaticField(block, i) || TransformCSharp4PostIncDecOperatorOnAddress(block, i)) {
					block.Instructions.RemoveAt(i);
					continue;
				}
				if (TransformPostIncDecOperator(block, i)) {
					block.Instructions.RemoveAt(i);
					continue;
				}
				if (TransformInlineAssignmentStObj(block, i) || TransformInlineAssignmentLocal(block, i))
					continue;
				if (TransformInlineCompoundAssignmentCall(block, i))
					continue;
				if (TransformRoslynCompoundAssignmentCall(block, i))
					continue;
				if (TransformRoslynPostIncDecOperatorOnAddress(block, i))
					continue;
			}
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// stobj(..., ldloc s)
		/// -->
		/// stloc l(stobj (..., value))
		/// </code>
		/// e.g. used for inline assignment to instance field
		/// 
		/// -or-
		/// 
		/// <code>
		/// stloc s(value)
		/// stobj (..., ldloc s)
		/// -->
		/// stloc s(stobj (..., value))
		/// </code>
		/// e.g. used for inline assignment to static field
		bool TransformInlineAssignmentStObj(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			// in some cases it can be a compiler-generated local
			if (inst == null || (inst.Variable.Kind != VariableKind.StackSlot && inst.Variable.Kind != VariableKind.Local))
				return false;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1);
			ILInstruction replacement;
			StObj fieldStore;
			ILVariable local;
			if (nextInst is StLoc localStore) { // with extra local
				if (localStore.Variable.Kind != VariableKind.Local || !localStore.Value.MatchLdLoc(inst.Variable))
					return false;
				if (!(inst.Variable.IsSingleDefinition && inst.Variable.LoadCount == 2))
					return false;
				var memberStore = block.Instructions.ElementAtOrDefault(i + 2);
				if (memberStore is StObj) {
					fieldStore = memberStore as StObj;
					if (!fieldStore.Value.MatchLdLoc(inst.Variable) || localStore.Variable.IsUsedWithin(fieldStore.Target))
						return false;
					replacement = new StObj(fieldStore.Target, inst.Value, fieldStore.Type);
				} else {
					return false;
				}
				context.Step("Inline assignment stobj (with extra local)", fieldStore);
				local = localStore.Variable;
				block.Instructions.RemoveAt(i + 1);
			} else if (nextInst is StObj) { // without extra local
				fieldStore = (StObj)nextInst;
				if (!fieldStore.Value.MatchLdLoc(inst.Variable) || inst.Variable.IsUsedWithin(fieldStore.Target))
					return false;
				context.Step("Inline assignment stobj", fieldStore);
				local = inst.Variable;
				replacement = new StObj(fieldStore.Target, inst.Value, fieldStore.Type);
			} else {
				return false;
			}
			block.Instructions.RemoveAt(i + 1);
			inst.ReplaceWith(new StLoc(local, replacement));
			return true;
		}
		
		/// <code>
		/// stloc s(binary(callvirt(getter), value))
		/// callvirt (setter, ldloc s)
		/// (followed by single usage of s in next instruction)
		/// -->
		/// stloc s(compound.op.new(callvirt(getter), value))
		/// </code>
		bool TransformInlineCompoundAssignmentCall(Block block, int i)
		{
			var mainStLoc = block.Instructions[i] as StLoc;
			// in some cases it can be a compiler-generated local
			if (mainStLoc == null || (mainStLoc.Variable.Kind != VariableKind.StackSlot && mainStLoc.Variable.Kind != VariableKind.Local))
				return false;
			BinaryNumericInstruction binary = mainStLoc.Value as BinaryNumericInstruction;
			ILVariable localVariable = mainStLoc.Variable;
			if (!localVariable.IsSingleDefinition)
				return false;
			if (localVariable.LoadCount != 2)
				return false;
			var getterCall = binary?.Left as CallInstruction;
			var setterCall = block.Instructions.ElementAtOrDefault(i + 1) as CallInstruction;
			if (!MatchingGetterAndSetterCalls(getterCall, setterCall))
				return false;
			if (!setterCall.Arguments.Last().MatchLdLoc(localVariable))
				return false;
			
			var next = block.Instructions.ElementAtOrDefault(i + 2);
			if (next == null)
				return false;
			if (next.Descendants.Where(d => d.MatchLdLoc(localVariable)).Count() != 1)
				return false;
			if (!CompoundAssignmentInstruction.IsBinaryCompatibleWithType(binary, getterCall.Method.ReturnType))
				return false;
			context.Step($"Inline compound assignment to '{getterCall.Method.AccessorOwner.Name}'", setterCall);
			block.Instructions.RemoveAt(i + 1); // remove setter call
			binary.ReplaceWith(new CompoundAssignmentInstruction(
				binary, getterCall, binary.Right,
			    getterCall.Method.ReturnType, CompoundAssignmentType.EvaluatesToNewValue));
			return true;
		}

		/// <summary>
		/// Roslyn compound assignment that's not inline within another instruction.
		/// </summary>
		bool TransformRoslynCompoundAssignmentCall(Block block, int i)
		{
			// stloc variable(callvirt get_Property(ldloc obj))
			// callvirt set_Property(ldloc obj, binary.op(ldloc variable, ldc.i4 1))
			// => compound.op.new(callvirt get_Property(ldloc obj), ldc.i4 1)
			if (!(block.Instructions[i] is StLoc stloc))
				return false;
			if (!(stloc.Variable.IsSingleDefinition && stloc.Variable.LoadCount == 1))
				return false;
			var getterCall = stloc.Value as CallInstruction;
			var setterCall = block.Instructions[i + 1] as CallInstruction;
			if (!(MatchingGetterAndSetterCalls(getterCall, setterCall)))
				return false;
			var binary = setterCall.Arguments.Last() as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(stloc.Variable))
				return false;
			if (!CompoundAssignmentInstruction.IsBinaryCompatibleWithType(binary, getterCall.Method.ReturnType))
				return false;
			context.Step($"Compound assignment to '{getterCall.Method.AccessorOwner.Name}'", setterCall);
			block.Instructions.RemoveAt(i + 1); // remove setter call
			stloc.ReplaceWith(new CompoundAssignmentInstruction(
				binary, getterCall, binary.Right,
				getterCall.Method.ReturnType, CompoundAssignmentType.EvaluatesToNewValue));
			return true;
		}

		static bool MatchingGetterAndSetterCalls(CallInstruction getterCall, CallInstruction setterCall)
		{
			if (getterCall == null || setterCall == null || !IsSameMember(getterCall.Method.AccessorOwner, setterCall.Method.AccessorOwner))
				return false;
			var owner = getterCall.Method.AccessorOwner as IProperty;
			if (owner == null || !IsSameMember(getterCall.Method, owner.Getter) || !IsSameMember(setterCall.Method, owner.Setter))
				return false;
			if (setterCall.Arguments.Count != getterCall.Arguments.Count + 1)
				return false;
			// Ensure that same arguments are passed to getterCall and setterCall:
			for (int j = 0; j < getterCall.Arguments.Count; j++) {
				if (!SemanticHelper.IsPure(getterCall.Arguments[j].Flags))
					return false;
				if (!getterCall.Arguments[j].Match(setterCall.Arguments[j]).Success)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Transform compound assignments where the return value is not being used,
		/// or where there's an inlined assignment within the setter call.
		/// </summary>
		/// <remarks>
		/// Called by ExpressionTransforms.
		/// </remarks>
		internal static bool HandleCallCompoundAssign(CallInstruction setterCall, StatementTransformContext context)
		{
			// callvirt set_Property(ldloc S_1, binary.op(callvirt get_Property(ldloc S_1), value))
			// ==> compound.op.new(callvirt(callvirt get_Property(ldloc S_1)), value)
			var setterValue = setterCall.Arguments.LastOrDefault();
			var storeInSetter = setterValue as StLoc;
			if (storeInSetter != null) {
				// callvirt set_Property(ldloc S_1, stloc v(binary.op(callvirt get_Property(ldloc S_1), value)))
				// ==> stloc v(compound.op.new(callvirt(callvirt get_Property(ldloc S_1)), value))
				setterValue = storeInSetter.Value;
			}
			if (!(setterValue is BinaryNumericInstruction binary))
				return false;
			var getterCall = binary.Left as CallInstruction;
			if (!MatchingGetterAndSetterCalls(getterCall, setterCall))
				return false;
			if (!CompoundAssignmentInstruction.IsBinaryCompatibleWithType(binary, getterCall.Method.ReturnType))
				return false;
			context.Step($"Compound assignment to '{getterCall.Method.AccessorOwner.Name}'", setterCall);
			ILInstruction newInst = new CompoundAssignmentInstruction(
				binary, getterCall, binary.Right,
				getterCall.Method.ReturnType, CompoundAssignmentType.EvaluatesToNewValue);
			if (storeInSetter != null) {
				storeInSetter.Value = newInst;
				newInst = storeInSetter;
				context.RequestRerun(); // moving stloc to top-level might trigger inlining
			}
			setterCall.ReplaceWith(newInst);
			return true;
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// -->
		/// stloc s(stloc l(value))
		/// </code>
		bool TransformInlineAssignmentLocal(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			if (inst == null || nextInst == null)
				return false;
			if (inst.Variable.Kind != VariableKind.StackSlot)
				return false;
			if (nextInst.Variable.Kind != VariableKind.Local || !nextInst.Value.MatchLdLoc(inst.Variable))
				return false;
			context.Step("Inline assignment to local variable", inst);
			var value = inst.Value;
			var var = nextInst.Variable;
			var stackVar = inst.Variable;
			block.Instructions.RemoveAt(i);
			nextInst.ReplaceWith(new StLoc(stackVar, new StLoc(var, value)));
			return true;
		}
		
		/// <code>
		/// stloc s(ldloc l)
		/// stloc l(binary.op(ldloc s, ldc.i4 1))
		/// -->
		/// stloc s(block {
		/// 	stloc s2(ldloc l)
		/// 	stloc l(binary.op(ldloc s2, ldc.i4 1))
		/// 	final: ldloc s2
		/// })
		/// </code>
		bool TransformPostIncDecOperator(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			if (inst == null || nextInst == null || !inst.Value.MatchLdLoc(out var l) || !ILVariableEqualityComparer.Instance.Equals(l, nextInst.Variable))
				return false;
			var binary = nextInst.Value as BinaryNumericInstruction;
			if (inst.Variable.Kind != VariableKind.StackSlot || nextInst.Variable.Kind == VariableKind.StackSlot || binary == null)
				return false;
			if (binary.IsLifted)
				return false;
			if ((binary.Operator != BinaryNumericOperator.Add && binary.Operator != BinaryNumericOperator.Sub) || !binary.Left.MatchLdLoc(inst.Variable) || !binary.Right.MatchLdcI4(1))
				return false;
			context.Step($"TransformPostIncDecOperator", inst);
			var tempStore = context.Function.RegisterVariable(VariableKind.StackSlot, inst.Variable.Type);
			var assignment = new Block(BlockType.PostfixOperator);
			assignment.Instructions.Add(new StLoc(tempStore, new LdLoc(nextInst.Variable)));
			assignment.Instructions.Add(new StLoc(nextInst.Variable, new BinaryNumericInstruction(binary.Operator, new LdLoc(tempStore), new LdcI4(1), binary.CheckForOverflow, binary.Sign)));
			assignment.FinalInstruction = new LdLoc(tempStore);
			nextInst.ReplaceWith(new StLoc(inst.Variable, assignment));
			return true;
		}
		
		/// ldaddress ::= ldelema | ldflda | ldsflda;
		/// <code>
		/// stloc s(ldaddress)
		/// stloc l(ldobj(ldloc s))
		/// stobj(ldloc s, binary.op(ldloc l, ldc.i4 1))
		/// -->
		/// stloc l(compound.op.old(ldobj(ldaddress), ldc.i4 1))
		/// </code>
		bool TransformPostIncDecOperatorOnAddress(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			var stobj = block.Instructions.ElementAtOrDefault(i + 2) as StObj;
			if (inst == null || nextInst == null || stobj == null)
				return false;
			if (!inst.Variable.IsSingleDefinition || inst.Variable.LoadCount != 2)
				return false;
			if (!(inst.Value is LdElema || inst.Value is LdFlda || inst.Value is LdsFlda))
				return false;
			ILInstruction target;
			IType targetType;
			if (nextInst.Variable.Kind == VariableKind.StackSlot || !nextInst.Value.MatchLdObj(out target, out targetType) || !target.MatchLdLoc(inst.Variable))
				return false;
			if (!stobj.Target.MatchLdLoc(inst.Variable))
				return false;
			var binary = stobj.Value as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(nextInst.Variable) || !binary.Right.MatchLdcI4(1)
			    || (binary.Operator != BinaryNumericOperator.Add && binary.Operator != BinaryNumericOperator.Sub))
				return false;
			context.Step($"TransformPostIncDecOperator", inst);
			var assignment = new CompoundAssignmentInstruction(binary, new LdObj(inst.Value, targetType), binary.Right, targetType, CompoundAssignmentType.EvaluatesToOldValue);
			stobj.ReplaceWith(new StLoc(nextInst.Variable, assignment));
			block.Instructions.RemoveAt(i + 1);
			return true;
		}

		/// <code>
		/// stloc l(ldobj(ldflda(target)))
		/// stobj(ldflda(target), binary.op(ldloc l, ldc.i4 1))
		/// -->
		/// compound.op.old(ldobj(ldflda(target)), ldc.i4 1)
		/// </code>
		bool TransformRoslynPostIncDecOperatorOnAddress(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var stobj = block.Instructions.ElementAtOrDefault(i + 1) as StObj;
			if (inst == null || stobj == null)
				return false;
			if (!inst.Variable.IsSingleDefinition || inst.Variable.LoadCount != 1)
				return false;
			if (!inst.Value.MatchLdObj(out var loadTarget, out var loadType) || !loadTarget.MatchLdFlda(out var fieldTarget, out var field))
				return false;
			if (!stobj.Target.MatchLdFlda(out var fieldTarget2, out var field2))
				return false;
			if (!fieldTarget.Match(fieldTarget2).Success || !field.Equals(field2))
				return false;
			var binary = stobj.Value as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(inst.Variable) || !binary.Right.MatchLdcI4(1)
				|| (binary.Operator != BinaryNumericOperator.Add && binary.Operator != BinaryNumericOperator.Sub))
				return false;
			context.Step("TransformRoslynPostIncDecOperator", inst);
			stobj.ReplaceWith(new CompoundAssignmentInstruction(binary, inst.Value, binary.Right, loadType, CompoundAssignmentType.EvaluatesToOldValue));
			block.Instructions.RemoveAt(i);
			return true;
		}

		/// <code>
		/// stloc s(ldflda)
		/// stloc s2(ldobj(ldflda(ldloc s)))
		/// stloc l(ldloc s2)
		/// stobj (ldflda(ldloc s), binary.add(ldloc s2, ldc.i4 1))
		/// -->
		/// stloc l(compound.op.old(ldobj(ldflda(ldflda)), ldc.i4 1))
		/// </code>
		bool TransformCSharp4PostIncDecOperatorOnAddress(Block block, int i)
		{
			var baseFieldAddress = block.Instructions[i] as StLoc;
			var fieldValue = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			var fieldValueCopyToLocal = block.Instructions.ElementAtOrDefault(i + 2) as StLoc;
			var stobj = block.Instructions.ElementAtOrDefault(i + 3) as StObj;
			if (baseFieldAddress == null || fieldValue == null || fieldValueCopyToLocal == null || stobj == null)
				return false;
			if (baseFieldAddress.Variable.Kind != VariableKind.StackSlot || fieldValue.Variable.Kind != VariableKind.StackSlot || fieldValueCopyToLocal.Variable.Kind != VariableKind.Local)
				return false;
			IType t;
			IField targetField;
			ILInstruction targetFieldLoad, baseFieldAddressLoad2;
			if (!fieldValue.Value.MatchLdObj(out targetFieldLoad, out t))
				return false;
			ILInstruction baseAddress;
			if (baseFieldAddress.Value is LdFlda) {
				IField targetField2;
				ILInstruction baseFieldAddressLoad3;
				if (!targetFieldLoad.MatchLdFlda(out baseFieldAddressLoad2, out targetField) || !baseFieldAddressLoad2.MatchLdLoc(baseFieldAddress.Variable))
					return false;
				if (!stobj.Target.MatchLdFlda(out baseFieldAddressLoad3, out targetField2) || !baseFieldAddressLoad3.MatchLdLoc(baseFieldAddress.Variable) || !IsSameMember(targetField, targetField2))
					return false;
				baseAddress = new LdFlda(baseFieldAddress.Value, targetField);
			} else if (baseFieldAddress.Value is LdElema) {
				if (!targetFieldLoad.MatchLdLoc(baseFieldAddress.Variable) || !stobj.Target.MatchLdLoc(baseFieldAddress.Variable))
					return false;
				baseAddress = baseFieldAddress.Value;
			} else {
				return false;
			}
			BinaryNumericInstruction binary = stobj.Value as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(fieldValue.Variable) || !binary.Right.MatchLdcI4(1)
			    || (binary.Operator != BinaryNumericOperator.Add && binary.Operator != BinaryNumericOperator.Sub))
				return false;
			context.Step($"TransformCSharp4PostIncDecOperatorOnAddress", baseFieldAddress);
			var assignment = new CompoundAssignmentInstruction(binary, new LdObj(baseAddress, t), binary.Right, t, CompoundAssignmentType.EvaluatesToOldValue);
			stobj.ReplaceWith(new StLoc(fieldValueCopyToLocal.Variable, assignment));
			block.Instructions.RemoveAt(i + 2);
			block.Instructions.RemoveAt(i + 1);
			return true;
		}
		
		/// <code>
		/// stloc s(ldobj(ldsflda))
		/// stobj (ldsflda, binary.op(ldloc s, ldc.i4 1))
		/// -->
		/// stloc s(compound.op.old(ldobj(ldsflda), ldc.i4 1))
		/// </code>
		bool TransformPostIncDecOnStaticField(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var stobj = block.Instructions.ElementAtOrDefault(i + 1) as StObj;
			if (inst == null || stobj == null)
				return false;
			ILInstruction target;
			IType type;
			IField field, field2;
			if (inst.Variable.Kind != VariableKind.StackSlot || !inst.Value.MatchLdObj(out target, out type) || !target.MatchLdsFlda(out field))
				return false;
			if (!stobj.Target.MatchLdsFlda(out field2) || !IsSameMember(field, field2))
				return false;
			var binary = stobj.Value as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(inst.Variable) || !binary.Right.MatchLdcI4(1))
				return false;
			context.Step($"TransformPostIncDecOnStaticField", inst);
			var assignment = new CompoundAssignmentInstruction(binary, inst.Value, binary.Right, type, CompoundAssignmentType.EvaluatesToOldValue);
			stobj.ReplaceWith(new StLoc(inst.Variable, assignment));
			return true;
		}
		
		static bool IsSameMember(IMember a, IMember b)
		{
			if (a == null || b == null)
				return false;
			a = a.MemberDefinition;
			b = b.MemberDefinition;
			return a.Equals(b);
		}
	}
}
