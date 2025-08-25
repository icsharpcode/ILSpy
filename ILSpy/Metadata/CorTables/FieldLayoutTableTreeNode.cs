// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class FieldLayoutTableTreeNode : MetadataTableTreeNode<FieldLayoutTableTreeNode.FieldLayoutEntry>
	{
		public FieldLayoutTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldLayout, metadataFile)
		{
		}

		protected override IReadOnlyList<FieldLayoutEntry> LoadTable()
		{
			var list = new List<FieldLayoutEntry>();

			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.FieldLayout);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			int fieldDefSize = metadata.GetTableRowCount(TableIndex.Field) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				list.Add(new FieldLayoutEntry(metadataFile, ptr, rid, fieldDefSize));
			}
			return list;
		}

		readonly struct FieldLayout
		{
			public readonly int Offset;
			public readonly FieldDefinitionHandle Field;

			public FieldLayout(ReadOnlySpan<byte> ptr, int fieldDefSize)
			{
				Offset = BinaryPrimitives.ReadInt32LittleEndian(ptr);
				Field = MetadataTokens.FieldDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(4, fieldDefSize)));
			}
		}

		internal struct FieldLayoutEntry
		{
			readonly MetadataFile metadataFile;
			readonly FieldLayout fieldLayout;

			public int RID { get; }
			public int Token => 0x10000000 | RID;
			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Field => MetadataTokens.GetToken(fieldLayout.Field);

			public void OnFieldClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, fieldLayout.Field, protocol: "metadata")));
			}

			string fieldTooltip;
			public string FieldTooltip => GenerateTooltip(ref fieldTooltip, metadataFile, fieldLayout.Field);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public int FieldOffset => fieldLayout.Offset;

			public FieldLayoutEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row, int fieldDefSize)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.FieldLayout)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.FieldLayout) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				this.fieldLayout = new FieldLayout(ptr.Slice(rowOffset), fieldDefSize);
				this.fieldTooltip = null;
			}
		}
	}
}
