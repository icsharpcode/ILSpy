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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms closure fields to local variables.
	/// 
	/// This is a post-processing step of <see cref="DelegateConstruction"/>, <see cref="LocalFunctionDecompiler"/>
	/// and <see cref="TransformExpressionTrees"/>.
	/// 
	/// In general we can perform SROA (scalar replacement of aggregates) on any variable that
	/// satisfies the following conditions:
	/// 1) It is initialized by an empty/default constructor call.
	/// 2) The variable is never passed to another method.
	/// 3) The variable is never the target of an invocation.
	/// 
	/// Note that 2) and 3) apply because declarations and uses of lambdas and local functions
	/// are already transformed by the time this transform is applied.
	/// </summary>
	class TransformDisplayClassUsage : ILVisitor, IILTransform
	{
		[Flags]
		enum Kind
		{
			DelegateConstruction = 1,
			LocalFunction = 2,
			ExpressionTree = 4,
			MonoStateMachine = 8
		}

		class VariableToDeclare
		{
			private readonly DisplayClass container;
			private readonly IField field;
			private ILVariable declaredVariable;

			public string Name => field.Name;

			public List<ILInstruction> AssignedValues { get; } = new List<ILInstruction>();

			public VariableToDeclare(DisplayClass container, IField field)
			{
				this.container = container;
				this.field = field;
			}

			public void UseExisting(ILVariable variable)
			{
				Debug.Assert(this.declaredVariable == null);
				this.declaredVariable = variable;
			}

			public ILVariable GetOrDeclare()
			{
				if (declaredVariable != null)
					return declaredVariable;
				declaredVariable = container.Variable.Function.RegisterVariable(VariableKind.Local, field.Type, field.Name);
				declaredVariable.HasInitialValue = true;
				declaredVariable.CaptureScope = container.CaptureScope;
				return declaredVariable;
			}
		}

		class DisplayClass
		{
			public readonly ILVariable Variable;
			public readonly ITypeDefinition Type;
			public readonly Dictionary<IField, VariableToDeclare> VariablesToDeclare;
			public BlockContainer CaptureScope;
			public Kind Kind;

			public DisplayClass(ILVariable variable, ITypeDefinition type)
			{
				Variable = variable;
				Type = type;
				VariablesToDeclare = new Dictionary<IField, VariableToDeclare>();
			}
		}

		ILTransformContext context;
		ITypeResolveContext decompilationContext;
		readonly Dictionary<ILVariable, DisplayClass> displayClasses = new Dictionary<ILVariable, DisplayClass>();
		readonly List<ILInstruction> instructionsToRemove = new List<ILInstruction>();

		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (this.context != null)
				throw new InvalidOperationException("Reentrancy in " + nameof(TransformDisplayClassUsage));
			try {
				this.context = context;
				this.decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
				AnalyzeFunction(function);
				Transform(function);
			} finally {
				ClearState();
			}
		}

		void ClearState()
		{
			instructionsToRemove.Clear();
			displayClasses.Clear();
			this.decompilationContext = null;
			this.context = null;
		}

		public void Analyze(ILFunction function, ILTransformContext context)
		{
			ClearState();
			this.context = context;
			this.decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			AnalyzeFunction(function);
		}

		private void AnalyzeFunction(ILFunction function)
		{
			// Traverse nested functions in post-order:
			// Inner functions are transformed before outer functions
			foreach (var f in function.Descendants.OfType<ILFunction>()) {
				foreach (var v in f.Variables.ToArray()) {
					var result = AnalyzeVariable(v);
					if (result == null)
						continue;
					displayClasses.Add(v, result);
				}
			}
		}

		private DisplayClass AnalyzeVariable(ILVariable v)
		{
			ITypeDefinition definition;
			switch (v.Kind) {
				case VariableKind.Parameter:
					if (context.Settings.YieldReturn && v.Function.StateMachineCompiledWithMono && v.IsThis())
						return HandleMonoStateMachine(v.Function, v);
					return null;
				case VariableKind.StackSlot:
				case VariableKind.Local:
				case VariableKind.DisplayClassLocal:
					if (!AnalyzeInitializer(out definition))
						return null;
					return Run(definition);
				case VariableKind.PinnedLocal:
				case VariableKind.UsingLocal:
				case VariableKind.ForeachLocal:
				case VariableKind.InitializerTarget:
				case VariableKind.NamedArgument:
				case VariableKind.ExceptionStackSlot:
				case VariableKind.ExceptionLocal:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}

			DisplayClass Run(ITypeDefinition type)
			{
				var result = new DisplayClass(v, type) {
					CaptureScope = v.CaptureScope
				};
				foreach (var ldloc in v.LoadInstructions) {
					if (!ValidateUse(result, ldloc))
						return null;
				}
				foreach (var ldloca in v.AddressInstructions) {
					if (!ValidateUse(result, ldloca))
						return null;
				}
				if (result.VariablesToDeclare.Count == 0)
					return null;
				if (IsMonoNestedCaptureScope(type)) {
					result.CaptureScope = null;
				}
				return result;
			}

			bool ValidateUse(DisplayClass result, ILInstruction use)
			{
				var current = use.Parent;
				ILInstruction value;
				if (current.MatchLdFlda(out _, out IField field)) {
					var keyField = (IField)field.MemberDefinition;
					result.VariablesToDeclare.TryGetValue(keyField, out VariableToDeclare variable);
					if (current.Parent.MatchStObj(out _, out value, out var type)) {
						if (variable == null) {
							variable = new VariableToDeclare(result, field);
						}
						variable.AssignedValues.Add(value);
					}

					var containingFunction = current.Ancestors.OfType<ILFunction>().FirstOrDefault();
					if (containingFunction != null && containingFunction != v.Function) {
						switch (containingFunction.Kind) {
							case ILFunctionKind.Delegate:
								result.Kind |= Kind.DelegateConstruction;
								break;
							case ILFunctionKind.ExpressionTree:
								result.Kind |= Kind.ExpressionTree;
								break;
							case ILFunctionKind.LocalFunction:
								result.Kind |= Kind.LocalFunction;
								break;
						}
					}
					result.VariablesToDeclare[keyField] = variable;
					return true;
				}
				if (current.MatchStObj(out _, out value, out _) && value == use)
					return true;
				return false;
			}

			bool AnalyzeInitializer(out ITypeDefinition t)
			{
				if (v.Kind != VariableKind.StackSlot) {
					t = v.Type.GetDefinition();
				} else if (v.StoreInstructions.Count > 0 && v.StoreInstructions[0] is StLoc stloc && stloc.Value is NewObj newObj) {
					t = newObj.Method.DeclaringTypeDefinition;
				} else {
					t = null;
				}
				if (t?.DeclaringTypeDefinition == null || t.ParentModule.PEFile != context.PEFile)
					return false;
				switch (definition.Kind) {
					case TypeKind.Class:
						if (!v.IsSingleDefinition)
							return false;
						if (!(v.StoreInstructions[0] is StLoc stloc))
							return false;
						if (!(stloc.Value is NewObj newObj && newObj.Method.Parameters.Count == 0))
							return false;
						break;
					case TypeKind.Struct:
						if (!v.HasInitialValue)
							return false;
						if (v.StoreCount != 1)
							return false;
						break;
				}

				return true;
			}
		}

		private void Transform(ILFunction function)
		{
			VisitILFunction(function);
			if (instructionsToRemove.Count > 0) {
				context.Step($"Remove instructions", function);
				foreach (var store in instructionsToRemove) {
					if (store.Parent is Block containingBlock)
						containingBlock.Instructions.Remove(store);
				}
			}
			context.Step($"ResetHasInitialValueFlag", function);
			foreach (var f in TreeTraversal.PostOrder(function, f => f.LocalFunctions))
				RemoveDeadVariableInit.ResetHasInitialValueFlag(f, context);
		}

		internal static bool IsClosure(ILTransformContext context, ILVariable variable, out ITypeDefinition closureType, out ILInstruction initializer)
		{
			closureType = null;
			initializer = null;
			if (variable.IsSingleDefinition && variable.StoreInstructions.SingleOrDefault() is StLoc inst) {
				initializer = inst;
				if (IsClosureInit(context, inst, out closureType)) {
					return true;
				}
			}
			closureType = variable.Type.GetDefinition();
			if (context.Settings.LocalFunctions && closureType?.Kind == TypeKind.Struct && variable.HasInitialValue && IsPotentialClosure(context, closureType)) {
				initializer = LocalFunctionDecompiler.GetStatement(variable.AddressInstructions.OrderBy(i => i.StartILOffset).First());
				return true;
			}
			return false;
		}

		static bool IsClosureInit(ILTransformContext context, StLoc inst, out ITypeDefinition closureType)
		{
			if (inst.Value is NewObj newObj) {
				closureType = newObj.Method.DeclaringTypeDefinition;
				return closureType != null && IsPotentialClosure(context, newObj);
			}
			closureType = null;
			return false;
		}

		bool IsMonoNestedCaptureScope(ITypeDefinition closureType)
		{
			if (!closureType.Name.Contains("AnonStorey"))
				return false;
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			return closureType.Fields.Any(f => IsPotentialClosure(decompilationContext.CurrentTypeDefinition, f.ReturnType.GetDefinition()));
		}

		/// <summary>
		/// mcs likes to optimize closures in yield state machines away by moving the captured variables' fields into the state machine type,
		/// We construct a <see cref="DisplayClass"/> that spans the whole method body.
		/// </summary>
		DisplayClass HandleMonoStateMachine(ILFunction function, ILVariable thisVariable)
		{
			if (!(function.StateMachineCompiledWithMono && thisVariable.IsThis()))
				return null;
			// Special case for Mono-compiled yield state machines
			ITypeDefinition closureType = thisVariable.Type.GetDefinition();
			if (!(closureType != decompilationContext.CurrentTypeDefinition
				&& IsPotentialClosure(decompilationContext.CurrentTypeDefinition, closureType, allowTypeImplementingInterfaces: true)))
				return null;

			var displayClass = new DisplayClass(thisVariable, thisVariable.Type.GetDefinition());
			displayClass.CaptureScope = (BlockContainer)function.Body;
			foreach (var stateMachineVariable in function.Variables) {
				if (stateMachineVariable.StateMachineField == null || displayClass.VariablesToDeclare.ContainsKey(stateMachineVariable.StateMachineField))
					continue;
				VariableToDeclare variableToDeclare = new VariableToDeclare(displayClass, stateMachineVariable.StateMachineField);
				variableToDeclare.UseExisting(stateMachineVariable);
				displayClass.VariablesToDeclare.Add(stateMachineVariable.StateMachineField, variableToDeclare);
			}
			if (!function.Method.IsStatic && FindThisField(out var thisField)) {
				var thisVar = function.Variables
					.FirstOrDefault(t => t.IsThis() && t.Type.GetDefinition() == decompilationContext.CurrentTypeDefinition);
				if (thisVar == null) {
					thisVar = new ILVariable(VariableKind.Parameter, decompilationContext.CurrentTypeDefinition, -1) { Name = "this" };
					function.Variables.Add(thisVar);
				}
				VariableToDeclare variableToDeclare = new VariableToDeclare(displayClass, thisField);
				variableToDeclare.UseExisting(thisVar);
				displayClass.VariablesToDeclare.Add(thisField, variableToDeclare);
			}
			return displayClass;

			bool FindThisField(out IField foundField)
			{
				foundField = null;
				foreach (var field in closureType.GetFields(f2 => !f2.IsStatic && !displayClass.VariablesToDeclare.ContainsKey(f2) && f2.Type.GetDefinition() == decompilationContext.CurrentTypeDefinition)) {
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

		internal static bool IsPotentialClosure(ITypeDefinition decompiledTypeDefinition, ITypeDefinition potentialDisplayClass, bool allowTypeImplementingInterfaces = false)
		{
			if (potentialDisplayClass == null || !potentialDisplayClass.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			switch (potentialDisplayClass.Kind) {
				case TypeKind.Struct:
					break;
				case TypeKind.Class:
					if (!potentialDisplayClass.IsSealed)
						return false;
					if (!allowTypeImplementingInterfaces) {
						if (!potentialDisplayClass.DirectBaseTypes.All(t => t.IsKnownType(KnownTypeCode.Object)))
							return false;
					}
					break;
				default:
					return false;
			}
			
			while (potentialDisplayClass != decompiledTypeDefinition) {
				potentialDisplayClass = potentialDisplayClass.DeclaringTypeDefinition;
				if (potentialDisplayClass == null)
					return false;
			}
			return true;
		}

		ILFunction currentFunction;

		protected internal override void VisitILFunction(ILFunction function)
		{
			var oldFunction = this.currentFunction;
			try {
				this.currentFunction = function;
				base.VisitILFunction(function);
			} finally {
				this.currentFunction = oldFunction;
			}
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

			if (inst.Parent is Block && inst.Variable.IsSingleDefinition) {
				if ((inst.Variable.Kind == VariableKind.Local || inst.Variable.Kind == VariableKind.StackSlot) && inst.Variable.LoadCount == 0 && (inst.Value is StLoc || inst.Value is CompoundAssignmentInstruction)) {
					context.Step($"Remove unused variable assignment {inst.Variable.Name}", inst);
					inst.ReplaceWith(inst.Value);
					return;
				}
				if (displayClasses.TryGetValue(inst.Variable, out _) && inst.Value is NewObj) {
					instructionsToRemove.Add(inst);
					return;
				}
				if (inst.Value.MatchLdLoc(out var displayClassVariable) && displayClasses.TryGetValue(displayClassVariable, out var displayClass)) {
					context.Step($"Found copy-assignment of display-class variable {displayClassVariable.Name}", inst);
					displayClasses.Add(inst.Variable, displayClass);
					instructionsToRemove.Add(inst);
					return;
				}
			}
		}

		protected internal override void VisitStObj(StObj inst)
		{
			inst.Value.AcceptVisitor(this);
			if (inst.Parent is Block) {
				if (IsParameterAssignment(inst, out var declarationSlot, out var parameter)) {
					context.Step($"Detected parameter assignment {parameter.Name}", inst);
					declarationSlot.UseExisting(parameter);
					instructionsToRemove.Add(inst);
					return;
				}
				if (IsDisplayClassAssignment(inst, out declarationSlot, out var variable)) {
					context.Step($"Detected display-class assignment {variable.Name}", inst);
					declarationSlot.UseExisting(variable);
					instructionsToRemove.Add(inst);
					return;
				}
			}
			inst.Target.AcceptVisitor(this);
			EarlyExpressionTransforms.StObjToStLoc(inst, context);
		}

		protected internal override void VisitLdObj(LdObj inst)
		{
			base.VisitLdObj(inst);
			EarlyExpressionTransforms.LdObjToLdLoc(inst, context);
		}

		private bool IsDisplayClassLoad(ILInstruction target, out ILVariable variable)
		{
			// We cannot use MatchLdLocRef here because local functions use ref parameters
			if (target.MatchLdLoc(out variable) || target.MatchLdLoca(out variable))
				return true;
			return false;
		}

		private bool IsDisplayClassAssignment(StObj inst, out VariableToDeclare declarationSlot, out ILVariable variable)
		{
			variable = null;
			declarationSlot = null;
			if (!IsDisplayClassFieldAccess(inst.Target, out var displayClassVar, out var displayClass, out var field))
				return false;
			if (!(inst.Value.MatchLdLoc(out var v) && displayClasses.ContainsKey(v)))
				return false;
			if (!displayClass.VariablesToDeclare.TryGetValue((IField)field.MemberDefinition, out declarationSlot))
				return false;
			if (displayClassVar.Function != currentFunction)
				return false;
			variable = v;
			return true;
		}

		private bool IsParameterAssignment(StObj inst, out VariableToDeclare declarationSlot, out ILVariable parameter)
		{
			parameter = null;
			declarationSlot = null;
			if (!IsDisplayClassFieldAccess(inst.Target, out var displayClassVar, out var displayClass, out var field))
				return false;
			if (!(inst.Value.MatchLdLoc(out var v) && v.Kind == VariableKind.Parameter && v.Function == currentFunction && v.Type.Equals(field.Type)))
				return false;
			if (!displayClass.VariablesToDeclare.TryGetValue((IField)field.MemberDefinition, out declarationSlot))
				return false;
			if (displayClassVar.Function != currentFunction)
				return false;
			if (declarationSlot.AssignedValues.Count > 1) {
				var first = declarationSlot.AssignedValues[0].Ancestors.OfType<ILFunction>().First();
				if (declarationSlot.AssignedValues.All(i => i.Ancestors.OfType<ILFunction>().First() == first))
					return false;
			}
			parameter = v;
			return true;
		}

		private bool IsDisplayClassFieldAccess(ILInstruction inst, out ILVariable displayClassVar, out DisplayClass displayClass, out IField field)
		{
			displayClass = null;
			displayClassVar = null;
			field = null;
			if (!(inst is LdFlda ldflda))
				return false;
			field = ldflda.Field;
			return IsDisplayClassLoad(ldflda.Target, out displayClassVar)
				&& displayClasses.TryGetValue(displayClassVar, out displayClass);
		}

		protected internal override void VisitLdFlda(LdFlda inst)
		{
			base.VisitLdFlda(inst);
			// Get display class info
			if (!IsDisplayClassFieldAccess(inst, out _, out DisplayClass displayClass, out IField field))
				return;
			var keyField = (IField)field.MemberDefinition;
			if (!displayClass.VariablesToDeclare.TryGetValue(keyField, out var v) || v == null) {
				displayClass.VariablesToDeclare[keyField] = v = new VariableToDeclare(displayClass, field);
			}
			context.Step($"Replace {field.FullName} with captured variable {v.Name}", inst);
			inst.ReplaceWith(new LdLoca(v.GetOrDeclare()).WithILRange(inst));
		}
	}
}
