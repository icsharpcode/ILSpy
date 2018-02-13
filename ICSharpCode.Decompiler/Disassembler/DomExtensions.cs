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
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Dom;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class DomExtensions
	{
		public static void WriteTo(this Dom.FieldDefinition field, ITextOutput output)
		{
			var signature = field.DecodeSignature(new DisassemblerSignatureProvider(field.Module, output), new GenericContext(field.DeclaringType));
			signature(ILNameSyntax.SignatureNoNamedTypeParameters);
			output.Write(' ');
			field.DeclaringType.WriteTo(output, ILNameSyntax.TypeName);
			output.Write("::");
			output.Write(DisassemblerHelpers.Escape(field.Name));
		}

		public static void WriteTo(this IMemberReference member, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			void WriteParent(EntityHandle parentHandle)
			{
				var metadata = member.Module.GetMetadataReader();
				switch (parentHandle.Kind) {
					case HandleKind.MethodDefinition:
						new Dom.MethodDefinition(member.Module, (MethodDefinitionHandle)parentHandle).WriteTo(output, genericContext, syntax);
						break;
					case HandleKind.ModuleReference:
						output.Write('[');
						var moduleRef = metadata.GetModuleReference((ModuleReferenceHandle)parentHandle);
						output.Write(metadata.GetString(moduleRef.Name));
						output.Write(']');
						break;
					case HandleKind.TypeDefinition:
						new Dom.TypeDefinition(member.Module, (TypeDefinitionHandle)parentHandle).WriteTo(output, syntax);
						break;
					case HandleKind.TypeReference:
						new Dom.TypeReference(member.Module, (TypeReferenceHandle)parentHandle).WriteTo(output, syntax);
						break;
					case HandleKind.TypeSpecification:
						new Dom.TypeSpecification(member.Module, (TypeSpecificationHandle)parentHandle).WriteTo(output, genericContext, syntax);
						break;
				}
			}

			MethodSignature<Action<ILNameSyntax>> signature;
			switch (member) {
				case Dom.MethodDefinition md:
					md.WriteTo(output);
					break;
				case Dom.MemberReference mr:
					switch (mr.Kind) {
						case MemberReferenceKind.Method:
							signature = mr.DecodeMethodSignature(new DisassemblerSignatureProvider(member.Module, output), genericContext);
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
							WriteParent(mr.Parent);
							output.Write("::");
							output.Write(DisassemblerHelpers.Escape(mr.Name));
							output.Write("(");
							for (int i = 0; i < signature.ParameterTypes.Length; ++i) {
								if (i > 0)
									output.Write(", ");
								signature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
							}
							output.Write(")");
							break;
						case MemberReferenceKind.Field:
							var fieldSignature = mr.DecodeFieldSignature(new DisassemblerSignatureProvider(member.Module, output), genericContext);
							fieldSignature(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write(' ');
							WriteParent(mr.Parent);
							output.Write("::");
							output.Write(DisassemblerHelpers.Escape(mr.Name));
							break;
					}
					break;
				case Dom.MethodSpecification ms:
					var substitution = ms.DecodeSignature(new DisassemblerSignatureProvider(ms.Module, output), genericContext);
					switch (ms.Method) {
						case Dom.MethodDefinition method:
							var metadata = method.Module.GetMetadataReader();
							signature = method.DecodeSignature(new DisassemblerSignatureProvider(method.Module, output), new GenericContext(method));
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
							if (!method.DeclaringType.IsNil) {
								method.DeclaringType.WriteTo(output, ILNameSyntax.TypeName);
								output.Write("::");
							}
							bool isCompilerControlled = (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
							if (isCompilerControlled) {
								output.Write(DisassemblerHelpers.Escape(method.Name + "$PST" + MetadataTokens.GetToken(method.Handle).ToString("X8")));
							} else {
								output.Write(DisassemblerHelpers.Escape(method.Name));
							}
							output.Write('<');
							for (int i = 0; i < substitution.Length; i++) {
								if (i > 0)
									output.Write(", ");
								substitution[i](syntax);
							}
							output.Write('>');
							output.Write("(");
							for (int i = 0; i < signature.ParameterTypes.Length; ++i) {
								if (i > 0)
									output.Write(", ");
								signature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
							}
							output.Write(")");
							break;
						case Dom.MemberReference mr:
							signature = mr.DecodeMethodSignature(new DisassemblerSignatureProvider(member.Module, output), genericContext);
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
							WriteParent(mr.Parent);
							output.Write("::");
							output.Write(DisassemblerHelpers.Escape(mr.Name));
							output.Write('<');
							for (int i = 0; i < substitution.Length; i++) {
								if (i > 0)
									output.Write(", ");
								substitution[i](syntax);
							}
							output.Write('>');
							output.Write("(");
							for (int i = 0; i < signature.ParameterTypes.Length; ++i) {
								if (i > 0)
									output.Write(", ");
								signature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
							}
							output.Write(")");
							break;
						}
					break;
			}
		}

		public static void WriteTo(this Dom.TypeReference typeRef, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var metadata = typeRef.Module.GetMetadataReader();
			if (!typeRef.ResolutionScope.IsNil) {
				output.Write("[");
				var currentTypeRef = typeRef;
				while (currentTypeRef.ResolutionScope.Kind == HandleKind.TypeReference) {
					currentTypeRef = new Dom.TypeReference(currentTypeRef.Module, (TypeReferenceHandle)currentTypeRef.ResolutionScope);
				}
				switch (currentTypeRef.ResolutionScope.Kind) {
					case HandleKind.ModuleReference:
						break;
					case HandleKind.AssemblyReference:
						var asmRef = metadata.GetAssemblyReference((AssemblyReferenceHandle)currentTypeRef.ResolutionScope);
						output.Write(DisassemblerHelpers.Escape(metadata.GetString(asmRef.Name)));
						break;
				}
				output.Write("]");
			}
			output.WriteReference(typeRef.FullName.ToILNameString(), typeRef);
		}

		public static void WriteTo(this Dom.TypeDefinition typeDefinition, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			output.WriteReference(typeDefinition.FullName.ToILNameString(), typeDefinition);
		}

		public static void WriteTo(this Dom.TypeSpecification typeSpecification, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var signature = typeSpecification.DecodeSignature(new DisassemblerSignatureProvider(typeSpecification.Module, output), genericContext);
			signature(syntax);
		}

		public static void WriteTo(this ITypeReference type, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			switch (type) {
				case Dom.TypeDefinition td:
					td.WriteTo(output, syntax);
					break;
				case Dom.TypeReference tr:
					tr.WriteTo(output, syntax);
					break;
				case Dom.TypeSpecification ts:
					ts.WriteTo(output, genericContext, syntax);
					break;
			}
		}

		public static void WriteTo(this Dom.MethodDefinition method, ITextOutput output)
		{
			var metadata = method.Module.GetMetadataReader();
			var signature = method.DecodeSignature(new DisassemblerSignatureProvider(method.Module, output), new GenericContext(method));
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
			if (!method.DeclaringType.IsNil) {
				method.DeclaringType.WriteTo(output, ILNameSyntax.TypeName);
				output.Write("::");
			}
			bool isCompilerControlled = (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(method.Name + "$PST" + MetadataTokens.GetToken(method.Handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(method.Name));
			}
			var genericParameters = method.GenericParameters;
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
							constraint.Type.CoerceTypeReference(method.Module).WriteTo(output, new GenericContext(method), ILNameSyntax.TypeName);
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

		public static bool IsBaseTypeOf(this ITypeReference baseType, ITypeReference derivedType)
		{
			var derivedTypeDefinition = derivedType.GetDefinition();
			while (!derivedTypeDefinition.IsNil) {
				if (derivedTypeDefinition.FullName == baseType.FullName)
					return true;
				derivedTypeDefinition = derivedTypeDefinition.BaseType.GetDefinition();
			}
			return false;
		}
	}

	public class TypeUsedInSignature : ISignatureTypeProvider<bool, Unit>
	{
		readonly PEFile module;
		readonly Dom.TypeDefinition type;

		public TypeUsedInSignature(ITypeReference type)
		{
			this.module = type.Module;
			this.type = type.GetDefinition();
		}

		public bool GetArrayType(bool elementType, ArrayShape shape) => elementType;

		public bool GetByReferenceType(bool elementType) => elementType;

		public bool GetFunctionPointerType(MethodSignature<bool> signature)
		{
			throw new NotImplementedException();
		}

		public bool GetGenericInstantiation(bool genericType, ImmutableArray<bool> typeArguments)
		{
			throw new NotImplementedException();
		}

		public bool GetGenericMethodParameter(Unit genericContext, int index) => false;

		public bool GetGenericTypeParameter(Unit genericContext, int index) => false;

		public bool GetModifiedType(bool modifier, bool unmodifiedType, bool isRequired)
		{
			throw new NotImplementedException();
		}

		public bool GetPinnedType(bool elementType) => elementType;

		public bool GetPointerType(bool elementType) => elementType;

		public bool GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			throw new NotImplementedException();
		}

		public bool GetSZArrayType(bool elementType) => elementType;

		public bool GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return module == type.Module && handle == type.Handle;
		}

		public bool GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return new Dom.TypeReference(module, handle).GetDefinition() == type;
		}

		public bool GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return new Dom.TypeSpecification(module, handle).GetDefinition() == type;
		}
	}
}