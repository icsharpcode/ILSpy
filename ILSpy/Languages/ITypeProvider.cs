using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Dom;

namespace ICSharpCode.ILSpy
{
	

	public sealed class CSharpSignatureTypeProvider : ISignatureTypeProvider<string, GenericContext>
	{
		public string GetArrayType(string elementType, ArrayShape shape)
		{
			throw new NotImplementedException();
		}

		public string GetByReferenceType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetFunctionPointerType(MethodSignature<string> signature)
		{
			throw new NotImplementedException();
		}

		public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
		{
			throw new NotImplementedException();
		}

		public string GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			throw new NotImplementedException();
		}

		public string GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			throw new NotImplementedException();
		}

		public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
		{
			throw new NotImplementedException();
		}

		public string GetPinnedType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetPointerType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			throw new NotImplementedException();
		}

		public string GetSZArrayType(string elementType)
		{
			throw new NotImplementedException();
		}

		public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}

		public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}

		public string GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}
	}
}
