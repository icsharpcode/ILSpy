// Copyright (c) 2018 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class MetadataExtensions
	{
		static string CalculatePublicKeyToken(BlobHandle blob, MetadataReader reader)
		{
			// Calculate public key token:
			// 1. hash the public key (the strong-name format mandates SHA-1; a managed
			// implementation is used so this works under restrictive crypto policies).
			byte[] publicKeyTokenBytes = new byte[20];
			Sha1ForNonSecretPurposes.HashData(reader.GetBlobBytes(blob), publicKeyTokenBytes);

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
			StringBuilder builder = new StringBuilder();
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

			StringBuilder sb = new StringBuilder(estimatedLength * 2);
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
			StringBuilder sb = new StringBuilder(reader.Length * 3);
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

		/// <summary>
		/// Converts <see cref="KnownTypeCode"/> to <see cref="PrimitiveTypeCode"/>.
		/// Returns 0 for known types that are not primitive types (such as <see cref="Span{T}"/>).
		/// </summary>
		public static PrimitiveTypeCode ToPrimitiveTypeCode(this KnownTypeCode typeCode)
		{
			switch (typeCode)
			{
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
					return 0;
			}
		}

		public static KnownTypeCode ToKnownTypeCode(this PrimitiveTypeCode typeCode)
		{
			switch (typeCode)
			{
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
			int rowCount = metadata.GetTableRowCount(TableIndex.MethodSemantics);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.MethodSemantics);

			int methodSize = SimpleIndexSize(metadata, TableIndex.MethodDef);
			// HasSemantics coded index: 1 tag bit over Event (tag 0) and Property (tag 1).
			int assocSize = CodedIndexSize(metadata, 1, TableIndex.Event, TableIndex.Property);
			CheckRowSize(metadata, TableIndex.MethodSemantics, 2 + methodSize + assocSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				var semantics = (MethodSemanticsAttributes)reader.ReadUInt16();
				int methodRow = methodSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int assocTag = assocSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				EntityHandle propOrEvent;
				if ((assocTag & 0x1) == 1)
				{
					propOrEvent = MetadataTokens.PropertyDefinitionHandle(assocTag >> 1);
				}
				else
				{
					propOrEvent = MetadataTokens.EventDefinitionHandle(assocTag >> 1);
				}
				yield return (MetadataTokens.Handle(((int)TableIndex.MethodSemantics << 24) | rid), semantics, MetadataTokens.MethodDefinitionHandle(methodRow), propOrEvent);
			}
		}

		/// <summary>
		/// Size in bytes of a column indexing <paramref name="table"/> directly
		/// (ECMA-335 II.24.2.6: 2 bytes while the table has fewer than 2^16 rows).
		/// </summary>
		static int SimpleIndexSize(MetadataReader metadata, TableIndex table)
		{
			return metadata.GetTableRowCount(table) < (1 << 16) ? 2 : 4;
		}

		/// <summary>
		/// Size in bytes of a coded-index column over <paramref name="tables"/> with
		/// <paramref name="tagBits"/> tag bits (ECMA-335 II.24.2.6: 2 bytes while every
		/// indexed table has fewer than 2^(16 - tagBits) rows).
		/// </summary>
		static int CodedIndexSize(MetadataReader metadata, int tagBits, params TableIndex[] tables)
		{
			int smallLimit = 1 << (16 - tagBits);
			foreach (var table in tables)
			{
				if (metadata.GetTableRowCount(table) >= smallLimit)
					return 4;
			}
			return 2;
		}

		/// <summary>
		/// Size in bytes of the FieldList/MethodList/ParamList member-range columns. SRM's rule
		/// (MetadataReader.InitializeTableReaders): the column is wide when the *Ptr table itself
		/// is large; otherwise the member table's row count governs the width, whether or not the
		/// indirection is present.
		/// </summary>
		static int ListColumnSize(MetadataReader metadata, TableIndex ptrTable, TableIndex memberTable)
		{
			return SimpleIndexSize(metadata, ptrTable) > 2 ? 4 : SimpleIndexSize(metadata, memberTable);
		}

		/// <summary>
		/// Size in bytes of a #Strings heap index column. The width is declared by the HeapSizes
		/// flags in the tables-stream header, not derived from the heap's size (a producer may set
		/// the large-heap flag over a small heap). SRM does not expose the flags, but the ModuleRef
		/// row consists of a single String column, so its computed row size is exactly this width.
		/// </summary>
		static int StringIndexSize(MetadataReader metadata)
		{
			return metadata.GetTableRowSize(TableIndex.ModuleRef);
		}

		/// <summary>
		/// Size in bytes of a #Blob heap index column (see <see cref="StringIndexSize"/>; the
		/// TypeSpec row consists of a single Blob column).
		/// </summary>
		static int BlobIndexSize(MetadataReader metadata)
		{
			return metadata.GetTableRowSize(TableIndex.TypeSpec);
		}

		/// <summary>
		/// Verifies that the computed column widths sum to the row size SRM derived from the
		/// tables-stream header. A width bug reads plausible-looking garbage rather than failing,
		/// and some width rules are not reproducible through public API at all (EnC minimal-delta
		/// metadata forces every table and coded index to 4 bytes), so fail loudly on mismatch.
		/// </summary>
		static void CheckRowSize(MetadataReader metadata, TableIndex table, int computedRowSize)
		{
			int actualRowSize = metadata.GetTableRowSize(table);
			if (computedRowSize != actualRowSize)
				throw new BadImageFormatException($"Unexpected {table} row size: computed {computedRowSize}, actual {actualRowSize}.");
		}

		public static IEnumerable<(int PackingSize, uint ClassSize, TypeDefinitionHandle Parent)> GetClassLayouts(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.ClassLayout);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.ClassLayout);
			int typeDefSize = SimpleIndexSize(metadata, TableIndex.TypeDef);
			CheckRowSize(metadata, TableIndex.ClassLayout, 2 + 4 + typeDefSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				ushort packingSize = reader.ReadUInt16();
				uint classSize = reader.ReadUInt32();
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (packingSize, classSize, MetadataTokens.TypeDefinitionHandle(parentRow));
			}
		}

		public static IEnumerable<(int Offset, FieldDefinitionHandle Field)> GetFieldLayoutRows(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.FieldLayout);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.FieldLayout);
			int fieldSize = SimpleIndexSize(metadata, TableIndex.Field);
			CheckRowSize(metadata, TableIndex.FieldLayout, 4 + fieldSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int offset = reader.ReadInt32();
				int fieldRow = fieldSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (offset, MetadataTokens.FieldDefinitionHandle(fieldRow));
			}
		}

		public static IEnumerable<(TypeDefinitionHandle Parent, EventDefinitionHandle EventList)> GetEventMaps(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.EventMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.EventMap);
			int typeDefSize = SimpleIndexSize(metadata, TableIndex.TypeDef);
			int eventListSize = ListColumnSize(metadata, TableIndex.EventPtr, TableIndex.Event);
			CheckRowSize(metadata, TableIndex.EventMap, typeDefSize + eventListSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int eventListRow = eventListSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (MetadataTokens.TypeDefinitionHandle(parentRow), MetadataTokens.EventDefinitionHandle(eventListRow));
			}
		}

		public static IEnumerable<(TypeDefinitionHandle Parent, PropertyDefinitionHandle PropertyList)> GetPropertyMaps(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.PropertyMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.PropertyMap);
			int typeDefSize = SimpleIndexSize(metadata, TableIndex.TypeDef);
			int propertyListSize = ListColumnSize(metadata, TableIndex.PropertyPtr, TableIndex.Property);
			CheckRowSize(metadata, TableIndex.PropertyMap, typeDefSize + propertyListSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int propertyListRow = propertyListSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (MetadataTokens.TypeDefinitionHandle(parentRow), MetadataTokens.PropertyDefinitionHandle(propertyListRow));
			}
		}

		public static IEnumerable<(TypeDefinitionHandle NestedClass, TypeDefinitionHandle EnclosingClass)> GetNestedClasses(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.NestedClass);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.NestedClass);
			int typeDefSize = SimpleIndexSize(metadata, TableIndex.TypeDef);
			CheckRowSize(metadata, TableIndex.NestedClass, 2 * typeDefSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int nestedRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int enclosingRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (MetadataTokens.TypeDefinitionHandle(nestedRow), MetadataTokens.TypeDefinitionHandle(enclosingRow));
			}
		}

		public static IEnumerable<(int RelativeVirtualAddress, FieldDefinitionHandle Field)> GetFieldRvas(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.FieldRva);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.FieldRva);
			int fieldSize = SimpleIndexSize(metadata, TableIndex.Field);
			CheckRowSize(metadata, TableIndex.FieldRva, 4 + fieldSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int rva = reader.ReadInt32();
				int fieldRow = fieldSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (rva, MetadataTokens.FieldDefinitionHandle(fieldRow));
			}
		}

		public static IEnumerable<(EntityHandle Parent, BlobHandle NativeType)> GetFieldMarshals(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.FieldMarshal);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.FieldMarshal);
			// HasFieldMarshal coded index: 1 tag bit over Field (tag 0) and Param (tag 1).
			int parentSize = CodedIndexSize(metadata, 1, TableIndex.Field, TableIndex.Param);
			int blobSize = BlobIndexSize(metadata);
			CheckRowSize(metadata, TableIndex.FieldMarshal, parentSize + blobSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int parentTag = parentSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int blobOffset = blobSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				EntityHandle parent = (parentTag & 0x1) == 1
					? MetadataTokens.ParameterHandle(parentTag >> 1)
					: MetadataTokens.FieldDefinitionHandle(parentTag >> 1);
				yield return (parent, MetadataTokens.BlobHandle(blobOffset));
			}
		}

		public static IEnumerable<(System.Reflection.MethodImportAttributes MappingFlags, EntityHandle MemberForwarded, StringHandle ImportName, ModuleReferenceHandle ImportScope)> GetImplMaps(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.ImplMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.ImplMap);
			// MemberForwarded coded index: 1 tag bit over Field (tag 0) and MethodDef (tag 1).
			int memberForwardedSize = CodedIndexSize(metadata, 1, TableIndex.Field, TableIndex.MethodDef);
			int stringSize = StringIndexSize(metadata);
			int moduleRefSize = SimpleIndexSize(metadata, TableIndex.ModuleRef);
			CheckRowSize(metadata, TableIndex.ImplMap, 2 + memberForwardedSize + stringSize + moduleRefSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				var mappingFlags = (System.Reflection.MethodImportAttributes)reader.ReadUInt16();
				int memberForwardedTag = memberForwardedSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int importNameOffset = stringSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int importScopeRow = moduleRefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				EntityHandle memberForwarded = (memberForwardedTag & 0x1) == 1
					? MetadataTokens.MethodDefinitionHandle(memberForwardedTag >> 1)
					: MetadataTokens.FieldDefinitionHandle(memberForwardedTag >> 1);
				yield return (mappingFlags, memberForwarded, MetadataTokens.StringHandle(importNameOffset), MetadataTokens.ModuleReferenceHandle(importScopeRow));
			}
		}

		public static IEnumerable<(TypeDefinitionHandle Class, EntityHandle Interface)> GetInterfaceImplRows(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.InterfaceImpl);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.InterfaceImpl);
			int typeDefSize = SimpleIndexSize(metadata, TableIndex.TypeDef);
			// TypeDefOrRef coded index: 2 tag bits over TypeDef (0), TypeRef (1) and TypeSpec (2).
			int interfaceSize = CodedIndexSize(metadata, 2, TableIndex.TypeDef, TableIndex.TypeRef, TableIndex.TypeSpec);
			CheckRowSize(metadata, TableIndex.InterfaceImpl, typeDefSize + interfaceSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int classRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int interfaceTag = interfaceSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				EntityHandle iface = (interfaceTag & 0x3) switch {
					0 => MetadataTokens.TypeDefinitionHandle(interfaceTag >> 2),
					1 => MetadataTokens.TypeReferenceHandle(interfaceTag >> 2),
					_ => MetadataTokens.TypeSpecificationHandle(interfaceTag >> 2),
				};
				yield return (MetadataTokens.TypeDefinitionHandle(classRow), iface);
			}
		}

		/// <summary>
		/// Enumerates one of the five *Ptr indirection tables (EventPtr, FieldPtr, MethodPtr,
		/// ParamPtr, PropertyPtr), yielding the referenced entity of each row.
		/// </summary>
		public static IEnumerable<EntityHandle> GetPtrRows(this MetadataReader metadata, TableIndex ptrTable)
		{
			TableIndex referencedTable = ptrTable switch {
				TableIndex.EventPtr => TableIndex.Event,
				TableIndex.FieldPtr => TableIndex.Field,
				TableIndex.MethodPtr => TableIndex.MethodDef,
				TableIndex.ParamPtr => TableIndex.Param,
				TableIndex.PropertyPtr => TableIndex.Property,
				_ => throw new ArgumentOutOfRangeException(nameof(ptrTable)),
			};
			int rowCount = metadata.GetTableRowCount(ptrTable);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(ptrTable);
			int handleSize = SimpleIndexSize(metadata, referencedTable);
			CheckRowSize(metadata, ptrTable, handleSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int row = handleSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return MetadataTokens.EntityHandle(((int)referencedTable << 24) | row);
			}
		}

		/// <summary>
		/// Reads the FieldList/MethodList columns of the TypeDef table, which SRM does not expose:
		/// the stored value is the running list position (the next type's first member row, or one
		/// past the member table's end), never 0. The columns sit at the end of the row, so they are
		/// read relative to the row end to avoid re-deriving the widths of the preceding columns.
		/// </summary>
		public static IEnumerable<(TypeDefinitionHandle Type, int FieldList, int MethodList)> GetTypeDefListColumns(this MetadataReader metadata)
		{
			int fieldListSize = ListColumnSize(metadata, TableIndex.FieldPtr, TableIndex.Field);
			int methodListSize = ListColumnSize(metadata, TableIndex.MethodPtr, TableIndex.MethodDef);
			// Flags + Name + Namespace + Extends (TypeDefOrRef coded index) precede the list columns.
			CheckRowSize(metadata, TableIndex.TypeDef,
				4 + 2 * StringIndexSize(metadata)
				+ CodedIndexSize(metadata, 2, TableIndex.TypeDef, TableIndex.TypeRef, TableIndex.TypeSpec)
				+ fieldListSize + methodListSize);
			int rowSize = metadata.GetTableRowSize(TableIndex.TypeDef);
			int tableOffset = metadata.GetTableMetadataOffset(TableIndex.TypeDef);
			var reader = metadata.AsBlobReader();
			foreach (var handle in metadata.TypeDefinitions)
			{
				reader.Offset = tableOffset + rowSize * MetadataTokens.GetRowNumber(handle) - fieldListSize - methodListSize;
				int fieldList = fieldListSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int methodList = methodListSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (handle, fieldList, methodList);
			}
		}

		/// <summary>
		/// Reads the ParamList column of the MethodDef table (see <see cref="GetTypeDefListColumns"/>
		/// for why SRM cannot provide it).
		/// </summary>
		public static IEnumerable<(MethodDefinitionHandle Method, int ParamList)> GetMethodDefParamLists(this MetadataReader metadata)
		{
			int paramListSize = ListColumnSize(metadata, TableIndex.ParamPtr, TableIndex.Param);
			// RVA + ImplFlags + Flags + Name + Signature precede the ParamList column.
			CheckRowSize(metadata, TableIndex.MethodDef,
				4 + 2 + 2 + StringIndexSize(metadata) + BlobIndexSize(metadata) + paramListSize);
			int rowSize = metadata.GetTableRowSize(TableIndex.MethodDef);
			int tableOffset = metadata.GetTableMetadataOffset(TableIndex.MethodDef);
			var reader = metadata.AsBlobReader();
			foreach (var handle in metadata.MethodDefinitions)
			{
				reader.Offset = tableOffset + rowSize * MetadataTokens.GetRowNumber(handle) - paramListSize;
				int paramList = paramListSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (handle, paramList);
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

		public static (int Offset, FieldDefinitionHandle FieldDef) GetFieldLayout(this MetadataReader metadata, EntityHandle fieldLayoutHandle)
		{
			var startPointer = metadata.AsReadOnlySpan();
			int offset = metadata.GetTableMetadataOffset(TableIndex.FieldLayout);
			int rowSize = metadata.GetTableRowSize(TableIndex.FieldLayout);
			int rowCount = metadata.GetTableRowCount(TableIndex.FieldLayout);

			int fieldRowNo = metadata.GetRowNumber(fieldLayoutHandle);
			bool small = metadata.GetTableRowCount(TableIndex.Field) <= ushort.MaxValue;
			for (int row = rowCount - 1; row >= 0; row--)
			{
				ReadOnlySpan<byte> ptr = startPointer.Slice(offset + rowSize * row);
				var rowNoSpan = ptr.Slice(4);
				uint rowNo = small ? BinaryPrimitives.ReadUInt16LittleEndian(rowNoSpan) : BinaryPrimitives.ReadUInt32LittleEndian(rowNoSpan);
				if (fieldRowNo == rowNo)
				{
					return (BinaryPrimitives.ReadInt32LittleEndian(ptr), MetadataTokens.FieldDefinitionHandle(fieldRowNo));
				}
			}
			return (0, default);
		}

		public static ReadOnlySpan<byte> AsReadOnlySpan(this MetadataReader metadataReader)
		{
			unsafe
			{
				return new(metadataReader.MetadataPointer, metadataReader.MetadataLength);
			}
		}

		public static BlobReader AsBlobReader(this MetadataReader metadataReader)
		{
			unsafe
			{
				return new(metadataReader.MetadataPointer, metadataReader.MetadataLength);
			}
		}

		public static uint ReadULEB128(this BinaryReader reader)
		{
			uint val = 0;
			int shift = 0;
			while (true)
			{
				byte b = reader.ReadByte();
				val |= (b & 0b0111_1111u) << shift;
				if ((b & 0b1000_0000) == 0)
					break;
				shift += 7;
				if (shift >= 35)
					throw new OverflowException();
			}
			return val;
		}

	}
}
