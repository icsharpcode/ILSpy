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

using ICSharpCode.Decompiler.Dom;
using ICSharpCode.Decompiler.IL;

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
		};

		public void DisassembleMethod(Dom.MethodDefinition method)
		{
			DisassembleMethodHeader(method);
			DisassembleMethodBlock(method);
		}

		public void DisassembleMethodHeader(Dom.MethodDefinition method)
		{
			// write method header
			output.WriteDefinition(".method ", method);
			DisassembleMethodHeaderInternal(method);
		}

		void DisassembleMethodHeaderInternal(Dom.MethodDefinition method)
		{
			var metadata = method.Module.GetMetadataReader();
			//    .method public hidebysig  specialname
			//               instance default class [mscorlib]System.IO.TextWriter get_BaseWriter ()  cil managed
			//
			//emit flags
			WriteEnum(method.Attributes & MethodAttributes.MemberAccessMask, methodVisibility);
			WriteFlags(method.Attributes & ~MethodAttributes.MemberAccessMask, methodAttributeFlags);
			bool isCompilerControlled = (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			if (isCompilerControlled)
				output.Write("privatescope ");

			if ((method.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl) {
				output.Write("pinvokeimpl");
				var info = method.Import;
				if (!info.Module.IsNil) {
					var moduleRef = metadata.GetModuleReference(info.Module);
					output.Write("(\"" + DisassemblerHelpers.EscapeString(metadata.GetString(moduleRef.Name)) + "\"");

					if (!info.Name.IsNil && metadata.GetString(info.Name) != method.Name)
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
			var declaringType = method.DeclaringType;
			var signature = method.DecodeSignature(new DisassemblerSignatureProvider(method.Module, output), new GenericContext(method));
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

			var parameters = method.Parameters.ToImmutableArray();
			if (parameters.Length > 0) {
				var marshallingDesc = metadata.GetParameter(parameters[0]).GetMarshallingDescriptor();

				if (!marshallingDesc.IsNil) {
					WriteMarshalInfo(metadata.GetBlobReader(marshallingDesc));
				}
			}

			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(method.Name + "$PST" + MetadataTokens.GetToken(method.Handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(method.Name));
			}

			WriteTypeParameters(output, method.Module, new GenericContext(method), method.GenericParameters);

			//( params )
			output.Write(" (");
			if (signature.ParameterTypes.Length > 0) {
				output.WriteLine();
				output.Indent();
				WriteParameters(metadata, parameters, signature);
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

		void DisassembleMethodBlock(Dom.MethodDefinition method)
		{
			OpenBlock(defaultCollapsed: isInType);
			WriteAttributes(method);

			var metadata = method.Module.GetMetadataReader();

			foreach (var h in method.GetMethodImplementations()) {
				var impl = metadata.GetMethodImplementation(h);
				output.Write(".override method ");
				var memberRef = impl.MethodDeclaration.CoerceMemberReference(method.Module);
				memberRef.WriteTo(output, new GenericContext(method));
				output.WriteLine();
			}

			foreach (var p in method.Parameters) {
				WriteParameterAttributes(method.Module, p);
			}
			WriteSecurityDeclarations(method.Module, method.DeclarativeSecurityAttributes);

			if (method.HasBody) {
				methodBodyDisassembler.Disassemble(method);
			}
			CloseBlock("end of method " + DisassemblerHelpers.Escape(method.DeclaringType.Name) + "::" + DisassemblerHelpers.Escape(method.Name));
		}

		#region Write Security Declarations
		void WriteSecurityDeclarations(PEFile module, DeclarativeSecurityAttributeHandleCollection secDeclProvider)
		{
			if (secDeclProvider.Count == 0)
				return;
			var metadata = module.GetMetadataReader();
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
				var blob = metadata.GetBlobReader(secdecl.PermissionSet);
				if ((char)blob.ReadByte() != '.') throw new InvalidOperationException("sanity check!");
				int count = blob.ReadCompressedInteger();
				for (int i = 0; i < count; i++) {
					var typeName = blob.ReadSerializedString();
					if (secdecl.Parent == EntityHandle.AssemblyDefinition) {
						output.Write("class ");
						output.Write(DisassemblerHelpers.Escape(typeName));
					} else {
						throw new NotImplementedException();
						//sa.AttributeType.WriteTo(output, ILNameSyntax.TypeName);
					}
					output.Write(" = {");
					blob.ReadByte();
					int argCount = blob.ReadByte();
					if (argCount > 0) {
						output.WriteLine();
						output.Indent();

						for (int j = 0; j < argCount; j++) {
							WriteSecurityDeclarationArgument(ref blob);
							output.WriteLine();
						}

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
		}
		
		void WriteSecurityDeclarationArgument(ref BlobReader blob)
		{
			var decoder = new BlobDecoder(blob);
			var namedArg = decoder.ReadNamedArg();
			switch (blob.ReadByte()) {
				case 0x53:
					output.Write("field ");
					break;
				case 0x54:
					output.Write("property ");
					break;
			}
			blob.Offset = decoder.Offset;
			var value = namedArg.Value.ConstantValue;
			string typeName = DisassemblerHelpers.PrimitiveTypeName(value.GetType().FullName);
			if (typeName != null) {
				output.Write(typeName);
			} else {
				namedArg.Value.Type.WriteTo(output);
			}
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(namedArg.Key));
			output.Write(" = ");
			if (value is string) {
				// secdecls use special syntax for strings
				output.Write("string('{0}')", DisassemblerHelpers.EscapeString((string)value).Replace("'", "\'"));
			} else {
				if (typeName != null) {
					output.Write(typeName);
				} else {
					namedArg.Value.Type.WriteTo(output);
				}
				output.Write('(');
				DisassemblerHelpers.WriteOperand(output, value);
				output.Write(')');
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

		void WriteParameters(MetadataReader metadata, ImmutableArray<ParameterHandle> parameters, MethodSignature<Action<ILNameSyntax>> signature)
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

		void WriteParameterAttributes(PEFile module, ParameterHandle handle)
		{
			var metadata = module.GetMetadataReader();
			var p = metadata.GetParameter(handle);
			if (p.GetDefaultValue().IsNil && p.GetCustomAttributes().Count == 0)
				return;
			output.Write(".param [{0}]", p.SequenceNumber);
			if (!p.GetDefaultValue().IsNil) {
				output.Write(" = ");
				WriteConstant(metadata, metadata.GetConstant(p.GetDefaultValue()));
			}
			output.WriteLine();
			WriteAttributes(module, p.GetCustomAttributes());
		}

		void WriteConstant(MetadataReader metadata, Constant constant)
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

		public void DisassembleField(Dom.FieldDefinition field)
		{
			var metadata = field.Module.GetMetadataReader();
			output.WriteDefinition(".field ", field);
			int offset = field.Offset;
			if (offset > -1) {
				output.Write("[" + offset + "] ");
			}
			WriteEnum(field.Attributes & FieldAttributes.FieldAccessMask, fieldVisibility);
			const FieldAttributes hasXAttributes = FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal | FieldAttributes.HasFieldRVA;
			WriteFlags(field.Attributes & ~(FieldAttributes.FieldAccessMask | hasXAttributes), fieldAttributes);

			var signature = field.DecodeSignature(new DisassemblerSignatureProvider(field.Module, output), new GenericContext(field.DeclaringType));

			if (!field.GetMarshallingDescriptor().IsNil) {
				WriteMarshalInfo(metadata.GetBlobReader(field.GetMarshallingDescriptor()));
			}
			signature(ILNameSyntax.Signature);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(field.Name));
			if (field.HasFlag(FieldAttributes.HasFieldRVA)) {
				output.Write(" at I_{0:x8}", field.RVA);
			}
			if (!field.GetDefaultValue().IsNil) {
				output.Write(" = ");
				WriteConstant(metadata, metadata.GetConstant(field.GetDefaultValue()));
			}
			output.WriteLine();
			if (field.CustomAttributes.Count > 0) {
				output.MarkFoldStart();
				WriteAttributes(field);
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

		public void DisassembleProperty(Dom.PropertyDefinition property)
		{
			var metadata = property.Module.GetMetadataReader();
			output.WriteDefinition(".property ", property);
			WriteFlags(property.Attributes, propertyAttributes);

			var declaringType = property.DeclaringType;
			var signature = property.DecodeSignature(new DisassemblerSignatureProvider(property.Module, output), new GenericContext(declaringType));

			if (signature.Header.IsInstance)
				output.Write("instance ");
			signature.ReturnType(ILNameSyntax.Signature);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(property.Name));

			output.Write("(");
			var parameters = property.GetAccessors().First().Method.Parameters;
			if (parameters.Count > 0) {
				output.WriteLine();
				output.Indent();
				WriteParameters(metadata, parameters.ToImmutableArray(), signature);
				output.Unindent();
			}
			output.Write(")");

			OpenBlock(false);
			WriteAttributes(property);
			WriteNestedMethod(".get", property.GetMethod);
			WriteNestedMethod(".set", property.SetMethod);
			CloseBlock();
		}

		void WriteNestedMethod(string keyword, Dom.MethodDefinition method)
		{
			if (method.IsNil)
				return;

			output.Write(keyword);
			output.Write(' ');
			method.WriteTo(output);
			output.WriteLine();
		}
		#endregion

		#region Disassemble Event
		EnumNameCollection<EventAttributes> eventAttributes = new EnumNameCollection<EventAttributes>() {
			{ EventAttributes.SpecialName, "specialname" },
			{ EventAttributes.RTSpecialName, "rtspecialname" },
		};

		public void DisassembleEvent(Dom.EventDefinition ev)
		{
			var metadata = ev.Module.GetMetadataReader();
			output.WriteDefinition(".event ", ev);
			WriteFlags(ev.Attributes, eventAttributes);
			var signature = ev.DecodeSignature(new DisassemblerSignatureProvider(ev.Module, output), new GenericContext(ev.DeclaringType));
			signature(ILNameSyntax.TypeName);
			output.Write(' ');
			output.Write(DisassemblerHelpers.Escape(ev.Name));
			OpenBlock(false);
			WriteAttributes(ev);
			WriteNestedMethod(".addon", ev.AddMethod);
			WriteNestedMethod(".removeon", ev.RemoveMethod);
			WriteNestedMethod(".fire", ev.InvokeMethod);
			foreach (var method in ev.OtherMethods) {
				WriteNestedMethod(".other", method);
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

		public void DisassembleType(Dom.TypeDefinition type)
		{
			var metadata = type.Module.GetMetadataReader();
			output.WriteDefinition(".class ", type);

			if ((type.Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface)
				output.Write("interface ");
			WriteEnum(type.Attributes & TypeAttributes.VisibilityMask, typeVisibility);
			WriteEnum(type.Attributes & TypeAttributes.LayoutMask, typeLayout);
			WriteEnum(type.Attributes & TypeAttributes.StringFormatMask, typeStringFormat);
			const TypeAttributes masks = TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.StringFormatMask;
			WriteFlags(type.Attributes & ~masks, typeAttributes);

			output.Write(type.DeclaringType.IsNil ? type.FullName.ToILNameString() : DisassemblerHelpers.Escape(type.Name));
			WriteTypeParameters(output, type.Module, new GenericContext(type), type.GenericParameters);
			output.MarkFoldStart(defaultCollapsed: isInType);
			output.WriteLine();

			if (type.BaseType != null) {
				output.Indent();
				output.Write("extends ");
				type.BaseType.WriteTo(output, new GenericContext(type), ILNameSyntax.TypeName);
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
					WriteAttributes(type.Module, iface.GetCustomAttributes());
					var ifaceRef = iface.Interface.CoerceTypeReference(type.Module);
					ifaceRef.WriteTo(output, new GenericContext(type), ILNameSyntax.TypeName);
				}
				output.WriteLine();
				output.Unindent();
			}

			output.WriteLine("{");
			output.Indent();
			bool oldIsInType = isInType;
			isInType = true;
			WriteAttributes(type);
			WriteSecurityDeclarations(type.Module, type.DeclarativeSecurityAttributes);
			var layout = type.GetLayout();
			if (!layout.IsDefault) {
				output.WriteLine(".pack {0}", layout.PackingSize);
				output.WriteLine(".size {0}", layout.Size);
				output.WriteLine();
			}
			if (!type.NestedTypes.IsEmpty) {
				output.WriteLine("// Nested Types");
				foreach (var nestedType in type.NestedTypes) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleType(nestedType);
					output.WriteLine();
				}
				output.WriteLine();
			}
			if (!type.Fields.IsEmpty) {
				output.WriteLine("// Fields");
				foreach (var field in type.Fields) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleField(field);
				}
				output.WriteLine();
			}
			if (!type.Methods.IsEmpty) {
				output.WriteLine("// Methods");
				foreach (var m in type.Methods) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleMethod(m);
					output.WriteLine();
				}
			}
			if (!type.Events.IsEmpty) {
				output.WriteLine("// Events");
				foreach (var ev in type.Events) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleEvent(ev);
					output.WriteLine();
				}
				output.WriteLine();
			}
			if (!type.Properties.IsEmpty) {
				output.WriteLine("// Properties");
				foreach (var prop in type.Properties) {
					cancellationToken.ThrowIfCancellationRequested();
					DisassembleProperty(prop);
				}
				output.WriteLine();
			}
			CloseBlock("end of class " + (!type.DeclaringType.IsNil ? type.Name : type.FullName.ToString()));
			isInType = oldIsInType;
		}

		void WriteTypeParameters(ITextOutput output, PEFile module, GenericContext context, GenericParameterHandleCollection p)
		{
			if (p.Count > 0) {
				output.Write('<');
				var metadata = module.GetMetadataReader();
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
							constraint.Type.CoerceTypeReference(module).WriteTo(output, context, ILNameSyntax.TypeName);
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
		void WriteAttributes(Dom.ICustomAttributeProvider attributes)
		{
			WriteAttributes(attributes.Module, attributes.CustomAttributes);
		}

		void WriteAttributes(PEFile module, CustomAttributeHandleCollection attributes)
		{
			var metadata = module.GetMetadataReader();
			foreach (CustomAttributeHandle a in attributes) {
				output.Write(".custom ");
				var attr = metadata.GetCustomAttribute(a);
				attr.Constructor.CoerceMemberReference(module).WriteTo(output, GenericContext.Empty);
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

		public void DisassembleNamespace(string nameSpace, IEnumerable<Dom.TypeDefinition> types)
		{
			if (!string.IsNullOrEmpty(nameSpace)) {
				output.Write(".namespace " + DisassemblerHelpers.Escape(nameSpace));
				OpenBlock(false);
			}
			bool oldIsInType = isInType;
			isInType = true;
			foreach (var td in types) {
				cancellationToken.ThrowIfCancellationRequested();
				DisassembleType(td);
				output.WriteLine();
			}
			if (!string.IsNullOrEmpty(nameSpace)) {
				CloseBlock();
				isInType = oldIsInType;
			}
		}

		public void WriteAssemblyHeader(PEFile module)
		{
			var metadata = module.GetMetadataReader();
			if (!metadata.IsAssembly) return;
			output.Write(".assembly ");
			var asm = metadata.GetAssemblyDefinition();
			if ((asm.Flags & AssemblyFlags.WindowsRuntime) == AssemblyFlags.WindowsRuntime)
				output.Write("windowsruntime ");
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(asm.Name)));
			OpenBlock(false);
			WriteAttributes(module, asm.GetCustomAttributes());
			WriteSecurityDeclarations(module, asm.GetDeclarativeSecurityAttributes());
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

		public void WriteAssemblyReferences(PEFile module)
		{
			var metadata = module.GetMetadataReader();
			foreach (var m in module.ModuleReferences) {
				var mref = metadata.GetModuleReference(m);
				output.WriteLine(".module extern {0}", DisassemblerHelpers.Escape(metadata.GetString(mref.Name)));
			}
			foreach (var a in module.AssemblyReferences) {
				var aref = metadata.GetAssemblyReference(a.Handle);
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

		public void WriteModuleHeader(PEFile module)
		{
			var metadata = module.GetMetadataReader();
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
			var moduleDefinition = metadata.GetModuleDefinition();

			output.WriteLine(".module {0}", metadata.GetString(moduleDefinition.Name));
			output.WriteLine("// MVID: {0}", metadata.GetGuid(moduleDefinition.Mvid).ToString("B").ToUpperInvariant());
			// TODO: imagebase, file alignment, stackreserve, subsystem
			//output.WriteLine(".corflags 0x{0:x} // {1}", module., module.Attributes.ToString());

			WriteAttributes(module, metadata.GetCustomAttributes(EntityHandle.ModuleDefinition));
		}

		public void WriteModuleContents(PEFile module)
		{
			foreach (var handle in module.TypeDefinitions) {
				DisassembleType(handle);
				output.WriteLine();
			}
		}
	}

	class DisassemblerSignatureProvider : ISignatureTypeProvider<Action<ILNameSyntax>, GenericContext>
	{
		readonly PEFile module;
		readonly MetadataReader metadata;
		readonly ITextOutput output;

		public DisassemblerSignatureProvider(PEFile module, ITextOutput output)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.output = output ?? throw new ArgumentNullException(nameof(output));
			this.metadata = module.GetMetadataReader();
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

		public Action<ILNameSyntax> GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return syntax => {
				output.Write("!!");
				WriteTypeParameter(genericContext.GetGenericMethodTypeParameterHandleOrNull(index), index, syntax);
			};
		}

		public Action<ILNameSyntax> GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return syntax => {
				output.Write("!");
				WriteTypeParameter(genericContext.GetGenericTypeParameterHandleOrNull(index), index, syntax);
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
				var td = new Dom.TypeDefinition(module, handle);
				td.WriteTo(output);
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
				var typeRef = new Dom.TypeReference(module, handle);
				typeRef.WriteTo(output);
			};
		}

		public Action<ILNameSyntax> GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}
}