using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.TypeSystem
{
	static class MetadataExtensions
	{
		public static string GetFullAssemblyName(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = "null";
			if (!asm.PublicKey.IsNil) {
				SHA1 sha1 = SHA1.Create();
				var publicKeyTokenBytes = sha1.ComputeHash(reader.GetBlobBytes(asm.PublicKey)).Skip(12).ToArray();
				publicKey = publicKeyTokenBytes.ToHexString();
			}
			return $"{reader.GetString(asm.Name)}, Version={asm.Version}, Culture={reader.GetString(asm.Culture)}, PublicKeyToken={publicKey}";
		}

		static string ToHexString(this byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		/// <summary>
		/// Gets the type of the attribute.
		/// </summary>
		/// <returns>Either <see cref="TypeDefinitionHandle"/>, <see cref="TypeReferenceHandle"/>,<see cref="TypeSpecificationHandle".</returns>
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

		public static FullTypeName GetFullTypeName(this EntityHandle handle, MetadataReader reader)
		{
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return ((TypeDefinitionHandle)handle).GetFullTypeName(reader);
				case HandleKind.TypeReference:
					return ((TypeReferenceHandle)handle).GetFullTypeName(reader);
				default:
					throw new NotSupportedException();
			}
		}

		public static FullTypeName GetFullTypeName(this TypeReferenceHandle handle, MetadataReader reader)
		{
			var tr = reader.GetTypeReference(handle);
			TypeReferenceHandle declaringTypeHandle;
			if ((declaringTypeHandle = tr.GetDeclaringType()).IsNil) {
				string @namespace = tr.Namespace.IsNil ? "" : reader.GetString(tr.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, reader.GetString(tr.Name)));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader).NestedType(reader.GetString(tr.Name), 0);
			}
		}

		public static FullTypeName GetFullTypeName(this TypeDefinitionHandle handle, MetadataReader reader)
		{
			return reader.GetTypeDefinition(handle).GetFullTypeName(reader);
		}

		public static FullTypeName GetFullTypeName(this TypeDefinition td, MetadataReader reader)
		{
			TypeDefinitionHandle declaringTypeHandle;
			if ((declaringTypeHandle = td.GetDeclaringType()).IsNil) {
				string @namespace = td.Namespace.IsNil ? "" : reader.GetString(td.Namespace);
				return new FullTypeName(new TopLevelTypeName(@namespace, reader.GetString(td.Name), td.GetGenericParameters().Count));
			} else {
				return declaringTypeHandle.GetFullTypeName(reader).NestedType(reader.GetString(td.Name), td.GetGenericParameters().Count);
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

		public static AssemblyReferenceHandle GetDeclaringAssembly(this TypeReferenceHandle handle, MetadataReader reader)
		{
			var tr = reader.GetTypeReference(handle);
			switch (tr.ResolutionScope.Kind) {
				case HandleKind.TypeReference:
					return ((TypeReferenceHandle)tr.ResolutionScope).GetDeclaringAssembly(reader);
				case HandleKind.AssemblyReference:
					return (AssemblyReferenceHandle)tr.ResolutionScope;
				default:
					return default(AssemblyReferenceHandle);
			}
		}

		public unsafe static MethodSemanticsAttributes GetMethodSemanticsAttributes(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			int rowCount = reader.GetTableRowCount(TableIndex.MethodSemantics);
			int token = reader.GetToken(handle);
			for (int row = rowCount - 1; row >= 0; row--) {
				byte* ptr = startPointer + offset + rowSize * row;
				ushort methodToken = *(ptr + 2);
				if (token == methodToken) {
					return (MethodSemanticsAttributes)(*ptr);
				}
			}
			return 0;
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

		public static bool IsConstructor(this MethodDefinition methodDefinition, MetadataReader reader)
		{
			string name = reader.GetString(methodDefinition.Name);
			return (methodDefinition.Attributes & (MethodAttributes.RTSpecialName | MethodAttributes.SpecialName)) != 0
				&& (name == ".cctor" || name == ".ctor");
		}

		public static bool IsEnum(this TypeDefinition typeDefinition, MetadataReader reader)
		{
			if (typeDefinition.BaseType.IsNil)
				return false;
			return typeDefinition.BaseType.GetFullTypeName(reader).ToString() == "System.Enum";
		}

		public static bool HasOverrides(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			for (int row = 1; row <= reader.GetTableRowCount(TableIndex.MethodImpl); row++) {
				var impl = reader.GetMethodImplementation(MetadataTokens.MethodImplementationHandle(row));
				if (impl.MethodBody == handle) return true;
			}
			return false;
		}

		public static bool HasParameters(this PropertyDefinitionHandle handle, MetadataReader reader)
		{
			var a = reader.GetPropertyDefinition(handle).GetAccessors();
			if (!a.Getter.IsNil) {
				var m = reader.GetMethodDefinition(a.Getter);
				return m.GetParameters().Count > 0;
			}
			if (!a.Setter.IsNil) {
				var m = reader.GetMethodDefinition(a.Setter);
				return m.GetParameters().Count > 1;
			}
			return false;
		}

		public static PrimitiveTypeCode ToPrimtiveTypeCode(this KnownTypeCode typeCode)
		{
			switch (typeCode) {
				case KnownTypeCode.Object:
					return PrimitiveTypeCode.Object;
				case KnownTypeCode.Boolean:
					return PrimitiveTypeCode.Boolean;
				case KnownTypeCode.Char:
					return PrimitiveTypeCode.Char;
				case KnownTypeCode.SByte:
					return PrimitiveTypeCode.SByte;
				case KnownTypeCode.Byte:
					return PrimitiveTypeCode.Byte;
				case KnownTypeCode.Int16:
					return PrimitiveTypeCode.Int16;
				case KnownTypeCode.UInt16:
					return PrimitiveTypeCode.UInt16;
				case KnownTypeCode.Int32:
					return PrimitiveTypeCode.Int32;
				case KnownTypeCode.UInt32:
					return PrimitiveTypeCode.UInt32;
				case KnownTypeCode.Int64:
					return PrimitiveTypeCode.Int64;
				case KnownTypeCode.UInt64:
					return PrimitiveTypeCode.UInt64;
				case KnownTypeCode.Single:
					return PrimitiveTypeCode.Single;
				case KnownTypeCode.Double:
					return PrimitiveTypeCode.Double;
				case KnownTypeCode.String:
					return PrimitiveTypeCode.String;
				case KnownTypeCode.Void:
					return PrimitiveTypeCode.Void;
				case KnownTypeCode.TypedReference:
					return PrimitiveTypeCode.TypedReference;
				case KnownTypeCode.IntPtr:
					return PrimitiveTypeCode.IntPtr;
				case KnownTypeCode.UIntPtr:
					return PrimitiveTypeCode.UIntPtr;
				default:
					throw new NotSupportedException();
			}
		}

		public static KnownTypeCode ToKnownTypeCode(this PrimitiveTypeCode typeCode)
		{
			switch (typeCode) {
				case PrimitiveTypeCode.Boolean:
					return KnownTypeCode.Boolean;
				case PrimitiveTypeCode.Byte:
					return KnownTypeCode.Byte;
				case PrimitiveTypeCode.SByte:
					return KnownTypeCode.SByte;
				case PrimitiveTypeCode.Char:
					return KnownTypeCode.Byte;
				case PrimitiveTypeCode.Int16:
					return KnownTypeCode.Byte;
				case PrimitiveTypeCode.UInt16:
					return KnownTypeCode.Byte;
				case PrimitiveTypeCode.Int32:
					return KnownTypeCode.Int32;
				case PrimitiveTypeCode.UInt32:
					return KnownTypeCode.UInt32;
				case PrimitiveTypeCode.Int64:
					return KnownTypeCode.Int64;
				case PrimitiveTypeCode.UInt64:
					return KnownTypeCode.UInt64;
				case PrimitiveTypeCode.Single:
					return KnownTypeCode.Single;
				case PrimitiveTypeCode.Double:
					return KnownTypeCode.Double;
				case PrimitiveTypeCode.IntPtr:
					return KnownTypeCode.IntPtr;
				case PrimitiveTypeCode.UIntPtr:
					return KnownTypeCode.UIntPtr;
				case PrimitiveTypeCode.Object:
					return KnownTypeCode.Object;
				case PrimitiveTypeCode.String:
					return KnownTypeCode.String;
				case PrimitiveTypeCode.TypedReference:
					return KnownTypeCode.TypedReference;
				case PrimitiveTypeCode.Void:
					return KnownTypeCode.Void;
				default:
					return KnownTypeCode.None;
			}
		}
	}
}
