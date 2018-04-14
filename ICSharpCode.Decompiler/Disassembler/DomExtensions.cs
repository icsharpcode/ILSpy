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
using SRM = System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Disassembler
{
	public static class MetadataExtensions
	{
		public static void WriteTo(this Metadata.TypeDefinition typeDefinition, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var metadata = typeDefinition.Module.GetMetadataReader();
			output.WriteReference(typeDefinition.This().GetFullTypeName(metadata).ToILNameString(), typeDefinition);
		}

		public static void WriteTo(this Metadata.FieldDefinition field, ITextOutput output)
		{
			var metadata = field.Module.GetMetadataReader();
			var fieldDefinition = metadata.GetFieldDefinition(field.Handle);
			var declaringType = new Metadata.TypeDefinition(field.Module, fieldDefinition.GetDeclaringType());
			var signature = fieldDefinition.DecodeSignature(new DisassemblerSignatureProvider(field.Module, output), new GenericContext(declaringType));
			signature(ILNameSyntax.SignatureNoNamedTypeParameters);
			output.Write(' ');
			declaringType.WriteTo(output, ILNameSyntax.TypeName);
			output.Write("::");
			output.Write(DisassemblerHelpers.Escape(metadata.GetString(fieldDefinition.Name)));
		}

		public static void WriteTo(this Metadata.MethodDefinition method, ITextOutput output)
		{
			var metadata = method.Module.GetMetadataReader();
			var methodDefinition = method.This();
			var signature = methodDefinition.DecodeSignature(new DisassemblerSignatureProvider(method.Module, output), new GenericContext(method));
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
			var declaringType = methodDefinition.GetDeclaringType();
			if (!declaringType.IsNil) {
				new Metadata.TypeDefinition(method.Module, declaringType).WriteTo(output, ILNameSyntax.TypeName);
				output.Write("::");
			}
			bool isCompilerControlled = (methodDefinition.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
			if (isCompilerControlled) {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(methodDefinition.Name) + "$PST" + MetadataTokens.GetToken(method.Handle).ToString("X8")));
			} else {
				output.Write(DisassemblerHelpers.Escape(metadata.GetString(methodDefinition.Name)));
			}
			var genericParameters = methodDefinition.GetGenericParameters();
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
							constraint.Type.WriteTo(method.Module, output, new GenericContext(method), ILNameSyntax.TypeName);
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

		public static void WriteTo(this EntityHandle entity, PEFile module, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			if (entity.IsNil)
				throw new ArgumentNullException(nameof(entity));
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			WriteTo(new Entity(module, entity), output, genericContext, syntax);
		}

		public static void WriteTo(this Entity entity, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			if (entity.IsNil)
				throw new ArgumentNullException(nameof(entity));
			var metadata = entity.Module.GetMetadataReader();

			switch (entity.Handle.Kind) {
				case HandleKind.TypeDefinition:
					WriteTo(entity, output, syntax);
					break;
				case HandleKind.TypeReference:
					WriteTo(new Metadata.TypeReference(entity.Module, (TypeReferenceHandle)entity.Handle), output, syntax);
					break;
				case HandleKind.TypeSpecification:
					WriteTo(new Metadata.TypeSpecification(entity.Module, (TypeSpecificationHandle)entity.Handle), output, genericContext, syntax);
					break;
				case HandleKind.FieldDefinition:
					WriteTo((Metadata.FieldDefinition)entity, output);
					break;
				case HandleKind.MethodDefinition:
					WriteTo((Metadata.MethodDefinition)entity, output);
					break;
				case HandleKind.MemberReference:
					WriteTo((MemberReferenceHandle)entity.Handle, entity.Module, output, genericContext, syntax);
					break;
				case HandleKind.MethodSpecification:
					WriteTo((MethodSpecificationHandle)entity.Handle, entity.Module, output, genericContext, syntax);
					break;
				case HandleKind.PropertyDefinition:
				case HandleKind.EventDefinition:
					throw new NotSupportedException();
				case HandleKind.StandaloneSignature:
					var standaloneSig = metadata.GetStandaloneSignature((StandaloneSignatureHandle)entity.Handle);
					switch (standaloneSig.GetKind()) {
						case StandaloneSignatureKind.Method:
							var methodSig = standaloneSig.DecodeMethodSignature(new DisassemblerSignatureProvider(entity.Module, output), genericContext);
							methodSig.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write('(');
							for (int i = 0; i < methodSig.ParameterTypes.Length; i++) {
								if (i > 0)
									output.Write(", ");
								methodSig.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
							}
							output.Write(')');
							break;
						case StandaloneSignatureKind.LocalVariables:
						default:
							throw new NotSupportedException();
					}
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public static void WriteTo(this MethodSpecificationHandle handle, PEFile module, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax)
		{
			var metadata = module.GetMetadataReader();
			var ms = metadata.GetMethodSpecification(handle);
			var substitution = ms.DecodeSignature(new DisassemblerSignatureProvider(module, output), genericContext);
			MethodSignature<Action<ILNameSyntax>> signature;
			switch (ms.Method.Kind) {
				case HandleKind.MethodDefinition:
					var methodDefinition = metadata.GetMethodDefinition((MethodDefinitionHandle)ms.Method);
					var methodName = metadata.GetString(methodDefinition.Name);
					signature = methodDefinition.DecodeSignature(new DisassemblerSignatureProvider(module, output), genericContext);
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
					var declaringType = methodDefinition.GetDeclaringType();
					if (!declaringType.IsNil) {
						new Metadata.TypeDefinition(module, declaringType).WriteTo(output, ILNameSyntax.TypeName);
						output.Write("::");
					}
					bool isCompilerControlled = (methodDefinition.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
					if (isCompilerControlled) {
						output.Write(DisassemblerHelpers.Escape(methodName + "$PST" + MetadataTokens.GetToken(ms.Method).ToString("X8")));
					} else {
						output.Write(DisassemblerHelpers.Escape(methodName));
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
				case HandleKind.MemberReference:
					var memberReference = metadata.GetMemberReference((MemberReferenceHandle)ms.Method);
					var memberName = metadata.GetString(memberReference.Name);
					signature = memberReference.DecodeMethodSignature(new DisassemblerSignatureProvider(module, output), genericContext);
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
					WriteParent(output, module, metadata, memberReference.Parent, genericContext, syntax);
					output.Write("::");
					output.Write(DisassemblerHelpers.Escape(memberName));
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
		}

		public static void WriteTo(this MemberReferenceHandle handle, PEFile module, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var metadata = module.GetMetadataReader();
			var mr = metadata.GetMemberReference(handle);
			var memberName = metadata.GetString(mr.Name);
			switch (mr.GetKind()) {
				case MemberReferenceKind.Method:
					var methodSignature = mr.DecodeMethodSignature(new DisassemblerSignatureProvider(module, output), genericContext);
					if (methodSignature.Header.HasExplicitThis) {
						output.Write("instance explicit ");
					} else if (methodSignature.Header.IsInstance) {
						output.Write("instance ");
					}
					if (methodSignature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
						output.Write("vararg ");
					}
					methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
					output.Write(' ');
					WriteParent(output, module, metadata, mr.Parent, genericContext, syntax);
					output.Write("::");
					output.Write(DisassemblerHelpers.Escape(memberName));
					output.Write("(");
					for (int i = 0; i < methodSignature.ParameterTypes.Length; ++i) {
						if (i > 0)
							output.Write(", ");
						if (i == methodSignature.RequiredParameterCount)
							output.Write("..., ");
						methodSignature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
					}
					output.Write(")");
					break;
				case MemberReferenceKind.Field:
					var fieldSignature = mr.DecodeFieldSignature(new DisassemblerSignatureProvider(module, output), genericContext);
					fieldSignature(ILNameSyntax.SignatureNoNamedTypeParameters);
					output.Write(' ');
					WriteParent(output, module, metadata, mr.Parent, genericContext, syntax);
					output.Write("::");
					output.Write(DisassemblerHelpers.Escape(memberName));
					break;
			}
		}

		public static void WriteTo(this Metadata.TypeReference typeRef, ITextOutput output, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var metadata = typeRef.Module.GetMetadataReader();
			if (!typeRef.ResolutionScope.IsNil) {
				output.Write("[");
				var currentTypeRef = typeRef;
				while (currentTypeRef.ResolutionScope.Kind == HandleKind.TypeReference) {
					currentTypeRef = new Metadata.TypeReference(currentTypeRef.Module, (TypeReferenceHandle)currentTypeRef.ResolutionScope);
				}
				switch (currentTypeRef.ResolutionScope.Kind) {
					case HandleKind.ModuleDefinition:
						var modDef = metadata.GetModuleDefinition();
						output.Write(DisassemblerHelpers.Escape(metadata.GetString(modDef.Name)));
						break;
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

		public static void WriteTo(this Metadata.TypeSpecification typeSpecification, ITextOutput output, GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			var signature = typeSpecification.DecodeSignature(new DisassemblerSignatureProvider(typeSpecification.Module, output), genericContext);
			signature(syntax);
		}

		static void WriteParent(ITextOutput output, PEFile module, MetadataReader metadata, EntityHandle parentHandle, GenericContext genericContext, ILNameSyntax syntax)
		{
			switch (parentHandle.Kind) {
				case HandleKind.MethodDefinition:
					var methodDef = metadata.GetMethodDefinition((MethodDefinitionHandle)parentHandle);
					new Metadata.TypeDefinition(module, methodDef.GetDeclaringType()).WriteTo(output, syntax);
					break;
				case HandleKind.ModuleReference:
					output.Write('[');
					var moduleRef = metadata.GetModuleReference((ModuleReferenceHandle)parentHandle);
					output.Write(metadata.GetString(moduleRef.Name));
					output.Write(']');
					break;
				case HandleKind.TypeDefinition:
					new Metadata.TypeDefinition(module, (TypeDefinitionHandle)parentHandle).WriteTo(output, syntax);
					break;
				case HandleKind.TypeReference:
					new Metadata.TypeReference(module, (TypeReferenceHandle)parentHandle).WriteTo(output, syntax);
					break;
				case HandleKind.TypeSpecification:
					new Metadata.TypeSpecification(module, (TypeSpecificationHandle)parentHandle).WriteTo(output, genericContext, syntax);
					break;
			}
		}
/*
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
		readonly Metadata.TypeDefinition type;

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
			return new Metadata.TypeReference(module, handle).GetDefinition() == type;
		}

		public bool GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return new Metadata.TypeSpecification(module, handle).GetDefinition() == type;
		}*/
	}
}