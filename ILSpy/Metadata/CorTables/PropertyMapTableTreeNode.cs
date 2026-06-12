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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the PropertyMap table — pairs each TypeDef that declares properties with the
	/// first row of its property list. Same shape as EventMap but for properties.
	/// </summary>
	public sealed class PropertyMapTableTreeNode : MetadataTableTreeNode<PropertyMapTableTreeNode.PropertyMapEntry>
	{
		public PropertyMapTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.PropertyMap, metadataFile)
		{
		}

		protected override IReadOnlyList<PropertyMapEntry> LoadTable()
		{
			var list = new List<PropertyMapEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.PropertyMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.PropertyMap);
			int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
			int propertyDefSize = metadata.GetTableRowCount(TableIndex.Property) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int propertyListRow = propertyDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new PropertyMapEntry(metadataFile, rid, MetadataTokens.TypeDefinitionHandle(parentRow), MetadataTokens.PropertyDefinitionHandle(propertyListRow)));
			}
			return list;
		}

		public sealed class PropertyMapEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x15000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, parent);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int PropertyList => MetadataTokens.GetToken(propertyList);

			string? propertyListTooltip;
			public string? PropertyListTooltip => GenerateTooltip(ref propertyListTooltip, metadataFile, propertyList);

			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle parent;
			readonly PropertyDefinitionHandle propertyList;

			public PropertyMapEntry(MetadataFile metadataFile, int rid, TypeDefinitionHandle parent, PropertyDefinitionHandle propertyList)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.parent = parent;
				this.propertyList = propertyList;
			}
		}
	}
}
