using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

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
			if (typeDefinition.BaseType.IsNil)
				return false;
			var baseType = typeDefinition.BaseType.GetFullTypeName(reader).ToString();
			if (baseType == "System.Enum")
				return true;
			var thisType = typeDefinition.GetFullTypeName(reader).ToString();
			return baseType == "System.ValueType" && thisType != "System.Enum";
		}

		public static bool IsEnum(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).IsEnum(reader);
		}

		public static bool IsEnum(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			if (typeDefinition.BaseType.IsNil)
				return false;
			return typeDefinition.BaseType.GetFullTypeName(reader).ToString() == "System.Enum";
		}

		public static bool IsEnum(this TypeDefinitionHandle handle, MetadataReader reader, out PrimitiveTypeCode underlyingType)
		{
			return reader.GetTypeDefinition(handle).IsEnum(reader, out underlyingType);
		}

		public static bool IsEnum(this TypeDefinition typeDefinition, MetadataReader reader, out PrimitiveTypeCode underlyingType)
		{
			underlyingType = 0;
			if (typeDefinition.BaseType.IsNil)
				return false;
			if (typeDefinition.BaseType.GetFullTypeName(reader).ToString() != "System.Enum")
				return false;
			var field = reader.GetFieldDefinition(typeDefinition.GetFields().First());
			var blob = reader.GetBlobReader(field.Signature);
			if (blob.ReadSignatureHeader().Kind != SignatureKind.Field)
				return false;
			underlyingType = (PrimitiveTypeCode)blob.ReadByte();
			return true;
		}

		public static bool IsDelegate(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).IsDelegate(reader);
		}

		public static bool IsDelegate(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			var baseType = typeDefinition.BaseType;
			return !baseType.IsNil && baseType.GetFullTypeName(reader).ToString() == typeof(MulticastDelegate).FullName;
		}

		public static bool IsExtensionMethod(this MethodDefinition methodDefinition, MetadataReader reader)
		{
			if (methodDefinition.HasFlag(MethodAttributes.Static)) {
				return methodDefinition.GetCustomAttributes().HasAttributeOfType<System.Runtime.CompilerServices.ExtensionAttribute>(reader);
			}
			return false;
		}

		public static bool HasBody(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			var methodDefinition = reader.GetMethodDefinition(handle);
			return (methodDefinition.Attributes & MethodAttributes.Abstract) == 0 &&
				(methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.InternalCall) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Native) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Unmanaged) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Runtime) == 0;
		}

		public static bool HasBody(this MethodDefinition methodDefinition)
		{
			return (methodDefinition.Attributes & MethodAttributes.Abstract) == 0 &&
				(methodDefinition.Attributes & MethodAttributes.PinvokeImpl) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.InternalCall) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Native) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Unmanaged) == 0 &&
				(methodDefinition.ImplAttributes & MethodImplAttributes.Runtime) == 0;
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
					throw new NotSupportedException();
			}
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
			string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(reader.GetString(tr.Name), out var typeParameterCount);
			TypeReferenceHandle declaringTypeHandle;
			if ((declaringTypeHandle = tr.GetDeclaringType()).IsNil) {
				string @namespace = tr.Namespace.IsNil ? "" : reader.GetString(tr.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, name, typeParameterCount));
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

		public static TType DecodeSignature<TType, TGenericContext>(this EventDefinition ev, MetadataReader reader, ISignatureTypeProvider<TType, TGenericContext> provider, TGenericContext genericContext)
		{
			switch (ev.Type.Kind) {
				case HandleKind.TypeDefinition:
					return provider.GetTypeFromDefinition(reader, (TypeDefinitionHandle)ev.Type, 0);
				case HandleKind.TypeReference:
					return provider.GetTypeFromReference(reader, (TypeReferenceHandle)ev.Type, 0);
				case HandleKind.TypeSpecification:
					return provider.GetTypeFromSpecification(reader, genericContext, (TypeSpecificationHandle)ev.Type, 0);
				default:
					throw new NotSupportedException();
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

		public static bool IsCompilerGenerated(this MethodDefinition method, MetadataReader metadata)
		{
			return method.GetCustomAttributes().HasCompilerGeneratedAttribute(metadata);
		}

		public static bool IsCompilerGenerated(this FieldDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetFieldDefinition(handle).IsCompilerGenerated(metadata);
		}

		public static bool IsCompilerGenerated(this FieldDefinition field, MetadataReader metadata)
		{
			return field.GetCustomAttributes().HasCompilerGeneratedAttribute(metadata);
		}

		public static bool IsCompilerGenerated(this TypeDefinitionHandle handle, MetadataReader metadata)
		{
			return metadata.GetTypeDefinition(handle).IsCompilerGenerated(metadata);
		}

		public static bool IsCompilerGenerated(this TypeDefinition type, MetadataReader metadata)
		{
			return type.GetCustomAttributes().HasCompilerGeneratedAttribute(metadata);
		}

		#endregion

		#region Attribute extensions
		/// <summary>
		/// Gets the type of the attribute.
		/// </summary>
		public static EntityHandle GetAttributeType(this CustomAttribute attribute, MetadataReader reader)
		{
			switch (attribute.Constructor.Kind) {
				case HandleKind.MethodDefinition:
					var md = reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
					return md.GetDeclaringType();
				case HandleKind.MemberReference:
					var mr = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
					return mr.Parent;
				default:
					throw new NotSupportedException();
			}
		}

		public static bool HasCompilerGeneratedAttribute(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata)
		{
			return customAttributes.HasAttributeOfType<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(metadata);
		}

		public static bool HasParamArrayAttribute(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata)
		{
			return customAttributes.HasAttributeOfType<System.ParamArrayAttribute>(metadata);
		}

		public static bool HasDefaultMemberAttribute(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata)
		{
			return customAttributes.HasAttributeOfType<System.Reflection.DefaultMemberAttribute>(metadata);
		}

		public static bool HasAttributeOfType(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata, Type type)
		{
			var typeName = type.FullName;
			foreach (var handle in customAttributes) {
				var customAttribute = metadata.GetCustomAttribute(handle);
				var attributeTypeName = customAttribute.GetAttributeType(metadata).GetFullTypeName(metadata).ToString();
				if (typeName == attributeTypeName)
					return true;
			}
			return false;
		}

		public static bool HasAttributeOfType<TAttribute>(this CustomAttributeHandleCollection customAttributes, MetadataReader metadata) where TAttribute : Attribute
		{
			return HasAttributeOfType(customAttributes, metadata, typeof(TAttribute));
		}
		#endregion

		public static byte[] GetInitialValue(this FieldDefinition field, PEReader pefile)
		{
			if (!field.HasFlag(FieldAttributes.HasFieldRVA) || field.GetRelativeVirtualAddress() == 0)
				return Empty<byte>.Array;
			int rva = field.GetRelativeVirtualAddress();
			int size = field.DecodeSignature(new FieldValueSizeDecoder(), default);
			var headers = pefile.PEHeaders;
			int index = headers.GetContainingSectionIndex(rva);
			var sectionHeader = headers.SectionHeaders[index];
			var sectionData = pefile.GetEntireImage();
			int totalOffset = rva + sectionHeader.PointerToRawData - sectionHeader.VirtualAddress;
			var reader = sectionData.GetReader();
			reader.Offset += totalOffset;
			int offset = field.GetOffset();
			if (offset > 0)
				reader.Offset += offset;
			return reader.ReadBytes(size);

		}

		class FieldValueSizeDecoder : ISignatureTypeProvider<int, Unit>
		{
			public int GetArrayType(int elementType, ArrayShape shape) => elementType;
			public int GetByReferenceType(int elementType) => elementType;
			public int GetFunctionPointerType(MethodSignature<int> signature) => IntPtr.Size;
			public int GetGenericInstantiation(int genericType, ImmutableArray<int> typeArguments) => throw new NotSupportedException();
			public int GetGenericMethodParameter(Unit genericContext, int index) => throw new NotSupportedException();
			public int GetGenericTypeParameter(Unit genericContext, int index) => throw new NotSupportedException();
			public int GetModifiedType(int modifier, int unmodifiedType, bool isRequired) => unmodifiedType;
			public int GetPinnedType(int elementType) => elementType;
			public int GetPointerType(int elementType) => elementType;

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
						return IntPtr.Size;
					default:
						throw new NotSupportedException();
				}
			}

			public int GetSZArrayType(int elementType) => throw new NotSupportedException();

			public int GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				var td = reader.GetTypeDefinition(handle);
				return td.GetLayout().Size;
			}

			public int GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				throw new NotImplementedException();
			}

			public int GetTypeFromSpecification(MetadataReader reader, Unit genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			{
				throw new NotImplementedException();
			}
		}
	}
}
