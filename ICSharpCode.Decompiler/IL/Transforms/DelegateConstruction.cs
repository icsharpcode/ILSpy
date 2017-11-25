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
using ICSharpCode.Decompiler.TypeSystem;

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
			this.decompilationContext = new SimpleTypeResolveContext(function.Method);
			var orphanedVariableInits = new List<ILInstruction>();
			var targetsToReplace = new List<IInstructionWithVariableOperand>();
			var translatedDisplayClasses = new HashSet<ITypeDefinition>();
			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					context.CancellationToken.ThrowIfCancellationRequested();
					foreach (var call in block.Instructions[i].Descendants.OfType<NewObj>()) {
						ILFunction f = TransformDelegateConstruction(call, out ILInstruction target);
						if (f != null) {
							call.ReplaceWith(f);
							if (target is IInstructionWithVariableOperand && !target.MatchLdThis())
								targetsToReplace.Add((IInstructionWithVariableOperand)target);
						}
					}
					if (block.Instructions[i].MatchStLoc(out ILVariable targetVariable, out ILInstruction value)) {
						var newObj = value as NewObj;
						// TODO : it is probably not a good idea to remove *all* display-classes
						// is there a way to minimize the false-positives?
						if (newObj != null && IsInSimpleDisplayClass(newObj.Method)) {
							targetVariable.CaptureScope = FindBlockContainer(block);
							targetsToReplace.Add((IInstructionWithVariableOperand)block.Instructions[i]);
							translatedDisplayClasses.Add(newObj.Method.DeclaringTypeDefinition);
						}
					}
				}
			}
			foreach (var target in targetsToReplace.OrderByDescending(t => ((ILInstruction)t).ILRange.Start)) {
				function.AcceptVisitor(new TransformDisplayClassUsages(function, target, target.Variable.CaptureScope, orphanedVariableInits, translatedDisplayClasses));
			}
			foreach (var store in orphanedVariableInits) {
				if (store.Parent is Block containingBlock)
					containingBlock.Instructions.Remove(store);
			}
		}

		private BlockContainer FindBlockContainer(Block block)
		{
			return BlockContainer.FindClosestContainer(block) ?? throw new NotSupportedException();
		}

		static bool IsInSimpleDisplayClass(IMethod method)
		{
			if (!method.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			return IsSimpleDisplayClass(method.DeclaringType);
		}

		internal static bool IsSimpleDisplayClass(IType type)
		{
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

		internal static bool IsPotentialClosure(ILTransformContext context, NewObj inst)
		{
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			return IsPotentialClosure(decompilationContext.CurrentTypeDefinition, inst.Method.DeclaringTypeDefinition);
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
				var methodDefinition = (Mono.Cecil.MethodDefinition)context.TypeSystem.GetCecil(targetMethod);
				var localTypeSystem = context.TypeSystem.GetSpecializingTypeSystem(new SimpleTypeResolveContext(targetMethod));
				var ilReader = new ILReader(localTypeSystem);
				ilReader.UseDebugSymbols = context.Settings.UseDebugSymbols;
				var function = ilReader.ReadIL(methodDefinition.Body, context.CancellationToken);
				function.DelegateType = value.Method.DeclaringType;
				function.CheckInvariant(ILPhase.Normal);

				var contextPrefix = targetMethod.Name;
				foreach (ILVariable v in function.Variables.Where(v => v.Kind != VariableKind.Parameter)) {
					v.Name = contextPrefix + v.Name;
				}

				var nestedContext = new ILTransformContext(function, localTypeSystem, context.Settings) {
					CancellationToken = context.CancellationToken
				};
				function.RunTransforms(CSharpDecompiler.GetILTransforms().TakeWhile(t => !(t is DelegateConstruction)), nestedContext);
				function.AcceptVisitor(new ReplaceDelegateTargetVisitor(target, function.Variables.SingleOrDefault(v => v.Index == -1 && v.Kind == VariableKind.Parameter)));
				// handle nested lambdas
				((IILTransform)new DelegateConstruction()).Run(function, nestedContext);
				function.AddILRange(target.ILRange);
				function.AddILRange(value.Arguments[1].ILRange);
				return function;
			}
			return null;
		}
		
		/// <summary>
		/// Replaces loads of 'this' with the target expression.
		/// Async delegates use: ldobj(ldloca this).
		/// </summary>
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
				if (inst.Variable == thisVariable) {
					inst.ReplaceWith(target.Clone());
					return;
				}
				base.VisitLdLoc(inst);
			}

			protected internal override void VisitLdObj(LdObj inst)
			{
				if (inst.Target.MatchLdLoca(thisVariable)) {
					inst.ReplaceWith(target.Clone());
					return;
				}
				base.VisitLdObj(inst);
			}
		}
		
		/// <summary>
		/// 1. Stores to display class fields are replaced with stores to local variables (in some
		///    cases existing variables are used; otherwise fresh variables are added to the
		///    ILFunction-container) and all usages of those fields are replaced with the local variable.
		///    (see initValues)
		/// 2. Usages of the display class container (or any copy) are removed.
		/// </summary>
		class TransformDisplayClassUsages : ILVisitor
		{
			ILFunction currentFunction;
			BlockContainer captureScope;
			readonly IInstructionWithVariableOperand targetLoad;
			readonly List<ILVariable> targetAndCopies = new List<ILVariable>();
			readonly List<ILInstruction> orphanedVariableInits;
			readonly HashSet<ITypeDefinition> translatedDisplayClasses;
			readonly Dictionary<IField, DisplayClassVariable> initValues = new Dictionary<IField, DisplayClassVariable>();
			
			struct DisplayClassVariable
			{
				public ILVariable variable;
				public ILInstruction value;
			}
			
			public TransformDisplayClassUsages(ILFunction function, IInstructionWithVariableOperand targetLoad, BlockContainer captureScope, List<ILInstruction> orphanedVariableInits, HashSet<ITypeDefinition> translatedDisplayClasses)
			{
				this.currentFunction = function;
				this.targetLoad = targetLoad;
				this.captureScope = captureScope;
				this.orphanedVariableInits = orphanedVariableInits;
				this.translatedDisplayClasses = translatedDisplayClasses;
				this.targetAndCopies.Add(targetLoad.Variable);
			}
			
			protected override void Default(ILInstruction inst)
			{
				foreach (var child in inst.Children) {
					child.AcceptVisitor(this);
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
				if (!inst.Target.MatchLdFlda(out ILInstruction target, out IField field) || !MatchesTargetOrCopyLoad(target))
					return;
				field = (IField)field.MemberDefinition;
				ILInstruction value;
				if (initValues.TryGetValue(field, out DisplayClassVariable info)) {
					inst.ReplaceWith(new StLoc(info.variable, inst.Value));
				} else {
					if (inst.Value.MatchLdLoc(out var v) && v.Kind == VariableKind.Parameter && currentFunction == v.Function) {
						// special case for parameters: remove copies of parameter values.
						orphanedVariableInits.Add(inst);
						value = inst.Value;
					} else {
						if (!translatedDisplayClasses.Contains(field.DeclaringTypeDefinition))
							return;
						v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
						v.CaptureScope = captureScope;
						inst.ReplaceWith(new StLoc(v, inst.Value));
						value = new LdLoc(v);
					}
					initValues.Add(field, new DisplayClassVariable { value = value, variable = v });
				}
			}
			
			protected internal override void VisitLdObj(LdObj inst)
			{
				base.VisitLdObj(inst);
				if (!inst.Target.MatchLdFlda(out ILInstruction target, out IField field))
					return;
				if (!initValues.TryGetValue((IField)field.MemberDefinition, out DisplayClassVariable info))
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
				if (!initValues.TryGetValue(field, out DisplayClassVariable info)) {
					if (!translatedDisplayClasses.Contains(field.DeclaringTypeDefinition))
						return;
					var v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
					v.CaptureScope = captureScope;
					inst.ReplaceWith(new LdLoca(v));
					var value = new LdLoc(v);
					initValues.Add(field, new DisplayClassVariable { value = value, variable = v });
				} else if (info.value is LdLoc l) {
					inst.ReplaceWith(new LdLoca(l.Variable));
				} else {
					throw new NotImplementedException();
				}
			}

			protected internal override void VisitCompoundAssignmentInstruction(CompoundAssignmentInstruction inst)
			{
				base.VisitCompoundAssignmentInstruction(inst);
				if (inst.Target.MatchLdLoc(out var v)) {
					inst.ReplaceWith(new StLoc(v, new BinaryNumericInstruction(inst.Operator, inst.Target, inst.Value, inst.CheckForOverflow, inst.Sign)));
		}
			}
		}
		#endregion
	}
}
