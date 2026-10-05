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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class RawMetadataExtensions
	{
		/// <summary>
		/// Enumerates the StateMachineMethod table of a Portable PDB, yielding the MoveNext
		/// method and the kickoff method of each row. SRM exposes the table only through
		/// <c>MethodDebugInformation.GetStateMachineKickoffMethod</c>, which cannot enumerate
		/// the rows, so the columns are read directly.
		/// </summary>
		public static IEnumerable<(MethodDefinitionHandle MoveNextMethod, MethodDefinitionHandle KickoffMethod)> GetStateMachineMethods(this MetadataReader metadata)
		{
			int rowCount = metadata.GetTableRowCount(TableIndex.StateMachineMethod);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod);
			int methodSize = metadata.GetTableRowSize(TableIndex.StateMachineMethod) / 2;
			CheckRowSize(metadata, TableIndex.StateMachineMethod, 2 * methodSize);
			for (int rid = 1; rid <= rowCount; rid++)
			{
				int moveNextRow = methodSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int kickoffRow = methodSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				yield return (MetadataTokens.MethodDefinitionHandle(moveNextRow),
					MetadataTokens.MethodDefinitionHandle(kickoffRow));
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

		public static IEnumerable<(MethodImportAttributes MappingFlags, EntityHandle MemberForwarded, StringHandle ImportName, ModuleReferenceHandle ImportScope)> GetImplMaps(this MetadataReader metadata)
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
				var mappingFlags = (MethodImportAttributes)reader.ReadUInt16();
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
	}
}
