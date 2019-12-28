// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.DebugInfo;

namespace ICSharpCode.Decompiler.Disassembler
{
	/// <summary>
	/// Disassembles type and member definitions.
	/// </summary>
	public sealed class ReflectionDisassembler
	{
		readonly ITextOutput output;
		CancellationToken cancellationToken;
		bool isInType;   // whether we are currently disassembling a whole type (-> defaultCollapsed for foldings)
		MethodBodyDisassembler methodBodyDisassembler;

		public bool DetectControlStructure {
			get => methodBodyDisassembler.DetectControlStructure;
			set => methodBodyDisassembler.DetectControlStructure = value;
		}

		public bool ShowSequencePoints {
			get => methodBodyDisassembler.ShowSequencePoints;
			set => methodBodyDisassembler.ShowSequencePoints = value;
		}

		public bool ShowMetadataTokens {
			get => methodBodyDisassembler.ShowMetadataTokens;
			set => methodBodyDisassembler.ShowMetadataTokens = value;
		}

		public bool ShowMetadataTokensInBase10 {
			get => methodBodyDisassembler.ShowMetadataTokensInBase10;
			set => methodBodyDisassembler.ShowMetadataTokensInBase10 = value;
		}

		public IDebugInfoProvider DebugInfo {
			get => methodBodyDisassembler.DebugInfo;
			set => methodBodyDisassembler.DebugInfo = value;
		}

		public bool ExpandMemberDefinitions { get; set; } = false;

		public IAssemblyResolver AssemblyResolver { get; set; }

		public ReflectionDisassembler(ITextOutput output, CancellationToken cancellationToken)
			: this(output, new MethodBodyDisassembler(output, cancellationToken), cancellationToken)
		{
		}

		public ReflectionDisassembler(ITextOutput output, MethodBodyDisassembler methodBodyDisassembler, CancellationToken cancellationToken)
		{
			if (output == null)
				throw new ArgumentNullException(nameof(output));
			this.output = output;
			this.cancellationToken = cancellationToken;
			this.methodBodyDisassembler = methodBodyDisassembler;
		}

		#region Disassemble Method
		EnumNameCollection<MethodAttributes> methodAttributeFlags = new EnumNameCollection<MethodAttributes>() {
			{ MethodAttributes.Final, "final" },
			{ MethodAttributes.HideBySig, "hidebysig" },
			{ MethodAttributes.SpecialName, "specialname" },
			{ MethodAttributes.PinvokeImpl, null },	// handled separately
			{ MethodAttributes.UnmanagedExport, "export" },
			{ MethodAttributes.RTSpecialName, "rtspecialname" },
			{ MethodAttributes.RequireSecObject, "reqsecobj" },
			{ MethodAttributes.NewSlot, "newslot" },
			{ MethodAttributes.CheckAccessOnOverride, "strict" },
			{ MethodAttributes.Abstract, "abstract" },
			{ MethodAttributes.Virtual, "virtual" },
			{ MethodAttributes.Static, "static" },
			{ MethodAttributes.HasSecurity, null },	// ?? also invisible in ILDasm
		};

		EnumNameCollection<MethodAttributes> methodVisibility = new EnumNameCollection<MethodAttributes>() {
			{ MethodAttributes.Private, "private" },
			{ MethodAttributes.FamANDAssem, "famandassem" },
			{ MethodAttributes.Assembly, "assembly" },
			{ MethodAttributes.Family, "family" },
			{ MethodAttributes.FamORAssem, "famorassem" },
			{ MethodAttributes.Public, "public" },
		};

		EnumNameCollection<SignatureCallingConvention> callingConvention = new EnumNameCollection<SignatureCallingConvention>() {
			{ SignatureCallingConvention.CDecl, "unmanaged cdecl" },
			{ SignatureCallingConvention.StdCall, "unmanaged stdcall" },
			{ SignatureCallingConvention.ThisCall, "unmanaged thiscall" },
			{ SignatureCallingConvention.FastCall, "unmanaged fastcall" },
			{ SignatureCallingConvention.VarArgs, "vararg" },
			{ SignatureCallingConvention.Default, null },
		};

		EnumNameCollection<MethodImplAttributes> methodCodeType = new EnumNameCollection<MethodImplAttributes>() {
			{ MethodImplAttributes.IL, "cil" },
			{ MethodImplAttributes.Native, "native" },
			{ MethodImplAttributes.OPTIL, "optil" },
			{ MethodImplAttributes.Runtime, "runtime" },
		};

		EnumNameCollection<MethodImplAttributes> methodImpl = new EnumNameCollection<MethodImplAttributes>() {
			{ MethodImplAttributes.Synchronized, "synchronized" },
			{ MethodImplAttributes.NoInlining, "noinlining" },
			{ MethodImplAttributes.NoOptimization, "nooptimization" },
			{ MethodImplAttributes.PreserveSig, "preservesig" },
			{ MethodImplAttributes.InternalCall, "internalcall" },
			{ MethodImplAttributes.ForwardRef, "forwardref" },
			{ MethodImplAttributes.AggressiveInlining, "aggressiveinlining" },
		};

		public void DisassembleMethod(PEFile module, MethodDefinitionHandle handle)
		{
			var genericContext = new GenericContext(handle, module);
			// write method header
			output.WriteReference(module, handle, ".method", isDefinition: true);
			output.Write(" ");
			DisassembleMethodHeaderInternal(module, handle, genericContext);
			DisassembleMethodBlock(module, handle, genericContext);
		}

		public void DisassembleMethodHeader(PEFile module, MethodDefinitionHandle handle)
		{
			var genericContext = new GenericContext(handle, module);
			// write method header
			output.WriteReference(module, handle, ".method", isDefinition: true);
			output.Write(" ");
			DisassembleMethodHeaderInternal(module, handle, genericContext);
		}

		void DisassembleMethodHeaderInternal(PEFile module, MethodDefinitionHandle handle, GenericContext genericContext)
		{
			var metadata = module.Metadata;

			WriteMetadataToken(output, module, handle, MetadataTokens.GetToken(handle),
				spaceAfter: true, spaceBefore: false, ShowMetadataTokens, ShowMetadataTokensInBase10);
			var methodDefinition = metadata.GetMethodDefinition(handle);
			//    .method public hidebysig  specialname
			//               instance default class [mscorlib]System.IO.TextWriter get_BaseWriter ()  cil managed
			//
			//emit flags
			WriteEnum(methodDefinition.Attributes & MethodAttributes.MemberAccessMask, methodVisibility);
			WriteFlags(methodDefinition.Attributes & ~MethodAttributes.MemberAccessMask, methodAttributeFlags);
			bool isCompilerControlled = (methodDefinition.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			if (isCompilerControlled)
				output.Write("privatescope ");

			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl) {
				output.Write("pinvokeimpl");
				var info = methodDefinition.GetImport();
				if (!info.Module.IsNil) {
					var moduleRef = metadata.GetModuleReference(info.Module);
					output.Write("(\"" + DisassemblerHelpers.EscapeString(metadata.GetString(moduleRef.Name)) + "\"");

					if (!info.Name.IsNil && metadata.GetString(info.Name) != metadata.GetString(methodDefinition.Name))
						output.Write(" as \"" + DisassemblerHelpers.EscapeString(metadata.GetString(info.Name)) + "\"");

					if ((info.Attributes & MethodImportAttributes.ExactSpelling) == MethodImportAttributes.ExactSpelling)
						output.Write(" nomangle");

					switch (info.Attributes & MethodImportAttributes.CharSetMask) {
						case MethodImportAttributes.CharSetAnsi:
							output.Write(" ansi");
							break;
						case MethodImportAttributes.CharSetAuto:
							output.Write(" autochar");
							break;
						case MethodImportAttributes.CharSetUnicode:
							output.Write(" unicode");
							break;
					}

					if ((info.Attributes & MethodImportAttributes.SetLastError) == MethodImportAttributes.SetLastError)
						output.Write(" lasterr");

					switch (info.Attributes & MethodImportAttributes.CallingConventionMask) {
						case MethodImportAttributes.CallingConventionCDecl:
							output.Write(" cdecl");
							break;
						case MethodImportAttributes.CallingConventionFastCall:
							output.Write(" fastcall");
							break;
						case MethodImportAttributes.CallingConventionStdCall:
							output.Write(" stdcall");
							break;
						case MethodImportAttributes.CallingConventionThisCall:
							output.Write(" thiscall");
							break;
						case MethodImportAttributes.CallingConventionWinApi:
							output.Write(" winapi");
							break;
					}

					output.Write(')');
				}
				output.Write(' ');
			}

			output.WriteLine();
			output.Indent();
			var declaringType = methodDefinition.GetDeclaringType();
			MethodSignature<Action<ILNameSyntax>>? signature;
			try {
				var signatureProvider = new DisassemblerSignatureTypeProvider(module, output);
				signature = methodDefinition.DecodeSignature(signatureProvider, genericContext);
				if (signature.Value.Header.HasExplicitThis) {
					output.Write("instance explicit ");
				} else if (signature.Value.Header.IsInstance) {
					output.Write("instance ");
				}

				//call convention
				WriteEnum(signature.Value.Header.CallingConvention, callingConvention);

				//return type
				signature.Value.ReturnType(ILNameSyntax.Signature);
			} catch (BadImageFormatException) {
				signature = null;
				output.Write("<bad signature>");
			}
			output.Write(' ');

			var parameters = methodDefinition.GetParameters();
			if (parameters.Count > 0) {
				var firstParam = metadata.GetParameter(parameters.First());
				if (firstParam.SequenceNumber == 0) {
					var marshallingDesc = firstParam.GetMarshallingDescriptor();
					if (!marshallingDesc.IsNil) {
						WriteMarshalInfo(metadata.GetBlobReader(marshallingDesc));
					}
				}
			}

			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(methodDefinition.Name) + "$PST" + MetadataTokens.GetToken(handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(methodDefinition.Name)));
			}

			WriteTypeParameters(output, module, genericContext, methodDefinition.GetGenericParameters());

			//( params )
			output.Write(" (");
			if (signature?.ParameterTypes.Length > 0) {
				output.WriteLine();
				output.Indent();
				WriteParameters(metadata, parameters, signature.Value);
				output.Unindent();
			}
			output.Write(") ");
			//cil managed
			WriteEnum(methodDefinition.ImplAttributes & MethodImplAttributes.CodeTypeMask, methodCodeType);
			if ((methodDefinition.ImplAttributes & MethodImplAttributes.ManagedMask) == MethodImplAttributes.Managed)
				output.Write("managed ");
			else
				output.Write("unmanaged ");
			WriteFlags(methodDefinition.ImplAttributes & ~(MethodImplAttributes.CodeTypeMask | MethodImplAttributes.ManagedMask), methodImpl);

			output.Unindent();
		}

		internal static void WriteMetadataToken(ITextOutput output, PEFile module, Handle? handle, int metadataToken, bool spaceAfter, bool spaceBefore, bool showMetadataTokens, bool base10)
		{
			if (showMetadataTokens || handle == null) {
				if (spaceBefore) {
					output.Write(' ');
				}
				output.Write("/* ");
				if (base10) {
					output.WriteReference(module, handle.GetValueOrDefault(), metadataToken.ToString(), "metadata");
				} else {
					output.WriteReference(module, handle.GetValueOrDefault(), metadataToken.ToString("X8"), "metadata");
				}
				output.Write(" */");
				if (spaceAfter) {
					output.Write(' ');
				}
			}
		}

		void DisassembleMethodBlock(PEFile module, MethodDefinitionHandle handle, GenericContext genericContext)
		{
			var metadata = module.Metadata;
			var methodDefinition = metadata.GetMethodDefinition(handle);

			OpenBlock(defaultCollapsed: isInType);
			WriteAttributes(module, methodDefinition.GetCustomAttributes());
			foreach (var h in handle.GetMethodImplementations(metadata)) {
				var impl = metadata.GetMethodImplementation(h);
				output.Write(".override method ");
				impl.MethodDeclaration.WriteTo(module, output, genericContext);
				output.WriteLine();
			}

			foreach (var p in methodDefinition.GetGenericParameters()) {
				WriteGenericParameterAttributes(module, genericContext, p);
			}
			foreach (var p in methodDefinition.GetParameters()) {
				WriteParameterAttributes(module, p);
			}
			WriteSecurityDeclarations(module, methodDefinition.GetDeclarativeSecurityAttributes());

			if (methodDefinition.HasBody()) {
				methodBodyDisassembler.Disassemble(module, handle);
			}
			var declaringType = metadata.GetTypeDefinition(methodDefinition.GetDeclaringType());
			CloseBlock("end of method " + DisassemblerHelpers.Escape(metadata.GetString(declaringType.Name)) + "::" + DisassemblerHelpers.Escape(metadata.GetString(methodDefinition.Name)));
		}

		#region Write Security Declarations
		void WriteSecurityDeclarations(PEFile module, DeclarativeSecurityAttributeHandleCollection secDeclProvider)
		{
			if (secDeclProvider.Count == 0)
				return;
			foreach (var h in secDeclProvider) {
				output.Write(".permissionset ");
				var secdecl = module.Metadata.GetDeclarativeSecurityAttribute(h);
				switch ((ushort)secdecl.Action) {
					case 1: // DeclarativeSecurityAction.Request
						output.Write("request");
						break;
					case 2: // DeclarativeSecurityAction.Demand
						output.Write("demand");
						break;
					case 3: // DeclarativeSecurityAction.Assert
						output.Write("assert");
						break;
					case 4: // DeclarativeSecurityAction.Deny
						output.Write("deny");
						break;
					case 5: // DeclarativeSecurityAction.PermitOnly
						output.Write("permitonly");
						break;
					case 6: // DeclarativeSecurityAction.LinkDemand
						output.Write("linkcheck");
						break;
					case 7: // DeclarativeSecurityAction.InheritDemand
						output.Write("inheritcheck");
						break;
					case 8: // DeclarativeSecurityAction.RequestMinimum
						output.Write("reqmin");
						break;
					case 9: // DeclarativeSecurityAction.RequestOptional
						output.Write("reqopt");
						break;
					case 10: // DeclarativeSecurityAction.RequestRefuse
						output.Write("reqrefuse");
						break;
					case 11: // DeclarativeSecurityAction.PreJitGrant
						output.Write("prejitgrant");
						break;
					case 12: // DeclarativeSecurityAction.PreJitDeny
						output.Write("prejitdeny");
						break;
					case 13: // DeclarativeSecurityAction.NonCasDemand
						output.Write("noncasdemand");
						break;
					case 14: // DeclarativeSecurityAction.NonCasLinkDemand
						output.Write("noncaslinkdemand");
						break;
					case 15: // DeclarativeSecurityAction.NonCasInheritance
						output.Write("noncasinheritance");
						break;
					default:
						output.Write(secdecl.Action.ToString());
						break;
				}
				var blob = module.Metadata.GetBlobReader(secdecl.PermissionSet);
				if (AssemblyResolver == null) {
					output.Write(" = ");
					WriteBlob(blob);
					output.WriteLine();
				} else if ((char)blob.ReadByte() != '.') {
					blob.Reset();
					output.WriteLine();
					output.Indent();
					output.Write("bytearray");
					WriteBlob(blob);
					output.WriteLine();
					output.Unindent();
				} else {
					var outputWithRollback = new TextOutputWithRollback(output);
					try {
						TryDecodeSecurityDeclaration(outputWithRollback, blob, module);
						outputWithRollback.Commit();
					} catch (Exception ex) when (ex is BadImageFormatException || ex is EnumUnderlyingTypeResolveException) {
						blob.Reset();
						output.Write(" = ");
						WriteBlob(blob);
						output.WriteLine();
					}
				}
			}
		}

		class SecurityDeclarationDecoder : ICustomAttributeTypeProvider<(PrimitiveTypeCode, string)>
		{
			readonly ITextOutput output;
			readonly IAssemblyResolver resolver;
			readonly PEFile module;

			public SecurityDeclarationDecoder(ITextOutput output, IAssemblyResolver resolver, PEFile module)
			{
				this.output = output;
				this.resolver = resolver;
				this.module = module;
			}

			public (PrimitiveTypeCode, string) GetPrimitiveType(PrimitiveTypeCode typeCode)
			{
				return (typeCode, null);
			}

			public (PrimitiveTypeCode, string) GetSystemType()
			{
				return (0, "type");
			}

			public (PrimitiveTypeCode, string) GetSZArrayType((PrimitiveTypeCode, string) elementType)
			{
				return (elementType.Item1, (elementType.Item2 ?? PrimitiveTypeCodeToString(elementType.Item1)) + "[]");
			}

			public (PrimitiveTypeCode, string) GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				throw new NotImplementedException();
			}

			public (PrimitiveTypeCode, string) GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				throw new NotImplementedException();
			}

			public (PrimitiveTypeCode, string) GetTypeFromSerializedName(string name)
			{
				if (resolver == null)
					throw new EnumUnderlyingTypeResolveException();
				var (containingModule, typeDefHandle) = ResolveType(name, module);
				if (typeDefHandle.IsNil)
					throw new EnumUnderlyingTypeResolveException();
				if (typeDefHandle.IsEnum(containingModule.Metadata, out var typeCode))
					return (typeCode, "enum " + name);
				return (0, name);
			}

			public PrimitiveTypeCode GetUnderlyingEnumType((PrimitiveTypeCode, string) type)
			{
				return type.Item1;
			}

			public bool IsSystemType((PrimitiveTypeCode, string) type)
			{
				return "type" == type.Item2;
			}

			(PEFile, TypeDefinitionHandle) ResolveType(string typeName, PEFile module)
			{
				string[] nameParts = typeName.Split(new[] { ", " }, 2, StringSplitOptions.None);
				string[] typeNameParts = nameParts[0].Split('.');
				PEFile containingModule = null;
				TypeDefinitionHandle typeDefHandle = default;
				// if we deal with an assembly-qualified name, resolve the assembly
				if (nameParts.Length == 2)
					containingModule = resolver.Resolve(AssemblyNameReference.Parse(nameParts[1]));
				if (containingModule != null) {
					// try to find the type in the assembly
					typeDefHandle = FindType(containingModule, typeNameParts);
				} else {
					// just fully-qualified name, try current assembly
					typeDefHandle = FindType(module, typeNameParts);
					containingModule = module;
					if (typeDefHandle.IsNil && TryResolveMscorlib(out var mscorlib)) {
						// otherwise try mscorlib
						typeDefHandle = FindType(mscorlib, typeNameParts);
						containingModule = mscorlib;
					}
				}

				return (containingModule, typeDefHandle);

				TypeDefinitionHandle FindType(PEFile currentModule, string[] name)
				{
					var metadata = currentModule.Metadata;
					var currentNamespace = metadata.GetNamespaceDefinitionRoot();
					ImmutableArray<TypeDefinitionHandle> typeDefinitions = default;

					for (int i = 0; i < name.Length; i++) {
						string identifier = name[i];
						if (!typeDefinitions.IsDefault) {
							restart:
							foreach (var type in typeDefinitions) {
								var typeDef = metadata.GetTypeDefinition(type);
								var currentTypeName = metadata.GetString(typeDef.Name);
								if (identifier == currentTypeName) {
									if (i + 1 == name.Length)
										return type;
									typeDefinitions = typeDef.GetNestedTypes();
									goto restart;
								}
							}
						} else {
							var next = currentNamespace.NamespaceDefinitions.FirstOrDefault(ns => metadata.StringComparer.Equals(metadata.GetNamespaceDefinition(ns).Name, identifier));
							if (!next.IsNil) {
								currentNamespace = metadata.GetNamespaceDefinition(next);
							} else {
								typeDefinitions = currentNamespace.TypeDefinitions;
								i--;
							}
						}
					}
					return default;
				}
			}

			PrimitiveTypeCode ResolveEnumUnderlyingType(string typeName, PEFile module)
			{
				if (typeName.StartsWith("enum ", StringComparison.Ordinal))
					typeName = typeName.Substring(5);
				var (containingModule, typeDefHandle) = ResolveType(typeName, module);

				if (typeDefHandle.IsNil || !typeDefHandle.IsEnum(containingModule.Metadata, out var typeCode))
					throw new EnumUnderlyingTypeResolveException();
				return typeCode;
			}

			PEFile mscorlib;

			bool TryResolveMscorlib(out PEFile mscorlib)
			{
				mscorlib = null;
				if (this.mscorlib != null) {
					mscorlib = this.mscorlib;
					return true;
				}
				if (resolver == null) {
					return false;
				}
				this.mscorlib = mscorlib = resolver.Resolve(AssemblyNameReference.Parse("mscorlib"));
				return this.mscorlib != null;
			}
		}

		void TryDecodeSecurityDeclaration(TextOutputWithRollback output, BlobReader blob, PEFile module)
		{
			output.WriteLine(" = {");
			output.Indent();

			string currentAssemblyName = null;
			string currentFullAssemblyName = null;
			if (module.Metadata.IsAssembly) {
				currentAssemblyName = module.Metadata.GetString(module.Metadata.GetAssemblyDefinition().Name);
				currentFullAssemblyName = module.Metadata.GetFullAssemblyName();
			}
			int count = blob.ReadCompressedInteger();			
			for (int i = 0; i < count; i++) {
				var fullTypeName = blob.ReadSerializedString();
				string[] nameParts = fullTypeName.Split(new[] { ", " }, StringSplitOptions.None);
				if (nameParts.Length < 2 || nameParts[1] == currentAssemblyName) {
					output.Write("class ");
					output.Write(DisassemblerHelpers.Escape(fullTypeName));
				} else {
					output.Write('[');
					output.Write(nameParts[1]);
					output.Write(']');
					output.Write(nameParts[0]);
				}
				output.Write(" = {");
				blob.ReadCompressedInteger(); // ?
											  // The specification seems to be incorrect here, so I'm using the logic from Cecil instead.
				int argCount = blob.ReadCompressedInteger();

				var decoder = new CustomAttributeDecoder<(PrimitiveTypeCode Code, string Name)>(new SecurityDeclarationDecoder(output, AssemblyResolver, module), module.Metadata, provideBoxingTypeInfo: true);
				var arguments = decoder.DecodeNamedArguments(ref blob, argCount);

				if (argCount > 0) {
					output.WriteLine();
					output.Indent();
				}

				foreach (var argument in arguments) {
					switch (argument.Kind) {
						case CustomAttributeNamedArgumentKind.Field:
							output.Write("field ");
							break;
						case CustomAttributeNamedArgumentKind.Property:
							output.Write("property ");
							break;
					}

					output.Write(argument.Type.Name ?? PrimitiveTypeCodeToString(argument.Type.Code));
					output.Write(" " + argument.Name + " = ");

					WriteValue(output, argument.Type, argument.Value);
					output.WriteLine();
				}

				if (argCount > 0) {
					output.Unindent();
				}

				output.Write('}');

				if (i + 1 < count)
					output.Write(',');
				output.WriteLine();
			}
			
			output.Unindent();
			output.WriteLine("}");
		}

		void WriteValue(ITextOutput output, (PrimitiveTypeCode Code, string Name) type, object value)
		{
			if (value is CustomAttributeTypedArgument<(PrimitiveTypeCode, string)> boxedValue) {
				output.Write("object(");
				WriteValue(output, boxedValue.Type, boxedValue.Value);
				output.Write(")");
			} else if (value is ImmutableArray<CustomAttributeTypedArgument<(PrimitiveTypeCode, string)>> arrayValue) {
				string elementType = type.Name != null && !type.Name.StartsWith("enum ", StringComparison.Ordinal)
									? type.Name.Remove(type.Name.Length - 2) : PrimitiveTypeCodeToString(type.Code);

				output.Write(elementType);
				output.Write("[");
				output.Write(arrayValue.Length.ToString());
				output.Write("](");
				bool first = true;
				foreach (var item in arrayValue) {
					if (!first) output.Write(" ");
					if (item.Value is CustomAttributeTypedArgument<(PrimitiveTypeCode, string)> boxedItem) {
						WriteValue(output, boxedItem.Type, boxedItem.Value);
					} else {
						WriteSimpleValue(output, item.Value, elementType);
					}
					first = false;
				}
				output.Write(")");
			} else {
				string typeName = type.Name != null && !type.Name.StartsWith("enum ", StringComparison.Ordinal)
					? type.Name : PrimitiveTypeCodeToString(type.Code);

				output.Write(typeName);
				output.Write("(");
				WriteSimpleValue(output, value, typeName);
				output.Write(")");
			}
		}

		private static void WriteSimpleValue(ITextOutput output, object value, string typeName)
		{
			switch (typeName) {
				case "string":
					output.Write("'" + DisassemblerHelpers.EscapeString(value.ToString()).Replace("'", "\'") + "'");
					break;
				case "type":
					var info = ((PrimitiveTypeCode Code, string Name))value;
					if (info.Name.StartsWith("enum ", StringComparison.Ordinal)) {
						output.Write(info.Name.Substring(5));
					} else {
						output.Write(info.Name);
					}
					break;
				default:
					DisassemblerHelpers.WriteOperand(output, value);
					break;
			}
		}

		static string PrimitiveTypeCodeToString(PrimitiveTypeCode typeCode)
		{
			switch (typeCode) {
				case PrimitiveTypeCode.Boolean:
					return "bool";
				case PrimitiveTypeCode.Byte:
					return "uint8";
				case PrimitiveTypeCode.SByte:
					return "int8";
				case PrimitiveTypeCode.Char:
					return "char";
				case PrimitiveTypeCode.Int16:
					return "int16";
				case PrimitiveTypeCode.UInt16:
					return "uint16";
				case PrimitiveTypeCode.Int32:
					return "int32";
				case PrimitiveTypeCode.UInt32:
					return "uint32";
				case PrimitiveTypeCode.Int64:
					return "int64";
				case PrimitiveTypeCode.UInt64:
					return "uint64";
				case PrimitiveTypeCode.Single:
					return "float32";
				case PrimitiveTypeCode.Double:
					return "float64";
				case PrimitiveTypeCode.String:
					return "string";
				case PrimitiveTypeCode.Object:
					return "object";
				default:
					return "unknown";
			}
		}

		#endregion

		#region WriteMarshalInfo
		void WriteMarshalInfo(BlobReader marshalInfo)
		{
			output.Write("marshal(");
			WriteNativeType(ref marshalInfo);
			output.Write(") ");
		}

		void WriteNativeType(ref BlobReader blob)
		{
			byte type;
			switch (type = blob.ReadByte()) {
				case 0x66: // None
				case 0x50: // Max
					break;
				case 0x02: // NATIVE_TYPE_BOOLEAN 
					output.Write("bool");
					break;
				case 0x03: // NATIVE_TYPE_I1
					output.Write("int8");
					break;
				case 0x04: // NATIVE_TYPE_U1
					output.Write("unsigned int8");
					break;
				case 0x05: // NATIVE_TYPE_I2
					output.Write("int16");
					break;
				case 0x06: // NATIVE_TYPE_U2
					output.Write("unsigned int16");
					break;
				case 0x07: // NATIVE_TYPE_I4
					output.Write("int32");
					break;
				case 0x08: // NATIVE_TYPE_U4
					output.Write("unsigned int32");
					break;
				case 0x09: // NATIVE_TYPE_I8
					output.Write("int64");
					break;
				case 0x0a: // NATIVE_TYPE_U8
					output.Write("unsigned int64");
					break;
				case 0x0b: // NATIVE_TYPE_R4
					output.Write("float32");
					break;
				case 0x0c: // NATIVE_TYPE_R8
					output.Write("float64");
					break;
				case 0x14: // NATIVE_TYPE_LPSTR
					output.Write("lpstr");
					break;
				case 0x1f: // NATIVE_TYPE_INT
					output.Write("int");
					break;
				case 0x20: // NATIVE_TYPE_UINT
					output.Write("unsigned int");
					break;
				case 0x26: // NATIVE_TYPE_FUNC
					output.Write("Func");
					break;
				case 0x2a: // NATIVE_TYPE_ARRAY
					if (blob.RemainingBytes > 0)
						WriteNativeType(ref blob);
					output.Write('[');
					int sizeParameterIndex = blob.TryReadCompressedInteger(out int value) ? value : -1;
					int size = blob.TryReadCompressedInteger(out value) ? value : -1;
					int sizeParameterMultiplier = blob.TryReadCompressedInteger(out value) ? value : -1;
					if (size >= 0) {
						output.Write(size.ToString());
					}
					if (sizeParameterIndex >= 0 && sizeParameterMultiplier != 0) {
						output.Write(" + ");
						output.Write(sizeParameterIndex.ToString());
					}
					output.Write(']');
					break;
				case 0x0f: // Currency
					output.Write("currency");
					break;
				case 0x13: // BStr
					output.Write("bstr");
					break;
				case 0x15: // LPWStr
					output.Write("lpwstr");
					break;
				case 0x16: // LPTStr
					output.Write("lptstr");
					break;
				case 0x17: // FixedSysString
					output.Write("fixed sysstring[{0}]", blob.ReadCompressedInteger());
					break;
				case 0x19: // IUnknown
					output.Write("iunknown");
					break;
				case 0x1a: // IDispatch
					output.Write("idispatch");
					break;
				case 0x1b: // Struct
					output.Write("struct");
					break;
				case 0x1c: // IntF
					output.Write("interface");
					break;
				case 0x1d: // SafeArray
					output.Write("safearray ");
					if (blob.RemainingBytes > 0) {
						byte elementType = blob.ReadByte();
						switch (elementType) {
							case 0: // None
								break;
							case 2: // I2
								output.Write("int16");
								break;
							case 3: // I4
								output.Write("int32");
								break;
							case 4: // R4
								output.Write("float32");
								break;
							case 5: // R8
								output.Write("float64");
								break;
							case 6: // Currency
								output.Write("currency");
								break;
							case 7: // Date
								output.Write("date");
								break;
							case 8: // BStr
								output.Write("bstr");
								break;
							case 9: // Dispatch
								output.Write("idispatch");
								break;
							case 10: // Error
								output.Write("error");
								break;
							case 11: // Bool
								output.Write("bool");
								break;
							case 12: // Variant
								output.Write("variant");
								break;
							case 13: // Unknown
								output.Write("iunknown");
								break;
							case 14: // Decimal
								output.Write("decimal");
								break;
							case 16: // I1
								output.Write("int8");
								break;
							case 17: // UI1
								output.Write("unsigned int8");
								break;
							case 18: // UI2
								output.Write("unsigned int16");
								break;
							case 19: // UI4
								output.Write("unsigned int32");
								break;
							case 22: // Int
								output.Write("int");
								break;
							case 23: // UInt
								output.Write("unsigned int");
								break;
							default:
								output.Write(elementType.ToString());
								break;
						}
					}
					break;
				case 0x1e: // FixedArray
					output.Write("fixed array");
					output.Write("[{0}]", blob.TryReadCompressedInteger(out value) ? value : 0);
					if (blob.RemainingBytes > 0) {
						output.Write(' ');
						WriteNativeType(ref blob);
					}
					break;
				case 0x22: // ByValStr
					output.Write("byvalstr");
					break;
				case 0x23: // ANSIBStr
					output.Write("ansi bstr");
					break;
				case 0x24: // TBStr
					output.Write("tbstr");
					break;
				case 0x25: // VariantBool
					output.Write("variant bool");
					break;
				case 0x28: // ASAny
					output.Write("as any");
					break;
				case 0x2b: // LPStruct
					output.Write("lpstruct");
					break;
				case 0x2c: // CustomMarshaler
					string guidValue = blob.ReadSerializedString();
					string unmanagedType = blob.ReadSerializedString();
					string managedType = blob.ReadSerializedString();
					string cookie = blob.ReadSerializedString();

					var guid = !string.IsNullOrEmpty(guidValue) ? new Guid(guidValue) : Guid.Empty;

					output.Write("custom(\"{0}\", \"{1}\"",
								 DisassemblerHelpers.EscapeString(managedType),
								 DisassemblerHelpers.EscapeString(cookie));
					if (guid != Guid.Empty || !string.IsNullOrEmpty(unmanagedType)) {
						output.Write(", \"{0}\", \"{1}\"", guid.ToString(), DisassemblerHelpers.EscapeString(unmanagedType));
					}
					output.Write(')');
					break;
				case 0x2d: // Error
					output.Write("error");
					break;
				default:
					output.Write(type.ToString());
					break;
			}
		}
		#endregion

		void WriteParameters(MetadataReader metadata, IEnumerable<ParameterHandle> parameters, MethodSignature<Action<ILNameSyntax>> signature)
		{
			int i = 0;
			int offset = signature.Header.IsInstance ? 1 : 0;

			foreach (var h in parameters) {
				var p = metadata.GetParameter(h);
				// skip return type parameter handle
				if (p.SequenceNumber == 0) continue;

				// fill gaps in parameter list
				while (i < p.SequenceNumber - 1) {
					if (i > 0) {
						output.Write(',');
						output.WriteLine();
					}
					signature.ParameterTypes[i](ILNameSyntax.Signature);
					output.Write(' ');
					output.WriteLocalReference("''", "param_" + (i + offset), isDefinition: true);
					i++;
				}

				// separator
				if (i > 0) {
					output.Write(',');
					output.WriteLine();
				}

				// print parameter
				if ((p.Attributes & ParameterAttributes.In) == ParameterAttributes.In)
					output.Write("[in] ");
				if ((p.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out)
					output.Write("[out] ");
				if ((p.Attributes & ParameterAttributes.Optional) == ParameterAttributes.Optional)
					output.Write("[opt] ");
				signature.ParameterTypes[i](ILNameSyntax.Signature);
				output.Write(' ');
				var md = p.GetMarshallingDescriptor();
				if (!md.IsNil) {
					WriteMarshalInfo(metadata.GetBlobReader(md));
				}
				output.WriteLocalReference(DisassemblerHelpers.Escape(metadata.GetString(p.Name)), "param_" + (i + offset), isDefinition: true);
				i++;
			}

			// add remaining parameter types as unnamed parameters
			while (i < signature.RequiredParameterCount) {
				if (i > 0) {
					output.Write(',');
					output.WriteLine();
				}
				signature.ParameterTypes[i](ILNameSyntax.Signature);
				output.Write(' ');
				output.WriteLocalReference("''", "param_" + (i + offset), isDefinition: true);
				i++;
			}

			output.WriteLine();
		}

		void WriteGenericParameterAttributes(PEFile module, GenericContext context, GenericParameterHandle handle)
		{
			var metadata = module.Metadata;
			var p = metadata.GetGenericParameter(handle);
			if (p.GetCustomAttributes().Count > 0) {
				output.Write(".param type {0}", metadata.GetString(p.Name));
				output.WriteLine();
				output.Indent();
				WriteAttributes(module, p.GetCustomAttributes());
				output.Unindent();
			}
			foreach (var constraintHandle in p.GetConstraints()) {
				var constraint = metadata.GetGenericParameterConstraint(constraintHandle);
				if (constraint.GetCustomAttributes().Count > 0) {
					output.Write(".param constraint {0}, ", metadata.GetString(p.Name));
					constraint.Type.WriteTo(module, output, context, ILNameSyntax.TypeName);
					output.WriteLine();
					output.Indent();
					WriteAttributes(module, constraint.GetCustomAttributes());
					output.Unindent();
				}
			}
		}

		void WriteParameterAttributes(PEFile module, ParameterHandle handle)
		{
			var metadata = module.Metadata;
			var p = metadata.GetParameter(handle);
			if (p.GetDefaultValue().IsNil && p.GetCustomAttributes().Count == 0)
				return;
			output.Write(".param [{0}]", p.SequenceNumber);
			if (!p.GetDefaultValue().IsNil) {
				output.Write(" = ");
				WriteConstant(metadata, metadata.GetConstant(p.GetDefaultValue()));
			}
			output.WriteLine();
			output.Indent();
			WriteAttributes(module, p.GetCustomAttributes());
			output.Unindent();
		}

		void WriteConstant(MetadataReader metadata, Constant constant)
		{
			switch (constant.TypeCode) {
				case ConstantTypeCode.NullReference:
					output.Write("nullref");
					break;
				default:
					var blob = metadata.GetBlobReader(constant.Value);
					object value;
					try {
						value = blob.ReadConstant(constant.TypeCode);
					} catch (ArgumentOutOfRangeException) {
						output.Write($"/* Constant with invalid typecode: {constant.TypeCode} */");
						return;
					}
					if (value is string) {
						DisassemblerHelpers.WriteOperand(output, value);
					} else {
						string typeName = DisassemblerHelpers.PrimitiveTypeName(value.GetType().FullName);
						output.Write(typeName);
						output.Write('(');
						float? cf = value as float?;
						double? cd = value as double?;
						if (cf.HasValue && (float.IsNaN(cf.Value) || float.IsInfinity(cf.Value))) {
							output.Write("0x{0:x8}", BitConverter.ToInt32(BitConverter.GetBytes(cf.Value), 0));
						} else if (cd.HasValue && (double.IsNaN(cd.Value) || double.IsInfinity(cd.Value))) {
							output.Write("0x{0:x16}", BitConverter.DoubleToInt64Bits(cd.Value));
						} else {
							DisassemblerHelpers.WriteOperand(output, value);
						}
						output.Write(')');
					}
					break;
			}
		}
		#endregion

		#region Disassemble Field
		EnumNameCollection<FieldAttributes> fieldVisibility = new EnumNameCollection<FieldAttributes>() {
			{ FieldAttributes.Private, "private" },
			{ FieldAttributes.FamANDAssem, "famandassem" },
			{ FieldAttributes.Assembly, "assembly" },
			{ FieldAttributes.Family, "family" },
			{ FieldAttributes.FamORAssem, "famorassem" },
			{ FieldAttributes.Public, "public" },
		};

		EnumNameCollection<FieldAttributes> fieldAttributes = new EnumNameCollection<FieldAttributes>() {
			{ FieldAttributes.Static, "static" },
			{ FieldAttributes.Literal, "literal" },
			{ FieldAttributes.InitOnly, "initonly" },
			{ FieldAttributes.SpecialName, "specialname" },
			{ FieldAttributes.RTSpecialName, "rtspecialname" },
			{ FieldAttributes.NotSerialized, "notserialized" },
		};

		public void DisassembleField(PEFile module, FieldDefinitionHandle field)
		{
			var metadata = module.Metadata;
			var fieldDefinition = metadata.GetFieldDefinition(field);
			output.WriteReference(module, field, ".field ", isDefinition: true);
			int offset = fieldDefinition.GetOffset();
			if (offset > -1) {
				output.Write("[" + offset + "] ");
			}
			WriteEnum(fieldDefinition.Attributes & FieldAttributes.FieldAccessMask, fieldVisibility);
			const FieldAttributes hasXAttributes = FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal | FieldAttributes.HasFieldRVA;
			WriteFlags(fieldDefinition.Attributes & ~(FieldAttributes.FieldAccessMask | hasXAttributes), fieldAttributes);

			var signature = fieldDefinition.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), new GenericContext(fieldDefinition.GetDeclaringType(), module));

			var marshallingDescriptor = fieldDefinition.GetMarshallingDescriptor();
			if (!marshallingDescriptor.IsNil) {
				WriteMarshalInfo(metadata.GetBlobReader(marshallingDescriptor));
			}

			signature(ILNameSyntax.Signature);
			output.Write(' ');
			var fieldName = metadata.GetString(fieldDefinition.Name);
			output.Write(DisassemblerHelpers.Escape(fieldName));
			char sectionPrefix = 'D';
			if (fieldDefinition.HasFlag(FieldAttributes.HasFieldRVA)) {
				int rva = fieldDefinition.GetRelativeVirtualAddress();
				sectionPrefix = GetRVASectionPrefix(module.Reader.PEHeaders, rva);
				output.Write(" at {1}_{0:X8}", rva, sectionPrefix);
			}

			var defaultValue = fieldDefinition.GetDefaultValue();
			if (!defaultValue.IsNil) {
				output.Write(" = ");
				WriteConstant(metadata, metadata.GetConstant(defaultValue));
			}
			output.WriteLine();
			var attributes = fieldDefinition.GetCustomAttributes();
			if (attributes.Count > 0) {
				output.MarkFoldStart();
				WriteAttributes(module, fieldDefinition.GetCustomAttributes());
				output.MarkFoldEnd();
			}
			if (fieldDefinition.HasFlag(FieldAttributes.HasFieldRVA)) {
				// Field data as specified in II.16.3.1 of ECMA-335 6th edition
				int rva = fieldDefinition.GetRelativeVirtualAddress();
				int sectionIndex = module.Reader.PEHeaders.GetContainingSectionIndex(rva);
				if (sectionIndex < 0) {
					output.WriteLine($"// RVA {rva:X8} invalid (not in any section)");
				} else {
					BlobReader initVal;
					try {
						initVal = fieldDefinition.GetInitialValue(module.Reader, null);
					} catch (BadImageFormatException ex) {
						initVal = default;
						output.WriteLine("// .data {2}_{0:X8} = {1}", fieldDefinition.GetRelativeVirtualAddress(), ex.Message, sectionPrefix);
					}
					if (initVal.Length > 0) {
						var sectionHeader = module.Reader.PEHeaders.SectionHeaders[sectionIndex];
						output.Write(".data ");
						if (sectionHeader.Name == ".text") {
							output.Write("cil ");
						} else if (sectionHeader.Name == ".tls") {
							output.Write("tls ");
						} else if (sectionHeader.Name != ".data") {
							output.Write($"/* {sectionHeader.Name} */ ");
						}
						output.Write($"{sectionPrefix}_{rva:X8} = bytearray ");
						WriteBlob(initVal);
						output.WriteLine();
					}
				}
			}
		}

		char GetRVASectionPrefix(System.Reflection.PortableExecutable.PEHeaders headers, int rva)
		{
			int sectionIndex = headers.GetContainingSectionIndex(rva);
			if (sectionIndex < 0)
				return 'D';
			var sectionHeader = headers.SectionHeaders[sectionIndex];
			switch (sectionHeader.Name) {
				case ".tls":
					return 'T';
				case ".text":
					return 'I';
				default:
					return 'D';
			}
		}
		#endregion

		#region Disassemble Property
		EnumNameCollection<PropertyAttributes> propertyAttributes = new EnumNameCollection<PropertyAttributes>() {
			{ PropertyAttributes.SpecialName, "specialname" },
			{ PropertyAttributes.RTSpecialName, "rtspecialname" },
			{ PropertyAttributes.HasDefault, "hasdefault" },
		};

		public void DisassembleProperty(PEFile module, PropertyDefinitionHandle property)
		{
			var metadata = module.Metadata;
			var propertyDefinition = metadata.GetPropertyDefinition(property);
			output.WriteReference(module, property, ".property", isDefinition: true);
			output.Write(" ");
			WriteFlags(propertyDefinition.Attributes, propertyAttributes);
			var accessors = propertyDefinition.GetAccessors();
			var declaringType = metadata.GetMethodDefinition(accessors.GetAny()).GetDeclaringType();
			var signature = propertyDefinition.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), new GenericContext(declaringType, module));

			if (signature.Header.IsInstance)
				output.Write("instance ");
			signature.ReturnType(ILNameSyntax.Signature);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(propertyDefinition.Name)));

			output.Write('(');
			if (signature.ParameterTypes.Length > 0) {
				var parameters = metadata.GetMethodDefinition(accessors.GetAny()).GetParameters();
				int parametersCount = accessors.Getter.IsNil ? parameters.Count - 1 : parameters.Count;

				output.WriteLine();
				output.Indent();
				WriteParameters(metadata, parameters.Take(parametersCount), signature);
				output.Unindent();
			}
			output.Write(')');

			OpenBlock(false);
			WriteAttributes(module, propertyDefinition.GetCustomAttributes());
			WriteNestedMethod(".get", module, accessors.Getter);
			WriteNestedMethod(".set", module, accessors.Setter);
			foreach (var method in accessors.Others) {
				WriteNestedMethod(".other", module, method);
			}
			CloseBlock();
		}

		void WriteNestedMethod(string keyword, PEFile module, MethodDefinitionHandle method)
		{
			if (method.IsNil)
				return;

			output.Write(keyword);
			output.Write(' ');
			((EntityHandle)method).WriteTo(module, output, GenericContext.Empty);
			output.WriteLine();
		}
		#endregion

		#region Disassemble Event
		EnumNameCollection<EventAttributes> eventAttributes = new EnumNameCollection<EventAttributes>() {
			{ EventAttributes.SpecialName, "specialname" },
			{ EventAttributes.RTSpecialName, "rtspecialname" },
		};

		public void DisassembleEvent(PEFile module, EventDefinitionHandle handle)
		{
			var eventDefinition = module.Metadata.GetEventDefinition(handle);
			var accessors = eventDefinition.GetAccessors();
			TypeDefinitionHandle declaringType;
			if (!accessors.Adder.IsNil) {
				declaringType = module.Metadata.GetMethodDefinition(accessors.Adder).GetDeclaringType();
			} else if (!accessors.Remover.IsNil) {
				declaringType = module.Metadata.GetMethodDefinition(accessors.Remover).GetDeclaringType();
			} else {
				declaringType = module.Metadata.GetMethodDefinition(accessors.Raiser).GetDeclaringType();
			}
			output.WriteReference(module, handle, ".event", isDefinition: true);
			output.Write(" ");
			WriteFlags(eventDefinition.Attributes, eventAttributes);
			var provider = new DisassemblerSignatureTypeProvider(module, output);
			Action<ILNameSyntax> signature;
			switch (eventDefinition.Type.Kind) {
				case HandleKind.TypeDefinition:
					signature = provider.GetTypeFromDefinition(module.Metadata, (TypeDefinitionHandle)eventDefinition.Type, 0);
					break;
				case HandleKind.TypeReference:
					signature = provider.GetTypeFromReference(module.Metadata, (TypeReferenceHandle)eventDefinition.Type, 0);
					break;
				case HandleKind.TypeSpecification:
					signature = provider.GetTypeFromSpecification(module.Metadata, new GenericContext(declaringType, module),
						(TypeSpecificationHandle)eventDefinition.Type, 0);
					break;
				default:
					throw new BadImageFormatException("Expected a TypeDef, TypeRef or TypeSpec handle!");
			}
			signature(ILNameSyntax.TypeName);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(module.Metadata.GetString(eventDefinition.Name)));
			OpenBlock(false);
			WriteAttributes(module, eventDefinition.GetCustomAttributes());
			WriteNestedMethod(".addon", module, accessors.Adder);
			WriteNestedMethod(".removeon", module, accessors.Remover);
			WriteNestedMethod(".fire", module, accessors.Raiser);
			foreach (var method in accessors.Others) {
				WriteNestedMethod(".other", module, method);
			}
			CloseBlock();
		}
		#endregion

		#region Disassemble Type
		EnumNameCollection<TypeAttributes> typeVisibility = new EnumNameCollection<TypeAttributes>() {
			{ TypeAttributes.Public, "public" },
			{ TypeAttributes.NotPublic, "private" },
			{ TypeAttributes.NestedPublic, "nested public" },
			{ TypeAttributes.NestedPrivate, "nested private" },
			{ TypeAttributes.NestedAssembly, "nested assembly" },
			{ TypeAttributes.NestedFamily, "nested family" },
			{ TypeAttributes.NestedFamANDAssem, "nested famandassem" },
			{ TypeAttributes.NestedFamORAssem, "nested famorassem" },
		};

		EnumNameCollection<TypeAttributes> typeLayout = new EnumNameCollection<TypeAttributes>() {
			{ TypeAttributes.AutoLayout, "auto" },
			{ TypeAttributes.SequentialLayout, "sequential" },
			{ TypeAttributes.ExplicitLayout, "explicit" },
		};

		EnumNameCollection<TypeAttributes> typeStringFormat = new EnumNameCollection<TypeAttributes>() {
			{ TypeAttributes.AutoClass, "auto" },
			{ TypeAttributes.AnsiClass, "ansi" },
			{ TypeAttributes.UnicodeClass, "unicode" },
		};

		EnumNameCollection<TypeAttributes> typeAttributes = new EnumNameCollection<TypeAttributes>() {
			{ TypeAttributes.Abstract, "abstract" },
			{ TypeAttributes.Sealed, "sealed" },
			{ TypeAttributes.SpecialName, "specialname" },
			{ TypeAttributes.Import, "import" },
			{ TypeAttributes.Serializable, "serializable" },
			{ TypeAttributes.WindowsRuntime, "windowsruntime" },
			{ TypeAttributes.BeforeFieldInit, "beforefieldinit" },
			{ TypeAttributes.HasSecurity, null },
		};

		public void DisassembleType(PEFile module, TypeDefinitionHandle type)
		{
			var typeDefinition = module.Metadata.GetTypeDefinition(type);
			output.WriteReference(module, type, ".class", isDefinition: true);
			output.Write(" ");
			if ((typeDefinition.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface)
				output.Write("interface ");
			WriteEnum(typeDefinition.Attributes & TypeAttributes.VisibilityMask, typeVisibility);
			WriteEnum(typeDefinition.Attributes & TypeAttributes.LayoutMask, typeLayout);
			WriteEnum(typeDefinition.Attributes & TypeAttributes.StringFormatMask, typeStringFormat);
			const TypeAttributes masks = TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.StringFormatMask;
			WriteFlags(typeDefinition.Attributes & ~masks, typeAttributes);

			output.Write(typeDefinition.GetDeclaringType().IsNil ? typeDefinition.GetFullTypeName(module.Metadata).ToILNameString() : DisassemblerHelpers.Escape(module.Metadata.GetString(typeDefinition.Name)));
			GenericContext genericContext = new GenericContext(type, module);
			WriteTypeParameters(output, module, genericContext, typeDefinition.GetGenericParameters());
			output.MarkFoldStart(defaultCollapsed: !ExpandMemberDefinitions && isInType);
			output.WriteLine();

			EntityHandle baseType = typeDefinition.GetBaseTypeOrNil();
			if (!baseType.IsNil) {
				output.Indent();
				output.Write("extends ");
				baseType.WriteTo(module, output, genericContext, ILNameSyntax.TypeName);
				output.WriteLine();
				output.Unindent();
			}

			var interfaces = typeDefinition.GetInterfaceImplementations();
			if (interfaces.Count > 0) {
				output.Indent();
				bool first = true;
				foreach (var i in interfaces) {
					if (!first)
						output.WriteLine(",");
					if (first)
						output.Write("implements ");
					else
						output.Write("           ");
					first = false;
					var iface = module.Metadata.GetInterfaceImplementation(i);
					WriteAttributes(module, iface.GetCustomAttributes());
					iface.Interface.WriteTo(module, output, genericContext, ILNameSyntax.TypeName);
				}
				output.WriteLine();
				output.Unindent();
			}

			output.WriteLine("{");
			output.Indent();
			bool oldIsInType = isInType;
			isInType = true;
			WriteAttributes(module, typeDefinition.GetCustomAttributes());
			WriteSecurityDeclarations(module, typeDefinition.GetDeclarativeSecurityAttributes());
			foreach (var tp in typeDefinition.GetGenericParameters()) {
				WriteGenericParameterAttributes(module, genericContext, tp);
			}
			var layout = typeDefinition.GetLayout();
			if (!layout.IsDefault) {
				output.WriteLine(".pack {0}", layout.PackingSize);
				output.WriteLine(".size {0}", layout.Size);
				output.WriteLine();
			}
			var nestedTypes = typeDefinition.GetNestedTypes();
			if (!nestedTypes.IsEmpty) {
				output.WriteLine("// Nested Types");
				foreach (var nestedType in nestedTypes) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleType(module, nestedType);
					output.WriteLine();
				}
				output.WriteLine();
			}
			var fields = typeDefinition.GetFields();
			if (fields.Any()) {
				output.WriteLine("// Fields");
				foreach (var field in fields) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleField(module, field);
				}
				output.WriteLine();
			}
			var methods = typeDefinition.GetMethods();
			if (methods.Any()) {
				output.WriteLine("// Methods");
				foreach (var m in methods) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleMethod(module, m);
					output.WriteLine();
				}
			}
			var events = typeDefinition.GetEvents();
			if (events.Any()) {
				output.WriteLine("// Events");
				foreach (var ev in events) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleEvent(module, ev);
					output.WriteLine();
				}
				output.WriteLine();
			}
			var properties = typeDefinition.GetProperties();
			if (properties.Any()) {
				output.WriteLine("// Properties");
				foreach (var prop in properties) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleProperty(module, prop);
				}
				output.WriteLine();
			}
			CloseBlock("end of class " + (!typeDefinition.GetDeclaringType().IsNil ? module.Metadata.GetString(typeDefinition.Name) : typeDefinition.GetFullTypeName(module.Metadata).ToString()));
			isInType = oldIsInType;
		}

		void WriteTypeParameters(ITextOutput output, PEFile module, GenericContext context, GenericParameterHandleCollection p)
		{
			if (p.Count > 0) {
				output.Write('<');
				var metadata = module.Metadata;
				for (int i = 0; i < p.Count; i++) {
					if (i > 0)
						output.Write(", ");
					var gp = metadata.GetGenericParameter(p[i]);
					if ((gp.Attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint) {
						output.Write("class ");
					} else if ((gp.Attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint) {
						output.Write("valuetype ");
					}
					if ((gp.Attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint) {
						output.Write(".ctor ");
					}
					var constraints = gp.GetConstraints();
					if (constraints.Count > 0) {
						output.Write('(');
						for (int j = 0; j < constraints.Count; j++) {
							if (j > 0)
								output.Write(", ");
							var constraint = metadata.GetGenericParameterConstraint(constraints[j]);
							constraint.Type.WriteTo(module, output, context, ILNameSyntax.TypeName);
						}
						output.Write(") ");
					}
					if ((gp.Attributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant) {
						output.Write('-');
					} else if ((gp.Attributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant) {
						output.Write('+');
					}
					output.Write(DisassemblerHelpers.Escape(metadata.GetString(gp.Name)));
				}
				output.Write('>');
			}
		}
		#endregion

		#region Helper methods
		void WriteAttributes(PEFile module, CustomAttributeHandleCollection attributes)
		{
			var metadata = module.Metadata;
			foreach (CustomAttributeHandle a in attributes) {
				output.Write(".custom ");
				var attr = metadata.GetCustomAttribute(a);
				attr.Constructor.WriteTo(module, output, GenericContext.Empty);
				if (!attr.Value.IsNil) {
					output.Write(" = ");
					WriteBlob(attr.Value, metadata);
				}
				output.WriteLine();
			}
		}

		void WriteBlob(BlobHandle blob, MetadataReader metadata)
		{
			var reader = metadata.GetBlobReader(blob);
			WriteBlob(reader);
		}

		void WriteBlob(BlobReader reader)
		{
			output.Write("(");
			output.Indent();

			for (int i = 0; i < reader.Length; i++) {
				if (i % 16 == 0 && i < reader.Length - 1) {
					output.WriteLine();
				} else {
					output.Write(' ');
				}
				output.Write(reader.ReadByte().ToString("x2"));
			}

			output.WriteLine();
			output.Unindent();
			output.Write(")");
		}

		void OpenBlock(bool defaultCollapsed)
		{
			output.MarkFoldStart(defaultCollapsed: !ExpandMemberDefinitions && defaultCollapsed);
			output.WriteLine();
			output.WriteLine("{");
			output.Indent();
		}

		void CloseBlock(string comment = null)
		{
			output.Unindent();
			output.Write("}");
			if (comment != null)
				output.Write(" // " + comment);
			output.MarkFoldEnd();
			output.WriteLine();
		}

		void WriteFlags<T>(T flags, EnumNameCollection<T> flagNames) where T : struct
		{
			long val = Convert.ToInt64(flags);
			long tested = 0;
			foreach (var pair in flagNames) {
				tested |= pair.Key;
				if ((val & pair.Key) != 0 && pair.Value != null) {
					output.Write(pair.Value);
					output.Write(' ');
				}
			}
			if ((val & ~tested) != 0)
				output.Write("flag({0:x4}) ", val & ~tested);
		}

		void WriteEnum<T>(T enumValue, EnumNameCollection<T> enumNames) where T : struct
		{
			long val = Convert.ToInt64(enumValue);
			foreach (var pair in enumNames) {
				if (pair.Key == val) {
					if (pair.Value != null) {
						output.Write(pair.Value);
						output.Write(' ');
					}
					return;
				}
			}
			if (val != 0) {
				output.Write("flag({0:x4})", val);
				output.Write(' ');
			}

		}

		sealed class EnumNameCollection<T> : IEnumerable<KeyValuePair<long, string>> where T : struct
		{
			List<KeyValuePair<long, string>> names = new List<KeyValuePair<long, string>>();

			public void Add(T flag, string name)
			{
				this.names.Add(new KeyValuePair<long, string>(Convert.ToInt64(flag), name));
			}

			public IEnumerator<KeyValuePair<long, string>> GetEnumerator()
			{
				return names.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return names.GetEnumerator();
			}
		}
		#endregion

		public void DisassembleNamespace(string nameSpace, PEFile module, IEnumerable<TypeDefinitionHandle> types)
		{
			if (!string.IsNullOrEmpty(nameSpace)) {
				output.Write(".namespace " + DisassemblerHelpers.Escape(nameSpace));
				OpenBlock(false);
			}
			bool oldIsInType = isInType;
			isInType = true;
			foreach (var td in types) {
				cancellationToken.ThrowIfCancellationRequested();
				DisassembleType(module, td);
				output.WriteLine();
			}
			if (!string.IsNullOrEmpty(nameSpace)) {
				CloseBlock();
				isInType = oldIsInType;
			}
		}

		public void WriteAssemblyHeader(PEFile module)
		{
			var metadata = module.Metadata;
			if (!metadata.IsAssembly) return;
			output.Write(".assembly ");
			var asm = metadata.GetAssemblyDefinition();
			if ((asm.Flags & AssemblyFlags.WindowsRuntime) == AssemblyFlags.WindowsRuntime)
				output.Write("windowsruntime ");
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(asm.Name)));
			OpenBlock(false);
			WriteAttributes(module, asm.GetCustomAttributes());
			WriteSecurityDeclarations(module, asm.GetDeclarativeSecurityAttributes());
			if (!asm.PublicKey.IsNil) {
				output.Write(".publickey = ");
				WriteBlob(asm.PublicKey, metadata);
				output.WriteLine();
			}
			if (asm.HashAlgorithm != AssemblyHashAlgorithm.None) {
				output.Write(".hash algorithm 0x{0:x8}", (int)asm.HashAlgorithm);
				if (asm.HashAlgorithm == AssemblyHashAlgorithm.Sha1)
					output.Write(" // SHA1");
				output.WriteLine();
			}
			Version v = asm.Version;
			if (v != null) {
				output.WriteLine(".ver {0}:{1}:{2}:{3}", v.Major, v.Minor, v.Build, v.Revision);
			}
			CloseBlock();
		}

		public void WriteAssemblyReferences(MetadataReader metadata)
		{
			foreach (var m in metadata.GetModuleReferences()) {
				var mref = metadata.GetModuleReference(m);
				output.WriteLine(".module extern {0}", DisassemblerHelpers.Escape(metadata.GetString(mref.Name)));
			}
			foreach (var a in metadata.AssemblyReferences) {
				var aref = metadata.GetAssemblyReference(a);
				output.Write(".assembly extern ");
				if ((aref.Flags & AssemblyFlags.WindowsRuntime) == AssemblyFlags.WindowsRuntime)
					output.Write("windowsruntime ");
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(aref.Name)));
				OpenBlock(false);
				if (!aref.PublicKeyOrToken.IsNil) {
					output.Write(".publickeytoken = ");
					WriteBlob(aref.PublicKeyOrToken, metadata);
					output.WriteLine();
				}
				if (aref.Version != null) {
					output.WriteLine(".ver {0}:{1}:{2}:{3}", aref.Version.Major, aref.Version.Minor, aref.Version.Build, aref.Version.Revision);
				}
				CloseBlock();
			}
		}

		public void WriteModuleHeader(PEFile module, bool skipMVID = false)
		{
			var metadata = module.Metadata;

			void WriteExportedType(ExportedType exportedType)
			{
				if (!exportedType.Namespace.IsNil) {
					output.Write(DisassemblerHelpers.Escape(metadata.GetString(exportedType.Namespace)));
					output.Write('.');
				}
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(exportedType.Name)));
			}

			foreach (var et in metadata.ExportedTypes) {
				var exportedType = metadata.GetExportedType(et);
				output.Write(".class extern ");
				if (exportedType.IsForwarder)
					output.Write("forwarder ");
				WriteExportedType(exportedType);
				OpenBlock(false);
				switch (exportedType.Implementation.Kind) {
					case HandleKind.AssemblyFile:
						var file = metadata.GetAssemblyFile((AssemblyFileHandle)exportedType.Implementation);
						output.WriteLine(".file {0}", metadata.GetString(file.Name));
						int typeDefId = exportedType.GetTypeDefinitionId();
						if (typeDefId != 0)
							output.WriteLine(".class 0x{0:x8}", typeDefId);
						break;
					case HandleKind.ExportedType:
						output.Write(".class extern ");
						var declaringType = metadata.GetExportedType((ExportedTypeHandle)exportedType.Implementation);
						while (true) {
							WriteExportedType(declaringType);
							if (declaringType.Implementation.Kind == HandleKind.ExportedType) {
								declaringType = metadata.GetExportedType((ExportedTypeHandle)declaringType.Implementation);
							} else {
								break;
							}
						}
						output.WriteLine();
						break;
					case HandleKind.AssemblyReference:
						output.Write(".assembly extern ");
						var reference = metadata.GetAssemblyReference((AssemblyReferenceHandle)exportedType.Implementation);
						output.Write(DisassemblerHelpers.Escape(metadata.GetString(reference.Name)));
						output.WriteLine();
						break;
					default:
						throw new BadImageFormatException("Implementation must either be an index into the File, ExportedType or AssemblyRef table.");
				}
				CloseBlock();
			}
			var moduleDefinition = metadata.GetModuleDefinition();

			output.WriteLine(".module {0}", metadata.GetString(moduleDefinition.Name));
			if (!skipMVID) {
				output.WriteLine("// MVID: {0}", metadata.GetGuid(moduleDefinition.Mvid).ToString("B").ToUpperInvariant());
			}

			var headers = module.Reader.PEHeaders;
			output.WriteLine(".imagebase 0x{0:x8}", headers.PEHeader.ImageBase);
			output.WriteLine(".file alignment 0x{0:x8}", headers.PEHeader.FileAlignment);
			output.WriteLine(".stackreserve 0x{0:x8}", headers.PEHeader.SizeOfStackReserve);
			output.WriteLine(".subsystem 0x{0:x} // {1}", headers.PEHeader.Subsystem, headers.PEHeader.Subsystem.ToString());
			output.WriteLine(".corflags 0x{0:x} // {1}", headers.CorHeader.Flags, headers.CorHeader.Flags.ToString());

			WriteAttributes(module, metadata.GetCustomAttributes(EntityHandle.ModuleDefinition));
		}

		public void WriteModuleContents(PEFile module)
		{
			foreach (var handle in module.Metadata.GetTopLevelTypeDefinitions()) {
				DisassembleType(module, handle);
				output.WriteLine();
			}
		}
	}
}