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
	internal class FieldRVATableTreeNode : MetadataTableTreeNode
	{
		public FieldRVATableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldRva, metadataFile)
		{
		}

		public override object Text => $"1D FieldRVA ({metadataFile.Metadata.GetTableRowCount(TableIndex.FieldRva)})";

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<FieldRVAEntry>();
			FieldRVAEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.FieldRva);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			int metadataOffset = metadataFile.MetadataOffset;
			for (int rid = 1; rid <= length; rid++)
			{
				FieldRVAEntry entry = new FieldRVAEntry(metadataFile, metadataOffset, ptr, rid);
				if (entry.RID == this.scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		readonly struct FieldRVA
		{
			public readonly int Offset;
			public readonly FieldDefinitionHandle Field;

			public FieldRVA(ReadOnlySpan<byte> ptr, int fieldDefSize)
			{
				Offset = BinaryPrimitives.ReadInt32LittleEndian(ptr);
				Field = MetadataTokens.FieldDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(4, fieldDefSize)));
			}
		}

		struct FieldRVAEntry
		{
			readonly MetadataFile metadataFile;
			readonly FieldRVA fieldRVA;

			public int RID { get; }

			public int Token => 0x1D000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Field => MetadataTokens.GetToken(fieldRVA.Field);

			public void OnFieldClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, fieldRVA.Field, protocol: "metadata")));
			}

			string fieldTooltip;
			public string FieldTooltip => GenerateTooltip(ref fieldTooltip, metadataFile, fieldRVA.Field);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public int FieldOffset => fieldRVA.Offset;

			public FieldRVAEntry(MetadataFile metadataFile, int metadataOffset, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.FieldRva)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.FieldRva) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				int fieldDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.Field) < ushort.MaxValue ? 2 : 4;
				this.fieldRVA = new FieldRVA(ptr.Slice(rowOffset), fieldDefSize);
				this.fieldTooltip = null;
			}
		}
	}
}
