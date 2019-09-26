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
		static HashAlgorithm GetHashAlgorithm(this MetadataReader reader)
		{
			switch (reader.GetAssemblyDefinition().HashAlgorithm) {
				case AssemblyHashAlgorithm.None:
					// only for multi-module assemblies?
					return SHA1.Create();
				case AssemblyHashAlgorithm.MD5:
					return MD5.Create();
				case AssemblyHashAlgorithm.Sha1:
					return SHA1.Create();
				case AssemblyHashAlgorithm.Sha256:
					return SHA256.Create();
				case AssemblyHashAlgorithm.Sha384:
					return SHA384.Create();
				case AssemblyHashAlgorithm.Sha512:
					return SHA512.Create();
				default:
					return SHA1.Create(); // default?
			}
		}

		static string CalculatePublicKeyToken(BlobHandle blob, MetadataReader reader)
		{
			// Calculate public key token:
			// 1. hash the public key using the appropriate algorithm.
			byte[] publicKeyTokenBytes = reader.GetHashAlgorithm().ComputeHash(reader.GetBlobBytes(blob));
			// 2. take the last 8 bytes
			// 3. according to Cecil we need to reverse them, other sources did not mention this.
			return publicKeyTokenBytes.TakeLast(8).Reverse().ToHexString(8);
		}

		public static string GetFullAssemblyName(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = "null";
			if (!asm.PublicKey.IsNil) {
				// AssemblyFlags.PublicKey does not apply to assembly definitions
				publicKey = CalculatePublicKeyToken(asm.PublicKey, reader);
			}
			return $"{reader.GetString(asm.Name)}, " +
				$"Version={asm.Version}, " +
				$"Culture={(asm.Culture.IsNil ? "neutral" : reader.GetString(asm.Culture))}, " +
				$"PublicKeyToken={publicKey}";
		}

		public static string GetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader)
		{
			string publicKey = "null";
			if (!reference.PublicKeyOrToken.IsNil) {
				if ((reference.Flags & AssemblyFlags.PublicKey) != 0) {
					publicKey = CalculatePublicKeyToken(reference.PublicKeyOrToken, reader);
				} else {
					publicKey = reader.GetBlobBytes(reference.PublicKeyOrToken).ToHexString(8);
				}
			}
			string properties = "";
			if ((reference.Flags & AssemblyFlags.Retargetable) != 0)
				properties = ", Retargetable=true";
			return $"{reader.GetString(reference.Name)}, " +
				$"Version={reference.Version}, " +
				$"Culture={(reference.Culture.IsNil ? "neutral" : reader.GetString(reference.Culture))}, " +
				$"PublicKeyToken={publicKey}{properties}";
		}

		public static string ToHexString(this IEnumerable<byte> bytes, int estimatedLength)
		{
			StringBuilder sb = new StringBuilder(estimatedLength * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		public static IEnumerable<TypeDefinitionHandle> GetTopLevelTypeDefinitions(this MetadataReader reader)
		{
			foreach (var handle in reader.TypeDefinitions) {
				var td = reader.GetTypeDefinition(handle);
				if (td.GetDeclaringType().IsNil)
					yield return handle;
			}
		}

		public static string ToILNameString(this FullTypeName typeName, bool omitGenerics = false)
		{
			string name;
			if (typeName.IsNested) {
				name = typeName.Name;
				if (!omitGenerics) {
					int localTypeParameterCount = typeName.GetNestedTypeAdditionalTypeParameterCount(typeName.NestingLevel - 1);
					if (localTypeParameterCount > 0)
						name += "`" + localTypeParameterCount;
				}
				name = Disassembler.DisassemblerHelpers.Escape(name);
				return $"{typeName.GetDeclaringType().ToILNameString(omitGenerics)}/{name}";
			}
			if (!string.IsNullOrEmpty(typeName.TopLevelTypeName.Namespace)) {
				name = $"{typeName.TopLevelTypeName.Namespace}.{typeName.Name}";
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			} else {
				name = typeName.Name;
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			}
			return Disassembler.DisassemblerHelpers.Escape(name);
		}

		public static IModuleReference GetDeclaringModule(this TypeReferenceHandle handle, MetadataReader reader)
		{
			var tr = reader.GetTypeReference(handle);
			switch (tr.ResolutionScope.Kind) {
				case HandleKind.TypeReference:
					return ((TypeReferenceHandle)tr.ResolutionScope).GetDeclaringModule(reader);
				case HandleKind.AssemblyReference:
					var asmRef = reader.GetAssemblyReference((AssemblyReferenceHandle)tr.ResolutionScope);
					return new DefaultAssemblyReference(reader.GetString(asmRef.Name));
				case HandleKind.ModuleReference:
					var modRef = reader.GetModuleReference((ModuleReferenceHandle)tr.ResolutionScope);
					return new DefaultAssemblyReference(reader.GetString(modRef.Name));
				default:
					return DefaultAssemblyReference.CurrentAssembly;
			}
		}

		internal static readonly TypeProvider minimalCorlibTypeProvider =
			new TypeProvider(new SimpleCompilation(MinimalCorlib.Instance));

		/// <summary>
		/// An attribute type provider that can be used to decode attribute signatures
		/// that only mention built-in types.
		/// </summary>
		public static ICustomAttributeTypeProvider<IType> MinimalAttributeTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		public static ISignatureTypeProvider<IType, TypeSystem.GenericContext> MinimalSignatureTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		public static PrimitiveTypeCode ToPrimitiveTypeCode(this KnownTypeCode typeCode)
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
					throw new ArgumentOutOfRangeException();
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

		public static IEnumerable<ModuleReferenceHandle> GetModuleReferences(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.ModuleRef);
			for (int row = 1; row <= rowCount; row++) {
				yield return MetadataTokens.ModuleReferenceHandle(row);
			}
		}

		public static IEnumerable<TypeSpecificationHandle> GetTypeSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.TypeSpec);
			for (int row = 1; row <= rowCount; row++) {
				yield return MetadataTokens.TypeSpecificationHandle(row);
			}
		}

		public static IEnumerable<MethodSpecificationHandle> GetMethodSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.MethodSpec);
			for (int row = 1; row <= rowCount; row++) {
				yield return MetadataTokens.MethodSpecificationHandle(row);
			}
		}

		public static IEnumerable<(Handle Handle, MethodSemanticsAttributes Semantics, MethodDefinitionHandle Method, EntityHandle Association)> GetMethodSemantics(this MetadataReader metadata)
		{
			int offset = metadata.GetTableMetadataOffset(TableIndex.MethodSemantics);
			int rowSize = metadata.GetTableRowSize(TableIndex.MethodSemantics);
			int rowCount = metadata.GetTableRowCount(TableIndex.MethodSemantics);

			bool methodSmall = metadata.GetTableRowCount(TableIndex.MethodDef) <= ushort.MaxValue;
			bool assocSmall = metadata.GetTableRowCount(TableIndex.Property) <= ushort.MaxValue && metadata.GetTableRowCount(TableIndex.Event) <= ushort.MaxValue;
			int assocOffset = (methodSmall ? 2 : 4) + 2;
			for (int row = 0; row < rowCount; row++) {
				yield return Read(row);
			}

			unsafe (Handle Handle, MethodSemanticsAttributes Semantics, MethodDefinitionHandle Method, EntityHandle Association) Read(int row)
			{
				byte* ptr = metadata.MetadataPointer + offset + rowSize * row;
				int methodDef = methodSmall ? *(ushort*)(ptr + 2) : (int)*(uint*)(ptr + 2);
				int assocDef = assocSmall ? *(ushort*)(ptr + assocOffset) : (int)*(uint*)(ptr + assocOffset);
				EntityHandle propOrEvent;
				if ((assocDef & 0x1) == 1) {
					propOrEvent = MetadataTokens.PropertyDefinitionHandle(assocDef >> 1);
				} else {
					propOrEvent = MetadataTokens.EventDefinitionHandle(assocDef >> 1);
				}
				return (MetadataTokens.Handle(0x18000000 | (row + 1)), (MethodSemanticsAttributes)(*(ushort*)ptr), MetadataTokens.MethodDefinitionHandle(methodDef), propOrEvent);
			}
		}

		public static IEnumerable<EntityHandle> GetFieldLayouts(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.FieldLayout);
			for (int row = 1; row <= rowCount; row++) {
				yield return MetadataTokens.EntityHandle(TableIndex.FieldLayout, row);
			}
		}

		public unsafe static (int Offset, FieldDefinitionHandle FieldDef) GetFieldLayout(this MetadataReader metadata, EntityHandle fieldLayoutHandle)
		{
			byte* startPointer = metadata.MetadataPointer;
			int offset = metadata.GetTableMetadataOffset(TableIndex.FieldLayout);
			int rowSize = metadata.GetTableRowSize(TableIndex.FieldLayout);
			int rowCount = metadata.GetTableRowCount(TableIndex.FieldLayout);
			
			int fieldRowNo = metadata.GetRowNumber(fieldLayoutHandle);
			bool small = metadata.GetTableRowCount(TableIndex.Field) <= ushort.MaxValue;
			for (int row = rowCount - 1; row >= 0; row--) {
				byte* ptr = startPointer + offset + rowSize * row;
				uint rowNo = small ? *(ushort*)(ptr + 4) : *(uint*)(ptr + 4);
				if (fieldRowNo == rowNo) {
					return (*(int*)ptr, MetadataTokens.FieldDefinitionHandle(fieldRowNo));
				}
			}
			return (0, default);
		}
	}
}
