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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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
			var cancellationToken = context.CancellationToken;
			foreach (var inst in function.Descendants) {
				cancellationToken.ThrowIfCancellationRequested();
				if (inst is NewObj call) {
					context.StepStartGroup($"TransformDelegateConstruction {call.StartILOffset}", call);
					ILFunction f = TransformDelegateConstruction(call, out ILInstruction target);
					if (f != null && target is IInstructionWithVariableOperand instWithVar) {
						if (instWithVar.Variable.Kind == VariableKind.Local) {
							instWithVar.Variable.Kind = VariableKind.DisplayClassLocal;
						}
						targetsToReplace.Add(instWithVar);
					}
					context.StepEndGroup();
				}
				if (inst.MatchStLoc(out ILVariable targetVariable, out ILInstruction value)) {
					var newObj = value as NewObj;
					// TODO : it is probably not a good idea to remove *all* display-classes
					// is there a way to minimize the false-positives?
					if (newObj != null && IsInSimpleDisplayClass(newObj.Method)) {
						targetVariable.CaptureScope = BlockContainer.FindClosestContainer(inst);
						targetsToReplace.Add((IInstructionWithVariableOperand)inst);
						translatedDisplayClasses.Add(newObj.Method.DeclaringTypeDefinition);
					}
				}
			}
			foreach (var target in targetsToReplace.OrderByDescending(t => ((ILInstruction)t).StartILOffset)) {
				context.Step($"TransformDisplayClassUsages {target.Variable}", (ILInstruction)target);
				function.AcceptVisitor(new TransformDisplayClassUsages(function, target, target.Variable.CaptureScope, orphanedVariableInits, translatedDisplayClasses));
			}
			context.Step($"Remove orphanedVariableInits", function);
			foreach (var store in orphanedVariableInits) {
				if (store.Parent is Block containingBlock)
					containingBlock.Instructions.Remove(store);
			}
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
			if (method == null || !(method.HasGeneratedName() || method.Name.Contains("$") || ContainsAnonymousType(method)))
				return false;
			if (!(method.IsCompilerGeneratedOrIsInCompilerGeneratedClass() || IsPotentialClosure(decompiledTypeDefinition, method.DeclaringTypeDefinition)))
				return false;
			return true;
		}

		static bool ContainsAnonymousType(IMethod method)
		{
			if (method.ReturnType.ContainsAnonymousType())
				return true;
			foreach (var p in method.Parameters) {
				if (p.Type.ContainsAnonymousType())
					return true;
			}
			return false;
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
		
		internal static GenericContext? GenericContextFromTypeArguments(TypeParameterSubstitution subst)
		{
			var classTypeParameters = new List<ITypeParameter>();
			var methodTypeParameters = new List<ITypeParameter>();
			if (subst.ClassTypeArguments != null) {
				foreach (var t in subst.ClassTypeArguments) {
					if (t is ITypeParameter tp)
						classTypeParameters.Add(tp);
					else
						return null;
				}
			}
			if (subst.MethodTypeArguments != null) {
				foreach (var t in subst.MethodTypeArguments) {
					if (t is ITypeParameter tp)
						methodTypeParameters.Add(tp);
					else
						return null;
				}
			}
			return new GenericContext(classTypeParameters, methodTypeParameters);
		}

		ILFunction TransformDelegateConstruction(NewObj value, out ILInstruction target)
		{
			target = null;
			if (!IsDelegateConstruction(value))
				return null;
			var targetMethod = ((IInstructionWithMethodOperand)value.Arguments[1]).Method;
			if (!IsAnonymousMethod(decompilationContext.CurrentTypeDefinition, targetMethod))
				return null;
			if (LocalFunctionDecompiler.IsLocalFunctionMethod(targetMethod.ParentModule.PEFile, (MethodDefinitionHandle)targetMethod.MetadataToken))
				return null;
			target = value.Arguments[0];
			if (targetMethod.MetadataToken.IsNil)
				return null;
			var methodDefinition = context.PEFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)targetMethod.MetadataToken);
			if (!methodDefinition.HasBody())
				return null;
			var genericContext = GenericContextFromTypeArguments(targetMethod.Substitution);
			if (genericContext == null)
				return null;
			var ilReader = context.CreateILReader();
			var body = context.PEFile.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
			var function = ilReader.ReadIL((MethodDefinitionHandle)targetMethod.MetadataToken, body, genericContext.Value, context.CancellationToken);
			function.DelegateType = value.Method.DeclaringType;
			function.CheckInvariant(ILPhase.Normal);
			// Embed the lambda into the parent function's ILAst, so that "Show steps" can show
			// how the lambda body is being transformed.
			value.ReplaceWith(function);

			var contextPrefix = targetMethod.Name;
			foreach (ILVariable v in function.Variables.Where(v => v.Kind != VariableKind.Parameter)) {
				v.Name = contextPrefix + v.Name;
			}

			var nestedContext = new ILTransformContext(context, function);
			function.RunTransforms(CSharpDecompiler.GetILTransforms().TakeWhile(t => !(t is DelegateConstruction)).Concat(GetTransforms()), nestedContext);
			nestedContext.Step("DelegateConstruction (ReplaceDelegateTargetVisitor)", function);
			function.AcceptVisitor(new ReplaceDelegateTargetVisitor(target, function.Variables.SingleOrDefault(v => v.Index == -1 && v.Kind == VariableKind.Parameter)));
			// handle nested lambdas
			nestedContext.StepStartGroup("DelegateConstruction (nested lambdas)", function);
			((IILTransform)new DelegateConstruction()).Run(function, nestedContext);
			nestedContext.StepEndGroup();
			function.AddILRange(target);
			function.AddILRange(value);
			function.AddILRange(value.Arguments[1]);
			return function;
		}

		private IEnumerable<IILTransform> GetTransforms()
		{
			yield return new CombineExitsTransform();
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
				if (targetLoad is ILInstruction instruction && instruction.MatchLdThis())
					return;
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
				if (!inst.Target.MatchLdFlda(out ILInstruction target, out IField field) || !MatchesTargetOrCopyLoad(target) || target.MatchLdThis())
					return;
				field = (IField)field.MemberDefinition;
				ILInstruction value;
				if (initValues.TryGetValue(field, out DisplayClassVariable info)) {
					inst.ReplaceWith(new StLoc(info.variable, inst.Value).WithILRange(inst));
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
						inst.ReplaceWith(new StLoc(v, inst.Value).WithILRange(inst));
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
				var replacement = info.value.Clone();
				replacement.SetILRange(inst);
				inst.ReplaceWith(replacement);
			}
			
			protected internal override void VisitLdFlda(LdFlda inst)
			{
				base.VisitLdFlda(inst);
				if (inst.Target.MatchLdThis() && inst.Field.Name == "$this"
					&& inst.Field.MemberDefinition.ReflectionName.Contains("c__Iterator")) {
					var variable = currentFunction.Variables.First((f) => f.Index == -1);
					inst.ReplaceWith(new LdLoca(variable).WithILRange(inst));
				}
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
					inst.ReplaceWith(new LdLoca(v).WithILRange(inst));
					var value = new LdLoc(v);
					initValues.Add(field, new DisplayClassVariable { value = value, variable = v });
				} else if (info.value is LdLoc l) {
					inst.ReplaceWith(new LdLoca(l.Variable).WithILRange(inst));
				} else {
					Debug.Fail("LdFlda pattern not supported!");
				}
			}

			protected internal override void VisitNumericCompoundAssign(NumericCompoundAssign inst)
			{
				base.VisitNumericCompoundAssign(inst);
				if (inst.Target.MatchLdLoc(out var v)) {
					inst.ReplaceWith(new StLoc(v, new BinaryNumericInstruction(inst.Operator, inst.Target, inst.Value, inst.CheckForOverflow, inst.Sign).WithILRange(inst)));
				}
			}
		}
		#endregion
	}
}
