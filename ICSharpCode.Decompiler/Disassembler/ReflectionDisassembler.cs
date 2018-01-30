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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading;

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
		MetadataReader metadata;
		PEReader reader;

		public bool DetectControlStructure {
			get => methodBodyDisassembler.DetectControlStructure;
			set => methodBodyDisassembler.DetectControlStructure = value;
		}

		public bool ShowSequencePoints {
			get => methodBodyDisassembler.ShowSequencePoints;
			set => methodBodyDisassembler.ShowSequencePoints = value;
		}

		public ReflectionDisassembler(ITextOutput output, PEReader reader, CancellationToken cancellationToken)
			: this(output, reader, new MethodBodyDisassembler(output, reader, cancellationToken), cancellationToken)
		{
		}

		public ReflectionDisassembler(ITextOutput output, PEReader reader, MethodBodyDisassembler methodBodyDisassembler, CancellationToken cancellationToken)
		{
			if (output == null)
				throw new ArgumentNullException(nameof(output));
			this.output = output;
			this.cancellationToken = cancellationToken;
			this.methodBodyDisassembler = methodBodyDisassembler;
			this.reader = reader;
			this.metadata = reader.GetMetadataReader();
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
		};

		public void DisassembleMethod(MethodDefinitionHandle method)
		{
			DisassembleMethodHeader(method);
			DisassembleMethodBlock(method);
		}

		public void DisassembleMethodHeader(MethodDefinitionHandle method)
		{
			// write method header
			output.WriteDefinition(".method ", method);
			DisassembleMethodHeaderInternal(method);
		}

		void DisassembleMethodHeaderInternal(MethodDefinitionHandle handle)
		{
			//    .method public hidebysig  specialname
			//               instance default class [mscorlib]System.IO.TextWriter get_BaseWriter ()  cil managed
			//
			var method = metadata.GetMethodDefinition(handle);
			//emit flags
			WriteEnum(method.Attributes & MethodAttributes.MemberAccessMask, methodVisibility);
			WriteFlags(method.Attributes & ~MethodAttributes.MemberAccessMask, methodAttributeFlags);
			bool isCompilerControlled = (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			if (isCompilerControlled)
				output.Write("privatescope ");

			if ((method.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl) {
				output.Write("pinvokeimpl");
				var info = method.GetImport();
				if (!info.Module.IsNil) {
					var moduleRef = metadata.GetModuleReference(info.Module);
					output.Write("(\"" + DisassemblerHelpers.EscapeString(metadata.GetString(moduleRef.Name)) + "\"");

					if (!info.Name.IsNil && info.Name != method.Name)
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
			var declaringType = metadata.GetTypeDefinition(method.GetDeclaringType());
			var reader = metadata.GetBlobReader(method.Signature);
			var signature = method.DecodeSignature(new MethodSignatureProvider(metadata, output), (declaringType.GetGenericParameters(), method.GetGenericParameters()));
			if (signature.Header.HasExplicitThis) {
				output.Write("instance explicit ");
			} else if (signature.Header.IsInstance) {
				output.Write("instance ");
			}

			//call convention
			WriteEnum(signature.Header.CallingConvention, callingConvention);

			//return type
			signature.ReturnType(ILNameSyntax.Signature);
			output.Write(' ');

			var parameters = method.GetParameters().ToImmutableArray();
			if (parameters.Length > 0) {
				var marshallingDesc = metadata.GetParameter(parameters[0]).GetMarshallingDescriptor();

				if (!marshallingDesc.IsNil) {
					WriteMarshalInfo(metadata.GetBlobReader(marshallingDesc));
				}
			}

			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(method.Name) + "$PST" + MetadataTokens.GetToken(handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(method.Name)));
			}

			WriteTypeParameters(output, method.GetGenericParameters());

			//( params )
			output.Write(" (");
			if (signature.ParameterTypes.Length > 0) {
				output.WriteLine();
				output.Indent();
				WriteParameters(parameters, signature);
				output.Unindent();
			}
			output.Write(") ");
			//cil managed
			WriteEnum(method.ImplAttributes & MethodImplAttributes.CodeTypeMask, methodCodeType);
			if ((method.ImplAttributes & MethodImplAttributes.ManagedMask) == MethodImplAttributes.Managed)
				output.Write("managed ");
			else
				output.Write("unmanaged ");
			WriteFlags(method.ImplAttributes & ~(MethodImplAttributes.CodeTypeMask | MethodImplAttributes.ManagedMask), methodImpl);

			output.Unindent();
		}

		void DisassembleMethodBlock(MethodDefinitionHandle handle)
		{
			var method = metadata.GetMethodDefinition(handle);
			OpenBlock(defaultCollapsed: isInType);
			WriteAttributes(method.GetCustomAttributes());
			// TODO: implement cache?
			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.MethodImpl); row++) {
				var impl = metadata.GetMethodImplementation(MetadataTokens.MethodImplementationHandle(row));
				if (impl.MethodBody != handle) continue;
				output.Write(".override method ");
				impl.MethodDeclaration.WriteTo(metadata, output);
				output.WriteLine();
			}

			foreach (var p in method.GetParameters()) {
				WriteParameterAttributes(p);
			}
			//WriteSecurityDeclarations(method);

			bool hasBody = (method.Attributes & MethodAttributes.Abstract) == 0 &&
				(method.Attributes & MethodAttributes.PinvokeImpl) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.InternalCall) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Native) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Unmanaged) == 0 &&
				(method.ImplAttributes & MethodImplAttributes.Runtime) == 0;

			if (hasBody) {
				methodBodyDisassembler.Disassemble(handle);
			}
			var td = metadata.GetTypeDefinition(method.GetDeclaringType());
			CloseBlock("end of method " + DisassemblerHelpers.Escape(metadata.GetString(td.Name)) + "::" + DisassemblerHelpers.Escape(metadata.GetString(method.Name)));
		}

		#region Write Security Declarations
		void WriteSecurityDeclarations(DeclarativeSecurityAttributeHandleCollection secDeclProvider)
		{
			if (secDeclProvider.Count == 0)
				return;/*
			foreach (var h in secDeclProvider) {
				output.Write(".permissionset ");
				var secdecl = metadata.GetDeclarativeSecurityAttribute(h);
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
				output.WriteLine(" = {");
				output.Indent();
				
				for (int i = 0; i < secdecl.SecurityAttributes.Count; i++) {
					SecurityAttribute sa = secdecl.SecurityAttributes[i];
					if (sa.AttributeType.Scope == sa.AttributeType.Module) {
						output.Write("class ");
						output.Write(DisassemblerHelpers.Escape(GetAssemblyQualifiedName(sa.AttributeType)));
					} else {
						sa.AttributeType.WriteTo(output, ILNameSyntax.TypeName);
					}
					output.Write(" = {");
					if (sa.HasFields || sa.HasProperties) {
						output.WriteLine();
						output.Indent();

						foreach (CustomAttributeNamedArgument na in sa.Fields) {
							output.Write("field ");
							WriteSecurityDeclarationArgument(na);
							output.WriteLine();
						}

						foreach (CustomAttributeNamedArgument na in sa.Properties) {
							output.Write("property ");
							WriteSecurityDeclarationArgument(na);
							output.WriteLine();
						}

						output.Unindent();
					}
					output.Write('}');

					if (i + 1 < secdecl.SecurityAttributes.Count)
						output.Write(',');
					output.WriteLine();
				}
				output.Unindent();
				output.WriteLine("}");
			}*/
		}
		/*
		void WriteSecurityDeclarationArgument(CustomAttributeNamedArgument na)
		{
			TypeReference type = na.Argument.Type;
			if (type.MetadataType == MetadataType.Class || type.MetadataType == MetadataType.ValueType) {
				output.Write("enum ");
				if (type.Scope != type.Module) {
					output.Write("class ");
					output.Write(DisassemblerHelpers.Escape(GetAssemblyQualifiedName(type)));
				} else {
					type.WriteTo(output, ILNameSyntax.TypeName);
				}
			} else {
				type.WriteTo(output);
			}
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(na.Name));
			output.Write(" = ");
			if (na.Argument.Value is string) {
				// secdecls use special syntax for strings
				output.Write("string('{0}')", DisassemblerHelpers.EscapeString((string)na.Argument.Value).Replace("'", "\'"));
			} else {
				WriteConstant(na.Argument.Value);
			}
		}

		string GetAssemblyQualifiedName(TypeReference type)
		{
			AssemblyNameReference anr = type.Scope as AssemblyNameReference;
			if (anr == null) {
				ModuleDefinition md = type.Scope as ModuleDefinition;
				if (md != null) {
					anr = md.Assembly.Name;
				}
			}
			if (anr != null) {
				return type.FullName + ", " + anr.FullName;
			} else {
				return type.FullName;
			}
		}*/
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
					goto default;  // ??
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
					break;
				case 0x1e: // FixedArray
					output.Write("fixed array");
					output.Write("[{0}]", blob.TryReadCompressedInteger(out value) ? value : 0);
					output.Write(' ');
					WriteNativeType(ref blob);
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

		void WriteParameters(ImmutableArray<ParameterHandle> parameters, MethodSignature<Action<ILNameSyntax>> signature)
		{
			int parameterOffset = parameters.Length > signature.ParameterTypes.Length ? 1 : 0;
			for (int i = 0; i < signature.ParameterTypes.Length; i++) {
				var p = metadata.GetParameter(parameters[i + parameterOffset]);
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
				output.WriteDefinition(DisassemblerHelpers.Escape(metadata.GetString(p.Name)), p);
				if (i < parameters.Length - 1)
					output.Write(',');
				output.WriteLine();
			}
		}

		void WriteParameterAttributes(ParameterHandle handle)
		{
			var p = metadata.GetParameter(handle);
			if (p.GetDefaultValue().IsNil && p.GetCustomAttributes().Count == 0)
				return;
			output.Write(".param [{0}]", p.SequenceNumber);
			if (!p.GetDefaultValue().IsNil) {
				output.Write(" = ");
				WriteConstant(metadata.GetConstant(p.GetDefaultValue()));
			}
			output.WriteLine();
			WriteAttributes(p.GetCustomAttributes());
		}

		void WriteConstant(Constant constant)
		{
			var blob = metadata.GetBlobReader(constant.Value);
			switch (constant.TypeCode) {
				case ConstantTypeCode.NullReference:
					output.Write("nullref");
					break;
				default:
					var value = blob.ReadConstant(constant.TypeCode);
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

		public void DisassembleField(FieldDefinitionHandle handle)
		{
			var field = metadata.GetFieldDefinition(handle);
			output.WriteDefinition(".field ", field);
			int offset = field.GetOffset();
			if (offset > -1) {
				output.Write("[" + offset + "] ");
			}
			WriteEnum(field.Attributes & FieldAttributes.FieldAccessMask, fieldVisibility);
			const FieldAttributes hasXAttributes = FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal | FieldAttributes.HasFieldRVA;
			WriteFlags(field.Attributes & ~(FieldAttributes.FieldAccessMask | hasXAttributes), fieldAttributes);

			var declaringType = metadata.GetTypeDefinition(field.GetDeclaringType());
			var reader = metadata.GetBlobReader(field.Signature);
			var signature = field.DecodeSignature(new MethodSignatureProvider(metadata, output), (declaringType.GetGenericParameters(), default(GenericParameterHandleCollection)));

			if (!field.GetMarshallingDescriptor().IsNil) {
				WriteMarshalInfo(metadata.GetBlobReader(field.GetMarshallingDescriptor()));
			}
			signature(ILNameSyntax.Signature);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(field.Name)));
			if ((field.Attributes & FieldAttributes.HasFieldRVA) == FieldAttributes.HasFieldRVA) {
				output.Write(" at I_{0:x8}", field.GetRelativeVirtualAddress());
			}
			if (!field.GetDefaultValue().IsNil) {
				output.Write(" = ");
				WriteConstant(metadata.GetConstant(field.GetDefaultValue()));
			}
			output.WriteLine();
			if (field.GetCustomAttributes().Count > 0) {
				output.MarkFoldStart();
				WriteAttributes(field.GetCustomAttributes());
				output.MarkFoldEnd();
			}
		}
		#endregion

		#region Disassemble Property
		EnumNameCollection<PropertyAttributes> propertyAttributes = new EnumNameCollection<PropertyAttributes>() {
			{ PropertyAttributes.SpecialName, "specialname" },
			{ PropertyAttributes.RTSpecialName, "rtspecialname" },
			{ PropertyAttributes.HasDefault, "hasdefault" },
		};

		public void DisassembleProperty(PropertyDefinitionHandle handle)
		{
			var property = metadata.GetPropertyDefinition(handle);
			output.WriteDefinition(".property ", property);
			WriteFlags(property.Attributes, propertyAttributes);

			var declaringType = metadata.GetTypeDefinition(FindDeclaringTypeForProperty(handle));
			var reader = metadata.GetBlobReader(property.Signature);
			var signature = property.DecodeSignature(new MethodSignatureProvider(metadata, output), (declaringType.GetGenericParameters(), default(GenericParameterHandleCollection)));

			if (signature.Header.IsInstance)
				output.Write("instance ");
			signature.ReturnType(ILNameSyntax.Signature);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(property.Name)));

			output.Write("(");
			var parameters = metadata.GetMethodDefinition(property.GetAccessors().Getter).GetParameters();
			if (parameters.Count > 0) {
				output.WriteLine();
				output.Indent();
				WriteParameters(parameters.ToImmutableArray(), signature);
				output.Unindent();
			}
			output.Write(")");

			OpenBlock(false);
			WriteAttributes(property.GetCustomAttributes());
			WriteNestedMethod(".get", property.GetAccessors().Getter);
			WriteNestedMethod(".set", property.GetAccessors().Setter);
			CloseBlock();
		}

		void WriteNestedMethod(string keyword, MethodDefinitionHandle handle)
		{
			if (handle.IsNil)
				return;

			output.Write(keyword);
			output.Write(' ');
			handle.WriteTo(metadata, output);
			output.WriteLine();
		}
		#endregion

		#region Disassemble Event
		EnumNameCollection<EventAttributes> eventAttributes = new EnumNameCollection<EventAttributes>() {
			{ EventAttributes.SpecialName, "specialname" },
			{ EventAttributes.RTSpecialName, "rtspecialname" },
		};

		public void DisassembleEvent(EventDefinitionHandle handle)
		{
			var ev = metadata.GetEventDefinition(handle);
			output.WriteDefinition(".event ", ev);
			WriteFlags(ev.Attributes, eventAttributes);
			ev.Type.WriteTo(metadata, output, ILNameSyntax.TypeName);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(ev.Name)));
			OpenBlock(false);
			WriteAttributes(ev.GetCustomAttributes());
			WriteNestedMethod(".addon", ev.GetAccessors().Adder);
			WriteNestedMethod(".removeon", ev.GetAccessors().Remover);
			WriteNestedMethod(".fire", ev.GetAccessors().Raiser);
			/*foreach (var method in ev.OtherMethods) {
				WriteNestedMethod(".other", method);
			}*/
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

		public void DisassembleType(TypeDefinitionHandle handle)
		{
			var type = metadata.GetTypeDefinition(handle);
			output.WriteDefinition(".class ", type);

			if ((type.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface)
				output.Write("interface ");
			WriteEnum(type.Attributes & TypeAttributes.VisibilityMask, typeVisibility);
			WriteEnum(type.Attributes & TypeAttributes.LayoutMask, typeLayout);
			WriteEnum(type.Attributes & TypeAttributes.StringFormatMask, typeStringFormat);
			const TypeAttributes masks = TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.StringFormatMask;
			WriteFlags(type.Attributes & ~masks, typeAttributes);

			string typeName = !type.GetDeclaringType().IsNil ? metadata.GetString(type.Name) : metadata.GetString(type.Namespace) + "." + metadata.GetString(type.Name);
			output.Write(DisassemblerHelpers.Escape(typeName));
			WriteTypeParameters(output, type.GetGenericParameters());
			output.MarkFoldStart(defaultCollapsed: isInType);
			output.WriteLine();

			if (type.BaseType != null) {
				output.Indent();
				output.Write("extends ");
				type.BaseType.WriteTo(metadata, output, ILNameSyntax.TypeName);
				output.WriteLine();
				output.Unindent();
			}
			var interfaces = type.GetInterfaceImplementations();
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
					var iface = metadata.GetInterfaceImplementation(i);
					WriteAttributes(iface.GetCustomAttributes());
					iface.Interface.WriteTo(metadata, output, ILNameSyntax.TypeName);
				}
				output.WriteLine();
				output.Unindent();
			}

			output.WriteLine("{");
			output.Indent();
			bool oldIsInType = isInType;
			isInType = true;
			WriteAttributes(type.GetCustomAttributes());
			//WriteSecurityDeclarations(type);
			var layout = type.GetLayout();
			if (!layout.IsDefault) {
				output.WriteLine(".pack {0}", layout.PackingSize);
				output.WriteLine(".size {0}", layout.Size);
				output.WriteLine();
			}
			if (!type.GetNestedTypes().IsEmpty) {
				output.WriteLine("// Nested Types");
				foreach (var nestedType in type.GetNestedTypes()) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleType(nestedType);
					output.WriteLine();
				}
				output.WriteLine();
			}
			if (type.GetFields().Count > 0) {
				output.WriteLine("// Fields");
				foreach (var field in type.GetFields()) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleField(field);
				}
				output.WriteLine();
			}
			if (type.GetMethods().Count > 0) {
				output.WriteLine("// Methods");
				foreach (var m in type.GetMethods()) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleMethod(m);
					output.WriteLine();
				}
			}
			if (type.GetEvents().Count > 0) {
				output.WriteLine("// Events");
				foreach (var ev in type.GetEvents()) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleEvent(ev);
					output.WriteLine();
				}
				output.WriteLine();
			}
			if (type.GetProperties().Count > 0) {
				output.WriteLine("// Properties");
				foreach (var prop in type.GetProperties()) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleProperty(prop);
				}
				output.WriteLine();
			}
			CloseBlock("end of class " + typeName);
			isInType = oldIsInType;
		}

		void WriteTypeParameters(ITextOutput output, GenericParameterHandleCollection p)
		{
			if (p.Count > 0) {
				output.Write('<');
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
							constraint.Type.WriteTo(metadata, output, ILNameSyntax.TypeName);
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
		void WriteAttributes(CustomAttributeHandleCollection attributes)
		{
			foreach (CustomAttributeHandle a in attributes) {
				output.Write(".custom ");
				var attr = metadata.GetCustomAttribute(a);
				attr.Constructor.WriteTo(metadata, output);
				byte[] blob = metadata.GetBlobBytes(attr.Value);
				if (blob.Length > 0) {
					output.Write(" = ");
					WriteBlob(blob);
				}
				output.WriteLine();
			}
		}

		void WriteBlob(byte[] blob)
		{
			output.Write("(");
			output.Indent();

			for (int i = 0; i < blob.Length; i++) {
				if (i % 16 == 0 && i < blob.Length - 1) {
					output.WriteLine();
				} else {
					output.Write(' ');
				}
				output.Write(blob[i].ToString("x2"));
			}

			output.WriteLine();
			output.Unindent();
			output.Write(")");
		}

		void OpenBlock(bool defaultCollapsed)
		{
			output.MarkFoldStart(defaultCollapsed: defaultCollapsed);
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

		public void DisassembleNamespace(string nameSpace, IEnumerable<TypeDefinitionHandle> types)
		{
			if (!string.IsNullOrEmpty(nameSpace)) {
				output.Write(".namespace " + DisassemblerHelpers.Escape(nameSpace));
				OpenBlock(false);
			}
			bool oldIsInType = isInType;
			isInType = true;
			foreach (TypeDefinitionHandle td in types) {
				cancellationToken.ThrowIfCancellationRequested();
				DisassembleType(td);
				output.WriteLine();
			}
			if (!string.IsNullOrEmpty(nameSpace)) {
				CloseBlock();
				isInType = oldIsInType;
			}
		}

		public void WriteAssemblyHeader()
		{
			var asm = metadata.GetAssemblyDefinition();
			output.Write(".assembly ");
			if ((asm.Flags & AssemblyFlags.WindowsRuntime) == AssemblyFlags.WindowsRuntime)
				output.Write("windowsruntime ");
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(asm.Name)));
			OpenBlock(false);
			WriteAttributes(asm.GetCustomAttributes());
			//WriteSecurityDeclarations(asm);
			var publicKey = metadata.GetBlobBytes(asm.PublicKey);
			if (publicKey.Length > 0) {
				output.Write(".publickey = ");
				WriteBlob(publicKey);
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

		public void WriteAssemblyReferences()
		{
			for (int row = 0; row < metadata.GetTableRowCount(TableIndex.ModuleRef); row++) {
				var mref = metadata.GetModuleReference(MetadataTokens.ModuleReferenceHandle(row));
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
					WriteBlob(metadata.GetBlobBytes(aref.PublicKeyOrToken));
					output.WriteLine();
				}
				if (aref.Version != null) {
					output.WriteLine(".ver {0}:{1}:{2}:{3}", aref.Version.Major, aref.Version.Minor, aref.Version.Build, aref.Version.Revision);
				}
				CloseBlock();
			}
		}

		public void WriteModuleHeader()
		{
			var module = metadata.GetModuleDefinition();
			foreach (var et in metadata.ExportedTypes) {
				var exportedType = metadata.GetExportedType(et);
				output.Write(".class extern ");
				if (exportedType.IsForwarder)
					output.Write("forwarder ");
				/*exportedType.Implementation.WriteTo(metadata, output);
				output.Write(metadata.GetFullName(exportedType. != null ? exportedType.Name : exportedType.FullName);
				OpenBlock(false);
				if (exportedType.DeclaringType != null)
					output.WriteLine(".class extern {0}", DisassemblerHelpers.Escape(exportedType.DeclaringType.FullName));
				else
					output.WriteLine(".assembly extern {0}", DisassemblerHelpers.Escape(exportedType.Scope.Name));
				CloseBlock();*/
			}

			output.WriteLine(".module {0}", metadata.GetString(module.Name));
			output.WriteLine("// MVID: {0}", metadata.GetGuid(module.Mvid).ToString("B").ToUpperInvariant());
			// TODO: imagebase, file alignment, stackreserve, subsystem
			//output.WriteLine(".corflags 0x{0:x} // {1}", module., module.Attributes.ToString());

			//WriteAttributes(metadata.GetCustomAttributes(metadata.getmod));
		}

		public void WriteModuleContents(ModuleDefinition module)
		{
			foreach (var handle in metadata.TypeDefinitions) {
				DisassembleType(handle);
				output.WriteLine();
			}
		}

		unsafe TypeDefinitionHandle FindDeclaringTypeForProperty(PropertyDefinitionHandle handle)
		{
			byte* startPointer = metadata.MetadataPointer;
			int offset = metadata.GetTableMetadataOffset(TableIndex.PropertyMap);
			int rowSize = metadata.GetTableRowSize(TableIndex.PropertyMap);
			int rowCount = metadata.GetTableRowCount(TableIndex.PropertyMap);
			int token = metadata.GetToken(handle);
			for (int row = rowCount - 1; row >= 0; row--) {
				byte* ptr = startPointer + offset + rowSize * row;
				ushort parentToken = *ptr;
				ushort list = *(ptr + 2);
				if (token > list)
					return MetadataTokens.TypeDefinitionHandle(parentToken);
			}
			return default(TypeDefinitionHandle);
		}
	}

	public static class MetadataReaderExtensions
	{
		public static void WriteTo(this EntityHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			if (handle.IsNil) return;
			switch (handle.Kind) {
				case HandleKind.TypeReference:
					((TypeReferenceHandle)handle).WriteTo(metadata, output, syntax);
					break;
				case HandleKind.TypeDefinition:
					((TypeDefinitionHandle)handle).WriteTo(metadata, output);
					break;
				case HandleKind.TypeSpecification:
					((TypeSpecificationHandle)handle).WriteTo(metadata, output);
					break;
				case HandleKind.AssemblyReference:
					var ar = metadata.GetAssemblyReference((AssemblyReferenceHandle)handle);
					output.Write(metadata.GetString(ar.Name));
					break;
				case HandleKind.MethodDefinition:
					((MethodDefinitionHandle)handle).WriteTo(metadata, output);
					break;
				case HandleKind.MemberReference:
					((MemberReferenceHandle)handle).WriteTo(metadata, output);
					break;
				case HandleKind.MethodSpecification:
					((MethodSpecificationHandle)handle).WriteTo(metadata, output);
					break;
				case HandleKind.FieldDefinition:
					((FieldDefinitionHandle)handle).WriteTo(metadata, output);
					break;
				default:
					throw new NotImplementedException(handle.Kind.ToString());
			}
		}

		public static void WriteTo(this TypeSpecificationHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var ts = metadata.GetTypeSpecification(handle);
			var signature = ts.DecodeSignature(new MethodSignatureProvider(metadata, output), (default(GenericParameterHandleCollection), default(GenericParameterHandleCollection)));
			signature(syntax);
		}

		public static void WriteTo(this FieldDefinitionHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var fd = metadata.GetFieldDefinition(handle);
			var signature = fd.DecodeSignature(new MethodSignatureProvider(metadata, output), (default(GenericParameterHandleCollection), default(GenericParameterHandleCollection)));
			var name = metadata.GetString(fd.Name);
			signature(syntax);
			output.Write(' ');
			fd.GetDeclaringType().WriteTo(metadata, output);
			output.Write("::");
			output.Write(DisassemblerHelpers.Escape(name));
		}

		public static void WriteTo(this MethodSpecificationHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var ms = metadata.GetMethodSpecification(handle);
			var signature = ms.DecodeSignature(new MethodSignatureProvider(metadata, output), (default(GenericParameterHandleCollection), default(GenericParameterHandleCollection)));
			ms.Method.WriteTo(metadata, output);
		}

		public static void WriteTo(this MemberReferenceHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var mr = metadata.GetMemberReference(handle);
			switch (mr.GetKind()) {
				case MemberReferenceKind.Method:
					var signature = mr.DecodeMethodSignature(new MethodSignatureProvider(metadata, output), (default(GenericParameterHandleCollection), default(GenericParameterHandleCollection)));
					if (signature.Header.HasExplicitThis) {
						output.Write("instance explicit ");
					} else if (signature.Header.IsInstance) {
						output.Write("instance ");
					}
					if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
						output.Write("vararg ");
					}
					signature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
					output.Write(' ');
					mr.Parent.WriteTo(metadata, output, syntax);
					output.Write("::");
					output.Write(DisassemblerHelpers.Escape(metadata.GetString(mr.Name)));
					output.Write("(");
					for (int i = 0; i < signature.ParameterTypes.Length; ++i) {
						if (i > 0)
							output.Write(", ");
						signature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
					}
					output.Write(")");
					break;
				case MemberReferenceKind.Field:
					var fieldSignature = mr.DecodeFieldSignature(new MethodSignatureProvider(metadata, output), (default(GenericParameterHandleCollection), default(GenericParameterHandleCollection)));
					fieldSignature(ILNameSyntax.TypeName);
					output.Write(' ');
					mr.Parent.WriteTo(metadata, output, syntax);
					output.Write("::");
					output.Write(DisassemblerHelpers.Escape(metadata.GetString(mr.Name)));
					break;
			}
		}

		public static void WriteTo(this TypeReferenceHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var tr = metadata.GetTypeReference(handle);
			output.Write("[");
			tr.ResolutionScope.WriteTo(metadata, output);
			output.Write("]");
			string fullName = "";
			if (!tr.Namespace.IsNil)
				fullName += DisassemblerHelpers.Escape(metadata.GetString(tr.Namespace)) + ".";
			fullName += DisassemblerHelpers.Escape(metadata.GetString(tr.Name));
			output.WriteReference(fullName, tr);
		}

		public static void WriteTo(this TypeDefinitionHandle handle, MetadataReader metadata, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var td = metadata.GetTypeDefinition(handle);
			output.WriteReference(GetFullName(td, metadata), td);
		}

		static string GetFullName(TypeDefinition td, MetadataReader metadata)
		{
			var declaringType = td.GetDeclaringType();
			string fullName = "";
			if (!td.Namespace.IsNil)
				fullName += DisassemblerHelpers.Escape(metadata.GetString(td.Namespace)) + ".";
			if (!declaringType.IsNil)
				fullName += GetFullName(metadata.GetTypeDefinition(declaringType), metadata) + "/";
			fullName += DisassemblerHelpers.Escape(metadata.GetString(td.Name));
			return fullName;
		}

		public static void WriteTo(this MethodDefinitionHandle handle, MetadataReader metadata, ITextOutput output)
		{
			var method = metadata.GetMethodDefinition(handle);
			bool isCompilerControlled = (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			var declaringType = metadata.GetTypeDefinition(method.GetDeclaringType());
			var reader = metadata.GetBlobReader(method.Signature);
			var signature = method.DecodeSignature(new MethodSignatureProvider(metadata, output), (declaringType.GetGenericParameters(), method.GetGenericParameters()));
			if (signature.Header.HasExplicitThis) {
				output.Write("instance explicit ");
			} else if (signature.Header.IsInstance) {
				output.Write("instance ");
			}
			if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
				output.Write("vararg ");
			}
			signature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
			output.Write(' ');
			if (!method.GetDeclaringType().IsNil) {
				method.GetDeclaringType().WriteTo(metadata, output, ILNameSyntax.TypeName);
				output.Write("::");
			}
			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(method.Name) + "$PST" + MetadataTokens.GetToken(handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(method.Name)));
			}
			var genericParameters = method.GetGenericParameters();
			if (genericParameters.Count > 0) {
				output.Write('<');
				for (int i = 0; i < genericParameters.Count; i++) {
					if (i > 0)
						output.Write(", ");
					var gp = metadata.GetGenericParameter(genericParameters[i]);
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
							constraint.Type.WriteTo(metadata, output, ILNameSyntax.TypeName);
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
			output.Write("(");
			for (int i = 0; i < signature.ParameterTypes.Length; ++i) {
				if (i > 0)
					output.Write(", ");
				signature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
			}
			output.Write(")");
		}
	}

	class MethodSignatureProvider : ISignatureTypeProvider<Action<ILNameSyntax>, (GenericParameterHandleCollection TypeParameters, GenericParameterHandleCollection MethodTypeParameters)>
	{
		readonly MetadataReader metadata;
		readonly ITextOutput output;

		public MethodSignatureProvider(MetadataReader metadata, ITextOutput output)
		{
			this.metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
			this.output = output ?? throw new ArgumentNullException(nameof(output));
		}

		public Action<ILNameSyntax> GetArrayType(Action<ILNameSyntax> elementType, ArrayShape shape)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('[');
				for (int i = 0; i < shape.Rank; i++) {
					if (i > 0)
						output.Write(", ");
					if (i < shape.LowerBounds.Length || i < shape.Sizes.Length) {
						int lower = 0;
						if (i < shape.LowerBounds.Length) {
							lower = shape.LowerBounds[i];
							output.Write(lower.ToString());
						}
						output.Write("...");
						if (i < shape.Sizes.Length)
							output.Write((lower + shape.Sizes[i] - 1).ToString());
					}
				}
				output.Write(']');
			};
		}

		public Action<ILNameSyntax> GetByReferenceType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('&');
			};
		}

		public Action<ILNameSyntax> GetFunctionPointerType(MethodSignature<Action<ILNameSyntax>> signature)
		{
			return syntax => {
				output.Write("method ");
				signature.ReturnType(syntax);
				output.Write(" *(");
				for (int i = 0; i < signature.ParameterTypes.Length; i++) {
					if (i > 0)
						output.Write(", ");
					signature.ParameterTypes[i](syntax);
				}
				output.Write(')');
			};
		}

		public Action<ILNameSyntax> GetGenericInstantiation(Action<ILNameSyntax> genericType, ImmutableArray<Action<ILNameSyntax>> typeArguments)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				genericType(syntaxForElementTypes);
				output.Write('<');
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						output.Write(", ");
					typeArguments[i](syntaxForElementTypes);
				}
				output.Write('>');
			};
		}

		public Action<ILNameSyntax> GetGenericMethodParameter((GenericParameterHandleCollection TypeParameters, GenericParameterHandleCollection MethodTypeParameters) genericContext, int index)
		{
			return syntax => {
				output.Write("!!");
				var param = genericContext.MethodTypeParameters.Count > index ? genericContext.MethodTypeParameters[index] : MetadataTokens.GenericParameterHandle(0);
				WriteTypeParameter(param, index, syntax);
			};
		}

		public Action<ILNameSyntax> GetGenericTypeParameter((GenericParameterHandleCollection TypeParameters, GenericParameterHandleCollection MethodTypeParameters) genericContext, int index)
		{
			return syntax => {
				output.Write("!");
				var param = genericContext.TypeParameters.Count > index ? genericContext.TypeParameters[index] : MetadataTokens.GenericParameterHandle(0);
				WriteTypeParameter(param, index, syntax);
			};
		}

		void WriteTypeParameter(GenericParameterHandle paramRef, int index, ILNameSyntax syntax)
		{
			if (paramRef.IsNil || syntax == ILNameSyntax.SignatureNoNamedTypeParameters)
				output.Write(index.ToString());
			else {
				var param = metadata.GetGenericParameter(paramRef);
				if (param.Name.IsNil)
					output.Write(param.Index.ToString());
				else
					output.Write(DisassemblerHelpers.EscapeString(metadata.GetString(param.Name)));
			}
		}

		public Action<ILNameSyntax> GetModifiedType(Action<ILNameSyntax> modifier, Action<ILNameSyntax> unmodifiedType, bool isRequired)
		{
			return syntax => {
				unmodifiedType(syntax);
				if (isRequired)
					output.Write(" modreq");
				else
					output.Write(" modopt");
				output.Write('(');
				modifier(ILNameSyntax.TypeName);
				output.Write(')');
			};
		}

		public Action<ILNameSyntax> GetPinnedType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write(" pinned");
			};
		}

		public Action<ILNameSyntax> GetPointerType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('*');
			};
		}

		public Action<ILNameSyntax> GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			switch (typeCode) {
				case PrimitiveTypeCode.SByte:
					return syntax => output.Write("int8");
				case PrimitiveTypeCode.Int16:
					return syntax => output.Write("int16");
				case PrimitiveTypeCode.Int32:
					return syntax => output.Write("int32");
				case PrimitiveTypeCode.Int64:
					return syntax => output.Write("int64");
				case PrimitiveTypeCode.Byte:
					return syntax => output.Write("uint8");
				case PrimitiveTypeCode.UInt16:
					return syntax => output.Write("uint16");
				case PrimitiveTypeCode.UInt32:
					return syntax => output.Write("uint32");
				case PrimitiveTypeCode.UInt64:
					return syntax => output.Write("uint64");
				case PrimitiveTypeCode.Single:
					return syntax => output.Write("float32");
				case PrimitiveTypeCode.Double:
					return syntax => output.Write("float64");
				case PrimitiveTypeCode.Void:
					return syntax => output.Write("void");
				case PrimitiveTypeCode.Boolean:
					return syntax => output.Write("bool");
				case PrimitiveTypeCode.String:
					return syntax => output.Write("string");
				case PrimitiveTypeCode.Char:
					return syntax => output.Write("char");
				case PrimitiveTypeCode.Object:
					return syntax => output.Write("object");
				case PrimitiveTypeCode.IntPtr:
					return syntax => output.Write("native int");
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public Action<ILNameSyntax> GetSZArrayType(Action<ILNameSyntax> elementType)
		{
			return syntax => {
				var syntaxForElementTypes = syntax == ILNameSyntax.SignatureNoNamedTypeParameters ? syntax : ILNameSyntax.Signature;
				elementType(syntaxForElementTypes);
				output.Write('[');
				output.Write(']');
			};
		}

		public Action<ILNameSyntax> GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return syntax => {
				switch (rawTypeKind) {
					case 0x11:
						output.Write("valuetype ");
						break;
				}
				handle.WriteTo(reader, output, syntax);
			};
		}

		public Action<ILNameSyntax> GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return syntax => {
				switch (rawTypeKind) {
					case 0x00:
						break;
					case 0x11:
						output.Write("valuetype ");
						break;
					case 0x12:
						output.Write("class ");
						break;
					default:
						throw new NotSupportedException($"rawTypeKind: {rawTypeKind} (0x{rawTypeKind:x})");
				}
				handle.WriteTo(reader, output, syntax);
			};
		}

		public Action<ILNameSyntax> GetTypeFromSpecification(MetadataReader reader, (GenericParameterHandleCollection TypeParameters, GenericParameterHandleCollection MethodTypeParameters) genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}
	}
}