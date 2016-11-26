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

using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class CachedDelegateInitialization : IBlockTransform
	{
		BlockTransformContext context;

		public void Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			if (!context.Settings.AnonymousMethods)
				return;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (block.Instructions[i] is IfInstruction inst) {
					if (CachedDelegateInitializationWithField(inst)) {
						block.Instructions.RemoveAt(i);
						continue;
					}
					if (CachedDelegateInitializationWithLocal(inst, out bool hasFieldStore, out ILVariable v)) {
						block.Instructions.RemoveAt(i);
						if (hasFieldStore) {
							block.Instructions.RemoveAt(i - 1);
						}
						//if (v.IsSingleDefinition && v.LoadCount == 0) {
						//	var store = v.Scope.Descendants.OfType<StLoc>().SingleOrDefault(stloc => stloc.Variable == v);
						//	if (store != null) {
						//		orphanedVariableInits.Add(store);
						//	}
						//}
						continue;
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
			if (trueInst == null || trueInst.Instructions.Count != 1 || !inst.FalseInst.MatchNop())
				return false;
			var storeInst = trueInst.Instructions[0];
			if (!inst.Condition.MatchCompEquals(out ILInstruction left, out ILInstruction right) || !left.MatchLdsFld(out IField field) || !right.MatchLdNull())
				return false;
			if (!storeInst.MatchStsFld(out ILInstruction value, out IField field2) || !field.Equals(field2) || !field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			if (!DelegateConstruction.IsDelegateConstruction(value as NewObj, true))
				return false;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var usages = nextInstruction.Descendants.Where(i => i.MatchLdsFld(field)).ToArray();
			if (usages.Length != 1)
				return false;
			context.Step("CachedDelegateInitializationWithField", inst);
			usages[0].ReplaceWith(value);
			return true;
		}

		bool CachedDelegateInitializationWithLocal(IfInstruction inst, out bool hasFieldStore, out ILVariable local)
		{
			// [stloc v(ldsfld CachedAnonMethodDelegate)]
			// if (comp(ldloc v == ldnull) {
			//     stloc v(DelegateConstruction)
			//     [stsfld CachedAnonMethodDelegate(v)]
			// }
			// ... one usage of v ...
			// =>
			// ... one usage of DelegateConstruction ...
			Block trueInst = inst.TrueInst as Block;
			hasFieldStore = false;
			local = null;
			if (trueInst == null || (trueInst.Instructions.Count < 1) || !inst.FalseInst.MatchNop())
				return false;
			if (!inst.Condition.MatchCompEquals(out ILInstruction left, out ILInstruction right) || !left.MatchLdLoc(out ILVariable v) || !right.MatchLdNull())
				return false;
			ILInstruction value, value2;
			var storeInst = trueInst.Instructions.Last();
			if (!storeInst.MatchStLoc(v, out value))
				return false;
			// the optional field store was moved into storeInst by inline assignment:
			if (!(value is NewObj)) {
				IField field, field2;
				if (value.MatchStsFld(out value2, out field)) {
					if (!(value2 is NewObj) || !field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
						return false;
					var storeBeforeIf = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex - 1) as StLoc;
					if (storeBeforeIf == null || storeBeforeIf.Variable != v || !storeBeforeIf.Value.MatchLdsFld(out field2) || !field.Equals(field2))
						return false;
					value = value2;
					hasFieldStore = true;
				} else if (value.MatchStFld(out value2, out field)) {
					if (!(value2 is NewObj) || !field.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
						return false;
					var storeBeforeIf = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex - 1) as StLoc;
					if (storeBeforeIf == null || storeBeforeIf.Variable != v || !storeBeforeIf.Value.MatchLdFld(out field2) || !field.Equals(field2))
						return false;
					value = value2;
					hasFieldStore = true;
				} else {
					return false;
				}
			}
			if (!DelegateConstruction.IsDelegateConstruction(value as NewObj, true))
				return false;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var usages = nextInstruction.Descendants.OfType<LdLoc>().Where(i => i.Variable == v).ToArray();
			if (usages.Length != 1)
				return false;
			context.Step("CachedDelegateInitializationWithLocal", inst);
			local = v;
			usages[0].ReplaceWith(value);
			return true;
		}
	}
}
