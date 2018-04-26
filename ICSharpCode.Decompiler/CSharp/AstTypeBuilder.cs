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
			throw new NotImplementedException();
		}

		public AstType GetGenericInstantiation(AstType genericType, ImmutableArray<AstType> typeArguments)
		{
			switch (genericType) {
				case SimpleType st:
					st.TypeArguments.AddRange(typeArguments);
					return st;
				case MemberType mt:
					mt.TypeArguments.AddRange(typeArguments);
					return mt;
				default:
					throw new NotImplementedException();
			}
		}

		public AstType GetGenericMethodParameter(GenericContext genericContext, int index)
		{
			return new SimpleType(genericContext.GetGenericMethodTypeParameterName(index));
		}

		public AstType GetGenericTypeParameter(GenericContext genericContext, int index)
		{
			return new SimpleType(genericContext.GetGenericMethodTypeParameterName(index));
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
			switch (typeCode) {
				case PrimitiveTypeCode.Boolean:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Boolean");
						return AstType.Create("System.Boolean");
					}
					return new PrimitiveType("bool");
				case PrimitiveTypeCode.Byte:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Byte");
						return AstType.Create("System.Byte");
					}
					return new PrimitiveType("byte");
				case PrimitiveTypeCode.SByte:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("SByte");
						return AstType.Create("System.SByte");
					}
					return new PrimitiveType("sbyte");
				case PrimitiveTypeCode.Char:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Char");
						return AstType.Create("System.Char");
					}
					return new PrimitiveType("char");
				case PrimitiveTypeCode.Int16:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Int16");
						return AstType.Create("System.Int16");
					}
					return new PrimitiveType("short");
				case PrimitiveTypeCode.UInt16:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("UInt16");
						return AstType.Create("System.UInt16");
					}
					return new PrimitiveType("ushort");
				case PrimitiveTypeCode.Int32:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Int32");
						return AstType.Create("System.In32");
					}
					return new PrimitiveType("int");
				case PrimitiveTypeCode.UInt32:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("UInt32");
						return AstType.Create("System.UInt32");
					}
					return new PrimitiveType("uint");
				case PrimitiveTypeCode.Int64:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Int64");
						return AstType.Create("System.Int64");
					}
					return new PrimitiveType("long");
				case PrimitiveTypeCode.UInt64:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("UInt64");
						return AstType.Create("System.UInt64");
					}
					return new PrimitiveType("ulong");
				case PrimitiveTypeCode.Single:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Single");
						return AstType.Create("System.Single");
					}
					return new PrimitiveType("float");
				case PrimitiveTypeCode.Double:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Double");
						return AstType.Create("System.Double");
					}
					return new PrimitiveType("double");
				case PrimitiveTypeCode.IntPtr:
					if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
						return AstType.Create("IntPtr");
					return AstType.Create("System.IntPtr");
				case PrimitiveTypeCode.UIntPtr:
					if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
						return AstType.Create("UIntPtr");
					return AstType.Create("System.UIntPtr");
				case PrimitiveTypeCode.Object:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("Object");
						return AstType.Create("System.Object");
					}
					return new PrimitiveType("object");
				case PrimitiveTypeCode.String:
					if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames) != 0) {
						if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
							return AstType.Create("String");
						return AstType.Create("System.String");
					}
					return new PrimitiveType("string");
				case PrimitiveTypeCode.TypedReference:
					if ((options & ConvertTypeOptions.IncludeNamespace) == 0)
						return AstType.Create("TypedReference");
					return AstType.Create("System.TypedReference");
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
			var td = reader.GetTypeDefinition(handle);
			var genericParams = td.GetGenericParameters().Select(gp => (AstType)new SimpleType(reader.GetString(reader.GetGenericParameter(gp).Name))).ToList();
			return MakeAstType(handle.GetFullTypeName(reader), genericParams);
		}

		public AstType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
		{
			return MakeAstType(handle.GetFullTypeName(reader), EmptyList<AstType>.Instance);
		}

		public AstType GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			var ts = reader.GetTypeSpecification(handle);
			return ts.DecodeSignature(this, genericContext);
		}

		AstType MakeAstType(FullTypeName fullTypeName, IList<AstType> genericParams)
		{
			if (fullTypeName.IsNested) {
				int count = fullTypeName.GetNestedTypeAdditionalTypeParameterCount(fullTypeName.NestingLevel - 1);
				if ((options & (ConvertTypeOptions.IncludeOuterTypeName | ConvertTypeOptions.IncludeNamespace)) != 0) {
					var outerType = MakeAstType(fullTypeName.GetDeclaringType(), genericParams);
					var mt = new MemberType(outerType, fullTypeName.Name);
					ApplyGenericParametersTo(mt, genericParams, count);
					return mt;
				} else {
					var st = new SimpleType(fullTypeName.Name);
					for (int i = 0; i < fullTypeName.TypeParameterCount - count; i++)
						genericParams.RemoveAt(0);
					ApplyGenericParametersTo(st, genericParams, count);
					return st;
				}
			}
			AstType baseType;
			var topLevel = fullTypeName.TopLevelTypeName;
			if ((options & ConvertTypeOptions.IncludeNamespace) != 0) {
				baseType = AstType.Create(topLevel.Namespace + "." + topLevel.Name);
			} else {
				baseType = AstType.Create(topLevel.Name);
			}
			ApplyGenericParametersTo(baseType, genericParams, topLevel.TypeParameterCount);
			return baseType;
		}

		void ApplyGenericParametersTo(AstType targetType, IList<AstType> genericParams, int count)
		{
			if (count > genericParams.Count)
				count = genericParams.Count;
			int i = 0;
			switch (targetType) {
				case MemberType mt:
					while (i < count) {
						// Always add the first and remove it from the other list.
						mt.TypeArguments.Add(genericParams[0]);
						genericParams.RemoveAt(0);
						i++;
					}
					break;
				case SimpleType st:
					while (i < count) {
						// Always add the first and remove it from the other list.
						st.TypeArguments.Add(genericParams[0]);
						genericParams.RemoveAt(0);
						i++;
					}
					break;
			}
		}
	}
}
