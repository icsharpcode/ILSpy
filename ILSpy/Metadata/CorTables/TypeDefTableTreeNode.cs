// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the TypeDef table — every type the module defines. The first row is the
	/// pseudo-type &lt;Module&gt;, owning module-scoped fields and methods. Each row carries
	/// attributes (visibility, layout, semantics), the optional base type, and pointers
	/// into the FieldList / MethodList for the type's members.
	/// </summary>
	public sealed class TypeDefTableTreeNode : MetadataTableTreeNode<TypeDefTableTreeNode.TypeDefEntry>
	{
		public TypeDefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeDef, metadataFile)
		{
		}

		protected override IReadOnlyList<TypeDefEntry> LoadTable()
		{
			var list = new List<TypeDefEntry>();
			var metadata = metadataFile.Metadata;
			// FieldList/MethodList are read from the raw rows: the computed member ranges
			// (TypeDefinition.GetFields/GetMethods) are empty for a memberless type, but the
			// stored column value is the running list position (the next type's first member
			// row, or one past the member table's end), never 0. Reading relative to the row
			// end avoids re-deriving the widths of the preceding string-heap and coded-index
			// columns. With a FieldPtr/MethodPtr indirection present, the list columns index
			// the pointer table, whose row count also governs the column width.
			int fieldListWidth = ListColumnWidth(metadata, TableIndex.FieldPtr, TableIndex.Field);
			int methodListWidth = ListColumnWidth(metadata, TableIndex.MethodPtr, TableIndex.MethodDef);
			int rowSize = metadata.GetTableRowSize(TableIndex.TypeDef);
			int tableOffset = metadata.GetTableMetadataOffset(TableIndex.TypeDef);
			var reader = metadata.AsBlobReader();
			foreach (var row in metadata.TypeDefinitions)
			{
				reader.Offset = tableOffset + rowSize * MetadataTokens.GetRowNumber(row) - fieldListWidth - methodListWidth;
				int fieldList = fieldListWidth == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int methodList = methodListWidth == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new TypeDefEntry(metadataFile, row, fieldList, methodList));
			}
			return list;

			static int ListColumnWidth(MetadataReader metadata, TableIndex ptrTable, TableIndex memberTable)
			{
				var indexed = metadata.GetTableRowCount(ptrTable) > 0 ? ptrTable : memberTable;
				return metadata.GetTableRowCount(indexed) <= ushort.MaxValue ? 2 : 4;
			}
		}

		public sealed class TypeDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle handle;
			readonly TypeDefinition typeDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.TypeDef, RID);

			[ColumnInfo("X8")]
			public TypeAttributes Attributes => typeDef.Attributes;

			public object AttributesTooltip => FlagsTooltip.ForTypeAttributes(typeDef.Attributes);

			public string Name => metadataFile.Metadata.GetString(typeDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Name):X} \"{Name}\"";

			public string Namespace => metadataFile.Metadata.GetString(typeDef.Namespace);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Namespace):X} \"{Namespace}\"";

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int BaseType => MetadataTokens.GetToken(typeDef.BaseType);

			public string? BaseTypeTooltip {
				get {
					var output = new PlainTextOutput();
					var provider = new DisassemblerSignatureTypeProvider(metadataFile, output);
					if (typeDef.BaseType.IsNil)
						return null;
					switch (typeDef.BaseType.Kind)
					{
						case HandleKind.TypeDefinition:
							provider.GetTypeFromDefinition(metadataFile.Metadata, (TypeDefinitionHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeReference:
							provider.GetTypeFromReference(metadataFile.Metadata, (TypeReferenceHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeSpecification:
							provider.GetTypeFromSpecification(metadataFile.Metadata, new MetadataGenericContext(default(TypeDefinitionHandle), metadataFile.Metadata), (TypeSpecificationHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						default:
							return null;
					}
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int FieldList => 0x04000000 | fieldList;

			string? fieldListTooltip;
			public string? FieldListTooltip {
				get {
					var @field = typeDef.GetFields().FirstOrDefault();
					if (@field.IsNil)
						return "(type has no fields; the stored value is the start of its empty field list: the next type's first field row, or one past the end of the Field table)";
					return GenerateTooltip(ref fieldListTooltip, metadataFile, @field);
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MethodList => 0x06000000 | methodList;

			string? methodListTooltip;
			public string? MethodListTooltip {
				get {
					var method = typeDef.GetMethods().FirstOrDefault();
					if (method.IsNil)
						return "(type has no methods; the stored value is the start of its empty method list: the next type's first method row, or one past the end of the MethodDef table)";
					return GenerateTooltip(ref methodListTooltip, metadataFile, method);
				}
			}

			readonly int fieldList;
			readonly int methodList;

			public TypeDefEntry(MetadataFile metadataFile, TypeDefinitionHandle handle, int fieldList, int methodList)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.fieldList = fieldList;
				this.methodList = methodList;
				typeDef = metadataFile.Metadata.GetTypeDefinition(handle);
			}
		}
	}
}
