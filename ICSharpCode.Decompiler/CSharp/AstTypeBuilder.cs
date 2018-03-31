using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

using FullTypeName = ICSharpCode.Decompiler.TypeSystem.FullTypeName;

namespace ICSharpCode.Decompiler.CSharp
{
	[Flags]
	public enum ConvertTypeOptions
	{
		None = 0,
		IncludeNamespace = 1,
		IncludeTypeParameterDefinitions = 2,
		DoNotUsePrimitiveTypeNames = 4,
		IncludeOuterTypeName = 8,
	}

	public class AstTypeBuilder : ISignatureTypeProvider<AstType, GenericContext>
	{
		ConvertTypeOptions options;

		public static AstType BuildAstType(EntityHandle type, PEFile module, GenericContext context, CustomAttributeHandleCollection customAttributes = default, ConvertTypeOptions options = ConvertTypeOptions.None)
		{
			if (type.IsNil)
				return AstType.Null;
			var metadata = module.GetMetadataReader();
			var provider = new AstTypeBuilder() { options = options };
			switch (type.Kind) {
				case HandleKind.TypeDefinition:
					return provider.GetTypeFromDefinition(metadata, (TypeDefinitionHandle)type, 0);
				case HandleKind.TypeReference:
					return provider.GetTypeFromReference(metadata, (TypeReferenceHandle)type, 0);
				case HandleKind.TypeSpecification:
					var ts = metadata.GetTypeSpecification((TypeSpecificationHandle)type);
					return ts.DecodeSignature(provider, context);
				default:
					throw new NotSupportedException();
			}
		}

		public AstType GetArrayType(AstType elementType, ArrayShape shape)
		{
			return elementType.MakeArrayType(shape.Rank);
		}

		public AstType GetByReferenceType(AstType elementType)
		{
			return elementType.MakeRefType();
		}

		public AstType GetFunctionPointerType(MethodSignature<AstType> signature)
		{
			throw new NotImplementedException();
		}

		public AstType GetGenericInstantiation(AstType genericType, ImmutableArray<AstType> typeArguments)
		{
			return new ComposedType { BaseType = genericType };
		}

		public AstType GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			throw new NotImplementedException();
		}

		public AstType GetGenericTypeParameter(GenericContext genericContext, int index)
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
			return elementType.MakePointerType();
		}

		public AstType GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			AstType t;
			switch (typeCode) {
				case PrimitiveTypeCode.Boolean:
					return new PrimitiveType("bool");
				case PrimitiveTypeCode.Byte:
					return new PrimitiveType("byte");
				case PrimitiveTypeCode.SByte:
					return new PrimitiveType("sbyte");
				case PrimitiveTypeCode.Char:
					return new PrimitiveType("char");
				case PrimitiveTypeCode.Int16:
					return new PrimitiveType("short");
				case PrimitiveTypeCode.UInt16:
					return new PrimitiveType("ushort");
				case PrimitiveTypeCode.Int32:
					return new PrimitiveType("int");
				case PrimitiveTypeCode.UInt32:
					return new PrimitiveType("uint");
				case PrimitiveTypeCode.Int64:
					return new PrimitiveType("long");
				case PrimitiveTypeCode.UInt64:
					return new PrimitiveType("ulong");
				case PrimitiveTypeCode.Single:
					return new PrimitiveType("float");
				case PrimitiveTypeCode.Double:
					return new PrimitiveType("double");
				case PrimitiveTypeCode.IntPtr:
					t = AstType.Create("System.IntPtr");
					t.AddAnnotation(new FullTypeName("System.IntPtr"));
					return t;
				case PrimitiveTypeCode.UIntPtr:
					t = AstType.Create("System.UIntPtr");
					t.AddAnnotation(new FullTypeName("System.UIntPtr"));
					return t;
				case PrimitiveTypeCode.Object:
					return new PrimitiveType("object"); // TODO : dynamic
				case PrimitiveTypeCode.String:
					return new PrimitiveType("string");
				case PrimitiveTypeCode.TypedReference:
					t = AstType.Create("System.TypedReference");
					t.AddAnnotation(new FullTypeName("System.TypedReference"));
					return t;
				case PrimitiveTypeCode.Void:
					return new PrimitiveType("void");
				default:
					throw new NotSupportedException();
			}
		}

		public AstType GetSZArrayType(AstType elementType)
		{
			return elementType.MakeArrayType();
		}

		public AstType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			AstType MakeAstType(FullTypeName fullTypeName)
			{
				if (fullTypeName.IsNested) {
					if ((options & (ConvertTypeOptions.IncludeOuterTypeName | ConvertTypeOptions.IncludeNamespace)) != 0) {
						var outerType = MakeAstType(fullTypeName.GetDeclaringType());
						return new MemberType(outerType, fullTypeName.Name);
					} else {
						return AstType.Create(fullTypeName.Name);
					}
				}
				var name = Decompiler.TypeSystem.ReflectionHelper.SplitTypeParameterCountFromReflectionName(fullTypeName.ReflectionName, out int parameterCount);
				if ((options & ConvertTypeOptions.IncludeNamespace) != 0)
					return AstType.Create(name);
				return AstType.Create(fullTypeName.Name);
			}
			return MakeAstType(handle.GetFullTypeName(reader));
		}

		public AstType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}

		public AstType GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			throw new NotImplementedException();
		}
#if false
		static void ApplyTypeArgumentsTo(AstType baseType, List<AstType> typeArguments)
		{
			SimpleType st = baseType as SimpleType;
			if (st != null) {
				TypeReference type = st.Annotation<TypeReference>();
				if (type != null) {
					ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out int typeParameterCount);
					if (typeParameterCount > typeArguments.Count)
						typeParameterCount = typeArguments.Count;
					st.TypeArguments.AddRange(typeArguments.GetRange(typeArguments.Count - typeParameterCount, typeParameterCount));
				} else {
					st.TypeArguments.AddRange(typeArguments);

				}
			}
			MemberType mt = baseType as MemberType;
			if (mt != null) {
				TypeReference type = mt.Annotation<TypeReference>();
				if (type != null) {
					ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out int typeParameterCount);
					if (typeParameterCount > typeArguments.Count)
						typeParameterCount = typeArguments.Count;
					mt.TypeArguments.AddRange(typeArguments.GetRange(typeArguments.Count - typeParameterCount, typeParameterCount));
					typeArguments.RemoveRange(typeArguments.Count - typeParameterCount, typeParameterCount);
					if (typeArguments.Count > 0)
						ApplyTypeArgumentsTo(mt.Target, typeArguments);
				} else {
					mt.TypeArguments.AddRange(typeArguments);
				}
			}
		}
#endif

	}

	class StringSignatureBuilder : ISignatureTypeProvider<string, GenericContext>
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
