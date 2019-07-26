using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using SRM = System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.Decompiler
{
	public static partial class SRMExtensions
	{
		public static bool HasFlag(this TypeDefinition typeDefinition, TypeAttributes attribute) => (typeDefinition.Attributes & attribute) == attribute;
		public static bool HasFlag(this MethodDefinition methodDefinition, MethodAttributes attribute) => (methodDefinition.Attributes & attribute) == attribute;
		public static bool HasFlag(this FieldDefinition fieldDefinition, FieldAttributes attribute) => (fieldDefinition.Attributes & attribute) == attribute;
		public static bool HasFlag(this PropertyDefinition propertyDefinition, PropertyAttributes attribute) => (propertyDefinition.Attributes & attribute) == attribute;
		public static bool HasFlag(this EventDefinition eventDefinition, EventAttributes attribute) => (eventDefinition.Attributes & attribute) == attribute;

		public static bool IsTypeKind(this HandleKind kind) => kind == HandleKind.TypeDefinition || kind == HandleKind.TypeReference || kind == HandleKind.TypeSpecification;
		public static bool IsMemberKind(this HandleKind kind) => kind == HandleKind.MethodDefinition || kind == HandleKind.PropertyDefinition || kind == HandleKind.FieldDefinition
																|| kind == HandleKind.EventDefinition || kind == HandleKind.MemberReference || kind == HandleKind.MethodSpecification;

		public static bool IsValueType(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).IsValueType(reader);
		}

		public static bool IsValueType(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			EntityHandle baseType = typeDefinition.GetBaseTypeOrNil();
			if (baseType.IsNil)
				return false;
			if (baseType.IsKnownType(reader, KnownTypeCode.Enum))
				return true;
			if (!baseType.IsKnownType(reader, KnownTypeCode.ValueType))
				return false;
			var thisType = typeDefinition.GetFullTypeName(reader);
			return !thisType.IsKnownType(KnownTypeCode.Enum);
		}

		public static bool IsEnum(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).IsEnum(reader);
		}

		public static bool IsEnum(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			EntityHandle baseType = typeDefinition.GetBaseTypeOrNil();
			if (baseType.IsNil)
				return false;
			return baseType.IsKnownType(reader, KnownTypeCode.Enum);
		}

		public static bool IsEnum(this TypeDefinitionHandle handle, MetadataReader reader, out PrimitiveTypeCode underlyingType)
		{
			return reader.GetTypeDefinition(handle).IsEnum(reader, out underlyingType);
		}

		public static bool IsEnum(this TypeDefinition typeDefinition, MetadataReader reader, out PrimitiveTypeCode underlyingType)
		{
			underlyingType = 0;
			EntityHandle baseType = typeDefinition.GetBaseTypeOrNil();
			if (baseType.IsNil)
				return false;
			if (!baseType.IsKnownType(reader, KnownTypeCode.Enum))
				return false;
			foreach (var handle in typeDefinition.GetFields()) {
				var field = reader.GetFieldDefinition(handle);
				if ((field.Attributes & FieldAttributes.Static) != 0)
					continue;
				var blob = reader.GetBlobReader(field.Signature);
				if (blob.ReadSignatureHeader().Kind != SignatureKind.Field)
					return false;
				underlyingType = (PrimitiveTypeCode)blob.ReadByte();
				return true;
			}
			return false;
		}

		public static bool IsDelegate(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).IsDelegate(reader);
		}

		public static bool IsDelegate(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			var baseType = typeDefinition.GetBaseTypeOrNil();
			return !baseType.IsNil && baseType.IsKnownType(reader, KnownTypeCode.MulticastDelegate);
		}
		
		public static bool HasBody(this MethodDefinition methodDefinition)
		{
			const MethodAttributes noBodyAttrs = MethodAttributes.Abstract | MethodAttributes.PinvokeImpl;
			const MethodImplAttributes noBodyImplAttrs = MethodImplAttributes.InternalCall
				| MethodImplAttributes.Native | MethodImplAttributes.Unmanaged | MethodImplAttributes.Runtime;
			return (methodDefinition.Attributes & noBodyAttrs) == 0 &&
				(methodDefinition.ImplAttributes & noBodyImplAttrs) == 0 &&
				methodDefinition.RelativeVirtualAddress > 0;
		}

		public static int GetCodeSize(this MethodBodyBlock body)
		{
			if (body == null)
				throw new ArgumentNullException(nameof(body));

			return body.GetILReader().Length;
		}

		public static MethodDefinitionHandle GetAny(this PropertyAccessors accessors)
		{
			if (!accessors.Getter.IsNil)
				return accessors.Getter;
			return accessors.Setter;
		}

		public static MethodDefinitionHandle GetAny(this EventAccessors accessors)
		{
			if (!accessors.Adder.IsNil)
				return accessors.Adder;
			if (!accessors.Remover.IsNil)
				return accessors.Remover;
			return accessors.Raiser;
		}

		public static TypeDefinitionHandle GetDeclaringType(this EntityHandle entity, MetadataReader metadata)
		{
			switch (entity.Kind) {
				case HandleKind.TypeDefinition:
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)entity);
					return td.GetDeclaringType();
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)entity);
					return fd.GetDeclaringType();
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)entity);
					return md.GetDeclaringType();
				case HandleKind.EventDefinition:
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)entity);
					return metadata.GetMethodDefinition(ed.GetAccessors().GetAny()).GetDeclaringType();
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)entity);
					return metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static TypeReferenceHandle GetDeclaringType(this TypeReference tr)
		{
			switch (tr.ResolutionScope.Kind) {
				case HandleKind.TypeReference:
					return (TypeReferenceHandle)tr.ResolutionScope;
				default:
					return default(TypeReferenceHandle);
			}
		}

		public static FullTypeName GetFullTypeName(this EntityHandle handle, MetadataReader reader)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return ((TypeDefinitionHandle)handle).GetFullTypeName(reader);
				case HandleKind.TypeReference:
					return ((TypeReferenceHandle)handle).GetFullTypeName(reader);
				case HandleKind.TypeSpecification:
					return ((TypeSpecificationHandle)handle).GetFullTypeName(reader);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static bool IsKnownType(this EntityHandle handle, MetadataReader reader, KnownTypeCode knownType)
		{
			return GetFullTypeName(handle, reader) == KnownTypeReference.Get(knownType).TypeName;
		}
		
		internal static bool IsKnownType(this EntityHandle handle, MetadataReader reader, KnownAttribute knownType)
		{
			return GetFullTypeName(handle, reader) == knownType.GetTypeName();
		}

		public static FullTypeName GetFullTypeName(this TypeSpecificationHandle handle, MetadataReader reader)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			var ts = reader.GetTypeSpecification(handle);
			return ts.DecodeSignature(new Metadata.FullTypeNameSignatureDecoder(reader), default(Unit));
		}

		public static FullTypeName GetFullTypeName(this TypeReferenceHandle handle, MetadataReader reader)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			var tr = reader.GetTypeReference(handle);
			string name;
			try {
				name = reader.GetString(tr.Name);
			} catch (BadImageFormatException) {
				name = $"TR{reader.GetToken(handle):x8}";
			}
			name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(name, out var typeParameterCount);
			TypeReferenceHandle declaringTypeHandle;
			try {
				declaringTypeHandle = tr.GetDeclaringType();
			} catch (BadImageFormatException) {
				declaringTypeHandle = default;
			}
			if (declaringTypeHandle.IsNil) {
				string ns;
				try {
					ns = tr.Namespace.IsNil ? "" : reader.GetString(tr.Namespace);
				} catch (BadImageFormatException) {
					ns = "";
				}
				return new FullTypeName(new TopLevelTypeName(ns, name, typeParameterCount));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader).NestedType(name, typeParameterCount);
			}
		}

		public static FullTypeName GetFullTypeName(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			return reader.GetTypeDefinition(handle).GetFullTypeName(reader);
		}

		public static FullTypeName GetFullTypeName(this TypeDefinition td, MetadataReader reader)
		{
			TypeDefinitionHandle declaringTypeHandle;
			string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(reader.GetString(td.Name), out var typeParameterCount);
			if ((declaringTypeHandle = td.GetDeclaringType()).IsNil) {
				string @namespace = td.Namespace.IsNil ? "" : reader.GetString(td.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, name, typeParameterCount));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader).NestedType(name, typeParameterCount);
			}
		}

		public static FullTypeName GetFullTypeName(this ExportedType type, MetadataReader metadata)
		{
			string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(type.Name), out int typeParameterCount);
			if (type.Implementation.Kind == HandleKind.ExportedType) {
				var outerType = metadata.GetExportedType((ExportedTypeHandle)type.Implementation);
				return outerType.GetFullTypeName(metadata).NestedType(name, typeParameterCount);
			} else {
				string ns = type.Namespace.IsNil ? "" : metadata.GetString(type.Namespace);
				return new TopLevelTypeName(ns, name, typeParameterCount);
			}
		}

		public static bool IsAnonymousType(this TypeDefinition type, MetadataReader metadata)
		{
			string name = metadata.GetString(type.Name);
			if (type.Namespace.IsNil && type.HasGeneratedName(metadata) && (name.Contains("AnonType") || name.Contains("AnonymousType"))) {
				return type.IsCompilerGenerated(metadata);
			}
			return false;
		}

		#region HasGeneratedName

		public static bool IsGeneratedName(this StringHandle handle, MetadataReader metadata)
		{
			return !handle.IsNil && metadata.GetString(handle).StartsWith("<", StringComparison.Ordinal);
		}

		public static bool HasGeneratedName(this MethodDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetMethodDefinition(handle).Name.IsGeneratedName(metadata);
		}

		public static bool HasGeneratedName(this TypeDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetTypeDefinition(handle).Name.IsGeneratedName(metadata);
		}

		public static bool HasGeneratedName(this TypeDefinition type, MetadataReader metadata)
		{
			return type.Name.IsGeneratedName(metadata);
		}

		public static bool HasGeneratedName(this FieldDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetFieldDefinition(handle).Name.IsGeneratedName(metadata);
		}

		#endregion

		#region IsCompilerGenerated

		public static bool IsCompilerGenerated(this MethodDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetMethodDefinition(handle).IsCompilerGenerated(metadata);
		}

		public static bool IsCompilerGeneratedOrIsInCompilerGeneratedClass(this MethodDefinitionHandle handle, MetadataReader metadata)
		{
			MethodDefinition method = metadata.GetMethodDefinition(handle);
			if (method.IsCompilerGenerated(metadata))
				return true;
			TypeDefinitionHandle declaringTypeHandle = method.GetDeclaringType();
			if (!declaringTypeHandle.IsNil && declaringTypeHandle.IsCompilerGenerated(metadata))
				return true;
			return false;
		}

		public static bool IsCompilerGeneratedOrIsInCompilerGeneratedClass(this TypeDefinitionHandle handle, MetadataReader metadata)
		{
			TypeDefinition type = metadata.GetTypeDefinition(handle);
			if (type.IsCompilerGenerated(metadata))
				return true;
			TypeDefinitionHandle declaringTypeHandle = type.GetDeclaringType();
			if (!declaringTypeHandle.IsNil && declaringTypeHandle.IsCompilerGenerated(metadata))
				return true;
			return false;
		}

		public static bool IsCompilerGenerated(this MethodDefinition method, MetadataReader metadata)
		{
			return method.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.CompilerGenerated);
		}

		public static bool IsCompilerGenerated(this FieldDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetFieldDefinition(handle).IsCompilerGenerated(metadata);
		}

		public static bool IsCompilerGenerated(this FieldDefinition field, MetadataReader metadata)
		{
			return field.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.CompilerGenerated);
		}

		public static bool IsCompilerGenerated(this TypeDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetTypeDefinition(handle).IsCompilerGenerated(metadata);
		}

		public static bool IsCompilerGenerated(this TypeDefinition type, MetadataReader metadata)
		{
			return type.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.CompilerGenerated);
		}

		#endregion

		#region Attribute extensions
		/// <summary>
		/// Gets the type of the attribute.
		/// </summary>
		public static EntityHandle GetAttributeType(this SRM.CustomAttribute attribute, MetadataReader reader)
		{
			switch (attribute.Constructor.Kind) {
				case HandleKind.MethodDefinition:
					var md = reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
					return md.GetDeclaringType();
				case HandleKind.MemberReference:
					var mr = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
					return mr.Parent;
				default:
					throw new BadImageFormatException("Unexpected token kind for attribute constructor: " + attribute.Constructor.Kind);
			}
		}
		
		public static bool HasKnownAttribute(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata, KnownAttribute type)
		{
			foreach (var handle in customAttributes) {
				var customAttribute = metadata.GetCustomAttribute(handle);
				if (customAttribute.IsKnownAttribute(metadata, type))
					return true;
			}
			return false;
		}
		
		internal static bool IsKnownAttribute(this SRM.CustomAttribute attr, MetadataReader metadata, KnownAttribute attrType)
		{
			return attr.GetAttributeType(metadata).IsKnownType(metadata, attrType);
		}

		public static Nullability? GetNullableContext(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata)
		{
			foreach (var handle in customAttributes) {
				var customAttribute = metadata.GetCustomAttribute(handle);
				if (customAttribute.IsKnownAttribute(metadata, KnownAttribute.NullableContext)) {
					// Decode 
					CustomAttributeValue<IType> value;
					try {
						value = customAttribute.DecodeValue(Metadata.MetadataExtensions.MinimalAttributeTypeProvider);
					} catch (BadImageFormatException) {
						continue;
					} catch (Metadata.EnumUnderlyingTypeResolveException) {
						continue;
					}
					if (value.FixedArguments.Length == 1 && value.FixedArguments[0].Value is byte b && b <= 2) {
						return (Nullability)b;
					}
				}
			}
			return null;
		}
		#endregion

		public static unsafe SRM.BlobReader GetInitialValue(this FieldDefinition field, PEReader pefile, ICompilation typeSystem)
		{
			if (!field.HasFlag(FieldAttributes.HasFieldRVA))
				return default;
			int rva = field.GetRelativeVirtualAddress();
			if (rva == 0)
				return default;
			int size = field.DecodeSignature(new FieldValueSizeDecoder(typeSystem), default);
			var sectionData = pefile.GetSectionData(rva);
			if (sectionData.Length == 0 && size != 0)
				throw new BadImageFormatException($"Field data (rva=0x{rva:x}) could not be found in any section!");
			if (size < 0 || size > sectionData.Length)
				throw new BadImageFormatException($"Invalid size {size} for field data!");
			return sectionData.GetReader(0, size);
		}

		sealed class FieldValueSizeDecoder : ISignatureTypeProvider<int, GenericContext>
		{
			readonly MetadataModule module;
			readonly int pointerSize;

			public FieldValueSizeDecoder(ICompilation typeSystem = null)
			{
				this.module = (MetadataModule)typeSystem?.MainModule;
				if (module == null)
					this.pointerSize = IntPtr.Size;
				else
					this.pointerSize = module.PEFile.Reader.PEHeaders.PEHeader.Magic == PEMagic.PE32 ? 4 : 8;
			}

			public int GetArrayType(int elementType, ArrayShape shape) => GetPrimitiveType(PrimitiveTypeCode.Object);
			public int GetSZArrayType(int elementType) => GetPrimitiveType(PrimitiveTypeCode.Object);
			public int GetByReferenceType(int elementType) => pointerSize;
			public int GetFunctionPointerType(MethodSignature<int> signature) => pointerSize;
			public int GetGenericInstantiation(int genericType, ImmutableArray<int> typeArguments) => genericType;
			public int GetGenericMethodParameter(GenericContext genericContext, int index) => 0;
			public int GetGenericTypeParameter(GenericContext genericContext, int index) => 0;
			public int GetModifiedType(int modifier, int unmodifiedType, bool isRequired) => unmodifiedType;
			public int GetPinnedType(int elementType) => elementType;
			public int GetPointerType(int elementType) => pointerSize;

			public int GetPrimitiveType(PrimitiveTypeCode typeCode) 
			{
				switch (typeCode) {
					case PrimitiveTypeCode.Boolean:
					case PrimitiveTypeCode.Byte:
					case PrimitiveTypeCode.SByte:
						return 1;
					case PrimitiveTypeCode.Char:
					case PrimitiveTypeCode.Int16:
					case PrimitiveTypeCode.UInt16:
						return 2;
					case PrimitiveTypeCode.Int32:
					case PrimitiveTypeCode.UInt32:
					case PrimitiveTypeCode.Single:
						return 4;
					case PrimitiveTypeCode.Int64:
					case PrimitiveTypeCode.UInt64:
					case PrimitiveTypeCode.Double:
						return 8;
					case PrimitiveTypeCode.IntPtr:
					case PrimitiveTypeCode.UIntPtr:
						return pointerSize;
					default:
						return 0;
				}
			}

			public int GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				var td = reader.GetTypeDefinition(handle);
				return td.GetLayout().Size;
			}

			public int GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				var typeDef = module?.ResolveType(handle, new GenericContext()).GetDefinition();
				if (typeDef == null || typeDef.MetadataToken.IsNil)
					return 0;
				reader = typeDef.ParentModule.PEFile.Metadata;
				var td = reader.GetTypeDefinition((TypeDefinitionHandle)typeDef.MetadataToken);
				return td.GetLayout().Size;
			}

			public int GetTypeFromSpecification(MetadataReader reader, GenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			{
				return reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
			}
		}

		public static EntityHandle GetBaseTypeOrNil(this TypeDefinition definition)
		{
			try {
				return definition.BaseType;
			} catch (BadImageFormatException) {
				return default;
			}
		}
	}
}
