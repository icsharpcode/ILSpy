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
					TransformInlineAssignmentFields(block, i);
					TransformInlineAssignmentLocal(block, i);
				}
			}
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
			block.Instructions.RemoveAt(i + 1);
			var value = inst.Value.Clone();
			inst.Value.ReplaceWith(new StLoc(nextInst.Variable, value));
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// stfld f(..., ldloc s)
		/// -->
		/// stloc l(stfld f(..., value))
		/// </code>
		static void TransformInlineAssignmentFields(Block block, int i)
		{
//			var inst = block.Instructions[i] as StLoc;
//			var nextInst = block.Instructions.ElementAtOrDefault(i + 1) as StLoc;
//			var fieldStore = block.Instructions.ElementAtOrDefault(i + 2) as StObj;
//			if (inst == null || nextInst == null || fieldStore == null)
//				return;
//			if (nextInst.Variable.Kind == VariableKind.StackSlot || !nextInst.Value.MatchLdLoc(inst.Variable) || !fieldStore.Value.MatchLdLoc(inst.Variable))
//				return;
//			var value = inst.Value.Clone();
//			var locVar = nextInst.Variable;
//			block.Instructions.RemoveAt(i + 1);
//			block.Instructions.RemoveAt(i + 1);
//			inst.ReplaceWith(new StLoc(locVar, new StObj(fieldStore.Target, value, fieldStore.Field)));
		}
	}
}
