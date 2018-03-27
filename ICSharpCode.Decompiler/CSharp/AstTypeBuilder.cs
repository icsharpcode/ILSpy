using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp
{
	class AstTypeBuilder : ISignatureTypeProvider<AstType, Unit>
	{
		public AstType GetArrayType(AstType elementType, ArrayShape shape)
		{
			throw new NotImplementedException();
		}

		public AstType GetByReferenceType(AstType elementType)
		{
			throw new NotImplementedException();
		}

		public AstType GetFunctionPointerType(MethodSignature<AstType> signature)
		{
			throw new NotImplementedException();
		}

		public AstType GetGenericInstantiation(AstType genericType, ImmutableArray<AstType> typeArguments)
		{
			throw new NotImplementedException();
		}

		public AstType GetGenericMethodParameter(Unit genericContext, int index)
		{
			throw new NotImplementedException();
		}

		public AstType GetGenericTypeParameter(Unit genericContext, int index)
		{
			throw new NotImplementedException();
		}

		public AstType GetModifiedType(AstType modifier, AstType unmodifiedType, bool isRequired)
		{
			throw new NotImplementedException();
		}

		public AstType GetPinnedType(AstType elementType)
		{
			throw new NotImplementedException();
		}

		public AstType GetPointerType(AstType elementType)
		{
			throw new NotImplementedException();
		}

		public AstType GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			throw new NotImplementedException();
		}

		public AstType GetSZArrayType(AstType elementType)
		{
			throw new NotImplementedException();
		}

		public AstType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}

		public AstType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}

		public AstType GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}
	}
}
