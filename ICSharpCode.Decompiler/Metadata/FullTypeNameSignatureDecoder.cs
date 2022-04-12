// Copyright (c) 2018 Siegfried Pammer
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

#nullable enable

using System;
using System.Collections.Immutable;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	public sealed class FullTypeNameSignatureDecoder : ISignatureTypeProvider<FullTypeName, Unit>, ICustomAttributeTypeProvider<FullTypeName>
	{
		readonly MetadataReader metadata;

		public FullTypeNameSignatureDecoder(MetadataReader metadata)
		{
			this.metadata = metadata;
		}

		public FullTypeName GetArrayType(FullTypeName elementType, ArrayShape shape)
		{
			return elementType;
		}

		public FullTypeName GetByReferenceType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetFunctionPointerType(MethodSignature<FullTypeName> signature)
		{
			return default;
		}

		public FullTypeName GetGenericInstantiation(FullTypeName genericType, ImmutableArray<FullTypeName> typeArguments)
		{
			return genericType;
		}

		public FullTypeName GetGenericMethodParameter(Unit genericContext, int index)
		{
			return default;
		}

		public FullTypeName GetGenericTypeParameter(Unit genericContext, int index)
		{
			return default;
		}

		public FullTypeName GetModifiedType(FullTypeName modifier, FullTypeName unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public FullTypeName GetPinnedType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetPointerType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			var ktr = KnownTypeReference.Get(typeCode.ToKnownTypeCode());
			if (ktr == null)
				return default;
			return new TopLevelTypeName(ktr.Namespace, ktr.Name, ktr.TypeParameterCount);
		}

		public FullTypeName GetSystemType()
		{
			return new TopLevelTypeName("System", "Type");
		}

		public FullTypeName GetSZArrayType(FullTypeName elementType)
		{
			return elementType;
		}

		public FullTypeName GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return handle.GetFullTypeName(reader);
		}

		public FullTypeName GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return handle.GetFullTypeName(reader);
		}

		public FullTypeName GetTypeFromSerializedName(string name)
		{
			return new FullTypeName(name);
		}

		public FullTypeName GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default);
		}

		public PrimitiveTypeCode GetUnderlyingEnumType(FullTypeName type)
		{
			throw new NotImplementedException();
		}

		public bool IsSystemType(FullTypeName type)
		{
			return type.IsKnownType(KnownTypeCode.Type);
		}
	}
}
