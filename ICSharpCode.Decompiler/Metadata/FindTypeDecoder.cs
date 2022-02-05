// Copyright (c) 2022 Siegfried Pammer
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
	public class FindTypeDecoder : ISignatureTypeProvider<bool, Unit>
	{
		readonly PEFile declaringModule;
		readonly MetadataModule? currentModule;
		readonly TypeDefinitionHandle handle;
		readonly string? typeName;
		readonly string? namespaceName;
		readonly PrimitiveTypeCode primitiveType;

		/// <summary>
		/// Constructs a FindTypeDecoder that finds uses of a specific type-definition handle.
		/// This assumes that the module we are search in is the same as the module containing the type-definiton.
		/// </summary>
		internal FindTypeDecoder(TypeDefinitionHandle handle, PEFile declaringModule)
		{
			this.handle = handle;
			this.declaringModule = declaringModule;
			this.primitiveType = 0;
			this.currentModule = null;
		}

		/// <summary>
		/// Constructs a FindTypeDecoder that can be used to find <paramref name="type"/> in signatures from <paramref name="currentModule"/>.
		/// </summary>
		public FindTypeDecoder(MetadataModule currentModule, ITypeDefinition type)
		{
			this.currentModule = currentModule;
			this.declaringModule = type.ParentModule.PEFile ?? throw new InvalidOperationException("Cannot use MetadataModule without PEFile as context.");
			this.handle = (TypeDefinitionHandle)type.MetadataToken;
			this.primitiveType = type.KnownTypeCode == KnownTypeCode.None ? 0 : type.KnownTypeCode.ToPrimitiveTypeCode();
			this.typeName = type.MetadataName;
			this.namespaceName = type.Namespace;
		}

		public bool GetArrayType(bool elementType, ArrayShape shape) => elementType;
		public bool GetByReferenceType(bool elementType) => elementType;
		public bool GetFunctionPointerType(MethodSignature<bool> signature)
		{
			if (signature.ReturnType)
				return true;
			foreach (bool type in signature.ParameterTypes)
			{
				if (type)
					return true;
			}
			return false;
		}

		public bool GetGenericInstantiation(bool genericType, ImmutableArray<bool> typeArguments)
		{
			if (genericType)
				return true;
			foreach (bool ta in typeArguments)
			{
				if (ta)
					return true;
			}
			return false;
		}

		public bool GetGenericMethodParameter(Unit genericContext, int index) => false;
		public bool GetGenericTypeParameter(Unit genericContext, int index) => false;
		public bool GetModifiedType(bool modifier, bool unmodifiedType, bool isRequired) => unmodifiedType || modifier;
		public bool GetPinnedType(bool elementType) => elementType;
		public bool GetPointerType(bool elementType) => elementType;

		public bool GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			return typeCode == primitiveType;
		}

		public bool GetSZArrayType(bool elementType) => elementType;

		public bool GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return this.handle == handle && reader == declaringModule.Metadata;
		}

		public bool GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			if (currentModule == null || typeName == null || namespaceName == null)
				return false;

			var tr = reader.GetTypeReference(handle);
			if (!reader.StringComparer.Equals(tr.Name, typeName))
				return false;
			if (!((tr.Namespace.IsNil && namespaceName.Length == 0) || reader.StringComparer.Equals(tr.Namespace, namespaceName)))
				return false;

			var t = currentModule.ResolveType(handle, default);
			var td = t.GetDefinition();
			if (td == null)
				return false;

			return td.MetadataToken == this.handle && td.ParentModule.PEFile == declaringModule;
		}

		public bool GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
		}
	}

}
