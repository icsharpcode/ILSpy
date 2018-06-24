// Copyright (c) 2018 Daniel Grunwald
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

using SRM = System.Reflection.Metadata;
using System.Diagnostics;
using System;
using ICSharpCode.Decompiler.Util;
using System.Linq;
using System.Collections.Immutable;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataTypeReference : ITypeReference
	{
		readonly SRM.EntityHandle type;
		readonly SRM.MetadataReader metadata;
		readonly SRM.CustomAttributeHandleCollection? typeAttributes;
		readonly TypeSystemOptions options;

		public MetadataTypeReference(SRM.EntityHandle type,
			SRM.MetadataReader metadata,
			SRM.CustomAttributeHandleCollection? typeAttributes,
			TypeSystemOptions options)
		{
			this.type = type;
			this.metadata = metadata;
			this.typeAttributes = typeAttributes;
			this.options = options;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return Resolve(type, metadata,
				new TypeProvider(context.CurrentAssembly),
				new GenericContext(context),
				options, typeAttributes);
		}

		public static IType Resolve(SRM.EntityHandle type,
			SRM.MetadataReader metadata,
			TypeProvider typeProvider,
			GenericContext genericContext,
			TypeSystemOptions options,
			SRM.CustomAttributeHandleCollection? typeAttributes = null)
		{
			if (type.IsNil)
				return SpecialType.UnknownType;
			IType ty;
			switch (type.Kind) {
				case SRM.HandleKind.TypeDefinition:
					ty = typeProvider.GetTypeFromDefinition(metadata, (SRM.TypeDefinitionHandle)type, 0);
					break;
				case SRM.HandleKind.TypeReference:
					ty = typeProvider.GetTypeFromReference(metadata, (SRM.TypeReferenceHandle)type, 0);
					break;
				case SRM.HandleKind.TypeSpecification:
					var typeSpec = metadata.GetTypeSpecification((SRM.TypeSpecificationHandle)type);
					ty = typeSpec.DecodeSignature(typeProvider, genericContext);
					break;
				default:
					Debug.Fail("Not a type handle");
					ty = SpecialType.UnknownType;
					break;
			}
			ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, typeProvider.Compilation,
					typeAttributes, metadata, options);
			return ty;
		}
	}

	sealed class FieldTypeReference : ITypeReference
	{
		readonly SRM.EntityHandle fieldHandle;
		readonly SRM.MetadataReader metadata;
		readonly TypeSystemOptions options;

		public FieldTypeReference(SRM.FieldDefinitionHandle fieldHandle,
			SRM.MetadataReader metadata,
			TypeSystemOptions options)
		{
			this.fieldHandle = fieldHandle;
			this.metadata = metadata;
			this.options = options;
		}

		public FieldTypeReference(SRM.MemberReferenceHandle fieldReferenceHandle,
			SRM.MetadataReader metadata,
			TypeSystemOptions attributeOptions)
		{
			this.fieldHandle = fieldReferenceHandle;
			this.metadata = metadata;
			this.options = attributeOptions;
		}

		IType ITypeReference.Resolve(ITypeResolveContext context)
		{
			if (fieldHandle.Kind == SRM.HandleKind.FieldDefinition) {
				return Resolve((SRM.FieldDefinitionHandle)fieldHandle, metadata, context, options);
			} else {
				var memberRef = metadata.GetMemberReference((SRM.MemberReferenceHandle)fieldHandle);
				IType ty = memberRef.DecodeFieldSignature(new TypeProvider(context.CurrentAssembly), new GenericContext(context));
				ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, context.Compilation,
					memberRef.GetCustomAttributes(), metadata, options);
				return ty;
			}
		}

		public static IType Resolve(SRM.FieldDefinitionHandle fieldHandle,
			SRM.MetadataReader metadata,
			ITypeResolveContext context,
			TypeSystemOptions options)
		{
			var fieldDef = metadata.GetFieldDefinition(fieldHandle);
			IType ty = fieldDef.DecodeSignature(new TypeProvider(context.CurrentAssembly), new GenericContext(context));
			ty = ApplyAttributeTypeVisitor.ApplyAttributesToType(ty, context.Compilation,
				fieldDef.GetCustomAttributes(), metadata, options);
			return ty;
		}
	}

	/// <summary>
	/// Represents an unresolved method signature.
	/// </summary>
	sealed class UnresolvedMethodSignature
	{
		readonly SRM.EntityHandle handle;
		readonly SRM.MetadataReader metadata;
		readonly TypeSystemOptions options;

		public UnresolvedMethodSignature(SRM.MethodDefinitionHandle handle, SRM.MetadataReader metadata,
			TypeSystemOptions options)
		{
			this.handle = handle;
			this.metadata = metadata;
			this.options = options;
		}

		public UnresolvedMethodSignature(SRM.PropertyDefinitionHandle handle, SRM.MetadataReader metadata,
			TypeSystemOptions attributeOptions)
		{
			this.handle = handle;
			this.metadata = metadata;
			this.options = attributeOptions;
		}

		public SRM.MethodSignature<IType> Resolve(ITypeResolveContext context)
		{
			return (SRM.MethodSignature<IType>)context.Compilation.CacheManager.GetOrAddShared(
				this, key => {
					Debug.Assert(key == this);
					switch (handle.Kind) {
						case SRM.HandleKind.MethodDefinition:
							return GetSignature(
								metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)handle),
								metadata, context, options
							);
						case SRM.HandleKind.PropertyDefinition:
							return GetSignature(
								metadata.GetPropertyDefinition((SRM.PropertyDefinitionHandle)handle),
								metadata, context, options
							);
						default:
							throw new InvalidOperationException();
					}
				}
			);
		}

		public static SRM.MethodSignature<IType> GetSignature(SRM.MethodDefinition methodDef,
			SRM.MetadataReader metadata, ITypeResolveContext context,
			TypeSystemOptions options)
		{
			var typeProvider = new TypeProvider(context.CurrentAssembly);
			var signature = methodDef.DecodeSignature(typeProvider, new GenericContext(context));
			return ApplyAttributes(signature, methodDef.GetParameters(), context.Compilation, metadata, options);
		}

		public static SRM.MethodSignature<IType> GetSignature(SRM.PropertyDefinition propertyDef,
			SRM.MetadataReader metadata, ITypeResolveContext context,
			TypeSystemOptions options)
		{
			var typeProvider = new TypeProvider(context.CurrentAssembly);
			var signature = propertyDef.DecodeSignature(typeProvider, new GenericContext(context));
			var accessors = propertyDef.GetAccessors();
			SRM.ParameterHandleCollection? parameterHandles = null;
			if (!accessors.Getter.IsNil) {
				var getter = metadata.GetMethodDefinition(accessors.Getter);
				parameterHandles = getter.GetParameters();
			} else {
				if (!accessors.Setter.IsNil) {
					var setter = metadata.GetMethodDefinition(accessors.Setter);
					parameterHandles = setter.GetParameters();
				}
			}
			return ApplyAttributes(signature, parameterHandles, context.Compilation, metadata, options);
		}

		static SRM.MethodSignature<IType> ApplyAttributes(SRM.MethodSignature<IType> signature, SRM.ParameterHandleCollection? parameterHandles, ICompilation compilation, SRM.MetadataReader metadata, TypeSystemOptions options)
		{
			SRM.CustomAttributeHandleCollection? returnTypeAttributes = null;
			var parameterAttributes = new SRM.CustomAttributeHandleCollection?[signature.ParameterTypes.Length];
			if (parameterHandles != null) {
				foreach (var parameterHandle in parameterHandles) {
					var par = metadata.GetParameter(parameterHandle);
					if (par.SequenceNumber == 0) {
						returnTypeAttributes = par.GetCustomAttributes();
					} else if (par.SequenceNumber <= parameterAttributes.Length) {
						parameterAttributes[par.SequenceNumber - 1] = par.GetCustomAttributes();
					}
				}
			}
			IType returnType = ApplyAttributeTypeVisitor.ApplyAttributesToType(
				signature.ReturnType, compilation, returnTypeAttributes, metadata, options
			);
			var parameterTypes = signature.ParameterTypes.SelectWithIndex(
				(i, p) => ApplyAttributeTypeVisitor.ApplyAttributesToType(
					p, compilation, parameterAttributes[i], metadata, options
				)
			).ToImmutableArray();
			return new SRM.MethodSignature<IType>(
				signature.Header, returnType,
				signature.RequiredParameterCount, signature.GenericParameterCount,
				parameterTypes
			);
		}
	}

	sealed class SignatureParameterTypeReference : ITypeReference
	{
		readonly UnresolvedMethodSignature unresolvedSig;
		readonly int index;

		public SignatureParameterTypeReference(UnresolvedMethodSignature unresolvedSig, int index)
		{
			this.unresolvedSig = unresolvedSig;
			this.index = index;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			var sig = unresolvedSig.Resolve(context);
			if (index < sig.ParameterTypes.Length)
				return sig.ParameterTypes[index];
			else
				return SpecialType.UnknownType;
		}
	}

	sealed class SignatureReturnTypeReference : ITypeReference
	{
		readonly UnresolvedMethodSignature unresolvedSig;

		public SignatureReturnTypeReference(UnresolvedMethodSignature unresolvedSig)
		{
			this.unresolvedSig = unresolvedSig;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return unresolvedSig.Resolve(context).ReturnType;
		}
	}
}
