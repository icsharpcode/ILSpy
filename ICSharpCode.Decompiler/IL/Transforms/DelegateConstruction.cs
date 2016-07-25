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
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class DelegateConstruction : IILTransform
	{
		ILTransformContext context;
		ITypeResolveContext decompilationContext;
		
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AnonymousMethods)
				return;
			this.context = context;
			this.decompilationContext = new SimpleTypeResolveContext(context.TypeSystem.Resolve(function.Method));
			var orphanedVariableInits = new List<ILInstruction>();
			var targetsToReplace = new List<ILInstruction>();
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					foreach (var call in block.Instructions[i].Descendants.OfType<NewObj>()) {
						ILInstruction target;
						ILFunction f = TransformDelegateConstruction(call, out target);
						if (f != null) {
							targetsToReplace.Add(target);
							call.Arguments[1].ReplaceWith(f);
						}
					}
					
					var inst = block.Instructions[i] as IfInstruction;
					if (inst != null) {
						if (CachedDelegateInitializationWithField(inst)) {
							block.Instructions.RemoveAt(i);
							continue;
						}
						bool hasFieldStore;
						ILVariable v;
						if (CachedDelegateInitializationWithLocal(inst, out hasFieldStore, out v)) {
							block.Instructions.RemoveAt(i);
							if (hasFieldStore) {
								block.Instructions.RemoveAt(i - 1);
							}
							if (v.IsSingleDefinition && v.LoadCount == 0) {
								var store = v.Scope.Descendants.OfType<StLoc>().SingleOrDefault(stloc => stloc.Variable == v);
								if (store != null) {
									orphanedVariableInits.Add(store);
								}
							}
							continue;
						}
					}
					
					ILVariable targetVariable;
					ILInstruction value;
					if (block.Instructions[i].MatchStLoc(out targetVariable, out value)) {
						var newObj = value as NewObj;
						// TODO : it is probably not a good idea to remove *all* display-classes
						// is there a way to minimize the false-positives?
						if (newObj != null && IsSimpleDisplayClass(newObj.Method)) {
							targetsToReplace.Add(block.Instructions[i]);
						}
					}
				}
			}
			foreach (var target in targetsToReplace) {
				if (target is IInstructionWithVariableOperand && !target.MatchLdThis())
					function.AcceptVisitor(new TransformDisplayClassUsages((IInstructionWithVariableOperand)target, orphanedVariableInits));
			}
			foreach (var store in orphanedVariableInits) {
				ILInstruction containingBlock = store.Parent as Block;
				if (containingBlock != null)
					((Block)containingBlock).Instructions.Remove(store);
			}
		}

		bool IsSimpleDisplayClass(IMethod method)
		{
			if (!method.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			var type = method.DeclaringType;
			if (!type.HasGeneratedName() || (!type.Name.Contains("DisplayClass") && !type.Name.Contains("AnonStorey")))
				return false;
			if (type.DirectBaseTypes.Any(t => !t.IsKnownType(KnownTypeCode.Object)))
				return false;
			return true;
		}

		#region TransformDelegateConstruction
		internal static bool IsDelegateConstruction(NewObj inst, bool allowTransformed = false)
		{
			if (inst == null || inst.Arguments.Count != 2 || inst.Method.DeclaringType.Kind != TypeKind.Delegate)
				return false;
			var opCode = inst.Arguments[1].OpCode;
			
			return opCode == OpCode.LdFtn || opCode == OpCode.LdVirtFtn || (allowTransformed && opCode == OpCode.ILFunction);
		}
		
		static bool IsAnonymousMethod(ITypeDefinition decompiledTypeDefinition, IMethod method)
		{
			if (method == null || !(method.HasGeneratedName() || method.Name.Contains("$")))
				return false;
			if (!(method.IsCompilerGeneratedOrIsInCompilerGeneratedClass() || IsPotentialClosure(decompiledTypeDefinition, method.DeclaringTypeDefinition)))
				return false;
			return true;
		}
		
		static bool IsPotentialClosure(ITypeDefinition decompiledTypeDefinition, ITypeDefinition potentialDisplayClass)
		{
			if (potentialDisplayClass == null || !potentialDisplayClass.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			while (potentialDisplayClass != decompiledTypeDefinition) {
				potentialDisplayClass = potentialDisplayClass.DeclaringTypeDefinition;
				if (potentialDisplayClass == null)
					return false;
			}
			return true;
		}
		
		ILFunction TransformDelegateConstruction(NewObj value, out ILInstruction target)
		{
			target = null;
			if (!IsDelegateConstruction(value))
				return null;
			var targetMethod = ((IInstructionWithMethodOperand)value.Arguments[1]).Method;
			if (IsAnonymousMethod(decompilationContext.CurrentTypeDefinition, targetMethod)) {
				target = value.Arguments[0];
				var localTypeSystem = context.TypeSystem.GetSpecializingTypeSystem(new SimpleTypeResolveContext(targetMethod));
				var function = ILFunction.Read(localTypeSystem, targetMethod, context.CancellationToken);
				
				var contextPrefix = targetMethod.Name;
				foreach (ILVariable v in function.Variables.Where(v => v.Kind != VariableKind.Parameter)) {
					v.Name = contextPrefix + v.Name;
				}
				
				function.RunTransforms(CSharpDecompiler.GetILTransforms(), context, t => t is DelegateConstruction);
				function.AcceptVisitor(new ReplaceDelegateTargetVisitor(target, function.Variables.SingleOrDefault(v => v.Index == -1 && v.Kind == VariableKind.Parameter)));
				// handle nested lambdas
				((IILTransform)new DelegateConstruction()).Run(function, new ILTransformContext { Settings = context.Settings, CancellationToken = context.CancellationToken, TypeSystem = localTypeSystem });
				return function;
			}
			return null;
		}
		
		class ReplaceDelegateTargetVisitor : ILVisitor
		{
			readonly ILVariable thisVariable;
			readonly ILInstruction target;
			
			public ReplaceDelegateTargetVisitor(ILInstruction target, ILVariable thisVariable)
			{
				this.target = target;
				this.thisVariable = thisVariable;
			}
			
			protected override void Default(ILInstruction inst)
			{
				foreach (var child in inst.Children) {
					child.AcceptVisitor(this);
				}
			}
			
			protected internal override void VisitLdLoc(LdLoc inst)
			{
				if (inst.MatchLdLoc(thisVariable)) {
					inst.ReplaceWith(target.Clone());
					return;
				}
				base.VisitLdLoc(inst);
			}
		}
		
		class TransformDisplayClassUsages : ILVisitor
		{
			ILFunction currentFunction;
			readonly IInstructionWithVariableOperand targetLoad;
			readonly List<ILVariable> targetAndCopies = new List<ILVariable>();
			readonly List<ILInstruction> orphanedVariableInits;
			readonly Dictionary<IField, DisplayClassVariable> initValues = new Dictionary<IField, DisplayClassVariable>();
			
			struct DisplayClassVariable
			{
				public ILVariable variable;
				public ILInstruction value;
			}
			
			public TransformDisplayClassUsages(IInstructionWithVariableOperand targetLoad, List<ILInstruction> orphanedVariableInits)
			{
				this.targetLoad = targetLoad;
				this.orphanedVariableInits = orphanedVariableInits;
				this.targetAndCopies.Add(targetLoad.Variable);
			}
			
			protected override void Default(ILInstruction inst)
			{
				foreach (var child in inst.Children) {
					child.AcceptVisitor(this);
				}
			}
			
			protected internal override void VisitILFunction(ILFunction function)
			{
				var old = currentFunction;
				currentFunction = function;
				try {
					base.VisitILFunction(function);
				} finally {
					currentFunction = old;
				}
			}
			
			protected internal override void VisitStLoc(StLoc inst)
			{
				base.VisitStLoc(inst);
				if (inst.Variable == targetLoad.Variable)
					orphanedVariableInits.Add(inst);
				if (MatchesTargetOrCopyLoad(inst.Value)) {
					targetAndCopies.Add(inst.Variable);
					orphanedVariableInits.Add(inst);
				}
			}
			
			bool MatchesTargetOrCopyLoad(ILInstruction inst)
			{
				return targetAndCopies.Any(v => inst.MatchLdLoc(v));
			}
			
			protected internal override void VisitStObj(StObj inst)
			{
				base.VisitStObj(inst);
				ILInstruction target;
				IField field;
				if (!inst.Target.MatchLdFlda(out target, out field) || !MatchesTargetOrCopyLoad(target))
					return;
				field = (IField)field.MemberDefinition;
				DisplayClassVariable info;
				if (initValues.TryGetValue(field, out info) && info.variable != null) {
					inst.ReplaceWith(new StLoc(info.variable, inst.Value));
				} else {
					ILInstruction value;
					ILVariable v;
					if (inst.Value.MatchLdLoc(out v)) {
						orphanedVariableInits.Add(inst);
						value = inst.Value;
					} else {
						v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
						inst.ReplaceWith(new StLoc(v, inst.Value));
						value = new LdLoc(v);
					}
					initValues.Add(field, new DisplayClassVariable { value = value, variable = v });
				}
			}
			
			protected internal override void VisitLdObj(LdObj inst)
			{
				base.VisitLdObj(inst);
				ILInstruction target;
				IField field;
				if (!inst.Target.MatchLdFlda(out target, out field) || !MatchesTargetOrCopyLoad(target))
					return;
				DisplayClassVariable info;
				if (!initValues.TryGetValue((IField)field.MemberDefinition, out info))
					return;
				inst.ReplaceWith(info.value.Clone());
			}
			
			protected internal override void VisitLdFlda(LdFlda inst)
			{
				base.VisitLdFlda(inst);
				if (inst.Parent is LdObj || inst.Parent is StObj)
					return;
				if (!MatchesTargetOrCopyLoad(inst.Target))
					return;
				var field = (IField)inst.Field.MemberDefinition;
				DisplayClassVariable info;
				if (!initValues.TryGetValue(field, out info)) {
					var v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
					inst.ReplaceWith(new LdLoca(v));
					var value = new LdLoc(v);
					initValues.Add(field, new DisplayClassVariable { value = value, variable = v });
				} else if (info.value is LdLoc) {
					inst.ReplaceWith(new LdLoca(((LdLoc)info.value).Variable));
				} else {
					throw new NotImplementedException();
				}
			}
		}
		#endregion

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
			if (!IsDelegateConstruction(value as NewObj, true))
				return false;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var usages = nextInstruction.Descendants.Where(i => i.MatchLdsFld(field)).ToArray();
			if (usages.Length != 1)
				return false;
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
			var condition = inst.Condition as Comp;
			hasFieldStore = false;
			local = null;
			if (condition == null || trueInst == null || (trueInst.Instructions.Count < 1) || !inst.FalseInst.MatchNop())
				return false;
			ILVariable v;
			ILInstruction value, value2;
			var storeInst = trueInst.Instructions.Last();
			if (!condition.Left.MatchLdLoc(out v) || !condition.Right.MatchLdNull())
				return false;
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
			if (!IsDelegateConstruction(value as NewObj, true))
				return false;
			var nextInstruction = inst.Parent.Children.ElementAtOrDefault(inst.ChildIndex + 1);
			if (nextInstruction == null)
				return false;
			var usages = nextInstruction.Descendants.OfType<LdLoc>().Where(i => i.Variable == v).ToArray();
			if (usages.Length != 1)
				return false;
			local = v;
			usages[0].ReplaceWith(value);
			return true;
		}
	}
}
