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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	public enum TokenSearchResult : byte
	{
		NoResult = 0,
		Byte = PrimitiveTypeCode.Byte,
		SByte = PrimitiveTypeCode.SByte,
		Int16 = PrimitiveTypeCode.Int16,
		UInt16 = PrimitiveTypeCode.UInt16,
		Int32 = PrimitiveTypeCode.Int32,
		UInt32 = PrimitiveTypeCode.UInt32,
		Int64 = PrimitiveTypeCode.Int64,
		UInt64 = PrimitiveTypeCode.UInt64,
		IntPtr = PrimitiveTypeCode.IntPtr,
		UIntPtr = PrimitiveTypeCode.UIntPtr,

		// lowest PrimitiveTypeCode is 1
		// highest PrimitiveTypeCode is 28 (0b0001_1100)
		// TokenSearchResult with a PrimitiveTypeCode set is only used when decoding an enum-type.
		// It is used for GetUnderlyingEnumType and should be masked out in all other uses.
		// MSB = Found
		// 127 = System.Type
		TypeCodeMask = 0b0111_1111,
		Found = 0b1000_0000,
		SystemType = 127,
	}

	class FindTypeInAttributeDecoder : ICustomAttributeTypeProvider<TokenSearchResult>
	{
		readonly PEFile declaringModule;
		readonly MetadataModule currentModule;
		readonly TypeDefinitionHandle handle;
		readonly PrimitiveTypeCode primitiveType;

		/// <summary>
		/// Constructs a FindTypeInAttributeDecoder that can be used to find <paramref name="type"/> in signatures from <paramref name="currentModule"/>.
		/// </summary>
		public FindTypeInAttributeDecoder(MetadataModule currentModule, ITypeDefinition type)
		{
			this.currentModule = currentModule;
			this.declaringModule = type.ParentModule?.PEFile ?? throw new InvalidOperationException("Cannot use MetadataModule without PEFile as context.");
			this.handle = (TypeDefinitionHandle)type.MetadataToken;
			this.primitiveType = type.KnownTypeCode == KnownTypeCode.None ? 0 : type.KnownTypeCode.ToPrimitiveTypeCode();
		}

		public TokenSearchResult GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			return typeCode == primitiveType ? TokenSearchResult.Found : 0;
		}

		public TokenSearchResult GetSystemType() => TokenSearchResult.SystemType;

		public TokenSearchResult GetSZArrayType(TokenSearchResult elementType) => elementType & TokenSearchResult.Found;

		public TokenSearchResult GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			TokenSearchResult result = TokenSearchResult.NoResult;

			if (handle.IsEnum(reader, out PrimitiveTypeCode underlyingType))
			{
				result = (TokenSearchResult)underlyingType;
			}
			else if (((EntityHandle)handle).IsKnownType(reader, KnownTypeCode.Type))
			{
				result = TokenSearchResult.SystemType;
			}
			if (this.handle == handle && reader == declaringModule.Metadata)
			{
				result |= TokenSearchResult.Found;
			}
			return result;
		}

		public TokenSearchResult GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			var t = currentModule.ResolveType(handle, default);
			return GetResultFromResolvedType(t);
		}

		public TokenSearchResult GetTypeFromSerializedName(string name)
		{
			if (name == null)
			{
				return TokenSearchResult.NoResult;
			}
			try
			{
				IType type = ReflectionHelper.ParseReflectionName(name)
					.Resolve(new SimpleTypeResolveContext(currentModule));
				return GetResultFromResolvedType(type);
			}
			catch (ReflectionNameParseException)
			{
				return TokenSearchResult.NoResult;
			}
		}

		private TokenSearchResult GetResultFromResolvedType(IType type)
		{
			var td = type.GetDefinition();
			if (td == null)
				return TokenSearchResult.NoResult;

			TokenSearchResult result = TokenSearchResult.NoResult;
			var underlyingType = td.EnumUnderlyingType?.GetDefinition();
			if (underlyingType != null)
			{
				result = (TokenSearchResult)underlyingType.KnownTypeCode.ToPrimitiveTypeCode();
			}
			else if (td.KnownTypeCode == KnownTypeCode.Type)
			{
				result = TokenSearchResult.SystemType;
			}
			if (td.MetadataToken == this.handle && td.ParentModule?.PEFile == declaringModule)
			{
				result |= TokenSearchResult.Found;
			}
			return result;
		}

		public PrimitiveTypeCode GetUnderlyingEnumType(TokenSearchResult type)
		{
			TokenSearchResult typeCode = type & TokenSearchResult.TypeCodeMask;
			if (typeCode == 0 || typeCode == TokenSearchResult.SystemType)
				throw new EnumUnderlyingTypeResolveException();
			return (PrimitiveTypeCode)typeCode;
		}

		public bool IsSystemType(TokenSearchResult type) => (type & TokenSearchResult.TypeCodeMask) == TokenSearchResult.SystemType;
	}
}
