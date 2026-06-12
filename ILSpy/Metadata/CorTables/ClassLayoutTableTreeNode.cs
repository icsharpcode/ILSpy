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
	/// View of the ClassLayout table — explicit field-layout overrides on TypeDefs marked
	/// Sequential or Explicit (e.g. <c>[StructLayout(LayoutKind.Explicit)]</c>). System.Reflection.Metadata
	/// doesn't surface these rows through a typed enumeration so we walk the table bytes
	/// directly via a BlobReader.
	/// </summary>
	public sealed class ClassLayoutTableTreeNode : MetadataTableTreeNode<ClassLayoutTableTreeNode.ClassLayoutEntry>
	{
		public ClassLayoutTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ClassLayout, metadataFile)
		{
		}

		protected override IReadOnlyList<ClassLayoutEntry> LoadTable()
		{
			var list = new List<ClassLayoutEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.ClassLayout);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.ClassLayout);
			int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				ushort packingSize = reader.ReadUInt16();
				uint classSize = reader.ReadUInt32();
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new ClassLayoutEntry(metadataFile, rid, packingSize, classSize, MetadataTokens.TypeDefinitionHandle(parentRow)));
			}
			return list;
		}

		public sealed class ClassLayoutEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x0F000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, parent);

			public ushort PackingSize { get; }

			public uint ClassSize { get; }

			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle parent;

			public ClassLayoutEntry(MetadataFile metadataFile, int rid, ushort packingSize, uint classSize, TypeDefinitionHandle parent)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				PackingSize = packingSize;
				ClassSize = classSize;
				this.parent = parent;
			}
		}
	}
}
