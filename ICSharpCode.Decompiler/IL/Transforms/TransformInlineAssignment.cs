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
	/// Description of TransformInlineAssignment.
	/// </summary>
	public class TransformInlineAssignment : IILTransform
	{
		ILTransformContext context;
		
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					if (InlineLdElemaUsages(block, i)) {
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
		
		/// <code>
		/// stloc s(ldelema)
		/// usages of ldobj(ldloc s) or stobj(ldloc s, ...) in next instruction
		/// -->
		/// use ldelema instead of ldloc s
		/// </code>
		static bool InlineLdElemaUsages(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			if (inst == null || inst.Variable.Kind != VariableKind.StackSlot || !(inst.Value is LdElema))
				return false;
			var valueToCopy = inst.Value;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var affectedUsages = nextInstruction.Descendants
				.OfType<IInstructionWithVariableOperand>().Where(ins => ins.Variable == inst.Variable)
				.Cast<ILInstruction>().ToArray();
			if (affectedUsages.Any(ins => !(ins.Parent is StObj || ins.Parent is LdObj)))
				return false;
			foreach (var usage in affectedUsages) {
				usage.ReplaceWith(valueToCopy.Clone());
			}
			return true;
		}
	}
}
