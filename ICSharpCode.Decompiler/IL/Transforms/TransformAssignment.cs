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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Description of TransformAssignment.
	/// </summary>
	public class TransformAssignment : IILTransform
	{
		ILTransformContext context;
		
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					if (TransformPostIncDecOperatorOnAddress(block, i) || TransformPostIncDecOnStaticField(block, i) || TransformCSharp4PostIncDecOperatorOnAddress(block, i)) {
						block.Instructions.RemoveAt(i);
						continue;
					}
					if (InlineLdAddressUsages(block, i)) {
						block.Instructions.RemoveAt(i);
						continue;
					}
					if (TransformPostIncDecOperator(block, i, function)) {
						block.Instructions.RemoveAt(i);
						continue;
					}
					TransformInlineAssignmentStObj(block, i);
				}
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
		static void TransformInlineAssignmentStObj(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			if (inst == null || inst.Variable.Kind != VariableKind.StackSlot)
				return;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1);
			ILInstruction value;
			StObj fieldStore;
			ILVariable local;
			if (nextInst is StLoc) { // instance fields
				var localStore = (StLoc)nextInst;
				fieldStore = block.Instructions.ElementAtOrDefault(i + 2) as StObj;
				if (fieldStore == null) { // otherwise it must local
					TransformInlineAssignmentLocal(block, i);
					return;
				}
				if (localStore.Variable.Kind == VariableKind.StackSlot || !localStore.Value.MatchLdLoc(inst.Variable) || !fieldStore.Value.MatchLdLoc(inst.Variable))
					return;
				value = inst.Value;
				local = localStore.Variable;
				block.Instructions.RemoveAt(i + 1);
			} else if (nextInst is StObj) { // static fields
				fieldStore = (StObj)nextInst;
				if (!fieldStore.Value.MatchLdLoc(inst.Variable))
					return;
				value = inst.Value;
				local = inst.Variable;
			} else return;
			block.Instructions.RemoveAt(i + 1);
			inst.ReplaceWith(new StLoc(local, new StObj(fieldStore.Target, value, fieldStore.Type)));
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// -->
		/// stloc s(stloc l(value))
		/// </code>
		static void TransformInlineAssignmentLocal(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
			if (inst == null || nextInst == null)
				return;
			if (nextInst.Variable.Kind == VariableKind.StackSlot || !nextInst.Value.MatchLdLoc(inst.Variable))
				return;
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
		static bool InlineLdAddressUsages(Block block, int i)
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
		static bool TransformPostIncDecOperator(Block block, int i, ILFunction function)
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
			var tempStore = function.RegisterVariable(VariableKind.StackSlot, inst.Variable.Type);
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
		static bool TransformPostIncDecOperatorOnAddress(Block block, int i)
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
		static bool TransformCSharp4PostIncDecOperatorOnAddress(Block block, int i)
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
				if (!stobj.Target.MatchLdFlda(out baseFieldAddressLoad3, out targetField2) || !baseFieldAddressLoad3.MatchLdLoc(baseFieldAddress.Variable) || !SameField(targetField, targetField2))
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
		static bool TransformPostIncDecOnStaticField(Block block, int i)
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
			if (!stobj.Target.MatchLdsFlda(out field2) || !SameField(field, field2))
				return false;
			var binary = stobj.Value as BinaryNumericInstruction;
			if (binary == null || !binary.Left.MatchLdLoc(inst.Variable) || !binary.Right.MatchLdcI4(1))
				return false;
			var assignment = new CompoundAssignmentInstruction(binary.Operator, inst.Value, binary.Right, type, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToOldValue);
			stobj.ReplaceWith(new StLoc(inst.Variable, assignment));
			return true;
		}
		
		static bool SameField(IField a, IField b)
		{
			a = (IField)a.MemberDefinition;
			b = (IField)b.MemberDefinition;
			return a.Equals(b);
		}
	}
}
