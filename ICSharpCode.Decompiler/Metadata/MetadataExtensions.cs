using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using SRM = System.Reflection.Metadata;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class MetadataExtensions
	{
		#region Resolver
		public static TypeDefinition ResolveAsType(this EntityHandle entity, PEFile module)
		{
			return new Entity(module, entity).ResolveAsType();
		}

		public static FieldDefinition ResolveAsField(this EntityHandle entity, PEFile module)
		{
			return new Entity(module, entity).ResolveAsField();
		}

		public static MethodDefinition ResolveAsMethod(this EntityHandle entity, PEFile module)
		{
			return new Entity(module, entity).ResolveAsMethod();
		}
		#endregion

		public static MethodDefinition AsMethod(this IMetadataEntity entity)
		{
			if (entity is MethodDefinition method)
				return method;
			if (entity is Entity e)
				return e;
			throw new NotSupportedException();
		}

		public static bool IsNil(this IAssemblyReference reference)
		{
			return reference == null || (reference is Metadata.AssemblyReference ar && ar.IsNil);
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
			return $"{reader.GetString(asm.Name)}, Version={asm.Version}, Culture={(asm.Culture.IsNil ? "neutral" : reader.GetString(asm.Culture))}, PublicKeyToken={publicKey}";
		}

		public static string GetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader)
		{
			string publicKey = "null";
			if (!reference.PublicKeyOrToken.IsNil) {
				byte[] publicKeyTokenBytes = reader.GetBlobBytes(reference.PublicKeyOrToken);
				if ((reference.Flags & AssemblyFlags.PublicKey) != 0) {
					SHA1 sha1 = SHA1.Create();
					publicKeyTokenBytes = sha1.ComputeHash(publicKeyTokenBytes).Skip(12).ToArray();
				}
				publicKey = publicKeyTokenBytes.ToHexString();
			}
			string properties = "";
			if ((reference.Flags & AssemblyFlags.Retargetable) != 0)
				properties = ", Retargetable=true";
			return $"{reader.GetString(reference.Name)}, Version={reference.Version}, Culture={(reference.Culture.IsNil ? "neutral" : reader.GetString(reference.Culture))}, PublicKeyToken={publicKey}{properties}";
		}

		static string ToHexString(this byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		public static IEnumerable<TypeDefinitionHandle> GetTopLevelTypeDefinitions(this MetadataReader reader)
		{
			unsafe HashSet<uint> GetNestedTypes()
			{
				byte* startPointer = reader.MetadataPointer;
				int offset = reader.GetTableMetadataOffset(TableIndex.NestedClass);
				int rowSize = reader.GetTableRowSize(TableIndex.NestedClass);
				int rowCount = reader.GetTableRowCount(TableIndex.NestedClass);
				var typeDefSize = reader.GetReferenceSize(TableIndex.TypeDef);

				var set = new HashSet<uint>();

				for (int row = 0; row < rowCount; row++) {
					byte* ptr = startPointer + offset + rowSize * row;
					uint currentTypeRow = typeDefSize == 2 ? *(ushort*)ptr : *(uint*)ptr;
					set.Add(currentTypeRow);
				}

				return set;
			}

			HashSet<uint> nestedTypes = GetNestedTypes();

			foreach (var handle in reader.TypeDefinitions) {
				if (!nestedTypes.Contains((uint)reader.GetRowNumber(handle)))
					yield return handle;
			}
		}

		public static string ToILNameString(this FullTypeName typeName)
		{
			string escapedName;
			if (typeName.IsNested) {
				escapedName = Disassembler.DisassemblerHelpers.Escape(typeName.Name);
				if (typeName.TypeParameterCount > 0)
					escapedName += "`" + typeName.TypeParameterCount;
				return $"{typeName.GetDeclaringType().ToILNameString()}/{escapedName}";
			} else if (!string.IsNullOrEmpty(typeName.TopLevelTypeName.Namespace)) {
				escapedName = Disassembler.DisassemblerHelpers.Escape($"{typeName.TopLevelTypeName.Namespace}.{typeName.Name}");
				if (typeName.TypeParameterCount > 0)
					escapedName += "`" + typeName.TypeParameterCount;
			} else {
				escapedName = Disassembler.DisassemblerHelpers.Escape(typeName.Name);
			}
			return escapedName;
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
					return default;
			}
		}

		public static bool IsConstructor(this SRM.MethodDefinition methodDefinition, MetadataReader reader)
		{
			string name = reader.GetString(methodDefinition.Name);
			return (methodDefinition.Attributes & (MethodAttributes.RTSpecialName | MethodAttributes.SpecialName)) != 0
				&& (name == ".cctor" || name == ".ctor");
		}

		public static bool HasMatchingDefaultMemberAttribute(this PropertyDefinitionHandle handle, PEFile module, out CustomAttributeHandle defaultMemberAttribute)
		{
			defaultMemberAttribute = default(CustomAttributeHandle);
			var metadata = module.GetMetadataReader();
			var propertyDefinition = metadata.GetPropertyDefinition(handle);
			var accessorHandle = propertyDefinition.GetAccessors().GetAny();
			var accessor = metadata.GetMethodDefinition(accessorHandle);
			if (accessor.GetParameters().Count > 0) {
				var basePropDef = propertyDefinition;
				var firstOverrideHandle = accessorHandle.GetMethodImplementations(metadata).FirstOrDefault();
				if (!firstOverrideHandle.IsNil) {
					// if the property is explicitly implementing an interface, look up the property in the interface:
					var firstOverride = metadata.GetMethodImplementation(firstOverrideHandle);
					var baseAccessor = new Metadata.Entity(module, firstOverride.MethodDeclaration).ResolveAsMethod();
					if (!baseAccessor.IsNil) {
						var declaringType = metadata.GetTypeDefinition(metadata.GetMethodDefinition(baseAccessor.Handle).GetDeclaringType());
						foreach (var basePropHandle in declaringType.GetProperties()) {
							var baseProp = metadata.GetPropertyDefinition(basePropHandle);
							var accessors = baseProp.GetAccessors();
							if (accessors.Getter == baseAccessor.Handle || accessors.Setter == baseAccessor.Handle) {
								basePropDef = baseProp;
								break;
							}
						}
					} else
						return false;
				}
				var defaultMemberName = accessor.GetDeclaringType().GetDefaultMemberName(metadata, out var attr);
				if (defaultMemberName == metadata.GetString(basePropDef.Name)) {
					defaultMemberAttribute = attr;
					return true;
				}
			}
			return false;
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
					return KnownTypeCode.Char;
				case PrimitiveTypeCode.Int16:
					return KnownTypeCode.Int16;
				case PrimitiveTypeCode.UInt16:
					return KnownTypeCode.UInt16;
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

		public unsafe static ParameterHandle At(this ParameterHandleCollection collection, MetadataReader metadata, int index)
		{
			if (metadata.GetTableRowCount(TableIndex.ParamPtr) > 0) {
				int rowSize = metadata.GetTableRowSize(TableIndex.ParamPtr);
				int paramRefSize = (metadata.GetReferenceSize(TableIndex.ParamPtr) > 2) ? 4 : metadata.GetReferenceSize(TableIndex.Param);
				int offset = metadata.GetTableMetadataOffset(TableIndex.ParamPtr) + index * rowSize;
				byte* ptr = metadata.MetadataPointer + offset;
				if (paramRefSize == 2)
					return MetadataTokens.ParameterHandle(*(ushort*)ptr);
				return MetadataTokens.ParameterHandle((int)*(uint*)ptr);
			}
			return MetadataTokens.ParameterHandle((index + 1) & 0xFFFFFF);
		}
	}
}
