// Copyright (c) 2011-2016 Siegfried Pammer
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
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class RemoveCachedDelegateInitialization : IILTransform
	{
		ILTransformContext context;
		ITypeResolveContext decompilationContext;
		
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!new DecompilerSettings().AnonymousMethods)
				return;
			this.context = context;
			this.decompilationContext = new SimpleTypeResolveContext(context.TypeSystem.Resolve(function.Method));
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					var inst = block.Instructions[i] as IfInstruction;
					if (inst != null) {
						if (CachedDelegateInitializationWithField(inst)) {
							block.Instructions.RemoveAt(i);
							continue;
						}
						if (CachedDelegateInitializationWithLocal(inst)) {
							block.Instructions.RemoveAt(i);
							continue;
						}
					}
				}
			}
		}
		
		bool CachedDelegateInitializationWithField(IfInstruction inst)
		{
			// if (comp(ldsfld CachedAnonMethodDelegate == ldnull) {
			//     stsfld CachedAnonMethodDelegate(DelegateConstruction)
			// }
			// ... one usage of CachedAnonMethodDelegate ...
			// =>
			// ... one usage of DelegateConstruction ...
			Block trueInst = inst.TrueInst as Block;
			var condition = inst.Condition as Comp;
			if (condition == null || trueInst == null || trueInst.Instructions.Count != 1 || !inst.FalseInst.MatchNop())
				return false;
			IField field, field2;
			ILInstruction value;
			var storeInst = trueInst.Instructions[0];
			if (!condition.Left.MatchLdsFld(out field) || !condition.Right.MatchLdNull())
				return false;
			if (!storeInst.MatchStsFld(out value, out field2) || !field.Equals(field2) || !field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			if (value.OpCode != OpCode.NewObj || !ExpressionBuilder.IsDelegateConstruction((CallInstruction)value))
				return false;
			var targetMethod = ((IInstructionWithMethodOperand)((CallInstruction)value).Arguments[1]).Method;
			if (!DelegateConstruction.IsAnonymousMethod(decompilationContext.CurrentTypeDefinition, targetMethod))
				return false;
			
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var usages = nextInstruction.Descendants.OfType<LdsFld>().Where(i => i.Field.Equals(field)).ToArray();
			if (usages.Length > 1)
				return false;
			usages[0].ReplaceWith(value);
			return true;
		}

		bool CachedDelegateInitializationWithLocal(IfInstruction inst)
		{
			return false;
		}
	}
}
