// Copyright (c) 2019 Siegfried Pammer
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
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms closure fields to local variables.
	/// 
	/// This is a post-processing step of <see cref="DelegateConstruction"/>, <see cref="LocalFunctionDecompiler"/> and <see cref="TransformExpressionTrees"/>.
	/// </summary>
	class TransformDisplayClassUsage : ILVisitor, IILTransform
	{
		class DisplayClass
		{
			public bool IsMono;
			public ILInstruction Initializer;
			public ILVariable Variable;
			public ITypeDefinition Definition;
			public Dictionary<IField, DisplayClassVariable> Variables;
			public BlockContainer CaptureScope;
			public ILFunction DeclaringFunction;
		}

		struct DisplayClassVariable
		{
			public ILVariable Variable;
			public ILInstruction Value;
		}

		ILTransformContext context;
		ILFunction currentFunction;
		readonly Dictionary<ILVariable, DisplayClass> displayClasses = new Dictionary<ILVariable, DisplayClass>();
		readonly List<ILInstruction> instructionsToRemove = new List<ILInstruction>();

		public void Run(ILFunction function, ILTransformContext context)
		{
			try {
				if (this.context != null || this.currentFunction != null)
					throw new InvalidOperationException("Reentrancy in " + nameof(TransformDisplayClassUsage));
				this.context = context;
				var decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
				// Traverse nested functions in post-order:
				// Inner functions are transformed before outer functions
				foreach (var f in function.Descendants.OfType<ILFunction>()) {
					foreach (var v in f.Variables.ToArray()) {
						if (HandleMonoStateMachine(function, v, decompilationContext, f))
							continue;
						if (IsClosure(v, out ITypeDefinition closureType, out var inst)) {
							AddOrUpdateDisplayClass(f, v, closureType, inst, localFunctionClosureParameter: false);
						}
						if (context.Settings.LocalFunctions && f.Kind == ILFunctionKind.LocalFunction && v.Kind == VariableKind.Parameter && v.Index > -1 && f.Method.Parameters[v.Index.Value] is IParameter p && LocalFunctionDecompiler.IsClosureParameter(p, decompilationContext)) {
							AddOrUpdateDisplayClass(f, v, ((ByReferenceType)p.Type).ElementType.GetDefinition(), f.Body, localFunctionClosureParameter: true);
						}
					}
					foreach (var displayClass in displayClasses.Values.OrderByDescending(d => d.Initializer.StartILOffset).ToArray()) {
						context.Step($"Transform references to " + displayClass.Variable, displayClass.Initializer);
						this.currentFunction = f;
						VisitILFunction(f);
					}
				}
				if (instructionsToRemove.Count > 0) {
					context.Step($"Remove instructions", function);
					foreach (var store in instructionsToRemove) {
						if (store.Parent is Block containingBlock)
							containingBlock.Instructions.Remove(store);
					}
				}
				RemoveDeadVariableInit.ResetHasInitialValueFlag(function, context);
			} finally {
				instructionsToRemove.Clear();
				displayClasses.Clear();
				this.context = null;
				this.currentFunction = null;
			}
		}

		private void AddOrUpdateDisplayClass(ILFunction f, ILVariable v, ITypeDefinition closureType, ILInstruction inst, bool localFunctionClosureParameter)
		{
			var displayClass = displayClasses.Values.FirstOrDefault(c => c.Definition == closureType);
			// TODO : figure out whether it is a mono compiled closure, without relying on the type name
			bool isMono = f.StateMachineCompiledWithMono || closureType.Name.Contains("AnonStorey");
			if (displayClass == null) {
				displayClasses.Add(v, new DisplayClass {
					IsMono = isMono,
					Initializer = inst,
					Variable = v,
					Definition = closureType,
					Variables = new Dictionary<IField, DisplayClassVariable>(),
					CaptureScope = (isMono && IsMonoNestedCaptureScope(closureType)) || localFunctionClosureParameter ? null : v.CaptureScope,
					DeclaringFunction = localFunctionClosureParameter ? f.DeclarationScope.Ancestors.OfType<ILFunction>().First() : f
				});
			} else {
				if (displayClass.CaptureScope == null && !localFunctionClosureParameter)
					displayClass.CaptureScope = isMono && IsMonoNestedCaptureScope(closureType) ? null : v.CaptureScope;
				displayClass.Variable = v;
				displayClass.Initializer = inst;
				displayClasses.Add(v, displayClass);
			}
		}

		bool IsClosure(ILVariable variable, out ITypeDefinition closureType, out ILInstruction initializer)
		{
			closureType = null;
			initializer = null;
			if (variable.IsSingleDefinition && variable.StoreInstructions.SingleOrDefault() is StLoc inst) {
				initializer = inst;
				if (IsClosureInit(inst, out closureType)) {
					instructionsToRemove.Add(inst);
					return true;
				}
			}
			closureType = variable.Type.GetDefinition();
			if (context.Settings.LocalFunctions && closureType?.Kind == TypeKind.Struct && variable.HasInitialValue && IsPotentialClosure(this.context, closureType)) {
				initializer = LocalFunctionDecompiler.GetStatement(variable.AddressInstructions.OrderBy(i => i.StartILOffset).First());
				return true;
			}
			return false;
		}

		bool IsClosureInit(StLoc inst, out ITypeDefinition closureType)
		{
			if (inst.Value is NewObj newObj) {
				closureType = newObj.Method.DeclaringTypeDefinition;
				return closureType != null && IsPotentialClosure(this.context, newObj);
			}
			closureType = null;
			return false;
		}

		bool IsOuterClosureReference(IField field)
		{
			return displayClasses.Values.Any(disp => disp.Definition == field.DeclaringTypeDefinition);
		}

		bool IsMonoNestedCaptureScope(ITypeDefinition closureType)
		{
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			return closureType.Fields.Any(f => IsPotentialClosure(decompilationContext.CurrentTypeDefinition, f.ReturnType.GetDefinition()));
		}

		/// <summary>
		/// mcs likes to optimize closures in yield state machines away by moving the captured variables' fields into the state machine type,
		/// We construct a <see cref="DisplayClass"/> that spans the whole method body.
		/// </summary>
		bool HandleMonoStateMachine(ILFunction currentFunction, ILVariable thisVariable, SimpleTypeResolveContext decompilationContext, ILFunction nestedFunction)
		{
			if (!(nestedFunction.StateMachineCompiledWithMono && thisVariable.IsThis()))
				return false;
			// Special case for Mono-compiled yield state machines
			ITypeDefinition closureType = thisVariable.Type.GetDefinition();
			if (!(closureType != decompilationContext.CurrentTypeDefinition
				&& IsPotentialClosure(decompilationContext.CurrentTypeDefinition, closureType)))
				return false;

			var displayClass = new DisplayClass {
				IsMono = true,
				Initializer = nestedFunction.Body,
				Variable = thisVariable,
				Definition = thisVariable.Type.GetDefinition(),
				Variables = new Dictionary<IField, DisplayClassVariable>(),
				CaptureScope = (BlockContainer)nestedFunction.Body
			};
			displayClasses.Add(thisVariable, displayClass);
			foreach (var stateMachineVariable in nestedFunction.Variables) {
				if (stateMachineVariable.StateMachineField == null)
					continue;
				displayClass.Variables.Add(stateMachineVariable.StateMachineField, new DisplayClassVariable {
					Variable = stateMachineVariable,
					Value = new LdLoc(stateMachineVariable)
				});
			}
			if (!currentFunction.Method.IsStatic && FindThisField(out var thisField)) {
				var thisVar = currentFunction.Variables
					.FirstOrDefault(t => t.IsThis() && t.Type.GetDefinition() == decompilationContext.CurrentTypeDefinition);
				if (thisVar == null) {
					thisVar = new ILVariable(VariableKind.Parameter, decompilationContext.CurrentTypeDefinition, -1) { Name = "this" };
					currentFunction.Variables.Add(thisVar);
				}
				displayClass.Variables.Add(thisField, new DisplayClassVariable { Variable = thisVar, Value = new LdLoc(thisVar) });
			}
			return true;

			bool FindThisField(out IField foundField)
			{
				foundField = null;
				foreach (var field in closureType.GetFields(f2 => !f2.IsStatic && !displayClass.Variables.ContainsKey(f2) && f2.Type.GetDefinition() == decompilationContext.CurrentTypeDefinition)) {
					thisField = field;
					return true;
				}
				return false;
			}
		}

		internal static bool IsSimpleDisplayClass(IType type)
		{
			if (!type.HasGeneratedName() || (!type.Name.Contains("DisplayClass") && !type.Name.Contains("AnonStorey")))
				return false;
			if (type.DirectBaseTypes.Any(t => !t.IsKnownType(KnownTypeCode.Object)))
				return false;
			return true;
		}

		internal static bool IsPotentialClosure(ILTransformContext context, NewObj inst)
		{
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Ancestors.OfType<ILFunction>().Last().Method);
			return IsPotentialClosure(decompilationContext.CurrentTypeDefinition, inst.Method.DeclaringTypeDefinition);
		}

		internal static bool IsPotentialClosure(ILTransformContext context, ITypeDefinition potentialDisplayClass)
		{
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Ancestors.OfType<ILFunction>().Last().Method);
			return IsPotentialClosure(decompilationContext.CurrentTypeDefinition, potentialDisplayClass);
		}

		internal static bool IsPotentialClosure(ITypeDefinition decompiledTypeDefinition, ITypeDefinition potentialDisplayClass)
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

		bool IsDisplayClassLoad(ILInstruction target, out ILVariable variable)
		{
			if (target.MatchLdLoc(out variable) || target.MatchLdLoca(out variable))
				return true;
			return false;
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
			// Sometimes display class references are copied into other local variables.
			// We remove the assignment and store the relationship between the display class and the variable in the
			// displayClasses dictionary.
			if (inst.Value.MatchLdLoc(out var closureVariable) && displayClasses.TryGetValue(closureVariable, out var displayClass)) {
				displayClasses[inst.Variable] = displayClass;
				instructionsToRemove.Add(inst);
			} else if (inst.Variable.Kind == VariableKind.Local && inst.Variable.IsSingleDefinition && inst.Variable.LoadCount == 0 && inst.Value is StLoc) {
				inst.ReplaceWith(inst.Value);
			}
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			// This instruction has been marked deletable, do not transform it further
			if (instructionsToRemove.Contains(inst))
				return;
			// The target of the store instruction must be a field reference
			if (!inst.Target.MatchLdFlda(out ILInstruction target, out IField field))
				return;
			// Get display class info
			if (!IsDisplayClassLoad(target, out var displayClassLoad) || !displayClasses.TryGetValue(displayClassLoad, out var displayClass))
				return;
			field = (IField)field.MemberDefinition;
			if (displayClass.Variables.TryGetValue(field, out DisplayClassVariable info)) {
				// If the display class field was previously initialized, we use a simple assignment.
				inst.ReplaceWith(new StLoc(info.Variable, inst.Value).WithILRange(inst));
			} else {
				// This is an uninitialized variable:
				ILInstruction value;
				if (inst.Value.MatchLdLoc(out var v) && v.Kind == VariableKind.Parameter && currentFunction == v.Function) {
					// Special case for parameters: remove copies of parameter values.
					instructionsToRemove.Add(inst);
					value = inst.Value;
				} else {
					Debug.Assert(displayClass.Definition == field.DeclaringTypeDefinition);
					// Introduce a fresh variable for the display class field.
					if (displayClass.IsMono && displayClass.CaptureScope == null && !IsOuterClosureReference(field)) {
						displayClass.CaptureScope = BlockContainer.FindClosestContainer(inst);
					}
					v = displayClass.DeclaringFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
					v.HasInitialValue = true;
					v.CaptureScope = displayClass.CaptureScope;
					inst.ReplaceWith(new StLoc(v, inst.Value).WithILRange(inst));
					value = new LdLoc(v);
				}
				displayClass.Variables.Add(field, new DisplayClassVariable { Value = value, Variable = v });
			}
		}

		protected internal override void VisitLdObj(LdObj inst)
		{
			base.VisitLdObj(inst);
			// The target of the store instruction must be a field reference
			if (!inst.Target.MatchLdFlda(out var target, out IField field))
				return;
			// Get display class info
			if (!IsDisplayClassLoad(target, out var displayClassLoad) || !displayClasses.TryGetValue(displayClassLoad, out var displayClass))
				return;
			// Get display class variable info
			if (!displayClass.Variables.TryGetValue((IField)field.MemberDefinition, out DisplayClassVariable info))
				return;
			// Replace usage of display class field with the variable.
			var replacement = info.Value.Clone();
			replacement.SetILRange(inst);
			inst.ReplaceWith(replacement);
		}

		protected internal override void VisitLdFlda(LdFlda inst)
		{
			base.VisitLdFlda(inst);
			// TODO : Figure out why this was added in https://github.com/icsharpcode/ILSpy/pull/1303
			if (inst.Target.MatchLdThis() && inst.Field.Name == "$this"
				&& inst.Field.MemberDefinition.ReflectionName.Contains("c__Iterator")) {
				//Debug.Assert(false, "This should not be executed!");
				var variable = currentFunction.Variables.First((f) => f.Index == -1);
				inst.ReplaceWith(new LdLoca(variable).WithILRange(inst));
			}
			// Skip stfld/ldfld
			if (inst.Parent is LdObj || inst.Parent is StObj)
				return;
			// Get display class info
			if (!IsDisplayClassLoad(inst.Target, out var displayClassLoad) || !displayClasses.TryGetValue(displayClassLoad, out var displayClass))
				return;
			var field = (IField)inst.Field.MemberDefinition;
			if (!displayClass.Variables.TryGetValue(field, out DisplayClassVariable info)) {
				// Introduce a fresh variable for the display class field.
				Debug.Assert(displayClass.Definition == field.DeclaringTypeDefinition);
				var v = displayClass.DeclaringFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
				v.HasInitialValue = true;
				v.CaptureScope = displayClass.CaptureScope;
				inst.ReplaceWith(new LdLoca(v).WithILRange(inst));
				displayClass.Variables.Add(field, new DisplayClassVariable { Value = new LdLoc(v), Variable = v });
			} else if (info.Value is LdLoc l) {
				inst.ReplaceWith(new LdLoca(l.Variable).WithILRange(inst));
			} else {
				Debug.Fail("LdFlda pattern not supported!");
			}
		}
	}
}
