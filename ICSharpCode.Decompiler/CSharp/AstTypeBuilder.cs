using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

using FullTypeName = ICSharpCode.Decompiler.TypeSystem.FullTypeName;
using TopLevelTypeName = ICSharpCode.Decompiler.TypeSystem.TopLevelTypeName;

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
		readonly ConvertTypeOptions options;

		public AstTypeBuilder(ConvertTypeOptions options)
		{
			this.options = options;
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
			if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
				return AstType.Create("IntPtr");
			return AstType.Create("System.IntPtr");
		}

		static readonly FullTypeName systemNullable = new TopLevelTypeName("System", "Nullable", 1);

		public AstType GetGenericInstantiation(AstType genericType, ImmutableArray<AstType> typeArguments)
		{
			var typeName = genericType.Annotations.OfType<FullTypeName>().FirstOrDefault();
			if (typeName == systemNullable && typeArguments.Length == 1) {
				return typeArguments[0].MakeNullableType();
			}
			if (typeName.IsNested && genericType is MemberType mt && typeArguments.Length > typeName.TypeParameterCount) {
				// Some type arguments belong to the outer type:
				int outerTpc = typeArguments.Length - typeName.TypeParameterCount;
				Debug.Assert(outerTpc > 0);
				GetGenericInstantiation(mt.Target, typeArguments.Slice(0, outerTpc).ToImmutableArray());
				foreach (var ta in typeArguments.Slice(typeArguments.Length - typeName.TypeParameterCount)) {
					mt.AddChild(ta, Roles.TypeArgument);
				}
			} else {
				foreach (var ta in typeArguments) {
					genericType.AddChild(ta, Roles.TypeArgument);
				}
			}
			return genericType;
		}
		
		public AstType GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return new SimpleType(genericContext.GetGenericMethodTypeParameterName(index));
		}

		public AstType GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return new SimpleType(genericContext.GetGenericTypeParameterName(index));
		}

		public AstType GetModifiedType(AstType modifier, AstType unmodifiedType, bool isRequired)
		{
			return unmodifiedType;
		}

		public AstType GetPinnedType(AstType elementType)
		{
			return elementType;
		}

		public AstType GetPointerType(AstType elementType)
		{
			return elementType.MakePointerType();
		}

		public AstType GetPrimitiveType(PrimitiveTypeCode typeCode)
		{
			var knownTypeCode = typeCode.ToKnownTypeCode();
			var ktr = Decompiler.TypeSystem.KnownTypeReference.Get(knownTypeCode);
			if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) == 0) {
				string keyword = Decompiler.TypeSystem.KnownTypeReference.GetCSharpNameByTypeCode(knownTypeCode);
				if (keyword != null)
					return new PrimitiveType(keyword);
			}
			if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
				return new SimpleType(ktr.Name);
			return AstType.Create(ktr.Namespace).MemberType(ktr.Name);
		}

		public AstType GetSZArrayType(AstType elementType)
		{
			return elementType.MakeArrayType();
		}

		public AstType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
		{
			return MakeAstType(handle.GetFullTypeName(reader));
		}

		public AstType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return MakeAstType(handle.GetFullTypeName(reader));
		}

		public AstType GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			var ts = reader.GetTypeSpecification(handle);
			return ts.DecodeSignature(this, genericContext);
		}

		AstType MakeAstType(FullTypeName fullTypeName)
		{
			if (fullTypeName.IsNested) {
				int count = fullTypeName.GetNestedTypeAdditionalTypeParameterCount(fullTypeName.NestingLevel - 1);
				var outerType = MakeAstType(fullTypeName.GetDeclaringType());
				// Note: we must always emit the outer type name here;
				// if not desired it can only be cleaned up after InsertDynamicTypeVisitor.
				var nestedType = new MemberType(outerType, fullTypeName.Name);
				nestedType.AddAnnotation(fullTypeName);
				return nestedType;
			}
			AstType baseType;
			var topLevel = fullTypeName.TopLevelTypeName;
			if ((options & ConvertTypeOptions.IncludeNamespace) != 0) {
				baseType = AstType.Create(topLevel.Namespace + "." + topLevel.Name);
			} else {
				baseType = AstType.Create(topLevel.Name);
			}
			baseType.AddAnnotation(fullTypeName);
			return baseType;
		}
	}
}
