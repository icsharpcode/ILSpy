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

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class MetadataExtensions
	{
		static HashAlgorithm GetHashAlgorithm(this MetadataReader reader)
		{
			return reader.GetAssemblyDefinition().HashAlgorithm switch {
				AssemblyHashAlgorithm.None =>
					// only for multi-module assemblies?
					SHA1.Create(),
				AssemblyHashAlgorithm.MD5 => MD5.Create(),
				AssemblyHashAlgorithm.Sha1 => SHA1.Create(),
				AssemblyHashAlgorithm.Sha256 => SHA256.Create(),
				AssemblyHashAlgorithm.Sha384 => SHA384.Create(),
				AssemblyHashAlgorithm.Sha512 => SHA512.Create(),
				_ => SHA1.Create()
			};
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

		public static string GetPublicKeyToken(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = "null";
			if (!asm.PublicKey.IsNil)
			{
				// AssemblyFlags.PublicKey does not apply to assembly definitions
				publicKey = CalculatePublicKeyToken(asm.PublicKey, reader);
			}
			return publicKey;
		}

		public static string GetFullAssemblyName(this MetadataReader reader)
		{
			if (!reader.IsAssembly)
				return string.Empty;
			var asm = reader.GetAssemblyDefinition();
			string publicKey = reader.GetPublicKeyToken();
			return $"{reader.GetString(asm.Name)}, " +
				$"Version={asm.Version}, " +
				$"Culture={(asm.Culture.IsNil ? "neutral" : reader.GetString(asm.Culture))}, " +
				$"PublicKeyToken={publicKey}";
		}

		public static bool TryGetFullAssemblyName(this MetadataReader reader, out string assemblyName)
		{
			try
			{
				assemblyName = GetFullAssemblyName(reader);
				return true;
			}
			catch (BadImageFormatException)
			{
				assemblyName = null;
				return false;
			}
		}

		public static string GetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader)
		{
			StringBuilder builder = new();
			builder.Append(reader.GetString(reference.Name));
			builder.Append(", Version=");
			builder.Append(reference.Version);
			builder.Append(", Culture=");
			if (reference.Culture.IsNil)
			{
				builder.Append("neutral");
			}
			else
			{
				builder.Append(reader.GetString(reference.Culture));
			}

			if (reference.PublicKeyOrToken.IsNil)
			{
				builder.Append(", PublicKeyToken=null");
			}
			else if ((reference.Flags & AssemblyFlags.PublicKey) != 0)
			{
				builder.Append(", PublicKeyToken=");
				builder.Append(CalculatePublicKeyToken(reference.PublicKeyOrToken, reader));
			}
			else
			{
				builder.Append(", PublicKeyToken=");
				builder.AppendHexString(reader.GetBlobReader(reference.PublicKeyOrToken));
			}
			if ((reference.Flags & AssemblyFlags.Retargetable) != 0)
			{
				builder.Append(", Retargetable=true");
			}
			return builder.ToString();
		}

		public static bool TryGetFullAssemblyName(this SRM.AssemblyReference reference, MetadataReader reader, out string assemblyName)
		{
			try
			{
				assemblyName = GetFullAssemblyName(reference, reader);
				return true;
			}
			catch (BadImageFormatException)
			{
				assemblyName = null;
				return false;
			}
		}

		public static string ToHexString(this IEnumerable<byte> bytes, int estimatedLength)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			StringBuilder sb = new(estimatedLength * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}

		public static void AppendHexString(this StringBuilder builder, BlobReader reader)
		{
			for (int i = 0; i < reader.Length; i++)
			{
				builder.AppendFormat("{0:x2}", reader.ReadByte());
			}
		}

		public static string ToHexString(this BlobReader reader)
		{
			StringBuilder sb = new(reader.Length * 3);
			for (int i = 0; i < reader.Length; i++)
			{
				if (i == 0)
					sb.AppendFormat("{0:X2}", reader.ReadByte());
				else
					sb.AppendFormat("-{0:X2}", reader.ReadByte());
			}
			return sb.ToString();
		}

		public static IEnumerable<TypeDefinitionHandle> GetTopLevelTypeDefinitions(this MetadataReader reader)
		{
			foreach (var handle in reader.TypeDefinitions)
			{
				var td = reader.GetTypeDefinition(handle);
				if (td.GetDeclaringType().IsNil)
					yield return handle;
			}
		}

		public static string ToILNameString(this FullTypeName typeName, bool omitGenerics = false)
		{
			string name;
			if (typeName.IsNested)
			{
				name = typeName.Name;
				if (!omitGenerics)
				{
					int localTypeParameterCount = typeName.GetNestedTypeAdditionalTypeParameterCount(typeName.NestingLevel - 1);
					if (localTypeParameterCount > 0)
						name += "`" + localTypeParameterCount;
				}
				name = Disassembler.DisassemblerHelpers.Escape(name);
				return $"{typeName.GetDeclaringType().ToILNameString(omitGenerics)}/{name}";
			}
			if (!string.IsNullOrEmpty(typeName.TopLevelTypeName.Namespace))
			{
				name = $"{typeName.TopLevelTypeName.Namespace}.{typeName.Name}";
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			}
			else
			{
				name = typeName.Name;
				if (!omitGenerics && typeName.TypeParameterCount > 0)
					name += "`" + typeName.TypeParameterCount;
			}
			return Disassembler.DisassemblerHelpers.Escape(name);
		}

		[Obsolete("Use MetadataModule.GetDeclaringModule() instead")]
		public static IModuleReference GetDeclaringModule(this TypeReferenceHandle handle, MetadataReader reader)
		{
			var tr = reader.GetTypeReference(handle);
			switch (tr.ResolutionScope.Kind)
			{
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
			new(new SimpleCompilation(MinimalCorlib.Instance));

		/// <summary>
		/// An attribute type provider that can be used to decode attribute signatures
		/// that only mention built-in types.
		/// </summary>
		public static ICustomAttributeTypeProvider<IType> MinimalAttributeTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		public static ISignatureTypeProvider<IType, GenericContext> MinimalSignatureTypeProvider {
			get => minimalCorlibTypeProvider;
		}

		/// <summary>
		/// Converts <see cref="KnownTypeCode"/> to <see cref="PrimitiveTypeCode"/>.
		/// Returns 0 for known types that are not primitive types (such as <see cref="Span{T}"/>).
		/// </summary>
		public static PrimitiveTypeCode ToPrimitiveTypeCode(this KnownTypeCode typeCode)
		{
			return typeCode switch {
				KnownTypeCode.Object => PrimitiveTypeCode.Object,
				KnownTypeCode.Boolean => PrimitiveTypeCode.Boolean,
				KnownTypeCode.Char => PrimitiveTypeCode.Char,
				KnownTypeCode.SByte => PrimitiveTypeCode.SByte,
				KnownTypeCode.Byte => PrimitiveTypeCode.Byte,
				KnownTypeCode.Int16 => PrimitiveTypeCode.Int16,
				KnownTypeCode.UInt16 => PrimitiveTypeCode.UInt16,
				KnownTypeCode.Int32 => PrimitiveTypeCode.Int32,
				KnownTypeCode.UInt32 => PrimitiveTypeCode.UInt32,
				KnownTypeCode.Int64 => PrimitiveTypeCode.Int64,
				KnownTypeCode.UInt64 => PrimitiveTypeCode.UInt64,
				KnownTypeCode.Single => PrimitiveTypeCode.Single,
				KnownTypeCode.Double => PrimitiveTypeCode.Double,
				KnownTypeCode.String => PrimitiveTypeCode.String,
				KnownTypeCode.Void => PrimitiveTypeCode.Void,
				KnownTypeCode.TypedReference => PrimitiveTypeCode.TypedReference,
				KnownTypeCode.IntPtr => PrimitiveTypeCode.IntPtr,
				KnownTypeCode.UIntPtr => PrimitiveTypeCode.UIntPtr,
				_ => 0
			};
		}

		public static KnownTypeCode ToKnownTypeCode(this PrimitiveTypeCode typeCode)
		{
			return typeCode switch {
				PrimitiveTypeCode.Boolean => KnownTypeCode.Boolean,
				PrimitiveTypeCode.Byte => KnownTypeCode.Byte,
				PrimitiveTypeCode.SByte => KnownTypeCode.SByte,
				PrimitiveTypeCode.Char => KnownTypeCode.Char,
				PrimitiveTypeCode.Int16 => KnownTypeCode.Int16,
				PrimitiveTypeCode.UInt16 => KnownTypeCode.UInt16,
				PrimitiveTypeCode.Int32 => KnownTypeCode.Int32,
				PrimitiveTypeCode.UInt32 => KnownTypeCode.UInt32,
				PrimitiveTypeCode.Int64 => KnownTypeCode.Int64,
				PrimitiveTypeCode.UInt64 => KnownTypeCode.UInt64,
				PrimitiveTypeCode.Single => KnownTypeCode.Single,
				PrimitiveTypeCode.Double => KnownTypeCode.Double,
				PrimitiveTypeCode.IntPtr => KnownTypeCode.IntPtr,
				PrimitiveTypeCode.UIntPtr => KnownTypeCode.UIntPtr,
				PrimitiveTypeCode.Object => KnownTypeCode.Object,
				PrimitiveTypeCode.String => KnownTypeCode.String,
				PrimitiveTypeCode.TypedReference => KnownTypeCode.TypedReference,
				PrimitiveTypeCode.Void => KnownTypeCode.Void,
				_ => KnownTypeCode.None
			};
		}

		public static IEnumerable<ModuleReferenceHandle> GetModuleReferences(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.ModuleRef);
			for (int row = 1; row <= rowCount; row++)
			{
				yield return MetadataTokens.ModuleReferenceHandle(row);
			}
		}

		public static IEnumerable<TypeSpecificationHandle> GetTypeSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.TypeSpec);
			for (int row = 1; row <= rowCount; row++)
			{
				yield return MetadataTokens.TypeSpecificationHandle(row);
			}
		}

		public static IEnumerable<MethodSpecificationHandle> GetMethodSpecifications(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.MethodSpec);
			for (int row = 1; row <= rowCount; row++)
			{
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
			for (int row = 0; row < rowCount; row++)
			{
				yield return Read(row);
			}

			unsafe (Handle Handle, MethodSemanticsAttributes Semantics, MethodDefinitionHandle Method, EntityHandle Association) Read(int row)
			{
				byte* ptr = metadata.MetadataPointer + offset + rowSize * row;
				int methodDef = methodSmall ? *(ushort*)(ptr + 2) : (int)*(uint*)(ptr + 2);
				int assocDef = assocSmall ? *(ushort*)(ptr + assocOffset) : (int)*(uint*)(ptr + assocOffset);
				EntityHandle propOrEvent;
				if ((assocDef & 0x1) == 1)
				{
					propOrEvent = MetadataTokens.PropertyDefinitionHandle(assocDef >> 1);
				}
				else
				{
					propOrEvent = MetadataTokens.EventDefinitionHandle(assocDef >> 1);
				}
				return (MetadataTokens.Handle(0x18000000 | (row + 1)), (MethodSemanticsAttributes)(*(ushort*)ptr), MetadataTokens.MethodDefinitionHandle(methodDef), propOrEvent);
			}
		}

		public static IEnumerable<EntityHandle> GetFieldLayouts(this MetadataReader metadata)
		{
			var rowCount = metadata.GetTableRowCount(TableIndex.FieldLayout);
			for (int row = 1; row <= rowCount; row++)
			{
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
			for (int row = rowCount - 1; row >= 0; row--)
			{
				byte* ptr = startPointer + offset + rowSize * row;
				uint rowNo = small ? *(ushort*)(ptr + 4) : *(uint*)(ptr + 4);
				if (fieldRowNo == rowNo)
				{
					return (*(int*)ptr, MetadataTokens.FieldDefinitionHandle(fieldRowNo));
				}
			}
			return (0, default);
		}
	}
}
