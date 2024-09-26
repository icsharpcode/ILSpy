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
	internal class FieldLayoutTableTreeNode : MetadataTableTreeNode
	{
		public FieldLayoutTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldLayout, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<FieldLayoutEntry>();
			FieldLayoutEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.FieldLayout);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				FieldLayoutEntry entry = new FieldLayoutEntry(metadataFile, ptr, rid);
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

		struct FieldLayoutEntry
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

			public FieldLayoutEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.FieldLayout)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.FieldLayout) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				int fieldDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.Field) < ushort.MaxValue ? 2 : 4;
				this.fieldLayout = new FieldLayout(ptr.Slice(rowOffset), fieldDefSize);
				this.fieldTooltip = null;
			}
		}
	}
}
