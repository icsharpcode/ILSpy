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
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
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
		class VariableToDeclare
		{
			private readonly DisplayClass container;
			private readonly IField field;
			private ILVariable declaredVariable;

			public string Name => field.Name;

			public bool CanPropagate { get; private set; }

			public HashSet<ILInstruction> Initializers { get; } = new HashSet<ILInstruction>();

			public VariableToDeclare(DisplayClass container, IField field, ILVariable declaredVariable = null)
			{
				this.container = container;
				this.field = field;
				this.declaredVariable = declaredVariable;

				Debug.Assert(declaredVariable == null || declaredVariable.StateMachineField == field);
			}

			public void Propagate(ILVariable variable)
			{
				Debug.Assert(declaredVariable == null || (variable == null && declaredVariable.StateMachineField == null));
				this.declaredVariable = variable;
				this.CanPropagate = variable != null;
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

		[DebuggerDisplay("[DisplayClass {Variable} : {Type}]")]
		class DisplayClass
		{
			public readonly ILVariable Variable;
			public readonly ITypeDefinition Type;
			public readonly Dictionary<IField, VariableToDeclare> VariablesToDeclare;
			public BlockContainer CaptureScope;
			public ILInstruction Initializer;

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

		public static bool AnalyzeVariable(ILVariable v, ILTransformContext context)
		{
			var tdcu = new TransformDisplayClassUsage();
			tdcu.context = context;
			tdcu.decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			return tdcu.AnalyzeVariable(v) != null;
		}

		private void AnalyzeFunction(ILFunction function)
		{
			foreach (var f in function.Descendants.OfType<ILFunction>()) {
				foreach (var v in f.Variables.ToArray()) {
					var result = AnalyzeVariable(v);
					if (result == null || displayClasses.ContainsKey(result.Variable))
						continue;
					context.Step($"Detected display-class {result.Variable}", result.Initializer ?? f.Body);
					displayClasses.Add(result.Variable, result);
				}
			}

			foreach (var (v, displayClass) in displayClasses.ToArray()) {
				if (!ValidateDisplayClassUses(v, displayClass))
					displayClasses.Remove(v);
			}

			foreach (var displayClass in displayClasses.Values) {
				foreach (var v in displayClass.VariablesToDeclare.Values) {
					if (v.CanPropagate) {
						var variableToPropagate = v.GetOrDeclare();
						if (variableToPropagate.Kind != VariableKind.Parameter && !displayClasses.ContainsKey(variableToPropagate))
							v.Propagate(null);
					}
				}
			}

			bool ValidateDisplayClassUses(ILVariable v, DisplayClass displayClass)
			{
				foreach (var ldloc in v.LoadInstructions) {
					if (!ValidateUse(displayClass, ldloc))
						return false;
				}
				foreach (var ldloca in v.AddressInstructions) {
					if (!ValidateUse(displayClass, ldloca))
						return false;
				}
				return true;
			}
		}

		private DisplayClass AnalyzeVariable(ILVariable v)
		{
			switch (v.Kind) {
				case VariableKind.Parameter:
					if (context.Settings.YieldReturn && v.Function.StateMachineCompiledWithMono && v.IsThis())
						return HandleMonoStateMachine(v.Function, v);
					return null;
				case VariableKind.StackSlot:
				case VariableKind.Local:
				case VariableKind.DisplayClassLocal:
					return DetectDisplayClass(v);
				case VariableKind.InitializerTarget:
					return DetectDisplayClassInitializer(v);
				case VariableKind.PinnedLocal:
				case VariableKind.UsingLocal:
				case VariableKind.ForeachLocal:
				case VariableKind.NamedArgument:
				case VariableKind.ExceptionStackSlot:
				case VariableKind.ExceptionLocal:
					return null;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		DisplayClass DetectDisplayClass(ILVariable v)
		{
			ITypeDefinition definition;
			if (v.Kind != VariableKind.StackSlot) {
				definition = v.Type.GetDefinition();
			} else if (v.StoreInstructions.Count > 0 && v.StoreInstructions[0] is StLoc stloc && stloc.Value is NewObj newObj) {
				definition = newObj.Method.DeclaringTypeDefinition;
			} else {
				definition = null;
			}
			if (definition == null)
				return null;
			if (!context.Settings.AggressiveScalarReplacementOfAggregates) {
				if (definition.DeclaringTypeDefinition == null || definition.ParentModule.PEFile != context.PEFile)
					return null;
				if (!IsPotentialClosure(context, definition))
					return null;
			}
			DisplayClass result;
			Block initBlock;
			int startIndex;
			switch (definition.Kind) {
				case TypeKind.Class:
					if (!v.IsSingleDefinition)
						return null;
					if (!(v.StoreInstructions[0] is StLoc stloc))
						return null;
					if (stloc.Value is NewObj newObj && ValidateConstructor(newObj.Method)) {
						result = new DisplayClass(v, definition) {
							CaptureScope = v.CaptureScope,
							Initializer = stloc
						};
					} else {
						return null;
					}

					initBlock = stloc.Parent as Block;
					startIndex = stloc.ChildIndex + 1;
					break;
				case TypeKind.Struct:
					if (!v.HasInitialValue)
						return null;
					if (v.StoreCount != 1)
						return null;
					result = new DisplayClass(v, definition) { CaptureScope = v.CaptureScope };
					initBlock = FindDisplayStructInitBlock(v);
					startIndex = 0;
					break;
				default:
					return null;
			}

			if (initBlock != null) {
				for (int i = startIndex; i < initBlock.Instructions.Count; i++) {
					var init = initBlock.Instructions[i];
					if (!init.MatchStFld(out var target, out var field, out var value))
						break;
					if (!target.MatchLdLocRef(result.Variable))
						break;
					if (result.VariablesToDeclare.ContainsKey((IField)field.MemberDefinition))
						break;
					var variable = AddVariable(result, (StObj)init, field);
					result.VariablesToDeclare[(IField)field.MemberDefinition] = variable;
				}
			}

			if (IsMonoNestedCaptureScope(definition)) {
				result.CaptureScope = null;
			}
			return result;
		}

		private Block FindDisplayStructInitBlock(ILVariable v)
		{
			var root = v.Function.Body;
			var block = Visit(root)?.Ancestors.OfType<Block>().FirstOrDefault();
			return block;

			ILInstruction Visit(ILInstruction inst)
			{
				switch (inst) {
					case LdLoc l:
						return l.Variable == v ? l : null;
					case StLoc s:
						return s.Variable == v ? s : null;
					case LdLoca la:
						return la.Variable == v ? la : null;
					default:
						return VisitChildren(inst);
				}
			}

			ILInstruction VisitChildren(ILInstruction inst)
			{
				ILInstruction result = null;
				foreach (var child in inst.Children) {
					var newResult = Visit(child);
					if (result == null) {
						result = newResult;
					} else if (newResult != null) {
						return inst;
					}
				}
				return result;
			}
		}

		DisplayClass DetectDisplayClassInitializer(ILVariable v)
		{
			if (v.StoreInstructions.Count != 1 || !(v.StoreInstructions[0] is StLoc store && store.Parent is Block initializerBlock && initializerBlock.Kind == BlockKind.ObjectInitializer))
				return null;
			if (!(store.Value is NewObj newObj))
				return null;
			var definition = newObj.Method.DeclaringType.GetDefinition();
			if (definition == null)
				return null;
			if (!context.Settings.AggressiveScalarReplacementOfAggregates) {
				if (definition.DeclaringTypeDefinition == null || definition.ParentModule.PEFile != context.PEFile)
					return null;
				if (!IsPotentialClosure(context, definition))
					return null;
			}
			if (!ValidateConstructor(newObj.Method))
				return null;
			if (!initializerBlock.Parent.MatchStLoc(out var referenceVariable))
				return null;
			var result = new DisplayClass(referenceVariable, definition) {
				CaptureScope = referenceVariable.CaptureScope,
				Initializer = initializerBlock.Parent
			};
			for (int i = 1; i < initializerBlock.Instructions.Count; i++) {
				var init = initializerBlock.Instructions[i];
				if (!init.MatchStFld(out var target, out var field, out _))
					break;
				if (!target.MatchLdLocRef(v))
					break;
				if (result.VariablesToDeclare.ContainsKey((IField)field.MemberDefinition))
					break;
				var variable = AddVariable(result, (StObj)init, field);
				result.VariablesToDeclare[(IField)field.MemberDefinition] = variable;
			}
			return result;
		}

		private bool ValidateConstructor(IMethod method)
		{
			try {
				if (method.Parameters.Count != 0)
					return false;
				var handle = (MethodDefinitionHandle)method.MetadataToken;
				var module = (MetadataModule)method.ParentModule;
				var file = module.PEFile;
				if (handle.IsNil || file != context.PEFile)
					return false;
				var def = file.Metadata.GetMethodDefinition(handle);
				if (def.RelativeVirtualAddress == 0)
					return false;
				var body = file.Reader.GetMethodBody(def.RelativeVirtualAddress);
				if (!body.LocalSignature.IsNil || body.ExceptionRegions.Length != 0)
					return false;
				var reader = body.GetILReader();
				if (reader.Length < 7)
					return false;
				// IL_0000: ldarg.0
				// IL_0001: call instance void [mscorlib]System.Object::.ctor()
				// IL_0006: ret
				if (DecodeOpCodeSkipNop(ref reader) != ILOpCode.Ldarg_0)
					return false;
				if (DecodeOpCodeSkipNop(ref reader) != ILOpCode.Call)
					return false;
				var baseCtorHandle = MetadataTokenHelpers.EntityHandleOrNil(reader.ReadInt32());
				if (baseCtorHandle.IsNil)
					return false;
				var objectCtor = module.ResolveMethod(baseCtorHandle, new TypeSystem.GenericContext());
				if (!objectCtor.DeclaringType.IsKnownType(KnownTypeCode.Object))
					return false;
				if (!objectCtor.IsConstructor || objectCtor.Parameters.Count != 0)
					return false;
				return DecodeOpCodeSkipNop(ref reader) == ILOpCode.Ret;
			} catch (BadImageFormatException) {
				return false;
			}
		}

		ILOpCode DecodeOpCodeSkipNop(ref BlobReader reader)
		{
			ILOpCode code;
			do {
				code = reader.DecodeOpCode();
			} while (code == ILOpCode.Nop);
			return code;
		}

		VariableToDeclare AddVariable(DisplayClass result, StObj statement, IField field)
		{
			VariableToDeclare variable = new VariableToDeclare(result, field);
			if (statement != null
				&& statement.Value.MatchLdLoc(out var variableToPropagate)
				&& variableToPropagate.StateMachineField == null 
				&& (variableToPropagate.Kind == VariableKind.StackSlot || variableToPropagate.Type.Equals(field.Type)))
			{
				variable.Propagate(variableToPropagate);
				variable.Initializers.Add(statement);
			}
			return variable;
		}

		bool ValidateUse(DisplayClass container, ILInstruction use)
		{
			IField field;
			switch (use.Parent) {
				case LdFlda ldflda when ldflda.MatchLdFlda(out _, out field):
					var keyField = (IField)field.MemberDefinition;
					if (!container.VariablesToDeclare.TryGetValue(keyField, out VariableToDeclare variable) || variable == null) {
						variable = AddVariable(container, null, field);
					}
					container.VariablesToDeclare[keyField] = variable;
					return variable != null;
				case StObj stobj when stobj.MatchStObj(out var target, out ILInstruction value, out _) && value == use:
					if (target.MatchLdFlda(out var load, out field) && load.MatchLdLocRef(out var otherVariable) && displayClasses.TryGetValue(otherVariable, out var displayClass)) {
						if (displayClass.VariablesToDeclare.TryGetValue((IField)field.MemberDefinition, out var declaredVar))
							return declaredVar.CanPropagate;
					}
					return true;
				default:
					return false;
			}
		}

		private void Transform(ILFunction function)
		{
			VisitILFunction(function);
			foreach (var displayClass in displayClasses.Values) {
				foreach (var v in displayClass.VariablesToDeclare.Values) {
					if (v.CanPropagate && v.Initializers.Count == 1) {
						instructionsToRemove.Add(v.Initializers.First());
					}
				}
			}
			if (instructionsToRemove.Count > 0) {
				context.Step($"Remove instructions", function);
				foreach (var store in instructionsToRemove) {
					if (store.Parent is Block containingBlock)
						containingBlock.Instructions.Remove(store);
				}
			}
			context.Step($"ResetHasInitialValueFlag", function);
			foreach (var f in TreeTraversal.PostOrder(function, f => f.LocalFunctions)) {
				RemoveDeadVariableInit.ResetHasInitialValueFlag(f, context);
				f.CapturedVariables.RemoveWhere(v => v.IsDead);
			}
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
				VariableToDeclare variableToDeclare = new VariableToDeclare(displayClass, stateMachineVariable.StateMachineField, stateMachineVariable);
				displayClass.VariablesToDeclare.Add(stateMachineVariable.StateMachineField, variableToDeclare);
			}
			if (!function.Method.IsStatic && FindThisField(out var thisField)) {
				var thisVar = function.Variables
					.FirstOrDefault(t => t.IsThis() && t.Type.GetDefinition() == decompilationContext.CurrentTypeDefinition);
				if (thisVar == null) {
					thisVar = new ILVariable(VariableKind.Parameter, decompilationContext.CurrentTypeDefinition, -1) { Name = "this" };
					function.Variables.Add(thisVar);
				}
				VariableToDeclare variableToDeclare = new VariableToDeclare(displayClass, thisField, thisVar);
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
			context.StepStartGroup("Visit " + function.Name);
			try {
				this.currentFunction = function;
				base.VisitILFunction(function);
			} finally {
				this.currentFunction = oldFunction;
				context.StepEndGroup(keepIfEmpty: true);
			}
		}

		protected override void Default(ILInstruction inst)
		{
			ILInstruction next;
			for (var child = inst.Children.FirstOrDefault(); child != null; child = next) {
				next = child.GetNextSibling();
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitStLoc(StLoc inst)
		{
			base.VisitStLoc(inst);

			if (inst.Parent is Block parentBlock && inst.Variable.IsSingleDefinition) {
				if ((inst.Variable.Kind == VariableKind.Local || inst.Variable.Kind == VariableKind.StackSlot) && inst.Variable.LoadCount == 0 && (inst.Value is StLoc || inst.Value is CompoundAssignmentInstruction)) {
					context.Step($"Remove unused variable assignment {inst.Variable.Name}", inst);
					inst.ReplaceWith(inst.Value);
					return;
				}
				if (displayClasses.TryGetValue(inst.Variable, out _) && inst.Value is NewObj) {
					instructionsToRemove.Add(inst);
					return;
				}
				if (displayClasses.TryGetValue(inst.Variable, out var displayClass) && inst.Value is Block initBlock && initBlock.Kind == BlockKind.ObjectInitializer) {
					instructionsToRemove.Add(inst);
					for (int i = 1; i < initBlock.Instructions.Count; i++) {
						var stobj = (StObj)initBlock.Instructions[i];
						var variable = displayClass.VariablesToDeclare[(IField)((LdFlda)stobj.Target).Field.MemberDefinition];
						parentBlock.Instructions.Insert(inst.ChildIndex + i, new StLoc(variable.GetOrDeclare(), stobj.Value).WithILRange(stobj));
					}
					return;
				}
				// TODO : this is dangerous!
				if (inst.Value.MatchLdLocRef(out var otherVariable) && displayClasses.TryGetValue(otherVariable, out displayClass)) {
					instructionsToRemove.Add(inst);
					displayClasses.Add(inst.Variable, displayClass);
				}
			}
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			var instructions = inst.Parent.Children;
			int childIndex = inst.ChildIndex;
			EarlyExpressionTransforms.StObjToStLoc(inst, context);
			if (inst != instructions[childIndex]) {
				foreach (var displayClass in displayClasses.Values) {
					foreach (var v in displayClass.VariablesToDeclare.Values) {
						if (v.CanPropagate) {
							if (v.Initializers.Contains(inst)) {
								v.Initializers.Add(instructions[childIndex]);
								v.Initializers.Remove(inst);
							}
						}
					}
				}
				if (instructions[childIndex].MatchStLoc(out var innerDisplayClassVar, out var value) && value.MatchLdLoc(out var outerDisplayClassVar)) {
					if (!displayClasses.ContainsKey(innerDisplayClassVar) && displayClasses.TryGetValue(outerDisplayClassVar, out var displayClass)) {
						displayClasses.Add(innerDisplayClassVar, displayClass);
						instructionsToRemove.Add(instructions[childIndex]);
					}
				}
			}
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
			var v = displayClass.VariablesToDeclare[keyField];
			context.Step($"Replace {field.Name} with captured variable {v.Name}", inst);
			ILVariable variable = v.GetOrDeclare();
			inst.ReplaceWith(new LdLoca(variable).WithILRange(inst));
			if (variable.Function != currentFunction && variable.Kind != VariableKind.DisplayClassLocal) {
				currentFunction.CapturedVariables.Add(variable);
			}
		}
	}
}
