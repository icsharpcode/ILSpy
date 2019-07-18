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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Decompiler step for C# 7.0 local functions
	/// </summary>
	class LocalFunctionDecompiler : IILTransform
	{
		ILTransformContext context;

		/// <summary>
		/// The transform works like this:
		/// 
		/// <para>
		/// local functions can either be used in method calls, i.e., call and callvirt instructions,
		/// or can be used as part of the "delegate construction" pattern, i.e., <c>newobj Delegate(&lt;target-expression&gt;, ldftn &lt;method&gt;)</c>. Note that this pattern would also be possible with <c>ldvirtftn</c>, but I haven't yet disovered a case where a compiler generates such code.
		/// </para>
		/// As local functions can be declared practically anywhere, we have to take a look at all use-sites and infer the declaration location from that.
		/// </summary>
		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.LocalFunctions)
				return;
			this.context = context;
			var localFunctions = new Dictionary<IMethod, List<CallInstruction>>();
			var cancellationToken = context.CancellationToken;
			// Find use-sites
			foreach (var inst in function.Descendants) {
				cancellationToken.ThrowIfCancellationRequested();
				if (inst is CallInstruction call && IsLocalFunctionMethod(call.Method) && !call.Method.IsLocalFunction) {
					if (!localFunctions.TryGetValue(call.Method, out var info)) {
						info = new List<CallInstruction>() { call };
						localFunctions.Add(call.Method, info);
					} else {
						info.Add(call);
					}
				} else if (inst is LdFtn ldftn && !ldftn.Method.IsLocalFunction && ldftn.Parent is NewObj newObj && IsLocalFunctionMethod(ldftn.Method) && DelegateConstruction.IsDelegateConstruction(newObj)) {
					context.StepStartGroup($"LocalFunctionDecompiler {ldftn.StartILOffset}", ldftn);
					if (!localFunctions.TryGetValue(ldftn.Method, out var info)) {
						info = new List<CallInstruction>() { newObj };
						localFunctions.Add(ldftn.Method, info);
					} else {
						info.Add(newObj);
					}
					context.StepEndGroup();
				}
			}

			foreach (var (method, useSites) in localFunctions) {
				context.StepStartGroup($"LocalFunctionDecompiler {useSites[0].StartILOffset}", useSites[0]);
				try {
					TransformLocalFunction(function, method, useSites);
				} finally {
					context.StepEndGroup();
				}
			}

			foreach (var f in function.LocalFunctions) {
				// handle nested functions
				var nestedContext = new ILTransformContext(context, f);
				nestedContext.StepStartGroup("LocalFunctionDecompiler (nested functions)", f);
				new LocalFunctionDecompiler().Run(f, nestedContext);
				nestedContext.StepEndGroup();
			}

			if (function.Kind == ILFunctionKind.TopLevelFunction) {
				var movableFunctions = TreeTraversal.PostOrder(function, f => f.LocalFunctions)
					.Where(f => f.Kind == ILFunctionKind.LocalFunction && f.DeclarationScope == null)
					.ToArray();
				foreach (var f in movableFunctions) {
					var parent = (ILFunction)f.Parent;
					f.DeclarationScope = (BlockContainer)function.Body;
					parent.LocalFunctions.Remove(f);
					function.LocalFunctions.Add(f);
				}
			}
		}

		static T FindCommonAncestorInstruction<T>(ILInstruction a, ILInstruction b)
			where T : ILInstruction
		{
			var ancestorsOfB = new HashSet<T>(b.Ancestors.OfType<T>());
			return a.Ancestors.OfType<T>().FirstOrDefault(ancestorsOfB.Contains);
		}

		internal static bool IsClosureParameter(IParameter parameter)
		{
			return parameter.IsRef
				&& ((ByReferenceType)parameter.Type).ElementType
					.GetDefinition()?.IsCompilerGenerated() == true;
		}

		static IType UnwrapByRef(IType type)
		{
			if (type is ByReferenceType byRef) {
				type = byRef.ElementType;
			}
			return type;
		}

		internal static ILInstruction GetStatement(ILInstruction inst)
		{
			while (inst.Parent != null) {
				if (inst.Parent is Block)
					return inst;
				inst = inst.Parent;
			}
			return inst;
		}

		private ILFunction TransformLocalFunction(ILFunction parentFunction, IMethod targetMethod, List<CallInstruction> useSites)
		{
			var methodDefinition = context.PEFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)targetMethod.MetadataToken);
			if (!methodDefinition.HasBody())
				return null;
			var genericContext = DelegateConstruction.GenericContextFromTypeArguments(targetMethod.Substitution);
			if (genericContext == null)
				return null;
			var function = parentFunction.Ancestors.OfType<ILFunction>().SelectMany(f => f.LocalFunctions).FirstOrDefault(f => f.Method == targetMethod);
			if (function == null) {
				var ilReader = context.CreateILReader();
				var body = context.PEFile.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
				function = ilReader.ReadIL((MethodDefinitionHandle)targetMethod.MetadataToken, body, genericContext.Value, ILFunctionKind.LocalFunction, context.CancellationToken);
				// Embed the local function into the parent function's ILAst, so that "Show steps" can show
				// how the local function body is being transformed.
				parentFunction.LocalFunctions.Add(function);
				function.DeclarationScope = (BlockContainer)parentFunction.Body;
				function.CheckInvariant(ILPhase.Normal);
				var nestedContext = new ILTransformContext(context, function);
				function.RunTransforms(CSharpDecompiler.GetILTransforms().TakeWhile(t => !(t is LocalFunctionDecompiler)), nestedContext);
				if (IsNonLocalTarget(targetMethod, useSites, out var target)) {
					Debug.Assert(target != null);
					nestedContext.Step("LocalFunctionDecompiler (ReplaceDelegateTargetVisitor)", function);
					var thisVar = function.Variables.SingleOrDefault(v => v.Index == -1 && v.Kind == VariableKind.Parameter);
					function.AcceptVisitor(new DelegateConstruction.ReplaceDelegateTargetVisitor(target, thisVar));
				}
				function.DeclarationScope = null;
				function.ReducedMethod = ReduceToLocalFunction(targetMethod);

				foreach (var innerUseSite in function.Descendants.OfType<CallInstruction>()) {
					if (innerUseSite.Method != function.Method)
						continue;
					if (innerUseSite.OpCode == OpCode.NewObj) {
						TransformToLocalFunctionReference(function, innerUseSite);
					} else {
						TransformToLocalFunctionInvocation(function.ReducedMethod, innerUseSite);
					}
				}
			}
			foreach (var useSite in useSites) {
				if (useSite.OpCode == OpCode.NewObj) {
					TransformToLocalFunctionReference(function, useSite);
				} else {
					DetermineCaptureAndDeclarationScope(function, useSite);
					TransformToLocalFunctionInvocation(function.ReducedMethod, useSite);
				}
			}

			if (function.DeclarationScope != null
				&& parentFunction.LocalFunctions.Contains(function)
				&& function.DeclarationScope.Parent is ILFunction betterParentFunction) {
				parentFunction.LocalFunctions.Remove(function);
				betterParentFunction.LocalFunctions.Add(function);
			}

			return function;
		}

		LocalFunctionMethod ReduceToLocalFunction(IMethod method)
		{
			int parametersToRemove = 0;
			for (int i = method.Parameters.Count - 1; i >= 0; i--) {
				if (!IsClosureParameter(method.Parameters[i]))
					break;
				parametersToRemove++;
			}
			return new LocalFunctionMethod(method, parametersToRemove);
		}

		static void TransformToLocalFunctionReference(ILFunction function, CallInstruction useSite)
		{
			useSite.Arguments[0].ReplaceWith(new LdNull().WithILRange(useSite.Arguments[0]));
			var fnptr = (IInstructionWithMethodOperand)useSite.Arguments[1];
			var replacement = new LdFtn(function.ReducedMethod).WithILRange((ILInstruction)fnptr);
			useSite.Arguments[1].ReplaceWith(replacement);
		}

		void TransformToLocalFunctionInvocation(LocalFunctionMethod reducedMethod, CallInstruction useSite)
		{
			bool wasInstanceCall = !useSite.Method.IsStatic;
			var replacement = new Call(reducedMethod);
			int firstArgumentIndex = wasInstanceCall ? 1 : 0;
			int argumentCount = useSite.Arguments.Count;
			int reducedArgumentCount = argumentCount - (reducedMethod.NumberOfCompilerGeneratedParameters + firstArgumentIndex);
			replacement.Arguments.AddRange(useSite.Arguments.Skip(firstArgumentIndex).Take(reducedArgumentCount));
			// copy flags:
			replacement.ConstrainedTo = useSite.ConstrainedTo;
			replacement.ILStackWasEmpty = useSite.ILStackWasEmpty;
			replacement.IsTail = useSite.IsTail;
			// copy IL ranges
			replacement = replacement.WithILRange(useSite);
			if (wasInstanceCall) {
				replacement = replacement.WithILRange(useSite.Arguments[0]);
			}
			for (int i = 0; i < reducedMethod.NumberOfCompilerGeneratedParameters; i++) {
				replacement = replacement.WithILRange(useSite.Arguments[argumentCount - i - 1]);
			}
			useSite.ReplaceWith(replacement);
		}

		void DetermineCaptureAndDeclarationScope(ILFunction function, CallInstruction useSite)
		{
			for (int i = useSite.Arguments.Count - 1; i >= 0; i--) {
				var arg = useSite.Arguments[i];
				ILVariable closureVar;
				if (!(arg.MatchLdLoc(out closureVar) || arg.MatchLdLoca(out closureVar)))
					break;
				if (closureVar.Kind == VariableKind.NamedArgument)
					break;
				if (!TransformDisplayClassUsage.IsPotentialClosure(context, UnwrapByRef(closureVar.Type).GetDefinition()))
					break;
				if (closureVar.AddressCount == 0 && closureVar.StoreInstructions.Count == 0)
					continue;
				// determine the capture scope of closureVar and the declaration scope of the function 
				var instructions = closureVar.StoreInstructions.OfType<ILInstruction>()
					.Concat(closureVar.AddressInstructions).OrderBy(inst => inst.StartILOffset);
				var additionalScope = BlockContainer.FindClosestContainer(instructions.First());
				if (closureVar.CaptureScope == null)
					closureVar.CaptureScope = additionalScope;
				else
					closureVar.CaptureScope = FindCommonAncestorInstruction<BlockContainer>(closureVar.CaptureScope, additionalScope);
				if (function.DeclarationScope == null)
					function.DeclarationScope = closureVar.CaptureScope;
				else
					function.DeclarationScope = FindCommonAncestorInstruction<BlockContainer>(function.DeclarationScope, closureVar.CaptureScope);
			}
		}

		bool IsNonLocalTarget(IMethod targetMethod, List<CallInstruction> useSites, out ILInstruction target)
		{
			target = null;
			if (targetMethod.IsStatic)
				return false;
			ValidateUseSites(useSites);
			target = useSites.Select(call => call.Arguments.First()).First();
			return !target.MatchLdThis();
		}

		[Conditional("DEBUG")]
		static void ValidateUseSites(List<CallInstruction> useSites)
		{
			ILInstruction targetInstruction = null;
			foreach (var site in useSites) {
				if (targetInstruction == null)
					targetInstruction = site.Arguments.First();
				else
					Debug.Assert(targetInstruction.Match(site.Arguments[0]).Success);
			}
		}

		internal static bool IsLocalFunctionReference(NewObj inst)
		{
			if (inst == null || inst.Arguments.Count != 2 || inst.Method.DeclaringType.Kind != TypeKind.Delegate)
				return false;
			var opCode = inst.Arguments[1].OpCode;

			return (opCode == OpCode.LdFtn || opCode == OpCode.LdVirtFtn)
				&& IsLocalFunctionMethod(((IInstructionWithMethodOperand)inst.Arguments[1]).Method);
		}

		public static bool IsLocalFunctionMethod(IMethod method)
		{
			if (method.MetadataToken.IsNil)
				return false;
			return IsLocalFunctionMethod(method.ParentModule.PEFile, (MethodDefinitionHandle)method.MetadataToken);
		}

		public static bool IsLocalFunctionMethod(PEFile module, MethodDefinitionHandle methodHandle)
		{
			var metadata = module.Metadata;
			var method = metadata.GetMethodDefinition(methodHandle);
			var declaringType = method.GetDeclaringType();

			if ((method.Attributes & MethodAttributes.Assembly) == 0 || !(method.IsCompilerGenerated(metadata) || declaringType.IsCompilerGenerated(metadata)))
				return false;

			if (!ParseLocalFunctionName(metadata.GetString(method.Name), out _, out _))
				return false;

			return true;
		}

		public static bool IsLocalFunctionDisplayClass(PEFile module, TypeDefinitionHandle typeHandle)
		{
			var metadata = module.Metadata;
			var type = metadata.GetTypeDefinition(typeHandle);

			if ((type.Attributes & TypeAttributes.NestedPrivate) == 0)
				return false;
			if (!type.HasGeneratedName(metadata))
				return false;

			var declaringTypeHandle = type.GetDeclaringType();
			var declaringType = metadata.GetTypeDefinition(declaringTypeHandle);

			foreach (var method in declaringType.GetMethods()) {
				if (!IsLocalFunctionMethod(module, method))
					continue;
				var md = metadata.GetMethodDefinition(method);
				if (md.DecodeSignature(new FindTypeDecoder(typeHandle), default).ParameterTypes.Any())
					return true;
			}

			return false;
		}

		/// <summary>
		/// Newer Roslyn versions use the format "&lt;callerName&gt;g__functionName|x_y"
		/// Older versions use "&lt;callerName&gt;g__functionNamex_y"
		/// </summary>
		static readonly Regex functionNameRegex = new Regex(@"^<(.*)>g__([^\|]*)\|{0,1}\d+(_\d+)?$", RegexOptions.Compiled);

		internal static bool ParseLocalFunctionName(string name, out string callerName, out string functionName)
		{
			callerName = null;
			functionName = null;
			if (string.IsNullOrWhiteSpace(name))
				return false;
			var match = functionNameRegex.Match(name);
			callerName = match.Groups[1].Value;
			functionName = match.Groups[2].Value;
			return match.Success;
		}

		struct FindTypeDecoder : ISignatureTypeProvider<bool, Unit>
		{
			readonly TypeDefinitionHandle handle;

			public FindTypeDecoder(TypeDefinitionHandle handle)
			{
				this.handle = handle;
			}

			public bool GetArrayType(bool elementType, ArrayShape shape) => elementType;
			public bool GetByReferenceType(bool elementType) => elementType;
			public bool GetFunctionPointerType(MethodSignature<bool> signature) => false;
			public bool GetGenericInstantiation(bool genericType, ImmutableArray<bool> typeArguments) => genericType;
			public bool GetGenericMethodParameter(Unit genericContext, int index) => false;
			public bool GetGenericTypeParameter(Unit genericContext, int index) => false;
			public bool GetModifiedType(bool modifier, bool unmodifiedType, bool isRequired) => unmodifiedType;
			public bool GetPinnedType(bool elementType) => elementType;
			public bool GetPointerType(bool elementType) => elementType;
			public bool GetPrimitiveType(PrimitiveTypeCode typeCode) => false;
			public bool GetSZArrayType(bool elementType) => false;

			public bool GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				return this.handle == handle;
			}

			public bool GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				return false;
			}

			public bool GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			{
				return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
			}
		}
	}
}
