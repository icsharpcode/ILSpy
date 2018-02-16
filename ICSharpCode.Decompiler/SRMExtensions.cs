using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
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
				foreach (var attribute in methodDefinition.GetCustomAttributes()) {
					string typeName = reader.GetCustomAttribute(attribute).GetAttributeType(reader).GetFullTypeName(reader).ToString();
					if (typeName == "System.Runtime.CompilerServices.ExtensionAttribute")
						return true;
				}
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

		public static FullTypeName GetFullTypeName(this TypeSpecificationHandle handle, MetadataReader reader, bool omitGenericParamCount = false)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			var ts = reader.GetTypeSpecification(handle);
			return ts.DecodeSignature(new Metadata.FullTypeNameSignatureDecoder(reader), default(Unit));
		}

		public static FullTypeName GetFullTypeName(this TypeReferenceHandle handle, MetadataReader reader, bool omitGenericParamCount = false)
		{
			if (handle.IsNil)
				throw new ArgumentNullException(nameof(handle));
			var tr = reader.GetTypeReference(handle);
			TypeReferenceHandle declaringTypeHandle;
			if ((declaringTypeHandle = tr.GetDeclaringType()).IsNil) {
				string @namespace = tr.Namespace.IsNil ? "" : reader.GetString(tr.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, reader.GetString(tr.Name)));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader, omitGenericParamCount).NestedType(reader.GetString(tr.Name), 0);
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
			if ((declaringTypeHandle = td.GetDeclaringType()).IsNil) {
				string @namespace = td.Namespace.IsNil ? "" : reader.GetString(td.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, reader.GetString(td.Name)));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader).NestedType(reader.GetString(td.Name), 0);
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
	}
}
