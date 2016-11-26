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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Description of TransformAssignment.
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
				if (InlineLdAddressUsages(block, i)) {
					block.Instructions.RemoveAt(i);
					continue;
				}
				if (TransformPostIncDecOperator(block, i)) {
					block.Instructions.RemoveAt(i);
					continue;
				}
				if (TransformInlineAssignmentStObj(block, i))
					continue;
				if (TransformInlineAssignmentCall(block, i))
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
		/// -or-
		/// <code>
		/// stloc s(value)
		/// stobj (..., ldloc s)
		/// -->
		/// stloc s(stobj (..., value))
		/// </code>
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
			if (nextInst is StLoc) { // instance fields
				var localStore = (StLoc)nextInst;
				if (localStore.Variable.Kind == VariableKind.StackSlot || !localStore.Value.MatchLdLoc(inst.Variable))
					return false;
				var memberStore = block.Instructions.ElementAtOrDefault(i + 2);
				if (memberStore is StObj) {
					fieldStore = memberStore as StObj;
					if (!fieldStore.Value.MatchLdLoc(inst.Variable))
						return false;
					replacement = new StObj(fieldStore.Target, inst.Value, fieldStore.Type);
				} else { // otherwise it must be local
					TransformInlineAssignmentLocal(block, i);
					return false;
				}
				context.Step("Inline assignment to instance field", fieldStore);
				local = localStore.Variable;
				block.Instructions.RemoveAt(i + 1);
			} else if (nextInst is StObj) { // static fields
				fieldStore = (StObj)nextInst;
				if (!fieldStore.Value.MatchLdLoc(inst.Variable))
					return false;
				context.Step("Inline assignment to static field", fieldStore);
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
		/// ... usage of s ...
		/// -->
		/// ... compound.op.new(callvirt(getter), value) ...
		/// </code>
		/// -or-
		/// <code>
		/// stloc s(stloc v(binary(callvirt(getter), value)))
		/// callvirt (setter, ldloc s)
		/// ... usage of v ...
		/// -->
		/// ... compound.op.new(callvirt(getter), value) ...
		/// </code>
		bool TransformInlineAssignmentCall(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			// in some cases it can be a compiler-generated local
			if (inst == null || (inst.Variable.Kind != VariableKind.StackSlot && inst.Variable.Kind != VariableKind.Local))
				return false;
			BinaryNumericInstruction binary;
			ILVariable localVariable;
			if (inst.Value is StLoc) {
				var tmp = (StLoc)inst.Value;
				binary = tmp.Value as BinaryNumericInstruction;
				localVariable = tmp.Variable;
			} else {
				binary = inst.Value as BinaryNumericInstruction;
				localVariable = inst.Variable;
			}
			var getterCall = binary?.Left as CallInstruction;
			var setterCall = block.Instructions.ElementAtOrDefault(i + 1) as CallInstruction;
			if (getterCall == null || setterCall == null || !IsSameMember(getterCall.Method.AccessorOwner, setterCall.Method.AccessorOwner))
				return false;
			var owner = getterCall.Method.AccessorOwner as IProperty;
			if (owner == null || !IsSameMember(getterCall.Method, owner.Getter) || !IsSameMember(setterCall.Method, owner.Setter))
				return false;
			var next = block.Instructions.ElementAtOrDefault(i + 2);
			if (next == null)
				return false;
			var usages = next.Descendants.Where(d => d.MatchLdLoc(localVariable)).ToArray();
			if (usages.Length != 1)
				return false;
			context.Step($"Compound assignment to '{owner.Name}'", setterCall);
			block.Instructions.RemoveAt(i + 1);
			block.Instructions.RemoveAt(i);
			usages[0].ReplaceWith(new CompoundAssignmentInstruction(binary.Operator, getterCall, binary.Right,
			                                                   getterCall.Method.ReturnType, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToNewValue));
			return true;
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// -->
		/// stloc s(stloc l(value))
		/// </code>
		void TransformInlineAssignmentLocal(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			if (inst == null || nextInst == null)
				return;
			if (nextInst.Variable.Kind == VariableKind.StackSlot || !nextInst.Value.MatchLdLoc(inst.Variable))
				return;
			context.Step("Inline assignment to local variable", inst);
			var value = inst.Value;
			var var = nextInst.Variable;
			var stackVar = inst.Variable;
			block.Instructions.RemoveAt(i);
			nextInst.ReplaceWith(new StLoc(stackVar, new StLoc(var, value)));
		}
		
		/// ldaddress ::= ldelema | ldflda | ldsflda;
		/// <code>
		/// stloc s(ldaddress)
		/// usages of ldobj(ldloc s) or stobj(ldloc s, ...) in next instruction
		/// -->
		/// use ldaddress instead of ldloc s
		/// </code>
		bool InlineLdAddressUsages(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			if (inst == null || inst.Variable.Kind != VariableKind.StackSlot || !(inst.Value is LdElema || inst.Value is LdFlda || inst.Value is LdsFlda))
				return false;
			var valueToCopy = inst.Value;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var affectedUsages = block.Descendants
				.OfType<IInstructionWithVariableOperand>()
				.Where(ins => ins != inst)
				.Where(ins => ins.Variable == inst.Variable)
				.Cast<ILInstruction>().ToArray();
			if (affectedUsages.Length == 0 || affectedUsages.Any(ins => !(ins.Parent is StObj || ins.Parent is LdObj)))
				return false;
			context.Step($"InlineLdAddressUsage", inst);
			foreach (var usage in affectedUsages) {
				usage.ReplaceWith(valueToCopy.Clone());
			}
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
			if (inst == null || nextInst == null)
				return false;
			var binary = nextInst.Value as BinaryNumericInstruction;
			if (inst.Variable.Kind != VariableKind.StackSlot || nextInst.Variable.Kind == VariableKind.StackSlot || binary == null)
				return false;
			if ((binary.Operator != BinaryNumericOperator.Add && binary.Operator != BinaryNumericOperator.Sub) || !binary.Left.MatchLdLoc(inst.Variable) || !binary.Right.MatchLdcI4(1))
				return false;
			context.Step($"TransformPostIncDecOperator", inst);
			var tempStore = context.Function.RegisterVariable(VariableKind.StackSlot, inst.Variable.Type);
			var assignment = new Block(BlockType.CompoundOperator);
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
			var assignment = new CompoundAssignmentInstruction(binary.Operator, new LdObj(inst.Value, targetType), binary.Right, targetType, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToOldValue);
			stobj.ReplaceWith(new StLoc(nextInst.Variable, assignment));
			block.Instructions.RemoveAt(i + 1);
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
			var assignment = new CompoundAssignmentInstruction(binary.Operator, new LdObj(baseAddress, t), binary.Right, t, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToOldValue);
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
			var assignment = new CompoundAssignmentInstruction(binary.Operator, inst.Value, binary.Right, type, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToOldValue);
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
