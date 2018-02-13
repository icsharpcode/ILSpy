using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler
{
	public static class MetadataExtensions
	{
		public static Dom.ITypeReference CoerceTypeReference(this EntityHandle handle, Dom.PEFile module)
		{
			if (handle.IsNil)
				return null;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return new Dom.TypeDefinition(module, (TypeDefinitionHandle)handle);
				case HandleKind.TypeReference:
					return new Dom.TypeReference(module, (TypeReferenceHandle)handle);
				case HandleKind.TypeSpecification:
					return new Dom.TypeSpecification(module, (TypeSpecificationHandle)handle);
				default:
					throw new ArgumentException("must be either TypeDef, TypeRef or TypeSpec!", nameof(handle));
			}
		}

		public static Dom.IMemberReference CoerceMemberReference(this EntityHandle handle, Dom.PEFile module)
		{
			if (handle.IsNil)
				return null;
			switch (handle.Kind) {
				case HandleKind.MemberReference:
					return new Dom.MemberReference(module, (MemberReferenceHandle)handle);
				case HandleKind.MethodDefinition:
					return new Dom.MethodDefinition(module, (MethodDefinitionHandle)handle);
				default:
					throw new ArgumentException("must be either MethodDef or MemberRef!", nameof(handle));
			}
		}

		public static bool IsNil(this Dom.IAssemblyReference reference)
		{
			return reference == null || (reference is Dom.AssemblyReference ar && ar.IsNil);
		}

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

		public static string GetFullAssemblyName(this AssemblyReference reference, MetadataReader reader)
		{
			string publicKey = "null";
			if (!reference.PublicKeyOrToken.IsNil && (reference.Flags & AssemblyFlags.PublicKey) != 0) {
				SHA1 sha1 = SHA1.Create();
				var publicKeyTokenBytes = sha1.ComputeHash(reader.GetBlobBytes(reference.PublicKeyOrToken)).Skip(12).ToArray();
				publicKey = publicKeyTokenBytes.ToHexString();
			}
			string properties = "";
			if ((reference.Flags & AssemblyFlags.Retargetable) != 0)
				properties = ", Retargetable=true";
			return $"{reader.GetString(reference.Name)}, Version={reference.Version}, Culture={reader.GetString(reference.Culture)}, PublicKeyToken={publicKey}{properties}";
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
		public static Dom.ITypeReference GetAttributeType(this CustomAttribute attribute, Dom.PEFile module)
		{
			var reader = module.GetMetadataReader();
			switch (attribute.Constructor.Kind) {
				case HandleKind.MethodDefinition:
					var md = reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor);
					return new Dom.TypeDefinition(module, md.GetDeclaringType());
				case HandleKind.MemberReference:
					var mr = reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
					return mr.Parent.CoerceTypeReference(module);
				default:
					throw new NotSupportedException();
			}
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

		public static IEnumerable<TypeDefinitionHandle> GetTopLevelTypeDefinitions(this MetadataReader reader)
		{
			var queue = new Queue<NamespaceDefinition>();
			queue.Enqueue(reader.GetNamespaceDefinitionRoot());
			while (queue.Count > 0) {
				var ns = queue.Dequeue();
				foreach (var td in ns.TypeDefinitions)
					yield return td;
				foreach (var nestedNS in ns.NamespaceDefinitions)
					queue.Enqueue(reader.GetNamespaceDefinition(nestedNS));
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
			return ts.DecodeSignature(new Dom.FullTypeNameSignatureDecoder(reader), default(Unit));
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

		public static string ToILNameString(this FullTypeName typeName)
		{
			var escapedName = Disassembler.DisassemblerHelpers.Escape(typeName.Name);
			if (typeName.IsNested) {
				return $"{typeName.GetDeclaringType().ToILNameString()}/{escapedName}";
			} else if (!string.IsNullOrEmpty(typeName.TopLevelTypeName.Namespace)) {
				return $"{typeName.TopLevelTypeName.Namespace}.{escapedName}";
			} else {
				return $"{escapedName}";
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

		public static string GetDefaultMemberName(this TypeDefinitionHandle type, MetadataReader reader)
		{
			return type.GetDefaultMemberName(reader, out var attr);
		}

		static readonly ITypeResolveContext minimalCorlibContext = new SimpleTypeResolveContext(MinimalCorlib.Instance.CreateCompilation());

		public static string GetDefaultMemberName(this TypeDefinitionHandle type, MetadataReader reader, out CustomAttributeHandle defaultMemberAttribute)
		{
			var td = reader.GetTypeDefinition(type);

			foreach (var h in td.GetCustomAttributes()) {
				var ca = reader.GetCustomAttribute(h);
				if (ca.GetAttributeType(reader).ToString() == "System.Reflection.DefaultMemberAttribute") {
					var decodedValues = ca.DecodeValue(new TypeSystemAttributeTypeProvider(minimalCorlibContext));
					if (decodedValues.FixedArguments.Length == 1 && decodedValues.FixedArguments[0].Value is string value) {
						defaultMemberAttribute = h;
						return value;
					}
				}
			}

			defaultMemberAttribute = default(CustomAttributeHandle);
			return null;
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

		public static bool IsSmallReference(this MetadataReader reader, TableIndex table)
		{
			// TODO detect whether #JTD is present (EnC)
			return reader.GetTableRowCount(table) <= ushort.MaxValue;
		}

		public static int GetReferenceSize(this MetadataReader reader, TableIndex table)
		{
			return IsSmallReference(reader, table) ? 2 : 4;
		}

		public static unsafe (int startRow, int endRow) BinarySearchRange(this MetadataReader reader, TableIndex table, int valueOffset, uint referenceValue, bool small)
		{
			int offset = reader.GetTableMetadataOffset(table);
			int rowSize = reader.GetTableRowSize(table);
			int rowCount = reader.GetTableRowCount(table);
			int tableLength = rowSize * rowCount;
			byte* startPointer = reader.MetadataPointer;

			int result = BinarySearch();
			if (result == -1)
				return (-1, -1);

			int start = result;

			while (start > 0 && GetValue(start - 1) == referenceValue)
				start--;

			int end = result;

			while (end + 1 < tableLength && GetValue(end + 1) == referenceValue)
				end++;

			return (start, end);

			uint GetValue(int row)
			{
				if (small)
					return *(ushort*)(startPointer + offset + row * rowSize + valueOffset);
				else
					return *(uint*)(startPointer + offset + row * rowSize + valueOffset);
			}

			int BinarySearch()
			{
				int startRow = 0;
				int endRow = rowCount - 1;
				while (startRow <= endRow) {
					int row = (startRow + endRow) / 2;
					uint currentValue = GetValue(row);
					if (referenceValue > currentValue) {
						startRow = row + 1;
					} else if (referenceValue < currentValue) {
						endRow = row - 1;
					} else {
						return row;
					}
				}
				return -1;
			}
		}

		public static AssemblyDefinition? GetAssemblyDefinition(this PEReader reader)
		{
			var metadata = reader.GetMetadataReader();
			if (metadata.IsAssembly)
				return metadata.GetAssemblyDefinition();
			return null;
		}
	}
}
