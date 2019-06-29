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
	/// This is a post-processing step of <see cref="DelegateConstruction"/> and <see cref="TransformExpressionTrees"/>.
	/// </summary>
	class TransformDisplayClassUsage : ILVisitor, IILTransform
	{
		class DisplayClass
		{
			public ILInstruction Initializer;
			public ILVariable Variable;
			public ITypeDefinition Definition;
			public Dictionary<IField, DisplayClassVariable> Variables;
			public BlockContainer CaptureScope;
		}

		struct DisplayClassVariable
		{
			public ILVariable Variable;
			public ILInstruction Value;
		}

		ILTransformContext context;
		ILFunction currentFunction;
		readonly Dictionary<ILVariable, DisplayClass> displayClasses = new Dictionary<ILVariable, DisplayClass>();
		readonly List<ILInstruction> orphanedVariableInits = new List<ILInstruction>();

		public void Run(ILFunction function, ILTransformContext context)
		{
			try {
				if (this.context != null || this.currentFunction != null)
					throw new InvalidOperationException("Reentrancy in " + nameof(TransformDisplayClassUsage));
				this.context = context;
				// Traverse nested functions in post-order:
				// Inner functions are transformed before outer functions
				foreach (var f in function.Descendants.OfType<ILFunction>()) {
					foreach (var v in f.Variables) {
						if (!(v.IsSingleDefinition && v.StoreInstructions.SingleOrDefault() is StLoc inst))
							continue;
						if (IsClosureInit(inst, out var closureType)) {
							displayClasses.Add(inst.Variable, new DisplayClass {
								Initializer = inst,
								Variable = v,
								Definition = closureType,
								Variables = new Dictionary<IField, DisplayClassVariable>(),
								CaptureScope = inst.Variable.CaptureScope
							});
							orphanedVariableInits.Add(inst);
						}
					}
					foreach (var displayClass in displayClasses.Values.OrderByDescending(d => d.Initializer.StartILOffset).ToArray()) {
						context.Step($"Transform references to " + displayClass.Variable, displayClass.Initializer);
						this.currentFunction = f;
						VisitILFunction(f);
					}
				}
				context.Step($"Remove orphanedVariableInits", function);
				foreach (var store in orphanedVariableInits) {
					if (store.Parent is Block containingBlock)
						containingBlock.Instructions.Remove(store);
				}
			} finally {
				orphanedVariableInits.Clear();
				displayClasses.Clear();
				this.context = null;
				this.currentFunction = null;
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
			var decompilationContext = new SimpleTypeResolveContext(context.Function.Method);
			return IsPotentialClosure(decompilationContext.CurrentTypeDefinition, inst.Method.DeclaringTypeDefinition);
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
				orphanedVariableInits.Add(inst);
			}
		}

		bool IsClosureInit(StLoc inst, out ITypeDefinition closureType)
		{
			closureType = null;
			if (!(inst.Value is NewObj newObj))
				return false;
			closureType = newObj.Method.DeclaringTypeDefinition;
			return closureType != null && IsPotentialClosure(this.context, newObj);
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			// This instruction has been marked deletable, do not transform it further
			if (orphanedVariableInits.Contains(inst))
				return;
			// The target of the store instruction must be a field reference
			if (!inst.Target.MatchLdFlda(out ILInstruction target, out IField field))
				return;
			// Skip assignments that reference fields of the outer class, this is not a closure assignment.
			if (target.MatchLdThis())
				return;
			// Get display class info
			if (!(target is LdLoc displayClassLoad && displayClasses.TryGetValue(displayClassLoad.Variable, out var displayClass)))
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
					orphanedVariableInits.Add(inst);
					value = inst.Value;
				} else {
					Debug.Assert(displayClass.Definition == field.DeclaringTypeDefinition);
					// Introduce a fresh variable for the display class field.
					v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
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
			if (!(target is LdLoc displayClassLoad && displayClasses.TryGetValue(displayClassLoad.Variable, out var displayClass)))
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
				var variable = currentFunction.Variables.First((f) => f.Index == -1);
				inst.ReplaceWith(new LdLoca(variable).WithILRange(inst));
			}
			// Skip stfld/ldfld
			if (inst.Parent is LdObj || inst.Parent is StObj)
				return;
			// Get display class info
			if (!(inst.Target is LdLoc displayClassLoad && displayClasses.TryGetValue(displayClassLoad.Variable, out var displayClass)))
				return;
			var field = (IField)inst.Field.MemberDefinition;
			if (!displayClass.Variables.TryGetValue(field, out DisplayClassVariable info)) {
				// Introduce a fresh variable for the display class field.
				Debug.Assert(displayClass.Definition == field.DeclaringTypeDefinition);
				var v = currentFunction.RegisterVariable(VariableKind.Local, field.Type, field.Name);
				v.CaptureScope = displayClass.CaptureScope;
				inst.ReplaceWith(new LdLoca(v).WithILRange(inst));
				displayClass.Variables.Add(field, new DisplayClassVariable { Value = new LdLoc(v), Variable = v });
			} else if (info.Value is LdLoc l) {
				inst.ReplaceWith(new LdLoca(l.Variable).WithILRange(inst));
			} else {
				Debug.Fail("LdFlda pattern not supported!");
			}
		}

		protected internal override void VisitNumericCompoundAssign(NumericCompoundAssign inst)
		{
			base.VisitNumericCompoundAssign(inst);
			// NumericCompoundAssign is only valid when used with fields: -> replace it with a BinaryNumericInstruction.
			if (inst.Target.MatchLdLoc(out var v)) {
				inst.ReplaceWith(new StLoc(v, new BinaryNumericInstruction(inst.Operator, inst.Target, inst.Value, inst.CheckForOverflow, inst.Sign).WithILRange(inst)));
			}
		}

	}
}
