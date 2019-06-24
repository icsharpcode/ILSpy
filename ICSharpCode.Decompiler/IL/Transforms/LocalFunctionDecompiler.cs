using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class LocalFunctionDecompiler : IILTransform
	{
		ILTransformContext context;
		ITypeResolveContext decompilationContext;

		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.LocalFunctions)
				return;
			this.context = context;
			this.decompilationContext = new SimpleTypeResolveContext(function.Method);
			var localFunctions = new Dictionary<IMethod, List<Call>>();
			var cancellationToken = context.CancellationToken;
			// Find use-sites
			foreach (var inst in function.Descendants) {
				cancellationToken.ThrowIfCancellationRequested();
				if (inst is Call call && IsLocalFunctionMethod(call.Method)) {
					context.StepStartGroup($"LocalFunctionDecompiler {call.StartILOffset}", call);
					if (!localFunctions.TryGetValue(call.Method, out var info)) {
						info = new List<Call>() { call };
						localFunctions.Add(call.Method, info);
					} else {
						info.Add(call);
					}
					context.StepEndGroup();
				}
			}

			foreach (var (method, useSites) in localFunctions) {
				var insertionPoint = FindInsertionPoint(useSites);
				if (TransformLocalFunction(method, (Block)insertionPoint.Parent, insertionPoint.ChildIndex + 1) == null)
					continue;
			}
		}

		static ILInstruction FindInsertionPoint(List<Call> useSites)
		{
			ILInstruction insertionPoint = null;
			foreach (var call in useSites) {
				if (insertionPoint == null) {
					insertionPoint = GetStatement(call);
					continue;
				}

				var ancestor = FindCommonAncestorInstruction(insertionPoint, GetStatement(call));

				if (ancestor == null)
					return null;

				insertionPoint = ancestor;
			}

			switch (insertionPoint) {
				case BlockContainer bc:
					return insertionPoint;
				case Block b:
					return insertionPoint;
				default:
					return insertionPoint;
			}
		}

		static ILInstruction FindCommonAncestorInstruction(ILInstruction a, ILInstruction b)
		{
			var ancestorsOfB = new HashSet<ILInstruction>(b.Ancestors);
			return a.Ancestors.FirstOrDefault(ancestorsOfB.Contains);
		}

		static ILInstruction GetStatement(ILInstruction inst)
		{
			while (inst.Parent != null) {
				if (inst.Parent is Block)
					return inst;
				inst = inst.Parent;
			}
			return inst;
		}

		private ILFunction TransformLocalFunction(IMethod targetMethod, Block parent, int insertionPoint)
		{
			var methodDefinition = context.PEFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)targetMethod.MetadataToken);
			if (!methodDefinition.HasBody())
				return null;
			var genericContext = DelegateConstruction.GenericContextFromTypeArguments(targetMethod.Substitution);
			if (genericContext == null)
				return null;
			var ilReader = context.CreateILReader();
			var body = context.PEFile.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress);
			var function = ilReader.ReadIL((MethodDefinitionHandle)targetMethod.MetadataToken, body, genericContext.Value, ILFunctionKind.LocalFunction, context.CancellationToken);
			// Embed the local function into the parent function's ILAst, so that "Show steps" can show
			// how the local function body is being transformed.
			parent.Instructions.Insert(insertionPoint, function);
			function.CheckInvariant(ILPhase.Normal);

			var nestedContext = new ILTransformContext(context, function);
			function.RunTransforms(CSharpDecompiler.GetILTransforms().TakeWhile(t => !(t is LocalFunctionDecompiler)), nestedContext);

			return function;
		}

		public static bool IsLocalFunctionMethod(IMethod method)
		{
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
		/// Newer Roslyn versions use the format "&ltcallerName&gtg__functionName|x_y"
		/// Older versions use "&ltcallerName&gtg__functionNamex_y"
		/// </summary>
		static readonly Regex functionNameRegex = new Regex(@"^<(.*)>g__(.*)\|{0,1}\d+(_\d+)?$", RegexOptions.Compiled);

		static bool ParseLocalFunctionName(string name, out string callerName, out string functionName)
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
			TypeDefinitionHandle handle;

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
