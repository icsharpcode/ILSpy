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

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transforms anonymous methods and lambdas by creating nested ILFunctions.
	/// </summary>
	public class DelegateConstruction : IILTransform
	{
		ILTransformContext context;
		ITypeResolveContext decompilationContext;
		readonly Stack<MethodDefinitionHandle> activeMethods = new Stack<MethodDefinitionHandle>();

		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.AnonymousMethods)
				return;
			var prevContext = this.context;
			var prevDecompilationContext = this.decompilationContext;
			try
			{
				activeMethods.Push((MethodDefinitionHandle)function.Method.MetadataToken);
				this.context = context;
				this.decompilationContext = new SimpleTypeResolveContext(function.Method);
				var cancellationToken = context.CancellationToken;
				foreach (var inst in function.Descendants)
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (inst is NewObj call)
					{
						context.StepStartGroup($"TransformDelegateConstruction {call.StartILOffset}", call);
						ILFunction f = TransformDelegateConstruction(call, out ILInstruction target);
						if (f != null && target is IInstructionWithVariableOperand instWithVar)
						{
							if (instWithVar.Variable.Kind == VariableKind.Local)
							{
								instWithVar.Variable.Kind = VariableKind.DisplayClassLocal;
							}
							if (instWithVar.Variable.IsSingleDefinition && instWithVar.Variable.StoreInstructions.SingleOrDefault() is StLoc store)
							{
								if (store.Value is NewObj)
								{
									instWithVar.Variable.CaptureScope = BlockContainer.FindClosestContainer(store);
								}
							}
						}
						context.StepEndGroup();
					}
				}
			}
			finally
			{
				this.context = prevContext;
				this.decompilationContext = prevDecompilationContext;
				activeMethods.Pop();
			}
		}

		internal static bool IsDelegateConstruction(NewObj inst, bool allowTransformed = false)
		{
			if (inst == null || inst.Arguments.Count != 2)
				return false;
			var opCode = inst.Arguments[1].OpCode;
			if (!(opCode == OpCode.LdFtn || opCode == OpCode.LdVirtFtn || (allowTransformed && opCode == OpCode.ILFunction)))
				return false;
			var typeKind = inst.Method.DeclaringType.Kind;
			return typeKind == TypeKind.Delegate || typeKind == TypeKind.Unknown;
		}

		static bool IsAnonymousMethod(ITypeDefinition decompiledTypeDefinition, IMethod method)
		{
			if (method == null)
				return false;
			if (!(method.HasGeneratedName()
				|| method.Name.Contains("$")
				|| method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()
				|| TransformDisplayClassUsage.IsPotentialClosure(decompiledTypeDefinition, method.DeclaringTypeDefinition)
				|| ContainsAnonymousType(method)))
				return false;
			return true;
		}

		static bool ContainsAnonymousType(IMethod method)
		{
			if (method.ReturnType.ContainsAnonymousType())
				return true;
			foreach (var p in method.Parameters)
			{
				if (p.Type.ContainsAnonymousType())
					return true;
			}
			return false;
		}

		static GenericContext? GenericContextFromTypeArguments(TypeParameterSubstitution subst)
		{
			var classTypeParameters = new List<ITypeParameter>();
			var methodTypeParameters = new List<ITypeParameter>();
			if (subst.ClassTypeArguments != null)
			{
				foreach (var t in subst.ClassTypeArguments)
				{
					if (t is ITypeParameter tp)
						classTypeParameters.Add(tp);
					else
						return null;
				}
			}
			if (subst.MethodTypeArguments != null)
			{
				foreach (var t in subst.MethodTypeArguments)
				{
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
			if (targetMethod.MetadataToken.IsNil)
				return null;
			if (LocalFunctionDecompiler.IsLocalFunctionMethod(targetMethod, context))
				return null;
			target = value.Arguments[0];
			if (!ValidateDelegateTarget(target))
				return null;
			var handle = (MethodDefinitionHandle)targetMethod.MetadataToken;
			if (activeMethods.Contains(handle))
			{
				this.context.Function.Warnings.Add(" Found self-referencing delegate construction. Abort transformation to avoid stack overflow.");
				return null;
			}
			var methodDefinition = context.PEFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)targetMethod.MetadataToken);
			if (!methodDefinition.HasBody())
				return null;
			var genericContext = GenericContextFromTypeArguments(targetMethod.Substitution);
			if (genericContext == null)
				return null;
			var ilReader = context.CreateILReader();
			var body = context.PEFile.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
			var function = ilReader.ReadIL((MethodDefinitionHandle)targetMethod.MetadataToken, body, genericContext.Value, ILFunctionKind.Delegate, context.CancellationToken);
			function.DelegateType = value.Method.DeclaringType;
			// Embed the lambda into the parent function's ILAst, so that "Show steps" can show
			// how the lambda body is being transformed.
			value.ReplaceWith(function);
			function.CheckInvariant(ILPhase.Normal);

			var contextPrefix = targetMethod.Name;
			foreach (ILVariable v in function.Variables.Where(v => v.Kind != VariableKind.Parameter))
			{
				v.Name = contextPrefix + v.Name;
			}

			var nestedContext = new ILTransformContext(context, function);
			function.RunTransforms(CSharpDecompiler.GetILTransforms().TakeWhile(t => !(t is DelegateConstruction)).Concat(GetTransforms()), nestedContext);
			nestedContext.Step("DelegateConstruction (ReplaceDelegateTargetVisitor)", function);
			function.AcceptVisitor(new ReplaceDelegateTargetVisitor(target, function.Variables.SingleOrDefault(VariableKindExtensions.IsThis)));
			// handle nested lambdas
			nestedContext.StepStartGroup("DelegateConstruction (nested lambdas)", function);
			((IILTransform)this).Run(function, nestedContext);
			nestedContext.StepEndGroup();
			function.AddILRange(target);
			function.AddILRange(value);
			function.AddILRange(value.Arguments[1]);
			return function;
		}

		private static bool ValidateDelegateTarget(ILInstruction inst)
		{
			switch (inst)
			{
				case LdNull _:
					return true;
				case LdLoc ldloc:
					return ldloc.Variable.IsSingleDefinition;
				case LdObj ldobj:
					// TODO : should make sure that the display-class 'this' is unused,
					// if the delegate target is ldobj(ldsflda field).
					if (ldobj.Target is LdsFlda)
						return true;
					// TODO : ldfld chains must be validated more thoroughly, i.e., we should make sure
					// that the value of the field is never changed.
					ILInstruction target = ldobj;
					while (target is LdObj || target is LdFlda)
					{
						if (target is LdObj o)
						{
							target = o.Target;
							continue;
						}
						if (target is LdFlda f)
						{
							target = f.Target;
							continue;
						}
					}
					return target is LdLoc;
				default:
					return false;
			}
		}

		private IEnumerable<IILTransform> GetTransforms()
		{
			yield return new CombineExitsTransform();
		}

		/// <summary>
		/// Replaces loads of 'this' with the target expression.
		/// Async delegates use: ldobj(ldloca this).
		/// </summary>
		internal class ReplaceDelegateTargetVisitor : ILVisitor
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
				foreach (var child in inst.Children)
				{
					child.AcceptVisitor(this);
				}
			}

			protected internal override void VisitILFunction(ILFunction function)
			{
				if (function == thisVariable?.Function)
				{
					ILVariable v = null;
					switch (target)
					{
						case LdLoc l:
							v = l.Variable;
							break;
						case LdObj lo:
							ILInstruction inner = lo.Target;
							while (inner is LdFlda ldf)
							{
								inner = ldf.Target;
							}
							if (inner is LdLoc l2)
								v = l2.Variable;
							break;
					}
					if (v != null)
						function.CapturedVariables.Add(v);
				}
				base.VisitILFunction(function);
			}

			protected internal override void VisitLdLoc(LdLoc inst)
			{
				if (inst.Variable == thisVariable)
				{
					inst.ReplaceWith(target.Clone());
					return;
				}
				base.VisitLdLoc(inst);
			}

			protected internal override void VisitLdObj(LdObj inst)
			{
				if (inst.Target.MatchLdLoca(thisVariable))
				{
					inst.ReplaceWith(target.Clone());
					return;
				}
				base.VisitLdObj(inst);
			}
		}
	}
}
