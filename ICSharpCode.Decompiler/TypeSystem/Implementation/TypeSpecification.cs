// Copyright (c) 2017 Siegfried Pammer
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	class SignatureTypeReference : ITypeReference
	{
		readonly TypeSpecification typeSpecification;
		readonly MetadataReader reader;

		public SignatureTypeReference(TypeSpecificationHandle handle, MetadataReader reader)
		{
			this.typeSpecification = reader.GetTypeSpecification(handle);
			this.reader = reader;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return typeSpecification.DecodeSignature(new TypeReferenceSignatureDecoder(), default(Unit)).Resolve(context);
		}
	}

	public sealed class PinnedType : TypeWithElementType
	{
		public PinnedType(IType elementType)
			: base(elementType)
		{
		}

		public override string NameSuffix => " pinned";

		public override bool? IsReferenceType => elementType.IsReferenceType;

		public override TypeKind Kind => TypeKind.Other;

		public override ITypeReference ToTypeReference()
		{
			return new PinnedTypeReference(elementType.ToTypeReference());
		}

		public override IType VisitChildren(TypeVisitor visitor)
		{
			var newType = elementType.AcceptVisitor(visitor);
			if (newType == elementType)
				return this;
			return new PinnedType(newType);
		}
	}

	public sealed class PinnedTypeReference : ITypeReference
	{
		readonly ITypeReference elementType;

		public PinnedTypeReference(ITypeReference elementType)
		{
			this.elementType = elementType;
		}

		public IType Resolve(ITypeResolveContext context)
		{
			return new PinnedType(elementType.Resolve(context));
		}
	}

	public struct Unit { }

	class TypeReferenceSignatureDecoder : ISignatureTypeProvider<ITypeReference, Unit>
	{
		public ITypeReference GetArrayType(ITypeReference elementType, ArrayShape shape)
		{
			return new ArrayTypeReference(elementType, shape.Rank);
		}

		public ITypeReference GetByReferenceType(ITypeReference elementType)
		{
			return new ByReferenceTypeReference(elementType);
		}

		public ITypeReference GetFunctionPointerType(MethodSignature<ITypeReference> signature)
		{
			return KnownTypeReference.IntPtr;
		}

		public ITypeReference GetGenericInstantiation(ITypeReference genericType, ImmutableArray<ITypeReference> typeArguments)
		{
			return new ParameterizedTypeReference(genericType, typeArguments);
		}

		public ITypeReference GetGenericMethodParameter(Unit genericContext, int index)
		{
			return new TypeParameterReference(SymbolKind.Method, index);
		}

		public ITypeReference GetGenericTypeParameter(Unit genericContext, int index)
		{
			return new TypeParameterReference(SymbolKind.TypeDefinition, index);
		}

		public ITypeReference GetModifiedType(ITypeReference modifier, ITypeReference unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public ITypeReference GetPinnedType(ITypeReference elementType)
		{
			return new PinnedTypeReference(elementType);
		}

		public ITypeReference GetPointerType(ITypeReference elementType)
		{
			return new PointerTypeReference(elementType);
		}

		public ITypeReference GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			return KnownTypeReference.Get(typeCode.ToKnownTypeCode());
		}

		public ITypeReference GetSZArrayType(ITypeReference elementType)
		{
			return new ArrayTypeReference(elementType);
		}

		public ITypeReference GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return new DefaultUnresolvedTypeDefinition(handle.GetFullTypeName(reader).ReflectionName);
		}

		public ITypeReference GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			var asmref = handle.GetDeclaringAssembly(reader);
			if (asmref.IsNil)
				return new GetClassTypeReference(handle.GetFullTypeName(reader), DefaultAssemblyReference.CurrentAssembly);
			var asm = reader.GetAssemblyReference(asmref);
			return new GetClassTypeReference(handle.GetFullTypeName(reader), new DefaultAssemblyReference(reader.GetString(asm.Name)));
		}

		public ITypeReference GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return new SignatureTypeReference(handle, reader);
		}
	}

	class TypeSystemAttributeTypeProvider : ICustomAttributeTypeProvider<ITypeReference>
	{
		public ITypeReference GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			return KnownTypeReference.Get(typeCode.ToKnownTypeCode());
		}

		public ITypeReference GetSystemType()
		{
			return KnownTypeReference.Get(KnownTypeCode.Type);
		}

		public ITypeReference GetSZArrayType(ITypeReference elementType)
		{
			return new ArrayTypeReference(elementType);
		}

		public ITypeReference GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			var type = reader.GetTypeDefinition(handle);
			return new DefaultUnresolvedTypeDefinition(type.GetFullTypeName(reader).ToString());
		}

		public ITypeReference GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return new DefaultUnresolvedTypeDefinition(handle.GetFullTypeName(reader).ToString());
		}

		public ITypeReference GetTypeFromSerializedName(string name)
		{
			return new GetClassTypeReference(new FullTypeName(name));
		}

		public PrimitiveTypeCode GetUnderlyingEnumType(ITypeReference type)
		{
			var def = type.GetEnumUnderlyingType().GetDefinition();
			if (def == null)
				throw new InvalidOperationException();
			return def.KnownTypeCode.ToPrimtiveTypeCode();
		}

		public bool IsSystemType(IType type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}
}
